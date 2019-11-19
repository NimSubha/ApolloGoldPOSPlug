using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using BlankOperations;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Data.SqlClient;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public class frmSearchOrder:LSRetailPosis.POSControls.Touch.frmTouchBase
    {

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if(disposing && (components != null))
            {
                components.Dispose();
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.grItems = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colOrderNum = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colCustomerName = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colOrderDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colDeliveryDate = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colTotalAmount = new DevExpress.XtraGrid.Columns.GridColumn();
            this.xtraGridBlending1 = new DevExpress.XtraGrid.Blending.XtraGridBlending();
            this.btnOK = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblGss = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.grItems, 0, 0);
            this.tableLayoutPanel1.ForeColor = System.Drawing.Color.Black;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(14, 91);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 314F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 314F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 314F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 314F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 314F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(980, 314);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // grItems
            // 
            this.grItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grItems.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grItems.Location = new System.Drawing.Point(3, 3);
            this.grItems.MainView = this.grdView;
            this.grItems.Name = "grItems";
            this.grItems.Size = new System.Drawing.Size(976, 308);
            this.grItems.TabIndex = 0;
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
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colOrderNum,
            this.colCustomerName,
            this.colOrderDate,
            this.colDeliveryDate,
            this.colTotalAmount});
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
            this.grdView.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // colOrderNum
            // 
            this.colOrderNum.Caption = "Order Number";
            this.colOrderNum.FieldName = "ORDER NO.";
            this.colOrderNum.Name = "colOrderNum";
            this.colOrderNum.OptionsColumn.AllowEdit = false;
            this.colOrderNum.OptionsColumn.AllowIncrementalSearch = false;
            this.colOrderNum.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colOrderNum.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colOrderNum.Visible = true;
            this.colOrderNum.VisibleIndex = 0;
            this.colOrderNum.Width = 150;
            // 
            // colCustomerName
            // 
            this.colCustomerName.Caption = "Customer Name";
            this.colCustomerName.FieldName = "CUSTOMER NAME";
            this.colCustomerName.Name = "colCustomerName";
            this.colCustomerName.OptionsColumn.AllowEdit = false;
            this.colCustomerName.OptionsColumn.AllowIncrementalSearch = false;
            this.colCustomerName.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colCustomerName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colCustomerName.Visible = true;
            this.colCustomerName.VisibleIndex = 1;
            this.colCustomerName.Width = 150;
            // 
            // colOrderDate
            // 
            this.colOrderDate.Caption = "Order Date";
            this.colOrderDate.FieldName = "ORDER DATE";
            this.colOrderDate.Name = "colOrderDate";
            this.colOrderDate.OptionsColumn.AllowEdit = false;
            this.colOrderDate.OptionsColumn.AllowIncrementalSearch = false;
            this.colOrderDate.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colOrderDate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colOrderDate.Visible = true;
            this.colOrderDate.VisibleIndex = 2;
            this.colOrderDate.Width = 150;
            // 
            // colDeliveryDate
            // 
            this.colDeliveryDate.Caption = "Delivery Date";
            this.colDeliveryDate.FieldName = "DELIVERY DATE";
            this.colDeliveryDate.Name = "colDeliveryDate";
            this.colDeliveryDate.OptionsColumn.AllowEdit = false;
            this.colDeliveryDate.OptionsColumn.AllowIncrementalSearch = false;
            this.colDeliveryDate.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colDeliveryDate.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colDeliveryDate.Visible = true;
            this.colDeliveryDate.VisibleIndex = 3;
            this.colDeliveryDate.Width = 150;
            // 
            // colTotalAmount
            // 
            this.colTotalAmount.Caption = "Total Amount";
            this.colTotalAmount.FieldName = "TOTAL AMOUNT";
            this.colTotalAmount.Name = "colTotalAmount";
            this.colTotalAmount.OptionsColumn.AllowEdit = false;
            this.colTotalAmount.OptionsColumn.AllowIncrementalSearch = false;
            this.colTotalAmount.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
            this.colTotalAmount.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
            this.colTotalAmount.Visible = true;
            this.colTotalAmount.VisibleIndex = 4;
            this.colTotalAmount.Width = 150;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnOK.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.Location = new System.Drawing.Point(396, 415);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(98, 45);
            this.btnOK.TabIndex = 146;
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(500, 415);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(98, 45);
            this.btnCancel.TabIndex = 145;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblGss
            // 
            this.lblGss.AutoSize = true;
            this.lblGss.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblGss.ForeColor = System.Drawing.Color.Black;
            this.lblGss.Location = new System.Drawing.Point(285, 9);
            this.lblGss.Name = "lblGss";
            this.lblGss.Size = new System.Drawing.Size(467, 65);
            this.lblGss.TabIndex = 147;
            this.lblGss.Text = "Select customer order";
            // 
            // frmSearchOrder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1008, 546);
            this.Controls.Add(this.lblGss);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.tableLayoutPanel1);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmSearchOrder";
            this.Text = "Search Order";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraGrid.Columns.GridColumn colOrderNum;
        private DevExpress.XtraGrid.Columns.GridColumn colOrderDate;
        private DevExpress.XtraGrid.Columns.GridColumn colDeliveryDate;
        private DevExpress.XtraGrid.Columns.GridColumn colCustomerName;
        private DevExpress.XtraGrid.Columns.GridColumn colTotalAmount;



        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private DevExpress.XtraGrid.GridControl grItems;
        private DevExpress.XtraGrid.Views.Grid.GridView grdView;
        private DevExpress.XtraGrid.Blending.XtraGridBlending xtraGridBlending1;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnOK;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;


        #region Variable Declaration

        public IPosTransaction pos { get; set; }
        frmCustomerOrder frmCustOrder = null;
        DataTable dtGridItems = new DataTable();

        [Import]
        private IApplication application;
        private Label lblGss;

        public string sOrderNo = string.Empty;

        #endregion

        #region Search Order
        public frmSearchOrder(IPosTransaction posTransaction, IApplication Application, frmCustomerOrder fCustOrder)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            frmCustOrder = fCustOrder;
            BindGrid(string.Empty);

        }

        public frmSearchOrder(IPosTransaction posTransaction, IApplication Application, string sWhereCondition)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;


            BindGrid(sWhereCondition);

        }

        public frmSearchOrder(DataTable dtOrderOfMala)
        {
            InitializeComponent();

            if(dtOrderOfMala != null && dtOrderOfMala.Rows.Count > 0)
            {
                grItems.DataSource = dtOrderOfMala.DefaultView;
            }

        }
        #endregion

        public void ShowForm()
        {
            this.Show();
        }

        #region Cancel Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region BindGrid
        private void BindGrid(string sWhereCondition)
        {

            dtGridItems = new DataTable();

            string commandText = " SELECT ORDERNUM AS [ORDER NO.],ORDERDATE AS [ORDER DATE],DELIVERYDATE AS [DELIVERY DATE], " +
                                 " CUSTNAME AS [CUSTOMER NAME],TOTALAMOUNT AS [TOTAL AMOUNT]  FROM CUSTORDER_HEADER  " +
                                 sWhereCondition +
                                 " ORDER BY ORDERNUM ASC";
            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }


            SqlCommand command = new SqlCommand(commandText, connection);


            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            dtGridItems.Load(reader);
            if(dtGridItems != null && dtGridItems.Rows.Count > 0)
            {
                grItems.DataSource = dtGridItems.DefaultView;
            }

        }

       
        #endregion

        #region return Order Number
        private string OrderNumber()
        {
            int OrderSelectedIndex = 0;
            if(dtGridItems != null && dtGridItems.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    OrderSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToSend = dtGridItems.Rows[OrderSelectedIndex];
                    if(frmCustOrder != null)
                        sOrderNo = frmCustOrder.sCustOrderSearchNumber = Convert.ToString(theRowToSend["ORDER NO."]);
                    else
                        sOrderNo = Convert.ToString(theRowToSend["ORDER NO."]);
                }
            }
            return sOrderNo.Trim();
        }
        #endregion

        #region OK CLICK
        private void btnOK_Click(object sender, EventArgs e)
        {
            string str = OrderNumber();
            this.Close();
        }
        #endregion

        #region Double CLick
        private void gridView1_DoubleClick(object sender, EventArgs e)
        {
            string str = OrderNumber();
            this.Close();
        }
        #endregion
    }
}
