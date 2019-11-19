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
using System.Text;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.DataAccess.DataUtil;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.Discount;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.MemoryTables;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using CustomerDiscountTypes = LSRetailPosis.Transaction.Line.Discount.CustomerDiscountItem.CustomerDiscountTypes;
using DE = Microsoft.Dynamics.Retail.Pos.DataEntity;
using DiscountTypes = LSRetailPosis.Transaction.Line.Discount.LineDiscountItem.DiscountTypes;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using PeriodicDiscOfferType = LSRetailPosis.Transaction.Line.Discount.PeriodicDiscountItem.PeriodicDiscOfferType;
using PeriodStatus = LSRetailPosis.Transaction.MemoryTables.Period.PeriodStatus;
using System.IO;


namespace Microsoft.Dynamics.Retail.Pos.DiscountService
{
    [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Revisit this class for refactoring")]
    [Export(typeof(IDiscount))]
    public class Discount : IDiscount, IInitializeable
    {
        TaxService.Tax objTax = new TaxService.Tax();
        private IUtility utiliy;

        #region Structs and enums

        private const string SelectFromPeriodicDiscount = @"
                    SELECT
                        pd.OFFERID,
                        pd.NAME,
                        pd.PERIODICDISCOUNTTYPE,
                        pd.CONCURRENCYMODE,
                        pd.CURRENCYCODE,
                        pd.ISDISCOUNTCODEREQUIRED,                        
                        pd.VALIDATIONPERIODID,
                        pd.DATEVALIDATIONTYPE,
                        pd.VALIDFROM,
                        pd.VALIDTO,
                        pd.DISCOUNTTYPE,
                        pd.NOOFLINESTOTRIGGER,
                        pd.DEALPRICEVALUE,
                        pd.DISCOUNTPERCENTVALUE,
                        pd.DISCOUNTAMOUNTVALUE,
                        pd.NOOFLEASTEXPENSIVELINES,
                        pd.NUMBEROFTIMESAPPLICABLE,
                        pd.LINENUM,
                        pd.DISCOUNTPERCENTORVALUE,    
                        pd.LOYALTYCARDREQUIRED_RU,                            
                        pd.LOYALTYSCHEMEID_RU,
                        pd.LOYALTYPOINTSMIN_RU,
                        pd.LOYALTYPOINTSMAX_RU,
                        rgl.CATEGORY,
                        ISNULL(mmol.LINEGROUP,'') AS LINEGROUP,
                        ISNULL(mmol.DISCOUNTTYPE,'') AS MIXANDMATCHDISCOUNTTYPE, 
                        ISNULL(mmol.NUMBEROFITEMSNEEDED,'') AS NUMBEROFITEMSNEEDED,    
                        ISNULL(dol.DISCOUNTMETHOD,0) AS DISCOUNTMETHOD,
                        ISNULL(dol.DISCAMOUNT,0) AS DISCAMOUNT, 
                        ISNULL(dol.DISCPCT, 0) AS DISCPCT, 
                        ISNULL(dol.OFFERPRICE, 0) AS OFFERPRICE, 
                        ISNULL(dol.OFFERPRICEINCLTAX, 0) AS OFFERPRICEINCLTAX,
    
                        ISNULL(uom.SYMBOL,'') AS SYMBOL,
                        rgl.VARIANT
                    FROM RETAILPERIODICDISCOUNTSFLATTENED pd ";

        private const string WherePeriodicDiscountAndLeftJoins = @"
                    LEFT JOIN UNITOFMEASURE uom ON uom.RECID = pd.UNITOFMEASURE
                    LEFT JOIN RETAILDISCOUNTLINEMIXANDMATCH mmol ON pd.DISCOUNTLINEID = mmol.RECID
                    LEFT JOIN RETAILDISCOUNTLINEOFFER dol ON pd.DISCOUNTLINEID = dol.RECID
                    WHERE (pd.STATUS = 1)
                    AND (pd.PERIODICDISCOUNTTYPE != 3) -- Don't fetch promotions
                    AND (pd.CURRENCYCODE IN (@STORECURRENCY, ''))
                    AND pd.PRICEDISCGROUP IN (SELECT spg.PRICEGROUPID FROM @STOREPRICEGROUPS spg)
                    AND ((pd.VALIDFROM <= @TODAY OR pd.VALIDFROM <= @NODATE)
                                AND (pd.VALIDTO >= @TODAY OR pd.VALIDTO <= @NODATE))";

        private const string SqlUnionAll = " UNION ALL ";

        // Sentinal value for 'no date specified' Duplicated throughout software
        private readonly DateTime NoDate = new DateTime(1900, 1, 1);

        // See Price.cs for duplicate definition.  Clean up if refactoring common code between services ever occurs
        private enum DateValidationTypes
        {
            Advanced = 0,
            Standard = 1
        }

        #region enum MetalType
        public enum MetalType
        {
            Other = 0,
            Gold = 1,
            Silver = 2,
            Platinum = 3,
            Alloy = 4,
            Diamond = 5,
            Pearl = 6,
            Stone = 7,
            Consumables = 8,
            Watch = 11,
            LooseDmd = 12,
            Palladium = 13,
            Jewellery = 14,
            Metal = 15,
            PackingMaterial = 16,
            Certificate = 17,
            GiftVoucher = 18,
        }
        #endregion

        enum DiscPromotionType
        {
            //Making = 0,
            //LineAmount = 1,
            //MakingAndStoneValue = 2,
            Making = 0,
            LineAmount = 1,
            MakingAndStoneValue = 2,
            Stone = 3,
            Diamond = 4,
            Making_Stone = 5,
            Making_Diamond = 6,
            Making_Stone_Diamond_Both = 7,
        }
        enum DiscType
        {
            Pct = 0,
            Amt = 1,
        }

        enum WarehouseType
        {
            Single = 0,
            All = 1,
        }
        enum RangeType
        {
            Weight = 0,
            LineAmount = 1,
        }

        #region enum CRWRetailDiscPermission Nimbus
        private enum CRWRetailDiscPermission // added on 29/08/2014
        {
            Cashier = 0,
            Salesperson = 1,
            Manager = 2,
            Other = 3,
            Manager2 = 4,
        }

        enum MRPOrMkDiscType
        {
            None = 0,
            Making = 1,
            MRP = 2,
        }

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

        string sPromoCode = string.Empty; // add on 19/11/2014
        string sCustomerId = string.Empty;
        bool bIsSpecialDisc = false;
        int iIsDiscFlat = 0;

        #endregion


        #endregion

        #region Member variables

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

        private IUtility Utility
        {
            get
            {
                return this.Application.BusinessLogic.Utility;
            }
        }

        // Internal Cache
        private List<PriceDiscData> priceDiscDataCache;
        private string priceDiscDataCacheToken; // Token to indicate cache source (maps to transcationId)

        private DataTable activeOffers = new DataTable("ACTIVEPERIODICOFFERS");
        private DataTable activeOfferLines = new DataTable("ACTIVEPERIODICOFFERLINES");
        private DataTable tmpMMOffer = new DataTable("TMPMMOFFER");
        private DataTable tmpDiscountCode = new DataTable("TMPDISCOUNTCODE");

        private DM.DiscountDataManager discountDataManager;
        private DiscountParameters DiscountParameters { get; set; }

        private ReadOnlyCollection<Int64> storePriceGroups = new List<Int64>().AsReadOnly();

        int iPromoDisctype = 0;
        int iCalcDiscType = 0;
        decimal dDiscAmt = 0;
        //==========Soutik==================
        decimal dMakDiscAmt = 0;
        decimal dStnDiscAmt = 0;
        decimal dDiaDiscAmt = 0;

        #endregion

        #region IInitializeable Members

        public void Initialize(DM.DiscountDataManager dataManagerForDiscounts, DiscountParameters discountParametersActive)
        {
            if (dataManagerForDiscounts == null)
            {
                NetTracer.Warning("dataManagerForDiscounts parameter is null");
                throw new ArgumentNullException("dataManagerForDiscounts");
            }

            if (discountParametersActive == null)
            {
                NetTracer.Warning("discountParametersActive parameter is null");
                throw new ArgumentNullException("discountParametersActive");
            }

            priceDiscDataCache = new List<PriceDiscData>();

            this.DiscountParameters = discountParametersActive;
            this.discountDataManager = dataManagerForDiscounts;
            this.storePriceGroups = this.discountDataManager.PriceGroupIdsFromStore(ApplicationSettings.Terminal.StorePrimaryId);

            if (ApplicationSettings.LogTraceLevel == LogTraceLevel.Trace)
            {
                StringBuilder priceGroupString =
                    this.storePriceGroups.Aggregate(new StringBuilder(), (pgString, pgId) => pgString.AppendFormat(" '{0}'", pgId.ToString()));
                LSRetailPosis.ApplicationLog.Log("Discount.Initialize()",
                    String.Format("Initializing discount. Store '{0}' belongs to price groups:{1}.",
                    ApplicationSettings.Terminal.StorePrimaryId, priceGroupString.ToString()),
                    LogTraceLevel.Trace);
            }

            MakeActiveOfferTables();        //Tables for periodic discount calculation
            MakeTmpOfferTable();            //Temporary table used to for mix match calculation
            MakeTmpDiscountCodeTable();
        }

        public void Initialize()
        {
            DiscountParameters parameters = new DiscountParameters();
            parameters.InitializeParameters();
            Initialize(
                new DM.DiscountDataManager(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID),
                parameters);
        }

        public void Uninitialize()
        {
            activeOffers.Dispose();
            activeOfferLines.Dispose();
            tmpMMOffer.Dispose();
            tmpDiscountCode.Dispose();
        }

        #endregion

        private static void CalculateTotalDiscount(IRetailTransaction rt)
        {
            rt.ClearTotalDiscountLines();

            if (rt.TotalManualPctDiscount != 0)
            {
                rt.AddTotalDiscPctLines();
            }
            if (rt.TotalManualDiscountAmount != 0)
            {
                rt.AddTotalDiscAmountLines(typeof(TotalDiscountItem), rt.TotalManualDiscountAmount);
            }
        }

        private static void CalculateLoyaltyDiscount(IRetailTransaction rt)
        {
            rt.ClearLoyaltyDiscountLines();

            if (rt.LoyaltyManualDiscountAmount.HasValue)
            {
                rt.AddLoyaltyDiscAmountLines(rt.LoyaltyManualDiscountAmount.Value);
            }
        }

        /// <summary>
        /// Calculates all of the discounts for the transactions.
        /// </summary>
        /// <param name="retailTransaction"></param>
        public void CalculateDiscount(IRetailTransaction retailTransaction)
        {
            RetailTransaction transaction = retailTransaction as RetailTransaction;
            if (transaction == null)
            {
                NetTracer.Error("retailTransaction parameter is null");
                throw new ArgumentNullException("retailTransaction");
            }
            #region Base
            if (transaction.PartnerData.ReCalculate == false
                && transaction.PartnerData.SKUAgingDisc == false
                && transaction.PartnerData.TierDisc == false
                && transaction.PartnerData.GENERALDISC == false
                && transaction.PartnerData.MakStnDiaDisc == false
                && transaction.PartnerData.AdvAgreemetDisc == false)
            {
                ICustomerOrderTransaction orderTransaction = retailTransaction as ICustomerOrderTransaction;
                bool priceLock = (orderTransaction == null) ? false : (orderTransaction.LockPrices);

                // if prices aren't locked on transaction, compute automatic discounts
                if (!priceLock)
                {
                    CalcPeriodicDisc(transaction);  //Calculation of periodic offers

                    if (!string.IsNullOrEmpty(transaction.Customer.CustomerId))
                    {
                        CalcCustomerDiscount(transaction); //Calculation of customer discount
                    }
                }
            }

            // this is manual total discount, it should always be calculated
            CalculateTotalDiscount(retailTransaction);

            CalculateLoyaltyDiscount(retailTransaction);
            #endregion

            // START: ADDED ON 30/08/2014 NIMBUS
            #region Flat Auto disc
            if (transaction != null && transaction.SaleItems != null && transaction.SaleItems.Count > 0
                && transaction.PartnerData.IsGiftCardIssue == "0"
                && transaction.PartnerData.APPLYGSSDISCDONE == false)
            {
                SaleLineItem saleItem = transaction.SaleItems.Last.Value;
                foreach (SaleLineItem saleLineItem in transaction.SaleItems)
                {
                    if (!saleLineItem.Voided && Convert.ToInt16(saleLineItem.ItemType) != 2)// && saleLineItem.ItemType != LSRetailPosis.Transaction.Line.GiftCertificateItem.GiftCertificateItem.ItemTypes.Item
                    {
                        if (saleLineItem.PartnerData.TransactionType == Convert.ToString((int)TransactionType.Sale) && saleLineItem.ReturnLineId == 0)
                        {
                            #region saleLineItem.LineId == 1
                            if (saleLineItem.LineId == 1 && saleLineItem.LinePctDiscount == 0)
                            {
                                //if(saleItem.DiscountLines.Count == 0)
                                //{
                                decimal dinitDiscValue = 0;
                                if (IsRetailItem(saleLineItem.ItemId))
                                {
                                    if (saleLineItem.PartnerData.isMRP)
                                        dinitDiscValue = GetDiscountFromDiscPolicy(saleLineItem.ItemId, Convert.ToDecimal(saleLineItem.Price), "OPENINGDISCPCT", 1);// get OPENINGDISCPCT field value FOR THE OPENING
                                    else
                                        dinitDiscValue = GetMkDiscountFromDiscPolicy(saleLineItem.ItemId, saleLineItem.Quantity, "OPENINGDISCPCT");

                                    if (Convert.ToInt16(saleLineItem.PartnerData.FLAT) == 1 && Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) > 0)
                                    {
                                        decimal dMkDisc = 0m;

                                        dMkDisc = ((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) * dinitDiscValue) / 100);

                                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                            dinitDiscValue = (dMkDisc * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                        iIsDiscFlat = 1;
                                    }
                                    //else
                                    //    dinitDiscValue = 0;// REQ BY Supapta Boral on 10/06/19

                                    LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                    discountItem.Percentage = dinitDiscValue;
                                    discountItem.LineDiscountType = DiscountTypes.Customer;
                                    saleLineItem.Add(discountItem);
                                    //}
                                    if (dinitDiscValue > 0)
                                    {
                                        saleLineItem.PartnerData.PROMOCODE = sPromoCode;
                                        saleLineItem.PartnerData.FLAT = iIsDiscFlat;
                                        saleLineItem.PartnerData.OpeningDisc = dinitDiscValue;
                                        if (saleItem.PartnerData.isMRP)
                                            saleLineItem.PartnerData.OpeningDiscType = (int)MRPOrMkDiscType.MRP;
                                        else
                                            saleLineItem.PartnerData.OpeningDiscType = (int)MRPOrMkDiscType.Making;
                                    }
                                    else
                                    {
                                        saleLineItem.PartnerData.PROMOCODE = "";
                                        saleLineItem.PartnerData.FLAT = "0";
                                        saleLineItem.PartnerData.OpeningDisc = 0;
                                        saleLineItem.PartnerData.OpeningDiscType = 0;
                                    }
                                }
                            }
                            #endregion

                            #region saleLineItem.LineId != 1
                            if (saleLineItem.ItemId == saleItem.ItemId && saleItem.LinePctDiscount == 0 && saleLineItem.LineId != 1 && !saleLineItem.Voided)
                            {
                                //if(saleItem.DiscountLines.Count == 0)
                                //{
                                decimal dinitDiscValue = 0;
                                if (IsRetailItem(saleLineItem.ItemId))
                                {
                                    if (saleItem.PartnerData.isMRP)
                                        dinitDiscValue = GetDiscountFromDiscPolicy(saleItem.ItemId, Convert.ToDecimal(saleItem.Price), "OPENINGDISCPCT", 1);// get OPENINGDISCPCT field value FOR THE OPENING
                                    else
                                        dinitDiscValue = GetMkDiscountFromDiscPolicy(saleLineItem.ItemId, saleLineItem.Quantity, "OPENINGDISCPCT");

                                    if (Convert.ToInt16(saleLineItem.PartnerData.FLAT) == 1 && Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) > 0)
                                    {
                                        decimal dMkDisc = 0m;

                                        dMkDisc = ((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) * dinitDiscValue) / 100);

                                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                            dinitDiscValue = (dMkDisc * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);

                                        iIsDiscFlat = 1;
                                    }
                                    //else
                                    //    dinitDiscValue = 0;// REQ BY Supapta Boral on 10/06/19

                                    LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                    discountItem.Percentage = dinitDiscValue;
                                    discountItem.LineDiscountType = DiscountTypes.Customer;
                                    saleItem.Add(discountItem);
                                    //}

                                    if (dinitDiscValue > 0)
                                    {
                                        saleLineItem.PartnerData.PROMOCODE = sPromoCode;
                                        saleLineItem.PartnerData.FLAT = iIsDiscFlat;
                                        saleLineItem.PartnerData.OpeningDisc = dinitDiscValue;
                                        if (saleItem.PartnerData.isMRP)
                                            saleLineItem.PartnerData.OpeningDiscType = (int)MRPOrMkDiscType.MRP;
                                        else
                                            saleLineItem.PartnerData.OpeningDiscType = (int)MRPOrMkDiscType.Making;
                                    }
                                    else
                                    {
                                        saleLineItem.PartnerData.PROMOCODE = "";
                                        saleLineItem.PartnerData.FLAT = iIsDiscFlat;
                                        saleLineItem.PartnerData.OpeningDisc = 0;
                                        saleLineItem.PartnerData.OpeningDiscType = 0;
                                    }
                                }
                            }
                            #endregion

                            objTax.Initialize(application);
                            objTax.CalculateTax(transaction);
                        }
                    }
                }
            }
            #endregion
            // END 

