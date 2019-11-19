/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.Line.Tax;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics365.Tax.Core;

namespace Microsoft.Dynamics.Retail.Pos.Tax.GenericTaxEngine
{
    /// <summary>
    /// Provides the taxable document built upon source retail transaction.
    /// </summary>
    public class TaxableDocumentProvider
    {

        Customer defaultCustomer;
        RetailTransaction retailTransaction;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customer">Default customer.</param>
        /// <param name="transaction">Retail transaction.</param>
        /// <param name="storeAddress">Store address</param>
        public TaxableDocumentProvider(Customer customer, RetailTransaction transaction)
        {
            this.defaultCustomer = customer;
            this.retailTransaction = transaction;
        }

        /// <summary>
        /// Retrieves a new Taxable Document object with mapping to source RetailTransaction from Enterprise POS.
        /// </summary>
        /// <returns>A new TaxableDocumentMapping instance containing the TaxableDocumentObject and mapping to source transaction data.</returns>
        public TaxableDocumentMapping GetTaxableDocumentWithMapping()
        {
            TaxableDocumentObject taxableDocument = new TaxableDocumentObject();

            taxableDocument.TransactionLineTableId = TaxEngineRetailConstants.TransactionTableId;
            taxableDocument.TransactionLineRecordId = TaxEngineRetailConstants.TransactionRecordId;

            TaxableDocumentMapping documentMapping = new TaxableDocumentMapping(retailTransaction, taxableDocument);

            taxableDocument.HeaderLine = BuildTaxabelDocumentHeaderAndSublines(documentMapping, retailTransaction);

            return documentMapping;
        }

        /// <summary>
        /// Gets transaction customer. Gets either transaction customer or store default customer if transaction customer is not set.
        /// </summary>
        /// <param name="retailTransaction">Retail transaction.</param>
        /// <returns>Transaction customer.</returns>
        private Customer GetTransactionCustomer(RetailTransaction retailTransaction)
        {
            Customer customer;
            if (retailTransaction.Customer != null && !retailTransaction.Customer.IsEmptyCustomer())
            {
                customer = retailTransaction.Customer;
            }
            else
            {
                customer = defaultCustomer;
            }

            return customer;
        }

        /// <summary>
        /// Defines consumption state code for retail transaction.
        /// </summary>
        /// <param name="transaction">Retail transaction.</param>
        /// <returns>Consumption state code.</returns>
        private string GetConsumptionStateCode(RetailTransaction transaction)
        {
            Customer customer = this.GetTransactionCustomer(transaction);
            string consumptionStateCode = string.Empty;
            string storeState = ApplicationSettings.Terminal.IndiaStoreTaxState ?? string.Empty;

            if (customer != null && customer.CustomerId != defaultCustomer.CustomerId)
            {
                var customerAddress = customer.PrimaryAddress;
                if (customerAddress != null && customerAddress.State != null)
                {
                    consumptionStateCode = customerAddress.State;
                }
            }
            else
            {
                consumptionStateCode = storeState;
            }

            return consumptionStateCode;
        }

        /// <summary>
        /// Defines if sales line is interstate.
        /// </summary>
        /// <param name="salesLine">Sales line.</param>
        /// <returns>True if sales line is interstate; false otherwise.</returns>
        private string IsInterState(SaleLineItem salesLine)
        {
            bool interState = false;
            Customer customer = this.GetTransactionCustomer(salesLine.Transaction);
            string storeState = ApplicationSettings.Terminal.IndiaStoreTaxState;
            string storeCountry = ApplicationSettings.Terminal.IndiaStoreTaxCountry;

            if (!string.IsNullOrWhiteSpace(storeCountry) &&
                !string.IsNullOrWhiteSpace(storeState) &&
                customer != null &&
                customer.CustomerId != defaultCustomer.CustomerId)
            {
                var customerAddress = customer.PrimaryAddress;
                if (customerAddress != null)
                {
                    interState = (storeCountry != customerAddress.Country) || (storeCountry == customerAddress.Country &&
                                    !string.IsNullOrWhiteSpace(storeState) &&
                                    !string.IsNullOrWhiteSpace(customerAddress.State) &&
                                    storeState != customerAddress.State);
                }
            }

            return interState ? TaxEngineRetailConstants.Yes : TaxEngineRetailConstants.No;
        }

