/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessObjects;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.TaxService;

namespace Microsoft.Dynamics.Retail.Pos.Tax
{
    [DebuggerDisplay("TaxAmount: {TaxAmount}, HasExempt: {HasExempt}, ExemptAmount: {ExemptAmount}")]
    public class LineTaxResult
    {
        public bool HasExempt { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal ExemptAmount { get; set; }
        public decimal TaxRatePercent { get; set; }
    }

    /// <summary>
    /// Taxcode cache key object maintains the taxcode query parameters.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
    public struct CacheKey
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CacheKey"/> struct.
        /// </summary>
        /// <param name="itemID">The item ID.</param>
        /// <param name="customerID">The customer ID.</param>
        /// <param name="taxGroup">The tax group.</param>
        /// <param name="salesTaxGroup">The sales tax group.</param>
        /// <param name="saleDateTime">The sale date and time.</param>
        public CacheKey(string itemID, string customerID, string taxGroup, string salesTaxGroup, DateTime saleDateTime)
            : this()
        {
            ItemID = itemID;
            CustomerID = customerID;
            TaxGroupID = taxGroup;
            SalesTaxGroupID = salesTaxGroup;
            SaleDateTime = saleDateTime;
        }

        /// <summary>
        /// Gets the item ID.
        /// </summary>
        public string ItemID { get; private set; }

        /// <summary>
        /// Gets the Customer ID.
        /// </summary>
        public string CustomerID { get; private set; }

        /// <summary>
        /// Gets the tax group ID.
        /// </summary>
        public string TaxGroupID { get; private set; }

        /// <summary>
        /// Gets the sales tax group ID.
        /// </summary>
        public string SalesTaxGroupID { get; private set; }

        /// <summary>
        /// Gets the sale date and time.
        /// </summary>
        public DateTime SaleDateTime { get; private set; }
    }

    /// <summary>
    /// Tax Provider for default AX Tax definitions
    /// </summary>
    [DebuggerDisplay("{Identifier}")]
    public class TaxCodeProvider : ITaxProvider
    {

        #region Properties
        /// <summary>
        /// Database connection
        /// </summary>
        protected IApplication Application { get; set; }

        protected static readonly DateTime NoDate = new DateTime(1901, 1, 1);
        #endregion

        private string cachedTransactionID;
        private Dictionary<CacheKey, List<TaxCode>> taxCodeCache;
        private TaxCodeInvoiceTotals invoiceTotals;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="DataAreaId"></param>
        public TaxCodeProvider(IApplication application)
            : this()
        {
            this.Application = application;
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public TaxCodeProvider()
        {
            this.Identifier = "LSRetailPosis.TaxService.DefaultTaxProvider";
            this.taxCodeCache = new Dictionary<CacheKey, List<TaxCode>>();
        }

        #region ITaxCodeProvider Members

        /// <summary>
        /// ID of this provider
        /// </summary>
        public string Identifier { get; protected set; }

        /// <summary>
        /// SQL selection text to read item tax
        /// </summary>
        protected virtual string TaxSelectSqlText
        {
            get
            {
                StringBuilder sb = new StringBuilder();

                sb.AppendLine("TAXONITEM.TAXITEMGROUP, ");
                sb.AppendLine("TAXONITEM.TAXCODE, ");
                sb.AppendLine("ISNULL(TAXDATA.TAXVALUE, 0.0) AS TAXVALUE, ");
                sb.AppendLine("ISNULL(TAXDATA.TAXLIMITMIN, 0.0) AS TAXLIMITMIN, ");
                sb.AppendLine("ISNULL(TAXDATA.TAXLIMITMAX, 0.0) AS TAXLIMITMAX, ");
                sb.AppendLine("TAXGROUPDATA.EXEMPTTAX, ");
                sb.AppendLine("TAXGROUPHEADING.TAXGROUPROUNDING, ");
                sb.AppendLine("TAXTABLE.TAXCURRENCYCODE, ");
                sb.AppendLine("TAXTABLE.TAXBASE, ");
                sb.AppendLine("TAXTABLE.TAXLIMITBASE, ");
                sb.AppendLine("TAXTABLE.TAXCALCMETHOD, ");
                sb.AppendLine("TAXTABLE.TAXONTAX, ");
                sb.AppendLine("TAXTABLE.TAXUNIT, ");
                sb.AppendLine("TAXTABLE.TAXINCLUDEINTAX, ");
                sb.AppendLine("ISNULL(TAXCOLLECTLIMIT.TAXMAX,0) AS TAXMAX, ");
                sb.AppendLine("ISNULL(TAXCOLLECTLIMIT.TAXMIN,0) AS TAXMIN ");

                return sb.ToString();
            }
        }

        private void AddTaxCode(CacheKey cacheKey, SqlDataReader reader, ITaxableItem taxableItem, Dictionary<string, TaxCode> codes)
        {
            TaxCode code = GetTaxCode(reader, taxableItem);

            codes.Add(code.Code, code);

            if (!taxCodeCache.ContainsKey(cacheKey))
                taxCodeCache[cacheKey] = new List<TaxCode>() { code };
            else
                taxCodeCache[cacheKey].Add(code);
        }

        /// <summary>
        /// Gets the tax code.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="taxableItem">The taxable item.</param>
        /// <returns>The taxcode object</returns>
        protected virtual TaxCode GetTaxCode(SqlDataReader reader, ITaxableItem taxableItem)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            return new TaxCode(
                reader["TAXCODE"] as string,
                taxableItem,
                reader["TAXITEMGROUP"] as string,
                reader["TAXCURRENCYCODE"] as string,
                (decimal)reader["TAXVALUE"],
                (decimal)reader["TAXLIMITMIN"],
                (decimal)reader["TAXLIMITMAX"],
                ((int)reader["EXEMPTTAX"] == 1),
                (TaxBase)reader["TAXBASE"],
                (TaxLimitBase)reader["TAXLIMITBASE"],
                (TaxCalculationMode)reader["TAXCALCMETHOD"],
                reader["TAXONTAX"] as string,
                reader["TAXUNIT"] as string,
                ((int)reader["TAXINCLUDEINTAX"] == 1),
                (decimal)reader["TAXMIN"],
                (decimal)reader["TAXMAX"],
                ((int)reader["TAXGROUPROUNDING"] == 1),
                this);
        }

