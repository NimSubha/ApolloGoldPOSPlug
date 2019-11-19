/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.POSProcesses.WinControls;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.DataEntity;
using DE = Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using TransactionStatus = LSRetailPosis.Transaction.PosTransaction.TransactionStatus;
using TypeOfTransaction = LSRetailPosis.Transaction.PosTransaction.TypeOfTransaction;

namespace Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch
{
    /// <summary>
    /// Summary description for frmJournal.
    /// </summary>
    public class frmJournal : frmTouchBase
    {
        #region User Interface Components

        private JournalData journalData;
        #endregion

        #region Member Variables

        private SalesOrderSearchCriteria searchCriteria;
        private DateTime? searchDate;
        private string searchReceiptId;
        private IApplication Application;
        private IPosTransaction posTransaction;
        private string selectedTransactionId = string.Empty;
        private string selectedStoreId = string.Empty;
        private string selectedTerminalId = string.Empty;
        private const int maxRowsAtEachQuery = 100;
        private DataTable transactions;
        private JournalDialogResults journalDialogResults;
        private Object journalDialogResultObject;
        private string salesText;

        #endregion

        private TableLayoutPanel tableLayoutPanel1;
        private SimpleButtonEx btnReceipt;
        private SimpleButtonEx btnInvoice;
        private SimpleButtonEx btnReturnTransaction;
        private SimpleButtonEx btnClear;
        private SimpleButtonEx btnSearch;
        private SimpleButtonEx btnClose;
        private Receipt receipt1;
        private LabelControl lblCustomerName;
        private LabelControl lblCustomer;
        private PanelControl basePanel;
        private PanelControl panelControl1;
        private PanelControl panelControl2;
        private PanelControl panelControl3;
        private TableLayoutPanel panelCustomerInfo;
        private LabelControl lblNameHeader;
        private TableLayoutPanel tableLayoutPanel3;
        private TableLayoutPanel tableLayoutPanel2;
        private SimpleButtonEx btnPgDown;
        private DevExpress.XtraGrid.Columns.GridColumn colTransactionDate;
        private DevExpress.XtraGrid.Columns.GridColumn colStaff;
        private DevExpress.XtraGrid.Columns.GridColumn colTerminal;
        private DevExpress.XtraGrid.Columns.GridColumn colReceiptID;
        private DevExpress.XtraGrid.Columns.GridColumn colType;
        private DevExpress.XtraGrid.Columns.GridColumn colNetAmount;
        private DevExpress.XtraGrid.GridControl grJournal;
        private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
        private SimpleButtonEx btnDown;
        private SimpleButtonEx btnPgUp;
        private SimpleButtonEx btnUp;
        private Label labelHeading;
        private TableLayoutPanel tableLayoutPanel4;
        private LabelControl lblCustomerAddress;
        private LabelControl lblAddressHeader;
        private SimpleButtonEx btnGiftReceipt;
        private Timer timer1;
        private StyleController styleController;
        private System.ComponentModel.IContainer components;

        #region Properties

        public JournalDialogResults JournalDialogResults
        {
            get { return journalDialogResults; }
        }

        public Object JournalDialogResultObject
        {
            get { return journalDialogResultObject; }
        }
        #endregion


        /// <summary>
        /// Sets the form.
        /// </summary>
        public frmJournal(IPosTransaction posTransaction, IApplication application)
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();

            this.posTransaction = posTransaction;
            this.Application = application;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                //
                // Get all text through the Translation function in the ApplicationLocalizer
                //
                // TextID's for frmJournal are reserved at 1650 - 1669 and 2400 - 2449
                // In use now are ID's 1669 and 2403
                //
                TranslateLabels();

                journalData = new JournalData(ApplicationSettings.Database.LocalConnection,
                    ApplicationSettings.Database.DATAAREAID);

                transactions = this.GetData();

                if (transactions.Rows.Count < 1)
                {
                    //No records
                    Dialog.InternalApplication.Services.Dialog.ShowMessage(1656);
                    this.Close();
                }
                else
                {
                    //Set the size of the form the same as the main form
                    this.Bounds = new Rectangle(
                        ApplicationSettings.MainWindowLeft,
                        ApplicationSettings.MainWindowTop,
                        ApplicationSettings.MainWindowWidth,
                        ApplicationSettings.MainWindowHeight);

                    this.receipt1.InitCustomFields();

                    grJournal.DataSource = transactions;
                    gridView1.OptionsCustomization.AllowSort = false;

                    gridView1_FocusedRowChanged(this, new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs(0, 0));
                }

                this.receipt1.ShowTwoPayments();

                this.timer1.InvokeOnTick(
                    ApplicationSettings.Terminal.AutoLogOffTimeOutInMin * 30, /*seconds per half-minute (half-time for popups)*/
                    this.Close);
            }