        /// <summary>
        /// Gets transaction date. Date of creation for customer orders or current date for regular sales.
        /// </summary>
        /// <param name="retailTransaction">Retail transaction.</param>
        /// <returns>Transaction date.</returns>
        private DateTime GetTransactionDate(RetailTransaction retailTransaction)
        {
            DateTime transactionDate = DateTime.Today;

            if (retailTransaction.TransactionType != PosTransaction.TypeOfTransaction.CustomerOrder)
            {
                transactionDate = retailTransaction.BeginDateTime.Date;
            }

            return transactionDate;
        }

        /// <summary>
        /// Gets delivery date. Current date if delivery date is not defined on sales line.
        /// </summary>
        /// <param name="lineItem">Sales line item.</param>
        /// <returns>Delivery date.</returns>
        private DateTime GetDeliveryDate(SaleLineItem lineItem)
        {
            DateTime deliveryDate = DateTime.Today;

            if (lineItem.DeliveryDate.HasValue && lineItem.DeliveryDate.Value != DateTime.MinValue)
            {
                deliveryDate = lineItem.DeliveryDate.Value;
            }

            return deliveryDate;
        }

        private string BoolToYesNo(bool value)
        {
            return value ? TaxEngineRetailConstants.Yes : TaxEngineRetailConstants.No;
        }

        /// <summary>
        /// Builds taxable document header and its sublines.
        /// </summary>
        /// <param name="documentMapping">Document mapping.</param>
        /// <param name="retailTransaction">Retail transaction.</param>
        /// <returns>Taxable document header.</returns>
        private TaxableDocumentLineObject BuildTaxabelDocumentHeaderAndSublines(TaxableDocumentMapping documentMapping, RetailTransaction retailTransaction)
        {
            TaxableDocumentLineObject header = new TaxableDocumentLineObject();

            List<TaxableDocumentLineObject> taxableSubLines = new List<TaxableDocumentLineObject>();

            taxableSubLines.AddRange(BuildTaxableDocumentLinesFromSalesLineItems(documentMapping, TaxEngineServiceProxy.GetSalesLinesForTaxCalculation(retailTransaction)));

            header.ModelFieldName = String.Empty;
            header.TransactionLineTableId = TaxEngineRetailConstants.TransactionTableId;
            header.TransactionLineRecordId = TaxEngineRetailConstants.TransactionRecordId;
            header.SubLines = taxableSubLines.ToArray();

            header.addField(TaxEngineRetailConstants.CompositionScheme, TaxEngineRetailConstants.No);

            var customer = GetTransactionCustomer(retailTransaction);
            header.addField(TaxEngineRetailConstants.ImportOrder, TaxEngineRetailConstants.No);
            header.addField(TaxEngineRetailConstants.ExportOrder, BoolToYesNo(customer.CustomerTaxInformation.Foreign));
            header.addField(TaxEngineRetailConstants.ForeignParty, BoolToYesNo(customer.CustomerTaxInformation.Foreign));

            header.addField(TaxEngineRetailConstants.LedgerCurrency, retailTransaction.StoreCurrencyCode);

            header.addField(TaxEngineRetailConstants.NatureOfAssesse, customer.CustomerTaxInformation.NatureOfAssessee.ToString());
            header.addField(TaxEngineRetailConstants.PreferrentialParty, BoolToYesNo(customer.CustomerTaxInformation.Preferential));

            header.addField(TaxEngineRetailConstants.RecId, TaxEngineRetailConstants.TransactionRecordId);
            header.addField(TaxEngineRetailConstants.Skipped, TaxEngineRetailConstants.No);
            header.addField(TaxEngineRetailConstants.TableId, TaxEngineRetailConstants.TransactionTableId);
            header.addField(TaxEngineRetailConstants.TaxAsPerOriginalInvoice, TaxEngineRetailConstants.No);
            header.addField(TaxEngineRetailConstants.TaxableDocumentType, TaxEngineRetailConstants.SalesInvoice);
            header.addField(TaxEngineRetailConstants.TotalDiscountPercentage, retailTransaction.TotalDiscount);

            return header;
        }

        /// <summary>
        /// Calculates assessable value for sales line.
        /// </summary>
        /// <param name="salesLine">Sales line.</param>
        /// <returns>Assessable value.</returns>
        private decimal CalculateSalesLineAssessableValue(SaleLineItem salesLine)
        {
            return salesLine.NetAmountPerUnit * salesLine.Quantity;
        }

