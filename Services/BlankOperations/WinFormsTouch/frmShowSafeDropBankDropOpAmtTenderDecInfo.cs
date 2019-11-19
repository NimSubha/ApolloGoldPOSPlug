using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LSRetailPosis.Transaction;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Reporting.WinForms;
using BarcodeLib.Barcode;
using System.IO;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmShowSafeDropBankDropOpAmtTenderDecInfo : Form
    {
        #region variable
        private Panel panel2;
        private Button btnSearch;
        private Label label3;
        private ComboBox cmbTransType;
        private Panel panel1;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private Button btnPrint;
        private Button btnExit;


        SqlConnection connection;
        BlankOperations oBlank = new BlankOperations();
        string sBC = string.Empty;

        string sInvoiceNo = "";
        string sInvDt = "-";

        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";

        string sDataAreaId = "";
        string sInventLocationId = "";

        string sAmtinwds = "";
        string sReceiptNo = "-";

        string sTerminal = string.Empty;
        string sTitle = string.Empty;
        string sType = "";
        string sVouType = "";

        // string sStorePh = "-";       
        string sTime = "";

        string sCompanyName = string.Empty;
        private Button btnClear;
        RetailTransaction retailTrans;
        #endregion
        private Label label1;
        private DateTimePicker dtpTDate;
        private Label label11;
        private DateTimePicker dtpFDate;
        DataTable dtCol = new DataTable();

        enum VouTransType
        {
            None = 0,
            SafeDrop = 1,
            BankDrop = 2,
            StartAmountDeclaration = 3, 
            TenderDeclaration = 4,  
        }

        public frmShowSafeDropBankDropOpAmtTenderDecInfo()
        {
            InitializeComponent();
        }

        public frmShowSafeDropBankDropOpAmtTenderDecInfo(SqlConnection conn)
        {
            InitializeComponent();
            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            cmbTransType.DataSource = Enum.GetValues(typeof(VouTransType));
        }

        private void GetData()
        {
            int iVoucherType = cmbTransType.SelectedIndex;

            if (iVoucherType == 4)
            {
                SqlCommand command = new SqlCommand("GETTENDERDECLAREFOREXTRAVOU", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
               
                command.Parameters.Add("@TransDate", SqlDbType.DateTime).Value = dtpFDate.Value.ToShortDateString();
                command.Parameters.Add("@STOREID", SqlDbType.NVarChar).Value = ApplicationSettings.Database.StoreID;
                command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
                SqlDataAdapter daCol = new SqlDataAdapter(command);
                daCol.Fill(dtCol);
            }
            else
            {
                SqlCommand command = new SqlCommand("GET_BD_SD_OC_TD_DATA", connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();

                command.Parameters.Add("@TYPE", SqlDbType.Int).Value = iVoucherType;
                command.Parameters.Add("@FDate", SqlDbType.DateTime).Value = dtpFDate.Value.ToShortDateString();
                command.Parameters.Add("@TDate", SqlDbType.DateTime).Value = dtpTDate.Value.ToShortDateString();
                command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
                SqlDataAdapter daCol = new SqlDataAdapter(command);
                daCol.Fill(dtCol);
            }
            grItems.DataSource = dtCol;
        }

        private void InitializeComponent()
        {
            this.panel2 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.dtpTDate = new System.Windows.Forms.DateTimePicker();
            this.label11 = new System.Windows.Forms.Label();
            this.dtpFDate = new System.Windows.Forms.DateTimePicker();
            this.btnClear = new System.Windows.Forms.Button();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbTransType = new System.Windows.Forms.ComboBox();
            this.btnExit = new System.Windows.Forms.Button();
            this.panel1 = new System.Windows.Forms.Panel();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.btnPrint = new System.Windows.Forms.Button();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.dtpTDate);
            this.panel2.Controls.Add(this.label11);
            this.panel2.Controls.Add(this.dtpFDate);
            this.panel2.Controls.Add(this.btnClear);
            this.panel2.Controls.Add(this.btnSearch);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.cmbTransType);
            this.panel2.Location = new System.Drawing.Point(12, 12);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(859, 34);
            this.panel2.TabIndex = 201;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label1.Location = new System.Drawing.Point(440, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(49, 13);
            this.label1.TabIndex = 275;
            this.label1.Text = "To Date:";
            // 
            // dtpTDate
            // 
            this.dtpTDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpTDate.Location = new System.Drawing.Point(495, 6);
            this.dtpTDate.Name = "dtpTDate";
            this.dtpTDate.Size = new System.Drawing.Size(102, 20);
            this.dtpTDate.TabIndex = 274;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label11.Location = new System.Drawing.Point(269, 9);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(59, 13);
            this.label11.TabIndex = 273;
            this.label11.Text = "From Date:";
            // 
            // dtpFDate
            // 
            this.dtpFDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFDate.Location = new System.Drawing.Point(329, 6);
            this.dtpFDate.Name = "dtpFDate";
            this.dtpFDate.Size = new System.Drawing.Size(102, 20);
            this.dtpFDate.TabIndex = 272;
            // 
            // btnClear
            // 
            this.btnClear.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClear.ForeColor = System.Drawing.Color.Black;
            this.btnClear.Location = new System.Drawing.Point(788, -1);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(70, 34);
            this.btnClear.TabIndex = 204;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.btnSearch.Image = global::Microsoft.Dynamics.Retail.Pos.BlankOperations.Properties.Resources.Magnify;
            this.btnSearch.Location = new System.Drawing.Point(694, -1);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(79, 34);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.label3.Location = new System.Drawing.Point(3, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(96, 13);
            this.label3.TabIndex = 202;
            this.label3.Text = "Transaction Type :";
            // 
            // cmbTransType
            // 
            this.cmbTransType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbTransType.ForeColor = System.Drawing.SystemColors.ControlText;
            this.cmbTransType.FormattingEnabled = true;
            this.cmbTransType.Location = new System.Drawing.Point(101, 5);
            this.cmbTransType.Name = "cmbTransType";
            this.cmbTransType.Size = new System.Drawing.Size(154, 21);
            this.cmbTransType.TabIndex = 1;
            this.cmbTransType.SelectedIndexChanged += new System.EventHandler(this.cmbTransType_SelectedIndexChanged);
            // 
            // btnExit
            // 
            this.btnExit.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnExit.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.btnExit.Location = new System.Drawing.Point(792, 455);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(70, 31);
            this.btnExit.TabIndex = 200;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.grItems);
            this.panel1.Location = new System.Drawing.Point(12, 52);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(859, 397);
            this.panel1.TabIndex = 202;
            // 
            // grItems
            // 
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grItems.Location = new System.Drawing.Point(0, 0);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(857, 395);
            this.grItems.TabIndex = 1;
            this.grItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(37)))), ((int)(((byte)(127)))));
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.Appearance.Row.Options.UseForeColor = true;
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grItems;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.RowHeight = 30;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            // 
            // btnPrint
            // 
            this.btnPrint.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnPrint.ForeColor = System.Drawing.Color.Green;
            this.btnPrint.Location = new System.Drawing.Point(691, 455);
            this.btnPrint.Name = "btnPrint";
            this.btnPrint.Size = new System.Drawing.Size(70, 31);
            this.btnPrint.TabIndex = 203;
            this.btnPrint.Text = "Print";
            this.btnPrint.UseVisualStyleBackColor = true;
            this.btnPrint.Click += new System.EventHandler(this.btnPrint_Click);
            // 
            // frmShowSafeDropBankDropOpAmtTenderDecInfo
            // 
            this.ClientSize = new System.Drawing.Size(883, 489);
            this.Controls.Add(this.btnPrint);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btnExit);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmShowSafeDropBankDropOpAmtTenderDecInfo";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            grItems.DataSource = null;
            cmbTransType.DataSource = Enum.GetValues(typeof(VouTransType));
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            int iVT = cmbTransType.SelectedIndex;
            
            if (grdView.RowCount > 0)
            {
                int SelectedRow = grdView.GetSelectedRows()[0];
                DataRow theRowToSelect = dtCol.Rows[SelectedRow];
                string sT = Convert.ToString(theRowToSelect["TERMINAL"]);
                DateTime dtTransDate = Convert.ToDateTime(theRowToSelect["TRANSDATE"]);
                string sStore = Convert.ToString(theRowToSelect["STORE"]);
                string sTransId = Convert.ToString(theRowToSelect["TRANSACTIONID"]);
                string sReceiptId = Convert.ToString(theRowToSelect["RECEIPTID"]);
                decimal dAmt = Convert.ToDecimal(theRowToSelect["AMOUNTTENDERED"]);
                string sTime= Convert.ToString(theRowToSelect["TRANSTIME"]);
                frmIncomeExpVoucher objIncExp = new frmIncomeExpVoucher(connection, sT, dtTransDate, sTime, sStore, sTransId, sReceiptId, dAmt, iVT);
                objIncExp.ShowDialog();
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            dtCol = new DataTable();
            grItems.DataSource = null;
            GetData();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void cmbTransType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbTransType.SelectedIndex == 4)
            {
                label11.Text = "Trans Date";
                label1.Visible = false;
                dtpTDate.Visible = false;
            }
            else
            {
                label11.Text = "From Date";
                label1.Visible = true;
                dtpTDate.Visible = true;
            }
        }
    }
}
