/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using LSRetailPosis.DataAccess;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.RoundingService
{
    [Export(typeof(IRounding))]
    public class Rounding : IRounding, IInitializeable
    {

        #region Enums

        /// <summary>
        /// Rounding method for Tax and Currency calculations
        /// </summary>
        internal enum RoundMethod
        {
            /// <summary>
            /// 0
            /// </summary>
            RoundNearest = 0,
            /// <summary>
            /// 1
            /// </summary>
            RoundDown = 1,
            /// <summary>
            /// 2
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "RoundUp", Justification = "Grandfather API")]
            RoundUp = 2
        }

        /// <summary>
        /// Rounding method for Tender calculations
        /// </summary>
        internal enum TenderRoundMethod
        {
            /// <summary>
            /// 0
            /// </summary>
            None = 0,
            /// <summary>
            /// 1
            /// </summary>
            RoundNearest = 1,
            /// <summary>
            /// 2
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "RoundUp", Justification = "Grandfather API")]
            RoundUp = 2,
            /// <summary>
            /// 3
            /// </summary>
            RoundDown = 3

        }

        #endregion

        #region Members

        private DataTable currencyInfo;
        private DataTable taxCodeInfo;
        private Dictionary<string, int> unitsOfMeasure = new Dictionary<string,int>();
        private string storeCurrencyCode;
        private const decimal defaultRoundingValue = 0.01M;

        [Import]
        public IApplication Application { get; set; }

        #endregion

        #region IInitializeableV1 Members

        public void Initialize()
        {
            currencyInfo = new DataTable();
            taxCodeInfo = new DataTable();

            currencyInfo = LoadCurrencyInfo(currencyInfo);
            taxCodeInfo = LoadTaxCodeInfo(taxCodeInfo);
            storeCurrencyCode = ApplicationSettings.Terminal.StoreCurrency;
        }

        public void Uninitialize()
        {
            currencyInfo.Dispose();
            taxCodeInfo.Dispose();
        }

        #endregion

        #region IRounding Methods

        /// <summary>
        /// Refresh currency rounding information from the database;
        /// </summary>
        public void Refresh()
        {
            currencyInfo = LoadCurrencyInfo(currencyInfo);
            taxCodeInfo = LoadTaxCodeInfo(taxCodeInfo);
        }

        /// <summary>
        /// Standard round to the minimal coin/amount according to the store currency
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <returns>The rounded value</returns>
        public decimal Round(decimal value)
        {
            return RoundToUnit(value, GetMinimumCurrencyUnit(storeCurrencyCode), Method(storeCurrencyCode));
        }

        /// <summary>
        /// Standard round to the minimal coin/amount according to the store currency
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <returns>The rounded value as string</returns>
        public string Round(decimal value, bool useCurrencySymbol)
        {
            decimal currencyUnit = GetMinimumCurrencyUnit(storeCurrencyCode);
            string numberFormat = NumberFormat(currencyUnit);
            decimal roundedValue = RoundToUnit(value, currencyUnit, Method(storeCurrencyCode));
            string roundedValueString = roundedValue.ToString(numberFormat);
            return AddPrefixSuffix(roundedValueString, useCurrencySymbol, storeCurrencyCode);
        }

        /// <summary>
        /// Rounding for display: If no decimal places then sales rounding is used otherwise the amount rounding is used
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <returns>The rounded value as string</returns>
        public string RoundForDisplay(decimal value, bool useCurrencySymbol, bool useSalesRounding)
        {
            return RoundForDisplay(value, storeCurrencyCode, useCurrencySymbol, useSalesRounding);
        }

        /// <summary>
        /// Rounding for display: If no decimal places then sales rounding is used otherwise the amount rounding is used
        /// </summary>
        /// <param name="value"></param>
        /// <param name="currencyCode"></param>
        /// <param name="useCurrencySymbol"></param>
        /// <param name="useSalesRounding"></param>
        /// <returns></returns>
        public string RoundForDisplay(decimal value, string currencyCode, bool useCurrencySymbol, bool useSalesRounding)
        {
            //If false then no change - use the "old" Round(decimal, bool) function
            if (!useSalesRounding)
            {
                return Round(value, currencyCode, useCurrencySymbol);
            }
            else
            {
                decimal currencyUnit = GetMinimumCurrencyUnit(currencyCode, useSalesRounding);
                string numberFormat = NumberFormat(currencyUnit);
                decimal roundedValue = RoundToUnit(value, currencyUnit, Method(currencyCode, useSalesRounding));
                string roundedValueString = roundedValue.ToString(numberFormat);
                return AddPrefixSuffix(roundedValueString, useCurrencySymbol, currencyCode);
            }
        }

        /// <summary>
        /// Returns a string to give the correct number number of decimals depending on the currency unit. 
        /// 1 will give return N0, 0,1 returns N1, 0,01 returns N2 etc.
        /// </summary>
        /// <param name="currencyUnit">The smallest currency unit</param>
        /// <returns>Returns the format string N0,N1, etc for the ToString function</returns>
        private static string NumberFormat(decimal currencyUnit)
        {
            //string result = "N"; Base

            string result = "F";
            if (Math.Round(currencyUnit) > 0)
            {
                result += "0";
            }
            else
            {
                for (int i = 1; i < 9; i++)
                {
                    decimal factor = (decimal)Math.Pow(10, i);
                    decimal multipl = currencyUnit * factor;
                    if (multipl == Math.Round(multipl))
                    {
                        result += i.ToString();
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Returns string type converting decimal amount to string.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public string RoundAmountViewer(decimal amount)
        {
            return amount.ToString(NumberFormat(amount));
        }

        /// <summary>
        /// Rounds the tender amount according to the tender type and store
        /// </summary>
        /// <param name="value">The amount to round</param>
        /// <param name="storeId">The store id</param>
        /// <param name="tenderTypeId">The tender type id</param>
        /// <returns>The rounded amount</returns>
        public decimal RoundAmount(decimal value, string storeId, string tenderTypeId)
        {
            int roundingMethod = 0;
            decimal roundingValue = defaultRoundingValue;
            GetPaymentRoundInfo(storeId, tenderTypeId, ref roundingMethod, ref roundingValue);
            return RoundToUnit(value, roundingValue, (TenderRoundMethod)roundingMethod);
        }

        /// <summary>
        /// Returns round off value of the passed value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="storeId"></param>
        /// <param name="tenderTypeId"></param>
        /// <param name="useCurrencySymbol"></param>
        /// <returns></returns>
        public string RoundAmount(decimal value, string storeId, string tenderTypeId, bool useCurrencySymbol)
        {
            decimal currencyUnit = GetMinimumCurrencyUnit(storeCurrencyCode);
            string numberFormat = NumberFormat(currencyUnit);
            decimal roundedValue = RoundAmount(value, storeId, tenderTypeId);
            string roundedValueString = roundedValue.ToString(numberFormat);
            return AddPrefixSuffix(roundedValueString, useCurrencySymbol, storeCurrencyCode);
        }

        /// <summary>
        /// Rounds the tender amount according to store 
        /// </summary>
        /// <param name="value">The amount to round</param>
        /// <param name="storeId">The store id</param>
        /// <returns>The rounded amount</returns>
        public decimal RoundAmount(decimal value, string storeId)
        {
            int roundingMethod = 0;
            decimal roundingValue = defaultRoundingValue;
            GetPaymentRoundInfo(storeId, ref roundingMethod, ref roundingValue);
            return RoundToUnit(value, roundingValue, (TenderRoundMethod)roundingMethod);
        }

        /// <summary>
        /// Standard round to minimal coin/amount in defined currency
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <param name="currency">The currency of the value</param>
        /// <returns>The rounded value</returns>
        public decimal Round(decimal value, string currencyCode)
        {
            return RoundToUnit(value, GetMinimumCurrencyUnit(currencyCode), Method(currencyCode));
        }

        /// <summary>
        /// Standard round to sales rounding
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <returns>The rounded value as string</returns>
        public decimal Round(decimal value, string currencyCode, bool useSalesRounding, int notUsed)
        {
            //If false then no change - use the "old" Round (decimal, bool) function
            if (!useSalesRounding)
            {
                return Round(value, currencyCode);
            }
            else
            {
                return RoundToUnit(value, GetMinimumCurrencyUnit(currencyCode, useSalesRounding), Method(currencyCode, useSalesRounding));
            }
        }

        /// <summary>
        /// Standard round to minimal coin/amount in defined currency
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <param name="currency">The currency of the value</param>
        /// <returns>The rounded value as string</returns>
        public string Round(decimal value, string currencyCode, bool useCurrencySymbol)
        {
            decimal currencyUnit = GetMinimumCurrencyUnit(currencyCode);
            string numberFormat = NumberFormat(currencyUnit);
            decimal roundedValue = RoundToUnit(value, currencyUnit, Method(currencyCode));
            string roundedValueString = roundedValue.ToString(numberFormat);
            return AddPrefixSuffix(roundedValueString, useCurrencySymbol, currencyCode);
        }

        /// <summary>
        /// Round for receipt format. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="numberOfDecimals"></param>
        /// <returns></returns>
        public string RoundForReceipt(decimal value, int numberOfDecimals)
        {
            decimal unit = 1.0M / (decimal)Math.Pow(10, numberOfDecimals);
            decimal roundedValue = RoundToUnit(value, unit, RoundMethod.RoundNearest);
            string numberFormat = NumberFormat(unit);
            return roundedValue.ToString(numberFormat);
        }
        /// <summary>
        /// Round the value with a certain number of decimals.
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <param name="numberOfDecimals">The number of decimals in the return value</param>
        /// <returns>The rounded value</returns>
        public decimal Round(decimal value, int numberOfDecimals)
        {
            decimal unit = 1.0M / (decimal)Math.Pow(10, numberOfDecimals);
            return RoundToUnit(value, unit, Method(storeCurrencyCode));
        }

        /// <summary>
        /// Round the value with a certain number of decimals.
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <param name="numberOfDecimals">The number of decimals in the return value</param>
        /// <returns>The rounded value</returns>
        public string Round(decimal value, int numberOfDecimals, bool useCurrencySymbol)
        {
            decimal unit = 1.0M / (decimal)Math.Pow(10, numberOfDecimals);
            decimal roundedValue = RoundToUnit(value, unit, Method(storeCurrencyCode));
            string formatString = "F" + numberOfDecimals.ToString();//"N" Base was N
            string roundedValueString = roundedValue.ToString(formatString);
            return AddPrefixSuffix(roundedValueString, useCurrencySymbol, storeCurrencyCode);
        }

        #region IRoundingV2

        /// <summary>
        /// Converts the string representation of a number input by a user to its System.Decimal equivalent based on how a decimal separator is treated as input in the system.        
        /// </summary>
        /// <param name="inputText">Number to parse</param>
        /// <param name="currencyCode">Currency code to determine smallest unit of currency</param>
        /// <param name="price">Value of result if parsing successful</param>
        /// <returns>True if parsing was successful, false otherwise</returns>
        /// <example>"100" could parse as 100 or 1.00</example>
        public bool TryParseCurrencyInput(string s, string currencyCode, out decimal value)
        {
            bool success = false;
            value        = decimal.Zero;

            if (!string.IsNullOrWhiteSpace(s) && !string.IsNullOrWhiteSpace(currencyCode))
            {
                if (Functions.DecimalNotRequiredForMinorCurrencyUnit)
                {
                    // Get the smallest currency unit (i.e. to determine the number of decimal places) e.g. 0.01
                    decimal smallestCurrencyUnit = GetMinimumCurrencyUnit(currencyCode);

                    // Try to parse without a decimal separator first...
                    if (success = decimal.TryParse(s, NumberStyles.Number & ~NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo, out value))
                    {
                        // If no decimal separator was entered we assume as if the decimal separator was entered first e.g. "50" = .50
                        value *= smallestCurrencyUnit;
                    }
                }

                if (!success)
                {
                    success = Decimal.TryParse(s, out value);
                }
            }

            return success;
        }

        #endregion

        /// <summary>
        /// Returns the value/amount as a string with currency symbol if needed.
        /// </summary>
        /// <param name="value">The value to convert to string with or without a pre-/postfix</param>
        /// <param name="userCurrencySymbol">Set as true if a currency symbol should be added to the string</param>
        /// <returns>The resulting value/amount string.</returns>
        private string AddPrefixSuffix(string value, bool useCurrencySymbol, string currencyCode)
        {
            string result = value;

            if (useCurrencySymbol)
            {
                string prefix = "";
                string suffix = "";
                foreach (DataRow row in currencyInfo.Select("CURRENCYCODE='" + currencyCode.ToUpperInvariant() + "'"))
                {
                    prefix = (string)row["CURRENCYPREFIX"] ?? "";
                    if (prefix.Length == 0)
                    {
                        if (row["SYMBOL"] != System.DBNull.Value)
                        {
                            prefix = (string)row["SYMBOL"];
                        }

                    }
                    if (row["CURRENCYSUFFIX"] != System.DBNull.Value)
                    {
                        suffix = (string)row["CURRENCYSUFFIX"];
                    }
                }

                if (prefix.Length > 0)
                {
                    result = prefix + " " + result;
                }

                if (suffix.Length > 0)
                {
                    result = result + " " + suffix;
                }

                return result;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Returns the lowest currency unit that will be rounded to for a certain currency code using normal rounding
        /// </summary>
        /// <param name="currencyCode">The currency code</param>
        /// <returns>The lowest currency unit</returns>
        public Decimal GetMinimumCurrencyUnit(string currencyCode)
        {
            Decimal retVal = defaultRoundingValue;

            if (!string.IsNullOrWhiteSpace(currencyCode))
            {
                try
                {
                    foreach (DataRow row in currencyInfo.Select("CURRENCYCODE='" + currencyCode.ToUpperInvariant() + "'"))
                    {
                        retVal = ((decimal)row["ROUNDOFFPRICE"] == 0) ? retVal : (decimal)row["ROUNDOFFPRICE"];
                    }

                    return retVal;
                }
                catch (Exception)
                {
                }
            }

            return retVal;
        }

        /// <summary>
        /// Gets the lowest currency unit that will be rounded to for a given currency code using either Sales or Amount rounding.
        /// </summary>
        /// <param name="currencyCode">Currency code</param>
        /// <param name="useSalesRounding">True to use sales rounding, false to use amount rounding</param>
        /// <returns>Lowest currency unit</returns>
        /// <example>0.01</example>
        private Decimal GetMinimumCurrencyUnit(string currencyCode, bool useSalesRounding)
        {
            Decimal retVal = defaultRoundingValue;

            try
            {
                foreach (DataRow row in currencyInfo.Select("CURRENCYCODE='" + currencyCode.ToUpperInvariant() + "'"))
                {
                    retVal = (useSalesRounding) ? (decimal)row["ROUNDOFFSALES"] : (decimal)row["ROUNDOFFPRICE"];
                    retVal = (retVal == 0) ? defaultRoundingValue : retVal;
                }
                return retVal;
            }
            catch (Exception)
            {
                return retVal;
            }
        }

        /// <summary>
        /// Returns the lowest currency unit that will be rounded to for a certain tax code using normal rounding
        /// </summary>
        /// <param name="taxCode">The tax code</param>
        /// <returns>The tax round unit</returns>
        private Decimal TaxCodeUnit(string taxCode)
        {
            Decimal retVal = defaultRoundingValue;

            try
            {
                foreach (DataRow row in taxCodeInfo.Select("TAXCODE='" + taxCode.ToUpperInvariant() + "'"))
                {
                    retVal = ((decimal)row["TAXROUNDOFF"] == 0) ? retVal : (decimal)row["TAXROUNDOFF"];
                }
                return retVal;
            }
            catch (Exception)
            {
                return retVal;
            }
        }

        /// <summary>
        /// Returns roundmethod that will be used to round the receipt amount
        /// </summary>
        /// <param name="currencyCode">The currency code</param>
        /// <returns>The round method that will be used.</returns>
        private RoundMethod Method(string currencyCode)
        {
            try
            {
                foreach (DataRow row in currencyInfo.Select("CURRENCYCODE='" + currencyCode.ToUpperInvariant() + "'"))
                {
                    int roundoffType = (int)row["ROUNDOFFTYPEPRICE"];
                    switch (roundoffType)
                    {
                        case 0: return RoundMethod.RoundNearest;
                        case 1: return RoundMethod.RoundDown;
                        case 2: return RoundMethod.RoundUp;
                    }
                }
                return RoundMethod.RoundNearest; //Default if nothing is found
            }
            catch (Exception)
            {
                return RoundMethod.RoundNearest; //Default if nothing is found
            }
        }

        /// <summary>
        /// Returns roundmethod that will be used to round the balance amount. If useSalesRounding is false Method(currencyCode) will be called
        /// </summary>
        /// <param name="currencyCode">The currency code</param>
        /// <param name="useSalesRounding">If true the Currency.RoundOffTypeSales is used otherwise Currency.ROUNDOFFTYPEPRICE is used</param>
        /// <returns>The round method that will be used.</returns>
        private RoundMethod Method(string currencyCode, bool useSalesRounding)
        {
            try
            {
                if (!useSalesRounding)
                    return Method(currencyCode);

                foreach (DataRow row in currencyInfo.Select("CURRENCYCODE='" + currencyCode.ToUpperInvariant() + "'"))
                {
                    int roundoffType = (int)row["ROUNDOFFTYPESALES"];
                    switch (roundoffType)
                    {
                        case 0: return RoundMethod.RoundNearest;
                        case 1: return RoundMethod.RoundDown;
                        case 2: return RoundMethod.RoundUp;
                    }
                }
                return RoundMethod.RoundNearest; //Default if nothing is found
            }
            catch (Exception)
            {
                return RoundMethod.RoundNearest; //Default if nothing is found
            }
        }

        /// <summary>
        /// Returns roundmethod that will be used to round the tax amount
        /// </summary>
        /// <param name="taxCode">The tax code</param>
        /// <returns>The round method that will be used.</returns>
        private RoundMethod TaxMethod(string taxCode)
        {
            try
            {
                foreach (DataRow row in taxCodeInfo.Select("TAXCODE='" + taxCode.ToUpperInvariant() + "'"))
                {
                    int roundoffType = (int)row["TAXROUNDOFFTYPE"];
                    switch (roundoffType)
                    {
                        case 0: return RoundMethod.RoundNearest;
                        case 1: return RoundMethod.RoundDown;
                        case 2: return RoundMethod.RoundUp;
                    }
                }
                return RoundMethod.RoundNearest; //Default if nothing is found
            }
            catch (Exception)
            {
                return RoundMethod.RoundNearest; //Default if nothing is found
            }
        }

        /// <summary>
        /// Standard round of the tax amount according to the tax code setup
        /// </summary>
        /// <param name="value">The value to round</param>
        /// <param name="taxCode">The tax code to round</param>
        /// <returns>The rounded value</returns>
        public decimal TaxRound(decimal value, string taxCode)
        {
            return RoundToUnit(value, TaxCodeUnit(taxCode), TaxMethod(taxCode));
        }

        /// <summary>
        /// Returns the quantity rounded to the correct amount of decimals, corresponding to the settings in the Unit table.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="UnitId"></param>
        /// <returns></returns>
        public string RoundQuantity(decimal value, string unitId)
        {
            if (!this.unitsOfMeasure.ContainsKey(unitId))
            {
                //// Get the number of decimals for the specific unit id.
                UnitOfMeasureData unitOfMeasureData = new UnitOfMeasureData(
                    this.Application.Settings.Database.Connection,
                    this.Application.Settings.Database.DataAreaID,
                    ApplicationSettings.Terminal.StorePrimaryId,
                    this.Application);

                this.unitsOfMeasure.Add(unitId, unitOfMeasureData.GetUnitDecimals(unitId));
            }

            if (this.unitsOfMeasure.ContainsKey(unitId))
            {
                // Get the number of decimals for the specific unit id.   
                return value.ToString(string.Format("N{0}", this.unitsOfMeasure[unitId]));
            }

            return value.ToString();
        }

        private int number;
        private int GetNumberOfDecimals(decimal round)
        {
            if (round < 1)
            {
                while (round != 0)
                {
                    round = (round * 10M) % 1;
                    number++;
                }
            }
            return number;
        }

        /// <summary>
        /// Rounds values to nearest currency unit, i.e 16,45 kr. rounded up if the smallest coin is 10 kr will give 20 kr.
        /// or if the smallest coin is 24 aurar(0,25 kr.) then if rounded up it will give 16,50 kr.
        /// </summary>
        /// <param name="value">The currency value or value to be rounded.</param>
        /// <param name="unit">The smallest unit to be rounded to.</param>
        /// <param name="roundMethod">The method of rounding, Nearest,up and down</param>
        /// <returns>Returns a value rounded to the nearest unit.</returns>
        private decimal RoundToUnit(decimal value, decimal unit, RoundMethod roundMethod)
        {
            if (unit != 0)
            {
                number = 0;
                decimal decimalValue = value / unit;
                decimal difference = Math.Abs(decimalValue) - Math.Abs(Math.Truncate(value / unit));

                // is rounding required?
                if (difference > 0)
                {
                    switch (roundMethod)
                    {
                        case RoundMethod.RoundNearest: { return Math.Round(Math.Round((value / unit), 0, MidpointRounding.AwayFromZero) * unit, GetNumberOfDecimals(unit), MidpointRounding.AwayFromZero); }
                        case RoundMethod.RoundDown:
                            {
                                if (value > 0M)
                                    return Math.Round(Math.Round((value / unit) - 0.5M, 0) * unit, GetNumberOfDecimals(unit));
                                else
                                    return Math.Round(Math.Round((value / unit) + 0.5M, 0) * unit, GetNumberOfDecimals(unit));
                            }
                        case RoundMethod.RoundUp:
                            {
                                if (value > 0M)
                                    return Math.Round(Math.Round((value / unit) + 0.5M, 0) * unit, GetNumberOfDecimals(unit));
                                else
                                    return Math.Round(Math.Round((value / unit) - 0.5M, 0) * unit, GetNumberOfDecimals(unit));
                            }
                    }
                }
                else
                {
                    // Value is considered to be rounded, however there are cases that the decimal part is so small,
                    // making the difference equals to zero. In this case, UI will show as many zeros as possible, which is wrong.
                    // Thus, we divide by the unit, truncate and then multiply back.
                    value = Math.Truncate(value / unit) * unit;
                }
            }
            return value;
        }

        /// <summary>
        /// Rounds values to nearest currency unit using the tender type rounding settings, i.e 16,45 kr. rounded up if the smallest coin is 10 kr will give 20 kr.
        /// or if the smallest coin is 24 aurar(0,25 kr.) then if rounded up it will give 16,50 kr.
        /// </summary>
        /// <param name="value">The currency value or value to be rounded.</param>
        /// <param name="unit">The smallest unit to be rounded to.</param>
        /// <param name="roundMethod">The method of rounding, None, Nearest, up or down</param>
        /// <returns>Returns a value rounded to the nearest unit.</returns>
        private decimal RoundToUnit(decimal value, decimal unit, TenderRoundMethod roundMethod)
        {
            if (roundMethod == TenderRoundMethod.None)
                return value;

            /* 
             * TenderRoundMethod = 0 - None, 1 - Nearest, 2 - Up, 3 - Down
             * RoundMethod = 0 - Nearest, 1 - Down, 2 - Up
             * 
             * When rounding payment numbers then the TenderRoundingMethod should be but when
             * rounding tax or currency numbers then the RoundMethod should be used.
             * 
             * If TenderRoundMethod is set to NONE then the value is returned without any rounding
             * otherwise we need to cast the TenderRoundingMethod to RoundingMethod and then call the original RoundToUnit function
             * 
             */
            RoundMethod currencyRound = RoundMethod.RoundNearest;
            switch (roundMethod)
            {
                case TenderRoundMethod.RoundNearest: { currencyRound = RoundMethod.RoundNearest; break; }
                case TenderRoundMethod.RoundUp: { currencyRound = RoundMethod.RoundUp; break; }
                case TenderRoundMethod.RoundDown: { currencyRound = RoundMethod.RoundDown; break; }
            }
            return RoundToUnit(value, unit, currencyRound);
        }


        private void GetPaymentRoundInfo(string storeId, string tenderTypeId, ref int roundingMethod, ref decimal roundingValue)
        {
            SqlConnection connection = Application.Settings.Database.Connection;
            string dataAreaId = Application.Settings.Database.DataAreaID;

            try
            {
                string queryString = "SELECT ROUNDINGMETHOD, ROUNDING FROM RETAILSTORETENDERTYPETABLE JOIN RETAILSTORETABLE ON RETAILSTORETENDERTYPETABLE.CHANNEL = RETAILSTORETABLE.RECID WHERE RETAILSTORETENDERTYPETABLE.DATAAREAID=@DATAAREAID AND RETAILSTORETENDERTYPETABLE.TENDERTYPEID=@TENDERTYPEID AND RETAILSTORETABLE.STORENUMBER=@STOREID ";
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    SqlParameter storeIdParm = command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10);
                    storeIdParm.Value = storeId;
                    SqlParameter tenderTypeIdParm = command.Parameters.Add("@TENDERTYPEID", SqlDbType.NVarChar, 10);
                    tenderTypeIdParm.Value = tenderTypeId;
                    SqlParameter dataAreaIdParm = command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
                    dataAreaIdParm.Value = dataAreaId;

                    if (connection.State != ConnectionState.Open) { connection.Open(); }
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
                    reader.Read();
                    if (reader.HasRows)
                    {
                        roundingMethod = Convert.ToInt32(reader["ROUNDINGMETHOD"]);
                        roundingValue = Convert.ToDecimal(reader["ROUNDING"]);
                    }
                    //else use default
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private void GetPaymentRoundInfo(string storeId, ref int roundingMethod, ref decimal roundingValue)
        {
            SqlConnection connection = Application.Settings.Database.Connection;
            string dataAreaId = Application.Settings.Database.DataAreaID;

            try
            {
                string queryString = "SELECT ROUNDINGMETHOD, ROUNDING FROM RETAILSTORETENDERTYPETABLE JOIN RETAILSTORETABLE ON RETAILSTORETENDERTYPETABLE.CHANNEL = RETAILSTORETABLE.RECID WHERE RETAILSTORETABLE.STORENUMBER=@STOREID AND RETAILSTORETENDERTYPETABLE.DATAAREAID=@DATAAREAID AND RETAILSTORETENDERTYPETABLE.POSOPERATION='200'";//POSOperation=200 is cash
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    SqlParameter storeIdParm = command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10);
                    storeIdParm.Value = storeId;
                    SqlParameter dataAreaIdParm = command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
                    dataAreaIdParm.Value = dataAreaId;

                    if (connection.State != ConnectionState.Open) { connection.Open(); }
                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
                    reader.Read();
                    if (reader.HasRows)
                    {
                        roundingMethod = Convert.ToInt32(reader["ROUNDINGMETHOD"]);
                        roundingValue = Convert.ToDecimal(reader["ROUNDING"]);
                    }
                    else //Some default.
                    {
                        roundingMethod = 0;
                        roundingValue = defaultRoundingValue;
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "currencyInfo")]
        private DataTable LoadCurrencyInfo(DataTable currencyInfo)
        {
            SqlConnection connection = Application.Settings.Database.Connection;

            try
            {
                string queryString = "SELECT ROUNDOFFPRICE, ROUNDOFFSALES, CURRENCYCODE, ROUNDOFFTYPEPRICE, ROUNDOFFTYPESALES, CURRENCYPREFIX, CURRENCYSUFFIX, SYMBOL FROM CURRENCY";
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    if (connection.State != ConnectionState.Open) { connection.Open(); }

                    SqlDataReader reader = command.ExecuteReader();
                    currencyInfo.Load(reader);
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return currencyInfo;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "taxCodeInfo")]
        private DataTable LoadTaxCodeInfo(DataTable taxCodeInfo)
        {
            SqlConnection connection = Application.Settings.Database.Connection;
            string dataAreaId = Application.Settings.Database.DataAreaID;

            try
            {
                string qryStr = " SELECT TAXCODE, TAXROUNDOFF, TAXROUNDOFFTYPE " +
                                " FROM TAXTABLE " +
                                " WHERE DATAAREAID = @DATAAREAID ";
                using (SqlCommand command = new SqlCommand(qryStr, connection))
                {
                    SqlParameter dataAreaIdParm = command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
                    dataAreaIdParm.Value = dataAreaId;

                    if (connection.State != ConnectionState.Open) { connection.Open(); }
                    SqlDataReader reader = command.ExecuteReader();
                    taxCodeInfo.Load(reader);
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

            return taxCodeInfo;
        }

        /// <summary>
        /// Converts passed amount from decimal to string.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="decimalSep"></param>
        /// <returns></returns>
        public string DecimalToStringTrimTrailingZeros(decimal amount, string decimalSep)
        {
            string result = amount.ToString();

            bool startSearching = false;
            int index = 0;
            int searchIndex = 0;
            bool zeroFound = false;
            foreach (char ch in result)
            {
                index++;
                if (ch.ToString() == decimalSep)
                    startSearching = true;
                if (startSearching == true)
                {
                    if (ch == '0')
                    {
                        if (searchIndex == 0)
                        {
                            if (!zeroFound)
                            {
                                searchIndex = index;
                                zeroFound = true;
                            }
                        }
                    }
                    else
                    {
                        searchIndex = 0;
                        zeroFound = false;
                    }
                }
            }
            return result.Substring(0, result.Length - searchIndex);
        }

        #endregion
    }
}
