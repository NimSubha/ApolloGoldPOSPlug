/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using LSRetailPosis.Transaction.Line.GiftCertificateItem;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.ShtrihPrinter
{
    /// <summary>
    /// Line manager class contains the logic for working with line elements of printer configuration file (<see cref="Line"/> configuration class).
    /// </summary>          
    internal sealed class LineManager
    {
        /// <summary>
        /// Used to return empty collection instead of null
        /// </summary>
        readonly static Lazy<IEnumerable<Line>> emptyLineCollection = new Lazy<IEnumerable<Line>>(() => new Line[] { });

        /// <summary>
        /// Creates a new instance of the LineManager class.
        /// </summary>
        /// <param name="transaction">The <see cref="RetailTransaction"/> object.</param>
        public LineManager(RetailTransaction transaction)
        {
            PopulateLineDictionaryForTransaction(transaction);
        }

        /// <summary>
        /// Creates new instance of the <see cref="LineManager"/> class.
        /// </summary>
        /// <param name="lineItem">Sales line item</param>
        public LineManager(LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem lineItem)
        {
            PopulateLineDictionaryForSale(lineItem);
            PopulateLineDictionaryForTransaction(null);
        }

        /// <summary>
        /// Creates an instance of <see cref="LineManager"/> from <see cref="ILoyaltyCardData"/> object.
        /// </summary>
        /// <param name="loyaltyCardData">The <see cref="ILoyaltyCardData"/> object.</param>
        /// <param name="currentPosDateTime">Current POS date and time to be printed in receipt <see cref="currentPosDateTime"/>.</param>
        public LineManager(ILoyaltyCardData loyaltyCardData, DateTime currentPosDateTime)
        {
            PopulateLineDictionaryFromLoyaltyCardData(loyaltyCardData, currentPosDateTime);
        }

        /// <summary>
        /// Contains values for paramaters
        /// </summary>                
        private IDictionary<FieldType, string> lineDictionary = new Dictionary<FieldType, string>();

        /// <summary>
        /// Determines whether the configuration line should be printed.
        /// </summary>        
        /// <param name="line">Configuration line</param>        
        /// <returns>True if line should not be printed out, false otherwise.</returns>
        public Boolean HideLine(Line line)
        {
            if (line.HideIfEmptyField != null && line.HideIfEmptyField.HasValue)
            {
                return string.IsNullOrEmpty(lineDictionary[line.HideIfEmptyField.Value]);
            }

            return false;
        }

        /// <summary>
        /// Generates formated text value of the line.
        /// </summary>        
        /// <param name="line">The <see cref="Line"/> object representing the confiruration file element.</param>        
        /// <returns>The formated text of the line.</returns>
        public string GetTextLine(Line line)
        {
            string result = string.Empty;

            foreach (Field field in line.FieldCollection)
            {
                result += FieldManager.GetFormatedText(field, lineDictionary);
            }

            return result;
        }

        /// <summary>
        /// Determines whether the line type is a type of discount.
        /// </summary>
        /// <param name="lineType">Line type</param>
        /// <returns>true if line type is a type of discount; false otherwise.</returns>
        public static bool LineTypeIsOfDiscountType(LineType lineType)
        {
            return (lineType == LineType.SummaryDiscount ||
                    lineType == LineType.LineDiscount ||
                    lineType == LineType.PeriodicDiscount ||
                    lineType == LineType.ReceiptDiscount ||
                    lineType == LineType.LoyaltyDiscount);
        }

        /// <summary>
        /// Updates discount amount field value in the lines dictionary.
        /// </summary>
        /// <param name="discountType">Discount type</param>
        /// <param name="discountAmount">Discount amount</param>
        /// <remarks>Used for printing discount amounts in discount lines of totals header document section.</remarks>
        public void UpdateDiscountAmountFieldValue(DiscountType discountType, decimal discountAmount)
        {
            var discountField = FieldManager.DiscountAmountFields.FirstOrDefault(df => FieldManager.FieldType2DiscountType(df) == discountType);

            lineDictionary[discountField] = discountAmount != 0m ? discountAmount.ToString("N", CultureInfo.InvariantCulture) : string.Empty;
        }

        /// <summary>
        /// Populates line dictionary by values from the SalesLineItem.
        /// </summary>        
        /// <param name="lineItem">An object containing the information about sales item.</param>       
        private void PopulateLineDictionaryForSale(LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem lineItem)
        {
            lineDictionary[FieldType.ItemNo] = lineItem != null ? lineItem.ItemId : string.Empty;
            lineDictionary[FieldType.ItemName] = lineItem != null ? lineItem.Description : string.Empty;
            lineDictionary[FieldType.ItemAlias] = lineItem != null ? lineItem.DescriptionAlias : string.Empty;
            lineDictionary[FieldType.ItemBarcode] = lineItem != null ? lineItem.BarcodeId : string.Empty;
            lineDictionary[FieldType.UnitOfMeasure] = lineItem != null ? lineItem.SalesOrderUnitOfMeasure : string.Empty;
            var giftCardItem = lineItem as GiftCertificateItem;
            lineDictionary[FieldType.GiftCardId] = giftCardItem != null ? giftCardItem.SerialNumber : string.Empty;

            if ((lineItem != null) && DiscountHelper.SalesLineHasDiscount(lineItem))
            {
                foreach (var discountField in FieldManager.DiscountAmountFields)
                {
                    var discountAmount = Math.Abs(DiscountHelper.GetSalesLineDiscountAmount(FieldManager.FieldType2DiscountType(discountField), lineItem));

                    lineDictionary[discountField] = (discountAmount != 0m) ? discountAmount.ToString("N", CultureInfo.InvariantCulture) : string.Empty;
                }

                foreach (var discountField in FieldManager.DiscountPercentFields)
                {
                    var discountPercent = Math.Abs(DiscountHelper.GetSalesLineDiscountPercent(FieldManager.FieldType2DiscountType(discountField), lineItem));

                    lineDictionary[discountField] = discountPercent.ToString("N", CultureInfo.InvariantCulture);
                }

                lineDictionary[FieldType.PeriodicDiscountName] = lineItem.TotalPeriodicOfferId ?? string.Empty;
            }
            else
            {
                lineDictionary[FieldType.LineDiscountAmount] = string.Empty;
                lineDictionary[FieldType.PeriodicDiscountAmount] = string.Empty;
                lineDictionary[FieldType.ReceiptDiscountAmount] = string.Empty;
                lineDictionary[FieldType.SummaryDiscountAmount] = string.Empty;
                lineDictionary[FieldType.LoyaltyAmount] = string.Empty;
                lineDictionary[FieldType.LineDiscountPercent] = string.Empty;
                lineDictionary[FieldType.PeriodicDiscountPercent] = string.Empty;
                lineDictionary[FieldType.ReceiptDiscountPercent] = string.Empty;
                lineDictionary[FieldType.SummaryDiscountPercent] = string.Empty;
                lineDictionary[FieldType.LoyaltyPercent] = string.Empty;
                lineDictionary[FieldType.PeriodicDiscountName] = string.Empty;
            }
        }

        /// <summary>
        /// Pupulates line dictionary by values from the transaction.
        /// </summary>        
        /// <param name="transaction"><see cref="RetailTransaction"/> object.</param>       
        private void PopulateLineDictionaryForTransaction(RetailTransaction transaction)
        {
            lineDictionary[FieldType.LoyaltyCard] = string.Empty;
            lineDictionary[FieldType.LoyaltyBalance] = string.Empty;
            lineDictionary[FieldType.LoyaltyAdded] = string.Empty;
            lineDictionary[FieldType.LoyaltyUsed] = string.Empty;

            if (transaction != null)
            {
                lineDictionary[FieldType.TerminalId] = transaction.TerminalId;
                lineDictionary[FieldType.Cashier] = String.IsNullOrEmpty(transaction.OperatorNameOnReceipt) ? transaction.OperatorName : transaction.OperatorNameOnReceipt;
                lineDictionary[FieldType.Address] = transaction.StoreAddress;
                lineDictionary[FieldType.ReceiptNumber] = PosApplication.Instance.Services.Peripherals.FiscalPrinter.GetNextReceiptId(transaction);
                lineDictionary[FieldType.StoreName] = transaction.StoreName;
                lineDictionary[FieldType.StorePhoneNo] = transaction.StorePhone;
                lineDictionary[FieldType.Salesperson] = string.IsNullOrEmpty(transaction.SalesPersonNameOnReceipt) ? transaction.SalesPersonName : transaction.SalesPersonNameOnReceipt;
                lineDictionary[FieldType.Customer] = transaction.Customer.Name;
                lineDictionary[FieldType.GiftCardId] =
                    string.Join(", ",
                        (from tl in transaction.TenderLines
                         let gc = tl as IGiftCardTenderLineItem
                         where gc != null && !gc.Voided
                         select gc.SerialNumber));

                if (transaction.LoyaltyItem != null)
                {
                    var loyaltyManager = new LoyaltyManager(transaction);

                    lineDictionary[FieldType.LoyaltyCard] = transaction.LoyaltyItem.LoyaltyCardNumber;
                    lineDictionary[FieldType.LoyaltyBalance] = loyaltyManager.LoyaltyBalance.ToString("N", CultureInfo.InvariantCulture);
                    lineDictionary[FieldType.LoyaltyAdded] = Math.Abs(loyaltyManager.LoyaltyPointsAdded).ToString("N", CultureInfo.InvariantCulture);
                    lineDictionary[FieldType.LoyaltyUsed] = Math.Abs(loyaltyManager.LoyaltyPointsUsed).ToString("N", CultureInfo.InvariantCulture);
                    lineDictionary[FieldType.LoyaltyAmount] = (loyaltyManager.LoyaltyAmount != 0m) ? Math.Abs(loyaltyManager.LoyaltyAmount).ToString("N", CultureInfo.InvariantCulture) : string.Empty;
                    lineDictionary[FieldType.LoyaltyPercent] = Math.Abs(loyaltyManager.LoyaltyPercent).ToString("N", CultureInfo.InvariantCulture);
                }

                InitDiscountFieldsFromTransaction(transaction);
            }
            else
            {
                PopulateLineDictionaryDefaultData();
            }
        }

        /// <summary>
        /// Populates discount fields of line dictionary from the retail transaction.
        /// </summary>
        /// <param name="transaction">Retail transaction</param>
        public void InitDiscountFieldsFromTransaction(RetailTransaction transaction)
        {
            if (DiscountHelper.RetailTransactionHasDiscount(transaction))
            {
                foreach (var discountField in FieldManager.DiscountAmountFields)
                {
                    var discountAmount = Math.Abs(DiscountHelper.GetRetailTransactionDiscountAmount(FieldManager.FieldType2DiscountType(discountField), transaction));

                    lineDictionary[discountField] = (discountAmount != 0m) ? discountAmount.ToString("N", CultureInfo.InvariantCulture) : string.Empty;
                }

                foreach (var discountField in FieldManager.DiscountPercentFields)
                {
                    var discountPercent = Math.Abs(DiscountHelper.GetRetailTransactionDiscountPercent(FieldManager.FieldType2DiscountType(discountField), transaction));

                    lineDictionary[discountField] = discountPercent.ToString("N", CultureInfo.InvariantCulture);
                }
            }
            else
            {
                lineDictionary[FieldType.LineDiscountAmount] = string.Empty;
                lineDictionary[FieldType.PeriodicDiscountAmount] = string.Empty;
                lineDictionary[FieldType.ReceiptDiscountAmount] = string.Empty;
                lineDictionary[FieldType.SummaryDiscountAmount] = string.Empty;
                lineDictionary[FieldType.LineDiscountPercent] = string.Empty;
                lineDictionary[FieldType.PeriodicDiscountPercent] = string.Empty;
                lineDictionary[FieldType.ReceiptDiscountPercent] = string.Empty;
                lineDictionary[FieldType.SummaryDiscountPercent] = string.Empty;
                lineDictionary[FieldType.RoundingDiscountAmount] = string.Empty;
                lineDictionary[FieldType.RoundingDiscountPercent] = string.Empty;
            }
        }

        public void UpdateDiscountFieldsWithRounding(RetailTransaction transaction, LineType lineType)
        {
            decimal roundingDiscountAmount = Math.Abs(DiscountHelper.GetRetailTransactionDiscountAmount(FieldManager.FieldType2DiscountType(FieldType.RoundingDiscountAmount), transaction));
            decimal roundingDiscountPercent = Math.Abs(DiscountHelper.GetRetailTransactionDiscountPercent(FieldManager.FieldType2DiscountType(FieldType.RoundingDiscountPercent), transaction));
            lineDictionary[FieldType.RoundingDiscountAmount] = (roundingDiscountAmount != decimal.Zero) ? roundingDiscountAmount.ToString("N", CultureInfo.InvariantCulture) : string.Empty;
            lineDictionary[FieldType.RoundingDiscountPercent] = (roundingDiscountPercent != decimal.Zero) ? roundingDiscountPercent.ToString("N", CultureInfo.InvariantCulture) : string.Empty;

            if (lineType == LineType.ReceiptDiscount)
            {
                decimal receiptDiscountAmount = Math.Abs(DiscountHelper.GetRetailTransactionDiscountAmount(FieldManager.FieldType2DiscountType(FieldType.ReceiptDiscountAmount), transaction));
                decimal receiptDiscountPercent = Math.Abs(DiscountHelper.GetRetailTransactionDiscountPercent(FieldManager.FieldType2DiscountType(FieldType.ReceiptDiscountPercent), transaction));

                receiptDiscountAmount += roundingDiscountAmount;
                receiptDiscountPercent += roundingDiscountPercent;

                lineDictionary[FieldType.ReceiptDiscountAmount] = (receiptDiscountAmount != decimal.Zero) ? receiptDiscountAmount.ToString("N", CultureInfo.InvariantCulture) : string.Empty;
                lineDictionary[FieldType.ReceiptDiscountPercent] = (receiptDiscountPercent != decimal.Zero) ? receiptDiscountPercent.ToString("N", CultureInfo.InvariantCulture) : string.Empty;
            }
            else if (lineType == LineType.SummaryDiscount)
            {
                decimal summaryDiscountAmount = Math.Abs(DiscountHelper.GetRetailTransactionDiscountAmount(FieldManager.FieldType2DiscountType(FieldType.SummaryDiscountAmount), transaction));
                decimal summaryDiscountPercent = Math.Abs(DiscountHelper.GetRetailTransactionDiscountPercent(FieldManager.FieldType2DiscountType(FieldType.SummaryDiscountPercent), transaction));

                summaryDiscountAmount += roundingDiscountAmount;
                summaryDiscountPercent += roundingDiscountPercent;

                lineDictionary[FieldType.SummaryDiscountAmount] = (summaryDiscountAmount != decimal.Zero) ? summaryDiscountAmount.ToString("N", CultureInfo.InvariantCulture) : string.Empty;
                lineDictionary[FieldType.SummaryDiscountPercent] = (summaryDiscountPercent != decimal.Zero) ? summaryDiscountPercent.ToString("N", CultureInfo.InvariantCulture) : string.Empty;
            }
        }

        /// <summary>
        /// Populates line dictionary with default values.
        /// </summary>
        private void PopulateLineDictionaryDefaultData()
        {
            lineDictionary[FieldType.TerminalId] = ApplicationSettings.Terminal.TerminalId;
            lineDictionary[FieldType.Cashier] = ApplicationSettings.Terminal.TerminalOperator.NameOnReceipt;
            lineDictionary[FieldType.Address] = ApplicationSettings.Terminal.StoreAddress;
            lineDictionary[FieldType.ReceiptNumber] = string.Empty;
            lineDictionary[FieldType.StoreName] = ApplicationSettings.Terminal.StoreName;
            lineDictionary[FieldType.StorePhoneNo] = ApplicationSettings.Terminal.StorePhone;
            lineDictionary[FieldType.Salesperson] = string.Empty;
            lineDictionary[FieldType.Customer] = string.Empty;
        }

        /// <summary>
        /// Populates line dictionary with the value from the loyalty card data.
        /// </summary>
        /// <param name="loyaltyCardData">An instance of the <see cref="ILoyaltyCardData"/>.</param>
        /// <param name="currentPosDateTime">Current POS date and time to be printed in receipt <see cref="currentPosDateTime"/>.</param>
        private void PopulateLineDictionaryFromLoyaltyCardData(ILoyaltyCardData loyaltyCardData, DateTime currentPosDateTime)
        {
            // Loyalty card related fields
            lineDictionary[FieldType.LoyaltyCard] = loyaltyCardData.CardNumber;
            lineDictionary[FieldType.LoyaltyCustomerName] = loyaltyCardData.CustomerName;
            lineDictionary[FieldType.LoyaltySchemeDescription] = loyaltyCardData.SchemeDescription;
            lineDictionary[FieldType.LoyaltyCardStatus] = loyaltyCardData.StatusString;
            lineDictionary[FieldType.LoyaltyCardType] = loyaltyCardData.TenderTypeString;
            lineDictionary[FieldType.LoyaltyAddedTotal] = loyaltyCardData.IssuedPoints.ToString("N", CultureInfo.InvariantCulture);
            lineDictionary[FieldType.LoyaltyUsedTotal] = loyaltyCardData.UsedPoints.ToString("N", CultureInfo.InvariantCulture);
            lineDictionary[FieldType.LoyaltyExpiredTotal] = loyaltyCardData.ExpiredPoints.ToString("N", CultureInfo.InvariantCulture);
            lineDictionary[FieldType.LoyaltyBalance] = loyaltyCardData.BalancePoints.ToString("N", CultureInfo.InvariantCulture);
            lineDictionary[FieldType.POSDate] = currentPosDateTime.ToShortDateString();
            lineDictionary[FieldType.POSTime] = currentPosDateTime.ToShortTimeString();
            // Non loyalty card related fields
            PopulateLineDictionaryDefaultData();
            lineDictionary[FieldType.Customer] = loyaltyCardData.CustomerName;
        }

        /// <summary>
        /// Gets the line collection for a specific document section of the layout.
        /// </summary>        
        /// <param name="config">Printer configuration</param>       
        /// <param name="layout">Layout type</param>       
        /// <param name="documentSectionType">Document section type</param>              
        /// <returns>Enumerable collection of lines.</returns>
        public static IEnumerable<Line> GetLineCollection(PrinterConfiguration config, LayoutType layout, DocumentSectionType documentSectionType)
        {
            IEnumerable<Line> resultLineCollection = null;            

            if (config != null)            
            {
                var configLayout = config.Layouts.FindLayout(layout);
                if (configLayout != null)
                {
                    var docSection = configLayout.DocumentSectionCollection.FindDocumentSection(documentSectionType);
                    if (docSection != null)
                    {
                        resultLineCollection = docSection.LineCollection.Cast<Line>();
                    }
                }
            }

            return resultLineCollection ?? emptyLineCollection.Value;
        }
    }
}
