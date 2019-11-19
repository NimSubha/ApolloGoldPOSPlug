/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LSRetailPosis.DataAccess;
using LSRetailPosis.DataAccess.DataUtil;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessObjects;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.Collections.Generic;
using LSRetailPosis.Transaction.Line.TaxItems;

namespace Microsoft.Dynamics.Retail.Pos.Tax
{
    /// <summary>
    /// From AX table TAXTABLE.TAXTYPE_IN
    /// </summary>
    public enum TaxTypes
    {
        None = 0,
        VAT = 1,
        SalesTax = 2,
        Excise = 3,
        ServiceTax = 4,
        Customs = 5
    }

    /// <summary>
    /// Provides logic specific to India tax calculations
    /// </summary>
    public sealed class TaxCodeIndia : TaxCode
    {
        /// <summary>
        /// Percentage off item tax basis
        /// </summary>
        public decimal AbatementPercent { get; set; }   

        /// <summary>
        /// Type of tax (India specific)
        /// </summary>
        public TaxTypes TaxType { get; set; }

        /// <summary>
        /// Formula from India formula designer
        /// </summary>
        private Formula formula;

        private string taxComponent;
        private IList<string> taxCodesInFormula = new List<String>();

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
        /// <param name="collectMin">Collection limits, the minimum tax that can be collected</param>
        /// <param name="collectMax">Collection limits, the maximum tax that can be collected</param>
        /// <param name="abatementPercent">Percentage off item tax basis</param>
        /// <param name="taxType">Type of tax (India specific)</param>
        /// <param name="provider">The tax code provider that created this instance.</param>
        public TaxCodeIndia(
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
            decimal collectMin,
            decimal collectMax,
            decimal abatementPercent,
            TaxTypes taxType,
            TaxCodeProvider provider)
            : base(value, limitMin, limitMax, provider)
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
            this.CollectLimitMax = collectMax;
            this.CollectLimitMin = collectMin;
            this.AbatementPercent = abatementPercent;
            this.TaxType = taxType;
        }


        /// <summary>
        /// Retrieve from DB only first time.
        /// </summary>
        public Formula Formula
        {
            get
            {
                if (formula == null)
                {
                    formula = FormulaData.GetFormula(this.TaxGroup, this.Code);
                }
                return formula;
            }
        }

        /// <summary>
        /// Gets the tax component.
        /// </summary>
        public string TaxComponent
        {
            get 
            {
                if (taxComponent == null)
                {
                    // Lazy-load the tax component
                    taxComponent = GetComponent();
                }
                return taxComponent;
            }
        }

        private string GetComponent()
        {
            SqlSelect sqlSelect = new SqlSelect("TAXTABLE T1 left join TAXCOMPONENTTABLE_IN T2 on T1.TAXCOMPONENTTABLE_IN=T2.RECID");
            sqlSelect.Select("ISNULL(T2.COMPONENT, '') as TAX_COMPONENT");
            sqlSelect.Where("T1.TAXCODE", Code, true);

            DBUtil dbUtil = new DBUtil(ApplicationSettings.Database.LocalConnection);
            DataTable dt = dbUtil.GetTable(sqlSelect);

            return dt.Rows.Count > 0 ? dt.Rows[0]["TAX_COMPONENT"].ToString() : String.Empty;
        }

        /// <summary>
        /// Whether the tax code is a tax-on-tax or not.
        /// </summary>
        public bool IsTaxOnTax { get; set; }

        /// <summary>
        /// Tax codes in the formula.
        /// </summary>
        /// <remarks>
        /// For example, if the formula of a tax item is +[ST-DL]+[ST-KA], the property will be valued as ["ST-DL", "ST-KA"].
        /// </remarks>
        public IList<String> TaxCodesInFormula
        {
            get { return taxCodesInFormula; }
        }

        private decimal GetMRP()
        {
            return Microsoft.Dynamics.Retail.Pos.PriceService.IndiaMRPHelper.GetMRP(this.LineItem);
        }