        /// <summary>
        /// Builds taxable document line from sales line.
        /// </summary>
        /// <param name="documentMapping">Document mapping.</param>
        /// <param name="saleItems">Collection of sales lines.</param>
        /// <returns>Collection of taxable document lines.</returns>
        private List<TaxableDocumentLineObject> BuildTaxableDocumentLinesFromSalesLineItems(TaxableDocumentMapping documentMapping, IEnumerable<SaleLineItem> saleItems)
        {
            List<TaxableDocumentLineObject> taxableLines = new List<TaxableDocumentLineObject>();

            long lineNum = TaxEngineRetailConstants.TransactionRecordIdFirstElement;

            foreach (SaleLineItem salesLine in saleItems)
            {
                TaxableDocumentLineObject taxableLine = new TaxableDocumentLineObject();

                taxableLine.ModelFieldName = TaxEngineConstants.TaxableDocumentSysAttributeLines;
                taxableLine.TransactionLineTableId = TaxEngineRetailConstants.TransactionLineTableId;
                taxableLine.TransactionLineRecordId = lineNum;
                taxableLine.SubLines = BuildTaxableDocumentSubLinesFromMiscellanousCharges(documentMapping, salesLine.MiscellaneousCharges, TaxEngineRetailConstants.TransactionLineChargeTableId, salesLine).ToArray();

                taxableLine.addField(TaxEngineRetailConstants.LineType, TaxEngineRetailConstants.LineTypeLine);
                taxableLine.addField(TaxEngineRetailConstants.AssessableValue, CalculateSalesLineAssessableValue(salesLine));
                taxableLine.addField(TaxEngineRetailConstants.ConsumptionState, GetConsumptionStateCode(salesLine.Transaction));

                taxableLine.addField(TaxEngineRetailConstants.CustomTariffCode, String.Empty);
                taxableLine.addField(TaxEngineRetailConstants.DeliveryDate, GetDeliveryDate(salesLine));
                taxableLine.addField(TaxEngineRetailConstants.DiscountAmount, salesLine.LineDiscount);
                taxableLine.addField(TaxEngineRetailConstants.Exempt, BoolToYesNo(salesLine.Exempt));
                taxableLine.addField(TaxEngineRetailConstants.GSTRegistrationNumber, ApplicationSettings.Terminal.IndiaGSTRegistrationNumber);
                taxableLine.addField(TaxEngineRetailConstants.HSNCode, salesLine.HSNCode);
                taxableLine.addField(TaxEngineRetailConstants.IECNumber, String.Empty);
                taxableLine.addField(TaxEngineRetailConstants.InterState, IsInterState(salesLine));

                taxableLine.addField(TaxEngineRetailConstants.ITCCategory, TaxEngineRetailConstants.Input);

                decimal mrp = Microsoft.Dynamics.Retail.Pos.PriceService.IndiaMRPHelper.GetMRP(salesLine);
                taxableLine.addField(TaxEngineRetailConstants.MaximumRetailPrice, mrp);

                taxableLine.addField(TaxEngineRetailConstants.NetAmount, salesLine.NetAmountPerUnit);

                taxableLine.addField(TaxEngineRetailConstants.PartyGSTRegistrationNumber, String.Empty);

                taxableLine.addField(TaxEngineRetailConstants.ProductType, salesLine.ItemType == BaseSaleItem.ItemTypes.Service
                    ? TaxEngineRetailConstants.Service : TaxEngineRetailConstants.Item);

                taxableLine.addField(TaxEngineRetailConstants.Purpose, TaxEngineRetailConstants.Transaction);

                taxableLine.addField(TaxEngineRetailConstants.Quantity, salesLine.Quantity);
                taxableLine.addField(TaxEngineRetailConstants.RecId, lineNum);
                taxableLine.addField(TaxEngineRetailConstants.Return, TaxEngineRetailConstants.No);
                taxableLine.addField(TaxEngineRetailConstants.SAC, salesLine.ServiceAccountingCode);

                taxableLine.addField(TaxEngineRetailConstants.ServiceCategory, TaxEngineRetailConstants.Inward);

                taxableLine.addField(TaxEngineRetailConstants.Skipped, BoolToYesNo(salesLine.Excluded));
                taxableLine.addField(TaxEngineRetailConstants.TableId, TaxEngineRetailConstants.TransactionLineTableId);

                taxableLine.addField(TaxEngineRetailConstants.TaxDirection, TaxEngineRetailConstants.SalesTaxPayable);

                taxableLine.addField(TaxEngineRetailConstants.TaxDocumentPurpose, TaxEngineRetailConstants.Transaction);
                taxableLine.addField(TaxEngineRetailConstants.TransactionCurrency, salesLine.RetailTransaction.StoreCurrencyCode);
                taxableLine.addField(TaxEngineRetailConstants.TransactionDate, GetTransactionDate(salesLine.Transaction));

                taxableLine.addField(TaxEngineRetailConstants.PostToLedger, TaxEngineRetailConstants.No);
                taxableLine.addField(TaxEngineRetailConstants.EnableAccounting, TaxEngineRetailConstants.Yes);
                taxableLine.addField(TaxEngineRetailConstants.IsScrap, TaxEngineRetailConstants.No);
                taxableLine.addField(TaxEngineRetailConstants.PricesIncludeSalesTax, BoolToYesNo(retailTransaction.TaxIncludedInPrice));

                taxableLines.Add(taxableLine);

                documentMapping.AddTaxableItemMap(salesLine, new TaxableItemId(TaxEngineRetailConstants.TransactionLineTableId, lineNum));

                lineNum++;
            }

            return taxableLines;
        }

