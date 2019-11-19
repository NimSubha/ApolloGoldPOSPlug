using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class SKUUnlock : frmTouchBase
    {

        SqlConnection SqlCon;
        DataTable dt;

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

        public SKUUnlock()
        {
            InitializeComponent();
        }

        public SKUUnlock(SqlConnection Conn, DataTable dtTrans)
        {
            InitializeComponent();
            SqlCon = Conn;
            dt = dtTrans;
        }

        private void btnUnlock_Click(object sender, EventArgs e)
        {
            if(!string.IsNullOrEmpty(txtSKUNo.Text.Trim()))
            {
                string sRtlTransId = string.Empty;
                if (IsLocked(txtSKUNo.Text.Trim(), out sRtlTransId))
                {
                    string sMsg = "Please release the SKU from suspended transaction"; 

                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(sMsg, MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    return;
                }

                string sUnlockMsg = IsBookedSKU(txtSKUNo.Text.Trim());

                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(sUnlockMsg, MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool IsLocked(string sSKUNo, out string sTransID)
        {
            bool bIsLocked = false;
            LSRetailPosis.DataAccess.SuspendRetrieveData objData = new LSRetailPosis.DataAccess.SuspendRetrieveData(SqlCon, ApplicationSettings.Database.DATAAREAID);
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    RoundingService.Rounding roun = new RoundingService.Rounding();
                    PosTransaction pSuspended = objData.GetSuspendedTransaction(SqlCon,
                                                                     Convert.ToString(dr[0]), roun);
                    RetailTransaction RTSuspended = pSuspended as RetailTransaction;
                    if (RTSuspended != null)
                    {
                        foreach (SaleLineItem Sl in RTSuspended.SaleItems)
                        {
                            if (Sl.ItemId == sSKUNo)
                            {
                                bIsLocked = true;
                                sTransID = RTSuspended.TransactionId;
                                return bIsLocked;
                            }
                        }
                    }
                }
            }

            sTransID = string.Empty;
            return bIsLocked;
        }
        private string IsBookedSKU(string sSKUNo)
        {
            string sMsg = string.Empty;


            string commandText = " DECLARE @SKUNUMBER AS NVARCHAR(20) DECLARE @UNLOCKMSG AS NVARCHAR(300)  SET @SKUNUMBER = '" + sSKUNo + "'" +
                                 " IF EXISTS(SELECT SkuNumber FROM SKUTableTrans WHERE SkuNumber = @SKUNUMBER AND ISNULL(isAvailable,0) = 1)" +
                                 " BEGIN  IF EXISTS (SELECT top(1)SkuNumber FROM RETAILCUSTOMERDEPOSITSKUDETAILS WHERE SkuNumber = @SKUNUMBER AND RELEASED = 0)" +
                                 " BEGIN SET @UNLOCKMSG = 'SKU is booked, cannot be unlocked'  END" +
                                 " ELSE  BEGIN UPDATE SKUTableTrans SET isLocked = 0 WHERE SkuNumber = @SKUNUMBER" +
                                 " SET @UNLOCKMSG = 'SKU is unlocked for sale' END  END" +
                                 " ELSE  BEGIN  SET @UNLOCKMSG = 'SKU is sold / invalid' END  SELECT @UNLOCKMSG";

            if (SqlCon.State == ConnectionState.Closed)
            {
                SqlCon.Open();
            }

            SqlCommand command = new SqlCommand(commandText, SqlCon);
            command.CommandTimeout = 0;

            sMsg = Convert.ToString(command.ExecuteScalar());
            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();

            return sMsg;
        }
    }
}
