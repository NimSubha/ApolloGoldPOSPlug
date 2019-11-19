namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    partial class OrderDetailsNavigationBar
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
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.labelPurchase = new System.Windows.Forms.LinkLabel();
            this.labelSale = new System.Windows.Forms.LinkLabel();
            this.labelExchange = new System.Windows.Forms.LinkLabel();
            this.labelPurchaseReturn = new System.Windows.Forms.LinkLabel();
            this.labelExchangeReturn = new System.Windows.Forms.LinkLabel();
            this.bindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Controls.Add(this.labelPurchase, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.labelSale, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.labelExchange, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.labelPurchaseReturn, 1, 3);
            this.tableLayoutPanel.Controls.Add(this.labelExchangeReturn, 1, 4);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 6;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(314, 267);
            this.tableLayoutPanel.TabIndex = 0;
            this.tableLayoutPanel.CellPaint += new System.Windows.Forms.TableLayoutCellPaintEventHandler(this.OnTableLayoutPanel_CellPaint);
            // 
            // labelPurchase
            // 
            this.labelPurchase.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelPurchase.AutoSize = true;
            this.labelPurchase.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.labelPurchase.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.labelPurchase.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            this.labelPurchase.Location = new System.Drawing.Point(33, 55);
            this.labelPurchase.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.labelPurchase.Name = "labelPurchase";
            this.labelPurchase.Size = new System.Drawing.Size(92, 25);
            this.labelPurchase.TabIndex = 1;
            this.labelPurchase.TabStop = true;
            this.labelPurchase.Text = "Purchase";
            this.labelPurchase.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            // 
            // labelSale
            // 
            this.labelSale.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelSale.AutoSize = true;
            this.labelSale.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.labelSale.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.labelSale.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            this.labelSale.Location = new System.Drawing.Point(33, 10);
            this.labelSale.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.labelSale.Name = "labelSale";
            this.labelSale.Size = new System.Drawing.Size(48, 25);
            this.labelSale.TabIndex = 0;
            this.labelSale.TabStop = true;
            this.labelSale.Text = "Sale";
            this.labelSale.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            // 
            // labelExchange
            // 
            this.labelExchange.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelExchange.AutoSize = true;
            this.labelExchange.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.labelExchange.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.labelExchange.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            this.labelExchange.Location = new System.Drawing.Point(33, 100);
            this.labelExchange.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.labelExchange.Name = "labelExchange";
            this.labelExchange.Size = new System.Drawing.Size(97, 25);
            this.labelExchange.TabIndex = 2;
            this.labelExchange.TabStop = true;
            this.labelExchange.Text = "Exchange";
            this.labelExchange.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            // 
            // labelPurchaseReturn
            // 
            this.labelPurchaseReturn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelPurchaseReturn.AutoSize = true;
            this.labelPurchaseReturn.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.labelPurchaseReturn.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.labelPurchaseReturn.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            this.labelPurchaseReturn.Location = new System.Drawing.Point(33, 145);
            this.labelPurchaseReturn.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.labelPurchaseReturn.Name = "labelPurchaseReturn";
            this.labelPurchaseReturn.Size = new System.Drawing.Size(158, 25);
            this.labelPurchaseReturn.TabIndex = 3;
            this.labelPurchaseReturn.TabStop = true;
            this.labelPurchaseReturn.Text = "Purchase Return";
            this.labelPurchaseReturn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            // 
            // labelExchangeReturn
            // 
            this.labelExchangeReturn.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.labelExchangeReturn.AutoSize = true;
            this.labelExchangeReturn.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold);
            this.labelExchangeReturn.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
            this.labelExchangeReturn.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(32)))), ((int)(((byte)(31)))), ((int)(((byte)(53)))));
            this.labelExchangeReturn.Location = new System.Drawing.Point(33, 190);
            this.labelExchangeReturn.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.labelExchangeReturn.Name = "labelExchangeReturn";
            this.labelExchangeReturn.Size = new System.Drawing.Size(163, 25);
            this.labelExchangeReturn.TabIndex = 4;
            this.labelExchangeReturn.TabStop = true;
            this.labelExchangeReturn.Text = "Exchange Return";
            this.labelExchangeReturn.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnLinkClicked);
            // 
            // bindingSource
            // 
            this.bindingSource.DataSource = typeof(Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch.OrderDetailsViewModel);
            // 
            // OrderDetailsNavigationBar
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
            this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Controls.Add(this.tableLayoutPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "OrderDetailsNavigationBar";
            this.Size = new System.Drawing.Size(314, 267);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.LinkLabel labelPurchase;
        private System.Windows.Forms.LinkLabel labelPurchaseReturn;
        private System.Windows.Forms.LinkLabel labelExchangeReturn;
        private System.Windows.Forms.LinkLabel labelExchange;
        private System.Windows.Forms.LinkLabel labelSale;
        private System.Windows.Forms.BindingSource bindingSource;
    }
}
