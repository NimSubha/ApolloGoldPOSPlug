/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.Contracts.UI;

namespace Microsoft.Dynamics.Retail.Pos.GiftCard
{
    partial class GiftCardForm
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
            if (disposing)
            {
                if (components != null)
                {
                    components.Dispose();
                }

                GiftCard.InternalApplication.Services.Peripherals.Scanner.ScannerMessageEvent -= new ScannerMessageEventHandler(ProcessScannedItem);
                GiftCard.InternalApplication.Services.Peripherals.MSR.MSRMessageEvent -= new MSRMessageEventHandler(ProcessSwipedCard);
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GiftCardForm));
            this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel3 = new System.Windows.Forms.TableLayoutPanel();
            this.tableLayoutPanel6 = new System.Windows.Forms.TableLayoutPanel();
            this.btnOk = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.tableLayoutPanel4 = new System.Windows.Forms.TableLayoutPanel();
            this.panelAmount = new DevExpress.XtraEditors.PanelControl();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.lblPayAmt = new System.Windows.Forms.Label();
            this.amtGiftCardAmounts = new LSRetailPosis.POSProcesses.WinControls.AmountViewer();
            this.numAmount = new LSRetailPosis.POSProcesses.WinControls.NumPad();
            this.lblGiftCardBalanceTitle = new System.Windows.Forms.Label();
            this.btnGet = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.lblGiftCardBalance = new System.Windows.Forms.Label();
            this.padGiftCardNumber = new LSRetailPosis.POSProcesses.WinControls.StringPad();
            this.lblHeading = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
            this.tableLayoutPanel3.SuspendLayout();
            this.tableLayoutPanel6.SuspendLayout();
            this.tableLayoutPanel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelAmount)).BeginInit();
            this.panelAmount.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // panelControl1
            // 
            this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.panelControl1.AutoSize = true;
            this.panelControl1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
            this.panelControl1.Location = new System.Drawing.Point(217, 191);
            this.panelControl1.Name = "panelControl1";
            this.panelControl1.Padding = new System.Windows.Forms.Padding(3);
            this.panelControl1.Size = new System.Drawing.Size(6, 6);
            this.panelControl1.TabIndex = 0;
            // 
            // tableLayoutPanel3
            // 
            this.tableLayoutPanel3.ColumnCount = 1;
            this.tableLayoutPanel3.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel6, 0, 2);
            this.tableLayoutPanel3.Controls.Add(this.tableLayoutPanel4, 0, 1);
            this.tableLayoutPanel3.Controls.Add(this.lblHeading, 0, 0);
            this.tableLayoutPanel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel3.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel3.Name = "tableLayoutPanel3";
            this.tableLayoutPanel3.Padding = new System.Windows.Forms.Padding(30, 40, 30, 11);
            this.tableLayoutPanel3.RowCount = 3;
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel3.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel3.Size = new System.Drawing.Size(988, 772);
            this.tableLayoutPanel3.TabIndex = 1;
            // 
            // tableLayoutPanel6
            // 
            this.tableLayoutPanel6.ColumnCount = 2;
            this.tableLayoutPanel3.SetColumnSpan(this.tableLayoutPanel6, 3);
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel6.Controls.Add(this.btnOk, 0, 0);
            this.tableLayoutPanel6.Controls.Add(this.btnCancel, 1, 0);
            this.tableLayoutPanel6.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel6.Location = new System.Drawing.Point(30, 696);
            this.tableLayoutPanel6.Margin = new System.Windows.Forms.Padding(0);
            this.tableLayoutPanel6.Name = "tableLayoutPanel6";
            this.tableLayoutPanel6.RowCount = 1;
            this.tableLayoutPanel6.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel6.Size = new System.Drawing.Size(928, 65);
            this.tableLayoutPanel6.TabIndex = 9;
            // 
            // btnOk
            // 
            this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnOk.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnOk.Appearance.Options.UseFont = true;
            this.btnOk.Location = new System.Drawing.Point(333, 4);
            this.btnOk.Margin = new System.Windows.Forms.Padding(4);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(127, 57);
            this.btnOk.TabIndex = 1;
            this.btnOk.Tag = "BtnExtraLong";
            this.btnOk.Text = "OK";
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(468, 4);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(127, 57);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Tag = "BtnExtraLong";
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // tableLayoutPanel4
            // 
            this.tableLayoutPanel4.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.tableLayoutPanel4.AutoSize = true;
            this.tableLayoutPanel4.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel4.ColumnCount = 3;
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel4.Controls.Add(this.panelAmount, 2, 2);
            this.tableLayoutPanel4.Controls.Add(this.numAmount, 1, 2);
            this.tableLayoutPanel4.Controls.Add(this.lblGiftCardBalanceTitle, 1, 1);
            this.tableLayoutPanel4.Controls.Add(this.btnGet, 2, 0);
            this.tableLayoutPanel4.Controls.Add(this.lblGiftCardBalance, 2, 1);
            this.tableLayoutPanel4.Controls.Add(this.padGiftCardNumber, 1, 0);
            this.tableLayoutPanel4.Location = new System.Drawing.Point(271, 205);
            this.tableLayoutPanel4.Name = "tableLayoutPanel4";
            this.tableLayoutPanel4.RowCount = 3;
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel4.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel4.Size = new System.Drawing.Size(445, 421);
            this.tableLayoutPanel4.TabIndex = 1;
            // 
            // panelAmount
            // 
            this.panelAmount.Controls.Add(this.tableLayoutPanel1);
            this.panelAmount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelAmount.Location = new System.Drawing.Point(259, 104);
            this.panelAmount.Margin = new System.Windows.Forms.Padding(5);
            this.panelAmount.Name = "panelAmount";
            this.panelAmount.Size = new System.Drawing.Size(181, 312);
            this.panelAmount.TabIndex = 18;
            this.panelAmount.Visible = false;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.lblPayAmt, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.amtGiftCardAmounts, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(2, 2);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 14, 3, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(177, 308);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // lblPayAmt
            // 
            this.lblPayAmt.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.lblPayAmt.AutoSize = true;
            this.lblPayAmt.Font = new System.Drawing.Font("Segoe UI", 14.25F);
            this.lblPayAmt.Location = new System.Drawing.Point(32, 0);
            this.lblPayAmt.Margin = new System.Windows.Forms.Padding(3, 0, 3, 5);
            this.lblPayAmt.Name = "lblPayAmt";
            this.lblPayAmt.Size = new System.Drawing.Size(112, 25);
            this.lblPayAmt.TabIndex = 10;
            this.lblPayAmt.Text = "Pay amount";
            // 
            // amtGiftCardAmounts
            // 
            this.amtGiftCardAmounts.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.amtGiftCardAmounts.CurrencyRate = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.amtGiftCardAmounts.ForeignCurrencyMode = false;
            this.amtGiftCardAmounts.HighestOptionAmount = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.amtGiftCardAmounts.IncludeRefAmount = true;
            this.amtGiftCardAmounts.LocalCurrencyCode = "";
            this.amtGiftCardAmounts.Location = new System.Drawing.Point(3, 33);
            this.amtGiftCardAmounts.LowesetOptionAmount = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.amtGiftCardAmounts.Name = "amtGiftCardAmounts";
            this.amtGiftCardAmounts.OptionsLimit = 5;
            this.amtGiftCardAmounts.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.amtGiftCardAmounts.ShowBorder = false;
            this.amtGiftCardAmounts.Size = new System.Drawing.Size(171, 272);
            this.amtGiftCardAmounts.TabIndex = 5;
            this.amtGiftCardAmounts.UsedCurrencyCode = "";
            this.amtGiftCardAmounts.ViewOption = LSRetailPosis.POSProcesses.WinControls.AmountViewer.ViewOptions.HigherAmounts;
            this.amtGiftCardAmounts.AmountChanged += new LSRetailPosis.POSProcesses.WinControls.AmountViewer.OutputChanged(this.amtGiftCardAmounts_AmountChanged);
            // 
            // numAmount
            // 
            this.numAmount.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.numAmount.Appearance.Options.UseFont = true;
            this.numAmount.AutoSize = true;
            this.numAmount.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.numAmount.CurrencyCode = null;
            this.numAmount.Dock = System.Windows.Forms.DockStyle.Fill;
            this.numAmount.EnteredQuantity = new decimal(new int[] {
            0,
            0,
            0,
            0});
            this.numAmount.EnteredValue = "";
            this.numAmount.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price;
            this.numAmount.Location = new System.Drawing.Point(3, 102);
            this.numAmount.MaskChar = "";
            this.numAmount.MaskInterval = 0;
            this.numAmount.MaxNumberOfDigits = 9;
            this.numAmount.MinimumSize = new System.Drawing.Size(248, 314);
            this.numAmount.Name = "numAmount";
            this.numAmount.NegativeMode = false;
            this.numAmount.NoOfTries = 0;
            this.numAmount.NumberOfDecimals = 2;
            this.numAmount.Padding = new System.Windows.Forms.Padding(0, 10, 0, 0);
            this.numAmount.PromptText = "Enter Amount:";
            this.numAmount.ShortcutKeysActive = false;
            this.numAmount.Size = new System.Drawing.Size(248, 316);
            this.numAmount.TabIndex = 3;
            this.numAmount.TimerEnabled = false;
            this.numAmount.EnterButtonPressed += new LSRetailPosis.POSProcesses.WinControls.NumPad.enterbuttonDelegate(this.numPad_EnterButtonPressed);
            // 
            // lblGiftCardBalanceTitle
            // 
            this.lblGiftCardBalanceTitle.AutoSize = true;
            this.lblGiftCardBalanceTitle.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblGiftCardBalanceTitle.Location = new System.Drawing.Point(5, 71);
            this.lblGiftCardBalanceTitle.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.lblGiftCardBalanceTitle.Name = "lblGiftCardBalanceTitle";
            this.lblGiftCardBalanceTitle.Size = new System.Drawing.Size(82, 25);
            this.lblGiftCardBalanceTitle.TabIndex = 6;
            this.lblGiftCardBalanceTitle.Text = "Balance:";
            // 
            // btnGet
            // 
            this.btnGet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnGet.Appearance.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnGet.Appearance.Options.UseFont = true;
            this.btnGet.Image = ((System.Drawing.Image)(resources.GetObject("btnGet.Image")));
            this.btnGet.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
            this.btnGet.Location = new System.Drawing.Point(260, 30);
            this.btnGet.Margin = new System.Windows.Forms.Padding(6);
            this.btnGet.Name = "btnGet";
            this.btnGet.Size = new System.Drawing.Size(57, 32);
            this.btnGet.TabIndex = 2;
            this.btnGet.Click += new System.EventHandler(this.btnValidateGiftCard_Click);
            // 
            // lblGiftCardBalance
            // 
            this.lblGiftCardBalance.AutoSize = true;
            this.lblGiftCardBalance.Font = new System.Drawing.Font("Segoe UI", 14F);
            this.lblGiftCardBalance.Location = new System.Drawing.Point(259, 71);
            this.lblGiftCardBalance.Margin = new System.Windows.Forms.Padding(5, 3, 3, 3);
            this.lblGiftCardBalance.Name = "lblGiftCardBalance";
            this.lblGiftCardBalance.Size = new System.Drawing.Size(162, 25);
            this.lblGiftCardBalance.TabIndex = 7;
            this.lblGiftCardBalance.Text = "(Balance Amount)";
            // 
            // padGiftCardNumber
            // 
            this.padGiftCardNumber.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.padGiftCardNumber.Appearance.Options.UseFont = true;
            this.padGiftCardNumber.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.padGiftCardNumber.Dock = System.Windows.Forms.DockStyle.Fill;
            this.padGiftCardNumber.EnteredValue = "";
            this.padGiftCardNumber.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.StringPadEntryTypes.None;
            this.padGiftCardNumber.Location = new System.Drawing.Point(3, 3);
            this.padGiftCardNumber.MaskChar = "";
            this.padGiftCardNumber.MaskInterval = 0;
            this.padGiftCardNumber.MaxNumberOfDigits = 30;
            this.padGiftCardNumber.Name = "padGiftCardNumber";
            this.padGiftCardNumber.NegativeMode = false;
            this.padGiftCardNumber.NumberOfDecimals = 0;
            this.padGiftCardNumber.ShortcutKeysActive = false;
            this.padGiftCardNumber.Size = new System.Drawing.Size(248, 62);
            this.padGiftCardNumber.TabIndex = 0;
            this.padGiftCardNumber.TimerEnabled = true;
            this.padGiftCardNumber.EnterButtonPressed += new LSRetailPosis.POSProcesses.WinControls.StringPad.enterbuttonDelegate(this.padGiftCardId_EnterButtonPressed);
            this.padGiftCardNumber.CardSwept += new LSRetailPosis.POSProcesses.WinControls.StringPad.cardSwipedDelegate(this.ProcessSwipedCard);
            // 
            // lblHeading
            // 
            this.lblHeading.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.lblHeading.AutoSize = true;
            this.lblHeading.Font = new System.Drawing.Font("Segoe UI Light", 36F);
            this.lblHeading.Location = new System.Drawing.Point(390, 40);
            this.lblHeading.Margin = new System.Windows.Forms.Padding(0);
            this.lblHeading.Name = "lblHeading";
            this.lblHeading.Padding = new System.Windows.Forms.Padding(0, 0, 0, 30);
            this.lblHeading.Size = new System.Drawing.Size(207, 95);
            this.lblHeading.TabIndex = 0;
            this.lblHeading.Tag = "";
            this.lblHeading.Text = "Gift Card";
            this.lblHeading.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // GiftCardForm
            // 
            this.Appearance.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Appearance.Options.UseFont = true;
            this.ClientSize = new System.Drawing.Size(988, 772);
            this.Controls.Add(this.tableLayoutPanel3);
            this.Controls.Add(this.panelControl1);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "GiftCardForm";
            this.Text = "GiftCardForm";
            this.Controls.SetChildIndex(this.panelControl1, 0);
            this.Controls.SetChildIndex(this.tableLayoutPanel3, 0);
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
            this.tableLayoutPanel3.ResumeLayout(false);
            this.tableLayoutPanel3.PerformLayout();
            this.tableLayoutPanel6.ResumeLayout(false);
            this.tableLayoutPanel4.ResumeLayout(false);
            this.tableLayoutPanel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.panelAmount)).EndInit();
            this.panelAmount.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private DevExpress.XtraEditors.PanelControl panelControl1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel3;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel6;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnOk;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel4;
        private LSRetailPosis.POSProcesses.WinControls.NumPad numAmount;
        private System.Windows.Forms.Label lblHeading;
        private System.Windows.Forms.Label lblGiftCardBalanceTitle;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnGet;
        private System.Windows.Forms.Label lblGiftCardBalance;
        private DevExpress.XtraEditors.PanelControl panelAmount;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label lblPayAmt;
        private LSRetailPosis.POSProcesses.WinControls.AmountViewer amtGiftCardAmounts;
        public LSRetailPosis.POSProcesses.WinControls.StringPad padGiftCardNumber;
    }
}
