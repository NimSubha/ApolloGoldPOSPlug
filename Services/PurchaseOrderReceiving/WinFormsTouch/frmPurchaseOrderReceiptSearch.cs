/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.POSProcesses.WinControls;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.PurchaseOrderReceiving.WinFormsTouch
{
	/// <summary>
	/// Summary description for frmPurchaseOrderReceiptSearch
	/// </summary>
	public class frmPurchaseOrderReceiptSearch : frmTouchBase
	{
		/// <summary>
		/// Formatter that converts PurchaseOrderReceiptStatus enum values in a grid column to user-displayable text
		/// </summary>
		private class StatusFormatter : IFormatProvider, ICustomFormatter
		{
			private readonly string StatusInProgress;
			private readonly string StatusOpen;
			private readonly string StatusClosed;

			/// <summary>
			/// Formats status settings.
			/// </summary>
			public StatusFormatter()
			{
				this.StatusInProgress = ApplicationLocalizer.Language.Translate(103128);   // In Progress
				this.StatusClosed = ApplicationLocalizer.Language.Translate(103129);   // Closed
				this.StatusOpen = ApplicationLocalizer.Language.Translate(103127);   // Open
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
						case (int)PurchaseOrderReceiptStatus.InProgress:
							return this.StatusInProgress;
						case (int)PurchaseOrderReceiptStatus.Open:
							return this.StatusOpen;
						case (int)PurchaseOrderReceiptStatus.Closed:
							return this.StatusClosed;
						default:
							return string.Empty;
					}
				}

				return (arg == null) ? string.Empty : arg.ToString();
			}
		}

		/// <summary>
		/// Formatter that converts PurchaseOrderReceiptCountingType enum values in a grid column to user-displayable text
		/// </summary>
		private class OrderTypeFormatter : IFormatProvider, ICustomFormatter
		{
			private readonly string PurchaseOrder;
			private readonly string TransferIn;
			private readonly string TransferOut;
			private readonly string PickingList;

			/// <summary>
			/// Formats status settings.
			/// </summary>
			public OrderTypeFormatter()
			{
				this.PurchaseOrder = ApplicationLocalizer.Language.Translate(10534); //Purchase Order 
				this.TransferIn = ApplicationLocalizer.Language.Translate(10536); //Transfer In
				this.TransferOut = ApplicationLocalizer.Language.Translate(10535); //Transfer Out
				this.PickingList = ApplicationLocalizer.Language.Translate(10537); // Picking List   
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
						case (int)PRCountingType.PurchaseOrder:
							return this.PurchaseOrder;
						case (int)PRCountingType.TransferIn:
							return this.TransferIn;
						case (int)PRCountingType.TransferOut:
							return this.TransferOut;
						case (int)PRCountingType.PickingList:
							return this.PickingList;
						default:
							return string.Empty;
					}
				}

				return (arg == null) ? string.Empty : arg.ToString();
			}
		}
		private DevExpress.XtraGrid.Blending.XtraGridBlending xtraGridBlending1;
		private PanelControl basePanel;

		private PurchaseOrderReceiptData receiptData;
		private System.Data.DataTable entryTable;
		private const int maxRowsAtEachQuery = 200;
		//private int loadedCount = 0;
		private string sortBy = "ReceiptNumber";
		private bool sortAsc = true;
		private string lastSearch = "";
		private TableLayoutPanel tableLayoutPanel6;
		private TableLayoutPanel tableLayoutPanel7;
		private DevExpress.XtraGrid.GridControl grReceipts;
		private DevExpress.XtraGrid.Views.Grid.GridView grdView;
		private DevExpress.XtraGrid.Columns.GridColumn colOrderType;
		private DevExpress.XtraGrid.Columns.GridColumn colPoNumber;
		private DevExpress.XtraGrid.Columns.GridColumn colStatus;
		private DevExpress.XtraGrid.Views.Grid.GridView gridView1;
		private TableLayoutPanel tableLayoutPanel1;
		private DevExpress.XtraEditors.TextEdit txtKeyboardInput;
		private SimpleButtonEx btnSearch;
		private SimpleButtonEx btnClear;
		private SimpleButtonEx btnSelect;
		private SimpleButtonEx btnClose;
		private SimpleButtonEx btnRefresh;
		private SimpleButtonEx btnUp;
		private SimpleButtonEx btnPgUp;
		private SimpleButtonEx btnPgDown;
		private SimpleButtonEx btnDown;
		private Label lblHeading;
		private TableLayoutPanel tableLayoutPanel8;

		/// <summary>
		/// Get/set property for singaling if form is called first time
		/// </summary>
		public bool RepeatCalled { get; set; }

		/// <summary>
		/// Get/set property for seleceted receipt number.
		/// </summary>
		public string SelectedReceiptNumber { get; set; }

		/// <summary>
		/// Get/set property for selected purchase Id
		/// </summary>
		public string SelectedPONumber { get; set; }

		/// <summary>
		/// Get/set property for selected purchase receipt type
		/// </summary>
		public PRCountingType SelectedPRType { get; set; }

		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Displays form of purchase order receipts search.
		/// </summary>
		public frmPurchaseOrderReceiptSearch()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			colStatus.DisplayFormat.Format = new StatusFormatter();
			colOrderType.DisplayFormat.Format = new OrderTypeFormatter();
		}

		protected override void OnLoad(EventArgs e)
		{
			if (!this.DesignMode)
			{
				this.Bounds = new Rectangle(
					new Point(ApplicationSettings.MainWindowLeft, ApplicationSettings.MainWindowTop),
					new Size(ApplicationSettings.MainWindowWidth, ApplicationSettings.MainWindowHeight));

				receiptData = new PurchaseOrderReceiptData(
					ApplicationSettings.Database.LocalConnection,
					ApplicationSettings.Database.DATAAREAID,
					ApplicationSettings.Terminal.StorePrimaryId);

				grdView.FocusedColumn = grdView.Columns["RECEIPTNUMBER"];

				this.RefreshReceiptList();

				TranslateLabels();

				SetFormFocus();
			}

			base.OnLoad(e);
		}

		private void TranslateLabels()
		{
			// Get all text through the Translation function in the ApplicationLocalizer

			colOrderType.Caption = ApplicationLocalizer.Language.Translate(1031201); //Type
			colPoNumber.Caption = ApplicationLocalizer.Language.Translate(103121); //Number
			colStatus.Caption = ApplicationLocalizer.Language.Translate(103122); //Status

			btnSelect.Text = ApplicationLocalizer.Language.Translate(103124); //Select
			btnRefresh.Text = ApplicationLocalizer.Language.Translate(103126); //Refresh
			btnClose.Text = ApplicationLocalizer.Language.Translate(103125); //Close

			//Title
			this.Text = ApplicationLocalizer.Language.Translate(1031202); //Picking/Receiving
			this.lblHeading.Text = ApplicationLocalizer.Language.Translate(1031202); //Picking/Receiving
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

		private void SelectReceipt()
		{
			if (grdView.RowCount > 0)
			{
				System.Data.DataRow Row = grdView.GetDataRow(grdView.GetSelectedRows()[0]);
				this.SelectedReceiptNumber = Row["RECEIPTNUMBER"] as string;
				this.SelectedPONumber = Row["PONUMBER"] as string;
				this.SelectedPRType = (PRCountingType)Enum.Parse(typeof(PRCountingType), Row["ORDERTYPE"].ToString(), true);
				this.DialogResult = System.Windows.Forms.DialogResult.OK;
				Close();
			}
			else
			{
				this.DialogResult = System.Windows.Forms.DialogResult.Cancel;
				Close();
			}
		}

		private void SetFormFocus()
		{
			txtKeyboardInput.Select();
		}

		private void RefreshReceiptList()
		{
			try
			{
				this.UseWaitCursor = true;

				// Purge all rows on grid
				if (this.entryTable != null)
				{
					this.entryTable.Rows.Clear();
				}

				// Download POs from AX and update POs in DB
				IList<IPRDocument> prDocuments = PurchaseOrderReceiving.InternalApplication.Services.StoreInventoryServices.GetOrderReceipts();

				if (prDocuments != null && prDocuments.Count > 0)
				{
					// Load POs from DB and populate to grid control
					LoadReceiptList(true);
				}
			}
			finally
			{
				this.UseWaitCursor = false;
			}
		}

		private void LoadReceiptList(bool localOnly)
		{
			// Load POs from database
			this.entryTable = receiptData.GetPurchaseOrderReceipts(txtKeyboardInput.Text);

			if (this.entryTable != null && this.entryTable.Rows.Count == 0 && !RepeatCalled & !localOnly)
			{
				RefreshReceiptList();
			}

			// Populate grid control
			grReceipts.DataSource = this.entryTable;
		}

		private void btnUp_Click(object sender, EventArgs e)
		{
			grdView.MovePrev();
			SetFormFocus();
		}

		private void btnDown_Click(object sender, EventArgs e)
		{
			grdView.MoveNext();
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
			SelectReceipt();
		}

		private void keyboard1_EnterButtonPressed()
		{
			if ((txtKeyboardInput.Text.Trim().Length == 0) || (txtKeyboardInput.Text == lastSearch))
			{
				btnSelect_Click(null, null);
			}
			else
			{
				lastSearch = txtKeyboardInput.Text;
				LoadReceiptList(true);
			}
		}

		private void gridView1_DoubleClick(object sender, EventArgs e)
		{
			Point p = grdView.GridControl.PointToClient(MousePosition);
			GridHitInfo info = grdView.CalcHitInfo(p);

			if (info.HitTest != GridHitTest.Column)
			{
				SelectReceipt();
			}
		}

		private void btnClear_Click(object sender, EventArgs e)
		{
			txtKeyboardInput.Text = string.Empty;
			LoadReceiptList(true);
		}

		private void gridView1_FocusedColumnChanged(object sender, DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventArgs e)
		{
			SetFormFocus();
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
			Point p = grdView.GridControl.PointToClient(MousePosition);
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
				LoadReceiptList(true);

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

		private void btnRefresh_Click(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				RefreshReceiptList();
				grReceipts.DataSource = this.entryTable;
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmPurchaseOrderReceiptSearch));
            this.xtraGridBlending1 = new DevExpress.XtraGrid.Blending.XtraGridBlending();
            this.basePanel = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel8 = new System.Windows.Forms.TableLayoutPanel();
            this.btnPgDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClose = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnDown = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnSelect = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnRefresh = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnPgUp = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel7 = new System.Windows.Forms.TableLayoutPanel();
            this.grReceipts = new DevExpress.XtraGrid.GridControl();
            this.grdView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.colOrderType = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colPoNumber = new DevExpress.XtraGrid.Columns.GridColumn();
            this.colStatus = new DevExpress.XtraGrid.Columns.GridColumn();
            this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.txtKeyboardInput = new DevExpress.XtraEditors.TextEdit();
            this.btnSearch = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnClear = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblHeading = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).BeginInit();
            this.basePanel.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel8.SuspendLayout();
            this.tableLayoutPanel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grReceipts)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).BeginInit();
            this.SuspendLayout();
            // 
            // xtraGridBlending1
            // 
            this.xtraGridBlending1.AlphaStyles.AddReplace("Row", 220);
            // 
            // basePanel
            // 
            this.basePanel.Controls.Add(this.tableLayoutPanel6);
            this.basePanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.basePanel.Location = new System.Drawing.Point(0, 0);
            this.basePanel.Margin = new System.Windows.Forms.Padding(0);
            this.basePanel.Name = "basePanel";
            this.basePanel.Size = new System.Drawing.Size(1024, 768);
            this.basePanel.TabIndex = 8;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 1;
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel8, 0, 3);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel7, 0, 2);
            this.tableLayoutPanel6.Controls.Add(this.tableLayoutPanel1, 0, 1);
            this.tableLayoutPanel6.Controls.Add(this.lblHeading, 0, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.Padding = new System.Windows.Forms.Padding(26, 40, 26, 11);
            this.tableLayoutPanel6.RowCount = 4;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel6.Size = new System.Drawing.Size(1020, 764);
            this.tableLayoutPanel6.TabIndex = 9;
            // 
            // tableLayoutPanel8
            // 
            this.tableLayoutPanel8.ColumnCount = 9;
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel8.Controls.Add(this.btnPgDown, 8, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnClose, 5, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnDown, 7, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnSelect, 3, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnRefresh, 4, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnUp, 1, 0);
            this.tableLayoutPanel8.Controls.Add(this.btnPgUp, 0, 0);
            this.tableLayoutPanel8.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel8.Location = new System.Drawing.Point(26, 688);
            this.tableLayoutPanel8.Margin = new System.Windows.Forms.Padding(0, 11, 0, 0);
            this.tableLayoutPanel8.Name = "tableLayoutPanel8";
            this.tableLayoutPanel8.RowCount = 1;
            this.tableLayoutPanel8.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel8.Size = new System.Drawing.Size(968, 65);
            this.tableLayoutPanel8.TabIndex = 10;
            // 
            // btnPgDown
            // 
            this.btnPgDown.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnPgDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnPgDown.Appearance.Options.UseFont = true;
            this.btnPgDown.Image = ((System.Drawing.Image)(resources.GetObject("btnPgDown.Image")));
            this.btnPgDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgDown.Location = new System.Drawing.Point(907, 4);
            this.btnPgDown.Name = "btnPgDown";
            this.btnPgDown.Size = new System.Drawing.Size(57, 57);
            this.btnPgDown.TabIndex = 16;
            this.btnPgDown.Text = "Ê";
            this.btnPgDown.Click += new System.EventHandler(this.btnPgDown_Click);
            // 
            // btnClose
            // 
            this.btnClose.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnClose.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnClose.Appearance.Options.UseFont = true;
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnClose.Location = new System.Drawing.Point(553, 4);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(127, 57);
            this.btnClose.TabIndex = 14;
            this.btnClose.Tag = "";
            this.btnClose.Text = "Close";
            // 
            // btnDown
            // 
            this.btnDown.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnDown.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnDown.Appearance.Options.UseFont = true;
            this.btnDown.Image = ((System.Drawing.Image)(resources.GetObject("btnDown.Image")));
            this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnDown.Location = new System.Drawing.Point(844, 4);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(57, 57);
            this.btnDown.TabIndex = 15;
            this.btnDown.Text = "ò";
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSelect.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnSelect.Appearance.Options.UseFont = true;
            this.btnSelect.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSelect.Location = new System.Drawing.Point(287, 4);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(127, 57);
            this.btnSelect.TabIndex = 12;
            this.btnSelect.Tag = "";
            this.btnSelect.Text = "Select";
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnRefresh.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnRefresh.Appearance.Options.UseFont = true;
            this.btnRefresh.Location = new System.Drawing.Point(420, 4);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(127, 57);
            this.btnRefresh.TabIndex = 13;
            this.btnRefresh.Tag = "";
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnUp.Appearance.Font = new System.Drawing.Font("Wingdings", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.btnUp.Appearance.Options.UseFont = true;
            this.btnUp.Image = ((System.Drawing.Image)(resources.GetObject("btnUp.Image")));
            this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnUp.Location = new System.Drawing.Point(66, 4);
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
            this.btnPgUp.Image = ((System.Drawing.Image)(resources.GetObject("btnPgUp.Image")));
            this.btnPgUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnPgUp.Location = new System.Drawing.Point(3, 4);
            this.btnPgUp.Name = "btnPgUp";
            this.btnPgUp.Padding = new System.Windows.Forms.Padding(0);
            this.btnPgUp.Size = new System.Drawing.Size(57, 57);
            this.btnPgUp.TabIndex = 10;
            this.btnPgUp.Text = "Ç";
            this.btnPgUp.Click += new System.EventHandler(this.btnPgUp_Click);
            // 
            // tableLayoutPanel7
            // 
            this.tableLayoutPanel7.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel7.ColumnCount = 1;
            this.tableLayoutPanel7.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel7.Controls.Add(this.grReceipts, 0, 0);
            this.tableLayoutPanel7.Location = new System.Drawing.Point(29, 182);
            this.tableLayoutPanel7.Name = "tableLayoutPanel7";
            this.tableLayoutPanel7.RowCount = 1;
            this.tableLayoutPanel7.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 38.95131F));
            this.tableLayoutPanel7.Size = new System.Drawing.Size(962, 492);
            this.tableLayoutPanel7.TabIndex = 14;
            // 
            // grReceipts
            // 
            this.grReceipts.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.grReceipts.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grReceipts.Location = new System.Drawing.Point(3, 3);
            this.grReceipts.MainView = this.grdView;
            this.grReceipts.Name = "grReceipts";
            this.grReceipts.Size = new System.Drawing.Size(956, 486);
            this.grReceipts.TabIndex = 20;
            this.grReceipts.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.grdView,
            this.gridView1});
            this.grReceipts.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grItems_KeyDown);
            // 
            // grdView
            // 
            this.grdView.Appearance.HeaderPanel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.grdView.Appearance.HeaderPanel.Options.UseFont = true;
            this.grdView.Appearance.Row.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.grdView.Appearance.Row.Options.UseFont = true;
            this.grdView.ColumnPanelRowHeight = 40;
            this.grdView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
            this.colOrderType,
            this.colPoNumber,
            this.colStatus});
            this.grdView.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.None;
            this.grdView.GridControl = this.grReceipts;
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
            this.grdView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
            new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colPoNumber, DevExpress.Data.ColumnSortOrder.Ascending)});
            this.grdView.VertScrollVisibility = DevExpress.XtraGrid.Views.Base.ScrollVisibility.Never;
            this.grdView.FocusedColumnChanged += new DevExpress.XtraGrid.Views.Base.FocusedColumnChangedEventHandler(this.gridView1_FocusedColumnChanged);
            this.grdView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grItems_KeyDown);
            this.grdView.MouseDown += new System.Windows.Forms.MouseEventHandler(this.grdView_MouseDown);
            this.grdView.DoubleClick += new System.EventHandler(this.gridView1_DoubleClick);
            // 
            // colOrderType
            // 
            this.colOrderType.AppearanceCell.Options.UseTextOptions = true;
            this.colOrderType.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.colOrderType.AppearanceHeader.Options.UseTextOptions = true;
            this.colOrderType.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.colOrderType.Caption = "Order type";
            this.colOrderType.FieldName = "ORDERTYPE";
            this.colOrderType.Name = "colOrderType";
            this.colOrderType.OptionsColumn.AllowEdit = false;
            this.colOrderType.Visible = true;
            this.colOrderType.VisibleIndex = 0;
            // 
            // colPoNumber
            // 
            this.colPoNumber.Caption = "Order number";
            this.colPoNumber.FieldName = "PONUMBER";
            this.colPoNumber.Name = "colPoNumber";
            this.colPoNumber.OptionsColumn.AllowEdit = false;
            this.colPoNumber.Visible = true;
            this.colPoNumber.VisibleIndex = 1;
            // 
            // colStatus
            // 
            this.colStatus.AppearanceCell.Options.UseTextOptions = true;
            this.colStatus.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.colStatus.AppearanceHeader.Options.UseTextOptions = true;
            this.colStatus.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
            this.colStatus.Caption = "Status";
            this.colStatus.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.colStatus.FieldName = "STATUS";
            this.colStatus.Name = "colStatus";
            this.colStatus.OptionsColumn.AllowEdit = false;
            this.colStatus.Visible = true;
            this.colStatus.VisibleIndex = 2;
            // 
            // gridView1
            // 
            this.gridView1.GridControl = this.grReceipts;
            this.gridView1.Name = "gridView1";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.txtKeyboardInput, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnSearch, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.btnClear, 2, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(29, 138);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 5, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(960, 38);
            this.tableLayoutPanel1.TabIndex = 13;
            // 
            // txtKeyboardInput
            // 
            this.txtKeyboardInput.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.txtKeyboardInput.Location = new System.Drawing.Point(3, 3);
            this.txtKeyboardInput.Name = "txtKeyboardInput";
            this.txtKeyboardInput.Properties.Appearance.BackColor = System.Drawing.Color.White;
            this.txtKeyboardInput.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.txtKeyboardInput.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
            this.txtKeyboardInput.Properties.Appearance.Options.UseBackColor = true;
            this.txtKeyboardInput.Properties.Appearance.Options.UseFont = true;
            this.txtKeyboardInput.Properties.Appearance.Options.UseForeColor = true;
            this.txtKeyboardInput.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
            this.txtKeyboardInput.Size = new System.Drawing.Size(828, 32);
            this.txtKeyboardInput.TabIndex = 1;
            this.txtKeyboardInput.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtKeyboardInput_KeyDown);
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnSearch.Location = new System.Drawing.Point(837, 3);
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
            this.btnClear.Image = ((System.Drawing.Image)(resources.GetObject("btnClear.Image")));
            this.btnClear.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnClear.Location = new System.Drawing.Point(900, 3);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(57, 32);
            this.btnClear.TabIndex = 3;
            this.btnClear.Text = "û";
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // lblHeading
            // 
            this.lblHeading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeading.Location = new System.Drawing.Point(284, 40);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeading.Size = new System.Drawing.Size(452, 95);
            this.lblHeading.TabIndex = 18;
            this.lblHeading.Tag = "";
            this.lblHeading.Text = "Picking and receiving";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // frmPurchaseOrderReceiptSearch
            // 
            this.ClientSize = new System.Drawing.Size(1024, 768);
            this.Controls.Add(this.basePanel);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmPurchaseOrderReceiptSearch";
            this.Controls.SetChildIndex(this.basePanel, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.basePanel)).EndInit();
            this.basePanel.ResumeLayout(false);
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel6.PerformLayout();
            this.tableLayoutPanel8.ResumeLayout(false);
            this.tableLayoutPanel7.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grReceipts)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.txtKeyboardInput.Properties)).EndInit();
            this.ResumeLayout(false);

		}
		#endregion

		private void btnSearch_Click(object sender, EventArgs e)
		{
			Cursor.Current = Cursors.WaitCursor;

			try
			{
				if (!(txtKeyboardInput.Text == lastSearch))
				{
					lastSearch = txtKeyboardInput.Text;
					LoadReceiptList(true);
				}
			}
			finally
			{
				Cursor.Current = Cursors.Default;
			}
		}

		private void txtKeyboardInput_KeyDown(object sender, KeyEventArgs e)
		{
			switch (e.KeyCode)
			{
				case Keys.Return:
					keyboard1_EnterButtonPressed();
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
	}
}
