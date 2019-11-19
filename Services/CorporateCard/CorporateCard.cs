/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using LSRetailPosis.DataAccess;

namespace Microsoft.Dynamics.Retail.Pos.CorporateCard
{
    [Export(typeof(ICorporateCard))]
    public class CorporateCard : ICorporateCard
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        // TextID's for CorporateCard are reserved at 50150 - 50199
        //
        // These TextID's are also used in frmTenderRestriction and frmInformation

        private RetailTransaction retailTransaction;
        private ICardInfo iCardInfo;
        private decimal amountValue;

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
        [Import]
        public IApplication Application { get; set; }

        #region ICorporateCard Members

        /// <summary>
        /// Updates the payment details in the database as per card information.
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <param name="amount"></param>
        /// <param name="posTransaction"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void ProcessCardPayment(ICardInfo cardInfo, decimal amount, object posTransaction)
        {
            LogMessage("Processing payment",
                LSRetailPosis.LogTraceLevel.Trace,
                "CorporateCard.ProcessCardPayment");

            this.iCardInfo = cardInfo;
            this.amountValue = this.Application.Services.Rounding.RoundAmount(amount, ApplicationSettings.Terminal.StoreId, cardInfo.TenderTypeId);

            retailTransaction = (RetailTransaction)posTransaction;

            decimal payableAmt = SetTenderRestrictions(retailTransaction, Convert.ToInt32(cardInfo.TenderTypeId), cardInfo.CardNumber, 24);
            payableAmt = this.Application.Services.Rounding.RoundAmount(payableAmt, ApplicationSettings.Terminal.StoreId, cardInfo.TenderTypeId);

            if (payableAmt > 0)
            {
                if (payableAmt < this.amountValue)
                {
                    this.amountValue = payableAmt;
                }

                // Process the payment.
                ProcessCorporateCardPayment();
            }
        }