        /// <summary>
        /// Calculates the tax amounts india.
        /// </summary>
        /// <param name="codes">The codes.</param>
        /// <param name="codeIndia">The code india.</param>
        /// <param name="amtPerUnitVal">The amt per unit val.</param>
        /// <param name="taxVal">The tax val.</param>
        /// <param name="formula">The formula val.</param>
        private static void CalculateTaxAmountsIndia(ReadOnlyCollection<TaxCode> codes, TaxCodeIndia codeIndia, ref decimal amtPerUnitVal, ref decimal taxVal, Formula formula)
        {
            decimal amtPerUnit = decimal.Zero;
            decimal taxCodeValue = decimal.Zero;
            decimal taxValueLoc;

            taxVal = decimal.Zero;
            amtPerUnitVal = decimal.Zero;

            if (codeIndia.Formula.TaxableBasis != TaxableBases.ExclAmount && codeIndia.Formula.TaxableBasis != formula.TaxableBasis)
            {
                return;
            }
            
            string[] tokens = codeIndia.Formula.ParseExpression();            

            if (tokens.Length > 1)
            {
                Formula basisFormula = FormulaData.GetFormula(codeIndia.TaxGroup, tokens[1]);

                if (basisFormula != null && basisFormula.TaxableBasis == formula.TaxableBasis)
                {
                    // Iterate through the formula
                    for (int index = 1; index < tokens.Length; index += 2)
                    {
                        TaxCode basisCode = (from c in codes
                                             where c.Code == tokens[index]
                                             select c).FirstOrDefault();

                        if ((basisCode != null) && !basisCode.Exempt)
                        {
                            codeIndia.IsTaxOnTax = true;
                            codeIndia.TaxCodesInFormula.Add(basisCode.Code);

                            // Either add or subtract the values based on the operator
                            switch (tokens[index - 1])
                            {
                                case "-":
                                    if (basisCode.TaxBase == TaxBase.AmountByUnit)
                                    {
                                        amtPerUnit -= basisCode.AmtPerUnitValue;
                                    }
                                    else
                                    {
                                        taxCodeValue -= basisCode.TaxValue;
                                    }
                                    break;
                                case "+":
                                    if (basisCode.TaxBase == TaxBase.AmountByUnit)
                                    {
                                        amtPerUnit += basisCode.AmtPerUnitValue;
                                    }
                                    else
                                    {
                                        taxCodeValue += basisCode.TaxValue;
                                    }
                                    break;
                                default:
                                    NetTracer.Error("CalculateTaxAmountsIndia(): Multiplication and division not currently supported in AX. tokens[{0}]: {1}", index - 1, tokens[index - 1]);
                                    Debug.Fail("Multiplication and division not currently supported in AX");

                                    break;
                            }
                        }
                    }
                }
            }

            

            taxValueLoc = codeIndia.Value;

            if (codeIndia.TaxBase == TaxBase.AmountByUnit)
            {
                taxVal = decimal.Zero;
                amtPerUnitVal = taxValueLoc;
            }
            else
            {
                if (codeIndia.Formula.TaxableBasis != TaxableBases.ExclAmount)
                {
                    taxVal = ((1 + taxCodeValue) * taxValueLoc) / 100;
                }
                else
                {
                    taxVal = (taxCodeValue * taxValueLoc) / 100;
                }

                taxVal *= (100 - codeIndia.AbatementPercent) / 100;
                amtPerUnitVal = amtPerUnit * taxValueLoc / 100;
            }
        }

        /// <summary>
        /// Calculates the tax amounts for non India tax codes (simple tax codes only supported).
        /// </summary>
        /// <param name="code">The code.</param>
        /// <param name="amtPerUnitVal">The amt per unit val.</param>
        /// <param name="taxVal">The tax val.</param>
        private static void CalculateTaxAmounts(TaxCode code, ref decimal amtPerUnitVal, ref decimal taxVal)
        {
            if (code.TaxBase == TaxBase.AmountByUnit)
            {
                amtPerUnitVal = code.Value;
                taxVal = decimal.Zero;
            }
            else
            {
                amtPerUnitVal = decimal.Zero;
                taxVal = code.Value;
            }
        }

