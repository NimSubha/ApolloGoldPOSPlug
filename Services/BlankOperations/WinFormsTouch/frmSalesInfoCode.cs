using System;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using System.Data;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmSalesInfoCode : Form
    {
        private IApplication application;
        public string sCodeOrRemarks = string.Empty;
        bool iBatch = false;
        bool isGift = false;
        string sProduct = "";
        string sArticle = "";
        string sConfig = "";
        string sSize = "";
        string sColor = "";
        public bool isCancel = false;
        decimal dAppQty = 0m;

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

        public frmSalesInfoCode()
        {
            InitializeComponent();
            btnClose.Visible = false;
        }

        public frmSalesInfoCode(bool isBath)
        {
            InitializeComponent();
            iBatch = isBath;
            btnClose.Visible = false;
            if (iBatch)
                lblVendorAccount.Text = "Batch No";
        }

        public frmSalesInfoCode(bool isBath, bool isGiftItem, string sProd, string sA, string sC, string sS, string sCo, IApplication app, decimal dTotFreeQtyApplied)
        {
            InitializeComponent();
            iBatch = false;
            isGift = isGiftItem;
            if (isGift)
            {
                lblVendorAccount.Text = "Gift item";
                sProduct = sProd;
                sArticle = sA;
                sConfig = sC;
                sSize = sS;
                sColor = sCo;
                application = app;

                btnClose.Visible = true;
                dAppQty = dTotFreeQtyApplied;

            }
            else
                btnClose.Visible = false;
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRemarks.Text.Trim()))
            {
                if (isGift)
                {
                    if (IsValidFreeGiftItem(sProduct, sArticle, sConfig, sSize, sColor))
                    {
                        sCodeOrRemarks = txtRemarks.Text.Trim();
                        this.Close();
                    }
                    else
                        MessageBox.Show("Please enter valid gift item.");
                }
                else
                {
                    sCodeOrRemarks = txtRemarks.Text.Trim();
                    this.Close();
                }
            }
            else
            {
                if (iBatch)
                    MessageBox.Show("Please enter batch no.");
                else if (isGift)
                    MessageBox.Show("Please enter valid gift item.");
                else
                    MessageBox.Show("Please enter PAN no.");

                txtRemarks.Focus();
            }

        }

        private bool IsValidFreeGiftItem(string sProdId, string sArticle, string sConfig, string sSize, string sColor)
        {
            bool bResult = false;
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " SELECT isnull(RETAILVARIANTID,'') FROM INVENTDIMCOMBINATION WHERE ITEMID='" + txtRemarks.Text.Trim() + "'AND INVENTDIMID in(" +
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

            if (!string.IsNullOrEmpty(sRETAILVARIANTID))
                bResult = true;
            else
                bResult = false;

            return bResult;
        }

        private void txtRemarks_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13) // this line added on 27/05/2014
            {
                btnOk_Click(sender, e);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (dAppQty > 0)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Already some free item is applid, so are you want to void the transaction ? ", MessageBoxButtons.YesNo, MessageBoxIcon.Question))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    if (Convert.ToString(dialog.DialogResult).ToUpper().Trim() == "YES")
                    {
                        Application.RunOperation(PosisOperations.VoidTransaction, "");
                        this.Close();
                    }
                    else
                        txtRemarks.Focus();
                }
            }
            else
            {
                txtRemarks.Text = "";
                this.Close();
            }
        }
    }
}
