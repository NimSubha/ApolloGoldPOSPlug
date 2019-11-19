using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Transaction.Line.SaleItem;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using System.IO;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class FrmMakStnDiaDisc : frmTouchBase
    {
        private IApplication application;
        public IPosTransaction pos { get; set; }
        SaleLineItem _saleLineItem;
        string sBaseItemID = "";
        bool isMRPUCP = false;
        string sCustomerId = string.Empty;
        RetailTransaction retailTrans;
        int iLine = 0;
        public bool bValid = false;
        int isPromo = 0;
        int iNimMakingDiscCalcType = 0;

        int iAllLineMkDisc = 0;
        int iFlatOnMk = 0;

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

        internal static IApplication InternalApplication { get; private set; }

        private enum CRWRetailDiscPermission // added on 29/08/2014
        {
            Cashier = 0,
            Salesperson = 1,
            Manager = 2,
            Other = 3,
            Manager2 = 4,
        }

        enum MakingDiscType
        {
            PerPCS = 0,
            PerGram = 1,
            Percent = 2,
        }

        enum MetalType
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

        public FrmMakStnDiaDisc()
        {
            InitializeComponent();
        }

        public FrmMakStnDiaDisc(IPosTransaction posTransaction, IApplication Application, SaleLineItem saleLineItem, int iPromo = 0)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            _saleLineItem = saleLineItem;
            sBaseItemID = _saleLineItem.ItemId;
            iLine = saleLineItem.LineId;

            RetailTransaction _retailTrans = posTransaction as RetailTransaction;
            retailTrans = _retailTrans;

            isMRPUCP = isMRP(sBaseItemID);
            isPromo = iPromo;

            if (!string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
            {
                sCustomerId = retailTrans.Customer.CustomerId;
            }
        }

        public FrmMakStnDiaDisc(IPosTransaction posTransaction, IApplication Application, int iForAllLineMkDisc = 0, int iFlatDiscOnMk = 0)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            // sBaseItemID = _saleLineItem.ItemId;
            iAllLineMkDisc = iForAllLineMkDisc;
            iFlatOnMk = iFlatDiscOnMk;

            RetailTransaction _retailTrans = posTransaction as RetailTransaction;
            retailTrans = _retailTrans;

            if (!string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
            {
                sCustomerId = retailTrans.Customer.CustomerId;
            }
        }

        public bool isMRP(string itemid)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) MRP FROM [INVENTTABLE] WHERE ITEMID='" + itemid + "' ");


            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());

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

            int iResult = 0;
            iResult = Convert.ToInt16(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return iResult;
        }

        private bool getValidMRPItemLineDiscount(ref decimal dDiscValue)
        {
            // Start : RH 01/09/2014
            int iSPId = 0;
            iSPId = getUserDiscountPermissionId();
            decimal dinitDiscValue = 0;
            string sFieldName = string.Empty;
            decimal dLineDisc = 0m;
            bool bIsValid = false;
            decimal dChangedTotAmt = 0m;
            decimal dTotAmt = 0m;

            if (!string.IsNullOrEmpty(txtMakDiscValue.Text))
                dLineDisc = Convert.ToDecimal(txtMakDiscValue.Text);
            else
                dLineDisc = 0;

            if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.CHANGEDTOTAMT))
                dChangedTotAmt = Convert.ToDecimal(_saleLineItem.PartnerData.CHANGEDTOTAMT);
            else
                dChangedTotAmt = 0;

            if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.TotalAmount))
                dTotAmt = Convert.ToDecimal(_saleLineItem.PartnerData.TotalAmount);
            else
                dTotAmt = 0;


            if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Salesperson)
                sFieldName = "MAXSALESPERSONSDISCPCT";
            else if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager)
                sFieldName = "MAXMANAGERLEVELDISCPCT";
            else if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager2)
                sFieldName = "MAXMANAGERLEVEL2DISCPCT";
            else
                sFieldName = "OPENINGDISCPCT";

            if (IsRetailItem(sBaseItemID))
            {
                //if (isMRPUCP)
                //{
                //    //dinitDiscValue = GetDiscountFromDiscPolicy(sBaseItemID, Convert.ToDecimal(_saleLineItem.PartnerData.ACTTOTAMT), sFieldName, 0);
                //    if (string.IsNullOrEmpty(txtPromoCode.Text))
                //        dinitDiscValue = GetDiscountFromDiscPolicy(sBaseItemID, Convert.ToDecimal(_saleLineItem.PartnerData.ACTTOTAMT), sFieldName, 0, "");
                //    else
                //        dinitDiscValue = GetDiscountFromDiscPolicy(sBaseItemID, Convert.ToDecimal(_saleLineItem.PartnerData.ACTTOTAMT), "PROMODISCOUNT", 0, txtPromoCode.Text.Trim());

                //}
                if (dinitDiscValue > 0)
                {
                    dDiscValue = (Convert.ToDecimal(_saleLineItem.PartnerData.ACTTOTAMT) * dinitDiscValue) / 100;
                    decimal dExistingLineDisc = 0m;

                    dExistingLineDisc = _saleLineItem.LineDiscount;

                    if ((dLineDisc > dDiscValue))
                    {
                        MessageBox.Show("Line discount percentage should not more than " + dDiscValue + "");
                        bIsValid = false;
                    }
                    else if (dLineDisc < 0)
                    {
                        MessageBox.Show("Line discount percentage should not negetive value");
                        bIsValid = false;
                    }
                    else
                        bIsValid = true;
                }
                else if (dLineDisc > 0)
                {
                    MessageBox.Show("Not allowed for this item");
                    //sMkPromoCode = "";
                    //iIsMkDiscFlat = 0;
                    _saleLineItem.PartnerData.PROMOCODE = "";
                    _saleLineItem.PartnerData.FLAT = 0;
                    bIsValid = false;
                }
            }
            else
                bIsValid = true;


            return bIsValid;

        }

        private decimal GetDiscountFromDiscPolicy(string sItemId, decimal dItemMainValue, string sWhichFieldValueWillGet, int iFlat, string sPromoCode)
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
            string sPCode = "";
            if (sWhichFieldValueWillGet == "OPENINGDISCPCT")
                sFlatQry = " AND CRWDISCOUNTPOLICY.FLAT =" + iFlat + "";
            else
                sFlatQry = "";

            if (!string.IsNullOrEmpty(sPromoCode))
                sPCode = " AND CRWDISCOUNTPOLICY.PROMOTIONCODE ='" + sPromoCode + "'";
            else
                sPCode = "";

            string commandText = "  DECLARE @LOCATION VARCHAR(20) " +
                                 "  DECLARE @PRODUCT BIGINT " +
                                 "  DECLARE @PARENTITEM VARCHAR(20) " +
                                 "  DECLARE @CUSTCODE VARCHAR(20) " +
                                 "  DECLARE @PRODUCTCODE VARCHAR(20) " +
                                 "  DECLARE @COLLECTIONCODE VARCHAR(20) " +
                                 "  DECLARE @ARTICLE_CODE VARCHAR(20) " +

                                 "  IF EXISTS(SELECT TOP(1) ACTIVATE FROM CRWDISCOUNTPOLICY )" + // WHERE RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'
                                 "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "' AND RETAIL=1)" +
                                 "  BEGIN SELECT @PARENTITEM = ITEMIDPARENT ,@PRODUCTCODE=PRODUCTTYPECODE" + // FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                                 "  ,@ARTICLE_CODE=ARTICLE_CODE,@COLLECTIONCODE =CollectionCode" +
                                 "  FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                //"  SELECT @PRODUCT = ITEMID FROM [INVENTTABLE] WHERE ITEMID = @PARENTITEM " +
                                 "  SELECT @CUSTCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "' " +
                                 "  END ";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //1
                " FROM   CRWDISCOUNTPOLICY  " +
                " WHERE (CRWDISCOUNTPOLICY.ITEMID=@PARENTITEM) " +
                " AND (CRWDISCOUNTPOLICY.CODE=@CUSTCODE) " +
                " AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "' OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE))" +
                     " BEGIN " +
                     "     SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT,ISNULL(CLUBBED,0) as CLUBBED  FROM   CRWDISCOUNTPOLICY   " +
                     "     WHERE     " +
                     "     (CRWDISCOUNTPOLICY.ITEMID=@PARENTITEM ) AND (CRWDISCOUNTPOLICY.CODE=@CUSTCODE ) " +

                     "  AND   (CRWDISCOUNTPOLICY.ProductCode=@ProductCode  or CRWDISCOUNTPOLICY.ProductCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.CollectionCode=@CollectionCode  or CRWDISCOUNTPOLICY.CollectionCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.ArticleCode=@ARTICLE_CODE  or CRWDISCOUNTPOLICY.ArticleCode='')" +

                     "     AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                     "      and ('" + dItemMainValue + "'  BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)   " +
                     "       AND CRWDISCOUNTPOLICY.ACTIVATE = 1" +
                     " " + sFlatQry + "" +
                     " " + sPCode + "" +
                     " ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.CODE DESC ";

            if (string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWDISCOUNTPOLICY.FLAT ASC END";
            else
                commandText += ", CRWDISCOUNTPOLICY.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //2
               " FROM   CRWDISCOUNTPOLICY  " +
               " WHERE (CRWDISCOUNTPOLICY.ITEMID=@PARENTITEM) " +
               " AND (CRWDISCOUNTPOLICY.CODE='') " +
               " AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE))" +
                    " BEGIN " +
                    "     SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT,ISNULL(CLUBBED,0) as CLUBBED   FROM   CRWDISCOUNTPOLICY   " +
                    "     WHERE     " +
                    "     (CRWDISCOUNTPOLICY.ITEMID=@PARENTITEM ) AND (CRWDISCOUNTPOLICY.CODE='' ) " +

                      "  AND   (CRWDISCOUNTPOLICY.ProductCode=@ProductCode  or CRWDISCOUNTPOLICY.ProductCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.CollectionCode=@CollectionCode  or CRWDISCOUNTPOLICY.CollectionCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.ArticleCode=@ARTICLE_CODE  or CRWDISCOUNTPOLICY.ArticleCode='')" +

                    "     AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                    "      and ('" + dItemMainValue + "'  BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)   " +
                    "       AND CRWDISCOUNTPOLICY.ACTIVATE = 1" +
                    " " + sFlatQry + "" +
                     " " + sPCode + "" +
                    " ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.CODE DESC ";

            if (string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWDISCOUNTPOLICY.FLAT ASC END";
            else
                commandText += ", CRWDISCOUNTPOLICY.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //3
                " FROM   CRWDISCOUNTPOLICY  " +
                " WHERE (CRWDISCOUNTPOLICY.ITEMID='') " +
                " AND (CRWDISCOUNTPOLICY.CODE=@CUSTCODE) " +
                " AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE))" +
                     " BEGIN " +
                     "     SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT,ISNULL(CLUBBED,0) as CLUBBED  FROM   CRWDISCOUNTPOLICY   " +
                     "     WHERE     " +
                     "     (CRWDISCOUNTPOLICY.ITEMID='' ) AND (CRWDISCOUNTPOLICY.CODE=@CUSTCODE ) " +

                       "  AND   (CRWDISCOUNTPOLICY.ProductCode=@ProductCode  or CRWDISCOUNTPOLICY.ProductCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.CollectionCode=@CollectionCode  or CRWDISCOUNTPOLICY.CollectionCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.ArticleCode=@ARTICLE_CODE  or CRWDISCOUNTPOLICY.ArticleCode='')" +

                     "     AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                     "      and ('" + dItemMainValue + "'  BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)   " +
                     "       AND CRWDISCOUNTPOLICY.ACTIVATE = 1" +
                     " " + sFlatQry + "" +
                      " " + sPCode + "" +
                     " ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.CODE DESC ";

            if (string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWDISCOUNTPOLICY.FLAT ASC END";
            else
                commandText += ", CRWDISCOUNTPOLICY.FLAT DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) RETAILSTOREID" + //4
               " FROM   CRWDISCOUNTPOLICY  " +
               " WHERE (CRWDISCOUNTPOLICY.ITEMID='') " +
               " AND (CRWDISCOUNTPOLICY.CODE='') " +
               " AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE))" +
                    " BEGIN " +
                    "     SELECT  TOP (1) CAST(CRWDISCOUNTPOLICY." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT,ISNULL(CLUBBED,0) as CLUBBED  FROM   CRWDISCOUNTPOLICY   " +
                    "     WHERE     " +
                    "     (CRWDISCOUNTPOLICY.ITEMID='' ) AND (CRWDISCOUNTPOLICY.CODE='' ) " +

                      "  AND   (CRWDISCOUNTPOLICY.ProductCode=@ProductCode  or CRWDISCOUNTPOLICY.ProductCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.CollectionCode=@CollectionCode  or CRWDISCOUNTPOLICY.CollectionCode='')" +
                     "  AND   (CRWDISCOUNTPOLICY.ArticleCode=@ARTICLE_CODE  or CRWDISCOUNTPOLICY.ArticleCode='')" +

                    "     AND (CRWDISCOUNTPOLICY.RETAILSTOREID='" + ApplicationSettings.Database.StoreID + "'  OR CRWDISCOUNTPOLICY.RETAILSTOREID='')" +
                    "      and ('" + dItemMainValue + "'  BETWEEN CRWDISCOUNTPOLICY.FROMAMOUNT AND CRWDISCOUNTPOLICY.TOAMOUNT)  " +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWDISCOUNTPOLICY.FROMDATE AND CRWDISCOUNTPOLICY.TODATE)   " +
                    "       AND CRWDISCOUNTPOLICY.ACTIVATE = 1" +
                    " " + sFlatQry + "" +
                     " " + sPCode + "" +
                    " ORDER BY CRWDISCOUNTPOLICY.ITEMID DESC,CRWDISCOUNTPOLICY.CODE DESC ";

            if (string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWDISCOUNTPOLICY.FLAT ASC END";
            else
                commandText += ", CRWDISCOUNTPOLICY.FLAT DESC END";

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dResult = Convert.ToDecimal(reader.GetValue(0));
                    // _saleLineItem.PartnerData.PROMOCODE = Convert.ToString(reader.GetValue(1));
                    _saleLineItem.PartnerData.FLAT = Convert.ToInt16(reader.GetValue(2));
                    _saleLineItem.PartnerData.NimMRPDiscClubbed = Convert.ToInt16(reader.GetValue(3));

                    _saleLineItem.PartnerData.NimPromoMRPDiscClubbed = Convert.ToInt16(reader.GetValue(3));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dResult;
        }

        private decimal GetMkDiscountFromDiscPolicy(string sItemId, decimal dItemMainValue, string sWhichFieldValueWillGet, string sPromoCode)
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
            string sPCode = "";
            if (!string.IsNullOrEmpty(sPromoCode))
                sPCode = " AND CRWMAKINGDISCOUNT.PROMOTIONCODE ='" + sPromoCode + "'";
            else
                sPCode = "";

            if (sWhichFieldValueWillGet == "OPENINGDISCPCT")
                sFlatQry = " AND CRWMAKINGDISCOUNT.FLAT =1";
            else
                sFlatQry = "";

            string commandText = " DECLARE @Temp TABLE (MAXSALESPERSONSDISCPCT numeric(20,2)," +
                                 " PROMOTIONCODE nvarchar(20),FLAT int,DiscountType int, CLUBBED int);" +
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
                     " INSERT INTO @Temp (MAXSALESPERSONSDISCPCT, PROMOTIONCODE,FLAT,DiscountType,CLUBBED)  " +
                     "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT," +
                     " ISNULL(DiscountType,0) as DiscountType, " +
                     " ISNULL(CLUBBED,0) as CLUBBED " +
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
                     " " + sPCode + "" +
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
                    " INSERT INTO @Temp (MAXSALESPERSONSDISCPCT, PROMOTIONCODE,FLAT,DiscountType,CLUBBED)  " +
                    "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT," +
                    " ISNULL(DiscountType,0) as DiscountType," +
                    " ISNULL(CLUBBED,0) as CLUBBED " +
                    " FROM   CRWMAKINGDISCOUNT   " +
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
                     " " + sPCode + "" +
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
                      " INSERT INTO @Temp (MAXSALESPERSONSDISCPCT, PROMOTIONCODE,FLAT,DiscountType,CLUBBED)  " +
                     "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                     "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT ," +
                     " ISNULL(DiscountType,0) as DiscountType," +
                      " ISNULL(CLUBBED,0) as CLUBBED " +
                     " FROM   CRWMAKINGDISCOUNT   " +
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
                      " " + sPCode + "" +
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
                     " INSERT INTO @Temp (MAXSALESPERSONSDISCPCT, PROMOTIONCODE,FLAT,DiscountType,CLUBBED)  " +
                    "     SELECT  TOP (1) CAST(CRWMAKINGDISCOUNT." + sWhichFieldValueWillGet.Trim() + " AS decimal(18, 2))" +
                    "     ,ISNULL(PROMOTIONCODE,'') AS PROMOTIONCODE,ISNULL(FLAT,0) as FLAT," +
                    " ISNULL(DiscountType,0) as DiscountType," +
                    " ISNULL(CLUBBED,0) as CLUBBED " +
                    " FROM   CRWMAKINGDISCOUNT   " +
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
                     " " + sPCode + "" +
                    " ORDER BY CRWMAKINGDISCOUNT.ITEMID DESC,CRWMAKINGDISCOUNT.CODE DESC ";
            if (string.IsNullOrEmpty(sFlatQry))
                commandText += ", CRWMAKINGDISCOUNT.FLAT ASC END";
            else
                commandText += ", CRWMAKINGDISCOUNT.FLAT DESC END";

            commandText += " select TOP 1 * from @Temp";

            SqlCommand commandMk = new SqlCommand(commandText.ToString(), connection);
            commandMk.CommandTimeout = 0;

            using (SqlDataReader mkDiscRd = commandMk.ExecuteReader())
            {
                while (mkDiscRd.Read())
                {
                    dResult = Convert.ToDecimal(mkDiscRd.GetValue(0));
                    //_saleLineItem.PartnerData.PROMOCODE = Convert.ToString(mkDiscRd.GetValue(1));
                    _saleLineItem.PartnerData.FLAT = Convert.ToInt16(mkDiscRd.GetValue(2));
                    iNimMakingDiscCalcType = Convert.ToInt16(mkDiscRd.GetValue(3));
                    _saleLineItem.PartnerData.NimMakingDiscClubbed = Convert.ToInt16(mkDiscRd.GetValue(4));

                    _saleLineItem.PartnerData.NimPromoMakingDiscClubbed = Convert.ToInt16(mkDiscRd.GetValue(4));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();


            return dResult;
        }

        private bool getValidMakingCalValue(ref decimal dDiscValue)
        {
            string sFieldName = string.Empty;
            int iSPId = 0;
            iSPId = getUserDiscountPermissionId();

            #region Variable
            bool bIsValid = false;
            decimal dinitDiscValue = 0;
            decimal dQty = 0;
            decimal dMkPerDisc = 0;
            int iPcs = 0;
            decimal dMkRate1 = 0m;
            decimal dMkRate = 0m;
            decimal dTotAmt = 0m;
            decimal dActTotAmt = 0m;
            decimal dManualChangesAmt = 0m;
            decimal dMkDiscTotAmt = 0m;
            decimal dChangedTotAmt = 0m;

            decimal dPcs = 0m;

            if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Salesperson)
                sFieldName = "MAXSALESPERSONSDISCPCT";
            else if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager)
                sFieldName = "MAXMANAGERLEVELDISCPCT";
            else if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager2)
                sFieldName = "MAXMANAGERLEVEL2DISCPCT";
            else
                sFieldName = "OPENINGDISCPCT";


            if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.Pieces))
                dPcs = Convert.ToDecimal(_saleLineItem.PartnerData.Pieces);
            else
                dPcs = 0;

            #region Line discount % // added on 140619 req by Supapta
            DataSet dsIngredients = new DataSet();
            int i = 1;
            int index = 1;
            int iMetalType = 0;

            StringReader reader = new StringReader(Convert.ToString(_saleLineItem.PartnerData.Ingredients));
            dsIngredients.ReadXml(reader);

            foreach (DataRow drIngredients in dsIngredients.Tables[0].Rows)
            {
                index = i;
                iMetalType = Convert.ToInt16(!string.IsNullOrEmpty(Convert.ToString(drIngredients["MetalType"])) ? drIngredients["MetalType"] : "0");

                if ((iMetalType == (int)MetalType.Gold) || (iMetalType == (int)MetalType.Silver)
                    || (iMetalType == (int)MetalType.Platinum) || (iMetalType == (int)MetalType.Palladium))
                {
                    dQty += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                }
                i++;
            }
            #endregion

            if (dQty == 0)
            {
                if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.Quantity))
                    dQty = Convert.ToDecimal(_saleLineItem.PartnerData.Quantity);
                else
                    dQty = 0;
            }

            if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.Pieces))
                iPcs = Convert.ToInt16(_saleLineItem.PartnerData.Pieces);
            else
                iPcs = 0;

            if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.ACTMKRATE))
                dMkRate1 = Convert.ToDecimal(_saleLineItem.PartnerData.ACTMKRATE);
            else
                dMkRate1 = 0;

            if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.MakingRate))
                dMkRate = Convert.ToDecimal(_saleLineItem.PartnerData.MakingRate);
            else
                dMkRate = 0;

            if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.TotalAmount))//TotalAmount
                dTotAmt = Convert.ToDecimal(_saleLineItem.PartnerData.TotalAmount);
            else
                dTotAmt = 0;
            if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.ACTTOTAMT))
                dActTotAmt = Convert.ToDecimal(_saleLineItem.PartnerData.ACTTOTAMT);
            else
                dActTotAmt = 0;
            //if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.MakingTotalDiscount))
            //    dMkDiscTotAmt = Convert.ToDecimal(_saleLineItem.PartnerData.MakingTotalDiscount);
            //else
            //    dMkDiscTotAmt = 0;

            if (!string.IsNullOrEmpty(txtMakDiscValue.Text))
                dMkDiscTotAmt = Convert.ToDecimal(txtMakDiscValue.Text);
            else
                dMkDiscTotAmt = 0;

            if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.CHANGEDTOTAMT))
                dManualChangesAmt = Convert.ToDecimal(_saleLineItem.PartnerData.CHANGEDTOTAMT);
            else
                dManualChangesAmt = 0;

            //if (!isMRPUCP)
            //{
            //    if (string.IsNullOrEmpty(txtPromoCode.Text))
            //        dinitDiscValue = GetMkDiscountFromDiscPolicy(sBaseItemID, dQty, sFieldName, "");// get OPENINGDISCPCT field value FOR THE OPENING
            //    else
            //        dinitDiscValue = GetMkDiscountFromDiscPolicy(sBaseItemID, dQty, "PROMODISCOUNT", txtPromoCode.Text.Trim());// get OPENINGDISCPCT field value FOR THE OPENING
            //}

            if (!string.IsNullOrEmpty(txtMakDiscValue.Text))//txtMakingDisc
                dMkPerDisc = Convert.ToDecimal(txtMakDiscValue.Text);//txtMakingDisc
            else
                dMkPerDisc = 0;

            if (!string.IsNullOrEmpty(_saleLineItem.PartnerData.CHANGEDTOTAMT))
                dChangedTotAmt = Convert.ToDecimal(_saleLineItem.PartnerData.CHANGEDTOTAMT);
            else
                dChangedTotAmt = 0;

            decimal dMakingQty = 0m;

            dMakingQty = Convert.ToDecimal(_saleLineItem.PartnerData.Quantity);

            #endregion

            if (iNimMakingDiscCalcType == (int)MakingDiscType.Percent)//MakingDiscountType
                dDiscValue = (Convert.ToDecimal(_saleLineItem.PartnerData.MakingAmount) * dinitDiscValue) / 100;
            else if (iNimMakingDiscCalcType == (int)MakingDiscType.PerGram)
                dDiscValue = (dMakingQty * dinitDiscValue);
            else
                dDiscValue = dinitDiscValue * iPcs;

            #region validate
            if (dDiscValue >= 0)
            {
                decimal dExistingLineDisc = 0m;

                dExistingLineDisc = _saleLineItem.LineDiscount;
                //if ((dExistingLineDisc > dMkDiscTotAmt) && !string.IsNullOrEmpty(txtPromoCode.Text))
                //{
                //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Promo discount is lower than existing discount,Do you want to over write ? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                //    {
                //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                //        if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "NO")
                //        {
                //            txtDiscValue.Text = "";
                //            bIsValid = false;
                //        }
                //    }
                //}

                if ((dMkDiscTotAmt > dDiscValue))
                {
                    MessageBox.Show("Discount should not more than " + Convert.ToString(decimal.Round(dDiscValue, 2, MidpointRounding.AwayFromZero)) + "");

                    txtMakDiscValue.Text = "";
                    bIsValid = false;
                }
                else if (dMkDiscTotAmt < 0)
                {
                    MessageBox.Show("Discount should not negetive value");

                    txtMakDiscValue.Text = "";
                    bIsValid = false;
                }
                else
                {
                    bIsValid = true;
                }
            }
            #endregion

            return bIsValid;
        }

        private bool IsRetailItem(string sItemId)
        {
            bool bRetailItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

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

        #region iSEntryOk()
        public bool iSEntryOk()
        {
            decimal vMak = 0;
            decimal vStn = 0;
            decimal vDia = 0;
            decimal vMakingValue = 0;
            decimal vMakingAgreement = 0;
            decimal vStoneAgreement = 0;
            decimal vDiamondAgreement = 0;
            DateTime dtFDate = DateTime.Now;

            string sStorGrp = getStoreFormatCode();

            vMakingValue = GetMakDiscount(sBaseItemID, sStorGrp, ref dtFDate,ref vMakingAgreement, ref vStoneAgreement, ref vDiamondAgreement);

            
             if (!string.IsNullOrEmpty(txtMakDiscValue.Text))
                vMak  = Convert.ToDecimal(txtMakDiscValue.Text);
            else
                vMak = 0;
            if (!string.IsNullOrEmpty(txtStnDiscValue.Text))
                vStn  = Convert.ToDecimal(txtStnDiscValue.Text);
            else
                vStn = 0;
            if (!string.IsNullOrEmpty(txtDiaDiscValue.Text))
                vDia  = Convert.ToDecimal(txtDiaDiscValue.Text);
            else
                vDia = 0;

            if (Convert.ToDecimal(vMak) > 100)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Making discount percentage should not more than 100", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtMakDiscValue.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else if (Convert.ToDecimal(vMak) < 0)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Making discount percentage should not be negative", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtMakDiscValue.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else if (Convert.ToDecimal(vStn) > 100)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Stone discount percentage should not more than 100", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtStnDiscValue.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else if (Convert.ToDecimal(vStn) < 0)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Stone discount percentage should not be negative", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtStnDiscValue.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else if (Convert.ToDecimal(vDia) > 100)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Diamond discount percentage should not more than 100", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtDiaDiscValue.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else if (Convert.ToDecimal(vDia) < 0)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Diamond discount percentage should not be negative", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtDiaDiscValue.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else if (vMak > vMakingAgreement)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Making discount percentage should not more than agreement " + vMakingAgreement + "", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtMakDiscValue.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else if (vStn > vStoneAgreement)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Stone discount percentage should not more than agreement " + vStoneAgreement + " ", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtMakDiscValue.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else if (vDia > vDiamondAgreement)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Diamond discount percentage should not more than agreement " + vDiamondAgreement + " ", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtMakDiscValue.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        #endregion

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (iSEntryOk())
            {
            decimal dDiscValue = 0m;
            decimal dGivenDisc = 0m;

            if (_saleLineItem.PartnerData.TransactionType == "0" && _saleLineItem.PartnerData.NimReturnLine == 0
                              && _saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                              && Convert.ToDecimal(_saleLineItem.PartnerData.TotalAmount) > 0
                              && !_saleLineItem.Voided)
            {
                _saleLineItem.PartnerData.NimMakDisc = 0;
                _saleLineItem.PartnerData.NimStnDisc = 0;
                _saleLineItem.PartnerData.NimDiaDisc = 0;

                #region MakingDisc
                if (!string.IsNullOrEmpty(txtMakDiscValue.Text))
                {
                    dGivenDisc = Convert.ToDecimal(txtMakDiscValue.Text);

                    if (!isMRPUCP)
                    {
                        //if (!getValidMRPItemLineDiscount(ref dDiscValue))
                        //{
                        //    _saleLineItem.PartnerData.NimMRPDiscType = false;
                        //    return;
                        //}
                        //else
                        //{
                        if (dGivenDisc >= 0)  //if (dGivenDisc <= dDiscValue)
                        {
                            _saleLineItem.PartnerData.NimMakDisc = dGivenDisc;
                            _saleLineItem.PartnerData.NimDiscLine = iLine;
                            _saleLineItem.PartnerData.MakStnDiaDisc = true;
                            _saleLineItem.PartnerData.MakStnDiaDiscDone = false;
                            retailTrans.PartnerData.MakStnDiaDisc = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid discount");
                        }
                        //}
                    }
                }
                #endregion

                #region StoneDisc
                if (!string.IsNullOrEmpty(txtStnDiscValue.Text))
                {
                    dGivenDisc = Convert.ToDecimal(txtStnDiscValue.Text);

                    if (!isMRPUCP)
                    {
                        //if (!getValidMRPItemLineDiscount(ref dDiscValue))
                        //{
                        //    _saleLineItem.PartnerData.NimMRPDiscType = false;
                        //    return;
                        //}
                        //else
                        //{
                        if (dGivenDisc >= 0)  //if (dGivenDisc <= dDiscValue)
                        {
                            _saleLineItem.PartnerData.NimStnDisc = dGivenDisc;
                            _saleLineItem.PartnerData.NimDiscLine = iLine;
                            _saleLineItem.PartnerData.MakStnDiaDisc = true;
                            _saleLineItem.PartnerData.MakStnDiaDiscDone = false;
                            retailTrans.PartnerData.MakStnDiaDisc = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid discount");
                        }
                        //}
                    }
                }
                #endregion

                #region DiamondDisc
                if (!string.IsNullOrEmpty(txtDiaDiscValue.Text))
                {
                    dGivenDisc = Convert.ToDecimal(txtDiaDiscValue.Text);

                    if (!isMRPUCP)
                    {
                        //if (!getValidMRPItemLineDiscount(ref dDiscValue))
                        //{
                        //    _saleLineItem.PartnerData.NimMRPDiscType = false;
                        //    return;
                        //}
                        //else
                        //{
                        if (dGivenDisc >= 0)  //if (dGivenDisc <= dDiscValue)
                        {
                            _saleLineItem.PartnerData.NimDiaDisc = dGivenDisc;
                            _saleLineItem.PartnerData.NimDiscLine = iLine;
                            _saleLineItem.PartnerData.MakStnDiaDisc = true;
                            _saleLineItem.PartnerData.MakStnDiaDiscDone = false;
                            retailTrans.PartnerData.MakStnDiaDisc = true;
                            this.Close();
                        }
                        else
                        {
                            MessageBox.Show("Invalid discount");
                        }
                        //}
                    }
                }
                #endregion
            }
            }
         }

        private void btnCLose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtMakDiscValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtStnDiscValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtDiaDiscValue_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }


        private decimal GetMakDiscount(string sItemId, string sStoreGrp, ref DateTime dtFROMDATE, ref decimal iMakAgr, ref decimal iStnAgr, ref decimal iDiaAgr)
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
                    iMakAgr = Convert.ToDecimal(mkDiscRd.GetValue(7));
                    iStnAgr = Convert.ToDecimal(mkDiscRd.GetValue(8));
                    iDiaAgr = Convert.ToDecimal(mkDiscRd.GetValue(9));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();


            return dResult;
        }
        

    }
}