        /// <summary>
        /// Back calculates the base price for items with included taxes.  Based off of the class in AX
        /// Tax.calcBaseAmtExclTax_IN
        /// </summary>
        /// <param name="lineItem">The line item.</param>
        /// <param name="codes">The tax codes.</param>
        /// <param name="formula">The formula val.</param>
        /// <returns>base price</returns>
        private static decimal GetBasePriceForTaxIncluded(decimal baseLine, ReadOnlyCollection<TaxCode> codes, Formula formula)
        {
            decimal taxVal;         // A summarized tax rate for this code computed from the formula
            decimal taxValLine;     // The summed tax rates for all codes for this line
            decimal amtPerUnitVal;  // A summarized amount per unit contribution for this code computed from the formula
            decimal amtPerUnitLine; // The summed amount per unit contributions for all codes for this line.

            taxValLine = decimal.Zero;
            amtPerUnitLine = decimal.Zero;

            foreach (var code in codes)
            {
                taxVal = decimal.Zero;
                amtPerUnitVal = decimal.Zero;

                if (code.TaxIncludedInPrice)
                {
                    // Handle codes differently based on whether they are India or not.
                    TaxCodeIndia codeIndia = code as TaxCodeIndia;
                    if (codeIndia != null)
                    {
                        CalculateTaxAmountsIndia(codes, codeIndia, ref amtPerUnitVal, ref taxVal, formula);
                    }
                    else
                    {
                        CalculateTaxAmounts(code, ref amtPerUnitVal, ref taxVal);
                    }
                    code.TaxValue = taxVal;
                    code.AmtPerUnitValue = amtPerUnitVal;
                }

                taxValLine += taxVal;
                amtPerUnitLine += amtPerUnitVal;
            }

            // Back compute and set the price from price with tax (baseLine).
            return (Math.Abs(baseLine) - amtPerUnitLine) / (1 + taxValLine);
        }

        /// <summary>
        /// Calculates tax for this code for the line item.
        /// Updates the line item by adding a new Tax Item
        /// </summary>
        /// <param name="codes"></param>
        /// <param name="invoiceTotals">The current invoice totals for this tax code.</param>
        /// <returns>the calculated amount of tax.</returns>
        public override decimal CalculateTaxAmount(ReadOnlyCollection<TaxCode> codes, TaxCodeInvoiceTotals invoiceTotals)
        {
            if (codes == null)
            {
                return decimal.Zero;
            }
            decimal taxAmount = decimal.Zero;

            this.LineItem.TaxGroupId = this.TaxGroup;
            taxAmount = this.TaxIncludedInPrice ? CalculateTaxIncluded(codes) : CalculateTaxExcluded(codes);

            if (!this.TaxGroupRounding)
            {
                // Adjust tax code amount if tax code is based on invoice total.
                taxAmount = invoiceTotals.AddTaxCodeAmountToInvoiceAndAdjust(this, this.TaxAmountNonRounded, taxAmount);
            }

            // record amounts on line item
            //ITaxItem taxItem = TaxService.Tax.InternalApplication.BusinessLogic.Utility.CreateTaxItem();
            TaxItemIndia taxItem = new TaxItemIndia();

            taxItem.Amount = taxAmount;
            taxItem.Percentage = this.Value;
            taxItem.TaxCode = this.Code;
            taxItem.TaxGroup = this.TaxGroup;
            taxItem.Exempt = this.Exempt;
            taxItem.IncludedInPrice = this.TaxIncludedInPrice;

            taxItem.TaxComponent = this.TaxComponent;
            taxItem.IsTaxOnTax = this.IsTaxOnTax;
            foreach (string codeInFormula in this.TaxCodesInFormula)
            {
                taxItem.TaxCodesInFormula.Add(codeInFormula);
            }

            switch (Formula.TaxableBasis)
            {
                case TaxableBases.MRP:
                    taxItem.IncludedInPrice = false;
                    break;
                case TaxableBases.ExclAmount:
                    string[] tokens = Formula.ParseExpression();
                    // Iterate through the formula
                    if (tokens.Length > 1)
                    {
                        Formula basisFormula = FormulaData.GetFormula(this.TaxGroup, tokens[1]);
                        if (basisFormula != null && basisFormula.TaxableBasis == TaxableBases.MRP)
                        {
                            taxItem.IncludedInPrice = false;
                        }
                    }
                    break;
                default:
                    break;
            }          
            
            taxItem.TaxBasis = this.TaxBasis;
            this.LineItem.Add(taxItem);

            return taxAmount;
        }

