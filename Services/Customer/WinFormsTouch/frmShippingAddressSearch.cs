/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.POSProcesses.WinControls;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;

namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    /// <summary>
    /// Summary description for frmShippingAddressSearch.
    /// </summary>
	public class frmShippingAddressSearch : frmTouchBase
    {
        private sealed class AddressInfo
        {
            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public Int64 Id { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public string Name { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public string Type { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public string Address { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public string Phone { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public string Url { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public string Email { get; set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode"), SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
            public string OrgId { get; set; }
        }

        private class StatusFormatter : IFormatProvider, ICustomFormatter
        {
            private readonly string Delivery;
            private readonly string None;
            private readonly string Invoice;
            private readonly string Business;
            private readonly string Home;
            private readonly string Other;
            private readonly string Payment;
            private readonly string RemitTo;
            private readonly string Service;
            private readonly string ThirdPartyShipping;
            private readonly string Swift;
            private readonly string Statement;
            private readonly string FixedAsset;
            private readonly string Onetime;
            private readonly string Recruit;

            /// <summary>
            /// Formats status settings.
            /// </summary>
            public StatusFormatter()
            {
                this.None               = ApplicationLocalizer.Language.Translate(51170); // None
                this.Invoice            = ApplicationLocalizer.Language.Translate(51171); // Invoice
                this.Delivery           = ApplicationLocalizer.Language.Translate(51172); // Delivery
                this.Swift              = ApplicationLocalizer.Language.Translate(51174); //Swift
                this.Payment            = ApplicationLocalizer.Language.Translate(51175); //Payment
                this.Service            = ApplicationLocalizer.Language.Translate(51176); //Service
                this.Home               = ApplicationLocalizer.Language.Translate(51177); //Home
                this.Other              = ApplicationLocalizer.Language.Translate(51178); //Other
                this.Business           = ApplicationLocalizer.Language.Translate(51179); //Business
                this.RemitTo            = ApplicationLocalizer.Language.Translate(51180); //Remit-To
                this.ThirdPartyShipping = ApplicationLocalizer.Language.Translate(51181); //Third PartyS hipping
                this.Statement = "Statement";
                this.FixedAsset = "Fixed Asset";
                this.Onetime = "One Time";
                this.Recruit = "Recruit";
            }

            /// <summary>
            /// The GetFormat method of the IFormatProvider interface.
            /// This must return an object that provides formatting services for the specified type.
            /// </summary>
            /// <param name="type"></param>
            /// <returns></returns>
            public object GetFormat(System.Type type)
            {
                return this;
            }

            /// <summary>
            /// The Format method of the ICustomFormatter interface.
            /// This must format the specified value according to the specified format settings.
            /// </summary>
            /// <param name="format"></param>
            /// <param name="arg"></param>
            /// <param name="formatProvider"></param>
            /// <returns></returns>
            public string Format(string format, object arg, IFormatProvider formatProvider)
            {
                if (arg is int)
                {
                    switch ((int)arg)
                    {
                        case (int)AddressType.Delivery:
                            return this.Delivery;
                        case (int)AddressType.Invoice:
                            return this.Invoice;
                        case (int)AddressType.None:
                            return this.None;
                        case (int)AddressType.Business:
                            return this.Business;
                        case (int)AddressType.Home:
                            return this.Home;
                        case (int)AddressType.Other:
                            return this.Other;
                        case (int)AddressType.Payment:
                            return this.Payment;
                        case (int)AddressType.RemitTo:
                            return this.RemitTo;
                        case (int)AddressType.Service:
                            return this.Service;
                        case (int)AddressType.ShipCarrierThirdPartyShipping:
                            return this.ThirdPartyShipping;
                        case (int)AddressType.SWIFT:
                            return this.Swift;
                        case (int)AddressType.Statement:
                            return this.Statement;
                        case (int)AddressType.FixedAsset:
                            return this.FixedAsset;
                        case (int)AddressType.Onetime:
                            return this.Onetime;
                        case (int)AddressType.Recruit:
                            return this.Recruit;
                        default:
                            return string.Empty;
                    }
                }
                return (arg == null) ? string.Empty : arg.ToString();
            }
        }

        private SimpleButtonEx btnEdit;
        private SimpleButtonEx btnNew;
        private PanelControl basePanel;
        private TableLayoutPanel tableLayoutPanel1;
        
        private IList<AddressInfo> addressList;

        private string sortBy = "Name";
        private bool sortAsc = true;
        public bool isEdit;
        private DevExpress.XtraGrid.GridControl grAddress;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Columns.GridColumn colAddressType ;
        private DevExpress.XtraGrid.Columns.GridColumn colName;
        private DevExpress.XtraGrid.Columns.GridColumn colAddress;
        private DevExpress.XtraGrid.Columns.GridColumn colOrgID;
        private TableLayoutPanel tableLayoutPanel2;
        private DevExpress.XtraEditors.TextEdit txtKeyboardInput;
        private SimpleButton btnSearch;
        private SimpleButton btnClear;
        private SimpleButtonEx btnSelect;
        private SimpleButtonEx btnUp;
        private SimpleButtonEx btnDown;
        private DM.CustomerDataManager customerDataManager;
        private ICustomer customer;
        private bool inputHasChanged = false;
        private SimpleButtonEx btnCancelSearch;
        private Label lblHeading;
        private SimpleButtonEx btnPgDown;
        private SimpleButtonEx btnPgUp;
        private TableLayoutPanel tableLayoutPanel7;
        private TableLayoutPanel tableLayoutPanel8;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.Container components = null;

        protected frmShippingAddressSearch()
        {
            //
            // Required for Windows Form Designer support
            //
            InitializeComponent();
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public frmShippingAddressSearch(ICustomer searchCustomer)
            : this()
        {
            this.customer = searchCustomer;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                customerDataManager = new DM.CustomerDataManager(
                    ApplicationSettings.Database.LocalConnection, 
                    ApplicationSettings.Database.DATAAREAID);
                //Page title
                this.Text = ApplicationLocalizer.Language.Translate(900); //Add shipping address

                colAddressType.DisplayFormat.Format = new StatusFormatter();

                GetAddressList();
                 
                // Show Org ID column if in Iceland
                string culture = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
                colOrgID.Visible = (culture == "is" || culture == "is-IS");

                TranslateLabels();
                SetFormFocus();
            }

            base.OnLoad(e);
        }

        private void GetAddressList()
        {
            IList<Address> addresses = customerDataManager.GetAddresses(this.customer.PartyId);
            string filter = txtKeyboardInput.Text.ToUpperInvariant();
            StatusFormatter formatter = new StatusFormatter();
            var addressFormatter = new AddressFormatter(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);

            this.addressList = (from p in addresses
                                where p.Name.ToUpperInvariant().Contains(filter) || p.LineAddress.ToUpperInvariant().Contains(filter)
                                select new AddressInfo
                                {
                                    Id = p.Id,
                                    Name = p.Name,
                                    Type = formatter.Format(null, p.AddressType, null),
                                    Address = addressFormatter.Format(address: p, multiline: false),
                                    Phone = p.Telephone,
                                    Url = p.URL,
                                    Email = p.Email
                                }).ToList<AddressInfo>();

            this.grAddress.DataSource = this.addressList;

            this.grdView.FocusedColumn = this.grdView.Columns["Id"];
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
            this.grAddress = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colAddressType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colAddress = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOrgID = new DevExpress.XtraGrid.Columns.GridColumn();
            this.btnEdit = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnNew = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.basePanel = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.lblHeading = new System.Windows.Forms.Label();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.txtKeyboardInput = new DevExpress.XtraEditors.TextEdit();
            this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
            this.btnClear = new DevExpress.XtraEditors.SimpleButton();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancelSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSelect = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grAddress)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).BeginInit();
            this.basePanel.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).BeginInit();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grAddress
            // 
            this.grAddress.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grAddress.Location = new System.Drawing.Point(4, 4);
            this.grAddress.MainView = this.grdView;
            this.grAddress.Margin = new System.Windows.Forms.Padding(4);
            this.grAddress.Name = "grAddress";
            this.tableLayoutPanel1.SetRowSpan(this.grAddress, 7);
            this.grAddress.Size = new System.Drawing.Size(853, 440);
            this.grAddress.TabIndex = 0;
            this.grAddress.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView});
            this.grAddress.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grItems_KeyDown);
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.ColumnPanelRowHeight = 40;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colAddressType,
            this.colName,
            this.colAddress,
            this.colOrgID});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grAddress;
            this.grdView.HorzScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.Name = "grdView";
            this.grdView.OptionsBehavior.Editable = false;
            this.grdView.OptionsCustomization.AllowFilter = false;
            this.grdView.OptionsSelection.EnableAppearanceFocusedCell = false;
            this.grdView.OptionsView.ShowGroupPanel = false;
            this.grdView.OptionsView.ShowIndicator = false;
            this.grdView.RowHeight = 40;
            this.grdView.ScrollStyle = DevExpress.XtraGrid.Views.Grid.ScrollStyleFlags.None;
            this.grdView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.Default;
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.gridView1_FocusedColumnChanged);
            this.grdView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grItems_KeyDown);
            this.grdView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdView_MouseDown);
            this.grdView.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // colAddressType
            // 
            this.colAddressType.Caption = "Address Type";
            this.colAddressType.FieldName = "Type";
            this.colAddressType.Name = "colAddressType";
            this.colAddressType.OptionsColumn.AllowEdit = false;
            this.colAddressType.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colAddressType.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colAddressType.Visible = true;
            this.colAddressType.VisibleIndex = 1;
            this.colAddressType.Width = 139;
            // 
            // colName
            // 
            this.colName.Caption = "Name";
            this.colName.FieldName = "Name";
            this.colName.Name = "colName";
            this.colName.OptionsColumn.AllowEdit = false;
            this.colName.OptionsColumn.AllowIncrementalSearch = false;
            this.colName.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colName.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colName.Visible = true;
            this.colName.VisibleIndex = 0;
            this.colName.Width = 100;
            // 
            // colAddress
            // 
            this.colAddress.Caption = "Address";
            this.colAddress.FieldName = "Address";
            this.colAddress.Name = "colAddress";
            this.colAddress.OptionsColumn.AllowEdit = false;
            this.colAddress.OptionsColumn.AllowIncrementalSearch = false;
            this.colAddress.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colAddress.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colAddress.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colAddress.Visible = true;
            this.colAddress.VisibleIndex = 2;
            this.colAddress.Width = 278;
            // 
            // colOrgID
            // 
            this.colOrgID.Caption = "Org Id";
            this.colOrgID.FieldName = "OrgId";
            this.colOrgID.Name = "colOrgID";
            this.colOrgID.OptionsColumn.AllowEdit = false;
            this.colOrgID.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colOrgID.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
            this.colOrgID.Width = 127;
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnEdit.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnEdit.Appearance.Options.UseFont = true;
            this.btnEdit.Location = new System.Drawing.Point(299, 4);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(4);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(127, 57);
            this.btnEdit.TabIndex = 8;
            this.btnEdit.Text = "Edit";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnNew
            // 
            this.btnNew.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnNew.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNew.Appearance.Options.UseFont = true;
            this.btnNew.Location = new System.Drawing.Point(164, 4);
            this.btnNew.Margin = new System.Windows.Forms.Padding(4);
            this.btnNew.Name = "btnNew";
            this.btnNew.Size = new System.Drawing.Size(127, 57);
            this.btnNew.TabIndex = 7;
            this.btnNew.Text = "New";
            this.btnNew.Click += new System.EventHandler(this.btnNew_Click);
            // 
            // basePanel
            // 
            this.basePanel.Controls.Add(this.tableLayoutPanel7);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Margin = new System.Windows.Forms.Padding(0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(917, 715);
            this.basePanel.TabIndex = 0;
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.ColumnCount = 1;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Controls.Add(this.lblHeading, 0, 0);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel2, 0, 1);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel8, 0, 3);
            this.tableLayoutPanel7.Controls.Add(this.tableLayoutPanel1, 0, 2);
            this.tableLayoutPanel7.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel7.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel7.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.Padding = new System.Windows.Forms.Padding(26, 40, 26, 11);
            this.tableLayoutPanel7.RowCount = 4;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel7.Size = new System.Drawing.Size(913, 711);
            this.tableLayoutPanel7.TabIndex = 1;
            // 
            // lblHeading
            // 
            this.lblHeading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeading.Location = new System.Drawing.Point(198, 40);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeading.Size = new System.Drawing.Size(517, 95);
            this.lblHeading.TabIndex = 4;
            this.lblHeading.Text = "Search shipping address";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 3;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel2.Controls.Add(this.txtKeyboardInput, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnSearch, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.btnClear, 2, 1);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(27, 138);
            this.tableLayoutPanel2.Margin = new System.Windows.Forms.Padding(1, 3, 3, 3);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 2;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.Size = new System.Drawing.Size(857, 35);
            this.tableLayoutPanel2.TabIndex = 10;
            // 
            // txtKeyboardInput
            // 
            this.txtKeyboardInput.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtKeyboardInput.Location = new System.Drawing.Point(3, 3);
            this.txtKeyboardInput.Name = "txtKeyboardInput";
            this.txtKeyboardInput.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtKeyboardInput.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtKeyboardInput.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtKeyboardInput.Properties.Appearance.Options.UseBackColor = true;
            this.txtKeyboardInput.Properties.Appearance.Options.UseFont = true;
            this.txtKeyboardInput.Properties.Appearance.Options.UseForeColor = true;
            this.txtKeyboardInput.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtKeyboardInput.Size = new System.Drawing.Size(725, 32);
            this.txtKeyboardInput.TabIndex = 1;
            this.txtKeyboardInput.TextChanged += new System.EventHandler(this.txtKeyboardInput_TextChanged);
            this.txtKeyboardInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.frmShippingAddressSearch_KeyDown);
            this.txtKeyboardInput.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.txtKeyboardInput_PreviewKeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnSearch.Appearance.Options.UseFont = true;
            this.btnSearch.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.search;
            this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearch.Location = new System.Drawing.Point(734, 3);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(57, 32);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "Search";
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // btnClear
            // 
            this.btnClear.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnClear.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F);
            this.btnClear.Appearance.Options.UseFont = true;
            this.btnClear.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.remove;
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(797, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(57, 32);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "û";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 10;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.Controls.Add(this.btnPgDown, 9, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnDown, 8, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnCancelSearch, 6, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnSelect, 5, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnEdit, 4, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnUp, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnNew, 3, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnPgUp, 0, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(26, 635);
            this.tableLayoutPanel8.Margin = new System.Windows.Forms.Padding(0, 11, 0, 0);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(861, 65);
            this.tableLayoutPanel8.TabIndex = 12;
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.bottom;
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(799, 4);
            this.btnPgDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(57, 57);
            this.btnPgDown.TabIndex = 18;
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.btnPgDown_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.down;
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(734, 4);
            this.btnDown.Margin = new System.Windows.Forms.Padding(4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(57, 57);
            this.btnDown.TabIndex = 15;
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnCancelSearch
            // 
            this.btnCancelSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancelSearch.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnCancelSearch.Appearance.Options.UseFont = true;
            this.btnCancelSearch.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancelSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnCancelSearch.Location = new System.Drawing.Point(569, 4);
            this.btnCancelSearch.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancelSearch.Name = "btnCancelSearch";
            this.btnCancelSearch.Size = new System.Drawing.Size(127, 57);
            this.btnCancelSearch.TabIndex = 14;
            this.btnCancelSearch.Text = "Cancel";
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSelect.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSelect.Appearance.Options.UseFont = true;
            this.btnSelect.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSelect.Location = new System.Drawing.Point(434, 4);
            this.btnSelect.Margin = new System.Windows.Forms.Padding(4);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(127, 57);
            this.btnSelect.TabIndex = 13;
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.up;
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(69, 4);
            this.btnUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(57, 57);
            this.btnUp.TabIndex = 11;
            this.btnUp.Text = "ñ";
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnPgUp
            // 
            this.btnPgUp.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnPgUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgUp.Appearance.Options.UseFont = true;
            this.btnPgUp.Image = global::Microsoft.Dynamics.Retail.Pos.Customer.Properties.Resources.top;
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(4, 4);
            this.btnPgUp.Margin = new System.Windows.Forms.Padding(4);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Size = new System.Drawing.Size(57, 57);
            this.btnPgUp.TabIndex = 12;
            this.btnPgUp.Text = "Ç";
            this.btnPgUp.Click += new System.EventHandler(this.btnPgUp_Click);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.grAddress, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(26, 176);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 8;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 56F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(861, 448);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // frmShippingAddressSearch
            // 
            this.ClientSize = new System.Drawing.Size(917, 715);
            this.Controls.Add(this.basePanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmShippingAddressSearch";
            this.Text = "Shipping address search";
            this.Controls.SetChildIndex(this.basePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grAddress)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).EndInit();
            this.basePanel.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            this.tableLayoutPanel7.PerformLayout();
            this.tableLayoutPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).EndInit();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        #endregion

        private void TranslateLabels()
        {
            // Get all text through the Translation function in the ApplicationLocalizer
            // TextID's for Customer BalanceReport are reserved at 51100 - 51119
            // Used textid's 51100 - 51110

            grdView.Columns[0].Caption = ApplicationLocalizer.Language.Translate(904); //Account
            grdView.Columns[1].Caption = ApplicationLocalizer.Language.Translate(51107); //Name
            grdView.Columns[2].Caption = ApplicationLocalizer.Language.Translate(51108); //Address

            btnSelect.Text = ApplicationLocalizer.Language.Translate(103124); //Select
            btnEdit.Text    = ApplicationLocalizer.Language.Translate(103150);// Edit
            btnNew.Text     = ApplicationLocalizer.Language.Translate(99130); //New
            btnCancelSearch.Text = ApplicationLocalizer.Language.Translate(103125); //Cancel
            lblHeading.Text = ApplicationLocalizer.Language.Translate(51194); // Search shipping address
        }

        /// <summary>
        /// Gets or sets the selected address
        /// </summary>
        public IAddress SelectedAddress { get; private set; }

        private void btnDown_Click(object sender, EventArgs e)
        {
            grdView.MoveNext();
            SetFormFocus();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            grdView.MovePrev();
            SetFormFocus();
        }

        private void btnPgDown_Click(object sender, EventArgs e)
        {
            grdView.MoveNextPage();
            SetFormFocus();
        }

        private void btnPgUp_Click(object sender, EventArgs e)
        {
            grdView.MovePrevPage();
            SetFormFocus();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectShippingAddress();
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            object row = GetCurrentRow();

            if (row != null)
            {
                isEdit = true;
                long recId = Convert.ToInt64(((AddressInfo)row).Id);

                Customer.EditShippingAddress(recId, this.customer);
                GetAddressList();
            }
        }

        /// <summary>
        /// Return the data-row for the currently selected/focused row in the grid
        /// </summary>
        /// <returns>DataRow if it exists, null otherwise</returns>
        private object GetCurrentRow()
        {
            ColumnView view = (ColumnView)grAddress.MainView;
            if (view != null)
            {
                return view.GetFocusedRow();
            }
            return null;
        }

        private void btnNew_Click(object sender, EventArgs e)
        {
            CreateNewShippingAddress();
        }

        private void CreateNewShippingAddress()
        {
            Customer.AddNewShippingAddress(this.customer);
            GetAddressList();
        }


        private void EnterButtonPressed()
        {
            if (!inputHasChanged)
            {
                btnSelect_Click(null, null);
            }
            else
            {
                GetAddressList();
            }

            inputHasChanged = false;
        }

        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            Point p = grdView.GridControl.PointToClient(MousePosition);
            GridHitInfo info = grdView.CalcHitInfo(p);

            if (info.HitTest != GridHitTest.Column)
            {
                SelectShippingAddress();
            }
        }

        private void SelectShippingAddress()
        {
            DialogResult result = DialogResult.Cancel;
            if (grdView.RowCount > 0)
            {
                int selectedIndex = grdView.GetSelectedRows()[0];

                if (this.addressList.Count() > selectedIndex)
                {
                    var selectedAddress = this.addressList[selectedIndex];

                    this.SelectedAddress = customerDataManager.GetAddress(selectedAddress.Id);
                    result = DialogResult.OK;
                }
            }

            this.DialogResult = result;
            Close();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            txtKeyboardInput.Text = string.Empty;
            GetAddressList();
        }

        private void gridView1_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
        {
            SetFormFocus();
        }

        private void SetFormFocus()
        {
            txtKeyboardInput.Select();
        }

        private void keyboard1_UpButtonPressed()
        {
            btnUp_Click(this, new EventArgs());
        }

        private void keyboard1_DownButtonPressed()
        {
            btnDown_Click(this, new EventArgs());
        }

        private void keyboard1_PgUpButtonPressed()
        {
            btnPgUp_Click(this, new EventArgs());
        }

        private void keyboard1_PgDownButtonPressed()
        {
            btnPgDown_Click(this, new EventArgs());
        }

        private void grdView_MouseDown(object sender, MouseEventArgs e)
        {
            Point p = grdView.GridControl.PointToClient(Control.MousePosition);
            GridHitInfo info = grdView.CalcHitInfo(p);

            if (info.HitTest == GridHitTest.Column)
            {
                
                if (sortBy == info.Column.FieldName)
                {
                    sortAsc = !sortAsc;
                }
                else
                {
                    sortAsc = true;
                }

                sortBy = info.Column.FieldName;

                GetAddressList();

                foreach (DevExpress.XtraGrid.Columns.GridColumn col in grdView.Columns)
                {
                    col.SortOrder = DevExpress.Data.ColumnSortOrder.None;
                }

                grdView.Columns[sortBy].SortOrder = sortAsc ? DevExpress.Data.ColumnSortOrder.Ascending : DevExpress.Data.ColumnSortOrder.Descending;
            }
        }

        private void grItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter || e.KeyCode == Keys.Return)
            {
                btnSelect_Click(null, null);
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            inputHasChanged = true;

            if (!string.IsNullOrEmpty(txtKeyboardInput.Text))
            {
                EnterButtonPressed();
            }
            else
            {
                btnClear_Click(sender, e);
            }
        }

        public void txtKeyboardInput_TextChanged(object sender, EventArgs e)
        {
            inputHasChanged = true;
        }

        private void frmShippingAddressSearch_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Return:
                    EnterButtonPressed();
                    break;
                case Keys.Up:
                    keyboard1_UpButtonPressed();
                    break;
                case Keys.Down:
                    keyboard1_DownButtonPressed();
                    break;
                case Keys.PageUp:
                    keyboard1_PgUpButtonPressed();
                    break;
                case Keys.PageDown:
                    keyboard1_PgDownButtonPressed();
                    break;
                default:
                    break;
            }
        }

        private void txtKeyboardInput_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