        protected const int MaxPriorityTaxCode = 4;

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        protected static int TaxCodePriority(TaxCode code)
        {
            // Return codes to be processed in the following order:
            // 1. Amount per unit & Percent of net & Percent Gross on net
            // 2. Percent of tax
            // 3. Percent of gross (single tax)
            // 4. Percent of gross (all taxes)
            switch (code.TaxBase)
            {
                case TaxBase.AmountByUnit:
                case TaxBase.PercentPerNet:
                case TaxBase.PercentGrossOnNet:
                    return 1;
                case TaxBase.PercentPerTax:
                    return 2;
                case TaxBase.PercentPerGross:
                    return string.IsNullOrEmpty(code.TaxOnTax) ? MaxPriorityTaxCode : 3;
                default:
                    return 0;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        protected virtual ReadOnlyCollection<TaxCode> SortCodes(IEnumerable<TaxCode> codes)
        {
            return new ReadOnlyCollection<TaxCode>(
                codes.OrderBy(code =>
                {
                    return TaxCodePriority(code);
                }).ToList());
        }

        /// <summary>
        /// Add Extendtion sql condition to select data
        /// </summary>
        /// <param name="command"></param>
        protected virtual string AddTaxSelectSqlCondition(SqlCommand command)
        {
            return string.Empty;
        }

        /// <summary>
        /// Retrieves a list of TaxCodes for the given sale line item
        /// </summary>
        /// <remarks>No user Input or variable in string TaxSelectSqlText. No sql injection threat.</remarks>
        /// <param name="taxableItem"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "No user Input or variable in string TaxSelectSqlText. No sql injection threat.")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        protected virtual ReadOnlyCollection<TaxCode> GetTaxCodes(ITaxableItem taxableItem)
        {
            if (taxableItem == null)
            { 
                throw new ArgumentNullException("taxableItem");
            }

            RetailTransaction transaction = (RetailTransaction)taxableItem.RetailTransaction;
            string customerId = string.Empty;

            // If the line has an EndDate specified (usually because it's a Returned line), 
            // then use that value to calculate taxes, otherwise use BeginDate
            DateTime itemSaleDateTime = (taxableItem.EndDateTime <= NoDate) ? taxableItem.BeginDateTime : taxableItem.EndDateTime;

            if (transaction != null && transaction.Customer != null)
            {
                customerId = transaction.Customer.CustomerId;
            }

            CacheKey cacheKey = new CacheKey(taxableItem.ItemId,
                customerId,
                taxableItem.TaxGroupId,
                taxableItem.SalesTaxGroupId,
                itemSaleDateTime);

            if (taxCodeCache.ContainsKey(cacheKey))
            {
                List<TaxCode> taxCodes = taxCodeCache[cacheKey];

                // Update the lineItem object in cached Taxcode object (Everytime we get new SalesLine Object)
                taxCodes.ForEach(t => t.LineItem = taxableItem);

                return SortCodes(taxCodes);
            }

            NetTracer.Information("TaxCodeProvider::GetTaxCodes(): Quering database.");

            SqlConnection connection;
            string dataAreaId;
              //  SqlConnection connection = ApplicationSettings.Database.LocalConnection; //Application.Settings.Database.Connection;
            if (Application != null)
            {
                connection = Application.Settings.Database.Connection;
                dataAreaId = Application.Settings.Database.DataAreaID;
            }
            else
            {
                connection = ApplicationSettings.Database.LocalConnection;
                dataAreaId = LSRetailPosis.Settings.ApplicationSettings.Database.DATAAREAID; 
            }

            try
            {
                Dictionary<string, TaxCode> codes = new Dictionary<string, TaxCode>();

                bool useDefaultTaxGroups = (cacheKey.TaxGroupID == null) || (cacheKey.SalesTaxGroupID == null);

                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = connection;

                    string sb = string.Format(@"SELECT DISTINCT {0}    FROM TAXGROUPHEADING  ", TaxSelectSqlText);

                    if (useDefaultTaxGroups)
                    {
                        // #1 Look in the DB for the default Customer/Store tax group mapping
                        if (String.IsNullOrWhiteSpace(cacheKey.CustomerID))
                        {
                            sb += @"INNER JOIN RETAILSTORETABLE ON 
                                TAXGROUPHEADING.TAXGROUP = RETAILSTORETABLE.TAXGROUP 
                                AND  RETAILSTORETABLE.STORENUMBER = @STOREID ";

                            command.Parameters.AddWithValue("@STOREID", ApplicationSettings.Terminal.StoreId);
                        }
                        else
                        {
                            sb += @"INNER JOIN CUSTTABLE ON 
                                TAXGROUPHEADING.TAXGROUP = CUSTTABLE.TAXGROUP 
                                AND CUSTTABLE.DATAAREAID = @DATAAREAID 
                                AND CUSTTABLE.ACCOUNTNUM = @CUSTOMERID ";

                            command.Parameters.AddWithValue("@CUSTOMERID", cacheKey.CustomerID);
                        }
                    }

                    sb += @"INNER JOIN TAXGROUPDATA ON 
                        TAXGROUPDATA.DATAAREAID = @DATAAREAID 
                        AND TAXGROUPHEADING.TAXGROUP = TAXGROUPDATA.TAXGROUP 

                        INNER JOIN TAXONITEM ON 
                        TAXONITEM.DATAAREAID = @DATAAREAID 
                        AND TAXGROUPDATA.TAXCODE = TAXONITEM.TAXCODE  ";

                    if (useDefaultTaxGroups)
                    {
                        // Join against the Item's default Item tax group
                        sb += @"INNER JOIN INVENTTABLEMODULE ON 
                            INVENTTABLEMODULE.DATAAREAID = @DATAAREAID 
                            AND INVENTTABLEMODULE.TAXITEMGROUPID = TAXONITEM.TAXITEMGROUP  ";
                    }

                    sb += @"INNER JOIN TAXDATA ON 
                        TAXDATA.DATAAREAID = @DATAAREAID 
                        AND TAXONITEM.TAXCODE = TAXDATA.TAXCODE 

                        INNER JOIN TAXTABLE ON 
                        TAXTABLE.DATAAREAID = @DATAAREAID 
                        AND TAXTABLE.TAXCODE = TAXDATA.TAXCODE 

                        LEFT JOIN TAXCOLLECTLIMIT ON 
                        TAXCOLLECTLIMIT.DATAAREAID = @DATAAREAID 
                        AND TAXCOLLECTLIMIT.TAXCODE = TAXDATA.TAXCODE 
                        AND (TAXCOLLECTLIMIT.TAXFROMDATE IS NULL 
                        OR @TRANSACTIONDATE >= TAXCOLLECTLIMIT.TAXFROMDATE 
                        OR TAXCOLLECTLIMIT.TAXFROMDATE < @NODATEBOUNDRY ) 
                        AND (TAXCOLLECTLIMIT.TAXTODATE IS NULL 
                        OR @TRANSACTIONDATE < DATEADD(d, 1, TAXCOLLECTLIMIT.TAXTODATE) 
                        OR TAXCOLLECTLIMIT.TAXTODATE < @NODATEBOUNDRY) 

                        WHERE (TAXGROUPHEADING.DATAAREAID = @DATAAREAID)  ";

                    command.Parameters.AddWithValue("@DATAAREAID", dataAreaId);

                    if (useDefaultTaxGroups)
                    {
                        sb += @"AND (INVENTTABLEMODULE.ITEMID = @ITEMID) 
                            AND (INVENTTABLEMODULE.MODULETYPE = @MODULETYPE)  ";

                        command.Parameters.AddWithValue("@ITEMID", cacheKey.ItemID);
                        command.Parameters.AddWithValue("@MODULETYPE", (int)ModuleType.Sales);
                    }
                    else
                    {
                        // Filter against the item's current Item Tax Group and Customer/Store tax group
                        sb += @"AND TAXONITEM.TAXITEMGROUP = @ITEMTAXGROUP 
                            AND TAXGROUPHEADING.TAXGROUP = @SALESTAXGROUP  ";

                        command.Parameters.AddWithValue("@SALESTAXGROUP", cacheKey.SalesTaxGroupID ?? string.Empty);
                        command.Parameters.AddWithValue("@ITEMTAXGROUP", cacheKey.TaxGroupID ?? string.Empty);
                    }

                    // Currently only evaluate taxes against the current time.
                    // Note that the date value of '1900-01-01 00:00.000' is the marker for "no boundry".
                    sb += @"AND 
                        ((@TRANSACTIONDATE >= TAXDATA.TAXFROMDATE OR TAXDATA.TAXFROMDATE < @NODATEBOUNDRY ) 
                        AND (@TRANSACTIONDATE < DATEADD(d, 1, TAXDATA.TAXTODATE) OR TAXDATA.TAXTODATE < @NODATEBOUNDRY)) ";

                    command.Parameters.AddWithValue("@NODATEBOUNDRY", NoDate);
                    command.Parameters.AddWithValue("@TRANSACTIONDATE", cacheKey.SaleDateTime);

                    sb += AddTaxSelectSqlCondition(command);

                    command.CommandText = sb.ToString();

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        string taxCodeKey = string.Empty;

                        while (reader.Read())
                        {
                            taxCodeKey = reader["TAXCODE"] as string;

                            if (codes.ContainsKey(taxCodeKey))
                            {
                                // Add a new 'value' entry for an existing tax code
                                codes[taxCodeKey].TaxIntervals.Add(
                                    new TaxInterval(
                                        (decimal)reader["TAXLIMITMIN"],
                                        (decimal)reader["TAXLIMITMAX"],
                                        (decimal)reader["TAXVALUE"]));
                            }
                            else
                            {
                                AddTaxCode(cacheKey, reader, taxableItem, codes);
                            }
                        }
                    }
                }

                // Link any taxes which rely on other taxes
                foreach (TaxCode tax in codes.Values)
                {
                    if (!string.IsNullOrEmpty(tax.TaxOnTax)
                        && (tax.TaxBase == TaxBase.PercentPerTax || tax.TaxBase == TaxBase.PercentPerGross)
                        && codes.Keys.Contains(tax.TaxOnTax))
                    {
                        tax.TaxOnTaxInstance = codes[tax.TaxOnTax] as TaxCode;
                    }
                }

                return SortCodes(codes.Values);

            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "GetTaxCodes() failed in an Exception");
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Calculate taxes for the transaction
        /// </summary>
        /// <param name="retailTransaction"></param>
        public void CalculateTax(IRetailTransaction retailTransaction)
        {
            RetailTransaction transaction = retailTransaction as RetailTransaction;
            if (transaction == null)
                throw new ArgumentNullException("retailTransaction");

            // Clear the cache if transaction is changed.
            if (string.IsNullOrWhiteSpace(cachedTransactionID)
                || (taxCodeCache.Count > 0 && !cachedTransactionID.Equals(transaction.TransactionId, StringComparison.OrdinalIgnoreCase)))
            {
                cachedTransactionID = transaction.TransactionId;
                taxCodeCache.Clear();
            }

            // Reset cached value for tax code invoice totals.
            this.invoiceTotals = new TaxCodeInvoiceTotals();

            // at different we have different implementations of Itaxable. Flatten them into a list and loop.
            List<ITaxableItem> taxableItems = new List<ITaxableItem>();
            
            // Order level charges
            taxableItems.AddRange(transaction.MiscellaneousCharges);

            // Line Level 
            foreach (SaleLineItem lineItem in transaction.SaleItems)
            {
                // lineitem itself
                taxableItems.Add(lineItem);

                // associated charges
                taxableItems.AddRange(lineItem.MiscellaneousCharges);
            }

            // Calculate tax on order-level miscellaneous charges
            foreach (ITaxableItem taxableItem in taxableItems)
            {
                this.CalculateTax(taxableItem, transaction);
            }

            //Round by Tax Group if required.
            IEnumerable<string> groups = GetTaxGroupsToRound();

            foreach (string group in groups)
            {
                foreach (BaseSaleItem lineItem in transaction.SaleItems)
                {
                    RoundTaxGroup(lineItem, group);
                }
            }
        }

        /// <summary>
        /// Get list of unique Tax Groups that require rounding.
        /// </summary>
        /// <returns></returns>
        private IEnumerable<string> GetTaxGroupsToRound()
        {
            List<string> taxGroups = new List<string>();
            foreach (var taxGroup in taxCodeCache.Values)
            {
                foreach (TaxCode code in taxGroup.Where(c => c.TaxGroupRounding))
                {
                    if (!taxGroups.Contains(code.TaxGroup))
                    {
                        taxGroups.Add(code.TaxGroup);
                    }
                }
            }
            return taxGroups;
        }

        /// <summary>
        /// Round tax code amounts at the Tax Group level
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="taxGroup"></param>
        private static void RoundTaxGroup(BaseSaleItem lineItem, string taxGroup)
        {
            if (lineItem.TaxLines.Count > 0)
            {
                decimal roundedAmount = decimal.Zero;
                decimal roundedSum = decimal.Zero;
                decimal rawSum = decimal.Zero;
                decimal diff = decimal.Zero;

                IRounding rounding = TaxService.Tax.InternalApplication.Services.Rounding;
                string storeCurrency = ApplicationSettings.Terminal.StoreCurrency;

                // Sum up raw and rounded tax amounts
                foreach (ITaxItem taxLine in lineItem.TaxLines.Where(t => (!t.Exempt) && string.Equals(taxGroup, t.TaxGroup)))
                {
                    // Accumulate rounded and unrounded/raw amounts.  Tax.Amount is the raw value.
                    roundedAmount = rounding.Round(taxLine.Amount, storeCurrency, true, 1);
                    rawSum += taxLine.Amount;
                    roundedSum += roundedAmount;

                    // Set Tax.Amount to the rounded amount
                    taxLine.Amount = roundedAmount;

                    // Compute the difference between the sums.
                    // If we have accumulated enough extra decimals to cause raw to round up, then we'll see a difference.
                    diff = rounding.Round(rawSum, storeCurrency, true, 1) - roundedSum;

                    // Apply the difference against the current line
                    if (diff != decimal.Zero)
                    {
                        taxLine.Amount += diff;
                        roundedSum += diff;
                    }
                }
            }
        }

        /// <summary>
        /// Calculate tax on the given line item.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="retailTransaction">The retail transaction.</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void CalculateTax(ITaxableItem taxableItem, IRetailTransaction retailTransaction)
        {
            var codes = GetTaxCodes(taxableItem);

            LineTaxResult lineTaxResult = new LineTaxResult
            {
                HasExempt = false,
                TaxRatePercent = decimal.Zero,
                TaxAmount = decimal.Zero,
                ExemptAmount = decimal.Zero
            };

            foreach (TaxCode code in codes)
            {
                lineTaxResult.TaxAmount += code.CalculateTaxAmount(codes, this.invoiceTotals);

                // sum up the amounts that are exempt
                if (code.Exempt)
                {
                    lineTaxResult.HasExempt = true;
                    lineTaxResult.ExemptAmount += lineTaxResult.TaxAmount;
                }
            }

            // Set the 'virtual tax rate', if extended price is ZERO, then just add the full amount
            decimal extendedPrice = (taxableItem.Price * Math.Abs(taxableItem.Quantity));
            if (extendedPrice == decimal.Zero)
            {
                extendedPrice = decimal.One;
            }

            lineTaxResult.TaxRatePercent = ((lineTaxResult.TaxAmount * 100) / extendedPrice);
            SetLineItemTaxRate(taxableItem, lineTaxResult);
        }

        public virtual decimal GetBasePriceForTaxIncluded(ITaxableItem taxableItem, ReadOnlyCollection<TaxCode> codes)
        {
            // check to see if we can do the 'simple' Inclusive algorithm
            bool simpleBasis = codes.All(c =>
                (c.TaxBase == TaxBase.PercentPerNet || c.TaxBase == TaxBase.PercentGrossOnNet)
                && (c.TaxLimitMin == decimal.Zero && c.TaxLimitMax == decimal.Zero));
            bool collectLimits = codes.Any(c => (c.CollectLimitMax != decimal.Zero || c.CollectLimitMin != decimal.Zero));
            bool multiplePercentage = codes.Any(c => (c.TaxIntervals.Count > 1));

            if (simpleBasis && !collectLimits && !multiplePercentage)
            {
                // Get base price for Simple TaxInclusive calculation
                return GetBasePriceSimpleTaxIncluded(taxableItem, codes);
            }
            else
            {
                // Get base price for Full TaxInclusive calculation
                return GetBasePriceAdvancedTaxIncluded(taxableItem, codes, collectLimits);
            }
        }

        /// <summary>
        /// Simple version of TaxIncluded algorithm for Tax Code collections that are not based on:
        /// - intervals,
        /// - limits,
        /// - collection limits
        /// - total invoice
        /// </summary>
        /// <param name="lineItem">The taxable item.</param>
        /// <param name="codes">The codes.</param>
        /// <returns>base price</returns>
        private static decimal GetBasePriceSimpleTaxIncluded(
            ITaxableItem lineItem, ReadOnlyCollection<TaxCode> codes)
        {
            // accumulation of % based tax
            decimal fullLineTaxRate = decimal.Zero;

            // accumulation of amount based tax
            decimal fullLineUnitTax = decimal.Zero;
            decimal nonExemptLineUnitTax = decimal.Zero;

            // 1. Determine sum of all AmountByUnit taxes (ref: AX\Classes\Tax.AmountExclTax() - line 222)
            decimal codeValue = decimal.Zero;

            // Reference dev item 5747
            foreach (TaxCode code in codes.Where(c => c.TaxBase == TaxBase.AmountByUnit))
            {
                codeValue = code.Calculate(codes, false);  // Amount by units don't depend on basePrice
                fullLineUnitTax += codeValue;
                nonExemptLineUnitTax += (code.Exempt) ? decimal.Zero : codeValue;
            }

            // 2. Determine sum of all tax rates for non-AmountByUnit taxes (ref: AX\Classes\Tax.AmountExclTax() - line 331)
            foreach (TaxCode code in codes.Where(c => c.TaxBase != TaxBase.AmountByUnit))
            {
                if (code.TaxBase == TaxBase.PercentPerGross && string.IsNullOrEmpty(code.TaxOnTax))
                {
                    //Sum all OTHER taxes...
                    codeValue = codes.Sum(c => (c.TaxBase == TaxBase.AmountByUnit) ? decimal.Zero : c.PercentPerTax());

                    //...and then apply the Gross tax on top of that
                    codeValue *= code.PercentPerTax() / 100;

                    // Add this rate to the running total.
                    fullLineTaxRate += codeValue;
                }
                else
                {
                    // Add this rate to the running total.
                    codeValue = code.PercentPerTax();
                    fullLineTaxRate += codeValue;
                }
            }

            // 3. Back calculate the Price based on tax rates, start with the Price that includes ALL taxes
            decimal taxBase = lineItem.NetAmountWithAllInclusiveTaxPerUnit - fullLineUnitTax;
            return (taxBase * 100) / (100 + fullLineTaxRate);
        }

        /// <summary>
        /// /// Advanced version of TaxIncluded algorithm for tax codes with the full range of supported tax properties
        /// </summary>
        /// <param name="taxableItem">The taxable item.</param>
        /// <param name="codes">The codes.</param>
        /// <param name="collectLimits">if set to <c>true</c> [collect limits].</param>
        /// <returns>the base price</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1505:AvoidUnmaintainableCode", Justification = "Method is a translation of AX algorithm")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Method is a translation of AX algorithm")]
        private static decimal GetBasePriceAdvancedTaxIncluded(
            ITaxableItem taxableItem, ReadOnlyCollection<TaxCode> codes, bool collectLimits)
        {
            // accumulation of amount based tax
            decimal fullLineUnitTax = decimal.Zero;
            decimal nonExemptLineUnitTax = decimal.Zero;
            decimal codeValue = decimal.Zero;

            //AX variables
            decimal endAmount = taxableItem.NetAmountWithAllInclusiveTaxPerUnit;   //endAmount will be the final price w/o tax
            decimal lineQty = taxableItem.Quantity != decimal.Zero ? Math.Abs(taxableItem.Quantity) : 1;
            decimal percentGross = decimal.Zero;
            int sign = 1;

            // 3.
            decimal taxLimitMax = decimal.Zero;
            decimal taxLimitMin = decimal.Zero;
            decimal startAmount = decimal.Zero;

            // 3a...
            decimal taxCalc = decimal.Zero;
            decimal baseCur;
            // Tax Amount deducted for a given Code
            Dictionary<string, decimal> deductedTax = new Dictionary<string, decimal>();

            // 3b ...
            decimal percentTotal;
            decimal tmpBase;

            // 3c..
            // Whether or not a Code needs to be removed from the sum of percent rates
            Dictionary<string, bool> removePercent = new Dictionary<string, bool>();

            //3d.
            decimal totalTax = decimal.Zero;
            // Whether or not the Code needs to be calculated
            Dictionary<string, bool> calcTax = new Dictionary<string, bool>();

            //
            // Begin Tax included calculation
            //

            //0. Initialize the supporting collections
            foreach (TaxCode code in codes)
            {
                deductedTax[code.Code] = decimal.Zero;
                removePercent[code.Code] = false;
                calcTax[code.Code] = true;
            }

            //
            // 1. Remove all AmountByUnit taxes
            //
            foreach (TaxCode code in codes.Where(c => c.TaxBase == TaxBase.AmountByUnit))
            {
                codeValue = code.Calculate(codes, false); // Reference dev item 5748.
                fullLineUnitTax += codeValue;
                nonExemptLineUnitTax += (code.Exempt) ? decimal.Zero : codeValue;
                calcTax[code.Code] = false;
            }

            percentGross = codes.Where(c => c.TaxBase == TaxBase.PercentPerGross)
                                .Sum(c => c.PercentPerTax(Math.Abs(endAmount)));
            endAmount -= (Math.Abs(nonExemptLineUnitTax) / lineQty) * (100 + percentGross) /100;

            //
            // 2. Record the sign, and then continue using the magnitude of endAmount
            //
            sign = (endAmount < decimal.Zero) ? -1 : 1;
            endAmount = Math.Abs(endAmount);

            //
            // 3.
            //
            while (startAmount < endAmount)
            {
                // 3a Consider interval limits
                taxCalc = decimal.Zero;
                taxLimitMax = decimal.Zero;

                foreach (TaxCode code in codes)
                {
                    if (code.TaxCalculationMethod == TaxCalculationMode.FullAmounts)
                    {
                        taxLimitMax = decimal.Zero;
                    }
                    else
                    {
                        if (IsStoreCurrency(code))
                        {
                            baseCur = TaxService.Tax.InternalApplication.Services.Currency.CurrencyToCurrency(
                                ApplicationSettings.Terminal.StoreCurrency, code.Currency, taxLimitMin);
                        }
                        else
                        {
                            baseCur = taxLimitMin;
                        }
                        baseCur += 1;

                        // if 'baseCur' falls into an interval
                        if (code.TaxIntervals.Exists(baseCur))
                        {
                            // get the Upper limit of the interval that 'baseCur'/'taxLimitMin' falls into
                            decimal amount = code.TaxIntervals.Find((taxLimitMin + 1)).TaxLimitMax;
                            taxLimitMax = (amount != decimal.Zero && amount < endAmount) ? amount : endAmount;
                        }
                    }

                    taxCalc += deductedTax[code.Code];
                }

                // 3b. Sum up all the Tax Percentage Rates
                percentTotal = 0;
                tmpBase = (taxLimitMax > decimal.Zero) ? taxLimitMax : endAmount;

                foreach (TaxCode code in codes.Where(c => calcTax[c.Code]))
                {
                    percentTotal += GetPercentPerTax(code, tmpBase, codes);
                }

                decimal taxMax;
                decimal baseInclTax;
                decimal baseExclTax;

                // 3c.
                // if this is the last interval??
                if (taxLimitMax == decimal.Zero)
                {
                    // Forward calculate taxes to see if we exceed the CollectLimit
                    foreach (TaxCode code in codes.Where(c => calcTax[c.Code]))
                    {
                        taxMax = code.CollectLimitMax;
                        baseInclTax = endAmount - taxLimitMin - taxCalc;
                        baseExclTax = baseInclTax * 100 / (100 + percentTotal);

                        if (taxMax != decimal.Zero)
                        {
                            tmpBase = endAmount;

                            decimal percent = GetPercentPerTax(code, tmpBase, codes);

                            if ((deductedTax[code.Code] + baseExclTax * percent / 100) > taxMax)
                            {
                                deductedTax[code.Code] = taxMax;
                                removePercent[code.Code] = true;
                            }
                        }
                    }

                    //3d.
                    // Now remove any rates that exceed their LimitMax
                    foreach (TaxCode code in codes)
                    {
                        if (removePercent[code.Code] && calcTax[code.Code])
                        {
                            tmpBase = endAmount;
                            percentTotal -= GetPercentPerTax(code, tmpBase, codes);
                            calcTax[code.Code] = false;
                        }

                        taxCalc += deductedTax[code.Code];
                    }
                }

                //4. Compute tax adjusted for limits
                totalTax = decimal.Zero;
                foreach (TaxCode code in codes.Where(c => c.TaxBase != TaxBase.AmountByUnit))
                {
                    if (calcTax[code.Code])
                    {
                        tmpBase = (taxLimitMax > decimal.Zero) ? taxLimitMax : endAmount;

                        decimal percent = GetPercentPerTax(code, tmpBase, codes);

                        if (taxLimitMax > decimal.Zero && taxLimitMax < endAmount)
                        {
                            deductedTax[code.Code] += (taxLimitMax - taxLimitMin) * percent / 100;
                        }
                        else
                        {
                            baseInclTax = endAmount - taxLimitMin - taxCalc;
                            baseExclTax = baseInclTax * 100 / (100 + percentTotal);
                            deductedTax[code.Code] += baseExclTax * percent / 100;
                        }

                        taxMax = code.CollectLimitMax;

                        if (taxMax > decimal.Zero && deductedTax[code.Code] > taxMax)
                        {
                            deductedTax[code.Code] = taxMax;
                        }
                    }

                    totalTax += deductedTax[code.Code];
                }

                if (taxLimitMax > decimal.Zero)
                {
                    taxLimitMin = taxLimitMax;
                    startAmount = taxLimitMin + totalTax;
                }
                else
                {
                    startAmount = endAmount;
                }

            } // END if( startAmount < endAmount)

            // 5a. Total up taxes
            foreach (TaxCode code in codes)
            {
                if (collectLimits && (deductedTax[code.Code] < code.CollectLimitMin))
                {
                    totalTax -= deductedTax[code.Code];
                    deductedTax[code.Code] = decimal.Zero;
                }

                if (IsStoreCurrency(code))
                {
                    taxCalc = TaxService.Tax.InternalApplication.Services.Rounding.TaxRound(
                        deductedTax[code.Code], code.Code);
                }
                else
                {
                    taxCalc = deductedTax[code.Code];
                }

                totalTax += (taxCalc - deductedTax[code.Code]);
                deductedTax[code.Code] = taxCalc;
            }

            //5b. Determine base price
            return (endAmount - totalTax) * sign;
        }

        /// <summary>
        /// Get the PercentRate for a given Tax (takes Gross Taxes into account)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="taxBase"></param>
        /// <param name="otherCodes"></param>
        /// <returns></returns>
        private static decimal GetPercentPerTax(TaxCode code, decimal taxBase, ReadOnlyCollection<TaxCode> otherCodes)
        {
            decimal percent = decimal.Zero;

            if (code.TaxBase == TaxBase.PercentPerGross)
            {
                decimal otherPercents = decimal.Zero;
                foreach (TaxCode t in otherCodes.Where(c => (c.Code != code.Code && c.TaxBase != TaxBase.AmountByUnit)))
                {
                    otherPercents += t.PercentPerTax(taxBase);
                }

                decimal grossPercent = code.PercentPerTax(taxBase);

                //Gross Percent needs to be expressed with respect to the original item price:
                // ActualPercent = GrossPercent * (full price + other taxes)/100
                percent = grossPercent * (100 + otherPercents) / 100m;
            }
            else
            {
                percent = code.PercentPerTax(taxBase);
            }

            return percent;
        }

        /// <summary>
        /// Whether or not a given TaxCode is in the Store Currency
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private static bool IsStoreCurrency(TaxCode code)
        {
            return string.IsNullOrEmpty(code.Currency)
                || code.Currency.Equals(ApplicationSettings.Terminal.StoreCurrency, StringComparison.OrdinalIgnoreCase);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        protected static void SetLineItemTaxRate(ITaxableItem taxableItem, decimal taxAmount)
        {
            decimal extendedPrice = (taxableItem.NetAmountPerUnit * taxableItem.Quantity);

            if (taxableItem.RetailTransaction.TaxIncludedInPrice)
            {
                extendedPrice -= taxAmount;
            }

            if (extendedPrice == decimal.Zero)
            {
                extendedPrice = 1;
            }

            taxableItem.TaxRatePct += ((taxAmount * 100) / extendedPrice);
        }

        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Grandfather")]
        protected static void SetLineItemTaxRate(ITaxableItem taxableItem, LineTaxResult lineTaxResult)
        {
            // Ignore any portion of the TaxAmount that is 'Exempt' when computing the rate.
            decimal amount = lineTaxResult.TaxAmount - lineTaxResult.ExemptAmount;
            SetLineItemTaxRate(taxableItem, amount);
        }

        /// <summary>
        /// Apply collect limits for the whole transaction
        /// </summary>
        /// <param name="retailTransaction"></param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        private void ApplyCollectionLimits(RetailTransaction retailTransaction)
        {
            List<TaxCode> codes = new List<TaxCode>();
            foreach (ISaleLineItem lineItem in retailTransaction.SaleItems)
            {
                codes.Union(GetTaxCodes(lineItem));
            }

            // For all tax codes which are Limited per Invoice and have a Collect Limit
            foreach (TaxCode code in codes.Where(c =>
                (c.TaxLimitBase == TaxLimitBase.InvoiceWithoutVat || c.TaxLimitBase == TaxLimitBase.InvoiceWithVat)
                && (c.CollectLimitMax > decimal.Zero)))
            {
                //Sum up total of all tax for this code on the whole transaction
                decimal taxAmount = retailTransaction.SaleItems.Sum(
                    saleItem => saleItem.TaxLines.Where(taxLine => taxLine.TaxCode == code.Code).Sum(taxLine => taxLine.Amount));

                // Collect limit exceeded
                if (taxAmount > code.CollectLimitMax)
                {
                    decimal factor = code.CollectLimitMax / taxAmount;
                    foreach (ISaleLineItem saleItem in retailTransaction.SaleItems)
                    {
                        // Adjust each tax item for this code
                        ITaxItem tax = saleItem.TaxLines.First(t => t.TaxCode == code.Code);
                        if (tax != null)
                        {
                            tax.Amount *= factor;
                        }
                    }
                }
            }
        }

        #endregion
    }
}