            //Start : GENERALDISC==true
            if (transaction != null && transaction.SaleItems != null && transaction.SaleItems.Count > 0)
            {
                if (transaction.PartnerData.APPLYGSSDISCDONE == false && Convert.ToDecimal(transaction.PartnerData.ExtraMkDiscPct) == 0)
                {
                    #region Making disc && MRP disc
                    if (transaction.PartnerData.IsGiftCardIssue == "0"
                        && transaction.PartnerData.SKUAgingDisc == false
                        && transaction.PartnerData.SKUAgingDiscDone == false
                        && transaction.PartnerData.TierDiscDone == false
                        && transaction.PartnerData.TierDisc == false
                        && transaction.PartnerData.APPLYGSSDISCDONE == false
                        && transaction.PartnerData.MakStnDiaDiscDone == false
                        && transaction.PartnerData.MakStnDiaDisc == false
                        && transaction.PartnerData.AdvAgreemetDisc == false
                        && transaction.PartnerData.AdvAgreemetDiscDone == false)
                    {
                        SaleLineItem saleItem = transaction.SaleItems.Last.Value;
                        decimal dinitDiscValue = 0;
                        DateTime dtFDate = DateTime.Now;

                        int isClubbed = 0;

                        dDiscAmt = 0m;

                        foreach (SaleLineItem saleLineItem in transaction.SaleItems)
                        {
                            if (Convert.ToInt16(saleLineItem.ItemType) != 2)
                            {
                                //if (saleLineItem.LinePctDiscount == 0)
                                //{
                                #region NimMakingDisc
                                if (!saleLineItem.Voided
                                    && saleLineItem.PartnerData.NimMakingDiscType == true
                                    && saleLineItem.LineId == saleLineItem.PartnerData.NimDiscLine
                                    )//&& Convert.ToDecimal(saleLineItem.PartnerData.NimMakingDisc) > 0//&& saleLineItem.LinePctDiscount == 0
                                {
                                    if (saleLineItem.PartnerData.TransactionType == Convert.ToString((int)TransactionType.Sale))
                                    {
                                        dinitDiscValue = saleLineItem.PartnerData.NimMakingDisc;
                                        isClubbed = Convert.ToInt16(saleLineItem.PartnerData.NimMakingDiscClubbed);

                                        if (isClubbed == 1
                                            && saleLineItem.PartnerData.NimMakingDiscTypeDone == false)
                                        {
                                            CalMakingDiscount(dinitDiscValue, isClubbed, saleLineItem);
                                        }
                                        else if (isClubbed == 0)
                                        {
                                            CalMakingDiscount(dinitDiscValue, isClubbed, saleLineItem);
                                        }

                                        //else
                                        //{
                                        //    MessageBox.Show("Making discount already applied");
                                        //}
                                    }
                                    saleLineItem.CalculateLine();
                                    foreach (ITaxItem taxLine in saleLineItem.TaxLines)
                                    {
                                        if (taxLine.Percentage != decimal.Zero)
                                        {
                                            taxLine.Amount = decimal.Round((saleLineItem.NetAmountWithNoTax * taxLine.Percentage / 100), 2, MidpointRounding.AwayFromZero);
                                        }
                                    }
                                    transaction.CalculateAmountDue();
                                    transaction.CalcTotals();
                                }
                                #endregion
                                // }

                                #region MRP
                                if (!saleLineItem.Voided
                                    && saleLineItem.LineId == saleLineItem.PartnerData.NimDiscLine
                                    && saleLineItem.PartnerData.NimMRPDiscType == true// && saleLineItem.LinePctDiscount == 0
                                    && Convert.ToDecimal(saleLineItem.PartnerData.NimMRPDisc) > 0)
                                {
                                    if (saleLineItem.PartnerData.TransactionType == Convert.ToString((int)TransactionType.Sale))
                                    {
                                        dinitDiscValue = saleLineItem.PartnerData.NimMRPDisc;
                                        isClubbed = Convert.ToInt16(saleLineItem.PartnerData.NimMRPDiscClubbed);

                                        if (isClubbed == 1
                                            && saleLineItem.PartnerData.NimMRPDiscTypeDone == false)
                                        {
                                            CalMRPDiscount(dinitDiscValue, isClubbed, saleLineItem);
                                        }
                                        else if (isClubbed == 0)
                                        {
                                            CalMRPDiscount(dinitDiscValue, isClubbed, saleLineItem);
                                        }
                                    }
                                    saleLineItem.CalculateLine();
                                    foreach (ITaxItem taxLine in saleLineItem.TaxLines)
                                    {
                                        if (taxLine.Percentage != decimal.Zero)
                                        {
                                            taxLine.Amount = decimal.Round((saleLineItem.NetAmountWithNoTax * taxLine.Percentage / 100), 2, MidpointRounding.AwayFromZero);
                                        }
                                    }
                                    transaction.CalculateAmountDue();
                                    transaction.CalcTotals();
                                }
                                #endregion


                                if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.NIMPROMOCODE))
                                    && string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.PROMOCODE)))
                                {
                                    //if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)))
                                    //{
                                    #region NimPromoMakingDisc
                                    if (!saleLineItem.Voided
                                        && saleLineItem.PartnerData.NimPromoMakingDiscType == true
                                        && saleLineItem.PartnerData.NimPromoMakingDiscTypeDone == false
                                        //&& saleLineItem.LinePctDiscount == 0
                                        && saleLineItem.LineId == Convert.ToInt16(saleLineItem.PartnerData.NimDiscLine))//&& Convert.ToDecimal(saleLineItem.PartnerData.NimMakingDisc) > 0
                                    {
                                        if (saleLineItem.PartnerData.TransactionType == Convert.ToString((int)TransactionType.Sale))
                                        {
                                            dinitDiscValue = saleLineItem.PartnerData.NimPromoMakingDisc;
                                            isClubbed = Convert.ToInt16(saleLineItem.PartnerData.NimPromoMakingDiscClubbed);

                                            if (isClubbed == 1
                                                && saleLineItem.PartnerData.NimPromoMakingDiscTypeDone == false)
                                            {
                                                CalPromoMakingDiscount(dinitDiscValue, isClubbed, saleLineItem);
                                            }
                                            else if (isClubbed == 0)
                                            {
                                                CalPromoMakingDiscount(dinitDiscValue, isClubbed, saleLineItem);
                                            }

                                            //else
                                            //{
                                            //    MessageBox.Show("Making discount already applied");
                                            //}
                                        }
                                        saleLineItem.CalculateLine();
                                        foreach (ITaxItem taxLine in saleLineItem.TaxLines)
                                        {
                                            if (taxLine.Percentage != decimal.Zero)
                                            {
                                                taxLine.Amount = decimal.Round((saleLineItem.NetAmountWithNoTax * taxLine.Percentage / 100), 2, MidpointRounding.AwayFromZero);
                                            }
                                        }
                                        transaction.CalculateAmountDue();
                                        transaction.CalcTotals();
                                    }
                                    #endregion
                                    //}

                                    #region PromoMRP
                                    if (!saleLineItem.Voided
                                        && saleLineItem.LineId == Convert.ToInt16(saleLineItem.PartnerData.NimDiscLine)
                                        && saleLineItem.PartnerData.NimPromoMRPDiscType == true
                                        && saleLineItem.PartnerData.NimPromoMRPDiscTypeDone == false
                                        //&& saleLineItem.LinePctDiscount == 0
                                        && Convert.ToDecimal(saleLineItem.PartnerData.NimPromoMRPDisc) > 0)
                                    {
                                        if (saleLineItem.PartnerData.TransactionType == Convert.ToString((int)TransactionType.Sale))
                                        {
                                            dinitDiscValue = saleLineItem.PartnerData.NimPromoMRPDisc;
                                            isClubbed = Convert.ToInt16(saleLineItem.PartnerData.NimPromoMRPDiscClubbed);

                                            if (isClubbed == 1
                                                && saleLineItem.PartnerData.NimPromoMRPDiscTypeDone == false)
                                            {
                                                CalPromoMRPDiscount(dinitDiscValue, isClubbed, saleLineItem);
                                            }
                                            else if (isClubbed == 0)
                                            {
                                                CalPromoMRPDiscount(dinitDiscValue, isClubbed, saleLineItem);
                                            }
                                        }
                                        saleLineItem.CalculateLine();
                                        foreach (ITaxItem taxLine in saleLineItem.TaxLines)
                                        {
                                            if (taxLine.Percentage != decimal.Zero)
                                            {
                                                taxLine.Amount = decimal.Round((saleLineItem.NetAmountWithNoTax * taxLine.Percentage / 100), 2, MidpointRounding.AwayFromZero);
                                            }
                                        }
                                        transaction.CalculateAmountDue();
                                        transaction.CalcTotals();
                                    }
                                    #endregion
                                }
                                // }
                                //else
                                //{
                                //    MessageBox.Show("Making discount already applied");
                                //}
                            }
                        }
                    }
                    #endregion

                    #region SKU aging disc
                    if (transaction.PartnerData.SKUAgingDisc == true
                        && transaction.PartnerData.SKUAgingDiscDone == false
                        && transaction.PartnerData.IsGiftCardIssue == "0"
                        && transaction.PartnerData.APPLYGSSDISCDONE == false)
                    {
                        Decimal dMetalValue = 0;
                        Decimal dDiamondValue = 0;
                        Decimal dStoneValue = 0;
                        Decimal dMetalWt = 0;
                        Decimal dDiamondWt = 0;
                        Decimal dStoneWt = 0;
                        decimal dinitDiscValue = 0;
                        DateTime dtFDate = DateTime.Now;
                        int iFDays = 0;
                        int iTDays = 0;
                        int isClubbed = 0;
                        int iDiscType = 0;

                        string sStorGrp = getStoreFormatCode();

                        foreach (SaleLineItem saleLineItem in transaction.SaleItems)
                        {
                            dDiscAmt = 0m;
                            if (Convert.ToInt16(saleLineItem.ItemType) != 2)
                            {
                                if (saleLineItem.LinePctDiscount == 0)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)))
                                    {
                                        if (!saleLineItem.Voided)
                                        {
                                            if (saleLineItem.PartnerData.TransactionType == Convert.ToString((int)TransactionType.Sale))
                                            {
                                                if (saleLineItem.LineId == 1 && saleLineItem.LinePctDiscount == 0)// && saleLineItem.LinePctDiscount == 0 added for Apollo
                                                {
                                                    dMetalValue = 0;
                                                    dDiamondValue = 0;
                                                    dStoneValue = 0;
                                                    dMetalWt = 0;
                                                    dDiamondWt = 0;
                                                    dStoneWt = 0;

                                                    #region Line discount %
                                                    DataSet dsIngredients = new DataSet();
                                                    int i = 1;
                                                    int index = 1;
                                                    int iMetalType = 0;

                                                    StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                                    dsIngredients.ReadXml(reader);

                                                    foreach (DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                                    {
                                                        index = i;
                                                        iMetalType = Convert.ToInt16(!string.IsNullOrEmpty(Convert.ToString(drIngredients["MetalType"])) ? drIngredients["MetalType"] : "0");

                                                        if ((iMetalType == (int)MetalType.Gold) || (iMetalType == (int)MetalType.Silver)
                                                            || (iMetalType == (int)MetalType.Platinum) || (iMetalType == (int)MetalType.Palladium))
                                                        {
                                                            dMetalValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                            dMetalWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                        }

                                                        if (iMetalType == (int)MetalType.LooseDmd)
                                                        {
                                                            dDiamondValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                            dDiamondWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                        }

                                                        if (iMetalType == (int)MetalType.Stone)
                                                        {
                                                            dStoneValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                            dStoneWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                        }
                                                        i++;
                                                    }

                                                    //if (saleLineItem.PartnerData.isMRP)
                                                    //    dinitDiscValue = GetSKUAgingDiscount(saleLineItem.ItemId, ref dtFDate, ref isClubbed, ref iDiscType,ref iFDays,ref iTDays);//, Convert.ToDecimal(saleLineItem.Quantity), dMetalWt, dDiamondWt, dStoneWt
                                                    //else
                                                    dinitDiscValue = GetSKUAgingDiscount(saleLineItem.ItemId, sStorGrp, ref dtFDate, ref isClubbed, ref iDiscType, ref iFDays, ref iTDays, ref iPromoDisctype);//, Convert.ToDecimal(saleLineItem.PartnerData.Quantity), dMetalWt, dDiamondWt, dStoneWt

                                                    DateTime dateTimeForItemAge = GetItemAge(saleLineItem.ItemId);
                                                    int iItemAge = 0;

                                                    if (dateTimeForItemAge > Convert.ToDateTime("01-01-1900"))
                                                    {
                                                        System.DateTime firstDate = dateTimeForItemAge;
                                                        System.DateTime secondDate = dtFDate;// DateTime.Now;
                                                        DateTime zeroTime = new DateTime(1, 1, 1);
                                                        System.TimeSpan diff = secondDate.Subtract(firstDate);
                                                        int iDays = Convert.ToInt16((secondDate - firstDate).TotalDays);

                                                        //int iYears = (zeroTime + diff).Year - 1;


                                                        //int iMonth = ((secondDate.Year - firstDate.Year) * 12) + (secondDate.Month - firstDate.Month);
                                                        iItemAge = iDays;
                                                    }


                                                    //if (iItemAgeType == (int)CRWPromotionAgeType.Day)
                                                    //    iItemAge = iDays;
                                                    //else if (iItemAgeType == (int)CRWPromotionAgeType.Month)
                                                    //    iItemAge = iMonth;
                                                    //else
                                                    //    iItemAge = iYears;

                                                    if (iFDays <= iItemAge && iTDays >= iItemAge)
                                                    {
                                                        if (iDiscType == (int)DiscType.Pct)
                                                        {
                                                            if (iPromoDisctype == (int)DiscPromotionType.LineAmount)
                                                                dDiscAmt = ((Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) * dinitDiscValue) / 100);
                                                            if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                                dDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) + Convert.ToDecimal(saleLineItem.PartnerData.WastageAmount)) * dinitDiscValue) / 100);
                                                            else if (iPromoDisctype == (int)DiscPromotionType.MakingAndStoneValue)
                                                                dDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) + dStoneValue + dDiamondValue) * dinitDiscValue) / 100);

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                            {
                                                                if (isClubbed == 0)
                                                                {
                                                                    NonClubbedSKUAgingDiscountApply(transaction, saleLineItem);
                                                                }
                                                                else
                                                                {
                                                                    LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                    {
                                                                        discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                                        discountItem.LineDiscountType = DiscountTypes.Customer;
                                                                        saleLineItem.Add(discountItem);

                                                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                            transaction.PartnerData.SKUAgingDiscDone = true;
                                                                        else
                                                                            transaction.PartnerData.SKUAgingDiscDone = false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (iDiscType == (int)DiscType.Amt)
                                                        {
                                                            if (iPromoDisctype == (int)DiscPromotionType.LineAmount)
                                                                dDiscAmt = dinitDiscValue; // only dinitDiscValue
                                                            if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                                dDiscAmt = dMetalWt * dinitDiscValue; // netwt * dinitDiscValue
                                                            else if (iPromoDisctype == (int)DiscPromotionType.MakingAndStoneValue)
                                                                dDiscAmt = (dMetalWt + dStoneWt + dDiamondWt) * dinitDiscValue; //stwt* dinitDiscValue

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                            {
                                                                if (isClubbed == 0)
                                                                {
                                                                    NonClubbedSKUAgingDiscountApply(transaction, saleLineItem);
                                                                }
                                                                else
                                                                {
                                                                    LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                        discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                                    discountItem.LineDiscountType = DiscountTypes.Customer;
                                                                    saleLineItem.Add(discountItem);

                                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                    {
                                                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                            transaction.PartnerData.SKUAgingDiscDone = true;
                                                                        else
                                                                            transaction.PartnerData.SKUAgingDiscDone = false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        if (dDiscAmt > 0)
                                                        {
                                                            saleLineItem.PartnerData.SKUAgingDiscType = iDiscType;
                                                            saleLineItem.PartnerData.SKUAgingDiscAmt = dDiscAmt;
                                                        }
                                                        else
                                                        {
                                                            saleLineItem.PartnerData.SKUAgingDiscType = 0;
                                                            saleLineItem.PartnerData.SKUAgingDiscAmt = 0;
                                                        }
                                                    }

                                                    #endregion Line discount %
                                                }
                                                else if (saleLineItem.ItemId == saleLineItem.ItemId && saleLineItem.LineId != 1)//&& saleLineItem.LinePctDiscount == 0
                                                {
                                                    dMetalValue = 0;
                                                    dDiamondValue = 0;
                                                    dStoneValue = 0;
                                                    dMetalWt = 0;
                                                    dDiamondWt = 0;
                                                    dStoneWt = 0;

                                                    #region Line Discount %
                                                    DataSet dsIngredients = new DataSet();
                                                    StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                                    dsIngredients.ReadXml(reader);
                                                    int i = 1;
                                                    int index = 1;
                                                    int iMetalType = 0;
                                                    foreach (DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                                    {
                                                        index = i;
                                                        iMetalType = Convert.ToInt16(!string.IsNullOrEmpty(Convert.ToString(drIngredients["MetalType"])) ? drIngredients["MetalType"] : "0");

                                                        if ((iMetalType == (int)MetalType.Gold) || (iMetalType == (int)MetalType.Silver)
                                                            || (iMetalType == (int)MetalType.Platinum) || (iMetalType == (int)MetalType.Palladium))
                                                        {
                                                            dMetalValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                            dMetalWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                        }

                                                        if (iMetalType == (int)MetalType.LooseDmd)
                                                        {
                                                            dDiamondValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                            dDiamondWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                        }

                                                        if (iMetalType == (int)MetalType.Stone)
                                                        {
                                                            dStoneValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                            dStoneWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                        }
                                                        i++;
                                                    }

                                                    //if (saleLineItem.PartnerData.isMRP)
                                                    //    dinitDiscValue = GetSKUAgingDiscount(saleLineItem.ItemId, ref dtFDate, ref isClubbed, ref iDiscType, ref iFDays, ref iTDays);//, Convert.ToDecimal(saleLineItem.Quantity), dMetalWt, dDiamondWt, dStoneWt
                                                    //else
                                                    dinitDiscValue = GetSKUAgingDiscount(saleLineItem.ItemId, sStorGrp, ref dtFDate, ref isClubbed, ref iDiscType, ref iFDays, ref iTDays, ref iPromoDisctype);//, Convert.ToDecimal(saleLineItem.PartnerData.Quantity), dMetalWt, dDiamondWt, dStoneWt

                                                    DateTime dateTimeForItemAge = GetItemAge(saleLineItem.ItemId);
                                                    int iItemAge = 0;
                                                    if (dateTimeForItemAge > Convert.ToDateTime("01-01-1900"))
                                                    {
                                                        System.DateTime firstDate = dateTimeForItemAge;
                                                        System.DateTime secondDate = dtFDate;// DateTime.Now;
                                                        DateTime zeroTime = new DateTime(1, 1, 1);
                                                        System.TimeSpan diff = secondDate.Subtract(firstDate);
                                                        int iDays = Convert.ToInt16((secondDate - firstDate).TotalDays);
                                                        int iYears = (zeroTime + diff).Year - 1;
                                                        int iMonth = ((secondDate.Year - firstDate.Year) * 12) + (secondDate.Month - firstDate.Month);
                                                        iItemAge = iDays;
                                                    }

                                                    //if (iItemAgeType == (int)CRWPromotionAgeType.Day)
                                                    //    iItemAge = iDays;
                                                    //else if (iItemAgeType == (int)CRWPromotionAgeType.Month)
                                                    //    iItemAge = iMonth;
                                                    //else
                                                    //    iItemAge = iYears;

                                                    if (iFDays <= iItemAge && iTDays >= iItemAge)
                                                    {
                                                        if (iDiscType == (int)DiscType.Pct)
                                                        {
                                                            if (iPromoDisctype == (int)DiscPromotionType.LineAmount)
                                                                dDiscAmt = ((Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) * dinitDiscValue) / 100);
                                                            else if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                                dDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) + Convert.ToDecimal(saleLineItem.PartnerData.WastageAmount)) * dinitDiscValue) / 100);
                                                            else if (iPromoDisctype == (int)DiscPromotionType.MakingAndStoneValue)
                                                                dDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) + dStoneValue + dDiamondValue) * dinitDiscValue) / 100);

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                            {
                                                                if (isClubbed == 0)
                                                                {
                                                                    NonClubbedSKUAgingDiscountApply(transaction, saleLineItem);
                                                                }
                                                                else
                                                                {
                                                                    LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                    {
                                                                        discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                                        discountItem.LineDiscountType = DiscountTypes.Customer;
                                                                        saleLineItem.Add(discountItem);

                                                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                            transaction.PartnerData.SKUAgingDiscDone = true;
                                                                        else
                                                                            transaction.PartnerData.SKUAgingDiscDone = false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        else if (iDiscType == (int)DiscType.Amt)
                                                        {
                                                            if (iPromoDisctype == (int)DiscPromotionType.LineAmount)
                                                                dDiscAmt = dinitDiscValue; // only dinitDiscValue
                                                            else if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                                dDiscAmt = dMetalWt * dinitDiscValue; // netwt * dinitDiscValue
                                                            else if (iPromoDisctype == (int)DiscPromotionType.MakingAndStoneValue)
                                                                dDiscAmt = (dMetalWt + dStoneWt + dDiamondWt) * dinitDiscValue; //stwt* dinitDiscValue

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                            {
                                                                if (isClubbed == 0)
                                                                {
                                                                    NonClubbedSKUAgingDiscountApply(transaction, saleLineItem);
                                                                }
                                                                else
                                                                {
                                                                    LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                    {
                                                                        discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                                        discountItem.LineDiscountType = DiscountTypes.Customer;
                                                                        saleLineItem.Add(discountItem);

                                                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                            transaction.PartnerData.SKUAgingDiscDone = true;
                                                                        else
                                                                            transaction.PartnerData.SKUAgingDiscDone = false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        if (dDiscAmt > 0)
                                                        {
                                                            saleLineItem.PartnerData.SKUAgingDiscType = iDiscType;
                                                            saleLineItem.PartnerData.SKUAgingDiscAmt = dDiscAmt;
                                                        }
                                                        else
                                                        {
                                                            saleLineItem.PartnerData.SKUAgingDiscType = 0;
                                                            saleLineItem.PartnerData.SKUAgingDiscAmt = 0;
                                                        }
                                                    }

                                                    #endregion Line Discount %
                                                }
                                            }
                                        }
                                        saleLineItem.CalculateLine();
                                        foreach (ITaxItem taxLine in saleLineItem.TaxLines)
                                        {
                                            if (taxLine.Percentage != decimal.Zero)
                                            {
                                                taxLine.Amount = decimal.Round((saleLineItem.NetAmountWithNoTax * taxLine.Percentage / 100), 2, MidpointRounding.AwayFromZero);
                                            }
                                        }
                                        transaction.CalculateAmountDue();
                                        transaction.CalcTotals();
                                    }
                                }
                                else
                                {
                                    transaction.PartnerData.SKUAgingDisc = false;
                                }
                            }

                        }
                    }
                    #endregion

                    #region Tier disc
                    if (transaction.PartnerData.TierDisc == true
                        && transaction.PartnerData.TierDiscDone == false
                        && transaction.PartnerData.IsGiftCardIssue == "0"
                        && transaction.PartnerData.APPLYGSSDISCDONE == false)
                    {

                        Decimal dMetalValue = 0;
                        Decimal dDiamondValue = 0;
                        Decimal dDiamondWt = 0;
                        //int iFDays = 0;
                        //int iTDays = 0;
                        Decimal dStoneValue = 0;
                        Decimal dMetalWt = 0;

                        Decimal dStoneWt = 0;
                        decimal dinitDiscValue = 0;
                        DateTime dtFDate = DateTime.Now;
                        string sCustLoyaltyScheme = "";

                        int isClubbed = 0;
                        int iDiscType = 0;

                        string sStorGrp = getStoreFormatCode();

                        foreach (SaleLineItem saleLineItem in transaction.SaleItems)
                        {
                            dDiscAmt = 0m;
                            if (Convert.ToInt16(saleLineItem.ItemType) != 2)
                            {
                                if (saleLineItem.LinePctDiscount == 0)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)))
                                    {
                                        if (!saleLineItem.Voided)
                                        {
                                            if (saleLineItem.PartnerData.TransactionType == Convert.ToString((int)TransactionType.Sale))
                                            {
                                                if (saleLineItem.LineId == 1 && saleLineItem.LinePctDiscount == 0)// 
                                                {
                                                    dMetalValue = 0;
                                                    dDiamondValue = 0;
                                                    dDiamondWt = 0;
                                                    dStoneValue = 0;
                                                    dMetalWt = 0;

                                                    dStoneWt = 0;

                                                    #region Line discount %
                                                    #region Cal
                                                    DataSet dsIngredients = new DataSet();
                                                    int i = 1;
                                                    int index = 1;
                                                    int iMetalType = 0;

                                                    StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                                    dsIngredients.ReadXml(reader);

                                                    foreach (DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                                    {
                                                        index = i;
                                                        iMetalType = Convert.ToInt16(!string.IsNullOrEmpty(Convert.ToString(drIngredients["MetalType"])) ? drIngredients["MetalType"] : "0");

                                                        if ((iMetalType == (int)MetalType.Gold) || (iMetalType == (int)MetalType.Silver)
                                                            || (iMetalType == (int)MetalType.Platinum) || (iMetalType == (int)MetalType.Palladium))
                                                        {
                                                            dMetalValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                            dMetalWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                        }

                                                        if (iMetalType == (int)MetalType.LooseDmd)
                                                        {
                                                            dDiamondValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                            dDiamondWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                        }

                                                        if (iMetalType == (int)MetalType.Stone)
                                                        {
                                                            dStoneValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                            dStoneWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                        }
                                                        i++;
                                                    }

                                                    //if (saleLineItem.PartnerData.isMRP)
                                                    //    dinitDiscValue = GetSKUAgingDiscount(saleLineItem.ItemId, ref dtFDate, ref isClubbed, ref iDiscType,ref iFDays,ref iTDays);//, Convert.ToDecimal(saleLineItem.Quantity), dMetalWt, dDiamondWt, dStoneWt
                                                    //else
                                                    #endregion

                                                    dinitDiscValue = GetTierDiscount(saleLineItem.ItemId, sStorGrp, saleLineItem.Quantity, transaction.Customer.CustomerId, ref sCustLoyaltyScheme, ref isClubbed, ref iDiscType, ref iPromoDisctype);//, Convert.ToDecimal(saleLineItem.PartnerData.Quantity), dMetalWt, dDiamondWt, dStoneWt

                                                    #region Commented for future
                                                    //DateTime dateTimeForItemAge = GetItemAge(saleLineItem.ItemId);
                                                    //int iItemAge = 0;

                                                    //if (dateTimeForItemAge > Convert.ToDateTime("01-01-1900"))
                                                    //{
                                                    //    System.DateTime firstDate = dateTimeForItemAge;
                                                    //    System.DateTime secondDate = dtFDate;// DateTime.Now;
                                                    //    DateTime zeroTime = new DateTime(1, 1, 1);
                                                    //    System.TimeSpan diff = secondDate.Subtract(firstDate);
                                                    //    int iDays = Convert.ToInt16((secondDate - firstDate).TotalDays);
                                                    //    int iYears = (zeroTime + diff).Year - 1;
                                                    //    int iMonth = ((secondDate.Year - firstDate.Year) * 12) + (secondDate.Month - firstDate.Month);
                                                    //    iItemAge = iDays;
                                                    //}


                                                    //if (iItemAgeType == (int)CRWPromotionAgeType.Day)
                                                    //    iItemAge = iDays;
                                                    //else if (iItemAgeType == (int)CRWPromotionAgeType.Month)
                                                    //    iItemAge = iMonth;
                                                    //else
                                                    //    iItemAge = iYears;

                                                    //if (iFDays <= iItemAge && iTDays >= iItemAge)
                                                    //{
                                                    #endregion

                                                    if (dinitDiscValue > 0)
                                                    {
                                                        #region DiscType.Pct
                                                        if (iDiscType == (int)DiscType.Pct)
                                                        {
                                                            if (iPromoDisctype == (int)DiscPromotionType.LineAmount)
                                                                dDiscAmt = ((Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) * dinitDiscValue) / 100);
                                                            if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                                dDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) + Convert.ToDecimal(saleLineItem.PartnerData.WastageAmount)) * dinitDiscValue) / 100);
                                                            else if (iPromoDisctype == (int)DiscPromotionType.MakingAndStoneValue)
                                                                dDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) + dStoneValue + dDiamondValue) * dinitDiscValue) / 100);

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                            {
                                                                if (isClubbed == 0)
                                                                {
                                                                    NonClubbedTierDiscountApply(transaction, saleLineItem);
                                                                }
                                                                else
                                                                {
                                                                    LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                        discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                                    discountItem.LineDiscountType = DiscountTypes.Customer;
                                                                    saleLineItem.Add(discountItem);

                                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                    {
                                                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                            transaction.PartnerData.TierDiscDone = true;
                                                                        else
                                                                            transaction.PartnerData.TierDiscDone = false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        #endregion

                                                        #region DiscType.Amt
                                                        else if (iDiscType == (int)DiscType.Amt)
                                                        {
                                                            if (iPromoDisctype == (int)DiscPromotionType.LineAmount)
                                                                dDiscAmt = dinitDiscValue; // only dinitDiscValue
                                                            if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                                dDiscAmt = dMetalWt * dinitDiscValue; // netwt * dinitDiscValue
                                                            else if (iPromoDisctype == (int)DiscPromotionType.MakingAndStoneValue)
                                                                dDiscAmt = (dMetalWt + dStoneWt + dDiamondWt) * dinitDiscValue; //stwt* dinitDiscValue

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                            {
                                                                if (isClubbed == 0)
                                                                {
                                                                    NonClubbedTierDiscountApply(transaction, saleLineItem);
                                                                }
                                                                else
                                                                {
                                                                    LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                        discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                                    discountItem.LineDiscountType = DiscountTypes.Customer;
                                                                    saleLineItem.Add(discountItem);

                                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                    {
                                                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                            transaction.PartnerData.TierDiscDone = true;
                                                                        else
                                                                            transaction.PartnerData.TierDiscDone = false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        #endregion

                                                        if (dDiscAmt > 0)
                                                        {
                                                            saleLineItem.PartnerData.TierDiscType = iDiscType;
                                                            saleLineItem.PartnerData.TierDiscAmt = dDiscAmt;
                                                        }
                                                        else
                                                        {
                                                            saleLineItem.PartnerData.TierDiscType = 0;
                                                            saleLineItem.PartnerData.TierDiscAmt = 0;
                                                        }
                                                    }
                                                    #endregion Line discount %

                                                }
                                                else if (saleLineItem.ItemId == saleLineItem.ItemId && saleLineItem.LineId != 1)//&& saleItem.LinePctDiscount == 0
                                                {
                                                    dMetalValue = 0;
                                                    dDiamondValue = 0;
                                                    dDiamondWt = 0;
                                                    dStoneValue = 0;
                                                    dMetalWt = 0;
                                                    dStoneWt = 0;

                                                    #region Line Discount %
                                                    #region Commented for future
                                                    DataSet dsIngredients = new DataSet();
                                                    StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                                    dsIngredients.ReadXml(reader);
                                                    int i = 1;
                                                    int index = 1;
                                                    int iMetalType = 0;
                                                    foreach (DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                                    {
                                                        index = i;
                                                        iMetalType = Convert.ToInt16(!string.IsNullOrEmpty(Convert.ToString(drIngredients["MetalType"])) ? drIngredients["MetalType"] : "0");

                                                        if ((iMetalType == (int)MetalType.Gold) || (iMetalType == (int)MetalType.Silver)
                                                            || (iMetalType == (int)MetalType.Platinum) || (iMetalType == (int)MetalType.Palladium))
                                                        {
                                                            dMetalValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                            dMetalWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                        }

                                                        if (iMetalType == (int)MetalType.LooseDmd)
                                                        {
                                                            dDiamondValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                            dDiamondWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                        }

                                                        if (iMetalType == (int)MetalType.Stone)
                                                        {
                                                            dStoneValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                            dStoneWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                        }
                                                        i++;
                                                    }

                                                    //if (saleItem.PartnerData.isMRP)
                                                    //    dinitDiscValue = GetSKUAgingDiscount(saleItem.ItemId, ref dtFDate, ref isClubbed, ref iDiscType, ref iFDays, ref iTDays);//, Convert.ToDecimal(saleItem.Quantity), dMetalWt, dDiamondWt, dStoneWt
                                                    //else
                                                    #endregion
                                                    dinitDiscValue = GetTierDiscount(saleLineItem.ItemId, sStorGrp, saleLineItem.Quantity, transaction.Customer.CustomerId, ref sCustLoyaltyScheme, ref isClubbed, ref iDiscType, ref iPromoDisctype);

                                                    #region commented for future
                                                    //DateTime dateTimeForItemAge = GetItemAge(saleLineItem.ItemId);
                                                    //int iItemAge = 0;
                                                    //if (dateTimeForItemAge > Convert.ToDateTime("01-01-1900"))
                                                    //{
                                                    //    System.DateTime firstDate = dateTimeForItemAge;
                                                    //    System.DateTime secondDate = dtFDate;// DateTime.Now;
                                                    //    DateTime zeroTime = new DateTime(1, 1, 1);
                                                    //    System.TimeSpan diff = secondDate.Subtract(firstDate);
                                                    //    int iDays = Convert.ToInt16((secondDate - firstDate).TotalDays);
                                                    //    int iYears = (zeroTime + diff).Year - 1;
                                                    //    int iMonth = ((secondDate.Year - firstDate.Year) * 12) + (secondDate.Month - firstDate.Month);
                                                    //    iItemAge = iDays;
                                                    //}

                                                    //if (iItemAgeType == (int)CRWPromotionAgeType.Day)
                                                    //    iItemAge = iDays;
                                                    //else if (iItemAgeType == (int)CRWPromotionAgeType.Month)
                                                    //    iItemAge = iMonth;
                                                    //else
                                                    //    iItemAge = iYears;

                                                    //if (iFDays <= iItemAge && iTDays >= iItemAge)
                                                    //{
                                                    #endregion

                                                    if (dinitDiscValue > 0)
                                                    {
                                                        #region DiscType.Pct
                                                        if (iDiscType == (int)DiscType.Pct)
                                                        {
                                                            if (iPromoDisctype == (int)DiscPromotionType.LineAmount)
                                                                dDiscAmt = ((Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) * dinitDiscValue) / 100);
                                                            else if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                                dDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) + Convert.ToDecimal(saleLineItem.PartnerData.WastageAmount)) * dinitDiscValue) / 100);
                                                            else if (iPromoDisctype == (int)DiscPromotionType.MakingAndStoneValue)
                                                                dDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) + dStoneValue) * dinitDiscValue) / 100);

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                            {
                                                                if (isClubbed == 0)
                                                                {
                                                                    NonClubbedTierDiscountApply(transaction, saleLineItem);
                                                                }
                                                                else
                                                                {
                                                                    LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                        discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                                    discountItem.LineDiscountType = DiscountTypes.Customer;
                                                                    saleLineItem.Add(discountItem);

                                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                    {
                                                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                            transaction.PartnerData.TierDiscDone = true;
                                                                        else
                                                                            transaction.PartnerData.TierDiscDone = false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        #endregion

                                                        #region DiscType.Amt
                                                        else if (iDiscType == (int)DiscType.Amt)
                                                        {
                                                            if (iPromoDisctype == (int)DiscPromotionType.LineAmount)
                                                                dDiscAmt = dinitDiscValue; // only dinitDiscValue
                                                            else if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                                dDiscAmt = dMetalWt * dinitDiscValue; // netwt * dinitDiscValue
                                                            else if (iPromoDisctype == (int)DiscPromotionType.MakingAndStoneValue)
                                                                dDiscAmt = (dMetalWt + dStoneWt) * dinitDiscValue; //stwt* dinitDiscValue

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                            {
                                                                if (isClubbed == 0)
                                                                {
                                                                    NonClubbedTierDiscountApply(transaction, saleLineItem);
                                                                }
                                                                else
                                                                {
                                                                    LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                        discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                                    discountItem.LineDiscountType = DiscountTypes.Customer;
                                                                    saleLineItem.Add(discountItem);

                                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                    {
                                                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                            transaction.PartnerData.TierDiscDone = true;
                                                                        else
                                                                            transaction.PartnerData.TierDiscDone = false;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                        #endregion

                                                        if (dDiscAmt > 0)
                                                        {
                                                            saleLineItem.PartnerData.TierDiscType = iDiscType;
                                                            saleLineItem.PartnerData.TierDiscAmt = dDiscAmt;
                                                        }
                                                        else
                                                        {
                                                            saleLineItem.PartnerData.TierDiscType = 0;
                                                            saleLineItem.PartnerData.TierDiscAmt = 0;
                                                        }
                                                    }
                                                    #endregion Line Discount %
                                                }
                                            }
                                        }
                                        saleLineItem.CalculateLine();
                                        foreach (ITaxItem taxLine in saleLineItem.TaxLines)
                                        {
                                            if (taxLine.Percentage != decimal.Zero)
                                            {
                                                taxLine.Amount = decimal.Round((saleLineItem.NetAmountWithNoTax * taxLine.Percentage / 100), 2, MidpointRounding.AwayFromZero);
                                            }
                                        }
                                        transaction.CalculateAmountDue();
                                        transaction.CalcTotals();
                                    }
                                }
                                //else
                                //{
                                //    MessageBox.Show("Tier discount already applied");
                                //}
                            }
                        }
                    }
                    #endregion

                    #region MakStnDia disc
                    if (transaction.PartnerData.MakStnDiaDisc == true
                        && transaction.PartnerData.IsGiftCardIssue == "0"
                        && transaction.PartnerData.APPLYGSSDISCDONE == false)
                    {

                        Decimal dMetalValue = 0;
                        Decimal dDiamondValue = 0;
                        Decimal dDiamondWt = 0;
                        Decimal dStoneValue = 0;
                        Decimal dMetalWt = 0;

                        Decimal dStoneWt = 0;
                        decimal dinitDiscValue = 0;
                        decimal dinitMakDiscValue = 0;
                        decimal dinitStnDiscValue = 0;
                        decimal dinitDiaDiscValue = 0;
                        DateTime dtFDate = DateTime.Now;

                        int iDiscType = 0;
                        decimal pMakAgreement = 0;
                        decimal pStnAgreement = 0;
                        decimal pDiaAgreement = 0;

                        string sStorGrp = getStoreFormatCode();

                        foreach (SaleLineItem saleLineItem in transaction.SaleItems)
                        {
                            dDiscAmt = 0m;
                            if (Convert.ToInt16(saleLineItem.ItemType) != 2)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)))
                                {
                                    if (!saleLineItem.Voided)
                                    {
                                        if (saleLineItem.PartnerData.TransactionType == Convert.ToString((int)TransactionType.Sale))
                                        {
                                            if (saleLineItem.LineId == 1 && saleLineItem.PartnerData.MakStnDiaDiscDone == false)
                                            {
                                                dMetalValue = 0;
                                                dDiamondValue = 0;
                                                dDiamondWt = 0;
                                                dStoneValue = 0;
                                                dMetalWt = 0;

                                                dStoneWt = 0;

                                                #region Line discount %
                                                #region Cal
                                                DataSet dsIngredients = new DataSet();
                                                int i = 1;
                                                int index = 1;
                                                int iMetalType = 0;

                                                //saleLineItem.DiscountLines.Clear();
                                                StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                                dsIngredients.ReadXml(reader);

                                                foreach (DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                                {
                                                    index = i;
                                                    iMetalType = Convert.ToInt16(!string.IsNullOrEmpty(Convert.ToString(drIngredients["MetalType"])) ? drIngredients["MetalType"] : "0");

                                                    if ((iMetalType == (int)MetalType.Gold) || (iMetalType == (int)MetalType.Silver)
                                                        || (iMetalType == (int)MetalType.Platinum) || (iMetalType == (int)MetalType.Palladium))
                                                    {
                                                        dMetalValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                        dMetalWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                    }

                                                    if (iMetalType == (int)MetalType.LooseDmd)
                                                    {
                                                        dDiamondValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                        dDiamondWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                    }

                                                    if (iMetalType == (int)MetalType.Stone)
                                                    {
                                                        dStoneValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                        dStoneWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                    }
                                                    i++;
                                                }

                                                #region GetmakingStoneDiamondDiscount
                                                //===========================================Get Making/Stone/Diamond Discount================================================================================
                                                dinitDiscValue = GetMakDiscount(saleLineItem.ItemId, sStorGrp, ref dtFDate, ref iDiscType, ref iPromoDisctype, ref pMakAgreement, ref pStnAgreement, ref pDiaAgreement);
                                                dinitMakDiscValue = Convert.ToDecimal(saleLineItem.PartnerData.NimMakDisc);
                                                dinitStnDiscValue = Convert.ToDecimal(saleLineItem.PartnerData.NimStnDisc);
                                                dinitDiaDiscValue = Convert.ToDecimal(saleLineItem.PartnerData.NimDiaDisc);
                                                #endregion
                                                #endregion



                                                if (dinitMakDiscValue > 0 || dinitStnDiscValue > 0 || dinitDiaDiscValue > 0)
                                                {
                                                    saleLineItem.DiscountLines.Clear();
                                                    #region DiscType.Pct
                                                    if (iDiscType == (int)DiscType.Pct)
                                                    {
                                                        if (iPromoDisctype == (int)DiscPromotionType.LineAmount)
                                                            dDiscAmt = ((Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) * dinitDiscValue) / 100);
                                                        if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                            dDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) + Convert.ToDecimal(saleLineItem.PartnerData.WastageAmount)) * dinitMakDiscValue) / 100);
                                                        else if (iPromoDisctype == (int)DiscPromotionType.MakingAndStoneValue)
                                                            dDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) + dStoneValue + dDiamondValue) * dinitDiscValue) / 100);
                                                        //============================Soutik=============================================================
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Stone)
                                                            dStnDiscAmt = ((Convert.ToDecimal(dStoneValue) * dinitStnDiscValue) / 100);
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Diamond)
                                                            dDiaDiscAmt = ((Convert.ToDecimal(dDiamondValue) * dinitDiaDiscValue) / 100);
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Making_Stone)
                                                        {
                                                            dMakDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount)) * dinitMakDiscValue) / 100);
                                                            dStnDiscAmt = ((Convert.ToDecimal(dStoneValue) * dinitStnDiscValue) / 100);
                                                        }
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Making_Diamond)
                                                        {
                                                            dMakDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount)) * dinitMakDiscValue) / 100);
                                                            dDiaDiscAmt = ((Convert.ToDecimal(dDiamondValue) * dinitDiaDiscValue) / 100);
                                                        }
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Making_Stone_Diamond_Both)
                                                        {
                                                            dMakDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount)) * dinitMakDiscValue) / 100);
                                                            dStnDiscAmt = ((Convert.ToDecimal(dStoneValue) * dinitStnDiscValue) / 100);
                                                            dDiaDiscAmt = ((Convert.ToDecimal(dDiamondValue) * dinitDiaDiscValue) / 100);
                                                        }

                                                        dDiscAmt = dMakDiscAmt + dStnDiscAmt + dDiaDiscAmt;

                                                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                        {
                                                            LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                            discountItem.LineDiscountType = DiscountTypes.Customer;
                                                            saleLineItem.Add(discountItem);

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                            {
                                                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                {
                                                                    saleLineItem.PartnerData.MakStnDiaDiscDone = true;
                                                                    transaction.PartnerData.MakStnDiaDiscDone = true;
                                                                }
                                                                else
                                                                {
                                                                    saleLineItem.PartnerData.MakStnDiaDiscDone = false;
                                                                    transaction.PartnerData.MakStnDiaDiscDone = false;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    #region DiscType.Amt
                                                    else if (iDiscType == (int)DiscType.Amt)
                                                    {
                                                        if (iPromoDisctype == (int)DiscPromotionType.LineAmount)
                                                            dDiscAmt = dinitDiscValue; // only dinitDiscValue
                                                        if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                            dDiscAmt = dMetalWt * dinitDiscValue; // netwt * dinitDiscValue
                                                        else if (iPromoDisctype == (int)DiscPromotionType.MakingAndStoneValue)
                                                            dDiscAmt = (dMetalWt + dStoneWt + dDiamondWt) * dinitDiscValue; //stwt* dinitDiscValue

                                                        //============================Soutik=============================================================
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Stone)
                                                            dStnDiscAmt = (dStoneWt) * dinitStnDiscValue;
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Diamond)
                                                            dDiaDiscAmt = (dDiamondWt) * dinitDiaDiscValue;
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Making_Stone)
                                                        {
                                                            dMakDiscAmt = (dMetalWt) * dinitMakDiscValue;
                                                            dStnDiscAmt = (dStoneWt) * dinitStnDiscValue;
                                                        }
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Making_Diamond)
                                                        {
                                                            dMakDiscAmt = (dMetalWt) * dinitMakDiscValue;
                                                            dDiaDiscAmt = (dDiamondWt) * dinitDiaDiscValue;
                                                        }
                                                        if (iPromoDisctype == (int)DiscPromotionType.Making_Stone_Diamond_Both)
                                                        {
                                                            dMakDiscAmt = (dMetalWt) * dinitMakDiscValue;
                                                            dStnDiscAmt = (dStoneWt) * dinitStnDiscValue;
                                                            dDiaDiscAmt = (dDiamondWt) * dinitStnDiscValue;
                                                        }

                                                        dDiscAmt = dMakDiscAmt + dStnDiscAmt + dDiaDiscAmt;

                                                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                        {
                                                            LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                            discountItem.LineDiscountType = DiscountTypes.Customer;
                                                            saleLineItem.Add(discountItem);

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                            {
                                                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                {
                                                                    saleLineItem.PartnerData.MakStnDiaDiscDone = true;
                                                                    transaction.PartnerData.MakStnDiaDiscDone = true;
                                                                }
                                                                else
                                                                {
                                                                    saleLineItem.PartnerData.MakStnDiaDiscDone = false;
                                                                    transaction.PartnerData.MakStnDiaDiscDone = false;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    if (dDiscAmt > 0)
                                                    {
                                                        saleLineItem.PartnerData.MakStnDiaDiscType = iPromoDisctype;
                                                        saleLineItem.PartnerData.NimMakingDiscount = dMakDiscAmt;
                                                        saleLineItem.PartnerData.NimStoneDiscount = dStnDiscAmt;
                                                        saleLineItem.PartnerData.NimDiamondDiscount = dDiaDiscAmt;
                                                    }
                                                    else
                                                    {
                                                        saleLineItem.PartnerData.MakStnDiaDiscType = 0;
                                                        saleLineItem.PartnerData.NimMakingDiscount = 0;
                                                        saleLineItem.PartnerData.NimStoneDiscount = 0;
                                                        saleLineItem.PartnerData.NimDiamondDiscount = 0;
                                                    }
                                                }
                                                #endregion Line discount %

                                            }
                                            else if (saleLineItem.ItemId == saleLineItem.ItemId && saleLineItem.LineId != 1 && saleLineItem.PartnerData.MakStnDiaDiscDone == false)//&& saleItem.LinePctDiscount == 0
                                            {
                                                dMetalValue = 0;
                                                dDiamondValue = 0;
                                                dDiamondWt = 0;
                                                dStoneValue = 0;
                                                dMetalWt = 0;
                                                dStoneWt = 0;
                                                //saleLineItem.DiscountLines.Clear();
                                                #region Line Discount %
                                                #region Commented for future
                                                DataSet dsIngredients = new DataSet();
                                                StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                                dsIngredients.ReadXml(reader);
                                                int i = 1;
                                                int index = 1;
                                                int iMetalType = 0;
                                                foreach (DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                                {
                                                    index = i;
                                                    iMetalType = Convert.ToInt16(!string.IsNullOrEmpty(Convert.ToString(drIngredients["MetalType"])) ? drIngredients["MetalType"] : "0");

                                                    if ((iMetalType == (int)MetalType.Gold) || (iMetalType == (int)MetalType.Silver)
                                                        || (iMetalType == (int)MetalType.Platinum) || (iMetalType == (int)MetalType.Palladium))
                                                    {
                                                        dMetalValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                        dMetalWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                    }

                                                    if (iMetalType == (int)MetalType.LooseDmd)
                                                    {
                                                        dDiamondValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                        dDiamondWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                    }

                                                    if (iMetalType == (int)MetalType.Stone)
                                                    {
                                                        dStoneValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                        dStoneWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                    }
                                                    i++;
                                                }

                                                #endregion
                                                dinitDiscValue = GetMakDiscount(saleLineItem.ItemId, sStorGrp, ref dtFDate, ref iDiscType, ref iPromoDisctype, ref pMakAgreement, ref pStnAgreement, ref pDiaAgreement);
                                                dinitMakDiscValue = Convert.ToDecimal(saleLineItem.PartnerData.NimMakDisc);
                                                dinitStnDiscValue = Convert.ToDecimal(saleLineItem.PartnerData.NimStnDisc);
                                                dinitDiaDiscValue = Convert.ToDecimal(saleLineItem.PartnerData.NimDiaDisc);

                                                if (dinitMakDiscValue > 0 || dinitStnDiscValue > 0 || dinitDiaDiscValue > 0)
                                                {
                                                    saleLineItem.DiscountLines.Clear();
                                                    #region DiscType.Pct
                                                    if (iDiscType == (int)DiscType.Pct)
                                                    {
                                                        if (iPromoDisctype == (int)DiscPromotionType.LineAmount)
                                                            dDiscAmt = ((Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) * dinitDiscValue) / 100);
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                            dDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) + Convert.ToDecimal(saleLineItem.PartnerData.WastageAmount)) * dinitDiscValue) / 100);
                                                        else if (iPromoDisctype == (int)DiscPromotionType.MakingAndStoneValue)
                                                            dDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) + dStoneValue) * dinitDiscValue) / 100);

                                                        //==================================Soutik========================================
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Stone)
                                                            dStnDiscAmt = ((Convert.ToDecimal(dStoneValue) * dinitStnDiscValue) / 100);
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Diamond)
                                                            dDiaDiscAmt = ((Convert.ToDecimal(dDiamondValue) * dinitDiaDiscValue) / 100);
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Making_Stone)
                                                        {
                                                            dMakDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount)) * dinitMakDiscValue) / 100);
                                                            dStnDiscAmt = (((dStoneValue) * dinitStnDiscValue) / 100);
                                                        }
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Making_Diamond)
                                                        {
                                                            dMakDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount)) * dinitMakDiscValue) / 100);
                                                            dDiaDiscAmt = (((dDiamondValue) * dinitDiaDiscValue) / 100);
                                                        }
                                                        if (iPromoDisctype == (int)DiscPromotionType.Making_Stone_Diamond_Both)
                                                            dMakDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount)) * dinitMakDiscValue) / 100);
                                                        dStnDiscAmt = (((dStoneValue) * dinitStnDiscValue) / 100);
                                                        dDiaDiscAmt = (((dDiamondValue) * dinitDiaDiscValue) / 100);

                                                        dDiscAmt = dMakDiscAmt + dStnDiscAmt + dDiaDiscAmt;

                                                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                        {
                                                            LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                            discountItem.LineDiscountType = DiscountTypes.Customer;
                                                            saleLineItem.Add(discountItem);

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                            {
                                                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                {
                                                                    saleLineItem.PartnerData.MakStnDiaDiscDone = true;
                                                                    transaction.PartnerData.MakStnDiaDiscDone = true;
                                                                }
                                                                else
                                                                {
                                                                    saleLineItem.PartnerData.MakStnDiaDiscDone = false;
                                                                    transaction.PartnerData.MakStnDiaDiscDone = false;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    #region DiscType.Amt
                                                    else if (iDiscType == (int)DiscType.Amt)
                                                    {
                                                        if (iPromoDisctype == (int)DiscPromotionType.LineAmount)
                                                            dDiscAmt = dinitDiscValue; // only dinitDiscValue
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                            dDiscAmt = dMetalWt * dinitDiscValue; // netwt * dinitDiscValue
                                                        else if (iPromoDisctype == (int)DiscPromotionType.MakingAndStoneValue)
                                                            dDiscAmt = (dMetalWt + dStoneWt) * dinitDiscValue; //stwt* dinitDiscValue
                                                        //==========================Soutik=======================================================
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Stone)
                                                            dStnDiscAmt = (dStoneWt) * dinitStnDiscValue;
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Diamond)
                                                            dDiaDiscAmt = (dDiamondWt) * dinitDiaDiscValue;
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Making_Stone)
                                                        {
                                                            dMakDiscAmt = (dMetalWt) * dinitMakDiscValue;
                                                            dStnDiscAmt = (dStoneWt) * dinitStnDiscValue;
                                                        }
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Making_Diamond)
                                                        {
                                                            dMakDiscAmt = (dMetalWt) * dinitMakDiscValue;
                                                            dDiaDiscAmt = (dDiamondWt) * dinitDiaDiscValue;
                                                        }
                                                        else if (iPromoDisctype == (int)DiscPromotionType.Making_Stone_Diamond_Both)
                                                        {
                                                            dMakDiscAmt = (dMetalWt) * dinitMakDiscValue;
                                                            dStnDiscAmt = (dStoneWt) * dinitStnDiscValue;
                                                            dDiaDiscAmt = (dDiamondWt) * dinitDiaDiscValue;
                                                        }

                                                        dDiscAmt = dMakDiscAmt + dStnDiscAmt + dDiaDiscAmt;

                                                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                        {
                                                            LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                            discountItem.LineDiscountType = DiscountTypes.Customer;
                                                            saleLineItem.Add(discountItem);

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                            {
                                                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                {
                                                                    saleLineItem.PartnerData.MakStnDiaDiscDone = true;
                                                                    transaction.PartnerData.MakStnDiaDiscDone = true;
                                                                }
                                                                else
                                                                {
                                                                    saleLineItem.PartnerData.MakStnDiaDiscDone = false;
                                                                    transaction.PartnerData.MakStnDiaDiscDone = false;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    if (dDiscAmt > 0)
                                                    {
                                                        saleLineItem.PartnerData.MakStnDiaDiscType = iPromoDisctype;
                                                        saleLineItem.PartnerData.NimMakingDiscount = dMakDiscAmt;
                                                        saleLineItem.PartnerData.NimStoneDiscount = dStnDiscAmt;
                                                        saleLineItem.PartnerData.NimDiamondDiscount = dDiaDiscAmt;
                                                    }
                                                    else
                                                    {
                                                        saleLineItem.PartnerData.MakStnDiaDiscType = 0;
                                                        saleLineItem.PartnerData.NimMakingDiscount = 0;
                                                        saleLineItem.PartnerData.NimStoneDiscount = 0;
                                                        saleLineItem.PartnerData.NimDiamondDiscount = 0;
                                                    }
                                                }
                                                #endregion Line Discount %
                                            }
                                        }
                                    }
                                    saleLineItem.CalculateLine();
                                    foreach (ITaxItem taxLine in saleLineItem.TaxLines)
                                    {
                                        if (taxLine.Percentage != decimal.Zero)
                                        {
                                            taxLine.Amount = decimal.Round((saleLineItem.NetAmountWithNoTax * taxLine.Percentage / 100), 2, MidpointRounding.AwayFromZero);
                                        }
                                    }
                                    transaction.CalculateAmountDue();
                                    transaction.CalcTotals();
                                }
                                //else
                                //{
                                //    MessageBox.Show("Tier discount already applied");
                                //}
                            }
                        }
                    }
                    #endregion

                    //============================================Soutik=================================================================================================================
                    #region Advance Discount Agreement
                    if (transaction.PartnerData.AdvAgreemetDisc == true && transaction.PartnerData.IsGiftCardIssue == "0" && transaction.PartnerData.APPLYGSSDISCDONE == false)
                    {
                        Decimal dMetalValue = 0;
                        Decimal dMetalWt = 0;
                        decimal dinitDiscValue = 0;
                        DateTime dtFDate = DateTime.Now;
                        DateTime dtToDate = DateTime.Now;
                        int iFdays = 0;
                        int iTdays = 0;
                        int iDiscType = 0;
                        DateTime CustAdvDepositDate = DateTime.Now;
                        int NoOfDaysDiff = 0;

                        string sStorGrp = getStoreFormatCode();

                        if (transaction.PartnerData.CustDepositDate != "")
                            CustAdvDepositDate = Convert.ToDateTime(transaction.PartnerData.CustDepositDate);

                        foreach (SaleLineItem saleLineItem in transaction.SaleItems)
                        {
                            dDiscAmt = 0m;
                            if (Convert.ToInt16(saleLineItem.ItemType) != 2)
                            {
                                if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)))
                                {
                                    if (!saleLineItem.Voided)
                                    {
                                        if (saleLineItem.PartnerData.TransactionType == Convert.ToString((int)TransactionType.Sale))
                                        {
                                            if (saleLineItem.ItemId == saleLineItem.ItemId && saleLineItem.LineId != 1 && saleLineItem.PartnerData.AdvAgreemetDiscDone == false)//&& saleItem.LinePctDiscount == 0
                                            {
                                                dMetalValue = 0;
                                                dMetalWt = 0;
                                                saleLineItem.DiscountLines.Clear();
                                                #region Line Discount %
                                                #region Commented for future
                                                DataSet dsIngredients = new DataSet();
                                                StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                                dsIngredients.ReadXml(reader);
                                                int i = 1;
                                                int index = 1;
                                                int iMetalType = 0;
                                                foreach (DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                                {
                                                    index = i;
                                                    iMetalType = Convert.ToInt16(!string.IsNullOrEmpty(Convert.ToString(drIngredients["MetalType"])) ? drIngredients["MetalType"] : "0");

                                                    if ((iMetalType == (int)MetalType.Gold) || (iMetalType == (int)MetalType.Silver)
                                                        || (iMetalType == (int)MetalType.Platinum) || (iMetalType == (int)MetalType.Palladium))
                                                    {
                                                        dMetalValue += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : "0");
                                                        dMetalWt += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                    }
                                                    i++;
                                                }

                                                #endregion
                                                dinitDiscValue = GetAdvanceDiscountAgreement(saleLineItem.ItemId, ref dtFDate, ref dtToDate, ref iPromoDisctype, ref iFdays, ref iTdays);
                                                //====================Get Date difference beyween Deposit Date to Today===============================
                                                NoOfDaysDiff = Convert.ToInt16((Convert.ToDateTime(DateTime.Now.ToShortDateString()) - CustAdvDepositDate).TotalDays); //Store Date Difference

                                                //if (dinitDiscValue > 0 && (iFdays >= NoOfDaysDiff && iTdays <= NoOfDaysDiff))  ---Ask Ripan Da 
                                                if (dinitDiscValue > 0 && (NoOfDaysDiff >= iFdays && NoOfDaysDiff <= iTdays))
                                                {
                                                    #region DiscType.Pct
                                                    if (iDiscType == (int)DiscType.Pct)
                                                    {
                                                        if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                            dDiscAmt = (((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) + Convert.ToDecimal(saleLineItem.PartnerData.WastageAmount)) * dinitDiscValue) / 100);

                                                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                        {
                                                            LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                            discountItem.LineDiscountType = DiscountTypes.Customer;
                                                            saleLineItem.Add(discountItem);

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                            {
                                                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                {
                                                                    saleLineItem.PartnerData.AdvAgreemetDiscDone = true;
                                                                    transaction.PartnerData.AdvAgreemetDiscDone = true;
                                                                }
                                                                else
                                                                {
                                                                    saleLineItem.PartnerData.AdvAgreemetDiscDone = false;
                                                                    transaction.PartnerData.AdvAgreemetDiscDone = false;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    #region DiscType.Amt
                                                    else if (iDiscType == (int)DiscType.Amt)
                                                    {
                                                        if (iPromoDisctype == (int)DiscPromotionType.Making)
                                                            dDiscAmt = dMetalWt * dinitDiscValue; // netwt * dinitDiscValue

                                                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                                                        {
                                                            LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                            discountItem.LineDiscountType = DiscountTypes.Customer;
                                                            saleLineItem.Add(discountItem);

                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                            {
                                                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                {
                                                                    saleLineItem.PartnerData.AdvAgreemetDiscDone = true;
                                                                    transaction.PartnerData.AdvAgreemetDiscDone = true;
                                                                }
                                                                else
                                                                {
                                                                    saleLineItem.PartnerData.AdvAgreemetDiscDone = false;
                                                                    transaction.PartnerData.AdvAgreemetDiscDone = false;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    #endregion

                                                    if (dDiscAmt > 0)
                                                    {
                                                        saleLineItem.PartnerData.MakStnDiaDiscType = iPromoDisctype;
                                                        saleLineItem.PartnerData.NimMakingDisc = dDiscAmt;
                                                    }
                                                    else
                                                    {
                                                        saleLineItem.PartnerData.MakStnDiaDiscType = iPromoDisctype;
                                                        saleLineItem.PartnerData.NimMakingDisc = 0;
                                                    }
                                                }
                                                #endregion Line Discount %
                                            }
                                        }
                                    }
                                    saleLineItem.CalculateLine();
                                    foreach (ITaxItem taxLine in saleLineItem.TaxLines)
                                    {
                                        if (taxLine.Percentage != decimal.Zero)
                                        {
                                            taxLine.Amount = decimal.Round((saleLineItem.NetAmountWithNoTax * taxLine.Percentage / 100), 2, MidpointRounding.AwayFromZero);
                                        }
                                    }
                                    transaction.CalculateAmountDue();
                                    transaction.CalcTotals();
                                }
                                else
                                {
                                    MessageBox.Show("Advance discount already applied");
                                }
                            }
                        }
                    }
                    #endregion
                    //===================================================================================================================================================================
                    objTax.Initialize(application);
                    objTax.CalculateTax(transaction);
                }
                else if (Convert.ToDecimal(transaction.PartnerData.ExtraMkDiscPct) > 0)
                {
                    #region Making disc
                    if (transaction.PartnerData.IsGiftCardIssue == "0"
                        && transaction.PartnerData.APPLYGSSDISCDONE == false)
                    {
                        SaleLineItem saleItem = transaction.SaleItems.Last.Value;
                        decimal dinitDiscValue = 0;
                        DateTime dtFDate = DateTime.Now;

                        dDiscAmt = 0m;

                        foreach (SaleLineItem saleLineItem in transaction.SaleItems)
                        {
                            if (Convert.ToInt16(saleLineItem.ItemType) != 2)
                            {
                                if (saleLineItem.LinePctDiscount == 0)
                                {
                                    #region NimMakingDisc
                                    if (!saleLineItem.Voided)
                                    {
                                        if (saleLineItem.PartnerData.TransactionType == Convert.ToString((int)TransactionType.Sale))
                                        {
                                            dinitDiscValue = Convert.ToDecimal(transaction.PartnerData.ExtraMkDiscPct);

                                            LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                                            if (Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) > 0)
                                            {
                                                decimal dMkDisc = 0m;//
                                                decimal dLineMkRate = 0m;
                                                decimal dLineGoldValue = Convert.ToDecimal(saleLineItem.PartnerData.LINEGOLDVALUE);//changes on 19/07/19 RH
                                                
                                                decimal dLineActMkRate = Convert.ToDecimal(saleLineItem.PartnerData.ACTMKRATE);
                                                decimal dLineChangedMkRate = Convert.ToDecimal(saleLineItem.PartnerData.MakingRate);

                                                if (dLineChangedMkRate > 0)
                                                    dLineMkRate = dLineChangedMkRate;
                                                else
                                                    dLineMkRate = dLineActMkRate;

                                                decimal dActMkValue = 0m;

                                                dActMkValue = ((dLineGoldValue * dLineMkRate) / 100);

                                                //dMkDisc = ((Convert.ToDecimal(saleLineItem.PartnerData.MakingAmount) * dinitDiscValue) / 100);

                                                dMkDisc = dActMkValue - ((dLineGoldValue * (dLineMkRate - dinitDiscValue)) / 100);

                                                #region Cost price validation

                                                decimal dSalesValue = 0m;
                                                // int iMetalType = GetMetalType(sBaseItemID);

                                                string sSqr = "select isnull(NOCOSTVALIDATERETAIL,0) from INVENTTABLE where itemid='" + saleLineItem.ItemId + "'";
                                                string sResultForNOCOSTVALIDATERETAIL = NIM_ReturnExecuteScalar(sSqr);

                                                if (sResultForNOCOSTVALIDATERETAIL == "0")
                                                {
                                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                    {
                                                        dSalesValue = Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                                        dSalesValue = dSalesValue - dLineGoldValue;

                                                        if (!getCostPrice(saleLineItem.ItemId, dSalesValue - dMkDisc))
                                                        {
                                                            MessageBox.Show("Sales price can not be less than cost price.", "Sales price can not be less than cost price.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                        }
                                                        else
                                                        {
                                                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                                dinitDiscValue = (dMkDisc * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);

                                                            discountItem.Percentage = dinitDiscValue;
                                                            discountItem.LineDiscountType = DiscountTypes.Customer;
                                                            saleLineItem.Add(discountItem);
                                                        }
                                                    }
                                                }
                                                #endregion


                                            }
                                        }
                                        saleLineItem.CalculateLine();
                                        foreach (ITaxItem taxLine in saleLineItem.TaxLines)
                                        {
                                            if (taxLine.Percentage != decimal.Zero)
                                            {
                                                taxLine.Amount = decimal.Round((saleLineItem.NetAmountWithNoTax * taxLine.Percentage / 100), 2, MidpointRounding.AwayFromZero);
                                            }
                                        }
                                        transaction.CalculateAmountDue();
                                        transaction.CalcTotals();
                                    }
                                    #endregion
                                }
                            }
                        }
                    }
                    #endregion

                    objTax.Initialize(application);
                    objTax.CalculateTax(transaction);
                }
            }
            //End
        }

        private bool getCostPrice(string sItemId, decimal dSalesValue)
        {
            bool bReturn = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            //added CRWSTONETRANSFERCOST on 060818
            string commandText = "select ABS(CAST(ISNULL(CostPrice + TRANSFERCOSTPRICE + CRWSTONETRANSFERCOST,0)AS DECIMAL(28,2)))  from SKUTABLE_POSTED WHERE skunumber = '" + sItemId + "'";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            decimal _dSalesValue = Convert.ToDecimal(command.ExecuteScalar());

            if (_dSalesValue <= dSalesValue)
                bReturn = true;
            else
                bReturn = false;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bReturn;
        }

        public string NIM_ReturnExecuteScalar(string query)
        {
            SqlCommand cmd = new SqlCommand();
            SqlConnection myCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            myCon.Open();

            try
            {
                if (myCon.State == ConnectionState.Closed)
                    myCon.Open();
                cmd = new SqlCommand(query, myCon);
                return Convert.ToString(cmd.ExecuteScalar());
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (myCon.State == ConnectionState.Open)
                    myCon.Close();
            }
        }

        private decimal GetMakDiscount(string sItemId, string sStoreGrp, ref DateTime dtFROMDATE, ref int iDISCTYPE, ref int iDsicOn, ref decimal iMakAgr, ref decimal iStnAgr, ref decimal iDiaAgr)
        {
            decimal dResult = 0;

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " DECLARE @Temp TABLE (DISCOUNT numeric(20,2), FROMDATE DATE,CLUBBED int,DISCTYPE int,FROMDAYS int,TODAYS int,DISCOUNTON int,MAKDISCOUNT numeric(20,2),STNDISCOUNT numeric(20,2),DIADISCOUNT numeric(20,2));" +
                                 "  DECLARE @LOCATION VARCHAR(20) " +
                                 "  DECLARE @PRODUCT BIGINT " +
                                 "  DECLARE @PARENTITEM VARCHAR(20) " +
                                 "  DECLARE @CUSTCODE VARCHAR(20) " +
                                 "  DECLARE @PRODUCTCODE VARCHAR(20) " +
                                 "  DECLARE @COLLECTIONCODE VARCHAR(20) " +
                                 "  DECLARE @ARTICLE_CODE VARCHAR(20) " +
                                 "  IF EXISTS(SELECT TOP(1) ACTIVATE FROM RetailMakingStoneDiamondAgreement SAD )" + // WHERE RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'
                                 "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "')" + // AND RETAIL=1 req by S.Sharma on 15/02/17
                                 "  BEGIN SELECT @PARENTITEM = ITEMIDPARENT,@PRODUCTCODE=PRODUCTTYPECODE, " +
                                 "  @ARTICLE_CODE=ARTICLE_CODE , @COLLECTIONCODE =CollectionCode" +
                                 "  FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                                 "  END " +
                                 "  ELSE" +
                                 "  BEGIN" +
                                 "  SET @PARENTITEM='" + sItemId + "' " +
                                 "  END" +
                                 "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "' AND RETAIL=0)" + // AND RETAIL=0 req by S.Sharma on 22/05/17
                                 "  BEGIN " +
                                 "  SET @PARENTITEM='" + sItemId + "'" +
                                 "  END ";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE  FROM   RetailMakingStoneDiamondAgreement  SAD WHERE (SAD.ITEMID=@PARENTITEM) " +
                         " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                         " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
                         " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                         " BEGIN " +
                         " INSERT INTO @Temp (DISCOUNT, FROMDATE,DISCTYPE,DISCOUNTON,MAKDISCOUNT,STNDISCOUNT,DIADISCOUNT)  " +
                         " SELECT  TOP (1) (CAST(Isnull(SAD.DISCAMTMAKING,0) AS decimal(18, 2))),FROMDATE,ISNULL(DISCTYPE,0) as DISCTYPE,ISNULL(DISCOUNTON,0) as DISCOUNTON, " +
                         " (CAST(Isnull(SAD.DISCAMTMAKING,0) AS decimal(18, 2))),(CAST(Isnull(SAD.DISCAMTSTONE,0) AS decimal(18, 2))),(CAST(Isnull(SAD.DISCAMTDIAMOND,0) AS decimal(18, 2))) " +
                         " FROM   RetailMakingStoneDiamondAgreement  SAD  WHERE (SAD.ITEMID=@PARENTITEM or SAD.ItemId='') " +
                         " AND   (SAD.ProductTypeCode=@ProductCode  or SAD.ProductTypeCode='')" +
                         " AND   (SAD.Collection=@CollectionCode  or SAD.Collection='')" +
                         " AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +
                         " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                         " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
                         " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)   " +
                         " AND SAD.ACTIVATE = 1" +
                         " ORDER BY SAD.ITEMID DESC " +
                         " END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //2
                         " FROM    RetailMakingStoneDiamondAgreement SAD  " +
                         " WHERE (SAD.ITEMID=@PARENTITEM) " +
                         " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                         " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
                         " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                         " BEGIN " +
                         " INSERT INTO @Temp (DISCOUNT, FROMDATE,DISCTYPE,DISCOUNTON,MAKDISCOUNT,STNDISCOUNT,DIADISCOUNT)  " +
                         " SELECT  TOP (1) (CAST(Isnull(SAD.DISCAMTMAKING,0) AS decimal(18, 2))), " +
                         " FROMDATE,ISNULL(DISCTYPE,0) as DISCTYPE," +
                         " ISNULL(DISCOUNTON,0) as DISCOUNTON, " +
                          " (CAST(Isnull(SAD.DISCAMTMAKING,0) AS decimal(18, 2))),(CAST(Isnull(SAD.DISCAMTSTONE,0) AS decimal(18, 2))),(CAST(Isnull(SAD.DISCAMTDIAMOND,0) AS decimal(18, 2))) " +
                         " FROM   RetailMakingStoneDiamondAgreement  SAD  " +
                         " WHERE     " +
                         " (SAD.ITEMID=@PARENTITEM or SAD.ITEMID='') " +
                         " AND   (SAD.ProductTypeCode=@ProductCode  or SAD.ProductTypeCode='')" +
                         " AND   (SAD.Collection=@CollectionCode  or SAD.Collection='')" +
                         " AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +
                         " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                         " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
                         " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)" +
                         " AND SAD.ACTIVATE = 1" +
                         " ORDER BY SAD.ITEMID DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //3
                        " FROM    RetailMakingStoneDiamondAgreement SAD  " +
                        " WHERE (SAD.ITEMID='') " +
                        " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                        " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
                        " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                        " BEGIN " +
                        " INSERT INTO @Temp (DISCOUNT, FROMDATE ,DISCTYPE,DISCOUNTON,MAKDISCOUNT,STNDISCOUNT,DIADISCOUNT)  " +
                        " SELECT  TOP (1) (CAST(Isnull(SAD.DISCAMTMAKING,0) AS decimal(18, 2))), " +
                        " FROMDATE,ISNULL(DISCTYPE,0) as DISCTYPE," +
                        " ISNULL(DISCOUNTON,0) as DISCOUNTON, " +
                        " (CAST(Isnull(SAD.DISCAMTMAKING,0) AS decimal(18, 2))),(CAST(Isnull(SAD.DISCAMTSTONE,0) AS decimal(18, 2))),(CAST(Isnull(SAD.DISCAMTDIAMOND,0) AS decimal(18, 2))) " +
                        " FROM  RetailMakingStoneDiamondAgreement SAD " +
                        " WHERE     " +
                        " (SAD.ITEMID='' or SAD.ITEMID='' )" +
                        " AND   (SAD.ProductTypeCode=@ProductCode  or SAD.ProductTypeCode='')" +
                        " AND   (SAD.Collection=@CollectionCode  or SAD.Collection='')" +
                        " AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +
                        " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                        " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
                        " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)   " +
                        " AND SAD.ACTIVATE = 1" +
                        " ORDER BY SAD.ITEMID DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //4
                        " FROM   RetailMakingStoneDiamondAgreement SAD " +
                        " WHERE (SAD.ITEMID='') " +
                        " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                        " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
                        " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                        " BEGIN " +
                        " INSERT INTO @Temp (DISCOUNT, FROMDATE ,DISCTYPE,DISCOUNTON,MAKDISCOUNT,STNDISCOUNT,DIADISCOUNT)  " +
                        " SELECT  TOP (1) (CAST(Isnull(SAD.DISCAMTMAKING,0) AS decimal(18, 2))), " +
                        " FROMDATE,ISNULL(DISCTYPE,0) as DISCTYPE," +
                        " ISNULL(DISCOUNTON,0) as DISCOUNTON, " +
                        " (CAST(Isnull(SAD.DISCAMTMAKING,0) AS decimal(18, 2))),(CAST(Isnull(SAD.DISCAMTSTONE,0) AS decimal(18, 2))),(CAST(Isnull(SAD.DISCAMTDIAMOND,0) AS decimal(18, 2))) " +
                        " FROM RetailMakingStoneDiamondAgreement SAD  " +
                        " WHERE     " +
                        " (SAD.ITEMID='' or SAD.ITEMID='')  " +
                        " AND   (SAD.ProductTypeCode=@ProductCode  or SAD.ProductTypeCode='')" +
                        " AND   (SAD.Collection=@CollectionCode  or SAD.Collection='')" +
                        " AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +
                        " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                        " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
                        " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)   " +
                        " AND SAD.ACTIVATE = 1" +
                        " ORDER BY SAD.ITEMID DESC END";
            commandText += " select TOP 1 * from @Temp";

            SqlCommand commandMk = new SqlCommand(commandText.ToString(), connection);
            commandMk.CommandTimeout = 0;

            using (SqlDataReader mkDiscRd = commandMk.ExecuteReader())
            {
                while (mkDiscRd.Read())
                {

                    dResult = Convert.ToDecimal(mkDiscRd.GetValue(0));
                    dtFROMDATE = Convert.ToDateTime(mkDiscRd.GetValue(1));
                    //iCLUBBED = Convert.ToInt16(mkDiscRd.GetValue(2));
                    iDISCTYPE = Convert.ToInt16(mkDiscRd.GetValue(3));
                    //iFROMDAYS = Convert.ToInt16(mkDiscRd.GetValue(4));
                    //iTODAYS = Convert.ToInt16(mkDiscRd.GetValue(5));
                    iDsicOn = Convert.ToInt16(mkDiscRd.GetValue(6));

                    iMakAgr = Convert.ToDecimal(mkDiscRd.GetValue(7));
                    iStnAgr = Convert.ToDecimal(mkDiscRd.GetValue(8));
                    iDiaAgr = Convert.ToDecimal(mkDiscRd.GetValue(9));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();


            return dResult;
        }

        private decimal GetAdvanceDiscountAgreement(string sItemId, ref DateTime dtFROMDATE, ref DateTime dtTODATE, ref int iDISCTYPE, ref int iFROMDAYS, ref int iTODAYS)
        {
            decimal dResult = 0;

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " DECLARE @Temp TABLE (DISCOUNT numeric(20,2), FROMDATE DATE,TODATE DATE,DISCTYPE int,FROMDAYS int,TODAYS int);" +
                                 "  DECLARE @LOCATION VARCHAR(20) " +
                                 "  DECLARE @PRODUCT BIGINT " +
                                 "  DECLARE @PARENTITEM VARCHAR(20) " +
                                 "  DECLARE @CUSTCODE VARCHAR(20) " +

                                 "  DECLARE @PRODUCTCODE VARCHAR(20) " +
                                 "  DECLARE @COLLECTIONCODE VARCHAR(20) " +
                                 "  DECLARE @ARTICLE_CODE VARCHAR(20) " +

                                 "  IF EXISTS(SELECT TOP(1) ACTIVE FROM CRWADVANCEDISCOUNTAGREEMENT SAD )" + // WHERE RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'
                                 "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "')" + // AND RETAIL=1 req by S.Sharma on 15/02/17
                                 "  BEGIN SELECT @PARENTITEM = ITEMIDPARENT,@PRODUCTCODE=PRODUCTTYPECODE" +
                                 "  ,@ARTICLE_CODE=ARTICLE_CODE , @COLLECTIONCODE =CollectionCode" +
                                 " FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                                 " END " +
                                 " ELSE" +
                                 " BEGIN" +
                                 " SET @PARENTITEM='" + sItemId + "' " +
                                 " END" +

                                 " IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "' AND RETAIL=0)" + // AND RETAIL=0 req by S.Sharma on 22/05/17
                                 " BEGIN " +
                                 " SET @PARENTITEM='" + sItemId + "'" +
                                 " END ";


            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVE FROM CRWADVANCEDISCOUNTAGREEMENT  SAD" +   //1
                           " WHERE (SAD.ProductType=@ProductCode  or SAD.ProductType='') AND " +
                           " (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                           " BEGIN " +
                           " INSERT INTO @Temp (DISCOUNT, FROMDATE,TODATE,DISCTYPE,FROMDAYS,TODAYS)  " +
                           " SELECT  TOP (1) CAST(SAD.DISCOUNT AS decimal(18, 2))," +
                           " FROMDATE,TODATE, 0 as DISCTYPE,FROMDAYS,TODAYS " +
                           " FROM   CRWADVANCEDISCOUNTAGREEMENT  SAD  WHERE (SAD.ProductType=@ProductCode  or SAD.ProductType='')" +
                           " AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +
                           " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)   " +
                           " AND SAD.ACTIVE = 1 ORDER BY SAD.ProductType DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVE FROM CRWADVANCEDISCOUNTAGREEMENT SAD  " + //2
                           " WHERE (SAD.ProductType=@ProductCode  or SAD.ProductType='') AND " +
                           " (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                           " BEGIN " +
                           " INSERT INTO @Temp (DISCOUNT, FROMDATE,TODATE,DISCTYPE,FROMDAYS,TODAYS)  " +
                           " SELECT  TOP (1) CAST(SAD.DISCOUNT AS decimal(18, 2))," +
                           " FROMDATE,TODATE,0 as DISCTYPE,FROMDAYS,TODAYS FROM   CRWADVANCEDISCOUNTAGREEMENT  SAD  " +
                           " WHERE (SAD.ProductType=@ProductCode  or SAD.ProductType='')" +
                           " AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +
                           " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)" +
                           " AND SAD.ACTIVE = 1 ORDER BY SAD.ProductType DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVE " + //3
                        " FROM    CRWADVANCEDISCOUNTAGREEMENT SAD  " +
                        " WHERE (SAD.ProductType='') " +
                        " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                        " BEGIN " +
                        " INSERT INTO @Temp (DISCOUNT, FROMDATE,TODATE ,DISCTYPE,FROMDAYS,TODAYS)  " +
                        " SELECT  TOP (1) CAST(SAD.DISCOUNT AS decimal(18, 2))," +
                        " FROMDATE,TODATE,0 as DISCTYPE," +
                        " FROMDAYS,TODAYS FROM  CRWADVANCEDISCOUNTAGREEMENT SAD " +
                        " WHERE     " +
                        " (SAD.ProductType='' )" +
                        " AND   (SAD.ProductType=@ProductCode  or SAD.ProductType='')" +
                        " AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +
                        " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)   " +
                        " AND SAD.ACTIVE = 1" +
                        " ORDER BY SAD.ProductType DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVE" + //4
                        " FROM   CRWADVANCEDISCOUNTAGREEMENT SAD " +
                        " WHERE (SAD.ProductType='') " +
                        " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                        " BEGIN " +
                        " INSERT INTO @Temp (DISCOUNT, FROMDATE,TODATE,DISCTYPE,FROMDAYS,TODAYS)  " +
                        " SELECT  TOP (1) CAST(SAD.DISCOUNT AS decimal(18, 2))," +
                        " FROMDATE,TODATE,0 as DISCTYPE," +
                        " FROMDAYS,TODAYS FROM CRWADVANCEDISCOUNTAGREEMENT SAD  " +
                        " WHERE     " +
                        " (SAD.ProductType='' )  " +
                        " AND   (SAD.ProductType=@ProductCode  or SAD.ProductType='')" +
                        " AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +
                        " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)   " +
                        " AND SAD.ACTIVE = 1 ORDER BY SAD.ProductType DESC END";


            commandText += " select TOP 1 * from @Temp";

            SqlCommand commandMk = new SqlCommand(commandText.ToString(), connection);
            commandMk.CommandTimeout = 0;

            using (SqlDataReader mkDiscRd = commandMk.ExecuteReader())
            {
                while (mkDiscRd.Read())
                {
                    dResult = Convert.ToDecimal(mkDiscRd.GetValue(0));
                    dtFROMDATE = Convert.ToDateTime(mkDiscRd.GetValue(1));
                    dtTODATE = Convert.ToDateTime(mkDiscRd.GetValue(2));
                    iDISCTYPE = 0;
                    iFROMDAYS = Convert.ToInt16(mkDiscRd.GetValue(4));
                    iTODAYS = Convert.ToInt16(mkDiscRd.GetValue(5));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();


            return dResult;
        }

        private void CalMRPDiscount(decimal dinitDiscValue, int isClubbed, SaleLineItem saleLineItem)
        {
            if (iCalcDiscType == (int)DiscType.Pct)
            {
                #region DiscType.Pct
                dDiscAmt = dinitDiscValue;

                if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                {
                    if (isClubbed == 0)
                    {
                        LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();

                        decimal dNewWouldBEDiscPct = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                        if (saleLineItem.LineDiscount > dDiscAmt)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Existing discount is greater than current discount for the item '" + saleLineItem.ItemId + "',Do you want to override the discount ? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                                {
                                    saleLineItem.DiscountLines.Clear();
                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                    {
                                        discountItem.Percentage = dNewWouldBEDiscPct;
                                        discountItem.LineDiscountType = DiscountTypes.Customer;
                                        saleLineItem.Add(discountItem);

                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                            saleLineItem.PartnerData.NimMRPDiscTypeDone = true;
                                        else
                                            saleLineItem.PartnerData.NimMRPDiscTypeDone = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                            {
                                saleLineItem.DiscountLines.Clear();

                                discountItem.Percentage = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                discountItem.LineDiscountType = DiscountTypes.Customer;
                                saleLineItem.Add(discountItem);


                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                    saleLineItem.PartnerData.NimMRPDiscTypeDone = true;
                                else
                                    saleLineItem.PartnerData.NimMRPDiscTypeDone = false;
                            }
                        }
                    }
                    else
                    {
                        LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                        {
                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                            discountItem.LineDiscountType = DiscountTypes.Customer;
                            saleLineItem.Add(discountItem);

                            if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                saleLineItem.PartnerData.NimMRPDiscTypeDone = true;
                            else
                                saleLineItem.PartnerData.NimMRPDiscTypeDone = false;
                        }
                    }
                }
                #endregion
            }
            else if (iCalcDiscType == (int)DiscType.Amt)
            {
                #region DiscType.Amt

                dDiscAmt = dinitDiscValue;

                if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                {
                    if (isClubbed == 0)
                    {
                        LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();

                        decimal dNewWouldBEDiscPct = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                        if (saleLineItem.LineDiscount > dDiscAmt)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Existing discount is greater than current discount,Are you want to override the discount ? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                                {
                                    saleLineItem.DiscountLines.Clear();

                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                    {
                                        discountItem.Percentage = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                        discountItem.LineDiscountType = DiscountTypes.Customer;
                                        saleLineItem.Add(discountItem);


                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                            saleLineItem.PartnerData.NimMRPDiscTypeDone = true;
                                        else
                                            saleLineItem.PartnerData.NimMRPDiscTypeDone = false;
                                    }

                                }
                            }
                        }
                        else
                        {
                            saleLineItem.DiscountLines.Clear();

                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                            {
                                discountItem.Percentage = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                discountItem.LineDiscountType = DiscountTypes.Customer;
                                saleLineItem.Add(discountItem);


                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                    saleLineItem.PartnerData.NimMRPDiscTypeDone = true;
                                else
                                    saleLineItem.PartnerData.NimMRPDiscTypeDone = false;
                            }
                        }
                    }
                    else
                    {
                        LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                            discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                        discountItem.LineDiscountType = DiscountTypes.Customer;
                        saleLineItem.Add(discountItem);

                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                        {
                            if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                saleLineItem.PartnerData.NimMRPDiscTypeDone = true;
                            else
                                saleLineItem.PartnerData.NimMRPDiscTypeDone = false;
                        }
                    }
                }
                #endregion
            }
            if (dDiscAmt > 0)
            {
                //saleLineItem.PartnerData.PROMOCODE = saleLineItem.PartnerData.NIMPROMOCODE;
                saleLineItem.PartnerData.NimMRPDiscType = true;
                saleLineItem.PartnerData.NimMRPDisc = dDiscAmt;
            }
            else
            {
                //saleLineItem.PartnerData.PROMOCODE = "";
                saleLineItem.PartnerData.NimMRPDiscType = false;
                saleLineItem.PartnerData.NimMRPDisc = 0;
            }

        }

        private void CalMakingDiscount(decimal dinitDiscValue, int isClubbed, SaleLineItem saleLineItem)
        {
            if (iCalcDiscType == (int)DiscType.Pct)
            {
                #region DiscType.Pct
                dDiscAmt = dinitDiscValue;

                if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                {
                    if (isClubbed == 0)
                    {
                        LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                        {
                            decimal dNewWouldBEDiscPct = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                            if (saleLineItem.LineDiscount > dDiscAmt)
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Existing discount is greater than current discount for the item '" + saleLineItem.ItemId + "',Do you want to override the discount ? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                                    {
                                        saleLineItem.DiscountLines.Clear();

                                        discountItem.Percentage = dNewWouldBEDiscPct;
                                        discountItem.LineDiscountType = DiscountTypes.Customer;
                                        saleLineItem.Add(discountItem);

                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                            saleLineItem.PartnerData.NimMakingDiscTypeDone = true;
                                        else
                                            saleLineItem.PartnerData.NimMakingDiscTypeDone = false;
                                    }
                                }
                            }
                            else
                            {
                                saleLineItem.DiscountLines.Clear();

                                discountItem.Percentage = dNewWouldBEDiscPct;
                                discountItem.LineDiscountType = DiscountTypes.Customer;
                                saleLineItem.Add(discountItem);

                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                    saleLineItem.PartnerData.NimMakingDiscTypeDone = true;
                                else
                                    saleLineItem.PartnerData.NimMakingDiscTypeDone = false;
                            }
                        }
                    }
                    else
                    {
                        LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                        {
                            discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                            discountItem.LineDiscountType = DiscountTypes.Customer;
                            saleLineItem.Add(discountItem);

                            if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                saleLineItem.PartnerData.NimMakingDiscTypeDone = true;
                            else
                                saleLineItem.PartnerData.NimMakingDiscTypeDone = false;
                        }
                    }
                }
                #endregion
            }
            else if (iCalcDiscType == (int)DiscType.Amt)
            {
                #region DiscType.Amt
                dDiscAmt = dinitDiscValue;

                if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                {
                    if (isClubbed == 0)
                    {
                        LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                        decimal dNewWouldBEDiscPct = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                        if (saleLineItem.LineDiscount > dDiscAmt)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Existing discount is greater than current discount,Are you want to override the discount ? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                                {
                                    saleLineItem.DiscountLines.Clear();

                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                    {
                                        discountItem.Percentage = dNewWouldBEDiscPct;// (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                        discountItem.LineDiscountType = DiscountTypes.Customer;
                                        saleLineItem.Add(discountItem);

                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                            saleLineItem.PartnerData.NimMakingDiscTypeDone = true;
                                        else
                                            saleLineItem.PartnerData.NimMakingDiscTypeDone = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                            {
                                saleLineItem.DiscountLines.Clear();
                                discountItem.Percentage = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                discountItem.LineDiscountType = DiscountTypes.Customer;
                                saleLineItem.Add(discountItem);

                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                    saleLineItem.PartnerData.NimMakingDiscTypeDone = true;
                                else
                                    saleLineItem.PartnerData.NimMakingDiscTypeDone = false;
                            }
                        }
                    }
                    else
                    {
                        LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                            discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                        discountItem.LineDiscountType = DiscountTypes.Customer;
                        saleLineItem.Add(discountItem);

                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                        {
                            if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                saleLineItem.PartnerData.NimMakingDiscTypeDone = true;
                            else
                                saleLineItem.PartnerData.NimMakingDiscTypeDone = false;
                        }
                    }
                }
                #endregion
            }
            if (dDiscAmt > 0)
            {
                // saleLineItem.PartnerData.PROMOCODE = saleLineItem.PartnerData.NIMPROMOCODE;
                saleLineItem.PartnerData.NimMakingDiscType = true;
                saleLineItem.PartnerData.NimMakingDisc = dinitDiscValue;
            }
            else
            {
                // saleLineItem.PartnerData.PROMOCODE = "";
                saleLineItem.PartnerData.NimMakingDiscType = false;
                saleLineItem.PartnerData.NimMakingDisc = 0;
            }
        }

        private void CalPromoMRPDiscount(decimal dinitDiscValue, int isClubbed, SaleLineItem saleLineItem)
        {
            if (string.IsNullOrEmpty(saleLineItem.PartnerData.PROMOCODE))
            {
                if (iCalcDiscType == (int)DiscType.Pct)
                {
                    #region DiscType.Pct
                    dDiscAmt = dinitDiscValue;

                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                    {
                        if (isClubbed == 0)
                        {
                            LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();

                            decimal dNewWouldBEDiscPct = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                            if (saleLineItem.LineDiscount > dDiscAmt)
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Existing discount is greater than current discount for the item '" + saleLineItem.ItemId + "',Do you want to override the discount ? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                                    {
                                        saleLineItem.DiscountLines.Clear();
                                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                        {
                                            discountItem.Percentage = dNewWouldBEDiscPct;
                                            discountItem.LineDiscountType = DiscountTypes.Customer;
                                            saleLineItem.Add(discountItem);

                                            if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                saleLineItem.PartnerData.NimPromoMRPDiscTypeDone = true;
                                            else
                                                saleLineItem.PartnerData.NimPromoMRPDiscTypeDone = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                {
                                    saleLineItem.DiscountLines.Clear();

                                    discountItem.Percentage = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                    discountItem.LineDiscountType = DiscountTypes.Customer;
                                    saleLineItem.Add(discountItem);


                                    if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                        saleLineItem.PartnerData.NimPromoMRPDiscTypeDone = true;
                                    else
                                        saleLineItem.PartnerData.NimPromoMRPDiscTypeDone = false;
                                }
                            }
                        }
                        else
                        {
                            LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                            {
                                if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                    discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                discountItem.LineDiscountType = DiscountTypes.Customer;
                                saleLineItem.Add(discountItem);

                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                    saleLineItem.PartnerData.NimPromoMRPDiscTypeDone = true;
                                else
                                    saleLineItem.PartnerData.NimPromoMRPDiscTypeDone = false;
                            }
                        }
                    }
                    #endregion
                }
                else if (iCalcDiscType == (int)DiscType.Amt)
                {
                    #region DiscType.Amt

                    dDiscAmt = dinitDiscValue;

                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                    {
                        if (isClubbed == 0)
                        {
                            LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();

                            decimal dNewWouldBEDiscPct = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                            if (saleLineItem.LineDiscount > dDiscAmt)
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Existing discount is greater than current discount,Are you want to override the discount ? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                                    {
                                        saleLineItem.DiscountLines.Clear();

                                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                        {
                                            discountItem.Percentage = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                            discountItem.LineDiscountType = DiscountTypes.Customer;
                                            saleLineItem.Add(discountItem);


                                            if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                                saleLineItem.PartnerData.NimPromoMRPDiscTypeDone = true;
                                            else
                                                saleLineItem.PartnerData.NimPromoMRPDiscTypeDone = false;
                                        }

                                    }
                                }
                            }
                            else
                            {
                                saleLineItem.DiscountLines.Clear();

                                if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                {
                                    discountItem.Percentage = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                    discountItem.LineDiscountType = DiscountTypes.Customer;
                                    saleLineItem.Add(discountItem);


                                    if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                        saleLineItem.PartnerData.NimPromoMRPDiscTypeDone = true;
                                    else
                                        saleLineItem.PartnerData.NimPromoMRPDiscTypeDone = false;
                                }
                            }
                        }
                        else
                        {
                            LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                            discountItem.LineDiscountType = DiscountTypes.Customer;
                            saleLineItem.Add(discountItem);

                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                            {
                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                    saleLineItem.PartnerData.NimPromoMRPDiscTypeDone = true;
                                else
                                    saleLineItem.PartnerData.NimPromoMRPDiscTypeDone = false;
                            }
                        }
                    }
                    #endregion
                }
                if (dDiscAmt > 0)
                {
                    saleLineItem.PartnerData.PROMOCODE = saleLineItem.PartnerData.NIMPROMOCODE;
                    if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.NIMPROMOCODE)))
                        saleLineItem.PartnerData.NIMPROMOAPPLID = true;
                    else
                        saleLineItem.PartnerData.NIMPROMOAPPLID = false;

                    saleLineItem.PartnerData.NimPromoMRPDiscType = true;
                    saleLineItem.PartnerData.NimPromoMRPDisc = dDiscAmt;
                }
                else
                {
                    saleLineItem.PartnerData.PROMOCODE = "";
                    saleLineItem.PartnerData.NimPromoMRPDiscType = false;
                    saleLineItem.PartnerData.NimPromoMRPDisc = 0;
                }
            }
        }

        private void CalPromoMakingDiscount(decimal dinitDiscValue, int isClubbed, SaleLineItem saleLineItem)
        {
            if (iCalcDiscType == (int)DiscType.Pct)
            {
                #region DiscType.Pct
                dDiscAmt = dinitDiscValue;

                if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                {
                    if (isClubbed == 0)
                    {
                        LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                        {
                            decimal dNewWouldBEDiscPct = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                            if (saleLineItem.LineDiscount > dDiscAmt)
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Existing discount is greater than current discount for the item '" + saleLineItem.ItemId + "',Do you want to override the discount ? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                                    {
                                        saleLineItem.DiscountLines.Clear();

                                        discountItem.Percentage = dNewWouldBEDiscPct;
                                        discountItem.LineDiscountType = DiscountTypes.Customer;
                                        saleLineItem.Add(discountItem);

                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                            saleLineItem.PartnerData.NimPromoMakingDiscTypeDone = true;
                                        else
                                            saleLineItem.PartnerData.NimPromoMakingDiscTypeDone = false;
                                    }
                                }
                            }
                            else
                            {
                                saleLineItem.DiscountLines.Clear();

                                discountItem.Percentage = dNewWouldBEDiscPct;
                                discountItem.LineDiscountType = DiscountTypes.Customer;
                                saleLineItem.Add(discountItem);

                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                    saleLineItem.PartnerData.NimPromoMakingDiscTypeDone = true;
                                else
                                    saleLineItem.PartnerData.NimPromoMakingDiscTypeDone = false;
                            }
                        }
                    }
                    else
                    {
                        LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                        {
                            discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                            discountItem.LineDiscountType = DiscountTypes.Customer;
                            saleLineItem.Add(discountItem);

                            if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                saleLineItem.PartnerData.NimPromoMakingDiscTypeDone = true;
                            else
                                saleLineItem.PartnerData.NimPromoMakingDiscTypeDone = false;
                        }
                    }
                }
                #endregion
            }
            else if (iCalcDiscType == (int)DiscType.Amt)
            {
                #region DiscType.Amt
                dDiscAmt = dinitDiscValue;

                if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) >= dDiscAmt)
                {
                    if (isClubbed == 0)
                    {
                        LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                        decimal dNewWouldBEDiscPct = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                        if (saleLineItem.LineDiscount > dDiscAmt)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Existing discount is greater than current discount,Are you want to override the discount ? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                                {
                                    saleLineItem.DiscountLines.Clear();

                                    if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                    {
                                        discountItem.Percentage = dNewWouldBEDiscPct;// (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                        discountItem.LineDiscountType = DiscountTypes.Customer;
                                        saleLineItem.Add(discountItem);

                                        if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                            saleLineItem.PartnerData.NimPromoMakingDiscTypeDone = true;
                                        else
                                            saleLineItem.PartnerData.NimPromoMakingDiscTypeDone = false;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                            {
                                saleLineItem.DiscountLines.Clear();
                                discountItem.Percentage = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                                discountItem.LineDiscountType = DiscountTypes.Customer;
                                saleLineItem.Add(discountItem);

                                if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                    saleLineItem.PartnerData.NimPromoMakingDiscTypeDone = true;
                                else
                                    saleLineItem.PartnerData.NimPromoMakingDiscTypeDone = false;
                            }
                        }
                    }
                    else
                    {
                        LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                            discountItem.Percentage = ((dDiscAmt) * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                        discountItem.LineDiscountType = DiscountTypes.Customer;
                        saleLineItem.Add(discountItem);

                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                        {
                            if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                saleLineItem.PartnerData.NimPromoMakingDiscTypeDone = true;
                            else
                                saleLineItem.PartnerData.NimPromoMakingDiscTypeDone = false;
                        }
                    }
                }
                #endregion
            }
            if (dDiscAmt > 0)
            {
                saleLineItem.PartnerData.PROMOCODE = saleLineItem.PartnerData.NIMPROMOCODE;
                saleLineItem.PartnerData.NimPromoMakingDiscType = true;
                saleLineItem.PartnerData.NimPromoMakingDisc = dinitDiscValue;
            }
            else
            {
                saleLineItem.PartnerData.PROMOCODE = "";
                saleLineItem.PartnerData.NimPromoMakingDiscType = false;
                saleLineItem.PartnerData.NimPromoMakingDisc = 0;
            }
        }


        private void NonClubbedTierDiscountApply(RetailTransaction transaction, SaleLineItem saleLineItem)
        {
            LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
            decimal dNewWouldBEDiscPct = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
            if (saleLineItem.LineDiscount > dDiscAmt)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Existing discount is greater than current discount for the item '" + saleLineItem.ItemId + "',Do you want to override the discount ? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                    {
                        saleLineItem.DiscountLines.Clear();

                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                        {
                            discountItem.Percentage = dNewWouldBEDiscPct;// (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                            discountItem.LineDiscountType = DiscountTypes.Customer;
                            saleLineItem.Add(discountItem);

                            if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                transaction.PartnerData.TierDiscDone = true;
                            else
                                transaction.PartnerData.TierDiscDone = false;
                        }
                    }
                }
            }
            else
            {
                saleLineItem.DiscountLines.Clear();

                if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                {
                    discountItem.Percentage = dNewWouldBEDiscPct;// (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                    discountItem.LineDiscountType = DiscountTypes.Customer;
                    saleLineItem.Add(discountItem);

                    if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                        transaction.PartnerData.TierDiscDone = true;
                    else
                        transaction.PartnerData.TierDiscDone = false;
                }
            }
        }

        private void NonClubbedSKUAgingDiscountApply(RetailTransaction transaction, SaleLineItem saleLineItem)
        {
            LineDiscountItem discountItem = (LineDiscountItem)this.Utility.CreateLineDiscountItem();
            decimal dNewWouldBEDiscPct = (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
            if (saleLineItem.LineDiscount > dDiscAmt)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Existing discount is greater than current discount for the item '" + saleLineItem.ItemId + "',Do you want to override the discount ? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                    {
                        saleLineItem.DiscountLines.Clear();

                        if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                        {
                            discountItem.Percentage = dNewWouldBEDiscPct;// (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                            discountItem.LineDiscountType = DiscountTypes.Customer;
                            saleLineItem.Add(discountItem);

                            if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                                transaction.PartnerData.SKUAgingDiscDone = true;
                            else
                                transaction.PartnerData.SKUAgingDiscDone = false;
                        }
                    }
                }
            }
            else
            {
                saleLineItem.DiscountLines.Clear();

                if (Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                {
                    discountItem.Percentage = dNewWouldBEDiscPct;// (dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount);
                    discountItem.LineDiscountType = DiscountTypes.Customer;
                    saleLineItem.Add(discountItem);

                    if ((dDiscAmt * 100) / Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount) > 0)
                        transaction.PartnerData.SKUAgingDiscDone = true;
                    else
                        transaction.PartnerData.SKUAgingDiscDone = false;
                }
            }
        }

        private decimal GetSKUAgingDiscount(string sItemId, string sStoreGrp, ref DateTime dtFROMDATE, ref int iCLUBBED,
            ref int iDISCTYPE, ref int iFROMDAYS, ref int iTODAYS, ref int iDsicOn)
        {
            decimal dResult = 0;

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " DECLARE @Temp TABLE (DISCOUNT numeric(20,2), FROMDATE DATE,CLUBBED int,DISCTYPE int,FROMDAYS int,TODAYS int,DISCOUNTON int);" +
                                 "  DECLARE @LOCATION VARCHAR(20) " +
                                 "  DECLARE @PRODUCT BIGINT " +
                                 "  DECLARE @PARENTITEM VARCHAR(20) " +
                                 "  DECLARE @CUSTCODE VARCHAR(20) " +

                                 "  DECLARE @PRODUCTCODE VARCHAR(20) " +
                                 "  DECLARE @COLLECTIONCODE VARCHAR(20) " +
                                 "  DECLARE @ARTICLE_CODE VARCHAR(20) " +

                                 "  IF EXISTS(SELECT TOP(1) ACTIVATE FROM CRWSKUAGEINGDISCOUNT SAD )" + // WHERE RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'
                                 "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "')" + // AND RETAIL=1 req by S.Sharma on 15/02/17
                                 "  BEGIN SELECT @PARENTITEM = ITEMIDPARENT,@PRODUCTCODE=PRODUCTTYPECODE" +
                                 "  ,@ARTICLE_CODE=ARTICLE_CODE , @COLLECTIONCODE =CollectionCode" +
                                 " FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                                 " END " +
                                 " ELSE" +
                                 " BEGIN" +
                                 " SET @PARENTITEM='" + sItemId + "' " +
                                 " END" +

                                 " IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "' AND RETAIL=0)" + // AND RETAIL=0 req by S.Sharma on 22/05/17
                                 " BEGIN " +
                                 " SET @PARENTITEM='" + sItemId + "'" +
                                 " END ";


            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //1
                " FROM   CRWSKUAGEINGDISCOUNT  SAD" +
                " WHERE (SAD.ITEMID=@PARENTITEM) " +
                 " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                     " BEGIN " +
                     " INSERT INTO @Temp (DISCOUNT, FROMDATE,CLUBBED,DISCTYPE,FROMDAYS,TODAYS,DISCOUNTON)  " +
                     "     SELECT  TOP (1) CAST(SAD.DISCAMT AS decimal(18, 2))" +
                     "     ,FROMDATE,ISNULL(CLUBBED,0) as CLUBBED,ISNULL(DISCTYPE,0) as DISCTYPE,FROMDAYS,TODAYS,ISNULL(DISCOUNTON,0) as DISCOUNTON " +
                     "       FROM   CRWSKUAGEINGDISCOUNT  SAD  WHERE     " +
                     "     (SAD.ITEMID=@PARENTITEM ) " +
                     "  AND   (SAD.SKU='" + sItemId + "'  or SAD.SKU='')" +
                     "  AND   (SAD.ProductTypeCode=@ProductCode  or SAD.ProductTypeCode='')" +
                     "  AND   (SAD.Collection=@CollectionCode  or SAD.Collection='')" +
                     "  AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +
                     " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)   " +
                     "       AND SAD.ACTIVATE = 1" +
                     " ORDER BY SAD.ITEMID DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //2
               " FROM    CRWSKUAGEINGDISCOUNT SAD  " +
               " WHERE (SAD.ITEMID=@PARENTITEM) " +

                 " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                    " BEGIN " +
                    " INSERT INTO @Temp (DISCOUNT, FROMDATE,CLUBBED,DISCTYPE,FROMDAYS,TODAYS,DISCOUNTON)  " +
                    "     SELECT  TOP (1) CAST(SAD.DISCAMT AS decimal(18, 2))" +
                    "     ,FROMDATE,ISNULL(CLUBBED,0) as CLUBBED,ISNULL(DISCTYPE,0) as DISCTYPE," +
                    " FROMDAYS,TODAYS ,ISNULL(DISCOUNTON,0) as DISCOUNTON FROM   CRWSKUAGEINGDISCOUNT  SAD  " +
                    "     WHERE     " +
                    "     (SAD.ITEMID=@PARENTITEM ) " +
                    "  AND   (SAD.SKU='" + sItemId + "'  or SAD.SKU='')" +
                     "  AND   (SAD.ProductTypeCode=@ProductCode  or SAD.ProductTypeCode='')" +
                     "  AND   (SAD.Collection=@CollectionCode  or SAD.Collection='')" +
                     "  AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +
                     " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)" +
                    "       AND SAD.ACTIVATE = 1" +
                    " ORDER BY SAD.ITEMID DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //3
                " FROM    CRWSKUAGEINGDISCOUNT SAD  " +
                " WHERE (SAD.ITEMID='') " +

                " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +

                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                     " BEGIN " +
                      " INSERT INTO @Temp (DISCOUNT, FROMDATE ,CLUBBED,DISCTYPE,FROMDAYS,TODAYS,DISCOUNTON)  " +
                     "     SELECT  TOP (1) CAST(SAD.DISCAMT AS decimal(18, 2))" +
                     "     ,FROMDATE,ISNULL(CLUBBED,0) as CLUBBED ,ISNULL(DISCTYPE,0) as DISCTYPE," +
                     " FROMDAYS,TODAYS,ISNULL(DISCOUNTON,0) as DISCOUNTON FROM  CRWSKUAGEINGDISCOUNT SAD " +
                     "     WHERE     " +
                     "     (SAD.ITEMID='' )" +
                     "  AND   (SAD.SKU='" + sItemId + "'  or SAD.SKU='')" +
                     "  AND   (SAD.ProductTypeCode=@ProductCode  or SAD.ProductTypeCode='')" +
                     "  AND   (SAD.Collection=@CollectionCode  or SAD.Collection='')" +
                     "  AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +

                      " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)   " +
                     "       AND SAD.ACTIVATE = 1" +
                     " ORDER BY SAD.ITEMID DESC END";
            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //4
               " FROM   CRWSKUAGEINGDISCOUNT SAD " +
               " WHERE (SAD.ITEMID='') " +
               " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE))" +
                    " BEGIN " +
                     " INSERT INTO @Temp (DISCOUNT, FROMDATE ,CLUBBED,DISCTYPE,FROMDAYS,TODAYS,DISCOUNTON)  " +
                    "     SELECT  TOP (1) CAST(SAD.DISCAMT AS decimal(18, 2))" +
                    "     ,FROMDATE,ISNULL(CLUBBED,0) as CLUBBED,ISNULL(DISCTYPE,0) as DISCTYPE," +
                    " FROMDAYS,TODAYS,ISNULL(DISCOUNTON,0) as DISCOUNTON FROM CRWSKUAGEINGDISCOUNT SAD  " +
                    "     WHERE     " +
                    "     (SAD.ITEMID='' )  " +
                    "  AND   (SAD.SKU='" + sItemId + "'  or SAD.SKU='')" +
                     "  AND   (SAD.ProductTypeCode=@ProductCode  or SAD.ProductTypeCode='')" +
                     "  AND   (SAD.Collection=@CollectionCode  or SAD.Collection='')" +
                     "  AND   (SAD.Article=@ARTICLE_CODE  or SAD.Article='')" +

                    " AND (SAD.STOREGROUP = CASE WHEN SAD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN SAD.TABLEGROUPALL=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN SAD.TABLEGROUPALL=2 THEN '' END)" +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN SAD.FROMDATE AND SAD.TODATE)   " +
                    "       AND SAD.ACTIVATE = 1" +
                    " ORDER BY SAD.ITEMID DESC END";
            commandText += " select TOP 1 * from @Temp";

            SqlCommand commandMk = new SqlCommand(commandText.ToString(), connection);
            commandMk.CommandTimeout = 0;

            using (SqlDataReader mkDiscRd = commandMk.ExecuteReader())
            {
                while (mkDiscRd.Read())
                {
                    dResult = Convert.ToDecimal(mkDiscRd.GetValue(0));
                    dtFROMDATE = Convert.ToDateTime(mkDiscRd.GetValue(1));
                    iCLUBBED = Convert.ToInt16(mkDiscRd.GetValue(2));
                    iDISCTYPE = Convert.ToInt16(mkDiscRd.GetValue(3));
                    iFROMDAYS = Convert.ToInt16(mkDiscRd.GetValue(4));
                    iTODAYS = Convert.ToInt16(mkDiscRd.GetValue(5));
                    iDsicOn = Convert.ToInt16(mkDiscRd.GetValue(6));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();


            return dResult;
        }

        private decimal GetTierDiscount(string sItemId, string sStoreGrp, decimal dQty, string sCustId,
            ref string sCustLScheme, ref int iCLUBBED, ref int iDISCTYPE, ref int iDsicOn)
        {
            decimal dResult = 0;

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " DECLARE @Temp TABLE (DISCOUNT numeric(20,2), LOYALTYSCHEMEID NVARCHAR(20),CLUBBED int,DISCTYPE int,DISCOUNTON int);" +
                                 "  DECLARE @LOCATION VARCHAR(20) " +
                                 "  DECLARE @PRODUCT BIGINT " +
                                 "  DECLARE @PARENTITEM VARCHAR(20) " +
                                 "  DECLARE @LOYALTYSCHEMEID VARCHAR(20) " + //CUSTCODE

                                 "  DECLARE @PRODUCTCODE VARCHAR(20) " +
                                 "  DECLARE @COLLECTIONCODE VARCHAR(20) " +
                                 "  DECLARE @ARTICLE_CODE VARCHAR(20) " +

                                 "  IF EXISTS(SELECT TOP(1) ACTIVATE FROM CRWLOYALTYTIERDISCOUNT LTD )" +
                                 "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "')" +
                                 "  BEGIN SELECT @PARENTITEM = ITEMIDPARENT,@PRODUCTCODE=PRODUCTTYPECODE" +
                                 "  ,@ARTICLE_CODE=ARTICLE_CODE , @COLLECTIONCODE =CollectionCode" +
                                 " FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                                 " END " +
                                 " ELSE" +
                                 " BEGIN" +
                                 " SET @PARENTITEM='" + sItemId + "' " +
                                 " END" +

                                 " SELECT @LOYALTYSCHEMEID = LOYALTYSCHEMEID" +//select LOYALTYSCHEMEID from RETAILLOYALTYMSRCARDTABLE where LOYALTYCUSTID=
                                 " FROM RETAILLOYALTYMSRCARDTABLE where LOYALTYCUSTID IN " +
                                 " (select LOYALTYCUSTID from RETAILLOYALTYCUSTTABLE where ACCOUNTNUM= '" + sCustId + "')" +

                                 " IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "' AND RETAIL=0)" +
                                 " BEGIN " +
                                 " SET @PARENTITEM='" + sItemId + "'" +
                                 " END ";


            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //1
                " FROM   CRWLOYALTYTIERDISCOUNT  LTD" +
                " WHERE (LTD.ITEMID=@PARENTITEM) " +
                //" AND (LTD.STOREGROUP = CASE WHEN LTD.TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN LTD.TABLEGROUPALL=0" +
                //           " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN LTD.TABLEGROUPALL=2 THEN '' END)" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN LTD.FROMDATE AND LTD.TODATE))" +
                     " BEGIN " +
                     " INSERT INTO @Temp (DISCOUNT, LOYALTYSCHEMEID,CLUBBED,DISCTYPE,DISCOUNTON)  " +
                     "     SELECT  TOP (1) CAST(LTD.DISCAMT AS decimal(18, 2))" +
                     "     ,LOYALTYSCHEMEID,ISNULL(CLUBBED,0) as CLUBBED,ISNULL(DISCTYPE,0) as DISCTYPE,ISNULL(DISCOUNTON,0) as DISCOUNTON " +
                     "       FROM   CRWLOYALTYTIERDISCOUNT  LTD  WHERE     " +
                     "     (LTD.ITEMID=@PARENTITEM ) " +
                     "  AND   (LTD.ProductTypeCode=@ProductCode  or LTD.ProductTypeCode='')" +
                     "  AND   (LTD.Collection=@CollectionCode  or LTD.Collection='')" +
                     "  AND   (LTD.Article=@ARTICLE_CODE  or LTD.Article='')" +
                     "  AND   (LTD.LOYALTYSCHEMEID=@LOYALTYSCHEMEID)" +

                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN LTD.FROMDATE AND LTD.TODATE)   " +
                     " AND " + dQty + "  BETWEEN LTD.FROMWEIGHT AND LTD.TOWEIGHT" +
                     "       AND LTD.ACTIVATE = 1" +
                     " ORDER BY LTD.ITEMID DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //2
               " FROM    CRWLOYALTYTIERDISCOUNT LTD  " +
               " WHERE (LTD.ITEMID=@PARENTITEM) " +

               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN LTD.FROMDATE AND LTD.TODATE))" +
                    " BEGIN " +
                    " INSERT INTO @Temp (DISCOUNT, LOYALTYSCHEMEID,CLUBBED,DISCTYPE,DISCOUNTON)  " +
                    "     SELECT  TOP (1) CAST(LTD.DISCAMT AS decimal(18, 2))" +
                    "     ,LOYALTYSCHEMEID,ISNULL(CLUBBED,0) as CLUBBED,ISNULL(DISCTYPE,0) as DISCTYPE," +
                    " ISNULL(DISCOUNTON,0) as DISCOUNTON FROM   CRWLOYALTYTIERDISCOUNT  LTD  " +
                    "     WHERE     " +
                    "     (LTD.ITEMID=@PARENTITEM ) " +
                     "  AND   (LTD.ProductTypeCode=@ProductCode  or LTD.ProductTypeCode='')" +
                     "  AND   (LTD.Collection=@CollectionCode  or LTD.Collection='')" +
                     "  AND   (LTD.Article=@ARTICLE_CODE  or LTD.Article='')" +
                    "  AND   (LTD.LOYALTYSCHEMEID=@LOYALTYSCHEMEID)" +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN LTD.FROMDATE AND LTD.TODATE)" +
                    " AND " + dQty + "  BETWEEN LTD.FROMWEIGHT AND LTD.TOWEIGHT" +
                    "  AND LTD.ACTIVATE = 1" +
                    " ORDER BY LTD.ITEMID DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //3
                " FROM    CRWLOYALTYTIERDISCOUNT LTD  " +
                " WHERE (LTD.ITEMID='') " +

                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN LTD.FROMDATE AND LTD.TODATE))" +
                     " BEGIN " +
                      " INSERT INTO @Temp (DISCOUNT, LOYALTYSCHEMEID ,CLUBBED,DISCTYPE,DISCOUNTON)  " +
                     "     SELECT  TOP (1) CAST(LTD.DISCAMT AS decimal(18, 2))" +
                     "     ,LOYALTYSCHEMEID,ISNULL(CLUBBED,0) as CLUBBED ,ISNULL(DISCTYPE,0) as DISCTYPE,ISNULL(DISCOUNTON,0) as DISCOUNTON FROM  CRWLOYALTYTIERDISCOUNT LTD " +
                     "     WHERE     " +
                     "     (LTD.ITEMID='' )" +
                     "  AND   (LTD.ProductTypeCode=@ProductCode  or LTD.ProductTypeCode='')" +
                     "  AND   (LTD.Collection=@CollectionCode  or LTD.Collection='')" +
                     "  AND   (LTD.Article=@ARTICLE_CODE  or LTD.Article='')" +
                     "  AND   (LTD.LOYALTYSCHEMEID=@LOYALTYSCHEMEID)" +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN LTD.FROMDATE AND LTD.TODATE)   " +
                     " AND " + dQty + "  BETWEEN LTD.FROMWEIGHT AND LTD.TOWEIGHT" +
                     "       AND LTD.ACTIVATE = 1" +
                     " ORDER BY LTD.ITEMID DESC END";
            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //4
               " FROM   CRWLOYALTYTIERDISCOUNT LTD " +
               " WHERE (LTD.ITEMID='') " +

               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN LTD.FROMDATE AND LTD.TODATE))" +
                    " BEGIN " +
                     " INSERT INTO @Temp (DISCOUNT, LOYALTYSCHEMEID ,CLUBBED,DISCTYPE,DISCOUNTON)  " +
                    "     SELECT  TOP (1) CAST(LTD.DISCAMT AS decimal(18, 2))" +
                    "     ,LOYALTYSCHEMEID,ISNULL(CLUBBED,0) as CLUBBED,ISNULL(DISCTYPE,0) as DISCTYPE,ISNULL(DISCOUNTON,0) as DISCOUNTON FROM CRWLOYALTYTIERDISCOUNT LTD  " +
                    "     WHERE     " +
                    "     (LTD.ITEMID='' )  " +
                     "  AND   (LTD.ProductTypeCode=@ProductCode  or LTD.ProductTypeCode='')" +
                     "  AND   (LTD.Collection=@CollectionCode  or LTD.Collection='')" +
                     "  AND   (LTD.Article=@ARTICLE_CODE  or LTD.Article='')" +
                     "  AND   (LTD.LOYALTYSCHEMEID=@LOYALTYSCHEMEID)" +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN LTD.FROMDATE AND LTD.TODATE)   " +
                    " AND " + dQty + "  BETWEEN LTD.FROMWEIGHT AND LTD.TOWEIGHT" +
                    "       AND LTD.ACTIVATE = 1" +
                    " ORDER BY LTD.ITEMID DESC END";
            commandText += " select TOP 1 * from @Temp";

            SqlCommand commandMk = new SqlCommand(commandText.ToString(), connection);
            commandMk.CommandTimeout = 0;

            using (SqlDataReader mkDiscRd = commandMk.ExecuteReader())
            {
                while (mkDiscRd.Read())
                {
                    dResult = Convert.ToDecimal(mkDiscRd.GetValue(0));
                    sCustLScheme = Convert.ToString(mkDiscRd.GetValue(1));
                    iCLUBBED = Convert.ToInt16(mkDiscRd.GetValue(2));
                    iDISCTYPE = Convert.ToInt16(mkDiscRd.GetValue(3));
                    iDsicOn = Convert.ToInt16(mkDiscRd.GetValue(4));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();


            return dResult;
        }

        //START:Nim
        protected DateTime GetItemAge(string sItemId)
        {
            DateTime CeationDateOfItem = new DateTime();

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select SkuDate from SKUTable_Posted where skunumber='" + sItemId + "'");

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            CeationDateOfItem = Convert.ToDateTime(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            return CeationDateOfItem;

        }

        private int getUserDiscountPermissionId()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select RETAILDISCPERMISSION from RETAILSTAFFTABLE where STAFFID='" + ApplicationSettings.Terminal.TerminalOperator.OperatorId + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToInt16(cmd.ExecuteScalar());
        }

        private bool IsRetailItem(string sItemId)
        {
            bool bRetailItem = false;

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = "select RETAIL from inventtable WHERE ITEMID = '" + sItemId + "'";


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bRetailItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bRetailItem;
        }

        private decimal GetMkDiscountFromDiscPolicy(string sItemId, decimal dItemMainValue, string sWhichFieldValueWillGet)
        {
            decimal dResult = 0;


            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();


            string sFlatQry = "";
            if (sWhichFieldValueWillGet == "OPENINGDISCPCT")
                sFlatQry = " AND CRWMAKINGDISCOUNT.FLAT =1";
            else
                sFlatQry = "";

            string commandText = " DECLARE @Temp TABLE (MAXSALESPERSONSDISCPCT numeric(20,2), PROMOTIONCODE nvarchar(20),FLAT int,DiscountType int);" +
                                "  DECLARE @LOCATION VARCHAR(20) " +
                                 "  DECLARE @PRODUCT BIGINT " +
                                 "  DECLARE @PARENTITEM VARCHAR(20) " +
                                 "  DECLARE @CUSTCODE VARCHAR(20) " +

                                 "  DECLARE @PRODUCTCODE VARCHAR(20) " +
                                 "  DECLARE @COLLECTIONCODE VARCHAR(20) " +
                                 "  DECLARE @ARTICLE_CODE VARCHAR(20) " +

                                 "  IF EXISTS(SELECT TOP(1) ACTIVATE FROM CRWMAKINGDISCOUNT )" + // WHERE RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'
                                 "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "')" + // AND RETAIL=1 req by S.Sharma on 15/02/17
                                 "  BEGIN SELECT @PARENTITEM = ITEMIDPARENT,@PRODUCTCODE=PRODUCTTYPECODE" +
                                 "  ,@ARTICLE_CODE=ARTICLE_CODE , @COLLECTIONCODE =CollectionCode" +
                                 " FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                                 " END " +
                                 " ELSE" +
                                 " BEGIN" +
                                 " SET @PARENTITEM='" + sItemId + "' " +
                                 " END" +

                                 " IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "' AND RETAIL=0)" + // AND RETAIL=0 req by S.Sharma on 22/05/17
                                 " BEGIN " +
                                 " SET @PARENTITEM='" + sItemId + "'" +
                                 " END " +

                                 "  SELECT @CUSTCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "'";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //1
                " FROM   CRWMAKINGDISCOUNT  " +
                " WHERE (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM) " +
                " AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE) " +
                " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE))" +
                     " BEGIN " +
                     " INSERT INTO @Temp (MAXSALESPERSONSDISCPCT, PROMOTIONCODE,FLAT,DiscountType)  " +
                     "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT,ISNULL(DiscountType,0) as DiscountType " +
                     "       FROM   CRWMAKINGDISCOUNT  WHERE     " +
                     "     (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM ) AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE ) " +

                     "  AND   (CRWMAKINGDISCOUNT.ProductCode=@ProductCode  or CRWMAKINGDISCOUNT.ProductCode='')" +
                     "  AND   (CRWMAKINGDISCOUNT.CollectionCode=@CollectionCode  or CRWMAKINGDISCOUNT.CollectionCode='')" +
                     "  AND   (CRWMAKINGDISCOUNT.ArticleCode=@ARTICLE_CODE  or CRWMAKINGDISCOUNT.ArticleCode='')" +

                     "     AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
                     "      and ('" + dItemMainValue + "'  BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)   " +
                     "       AND CRWMAKINGDISCOUNT.ACTIVATE = 1" +
                     " " + sFlatQry + "" +
                     " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC ";
            if (string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWMAKINGDISCOUNT.FLAT ASC END";
            else
                commandText += ", CRWMAKINGDISCOUNT.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //2
               " FROM   CRWMAKINGDISCOUNT  " +
               " WHERE (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM) " +
               " AND (CRWMAKINGDISCOUNT.CODE='') " +
               " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE))" +
                    " BEGIN " +
                    " INSERT INTO @Temp (MAXSALESPERSONSDISCPCT, PROMOTIONCODE,FLAT,DiscountType)  " +
                    "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT,ISNULL(DiscountType,0) as DiscountType  FROM   CRWMAKINGDISCOUNT   " +
                    "     WHERE     " +
                    "     (CRWMAKINGDISCOUNT.ITEMID=@PARENTITEM ) AND (CRWMAKINGDISCOUNT.CODE='' ) " +

                     "  AND   (CRWMAKINGDISCOUNT.ProductCode=@ProductCode  or CRWMAKINGDISCOUNT.ProductCode='')" +
                     "  AND   (CRWMAKINGDISCOUNT.CollectionCode=@CollectionCode  or CRWMAKINGDISCOUNT.CollectionCode='')" +
                     "  AND   (CRWMAKINGDISCOUNT.ArticleCode=@ARTICLE_CODE  or CRWMAKINGDISCOUNT.ArticleCode='')" +

                    "     AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
                    "      and ('" + dItemMainValue + "'  BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)" +
                    "       AND CRWMAKINGDISCOUNT.ACTIVATE = 1" +
                    " " + sFlatQry + "" +
                    " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC ";
            if (string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWMAKINGDISCOUNT.FLAT ASC END";
            else
                commandText += ", CRWMAKINGDISCOUNT.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //3
                " FROM   CRWMAKINGDISCOUNT  " +
                " WHERE (CRWMAKINGDISCOUNT.ITEMID='') " +
                " AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE) " +
                " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE))" +
                     " BEGIN " +
                      " INSERT INTO @Temp (MAXSALESPERSONSDISCPCT, PROMOTIONCODE,FLAT,DiscountType)  " +
                     "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT ,ISNULL(DiscountType,0) as DiscountType FROM   CRWMAKINGDISCOUNT   " +
                     "     WHERE     " +
                     "     (CRWMAKINGDISCOUNT.ITEMID='' ) AND (CRWMAKINGDISCOUNT.CODE=@CUSTCODE ) " +

                     "   AND  (CRWMAKINGDISCOUNT.ProductCode=@ProductCode  or CRWMAKINGDISCOUNT.ProductCode='')" +
                     "   AND  (CRWMAKINGDISCOUNT.CollectionCode=@CollectionCode  or CRWMAKINGDISCOUNT.CollectionCode='')" +
                     "   AND  (CRWMAKINGDISCOUNT.ArticleCode=@ARTICLE_CODE  or CRWMAKINGDISCOUNT.ArticleCode='')" +

                     "     AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
                     "      and ('" + dItemMainValue + "'  BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)   " +
                     "       AND CRWMAKINGDISCOUNT.ACTIVATE = 1" +
                     " " + sFlatQry + "" +
                     " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC ";
            if (string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWMAKINGDISCOUNT.FLAT ASC END";
            else
                commandText += ", CRWMAKINGDISCOUNT.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //4
               " FROM   CRWMAKINGDISCOUNT  " +
               " WHERE (CRWMAKINGDISCOUNT.ITEMID='') " +
               " AND (CRWMAKINGDISCOUNT.CODE='') " +
               " AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE))" +
                    " BEGIN " +
                     " INSERT INTO @Temp (MAXSALESPERSONSDISCPCT, PROMOTIONCODE,FLAT,DiscountType)  " +
                    "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT,ISNULL(DiscountType,0) as DiscountType   FROM   CRWMAKINGDISCOUNT   " +
                    "     WHERE     " +
                    "     (CRWMAKINGDISCOUNT.ITEMID='' ) AND (CRWMAKINGDISCOUNT.CODE='' ) " +

                     "   AND (CRWMAKINGDISCOUNT.ProductCode=@ProductCode  or CRWMAKINGDISCOUNT.ProductCode='')" +
                     "   AND  (CRWMAKINGDISCOUNT.CollectionCode=@CollectionCode  or CRWMAKINGDISCOUNT.CollectionCode='')" +
                     "   AND  (CRWMAKINGDISCOUNT.ArticleCode=@ARTICLE_CODE  or CRWMAKINGDISCOUNT.ArticleCode='')" +

                    "     AND (CRWMAKINGDISCOUNT.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWMAKINGDISCOUNT.RETAILSTOREID='')" +
                    "      and ('" + dItemMainValue + "'  BETWEEN CRWMAKINGDISCOUNT.FROMWT AND CRWMAKINGDISCOUNT.TOWT)  " +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWMAKINGDISCOUNT.FROMDATE AND CRWMAKINGDISCOUNT.TODATE)   " +
                    "       AND CRWMAKINGDISCOUNT.ACTIVATE = 1" +
                    " " + sFlatQry + "" +
                    " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC ";
            if (string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWMAKINGDISCOUNT.FLAT ASC END";
            else
                commandText += ", CRWMAKINGDISCOUNT.FLAT DESC END";

            commandText += " select TOP 1 * from @Temp";

            SqlCommand commandMk = new SqlCommand(commandText.ToString(), connection);
            commandMk.CommandTimeout = 0;
            //SqlDataReader mkDiscRd = commandMk.ExecuteReader();

            using (SqlDataReader mkDiscRd = commandMk.ExecuteReader())
            {
                while (mkDiscRd.Read())
                {
                    dResult = Convert.ToDecimal(mkDiscRd.GetValue(0));
                    sPromoCode = Convert.ToString(mkDiscRd.GetValue(1));
                    iIsDiscFlat = Convert.ToInt16(mkDiscRd.GetValue(2));

                    //sMkPromoCode = Convert.ToString(mkDiscRd.GetValue(1));
                    //iIsMkDiscFlat = Convert.ToInt16(mkDiscRd.GetValue(2));
                    //iMkDisType = Convert.ToInt16(mkDiscRd.GetValue(3));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();


            return dResult;
        }

        private decimal GetDiscountFromDiscPolicy(string sItemId, decimal dItemMainValue, string sWhichFieldValueWillGet, int iFlat)
        {
            decimal dResult = 0;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            #region old logic
            ////string commandText = "  DECLARE @INVENTLOCATION VARCHAR(20)    DECLARE @LOCATION VARCHAR(20)   DECLARE @ITEM VARCHAR(20)" +
            ////                     "  DECLARE @PARENTITEM VARCHAR(20) " +
            ////                     "  SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  " +
            ////                     "  RETAILCHANNELTABLE INNER JOIN  RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID  " +
            ////                     "  WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "' " +
            ////    // "  SELECT @MFGCODE = MFG_CODE FROM INVENTTABLE WHERE ITEMID='" + sItemId + "'" +
            ////                     "  IF EXISTS(SELECT TOP(1) ACTIVATE FROM CRWDISCOUNTPOLICY WHERE WAREHOUSE=@INVENTLOCATION) " +
            ////                     "  BEGIN SET @LOCATION=@INVENTLOCATION  END ELSE BEGIN  SET @LOCATION='' END  " +
            ////                     "  SET @PARENTITEM = ''" +
            ////                     "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "' AND RETAIL=1)" +
            ////                     "  BEGIN SELECT @PARENTITEM = ITEMIDPARENT FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
            ////                     "  END  SET @ITEM='" + sItemId + "'";

            ////commandText += " SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2)),ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE " +
            ////                " FROM   CRWDISCOUNTPOLICY  " +//INNER JOIN
            ////    //" (SELECT MFG_CODE FROM INVENTTABLE WHERE  ITEMID=  '" + ItemID.Trim() + "') T ON CRWDISCOUNTPOLICY.MFG_CODE = T.MFG_CODE " +
            ////                " WHERE    " +
            ////                " (CRWDISCOUNTPOLICY.ITEMID=@ITEM OR CRWDISCOUNTPOLICY.ITEMID = @PARENTITEM OR CRWDISCOUNTPOLICY.ITEMID='')" +
            ////                " AND CRWDISCOUNTPOLICY.WAREHOUSE=@LOCATION  " +
            ////                " AND  (CRWDISCOUNTPOLICY.ARTICLECODE=(SELECT ARTICLE_CODE FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId.Trim() + "') " +
            ////                " OR CRWDISCOUNTPOLICY.ARTICLECODE='') AND " +
            ////                "  ('" + dItemMainValue + "' BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
            ////                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)  " +
            ////                " AND CRWDISCOUNTPOLICY.ACTIVATE = 1 " +
            ////                "  ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.ARTICLECODE DESC" +
            ////                " ,CRWDISCOUNTPOLICY.FROMDATE DESC";
            #endregion
            string sFlatQry = "";
            if (sWhichFieldValueWillGet == "OPENINGDISCPCT")
                sFlatQry = " AND CRWDISCOUNTPOLICY.FLAT =" + iFlat + "";
            else
                sFlatQry = "";

            string commandText = "  DECLARE @LOCATION VARCHAR(20) " +
                                 "  DECLARE @PRODUCT BIGINT " +
                                 "  DECLARE @PARENTITEM VARCHAR(20) " +
                                 "  DECLARE @CUSTCODE VARCHAR(20) " +
                                 "  IF EXISTS(SELECT TOP(1) ACTIVATE FROM CRWDISCOUNTPOLICY )" + // WHERE RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'
                                 "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "' AND RETAIL=1)" +
                                 "  BEGIN SELECT @PARENTITEM = ITEMIDPARENT FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                //"  SELECT @PRODUCT = ITEMID FROM [INVENTTABLE] WHERE ITEMID = @PARENTITEM " +
                                 "  SELECT @CUSTCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "' " +
                                 "  END ";
            #region
            //commandText += " SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2)),ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT " +
            //                " FROM   CRWDISCOUNTPOLICY  " +
            //                " WHERE    " +
            //                " (CRWDISCOUNTPOLICY.ITEMID=@PARENTITEM  or CRWDISCOUNTPOLICY.ITEMID='')" +
            //                " AND (CRWDISCOUNTPOLICY.CODE=@CUSTCODE  or CRWDISCOUNTPOLICY.CODE='')" +
            //                " AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' or CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
            //                " and ('" + dItemMainValue + "' BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
            //                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)  " +
            //                " AND CRWDISCOUNTPOLICY.ACTIVATE = 1 " +
            //                " " + sFlatQry + "" +
            //                " ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.CODE DESC , CRWDISCOUNTPOLICY.FLAT DESC";
            #endregion

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //1
                " FROM   CRWDISCOUNTPOLICY  " +
                " WHERE (CRWDISCOUNTPOLICY.ITEMID=@PARENTITEM) " +
                " AND (CRWDISCOUNTPOLICY.CODE=@CUSTCODE) " +
                " AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE))" +
                     " BEGIN " +
                     "     SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT  FROM   CRWDISCOUNTPOLICY   " +
                     "     WHERE     " +
                     "     (CRWDISCOUNTPOLICY.ITEMID=@PARENTITEM ) AND (CRWDISCOUNTPOLICY.CODE=@CUSTCODE ) " +
                     "     AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                     "      and ('" + dItemMainValue + "'  BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)   " +
                     "       AND CRWDISCOUNTPOLICY.ACTIVATE = 1" +
                     " " + sFlatQry + "" +
                     " ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.CODE DESC ";
            //if (string.IsNullOrEmpty(sFlatQry))
            //    commandText += ", CRWDISCOUNTPOLICY.FLAT ASC END";
            //else
            commandText += ", CRWDISCOUNTPOLICY.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //2
               " FROM   CRWDISCOUNTPOLICY  " +
               " WHERE (CRWDISCOUNTPOLICY.ITEMID=@PARENTITEM) " +
               " AND (CRWDISCOUNTPOLICY.CODE='') " +
               " AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE))" +
                    " BEGIN " +
                    "     SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT  FROM   CRWDISCOUNTPOLICY   " +
                    "     WHERE     " +
                    "     (CRWDISCOUNTPOLICY.ITEMID=@PARENTITEM ) AND (CRWDISCOUNTPOLICY.CODE='' ) " +
                    "     AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                    "      and ('" + dItemMainValue + "'  BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)   " +
                    "       AND CRWDISCOUNTPOLICY.ACTIVATE = 1" +
                    " " + sFlatQry + "" +
                    " ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.CODE DESC ";
            //if (string.IsNullOrEmpty(sFlatQry))
            //    commandText += ", CRWDISCOUNTPOLICY.FLAT ASC END";
            //else
            commandText += ", CRWDISCOUNTPOLICY.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //3
                " FROM   CRWDISCOUNTPOLICY  " +
                " WHERE (CRWDISCOUNTPOLICY.ITEMID='') " +
                " AND (CRWDISCOUNTPOLICY.CODE=@CUSTCODE) " +
                " AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'   OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE))" +
                     " BEGIN " +
                     "     SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT  FROM   CRWDISCOUNTPOLICY   " +
                     "     WHERE     " +
                     "     (CRWDISCOUNTPOLICY.ITEMID='' ) AND (CRWDISCOUNTPOLICY.CODE=@CUSTCODE ) " +
                     "     AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                     "      and ('" + dItemMainValue + "'  BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)   " +
                     "       AND CRWDISCOUNTPOLICY.ACTIVATE = 1" +
                     " " + sFlatQry + "" +
                     " ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.CODE DESC ";
            //if (string.IsNullOrEmpty(sFlatQry))
            //    commandText += ", CRWDISCOUNTPOLICY.FLAT ASC END";
            //else
            commandText += ", CRWDISCOUNTPOLICY.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //4
               " FROM   CRWDISCOUNTPOLICY  " +
               " WHERE (CRWDISCOUNTPOLICY.ITEMID='') " +
               " AND (CRWDISCOUNTPOLICY.CODE='') " +
               " AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE))" +
                    " BEGIN " +
                    "     SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT  FROM   CRWDISCOUNTPOLICY   " +
                    "     WHERE     " +
                    "     (CRWDISCOUNTPOLICY.ITEMID='' ) AND (CRWDISCOUNTPOLICY.CODE='' ) " +
                    "     AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                    "      and ('" + dItemMainValue + "'  BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)   " +
                    "       AND CRWDISCOUNTPOLICY.ACTIVATE = 1" +
                    " " + sFlatQry + "" +
                    " ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.CODE DESC ";
            //if (string.IsNullOrEmpty(sFlatQry))
            //    commandText += ", CRWDISCOUNTPOLICY.FLAT ASC END";
            //else
            commandText += ", CRWDISCOUNTPOLICY.FLAT DESC END";

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dResult = Convert.ToDecimal(reader.GetValue(0));
                    sPromoCode = Convert.ToString(reader.GetValue(1));
                    iIsDiscFlat = Convert.ToInt16(reader.GetValue(2));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dResult;
        }

        private string getStoreFormatCode()
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT ISNULL(STOREFORMAT,'') STOREFORMAT FROM RETAILSTORETABLE WHERE STORENUMBER = '" + ApplicationSettings.Database.StoreID + "'");

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            string sStoreFormat = Convert.ToString(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return sStoreFormat;

        }

        // END NIMBUS


        #region CustomerDiscount
        /// <summary>
        /// Add total discount amount to the value amount.
        /// </summary>
        /// <param name="retailTransaction"></param>
        /// <param name="amountValue"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void AddTotalDiscountAmount(IRetailTransaction retailTransaction, decimal amountValue)
        {
            retailTransaction.SetTotalDiscAmount(amountValue);
        }

        /// <summary>
        /// Add loyalty points discount amount to the value amount
        /// </summary>
        /// <param name="retailTransaction">The retail transaction</param>
        /// <param name="amountValue">The amount to be set</param>
        public void AddLoyaltyDiscountAmount(IRetailTransaction retailTransaction, decimal amountValue)
        {
            if (retailTransaction == null)
            {
                throw new ArgumentNullException("retailTransaction");
            }

            retailTransaction.SetLoyaltyDiscAmount(amountValue);
        }

        /// <summary>
        /// Returns true if total discount amount is authorized.
        /// </summary>
        /// <param name="retailTransaction"></param>
        /// <param name="amountValue"></param>
        /// <param name="maxAmountValue"></param>
        /// <returns></returns>
        public bool AuthorizeTotalDiscountAmount(IRetailTransaction retailTransaction, decimal amountValue, decimal maxAmountValue)
        {
            if (retailTransaction == null)
            {
                NetTracer.Warning("retailTransaction parameter is null");
                throw new ArgumentNullException("retailTransaction");
            }

            bool returnValue = true;
            decimal tempGrossAmount = retailTransaction.GrossAmount - retailTransaction.LineDiscount - retailTransaction.PeriodicDiscountAmount;

            if (Discount.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled()
                && !Discount.InternalApplication.Services.Peripherals.FiscalPrinter.AuthorizeTotalDiscountAmount(retailTransaction, amountValue, maxAmountValue))
            {
                returnValue = false;
            }
            else if (amountValue > Math.Abs(tempGrossAmount))
            {
                returnValue = false;
                string message = ApplicationLocalizer.Language.Translate(3178); //The discount amount is to high. The discount amount cannot exceed the balance due.
                using (frmMessage dialog = new frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    POSFormsManager.ShowPOSForm(dialog);
                }
            }
            else if (amountValue > Math.Abs(maxAmountValue))
            {
                returnValue = false;
                string maximumAmountRounded = this.Application.Services.Rounding.Round(maxAmountValue, true);
                decimal maximumDiscountPct = (tempGrossAmount == decimal.Zero) ? decimal.Zero : (100m * maxAmountValue / tempGrossAmount);

                string message = ApplicationLocalizer.Language.Translate(3173, maximumAmountRounded, maximumDiscountPct.ToString("n2")); //The discount amount is to high. The discount percentage limit is set to xxxx %.
                using (frmMessage dialog = new frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    POSFormsManager.ShowPOSForm(dialog);
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Sets total discount percentage as per the given discount percentage.
        /// </summary>
        /// <param name="retailTransaction"></param>
        /// <param name="percentValue"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void AddTotalDiscountPercent(IRetailTransaction retailTransaction, decimal percentValue)
        {
            retailTransaction.SetTotalDiscPercent(percentValue);
        }

        /// <summary>
        /// Returns true if total discount percent is authorized.
        /// </summary>
        /// <param name="retailTransaction"></param>
        /// <param name="percentValue"></param>
        /// <param name="maxPercentValue"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public bool AuthorizeTotalDiscountPercent(IRetailTransaction retailTransaction, decimal percentValue, decimal maxPercentValue)
        {
            bool returnValue = true;
            decimal tempGrossAmount = retailTransaction.GrossAmount - retailTransaction.LineDiscount - retailTransaction.PeriodicDiscountAmount;

            if (Discount.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled()
                && !Discount.InternalApplication.Services.Peripherals.FiscalPrinter.AuthorizeTotalDiscountPercent(retailTransaction, percentValue, maxPercentValue))
            {
                returnValue = false;
            }
            else if (percentValue > 100m || percentValue > maxPercentValue)
            {
                string message = string.Format(LSRetailPosis.ApplicationLocalizer.Language.Translate(3511), maxPercentValue.ToString("n2")); //The discount percentage is to high. The maximum limit is set to xxx %.
                using (frmMessage dialog = new frmMessage(message, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information))
                {
                    this.Application.ApplicationFramework.POSShowForm(dialog);
                }

                returnValue = false;
            }

            return returnValue;
        }

        /// <summary>
        /// Adds disount item to the given discount item.
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="discountItem"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void AddLineDiscountAmount(ISaleLineItem lineItem, ILineDiscountItem discountItem)
        {
            ((SaleLineItem)lineItem).Add((LineDiscountItem)discountItem);
        }

        /// <summary>
        /// Add discount line
        /// </summary>
        /// <param name="lineItem">Line to which discount would be added</param>
        /// <param name="discountItem">Discount item to add</param>
        public void AddDiscountLine(ISaleLineItem lineItem, IDiscountItem discountItem)
        {
            ((SaleLineItem)lineItem).Add((DiscountItem)discountItem);
        }

        /// <summary>
        /// Returns true if discount amount is authorized.
        /// </summary>
        /// <param name="saleLineItem"></param>
        /// <param name="lineDiscountItem"></param>
        /// <param name="maximumDiscountAmt"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1"), SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public bool AuthorizeLineDiscountAmount(ISaleLineItem saleLineItem, ILineDiscountItem lineDiscountItem, decimal maximumDiscountAmt)
        {
            bool returnValue = true;
            decimal itemPriceWithoutDiscount = saleLineItem.Price * saleLineItem.Quantity;
            decimal tempItemAmount = saleLineItem.NetAmount - lineDiscountItem.Amount;
            maximumDiscountAmt *= saleLineItem.Quantity;

            if (Discount.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled()
                && !Discount.InternalApplication.Services.Peripherals.FiscalPrinter.AuthorizeLineDiscountAmount(saleLineItem, lineDiscountItem, maximumDiscountAmt))
            {
                returnValue = false;
            }
            else if (lineDiscountItem.Amount > Math.Abs(itemPriceWithoutDiscount) || tempItemAmount < decimal.Zero)
            {
                returnValue = false;
                string message = ApplicationLocalizer.Language.Translate(3177); //The discount amount is to high. The discount amount cannot exceed the item price.
                using (frmMessage dialog = new frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    POSFormsManager.ShowPOSForm(dialog);
                }
            }
            else if (lineDiscountItem.Amount > Math.Abs(maximumDiscountAmt))
            {
                returnValue = false;
                string maximumAmountRounded = this.Application.Services.Rounding.Round(maximumDiscountAmt, true);
                decimal maximumDiscountPct = (itemPriceWithoutDiscount == decimal.Zero) ? decimal.Zero : (100m * maximumDiscountAmt / itemPriceWithoutDiscount);
                string message = ApplicationLocalizer.Language.Translate(3173, maximumAmountRounded, maximumDiscountPct.ToString("n2")); //The discount amount is to high. The discount percentage limit is set to xxxx %.

                using (frmMessage dialog = new frmMessage(message, MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    POSFormsManager.ShowPOSForm(dialog);
                }
            }
            return returnValue;
        }

        /// <summary>
        /// Add disount percent to the given discount item.
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="discountItem"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void AddLineDiscountPercent(ISaleLineItem lineItem, ILineDiscountItem discountItem)
        {
            ((SaleLineItem)lineItem).Add((LineDiscountItem)discountItem);
        }

        /// <summary>
        /// Returns true if discount percent is correct.
        /// </summary>
        /// <param name="saleLineItem"></param>
        /// <param name="lineDiscountItem"></param>
        /// <param name="maximumDiscountPct"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1"), SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public bool AuthorizeLineDiscountPercent(ISaleLineItem saleLineItem, ILineDiscountItem lineDiscountItem, decimal maximumDiscountPct)
        {
            bool returnValue = true;
            decimal itemPriceWithoutDiscount = saleLineItem.Price * saleLineItem.Quantity;

            if (Discount.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled()
                && !Discount.InternalApplication.Services.Peripherals.FiscalPrinter.AuthorizeLineDiscountPercent(saleLineItem, lineDiscountItem, maximumDiscountPct))
            {
                returnValue = false;
            }
            else if (lineDiscountItem.Percentage > 100m || lineDiscountItem.Percentage > maximumDiscountPct)
            {
                string message = string.Format(LSRetailPosis.ApplicationLocalizer.Language.Translate(3183), maximumDiscountPct.ToString("n2")); //The discount percentage is to high. The limit is xxx %.
                using (frmMessage dialog = new frmMessage(message, System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Information))
                {
                    this.Application.ApplicationFramework.POSShowForm(dialog);
                }

                returnValue = false;
            }

            return returnValue;
        }

        /// <summary>
        /// Calculate the customer discount.
        /// </summary>
        /// <param name="retailTransaction">The retail transaction.</param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Calc", Justification = "Cannot change public API.")]
        public void CalcCustomerDiscount(RetailTransaction retailTransaction)
        {
            //Calc line discount
            retailTransaction = (RetailTransaction)CalcLineDiscount(retailTransaction);

            //Calc multiline discount
            retailTransaction = (RetailTransaction)CalcMultiLineDiscount(retailTransaction);

            //Calc total discount
            retailTransaction = (RetailTransaction)CalcTotalCustomerDiscount(retailTransaction);
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Revisit. Don't want to risk refactoring right now.")]
        private void GetLineDiscountLines(RetailTransaction retailTransaction, SaleLineItem saleItem, string currencyCode, ref decimal absQty, ref decimal discountAmount, ref decimal percent1, ref decimal percent2, ref decimal minQty)
        {
            int idx = 0;
            while (idx < 9)
            {
                int itemCode = idx % 3;    //Mod divsion
                int accountCode = idx / 3;

                string accountRelation = (accountCode == (int)PriceDiscAccountCode.Table) ? retailTransaction.Customer.CustomerId : (accountCode == (int)PriceDiscAccountCode.GroupId) ? retailTransaction.Customer.LineDiscountGroup : string.Empty;
                string itemRelation = (itemCode == (int)PriceDiscItemCode.Table) ? saleItem.ItemId : (itemCode == (int)PriceDiscItemCode.GroupId) ? saleItem.LineDiscountGroup : string.Empty;

                if (accountRelation == null)
                {
                    accountRelation = string.Empty;
                }

                if (itemRelation == null)
                {
                    itemRelation = string.Empty;
                }

                PriceDiscType relation = PriceDiscType.LineDiscSales; //Sales line discount - 5

                if (DiscountParameters.Activation(relation, (PriceDiscAccountCode)accountCode, (PriceDiscItemCode)itemCode))
                {
                    if ((ValidRelation((PriceDiscAccountCode)accountCode, accountRelation)) &&
                        (ValidRelation((PriceDiscItemCode)itemCode, itemRelation)))
                    {
                        try
                        {
                            bool dimensionDiscountFound = false;

                            if (!string.IsNullOrEmpty(saleItem.Dimension.VariantId))
                            {
                                var dimensionPriceDiscTable = GetPriceDiscDataCached(retailTransaction.TransactionId, relation, itemRelation, accountRelation, itemCode, accountCode, absQty, currencyCode, saleItem.Dimension, true);

                                foreach (DiscountAgreementArgs row in dimensionPriceDiscTable)
                                {
                                    bool unitsAreUndefinedOrEqual =
                                        (String.IsNullOrEmpty(row.UnitId) ||
                                         String.Equals(row.UnitId, saleItem.SalesOrderUnitOfMeasure, StringComparison.OrdinalIgnoreCase));

                                    if (unitsAreUndefinedOrEqual)
                                    {
                                        percent1 += row.Percent1;
                                        percent2 += row.Percent2;
                                        discountAmount += row.Amount;
                                        minQty += row.QuantityAmountFrom;
                                    }

                                    if (percent1 > 0M || percent2 > 0M || discountAmount > 0M)
                                    {
                                        dimensionDiscountFound = true;
                                    }

                                    if (!row.SearchAgain)
                                    {
                                        idx = 9;
                                    }
                                }
                            }

                            if (!dimensionDiscountFound)
                            {
                                var priceDiscTable = GetPriceDiscDataCached(retailTransaction.TransactionId, relation, itemRelation, accountRelation, itemCode, accountCode, absQty, currencyCode, saleItem.Dimension, false);

                                foreach (DiscountAgreementArgs row in priceDiscTable)
                                {
                                    bool unitsAreUndefinedOrEqual =
                                        (String.IsNullOrEmpty(row.UnitId) ||
                                         String.Equals(row.UnitId, saleItem.SalesOrderUnitOfMeasure, StringComparison.OrdinalIgnoreCase));

                                    if (unitsAreUndefinedOrEqual)
                                    {
                                        percent1 += row.Percent1;
                                        percent2 += row.Percent2;
                                        discountAmount += row.Amount;
                                        minQty += row.QuantityAmountFrom;
                                    }

                                    if (!row.SearchAgain)
                                    {
                                        idx = 9;
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            NetTracer.Error(ex, "Discount::GetLineDiscountLines failed for retailTransaction {0} saleItem {1} currencyCode {2}", retailTransaction, saleItem, currencyCode);
                            LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                            throw;
                        }
                    }
                }

                idx++;
            }
        }

        /// <summary>
        /// The calculation of a customer line discount.
        /// </summary>
        /// <param name="retailTransaction">The retail transaction.</param>
        /// <returns>The retail transaction.</returns>
        private IRetailTransaction CalcLineDiscount(RetailTransaction retailTransaction)
        {
            //Loop trough all items all calc line discount
            foreach (SaleLineItem saleItem in retailTransaction.CalculableSalesLines)
            {
                decimal absQty = Math.Abs(saleItem.Quantity);
                decimal discountAmount = 0m;
                decimal percent1 = 0m;
                decimal percent2 = 0m;
                decimal minQty = 0m;

                GetLineDiscountLines(retailTransaction, saleItem, retailTransaction.StoreCurrencyCode, ref absQty, ref discountAmount, ref percent1, ref percent2, ref minQty);

                if (percent1 == 0M
                    && percent2 == 0M
                    && discountAmount == 0M
                    && (ApplicationSettings.Terminal.StoreCurrency != ApplicationSettings.Terminal.CompanyCurrency))
                {
                    GetLineDiscountLines(retailTransaction, saleItem, ApplicationSettings.Terminal.CompanyCurrency, ref absQty, ref discountAmount, ref percent1, ref percent2, ref minQty);
                    discountAmount = this.Application.Services.Currency.CurrencyToCurrency(ApplicationSettings.Terminal.CompanyCurrency, ApplicationSettings.Terminal.StoreCurrency, discountAmount);
                }

                decimal totalPercentage = (1 - (1 - (percent1 / 100)) * (1 - (percent2 / 100))) * 100;

                if (((totalPercentage != 0m) || (discountAmount != 0m)) && (!saleItem.DiscountsWereRemoved))
                {

                    CustomerDiscountItem discountItem = (CustomerDiscountItem)this.Utility.CreateCustomerDiscountItem();
                    discountItem.LineDiscountType = DiscountTypes.Customer;
                    discountItem.CustomerDiscountType = CustomerDiscountTypes.LineDiscount;
                    discountItem.Percentage = totalPercentage;
                    discountItem.Amount = discountAmount;

                    UpdateDiscountLines(saleItem, discountItem);
                }
            }

            return retailTransaction;
        }

        /// <summary>
        /// The calculation of a customer multiline discount.
        /// </summary>
        /// <param name="retailTransaction">The retail transaction.</param>
        /// <returns>The retail transaction.</returns>
        private IRetailTransaction CalcMultiLineDiscount(RetailTransaction retailTransaction)
        {
            string storeCurrency = ApplicationSettings.Terminal.StoreCurrency;
            string companyCurrency = ApplicationSettings.Terminal.CompanyCurrency;
            var multilineDiscountCalculator = new MultilineDiscountCalculator(this.DiscountParameters, this.Application, this, storeCurrency, companyCurrency);

            return multilineDiscountCalculator.CalcMultiLineDiscount(retailTransaction);
        }

        /// <summary>
        /// The calculation of the total customer discount.
        /// </summary>
        /// <param name="retailTransaction">The retail transaction.</param>
        /// <returns>The retail transaction.</returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfather")]
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Needs refactoring")]
        private IRetailTransaction CalcTotalCustomerDiscount(RetailTransaction retailTransaction)
        {
            decimal totalAmount = 0;

            RetailTransaction retailTransactionClone = (RetailTransaction)retailTransaction.CloneTransaction();

            //Find the total amount as a basis for the total discount
            foreach (ISaleLineItem saleItem in retailTransactionClone.CalculableSalesLines)
            {
                if ((saleItem.IncludedInTotalDiscount) && (!saleItem.DiscountsWereRemoved) && (!saleItem.Voided))
                {
                    saleItem.CalculateLine();
                    totalAmount += saleItem.NetAmountWithAllInclusiveTax;
                }
            }

            decimal absTotalAmount = Math.Abs(totalAmount);

            //Find the total discounts.
            PriceDiscType relation = PriceDiscType.EndDiscSales; //Total sales discount - 7
            int itemCode = (int)PriceDiscItemCode.All; //All items - 2
            int accountCode = 0;
            string itemRelation = string.Empty;
            decimal percent1 = 0m;
            decimal percent2 = 0m;
            decimal discountAmount = 0m;
            Dimensions dimension = (Dimensions)this.Utility.CreateDimension();

            int idx = 0;
            while (idx < /* Max(PriceDiscAccountCode) */ 3)
            {   // Check discounts for Store Currency 
                accountCode = idx;

                string accountRelation = (accountCode == (int)PriceDiscAccountCode.Table) ? retailTransaction.Customer.CustomerId : (accountCode == (int)PriceDiscAccountCode.GroupId) ? retailTransaction.Customer.TotalDiscountGroup : string.Empty;
                accountRelation = accountRelation ?? String.Empty;

                // Only get Active discount combinations
                if (DiscountParameters.Activation(relation, (PriceDiscAccountCode)accountCode, (PriceDiscItemCode)itemCode))
                {
                    var priceDiscTable = GetPriceDiscDataCached(retailTransaction.TransactionId, relation, itemRelation, accountRelation, itemCode, accountCode, absTotalAmount, retailTransaction.StoreCurrencyCode, dimension, false);

                    foreach (DiscountAgreementArgs row in priceDiscTable)
                    {
                        percent1 += row.Percent1;
                        percent2 += row.Percent2;
                        discountAmount += row.Amount;

                        if (!row.SearchAgain)
                        {
                            idx = 3;
                        }
                    }
                }
                idx++;
            }

            if (percent1 == 0M && percent2 == 0M && discountAmount == 0M && (ApplicationSettings.Terminal.CompanyCurrency != ApplicationSettings.Terminal.StoreCurrency))
            {
                idx = 0;
                while (idx < /* Max(PriceDiscAccountCode) */ 3)
                {   // Check discounts for Company Currency 
                    accountCode = idx;

                    string accountRelation = (accountCode == (int)PriceDiscAccountCode.Table) ? retailTransaction.Customer.CustomerId : (accountCode == (int)PriceDiscAccountCode.GroupId) ? retailTransaction.Customer.TotalDiscountGroup : string.Empty;

                    // Only get Active discount combinations
                    if (DiscountParameters.Activation(relation, (PriceDiscAccountCode)accountCode, (PriceDiscItemCode)itemCode))
                    {
                        var priceDiscTable = GetPriceDiscDataCached(retailTransaction.TransactionId, relation, itemRelation, accountRelation, itemCode, accountCode, absTotalAmount, ApplicationSettings.Terminal.CompanyCurrency, dimension, false);

                        foreach (DiscountAgreementArgs row in priceDiscTable)
                        {
                            percent1 += row.Percent1;
                            percent2 += row.Percent2;
                            discountAmount += row.Amount;

                            if (!row.SearchAgain)
                            {
                                idx = 3;
                            }
                        }
                    }

                    idx++;
                }

                discountAmount = this.Application.Services.Currency.CurrencyToCurrency(ApplicationSettings.Terminal.CompanyCurrency, ApplicationSettings.Terminal.StoreCurrency, discountAmount);
            }

            decimal totalPercentage = (1 - (1 - (percent1 / 100)) * (1 - (percent2 / 100))) * 100;

            if (discountAmount != decimal.Zero)
            {
                retailTransaction.AddTotalDiscAmountLines(typeof(CustomerDiscountItem), discountAmount);
            }

            if (totalPercentage != 0)
            {
                //Update the sale items.
                foreach (SaleLineItem saleItem in retailTransaction.CalculableSalesLines)
                {
                    if (saleItem.IncludedInTotalDiscount)
                    {
                        CustomerDiscountItem discountItem = saleItem.GetCustomerDiscountItem(CustomerDiscountTypes.TotalDiscount, DiscountTypes.Customer);
                        discountItem.Percentage = totalPercentage;
                    }
                }
            }

            return retailTransaction;
        }

        internal ReadOnlyCollection<DiscountAgreementArgs> GetPriceDiscDataCached(string cacheToken, PriceDiscType relation,
            string itemRelation, string accountRelation, int itemCode, int accountCode,
            decimal quantityAmount, string targetCurrencyCode, Dimensions itemDimensions, bool includeDimensions)
        {
            if (!cacheToken.Equals(priceDiscDataCacheToken, StringComparison.OrdinalIgnoreCase))
            {   // Clear the cache and update the token
                priceDiscDataCache.Clear();
                priceDiscDataCacheToken = cacheToken;
            }
            else
            {   // Check the cache...

                // Linq Solution (best measured over hash, for-each)
                var tmp = (from item in priceDiscDataCache
                           where
                               (item.Relation == relation) &&
                               (item.ItemRelation == itemRelation) &&
                               (item.AccountRelation == accountRelation) &&
                               (item.ItemCode == itemCode) &&
                               (item.AccountCode == accountCode) &&
                               (item.QuantityAmount == quantityAmount) &&
                               (item.TargetCurrencyCode == targetCurrencyCode) &&
                               (item.ItemDimensions.IsEquivalent(itemDimensions)) &&
                               (item.IncludeDimensions == includeDimensions)
                           select item).FirstOrDefault();

                if (tmp != null)
                {
                    return tmp.SqlResults;
                }
            }

            // Item not found in cache - Get from SQL and add to the cache
            var result = GetPriceDiscData(relation, itemRelation, accountRelation, itemCode, accountCode, quantityAmount, targetCurrencyCode, itemDimensions, includeDimensions);
            priceDiscDataCache.Add(new PriceDiscData(result, relation, itemRelation, accountRelation, itemCode, accountCode, quantityAmount, targetCurrencyCode, itemDimensions, includeDimensions));

            return result;
        }

        internal struct DiscountAgreementArgs
        {
            public Decimal Percent1 { get; set; }
            public Decimal Percent2 { get; set; }
            public Decimal Amount { get; set; }
            public Decimal QuantityAmountFrom { get; set; }
            public String UnitId { get; set; }
            //            public Decimal QuantityAmountTo { get; set; }
            public bool SearchAgain { get; set; }
            //public string InventDimId { get; set; }
            //public string InventStyleId { get; set; }
            //public string InventColorId { get; set; }
            //public string InventSizeid { get; set; }
        }

        /// <summary>
        /// Gets the discount data.
        /// </summary>
        /// <remarks>Caller is responsible for disposing returned object</remarks>
        /// <param name="relation">The relation (line,mulitline,total)</param>
        /// <param name="itemRelation">The item relation</param>
        /// <param name="accountRelation">The account relation</param>
        /// <param name="itemCode">The item code (table,group,all)</param>
        /// <param name="accountCode">The account code(table,group,all)</param>
        /// <param name="quantityAmount">The quantity or amount that sets the minimum quantity or amount needed</param>
        /// <param name="targetCurrencyCode">The store or company currency</param>
        /// <returns></returns>
        private ReadOnlyCollection<DiscountAgreementArgs> GetPriceDiscData(
            PriceDiscType relation,
            string itemRelation,
            string accountRelation,
            int itemCode,
            int accountCode,
            decimal quantityAmount,
            string targetCurrencyCode,
            Contracts.DataEntity.IDimension itemDimensions,
            bool includeDimensions)
        {
            accountRelation = accountRelation ?? string.Empty;
            itemRelation = itemRelation ?? string.Empty;
            targetCurrencyCode = targetCurrencyCode ?? string.Empty;
            string inventColorId = ((itemDimensions.ColorId != null && includeDimensions) ? itemDimensions.ColorId : string.Empty);
            string inventSizeId = ((itemDimensions.SizeId != null && includeDimensions) ? itemDimensions.SizeId : string.Empty);
            string inventStyleId = ((itemDimensions.StyleId != null && includeDimensions) ? itemDimensions.StyleId : string.Empty);

            var discountAgreements = this.discountDataManager.GetPriceDiscData((int)relation,
                itemRelation, accountRelation, itemCode, accountCode,
                quantityAmount, targetCurrencyCode, inventColorId, inventSizeId, inventStyleId, this.NoDate);

            // can't use initializer in select of Linq-to-Entities query
            return discountAgreements.Select(t => new DiscountAgreementArgs
            {
                Percent1 = t.Percent1,
                Percent2 = t.Percent2,
                Amount = t.Amount,
                UnitId = t.UnitId,
                QuantityAmountFrom = t.QuantityAmountFrom,
                //                QuantityAmountTo = t.QuantityAmountTo,
                SearchAgain = t.ShouldSearchAgain == 0 ? false : true
            }).ToList().AsReadOnly();
        }

        /// <summary>
        /// Update the discount items.
        /// </summary>
        /// <param name="saleItem">The item line that the discount line is added to.</param>
        /// <param name="discountItem">The new discount item</param>
        internal static void UpdateDiscountLines(SaleLineItem saleItem, CustomerDiscountItem discountItem)
        {
            //Check if line discount is found, if so then update
            bool discountLineFound = false;
            foreach (DiscountItem discLine in saleItem.DiscountLines)
            {
                CustomerDiscountItem customerDiscLine = discLine as CustomerDiscountItem;
                if (customerDiscLine != null)
                {
                    //If found then update
                    if ((customerDiscLine.LineDiscountType == discountItem.LineDiscountType) &&
                        (customerDiscLine.CustomerDiscountType == discountItem.CustomerDiscountType))
                    {
                        customerDiscLine.Percentage = discountItem.Percentage;
                        customerDiscLine.Amount = discountItem.Amount;
                        discountLineFound = true;
                    }
                }
            }
            //If line discount is not found then add it.
            if (!discountLineFound)
            {
                saleItem.Add(discountItem);
            }

            saleItem.WasChanged = true;
        }

        /// <summary>
        /// Is there a valid relation between the itemcode and relation?
        /// </summary>
        /// <param name="itemCode">The item code (table,group,all)</param>
        /// <param name="relation">The item relation</param>
        /// <returns>Returns true if the relation ok, else false.</returns>
        internal static bool ValidRelation(PriceDiscItemCode itemCode, string relation)
        {
            System.Diagnostics.Debug.Assert(relation != null, "relation should not be null.");

            bool ok = true;

            if (!string.IsNullOrEmpty(relation) && (itemCode == PriceDiscItemCode.All))
            {
                ok = false;
            }

            if (string.IsNullOrEmpty(relation) && (itemCode != PriceDiscItemCode.All))
            {
                ok = false;
            }

            return ok;
        }

        /// <summary>
        /// Is there a valid relation between the accountcode and relation?
        /// </summary>
        /// <param name="accountCode">The account code (table,group,all).</param>
        /// <param name="relation">The account relation.</param>
        /// <returns></returns>
        internal static bool ValidRelation(PriceDiscAccountCode accountCode, string relation)
        {
            System.Diagnostics.Debug.Assert(relation != null, "relation should not be null.");

            bool ok = true;

            if (!string.IsNullOrEmpty(relation) && (accountCode == PriceDiscAccountCode.All))
            {
                ok = false;
            }

            if (string.IsNullOrEmpty(relation) && (accountCode != PriceDiscAccountCode.All))
            {
                ok = false;
            }

            return ok;
        }

        #endregion CustomerDiscount

        #region PeriodicDiscount

        /// <summary>
        /// Find the discount validation period and determines if it is active for the given date and time
        /// </summary>
        /// <param name="validationPeriod">Period Id of the validation period to check</param>
        /// <param name="transDateTime">Date/time to validate</param>
        /// <returns>Boolean indicating whether the validation period is active for the given date and time</returns>
        public bool IsValidationPeriodActive(string validationPeriodId, DateTime transDateTime)
        {
            if (string.IsNullOrEmpty(validationPeriodId))  //then it is always a valid period
            {
                NetTracer.Information("validationPeriodId parameter is null");
                return true;
            }

            DE.RetailDiscountValidationPeriod validationPeriod = discountDataManager.GetDiscValidationPeriod(validationPeriodId);
            DateTime transDate = transDateTime.Date;
            TimeSpan transTime = transDateTime.TimeOfDay;

            if (validationPeriod == null) //If validation period is not found
            {
                NetTracer.Information("validationPeriod is null");
                return true;
            }

            //Is the discount valid within the start and end date period?
            if (IsDateWithinStartEndDate(transDate, validationPeriod.ValidFrom.Date, validationPeriod.ValidTo.Date))
            {
                bool answerFound = false;
                bool isActive = false;

                // does today's configuration tell if period is active?
                if (IsRangeDefinedForDay(validationPeriod, transDate.DayOfWeek))
                {
                    isActive = IsPeriodActiveForDayAndTime(validationPeriod, transDate.DayOfWeek, transTime, false);
                    answerFound = true;
                }

                // if we don't know or got negative result, see if yesterday will activate it (if its range ends after midnight)
                DayOfWeek yesterday = transDate.AddDays(-1).DayOfWeek;
                bool lastRangeDefinedAfterMidnight =
                IsRangeDefinedForDay(validationPeriod, yesterday) && validationPeriod.IsEndTimeAfterMidnightForDay(yesterday);

                if ((!answerFound || isActive == false) && lastRangeDefinedAfterMidnight)
                {
                    // if yesterday makes it active, set isActive = true
                    isActive = IsPeriodActiveForDayAndTime(validationPeriod, yesterday, transTime, true);
                    answerFound = true;
                }

                // if we still don't know, try using general configuration
                if (!answerFound)
                {
                    var configuration = new PeriodRangeConfiguration
                    {
                        StartTime = validationPeriod.StartingTime,
                        EndTime = validationPeriod.EndingTime,
                        IsActiveOnlyWithinBounds = validationPeriod.IsTimeBounded != 0,
                        EndsTomorrow = validationPeriod.IsEndTimeAfterMidnight != 0
                    };

                    if ((validationPeriod.StartingTime != 0) && (validationPeriod.EndingTime != 0))
                    {
                        int currentTime = Convert.ToInt32(transTime.TotalSeconds);
                        isActive = IsTimeActiveForConfiguration(currentTime, configuration, false);
                        answerFound = true;
                    }
                }

                return answerFound ? isActive : (validationPeriod.IsTimeBounded == 1);
            }

            // not within date range, so active if not set to be within date range
            return validationPeriod.IsTimeBounded != 1;
        }

        private static bool IsRangeDefinedForDay(DE.RetailDiscountValidationPeriod period, DayOfWeek day)
        {
            return (period.StartingTimeForDay(day) != 0) && (period.EndingTimeForDay(day) != 0);
        }

        private static bool IsPeriodActiveForDayAndTime(DE.RetailDiscountValidationPeriod period, DayOfWeek day, TimeSpan time, bool testOnlyAfterMidnight)
        {
            var configuration = new PeriodRangeConfiguration
            {
                StartTime = period.StartingTimeForDay(day),
                EndTime = period.EndingTimeForDay(day),
                EndsTomorrow = period.IsEndTimeAfterMidnightForDay(day),
                IsActiveOnlyWithinBounds = period.IsTimeBoundedForDay(day),
            };

            Int32 currentTime = Convert.ToInt32(time.TotalSeconds);
            return IsTimeActiveForConfiguration(currentTime, configuration, testOnlyAfterMidnight);
        }

        /// <summary>
        /// For a given time, and period time-range setup, and whether to restrict our search to after midnight,
        ///  this method tells if the given time is active or inactive within the context of the range
        /// </summary>
        /// <param name="currentTime">Current time in seconds past midnight</param>
        /// <param name="configuration">Period time range setup parameters</param>
        /// <param name="testOnlyAfterMidnight">Whether we only check for activity after midnight</param>
        /// <returns>Result telling if given time is active in the configuration</returns>
        private static bool IsTimeActiveForConfiguration(Int32 currentTime, PeriodRangeConfiguration configuration, bool testOnlyAfterMidnight)
        {
            // if time falls between start and end times, return true if set to be active in range
            bool rangeAppliesBeforeMidnight = (configuration.StartTime <= currentTime &&
                ((configuration.EndTime >= currentTime) || configuration.EndsTomorrow));

            if (!testOnlyAfterMidnight && rangeAppliesBeforeMidnight)
            {
                return configuration.IsActiveOnlyWithinBounds;
            }

            // if time is before end time for ending times past midnight, return true if set to be active in range
            bool rangeAppliesAfterMidnight = (configuration.EndsTomorrow && configuration.EndTime >= currentTime);

            if (rangeAppliesAfterMidnight)
            {
                return configuration.IsActiveOnlyWithinBounds;
            }

            return !configuration.IsActiveOnlyWithinBounds;
        }

        /// <summary>
        /// Represent a time-range configuration for discount validation period
        ///  These ranges have a start and end time, indicator for ending past midnight, and 
        ///  flag indicated what finding a time in this range means (i.e. whether being in the range validates/invalidates the time)
        /// </summary>
        private struct PeriodRangeConfiguration
        {
            public Int32 StartTime;
            public Int32 EndTime;
            public bool EndsTomorrow;
            public bool IsActiveOnlyWithinBounds;
        }

        /// <summary>
        /// //Finds if a discount period is valid.
        /// </summary>
        /// <param name="periodId">The unique period id.</param>
        /// <param name="transDateTime">The date and time for the transaction.</param>
        /// <returns></returns>
        private PeriodStatus DiscountPeriodValid(string periodId, DateTime transDateTime)
        {
            return IsValidationPeriodActive(periodId, transDateTime) ? PeriodStatus.IsValid : PeriodStatus.IsInvalid;
        }

        private void MakeActiveOfferTables()
        {
            //Adding colums to activeOffers
            activeOffers.Columns.Add("OFFERID", typeof(string));
            activeOffers.Columns.Add("DESCRIPTION", typeof(string));
            activeOffers.Columns.Add("STATUS", typeof(int));
            activeOffers.Columns.Add("PDTYPE", typeof(int));
            activeOffers.Columns.Add("CONCURRENCYMODE", typeof(int));
            activeOffers.Columns.Add("DISCVALIDPERIODID", typeof(string));
            activeOffers.Columns.Add("DATEVALIDATIONTYPE", typeof(DateValidationTypes));
            activeOffers.Columns.Add("VALIDFROM", typeof(DateTime));
            activeOffers.Columns.Add("VALIDTO", typeof(DateTime));
            activeOffers.Columns.Add("DISCOUNTTYPE", typeof(int));
            activeOffers.Columns.Add("SAMEDIFFMMLINES", typeof(int));
            activeOffers.Columns.Add("NOOFLINESTOTRIGGER", typeof(int));
            activeOffers.Columns.Add("DEALPRICEVALUE", typeof(decimal));
            activeOffers.Columns.Add("DISCOUNTPCTVALUE", typeof(decimal));
            activeOffers.Columns.Add("DISCOUNTAMOUNTVALUE", typeof(decimal));
            activeOffers.Columns.Add("NOOFLEASTEXPENSIVEITEMS", typeof(int));
            activeOffers.Columns.Add("NOOFTIMESAPPLICABLE", typeof(int));
            activeOffers.Columns.Add("NOLINESTRIGGERED", typeof(int));      // The number of lines that have been triggerd in an mix and match offer. Shoulb be equal or less than NoOfLinesToTrigger.
            activeOffers.Columns.Add("NOOFTIMESACTIVATED", typeof(int));    // The number times the offer has been activated. Should be equal or less than NoOfTimesApplicable
            activeOffers.Columns.Add("ISDISCOUNTCODEREQUIRED", typeof(int));
            activeOffers.Columns.Add("CURRENCYCODE", typeof(string));
            DataColumn[] primaryKey = new DataColumn[2];
            primaryKey[0] = activeOffers.Columns["OFFERID"];
            primaryKey[1] = activeOffers.Columns["PDTYPE"];
            activeOffers.PrimaryKey = primaryKey;

            //Adding columns to activeOfferLines
            activeOfferLines.Columns.Add("OFFERID", typeof(string));            // The offer id
            activeOfferLines.Columns.Add("LINEID", typeof(int));                // The offer line id
            activeOfferLines.Columns.Add("PRODUCTCATEGORY", typeof(Int64));     // The product Category on offer line
            activeOfferLines.Columns.Add("PRODUCTID", typeof(Int64));
            activeOfferLines.Columns.Add("DISTINCTPRODUCTVARIANTID", typeof(Int64));
            activeOfferLines.Columns.Add("SALELINEID", typeof(int));            // The retailtransaction sale line id
            activeOfferLines.Columns.Add("QUANTITY", typeof(decimal));          // The item quantity
            activeOfferLines.Columns.Add("UNIT", typeof(string));               // The item unit (UOM)
            activeOfferLines.Columns.Add("DEALPRICEORDISCPCT", typeof(decimal));// The deal price or discount percentage
            activeOfferLines.Columns.Add("LINEGROUP", typeof(string));
            activeOfferLines.Columns.Add("DISCTYPE", typeof(int));
            activeOfferLines.Columns.Add("STATUS", typeof(int));
            activeOfferLines.Columns.Add("NOOFITEMSNEEDED", typeof(int));
            activeOfferLines.Columns.Add("MIXMATCHPRIORITY", typeof(decimal)); // The order priority for the mix and match lines, higher values give higher priority

            // columns for Discount Offer specific details
            activeOfferLines.Columns.Add("DISCOUNTMETHOD", typeof(int));    // Dicount Offer method
            activeOfferLines.Columns.Add("DISCAMOUNT", typeof(decimal));    // Discount amount
            activeOfferLines.Columns.Add("DISCPCT", typeof(decimal));       // Discount percent
            activeOfferLines.Columns.Add("OFFERPRICE", typeof(decimal));    // Offer price
            activeOfferLines.Columns.Add("OFFERPRICEINCLTAX", typeof(decimal)); // Offer Price (w/ tax)

            // Primary Key
            DataColumn[] primaryLineKey = new DataColumn[7];
            primaryLineKey[0] = activeOfferLines.Columns["OFFERID"];
            primaryLineKey[1] = activeOfferLines.Columns["PRODUCTCATEGORY"];
            primaryLineKey[2] = activeOfferLines.Columns["PRODUCTID"];
            primaryLineKey[3] = activeOfferLines.Columns["DISTINCTPRODUCTVARIANTID"];
            primaryLineKey[4] = activeOfferLines.Columns["SALELINEID"];
            primaryLineKey[5] = activeOfferLines.Columns["DEALPRICEORDISCPCT"];
            primaryLineKey[6] = activeOfferLines.Columns["UNIT"];
            activeOfferLines.PrimaryKey = primaryLineKey;
        }

        private void MakeTmpOfferTable()
        {
            tmpMMOffer.Columns.Add("SALELINEID", typeof(int));
            tmpMMOffer.Columns.Add("ITEMSTRIGGERED", typeof(int));
            tmpMMOffer.Columns.Add("DISCTYPE", typeof(int));
            tmpMMOffer.Columns.Add("DEALPRICEORDISCPCT", typeof(decimal));
            tmpMMOffer.Columns.Add("PRICE", typeof(decimal));
            DataColumn[] pk = new DataColumn[1];
            pk[0] = tmpMMOffer.Columns["SALELINEID"];
            tmpMMOffer.PrimaryKey = pk;
        }

        private void MakeTmpDiscountCodeTable()
        {
            tmpDiscountCode.Columns.Add("DISCOUNTCODE", typeof(String));
            tmpDiscountCode.Columns.Add("BARCODE", typeof(String));
            tmpDiscountCode.Columns.Add("DISCOUNTOFFERID", typeof(String));
        }

        // Duplicated in Price.cs.  Refactor whenever change in architecture allows.
        private bool IsDateWithinStartEndDate(DateTime dateToCheck, DateTime startDate, DateTime endDate)
        {
            return (((dateToCheck >= startDate) || (startDate == NoDate))
                && ((dateToCheck <= endDate) || (endDate == NoDate)));
        }

        /// <summary>
        /// Loops through the transaction to find offers that the items are in.
        /// </summary>
        /// <param name="retailTransaction"></param>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfather")]
        private void FindPeriodicOffers(RetailTransaction retailTransaction)
        {
            activeOffers.Clear();
            activeOfferLines.Clear();

            PeriodicDiscount cache = retailTransaction.PeriodicDiscount;

            //Find all the active periodic offers 
            foreach (SaleLineItem saleItem in retailTransaction.CalculableSalesLines)
            {
                if (saleItem.NoDiscountAllowed)
                {
                    continue;
                }

                // Update the cache for this item if needed.
                UpdateCache(cache, saleItem);

                //Get the data for the item
                DataTable periodicDiscountData = cache.Get(
                    saleItem.ProductId,
                    saleItem.Dimension.DistinctProductVariantId);

                AddPeriodicDiscountsToCalculation(retailTransaction, saleItem, periodicDiscountData);
            }
        }

        private PeriodStatus GetPeriodicDiscountPeriodStatus(RetailTransaction retailTransaction, BaseSaleItem saleItem, DataRow periodicDiscount)
        {
            string discValidPeriodId = (string)periodicDiscount["DISCVALIDPERIODID"];
            DateValidationTypes dateValidationType = (DateValidationTypes)periodicDiscount["DATEVALIDATIONTYPE"];

            PeriodStatus periodStatus = PeriodStatus.IsInvalid;
            switch (dateValidationType)
            {
                case DateValidationTypes.Advanced:
                    periodStatus = retailTransaction.Period.IsValid(discValidPeriodId);
                    if (periodStatus == PeriodStatus.NotFoundInMemoryTable)
                    {
                        periodStatus = DiscountPeriodValid(discValidPeriodId, DateTime.Now);
                        retailTransaction.Period.Add(discValidPeriodId, (periodStatus == PeriodStatus.IsValid));
                    }
                    break;
                case DateValidationTypes.Standard:
                    periodStatus = IsDateWithinStartEndDate(DateTime.Now.Date, (DateTime)periodicDiscount["VALIDFROM"], (DateTime)periodicDiscount["VALIDTO"])
                        ? PeriodStatus.IsValid : PeriodStatus.IsInvalid;
                    break;

                default:
                    NetTracer.Warning("Discount::GetPeriodicDiscountPeriodStatus: Invalid Discount Validation Type (retailTransaction {0} saleItem {1}", retailTransaction.TransactionId, saleItem.ItemId);
                    System.Diagnostics.Debug.Fail("Invalid Discount Validation Type");
                    break;
            }
            return periodStatus;
        }

        private static bool IsLoyaltyCardDiscountApplicable(RetailTransaction retailTransaction, DataRow periodicDiscount)
        {

            bool loyaltyCardRequired = DBUtil.ToBool(periodicDiscount["LOYALTYCARDREQUIRED_RU"]);
            string loyaltySchemeId = DBUtil.ToStr(periodicDiscount["LOYALTYSCHEMEID_RU"]);
            decimal minPoints = DBUtil.ToDecimal(periodicDiscount["LOYALTYPOINTSMIN_RU"]);
            decimal maxPoints = DBUtil.ToDecimal(periodicDiscount["LOYALTYPOINTSMAX_RU"]);

            if (!loyaltyCardRequired)
                return true;

            if (retailTransaction.LoyaltyItem == null || string.IsNullOrWhiteSpace(retailTransaction.LoyaltyItem.LoyaltyCardNumber))
                return false;

            if (!string.IsNullOrWhiteSpace(loyaltySchemeId) && retailTransaction.LoyaltyItem.SchemeID != loyaltySchemeId)
                return false;

            return (retailTransaction.LoyaltyItem.AccumulatedLoyaltyPoints >= minPoints &&
                    (maxPoints == 0 || retailTransaction.LoyaltyItem.AccumulatedLoyaltyPoints <= maxPoints));

        }

        private bool IsPeriodicDiscountApplicable(RetailTransaction retailTransaction, BaseSaleItem saleItem, DataRow periodicDiscount)
        {
            bool result = GetPeriodicDiscountPeriodStatus(retailTransaction, saleItem, periodicDiscount) == PeriodStatus.IsValid
                           && !(PeriodicDiscOfferType.MixAndMatch == (PeriodicDiscOfferType)periodicDiscount["PDTYPE"]
                               && (saleItem.PriceOverridden || saleItem.IsManuallyDiscounted));

            result = result && IsLoyaltyCardDiscountApplicable(retailTransaction, periodicDiscount);
            return result;
        }

        /// <summary>
        /// Adds given discount data to transaction to be computed for the given sales item
        /// </summary>
        /// <param name="retailTransaction">Transaction with the discount and item</param>
        /// <param name="saleItem">The sale item on the transaction which the discount data applies to</param>
        /// <param name="periodicDiscountData">The discount data, which should be considered for calculation.</param>
        public void AddPeriodicDiscountsToCalculation(RetailTransaction retailTransaction, BaseSaleItem saleItem, DataTable periodicDiscountData)
        {
            if (retailTransaction == null)
            {
                NetTracer.Warning("retailTransaction parameter is null");
                throw new ArgumentNullException("retailTransaction");
            }
            if (saleItem == null)
            {
                NetTracer.Warning("saleItem parameter is null");
                throw new ArgumentNullException("saleItem");
            }
            if (periodicDiscountData == null)
            {
                NetTracer.Warning("periodicDiscountData parameter is null");
                throw new ArgumentNullException("periodicDiscountData");
            }
            //Loop through the offers found for the item
            foreach (DataRow row in periodicDiscountData.Rows)
            {
                if (IsPeriodicDiscountApplicable(retailTransaction, saleItem, row))
                {
                    try
                    {
                        string filterExpr = string.Format("OFFERID='{0}'", row["OFFERID"]);
                        DataRow[] dr = activeOffers.Select(filterExpr);
                        // If has not been added yet to active offers
                        if (dr.Length == 0)
                        {
                            DataRow offerRow;
                            offerRow = activeOffers.NewRow();
                            offerRow["OFFERID"] = row["OFFERID"];
                            offerRow["DESCRIPTION"] = row["DESCRIPTION"];
                            offerRow["PDTYPE"] = row["PDTYPE"];
                            offerRow["CONCURRENCYMODE"] = row["CONCURRENCYMODE"];
                            offerRow["CURRENCYCODE"] = row["CURRENCYCODE"];
                            offerRow["DISCVALIDPERIODID"] = row["DISCVALIDPERIODID"];
                            offerRow["DATEVALIDATIONTYPE"] = row["DATEVALIDATIONTYPE"];
                            offerRow["VALIDFROM"] = row["VALIDFROM"];
                            offerRow["VALIDTO"] = row["VALIDTO"];
                            offerRow["DISCOUNTTYPE"] = row["DISCOUNTTYPE"];
                            offerRow["SAMEDIFFMMLINES"] = row["SAMEDIFFMMLINES"];
                            offerRow["NOOFLINESTOTRIGGER"] = row["NOOFLINESTOTRIGGER"];
                            offerRow["DEALPRICEVALUE"] = row["DEALPRICEVALUE"];
                            offerRow["DISCOUNTPCTVALUE"] = row["DISCOUNTPCTVALUE"];
                            offerRow["DISCOUNTAMOUNTVALUE"] = row["DISCOUNTAMOUNTVALUE"];
                            offerRow["NOOFLEASTEXPENSIVEITEMS"] = row["NOOFLEASTEXPITEMS"];
                            offerRow["NOOFTIMESAPPLICABLE"] = row["NOOFTIMESAPPLICABLE"];
                            offerRow["NOLINESTRIGGERED"] = 0;
                            offerRow["NOOFTIMESACTIVATED"] = 0;
                            offerRow["ISDISCOUNTCODEREQUIRED"] = row["ISDISCOUNTCODEREQUIRED"];
                            activeOffers.Rows.Add(offerRow);
                        }

                        filterExpr = string.Format(
                            "OFFERID='{0}' AND PRODUCTID='{1}' AND DISTINCTPRODUCTVARIANTID = '{2}' AND SALELINEID='{3}' AND DEALPRICEORDISCPCT='{4}' AND UNIT='{5}'",
                            row["OFFERID"], row["PRODUCTID"], row["DISTINCTPRODUCTVARIANTID"], saleItem.LineId.ToString(), row["DEALPRICEORDISCPCT"], row["UNIT"]);

                        DataRow[] aol = activeOfferLines.Select(filterExpr);
                        if (aol.Length == 0)
                        {
                            DataRow offerLineRow;
                            offerLineRow = activeOfferLines.NewRow();
                            offerLineRow["OFFERID"] = row["OFFERID"];
                            offerLineRow["LINEID"] = row["LINEID"];
                            offerLineRow["PRODUCTCATEGORY"] = row["PRODUCTCATEGORY"];
                            offerLineRow["PRODUCTID"] = row["PRODUCTID"];
                            offerLineRow["DISTINCTPRODUCTVARIANTID"] = row["DISTINCTPRODUCTVARIANTID"];
                            offerLineRow["SALELINEID"] = saleItem.LineId;
                            offerLineRow["QUANTITY"] = saleItem.Quantity;
                            offerLineRow["UNIT"] = row["UNIT"];
                            offerLineRow["DEALPRICEORDISCPCT"] = row["DEALPRICEORDISCPCT"];
                            offerLineRow["LINEGROUP"] = row["LINEGROUP"];
                            offerLineRow["DISCTYPE"] = row["DISCTYPE"];
                            offerLineRow["STATUS"] = row["STATUS"];

                            int noOfItemsNeeded = (row["NOOFITEMSNEEDED"] == System.DBNull.Value) ? (0) : (int)row["NOOFITEMSNEEDED"];

                            // MixMatch priority should be based ONLY on the individual unit price, this prevents
                            // groups of cheaper items getting rated 'higher/more-expensive' than a single higher priced item.
                            // For example 3x$10 > 1x$20.  The $10 item should be selected for 'least expensive', even though the "extended" price is higher.
                            decimal priority = saleItem.Price;

                            offerLineRow["NOOFITEMSNEEDED"] = noOfItemsNeeded;
                            offerLineRow["MIXMATCHPRIORITY"] = priority;

                            // Discount Offer line details
                            offerLineRow["DISCOUNTMETHOD"] = row["DISCOUNTMETHOD"];
                            offerLineRow["DISCAMOUNT"] = row["DISCAMOUNT"];
                            offerLineRow["DISCPCT"] = row["DISCPCT"];
                            offerLineRow["OFFERPRICE"] = row["OFFERPRICE"];
                            offerLineRow["OFFERPRICEINCLTAX"] = row["OFFERPRICEINCLTAX"];

                            activeOfferLines.Rows.Add(offerLineRow);
                        }
                    }
                    catch (Exception ex)
                    {
                        NetTracer.Error(ex, "Discount::AddPeriodicDiscountsToCalculation failed for retailTransaction {0} saleItem {1}", retailTransaction.TransactionId, saleItem.ItemId);
                        LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                        throw;
                    }
                }
            }
        }

        /// <summary>
        /// Check whether or not an item's offers are already in the cache, and add them from the DB if necessary.
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="saleItem"></param>
        private void UpdateCache(PeriodicDiscount cache, SaleLineItem saleItem)
        {
            if (!cache.HasItem(saleItem.ProductId, saleItem.Dimension.DistinctProductVariantId))
            {
                // If not in cache retrieve from DB and add to cache
                ReadOnlyCollection<PeriodicDiscountTemporaryData> periodicDiscountData = GetPeriodicDiscountData(saleItem.ProductId, saleItem.Dimension.DistinctProductVariantId);
                List<string> offerIds = new List<string>();
                foreach (var discount in periodicDiscountData)
                {
                    offerIds.Add(discount.OfferId);
                }
                ReadOnlyCollection<DiscountCodeDataTemporaryData> discountCodeData = GetDiscountCodesForOffer(offerIds);
                cache.Add(periodicDiscountData, saleItem.ProductId, saleItem.Dimension.DistinctProductVariantId, discountCodeData);
            }
        }

        /// <summary>
        /// Loops through the active offers in priority order.  
        /// Starting with offers with the highest order(lowest number) first. 
        /// </summary>
        /// <param name="retailtransaction">The retailtransaction.</param>
        public RetailTransaction RegisterPeriodicDisc(RetailTransaction retailTransaction)
        {
            bool isDiscountValid = true;
            foreach (DataRow row in activeOffers.Select(string.Empty, "CONCURRENCYMODE ASC, OFFERID ASC"))
            {
                isDiscountValid = true;
                string offerId = (string)row["OFFERID"];
                string offerName = (string)row["DESCRIPTION"];
                bool isDiscountCodeRequired = Convert.ToBoolean((int)row["ISDISCOUNTCODEREQUIRED"]);
                string discountCode = string.Empty;
                DiscountMethodType discType = (DiscountMethodType)row["DISCOUNTTYPE"];
                PeriodicDiscOfferType discOfferType = (PeriodicDiscOfferType)row["PDTYPE"];
                ConcurrencyMode concurrencyMode = (ConcurrencyMode)row["CONCURRENCYMODE"];
                //Check if offer id need a discount code in order to get applied
                if (isDiscountCodeRequired)
                {
                    //if discount code is reqired, then get the required code from discountcode table against offerid and match up with the list of discount coupons available in the current transaction
                    //this region will only get executed when ISDISCOUNTCODEREQUIRED flag is set to true else will be skipped                    
                    tmpDiscountCode.Clear();
                    PeriodicDiscount cache = retailTransaction.PeriodicDiscount;
                    tmpDiscountCode = cache.GetDiscountOfferDetails(offerId);

                    //Match the discount codes avialble with the codes scanned at POS(stored in a List)
                    isDiscountValid = false;
                    foreach (DataRow dr in tmpDiscountCode.Select())
                    {
                        string candidateDiscountCode = dr["DISCOUNTCODE"].ToString();
                        if (retailTransaction.DiscountCodes.Contains(candidateDiscountCode))
                        {
                            isDiscountValid = true;
                            discountCode = candidateDiscountCode;
                            break;
                        }
                    }
                }
                if (isDiscountValid)
                {
                    switch (discOfferType)
                    {
                        case PeriodicDiscOfferType.MixAndMatch:
                            retailTransaction = CalcMixMatch(offerId, offerName, discountCode, row, retailTransaction, concurrencyMode);
                            break;

                        case PeriodicDiscOfferType.Multibuy:
                            retailTransaction = CalcMultiBuy(offerId, offerName, discountCode, retailTransaction, discType, concurrencyMode);
                            break;

                        case PeriodicDiscOfferType.Offer:
                            retailTransaction = CalcDiscountOffer(offerId, offerName, discountCode, retailTransaction, concurrencyMode);
                            break;
                    }
                }
            }
            return retailTransaction;
        }

        /// <summary>
        /// Calculate the periodic discounts for the transation.
        /// </summary>
        /// <param name="retailtransaction"></param>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Calc", Justification = "Cannot change public API.")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void CalcPeriodicDisc(RetailTransaction retailTransaction)
        {
            //Clear all the periodic discounts 
            retailTransaction.ClearPeriodicDiscounts();

            //Clear Customer discounts
            retailTransaction.ClearCustomerDiscounts();

            //Find all possible offfers
            FindPeriodicOffers(retailTransaction);

            //Calculate the periodic offers
            retailTransaction = RegisterPeriodicDisc(retailTransaction);

            //Apply concurrency rules to valid offers
            DiscountConcurrencyRules.ApplyConcurrencyRules(retailTransaction);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Caller is responsible for disposing returned object</remarks>
        /// <param name="productId"></param>
        /// <param name="variantId"></param>
        /// <returns></returns>
        private ReadOnlyCollection<PeriodicDiscountTemporaryData> GetPeriodicDiscountData(Int64 productId, Int64 variantId)
        {
            List<PeriodicDiscountTemporaryData> periodicDiscounts = new List<PeriodicDiscountTemporaryData>();
            DateTime today = DateTime.Now.Date;

            SqlConnection connection = Discount.InternalApplication.Settings.Database.Connection;
            string dataAreaId = Discount.InternalApplication.Settings.Database.DataAreaID;

            try
            {
                string queryString = string.Concat(
                    SelectFromPeriodicDiscount,  // Product or Variant, no category
                    @"JOIN RETAILGROUPMEMBERLINE rgl 
                        ON pd.RETAILGROUPMEMBERLINE = rgl.RECID AND
                         ((rgl.VARIANT != 0 AND rgl.VARIANT = @VARIANTID) OR
                          (rgl.VARIANT = 0 AND rgl.PRODUCT != 0 AND rgl.PRODUCT = @PRODUCTID))",
                    WherePeriodicDiscountAndLeftJoins,

                    SqlUnionAll,

                    SelectFromPeriodicDiscount, // Cateogry with product only
                    @"
                    JOIN RETAILGROUPMEMBERLINE rgl 
                        ON pd.RETAILGROUPMEMBERLINE = rgl.RECID AND rgl.VARIANT = 0 AND rgl.PRODUCT = 0 AND rgl.CATEGORY != 0
                    JOIN RETAILCATEGORYCONTAINMENTLOOKUP rccl ON rccl.CATEGORY = rgl.CATEGORY
                    JOIN ECORESPRODUCTCATEGORY pc on pc.CATEGORY = rccl.CONTAINEDCATEGORY AND pc.PRODUCT = @PRODUCTID",
                    WherePeriodicDiscountAndLeftJoins,

                    SqlUnionAll,

                    SelectFromPeriodicDiscount, // Category with variant only
                    @"
                    JOIN RETAILGROUPMEMBERLINE rgl 
                        ON pd.RETAILGROUPMEMBERLINE = rgl.RECID AND rgl.VARIANT = 0 AND rgl.PRODUCT = 0 AND rgl.CATEGORY != 0
                    JOIN RETAILCATEGORYCONTAINMENTLOOKUP rccl ON rccl.CATEGORY = rgl.CATEGORY
                    JOIN RETAILSPECIALCATEGORYVARIANT rscv on rscv.CATEGORY = rccl.CONTAINEDCATEGORY and rscv.PRODUCT = @VARIANTID",
                    WherePeriodicDiscountAndLeftJoins,

                    " ORDER BY pd.OFFERID, pd.LINENUM");

                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@STORECURRENCY", ApplicationSettings.Terminal.StoreCurrency);
                    command.Parameters.AddWithValue("@VARIANTID", variantId);
                    command.Parameters.AddWithValue("@PRODUCTID", productId);
                    command.Parameters.AddWithValue("@TODAY", today);
                    command.Parameters.AddWithValue("@NODATE", DateTime.Parse("1900-01-01"));

                    // convert store price group list to data-table for use as TVP in the query.
                    using (DataTable priceGroupTable = new DataTable())
                    {
                        priceGroupTable.Columns.Add("PRICEGROUPID", typeof(long));
                        foreach (long priceGroupId in this.storePriceGroups)
                        {
                            priceGroupTable.Rows.Add(priceGroupId);
                        }

                        // Fill out TVP for store price group list
                        SqlParameter param = command.Parameters.Add("@STOREPRICEGROUPS", SqlDbType.Structured);
                        param.Direction = ParameterDirection.Input;
                        param.TypeName = "GETPRICEDISCOUNTDATA_PRICEGROUPS_TABLETYPE";
                        param.Value = priceGroupTable;

                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            PeriodicDiscountTemporaryData pdt = new PeriodicDiscountTemporaryData()
                            {
                                OfferId = (string)reader["OFFERID"],
                                Description = (string)reader["NAME"],
                                PeriodicDiscountType = (int)reader["PERIODICDISCOUNTTYPE"],
                                ConcurrencyMode = (int)reader["CONCURRENCYMODE"],
                                CurrencyCode = (string)reader["CURRENCYCODE"],
                                IsDiscountCodeRequired = (int)reader["ISDISCOUNTCODEREQUIRED"],
                                ValidationPeriodId = (string)reader["VALIDATIONPERIODID"],
                                DateValidationType = (int)reader["DATEVALIDATIONTYPE"],
                                ValidFrom = (DateTime)reader["VALIDFROM"],
                                ValidTo = (DateTime)reader["VALIDTO"],
                                DiscountType = (int)reader["DISCOUNTTYPE"],
                                NumberOfLinesToTrigger = (int)reader["NOOFLINESTOTRIGGER"],
                                DealPriceValue = (decimal)reader["DEALPRICEVALUE"],
                                DiscountPercentValue = (decimal)reader["DISCOUNTPERCENTVALUE"],
                                DiscountAmountValue = (decimal)reader["DISCOUNTAMOUNTVALUE"],
                                NumberOfLeastExpensiveLines = (int)reader["NOOFLEASTEXPENSIVELINES"],
                                NumberOfTimesApplicable = (int)reader["NUMBEROFTIMESAPPLICABLE"],
                                DiscountLineNumber = (decimal)reader["LINENUM"],
                                DiscountPercentOrValue = (decimal)reader["DISCOUNTPERCENTORVALUE"],
                                LoyaltyCardRequired_RU = (int)reader["LOYALTYCARDREQUIRED_RU"],
                                LoyaltySchemeId_RU = (string)reader["LOYALTYSCHEMEID_RU"],
                                LoyaltyPointsMin_RU = (decimal)reader["LOYALTYPOINTSMIN_RU"],
                                LoyaltyPointsMax_RU = (decimal)reader["LOYALTYPOINTSMAX_RU"],

                                LineGroup = (string)(reader["LINEGROUP"] ?? String.Empty),
                                DiscType = (int)(reader["MIXANDMATCHDISCOUNTTYPE"] ?? 0),
                                NumberOfItemsNeeded = (int)(reader["NUMBEROFITEMSNEEDED"] ?? 0),

                                DiscountMethod = (int)(reader["DISCOUNTMETHOD"] ?? 0),
                                DiscountAmount = (decimal)(reader["DISCAMOUNT"] ?? 0),
                                DiscountPercent = (decimal)(reader["DISCPCT"] ?? 0),
                                OfferPrice = (decimal)(reader["OFFERPRICE"] ?? 0),
                                OfferPriceIncludingTax = (decimal)(reader["OFFERPRICEINCLTAX"] ?? 0),

                                ProductCategory = (Int64)reader["CATEGORY"],
                                UnitOfMeasureSymbol = (string)(reader["SYMBOL"] ?? string.Empty),
                                DistinctProductVariant = (Int64)reader["VARIANT"]
                            };

                            periodicDiscounts.Add(pdt);
                        }

                        // Delete duplicate offer lines which variant is not 0 if a offer line of variant 0 exists in same offer.
                        periodicDiscounts.RemoveAll(p => (from r in periodicDiscounts
                                                          where r.DistinctProductVariant == 0
                                                             && p.DistinctProductVariant != 0
                                                             && r.OfferId == p.OfferId
                                                          select r).Any());
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

            if (ApplicationSettings.LogTraceLevel == LogTraceLevel.Trace)
            {
                var distinctOfferIds = periodicDiscounts.Select(p => p.OfferId).Distinct();
                LSRetailPosis.ApplicationLog.Log("Discount.GetPeriodicDiscountData()",
                    String.Format("Found {0} periodic discounts for product '{1}' (variant '{2}'):{3}",
                    distinctOfferIds.Count(), productId, variantId,
                    distinctOfferIds.Aggregate(new StringBuilder(), (ids, id) => ids.AppendFormat(" '{0}'", id)).ToString()),
                    LogTraceLevel.Trace);
            }

            return periodicDiscounts.AsReadOnly();
        }


        private ReadOnlyCollection<DiscountCodeDataTemporaryData> GetDiscountCodesForOffer(List<string> offerIds)
        {
            ReadOnlyCollection<DiscountCodeDataTemporaryData> discountCodeData = new List<DiscountCodeDataTemporaryData>().AsReadOnly();
            try
            {
                SqlConnection connection = Application.Settings.Database.Connection;
                string dataAreaId = Application.Settings.Database.DataAreaID;
                using (DM.PosDbContext posDb = new DM.PosDbContext(connection))
                {
                    var discountCodes = (from discountCode in posDb.RetailDiscountCodes
                                         where offerIds.Contains(discountCode.DiscountOfferId) && discountCode.DataAreaId == dataAreaId
                                         select discountCode).ToList();
                    discountCodeData = discountCodes.Select(r => new DiscountCodeDataTemporaryData
                    {
                        DiscountOfferId = r.DiscountOfferId,
                        Barcode = r.Barcode,
                        DiscountCode = r.DiscountCode
                    }).ToList().AsReadOnly();
                }

            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "Discount::GetDiscountCodesForOffer failed.");
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            return discountCodeData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Caller is responsible for disposing returned object</remarks>
        /// <param name="offerId"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for disposing returned object")]
        private DataTable GetMMLineGroups(string offerId)
        {
            DataTable mmLineGroups = new DataTable();

            var lineGroups = this.discountDataManager.GetMMLineGroups(offerId);

            mmLineGroups.Columns.Add("LINEGROUP", typeof(string));
            mmLineGroups.Columns.Add("NOOFITEMSNEEDED", typeof(Int32));
            mmLineGroups.Columns.Add("ITEMSTRIGGERED", typeof(Int32));

            foreach (var lg in lineGroups)
            {
                DataRow row = mmLineGroups.NewRow();
                row["LINEGROUP"] = lg.MixAndMatchLineGroup;
                row["NOOFITEMSNEEDED"] = lg.NumberOfItemsNeeded;
                row["ITEMSTRIGGERED"] = lg.NumberOfItemsNeeded;
                mmLineGroups.Rows.Add(row);
            }

            return mmLineGroups;
        }

        /// <summary>
        /// Calculate Discount Offers
        /// </summary>
        /// <param name="offerId"></param>
        /// <param name="offerName"></param>
        /// <param name="retailTransaction"></param>
        /// <param name="discType"></param>
        /// <returns></returns>
        private RetailTransaction CalcDiscountOffer(string offerId, string offerName, string discountCode, RetailTransaction retailTransaction, ConcurrencyMode concurrencyMode)
        {
            //Loop through all the lines in a specific offer
            foreach (DataRow row in activeOfferLines.Select("OFFERID='" + offerId + "'", "OFFERID ASC, DEALPRICEORDISCPCT DESC"))
            {
                SaleLineItem saleItem = retailTransaction.CalculableSalesLines.Single(cl => cl.LineId == (int)row["SALELINEID"]);

                bool continueWithDiscount = false;
                string offerUOM = Utility.ToString(row["UNIT"]);

                if (!string.IsNullOrEmpty(offerUOM) && !string.Equals(offerUOM, saleItem.SalesOrderUnitOfMeasure, StringComparison.OrdinalIgnoreCase))
                {
                    // If the UOM is specified (non-empty) and the line item doesn't match, then skip this discount
                    continue;
                }

                // If items haven't already been nabbed by other discounts
                decimal leftToDiscount = (Math.Abs(saleItem.Quantity) - Math.Abs(saleItem.QuantityDiscounted));
                if (leftToDiscount > 0M)
                {
                    if (Math.Abs(saleItem.QuantityDiscounted) > 0M)
                    {
                        // Split off a new row for the partial quantity that is not covered by the previous discount.
                        DataRow newRow = SplitLine(ref retailTransaction, saleItem.LineId, leftToDiscount);

                        // Continue with the discount using the newly split row.
                        saleItem = retailTransaction.CalculableSalesLines.Single(cl => cl.LineId == retailTransaction.SaleItems.Count);
                    }
                    continueWithDiscount = true;
                }

                // Apply the Discount Offer
                if (continueWithDiscount && (saleItem.Quantity != 0m))
                {
                    RegisterDiscountOffer(offerId, offerName, discountCode, row, saleItem, concurrencyMode);
                }
            }

            return retailTransaction;
        }

        private void RegisterDiscountOffer(string offerId, string offerName, string discountCode, DataRow row, SaleLineItem saleItem, ConcurrencyMode concurrencyMode)
        {
            PeriodicDiscountItem discountItem = (PeriodicDiscountItem)this.Utility.CreatePeriodicDiscountItem();
            discountItem.LineDiscountType = DiscountTypes.Periodic;
            discountItem.PeriodicDiscountType = PeriodicDiscOfferType.Offer;
            discountItem.OfferId = offerId;
            discountItem.OfferName = offerName;
            discountItem.DiscountCode = discountCode;
            discountItem.ConcurrencyMode = concurrencyMode;
            discountItem.IsCompoundable = false;
            // discount offers should be grouped for consideration by item, allowing application of a single discount line in an offer
            discountItem.PeriodicDiscGroupId = saleItem.ItemId;

            decimal offerPrice;
            DiscountMethod discountMethod = (DiscountMethod)Utility.ToInt(row["DISCOUNTMETHOD"]);
            switch (discountMethod)
            {
                case DiscountMethod.DiscountAmount: // amount
                    discountItem.Percentage = 0m;
                    discountItem.Amount = Utility.ToDecimal(row["DISCAMOUNT"]);
                    discountItem.IsCompoundable = true;
                    break;

                case DiscountMethod.OfferPrice: // price
                    offerPrice = Utility.ToDecimal(row["OFFERPRICE"]);
                    offerPrice = AmountInStoreCurrency(offerPrice);

                    discountItem.Percentage = 0m;
                    discountItem.Amount = saleItem.Price - offerPrice;
                    discountItem.Amount = discountItem.Amount < 0 ? 0m : discountItem.Amount;
                    break;

                case DiscountMethod.OfferPriceInclTax: // price w/ tax
                    offerPrice = Utility.ToDecimal(row["OFFERPRICEINCLTAX"]);
                    offerPrice = AmountInStoreCurrency(offerPrice);

                    discountItem.Percentage = 0m;
                    discountItem.Amount = saleItem.Price - offerPrice;
                    discountItem.Amount = discountItem.Amount < 0 ? 0m : discountItem.Amount;
                    break;

                case DiscountMethod.DiscountPercent: // percent
                default:
                    // percentage should not be more than 100%
                    discountItem.Percentage = Math.Min(100, Utility.ToDecimal(row["DISCPCT"]));
                    discountItem.Amount = 0m;
                    discountItem.IsCompoundable = true;
                    break;
            }

            saleItem.WasChanged = true;

            UpdatePeriodicDiscountLines(saleItem, discountItem);
        }

        /// <summary>
        /// Sums up the quantity for the item in different lines in a certain multibuy offer. 
        /// </summary>
        /// <param name="offerId">The id of the offer.</param>
        /// <param name="productId">The id of the product to find quantity for.</param>
        /// <param name="distinctProductVariantId">The variant id to find quantity for.</param>
        /// <param name="retailTransaction">The retail transaction</param>
        /// <param name="productCategory">The product category to find quantity for.</param>
        /// <returns>The total quantity of the item in the transaction.</returns>
        private decimal MultibuyLineQty(string offerId, Int64 productId, Int64 distinctProductVariantId, RetailTransaction retailTransaction, Int64 productCategory)
        {
            Decimal result = decimal.Zero;

            //Loop through all the lines in a specific offer to find the totals for that item.
            foreach (DataRow row in activeOfferLines.Select("OFFERID='" + offerId + "'", "OFFERID ASC, LINEID ASC"))
            {
                SaleLineItem saleItem = retailTransaction.CalculableSalesLines.Single(cl => cl.LineId == (int)row["SALELINEID"]);

                // If the Offer's UOM is specified (non-empty) then only count lines with a matching UOM.
                string offerUOM = Utility.ToString(row["UNIT"]);
                Int64 offerProductCategory = (Int64)row["PRODUCTCATEGORY"];

                if (!saleItem.NoDiscountAllowed
                    && !saleItem.Voided
                    && ((productCategory != 0 && productCategory == offerProductCategory) || saleItem.ProductId == productId)
                    && ((distinctProductVariantId == 0) || (saleItem.Dimension.DistinctProductVariantId == distinctProductVariantId))
                    && ((offerUOM.Length == 0) || String.Equals(offerUOM, saleItem.SalesOrderUnitOfMeasure, StringComparison.OrdinalIgnoreCase)))
                {
                    //If the item doesn't have a periodic discount or has been added to the same offer we are looking at
                    //add the total quantity of the item to the result
                    if ((saleItem.PeriodicDiscountPossibilities.Count == 0) || (saleItem.PeriodicDiscountPossibilities.Select(p => p.OfferId).Contains(offerId)))
                    {
                        result += saleItem.Quantity;
                    }
                    //If the item is in another offer then get the possible number of items that have not been discounted yet
                    else if ((saleItem.PeriodicDiscountPossibilities.Count != 0) && (!saleItem.PeriodicDiscountPossibilities.Select(p => p.OfferId).Contains(offerId)))
                    {
                        result += (saleItem.Quantity - saleItem.QuantityDiscounted);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Calculate the periodic multibuy discount.
        /// </summary>
        /// <param name="offerId">The id of the offer</param>
        /// <param name="discountCode">The discount code.</param>
        /// <param name="retailTransaction">The retail transaction</param>
        /// <param name="discType"></param>
        /// <returns>The retail transaction</returns>
        private RetailTransaction CalcMultiBuy(string offerId, string offerName, string discountCode, RetailTransaction retailTransaction, DiscountMethodType discType, ConcurrencyMode concurrencyMode)
        {
            //Loop through all the lines in a specific offer to calculate the discount
            foreach (DataRow row in activeOfferLines.Select("OFFERID='" + offerId + "'", "OFFERID ASC, LINEID ASC"))
            {
                SaleLineItem saleItem = retailTransaction.CalculableSalesLines.Where(cl => cl.LineId == (int)row["SALELINEID"]).First();
                Int64 distinctProductVariantId = (Int64)row["DISTINCTPRODUCTVARIANTID"];
                Int64 productCategory = (Int64)row["PRODUCTCATEGORY"];
                if (distinctProductVariantId == 0 || distinctProductVariantId == saleItem.Dimension.DistinctProductVariantId)
                {
                    // continue to next offer line if the unit of measure doesn't match
                    string offerUOM = Utility.ToString(row["UNIT"]);
                    if (!string.IsNullOrEmpty(offerUOM) && !string.Equals(offerUOM, saleItem.SalesOrderUnitOfMeasure, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }

                    // get total number of times item is in the transaction
                    decimal totQtyForMultiBuyLine = MultibuyLineQty(offerId, saleItem.ProductId, distinctProductVariantId, retailTransaction, productCategory);

                    // find multibuy line whose required items is closest to totQtyForMultiBuyLine without going over
                    MultibuyLine multiBuyLine = MultibuyLine.FetchForOfferAndQuantity(offerId, Math.Abs(totQtyForMultiBuyLine));

                    // continue to next offer if no valid multibuy line was found
                    if (multiBuyLine.MinQuantity <= 0m)
                    {
                        continue;
                    }

                    decimal leftToDiscount = (Math.Abs(saleItem.Quantity) - Math.Abs(saleItem.QuantityDiscounted));
                    if (leftToDiscount > 0m)
                    {
                        if (Math.Abs(saleItem.QuantityDiscounted) > 0M)
                        {
                            // If some of the quantity is already 'claimed' split off a new sales line with the unclaimed items and use it
                            DataRow newRow = SplitLine(ref retailTransaction, saleItem.LineId, leftToDiscount);
                            saleItem = retailTransaction.CalculableSalesLines.Single(cl => cl.LineId == retailTransaction.SaleItems.Count);
                        }
                        multiBuyLine = RegisterMultibuyDiscount(offerId, offerName, discountCode, discType, saleItem, multiBuyLine, concurrencyMode);
                    }
                }
            }

            return retailTransaction;
        }

        private MultibuyLine RegisterMultibuyDiscount(string offerId, string offerName, string discountCode, DiscountMethodType discType, SaleLineItem saleItem, MultibuyLine multiBuyLine, ConcurrencyMode concurrencyMode)
        {
            if (saleItem.Price != 0m && saleItem.Quantity != 0m)
            {
                saleItem.WasChanged = true;

                PeriodicDiscountItem discountItem = (PeriodicDiscountItem)this.Utility.CreatePeriodicDiscountItem();
                discountItem.LineDiscountType = DiscountTypes.Periodic;
                discountItem.PeriodicDiscountType = PeriodicDiscOfferType.Multibuy;
                discountItem.OfferId = offerId;
                discountItem.OfferName = offerName;
                discountItem.DiscountCode = discountCode;
                discountItem.ConcurrencyMode = concurrencyMode;
                discountItem.IsCompoundable = false;
                // quantity discounts should be grouped for consideration by item for whole transaction.
                //  if we only apply it to some of the items, can't be sure we stay above the quantity activation threshold
                discountItem.PeriodicDiscGroupId = saleItem.ItemId;

                if ((discType == DiscountMethodType.DealPrice) || (discType == DiscountMethodType.MultiplyDealPrice))
                {
                    discountItem.Percentage = 0m;
                    discountItem.Amount = saleItem.Price - AmountInStoreCurrency(multiBuyLine.UnitPriceOrDiscPct);
                }
                else // discount percent
                {
                    discountItem.Percentage = multiBuyLine.UnitPriceOrDiscPct;
                    discountItem.Amount = 0m;
                    discountItem.IsCompoundable = true;
                }

                UpdatePeriodicDiscountLines(saleItem, discountItem);
            }
            else
            {
                NetTracer.Information("Discount::RegisterMultibuyDiscount: saleItem.Price and/or saleItem.Quantity is zero for offerId {0} offerName {1} discountCode {2}", offerId, offerName, discountCode);
            }
            return multiBuyLine;
        }

        private static DataTable CompressActiveOfferLines(DataTable offerLines)
        {
            bool negQtyExists = false;
            bool posQtyExists = false;

            foreach (DataRow row in offerLines.Select())
            {
                if ((decimal)row["QUANTITY"] > 0m)
                {
                    posQtyExists = true;
                }

                if ((decimal)row["QUANTITY"] < 0m)
                {
                    negQtyExists = true;
                }
            }

            // if transaction has mix and match lines which are negative and positive
            if (posQtyExists && negQtyExists)
            {
                // get lines in mix and match offer, and sort in order of transaction
                foreach (DataRow row in offerLines.Select(string.Empty, "SALELINEID ASC"))
                {
                    // for each offer line, get all offer lines with the same product/variant, sorted reverse order of transaction
                    foreach (DataRow row2 in offerLines.Select(String.Format("PRODUCTID={0} AND DISTINCTPRODUCTVARIANTID={1}",
                        (Int64)row["PRODUCTID"], (Int64)row["DISTINCTPRODUCTVARIANTID"]), "SALELINEID DESC"))
                    {
                        // only do anything if inner line is below outer line in transaction order
                        if ((int)row2["SALELINEID"] > (int)row["SALELINEID"])
                        {
                            // if outer line is positive and inner line is negative
                            if ((decimal)row["QUANTITY"] > 0m && (decimal)row2["QUANTITY"] < 0m)
                            {
                                // if the sum of inner and outer line is positive or zero, move all quantity to the outer line
                                if ((decimal)row["QUANTITY"] + (decimal)row2["QUANTITY"] >= 0m)
                                {
                                    row["QUANTITY"] = (decimal)row["QUANTITY"] + (decimal)row2["QUANTITY"];
                                    row2["QUANTITY"] = 0m;
                                }
                                // if sum of inner and outer line is negative, move all quantity to inner line
                                else
                                {
                                    row2["QUANTITY"] = (decimal)row["QUANTITY"] + (decimal)row2["QUANTITY"];
                                    row["QUANTITY"] = 0m;
                                }
                            }
                            // if outer line is negative and inner line is positive
                            else if ((decimal)row["QUANTITY"] < 0m && (decimal)row2["QUANTITY"] > 0m)
                            {
                                // if sum of inner and outer lines are negative or zero, move all quantity to outer line
                                if ((decimal)row["QUANTITY"] + (decimal)row2["QUANTITY"] <= 0m)
                                {
                                    row["QUANTITY"] = (decimal)row["QUANTITY"] + (decimal)row2["QUANTITY"];
                                    row2["QUANTITY"] = 0m;
                                }
                                // if sume of inner and outer lines are positive, move all quantity to inner line
                                else
                                {
                                    row2["QUANTITY"] = (decimal)row["QUANTITY"] + (decimal)row2["QUANTITY"];
                                    row["QUANTITY"] = 0m;
                                }
                            }
                        }
                        // go to next outer line if there are no more matching inner lines below it
                        else
                        {
                            break;
                        }
                    }
                }
            }

            return offerLines;
        }

        private DataTable GetMixMatchOfferLines(string offerId)
        {
            //Get a copy of active offerLines for this offer ordered by priority
            using (DataView tmpOffer = new DataView(activeOfferLines))
            {
                tmpOffer.RowFilter = "OFFERID='" + offerId + "'";
                tmpOffer.Sort = ("MIXMATCHPRIORITY ASC");

                //Create a new datatable with the Mix & Match information sorted by the Mix & Match priority
                DataTable offerLines = tmpOffer.ToTable();
                //Set the primary key as M&M priority + Sale line id + PRODUCT ID + VARIANT ID
                DataColumn[] pk = new DataColumn[5];
                pk[0] = offerLines.Columns["MIXMATCHPRIORITY"];
                pk[1] = offerLines.Columns["SALELINEID"];
                pk[2] = offerLines.Columns["PRODUCTID"];
                pk[3] = offerLines.Columns["DISTINCTPRODUCTVARIANTID"];
                pk[4] = offerLines.Columns["UNIT"];
                offerLines.PrimaryKey = pk;

                //Must compress check because of minus quantities
                offerLines = CompressActiveOfferLines(offerLines);
                return offerLines;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfathered")]
        private RetailTransaction CalcMixMatch(string offerId, string offerName, string discountCode, DataRow activeOffer, RetailTransaction retailTransaction, ConcurrencyMode concurrencyMode)
        {
            DataTable offerLines = GetMixMatchOfferLines(offerId);

            DataTable mmLineGroups = GetMMLineGroups(offerId);
            if (mmLineGroups.Rows.Count == 0)
            {
                NetTracer.Information("Discount::CalcMixMatch: mmLineGroups.Rows.Count is zero for offerId {0} offerName {1} discountCode {2}", offerId, offerName, discountCode);
                return retailTransaction;
            }

            ResetMixAndMatchCalculation(mmLineGroups);
            UInt32 numberOfTimesOfferHasBeenApplied = 0;
            SortedSet<int> leastExpensiveLineIdSet = new SortedSet<int>();
            try
            {
                do
                {
                    DiscountMethodType DiscountType = (DiscountMethodType)activeOffer["DISCOUNTTYPE"];

                    // Split the lines based on tmpMMOffer rows ITEMSTRIGGERED value.
                    // This is to make sure that triggered items should not have aggregated quantity before registering discounts
                    foreach (DataRow row in this.tmpMMOffer.Select())
                    {
                        ISaleLineItem saleItem = retailTransaction.CalculableSalesLines.Single(cl => cl.LineId == (int)row["SALELINEID"]);

                        if ((Math.Abs(saleItem.Quantity) - Math.Abs((int)row["ITEMSTRIGGERED"])) > decimal.Zero)
                        {
                            if (Math.Abs((int)row["ITEMSTRIGGERED"]) > decimal.Zero)
                            {
                                // split the line, and continue with discount using the new line
                                SplitLine(ref retailTransaction, saleItem.LineId, (Math.Sign(saleItem.Quantity) * (Math.Abs(saleItem.Quantity) - (int)row["ITEMSTRIGGERED"])));
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //split product for least expensive
                    if (DiscountType == DiscountMethodType.LeastExpensive)
                    {
                        leastExpensiveLineIdSet.Clear();
                        int noOfLeastExpensiveItem = (int)activeOffer["NOOFLEASTEXPENSIVEITEMS"];
                        decimal leastExpensivePrice = GetMaxLeastExpensiveAmount(noOfLeastExpensiveItem, retailTransaction);
                        int qtyCount = 0;
                        int splitCount = 0;
                        foreach (DataRow offerline in this.tmpMMOffer.Select("PRICE <= " + leastExpensivePrice.ToString(System.Globalization.CultureInfo.InvariantCulture), "PRICE ASC, SALELINEID DESC"))
                        {
                            ISaleLineItem saleItem = retailTransaction.CalculableSalesLines.Single(cl => cl.LineId == (int)offerline["SALELINEID"]);

                            qtyCount += Convert.ToInt32(Math.Abs(saleItem.Quantity));
                            if (qtyCount > noOfLeastExpensiveItem)
                            {
                                DataRow discountedRow = SplitLineForLeastExpensive(ref retailTransaction, saleItem.LineId, Math.Sign(saleItem.Quantity) * (noOfLeastExpensiveItem - splitCount));
                                leastExpensiveLineIdSet.Add((int)discountedRow["SALELINEID"]);
                                break;
                            }

                            leastExpensiveLineIdSet.Add(saleItem.LineId);
                            if (qtyCount == noOfLeastExpensiveItem)
                            {
                                break;
                            }

                            splitCount += Convert.ToInt32(Math.Abs(saleItem.Quantity));
                        }
                    }

                    //If the criteria for the offer has been fulfilled then update the items.
                    if (AllGroupsTriggered(mmLineGroups))
                    {
                        foreach (DataRow tmpMMRow in tmpMMOffer.Select())
                        {
                            ISaleLineItem saleItem = retailTransaction.CalculableSalesLines.Single(cl => cl.LineId == (int)tmpMMRow["SALELINEID"]);

                            //When setting QuantityDiscounted, take the quantity sign into account so that the discount is correctly calculated for Return lines.
                            saleItem.QuantityDiscounted += (int)tmpMMRow["ITEMSTRIGGERED"] * Math.Sign(saleItem.Quantity);
                        }

                        //Calculate discount and update all saleitems
                        RegisterMixMatch(offerId, offerName, discountCode, activeOffer, tmpMMOffer, retailTransaction, concurrencyMode, numberOfTimesOfferHasBeenApplied, leastExpensiveLineIdSet);
                        numberOfTimesOfferHasBeenApplied += 1;

                        ResetMixAndMatchCalculation(mmLineGroups);
                    }

                    foreach (DataRow row in offerLines.Select())
                    {
                        ISaleLineItem saleItem = retailTransaction.CalculableSalesLines.Single(cl => cl.LineId == (int)row["SALELINEID"]);

                        if ((Math.Abs(saleItem.Quantity) - Math.Abs(saleItem.QuantityDiscounted)) > decimal.Zero)
                        {
                            if (Math.Abs(saleItem.QuantityDiscounted) > decimal.Zero)
                            {
                                // split the line, and continue with discount using the new line
                                SplitLine(ref retailTransaction, saleItem.LineId, (saleItem.Quantity - saleItem.QuantityDiscounted));
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }

                    //Get all offerlines for the offer in question
                    offerLines = GetMixMatchOfferLines(offerId);

                    int totQuantityDiscounted = 0;

                    foreach (DataRow row in offerLines.Select(string.Empty, "MIXMATCHPRIORITY DESC"))
                    {
                        //Debug
                        decimal priority = (decimal)row["MIXMATCHPRIORITY"];

                        if (AllGroupsTriggered(mmLineGroups)) { break; }

                        ISaleLineItem saleItem = retailTransaction.CalculableSalesLines.Single(cl => cl.LineId == (int)row["SALELINEID"]);
                        DataRow discountedRow = row;
                        string offerUOM = Utility.ToString(row["UNIT"]);

                        if (!string.IsNullOrEmpty(offerUOM) && !string.Equals(offerUOM, saleItem.SalesOrderUnitOfMeasure, StringComparison.OrdinalIgnoreCase))
                        {
                            continue;
                        }

                        // calculate the discount using the selected row & line item
                        if (saleItem.Quantity != decimal.Zero)
                        {
                            decimal discountedRowQuantity = (decimal)discountedRow["QUANTITY"];
                            decimal leftToDiscount = Math.Abs(discountedRowQuantity) - Math.Abs(saleItem.QuantityDiscounted);
                            totQuantityDiscounted += Convert.ToInt32(leftToDiscount);

                            // Mix and Match discount only currently works for integer quantities.  Therefore
                            // leftToDiscount must be at least 1
                            if ((leftToDiscount >= 1.0M) && (saleItem.NoDiscountAllowed == false))
                            {
                                FindTriggeredMixAndMatchItems(mmLineGroups, totQuantityDiscounted, saleItem, discountedRow, leftToDiscount);
                            }
                        }
                    }
                } while (AllGroupsTriggered(mmLineGroups));
            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "Discount::CalcMixMatch: failed while matching mix and match lines for offerId {0} offerName {1} discountCode {2}", offerId, offerName, discountCode);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }

            return retailTransaction;
        }

        private void FindTriggeredMixAndMatchItems(DataTable mmLineGroups, int totQuantityDiscounted, ISaleLineItem saleItem, DataRow discountedRow, decimal leftToDiscount)
        {
            //Go through each instance of the sale item and check if it can be in the promotion.
            string lineGroup = (string)discountedRow["LINEGROUP"];
            for (int i = 0; i < leftToDiscount; i++)
            {
                foreach (DataRow mmRow in mmLineGroups.Select("LINEGROUP='" + lineGroup + "'"))
                {
                    int noOfItemsNeeded = (int)mmRow["NOOFITEMSNEEDED"];
                    int itemsTriggered = (int)mmRow["ITEMSTRIGGERED"];

                    if ((totQuantityDiscounted >= itemsTriggered) && (noOfItemsNeeded > itemsTriggered))
                    {
                        bool found = false;

                        foreach (DataRow mmOldRow in tmpMMOffer.Select("SALELINEID='" + saleItem.LineId.ToString() + "'"))
                        {
                            mmOldRow["ITEMSTRIGGERED"] = (int)mmOldRow["ITEMSTRIGGERED"] + 1;
                            found = true;
                        }

                        if (!found)
                        {
                            DataRow tmpMMRow;
                            tmpMMRow = tmpMMOffer.NewRow();
                            tmpMMRow["SALELINEID"] = saleItem.LineId;
                            tmpMMRow["ITEMSTRIGGERED"] = 1;
                            tmpMMRow["DISCTYPE"] = discountedRow["DISCTYPE"];
                            tmpMMRow["DEALPRICEORDISCPCT"] = ((int)discountedRow["DISCTYPE"] == (int)DiscountMethodType.DealPrice)
                                                                ? (decimal)discountedRow["DEALPRICEORDISCPCT"] / noOfItemsNeeded : discountedRow["DEALPRICEORDISCPCT"];
                            tmpMMRow["PRICE"] = saleItem.Price;
                            tmpMMOffer.Rows.Add(tmpMMRow);
                        }

                        mmRow["ITEMSTRIGGERED"] = itemsTriggered + 1;
                    }
                }
            }
        }

        private void ResetMixAndMatchCalculation(DataTable mmLineGroups)
        {
            //Initialize
            tmpMMOffer.Clear();

            //Set items triggerd to zero for all line groups
            foreach (DataRow row in mmLineGroups.Select())
            {
                row["ITEMSTRIGGERED"] = 0;
            }
        }

        /// <summary>
        /// Adjusts multi line discounts to make the amount come out to an exact target discount to account
        /// for rounding
        /// </summary>
        /// <param name="tmpMMOffer"></param>
        /// <param name="retailTransaction"></param>
        /// <param name="targetDiscountAmt"></param>
        /// <param name="application"></param>
        private static void AdjustMultiLineDiscount(DataTable tmpMMOffer, RetailTransaction retailTransaction, decimal targetDiscountAmt, IApplication application)
        {
            SaleLineItem highestDiscountLine = null;
            decimal actualDiscount = decimal.Zero;
            decimal highestDiscountAmount = decimal.Zero;

            // Sum up all of the rounded periodic mix and match discounts to find the actual discount
            // Also find the row with the highest discount for the one to adjust.
            foreach (DataRow row in tmpMMOffer.Rows)
            {
                SaleLineItem saleItem = retailTransaction.GetItem((int)row["SALELINEID"]);
                DiscountItem discountItem = saleItem.GetPossiblePeriodicDiscountItem(PeriodicDiscOfferType.MixAndMatch, DiscountTypes.Periodic);
                decimal extendedDiscountAmount = application.Services.Rounding.Round(discountItem.Amount * Math.Abs(saleItem.Quantity));
                if ((Math.Abs(extendedDiscountAmount) > highestDiscountAmount) || (highestDiscountLine == null))
                {
                    highestDiscountAmount = Math.Abs(extendedDiscountAmount);
                    highestDiscountLine = saleItem;
                }

                actualDiscount += extendedDiscountAmount;
            }

            // Adjust the line to make the discount come out exact.
            if ((targetDiscountAmt != actualDiscount) && (highestDiscountLine != null))
            {
                DiscountItem discountItem = highestDiscountLine.GetPossiblePeriodicDiscountItem(PeriodicDiscOfferType.MixAndMatch, DiscountTypes.Periodic);
                discountItem.Amount += (targetDiscountAmt - actualDiscount) / Math.Abs(highestDiscountLine.Quantity);
            }
        }

        private void RegisterMixMatch(string offerId, string offerName, string discountCode, DataRow activeOffer, DataTable tmpMixAndMatchOffer, RetailTransaction retailTransaction, ConcurrencyMode concurrencyMode, UInt32 groupNumber, SortedSet<int> leastExpensiveLineIdSet)
        {
            DiscountMethodType DiscountType = (DiscountMethodType)activeOffer["DISCOUNTTYPE"];
            decimal dealPrice = (decimal)activeOffer["DEALPRICEVALUE"];
            decimal discountAmount = (decimal)activeOffer["DISCOUNTAMOUNTVALUE"];
            string discountCurrency = string.IsNullOrEmpty((string)activeOffer["CURRENCYCODE"]) ? ApplicationSettings.Terminal.CompanyCurrency : (string)activeOffer["CURRENCYCODE"];
            decimal totalAmount = decimal.Zero;
            bool isCompoundable = false;

            dealPrice = AmountInStoreCurrency(discountCurrency, dealPrice);
            discountAmount = AmountInStoreCurrency(discountCurrency, discountAmount);

            int noOfLeastExpensiveItem = (int)activeOffer["NOOFLEASTEXPENSIVEITEMS"];
            try
            {
                foreach (DataRow row in tmpMixAndMatchOffer.Select())
                {
                    ISaleLineItem saleItem = retailTransaction.GetItem((int)row["SALELINEID"]);

                    totalAmount += saleItem.Price * (int)row["ITEMSTRIGGERED"];
                }

                foreach (DataRow row in tmpMixAndMatchOffer.Select(string.Empty, "PRICE ASC"))
                {
                    SaleLineItem saleItem = retailTransaction.GetItem((int)row["SALELINEID"]);
                    saleItem.WasChanged = true;

                    decimal percentage = 0m;
                    decimal amount = 0m;

                    // choose correct price field based on tax scheme
                    decimal saleprice = saleItem.Price;

                    if (saleItem.Quantity != 0m && totalAmount != 0m)
                    {
                        switch (DiscountType)
                        {
                            case DiscountMethodType.DealPrice:
                                {
                                    percentage = 0m;
                                    amount = this.Application.Services.Rounding.Round((totalAmount - dealPrice) * Math.Abs((saleprice * saleItem.QuantityDiscounted / totalAmount)));
                                    amount = amount / Math.Abs(saleItem.Quantity); //Discount amount per pcs.
                                }
                                break;
                            case DiscountMethodType.DiscountPercent:
                                {
                                    percentage = (decimal)activeOffer["DISCOUNTPCTVALUE"] * Math.Abs((saleItem.QuantityDiscounted / (saleItem.Quantity)));
                                    amount = 0m;
                                    isCompoundable = true;
                                }
                                break;
                            case DiscountMethodType.DiscountAmount:
                                {
                                    percentage = 0m;
                                    amount = discountAmount * Math.Abs((saleprice * saleItem.QuantityDiscounted / totalAmount));
                                    amount = amount / Math.Abs(saleItem.Quantity); //Discount amount per pcs.
                                    isCompoundable = true;
                                }
                                break;
                            case DiscountMethodType.LeastExpensive:
                                {
                                    if (leastExpensiveLineIdSet.Contains(saleItem.LineId))
                                    {
                                        percentage = 100m;
                                        amount = saleprice;
                                    }
                                    else
                                    {
                                        percentage = 0m;
                                        amount = 0m;
                                    }
                                }
                                break;
                            case DiscountMethodType.LineSpecific:
                                {
                                    if ((int)row["DISCTYPE"] == (int)DiscountMethodType.DealPrice)
                                    {
                                        percentage = 0m;
                                        decimal lineSpecDealPrice = (decimal)row["DEALPRICEORDISCPCT"];
                                        lineSpecDealPrice = AmountInStoreCurrency(discountCurrency, lineSpecDealPrice);

                                        amount = ((saleprice - lineSpecDealPrice) * Math.Abs(saleItem.QuantityDiscounted));
                                        amount = amount / Math.Abs(saleItem.Quantity); //Discount amount per pcs.
                                    }
                                    else // discount percent
                                    {
                                        percentage = (decimal)row["DEALPRICEORDISCPCT"] * Math.Abs((saleItem.QuantityDiscounted / (saleItem.Quantity)));
                                        amount = 0;
                                        isCompoundable = true;
                                    }
                                }
                                break;
                        }

                        PeriodicDiscountItem discountItem = (PeriodicDiscountItem)this.Utility.CreatePeriodicDiscountItem();
                        discountItem.LineDiscountType = DiscountTypes.Periodic;
                        discountItem.PeriodicDiscountType = PeriodicDiscOfferType.MixAndMatch;
                        discountItem.Percentage = percentage;
                        discountItem.Amount = amount;
                        discountItem.OfferId = offerId;
                        discountItem.OfferName = offerName;
                        discountItem.DiscountCode = discountCode;
                        discountItem.ConcurrencyMode = concurrencyMode;
                        discountItem.IsCompoundable = isCompoundable;
                        // group Id represents this unique mix and match set which is being registered
                        discountItem.PeriodicDiscGroupId = discountItem.OfferId + "_" + groupNumber.ToString();

                        UpdatePeriodicDiscountLines(saleItem, discountItem);
                    }
                }

                // Adjust the multi-line discounts to make the amounts come out exact.
                switch (DiscountType)
                {
                    case DiscountMethodType.DealPrice:
                        AdjustMultiLineDiscount(
                            tmpMixAndMatchOffer,
                            retailTransaction,
                            (totalAmount - dealPrice),
                            this.Application);
                        break;

                    /*case DiscountMethodType.LeastExpensive:
                        AdjustMultiLineDiscount(
                            tmpMixAndMatchOffer,
                            retailTransaction,
                            GetLeastExpensiveAmount((int)activeOffer["NOOFLEASTEXPENSIVEITEMS"], retailTransaction),
                            this.Application);
                        break;*/
                }
            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "Discount::RegisterMixMatch failed for offerId {0} offerName {1} discountCode {2}", offerId, offerName, discountCode);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }


        /// <summary>
        /// If the company currency differs from the store, convert the given
        ///   amount from the company currency to the store currency
        /// </summary>
        /// <param name="amount">Amount in company currency</param>
        /// <returns>Amount in store currency</returns>
        private decimal AmountInStoreCurrency(decimal amount)
        {
            if (ApplicationSettings.Terminal.CompanyCurrency != ApplicationSettings.Terminal.StoreCurrency)
            {
                return this.Application.Services.Currency.CurrencyToCurrency(
                    ApplicationSettings.Terminal.CompanyCurrency,
                    ApplicationSettings.Terminal.StoreCurrency,
                    amount);
            }
            else
            {
                return amount;
            }
        }

        /// <summary>
        /// If the discount currency differs from the store, convert the given
        /// amount from the discount currency to the store currency
        /// </summary>
        /// <param name="discountCurrency">The discount currency</param>
        /// <param name="amount">Amount in discount currency</param>
        /// <returns>Amount in store currency</returns>
        private decimal AmountInStoreCurrency(string discountCurrency, decimal amount)
        {
            if (discountCurrency != ApplicationSettings.Terminal.StoreCurrency)
            {
                return this.Application.Services.Currency.CurrencyToCurrency(
                    discountCurrency,
                    ApplicationSettings.Terminal.StoreCurrency,
                    amount);
            }
            else
            {
                return amount;
            }
        }

        /// <summary>
        /// Returns the max of the least discount amount for the least exepnsive items.
        /// For example(Buy 2 get 2 Free): one transatcion like below
        /// Product1    $10     1pcs
        /// Product2    $5      1pcs
        /// Product3    $7      1pcs
        /// Product4    $9      1pcs
        /// For above scenario the Max least discount amount should be 7
        /// <param name="retailTransaction">The retailtransaction</param>
        /// <param name="noLeastExpensiveItems">The number of least expensive items that are free</param>
        /// <returns></returns>
        private decimal GetMaxLeastExpensiveAmount(int noLeastExpensiveItems, RetailTransaction retailTransaction)
        {
            decimal discountAmount = 0m;
            decimal salePrice = 0m;
            int items = 0;
            decimal[] leastExpensive = new decimal[noLeastExpensiveItems];
            int index = 0;
            foreach (DataRow row in this.tmpMMOffer.Select(string.Empty, "PRICE ASC"))
            {
                ISaleLineItem saleItem = retailTransaction.GetItem((int)row["SALELINEID"]);
                salePrice = saleItem.Price;

                if (Math.Abs(saleItem.Quantity) + items <= noLeastExpensiveItems)
                {
                    items += Math.Abs((int)saleItem.Quantity);
                }
                else
                {
                    items = noLeastExpensiveItems;
                }
                leastExpensive[index] = salePrice;
                index++;

                if (items == noLeastExpensiveItems)
                {
                    break;
                }
            }

            discountAmount = leastExpensive.Max();

            return discountAmount;
        }

        private static bool AllGroupsTriggered(DataTable mmLineGroups)
        {
            foreach (DataRow row in mmLineGroups.Select())
            {
                if ((int)row["NOOFITEMSNEEDED"] > (int)row["ITEMSTRIGGERED"])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Used to split a item line if needed in a periodicdiscount.  A splitting of a line is needed if part of the 
        /// quantity has been used in another (mix&match)offer.
        /// </summary>
        /// <param name="retailTransaction">The retail transaction.</param>
        /// <param name="lineId">The id of the line that will be splited.</param>
        /// <param name="qtyNewLine">The quantity that will be taken taken from one line and put into a new line</param>
        private DataRow SplitLine(ref RetailTransaction retailTransaction, int lineId, decimal qtyNewLine)
        {
            if (qtyNewLine == 0M)
            {
                NetTracer.Warning("qtyNewLine parameter is zero");
                return null;
            }

            bool newLineAdded = AddNewLine(ref retailTransaction, lineId, qtyNewLine);

            DataRow offerLineRow = null;

            //Refresh the offer information after adding a sales line
            if (newLineAdded)
            {
                AddNewActiveOfferLine(retailTransaction, lineId, qtyNewLine, ref offerLineRow);
            }

            return offerLineRow;
        }

        /// <summary>
        /// Used to split a item line if needed in a periodicdiscount.  A splitting of a line is needed if part of the 
        /// quantity has been used in another (mix&match)offer.
        /// </summary>
        /// <param name="retailTransaction">The retail transaction.</param>
        /// <param name="lineId">The id of the line that will be splited.</param>
        /// <param name="qtyNewLine">The quantity that will be taken taken from one line and put into a new line</param>
        private DataRow SplitLineForLeastExpensive(ref RetailTransaction retailTransaction, int lineId, decimal qtyNewLine)
        {
            if (qtyNewLine == 0M)
            {
                NetTracer.Warning("qtyNewLine parameter is zero");
                return null;
            }

            bool newLineAdded = AddNewLine(ref retailTransaction, lineId, qtyNewLine);

            DataRow offerLineRow = null;
            //Refresh the offer information after adding a sales line
            if (newLineAdded)
            {
                AddNewActiveOfferLine(retailTransaction, lineId, qtyNewLine, ref offerLineRow);

                //Doing the same for the tmpMMOffer rows
                foreach (DataRow row in tmpMMOffer.Select("SALELINEID ='" + lineId.ToString() + "'"))
                {
                    if (Convert.ToDecimal(row["ITEMSTRIGGERED"]) > Math.Abs(qtyNewLine))
                    {
                        row["ITEMSTRIGGERED"] = Convert.ToDecimal(row["ITEMSTRIGGERED"]) - Math.Abs(qtyNewLine);
                    }

                    AddNewOffer(retailTransaction, qtyNewLine, row);
                    break;
                }
            }

            return offerLineRow;
        }

        private bool AddNewLine(ref RetailTransaction retailTransaction, int lineId, decimal qtyNewLine)
        {
            if (qtyNewLine == 0M)
            {
                NetTracer.Warning("qtyNewLine parameter is zero");
                return false;
            }

            bool newLineAdded = false;

            //Create a list for item´s to be removed
            LinkedList<SaleLineItem> newSaleLinesList = new LinkedList<SaleLineItem>();

            foreach (SaleLineItem saleItem in retailTransaction.SaleItems.Where(i => i.LineId == lineId))
            {
                //Create the dublicate sale line 
                SaleLineItem newSaleItem1 = new SaleLineItem(retailTransaction.StoreCurrencyCode, this.Application.Services.Rounding, retailTransaction);
                newSaleItem1 = (SaleLineItem)saleItem.CloneLineItem();
                newSaleItem1.Quantity = qtyNewLine;
                newSaleItem1.QuantityDiscounted = 0m;
                newSaleItem1.DiscountLines.Clear();
                newSaleItem1.PeriodicDiscountPossibilities.Clear();

                newSaleLinesList.AddLast(newSaleItem1);
                newLineAdded = true;

                //Update the discount line on the original sale item to reflect the new quantity
                if (Math.Abs(saleItem.QuantityDiscounted) > decimal.Zero)
                {
                    foreach (ILineDiscountItem discLine in saleItem.DiscountLines.Concat(saleItem.PeriodicDiscountPossibilities))
                    {
                        discLine.Percentage = discLine.Percentage * (saleItem.Quantity) / Math.Abs(saleItem.QuantityDiscounted);
                        discLine.Amount = discLine.Amount * Math.Abs(saleItem.Quantity) / Math.Abs(saleItem.QuantityDiscounted);
                    }
                }

                //Set the new quantity on the orgininal sale line item
                saleItem.Quantity += -qtyNewLine;
            }

            foreach (SaleLineItem item in newSaleLinesList)
            {
                retailTransaction.Add(item);
            }

            return newLineAdded;
        }

        private void AddNewOffer(RetailTransaction retailTransaction, decimal qtyNewLine, DataRow row)
        {
            DataRow tmpMMRow;
            tmpMMRow = this.tmpMMOffer.NewRow();
            tmpMMRow["SALELINEID"] = retailTransaction.SaleItems.Count;
            tmpMMRow["ITEMSTRIGGERED"] = Math.Abs(qtyNewLine);
            tmpMMRow["DISCTYPE"] = row["DISCTYPE"];
            tmpMMRow["DEALPRICEORDISCPCT"] = row["DEALPRICEORDISCPCT"];
            tmpMMRow["PRICE"] = row["PRICE"];
            this.tmpMMOffer.Rows.Add(tmpMMRow);
        }

        private void AddNewActiveOfferLine(RetailTransaction retailTransaction, int lineId, decimal qtyNewLine, ref DataRow offerLineRow)
        {
            foreach (DataRow row in this.activeOfferLines.Select("SALELINEID ='" + lineId.ToString() + "'"))
            {
                if (Math.Abs(Convert.ToDecimal(row["QUANTITY"])) - Math.Abs(qtyNewLine) > 0m)
                {
                    row["QUANTITY"] = Math.Sign(qtyNewLine) * (Math.Abs(Convert.ToDecimal(row["QUANTITY"])) - Math.Abs(qtyNewLine));
                }
                offerLineRow = this.activeOfferLines.NewRow();
                offerLineRow.ItemArray = row.ItemArray;
                offerLineRow["SALELINEID"] = retailTransaction.SaleItems.Count;
                offerLineRow["QUANTITY"] = qtyNewLine;
                this.activeOfferLines.Rows.Add(offerLineRow);
            }
        }

        /// <summary>
        /// Update the periodic discount items.
        /// </summary>
        /// <param name="saleItem">The item line that the discount line is added to.</param>
        /// <param name="discountItem">The new discount item</param>
        private static void UpdatePeriodicDiscountLines(SaleLineItem saleItem, PeriodicDiscountItem discountItem)
        {
            //Check if line discount is found, if so then update
            bool discountLineFound = false;
            foreach (PeriodicDiscountItem periodicDiscLine in saleItem.PeriodicDiscountPossibilities)
            {
                //If found then update
                if ((periodicDiscLine.LineDiscountType == discountItem.LineDiscountType) &&
                    (periodicDiscLine.PeriodicDiscountType == discountItem.PeriodicDiscountType) &&
                    (periodicDiscLine.OfferId == discountItem.OfferId) &&
                    (periodicDiscLine.PeriodicDiscGroupId == discountItem.PeriodicDiscGroupId))
                {
                    periodicDiscLine.Percentage = discountItem.Percentage;
                    periodicDiscLine.Amount = discountItem.Amount;
                    periodicDiscLine.IsCompoundable = discountItem.IsCompoundable;
                    periodicDiscLine.ConcurrencyMode = discountItem.ConcurrencyMode;
                    periodicDiscLine.PeriodicDiscGroupId = discountItem.PeriodicDiscGroupId;
                    discountLineFound = true;
                }
            }
            //If line discount is not found then add it.
            if (discountLineFound == false)
            {
                saleItem.Add(discountItem);
                String message = String.Format("Adding possible periodic discount '{0}' to item '{1}' line '{2}'.", discountItem.OfferId, saleItem.ItemId, saleItem.LineId);
                LSRetailPosis.ApplicationLog.Log("Discount.UpdatePeriodicDiscountLine()", message, LogTraceLevel.Trace);
            }
        }

        #endregion PeriodicDiscount
    }

    internal class PriceDiscData
    {
        public PriceDiscType Relation { get; private set; }
        public string ItemRelation { get; private set; }
        public string AccountRelation { get; private set; }
        public int ItemCode { get; private set; }
        public int AccountCode { get; private set; }
        public decimal QuantityAmount { get; private set; }
        public string TargetCurrencyCode { get; private set; }
        public Dimensions ItemDimensions { get; private set; }
        public bool IncludeDimensions { get; private set; }
        public ReadOnlyCollection<Discount.DiscountAgreementArgs> SqlResults { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PriceDiscData"/> class.
        /// </summary>
        /// <param name="relation">The relation.</param>
        /// <param name="itemRelation">The item relation.</param>
        /// <param name="accountRelation">The account relation.</param>
        /// <param name="itemCode">The item code.</param>
        /// <param name="accountCode">The account code.</param>
        /// <param name="quantityAmount">The quantity amount.</param>
        /// <param name="targetCurrencyCode">The target currency code.</param>
        /// <param name="itemDimensions">The item dimensions.</param>
        /// <param name="includeDimensions">if set to <c>true</c> [include dimensions].</param>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public PriceDiscData(PriceDiscType relation, string itemRelation, string accountRelation, int itemCode, int accountCode, decimal quantityAmount, string targetCurrencyCode, Dimensions itemDimensions, bool includeDimensions)
            : this(null, relation, itemRelation, accountRelation, itemCode, accountCode, quantityAmount, targetCurrencyCode, itemDimensions, includeDimensions)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="PriceDiscData"/> class.
        /// </summary>
        /// <param name="sqlResults">The SQL results.</param>
        /// <param name="relation">The relation.</param>
        /// <param name="itemRelation">The item relation.</param>
        /// <param name="accountRelation">The account relation.</param>
        /// <param name="itemCode">The item code.</param>
        /// <param name="accountCode">The account code.</param>
        /// <param name="quantityAmount">The quantity amount.</param>
        /// <param name="targetCurrencyCode">The target currency code.</param>
        /// <param name="itemDimensions">The item dimensions.</param>
        /// <param name="includeDimensions">if set to <c>true</c> [include dimensions].</param>
        public PriceDiscData(ReadOnlyCollection<Discount.DiscountAgreementArgs> sqlResults, PriceDiscType relation, string itemRelation, string accountRelation, int itemCode, int accountCode, decimal quantityAmount, string targetCurrencyCode, Dimensions itemDimensions, bool includeDimensions)
        {
            this.SqlResults = sqlResults;

            this.Relation = relation;
            this.ItemRelation = itemRelation;

            this.AccountRelation = accountRelation;
            this.ItemCode = itemCode;
            this.AccountCode = accountCode;
            this.QuantityAmount = quantityAmount;
            this.TargetCurrencyCode = targetCurrencyCode;
            this.ItemDimensions = itemDimensions;
            this.IncludeDimensions = includeDimensions;
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.InvariantCulture, "Rel {0}, IR {1}, AR {2}, IC {3}, AC {4}, QTY {5}, CUR {6}, ID {7}, IID {8}", Relation, ItemRelation, AccountRelation, ItemCode, AccountCode, QuantityAmount, TargetCurrencyCode, ItemDimensions, IncludeDimensions);
        }
    }
}
