/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System.ComponentModel.Composition;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessObjects;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

using BlankOperations;
using System;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using System.Data;
using System.IO;
using LSRetailPosis.Transaction;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.CSharp.RuntimeBinder;
using BlankOperations.WinFormsTouch;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Drawing;
using System.Drawing.Imaging;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using System.Globalization;
using System.Collections.Generic;
using Microsoft.Dynamics.Retail.Pos.Printing;
using Microsoft.Reporting.WinForms;
using System.Drawing.Printing;
using System.Reflection;
using DecimalToText;
using System.Net;
using System.Collections;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations
{


    [Export(typeof(IBlankOperations))]
    public sealed class BlankOperations : IBlankOperations
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
                //  InternalApplication = value;
            }
        }
        private DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
               LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection,
               LSRetailPosis.Settings.ApplicationSettings.Database.DATAAREAID);
        string sSaleItem = string.Empty;

        public string FreeGiftCWQTY = string.Empty;
        public string FreeGiftQTY = string.Empty;
        public string FreeGiftPromoCode = string.Empty;
        public string FreeGiftConfig = string.Empty;
        public string FreeGiftColor = string.Empty;
        public string FreeGiftSize = string.Empty;

        int iOWNDMD = 0; int iOWNOG = 0; int iOTHERDMD = 0; int iOTHEROG = 0;

        public string AdjustmentOrderNum = string.Empty;
        public string AdjustmentCustAccount = string.Empty;
        string sCustOrderReceiptDate = "";
        int iIsCustOrderWithAdv = 0;



        private IList<Stream> m_streams;
        private int currentPageIndex;

        private enum TransactionType
        {
            Sale = 0,
            Purchase = 1,
            Exchange = 3,
            PurchaseReturn = 2,
            ExchangeReturn = 4,
        }

        enum Salutation
        {
            Dr = 0,
            Mr = 1,
            Miss = 2,
            Mrs = 3,
            Ms = 4,
            None = 5,
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

        private enum CRWRetailDiscPermission // added on 29/08/2014
        {
            Cashier = 0,
            Salesperson = 1,
            Manager = 2,
            Other = 3,
            Manager2 = 4,
        }

        DataTable dtSalutation = new DataTable();

        public string sPincode { get; set; }

        //private ISuspendRetrieveSystem CustomerSystem
        //{
        //    get { return this.Application.BusinessLogic.SuspendRetrieveSystem; }
        //}
        //  internal static IApplication InternalApplication { get; private set; }



        // Get all text through the Translation function in the ApplicationLocalizer
        // TextID's for BlankOperations are reserved at 50700 - 50999

        #region IBlankOperations Members
        /// <summary>
        /// Displays an alert message according operation id passed.
        /// </summary>
        /// <param name="operationInfo"></param>
        /// <param name="posTransaction"></param>        
        /// 


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void BlankOperation(IBlankOperationInfo operationInfo, IPosTransaction posTransaction)
        {

            string sCustAc = string.Empty;
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            SqlConnection connection = new SqlConnection();
            DataTable dt = new DataTable();


            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            switch (operationInfo.OperationId)
            {
                case "CUSTORDR":
                    #region CUSTORDR
                    if (retailTrans != null)
                    {
                        #region Old
                        if (string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
                        {
                            if (CheckTerminal(ApplicationSettings.Terminal.TerminalId))
                            {
                                frmCustomerOrder objCustOrdr = new frmCustomerOrder(posTransaction, application);
                                objCustOrdr.ShowDialog();

                                PostCustOrderSave(posTransaction, objCustOrdr);
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(" Terminal ID : " + ApplicationSettings.Terminal.TerminalId + " set up not completed.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    operationInfo.OperationHandled = false;
                                }
                            }
                        }
                        else
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please complete the existing transaction before this.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                operationInfo.OperationHandled = false;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        if (CheckTerminal(ApplicationSettings.Terminal.TerminalId))
                        {
                            frmCustomerOrder objCustOrdr = new frmCustomerOrder(posTransaction, application);
                            objCustOrdr.ShowDialog();

                            PostCustOrderSave1(posTransaction, objCustOrdr);

                            //if (IsCustOrderConfirmed(objCustOrdr.sCustOrder) && objCustOrdr.iIsCustOrderWithAdv == 0)
                            //{
                            //try
                            //{
                            //    PrintVoucher(objCustOrdr.sCustOrder, 2);
                            //}
                            //catch { }
                            //}
                            objCustOrdr.chkSUVARNAVRUDDHI.Checked = false;
                        }
                        else
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(" Terminal ID : " + ApplicationSettings.Terminal.TerminalId + " set up not completed.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                operationInfo.OperationHandled = false;
                            }
                        }
                    }
                    break;

                    #endregion
                case "REPAIRORDR":
                    #region Repair
                    if (retailTrans != null)
                    {
                        if (string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
                        {
                            frmRepair objRepair = new frmRepair(posTransaction, application);
                            objRepair.ShowDialog();
                        }
                        else
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please complete the existing transaction before this.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                operationInfo.OperationHandled = false;
                            }
                        }
                    }
                    else
                    {
                        frmRepair objRepair = new frmRepair(posTransaction, application);
                        objRepair.ShowDialog();
                    }
                    break;
                    #endregion
                case "REPAIRRETORDR":
                    #region REPAIRRETORDR

                    if (retailTrans != null)
                    {
                        if (Convert.ToString(retailTrans.Customer.CustomerId) == string.Empty)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select customer", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            operationInfo.OperationHandled = true;
                            return;
                        }

                        frmYESNO frmYN = new frmYESNO("HO repair", "Store repair");
                        frmYN.ShowDialog();

                        if (frmYN.isY)
                        {
                            #region IN-House
                            sCustAc = Convert.ToString(retailTrans.Customer.CustomerId);
                            DataTable dtReturn = new DataTable();
                            DataRow selRow = null;
                            string sRepairId = "";
                            string sBatchId = "";

                            dtReturn = GetRepairReturnData("", sCustAc, 1);// inhouse =1

                            if (dtReturn != null && dtReturn.Rows.Count > 0)
                            {
                                Dialog.Dialog oDialog = new Dialog.Dialog();
                                oDialog.GenericSearch(dtReturn, ref selRow, "In-House Ornament Repair Return");

                                if (selRow != null)
                                {
                                    DataTable dtSelect = new DataTable();
                                    sRepairId = Convert.ToString(selRow["Repair No"]);
                                    sBatchId = Convert.ToString(selRow["Batch Id"]);

                                    dtSelect = GetRepairReturnData(sBatchId, sCustAc, 1);

                                    if (dtSelect != null && dtSelect.Rows.Count > 0)
                                    {
                                        frmRepairReturn objRepairReturn = new frmRepairReturn(posTransaction, application, dtSelect.Rows[0]);
                                        DialogResult dres = objRepairReturn.ShowDialog();
                                        if (dres == DialogResult.OK)
                                        {
                                            retailTrans.PartnerData.IsRepairReturn = true;
                                            retailTrans.PartnerData.RefRepairId = sRepairId;
                                            retailTrans.PartnerData.RepairRetTransId = objRepairReturn.sRepairRetId;
                                            string sRepairRetAdejItemId = string.Empty;
                                            RepairAdjItemId(ref sRepairRetAdejItemId);
                                            DataTable dtSKU = objRepairReturn.dtRepairReturnCashAdvanceDataSku;
                                            retailTrans.PartnerData.REPAIRRETCHARGES = objRepairReturn.dRepairExpense;

                                            string sRepBatchId = Convert.ToString(dtSelect.Rows[0]["REPAIRID"]);
                                            retailTrans.PartnerData.REPAIRID = sRepBatchId;

                                            DataTable dtNoOfRow = GetInHouseRepReturnTotalRow(sBatchId);

                                            if (dtNoOfRow != null && dtNoOfRow.Rows.Count > 0)
                                            {
                                                foreach (DataRow dr in dtNoOfRow.Rows)
                                                {
                                                    Application.RunOperation(PosisOperations.ItemSale, Convert.ToString(dr["ITEMID"]));
                                                }
                                            }

                                            if (dtSKU != null)
                                            {
                                                foreach (DataRow dr in dtSKU.Rows)
                                                {
                                                    if (objRepairReturn.dRepairExpense > 0)
                                                    {
                                                        Application.RunOperation(PosisOperations.ItemSale, sRepairRetAdejItemId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    operationInfo.OperationHandled = true;
                                    return;
                                }
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog1 = new LSRetailPosis.POSProcesses.frmMessage("No Order Exists.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog1);
                                }
                                operationInfo.OperationHandled = true;
                                return;
                            }
                            #endregion
                        }
                        else if (frmYN.isN)
                        {
                            #region Out-Side
                            sCustAc = Convert.ToString(retailTrans.Customer.CustomerId);
                            DataTable dtReturn = new DataTable();
                            DataRow selRow = null;
                            string sRepairId = "";
                            string sBatchId = "";

                            dtReturn = GetRepairReturnData("", sCustAc, 2);// OutSide =2

                            if (dtReturn != null && dtReturn.Rows.Count > 0)
                            {
                                Dialog.Dialog oDialog = new Dialog.Dialog();
                                oDialog.GenericSearch(dtReturn, ref selRow, "Out-Side Ornament Repair Return");

                                if (selRow != null)
                                {
                                    DataTable dtSelect = new DataTable();
                                    sRepairId = Convert.ToString(selRow["Repair No"]);
                                    sBatchId = Convert.ToString(selRow["Batch Id"]);

                                    dtSelect = GetRepairReturnData(sBatchId, sCustAc, 2);

                                    if (dtSelect != null && dtSelect.Rows.Count > 0)
                                    {
                                        frmRepairReturn objRepairReturn = new frmRepairReturn(posTransaction, application, dtSelect.Rows[0]);
                                        DialogResult dres = objRepairReturn.ShowDialog();
                                        if (dres == DialogResult.OK)
                                        {
                                            retailTrans.PartnerData.IsRepairReturn = true;
                                            retailTrans.PartnerData.RefRepairId = sRepairId;
                                            retailTrans.PartnerData.RepairRetTransId = objRepairReturn.sRepairRetId;
                                            //retailTrans.PartnerData.REPAIRIDFORRETURN = sRepairId;
                                            string sRepairRetAdejItemId = string.Empty;
                                            RepairAdjItemId(ref sRepairRetAdejItemId);
                                            DataTable dtSKU = objRepairReturn.dtRepairReturnCashAdvanceDataSku;
                                            retailTrans.PartnerData.REPAIRRETCHARGES = objRepairReturn.dRepairExpense;

                                            string sRepBatchId = Convert.ToString(dtSelect.Rows[0]["REPAIRID"]);
                                            retailTrans.PartnerData.REPAIRID = sRepBatchId;

                                            DataTable dtNoOfRow = GetTotalRow(sRepBatchId);

                                            if (dtNoOfRow != null && dtNoOfRow.Rows.Count > 0)
                                            {
                                                foreach (DataRow dr in dtNoOfRow.Rows)
                                                {
                                                    DataTable dtRepOG = GetOgInfoForRepairReturn(sRepBatchId, Convert.ToString(dr["TRANSACTIONID"]));
                                                    dtRepOG.TableName = "RepOG";

                                                    string sTableName = "REPOG" + "" + ApplicationSettings.Database.TerminalID;

                                                    if (dtRepOG != null)
                                                    {
                                                        foreach (DataRow dr1 in dtRepOG.Rows)
                                                        {
                                                            GETOGRepairInfo(sTableName, Convert.ToDecimal(dr1["EXPECTEDQUANTITY"]), Convert.ToString(dr1["INVENTBATCHID"]));
                                                            Application.RunOperation(PosisOperations.ItemSale, Convert.ToString(dr1["ITEMID"]));
                                                        }
                                                    }
                                                }
                                            }

                                            if (dtSKU != null)
                                            {
                                                foreach (DataRow dr in dtSKU.Rows)
                                                {
                                                    if (objRepairReturn.dRepairExpense > 0)
                                                    {
                                                        Application.RunOperation(PosisOperations.ItemSale, sRepairRetAdejItemId);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    operationInfo.OperationHandled = true;
                                    return;
                                }
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog1 = new LSRetailPosis.POSProcesses.frmMessage("No Order Exists.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog1);
                                }
                                operationInfo.OperationHandled = true;
                                return;
                            }
                            #endregion
                        }

                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select customer", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        operationInfo.OperationHandled = true;
                        return;
                    }
                    break;
                    #endregion
                case "FGRECEIVE":
                    #region FG Received
                    frmFGReceived objFGReceive = new frmFGReceived();
                    objFGReceive.ShowDialog();
                    break;
                    #endregion
                case "FGTRANSFER":
                    #region FG Transferred to company
                    frmFGTransferred objFGTransfer = new frmFGTransferred();
                    objFGTransfer.ShowDialog();
                    break;
                    #endregion
                case "TRANSORDERCREATE":
                    #region Transfer Order Create
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    frmTransferOrder objTransOrderCreate = new frmTransferOrder(connection);
                    objTransOrderCreate.ShowDialog();
                    break;
                    #endregion
                case "TRANSORDRCV":
                    #region Transfer Order Receive
                    frmTransferOrderReceive objTransOrderRcv = new frmTransferOrderReceive();
                    objTransOrderRcv.ShowDialog();
                    break;
                    #endregion
                case "CHANGEAMOUNT":
                    #region CHANGE CUSTOMER DEPOSIT AMOUNT
                    if (Convert.ToString(posTransaction.GetType().Name).ToUpper().Trim() == "CUSTOMERPAYMENTTRANSACTION")
                    {
                        LSRetailPosis.POSProcesses.frmInputNumpad oNum = new LSRetailPosis.POSProcesses.frmInputNumpad(true, true);
                        oNum.Text = "Customer account deposit";
                        oNum.EntryTypes = Contracts.UI.NumpadEntryTypes.Price;
                        string sFixConfigId = "";
                        decimal dManualBookedQty = 0m;



                        oNum.PromptText = "Deposit amount";
                        oNum.ShowDialog();
                        if (!string.IsNullOrEmpty(oNum.InputText))
                        {
                            LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;
                            decimal dFixRate = GetFixedRate("", ref sFixConfigId);

                            decimal dMaxBookedQty = 0m;
                            string sProductType = "";

                            custTrans.Amount = Convert.ToDecimal(oNum.InputText);
                            custTrans.CustomerDepositItem.Amount = Convert.ToDecimal(oNum.InputText);
                            custTrans.TransSalePmtDiff = Convert.ToDecimal(oNum.InputText);

                            decimal dGoldBookingRatePercentage = GetGoldBookingRatePer();
                            decimal dRateBookingAmt = custTrans.Amount / (dGoldBookingRatePercentage / 100);
                            if (dFixRate > 0 && dRateBookingAmt > 0)
                                dMaxBookedQty = (dRateBookingAmt / dFixRate);
                            if (IsManualBookedQty())
                            {
                                frmInputGoldBookingQty objBlankOp = new frmInputGoldBookingQty(dMaxBookedQty);
                                objBlankOp.ShowDialog();
                                dManualBookedQty = objBlankOp.dQty;
                            }

                            custTrans.PartnerData.ManualBookedQty = dManualBookedQty;
                            custTrans.PartnerData.GoldBookingRatePercentage = dGoldBookingRatePercentage;
                            custTrans.PartnerData.ProductType = sProductType;

                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Customer Deposit Found.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                    break;
                    #endregion
                case "SERVICEITEM":
                    #region SERVICE ITEM - AMOUNT ADJUSTMENT
                    if (retailTrans != null)
                    {
                        if (operationInfo.ReturnItems)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Adjustment Cannot be returned.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            operationInfo.OperationHandled = false;
                        }
                        bool isServiceItemExists = false;
                        System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
                        if (saleline.Count == 0)
                        {
                            retailTrans.PartnerData.TransAdjGoldQty = 0; // Avg Gold Rate Adjustment
                            string sAdJustItem = AdjustmentItemID();
                            Application.RunOperation(PosisOperations.ItemSale, sAdJustItem);
                            return;
                        }
                        // Application.RunOperation(PosisOperations.ItemSale, AdjustmentItemID());

                        #region Commented
                        if (saleline.Count > 0)
                        {

                            foreach (var sale in saleline)
                            {
                                if (sale.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                {
                                    isServiceItemExists = true;
                                }
                            }
                            if (!isServiceItemExists)
                            {
                                Application.RunOperation(PosisOperations.ItemSale, AdjustmentItemID());
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can only be adjusted in the beginning of transaction.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                                operationInfo.OperationHandled = false;
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount cannot be adjusted without any customer. Please select the customer to adjust the Amount.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        operationInfo.OperationHandled = false;
                    }

                    break;
                    #endregion
                case "GSSMATURITY":
                    #region GSS MATURITY
                    frmGSSMaturity objGSSMaturity = new frmGSSMaturity();
                    objGSSMaturity.ShowDialog();
                    break;
                    #endregion
                case "GSSMATURITYADJ":
                    #region [GSS Maturity Adjustment]
                    if (retailTrans != null)
                    {
                        if (operationInfo.ReturnItems)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Adjustment Cannot be returned.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            return;
                        }

                        if (Convert.ToString(retailTrans.Customer.CustomerId) == string.Empty)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Customer found.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            return;
                        }
                        if (retailTrans.SaleItems.Count > 0)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Not a valid Transaction.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            return;
                        }

                        sCustAc = retailTrans.Customer.CustomerId;
                        DataRow drSelected = null;

                        try
                        {
                            if (PosApplication.Instance.TransactionServices.CheckConnection())
                            {
                                ReadOnlyCollection<object> cAGSS;
                                string sStoreId = ApplicationSettings.Terminal.StoreId;
                                cAGSS = PosApplication.Instance.TransactionServices.InvokeExtension("getUnadjustedGSS", sCustAc);//getUnadjustedGSS

                                DataSet dsWH = new DataSet();
                                StringReader srTransDetail = new StringReader(Convert.ToString(cAGSS[3]));

                                if (Convert.ToString(cAGSS[3]).Trim().Length > 38)
                                {
                                    dsWH.ReadXml(srTransDetail);
                                }
                                if (dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                                {
                                    Dialog.WinFormsTouch.frmGenericSearch OSearch = new Dialog.WinFormsTouch.frmGenericSearch(dsWH.Tables[0], drSelected, "FPP Adjustment");
                                    OSearch.ShowDialog();
                                    drSelected = OSearch.SelectedDataRow;

                                    if (drSelected != null)
                                    {
                                        string sGSSNo = Convert.ToString(drSelected[0]);

                                        #region [call ts]

                                        try
                                        {
                                            if (PosApplication.Instance.TransactionServices.CheckConnection())
                                            {
                                                decimal dGSSTotQty = 0.00M;
                                                decimal dTotAmt = 0.00M;
                                                decimal dRoyaltyAmt = 0.00M;
                                                decimal dGSSAvgRate = 0.00M;
                                                int iSchemeDepositeType = 0;
                                                decimal dGSSTotCashAmt = 0.00M;

                                                ReadOnlyCollection<object> containerArray;

                                                containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GSSMaturityDetails", sGSSNo, DateTime.Now);

                                                dGSSTotQty = Convert.ToDecimal(containerArray[2]);
                                                dTotAmt = Convert.ToDecimal(containerArray[3]);
                                                dGSSAvgRate = Convert.ToDecimal(containerArray[4]);
                                                dRoyaltyAmt = Convert.ToDecimal(containerArray[5]);
                                                iSchemeDepositeType = Convert.ToInt16(containerArray[6]);
                                                dGSSTotCashAmt = Convert.ToDecimal(containerArray[7]);

                                                decimal dGSSTAXPCT = 0m;
                                                decimal dNewAvgRate = 0m;
                                                bool bGSSTaxApplicable = IsGSSEmiInclOfTax(ref dGSSTAXPCT);
                                                string ScCode = getSchemeCodeByGssAcc(sGSSNo);

                                                if (bGSSTaxApplicable && dGSSTAXPCT > 0)
                                                {
                                                    if (dGSSTotQty > 0)
                                                        dNewAvgRate = (dTotAmt - ((dTotAmt * dGSSTAXPCT) / (100 + dGSSTAXPCT))) / dGSSTotQty;
                                                    else
                                                        dNewAvgRate = decimal.Round(dGSSAvgRate, 2, MidpointRounding.AwayFromZero);


                                                    dGSSAvgRate = decimal.Round(dNewAvgRate, 2, MidpointRounding.AwayFromZero);
                                                }

                                                if (dTotAmt > 0)
                                                {
                                                    System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
                                                    if (saleline.Count == 0)//for adv and gss @ a time test purpose commented on 160418
                                                    {
                                                        string sGSSAdjItemId = "";
                                                        string sGSSDiscItemId = "";
                                                        string sGSSAmountAdjItemId = "";
                                                        string sGSSAmountDiscItemId = "";
                                                        int iGSSDiscAgreementBase = 0;

                                                        GSSAdjustmentItemID(ref sGSSAdjItemId, ref sGSSDiscItemId, ref sGSSAmountAdjItemId, ref sGSSAmountDiscItemId, ref iGSSDiscAgreementBase);

                                                        retailTrans.PartnerData.GSSMaturityNo = sGSSNo;
                                                        retailTrans.PartnerData.GSSSaleWt = 0;
                                                        retailTrans.PartnerData.GSSTotQty = dGSSTotQty;
                                                        retailTrans.PartnerData.GSSTotAmt = dTotAmt;
                                                        retailTrans.PartnerData.GSSAvgRate = dGSSAvgRate;
                                                        retailTrans.PartnerData.GSSRoyaltyAmt = dRoyaltyAmt;
                                                        retailTrans.PartnerData.SchemeDepositeType = iSchemeDepositeType;
                                                        retailTrans.PartnerData.GSSSchemeCode = ScCode;

                                                        if (iSchemeDepositeType == 0)//existing/old is type gold
                                                        {
                                                            Application.RunOperation(PosisOperations.ItemSale, sGSSAdjItemId);
                                                            if (iGSSDiscAgreementBase == 0)//added on 310119 for PNG
                                                            {
                                                                if (dRoyaltyAmt > 0)
                                                                {
                                                                    Application.RunOperation(PosisOperations.ItemSale, sGSSDiscItemId);
                                                                }
                                                            }
                                                        }
                                                        else//amount changed on 18102017
                                                        {
                                                            Application.RunOperation(PosisOperations.ItemSale, sGSSAmountAdjItemId);
                                                            if (iGSSDiscAgreementBase == 0)
                                                            {
                                                                if (dRoyaltyAmt > 0)
                                                                {
                                                                    Application.RunOperation(PosisOperations.ItemSale, sGSSAmountDiscItemId);
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Transaction Service not found", MessageBoxButtons.OK, MessageBoxIcon.Information))
                                                {
                                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                                }
                                            }
                                        }
                                        catch (Exception ex)
                                        {
                                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Transaction Service not found", MessageBoxButtons.OK, MessageBoxIcon.Information))
                                            {
                                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                            }
                                        }

                                        #endregion
                                    }

                                }
                                else
                                {
                                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No valid FPP found for adjustment.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                    {
                                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    }
                                    return;
                                }
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Real time service not working.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                                return;
                            }
                        }
                        catch (Exception ex)
                        {

                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer not found", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        return;
                    }
                    break;
                    #endregion
                case "SALESPERSON":
                    #region SALESPERSON
                    if (retailTrans != null)
                    {
                        if (retailTrans.SaleItems.Count == 0)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            return;
                        }
                        foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                        {
                            if (Convert.ToString(operationInfo.ItemLineId) == Convert.ToString(saleLineItem.LineId))
                            {
                                LSRetailPosis.POSProcesses.frmSalesPerson dialog = new LSRetailPosis.POSProcesses.frmSalesPerson();
                                dialog.ShowDialog();
                                saleLineItem.SalesPersonId = dialog.SelectedEmployeeId;
                                saleLineItem.SalespersonName = dialog.SelectEmployeeName;
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        return;
                    }
                    break;
                    #endregion
                case "UNLOCKSKU":
                    #region Unlock SKU
                    if (retailTrans == null)
                    {
                        DataTable dtTrans = new DataTable();
                        string sOpTerminals = string.Empty;

                        // Application.RunOperation(PosisOperations.RecallTransaction, "0000000001");

                        dt = Application.BusinessLogic.SuspendRetrieveSystem.RetrieveTransactionList(ApplicationSettings.Terminal.StoreId);

                        string sQry = "SELECT isnull(TERMINALID,'') as TERMINALID FROM RETAILTRANSACTION WHERE STOREID = '" + ApplicationSettings.Terminal.StoreId + "'" +
                                      " AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "' AND TERMINALID <> '" + ApplicationSettings.Terminal.TerminalId + "'";

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        SqlCommand cmd = new SqlCommand(sQry, connection);
                        cmd.CommandTimeout = 0;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dtTrans);

                        if (dtTrans != null && dtTrans.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtTrans.Rows)
                            {
                                sOpTerminals = sOpTerminals + " : " + Convert.ToString(dr[0]);
                            }

                            string sValidMsg = "Please close the Terminal operation (" + sOpTerminals + ") properly before unlock";
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(sValidMsg, MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                        else
                        {
                            SKUUnlock objSKUUnlock = new SKUUnlock(connection, dt);
                            objSKUUnlock.ShowDialog();
                        }
                    }
                    break;
                    #endregion
                case "REPORT":
                    #region REPORT
                    frmSearchOrder ofrmSearch = new frmSearchOrder(posTransaction, application, string.Empty);
                    ofrmSearch.ShowDialog();
                    string ordernum = ofrmSearch.sOrderNo;
                    if (!string.IsNullOrEmpty(ordernum))
                    {
                        DataSet ds = GetCustomerOrderData(ordernum);

                        FormModulation formMod = new FormModulation(Application.Settings.Database.Connection);
                        RetailTransaction retailTransaction = null;

                        FormInfo formInfo = formMod.GetInfoForForm(Microsoft.Dynamics.Retail.Pos.Contracts.Services.FormType.Unknown, false, LSRetailPosis.Settings.HardwareProfiles.Printer.ReceiptProfileId);
                        formMod.GetTransformedTransaction(formInfo, retailTransaction, ds);

                        string textForPreview = formInfo.Header;
                        textForPreview += formInfo.Details;
                        textForPreview += formInfo.Footer;
                        textForPreview = textForPreview.Replace("|1C", string.Empty);
                        textForPreview = textForPreview.Replace("|2C", string.Empty);
                        frmReportList preview = new frmReportList(textForPreview, null);
                        //  preview.ShowDialog();

                        this.Application.ApplicationFramework.POSShowForm(preview);
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Order Exists.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        return;
                    }
                    break;
                    #endregion
                case "SKUDETAILS":
                    #region SKUDETAILS
                    string lsItems = string.Empty;
                    if (Convert.ToString(posTransaction.GetType().Name).ToUpper().Trim() == "CUSTOMERPAYMENTTRANSACTION")
                    {
                        if (string.IsNullOrEmpty(Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData)))
                        {
                            CustomerDepositSKUDetails oSkuDetails = new CustomerDepositSKUDetails(500, posTransaction, Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo));
                            oSkuDetails.ShowDialog();

                            lsItems = oSkuDetails.lsitems;
                            DataTable dtSKU = oSkuDetails.dtSkuGridItems;
                            if (dtSKU != null && dtSKU.Rows.Count > 0)
                            {
                                dtSKU.TableName = "Ingredients";

                                MemoryStream mstr = new MemoryStream();
                                dtSKU.WriteXml(mstr, true);
                                mstr.Seek(0, SeekOrigin.Begin);
                                StreamReader sr = new StreamReader(mstr);
                                string sXML = string.Empty;
                                sXML = sr.ReadToEnd();
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData = sXML;
                            }
                            else
                            {
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData = string.Empty;
                            }
                        }
                        else
                        {
                            DataSet ds = new DataSet();
                            StringReader reader = new StringReader(Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData));
                            ds.ReadXml(reader);
                            CustomerDepositSKUDetails oSkuDetails = new CustomerDepositSKUDetails(ds.Tables[0], 500, posTransaction, Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.OrderNo));
                            oSkuDetails.ShowDialog();
                            lsItems = oSkuDetails.lsitems;
                            DataTable dtSKU = oSkuDetails.dtSkuGridItems;
                            if (dtSKU != null && dtSKU.Rows.Count > 0)
                            {
                                dtSKU.TableName = "Ingredients";

                                MemoryStream mstr = new MemoryStream();
                                dtSKU.WriteXml(mstr, true);
                                mstr.Seek(0, SeekOrigin.Begin);
                                StreamReader sr = new StreamReader(mstr);
                                string sXML = string.Empty;
                                sXML = sr.ReadToEnd();
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData = sXML;
                            }
                            else
                            {
                                ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.SKUData = string.Empty;
                            }
                        }
                        ((LSRetailPosis.Transaction.CustomerPaymentTransaction)(posTransaction)).PartnerData.ItemIds = lsItems;
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Customer Deposit Found.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                    break;
                    #endregion
                case "SALESDETAILS":
                    #region SALESDETAILS
                    //RetailTransaction retailTrans = posTransaction as RetailTransaction;
                    if (retailTrans != null)
                    {
                        if (retailTrans.PartnerData.SingaporeTaxCal == "0")
                        {
                            if (connection.State == ConnectionState.Closed)
                                connection.Open();

                            foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                            {
                                if (Convert.ToString(operationInfo.ItemLineId) == Convert.ToString(saleLineItem.LineId))
                                {

                                    try
                                    {
                                        DataSet dsIngredients = new DataSet();
                                        if (!string.IsNullOrEmpty(Convert.ToString(saleLineItem.PartnerData.Ingredients)) && Convert.ToString(saleLineItem.PartnerData.Ingredients) != "0")
                                        {
                                            StringReader reader = new StringReader(Convert.ToString(saleLineItem.PartnerData.Ingredients));
                                            dsIngredients.ReadXml(reader);
                                        }
                                        else
                                        {
                                            dsIngredients = null;
                                        }
                                        frmCustomFieldCalculations oCustomCalc =
                                            new frmCustomFieldCalculations(
                                                Convert.ToString(saleLineItem.PartnerData.Pieces),
                                                Convert.ToString(saleLineItem.PartnerData.Quantity),
                                                Convert.ToString(saleLineItem.PartnerData.Rate),
                                                Convert.ToString(saleLineItem.PartnerData.RateType),
                                                Convert.ToString(saleLineItem.PartnerData.MakingRate),
                                                Convert.ToString(saleLineItem.PartnerData.MakingType),
                                                Convert.ToString(saleLineItem.PartnerData.Amount),
                                                Convert.ToString(saleLineItem.PartnerData.MakingDisc),
                                                Convert.ToString(saleLineItem.PartnerData.MakingAmount),
                                                Convert.ToString(saleLineItem.PartnerData.TotalAmount),
                                                Convert.ToString(saleLineItem.PartnerData.TotalWeight),
                                                Convert.ToString(saleLineItem.PartnerData.LossPct),
                                                Convert.ToString(saleLineItem.PartnerData.LossWeight),
                                                Convert.ToString(saleLineItem.PartnerData.ExpectedQuantity),
                                                Convert.ToString(saleLineItem.PartnerData.TransactionType),
                                                Convert.ToString(saleLineItem.PartnerData.OChecked), dsIngredients,
                                                Convert.ToString(saleLineItem.PartnerData.OrderNum),
                                                Convert.ToString(saleLineItem.PartnerData.OrderLineNum),
                                                Convert.ToString(saleLineItem.PartnerData.WastageType),
                                                Convert.ToString(saleLineItem.PartnerData.WastagePercentage),
                                                Convert.ToString(saleLineItem.PartnerData.WastageQty),
                                                Convert.ToString(saleLineItem.PartnerData.WastageAmount),
                                                Convert.ToString(saleLineItem.PartnerData.MakingDiscountType),
                                                Convert.ToString(saleLineItem.PartnerData.MakingTotalDiscount),
                                                Convert.ToString(saleLineItem.PartnerData.Purity),
                                                Convert.ToString(saleLineItem.PartnerData.RATESHOW),
                                                Convert.ToString(saleLineItem.PartnerData.ACTTOTAMT),
                                                Convert.ToString(saleLineItem.PartnerData.ACTMKRATE),
                                                Convert.ToString(saleLineItem.ItemId),
                                                Convert.ToString(saleLineItem.PartnerData.LINEDISC),
                                                saleLineItem,
                                                retailTrans,
                                                connection, this.application);

                                        oCustomCalc.ShowDialog();
                                        oCustomCalc.txtLineDisc.Focus();

                                        Microsoft.Dynamics.Retail.Pos.DiscountService.Discount objDisc = new Microsoft.Dynamics.Retail.Pos.DiscountService.Discount();
                                        if (retailTrans != null)
                                        {
                                            retailTrans.PartnerData.ReCalculate = true;

                                            objDisc.Application = this.application;

                                            #region Recalculate Tax
                                            //decimal dNewTaxPerc = saleLineItem.PartnerData.NewTaxPerc;
                                            //if(dNewTaxPerc>0)
                                            //    saleLineItem.TaxRatePct = dNewTaxPerc;


                                            //TaxService.Tax objTax = new TaxService.Tax();

                                            //if (application != null)
                                            //    objTax.Application = application;
                                            //else
                                            //    objTax.Application = Application;

                                            //objTax.Initialize();
                                            //objTax.CalculateTax(saleLineItem, retailTrans);
                                            #endregion

                                            objDisc.CalculateDiscount(retailTrans);
                                            // saleLineItem.CalculateLine();
                                            retailTrans.CalcTotals();
                                            retailTrans.CalculateAmountDue();
                                        }

                                        saleLineItem.PriceOverridden = false;
                                    }
                                    catch (RuntimeBinderException)
                                    {
                                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Details Present for this item.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                                        {
                                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show("Already recalculation of tax is done.");
                            return;
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                    break;
                    #endregion
                case "RELEASESKU":
                    #region RELEASESKU
                    frmSKURelease oRelease = new frmSKURelease(ReleaseSKU(), null, "SKU RELEASE FORM", application);
                    Application.ApplicationFramework.POSShowForm(oRelease);
                    break;
                    #endregion
                case "MULTIPLEADJUSTMENT":
                    #region Multiple Adjustment
                    if (retailTrans != null)
                    {
                        if (operationInfo.ReturnItems)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Adjustment Cannot be returned.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            return;
                        }
                        bool isServiceItemExists = false;

                        System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
                        if (saleline.Count == 0)
                        {
                            retailTrans.PartnerData.TransAdjGoldQty = 0;
                            string sTableName = "ORDERINFO" + "" + ApplicationSettings.Database.TerminalID;

                            dt = GetCustomerMultiAdjdada(retailTrans, dt);

                            dt = RemoveDuplicateRows(dt, "OrderNum");

                            if (dt != null && dt.Rows.Count > 0)
                            {
                                Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dt, null, "Customer deposits");
                                this.Application.ApplicationFramework.POSShowForm(oSearch);
                                DataRow dr = null;
                                dr = oSearch.SelectedDataRow;
                                if (dr != null)
                                {

                                    DataTable dtBookedSKU = new DataTable();

                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["ORDERNUM"])))
                                    {
                                        dtBookedSKU = BookedSKU(Convert.ToString(dr["ORDERNUM"]), Convert.ToString(dr["CUSTACCOUNT"]));
                                    }

                                    bool bIsSuvarnaVridhi = IsCustOrderSUVARNAVRUDHI(Convert.ToString(dr["ORDERNUM"]));
                                    DateTime dateReqDel = getRequestDeliveryDate(Convert.ToString(dr["ORDERNUM"]));

                                    if (bIsSuvarnaVridhi && dateReqDel > Convert.ToDateTime(DateTime.Now.ToShortDateString()))
                                    {
                                        MessageBox.Show("This order can not be adjusted before " + dateReqDel.ToShortDateString() + "");
                                    }
                                    else
                                    {
                                        if (dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
                                        {
                                            retailTrans.PartnerData.SKUBookedItems = true;
                                            retailTrans.PartnerData.SKUBookedItemsExists = "Y";
                                        }
                                        retailTrans.PartnerData.AdjustmentOrderNum = string.IsNullOrEmpty(Convert.ToString(dr["ORDERNUM"])) ? string.Empty : Convert.ToString(dr["ORDERNUM"]);
                                        retailTrans.PartnerData.AdjustmentCustAccount = string.IsNullOrEmpty(Convert.ToString(dr["CUSTACCOUNT"])) ? string.Empty : Convert.ToString(dr["CUSTACCOUNT"]);
                                        AdjustmentOrderNum = string.IsNullOrEmpty(Convert.ToString(dr["ORDERNUM"])) ? string.Empty : Convert.ToString(dr["ORDERNUM"]);
                                        AdjustmentCustAccount = string.IsNullOrEmpty(Convert.ToString(dr["CUSTACCOUNT"])) ? string.Empty : Convert.ToString(dr["CUSTACCOUNT"]);

                                        DropTempTable(sTableName);
                                        getMultiAdjOrderInfo(sTableName, AdjustmentOrderNum, AdjustmentCustAccount);

                                        string sGMAAdvItem = "";
                                        string sGMALGItem = "";
                                        string sGMADiscItem = "";
                                        string sGMAGSTItem = "";
                                        decimal dGMAAdv = 0m;


                                        GMAAdjustmentItemID(ref sGMAAdvItem, ref sGMAGSTItem, ref sGMALGItem, ref sGMADiscItem);

                                        if (IsGMA(AdjustmentOrderNum, ref dGMAAdv))
                                        {
                                            if (dateReqDel > Convert.ToDateTime(DateTime.Now.ToShortDateString()))
                                            {
                                                MessageBox.Show("This GMA can not be adjusted before " + dateReqDel.ToShortDateString() + "");
                                            }
                                            else
                                            {
                                                retailTrans.PartnerData.GMATotAdvance = dGMAAdv;
                                                retailTrans.PartnerData.GMAAdjustment = true;
                                                Application.RunOperation(PosisOperations.ItemSale, sGMAAdvItem);
                                            }
                                        }
                                        else
                                        {
                                            #region For order adj
                                            //if (IsCustOrderWithAdv(AdjustmentOrderNum))
                                            //{
                                            DataTable dtNew = new DataTable();
                                            dtNew = GetMultiUnadjustedAdvData(retailTrans, dtNew);
                                            //  dtNew = CustomerAdvanceData(retailTrans.Customer.CustomerId);

                                            if (dtNew != null && dtNew.Rows.Count > 0)
                                            {
                                                foreach (DataRow drNew in dtNew.Select("ORDERNUM='" + Convert.ToString(dr["ORDERNUM"]) + "' AND  CUSTACCOUNT='" + Convert.ToString(dr["CUSTACCOUNT"]) + "'"))
                                                {
                                                    Application.RunOperation(PosisOperations.ItemSale, AdjustmentItemID());
                                                }
                                            }
                                            // }


                                            DataRow drOrd = null;
                                            drOrd = oSearch.SelectedDataRow;
                                            if (dr != null)
                                            {
                                                if (dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
                                                {
                                                    retailTrans.PartnerData.SKUBookedItems = true;

                                                    foreach (DataRow drNew in dtBookedSKU.Rows)
                                                    {
                                                        DropTempTable(sTableName);
                                                        // string sTableName1 = "ORDERINFO" + "" + ApplicationSettings.Database.TerminalID;
                                                        getMultiAdjOrderInfo(sTableName, AdjustmentOrderNum, AdjustmentCustAccount, Convert.ToInt16(drNew["LineNum"]));

                                                        Application.RunOperation(PosisOperations.ItemSale, Convert.ToString(drNew["SKUNUMBER"]));
                                                    }
                                                    retailTrans.PartnerData.SKUBookedItems = false;
                                                }
                                                else
                                                {
                                                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Booked SKU found for the customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                                    {
                                                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                                    }
                                                    DropTempTable(sTableName);
                                                }
                                            }
                                            #endregion
                                        }
                                    }
                                }

                                //return;
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Active deposits for this customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                            }
                        }
                        if (saleline.Count > 0)
                        {
                            foreach (var sale in saleline)
                            {
                                if (sale.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                {
                                    isServiceItemExists = true;
                                }
                            }
                            if (!isServiceItemExists)
                            {
                                // dt = CustomerAdjustment(retailTrans.Customer.CustomerId);
                                dt = GetCustomerMultiAdjdada(retailTrans, dt);
                                if (dt != null && dt.Rows.Count > 0)
                                {
                                    Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dt, null, "Customer Deposit");
                                    this.Application.ApplicationFramework.POSShowForm(oSearch);
                                    DataRow dr = null;
                                    dr = oSearch.SelectedDataRow;
                                    DataTable dtBookedSKU = new DataTable();
                                    if (dr != null)
                                    {
                                        bool bIsSuvarnaVridhi = IsCustOrderSUVARNAVRUDHI(Convert.ToString(dr["ORDERNUM"]));
                                        DateTime dateReqDel = getRequestDeliveryDate(Convert.ToString(dr["ORDERNUM"]));

                                        if (bIsSuvarnaVridhi && dateReqDel > Convert.ToDateTime(DateTime.Now.ToShortDateString()))
                                        {
                                            MessageBox.Show("This order can not be adjusted before " + dateReqDel.ToShortDateString() + "");
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(Convert.ToString(dr["ORDERNUM"])))
                                            {
                                                dtBookedSKU = BookedSKU(Convert.ToString(dr["ORDERNUM"]), Convert.ToString(dr["CUSTACCOUNT"]));
                                            }
                                            if (dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
                                            {
                                                retailTrans.PartnerData.SKUBookedItems = true;
                                                retailTrans.PartnerData.SKUBookedItemsExists = "Y";
                                            }

                                            retailTrans.PartnerData.AdjustmentOrderNum = string.IsNullOrEmpty(Convert.ToString(dr["ORDERNUM"])) ? string.Empty : Convert.ToString(dr["ORDERNUM"]);
                                            retailTrans.PartnerData.AdjustmentCustAccount = string.IsNullOrEmpty(Convert.ToString(dr["CUSTACCOUNT"])) ? string.Empty : Convert.ToString(dr["CUSTACCOUNT"]);
                                            DataTable dtNew = new DataTable();
                                            //dtNew = CustomerAdvanceData(retailTrans.Customer.CustomerId);
                                            AdjustmentOrderNum = string.IsNullOrEmpty(Convert.ToString(dr["ORDERNUM"])) ? string.Empty : Convert.ToString(dr["ORDERNUM"]);
                                            AdjustmentCustAccount = string.IsNullOrEmpty(Convert.ToString(dr["CUSTACCOUNT"])) ? string.Empty : Convert.ToString(dr["CUSTACCOUNT"]);

                                            string sTableName = "ORDERINFO" + "" + ApplicationSettings.Database.TerminalID;
                                            getMultiAdjOrderInfo(sTableName, AdjustmentOrderNum, AdjustmentCustAccount);

                                            dtNew = GetMultiUnadjustedAdvData(retailTrans, dtNew);
                                            foreach (DataRow drNew in dtNew.Select("ORDERNUM='" + Convert.ToString(dr["ORDERNUM"]) + "' AND  CUSTACCOUNT='" + Convert.ToString(dr["CUSTACCOUNT"]) + "'"))
                                            {
                                                Application.RunOperation(PosisOperations.ItemSale, AdjustmentItemID());
                                            }

                                            DataRow drOrd = null;
                                            drOrd = oSearch.SelectedDataRow;
                                            if (dr != null)
                                            {
                                                if (dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
                                                {
                                                    retailTrans.PartnerData.SKUBookedItems = true;
                                                    foreach (DataRow drNew in dtBookedSKU.Rows)
                                                    {
                                                        Application.RunOperation(PosisOperations.ItemSale, Convert.ToString(drNew["SKUNUMBER"]));
                                                    }
                                                    retailTrans.PartnerData.SKUBookedItems = false;
                                                }
                                                else
                                                {
                                                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Booked SKU found for the customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                                    {
                                                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                                    }
                                                    DropTempTable(sTableName);
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Active deposits for this customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                    {
                                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    }
                                }

                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount can only be adjusted in the beginning of transaction.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Amount cannot be adjusted without any customer. Please select the customer to adjust the Amount.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }

                    }

                    break;
                    #endregion
                case "STOCKTRANSFER":
                    #region STOCK TRANSFER
                    frmGetArticleWiseStock objFrmStokTrans = new frmGetArticleWiseStock(posTransaction, application);
                    objFrmStokTrans.ShowDialog();
                    break;
                    #endregion
                case "GSSAC":
                    #region GSSAcOpenning
                    frmGSSAcOpenning objGss = new frmGSSAcOpenning();
                    objGss.ShowDialog();
                    break;
                    #endregion
                case "LOCALCUST":
                    #region Local Customer
                    if (retailTrans != null)
                    {
                        if (string.IsNullOrEmpty(retailTrans.Customer.CustomerId))
                        {
                            frmAddLocalCustomer objAddLocalCustomer = new frmAddLocalCustomer(retailTrans);
                            objAddLocalCustomer.ShowDialog();
                        }
                        else
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Clear selected customer", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        return;
                    }
                    break;
                    #endregion
                case "TRANSACTIONREPORT":
                    #region Transaction Report
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    TransReportFrm reportfrm = new TransReportFrm(connection);
                    reportfrm.ShowDialog();
                    break;
                    #endregion
                case "OGPREPORT":
                    #region OGP Report
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    frmR_OldGoldPurchase reportOGP = new frmR_OldGoldPurchase(connection);
                    reportOGP.ShowDialog();
                    break;
                    #endregion
                case "CWSTKREPORT":
                    #region Counter wise Stock Report
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    frmReportInput frmInp2 = new frmReportInput(connection, 1);// CounterWiseStkReport
                    frmInp2.ShowDialog();

                    break;
                    #endregion
                case "CWSTKREPORTDTL":
                    #region Counter wise Stock Detail Report
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    frmReportInput frmInp1 = new frmReportInput(connection, 2);// CounterWiseStkDtlReport
                    frmInp1.ShowDialog();
                    break;
                    #endregion
                case "SALEINV":
                    #region Sales Inv Report
                    Application.RunOperation(PosisOperations.ShowJournal, string.Empty);
                    break;
                    #endregion
                case "CASHREGISTER":
                    #region Cash Register
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    frmCashRegister objCashRegister = new frmCashRegister();
                    objCashRegister.ShowDialog();
                    break;
                    #endregion
                case "TRANSORDRPRINT":
                    #region Print Transfer Order
                    frmTransferOrderPrint objTransOrdrPrint = new frmTransferOrderPrint();
                    objTransOrdrPrint.ShowDialog();
                    break;
                    #endregion
                case "METALRATE":
                    #region Metal Rate
                    frmDisplayMetalRate objMetalRate = new frmDisplayMetalRate();
                    objMetalRate.ShowDialog();
                    break;
                    #endregion
                case "GSSINSREC"://Start: RHossain on 18/09/2013
                    #region GSS INSTALLMENT RECEIPT REPORT
                    if (retailTrans != null)
                    {
                        string strSql = " select TRANSACTIONID, CUSTACCOUNT,GSSNUMBER , CAST(ISNULL(AMOUNT,0) AS DECIMAL(28,2))" +
                                          " AS AMOUNT, CAST(TERMINALID AS NVARCHAR(20)) AS TERMINALID from RETAILDEPOSITTABLE where isnull(GSSNUMBER,'') !=''" +
                                          " AND (CUSTACCOUNT = '" + retailTrans.Customer.CustomerId + "') ";

                        dt = getPaymentTransData(strSql);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dt, null, "FPP Instalment receipt");
                            oSearch.ShowDialog();
                            DataRow dr = null;
                            dr = oSearch.SelectedDataRow;
                            if (dr != null)
                            {
                                //string ScCode = getSchemeCodeByGssAcc(Convert.ToString(dr["GSSNUMBER"])); 
                                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                                frmR_GSSInstalmentReceipt objRGSS = new frmR_GSSInstalmentReceipt(posTransaction, SqlCon, Convert.ToString(dr["transactionid"]), Convert.ToString(dr["amount"]), Convert.ToString(dr["GSSNUMBER"]), Convert.ToString(dr["TERMINALID"]), 0);
                                objRGSS.ShowDialog();
                            }
                        }
                    }
                    break;
                    #endregion
                //End: RHossain on 20/09/2013  
                case "PRODADVANCEREC": // PRODUCT ADVANCE RECEIPT REPORT //Start: RHossain on 20/09/2013
                    #region PRODUCT ADVANCE RECEIPT REPORT
                    if (retailTrans != null)
                    {
                        string strSql = " select CAST(TRANSACTIONID AS NVARCHAR(30)) AS TRANSACTIONID, CAST(CUSTACCOUNT AS NVARCHAR(30)) AS CUSTACCOUNT, CAST(CAST(ISNULL(AMOUNT,0) AS DECIMAL(28,2)) AS NVARCHAR(50))" +
                                         " AS AMOUNT, CAST(TERMINALID AS NVARCHAR(20)) AS TERMINALID from RETAILDEPOSITTABLE where isnull(GSSNUMBER,'') =''" +
                                         " AND (CUSTACCOUNT = '" + retailTrans.Customer.CustomerId + "') ORDER BY TRANSACTIONID DESC";

                        dt = getPaymentTransData(strSql);
                        if (dt != null && dt.Rows.Count > 0)
                        {
                            Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dt, null, "Product advance receipt");
                            oSearch.ShowDialog();
                            DataRow dr = null;
                            dr = oSearch.SelectedDataRow;
                            if (dr != null)
                            {
                                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                                frmR_ProductAdvanceReceipt objProdAdv = new frmR_ProductAdvanceReceipt(posTransaction, SqlCon, Convert.ToString(dr["transactionid"]),
                                                Convert.ToString(dr["amount"]), Convert.ToString(dr["TERMINALID"]));
                                objProdAdv.ShowDialog();
                            }
                        }
                    }
                    break;
                    #endregion
                case "OGREPORT"://Start: RHossain on 16/04/2014
                    #region OGReport
                    frmOGReport objOg = new frmOGReport(1); //1 for OG Report
                    objOg.ShowDialog();
                    break;
                    #endregion
                //End: RHossain on 16/04/2014
                case "SALESREPORT":
                    #region SalesReport
                    frmOGReport objOGR = new frmOGReport(2);//2 for Sales Report
                    objOGR.ShowDialog();
                    break;
                    #endregion
                case "COUNTERMOVEMENT":
                    #region CounterMovementReport
                    frmOGReport objOGR2 = new frmOGReport(3);//2 for COUNTER MOVEMENT
                    objOGR2.ShowDialog();
                    break;
                    #endregion
                case "GSSACCSTATEMENT":
                    #region GSS Account Statement
                    frmGSSAccStatment objGSSACST = new frmGSSAccStatment();
                    objGSSACST.ShowDialog();
                    break;
                    #endregion
                case "SKUBOOKINGREPORT"://Start: RHossain on 6/05/2014
                    #region SKU Booking Report
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    frmR_SKUBookingReport objSKUBR = new frmR_SKUBookingReport(connection);
                    objSKUBR.ShowDialog();
                    break;
                    #endregion
                case "CUSTOMERFOOTFALL":
                    #region CUSTOMERFOOTFALL
                    frmCustomerFootfall objCF = new frmCustomerFootfall(posTransaction, application);
                    objCF.ShowDialog();
                    break;
                    #endregion
                //End: RHossain on 06/05/2014
                case "SPLINEDISC":
                    #region SPLINEDISC
                    //if (retailTrans != null)
                    //{
                    //    if (Convert.ToString(retailTrans.Customer.CustomerId) == string.Empty)
                    //    {
                    //        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select customer", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    //        {
                    //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    //        }
                    //        operationInfo.OperationHandled = false;
                    //    }
                    //    else
                    //    {
                    //        if (operationInfo.ReturnItems)
                    //        {
                    //            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Discount cannot be apply.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    //            {
                    //                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    //            }
                    //            operationInfo.OperationHandled = false;
                    //        }
                    //        bool isServiceItemExists = false;
                    //        System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);

                    //        if (saleline.Count > 0)
                    //        {
                    //            foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                    //            {
                    //                if (Convert.ToString(operationInfo.ItemLineId) == Convert.ToString(saleLineItem.LineId))
                    //                {
                    //                    saleLineItem.PartnerData.IsSpecialDisc = true;
                    //                }
                    //            }

                    //            Application.RunOperation(PosisOperations.LineDiscountPercent, "");
                    //        }

                    //    }
                    //}
                    break;
                    #endregion
                case "SETSKUSALE":
                    #region SETSKUSALE
                    if (retailTrans != null)
                    {
                        if (Convert.ToString(retailTrans.Customer.CustomerId) == string.Empty)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select customer", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            operationInfo.OperationHandled = false;
                        }
                        else
                        {
                            frmSETSKUScan objSETSKU = new frmSETSKUScan(application);
                            objSETSKU.ShowDialog();

                            DataTable dtSetSKU = new DataTable();
                            dtSetSKU = objSETSKU.dtSETSku;

                            if (dtSetSKU != null) // add 26/09/2014 RH
                            {
                                foreach (DataRow dr in dtSetSKU.Rows)
                                {
                                    if (!string.IsNullOrEmpty(Convert.ToString(dr["SKUNUMBER"])))
                                        Application.RunOperation(PosisOperations.ItemSale, Convert.ToString(dr["SKUNUMBER"]));
                                }
                            }

                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select customer", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        operationInfo.OperationHandled = false;
                    }
                    break;
                    #endregion
                case "APPLYFREEGIFTITEM":
                    #region APPLYFREEGIFTITEM
                    if (retailTrans != null)
                    {
                        //Start: added on 241117
                        bool bFullFreeGiftApplid = false;
                        decimal dTotFreeQtyApplied = 0m;

                        System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
                        if (saleline.Count > 0)
                        {
                            foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                            {
                                if (!saleLineItem.Voided)
                                {
                                    if (saleLineItem.NetAmount == 0)
                                        dTotFreeQtyApplied += decimal.Round((Convert.ToDecimal(saleLineItem.PartnerData.Quantity)), 3, MidpointRounding.AwayFromZero);


                                    if (!string.IsNullOrEmpty(retailTrans.PartnerData.FreeGiftQTY))
                                    {
                                        if (dTotFreeQtyApplied == Convert.ToDecimal(retailTrans.PartnerData.FreeGiftQTY))
                                            bFullFreeGiftApplid = true;
                                        else
                                            bFullFreeGiftApplid = false;
                                    }
                                }
                            }
                        }
                        //End: added on 241117

                        if (!bFullFreeGiftApplid)//string.IsNullOrEmpty(retailTrans.PartnerData.FreeGiftQTY)
                        {
                            if (Convert.ToString(retailTrans.Customer.CustomerId) == string.Empty)
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select customer", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                                operationInfo.OperationHandled = false;
                            }
                            else
                            {
                                if (operationInfo.ReturnItems)
                                {
                                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Free gift item promo cannot be apply.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                    {
                                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    }
                                    operationInfo.OperationHandled = false;
                                }
                                bool isServiceItemExists = false;
                                Dictionary<string, decimal> dicSum = new Dictionary<string, decimal>();

                                DataTable dtItemInfo = new DataTable();
                                decimal dCWQTY = 0;
                                decimal dQTY = 0m;
                                string sItemId = string.Empty;
                                string sPromoCode = string.Empty;
                                string sConf = string.Empty;
                                string sSize = string.Empty;
                                string sColor = string.Empty;
                                string sFreeArticleCode = string.Empty;
                                string sFreeProductType = string.Empty;
                                string sStorGrp = getStoreFormatCode();


                                if (dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                                {
                                    dtItemInfo.Columns.Add("PRODUCTTYPE", typeof(string));
                                    dtItemInfo.Columns.Add("AMOUNT", typeof(decimal));
                                }
                                if (saleline.Count > 0)
                                {
                                    foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                                    {
                                        if (!saleLineItem.Voided && saleLineItem.NetAmount != 0
                                            && saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                            && saleLineItem.PartnerData.NimReturnLine == 0)
                                        {
                                            GetItemType(saleLineItem.ItemId);
                                            if (iOWNDMD == 0 && iOWNOG == 0 && iOTHERDMD == 0 && iOTHEROG == 0)
                                            {
                                                string sArticle = "";
                                                string sItemProductType = GetItemProductType(saleLineItem.ItemId, ref sArticle);
                                                DataRow dr;

                                                dr = dtItemInfo.NewRow();
                                                dr["PRODUCTTYPE"] = sItemProductType;
                                                if (saleLineItem.PartnerData.isMRP)
                                                    dr["AMOUNT"] = decimal.Round((Convert.ToDecimal(saleLineItem.Price)), 2, MidpointRounding.AwayFromZero);
                                                else
                                                    dr["AMOUNT"] = decimal.Round((Convert.ToDecimal(saleLineItem.PartnerData.TotalAmount)), 2, MidpointRounding.AwayFromZero);

                                                dtItemInfo.Rows.Add(dr);
                                            }
                                        }
                                    }

                                    foreach (DataRow row in dtItemInfo.Rows)
                                    {
                                        string group = row["PRODUCTTYPE"].ToString();
                                        decimal rate = Convert.ToDecimal(row["AMOUNT"]);
                                        if (dicSum.ContainsKey(group))
                                            dicSum[group] += rate;
                                        else
                                            dicSum.Add(group, rate);
                                    }

                                    DataTable table = new DataTable();

                                    table.Columns.Add("PRODUCTTYPE", typeof(string));
                                    table.Columns.Add("AMOUNT", typeof(decimal));
                                    foreach (string sProductType in dicSum.Keys)
                                    {
                                        table.Rows.Add(sProductType, dicSum[sProductType]);
                                    }

                                    if (table != null && table.Rows.Count > 0)
                                    {
                                        table.TableName = "FREEGIFTCON";

                                        MemoryStream mstr = new MemoryStream();
                                        table.WriteXml(mstr, true);
                                        mstr.Seek(0, SeekOrigin.Begin);
                                        StreamReader sr = new StreamReader(mstr);
                                        string sXML = string.Empty;
                                        sXML = sr.ReadToEnd();
                                        retailTrans.PartnerData.FREEGIFTCON = sXML;
                                    }
                                }

                                foreach (string sProductType in dicSum.Keys)
                                {
                                    GetFreeGiftInfo(Convert.ToDecimal(dicSum[sProductType]), sProductType,
                                        sStorGrp, ref dCWQTY, ref dQTY,
                                        ref  sPromoCode, ref  sItemId, ref sConf,
                                        ref sColor, ref sSize, ref sFreeProductType
                                        , ref sFreeArticleCode);

                                    retailTrans.PartnerData.FreeGiftCWQTY = Convert.ToString(dCWQTY);
                                    retailTrans.PartnerData.FreeGiftQTY = Convert.ToString(dQTY);
                                    retailTrans.PartnerData.FreeGiftPromoCode = sPromoCode;

                                    //if (dTotFreeQtyApplied == 0)
                                    //{
                                    //    sItemId = ShowPopUpForGiftItem(dTotFreeQtyApplied, sItemId, sConf, sSize, sColor, sFreeArticleCode, sFreeProductType);
                                    //    if (!string.IsNullOrEmpty(sItemId))
                                    //    {
                                    //        Application.RunOperation(PosisOperations.ItemSale, sItemId);
                                    //    }
                                    //}

                                    if (!string.IsNullOrEmpty(retailTrans.PartnerData.FreeGiftQTY))
                                    {
                                        if (dTotFreeQtyApplied != Convert.ToDecimal(retailTrans.PartnerData.FreeGiftQTY))
                                        {
                                            sItemId = ShowPopUpForGiftItem(dTotFreeQtyApplied, sItemId, sConf, sSize, sColor, sFreeArticleCode, sFreeProductType);

                                            if (!string.IsNullOrEmpty(sItemId))
                                            {
                                                Application.RunOperation(PosisOperations.ItemSale, sItemId);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Already applied,please void the transaction and try to do the same again if required.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                            operationInfo.OperationHandled = false;
                        }
                    }
                    break;
                    #endregion
                case "BULITEMRECEIVE":
                    #region BULITEMRECEIVE
                    frmBulkReceive objBIR = new frmBulkReceive(posTransaction, application);
                    objBIR.ShowDialog();
                    break;
                    #endregion
                case "BULITEMISSUE":
                    #region BULITEMISSUE
                    frmBulkItemIssue objBII = new frmBulkItemIssue(posTransaction, application);
                    objBII.ShowDialog();
                    break;
                    #endregion
                case "BULITEMSHIPMENT":
                    #region BULITEMISSUE
                    frmBulkItemShipment objBIS = new frmBulkItemShipment();
                    objBIS.ShowDialog();
                    break;
                    #endregion
                case "REPAIRINHOUSETRANSFER":
                    #region REPAIRINHOUSETRANSFER
                    frmRepairInHouseTransfer objrepInHouse = new frmRepairInHouseTransfer();
                    objrepInHouse.ShowDialog();
                    break;
                    #endregion
                case "REPAIRISSUERECEIVE":
                    #region REPAIRISSUERECEIVE
                    if (CheckTerminal(ApplicationSettings.Terminal.TerminalId))
                    {
                        //using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("If Issue then click YES else NO? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                        //{
                        //    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        frmYESNO frmYN = new frmYESNO("Issue to Karigar", "Receive from Karigar");
                        frmYN.ShowDialog();

                        if (frmYN.isY)
                        {
                            frmRepairIssueReceive objIssueRec = new frmRepairIssueReceive(posTransaction, application, 1);// 1 for Issue 
                            objIssueRec.ShowDialog();
                        }
                        else if (frmYN.isN)
                        {
                            frmRepairIssueReceive objIssueRec = new frmRepairIssueReceive(posTransaction, application, 2);//  2 for Receive
                            objIssueRec.ShowDialog();
                        }
                        //}

                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(" Terminal ID : " + ApplicationSettings.Terminal.TerminalId + " set up not completed.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            operationInfo.OperationHandled = false;
                        }
                    }
                    break;
                    #endregion
                case "GETESTIMATION":
                    #region GETESTIMATION
                    frmGetEstimateion objEST = new frmGetEstimateion(posTransaction, application);
                    objEST.ShowDialog();
                    break;
                    #endregion
                case "ARTICLEWISESTOCK":
                    #region ARTICLEWISESTOCK
                    frmArticleWiseStockCount obaws = new frmArticleWiseStockCount(posTransaction, application);
                    obaws.ShowDialog();
                    break;
                    #endregion
                case "AWCSSUMMERY":
                    #region AWCSSUMMERY
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    frmAWCSReport oAW = new frmAWCSReport("", connection);
                    oAW.ShowDialog();
                    break;
                    #endregion
                case "GOLDSTOCKSUMMARY":
                    #region GOLDSTOCKSUMMARY
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    frmInputForGoldStockReport oGS = new frmInputForGoldStockReport(connection);
                    oGS.ShowDialog();
                    break;
                    #endregion
                case "GOLDSTOCKDETAILS":
                    #region GOLDSTOCKDETAILS
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    frmInputForGoldStockReport oSD = new frmInputForGoldStockReport(connection, 1); // 1 FOR DETAILS
                    oSD.ShowDialog();
                    break;
                    #endregion
                case "FETCHCUSTOMER"://+919231578421
                    #region FETCHCUSTOMER
                    frmFetchCustomerFromHO objFetchCust = new frmFetchCustomerFromHO();
                    objFetchCust.ShowDialog();
                    break;
                    #endregion FETCHCUSTOMER
                case "FETCHITEM":
                    #region FETCHITEM
                    frmFetchItem objFetchItem = new frmFetchItem(posTransaction, application);
                    objFetchItem.ShowDialog();
                    break;
                    #endregion FETCHITEM
                case "CURRENCYDECLARATION":
                    #region CURRENCYDECLARATION
                    frmForeignDenomination obfcd = new frmForeignDenomination(posTransaction, application);
                    obfcd.ShowDialog();
                    break;
                    #endregion
                case "RSTSTARTSTOP":
                    #region RSTSTARTSTOP
                    frmRetailStockCounting obrs = new frmRetailStockCounting(posTransaction, application);
                    obrs.ShowDialog();
                    break;
                    #endregion
                case "RETAILSTOCKCOUNTING":
                    #region RETAILSTOCKCOUNTING
                    frmRetailStockCounting1 obrsc = new frmRetailStockCounting1(posTransaction, application);
                    obrsc.ShowDialog();
                    break;
                    #endregion
                case "DASHBOARD":
                    #region DASHBOARD
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    frmDateFilter odf = new frmDateFilter(connection);
                    odf.ShowDialog();
                    break;
                    #endregion
                case "STOCKVERIFICATION":
                    #region Counter wise Stock Report
                    frmStockVerificationInput frmsv = new frmStockVerificationInput();// CounterWiseStkReport
                    frmsv.ShowDialog();
                    break;
                    #endregion
                case "CHANGEPASSWORD":
                    #region CHANGEPASSWORD
                    if (retailTrans == null)
                    {
                        frmChangePassword objCP = new frmChangePassword(posTransaction, application);
                        objCP.ShowDialog();
                    }
                    else
                    {
                        MessageBox.Show("Please complete the running transaction.");
                    }
                    break;
                    #endregion
                case "RECALTAX":  //
                    #region For Singapore only
                    if (retailTrans != null)
                    {
                        bool bIsValidClick = false;
                        foreach (SaleLineItem SLineItem in retailTrans.SaleItems) // checking bIsValidClick
                        {
                            if (!SLineItem.Voided)
                            {
                                #region If excahnge then  For Singapur only
                                if (retailTrans.IncomeExpenseAmounts == 0
                                    && retailTrans.SalesInvoiceAmounts == 0)
                                {
                                    if (SLineItem.PartnerData.isFullReturn == false)// if exchange
                                    {
                                        bool bIsOGTaxApplicable = chkOGTaxApplicable();
                                        if (bIsOGTaxApplicable)
                                        {
                                            decimal dTotOgTaxAmt = 0m;
                                            decimal dTotSalesGoldTaxAmt = 0m;

                                            foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
                                            {
                                                if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                                                    && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                                    && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                                                    && !saleLineItem1.Voided)
                                                {
                                                    dTotSalesGoldTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDTAX));
                                                }
                                                if ((saleLineItem1.PartnerData.TransactionType == "3" || saleLineItem1.PartnerData.TransactionType == "1")
                                                    && saleLineItem1.PartnerData.NimReturnLine == 0
                                                    && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                                    && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) < 0
                                                    && !saleLineItem1.Voided)
                                                {
                                                    //dTotOgTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.TaxAmount));
                                                    int iPM = GetMetalType(saleLineItem1.ItemId);
                                                    if (iPM == (int)MetalType.Gold)
                                                    {
                                                        dTotOgTaxAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
                                                    }
                                                }
                                            }

                                            if (dTotOgTaxAmt > 0 && dTotSalesGoldTaxAmt > 0 && retailTrans.PartnerData.SingaporeTaxCal == "0")
                                                bIsValidClick = true;
                                            else
                                                bIsValidClick = false;
                                        }
                                    }
                                }
                                #endregion
                            }
                        }

                        decimal dLineGoldAmt = 0m;
                        decimal dTotOgExcAmt = 0m;
                        decimal dTotSalesLineGoldValue = 0m;
                        decimal dTotSalesTaxAmt = 0m;
                        decimal dNewTaxPerc = 0m;
                        decimal dGoldValueDiff = 0m;
                        int iMetalType = 0;
                        int iSuccess = 0;
                        frmVertualRecalTaxValue objVRTV;
                        if (bIsValidClick)
                        {
                            if (retailTrans.PartnerData.SingaporeTaxCal == "0")
                            {
                                #region Vertual Show
                                decimal dTotVNetAmt = 0m;
                                decimal dTotVTaxAmt = 0m;
                                decimal dTotVAmount = 0m;
                                PreActionRecalTax(retailTrans, ref dTotOgExcAmt, ref dTotSalesLineGoldValue, ref dTotSalesTaxAmt, ref iMetalType);

                                VertualPostActionRecalTax(retailTrans, ref dLineGoldAmt,
                                    ref dTotOgExcAmt, dTotSalesLineGoldValue,
                                    ref dNewTaxPerc, ref dGoldValueDiff, ref iMetalType,
                                    ref dTotVNetAmt,
                                    ref dTotVAmount,
                                    ref dTotVTaxAmt);

                                objVRTV = new frmVertualRecalTaxValue(dTotVNetAmt - dTotVTaxAmt, dTotVNetAmt, dTotVTaxAmt, application);//dTotVAmount
                                objVRTV.ShowDialog();

                                #endregion

                                #region If excahnge then  For Singapore only
                                if (!objVRTV.isCancel)
                                {
                                    #region Discount
                                    if (objVRTV.dDiscAmt > 0)
                                    {
                                        //Application.RunOperation(PosisOperations.TotalDiscountAmount, objVRTV.dDiscAmt);

                                        Microsoft.Dynamics.Retail.Pos.DiscountService.Discount objDisc = new Microsoft.Dynamics.Retail.Pos.DiscountService.Discount();
                                        objDisc.Application = this.application;
                                        retailTrans.PartnerData.ReCalculate = true;
                                        objDisc.AddTotalDiscountAmount(retailTrans, objVRTV.dDiscAmt);
                                        objDisc.CalculateDiscount(retailTrans);

                                        foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
                                        {
                                            saleLineItem1.TaxLines.Clear();
                                            saleLineItem1.CalculateLine();

                                            TaxService.Tax objTax = new TaxService.Tax();

                                            if (application != null)
                                                objTax.Application = application;
                                            else
                                                objTax.Application = Application;

                                            objTax.Initialize();
                                            objTax.CalculateTax(saleLineItem1, retailTrans);

                                            retailTrans.CalcTotals();
                                            retailTrans.CalculateAmountDue();
                                        }
                                    }
                                    #endregion

                                    if (retailTrans.IncomeExpenseAmounts == 0
                                        && retailTrans.SalesInvoiceAmounts == 0)
                                    {
                                        dLineGoldAmt = 0m;
                                        dTotOgExcAmt = 0m;
                                        dTotSalesLineGoldValue = 0m;
                                        dTotSalesTaxAmt = 0m;
                                        dNewTaxPerc = 0m;
                                        dGoldValueDiff = 0m;
                                        iMetalType = 0;

                                        retailTrans.PartnerData.SingaporeTaxCal = "1";

                                        foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
                                        {
                                            if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                                                && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                                && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                                                && !saleLineItem1.Voided)
                                            {
                                                decimal dLineGoldValue = 0m;
                                                decimal dLineGoldTax = 0m;
                                                bool isBulkItem = false;
                                                if (IsBulkItem(saleLineItem1.ItemId))
                                                    isBulkItem = true;
                                                else
                                                    isBulkItem = false;
                                                if (isBulkItem == false)
                                                {
                                                    if (!string.IsNullOrEmpty(saleLineItem1.PartnerData.LINEGOLDVALUE))
                                                        dLineGoldValue = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDVALUE));
                                                }
                                                else
                                                {
                                                    iMetalType = GetMetalType(saleLineItem1.ItemId);
                                                    if (iMetalType == (int)MetalType.Gold)
                                                    {
                                                        if (!string.IsNullOrEmpty(saleLineItem1.PartnerData.Amount))
                                                            dLineGoldValue = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.Amount));
                                                    }
                                                }

                                                if (!string.IsNullOrEmpty(saleLineItem1.PartnerData.LINEGOLDTAX))
                                                    dLineGoldTax = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDTAX));

                                                dTotSalesLineGoldValue += dLineGoldValue;
                                                dTotSalesTaxAmt += dLineGoldTax;
                                            }
                                            if ((saleLineItem1.PartnerData.TransactionType == "3" || saleLineItem1.PartnerData.TransactionType == "1")
                                                && saleLineItem1.PartnerData.NimReturnLine == 0
                                                && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                                && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) < 0
                                                && !saleLineItem1.Voided)
                                            {
                                                iMetalType = GetMetalType(saleLineItem1.ItemId);
                                                if (iMetalType == (int)MetalType.Gold)
                                                    dTotOgExcAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
                                            }
                                        }

                                        if (dTotSalesLineGoldValue > 0 && dTotOgExcAmt > 0)
                                        {
                                            if (dTotSalesLineGoldValue > dTotOgExcAmt)
                                            {
                                                foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
                                                {
                                                    if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                                                        && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                                        && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                                                        && !saleLineItem1.Voided
                                                        && dTotOgExcAmt > 0)
                                                    {

                                                        iMetalType = GetMetalType(saleLineItem1.ItemId);
                                                        bool isBulkItem = false;
                                                        if (IsBulkItem(saleLineItem1.ItemId))
                                                            isBulkItem = true;
                                                        else
                                                            isBulkItem = false;

                                                        if (iMetalType == (int)MetalType.Gold)
                                                        {
                                                            if (isBulkItem == false)
                                                            {
                                                                if (!string.IsNullOrEmpty(saleLineItem1.PartnerData.LINEGOLDVALUE))
                                                                    dLineGoldAmt = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDVALUE));
                                                            }
                                                            else
                                                            {
                                                                if (!string.IsNullOrEmpty(saleLineItem1.PartnerData.Amount))
                                                                    dLineGoldAmt = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.Amount));
                                                            }
                                                        }

                                                        //if (iMetalType == (int)MetalType.Gold && isBulkItem == true)
                                                        //    dLineGoldAmt = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
                                                        //else
                                                        //    dLineGoldAmt = dLineGoldValue;

                                                        dGoldValueDiff = dTotOgExcAmt - dLineGoldAmt;

                                                        if (iMetalType == (int)MetalType.Gold)
                                                        {
                                                            if (dGoldValueDiff > 0)
                                                            {
                                                                RecalTaxAtOgExchange(retailTrans, dNewTaxPerc, saleLineItem1);
                                                                dTotOgExcAmt = dTotOgExcAmt - dLineGoldAmt;
                                                                iSuccess = 1;
                                                            }
                                                            else
                                                            {
                                                                decimal dGoldTax = 0m;
                                                                dLineGoldAmt = Math.Abs(dGoldValueDiff);

                                                                decimal dLineAmt = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
                                                                decimal dLineTax = Math.Abs(Convert.ToDecimal(saleLineItem1.TaxAmount));
                                                                dGoldTax = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDTAX));
                                                                decimal dNewTaxAmt = dLineTax - dGoldTax;
                                                                decimal dTaxPerc = getItemTaxPercentage(saleLineItem1.ItemId);
                                                                decimal dB4TaxImpNetAmt = 0m;
                                                                dB4TaxImpNetAmt = saleLineItem1.NetAmount;
                                                                decimal dRestOfAmtTax = 0m;
                                                                dRestOfAmtTax = (dLineGoldAmt * dTaxPerc) / 100;

                                                                if (dLineTax > 0)
                                                                {
                                                                    dNewTaxPerc = ((dNewTaxAmt + dRestOfAmtTax) * dTaxPerc) / dLineTax;

                                                                    // saleLineItem1.PriceOverridden = true;
                                                                    saleLineItem1.NetAmount = Convert.ToDecimal(saleLineItem1.NetAmount - dGoldTax + dRestOfAmtTax);
                                                                    saleLineItem1.GrossAmount = dLineAmt;// Convert.ToDecimal(saleLineItem1.NetAmount - dGoldTax + dRestOfAmtTax);
                                                                    saleLineItem1.Price = (dLineAmt - dGoldTax + dRestOfAmtTax) / saleLineItem1.Quantity;

                                                                    saleLineItem1.PartnerData.LINEGOLDTAX = Convert.ToString(dRestOfAmtTax);
                                                                    saleLineItem1.PartnerData.NewTaxPerc = dNewTaxPerc;
                                                                    saleLineItem1.PartnerData.MakingAmount = Convert.ToString(Convert.ToDecimal(saleLineItem1.PartnerData.MakingAmount) - dGoldTax + dRestOfAmtTax);
                                                                    saleLineItem1.PartnerData.TotalAmount = Convert.ToString(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) - dGoldTax + dRestOfAmtTax);


                                                                    saleLineItem1.TaxRatePct = dNewTaxPerc;
                                                                    saleLineItem1.TaxLines.Clear();
                                                                    saleLineItem1.CalculateLine();


                                                                    dTotOgExcAmt = 0;

                                                                    #region Recalculate Tax
                                                                    TaxService.Tax objTax = new TaxService.Tax();

                                                                    if (application != null)
                                                                        objTax.Application = application;
                                                                    else
                                                                        objTax.Application = Application;

                                                                    objTax.Initialize();
                                                                    objTax.CalculateTax(saleLineItem1, retailTrans);
                                                                    retailTrans.CalcTotals();
                                                                    retailTrans.CalculateAmountDue();

                                                                    iSuccess = 1;
                                                                    #endregion
                                                                }
                                                            }

                                                        }
                                                    }
                                                }

                                            }
                                            else// all tax 0 pct
                                            {
                                                foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
                                                {
                                                    iMetalType = GetMetalType(saleLineItem1.ItemId);
                                                    if (iMetalType == (int)MetalType.Gold)
                                                    {
                                                        if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                                                            && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                                            && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                                                            && !saleLineItem1.Voided)
                                                        {
                                                            RecalTaxAtOgExchange(retailTrans, dNewTaxPerc, saleLineItem1);
                                                            iSuccess = 1;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("There is no gold type item in the sales line.");
                                            retailTrans.PartnerData.SingaporeTaxCal = "0";
                                            return;
                                        }
                                    }
                                }
                                #endregion
                            }
                            else
                            {
                                MessageBox.Show("Already recalculation of tax is done.");
                                return;
                            }
                            if (iSuccess == 1)
                            {
                                MessageBox.Show("Recalculation of tax is done");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Invalid operation.");
                            return;
                        }
                    }
                    #endregion
                    break;
                case "SHOWTRANSACTION"://Bank drop, safe drop,Opening cash Declaration,Tender declaration
                    #region SHOWTRANSACTION
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    frmShowSafeDropBankDropOpAmtTenderDecInfo objFrm = new frmShowSafeDropBankDropOpAmtTenderDecInfo(connection);
                    objFrm.ShowDialog();
                    break;
                    #endregion
                case "SKUAGINGDISC":
                    #region SKUAgingDisc
                    if (retailTrans != null)
                    {
                        if (retailTrans.SaleItems.Count == 0)
                        {
                            retailTrans.PartnerData.SKUAgingDisc = false;
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                        else
                        {
                            if (retailTrans.PartnerData.SKUAgingDiscDone == false)
                            {
                                retailTrans.PartnerData.SKUAgingDisc = true;
                                DiscountService.Discount objDisc = new DiscountService.Discount();

                                objDisc.Application = this.application;
                                objDisc.CalculateDiscount(retailTrans);
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("SKU aging discount already has been applied.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                    break;
                    #endregion
                case "TIERDISC"://
                    #region TIERDISC
                    if (retailTrans != null)
                    {
                        if (retailTrans.SaleItems.Count == 0)
                        {
                            retailTrans.PartnerData.TierDisc = false;
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                        else
                        {
                            if (retailTrans.PartnerData.TierDiscDone == false && retailTrans.PartnerData.MakStnDiaDiscDone == false)
                            {
                                retailTrans.PartnerData.TierDisc = true;
                                DiscountService.Discount objDisc = new DiscountService.Discount();

                                objDisc.Application = this.application;
                                objDisc.CalculateDiscount(retailTrans);
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Tier discount already has been applied.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                    break;
                    #endregion
                case "DISCOUNT"://
                    #region Discount
                    if (retailTrans != null)
                    {
                        if (retailTrans.SaleItems.Count == 0)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                        else
                        {
                            if (retailTrans.PartnerData.APPLYGSSDISCDONE == false && retailTrans.PartnerData.MakStnDiaDiscDone == false)
                            {
                                foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                                {
                                    if (Convert.ToString(operationInfo.ItemLineId) == Convert.ToString(saleLineItem.LineId))
                                    {
                                        //if (saleLineItem.LinePctDiscount == 0)
                                        //{
                                            frmDiscount objCP = new frmDiscount(posTransaction, application, saleLineItem);
                                            objCP.ShowDialog();

                                            if (retailTrans.PartnerData.GENERALDISC == true)
                                            {
                                                DiscountService.Discount objDisc = new DiscountService.Discount();

                                                objDisc.Application = this.application;
                                                objDisc.CalculateDiscount(retailTrans);
                                            }
                                        //}
                                        //else
                                        //{
                                        //    MessageBox.Show("Discount already applied");
                                        //}
                                    }
                                }
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Discount not allowed if GSS discount is done..", MessageBoxButtons.OK, MessageBoxIcon.Information))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                    break;
                    #endregion
                case "PROMODISCOUNT"://
                    #region PROMODISCOUNT
                    if (retailTrans != null)
                    {
                        if (retailTrans.SaleItems.Count == 0)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                        else
                        {
                            if (retailTrans.PartnerData.APPLYGSSDISCDONE == false && retailTrans.PartnerData.MakStnDiaDiscDone == false)
                            {
                                foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                                {
                                    if (Convert.ToString(operationInfo.ItemLineId) == Convert.ToString(saleLineItem.LineId))
                                    {
                                        if (saleLineItem.LinePctDiscount == 0)
                                        {
                                            frmDiscount objCP = new frmDiscount(posTransaction, application, saleLineItem, 1);
                                            objCP.ShowDialog();

                                            if (retailTrans.PartnerData.GENERALDISC == true)
                                            {
                                                DiscountService.Discount objDisc = new DiscountService.Discount();

                                                objDisc.Application = this.application;
                                                objDisc.CalculateDiscount(retailTrans);
                                            }
                                        }
                                        else
                                        {
                                            MessageBox.Show("Discount already applied");
                                        }

                                    }
                                }
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Discount not allowed if GSS discount is done..", MessageBoxButtons.OK, MessageBoxIcon.Information))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                    break;
                    #endregion
                case "VOIDLOYALTY":
                    #region For loyalty delete
                    if (retailTrans != null)
                    {
                        if (retailTrans.Customer != null)
                        {
                            retailTrans.LoyaltyItem.UsageType =
                                LSRetailPosis.Transaction.Line.LoyaltyItem.LoyaltyItemUsageType.NotUsed;
                            retailTrans.LoyaltyItem.LoyaltyCardNumber = "";
                            retailTrans.LoyaltyItem.LoyaltyCustID = "";
                            retailTrans.LoyaltyItem.SchemeID = "";
                            retailTrans.LoyaltyItem.CustID = "";
                            //Loyalty.Loyalty objL = new Loyalty.Loyalty();
                            //DE.ICardInfo cardInfo = null;
                            //if (application != null)
                            //    objL.Application = application;
                            //else
                            //    objL.Application = this.Application;
                            //objL.AddLoyaltyRequest(retailTrans, cardInfo);
                        }
                    }
                    break;
                    #endregion
                case "APPLYGSSDISC":
                    #region For APPLYGSSDISC
                    if (retailTrans != null)
                    {
                        System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
                        if (saleline.Count > 0)//for adv and gss @ a time test purpose commented on 160418
                        {
                            string sGSSAdjItemId = "";
                            string sGSSDiscItemId = "";
                            string sGSSAmountAdjItemId = "";
                            string sGSSAmountDiscItemId = "";
                            int iGSSDiscAgreementBase = 0;

                            if (retailTrans.PartnerData.APPLYGSSDISCDONE == false && retailTrans.PartnerData.MakStnDiaDiscDone == false)
                            {
                                GSSAdjustmentItemID(ref sGSSAdjItemId, ref sGSSDiscItemId, ref sGSSAmountAdjItemId, ref sGSSAmountDiscItemId, ref iGSSDiscAgreementBase);

                                if (Convert.ToInt16(retailTrans.PartnerData.SchemeDepositeType) == 0)//existing/old is type gold
                                {
                                    if (iGSSDiscAgreementBase == 1)//added on 310119 for PNG
                                    {
                                        retailTrans.PartnerData.APPLYGSSDISC = true;
                                        if (Convert.ToDecimal(retailTrans.PartnerData.GSSRoyaltyAmt) > 0)
                                        {
                                            Application.RunOperation(PosisOperations.ItemSale, sGSSDiscItemId);
                                        }
                                    }
                                }
                                else//amount changed on 18102017
                                {
                                    if (iGSSDiscAgreementBase == 1)
                                    {
                                        retailTrans.PartnerData.APPLYGSSDISC = true;
                                        if (Convert.ToDecimal(retailTrans.PartnerData.GSSRoyaltyAmt) > 0)
                                        {
                                            Application.RunOperation(PosisOperations.ItemSale, sGSSAmountDiscItemId);
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("FPP discount already has been applied.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                            }
                        }
                    }
                    break;
                    #endregion
                case "GETCUSTOMERBALANCE":
                    #region GETCUSTOMERBALANCE
                    frmCustomerBalance objCB = new frmCustomerBalance();
                    objCB.ShowDialog();
                    break;
                    #endregion
                case "SUSPENDEDTRANSACTIONINFO"://customized recall transaction
                    #region SUSPENDEDTRANSACTIONINFO
                    if (retailTrans == null)
                    {
                        DataTable dtTrans = new DataTable();
                        DataRow drSelected = null;
                        string sOpTerminals = string.Empty;
                        SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                        SqlCon.Open();

                        SqlCommand SqlComm = new SqlCommand();
                        SqlComm.Connection = SqlCon;
                        SqlComm.CommandType = CommandType.Text;
                        SqlComm.CommandText = " SELECT SUSPENDEDTRANSACTIONID TRANSACTIONID,RECEIPTID,CAST(CAST(NETAMOUNT AS NUMERIC(20,2)) as NVARCHAR(30)) NETAMOUNT" +
                                               " ,CUSTACCOUNT,CUSTNAME,CAST(CUSTMOBILE as NVARCHAR(30)) CUSTMOBILE FROM RETAILSUSPENDEDTRANSACTIONS" +
                                               " where STOREID='" + ApplicationSettings.Database.StoreID + "'";

                        SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                        SqlDa.Fill(dtTrans);

                        if (dtTrans != null && dtTrans.Rows.Count > 0)
                        {
                            Dialog.WinFormsTouch.frmGenericSearch OSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtTrans, drSelected, "Recall Transaction");
                            OSearch.ShowDialog();
                            drSelected = OSearch.SelectedDataRow;

                            if (drSelected != null)
                                Application.RunOperation(PosisOperations.RecallTransaction, Convert.ToString(drSelected[0]));
                        }
                    }
                    break;
                    #endregion
                case "OFFLINESALESRECORD":
                    #region OFFLINESALESRECORD
                    frmOfflineBillRecording objos = new frmOfflineBillRecording(posTransaction, application);
                    objos.ShowDialog();
                    break;
                    #endregion
                case "OFFLINEBILLSETTLEMENT":
                    #region OFFLINEBILLSETTLEMENT
                    frmOfflineBillSettle objOBS = new frmOfflineBillSettle(posTransaction, application);
                    objOBS.ShowDialog();
                    break;
                    #endregion
                case "REPRINTGV":
                    frmPrintGV objGV = new frmPrintGV();
                    objGV.ShowDialog();
                    break;
                case "PDCENTRY":
                    #region PDCENTRY
                    frmPDCEntry objPDC = new frmPDCEntry(posTransaction, application);
                    objPDC.ShowDialog();
                    break;
                    #endregion
                case "UNLOCKADVANCE":
                    #region Unlock ADVANCE
                    if (retailTrans != null)
                    {
                        DataTable dtTrans = new DataTable();
                        string sOpTerminals = string.Empty;

                        dt = Application.BusinessLogic.SuspendRetrieveSystem.RetrieveTransactionList(ApplicationSettings.Terminal.StoreId);

                        string sQry = "SELECT isnull(TERMINALID,'') as TERMINALID FROM RETAILTRANSACTION WHERE STOREID = '" + ApplicationSettings.Terminal.StoreId + "'" +
                                      " AND DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "' AND TERMINALID <> '" + ApplicationSettings.Terminal.TerminalId + "'";

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        SqlCommand cmd = new SqlCommand(sQry, connection);
                        cmd.CommandTimeout = 0;
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        da.Fill(dtTrans);

                        if (dtTrans != null && dtTrans.Rows.Count > 0)
                        {
                            foreach (DataRow dr in dtTrans.Rows)
                            {
                                sOpTerminals = sOpTerminals + " : " + Convert.ToString(dr[0]);
                            }

                            string sValidMsg = "Please close the Terminal operation (" + sOpTerminals + ") properly before unlock";
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(sValidMsg, MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                        else
                        {
                            sCustAc = retailTrans.Customer.CustomerId;
                            DataRow drSelected = null;

                            try
                            {
                                if (PosApplication.Instance.TransactionServices.CheckConnection())
                                {
                                    DataTable dtLockedAdv = GetUnadjustedLockedAdvData(retailTrans.Customer.CustomerId);
                                    if (dtLockedAdv != null && dtLockedAdv.Rows.Count > 0)
                                    {
                                        Dialog.WinFormsTouch.frmGenericSearch OSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtLockedAdv, drSelected, "Locked Advance List");
                                        OSearch.ShowDialog();
                                        drSelected = OSearch.SelectedDataRow;

                                        if (drSelected != null)
                                        {
                                            string sTransId = Convert.ToString(drSelected[1]);
                                            string sTerminalId = Convert.ToString(drSelected[8]);
                                            string sStoreId = Convert.ToString(drSelected[7]);
                                            unlockLockedAdvData(sTransId, sStoreId, sTerminalId);
                                        }
                                    }
                                }
                                else
                                {
                                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Real time service not working.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                                    {
                                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                    }
                                    return;
                                }
                            }
                            catch (Exception ex)
                            {

                            }
                        }
                    }
                    break;
                    #endregion
                case "MOPWISETRANSACTION":
                    #region MOPWiseTransaction
                    SqlConnection SqlConCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                    frmDateFilter frmDateFilter = new frmDateFilter(SqlConCon, 1);
                    frmDateFilter.ShowDialog();
                    break;
                    #endregion
                case "FLATMAKINGDISCOUNT"://
                    #region FLATMAKINGDISCOUNT
                    if (retailTrans != null)
                    {
                        if (retailTrans.SaleItems.Count == 0)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                        else
                        {
                            if (retailTrans.PartnerData.APPLYGSSDISCDONE == false)
                            {
                                frmDiscount objCP = new frmDiscount(posTransaction, application,1);
                                objCP.ShowDialog();

                                if (Convert.ToDecimal(retailTrans.PartnerData.ExtraMkDiscPct)> 0)
                                {
                                    if (retailTrans.PartnerData.GENERALDISC == true)
                                    {
                                        DiscountService.Discount objDisc = new DiscountService.Discount();

                                        objDisc.Application = this.application;
                                        objDisc.CalculateDiscount(retailTrans);
                                    }
                                }
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Discount not allowed if GSS discount is done..", MessageBoxButtons.OK, MessageBoxIcon.Information))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                    break;
                    #endregion
                case "FLATMAKINGCHARGE":
                    #region FLATMAKINGCHARGE
                    if (retailTrans != null)
                    {
                        if (retailTrans.PartnerData.APPLYFLATMKDISCPCT == false)
                        {
                            frmDiscount objCP = new frmDiscount(posTransaction, application, 0, 1);
                            objCP.ShowDialog();
                        }
                        else
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Already applied.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select a customer.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                      
                    break;
                    #endregion
                case "APPLYGMADISC":
                    #region For APPLYGMADISC
                    if (retailTrans != null)
                    {
                        System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(posTransaction)).SaleItems);
                        if (saleline.Count > 0)
                        {
                            string sGMAAdvItem = "";
                            string sGMALGItem = "";
                            string sGMADiscItem = "";
                            string sGMAGSTItem = "";
                            decimal dTotAdvAmt = 0m;
                            decimal dGMALossGainAmt = 0m;
                            decimal dGMADiscAmt = 0m;
                            decimal dGMAGSTAmt = 0m;


                            if (retailTrans.PartnerData.APPLYGMADISCDONE == false && !string.IsNullOrEmpty(Convert.ToString(retailTrans.PartnerData.AdjustmentOrderNum)))
                            {
                                GMAAdjustmentItemID(ref sGMAAdvItem, ref sGMAGSTItem, ref sGMALGItem, ref sGMADiscItem);

                                dTotAdvAmt = Convert.ToDecimal(retailTrans.PartnerData.GMATotAdvance);

                                if (dTotAdvAmt > 0)
                                {
                                    decimal dGMAAdvTotQty = 0m;
                                    decimal dGMADiscQty = 0m;
                                    decimal dCurrentRate = 0m;
                                    decimal dIGST = 0m;
                                    decimal dCGSTSGST = 0m;
                                    decimal dGSTPct = 0m;
                                    decimal dSalesQty = 0m;
                                    decimal dCessPct = 0m;


                                    dCurrentRate = getMetalRate(Convert.ToString(retailTrans.PartnerData.AdjustmentOrderNum));

                                    getGMADiscAndAdvWt(Convert.ToString(retailTrans.PartnerData.AdjustmentOrderNum), ref dGMAAdvTotQty, ref dGMADiscQty);

                                    foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                                    {
                                        if (!saleLineItem.Voided
                                            && saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                                            && saleLineItem.PartnerData.TransactionType == Convert.ToString((int)TransactionType.Sale)
                                            && saleLineItem.ReturnLineId == 0)
                                        {
                                            bool isBulkItem = false;
                                            if (IsBulkItem(saleLineItem.ItemId))
                                                isBulkItem = true;
                                            else
                                                isBulkItem = false;

                                            if (isBulkItem == true)
                                            {
                                                int iMetalType = GetMetalType(saleLineItem.ItemId);

                                                if (iMetalType == (int)MetalType.Gold)
                                                    dSalesQty += Convert.ToDecimal(saleLineItem.PartnerData.Quantity);
                                            }
                                            else
                                            {
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

                                                    if ((iMetalType == (int)MetalType.Gold))
                                                    {
                                                        dSalesQty += Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(drIngredients["QTY"])) ? drIngredients["QTY"] : "0");
                                                    }
                                                    i++;
                                                }
                                            }
                                        }
                                    }

                                    if (dSalesQty < dGMAAdvTotQty)
                                    {
                                        MessageBox.Show("Sale quantity is less than the advance quantity.");
                                        return;
                                    }

                                    #region GST
                                    foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                                    {
                                        if (!saleLineItem.Voided
                                            && saleLineItem.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service)
                                        {
                                            foreach (LSRetailPosis.Transaction.Line.TaxItems.TaxItemGTE taxLine in saleLineItem.TaxLines)
                                            {
                                                if (taxLine.TaxComponent == "SGST")
                                                {
                                                    dCGSTSGST += taxLine.Percentage;
                                                }
                                                if (taxLine.TaxComponent == "IGST")
                                                {
                                                    dIGST = taxLine.Percentage;
                                                }
                                                if (taxLine.TaxComponent == "CGST")
                                                {
                                                    dCGSTSGST += taxLine.Percentage;
                                                }
                                                //========================Soutik==================================
                                                if (taxLine.TaxComponent == "CESS")
                                                {
                                                    dCessPct = 0;
                                                    dCessPct = taxLine.Percentage;
                                                }
                                                //==============================================================
                                            }
                                            break;
                                        }
                                    }

                                    if (dCGSTSGST > 0)
                                        dGSTPct = dCGSTSGST+dCessPct;
                                    else
                                        dGSTPct = dIGST;
                                    #endregion

                                    dGMALossGainAmt = dTotAdvAmt - Convert.ToDecimal((Convert.ToString(decimal.Round((dCurrentRate * dGMAAdvTotQty), 2, MidpointRounding.AwayFromZero)))); //(dCurrentRate * dGMAAdvTotQty);
                                    dGMADiscAmt = Convert.ToDecimal((Convert.ToString(decimal.Round((dCurrentRate * dGMADiscQty), 2, MidpointRounding.AwayFromZero))));
                                    dGMAGSTAmt = Convert.ToDecimal((Convert.ToString(decimal.Round(((dTotAdvAmt * -1) + dGMALossGainAmt + (dGMADiscAmt * -1)) * dGSTPct / 100, 2, MidpointRounding.AwayFromZero))));

                                    //================Soutik=================As Per Swarnavo - GSTAMt ALways Negative-------26-08-2019----
                                    dGMAGSTAmt = dGMAGSTAmt * (-1);

                                    retailTrans.PartnerData.GMALossGainTotAmt = dGMALossGainAmt;
                                    retailTrans.PartnerData.GMADiscAmt = dGMADiscAmt;
                                    retailTrans.PartnerData.GMAGSTAmt = dGMAGSTAmt;

                                    retailTrans.PartnerData.APPLYGMADISC = true;
                                    Application.RunOperation(PosisOperations.ItemSale, sGMALGItem);
                                    Application.RunOperation(PosisOperations.ItemSale, sGMADiscItem);
                                    Application.RunOperation(PosisOperations.ItemSale, sGMAGSTItem);
                                }
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("GMA discount already has been applied.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                                {
                                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                }
                            }
                        }
                    }
                    break;
                    #endregion
                case "REPRINTGMADISCVOUCHER":
                    frmPrintGV objGV1 = new frmPrintGV(1, posTransaction, application);
                    objGV1.ShowDialog();
                    break;
                case "MAKSTNDIADISC"://created by Soutik
                    #region MakStnDiaDis
                    if (retailTrans != null)
                    {
                        if (retailTrans.SaleItems.Count == 0)
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                        else
                        {
                            //if (retailTrans.PartnerData.MakStnDiaDiscDone == false)
                            //{
                                foreach (SaleLineItem saleLineItem in retailTrans.SaleItems)
                                {
                                    if (Convert.ToString(operationInfo.ItemLineId) == Convert.ToString(saleLineItem.LineId))
                                    {
                                        //if (saleLineItem.LinePctDiscount == 0)
                                        //{
                                        FrmMakStnDiaDisc objCP = new FrmMakStnDiaDisc(posTransaction, application, saleLineItem, 1);
                                        objCP.ShowDialog();

                                        if (retailTrans.PartnerData.MakStnDiaDisc == true)
                                        {
                                            DiscountService.Discount objDisc = new DiscountService.Discount();

                                            objDisc.Application = this.application;
                                            objDisc.CalculateDiscount(retailTrans);
                                        }
                                        //}
                                        //else
                                        //{
                                        //    MessageBox.Show("Discount already applied");
                                        //}

                                    }
                                }
                            //}
                            //else
                            //{
                            //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Discount not allowed if once discount is done..", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            //    {
                            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            //    }
                            //}
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                    break;
                    #endregion
                case "ADVDISCAGREE":  // advance discount agreement
                    #region Advance Discount Agreement
                    if (retailTrans != null)
                    {
                        if (retailTrans.SaleItems.Count == 0)
                        {
                            retailTrans.PartnerData.AdvAgreemetDisc = false;
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        else
                        {
                            if (retailTrans.PartnerData.AdvAgreemetDiscDone == false && retailTrans.PartnerData.MakStnDiaDiscDone == false)
                            {
                                retailTrans.PartnerData.AdvAgreemetDisc = true;
                                DiscountService.Discount objDisc = new DiscountService.Discount();

                                objDisc.Application = this.application;
                                objDisc.CalculateDiscount(retailTrans);
                            }
                            else
                            {
                                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Advance discount already has been applied.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("There are no items in the list.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    break;
                    #endregion
                //=====================Soutik============================================
                case "INCEXPREG":
                    #region Income Expense Register
                    if (connection.State == ConnectionState.Closed)
                        connection.Open();
                    FrmIncomeExpenseRegister objIncExpRegister = new FrmIncomeExpenseRegister();
                    objIncExpRegister.ShowDialog();
                    break;
                    #endregion
                //=========================================================================
                default:
                    #region default
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please pass the correct parameters.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    break;
                    #endregion
            }
            operationInfo.OperationHandled = true;
        }

        private bool IsGMA(string sOrderNo, ref decimal dTotAmt)
        {
            bool bResult = false;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT GMA,CAST(TOTALAMOUNT AS NUMERIC (28,2)) AS TOTALAMOUNT  FROM [CUSTORDER_HEADER] WHERE ORDERNUM='" + sOrderNo + "'");
            DataTable dtGSS = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataAdapter daGss = new SqlDataAdapter(cmd);
            daGss.Fill(dtGSS);

            if (dtGSS != null && dtGSS.Rows.Count > 0)
            {
                bResult = Convert.ToBoolean(dtGSS.Rows[0]["GMA"]);
                dTotAmt = Convert.ToDecimal(dtGSS.Rows[0]["TOTALAMOUNT"]);
            }
            return bResult;
        }

        private decimal getMetalRate(string sOrderNum)
        {
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            string storeId = ApplicationSettings.Database.StoreID;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @ConfigId VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + storeId.Trim() + "'");
            commandText.Append(" SELECT @ConfigId=CONFIGID FROM CUSTORDER_DETAILS ");
            commandText.Append(" WHERE ORDERNUM='" + sOrderNum.Trim() + "'");

            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RATES,0) AS decimal (6,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            commandText.Append(" AND METALTYPE=" + (int)MetalType.Gold + " ");
            commandText.Append(" AND RETAIL=1 AND RATETYPE=3 ");//'" + (int)RateTypeNew.Sale + "'
            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD=@ConfigId ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");
            commandText.Append(" END ");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return Convert.ToDecimal(sResult.Trim());
            else
                return 0;
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

        private void getGMADiscAndAdvWt(string sOrderNo, ref decimal dGMATotWt, ref decimal dGMADiscWt)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select top 1 GMADISCWT,CAST(QTY AS NUMERIC (28,3))  QTY from CUSTORDER_DETAILS  where GMADISCWT>0 and ORDERNUM='" + sOrderNo + "'");

            DataTable dtGSS = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataAdapter daGss = new SqlDataAdapter(cmd);
            daGss.Fill(dtGSS);

            if (dtGSS != null && dtGSS.Rows.Count > 0)
            {
                dGMATotWt = Convert.ToDecimal(dtGSS.Rows[0]["QTY"]);
                dGMADiscWt = Convert.ToDecimal(dtGSS.Rows[0]["GMADISCWT"]);
            }
        }

        private void PostCustOrderSave1(IPosTransaction posTransaction, frmCustomerOrder objCustOrdr)
        {
            if (objCustOrdr.bDataSaved)
            {
                #region Commented
                /*int iSPId = 0;

                iSPId = getUserDiscountPermissionId();
                if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager || Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager2)
                    iIsCustOrderWithAdv = 0;//means optional
                else
                    iIsCustOrderWithAdv = 1;//Must

                if (iIsCustOrderWithAdv == 1)
                {
                    if (objCustOrdr.dtChequeInfo == null || objCustOrdr.dtChequeInfo.Rows.Count == 0)
                    {
                        SaveCustomerOrderDepositDetails(Convert.ToString(objCustOrdr.sCustOrder), Convert.ToString(objCustOrdr.sTotalAmt));
                        Application.RunOperation(PosisOperations.Customer, objCustOrdr.sCustAcc);
                        Application.RunOperation(PosisOperations.CustomerAccountDeposit, string.Empty);
                    }
                    UpdateCustomerOrderWithAdvOrNot(Convert.ToString(objCustOrdr.sCustOrder));
                    objCustOrdr.iIsCustOrderWithAdv = 1;
                }
                else if (iIsCustOrderWithAdv == 0)
                {
                    if (objCustOrdr.dtChequeInfo == null || objCustOrdr.dtChequeInfo.Rows.Count == 0)
                    {
                        if (objCustOrdr.chkSUVARNAVRUDDHI.Checked == false)
                        {
                            frmYESNO frmYN = new frmYESNO("With Advance", "Without Advance");
                            frmYN.ShowDialog();

                            if (frmYN.isY)
                            {
                                SaveCustomerOrderDepositDetails(Convert.ToString(objCustOrdr.sCustOrder), Convert.ToString(objCustOrdr.sTotalAmt));
                                Application.RunOperation(PosisOperations.Customer, objCustOrdr.sCustAcc);
                                Application.RunOperation(PosisOperations.CustomerAccountDeposit, string.Empty);
                                UpdateCustomerOrderWithAdvOrNot(Convert.ToString(objCustOrdr.sCustOrder));
                                objCustOrdr.iIsCustOrderWithAdv = 1;
                            }
                            else
                                objCustOrdr.iIsCustOrderWithAdv = 0;
                        }
                        else
                        {
                            SaveCustomerOrderDepositDetails(Convert.ToString(objCustOrdr.sCustOrder), Convert.ToString(objCustOrdr.sTotalAmt));
                            Application.RunOperation(PosisOperations.Customer, objCustOrdr.sCustAcc);
                            Application.RunOperation(PosisOperations.CustomerAccountDeposit, string.Empty);
                            UpdateCustomerOrderWithAdvOrNot(Convert.ToString(objCustOrdr.sCustOrder));
                            objCustOrdr.iIsCustOrderWithAdv = 1;
                        }
                    }
                }*/
                #endregion

                if (Convert.ToString(objCustOrdr.sCustOrder) != string.Empty && objCustOrdr.iCancel == 0)
                {
                    DataTable dtBookedSKU = new DataTable();
                    dtBookedSKU = GetBookedInfo(Convert.ToString(objCustOrdr.sCustOrder));
                    if (dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtBookedSKU.Rows)
                        {
                            SaveBookedSKU(posTransaction, Convert.ToString(objCustOrdr.sCustOrder), Convert.ToString(dr["ITEMID"]), objCustOrdr.sCustAcc, Convert.ToInt16(dr["LineNum"]));
                        }
                    }
                }
            }
        }

        private void PostCustOrderSave(IPosTransaction posTransaction, frmCustomerOrder objCustOrdr)
        {
            if (objCustOrdr.bDataSaved)
            {
                int iSPId = 0;

                iSPId = getUserDiscountPermissionId();
                if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager || Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager2)
                    iIsCustOrderWithAdv = 0;//means optional
                else
                    iIsCustOrderWithAdv = 1;//must

                if (iIsCustOrderWithAdv == 1)
                {
                    if (objCustOrdr.dtChequeInfo == null || objCustOrdr.dtChequeInfo.Rows.Count == 0)
                    {
                        if (objCustOrdr.chkSUVARNAVRUDDHI.Checked == true) //dTotalAdvAmt
                            SaveCustomerOrderDepositDetails(Convert.ToString(objCustOrdr.sCustOrder), Convert.ToString(objCustOrdr.dTotalAdvAmt));
                        else
                            SaveCustomerOrderDepositDetails(Convert.ToString(objCustOrdr.sCustOrder), Convert.ToString(objCustOrdr.sTotalAmt));

                        Application.RunOperation(PosisOperations.Customer, objCustOrdr.sCustAcc);
                        Application.RunOperation(PosisOperations.CustomerAccountDeposit, string.Empty);
                        objCustOrdr.iIsCustOrderWithAdv = 1;
                    }

                    UpdateCustomerOrderWithAdvOrNot(Convert.ToString(objCustOrdr.sCustOrder));
                }
                else if (iIsCustOrderWithAdv == 0)
                {
                    if (objCustOrdr.dtChequeInfo == null || objCustOrdr.dtChequeInfo.Rows.Count == 0)
                    {
                        if (objCustOrdr.chkSUVARNAVRUDDHI.Checked == true) //dTotalAdvAmt
                            SaveCustomerOrderDepositDetails(Convert.ToString(objCustOrdr.sCustOrder), Convert.ToString(objCustOrdr.dTotalAdvAmt));
                        else
                            SaveCustomerOrderDepositDetails(Convert.ToString(objCustOrdr.sCustOrder), Convert.ToString(objCustOrdr.sTotalAmt));


                        if (objCustOrdr.chkSUVARNAVRUDDHI.Checked == false)
                        {
                            frmYESNO frmYN = new frmYESNO("With Advance", "Without Advance");
                            frmYN.ShowDialog();

                            if (frmYN.isY)
                            {
                                Application.RunOperation(PosisOperations.Customer, objCustOrdr.sCustAcc);
                                Application.RunOperation(PosisOperations.CustomerAccountDeposit, string.Empty);
                                UpdateCustomerOrderWithAdvOrNot(Convert.ToString(objCustOrdr.sCustOrder));
                                objCustOrdr.iIsCustOrderWithAdv = 1;
                            }
                            else
                                objCustOrdr.iIsCustOrderWithAdv = 0;
                        }
                        else
                        {
                            Application.RunOperation(PosisOperations.Customer, objCustOrdr.sCustAcc);
                            Application.RunOperation(PosisOperations.CustomerAccountDeposit, string.Empty);
                            UpdateCustomerOrderWithAdvOrNot(Convert.ToString(objCustOrdr.sCustOrder));
                            objCustOrdr.iIsCustOrderWithAdv = 1;
                        }

                    }
                }

                if (Convert.ToString(objCustOrdr.sCustOrder) != string.Empty && objCustOrdr.iCancel == 0)
                {
                    DataTable dtBookedSKU = new DataTable();
                    dtBookedSKU = GetBookedInfo(Convert.ToString(objCustOrdr.sCustOrder));
                    if (dtBookedSKU != null && dtBookedSKU.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtBookedSKU.Rows)
                        {
                            SaveBookedSKU(posTransaction, Convert.ToString(objCustOrdr.sCustOrder), Convert.ToString(dr["ITEMID"]), objCustOrdr.sCustAcc, Convert.ToInt16(dr["LineNum"]));
                        }
                    }
                }

            }
        }

        public void PostCustOrderSaveCashierTask(IPosTransaction posTransaction, frmCustomerOrder objCustOrdr, IApplication _App)
        {
            if (_App != null)
            {
                int iSPId = 0;
                iSPId = getUserDiscountPermissionId();
                string sCustAcc = objCustOrdr.txtCustomerAccount.Text.Trim();
                string sCustOrder = Convert.ToString(objCustOrdr.sCustOrderSearchNumber);
                string sTotAmt = Convert.ToString(objCustOrdr.sTotalAmt);
                objCustOrdr.Close();

                if (!IsCustOrderConfirmed(sCustOrder))
                {
                    if (Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager || Convert.ToInt16(iSPId) == (int)CRWRetailDiscPermission.Manager2)
                        iIsCustOrderWithAdv = 0;//optional
                    else
                        iIsCustOrderWithAdv = 1;//Must

                    if (iIsCustOrderWithAdv == 1)
                    {
                        if (objCustOrdr.dtChequeInfo == null || objCustOrdr.dtChequeInfo.Rows.Count == 0)
                        {
                            SaveCustomerOrderDepositDetails(sCustOrder, sTotAmt);
                            _App.RunOperation(PosisOperations.Customer, sCustAcc);
                            _App.RunOperation(PosisOperations.CustomerAccountDeposit, string.Empty);
                        }
                        UpdateCustomerOrderWithAdvOrNot(sCustOrder);

                        objCustOrdr.iIsCustOrderWithAdv = 1;
                    }
                    else if (iIsCustOrderWithAdv == 0)
                    {
                        if (objCustOrdr.dtChequeInfo == null || objCustOrdr.dtChequeInfo.Rows.Count == 0)
                        {
                            if (objCustOrdr.chkSUVARNAVRUDDHI.Checked == false)
                            {
                                frmYESNO frmYN = new frmYESNO("With Advance", "Without Advance");
                                frmYN.ShowDialog();

                                if (frmYN.isY)
                                {
                                    SaveCustomerOrderDepositDetails(sCustOrder, sTotAmt);
                                    _App.RunOperation(PosisOperations.Customer, sCustAcc);
                                    _App.RunOperation(PosisOperations.CustomerAccountDeposit, string.Empty);
                                    UpdateCustomerOrderWithAdvOrNot(sCustOrder);
                                    objCustOrdr.iIsCustOrderWithAdv = 1;
                                }
                                else
                                    objCustOrdr.iIsCustOrderWithAdv = 0;
                            }
                            else
                            {
                                SaveCustomerOrderDepositDetails(sCustOrder, sTotAmt);
                                _App.RunOperation(PosisOperations.Customer, sCustAcc);
                                _App.RunOperation(PosisOperations.CustomerAccountDeposit, string.Empty);
                                UpdateCustomerOrderWithAdvOrNot(sCustOrder);
                                objCustOrdr.iIsCustOrderWithAdv = 1;
                            }
                        }
                        ConfirmCustOrder(sCustOrder);
                        PrintVoucher(sCustOrder, 1);
                    }


                }
                else
                {
                    MessageBox.Show("Cashier task has been already done");
                }
            }
        }

        private void VertualPostActionRecalTax(RetailTransaction retailTrans, ref decimal dLineGoldAmt,
            ref decimal dTotOgExcAmt, decimal dTotSalesLineGoldValue, ref decimal dNewTaxPerc,
            ref decimal dGoldValueDiff, ref int iMetalType,
            ref decimal dVNetAmt, ref decimal dVGrossAmt, ref decimal dVTotTax)
        {

            decimal dVPrice = 0m;
            decimal dVGoldTax = 0m;
            decimal dVNewTaxPct = 0m;

            if (dTotSalesLineGoldValue > dTotOgExcAmt)
            {
                foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
                {
                    if (!saleLineItem1.Voided)
                    {
                        if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                            && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                            && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                            && !saleLineItem1.Voided
                            && dTotOgExcAmt > 0)
                        {

                            iMetalType = GetMetalType(saleLineItem1.ItemId);
                            bool isBulkItem = false;
                            if (IsBulkItem(saleLineItem1.ItemId))
                                isBulkItem = true;
                            else
                                isBulkItem = false;

                            if (iMetalType == (int)MetalType.Gold && saleLineItem1.TaxAmount > 0)
                            {
                                if (isBulkItem == false)
                                {
                                    if (!string.IsNullOrEmpty(saleLineItem1.PartnerData.LINEGOLDVALUE))
                                        dLineGoldAmt = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDVALUE));
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(saleLineItem1.PartnerData.Amount))
                                        dLineGoldAmt = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.Amount));
                                }
                            }

                            dGoldValueDiff = dTotOgExcAmt - dLineGoldAmt;

                            if (iMetalType == (int)MetalType.Gold && saleLineItem1.TaxAmount > 0)
                            {
                                if (dGoldValueDiff > 0)
                                {
                                    decimal dGoldTax = 0m;

                                    decimal dLineAmt = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
                                    decimal dLineTax = Math.Abs(Convert.ToDecimal(saleLineItem1.TaxAmount));
                                    dGoldTax = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDTAX));
                                    decimal dNewTaxAmt = dLineTax - dGoldTax;
                                    decimal dTaxPerc = getItemTaxPercentage(saleLineItem1.ItemId);

                                    if (dLineTax > 0)
                                    {
                                        dNewTaxPerc = (dNewTaxAmt * dTaxPerc) / dLineTax;

                                        dVNetAmt += Convert.ToDecimal(dLineAmt - dGoldTax);
                                        dVGrossAmt += Convert.ToDecimal(dLineAmt);
                                        dVNewTaxPct = dNewTaxPerc;

                                        dVTotTax += (Convert.ToDecimal(dLineAmt - dGoldTax) * dNewTaxPerc / (100 + dNewTaxPerc));

                                        dTotOgExcAmt = dTotOgExcAmt - dLineGoldAmt;
                                    }
                                }
                                else
                                {
                                    decimal dGoldTax = 0m;
                                    dLineGoldAmt = Math.Abs(dGoldValueDiff);

                                    decimal dLineAmt = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
                                    decimal dLineTax = Math.Abs(Convert.ToDecimal(saleLineItem1.TaxAmount));
                                    dGoldTax = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDTAX));
                                    decimal dNewTaxAmt = dLineTax - dGoldTax;
                                    decimal dTaxPerc = getItemTaxPercentage(saleLineItem1.ItemId);

                                    decimal dRestOfAmtTax = 0m;
                                    dRestOfAmtTax = (dLineGoldAmt * dTaxPerc) / 100;

                                    if (dLineTax > 0)
                                    {
                                        dNewTaxPerc = ((dNewTaxAmt + dRestOfAmtTax) * dTaxPerc) / dLineTax;
                                        dVNetAmt += Convert.ToDecimal(saleLineItem1.NetAmount - dGoldTax + dRestOfAmtTax);
                                        dVGrossAmt += Convert.ToDecimal(dLineAmt);
                                        dVGoldTax = dRestOfAmtTax;
                                        dVNewTaxPct = dNewTaxPerc;

                                        dVTotTax += Convert.ToDecimal(dLineAmt - dGoldTax + dRestOfAmtTax) * dNewTaxPerc / (100 + dNewTaxPerc);

                                        dTotOgExcAmt = 0;
                                    }
                                }

                            }
                            else
                            {
                                dVNetAmt += saleLineItem1.NetAmount;
                                dVGrossAmt += saleLineItem1.GrossAmount;
                                dVTotTax += saleLineItem1.TaxAmount;
                            }
                        }
                        else
                        {
                            dVNetAmt += saleLineItem1.NetAmount;
                            dVGrossAmt += saleLineItem1.GrossAmount;
                            dVTotTax += saleLineItem1.TaxAmount;
                        }
                    }
                }

            }
            else// all tax 0 pct
            {
                foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
                {
                    iMetalType = GetMetalType(saleLineItem1.ItemId);
                    if (iMetalType == (int)MetalType.Gold)
                    {
                        if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                            && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                            && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                            && !saleLineItem1.Voided)
                        {
                            decimal dGoldTax = 0m;

                            decimal dLineAmt = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
                            decimal dLineTax = Math.Abs(Convert.ToDecimal(saleLineItem1.TaxAmount));
                            dGoldTax = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDTAX));
                            decimal dNewTaxAmt = dLineTax - dGoldTax;
                            decimal dTaxPerc = getItemTaxPercentage(saleLineItem1.ItemId);

                            if (dLineTax > 0)
                            {
                                dNewTaxPerc = (dNewTaxAmt * dTaxPerc) / dLineTax;
                                dVNetAmt += Convert.ToDecimal(dLineAmt - dGoldTax);
                                dVGrossAmt += Convert.ToDecimal(dLineAmt - dGoldTax);
                                dVNewTaxPct = dNewTaxPerc;

                                dVTotTax += (dLineAmt - dGoldTax) * dNewTaxPerc / (100 + dNewTaxPerc);
                            }
                        }
                        else
                        {
                            dVNetAmt += saleLineItem1.NetAmount;
                            dVGrossAmt += saleLineItem1.GrossAmount;
                            dVTotTax += saleLineItem1.TaxAmount;
                        }
                    }
                    else
                    {
                        dVNetAmt += saleLineItem1.NetAmount;
                        dVGrossAmt += saleLineItem1.GrossAmount;
                        dVTotTax += saleLineItem1.TaxAmount;
                    }
                }
            }
        }

        private void PreActionRecalTax(RetailTransaction retailTrans, ref decimal dTotOgExcAmt, ref decimal dTotSalesLineGoldValue, ref decimal dTotSalesTaxAmt, ref int iMetalType)
        {
            foreach (SaleLineItem saleLineItem1 in retailTrans.SaleItems)
            {
                if (!saleLineItem1.Voided)
                {
                    if (saleLineItem1.PartnerData.TransactionType == "0" && saleLineItem1.PartnerData.NimReturnLine == 0
                        && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                        && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) > 0
                        && !saleLineItem1.Voided)
                    {
                        decimal dLineGoldValue = 0m;
                        decimal dLineGoldTax = 0m;
                        bool isBulkItem = false;
                        if (IsBulkItem(saleLineItem1.ItemId))
                            isBulkItem = true;
                        else
                            isBulkItem = false;
                        if (isBulkItem == false)
                        {
                            if (!string.IsNullOrEmpty(saleLineItem1.PartnerData.LINEGOLDVALUE))
                                dLineGoldValue = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDVALUE));
                        }
                        else
                        {
                            iMetalType = GetMetalType(saleLineItem1.ItemId);
                            if (iMetalType == (int)MetalType.Gold)
                            {
                                if (!string.IsNullOrEmpty(saleLineItem1.PartnerData.Amount))
                                    dLineGoldValue = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.Amount));
                            }
                        }

                        if (!string.IsNullOrEmpty(saleLineItem1.PartnerData.LINEGOLDTAX))
                            dLineGoldTax = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDTAX));

                        dTotSalesLineGoldValue += dLineGoldValue;
                        dTotSalesTaxAmt += dLineGoldTax;
                    }
                    if ((saleLineItem1.PartnerData.TransactionType == "3" || saleLineItem1.PartnerData.TransactionType == "1")
                        && saleLineItem1.PartnerData.NimReturnLine == 0
                        && saleLineItem1.ItemType != LSRetailPosis.Transaction.Line.SaleItem.BaseSaleItem.ItemTypes.Service
                        && Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) < 0
                        && !saleLineItem1.Voided)
                    {
                        iMetalType = GetMetalType(saleLineItem1.ItemId);
                        if (iMetalType == (int)MetalType.Gold)
                            dTotOgExcAmt += Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
                    }
                }
            }
        }

        private void RecalTaxAtOgExchange(RetailTransaction retailTrans, decimal dNewTaxPerc, SaleLineItem saleLineItem1)
        {
            int iMetalType = GetMetalType(saleLineItem1.ItemId);
            decimal dGoldTax = 0m;

            decimal dLineAmt = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount));
            decimal dLineTax = Math.Abs(Convert.ToDecimal(saleLineItem1.TaxAmount));
            dGoldTax = Math.Abs(Convert.ToDecimal(saleLineItem1.PartnerData.LINEGOLDTAX));
            decimal dNewTaxAmt = dLineTax - dGoldTax;
            decimal dTaxPerc = getItemTaxPercentage(saleLineItem1.ItemId);

            // saleLineItem1.PriceOverridden = true;

            saleLineItem1.NetAmount = Convert.ToDecimal(saleLineItem1.NetAmount - dGoldTax);
            saleLineItem1.GrossAmount = dLineAmt;
            saleLineItem1.Price = (dLineAmt - dGoldTax) / saleLineItem1.Quantity;

            saleLineItem1.PartnerData.MakingAmount = Convert.ToString(Convert.ToDecimal(saleLineItem1.PartnerData.MakingAmount) - dGoldTax);
            saleLineItem1.PartnerData.TotalAmount = Convert.ToString(Convert.ToDecimal(saleLineItem1.PartnerData.TotalAmount) - dGoldTax);

            if (dLineTax > 0)
            {
                dNewTaxPerc = (dNewTaxAmt * dTaxPerc) / dLineTax;
                saleLineItem1.PartnerData.LINEGOLDTAX = "0";
                saleLineItem1.PartnerData.NewTaxPerc = dNewTaxPerc;

                saleLineItem1.TaxRatePct = dNewTaxPerc;
                saleLineItem1.TaxLines.Clear();
                saleLineItem1.CalculateLine();

                #region Recalculate Tax
                TaxService.Tax objTax = new TaxService.Tax();

                if (application != null)
                    objTax.Application = application;
                else
                    objTax.Application = Application;

                objTax.Initialize();
                objTax.CalculateTax(saleLineItem1, retailTrans);
                retailTrans.CalcTotals();
                retailTrans.CalculateAmountDue();
                #endregion
            }
            // MessageBox.Show("Recalculation of tax is done");
            // return dNewTaxPerc;
        }

        private string ShowPopUpForGiftItem(decimal dTotFreeQtyApplied, string sItemId, string sConf, string sSize, string sColor, string sFreeArticleCode, string sFreeProductType)
        {
            if (string.IsNullOrEmpty(sItemId))
            {
                frmSalesInfoCode objGI = new frmSalesInfoCode(false, true, sFreeProductType, sFreeArticleCode
                    , sConf, sSize, sColor, application, dTotFreeQtyApplied);
                objGI.ShowDialog();
                sItemId = objGI.txtRemarks.Text;
                //bool bCancel = objGI.isCancel;

                //if (dTotFreeQtyApplied > 0 && bCancel == true)
                //{
                //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Already some free item is applid, so are you want to void the transaction ? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                //    {
                //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                //        if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                //        {
                //            Application.RunOperation(PosisOperations.VoidTransaction, "");
                //        }
                //        else
                //        {
                //            frmSalesInfoCode objGI1 = new frmSalesInfoCode(false, true, sFreeProductType, sFreeArticleCode
                //                , sConf, sSize, sColor, application, dTotFreeQtyApplied);
                //            objGI1.ShowDialog();
                //            sItemId = objGI1.txtRemarks.Text;
                //            bool bCancel1 = objGI1.isCancel; 
                //        }
                //    }
                //}
            }
            return sItemId;
        }

        private static DataTable GetMultiUnadjustedAdvData(RetailTransaction retailTrans, DataTable dtNew)
        {
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("getUnadjustedAdvance", retailTrans.Customer.CustomerId);

                    DataSet dsWH = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsWH.ReadXml(srTransDetail);
                    }
                    if (dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                    {
                        dtNew = dsWH.Tables[0];
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Active Deposit found for the selected customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return dtNew;
        }

        private static DataTable GetUnadjustedLockedAdvData(string sCustId)
        {
            DataTable dtNew = new DataTable();
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("getUnadjustedLockAdvance", sCustId);

                    DataSet dsWH = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsWH.ReadXml(srTransDetail);
                    }
                    if (dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                    {
                        dtNew = dsWH.Tables[0];
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Active Deposit found for the selected customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return dtNew;
        }

        private static void unlockLockedAdvData(string sTransId, string sStoreId, string sTerminalId)
        {
            try
            {
                ReadOnlyCollection<object> containerArray;
                string sMsg = string.Empty;

                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("updateAdvanceForVoid", sTransId, sStoreId, sTerminalId);
                    sMsg = Convert.ToString(containerArray[2]);
                    bool bStatus = Convert.ToBoolean(containerArray[1]);
                    if (bStatus)
                    {
                        MessageBox.Show("Transaction " + sTransId + " is successfully unlocked");
                    }
                }
            }
            catch (Exception ex)
            {
            }
        }

        public DataTable RemoveDuplicateRows(DataTable dTable, string colName)
        {
            Hashtable hTable = new Hashtable();
            ArrayList duplicateList = new ArrayList();

            //Add list of all the unique item value to hashtable, which stores combination of key, value pair.
            //And add duplicate item value in arraylist.
            foreach (DataRow drow in dTable.Rows)
            {
                if (hTable.Contains(drow[colName]))
                    duplicateList.Add(drow);
                else
                    hTable.Add(drow[colName], string.Empty);
            }

            //Removing a list of duplicate items from datatable.
            foreach (DataRow dRow in duplicateList)
                dTable.Rows.Remove(dRow);

            //Datatable which contains unique records will be return as output.
            return dTable;
        }

        private static DataTable GetCustomerMultiAdjdada(RetailTransaction retailTrans, DataTable dt)
        {
            int iWithOutAdvOldList = 0;
            DataTable dtItemInfo = new DataTable("dtItemInfo");
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("CustomerMultiAdjustment", retailTrans.Customer.CustomerId);

                    DataSet dsWH = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsWH.ReadXml(srTransDetail);
                    }
                    if (dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                    {
                        dt = dsWH.Tables[0];
                        dtItemInfo = dt;

                        DataTable dtWithOutAdv = GetWithOutAdvCustomerOrder(retailTrans.Customer.CustomerId);

                        if (dtWithOutAdv != null && dtWithOutAdv.Rows.Count > 0)
                        {
                            iWithOutAdvOldList = 1;
                            foreach (DataRow drClone in dtWithOutAdv.Rows)
                            {
                                dtItemInfo.ImportRow(drClone);
                            }
                        }
                    }
                    else
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Active Deposit found for the selected customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }

            //if (dtItemInfo == null && dtItemInfo.Rows.Count == 0)
            //{
            DataTable dtWithOutAdv1 = GetWithOutAdvCustomerOrder(retailTrans.Customer.CustomerId);

            if (dtWithOutAdv1 != null && dtWithOutAdv1.Rows.Count > 0)
            {
                if (iWithOutAdvOldList == 0)
                {
                    if (dtItemInfo.Rows.Count == 0)
                        dtItemInfo = dtWithOutAdv1.Clone();

                    foreach (DataRow drClone in dtWithOutAdv1.Rows)
                    {
                        dtItemInfo.ImportRow(drClone);
                    }
                }
            }
            //}

            return dtItemInfo;
        }
        #endregion

        //static DataTable CreateTable(IEnumerable<Dictionary<string, object>> parrent,
        //                     DataRow row, DataTable table)
        //{
        //    if (row == null)
        //    {
        //        row = table.NewRow();
        //    }

        //    foreach (var v in parrent)
        //    {
        //        foreach (var o in v)
        //        {
        //            if (o.Value.GetType().IsGenericType)
        //            {
        //                var dic = (IEnumerable<Dictionary<string, object>>)o.Value;
        //                CreateTable(dic, row, table);
        //            }
        //            else
        //            {
        //                row[o.Key] = o.Value;
        //            }
        //        }
        //        if (row.RowState == DataRowState.Added)
        //        {
        //            DataRow tempRow = table.NewRow();
        //            tempRow.ItemArray = row.ItemArray;
        //            table.Rows.Add(tempRow);
        //        }
        //        else
        //        {
        //            table.Rows.Add(row);
        //        }
        //    }

        //    return table;
        //}

        #region - Changed By Nimbus - FOR AMOUNT ADJUSTMENT
        private DataRow AmountToBeAdjusted(string custAccount)
        {
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            SqlCon.Open();

            SqlCommand SqlComm = new SqlCommand();
            SqlComm.Connection = SqlCon;
            SqlComm.CommandType = CommandType.Text;
            SqlComm.CommandText = " SELECT     RETAILTRANSACTIONTABLE.TRANSACTIONID AS [Transaction ID], RETAILTRANSACTIONTABLE.CUSTACCOUNT AS [Customer Account], " +
                " DIRPARTYTABLE.NAMEALIAS AS [Customer Name], CAST(SUM(RETAILTRANSACTIONPAYMENTTRANS.AMOUNTCUR) AS NUMERIC(28,3)) AS [Total Amount] " +
                " FROM DIRPARTYTABLE INNER JOIN CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                " RETAILTRANSACTIONTABLE INNER JOIN RETAILTRANSACTIONPAYMENTTRANS ON RETAILTRANSACTIONTABLE.TRANSACTIONID = RETAILTRANSACTIONPAYMENTTRANS.TRANSACTIONID ON  " +
                " CUSTTABLE.ACCOUNTNUM = RETAILTRANSACTIONTABLE.CUSTACCOUNT WHERE (RETAILTRANSACTIONTABLE.CUSTACCOUNT = '" + custAccount + "') " +
                " GROUP BY RETAILTRANSACTIONTABLE.TRANSACTIONID, RETAILTRANSACTIONTABLE.CUSTACCOUNT,DIRPARTYTABLE.NAMEALIAS ";


            DataRow drSelected = null;
            DataTable AdjustmentDT = new DataTable();


            SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
            SqlDa.Fill(AdjustmentDT);
            if (AdjustmentDT != null && AdjustmentDT.Rows.Count > 0)
            {

                Dialog.WinFormsTouch.frmGenericSearch OSearch = new Dialog.WinFormsTouch.frmGenericSearch(AdjustmentDT, drSelected, "Adjustment");
                OSearch.ShowDialog();
                drSelected = OSearch.SelectedDataRow;
            }
            else
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Active Deposit found for the selected customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }
            return drSelected;
        }
        #endregion

        #region Save Customer Order Deposit Details
        private void SaveCustomerOrderDepositDetails(string customerorder, string amt)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            bool bIsGoldFixing = CheckGoldFixing();

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string sGoldRate = GetMetalRate();

            // string commandText = " UPDATE RETAILTEMPTABLE SET CUSTORDER=@CUSTORDER,GOLDFIXING=@GOLDFIXING,MINIMUMDEPOSITFORCUSTORDER=@MINAMT WHERE ID=1 ";
            string commandText = " UPDATE RETAILTEMPTABLE SET CUSTORDER=@CUSTORDER,GOLDFIXING=@GOLDFIXING,MINIMUMDEPOSITFORCUSTORDER=@MINAMT WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE

            SqlCommand command = new SqlCommand(commandText, connection);
            command.Parameters.Clear();
            command.Parameters.Add("@CUSTORDER", SqlDbType.NVarChar).Value = customerorder;


            //if(bIsGoldFixing)
            //{
            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Please select your option for Gold Fixing...Gold Rate is : " + sGoldRate, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        command.Parameters.Add("@GOLDFIXING", SqlDbType.Bit).Value = Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES" ? "True" : "False";
            //    }
            //}
            //else
            //{
            //    command.Parameters.Add("@GOLDFIXING", SqlDbType.Bit).Value = "False";
            //}
            command.Parameters.Add("@GOLDFIXING", SqlDbType.Bit).Value = "True";
            command.Parameters.Add("@MINAMT", SqlDbType.Decimal).Value = Convert.ToDecimal(amt);
            command.ExecuteNonQuery();

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        private void UpdateCustomerOrderWithAdvOrNot(string customerorder)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " UPDATE CUSTORDER_HEADER SET WITHADVANCE=1 WHERE ORDERNUM = '" + customerorder + "' ";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.ExecuteNonQuery();

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        private void ConfirmCustOrder(string customerorder)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " UPDATE CUSTORDER_HEADER SET IsConfirmed=1 WHERE ORDERNUM = '" + customerorder + "' ";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.ExecuteNonQuery();

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }



        private string GetMetalRate()
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

        private bool CheckTerminal(string sTerminalID)
        {
            bool bReturn = false;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string commandText = " SELECT TERMINALID FROM RETAILTEMPTABLE WHERE TERMINALID = '" + sTerminalID + "' "; // RETAILTEMPTABLE


            SqlCommand command = new SqlCommand(commandText, connection);

            string sResult = Convert.ToString(command.ExecuteScalar());
            connection.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                bReturn = true;
            }
            return bReturn;
        }

        private bool CheckGoldFixing() // ADDED ON 09/04/2015
        {
            bool bReturn = false;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
                connection.Open();


            string commandText = " SELECT top 1 isnull(GOLDFIXING,0) FROM RETAILPARAMETERS"; // RETAILTEMPTABLE


            SqlCommand command = new SqlCommand(commandText, connection);

            int sResult = Convert.ToInt16(command.ExecuteScalar());
            connection.Close();
            if (sResult > 0)
            {
                bReturn = true;
            }
            return bReturn;
        }
        #endregion

        #region GetCustomerOrderData
        private DataSet GetCustomerOrderData(string ordernum)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                //SqlComm.CommandText = "SELECT STAFFID,TERMINALID,ORDERNUM,CUSTACCOUNT,CUSTNAME,ORDERDATE,DELIVERYDATE,TOTALAMOUNT,isDelivered FROM CUSTORDER_HEADER WHERE ORDERNUM='" + ordernum + "'" +
                //    " SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CONFIGID],[CODE],[SIZEID],[STYLE],[INVENTDIMID],[PCS],[QTY],[CRATE] AS [RATE] " +
                //    " ,[RATETYPE],[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE],[MAKINGAMOUNT],[EXTENDEDDETAILSAMOUNT] FROM [CUSTORDER_DETAILS] WHERE ORDERNUM='" + ordernum + "' " +
                //    " SELECT *,CRATE AS RATE,0 AS MAKINGAMOUNT,0 AS EXTENDEDDETAILSAMOUNT FROM CUSTORDER_SUBDETAILS  WHERE ORDERNUM='" + ordernum + "'";

                SqlComm.CommandText = "SELECT STAFFID,TERMINALID,ORDERNUM,CUSTACCOUNT,CUSTNAME,ORDERDATE,DELIVERYDATE,TOTALAMOUNT,isDelivered,'CustOrdr' as TransType FROM CUSTORDER_HEADER WHERE ORDERNUM='" + ordernum + "'" +
                   " SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CONFIGID],[CODE],[SIZEID],[STYLE],[INVENTDIMID],[PCS],[QTY],[CRATE] AS [RATE] " +
                   " ,[RATETYPE],[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE],[MAKINGAMOUNT],[EXTENDEDDETAILSAMOUNT],'CustOrdr' as TransType FROM [CUSTORDER_DETAILS] WHERE ORDERNUM='" + ordernum + "' " +
                   " SELECT *,CRATE AS RATE,0 AS MAKINGAMOUNT,0 AS EXTENDEDDETAILSAMOUNT,'CustOrdr' as TransType FROM CUSTORDER_SUBDETAILS  WHERE ORDERNUM='" + ordernum + "'";


                DataSet EmployeeDt = new DataSet();


                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(EmployeeDt);

                return EmployeeDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
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

            sbQuery.Append("SELECT TOP(1) ADJUSTMENTITEMID FROM [RETAILPARAMETERS] ");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }
        #endregion

        #region GET SKU DATA FOR RELEASING
        private DataTable ReleaseSKU()
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                // SqlComm.CommandText = "select TRANSID AS [TRANSACTIONID], SKUNUMBER AS [SKU], ISNULL(CONVERT(VARCHAR(10),SKURELEASEDDATE,103),'') AS [RELEASEDATE],REASON FROM retailcustomerdepositskudetails";
                SqlComm.CommandText = "select distinct SKUNUMBER AS [SKU],TRANSID AS [TRANSACTIONID]," +
                                    " ISNULL(CONVERT(VARCHAR(10),SKUBOOKINGDATE,103),'') AS [SKUBOOKINGDATE]," +
                                    " ISNULL(CONVERT(VARCHAR(10),SKURELEASEDDATE,103),'') AS [RELEASEDATE]," +
                                    " REASON FROM retailcustomerdepositskudetails WHERE DELIVERED = 0 AND RELEASED = 0" +
                                    " UNION ALL select A.SKUNUMBER AS [SKU],'' TRANSACTIONID," +
                                    " ISNULL(CONVERT(VARCHAR(10),A.SKUDATE,103),'') AS [SKUBOOKINGDATE]," +
                                    " ISNULL(CONVERT(VARCHAR(10),getdate(),103),'') AS [RELEASEDATE], '' REASON" +
                                    " from SKUTable_Posted A left join SKUTableTrans B on A.SKUNUMBER=B.SKUNUMBER" +
                                    " where CUSTORDERLINENUM !=0 and CUSTORDERNUM!='' and B.isAvailable=1 and B.TOCOUNTER!='Transit'";

                DataTable SKUDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(SKUDt);

                return SKUDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region GET Customer Balance
        private DataSet CustomerBalance()
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = " SELECT     CUSTTRANS.ACCOUNTNUM AS ACCOUNT,DIRPARTYTABLE.NAMEALIAS AS CUSTOMER " +
                                    " ,CAST(CAST(SUM(CUSTTRANS.AMOUNTCUR) AS NUMERIC(16, 2)) AS VARCHAR(10)) AS BALANCE , CUSTTRANS.CURRENCYCODE AS CURRENCY " +
                                    " FROM         DIRPARTYTABLE INNER JOIN " +
                                    " CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                                    " CUSTTRANS ON CUSTTABLE.ACCOUNTNUM = CUSTTRANS.ACCOUNTNUM " +
                                    " WHERE (CUSTTRANS.SETTLEAMOUNTCUR = 0) AND (CUSTTRANS.SETTLEAMOUNTMST = 0) " +
                                    " GROUP BY CUSTTRANS.CURRENCYCODE, CUSTTRANS.ACCOUNTNUM, DIRPARTYTABLE.NAMEALIAS ;";

                SqlComm.CommandText += "  SELECT CUSTTRANS.VOUCHER,    CUSTTRANS.ACCOUNTNUM AS ACCOUNT,DIRPARTYTABLE.NAMEALIAS AS CUSTOMER,CUSTTRANS.TRANSTYPE " +
                                     " ,CAST(CAST(SUM(CUSTTRANS.AMOUNTCUR) AS NUMERIC(16, 2)) AS VARCHAR(10)) AS BALANCE , CUSTTRANS.CURRENCYCODE AS CURRENCY " +
                     " FROM         DIRPARTYTABLE INNER JOIN " +
                      " CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                      " CUSTTRANS ON CUSTTABLE.ACCOUNTNUM = CUSTTRANS.ACCOUNTNUM " +
                      " WHERE    (CUSTTRANS.SETTLEAMOUNTCUR = 0) AND (CUSTTRANS.SETTLEAMOUNTMST = 0) " +
                      " GROUP BY CUSTTRANS.CURRENCYCODE, CUSTTRANS.ACCOUNTNUM, DIRPARTYTABLE.NAMEALIAS,CUSTTRANS.TRANSTYPE,CUSTTRANS.VOUCHER ";


                DataSet CustBalDt = new DataSet();


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

        #region GET Adjustment
        private DataTable CustomerAdjustment(string custAccount)
        {
            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = " SELECT     RETAILADJUSTMENTTABLE.ORDERNUM AS [ORDER]," +
                                        " RETAILADJUSTMENTTABLE.CUSTACCOUNT AS ACCOUNT," +
                                        " CAST(CAST(SUM(RETAILADJUSTMENTTABLE.AMOUNT) AS NUMERIC(16, 2)) AS VARCHAR(18))  " +
                                        " AS AMOUNT, CAST(CAST(SUM(RETAILADJUSTMENTTABLE.GOLDQUANTITY) AS NUMERIC(16, 3)) AS VARCHAR(18)) AS QUANTITY," +
                                        " DIRPARTYTABLE.NAMEALIAS AS NAME " +
                                        " FROM         DIRPARTYTABLE INNER JOIN " +
                                        " CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
                                        " RETAILADJUSTMENTTABLE ON CUSTTABLE.ACCOUNTNUM = RETAILADJUSTMENTTABLE.CUSTACCOUNT " +
                                        " WHERE RETAILADJUSTMENTTABLE.RETAILDEPOSITTYPE=1 " +
                                        " AND RETAILADJUSTMENTTABLE.ISADJUSTED=0 AND  " +
                                        " (RETAILADJUSTMENTTABLE.CUSTACCOUNT = '" + custAccount + "') " +
                                        " GROUP BY RETAILADJUSTMENTTABLE.ORDERNUM, RETAILADJUSTMENTTABLE.CUSTACCOUNT," +
                                        " DIRPARTYTABLE.NAMEALIAS ";


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

        #region Repair- OG Purchase
        private DataTable GetOgInfoForRepairReturn(string sRepairBatchId, string sTransId)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                SqlComm.CommandText = " select ISNULL(OGFINALAMT,0) OGFINALAMT, ISNULL(EXPECTEDQUANTITY,0) EXPECTEDQUANTITY," +
                                      " B.ITEMID ,B.INVENTBATCHID ,A.MAKINGAMOUNT," +
                                      " A.MAKINGDISCOUNT,A.MakingDiscountAmount ,CONFIGID" +
                                      " from RETAIL_CUSTOMCALCULATIONS_TABLE A " +
                                      " left join RETAILTRANSACTIONSALESTRANS B on A.TRANSACTIONID=B.TRANSACTIONID " +
                                      " and A.STOREID=B.STORE And A.TERMINALID =B.TERMINALID " +
                                      " where REPAIRBATCHID='" + sRepairBatchId + "'" +
                                      " and CAST(B.TRANSACTIONID AS VARCHAR(20)) + CAST(CAST(B.LINENUM as numeric(10,0)) AS VARCHAR(10))='" + sTransId + "'" +
                                      " AND A.STOREID='" + ApplicationSettings.Database.StoreID + "' order by B.INVENTBATCHID";

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

        private DataTable GetTotalRow(string sRepairBatchId)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                SqlComm.CommandText = " select " +
                                      " CAST(B.TRANSACTIONID AS VARCHAR(20)) + CAST(CAST(B.LINENUM as numeric(10,0)) AS VARCHAR(10)) as TRANSACTIONID" +
                                      " from RETAIL_CUSTOMCALCULATIONS_TABLE A " +
                                      " left join RETAILTRANSACTIONSALESTRANS B on A.TRANSACTIONID=B.TRANSACTIONID " +
                                      " and A.STOREID=B.STORE And A.TERMINALID =B.TERMINALID " +
                                      " where REPAIRBATCHID='" + sRepairBatchId + "'" +
                                      " AND A.STOREID='" + ApplicationSettings.Database.StoreID + "' order by B.INVENTBATCHID";

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


        private DataTable GetInHouseRepReturnTotalRow(string sRepairId)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                SqlComm.CommandText = " select ITEMID,AMOUNT,QTY,PDSCWQTY  from CRWRETAILREPAIRINGREDIENT where BATCHID='" + sRepairId + "'";

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
        #endregion

        #region GET Advance Data
        public DataTable CustomerAdvanceData(string custAccount)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = " SELECT  RETAILADJUSTMENTTABLE.ORDERNUM, RETAILADJUSTMENTTABLE.ISADJUSTED,   RETAILADJUSTMENTTABLE.TRANSACTIONID AS [TransactionID], " +
               " RETAILADJUSTMENTTABLE.CUSTACCOUNT AS [CustomerAccount], " +
               " DIRPARTYTABLE.NAMEALIAS AS [CustomerName],   " +
                    // " CAST(RETAILADJUSTMENTTABLE.AMOUNT AS NUMERIC(28,3)) AS [TotalAmount]  " +
               " CAST(RETAILADJUSTMENTTABLE.AMOUNT AS NUMERIC(28,3)) AS [TotalAmount], ISNULL(GOLDFIXING,0) AS GoldFixing,(CASE WHEN GOLDFIXING = 0 THEN 0 ELSE ISNULL(GOLDQUANTITY,0) END) AS GoldQty " + //// Avg Gold Rate Adjustment
               " ,ISNULL(RETAILADJUSTMENTTABLE.RETAILSTOREID,'') AS RETAILSTOREID,ISNULL(RETAILADJUSTMENTTABLE.RETAILTERMINALID,'') AS RETAILTERMINALID" +
               " FROM         DIRPARTYTABLE INNER JOIN " +
               " CUSTTABLE ON DIRPARTYTABLE.RECID = CUSTTABLE.PARTY INNER JOIN " +
               " RETAILADJUSTMENTTABLE ON CUSTTABLE.ACCOUNTNUM = RETAILADJUSTMENTTABLE.CUSTACCOUNT " +
               " WHERE     (RETAILADJUSTMENTTABLE.ISADJUSTED = 0) AND (RETAILADJUSTMENTTABLE.RETAILDEPOSITTYPE = 1) " +
               " AND (RETAILADJUSTMENTTABLE.CUSTACCOUNT = '" + custAccount + "') ";


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

        #region GSS Maturity Adjustment Item ID
        private void GSSAdjustmentItemID(ref string sGSSAdjItemId, ref string sGSSDiscItemId,
            ref string sGSSAmountAdjItemId, ref string sGSSAmountDiscItemId, ref int iGssDiscAgreementBase)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT GSSADJUSTMENTITEMID,GSSDISCOUNTITEMID,GSSAMOUNTADJUSTMENTITEMID,GSSAMOUNTDISCOUNTITEMID,GSSDISCOUNTAGREEMENTBASE");
            sbQuery.Append(" FROM [RETAILPARAMETERS] WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "' ");
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
                sGSSAmountAdjItemId = Convert.ToString(dtGSS.Rows[0]["GSSAMOUNTADJUSTMENTITEMID"]);
                sGSSAmountDiscItemId = Convert.ToString(dtGSS.Rows[0]["GSSAMOUNTDISCOUNTITEMID"]);
                iGssDiscAgreementBase = Convert.ToInt16(dtGSS.Rows[0]["GSSDISCOUNTAGREEMENTBASE"]);
            }

        }
        #endregion

        #region GET OrderData
        private DataTable OrderData(string custAccount)
        {
            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "select CUSTACCOUNT AS ACCOUNT,CUSTNAME AS NAME ,ORDERNUM AS [ORDER],CONVERT(VARCHAR(10),ORDERDATE,103) AS ORDERDATE " +
                                      " ,CONVERT(VARCHAR(10),DELIVERYDATE,103) AS DELIVERYDATE,CAST(TOTALAMOUNT AS NUMERIC(26,2)) AS AMOUNT " +
                                      " FROM CUSTORDER_HEADER WHERE isDelivered=0 AND isConfirmed=1 " +
                                      " AND CUSTACCOUNT='" + custAccount + "' ";


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

        #region GET BookedSKU
        private DataTable BookedSKU(string orderNum, string account)
        {
            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "select distinct SKUNUMBER,LineNum FROM RETAILCUSTOMERDEPOSITSKUDETAILS WHERE" +
                                      " ORDERNUMBER='" + orderNum + "' AND CUSTOMERID='" + account + "' AND DELIVERED=0 AND RELEASED = 0" +
                                      " union " +
                                      " select SKUNUMBER,CUSTORDERLINENUM  LineNum from SKUTable_Posted where CUSTORDERNUM ='" + orderNum + "'";

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

        #region GetRepairData

        private DataSet GetRepairData(string ordernum)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                //SqlComm.CommandText = "SELECT STAFFID,TERMINALID,ORDERNUM,CUSTACCOUNT,CUSTNAME,ORDERDATE,DELIVERYDATE,TOTALAMOUNT,isDelivered FROM CUSTORDER_HEADER WHERE ORDERNUM='" + ordernum + "'" +
                //    " SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CONFIGID],[CODE],[SIZEID],[STYLE],[INVENTDIMID],[PCS],[QTY],[CRATE] AS [RATE] " +
                //    " ,[RATETYPE],[AMOUNT],[MAKINGRATE],[MAKINGRATETYPE],[MAKINGAMOUNT],[EXTENDEDDETAILSAMOUNT] FROM [CUSTORDER_DETAILS] WHERE ORDERNUM='" + ordernum + "' " +
                //    " SELECT *,CRATE AS RATE,0 AS MAKINGAMOUNT,0 AS EXTENDEDDETAILSAMOUNT FROM CUSTORDER_SUBDETAILS  WHERE ORDERNUM='" + ordernum + "'";

                // consider One item at a time will take for repair 
                SqlComm.CommandText = "SELECT A.RETAILSTAFFID AS STAFFID ,A.RETAILTERMINALID AS TERMINALID, A.RETAILSTOREID AS STOREID,A.REPAIRID AS ORDERNUM,A.CUSTACCOUNT,A.CUSTNAME,A.ORDERDATE,'Repair' as TransType, B.AMOUNT AS [TOTALAMOUNT]" +
                    " FROM RetailRepairHdr A INNER JOIN RetailRepairDetail B ON A.REPAIRID = B.REPAIRID WHERE A.REPAIRID ='" + ordernum + "'" +
                    " SELECT REPAIRID AS [ORDERNUM],[LINENUM],[RETAILSTOREID],[RETAILTERMINALID],[ITEMID],[INVENTDIMID],[PCS],[QTY],[AMOUNT],'Repair' as TransType" +
                    " FROM [RetailRepairDetail] WHERE REPAIRID ='" + ordernum + "' " +
                    " SELECT REPAIRID AS [ORDERNUM],[LINENUM],[LINENUMDTL] AS [ORDERDETAILNUM],[RETAILSTOREID],[RETAILTERMINALID],[ITEMID],[INVENTDIMID],[PCS],[QTY],[AMOUNT],'Repair' as TransType FROM RetailRepairSubDetail  WHERE REPAIRID='" + ordernum + "'";


                DataSet EmployeeDt = new DataSet();


                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(EmployeeDt);

                return EmployeeDt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region [Repair Return]

        private DataTable GetRepairReturnData(string sRepairBatchId, string sCustAccount = "", int iIsInhouseOrOutSide = 0)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                if (sRepairBatchId == "")
                {
                    SqlComm.CommandText = "SELECT H.REPAIRID AS [Repair No],d.BatchId [Batch Id], OrderDate as [Order Date]," +
                                        " CUSTACCOUNT AS [Customer Account], " +
                                        " CustName as [Customer Name] " +
                                        " from RetailRepairHdr H left join RetailRepairDetail D on H.RepairId =D.RepairId " +
                                        " where CUSTACCOUNT = '" + sCustAccount + "' AND H.IsDelivered = 0 and D.IsDelivered=0 and H.RepairType=" + iIsInhouseOrOutSide + "";

                }
                else
                {
                    SqlComm.CommandText = "SELECT B.BatchId as REPAIRID,A.OrderDate,A.CUSTACCOUNT, A.CustName,ISNULL(A.CUSTADDRESS,'') AS CUSTADDRESS," +
                                        " ISNULL(A.CUSTPHONE,'') AS CUSTPHONE,B.ITEMID" +
                                        " ,ISNULL(X.TRANSACTIONID,'') AS TRANSACTIONID,ISNULL(A.RetailStoreId,'') AS RETAILSTOREID," +
                                        " ISNULL(A.RetailTerminalId,'') AS RETAILTERMINALID from RetailRepairHdr a" +
                                        " left join RetailRepairDetail b on a.RepairId=b.RepairId" +
                                        " LEFT OUTER JOIN RETAILADJUSTMENTTABLE X ON A.REPAIRID = X.REPAIRID AND X.ISADJUSTED = 0" +
                                        " WHERE b.BatchId = '" + sRepairBatchId + "' and a.RepairType=" + iIsInhouseOrOutSide + "";
                }

                DataTable dtRepairRet = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtRepairRet);

                return dtRepairRet;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region getPaymentTransData
        public DataTable getPaymentTransData(string _sSqlStr)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = _sSqlStr;


                DataTable dtGSS = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtGSS);

                return dtGSS;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        #endregion

        #region getSchemeCodeByGssAcc
        private string getSchemeCodeByGssAcc(string sGssAcc)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT SCHEMECODE FROM [GSSACCOUNTOPENINGPOSTED] where GSSACCOUNTNO='" + sGssAcc + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToString(cmd.ExecuteScalar());
        }
        #endregion

        public string GetCompanyName(SqlConnection conn)
        {
            string sCName = string.Empty;

            string sQry = "SELECT ISNULL(A.NAME,'') FROM DIRPARTYTABLE A INNER JOIN COMPANYINFO B" +
                " ON A.RECID = B.RECID WHERE B.DATAAREA = '" + ApplicationSettings.Database.DATAAREAID + "'";

            //using (SqlCommand cmd = new SqlCommand(sQry, conn))
            //{
            SqlCommand cmd = new SqlCommand(sQry, conn);
            cmd.CommandTimeout = 0;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            sCName = Convert.ToString(cmd.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            //}

            return sCName;

        }

        public string GetCompanyTaxRegNum(SqlConnection conn)
        {
            string sCName = string.Empty;

            string sQry = "SELECT ISNULL(A.COREGNUM,'') FROM COMPANYINFO A " +
                " WHERE A.DATAAREA = '" + ApplicationSettings.Database.DATAAREAID + "'";
            // " and NativeLanguage='" + ApplicationSettings.Terminal.CultureName +"'";

            //using (SqlCommand cmd = new SqlCommand(sQry, conn))
            //{
            SqlCommand cmd = new SqlCommand(sQry, conn);
            cmd.CommandTimeout = 0;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            sCName = Convert.ToString(cmd.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            //}

            return sCName;

        }

        /// <summary>
        /// Purpose : added for dynamically comp logo into report
        /// Created Date :26/08/2014
        /// Created By : RHossain
        /// </summary>
        /// <param name="sTransId"></param>
        /// <returns></returns>
        public byte[] GetCompLogo(string sDataAreaId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT [IMAGE] FROM CompanyImage WHERE DATAAREAID='" + sDataAreaId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            object sResult = command.ExecuteScalar();

            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (sResult != null)
                return (byte[])sResult;
            else
                return null;

        }

        public string GetCurrencySymbol()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT SYMBOL FROM CURRENCY WHERE CURRENCYCODE='" + ApplicationSettings.Terminal.StoreCurrency + "'");

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

        public string AmtinwdsForInd(double amt)
        {
            object[] words = new object[28];
            string Awds = null;
            string x = null;
            string y = null;
            string a = null;
            string t = null;
            string cror = null;
            string lakh = null;
            string lak2 = null;
            string thou = null;
            string tho2 = null;
            string hund = null;
            string rupe = null;
            string rup2 = null;
            string pais = null;
            string pai2 = null;

            words[1] = "One ";
            words[2] = "Two ";
            words[3] = "Three ";
            words[4] = "Four ";
            words[5] = "Five ";
            words[6] = "Six ";
            words[7] = "Seven ";
            words[8] = "Eight ";
            words[9] = "Nine ";
            words[10] = "Ten ";
            words[11] = "Eleven ";
            words[12] = "Twelve ";
            words[13] = "Thirteen ";
            words[14] = "Fourteen ";
            words[15] = "Fifteen ";
            words[16] = "Sixteen ";
            words[17] = "Seventeen ";
            words[18] = "Eighteen ";
            words[19] = "Ninteen ";
            words[20] = "Twenty ";
            words[21] = "Thirty ";
            words[22] = "Forty ";
            words[23] = "Fifty ";
            words[24] = "Sixty ";
            words[25] = "Seventy ";
            words[26] = "Eighty ";
            words[27] = "Ninety ";

            if (amt >= 1)
            {
                Awds = " ";//Rupees
            }
            else
            {
                Awds = " "; //Rupee
            }
            x = (amt.ToString("0.00")).PadLeft(12, Convert.ToChar("0"));
            cror = x.Substring(1, 1);
            lakh = x.Substring(2, 2);
            lak2 = x.Substring(3, 1);
            thou = x.Substring(4, 2);
            tho2 = x.Substring(5, 1);
            hund = x.Substring(6, 1);
            rupe = x.Substring(7, 2);
            rup2 = x.Substring(8, 1);
            pais = x.Substring(10, 2);
            pai2 = x.Substring(11, 1);
            y = "";
            if (Convert.ToInt32(cror) > 0)
            {
                y = words[Convert.ToInt32(cror)].ToString() + "crores ";
            }
            t = Convert.ToString(lakh);
            if (Convert.ToInt32(t) > 0)
            {
                if (Convert.ToInt32(t) > 20)
                {
                    a = lakh.Substring(0, 1);
                    y = y + words[18 + Convert.ToInt32(a)];
                    if (Convert.ToInt32(lak2) != 0)
                        y = y + words[Convert.ToInt32(lak2)];
                    else
                        y = y + "";
                }
                else
                {
                    y = y + words[Convert.ToInt32(t)];
                }
                y = y + "lakhs ";
            }
            t = Convert.ToString(thou);
            if (Convert.ToInt32(t) > 0)
            {
                if (Convert.ToInt32(t) > 20)
                {
                    a = thou.Substring(0, 1);
                    y = y + words[18 + Convert.ToInt32(a)];
                    if (Convert.ToInt32(tho2) != 0)
                        y = y + words[Convert.ToInt32(tho2)];
                    else
                        y = y + "";
                }
                else
                {
                    y = y + words[Convert.ToInt32(t)];
                }
                y = y + "thousand ";
            }
            if (Convert.ToInt32(hund) > 0)
            {
                y = y + words[Convert.ToInt32(hund)] + "hundred ";
            }
            t = Convert.ToString(rupe);
            if (Convert.ToInt32(t) > 0)
            {
                if (Convert.ToInt32(t) > 20)
                {
                    a = rupe.Substring(0, 1);
                    y = y + words[18 + Convert.ToInt32(a)];
                    if (Convert.ToInt32(rup2) != 0)
                        y = y + words[Convert.ToInt32(rup2)];
                    else
                        y = y + "";
                }
                else
                {
                    y = y + words[Convert.ToInt32(t)];
                }
            }
            t = Convert.ToString(pais);
            if (Convert.ToInt32(t) > 0)
            {
                y = y + "paise ";
                if (Convert.ToInt32(t) > 20)
                {
                    a = pais.Substring(0, 1);
                    y = y + words[18 + Convert.ToInt32(a)];
                    if (Convert.ToInt32(pai2) != 0)
                        y = y + words[Convert.ToInt32(pai2)];
                    else
                        y = y + "";
                }
                else
                {
                    y = y + words[Convert.ToInt32(t)];
                }
            }
            string amtwrd = "";
            if (y.Length > 0)
            {
                amtwrd = Awds + y + "only ";
            }
            return amtwrd;
        }

        public string Amtinwds(double amt)
        {
            MultiCurrency objMulC = null;
            if (Convert.ToString(ApplicationSettings.Terminal.StoreCurrency) != "INR")
                objMulC = new MultiCurrency(Criteria.Foreign);
            else
                objMulC = new MultiCurrency(Criteria.Indian);
            Color cBlack = Color.FromName("Black");

            if (Convert.ToString(ApplicationSettings.Terminal.StoreCurrency) == "INR")
                return AmtinwdsForInd(Math.Abs(Convert.ToDouble(amt)));//GetCurrencyNameWithCode() + " " +
            else
                return objMulC.ConvertToWord(Convert.ToString(amt)) + "" + " Only";//GetCurrencyNameWithCode() + " " +
        }

        public string AmtinwdsInArabic(double amt)//newAmtToWordsInAra
        {
            MultiCurrency objMulC = null;
            ToArabic objAr = new ToArabic();

            if (Convert.ToString(ApplicationSettings.Terminal.StoreCurrency) != "INR")
                objMulC = new MultiCurrency(Criteria.Foreign);
            else
                objMulC = new MultiCurrency(Criteria.Indian);
            Color cBlack = Color.FromName("Black");

            return objAr.ConvertToText(amt) + "  " + GetCurrencyNameInArabicWithCode();
        }

        private string GetCurrencyNameWithCode()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT TXT FROM CURRENCY WHERE CURRENCYCODE='" + ApplicationSettings.Terminal.StoreCurrency + "'");

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
                return " ";
        }


        private string GetCurrencyNameInArabicWithCode()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT TXTARABIC FROM CURRENCY WHERE CURRENCYCODE='" + ApplicationSettings.Terminal.StoreCurrency + "'");

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

        //public DataTable GetHeaderInfo(string sOrderNo)
        //{
        //    try
        //    {

        //        SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
        //        SqlCon.Open();

        //        SqlCommand SqlComm = new SqlCommand();
        //        SqlComm.Connection = SqlCon;
        //        SqlComm.CommandType = CommandType.Text;
        //        SqlComm.CommandText = "select ORDERNUM as ORDERNO,CONVERT(VARCHAR(15),ORDERDATE,103) ORDERDATE,CONVERT(VARCHAR(15),DELIVERYDATE,103) DELIVERYDATE,CUSTACCOUNT as CUSTID,CUSTNAME," +
        //                              " CUSTADDRESS as CUSTADD,CUSTPHONE,CAST(ISNULL(TOTALAMOUNT,0)AS DECIMAL(18,2)) AS TOTALAMOUNT FROM CUSTORDER_HEADER" +
        //                              " WHERE ORDERNUM='" + sOrderNo + "'";

        //        DataTable CustBalDt = new DataTable();

        //        SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
        //        SqlDa.Fill(CustBalDt);
        //        var SelectedCust = customerDataManager.GetTransactionalCustomer(CustBalDt.Rows[0]["CUSTID"].ToString());
        //        CustBalDt.Rows[0]["CUSTNAME"] = Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations.GetCustomerNameWithSalutation(SelectedCust);
        //        CustBalDt.Rows[0]["CUSTADD"] = Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations.AddressLines(SelectedCust);
        //        sPincode = SelectedCust.PostalCode;
        //        return CustBalDt;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        private static string getFullStateName(string sStateId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            string commandText = string.Empty;

            commandText = "SELECT [NAME] FROM [dbo].[LOGISTICSADDRESSSTATE] WHERE [STATEID]=@STATEID";

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            command.Parameters.Add("@STATEID", SqlDbType.VarChar, 10).Value = sStateId;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim().ToUpper();
            }
            else
            {
                return "-";
            }
        }

        private static string GetCountryFullName(string sCountryRegionId, string sLanguageId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            StringBuilder commandText = new StringBuilder();
            if (string.IsNullOrEmpty(sLanguageId))
                sLanguageId = CultureInfo.CurrentUICulture.IetfLanguageTag;
            commandText.Append("select SHORTNAME from LOGISTICSADDRESSCOUNTRYREGIONTRANSLATION where COUNTRYREGIONID='" + sCountryRegionId + "' and LANGUAGEID='" + sLanguageId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim().ToUpper();
            }
            else
            {
                return "-";
            }
        }

        internal static string AddressLines(LSRetailPosis.Transaction.Customer customer)
        {
            string[] Addresslines = new string[6];

            Addresslines[0] = customer.StreetName;
            Addresslines[1] = customer.City;
            Addresslines[2] = customer.DistrictName;
            Addresslines[3] = BlankOperations.getFullStateName(customer.State);
            Addresslines[4] = BlankOperations.GetCountryFullName(customer.Country, customer.Language);
            Addresslines[5] = customer.PostalCode;
            string sConcatedAddr = null;
            sConcatedAddr += Addresslines[0];
            if (!string.IsNullOrWhiteSpace(sConcatedAddr))
                sConcatedAddr += "\n";
            if (!string.IsNullOrWhiteSpace(sConcatedAddr) && !string.IsNullOrWhiteSpace(Addresslines[1]))
                sConcatedAddr += Addresslines[1];
            if (!string.IsNullOrWhiteSpace(sConcatedAddr) && !string.IsNullOrWhiteSpace(Addresslines[2]))
                sConcatedAddr += ", " + Addresslines[2];
            sConcatedAddr += "\n";
            sConcatedAddr += Addresslines[3];
            if (!string.IsNullOrWhiteSpace(Addresslines[3]))
            {
                if (!string.IsNullOrWhiteSpace(Addresslines[4]))
                    sConcatedAddr += ", " + Addresslines[4];

            }
            return sConcatedAddr;
        }

        private static string GetSalutation(string sCustomerId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            StringBuilder commandText = new StringBuilder();

            commandText.Append("SELECT isnull(RETAILSALUTATION,0) from CUSTTABLE WHERE ACCOUNTNUM='" + sCustomerId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            int iResult = Convert.ToInt16(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            string salutation = Enum.GetName(typeof(Salutation), iResult);

            #region added for Other Salutation @p.jana 18/12/2014
            if (salutation == "")
                salutation = string.Empty;
            #endregion

            return salutation;
        }

        internal static string GetCustomerNameWithSalutation(LSRetailPosis.Transaction.Customer customer)
        {
            string sName = BlankOperations.GetSalutation(customer.CustomerId);
            if (sName == "None" || string.IsNullOrEmpty(sName))
                sName = Convert.ToString(customer.Name).Trim();
            else
                sName = Convert.ToString(sName + " " + customer.Name).Trim();
            return sName;
        }

        //public DataTable GetDetailInfo(string sOrderNo)
        //{
        //    try
        //    {
        //        SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
        //        SqlCon.Open();

        //        SqlCommand SqlComm = new SqlCommand();
        //        SqlComm.Connection = SqlCon;
        //        SqlComm.CommandType = CommandType.Text;
        //        SqlComm.CommandText = "select A.ITEMID as SKUID, A.ITEMID + '-' + F.NAME as ITEMID,PCS,QTY,CRate as RATE,AMOUNT,MAKINGRATE,MAKINGAMOUNT," +
        //                              " LineTotalAmt as TOTALAMOUNT,REMARKSDTL as REMARKS,IsBookedSKU as IsBooked,A.CONFIGID,A.CODE,A.SIZEID,A.WastageAmount " +
        //                              " AS WastageAmt FROM CUSTORDER_DETAILS A" +
        //                              " INNER JOIN INVENTTABLE D ON A.ITEMID = D.ITEMID " +
        //                              " LEFT OUTER JOIN ECORESPRODUCT E ON D.PRODUCT = E.RECID " +
        //                              " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT and f.LANGUAGEID='en-us'" +
        //                              " WHERE ORDERNUM='" + sOrderNo + "'"; // SKUID for get the itemid for image selection of parent item id of that item

        //        DataTable CustBalDt = new DataTable();

        //        SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
        //        SqlDa.Fill(CustBalDt);

        //        # region //ADDED ON 28/10/14 RH For image show in sales voucher

        //        string sItemParentId = string.Empty;
        //        string sArchivePath = string.Empty;

        //        DataTable dtDetail = new DataTable();
        //        dtDetail = CustBalDt;

        //        dtDetail.Columns.Add("ORDERLINEIMAGE", typeof(string));
        //        int i = 1;
        //        foreach (DataRow d in dtDetail.Rows)
        //        {
        //            sArchivePath = GetArchivePathFromImage();
        //            string path = sArchivePath + "" + sOrderNo + "_" + i + ".jpeg"; //

        //            if (File.Exists(path))
        //            {
        //                Image img = Image.FromFile(path);
        //                byte[] arr;
        //                using (MemoryStream ms1 = new MemoryStream())
        //                {
        //                    img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
        //                    arr = ms1.ToArray();
        //                }

        //                d["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
        //            }
        //            else
        //            {
        //                sSaleItem = Convert.ToString(d["SKUID"]);
        //                sItemParentId = GetItemParentId(sSaleItem);

        //                if (sItemParentId == "-")
        //                    sItemParentId = sSaleItem;

        //                path = sArchivePath + "" + sItemParentId + "" + ".jpeg"; //

        //                if (File.Exists(path))
        //                {
        //                    Image img = Image.FromFile(path);
        //                    byte[] arr;
        //                    using (MemoryStream ms1 = new MemoryStream())
        //                    {
        //                        img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
        //                        arr = ms1.ToArray();
        //                    }

        //                    d["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
        //                }
        //                else
        //                    d["ORDERLINEIMAGE"] = "";

        //            }
        //            i++;
        //        }
        //        dtDetail.AcceptChanges();
        //        #endregion//end

        //        return dtDetail;

        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        private string GetItemParentId(string sSalesItem)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select ITEMIDPARENT  from INVENTTABLE  where ITEMID='" + sSalesItem + "'");

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

        public string GetArchivePathFromImage()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select TOP(1) ARCHIVEPATH  from RETAILSTORETABLE where STORENUMBER='" + ApplicationSettings.Database.StoreID + "'");

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

        public void NIM_GetOGSketch(string sImage, string sArchivePath, string sPurchInvoiceNo, int iLineId)
        {
            if (!string.IsNullOrEmpty(sImage))
            {
                string path = sArchivePath + "" + sPurchInvoiceNo + "_" + iLineId + ".jpeg"; //

                Bitmap b = new Bitmap(Convert.ToString(sImage));
                b.Save(Convert.ToString(path));
            }
        }

        public void NIM_SaveOrderSampleSketch(string sImage, string sArchivePath, string sPurchInvoiceNo)
        {
            if (!string.IsNullOrEmpty(sImage))
            {
                if (ImageFormat.Jpeg == GetImageFormat(sImage))
                {
                    string path = sArchivePath + "" + sPurchInvoiceNo + "" + ImageFormat.Jpeg;//".jpeg"; //

                    Bitmap b = new Bitmap(Convert.ToString(sImage));
                    b.Save(Convert.ToString(path));
                }
            }
        }

        private static ImageFormat GetImageFormat(string fileName)
        {
            string extension = Path.GetExtension(fileName);
            if (string.IsNullOrEmpty(extension))
                throw new ArgumentException(
                    string.Format("Unable to determine file extension for fileName: {0}", fileName));

            switch (extension.ToLower()) //.jpg, *.jpeg, *.jpe, *.jfif
            {
                case @".jpg":
                case @".jpe":
                case @".jfif":
                case @".jpeg":
                    return ImageFormat.Jpeg;

                default:
                    throw new NotImplementedException();
            }
        }

        private bool AskUserToCustomerDeposit()
        {
            bool IsDeposit = false;
            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer Deposit?", MessageBoxButtons.YesNo, MessageBoxIcon.Information))
            {
                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                if (dialog.DialogResult == DialogResult.Yes)
                {
                    IsDeposit = true;
                }
            }
            return IsDeposit;
        }

        private void SaveRepairOrderDepositDetails(string repairId, string repairAmnt)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            if (string.IsNullOrWhiteSpace(repairAmnt))
                repairAmnt = "0";

            string commandText = " UPDATE RETAILTEMPTABLE SET CUSTID='REPAIR', CUSTORDER=@REPAIRID,GOLDFIXING=@GOLDFIXING,MINIMUMDEPOSITFORCUSTORDER=@MINAMT WHERE ID=1 AND TERMINALID = '" + ApplicationSettings.Terminal.TerminalId + "' "; // RETAILTEMPTABLE

            SqlCommand command = new SqlCommand(commandText, connection);
            command.Parameters.Clear();
            command.Parameters.Add("@REPAIRID", SqlDbType.NVarChar).Value = repairId;


            command.Parameters.Add("@GOLDFIXING", SqlDbType.Bit).Value = "False";

            command.Parameters.Add("@MINAMT", SqlDbType.Decimal).Value = Convert.ToDecimal(repairAmnt);
            command.ExecuteNonQuery();
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private void RepairAdjItemId(ref string NIM_REPAIRADJITEM)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT CRWREPAIRADJITEM FROM [RETAILPARAMETERS] WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "' ");
            DataTable dtGSS = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    NIM_REPAIRADJITEM = Convert.ToString(reader.GetValue(0));
                }
            }
            reader.Close();
            reader.Dispose();
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
        }

        private Decimal GetRepairItemConfigRate(string sRepairId)
        {
            decimal dRetAmt = 0M;
            //get rate from db
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            StringBuilder commandText = new StringBuilder();
            commandText.AppendLine(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.AppendLine(" DECLARE @CONFIGID VARCHAR(20) ");
            commandText.AppendLine(" DECLARE @ITEMID VARCHAR(20) ");
            commandText.AppendLine(" DECLARE @METALTYPE INT  ");
            commandText.AppendLine(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM RETAILCHANNELTABLE INNER JOIN  ");
            commandText.AppendLine(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.AppendLine(" WHERE RETAILSTORETABLE.STORENUMBER='" + ApplicationSettings.Database.StoreID + "' ");
            commandText.AppendLine(" SELECT TOP 1 @CONFIGID=CONFIGID,@ITEMID=ITEMID FROM RETAILREPAIRDETAIL WHERE REPAIRID='" + sRepairId + "'; ");
            commandText.AppendLine(" SELECT TOP 1 @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID=@ITEMID; ");
            commandText.AppendLine(" SELECT TOP 1 CAST(RATES AS NUMERIC(28,2))AS RATES ");
            commandText.AppendLine(" FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  AND METALTYPE = @METALTYPE AND ACTIVE=1  AND CONFIGIDSTANDARD=@CONFIGID and RETAIL=1  ");
            commandText.AppendLine(" --AND RATETYPE = 4   ");
            commandText.AppendLine(" ORDER BY DATEADD(SECOND, [TIME], [TRANSDATE]) DESC ");

            using (SqlCommand cmd = new SqlCommand(commandText.ToString(), connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            dRetAmt = Convert.ToDecimal(reader.GetValue(reader.GetOrdinal("RATES")));
                        }
                    }
                    reader.Close();
                    reader.Dispose();
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dRetAmt;
        }

        public DataTable NIM_LoadCombo(string _tableName, string _fieldName, string _condition = null, string _sqlStr = null)
        {
            try
            {
                // Open Sql Connection  
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                // Create a Command  
                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                if (!string.IsNullOrEmpty(_sqlStr))
                    SqlComm.CommandText = _sqlStr;
                else
                    SqlComm.CommandText = "select  " + _fieldName + " " +
                                            " FROM " + _tableName + " " +
                                            " " + _condition + " ";

                DataTable dtComboField = new DataTable();
                DataRow row = dtComboField.NewRow();
                dtComboField.Rows.InsertAt(row, 0);

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtComboField);

                return dtComboField;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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
            , ref string sColor, ref string sSize, ref string sFreeProductType, ref string sFreeArticleCode)
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
                            " FROM  CRWGIFTITEMPROMOTIONAGREEMENT ag " +
                            " left join INVENTDIM as id on ag.INVENTDIMID = id.INVENTDIMID " +
                            " WHERE (STOREGROUP = CASE WHEN TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN TABLEGROUPALL=0" +
                            " THEN '" + ApplicationSettings.Database.StoreID + "' WHEN TABLEGROUPALL=2 THEN '' END)" +
                            " AND (PRODUCTTYPECODE='" + sProductType + "')" +
                            " and (" + dAmt + " BETWEEN FROMAMOUNT AND TOAMOUNT)  " +
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
                reader.Close();
                reader.Dispose();

                if (connection.State == ConnectionState.Open)
                    connection.Close();

                string commandText1 = "SELECT   CAST(ISNULL(PDSCWQTY,0) AS DECIMAL(28,0)) PDSCWQTY, " +
                                       " CAST(ISNULL(QTY,0) AS DECIMAL(28,3)) QTY,ISNULL(PROMOTIONCODE,'') PROMOTIONCODE,ISNULL(ITEMID,'') ITEMID,  " +
                                       " isnull(CONFIGID,'') CONFIGID,isnull(INVENTSIZEID,'') INVENTSIZEID,isnull(INVENTCOLORID,'') INVENTCOLORID,  " +
                                       " isnull(FreeArticleCode,'') FreeArticleCode,isnull(FreeProductType,'') FreeProductType  " +
                                       " FROM  CRWGIFTITEMPROMOTIONAGREEMENT ag  left join INVENTDIM as id on ag.INVENTDIMID = id.INVENTDIMID " +
                                       " WHERE (STOREGROUP = CASE WHEN TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN TABLEGROUPALL=0 THEN '" + ApplicationSettings.Database.StoreID + "' " +
                                       " WHEN TABLEGROUPALL=2 THEN '' END) AND (PRODUCTTYPECODE='" + sProductType + "') " +
                                       " and TOAMOUNT = (select CAST(ISNULL(max(TOAMOUNT),0) AS DECIMAL(28,3))  " +
                                       " TOAMOUNT from CRWGIFTITEMPROMOTIONAGREEMENT " +
                                       " ag1 left join INVENTDIM as id on ag1.INVENTDIMID = id.INVENTDIMID  " +
                                       " where (STOREGROUP = CASE WHEN TABLEGROUPALL=1 THEN '" + sStoreGrp + "' WHEN TABLEGROUPALL=0 THEN  " +
                                       " '" + ApplicationSettings.Database.StoreID + "' WHEN TABLEGROUPALL=2 THEN '' END) " +
                                       " AND (PRODUCTTYPECODE='" + sProductType + "') " +
                                       " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) " +
                                       " BETWEEN FROMDATE AND TODATE)   AND CRWCONFIRM = 1 AND ag1.DATAAREAID ='" + ApplicationSettings.Database.DATAAREAID + "') " +
                                       " and TOAMOUNT < " + dAmt + "" +
                                       " AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN FROMDATE AND TODATE) " +
                                       " AND CRWCONFIRM = 1 AND ag.DATAAREAID ='" + ApplicationSettings.Database.DATAAREAID + "'   " +
                                       " ORDER BY ITEMID DESC,PROMOTIONCODE DESC";

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                SqlCommand command1 = new SqlCommand(commandText1.ToString(), connection);
                command1.CommandTimeout = 0;
                SqlDataReader reader1 = command1.ExecuteReader();

                if (reader1.HasRows)
                {
                    while (reader1.Read())
                    {
                        dCWQTY = Convert.ToDecimal(reader1.GetValue(0));
                        dQTY = Convert.ToDecimal(reader1.GetValue(1));
                        sPCode = Convert.ToString(reader1.GetValue(2));
                        sItemID = Convert.ToString(reader1.GetValue(3));
                        sConf = Convert.ToString(reader1.GetValue(4));
                        sColor = Convert.ToString(reader1.GetValue(5));
                        sSize = Convert.ToString(reader1.GetValue(6));
                        sFreeArticleCode = Convert.ToString(reader1.GetValue(7));
                        sFreeProductType = Convert.ToString(reader1.GetValue(8));
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
            }
            reader.Close();
            reader.Dispose();

            if (connection.State == ConnectionState.Open)
                connection.Close();

        }

        private void GETOGRepairInfo(string sTableName, decimal dQty, string sInvntBatchId)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" IF NOT EXISTS (SELECT A.NAME FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN CREATE TABLE " + sTableName + "(");
            commandText.Append(" QTY NUMERIC (28,3),INVENTBATCHID NVARCHAR(30)) END");
            commandText.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN  INSERT INTO  " + sTableName + "  VALUES('" + dQty + "','" + sInvntBatchId + "') END");

            //commandText.Append("IF (EXISTS (SELECT * ");
            //commandText.Append(" FROM INFORMATION_SCHEMA.TABLES ");
            //commandText.Append(" WHERE TABLE_SCHEMA = 'dbo' ");
            //commandText.Append(" AND  TABLE_NAME = '" + sTableName + "'))");
            //commandText.Append(" BEGIN");
            //commandText.Append(" delete from  " + sTableName + "; ");
            //commandText.Append(" INSERT INTO   " + sTableName + "  VALUES('" + dQty + "','" + sInvntBatchId + "') ");
            //commandText.Append(" END");
            //commandText.Append(" else");
            //commandText.Append(" BEGIN");
            //commandText.Append(" CREATE TABLE  " + sTableName + "( QTY NUMERIC (28,3),INVENTBATCHID NVARCHAR(30))");
            //commandText.Append(" END ");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private void getMultiAdjOrderInfo(string sTableName, string sOrderNo, string sCustAcc, int iLine = 0)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" IF NOT EXISTS (SELECT A.NAME FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN CREATE TABLE " + sTableName + "(");
            commandText.Append(" LINENUM INT,ORDERNO NVARCHAR(20),CUSTACC NVARCHAR(20)) END");
            commandText.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN delete from " + sTableName + "; INSERT INTO  " + sTableName + "  VALUES(" + iLine + ",'" + sOrderNo + "','" + sCustAcc + "') END");

            //commandText.Append("IF (EXISTS (SELECT * ");
            //commandText.Append(" FROM INFORMATION_SCHEMA.TABLES ");
            //commandText.Append(" WHERE TABLE_SCHEMA = 'dbo' ");
            //commandText.Append(" AND  TABLE_NAME = '" + sTableName + "'))");
            //commandText.Append(" BEGIN");
            //commandText.Append(" delete from  " + sTableName + "; ");
            //commandText.Append(" INSERT INTO   " + sTableName + "  VALUES(" + iLine + ",'" + sOrderNo + "','" + sCustAcc + "') ");
            //commandText.Append(" END");
            //commandText.Append(" else");
            //commandText.Append(" BEGIN");
            //commandText.Append(" CREATE TABLE  " + sTableName + "( LINENUM INT,ORDERNO NVARCHAR(20),CUSTACC NVARCHAR(20))");
            //commandText.Append(" END ");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private DataTable GetBookedInfo(string sOrderNo)  // SKU allow
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            DataTable dt = new DataTable();
            string commandText = "SELECT ITEMID,cast(LineNum as Int) LineNum FROM CUSTORDER_DETAILS WHERE" +
                " ORDERNUM = '" + sOrderNo + "' and (ISNULL(IsBookedSKU,0) = 1 or ISNULL(FESTIVECODE,'') != '') ";
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(commandText, conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dt;
        }

        private void SaveBookedSKU(IPosTransaction POSTrans, string sOrderNo, string sSKUNo, string sCustId, int iLine)
        {
            // LSRetailPosis.Transaction.CustomerPaymentTransaction PayTrans = POSTrans as LSRetailPosis.Transaction.CustomerPaymentTransaction;

            string commandText = " IF NOT EXISTS(SELECT TOP 1 * FROM RETAILCUSTOMERDEPOSITSKUDETAILS " +
                                   " WHERE ORDERNUMBER=@ORDERNUMBER " +
                                   " AND SKUNUMBER=@SKUNUMBER AND LINENUM=@LINENUM) BEGIN  " +
                                   " INSERT INTO [RETAILCUSTOMERDEPOSITSKUDETAILS] " +
                                   " ([TRANSID],[CUSTOMERID],[ORDERNUMBER],[SKUNUMBER],[SKUBOOKINGDATE],[SKURELEASEDDATE] " +
                                   " ,[SKUSALEDATE],[DELIVERED],[STOREID],[TERMINALID],[DATAAREAID],[STAFFID],[LINENUM]) " +
                                   "  VALUES " +
                                   " (@TRANSID,@CUSTOMERID,@ORDERNUMBER,@SKUNUMBER,@SKUBOOKINGDATE,@SKURELEASEDDATE,@SKUSALEDATE " +
                                   " ,@DELIVERED,@STOREID,@TERMINALID,@DATAAREAID,@STAFFID,@LINENUM) END ";

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(commandText, conn))
            {
                command.Parameters.Add("@TRANSID", SqlDbType.NVarChar, 20).Value = Convert.ToString(POSTrans.TransactionId);
                command.Parameters.Add("@CUSTOMERID", SqlDbType.NVarChar, 20).Value = Convert.ToString(sCustId);
                command.Parameters.Add("@ORDERNUMBER", SqlDbType.NVarChar, 20).Value = sOrderNo;
                command.Parameters.Add("@SKUNUMBER", SqlDbType.NVarChar, 20).Value = sSKUNo;
                command.Parameters.Add("@LINENUM", SqlDbType.Int).Value = iLine;
                command.Parameters.Add("@SKUBOOKINGDATE", SqlDbType.Date).Value = Convert.ToDateTime(DateTime.Now).Date;
                command.Parameters.Add("@SKURELEASEDDATE", SqlDbType.DateTime, 20).Value = Convert.ToDateTime("1/1/1900 12:00:00 AM");
                command.Parameters.Add("@SKUSALEDATE", SqlDbType.DateTime, 60).Value = Convert.ToDateTime("1/1/1900 12:00:00 AM");
                command.Parameters.Add("@DELIVERED", SqlDbType.Bit).Value = false;
                command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Database.DATAAREAID;

                command.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 20).Value = Convert.ToString(ApplicationSettings.Terminal.TerminalOperator.OperatorId);

                command.ExecuteNonQuery();
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

        }


        #region start :Multi copy print
        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        public void Export(LocalReport report)
        {
            PageSettings m_pageSettings = new PageSettings();
            m_pageSettings.PaperSize = report.GetDefaultPageSettings().PaperSize;

            string deviceInfo =
              "<DeviceInfo>" +
              "  <OutputFormat>EMF</OutputFormat>" +
              "  <PageWidth>5.83in</PageWidth>" +
              "  <PageHeight>8.27in</PageHeight>" +
              "  <MarginTop>0.11811in</MarginTop>" +
              "  <MarginLeft>0.11811in</MarginLeft>" +
              "  <MarginRight>0.11811in</MarginRight>" +
              "  <MarginBottom>0.19685in</MarginBottom>" +
              "</DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in m_streams)
            {
                stream.Position = 0;
            }
        }

        public void ExportForEstimation(LocalReport report)
        {
            string deviceInfo =
              "<DeviceInfo>" +
              "  <OutputFormat>EMF</OutputFormat>" +
              "  <PageWidth>3in</PageWidth>" +
              "  <PageHeight>11in</PageHeight>" +
              "  <MarginTop>0.005in</MarginTop>" +
              "  <MarginLeft>0in</MarginLeft>" +
              "  <MarginRight>0.001in</MarginRight>" +
              "  <MarginBottom>0in</MarginBottom>" +
              "</DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in m_streams)
            {
                stream.Position = 0;
            }
        }
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
           Metafile(m_streams[currentPageIndex]);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            currentPageIndex++;
            ev.HasMorePages = (currentPageIndex < m_streams.Count);
        }

        public void Print_Estimation()
        {

            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                try
                {
                    printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                    currentPageIndex = 0;
                    printDoc.Print();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            Dispose();
        }

        public void Print_Invoice(LocalReport report, Int16 iNoOfCopy)
        {

            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                try
                {
                    PageSettings m_pageSettings = new PageSettings();
                    m_pageSettings.PaperSize = report.GetDefaultPageSettings().PaperSize;
                    printDoc.DefaultPageSettings.PaperSize = report.GetDefaultPageSettings().PaperSize;

                    printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                    currentPageIndex = 0;
                    printDoc.PrinterSettings.Copies = iNoOfCopy;
                    if (iNoOfCopy > 0)
                        printDoc.Print();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            Dispose();
        }
        public void Dispose()
        {
            if (m_streams != null)
            {
                foreach (Stream stream in m_streams)
                    stream.Close();
                m_streams = null;
            }
        }
        #endregion start : print

        public DataTable NIM_LoadCombo(string _sqlStr)
        {
            try
            {
                // Open Sql Connection  
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                // Create a Command  
                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;


                SqlComm.CommandText = _sqlStr;


                DataTable dtComboField = new DataTable();
                //DataRow row = dtComboField.NewRow();
                //dtComboField.Rows.InsertAt(row, 0);

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtComboField);

                return dtComboField;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
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

        private decimal GetGoldBookingRatePer()
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
                                 " SELECT ISNULL(FIXEDRATEADVANCEPCT,0) FROM RETAILPARAMETERS WHERE DATAAREAID = '" + application.Settings.Database.DataAreaID + "'";

            SqlCommand command = new SqlCommand(commandText, connection);
            // string strCustOrder = Convert.ToString(command.ExecuteScalar());
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    dFixRatePct = Convert.ToDecimal(reader.GetValue(0));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            /* BlankOperations.WinFormsTouch.frmInputGoldBookingPercentage objBlankOp = new BlankOperations.WinFormsTouch.frmInputGoldBookingPercentage();
             objBlankOp.ShowDialog();
             dFixRatePct = objBlankOp.dPct;*/

            return dFixRatePct;
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
                commandString += "SELECT top 1 @CONFIGID = [GSSDefaultConfigIdGold] from RETAILPARAMETERS ";
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

        private decimal getGoldTax(decimal dLineGoldValue, string sBaseItemID)
        {
            decimal dGVal = 0m;
            decimal dTaxPerc = 0m;
            decimal dTaxGoldAmt = 0m;

            if (dLineGoldValue > 0)
            {
                dGVal = dLineGoldValue;
                dTaxPerc = getItemTaxPercentage(sBaseItemID);

                if (dTaxPerc > 0 && dGVal > 0)
                    dTaxGoldAmt = decimal.Round(dGVal * (dTaxPerc / 100), 2, MidpointRounding.AwayFromZero);// Convert.ToString(dGVal * (dTaxPerc / 100));
                else
                    dTaxGoldAmt = 0;
            }
            //else if (isBulkItem == true && iMetalType == (int)MetalType.Gold)
            //{
            //    if (!string.IsNullOrEmpty(txtTotalAmount.Text))
            //    {
            //        dGVal = Convert.ToDecimal(txtTotalAmount.Text);
            //        dTaxPerc = getItemTaxPercentage(sBaseItemID);

            //        if (dTaxPerc > 0 && dGVal > 0)
            //            dTaxGoldAmt = decimal.Round(dGVal * (dTaxPerc / 100), 2, MidpointRounding.AwayFromZero);// Convert.ToString(dGVal * (dTaxPerc / 100));
            //        else
            //            dTaxGoldAmt = 0;
            //    }
            //}
            else
                dTaxGoldAmt = 0;

            return dTaxGoldAmt;
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

        protected int GetMetalType(string sItemId)
        {
            int iMetalType = 100;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select metaltype from inventtable where itemid='" + sItemId + "'");

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            iMetalType = Convert.ToInt16(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();
            return iMetalType;

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
            command.Dispose();

            return bBulkItem;
        }

        private void GetItemType(string item) // for Malabar based on this new item type sales radio button or rest of the radio  button will be on/off
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

        private bool IsGSSEmiInclOfTax(ref decimal dGSSTaxPct)
        {
            bool bResult = false;
            int iGSSTax = 0;

            SqlConnection connection = new SqlConnection();
            try
            {
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                if (connection.State == ConnectionState.Closed)
                    connection.Open();

                StringBuilder commandText = new StringBuilder();
                commandText.Append(" select isnull(GSSEmiInclOfTax,0),isnull(GSSTAXPERCENTAGE,0) from RETAILPARAMETERS where  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

                SqlCommand cmd = new SqlCommand(commandText.ToString(), connection);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        iGSSTax = Convert.ToInt16(reader.GetValue(0));
                        dGSSTaxPct = Convert.ToInt16(reader.GetValue(1));
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
            if (iGSSTax == 1)
                bResult = true;

            return bResult;
        }

        private bool chkOGTaxApplicable()
        {
            bool bResult = false;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT isnull(OGTaxApplicable,0) FROM RETAILPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    int iResult = (int)reader.GetValue(0);

                    if (iResult == 1)
                        bResult = true;
                    else
                        bResult = false;

                }
            }
            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();
            return bResult;
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

            sCompName = GetCompanyName(conn);
            //datasources
            List<ReportDataSource> rds = new List<ReportDataSource>();
            rds.Add(new ReportDataSource("HEADERINFO", (DataTable)GetHeaderInfo(sOrderNo)));
            rds.Add(new ReportDataSource("DETAILINFO", (DataTable)GetDetailInfo(sOrderNo)));
            rds.Add(new ReportDataSource("CUSTORDINGR", (DataTable)GetCustOrderIngr(sOrderNo)));
            string sAmtinwds = Amtinwds(Math.Abs(Convert.ToDouble(getTotAmtOfOrder(sOrderNo)))); // added on 28/04/2014 RHossain               
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
                //var SelectedCust = customerDataManager.GetTransactionalCustomer(CustBalDt.Rows[0]["CUSTID"].ToString());
                //CustBalDt.Rows[0]["CUSTNAME"] = Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations.GetCustomerNameWithSalutation(SelectedCust);
                //CustBalDt.Rows[0]["CUSTADD"] = Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations.AddressLines(SelectedCust);
                //sPincode = SelectedCust.PostalCode;
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
                                      " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT and F.LANGUAGEID ='en-us'" +
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
                                     " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT and F.LANGUAGEID='en-us'" +
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

                SqlComm.CommandText = " select A.ORDERNUM OrderNum,cast(B.LineNum as Int) LineNum ,A.CUSTACCOUNT as CustAccount" +
                                      " ,A.CUSTNAME as CustomerName," +
                                      " CAST(Sum(B.AMOUNT) AS NVARCHAR) as Amount,CAST(Sum(B.QTY) AS NVARCHAR) as GoldQuantity from CUSTORDER_HEADER A" +
                                      " left join CUSTORDER_DETAILS B on A.ORDERNUM=B.ORDERNUM" +
                                      " where a.ISFIXEDQTY=0 and a.isDelivered=0 and b.isDelivered=0 " +
                                      " and a.WITHADVANCE=0 and a.CUSTACCOUNT='" + sCustAcc + "'" +
                                      " group by A.ORDERNUM,A.CUSTACCOUNT,A.CUSTNAME,B.LineNum  order by B.LINENUM";

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

        #region IsCustOrder SUVARNAVRUDHI
        public bool IsCustOrderSUVARNAVRUDHI(string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT SUVARNAVRUDHI  FROM [CUSTORDER_HEADER] WHERE ORDERNUM='" + sOrderNo + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        private bool IsCustOrderWithAdv(string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT WITHADVANCE  FROM [CUSTORDER_HEADER] WHERE ORDERNUM='" + sOrderNo + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }


        public DateTime getRequestDeliveryDate(string sOrderNo)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT REQUESTDELIVERYDATE FROM [CUSTORDER_HEADER] WHERE ORDERNUM='" + sOrderNo + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToDateTime(cmd.ExecuteScalar());
        }

        public void SendSMS(string sTextSMS, string sMNo)
        {
            sMNo = "9614951382";
            sTextSMS = "Hi ur OTP is 0000";

            //string sSMSAPI = "http://bulkpush.mytoday.com/BulkSms/SingleMsgApi?feedid=345711&username=8451045896&password=dgddj&To=" + sMNo + "&Text=" + sTextSMS + "";
            string sSMSAPI = "https://alerts.solutionsinfini.com/api/v4/?method=sms&api_key=Af41aacde212c092176d8989abd194a10&username=6312696&password=var@1&To=" + sMNo + "&Text=" + sTextSMS + "";

            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(sSMSAPI);


            HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
            System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
            string responseString = respStreamReader.ReadToEnd();

            DataSet dsSMS = new DataSet();
            if (!string.IsNullOrEmpty(Convert.ToString(responseString)))
            {
                StringReader reader = new StringReader(Convert.ToString(responseString));
                dsSMS.ReadXml(reader);
            }

            respStreamReader.Close();
            myResp.Close();

        }
        #endregion

        #endregion


        private void DropTempTable(string sTblName)
        {
            SqlConnection conn = new SqlConnection();
            //string sTblName = "NEGCASHPAY" + ApplicationSettings.Terminal.TerminalId;
            //string sTblName1 = "DAILYCASHPAY" + ApplicationSettings.Terminal.TerminalId;
            //string sTblName2 = "DAILYCASHPAYNEG" + ApplicationSettings.Terminal.TerminalId;
            //string sTblName3 = "MAKINGINFO" + ApplicationSettings.Terminal.TerminalId;

            #region Cash pay limit temp table drop
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string sSQLQry = "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName + "')" +
                          " BEGIN  DROP TABLE " + sTblName + " END ";
            //sSQLQry = sSQLQry + "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName1 + "')" +
            //            " BEGIN  DROP TABLE " + sTblName1 + " END ";
            //sSQLQry = sSQLQry + "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName2 + "')" +
            //            " BEGIN  DROP TABLE " + sTblName2 + " END ";
            //sSQLQry = sSQLQry + "IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTblName3 + "')" +
            //            " BEGIN  DROP TABLE " + sTblName3 + " END ";

            using (SqlCommand command = new SqlCommand(sSQLQry, conn))
            {
                command.ExecuteNonQuery();
            }
            #endregion
        }

        #region start :Multi copy print
        /*private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            Stream stream = new MemoryStream();
            m_streams.Add(stream);
            return stream;
        }

        public void Export(LocalReport report)
        {
            string deviceInfo =
              "<DeviceInfo>" +
              "  <OutputFormat>EMF</OutputFormat>" +
              "  <PageWidth>8.5in</PageWidth>" +
              "  <PageHeight>11.02362in</PageHeight>" +
              "  <MarginTop>0.11811in</MarginTop>" +
              "  <MarginLeft>0.03937in</MarginLeft>" +
              "  <MarginRight>0.03937in</MarginRight>" +
              "  <MarginBottom>0.11811in</MarginBottom>" +
              "</DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in m_streams)
            {
                stream.Position = 0;
            }
        }

        public void ExportForEstimation(LocalReport report)
        {
            string deviceInfo =
              "<DeviceInfo>" +
              "  <OutputFormat>EMF</OutputFormat>" +
              "  <PageWidth>3in</PageWidth>" +
              "  <PageHeight>11in</PageHeight>" +
              "  <MarginTop>0.005in</MarginTop>" +
              "  <MarginLeft>0in</MarginLeft>" +
              "  <MarginRight>0.001in</MarginRight>" +
              "  <MarginBottom>0in</MarginBottom>" +
              "</DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<Stream>();
            report.Render("Image", deviceInfo, CreateStream, out warnings);
            foreach (Stream stream in m_streams)
            {
                stream.Position = 0;
            }
        }
        private void PrintPage(object sender, PrintPageEventArgs ev)
        {
            Metafile pageImage = new
           Metafile(m_streams[currentPageIndex]);

            // Adjust rectangular area with printer margins.
            Rectangle adjustedRect = new Rectangle(
                ev.PageBounds.Left - (int)ev.PageSettings.HardMarginX,
                ev.PageBounds.Top - (int)ev.PageSettings.HardMarginY,
                ev.PageBounds.Width,
                ev.PageBounds.Height);

            // Draw a white background for the report
            ev.Graphics.FillRectangle(Brushes.White, adjustedRect);

            // Draw the report content
            ev.Graphics.DrawImage(pageImage, adjustedRect);

            // Prepare for the next page. Make sure we haven't hit the end.
            currentPageIndex++;
            ev.HasMorePages = (currentPageIndex < m_streams.Count);
        }

        public void Print_Estimation()
        {

            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                try
                {
                    printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                    currentPageIndex = 0;
                    printDoc.Print();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            Dispose();
        }

        public void Print_Invoice(Int16 iNoOfCopy)
        {

            if (m_streams == null || m_streams.Count == 0)
                throw new Exception("Error: no stream to print.");
            PrintDocument printDoc = new PrintDocument();
            if (!printDoc.PrinterSettings.IsValid)
            {
                throw new Exception("Error: cannot find the default printer.");
            }
            else
            {
                try
                {
                    printDoc.PrintPage += new PrintPageEventHandler(PrintPage);
                    currentPageIndex = 0;
                    printDoc.PrinterSettings.Copies = iNoOfCopy;
                    if (iNoOfCopy > 0)
                        printDoc.Print();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }

            }
            Dispose();
        }

        public void Dispose()
        {
            if (m_streams != null)
            {
                foreach (Stream stream in m_streams)
                    stream.Close();
                m_streams = null;
            }
        }*/
        #endregion start : print
    }
}
