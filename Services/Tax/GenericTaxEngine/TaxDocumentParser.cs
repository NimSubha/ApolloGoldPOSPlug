/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.Line.TaxItems;
using LSRetailPosis.Transaction.Line.Tax;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics365.Tax.Core;
using Microsoft.Dynamics365.Tax.DataModel.Enum;


using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;

namespace Microsoft.Dynamics.Retail.Pos.Tax.GenericTaxEngine
{
    /// <summary>
    /// Parses the structure of tax document to fill the RetailTransaction with TaxItem objects.
    /// </summary>
    public static class TaxDocumentParser
    {
        /// <summary>
        /// Populates a RetailTransaction from Enterprise POS with TaxItem objects upon data from tax document object.
        /// </summary>
        /// <param name="taxDocument">Tax document object.</param>
        /// <param name="retailTransaction">The POS transaction.</param>
        /// 
        public static void PopulateRetailTransactionWithTaxItems(ITaxDocument taxDocument, TaxableDocumentMapping documentMapping)
        {
            if (documentMapping == null)
            {
                throw new ArgumentNullException("documentMapping");
            }

            var retailTransaction = documentMapping.RetailTransaction;
            
            // Flatten taxableItems of different levels into a list and loop.
            List<ITaxableItem> taxableItems = new List<ITaxableItem>();

            // Order level charges
            taxableItems.AddRange(retailTransaction.MiscellaneousCharges);

            // Line Level 
            foreach (SaleLineItem saleLine in TaxEngineServiceProxy.GetSalesLinesForTaxCalculation(retailTransaction))
            {
                // lineitem itself
                taxableItems.Add(saleLine);

                // associated charges
                taxableItems.AddRange(saleLine.MiscellaneousCharges);
            }

            foreach (ITaxableItem taxableItem in taxableItems)
            {
                PopulateTaxableItemWithTaxItems(taxDocument, documentMapping, taxableItem);
            }
        }

        private static void PopulateTaxableItemWithTaxItems(ITaxDocument taxDocument, TaxableDocumentMapping documentMapping, ITaxableItem taxableItem)
        {
            decimal taxAmount = decimal.Zero;
            bool taxIncludedInPrice = false;

            //==========================Soutik===========================================
            string scustid = documentMapping.RetailTransaction.Customer.CustomerId;
            string sCustGST = string.Empty;
            if (!string.IsNullOrEmpty(scustid))
            {
                sCustGST = GetGSTNoByCustId(scustid);
            }
            else
            {
                sCustGST = "";
            }
            //=============================================================================
            TaxableItemId taxableItemId = documentMapping.lookupTaxableItemId(taxableItem);


            if (taxableItemId != null)
            {
                ITaxDocumentLine documentLine = taxDocument.findLineByOrig(taxableItemId.TableId, taxableItemId.RecordId);

                if (documentLine != null)
                {
                    ITaxDocumentComponentLineEnumerator componentEnumerator = documentLine.componentLines();

                    while (componentEnumerator.moveNext())
                    {
                        ITaxDocumentComponentLine curComponent = componentEnumerator.current();

                        PopulateTaxableItemWithTaxMeasures(documentLine, curComponent, curComponent.measures(), taxableItem);

                        ITaxDocumentMeasureValue debitMeasureValue = curComponent.sumByTaxAccountingProvider(TaxAccountingProvider.Party, TaxAcctPostingProfDistributionSide.Debit);
                        ITaxDocumentMeasureValue creditMeasureValue = curComponent.sumByTaxAccountingProvider(TaxAccountingProvider.Party, TaxAcctPostingProfDistributionSide.Credit);

                        if (debitMeasureValue != null && creditMeasureValue != null)
                        {
                            var amountValue = debitMeasureValue.amountTransactionCurrency() - creditMeasureValue.amountTransactionCurrency();
                            ITaxItem taxItem = BuildTaxItem(documentLine, curComponent, amountValue,sCustGST);

                            taxableItem.Add(taxItem);

                            taxAmount += taxItem.Amount;
                            taxIncludedInPrice = taxIncludedInPrice || taxItem.IncludedInPrice;
                        }
                    }
                }
            }

            taxableItem.TaxRatePct = CalculateTaxPercentage(taxableItem, taxAmount, taxIncludedInPrice);
        }

        private static decimal CalculateTaxPercentage(ITaxableItem taxableItem, decimal taxAmount, bool taxIncludedInPrice)
        {
            // Calculate 'effective' tax rate, considering summary tax amount and price inclusive property
            decimal lineTotal = taxableItem.Quantity > decimal.Zero ? taxableItem.NetAmountPerUnit * taxableItem.Quantity : decimal.Zero;
            decimal taxBasis = taxIncludedInPrice ? lineTotal - taxAmount : lineTotal;
            decimal taxPercentage = taxableItem.TaxRatePct = taxBasis > decimal.Zero ? (taxAmount * 100) / taxBasis : decimal.Zero;

            return taxPercentage;
        }

