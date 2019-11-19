//Microsoft Dynamics AX for Retail POS Plug-ins 
//The following project is provided as SAMPLE code. Because this software is "as is," we may not provide support services for it.

using System.ComponentModel.Composition;
using System.Data.SqlClient;
using LSRetailPosis;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Data;
using LSRetailPosis.Settings;
using System.Text;
using System;
using LSRetailPosis.Transaction;
using System.Collections.Generic;
using System.Reflection;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.CSharp.RuntimeBinder;
using System.Windows.Forms;
using System.Collections.ObjectModel;
using LSRetailPosis.Transaction.Line.TenderItem;
using System.IO;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Linq;
using LSRetailPosis.Transaction.Line.Discount;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using LSRetailPosis.Transaction.Line.LoyaltyItem;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using DE = Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Reporting.WinForms;

namespace Microsoft.Dynamics.Retail.Pos.TransactionTriggers
{
    [Export(typeof(ITransactionTrigger))]
    public class TransactionTriggers : ITransactionTrigger
    {

        //Start:Nim
        string preTransType = string.Empty; ////Start : Changes on 08/04/2014 RHossain   
        string curTransType = string.Empty; //Start : Changes on 08/04/2014 RHossain   
        string sSalesInfoCode = string.Empty;//Start : Changes on 22/05/2014 RHossain  
        int bIsSale = 0;
        string sSplReturn = string.Empty;
        private IApplication application;
        int bIsPurchase = 0;
        int iOWNDMD = 0; int iOWNOG = 0; int iOTHERDMD = 0; int iOTHEROG = 0;
        bool bAdvRefund = false;

        string sTestValue = "";
        decimal dCashPayAmt = 0m;

        //===============Soutik
        string sCustAdjustMentId = string.Empty;

        DataTable dtPType = new DataTable();

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

            }
        }

        bool isDoneConvertAdvance = false;

        private IUtility Utility
        {
            get { return this.Application.BusinessLogic.Utility; }
        }

        enum AdvAgainst
        {
            None = 0,
            OGPurchase = 1,
            OGExchange = 2,
            SaleReturn = 3,
        }

        enum BulkTransactionType
        {
            None = 0,
            Receive = 1,
            Issue = 2,
            Sales = 3,
            SalesReturn = 4,
            OGPurchase = 5,
            OGExchange = 6,
            OGPurchaseReturn = 7,
            OGExchangeReturn = 8,
            OrderSampleReceive = 9,
            OrderStnDmdReceive = 10,
        }

        #region enum  RateType
        enum RateType
        {
            Weight = 0,
            Pieces = 1,
            Tot = 2,
        }
        #endregion

        enum MRPOrMkDiscType
        {
            None = 0,
            Making = 1,
            MRP = 2,
        }

        #region enum CRWRetailDiscPermission
        private enum CRWRetailDiscPermission
        {
            Cashier = 0,
            Salesperson = 1,
            Manager = 2,
            Other = 3,
            Manager2 = 4,
        }
        #endregion

        #region Changed By Nimbus - Update Customer Advance Adjustment
        private void updateCustomerAdvanceAdjustment(string transactionid,
                                                     string sStoreId, string sTerminalId, int adjustment)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            string commandText = string.Empty;

            //commandText = " UPDATE RETAILTRANSACTIONPAYMENTTRANS SET isAdjusted = '" + adjustment + "' WHERE TRANSACTIONID ='" + transactionid.Trim() + "' ";
            commandText += " UPDATE RETAILADJUSTMENTTABLE SET ISADJUSTED = '" + adjustment + "' WHERE TRANSACTIONID ='" + transactionid.Trim() + "' AND RETAILSTOREID = '" + sStoreId + "' AND RETAILTERMINALID ='" + sTerminalId + "' ";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            command.ExecuteNonQuery();

        }
        #endregion


        private bool IsTaxApplicable(SqlTransaction sqlTransaction)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" select TAXAPPLICABLE from RETAILPARAMETERS where  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;
            return Convert.ToBoolean(command.ExecuteScalar());
        }

        private void SaveConvertToAdvInLocal(IPosTransaction posTransaction, SqlTransaction sqlTransaction, int iAdvAgainst
            , string sCustId, string sOrderN, decimal dAmt, int iGoldFix
            , int iDepositeType, string sVouAgainst
            , decimal sRate, string sFixedConfigId
            , decimal dManualBookedWty, decimal dFixedRatePct, string sProductType, decimal dAdvPct, decimal dAdvTaxAmt, int iConvToAdv,decimal dCustBookQty,int iNoOfDays)
        {
            RetailTransaction retailTransaction = posTransaction as RetailTransaction;
            string sSqlString = "";
            decimal dGoldBookingRatePercentage = Convert.ToDecimal(retailTransaction.PartnerData.GoldBookingRatePercentage);// GetGoldBookingRatePer(sqlTransaction);

            sSqlString = SaveAdvanceDataIntoLocal(posTransaction,
                                                          sCustId,
                                                          sOrderN,
                                                          dAmt,
                                                          iGoldFix, sRate, iDepositeType,
                                                          sFixedConfigId, iAdvAgainst, sVouAgainst
                                                        , sSqlString, dManualBookedWty
                                                        , dFixedRatePct, sProductType, dGoldBookingRatePercentage
                                                        , dAdvPct, dAdvTaxAmt, iConvToAdv,dCustBookQty,iNoOfDays);

            using (SqlCommand sqlCmd = new SqlCommand(sSqlString, sqlTransaction.Connection, sqlTransaction))
            {
                sqlCmd.ExecuteNonQuery();
            }
        }

        private string SaveAdvanceDataIntoLocal(IPosTransaction posTransaction, int goldfixing, string commandString, decimal dGssTaxAmt)
        {
            #region INSERT INTO [RETAILDEPOSITTABLE]
            commandString += " INSERT INTO [RETAILDEPOSITTABLE] " +
                           " ([TRANSACTIONID],[CUSTACCOUNT],[ORDERNUM],[AMOUNT],[GOLDQUANTITY],[GOLDFIXING],[RATE] " +
                           " ,[DEPOSITTYPE],[STOREID],[TERMINALID],[STAFFID],[DATAAREAID],[CREATEDON] ";
            if (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType == "GSS")
            {
                commandString += ",[GSSNUMBER],[NOOFMONTHS] ";
            }
            commandString += ",FIXEDCONFIGID,ManualBookedQty,BookePct,ProductType,TAXPERCENTAGE,TAXAMOUNT";
            //============================Soutik
            commandString += ",CommitedQty,CommitedForDays";
            commandString += " ) " +
                           " VALUES " +
                           " ('" + posTransaction.TransactionId + "' " +
                           " ,'" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId + "' " +
                           " ,'" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo + "' " +
                           " ," + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount + " ";

            if (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType == "GSS")
                commandString += " , ISNULL(" + (Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.RateBookingAmt) - dGssTaxAmt) + "/@RATE,0) ";//.Amount
            else
                commandString += " , ISNULL(" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.RateBookingAmt + "/@RATE,0) ";//.Amount

            commandString += " ," + goldfixing + " " +
                       " ,isnull(@RATE,0) " +
                       " ," + Convert.ToInt16(Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone) ? (int)Enums.EnumClass.DepositType.GSS : (int)Enums.EnumClass.DepositType.Normal) + " " +
                       " ,'" + posTransaction.StoreId + "' " +
                       " ,'" + posTransaction.TerminalId + "' " +
                       " ,'" + posTransaction.OperatorId + "' " +
                       " ,'" + application.Settings.Database.DataAreaID + "' " +
                       " ,GETDATE() ";
            if (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType == "GSS")
            {
                commandString += ",'" + Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum) + "' ";
                commandString += "," + Convert.ToInt16(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths) + " ";
            }
            // commandString += "," + Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRate) + "";
            commandString += ",'" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedConfigId + "'";
            commandString += ",'" + Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ManualBookedQty) + "' ";
            commandString += ",'" + Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldBookingRatePercentage) + "' ";
            commandString += ",'" + Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ProductType) + "' ";
            commandString += ",'" + Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxPct) + "' ";
            commandString += ",'" + Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxAmt) + "' ";
            //commandString += ",'" + Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxAmt) + "'); ";
           //===========================Soutik
            commandString += ",'" + Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustAdvCommitedQty) + "' ";
            commandString += ",'" + Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustCommitedForDays) + "'); ";

            //((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxAmt = dTaxAmt;
            //((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxPct = dAdvTaxPerc;

            #endregion

            #region INSERT INTO [RETAILADJUSTMENTTABLE]
            if (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType != "GSS")
            {
                commandString += " INSERT INTO [RETAILADJUSTMENTTABLE] " +
                            " ([TRANSACTIONID],[CUSTACCOUNT],[ORDERNUM],[AMOUNT],[GOLDQUANTITY],[GOLDFIXING],[ISADJUSTED],[RATE] " +
                            " ,[RETAILDEPOSITTYPE],[RETAILSTOREID],[RETAILTERMINALID],[RETAILSTAFFID]," +
                            " [DATAAREAID],[ADVAGAINST],[VOUCHERAGAINST],FIXEDCONFIGID ,ManualBookedQty,BookePct,ProductType,TAXPERCENTAGE,TAXAMOUNT,CASHAMOUNT,CommitedQty,CommitedForDays) " +
                            " VALUES " +
                            " ('" + posTransaction.TransactionId + "' " +
                            " ,'" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId + "' " +
                            " ,'" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo + "' " +
                            " ," + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount + " ";
                // if(goldfixing == 1 && dOrdQty == 0)
                commandString += " , ISNULL(" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.RateBookingAmt + "/@RATE,0) "; //.Amount 
                // else
                //   commandString += " ," + dOrdQty + "";

                commandString += " ," + goldfixing + " " +
                            " ,0 " +
                            " ,isnull(@RATE,0) " +
                            " ," + Convert.ToInt16(Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone) ? (int)Enums.EnumClass.DepositType.GSS : (int)Enums.EnumClass.DepositType.Normal) + " " +
                            " ,'" + posTransaction.StoreId + "' " +
                            " ,'" + posTransaction.TerminalId + "' " +
                            " ,'" + posTransaction.OperatorId + "' " +
                            " ,'" + application.Settings.Database.DataAreaID + "' " +
                            " ,'" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ADVAGAINST + "'" +
                            " ,'" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.VOUCHERAGAINST + "'" +
                    //"," + Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRate) + ""+
                            ",'" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedConfigId + "'" +
                            ",'" + Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ManualBookedQty) + "' " +
                            ",'" + Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldBookingRatePercentage) + "' " +
                            ",'" + Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ProductType) + "' " +
                            ",'" + Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxPct) + "' " +
                            ",'" + Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxAmt) + "' " +
                            ",'" + Convert.ToString(dCashPayAmt) + "' " +
                            //==============================Soutik
                            ",'" + Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustAdvCommitedQty) + "' " +
                            ",'" + Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustCommitedForDays) + "' " +
                            " ); ";



            }
            #endregion
            return commandString;
        }

        private string SaveAdvanceDataIntoLocal(IPosTransaction posTransaction, string sCustId, string sOrderNo, decimal dAmt,
            int goldfixing, decimal dRate, int iDepositeType, string sFixConfig
            , int iVouAgainst, string sVouNoAgainst, string commandString
             , decimal dManualBookedWty, decimal dFixedRatePct, string sProductType,
            decimal dGoldBookingRatePercentage, decimal dAdvPct, decimal dAdvTaxAmt, int iIsConvToAdv,decimal dCommitedQty,int iCommitedForDays)
        {
            #region INSERT INTO [RETAILDEPOSITTABLE]
            decimal dRateBookingAmt = 0m;
            decimal dBookedQty = 0M;

            if (dGoldBookingRatePercentage > 0)
                dRateBookingAmt = dAmt / (dGoldBookingRatePercentage / 100);

            if (dRate > 0)
            {
                if (iVouAgainst == 0)
                    dBookedQty = dRateBookingAmt / dRate;
                else
                    dBookedQty = dAmt / dRate;
            }

            if (iIsConvToAdv == 1)
            {
                dAdvPct = 0;
                dAdvTaxAmt = 0;
            }

            commandString += " INSERT INTO [RETAILDEPOSITTABLE] " +
                           " ([TRANSACTIONID],[CUSTACCOUNT],[ORDERNUM],[AMOUNT],[GOLDQUANTITY],[GOLDFIXING],[RATE] " +
                           " ,[DEPOSITTYPE],[STOREID],[TERMINALID],[STAFFID],[DATAAREAID],[CREATEDON],FIXEDCONFIGID" +
                           " ,ManualBookedQty,BookePct,ProductType,TAXPERCENTAGE,TAXAMOUNT,CommitedQty,CommitedForDays) " +
                           " VALUES " +
                           " ('" + posTransaction.TransactionId + "' " +
                           " ,'" + sCustId + "' " +
                           " ,'" + sOrderNo + "' " +
                           " ," + dAmt + " " +
                           " ," + dBookedQty + " " +
                           " ," + goldfixing + " " +
                           " ,'" + dRate + "' " +
                           " ," + iDepositeType + " " +
                           " ,'" + posTransaction.StoreId + "' " +
                           " ,'" + posTransaction.TerminalId + "' " +
                           " ,'" + posTransaction.OperatorId + "' " +
                           " ,'" + application.Settings.Database.DataAreaID + "' " +
                           " ,GETDATE() " +
                           " ,'" + sFixConfig + "'" +
                            " ,'" + dManualBookedWty + "'" +
                            " ,'" + dFixedRatePct + "'" +
                            " ,'" + sProductType + "'" +
                            " ,'" + dAdvPct + "'" +
                            " ,'" + dAdvTaxAmt + "'" +
                            " ,'" + dCommitedQty + "'" +
                            " ,'" + iCommitedForDays + "'); ";

            #endregion


            #region INSERT INTO [RETAILADJUSTMENTTABLE]

            commandString += " INSERT INTO [RETAILADJUSTMENTTABLE] " +
                        " ([TRANSACTIONID],[CUSTACCOUNT],[ORDERNUM],[AMOUNT],[GOLDQUANTITY],[GOLDFIXING],[ISADJUSTED],[RATE] " +
                        " ,[RETAILDEPOSITTYPE],[RETAILSTOREID],[RETAILTERMINALID],[RETAILSTAFFID]," +
                        " [DATAAREAID],[ADVAGAINST],[VOUCHERAGAINST],FIXEDCONFIGID " +
                        " ,ManualBookedQty,BookePct,ProductType,TAXPERCENTAGE,TAXAMOUNT,ISCONVERTTOTADV,CASHAMOUNT,CommitedQty,CommitedForDays) " +
                        " VALUES " +
                        " ('" + posTransaction.TransactionId + "' " +
                       " ,'" + sCustId + "' " +
                       " ,'" + sOrderNo + "' " +
                       " ," + dAmt + " " +
                       " ," + dBookedQty + " " +
                       " ," + goldfixing + " " +
                       " ," + 0 + " " +
                       " ,'" + dRate + "' " +
                       " ," + iDepositeType + " " +
                       " ,'" + posTransaction.StoreId + "' " +
                       " ,'" + posTransaction.TerminalId + "' " +
                       " ,'" + posTransaction.OperatorId + "' " +
                        " ,'" + application.Settings.Database.DataAreaID + "' " +
                        " ,'" + iVouAgainst + "'" +
                        " ,'" + sVouNoAgainst + "'" +
                        " ,'" + sFixConfig + "'" +
                        " ,'" + dManualBookedWty + "'" +
                        " ,'" + dFixedRatePct + "'" +
                        " ,'" + sProductType + "'" +
                        " ,'" + dAdvPct + "'" +
                        " ,'" + dAdvTaxAmt + "'," + iIsConvToAdv + ",'" + dCashPayAmt + "','" + dCommitedQty + "','" + iCommitedForDays + "' ); ";

            #endregion
            return commandString;
        }

        private string GetCARDTYPEID(string sCardNo)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select top 1 CARDTYPEID  from RETAILTENDERTYPECARDNUMBERS where '" + sCardNo + "'");
            commandText.Append(" BETWEEN CARDNUMBERFROM AND CARDNUMBERTO  and LEN('" + sCardNo + "') = CARDNUMBERLENGTH ");

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
                return "";
        }

        #region Changed By Nimbus
        private string GetCustomerMobilePrimary(string sCustAcc)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT RETAILMOBILEPRIMARY FROM CUSTTABLE WHERE ACCOUNTNUM='" + sCustAcc + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        private void UpdateRETAILSUSPENDEDTRANSACTIONS(string sTerminalId, string sCustAcc, string sCustName, string sCustMobile, string sSusPendTransId)
        {
            SqlConnection conn = new SqlConnection();

            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();


            commandText.Append(" UPDATE RETAILSUSPENDEDTRANSACTIONS SET ");
            commandText.Append(" CUSTACCOUNT = '" + sCustAcc + "' ,CUSTNAME='" + sCustName + "',CUSTMOBILE='" + sCustMobile + "',RECEIPTID='" + sSusPendTransId + "' ");
            commandText.Append(" WHERE CAST(SUSPENDEDTRANSACTIONID AS bigint) ");
            commandText.Append(" =(select max(CAST(SUSPENDEDTRANSACTIONID AS bigint)) from RETAILSUSPENDEDTRANSACTIONS)");
            commandText.Append(" AND TERMINALID = '" + sTerminalId + "'; ");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private void CheckInventory(IPosTransaction posTransaction, int tType)
        {
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                {
                    if (!saleLineItem.Voided)
                    {
                        string commandtext = "  " +
                            //  " UPDATE SKUTable_Posted SET isLocked='False' WHERE SkuNumber='" + saleLineItem.ItemId + "' " +
                                  " UPDATE SKUTableTrans SET isLocked='False' WHERE SkuNumber='" + saleLineItem.ItemId + "' " + //SKU Table New
                            //" AND STOREID='" + ApplicationSettings.Terminal.StoreId + "' AND TERMINALID='" + ApplicationSettings.Terminal.TerminalId + "' " +
                                  " AND DATAAREAID='" + application.Settings.Database.DataAreaID + "'  " +
                                  " AND isAvailable='True' ";

                        SqlConnection connection = new SqlConnection();
                        if (application != null)
                            connection = application.Settings.Database.Connection;
                        else
                            connection = ApplicationSettings.Database.LocalConnection;
                        if (connection.State == ConnectionState.Closed)
                            connection.Open();
                        SqlCommand command = new SqlCommand(commandtext, connection);
                        command.CommandTimeout = 0;
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        #region [Exchange / Buy Back]
        // private string getGoldRate(string sItemId, string sConfigId, int iRateType, string StoreID)   
        private decimal getGoldRate(string sItemId, string sConfigId, int iRateType, string StoreID)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");

            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM     RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");

            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RATES,0) AS decimal (6,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION AND METALTYPE = ");
            commandText.Append(1);

            if (iRateType == 3)
                commandText.Append(" AND RETAIL=1 AND RATETYPE= ");
            else
                commandText.Append(" AND RATETYPE= ");

            commandText.Append(iRateType);

            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + sConfigId.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            decimal dGoldRate = Convert.ToDecimal(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dGoldRate;

        }

        private decimal GetIngredientInfo(ref decimal ExchangePercent, ref decimal BuyBackPercent, ref int CalcType,
                                           string StoreID,
                                          string ItemID, string SizeID, string ColorID, decimal dStoneWtRange, DateTime dtTransdate)
        {

            decimal dStoneRate = 0m;
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" SET DATEFORMAT DMY;   DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");

            //commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + ItemID.Trim() + "' ");

            //commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.LooseDmd + "','" + (int)MetalType.Stone + "')) ");
            //commandText.Append(" BEGIN ");


            // commandText.Append(" SELECT     ISNULL(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CALCTYPE,0) AS CALCTYPE,ISNULL(DISCTYPE,0) AS DISCTYPE, ISNULL(DISCAMT,0) AS DISCAMT ");

            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE,0) AS numeric(28, 2)) ");
            commandText.Append(" ,ISNULL(BUYBACKDEDUCTPCT,0) AS BUYBACKDEDUCTPCT, ISNULL(EXCHANGEDEDUCTPCT,0) AS EXCHANGEDEDUCTPCT,ISNULL(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CALCTYPE,0) AS CALCTYPE ");

            commandText.Append(" FROM    RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            //  commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + grweigh + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND (  ");
            commandText.Append(dStoneWtRange);
            commandText.Append(" BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            //  commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            // based on transaction date
            // commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, CAST('" + dtTransdate + "' AS DATETIME)), 0) BETWEEN  "); //dtTransdate
            // fetching diamond as per current rate
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  "); //dtTransdate
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "')  AND  ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID.Trim() + "') ");

            // commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "')");
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013
            //  commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");
            commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID DESC, RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");  // Changed order sequence on 03.06.2013 as per u.das

            //commandText.Append(" END ");
            // }
            //   commandText.Append("AND CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME)<=CAST(CONVERT(VARCHAR(8),GETDATE(),108) AS TIME)  ");

            //if (conn.State == ConnectionState.Closed)
            //    conn.Open();

            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dStoneRate = Convert.ToDecimal(reader.GetValue(0));
                    BuyBackPercent = Convert.ToDecimal(reader.GetValue(1));
                    ExchangePercent = Convert.ToDecimal(reader.GetValue(2));
                    CalcType = Convert.ToInt32(reader.GetValue(3));
                    //iSDisctype = Convert.ToInt32(reader.GetValue(1));
                    //dSDiscAmt = Convert.ToDecimal(reader.GetValue(2));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();
            return dStoneRate;
        }

        private int getMetalType(string sItemId, SqlTransaction sqlTransaction)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT ISNULL(METALTYPE,0) FROM INVENTTABLE WHERE ITEMID = '" + sItemId + "' AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "'  ");

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;

            int iMetalType = Convert.ToInt32(command.ExecuteScalar());
            return iMetalType;
        }

        private string getDefaultBatchId(string sItemId, SqlTransaction sqlTransaction)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT ISNULL(CRWDEFAULTBATCHID,'') FROM INVENTTABLE WHERE ITEMID = '" + sItemId + "' AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "'  ");

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;

            string sDefaultBatchId = Convert.ToString(command.ExecuteScalar());
            return sDefaultBatchId;
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
        #endregion

        private DataTable GetBookedInfo(string sOrderNo, SqlTransaction sqlTransaction)  // SKU allow
        {
            DataTable dt = new DataTable();
            string commandText = "SELECT ITEMID FROM CUSTORDER_DETAILS WHERE ORDERNUM = '" + sOrderNo + "' AND ISNULL(IsBookedSKU,0) = 1 "
                                 ;

            using (SqlCommand command = new SqlCommand(commandText, sqlTransaction.Connection, sqlTransaction))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);
            }

            return dt;
        }

        private void SaveBookedSKU(IPosTransaction POSTrans, string sOrderNo, string sSKUNo, SqlTransaction sqlTransaction)
        {
            LSRetailPosis.Transaction.CustomerPaymentTransaction PayTrans = POSTrans as LSRetailPosis.Transaction.CustomerPaymentTransaction;

            string commandText = " IF NOT EXISTS(SELECT TOP 1 * FROM RETAILCUSTOMERDEPOSITSKUDETAILS " +
                                   " WHERE ORDERNUMBER=@ORDERNUMBER " +
                                   " AND SKUNUMBER=@SKUNUMBER) BEGIN  " +
                                   " INSERT INTO [RETAILCUSTOMERDEPOSITSKUDETAILS] " +
                                   " ([TRANSID],[CUSTOMERID],[ORDERNUMBER],[SKUNUMBER],[SKUBOOKINGDATE],[SKURELEASEDDATE] " +
                                   " ,[SKUSALEDATE],[DELIVERED],[STOREID],[TERMINALID],[DATAAREAID],[STAFFID]) " +
                                   "  VALUES " +
                                   " (@TRANSID,@CUSTOMERID,@ORDERNUMBER,@SKUNUMBER,@SKUBOOKINGDATE,@SKURELEASEDDATE,@SKUSALEDATE " +
                                   " ,@DELIVERED,@STOREID,@TERMINALID,@DATAAREAID,@STAFFID) END ";

            using (SqlCommand command = new SqlCommand(commandText, sqlTransaction.Connection, sqlTransaction))
            {
                command.Parameters.Add("@TRANSID", SqlDbType.NVarChar, 20).Value = Convert.ToString(PayTrans.TransactionId);
                command.Parameters.Add("@CUSTOMERID", SqlDbType.NVarChar, 20).Value = Convert.ToString(PayTrans.Customer.CustomerId);
                command.Parameters.Add("@ORDERNUMBER", SqlDbType.NVarChar, 20).Value = sOrderNo;
                command.Parameters.Add("@SKUNUMBER", SqlDbType.NVarChar, 20).Value = sSKUNo;
                command.Parameters.Add("@SKUBOOKINGDATE", SqlDbType.Date).Value = Convert.ToDateTime(DateTime.Now).Date;
                command.Parameters.Add("@SKURELEASEDDATE", SqlDbType.DateTime, 20).Value = Convert.ToDateTime("1/1/1900 12:00:00 AM");
                command.Parameters.Add("@SKUSALEDATE", SqlDbType.DateTime, 60).Value = Convert.ToDateTime("1/1/1900 12:00:00 AM");
                command.Parameters.Add("@DELIVERED", SqlDbType.Bit).Value = false;
                command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Database.DATAAREAID;

                command.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 20).Value = Convert.ToString(PayTrans.OperatorId);

                command.ExecuteNonQuery();
            }

        }


        private DataTable GetCardIfo(string sTransactionId, SqlTransaction sqlTransaction = null)
        {
            string sTblName = "EXTNDCARDINFO" + ApplicationSettings.Terminal.TerminalId;

            DataTable dtCard = new DataTable();
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName + "')");
            // commandText.Append(" BEGIN  SELECT ISNULL(TRANSACTIONID,'') AS TRANSACTIONID, ISNULL(CARDNO,'') AS CARDNO, ISNULL(APPROVALCODE,'') AS APPROVALCODE FROM " + sTblName + " END");

            commandText.Append(" BEGIN  SELECT ISNULL(CARDNO,'') AS CARDNO, ISNULL(APPROVALCODE,'') AS APPROVALCODE,ISNULL(CARDEXPIRYMONTH,'') AS CARDEXPIRYMONTH, ");
            commandText.Append(" ISNULL(CARDEXPIRYYEAR,'') AS CARDEXPIRYYEAR FROM " + sTblName + " WHERE TRANSACTIONID = '" + sTransactionId + "' END");

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(commandText.ToString(), conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dtCard);
            }

            return dtCard;
        }

        private int GetFullExchngValidDay(string sStoreId)
        {
            int iValidDay = 0;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "SELECT ISNULL(FULLEXCHANGEVALIDDAY,0) FROM RETAILSTORETABLE WHERE STORENUMBER = '" + sStoreId + "' ";


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            iValidDay = Convert.ToInt32(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return iValidDay;
        }

        private decimal GetMaxInvoiceAmount(SqlTransaction sqlTransaction) // added on 23/05/2014 for five PAN  if invoiceamount > 20000000(example) then PAN enter.
        {
            string commandText = "SELECT ISNULL(MAXINVOICEAMOUNT,0) FROM RETAILPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;

            decimal dAmount = Convert.ToDecimal(command.ExecuteScalar());
            return dAmount;
        }

        private DataTable GetAdvDetailInfo(string sStringSql)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = sStringSql;

                DataTable CustAdvDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustAdvDt);
                return CustAdvDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private DataTable GetDataTableInfo(string sStringSql, SqlTransaction sqlTransaction)
        {
            try
            {
                SqlCommand command = new SqlCommand(sStringSql.ToString(), sqlTransaction.Connection, sqlTransaction);
                DataTable CustAdvDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(command);
                SqlDa.Fill(CustAdvDt);
                return CustAdvDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private bool IsBatchItem(string sItemId, SqlTransaction sqlTransaction)
        {

            string commandText = string.Empty;
            commandText = "select ISNULL(TRACKINGDIMENSIONGROUP,'') TRACKINGDIMENSIONGROUP from ECORESTRACKINGDIMENSIONGROUPFLDSETUP " +
                        " where TRACKINGDIMENSIONGROUP = (select TRACKINGDIMENSIONGROUP  from ECORESTRACKINGDIMENSIONGROUPITEM where ITEMID ='" + sItemId + "'" +
                        " and ITEMDATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "') and DIMENSIONFIELDID=2 and ISACTIVE=1";

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (!string.IsNullOrEmpty(sResult))
                return true;
            else
                return false;
        }

        private void GetValidBulkItemPcsAndQtyForTrans(ref decimal dPdsCWQTY,
                                                    ref decimal dQty, string sItemId,
                                                    string ConfigID,
                                                    string SizeID,
                                                    string ColorID,
                                                    string StyleID,
                                                    string sBatchId, SqlTransaction sqlTransaction)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select sum(PdsCWQty) as PDSCWQTY,sum(Qty) as QTY from BulkItemTransTable ");
            sbQuery.Append(" Where ItemId='" + sItemId + "'");
            sbQuery.Append(" and InventColorId='" + ColorID + "'");
            sbQuery.Append(" and ConfigId='" + ConfigID + "'");
            sbQuery.Append(" and InventSizeId='" + SizeID + "'");
            sbQuery.Append(" and InventStyleId='" + StyleID + "'");
            sbQuery.Append(" and InventBatchId='" + sBatchId + "'");
            sbQuery.Append(" group by ItemId,ConfigId,InventColorId,InventSizeId,InventStyleId,InventBatchId ");

            DataTable dtBI = new DataTable();
            SqlCommand command = new SqlCommand(sbQuery.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;

            SqlDataAdapter daGss = new SqlDataAdapter(command);
            daGss.Fill(dtBI);

            if (dtBI != null && dtBI.Rows.Count > 0)
            {
                dPdsCWQTY = Convert.ToDecimal(dtBI.Rows[0]["PDSCWQTY"]);
                dQty = Convert.ToDecimal(dtBI.Rows[0]["QTY"]);
            }

        }


        private bool IsCatchWtItem(string sItemId, SqlTransaction sqlTransaction)
        {
            bool bCatchWtItem = false;


            string commandText = "DECLARE @ISCATCHWT INT  SET @ISCATCHWT = 0" +
                                " IF EXISTS (SELECT ITEMID FROM pdscatchweightitem WHERE ITEMID = '" + sItemId + "')" +
                                 " BEGIN SET @ISCATCHWT = 1 END SELECT @ISCATCHWT";


            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);

            bCatchWtItem = Convert.ToBoolean(command.ExecuteScalar());
            return bCatchWtItem;
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

        private bool isSRToAdvance(int iTenderTypeId, SqlTransaction sqlTransaction)
        {
            string commandText = string.Empty;
            commandText = "DECLARE @CHANNEL bigint  SELECT @CHANNEL = ISNULL(RECID,0)" +
                          " FROM RETAILSTORETABLE WHERE STORENUMBER = '" + ApplicationSettings.Database.StoreID + "'" +
                          " SELECT SRTOADVANCE FROM RetailStoreTenderTypeTable WHERE TENDERTYPEID=" + iTenderTypeId + "" +
                          " AND CHANNEL = @CHANNEL ";

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;

            bool sResult = Convert.ToBoolean(command.ExecuteScalar());
            return sResult;
        }

        private string CustAdjusmentItemId(SqlTransaction sqlTransaction)
        {
            string commandText = string.Empty;
            commandText = "SELECT TOP(1) ADJUSTMENTITEMID FROM [RETAILPARAMETERS] WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;

            string sResult = sResult = Convert.ToString(command.ExecuteScalar());
            return sResult;
        }

        private string GetDefaultConfigId(SqlTransaction sqlTransaction)
        {
            string commandText = string.Empty;
            commandText = "SELECT TOP  1 DEFAULTCONFIGIDGOLD FROM [RETAILPARAMETERS] WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;

            return Convert.ToString(command.ExecuteScalar());
        }

        private string GetDefaultConfigId()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) DEFAULTCONFIGIDGOLD FROM [RETAILPARAMETERS] WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            string sResult = string.Empty;
            sResult = Convert.ToString(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return sResult;
        }

        private decimal GetDefaultAdvTaxPct(SqlTransaction sqlTransaction)
        {
            string commandText = string.Empty;
            commandText = "SELECT TOP  1 isnull(ADVANCETAXPERCENTAGE,0) FROM [RETAILPARAMETERS] WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;
            return Convert.ToDecimal(command.ExecuteScalar());
        }

        private void SaveBulkItemTransDetails(SqlTransaction sqlTransaction, RetailTransaction retailTransaction, SaleLineItem saleLineItem)
        {

            string commandString = " INSERT INTO [BulkItemTransTable]([TransReceiptId],[LineNumber]," +
                                      " [TransDate],[TransType],[ItemId],[ConfigId]," +
                                      " [InventSizeId],[InventColorId],[InventStyleId]," +
                                      " InventBatchId,[PdsCWQty],[Qty]," +
                                      " [RetailStaffId],[RetailStoreId]," +
                                      " [RetailTerminalId],DATAAREAID," +
                                      " [GrossWt],NetWt)" +
                                      " VALUES(@TransReceiptId  ,@LINENUM ," +
                                      " @TransDate,@TransType, @INVENTITEMID," +
                                      " @CONFIGID,@InventSizeId,@InventColorId,@InventStyleId," +
                                      " @InventBatchId,@PIECES,@QUANTITY,@RETAILSTAFFID," +
                                      " @STOREID,@TERMINALID, @DATAAREAID,@GrossWt,@NetWt) ";

            using (SqlCommand sqlCmd = new SqlCommand(commandString, sqlTransaction.Connection, sqlTransaction))
            {

                sqlCmd.Parameters.Add(new SqlParameter("@TransReceiptId", retailTransaction.ReceiptId));
                sqlCmd.Parameters.Add(new SqlParameter("@TransDate", DateTime.Now));


                decimal dGrossWt = 0m;
                decimal dNETWT = 0m;
                if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0
                                      || Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 2
                                      || Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 4) && (saleLineItem.ReturnLineId == 0)) //!retailTransaction.SaleIsReturnSale
                {
                    dGrossWt = Convert.ToDecimal(saleLineItem.PartnerData.OGCHANGEDGROSSWT);
                    dNETWT = Convert.ToDecimal(saleLineItem.PartnerData.NETWT);

                    if (dGrossWt == 0)
                        dGrossWt = Convert.ToDecimal(saleLineItem.PartnerData.Quantity);

                    if (dNETWT == 0)
                        dNETWT = Convert.ToDecimal(saleLineItem.PartnerData.Quantity);

                    sqlCmd.Parameters.Add(new SqlParameter("@PIECES", !string.IsNullOrEmpty(saleLineItem.PartnerData.Pieces) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.Pieces) * -1) : "0"));
                    sqlCmd.Parameters.Add(new SqlParameter("@QUANTITY", !string.IsNullOrEmpty(saleLineItem.PartnerData.Quantity) ? Convert.ToString(Convert.ToDecimal(saleLineItem.PartnerData.Quantity) * -1) : "0"));
                    sqlCmd.Parameters.Add(new SqlParameter("@GrossWt", Math.Abs(dGrossWt) * -1));
                    sqlCmd.Parameters.Add(new SqlParameter("@NetWt", Math.Abs(dNETWT) * -1));
                }
                else
                {
                    if ((saleLineItem.ReturnLineId == 0))
                    {
                        dGrossWt = Convert.ToDecimal(saleLineItem.PartnerData.OGCHANGEDGROSSWT);
                        dNETWT = Convert.ToDecimal(saleLineItem.PartnerData.NETWT);
                    }

                    if (dGrossWt == 0)
                        dGrossWt = Convert.ToDecimal(saleLineItem.PartnerData.Quantity);

                    if (dNETWT == 0)
                        dNETWT = Convert.ToDecimal(saleLineItem.PartnerData.Quantity);

                    sqlCmd.Parameters.Add(new SqlParameter("@PIECES", !string.IsNullOrEmpty(saleLineItem.PartnerData.Pieces) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Pieces))) : "0"));
                    sqlCmd.Parameters.Add(new SqlParameter("@QUANTITY", !string.IsNullOrEmpty(saleLineItem.PartnerData.Quantity) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Quantity))) : "0"));
                    sqlCmd.Parameters.Add(new SqlParameter("@GrossWt", Math.Abs(dGrossWt)));
                    sqlCmd.Parameters.Add(new SqlParameter("@NetWt", Math.Abs(dNETWT)));
                }

                if ((saleLineItem.ReturnLineId != 0)) //retailTransaction.SaleIsReturnSale == true
                    sqlCmd.Parameters.Add(new SqlParameter("@TransType", (int)BulkTransactionType.SalesReturn));
                else
                {
                    if (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0)
                        sqlCmd.Parameters.Add(new SqlParameter("@TransType", (int)BulkTransactionType.Sales));
                    else if (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 1)
                        sqlCmd.Parameters.Add(new SqlParameter("@TransType", (int)BulkTransactionType.OGPurchase));
                    else if (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 2)
                        sqlCmd.Parameters.Add(new SqlParameter("@TransType", (int)BulkTransactionType.OGPurchaseReturn));
                    else if (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 3)
                        sqlCmd.Parameters.Add(new SqlParameter("@TransType", (int)BulkTransactionType.OGExchange));
                    else if (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 4)
                        sqlCmd.Parameters.Add(new SqlParameter("@TransType", (int)BulkTransactionType.OGExchangeReturn));

                }
                sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));
                sqlCmd.Parameters.Add(new SqlParameter("@STOREID", retailTransaction.StoreId));
                sqlCmd.Parameters.Add(new SqlParameter("@TERMINALID", retailTransaction.TerminalId));
                sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONID", retailTransaction.TransactionId));
                sqlCmd.Parameters.Add(new SqlParameter("@RETAILSTAFFID", retailTransaction.OperatorId));

                sqlCmd.Parameters.Add(new SqlParameter("@LINENUM", saleLineItem.LineId));
                sqlCmd.Parameters.Add(new SqlParameter("@INVENTITEMID", saleLineItem.ItemId));
                sqlCmd.Parameters.Add(new SqlParameter("@CONFIGID", !string.IsNullOrEmpty(saleLineItem.Dimension.ConfigId) ? saleLineItem.Dimension.ConfigId : string.Empty));
                sqlCmd.Parameters.Add(new SqlParameter("@InventSizeId", !string.IsNullOrEmpty(saleLineItem.Dimension.SizeId) ? saleLineItem.Dimension.SizeId : string.Empty));
                sqlCmd.Parameters.Add(new SqlParameter("@InventColorId", !string.IsNullOrEmpty(saleLineItem.Dimension.ColorId) ? saleLineItem.Dimension.ColorId : string.Empty));
                sqlCmd.Parameters.Add(new SqlParameter("@InventStyleId", !string.IsNullOrEmpty(saleLineItem.Dimension.StyleId) ? saleLineItem.Dimension.StyleId : string.Empty));

                if ((saleLineItem.ReturnLineId != 0))
                {
                    if (IsBatchItem(saleLineItem.ItemId, sqlTransaction))
                    {
                        string cmdStr1 = string.Empty;

                        int iMetalType = getMetalType(saleLineItem.ItemId, sqlTransaction);
                        string sDefaultBatchId = getDefaultBatchId(saleLineItem.ItemId, sqlTransaction);
                        bool isOGItem = IsOGItemType(saleLineItem.ItemId, sqlTransaction);

                        string sBatchIdforOg = "";

                        if (isOGItem)
                            sBatchIdforOg = DateTime.Now.ToString("ddMMyyyy") + "" + ApplicationSettings.Database.StoreID;
                        else
                            sBatchIdforOg = retailTransaction.ReceiptId + "" + saleLineItem.LineId;

                        string sOldBatchId = "";
                        if (isOGItem)
                            sOldBatchId = DateTime.Now.ToString("ddMMyyyy") + "" + ApplicationSettings.Database.StoreID;
                        else
                            sOldBatchId = retailTransaction.ReceiptId + "" + saleLineItem.LineId;

                        string sBatchId = string.Empty;

                        if (iMetalType == (int)Enums.EnumClass.MetalType.Gold
                            || iMetalType == (int)Enums.EnumClass.MetalType.Silver
                            || iMetalType == (int)Enums.EnumClass.MetalType.Platinum
                            || iMetalType == (int)Enums.EnumClass.MetalType.Palladium)
                        {
                            if (string.IsNullOrEmpty(sDefaultBatchId))//added on 09/08/17
                                sBatchId = sBatchIdforOg;
                            else
                                sBatchId = sDefaultBatchId;
                        }
                        else
                            sBatchId = sOldBatchId;

                        //string sBatch = retailTransaction.ReceiptId + "" + saleLineItem.LineId + "";

                        if (!string.IsNullOrEmpty(saleLineItem.PartnerData.REPAIRBATCHID))//added on 28082018
                        {
                            sqlCmd.Parameters.Add(new SqlParameter("@InventBatchId", Convert.ToString(saleLineItem.PartnerData.REPAIRBATCHID)));
                        }
                        else
                        {
                            sqlCmd.Parameters.Add(new SqlParameter("@InventBatchId", sBatchId));
                        }

                    }
                    else
                        sqlCmd.Parameters.Add(new SqlParameter("@InventBatchId", string.Empty));
                }
                else
                {
                    if (string.IsNullOrEmpty(saleLineItem.PartnerData.BULKINVENTBATCHID))
                    {
                        if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 1 || Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 3))
                        {
                            string cmdStr1 = string.Empty;
                            if (IsBatchItem(saleLineItem.ItemId, sqlTransaction))
                            {
                                int iMetalType = getMetalType(saleLineItem.ItemId, sqlTransaction);

                                bool isOGItem = IsOGItemType(saleLineItem.ItemId, sqlTransaction);
                                string sBatchIdforOg = "";

                                if (isOGItem)
                                    sBatchIdforOg = DateTime.Now.ToString("ddMMyyyy") + "" + ApplicationSettings.Database.StoreID;
                                else
                                    sBatchIdforOg = retailTransaction.ReceiptId + "" + saleLineItem.LineId;

                                string sOldBatchId = "";
                                if (isOGItem)
                                    sOldBatchId = DateTime.Now.ToString("ddMMyyyy") + "" + ApplicationSettings.Database.StoreID;
                                else
                                    sOldBatchId = retailTransaction.ReceiptId + "" + saleLineItem.LineId;

                                string sBatchId = string.Empty;

                                if (iMetalType == (int)Enums.EnumClass.MetalType.Gold
                                   || iMetalType == (int)Enums.EnumClass.MetalType.Silver
                                   || iMetalType == (int)Enums.EnumClass.MetalType.Platinum
                                   || iMetalType == (int)Enums.EnumClass.MetalType.Palladium)
                                    sBatchId = sBatchIdforOg;
                                else
                                    sBatchId = sOldBatchId;

                                //string sBatch = retailTransaction.ReceiptId + "" + saleLineItem.LineId + "";
                                if (!string.IsNullOrEmpty(saleLineItem.PartnerData.REPAIRBATCHID))//added on 28082018
                                {
                                    sqlCmd.Parameters.Add(new SqlParameter("@InventBatchId", Convert.ToString(saleLineItem.PartnerData.REPAIRBATCHID)));
                                }
                                else
                                {
                                    sqlCmd.Parameters.Add(new SqlParameter("@InventBatchId", sBatchId));
                                }

                                // sqlCmd.Parameters.Add(new SqlParameter("@InventBatchId", sBatchId));
                            }
                        }
                        else if (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 2 || Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 4)
                        {
                            sqlCmd.Parameters.Add(new SqlParameter("@InventBatchId", !string.IsNullOrEmpty(saleLineItem.PartnerData.RETAILBATCHNO) ? saleLineItem.PartnerData.RETAILBATCHNO : string.Empty));
                        }
                        else
                            sqlCmd.Parameters.Add(new SqlParameter("@InventBatchId", ""));
                    }
                    else
                    {
                        sqlCmd.Parameters.Add(new SqlParameter("@InventBatchId", !string.IsNullOrEmpty(saleLineItem.PartnerData.BULKINVENTBATCHID) ? saleLineItem.PartnerData.BULKINVENTBATCHID : string.Empty));
                    }
                }

                sqlCmd.ExecuteNonQuery();
            }
        }

        private void UpdateVoidedData(SqlTransaction sqlTransaction, RetailTransaction retailTransaction, SaleLineItem saleLineItem)
        {

            string sSql = " UPDATE CUSTORDER_DETAILS SET isDelivered = 0 WHERE ORDERNUM=@ORDERNUM AND LINENUM=@ORDERLINENUM " +
                                                 " DECLARE @COUNT2 INT " +
                                                 " SELECT @COUNT2=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM=@ORDERNUM AND isDelivered = 0  " +
                                                 " IF(@COUNT2>0) " +
                                                 " BEGIN " +
                                                 " UPDATE CUSTORDER_HEADER SET isDelivered = 0 WHERE ORDERNUM=@ORDERNUM  " +
                                                 " END ";
            sSql = sSql + " UPDATE SKUTableTrans SET isAvailable=(CASE  WHEN isAvailable='False' THEN 'False' ELSE 'True' END),isLocked='False' WHERE SkuNumber=@INVENTITEMID " + //SKU Table New
                         " AND DATAAREAID=@DATAAREAID  ";

            using (SqlCommand sqlCmd = new SqlCommand(sSql, sqlTransaction.Connection, sqlTransaction))
            {

                sqlCmd.Parameters.Add(new SqlParameter("@ORDERNUM", !string.IsNullOrEmpty(saleLineItem.PartnerData.OrderNum) ? saleLineItem.PartnerData.OrderNum : string.Empty));
                sqlCmd.Parameters.Add(new SqlParameter("@ORDERLINENUM", !string.IsNullOrEmpty(saleLineItem.PartnerData.OrderLineNum) ? saleLineItem.PartnerData.OrderLineNum : "0"));
                sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));
                sqlCmd.Parameters.Add(new SqlParameter("@INVENTITEMID", saleLineItem.ItemId));

                sqlCmd.ExecuteNonQuery();
            }
        }

        private void OrderCancel(SqlTransaction sqlTransaction, string sOrderNo)
        {
            string sSql = " UPDATE CUSTORDER_HEADER SET IsConfirmed = 0 WHERE ORDERNUM=@ORDERNUM  ";

            using (SqlCommand sqlCmd = new SqlCommand(sSql, sqlTransaction.Connection, sqlTransaction))
            {
                sqlCmd.Parameters.Add(new SqlParameter("@ORDERNUM", sOrderNo));
                sqlCmd.ExecuteNonQuery();
            }
        }

        private void OrderConfirm(SqlTransaction sqlTransaction, string sOrderNo)
        {
            string sSql = " UPDATE CUSTORDER_HEADER SET IsConfirmed = 1 WHERE ORDERNUM=@ORDERNUM  ";

            using (SqlCommand sqlCmd = new SqlCommand(sSql, sqlTransaction.Connection, sqlTransaction))
            {
                sqlCmd.Parameters.Add(new SqlParameter("@ORDERNUM", sOrderNo));
                sqlCmd.ExecuteNonQuery();
            }
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
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bBulkItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bBulkItem;
        }

        private bool IsBulkItem(string sItemId, SqlTransaction sqlTransaction)
        {
            bool bBulkItem = false;

            string commandText = "DECLARE @ISBULK INT  SET @ISBULK = 0 IF EXISTS (SELECT ITEMID FROM INVENTTABLE WHERE ITEMID = '" + sItemId + "' AND RETAIL=0)" +
                                 " BEGIN SET @ISBULK = 1 END SELECT @ISBULK";


            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;

            bBulkItem = Convert.ToBoolean(command.ExecuteScalar());

            return bBulkItem;
        }

        private decimal GetIngredientsMetalRate(string sItemId, string sCongId, string sTransId,
                                                string sTerminalId, string sStoreId, SqlTransaction sqlTrans, ref decimal dQTY)
        {
            decimal dValue = 0;

            string commandText = "select   isnull(sum(isnull(SI.CValue,0)),0) CValue, " +
                                 " isnull(sum(isnull(QTY,0)),0) as QTY from RETAIL_SALE_INGREDIENTS_DETAILS SI" +//SI.CRate
                                 " left join INVENTTABLE I ON SI.SKUNUMBER = I.ITEMID" +
                                 " where SI.SKUNUMBER='" + sItemId + "' and SI.CONFIGID='" + sCongId + "'" +
                                 " and SI.TRANSACTIONID='" + sTransId + "' AND I.METALTYPE=" + (int)Enums.EnumClass.MetalType.Gold + "" +
                                 " and SI.TERMINALID='" + sTerminalId + "' and SI.STOREID='" + sStoreId + "'";


            SqlCommand command = new SqlCommand(commandText, sqlTrans.Connection, sqlTrans);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dValue = Convert.ToDecimal(reader.GetValue(0));
                    dQTY = Convert.ToDecimal(reader.GetValue(1));
                }
            }
            reader.Close();
            reader.Dispose();

            return dValue;
        }

        private string GetMetalRateForAdv()
        {
            string storeId = string.Empty;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            storeId = ApplicationSettings.Database.StoreID;
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @CONFIGID VARCHAR(20) ");
            commandText.Append(" DECLARE @RATE numeric(28, 3) ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE RETAILSTORETABLE.STORENUMBER='" + storeId + "' ");//INVENTPARAMETERS

            commandText.Append(" SELECT @CONFIGID = DEFAULTCONFIGIDGOLD FROM [RETAILPARAMETERS] WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
            commandText.Append(" SELECT TOP 1 CAST(RATES AS NUMERIC(28,2))AS RATES FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            commandText.Append(" AND METALTYPE = 1 AND ACTIVE=1 "); // METALTYPE -- > GOLD
            commandText.Append(" AND CONFIGIDSTANDARD=@CONFIGID  AND RATETYPE = 3 AND RETAIL=1"); // RATETYPE -- > GSS->Sale 4->3 on 10/06/2016 req by S.Sharma
            commandText.Append(" ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            return Convert.ToString(sResult.Trim());
        }

        private bool isMRP(string itemid)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) MRP FROM [INVENTTABLE] WHERE ITEMID='" + itemid + "' ");

            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), conn);
            return Convert.ToBoolean(cmd.ExecuteScalar());

        }

        private string GetSalesCounter(string sItemId, SqlTransaction sqlTransaction)
        {
            string commandText = string.Empty;
            commandText = "SELECT TOCOUNTER FROM SKUTableTrans where SkuNumber='" + sItemId + "'";

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            return sResult;
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
        //private bool IsBatchItem(string sItemId, SqlTransaction sqlTransaction)
        //{
        //    bool bBatchItem = false;
        //    string commandText = "select ISACTIVE from ECORESTRACKINGDIMENSIONGROUPITEM a " +
        //                         " left join ECORESTRACKINGDIMENSIONGROUPFLDSETUP b on a.TRACKINGDIMENSIONGROUP=b.TRACKINGDIMENSIONGROUP" +
        //                         " where ITEMID='" + sItemId + "' and b.ISACTIVE=1 " +
        //                         " and b.DIMENSIONFIELDID=2 and a.ITEMDATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";

        //    SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
        //    command.CommandTimeout = 0;

        //    bBatchItem = Convert.ToBoolean(command.ExecuteScalar());
        //    return bBatchItem;
        //}

        #region Adjustment Item Name
        private string AdjustmentItemID()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) ADJUSTMENTITEMID FROM [RETAILPARAMETERS] WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            string sResult = string.Empty;
            sResult = Convert.ToString(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return sResult;
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
        #endregion

        private bool IsOGItemType(string item, SqlTransaction sqlTrans) // for Malabar based on this new item type sales radio button or rest of the radio  button will be on/off
        {
            int iOWNDMD = 0;
            int iOWNOG = 0;
            int iOTHERDMD = 0;
            int iOTHEROG = 0;
            try
            {
                //if(conn.State == ConnectionState.Closed)
                //  conn.Open();
                string query = " SELECT TOP(1) OWNDMD,OWNOG,OTHERDMD,OTHEROG FROM INVENTTABLE WHERE ITEMID='" + item + "'";
                //SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
                SqlCommand cmd = new SqlCommand(query.ToString(), sqlTrans.Connection, sqlTrans);
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

            if (iOWNOG == 1 || iOTHEROG == 1)
                return true;
            else
                return false;
        }

        private string GetSalesPersonAtSalesTime(string sStoreId, string sTerminalId, string sTransId, string sLineNum, ref string sName)
        {
            string sResult = "";
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            SqlCon.Open();

            SqlCommand SqlComm = new SqlCommand();
            SqlComm.Connection = SqlCon;

            string commandText = "select A.STAFF,d.NAMEALIAS from RETAILTRANSACTIONSALESTRANS A" +
                                " LEFT JOIN dbo.RETAILSTAFFTABLE AS T11 on A.STAFF =T11.STAFFID " +
                                " LEFT JOIN dbo.HCMWORKER AS T22 ON T11.STAFFID = T22.PERSONNELNUMBER" +
                                " left join DIRPARTYTABLE d on t22.PERSON=d.RECID " +
                                " where TRANSACTIONID='" + sTransId + "'" +
                                " and STORE='" + sStoreId + "' and TERMINALID='" + sTerminalId + "' and LINENUM='" + sLineNum + "'";

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sResult = Convert.ToString(reader.GetValue(0));
                        sName = Convert.ToString(reader.GetValue(1));
                    }
                }
            }
            return sResult;
        }

        private bool isUCP(string itemid, SqlConnection connection)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) MRP FROM [INVENTTABLE] WHERE ITEMID='" + itemid + "' ");


            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());

        }

        private bool isRetailItem(string itemid, SqlConnection connection)
        {
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append(" DECLARE @ISVALID INT");
            sbQuery.Append(" IF EXISTS (SELECT ITEMID FROM INVENTTABLE WHERE ITEMID = '" + itemid + "'");
            sbQuery.Append(" AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "' AND RETAIL = 1) ");
            sbQuery.Append(" AND EXISTS (SELECT SkuNumber FROM SKUTableTrans WHERE SkuNumber = '" + itemid + "'");
            sbQuery.Append(" AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "')");
            sbQuery.Append(" BEGIN SET @ISVALID = 1 END ELSE BEGIN SET @ISVALID = 0 END SELECT ISNULL(@ISVALID,0)");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());

        }
        private void IsManualBookedQty(SqlTransaction sqlTransaction, ref bool isManual, ref decimal dRate)
        {
            string commandText = string.Empty;
            commandText = " SELECT ISNULL(ManualBookedQty,0),ISNULL(FIXEDRATEADVANCEPCT,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'";

            SqlCommand command = new SqlCommand(commandText.ToString(), sqlTransaction.Connection, sqlTransaction);
            command.CommandTimeout = 0;
            // bool sResult = Convert.ToBoolean(command.ExecuteScalar());


            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    isManual = Convert.ToBoolean(reader.GetValue(0));
                    dRate = Convert.ToDecimal(reader.GetValue(1));
                }
            }
            reader.Close();
        }

        private decimal GetGoldBookingRatePer(SqlTransaction sqlTransaction)
        {
            decimal dFixRatePct = 0M;

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append(" DECLARE @FIXEDRATEVAL AS NUMERIC(32,2)");
            sbQuery.Append(" DECLARE @ORDERQTY AS NUMERIC(32,2)");
            sbQuery.Append("SELECT ISNULL(FIXEDRATEADVANCEPCT,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'");


            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), sqlTransaction.Connection);
            dFixRatePct = Convert.ToDecimal(cmd.ExecuteScalar());

            return dFixRatePct;
        }

        private void GetItemType(string item) // for Malabar based on this new item type sales radio button or rest of the radio  button will be on/off
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            try
            {

                string query = " SELECT TOP(1) OWNDMD,OWNOG,OTHERDMD,OTHEROG FROM INVENTTABLE WHERE ITEMID='" + item + "'";

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

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

        private decimal getItemTaxPercentage(string sItemId)
        {
            SqlConnection connection = new SqlConnection();
            decimal sResult = 0m;

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            //commandText = "select ABS(CAST(ISNULL(TD.TAXVALUE,0)AS DECIMAL(28,2))) TAXVALUE from TAXDATA td," +
            //               " INVENTTABLEMODULE im,taxonitem it where it.TAXITEMGROUP=im.TAXITEMGROUPID and td.TAXCODE=it.taxcode and  im.moduletype=2 and im.ITEMID='" + sItemId + "'";


            commandText = "select (CAST(ISNULL(TD.TAXVALUE,0)AS DECIMAL(28,2))) TAXVALUE from TAXDATA td," +
                  " INVENTTABLEMODULE im,taxonitem it," +
                  " RETAILSTORETABLE rs, TAXGROUPDATA tg" +
                  " where it.TAXITEMGROUP=im.TAXITEMGROUPID " +
                  " and td.TAXCODE=it.taxcode and  im.moduletype=2 and im.ITEMID='" + sItemId + "'" +
                  " and rs.STORENUMBER='" + ApplicationSettings.Database.StoreID + "'" +
                  " and rs.TAXGROUP=tg.TAXGROUP" +
                  " and tg.TAXCODE=td.TAXCODE and tg.EXEMPTTAX=0";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            sResult = Convert.ToDecimal(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            return sResult;

        }

        private void GetSalesReturnPolicy(string sItemId, ref int iValidityDays, ref decimal dCashBack, ref decimal dConvToAdv, ref decimal dExchPct)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " DECLARE @Temp TABLE (EXCHANGEVALIDITYDAYS int, CASHBACKBUYBACK numeric(20,2)," +
                                 " CONVERTTOADVANCE  numeric(20,2),EXCHANGEPERCENTAGE  numeric(20,2));" +
                                 "  DECLARE @LOCATION VARCHAR(20) " +
                                 "  DECLARE @PRODUCT BIGINT " +
                                 "  DECLARE @PARENTITEM VARCHAR(20) " +
                                 "  DECLARE @CUSTCODE VARCHAR(20) " +

                                 "  DECLARE @PRODUCTCODE VARCHAR(20) " +
                                 "  DECLARE @COLLECTIONCODE VARCHAR(20) " +
                                 "  DECLARE @ARTICLE_CODE VARCHAR(20) " +

                                 "  IF EXISTS(SELECT TOP(1) ACTIVATE FROM CRWSALESRETURNPOLICY )" +
                                 "  IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "')" +
                                 "  BEGIN SELECT @PARENTITEM = ITEMIDPARENT,@PRODUCTCODE=PRODUCTTYPECODE" +
                                 "  ,@ARTICLE_CODE=ARTICLE_CODE , @COLLECTIONCODE =CollectionCode" +
                                 " FROM [INVENTTABLE] WHERE ITEMID = '" + sItemId + "' " +
                                 " END " +
                                 " ELSE" +
                                 " BEGIN" +
                                 " SET @PARENTITEM='" + sItemId + "' " +
                                 " END" +
                                 " IF EXISTS(SELECT TOP(1) ITEMID FROM  INVENTTABLE WHERE ITEMID='" + sItemId + "' AND RETAIL=0)" +
                                 " BEGIN " +
                                 " SET @PARENTITEM='" + sItemId + "'" +
                                 " END ";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //1
                " FROM   CRWSALESRETURNPOLICY  " +
                " WHERE (CRWSALESRETURNPOLICY.ITEMID=@PARENTITEM) " +
                " AND (CRWSALESRETURNPOLICY.ACCOUNTRELATION='" + ApplicationSettings.Database.StoreID + "' OR CRWSALESRETURNPOLICY.ACCOUNTRELATION='')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWSALESRETURNPOLICY.FROMDATE AND CRWSALESRETURNPOLICY.TODATE))" +
                     " BEGIN " +
                     " INSERT INTO @Temp (EXCHANGEVALIDITYDAYS, CASHBACKBUYBACK,CONVERTTOADVANCE,EXCHANGEPERCENTAGE)  " +
                     "     SELECT  TOP (1) EXCHANGEVALIDITYDAYS,CAST(CASHBACKBUYBACK AS numeric(20,2)) CASHBACKBUYBACK," +
                     "   CAST(CONVERTTOADVANCE AS numeric(20,2)) CONVERTTOADVANCE," +
                     "   CAST(EXCHANGEPERCENTAGE AS numeric(20,2)) EXCHANGEPERCENTAGE " +
                     "       FROM   CRWSALESRETURNPOLICY  WHERE     " +
                     "     (CRWSALESRETURNPOLICY.ITEMID=@PARENTITEM )" +

                     "  AND   (CRWSALESRETURNPOLICY.ProductCode=@ProductCode  or CRWSALESRETURNPOLICY.ProductCode='')" +
                     "  AND   (CRWSALESRETURNPOLICY.CollectionCode=@CollectionCode  or CRWSALESRETURNPOLICY.CollectionCode='')" +
                     "  AND   (CRWSALESRETURNPOLICY.ArticleCode=@ARTICLE_CODE  or CRWSALESRETURNPOLICY.ArticleCode='')" +

                     "     AND (CRWSALESRETURNPOLICY.ACCOUNTRELATION='" + ApplicationSettings.Database.StoreID + "' OR CRWSALESRETURNPOLICY.ACCOUNTRELATION='')" +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWSALESRETURNPOLICY.FROMDATE AND CRWSALESRETURNPOLICY.TODATE)   " +
                     "       AND CRWSALESRETURNPOLICY.ACTIVATE = 1" +
                     " ORDER BY CRWSALESRETURNPOLICY.ITEMID DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //2
               " FROM   CRWSALESRETURNPOLICY  " +
               " WHERE (CRWSALESRETURNPOLICY.ITEMID=@PARENTITEM) " +
               " AND (CRWSALESRETURNPOLICY.ACCOUNTRELATION='" + ApplicationSettings.Database.StoreID + "' OR CRWSALESRETURNPOLICY.ACCOUNTRELATION='')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWSALESRETURNPOLICY.FROMDATE AND CRWSALESRETURNPOLICY.TODATE))" +
                    " BEGIN " +
                    " INSERT INTO @Temp (EXCHANGEVALIDITYDAYS, CASHBACKBUYBACK,CONVERTTOADVANCE,EXCHANGEPERCENTAGE)  " +
                    "     SELECT  TOP (1) EXCHANGEVALIDITYDAYS,CAST(CASHBACKBUYBACK AS numeric(20,2)) CASHBACKBUYBACK," +
                     "   CAST(CONVERTTOADVANCE AS numeric(20,2)) CONVERTTOADVANCE," +
                     "   CAST(EXCHANGEPERCENTAGE AS numeric(20,2)) EXCHANGEPERCENTAGE FROM CRWSALESRETURNPOLICY   " +
                    "     WHERE     " +
                    "     (CRWSALESRETURNPOLICY.ITEMID=@PARENTITEM ) " +

                     "  AND   (CRWSALESRETURNPOLICY.ProductCode=@ProductCode  or CRWSALESRETURNPOLICY.ProductCode='')" +
                     "  AND   (CRWSALESRETURNPOLICY.CollectionCode=@CollectionCode  or CRWSALESRETURNPOLICY.CollectionCode='')" +
                     "  AND   (CRWSALESRETURNPOLICY.ArticleCode=@ARTICLE_CODE  or CRWSALESRETURNPOLICY.ArticleCode='')" +

                    "     AND (CRWSALESRETURNPOLICY.ACCOUNTRELATION='" + ApplicationSettings.Database.StoreID + "' OR CRWSALESRETURNPOLICY.ACCOUNTRELATION='')" +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWSALESRETURNPOLICY.FROMDATE AND CRWSALESRETURNPOLICY.TODATE)" +
                    "       AND CRWSALESRETURNPOLICY.ACTIVATE = 1" +
                    " ORDER BY CRWSALESRETURNPOLICY.ITEMID DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //3
                " FROM   CRWSALESRETURNPOLICY  " +
                " WHERE (CRWSALESRETURNPOLICY.ITEMID='') " +
                " AND (CRWSALESRETURNPOLICY.ACCOUNTRELATION='" + ApplicationSettings.Database.StoreID + "' OR CRWSALESRETURNPOLICY.ACCOUNTRELATION='')" +
                " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWSALESRETURNPOLICY.FROMDATE AND CRWSALESRETURNPOLICY.TODATE))" +
                     " BEGIN " +
                      " INSERT INTO @Temp (EXCHANGEVALIDITYDAYS, CASHBACKBUYBACK,CONVERTTOADVANCE,EXCHANGEPERCENTAGE)  " +
                     "     SELECT  TOP (1) EXCHANGEVALIDITYDAYS,CAST(CASHBACKBUYBACK AS numeric(20,2)) CASHBACKBUYBACK," +
                     "   CAST(CONVERTTOADVANCE AS numeric(20,2)) CONVERTTOADVANCE," +
                     "   CAST(EXCHANGEPERCENTAGE AS numeric(20,2)) EXCHANGEPERCENTAGE FROM CRWSALESRETURNPOLICY " +
                     "     WHERE     " +
                     "     (CRWSALESRETURNPOLICY.ITEMID='' ) " +

                     "   AND  (CRWSALESRETURNPOLICY.ProductCode=@ProductCode  or CRWSALESRETURNPOLICY.ProductCode='')" +
                     "   AND  (CRWSALESRETURNPOLICY.CollectionCode=@CollectionCode  or CRWSALESRETURNPOLICY.CollectionCode='')" +
                     "   AND  (CRWSALESRETURNPOLICY.ArticleCode=@ARTICLE_CODE  or CRWSALESRETURNPOLICY.ArticleCode='')" +

                     "     AND (CRWSALESRETURNPOLICY.ACCOUNTRELATION='" + ApplicationSettings.Database.StoreID + "' OR CRWSALESRETURNPOLICY.ACCOUNTRELATION='')" +
                     "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWSALESRETURNPOLICY.FROMDATE AND CRWSALESRETURNPOLICY.TODATE)   " +
                     "       AND CRWSALESRETURNPOLICY.ACTIVATE = 1" +
                     " ORDER BY CRWSALESRETURNPOLICY.ITEMID DESC END";

            commandText += " IF EXISTS ( SELECT  TOP (1) ACTIVATE" + //4
               " FROM   CRWSALESRETURNPOLICY  " +
               " WHERE (CRWSALESRETURNPOLICY.ITEMID='') " +
               " AND (CRWSALESRETURNPOLICY.ACCOUNTRELATION='" + ApplicationSettings.Database.StoreID + "' OR CRWSALESRETURNPOLICY.ACCOUNTRELATION='')" +
               " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWSALESRETURNPOLICY.FROMDATE AND CRWSALESRETURNPOLICY.TODATE))" +
                    " BEGIN " +
                     " INSERT INTO @Temp (EXCHANGEVALIDITYDAYS, CASHBACKBUYBACK,CONVERTTOADVANCE,EXCHANGEPERCENTAGE)  " +
                    "     SELECT  TOP (1) EXCHANGEVALIDITYDAYS,CAST(CASHBACKBUYBACK AS numeric(20,2)) CASHBACKBUYBACK," +
                     "   CAST(CONVERTTOADVANCE AS numeric(20,2)) CONVERTTOADVANCE," +
                     "   CAST(EXCHANGEPERCENTAGE AS numeric(20,2)) EXCHANGEPERCENTAGE FROM CRWSALESRETURNPOLICY   " +
                    "     WHERE     " +
                    "     (CRWSALESRETURNPOLICY.ITEMID='' ) " +

                     "   AND (CRWSALESRETURNPOLICY.ProductCode=@ProductCode  or CRWSALESRETURNPOLICY.ProductCode='')" +
                     "   AND  (CRWSALESRETURNPOLICY.CollectionCode=@CollectionCode  or CRWSALESRETURNPOLICY.CollectionCode='')" +
                     "   AND  (CRWSALESRETURNPOLICY.ArticleCode=@ARTICLE_CODE  or CRWSALESRETURNPOLICY.ArticleCode='')" +

                    "     AND (CRWSALESRETURNPOLICY.ACCOUNTRELATION='" + ApplicationSettings.Database.StoreID + "' OR CRWSALESRETURNPOLICY.ACCOUNTRELATION='')" +
                    "       AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN CRWSALESRETURNPOLICY.FROMDATE AND CRWSALESRETURNPOLICY.TODATE)   " +
                    "       AND CRWSALESRETURNPOLICY.ACTIVATE = 1" +
                    " ORDER BY CRWSALESRETURNPOLICY.ITEMID DESC END";
            commandText += " select TOP 1 * from @Temp";

            SqlCommand commandMk = new SqlCommand(commandText.ToString(), connection);
            commandMk.CommandTimeout = 0;

            using (SqlDataReader salesReturnPolicyRd = commandMk.ExecuteReader())
            {
                while (salesReturnPolicyRd.Read())
                {
                    iValidityDays = Convert.ToInt16(salesReturnPolicyRd.GetValue(0));
                    dCashBack = Convert.ToDecimal(salesReturnPolicyRd.GetValue(1));
                    dConvToAdv = Convert.ToDecimal(salesReturnPolicyRd.GetValue(2));
                    dExchPct = Convert.ToDecimal(salesReturnPolicyRd.GetValue(3));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        #region Print Customer order Voucher
        public void PrintVoucher(string sOrderNo, Int16 iCopy, RetailTransaction retailTransaction = null)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string sCompName = string.Empty;
            string sStoreName = string.Empty;
            string sStoreAddress = string.Empty;
            string sStorePhNo = string.Empty;
            string sGSTNo = string.Empty;
            string sStoreTaxState = "";
            string sInvoiceTime = "";


            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
            if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);
            if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);

            if (Convert.ToString(ApplicationSettings.Terminal.IndiaGSTRegistrationNumber) != string.Empty)
                sGSTNo = Convert.ToString(ApplicationSettings.Terminal.IndiaGSTRegistrationNumber);

            string sCompanyName = GetCompanyName();
            //-------

            if (Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState) != string.Empty)
                sStoreTaxState = Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState);

            if (retailTransaction != null)
                sInvoiceTime = (retailTransaction).BeginDateTime.ToString("HH:mm");

            sCompName = GetCompanyName();
            //datasources
            List<ReportDataSource> rds = new List<ReportDataSource>();
            rds.Add(new ReportDataSource("HEADERINFO", (DataTable)GetHeaderInfo(sOrderNo)));
            rds.Add(new ReportDataSource("DETAILINFO", (DataTable)GetDetailInfo(sOrderNo)));
            rds.Add(new ReportDataSource("CUSTORDINGR", (DataTable)GetCustOrderIngr(sOrderNo)));
            // string sAmtinwds = Amtinwds(Math.Abs(Convert.ToDouble(getTotAmtOfOrder(sOrderNo)))); // added on 28/04/2014 RHossain               
            BlankOperations.BlankOperations objBlankOpe = new BlankOperations.BlankOperations();
            string sAmtinwds = objBlankOpe.Amtinwds(Math.Abs(Convert.ToDouble(getTotAmtOfOrder(sOrderNo))));

            //parameters
            List<ReportParameter> rps = new List<ReportParameter>();
            rps.Add(new ReportParameter("Title", "Customer Order Voucher", true));
            rps.Add(new ReportParameter("StorePhone", string.IsNullOrEmpty(ApplicationSettings.Terminal.StorePhone) ? " " : ApplicationSettings.Terminal.StorePhone, true));
            rps.Add(new ReportParameter("StoreAddress", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreAddress) ? " " : ApplicationSettings.Terminal.StoreAddress, true));
            rps.Add(new ReportParameter("StoreName", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreName) ? " " : ApplicationSettings.Terminal.StoreName, true));
            rps.Add(new ReportParameter("GSTNo", sGSTNo, true));
            rps.Add(new ReportParameter("CompName", sCompanyName, true));
            rps.Add(new ReportParameter("Amtinwds", sAmtinwds, true));

            string reportName = @"rptCustOrdVoucher";
            string reportPath = @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";
            RdlcViewer rptView = new RdlcViewer("Customer Order Voucher", reportPath, rds, rps, null);
            rptView.ShowDialog();

            // Export(reportName.);
        }

        private string GetCompanyName()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string sCName = string.Empty;

            string sQry = "SELECT ISNULL(A.NAME,'') FROM DIRPARTYTABLE A INNER JOIN COMPANYINFO B" +
                " ON A.RECID = B.RECID WHERE B.DATAAREA = '" + ApplicationSettings.Database.DATAAREAID + "'";

            //using (SqlCommand cmd = new SqlCommand(sQry, conn))
            //{
            SqlCommand cmd = new SqlCommand(sQry, connection);
            cmd.CommandTimeout = 0;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            sCName = Convert.ToString(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            //}

            return sCName;

        }

        private string getTotAmtOfOrder(string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string sCName = string.Empty;

            string sQry = "SELECT SUM(AMOUNT + MAKINGAMOUNT) FROM CUSTORDER_DETAILS WHERE ORDERNUM='" + sOrderNo + "'";

            SqlCommand cmd = new SqlCommand(sQry, connection);
            cmd.CommandTimeout = 0;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            sCName = Convert.ToString(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return sCName;
        }



        //dd
        public DataTable GetHeaderInfo(string sOrderNo)
        {
            try
            {
                string sCustOrderReceiptDate = "";
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "select ORDERNUM as ORDERNO,CONVERT(VARCHAR(15),ORDERDATE,103) ORDERDATE,CONVERT(VARCHAR(15),DELIVERYDATE,103) DELIVERYDATE,CUSTACCOUNT as CUSTID,CUSTNAME," +
                                      " CUSTADDRESS as CUSTADD,CUSTPHONE,CAST(ISNULL(TOTALAMOUNT,0)AS DECIMAL(18,2)) AS TOTALAMOUNT FROM CUSTORDER_HEADER" +
                                      " WHERE ORDERNUM='" + sOrderNo + "'";

                DataTable CustBalDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustBalDt);

                if (CustBalDt.Rows.Count > 0)
                    sCustOrderReceiptDate = Convert.ToDateTime(CustBalDt.Rows[0]["ORDERDATE"]).ToString("dd-MM-yyyy");

                return CustBalDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //
        public DataTable GetDetailInfo(string sOrderNo)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "select A.ITEMID as SKUID, A.ITEMID + '-' + F.NAME as ITEMID,PCS,QTY,CRate as RATE,AMOUNT,MAKINGRATE,MAKINGAMOUNT," +
                                      " LineTotalAmt as TOTALAMOUNT,REMARKSDTL as REMARKS,IsBookedSKU as IsBooked,A.CONFIGID,A.CODE,A.SIZEID,A.WastageAmount " +
                                      " AS WastageAmt FROM CUSTORDER_DETAILS A" +//, CONVERT(VARCHAR(11)DELIVERYDATE,103) as DELIVERYDATE
                                      " INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID " +
                                      " LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID " +
                                      " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT and F.LANGUAGEID='en-us'" +
                                      " WHERE ORDERNUM='" + sOrderNo + "'"; // SKUID for get the itemid for image selection of parent item id of that item

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

        private DataTable GetCustOrderIngr(string sOrderNo)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                SqlComm.CommandText = " SELECT A.ITEMID + '-' + F.NAME as ITEMID ,PCS,QTY ,CRATE,AMOUNT,A.CONFIGID,A.CODE,A.SIZEID    FROM CUSTORDER_SUBDETAILS A" +
                                     " INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID " +
                                     " LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID " +
                                     " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT" +
                                     " where ORDERNUM='" + sOrderNo + "' ORDER BY ORDERNUM, LINENUM";

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


        private static DataTable GetWithOutAdvCustomerOrder(string sCustAcc)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                SqlComm.CommandText = " select A.ORDERNUM OrderNum,A.CUSTACCOUNT as CustAccount" +
                                      " ,A.CUSTNAME as CustomerName," +
                                      " Sum(B.AMOUNT) as Amount,Sum(B.QTY) as GoldQuantity from CUSTORDER_HEADER A" +
                                      " left join CUSTORDER_DETAILS B on A.ORDERNUM=B.ORDERNUM" +
                                      " where a.ISFIXEDQTY=0 and a.isDelivered=0 and b.isDelivered=0 " +
                                      " and a.WITHADVANCE=0 and a.CUSTACCOUNT='" + sCustAcc + "'" +
                                      " group by A.ORDERNUM,A.CUSTACCOUNT,A.CUSTNAME";

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

        #endregion

        #endregion
        //End:Nim

        #region Constructor - Destructor

        public TransactionTriggers()
        {

            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for TransactionTriggers are reserved at 50300 - 50349
        }

        #endregion

        public void BeginTransaction(IPosTransaction posTransaction)
        {
            #region Changed By Nimbus

            RetailTransaction retailTrans = posTransaction as RetailTransaction;

            if (retailTrans != null)
            {
                retailTrans.PartnerData.AdjustmentOrderNum = string.Empty;
                retailTrans.PartnerData.AdjustmentCustAccount = string.Empty;
                retailTrans.PartnerData.SKUBookedItems = false;
                retailTrans.PartnerData.SKUBookedItemsExists = "N";
                retailTrans.PartnerData.ItemIsReturnItem = false;
                retailTrans.PartnerData.IsGSSMaturity = false;
                retailTrans.PartnerData.SaleAdjustment = false;
                retailTrans.PartnerData.EFTCardNo = string.Empty;
                retailTrans.PartnerData.LCCustomerName = string.Empty;
                retailTrans.PartnerData.PackingMaterial = string.Empty;
                retailTrans.PartnerData.CertificateIssue = string.Empty;
                retailTrans.PartnerData.FreeGiftCWQTY = string.Empty;
                retailTrans.PartnerData.FreeGiftQTY = string.Empty;
                retailTrans.PartnerData.FreeGiftPromoCode = string.Empty;
                retailTrans.PartnerData.FREEGIFTCON = string.Empty;
                retailTrans.PartnerData.ExpRepairBatchId = string.Empty;
                retailTrans.PartnerData.IsRepairReturn = false;
                retailTrans.PartnerData.REPAIRRETCHARGES = string.Empty;
                retailTrans.PartnerData.RepairNettWtDiff = string.Empty;
                //retailTrans.PartnerData.REPAIRIDFORRETURN = string.Empty;
                retailTrans.PartnerData.INVBATCHIDFORREPAIR = string.Empty;
                retailTrans.PartnerData.REPAIROGRETURN = string.Empty;

                retailTrans.PartnerData.REPAIRID = string.Empty;
                retailTrans.PartnerData.RefRepairId = string.Empty;
                retailTrans.PartnerData.dSaleAdjustmentAvgGoldRate = "0";
                retailTrans.PartnerData.isDoneConvertAdvance = false;
                retailTrans.PartnerData.CustOrderWithAdv = false;
                retailTrans.PartnerData.dCAdvAdjustmentAvgGoldRate = "0";
                retailTrans.PartnerData.ReCalculate = false;
                retailTrans.PartnerData.OFFLINECUSTID = "";
                retailTrans.PartnerData.SearchCustomer = true;
                retailTrans.PartnerData.TransAdjGoldQty = "0";
                retailTrans.PartnerData.RunningQtyAdjustment = "0";
                retailTrans.PartnerData.LOYALTYASKINGDONE = false;
                retailTrans.PartnerData.LOYALTYTYPECODE = "";
                retailTrans.PartnerData.LoyaltyProvider = "";
                retailTrans.PartnerData.LOYALTYCARDNO = "";
                retailTrans.PartnerData.GoldBookingRatePercentage = "0";
                retailTrans.PartnerData.GSSMaturityNo = "";
                retailTrans.PartnerData.TotReturnGoldQty = "0";
                retailTrans.PartnerData.TotReturnAmt = "0";
                retailTrans.PartnerData.SingaporeTaxCal = "0";
                retailTrans.PartnerData.CashReturnWithOGTax = "0";
                retailTrans.PartnerData.IsAddToGiftCard = "0";
                retailTrans.PartnerData.IsGiftCardIssue = "0";
                retailTrans.PartnerData.SKUAgingDisc = false;
                retailTrans.PartnerData.SKUAgingDiscDone = false;
                retailTrans.PartnerData.TierDisc = false;
                retailTrans.PartnerData.TierDiscDone = false;
                retailTrans.PartnerData.GENERALDISC = false;
                retailTrans.PartnerData.SchemeDepositeType = "0";
                retailTrans.PartnerData.APPLYGSSDISC = false;
                retailTrans.PartnerData.APPLYGSSDISCDONE = false;
                retailTrans.PartnerData.GSSSchemeCode = "";
                retailTrans.PartnerData.SalesAssociateCode = "";// apollo
                retailTrans.PartnerData.BussinesAgentCode = "";// apollo
                retailTrans.PartnerData.ExtraMkDiscPct = "0";
                retailTrans.PartnerData.APPLYFLATMKDISCPCT = false;
                retailTrans.PartnerData.FlatMkDiscPct = "0";

                retailTrans.PartnerData.MakStnDiaDisc = false;
                retailTrans.PartnerData.MakStnDiaDiscDone = false;

                //=======================Soutik===============================
                retailTrans.PartnerData.CustAdvCommitedQty = 0;
                retailTrans.PartnerData.CustCommitedForDays = 0;
                retailTrans.PartnerData.AdvAgreemetDisc = false;
                retailTrans.PartnerData.AdvAgreemetDiscDone = false;
                retailTrans.PartnerData.CustDepositDate = "";
                retailTrans.PartnerData.CustAdvCommitedQty = 0;
                retailTrans.PartnerData.CustCommitedForDays = 0;
                retailTrans.PartnerData.AutoGenCommitedQty = 0;
                //============================================================

                retailTrans.PartnerData.GMAAdjustment = false;
                retailTrans.PartnerData.GMATotAdvance = "0";
                retailTrans.PartnerData.GMALossGainTotAmt = "0";
                retailTrans.PartnerData.GMADiscAmt = "0";
                retailTrans.PartnerData.GMAGSTAmt = "0";
                retailTrans.PartnerData.APPLYGMADISC = false;
                retailTrans.PartnerData.APPLYGMADISCDONE = false;


                if (string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventLocationId))
                    && string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventSiteId)))
                {
                    string commandtext = " SELECT     RETAILCHANNELTABLE.INVENTLOCATION, INVENTLOCATION.INVENTSITEID  " +
                                     " FROM         RETAILCHANNELTABLE INNER JOIN " +
                                     " INVENTLOCATION ON RETAILCHANNELTABLE.INVENTLOCATION = INVENTLOCATION.INVENTLOCATIONID INNER JOIN " +
                                     " RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID " +
                                     " WHERE     (RETAILSTORETABLE.STORENUMBER = '" + posTransaction.StoreId + "') ";

                    SqlConnection connection = new SqlConnection();
                    if (application != null)
                        connection = application.Settings.Database.Connection;
                    else
                        connection = ApplicationSettings.Database.LocalConnection;
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    SqlCommand command = new SqlCommand(commandtext, connection);
                    command.CommandTimeout = 0;
                    SqlDataReader reader = command.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            ((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventLocationId = Convert.ToString(reader.GetValue(0));
                            ((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventSiteId = Convert.ToString(reader.GetValue(1));
                        }
                    }
                    if (connection.State == ConnectionState.Open)
                        connection.Close();

                }
            }

            #endregion

            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.BeginTransaction", "When starting the transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        /// <summary>
        /// Triggered during save of a transaction to database.
        /// </summary>
        /// <param name="posTransaction">PosTransaction object.</param>
        /// <param name="sqlTransaction">SqlTransaction object.</param>
        /// <remarks>
        /// Use provided sqlTransaction to write to the DB. Don't commit, rollback transaction or close the connection.
        /// Any exception thrown from this trigger will rollback the saving of pos transaction.
        /// </remarks>
        public void SaveTransaction(IPosTransaction posTransaction, SqlTransaction sqlTransaction)
        {
            ApplicationLog.Log("TransactionTriggers.SaveTransaction", "Saving a transaction.", LogTraceLevel.Trace);

            //Example:
            //if (posTransaction is IRetailTransaction)
            //{
            //    string commandString = "INSERT INTO PARTNER_CUSTOMTRANSACTIONTABLE VALUES (@VAL1)";

            //    using (SqlCommand sqlCmd = new SqlCommand(commandString, sqlTransaction.Connection, sqlTransaction))
            //    {
            //        sqlCmd.Parameters.Add(new SqlParameter("@VAL1", posTransaction.PartnerData.TestData));
            //        sqlCmd.ExecuteNonQuery();
            //    }
            //}        

            //Start: Nim
            string sTestErrorSqlString = "";

            #region Saving Custom Fields to DB - CHANGED BY NIMBUS
            try
            {
                RetailTransaction retailTransaction = posTransaction as RetailTransaction;

                //if (retailTransaction != null && sqlTransaction != null)
                if (retailTransaction != null && sqlTransaction != null
                       && retailTransaction.IncomeExpenseAmounts == 0
                       && retailTransaction.PartnerData.IsAddToGiftCard == "0"
                       && retailTransaction.PartnerData.IsGiftCardIssue == "0")
                {
                    // START : DEV ON :22/05/2014
                    decimal dBulkPdsQty = 0m;
                    decimal dBulkQty = 0m;

                    if (retailTransaction.TenderLines.Count > 0)
                    {
                        foreach (ITenderLineItem tenderItem in retailTransaction.TenderLines)
                        {
                            if (tenderItem.Amount > 0 && !tenderItem.Voided)
                            {
                                if (Convert.ToInt16(tenderItem.TenderTypeId) == 1)
                                {
                                    dCashPayAmt += tenderItem.Amount;
                                }
                                //bAdvRefund = isSRToAdvance(Convert.ToInt16(tenderItem.TenderTypeId), sqlTransaction);   
                            }
                        }
                    }

                    if (retailTransaction.PartnerData.SaleAdjustment == true
                        || retailTransaction.PartnerData.IsGSSMaturity == true)
                    {
                        foreach (SaleLineItem SLineItem in retailTransaction.SaleItems)
                        {
                            if (SLineItem.ItemType != LSRetailPosis.Transaction.Line.GiftCertificateItem.GiftCertificateItem.ItemTypes.Item)
                            {
                                #region ADVANCE ADJUSTMENT WITH OUT ANY SALES ITEM
                                if (retailTransaction.IncomeExpenseAmounts == 0
                                    && retailTransaction.SalesInvoiceAmounts == 0)
                                {
                                    int iExistSalableItem1 = 0;

                                    foreach (SaleLineItem saleLineItem1 in retailTransaction.SaleItems)
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

                                    if (iExistSalableItem1 == 0)
                                    {
                                        bAdvRefund = true;
                                    }
                                }
                                #endregion
                            }
                        }
                    }


                    if (retailTransaction.SalesInvoiceAmounts > 0)
                    {
                        return;
                    }

                    if (retailTransaction.PartnerData.ExpRepairBatchId == "")
                    {
                        if (retailTransaction.IncomeExpenseAmounts != 0)
                        {
                            return;
                        }
                    }


                    #region Bulk item stock checking

                    DataTable dtItemInfo = new DataTable("dtItemInfo");
                    //if(retailTransaction.SaleIsReturnSale == false)
                    //{
                    foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                    {
                        if (saleLineItem.ReturnLineId == 0)
                        {
                            if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided
                                && retailTransaction.IncomeExpenseAmounts == 0)
                            {
                                if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && !saleLineItem.Voided)
                                {
                                    if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0)
                                        || (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 2)
                                        || (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 4))
                                    {
                                        string sBatch = "";
                                        if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 2 || Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 4))
                                        {
                                            sBatch = saleLineItem.PartnerData.RETAILBATCHNO;
                                        }
                                        else if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 1 || Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 3))
                                        {
                                            sBatch = retailTransaction.ReceiptId + "" + saleLineItem.LineId;
                                        }
                                        else if (!string.IsNullOrEmpty(saleLineItem.PartnerData.BULKINVENTBATCHID))
                                        {
                                            sBatch = saleLineItem.PartnerData.BULKINVENTBATCHID;
                                        }
                                        else if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.REPAIRINVENTBATCHID)))
                                        {
                                            sBatch = saleLineItem.PartnerData.REPAIRINVENTBATCHID;
                                        }


                                        if (IsBulkItem(saleLineItem.ItemId, sqlTransaction))
                                        {
                                            saleLineItem.PartnerData.BulkItem = 1;

                                            DataTable dtGroup = new DataTable();
                                            bool IsEdit = false;
                                            DataRow dr;
                                            if (IsEdit == false && dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                                            {
                                                dtItemInfo.Columns.Add("ITEMID", typeof(string));
                                                dtItemInfo.Columns.Add("CONFIGURATION", typeof(string));
                                                dtItemInfo.Columns.Add("COLOR", typeof(string));
                                                dtItemInfo.Columns.Add("SIZE", typeof(string));
                                                dtItemInfo.Columns.Add("STYLE", typeof(string));
                                                dtItemInfo.Columns.Add("BATCH", typeof(string));
                                                dtItemInfo.Columns.Add("PCS", typeof(decimal));
                                                dtItemInfo.Columns.Add("QUANTITY", typeof(decimal));
                                                //dtItemInfo.Columns.Add("GROSSWT", typeof(decimal));
                                                //dtItemInfo.Columns.Add("NETWT", typeof(decimal));
                                            }

                                            if (IsEdit == false)
                                            {
                                                dr = dtItemInfo.NewRow();
                                                dr["ITEMID"] = Convert.ToString(saleLineItem.ItemId);
                                                dr["CONFIGURATION"] = Convert.ToString(saleLineItem.Dimension.ConfigId);
                                                dr["COLOR"] = Convert.ToString(saleLineItem.Dimension.ColorId);
                                                dr["STYLE"] = Convert.ToString(saleLineItem.Dimension.StyleId);
                                                dr["SIZE"] = Convert.ToString(saleLineItem.Dimension.SizeId);
                                                dr["BATCH"] = sBatch;
                                                decimal dPcs = 0m;

                                                if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Pieces)))
                                                    dPcs = Convert.ToDecimal(saleLineItem.PartnerData.Pieces);
                                                else
                                                    dPcs = 0;

                                                dr["PCS"] = dPcs;

                                                if (!string.IsNullOrEmpty(saleLineItem.PartnerData.Quantity))
                                                    dr["QUANTITY"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.Quantity), 3, MidpointRounding.AwayFromZero);
                                                else
                                                    dr["QUANTITY"] = 0;

                                                //if (!string.IsNullOrEmpty(saleLineItem.PartnerData.OGCHANGEDGROSSWT))
                                                //    dr["GROSSWT"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.OGCHANGEDGROSSWT), 3, MidpointRounding.AwayFromZero);
                                                //else
                                                //    dr["GROSSWT"] = 0;

                                                //if (!string.IsNullOrEmpty(saleLineItem.PartnerData.NETWT))
                                                //    dr["NETWT"] = decimal.Round(Convert.ToDecimal(saleLineItem.PartnerData.NETWT), 3, MidpointRounding.AwayFromZero);
                                                //else
                                                //    dr["NETWT"] = 0;



                                                dtItemInfo.Rows.Add(dr);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    // }

                    //if(retailTransaction.SaleIsReturnSale == false)
                    //{
                    foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                    {
                        if (saleLineItem.ReturnLineId == 0)
                        {
                            if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided
                                && retailTransaction.IncomeExpenseAmounts == 0)
                            {
                                if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && !saleLineItem.Voided)
                                {
                                    if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0)
                                        || (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 2)
                                        || (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 4))
                                    {

                                        if (IsBulkItem(saleLineItem.ItemId, sqlTransaction))
                                        {
                                            saleLineItem.PartnerData.BulkItem = 1;

                                            DataTable dtGroup = new DataTable();

                                            if (dtItemInfo.Rows.Count > 0 && dtItemInfo != null)
                                            {
                                                var query = from r in dtItemInfo.AsEnumerable()
                                                            group r by new
                                                            {
                                                                ITEMID = r.Field<string>("ITEMID"),
                                                                CONFIGURATION = r.Field<string>("CONFIGURATION"),
                                                                SIZE = r.Field<string>("SIZE"),
                                                                COLOR = r.Field<string>("COLOR"),
                                                                STYLE = r.Field<string>("STYLE"),
                                                                BATCH = r.Field<string>("BATCH")
                                                            } into groupedTable
                                                            select new
                                                            {
                                                                ITEMID = groupedTable.Key.ITEMID,
                                                                CONFIGURATION = groupedTable.Key.CONFIGURATION,
                                                                SIZE = groupedTable.Key.SIZE,
                                                                COLOR = groupedTable.Key.COLOR,
                                                                STYLE = groupedTable.Key.STYLE,
                                                                BATCH = groupedTable.Key.BATCH,
                                                                QUANTITY = groupedTable.Sum(s => s.Field<decimal>("QUANTITY")),
                                                                PCS = groupedTable.Sum(s => s.Field<decimal>("PCS"))
                                                                //GROSSWT = groupedTable.Sum(s => s.Field<decimal>("GROSSWT")),
                                                                //NETWT = groupedTable.Sum(s => s.Field<decimal>("NETWT"))

                                                            };
                                                dtGroup = ConvertToDataTable(query);
                                            }



                                            if (dtGroup.Rows.Count > 0 && dtGroup != null)
                                            {
                                                foreach (DataRow dr1 in dtGroup.Rows)
                                                {
                                                    if (IsBulkItem(Convert.ToString(dr1["ITEMID"]), sqlTransaction))
                                                    {
                                                        //saleLine.PartnerData.BulkItem = 1;

                                                        GetValidBulkItemPcsAndQtyForTrans(ref dBulkPdsQty, ref dBulkQty,
                                                                                            Convert.ToString(dr1["ITEMID"]), Convert.ToString(dr1["CONFIGURATION"]),
                                                                                            Convert.ToString(dr1["SIZE"]), Convert.ToString(dr1["COLOR"]),
                                                                                            Convert.ToString(dr1["STYLE"]), Convert.ToString(dr1["BATCH"]), sqlTransaction);

                                                        if (IsCatchWtItem(Convert.ToString(dr1["ITEMID"]), sqlTransaction))
                                                        {
                                                            if (dBulkPdsQty <= 0 || dBulkQty <= 0)
                                                            {
                                                                sqlTransaction.Rollback();
                                                                sqlTransaction.Dispose();
                                                                MessageBox.Show("Please check the inventory", "Please check the inventory for " + Convert.ToString(dr1["ITEMID"]) + " item.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                                throw new RuntimeBinderException();
                                                            }
                                                            else if (dBulkPdsQty < Convert.ToDecimal(dr1["PCS"]) || dBulkQty < Convert.ToDecimal(dr1["QUANTITY"]))
                                                            {
                                                                sqlTransaction.Rollback();
                                                                sqlTransaction.Dispose();
                                                                MessageBox.Show("Please check the inventory for " + Convert.ToString(dr1["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                                throw new RuntimeBinderException();
                                                            }
                                                        }
                                                        else
                                                        {
                                                            if (dBulkQty <= 0)
                                                            {
                                                                sqlTransaction.Rollback();
                                                                sqlTransaction.Dispose();
                                                                MessageBox.Show("Please check the inventory", "Please check the inventory for " + Convert.ToString(dr1["ITEMID"]) + " item.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                                throw new RuntimeBinderException();
                                                            }
                                                            else if (dBulkQty < Convert.ToDecimal(dr1["QUANTITY"]))
                                                            {
                                                                sqlTransaction.Rollback();
                                                                sqlTransaction.Dispose();
                                                                MessageBox.Show("Please check the inventory for " + Convert.ToString(dr1["ITEMID"]) + " item.", "Please check the inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                                throw new RuntimeBinderException();
                                                            }
                                                        }
                                                    }
                                                    else
                                                    {
                                                        sqlTransaction.Rollback();
                                                        sqlTransaction.Dispose();
                                                        MessageBox.Show("Invalid item", "Invalid item", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                                        throw new RuntimeBinderException();
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    #endregion

                    #region cal tot BillAmt
                    decimal dBillAmt = 0.0m;
                    dBillAmt = retailTransaction.AmountDue;
                    foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                    {
                        if (saleLineItem.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                        {
                            if (!saleLineItem.Voided)
                            {
                                dBillAmt = dBillAmt + saleLineItem.OriginalPrice;
                            }
                        }
                    }
                    #endregion

                    #region  GSS Maturity // commented

                    //if(retailTransaction.PartnerData.IsGSSMaturity)
                    //{
                    //    if(retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                    //    {
                    //        bool bResult = false;
                    //        try
                    //        {
                    //            if(this.Application.TransactionServices.CheckConnection())
                    //            {

                    //                string sGSSNo = Convert.ToString(retailTransaction.PartnerData.GSSMaturityNo);

                    //                ReadOnlyCollection<object> containerArray;
                    //                containerArray = this.Application.TransactionServices.InvokeExtension("UpdateGSSMaturityAdjustment",
                    //                                                                                      sGSSNo, true, DateTime.Now, ((LSRetailPosis.Transaction.PosTransaction)(retailTransaction)).ReceiptId);

                    //                bResult = Convert.ToBoolean(containerArray[1]);
                    //            }
                    //        }
                    //        catch(Exception)
                    //        {
                    //            MessageBox.Show(string.Format("Failed due to RTS issue."));
                    //            throw new RuntimeBinderException();
                    //        }
                    //        if(bResult == false)
                    //        {
                    //            MessageBox.Show(string.Format("Failed to GSS Maturity Adjustment."));
                    //            throw new RuntimeBinderException();
                    //        }
                    //        else
                    //        {
                    //            #region IsGSSMaturity
                    //            if(retailTransaction.PartnerData.IsGSSMaturity)
                    //            {
                    //                if(retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                    //                {
                    //                    string scmdGssMaturity = "SET DATEFORMAT DMY;  UPDATE GSSACCOUNTOPENINGPOSTED" +
                    //                                             " SET GSSADJUSTED = 1,GSSADJUSTEDDATE = '" + DateTime.Now + "'" +
                    //                                             " WHERE GSSACCOUNTNO = '" + Convert.ToString(retailTransaction.PartnerData.GSSMaturityNo) + "';";

                    //                    scmdGssMaturity = scmdGssMaturity + "  INSERT INTO RETAILGSSMATURITYADJUSTMENT (TRANSACTIONID,GSSACCOUNTNO,CUSTACCOUNT,GSSTOTALQTY,GSSTOTALAMOUNT," +
                    //                                        " GSSAVGRATE,GSSROYALITYAMOUNT,STOREID,TERMINALID,STAFFID,DATAAREAID) VALUES (@TRANSACTIONID,@GSSACCOUNTNO,@CUSTACCOUNT,@GSSTOTALQTY,@GSSTOTALAMOUNT," +
                    //                                        " @GSSAVGRATE,@GSSROYALITYAMOUNT,@STOREID,@TERMINALID,@STAFFID,@DATAAREAID);";


                    //                    using(SqlCommand sqlCmd = new SqlCommand(scmdGssMaturity, sqlTransaction.Connection, sqlTransaction))
                    //                    {
                    //                        sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONID", posTransaction.TransactionId));
                    //                        sqlCmd.Parameters.Add(new SqlParameter("@GSSACCOUNTNO", retailTransaction.PartnerData.GSSMaturityNo));
                    //                        sqlCmd.Parameters.Add(new SqlParameter("@CUSTACCOUNT", retailTransaction.Customer.CustomerId));

                    //                        sqlCmd.Parameters.Add(new SqlParameter("@GSSTOTALQTY", retailTransaction.PartnerData.GSSTotQty));
                    //                        sqlCmd.Parameters.Add(new SqlParameter("@GSSTOTALAMOUNT", retailTransaction.PartnerData.GSSTotAmt));
                    //                        sqlCmd.Parameters.Add(new SqlParameter("@GSSAVGRATE", retailTransaction.PartnerData.GSSAvgRate));
                    //                        sqlCmd.Parameters.Add(new SqlParameter("@GSSROYALITYAMOUNT", retailTransaction.PartnerData.GSSRoyaltyAmt));

                    //                        sqlCmd.Parameters.Add(new SqlParameter("@STOREID", posTransaction.StoreId));
                    //                        sqlCmd.Parameters.Add(new SqlParameter("@TERMINALID", posTransaction.TerminalId));
                    //                        sqlCmd.Parameters.Add(new SqlParameter("@STAFFID", posTransaction.OperatorId));
                    //                        sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));

                    //                        sqlCmd.ExecuteNonQuery();
                    //                    }

                    //                }
                    //            }


                    //            #endregion
                    //        }
                    //    }
                    //}

                    #endregion


                    #region Insert Qry RETAIL_CUSTOMCALCULATIONS_TABLE
                    int iIngredient = 0;
                    //item.PartnerData.TierDiscType = Convert.ToInt16(dr["TierDiscType"]);
                    //item.PartnerData.TierDiscAmt = Convert.ToDecimal(dr["TierDiscAmt"]); 

                    string commandString = " INSERT INTO RETAIL_CUSTOMCALCULATIONS_TABLE " +
                                           " ([DATAAREAID],[STOREID],[TERMINALID],[TRANSACTIONID],[LINENUM],[PIECES],[QUANTITY],[RATETYPE],[MAKINGRATE] " +
                                           " ,[MAKINGTYPE],[AMOUNT],[MAKINGDISCOUNT],[MAKINGAMOUNT],[TOTALAMOUNT],[TOTALWEIGHT],[LOSSPERCENTAGE],[LOSSWEIGHT] " +
                                           " ,[EXPECTEDQUANTITY],[TRANSACTIONTYPE],[OWN],[SKUDETAILS],[ORDERNUM],[ORDERLINENUM],[CUSTNO],[RETAILSTAFFID] " +
                                           " ,[IsIngredient],[MakingDiscountAmount],[STUDDAMOUNT],[CRate],[ADVANCEADJUSTMENTID],[SAMPLERETURN] " +
                                           " ,[WastageType],[WastageQty],[WastageAmount],[WastagePercentage],[WastageRate],[MAKINGDISCTYPE]," +
                                           " [CONFIGID],[ADVANCEADJUSTMENTSTOREID],[ADVANCEADJUSTMENTTERMINALID],[PURITY]" + // Changed for wastage and Making Discount
                                           " ,OGGROSSUNIT,OGDMDPCS,OGDMDWT,OGDMDUNIT,OGDMDAMT,OGSTONEPCS,OGSTONEWT,OGSTONEUNIT,OGSTONEAMT," +
                                           " OGNETWT,OGNETRATE,OGNETUNIT,OGNETAMT,REFINVOICENO,SALESCHANNELTYPE," +
                                           " SELLINGPRICE,ITEMIDPARENT,UPDATEDCOSTPRICE,GROUPCOSTPRICE,PROMOTIONCODE,FLAT,SPECIALDISCINFO,RETAILBATCHNO," +
                                           " ACTMKRATE,ACTTOTAMT,CHANGEDTOTAMT,LINEDISC,OGREFBATCHNO,OGCHANGEDGROSSWT," +
                                           " OGDMDRATE,OGSTONEATE,OGGROSSAMT,OGACTAMT,OGCHANGEDAMT,OGFINALAMT,FGPROMOCODE," +
                                           " ISREPAIR,REPAIRBATCHID,TRANSFERCOSTPRICE,GOLDTAXVALUE,WTDIFFDISCQTY,WTDIFFDISCAMT," +
                                           " PURITYREADING1,PURITYREADING2,PURITYREADING3, PURITYPERSON,PURITYPERSONNAME," +
                                           " SKUAgingDiscType,SKUAgingDiscAmt,TierDiscType,TierDiscAmt,MRPOrMkDiscType,MRPOrMkDiscAmt," +
                                           " OGPSTONEPCS,OGPSTONEWT,OGPSTONEUNIT,OGPSTONERATE,OGPSTONEAMOUNT," +
                                           " MRPORMKPROMODISCCODE,MRPORMKPROMODISCTYPE,MRPORMKPROMODISCAMT,OpeningDiscType,OpeningDisc,"+
                                           " MakStnDiaDiscType,MakingDiscntAmt,StoneDiscntAmt,DiamondDiscntAmt)" +
                        //  " ,LocalCustomerName,LocalCustomerAddress,LocalCustomerContactNo)" +
                                           " VALUES " +
                                           " (@DATAAREAID,@STOREID,@TERMINALID,@TRANSACTIONID,@LINENUM,@PIECES,@QUANTITY," +
                                           " @RATETYPE,@MAKINGRATE,@MAKINGTYPE,@AMOUNT,@MAKINGDISCOUNT,@MAKINGAMOUNT,@TOTALAMOUNT, " +
                                           " @TOTALWEIGHT,@LOSSPERCENTAGE,@LOSSWEIGHT,@EXPECTEDQUANTITY,@TRANSACTIONTYPE,@OWN,@SKUDETAILS, " +
                                           " @ORDERNUM,@ORDERLINENUM,@CUSTNO,@RETAILSTAFFID,@ISINGREDIENT,@MAKINGDISCAMT,@STUDDAMOUNT, " +
                                           " @RATE,@ADVANCEADJUSTMENTID,@SAMPLERETURN,@WastageType,@WastageQty,@WastageAmount,@WastagePercentage," +
                                           " @WastageRate,@MAKINGDISCTYPE,@CONFIGID,@ADVANCEADJUSTMENTSTOREID,@ADVANCEADJUSTMENTTERMINALID,@PURITY," +  // Changed for wastage
                                           " @OGGROSSUNIT,@OGDMDPCS,@OGDMDWT,@OGDMDUNIT,@OGDMDAMT,@OGSTONEPCS,@OGSTONEWT,@OGSTONEUNIT,@OGSTONEAMT," +
                                           " @OGNETWT,@OGNETRATE,@OGNETUNIT,@OGNETAMT,@REFINVOICENO,@SALESCHANNELTYPE," +
                                           " @SELLINGPRICE,@ITEMIDPARENT,@UPDATEDCOSTPRICE,@GROUPCOSTPRICE,@PROMOTIONCODE," +
                                           " @FLAT,@SPECIALDISCINFO,@RETAILBATCHNO,@ACTMKRATE,@ACTTOTAMT,@CHANGEDTOTAMT,@LINEDISC," +
                                           " @OGREFBATCHNO,@OGCHANGEDGROSSWT,@OGDMDRATE,@OGSTONEATE,@OGGROSSAMT,@OGACTAMT," +
                                           " @OGCHANGEDAMT,@OGFINALAMT,@FGPROMOCODE,@ISREPAIR,@REPAIRBATCHID," +
                                           " @TRANSFERCOSTPRICE,@GOLDTAXVALUE,@WTDIFFDISCQTY,@WTDIFFDISCAMT," +
                                           " @PURITYREADING1,@PURITYREADING2,@PURITYREADING3,@PURITYPERSON," +
                                           " @PURITYPERSONNAME,@SKUAgingDiscType,@SKUAgingDiscAmt,@TierDiscType," +
                                           " @TierDiscAmt,@MRPOrMkDiscType,@MRPOrMkDiscAmt," +
                                           " @OGPSTONEPCS,@OGPSTONEWT,@OGPSTONEUNIT,@OGPSTONERATE,@OGPSTONEAMOUNT," +
                                           " @MRPORMKPROMODISCCODE,@MRPORMKPROMODISCTYPE,@MRPORMKPROMODISCAMT,@OpeningDiscType,@OpeningDisc,"+
                                           " @MakStnDiaDiscType,@MakingDiscAmt1,@StoneDiscAmt1,@DiamondDiscAmt1)" +
                        // " ,@LocalCustomerName,@LocalCustomerAddress,@LocalCustomerContactNo)" +
                                           " " +
                                           " DECLARE @isVoided bit " +
                                           " SET @isVoided = @isVoid ";


                    #endregion


                    if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                    {
                        #region When not voided Update Customer order
                        commandString = commandString +
                                         " IF(@isVoided='False' OR @isVoided='0') " +
                                         " BEGIN ";
                        commandString = commandString +
                            //   " IF CAST(@ITEMAMOUNT AS NUMERIC(28,2)) > 0 " + 
                                  " IF CAST(@ITEMAMOUNT AS NUMERIC(28,2)) >= 0 and (CAST(@QUANTITY AS NUMERIC(28,3))  * -1)  >= 0 " +
                                  " BEGIN " +
                            //  " UPDATE SKUTable_Posted SET isAvailable='False',isLocked='False' WHERE SkuNumber=@INVENTITEMID " +
                                  " UPDATE SKUTableTrans SET isAvailable='False',isLocked='False' WHERE SkuNumber=@INVENTITEMID " + //SKU Table New
                            //    " AND STOREID=@STOREID " + 
                                  " AND DATAAREAID=@DATAAREAID  " +
                                  " END " +
                                  " ELSE " +
                            // " UPDATE SKUTable_Posted SET isAvailable='True',isLocked='False' WHERE SkuNumber=@INVENTITEMID " +
                                  " UPDATE SKUTableTrans SET isAvailable='True',isLocked='False' WHERE SkuNumber=@INVENTITEMID " + //SKU Table New
                            //" AND STOREID=@STOREID " +
                                  " AND DATAAREAID=@DATAAREAID  ";
                        commandString = commandString +
                                          " UPDATE CUSTORDER_DETAILS SET isDelivered = 1 WHERE ORDERNUM=@ORDERNUM AND LINENUM=@ORDERLINENUM " +
                                          " DECLARE @COUNT INT " +
                                          " SELECT @COUNT=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM=@ORDERNUM AND isDelivered = 0  " +
                                              " IF(@COUNT=0) " +
                                                  " BEGIN " +
                                                     " UPDATE CUSTORDER_HEADER SET isDelivered = 1 WHERE ORDERNUM=@ORDERNUM  " +
                                                  " END " +
                            //" END " + R.H commented on 08/10/14
                                              " ELSE " +
                                                  " BEGIN " +
                                                      " UPDATE CUSTORDER_DETAILS SET isDelivered = 0 WHERE ORDERNUM=@ORDERNUM AND LINENUM=@ORDERLINENUM " +
                                                      " DECLARE @COUNT1 INT " +
                                                      " SELECT @COUNT1=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM=@ORDERNUM AND isDelivered = 0  " +
                                                      " IF(@COUNT1>0) " +
                                                          " BEGIN " +
                                                             " UPDATE CUSTORDER_HEADER SET isDelivered = 0 WHERE ORDERNUM=@ORDERNUM  " +
                                                          " END " +
                                                 " END " +
                                        " END; ";
                        #endregion

                        #region For repair Out-Side Return
                        if (retailTransaction.PartnerData.IsRepairReturn)
                        {
                            commandString = commandString +
                                            " UPDATE RetailRepairDetail SET isDelivered = 1 WHERE BatchId=@BatchId" +
                                            " DECLARE @COUNTR INT " +
                                            " SELECT @COUNTR=COUNT(BatchId) FROM RetailRepairDetail WHERE RepairId=@RepairId AND isDelivered = 0" +
                                            " IF(@COUNTR=0)" +
                                            " BEGIN " +
                                            " UPDATE RetailRepairHdr SET isDelivered = 1 WHERE RepairId=@RepairId " +
                                            " END " +
                                            " ELSE " +
                                            " BEGIN " +
                                            " UPDATE RetailRepairDetail SET isDelivered = 0 WHERE BatchId=@BatchId " +
                                            " DECLARE @COUNTR1 INT " +
                                            " SELECT @COUNTR1=COUNT(BatchId) FROM RetailRepairDetail WHERE RepairId=@RepairId AND isDelivered = 0 " +
                                            " IF(@COUNTR1>0)" +
                                            " BEGIN " +
                                            " UPDATE RetailRepairHdr SET isDelivered = 0 WHERE RepairId=@RepairId " +
                                            " END " +
                                            " END;";
                        }
                        #endregion

                        #region Update RETAILCUSTOMERDEPOSITSKUDETAILS with deliver=1
                        if (retailTransaction.PartnerData.SKUBookedItemsExists == "Y")
                        {
                            string skunumber = string.Empty;
                            string sOLine = "0";
                            string sOrderNo = "";

                            foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                            {
                                if (!saleLineItem.Voided && saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                {
                                    if (string.IsNullOrEmpty(skunumber))
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.OrderLineNum)))
                                            sOLine = "'" + Convert.ToString(saleLineItem.PartnerData.OrderLineNum) + "'";

                                        sOrderNo = Convert.ToString(saleLineItem.PartnerData.OrderNum);

                                        skunumber = "'" + saleLineItem.ItemId + "'";
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.OrderLineNum)))
                                            sOLine += ",'" + Convert.ToString(saleLineItem.PartnerData.OrderLineNum) + "'";

                                        skunumber += ",'" + saleLineItem.ItemId + "'";
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(skunumber))
                                skunumber = "''";

                            commandString = commandString + " UPDATE RETAILCUSTOMERDEPOSITSKUDETAILS SET DELIVERED=1 WHERE SKUNUMBER IN (" + skunumber + ") and LINENUM IN (" + sOLine + ")  and ORDERNUMBER ='" + sOrderNo + "'";
                        }

                        if (retailTransaction.SaleIsReturnSale == true)
                        {

                            /*  commandString = commandString + " DECLARE @COUNTERCODE AS NVARCHAR(20)" +
                                              " SELECT @COUNTERCODE = COUNTERCODE FROM RETAILSTORECOUNTERTABLE" +
                                              " WHERE  RETAILSTOREID = '" + ApplicationSettings.Terminal.StoreId + "' AND DEFAULTSC = 1" +
                                              " IF EXISTS (SELECT SKUNUMBER FROM SKUTableTrans WHERE SKUNUMBER = @INVENTITEMID) BEGIN " +
                                              " UPDATE SKUTableTrans SET TOCOUNTER = @COUNTERCODE WHERE SKUNUMBER = @INVENTITEMID END ELSE BEGIN" +
                                              " IF EXISTS (SELECT SKUNUMBER FROM SKUTable_Posted WHERE SKUNUMBER = @INVENTITEMID) BEGIN " +
                                              " INSERT INTO SKUTableTrans (SkuDate,SkuNumber,DATAAREAID,CREATEDON" +
                                              " ,isLocked,isAvailable,PDSCWQTY,QTY,INGREDIENT,FROMCOUNTER,TOCOUNTER," +
                                              " ECORESCONFIGURATIONNAME,INVENTCOLORID,INVENTSIZEID,RETAILSTOREID)" +
                                              " SELECT SkuDate,SkuNumber,DATAAREAID,CREATEDON,isLocked," +
                                              " 1,PDSCWQTY,QTY,INGREDIENT,FROMCOUNTER,@COUNTERCODE,ECORESCONFIGURATIONNAME," +
                                              " INVENTCOLORID,INVENTSIZEID,'" + ApplicationSettings.Terminal.StoreId + "'" +
                                              " FROM SKUTable_Posted WHERE SKUNUMBER = @INVENTITEMID END END"
                                              ;*/

                            #region for convert  to advance //@ SR

                            #region isOGPurchOrExch chking with SR
                            bool isOGPurchOrExch = false;
                            int iAdvAgainst = 0;
                            foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                            {
                                if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                {
                                    if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                    {
                                        if (!saleLineItem.Voided && retailTransaction.PartnerData.isDoneConvertAdvance == false)
                                        {
                                            if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 1)
                                              || (Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 3))
                                            {
                                                isOGPurchOrExch = true;
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region chking iAdvAgainst
                            string sAdvSalesMan = "";
                            foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                            {
                                if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                {
                                    if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                    {
                                        if (!saleLineItem.Voided && retailTransaction.PartnerData.isDoneConvertAdvance == false)
                                        {
                                            if (isOGPurchOrExch)
                                            {
                                                if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 3))
                                                {
                                                    iAdvAgainst = (int)AdvAgainst.OGExchange;
                                                    break;
                                                }
                                                else
                                                    iAdvAgainst = (int)AdvAgainst.None;

                                            }
                                            else
                                                iAdvAgainst = (int)AdvAgainst.None;

                                            sAdvSalesMan = Convert.ToString(saleLineItem.SalesPersonId);
                                        }
                                    }
                                }
                            }

                            #endregion

                            if (retailTransaction.TenderLines.Count > 0)
                            {
                                foreach (ITenderLineItem tenderItem in retailTransaction.TenderLines)
                                {
                                    if (tenderItem.Amount < 0 && !tenderItem.Voided)
                                    {
                                        bool bIsSRToAdv = isSRToAdvance(Convert.ToInt16(tenderItem.TenderTypeId), sqlTransaction);
                                        string sGetDefaultConfigId = GetDefaultConfigId(sqlTransaction);


                                        #region RTS globally Insert Advance data
                                        string sTransId = posTransaction.TransactionId;
                                        string sCustId = retailTransaction.Customer.CustomerId;
                                        string sOrderN = retailTransaction.PartnerData.AdjustmentOrderNum;
                                        decimal dAmt = Math.Abs(tenderItem.Amount);
                                        decimal dAdvTaxPct = GetDefaultAdvTaxPct(sqlTransaction);
                                        decimal dAdvTaxAmt = 0m;
                                        if (bIsSRToAdv)
                                            dAdvTaxAmt = decimal.Round(Convert.ToDecimal(dAmt) * dAdvTaxPct / (100 + dAdvTaxPct), 2, MidpointRounding.AwayFromZero);

                                        int iGoldFix = 1;
                                        int iDepositeType = (int)Enums.EnumClass.DepositType.Normal;
                                        string sStoreId = posTransaction.StoreId;
                                        string sTerminal = posTransaction.TerminalId;
                                        string sOpId = posTransaction.OperatorId;
                                        string sDataAreaId = application.Settings.Database.DataAreaID;

                                        if (!isOGPurchOrExch)
                                            iAdvAgainst = (int)AdvAgainst.SaleReturn;

                                        string sVouAgainst = posTransaction.ReceiptId; //Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.VOUCHERAGAINST);
                                        string sGSSNo = "";// Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum);
                                        int iNoOfMonth = 0;

                                        iNoOfMonth = 0;// Convert.ToInt16(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths);

                                        string sOpType = "";// Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType);
                                        decimal sRate = 0m; //Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRate);
                                        string sFixedConfigId = sGetDefaultConfigId;// Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedConfigId);
                                        bool bResult = false;

                                        if (!string.IsNullOrEmpty(sVouAgainst))
                                        {
                                            commandString += " UPDATE RETAILTRANSACTIONTABLE SET ADVANCEDONE=1 , ADVANCEDONEWITH ='" + sTransId + "'" +
                                                       " where RECEIPTID='" + sVouAgainst + "'" +
                                                       " and TERMINAL='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";

                                        }

                                        if (bIsSRToAdv)
                                        {
                                            decimal dManualBookedWty = 0m;
                                            decimal dFixedRatePct = 0m;
                                            //string sCustOrder = OrderNum();

                                            string sProductType = string.Empty;
                                            int iConvToAdv = 0;
                                            if (bIsSRToAdv)
                                                iConvToAdv = 1;
                                            //BlankOperations.WinFormsTouch.frmProductTypeInput objBlankOpe = new BlankOperations.WinFormsTouch.frmProductTypeInput(dtPType);
                                            //objBlankOpe.ShowDialog();
                                            //sProductType = objBlankOpe.sProductType;

                                            //if (isManual)
                                            //{
                                            //    BlankOperations.WinFormsTouch.frmInputGoldBookingQty objBlankOp = new BlankOperations.WinFormsTouch.frmInputGoldBookingQty();
                                            //    objBlankOp.ShowDialog();
                                            //    dManualBookedWty = objBlankOp.dQty;
                                            //}

                                            #region get rate for adv
                                            decimal dTotValue = 0m;
                                            decimal dTotQty = 0m;
                                            decimal dQty = 0m;
                                            decimal dValue = 0m;

                                            #region GetRateSalesLineIngredient
                                            foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                                            {
                                                if (!saleLineItem.Voided && saleLineItem.ReturnTransId != "")
                                                {
                                                    //sRate = Math.Abs(Convert.ToDecimal(saleLineItem.PartnerData.Rate));

                                                    try
                                                    {
                                                        if (this.Application.TransactionServices.CheckConnection())
                                                        {
                                                            ReadOnlyCollection<object> containerArray;
                                                            containerArray = this.Application.TransactionServices.InvokeExtension("GetRateSalesLineIngredient",
                                                                                                                                 saleLineItem.ItemId,
                                                                                                                                 sGetDefaultConfigId,
                                                                                                                                 saleLineItem.ReturnTransId,
                                                                                                                                 saleLineItem.ReturnTerminalId,
                                                                                                                                 saleLineItem.ReturnStoreId);


                                                            DataSet dsTransDetail = new DataSet();
                                                            StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                                                            if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                                                            {
                                                                dsTransDetail.ReadXml(srTransDetail);
                                                            }

                                                            if (dsTransDetail != null
                                                                && dsTransDetail.Tables.Count > 0 && dsTransDetail.Tables[0].Rows.Count > 0)
                                                            {
                                                                DataTable dtRec = dsTransDetail.Tables[0];

                                                                foreach (DataRow drtrans in dtRec.Rows)
                                                                {
                                                                    dValue = Convert.ToDecimal(drtrans["CValue"]);
                                                                    dQty = Convert.ToDecimal(drtrans["Qty"]);
                                                                }
                                                            }

                                                            //dValue = Convert.ToDecimal(containerArray[0]);
                                                            //dQty = Convert.ToDecimal(containerArray[1]);
                                                        }
                                                    }
                                                    catch (Exception)
                                                    {
                                                        sqlTransaction.Rollback();
                                                        sqlTransaction.Dispose();
                                                        MessageBox.Show(string.Format("Failed due to RTS issue."));
                                                        throw new RuntimeBinderException();
                                                    }

                                                    if (dValue == 0)
                                                    {
                                                        dValue = GetIngredientsMetalRate(saleLineItem.ItemId, sGetDefaultConfigId,
                                                                saleLineItem.ReturnTransId, saleLineItem.ReturnTerminalId,
                                                                saleLineItem.ReturnStoreId, sqlTransaction, ref dQty);
                                                    }

                                                    //if(isMRP(saleLineItem.ItemId))
                                                    //{
                                                    //    sRate = Convert.ToDecimal(GetMetalRateForAdv());
                                                    //}

                                                    //if(sRate == 0)
                                                    //{
                                                    //    sRate = Convert.ToDecimal(retailTransaction.PartnerData.dSaleAdjustmentAvgGoldRate);
                                                    //}

                                                    dTotValue += dValue;
                                                    dTotQty += dQty;
                                                }
                                            }
                                            #endregion GetRateSalesLineIngredient

                                            if (!isOGPurchOrExch)
                                            {
                                                if (dTotValue > 0 && dTotQty > 0)
                                                {
                                                    sRate = dTotValue / dTotQty;
                                                }
                                                if (sRate == 0)
                                                {
                                                    sRate = Convert.ToDecimal(GetMetalRateForAdv());
                                                }
                                            }
                                            else
                                            {
                                                sRate = Convert.ToDecimal(GetMetalRateForAdv());
                                            }


                                            #endregion

                                            if (retailTransaction.PartnerData.isDoneConvertAdvance == false)
                                            {
                                                #region insertAdvanceData
                                                try
                                                {
                                                    if (this.Application.TransactionServices.CheckConnection())
                                                    {
                                                        ReadOnlyCollection<object> containerArray;
                                                        containerArray = this.Application.TransactionServices.InvokeExtension("insertAdvanceData",//1
                                                                                                                              sTransId, sCustId, sOrderN,
                                                                                                                              dAmt, iGoldFix, iDepositeType,
                                                                                                                              sStoreId, sTerminal, sOpId,
                                                                                                                              sDataAreaId, iAdvAgainst, sVouAgainst,
                                                                                                                              sGSSNo, iNoOfMonth, sRate, sFixedConfigId,
                                                                                                                              sOpType, 0, sAdvSalesMan, dManualBookedWty,
                                                                                                                              dFixedRatePct, sProductType,
                                                                                                                              dAdvTaxPct, dAdvTaxAmt, iConvToAdv, dCashPayAmt,0,0);


                                                        bResult = Convert.ToBoolean(containerArray[1]);
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    sqlTransaction.Rollback();
                                                    sqlTransaction.Dispose();
                                                    MessageBox.Show(string.Format("Failed due to RTS issue."));
                                                    throw new RuntimeBinderException();
                                                }
                                                if (bResult == false)
                                                {
                                                    #region 2nd time checcking
                                                    bool bIsDone = false;
                                                    try
                                                    {
                                                        if (this.Application.TransactionServices.CheckConnection())
                                                        {
                                                            ReadOnlyCollection<object> containerArray1;
                                                            containerArray1 = this.Application.TransactionServices.InvokeExtension("isAdvanceCreated",//added on 19/07/2017
                                                                                                                                   sCustId, sTransId,
                                                                                                                                   sStoreId, sTerminal);

                                                            bIsDone = Convert.ToBoolean(containerArray1[1]);

                                                            if (bIsDone == false)
                                                            {
                                                                ReadOnlyCollection<object> containerArray;
                                                                containerArray = this.Application.TransactionServices.InvokeExtension("voidAdvance",
                                                                                                                                      sCustId, sTransId,
                                                                                                                                      sStoreId, sTerminal);
                                                                MessageBox.Show(string.Format("Failed to save advance data."));
                                                                throw new RuntimeBinderException();
                                                            }
                                                            else
                                                            {
                                                                retailTransaction.PartnerData.isDoneConvertAdvance = true;
                                                                SaveConvertToAdvInLocal(posTransaction, sqlTransaction, iAdvAgainst
                                                                    , sCustId, sOrderN, dAmt, iGoldFix
                                                                    , iDepositeType, sVouAgainst, sRate
                                                                    , sFixedConfigId, dManualBookedWty
                                                                    , dFixedRatePct, sProductType
                                                                    , dAdvTaxPct, dAdvTaxAmt, iConvToAdv,0,0);
                                                            }
                                                        }
                                                    }
                                                    catch (Exception)
                                                    {
                                                        ReadOnlyCollection<object> containerArray;
                                                        containerArray = this.Application.TransactionServices.InvokeExtension("voidAdvance",
                                                                                                                          sCustId, sTransId,
                                                                                                                          sStoreId, sTerminal);
                                                        sqlTransaction.Rollback();
                                                        sqlTransaction.Dispose();
                                                        MessageBox.Show(string.Format("Failed due to RTS issue."));
                                                        throw new RuntimeBinderException();
                                                    }
                                                    #endregion
                                                }
                                                else
                                                {
                                                    retailTransaction.PartnerData.isDoneConvertAdvance = true;

                                                    SaveConvertToAdvInLocal(posTransaction, sqlTransaction, iAdvAgainst
                                                        , sCustId, sOrderN, dAmt, iGoldFix, iDepositeType
                                                        , sVouAgainst, sRate, sFixedConfigId, dManualBookedWty
                                                                    , dFixedRatePct, sProductType, dAdvTaxPct, dAdvTaxAmt, iConvToAdv,0,0);
                                                }

                                                #endregion
                                            }
                                        }

                                        #endregion
                                    }

                                }
                            }

                            #endregion
                        }
                        #endregion
                    }
                    else
                    {
                        #region Update Customer order with isdeliver=0
                        foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                        {
                            if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                            {
                                UpdateVoidedData(sqlTransaction, retailTransaction, saleLineItem);
                            }
                        }

                        //commandString = commandString +
                        //                 " UPDATE CUSTORDER_DETAILS SET isDelivered = 0 WHERE ORDERNUM=@ORDERNUM AND LINENUM=@ORDERLINENUM " +
                        //                 " DECLARE @COUNT2 INT " +
                        //                 " SELECT @COUNT2=COUNT(LINENUM) FROM CUSTORDER_DETAILS WHERE ORDERNUM=@ORDERNUM AND isDelivered = 0  " +
                        //                 " IF(@COUNT2>0) " +
                        //                 " BEGIN " +
                        //                 " UPDATE CUSTORDER_HEADER SET isDelivered = 0 WHERE ORDERNUM=@ORDERNUM  " +
                        //                 " END ";
                        //commandString = commandString +

                        //        // " UPDATE SKUTable_Posted SET isAvailable=(CASE  WHEN isAvailable='False' THEN 'False' ELSE 'True' END),isLocked='False' WHERE SkuNumber=@INVENTITEMID " +
                        //         " UPDATE SKUTableTrans SET isAvailable=(CASE  WHEN isAvailable='False' THEN 'False' ELSE 'True' END),isLocked='False' WHERE SkuNumber=@INVENTITEMID " + //SKU Table New
                        //    // " AND STOREID=@STOREID " +
                        //         " AND DATAAREAID=@DATAAREAID  ";
                        #endregion
                    }


                    if (retailTransaction.IncomeExpenseAmounts == 0)
                    {
                        #region BatchId save & Gift card Issue By Nimbus for Dubai Malabar
                        foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                        {

                            if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                            {
                                int iMetalType = getMetalType(saleLineItem.ItemId, sqlTransaction);
                                if (iMetalType != (int)Enums.EnumClass.MetalType.PackingMaterial) //PackingMaterial supposed to Shipping charges service item  Req by Mr. Sailendra Sharma on 29/08/2017
                                {
                                    if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                    {
                                        #region Gift card Issue By Nimbus for Dubai Malabar
                                        if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0))
                                        {
                                            string cmdStr1 = string.Empty;

                                            DataTable dtSKUInfo = new DataTable();
                                            string strQRY = "select top 1 isnull(StoreGroup,'') StoreGroup," +
                                                            " isnull(MinPurAmt,0) MinPurAmt,isnull(FreeGift,0) FreeGift," +
                                                            " CRWGiftType,isnull(GiftVoucherCode,'') GiftVoucherCode," +
                                                            " isnull(ACCOUNTTYPE,0) ACCOUNTTYPE," +
                                                            " isnull(ValidityDay,0) ValidityDay from SKUTable_Posted" +
                                                            " Where SkuNumber='" + saleLineItem.ItemId + "'";

                                            dtSKUInfo = GetDataTableInfo(strQRY, sqlTransaction);
                                            string strGVCode = string.Empty;
                                            string strStoreGroup = string.Empty;
                                            decimal dMinPurAmt = 0m;
                                            int iFreeGift = 0;
                                            int iValidityDay = 0;
                                            int iGiftType = 0;
                                            string sStoreCurrency = string.Empty;
                                            sStoreCurrency = ApplicationSettings.Terminal.StoreCurrency;
                                            int iAccType = 0;

                                            if (dtSKUInfo != null && dtSKUInfo.Rows.Count > 0)
                                            {
                                                foreach (DataRow dr in dtSKUInfo.Rows)
                                                {
                                                    strGVCode = Convert.ToString(dr["GiftVoucherCode"]);
                                                    strStoreGroup = Convert.ToString(dr["StoreGroup"]);
                                                    dMinPurAmt = Convert.ToDecimal(dr["MinPurAmt"]);
                                                    iFreeGift = Convert.ToInt16(dr["FreeGift"]);
                                                    iValidityDay = Convert.ToInt16(dr["ValidityDay"]);
                                                    iGiftType = Convert.ToInt16(dr["CRWGiftType"]);
                                                    iAccType = Convert.ToInt16(dr["ACCOUNTTYPE"]);
                                                }
                                            }

                                            if (iMetalType == (int)Enums.EnumClass.MetalType.GiftVoucher)
                                            {
                                                if (this.Application.TransactionServices.CheckConnection())
                                                {
                                                    bool bResult = false;
                                                    ReadOnlyCollection<object> containerArray;
                                                    try
                                                    {

                                                        containerArray = this.Application.TransactionServices.InvokeExtension("GiftCardCreate",
                                                                                                                              saleLineItem.ItemId,
                                                                                                                              Convert.ToDecimal(saleLineItem.NetAmount),
                                                                                                                              posTransaction.TransactionId,
                                                                                                                              posTransaction.StoreId,
                                                                                                                              posTransaction.TerminalId,
                                                                                                                              posTransaction.OperatorId,
                                                                                                                              strGVCode, strStoreGroup,
                                                                                                                              dMinPurAmt, iFreeGift, iValidityDay,
                                                                                                                              iGiftType, sStoreCurrency, iAccType);

                                                        bResult = Convert.ToBoolean(containerArray[1]);
                                                        if (bResult == false)
                                                        {
                                                            sqlTransaction.Rollback();
                                                            sqlTransaction.Dispose();
                                                            MessageBox.Show(Convert.ToString(containerArray[2]));
                                                            throw new RuntimeBinderException();
                                                        }

                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        //sqlTransaction.Rollback();
                                                        //sqlTransaction.Dispose();
                                                        //// MessageBox.Show(string.Format("Failed to save advance data."));
                                                        //throw new RuntimeBinderException();
                                                    }
                                                }
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        #region Advance adjusted
                                        if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0))
                                        {
                                            try
                                            {
                                                ReadOnlyCollection<object> containerArray;
                                                string sMsg = string.Empty;
                                                string sTransId = Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentTransactionID);
                                                string sStoreId = Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentStoreId);
                                                string sTerminalId = Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentTerminalId);
                                                if (!string.IsNullOrEmpty(sTransId))
                                                {
                                                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                                                    {
                                                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("updateAdvanceForAdjust",
                                                                        sTransId,
                                                                        sStoreId,
                                                                        sTerminalId,
                                                                        bAdvRefund);


                                                        sMsg = Convert.ToString(containerArray[2]);
                                                        // MessageBox.Show(sMsg);

                                                        bool bResult = Convert.ToBoolean(containerArray[1]);
                                                        if (!bResult)
                                                        {
                                                            ReadOnlyCollection<object> containerArray1;
                                                            containerArray1 = PosApplication.Instance.TransactionServices.InvokeExtension("AdvanceAdjustedCheck",
                                                                                sTransId,
                                                                                sStoreId,
                                                                                sTerminalId);

                                                            bool bResult1 = Convert.ToBoolean(containerArray1[1]);

                                                            if (!bResult1)
                                                            {
                                                                sqlTransaction.Rollback();
                                                                sqlTransaction.Dispose();
                                                                // MessageBox.Show(string.Format("Failed to save advance data."));
                                                                throw new RuntimeBinderException();
                                                            }
                                                        }
                                                    }

                                                } //AdvanceAdjustedCheck
                                            }
                                            catch (Exception ex)
                                            {
                                                sqlTransaction.Rollback();
                                                sqlTransaction.Dispose();
                                                // MessageBox.Show(string.Format("Failed to save advance data."));
                                                throw new RuntimeBinderException();
                                            }
                                        }
                                        #endregion
                                    }
                                }
                            }

                            #region Update RETAILTRANSACTIONSALESTRANS
                            if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                            {
                                string cmdStr = string.Empty; //start for payment transdate will be acctual transdate and time dev on 13/07/2017 req  by Mr. S.Sharma

                                cmdStr = " IF EXISTS(select top 1 TRANSDATE from RETAILTRANSACTIONPAYMENTTRANS" +
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "'" +
                                                      " and TERMINAL='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "') BEGIN" +
                                    " UPDATE RETAILTRANSACTIONSALESTRANS SET TRANSDATE= (select top 1 TRANSDATE from RETAILTRANSACTIONPAYMENTTRANS" +
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "'" +
                                                      " and TERMINAL='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "')," +
                                                      " TRANSTIME= (select top 1 TRANSTIME from RETAILTRANSACTIONPAYMENTTRANS" +
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "'" +
                                                      " and TERMINAL='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "')" +
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "' and LINENUM=" + saleLineItem.LineId + "" +
                                                      " and TERMINALID='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";

                                cmdStr += " UPDATE RETAILTRANSACTIONTABLE SET TRANSDATE= (select top 1 TRANSDATE from RETAILTRANSACTIONPAYMENTTRANS" +
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "'" +
                                                      " and TERMINAL='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "')," +
                                                      " TRANSTIME= (select top 1 TRANSTIME from RETAILTRANSACTIONPAYMENTTRANS" +
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "'" +
                                                      " and TERMINAL='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "')" +
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "'" +
                                                      " and TERMINAL='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "' END";

                                using (SqlCommand sqlCmd = new SqlCommand(cmdStr, sqlTransaction.Connection, sqlTransaction))
                                {
                                    sqlCmd.ExecuteNonQuery();
                                }//end


                                if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && saleLineItem.ReturnLineId == 0)
                                {
                                    if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 1 || Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 3))
                                    {
                                        string cmdStr1 = string.Empty;
                                        if (IsBatchItem(saleLineItem.ItemId, sqlTransaction))
                                        {
                                            int iMetalType = getMetalType(saleLineItem.ItemId, sqlTransaction);
                                            bool isOGItem = IsOGItemType(saleLineItem.ItemId, sqlTransaction);

                                            string sBatchIdforOg = "";

                                            if (isOGItem)
                                                sBatchIdforOg = DateTime.Now.ToString("ddMMyyyy") + "" + posTransaction.StoreId;
                                            else
                                                sBatchIdforOg = retailTransaction.ReceiptId + "" + saleLineItem.LineId;

                                            string sOldBatchId = "";
                                            if (isOGItem)
                                                sOldBatchId = DateTime.Now.ToString("ddMMyyyy") + "" + ApplicationSettings.Database.StoreID;
                                            else
                                                sOldBatchId = retailTransaction.ReceiptId + "" + saleLineItem.LineId;

                                            string sBatchId = string.Empty;

                                            if (iMetalType == (int)Enums.EnumClass.MetalType.Gold
                                                || iMetalType == (int)Enums.EnumClass.MetalType.Silver
                                                || iMetalType == (int)Enums.EnumClass.MetalType.Platinum
                                                || iMetalType == (int)Enums.EnumClass.MetalType.Palladium)
                                                sBatchId = sBatchIdforOg;
                                            else
                                                sBatchId = sOldBatchId;

                                            if (!string.IsNullOrEmpty(saleLineItem.PartnerData.REPAIRBATCHID))//added on 28082018
                                            {
                                                sBatchId = saleLineItem.PartnerData.REPAIRBATCHID;
                                            }

                                            cmdStr1 = " UPDATE RETAILTRANSACTIONSALESTRANS SET INVENTBATCHID='" + sBatchId + "'" + // added on 31/12/14 on req R.Nandy
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "' and LINENUM=" + saleLineItem.LineId + "" +
                                                      " and TERMINALID='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";

                                            using (SqlCommand sqlCmd = new SqlCommand(cmdStr1, sqlTransaction.Connection, sqlTransaction))
                                            {
                                                sqlCmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                    else if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 2 || Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 4))
                                    {
                                        string cmdStr1 = string.Empty;
                                        if (IsBatchItem(saleLineItem.ItemId, sqlTransaction))
                                        {
                                            cmdStr1 = " UPDATE RETAILTRANSACTIONSALESTRANS SET INVENTBATCHID='" + saleLineItem.PartnerData.RETAILBATCHNO + "'" + // added on 31/12/14 on req R.Nandy
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "' and LINENUM=" + saleLineItem.LineId + "" +
                                                      " and TERMINALID='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";

                                            cmdStr1 += " UPDATE RETAILTRANSACTIONSALESTRANS SET ISOGRETURN=1" + //ADDED ON 170417 REQ BY S.SHARMA
                                                     " where RECEIPTID='" + saleLineItem.PartnerData.OGRECEIPTNO + "' and LINENUM=" + saleLineItem.PartnerData.OGLINENUM + "";
                                            //" and TERMINALID='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";

                                            using (SqlCommand sqlCmd = new SqlCommand(cmdStr1, sqlTransaction.Connection, sqlTransaction))
                                            {
                                                sqlCmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                    else if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0 && !string.IsNullOrEmpty(saleLineItem.PartnerData.REPAIRINVENTBATCHID)))
                                    {
                                        string cmdStr1 = string.Empty;
                                        if (IsBatchItem(saleLineItem.ItemId, sqlTransaction))
                                        {
                                            cmdStr1 = " UPDATE RETAILTRANSACTIONSALESTRANS SET INVENTBATCHID='" + saleLineItem.PartnerData.REPAIRINVENTBATCHID + "'" + // added on 31/12/14 on req R.Nandy
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "' and LINENUM=" + saleLineItem.LineId + "" +
                                                      " and TERMINALID='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";
                                            using (SqlCommand sqlCmd = new SqlCommand(cmdStr1, sqlTransaction.Connection, sqlTransaction))
                                            {
                                                sqlCmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                    else if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0 && !string.IsNullOrEmpty(saleLineItem.PartnerData.BULKINVENTBATCHID)))
                                    {
                                        string cmdStr1 = string.Empty;
                                        if (IsBatchItem(saleLineItem.ItemId, sqlTransaction))
                                        {
                                            cmdStr1 = " UPDATE RETAILTRANSACTIONSALESTRANS SET INVENTBATCHID='" + saleLineItem.PartnerData.BULKINVENTBATCHID + "'" + // added on 31/12/14 on req R.Nandy
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "' and LINENUM=" + saleLineItem.LineId + "" +
                                                      " and TERMINALID='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";
                                            using (SqlCommand sqlCmd = new SqlCommand(cmdStr1, sqlTransaction.Connection, sqlTransaction))
                                            {
                                                sqlCmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                                else //if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0))
                                {
                                    if (saleLineItem.ReturnLineId == 0)
                                    {
                                        string strRepairBatch = " UPDATE RETAILTRANSACTIONSALESTRANS SET INVENTBATCHID='" + saleLineItem.PartnerData.RETAILBATCHNO + "'" + // added on 31/12/14 on req R.Nandy
                                                           " where TRANSACTIONID='" + posTransaction.TransactionId + "' and LINENUM=" + saleLineItem.LineId + "" +
                                                           " and TERMINALID='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";
                                        using (SqlCommand sqlCmd = new SqlCommand(strRepairBatch, sqlTransaction.Connection, sqlTransaction))
                                        {
                                            sqlCmd.ExecuteNonQuery();
                                        }
                                    }
                                    if (retailTransaction.PartnerData.IsGSSMaturity)
                                    {
                                        string cmdStr1 = string.Empty;
                                        string sGSSNo = Convert.ToString(retailTransaction.PartnerData.GSSMaturityNo);
                                        if (!string.IsNullOrEmpty(sGSSNo))
                                        {
                                            cmdStr1 = " UPDATE RETAILTRANSACTIONSALESTRANS SET GSVGSSADVREF='" + sGSSNo + "'" +
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "' and LINENUM=" + saleLineItem.LineId + "" +
                                                      " and TERMINALID='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";
                                            using (SqlCommand sqlCmd = new SqlCommand(cmdStr1, sqlTransaction.Connection, sqlTransaction))
                                            {
                                                sqlCmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        int iMetalType = getMetalType(saleLineItem.ItemId, sqlTransaction);
                                        if (iMetalType != (int)Enums.EnumClass.MetalType.PackingMaterial)
                                        {
                                            string sAdvAdjId = !string.IsNullOrEmpty(saleLineItem.PartnerData.ServiceItemCashAdjustmentTransactionID) ? saleLineItem.PartnerData.ServiceItemCashAdjustmentTransactionID : string.Empty;

                                            if (!string.IsNullOrEmpty(sAdvAdjId))
                                            {
                                                string cmdStr1 = " UPDATE RETAILTRANSACTIONSALESTRANS SET GSVGSSADVREF='" + sAdvAdjId + "'" +
                                                           " where TRANSACTIONID='" + posTransaction.TransactionId + "' and LINENUM=" + saleLineItem.LineId + "" +
                                                           " and TERMINALID='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";
                                                using (SqlCommand sqlCmd = new SqlCommand(cmdStr1, sqlTransaction.Connection, sqlTransaction))
                                                {
                                                    sqlCmd.ExecuteNonQuery();
                                                }
                                            }
                                        }

                                        if (IsBatchItem(saleLineItem.ItemId, sqlTransaction))
                                        {
                                            //int iMetalType = getMetalType(saleLineItem.ItemId, sqlTransaction);
                                            string sDefaultBatchId = getDefaultBatchId(saleLineItem.ItemId, sqlTransaction);

                                            bool isOGItem = IsOGItemType(saleLineItem.ItemId, sqlTransaction);

                                            string sBatchIdforOg = "";

                                            if (isOGItem)
                                                sBatchIdforOg = DateTime.Now.ToString("ddMMyyyy") + "" + posTransaction.StoreId;
                                            else
                                                sBatchIdforOg = retailTransaction.ReceiptId + "" + saleLineItem.LineId;

                                            string sOldBatchId = "";
                                            if (isOGItem)
                                                sOldBatchId = DateTime.Now.ToString("ddMMyyyy") + "" + ApplicationSettings.Database.StoreID;
                                            else
                                                sOldBatchId = retailTransaction.ReceiptId + "" + saleLineItem.LineId;

                                            string sBatchId = string.Empty;

                                            if (iMetalType == (int)Enums.EnumClass.MetalType.Gold
                                               || iMetalType == (int)Enums.EnumClass.MetalType.Silver
                                               || iMetalType == (int)Enums.EnumClass.MetalType.Platinum
                                               || iMetalType == (int)Enums.EnumClass.MetalType.Palladium)
                                            {
                                                if (string.IsNullOrEmpty(sDefaultBatchId))//added on 09/08/17
                                                    sBatchId = sBatchIdforOg;
                                                else
                                                    sBatchId = sDefaultBatchId;
                                            }
                                            else
                                                sBatchId = sOldBatchId;

                                            if (!string.IsNullOrEmpty(saleLineItem.PartnerData.REPAIRBATCHID))//added on 28082018
                                            {
                                                sBatchId = saleLineItem.PartnerData.REPAIRBATCHID;
                                            }

                                            string cmdStr1 = " UPDATE RETAILTRANSACTIONSALESTRANS SET INVENTBATCHID='" + sBatchId + "'" + // added on 31/12/14 on req R.Nandy
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "' and LINENUM=" + saleLineItem.LineId + "" +
                                                      " and TERMINALID='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";

                                            using (SqlCommand sqlCmd = new SqlCommand(cmdStr1, sqlTransaction.Connection, sqlTransaction))
                                            {
                                                sqlCmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }


                        //if(retailTransaction.SaleIsReturnSale == false)
                        //{
                        #region Counter update RETAILTRANSACTIONSALESTRANS
                        foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                        {
                            if (saleLineItem.ReturnLineId == 0)
                            {
                                if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                {
                                    if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                    {
                                        if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0))
                                        {
                                            string cmdStr1 = string.Empty;
                                            string sSalesCounter = GetSalesCounter(saleLineItem.ItemId, sqlTransaction);
                                            if (!string.IsNullOrEmpty(sSalesCounter))
                                            {
                                                cmdStr1 = " UPDATE RETAILTRANSACTIONSALESTRANS SET SALESCOUNTER='" + sSalesCounter + "'" +
                                                          " where TRANSACTIONID='" + posTransaction.TransactionId + "' and LINENUM=" + saleLineItem.LineId + "" +
                                                          " and TERMINALID='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";
                                                using (SqlCommand sqlCmd = new SqlCommand(cmdStr1, sqlTransaction.Connection, sqlTransaction))
                                                {
                                                    sqlCmd.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                {
                                    if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                    {
                                        if ((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 0))
                                        {
                                            string cmdStr1 = string.Empty;
                                            cmdStr1 = " DECLARE @COUNTERCODE AS NVARCHAR(20)" +
                                                      " SELECT @COUNTERCODE = COUNTERCODE FROM RETAILSTORECOUNTERTABLE WHERE  RETAILSTOREID = '" + ApplicationSettings.Terminal.StoreId + "' AND DEFAULTSC = 1" +
                                                      " UPDATE RETAILTRANSACTIONSALESTRANS SET TOCOUNTER=@COUNTERCODE" +
                                                      " where TRANSACTIONID='" + posTransaction.TransactionId + "' and LINENUM=" + saleLineItem.LineId + "" +
                                                      " and TERMINALID='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";

                                            cmdStr1 = cmdStr1 + " SELECT @COUNTERCODE = COUNTERCODE FROM RETAILSTORECOUNTERTABLE" +
                                                      " WHERE  RETAILSTOREID = '" + ApplicationSettings.Terminal.StoreId + "' AND DEFAULTSC = 1" +
                                                      " IF EXISTS (SELECT SKUNUMBER FROM SKUTableTrans WHERE SKUNUMBER = '" + saleLineItem.ItemId + "') BEGIN " +
                                                      " UPDATE SKUTableTrans SET TOCOUNTER = @COUNTERCODE WHERE SKUNUMBER = '" + saleLineItem.ItemId + "' END ELSE BEGIN" +
                                                      " IF EXISTS (SELECT SKUNUMBER FROM SKUTable_Posted WHERE SKUNUMBER = '" + saleLineItem.ItemId + "') BEGIN " +
                                                      " INSERT INTO SKUTableTrans (SkuDate,SkuNumber,DATAAREAID,CREATEDON" +
                                                      " ,isLocked,isAvailable,PDSCWQTY,QTY,INGREDIENT,FROMCOUNTER,TOCOUNTER," +
                                                      " ECORESCONFIGURATIONNAME,INVENTCOLORID,INVENTSIZEID,RETAILSTOREID)" +
                                                      " SELECT SkuDate,SkuNumber,DATAAREAID,CREATEDON,isLocked," +
                                                      " 1,PDSCWQTY,QTY,INGREDIENT,FROMCOUNTER,@COUNTERCODE,ECORESCONFIGURATIONNAME," +
                                                      " INVENTCOLORID,INVENTSIZEID,'" + ApplicationSettings.Terminal.StoreId + "'" +
                                                      " FROM SKUTable_Posted WHERE SKUNUMBER ='" + saleLineItem.ItemId + "' END END";

                                            using (SqlCommand sqlCmd = new SqlCommand(cmdStr1, sqlTransaction.Connection, sqlTransaction))
                                            {
                                                sqlCmd.ExecuteNonQuery();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                        //else
                        //{
                        //    foreach(SaleLineItem saleLineItem in retailTransaction.SaleItems)
                        //    {

                        //    }
                        //}

                        #endregion
                        if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                        {
                            #region for convert  to advance
                            //if(retailTransaction.SaleIsReturnSale == false)
                            //{
                            string sAdvSalesMan = "";
                            #region isOGItem chking
                            bool isOGItem = false;
                            int iAdvAgainst = 0;
                            foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                            {
                                if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                {
                                    if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                    {
                                        if (!saleLineItem.Voided && saleLineItem.ReturnLineId == 0 && retailTransaction.PartnerData.isDoneConvertAdvance == false)
                                        {
                                            isOGItem = IsOGItemType(saleLineItem.ItemId, sqlTransaction);
                                            sAdvSalesMan = saleLineItem.SalesPersonId;
                                            if (isOGItem)
                                                break;
                                        }
                                    }
                                }
                            }
                            //}
                            #endregion

                            #region chking iAdvAgainst
                            foreach (SaleLineItem saleLineItem2 in retailTransaction.SaleItems)
                            {
                                if (saleLineItem2.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                {
                                    if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                    {
                                        if (!saleLineItem2.Voided && saleLineItem2.ReturnLineId == 0 && retailTransaction.PartnerData.isDoneConvertAdvance == false)
                                        {
                                            if (isOGItem)
                                            {
                                                if ((Convert.ToInt16(saleLineItem2.PartnerData.TransactionType) == 3))
                                                {
                                                    iAdvAgainst = (int)AdvAgainst.OGExchange;
                                                    break;
                                                }
                                                else
                                                    iAdvAgainst = (int)AdvAgainst.None;
                                                /////////////////////////////////////////////////////////////////////////////
                                            }
                                            else
                                                iAdvAgainst = (int)AdvAgainst.None;

                                            sAdvSalesMan = saleLineItem2.SalesPersonId;
                                        }
                                    }
                                }
                            }

                            foreach (SaleLineItem saleLineItem1 in retailTransaction.SaleItems)
                            {
                                if (saleLineItem1.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                {
                                    if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                    {
                                        if (!saleLineItem1.Voided && retailTransaction.SaleIsReturnSale == false && retailTransaction.PartnerData.isDoneConvertAdvance == false)
                                        {
                                            if (saleLineItem1.PartnerData.AdvAgainst == (int)AdvAgainst.None)
                                            {
                                                iAdvAgainst = (int)AdvAgainst.None;
                                                break;
                                            }
                                            if (saleLineItem1.PartnerData.AdvAgainst == (int)AdvAgainst.OGPurchase)
                                            {
                                                iAdvAgainst = (int)AdvAgainst.OGPurchase;
                                            }
                                            if (saleLineItem1.PartnerData.AdvAgainst == (int)AdvAgainst.OGExchange)
                                            {
                                                iAdvAgainst = (int)AdvAgainst.OGExchange;
                                            }
                                            if (saleLineItem1.PartnerData.AdvAgainst == (int)AdvAgainst.SaleReturn)
                                            {
                                                iAdvAgainst = (int)AdvAgainst.SaleReturn;
                                            }

                                            sAdvSalesMan = saleLineItem1.SalesPersonId;
                                        }
                                    }
                                }
                            }
                            #endregion

                            //===================================Soutik===================================================
                            #region To check multiple adjustment exists or not
                            bool IsMultipleAdvAdjust = false;
                            bool IsAdvExists = false;
                            decimal dCustAdvAverageRate = 0;
                            int NoOfCustAdvAonly = 0;
                            decimal dCustAdvDepositTotAmt = 0m;
                            decimal dCustAdvBookedQty = 0m;

                            sCustAdjustMentId = CustAdjusmentItemId(sqlTransaction);
                            //=============Check Customer Advance Exists Or Not
                            foreach (SaleLineItem SaleLineItem4 in retailTransaction.SaleItems)
                            {
                                if (retailTransaction.SaleItems.Count > 0)
                                {
                                    if (SaleLineItem4.ItemId == sCustAdjustMentId)
                                    {
                                        IsAdvExists = true;
                                        NoOfCustAdvAonly += 1;
                                        dCustAdvAverageRate += decimal.Round(Convert.ToDecimal(SaleLineItem4.PartnerData.SaleAdjustmentMetalRate), 2, MidpointRounding.AwayFromZero);

                                        dCustAdvDepositTotAmt += decimal.Round(Convert.ToDecimal(SaleLineItem4.PartnerData.SaleAdjustmentGoldAmt), 2, MidpointRounding.AwayFromZero);
                                        dCustAdvBookedQty += decimal.Round(Convert.ToDecimal(SaleLineItem4.PartnerData.SaleAdjustmentGoldQty), 3, MidpointRounding.AwayFromZero);

                                        //break;
                                    }
                                }
                            }

                            foreach (SaleLineItem SaleLineItem5 in retailTransaction.SaleItems)
                            {
                                if (!SaleLineItem5.Voided && retailTransaction.SaleItems.Count > 0)
                                {
                                    if (SaleLineItem5.ItemId != sCustAdjustMentId && SaleLineItem5.GrossAmount < 0 && IsAdvExists)
                                    {
                                        IsMultipleAdvAdjust = true;
                                        break;
                                    }
                                }
                            }
                            #endregion

                            #region SR to Advance //
                            if (retailTransaction.TenderLines.Count > 0 && retailTransaction.PartnerData.isDoneConvertAdvance == false)
                            {
                                foreach (ITenderLineItem tenderItem in retailTransaction.TenderLines)
                                {
                                    if (tenderItem.Amount < 0 && !tenderItem.Voided)
                                    {
                                        bool bIsSRToAdv = isSRToAdvance(Convert.ToInt16(tenderItem.TenderTypeId), sqlTransaction);
                                        //bool bTaxApplicable = IsTaxApplicable(sqlTransaction);
                                        string sGetDefaultConfigId = GetDefaultConfigId(sqlTransaction);
                                        #region RTS globally Insert Advance data
                                        string sTransId = posTransaction.TransactionId;
                                        string sCustId = retailTransaction.Customer.CustomerId;
                                        string sOrderN = retailTransaction.PartnerData.AdjustmentOrderNum;
                                        decimal dAmt = Math.Abs(tenderItem.Amount);// ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount;
                                        decimal dAdvTaxPct = GetDefaultAdvTaxPct(sqlTransaction);
                                        decimal dAdvTaxAmt = 0m;
                                        //  dAdvTaxAmt = decimal.Round(Convert.ToDecimal(dAmt) * dAdvTaxPct / 100, 2, MidpointRounding.AwayFromZero);
                                        if (bIsSRToAdv)
                                            dAdvTaxAmt = decimal.Round(Convert.ToDecimal(dAmt) * dAdvTaxPct / (100 + dAdvTaxPct), 2, MidpointRounding.AwayFromZero);

                                        int iConvToAdv = 0;
                                        if (bIsSRToAdv)
                                            iConvToAdv = 1;

                                        int iGoldFix = 1;
                                        int iDepositeType = (int)Enums.EnumClass.DepositType.Normal;
                                        string sStoreId = posTransaction.StoreId;
                                        string sTerminal = posTransaction.TerminalId;
                                        string sOpId = posTransaction.OperatorId;
                                        string sDataAreaId = application.Settings.Database.DataAreaID;

                                        //if((Convert.ToInt16(saleLineItem.PartnerData.TransactionType) == 1))
                                        //    iAdvAgainst = (int)AdvAgainst.OGPurchase;
                                        //else 
                                        string sVouAgainst = posTransaction.ReceiptId; //Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.VOUCHERAGAINST);
                                        string sGSSNo = "";// Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum);
                                        int iNoOfMonth = 0;

                                        iNoOfMonth = 0;// Convert.ToInt16(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths);

                                        string sOpType = "";// Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType);
                                        decimal sRate = 0m; //Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRate);
                                        string sFixedConfigId = sGetDefaultConfigId;// Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedConfigId);
                                        bool bResult = false;


                                        if (bIsSRToAdv && retailTransaction.PartnerData.isDoneConvertAdvance == false)
                                        {
                                            decimal dManualBookedWty = 0m;
                                            // bool isManual = false;
                                            decimal dFixedRatePct = 0m;
                                            //string sCustOrder = OrderNum();
                                            //IsManualBookedQty(sqlTransaction, ref isManual, ref dFixedRatePct);
                                            string sProductType = string.Empty;
                                            //DataTable dtPType = new DataTable();
                                            //string strQRY = "select DISTINCT PRODUCTTYPECODE from INVENTTABLE where PRODUCTTYPECODE!=''";

                                            //dtPType = GetDataTableInfo(strQRY, sqlTransaction);

                                            //BlankOperations.WinFormsTouch.frmProductTypeInput objBlankOpe = new BlankOperations.WinFormsTouch.frmProductTypeInput(dtPType);
                                            //objBlankOpe.ShowDialog();
                                            //sProductType = objBlankOpe.sProductType;

                                            //if (isManual)
                                            //{
                                            //    BlankOperations.WinFormsTouch.frmInputGoldBookingQty objBlankOp = new BlankOperations.WinFormsTouch.frmInputGoldBookingQty();
                                            //    objBlankOp.ShowDialog();
                                            //    dManualBookedWty = objBlankOp.dQty;
                                            //}

                                            if (!string.IsNullOrEmpty(sVouAgainst))
                                            {
                                                string sSqlString = " UPDATE RETAILTRANSACTIONTABLE SET ADVANCEDONE=1 , ADVANCEDONEWITH ='" + sTransId + "'" +
                                                             " where RECEIPTID='" + sVouAgainst + "'" +
                                                             " and TERMINAL='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";

                                                using (SqlCommand sqlCmd = new SqlCommand(sSqlString, sqlTransaction.Connection, sqlTransaction))
                                                {
                                                    sqlCmd.ExecuteNonQuery();
                                                }
                                            }

                                            #region Comment on 17-07-2019 
                                            //if (retailTransaction.PartnerData.SaleAdjustment)
                                            //{
                                            //    if (!isOGItem)
                                            //    {
                                            //        if (sRate == 0)
                                            //        {
                                            //            // sRate = Convert.ToDecimal(retailTransaction.PartnerData.dSaleAdjustmentAvgGoldRate);
                                            //            sRate = decimal.Round(Convert.ToDecimal(retailTransaction.PartnerData.dCAdvAdjustmentAvgGoldRate), 2, MidpointRounding.AwayFromZero);
                                            //        }
                                            //    }
                                            //    else
                                            //        sRate = Convert.ToDecimal(GetMetalRateForAdv());
                                            //}
                                            //else
                                            //    sRate = Convert.ToDecimal(GetMetalRateForAdv());
                                            ////}
                                            #endregion
                                            //=====================================Soutik
                                            if (retailTransaction.PartnerData.SaleAdjustment)
                                            {
                                                if (!isOGItem)
                                                {
                                                    if (sRate == 0 && !IsMultipleAdvAdjust)
                                                    {
                                                        // sRate = Convert.ToDecimal(retailTransaction.PartnerData.dSaleAdjustmentAvgGoldRate);
                                                        if (NoOfCustAdvAonly > 1)
                                                        {
                                                            //sRate = decimal.Round(Convert.ToDecimal(dCustAdvAverageRate / NoOfCustAdvAonly), 2, MidpointRounding.AwayFromZero);
                                                            sRate = decimal.Round(Convert.ToDecimal(dCustAdvDepositTotAmt / dCustAdvBookedQty), 2, MidpointRounding.AwayFromZero);
                                                        }
                                                        else
                                                            sRate = decimal.Round(Convert.ToDecimal(retailTransaction.PartnerData.dCAdvAdjustmentAvgGoldRate), 2, MidpointRounding.AwayFromZero);
                                                    }
                                                    else
                                                    {
                                                        sRate = Convert.ToDecimal(GetMetalRateForAdv());
                                                    }
                                                }
                                                else
                                                    sRate = Convert.ToDecimal(GetMetalRateForAdv());
                                            }
                                            else
                                                sRate = Convert.ToDecimal(GetMetalRateForAdv());
                                            //}

                                            try
                                            {
                                                if (this.Application.TransactionServices.CheckConnection())
                                                {
                                                    ReadOnlyCollection<object> containerArray;
                                                    containerArray = this.Application.TransactionServices.InvokeExtension("insertAdvanceData",//2
                                                                                                                          sTransId, sCustId, sOrderN,
                                                                                                                          dAmt, iGoldFix, iDepositeType,
                                                                                                                          sStoreId, sTerminal, sOpId,
                                                                                                                          sDataAreaId, iAdvAgainst, sVouAgainst,
                                                                                                                          sGSSNo, iNoOfMonth, sRate,
                                                                                                                          sFixedConfigId, sOpType, 0, sAdvSalesMan,
                                                                                                                          dManualBookedWty, dFixedRatePct, sProductType,
                                                                                                                          dAdvTaxPct, dAdvTaxAmt, iConvToAdv, dCashPayAmt,0,0);

                                                    bResult = Convert.ToBoolean(containerArray[1]);
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                sqlTransaction.Rollback();
                                                sqlTransaction.Dispose();
                                                MessageBox.Show(string.Format("Failed due to RTS issue."));
                                                throw new RuntimeBinderException();
                                            }
                                            if (bResult == false)
                                            {
                                                #region rechecking
                                                bool bIsDone = false;
                                                try
                                                {
                                                    if (this.Application.TransactionServices.CheckConnection())
                                                    {
                                                        ReadOnlyCollection<object> containerArray1;
                                                        containerArray1 = this.Application.TransactionServices.InvokeExtension("isAdvanceCreated",//added on 19/07/2017
                                                                                                                               sCustId, sTransId,
                                                                                                                               sStoreId, sTerminal);

                                                        bIsDone = Convert.ToBoolean(containerArray1[1]);

                                                        if (bIsDone == false)
                                                        {
                                                            ReadOnlyCollection<object> containerArray;
                                                            containerArray = this.Application.TransactionServices.InvokeExtension("voidAdvance",
                                                                                                                                  sCustId, sTransId,
                                                                                                                                  sStoreId, sTerminal);
                                                            MessageBox.Show(string.Format("Failed to save advance data."));
                                                            throw new RuntimeBinderException();
                                                        }
                                                        else
                                                        {
                                                            retailTransaction.PartnerData.isDoneConvertAdvance = true;
                                                            SaveConvertToAdvInLocal(posTransaction, sqlTransaction, iAdvAgainst
                                                                , sCustId, sOrderN, dAmt, iGoldFix, iDepositeType
                                                                , sVouAgainst, sRate, sFixedConfigId
                                                                , dManualBookedWty, dFixedRatePct, sProductType, dAdvTaxPct, dAdvTaxAmt, iConvToAdv,0,0);
                                                        }
                                                    }
                                                }
                                                catch (Exception)
                                                {
                                                    ReadOnlyCollection<object> containerArray;
                                                    containerArray = this.Application.TransactionServices.InvokeExtension("voidAdvance",
                                                                                                                          sCustId, sTransId,
                                                                                                                          sStoreId, sTerminal);
                                                    sqlTransaction.Rollback();
                                                    sqlTransaction.Dispose();
                                                    MessageBox.Show(string.Format("Failed due to RTS issue."));
                                                    throw new RuntimeBinderException();
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                retailTransaction.PartnerData.isDoneConvertAdvance = true;
                                                SaveConvertToAdvInLocal(posTransaction, sqlTransaction, iAdvAgainst
                                                    , sCustId, sOrderN, dAmt, iGoldFix, iDepositeType
                                                    , sVouAgainst, sRate, sFixedConfigId
                                                    , dManualBookedWty, dFixedRatePct, sProductType, dAdvTaxPct, dAdvTaxAmt, iConvToAdv,0,0);
                                            }
                                        }

                                        #endregion
                                    }

                                }
                            }
                            #endregion


                            #endregion
                        }
                        //}


                        foreach (SaleLineItem saleLineItem3 in retailTransaction.SaleItems)
                        {
                            int iMetalType = getMetalType(saleLineItem3.ItemId, sqlTransaction);
                            if (saleLineItem3.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service ||
                                iMetalType == (int)Enums.EnumClass.MetalType.PackingMaterial)
                            {
                                #region Sales

                                if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem3.PartnerData.Ingredients)))
                                {
                                    if ((Convert.ToInt16(saleLineItem3.PartnerData.TransactionType) == 0))
                                    {
                                        #region If Ingredients =1
                                        string stest = Convert.ToString(saleLineItem3.PartnerData.Ingredients);

                                        iIngredient = 1;
                                        StringBuilder sbIngredients = new StringBuilder();
                                        sbIngredients.Append(" INSERT INTO [RETAIL_SALE_INGREDIENTS_DETAILS] ");
                                        sbIngredients.Append("  ([DATAAREAID],[STOREID],[TERMINALID],[TRANSACTIONID],[REFLINENUM],[LINENUM],[STAFFID],[SKUNUMBER] ");
                                        sbIngredients.Append(" ,[ITEMID],[INVENTDIMID],[INVENTLOCATIONID],[INVENTSIZEID],[INVENTCOLORID] ");
                                        sbIngredients.Append(" ,[CONFIGID],[INVENTBATCHID],[PCS],[GRWEIGHT],[QTY] ,[CVALUE],[CRATE],[INVENTSITEID] ");
                                        sbIngredients.Append("  ,[UNITID],[CREATEDON],[SKUDATE],[CTYPE] ");
                                        sbIngredients.Append(" ,[IngrdDiscType],[IngrdDiscAmt],[IngrdDiscTotAmt])"); // Stone Discount
                                        sbIngredients.Append(" VALUES ");
                                        sbIngredients.Append("  (@DATAAREAID,@STOREID,@TERMINALID, @TRANSACTIONID,@REFLINENUM, @LINENUM,@STAFFID, @SKUNUMBER ");
                                        sbIngredients.Append(" ,@ITEMID,@INVENTDIMID, @INVENTLOCATIONID,@INVENTSIZEID, @INVENTCOLORID, @CONFIGID ");
                                        sbIngredients.Append(" ,@INVENTBATCHID,@PCS, @GRWEIGHT, @QTY,@CVALUE, @RATE, @INVENTSITEID, @UNITID, GETDATE(),GETDATE(),@CTYPE,@IngrdDiscType,@IngrdDiscAmt,@IngrdDiscTotAmt) ");

                                        DataSet dsIngredients = new DataSet();
                                        StringReader reader = new StringReader(Convert.ToString(saleLineItem3.PartnerData.Ingredients));
                                        dsIngredients.ReadXml(reader);
                                        int i = 1;
                                        int index = 1;
                                        foreach (DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                        {
                                            index = i;
                                            using (SqlCommand sqlCmd = new SqlCommand(sbIngredients.ToString(), sqlTransaction.Connection, sqlTransaction))
                                            {
                                                sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));
                                                sqlCmd.Parameters.Add(new SqlParameter("@STOREID", posTransaction.StoreId));
                                                sqlCmd.Parameters.Add(new SqlParameter("@TERMINALID", posTransaction.TerminalId));
                                                sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONID", posTransaction.TransactionId));
                                                sqlCmd.Parameters.Add(new SqlParameter("@REFLINENUM", saleLineItem3.LineId));
                                                sqlCmd.Parameters.Add(new SqlParameter("@LINENUM", index));
                                                sqlCmd.Parameters.Add(new SqlParameter("@STAFFID", posTransaction.OperatorId));
                                                sqlCmd.Parameters.Add(new SqlParameter("@SKUNUMBER", !string.IsNullOrEmpty(Convert.ToString(drIngredients["SkuNumber"])) ? drIngredients["SkuNumber"] : string.Empty));

                                                //     sqlCmd.Parameters.Add(new SqlParameter("@LTYPE", !string.IsNullOrEmpty(Convert.ToString(drIngredients["LType"])) ? drIngredients["LType"] : DBNull.Value));
                                                //   sqlCmd.Parameters.Add(new SqlParameter("@LTYPE", DBNull.Value));
                                                sqlCmd.Parameters.Add(new SqlParameter("@ITEMID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["ItemID"])) ? drIngredients["ItemID"] : string.Empty));
                                                sqlCmd.Parameters.Add(new SqlParameter("@INVENTDIMID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventDimID"])) ? drIngredients["InventDimID"] : string.Empty));
                                                //  sqlCmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventLocationID"])) ? drIngredients["InventLocationID"] : DBNull.Value));
                                                //  sqlCmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONID", DBNull.Value));
                                                if (string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventLocationId)))
                                                    sqlCmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONID", string.Empty));
                                                else
                                                    sqlCmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONID", Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventLocationId)));
                                                sqlCmd.Parameters.Add(new SqlParameter("@INVENTSIZEID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventSizeID"])) ? drIngredients["InventSizeID"] : string.Empty));
                                                sqlCmd.Parameters.Add(new SqlParameter("@INVENTCOLORID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventColorID"])) ? drIngredients["InventColorID"] : string.Empty));
                                                sqlCmd.Parameters.Add(new SqlParameter("@CONFIGID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["ConfigID"])) ? drIngredients["ConfigID"] : string.Empty));
                                                sqlCmd.Parameters.Add(new SqlParameter("@INVENTBATCHID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventBatchID"])) ? drIngredients["InventBatchID"] : string.Empty));
                                                sqlCmd.Parameters.Add(new SqlParameter("@PCS", !string.IsNullOrEmpty(Convert.ToString(drIngredients["PCS"])) ? drIngredients["PCS"] : string.Empty));
                                                //   sqlCmd.Parameters.Add(new SqlParameter("@GRWEIGHT", !string.IsNullOrEmpty(Convert.ToString(drIngredients["GrWeight"])) ? drIngredients["GrWeight"] : DBNull.Value));
                                                //  sqlCmd.Parameters.Add(new SqlParameter("@GRWEIGHT", DBNull.Value));

                                                sqlCmd.Parameters.Add(new SqlParameter("@GRWEIGHT", !string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : string.Empty));
                                                sqlCmd.Parameters.Add(new SqlParameter("@QTY", !string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : string.Empty));
                                                sqlCmd.Parameters.Add(new SqlParameter("@CVALUE", !string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : string.Empty));
                                                sqlCmd.Parameters.Add(new SqlParameter("@RATE", !string.IsNullOrEmpty(Convert.ToString(drIngredients["Rate"])) ? drIngredients["Rate"] : string.Empty));

                                                //  sqlCmd.Parameters.Add(new SqlParameter("@CTYPE", Convert.ToString((int)Enums.EnumClass.CalcType.Weight)));
                                                sqlCmd.Parameters.Add(new SqlParameter("@CTYPE", !string.IsNullOrEmpty(Convert.ToString(drIngredients["CTYPE"])) ? drIngredients["CTYPE"] : "0"));

                                                //   sqlCmd.Parameters.Add(new SqlParameter("@CTYPE", DBNull.Value));
                                                //sqlCmd.Parameters.Add(new SqlParameter("@INVENTSITEID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventSiteId"])) ? drIngredients["InventSiteId"] : DBNull.Value));
                                                //   sqlCmd.Parameters.Add(new SqlParameter("@INVENTSITEID", DBNull.Value));
                                                if (string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventSiteId)))
                                                    sqlCmd.Parameters.Add(new SqlParameter("@INVENTSITEID", string.Empty));
                                                else
                                                    sqlCmd.Parameters.Add(new SqlParameter("@INVENTSITEID", Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventSiteId)));
                                                sqlCmd.Parameters.Add(new SqlParameter("@UNITID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["UnitID"])) ? drIngredients["UnitID"] : string.Empty));

                                                if (string.IsNullOrEmpty(Convert.ToString(saleLineItem3.ReturnTransId))) // added on 16.07.2013
                                                {
                                                    sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscType", !string.IsNullOrEmpty(Convert.ToString(drIngredients["IngrdDiscType"])) ? drIngredients["IngrdDiscType"] : "0")); // Stone Discount
                                                    sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscAmt", !string.IsNullOrEmpty(Convert.ToString(drIngredients["IngrdDiscAmt"])) ? drIngredients["IngrdDiscAmt"] : "0"));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscTotAmt", !string.IsNullOrEmpty(Convert.ToString(drIngredients["IngrdDiscTotAmt"])) ? drIngredients["IngrdDiscTotAmt"] : "0"));
                                                }
                                                else
                                                {
                                                    sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscType", "0"));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscAmt", "0"));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscTotAmt", "0"));
                                                }

                                                // studdamount += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : 0);

                                                sqlCmd.ExecuteNonQuery();
                                                i++;
                                            }
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        if (iMetalType == (int)Enums.EnumClass.MetalType.Gold
                                            || iMetalType == (int)Enums.EnumClass.MetalType.Silver
                                            || iMetalType == (int)Enums.EnumClass.MetalType.Platinum)
                                        {
                                            #region If Ingredients =1
                                            string stest = Convert.ToString(saleLineItem3.PartnerData.Ingredients);

                                            iIngredient = 1;
                                            StringBuilder sbIngredients = new StringBuilder();
                                            sbIngredients.Append(" INSERT INTO [RETAIL_SALE_INGREDIENTS_DETAILS] ");
                                            sbIngredients.Append("  ([DATAAREAID],[STOREID],[TERMINALID],[TRANSACTIONID],[REFLINENUM],[LINENUM],[STAFFID],[SKUNUMBER] ");
                                            sbIngredients.Append(" ,[ITEMID],[INVENTDIMID],[INVENTLOCATIONID],[INVENTSIZEID],[INVENTCOLORID] ");
                                            sbIngredients.Append(" ,[CONFIGID],[INVENTBATCHID],[PCS],[GRWEIGHT],[QTY] ,[CVALUE],[CRATE],[INVENTSITEID] ");
                                            sbIngredients.Append("  ,[UNITID],[CREATEDON],[SKUDATE],[CTYPE] ");
                                            sbIngredients.Append(" ,[IngrdDiscType],[IngrdDiscAmt],[IngrdDiscTotAmt])"); // Stone Discount
                                            sbIngredients.Append(" VALUES ");
                                            sbIngredients.Append("  (@DATAAREAID,@STOREID,@TERMINALID, @TRANSACTIONID,@REFLINENUM, @LINENUM,@STAFFID, @SKUNUMBER ");
                                            sbIngredients.Append(" ,@ITEMID,@INVENTDIMID, @INVENTLOCATIONID,@INVENTSIZEID, @INVENTCOLORID, @CONFIGID ");
                                            sbIngredients.Append(" ,@INVENTBATCHID,@PCS, @GRWEIGHT, @QTY,@CVALUE, @RATE, @INVENTSITEID, @UNITID, GETDATE(),GETDATE(),@CTYPE,@IngrdDiscType,@IngrdDiscAmt,@IngrdDiscTotAmt) ");

                                            DataSet dsIngredients = new DataSet();
                                            StringReader reader = new StringReader(Convert.ToString(saleLineItem3.PartnerData.Ingredients));
                                            dsIngredients.ReadXml(reader);
                                            int i = 1;
                                            int index = 1;
                                            foreach (DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                            {
                                                index = i;
                                                using (SqlCommand sqlCmd = new SqlCommand(sbIngredients.ToString(), sqlTransaction.Connection, sqlTransaction))
                                                {
                                                    sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@STOREID", posTransaction.StoreId));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@TERMINALID", posTransaction.TerminalId));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONID", posTransaction.TransactionId));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@REFLINENUM", saleLineItem3.LineId));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@LINENUM", index));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@STAFFID", posTransaction.OperatorId));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@SKUNUMBER", !string.IsNullOrEmpty(Convert.ToString(drIngredients["SkuNumber"])) ? drIngredients["SkuNumber"] : string.Empty));

                                                    //     sqlCmd.Parameters.Add(new SqlParameter("@LTYPE", !string.IsNullOrEmpty(Convert.ToString(drIngredients["LType"])) ? drIngredients["LType"] : DBNull.Value));
                                                    //   sqlCmd.Parameters.Add(new SqlParameter("@LTYPE", DBNull.Value));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@ITEMID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["ItemID"])) ? drIngredients["ItemID"] : string.Empty));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@INVENTDIMID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventDimID"])) ? drIngredients["InventDimID"] : string.Empty));
                                                    //  sqlCmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventLocationID"])) ? drIngredients["InventLocationID"] : DBNull.Value));
                                                    //  sqlCmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONID", DBNull.Value));
                                                    if (string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventLocationId)))
                                                        sqlCmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONID", string.Empty));
                                                    else
                                                        sqlCmd.Parameters.Add(new SqlParameter("@INVENTLOCATIONID", Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventLocationId)));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@INVENTSIZEID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventSizeID"])) ? drIngredients["InventSizeID"] : string.Empty));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@INVENTCOLORID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventColorID"])) ? drIngredients["InventColorID"] : string.Empty));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@CONFIGID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["ConfigID"])) ? drIngredients["ConfigID"] : string.Empty));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@INVENTBATCHID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventBatchID"])) ? drIngredients["InventBatchID"] : string.Empty));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@PCS", !string.IsNullOrEmpty(Convert.ToString(drIngredients["PCS"])) ? drIngredients["PCS"] : string.Empty));
                                                    //   sqlCmd.Parameters.Add(new SqlParameter("@GRWEIGHT", !string.IsNullOrEmpty(Convert.ToString(drIngredients["GrWeight"])) ? drIngredients["GrWeight"] : DBNull.Value));
                                                    //  sqlCmd.Parameters.Add(new SqlParameter("@GRWEIGHT", DBNull.Value));

                                                    sqlCmd.Parameters.Add(new SqlParameter("@GRWEIGHT", !string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : string.Empty));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@QTY", !string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : string.Empty));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@CVALUE", !string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : string.Empty));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@RATE", !string.IsNullOrEmpty(Convert.ToString(drIngredients["Rate"])) ? drIngredients["Rate"] : string.Empty));

                                                    //  sqlCmd.Parameters.Add(new SqlParameter("@CTYPE", Convert.ToString((int)Enums.EnumClass.CalcType.Weight)));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@CTYPE", !string.IsNullOrEmpty(Convert.ToString(drIngredients["CTYPE"])) ? drIngredients["CTYPE"] : "0"));

                                                    //   sqlCmd.Parameters.Add(new SqlParameter("@CTYPE", DBNull.Value));
                                                    //sqlCmd.Parameters.Add(new SqlParameter("@INVENTSITEID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["InventSiteId"])) ? drIngredients["InventSiteId"] : DBNull.Value));
                                                    //   sqlCmd.Parameters.Add(new SqlParameter("@INVENTSITEID", DBNull.Value));
                                                    if (string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventSiteId)))
                                                        sqlCmd.Parameters.Add(new SqlParameter("@INVENTSITEID", string.Empty));
                                                    else
                                                        sqlCmd.Parameters.Add(new SqlParameter("@INVENTSITEID", Convert.ToString(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).InventSiteId)));
                                                    sqlCmd.Parameters.Add(new SqlParameter("@UNITID", !string.IsNullOrEmpty(Convert.ToString(drIngredients["UnitID"])) ? drIngredients["UnitID"] : string.Empty));

                                                    if (string.IsNullOrEmpty(Convert.ToString(saleLineItem3.ReturnTransId))) // added on 16.07.2013
                                                    {
                                                        sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscType", !string.IsNullOrEmpty(Convert.ToString(drIngredients["IngrdDiscType"])) ? drIngredients["IngrdDiscType"] : "0")); // Stone Discount
                                                        sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscAmt", !string.IsNullOrEmpty(Convert.ToString(drIngredients["IngrdDiscAmt"])) ? drIngredients["IngrdDiscAmt"] : "0"));
                                                        sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscTotAmt", !string.IsNullOrEmpty(Convert.ToString(drIngredients["IngrdDiscTotAmt"])) ? drIngredients["IngrdDiscTotAmt"] : "0"));
                                                    }
                                                    else
                                                    {
                                                        sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscType", "0"));
                                                        sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscAmt", "0"));
                                                        sqlCmd.Parameters.Add(new SqlParameter("@IngrdDiscTotAmt", "0"));
                                                    }

                                                    // studdamount += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : 0);

                                                    sqlCmd.ExecuteNonQuery();
                                                    i++;
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                }

                                using (SqlCommand sqlCmd = new SqlCommand(commandString, sqlTransaction.Connection, sqlTransaction))
                                {
                                    if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                    {
                                        #region [ Sale / Purchase Return / Exchange Return]
                                        if ((Convert.ToInt16(saleLineItem3.PartnerData.TransactionType) == 0
                                           || Convert.ToInt16(saleLineItem3.PartnerData.TransactionType) == 2
                                           || Convert.ToInt16(saleLineItem3.PartnerData.TransactionType) == 4) && saleLineItem3.ReturnLineId == 0) //(!retailTransaction.SaleIsReturnSale)
                                        {
                                            bIsPurchase = 0;
                                            decimal studdamount = 0;
                                            if (Convert.ToInt16(saleLineItem3.PartnerData.TransactionType) == 0)
                                                bIsSale = 1; // for sms sending

                                            if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem3.PartnerData.Ingredients)))
                                            {
                                                DataSet dsIngredients = new DataSet();
                                                StringReader reader = new StringReader(Convert.ToString(saleLineItem3.PartnerData.Ingredients));
                                                dsIngredients.ReadXml(reader);
                                                foreach (DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                                {
                                                    studdamount += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : 0);
                                                }
                                            }

                                            sqlCmd.Parameters.Add(new SqlParameter("@PIECES", !string.IsNullOrEmpty(saleLineItem3.PartnerData.Pieces) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.Pieces) * -1) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@QUANTITY", !string.IsNullOrEmpty(saleLineItem3.PartnerData.Quantity) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.Quantity) * -1) : "0"));
                                            if (Convert.ToInt16(saleLineItem3.PartnerData.RateType) == 2) // Tot
                                                sqlCmd.Parameters.Add(new SqlParameter("@RATE", !string.IsNullOrEmpty(saleLineItem3.PartnerData.Rate) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.Rate) * -1) : "0"));
                                            else
                                                sqlCmd.Parameters.Add(new SqlParameter("@RATE", !string.IsNullOrEmpty(saleLineItem3.PartnerData.Rate) ? saleLineItem3.PartnerData.Rate : "0"));

                                            if (Convert.ToInt16(saleLineItem3.PartnerData.MakingType) == 3) // Tot //MAKINGRATE to MakingAmount changed on 221217
                                                sqlCmd.Parameters.Add(new SqlParameter("@MAKINGRATE", !string.IsNullOrEmpty(Convert.ToString(saleLineItem3.PartnerData.MakingAmount)) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.MakingAmount) * -1) : "0"));
                                            else
                                                sqlCmd.Parameters.Add(new SqlParameter("@MAKINGRATE", !string.IsNullOrEmpty(Convert.ToString(saleLineItem3.PartnerData.MakingAmount)) ? Convert.ToString(saleLineItem3.PartnerData.MakingAmount) : "0"));

                                            sqlCmd.Parameters.Add(new SqlParameter("@MAKINGAMOUNT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.MakingAmount) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.MakingAmount) * -1) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@AMOUNT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.Amount) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.Amount) * -1) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@TOTALAMOUNT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.TotalAmount) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.TotalAmount) * -1) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@TOTALWEIGHT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.TotalWeight) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.TotalWeight) * -1) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@LOSSWEIGHT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.LossWeight) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.LossWeight) * -1) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@EXPECTEDQUANTITY", !string.IsNullOrEmpty(saleLineItem3.PartnerData.ExpectedQuantity) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.ExpectedQuantity) * -1) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.MakingTotalDiscount) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.MakingTotalDiscount) * -1) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@STUDDAMOUNT", (studdamount * -1)));
                                            sqlCmd.Parameters.Add(new SqlParameter("@WastageQty", !string.IsNullOrEmpty(saleLineItem3.PartnerData.WastageQty) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.WastageQty) * -1) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@WastageAmount", !string.IsNullOrEmpty(saleLineItem3.PartnerData.WastageAmount) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.WastageAmount) * -1) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@TRANSFERCOSTPRICE", !string.IsNullOrEmpty(saleLineItem3.PartnerData.TRANSFERCOSTPRICE) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.TRANSFERCOSTPRICE) * -1) : "0"));//saleLineItem3.PartnerData.TRANSFERCOSTPRICE));
                                            sqlCmd.Parameters.Add(new SqlParameter("@GOLDTAXVALUE", !string.IsNullOrEmpty(saleLineItem3.PartnerData.LINEGOLDTAX) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.LINEGOLDTAX) * -1) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@WTDIFFDISCQTY", !string.IsNullOrEmpty(saleLineItem3.PartnerData.WTDIFFDISCQTY) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.WTDIFFDISCQTY) * -1) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@WTDIFFDISCAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.WTDIFFDISCAMT) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.WTDIFFDISCAMT) * -1) : "0"));
                                            //item.PartnerData.WTDIFFDISCQTY = Convert.ToString(dr["WTDIFFDISCQTY"]);
                                        }
                                        else
                                        {
                                            bIsSale = 0;
                                            decimal studdamount = 0;
                                            if (Convert.ToInt16(saleLineItem3.PartnerData.TransactionType) == 1)
                                                bIsPurchase = 1;

                                            if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem3.PartnerData.Ingredients)))
                                            {
                                                DataSet dsIngredients = new DataSet();
                                                StringReader reader = new StringReader(Convert.ToString(saleLineItem3.PartnerData.Ingredients));
                                                dsIngredients.ReadXml(reader);
                                                foreach (DataRow drIngredients in dsIngredients.Tables[0].Rows)
                                                {
                                                    studdamount += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["CValue"])) ? drIngredients["CValue"] : 0);
                                                }
                                            }


                                            saleLineItem3.PartnerData.PROMOCODE = string.Empty;
                                            saleLineItem3.PartnerData.SALESCHANNELTYPE = string.Empty;
                                            saleLineItem3.PartnerData.SellingCostPrice = 0;
                                            saleLineItem3.PartnerData.ItemIdParent = string.Empty;
                                            saleLineItem3.PartnerData.UpdatedCostPrice = 0;
                                            saleLineItem3.PartnerData.GroupCostPrice = 0;
                                            saleLineItem3.PartnerData.FLAT = 0;
                                            saleLineItem3.PartnerData.RETAILBATCHNO = "";


                                            sqlCmd.Parameters.Add(new SqlParameter("@PIECES", !string.IsNullOrEmpty(saleLineItem3.PartnerData.Pieces) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.Pieces))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@QUANTITY", !string.IsNullOrEmpty(saleLineItem3.PartnerData.Quantity) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.Quantity))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@RATE", !string.IsNullOrEmpty(saleLineItem3.PartnerData.Rate) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.Rate))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@MAKINGRATE", !string.IsNullOrEmpty(saleLineItem3.PartnerData.MakingAmount) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.MakingAmount))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@MAKINGAMOUNT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.MakingAmount) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.MakingAmount))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@AMOUNT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.Amount) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.Amount))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@TOTALAMOUNT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.TotalAmount) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.TotalAmount))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@TOTALWEIGHT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.TotalWeight) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.TotalWeight))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@LOSSWEIGHT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.LossWeight) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.LossWeight))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@EXPECTEDQUANTITY", !string.IsNullOrEmpty(saleLineItem3.PartnerData.ExpectedQuantity) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.ExpectedQuantity))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.MakingTotalDiscount) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.MakingTotalDiscount))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@STUDDAMOUNT", (studdamount)));
                                            sqlCmd.Parameters.Add(new SqlParameter("@WastageQty", !string.IsNullOrEmpty(saleLineItem3.PartnerData.WastageQty) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.WastageQty))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@WastageAmount", !string.IsNullOrEmpty(saleLineItem3.PartnerData.WastageAmount) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.WastageAmount))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@TRANSFERCOSTPRICE", saleLineItem3.PartnerData.TRANSFERCOSTPRICE));
                                            sqlCmd.Parameters.Add(new SqlParameter("@GOLDTAXVALUE", !string.IsNullOrEmpty(saleLineItem3.PartnerData.LINEGOLDTAX) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.LINEGOLDTAX))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@WTDIFFDISCQTY", !string.IsNullOrEmpty(saleLineItem3.PartnerData.WTDIFFDISCQTY) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.WTDIFFDISCQTY))) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@WTDIFFDISCAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.WTDIFFDISCAMT) ? Convert.ToString(Math.Abs(Convert.ToDecimal(saleLineItem3.PartnerData.WTDIFFDISCAMT))) : "0"));
                                            //item.PartnerData.WTDIFFDISCQTY = Convert.ToString(dr["WTDIFFDISCQTY"]);

                                        }
                                        #endregion

                                        #region Other common param
                                        sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));
                                        sqlCmd.Parameters.Add(new SqlParameter("@STOREID", posTransaction.StoreId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@TERMINALID", posTransaction.TerminalId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONID", posTransaction.TransactionId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@LINENUM", saleLineItem3.LineId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@RATETYPE", !string.IsNullOrEmpty(saleLineItem3.PartnerData.RateType) ? saleLineItem3.PartnerData.RateType : "0"));
                                        sqlCmd.Parameters.Add(new SqlParameter("@MAKINGTYPE", !string.IsNullOrEmpty(saleLineItem3.PartnerData.MakingType) ? saleLineItem3.PartnerData.MakingType : "0"));
                                        sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCOUNT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.MakingDisc) ? saleLineItem3.PartnerData.MakingDisc : "0"));
                                        sqlCmd.Parameters.Add(new SqlParameter("@LOSSPERCENTAGE", !string.IsNullOrEmpty(saleLineItem3.PartnerData.LossPct) ? saleLineItem3.PartnerData.LossPct : "0"));
                                        sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONTYPE", !string.IsNullOrEmpty(saleLineItem3.PartnerData.TransactionType) ? saleLineItem3.PartnerData.TransactionType : "0"));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OWN", Convert.ToBoolean(saleLineItem3.PartnerData.OChecked) ? "1" : "0"));
                                        sqlCmd.Parameters.Add(new SqlParameter("@SKUDETAILS", !string.IsNullOrEmpty(saleLineItem3.PartnerData.Ingredients) ? saleLineItem3.PartnerData.Ingredients : "0"));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ORDERNUM", !string.IsNullOrEmpty(saleLineItem3.PartnerData.OrderNum) ? saleLineItem3.PartnerData.OrderNum : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ORDERLINENUM", !string.IsNullOrEmpty(saleLineItem3.PartnerData.OrderLineNum) ? saleLineItem3.PartnerData.OrderLineNum : "0"));
                                        sqlCmd.Parameters.Add(new SqlParameter("@CUSTNO", !string.IsNullOrEmpty(saleLineItem3.PartnerData.CustNo) ? saleLineItem3.PartnerData.CustNo : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@isVoid", Convert.ToBoolean(saleLineItem3.Voided)));
                                        sqlCmd.Parameters.Add(new SqlParameter("@INVENTITEMID", Convert.ToString(saleLineItem3.ItemId)));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ITEMAMOUNT", Convert.ToString(saleLineItem3.NetAmount)));
                                        sqlCmd.Parameters.Add(new SqlParameter("@RETAILSTAFFID", posTransaction.OperatorId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ISINGREDIENT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.Ingredients) ? 1 : 0));

                                        //Advance Adjustment ID
                                        sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTID", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@SAMPLERETURN", saleLineItem3.PartnerData.SampleReturn));

                                        // Added for Wastage
                                        sqlCmd.Parameters.Add(new SqlParameter("@WastageType", saleLineItem3.PartnerData.WastageType));
                                        sqlCmd.Parameters.Add(new SqlParameter("@WastagePercentage", saleLineItem3.PartnerData.WastagePercentage));
                                        sqlCmd.Parameters.Add(new SqlParameter("@WastageRate", saleLineItem3.PartnerData.WastageRate));
                                        //
                                        // Making Discount Type
                                        sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCTYPE", saleLineItem3.PartnerData.MakingDiscountType));
                                        //
                                        sqlCmd.Parameters.Add(new SqlParameter("@CONFIGID", saleLineItem3.PartnerData.ConfigId)); //configid
                                        sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTSTOREID", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTTERMINALID", string.Empty));

                                        sqlCmd.Parameters.Add(new SqlParameter("@PURITY", saleLineItem3.PartnerData.Purity)); // PURITY
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGGROSSUNIT", saleLineItem3.PartnerData.GROSSUNIT));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGDMDPCS", saleLineItem3.PartnerData.DMDPCS));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGDMDWT", saleLineItem3.PartnerData.DMDWT));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGDMDUNIT", saleLineItem3.PartnerData.DMDUNIT));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGDMDAMT", saleLineItem3.PartnerData.DMDAMOUNT));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEPCS", saleLineItem3.PartnerData.STONEPCS));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEWT", saleLineItem3.PartnerData.STONEWT));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEUNIT", saleLineItem3.PartnerData.STONEUNIT));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEAMT", saleLineItem3.PartnerData.STONEAMOUNT));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGNETWT", saleLineItem3.PartnerData.NETWT));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGNETRATE", saleLineItem3.PartnerData.NETRATE));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGNETUNIT", saleLineItem3.PartnerData.NETUNIT));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGNETAMT", saleLineItem3.PartnerData.NETAMOUNT));
                                        sqlCmd.Parameters.Add(new SqlParameter("@REFINVOICENO", saleLineItem3.PartnerData.OGREFINVOICENO));
                                        sqlCmd.Parameters.Add(new SqlParameter("@SALESCHANNELTYPE", saleLineItem3.PartnerData.SALESCHANNELTYPE));

                                        sqlCmd.Parameters.Add(new SqlParameter("@SELLINGPRICE", saleLineItem3.PartnerData.SellingCostPrice));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ITEMIDPARENT", saleLineItem3.PartnerData.ItemIdParent));
                                        sqlCmd.Parameters.Add(new SqlParameter("@UPDATEDCOSTPRICE", saleLineItem3.PartnerData.UpdatedCostPrice));
                                        sqlCmd.Parameters.Add(new SqlParameter("@GROUPCOSTPRICE", saleLineItem3.PartnerData.GroupCostPrice));
                                        sqlCmd.Parameters.Add(new SqlParameter("@PROMOTIONCODE", saleLineItem3.PartnerData.PROMOCODE)); // added on 20/11/2014
                                        sqlCmd.Parameters.Add(new SqlParameter("@FLAT", saleLineItem3.PartnerData.FLAT)); // added on 28/12/2015
                                        sqlCmd.Parameters.Add(new SqlParameter("@SPECIALDISCINFO", saleLineItem3.PartnerData.SpecialDiscInfo)); // added on 28/12/2015
                                        sqlCmd.Parameters.Add(new SqlParameter("@RETAILBATCHNO", saleLineItem3.PartnerData.RETAILBATCHNO));
                                        #endregion

                                        #region Bulk Item
                                        if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                        {
                                            if (saleLineItem3.PartnerData.BulkItem == 1 && !saleLineItem3.Voided) // && saleLineItem3.ReturnLineId == 0
                                                SaveBulkItemTransDetails(sqlTransaction, retailTransaction, saleLineItem3);
                                        }
                                        #endregion

                                        sqlCmd.Parameters.Add(new SqlParameter("@ACTMKRATE", saleLineItem3.PartnerData.ACTMKRATE));

                                        if (Convert.ToInt16(saleLineItem3.PartnerData.TransactionType) == 1 ||
                                            Convert.ToInt16(saleLineItem3.PartnerData.TransactionType) == 3) // Req by S.Sharma on 15/03/17
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@ACTTOTAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.ACTTOTAMT) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.ACTTOTAMT)) : "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@CHANGEDTOTAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.CHANGEDTOTAMT) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.CHANGEDTOTAMT)) : "0"));

                                        }
                                        else
                                        {
                                            if (saleLineItem3.ReturnLineId != 0)
                                            {
                                                if (Convert.ToDecimal(saleLineItem3.PartnerData.ACTTOTAMT) < 0)
                                                {
                                                    if (iMetalType == (int)Enums.EnumClass.MetalType.PackingMaterial)
                                                        sqlCmd.Parameters.Add(new SqlParameter("@ACTTOTAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.TotalAmount) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.TotalAmount) * -1) : "0"));// saleLineItem3.PartnerData.ACTTOTAMT));
                                                    else
                                                        sqlCmd.Parameters.Add(new SqlParameter("@ACTTOTAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.ACTTOTAMT) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.ACTTOTAMT) * -1) : "0"));// saleLineItem3.PartnerData.ACTTOTAMT));

                                                    sqlCmd.Parameters.Add(new SqlParameter("@CHANGEDTOTAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.CHANGEDTOTAMT) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.CHANGEDTOTAMT) * -1) : "0"));//saleLineItem3.PartnerData.CHANGEDTOTAMT));
                                                }
                                                else
                                                {
                                                    if (iMetalType == (int)Enums.EnumClass.MetalType.PackingMaterial)
                                                        sqlCmd.Parameters.Add(new SqlParameter("@ACTTOTAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.TotalAmount) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.TotalAmount)) : "0"));// saleLineItem3.PartnerData.ACTTOTAMT));
                                                    else
                                                        sqlCmd.Parameters.Add(new SqlParameter("@ACTTOTAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.ACTTOTAMT) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.ACTTOTAMT)) : "0"));// saleLineItem3.PartnerData.ACTTOTAMT));

                                                    sqlCmd.Parameters.Add(new SqlParameter("@CHANGEDTOTAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.CHANGEDTOTAMT) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.CHANGEDTOTAMT)) : "0"));//saleLineItem3.PartnerData.CHANGEDTOTAMT));
                                                }
                                            }
                                            else
                                            {
                                                decimal dGoldTaxValue = 0m;
                                                dGoldTaxValue = !string.IsNullOrEmpty(saleLineItem3.PartnerData.LINEGOLDTAX) ? Convert.ToDecimal(saleLineItem3.PartnerData.LINEGOLDTAX) : 0;

                                                sqlCmd.Parameters.Add(new SqlParameter("@ACTTOTAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.ACTTOTAMT) ? Convert.ToString((Convert.ToDecimal(saleLineItem3.PartnerData.ACTTOTAMT) + dGoldTaxValue) * -1) : "0"));// saleLineItem3.PartnerData.ACTTOTAMT));
                                                sqlCmd.Parameters.Add(new SqlParameter("@CHANGEDTOTAMT", !string.IsNullOrEmpty(saleLineItem3.PartnerData.CHANGEDTOTAMT) ? Convert.ToString(Convert.ToDecimal(saleLineItem3.PartnerData.CHANGEDTOTAMT) * -1) : "0"));//saleLineItem3.PartnerData.CHANGEDTOTAMT));
                                            }
                                        }

                                        sqlCmd.Parameters.Add(new SqlParameter("@LINEDISC", saleLineItem3.PartnerData.LINEDISC));


                                        #region  if(retailTransaction.SaleIsReturnSale == true)
                                        if (saleLineItem3.ReturnLineId != 0)//retailTransaction.SaleIsReturnSale == true
                                        {
                                            //sqlCmd.Parameters.Add(new SqlParameter("@ACTMKRATE", "0"));
                                            //sqlCmd.Parameters.Add(new SqlParameter("@ACTTOTAMT", "0"));// saleLineItem3.PartnerData.ACTTOTAMT));
                                            //sqlCmd.Parameters.Add(new SqlParameter("@CHANGEDTOTAMT", "0"));//saleLineItem3.PartnerData.CHANGEDTOTAMT));
                                            //sqlCmd.Parameters.Add(new SqlParameter("@LINEDISC", "0"));

                                            //OG new fld
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGREFBATCHNO", ""));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGCHANGEDGROSSWT", "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGDMDRATE", "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEATE", "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGGROSSAMT", "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGACTAMT", "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGCHANGEDAMT", "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGFINALAMT", "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@FGPROMOCODE", ""));
                                            sqlCmd.Parameters.Add(new SqlParameter("@ISREPAIR", "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@REPAIRBATCHID", ""));

                                        }
                                        else
                                        {

                                            //OG new fld
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGREFBATCHNO", saleLineItem3.PartnerData.OGREFBATCHNO));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGCHANGEDGROSSWT", saleLineItem3.PartnerData.OGCHANGEDGROSSWT));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGDMDRATE", saleLineItem3.PartnerData.OGDMDRATE));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEATE", saleLineItem3.PartnerData.OGSTONEATE));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGGROSSAMT", saleLineItem3.PartnerData.OGGROSSAMT));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGACTAMT", saleLineItem3.PartnerData.OGACTAMT));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGCHANGEDAMT", saleLineItem3.PartnerData.OGCHANGEDAMT));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OGFINALAMT", saleLineItem3.PartnerData.OGFINALAMT));
                                            sqlCmd.Parameters.Add(new SqlParameter("@FGPROMOCODE", saleLineItem3.PartnerData.FGPROMOCODE));
                                            sqlCmd.Parameters.Add(new SqlParameter("@ISREPAIR", saleLineItem3.PartnerData.ISREPAIR));
                                            sqlCmd.Parameters.Add(new SqlParameter("@REPAIRBATCHID", saleLineItem3.PartnerData.REPAIRBATCHID));
                                        }
                                        #endregion

                                        sqlCmd.Parameters.Add(new SqlParameter("@BatchId", !string.IsNullOrEmpty(retailTransaction.PartnerData.REPAIRID) ? retailTransaction.PartnerData.REPAIRID : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@RepairId", !string.IsNullOrEmpty(retailTransaction.PartnerData.RefRepairId) ? retailTransaction.PartnerData.RefRepairId : string.Empty));

                                        //added on 070119
                                        sqlCmd.Parameters.Add(new SqlParameter("@PURITYREADING1", saleLineItem3.PartnerData.PurityReading1));
                                        sqlCmd.Parameters.Add(new SqlParameter("@PURITYREADING2", saleLineItem3.PartnerData.PurityReading2));
                                        sqlCmd.Parameters.Add(new SqlParameter("@PURITYREADING3", saleLineItem3.PartnerData.PurityReading3));
                                        sqlCmd.Parameters.Add(new SqlParameter("@PURITYPERSON", !string.IsNullOrEmpty(saleLineItem3.PartnerData.PURITYPERSON) ? Convert.ToString((saleLineItem3.PartnerData.PURITYPERSON)) : ""));
                                        sqlCmd.Parameters.Add(new SqlParameter("@PURITYPERSONNAME", !string.IsNullOrEmpty(saleLineItem3.PartnerData.PURITYPERSONNAME) ? Convert.ToString((saleLineItem3.PartnerData.PURITYPERSONNAME)) : ""));

                                        sqlCmd.Parameters.Add(new SqlParameter("@SKUAgingDiscType", saleLineItem3.PartnerData.SKUAgingDiscType));
                                        sqlCmd.Parameters.Add(new SqlParameter("@SKUAgingDiscAmt", saleLineItem3.PartnerData.SKUAgingDiscAmt));

                                        sqlCmd.Parameters.Add(new SqlParameter("@TierDiscType", saleLineItem3.PartnerData.TierDiscType));
                                        sqlCmd.Parameters.Add(new SqlParameter("@TierDiscAmt", saleLineItem3.PartnerData.TierDiscAmt));

                                        if (saleLineItem3.PartnerData.NimMakingDiscType == true)
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPOrMkDiscType", Convert.ToString((int)MRPOrMkDiscType.Making)));
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPOrMkDiscAmt", saleLineItem3.PartnerData.NimMakingDisc));
                                        }
                                        else if (saleLineItem3.PartnerData.NimMRPDiscType == true)
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPOrMkDiscType", Convert.ToString((int)MRPOrMkDiscType.MRP)));
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPOrMkDiscAmt", saleLineItem3.PartnerData.NimMRPDisc));
                                        }
                                        else
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPOrMkDiscType", Convert.ToString((int)MRPOrMkDiscType.None)));
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPOrMkDiscAmt", "0"));
                                        }

                                        sqlCmd.Parameters.Add(new SqlParameter("@OGPSTONEPCS", saleLineItem3.PartnerData.OGPSTONEPCS));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGPSTONEWT", saleLineItem3.PartnerData.OGPSTONEWT));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGPSTONEUNIT", saleLineItem3.PartnerData.OGPSTONEUNIT));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGPSTONERATE", saleLineItem3.PartnerData.OGPSTONERATE));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGPSTONEAMOUNT", saleLineItem3.PartnerData.OGPSTONEAMOUNT));

                                        if (saleLineItem3.PartnerData.NimPromoMakingDiscType == true)
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPORMKPROMODISCTYPE", Convert.ToString((int)MRPOrMkDiscType.Making)));
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPORMKPROMODISCAMT", saleLineItem3.PartnerData.NimPromoMakingDisc));
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPORMKPROMODISCCODE", saleLineItem3.PartnerData.PROMOCODE));
                                        }
                                        else if (saleLineItem3.PartnerData.NimPromoMRPDiscType == true)
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPORMKPROMODISCTYPE", Convert.ToString((int)MRPOrMkDiscType.MRP)));
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPORMKPROMODISCAMT", saleLineItem3.PartnerData.NimMRPDisc));
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPORMKPROMODISCCODE", saleLineItem3.PartnerData.PROMOCODE));
                                        }
                                        else
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPORMKPROMODISCTYPE", Convert.ToString((int)MRPOrMkDiscType.None)));
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPORMKPROMODISCAMT", "0"));
                                            sqlCmd.Parameters.Add(new SqlParameter("@MRPORMKPROMODISCCODE", ""));
                                        }

                                        if (saleLineItem3.PartnerData.OpeningDisc > 0)
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@OpeningDiscType", Convert.ToString(saleLineItem3.PartnerData.OpeningDiscType)));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OpeningDisc", saleLineItem3.PartnerData.OpeningDisc));
                                        }
                                        else
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@OpeningDiscType", Convert.ToString((int)MRPOrMkDiscType.None)));
                                            sqlCmd.Parameters.Add(new SqlParameter("@OpeningDisc", "0"));
                                        }

                                        sqlCmd.Parameters.Add(new SqlParameter("@MakStnDiaDiscType", Convert.ToString(saleLineItem3.PartnerData.MakStnDiaDiscType)));
                                        sqlCmd.Parameters.Add(new SqlParameter("@MakingDiscAmt1", saleLineItem3.PartnerData.NimMakingDiscount));
                                        sqlCmd.Parameters.Add(new SqlParameter("@StoneDiscAmt1", saleLineItem3.PartnerData.NimStoneDiscount));
                                        sqlCmd.Parameters.Add(new SqlParameter("@DiamondDiscAmt1", saleLineItem3.PartnerData.NimDiamondDiscount));


                                        //item.PartnerData.TierDiscType = Convert.ToInt16(dr["TierDiscType"]);
                                        //item.PartnerData.TierDiscAmt = Convert.ToDecimal(dr["TierDiscAmt"]); 


                                        sqlCmd.ExecuteNonQuery();
                                    }
                                }

                                #endregion
                            }
                            else
                            {
                                #region Service Item But Not Voided
                                if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                {
                                    StringBuilder sbAdjustment = new StringBuilder();
                                    //sbAdjustment.Append(" INSERT INTO RETAIL_CUSTOMCALCULATIONS_TABLE VALUES ");
                                    sbAdjustment.Append(" INSERT INTO RETAIL_CUSTOMCALCULATIONS_TABLE ");
                                    sbAdjustment.Append(" ([DATAAREAID],[STOREID],[TERMINALID],[TRANSACTIONID],[LINENUM],[PIECES],[QUANTITY], ");
                                    sbAdjustment.Append(" [RATETYPE],[MAKINGRATE],[MAKINGTYPE],[AMOUNT],[MAKINGDISCOUNT],[MAKINGAMOUNT],[TOTALAMOUNT], ");
                                    sbAdjustment.Append(" [TOTALWEIGHT],[LOSSPERCENTAGE],[LOSSWEIGHT],[EXPECTEDQUANTITY],[TRANSACTIONTYPE],[OWN],[SKUDETAILS], ");
                                    sbAdjustment.Append(" [ORDERNUM],[ORDERLINENUM],[CUSTNO],[RETAILSTAFFID],[IsIngredient],[MakingDiscountAmount],");
                                    sbAdjustment.Append(" [STUDDAMOUNT],[CRate],[ADVANCEADJUSTMENTID],[SAMPLERETURN],[WastageType],[WastageQty],");
                                    sbAdjustment.Append(" [WastageAmount],[WastagePercentage],[WastageRate],[MAKINGDISCTYPE],[CONFIGID], ");
                                    sbAdjustment.Append(" [ADVANCEADJUSTMENTSTOREID],[ADVANCEADJUSTMENTTERMINALID],[PURITY]"); // Changed for wastage and Making Discount
                                    sbAdjustment.Append(" ,OGGROSSUNIT,OGDMDPCS,OGDMDWT,OGDMDUNIT,OGDMDAMT,OGSTONEPCS,OGSTONEWT,OGSTONEUNIT,OGSTONEAMT,");
                                    sbAdjustment.Append(" OGNETWT,OGNETRATE,OGNETUNIT,OGNETAMT,REFINVOICENO,SALESCHANNELTYPE,");
                                    sbAdjustment.Append(" SELLINGPRICE,ITEMIDPARENT,UPDATEDCOSTPRICE,GROUPCOSTPRICE,PROMOTIONCODE,FLAT,SPECIALDISCINFO,RETAILBATCHNO,");
                                    sbAdjustment.Append(" ACTMKRATE,ACTTOTAMT,CHANGEDTOTAMT,LINEDISC,");
                                    sbAdjustment.Append(" OGREFBATCHNO,OGCHANGEDGROSSWT,OGDMDRATE,OGSTONEATE,OGGROSSAMT,OGACTAMT,OGCHANGEDAMT,OGFINALAMT,FGPROMOCODE,");
                                    sbAdjustment.Append(" ISREPAIR,REPAIRBATCHID,OGPSTONEPCS,OGPSTONEWT,OGPSTONEUNIT,OGPSTONERATE,OGPSTONEAMOUNT)");


                                    //  " ,LocalCustomerName,LocalCustomerAddress,LocalCustomerContactNo)" +
                                    sbAdjustment.Append(" VALUES ");

                                    sbAdjustment.Append(" (@DATAAREAID,@STOREID,@TERMINALID,@TRANSACTIONID,@LINENUM,@PIECES,@QUANTITY,");
                                    sbAdjustment.Append(" @RATETYPE,@MAKINGRATE,@MAKINGTYPE,@AMOUNT,@MAKINGDISCOUNT,@MAKINGAMOUNT,@TOTALAMOUNT, ");
                                    sbAdjustment.Append(" @TOTALWEIGHT,@LOSSPERCENTAGE,@LOSSWEIGHT,@EXPECTEDQUANTITY,@TRANSACTIONTYPE,@OWN,@SKUDETAILS, ");
                                    sbAdjustment.Append(" @ORDERNUM,@ORDERLINENUM,@CUSTNO,@RETAILSTAFFID,@ISINGREDIENT,@MAKINGDISCAMT, ");
                                    sbAdjustment.Append(" @STUDDAMOUNT,@RATE,@ADVANCEADJUSTMENTID,@SAMPLERETURN,@WastageType,@WastageQty,");
                                    sbAdjustment.Append(" @WastageAmount,@WastagePercentage,@WastageRate,@MAKINGDISCTYPE,@CONFIGID,");
                                    sbAdjustment.Append(" @ADVANCEADJUSTMENTSTOREID,@ADVANCEADJUSTMENTTERMINALID,@PURITY,");
                                    sbAdjustment.Append(" @OGGROSSUNIT,@OGDMDPCS,@OGDMDWT,@OGDMDUNIT,@OGDMDAMT,@OGSTONEPCS,@OGSTONEWT,@OGSTONEUNIT,@OGSTONEAMT,");
                                    sbAdjustment.Append(" @OGNETWT,@OGNETRATE,@OGNETUNIT,@OGNETAMT,@REFINVOICENO,@SALESCHANNELTYPE,");
                                    sbAdjustment.Append(" @SELLINGPRICE,@ITEMIDPARENT,@UPDATEDCOSTPRICE,@GROUPCOSTPRICE,@PROMOTIONCODE,@FLAT,@SPECIALDISCINFO,@RETAILBATCHNO,");
                                    sbAdjustment.Append(" @ACTMKRATE,@ACTTOTAMT,@CHANGEDTOTAMT,@LINEDISC,");
                                    sbAdjustment.Append(" @OGREFBATCHNO,@OGCHANGEDGROSSWT,@OGDMDRATE,@OGSTONEATE,@OGGROSSAMT,");
                                    sbAdjustment.Append(" @OGACTAMT,@OGCHANGEDAMT,@OGFINALAMT,@FGPROMOCODE,@ISREPAIR,@REPAIRBATCHID,");
                                    sbAdjustment.Append(" @OGSTONEPCS,@OGPSTONEWT,@OGPSTONEUNIT,@OGPSTONERATE,@OGPSTONEAMOUNT)");

                                    if (!retailTransaction.PartnerData.IsGSSMaturity)
                                    {
                                        sbAdjustment.Append(" UPDATE RETAILTRANSACTIONPAYMENTTRANS SET ");
                                        sbAdjustment.Append(" isAdjusted = @ADJUSTED WHERE TRANSACTIONID =@TRANSID AND STORE = @RETAILSTOREID AND TERMINAL = @RETAILTERMINALID; ");
                                        sbAdjustment.Append(" UPDATE RETAILADJUSTMENTTABLE SET ");
                                        sbAdjustment.Append(" isAdjusted = @ADJUSTED WHERE TRANSACTIONID = @TRANSID AND RETAILSTOREID = @RETAILSTOREID AND RETAILTERMINALID = @RETAILTERMINALID");
                                    }



                                    using (SqlCommand sqlCmd = new SqlCommand(sbAdjustment.ToString(), sqlTransaction.Connection, sqlTransaction))
                                    {
                                        if (!retailTransaction.PartnerData.IsGSSMaturity)
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@TRANSID", Convert.ToString(saleLineItem3.PartnerData.ServiceItemCashAdjustmentTransactionID)));

                                            sqlCmd.Parameters.Add(new SqlParameter("@RETAILSTOREID", Convert.ToString(saleLineItem3.PartnerData.ServiceItemCashAdjustmentStoreId)));
                                            sqlCmd.Parameters.Add(new SqlParameter("@RETAILTERMINALID", Convert.ToString(saleLineItem3.PartnerData.ServiceItemCashAdjustmentTerminalId)));

                                            if (saleLineItem3.Voided)
                                                sqlCmd.Parameters.Add(new SqlParameter("@ADJUSTED", Convert.ToInt16(0)));
                                            else
                                                sqlCmd.Parameters.Add(new SqlParameter("@ADJUSTED", Convert.ToInt16(1)));
                                        }

                                        sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));
                                        sqlCmd.Parameters.Add(new SqlParameter("@STOREID", posTransaction.StoreId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@TERMINALID", posTransaction.TerminalId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONID", posTransaction.TransactionId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@LINENUM", saleLineItem3.LineId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@PIECES", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@QUANTITY", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@RATE", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@RATETYPE", "0"));
                                        sqlCmd.Parameters.Add(new SqlParameter("@MAKINGRATE", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@MAKINGTYPE", "0"));
                                        sqlCmd.Parameters.Add(new SqlParameter("@AMOUNT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCOUNT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@MAKINGAMOUNT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@TOTALAMOUNT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@TOTALWEIGHT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@LOSSPERCENTAGE", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@LOSSWEIGHT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@EXPECTEDQUANTITY", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONTYPE", (int)Enums.EnumClass.TransactionType.Adjustment));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OWN", Convert.ToInt16("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@SKUDETAILS", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ORDERNUM", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ORDERLINENUM", "0"));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ITEMAMOUNT", Convert.ToString(saleLineItem3.NetAmount)));
                                        sqlCmd.Parameters.Add(new SqlParameter("@RETAILSTAFFID", posTransaction.OperatorId));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ISINGREDIENT", iIngredient));
                                        sqlCmd.Parameters.Add(new SqlParameter("@CUSTNO", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCAMT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@STUDDAMOUNT", Convert.ToDecimal("0")));

                                        if (retailTransaction.PartnerData.IsGSSMaturity)
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTID", string.Empty));
                                            sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTSTOREID", string.Empty));
                                            sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTTERMINALID", string.Empty));
                                        }
                                        else
                                        {
                                            sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTID", !string.IsNullOrEmpty(saleLineItem3.PartnerData.ServiceItemCashAdjustmentTransactionID) ? saleLineItem3.PartnerData.ServiceItemCashAdjustmentTransactionID : string.Empty));
                                            sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTSTOREID", !string.IsNullOrEmpty(saleLineItem3.PartnerData.ServiceItemCashAdjustmentStoreId) ? saleLineItem3.PartnerData.ServiceItemCashAdjustmentStoreId : string.Empty));
                                            sqlCmd.Parameters.Add(new SqlParameter("@ADVANCEADJUSTMENTTERMINALID", !string.IsNullOrEmpty(saleLineItem3.PartnerData.ServiceItemCashAdjustmentTerminalId) ? saleLineItem3.PartnerData.ServiceItemCashAdjustmentTerminalId : string.Empty));
                                        }
                                        sqlCmd.Parameters.Add(new SqlParameter("@SAMPLERETURN", Convert.ToInt16("0")));

                                        sqlCmd.Parameters.Add(new SqlParameter("@WastageType", Convert.ToInt16("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@WastageQty", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@WastageAmount", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@WastagePercentage", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@WastageRate", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@MAKINGDISCTYPE", Convert.ToInt16("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@CONFIGID", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@PURITY", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGGROSSUNIT", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGDMDPCS", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGDMDWT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGDMDUNIT", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGDMDAMT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEPCS", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEWT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEUNIT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEAMT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGNETWT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGNETRATE", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGNETUNIT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGNETAMT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@REFINVOICENO", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@SALESCHANNELTYPE", string.Empty));

                                        sqlCmd.Parameters.Add(new SqlParameter("@SELLINGPRICE", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ITEMIDPARENT", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@UPDATEDCOSTPRICE", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@GROUPCOSTPRICE", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@PROMOTIONCODE", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@FLAT", Convert.ToInt16("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@SPECIALDISCINFO", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@RETAILBATCHNO", string.Empty));

                                        sqlCmd.Parameters.Add(new SqlParameter("@ACTMKRATE", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ACTTOTAMT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@CHANGEDTOTAMT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@LINEDISC", Convert.ToDecimal("0")));

                                        sqlCmd.Parameters.Add(new SqlParameter("@OGREFBATCHNO", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGCHANGEDGROSSWT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGDMDRATE", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGSTONEATE", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGGROSSAMT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGACTAMT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGCHANGEDAMT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGFINALAMT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@FGPROMOCODE", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@ISREPAIR", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@REPAIRBATCHID", string.Empty));

                                        sqlCmd.Parameters.Add(new SqlParameter("@BatchId", !string.IsNullOrEmpty(retailTransaction.PartnerData.REPAIRID) ? retailTransaction.PartnerData.REPAIRID : string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@RepairId", !string.IsNullOrEmpty(retailTransaction.PartnerData.RefRepairId) ? retailTransaction.PartnerData.RefRepairId : string.Empty));

                                        sqlCmd.Parameters.Add(new SqlParameter("@OGPSTONEPCS", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGPSTONEWT", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGPSTONEUNIT", string.Empty));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGPSTONERATE", Convert.ToDecimal("0")));
                                        sqlCmd.Parameters.Add(new SqlParameter("@OGPSTONEAMOUNT", Convert.ToDecimal("0")));

                                        sqlCmd.ExecuteNonQuery();
                                    }
                                }

                                #endregion
                            }
                        }

                        #region Local Customer save
                        if (!string.IsNullOrEmpty(Convert.ToString(retailTransaction.PartnerData.LCCustomerName)))
                        {
                            if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                            {
                                string sLCName = Convert.ToString(retailTransaction.PartnerData.LCCustomerName);
                                //string sLCAddress = Convert.ToString(retailTransaction.PartnerData.LCCustomerAddress);
                                //string sLCContactNo = Convert.ToString(retailTransaction.PartnerData.LCCustomerContactNo);

                                //string sLCQry = " UPDATE RETAILTRANSACTIONTABLE SET LocalCustomerName = '" + sLCName + "', LocalCustomerAddress = '" + sLCAddress + "'," +
                                //                " LocalCustomerContactNo = '" + sLCContactNo + "' WHERE TRANSACTIONID='" + retailTransaction.TransactionId + "' AND TERMINAL = '" + ApplicationSettings.Terminal.TerminalId + "'";
                                string sLCQry = " UPDATE RETAILTRANSACTIONTABLE SET LocalCustomerName = '" + sLCName + "' WHERE TRANSACTIONID='" + retailTransaction.TransactionId + "' AND TERMINAL = '" + ApplicationSettings.Terminal.TerminalId + "'";

                                using (SqlCommand cmdLC = new SqlCommand(sLCQry, sqlTransaction.Connection, sqlTransaction))
                                {
                                    cmdLC.ExecuteNonQuery();
                                }
                            }
                        }
                        #endregion

                        #region SalesAssociateCode/BussinesAgentCode save
                        if (!string.IsNullOrEmpty(Convert.ToString(retailTransaction.PartnerData.SalesAssociateCode)))
                        {
                            if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                            {
                                string sSalesAssociateCode = Convert.ToString(retailTransaction.PartnerData.SalesAssociateCode);
                                string sBussinesAgentCode = Convert.ToString(retailTransaction.PartnerData.BussinesAgentCode);

                                string sLCQry = " UPDATE RETAILTRANSACTIONTABLE SET SalesAssociateCode = '" + sSalesAssociateCode + "'," +
                                                " BussinesAgentCode = '" + sBussinesAgentCode + "'" +
                                                " WHERE TRANSACTIONID='" + retailTransaction.TransactionId + "'" +
                                                " AND TERMINAL = '" + ApplicationSettings.Terminal.TerminalId + "'";

                                using (SqlCommand cmdLC = new SqlCommand(sLCQry, sqlTransaction.Connection, sqlTransaction))
                                {
                                    cmdLC.ExecuteNonQuery();
                                }
                            }
                        }
                        #endregion


                        #region PAN Save
                        if (!string.IsNullOrEmpty(sSalesInfoCode))
                        {
                            string sLCQry = " UPDATE RETAILTRANSACTIONTABLE SET PANNO = '" + sSalesInfoCode + "'" +
                                            " WHERE TRANSACTIONID='" + retailTransaction.TransactionId + "'" +
                                            " AND TERMINAL = '" + ApplicationSettings.Terminal.TerminalId + "'";

                            using (SqlCommand cmdLC = new SqlCommand(sLCQry, sqlTransaction.Connection, sqlTransaction))
                            {
                                cmdLC.ExecuteNonQuery();
                            }
                        }
                        #endregion

                        if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                        {
                            #region Credit memo Update
                            if (bIsPurchase == 0)
                            {
                                foreach (var saleTender in retailTransaction.TenderLines)
                                {
                                    CreditMemoTenderLineItem creditMemoItem = saleTender as CreditMemoTenderLineItem;
                                    if (creditMemoItem != null && saleTender.Amount > 0)
                                    {
                                        string sStroreId = ApplicationSettings.Database.StoreID;
                                        string sTerminalId = ApplicationSettings.Database.TerminalID;
                                        string sTransId = retailTransaction.TransactionId;
                                        string sReceiptId = retailTransaction.ReceiptId;
                                        string sCMNO = creditMemoItem.SerialNumber;// Convert.ToString(dtCMInfo.Rows[i - 1]["CARDNO"]);
                                        #region Credit memo update for Advance
                                        try
                                        {
                                            // LogMessage("Marking a credit memo as used....", LogTraceLevel.Trace, "CreditMemo.UpdateCreditMemo");
                                            bool retVal = false;
                                            string comment = string.Empty;

                                            try
                                            {
                                                // Begin by checking if there is a connection to the Transaction Service
                                                if (this.Application.TransactionServices.CheckConnection())
                                                {

                                                    this.Application.TransactionServices.UpdateCreditMemo(ref retVal, ref comment, sCMNO,
                                                            ApplicationSettings.Terminal.StoreId,
                                                            ApplicationSettings.Terminal.TerminalId,
                                                            ApplicationSettings.Terminal.TerminalOperator.OperatorId,
                                                            sTransId,
                                                            posTransaction.ReceiptId,
                                                            "1",
                                                            saleTender.Amount,//Convert.ToDecimal(dtCMInfo.Rows[i - 1]["AMOUNT"]),
                                                            DateTime.Now);
                                                }
                                                else
                                                {
                                                    sqlTransaction.Rollback();
                                                    sqlTransaction.Dispose();
                                                    // Application.RunOperation(PosisOperations.VoidTransaction, retailTransaction);
                                                    MessageBox.Show(string.Format("Failed due to RTS connection "));
                                                    throw new RuntimeBinderException();
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                ReadOnlyCollection<object> containerArray1;
                                                containerArray1 = this.Application.TransactionServices.InvokeExtension("CreditMemoAppliedCheck",
                                                                                                                       sCMNO,
                                                                                                                       ApplicationSettings.Terminal.StoreId,
                                                                                                                       ApplicationSettings.Terminal.TerminalId, sTransId);

                                                bool bResult1 = Convert.ToBoolean(containerArray1[1]);

                                                if (!bResult1)
                                                {
                                                    sqlTransaction.Rollback();
                                                    sqlTransaction.Dispose();
                                                    //Application.RunOperation(PosisOperations.VoidTransaction, retailTransaction);
                                                    MessageBox.Show(string.Format("Credit memo adjusted failed due to RTS transaction issue."));
                                                    throw new RuntimeBinderException();
                                                }

                                            }

                                            if (retVal == false)
                                            {
                                                ReadOnlyCollection<object> containerArray1;
                                                containerArray1 = this.Application.TransactionServices.InvokeExtension("CreditMemoAppliedCheck",
                                                                                                                       sCMNO,
                                                                                                                       ApplicationSettings.Terminal.StoreId,
                                                                                                                       ApplicationSettings.Terminal.TerminalId, sTransId);

                                                bool bResult1 = Convert.ToBoolean(containerArray1[1]);

                                                if (!bResult1)
                                                {
                                                    sqlTransaction.Rollback();
                                                    sqlTransaction.Dispose();
                                                    // Application.RunOperation(PosisOperations.VoidTransaction, retailTransaction);
                                                    MessageBox.Show(string.Format("Credit memo adjusted failed due to RTS transaction issue."));
                                                    throw new RuntimeBinderException();
                                                }
                                            }
                                        }
                                        catch (Exception x)
                                        {
                                            LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                                            throw;
                                        }
                                        #endregion
                                    }
                                }
                            }
                            #endregion

                            #region  GSS Maturity // added on 15/09/2016

                            if (retailTransaction.PartnerData.IsGSSMaturity)
                            {
                                string sGSSNo = Convert.ToString(retailTransaction.PartnerData.GSSMaturityNo);
                                if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                {
                                    try
                                    {
                                        if (this.Application.TransactionServices.CheckConnection())
                                        {
                                            bool bResult = false;


                                            ReadOnlyCollection<object> containerArray;
                                            containerArray = this.Application.TransactionServices.InvokeExtension("UpdateGSSMaturityAdjustment",
                                                                                                                  sGSSNo, true, DateTime.Now, ((LSRetailPosis.Transaction.PosTransaction)(retailTransaction)).ReceiptId);

                                            bResult = Convert.ToBoolean(containerArray[1]);
                                            if (!bResult)
                                            {
                                                ReadOnlyCollection<object> containerArray1;
                                                containerArray1 = this.Application.TransactionServices.InvokeExtension("GSSAdjustedCheck",
                                                                                                                      sGSSNo, ((LSRetailPosis.Transaction.PosTransaction)(retailTransaction)).ReceiptId);

                                                bool bResult1 = Convert.ToBoolean(containerArray1[1]);

                                                if (!bResult1)
                                                {
                                                    sqlTransaction.Rollback();
                                                    sqlTransaction.Dispose();
                                                    //Application.RunOperation(PosisOperations.VoidTransaction, retailTransaction);
                                                    MessageBox.Show(string.Format("Failed to update GSS adjustment due to RTS transaction issue"));
                                                    throw new RuntimeBinderException();
                                                }
                                                else
                                                {
                                                    #region IsGSSMaturity==true, INSERT INTO RETAILGSSMATURITYADJUSTMENT
                                                    if (retailTransaction.PartnerData.IsGSSMaturity)
                                                    {
                                                        if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                                        {

                                                            string sGSS = "SET DATEFORMAT DMY;  UPDATE GSSACCOUNTOPENINGPOSTED" +
                                                                                     " SET GSSADJUSTED = 1,GSSADJUSTEDDATE = '" + DateTime.Now + "'" +
                                                                                     " WHERE GSSACCOUNTNO = '" + Convert.ToString(retailTransaction.PartnerData.GSSMaturityNo) + "';";

                                                            sGSS += "  INSERT INTO RETAILGSSMATURITYADJUSTMENT (TRANSACTIONID,GSSACCOUNTNO,CUSTACCOUNT,GSSTOTALQTY,GSSTOTALAMOUNT," +
                                                                                " GSSAVGRATE,GSSROYALITYAMOUNT,STOREID,TERMINALID,STAFFID,DATAAREAID) VALUES (@TRANSACTIONID,@GSSACCOUNTNO,@CUSTACCOUNT,@GSSTOTALQTY,@GSSTOTALAMOUNT," +
                                                                                " @GSSAVGRATE,@GSSROYALITYAMOUNT,@STOREID,@TERMINALID,@STAFFID,@DATAAREAID);";

                                                            using (SqlCommand sqlCmd = new SqlCommand(sGSS.ToString(), sqlTransaction.Connection, sqlTransaction))
                                                            {
                                                                sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONID", posTransaction.TransactionId));
                                                                sqlCmd.Parameters.Add(new SqlParameter("@GSSACCOUNTNO", retailTransaction.PartnerData.GSSMaturityNo));
                                                                sqlCmd.Parameters.Add(new SqlParameter("@CUSTACCOUNT", retailTransaction.Customer.CustomerId));

                                                                sqlCmd.Parameters.Add(new SqlParameter("@GSSTOTALQTY", retailTransaction.PartnerData.GSSTotQty));
                                                                sqlCmd.Parameters.Add(new SqlParameter("@GSSTOTALAMOUNT", retailTransaction.PartnerData.GSSTotAmt));
                                                                sqlCmd.Parameters.Add(new SqlParameter("@GSSAVGRATE", retailTransaction.PartnerData.GSSAvgRate));
                                                                sqlCmd.Parameters.Add(new SqlParameter("@GSSROYALITYAMOUNT", retailTransaction.PartnerData.GSSRoyaltyAmt));

                                                                sqlCmd.Parameters.Add(new SqlParameter("@STOREID", posTransaction.StoreId));
                                                                sqlCmd.Parameters.Add(new SqlParameter("@TERMINALID", posTransaction.TerminalId));
                                                                sqlCmd.Parameters.Add(new SqlParameter("@STAFFID", posTransaction.OperatorId));
                                                                sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));

                                                                sqlCmd.ExecuteNonQuery();
                                                            }

                                                        }
                                                    }


                                                    #endregion
                                                }
                                            }
                                            else
                                            {
                                                #region IsGSSMaturity==true, INSERT INTO RETAILGSSMATURITYADJUSTMENT
                                                if (retailTransaction.PartnerData.IsGSSMaturity)
                                                {
                                                    if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                                    {

                                                        string sGSS = "SET DATEFORMAT DMY;  UPDATE GSSACCOUNTOPENINGPOSTED" +
                                                                                 " SET GSSADJUSTED = 1,GSSADJUSTEDDATE = '" + DateTime.Now + "'" +
                                                                                 " WHERE GSSACCOUNTNO = '" + Convert.ToString(retailTransaction.PartnerData.GSSMaturityNo) + "';";

                                                        sGSS += "  INSERT INTO RETAILGSSMATURITYADJUSTMENT (TRANSACTIONID,GSSACCOUNTNO,CUSTACCOUNT,GSSTOTALQTY,GSSTOTALAMOUNT," +
                                                                            " GSSAVGRATE,GSSROYALITYAMOUNT,STOREID,TERMINALID,STAFFID,DATAAREAID) VALUES (@TRANSACTIONID,@GSSACCOUNTNO,@CUSTACCOUNT,@GSSTOTALQTY,@GSSTOTALAMOUNT," +
                                                                            " @GSSAVGRATE,@GSSROYALITYAMOUNT,@STOREID,@TERMINALID,@STAFFID,@DATAAREAID);";

                                                        using (SqlCommand sqlCmd = new SqlCommand(sGSS.ToString(), sqlTransaction.Connection, sqlTransaction))
                                                        {
                                                            sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONID", posTransaction.TransactionId));
                                                            sqlCmd.Parameters.Add(new SqlParameter("@GSSACCOUNTNO", retailTransaction.PartnerData.GSSMaturityNo));
                                                            sqlCmd.Parameters.Add(new SqlParameter("@CUSTACCOUNT", retailTransaction.Customer.CustomerId));

                                                            sqlCmd.Parameters.Add(new SqlParameter("@GSSTOTALQTY", retailTransaction.PartnerData.GSSTotQty));
                                                            sqlCmd.Parameters.Add(new SqlParameter("@GSSTOTALAMOUNT", retailTransaction.PartnerData.GSSTotAmt));
                                                            sqlCmd.Parameters.Add(new SqlParameter("@GSSAVGRATE", retailTransaction.PartnerData.GSSAvgRate));
                                                            sqlCmd.Parameters.Add(new SqlParameter("@GSSROYALITYAMOUNT", retailTransaction.PartnerData.GSSRoyaltyAmt));

                                                            sqlCmd.Parameters.Add(new SqlParameter("@STOREID", posTransaction.StoreId));
                                                            sqlCmd.Parameters.Add(new SqlParameter("@TERMINALID", posTransaction.TerminalId));
                                                            sqlCmd.Parameters.Add(new SqlParameter("@STAFFID", posTransaction.OperatorId));
                                                            sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));

                                                            sqlCmd.ExecuteNonQuery();
                                                        }

                                                    }
                                                }


                                                #endregion
                                            }
                                        }
                                        else
                                        {
                                            sqlTransaction.Rollback();
                                            sqlTransaction.Dispose();
                                            //Application.RunOperation(PosisOperations.VoidTransaction, retailTransaction);
                                            MessageBox.Show(string.Format("Failed due to RTS connection issue "));
                                            throw new RuntimeBinderException();
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        ReadOnlyCollection<object> containerArray1;
                                        containerArray1 = this.Application.TransactionServices.InvokeExtension("GSSAdjustedCheck",
                                                                                                              sGSSNo, ((LSRetailPosis.Transaction.PosTransaction)(retailTransaction)).ReceiptId);

                                        bool bResult2 = Convert.ToBoolean(containerArray1[1]);

                                        if (!bResult2)
                                        {
                                            sqlTransaction.Rollback();
                                            sqlTransaction.Dispose();
                                            // Application.RunOperation(PosisOperations.VoidTransaction, retailTransaction);
                                            MessageBox.Show(string.Format("Failed to update GSS adjustment due to RTS transaction issue "));
                                            throw new RuntimeBinderException();
                                        }
                                        else
                                        {
                                            #region IsGSSMaturity==true, INSERT INTO RETAILGSSMATURITYADJUSTMENT
                                            if (retailTransaction.PartnerData.IsGSSMaturity)
                                            {
                                                if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                                                {

                                                    string sGSS = "SET DATEFORMAT DMY;  UPDATE GSSACCOUNTOPENINGPOSTED" +
                                                                             " SET GSSADJUSTED = 1,GSSADJUSTEDDATE = '" + DateTime.Now + "'" +
                                                                             " WHERE GSSACCOUNTNO = '" + Convert.ToString(retailTransaction.PartnerData.GSSMaturityNo) + "';";

                                                    sGSS += "  INSERT INTO RETAILGSSMATURITYADJUSTMENT (TRANSACTIONID,GSSACCOUNTNO,CUSTACCOUNT,GSSTOTALQTY,GSSTOTALAMOUNT," +
                                                                        " GSSAVGRATE,GSSROYALITYAMOUNT,STOREID,TERMINALID,STAFFID,DATAAREAID) VALUES (@TRANSACTIONID,@GSSACCOUNTNO,@CUSTACCOUNT,@GSSTOTALQTY,@GSSTOTALAMOUNT," +
                                                                        " @GSSAVGRATE,@GSSROYALITYAMOUNT,@STOREID,@TERMINALID,@STAFFID,@DATAAREAID);";

                                                    using (SqlCommand sqlCmd = new SqlCommand(sGSS.ToString(), sqlTransaction.Connection, sqlTransaction))
                                                    {
                                                        sqlCmd.Parameters.Add(new SqlParameter("@TRANSACTIONID", posTransaction.TransactionId));
                                                        sqlCmd.Parameters.Add(new SqlParameter("@GSSACCOUNTNO", retailTransaction.PartnerData.GSSMaturityNo));
                                                        sqlCmd.Parameters.Add(new SqlParameter("@CUSTACCOUNT", retailTransaction.Customer.CustomerId));

                                                        sqlCmd.Parameters.Add(new SqlParameter("@GSSTOTALQTY", retailTransaction.PartnerData.GSSTotQty));
                                                        sqlCmd.Parameters.Add(new SqlParameter("@GSSTOTALAMOUNT", retailTransaction.PartnerData.GSSTotAmt));
                                                        sqlCmd.Parameters.Add(new SqlParameter("@GSSAVGRATE", retailTransaction.PartnerData.GSSAvgRate));
                                                        sqlCmd.Parameters.Add(new SqlParameter("@GSSROYALITYAMOUNT", retailTransaction.PartnerData.GSSRoyaltyAmt));

                                                        sqlCmd.Parameters.Add(new SqlParameter("@STOREID", posTransaction.StoreId));
                                                        sqlCmd.Parameters.Add(new SqlParameter("@TERMINALID", posTransaction.TerminalId));
                                                        sqlCmd.Parameters.Add(new SqlParameter("@STAFFID", posTransaction.OperatorId));
                                                        sqlCmd.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));

                                                        sqlCmd.ExecuteNonQuery();
                                                    }

                                                }
                                            }


                                            #endregion
                                        }
                                    }
                                }
                            }

                            #endregion

                            #region Qty save into retailtransactionsalestrans

                            if (retailTransaction.SaleIsReturnSale)
                            {
                                foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                                {
                                    if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                    {
                                        if (saleLineItem.ReturnLineId != 0)
                                        {
                                            if (!string.IsNullOrEmpty(saleLineItem.PartnerData.RETURNTYPE))
                                            {
                                                string sLCQry = " UPDATE RETAILTRANSACTIONSALESTRANS SET CRWRETURNTYPE = '" + Convert.ToString(saleLineItem.PartnerData.RETURNTYPE) + "'" +
                                                                " WHERE TRANSACTIONID='" + retailTransaction.TransactionId + "'" +
                                                                " AND ITEMID = '" + saleLineItem.ItemId + "'" +
                                                                " AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "'" +
                                                                " AND LINENUM = " + saleLineItem.LineId + "";

                                                using (SqlCommand cmdLC = new SqlCommand(sLCQry, sqlTransaction.Connection, sqlTransaction))
                                                {
                                                    cmdLC.ExecuteNonQuery();
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            #endregion
                        }
                    }


                }
                else if (retailTransaction != null && sqlTransaction != null
                      && retailTransaction.IncomeExpenseAmounts != 0
                      && retailTransaction.PartnerData.IsAddToGiftCard == "0"
                      && retailTransaction.PartnerData.IsGiftCardIssue == "0")
                {
                    #region Expense save with Repair Batch Id
                    if (retailTransaction.IncomeExpenseAmounts > 0)
                    {
                        if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                        {
                            string sLCQry = " UPDATE RETAILTRANSACTIONINCOMEEXPENSETRANS" +
                                            " SET REPAIRBATCHID = '" + retailTransaction.PartnerData.ExpRepairBatchId + "'" +
                                            " WHERE TRANSACTIONID='" + retailTransaction.TransactionId + "'" +
                                            " AND TERMINAL = '" + ApplicationSettings.Terminal.TerminalId + "'";


                            using (SqlCommand cmdLC = new SqlCommand(sLCQry, sqlTransaction.Connection, sqlTransaction))
                            {
                                cmdLC.ExecuteNonQuery();
                            }
                        }
                    }
                    #endregion
                }
                else if (retailTransaction != null && sqlTransaction != null
                     && retailTransaction.PartnerData.IsAddToGiftCard == "0"
                     && retailTransaction.PartnerData.IsGiftCardIssue == "1")
                {
                    #region CRWGiftCardIssueNumberSeq
                    if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                    {
                        string sLCQry = " update CRWGiftCardIssueNumberSeq set GiftCardNumberSeedValue=GiftCardNumberSeedValue + 1";

                        using (SqlCommand cmdLC = new SqlCommand(sLCQry, sqlTransaction.Connection, sqlTransaction))
                        {
                            cmdLC.ExecuteNonQuery();
                        }
                    }

                    #endregion
                }
            }
            catch (RuntimeBinderException)
            {
                #region advance voided once any error in local transaction
                string sTransId = posTransaction.TransactionId;
                string sCustId = ((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).Customer.CustomerId;
                string sStoreId = posTransaction.StoreId;
                string sTerminal = posTransaction.TerminalId;

                try
                {
                    if (this.Application.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray;
                        containerArray = this.Application.TransactionServices.InvokeExtension("voidAdvance",
                                                                                              sCustId, sTransId,
                                                                                              sStoreId, sTerminal);

                    }
                }
                catch (Exception)
                {
                }
                #endregion

                MessageBox.Show("Unable to complete transaction due to connection issue.");
            }
            #endregion

            #region   CUSTOMERPAYMENTTRANSACTION
            try
            {

                RetailTransaction retailTransaction = posTransaction as RetailTransaction;

                if (Convert.ToString(posTransaction.GetType().Name).ToUpper().Trim() == "CUSTOMERPAYMENTTRANSACTION")
                {
                    #region custtrans
                    //
                    string custOrder = string.Empty;
                    string repairid = string.Empty;
                    bool IsRepair = false;
                    if (Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.IsRepair))
                    {
                        repairid = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo;
                        IsRepair = true;
                    }
                    else
                    {
                        custOrder = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo;
                    }
                    //
                    #endregion

                    sTestValue = "1";

                    #region // SKU allow
                    string sBookedCustOrdrNo = "";
                    sBookedCustOrdrNo = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo);
                    if (((LSRetailPosis.Transaction.PosTransaction)(posTransaction)).EntryStatus != PosTransaction.TransactionStatus.Voided)
                    {
                        if (sBookedCustOrdrNo != string.Empty)
                        {
                            DataTable dtBookedSKU = new DataTable();
                            dtBookedSKU = GetBookedInfo(sBookedCustOrdrNo, sqlTransaction);
                            if (dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dtBookedSKU.Rows)
                                {
                                    SaveBookedSKU(posTransaction, sBookedCustOrdrNo, Convert.ToString(dr["ITEMID"]), sqlTransaction);
                                }
                            }
                        }

                        try
                        {
                            OrderConfirm(sqlTransaction, sBookedCustOrdrNo);
                        }
                        catch { }

                    }
                    else
                    {
                        OrderCancel(sqlTransaction, sBookedCustOrdrNo);
                    }

                    #endregion

                    sTestValue = "2";

                    #region Gold fixing Action
                    int goldfixing = 0;
                    int GSS = 0;
                    if (Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixing))
                    {
                        goldfixing = 1;
                    }
                    if (Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType) == "GSS")
                    {
                        GSS = 1;
                    }

                    string sGSSNo = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum);
                    if (!string.IsNullOrEmpty(sGSSNo))
                    {
                        GSS = 1;
                    }

                    string commandString = " UPDATE RETAILTRANSACTIONTABLE SET CUSTOMERORDER='" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo + "'" +
                        " , GOLDFIXING=" + goldfixing + ",ISGSS=" + GSS + " WHERE TRANSACTIONID='" + posTransaction.TransactionId + "' AND TERMINAL = '" + ApplicationSettings.Terminal.TerminalId + "';" +
                        // " UPDATE RETAILTEMPTABLE SET TRANSID=NULL,CUSTID=NULL,CUSTORDER=NULL,GOLDFIXING=NULL,ITEMRETURN='False',MINIMUMDEPOSITFORCUSTORDER=NULL WHERE ID IN (1,2); ";
                           " UPDATE RETAILTEMPTABLE SET TRANSID=NULL,CUSTID=NULL,CUSTORDER=NULL,GOLDFIXING=NULL,ITEMRETURN='False',MINIMUMDEPOSITFORCUSTORDER=NULL WHERE ID IN (1,2) AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE

                    if (((LSRetailPosis.Transaction.PosTransaction)(posTransaction)).EntryStatus == PosTransaction.TransactionStatus.Voided)
                    {
                        string Items = string.Empty;
                        Items = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds;
                        if (string.IsNullOrEmpty(Items))
                            Items = "''";
                        // commandString = commandString + " DELETE FROM RETAILCUSTOMERDEPOSITSKUDETAILS WHERE TRANSID='" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).TransactionId + "';" +
                        commandString = commandString + " DELETE FROM RETAILCUSTOMERDEPOSITSKUDETAILS WHERE TRANSID='" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).TransactionId + "' AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "';" + // -- CHANGED ON 24.01.2014
                            // " UPDATE SKUTable_Posted SET isLocked='False' WHERE SkuNumber IN (" + Items + ")";
                            //     " UPDATE SKUTableTrans SET isLocked='False' WHERE SkuNumber IN (" + Items + ")"; //SKU Table New 
                            //      " UPDATE SKUTableTrans SET isLocked='False',isAvailable = 'TRUE' WHERE SkuNumber IN (" + Items + ")"; //SKU Table New  // -- CHANGED ON 24.01.2014
                            " UPDATE SKUTableTrans SET isLocked='False',isAvailable = 'TRUE' WHERE SkuNumber" +
                            " IN (select b.ITEMID from CUSTORDER_HEADER a " +
                            " left join CUSTORDER_DETAILS b on a.ORDERNUM =b.ORDERNUM " +
                            " where  b.IsBookedSKU=1" + //a.IsConfirmed=0 and
                            " and a.ORDERNUM='" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo + "') ";

                    }
                    else
                    {
                        string Items = string.Empty;
                        bool bGoldFixing = false;
                        Items = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds;
                        bGoldFixing = Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixing);
                        string sCustOrdNo = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdNo);

                        if (!string.IsNullOrEmpty(Items))
                            //commandString = commandString + " UPDATE SKUTable_Posted SET isLocked='False',isAvailable='False' WHERE SkuNumber IN (" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds + "); ";
                            commandString = commandString + " UPDATE SKUTableTrans SET isLocked='False',isAvailable='False' WHERE SkuNumber IN (" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds + "); "; //SKU Table New
                        //commandString = commandString + " UPDATE CUSTORDER_HEADER SET IsConfirmed=1 WHERE ORDERNUM='" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo + "' ";

                        // Fixed Metal Rate New
                        if (!string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdNo)))
                        {

                            decimal dPayment = 0m;
                            decimal dFixedRatePercentage = 0m;
                            decimal dCustOrderTotalAmt = 0m;
                            decimal dMinPayAmt = 0m;


                            if (Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdPercentage) > 0)
                            {
                                LSRetailPosis.Transaction.CustomerPaymentTransaction custPayTrans = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;
                                dFixedRatePercentage = Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdPercentage);
                                dCustOrderTotalAmt = Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdTotAmt);

                                dPayment = custPayTrans.Amount;

                                dMinPayAmt = (dFixedRatePercentage / 100) * dCustOrderTotalAmt;

                                if (!bGoldFixing)
                                {
                                    if (dPayment >= dMinPayAmt)
                                    {
                                        commandString = commandString + " UPDATE CUSTORDER_HEADER SET ISFIXEDQTY = 0 ,ISMETALRATEFREEZE=1 WHERE ORDERNUM = '" + sCustOrdNo + "' ";
                                    }
                                }
                                else
                                {
                                    if (dPayment >= Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixingMinAmt))
                                    {
                                        commandString = commandString + " UPDATE CUSTORDER_HEADER SET ISFIXEDQTY = 1 , ISMETALRATEFREEZE=0 WHERE ORDERNUM = '" + sCustOrdNo + "' ";
                                    }
                                }
                            }
                        }

                        //--- end

                    }
                    #endregion

                    sTestValue = "3";
                    #region GSS EMI

                    if (Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone) == true)
                    {
                        // if (retailTransaction.EntryStatus != PosTransaction.TransactionStatus.Voided)
                        if (((LSRetailPosis.Transaction.PosTransaction)(posTransaction)).EntryStatus != PosTransaction.TransactionStatus.Voided)
                        {
                            bool bResult = false;
                            string sTransId = posTransaction.TransactionId;
                            string sCustId = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId;
                            string sStoreId = posTransaction.StoreId;
                            string sTerminal = posTransaction.TerminalId;
                            string sOpId = posTransaction.OperatorId;
                            decimal dAmt = Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount);
                            decimal dGSSTaxAmt = Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxAmt);
                            decimal dGSSTaxPct = Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxPct);
                            string sSms = "";
                            try
                            {
                                if (this.Application.TransactionServices.CheckConnection())
                                {
                                    sGSSNo = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum);
                                    int iNoOfMonth = Convert.ToInt32(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths);

                                    ReadOnlyCollection<object> containerArray;
                                    containerArray = this.Application.TransactionServices.InvokeExtension("GSSEMICreate",
                                                                                                          sTransId,
                                                                                                          sCustId,
                                                                                                          sGSSNo,
                                                                                                          iNoOfMonth,
                                                                                                          dAmt,
                                                                                                          DateTime.Now,
                                                                                                          sStoreId, sTerminal,
                                                                                                          sOpId);//dGSSTaxAmt,dGSSTaxPct --added on 080318

                                    bResult = Convert.ToBoolean(containerArray[1]);
                                    sSms = Convert.ToString(containerArray[2]);
                                    if (bResult == false)
                                    {
                                        throw new RuntimeBinderException();
                                    }

                                    #region 2nd time checking

                                    //if (bResult == false)
                                    //{
                                    //    #region commented
                                    //    /*bool bIsDone = false;
                                    //    try
                                    //    {
                                    //        if (this.Application.TransactionServices.CheckConnection())
                                    //        {
                                    //            ReadOnlyCollection<object> containerArray1;
                                    //            containerArray1 = this.Application.TransactionServices.InvokeExtension("isEmiCreated",
                                    //                                                                                   sCustId, sTransId,
                                    //                                                                                   sStoreId, sTerminal);

                                    //            bIsDone = Convert.ToBoolean(containerArray1[1]);

                                    //            if (bIsDone == false)
                                    //            {
                                    //                MessageBox.Show(sSms+ ","+  "Failed to create GSS EMI");
                                    //                throw new RuntimeBinderException();
                                    //            }
                                    //        }
                                    //    }
                                    //    catch (Exception)
                                    //    {
                                    //        MessageBox.Show(string.Format("Failed to create GSS EMI"));
                                    //        throw new RuntimeBinderException();
                                    //    }*/
                                    //    #endregion

                                    //    MessageBox.Show(sSms + "," + "Failed to create GSS EMI");
                                    //    throw new RuntimeBinderException();
                                    //}
                                    #endregion
                                }
                            }

                            catch (Exception)
                            {
                                ReadOnlyCollection<object> containerArray;
                                containerArray = this.Application.TransactionServices.InvokeExtension("voidEMICreate",
                                                                                                      sCustId, sTransId,
                                                                                                      sStoreId, sTerminal);

                                // Application.RunOperation(PosisOperations.VoidTransaction, retailTransaction);
                                if (!string.IsNullOrEmpty(sSms))
                                {
                                    MessageBox.Show(sSms);
                                }
                                else
                                {
                                    MessageBox.Show(string.Format("Failed due to RTS issue."));
                                }
                                throw new RuntimeBinderException();
                            }

                        }
                    }
                    #endregion

                    if (((LSRetailPosis.Transaction.PosTransaction)(posTransaction)).EntryStatus != PosTransaction.TransactionStatus.Voided)
                    {
                        sTestValue = "4";
                        #region Get RATE
                        commandString += " DECLARE @INVENTLOCATION VARCHAR(20) ";
                        commandString += " DECLARE @CONFIGID VARCHAR(20) ";
                        commandString += " DECLARE @RATE numeric(28, 3) ";
                        commandString += " SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ";
                        commandString += " RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ";
                        commandString += " WHERE RETAILSTORETABLE.STORENUMBER='" + posTransaction.StoreId + "'";

                        if (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType == "GSS")
                            commandString += "SELECT top 1 @CONFIGID = [GSSDefaultConfigIdGold] from RETAILPARAMETERS WHERE DATAAREAID='" + application.Settings.Database.DataAreaID + "'";
                        else
                            commandString += " SELECT @CONFIGID = DEFAULTCONFIGIDGOLD FROM [RETAILPARAMETERS] WHERE DATAAREAID='" + application.Settings.Database.DataAreaID + "'";//INVENTPARAMETERS

                        commandString += "  SELECT TOP 1 @RATE=RATES FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ";
                        commandString += " AND METALTYPE=" + (int)Enums.EnumClass.MetalType.Gold + " AND ACTIVE=1 and RETAIL=1 ";
                        commandString += " AND CONFIGIDSTANDARD=@CONFIGID  AND RATETYPE=" + (int)Enums.EnumClass.RateType.Sale + " ";// GSS ->Sales Req by S.Sharma on 10/06/2016
                        commandString += " ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC;";
                        #endregion

                        sTestValue = "5";

                        #region INSERT INTO [DEPOSIT_GOLD_DETAILS]
                        // " ORDER BY [TRANSDATE],[TIME] DESC; ";
                        if (Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixing))
                        {

                            commandString += " INSERT INTO [DEPOSIT_GOLD_DETAILS] " +
                                                  " ([ADVANCEID],[ORDERID],[CUSTACCOUNT],[PURCQTY],[AMOUNT],[STOREID] " +
                                                  " ,[TERMINALID],[STAFFID],[DATAAREAID],[CREATEDON]) " +
                                                  " VALUES " +
                                                  " ('" + posTransaction.TransactionId + "', " +
                                                  " '" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo + "', " +
                                                  " '" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId + "', " +
                                                  " ISNULL(" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount + "/@RATE,0), " +
                                                  " " + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount + " , " +
                                                  "'" + posTransaction.StoreId + "', " +
                                                  " '" + posTransaction.TerminalId + "', " +
                                                  "'" + posTransaction.OperatorId + "', " +
                                                  "'" + application.Settings.Database.DataAreaID + "', " +
                                                  " GETDATE()); ";
                        }
                        //else
                        //{
                        //    commandString += " SET @RATE= NULL ; "; // commented for malabara on 16/12/16
                        //}
                        #endregion

                        sTestValue = "6";

                        #region - RETAIL GSS ACCOUNT DEPOSIT - COMMENTED
                        //   if (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType == "GSS")
                        //  {

                        //commandString += "  " +
                        //                       " INSERT INTO [RETAILGSSACCOUNTDEPOSIT] " +
                        //                       " ([TRANSACTIONID],[CUSTACCOUNT],[GSSNUMBER],[NOOFMONTHS],[AMOUNT],[GOLDFIXING],[STOREID] " +
                        //                       " ,[TERMINALID],[STAFFID],[DATAAREAID],[CREATEDON]) " +
                        //                       " VALUES " +
                        //                       " ('" + posTransaction.TransactionId + "', " +
                        //                       " '" + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId + "', " +
                        //                       " '" + Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum) + "', " +
                        //                       " '" + Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths) + "', " +
                        //                       " " + ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount + " , " +
                        //                       " " + goldfixing + ", " +
                        //                       "'" + posTransaction.StoreId + "', " +
                        //                       " '" + posTransaction.TerminalId + "', " +
                        //                       "'" + posTransaction.OperatorId + "', " +
                        //                       "'" + application.Settings.Database.DataAreaID + "', " +
                        //                       " GETDATE()); ";

                        //  }
                        //((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRate = dFixRate;
                        //((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedConfigId = sFixConfigId;
                        #endregion
                        decimal dOrdQty = 0m;

                        //dOrdQty = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustOrdQty;
                        decimal dAdvTaxPct = Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxPct);
                        decimal dAdvTaxAmt = Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxAmt);

                        commandString = SaveAdvanceDataIntoLocal(posTransaction, goldfixing, commandString, dAdvTaxAmt);//1

                        #region RTS globally Insert Advance data//
                        string sTransId = posTransaction.TransactionId;
                        string sCustId = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId;
                        string sOrderN = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo;
                        decimal dAmt = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount;


                        //dAdvTaxAmt = decimal.Round(Convert.ToDecimal(dAmt) * dAdvTaxPct / 100, 2, MidpointRounding.AwayFromZero);


                        int iGoldFix = goldfixing;
                        int iDepositeType = Convert.ToInt16(Convert.ToBoolean(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone) ? (int)Enums.EnumClass.DepositType.GSS : (int)Enums.EnumClass.DepositType.Normal);
                        string sStoreId = posTransaction.StoreId;
                        string sTerminal = posTransaction.TerminalId;
                        string sOpId = posTransaction.OperatorId;
                        string sDataAreaId = application.Settings.Database.DataAreaID;
                        int iAdvAgainst = Convert.ToInt16(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ADVAGAINST);
                        string sVouAgainst = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.VOUCHERAGAINST);
                        sGSSNo = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum);
                        int iNoOfMonth = 0;
                        if (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType == "GSS")
                            iNoOfMonth = Convert.ToInt16(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths);

                        string sOpType = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType);
                        decimal sRate = Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRate);
                        string sFixedConfigId = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedConfigId);
                        string sSalesMan = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SalesPersonId);

                        decimal dManualBookedQty = Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ManualBookedQty);
                        decimal dGoldBookingRatePercentage = Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldBookingRatePercentage);
                        string sProductType = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ProductType);

                        //======================================Soutik==============================================
                        decimal dCustAdvanceCommitedQty = Convert.ToDecimal(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustAdvCommitedQty);
                        int iNoOfCommitedDays = Convert.ToInt32(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustCommitedForDays); 

                        bool bResult = false;


                        if (!string.IsNullOrEmpty(sVouAgainst))
                        {
                            commandString += " UPDATE RETAILTRANSACTIONTABLE SET ADVANCEDONE=1 , ADVANCEDONEWITH ='" + sTransId + "'" +
                                       " where RECEIPTID='" + sVouAgainst + "'" +
                                       " and TERMINAL='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";

                        }
                        if (!string.IsNullOrEmpty(sSalesMan))
                        {
                            commandString += " UPDATE RETAILTRANSACTIONTABLE SET ADVANCESALESMANID ='" + sSalesMan + "'" +
                                       " where TRANSACTIONID='" + sTransId + "'" +
                                       " and TERMINAL='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";

                        }

                        commandString += " UPDATE RETAILTRANSACTIONTABLE SET GOLDFIXING =" + goldfixing + "" +
                               " where TRANSACTIONID='" + sTransId + "'" +
                               " and TERMINAL='" + posTransaction.TerminalId + "' and STORE='" + posTransaction.StoreId + "'";

                        try
                        {
                            if (this.Application.TransactionServices.CheckConnection())
                            {
                                sTestValue = "7";

                                ReadOnlyCollection<object> containerArray;
                                containerArray = this.Application.TransactionServices.InvokeExtension("insertAdvanceData",//3
                                                                                                      sTransId, sCustId, sOrderN,
                                                                                                      dAmt, iGoldFix, iDepositeType,
                                                                                                      sStoreId, sTerminal, sOpId,
                                                                                                      sDataAreaId, iAdvAgainst, sVouAgainst,
                                                                                                      sGSSNo, iNoOfMonth, sRate, sFixedConfigId,
                                                                                                      sOpType, 0, sSalesMan, dManualBookedQty,
                                                                                                      dGoldBookingRatePercentage, sProductType,
                                                                                                      dAdvTaxPct, dAdvTaxAmt, 0, dCashPayAmt,
                                                                                                      dCustAdvanceCommitedQty, iNoOfCommitedDays);//, sSalesMan

                                bResult = Convert.ToBoolean(containerArray[1]);
                            }
                        }
                        catch (Exception)
                        {
                            MessageBox.Show(string.Format("Failed due to RTS issue."));
                            throw new RuntimeBinderException();
                        }
                        if (bResult == false)
                        {
                            #region 2nd time checking
                            bool bIsDone = false;
                            try
                            {
                                if (this.Application.TransactionServices.CheckConnection())
                                {
                                    sTestValue = "9";

                                    ReadOnlyCollection<object> containerArray1;
                                    containerArray1 = this.Application.TransactionServices.InvokeExtension("isAdvanceCreated",//added on 19/07/2017
                                                                                                           sCustId, sTransId,
                                                                                                           sStoreId, sTerminal);

                                    bIsDone = Convert.ToBoolean(containerArray1[1]);

                                    if (bIsDone == false)
                                    {
                                        ReadOnlyCollection<object> containerArray;
                                        containerArray = this.Application.TransactionServices.InvokeExtension("voidAdvance",
                                                                                                              sCustId, sTransId,
                                                                                                              sStoreId, sTerminal);

                                        MessageBox.Show(string.Format("Failed to save advance data."));
                                        throw new RuntimeBinderException();
                                    }
                                }
                            }
                            catch (Exception)
                            {
                                sTestValue = "10";

                                ReadOnlyCollection<object> containerArray;
                                containerArray = this.Application.TransactionServices.InvokeExtension("voidAdvance",
                                                                                                      sCustId, sTransId,
                                                                                                      sStoreId, sTerminal);

                                MessageBox.Show(string.Format("Failed due to RTS issue."));
                                throw new RuntimeBinderException();
                            }
                            #endregion
                        }
                        if (bResult == true)
                            sTestValue = "11";
                        else
                            sTestValue = "12";


                        #endregion
                    }
                    sTestErrorSqlString = commandString + " ; Test Value " + sTestValue;
                    using (SqlCommand command = new SqlCommand(commandString, sqlTransaction.Connection, sqlTransaction))
                    {
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception)
            {
                #region advance voided once any error in local transaction
                string sTransId = posTransaction.TransactionId;
                string sCustId = ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Customer.CustomerId;
                string sStoreId = posTransaction.StoreId;
                string sTerminal = posTransaction.TerminalId;


                try
                {
                    if (this.Application.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray;
                        containerArray = this.Application.TransactionServices.InvokeExtension("voidAdvance",
                                                                                              sCustId, sTransId,
                                                                                              sStoreId, sTerminal);
                    }
                }
                catch (Exception)
                {
                    if (this.Application.TransactionServices.CheckConnection())
                    {

                        ReadOnlyCollection<object> containerArray;
                        containerArray = this.Application.TransactionServices.InvokeExtension("voidAdvance",
                                                                                              sCustId, sTransId,
                                                                                              sStoreId, sTerminal);
                    }
                }

                try
                {
                    if (this.Application.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray;
                        containerArray = this.Application.TransactionServices.InvokeExtension("voidEMICreate",
                                                                                              sCustId, sTransId,
                                                                                              sStoreId, sTerminal);
                    }
                }
                catch (Exception)
                {

                    if (this.Application.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray;
                        containerArray = this.Application.TransactionServices.InvokeExtension("voidEMICreate",
                                                                                              sCustId, sTransId,
                                                                                              sStoreId, sTerminal);
                    }
                }

                #endregion

                #region  After Exception Action
                // string s = " UPDATE RETAILTEMPTABLE SET TRANSID=NULL,CUSTID=NULL,CUSTORDER=NULL,GOLDFIXING=NULL,ITEMRETURN='False',MINIMUMDEPOSITFORCUSTORDER=NULL WHERE ID IN (1,2); ";
                string s = " UPDATE RETAILTEMPTABLE SET TRANSID=NULL,CUSTID=NULL,CUSTORDER=NULL,GOLDFIXING=NULL,ITEMRETURN='False',MINIMUMDEPOSITFORCUSTORDER=NULL WHERE ID IN (1,2) AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "'" + // RETAILTEMPTABLE
                    //Start: added on 16/06/2014 
                    // when gss emi is recived by real tiem service, after selecting the amount or scheme, if
                    //Connection is out then that payment already saved in these three table(base)
                    // so in our customization these data should not be there.
                          " DELETE FROM RETAILTRANSACTIONTABLE WHERE TRANSACTIONID = '" + posTransaction.TransactionId + "'" +
                          " AND STORE='" + posTransaction.StoreId + "'" +
                          " AND TERMINAL='" + ApplicationSettings.Terminal.TerminalId + "'" +
                          " AND DATAAREAID ='" + application.Settings.Database.DataAreaID + "'" +

                          " DELETE FROM RETAILTRANSACTIONTABLEEX5 WHERE TRANSACTIONID = '" + posTransaction.TransactionId + "'" +
                          " AND STORE='" + posTransaction.StoreId + "'" +
                          " AND TERMINAL='" + ApplicationSettings.Terminal.TerminalId + "'" +
                          " AND DATAAREAID ='" + application.Settings.Database.DataAreaID + "'" +

                          " DELETE FROM RETAILTRANSACTIONPAYMENTTRANS WHERE TRANSACTIONID = '" + posTransaction.TransactionId + "'" +
                          " AND STORE='" + posTransaction.StoreId + "'" +
                          " AND TERMINAL='" + ApplicationSettings.Terminal.TerminalId + "'" +
                          " AND DATAAREAID ='" + application.Settings.Database.DataAreaID + "'";
                //End : added on 16/06/2014 
                using (SqlCommand command = new SqlCommand(s, sqlTransaction.Connection, sqlTransaction))
                {
                    command.ExecuteNonQuery();
                }
                string filePath = @"C:\Nimbus_Error-" + posTransaction.TransactionId + "_" + ApplicationSettings.Terminal.TerminalId + ".txt";

                using (StreamWriter writer = new StreamWriter(filePath, true))
                {
                    writer.WriteLine("Message :" + sTestErrorSqlString +
                       "" + Environment.NewLine + "Date :" + DateTime.Now.ToString());
                    writer.WriteLine(Environment.NewLine + "-----------------------------------------------------------------------------" + Environment.NewLine);
                }

                MessageBox.Show("Unable to save due to connection issue.");//" + sTestValue + "

                #endregion

                DropTempTable();
            }
            #endregion

            //End:Nim
        }


        #region Print Customer order Voucher
        public void PrintVoucher(string sOrderNo, SqlTransaction sqlTransaction, Int16 iCopy, RetailTransaction retailTransaction = null)
        {
            string sCompName = string.Empty;
            string sStoreName = string.Empty;
            string sStoreAddress = string.Empty;
            string sStorePhNo = string.Empty;
            string sGSTNo = string.Empty;
            string sStoreTaxState = "";
            string sInvoiceTime = "";

            if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
            if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);
            if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);

            if (Convert.ToString(ApplicationSettings.Terminal.IndiaGSTRegistrationNumber) != string.Empty)
                sGSTNo = Convert.ToString(ApplicationSettings.Terminal.IndiaGSTRegistrationNumber);

            // string sCompanyName = GetCompanyName();
            //-------

            if (Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState) != string.Empty)
                sStoreTaxState = Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState);

            if (retailTransaction != null)
                sInvoiceTime = (retailTransaction).BeginDateTime.ToString("HH:mm");

            sCompName = GetCompanyName(sqlTransaction);
            //datasources
            List<ReportDataSource> rds = new List<ReportDataSource>();
            rds.Add(new ReportDataSource("HEADERINFO", (DataTable)GetHeaderInfo(sOrderNo, sqlTransaction)));
            rds.Add(new ReportDataSource("DETAILINFO", (DataTable)GetDetailInfo(sOrderNo, sqlTransaction)));
            rds.Add(new ReportDataSource("CUSTORDINGR", (DataTable)GetCustOrderIngr(sOrderNo, sqlTransaction)));

            BlankOperations.BlankOperations objBlankOpe = new BlankOperations.BlankOperations();

            string sAmtinwds = objBlankOpe.Amtinwds(Math.Abs(Convert.ToDouble(getTotAmtOfOrder(sOrderNo, sqlTransaction)))); // added on 28/04/2014 RHossain               
            //parameters
            List<ReportParameter> rps = new List<ReportParameter>();
            rps.Add(new ReportParameter("Title", "Customer Order Voucher", true));
            rps.Add(new ReportParameter("StorePhone", string.IsNullOrEmpty(ApplicationSettings.Terminal.StorePhone) ? " " : ApplicationSettings.Terminal.StorePhone, true));
            rps.Add(new ReportParameter("StoreAddress", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreAddress) ? " " : ApplicationSettings.Terminal.StoreAddress, true));
            rps.Add(new ReportParameter("StoreName", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreName) ? " " : ApplicationSettings.Terminal.StoreName, true));
            rps.Add(new ReportParameter("GSTNo", sGSTNo, true));
            rps.Add(new ReportParameter("CompName", sCompName, true));
            rps.Add(new ReportParameter("Amtinwds", sAmtinwds, true));

            string reportName = @"rptCustOrdVoucher";
            string reportPath = @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";
            BlankOperations.WinFormsTouch.RdlcViewer rptView = new BlankOperations.WinFormsTouch.RdlcViewer("Customer Order Voucher", reportPath, rds, rps, null);
            rptView.ShowDialog();

            // Export(reportName.);
        }

        private string GetCompanyName(SqlTransaction sqlTransaction)
        {
            string sCName = string.Empty;
            string sQry = "SELECT ISNULL(A.NAME,'') FROM DIRPARTYTABLE A INNER JOIN COMPANYINFO B" +
                " ON A.RECID = B.RECID WHERE B.DATAAREA = '" + ApplicationSettings.Database.DATAAREAID + "'";

            using (SqlCommand cmd = new SqlCommand(sQry, sqlTransaction.Connection, sqlTransaction))
            {
                cmd.CommandTimeout = 0;
                sCName = Convert.ToString(cmd.ExecuteScalar());
            }

            return sCName;

        }

        private string getTotAmtOfOrder(string sOrderNo, SqlTransaction sqlTransaction)
        {
            string sCName = string.Empty;
            string sQry = "SELECT SUM(AMOUNT + MAKINGAMOUNT) FROM CUSTORDER_DETAILS WHERE ORDERNUM='" + sOrderNo + "'";

            using (SqlCommand cmd = new SqlCommand(sQry, sqlTransaction.Connection, sqlTransaction))
            {
                cmd.CommandTimeout = 0;
                sCName = Convert.ToString(cmd.ExecuteScalar());
            }

            return sCName;
        }


        //dd
        public DataTable GetHeaderInfo(string sOrderNo, SqlTransaction sqlTransaction)
        {
            try
            {
                string sCustOrderReceiptDate = Convert.ToDateTime(DateTime.Now).ToShortDateString();

                string commandText = "select ORDERNUM as ORDERNO,CONVERT(VARCHAR(15),ORDERDATE,103) ORDERDATE,CONVERT(VARCHAR(15),DELIVERYDATE,103) DELIVERYDATE,CUSTACCOUNT as CUSTID,CUSTNAME," +
                                      " CUSTADDRESS as CUSTADD,CUSTPHONE,CAST(ISNULL(TOTALAMOUNT,0)AS DECIMAL(18,2)) AS TOTALAMOUNT FROM CUSTORDER_HEADER" +
                                      " WHERE ORDERNUM='" + sOrderNo + "'";

                DataTable CustBalDt = new DataTable();
                using (SqlCommand command = new SqlCommand(commandText, sqlTransaction.Connection, sqlTransaction))
                {
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(CustBalDt);
                }

                if (CustBalDt.Rows.Count > 0)
                    sCustOrderReceiptDate = Convert.ToDateTime(CustBalDt.Rows[0]["ORDERDATE"]).ToString("dd-MM-yyyy");

                return CustBalDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        //
        public DataTable GetDetailInfo(string sOrderNo, SqlTransaction sqlTransaction)
        {
            try
            {
                string commandText = "select A.ITEMID as SKUID, A.ITEMID + '-' + F.NAME as ITEMID,PCS,QTY,CRate as RATE,AMOUNT,MAKINGRATE,MAKINGAMOUNT," +
                                      " LineTotalAmt as TOTALAMOUNT,REMARKSDTL as REMARKS,IsBookedSKU as IsBooked,A.CONFIGID,A.CODE,A.SIZEID,A.WastageAmount " +
                                      " AS WastageAmt FROM CUSTORDER_DETAILS A" +//, CONVERT(VARCHAR(11)DELIVERYDATE,103) as DELIVERYDATE
                                      " INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID " +
                                      " LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID " +
                                      " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT" +
                                      " WHERE ORDERNUM='" + sOrderNo + "'"; // SKUID for get the itemid for image selection of parent item id of that item

                DataTable CustBalDt = new DataTable();
                using (SqlCommand command = new SqlCommand(commandText, sqlTransaction.Connection, sqlTransaction))
                {
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(CustBalDt);
                }

                return CustBalDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private DataTable GetCustOrderIngr(string sOrderNo, SqlTransaction sqlTransaction)
        {
            try
            {
                string commandText = " SELECT A.ITEMID + '-' + F.NAME as ITEMID ,PCS,QTY ,CRATE,AMOUNT,A.CONFIGID,A.CODE,A.SIZEID    FROM CUSTORDER_SUBDETAILS A" +
                                      " INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID " +
                                      " LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID " +
                                      " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT" +
                                      " where ORDERNUM='" + sOrderNo + "' ORDER BY ORDERNUM, LINENUM";

                DataTable CustBalDt = new DataTable();

                using (SqlCommand command = new SqlCommand(commandText, sqlTransaction.Connection, sqlTransaction))
                {
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(CustBalDt);
                }

                return CustBalDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

        #region IsCustOrderConfirmed
        public bool IsCustOrderConfirmed(string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT IsConfirmed  FROM [CUSTORDER_HEADER] WHERE ORDERNUM='" + sOrderNo + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }
        #endregion


        #endregion

        public void PreEndTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.PreEndTransaction", "When concluding the transaction, prior to printing and saving...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostEndTransaction(IPosTransaction posTransaction)
        {
            //Start:Nim
            #region Nimbus chages
            try
            {
                RetailTransaction retailTransaction = posTransaction as RetailTransaction;
                string sStoreId = ApplicationSettings.Terminal.StoreId;
                string sTerminalId = ApplicationSettings.Terminal.TerminalId;

                if (Convert.ToString(posTransaction.GetType().Name).ToUpper().Trim() == "CUSTOMERPAYMENTTRANSACTION")
                {
                    LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;
                    if (custTrans != null)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(custTrans.PartnerData.EFTCardNo)))
                        {
                            DataTable dtCardInfo = new DataTable();
                            SqlConnection conn = new SqlConnection();

                            if (application != null)
                                conn = application.Settings.Database.Connection;
                            else
                                conn = ApplicationSettings.Database.LocalConnection;

                            dtCardInfo = GetCardIfo(custTrans.TransactionId);

                            if (dtCardInfo != null && dtCardInfo.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dtCardInfo.Rows)
                                {
                                    string sEFTAppovalCode = Convert.ToString(dr["APPROVALCODE"]);
                                    string sExpMonth = Convert.ToString(dr["CARDEXPIRYMONTH"]);
                                    string sExpYear = Convert.ToString(dr["CARDEXPIRYYEAR"]);

                                    string sQuery = " UPDATE RETAILTRANSACTIONPAYMENTTRANS SET EFTApprovalCode = '" + sEFTAppovalCode + "'" +
                                                    " ,CARDEXPIRYMONTH = '" + sExpMonth + "',CARDEXPIRYYEAR ='" + sExpYear + "'" +
                                                    " WHERE TRANSACTIONID = '" + custTrans.TransactionId + "'" +
                                                    " AND (ISNULL(CARDORACCOUNT,'') = '" + Convert.ToString(dr["CARDNO"]) + "')" +
                                                    " AND STORE = '" + sStoreId + "' AND TERMINAL = '" + sTerminalId + "' ";

                                    // using (SqlCommand command = new SqlCommand(sQuery, sqlTransaction.Connection, sqlTransaction))
                                    using (SqlCommand command = new SqlCommand(sQuery, conn))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                }

                                string sCardTblName = "EXTNDCARDINFO" + ApplicationSettings.Terminal.TerminalId;

                                string sQry = "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sCardTblName + "')" +
                                              " BEGIN  DROP TABLE " + sCardTblName + " END ";
                                using (SqlCommand command = new SqlCommand(sQry, conn))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                        }

                        //REPAIR order cash advance update
                        try
                        {
                            if (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.IsRepair)
                            {
                                string repairid = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo);

                                SqlConnection conn = new SqlConnection();

                                if (application != null)
                                    conn = application.Settings.Database.Connection;
                                else
                                    conn = ApplicationSettings.Database.LocalConnection;

                                if (conn.State == ConnectionState.Closed)
                                    conn.Open();

                                string sQuery = string.Empty;
                                sQuery += " UPDATE  [RetailRepairDetail] SET [CASHADVANCE]='" + custTrans.Amount +
                                          "' WHERE [RepairId]='" + repairid + "' AND [RetailStoreId]='" + custTrans.StoreId +
                                          "' AND [RetailTerminalId]='" + custTrans.TerminalId +
                                          "' AND [DATAAREAID]='" + ApplicationSettings.Database.DATAAREAID + "';";
                                using (SqlCommand command = new SqlCommand(sQuery, conn))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                        }
                        finally
                        {

                        }
                    }

                }
                if (retailTransaction != null)
                {
                    SqlConnection conn = new SqlConnection();
                    if (!string.IsNullOrEmpty(Convert.ToString(retailTransaction.PartnerData.EFTCardNo)))
                    {
                        DataTable dtCardInfo = new DataTable();

                        if (application != null)
                            conn = application.Settings.Database.Connection;
                        else
                            conn = ApplicationSettings.Database.LocalConnection;

                        dtCardInfo = GetCardIfo(retailTransaction.TransactionId);

                        if (dtCardInfo != null && dtCardInfo.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtCardInfo.Rows)
                            {
                                string sEFTAppovalCode = Convert.ToString(dr["APPROVALCODE"]);
                                string sExpMonth = Convert.ToString(dr["CARDEXPIRYMONTH"]);
                                string sExpYear = Convert.ToString(dr["CARDEXPIRYYEAR"]);

                                string sQuery = " UPDATE RETAILTRANSACTIONPAYMENTTRANS SET EFTApprovalCode = '" + sEFTAppovalCode + "'" +
                                                " ,CARDEXPIRYMONTH = '" + sExpMonth + "',CARDEXPIRYYEAR ='" + sExpYear + "'" +
                                                " WHERE TRANSACTIONID = '" + retailTransaction.TransactionId + "'" +
                                                " AND (ISNULL(CARDORACCOUNT,'') = '" + Convert.ToString(dr["CARDNO"]) + "')" +
                                                " AND STORE = '" + sStoreId + "' AND TERMINAL = '" + sTerminalId + "'";
                                using (SqlCommand command = new SqlCommand(sQuery, conn))
                                {
                                    command.ExecuteNonQuery();
                                }
                            }
                            string sCardTblName = "EXTNDCARDINFO" + ApplicationSettings.Terminal.TerminalId;

                            string sQry = "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sCardTblName + "')" +
                                          " BEGIN  DROP TABLE " + sCardTblName + " END ";
                            using (SqlCommand command = new SqlCommand(sQry, conn))
                            {
                                command.ExecuteNonQuery();
                            }
                        }
                    }

                    #region Update Suspended Transaction Info
                    string sCustomerId = "";
                    string sCustomerName = "";
                    string sCustomerMobile = "";
                    string sRECEIPTID = "";

                    if (retailTransaction != null
                        && retailTransaction.EntryStatus == PosTransaction.TransactionStatus.OnHold)
                    {
                        sCustomerId = retailTransaction.Customer.CustomerId;
                        sCustomerName = retailTransaction.Customer.Name;
                        sRECEIPTID = retailTransaction.TransactionId;

                        sCustomerMobile = GetCustomerMobilePrimary(sCustomerId);

                        UpdateRETAILSUSPENDEDTRANSACTIONS(ApplicationSettings.Database.TerminalID, sCustomerId, sCustomerName, sCustomerMobile, sRECEIPTID);
                    }
                    #endregion

                    #region Cash pay limit temp table drop
                    DropTempTable();

                    #endregion

                    #region added on  22/08/16 for gift card id save for separtae posting for Malabar Dubai
                    if (retailTransaction.TenderLines.Count > 0) // added on  22/08/16 for gift card id save for separtae posting for Malabar Dubai
                    {
                        foreach (ITenderLineItem tenderItem in retailTransaction.TenderLines)
                        {
                            GiftCertificateTenderLineItem giftCardTenderLineItem = tenderItem as GiftCertificateTenderLineItem;
                            if (giftCardTenderLineItem != null)
                            {
                                if (application != null)
                                    conn = application.Settings.Database.Connection;
                                else
                                    conn = ApplicationSettings.Database.LocalConnection;
                                if (tenderItem.Amount > 0)
                                {
                                    string sCardTypeId = GetCARDTYPEID(giftCardTenderLineItem.SerialNumber);// added Nimbus on 22/08/2016

                                    string sQuery = " UPDATE RETAILTRANSACTIONPAYMENTTRANS SET CARDTYPEID = '" + sCardTypeId + "'" +
                                                     " WHERE TRANSACTIONID = '" + retailTransaction.TransactionId + "'" +
                                                     " AND (ISNULL(GIFTCARDID,'') = '" + giftCardTenderLineItem.SerialNumber + "')" + //" + Convert.ToString(dr["CARDNO"]) + "
                                                     " AND STORE = '" + sStoreId + "' AND TERMINAL = '" + sTerminalId + "'";

                                    if (conn.State == ConnectionState.Closed)
                                        conn.Open();
                                    using (SqlCommand command = new SqlCommand(sQuery, conn))
                                    {
                                        command.ExecuteNonQuery();
                                    }
                                }
                            }
                        }
                    }
                    #endregion
                }

                #region RCARD

                RetailTransaction retailTrans = posTransaction as RetailTransaction;
                if (retailTrans != null && retailTrans.EntryStatus != LSRetailPosis.Transaction.PosTransaction.TransactionStatus.OnHold) //&& LSRetailPosis.Transaction.PosTransaction.TransactionStatus.OnHold
                {
                    //if (retailTrans.SaleIsReturnSale == false)
                    //{
                    if (retailTrans.EntryStatus != PosTransaction.TransactionStatus.Voided)
                    {
                        SqlConnection conn = new SqlConnection();
                        if (application != null)
                            conn = application.Settings.Database.Connection;
                        else
                            conn = ApplicationSettings.Database.LocalConnection;

                        if (!string.IsNullOrEmpty(retailTrans.PartnerData.LOYALTYTYPECODE))//LOYALTYTYPECODE,LoyaltyProvider,LOYALTYCARDNO
                        {
                            string sLCQry = " UPDATE RETAILTRANSACTIONTABLE SET LOYALTYTYPECODE = '" + retailTrans.PartnerData.LOYALTYTYPECODE + "'," +
                                            " LOYALTYPROVIDER = '" + retailTrans.PartnerData.LoyaltyProvider + "'," +
                                            " LOYALTYCARDNO = '" + retailTrans.PartnerData.LOYALTYCARDNO + "'" +
                                            " WHERE TRANSACTIONID='" + retailTrans.TransactionId + "'" +
                                            " AND TERMINAL = '" + ApplicationSettings.Terminal.TerminalId + "'";

                            using (SqlCommand cmdLC = new SqlCommand(sLCQry, conn))
                            {
                                cmdLC.ExecuteNonQuery();
                            }
                        }


                    }
                    //}
                }

                #endregion

                #region Tender Declare Trans .... created on 17022017
                if (Convert.ToString(posTransaction.GetType().Name).ToUpper().Trim() == "TENDERDECLARATIONTRANSACTION")
                {
                    LSRetailPosis.Transaction.TenderDeclarationTransaction tenderDecTrans = posTransaction as LSRetailPosis.Transaction.TenderDeclarationTransaction;
                    if (tenderDecTrans != null)
                    {
                        if (!string.IsNullOrEmpty(Convert.ToString(tenderDecTrans.PartnerData.TDDATA)))
                        {
                            DataTable dtTENDER = new DataTable();
                            SqlConnection conn = new SqlConnection();

                            if (application != null)
                                conn = application.Settings.Database.Connection;
                            else
                                conn = ApplicationSettings.Database.LocalConnection;

                            if (tenderDecTrans != null)
                            {
                                DataSet dsTD = new DataSet();
                                StringReader reader = new StringReader(Convert.ToString(tenderDecTrans.PartnerData.TDDATA));
                                dsTD.ReadXml(reader);

                                dtTENDER = dsTD.Tables[0];
                                int i = 0;
                                if (dtTENDER != null && dtTENDER.Rows.Count > 0)
                                {
                                    foreach (DataRow dr in dtTENDER.Rows)
                                    {
                                        i++;
                                        decimal dDENOMINATION = Convert.ToDecimal(dr["DENOMINATION"]);
                                        decimal dDENOMINATIONTEXT = Convert.ToDecimal(dr["DENOMINATIONTEXT"]);
                                        decimal dQUANTITY = Convert.ToDecimal(dr["QUANTITY"]);
                                        decimal dTOTAL = Convert.ToDecimal(dr["TOTAL"]);
                                        int dDenoType;
                                        if (Convert.ToString(dr["DENOMINATIONTYPE"]) == "Coin")
                                            dDenoType = 0;
                                        else if (Convert.ToString(dr["DENOMINATIONTYPE"]) == "Note")
                                            dDenoType = 1;
                                        else
                                            dDenoType = 2;

                                        string commandText = " INSERT INTO [CRWTENDERDENOMINATION] " +
                                                           " ([DENOMINATION],[DENOMINATIONTEXT],[QUANTITY],[TOTAL],[TRANSACTIONID],[STOREID] " +
                                                           " ,[TRANSDATE],[TERMINALID],[STAFFID],[DATAAREAID]) " +
                                                           "  VALUES " +
                                                           " (@DENOMINATION,@DENOMINATIONTEXT,@QUANTITY,@TOTAL,@TRANSACTIONID" +
                                                           " ,@STOREID,@TRANSDATE,@TERMINALID,@STAFFID,@DATAAREAID)";

                                        commandText += " INSERT INTO [RETAILCASHDECLARATION] " +
                                                         " (CURRENCYCODE,AMOUNTCUR,type,QTY,LINENUM,TOTALLINE,TOTALAMOUNT,DATAAREAID,RECID) " +
                                                         "  VALUES " +
                                                         " ('" + ApplicationSettings.Terminal.CompanyCurrency + "'," + dDENOMINATION + ", " + dDenoType + "," + dQUANTITY + "," + i + "" +
                                                         " ," + dtTENDER.Rows.Count + "," + dTOTAL + ",@DATAAREAID, " + Convert.ToString(tenderDecTrans.TransactionId) + "" + i + ")";

                                        if (conn.State == ConnectionState.Closed)
                                            conn.Open();
                                        using (SqlCommand command = new SqlCommand(commandText, conn))
                                        {

                                            command.Parameters.Add("@DENOMINATION", SqlDbType.Decimal).Value = dDENOMINATION;
                                            command.Parameters.Add("@DENOMINATIONTEXT", SqlDbType.Decimal).Value = dDENOMINATIONTEXT;
                                            command.Parameters.Add("@QUANTITY", SqlDbType.Decimal).Value = dQUANTITY;
                                            command.Parameters.Add("@TOTAL", SqlDbType.Decimal).Value = dTOTAL;

                                            command.Parameters.Add("@TRANSDATE", SqlDbType.Date).Value = Convert.ToDateTime(DateTime.Now).Date;
                                            command.Parameters.Add("@TRANSACTIONID", SqlDbType.NVarChar, 20).Value = Convert.ToString(tenderDecTrans.TransactionId);
                                            command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.StoreId;
                                            command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.TerminalId;
                                            command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Database.DATAAREAID;
                                            command.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 20).Value = Convert.ToString(tenderDecTrans.OperatorId);

                                            command.ExecuteNonQuery();
                                        }
                                        if (conn.State == ConnectionState.Open)
                                            conn.Close();
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                string sBookedCustOrdrNo = "";
                sBookedCustOrdrNo = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo);
                if (((LSRetailPosis.Transaction.PosTransaction)(posTransaction)).EntryStatus != PosTransaction.TransactionStatus.Voided)
                {
                    try
                    {
                        PrintVoucher(sBookedCustOrdrNo, 2);
                    }
                    catch { }

                }

                DropTempTable();

            }
            catch (Exception)
            {

            }
            #endregion
            //End:Nim

            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.PostEndTransaction", "When concluding the transaction, after printing and saving", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PreVoidTransaction(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.PreVoidTransaction", "Before voiding the transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostVoidTransaction(IPosTransaction posTransaction)
        {
            //Start:Nim
            #region Changed By Nimbus

            CheckInventory(posTransaction, (int)Enums.EnumClass.InventoryTransactionType.Void);
            RetailTransaction retailtrans = posTransaction as RetailTransaction;
            if (retailtrans != null)
            {
                if (retailtrans.SalesInvoiceAmounts > 0)
                {
                    return;
                }
                if (retailtrans.IncomeExpenseAmounts != 0)
                {
                    return;
                }

                //if(retailtrans.PartnerData.IsGSSMaturity)
                //{
                //    return;
                //}

                //if(retailtrans.PartnerData.IsRepairReturn)
                //{
                //    return;
                //}

                foreach (SaleLineItem saleLineItem in retailtrans.SaleItems)
                {
                    if (saleLineItem.ItemType == LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                    {
                        int iMetalType = getMetalType(saleLineItem.ItemId);
                        if (iMetalType != (int)Enums.EnumClass.MetalType.PackingMaterial)
                        {
                            //updateCustomerAdvanceAdjustment(Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentTransactionID),
                            //   Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentStoreId),
                            //   Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentTerminalId), 0);

                            //RTS is calling for update changed on 27/11/2015
                            string sTransId = Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentTransactionID);
                            string sStoreId = Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentStoreId);
                            string sTerminalId = Convert.ToString(saleLineItem.PartnerData.ServiceItemCashAdjustmentTerminalId);

                            try
                            {
                                ReadOnlyCollection<object> containerArray;
                                string sMsg = string.Empty;

                                if (PosApplication.Instance.TransactionServices.CheckConnection())
                                {
                                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("updateAdvanceForVoid", sTransId, sStoreId, sTerminalId);
                                    sMsg = Convert.ToString(containerArray[2]);
                                    //MessageBox.Show(sMsg);

                                }
                            }
                            catch (Exception ex)
                            {
                            }
                        }
                    }
                    if (saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                        && Convert.ToString(retailtrans.PartnerData.IsGiftCardIssue) == "0")
                    {
                        //RTS is calling for update changed on 27/11/2015
                        if (!string.IsNullOrEmpty(saleLineItem.PartnerData.TransactionType))
                        {
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

                //Start :
                #region //Nimbus
                using (SqlConnection conn = (Application != null) ? Application.Settings.Database.Connection : ApplicationSettings.Database.LocalConnection)
                {
                    try
                    {
                        //Start : 18/09/14
                        StringBuilder commandText = new StringBuilder();

                        if (!string.IsNullOrEmpty(retailtrans.PartnerData.REPAIRID))
                            commandText.AppendLine(" update RetailRepairDetail set IsDelivered=0 where BatchId ='" + retailtrans.PartnerData.REPAIRID + "'; ");

                        DropTempTable();

                        string sTableName = "ORDERINFO" + "" + ApplicationSettings.Database.TerminalID;

                        commandText.AppendLine("IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
                        commandText.AppendLine(" update RETAILADJUSTMENTTABLE set ISADJUSTED=0 where ORDERNUM in (select ORDERNO  from " + sTableName + ")");
                        commandText.AppendLine(" BEGIN  DROP TABLE " + sTableName + " END ");

                        if (!string.IsNullOrEmpty(commandText.ToString()))
                        {
                            if (conn.State == ConnectionState.Closed)
                                conn.Open();

                            using (SqlCommand cmd = new SqlCommand(commandText.ToString(), conn))
                            {
                                cmd.ExecuteNonQuery();
                            }
                        }
                        //End : 
                    }
                    catch { }
                    finally
                    {
                        if (conn.State == ConnectionState.Open)
                            conn.Close();
                    }
                }
                #endregion
                //End : 18/09/14

            }


            #endregion
            //End:Nim

            string source = "TransactionTriggers.PostVoidTransaction";
            string value = "After voiding the transaction...";
            LSRetailPosis.ApplicationLog.Log(source, value, LSRetailPosis.LogTraceLevel.Trace);
            LSRetailPosis.ApplicationLog.WriteAuditEntry(source, value);
        }

        private void DropTempTable()
        {
            SqlConnection conn = new SqlConnection();
            string sTblName = "NEGCASHPAY" + ApplicationSettings.Terminal.TerminalId;
            string sTblName1 = "DAILYCASHPAY" + ApplicationSettings.Terminal.TerminalId;
            string sTblName2 = "DAILYCASHPAYNEG" + ApplicationSettings.Terminal.TerminalId;
            string sTblName3 = "MAKINGINFO" + ApplicationSettings.Terminal.TerminalId;
            string sTblName4 = "OGPCASHPAY" + ApplicationSettings.Terminal.TerminalId;

            #region Cash pay limit temp table drop
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string sSQLQry = "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName + "')" +
                          " BEGIN  DROP TABLE " + sTblName + " END ";
            sSQLQry = sSQLQry + "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName1 + "')" +
                        " BEGIN  DROP TABLE " + sTblName1 + " END ";
            sSQLQry = sSQLQry + "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName2 + "')" +
                        " BEGIN  DROP TABLE " + sTblName2 + " END ";
            sSQLQry = sSQLQry + "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName3 + "')" +
                        " BEGIN  DROP TABLE " + sTblName3 + " END ";
            sSQLQry = sSQLQry + "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName4 + "')" +
                       " BEGIN  DROP TABLE " + sTblName4 + " END ";

            using (SqlCommand command = new SqlCommand(sSQLQry, conn))
            {
                command.ExecuteNonQuery();
            }
            #endregion
        }

        public void PreReturnTransaction(IPreTriggerResult preTriggerResult, IRetailTransaction originalTransaction, IPosTransaction posTransaction)
        {
            //Start:Nim
            #region Chages BY NIMBUS
            RetailTransaction SaleTrans = posTransaction as RetailTransaction; // sales line trans
            RetailTransaction retailTransaction = originalTransaction as RetailTransaction;// current trans

            //if(SaleTrans.SaleItems.Count > 0)
            //{
            //    preTriggerResult.ContinueOperation = false;
            //    preTriggerResult.MessageId = 999998;
            //    return;
            //}
            string sAdjustmentId = AdjustmentItemID();

            string sGetDefaultConfigId = GetDefaultConfigId();
            int iMetalType = 0;

            string sGSSAdjItemId = "";
            string sGSSDiscItemId = "";
            string sCRWREPAIRADJITEMFOROG = "";
            string sCRWREPAIRADJITEM = "";
            string sGSSAmountAdjItemId = "";
            string sGSSAmountDiscItemId = "";
            GSSAdjustmentItemID(ref sGSSAdjItemId, ref sGSSDiscItemId,
                ref sCRWREPAIRADJITEMFOROG, ref sCRWREPAIRADJITEM,
                ref sGSSAmountAdjItemId, ref sGSSAmountDiscItemId);

            if (retailTransaction != null && SaleTrans != null)
            {

                #region If no state code in customer
                string sStoreTaxState = "";
                string sStoreTaxCountry = "";
                if (Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState) != string.Empty)
                    sStoreTaxState = Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState);

                if (Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxCountry) != string.Empty)
                    sStoreTaxCountry = Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxCountry);

                string sCustTaxState = Convert.ToString(retailTransaction.Customer.State);
                string sCustTaxCountry = Convert.ToString(retailTransaction.Customer.Country);

                if (sStoreTaxState != sCustTaxState) // for cal CGST and SGST only @PAN INDIA
                    retailTransaction.Customer.State = sStoreTaxState;

                if (sStoreTaxCountry != sCustTaxCountry) // for cal CGST and SGST only @PAN INDIA
                    retailTransaction.Customer.Country = sStoreTaxCountry;
                #endregion


                SaleTrans.PartnerData.SingaporeTaxCal = "0";
                foreach (SaleLineItem SLineItem in SaleTrans.SaleItems)
                {
                    if (SLineItem.ItemId == sAdjustmentId || SLineItem.ItemId == sGSSAdjItemId || SLineItem.ItemId == sGSSAmountAdjItemId)
                    {
                        if (retailTransaction.SaleIsReturnSale == true)
                        {
                            MessageBox.Show("Sales return can not done if adjustment is there");
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }
                    }
                    if (Convert.ToInt16(SLineItem.PartnerData.TransactionType) == 0 && SLineItem.Quantity > 0)
                    {
                        if (retailTransaction.SaleIsReturnSale == true)
                        {
                            MessageBox.Show("Sales return can not done if sales line is there first");
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }
                    }

                    if (Convert.ToInt16(SLineItem.PartnerData.TransactionType) > 0)
                    {
                        if (retailTransaction.SaleIsReturnSale == true)
                        {
                            MessageBox.Show("Sales return can not done if OG transaction are in sales line");
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }
                    }

                    iMetalType = getMetalType(SLineItem.ItemId);

                    if (iMetalType == (int)Enums.EnumClass.MetalType.GiftVoucher)
                    {
                        if (retailTransaction.SaleIsReturnSale == true)
                        {
                            MessageBox.Show("Sales return can not done if gift item are in sales return line");
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }
                    }
                }

                if (SaleTrans.Customer.CustomerId != null)
                {
                    if (SaleTrans.Customer.CustomerId != retailTransaction.Customer.CustomerId)
                    {
                        MessageBox.Show("Selected transaction line customer should be same as current line customer");
                        preTriggerResult.ContinueOperation = false;
                        return;
                    }
                }
            }


            if (retailTransaction != null)
            {
                if (retailTransaction.SaleIsReturnSale)
                {
                    SaleTrans.PartnerData.SingaporeTaxCal = "0";
                    foreach (SaleLineItem saleLineItem1 in SaleTrans.SaleItems)
                    {

                        string sSKU = string.Empty;
                        DataTable dt = new DataTable();
                        dt.Columns.Add("ItemId");
                        DataRow row = null;

                        var query = from i in SaleTrans.SaleItems
                                    orderby i.ItemId
                                    select new { i.ItemId };

                        foreach (var rowObj in query)
                        {
                            row = dt.NewRow();
                            dt.Rows.Add(rowObj.ItemId);
                        }
                        foreach (DataRow dr in dt.Rows)
                        {
                            foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                            {
                                if (saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && !saleLineItem1.Voided)
                                {
                                    if (Convert.ToString(retailTransaction.ReceiptId) == Convert.ToString(saleLineItem1.PartnerData.ReturnReceiptId) && !saleLineItem.Voided)
                                    {
                                        if (Convert.ToString(dr["ITEMID"]) == saleLineItem.ItemId && saleLineItem1.PartnerData.NimReturnLine == saleLineItem.PartnerData.NimReturnLine)
                                        {
                                            MessageBox.Show("Same Item is already taken for return");
                                            preTriggerResult.ContinueOperation = false;
                                            return;
                                        }
                                    }
                                }
                            }
                        }

                    }
                }
            }


            DataTable dtRec = new DataTable();


            bool isExchange = false;
            bool isCashBack = false;
            bool isConToAdv = false;
            bool isFullSalesReturn = false;

            decimal dTotExchReturnQty = 0m;
            decimal dTotExchReturnAmt = 0m;

            if (retailTransaction != null)
            {
                retailTransaction.PartnerData.SingaporeTaxCal = "0";
                ReadOnlyCollection<object> containerArraySR;

                try
                {
                    if (this.Application.TransactionServices.CheckConnection())
                    {
                        string sTransactionId = originalTransaction.TransactionId;
                        string sStoreId = originalTransaction.StoreId;
                        string sTerminalId = originalTransaction.TerminalId;

                        containerArraySR = this.Application.TransactionServices.InvokeExtension
                                                                                    ("GetSalesReturnInfo", sTransactionId, sStoreId, sTerminalId);
                        DataSet dsTransDetail = new DataSet();
                        StringReader srTransDetail = new StringReader(Convert.ToString(containerArraySR[3]));

                        if (Convert.ToString(containerArraySR[3]).Trim().Length > 38)
                        {
                            dsTransDetail.ReadXml(srTransDetail);
                        }

                        if (dsTransDetail != null
                            && dsTransDetail.Tables.Count > 0 && dsTransDetail.Tables[0].Rows.Count > 0)
                        {
                            dtRec = dsTransDetail.Tables[0];

                            dtRec.Columns.Add("SKUDETAILS", typeof(string));

                            DataSet dsIngredient = new DataSet();
                            StringReader srIngredient = new StringReader(Convert.ToString(containerArraySR[4]));

                            if (Convert.ToString(containerArraySR[4]).Trim().Length > 38)
                            {
                                dsIngredient.ReadXml(srIngredient);
                                dsIngredient.AcceptChanges();
                            }

                            if (dsIngredient != null
                                && dsIngredient.Tables.Count > 0 && dsIngredient.Tables[0].Rows.Count > 0)
                            {
                                int d = 1; //changes decimal to int // 19/05/2014
                                foreach (DataRow drtrans in dtRec.Rows)
                                {
                                    DataTable dtfilter = new DataTable();
                                    string sfilter = "RefLineNum = '" + dtRec.Rows[Convert.ToInt32(d) - 1]["LineNum"] + "' "; //changes decimal to int // 19/05/2014 only d was there
                                    DataView dv = new DataView(dsIngredient.Tables[0]);
                                    dv.RowFilter = sfilter;
                                    dtfilter = dv.ToTable();
                                    dtfilter.AcceptChanges();
                                    if (dtfilter != null && dtfilter.Rows.Count > 0) //blocked  19/05/2014
                                    {
                                        MemoryStream mstr = new MemoryStream();
                                        dtfilter.WriteXml(mstr, true);
                                        mstr.Seek(0, SeekOrigin.Begin);
                                        StreamReader sr = new StreamReader(mstr);
                                        string sXML = string.Empty;
                                        sXML = sr.ReadToEnd();
                                        dtRec.Rows[Convert.ToInt32(d) - 1]["SKUDETAILS"] = sXML;
                                        dtRec.AcceptChanges();
                                    }
                                    d++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {


                }

                if (dtRec == null || dtRec.Rows.Count <= 0)
                {
                    string commandString = " SELECT RCT.[LINENUM] LINENUM,[PIECES],[QUANTITY],[CRATE] AS RATE,[RATETYPE],[MAKINGRATE],[MAKINGTYPE],[AMOUNT], " +
                                           " [MAKINGDISCOUNT],[MAKINGAMOUNT] MAKINGAMOUNT,[TOTALAMOUNT],[TOTALWEIGHT],[LOSSPERCENTAGE],[LOSSWEIGHT], " +
                                           " [EXPECTEDQUANTITY],[TRANSACTIONTYPE],[OWN],[SKUDETAILS],[ORDERNUM],[ORDERLINENUM],[CUSTNO]" +
                                           " ,ISNULL(WastageType,0) AS WastageType,ISNULL(WastageQty,0) AS WastageQty," +
                                           " ISNULL(WastageAmount,0) AS WastageAmount" + // Changed for wastage
                                           " ,ISNULL(WastagePercentage,0) AS WastagePercentage,ISNULL(WastageRate,0) AS WastageRate," +
                                           " ISNULL(MakingDiscountAmount,0) as MakingDiscountAmount,MAKINGDISCTYPE" + // Making Discount Type
                                           " ,T.TRANSDATE,T.LOYALTYTYPECODE,T.LOYALTYPROVIDER,T.LOYALTYCARDNO,ISNULL(IsIngredient,0) AS IsIngredient," +
                                           " ISNULL(RCT.CONFIGID,'') AS CONFIGID," +
                                           " ISNULL(RCT.PURITY,0) AS PURITY" +
                                           " ,ACTMKRATE,ACTTOTAMT,CAST(ISNULL(CHANGEDTOTAMT,0) AS DECIMAL(28,2)) CHANGEDTOTAMT" +
                                           " ,CAST(ISNULL(LINEDISC,0) AS DECIMAL(28,2)) LINEDISC,isnull(TRANSFERCOSTPRICE,0) TRANSFERCOSTPRICE" +//GOLDTAXVALUE
                                           " ,isnull(GOLDTAXVALUE,0) GOLDTAXVALUE,isnull(ST.TAXAMOUNT,0) TAXAMOUNT" +
                                           " ,isnull(WTDIFFDISCQTY,0) WTDIFFDISCQTY,isnull(PurityReading1,0) PurityReading1" +
                                           " ,isnull(PurityReading2,0) PurityReading2 ,isnull(PurityReading3,0) PurityReading3" +
                                           " ,ISNULL(RCT.PURITYPERSON,'') AS PURITYPERSON" +
                                           " ,ISNULL(RCT.PURITYPERSONNAME,'') AS PURITYPERSONNAME" +
                                           " ,isnull(SKUAgingDiscType,0) SKUAgingDiscType" +
                                           " ,isnull(SKUAgingDiscAmt,0) SKUAgingDiscAmt" +
                                           " ,isnull(TierDiscType,0) TierDiscType" +
                                           " ,isnull(TierDiscAmt,0) TierDiscAmt" +
                                           " ,isnull(WTDIFFDISCAMT,0) WTDIFFDISCAMT" +
                                           " ,isnull(MRPOrMkDiscType,0) MRPOrMkDiscType" +
                                           " ,isnull(MRPOrMkDiscAmt,0) MRPOrMkDiscAmt" +
                                           " ,isnull(OGPSTONEPCS,0) OGPSTONEPCS" +
                                           " ,isnull(OGPSTONEWT,0) OGPSTONEWT" +
                                           " ,isnull(OGPSTONEUNIT,'') OGPSTONEUNIT" +
                                           " ,isnull(OGPSTONERATE,0) OGPSTONERATE" +
                                           " ,isnull(OGPSTONEAMOUNT,0) OGPSTONEAMOUNT" +
                                           " ,ISNULL(RCT.MRPORMKPROMODISCCODE,'') AS MRPORMKPROMODISCCODE" +
                                           " ,isnull(RCT.MRPORMKPROMODISCTYPE,0) MRPORMKPROMODISCTYPE" +
                                           " ,isnull(RCT.MRPORMKPROMODISCAMT,0) MRPORMKPROMODISCAMT" +
                                           " ,OpeningDiscType,OpeningDisc FROM [RETAIL_CUSTOMCALCULATIONS_TABLE] RCT " +
                                           " INNER join RETAILTRANSACTIONSALESTRANS ST  ON RCT.TRANSACTIONID = ST.TRANSACTIONID " +
                                           " AND RCT.STOREID = ST.STORE  AND RCT.TERMINALID = ST.TERMINALID " +
                                           " AND RCT.LINENUM = ST.LINENUM " +
                                           " INNER JOIN RETAILTRANSACTIONTABLE T " +
                                           " ON RCT.TRANSACTIONID = T.TRANSACTIONID " + // type = 2 for sales
                                           " AND RCT.STOREID = T.STORE " +
                                           " AND RCT.TERMINALID = T.TERMINAL" +
                                           " WHERE  RCT.DATAAREAID=@DATAAREAID" +
                                           " AND [STOREID]=@STOREID AND RCT.[TERMINALID]=@TERMINALID AND " + //
                                           " RCT.TRANSACTIONTYPE <> 5 AND" +
                                           " RCT.TRANSACTIONID=@TRANSACTIONID AND T.TYPE = 2 ";

                    SqlConnection connection = new SqlConnection();

                    if (application != null)
                        connection = application.Settings.Database.Connection;
                    else
                        connection = ApplicationSettings.Database.LocalConnection;


                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlCommand command = new SqlCommand(commandString, connection);

                    command.Parameters.Add(new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID));
                    command.Parameters.Add(new SqlParameter("@STOREID", originalTransaction.StoreId));
                    command.Parameters.Add(new SqlParameter("@TERMINALID", originalTransaction.TerminalId));
                    command.Parameters.Add(new SqlParameter("@TRANSACTIONID", originalTransaction.TransactionId));


                    command.CommandTimeout = 0;
                    SqlDataReader reader = command.ExecuteReader();
                    dtRec.Load(reader);

                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }

                retailTransaction.SaleIsReturnSale = true;
            }



            if (dtRec != null && dtRec.Rows.Count > 0)
            {
                foreach (DataColumn col in dtRec.Columns)
                    col.ReadOnly = false;
            }

            System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> lineitemoriginal = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(originalTransaction)).SaleItems);
            System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> lineitem = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(originalTransaction)).SaleItems);
            if (dtRec != null && dtRec.Rows.Count > 0)
            {
                #region
                string sSalesLoaltyType = Convert.ToString(dtRec.Rows[0]["LOYALTYTYPECODE"]);
                string sSalesLoaltyTypeProv = Convert.ToString(dtRec.Rows[0]["LOYALTYPROVIDER"]);
                string sSalesLoaltyCardNo = Convert.ToString(dtRec.Rows[0]["LOYALTYCARDNO"]);

                decimal dCValue = 0m;
                decimal dTotCValue = 0m;
                decimal dExtraCalculatedGoldTaxAmt = 0;

                decimal dCValueForOtherConfig = 0m; // added on 060818

                frmOptionSelectionExchangeBuyback objReturnOption = new frmOptionSelectionExchangeBuyback();
                objReturnOption.ShowDialog();

                if (objReturnOption.isCancel == true)
                {
                    preTriggerResult.ContinueOperation = false;
                    return;
                }


                isExchange = objReturnOption.isExchange;
                isCashBack = objReturnOption.isCashBack;
                isConToAdv = objReturnOption.isConvertToAdv;
                int iVal = 0;
                if (isExchange)
                    iVal = 1;
                else if (isCashBack)
                    iVal = 2;
                else if (isConToAdv)
                    iVal = 3;

                #region Validation
                if (retailTransaction != null)
                {
                    if (retailTransaction.SaleIsReturnSale)
                    {
                        foreach (SaleLineItem saleLineItem1 in SaleTrans.SaleItems)
                        {
                            foreach (SaleLineItem saleLineItem in retailTransaction.SaleItems)
                            {
                                if (saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service && !saleLineItem1.Voided)
                                {
                                    if (saleLineItem.ItemId != saleLineItem1.ItemId)
                                    {
                                        if (iVal != saleLineItem1.PartnerData.TransReturnPayMode && !saleLineItem.Voided)
                                        {
                                            if (saleLineItem1.PartnerData.TransReturnPayMode == 1 && iVal != 1)
                                            {
                                                MessageBox.Show("Only exchange is allowed");
                                                preTriggerResult.ContinueOperation = false;
                                                return;
                                            }
                                            else if (saleLineItem1.PartnerData.TransReturnPayMode == 2 && iVal != 2)
                                            {
                                                MessageBox.Show("Only cash back is allowed");
                                                preTriggerResult.ContinueOperation = false;
                                                return;
                                            }
                                            else if (saleLineItem1.PartnerData.TransReturnPayMode == 3 && iVal != 3)
                                            {
                                                MessageBox.Show("Only convert to advance is allowed");
                                                preTriggerResult.ContinueOperation = false;
                                                return;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }


                if (objReturnOption.isFullReturn == true)
                {
                    isFullSalesReturn = true;
                    foreach (var item in lineitemoriginal)
                    {
                        item.PartnerData.isFullReturn = true;

                        GetItemType(item.ItemId);
                        if (iOWNDMD == 1 || iOWNOG == 1 || iOTHERDMD == 1 || iOTHEROG == 1)
                        {
                            MessageBox.Show("Sales return can not done if OG item is there in sales line.");
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }
                    }
                }
                else
                {
                    isFullSalesReturn = false;
                    foreach (var item in lineitemoriginal)
                    {
                        item.PartnerData.isFullReturn = false;

                        GetItemType(item.ItemId);
                        if (iOWNDMD == 1 || iOWNOG == 1 || iOTHERDMD == 1 || iOTHEROG == 1)
                        {
                            MessageBox.Show("Sales return can not done if OG item is there in sales line.");
                            preTriggerResult.ContinueOperation = false;
                            return;
                        }
                    }
                }
                #endregion

                #region PNG Return Policy
                if (isFullSalesReturn || isExchange || isCashBack || isConToAdv)
                {
                    DateTime dtTransDate = Convert.ToDateTime(dtRec.Rows[0]["TRANSDATE"]);
                    int iFullExchngValidDay = GetFullExchngValidDay(posTransaction.StoreId);

                    if ((DateTime.Now - dtTransDate).TotalDays <= iFullExchngValidDay)
                    {
                        foreach (var item in lineitem)
                        {
                            string sTransactionId = originalTransaction.TransactionId;
                            string sStoreId = originalTransaction.StoreId;
                            string sTerminalId = originalTransaction.TerminalId;

                            int iSalesReturnValidDay = 0;
                            decimal dCashBackPct = 0;
                            decimal dExchPct = 0;
                            decimal dConvToAdvPct = 0m;

                            GetSalesReturnPolicy(item.ItemId, ref iSalesReturnValidDay, ref dCashBackPct, ref dConvToAdvPct, ref dExchPct);

                            //if (dCashBackPct > 0 || dConvToAdvPct > 0 || dExchPct > 0)
                            //{
                            #region 111
                            if (isMRP(item.ItemId))
                                isFullSalesReturn = true;
                            else
                                isFullSalesReturn = false;

                            iMetalType = getMetalType(item.ItemId);

                            foreach (DataRow dr in dtRec.Rows)
                            {
                                #region Start : UCP item return issue
                                SqlConnection conn = new SqlConnection();
                                if (application != null)
                                    conn = application.Settings.Database.Connection; //Settings.Database.Connection;
                                else
                                    conn = ApplicationSettings.Database.LocalConnection;

                                bool isMrp = false;
                                int cntdisc = 0;
                                isMrp = isUCP(item.ItemId, conn);
                                if (isMrp)
                                {
                                    if (Convert.ToDecimal(item.LineId) == Convert.ToDecimal(dr["LINENUM"]))
                                    {
                                        cntdisc = 0;
                                        foreach (DiscountItem discLine in item.DiscountLines)
                                        {
                                            if (cntdisc == 0)
                                            {
                                                discLine.Percentage = ((item.LineDiscount / item.Price) * 100) + item.TotalPctDiscount;
                                                cntdisc++;
                                            }
                                            else
                                            {
                                                discLine.Percentage = 0;
                                            }

                                        }
                                        retailTransaction.LineDiscount = 0;

                                        item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                        //if (isExchange)
                                        //    item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero)))) * dExchPct / 100;
                                        //else if (isCashBack)
                                        //    item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero)))) * dCashBackPct / 100;
                                        //else if (isConToAdv)
                                        //    item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero)))) * dConvToAdvPct / 100;


                                        if (isExchange)
                                            item.PartnerData.RETURNTYPE = "Exchange";
                                        else if (isCashBack)
                                            item.PartnerData.RETURNTYPE = "CashBack";
                                        else if (isConToAdv)
                                            item.PartnerData.RETURNTYPE = "ConvToAdv";

                                        item.PartnerData.CalExchangeQty = 0;
                                    }
                                    item.PartnerData.isMRP = true;
                                }

                                #endregion End : UCP item return issue
                                else
                                {
                                    if (isFullSalesReturn == false && isExchange)
                                    {
                                        #region [Exchange / Buy Back]
                                        decimal dOldHdrRate = 0m;

                                        DateTime dtDate = Convert.ToDateTime(dtRec.Rows[0]["TRANSDATE"]);

                                        if (Convert.ToInt32(dr["IsIngredient"]) == 1)
                                        {
                                            dCValue = 0;
                                            DataSet dsIngredients = new DataSet();
                                            StringReader reader = new StringReader(Convert.ToString(dr["SKUDETAILS"]));
                                            dsIngredients.ReadXml(reader);

                                            if (dsIngredients != null && dsIngredients.Tables[0].Rows.Count > 0)
                                            {
                                                foreach (DataRow drIng in dsIngredients.Tables[0].Rows)
                                                {
                                                    //dIngrdRate = 0m;
                                                    string sSelfStoreId = ApplicationSettings.Terminal.StoreId;

                                                    if (Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.Gold)
                                                    {
                                                        if (sGetDefaultConfigId == Convert.ToString(drIng["ConfigID"]))
                                                        {
                                                            dOldHdrRate = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["RATE"])), 2, MidpointRounding.AwayFromZero))));

                                                            dCValue += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["CValue"])), 2, MidpointRounding.AwayFromZero))));
                                                        }
                                                        if (sGetDefaultConfigId != Convert.ToString(drIng["ConfigID"]))
                                                        {
                                                            dCValueForOtherConfig = 0;
                                                            dCValueForOtherConfig += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["CValue"])), 2, MidpointRounding.AwayFromZero))));
                                                        }
                                                        //dOldGoldQty += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["QTY"])), 3, MidpointRounding.AwayFromZero))));
                                                    }
                                                    else if ((Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.GiftVoucher)) // added on  23/08/2016 for Malabar Dubai
                                                    {
                                                        preTriggerResult.ContinueOperation = false;
                                                        MessageBox.Show("Gift voucher can not return.");
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (iMetalType == (int)Enums.EnumClass.MetalType.Gold)
                                            {
                                                if (sGetDefaultConfigId == Convert.ToString(dr["ConfigID"]))
                                                {
                                                    dCValue = 0;
                                                    dOldHdrRate = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["RATE"])), 2, MidpointRounding.AwayFromZero))));

                                                    dCValue += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                                }
                                                if (sGetDefaultConfigId != Convert.ToString(dr["ConfigID"]))
                                                {
                                                    dCValueForOtherConfig = 0;
                                                    dCValueForOtherConfig += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                                }
                                                //dOldGoldQty += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["QTY"])), 3, MidpointRounding.AwayFromZero))));
                                            }
                                            if ((iMetalType == (int)Enums.EnumClass.MetalType.GiftVoucher))
                                            {
                                                preTriggerResult.ContinueOperation = false;
                                                MessageBox.Show("Gift voucher can not return.");
                                                return;
                                            }
                                        }

                                        #endregion
                                        if (dOldHdrRate > 0)
                                        {
                                            dTotCValue = 0;
                                            dTotCValue += dCValue;
                                        }
                                        else if (dCValueForOtherConfig != 0 && dOldHdrRate == 0)
                                        {
                                            dTotCValue = 0;
                                            dTotCValue += dCValueForOtherConfig;
                                        }
                                        else
                                            item.PartnerData.CalExchangeQty = 0m;

                                        if (isExchange || isConToAdv)
                                        {
                                            decimal dTaxPerc = getItemTaxPercentage(item.ItemId);
                                            decimal dGoldTax = 0m;

                                            if (dTaxPerc > 0 && dTotCValue > 0)
                                                dGoldTax = decimal.Round(dTotCValue * (dTaxPerc / 100), 2, MidpointRounding.AwayFromZero);// Convert.ToString(dGVal * (dTaxPerc / 100));
                                            else
                                                dGoldTax = 0;

                                            item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));

                                            //decimal dPrice = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero)))); ;

                                            //if (isExchange)
                                            //    item.Price = dPrice * dExchPct / 100;
                                            //else if (isCashBack)
                                            //    item.Price = dPrice * dCashBackPct / 100;
                                            //else if (isConToAdv)
                                            //    item.Price = dPrice * dConvToAdvPct / 100;


                                            decimal dDiscPct = 0m;
                                            if (item.Price > 0)
                                            {
                                                dDiscPct = (item.LineDiscount) / (item.Price * item.Quantity) * 100;
                                            }

                                            foreach (var itemD in item.DiscountLines)
                                            {
                                                itemD.Percentage = dDiscPct;
                                            }

                                            if (dOldHdrRate > 0)
                                            {
                                                decimal dTax = ((dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) * dTaxPerc) / (100 + dTaxPerc);
                                                dExtraCalculatedGoldTaxAmt = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"])))), 2, MidpointRounding.AwayFromZero)))); ;

                                                dTotExchReturnQty += Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(item.NetAmountWithNoTax) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"])) - dTax) / dOldHdrRate), 3, MidpointRounding.AwayFromZero))));
                                                item.PartnerData.CalExchangeQty = dTotExchReturnQty;
                                                dTotExchReturnQty = 0m;
                                            }
                                            else
                                                item.PartnerData.CalExchangeQty = 0m;

                                            item.PartnerData.NewTaxPerc = 0;
                                        }
                                        else
                                        {
                                            if (dOldHdrRate > 0)
                                            {
                                                dTotExchReturnQty += Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(item.NetAmountWithNoTax)) / dOldHdrRate), 3, MidpointRounding.AwayFromZero))));
                                                item.PartnerData.CalExchangeQty = dTotExchReturnQty;
                                                dTotExchReturnQty = 0m;
                                            }
                                            else
                                                item.PartnerData.CalExchangeQty = 0m;

                                            item.PartnerData.NewTaxPerc = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) * Math.Abs(Convert.ToDecimal(dr["TAXAMOUNT"])) / 100), 15, MidpointRounding.AwayFromZero))));
                                            SaleTrans.PartnerData.CashReturnWithOGTax = "1";//SaleTrans=current retail transaction


                                            item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));

                                            //decimal dPrice = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));

                                            //if (isExchange)
                                            //    item.Price = dPrice * dExchPct / 100;
                                            //else if (isCashBack)
                                            //    item.Price = dPrice * dCashBackPct / 100;
                                            //else if (isConToAdv)
                                            //    item.Price = dPrice * dConvToAdvPct / 100;
                                        }
                                    }

                                    #region convert to adv on 210618

                                    if (isFullSalesReturn == false && isConToAdv)
                                    {
                                        #region [isConToAdv]
                                        decimal dOldHdrRate = 0m;

                                        DateTime dtDate = Convert.ToDateTime(dtRec.Rows[0]["TRANSDATE"]);

                                        if (Convert.ToInt32(dr["IsIngredient"]) == 1)
                                        {
                                            dCValue = 0;
                                            DataSet dsIngredients = new DataSet();
                                            StringReader reader = new StringReader(Convert.ToString(dr["SKUDETAILS"]));
                                            dsIngredients.ReadXml(reader);

                                            if (dsIngredients != null && dsIngredients.Tables[0].Rows.Count > 0)
                                            {
                                                foreach (DataRow drIng in dsIngredients.Tables[0].Rows)
                                                {
                                                    //dIngrdRate = 0m;
                                                    string sSelfStoreId = ApplicationSettings.Terminal.StoreId;

                                                    if (Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.Gold)
                                                    {
                                                        if (sGetDefaultConfigId == Convert.ToString(drIng["ConfigID"]))
                                                        {
                                                            dOldHdrRate = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["RATE"])), 2, MidpointRounding.AwayFromZero))));

                                                            dCValue += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["CValue"])), 2, MidpointRounding.AwayFromZero))));
                                                        }
                                                        if (sGetDefaultConfigId != Convert.ToString(drIng["ConfigID"]))
                                                        {
                                                            dCValueForOtherConfig = 0;
                                                            dCValueForOtherConfig += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["CValue"])), 2, MidpointRounding.AwayFromZero))));
                                                        }
                                                        //dOldGoldQty += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["QTY"])), 3, MidpointRounding.AwayFromZero))));
                                                    }
                                                    else if ((Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.GiftVoucher)) // added on  23/08/2016 for Malabar Dubai
                                                    {
                                                        preTriggerResult.ContinueOperation = false;
                                                        MessageBox.Show("Gift voucher can not return.");
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (iMetalType == (int)Enums.EnumClass.MetalType.Gold)
                                            {
                                                if (sGetDefaultConfigId == Convert.ToString(dr["ConfigID"]))
                                                {
                                                    dCValue = 0;
                                                    dOldHdrRate = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["RATE"])), 2, MidpointRounding.AwayFromZero))));

                                                    dCValue += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                                }
                                                if (sGetDefaultConfigId != Convert.ToString(dr["ConfigID"]))
                                                {
                                                    dCValueForOtherConfig = 0;
                                                    dCValueForOtherConfig += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                                }
                                                //dOldGoldQty += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["QTY"])), 3, MidpointRounding.AwayFromZero))));
                                            }
                                            if ((iMetalType == (int)Enums.EnumClass.MetalType.GiftVoucher))
                                            {
                                                preTriggerResult.ContinueOperation = false;
                                                MessageBox.Show("Gift voucher can not return.");
                                                return;
                                            }
                                        }

                                        #endregion
                                        if (dOldHdrRate > 0)
                                        {
                                            dTotCValue = 0;
                                            dTotCValue += dCValue;
                                            // dTotCValue += dCValue;
                                        }
                                        else if (dCValueForOtherConfig != 0 && dOldHdrRate == 0)
                                        {
                                            dTotCValue = 0;
                                            dTotCValue += dCValueForOtherConfig;
                                        }


                                        if (isExchange || isConToAdv)
                                        {
                                            decimal dTaxPerc = getItemTaxPercentage(item.ItemId);
                                            decimal dGoldTax = 0m;

                                            if (dTaxPerc > 0 && dTotCValue > 0)
                                                dGoldTax = decimal.Round(dTotCValue * (dTaxPerc / 100), 2, MidpointRounding.AwayFromZero);// Convert.ToString(dGVal * (dTaxPerc / 100));
                                            else
                                                dGoldTax = 0;

                                            item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));
                                            //dTotCValue

                                            //decimal dPrice = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));

                                            //if (isExchange)
                                            //    item.Price = dPrice * dExchPct / 100;
                                            //else if (isCashBack)
                                            //    item.Price = dPrice * dCashBackPct / 100;
                                            //else if (isConToAdv)
                                            //    item.Price = dPrice * dConvToAdvPct / 100;

                                            decimal dDiscPct = 0m;
                                            dDiscPct = (item.LineDiscount) / (item.Price * item.Quantity) * 100;
                                            // DISCAMOUNT/(203.28*QTY)*100
                                            foreach (var itemD in item.DiscountLines)
                                            {
                                                itemD.Percentage = dDiscPct;
                                            }
                                            item.PartnerData.NewTaxPerc = 0;
                                        }
                                        else
                                        {
                                            // item.PartnerData.NewTaxAmt = Convert.ToDecimal(dr["TAXAMOUNT"]);
                                            item.PartnerData.NewTaxPerc = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) * Math.Abs(Convert.ToDecimal(dr["TAXAMOUNT"])) / 100), 15, MidpointRounding.AwayFromZero))));
                                            SaleTrans.PartnerData.CashReturnWithOGTax = "1";//SaleTrans=current retail transaction

                                            item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));

                                            //decimal dPrice = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));

                                            //if (isExchange)
                                            //    item.Price = dPrice * dExchPct / 100;
                                            //else if (isCashBack)
                                            //    item.Price = dPrice * dCashBackPct / 100;
                                            //else if (isConToAdv)
                                            //    item.Price = dPrice * dConvToAdvPct / 100;
                                        }
                                    }
                                    #endregion

                                    if (isExchange)
                                        item.PartnerData.RETURNTYPE = "Exchange";
                                    else if (isCashBack)
                                        item.PartnerData.RETURNTYPE = "CashBack";
                                    else if (isConToAdv)
                                        item.PartnerData.RETURNTYPE = "ConvToAdv";

                                    item.PartnerData.isMRP = false;
                                }
                                if (Convert.ToDecimal(item.LineId) == Convert.ToDecimal(dr["LINENUM"]))
                                {
                                    if (!isMrp)
                                    {
                                        decimal dBulkMetalRate = 0m;
                                        decimal dTotAmtNew = 0m;
                                        if (IsBulkItem(item.ItemId))
                                        {
                                            item.PartnerData.BulkItem = 1;

                                            #region
                                            string sSelfStoreId = ApplicationSettings.Terminal.StoreId;

                                            // int iMetalType = getMetalType(item.ItemId);

                                            if (iMetalType == (int)Enums.EnumClass.MetalType.Gold)
                                            {
                                                if (isExchange)
                                                {
                                                    if (sGetDefaultConfigId == Convert.ToString(dr["ConfigID"]))
                                                    {
                                                        dCValue = 0;
                                                        dBulkMetalRate = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["RATE"])), 2, MidpointRounding.AwayFromZero))));
                                                        dCValue += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                                    }
                                                    if (sGetDefaultConfigId != Convert.ToString(dr["ConfigID"]))
                                                    {
                                                        dCValueForOtherConfig = 0;
                                                        dCValueForOtherConfig += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                                    }

                                                    if (dBulkMetalRate > 0)
                                                    {
                                                        dTotCValue = 0;
                                                        //dTotExchReturnQty += Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / dBulkMetalRate), 3, MidpointRounding.AwayFromZero))));
                                                        dTotExchReturnQty += Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(item.NetAmountWithNoTax)) / dBulkMetalRate), 3, MidpointRounding.AwayFromZero))));
                                                        item.PartnerData.CalExchangeQty = dTotExchReturnQty;
                                                        dTotExchReturnQty = 0m;

                                                        dTotCValue = dCValue;
                                                    }
                                                    else if (dCValueForOtherConfig > 0)
                                                    {
                                                        dTotCValue = dCValueForOtherConfig;
                                                    }
                                                    else
                                                        item.PartnerData.CalExchangeQty = 0m;

                                                    item.PartnerData.NewTaxPerc = 0;
                                                }
                                                //else
                                                //{
                                                //    item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                                //}


                                                if (isExchange || isConToAdv)
                                                {
                                                    decimal dTaxPerc = getItemTaxPercentage(item.ItemId);
                                                    decimal dGoldTax = 0m;

                                                    if (dTaxPerc > 0 && dTotCValue > 0)
                                                        dGoldTax = decimal.Round(dTotCValue * (dTaxPerc / 100), 2, MidpointRounding.AwayFromZero);// Convert.ToString(dGVal * (dTaxPerc / 100));
                                                    else
                                                        dGoldTax = 0;


                                                    item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));
                                                    //dTotCValue

                                                    //decimal dPrice = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));

                                                    //if (isExchange)
                                                    //    item.Price = dPrice * dExchPct / 100;
                                                    //else if (isCashBack)
                                                    //    item.Price = dPrice * dCashBackPct / 100;
                                                    //else if (isConToAdv)
                                                    //    item.Price = dPrice * dConvToAdvPct / 100;

                                                    decimal dDiscPct = 0m;
                                                    dDiscPct = (item.LineDiscount) / (item.Price * item.Quantity) * 100;
                                                    // DISCAMOUNT/(203.28*QTY)*100
                                                    foreach (var itemD in item.DiscountLines)
                                                    {
                                                        itemD.Percentage = dDiscPct;
                                                    }

                                                    item.PartnerData.NewTaxPerc = 0;
                                                }
                                                else
                                                {
                                                    SaleTrans.PartnerData.CashReturnWithOGTax = "1";
                                                    item.PartnerData.NewTaxPerc = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) * Math.Abs(Convert.ToDecimal(dr["TAXAMOUNT"])) / 100), 15, MidpointRounding.AwayFromZero))));


                                                    item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                                    //decimal dPrice = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));

                                                    //if (isExchange)
                                                    //    item.Price = dPrice * dExchPct / 100;
                                                    //else if (isCashBack)
                                                    //    item.Price = dPrice * dCashBackPct / 100;
                                                    //else if (isConToAdv)
                                                    //    item.Price = dPrice * dConvToAdvPct / 100;

                                                }
                                            }
                                            else
                                            {
                                                item.PartnerData.NewTaxPerc = 0;
                                                item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));

                                                //decimal dPrice = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));

                                                //if (isExchange)
                                                //    item.Price = dPrice * dExchPct / 100;
                                                //else if (isCashBack)
                                                //    item.Price = dPrice * dCashBackPct / 100;
                                                //else if (isConToAdv)
                                                //    item.Price = dPrice * dConvToAdvPct / 100;
                                            }
                                            #endregion
                                        }
                                        else
                                        {
                                            item.PartnerData.BulkItem = 0;
                                            if (isExchange || isConToAdv)
                                            {
                                                decimal dTaxPerc = getItemTaxPercentage(item.ItemId);
                                                decimal dGoldTax = 0m;

                                                if (dTaxPerc > 0 && dTotCValue > 0)
                                                    dGoldTax = decimal.Round(dTotCValue * (dTaxPerc / 100), 2, MidpointRounding.AwayFromZero);// Convert.ToString(dGVal * (dTaxPerc / 100));
                                                else
                                                    dGoldTax = 0;

                                                item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));

                                                //decimal dPrice = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));

                                                //if (isExchange)
                                                //    item.Price = dPrice * dExchPct / 100;
                                                //else if (isCashBack)
                                                //    item.Price = dPrice * dCashBackPct / 100;
                                                //else if (isConToAdv)
                                                //    item.Price = dPrice * dConvToAdvPct / 100;

                                                decimal dDiscPct = 0m;
                                                dDiscPct = (item.LineDiscount) / (item.Price * item.Quantity) * 100;
                                                foreach (var itemD in item.DiscountLines)
                                                {
                                                    itemD.Percentage = dDiscPct;
                                                }

                                                item.PartnerData.NewTaxPerc = 0;
                                            }
                                            else
                                            {
                                                // item.PartnerData.NewTaxAmt = Convert.ToDecimal(dr["TAXAMOUNT"]);
                                                item.PartnerData.NewTaxPerc = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) * Math.Abs(Convert.ToDecimal(dr["TAXAMOUNT"])) / 100), 15, MidpointRounding.AwayFromZero))));
                                                SaleTrans.PartnerData.CashReturnWithOGTax = "1";//SaleTrans=current retail transaction

                                                item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));

                                                //decimal dPrice = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));

                                                //if (isExchange)
                                                //    item.Price = dPrice * dExchPct / 100;
                                                //else if (isCashBack)
                                                //    item.Price = dPrice * dCashBackPct / 100;
                                                //else if (isConToAdv)
                                                //    item.Price = dPrice * dConvToAdvPct / 100;

                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (IsBulkItem(item.ItemId))
                                            item.PartnerData.BulkItem = 1;
                                        else
                                            item.PartnerData.BulkItem = 0;

                                        item.PartnerData.NewTaxPerc = 0;
                                    }

                                    if (isExchange)
                                        item.PartnerData.TransReturnPayMode = 1;
                                    else if (isCashBack)
                                    {
                                        item.PartnerData.TransReturnPayMode = 2;
                                        item.HSNCode = "";// for SR buyback/cash back GST=0 for applo added on 190719 RH
                                    }
                                    else if (isConToAdv)
                                        item.PartnerData.TransReturnPayMode = 3;

                                    #region value assign into dynamics object
                                    item.PartnerData.ReturnReceiptId = retailTransaction.ReceiptId;
                                    item.PartnerData.SpecialDiscInfo = sSplReturn;

                                    item.PartnerData.Pieces = Convert.ToString(dr["PIECES"]);
                                    item.PartnerData.Quantity = Convert.ToString(dr["QUANTITY"]);
                                    item.PartnerData.Rate = Convert.ToString(dr["RATE"]);
                                    item.PartnerData.RateType = Convert.ToString(dr["RATETYPE"]);
                                    item.PartnerData.MakingRate = Convert.ToString(dr["MAKINGRATE"]); //
                                    item.PartnerData.MakingType = Convert.ToString(dr["MAKINGTYPE"]); //
                                    item.PartnerData.Amount = Convert.ToString(dr["AMOUNT"]);
                                    item.PartnerData.MakingDisc = "0";// Convert.ToString(dr["MAKINGDISCOUNT"]); //
                                    item.PartnerData.MakingAmount = Convert.ToString(Convert.ToDecimal(dr["MAKINGAMOUNT"]) - dExtraCalculatedGoldTaxAmt);// dExtraCalculatedGoldTaxAmt is added on 160818
                                    item.PartnerData.TotalAmount = Convert.ToString(Convert.ToDecimal(dr["TOTALAMOUNT"]) - dExtraCalculatedGoldTaxAmt);// dExtraCalculatedGoldTaxAmt is added on 160818
                                    item.PartnerData.TotalWeight = Convert.ToString(dr["TOTALWEIGHT"]);
                                    item.PartnerData.LossPct = Convert.ToString(dr["LOSSPERCENTAGE"]);
                                    item.PartnerData.LossWeight = Convert.ToString(dr["LOSSWEIGHT"]);
                                    item.PartnerData.ExpectedQuantity = Convert.ToString(dr["EXPECTEDQUANTITY"]);
                                    item.PartnerData.TransactionType = Convert.ToString(dr["TRANSACTIONTYPE"]);
                                    // item.PartnerData.OChecked = Convert.ToString(dr["OWN"]);
                                    item.PartnerData.OChecked = false;

                                    if (Convert.ToInt32(dr["IsIngredient"]) == 1)
                                        item.PartnerData.Ingredients = Convert.ToString(dr["SKUDETAILS"]); //
                                    else
                                        item.PartnerData.Ingredients = "";
                                    item.PartnerData.OrderNum = Convert.ToString(dr["ORDERNUM"]);
                                    item.PartnerData.OrderLineNum = Convert.ToString(dr["ORDERLINENUM"]);
                                    item.PartnerData.CustNo = Convert.ToString(dr["CUSTNO"]);
                                    item.PartnerData.SampleReturn = false;

                                    item.PartnerData.WastageType = Convert.ToString(dr["WastageType"]);
                                    item.PartnerData.WastageQty = Convert.ToString(dr["WastageQty"]);
                                    item.PartnerData.WastageAmount = Convert.ToString(dr["WastageAmount"]);
                                    item.PartnerData.WastagePercentage = Convert.ToString(dr["WastagePercentage"]);
                                    item.PartnerData.WastageRate = Convert.ToString(dr["WastageRate"]);

                                    item.PartnerData.MakingDiscountType = "0";// Convert.ToString(dr["MAKINGDISCTYPE"]);
                                    item.PartnerData.MakingTotalDiscount = "0";// Convert.ToString(dr["MakingDiscountAmount"]);
                                    item.PartnerData.ConfigId = Convert.ToString(dr["CONFIGID"]);

                                    item.PartnerData.Purity = "0";
                                    item.PartnerData.GROSSWT = "0";
                                    item.PartnerData.GROSSUNIT = string.Empty;
                                    item.PartnerData.DMDWT = "0";
                                    item.PartnerData.DMDPCS = "0";
                                    item.PartnerData.DMDUNIT = string.Empty;
                                    item.PartnerData.DMDAMOUNT = "0";
                                    item.PartnerData.STONEWT = "0";
                                    item.PartnerData.STONEPCS = "0";
                                    item.PartnerData.STONEUNIT = string.Empty;
                                    item.PartnerData.STONEAMOUNT = "0";
                                    item.PartnerData.NETWT = "0";
                                    item.PartnerData.NETRATE = "0";
                                    item.PartnerData.NETUNIT = string.Empty;
                                    item.PartnerData.NETPURITY = "0";
                                    item.PartnerData.NETAMOUNT = "0";
                                    item.PartnerData.OGREFINVOICENO = string.Empty;
                                    item.PartnerData.ServiceItemCashAdjustmentTransactionID = "";
                                    item.PartnerData.ACTMKRATE = Convert.ToString(dr["MAKINGRATE"]);
                                    //changed on 210617 req by S.Sharma
                                    item.PartnerData.ACTTOTAMT = Convert.ToString(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])) + Math.Abs(Convert.ToDecimal(dr["MAKINGAMOUNT"])) + Math.Abs(dExtraCalculatedGoldTaxAmt));// dExtraCalculatedGoldTaxAmt is added on 160818;// Convert.ToString(dr["TOTALAMOUNT"]);// Convert.ToString(dr["ACTTOTAMT"]);
                                    item.PartnerData.CHANGEDTOTAMT = Convert.ToString(dr["CHANGEDTOTAMT"]);
                                    item.PartnerData.LINEDISC = Convert.ToString(dr["LINEDISC"]);
                                    item.PartnerData.TRANSFERCOSTPRICE = Convert.ToString(dr["TRANSFERCOSTPRICE"]);
                                    //ADDED ON 20122017
                                    item.PartnerData.LINEGOLDTAX = Convert.ToString(dr["GOLDTAXVALUE"]);
                                    item.PartnerData.NewTaxAmt = Convert.ToDecimal(dr["TAXAMOUNT"]);
                                    item.PartnerData.WTDIFFDISCQTY = Convert.ToString(dr["WTDIFFDISCQTY"]);
                                    item.PartnerData.WTDIFFDISCAMT = Convert.ToString(dr["WTDIFFDISCAMT"]);

                                    if (!string.IsNullOrEmpty(sSalesLoaltyType))
                                    {
                                        item.PartnerData.LOYALTYTYPECODE = sSalesLoaltyType;
                                        item.PartnerData.LoyaltyProvider = sSalesLoaltyTypeProv;
                                        item.PartnerData.LOYALTYCARDNO = sSalesLoaltyCardNo;
                                        item.PartnerData.LOYALTYASKINGDONE = true;
                                    }
                                    else
                                    {
                                        item.PartnerData.LOYALTYTYPECODE = "";
                                        item.PartnerData.LoyaltyProvider = "";
                                        item.PartnerData.LOYALTYCARDNO = "";
                                        item.PartnerData.LOYALTYASKINGDONE = false;
                                    }
                                    item.PartnerData.PurityReading1 = Convert.ToDecimal(dr["PurityReading1"]);
                                    item.PartnerData.PurityReading2 = Convert.ToDecimal(dr["PurityReading2"]);
                                    item.PartnerData.PurityReading3 = Convert.ToDecimal(dr["PurityReading3"]);
                                    item.PartnerData.PURITYPERSON = Convert.ToString(dr["PURITYPERSON"]);
                                    item.PartnerData.PURITYPERSONNAME = Convert.ToString(dr["PURITYPERSONNAME"]);
                                    item.PartnerData.SKUAgingDiscType = Convert.ToInt16(dr["SKUAgingDiscType"]);
                                    item.PartnerData.SKUAgingDiscAmt = Convert.ToDecimal(dr["SKUAgingDiscAmt"]);
                                    item.PartnerData.TierDiscType = Convert.ToInt16(dr["TierDiscType"]);
                                    item.PartnerData.TierDiscAmt = Convert.ToDecimal(dr["TierDiscAmt"]);
                                    item.PartnerData.NimDiscLine = 0;
                                    if (Convert.ToInt16(dr["MRPOrMkDiscType"]) == (int)MRPOrMkDiscType.Making)
                                    {
                                        item.PartnerData.NimMakingDiscType = true;//
                                        item.PartnerData.NimMakingDisc = Convert.ToDecimal(dr["MRPOrMkDiscAmt"]); //MRPOrMkDiscAmt
                                        item.PartnerData.NimMRPDiscType = false;
                                        item.PartnerData.NimMRPDisc = 0; //MRPOrMkDiscAmt
                                    }
                                    else if (Convert.ToInt16(dr["MRPOrMkDiscType"]) == (int)MRPOrMkDiscType.MRP)
                                    {
                                        item.PartnerData.NimMRPDiscType = true;
                                        item.PartnerData.NimMRPDisc = Convert.ToDecimal(dr["MRPOrMkDiscAmt"]); //MRPOrMkDiscAmt
                                        item.PartnerData.NimMakingDiscType = false;//
                                        item.PartnerData.NimMakingDisc = 0; //MRPOrMkDiscAmt
                                    }
                                    else
                                    {
                                        item.PartnerData.NimMakingDiscType = false;
                                        item.PartnerData.NimMakingDisc = 0;
                                        item.PartnerData.NimMRPDisc = 0;
                                        item.PartnerData.NimMRPDiscType = false;
                                    }

                                    item.PartnerData.OGPSTONEPCS = Convert.ToDecimal(dr["OGPSTONEPCS"]);
                                    item.PartnerData.OGPSTONEWT = Convert.ToDecimal(dr["OGPSTONEWT"]);
                                    item.PartnerData.OGPSTONEUNIT = Convert.ToString(dr["OGPSTONEUNIT"]);
                                    item.PartnerData.OGPSTONERATE = Convert.ToDecimal(dr["OGPSTONERATE"]);
                                    item.PartnerData.OGPSTONEAMOUNT = Convert.ToDecimal(dr["OGPSTONEAMOUNT"]);

                                    if (Convert.ToInt16(dr["MRPORMKPROMODISCTYPE"]) == (int)MRPOrMkDiscType.Making)
                                    {
                                        //item.PartnerData.NIMPROMOCODE = Convert.ToDecimal(dr["MRPORMKPROMODISCCODE"]);
                                        item.PartnerData.NIMPROMOCODE = Convert.ToString(dr["MRPORMKPROMODISCCODE"]);
                                        item.PartnerData.NimPromoMakingDiscType = true;
                                        item.PartnerData.NimPromoMakingDisc = Convert.ToDecimal(dr["MRPORMKPROMODISCAMT"]);
                                        item.PartnerData.NimPromoMRPDiscType = false;
                                        item.PartnerData.NimPromoMRPDisc = 0;
                                    }
                                    else if (Convert.ToInt16(dr["MRPORMKPROMODISCTYPE"]) == (int)MRPOrMkDiscType.MRP)
                                    {
                                        //item.PartnerData.NIMPROMOCODE = Convert.ToDecimal(dr["MRPORMKPROMODISCCODE"]);
                                        item.PartnerData.NIMPROMOCODE = Convert.ToString(dr["MRPORMKPROMODISCCODE"]);
                                        item.PartnerData.NimPromoMRPDiscType = true;
                                        item.PartnerData.NimPromoMRPDisc = Convert.ToDecimal(dr["MRPORMKPROMODISCAMT"]);
                                        item.PartnerData.NimPromoMakingDiscType = false;
                                        item.PartnerData.NimPromoMakingDisc = 0;
                                    }
                                    else
                                    {
                                        item.PartnerData.NIMPROMOCODE = "";
                                        item.PartnerData.NimPromoMakingDiscType = false;
                                        item.PartnerData.NimPromoMakingDisc = 0;
                                        item.PartnerData.NimPromoMRPDisc = 0;
                                        item.PartnerData.NimPromoMRPDiscType = false;
                                    }

                                    item.PartnerData.MakStnDiaDiscType = 0;
                                    item.PartnerData.NimMakingDiscount = 0;
                                    item.PartnerData.NimStoneDiscount = 0;
                                    item.PartnerData.NimDiamondDiscount = 0;

                                    item.PartnerData.MakStnDiaDisc = false;
                                    item.PartnerData.MakStnDiaDiscDone = false;

                                    item.PartnerData.AdvAgreemetDisc = false;
                                    item.PartnerData.AdvAgreemetDiscDone = false;
                                    item.PartnerData.SaleAdjustmentAdvanceDepositDate = "";

                                    break;
                                    #endregion value assign into dynamics object
                                }
                            }
                            #endregion
                            //}
                            //else
                            //{
                            //    preTriggerResult.ContinueOperation = false;
                            //    preTriggerResult.MessageId = 999997;
                            //    return;
                            //}
                        }
                    }
                }
                #endregion

                #endregion


                #region Bellow 7 days/ param days 100 % return
                DateTime dtTransDate1 = Convert.ToDateTime(dtRec.Rows[0]["TRANSDATE"]);
                int iFullExchngValidDay1 = GetFullExchngValidDay(posTransaction.StoreId);
                if ((DateTime.Now - dtTransDate1).TotalDays <= iFullExchngValidDay1)
                {
                    #region Sale Return Days Validity
                    foreach (var item in lineitem)
                    {
                        string sTransactionId = originalTransaction.TransactionId;
                        string sStoreId = originalTransaction.StoreId;
                        string sTerminalId = originalTransaction.TerminalId;

                        int iSalesReturnValidDay = 0;
                        decimal dCashBackPct = 0;
                        decimal dExchPct = 0;
                        decimal dConvToAdvPct = 0m;

                        GetSalesReturnPolicy(item.ItemId, ref iSalesReturnValidDay, ref dCashBackPct, ref dConvToAdvPct, ref dExchPct);

                        string sSMName = "";
                        if (isMRP(item.ItemId))
                            isFullSalesReturn = true;
                        else
                            isFullSalesReturn = false;

                        //string sSm = GetSalesPersonAtSalesTime(sStoreId, sTerminalId, sTransactionId, Convert.ToString(item.LineId), ref sSMName);
                        //item.SalesPersonId = sSm;
                        //item.SalespersonName = sSMName;//blocked req by Prashant on 28/08/2017
                        iMetalType = getMetalType(item.ItemId);

                        //added by prashant on 21.01.2018 as instructed by Ripan
                        if (iMetalType == (int)Enums.EnumClass.MetalType.Jewellery)
                        {
                            item.PartnerData.GrossWt = "0";
                        }// ended by prashant on 21.01.2018 as instructed by Ripan

                        #region Auto Fetch Sales man
                        /*for (int i = 0; i < 1000; i++)// for malabar dubai changes only one sales persons will be added on transaction //20/06/2016
                    {
                        if (!string.IsNullOrEmpty(item.SalesPersonId))
                            break;
                        else
                        {
                            if (retailTransaction != null)
                            {
                                if (retailTransaction.SaleItems.Count > 0)
                                {
                                    foreach (SaleLineItem salsItems in retailTransaction.SaleItems)
                                    {
                                        //if(saleLine.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                        //{
                                        if (!string.IsNullOrEmpty(salsItems.SalesPersonId))
                                        {
                                            item.SalesPersonId = salsItems.SalesPersonId;
                                            item.SalespersonName = salsItems.SalespersonName;
                                            break;
                                        }
                                        else
                                        {
                                            LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                            dialog6.ShowDialog();
                                            if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                            {
                                                item.SalesPersonId = dialog6.SelectedEmployeeId;
                                                item.SalespersonName = dialog6.SelectEmployeeName;
                                                break;
                                            }
                                        }
                                        // }
                                    }
                                }
                                else
                                {
                                    LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                    dialog6.ShowDialog();
                                    if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                                    {
                                        item.SalesPersonId = dialog6.SelectedEmployeeId;
                                        item.SalespersonName = dialog6.SelectEmployeeName;
                                        break;
                                    }
                                }
                            }
                        }
                    }*/
                        #endregion

                        foreach (DataRow dr in dtRec.Rows)
                        {
                            #region Tax Blank Added on 19/01/2018
                            DateTime transDate = Convert.ToDateTime(dtRec.Rows[0]["TRANSDATE"]);
                            DateTime taxCutOffDate = getTaxCutOffDate();
                            if (transDate < taxCutOffDate)
                            {
                                item.SalesTaxGroupId = "";
                                item.SalesTaxGroupIdOriginal = "";
                                item.TaxGroupId = "";
                                item.TaxGroupIdOriginal = "";
                            }
                            #endregion

                            #region Start : UCP item return issue
                            SqlConnection conn = new SqlConnection();
                            if (application != null)
                                conn = application.Settings.Database.Connection; //Settings.Database.Connection;
                            else
                                conn = ApplicationSettings.Database.LocalConnection;

                            bool isMrp = false;
                            int cntdisc = 0;
                            isMrp = isUCP(item.ItemId, conn);
                            if (isMrp)
                            {
                                if (Convert.ToDecimal(item.LineId) == Convert.ToDecimal(dr["LINENUM"]))
                                {
                                    cntdisc = 0;
                                    foreach (DiscountItem discLine in item.DiscountLines)
                                    {
                                        if (cntdisc == 0)
                                        {
                                            discLine.Percentage = ((item.LineDiscount / item.Price) * 100) + item.TotalPctDiscount;
                                            cntdisc++;
                                        }
                                        else
                                        {
                                            discLine.Percentage = 0;
                                        }

                                    }
                                    retailTransaction.LineDiscount = 0;
                                    item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero)))); // (Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])));


                                    if (isExchange)
                                        item.PartnerData.RETURNTYPE = "Exchange";
                                    else if (isCashBack)
                                        item.PartnerData.RETURNTYPE = "CashBack";
                                    else if (isConToAdv)
                                        item.PartnerData.RETURNTYPE = "ConvToAdv";

                                    item.PartnerData.CalExchangeQty = 0;
                                }
                                item.PartnerData.isMRP = true;
                            }

                            #endregion End : UCP item return issue
                            else
                            {
                                if (isFullSalesReturn == false && isExchange)
                                {
                                    #region [Exchange / Buy Back]
                                    decimal dOldHdrRate = 0m;

                                    DateTime dtDate = Convert.ToDateTime(dtRec.Rows[0]["TRANSDATE"]);

                                    if (Convert.ToInt32(dr["IsIngredient"]) == 1)
                                    {
                                        dCValue = 0;
                                        DataSet dsIngredients = new DataSet();
                                        StringReader reader = new StringReader(Convert.ToString(dr["SKUDETAILS"]));
                                        dsIngredients.ReadXml(reader);

                                        if (dsIngredients != null && dsIngredients.Tables[0].Rows.Count > 0)
                                        {
                                            foreach (DataRow drIng in dsIngredients.Tables[0].Rows)
                                            {
                                                //dIngrdRate = 0m;
                                                string sSelfStoreId = ApplicationSettings.Terminal.StoreId;

                                                if (Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.Gold)
                                                {
                                                    if (sGetDefaultConfigId == Convert.ToString(drIng["ConfigID"]))
                                                    {
                                                        dOldHdrRate = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["RATE"])), 2, MidpointRounding.AwayFromZero))));

                                                        dCValue += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["CValue"])), 2, MidpointRounding.AwayFromZero))));
                                                    }
                                                    if (sGetDefaultConfigId != Convert.ToString(drIng["ConfigID"]))
                                                    {
                                                        dCValueForOtherConfig = 0;
                                                        dCValueForOtherConfig += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["CValue"])), 2, MidpointRounding.AwayFromZero))));
                                                    }
                                                    //dOldGoldQty += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["QTY"])), 3, MidpointRounding.AwayFromZero))));
                                                }
                                                else if ((Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.GiftVoucher)) // added on  23/08/2016 for Malabar Dubai
                                                {
                                                    preTriggerResult.ContinueOperation = false;
                                                    MessageBox.Show("Gift voucher can not return.");
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (iMetalType == (int)Enums.EnumClass.MetalType.Gold)
                                        {
                                            if (sGetDefaultConfigId == Convert.ToString(dr["ConfigID"]))
                                            {
                                                dCValue = 0;
                                                dOldHdrRate = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["RATE"])), 2, MidpointRounding.AwayFromZero))));

                                                dCValue += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                            }
                                            if (sGetDefaultConfigId != Convert.ToString(dr["ConfigID"]))
                                            {
                                                dCValueForOtherConfig = 0;
                                                dCValueForOtherConfig += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                            }
                                            //dOldGoldQty += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["QTY"])), 3, MidpointRounding.AwayFromZero))));
                                        }
                                        if ((iMetalType == (int)Enums.EnumClass.MetalType.GiftVoucher))
                                        {
                                            preTriggerResult.ContinueOperation = false;
                                            MessageBox.Show("Gift voucher can not return.");
                                            return;
                                        }
                                    }


                                    #region old commented on 12/01/2018 for new exchange logic
                                    /* if (Convert.ToInt32(dr["IsIngredient"]) == 1)
                                {
                                    DataSet dsIngredients = new DataSet();
                                    StringReader reader = new StringReader(Convert.ToString(dr["SKUDETAILS"]));
                                    dsIngredients.ReadXml(reader);

                                    if (dsIngredients != null && dsIngredients.Tables[0].Rows.Count > 0)
                                    {
                                        foreach (DataRow drIng in dsIngredients.Tables[0].Rows)
                                        {
                                            //dIngrdRate = 0m;
                                            string sSelfStoreId = ApplicationSettings.Terminal.StoreId;

                                            if (Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.Gold)
                                            {
                                                #region
                                                if (isExchange)
                                                {
                                                    int iRateType = (int)Enums.EnumClass.RateType.Exchange;

                                                    dIngrdRate = getGoldRate(Convert.ToString(drIng["ItemID"]), Convert.ToString(drIng["ConfigID"]),
                                                                            3, sSelfStoreId);// S.Sharma -- told to cancel "iRateType" against 3 //originalTransaction.StoreId
                                                }
                                                else
                                                {
                                                    int iRateType = (int)Enums.EnumClass.RateType.OGP;

                                                    dIngrdRate = getGoldRate(Convert.ToString(drIng["ItemID"]), Convert.ToString(drIng["ConfigID"]),
                                                                            iRateType, sSelfStoreId);// originalTransaction.StoreId
                                                }

                                                dNewIngrdCValue += dIngrdRate * Convert.ToDecimal(drIng["QTY"]);
                                                dIngrCValue += Convert.ToDecimal(drIng["CValue"]);

                                                drIng["CValue"] = dIngrdRate * Convert.ToDecimal(drIng["QTY"]);// dNewIngrdCValue;
                                                drIng["Rate"] = dIngrdRate;
                                                dsIngredients.Tables[0].AcceptChanges();
                                                dsIngredients.Tables[0].TableName = "Ingredients";
                                                MemoryStream mstr = new MemoryStream();
                                                dsIngredients.Tables[0].WriteXml(mstr, true);
                                                mstr.Seek(0, SeekOrigin.Begin);
                                                StreamReader sr = new StreamReader(mstr);
                                                string sXML = string.Empty;
                                                sXML = sr.ReadToEnd();
                                                dr["SKUDETAILS"] = sXML;
                                                dtRec.AcceptChanges();
                                                #endregion
                                            }
                                            else if ((Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.LooseDmd)
                                                     || (Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.Stone)) // add stone
                                            {
                                                #region

                                                decimal dExchangePercent = 0m;
                                                decimal dBuyBackPercent = 0m;
                                                decimal dStoneWtRange = 0m;

                                                int iCalcType = 0;
                                                decimal dDeductionAmt = 0;

                                                if (Convert.ToDecimal(drIng["PCS"]) > 0)
                                                {
                                                    dStoneWtRange = Convert.ToDecimal(drIng["QTY"]) / Convert.ToDecimal(drIng["PCS"]);
                                                }
                                                else
                                                {
                                                    dStoneWtRange = Convert.ToDecimal(drIng["QTY"]);
                                                }

                                                if (!string.IsNullOrEmpty(Convert.ToString(drIng["IngrdDiscTotAmt"])))
                                                {
                                                    dStoneDiscAmt = Math.Abs(Convert.ToDecimal(drIng["IngrdDiscTotAmt"]));
                                                }

                                                dIngrdRate = GetIngredientInfo(ref dExchangePercent, ref dBuyBackPercent, ref iCalcType,
                                                                                originalTransaction.StoreId, Convert.ToString(drIng["ItemID"]),
                                                                                Convert.ToString(drIng["InventSizeID"]), Convert.ToString(drIng["InventColorID"]),
                                                                                dStoneWtRange, dtDate);
                                                if (isExchange)
                                                {
                                                    dDeductionAmt = (dIngrdRate * dExchangePercent) / 100;
                                                }
                                                else
                                                {
                                                    dDeductionAmt = (dIngrdRate * dBuyBackPercent) / 100;
                                                }
                                                // -- calc type
                                                // new cvalue - total deduction amount -- dStoneDiscAmt
                                                if (iCalcType == (int)Enums.EnumClass.CalcType.Weight)
                                                {
                                                    dIngrStoneValue = (dIngrdRate * Convert.ToDecimal(drIng["QTY"])) - (dDeductionAmt * Convert.ToDecimal(drIng["QTY"])) - dStoneDiscAmt;
                                                }
                                                else if (iCalcType == (int)Enums.EnumClass.CalcType.Pieces)
                                                {
                                                    dIngrStoneValue = (dIngrdRate * Convert.ToDecimal(drIng["PCS"])) - (dDeductionAmt * Convert.ToDecimal(drIng["PCS"])) - dStoneDiscAmt;
                                                }
                                                else
                                                {
                                                    dIngrStoneValue = dIngrdRate - dDeductionAmt - dStoneDiscAmt;
                                                }

                                                drIng["CValue"] = dIngrStoneValue;
                                                drIng["Rate"] = dIngrdRate;

                                                dsIngredients.Tables[0].AcceptChanges();

                                                dsIngredients.Tables[0].TableName = "Ingredients";
                                                MemoryStream mstr = new MemoryStream();
                                                dsIngredients.Tables[0].WriteXml(mstr, true);
                                                mstr.Seek(0, SeekOrigin.Begin);
                                                StreamReader sr = new StreamReader(mstr);
                                                string sXML = string.Empty;
                                                sXML = sr.ReadToEnd();
                                                dr["SKUDETAILS"] = sXML;
                                                dtRec.AcceptChanges();
                                                #endregion
                                            }
                                            else if ((Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.GiftVoucher)) // added on  23/08/2016 for Malabar Dubai
                                            {
                                                preTriggerResult.ContinueOperation = false;
                                                MessageBox.Show("Gift voucher can not return.");
                                                return;
                                            }
                                        }

                                        //Start added on 070617

                                        if (Convert.ToInt16(dr["RATETYPE"]) != (int)RateType.Tot)// added for old systems Sales return on 17/07/2017
                                        {
                                            dr["RATETYPE"] = (int)RateType.Tot;
                                            dOldHdrRate = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])), 2, MidpointRounding.AwayFromZero))));// Math.Abs(Convert.ToDecimal(dr["AMOUNT"]));

                                            dr["RATE"] = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["AMOUNT"])) - dIngrCValue) + dNewIngrdCValue - dStoneDiscAmt, 2, MidpointRounding.AwayFromZero))));
                                            dr["AMOUNT"] = Convert.ToDecimal((Convert.ToString(decimal.Round(Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero))));// Convert.ToDecimal(dr["RATE"]);
                                        }
                                        else
                                        {

                                            dOldHdrRate = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["RATE"])), 2, MidpointRounding.AwayFromZero))));// Math.Abs(Convert.ToDecimal(dr["RATE"])); // Tot -- for ingredient  -- > Amount = Rate

                                            dr["RATE"] = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["RATE"])) - dIngrCValue) + dNewIngrdCValue - dStoneDiscAmt, 2, MidpointRounding.AwayFromZero))));
                                            dr["AMOUNT"] = Convert.ToDecimal((Convert.ToString(decimal.Round(Convert.ToDecimal(dr["RATE"]), 2, MidpointRounding.AwayFromZero))));// Convert.ToDecimal(dr["RATE"]);
                                        }
                                        dr["TOTALAMOUNT"] = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) - dOldHdrRate) + Convert.ToDecimal(dr["AMOUNT"]) + Math.Abs(Convert.ToDecimal(dr["MAKINGAMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                        dtRec.AcceptChanges();

                                        //end
                                        dr["TOTALAMOUNT"] = Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) -
                                                                Math.Abs(Convert.ToDecimal(dr["MAKINGAMOUNT"])) -
                                                                Math.Abs(Convert.ToDecimal(dr["WastageAmount"]));

                                        dr["MAKINGDISCOUNT"] = 0;
                                        dr["MAKINGDISCTYPE"] = 0;
                                        dr["MakingDiscountAmount"] = 0;

                                        dtRec.AcceptChanges();
                                    }
                                }
                                else
                                {

                                    int iMetalType = getMetalType(item.ItemId);
                                    if ((iMetalType == (int)Enums.EnumClass.MetalType.LooseDmd)
                                        || (iMetalType == (int)Enums.EnumClass.MetalType.Stone))
                                    {
                                        #region

                                        decimal dExchangePercent = 0m;
                                        decimal dBuyBackPercent = 0m;
                                        decimal dStoneWtRange = 0m;

                                        decimal dStoneRate = 0m;
                                        decimal dDeductionAMt = 0m;

                                        int iCalcType = 0;
                                        decimal dNewCValue = 0m;

                                        if (Convert.ToDecimal(dr["PCS"]) > 0)
                                        {
                                            dStoneWtRange = Convert.ToDecimal(dr["QTY"]) / Convert.ToDecimal(dr["PCS"]);
                                        }
                                        else
                                        {
                                            dStoneWtRange = Convert.ToDecimal(dr["QTY"]);
                                        }

                                        dStoneRate = GetIngredientInfo(ref dExchangePercent, ref dBuyBackPercent, ref iCalcType,
                                                                        originalTransaction.StoreId,
                                                                       item.ItemId,
                                                                       item.Dimension.SizeId,
                                                                       item.Dimension.ColorId,
                                                                       dStoneWtRange, dtDate);
                                        if (isExchange)
                                        {
                                            dDeductionAMt = ((dExchangePercent * dStoneRate) / 100);
                                        }
                                        else
                                        {
                                            dDeductionAMt = ((dBuyBackPercent * dStoneRate) / 100);
                                        }

                                        if (iCalcType == (int)Enums.EnumClass.CalcType.Weight)
                                        {
                                            dNewCValue = (dStoneRate * Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))) - (dDeductionAMt * Math.Abs(Convert.ToDecimal(dr["QUANTITY"])));

                                        }
                                        else if (iCalcType == (int)Enums.EnumClass.CalcType.Pieces)
                                        {
                                            dNewCValue = (dStoneRate * Math.Abs(Convert.ToDecimal(dr["PIECES"]))) - (dDeductionAMt * Math.Abs(Convert.ToDecimal(dr["PIECES"])));
                                        }
                                        else
                                        {
                                            dNewCValue = dStoneRate - dDeductionAMt;
                                        }

                                        dr["RATE"] = Convert.ToDecimal((Convert.ToString(decimal.Round(dStoneRate, 2, MidpointRounding.AwayFromZero))));// dStoneRate;
                                        dr["RATETYPE"] = iCalcType;
                                        dr["AMOUNT"] = Convert.ToDecimal((Convert.ToString(decimal.Round(dNewCValue, 2, MidpointRounding.AwayFromZero)))); //dNewCValue;
                                        dr["TOTALAMOUNT"] = Convert.ToDecimal((Convert.ToString(decimal.Round(dNewCValue, 2, MidpointRounding.AwayFromZero))));

                                        dtRec.AcceptChanges();

                                        #endregion
                                    }
                                    else if ((iMetalType == (int)Enums.EnumClass.MetalType.GiftVoucher)) // added on  23/08/2016 for Malabar Dubai
                                    {
                                        preTriggerResult.ContinueOperation = false;
                                        MessageBox.Show("Gift voucher can not return.");
                                        return;
                                    }
                                }*/
                                    #endregion

                                    #endregion
                                    if (dOldHdrRate > 0)
                                    {
                                        dTotCValue = 0;
                                        // dTotExchReturnQty += Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / dOldHdrRate), 3, MidpointRounding.AwayFromZero))));
                                        //dTotExchReturnQty += Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(item.NetAmountWithNoTax)) / dOldHdrRate), 3, MidpointRounding.AwayFromZero))));
                                        //item.PartnerData.CalExchangeQty = dTotExchReturnQty;
                                        //dTotExchReturnQty = 0m;
                                        dTotCValue += dCValue;
                                        // dTotCValue += dCValue;
                                    }
                                    else if (dCValueForOtherConfig != 0 && dOldHdrRate == 0)
                                    {
                                        dTotCValue = 0;
                                        dTotCValue += dCValueForOtherConfig;
                                    }
                                    else
                                        item.PartnerData.CalExchangeQty = 0m;

                                    if (isExchange || isConToAdv)
                                    {
                                        decimal dTaxPerc = getItemTaxPercentage(item.ItemId);
                                        decimal dGoldTax = 0m;

                                        if (dTaxPerc > 0 && dTotCValue > 0)
                                            dGoldTax = decimal.Round(dTotCValue * (dTaxPerc / 100), 2, MidpointRounding.AwayFromZero);// Convert.ToString(dGVal * (dTaxPerc / 100));
                                        else
                                            dGoldTax = 0;

                                        //

                                        item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));
                                        //dTotCValue
                                        decimal dDiscPct = 0m;
                                        if (item.Price > 0)
                                        {
                                            dDiscPct = (item.LineDiscount) / (item.Price * item.Quantity) * 100;
                                        }
                                        // DISCAMOUNT/(203.28*QTY)*100
                                        foreach (var itemD in item.DiscountLines)
                                        {
                                            itemD.Percentage = dDiscPct;
                                        }


                                        if (dOldHdrRate > 0)
                                        {
                                            // dTotExchReturnQty += Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / dOldHdrRate), 3, MidpointRounding.AwayFromZero))));
                                            decimal dTax = ((dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) * dTaxPerc) / (100 + dTaxPerc);
                                            dExtraCalculatedGoldTaxAmt = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"])))), 2, MidpointRounding.AwayFromZero)))); ;

                                            dTotExchReturnQty += Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(item.NetAmountWithNoTax) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"])) - dTax) / dOldHdrRate), 3, MidpointRounding.AwayFromZero))));
                                            item.PartnerData.CalExchangeQty = dTotExchReturnQty;
                                            dTotExchReturnQty = 0m;
                                        }
                                        else
                                            item.PartnerData.CalExchangeQty = 0m;

                                        item.PartnerData.NewTaxPerc = 0;
                                    }
                                    else
                                    {
                                        if (dOldHdrRate > 0)
                                        {
                                            // dTotExchReturnQty += Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / dOldHdrRate), 3, MidpointRounding.AwayFromZero))));
                                            dTotExchReturnQty += Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(item.NetAmountWithNoTax)) / dOldHdrRate), 3, MidpointRounding.AwayFromZero))));
                                            item.PartnerData.CalExchangeQty = dTotExchReturnQty;
                                            dTotExchReturnQty = 0m;
                                        }
                                        else
                                            item.PartnerData.CalExchangeQty = 0m;

                                        //item.PartnerData.NewTaxAmt = Convert.ToDecimal(dr["TAXAMOUNT"]);
                                        item.PartnerData.NewTaxPerc = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) * Math.Abs(Convert.ToDecimal(dr["TAXAMOUNT"])) / 100), 15, MidpointRounding.AwayFromZero))));
                                        SaleTrans.PartnerData.CashReturnWithOGTax = "1";//SaleTrans=current retail transaction
                                        item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                    }
                                }

                                #region convert to adv on 210618

                                if (isFullSalesReturn == false && isConToAdv)
                                {
                                    #region [isConToAdv]
                                    decimal dOldHdrRate = 0m;

                                    DateTime dtDate = Convert.ToDateTime(dtRec.Rows[0]["TRANSDATE"]);

                                    if (Convert.ToInt32(dr["IsIngredient"]) == 1)
                                    {
                                        dCValue = 0;
                                        DataSet dsIngredients = new DataSet();
                                        StringReader reader = new StringReader(Convert.ToString(dr["SKUDETAILS"]));
                                        dsIngredients.ReadXml(reader);

                                        if (dsIngredients != null && dsIngredients.Tables[0].Rows.Count > 0)
                                        {
                                            foreach (DataRow drIng in dsIngredients.Tables[0].Rows)
                                            {
                                                //dIngrdRate = 0m;
                                                string sSelfStoreId = ApplicationSettings.Terminal.StoreId;

                                                if (Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.Gold)
                                                {
                                                    if (sGetDefaultConfigId == Convert.ToString(drIng["ConfigID"]))
                                                    {
                                                        dOldHdrRate = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["RATE"])), 2, MidpointRounding.AwayFromZero))));

                                                        dCValue += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["CValue"])), 2, MidpointRounding.AwayFromZero))));
                                                    }
                                                    if (sGetDefaultConfigId != Convert.ToString(drIng["ConfigID"]))
                                                    {
                                                        dCValueForOtherConfig = 0;
                                                        dCValueForOtherConfig += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["CValue"])), 2, MidpointRounding.AwayFromZero))));
                                                    }
                                                    //dOldGoldQty += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["QTY"])), 3, MidpointRounding.AwayFromZero))));
                                                }
                                                else if ((Convert.ToInt32(drIng["METALTYPE"]) == (int)Enums.EnumClass.MetalType.GiftVoucher)) // added on  23/08/2016 for Malabar Dubai
                                                {
                                                    preTriggerResult.ContinueOperation = false;
                                                    MessageBox.Show("Gift voucher can not return.");
                                                    return;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (iMetalType == (int)Enums.EnumClass.MetalType.Gold)
                                        {
                                            if (sGetDefaultConfigId == Convert.ToString(dr["ConfigID"]))
                                            {
                                                dCValue = 0;
                                                dOldHdrRate = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["RATE"])), 2, MidpointRounding.AwayFromZero))));

                                                dCValue += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                            }
                                            if (sGetDefaultConfigId != Convert.ToString(dr["ConfigID"]))
                                            {
                                                dCValueForOtherConfig = 0;
                                                dCValueForOtherConfig += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                            }
                                            //dOldGoldQty += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(drIng["QTY"])), 3, MidpointRounding.AwayFromZero))));
                                        }
                                        if ((iMetalType == (int)Enums.EnumClass.MetalType.GiftVoucher))
                                        {
                                            preTriggerResult.ContinueOperation = false;
                                            MessageBox.Show("Gift voucher can not return.");
                                            return;
                                        }
                                    }

                                    #endregion
                                    if (dOldHdrRate > 0)
                                    {
                                        dTotCValue = 0;
                                        dTotCValue += dCValue;
                                        // dTotCValue += dCValue;
                                    }
                                    else if (dCValueForOtherConfig != 0 && dOldHdrRate == 0)
                                    {
                                        dTotCValue = 0;
                                        dTotCValue += dCValueForOtherConfig;
                                    }


                                    if (isExchange || isConToAdv)
                                    {
                                        decimal dTaxPerc = getItemTaxPercentage(item.ItemId);
                                        decimal dGoldTax = 0m;

                                        if (dTaxPerc > 0 && dTotCValue > 0)
                                            dGoldTax = decimal.Round(dTotCValue * (dTaxPerc / 100), 2, MidpointRounding.AwayFromZero);// Convert.ToString(dGVal * (dTaxPerc / 100));
                                        else
                                            dGoldTax = 0;

                                        item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));
                                        // decimal dPrice = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));
                                        //dTotCValue

                                        //if (isExchange)
                                        //    item.Price = dPrice * dExchPct / 100;
                                        //else if (isCashBack)
                                        //    item.Price = dPrice * dCashBackPct / 100;
                                        //else if (isConToAdv)
                                        //    item.Price = dPrice * dConvToAdvPct / 100;

                                        decimal dDiscPct = 0m;
                                        dDiscPct = (item.LineDiscount) / (item.Price * item.Quantity) * 100;
                                        // DISCAMOUNT/(203.28*QTY)*100
                                        foreach (var itemD in item.DiscountLines)
                                        {
                                            itemD.Percentage = dDiscPct;
                                        }
                                        item.PartnerData.NewTaxPerc = 0;
                                    }
                                    else
                                    {
                                        // item.PartnerData.NewTaxAmt = Convert.ToDecimal(dr["TAXAMOUNT"]);
                                        item.PartnerData.NewTaxPerc = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) * Math.Abs(Convert.ToDecimal(dr["TAXAMOUNT"])) / 100), 15, MidpointRounding.AwayFromZero))));
                                        SaleTrans.PartnerData.CashReturnWithOGTax = "1";//SaleTrans=current retail transaction
                                        item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                    }
                                }
                                #endregion

                                if (isExchange)
                                    item.PartnerData.RETURNTYPE = "Exchange";
                                else if (isCashBack)
                                    item.PartnerData.RETURNTYPE = "CashBack";
                                else if (isConToAdv)
                                    item.PartnerData.RETURNTYPE = "ConvToAdv";

                                item.PartnerData.isMRP = false;
                            }
                            //  if (Convert.ToString(item.LineId) == Convert.ToString(Convert.ToInt16(dr["LINENUM"]))) 
                            if (Convert.ToDecimal(item.LineId) == Convert.ToDecimal(dr["LINENUM"]))
                            {
                                #region Old logic Commented on 12/01/2018
                                /*if (IsBulkItem(item.ItemId))
                            {
                                item.PartnerData.BulkItem = 1;

                                #region
                                string sSelfStoreId = ApplicationSettings.Terminal.StoreId;
                                decimal dBulkMetalRate = 0m;
                                decimal dTotAmtNew = 0m;
                                int iMetalType = getMetalType(item.ItemId);

                                if (iMetalType == (int)Enums.EnumClass.MetalType.Gold)
                                {
                                    if (isExchange)
                                    {
                                        dBulkMetalRate = getGoldRate(Convert.ToString(item.ItemId), Convert.ToString(dr["CONFIGID"]),
                                                                3, sSelfStoreId);// S.Sharma -- told to cancel "iRateType" against 3 //originalTransaction.StoreId


                                        dTotAmtNew = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["QUANTITY"])) * dBulkMetalRate + Math.Abs(Convert.ToDecimal(dr["MAKINGAMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                        dr["RATE"] = dBulkMetalRate;
                                        dr["AMOUNT"] = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["QUANTITY"])) * dBulkMetalRate, 2, MidpointRounding.AwayFromZero))));//added on 17082017
                                        dr["TOTALAMOUNT"] = dTotAmtNew;
                                        dtRec.AcceptChanges();

                                        item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(dTotAmtNew) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                    }
                                    else
                                    {
                                        item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                    }
                                }
                                else
                                {
                                    item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                }
                                #endregion
                            }
                            else
                            {
                                item.PartnerData.BulkItem = 0;
                                item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                            }*/
                                #endregion
                                if (!isMrp)
                                {
                                    decimal dBulkMetalRate = 0m;
                                    decimal dTotAmtNew = 0m;
                                    if (IsBulkItem(item.ItemId))
                                    {
                                        item.PartnerData.BulkItem = 1;

                                        #region
                                        string sSelfStoreId = ApplicationSettings.Terminal.StoreId;

                                        // int iMetalType = getMetalType(item.ItemId);

                                        if (iMetalType == (int)Enums.EnumClass.MetalType.Gold)
                                        {
                                            if (isExchange)
                                            {
                                                if (sGetDefaultConfigId == Convert.ToString(dr["ConfigID"]))
                                                {
                                                    dCValue = 0;
                                                    dBulkMetalRate = Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["RATE"])), 2, MidpointRounding.AwayFromZero))));
                                                    dCValue += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                                }
                                                if (sGetDefaultConfigId != Convert.ToString(dr["ConfigID"]))
                                                {
                                                    dCValueForOtherConfig = 0;
                                                    dCValueForOtherConfig += Convert.ToDecimal((Convert.ToString(decimal.Round(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])), 2, MidpointRounding.AwayFromZero))));
                                                }

                                                if (dBulkMetalRate > 0)
                                                {
                                                    dTotCValue = 0;
                                                    //dTotExchReturnQty += Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / dBulkMetalRate), 3, MidpointRounding.AwayFromZero))));
                                                    dTotExchReturnQty += Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(item.NetAmountWithNoTax)) / dBulkMetalRate), 3, MidpointRounding.AwayFromZero))));
                                                    item.PartnerData.CalExchangeQty = dTotExchReturnQty;
                                                    dTotExchReturnQty = 0m;

                                                    dTotCValue = dCValue;
                                                }
                                                else if (dCValueForOtherConfig > 0)
                                                {
                                                    dTotCValue = dCValueForOtherConfig;
                                                }
                                                else
                                                    item.PartnerData.CalExchangeQty = 0m;

                                                item.PartnerData.NewTaxPerc = 0;
                                            }
                                            //else
                                            //{
                                            //    item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                            //}


                                            if (isExchange || isConToAdv)
                                            {
                                                decimal dTaxPerc = getItemTaxPercentage(item.ItemId);
                                                decimal dGoldTax = 0m;

                                                if (dTaxPerc > 0 && dTotCValue > 0)
                                                    dGoldTax = decimal.Round(dTotCValue * (dTaxPerc / 100), 2, MidpointRounding.AwayFromZero);// Convert.ToString(dGVal * (dTaxPerc / 100));
                                                else
                                                    dGoldTax = 0;


                                                item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));
                                                //decimal dPrice = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));
                                                ////dTotCValue

                                                //if (isExchange)
                                                //    item.Price = dPrice * dExchPct / 100;
                                                //else if (isCashBack)
                                                //    item.Price = dPrice * dCashBackPct / 100;
                                                //else if (isConToAdv)
                                                //    item.Price = dPrice * dConvToAdvPct / 100;


                                                decimal dDiscPct = 0m;
                                                dDiscPct = (item.LineDiscount) / (item.Price * item.Quantity) * 100;
                                                // DISCAMOUNT/(203.28*QTY)*100
                                                foreach (var itemD in item.DiscountLines)
                                                {
                                                    itemD.Percentage = dDiscPct;
                                                }

                                                item.PartnerData.NewTaxPerc = 0;
                                            }
                                            else
                                            {
                                                SaleTrans.PartnerData.CashReturnWithOGTax = "1";
                                                item.PartnerData.NewTaxPerc = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) * Math.Abs(Convert.ToDecimal(dr["TAXAMOUNT"])) / 100), 15, MidpointRounding.AwayFromZero))));
                                                item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                            }
                                        }
                                        else
                                        {
                                            item.PartnerData.NewTaxPerc = 0;
                                            item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                        }
                                        #endregion
                                    }
                                    else
                                    {
                                        item.PartnerData.BulkItem = 0;
                                        if (isExchange || isConToAdv)
                                        {
                                            decimal dTaxPerc = getItemTaxPercentage(item.ItemId);
                                            decimal dGoldTax = 0m;

                                            if (dTaxPerc > 0 && dTotCValue > 0)
                                                dGoldTax = decimal.Round(dTotCValue * (dTaxPerc / 100), 2, MidpointRounding.AwayFromZero);// Convert.ToString(dGVal * (dTaxPerc / 100));
                                            else
                                                dGoldTax = 0;


                                            item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));
                                            //decimal  dPrice = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) + dGoldTax - Math.Abs(Convert.ToDecimal(dr["GOLDTAXVALUE"]))) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"])), 15, MidpointRounding.AwayFromZero))));

                                            ////dTotCValue

                                            //if (isExchange)
                                            //    item.Price = dPrice * dExchPct / 100;
                                            //else if (isCashBack)
                                            //    item.Price = dPrice * dCashBackPct / 100;
                                            //else if (isConToAdv)
                                            //    item.Price = dPrice * dConvToAdvPct / 100;

                                            decimal dDiscPct = 0m;
                                            dDiscPct = (item.LineDiscount) / (item.Price * item.Quantity) * 100;
                                            // DISCAMOUNT/(203.28*QTY)*100
                                            foreach (var itemD in item.DiscountLines)
                                            {
                                                itemD.Percentage = dDiscPct;
                                            }

                                            item.PartnerData.NewTaxPerc = 0;
                                        }
                                        else
                                        {
                                            // item.PartnerData.NewTaxAmt = Convert.ToDecimal(dr["TAXAMOUNT"]);
                                            item.PartnerData.NewTaxPerc = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) * Math.Abs(Convert.ToDecimal(dr["TAXAMOUNT"])) / 100), 15, MidpointRounding.AwayFromZero))));
                                            SaleTrans.PartnerData.CashReturnWithOGTax = "1";//SaleTrans=current retail transaction
                                            item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                            //decimal dPrice = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                            //if (isExchange)
                                            //    item.Price = dPrice * dExchPct / 100;
                                            //else if (isCashBack)
                                            //    item.Price = dPrice * dCashBackPct / 100;
                                            //else if (isConToAdv)
                                            //    item.Price = dPrice * dConvToAdvPct / 100;
                                        }
                                        //item.Price = Convert.ToDecimal((Convert.ToString(decimal.Round((Math.Abs(Convert.ToDecimal(dr["TOTALAMOUNT"])) / Math.Abs(Convert.ToDecimal(dr["QUANTITY"]))), 15, MidpointRounding.AwayFromZero))));
                                    }
                                }
                                else
                                {
                                    if (IsBulkItem(item.ItemId))
                                    {
                                        item.PartnerData.BulkItem = 1;
                                    }
                                    else
                                    {
                                        item.PartnerData.BulkItem = 0;
                                    }

                                    item.PartnerData.NewTaxPerc = 0;
                                }


                                if (isExchange)
                                    item.PartnerData.TransReturnPayMode = 1;
                                else if (isCashBack)
                                    item.PartnerData.TransReturnPayMode = 2;
                                else if (isConToAdv)
                                    item.PartnerData.TransReturnPayMode = 3;


                                #region value assign into dynamics object
                                item.PartnerData.ReturnReceiptId = retailTransaction.ReceiptId;
                                item.PartnerData.SpecialDiscInfo = sSplReturn;

                                item.PartnerData.Pieces = Convert.ToString(dr["PIECES"]);
                                item.PartnerData.Quantity = Convert.ToString(dr["QUANTITY"]);
                                item.PartnerData.Rate = Convert.ToString(dr["RATE"]);
                                item.PartnerData.RateType = Convert.ToString(dr["RATETYPE"]);
                                item.PartnerData.MakingRate = Convert.ToString(dr["MAKINGRATE"]); //
                                item.PartnerData.MakingType = Convert.ToString(dr["MAKINGTYPE"]); //
                                item.PartnerData.Amount = Convert.ToString(dr["AMOUNT"]);
                                item.PartnerData.MakingDisc = "0";// Convert.ToString(dr["MAKINGDISCOUNT"]); //
                                item.PartnerData.MakingAmount = Convert.ToString(Convert.ToDecimal(dr["MAKINGAMOUNT"]) - dExtraCalculatedGoldTaxAmt);// dExtraCalculatedGoldTaxAmt is added on 160818
                                item.PartnerData.TotalAmount = Convert.ToString(Convert.ToDecimal(dr["TOTALAMOUNT"]) - dExtraCalculatedGoldTaxAmt);// dExtraCalculatedGoldTaxAmt is added on 160818
                                item.PartnerData.TotalWeight = Convert.ToString(dr["TOTALWEIGHT"]);
                                item.PartnerData.LossPct = Convert.ToString(dr["LOSSPERCENTAGE"]);
                                item.PartnerData.LossWeight = Convert.ToString(dr["LOSSWEIGHT"]);
                                item.PartnerData.ExpectedQuantity = Convert.ToString(dr["EXPECTEDQUANTITY"]);
                                item.PartnerData.TransactionType = Convert.ToString(dr["TRANSACTIONTYPE"]);
                                // item.PartnerData.OChecked = Convert.ToString(dr["OWN"]);
                                item.PartnerData.OChecked = false;

                                if (Convert.ToInt32(dr["IsIngredient"]) == 1)
                                    item.PartnerData.Ingredients = Convert.ToString(dr["SKUDETAILS"]); //
                                else
                                    item.PartnerData.Ingredients = "";
                                item.PartnerData.OrderNum = Convert.ToString(dr["ORDERNUM"]);
                                item.PartnerData.OrderLineNum = Convert.ToString(dr["ORDERLINENUM"]);
                                item.PartnerData.CustNo = Convert.ToString(dr["CUSTNO"]);
                                item.PartnerData.SampleReturn = false;

                                item.PartnerData.WastageType = Convert.ToString(dr["WastageType"]);
                                item.PartnerData.WastageQty = Convert.ToString(dr["WastageQty"]);
                                item.PartnerData.WastageAmount = Convert.ToString(dr["WastageAmount"]);
                                item.PartnerData.WastagePercentage = Convert.ToString(dr["WastagePercentage"]);
                                item.PartnerData.WastageRate = Convert.ToString(dr["WastageRate"]);

                                item.PartnerData.MakingDiscountType = "0";// Convert.ToString(dr["MAKINGDISCTYPE"]);
                                item.PartnerData.MakingTotalDiscount = "0";// Convert.ToString(dr["MakingDiscountAmount"]);
                                item.PartnerData.ConfigId = Convert.ToString(dr["CONFIGID"]);

                                item.PartnerData.Purity = "0";
                                item.PartnerData.GROSSWT = "0";
                                item.PartnerData.GROSSUNIT = string.Empty;
                                item.PartnerData.DMDWT = "0";
                                item.PartnerData.DMDPCS = "0";
                                item.PartnerData.DMDUNIT = string.Empty;
                                item.PartnerData.DMDAMOUNT = "0";
                                item.PartnerData.STONEWT = "0";
                                item.PartnerData.STONEPCS = "0";
                                item.PartnerData.STONEUNIT = string.Empty;
                                item.PartnerData.STONEAMOUNT = "0";
                                item.PartnerData.NETWT = "0";
                                item.PartnerData.NETRATE = "0";
                                item.PartnerData.NETUNIT = string.Empty;
                                item.PartnerData.NETPURITY = "0";
                                item.PartnerData.NETAMOUNT = "0";
                                item.PartnerData.OGREFINVOICENO = string.Empty;
                                item.PartnerData.ServiceItemCashAdjustmentTransactionID = "";
                                item.PartnerData.ACTMKRATE = Convert.ToString(dr["MAKINGRATE"]);
                                //changed on 210617 req by S.Sharma
                                item.PartnerData.ACTTOTAMT = Convert.ToString(Math.Abs(Convert.ToDecimal(dr["AMOUNT"])) + Math.Abs(Convert.ToDecimal(dr["MAKINGAMOUNT"])) + Math.Abs(dExtraCalculatedGoldTaxAmt));// dExtraCalculatedGoldTaxAmt is added on 160818;// Convert.ToString(dr["TOTALAMOUNT"]);// Convert.ToString(dr["ACTTOTAMT"]);
                                item.PartnerData.CHANGEDTOTAMT = Convert.ToString(dr["CHANGEDTOTAMT"]);
                                item.PartnerData.LINEDISC = Convert.ToString(dr["LINEDISC"]);
                                item.PartnerData.TRANSFERCOSTPRICE = Convert.ToString(dr["TRANSFERCOSTPRICE"]);
                                //ADDED ON 20122017
                                item.PartnerData.LINEGOLDTAX = Convert.ToString(dr["GOLDTAXVALUE"]);
                                item.PartnerData.NewTaxAmt = Convert.ToDecimal(dr["TAXAMOUNT"]);
                                item.PartnerData.WTDIFFDISCQTY = Convert.ToString(dr["WTDIFFDISCQTY"]);
                                item.PartnerData.WTDIFFDISCAMT = Convert.ToString(dr["WTDIFFDISCAMT"]);

                                if (!string.IsNullOrEmpty(sSalesLoaltyType))
                                {
                                    item.PartnerData.LOYALTYTYPECODE = sSalesLoaltyType;
                                    item.PartnerData.LoyaltyProvider = sSalesLoaltyTypeProv;
                                    item.PartnerData.LOYALTYCARDNO = sSalesLoaltyCardNo;
                                    item.PartnerData.LOYALTYASKINGDONE = true;
                                }
                                else
                                {
                                    item.PartnerData.LOYALTYTYPECODE = "";
                                    item.PartnerData.LoyaltyProvider = "";
                                    item.PartnerData.LOYALTYCARDNO = "";
                                    item.PartnerData.LOYALTYASKINGDONE = false;
                                }
                                item.PartnerData.PurityReading1 = Convert.ToDecimal(dr["PurityReading1"]);
                                item.PartnerData.PurityReading2 = Convert.ToDecimal(dr["PurityReading2"]);
                                item.PartnerData.PurityReading3 = Convert.ToDecimal(dr["PurityReading3"]);
                                item.PartnerData.PURITYPERSON = Convert.ToString(dr["PURITYPERSON"]);
                                item.PartnerData.PURITYPERSONNAME = Convert.ToString(dr["PURITYPERSONNAME"]);
                                item.PartnerData.SKUAgingDiscType = Convert.ToInt16(dr["SKUAgingDiscType"]);
                                item.PartnerData.SKUAgingDiscAmt = Convert.ToDecimal(dr["SKUAgingDiscAmt"]);
                                item.PartnerData.TierDiscType = Convert.ToInt16(dr["TierDiscType"]);
                                item.PartnerData.TierDiscAmt = Convert.ToDecimal(dr["TierDiscAmt"]);
                                item.PartnerData.NimDiscLine = 0;
                                if (Convert.ToInt16(dr["MRPOrMkDiscType"]) == (int)MRPOrMkDiscType.Making)
                                {
                                    item.PartnerData.NimMakingDiscType = true;//
                                    item.PartnerData.NimMakingDisc = Convert.ToDecimal(dr["MRPOrMkDiscAmt"]); //MRPOrMkDiscAmt
                                    item.PartnerData.NimMRPDiscType = false;
                                    item.PartnerData.NimMRPDisc = 0; //MRPOrMkDiscAmt
                                }
                                else if (Convert.ToInt16(dr["MRPOrMkDiscType"]) == (int)MRPOrMkDiscType.MRP)
                                {
                                    item.PartnerData.NimMRPDiscType = true;
                                    item.PartnerData.NimMRPDisc = Convert.ToDecimal(dr["MRPOrMkDiscAmt"]); //MRPOrMkDiscAmt
                                    item.PartnerData.NimMakingDiscType = false;//
                                    item.PartnerData.NimMakingDisc = 0; //MRPOrMkDiscAmt
                                }
                                else
                                {
                                    item.PartnerData.NimMakingDiscType = false;
                                    item.PartnerData.NimMakingDisc = 0;
                                    item.PartnerData.NimMRPDisc = 0;
                                    item.PartnerData.NimMRPDiscType = false;
                                }
                                item.PartnerData.OGPSTONEPCS = Convert.ToDecimal(dr["OGPSTONEPCS"]);
                                item.PartnerData.OGPSTONEWT = Convert.ToDecimal(dr["OGPSTONEWT"]);
                                item.PartnerData.OGPSTONEUNIT = Convert.ToString(dr["OGPSTONEUNIT"]);
                                item.PartnerData.OGPSTONERATE = Convert.ToDecimal(dr["OGPSTONERATE"]);
                                item.PartnerData.OGPSTONEAMOUNT = Convert.ToDecimal(dr["OGPSTONEAMOUNT"]);

                                if (Convert.ToInt16(dr["MRPORMKPROMODISCTYPE"]) == (int)MRPOrMkDiscType.Making)
                                {
                                    //item.PartnerData.NIMPROMOCODE = Convert.ToDecimal(dr["MRPORMKPROMODISCCODE"]);
                                    item.PartnerData.NIMPROMOCODE = Convert.ToString(dr["MRPORMKPROMODISCCODE"]);
                                    item.PartnerData.NimPromoMakingDiscType = true;
                                    item.PartnerData.NimPromoMakingDisc = Convert.ToDecimal(dr["MRPORMKPROMODISCAMT"]);
                                    item.PartnerData.NimPromoMRPDiscType = false;
                                    item.PartnerData.NimPromoMRPDisc = 0;
                                }
                                else if (Convert.ToInt16(dr["MRPORMKPROMODISCTYPE"]) == (int)MRPOrMkDiscType.MRP)
                                {
                                    //item.PartnerData.NIMPROMOCODE = Convert.ToDecimal(dr["MRPORMKPROMODISCCODE"]);
                                    item.PartnerData.NIMPROMOCODE = Convert.ToString(dr["MRPORMKPROMODISCCODE"]);
                                    item.PartnerData.NimPromoMRPDiscType = true;
                                    item.PartnerData.NimPromoMRPDisc = Convert.ToDecimal(dr["MRPORMKPROMODISCAMT"]);
                                    item.PartnerData.NimPromoMakingDiscType = false;
                                    item.PartnerData.NimPromoMakingDisc = 0;
                                }
                                else
                                {
                                    item.PartnerData.NIMPROMOCODE = "";
                                    item.PartnerData.NimPromoMakingDiscType = false;
                                    item.PartnerData.NimPromoMakingDisc = 0;
                                    item.PartnerData.NimPromoMRPDisc = 0;
                                    item.PartnerData.NimPromoMRPDiscType = false;
                                }

                                item.PartnerData.OpeningDiscType = Convert.ToInt16(dr["OpeningDiscType"]);
                                item.PartnerData.OpeningDisc = Convert.ToDecimal(dr["OpeningDisc"]);
                                item.PartnerData.REPAIRBATCHID = "";

                                item.PartnerData.MakStnDiaDiscType = 0;
                                item.PartnerData.NimMakingDiscount = 0;
                                item.PartnerData.NimStoneDiscount = 0;
                                item.PartnerData.NimDiamondDiscount = 0;

                                item.PartnerData.MakStnDiaDisc = false;
                                item.PartnerData.MakStnDiaDiscDone = false;

                                item.PartnerData.AdvAgreemetDisc = false;
                                item.PartnerData.AdvAgreemetDiscDone = false;
                                item.PartnerData.SaleAdjustmentAdvanceDepositDate = "";

                                break;
                                #endregion value assign into dynamics object
                            }
                        }
                    }
                    #endregion
                }
                else
                {
                    preTriggerResult.ContinueOperation = false;
                    MessageBox.Show("Return not possible!.");
                    return;
                }
                #endregion
            }
            else
            {
                foreach (var item in lineitem)
                {
                    #region value assign into dynamics object
                    item.PartnerData.TransReturnPayMode = 0;
                    item.PartnerData.ReturnReceiptId = "";
                    item.PartnerData.Pieces = "";
                    item.PartnerData.Quantity = "";
                    item.PartnerData.Rate = "";
                    item.PartnerData.RateType = "";
                    item.PartnerData.MakingRate = "";
                    item.PartnerData.MakingType = "";
                    item.PartnerData.Amount = "";
                    item.PartnerData.MakingDisc = "";
                    item.PartnerData.MakingAmount = "";
                    item.PartnerData.TotalAmount = "";
                    item.PartnerData.TotalWeight = "";
                    item.PartnerData.LossPct = "";
                    item.PartnerData.LossWeight = "";
                    item.PartnerData.ExpectedQuantity = "";
                    item.PartnerData.TransactionType = "";
                    //  item.PartnerData.OChecked = ""; 
                    item.PartnerData.OChecked = false;
                    item.PartnerData.Ingredients = "";
                    item.PartnerData.SampleReturn = false;

                    item.PartnerData.WastageType = "0";
                    item.PartnerData.WastageQty = "0";
                    item.PartnerData.WastageAmount = "";

                    item.PartnerData.WastagePercentage = "0";
                    item.PartnerData.WastageRate = "0";

                    item.PartnerData.MakingDiscountType = "0";
                    item.PartnerData.MakingTotalDiscount = "0";
                    item.PartnerData.ConfigId = string.Empty;
                    item.PartnerData.Purity = "0";
                    item.PartnerData.GROSSWT = "0";
                    item.PartnerData.GROSSUNIT = string.Empty;
                    item.PartnerData.DMDWT = "0";
                    item.PartnerData.DMDPCS = "0";
                    item.PartnerData.DMDUNIT = string.Empty;
                    item.PartnerData.DMDAMOUNT = "0";
                    item.PartnerData.STONEWT = "0";
                    item.PartnerData.STONEPCS = "0";
                    item.PartnerData.STONEUNIT = string.Empty;
                    item.PartnerData.STONEAMOUNT = "0";
                    item.PartnerData.NETWT = "0";
                    item.PartnerData.NETRATE = "0";
                    item.PartnerData.NETUNIT = string.Empty;
                    item.PartnerData.NETPURITY = "0";
                    item.PartnerData.NETAMOUNT = "0";
                    item.PartnerData.OGREFINVOICENO = string.Empty;
                    item.PartnerData.SpecialDiscInfo = string.Empty;
                    item.PartnerData.RETAILBATCHNO = string.Empty;

                    item.PartnerData.ACTMKRATE = "0";
                    item.PartnerData.ACTTOTAMT = "0";
                    item.PartnerData.CHANGEDTOTAMT = "0";
                    item.PartnerData.LINEDISC = "0";
                    item.PartnerData.ServiceItemCashAdjustmentTransactionID = "";

                    item.PartnerData.OrderNum = "";
                    item.PartnerData.OrderLineNum = "";
                    item.PartnerData.CustNo = "";
                    item.PartnerData.SampleReturn = false;
                    item.PartnerData.TRANSFERCOSTPRICE = "0";
                    item.PartnerData.RETURNTYPE = "";
                    item.PartnerData.LINEGOLDTAX = "0";
                    item.PartnerData.CalExchangeQty = "0";
                    item.PartnerData.WTDIFFDISCQTY = "0";
                    item.PartnerData.WTDIFFDISCAMT = "0";

                    item.PartnerData.PurityReading1 = 0;
                    item.PartnerData.PurityReading2 = 0;
                    item.PartnerData.PurityReading3 = 0;
                    item.PartnerData.PURITYPERSON = "";
                    item.PartnerData.PURITYPERSONNAME = "";
                    item.PartnerData.SKUAgingDiscType = 0;
                    item.PartnerData.SKUAgingDiscAmt = 0;
                    item.PartnerData.TierDiscType = 0;
                    item.PartnerData.TierDiscAmt = 0;
                    item.PartnerData.NimDiscLine = 0;
                    item.PartnerData.NimMakingDiscType = false;
                    item.PartnerData.NimMakingDisc = 0;
                    item.PartnerData.NimMRPDisc = 0;
                    item.PartnerData.NimMRPDiscType = false;
                    item.PartnerData.OGPSTONEPCS = 0;
                    item.PartnerData.OGPSTONEWT = 0;
                    item.PartnerData.OGPSTONEUNIT = "";
                    item.PartnerData.OGPSTONERATE = 0;
                    item.PartnerData.OGPSTONEAMOUNT = 0;

                    item.PartnerData.NIMPROMOCODE = "";
                    item.PartnerData.NimPromoMakingDiscType = false;
                    item.PartnerData.NimPromoMakingDisc = 0;
                    item.PartnerData.NimPromoMRPDisc = 0;
                    item.PartnerData.NimPromoMRPDiscType = false;
                    item.PartnerData.OpeningDiscType = 0;
                    item.PartnerData.OpeningDisc = 0;

                    item.PartnerData.MakStnDiaDiscType = 0;
                    item.PartnerData.NimMakingDiscount = 0;
                    item.PartnerData.NimStoneDiscount = 0;
                    item.PartnerData.NimDiamondDiscount = 0;

                    item.PartnerData.MakStnDiaDisc = false;
                    item.PartnerData.MakStnDiaDiscDone = false;

                    item.PartnerData.AdvAgreemetDisc = false;
                    item.PartnerData.AdvAgreemetDiscDone = false;
                    item.PartnerData.SaleAdjustmentAdvanceDepositDate = "";

                    #endregion value assign into dynamics object
                }
            }

            #endregion
            //End:Nim
            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.PreReturnTransaction", "Before returning the transaction...", LSRetailPosis.LogTraceLevel.Trace);
        }

        public void PostReturnTransaction(IPosTransaction posTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.PostReturnTransaction", "After returning the transaction...", LSRetailPosis.LogTraceLevel.Trace);

            //==============================Code remarks on 27-08-2019 As Per Swarnavo & Supapta Da=====================
            //#region For auto loyalty add for PNG
            //RetailTransaction retailTrans = posTransaction as RetailTransaction;
            //if (!string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
            //{
            //    Loyalty.Loyalty objL = new Loyalty.Loyalty();
            //    DE.ICardInfo cardInfo = null;
            //    if (application != null)
            //        objL.Application = application;
            //    else
            //        objL.Application = this.Application;
            //    RetailTransaction asRetailTransaction = posTransaction as RetailTransaction;
            //    objL.AddLoyaltyRequest(asRetailTransaction, cardInfo);
            //}
            //#endregion
            //==========================================================================================================================
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public void PreConfirmReturnTransaction(IPreTriggerResult preTriggerResult, IRetailTransaction originalTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.PreConfirmReturnTransaction", "Before confirming return transaction...", LogTraceLevel.Trace);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        public void PreRollbackTransaction(IPreTriggerResult preTriggerResult, IPosTransaction originalTransaction)
        {
            LSRetailPosis.ApplicationLog.Log("TransactionTriggers.PreRollbackTransaction", "Before rolling back a failed-to-save transaction...", LogTraceLevel.Error);

            //Example:
            //DialogResult r = System.Windows.Forms.MessageBox.Show(
            //    LSRetailPosis.ApplicationLocalizer.Language.Translate(119),
            //    LSRetailPosis.ApplicationLocalizer.Language.Translate(118), 
            //    System.Windows.Forms.MessageBoxButtons.RetryCancel);
            //preTriggerResult.ContinueOperation = (r != DialogResult.Retry);
        }
    }
}
