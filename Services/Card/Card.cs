/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.Card
{
    [Export(typeof(ICard))]
    public class Card : ICard
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        // TextID's for Card are reserved at 50200 - 50249

        #region Variables

        private ArrayList cardTypes;

        [Import]
        public IApplication Application { get; set; }

        private IUtility Utility
        {
            get { return this.Application.BusinessLogic.Utility; }
        }

        #endregion

        #region Ctor
        /// <summary>
        /// Constructor.
        /// </summary>
        public Card()
        {
            cardTypes = new ArrayList();
        }
        #endregion

        #region ICard Members

        /// <summary>
        /// In the current implementation, this method includes another method that first retrieves the data of all cards available.
        /// Then it is for each retrieved card type it is tested whether the given cardnumber (with the length as stored in the database)
        /// is within the range:
        /// <code>foreach (ICardInfo cardInfo in cardTypes)(...)</code>
        /// If a valid number range has been found, the card number, the track2 data and the entering method are assigned to a CardInfo object
        /// i.e. 'correctCardType'.
        /// \POSProcesses\Operations\PayCard.Execute() will evaluate, whether it is an international credit cart, a debit card,
        /// a loyality card or a corporate card. An instance of PayCorporateCard is created and it's method RunOperation() called.
        /// (Further background: PayCard.cs is derived from TenderOperation.cs which is derived from Operation.cs.)
        /// \POSProcesses\Operations\PayCorporateCard.Execute() calls then the interface method of
        /// \ServiceInterfaces\CorporateCardInterface\ICorporateCard.cs.
        /// </summary>
        /// <param name="cardInfo">If not null, it contains all data, i.e. card number, card name, checkings data etc.</param>
        public void GetCardType(ref ICardInfo cardInfo)
        {
            if (cardInfo == null)
            {
                throw new ArgumentNullException("cardInfo");
            }

            GetCardType(ref cardInfo, true, null);
        }


        /// <summary>
        /// Prompt to select card type.
        /// </summary>
        /// <param name="cardInfo">If not null, it contains all data, i.e. card number, card name, checkings data etc.</param>
        /// <param name="cardList">The list of card types.</param>
        /// <param name="allowCancel">if set to <c>true</c> [allow cancel].</param>
        public void SelectCardType(ref ICardInfo cardInfo, bool allowCancel, IEnumerable<ICardInfo> cardList)
        {
            using (frmSelectCardType dialog = new frmSelectCardType(cardList, allowCancel))
            {
                this.Application.ApplicationFramework.POSShowForm(dialog);

                if (dialog.DialogResult == System.Windows.Forms.DialogResult.OK)
                {
                    cardInfo = dialog.SelectedCard;
                }
                else
                {
                    cardInfo = null;
                }
            }
        }

        /// <summary>
        /// Find card type for specific card info from database.
        /// </summary>
        /// <param name="cardInfo">The card info (may be null)</param>
        /// <param name="allowCancel">if set to <c>true</c> [allow cancel].</param>
        /// <param name="exclusionList">The list of card types to exlcude from search.</param>
        public void GetCardType(ref ICardInfo cardInfo, bool allowCancel, IEnumerable<CardTypes> exclusionList)
        {
            if (cardInfo == null)
            {
                throw new ArgumentNullException("cardInfo");
            }

            if (cardInfo.CardType == CardTypes.Unknown)
            {
                Nullable<CardTypes> types = null;
                GetAllCardTypes(types);
                ICardInfo correctCardType = FindCorrectCardType(cardInfo, allowCancel, exclusionList);

                if (correctCardType != null)
                {
                    correctCardType.CardNumber = cardInfo.CardNumber;
                    correctCardType.Track1 = cardInfo.Track1;
                    correctCardType.Track2 = cardInfo.Track2;
                    correctCardType.Track3 = cardInfo.Track3;
                    correctCardType.Track4 = cardInfo.Track4;
                    correctCardType.ExpDate = cardInfo.ExpDate;
                    correctCardType.CardEntryType = cardInfo.CardEntryType;
                    correctCardType.Address = cardInfo.Address;
                    correctCardType.ZipCode = cardInfo.ZipCode;
                    correctCardType.SecurityCode = cardInfo.SecurityCode;
                    correctCardType.AuthCode = cardInfo.AuthCode;

                    cardInfo = correctCardType;
                }
            }

        }

        /// <summary>
        /// Checks if entered cardnumber is of a valid length.
        /// </summary>
        /// <param name="cardNumber">The card number as a string.</param>
        /// <returns>Returns true if a card number is of a valid length, else false.</returns>
        public bool IsCardLengthValid(string cardNumber)
        {
            return !string.IsNullOrWhiteSpace(cardNumber);
        }

        /// <summary>
        /// Checks if expiration date is valid
        /// </summary>
        /// <param name="expirationDate">The expiration date in mmYY format</param>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsExpiryDateValid(string expirationDateString)
        {
            bool valueOK = false;

            DateTime expirationDate;
            if (TryParseExpiration(expirationDateString, out expirationDate))
            {
                valueOK = IsExpiryDateValid(expirationDate);
            }

            return valueOK;
        }

        /// <summary>
        /// Checks if expiration date is valid
        /// </summary>
        /// <param name="expirationDate">Expiration date</param>
        /// <returns>True if valid, false otherwise</returns>
        public bool IsExpiryDateValid(DateTime expirationDate)
        {
            return expirationDate >= DateTime.Today;
        }

        /// <summary>
        /// Attempts to parse date string in mmYY format
        /// </summary>
        /// <param name="expirationDateString">String in mmyy format</param>
        /// <param name="expirationDate">Parsed date if successful</param>
        /// <returns></returns>
        public bool TryParseExpiration(string expirationDateString, out DateTime expirationDate)
        {
            bool conversionSuccessful = false;

            // Default to min date
            expirationDate = DateTime.MinValue;

            if (!string.IsNullOrWhiteSpace(expirationDateString) && (expirationDateString.Length == 4))
            {
                // Parse month and year
                int month, year;
                if (int.TryParse(expirationDateString.Substring(0, 2), out month) && int.TryParse(expirationDateString.Substring(2, 2), out year))
                {
                    if (month >= 1 && month <= 12 && year >= 0)
                    {
                        try
                        {
                            // Convert to 4-digit year
                            year = System.Globalization.CultureInfo.CurrentCulture.Calendar.ToFourDigitYear(year);

                            int lastDayOfMonth = System.Globalization.CultureInfo.CurrentCulture.Calendar.GetDaysInMonth(year, month);

                            expirationDate = new DateTime(year, month, lastDayOfMonth);
                            conversionSuccessful = true;
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                        }
                    }
                }
            }

            return conversionSuccessful;
        }

        /// <summary>
        /// Will return a collection of card info from the database specific to the current store for the input tender type id. 
        /// </summary>
        /// <param name="tenderTypeId">input tender type id (e.g. credit cards, loaylty cards, gift cards)</param>
        /// <remarks>
        /// This will return only a distinct collection of cards that can be used by the system. 
        /// The card info does not contain full info (like card number ranges, etc).
        /// </remarks>
        public IEnumerable<ICardInfo> GetAvailableCardsByTenderTypeId(string tenderTypeId)
        {
            List<ICardInfo> cards = new List<ICardInfo>();

            SqlConnection connection = Application.Settings.Database.Connection;

            try
            {
                string queryString = @"SELECT S.CARDTYPEID, C.NAME, S.TENDERTYPEID, 
                                        S.CARDFEE, S.CHECKMODULUS, S.CHECKEXPIREDDATE, 
                                        S.CASHBACKLIMIT, S.PROCESSLOCALLY, 
                                        S.ALLOWMANUALINPUT, S.ENTERFLEETINFO, 
                                        C.CARDTYPES, C.CARDISSUER
                                       FROM RETAILSTORETENDERTYPECARDTABLE S 
                                          INNER JOIN RETAILTENDERTYPECARDTABLE C ON C.CARDTYPEID = S.CARDTYPEID 
                                          INNER JOIN RETAILSTORETABLE RST ON RST.RECID = S.CHANNEL 
                                       WHERE RST.STORENUMBER = @STOREID ";
                
                if (!string.IsNullOrWhiteSpace(tenderTypeId))
                {
                    queryString += "AND S.TENDERTYPEID = @TENDERTYPEID ";
                }

                queryString += "ORDER BY C.NAME";

                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    SqlParameter storeIdParam = command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10);

                    storeIdParam.Value = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId; ;

                    if (!string.IsNullOrWhiteSpace(tenderTypeId))
                    {
                        SqlParameter tenderTypeParam = command.Parameters.Add("@TENDERTYPEID", SqlDbType.NVarChar, 10);
                        tenderTypeParam.Value = tenderTypeId;
                    }

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ICardInfo temp = this.Utility.CreateCardInfo();

                            temp.CardTypeId = Utility.ToString(reader["CARDTYPEID"]);
                            temp.CardName = Utility.ToString(reader["NAME"]);
                            temp.TenderTypeId = Utility.ToString(reader["TENDERTYPEID"]);
                            temp.CardFee = Utility.ToDecimal(reader["CARDFEE"]);
                            temp.ModulusCheck = Utility.ToBool(reader["CHECKMODULUS"]);
                            temp.ExpDateCheck = Utility.ToBool(reader["CHECKEXPIREDDATE"]);
                            temp.ProcessLocally = Utility.ToBool(reader["PROCESSLOCALLY"]);
                            temp.AllowManualInput = Utility.ToBool(reader["ALLOWMANUALINPUT"]);
                            temp.EnterFleetInfo = Utility.ToBool(reader["ENTERFLEETINFO"]);
                            temp.CardType = (CardTypes)Utility.ToInt(reader["CARDTYPES"]);
                            temp.Issuer = Utility.ToString(reader["CARDISSUER"] ?? string.Empty);
                            temp.CashBackLimit = Utility.ToDecimal(reader["CASHBACKLIMIT"]);

                            cards.Add(temp);
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return cards.AsReadOnly();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Finds the type of the correct card.
        /// </summary>
        /// <param name="swipedCard">The swiped card.</param>
        /// <param name="allowCancel">if set to <c>true</c> [allow cancel].</param>
        /// <param name="exclusionList">The list of card types to exlcude from search.</param>
        /// <returns>The card info object.</returns>
        private ICardInfo FindCorrectCardType(ICardInfo swipedCard, bool allowCancel, IEnumerable<CardTypes> exclusionList)
        {
            ICardInfo result = null;

            Int64 binTo = 0;
            Int64 binFrom = 0;
            int binLength = 0;
            Int64 cardSubString = 0;

            List<ICardInfo> matchingCards = new List<ICardInfo>();

            foreach (ICardInfo cardInfo in cardTypes)
            {
                binTo = Convert.ToInt64(cardInfo.BinTo);
                binFrom = Convert.ToInt64(cardInfo.BinFrom);
                binLength = Convert.ToInt32(cardInfo.BinLength);

                string[] track2DataParts = LSRetailPosis.Settings.HardwareProfiles.MSR.Track2Parts(swipedCard.Track2);

                if (track2DataParts.Length > 0 && !string.IsNullOrEmpty(track2DataParts[0]))
                {
                    string cardNumber = track2DataParts[0]; // The first information in track 2 is always the credit card number.

                    if (cardNumber.Length >= binLength)
                    {
                        cardSubString = Convert.ToInt64(cardNumber.Substring(0, binLength));
                    }
                    else
                    {
                        // If the card number is shorter than the mask, just take the card number as is.
                        cardSubString = Convert.ToInt64(cardNumber);
                    }
                }
                else
                {
                    swipedCard.CardNumber = swipedCard.CardNumber.Replace("-", string.Empty);
                    if (swipedCard.CardNumber.Length >= binLength)
                    {
                        cardSubString = Convert.ToInt64(swipedCard.CardNumber.Substring(0, binLength));
                    }
                    else if (swipedCard.CardNumber.Trim().Length != 0)
                    {
                        cardSubString = Convert.ToInt64(swipedCard.CardNumber);
                    }
                    else
                    {
                        cardSubString = 0;
                    }
                }

                if ((binFrom <= cardSubString)
                    && (cardSubString <= binTo)
                    && (exclusionList == null || !exclusionList.Contains(cardInfo.CardType)))
                {
                    // The card number is within this bin series and not in exlustion list
                    matchingCards.Add(cardInfo);
                }
            }

            // Select from matching card types
            if (matchingCards.Count == 1)
            {
                // if there is only one, then just auto-select it
                result = matchingCards[0];
            }
            else if (matchingCards.Count > 1)
            {
                // otherwise, pop UI and prompt the user to select a card type.
                using (frmSelectCardType dialog = new frmSelectCardType(matchingCards, allowCancel))
                {
                    this.Application.ApplicationFramework.POSShowForm(dialog);

                    if (dialog.DialogResult == System.Windows.Forms.DialogResult.OK)
                    {
                        result = dialog.SelectedCard;
                    }
                    else
                    {
                        // User hit cancel
                        result = swipedCard;
                        result.CardType = CardTypes.None;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get All defined card types/masks
        /// </summary>
        /// <param name="types">card types to retrieve</param>
        private void GetAllCardTypes(Nullable<CardTypes> types)
        {
            SqlConnection connection = Application.Settings.Database.Connection;

            try
            {
                cardTypes.Clear();

                string queryString = "SELECT S.CARDTYPEID, S.NAME, S.TENDERTYPEID, S.CARDFEE, S.CHECKMODULUS, S.CHECKEXPIREDDATE, S.CASHBACKLIMIT, S.PROCESSLOCALLY, S.ALLOWMANUALINPUT, S.ENTERFLEETINFO, C.CARDTYPES, C.CARDISSUER, N.CARDNUMBERTO, N.CARDNUMBERFROM, N.CARDNUMBERLENGTH FROM RETAILSTORETENDERTYPECARDTABLE S INNER JOIN RETAILTENDERTYPECARDTABLE C ON C.CARDTYPEID = S.CARDTYPEID INNER JOIN RETAILTENDERTYPECARDNUMBERS N ON N.CARDTYPEID = S.CARDTYPEID INNER JOIN RETAILSTORETABLE RST ON RST.RECID = S.CHANNEL WHERE RST.STORENUMBER = @STOREID ";

                if (types.HasValue)
                {
                    queryString += "AND C.CARDTYPES = @CARDTYPES ";
                }

                queryString += "ORDER BY N.CARDNUMBERLENGTH DESC";

                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    SqlParameter dataAreaIdParam = command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
                    SqlParameter storeIdParam = command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10);

                    if (types.HasValue)
                    {
                        SqlParameter cardTypesParam = command.Parameters.Add("@CARDTYPES", SqlDbType.Int);
                        cardTypesParam.Value = (int)types.Value;
                    }

                    dataAreaIdParam.Value = Application.Settings.Database.DataAreaID;
                    storeIdParam.Value = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId; ;

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ICardInfo temp = this.Utility.CreateCardInfo();

                            temp.CardTypeId = Utility.ToString(reader["CARDTYPEID"]);
                            temp.CardName = Utility.ToString(reader["NAME"]);
                            temp.TenderTypeId = Utility.ToString(reader["TENDERTYPEID"]);
                            temp.CardFee = Utility.ToDecimal(reader["CARDFEE"]);
                            temp.ModulusCheck = Utility.ToBool(reader["CHECKMODULUS"]);
                            temp.ExpDateCheck = Utility.ToBool(reader["CHECKEXPIREDDATE"]);
                            temp.ProcessLocally = Utility.ToBool(reader["PROCESSLOCALLY"]);
                            temp.AllowManualInput = Utility.ToBool(reader["ALLOWMANUALINPUT"]);
                            temp.EnterFleetInfo = Utility.ToBool(reader["ENTERFLEETINFO"]);
                            temp.CardType = (CardTypes)Utility.ToInt(reader["CARDTYPES"]);
                            temp.Issuer = Utility.ToString(reader["CARDISSUER"] ?? string.Empty);
                            temp.BinTo = Utility.ToString(reader["CARDNUMBERTO"]);
                            temp.BinFrom = Utility.ToString(reader["CARDNUMBERFROM"]);
                            temp.BinLength = Utility.ToInt(reader["CARDNUMBERLENGTH"]);
                            temp.CashBackLimit = Utility.ToDecimal(reader["CASHBACKLIMIT"]);

                            cardTypes.Add(temp);
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        #endregion
    }
}

