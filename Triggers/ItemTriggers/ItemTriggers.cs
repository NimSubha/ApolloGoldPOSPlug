//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction;
using System.Collections;
using System.IO;
using BlankOperations.WinFormsTouch;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;

namespace Microsoft.Dynamics.Retail.Pos.ItemTriggers
{
    /// <summary>
    /// <example><code>
    /// // In order to get a copy of the last item added to the transaction, use the following code:
    /// LinkedListNode<SaleLineItem> saleItem = ((RetailTransaction)posTransaction).SaleItems.Last;
    /// // To remove the last line use:
    /// ((RetailTransaction)posTransaction).SaleItems.RemoveLast();
    /// </code></example>
    /// </summary>
    [Export(typeof(IItemTrigger))]
    public class ItemTriggers : IItemTrigger
    {
        //Start:Nim
        #region Nim Changes
        [Import]
        private IApplication application;

        string sItemIdParent = string.Empty;
        decimal dGroupCostPrice = 0;
        decimal dUpdatedCostPrice = 0;
        decimal dSellingCostPrice = 0;
        int iOWNDMD = 0; int iOWNOG = 0; int iOTHERDMD = 0; int iOTHEROG = 0;

        decimal dBulkPdsQty = 0m;
        decimal dBulkQty = 0m;

        enum AdvAgainst
        {
            None = 0,
            OGPurchase = 1,
            OGExchange = 2,
            SaleReturn = 3,
        }

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
        }

        enum RateTypeNew
        {
            Purchase = 0,
            OGP = 1,
            OGOP = 2,
            Sale = 3,
            GSS = 4,
            Exchange = 6,
            OtherExchange = 8,
        }