        private static void PopulateTaxableItemWithTaxMeasures(ITaxDocumentLine documentLine, ITaxDocumentComponentLine componentLine, ITaxDocumentMeasureEnumerator measureEnumerator, ITaxableItem taxableItem)
        {
            while (measureEnumerator.moveNext())
            {
                ITaxDocumentMeasure measure = measureEnumerator.current();

                if (measure != null)
                {
                    try
                    {
                        TaxMeasure taxMeasure = new TaxMeasure();

                        taxMeasure.Value = measure.value().value();
                        taxMeasure.AmountTransactionCurrency = measure.value().amountTransactionCurrency();
                        taxMeasure.AmountAccountingCurrency = measure.value().amountAccountingCurrency();
                        taxMeasure.AmountReportingCurrency = measure.value().amountTransactionCurrency();
                        taxMeasure.Name = measure.metaData().name();

                        taxMeasure.Path = BuildTaxMeasurePath(documentLine, componentLine, measure);

                        taxableItem.AddTaxMeasure(taxMeasure);
                    }
                    catch (Exception ex)
                    {
                        // error message: "Failed to find mapped tax document element."
                        throw new TaxDocumentParserException(120003, ex);
                    }
                }
            }
        }

        private static string BuildTaxMeasurePath(ITaxDocumentLine documentLine, ITaxDocumentComponentLine componentLine, ITaxDocumentMeasure measure)
        {
            return String.Concat(
                        TaxEngineRetailConstants.Header,
                        TaxEngineRetailConstants.PathDelimeter,
                        documentLine.metaData().isHeaderLine() ?
                            String.Empty :
                            String.Concat(
                                TaxEngineConstants.TaxableDocumentSysAttributeLines,
                                TaxEngineRetailConstants.PathDelimeter),
                        componentLine.metaData().taxType(),
                        TaxEngineRetailConstants.PathDelimeter,
                        componentLine.metaData().taxComponent(),
                        TaxEngineRetailConstants.PathDelimeter,
                        measure.metaData().name());
        }

        private static bool ConvertTaxDocumentFieldValueToBool(object fieldValue)
        {
            bool ret = false;
            string fieldValueString = fieldValue.ToString();
            string enumValueString = fieldValueString.Split(TaxEngineRetailConstants.EREnumSeparator).LastOrDefault();

            switch (enumValueString)
            {
                case TaxEngineRetailConstants.Yes:
                    ret = true;
                    break;
                case TaxEngineRetailConstants.No:
                    ret = false;
                    break;
                default:
                    // error message: "Failed to find mapped tax document element."
                    throw new TaxDocumentParserException(120003);
            }
            return ret;
        }

        private static ITaxItem BuildTaxItem(ITaxDocumentLine documentLine, ITaxDocumentComponentLine componentLine, decimal amountValue,string sCustGstin)
        {
            TaxItemGTE newTaxItem = new TaxItemGTE();

            try
            {
                newTaxItem.Amount = amountValue;
                ////==========================================SoutikCESS=====================================================
                //if (componentLine.metaData().taxComponent() == "CESS" && (!string.IsNullOrEmpty(sCustGstin)))
                //{
                //    newTaxItem.Amount = 0;
                //}
                //else 
                //{
                //    newTaxItem.Amount = amountValue;
                //}
                ////=======================================================================================================
                newTaxItem.IncludedInPrice = documentLine.priceInclTax();
                newTaxItem.TaxComponent = componentLine.metaData().taxComponent();
                newTaxItem.TaxBasis = componentLine.getMeasure(TaxEngineRetailConstants.TaxDocumentMeasureBaseAmount).value().amountTransactionCurrency();
                newTaxItem.Percentage = componentLine.getMeasure(TaxEngineRetailConstants.TaxDocumentMeasureRate).value().value() * 100;
                newTaxItem.Exempt = ConvertTaxDocumentFieldValueToBool(documentLine.getFieldValue(TaxEngineRetailConstants.Exempt));
            }
            catch (Exception ex)
            {
                // error message: "Failed to find mapped tax document element."
                throw new TaxDocumentParserException(120003, ex);
            }

            return newTaxItem;
        }

        private static string GetGSTNoByCustId(string sCustId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();

            string sQry = " select top 1 TAXREGISTRATIONNUMBERS_IN.REGISTRATIONNUMBER" +
                         " from" +
                         " TAXREGISTRATIONNUMBERS_IN inner join TAXINFORMATION_IN" +
                         " on TAXREGISTRATIONNUMBERS_IN.RECID = TAXINFORMATION_IN.GSTIN" +
                         " inner join LOGISTICSLOCATION " +
                         " on TAXINFORMATION_IN.REGISTRATIONLOCATION = LOGISTICSLOCATION.RECID" +
                         " and TAXINFORMATION_IN.ISPRIMARY = 1" +
                         " inner join DIRPARTYLOCATION" +
                         " on DIRPARTYLOCATION.LOCATION = LOGISTICSLOCATION.RECID" +
                         " and DIRPARTYLOCATION.ISPRIMARY = 1" +
                         " inner join CUSTTABLE on CustTable.party = DIRPARTYLOCATION.PARTY" +
                         " and CUSTTABLE.ACCOUNTNUM = '" + sCustId + "'";

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(sQry.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "";
        }
    }
}