        /// <summary>
        /// Updates card payment details in the database as per given card information.
        /// </summary>
        /// <param name="cardInfo"></param>
        /// <param name="posTransaction"></param>
        public void VoidCardPayment(ICardInfo cardInfo, object posTransaction)
        {
            try
            {
                LogMessage("Voiding payment",
                    LSRetailPosis.LogTraceLevel.Trace,
                    "CorporateCard.VoidCardPayment");
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        #endregion

        private void ProcessCorporateCardPayment()
        {
            try
            {
                LogMessage(string.Empty,
                    LSRetailPosis.LogTraceLevel.Trace,
                    "CorporateCard.ProcessCorporateCardPayment");

                // Add the corporate card tenderline to the transaction
                AddCorporateCardTenderLine();
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        private void AddCorporateCardTenderLine()
        {
            try
            {
                LogMessage("Adding the tender line to the transaction",
                    LSRetailPosis.LogTraceLevel.Trace,
                    "CorporateCard.AddCorporateCardTenderLine");

                bool getAdditionalInfo = false;

                // Generate the tender line for the card
                CorporateCardTenderLineItem cardTenderLine = (CorporateCardTenderLineItem)this.Application.BusinessLogic.Utility.CreateCorporateCardTenderLineItem();
                cardTenderLine.TenderTypeId = iCardInfo.TenderTypeId;
                cardTenderLine.CardName = iCardInfo.CardName;
                cardTenderLine.Description = LSRetailPosis.ApplicationLocalizer.Language.Translate(50150) + " " + iCardInfo.CardName; //Card payment
                cardTenderLine.Amount = this.amountValue;

                if (iCardInfo.CardEntryType == CardEntryTypes.MAGNETIC_STRIPE_READ)
                {
                    // We have the track, from which we need to get the card number and exp date
                    string track2Data = LSRetailPosis.Settings.HardwareProfiles.MSR.Track2Data(iCardInfo.Track2);

                    cardTenderLine.CardNumber = track2Data.Substring(0, track2Data.IndexOf("-"));
                    cardTenderLine.ExpiryDate = track2Data.Substring(track2Data.IndexOf("-") + 1, 4);
                }
                else
                {
                    // We have the card number and the exp. date
                    cardTenderLine.CardNumber = iCardInfo.CardNumber;
                    cardTenderLine.ExpiryDate = iCardInfo.ExpDate;
                    getAdditionalInfo = true;
                }

                // Get information about the tender...
                TenderData tenderData = new TenderData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
                ITender tenderInfo = tenderData.GetTender(iCardInfo.TenderTypeId, ApplicationSettings.Terminal.StoreId);
                cardTenderLine.OpenDrawer = tenderInfo.OpenDrawer;
                cardTenderLine.ChangeTenderID = tenderInfo.ChangeTenderID;
                cardTenderLine.MinimumChangeAmount = tenderInfo.MinimumChangeAmount;
                cardTenderLine.AboveMinimumTenderId = tenderInfo.AboveMinimumTenderId;

                //Get additional information on corporate card if needed
                if (getAdditionalInfo && !GetAdditionalInformation(cardTenderLine, this.Application))
                {
                    return;
                }

                //convert from the store-currency to the company-currency...
                cardTenderLine.CompanyCurrencyAmount = this.Application.Services.Currency.CurrencyToCurrency(
                    ApplicationSettings.Terminal.StoreCurrency,
                    ApplicationSettings.Terminal.CompanyCurrency,
                    this.amountValue);
                // the exchange rate between the store amount(not the paid amount) and the company currency
                cardTenderLine.ExchrateMST = this.Application.Services.Currency.ExchangeRate(
                    ApplicationSettings.Terminal.StoreCurrency) * 100;

                cardTenderLine.SignatureData = LSRetailPosis.POSProcesses.TenderOperation.ProcessSignatureCapture(tenderInfo, cardTenderLine);

                retailTransaction.Add(cardTenderLine);

                ((IPosTransactionV2)retailTransaction).LastRunOperationIsValidPayment = true;
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        private static bool GetAdditionalInformation(CorporateCardTenderLineItem cardTenderLine, IApplication application)
        {
            ApplicationLog.Log("CorporateCard.GetAdditionalInformation", "Additional fleet card infomration", LogTraceLevel.Trace);

            bool result = false;

            using (frmInformation frmAddInfo = new frmInformation())
            {
                application.ApplicationFramework.POSShowForm(frmAddInfo);

                if (frmAddInfo.DialogResult == DialogResult.OK)
                {
                    cardTenderLine.DriverId = frmAddInfo.DriverId;
                    cardTenderLine.VehicleId = frmAddInfo.VehicleId;
                    cardTenderLine.OdometerReading = Convert.ToInt32(frmAddInfo.Odometer);
                    result = true;
                }
            }

            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="TenderId"></param>
        /// <param name="FleetCardNumber"></param>
        /// <param name="RestrictionId"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "TenderId")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "RestrictionId")]
        [SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "FleetCardNumber", Justification = "Grandfather")]
        private decimal SetTenderRestrictions(RetailTransaction posTransaction, int TenderId, string FleetCardNumber, int RestrictionId)
        {
            try
            {
                decimal payableAmt = 0;

                payableAmt = this.Application.Services.TenderRestriction.FindTenderRestriction(posTransaction, iCardInfo);
                payableAmt = this.Application.Services.Rounding.Round(payableAmt);

                if (payableAmt != posTransaction.NetAmountWithTax)
                {
                    string message = string.Format(
                        LSRetailPosis.ApplicationLocalizer.Language.Translate(50153),
                        this.Application.Services.Rounding.Round(payableAmt));

                    using (frmTenderRestriction frmExcluded = new frmTenderRestriction(posTransaction))
                    {
                        frmExcluded.DisplayMsg = message;
                        this.Application.ApplicationFramework.POSShowForm(frmExcluded);

                        if (frmExcluded.DialogResult == DialogResult.No)
                        {
                            this.Application.Services.TenderRestriction.ClearTenderRestriction(retailTransaction);
                            return 0;
                        }
                    }
                }
                return payableAmt;
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        /// <summary>
        /// Log a message to the file.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="traceLevel"></param>
        /// <param name="args"></param>
        private void LogMessage(string message, LogTraceLevel traceLevel, params object[] args)
        {
            ApplicationLog.Log(this.GetType().Name, string.Format(message, args), traceLevel);
        }
    }
}