        /// <summary>
        /// Calculate the tax bases calculationBase and limitBase (which is zero for India).
        /// </summary>
        /// <param name="basePrice">The base price.</param>
        /// <param name="taxInStoreCurrency">if set to <c>true</c> [tax in store currency].</param>
        /// <param name="calculateBasePrice">if set to <c>true</c> [Calculate the base price].</param>
        /// <param name="calculationBase">The calculation base.</param>
        /// <param name="limitBase">The limit base.</param>
        protected override void GetBases(ReadOnlyCollection<TaxCode> codes, bool taxInStoreCurrency, bool calculateBasePrice, out decimal calculationBase, out decimal limitBase)
        {
            limitBase = decimal.Zero;

            // For amount by unit calculation base is just the quantity.
            if (this.TaxBase == TaxBase.AmountByUnit)
            {
                calculationBase = LineItem.Quantity;

                // If the tax is calculated in a different UOM, then convert if possible 
                // this is only applicable for lineItem taxes.
                BaseSaleItem saleLineItem = this.LineItem as BaseSaleItem;

                if (saleLineItem  != null && 
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
                return;
            }

            // Determine the starting calculation base (includes the line price or not)
            switch (Formula.TaxableBasis)
            {
                case TaxableBases.LineAmount:
                    calculationBase = this.LineItem.NetAmountWithAllInclusiveTaxPerUnit;
                    break;
                case TaxableBases.MRP:
                    calculationBase = GetMRP();
                    break;
                default:
                    calculationBase = decimal.Zero;
                    break;
            }

            if (this.TaxIncludedInPrice)
            {
                calculationBase = GetBasePriceForTaxIncluded(calculationBase, codes, Formula);
            }

            calculationBase *= Math.Abs(this.LineItem.Quantity);

            // Calculation expression is of the form: +[BCD]+[CVD]+[E-CESS_CVD]+[PE-C_CVD]+[SHE-C_CVD]
            // where the brackets are replaced with the delimiter char(164)
            // and BCD, CVD ... are tax codes.
            // The operator may be + - / *.
            string[] tokens = Formula.ParseExpression();

            for (int index = 1; index < tokens.Length; index += 2)
            {
                ITaxItem taxItem = (from line in this.LineItem.TaxLines
                                   where line.TaxCode == tokens[index]
                                   select line).FirstOrDefault();

                if (taxItem != null)
                {
                    this.IsTaxOnTax = true;
                    this.TaxCodesInFormula.Add(taxItem.TaxCode);
                }

                decimal amount = taxItem == null ? decimal.Zero : taxItem.Amount * Math.Sign(this.LineItem.Quantity);

                switch (tokens[index - 1])
                {
                    case "+":
                        calculationBase += amount;
                        break;
                    case "-":
                        calculationBase -= amount;
                        break;
                    case "*":
                        calculationBase *= amount;
                        break;
                    case "/":
                        calculationBase = (amount == decimal.Zero ? calculationBase : calculationBase /= amount);
                        break;
                    default:
                        NetTracer.Error("GetBases(): Invalid operator in formula. tokens[{0}]: {1}", index - 1, tokens[index - 1]);
                        System.Diagnostics.Debug.Fail("Invalid operator in formula");
                        break;
                }
            }

            // Knock any abatement off of the taxable basis
            calculationBase *= (100 - AbatementPercent) / 100;
        }

        public override bool TaxIncludedInPrice
        {
            get
            {
                return Formula.PriceIncludesTax;
            }
        }
    }
}
