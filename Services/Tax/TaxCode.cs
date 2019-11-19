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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LSRetailPosis.DataAccess;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessObjects;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.Tax
{
    /// <summary>
    /// Representation of a single tax code
    /// </summary>
    [DebuggerDisplay("{Code}, {TaxBase}, {TaxLimitBase}")]
    public class TaxCode //: ITaxCode
    {

        #region properties
        /// <summary>
        /// TaxCode
        /// </summary>
        public string Code { get; protected set; }

        /// <summary>
        /// TaxGroup that this Tax Code belongs to
        /// </summary>
        public string TaxGroup { get; protected set; }

        /// <summary>
        /// Currency that this tax is calculated in
        /// </summary>
        public string Currency { get; protected set; }

        /// <summary>
        /// Value/Rate of the tax
        /// </summary>
        /// <remarks>REMOVE in favor of TaxIntervals</remarks>
        public decimal Value { get; private set; }

        /// <summary>
        /// Minimum amount required to calculate this tax.
        /// </summary>
        /// <remarks>REMOVE in favor of TaxIntervals collection. If the price is less than this, then the tax will not be calculated</remarks>
        public decimal TaxLimitMin { get; private set; }

        /// <summary>
        /// Maximum amount required to calculate this tax
        /// </summary>
        /// <remarks>REMOVE in favor of TaxIntervals collection. If the price is more than this, then the tax will not be calculated</remarks>
        public decimal TaxLimitMax { get; private set; }

        /// <summary>
        /// Collection limits, the minimum tax that can be collected
        /// </summary>
        /// <remarks>If the calculated tax is less than this, then ZERO tax will be collected</remarks>
        public decimal CollectLimitMin { get; protected set; }

        /// <summary>
        /// Collection limits, the maximum tax that can be collected
        /// </summary>
        /// <remarks>If the calculated tax is more than this, then THIS will be the tax amount collected</remarks>
        public decimal CollectLimitMax { get; protected set; }

        /// <summary>
        /// Whether or not this tax is exempt
        /// </summary>
        public bool Exempt { get; protected set; }

        /// <summary>
        /// Origin from which sales tax is calculated
        /// </summary>
        public TaxBase TaxBase { get; protected set; }

        /// <summary>
        /// Basis of sales tax limits
        /// </summary>
        public TaxLimitBase TaxLimitBase { get; protected set; }

        /// <summary>
        /// Whether tax is calculated for entire amounts or for intervals
        /// </summary>
        public TaxCalculationMode TaxCalculationMethod { get; protected set; }

        /// <summary>
        /// TaxCode of the other sales tax that this tax is based on.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TaxOn", Justification = "Cannot change public API.")]
        public string TaxOnTax { get; protected set; }

        /// <summary>
        /// Unit for calculating per-unit amounts
        /// </summary>
        public string Unit { get; protected set; }

        /// <summary>
        /// Whether tax is calculated before sales tax or not. Mainly used for duty charges.
        /// </summary>
        public bool TaxIncludeInTax { get; protected set; }

        /// <summary>
        /// TaxCode instance refered by TaxOnTax property
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "TaxOn", Justification = "Cannot change public API.")]
        public TaxCode TaxOnTaxInstance { get; set; }

        /// <summary>
        /// Collection of tax rate intervals defined for this tax code
        /// </summary>
        internal Collection<TaxInterval> TaxIntervals { get; private set; }

        public ITaxableItem LineItem { get; internal set; }

        public decimal TaxValue { get; set; }

        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Amt", Justification = "Cannot change public API.")]
        public decimal AmtPerUnitValue { get; set; }

        /// <summary>
        /// Get tax basis. A value used as starting point for tax formula.
        /// </summary>
        public decimal TaxBasis { get; protected set; }

        /// <summary>
        /// Whether or not this code should be rounded at the Tax Group scope.
        /// </summary>
        public bool TaxGroupRounding { get; set; }

        /// <summary>
        /// The tax code provider that created this instance.
        /// </summary>
        protected TaxCodeProvider Provider { get; private set; }

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public TaxCode(decimal value, decimal taxLimitMin, decimal taxLimitMax, TaxCodeProvider provider)
        {
            this.TaxIntervals = new Collection<TaxInterval>(new List<TaxInterval>(1));
            // should this be removed in favor of intervals?
            this.Value = value;
            this.TaxLimitMin = taxLimitMin;
            this.TaxLimitMax = taxLimitMax;
            this.Provider = provider;

            TaxIntervals.Add(new TaxInterval(taxLimitMin, taxLimitMax, value));
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="code">TaxCode</param>
        /// <param name="lineItem">The line item which the tax is being applied to</param>
        /// <param name="taxGroup">TaxGroup that this Tax Code belongs to</param>
        /// <param name="currency">Currency that this tax is calculated in</param>
        /// <param name="value">Value/Rate of the tax</param>
        /// <param name="limitMin">Minimum amount required to calculate this tax.</param>
        /// <param name="limitMax">Maximum amount required to calculate this tax</param>
        /// <param name="exempt">Whether or not this tax is exempt</param>
        /// <param name="taxBase">Origin from which sales tax is calculated</param>
        /// <param name="limitBase">Basis of sales tax limits</param>
        /// <param name="method">Whether tax is calculated for entire amounts or for intervals</param>
        /// <param name="taxOnTax">TaxCode of the other sales tax that this tax is based on.</param>
        /// <param name="unit">Unit for calculating per-unit amounts</param>
        /// <param name="taxIncludeInTax">Whether tax is calculated before sales tax or not.</param>
        /// <param name="collectMin">Collection limits, the minimum tax that can be collected</param>
        /// <param name="collectMax">Collection limits, the maximum tax that can be collected</param>
        /// <param name="groupRounding">Whether or not this code should be rounded at the Tax Group scope.</param>
        /// <param name="provider">The tax code provider that created this instance.</param>
        public TaxCode(
            string code,
            ITaxableItem lineItem,
            string taxGroup,
            string currency,
            decimal value,
            decimal limitMin,
            decimal limitMax,
            bool exempt,
            TaxBase taxBase,
            TaxLimitBase limitBase,
            TaxCalculationMode method,
            string taxOnTax,
            string unit,
            bool taxIncludeInTax,
            decimal collectMin,
            decimal collectMax,
            bool groupRounding,
            TaxCodeProvider provider)
            : this(value, limitMin, limitMax, provider)
        {
            this.Code = code;
            this.LineItem = lineItem;
            this.TaxGroup = taxGroup;
            this.Currency = currency;
            this.Exempt = exempt;
            this.TaxBase = taxBase;
            this.TaxLimitBase = limitBase;
            this.TaxCalculationMethod = method;
            this.TaxOnTax = taxOnTax;
            this.Unit = unit;
            this.TaxIncludeInTax = taxIncludeInTax; // calculate before sales tax case : for duty charges
            this.CollectLimitMax = collectMax;
            this.CollectLimitMin = collectMin;
            this.TaxGroupRounding = groupRounding;
        }

        /// <summary>
        /// Gets a value indicating whether the price includes the tax amount.
        /// </summary>
        /// <value>
        /// If <c>true</c> then tax is included in the price; otherwise, <c>false</c>.
        /// </value>
        public virtual bool TaxIncludedInPrice
        {
            get { return this.LineItem.RetailTransaction.TaxIncludedInPrice; }
        }

        /// <summary>
        /// Gets raw, non-rounded calculated tax value.
        /// </summary>
        public decimal TaxAmountNonRounded { get; private set; }

        /// <summary>
        /// Calculates tax for this code for the line item.
        /// Updates the line item by adding a new Tax Item
        /// </summary>
        /// <param name="codes">The collection of tax codes.</param>
        /// <param name="invoiceTotals">The current invoice totals for this tax code.</param>
        /// <returns>The calculated amount of tax.</returns>
        public virtual decimal CalculateTaxAmount(ReadOnlyCollection<TaxCode> codes, TaxCodeInvoiceTotals invoiceTotals)
        {
            decimal taxAmount = decimal.Zero;

            this.LineItem.TaxGroupId = this.TaxGroup;
            taxAmount = this.TaxIncludedInPrice ? CalculateTaxIncluded(codes) : CalculateTaxExcluded(codes);

            if (!this.TaxGroupRounding)
            {
                // Adjust tax code amount if tax code is based on invoice total.
                taxAmount = invoiceTotals.AddTaxCodeAmountToInvoiceAndAdjust(this, this.TaxAmountNonRounded, taxAmount);
            }

            // record amounts on line item
            ITaxItem taxItem = TaxService.Tax.InternalApplication.BusinessLogic.Utility.CreateTaxItem();

            taxItem.Amount          = taxAmount;
            taxItem.Percentage      = this.Value;
            taxItem.TaxCode         = this.Code;
            taxItem.TaxGroup        = this.TaxGroup;
            taxItem.Exempt          = this.Exempt;
            taxItem.IncludedInPrice = this.TaxIncludedInPrice;
            taxItem.TaxBasis        = this.TaxBasis;

            this.LineItem.Add(taxItem);

            return taxAmount;
        }

        /// <summary>
        /// Calculates tax for this tax code
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="calculateBasePrice">Base price needs to be calculated</param>
        /// <returns>The calculated tax</returns>
        public decimal Calculate(ReadOnlyCollection<TaxCode> codes, bool calculateBasePrice)
        {
            decimal calculationBase;
            decimal limitBase;
            decimal taxAmount = decimal.Zero;
            decimal taxValue = this.Value;  // When we add support for Intervals this will vary

            bool taxInStoreCurrency = (String.IsNullOrEmpty(this.Currency)
                || this.Currency.Equals(ApplicationSettings.Terminal.StoreCurrency, StringComparison.OrdinalIgnoreCase));

            //
            // 1a. Determine the base for the tax calculation
            //
            GetBases(codes, taxInStoreCurrency, calculateBasePrice, out calculationBase, out limitBase);

            // Round the tax basis based on the tax rounding rules for this tax-code to avoid accomulated rounding issue
            this.TaxBasis = TaxService.Tax.InternalApplication.Services.Rounding.TaxRound(calculationBase, this.Code);

            //
            // 1b. Determine how many intervals are needed for calculation
            //
            IEnumerable<TaxInterval> calculationIntervals = GetIntervals(limitBase);

            //
            // 2. Calculate the tax amount
            //
            foreach (TaxInterval interval in calculationIntervals)
            {
                decimal intervalBase;
                if (this.TaxCalculationMethod == TaxCalculationMode.FullAmounts || this.TaxBase == TaxBase.AmountByUnit)
                {
                    // use the whole amount for each interval (which should only be ONE in practice)
                    intervalBase = calculationBase;
                }
                else
                {
                    // use the segment of the base that falls into this interval
                    if (interval.TaxLimitMax == decimal.Zero)
                    {
                        // Max of ZERO signals infinite upper bound, so just use the full amount of the base
                        intervalBase = (calculationBase - interval.TaxLimitMin);
                    }
                    else
                    {
                        // Otherwise, use the segment of the base that is bounded by the min and max.
                        intervalBase = Math.Min(calculationBase, interval.TaxLimitMax) - interval.TaxLimitMin;
                    }
                }

                switch (this.TaxBase)
                {
                    case TaxBase.AmountByUnit:
                        // Quantity * TaxValueAmount
                        taxAmount += intervalBase * interval.Value;
                        break;

                    case TaxBase.PercentPerNet:
                    case TaxBase.PercentPerGross:
                    case TaxBase.PercentPerTax:
                        // Price * TaxValue
                        // (Price+Taxes) * TaxValue
                        // (OtherTaxAmount) * TaxValue
                        taxAmount += (intervalBase * interval.Value) / 100;
                        break;

                    case TaxBase.PercentGrossOnNet:
                        // Price * PGON(TaxValue)
                        taxAmount += (intervalBase * PercentGrossOnNet(interval.Value)) / 100;
                        break;
                }
            }

            //
            // 2c. Margin base: Per unit, line, invoice
            //
            switch (this.TaxLimitBase)
            {
                case TaxLimitBase.UnitWithoutVat:
                case TaxLimitBase.UnitWithVat:
                    // taxes were calculated per single unit, so multiply by the line quantity
                    // Note:  Abs(Qty) required as bases are all positive during Calculate().
                    taxAmount *= Math.Abs(this.LineItem.Quantity);
                    break;

                case TaxLimitBase.LineWithoutVat:
                case TaxLimitBase.LineWithVat:
                    //Do nothing else, taxes were already calculated for the whole line
                    break;

                default:
                    //do nothing else
                    break;
            }

            //
            // 3. If the tax was in a different currency, then convert the computed amount back into the store currency
            //
            if (!taxInStoreCurrency)
            {
                taxAmount = TaxService.Tax.InternalApplication.Services.Currency.CurrencyToCurrency(
                    this.Currency,
                    ApplicationSettings.Terminal.StoreCurrency,
                    taxAmount);
            }

            //4. Adjust tax sign per sign of the Line Item quantity.
            taxAmount *= Math.Sign(this.LineItem.Quantity);

            // Save non-rounded value for "Net amount of invoice balance" adjustment use.
            this.TaxAmountNonRounded = taxAmount;

            //
            // 5. Round the result based on the tax rounding rules for this tax-code
            //
            if (!this.TaxGroupRounding)
            {
                taxAmount = TaxService.Tax.InternalApplication.Services.Rounding.TaxRound(taxAmount, this.Code);
            }

            return taxAmount;
        }

        /// <summary>
        /// Get the intervals used for determing the tax rate/values
        /// </summary>
        /// <param name="limitBase"></param>
        /// <returns></returns>
        private IEnumerable<TaxInterval> GetIntervals(decimal limitBase)
        {
            if (this.TaxCalculationMethod == TaxCalculationMode.FullAmounts)
            {
                // ONLY return the FIRST interval where price is in the closed interval (including Min/Max). - This is AX behaviour
                // Order by Tax Min limit, so that it is ensured, we picked the tax interval matching with the least TaxMin value
                return new Collection<TaxInterval>() { this.TaxIntervals.OrderBy(t => t.TaxLimitMin).FirstOrDefault(t => t.WholeAmountInInterval(limitBase)) };
            }
            else if (this.TaxCalculationMethod == TaxCalculationMode.Interval)
            {
                // return ANY interval where the Price is greater than the Min
                return this.TaxIntervals.Where(t => t.AmountInInterval(limitBase));
            }

            // return an empty list
            return new TaxInterval[0];
        }
        /// <summary>
        /// returns the calculation base for AmountPerUnit tax calculations
        /// </summary>
        public decimal AmountPerUnitCalculationBase
        {
            get
            {
                return (TaxLimitBase == TaxLimitBase.UnitWithoutVat || TaxLimitBase == TaxLimitBase.UnitWithVat) ?
                    decimal.One : Math.Abs(this.LineItem.Quantity);
            }
        }

        /// <summary>
        /// Get the bases for tax calculations
        /// </summary>
        /// <param name="codes">The collection of tax codes.</param>
        /// <param name="taxInStoreCurrency">if set to <c>true</c> [tax in store currency].</param>
        /// <param name="calculateBasePrice">if set to <c>true</c> [Calculate the base price].</param>
        /// <param name="calculationBase">The calculation base.</param>
        /// <param name="limitBase">The limit base.</param>
        protected virtual void GetBases(ReadOnlyCollection<TaxCode> codes,
            bool taxInStoreCurrency,
            bool calculateBasePrice,
            out decimal calculationBase,
            out decimal limitBase)
        {
            decimal basePrice = decimal.Zero;
            if (calculateBasePrice)
            {
                basePrice = this.TaxIncludedInPrice ?
                    Provider.GetBasePriceForTaxIncluded(this.LineItem, codes) :
                    LineItem.NetAmountPerUnit + GetApplicableDutyTaxes(this.LineItem, codes);
            }

            //1. Get initial value for the Calculation Base
            switch (this.TaxBase)
            {
                case TaxBase.PercentPerTax:

                    // Base is the amount of the other tax
                    switch (this.TaxLimitBase)
                    {
                        case TaxLimitBase.InvoiceWithoutVat:
                        case TaxLimitBase.InvoiceWithVat:
                            calculationBase = Math.Abs(this.CalculateTaxOnTax(this.LineItem.RetailTransaction));
                            break;
                        case TaxLimitBase.UnitWithoutVat:
                        case TaxLimitBase.UnitWithVat:
                            // if this tax's Limit is per-unit, then we need to convert the existing tax amounts from per-line to per-unit
                            decimal quantity = (this.LineItem.Quantity == decimal.Zero) ? decimal.One : this.LineItem.Quantity;
                            calculationBase = Math.Abs(this.CalculateTaxOnTax()) / Math.Abs(quantity);
                            break;
                        default:
                            calculationBase = Math.Abs(this.CalculateTaxOnTax());
                            break;
                    }
                    break;

                case TaxBase.PercentPerGross:

                    // Base is the price + other taxes
                    calculationBase = basePrice;

                    // If the Limit base is NOT per-unit, then we need to factor in the line quanity
                    if (TaxLimitBase != TaxLimitBase.UnitWithoutVat && TaxLimitBase != TaxLimitBase.UnitWithVat)
                    {
                        calculationBase *= Math.Abs(this.LineItem.Quantity);
                    }

                    if (!string.IsNullOrEmpty(this.TaxOnTax))
                    {
                        // Base is the Price + the amount of a single other tax
                        calculationBase += Math.Abs(this.CalculateTaxOnTax());
                    }
                    else
                    {
                        // Base is the Price + all other taxes
                        calculationBase += Math.Abs(TaxCode.SumAllTaxAmounts(this.LineItem));
                    }

                    break;

                case TaxBase.AmountByUnit:
                    calculationBase = AmountPerUnitCalculationBase;
                    break;

                case TaxBase.PercentPerNet:
                case TaxBase.PercentGrossOnNet:
                default:

                    // Base is the Price
                    calculationBase = basePrice;

                    // If the Limit base is NOT per-unit, then we need to factor in the line quanity
                    if (TaxLimitBase != TaxLimitBase.UnitWithoutVat && TaxLimitBase != TaxLimitBase.UnitWithVat)
                    {
                        calculationBase *= Math.Abs(this.LineItem.Quantity);
                    }

                    break;
            }

            //3. Set Limit Base
            if (this.TaxBase == TaxBase.AmountByUnit)
            {
                // Base for limits/intervals is base-quantity * price
                limitBase = calculationBase * basePrice;

                // Convert limit base to Tax currency, if different
                if (!taxInStoreCurrency)
                {
                    limitBase = TaxService.Tax.InternalApplication.Services.Currency.CurrencyToCurrency(
                        ApplicationSettings.Terminal.StoreCurrency,
                        this.Currency,
                        limitBase);
                }

                // If the tax is calculated in a different UOM, then convert if possible
                // this is only applicable for lineItem taxes.
                BaseSaleItem saleLineItem = this.LineItem as BaseSaleItem;

                if (saleLineItem != null &&
                    !string.Equals(this.Unit, this.LineItem.SalesOrderUnitOfMeasure, StringComparison.OrdinalIgnoreCase))
                {
                    UnitOfMeasureData uomData = new UnitOfMeasureData(
                        ApplicationSettings.Database.LocalConnection,
                        ApplicationSettings.Database.DATAAREAID,
                        ApplicationSettings.Terminal.StorePrimaryId,
                        TaxService.Tax.InternalApplication);
                    UnitQtyConversion uomConversion = uomData.GetUOMFactor(this.LineItem.SalesOrderUnitOfMeasure, this.Unit, saleLineItem);
                    calculationBase *= uomConversion.GetFactorForQty(this.LineItem.Quantity);
                }
            }
            else
            {
                // Convert base to Tax currency, if different
                if (!taxInStoreCurrency)
                {
                    calculationBase = TaxService.Tax.InternalApplication.Services.Currency.CurrencyToCurrency(
                        ApplicationSettings.Terminal.StoreCurrency,
                        this.Currency,
                        calculationBase);
                }

                // Base for limits/intervals is same for Calculations
                limitBase = calculationBase;
            }
        }

        /// <summary>
        /// Return the basic rate for this tax
        /// </summary>
        /// <returns></returns>
        public decimal PercentPerTax()
        {
            switch (this.TaxBase)
            {
                case TaxBase.PercentPerTax:
                    if (!String.IsNullOrEmpty(this.TaxOnTax) && this.TaxOnTaxInstance != null)
                    {
                        return (this.Value * this.TaxOnTaxInstance.Value) / 100;
                    }
                    else
                    {
                        return decimal.Zero;
                    }

                case TaxBase.PercentPerGross:
                    if (!String.IsNullOrEmpty(this.TaxOnTax) && this.TaxOnTaxInstance != null)
                    {
                        return this.Value + ((this.Value * this.TaxOnTaxInstance.Value) / 100);
                    }
                    else
                    {
                        return this.Value;
                    }

                case TaxBase.PercentGrossOnNet:
                    return PercentGrossOnNet(this.Value);

                case TaxBase.AmountByUnit:
                case TaxBase.PercentPerNet:
                default:
                    return this.Value;
            }
        }

        /// <summary>
        /// Return the basic rate for this tax
        /// </summary>
        /// <param name="limitBase"></param>
        /// <returns></returns>
        public decimal PercentPerTax(decimal limitBase)
        {
            // Find the interval for this limitBase
            TaxInterval interval = this.TaxIntervals.Find(limitBase);
            decimal intervalRate = interval.Value;

            switch (this.TaxBase)
            {
                case TaxBase.PercentPerTax:
                    if (!String.IsNullOrEmpty(this.TaxOnTax) && this.TaxOnTaxInstance != null)
                    {
                        return (intervalRate * this.TaxOnTaxInstance.Value) / 100;
                    }
                    else
                    {
                        return decimal.Zero;
                    }

                case TaxBase.PercentPerGross:
                    if (!String.IsNullOrEmpty(this.TaxOnTax) && this.TaxOnTaxInstance != null)
                    {
                        return intervalRate + ((intervalRate * this.TaxOnTaxInstance.Value) / 100);
                    }
                    else
                    {
                        return intervalRate;
                    }

                case TaxBase.PercentGrossOnNet:
                    return PercentGrossOnNet(intervalRate);

                case TaxBase.AmountByUnit:
                case TaxBase.PercentPerNet:
                default:
                    return intervalRate;
            }
        }

        /// <summary>
        /// Gets the duty taxes.
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="codes">The codes.</param>
        /// <returns>Sum of duty taxes applicable on the line.</returns>
        private static decimal GetApplicableDutyTaxes(ITaxableItem lineItem, ReadOnlyCollection<TaxCode> codes)
        {
            decimal dutyTaxSum = 0;

            if (lineItem != null &&
                codes != null &&
                codes.Any())
            {
                var dutyTaxes = codes.Where(c => c.TaxIncludeInTax == true);
                dutyTaxSum = dutyTaxes.Sum(dt => dt.Value);
            }

            return dutyTaxSum;
        }

        /// <summary>
        /// Calculate the Percent Gross On Net amount for a given percent
        /// </summary>
        /// <param name="percent"></param>
        /// <returns></returns>
        private static decimal PercentGrossOnNet(decimal percent)
        {
            // Lifted from AX \ Classes \ Tax.calcPctGrosOnNet(...)
            if (percent == 100m)
            {
                return 100m;
            }
            else
            {
                return (percent / (100m - percent)) * 100m;
            }
        }

        /// <summary>
        /// Calculate the previous tax amount that the current tax is based on.
        /// </summary>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        private decimal CalculateTaxOnTax()
        {
            decimal taxAmount = decimal.Zero;
            if (!String.IsNullOrEmpty(this.TaxOnTax) && this.TaxOnTaxInstance != null)
            {
                // For each TaxLine that matches, sum the Amount
                taxAmount = this.LineItem.TaxLines.Where(taxLine => taxLine.TaxCode == this.TaxOnTax).Sum(taxLine => taxLine.Amount);
            }

            return taxAmount;
        }

        /// <summary>
        /// Sum up all amounts from the Tax-On-Tax for the whole transaction
        /// </summary>
        /// <param name="retailTransaction"></param>
        /// <returns></returns>
        private decimal CalculateTaxOnTax(IRetailTransaction retailTransaction)
        {
            decimal taxAmount = decimal.Zero;
            if (!String.IsNullOrEmpty(this.TaxOnTax))
            {
                //For each SaleItem, for each TaxLine that matches, sum the TaxLine.Amounts
                taxAmount = ((RetailTransaction)retailTransaction).SaleItems.Sum(
                    saleItem => saleItem.TaxLines.Where(taxLine => taxLine.TaxCode == this.TaxOnTax).Sum(taxLine => taxLine.Amount));
            }

            return taxAmount;
        }

        /// <summary>
        /// Return a sum of all the currently applied tax amounts
        /// </summary>
        /// <param name="lineItem"></param>
        /// <returns></returns>
        private static decimal SumAllTaxAmounts(ITaxableItem lineItem)
        {
            decimal allTaxAmounts = lineItem.TaxLines.Sum(t => t.Exempt ? decimal.Zero : t.Amount);
            return allTaxAmounts;
        }


        /// <summary>
        /// Calculate TaxInclusive amounts for a single tax code
        /// </summary>
        /// <param name="basePrice">The base price.</param>
        /// <param name="lineTaxResult">The line tax result.</param>
        public decimal CalculateTaxIncluded(ReadOnlyCollection<TaxCode> codes)
        {
            decimal codeAmount = this.Calculate(codes, true);

            if (!this.TaxGroupRounding)
            {
                codeAmount = TaxService.Tax.InternalApplication.Services.Rounding.Round(codeAmount);
            }

            return codeAmount;
        }

        /// <summary>
        /// Calculate TaxExclusive amounts for a single tax code
        /// </summary>
        /// <param name="basePrice">The base price.</param>
        /// <param name="lineTaxResult">The line tax result.</param>
        public decimal CalculateTaxExcluded(ReadOnlyCollection<TaxCode> codes)
        {
            decimal codeAmount = decimal.Zero;

            if (!this.Exempt)
            {
                // Calculate the tax amount
                codeAmount = this.Calculate(codes, true);

                // Apply collection limits                
                return ApplyCollectLimit(codeAmount);
            }

            return codeAmount;
        }

        /// <summary>
        /// Apply Tax Collect limits for this line
        /// </summary>
        /// <param name="taxAmount"></param>
        /// <returns></returns>
        private decimal ApplyCollectLimit(decimal taxAmount)
        {
            decimal limitedAmount = taxAmount;
            bool limitReached = false;
            bool taxInStoreCurrency = (String.IsNullOrEmpty(this.Currency)
                || this.Currency.Equals(ApplicationSettings.Terminal.StoreCurrency, StringComparison.OrdinalIgnoreCase));

            if (!taxInStoreCurrency)
            {
                // convert to tax currency to evaluate limits
                limitedAmount = TaxService.Tax.InternalApplication.Services.Currency.CurrencyToCurrency(
                    this.Currency,
                    ApplicationSettings.Terminal.StoreCurrency,
                    limitedAmount);
            }

            //
            // 2. Evaluate the limit
            //
            if (this.CollectLimitMin != decimal.Zero && limitedAmount < this.CollectLimitMin)
            {
                limitedAmount = decimal.Zero;
            }
            else if (this.CollectLimitMax != decimal.Zero && limitedAmount > this.CollectLimitMax)
            {
                limitedAmount = this.CollectLimitMax;
                limitReached = true;
            }

            if (!taxInStoreCurrency && limitReached)
            {
                // convert back to store currency if necessary
                limitedAmount = TaxService.Tax.InternalApplication.Services.Currency.CurrencyToCurrency(
                    ApplicationSettings.Terminal.StoreCurrency,
                    this.Currency,
                    limitedAmount);
            }

            return limitedAmount;
        }
    }


    /// <summary>
    /// Tax Interval - container class for TaxData intervals
    /// </summary>
    internal struct TaxInterval
    {
        public readonly decimal TaxLimitMin;
        public readonly decimal TaxLimitMax;
        public readonly decimal Value;

        /// <summary>
        /// Sets the tax interval (max & min)
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <param name="value"></param>
        public TaxInterval(decimal min, decimal max, decimal value)
        {
            this.TaxLimitMax = max;
            this.TaxLimitMin = min;
            this.Value = value;
        }
    }

    internal static class TaxExtensions
    {
        /// <summary>
        /// Determine whether the given amount is wholly within the interval
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        /// <example>
        /// Interval = $25 - $100
        /// Amount = $10  => FALSE
        /// Amount = $50  => TRUE
        /// Amount = $150 => FALSE
        /// </example>
        public static bool WholeAmountInInterval(this TaxInterval interval, decimal amount)
        {
            // return if the amount is in within the interval or equal to either end.
            return (interval.TaxLimitMin == decimal.Zero || interval.TaxLimitMin <= amount)
                && (interval.TaxLimitMax == decimal.Zero || interval.TaxLimitMax >= amount);
        }

        /// <summary>
        /// Determine whether any portion of the given amount is within the given interval
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        /// <example>
        /// Interval = $25 - $100
        /// Amount = $10  => FALSE
        /// Amount = $50  => TRUE
        /// Amount = $150 => TRUE
        /// </example>
        public static bool AmountInInterval(this TaxInterval interval, decimal amount)
        {
            return (interval.TaxLimitMin == decimal.Zero) || (interval.TaxLimitMin < amount);
        }

        /// <summary>
        /// Determine whether an Interval exists that includes the given Limit Base
        /// </summary>
        /// <param name="intervals"></param>
        /// <param name="limitBase"></param>
        /// <returns></returns>
        public static bool Exists(this IEnumerable<TaxInterval> intervals, decimal limitBase)
        {
            return intervals.Any(t => t.WholeAmountInInterval(limitBase));
        }

        /// <summary>
        /// Retrieve the Interval which includes the Limit Base
        /// </summary>
        /// <param name="intervals"></param>
        /// <param name="limitBase"></param>
        /// <returns></returns>
        public static TaxInterval Find(this IEnumerable<TaxInterval> intervals, decimal limitBase)
        {
            return intervals.FirstOrDefault(t => t.WholeAmountInInterval(limitBase));
        }
    }
}
