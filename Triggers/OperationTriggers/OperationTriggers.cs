using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using System.Data;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using System;
using System.Windows.Forms;
using Microsoft.CSharp.RuntimeBinder;
using System.Collections.ObjectModel;
using System.IO;
using LSRetailPosis.Settings;
using System.Data.SqlClient;
using System.Text;
using DE = Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
namespace Microsoft.Dynamics.Retail.Pos.OperationTriggers
{
    [Export(typeof(IOperationTrigger))]
    public class OperationTriggers : IOperationTrigger
    {
        private IApplication application;

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

        #region Constructor - Destructor

        public OperationTriggers()
        {

            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for InfocodeTriggers are reserved at 59000 - 59999
        }

        #endregion

        //=========================Soutik
        bool IsCustomerCommitedQtyExists = false;
        string sSalesManDesc = string.Empty;
        decimal dCommitedQty = 0;
        int iCommitedForDays = 0;
        #region IOperationTriggersV1 Members

        /// <summary>
        /// Before the operation is processed this trigger is called.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        /// <param name="posisOperation"></param>
        public void PreProcessOperation(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction, PosisOperations posisOperation)
         {
            //Start:Nim
            LSRetailPosis.Transaction.RetailTransaction retailTrans = posTransaction as LSRetailPosis.Transaction.RetailTransaction;

            if (posisOperation == PosisOperations.Customer)
            {
                if (retailTrans != null)
                {
                    if (retailTrans.LastRunOperation == PosisOperations.ProcessInput)
                    {
                        retailTrans.PartnerData.SearchCustomer = false;
                    }
                }
            }
            if (posisOperation == PosisOperations.PayLoyalty)
            {
                //if (retailTrans != null)
                //{
                //    Loyalty.Loyalty objL = new Loyalty.Loyalty();
                //    DE.ICardInfo cardInfo = null;
                //    if (application != null)
                //        objL.Application = application;
                //    else
                //        objL.Application = this.Application;
                //    objL.AddLoyaltyRequest(retailTrans, cardInfo);

                //    foreach (ILoyaltyItem discLine in retailTrans.LoyaltyItem)
                //    {
                //    }
                //    retailTrans.ClearLoyaltyDiscountLines();

                //    retailTrans.LoyaltyItem.LoyaltyCardNumber = "";
                //    retailTrans.LoyaltyItem.LoyaltyCustID = "";
                //    retailTrans.LoyaltyItem.SchemeID = "";
                //    retailTrans.LoyaltyItem.CustID = "";
                //    retailTrans.LoyaltyItem.UsageType = LSRetailPosis.Transaction.Line.LoyaltyItem.LoyaltyItemUsageType.UsedForLoyaltyTender;
                //}
            }

            //==========================Soutik===============================================
            #region Customer Account Deposit
            if (posisOperation == PosisOperations.CustomerAccountDeposit)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Do you want to commit quantity against advance", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                    {

                        decimal AdvBookQtyPer = 0;
                        decimal MinDeposit = 0;
                        decimal MaxDeposit = 0;
                        string dCurrentMetalRate = string.Empty;
                        if (retailTrans != null)
                        {
                            FrmCustomerAdvBookQtyDays objCP = new FrmCustomerAdvBookQtyDays(posTransaction, application);
                            objCP.ShowDialog();


                            if (retailTrans.PartnerData.CustAdvCommitedQty > 0)
                            {
                                dCommitedQty = Convert.ToDecimal(retailTrans.PartnerData.CustAdvCommitedQty);
                                IsCustomerCommitedQtyExists = true;
                            }
                            else
                            {
                                dCommitedQty = 0m;
                                IsCustomerCommitedQtyExists = false;
                            }

                            if (retailTrans.PartnerData.CustCommitedForDays > 0)
                                iCommitedForDays = Convert.ToInt32(retailTrans.PartnerData.CustCommitedForDays);
                            else
                                iCommitedForDays = 0;

                            AdvBookQtyPer = GetAdvanceBookQtyPer();
                            dCurrentMetalRate = GetCurrentMetalRate();

                            MaxDeposit = decimal.Round(Convert.ToDecimal(dCommitedQty) * Convert.ToDecimal(dCurrentMetalRate), 2, MidpointRounding.AwayFromZero);

                            if (MaxDeposit > 0)
                                MinDeposit = decimal.Round((Convert.ToDecimal(MaxDeposit) * Convert.ToDecimal(AdvBookQtyPer)) / 100, 2, MidpointRounding.AwayFromZero);

                            //select TRANSID,MINIMUMDEPOSITFORCUSTORDER,TERMINALID ,* from RETAILTEMPTABLE --with MaxDeposit
                            //retailTrans.TransactionId;

                            RetailTempTable_Update(retailTrans.TransactionId, MaxDeposit);

                        }
                    }
                    else
                    {
                        retailTrans.PartnerData.CustAdvCommitedQty = 0;
                        retailTrans.PartnerData.CustCommitedForDays = 0;
                    }
                }
            }
            #endregion
            //ENd:Nim

            LSRetailPosis.ApplicationLog.Log("ICustomerTriggersV1.PreProcessOperation", "Before the operation is processed this trigger is called.", LSRetailPosis.LogTraceLevel.Trace);
        }

        #region Get Advance Book Qty Percentage
        private decimal GetAdvanceBookQtyPer() 
        {
            decimal dRetailAdvBookQtyPer = 0;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            string commandText = " SELECT ISNULL(ADVBOOKQTYPER,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'";
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dRetailAdvBookQtyPer = Convert.ToDecimal(reader.GetValue(0));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dRetailAdvBookQtyPer;
        }
        #endregion

        #region Get Current Metal Rate
        private string GetCurrentMetalRate()
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
            commandText.Append(" WHERE RETAILSTORETABLE.STORENUMBER='" + storeId + "' ");//[INVENTPARAMETERS] 

            commandText.Append(" SELECT @CONFIGID = DEFAULTCONFIGIDGOLD FROM RETAILPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");
            commandText.Append(" SELECT TOP 1 CAST(RATES AS NUMERIC(28,2))AS RATES FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            commandText.Append(" AND METALTYPE = 1 AND ACTIVE=1 "); // METALTYPE -- > GOLD
            commandText.Append(" AND CONFIGIDSTANDARD=@CONFIGID  AND RATETYPE = 3 and Retail=1 "); // RATETYPE -- > GSS->Sale 4->3 on 10/06/2016 req by S.Sharma
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
        #endregion

        #region UPDATE RETAILTEMPTABLE For Advance Book Qty
        private void RetailTempTable_Update(string sTransId,decimal dMaxAmt)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " UPDATE RETAILTEMPTABLE SET TRANSID='" + sTransId + "' ,MINIMUMDEPOSITFORCUSTORDER=" + dMaxAmt + ",TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "+
                                 " WHERE ID=1 And TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' ";
            SqlCommand command = new SqlCommand(commandText, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
        #endregion 


        /// <summary>
        /// After the operation has been processed this trigger is called.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="posisOperation"></param>
        public void PostProcessOperation(IPosTransaction posTransaction, PosisOperations posisOperation)
        {
            //Start:Nim
            string sOrderSalesManId = "";
            string sOrderSalesManName = "";
            if (posisOperation == PosisOperations.CustomerAccountDeposit)
            {
                LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;
                int iIsBreak = 0;

                if (custTrans != null && custTrans.Amount != 0)
                {
                    custTrans.PartnerData.EFTCardNo = string.Empty;

                    #region Auto sales person add
                    for (int i = 0; i < 1000; i++)// for malabar dubai changes only one sales persons will be added on transaction //12/05/2017
                    {
                        if (!string.IsNullOrEmpty(custTrans.customerDepositItem.Comment))
                        {
                            iIsBreak = 1;
                            break;
                        }
                        else
                        {

                            LSRetailPosis.POSProcesses.frmSalesPerson dialog6 = new LSRetailPosis.POSProcesses.frmSalesPerson();
                            dialog6.ShowDialog();
                            if (!string.IsNullOrEmpty(dialog6.SelectedEmployeeId))
                            {
                                custTrans.customerDepositItem.Comment = dialog6.SelectedEmployeeId + " - " + dialog6.SelectEmployeeName;
                                custTrans.PartnerData.SalesPersonId = dialog6.SelectedEmployeeId;
                                sSalesManDesc = dialog6.SelectedEmployeeId + " - " + dialog6.SelectEmployeeName;
                                //saleLineItem.SalesPersonId = dialog6.SelectedEmployeeId;
                                //saleLineItem.SalespersonName = dialog6.SelectEmployeeName;
                                break;
                            }
                        }
                    }
                    #endregion

                    if (iIsBreak == 1)
                    {
                        posTransaction.OperationCancelled = true;
                    }
                    else
                    {
                        #region Start 200318
                        if (!string.IsNullOrEmpty(Convert.ToString(custTrans.Customer.CustomerId)))
                        {
                            bool IsRepair = false;
                            string sCustOrder = OrderNum(out IsRepair);
                            decimal dFixRate = 0m;
                            string sFixConfigId = string.Empty;
                            decimal dManualBookedQty = 0m;
                            decimal dAdvTaxPerc = 0m;
                            //string sCustOrder = OrderNum();
                            bool bGSSTaxApplicable = IsGSSEmiInclOfTax();
                            decimal dGSSTaxPct = getGSSTaxPercentage();


                            string sProductType = string.Empty;
                            //BlankOperations.WinFormsTouch.frmProductTypeInput objBlankOpe = new BlankOperations.WinFormsTouch.frmProductTypeInput();
                            //objBlankOpe.ShowDialog();
                            //sProductType = objBlankOpe.sProductType;
                            decimal dMaxSvAmt = 0m;
                            decimal dMinSvAmt = 0m;
                            decimal dSVTollarenceAmt = 0m;

                            decimal dGoldBookingRatePercentage = GetGoldBookingRatePer(ref dAdvTaxPerc, ref dSVTollarenceAmt);
                            decimal dRateBookingAmt = 0m;
                            if (dGoldBookingRatePercentage > 0)
                                dRateBookingAmt = custTrans.Amount / (dGoldBookingRatePercentage / 100); // new cal for mala 11/01/17
                            //  decimal dTaxAmt = decimal.Round(Convert.ToDecimal(custTrans.Amount) * dAdvTaxPerc / 100, 2, MidpointRounding.AwayFromZero);

                            decimal dTaxAmt = decimal.Round(Convert.ToDecimal(custTrans.Amount) * dAdvTaxPerc / (dAdvTaxPerc + 100), 2, MidpointRounding.AwayFromZero);

                            if (!string.IsNullOrEmpty(sCustOrder))
                            {
                                decimal dFixedRatePercentage = 0m;
                                decimal dCustOrderTotalAmt = 0m;
                                decimal dCustOrdQty = 0m;


                                dFixRate = GetFixedRate("", ref sFixConfigId);

                                if (!IsRepair)
                                    dFixedRatePercentage = GetCustOrderFixedRateInfo(sCustOrder, ref dCustOrderTotalAmt);

                                //decimal dConvertedToFixingQty = decimal.Round(getConvertedGoldQty(sTransItemConfigID, Convert.ToDecimal(dTotWt)), 3, MidpointRounding.AwayFromZero);
                                decimal dOrdQty = 0m;
                                DataTable dtOr = GetOrderLineInfo(sCustOrder);

                                sOrderSalesManId = getOrderSMId(sCustOrder);
                                sOrderSalesManName = getSalesManName(sOrderSalesManId);

                                if (!string.IsNullOrEmpty(sOrderSalesManId))
                                {
                                    custTrans.customerDepositItem.Comment = sOrderSalesManId + " - " + sOrderSalesManName;
                                    custTrans.PartnerData.SalesPersonId = sOrderSalesManId;
                                }

                                if (dtOr != null && dtOr.Rows.Count > 0)
                                {
                                    //dOrdQty += getConvertedGoldQty(Convert.ToString(dtOr.Rows[0]["CONFIGID"]), Convert.ToDecimal(dtOr.Rows[0]["QTY"])); 
                                    dOrdQty = decimal.Round((dCustOrderTotalAmt / dFixRate), 3, MidpointRounding.AwayFromZero);
                                }

                                SqlConnection connection = new SqlConnection();

                                if (application != null)
                                    connection = application.Settings.Database.Connection;
                                else
                                    connection = ApplicationSettings.Database.LocalConnection;

                                Enums.EnumClass oEnum = new Enums.EnumClass();
                                string sMaxAmount = string.Empty;
                                string sTerminalID = ApplicationSettings.Terminal.TerminalId;
                                string sMinAmt = "0";


                                int isSV = isSuvarnaVrudhi(sCustOrder);
                                if (isSV == 0)
                                    sMinAmt = Convert.ToString(oEnum.ValidateMinDeposit(connection, out sMaxAmount, sTerminalID, dCustOrderTotalAmt));
                                else
                                    getOrderAdvanceAmount(sCustOrder, ref dMaxSvAmt);

                                if (isSV == 0)
                                {
                                    (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Comment = "INFORMATION : MINIMUM DEPOSIT AMOUNT IS " + decimal.Round(Convert.ToDecimal(sMinAmt), 2, MidpointRounding.AwayFromZero) + " " +
                                                                                                                                             "AND MAXIMUM DEPOSIT AMOUNT IS " + decimal.Round(Convert.ToDecimal(sMaxAmount), 2, MidpointRounding.AwayFromZero);
                                }
                                else
                                {
                                    (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Comment = "INFORMATION : MINIMUM DEPOSIT AMOUNT IS " + decimal.Round(Convert.ToDecimal(dMaxSvAmt - dSVTollarenceAmt), 2, MidpointRounding.AwayFromZero) + " " +
                                                                                                                                             "AND MAXIMUM DEPOSIT AMOUNT IS " + decimal.Round(Convert.ToDecimal(dMaxSvAmt), 2, MidpointRounding.AwayFromZero);
                                }

                                frmOptionSelectionGSSorCustomerOrder optionSelection = new frmOptionSelectionGSSorCustomerOrder(posTransaction, 1);
                                optionSelection.ShowDialog();

                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdNo = sCustOrder;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdPercentage = dFixedRatePercentage;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdTotAmt = dCustOrderTotalAmt;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustOrdQty = dOrdQty;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum = string.Empty;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths = string.Empty;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType = Convert.ToString("NORMALCUSTOMERDEPOSIT");
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone = Convert.ToBoolean(false);
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo = sCustOrder;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData = string.Empty;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds = string.Empty;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.IsRepair = IsRepair;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixingMinAmt = sMinAmt;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ADVAGAINST = optionSelection.iAdvAgainst;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.VOUCHERAGAINST = Convert.ToString(optionSelection.sAdvAgainstVou);
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRate = dFixRate;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedConfigId = sFixConfigId;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.RateBookingAmt = dRateBookingAmt;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ManualBookedQty = dManualBookedQty;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldBookingRatePercentage = dGoldBookingRatePercentage;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ProductType = sProductType;

                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxAmt = dTaxAmt;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxPct = dAdvTaxPerc;

                                //============================================Soutik
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustAdvCommitedQty = dCommitedQty;
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustCommitedForDays = iCommitedForDays;

                            }
                            else
                            {
                                #region Comment


                                frmOptionSelectionGSSorCustomerOrder optionSelection = new frmOptionSelectionGSSorCustomerOrder(posTransaction);
                                optionSelection.ShowDialog();
                                SqlConnection connection = new SqlConnection();

                                if (application != null)
                                    connection = application.Settings.Database.Connection;
                                else
                                    connection = ApplicationSettings.Database.LocalConnection;

                                if (connection.State == ConnectionState.Closed)
                                    connection.Open();
                                DataTable dtGSS = new DataTable();

                                if (optionSelection.isGSS) //GetGSSAccount
                                {
                                    try
                                    {
                                        if (this.Application.TransactionServices.CheckConnection())
                                        {
                                            ReadOnlyCollection<object> cAGSS;
                                            string sStoreId = ApplicationSettings.Terminal.StoreId;
                                            cAGSS = this.Application.TransactionServices.InvokeExtension("GetGSSAccount", custTrans.Customer.CustomerId);

                                            DataSet dsWH = new DataSet();
                                            StringReader srTransDetail = new StringReader(Convert.ToString(cAGSS[3]));

                                            if (Convert.ToString(cAGSS[3]).Trim().Length > 38)
                                            {
                                                dsWH.ReadXml(srTransDetail);
                                            }
                                            if (dsWH != null && dsWH.Tables.Count > 0)//[0].Rows
                                            {
                                                dtGSS = dsWH.Tables[0];
                                            }
                                        }
                                    }

                                    catch (Exception)
                                    {
                                        MessageBox.Show(string.Format("Failed due to RTS issue."));
                                        throw new RuntimeBinderException();
                                    }

                                    #region Old local
                                    //string commandText = string.Empty;

                                    //commandText = " SELECT   GSSACCOUNTOPENINGPOSTED.GSSACCOUNTNO AS [GSSACCOUNTNO.], " +
                                    //              " DIRPARTYTABLE.NAMEALIAS AS [CUSTOMERNAME],  " +
                                    //              "  CAST(GSSACCOUNTOPENINGPOSTED.INSTALLMENTAMOUNT AS NUMERIC(28,2)) AS [INSTALLMENTAMOUNT], " +
                                    //              " ( CASE  WHEN  GSSACCOUNTOPENINGPOSTED.SCHEMETYPE=0 THEN 'FIXED' ELSE 'FLEXIBLE' END) AS [SCHEMETYPE], " +
                                    //              " GSSACCOUNTOPENINGPOSTED.SCHEMECODE AS [SCHEMECODE], " +
                                    //              " ( CASE WHEN GSSACCOUNTOPENINGPOSTED.SCHEMEDEPOSITTYPE=0 THEN 'GOLD' ELSE 'AMOUNT' END) AS [DEPOSITTYPE],   " +
                                    //              " GSSACCOUNTOPENINGPOSTED.GSSConfirm AS [GSSCONFIRM] " +
                                    //              " FROM         DIRPARTYTABLE INNER JOIN " +
                                    //              " CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                                    //              " GSSACCOUNTOPENINGPOSTED ON CUSTTABLE.ACCOUNTNUM = GSSACCOUNTOPENINGPOSTED.CUSTACCOUNT " +
                                    //              " WHERE GSSACCOUNTOPENINGPOSTED.CUSTACCOUNT = '" + custTrans.Customer.CustomerId + "'";

                                    //if(connection.State == ConnectionState.Closed)
                                    //    connection.Open();


                                    //SqlCommand cmd = new SqlCommand(commandText, connection);
                                    //SqlDataReader rdr = cmd.ExecuteReader();
                                    //DataTable dtGSS = new DataTable();
                                    //dtGSS.Load(rdr);
                                    #endregion

                                    BlankOperations.WinFormsTouch.frmGSSInput oGSS = new frmGSSInput(dtGSS, (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Amount);
                                    oGSS.ShowDialog();

                                    //rdr.Dispose();

                                    if (oGSS.bStatus)
                                    {

                                        decimal dGSSTaxAmt = decimal.Round(Convert.ToDecimal(oGSS.Amount) * dGSSTaxPct / (dGSSTaxPct + 100), 2, MidpointRounding.AwayFromZero);

                                        dFixRate = GetFixedRate("GSS", ref sFixConfigId);

                                        //bool bGoldScheme = IsGSSGoldScheme(oGSS.GSSnumber);
                                        //if (bGoldScheme && oGSS.GoldFixing == false)
                                        //{
                                        //    MessageBox.Show("Can not connect to HO to validate the GSS EMI");
                                        //    posTransaction.OperationCancelled = true;
                                        //}


                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum = Convert.ToString(oGSS.GSSnumber);
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths = Convert.ToString(oGSS.months);
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType = Convert.ToString("GSS");
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdPercentage = 0;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdTotAmt = 0;

                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone = Convert.ToBoolean(true);
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixing = oGSS.GoldFixing;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).Amount = Convert.ToDecimal(oGSS.Amount);
                                        (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Amount = Convert.ToDecimal(oGSS.Amount);
                                        (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).TransSalePmtDiff = Convert.ToDecimal(oGSS.Amount);
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustOrdQty = 0;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdNo = string.Empty;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.IsRepair = Convert.ToBoolean(false);
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds = string.Empty;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixingMinAmt = "0";
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ADVAGAINST = "0";
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.VOUCHERAGAINST = string.Empty;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRate = dFixRate;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedConfigId = sFixConfigId;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.RateBookingAmt = oGSS.Amount;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ManualBookedQty = "0";
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldBookingRatePercentage = "0";
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ProductType = sProductType;

                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxAmt = dGSSTaxAmt;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxPct = dGSSTaxPct;

                                        //============================================Soutik
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustAdvCommitedQty = dCommitedQty;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustCommitedForDays = iCommitedForDays;
                                    }
                                    else
                                    {

                                        dFixRate = GetFixedRate("", ref sFixConfigId);

                                        decimal dMaxBookedQty = 0m;
                                        if (dFixRate > 0 && dRateBookingAmt > 0)
                                            dMaxBookedQty = (dRateBookingAmt / dFixRate);

                                        if (IsManualBookedQty())
                                        {
                                            BlankOperations.WinFormsTouch.frmInputGoldBookingQty objBlankOp = new BlankOperations.WinFormsTouch.frmInputGoldBookingQty(dMaxBookedQty);
                                            objBlankOp.ShowDialog();
                                            dManualBookedQty = objBlankOp.dQty;
                                        }

                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum = string.Empty;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths = string.Empty;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType = Convert.ToString("NORMALCUSTOMERDEPOSIT");

                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone = Convert.ToBoolean(false);

                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdNo = string.Empty;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdPercentage = 0;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdTotAmt = 0;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustOrdQty = 0;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds = string.Empty;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixingMinAmt = "0";
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.IsRepair = false;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ADVAGAINST = "0";
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.VOUCHERAGAINST = string.Empty;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRate = dFixRate;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedConfigId = sFixConfigId;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.RateBookingAmt = dRateBookingAmt;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ManualBookedQty = dManualBookedQty;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldBookingRatePercentage = dGoldBookingRatePercentage;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ProductType = sProductType;

                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxAmt = dTaxAmt;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxPct = dAdvTaxPerc;

                                        //============================================Soutik
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustAdvCommitedQty = dCommitedQty;
                                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustCommitedForDays = iCommitedForDays;
                                    }
                                }
                                #endregion

                                else
                                {
                                    dFixRate = GetFixedRate("", ref sFixConfigId);
                                    decimal dMaxBookedQty = 0m;
                                    if (dFixRate > 0 && dRateBookingAmt > 0)
                                        dMaxBookedQty = (dRateBookingAmt / dFixRate);

                                    if (IsManualBookedQty())
                                    {
                                        BlankOperations.WinFormsTouch.frmInputGoldBookingQty objBlankOp = new BlankOperations.WinFormsTouch.frmInputGoldBookingQty(dMaxBookedQty);
                                        objBlankOp.ShowDialog();
                                        dManualBookedQty = objBlankOp.dQty;
                                    }

                                    #region If Customer Book Qty Exists
                                    string sTerminalID;
                                    string sCustMinAmt;
                                    string sCustMaxAmt;
                                    decimal dManualBookedCommitedQty;
                                    if (IsCustomerCommitedQtyExists )  // Ask Ripan Da 31-07-2019
                                    {
                                        getCommitedQty(dFixRate, dRateBookingAmt, out sTerminalID, out sCustMinAmt, out sCustMaxAmt, out dManualBookedCommitedQty);

                                        Enums.EnumClass oEnum = new Enums.EnumClass();
                                        sCustMinAmt = Convert.ToString(oEnum.CustomerAdvValidateMinDeposit(connection, out sCustMaxAmt, sTerminalID,0));

                                        if (dManualBookedCommitedQty == 0 )
                                            (((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction))).CustomerDepositItem.Comment = "" + sSalesManDesc  + "" + Environment.NewLine + "INFORMATION : MINIMUM DEPOSIT AMOUNT IS " + decimal.Round(Convert.ToDecimal(sCustMinAmt), 2, MidpointRounding.AwayFromZero) + " " +
                                                                                                                                                                                    "AND MAXIMUM DEPOSIT AMOUNT IS " + decimal.Round(Convert.ToDecimal(sCustMaxAmt), 2, MidpointRounding.AwayFromZero);
                                    }
                                    else
                                        if (iCommitedForDays > 0)
                                            getCommitedQty(dFixRate, dRateBookingAmt, out sTerminalID, out sCustMinAmt, out sCustMaxAmt, out dManualBookedCommitedQty);
                                        

                                    #endregion

                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GSSNum = string.Empty;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.NumMonths = string.Empty;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationType = Convert.ToString("NORMALCUSTOMERDEPOSIT");

                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OperationDone = Convert.ToBoolean(false);

                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdNo = string.Empty;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdPercentage = 0;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRateCustOrdTotAmt = 0;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustOrdQty = 0;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds = string.Empty;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.IsRepair = false;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldFixingMinAmt = "0";
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ADVAGAINST = optionSelection.iAdvAgainst;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.VOUCHERAGAINST = Convert.ToString(optionSelection.sAdvAgainstVou);
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedRate = dFixRate;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.FixedConfigId = sFixConfigId;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.RateBookingAmt = dRateBookingAmt;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ManualBookedQty = dManualBookedQty;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.GoldBookingRatePercentage = dGoldBookingRatePercentage;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ProductType = sProductType;

                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxAmt = dTaxAmt;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.AdvTaxPct = dAdvTaxPerc;

                                    //============================================Soutik
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustAdvCommitedQty = dCommitedQty;
                                    ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.CustCommitedForDays = iCommitedForDays;
                                }
                            }
                        }
                        #endregion
                    }
                }
                else
                {
                    UpdateRetailTempTable();
                }
            }
            else if (posisOperation == PosisOperations.ItemSale)
            {
                /*LSRetailPosis.Transaction.RetailTransaction retailTrans = posTransaction as LSRetailPosis.Transaction.RetailTransaction;
                if (retailTrans != null)
                {
                    string sOrderNo = retailTrans.PartnerData.AdjustmentOrderNum;
                    if (!string.IsNullOrEmpty(sOrderNo))
                    {
                        foreach (var salesLine in retailTrans.SaleItems)
                        {
                            if (!string.IsNullOrEmpty(Convert.ToString(salesLine.PartnerData.OrderLineNum)))
                            {
                                sOrderSalesManId = getOrderSMId(sOrderNo, Convert.ToInt16(salesLine.PartnerData.OrderLineNum));
                                sOrderSalesManName = getSalesManName(sOrderSalesManId);
                            }

                            salesLine.SalesPersonId = sOrderSalesManId;
                            salesLine.SalespersonName = sOrderSalesManName;
                        }
                        //retailTrans.SalesPersonId = sOrderSalesManId;
                        //retailTrans.SalesPersonName = sOrderSalesManName;
                    }
                }*/
            }
             //End:Nim

            LSRetailPosis.ApplicationLog.Log("IOperationTriggersV1.PostProcessOperation", "After the operation has been processed this trigger is called.", LSRetailPosis.LogTraceLevel.Trace);
        }

        private void getCommitedQty(decimal dFixRate, decimal dRateBookingAmt, out string sTerminalID, out string sCustMinAmt, out string sCustMaxAmt, out decimal dManualBookedCommitedQty)
        {
            sTerminalID = ApplicationSettings.Terminal.TerminalId;
            sCustMinAmt = string.Empty;
            sCustMaxAmt = "0";
            dManualBookedCommitedQty = 0m;

            if (dFixRate > 0 && dRateBookingAmt > 0 && dCommitedQty == 0)
                dManualBookedCommitedQty = decimal.Round(Convert.ToDecimal((dRateBookingAmt / dFixRate)), 3, MidpointRounding.AwayFromZero);

            if (dCommitedQty == 0 && dManualBookedCommitedQty > 0)
                dCommitedQty = dManualBookedCommitedQty;
        }

        #endregion

        //Start:Nim
        #region Nim
        #region RETURN ORDER NUM
        private string OrderNum(out bool isRepair)
        {
            isRepair = false;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            // string commandText = " SELECT CUSTORDER FROM RETAILTEMPTABLE WHERE ID=1 ";
            string commandText = " SELECT CUSTORDER,CUSTID FROM RETAILTEMPTABLE WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
            SqlCommand command = new SqlCommand(commandText, connection);
            SqlDataReader reader = command.ExecuteReader();
            string strCustOrder = string.Empty;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    strCustOrder = Convert.ToString(reader.GetValue(reader.GetOrdinal("CUSTORDER")));
                    //string sIsRepair = strCustOrder = Convert.ToString(reader.GetValue(reader.GetOrdinal("CUSTID")));
                    string sIsRepair = Convert.ToString(reader.GetValue(reader.GetOrdinal("CUSTID")));
                    if (sIsRepair == "REPAIR")
                    {
                        isRepair = true;
                    }
                }
            }
            connection.Close();
            return strCustOrder;

        }
        #endregion

        #region UPDATE RETAILTEMPTABLE
        private void UpdateRetailTempTable()
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            // string commandText = " UPDATE RETAILTEMPTABLE SET TRANSID=NULL,CUSTORDER=NULL,GOLDFIXING=NULL,MINIMUMDEPOSITFORCUSTORDER=NULL WHERE ID=1 ";
            //Start : 05/06/2014
            string commandText = " UPDATE SKUTableTrans SET isLocked='False',isAvailable = 'TRUE' WHERE SkuNumber" +
                            " IN (select b.ITEMID from CUSTORDER_HEADER" +
                            " a left join CUSTORDER_DETAILS b on a.ORDERNUM =b.ORDERNUM " +
                            " where  b.IsBookedSKU=1 and" + //a.IsConfirmed=0 and
                            " a.ORDERNUM=(select CUSTORDER  from  RETAILTEMPTABLE" +
                            " WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "'))" +

                            " UPDATE CUSTORDER_HEADER SET IsConfirmed=0 WHERE ORDERNUM =(select CUSTORDER  from  RETAILTEMPTABLE" +
                            " WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "')" +
                //End : 05/06/2014
                            " UPDATE RETAILTEMPTABLE SET TRANSID=NULL,CUSTORDER=NULL,GOLDFIXING=NULL," +
                            " MINIMUMDEPOSITFORCUSTORDER=NULL WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE
            SqlCommand command = new SqlCommand(commandText, connection);
            command.ExecuteNonQuery();
            connection.Close();
        }
        #endregion

        private decimal GetCustOrderFixedRateInfo(string sOrderNo, ref decimal dCOTotAmt)  // Fixed Metal Rate New
        {
            decimal dFixRatePct = 0M;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string commandText = " DECLARE @FIXEDRATEVAL AS NUMERIC(32,2)" +
                                 " DECLARE @ORDERQTY AS NUMERIC(32,2)" +
                                 " SELECT @FIXEDRATEVAL = ISNULL(FIXEDRATEADVANCEPCT,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'" +
                                 " SELECT @FIXEDRATEVAL AS FIXEDRATEPCT,ISNULL(TOTALAMOUNT,0) AS TOTALAMOUNT FROM CUSTORDER_HEADER WHERE ORDERNUM = '" + sOrderNo + "' ";
            // " SELECT @ORDERQTY = sum(CAST(ISNULL(QTY,0) AS DECIMAL(28,3))) AS QTY FROM CUSTORDER_DETAILS  WHERE ORDERNUM = '" + sOrderNo + "'";


            SqlCommand command = new SqlCommand(commandText, connection);
            // string strCustOrder = Convert.ToString(command.ExecuteScalar());
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dFixRatePct = Convert.ToDecimal(reader.GetValue(0));
                    dCOTotAmt = Convert.ToDecimal(reader.GetValue(1));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();


            /*BlankOperations.WinFormsTouch.frmInputGoldBookingPercentage objBlankOp = new BlankOperations.WinFormsTouch.frmInputGoldBookingPercentage();
            objBlankOp.ShowDialog();
            dFixRatePct = objBlankOp.dPct;*/

            return dFixRatePct;
        }


        private void getOrderAdvanceAmount(string sOrderNo, ref decimal dTotAdvAmt)
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

            string commandText = " SELECT SUM(ISNULL(ADVANCEAMT,0)) FROM CUSTORDER_DETAILS WHERE ORDERNUM = '" + sOrderNo + "' ";


            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dTotAdvAmt = Convert.ToDecimal(reader.GetValue(0));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        private decimal getConvertedGoldQty(string sTransConfigid, decimal dQtyToConvert)//, bool istranstofixing
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION NVARCHAR(20)  DECLARE @FIXINGCONFIGID NVARCHAR(20)  DECLARE @TRANSCONFIGID NVARCHAR(20)  DECLARE @istranstofixing NVARCHAR(5) ");
            commandText.Append(" DECLARE @QTY NUMERIC(32,10)  DECLARE @FIXINGCONFIGRATIO NUMERIC(32,16)  DECLARE @TRANSCONFIGRATIO NUMERIC(32,16) ");
            commandText.Append("SET @TRANSCONFIGID = '" + sTransConfigid + "'");
            commandText.Append("SET @QTY = ");
            commandText.Append(dQtyToConvert);

            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "'  "); //INVENTPARAMETERS
            commandText.Append(" SELECT @FIXINGCONFIGID = DefaultConfigIdGold FROM RETAILPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" SELECT @FIXINGCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = 1 AND CONFIGIDSTANDARD = @FIXINGCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" SELECT @TRANSCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = 1 AND CONFIGIDSTANDARD = @TRANSCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            commandText.Append(" IF(@FIXINGCONFIGRATIO >= @TRANSCONFIGRATIO) ");
            commandText.Append(" BEGIN  ");
            commandText.Append(" IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) ");
            commandText.Append(" BEGIN  ");
            commandText.Append(" SELECT ISNULL((( @TRANSCONFIGRATIO * @QTY) / @FIXINGCONFIGRATIO),0) AS CONVERTEDQTY ");
            commandText.Append(" END  ");
            commandText.Append(" else");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT @QTY AS CONVERTEDQTY ");
            commandText.Append(" END ");
            commandText.Append(" END");
            commandText.Append(" ELSE");
            commandText.Append(" BEGIN	");
            commandText.Append(" IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) ");
            commandText.Append(" BEGIN  ");
            commandText.Append(" SELECT ISNULL(((@FIXINGCONFIGRATIO * @QTY) / @TRANSCONFIGRATIO),0) AS CONVERTEDQTY   ");
            commandText.Append(" END  ");
            commandText.Append(" ELSE");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT @QTY AS CONVERTEDQTY ");
            commandText.Append(" END ");
            commandText.Append(" END  ");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            decimal dResult = Convert.ToDecimal(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dResult;

        }

        private decimal GetFixedRate(string sOperationType, ref string sConfigId)  // Fixed Metal Rate New
        {
            decimal dFixRatePct = 0M;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandString = " DECLARE @INVENTLOCATION VARCHAR(20) ";
            commandString += " DECLARE @CONFIGID VARCHAR(20) ";
            commandString += " DECLARE @RATE numeric(28, 3) ";
            commandString += " SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  RETAILCHANNELTABLE INNER JOIN ";
            commandString += " RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ";
            commandString += " WHERE RETAILSTORETABLE.STORENUMBER='" + ApplicationSettings.Terminal.StoreId + "'";

            if (sOperationType == "GSS")
                commandString += "SELECT top 1 @CONFIGID = [GSSDefaultConfigIdGold] from RETAILPARAMETERS WHERE DATAAREAID='" + application.Settings.Database.DataAreaID + "'";
            else
                commandString += " SELECT @CONFIGID = DEFAULTCONFIGIDGOLD FROM [RETAILPARAMETERS] WHERE DATAAREAID='" + application.Settings.Database.DataAreaID + "'";//INVENTPARAMETERS

            commandString += "  SELECT TOP 1  RATES,CONFIGIDSTANDARD FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ";
            commandString += " AND METALTYPE=" + (int)Enums.EnumClass.MetalType.Gold + " AND ACTIVE=1 and RETAIL=1 ";
            commandString += " AND CONFIGIDSTANDARD=@CONFIGID  AND RATETYPE=" + (int)Enums.EnumClass.RateType.Sale + " ";// GSS ->Sales Req by S.Sharma on 10/06/2016
            commandString += " ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC;";

            SqlCommand command = new SqlCommand(commandString, connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dFixRatePct = Convert.ToDecimal(reader.GetValue(0));
                    sConfigId = Convert.ToString(reader.GetValue(1));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dFixRatePct;
        }

        private DataTable GetOrderLineInfo(string sOrder)
        {

            DataTable dtOrd = new DataTable();
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" select SalesManId,LINENUM,CONFIGID,CAST(ISNULL(QTY,0) AS DECIMAL(28,3)) AS QTY from CUSTORDER_DETAILS where ORDERNUM='" + sOrder + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(commandText.ToString(), conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dtOrd);
            }

            return dtOrd;
        }

        private decimal GetGoldBookingRatePer(ref decimal dAdvTaxPerc, ref decimal dSVT)
        {
            decimal dFixRatePct = 0M;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string commandText = " SELECT ISNULL(FIXEDRATEADVANCEPCT,0),ISNULL(ADVANCETAXPERCENTAGE,0),ISNULL(SVADVTOLERANCE,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'";

            SqlCommand command = new SqlCommand(commandText, connection);
            // string strCustOrder = Convert.ToString(command.ExecuteScalar());
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dFixRatePct = Convert.ToDecimal(reader.GetValue(0));
                    dAdvTaxPerc = Convert.ToDecimal(reader.GetValue(1));
                    dSVT = Convert.ToDecimal(reader.GetValue(2));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            /* BlankOperations.WinFormsTouch.frmInputGoldBookingPercentage objBlankOp = new BlankOperations.WinFormsTouch.frmInputGoldBookingPercentage();
             objBlankOp.ShowDialog();
             dFixRatePct = objBlankOp.dPct;*/

            return dFixRatePct;
        }

        private bool IsManualBookedQty()
        {
            bool bResult = false;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string commandText = " SELECT ISNULL(ManualBookedQty,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    bResult = Convert.ToBoolean(reader.GetValue(0));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bResult;
        }

        private decimal getGSSTaxPercentage()
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;
            decimal sResult = 0m;

            string commandText = string.Empty;
            commandText = "select top 1 isnull(GSSTAXPERCENTAGE,0) from RETAILPARAMETERS where DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'";
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            sResult = Convert.ToDecimal(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            return sResult;

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

        private bool IsGSSGoldScheme(string sGSSAcc)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select SCHEMEDEPOSITTYPE from GSSACCOUNTOPENINGPOSTED where GSSACCOUNTNO ='" + sGSSAcc + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            int sResult = Convert.ToInt16(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (sResult == 0)
                return true;
            else
                return false;

        }

        private string getOrderSMId(string sOrderNo, int iLine = 0)
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;
            string sResult = "";

            string commandText = string.Empty;
            commandText = "select isnull(SalesManID,'') SalesManID from CUSTORDER_DETAILS  where ORDERNUM='" + sOrderNo + "' And LINENUM=" + iLine + "";
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            sResult = Convert.ToString(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            return sResult;
        }

        private int isSuvarnaVrudhi(string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            connection = ApplicationSettings.Database.LocalConnection;
            int iResult = 0;

            string commandText = string.Empty;
            commandText = "select isnull(SUVARNAVRUDHI,0) SUVARNAVRUDHI from CUSTORDER_HEADER  where ORDERNUM='" + sOrderNo + "'";
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            iResult = Convert.ToInt16(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            return iResult;
        }

        private string getSalesManName(string sStaffId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "select d.NAME as Name from RETAILSTAFFTABLE r" +
                                " left join dbo.HCMWORKER as h on h.PERSONNELNUMBER = r.STAFFID" +
                                " left join dbo.DIRPARTYTABLE as d on d.RECID = h.PERSON " +
                                " where r.STAFFID='" + sStaffId + "'";

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

        #endregion
        //End:Nim

    }
}
