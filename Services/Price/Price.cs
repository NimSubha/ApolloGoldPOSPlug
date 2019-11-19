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
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.Line.SalesInvoiceItem;
using LSRetailPosis.Transaction.Line.SalesOrderItem;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.DataEntity.Extensions;
using Microsoft.Dynamics.Retail.Diagnostics;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using System.Text;
using System.IO;
using Microsoft.CSharp.RuntimeBinder;
using System.Reflection;

namespace Microsoft.Dynamics.Retail.Pos.PriceService
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "EF pushed this over the limit. This class needs refactoring.")]
    [Export(typeof(IPrice))]
    public class Price : IPrice, IInitializeable
    {
        #region TransactionType
        private enum TransactionType
        {
            Sale = 0,
            Purchase = 1,
            Exchange = 3,
            PurchaseReturn = 2,
            ExchangeReturn = 4,
        }
        #endregion


        #region Member variables

        private PriceParameters salesPriceParameters;
        private ReadOnlyCollection<string> storePriceGroups;
        private ReadOnlyCollection<long> storePriceGroupIds;

        private const string SelectFromPeriodicDiscount = @" 
                SELECT
                    promo.OFFERID,
                    promo.VALIDATIONPERIODID,
                    promo.VALIDFROM,
                    promo.VALIDTO,
                    promo.CONCURRENCYMODE,
                    promo.DATEVALIDATIONTYPE,

                    promoOffer.DISCOUNTMETHOD,
                    promoOffer.OFFERPRICE,
                    promoOffer.OFFERPRICEINCLTAX,
                    promoOffer.DISCPCT,
                    promoOffer.DISCAMOUNT
 
                FROM RETAILPERIODICDISCOUNT promo
                  JOIN RETAILPERIODICDISCOUNTLINE promoLine
                      on promo.OFFERID = promoLine.OFFERID
                  JOIN RETAILDISCOUNTLINEOFFER promoOffer
                    on promoLine.RECID = promoOffer.RECID
                  LEFT JOIN UNITOFMEASURE uom
                    ON uom.RECID = promoLine.UNITOFMEASURE
                  join RETAILGROUPMEMBERLINE rgl
                      on promoLine.RETAILGROUPMEMBERLINE = rgl.RECID ";

        private const string WherePeriodicDiscount = @" 
                   where promo.STATUS = 1
                   AND promo.PERIODICDISCOUNTTYPE = 3
                   AND promo.DATAAREAID = @DATAAREAID
                   AND (promo.CURRENCYCODE in ('', @STORECURRENCY))
                   AND (promoLine.UNITOFMEASURE = 0 OR uom.SYMBOL in ('', @UNITOFMEASURE))
                   AND (promo.PRICEDISCGROUP IN (SELECT spg.PRICEGROUPID FROM @STOREPRICEGROUPS spg))
                   AND ((promo.VALIDFROM <= @TODAY OR promo.VALIDFROM <= @NODATE)
                   AND (promo.VALIDTO >= @TODAY OR promo.VALIDTO <= @NODATE)) ";

        private const string SqlUnionAll = " UNION ALL ";

        /// <summary>
        /// Sentinal value for 'no date specified'
        /// </summary>
        internal static readonly DateTime NoDate = new DateTime(1900, 1, 1);

        // See Discount.cs for duplicate definition.  Clean up if refactoring common code between services ever occurs
        internal enum DateValidationTypes
        {
            Advanced = 0,
            Standard = 1
        }

        private DM.DiscountDataManager discountDataManager =
            new DM.DiscountDataManager(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);

        #endregion

        #region Properties
        /// <summary>
        /// IApplication instance.
        /// </summary>
        private IApplication application;

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
        [Import]
        public IApplication Application
        {
            get
            {
                return this.application;
            }
            set
            {
                this.application = value;
                InternalApplication = value;
            }
        }

        /// <summary>
        /// Gets or sets the static IApplication instance.
        /// </summary>
        internal static IApplication InternalApplication { get; private set; }

        #endregion

        #region IInitializeableV1 Members

        /// <summary>
        /// Initialize price service.
        /// </summary>
        public void Initialize()
        {
            this.salesPriceParameters = GetPriceParameters();
            this.storePriceGroups = this.discountDataManager.PriceGroupsFromStore(ApplicationSettings.Terminal.StorePrimaryId);
            this.storePriceGroupIds = this.discountDataManager.PriceGroupIdsFromStore(ApplicationSettings.Terminal.StorePrimaryId);
        }

        /// <summary>
        /// Uninitialize price service.
        /// </summary>
        public void Uninitialize()
        {
        }

        #endregion

        #region IPrice

        /// <summary>
        /// <remarks></remarks>
        /// </summary>
        /// <param name="retailTransaction"></param>
        public void GetPrice(IRetailTransaction retailTransaction)
        {
            RetailTransaction transaction = (RetailTransaction)retailTransaction;
            ICustomerOrderTransaction orderTransaction = retailTransaction as ICustomerOrderTransaction;
            bool priceLock = (orderTransaction == null) ? false : (orderTransaction.LockPrices);

            if (transaction != null
                && transaction.CalculableSalesLines != null
                && transaction.CalculableSalesLines.Any())
            {
                SaleLineItem saleItem = transaction.CurrentSaleLineItem;

                Decimal quantity = GetQuantity(transaction, saleItem.ItemId);
                Decimal price = decimal.Zero;

                // For gift cards the only valid price is the one that the user typed in.
                // So we do not get a price from the database.
                if (saleItem is IGiftCardLineItem)
                {
                    saleItem.WasChanged = true;
                    return;
                }

                // For sales orders the only valid price is the one that is already on the sales order
                // So we do not get a price from the database.
                if (saleItem is SalesOrderLineItem)
                {
                    saleItem.WasChanged = true;
                    return;
                }

                // For sales invoices the only valid price is the one that is already on the sales invoice
                // So we do not get a price from the database.
                if (saleItem is SalesInvoiceLineItem)
                {
                    saleItem.WasChanged = true;
                    return;
                }

                if (saleItem.HasBeenRecalled && priceLock)
                {
                    // Item was ordered and prices are Locked
                    return;
                }

                price = GetActivePrice(transaction, saleItem, quantity);

                //Update all item with the item id with this price
                // This applies to all identical items except those who have had their price overridden/keyed in and items returned with a receipt.

                //Base
                #region Base
                /* foreach (SaleLineItem saleLineItem in transaction.CalculableSalesLines)
                {
                    if (!saleItem.PriceOverridden
                        && saleLineItem.ItemId == saleItem.ItemId
                        && !saleLineItem.PriceOverridden
                        && !saleLineItem.PriceHasBeenKeyedIn
                        && !saleLineItem.ReceiptReturnItem
                        && saleLineItem.SalesOrderUnitOfMeasure == saleItem.SalesOrderUnitOfMeasure
                        && saleLineItem.PriceUnit == saleItem.PriceUnit
                        && saleLineItem.Dimension.IsEquivalent(saleItem.Dimension)
                        && !transaction.RecalledOrder
                        && saleLineItem.SerialId == saleItem.SerialId) // Included Serial ID to change the price only for that serial ID
                    {
                        saleLineItem.Price = price;

                        if (saleLineItem.PriceInBarcode)
                        {
                            if (saleLineItem.Price != 0 && saleLineItem.Price != saleLineItem.BarcodeEmbeddedPrice)
                            {
                                decimal calcQuantity = Convert.ToDecimal(this.Application.Services.Rounding.RoundQuantity(saleLineItem.BarcodeEmbeddedPrice / saleLineItem.Price, saleLineItem.SalesOrderUnitOfMeasure));
                                saleLineItem.Quantity = saleLineItem.Quantity < 0 ? -1 * calcQuantity : calcQuantity;
                                saleLineItem.BarcodeCalculatedQuantity = calcQuantity;
                            }
                            else
                            {
                                saleLineItem.Price = saleLineItem.BarcodeEmbeddedPrice;
                                saleLineItem.BarcodeCalculatedQuantity = 1;
                            }
                        }
                    }

                    saleLineItem.WasChanged = true;
                }*/
                #endregion

                //-- Consider for custom Service Items
                //Start:Nim
                #region Nimbus
                string sPOSGSSAdjItemId = "";
                string sPOSGSSDiscItemId = "";
                string sPOSGSSAmountAdjItemId = "";
                string sPOSGSSAmountDiscItemId = "";

                string sCRWREPAIRADJITEMFOROG = "";
                string sCRWREPAIRADJITEM = "";
                string sPOSAdjustmentID = AdjustmentItemID();

                GSSAdjustmentItemID(ref sPOSGSSAdjItemId, ref sPOSGSSDiscItemId, ref sCRWREPAIRADJITEMFOROG,
                    ref sCRWREPAIRADJITEM, ref sPOSGSSAmountAdjItemId, ref sPOSGSSAmountDiscItemId);

                string sGMAAdvItem = "";
                string sGMALGItem = "";
                string sGMADiscItem = "";
                string sGMAGSTItem = "";

                GMAAdjustmentItemID(ref sGMAAdvItem, ref sGMAGSTItem, ref sGMALGItem, ref sGMADiscItem);

                foreach (SaleLineItem saleLineItem in transaction.CalculableSalesLines)
                {
                    if (!saleItem.PriceOverridden
                        && saleLineItem.ItemId == saleItem.ItemId
                        && !saleLineItem.PriceOverridden
                        && !saleLineItem.PriceHasBeenKeyedIn
                        && !saleLineItem.ReceiptReturnItem
                        && saleLineItem.SalesOrderUnitOfMeasure == saleItem.SalesOrderUnitOfMeasure
                        && saleLineItem.UnitQuantity == saleItem.UnitQuantity
                        && saleLineItem.Dimension.IsEquivalent(saleItem.Dimension)
                        && !transaction.RecalledOrder
                        && saleLineItem.SerialId == saleItem.SerialId)// Included Serial ID to change the price only for that serial ID
                    {
                        if (saleLineItem.ItemId == sPOSAdjustmentID
                            || saleLineItem.ItemId == sPOSGSSAdjItemId
                            || saleLineItem.ItemId == sPOSGSSDiscItemId
                            || saleLineItem.ItemId == sCRWREPAIRADJITEMFOROG
                            || saleLineItem.ItemId == sCRWREPAIRADJITEM
                            || saleLineItem.ItemId == sPOSGSSAmountAdjItemId
                            || saleLineItem.ItemId == sPOSGSSAmountDiscItemId

                            || saleLineItem.ItemId == sGMAAdvItem
                            || saleLineItem.ItemId == sGMALGItem
                            || saleLineItem.ItemId == sGMADiscItem
                            || saleLineItem.ItemId == sGMAGSTItem)
                        {
                            // saleLineItem.Price = price;

                            saleLineItem.Price = price;

                            if (saleLineItem.PriceInBarcode)
                            {
                                if (saleLineItem.Price != 0 && saleLineItem.Price != saleLineItem.BarcodeEmbeddedPrice)
                                {
                                    decimal calcQuantity = Convert.ToDecimal(this.Application.Services.Rounding.RoundQuantity(saleLineItem.BarcodeEmbeddedPrice / saleLineItem.Price, saleLineItem.SalesOrderUnitOfMeasure));
                                    saleLineItem.Quantity = saleLineItem.Quantity < 0 ? -1 * calcQuantity : calcQuantity;
                                    saleLineItem.BarcodeCalculatedQuantity = calcQuantity;
                                }
                                else
                                {
                                    saleLineItem.Price = saleLineItem.BarcodeEmbeddedPrice;
                                    saleLineItem.BarcodeCalculatedQuantity = 1;
                                }
                            }
                        }
                    }


                    saleLineItem.WasChanged = true;
                }
                #endregion

                // End:Nim
            }
        }

        /// <summary>
        /// Updates all prices as per included tax.
        /// </summary>
        /// <param name="retailTransaction"></param>
        /// <param name="restoreItemPrices"></param>
        public void UpdateAllPrices(IRetailTransaction retailTransaction, bool restoreItemPrices)
        {
            RetailTransaction transaction = retailTransaction as RetailTransaction;
            if (transaction == null)
            {
                NetTracer.Warning("retailTransaction parameter is null");
                throw new ArgumentNullException("retailTransaction");
            }

            ICustomerOrderTransaction orderTransaction = retailTransaction as ICustomerOrderTransaction;
            bool priceLock = (orderTransaction == null) ? false : (orderTransaction.LockPrices);

            //Default to using the Store setting
            transaction.TaxIncludedInPrice = ApplicationSettings.Terminal.TaxIncludedInPrice;

            // Use setting from the customer if 'Use Customer Based Taxes' is enabled, and a customer has been set.
            if (ApplicationSettings.Terminal.UseCustomerBasedTax
                && transaction.Customer != null
                && (!transaction.Customer.IsEmptyCustomer()))
            {
                transaction.TaxIncludedInPrice = transaction.Customer.PricesIncludeSalesTax;
            }

            foreach (SaleLineItem saleItem in transaction.CalculableSalesLines)
            {
                if (saleItem is IGiftCardLineItem)
                {
                    // Gift card line item doesn'need price calculation.
                    continue;
                }

                // if NOT restoring prices, honor the lock/has-been-recalled flags for customer orders
                if (saleItem.HasBeenRecalled && priceLock && !restoreItemPrices)
                {
                    // Item was ordered and prices are Locked
                    continue;
                }

                if (!saleItem.PriceInBarcode)
                {
                    decimal quantity = GetQuantity(transaction, saleItem.ItemId);

                    decimal priceNow;

                    // If we are restoring item prices, reset Price Override flag.
                    if (saleItem.PriceOverridden && restoreItemPrices)
                    {
                        saleItem.PriceOverridden = false;
                    }

                    // for keyed in and overridden prices, the price is just the current price (per the tax settings).
                    if (saleItem.PriceHasBeenKeyedIn || saleItem.PriceOverridden || saleItem.ReceiptReturnItem)
                    {
                        priceNow = saleItem.Price;
                    }
                    else
                    {
                        // Now use GetActivePrice
                        priceNow = GetActivePrice(transaction, saleItem, quantity);
                    }

                    if (saleItem.Price != priceNow)
                    {
                        saleItem.Price = priceNow;
                        saleItem.WasChanged = true;
                    }
                }
            }
        }

        private PriceParameters GetPriceParameters()
        {
            string queryString = "SELECT SALESPRICEACCOUNTITEM, SALESPRICEGROUPITEM, SALESPRICEALLITEM FROM PRICEPARAMETERS WHERE "
                + "DATAAREAID = @DATAAREAID ";
            SqlConnection connection = Application.Settings.Database.Connection;
            string dataAreaId = Application.Settings.Database.DataAreaID;

            bool salesPriceAccountItem = false;
            bool salesPriceGroupItem = false;
            bool salesPriceAllItem = false;

            try
            {
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    SqlParameter dataAreaIdParm = command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
                    dataAreaIdParm.Value = dataAreaId;

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow);
                    reader.Read();
                    if (reader.HasRows)
                    {
                        if (reader.GetInt32(reader.GetOrdinal("SALESPRICEACCOUNTITEM")) == 1) { salesPriceAccountItem = true; } else { salesPriceAccountItem = false; }
                        if (reader.GetInt32(reader.GetOrdinal("SALESPRICEGROUPITEM")) == 1) { salesPriceGroupItem = true; } else { salesPriceGroupItem = false; }
                        if (reader.GetInt32(reader.GetOrdinal("SALESPRICEALLITEM")) == 1) { salesPriceAllItem = true; } else { salesPriceAllItem = false; }
                    }
                    else
                    {
                        salesPriceAccountItem = false;
                        salesPriceGroupItem = false;
                        salesPriceAllItem = false;
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

            return new PriceParameters(salesPriceAccountItem, salesPriceGroupItem, salesPriceAllItem);
        }

        /// <summary>
        /// Loops trough all sale items to find a quantity for a certain item.
        /// </summary>
        /// <param name="retailTransaction">The retail transaction.</param>
        /// <param name="itemId">The item id a quantity is needed.</param>
        /// <returns></returns>
        private static decimal GetQuantity(RetailTransaction retailTransaction, string itemId)
        {
            Decimal quantity = 0;
            foreach (ISaleLineItem saleLineItem in retailTransaction.SaleItems)
            {
                if (saleLineItem.ItemId == itemId && !saleLineItem.Voided)
                {
                    quantity += saleLineItem.Quantity;
                }
            }

            if (quantity == 0) //If item is bought and return in the same quantity, a price is found as if one pcs. was bought, and not zero.
            {
                return 1;
            }
            else
            {
                return quantity;
            }
        }

        // Duplicated in discount.cs.  Refactor when opportunity allows.
        static private bool IsDateWithinStartEndDate(DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return (((dateToCheck >= startDate) || (startDate == NoDate))
                && ((dateToCheck <= endDate) || (endDate == NoDate)));
        }

        /// <summary>
        /// Find the promotion price for a given item
        /// </summary>
        /// <param name="productId">Product Id</param>
        /// <param name="distinctProductVariantId">Variant Id</param>
        /// <param name="unitOfMeasure">Unit of Measure for the promotion (null will find all promotions regardless of UOM)</param>
        /// <returns></returns>
        private List<PromotionInfo> FindPromotionPrice(Int64 productId, Int64 distinctProductVariantId, string unitOfMeasure)
        {
            List<PromotionInfo> promotionLines = new List<PromotionInfo>();
            unitOfMeasure = unitOfMeasure ?? string.Empty; // if null set to empty string.
            DateTime today = DateTime.Now.Date;
            SqlConnection connection = Price.InternalApplication.Settings.Database.Connection;
            string dataAreaId = Price.InternalApplication.Settings.Database.DataAreaID;

            try
            {
                string queryString = string.Concat(
                                                      SelectFromPeriodicDiscount, // Product or Variant, no category
                                                      @" AND ((rgl.VARIANT != 0 AND rgl.VARIANT = @DISTINCTPRODUCTVARIANTID)
                                                          or (rgl.VARIANT = 0 AND rgl.PRODUCT != 0 AND rgl.PRODUCT = @ProductId)) ",
                                                      WherePeriodicDiscount,

                                                      SqlUnionAll,

                                                      SelectFromPeriodicDiscount, // Category with product only
                                                      @" AND (rgl.VARIANT = 0 AND rgl.PRODUCT = 0 AND rgl.CATEGORY != 0)
				                                        JOIN RETAILCATEGORYCONTAINMENTLOOKUP rccl 
                                                             on rccl.CATEGORY = rgl.CATEGORY
                                                        JOIN ECORESPRODUCTCATEGORY pc 
                                                            on pc.CATEGORY = rccl.CONTAINEDCATEGORY and pc.PRODUCT = @ProductId ",
                                                      WherePeriodicDiscount,

                                                      SqlUnionAll,

                                                      SelectFromPeriodicDiscount, // Category with variant only
                                                      @" AND (rgl.VARIANT = 0 AND rgl.PRODUCT = 0 AND rgl.CATEGORY != 0)
				                                        JOIN RETAILCATEGORYCONTAINMENTLOOKUP rccl 
                                                             on rccl.CATEGORY = rgl.CATEGORY
                                                        JOIN RETAILSPECIALCATEGORYVARIANT rscv 
                                                             on rscv.CATEGORY = rccl.CONTAINEDCATEGORY and rscv.PRODUCT = @DISTINCTPRODUCTVARIANTID ",
                                                      WherePeriodicDiscount);

                using (SqlCommand command = new SqlCommand(queryString, connection))
                using (DataTable priceGroupTable = new DataTable())
                {
                    command.Parameters.AddWithValue("@DATAAREAID", dataAreaId);
                    command.Parameters.AddWithValue("@STORECURRENCY", ApplicationSettings.Terminal.StoreCurrency);
                    command.Parameters.AddWithValue("@DISTINCTPRODUCTVARIANTID", distinctProductVariantId);
                    command.Parameters.AddWithValue("@PRODUCTID", productId);
                    command.Parameters.AddWithValue("@UNITOFMEASURE", unitOfMeasure);
                    command.Parameters.AddWithValue("@TODAY", today);
                    command.Parameters.AddWithValue("@NODATE", DateTime.Parse("1900-01-01"));

                    // convert store price group list to data-table for use as TVP in the query.
                    priceGroupTable.Columns.Add("PRICEGROUPID", typeof(long));
                    foreach (long priceGroupId in this.storePriceGroupIds)
                    {
                        priceGroupTable.Rows.Add(priceGroupId);
                    }

                    SqlParameter param = command.Parameters.Add("@STOREPRICEGROUPS", SqlDbType.Structured);
                    param.Direction = ParameterDirection.Input;
                    param.TypeName = "FINDPROMOTIONPRICE_PRICEGROUPS_TABLETYPE";
                    param.Value = priceGroupTable;

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        PromotionInfo promoInfo = new PromotionInfo()
                        {
                            PromoId = (string)reader["OFFERID"],
                            DiscValidationPeriod = (string)reader["VALIDATIONPERIODID"],
                            MaxDiscPct = (decimal)reader["DISCPCT"],
                            MaxDiscAmount = (decimal)reader["DISCAMOUNT"],
                            PriceInclTax = (decimal)reader["OFFERPRICEINCLTAX"],
                            Price = (decimal)reader["OFFERPRICE"],
                            DiscountMethod = (DiscountMethod)reader["DISCOUNTMETHOD"],
                            Concurrency = (ConcurrencyMode)reader["CONCURRENCYMODE"],
                            ValidationType = (DateValidationTypes)reader["DATEVALIDATIONTYPE"],
                            ValidFrom = (DateTime)reader["VALIDFROM"],
                            ValidTo = (DateTime)reader["VALIDTO"],
                        };

                        promotionLines.Add(promoInfo);
                    }

                    // filter valid promo lines after data reader is finished
                    promotionLines = promotionLines.Where(p => IsPromoValid(p)).ToList();
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return promotionLines;
        }

        private bool IsPromoValid(PromotionInfo promoLine)
        {
            return IsPromoPeriodValid(promoLine.ValidationType, promoLine.DiscValidationPeriod, promoLine.ValidFrom, promoLine.ValidTo);
        }

        private bool IsPromoPeriodValid(DateValidationTypes validationType, string validationPeriodId, DateTime startDate, DateTime endDate)
        {
            bool promoPeriodValid = false;
            DateTime currentDate = DateTime.Now;

            if (validationType == DateValidationTypes.Advanced)
            {
                promoPeriodValid = this.Application.Services.Discount.IsValidationPeriodActive(validationPeriodId, currentDate);
            }
            else if (validationType == DateValidationTypes.Standard)
            {
                promoPeriodValid = IsDateWithinStartEndDate(currentDate.Date, startDate, endDate);
            }
            else
            {
                NetTracer.Error("Price::IsPromoPeriodValid: Invalid Discount Validation Type");
                System.Diagnostics.Debug.Fail("Invalid Discount Validation Type");
            }

            return promoPeriodValid;
        }

        private PriceResult FindPriceAgreement(RetailPriceArgs args)
        {
            // salesUOM is the unit of measure being set at the moment
            // inventUOM is the base UOM for the item

            // First we get the price according to the base UOM
            PriceAgreementArgs p = args.AgreementArgsForInventory();
            PriceResult result = TradeAgreementPricing.priceAgr(p, this.salesPriceParameters); // eg: Pcs

            // Is the current UOM something different than the base UOM?
            if (args.SalesUOM != args.InventUOM)
            {
                // Then let's see if we find some price agreement for that UOM
                p = args.ArgreementArgsForSale();
                PriceResult salesUOMResult = TradeAgreementPricing.priceAgr(p, this.salesPriceParameters); // eg: Box

                // If there is a price found then we return that as the price
                if (salesUOMResult.Price > decimal.Zero)
                {
                    return salesUOMResult;
                }
                else
                {
                    return new PriceResult(result.Price * args.UnitQtyConversion.GetFactorForQty(args.Quantity), result.IncludesTax);
                }
            }

            // else we return baseUOM price mulitplied with the unit qty factor.
            return result;
        }

        /// <summary>
        /// Returns the basic price found in the inventory table. Used if no price is found in the PriceDiscountTable
        /// </summary>
        /// <param name="itemId">The unique item id as stored in the inventory table</param>
        /// <returns>The basic sales price</returns>
        public decimal GetBasicPrice(string itemId)
        {
            //SqlConnection connection = Application.Settings.Database.Connection;
            //string dataAreaId = Application.Settings.Database.DataAreaID;

            //Start:Nim
            string dataAreaId = "";
            SqlConnection connection = new SqlConnection();
            if (Application != null)
            {
                connection = Application.Settings.Database.Connection;
                dataAreaId = Application.Settings.Database.DataAreaID;
            }
            else
            {
                connection = LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection;
                dataAreaId = ApplicationSettings.Database.DATAAREAID;
            }
            //End:Nim


            decimal price;

            try
            {
                //moduletype = 2(sale)
                string queryString = "SELECT PRICE AS PRICE, COUNT(*) "
                    + "FROM INVENTTABLEMODULE "
                    + "WHERE DATAAREAID = @DATAAREAID AND MODULETYPE = 2 AND ITEMID = @ITEMID "
                    + "GROUP BY PRICE";
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = itemId;
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = dataAreaId;

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    object obj = command.ExecuteScalar();
                    if (obj == null)
                    {
                        price = 0;
                    }
                    else
                    {
                        price = (decimal)(obj);
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

            return price;
        }

        private PriceResult GetRetailPrice(RetailPriceArgs args)
        {
            PriceResult result;

            //Store and trade agreement price
            result = FindPriceAgreement(args);

            //If the store currency is not the same as the HO currency then check if there is a price for the HO currency in the trade agreements
            if ((result.Price == 0) && (args.CurrencyCode != ApplicationSettings.Terminal.CompanyCurrency))
            {
                string originalCurrencyCode = args.CurrencyCode;
                args.CurrencyCode = ApplicationSettings.Terminal.CompanyCurrency;
                result = FindPriceAgreement(args);
                result = new PriceResult(result, ApplicationSettings.Terminal.CompanyCurrency, originalCurrencyCode);
            }

            return result;
        }

        /*
         * The lowest price from the following becomes the Active price:
         *      Item Card
         *      Trade Agreements
         *      Promotion
         *      Key in new Price
         * 
         * Find the Customer Price
         *      
         * If the Active price is lower then Customer price => Use Active price
         * If the Customer price is lower then Active price => Use Customer Price
         */
        private decimal GetActivePrice(RetailTransaction retailTransaction, SaleLineItem saleItem, Decimal quantity)
        {
            decimal price = 0;

            #region [// GSS Maturity Adjustment]
            string sGSSAdjItemId = "";
            string sGSSDiscItemId = "";
            string sCRWREPAIRADJITEMFOROG = "";
            string sCRWREPAIRADJITEM = "";
            string sGSSAmountAdjItemId = "";
            string sGSSAmountDiscItemId = "";

            GSSAdjustmentItemID(ref sGSSAdjItemId, ref sGSSDiscItemId, ref sCRWREPAIRADJITEMFOROG, ref sCRWREPAIRADJITEM, ref sGSSAmountAdjItemId, ref sGSSAmountDiscItemId);

            if (saleItem.ItemId == sGSSAdjItemId) //"GSS Maturity Deposit")
            {
                saleItem.CostPrice = 0.01M;
                saleItem.Quantity = -1;
                ((LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem)(saleItem)).Comment = "GSS (Gold) Maturity Amount";
                price = Convert.ToDecimal((Convert.ToString(decimal.Round(Convert.ToDecimal(retailTransaction.PartnerData.GSSTotAmt), 14, MidpointRounding.AwayFromZero)))); //Convert.ToDecimal(retailTransaction.PartnerData.GSSTotAmt);
                System.Dynamic.DynamicObject dobj = (System.Dynamic.DynamicObject)retailTransaction.PartnerData;
                return price;
            }
            if (saleItem.ItemId == sGSSDiscItemId) //"GSS Royalty")
            {
                saleItem.CostPrice = 0.01M;
                saleItem.Quantity = -1;
                ((LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem)(saleItem)).Comment = "GSS (Gold) Maturity Royalty Amount";

                if (Convert.ToBoolean(retailTransaction.PartnerData.APPLYGSSDISC) == true)
                {
                    price = GetGssDiscountPriceFromAgreement(retailTransaction, price);

                    if (price > 0)
                        retailTransaction.PartnerData.APPLYGSSDISCDONE = true;
                }
                else
                    price = Convert.ToDecimal(retailTransaction.PartnerData.GSSRoyaltyAmt);

                return price;
            }

            if (saleItem.ItemId == sGSSAmountAdjItemId)
            {
                saleItem.CostPrice = 0.01M;
                saleItem.Quantity = -1;
                ((LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem)(saleItem)).Comment = "GSS (Amount) Maturity Amount";
                price = Convert.ToDecimal((Convert.ToString(decimal.Round(Convert.ToDecimal(retailTransaction.PartnerData.GSSTotAmt), 14, MidpointRounding.AwayFromZero)))); //Convert.ToDecimal(retailTransaction.PartnerData.GSSTotAmt);
                System.Dynamic.DynamicObject dobj = (System.Dynamic.DynamicObject)retailTransaction.PartnerData;
                return price;
            }
            if (saleItem.ItemId == sGSSAmountDiscItemId)
            {
                saleItem.CostPrice = 0.01M;
                saleItem.Quantity = -1;
                ((LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem)(saleItem)).Comment = "GSS (Amount) Maturity Royalty Amount";

                if (Convert.ToBoolean(retailTransaction.PartnerData.APPLYGSSDISC) == true)
                {
                    price = GetGssDiscountPriceFromAgreement(retailTransaction, price);

                    if (price > 0)
                        retailTransaction.PartnerData.APPLYGSSDISCDONE = true;
                }
                else
                    price = Convert.ToDecimal(retailTransaction.PartnerData.GSSRoyaltyAmt);

                return price;
            }

            if (!string.IsNullOrEmpty(Convert.ToString(retailTransaction.PartnerData.RepairNettWtDiff)))
            {
                if (saleItem.ItemId == sCRWREPAIRADJITEMFOROG) //"OG purchase for repair item")
                {
                    saleItem.CostPrice = 0.01M;
                    saleItem.Quantity = Convert.ToDecimal(retailTransaction.PartnerData.RepairNettWtDiff);
                    ((LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem)(saleItem)).Comment = "OG purchase for repair item";

                    price = Convert.ToDecimal(retailTransaction.PartnerData.REPAIRRETMAKINGAMT);
                    System.Dynamic.DynamicObject dobj = (System.Dynamic.DynamicObject)retailTransaction.PartnerData;
                    return price;
                }
            }
            if (!string.IsNullOrEmpty(Convert.ToString(retailTransaction.PartnerData.REPAIRRETCHARGES)))
            {
                if (saleItem.ItemId == sCRWREPAIRADJITEM) //"Repair Charges")
                {
                    saleItem.CostPrice = 0.01M;
                    saleItem.Quantity = 1;
                    ((LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem)(saleItem)).Comment = "Repair Charges";

                    price = Convert.ToDecimal(retailTransaction.PartnerData.REPAIRRETCHARGES);

                    return price;
                }
            }
            #endregion

            #region [// GMA Maturity Adjustment]
            string sGMAAdvItem = "";
            string sGMALGItem = "";
            string sGMADiscItem = "";
            string sGMAGSTItem = "";

            GMAAdjustmentItemID(ref sGMAAdvItem, ref sGMAGSTItem, ref sGMALGItem, ref sGMADiscItem);

            if (saleItem.ItemId == sGMAAdvItem) //"GMA Advance")
            {
                saleItem.CostPrice = 0.01M;
                saleItem.Quantity = -1;
                ((LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem)(saleItem)).Comment = "Gold Metal Advance Amount";
                price = Convert.ToDecimal((Convert.ToString(decimal.Round(Convert.ToDecimal(retailTransaction.PartnerData.GMATotAdvance), 14, MidpointRounding.AwayFromZero))));
                System.Dynamic.DynamicObject dobj = (System.Dynamic.DynamicObject)retailTransaction.PartnerData;
                return price;
            }

            if (saleItem.ItemId == sGMALGItem)
            {
                saleItem.CostPrice = 0.01M;
                saleItem.Quantity = 1;
                ((LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem)(saleItem)).Comment = "GMA Loss/Gain Amount";
                price = Convert.ToDecimal((Convert.ToString(decimal.Round(Convert.ToDecimal(retailTransaction.PartnerData.GMALossGainTotAmt), 14, MidpointRounding.AwayFromZero))));
                System.Dynamic.DynamicObject dobj = (System.Dynamic.DynamicObject)retailTransaction.PartnerData;
                return price;
            }
            if (saleItem.ItemId == sGMADiscItem)
            {
                saleItem.CostPrice = 0.01M;
                saleItem.Quantity = -1;
                ((LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem)(saleItem)).Comment = "GMA Discount Amount";

                if (Convert.ToBoolean(retailTransaction.PartnerData.APPLYGMADISC) == true)
                {
                    price = Convert.ToDecimal((Convert.ToString(decimal.Round(Convert.ToDecimal(retailTransaction.PartnerData.GMADiscAmt), 14, MidpointRounding.AwayFromZero))));
                    //if (price > 0)
                    //    retailTransaction.PartnerData.APPLYGMADISCDONE = true;
                }

                return price;
            }
            if (saleItem.ItemId == sGMAGSTItem)
            {
                saleItem.CostPrice = 0.01M;
                saleItem.Quantity = -1;
                ((LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem)(saleItem)).Comment = "GMA GST Amount";

                price = Convert.ToDecimal((Convert.ToString(decimal.Round(Convert.ToDecimal(retailTransaction.PartnerData.GMAGSTAmt), 14, MidpointRounding.AwayFromZero))));

                if (Convert.ToBoolean(retailTransaction.PartnerData.APPLYGMADISC) == true)
                {
                    retailTransaction.PartnerData.APPLYGMADISCDONE = true;
                }

                return price;
            }

            #endregion

            // for keyed in and overridden prices the price is just the current price.
            if (saleItem.PriceHasBeenKeyedIn || saleItem.PriceOverridden)
            {
                price = saleItem.Price;
            }
            else
            {
                PriceResult result = GetActiveTradeAgreement(retailTransaction, saleItem, quantity);

                //Basic item price
                if (result.Price == decimal.Zero)
                {
                    decimal basicPrice = GetBasicPrice(saleItem.ItemId);

                    //If the store currency is not the same as the HO currency (which the basic price is in) then convert the price to the store currency
                    if ((basicPrice > 0M) && (retailTransaction.StoreCurrencyCode != ApplicationSettings.Terminal.CompanyCurrency))
                    {
                        basicPrice = this.Application.Services.Currency.CurrencyToCurrency(
                            ApplicationSettings.Terminal.CompanyCurrency,
                            retailTransaction.StoreCurrencyCode, basicPrice);
                    }

                    // Is the current UOM something different than the base UOM for sales
                    if ((saleItem.SalesOrderUnitOfMeasure != saleItem.BackofficeSalesOrderUnitOfMeasure)
                        && (saleItem.UnitQtyConversion.Factor > decimal.Zero))
                    {
                        // then we have to multiply the price with the quantity factor
                        basicPrice = basicPrice * saleItem.UnitQtyConversion.GetFactorForQty(quantity);
                    }
                    result = new PriceResult(basicPrice, PriceGroupIncludesTax.NotSpecified);
                }

                //If the there is a Promotion active then we should calculate if the promotion discount makes the price
                //lower then the current price
                List<PromotionInfo> PromotionLines = FindPromotionPrice(saleItem.ProductId, saleItem.Dimension.DistinctProductVariantId, saleItem.SalesOrderUnitOfMeasure);
                price = CalculatePromotionPrice(PromotionLines, result.Price, saleItem);

                // if unit rounding is active, round at this level (the unit level) before returning
                if (retailTransaction.ApplyDiscountsOnUnitPrices)
                {
                    price = this.application.Services.Rounding.Round(price);
                }

                saleItem.Price = price;
            }

            //Start:Nim
            #region Nimbus
            if (!Convert.ToBoolean(retailTransaction.PartnerData.ItemIsReturnItem))
            {
                if (!Convert.ToBoolean(saleItem.PartnerData.isMRP))
                {
                    if (saleItem.ItemId == AdjustmentItemID())
                    {
                        try
                        {
                            saleItem.CostPrice = 0.01M;
                            saleItem.Quantity = -1;
                            ((LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem)(saleItem)).Comment = "ADVANCE ADJUSTMENT";
                            price = Convert.ToDecimal(saleItem.PartnerData.ServiceItemCashAdjustmentPrice);
                        }
                        catch (RuntimeBinderException)
                        {

                        }
                        return price;
                    }

                    #region Changed By Nimbus
                    // CODE TO BE ADDED FOR THE CUSTOM CALCULATIONS

                    if (!retailTransaction.PartnerData.ItemIsReturnItem)
                    {
                        string sprice = string.Empty;

                        if (((LSRetailPosis.Transaction.PosTransaction)(retailTransaction)).LastRunOperation.ToString().ToUpper().Trim() == "SETQTY")
                        {
                            if (!saleItem.Voided)
                            {
                                CallCustomCalculations(saleItem, retailTransaction, ref sprice);

                                price = Convert.ToDecimal(sprice);
                            }
                            else
                            {
                                saleItem.UnitQuantity = saleItem.Quantity = !string.IsNullOrEmpty(saleItem.PartnerData.Quantity) ? Convert.ToDecimal(saleItem.PartnerData.Quantity) : Convert.ToDecimal(saleItem.Quantity);
                                if (!string.IsNullOrEmpty(Convert.ToString(saleItem.PartnerData.TotalAmount)))
                                {
                                    saleItem.Price = Convert.ToDecimal(saleItem.PartnerData.TotalAmount) / (saleItem.Quantity);
                                    price = saleItem.Price;
                                }
                            }
                        }
                        else
                        {
                            if (!retailTransaction.PartnerData.ItemIsReturnItem)
                            {
                                CallCustomCalculations(saleItem, retailTransaction, ref sprice);

                                price = Convert.ToDecimal(sprice);
                            }
                            else
                            {
                                saleItem.UnitQuantity = saleItem.Quantity = !string.IsNullOrEmpty(saleItem.PartnerData.Quantity) ? Convert.ToDecimal(saleItem.PartnerData.Quantity) : Convert.ToDecimal(saleItem.Quantity);
                                if (!string.IsNullOrEmpty(Convert.ToString(saleItem.PartnerData.TotalAmount)))
                                {
                                    saleItem.Price = Convert.ToDecimal(saleItem.PartnerData.TotalAmount) / (saleItem.Quantity);
                                    price = saleItem.Price;
                                }
                            }
                        }

                    }

                    #endregion
                }
                else if (Convert.ToDecimal(saleItem.PartnerData.Quantity) > 0)
                {
                    saleItem.UnitQuantity = saleItem.Quantity = !string.IsNullOrEmpty(saleItem.PartnerData.Quantity) ? Convert.ToDecimal(saleItem.PartnerData.Quantity) : Convert.ToDecimal(saleItem.Quantity);
                    if (!string.IsNullOrEmpty(Convert.ToString(saleItem.PartnerData.TotalAmount)))
                    {
                        saleItem.Price = Convert.ToDecimal(saleItem.PartnerData.TotalAmount) / (saleItem.Quantity);
                        price = saleItem.Price;
                    }
                }
            }
            #endregion
            //End:Nim

            return price;
        }

        private decimal GetGssDiscountPriceFromAgreement(RetailTransaction retailTransaction, decimal price)
        {
            DataTable tblProduct = new DataTable("Product");
            DataTable dtProductTypeAndMRPWiseGroup = new DataTable();

            #region APPLYGSSDISC
            if (retailTransaction.PartnerData.APPLYGSSDISC == true)
            {
                tblProduct.Columns.Add("ItemId", typeof(string));
                tblProduct.Columns.Add("ProductType", typeof(string));
                tblProduct.Columns.Add("MRP", typeof(bool));
                tblProduct.Columns.Add("Qty", typeof(decimal));
                tblProduct.Columns.Add("Amt", typeof(decimal));
            }

            foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
            {
                if (Convert.ToInt16(saleLineItem.ItemType) != 2)
                {
                    if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)))
                    {
                        if (!saleLineItem.Voided)
                        {
                            if (saleLineItem.PartnerData.TransactionType == Convert.ToString((int)TransactionType.Sale))
                            {
                                string sProductType = GetItemProductType(saleLineItem.ItemId);
                                bool bMRP = Convert.ToBoolean(saleLineItem.PartnerData.isMRP);
                                decimal dQty = Convert.ToDecimal(saleLineItem.PartnerData.Quantity);
                                decimal dAmt = Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) - saleLineItem.LineDiscount;

                                tblProduct.Rows.Add(saleLineItem.ItemId, sProductType, bMRP, dQty, dAmt);
                            }
                        }
                    }
                    else
                    {
                        if (!saleLineItem.Voided)
                        {
                            if (saleLineItem.PartnerData.TransactionType == Convert.ToString((int)TransactionType.Sale))
                            {
                                string sProductType = GetItemProductType(saleLineItem.ItemId);
                                bool bMRP = Convert.ToBoolean(saleLineItem.PartnerData.isMRP);
                                decimal dQty = Convert.ToDecimal(saleLineItem.PartnerData.Quantity);
                                decimal dAmt = Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) - saleLineItem.LineDiscount;

                                tblProduct.Rows.Add(saleLineItem.ItemId, sProductType, bMRP, dQty, dAmt);
                            }
                        }
                    }
                }
            }

            if (tblProduct.Rows.Count > 0 && tblProduct != null)
            {
                var query =
                       tblProduct.AsEnumerable()
                       .Select(x =>
                           new
                           {
                               ProductType = x["ProductType"],
                               MRP = x["MRP"],
                               Amt = x["Amt"],
                               Qty = x["Qty"],
                           }
                        )
                        .GroupBy(s => new { s.ProductType, s.MRP })
                        .Select(groupedTable =>
                               new
                               {
                                   Category = groupedTable.Key.ProductType,
                                   Article = groupedTable.Key.MRP,
                                   Amt = groupedTable.Sum(x => Math.Round(Convert.ToDecimal(x.Amt), 2)),
                                   Qty = groupedTable.Sum(x => Math.Round(Convert.ToDecimal(x.Qty), 3)),
                               }
                        );

                dtProductTypeAndMRPWiseGroup = ConvertToDataTable(query);
            }

            decimal minAmount = decimal.MaxValue;
            decimal maxAmount = decimal.MinValue;
            string sProduct = "";
            bool isMrp = false;
            string sSchemeCode = retailTransaction.PartnerData.GSSSchemeCode;

            foreach (DataRow dr in dtProductTypeAndMRPWiseGroup.Rows)
            {
                decimal accountLevel = dr.Field<decimal>("Amt");
                minAmount = Math.Min(minAmount, accountLevel);
                maxAmount = Math.Max(maxAmount, accountLevel);
            }
            DataRow[] result = dtProductTypeAndMRPWiseGroup.Select("Amt = " + maxAmount + "");
            foreach (DataRow row in result)
            {
                sProduct = Convert.ToString(row[0]);
                isMrp = Convert.ToBoolean(row[1]);
            }

            decimal dDiscPrc = GetGSSDiscountAgreement(sProduct, sSchemeCode, Convert.ToInt16(isMrp));

            if (dDiscPrc > 0 && Convert.ToDecimal(retailTransaction.PartnerData.GSSRoyaltyAmt) > 0)
                price = (Convert.ToDecimal(retailTransaction.PartnerData.GSSRoyaltyAmt) * dDiscPrc) / 100;
            else
                price = 0;

            #endregion
            return price;
        }

        //Start:Nim
        #region Nimbus
        private string AdjustmentItemID()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) ADJUSTMENTITEMID FROM [RETAILPARAMETERS] ");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }
        private void GSSAdjustmentItemID(ref string sGSSAdjItemId, ref string sGSSDiscItemId,
            ref string sCRWREPAIRADJITEMFOROG, ref string sCRWREPAIRADJITEM,
            ref string sGSSAmountAdjItemId, ref string sGSSAmountDiscItemId)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT GSSADJUSTMENTITEMID,GSSDISCOUNTITEMID,CRWREPAIRADJITEMFOROG,CRWREPAIRADJITEM,GSSAMOUNTADJUSTMENTITEMID,GSSAMOUNTDISCOUNTITEMID FROM [RETAILPARAMETERS] WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "' ");
            DataTable dtGSS = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataAdapter daGss = new SqlDataAdapter(cmd);
            daGss.Fill(dtGSS);

            if (dtGSS != null && dtGSS.Rows.Count > 0)
            {
                sGSSAdjItemId = Convert.ToString(dtGSS.Rows[0]["GSSADJUSTMENTITEMID"]);
                sGSSDiscItemId = Convert.ToString(dtGSS.Rows[0]["GSSDISCOUNTITEMID"]);
                sCRWREPAIRADJITEMFOROG = Convert.ToString(dtGSS.Rows[0]["CRWREPAIRADJITEMFOROG"]);
                sCRWREPAIRADJITEM = Convert.ToString(dtGSS.Rows[0]["CRWREPAIRADJITEM"]);
                sGSSAmountAdjItemId = Convert.ToString(dtGSS.Rows[0]["GSSAMOUNTADJUSTMENTITEMID"]);
                sGSSAmountDiscItemId = Convert.ToString(dtGSS.Rows[0]["GSSAMOUNTDISCOUNTITEMID"]);
            }
        }

        private void GMAAdjustmentItemID(ref string sCRWGMAADVSERVITEM, ref string sCRWGSTSERVITEM,
        ref string sCRWLOSSGAINSERVITEM, ref string sCRWDISCSERVICEITEM)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT  CRWGMAADVSERVITEM,CRWGSTSERVITEM,CRWLOSSGAINSERVITEM,CRWDISCSERVICEITEM  FROM [RETAILPARAMETERS] WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "' ");
            DataTable dtGSS = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataAdapter daGss = new SqlDataAdapter(cmd);
            daGss.Fill(dtGSS);

            if (dtGSS != null && dtGSS.Rows.Count > 0)
            {
                sCRWGMAADVSERVITEM = Convert.ToString(dtGSS.Rows[0]["CRWGMAADVSERVITEM"]);
                sCRWGSTSERVITEM = Convert.ToString(dtGSS.Rows[0]["CRWGSTSERVITEM"]);
                sCRWLOSSGAINSERVITEM = Convert.ToString(dtGSS.Rows[0]["CRWLOSSGAINSERVITEM"]);
                sCRWDISCSERVICEITEM = Convert.ToString(dtGSS.Rows[0]["CRWDISCSERVICEITEM"]);
            }

        }

        private string GetItemProductType(string sItemId)
        {
            string sCName = string.Empty;
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            if (conn.State == ConnectionState.Open)
                conn.Close();

            string sQry = "select isnull(PRODUCTTYPECODE,'') from INVENTTABLE WHERE ITEMID = '" + sItemId.Trim() + "'";

            SqlCommand cmd = new SqlCommand(sQry, conn);
            cmd.CommandTimeout = 0;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            sCName = Convert.ToString(cmd.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sCName;

        }

        private void CallCustomCalculations(SaleLineItem saleLineItem, RetailTransaction retailTransaction, ref string price)
        {
            #region Changed By NIMBUS

            if (saleLineItem != null)
            {
                #region - For Setting Price and Quantity and Other Details

                if (saleLineItem.PartnerData.CustNo != retailTransaction.Customer.CustomerId)
                {
                    saleLineItem.PartnerData.OrderNum = string.Empty;
                    saleLineItem.PartnerData.OrderLineNum = string.Empty;
                    saleLineItem.PartnerData.CustNo = string.Empty;
                }

                if (saleLineItem.QuantityOrdered < 0)
                {
                    #region Return Sale Item

                    // string sRateType = saleLineItem.PartnerData.RateType;

                    saleLineItem.UnitQuantity = saleLineItem.Quantity = !string.IsNullOrEmpty(saleLineItem.PartnerData.Quantity) ? -1 * Convert.ToDecimal(saleLineItem.PartnerData.Quantity) : Convert.ToDecimal(saleLineItem.Quantity);

                    if (!string.IsNullOrEmpty(saleLineItem.PartnerData.TotalAmount))
                    {
                        saleLineItem.Price = Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) / -(saleLineItem.Quantity);
                    }
                    #endregion
                }
                else
                {
                    #region Sale Item
                    saleLineItem.UnitQuantity = saleLineItem.Quantity = !string.IsNullOrEmpty(saleLineItem.PartnerData.Quantity) ? Convert.ToDecimal(saleLineItem.PartnerData.Quantity) : Convert.ToDecimal(saleLineItem.Quantity);
                    bool bIsFreeGiftItem = true;

                    if (retailTransaction != null)
                    {
                        if (!string.IsNullOrEmpty(retailTransaction.PartnerData.FreeGiftCWQTY))
                        {
                            saleLineItem.PartnerData.TotalAmount = "0";
                            saleLineItem.PartnerData.Amount = "0";
                            saleLineItem.PartnerData.ACTTOTAMT = "0";

                            saleLineItem.PartnerData.Rate = "0";
                            saleLineItem.PartnerData.MakingRate = "0";
                            saleLineItem.PartnerData.MakingAmount = "0";
                            saleLineItem.PartnerData.ACTMKRATE = "0";
                            saleLineItem.PartnerData.LINEGOLDTAX = "0";
                            saleLineItem.PartnerData.LINEGOLDVALUE = "0";
                            saleLineItem.PartnerData.MakingDisc = "0";

                            if (!string.IsNullOrEmpty(saleLineItem.PartnerData.Ingredients))
                            {
                                DataSet dsIngredients = new DataSet();
                                StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                dsIngredients.ReadXml(reader);
                                foreach (DataRow drIng in dsIngredients.Tables[0].Rows)
                                {
                                    drIng["Rate"] = "0";
                                    drIng["CValue"] = "0";

                                    dsIngredients.Tables[0].AcceptChanges();
                                }

                                MemoryStream mstr = new MemoryStream();
                                dsIngredients.Tables[0].WriteXml(mstr, true);
                                mstr.Seek(0, SeekOrigin.Begin);
                                StreamReader sr = new StreamReader(mstr);
                                string sXML = string.Empty;
                                sXML = sr.ReadToEnd();
                                saleLineItem.PartnerData.Ingredients = sXML;

                                bIsFreeGiftItem = true;
                            }
                        }
                        else
                            bIsFreeGiftItem = false;
                    }
                    if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.TotalAmount)))
                    {
                        if (bIsFreeGiftItem == false)
                            saleLineItem.Price = Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) / (saleLineItem.Quantity);
                        else
                            saleLineItem.Price = 0;
                    }

                    #endregion
                }

                price = Convert.ToString(saleLineItem.Price);
                #endregion
            }
            #endregion
        }

        private void updateOrderDelivery(string orderNum, string orderLineNum, bool voided, string itemid = "")
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            string commandText = string.Empty;

            if (!voided)
            {
                commandText = " UPDATE CUSTORDER_DETAILS SET isDelivered = 1 WHERE ORDERNUM='" + orderNum.Trim() + "' AND  LINENUM='" + orderLineNum.Trim() + "' " +
                              " DECLARE @COUNT INT " +
                              " SELECT @COUNT=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM='" + orderNum.Trim() + "' AND isDelivered = 0  " +
                              " IF(@COUNT=0) " +
                              " BEGIN " +
                              " UPDATE CUSTORDER_HEADER SET isDelivered = 1 WHERE ORDERNUM='" + orderNum.Trim() + "'  " +
                              " END ";
            }
            else
            {
                commandText = " UPDATE CUSTORDER_DETAILS SET isDelivered = 0 WHERE ORDERNUM='" + orderNum.Trim() + "' AND  LINENUM='" + orderLineNum.Trim() + "' " +
                             " DECLARE @COUNT INT " +
                             " SELECT @COUNT=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM='" + orderNum.Trim() + "' AND isDelivered = 0  " +
                             " IF(@COUNT>0) " +
                             " BEGIN " +
                             " UPDATE CUSTORDER_HEADER SET isDelivered = 0 WHERE ORDERNUM='" + orderNum.Trim() + "'  " +
                             " END ";
            }
            commandText = commandText + " UPDATE SKUTable_Posted SET isLocked='False' " +
                                        " WHERE SkuNumber='" + itemid + "' " +
                                        " AND STOREID='" + ApplicationSettings.Terminal.StoreId + "' " +
                                        " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' ";
            commandText = commandText + " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            command.ExecuteNonQuery();

        }
        #endregion
        //End:Nim

        internal PriceResult GetActiveTradeAgreement(RetailTransaction retailTransaction, SaleLineItem saleItem, Decimal quantity)
        {
            PriceResult result;

            // Get basic arguments for Price evaluation
            RetailPriceArgs args = new RetailPriceArgs()
            {
                Barcode = saleItem.BarcodeId,
                CurrencyCode = retailTransaction.StoreCurrencyCode, // store currency
                CustomerId = retailTransaction.Customer.CustomerId,
                Dimensions = saleItem.Dimension,
                InventUOM = saleItem.BackofficeSalesOrderUnitOfMeasure,
                ItemId = saleItem.ItemId,
                PriceGroups = this.storePriceGroups,
                Quantity = quantity,
                SalesUOM = saleItem.SalesOrderUnitOfMeasure,
                UnitQtyConversion = saleItem.UnitQtyConversion,
                SerialId = saleItem.SerialId
            };

            //Get the active retail price - checks following prices brackets in order: Customer TAs, Store price group TAs, 'All' TAs.
            // First bracket to return a price 'wins'. Each bracket returns the lowest price it can find.
            result = GetRetailPrice(args);

            //Direct customer TA price would have been caught above. 
            // Compare against customer price group TAs now and override if lower than previously found price (or if previously found price was 0).
            if (!string.IsNullOrEmpty(retailTransaction.Customer.CustomerId)
                && !string.IsNullOrEmpty(retailTransaction.Customer.PriceGroup)
                && !this.storePriceGroups.Contains(retailTransaction.Customer.PriceGroup))
            {
                //Customer price group
                args.PriceGroups = new ReadOnlyCollection<string>(new[] { retailTransaction.Customer.PriceGroup });
                PriceResult customerResult = FindPriceAgreement(args);

                //If the store currency is not the same as the HO currency then check if there is a price for the HO currency in the trade agreements
                if ((customerResult.Price == 0) && (retailTransaction.StoreCurrencyCode != ApplicationSettings.Terminal.CompanyCurrency))
                {
                    // convert the TA from company currency to store currency
                    args.CurrencyCode = ApplicationSettings.Terminal.CompanyCurrency;
                    customerResult = FindPriceAgreement(args);
                    customerResult = new PriceResult(customerResult, ApplicationSettings.Terminal.CompanyCurrency, retailTransaction.StoreCurrencyCode);
                }

                // Pick the Customer price if either the Retail price is ZERO, or the Customer Price is non-zero AND lower
                if ((result.Price == decimal.Zero)
                    || ((customerResult.Price > decimal.Zero) && (customerResult.Price <= result.Price)))
                {
                    result = customerResult;
                }
            }

            return result;
        }

        /// <summary>
        /// Given a set of promotion lines, tentative item price, and item, calculate the price after promotions are applied
        /// </summary>
        /// <param name="promotionLines">List of promotion configurations active for this item</param>
        /// <param name="price">Price of the item before the promotion, derived from trade agreement or base item price</param>
        /// <param name="saleItem">The sale item whose price is being determined</param>
        /// <returns>Unrounded price after applying all promotions</returns>
        private decimal CalculatePromotionPrice(List<PromotionInfo> promotionLines, decimal price, BaseSaleItem saleItem)
        {
            try
            {
                decimal promoPrice = PromotionPricing.CalculatePromotionPrice(promotionLines, price, saleItem);
                return promoPrice;
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Returns price of the item as per given item id.
        /// </summary>
        /// <param name="itemId"></param>
        /// <param name="unitOfMeasure"></param>
        /// <returns></returns>
        public decimal GetItemPrice(string itemId, string unitOfMeasure)
        {
            IRetailTransaction rt = this.Application.BusinessLogic.Utility.CreateRetailTransaction(
                ApplicationSettings.Terminal.StoreId,
                ApplicationSettings.Terminal.StoreCurrency,
                ApplicationSettings.Terminal.TaxIncludedInPrice,
                this.Application.Services.Rounding);
            SaleLineItem saleLine = (SaleLineItem)this.Application.BusinessLogic.Utility.CreateSaleLineItem(
                ApplicationSettings.Terminal.StoreCurrency, this.Application.Services.Rounding, rt);
            saleLine.ItemId = itemId;
            decimal price;
            PriceAgreementArgs args;

            // 1. Look for a TA price in the StoreCurrency
            args = new PriceAgreementArgs()
            {
                CurrencyCode = ApplicationSettings.Terminal.StoreCurrency,
                CustomerId = string.Empty,
                Dimensions = saleLine.Dimension,
                ItemId = itemId,
                PriceGroups = this.storePriceGroups,
                Quantity = 1,
                UnitOfMeasure = unitOfMeasure
            };
            PriceResult result = TradeAgreementPricing.priceAgr(args, this.salesPriceParameters);
            price = result.Price;

            // 2. If we didn't find a TA in the StoreCurrency, try to find one in the CompanyCurrency
            if ((price == decimal.Zero)
                && (rt.StoreCurrencyCode != ApplicationSettings.Terminal.CompanyCurrency))
            {
                args = new PriceAgreementArgs()
                {
                    CurrencyCode = ApplicationSettings.Terminal.CompanyCurrency,
                    CustomerId = string.Empty,
                    Dimensions = saleLine.Dimension,
                    ItemId = itemId,
                    PriceGroups = this.storePriceGroups,
                    Quantity = 1,
                    UnitOfMeasure = unitOfMeasure
                };
                result = TradeAgreementPricing.priceAgr(args, this.salesPriceParameters);
                price = this.Application.Services.Currency.CurrencyToCurrency(ApplicationSettings.Terminal.CompanyCurrency, rt.StoreCurrencyCode, result.Price);
            }

            // 3. If we STILL don't have a TA price, then fall back to the BasicPrice
            if (price == decimal.Zero)
            {
                NetTracer.Information("Price::GetItemPrice: Price is zero for itemId {0} unitOfMeasure {1}, falling back to the BasicPrice.", itemId, unitOfMeasure);
                price = GetBasicPrice(saleLine.ItemId);
                //If the store currency is not the same as the Company currency (which the basic price is in) then convert the price to the store currency
                if (rt.StoreCurrencyCode != ApplicationSettings.Terminal.CompanyCurrency)
                {
                    price = this.Application.Services.Currency.CurrencyToCurrency(ApplicationSettings.Terminal.CompanyCurrency, rt.StoreCurrencyCode, price);
                }
            }
            return price;
        }

        public DataTable ConvertToDataTable<T>(IEnumerable<T> varlist)
        {
            DataTable dtReturn = new DataTable();
            // column names
            PropertyInfo[] oProps = null;

            if (varlist == null) return dtReturn;
            foreach (T rec in varlist)
            {
                // Use reflection to get property names, to create table, Only first time, others will follow
                if (oProps == null)
                {
                    oProps = ((Type)rec.GetType()).GetProperties();
                    foreach (PropertyInfo pi in oProps)
                    {
                        Type colType = pi.PropertyType;

                        if ((colType.IsGenericType) && (colType.GetGenericTypeDefinition() == typeof(Nullable<>)))
                        {
                            colType = colType.GetGenericArguments()[0];
                        }
                        dtReturn.Columns.Add(new DataColumn(pi.Name, colType));
                    }
                }
                DataRow dr = dtReturn.NewRow();
                foreach (PropertyInfo pi in oProps)
                {
                    dr[pi.Name] = pi.GetValue(rec, null) == null ? DBNull.Value : pi.GetValue
                    (rec, null);
                }
                dtReturn.Rows.Add(dr);
            }
            return dtReturn;
        }

        private decimal GetGSSDiscountAgreement(string sProductType, string sSchemeCode, int bMRP)
        {
            decimal dResult = 0;

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " IF EXISTS ( SELECT  TOP (1) ACTIVE" +
                " FROM   CRWGSSDISCOUNTAGREEMENT  A" +
                " WHERE (A.PRODUCTTYPE='" + sProductType + "') AND A.SCHEMECODE='" + sSchemeCode + "' AND A.MRP=" + bMRP + "" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN A.FROMDATE AND A.TODATE))" +
                     " BEGIN " +
                     "     SELECT  TOP (1) CAST(A.DISCOUNT AS decimal(18, 2))" +
                     "       FROM   CRWGSSDISCOUNTAGREEMENT  A  WHERE     " +
                     "     (A.PRODUCTTYPE='" + sProductType + "') AND A.SCHEMECODE='" + sSchemeCode + "' AND A.MRP=" + bMRP + "" +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN A.FROMDATE AND A.TODATE)   " +
                     "       AND A.ACTIVE = 1" +
                     " ORDER BY A.PRODUCTTYPE DESC END";

            SqlCommand commandMk = new SqlCommand(commandText.ToString(), connection);
            commandMk.CommandTimeout = 0;

            using (SqlDataReader mkDiscRd = commandMk.ExecuteReader())
            {
                while (mkDiscRd.Read())
                {
                    dResult = Convert.ToDecimal(mkDiscRd.GetValue(0));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();


            return dResult;
        }
        #endregion
    }
}