            base.OnLoad(e);
        }

        private void TranslateLabels()
        {
            colNetAmount.Caption = ApplicationLocalizer.Language.Translate(1650); //Amount
            colStaff.Caption = ApplicationLocalizer.Language.Translate(1651); //Staff
            colTerminal.Caption = ApplicationLocalizer.Language.Translate(51086);//Terminal
            colTransactionDate.Caption = ApplicationLocalizer.Language.Translate(1652); //Date
            colReceiptID.Caption = ApplicationLocalizer.Language.Translate(1653); //Transaction
            colType.Caption = ApplicationLocalizer.Language.Translate(1666); //Type

            btnClose.Text = ApplicationLocalizer.Language.Translate(1654); //Close
            btnInvoice.Text = ApplicationLocalizer.Language.Translate(1659); //Invoice/Reikningur
            btnReceipt.Text = ApplicationLocalizer.Language.Translate(1660); //Receipt/Kvittun
            btnSearch.Text = ApplicationLocalizer.Language.Translate(2402); //Search
            btnClear.Text = ApplicationLocalizer.Language.Translate(2403); //Clear search
            btnReturnTransaction.Text = ApplicationLocalizer.Language.Translate(1663); //Return transaction
            lblCustomer.Text = ApplicationLocalizer.Language.Translate(1667); //Customer
            lblNameHeader.Text = ApplicationLocalizer.Language.Translate(1668); //Name:
            lblAddressHeader.Text = ApplicationLocalizer.Language.Translate(1669); //Address:
            this.labelHeading.Text = ApplicationLocalizer.Language.Translate(1670); //Show Journal
            this.Text = ApplicationLocalizer.Language.Translate(1670); //Show journal
            btnGiftReceipt.Text = ApplicationLocalizer.Language.Translate(99872); //Gift receipt:
            this.salesText = ApplicationLocalizer.Language.Translate(1664);
        }

        void gridView1_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
        {
            if (e.Column == colNetAmount)
            {   // NetAmount column.  Override currency displayed

                // Get teh rows currency code
                DataRow Row = gridView1.GetDataRow(e.ListSourceRowIndex);
                string currencyCode = Row["CURRENCY"] as string;

                if (!string.IsNullOrEmpty(currencyCode))
                {   // Use the configured currency for displaying this value
                    e.DisplayText = Dialog.InternalApplication.Services.Rounding.Round((decimal)e.Value, currencyCode, true);
                }
            }
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }
            }

            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.styleController = new DevExpress.XtraEditors.StyleController(this.components);
            this.grJournal = new DevExpress.XtraGrid.GridControl();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colTransactionDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStaff = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTerminal = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colReceiptID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colNetAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.btnGiftReceipt = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnReturnTransaction = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnInvoice = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnReceipt = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.receipt1 = new LSRetailPosis.POSProcesses.WinControls.Receipt();
            this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
            this.panelCustomerInfo = new System.Windows.Forms.TableLayoutPanel();
            this.lblCustomerAddress = new DevExpress.XtraEditors.LabelControl();
            this.lblCustomerName = new DevExpress.XtraEditors.LabelControl();
            this.lblNameHeader = new DevExpress.XtraEditors.LabelControl();
            this.lblAddressHeader = new DevExpress.XtraEditors.LabelControl();
            this.lblCustomer = new DevExpress.XtraEditors.LabelControl();
            this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.labelHeading = new System.Windows.Forms.Label();
            this.basePanel = new DevExpress.XtraEditors.PanelControl();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grJournal)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).BeginInit();
            this.panelCustomerInfo.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).BeginInit();
            this.SuspendLayout();
            // 
            // grJournal
            // 
            this.grJournal.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.tableLayoutPanel2.SetColumnSpan(this.grJournal, 5);
            this.grJournal.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grJournal.Location = new System.Drawing.Point(3, 3);
            this.grJournal.MainView = this.gridView1;
            this.grJournal.Margin = new System.Windows.Forms.Padding(0);
            this.grJournal.Name = "grJournal";
            this.grJournal.Size = new System.Drawing.Size(619, 404);
            this.grJournal.TabIndex = 0;
            this.grJournal.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.gridView1});
            // 
            // gridView1
            // 
            this.gridView1.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.gridView1.Appearance.HeaderPanel.Options.UseFont = true;
            this.gridView1.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gridView1.Appearance.Row.Options.UseFont = true;
            this.gridView1.ColumnPanelRowHeight = 40;
            this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colTransactionDate,
            this.colStaff,
            this.colTerminal,
            this.colReceiptID,
            this.colType,
            this.colNetAmount});
            this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.gridView1.GridControl = this.grJournal;
            this.gridView1.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridView1.Name = "gridView1";
            this.gridView1.OptionsBehavior.Editable = false;
            this.gridView1.OptionsCustomization.AllowColumnMoving = false;
            this.gridView1.OptionsCustomization.AllowFilter = false;
            this.gridView1.OptionsCustomization.AllowQuickHideColumns = false;
            this.gridView1.OptionsMenu.EnableColumnMenu = false;
            this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.gridView1.OptionsView.ShowGroupPanel = false;
            this.gridView1.OptionsView.ShowIndicator = false;
            this.gridView1.RowHeight = 40;
            this.gridView1.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.gridView1.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.gridView1.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView1_FocusedRowChanged);
            this.gridView1.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridView1_CustomColumnDisplayText);
            // 
            // colTransactionDate
            // 
            this.colTransactionDate.Caption = "Date";
            this.colTransactionDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colTransactionDate.FieldName = "CREATEDDATE";
            this.colTransactionDate.Name = "colTransactionDate";
            this.colTransactionDate.OptionsColumn.AllowEdit = false;
            this.colTransactionDate.UnboundType = DevExpress.Data.UnboundColumnType.String;
            this.colTransactionDate.Visible = true;
            this.colTransactionDate.VisibleIndex = 0;
            this.colTransactionDate.Width = 165;
            // 
            // colStaff
            // 
            this.colStaff.Caption = "Staff";
            this.colStaff.FieldName = "STAFF";
            this.colStaff.Name = "colStaff";
            this.colStaff.Visible = true;
            this.colStaff.VisibleIndex = 1;
            this.colStaff.Width = 88;
            // 
            // colTerminal
            // 
            this.colTerminal.Caption = "Terminal";
            this.colTerminal.FieldName = "TERMINAL";
            this.colTerminal.Name = "colTerminal";
            this.colTerminal.Visible = true;
            this.colTerminal.VisibleIndex = 2;
            this.colTerminal.Width = 88;
            // 
            // colReceiptID
            // 
            this.colReceiptID.Caption = "Transaction";
            this.colReceiptID.FieldName = "RECEIPTID";
            this.colReceiptID.Name = "colReceiptID";
            this.colReceiptID.Visible = true;
            this.colReceiptID.VisibleIndex = 3;
            this.colReceiptID.Width = 164;
            // 
            // colType
            // 
            this.colType.Caption = "Type";
            this.colType.FieldName = "TYPE";
            this.colType.Name = "colType";
            this.colType.UnboundType = DevExpress.Data.UnboundColumnType.String;
            this.colType.Visible = true;
            this.colType.VisibleIndex = 4;
            // 
            // colNetAmount
            // 
            this.colNetAmount.AppearanceHeader.Options.UseTextOptions = true;
            this.colNetAmount.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
            this.colNetAmount.Caption = "Amount";
            this.colNetAmount.DisplayFormat.FormatString = "c2";
            this.colNetAmount.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
            this.colNetAmount.FieldName = "GROSSAMOUNT";
            this.colNetAmount.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Right;
            this.colNetAmount.Name = "colNetAmount";
            this.colNetAmount.Visible = true;
            this.colNetAmount.VisibleIndex = 5;
            this.colNetAmount.Width = 80;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.tableLayoutPanel3, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.panelControl1, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.labelHeading, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(30, 40, 30, 15);
            this.tableLayoutPanel1.RowCount = 13;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(933, 707);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 8;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel3.Controls.Add(this.btnGiftReceipt, 0, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnClose, 5, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnReturnTransaction, 4, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnInvoice, 3, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnReceipt, 2, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnSearch, 1, 0);
            this.tableLayoutPanel3.Controls.Add(this.btnClear, 6, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(30, 626);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0, 15, 0, 0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.RowCount = 1;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Size = new System.Drawing.Size(873, 66);
            this.tableLayoutPanel3.TabIndex = 12;
            // 
            // btnGiftReceipt
            // 
            this.btnGiftReceipt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGiftReceipt.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnGiftReceipt.Appearance.Options.UseFont = true;
            this.btnGiftReceipt.Enabled = false;
            this.btnGiftReceipt.Location = new System.Drawing.Point(0, 4);
            this.btnGiftReceipt.Margin = new System.Windows.Forms.Padding(0);
            this.btnGiftReceipt.Name = "btnGiftReceipt";
            this.btnGiftReceipt.ShowToolTips = false;
            this.btnGiftReceipt.Size = new System.Drawing.Size(69, 57);
            this.btnGiftReceipt.TabIndex = 5;
            this.btnGiftReceipt.Tag = "";
            this.btnGiftReceipt.Text = "Gift receipt";
            this.btnGiftReceipt.Click += new System.EventHandler(this.btnGiftReceipt_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(612, 4);
            this.btnClose.Margin = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.btnClose.Name = "btnClose";
            this.btnClose.Padding = new System.Windows.Forms.Padding(0);
            this.btnClose.ShowToolTips = false;
            this.btnClose.Size = new System.Drawing.Size(127, 57);
            this.btnClose.TabIndex = 10;
            this.btnClose.Tag = "";
            this.btnClose.Text = "Close";
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // btnReturnTransaction
            // 
            this.btnReturnTransaction.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnReturnTransaction.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnReturnTransaction.Appearance.Options.UseFont = true;
            this.btnReturnTransaction.Location = new System.Drawing.Point(478, 4);
            this.btnReturnTransaction.Margin = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.btnReturnTransaction.Name = "btnReturnTransaction";
            this.btnReturnTransaction.Padding = new System.Windows.Forms.Padding(0);
            this.btnReturnTransaction.Size = new System.Drawing.Size(127, 57);
            this.btnReturnTransaction.TabIndex = 9;
            this.btnReturnTransaction.Tag = "";
            this.btnReturnTransaction.Text = "Return";
            this.btnReturnTransaction.Click += new System.EventHandler(this.btnReturnTransaction_Click);
            // 
            // btnInvoice
            // 
            this.btnInvoice.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnInvoice.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnInvoice.Appearance.Options.UseFont = true;
            this.btnInvoice.Location = new System.Drawing.Point(344, 4);
            this.btnInvoice.Margin = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.btnInvoice.Name = "btnInvoice";
            this.btnInvoice.Padding = new System.Windows.Forms.Padding(0);
            this.btnInvoice.Size = new System.Drawing.Size(127, 57);
            this.btnInvoice.TabIndex = 8;
            this.btnInvoice.Tag = "";
            this.btnInvoice.Text = "Invoice";
            this.btnInvoice.Click += new System.EventHandler(this.btnInvoice_Click);
            // 
            // btnReceipt
            // 
            this.btnReceipt.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnReceipt.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnReceipt.Appearance.Options.UseFont = true;
            this.btnReceipt.Location = new System.Drawing.Point(210, 4);
            this.btnReceipt.Margin = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.btnReceipt.Name = "btnReceipt";
            this.btnReceipt.Padding = new System.Windows.Forms.Padding(0);
            this.btnReceipt.Size = new System.Drawing.Size(127, 57);
            this.btnReceipt.TabIndex = 7;
            this.btnReceipt.Tag = "";
            this.btnReceipt.Text = "Receipt";
            this.btnReceipt.Click += new System.EventHandler(this.btnReceipt_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSearch.Appearance.Options.UseFont = true;
            this.btnSearch.Location = new System.Drawing.Point(76, 4);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Padding = new System.Windows.Forms.Padding(0);
            this.btnSearch.ShowToolTips = false;
            this.btnSearch.Size = new System.Drawing.Size(127, 57);
            this.btnSearch.TabIndex = 6;
            this.btnSearch.Tag = "";
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Location = new System.Drawing.Point(746, 4);
            this.btnClear.Margin = new System.Windows.Forms.Padding(7, 0, 0, 0);
            this.btnClear.Name = "btnClear";
            this.btnClear.Padding = new System.Windows.Forms.Padding(0);
            this.btnClear.ShowToolTips = false;
            this.btnClear.Size = new System.Drawing.Size(127, 57);
            this.btnClear.TabIndex = 4;
            this.btnClear.Tag = "";
            this.btnClear.Text = "Clear";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // panelControl1
            // 
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Controls.Add(this.tableLayoutPanel4);
            this.panelControl1.Controls.Add(this.panelControl2);
            this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl1.Location = new System.Drawing.Point(30, 135);
            this.panelControl1.Margin = new System.Windows.Forms.Padding(0);
            this.panelControl1.Name = "panelControl1";
            this.tableLayoutPanel1.SetRowSpan(this.panelControl1, 11);
            this.panelControl1.Size = new System.Drawing.Size(873, 476);
            this.panelControl1.TabIndex = 11;
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.ColumnCount = 1;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.Controls.Add(this.receipt1, 0, 1);
            this.tableLayoutPanel4.Controls.Add(this.panelControl3, 0, 0);
            this.tableLayoutPanel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel4.Location = new System.Drawing.Point(629, 0);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 2;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 30F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 70F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(244, 476);
            this.tableLayoutPanel4.TabIndex = 3;
            // 
            // receipt1
            // 
            this.receipt1.Appearance.Options.UseBackColor = true;
            this.receipt1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.receipt1.Location = new System.Drawing.Point(0, 142);
            this.receipt1.LookAndFeel.SkinName = "Money Twins";
            this.receipt1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.receipt1.Margin = new System.Windows.Forms.Padding(0);
            this.receipt1.Name = "receipt1";
            this.receipt1.ReturnItems = false;
            this.receipt1.Size = new System.Drawing.Size(244, 334);
            this.receipt1.TabIndex = 1;
            // 
            // panelControl3
            // 
            this.panelControl3.Controls.Add(this.panelCustomerInfo);
            this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelControl3.Location = new System.Drawing.Point(3, 0);
            this.panelControl3.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.panelControl3.Name = "panelControl3";
            this.panelControl3.Size = new System.Drawing.Size(238, 139);
            this.panelControl3.TabIndex = 2;
            // 
            // panelCustomerInfo
            // 
            this.panelCustomerInfo.ColumnCount = 2;
            this.panelCustomerInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.panelCustomerInfo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.panelCustomerInfo.Controls.Add(this.lblCustomerAddress, 1, 2);
            this.panelCustomerInfo.Controls.Add(this.lblCustomerName, 1, 1);
            this.panelCustomerInfo.Controls.Add(this.lblNameHeader, 0, 1);
            this.panelCustomerInfo.Controls.Add(this.lblAddressHeader, 0, 2);
            this.panelCustomerInfo.Controls.Add(this.lblCustomer, 0, 0);
            this.panelCustomerInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelCustomerInfo.Location = new System.Drawing.Point(2, 2);
            this.panelCustomerInfo.MinimumSize = new System.Drawing.Size(254, 100);
            this.panelCustomerInfo.Name = "panelCustomerInfo";
            this.panelCustomerInfo.RowCount = 3;
            this.panelCustomerInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelCustomerInfo.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.panelCustomerInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.panelCustomerInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.panelCustomerInfo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.panelCustomerInfo.Size = new System.Drawing.Size(254, 135);
            this.panelCustomerInfo.TabIndex = 0;
            // 
            // lblCustomerAddress
            // 
            this.lblCustomerAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCustomerAddress.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerAddress.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.lblCustomerAddress.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Top;
            this.lblCustomerAddress.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
            this.lblCustomerAddress.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.Vertical;
            this.lblCustomerAddress.Location = new System.Drawing.Point(60, 49);
            this.lblCustomerAddress.Name = "lblCustomerAddress";
            this.lblCustomerAddress.Size = new System.Drawing.Size(191, 17);
            this.lblCustomerAddress.TabIndex = 4;
            this.lblCustomerAddress.Text = "lblCustomerAddress";
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblCustomerName.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCustomerName.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
            this.lblCustomerName.Location = new System.Drawing.Point(60, 26);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(191, 17);
            this.lblCustomerName.TabIndex = 2;
            this.lblCustomerName.Text = "lblCustomerName";
            // 
            // lblNameHeader
            // 
            this.lblNameHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblNameHeader.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblNameHeader.Location = new System.Drawing.Point(3, 26);
            this.lblNameHeader.Name = "lblNameHeader";
            this.lblNameHeader.Size = new System.Drawing.Size(38, 17);
            this.lblNameHeader.TabIndex = 1;
            this.lblNameHeader.Text = "Name:";
            // 
            // lblAddressHeader
            // 
            this.lblAddressHeader.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblAddressHeader.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAddressHeader.Location = new System.Drawing.Point(3, 49);
            this.lblAddressHeader.Name = "lblAddressHeader";
            this.lblAddressHeader.Size = new System.Drawing.Size(51, 17);
            this.lblAddressHeader.TabIndex = 3;
            this.lblAddressHeader.Text = "Address:";
            // 
            // lblCustomer
            // 
            this.lblCustomer.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCustomer.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.panelCustomerInfo.SetColumnSpan(this.lblCustomer, 2);
            this.lblCustomer.Location = new System.Drawing.Point(3, 3);
            this.lblCustomer.Name = "lblCustomer";
            this.lblCustomer.Size = new System.Drawing.Size(59, 17);
            this.lblCustomer.TabIndex = 0;
            this.lblCustomer.Text = "Customer";
            // 
            // panelControl2
            // 
            this.panelControl2.Controls.Add(this.tableLayoutPanel2);
            this.panelControl2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panelControl2.Location = new System.Drawing.Point(0, 0);
            this.panelControl2.Name = "panelControl2";
            this.panelControl2.Size = new System.Drawing.Size(629, 476);
            this.panelControl2.TabIndex = 1;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.AutoSize = true;
            this.tableLayoutPanel2.ColumnCount = 5;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.btnPgDown, 4, 1);
            this.tableLayoutPanel2.Controls.Add(this.grJournal, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnDown, 3, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnPgUp, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnUp, 1, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(3, 0, 0, 0);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.Padding = new System.Windows.Forms.Padding(3);
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(625, 472);
            this.tableLayoutPanel2.TabIndex = 3;
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.bottom;
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(565, 412);
            this.btnPgDown.Margin = new System.Windows.Forms.Padding(7, 5, 0, 0);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Padding = new System.Windows.Forms.Padding(0);
            this.btnPgDown.ShowToolTips = false;
            this.btnPgDown.Size = new System.Drawing.Size(57, 57);
            this.btnPgDown.TabIndex = 3;
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.btnPgDown_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.down;
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(501, 412);
            this.btnDown.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.btnDown.Name = "btnDown";
            this.btnDown.Padding = new System.Windows.Forms.Padding(0);
            this.btnDown.ShowToolTips = false;
            this.btnDown.Size = new System.Drawing.Size(57, 57);
            this.btnDown.TabIndex = 2;
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnPgUp
            // 
            this.btnPgUp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;
            this.btnPgUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.top;
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(3, 412);
            this.btnPgUp.Margin = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Padding = new System.Windows.Forms.Padding(0);
            this.btnPgUp.ShowToolTips = false;
            this.btnPgUp.Size = new System.Drawing.Size(57, 57);
            this.btnPgUp.TabIndex = 0;
            this.btnPgUp.Text = "Ç";
            this.btnPgUp.Click += new System.EventHandler(this.btnPgUp_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = global::Microsoft.Dynamics.Retail.Pos.Dialog.Properties.Resources.up;
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(67, 412);
            this.btnUp.Margin = new System.Windows.Forms.Padding(7, 5, 0, 0);
            this.btnUp.Name = "btnUp";
            this.btnUp.Padding = new System.Windows.Forms.Padding(0);
            this.btnUp.ShowToolTips = false;
            this.btnUp.Size = new System.Drawing.Size(57, 57);
            this.btnUp.TabIndex = 1;
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // labelHeading
            // 
            this.labelHeading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.labelHeading.AutoSize = true;
            this.labelHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.labelHeading.Location = new System.Drawing.Point(319, 40);
            this.labelHeading.Margin = new System.Windows.Forms.Padding(0);
            this.labelHeading.Name = "labelHeading";
            this.labelHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.labelHeading.Size = new System.Drawing.Size(295, 95);
            this.labelHeading.TabIndex = 13;
            this.labelHeading.Tag = "";
            this.labelHeading.Text = "Show Journal";
            this.labelHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // basePanel
            // 
            this.basePanel.Controls.Add(this.tableLayoutPanel1);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(937, 711);
            this.basePanel.TabIndex = 0;
            this.basePanel.TabStop = true;
            // 
            // frmJournal
            // 
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(937, 711);
            this.Controls.Add(this.basePanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmJournal";
            this.Text = "Show journal";
            this.Controls.SetChildIndex(this.basePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grJournal)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.tableLayoutPanel3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.tableLayoutPanel4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.panelControl3)).EndInit();
            this.panelCustomerInfo.ResumeLayout(false);
            this.panelCustomerInfo.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        #region Form Events
        private void gridView1_FocusedRowChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs e)
        {
            timer1.Restart(); //reset the timer.
            
            if (gridView1.SelectedRowsCount > 0)
            {
                DataRow Row = gridView1.GetDataRow(gridView1.GetSelectedRows()[0]);
                selectedTransactionId = (string)Row["TRANSACTIONID"];
                selectedStoreId = (string)Row["STORE"];
                selectedTerminalId = (string)Row["TERMINAL"];

                PosTransaction transaction = LoadTransaction(selectedTransactionId, selectedStoreId, selectedTerminalId);
                EnableButtons(transaction);
                SetGiftReceiptOption(transaction);
                DisplayCustomerInfo(transaction);
                receipt1.ShowTransaction(transaction);
            }
            else
            {
                // Clear view
                DisplayCustomerInfo(null);
                receipt1.ShowTransaction(null);
            }
            this.receipt1.ShowTwoPayments();
        }

        /// <summary>
        /// Enable gift receipt button if applicable
        /// </summary>
        /// <param name="transaction">transaction</param>
        private void SetGiftReceiptOption(PosTransaction transaction)
        {
            try
            {
                if (transaction != null)
                {
                    ITransactionSystem transSys = this.Application.BusinessLogic.TransactionSystem;
                    this.btnGiftReceipt.Enabled = transSys.IsGiftReceiptOptionApplicable(transaction);
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        private void EnableButtons(PosTransaction transaction)
        {
            bool enabled = true;
            switch (transaction.TransactionType)
            {
                case TypeOfTransaction.CustomerOrder:
                    enabled = false;
                    break;
            }

            this.btnInvoice.Enabled = enabled;
            this.btnReturnTransaction.Enabled = enabled;

            if (LSRetailPosis.Settings.FunctionalityProfiles.Functions.CountryRegion == LSRetailPosis.Settings.FunctionalityProfiles.SupportedCountryRegion.RU)
            {
                // We want to enable this button under Russian country context only for the last transaction which is shown in the first row of the data grid.
                this.btnReceipt.Enabled = gridView1.IsFirstRow;
            }
        }

        #endregion


        #region Buttons

        private void btnDown_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            gridView1.MoveNext();
            GetMoreTransactions();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            gridView1.MovePrev();
        }

        private void btnPgUp_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            gridView1.MovePrevPage();
        }

        private void btnPgDown_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            gridView1.MoveNextPage();
            GetMoreTransactions();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            
            try
            {
                // Display the search dialog....
                using (frmJournalSearch searchJournal = new frmJournalSearch())
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(searchJournal);

                    if (searchJournal.DialogResult != DialogResult.OK)
                    {
                        return;
                    }

                    // In order to page in search result we need to store search values.
                    this.searchCriteria = searchJournal.SearchCriteria;
                    if (this.searchCriteria == null || this.searchCriteria.IsEmpty())
                    {
                        transactions = this.GetData();
                    }
                    else
                    {
                        transactions = this.GetSearchResults();
                    }
                    
                    grJournal.DataSource = transactions;
                    grJournal.RefreshDataSource();

                    if (transactions.Rows.Count <= 0)
                    {
                        //No records
                        using (LSRetailPosis.POSProcesses.frmMessage message = new LSRetailPosis.POSProcesses.frmMessage(ApplicationLocalizer.Language.Translate(1656), MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            POSFormsManager.ShowPOSForm(message);
                        }
                    }

                    gridView1_FocusedRowChanged(sender, new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs(0, 0));
                }
            }
            catch (Exception ex)
            {
                ApplicationExceptionHandler.HandleException(this.ToString(), ex);
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            
            try
            {
                lblCustomerName.Text = string.Empty;
                lblCustomerAddress.Text = string.Empty;
                this.searchReceiptId = null;
                this.searchDate = null;
                transactions = this.GetData();
                grJournal.DataSource = transactions;
                grJournal.RefreshDataSource();

                if (transactions.Rows.Count <= 0)
                {
                    //No records
                    using (LSRetailPosis.POSProcesses.frmMessage message = new LSRetailPosis.POSProcesses.frmMessage(ApplicationLocalizer.Language.Translate(1656), MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        POSFormsManager.ShowPOSForm(message);
                    }
                }

                gridView1_FocusedRowChanged(sender, new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventArgs(0, 0));
            }
            catch (Exception ex)
            {
                ApplicationExceptionHandler.HandleException(this.ToString(), ex);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.journalDialogResults = JournalDialogResults.Close;
            this.journalDialogResultObject = null;
            Close();
        }

        private void btnReceipt_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            
            try
            {
                PrintReceipt(LoadTransaction(selectedTransactionId, selectedStoreId, selectedTerminalId));
                ((PosTransaction)posTransaction).EntryStatus = TransactionStatus.Cancelled;

                this.journalDialogResults = JournalDialogResults.PrintReceipt;
                this.journalDialogResultObject = Tuple.Create(selectedTransactionId, selectedStoreId, selectedTerminalId);
            }
            catch (PosisException pex)
            {
                POSFormsManager.ShowPOSErrorDialog(pex);
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                POSFormsManager.ShowPOSErrorDialog(new PosisException(1650, ex));
            }
        }

        private void btnInvoice_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            
            try
            {
                PrintSlip(LoadTransaction(selectedTransactionId, selectedStoreId, selectedTerminalId));
                ((PosTransaction)posTransaction).EntryStatus = TransactionStatus.Cancelled;

                this.journalDialogResults = JournalDialogResults.PrintInvoice;
                this.journalDialogResultObject = Tuple.Create(selectedTransactionId, selectedStoreId, selectedTerminalId); ;
            }
            catch (PosisException pex)
            {
                POSFormsManager.ShowPOSErrorDialog(pex);
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                POSFormsManager.ShowPOSErrorDialog(new PosisException(1650, ex));
            }
        }

        private void btnReturnTransaction_Click(object sender, EventArgs e)
        {

            timer1.Restart(); //reset the timer.
            
            // Get the receipt id
            System.Data.DataRow Row = gridView1.GetDataRow(gridView1.GetSelectedRows()[0]);
            string receiptId = (string)Row["RECEIPTID"];

            this.journalDialogResults = JournalDialogResults.ReturnTransaction;
            this.journalDialogResultObject = receiptId;

            Close();
        }

        private void btnGiftReceipt_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            
            try
            {
                PrintGiftReceipt(LoadTransaction(selectedTransactionId, selectedStoreId, selectedTerminalId));
            }
            catch (PosisException pex)
            {
                POSFormsManager.ShowPOSErrorDialog(pex);
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                POSFormsManager.ShowPOSErrorDialog(new PosisException(1650, ex));
            }
        }

        private DataTable GetData(Int64 lastRowNumber = 0)
        {
            return journalData.GetJournalData(lastRowNumber,
                                              this.searchReceiptId,
                                              this.searchDate,
                                              maxRowsAtEachQuery,
                                              this.salesText,    // Sales
                                              ApplicationLocalizer.Language.Translate(1665),    // Payment
                                              ApplicationLocalizer.Language.Translate(2400),    // Sales order
                                              ApplicationLocalizer.Language.Translate(2401),    // Sales invoice
                                              ApplicationLocalizer.Language.Translate(2404),    // Customer orders
                                              ApplicationLocalizer.Language.Translate(4546));   // Income expense
        }

        private DataTable GetSearchResults(Int64 lastRowNumber = 0)
        {
            var localOrders = this.journalData.SearchOrders(this.searchCriteria,
                                    lastRowNumber,
                                    maxRowsAtEachQuery,
                                    this.salesText,    // Sales
                                    ApplicationLocalizer.Language.Translate(1665),    // Payment
                                    ApplicationLocalizer.Language.Translate(2400),    // Sales order
                                    ApplicationLocalizer.Language.Translate(2401),    // Sales invoice
                                    ApplicationLocalizer.Language.Translate(2404),    // Customer orders
                                    ApplicationLocalizer.Language.Translate(4546));   // Income expense

            return localOrders;
        }

        #endregion

        #region Private functions

        private void GetMoreTransactions()
        {
            int topRowIndex = this.gridView1.TopRowIndex;
            if ((gridView1.IsLastRow) && (gridView1.RowCount > 0))
            {
                DataRow Row = gridView1.GetDataRow(gridView1.GetSelectedRows()[0]);
                Int64 rowNumber = (Int64)Row["ROWNUMBER"];
                DataTable nextPageTransactions;
                if (this.searchCriteria == null || this.searchCriteria.IsEmpty())
                {
                    nextPageTransactions = this.GetData(rowNumber);
                }
                else
                {
                    nextPageTransactions = this.GetSearchResults(rowNumber);
                }
                transactions.Merge(nextPageTransactions);
                gridView1.TopRowIndex = topRowIndex;
            }
        }

        private PosTransaction LoadTransaction(string transactionId, string storeId, string terminalId)
        {
            try
            {
                TransactionData transData = new TransactionData(Application.Settings.Database.Connection,
                        Application.Settings.Database.DataAreaID, Application);

                return transData.LoadSerializedTransaction(transactionId, storeId, terminalId);
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        private void PrintReceipt(IPosTransaction transaction)
        {
            try
            {
                if (transaction != null)
                {
                    ITransactionSystem transSys = this.Application.BusinessLogic.TransactionSystem;
                    transSys.PrintTransaction(transaction, true, true);
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        private void PrintGiftReceipt(PosTransaction retailTransaction)
        {
            try
            {
                if (retailTransaction != null)
                {
                    ITransactionSystem transSys = this.Application.BusinessLogic.TransactionSystem;
                    transSys.PrintGiftReceipt(retailTransaction, true, true);
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        private void PrintSlip(IPosTransaction transaction)
        {
            try
            {
                if (transaction != null)
                {
                    ITransactionSystem transSys = this.Application.BusinessLogic.TransactionSystem;
                    transSys.PrintInvoice(transaction, false, false);
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "GrandFather PS6015")]
        private void DisplayCustomerInfo(IPosTransaction transaction)
        {
            lblCustomerName.Text = null;
            lblCustomerAddress.Text = null;
            DE.ICustomer transactionCustomer = null;

            if (transaction is IRetailTransaction)
            {
                RetailTransaction retailTransaction = (RetailTransaction)transaction;
                transactionCustomer = retailTransaction.Customer;
            }
            else if (transaction is CustomerPaymentTransaction)
            {
                CustomerPaymentTransaction customerPaymentTransaction = (CustomerPaymentTransaction)transaction;
                transactionCustomer = customerPaymentTransaction.Customer;
            }

            if (transactionCustomer != null)
            {
                lblCustomerName.Text = transactionCustomer.Name;
                lblCustomerAddress.Text = transactionCustomer.Address;
            }
            else
            {
                lblCustomerName.Text = lblCustomerAddress.Text = null;
            }
        }
        #endregion
    }
}