        public bool isMRP(string itemid, SqlConnection connection)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) MRP FROM [INVENTTABLE] WHERE ITEMID='" + itemid + "' ");


            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());

        }

        #region SKU Operations
        private void CheckForSKUExistence(string itemid, SqlConnection connection, out int? isLock
            , out int? isAvailable, ref string sSuspendCustAndTransId, ref string sTransitCounter)
        {
            StringBuilder sbQuery = new StringBuilder();

            // sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM skutable_posted WHERE SkuNumber='" + itemid + "') ");
            sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM SKUTableTrans WHERE SkuNumber='" + itemid + "') ");
            sbQuery.Append(" BEGIN  ");
            // sbQuery.Append(" SELECT isAvailable,isLocked FROM skutable_posted WHERE SkuNumber='" + itemid + "' ");
            sbQuery.Append(" SELECT isAvailable,isLocked,SUSPENDCUSTACCOUNT,ToCounter FROM SKUTableTrans WHERE SkuNumber='" + itemid + "'");// and ToCounter!='Transit' 
            sbQuery.Append(" END ");
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = null;
            reader = cmd.ExecuteReader();

            //  return Convert.ToBoolean(cmd.ExecuteScalar());

            if (reader.HasRows)
            {
                bool isAvail = false;
                bool isLocked = false;
                while (reader.Read())
                {
                    isAvail = Convert.ToBoolean(reader.GetValue(0));
                    isLocked = Convert.ToBoolean(reader.GetValue(1));
                    sSuspendCustAndTransId = Convert.ToString(reader.GetValue(2));
                    sTransitCounter = Convert.ToString(reader.GetValue(3));
                }
                reader.Close();
                isLock = Convert.ToInt16(isLocked);
                isAvailable = Convert.ToInt16(isAvail);

            }
            else
            {
                reader.Close();
                isLock = null;
                isAvailable = null;
                sSuspendCustAndTransId = "";
                sTransitCounter = "";
            }

        }

        private void CheckForBookSKUDelivered(string itemid, SqlConnection connection, out int? isValid, ref string sOrderBook, ref int isRelesed, ref string sTransitCounter)
        {
            StringBuilder sbQuery = new StringBuilder();
            isValid = 0;


            sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM SKUTableTrans WHERE SkuNumber='" + itemid + "' and BOOKEDORDERNO!='') ");
            sbQuery.Append(" BEGIN  ");
            sbQuery.Append(" SELECT Isnull(o.isDelivered,0) ,s.BOOKEDORDERNO,sb.RELEASED,ToCounter FROM SKUTableTrans s ");
            sbQuery.Append(" left join CUSTORDER_HEADER O on s.BOOKEDORDERNO=o.ORDERNUM ");// and ToCounter!='Transit' 
            sbQuery.Append(" left join RETAILCUSTOMERDEPOSITSKUDETAILS Sb on  s.SkuNumber= sb.SKUNUMBER WHERE S.SkuNumber='" + itemid + "'");
            sbQuery.Append(" END ");
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = null;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    isValid = Convert.ToInt16(reader.GetValue(0));
                    sOrderBook = Convert.ToString(reader.GetValue(1));
                    isRelesed = Convert.ToInt16(reader.GetValue(2));
                    sTransitCounter = Convert.ToString(reader.GetValue(3));
                }
                reader.Close();
            }
            else
            {
                reader.Close();
                isValid = 0;
                sOrderBook = "";
                sTransitCounter = "";
            }

        }

        private void CheckForBookSKUDelivered(string itemid, SqlConnection connection, out int? isValid, ref string sOrderBook, ref int isRelesed)
        {
            StringBuilder sbQuery = new StringBuilder();
            isValid = 0;


            sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM SKUTableTrans WHERE SkuNumber='" + itemid + "' and BOOKEDORDERNO!='') ");
            sbQuery.Append(" BEGIN  ");
            sbQuery.Append(" SELECT o.isDelivered ,s.BOOKEDORDERNO,sb.RELEASED FROM SKUTableTrans s ");
            sbQuery.Append(" left join CUSTORDER_HEADER O on s.BOOKEDORDERNO=o.ORDERNUM ");// and ToCounter!='Transit' 
            sbQuery.Append(" left join RETAILCUSTOMERDEPOSITSKUDETAILS Sb on  s.SkuNumber= sb.SKUNUMBER WHERE S.SkuNumber='" + itemid + "'");
            sbQuery.Append(" END ");
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = null;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    isValid = Convert.ToInt16(reader.GetValue(0));
                    sOrderBook = Convert.ToString(reader.GetValue(1));
                    isRelesed = Convert.ToInt16(reader.GetValue(2));
                }
                reader.Close();
            }
            else
            {
                reader.Close();
                isValid = 0;
                sOrderBook = "";
            }

        }

        private void CheckForReturnSKUExistence(string itemid, SqlConnection connection, out int? isRetunLock, out int? isReturnAvailable)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append(" DECLARE @RETURN BIT ");
            // sbQuery.Append(" SELECT @RETURN=ITEMRETURN FROM RETAILTEMPTABLE WHERE ID=2 ");
            sbQuery.Append(" SELECT @RETURN=ITEMRETURN FROM RETAILTEMPTABLE WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "); // RETAILTEMPTABLE
            sbQuery.Append(" IF(@RETURN=1) ");
            sbQuery.Append(" BEGIN  ");
            //sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM skutable_posted WHERE SKUNUMBER='" + itemid + "')  "); 
            sbQuery.Append(" IF EXISTS(SELECT TOP 1 * FROM SKUTableTrans WHERE SKUNUMBER='" + itemid + "')  ");
            sbQuery.Append(" BEGIN  ");
            //sbQuery.Append(" SELECT isLocked,isAvailable FROM skutable_posted WHERE SKUNUMBER='" + itemid + "'  "); 
            sbQuery.Append(" SELECT isLocked,isAvailable FROM SKUTableTrans WHERE SKUNUMBER='" + itemid + "'  ");
            sbQuery.Append(" END  ");
            sbQuery.Append(" END  ");


            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = null;
            reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                bool isAvail = false;
                bool isLocked = false;
                while (reader.Read())
                {
                    isLocked = Convert.ToBoolean(reader.GetValue(0));
                    isAvail = Convert.ToBoolean(reader.GetValue(1));
                }
                reader.Close();
                isRetunLock = Convert.ToInt16(isLocked);
                isReturnAvailable = Convert.ToInt16(isAvail);
            }
            else
            {
                reader.Close();
                isRetunLock = null;
                isReturnAvailable = null;
            }

        }
        #endregion

        #region Adjustment Item Name
        private string AdjustmentItemID()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) ADJUSTMENTITEMID FROM [RETAILPARAMETERS] where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }

        private DateTime getTaxCutOffDate()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) TAX_CUTOFFDATE FROM [RETAILPARAMETERS] where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToDateTime(cmd.ExecuteScalar());
        }
        #endregion

        #region GSS Maturity Adjustment Item ID
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
        #endregion


        private void UpdateMakingInfo(string sTableName, string sTransactionId, decimal dAmt)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN  Update " + sTableName + " set MakingAmt = MakingAmt - " + dAmt + " where TRANSACTIONID = '" + sTransactionId + "' END");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        #region Changed By Nimbus - Update Order Delivery
        private void updateOrderDelivery(string orderNum, string orderLineNum, bool voided, ISaleLineItem saleline, string itemid = "")
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            string commandText = string.Empty;
            if (orderNum == null && orderLineNum == null && !voided && !string.IsNullOrEmpty(itemid))
            {
                //  if (saleline != null && saleline.ZeroPriceValid)
                if (saleline != null)
                {
                    /* //SKU Table New
                    commandText = commandText + " UPDATE SKUTable_Posted SET isLocked='True' " +  
                                              " WHERE SkuNumber='" + itemid + "' " +
                        //  " AND STOREID='" + ApplicationSettings.Terminal.StoreId + "' " +
                                              " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' ";
                     */
                    commandText = commandText + " UPDATE SKUTableTrans SET isLocked='True' " +
                                              " WHERE SkuNumber='" + itemid + "' " +
                        //  " AND STOREID='" + ApplicationSettings.Terminal.StoreId + "' " +
                                              " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' ";

                }
                //  commandText = commandText + " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2";
                commandText = commandText + " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
            }
            else
            {
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
                    if (string.IsNullOrEmpty(orderNum))
                        orderNum = "0";
                    if (string.IsNullOrEmpty(orderLineNum))
                        orderLineNum = "0";
                    commandText = " UPDATE CUSTORDER_DETAILS SET isDelivered = 0 WHERE ORDERNUM='" + orderNum.Trim() + "' AND  LINENUM='" + orderLineNum.Trim() + "' " +
                                 " DECLARE @COUNT INT " +
                                 " SELECT @COUNT=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM='" + orderNum.Trim() + "' AND isDelivered = 0  " +
                                 " IF(@COUNT>0) " +
                                 " BEGIN " +
                                 " UPDATE CUSTORDER_HEADER SET isDelivered = 0 WHERE ORDERNUM='" + orderNum.Trim() + "'  " +
                                 " END ";

                }

                /* //SKU Table New
                commandText = commandText + " UPDATE SKUTable_Posted SET isLocked='False' " +  
                                            " WHERE SkuNumber='" + itemid + "' " +
                    //  " AND STOREID='" + ApplicationSettings.Terminal.StoreId + "' " +
                                            " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' ";
                 */
                commandText = commandText + " UPDATE SKUTableTrans SET isLocked='False' " +  //SKU Table New
                                            " WHERE SkuNumber='" + itemid + "' " +
                    //  " AND STOREID='" + ApplicationSettings.Terminal.StoreId + "' " +
                                            " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' ";
                //commandText = commandText + " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2";
                commandText = commandText + " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
            }

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            command.ExecuteNonQuery();

        }
        #endregion

        #region SaleOperation
        private bool SaleOperation(ISaleLineItem saleLineItem, IPosTransaction posTransaction, SqlConnection connection)
        {
            SaleLineItem saleLine = (SaleLineItem)saleLineItem;
            RetailTransaction retailTrans = posTransaction as RetailTransaction;

            string sMRPRate = string.Empty;

            ArrayList al = new ArrayList();
            if (saleLine != null)
            {
                foreach (SaleLineItem salsItems in retailTrans.SaleItems)
                {
                    if (salsItems.ItemId == AdjustmentItemID())
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId)))
                        {
                            StringBuilder sbQuery = new StringBuilder();
                            string sAdjTransId = Convert.ToString(salsItems.PartnerData.ServiceItemCashAdjustmentTransactionID);
                            sbQuery.Append("SELECT ORDERNUM AS CUSTOMERORDER FROM CUSTORDER_HEADER WHERE isDelivered=0");
                            sbQuery.Append("AND IsConfirmed=1 and custaccount='" + Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId) + "'");

                            sbQuery.Append(" union select SP.CUSTORDERNUM as CUSTOMERORDER from SKUTable_Posted SP");
                            sbQuery.Append(" left join CUSTORDER_HEADER H on SP.CUSTORDERNUM =H.ORDERNUM");
                            sbQuery.Append(" left join CUSTORDER_DETAILS D on d.ORDERNUM =h.ORDERNUM");
                            sbQuery.Append(" where CUSTORDERNUM!='' and d.isDelivered=0");
                            sbQuery.Append(" and CUSTACCOUNT='" + Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId) + "'");

                            //blocked bcz @ the time booked item taking for adjustment , b4 the sku fetching auto adv is adjusted issue
                            //sbQuery.Append(" SELECT ORDERNUM AS CUSTOMERORDER FROM RETAILADJUSTMENTTABLE WHERE ISADJUSTED=0");
                            //sbQuery.Append(" and CUSTACCOUNT='" + Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId) + "'");
                            //sbQuery.Append(" and ISADJUSTED=0 and TRANSACTIONID='" + sAdjTransId + "'");


                            if (connection.State == ConnectionState.Closed)
                                connection.Open();
                            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
                            SqlDataReader reader = cmd.ExecuteReader();
                            if (reader.HasRows)
                            {
                                while (reader.Read())
                                {
                                    al.Add(Convert.ToString(reader["CUSTOMERORDER"]));
                                }
                            }
                            reader.Close();
                            connection.Close();
                        }
                    }
                }
            }

            bool isMRPExists = isMRP(saleLineItem.ItemId, connection);
            RetailTransaction posClone = posTransaction as RetailTransaction;
            if (isMRPExists)
            {
                saleLine.PartnerData.isMRP = true;
                saleLine.PartnerData.TransactionType = "0";
                saleLine.PartnerData.Ingredients = "";
                saleLine.PartnerData.Quantity = "0";
                saleLine.PartnerData.IsSpecialDisc = false;
                saleLine.PartnerData.SpecialDiscInfo = "";
                saleLine.PartnerData.CustNo = "";
                saleLine.PartnerData.RETAILBATCHNO = string.Empty;
                saleLine.PartnerData.GrossWt = "0";

                RetailTransaction pos = posClone.CloneTransaction() as RetailTransaction;
                pos.Add(saleLine);
                application.Services.Price.GetPrice(pos);
                SaleLineItem saleItemforMrp = pos.SaleItems.Last.Value;
                sMRPRate = Convert.ToString(saleItemforMrp.Price);

            }
            else
            {
                if (!retailTrans.PartnerData.IsRepairReturn)
                {
                    saleLine.PartnerData.Quantity = "0";
                    saleLine.PartnerData.RETAILBATCHNO = string.Empty;
                }
                saleLine.PartnerData.TransactionType = "0";
                saleLine.PartnerData.isMRP = false;
                saleLine.PartnerData.IsSpecialDisc = false;
                saleLine.PartnerData.SpecialDiscInfo = "";
                saleLine.PartnerData.CustNo = "";
                saleLine.PartnerData.Ingredients = "";
                saleLine.PartnerData.TotalAmount = "";

            }

            connection.Close();

            #region Freegift Item
            decimal dCWQTY = 0;
            decimal dQTY = 0m;
            string sItemId = string.Empty;
            string sPromoCode = string.Empty;
            string sConf = string.Empty;
            string sSize = string.Empty;
            string sColor = string.Empty;
            string sProductType = string.Empty;
            string sArticleCode = string.Empty;
            string sAmt = string.Empty;

            if (retailTrans != null)
            {
                if (!string.IsNullOrEmpty(retailTrans.PartnerData.FREEGIFTCON))
                {
                    DataSet dsFreeGiftCon = new DataSet();
                    StringReader srTransHdr = new StringReader(Convert.ToString(retailTrans.PartnerData.FREEGIFTCON));
                    if (Convert.ToString(retailTrans.PartnerData.FREEGIFTCON).Length > 38)
                    {
                        dsFreeGiftCon.ReadXml(srTransHdr);
                    }

                    string sStorGrp = getStoreFormatCode();
                    string sArticle = "";
                    string sItemProductType = GetItemProductType(saleLine.ItemId, ref sArticle);

                    if (dsFreeGiftCon != null && dsFreeGiftCon.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow row in dsFreeGiftCon.Tables[0].Rows)
                        {
                            sProductType = Convert.ToString(row["PRODUCTTYPE"].ToString());
                            sAmt = Convert.ToString(row["AMOUNT"].ToString());
                            string sFreeArticleCode = string.Empty;
                            string sFreeProductType = string.Empty;

                            GetFreeGiftInfo(Convert.ToDecimal(sAmt), sProductType, sStorGrp,
                                ref dCWQTY, ref dQTY, ref  sPromoCode, ref  sItemId,
                                ref sConf, ref sColor, ref sSize, ref sFreeProductType, ref sFreeArticleCode);


                            if (!string.IsNullOrEmpty(sItemId))
                            {
                                if (saleLine.ItemId == sItemId)
                                {
                                    break;
                                }
                            }
                            else
                            {
                                if (IsValidFreeGiftItem(saleLine, sFreeProductType, sFreeArticleCode, sConf, sSize, sColor))
                                {
                                    break;
                                }
                                else
                                {
                                    retailTrans.PartnerData.FreeGiftQTY = "";
                                    retailTrans.PartnerData.FreeGiftCWQTY = "";
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            frmCustomFieldCalculations
                oCustomCalc = new frmCustomFieldCalculations(connection,
                                                             saleLine.ItemId,
                                                             ApplicationSettings.Database.StoreID,
                                                             string.IsNullOrEmpty(Convert.ToString(saleLine.Dimension.ConfigId)) ? "" : Convert.ToString(saleLine.Dimension.ConfigId),
                                                             string.IsNullOrEmpty(Convert.ToString(saleLine.Dimension.SizeId)) ? "" : Convert.ToString(saleLine.Dimension.SizeId),
                                                             string.IsNullOrEmpty(Convert.ToString(saleLine.Dimension.ColorId)) ? "" : Convert.ToString(saleLine.Dimension.ColorId),
                                                             string.IsNullOrEmpty(Convert.ToString(saleLine.Dimension.StyleId)) ? "" : Convert.ToString(saleLine.Dimension.StyleId),
                                                             al, sMRPRate, isMRPExists, posTransaction, saleLine.BackofficeSalesOrderUnitOfMeasure, dQTY, dCWQTY, sPromoCode);

            bool zeropricevalid = true;
            if (oCustomCalc.isCancelClick)
            {
                string query = string.Empty;
                // query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' ";

                query = query + " UPDATE SKUTableTrans SET isLocked='False' " +
                                   " WHERE SkuNumber='" + saleLine.ItemId + "' and isavailable=1" +
                                   " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' ";
                isItemAvailableORReturn(query);

                return false;
            }

            oCustomCalc.ShowDialog();

            if (oCustomCalc.isCancelClick)
            {
                string query = string.Empty;
                //query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                query = query + " UPDATE SKUTableTrans SET isLocked='False' " +
                               " WHERE SkuNumber='" + saleLine.ItemId + "' and isavailable=1 " +
                               " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "' ";
                isItemAvailableORReturn(query);

                return false;
            }

            List<KeyValuePair<string, string>> list = new List<KeyValuePair<string, string>>();
            if (Convert.ToInt16(oCustomCalc.RadioChecked) == (int)Enums.EnumClass.TransactionType.Sale)
                list = oCustomCalc.saleList;
            else
                list = oCustomCalc.purchaseList;

            #region Bulk Item Trans
            if (saleLine.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                && saleLine.PartnerData.BulkItem == 1)
            {

                #region
                if (saleLine.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                {
                    GetItemType(saleLineItem.ItemId);
                    if (iOWNDMD == 0 && iOWNOG == 0 && iOTHERDMD == 0 && iOTHEROG == 0)
                    {
                        if (IsBulkItem(saleLineItem.ItemId))
                        {
                            saleLine.PartnerData.BulkItem = 1;
                            if (IsBatchItem(saleLineItem.ItemId))
                            {
                                //frmSalesInfoCode objBatch = new frmSalesInfoCode(true);
                                //objBatch.ShowDialog();
                                //saleLine.PartnerData.BULKINVENTBATCHID = objBatch.sCodeOrRemarks;
                                if (list != null)//added this line only on 241117
                                    GetValidBulkItemPcsAndQtyForTrans(ref dBulkPdsQty, ref dBulkQty, saleLine, list[66].Value);
                            }
                            else
                            {
                                GetValidBulkItemPcsAndQtyForTrans(ref dBulkPdsQty, ref dBulkQty, saleLine, "");
                            }
                        }
                    }

                }
                #endregion


                if (IsCatchWtItem(saleLineItem.ItemId))
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        if (list != null)
                        {
                            if ((Convert.ToDecimal(list[0].Value) == dBulkPdsQty && Convert.ToDecimal(list[1].Value) != dBulkQty)
                                || (Convert.ToDecimal(list[0].Value) != dBulkPdsQty && Convert.ToDecimal(list[1].Value) == dBulkQty))
                            {
                                if (retailTrans.PartnerData.SaleAdjustment)
                                {
                                    retailTrans.PartnerData.TransAdjGoldQty = Convert.ToDecimal(retailTrans.PartnerData.TransAdjGoldQty) - Convert.ToDecimal(retailTrans.PartnerData.RunningQtyAdjustment);
                                    MessageBox.Show("Please check your stock.");
                                    oCustomCalc.ShowDialog();
                                }
                                if (oCustomCalc.isCancelClick)
                                {
                                    string query = string.Empty;
                                    //query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                                    query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                                    isItemAvailableORReturn(query);

                                    return false;
                                }


                                if (Convert.ToInt16(oCustomCalc.RadioChecked) == (int)Enums.EnumClass.TransactionType.Sale)
                                    list = oCustomCalc.saleList;
                                else
                                    list = oCustomCalc.purchaseList;
                            }
                            else
                                break;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < 1000; i++)
                    {
                        if (list != null)
                        {
                            if (Convert.ToDecimal(list[1].Value) > dBulkQty)
                            {
                                if (retailTrans.PartnerData.SaleAdjustment)
                                {
                                    retailTrans.PartnerData.TransAdjGoldQty = Convert.ToDecimal(retailTrans.PartnerData.TransAdjGoldQty) - Convert.ToDecimal(retailTrans.PartnerData.RunningQtyAdjustment);
                                    MessageBox.Show("Please check your stock.");
                                    oCustomCalc.ShowDialog();
                                }
                                if (oCustomCalc.isCancelClick)
                                {
                                    string query = string.Empty;
                                    //query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                                    query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                                    isItemAvailableORReturn(query);

                                    return false;
                                }
                                if (Convert.ToInt16(oCustomCalc.RadioChecked) == (int)Enums.EnumClass.TransactionType.Sale)
                                    list = oCustomCalc.saleList;
                                else
                                    list = oCustomCalc.purchaseList;
                            }
                            else
                                break;
                        }
                        else
                        {
                            return false;
                        }
                    }

                }
            }
            #endregion

            if (IsGiftItem(saleLine.ItemId) && saleLine.ZeroPriceValid == false)// req by Bapi da on 14/08/2015
            {
                MessageBox.Show("Please check zero price valid or not");
                return false;
            }

            zeropricevalid = saleLine.ZeroPriceValid;
            string amount = Convert.ToString(list[9].Value);
            if (!zeropricevalid && !string.IsNullOrEmpty(amount) && Convert.ToDecimal(amount) == 0)
            {
                return true;
            }
            updateOrderDelivery(null, null, false, saleLineItem, saleLine.ItemId);
            GetSKUExtraInfo(saleLine.ItemId);

            #region SaleLine PartnerData Object
            saleLine.PartnerData.Pieces = list[0].Value;
            saleLine.PartnerData.Quantity = list[1].Value;
            saleLine.PartnerData.Rate = list[2].Value;
            saleLine.PartnerData.RateType = list[3].Value;
            saleLine.PartnerData.MakingRate = list[4].Value;
            saleLine.PartnerData.MakingType = list[5].Value;
            saleLine.PartnerData.Amount = list[6].Value;
            saleLine.PartnerData.MakingDisc = list[7].Value;
            saleLine.PartnerData.MakingAmount = list[8].Value;
            saleLine.PartnerData.TotalAmount = list[9].Value;
            saleLine.PartnerData.TotalWeight = list[10].Value;
            saleLine.PartnerData.LossPct = list[11].Value;
            saleLine.PartnerData.LossWeight = list[12].Value;
            saleLine.PartnerData.ExpectedQuantity = list[13].Value;
            saleLine.PartnerData.TransactionType = oCustomCalc.RadioChecked.ToUpper().Trim();
            saleLine.PartnerData.OChecked = list[14].Value;

            if (oCustomCalc.dtIngredientsClone != null && oCustomCalc.dtIngredientsClone.Rows.Count > 0)
            {
                oCustomCalc.dtIngredientsClone.TableName = "Ingredients";

                MemoryStream mstr = new MemoryStream();
                oCustomCalc.dtIngredientsClone.WriteXml(mstr, true);
                mstr.Seek(0, SeekOrigin.Begin);
                StreamReader sr = new StreamReader(mstr);
                string sXML = string.Empty;
                sXML = sr.ReadToEnd();
                saleLine.PartnerData.Ingredients = sXML;
                //   oCustomCalc.dtIngredientsClone.WriteXml(saleLine.ItemId + "-" + saleLine.LineId);
            }
            else
            {
                saleLine.PartnerData.Ingredients = string.Empty;
            }


            saleLine.PartnerData.OrderNum = list[15].Value;
            saleLine.PartnerData.OrderLineNum = list[16].Value;
            saleLine.PartnerData.CustNo = Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId) == null ? string.Empty : Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId);
            saleLine.PartnerData.SampleReturn = string.IsNullOrEmpty(Convert.ToString(list[17].Value)) ? "0" : (Convert.ToBoolean(list[17].Value) ? Convert.ToString("1") : Convert.ToString("0"));
            saleLine.PartnerData.WastageType = list[18].Value;
            saleLine.PartnerData.WastageQty = list[19].Value;
            saleLine.PartnerData.WastageAmount = list[20].Value;
            saleLine.PartnerData.WastagePercentage = list[21].Value;
            saleLine.PartnerData.WastageRate = list[22].Value;
            saleLine.PartnerData.MakingDiscountType = list[23].Value;
            saleLine.PartnerData.MakingTotalDiscount = list[24].Value;
            saleLine.PartnerData.ConfigId = list[25].Value;

            saleLine.PartnerData.Purity = list[26].Value;
            saleLine.PartnerData.GROSSWT = list[27].Value;
            saleLine.PartnerData.GROSSUNIT = list[28].Value;
            saleLine.PartnerData.DMDWT = list[29].Value;
            saleLine.PartnerData.DMDPCS = list[30].Value;
            saleLine.PartnerData.DMDUNIT = list[31].Value;
            saleLine.PartnerData.DMDAMOUNT = list[32].Value;
            saleLine.PartnerData.STONEWT = list[33].Value;
            saleLine.PartnerData.STONEPCS = list[34].Value;
            saleLine.PartnerData.STONEUNIT = list[35].Value;
            saleLine.PartnerData.STONEAMOUNT = list[36].Value;
            saleLine.PartnerData.NETWT = list[37].Value;
            saleLine.PartnerData.NETRATE = list[38].Value;
            saleLine.PartnerData.NETUNIT = list[39].Value;
            saleLine.PartnerData.NETPURITY = list[40].Value;
            saleLine.PartnerData.NETAMOUNT = list[41].Value;
            saleLine.PartnerData.OGREFINVOICENO = list[42].Value;

            //start : added on req of R.Nandy 151114
            saleLine.PartnerData.ItemIdParent = sItemIdParent;
            saleLine.PartnerData.GroupCostPrice = dGroupCostPrice;
            saleLine.PartnerData.UpdatedCostPrice = dUpdatedCostPrice;
            saleLine.PartnerData.SellingCostPrice = dSellingCostPrice;
            //end  : 151114
            saleLine.PartnerData.FLAT = list[43].Value;
            saleLine.PartnerData.PROMOCODE = list[44].Value;


            //saleLine.PartnerData.isMRP = false;
            saleLine.PartnerData.IsSpecialDisc = false;
            saleLine.PartnerData.SpecialDiscInfo = "";
            saleLine.PartnerData.RETAILBATCHNO = list[45].Value;

            saleLine.PartnerData.ACTMKRATE = list[46].Value;
            saleLine.PartnerData.ACTTOTAMT = list[47].Value;
            saleLine.PartnerData.CHANGEDTOTAMT = list[48].Value;
            saleLine.PartnerData.LINEDISC = list[49].Value;

            // OG NEW FLD
            saleLine.PartnerData.OGREFBATCHNO = list[50].Value;
            saleLine.PartnerData.OGCHANGEDGROSSWT = list[51].Value;
            saleLine.PartnerData.OGDMDRATE = list[52].Value;
            saleLine.PartnerData.OGSTONEATE = list[53].Value;
            saleLine.PartnerData.OGGROSSAMT = list[54].Value;
            saleLine.PartnerData.OGACTAMT = list[55].Value;
            saleLine.PartnerData.OGCHANGEDAMT = list[56].Value;
            saleLine.PartnerData.OGFINALAMT = list[57].Value;
            saleLine.PartnerData.FGPROMOCODE = list[58].Value;// for Malabar // added on30/08/16

            saleLine.PartnerData.ISREPAIR = list[59].Value;
            saleLine.PartnerData.REPAIRBATCHID = list[60].Value;
            saleLine.PartnerData.REPAIRINVENTBATCHID = list[61].Value;
            saleLine.PartnerData.RATESHOW = list[62].Value;
            saleLine.PartnerData.isFullReturn = false;
            saleLine.PartnerData.TRANSFERCOSTPRICE = list[63].Value;

            saleLine.PartnerData.OGLINENUM = list[64].Value;
            saleLine.PartnerData.OGRECEIPTNO = list[65].Value;
            saleLine.PartnerData.BULKINVENTBATCHID = list[66].Value;
            saleLine.PartnerData.LINEGOLDVALUE = list[67].Value;
            saleLine.PartnerData.LINEGOLDTAX = list[68].Value;
            saleLine.PartnerData.WTDIFFDISCQTY = list[69].Value;

            saleLine.PartnerData.PurityReading1 = list[70].Value;
            saleLine.PartnerData.PurityReading2 = list[71].Value;
            saleLine.PartnerData.PurityReading3 = list[72].Value;
            saleLine.PartnerData.PURITYPERSON = list[73].Value;
            saleLine.PartnerData.PURITYPERSONNAME = list[74].Value;

            saleLine.PartnerData.SKUAgingDiscType = 0;
            saleLine.PartnerData.SKUAgingDiscAmt = 0;
            saleLine.PartnerData.TierDiscType = 0;
            saleLine.PartnerData.TierDiscAmt = 0;

            saleLine.PartnerData.LINEOMVALUE = list[75].Value;
            saleLine.PartnerData.WTDIFFDISCAMT = list[76].Value;

            saleLine.PartnerData.OGPSTONEWT = list[77].Value;
            saleLine.PartnerData.OGPSTONEPCS = list[78].Value;
            saleLine.PartnerData.OGPSTONEUNIT = list[79].Value;
            saleLine.PartnerData.OGPSTONERATE = list[80].Value;
            saleLine.PartnerData.OGPSTONEAMOUNT = list[81].Value;

            saleLine.PartnerData.MakStnDiaDiscType = 0;
            saleLine.PartnerData.NimMakingDiscount = 0;
            saleLine.PartnerData.NimStoneDiscount = 0;
            saleLine.PartnerData.NimDiamondDiscount = 0;

            //added on150318
            /*if (saleLine.PartnerData.TransactionType == "1")
            {
                saleLine.SalesTaxGroupId = "";
                saleLine.SalesTaxGroupIdOriginal = "";
                saleLine.TaxGroupId = "";
                saleLine.TaxGroupIdOriginal = "";
            }*/
            // blocked by S.Sharma on 10/04/18 for posting purpose


            #endregion

            RetailTransaction retailTransaction = posTransaction as RetailTransaction;
            if (saleLineItem != null)
            {
                #region for Free gift validation
                if (!string.IsNullOrEmpty(retailTrans.PartnerData.FREEGIFTCON))
                {
                    SaleLineItem saleLine1 = (SaleLineItem)saleLineItem;
                    decimal dTotFreeQtyApplied = 0m;
                    bool bFullFreeGiftApplid = false;

                    foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
                    {
                        if (!saleLineItem1.Voided)
                        {
                            if (saleLineItem1.NetAmount == 0)
                                dTotFreeQtyApplied += decimal.Round((Convert.ToDecimal(saleLineItem1.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);
                        }
                    }

                    dTotFreeQtyApplied = dTotFreeQtyApplied + decimal.Round((Convert.ToDecimal(saleLine.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);

                    if (!string.IsNullOrEmpty(retailTrans.PartnerData.FreeGiftQTY))
                    {
                        if (dTotFreeQtyApplied > Convert.ToDecimal(retailTrans.PartnerData.FreeGiftQTY))
                            bFullFreeGiftApplid = false;
                        else
                            bFullFreeGiftApplid = true;
                    }

                    if (!bFullFreeGiftApplid)
                    {
                        MessageBox.Show("Free quantity exceed, alloted " + Convert.ToString(retailTrans.PartnerData.FreeGiftQTY) + "");
                        string query = " UPDATE SKUTABLETRANS SET ISLOCKED='False' WHERE  SKUNUMBER = '" + saleLine.ItemId + "' AND DATAAREAID='" + application.Settings.Database.DataAreaID + "'";
                        isItemAvailableORReturn(query);
                        return false;
                    }
                }
                #endregion

                #region Auto Fetch Sales man
                for (int i = 0; i < 1000; i++)// for malabar dubai changes only one sales persons will be added on transaction //20/06/2016
                {
                    if (!string.IsNullOrEmpty(saleLineItem.SalesPersonId))
                        break;
                    else
                    {
                        if (retailTransaction != null)
                        {
                            /*if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.OrderLineNum)))
                            {
                                int iLineNum = 0;
                                iLineNum = Convert.ToInt16(saleLineItem.PartnerData.OrderLineNum);

                                if (iLineNum == 0)
                                {
                                    LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                    dialog6.ShowDialog();
                                    if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                    {
                                        saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                        saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                dialog6.ShowDialog();
                                if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                {
                                    saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                    saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                    break;
                                }
                            }*/

                            LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                            dialog6.ShowDialog();
                            if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                            {
                                saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                break;
                            }

                        }

                    }

                }
                #endregion

                string sSCType = "";// GetSaLsChannelType();// commented on 13/10/15 req by s.giri

                saleLine.PartnerData.SALESCHANNELTYPE = sSCType;

                if (!string.IsNullOrEmpty(Convert.ToString(saleLine.PartnerData.OrderNum)) && !string.IsNullOrEmpty(Convert.ToString(saleLine.PartnerData.OrderLineNum)))
                {
                    updateOrderDelivery(Convert.ToString(saleLine.PartnerData.OrderNum), Convert.ToString(saleLine.PartnerData.OrderLineNum), false, saleLineItem);
                }
            }

            return true;
        }

        #region Changed By Nimbus
        private void isItemAvailableORReturn(string query)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(query, connection);
            command.CommandTimeout = 0;
            command.ExecuteNonQuery();

        }
        #endregion

        private bool IsValidFreeGiftItem(SaleLineItem saleLine, string sProdId, string sArticle, string sConfig, string sSize, string sColor)
        {
            bool bResult = false;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " SELECT isnull(RETAILVARIANTID,'') FROM INVENTDIMCOMBINATION WHERE ITEMID='" + saleLine.ItemId + "'AND INVENTDIMID in(" +
                                " select INVENTDIMID from INVENTDIM where CONFIGID='" + sConfig + "'" +
                                " and INVENTSIZEID='" + sSize + "' and INVENTCOLORID='" + sColor + "' AND INVENTSITEID='' AND INVENTLOCATIONID=''" +
                                " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "') and itemid in(select itemid from INVENTTABLE" +
                                " where PRODUCTTYPECODE='" + sProdId + "' and ARTICLE_CODE='" + sArticle + "'" +
                                " and DATAAREAID='" + application.Settings.Database.DataAreaID + "')";

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            string sRETAILVARIANTID = Convert.ToString(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (saleLine.Dimension.ConfigId != sConfig)
            {
                MessageBox.Show("Invalid config selected.");

                bResult = false;
            }
            else if (saleLine.Dimension.SizeId != sSize)
            {
                MessageBox.Show("Invalid size selected.");
                return false;
            }
            else if (saleLine.Dimension.ColorId != sColor)
            {
                MessageBox.Show("Invalid color selected.");
                return false;
            }
            else
            {
                if (!string.IsNullOrEmpty(sRETAILVARIANTID))
                    bResult = true;
                else
                    bResult = false;
            }

            return bResult;
        }
        #endregion

        private string GetSaLsChannelType()
        {
            string sSCType = string.Empty;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;

            commandText = "SELECT CHANNELTYPE,[DESCRIPTION] FROM  [DBO].[CRWSALESCHANNEL] ORDER BY DEFAULTVALUE DESC";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand cmd = new SqlCommand(commandText, connection);
            SqlDataAdapter daSC = new SqlDataAdapter(cmd);
            DataTable dtSC = new DataTable();
            daSC.Fill(dtSC);
            if (dtSC != null && dtSC.Rows.Count > 0)
            {
                BlankOperations.WinFormsTouch.frmGeneralSearch oSearch = new BlankOperations.WinFormsTouch.frmGeneralSearch(dtSC);
                oSearch.ShowDialog();

                sSCType = oSearch.SelectedChannel;
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No record found.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
            return sSCType;
        }

        #region GET SKU DATA FOR RELEASING
        private void GetSKUExtraInfo(string sSKUNo)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "SELECT ITEMIDPARENT,GROUPCOSTPRICE,UPDATEDCOSTPRICE,SELLINGPRICE FROM SKUTable_Posted WHERE SKUNUMBER='" + sSKUNo + "'";

                SqlCommand command = new SqlCommand(SqlComm.CommandText.ToString(), SqlCon);
                command.CommandTimeout = 0;
                SqlDataReader reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sItemIdParent = Convert.ToString(reader.GetValue(0));
                        dGroupCostPrice = Convert.ToDecimal(reader.GetValue(1));
                        dUpdatedCostPrice = Convert.ToDecimal(reader.GetValue(2));
                        dSellingCostPrice = Convert.ToDecimal(reader.GetValue(3));
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        /// <summary>
        /// GIFT =1 in inventtable 
        /// Dev on  : 14/08/2015 by : RHossain
        /// </summary>
        /// <param name="sItemId"></param>
        /// <returns></returns>
        private bool IsGiftItem(string sItemId)
        {
            bool bGiftItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select GIFT from inventtable WHERE ITEMID = '" + sItemId + "'";


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bGiftItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bGiftItem;
        }

        private bool IsValidAdvance(string sTransId, string sStoreId, string sTerminalId)
        {
            bool bValidAdvance = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = " select TransactionId from RetailAdjustmentTable WHERE TransactionId='" + sTransId + "'" +
                                                   " AND RETAILTERMINALID = '" + sTerminalId + "' and  RETAILSTOREID= '" + sStoreId + "'"; // RETAILTEMPTABLE


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            string sTransactionId = Convert.ToString(command.ExecuteScalar());

            if (!string.IsNullOrEmpty(sTransactionId))
                bValidAdvance = true;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bValidAdvance;
        }

        protected int GetMetalType(string sItemId, SqlConnection connection)
        {
            int iMetalType = 100;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select metaltype from inventtable where itemid='" + sItemId + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    iMetalType = (int)reader.GetValue(0);
                }
            }
            if (connection.State == ConnectionState.Open)
                connection.Close();
            return iMetalType;

        }

        /// <summary>
        /// for Malabar based on this new item type 
        /// sales radio button or rest of the radio  button will be on/off
        /// </summary>
        /// <param name="item"></param>
        /// <param name="conn"></param>
        private void GetItemType(string item, SqlConnection conn)
        {
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = " SELECT TOP(1) OWNDMD,OWNOG,OTHERDMD,OTHEROG FROM INVENTTABLE WHERE ITEMID='" + item + "'";


                SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        iOWNDMD = Convert.ToInt16(reader.GetValue(0));
                        iOWNOG = Convert.ToInt16(reader.GetValue(1));
                        iOTHERDMD = Convert.ToInt16(reader.GetValue(2));
                        iOTHEROG = Convert.ToInt16(reader.GetValue(3));
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private string GetItemProductType(string sSalesItem)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select PRODUCTTYPECODE  from INVENTTABLE  where ITEMID='" + sSalesItem + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
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

        private void GetFreeGiftInfo(decimal dAmt, string sProductType, string sStoreGrp, ref decimal dCWQTY,
            ref decimal dQTY, ref string sPCode, ref string sItemID, ref string sConf
            , ref string sColor, ref string sSize,
            ref string sFreeProductType,
            ref string sFreeArticleCode)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " SELECT  TOP (1) CAST(ISNULL(PDSCWQTY,0) AS DECIMAL(28,0)) PDSCWQTY," +
                            " CAST(ISNULL(QTY,0) AS DECIMAL(28,3)) QTY,ISNULL(PROMOTIONCODE,'') PROMOTIONCODE,ISNULL(ITEMID,'') ITEMID, " +
                            " isnull(CONFIGID,'') CONFIGID,isnull(INVENTSIZEID,'') INVENTSIZEID,isnull(INVENTCOLORID,'') INVENTCOLORID, " +
                            " isnull(FreeArticleCode,'') FreeArticleCode,isnull(FreeProductType,'') FreeProductType " +
                            " FROM   CRWGIFTITEMPROMOTIONAGREEMENT ag " +
                            " left join INVENTDIM as id on ag.INVENTDIMID = id.INVENTDIMID " +
                            " WHERE (STOREGROUP = CASE WHEN TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN TABLEGROUPALL=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN TABLEGROUPALL=2 THEN '' END)" +
                            " AND (PRODUCTTYPECODE='" + sProductType + "')" +
                            " and (" + dAmt + " BETWEEN FROMAMOUNT AND TOAMOUNT  or " + dAmt + " >= TOAMOUNT)  " +
                            " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN FROMDATE AND TODATE)  " +
                            " AND CRWCONFIRM = 1 AND ag.DATAAREAID ='" + ApplicationSettings.Database.DATAAREAID + "'  " +
                            " ORDER BY ITEMID DESC,PROMOTIONCODE DESC";


            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dCWQTY = Convert.ToDecimal(reader.GetValue(0));
                    dQTY = Convert.ToDecimal(reader.GetValue(1));
                    sPCode = Convert.ToString(reader.GetValue(2));
                    sItemID = Convert.ToString(reader.GetValue(3));
                    sConf = Convert.ToString(reader.GetValue(4));
                    sColor = Convert.ToString(reader.GetValue(5));
                    sSize = Convert.ToString(reader.GetValue(6));
                    sFreeArticleCode = Convert.ToString(reader.GetValue(7));
                    sFreeProductType = Convert.ToString(reader.GetValue(8));
                }
            }
            else
            {
                dCWQTY = 0;
                dQTY = 0;
                sPCode = "";
                sItemID = "";
                sFreeArticleCode = "";
                sFreeProductType = "";
            }
            reader.Close();
            reader.Dispose();
            if (connection.State == ConnectionState.Open)
                connection.Close();

        }

        private bool IsBulkItem(string sItemId)
        {
            bool bBulkItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @ISBULK INT  SET @ISBULK = 0 IF EXISTS (SELECT ITEMID FROM INVENTTABLE WHERE ITEMID = '" + sItemId + "' AND RETAIL=0)" +
                                 " BEGIN SET @ISBULK = 1 END SELECT @ISBULK";


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bBulkItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bBulkItem;
        }

        private void GetValidBulkItemPcsAndQtyForTrans(ref decimal dPdsCWQTY, ref decimal dQty, SaleLineItem salesItem, string sBatch = null)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select sum(PdsCWQty) as PDSCWQTY,sum(Qty) as QTY from BulkItemTransTable ");
            sbQuery.Append(" Where ItemId='" + salesItem.ItemId + "' and InventColorId='" + salesItem.Dimension.ColorId + "'");
            sbQuery.Append(" and ConfigId='" + salesItem.Dimension.ConfigId + "'");
            sbQuery.Append(" and InventSizeId='" + salesItem.Dimension.SizeId + "'");
            sbQuery.Append(" and InventStyleId='" + salesItem.Dimension.StyleId + "'");
            if (!string.IsNullOrEmpty(sBatch))
                sbQuery.Append(" and InventBatchId='" + sBatch + "'");

            sbQuery.Append(" group by ItemId,ConfigId,InventColorId,InventSizeId,InventStyleId,InventBatchId ");

            DataTable dtBI = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataAdapter daGss = new SqlDataAdapter(cmd);
            daGss.Fill(dtBI);

            if (dtBI != null && dtBI.Rows.Count > 0)
            {
                dPdsCWQTY = Convert.ToDecimal(dtBI.Rows[0]["PDSCWQTY"]);
                dQty = Convert.ToDecimal(dtBI.Rows[0]["QTY"]);
            }

        }

        private bool IsCatchWtItem(string sItemId)
        {
            bool bCatchWtItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @ISCATCHWT INT  SET @ISCATCHWT = 0 IF EXISTS (SELECT ITEMID FROM pdscatchweightitem WHERE ITEMID = '" + sItemId + "')" +
                                 " BEGIN SET @ISCATCHWT = 1 END SELECT @ISCATCHWT";


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bCatchWtItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bCatchWtItem;
        }

        private bool IsBatchItem(string sItemId)
        {
            bool bBatchItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select ISACTIVE from ECORESTRACKINGDIMENSIONGROUPITEM a " +
                                 " left join ECORESTRACKINGDIMENSIONGROUPFLDSETUP b on a.TRACKINGDIMENSIONGROUP=b.TRACKINGDIMENSIONGROUP" +
                                 " where ITEMID='" + sItemId + "' and b.ISACTIVE=1 " +
                                 " and b.DIMENSIONFIELDID=2 and a.ITEMDATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bBatchItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bBatchItem;
        }

        /// <summary>
        /// for Malabar based on this new item type sales 
        /// radio button or rest of the radio  button will be on/off
        /// </summary>
        /// <param name="item"></param>
        private void GetItemType(string item)
        {
            SqlConnection connection = new SqlConnection();
            try
            {
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                string query = " SELECT TOP(1) OWNDMD,OWNOG,OTHERDMD,OTHEROG FROM INVENTTABLE WHERE ITEMID='" + item + "'";


                SqlCommand cmd = new SqlCommand(query.ToString(), connection);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        iOWNDMD = Convert.ToInt16(reader.GetValue(0));
                        iOWNOG = Convert.ToInt16(reader.GetValue(1));
                        iOTHERDMD = Convert.ToInt16(reader.GetValue(2));
                        iOTHEROG = Convert.ToInt16(reader.GetValue(3));
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }
        }

        private DataTable GetInHouseRepReturnTotalRow(string sRepairId)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                SqlComm.CommandText = " select ITEMID,CAST(ISNULL(AMOUNT,0)AS DECIMAL(28,2)) AMOUNT" +
                                     " ,CAST(ISNULL(QTY,0)AS DECIMAL(28,3)) QTY  ," +
                                     " CAST(ISNULL(PDSCWQTY,0)AS DECIMAL(28,0)) PDSCWQTY" +
                                    " ,BatchId  from CRWRETAILREPAIRINGREDIENT where REPAIRID='" + sRepairId + "'";

                DataTable dtRepOg = new DataTable();


                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtRepOg);

                return dtRepOg;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private void getMultiAdjOrderNo(string sTableName, ref string sOrder, ref string sCust, ref int iLine)
        {
            SqlConnection connection = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            sbQuery.Append(" BEGIN SELECT ISNULL(ORDERNO,'') AS ORDERNO,ISNULL(CUSTACC,'') AS CUSTACC,isnull(cast(LINENUM as int),0) LINENUM FROM " + sTableName + " END");

            DataTable dtGSS = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sOrder = Convert.ToString(reader.GetValue(0));
                    sCust = Convert.ToString(reader.GetValue(1));
                    iLine = Convert.ToInt16(reader.GetValue(2));
                }
            }
            reader.Close();
            reader.Dispose();
            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        private DataTable BookedSKU(string sSkuNo, string account)
        {
            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "select SKUNUMBER,ORDERNUMBER,LINENUM FROM RETAILCUSTOMERDEPOSITSKUDETAILS WHERE" +
                                      " SKUNUMBER='" + sSkuNo + "' AND CUSTOMERID='" + account + "' AND DELIVERED=0 AND RELEASED = 0" +
                                      " union " +
                                      " select SKUNUMBER,CUSTORDERNUM ORDERNUMBER,CUSTORDERLINENUM LINENUM from SKUTable_Posted where CUSTORDERNUM !='' AND SKUNUMBER='" + sSkuNo + "'";

                DataTable CustBalDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustBalDt);

                return CustBalDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool isBookedSKUWithDiffCustomer(string sSkuNo, string account, string sOrdNo, ref string sCustOrd, ref string sOrdCustAcc)
        {
            bool bResult = false;
            string sSKU = "";
            string sCust = "";
            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                //string sSQl = "select top 1 SKUNUMBER,CUSTOMERID,ORDERNUMBER FROM RETAILCUSTOMERDEPOSITSKUDETAILS WHERE" +
                //              " SKUNUMBER='" + sSkuNo + "' AND DELIVERED=0 AND RELEASED = 0 order by REPLICATIONCOUNTER desc"+
                //              " union " +
                //              " SELECT SP.SKUNUMBER SKUNUMBER, CH.CUSTACCOUNT CUSTOMERID ,SP.CUSTORDERNUM ORDERNUMBER " +
                //              " FROM SKUTable_Posted SP "+
                //              " LEFT join CUSTORDER_HEADER CH on SP.CUSTORDERNUM= CH.ORDERNUM where SKUNUMBER !='" + sSkuNo + "'";


                string sSQl = " select SKUNUMBER,CUSTOMERID,ORDERNUMBER from" +
                                " (select top 1 SKUNUMBER,CUSTOMERID,ORDERNUMBER FROM RETAILCUSTOMERDEPOSITSKUDETAILS " +
                                " WHERE SKUNUMBER='" + sSkuNo + "'" +
                                " AND DELIVERED=0 AND RELEASED = 0   order by REPLICATIONCOUNTER desc) A" +
                                " union" +
                                " select SKUNUMBER,CUSTOMERID,ORDERNUMBER from " +
                                " (SELECT SP.SKUNUMBER SKUNUMBER, CH.CUSTACCOUNT CUSTOMERID ,SP.CUSTORDERNUM ORDERNUMBER  " +
                                " FROM SKUTable_Posted SP  LEFT join CUSTORDER_HEADER CH on SP.CUSTORDERNUM= CH.ORDERNUM" +
                                " left join SKUTableTrans ST on SP.SkuNumber=ST.SkuNumber " +
                                " where SP.SKUNUMBER ='" + sSkuNo + "' and ST.isAvailable=0) B";

                SqlCommand cmd = new SqlCommand(sSQl, SqlCon);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sSKU = Convert.ToString(reader.GetValue(0));
                        sCust = Convert.ToString(reader.GetValue(1));
                        sOrdCustAcc = sCust;
                        sCustOrd = Convert.ToString(reader.GetValue(2));
                    }
                }
                reader.Close();
                reader.Dispose();
                if (SqlCon.State == ConnectionState.Open)
                    SqlCon.Close();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            if (!string.IsNullOrEmpty(sCust))
            {
                if (sCust != account)
                    bResult = true;

                if (sOrdNo != sCustOrd)
                    bResult = true;

            }

            return bResult;
        }

        private string GetItemProductType(string sSalesItem, ref string sArticleCode)
        {
            string sProductType = "";
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select PRODUCTTYPECODE,ARTICLE_CODE  from INVENTTABLE  where ITEMID='" + sSalesItem + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sProductType = Convert.ToString(reader.GetValue(0));
                    sArticleCode = Convert.ToString(reader.GetValue(1));
                }
            }
            else
            {
                sProductType = "";
                sArticleCode = "";
            }
            reader.Close();
            reader.Dispose();

            return sProductType;

            if (conn.State == ConnectionState.Open)
                conn.Close();
        }

        private bool IsGSSEmiInclOfTax()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" select isnull(GSSEmiInclOfTax,0) from RETAILPARAMETERS where  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            bool sResult = Convert.ToBoolean(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sResult;

        }

        private int getMetalType(string sItemId)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT ISNULL(METALTYPE,0) FROM INVENTTABLE WHERE ITEMID = '" + sItemId + "' AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "'  ");

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            int iMetalType = Convert.ToInt32(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return iMetalType;

        }

        private bool isServiceItem(IPosTransaction transaction, int lineid, ISaleLineItem saleLineItem, string operation)
        {
            if (lineid == 0)
                return false;

            bool isServiceItemExists = false;
            SaleLineItem saleitem = saleLineItem as SaleLineItem;
            if (operation.ToUpper().Trim() != "QTY")
            {
                saleitem = null;
            }
            if (saleitem == null)
            {
                System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(transaction)).SaleItems);
                foreach (var sale in saleline)
                {
                    if (sale.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && !sale.Voided)
                    {
                        if (sale.LineId == lineid)
                        {
                            isServiceItemExists = true;
                            break;
                        }
                    }
                }

            }
            return isServiceItemExists;
        }

        private decimal getMetalRate(string sItemId, string sConfigId)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + sItemId.Trim() + "' ");

            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RATES,0) AS decimal (6,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");

            commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");


            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + sConfigId.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");
            commandText.Append(" END ");

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            if (!string.IsNullOrEmpty(sResult))
                return Convert.ToDecimal(sResult.Trim());
            else
                return 0;
        }

        private decimal getRateFromMetalTable(string sItemId,
            string sCustomerId, string ConfigID, string SizeID, string ColorID, decimal dStoneWtRange)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) DECLARE @CUSTCLASSCODE VARCHAR(20)");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + sItemId.Trim() + "' ");
            //added on 09/02/16
            commandText.Append(" SELECT @CUSTCLASSCODE = CUSTCLASSIFICATIONID FROM [CUSTTABLE] WHERE ACCOUNTNUM = '" + sCustomerId + "' ");

            commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(28, 2))   ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION or RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE='') AND (  ");
            commandText.Append(dStoneWtRange);
            commandText.Append(" BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + sItemId.Trim() + "')  AND  ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID + "') ");
            commandText.Append(" AND(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.AccountNum='" + sCustomerId.Trim() + "' ");
            commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.AccountNum='') AND ");
            commandText.Append(" (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CustClassificationId=@CUSTCLASSCODE ");
            commandText.Append(" OR RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CustClassificationId='')");
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // added on 02.09.2013
            commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID DESC, RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");  // Changed order sequence on 03.06.2013 as per u.das
            commandText.Append("  ");//END

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (string.IsNullOrEmpty(sResult))
                sResult = "0";

            return Convert.ToDecimal(sResult.Trim());

        }
        #endregion
        //ENd:Nim

        #region Constructor - Destructor

        public ItemTriggers()
        {

            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for ItemTriggers are reserved at 50350 - 50399
        }

        #endregion

        #region IItemTriggersV1 Members

        public void PreSale(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PreSale", "Prior to the sale of an item...", LSRetailPosis.LogTraceLevel.Trace);

            //Satrt: Nim
            #region - Changed By Nimbus
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans.TransactionType == PosTransaction.TypeOfTransaction.Sales && string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
            {
                MessageBox.Show("Please select a customer.");
                preTriggerResult.ContinueOperation = false;
                return;
            }


            //Start: For Singapore only added on 070318
            if (retailTrans.TransactionType == PosTransaction.TypeOfTransaction.Sales)
            {
                if (retailTrans.PartnerData.SingaporeTaxCal == "1")
                {
                    MessageBox.Show("Item can not be added once recalculation of tax is done.");
                    preTriggerResult.ContinueOperation = false;
                    return;
                }

                if (retailTrans.PartnerData.APPLYGSSDISCDONE == true)
                {
                    MessageBox.Show("Item can not be added once GSS discount is applied");
                    preTriggerResult.ContinueOperation = false;
                    return;
                }

                if (retailTrans.PartnerData.APPLYGMADISCDONE == true)
                {
                    MessageBox.Show("Item can not be added once GMA discount is applied");
                    preTriggerResult.ContinueOperation = false;
                    return;
                }
            }
            //End: For Singapore only

            //if(retailTrans.SaleIsReturnSale)
            //{
            //    preTriggerResult.ContinueOperation = false;
            //    preTriggerResult.MessageId = 999998;
            //    return;
            //}

            //retailTrans.PartnerData.ItemIsReturnItem = false;
            System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> lineitemoriginal = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(retailTrans)).SaleItems);

            if (lineitemoriginal != null)
            {
                foreach (var item in lineitemoriginal)
                {
                    if (item.PartnerData.isFullReturn == true)
                    {
                        MessageBox.Show("Extra line can not be added for this transaction");
                        preTriggerResult.ContinueOperation = false;
                        return;
                    }
                    //if (item.PartnerData.isFullReturn == false && retailTrans.SaleIsReturnSale == true)//added on 17/06/19 req Supapta/Neerja for Apollo
                    //{
                    //    MessageBox.Show("Extra line can not be added for this transaction");
                    //    preTriggerResult.ContinueOperation = false;
                    //    return;
                    //}
                }
            }


            SaleLineItem saleLine = (SaleLineItem)saleLineItem;
            saleLine.PartnerData.NewTaxPerc = 0;
            saleLine.PartnerData.NewTaxAmt = 0;
            saleLine.PartnerData.BulkItem = 0;
            saleLine.PartnerData.BULKINVENTBATCHID = string.Empty;
            saleLine.PartnerData.ReturnReceiptId = "";
            saleLine.PartnerData.TransReturnPayMode = 0;
            saleLine.PartnerData.NimReturnLine = 0;
            saleLine.PartnerData.AdvAgainst = 0;

            saleLine.PartnerData.LOYALTYTYPECODE = "";
            saleLine.PartnerData.LoyaltyProvider = "";
            saleLine.PartnerData.LOYALTYCARDNO = "";
            saleLine.PartnerData.LOYALTYASKINGDONE = false;
            saleLine.PartnerData.CalExchangeQty = 0;//added on 190118
            saleLine.PartnerData.TransReturnPayMode = 0;
            saleLine.PartnerData.SKUAgingDiscType = 0;
            saleLine.PartnerData.SKUAgingDiscAmt = 0;
            saleLine.PartnerData.TierDiscType = 0;
            saleLine.PartnerData.TierDiscAmt = 0;

            saleLine.PartnerData.OGFINALAMT = 0;

            saleLine.PartnerData.NimDiscLine = 0;

            saleLine.PartnerData.NimMRPDiscType = false;
            saleLine.PartnerData.NimMRPDisc = 0;
            saleLine.PartnerData.NimMRPDiscTypeDone = false;
            saleLine.PartnerData.NimMRPDiscClubbed = false;

            saleLine.PartnerData.NimMakingDiscType = false;
            saleLine.PartnerData.NimMakingDiscTypeDone = false;
            saleLine.PartnerData.NimMakingDisc = 0;
            saleLine.PartnerData.NimMakingDiscClubbed = false;

            saleLine.PartnerData.isFullReturn = false;

            saleLine.PartnerData.NimPromoMRPDiscType = false;
            saleLine.PartnerData.NimPromoMRPDisc = 0;
            saleLine.PartnerData.NimPromoMRPDiscTypeDone = false;
            saleLine.PartnerData.NimPromoMRPDiscClubbed = false;

            saleLine.PartnerData.NIMPROMOCODE = "";
            saleLine.PartnerData.NimPromoMakingDiscType = false;
            saleLine.PartnerData.NimPromoMakingDiscTypeDone = false;
            saleLine.PartnerData.NimPromoMakingDisc = 0;
            saleLine.PartnerData.NimPromoMakingDiscClubbed = false;
            saleLine.PartnerData.OpeningDisc = 0;
            saleLine.PartnerData.OpeningDiscType = 0;

            saleLine.PartnerData.NimMakDisc = 0;
            saleLine.PartnerData.NimStnDisc = 0;
            saleLine.PartnerData.NimDiaDisc = 0;
            saleLine.PartnerData.MakStnDiaDiscType = 0;
            saleLine.PartnerData.NimMakingDiscount = 0;
            saleLine.PartnerData.NimStoneDiscount = 0;
            saleLine.PartnerData.NimDiamondDiscount = 0;

            saleLine.PartnerData.MakStnDiaDisc = false;
            saleLine.PartnerData.MakStnDiaDiscDone = false;

            saleLine.PartnerData.AdvAgreemetDisc = false;
            saleLine.PartnerData.AdvAgreemetDiscDone = false;
            saleLine.PartnerData.SaleAdjustmentAdvanceDepositDate = "";

            #region sales 1
            if (retailTrans.TransactionType == PosTransaction.TypeOfTransaction.Sales)
            {
                if (saleLineItem != null)
                {
                    if (IsBulkItem(saleLineItem.ItemId))
                    {
                        saleLine.PartnerData.BulkItem = 1;
                    }
                    else
                    {
                        string sOrdNo = "";
                        DataTable dtBookedSKU = BookedSKU(saleLineItem.ItemId, Convert.ToString(retailTrans.Customer.CustomerId));
                        if (dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
                        {
                            retailTrans.PartnerData.SKUBookedItems = true;
                            retailTrans.PartnerData.SKUBookedItemsExists = "Y";

                            sOrdNo = Convert.ToString(retailTrans.PartnerData.AdjustmentOrderNum);

                            //foreach (DataRow dr in dtBookedSKU.Select("SKUNUMBER = '" + saleLineItem.ItemId + "'"))
                            //{
                            //    //sOrdNo = Convert.ToString(dr["ORDERNUMBER"]);
                            //   // iLine = Convert.ToInt16(dr["LINENUM"]);
                            //}
                        }
                        string sCustOrd = "";
                        string sOrdCustAcc = "";
                        bool isBooked = isBookedSKUWithDiffCustomer(saleLineItem.ItemId, Convert.ToString(retailTrans.Customer.CustomerId), sOrdNo, ref sCustOrd, ref sOrdCustAcc);

                        if (isBooked) // || string.IsNullOrEmpty(sCustOrd)
                        {
                            MessageBox.Show("Item is booked with another customer order " + sCustOrd + " And Customer Acc is " + sOrdCustAcc + "");
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }
                    }

                    if (retailTrans.PartnerData.IsRepairReturn == true)
                    {
                        string sRepId = retailTrans.PartnerData.RefRepairId;
                        DataTable dtNoOfRow = GetInHouseRepReturnTotalRow(sRepId);
                        decimal sQty = 0m;
                        string sBatchId = string.Empty;
                        decimal sAmt = 0m;

                        if (dtNoOfRow != null && dtNoOfRow.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtNoOfRow.Select("ITEMID = '" + saleLineItem.ItemId + "'"))
                            {
                                #region Item Param
                                sQty = Convert.ToDecimal(dr["QTY"]);
                                sBatchId = Convert.ToString(dr["BatchId"]);
                                decimal dMetalCurrentRate = 0m;

                                int iMetalType = getMetalType(saleLineItem.ItemId);
                                if (iMetalType == (int)Enums.EnumClass.MetalType.Gold
                                    || iMetalType == (int)Enums.EnumClass.MetalType.Platinum
                                    || iMetalType == (int)Enums.EnumClass.MetalType.Silver
                                    || iMetalType == (int)Enums.EnumClass.MetalType.Palladium)
                                {
                                    dMetalCurrentRate = getMetalRate(saleLineItem.ItemId, saleLine.Dimension.ConfigId);
                                    sAmt = Convert.ToDecimal(sQty * dMetalCurrentRate);
                                }
                                //else if (iMetalType == (int)Enums.EnumClass.MetalType.LooseDmd
                                //    || iMetalType == (int)Enums.EnumClass.MetalType.Stone
                                //    || iMetalType == (int)Enums.EnumClass.MetalType.Diamond)
                                //{

                                //    dMetalCurrentRate = getRateFromMetalTable(saleLineItem.ItemId, retailTrans.Customer.CustomerId,
                                //        saleLine.Dimension.ConfigId, saleLine.Dimension.SizeId, saleLine.Dimension.ColorId, sQty);
                                //}
                                else
                                    sAmt = Convert.ToDecimal(dr["AMOUNT"]);

                                saleLine.PartnerData.TransactionType = "0";// add on 16/04/2014
                                retailTrans.PartnerData.IsRepairReturn = true;
                                saleLine.PartnerData.isMRP = false;
                                saleLine.PartnerData.IsSpecialDisc = false;
                                saleLine.PartnerData.SpecialDiscInfo = "";
                                saleLine.PartnerData.CustNo = "";
                                saleLine.PartnerData.Quantity = Convert.ToString(sQty);
                                saleLine.PartnerData.Ingredients = "";
                                saleLine.PartnerData.TotalAmount = Convert.ToString(sAmt);
                                saleLine.PartnerData.RETAILBATCHNO = sBatchId;
                                saleLine.PartnerData.REPAIRINVENTBATCHID = sBatchId;
                                saleLine.PartnerData.Pieces = Convert.ToString(dr["PDSCWQTY"]);

                                saleLine.PartnerData.ServiceItemCashAdjustmentTransactionID = string.Empty;
                                saleLine.PartnerData.ServiceItemCashAdjustmentStoreId = string.Empty;
                                saleLine.PartnerData.ServiceItemCashAdjustmentTerminalId = string.Empty;

                                saleLine.PartnerData.Rate = "0";
                                saleLine.PartnerData.RateType = "0";
                                saleLine.PartnerData.MakingRate = "";
                                saleLine.PartnerData.MakingType = "0";
                                saleLine.PartnerData.Amount = "0";
                                saleLine.PartnerData.MakingDisc = "0";
                                saleLine.PartnerData.MakingAmount = "0";
                                saleLine.PartnerData.TotalWeight = "0";
                                saleLine.PartnerData.LossPct = "0";
                                saleLine.PartnerData.LossWeight = "0";
                                saleLine.PartnerData.ExpectedQuantity = "";
                                saleLine.PartnerData.OChecked = false;
                                saleLine.PartnerData.SampleReturn = false;
                                saleLine.PartnerData.WastageType = "0";
                                saleLine.PartnerData.WastageQty = "0";
                                saleLine.PartnerData.WastageAmount = "";
                                saleLine.PartnerData.WastagePercentage = "0";
                                saleLine.PartnerData.WastageRate = "0";
                                saleLine.PartnerData.MakingDiscountType = "0";
                                saleLine.PartnerData.MakingTotalDiscount = "0";
                                saleLine.PartnerData.ConfigId = string.Empty;
                                saleLine.PartnerData.Purity = "0";
                                saleLine.PartnerData.GROSSWT = "0";
                                saleLine.PartnerData.GROSSUNIT = string.Empty;
                                saleLine.PartnerData.DMDWT = "0";
                                saleLine.PartnerData.DMDPCS = "0";
                                saleLine.PartnerData.DMDUNIT = string.Empty;
                                saleLine.PartnerData.DMDAMOUNT = "0";
                                saleLine.PartnerData.STONEWT = "0";
                                saleLine.PartnerData.STONEPCS = "0";
                                saleLine.PartnerData.STONEUNIT = string.Empty;
                                saleLine.PartnerData.STONEAMOUNT = "0";
                                saleLine.PartnerData.NETWT = "0";
                                saleLine.PartnerData.NETRATE = "0";
                                saleLine.PartnerData.NETUNIT = string.Empty;
                                saleLine.PartnerData.NETPURITY = "0";
                                saleLine.PartnerData.NETAMOUNT = "0";
                                saleLine.PartnerData.OGREFINVOICENO = string.Empty;
                                saleLine.PartnerData.SpecialDiscInfo = string.Empty;
                                saleLine.PartnerData.RETAILBATCHNO = string.Empty;
                                saleLine.PartnerData.ACTMKRATE = "0";
                                saleLine.PartnerData.ACTTOTAMT = "0";
                                saleLine.PartnerData.CHANGEDTOTAMT = "0";
                                saleLine.PartnerData.LINEDISC = "0";

                                saleLine.PartnerData.SALESCHANNELTYPE = string.Empty;
                                saleLine.PartnerData.SellingCostPrice = "0";
                                saleLine.PartnerData.ItemIdParent = "";
                                saleLine.PartnerData.UpdatedCostPrice = "0";
                                saleLine.PartnerData.GroupCostPrice = "0";
                                saleLine.PartnerData.PROMOCODE = string.Empty;
                                saleLine.PartnerData.FLAT = "0";
                                saleLine.PartnerData.OGREFBATCHNO = "";
                                saleLine.PartnerData.OGCHANGEDGROSSWT = "0";
                                saleLine.PartnerData.OGDMDRATE = "0";
                                saleLine.PartnerData.OGSTONEATE = "0";
                                saleLine.PartnerData.OGGROSSAMT = "0";
                                saleLine.PartnerData.OGACTAMT = "0";
                                saleLine.PartnerData.OGCHANGEDAMT = "0";
                                saleLine.PartnerData.OGFINALAMT = "0";
                                saleLine.PartnerData.FGPROMOCODE = "";
                                saleLine.PartnerData.ISREPAIR = "1";
                                saleLine.PartnerData.REPAIRBATCHID = sBatchId;

                                saleLine.PartnerData.REPAIRID = sRepId;
                                saleLine.PartnerData.RefRepairId = retailTrans.PartnerData.RefRepairId;
                                saleLine.PartnerData.TRANSFERCOSTPRICE = "0";
                                saleLine.PartnerData.LINEGOLDVALUE = "0";
                                saleLine.PartnerData.LINEGOLDTAX = "0";
                                saleLine.PartnerData.WTDIFFDISCQTY = "0";
                                saleLine.PartnerData.PurityReading1 = "0";
                                saleLine.PartnerData.PurityReading2 = "0";
                                saleLine.PartnerData.PurityReading3 = "0";
                                saleLine.PartnerData.PURITYPERSON = "";
                                saleLine.PartnerData.PURITYPERSONNAME = "";
                                saleLine.PartnerData.LINEOMVALUE = "0";
                                saleLine.PartnerData.WTDIFFDISCAMT = "0";

                                saleLine.PartnerData.OGPSTONEWT = "0";
                                saleLine.PartnerData.OGPSTONEPCS = "0";
                                saleLine.PartnerData.OGPSTONEUNIT = string.Empty;
                                saleLine.PartnerData.OGPSTONERATE = "0";
                                saleLine.PartnerData.OGPSTONEAMOUNT = "0";
                                #endregion

                                #region Sales person
                                /* if (saleLineItem != null)
                                {
                                    for (int i = 0; i < 1000; i++)// for malabar dubai changes only one sales persons will be added on transaction //20/06/2016
                                    {
                                        if (!string.IsNullOrEmpty(saleLineItem.SalesPersonId))
                                            break;
                                        else
                                        {
                                            if (retailTrans != null)
                                            {
                                                if (retailTrans.SaleItems.Count > 0)
                                                {
                                                    foreach (SaleLineItem salsItems in retailTrans.SaleItems)
                                                    {
                                                        //if(saleLine.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                                        //{
                                                        if (!string.IsNullOrEmpty(salsItems.SalesPersonId))
                                                        {
                                                            saleLineItem.SalesPersonId = salsItems.SalesPersonId;
                                                            saleLineItem.SalespersonName = salsItems.SalespersonName;
                                                            break;
                                                        }
                                                        else
                                                        {

                                                            LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                                            dialog6.ShowDialog();
                                                            if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                                            {
                                                                saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                                                saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                                                break;
                                                            }
                                                        }
                                                    }
                                                    // }
                                                }
                                                else
                                                {
                                                    LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                                    dialog6.ShowDialog();
                                                    if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                                    {
                                                        saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                                        saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                                        break;
                                                    }
                                                }
                                            }

                                        }
                                    }
                                }*/
                                #endregion
                                return;
                            }
                        }
                        else
                        {
                            #region
                            if (saleLine.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                            {
                                GetItemType(saleLineItem.ItemId);
                                if (iOWNDMD == 0 && iOWNOG == 0 && iOTHERDMD == 0 && iOTHEROG == 0)
                                {
                                    if (IsBulkItem(saleLineItem.ItemId))
                                    {
                                        saleLine.PartnerData.BulkItem = 1;
                                        if (IsBatchItem(saleLineItem.ItemId))
                                        {
                                            //frmSalesInfoCode objBatch = new frmSalesInfoCode(true);
                                            //objBatch.ShowDialog();
                                            //saleLine.PartnerData.BULKINVENTBATCHID = objBatch.sCodeOrRemarks;

                                            GetValidBulkItemPcsAndQtyForTrans(ref dBulkPdsQty, ref dBulkQty, saleLine, saleLine.PartnerData.BULKINVENTBATCHID);
                                        }
                                        else
                                        {
                                            GetValidBulkItemPcsAndQtyForTrans(ref dBulkPdsQty, ref dBulkQty, saleLine, "");
                                        }
                                        if (IsCatchWtItem(saleLineItem.ItemId))
                                        {
                                            if (dBulkPdsQty <= 0 || dBulkQty <= 0)
                                            {
                                                MessageBox.Show("Please check the inventory for " + Convert.ToString(saleLineItem.ItemId) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                preTriggerResult.ContinueOperation = false;
                                            }
                                            else if (dBulkPdsQty < Convert.ToDecimal(saleLine.PartnerData.Pieces) || dBulkQty < Convert.ToDecimal(saleLine.PartnerData.Quantity))
                                            {
                                                MessageBox.Show("Please check the inventory for " + Convert.ToString(saleLineItem.ItemId) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                preTriggerResult.ContinueOperation = false;
                                            }
                                        }
                                        else
                                        {
                                            if (dBulkQty <= 0)
                                            {
                                                MessageBox.Show("Please check the inventory for " + Convert.ToString(saleLineItem.ItemId) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                preTriggerResult.ContinueOperation = false;
                                            }
                                            else if (dBulkQty < Convert.ToDecimal(saleLine.PartnerData.Quantity))
                                            {
                                                MessageBox.Show("Please check the inventory for " + Convert.ToString(saleLineItem.ItemId) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                preTriggerResult.ContinueOperation = false;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        saleLine.PartnerData.BulkItem = 0;
                                    }
                                }
                                else
                                {
                                    if (IsBulkItem(saleLineItem.ItemId))
                                    {
                                        saleLine.PartnerData.BulkItem = 1;
                                    }
                                    else
                                    {
                                        saleLine.PartnerData.BulkItem = 0;
                                    }
                                }
                            }
                            #endregion
                        }
                    }

                }
            }
            #endregion

            string sMRPRate = string.Empty;
            if (saleLineItem != null)
            {
                #region
                retailTrans.PartnerData.IsGSSMaturity = false;
                string sGSSAdjItemId = "";
                string sGSSDiscItemId = "";
                string sCRWREPAIRADJITEMFOROG = "";
                string sCRWREPAIRADJITEM = "";
                string sPOSGSSAmountAdjItemId = "";
                string sPOSGSSAmountDiscItemId = "";
                GSSAdjustmentItemID(ref sGSSAdjItemId, ref sGSSDiscItemId,
                    ref sCRWREPAIRADJITEMFOROG, ref sCRWREPAIRADJITEM,
                    ref sPOSGSSAmountAdjItemId, ref sPOSGSSAmountDiscItemId);

                bool bGSSTaxApplicable = IsGSSEmiInclOfTax();

                string sGMAAdvItem = "";
                string sGMALGItem = "";
                string sGMADiscItem = "";
                string sGMAGSTItem = "";

                GMAAdjustmentItemID(ref sGMAAdvItem, ref sGMAGSTItem, ref sGMALGItem, ref sGMADiscItem);

                if (saleLineItem.ItemId == sCRWREPAIRADJITEMFOROG
                   || saleLineItem.ItemId == sCRWREPAIRADJITEM)
                {
                    saleLine.PartnerData.TransactionType = "0";// add on 16/04/2014
                    retailTrans.PartnerData.IsRepairReturn = true;
                    saleLine.PartnerData.isMRP = false;
                    saleLine.PartnerData.IsSpecialDisc = false;
                    saleLine.PartnerData.SpecialDiscInfo = "";
                    saleLine.PartnerData.CustNo = "";
                    saleLine.PartnerData.Quantity = "1";
                    saleLine.PartnerData.Ingredients = "";
                    saleLine.PartnerData.TotalAmount = "";
                    saleLine.PartnerData.RETAILBATCHNO = string.Empty;
                    saleLine.PartnerData.isFullReturn = false;

                    saleLine.PartnerData.ServiceItemCashAdjustmentTransactionID = string.Empty;
                    saleLine.PartnerData.ServiceItemCashAdjustmentStoreId = string.Empty;
                    saleLine.PartnerData.ServiceItemCashAdjustmentTerminalId = string.Empty;

                    #region Sales person
                    /*if (saleLineItem != null)
                    {
                        for (int i = 0; i < 1000; i++)// for malabar dubai changes only one sales persons will be added on transaction //20/06/2016
                        {
                            if (!string.IsNullOrEmpty(saleLineItem.SalesPersonId))
                                break;
                            else
                            {
                                if (retailTrans != null)
                                {
                                    if (retailTrans.SaleItems.Count > 0)
                                    {
                                        foreach (SaleLineItem salsItems in retailTrans.SaleItems)
                                        {
                                            //if(saleLine.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                            //{
                                            if (!string.IsNullOrEmpty(salsItems.SalesPersonId))
                                            {
                                                saleLineItem.SalesPersonId = salsItems.SalesPersonId;
                                                saleLineItem.SalespersonName = salsItems.SalespersonName;
                                                break;
                                            }
                                            else
                                            {

                                                LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                                dialog6.ShowDialog();
                                                if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                                {
                                                    saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                                    saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                                    break;
                                                }
                                            }
                                        }
                                        // }
                                    }
                                    else
                                    {
                                        LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                        dialog6.ShowDialog();
                                        if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                        {
                                            saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                            saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                            break;
                                        }
                                    }
                                }

                            }
                        }
                    }*/
                    #endregion
                    return;
                }

                if (saleLineItem.ItemId == sGSSAdjItemId
                    || saleLineItem.ItemId == sGSSDiscItemId
                    || saleLineItem.ItemId == sPOSGSSAmountAdjItemId
                    || saleLineItem.ItemId == sPOSGSSAmountDiscItemId)
                {
                    saleLine.PartnerData.TransactionType = "0";// add on 16/04/2014
                    retailTrans.PartnerData.IsRepairReturn = false;
                    saleLine.PartnerData.isMRP = false;
                    saleLine.PartnerData.IsSpecialDisc = false;
                    saleLine.PartnerData.SpecialDiscInfo = "";
                    saleLine.PartnerData.CustNo = "";
                    saleLine.PartnerData.Quantity = "0";
                    saleLine.PartnerData.Ingredients = "";
                    saleLine.PartnerData.TotalAmount = "";
                    saleLine.PartnerData.RETAILBATCHNO = string.Empty;
                    saleLine.PartnerData.ServiceItemCashAdjustmentTransactionID = string.Empty;
                    saleLine.PartnerData.ServiceItemCashAdjustmentStoreId = string.Empty;
                    saleLine.PartnerData.ServiceItemCashAdjustmentTerminalId = string.Empty;
                    saleLine.PartnerData.SaleAdjustmentGoldQty = "0";
                    saleLine.PartnerData.SaleAdjustmentGoldAmt = "0";

                    if (!bGSSTaxApplicable)
                    {
                        saleLine.SalesTaxGroupId = "";
                        saleLine.SalesTaxGroupIdOriginal = "";
                        saleLine.TaxGroupId = "";
                        saleLine.TaxGroupIdOriginal = "";
                    }

                    retailTrans.PartnerData.IsGSSMaturity = true;
                    saleLine.PartnerData.isFullReturn = false;

                    #region Sales person
                    /*if (saleLineItem != null)
                    {
                        for (int i = 0; i < 1000; i++)// for malabar dubai changes only one sales persons will be added on transaction //20/06/2016
                        {
                            if (!string.IsNullOrEmpty(saleLineItem.SalesPersonId))
                                break;
                            else
                            {
                                if (retailTrans != null)
                                {
                                    if (retailTrans.SaleItems.Count > 0)
                                    {
                                        foreach (SaleLineItem salsItems in retailTrans.SaleItems)
                                        {
                                            //if(saleLine.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                            //{

                                            if (salsItems.ItemId == saleLineItem.ItemId)
                                            {
                                                MessageBox.Show("Invalid item selected");
                                                preTriggerResult.ContinueOperation = false;
                                                return;
                                            }

                                            if (!string.IsNullOrEmpty(salsItems.SalesPersonId))
                                            {
                                                saleLineItem.SalesPersonId = salsItems.SalesPersonId;
                                                saleLineItem.SalespersonName = salsItems.SalespersonName;
                                                break;
                                            }
                                            else
                                            {

                                                LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                                dialog6.ShowDialog();
                                                if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                                {
                                                    saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                                    saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                                    break;
                                                }
                                            }
                                        }
                                        // }
                                    }
                                    else
                                    {
                                        LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                        dialog6.ShowDialog();
                                        if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                        {
                                            saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                            saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                            break;
                                        }
                                    }
                                }

                            }
                        }
                    }*/
                    #endregion

                    return;
                }

                #region GMA
                if (saleLineItem.ItemId == sGMAAdvItem
                 || saleLineItem.ItemId == sGMALGItem
                 || saleLineItem.ItemId == sGMADiscItem
                 || saleLineItem.ItemId == sGMAGSTItem)
                {
                    saleLine.PartnerData.TransactionType = "0";
                    retailTrans.PartnerData.IsRepairReturn = false;
                    saleLine.PartnerData.isMRP = false;
                    saleLine.PartnerData.IsSpecialDisc = false;
                    saleLine.PartnerData.SpecialDiscInfo = "";
                    saleLine.PartnerData.CustNo = "";
                    saleLine.PartnerData.Quantity = "0";
                    saleLine.PartnerData.Ingredients = "";
                    saleLine.PartnerData.TotalAmount = "";
                    saleLine.PartnerData.RETAILBATCHNO = string.Empty;
                    saleLine.PartnerData.ServiceItemCashAdjustmentTransactionID = string.Empty;
                    saleLine.PartnerData.ServiceItemCashAdjustmentStoreId = string.Empty;
                    saleLine.PartnerData.ServiceItemCashAdjustmentTerminalId = string.Empty;
                    saleLine.PartnerData.SaleAdjustmentGoldQty = "0";
                    saleLine.PartnerData.SaleAdjustmentGoldAmt = "0";

                    saleLine.PartnerData.isFullReturn = false;
                    return;
                }
                #endregion


                foreach (SaleLineItem SLItem in retailTrans.SaleItems)
                {
                    if (SLItem.ItemId == sGSSAdjItemId || SLItem.ItemId == sPOSGSSAmountAdjItemId)
                    {
                        saleLine.PartnerData.TransactionType = "0";// add on 16/04/2014

                        retailTrans.PartnerData.IsGSSMaturity = true;

                        if (!bGSSTaxApplicable)
                        {
                            SLItem.SalesTaxGroupId = "";
                            SLItem.SalesTaxGroupIdOriginal = "";
                            SLItem.TaxGroupId = "";
                            SLItem.TaxGroupIdOriginal = "";
                        }

                        break;
                    }
                }

                if (retailTrans.SaleIsReturnSale)//for new exchange 
                {
                    foreach (SaleLineItem SLItem in retailTrans.SaleItems)
                    {
                        decimal dCalcExchangeQt = Convert.ToDecimal(SLItem.PartnerData.CalExchangeQty);
                        if (dCalcExchangeQt > 0)
                        {
                            retailTrans.PartnerData.SaleAdjustment = true;
                            saleLine.PartnerData.LINEGOLDVALUE = "0";
                            saleLine.PartnerData.LINEGOLDTAX = "0";
                            saleLine.PartnerData.LINEOMVALUE = "0";
                            break;
                        }
                    }
                }

                #endregion

                #region
                string sAdjustmentId = AdjustmentItemID();
                foreach (SaleLineItem SLineItem in retailTrans.SaleItems)
                {
                    if (SLineItem.ItemId == sAdjustmentId)
                    {
                        saleLine.PartnerData.TransactionType = "0";// add on 16/04/2014
                        retailTrans.PartnerData.SaleAdjustment = true;
                        saleLine.PartnerData.LINEGOLDVALUE = "0";
                        saleLine.PartnerData.LINEGOLDTAX = "0";
                        saleLine.PartnerData.LINEOMVALUE = "0";
                        #region Sales person
                        /*if (saleLineItem != null)
                        {
                            for (int i = 0; i < 1000; i++)// for malabar dubai changes only one sales persons will be added on transaction //20/06/2016
                            {
                                if (!string.IsNullOrEmpty(saleLineItem.SalesPersonId))
                                    break;
                                else
                                {
                                    if (retailTrans != null)
                                    {
                                        if (retailTrans.SaleItems.Count > 0)
                                        {
                                            foreach (SaleLineItem salsItems in retailTrans.SaleItems)
                                            {
                                                //if(saleLine.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                                //{
                                                if (!string.IsNullOrEmpty(salsItems.SalesPersonId))
                                                {
                                                    saleLineItem.SalesPersonId = salsItems.SalesPersonId;
                                                    saleLineItem.SalespersonName = salsItems.SalespersonName;
                                                    break;
                                                }
                                                else
                                                {

                                                    LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                                    dialog6.ShowDialog();
                                                    if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                                    {
                                                        saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                                        saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                                        break;
                                                    }
                                                }
                                            }
                                            // }
                                        }
                                        else
                                        {
                                            LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                            dialog6.ShowDialog();
                                            if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                            {
                                                saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                                saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                                break;
                                            }
                                        }
                                    }

                                }
                            }
                        }*/
                        #endregion
                        break;
                    }
                }
                #endregion

                #region Service Item
                if (saleLineItem.ItemId == AdjustmentItemID())
                {

                    saleLine.PartnerData.isMRP = false;
                    saleLine.PartnerData.TransactionType = "0";// add on 16/04/2014
                    saleLine.PartnerData.Ingredients = string.Empty; // added on 12/11/14
                    saleLine.PartnerData.TotalAmount = "0";// added on 12/11/14
                    saleLine.PartnerData.Rate = "0";// added on 12/11/14
                    saleLine.PartnerData.IsSpecialDisc = false;
                    saleLine.PartnerData.SpecialDiscInfo = "";
                    saleLine.PartnerData.CustNo = "";
                    saleLine.PartnerData.RETAILBATCHNO = string.Empty;
                    saleLine.PartnerData.ACTMKRATE = "0";
                    saleLine.PartnerData.ACTTOTAMT = "0";
                    saleLine.PartnerData.CHANGEDTOTAMT = "0";
                    saleLine.PartnerData.LINEDISC = "0";

                    saleLine.PartnerData.OGREFBATCHNO = "0";
                    saleLine.PartnerData.OGCHANGEDGROSSWT = "0";
                    saleLine.PartnerData.OGDMDRATE = "0";
                    saleLine.PartnerData.OGSTONEATE = "0";
                    saleLine.PartnerData.OGGROSSAMT = "0";
                    saleLine.PartnerData.OGACTAMT = "0";
                    saleLine.PartnerData.OGCHANGEDAMT = "0";
                    saleLine.PartnerData.OGFINALAMT = "0";
                    saleLine.PartnerData.FGPROMOCODE = string.Empty;
                    saleLine.PartnerData.ISREPAIR = "0";
                    saleLine.PartnerData.REPAIRBATCHID = string.Empty;
                    saleLine.PartnerData.REPAIRINVENTBATCHID = string.Empty;
                    saleLine.PartnerData.isFullReturn = false;
                    saleLine.PartnerData.TRANSFERCOSTPRICE = "0";
                    saleLine.PartnerData.LINEGOLDVALUE = "0";
                    saleLine.PartnerData.LINEGOLDTAX = "0";
                    saleLine.PartnerData.WTDIFFDISCQTY = "0";
                    saleLine.PartnerData.LINEOMVALUE = "0";
                    saleLine.PartnerData.WTDIFFDISCAMT = "0";

                    saleLine.PartnerData.SaleAdjustmentCommitedQty = 0;
                    saleLine.PartnerData.SaleAdjustmentCommitedForDays = 0;
                    saleLine.PartnerData.SaleAdjustmentAdvanceDepositDate = "";

                    if (string.IsNullOrEmpty(Convert.ToString(retailTrans.PartnerData.AdjustmentCustAccount)))
                    {
                        System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> sale = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);

                        DataRow drAdjustment = null;
                        // BlankOperations.CustomerAdvanceAdjustment oAdjustment = new BlankOperations.CustomerAdvanceAdjustment(posTransaction);
                        BlankOperations.CustomerAdvanceAdjustment oAdjustment = new BlankOperations.CustomerAdvanceAdjustment(posTransaction);

                        if (sale.Count == 0)
                            drAdjustment = oAdjustment.AmountToBeAdjusted(retailTrans.Customer.CustomerId);
                        else
                            drAdjustment = oAdjustment.AmountToBeAdjusted(retailTrans.Customer.CustomerId, true);

                        if (drAdjustment == null)
                        {
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }
                        else
                        {
                            // Avg Gold Rate Adjustment
                            #region Old logic
                            ////if(sale.Count == 0)
                            ////{
                            ////    if(Convert.ToInt32(drAdjustment["GoldFixing"]) == 1)
                            ////        retailTrans.PartnerData.SaleAdjustmentGoldAmt = Convert.ToDecimal(drAdjustment["AMOUNT"]);
                            ////    else
                            ////        retailTrans.PartnerData.SaleAdjustmentGoldAmt = 0;

                            ////    retailTrans.PartnerData.SaleAdjustmentGoldQty = Convert.ToDecimal(drAdjustment["GoldQuantity"]);
                            ////}
                            ////else
                            ////{
                            ////    if(Convert.ToInt32(drAdjustment["GoldFixing"]) == 1)
                            ////        retailTrans.PartnerData.SaleAdjustmentGoldAmt = Convert.ToDecimal(retailTrans.PartnerData.SaleAdjustmentGoldAmt) + Convert.ToDecimal(drAdjustment["AMOUNT"]);


                            ////    retailTrans.PartnerData.SaleAdjustmentGoldQty = Convert.ToDecimal(drAdjustment["GoldQuantity"]);

                            ////    retailTrans.PartnerData.SaleAdjustmentGoldQty = Convert.ToDecimal(retailTrans.PartnerData.SaleAdjustmentGoldQty) + Convert.ToDecimal(drAdjustment["GoldQuantity"]);
                            ////}
                            #endregion
                            //
                            //Start: added on 23112017 for inavild advance not adjust
                            DateTime transDate = Convert.ToDateTime(drAdjustment["TransDate"]);
                            string iIsConvertToAdvAdj = Convert.ToString(drAdjustment["ConvertToAdvance"]);
                            string sIsLegacy = Convert.ToString(drAdjustment["Legacy"]);

                            if (Convert.ToString(drAdjustment["RETAILSTOREID"]) == Convert.ToString(ApplicationSettings.Database.StoreID)
                                && (Convert.ToString(sIsLegacy).ToUpper() == "NO"))//added on 130218
                            {
                                if (transDate <= DateTime.Now && transDate >= DateTime.Now.AddDays(-7))
                                {
                                    bool bValid = IsValidAdvance(Convert.ToString(drAdjustment["TransactionID"]), Convert.ToString(drAdjustment["RETAILSTOREID"]), Convert.ToString(drAdjustment["RETAILTERMINALID"]));
                                    if (!bValid)
                                    {
                                        MessageBox.Show("Invalid advance! Please contact head office.");
                                        preTriggerResult.ContinueOperation = false;
                                        return;
                                    }
                                }
                            }//End: added on 23112017 for inavild advance not adjust

                            #region Tax Blank Added on 20/12/2017
                            DateTime taxCutOffDate = getTaxCutOffDate();
                            if (transDate < taxCutOffDate)
                            {
                                saleLine.SalesTaxGroupId = "";
                                saleLine.SalesTaxGroupIdOriginal = "";
                                saleLine.TaxGroupId = "";
                                saleLine.TaxGroupIdOriginal = "";
                            }

                            if (Convert.ToString(iIsConvertToAdvAdj).ToUpper() == "YES")//added on 21122017
                            {
                                saleLine.SalesTaxGroupId = "";
                                saleLine.SalesTaxGroupIdOriginal = "";
                                saleLine.TaxGroupId = "";
                                saleLine.TaxGroupIdOriginal = "";
                            }
                            #endregion

                            //if(Convert.ToInt32(drAdjustment["GoldFixing"]) == 1)// for mala changes on 19/12/16 commented
                            //{
                            saleLine.PartnerData.SaleAdjustmentGoldAmt = Convert.ToDecimal(drAdjustment["AMOUNT"]);
                            saleLine.PartnerData.SaleAdjustmentGoldQty = Convert.ToDecimal(drAdjustment["GoldQuantity"]);
                            saleLine.PartnerData.SaleAdjustmentMetalRate = Convert.ToDecimal(drAdjustment["MetalRate"]);
                            saleLine.PartnerData.SaleAdjustmentOrderNo = Convert.ToString(drAdjustment["ORDERNUM"]);
                            //}
                            //else
                            //{
                            //    saleLine.PartnerData.SaleAdjustmentGoldAmt = 0;
                            //    saleLine.PartnerData.SaleAdjustmentGoldQty = 0;
                            //}

                            saleLine.PartnerData.ServiceItemCashAdjustmentPrice = Convert.ToString(drAdjustment["AMOUNT"]);
                            saleLine.PartnerData.ServiceItemCashAdjustmentTransactionID = Convert.ToString(drAdjustment["TransactionID"]);
                            saleLine.PartnerData.ServiceItemCashAdjustmentStoreId = Convert.ToString(drAdjustment["RETAILSTOREID"]);
                            saleLine.PartnerData.ServiceItemCashAdjustmentTerminalId = Convert.ToString(drAdjustment["RETAILTERMINALID"]);
                            
                            //=========================================Soutik==============================================================
                            saleLine.PartnerData.SaleAdjustmentCommitedQty = Convert.ToDecimal(drAdjustment["CommitedQty"]);
                            saleLine.PartnerData.SaleAdjustmentCommitedForDays = Convert.ToDecimal(drAdjustment["CommitedForDays"]);
                            saleLine.PartnerData.SaleAdjustmentAdvanceDepositDate = Convert.ToString(transDate);
                            //=============================================================================================================


                            // None = 0,
                            //OGPurchase = 1,
                            //OGExchange = 2,
                            //SaleReturn = 3,
                            int iAdvagainst = 0;
                            if (Convert.ToString(drAdjustment["AdvAgainst"]) == "OG Purchase")
                                iAdvagainst = 1;
                            else if (Convert.ToString(drAdjustment["AdvAgainst"]) == "OG Exchange")
                                iAdvagainst = 2;
                            else if (Convert.ToString(drAdjustment["AdvAgainst"]) == "Sale Return")
                                iAdvagainst = 3;

                            saleLine.PartnerData.AdvAgainst = iAdvagainst; //(int)(AdvAgainst)Enum.Parse(typeof(AdvAgainst), Convert.ToString(drAdjustment["AdvAgainst"]));// Convert.ToInt16(drAdjustment["AdvAgainst"]);


                            if (iAdvagainst == 1)//"OG Purchase"   added on 12032018
                            {
                                saleLine.SalesTaxGroupId = "";
                                saleLine.SalesTaxGroupIdOriginal = "";
                                saleLine.TaxGroupId = "";
                                saleLine.TaxGroupIdOriginal = "";
                            }


                            //isItemAvailableORReturn(" UPDATE RETAILADJUSTMENTTABLE SET ISADJUSTED='1' WHERE TRANSACTIONID='" + Convert.ToString(drAdjustment["TransactionID"]) + "' AND RETAILSTOREID='" + Convert.ToString(drAdjustment["RETAILSTOREID"]) + "' AND RETAILTERMINALID='" + Convert.ToString(drAdjustment["RETAILTERMINALID"]) + "'");
                            //updateCustomerAdvanceAdjustment(Convert.ToString(drAdjustment["Transaction ID"]), 1);

                            //RTS is calling for update changed on 27/11/2015
                            try
                            {
                                ReadOnlyCollection<object> containerArray;
                                string sMsg = string.Empty;

                                if (PosApplication.Instance.TransactionServices.CheckConnection())
                                {
                                    ReadOnlyCollection<object> conArrLockChk;
                                    conArrLockChk = PosApplication.Instance.TransactionServices.InvokeExtension("advanceLockedCheck",
                                                        Convert.ToString(drAdjustment["TransactionID"]),
                                                        Convert.ToString(drAdjustment["RETAILSTOREID"]),
                                                        Convert.ToString(drAdjustment["RETAILTERMINALID"]));

                                    bool bResultLockChk = Convert.ToBoolean(conArrLockChk[1]);

                                    if (!bResultLockChk)
                                    {
                                        //containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("updateAdvanceForAdjust", Convert.ToString(drAdjustment["TransactionID"]), Convert.ToString(drAdjustment["RETAILSTOREID"]), Convert.ToString(drAdjustment["RETAILTERMINALID"]));
                                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("advanceLock",
                                                        Convert.ToString(drAdjustment["TransactionID"]),
                                                        Convert.ToString(drAdjustment["RETAILSTOREID"]),
                                                        Convert.ToString(drAdjustment["RETAILTERMINALID"]));

                                        sMsg = Convert.ToString(containerArray[2]);
                                        // MessageBox.Show(sMsg);

                                        bool bResult = Convert.ToBoolean(containerArray[1]);
                                        if (!bResult)
                                        {
                                            ReadOnlyCollection<object> containerArray1;
                                            //containerArray1 = PosApplication.Instance.TransactionServices.InvokeExtension("AdvanceAdjustedCheck",
                                            //Convert.ToString(drAdjustment["TransactionID"]), Convert.ToString(drAdjustment["RETAILSTOREID"]), Convert.ToString(drAdjustment["RETAILTERMINALID"]));
                                            containerArray1 = PosApplication.Instance.TransactionServices.InvokeExtension("advanceLockedCheck",
                                                                Convert.ToString(drAdjustment["TransactionID"]),
                                                                Convert.ToString(drAdjustment["RETAILSTOREID"]),
                                                                Convert.ToString(drAdjustment["RETAILTERMINALID"]));

                                            bool bResult1 = Convert.ToBoolean(containerArray1[1]);

                                            if (!bResult1)
                                            {
                                                MessageBox.Show(string.Format("Failed to advance lock due to RTS transaction issue"));
                                                preTriggerResult.ContinueOperation = false;
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show(string.Format("Advance is locked"));
                                        preTriggerResult.ContinueOperation = false;
                                        return;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {


                            }
                        }
                    }
                    else
                    {
                        //string order = Convert.ToString(retailTrans.PartnerData.AdjustmentOrderNum);
                        //string cust = Convert.ToString(retailTrans.PartnerData.AdjustmentCustAccount);
                        string sTableName = "ORDERINFO" + "" + ApplicationSettings.Database.TerminalID;
                        string order = "";
                        string cust = "";
                        int iLineNum = 0;
                        getMultiAdjOrderNo(sTableName, ref order, ref cust, ref iLineNum);

                        BlankOperations.CustomerAdvanceAdjustment oAdjustment = new BlankOperations.CustomerAdvanceAdjustment(posTransaction);
                        DataRow drAdjustment = null;
                        System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> sale = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
                        if (sale.Count == 0)
                            drAdjustment = oAdjustment.AmountToBeAdjusted(retailTrans.Customer.CustomerId, false, cust, order);
                        else
                            drAdjustment = oAdjustment.AmountToBeAdjusted(retailTrans.Customer.CustomerId, true, cust, order);
                        if (drAdjustment == null)
                        {
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }
                        else
                        {
                            //Start: added on 23112017 for inavild advance not adjust
                            DateTime transDate = Convert.ToDateTime(drAdjustment["TransDate"]);
                            string iIsConvertToAdvAdj = Convert.ToString(drAdjustment["ConvertToAdvance"]);
                            if (Convert.ToString(drAdjustment["RETAILSTOREID"]) == Convert.ToString(ApplicationSettings.Database.StoreID))
                            {
                                if (transDate <= DateTime.Now && transDate >= DateTime.Now.AddDays(-7))
                                {
                                    bool bValid = IsValidAdvance(Convert.ToString(drAdjustment["TransactionID"]), Convert.ToString(drAdjustment["RETAILSTOREID"]), Convert.ToString(drAdjustment["RETAILTERMINALID"]));
                                    if (!bValid)
                                    {
                                        MessageBox.Show("Invalid advance! Please contact head office.");
                                        preTriggerResult.ContinueOperation = false;
                                        return;
                                    }
                                }
                            }
                            //End: added on 23112017 for inavild advance not adjust
                            #region Tax Blank Added on 20/12/2017
                            DateTime taxCutOffDate = getTaxCutOffDate();
                            if (transDate < taxCutOffDate)
                            {
                                saleLine.SalesTaxGroupId = "";
                                saleLine.SalesTaxGroupIdOriginal = "";
                                saleLine.TaxGroupId = "";
                                saleLine.TaxGroupIdOriginal = "";
                            }

                            if (Convert.ToString(iIsConvertToAdvAdj).ToUpper() == "YES")//added on 21122017
                            {
                                saleLine.SalesTaxGroupId = "";
                                saleLine.SalesTaxGroupIdOriginal = "";
                                saleLine.TaxGroupId = "";
                                saleLine.TaxGroupIdOriginal = "";
                            }
                            #endregion

                            // Avg Gold Rate Adjustment
                            #region Old logic
                            ////if(sale.Count == 0)
                            ////{
                            ////    if(Convert.ToInt32(drAdjustment["GoldFixing"]) == 1)
                            ////    {
                            ////        retailTrans.PartnerData.SaleAdjustmentGoldAmt = Convert.ToDecimal(drAdjustment["AMOUNT"]);
                            ////    }
                            ////    else
                            ////    {
                            ////        retailTrans.PartnerData.SaleAdjustmentGoldAmt = 0;
                            ////    }

                            ////    retailTrans.PartnerData.SaleAdjustmentGoldQty = Convert.ToDecimal(drAdjustment["GoldQty"]);
                            ////}
                            ////else
                            ////{
                            ////    if(Convert.ToInt32(drAdjustment["GoldFixing"]) == 1)
                            ////    {
                            ////        retailTrans.PartnerData.SaleAdjustmentGoldAmt = Convert.ToDecimal(retailTrans.PartnerData.SaleAdjustmentGoldAmt) + Convert.ToDecimal(drAdjustment["AMOUNT"]);
                            ////    }
                            ////    retailTrans.PartnerData.SaleAdjustmentGoldQty = Convert.ToDecimal(retailTrans.PartnerData.SaleAdjustmentGoldQty) + Convert.ToDecimal(drAdjustment["GoldQty"]);
                            ////}
                            #endregion
                            //
                            //if(Convert.ToInt32(drAdjustment["GoldFixing"]) == 1)// for mala changes on 19/12/16 commented
                            //{
                            saleLine.PartnerData.SaleAdjustmentGoldAmt = Convert.ToDecimal(drAdjustment["AMOUNT"]);
                            saleLine.PartnerData.SaleAdjustmentGoldQty = Convert.ToDecimal(drAdjustment["GoldQuantity"]);
                            saleLine.PartnerData.SaleAdjustmentMetalRate = Convert.ToDecimal(drAdjustment["MetalRate"]);// added for order
                            saleLine.PartnerData.SaleAdjustmentOrderNo = Convert.ToString(drAdjustment["ORDERNUM"]);
                            saleLine.PartnerData.OrderLineNum = Convert.ToString(iLineNum);
                            //}
                            //else
                            //{
                            //    saleLine.PartnerData.SaleAdjustmentGoldAmt = 0;
                            //    saleLine.PartnerData.SaleAdjustmentGoldQty = 0;
                            //}

                            saleLine.PartnerData.ServiceItemCashAdjustmentPrice = Convert.ToString(drAdjustment["AMOUNT"]);
                            saleLine.PartnerData.ServiceItemCashAdjustmentTransactionID = Convert.ToString(drAdjustment["TransactionID"]);
                            saleLine.PartnerData.ServiceItemCashAdjustmentStoreId = Convert.ToString(drAdjustment["RETAILSTOREID"]);
                            saleLine.PartnerData.ServiceItemCashAdjustmentTerminalId = Convert.ToString(drAdjustment["RETAILTERMINALID"]);

                            int iAdvagainst = 0;
                            if (Convert.ToString(drAdjustment["AdvAgainst"]) == "OG Purchase")
                                iAdvagainst = 1;
                            else if (Convert.ToString(drAdjustment["AdvAgainst"]) == "OG Exchange")
                                iAdvagainst = 2;
                            else if (Convert.ToString(drAdjustment["AdvAgainst"]) == "Sale Return")
                                iAdvagainst = 3;
                            saleLine.PartnerData.AdvAgainst = iAdvagainst;// Convert.ToInt16(drAdjustment["AdvAgainst"]);


                            if (iAdvagainst == 1)//"OG Purchase"   added on 12032018
                            {
                                saleLine.SalesTaxGroupId = "";
                                saleLine.SalesTaxGroupIdOriginal = "";
                                saleLine.TaxGroupId = "";
                                saleLine.TaxGroupIdOriginal = "";
                            }
                            // isItemAvailableORReturn(" UPDATE RETAILADJUSTMENTTABLE SET ISADJUSTED='1' WHERE TRANSACTIONID='" + Convert.ToString(drAdjustment["TransactionID"]) + "' AND RETAILSTOREID='" + Convert.ToString(drAdjustment["RETAILSTOREID"]) + "' AND RETAILTERMINALID='" + Convert.ToString(drAdjustment["RETAILTERMINALID"]) + "'");

                            try
                            {
                                ReadOnlyCollection<object> containerArray;
                                string sMsg = string.Empty;

                                if (PosApplication.Instance.TransactionServices.CheckConnection())
                                {

                                    ReadOnlyCollection<object> conArrLockChk;
                                    conArrLockChk = PosApplication.Instance.TransactionServices.InvokeExtension("advanceLockedCheck",
                                                        Convert.ToString(drAdjustment["TransactionID"]),
                                                        Convert.ToString(drAdjustment["RETAILSTOREID"]),
                                                        Convert.ToString(drAdjustment["RETAILTERMINALID"]));

                                    bool bResultLockChk = Convert.ToBoolean(conArrLockChk[1]);

                                    if (!bResultLockChk)
                                    {
                                        //containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("updateAdvanceForAdjust", Convert.ToString(drAdjustment["TransactionID"]), Convert.ToString(drAdjustment["RETAILSTOREID"]), Convert.ToString(drAdjustment["RETAILTERMINALID"]));

                                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("advanceLock",
                                                       Convert.ToString(drAdjustment["TransactionID"]),
                                                       Convert.ToString(drAdjustment["RETAILSTOREID"]),
                                                       Convert.ToString(drAdjustment["RETAILTERMINALID"]));
                                        sMsg = Convert.ToString(containerArray[2]);
                                        // MessageBox.Show(sMsg);

                                        bool bResult = Convert.ToBoolean(containerArray[1]);
                                        if (!bResult)
                                        {
                                            ReadOnlyCollection<object> containerArray1;
                                            //containerArray1 = PosApplication.Instance.TransactionServices.InvokeExtension("AdvanceAdjustedCheck",
                                            // Convert.ToString(drAdjustment["TransactionID"]), Convert.ToString(drAdjustment["RETAILSTOREID"]), Convert.ToString(drAdjustment["RETAILTERMINALID"]));

                                            containerArray1 = PosApplication.Instance.TransactionServices.InvokeExtension("advanceLockedCheck",
                                                                Convert.ToString(drAdjustment["TransactionID"]),
                                                                Convert.ToString(drAdjustment["RETAILSTOREID"]),
                                                                Convert.ToString(drAdjustment["RETAILTERMINALID"]));

                                            bool bResult1 = Convert.ToBoolean(containerArray1[1]);

                                            if (!bResult1)
                                            {
                                                MessageBox.Show(string.Format("Failed to advance lock due to RTS transaction issue"));
                                                preTriggerResult.ContinueOperation = false;
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        MessageBox.Show(string.Format("Advance is locked"));
                                        preTriggerResult.ContinueOperation = false;
                                        return;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }

                    #region Sales person
                    /*
                    if (saleLineItem != null)
                    {
                        for (int i = 0; i < 1000; i++)// for malabar dubai changes only one sales persons will be added on transaction //20/06/2016
                        {
                            if (!string.IsNullOrEmpty(saleLineItem.SalesPersonId))
                                break;
                            else
                            {
                                if (retailTrans != null)
                                {
                                    if (retailTrans.SaleItems.Count > 0)
                                    {
                                        foreach (SaleLineItem salsItems in retailTrans.SaleItems)
                                        {
                                            //if(saleLine.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                            //{
                                            if (!string.IsNullOrEmpty(salsItems.SalesPersonId))
                                            {
                                                saleLineItem.SalesPersonId = salsItems.SalesPersonId;
                                                saleLineItem.SalespersonName = salsItems.SalespersonName;
                                                break;
                                            }
                                            else
                                            {

                                                LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                                dialog6.ShowDialog();
                                                if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                                {
                                                    saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                                    saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                                    break;
                                                }
                                            }
                                        }
                                        // }
                                    }
                                    else
                                    {
                                        LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                        dialog6.ShowDialog();
                                        if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                        {
                                            saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                            saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                            break;
                                        }
                                    }
                                }

                            }
                        }
                    }*/
                    #endregion
                    return;
                }
                #endregion

                SqlConnection connection = new SqlConnection();
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                int? isLocked = null;
                int? isAvailable = null;
                int? isReturnLocked = null;
                int? isReturnAvailable = null;
                string sSuspendedeCustAccAndTransId = "";

                #region Non Booked SKU
                if (!Convert.ToBoolean(retailTrans.PartnerData.SKUBookedItems))
                {
                    #region When Normal sales
                    CheckForReturnSKUExistence(saleLine.ItemId, connection, out isReturnLocked, out isReturnAvailable);

                    if (isReturnAvailable == null && isReturnLocked == null)
                    {
                        string sTransCounter = "";

                        CheckForSKUExistence(saleLine.ItemId, connection, out isLocked, out isAvailable, ref sSuspendedeCustAccAndTransId, ref sTransCounter);

                        if (sTransCounter == "Transit")
                        {
                            MessageBox.Show("Item is in " + sTransCounter + " warehouse");
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }

                        if (isLocked != null && isAvailable != null)
                        {
                            if (Convert.ToBoolean(isLocked))
                            {
                                preTriggerResult.ContinueOperation = false;
                                if (!string.IsNullOrEmpty(sSuspendedeCustAccAndTransId))
                                {
                                    MessageBox.Show("Item has suspended against " + sSuspendedeCustAccAndTransId + "");
                                }
                                else
                                {
                                    preTriggerResult.MessageId = 50397;
                                }
                                //preTriggerResult.MessageId = 50397;

                                string query = string.Empty;

                                // query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                                query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                                isItemAvailableORReturn(query);
                            }
                            else if (!Convert.ToBoolean(isLocked) && !Convert.ToBoolean(isAvailable))
                            {
                                preTriggerResult.ContinueOperation = false;
                                preTriggerResult.MessageId = 50398;
                                string query = string.Empty;
                                //query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                                query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                                isItemAvailableORReturn(query);
                            }
                            else
                            {
                                if (!SaleOperation(saleLine, posTransaction, connection))
                                {
                                    preTriggerResult.ContinueOperation = false;
                                }
                            }
                        }
                        else
                        {
                            if (!SaleOperation(saleLineItem, posTransaction, connection))
                            {
                                preTriggerResult.ContinueOperation = false;
                            }
                        }
                    }
                    else if (isReturnAvailable == 1 && isReturnLocked == 1)
                    {
                        preTriggerResult.MessageId = 50398;
                        preTriggerResult.ContinueOperation = false;
                        string query = string.Empty;
                        //   query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                        query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                        isItemAvailableORReturn(query);
                    }
                    else if (isReturnAvailable == 1 && isReturnLocked == 0)
                    {
                        preTriggerResult.MessageId = 50395;
                        preTriggerResult.ContinueOperation = false;
                        string query = string.Empty;
                        //  query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                        query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                        isItemAvailableORReturn(query);
                    }

                    else if ((isReturnAvailable == 0 && isReturnLocked == 0))
                    {
                        if (!SaleOperation(saleLineItem, posTransaction, connection))
                        {
                            preTriggerResult.ContinueOperation = false;
                        }
                    }
                    #endregion
                }
                else
                {
                    #region order item , multiple adjustment
                    string sOrderedSKU = "";
                    int? isOrderDeliverdSKU = null;
                    int iReleased = 0;
                    string sTransCounter = "";

                    CheckForBookSKUDelivered(saleLine.ItemId, connection, out isOrderDeliverdSKU, ref sOrderedSKU, ref iReleased, ref sTransCounter);

                    if (!string.IsNullOrEmpty(sOrderedSKU) && isOrderDeliverdSKU == 0 && iReleased == 0)
                    {

                        if (sTransCounter == "Transit")
                        {
                            MessageBox.Show("Item is in " + sTransCounter + " warehouse");
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }

                        if (Convert.ToString(retailTrans.PartnerData.AdjustmentOrderNum) != sOrderedSKU)
                        {
                            MessageBox.Show("Item is booked for order " + sOrderedSKU + " ");
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }
                        else if (!SaleOperation(saleLineItem, posTransaction, connection))
                        {
                            preTriggerResult.ContinueOperation = false;
                        }
                    }
                    else
                    {
                        CheckForReturnSKUExistence(saleLine.ItemId, connection, out isReturnLocked, out isReturnAvailable);

                        if (isReturnAvailable == null && isReturnLocked == null)
                        {
                            CheckForSKUExistence(saleLine.ItemId, connection, out isLocked, out isAvailable, ref sSuspendedeCustAccAndTransId, ref sTransCounter);

                            if (sTransCounter == "Transit")
                            {
                                MessageBox.Show("Item is in " + sTransCounter + " warehouse");
                                preTriggerResult.ContinueOperation = false;
                                return;
                            }

                            if (isLocked != null && isAvailable != null)
                            {
                                if (Convert.ToBoolean(isLocked))
                                {
                                    preTriggerResult.ContinueOperation = false;
                                    if (!string.IsNullOrEmpty(sSuspendedeCustAccAndTransId))
                                    {
                                        MessageBox.Show("Item has suspended against " + sSuspendedeCustAccAndTransId + "");
                                    }
                                    else
                                    {
                                        preTriggerResult.MessageId = 50397;
                                    }
                                    //preTriggerResult.MessageId = 50397;

                                    string query = string.Empty;

                                    // query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                                    query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                                    isItemAvailableORReturn(query);
                                }
                                else if (!Convert.ToBoolean(isLocked) && !Convert.ToBoolean(isAvailable))
                                {
                                    preTriggerResult.ContinueOperation = false;
                                    preTriggerResult.MessageId = 50398;
                                    string query = string.Empty;
                                    //query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                                    query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                                    isItemAvailableORReturn(query);
                                }
                                else
                                {
                                    if (!SaleOperation(saleLine, posTransaction, connection))
                                    {
                                        preTriggerResult.ContinueOperation = false;
                                    }
                                }
                            }
                            else
                            {
                                if (!SaleOperation(saleLineItem, posTransaction, connection))
                                {
                                    preTriggerResult.ContinueOperation = false;
                                }
                            }
                        }
                        else if (isReturnAvailable == 1 && isReturnLocked == 1)
                        {
                            preTriggerResult.MessageId = 50398;
                            preTriggerResult.ContinueOperation = false;
                            string query = string.Empty;
                            //   query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                            query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                            isItemAvailableORReturn(query);
                        }
                        else if (isReturnAvailable == 1 && isReturnLocked == 0)
                        {
                            preTriggerResult.MessageId = 50395;
                            preTriggerResult.ContinueOperation = false;
                            string query = string.Empty;
                            //  query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
                            query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
                            isItemAvailableORReturn(query);
                        }

                        else if ((isReturnAvailable == 0 && isReturnLocked == 0))
                        {
                            if (!SaleOperation(saleLineItem, posTransaction, connection))
                            {
                                preTriggerResult.ContinueOperation = false;
                            }
                        }
                    }
                    //if (!SaleOperation(saleLineItem, posTransaction, connection))
                    //{
                    //    preTriggerResult.ContinueOperation = false;
                    //}
                    #endregion
                }
                #endregion

            }
            #endregion
            //End Nim
        }

        private static DataTable getIngredients(string sItemId)
        {
            DataTable dtIngredients = new DataTable();
            SqlConnection connection = new SqlConnection();

            connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            StringBuilder commandText = new StringBuilder();
            commandText.Append("  IF EXISTS(SELECT TOP(1) [SkuNumber] FROM [SKULine_Posted] WHERE  [SkuNumber] = '" + sItemId.Trim() + "')");
            commandText.Append("  BEGIN  ");
            commandText.Append(" SELECT SKULine_Posted.SkuNumber, SKULine_Posted.SkuDate, SKULine_Posted.ItemID,");
            commandText.Append(" INVENTDIM.InventDimID, INVENTDIM.InventSizeID,  ");
            commandText.Append(" INVENTDIM.InventColorID, INVENTDIM.ConfigID, INVENTDIM.InventBatchID,");
            commandText.Append(" CAST(ISNULL(SKULine_Posted.PDSCWQTY,0) AS INT) AS PCS, CAST(ISNULL(SKULine_Posted.QTY,0) AS NUMERIC(16,3)) AS QTY,  ");
            commandText.Append(" SKULine_Posted.CValue, SKULine_Posted.CRate AS Rate, SKULine_Posted.UnitID,X.METALTYPE");
            commandText.Append(" FROM  SKULine_Posted INNER JOIN ");
            commandText.Append(" INVENTDIM ON SKULine_Posted.InventDimID = INVENTDIM.INVENTDIMID ");
            commandText.Append(" INNER JOIN INVENTTABLE X ON SKULine_Posted.ITEMID = X.ITEMID ");
            commandText.Append(" WHERE INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
            commandText.Append(" AND  [SkuNumber] = '" + sItemId.Trim() + "' ORDER BY X.METALTYPE END ");


            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            using (SqlDataReader readerFixRateIngr = command.ExecuteReader())
            {
                dtIngredients.Load(readerFixRateIngr);
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dtIngredients;
        }

        public void PostSale(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PostSale", "After the sale of an item...", LSRetailPosis.LogTraceLevel.Trace);
            //Start:Nim
            string query = string.Empty;
            // query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 ";
            query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN='False' WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' ";
            isItemAvailableORReturn(query);
            //End:Nim
        }

        public void PreReturnItem(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            //Start:Nim
            #region Changed By Nimbus
            string query = string.Empty;
            // query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN=(CASE  WHEN ITEMRETURN='False' THEN 'True' ELSE 'False' END) WHERE ID=2 ";
            query = " UPDATE RETAILTEMPTABLE SET ITEMRETURN=(CASE  WHEN ITEMRETURN='False' THEN 'True' ELSE 'False' END) WHERE ID=2 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
            isItemAvailableORReturn(query);

            #endregion

            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            retailTrans.PartnerData.ItemIsReturnItem = true;
            //End:Nim
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PreReturnItem", "Prior to entering return state...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostReturnItem(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PostReturnItem", "After entering return state", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PreVoidItem(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, int lineId)
        {
            //Start:Nim
            #region
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                int isGiftCard = 0;

                foreach (var saleTender in retailTrans.TenderLines)
                {
                    if (saleTender.Amount > 0 && !saleTender.Voided)
                    {
                        MessageBox.Show("Void product cannot be used if payment line exists.");
                        preTriggerResult.ContinueOperation = false;
                        break;
                    }
                }

                foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                {
                    if (saleLineItem.ItemId != AdjustmentItemID())
                    {
                        if (Convert.ToString(lineId) == Convert.ToString(saleLineItem.LineId) && saleLineItem.Voided)
                        {
                            preTriggerResult.MessageId = 50396;
                            preTriggerResult.ContinueOperation = false;
                            break;
                        }
                    }
                    if (IsGiftItem(saleLineItem.ItemId))
                    {
                        isGiftCard = 1;
                        break;
                    }

                    if (Convert.ToString(lineId) == Convert.ToString(saleLineItem.LineId))
                    {
                        if (saleLineItem.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                        {
                            MessageBox.Show("No service item should be voided. Please void the transaction");
                            preTriggerResult.ContinueOperation = false;
                            break;
                        }
                    }
                }

                if (isGiftCard == 1)
                {
                    MessageBox.Show("Item can not be voided if gift card is there. Please void the transaction");
                    preTriggerResult.ContinueOperation = false;
                    return;
                }

                if (!string.IsNullOrEmpty(retailTrans.PartnerData.FreeGiftQTY))
                {
                    MessageBox.Show("Item can not be voided if free gift are there in sales line. Please void the transaction");
                    preTriggerResult.ContinueOperation = false;
                    return;
                }

                //Start: For Singapore only added on 070318
                if (retailTrans.PartnerData.SingaporeTaxCal == "1")
                {
                    MessageBox.Show("Item can not be voided once recalculation of tax is done. Please void the transaction");
                    preTriggerResult.ContinueOperation = false;
                    return;
                }
                //End: For Singapore only

                if (retailTrans.PartnerData.IsGSSMaturity)
                {
                    retailTrans.PartnerData.GSSSaleWt = Convert.ToDecimal(retailTrans.PartnerData.GSSSaleWt) - Convert.ToDecimal(retailTrans.PartnerData.RunningQtyGSS);
                }

                if (retailTrans.PartnerData.SaleAdjustment)
                {
                    retailTrans.PartnerData.TransAdjGoldQty = Convert.ToDecimal(retailTrans.PartnerData.TransAdjGoldQty) - Convert.ToDecimal(retailTrans.PartnerData.RunningQtyAdjustment);
                    MessageBox.Show("No item should be voided for adjustment. Please void the transaction");
                    preTriggerResult.ContinueOperation = false;
                    return;
                }
                if (retailTrans.PartnerData.APPLYGSSDISCDONE == true)
                {
                    MessageBox.Show("No item should be voided after GSS discount apply. Please void the transaction");
                    preTriggerResult.ContinueOperation = false;
                    return;
                }
                if (retailTrans.PartnerData.APPLYGMADISCDONE == true)
                {
                    MessageBox.Show("Item can not be added once GMA discount is applied");
                    preTriggerResult.ContinueOperation = false;
                    return;
                }

                foreach (SaleLineItem SLineItem in retailTrans.SaleItems)
                {
                    if (SLineItem.ItemType != BaseSaleItem.ItemTypes.Service)
                    {
                        #region If excahnge then 1 salable item should be in sales line
                        if (retailTrans.IncomeExpenseAmounts == 0
                            && retailTrans.SalesInvoiceAmounts == 0)
                        {
                            int iExistSalableItem1 = 0;
                            if (Convert.ToInt16(SLineItem.PartnerData.TransactionType) == 3)
                            {
                                foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
                                {
                                    if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                                        && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                        && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                                        && !saleLineItem1.Voided)
                                    {
                                        iExistSalableItem1 = 1;
                                        break;
                                    }
                                }

                                if (iExistSalableItem1 == 1)
                                {
                                    MessageBox.Show("Item can not be voided. Please void the transaction");
                                    preTriggerResult.ContinueOperation = false;
                                    return;
                                }
                            }
                        }
                        #endregion
                    }
                }

            }
            #endregion
            //End:Nim

            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PreVoidItem", "Before voiding an item", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostVoidItem(IPosTransaction posTransaction, int lineId)
        {
            //Start:Nim
            #region changed By Nimbus
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                {
                    if (saleLineItem.ItemId != AdjustmentItemID())
                    {
                        if (Convert.ToString(lineId) == Convert.ToString(saleLineItem.LineId))
                        {
                            retailTrans.PartnerData.PackingMaterial = "";
                            updateOrderDelivery(Convert.ToString(saleLineItem.PartnerData.OrderNum), Convert.ToString(saleLineItem.PartnerData.OrderLineNum), true, saleLineItem, saleLineItem.ItemId);


                            #region Making minus
                            decimal dMkAmt = 0m;

                            if (!String.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.MakingAmount)))
                                dMkAmt = Convert.ToDecimal(Convert.ToString(saleLineItem.PartnerData.MakingAmount));

                            string sTblName = "MAKINGINFO" + ApplicationSettings.Terminal.TerminalId;
                            UpdateMakingInfo(sTblName, retailTrans.TransactionId, dMkAmt);
                            #endregion
                        }
                    }

                    if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                    {
                        //RTS is calling for update changed on 27/11/2015
                        if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0))
                        {
                            string cmdStr1 = string.Empty;
                            int iMetalType = getMetalType(saleLineItem.ItemId);
                            if (iMetalType == (int)Enums.EnumClass.MetalType.GiftVoucher)
                            {
                                try
                                {
                                    string sMsg = string.Empty;
                                    ReadOnlyCollection<object> containerArray1;

                                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                                    {
                                        containerArray1 = PosApplication.Instance.TransactionServices.InvokeExtension("GiftCardVoid", saleLineItem.ItemId);
                                        sMsg = Convert.ToString(containerArray1[2]);
                                    }
                                }
                                catch (Exception ex)
                                {
                                }
                            }
                        }
                    }

                }

            }

            #endregion
            //End:Nim

            string source = "IItemTriggersV1.PostVoidItem";
            string value = "After voiding an item";
            LSRetailPosis.ApplicationLog.Log(source, value, LSRetailPosis.LogTraceLevel.Trace);
            LSRetailPosis.ApplicationLog.WriteAuditEntry(source, value);
        }

        public void PreSetQty(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
            //Start:Nim
            if (isServiceItem(posTransaction, lineId, saleLineItem, "QTY"))
            {
                preTriggerResult.ContinueOperation = false;
                preTriggerResult.MessageId = 50399;
            }
            //End:Nim

            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PreSetQty", "Before setting the qty for an item", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostSetQty(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PostSetQty", "After setting the qty from an item", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PrePriceOverride(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
            //Start:Nim
            if (isServiceItem(posTransaction, lineId, saleLineItem, "POVERRIDE"))
            {
                preTriggerResult.ContinueOperation = false;
                preTriggerResult.MessageId = 50399;
            }
            //End:Nim
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PrePriceOverride", "Before overriding the price on an item", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostPriceOverride(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV1.PostPriceOverride", "After overriding the price of an item", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

        #region IItemTriggersV2 Members

        public void PreClearQty(IPreTriggerResult preTriggerResult, ISaleLineItem saleLineItem, IPosTransaction posTransaction, int lineId)
        {
            //Start:Nim
            if (isServiceItem(posTransaction, lineId, saleLineItem, "CQTY"))
            {
                preTriggerResult.ContinueOperation = false;
                preTriggerResult.MessageId = 50399;
            }
            //End:Nim

            LSRetailPosis.ApplicationLog.Log("IItemTriggersV2.PreClearQty", "Triggered before clear the quantity of an item.", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostClearQty(IPosTransaction posTransaction, ISaleLineItem saleLineItem)
        {
            LSRetailPosis.ApplicationLog.Log("IItemTriggersV2.PostClearQty", "Triggered after clear the quantity of an item.", LSRetailPosis.LogTraceLevel.Trace);
        }

        #endregion

    }
}