        /// <summary>
        /// Converts charge method enum to string acceptable by GTE.
        /// </summary>
        /// <param name="chargeMethod">Change method.</param>
        /// <returns>GTE charge method.</returns>
        //private static string ConvertChargeMethod(ChargeMethod chargeMethod)
        //{
        //    string ret = string.Empty;
        //
        //    switch (chargeMethod)
        //    {
        //        //case ChargeMethod.Fixed:
        //        //    ret = "Fixed";
        //        //    break;
        //
        //        case ChargeMethod.Pieces:
        //            ret = "Pcs.";
        //            break;
        //
        //        //case ChargeMethod.Percent:
        //        //    ret = 'Percent';
        //        //    break;
        //
        //        //case ChargeMethod.External:
        //        //    ret = 'External';
        //        //    break;
        //        default:
        //            ret = chargeMethod.ToString();
        //            break;
        //
        //    }
        //
        //    return ret;
        //}

        /// <summary>
        /// Converts Service Category enum to string acceptable by GTE.
        /// </summary>
        /// <param name="serviceCategory">Service Category.</param>
        /// <returns>GTE Service Category.</returns>
        private string ConvertServiceCategory(ServiceCategory serviceCategory)
        {
            string ret = string.Empty;

            switch (serviceCategory)
            {
                case ServiceCategory.Inward:
                    ret = TaxEngineRetailConstants.ServiceCategoryInward;
                    break;

                case ServiceCategory.InterUnit:
                    ret = TaxEngineRetailConstants.ServiceCategoryInterUnit;
                    break;

                case ServiceCategory.Others:
                    ret = TaxEngineRetailConstants.ServiceCategoryOthers;
                    break;

                default:
                    throw new NotSupportedException("Unsupported ServiceCategory");
            }

            return ret;
        }

        /// <summary>
        /// Converts ITC Category enum to string acceptable by GTE.
        /// </summary>
        /// <param name="itcCategory">ITC Category.</param>
        /// <returns>GTE ITC Category.</returns>
        private string ConvertITCCategory(ITCCategory itcCategory)
        {
            string ret = string.Empty;

            switch (itcCategory)
            {
                case ITCCategory.Input:
                    ret = TaxEngineRetailConstants.ITCCategoryInput;
                    break;

                case ITCCategory.CapitalGoods:
                    ret = TaxEngineRetailConstants.ITCCategoryCapitalGoods;
                    break;

                case ITCCategory.Others:
                    ret = TaxEngineRetailConstants.ITCCategoryOthers;
                    break;

                default:
                    throw new NotSupportedException("Unsupported ITCCategory");
            }

            return ret;
        }

        /// <summary>
        /// Gets charge date. Current date if charge date is not defined on miscellaneous charge line.
        /// </summary>
        /// <param name="lineItem">Miscellaneous charge line item.</param>
        /// <returns>Charge date.</returns>
        private DateTime GetChargeDate(MiscellaneousCharge lineItem)
        {
            DateTime chargeDate = DateTime.Today;

            if (lineItem.BeginDateTime != DateTime.MinValue)
            {
                chargeDate = lineItem.BeginDateTime;
            }

            return chargeDate;
        }

        /// <summary>
        /// Builds taxable document lines for miscellanous charges.
        /// </summary>
        /// <param name="documentMapping">Document mapping.</param>
        /// <param name="miscellaneousCharges">Miscellanous charges.</param>
        /// <param name="lineTableId">Owner table id (either header or line).</param>
        /// <param name="salesLine">Sales line.</param>
        /// <returns>List of taxable lines.</returns>
        private List<TaxableDocumentLineObject> BuildTaxableDocumentSubLinesFromMiscellanousCharges(TaxableDocumentMapping documentMapping, IEnumerable<MiscellaneousCharge> miscellaneousCharges, int lineTableId, SaleLineItem salesLine)
        {
            List<TaxableDocumentLineObject> taxableSubLines = new List<TaxableDocumentLineObject>();

            long lineNum = TaxEngineRetailConstants.TransactionRecordIdFirstElement;

            foreach (MiscellaneousCharge charge in miscellaneousCharges)
            {
                TaxableDocumentLineObject taxableSubLine = new TaxableDocumentLineObject();

                taxableSubLine.ModelFieldName = TaxEngineRetailConstants.SubLines;
                taxableSubLine.TransactionLineTableId = lineTableId;
                taxableSubLine.TransactionLineRecordId = lineNum;

                taxableSubLine.addField(TaxEngineRetailConstants.ChargesCode, charge.ChargeCode);
                taxableSubLine.addField(TaxEngineRetailConstants.LineType, TaxEngineRetailConstants.LineTypeMiscChargeLine);
                taxableSubLine.addField(TaxEngineRetailConstants.ProductType, TaxEngineRetailConstants.Service);
                taxableSubLine.addField(TaxEngineRetailConstants.DeliveryDate, GetChargeDate(charge));
                taxableSubLine.addField(TaxEngineRetailConstants.NetAmount, charge.NetAmountPerUnit);
                taxableSubLine.addField(TaxEngineRetailConstants.Exempt, BoolToYesNo(charge.IndiaTaxInformation.IsExempt));
                taxableSubLine.addField(TaxEngineRetailConstants.Purpose, TaxEngineRetailConstants.Transaction);
                taxableSubLine.addField(TaxEngineRetailConstants.ConsumptionState, GetConsumptionStateCode(retailTransaction));
                taxableSubLine.addField(TaxEngineRetailConstants.ServiceCategory, ConvertServiceCategory(charge.IndiaTaxInformation.ServiceCategory));
                taxableSubLine.addField(TaxEngineRetailConstants.AssessableValue, charge.Amount);
                taxableSubLine.addField(TaxEngineRetailConstants.InterState, IsInterState(salesLine));
                taxableSubLine.addField(TaxEngineRetailConstants.PartyGSTRegistrationNumber, string.Empty);
                taxableSubLine.addField(TaxEngineRetailConstants.HSNCode, charge.IndiaTaxInformation.HSNCode);
                taxableSubLine.addField(TaxEngineRetailConstants.SAC, charge.IndiaTaxInformation.ServiceAccountingCode);
                taxableSubLine.addField(TaxEngineRetailConstants.GSTRegistrationNumber, ApplicationSettings.Terminal.IndiaGSTRegistrationNumber);
                taxableSubLine.addField(TaxEngineRetailConstants.ITCCategory, ConvertITCCategory(charge.IndiaTaxInformation.ITCCategory));

                taxableSubLine.addField(TaxEngineRetailConstants.TaxDocumentPurpose, TaxEngineRetailConstants.Transaction);
                taxableSubLine.addField(TaxEngineRetailConstants.TransactionCurrency, retailTransaction.StoreCurrencyCode);
                taxableSubLine.addField(TaxEngineRetailConstants.TransactionDate, GetChargeDate(charge));
                taxableSubLine.addField(TaxEngineRetailConstants.TaxDirection, TaxEngineRetailConstants.SalesTaxPayable);
                taxableSubLine.addField(TaxEngineRetailConstants.Return, TaxEngineRetailConstants.No);
                taxableSubLine.addField(TaxEngineRetailConstants.RecId, lineNum);
                taxableSubLine.addField(TaxEngineRetailConstants.TableId, lineTableId);

                taxableSubLine.addField(TaxEngineRetailConstants.PostToLedger, TaxEngineRetailConstants.No);
                taxableSubLine.addField(TaxEngineRetailConstants.EnableAccounting, TaxEngineRetailConstants.Yes);
                taxableSubLine.addField(TaxEngineRetailConstants.IsScrap, TaxEngineRetailConstants.No);
                taxableSubLine.addField(TaxEngineRetailConstants.PricesIncludeSalesTax, BoolToYesNo(retailTransaction.TaxIncludedInPrice));

                taxableSubLines.Add(taxableSubLine);

                documentMapping.AddTaxableItemMap(charge, new TaxableItemId(lineTableId, lineNum));

                lineNum++;
            }

            return taxableSubLines;
        }
    }
}
