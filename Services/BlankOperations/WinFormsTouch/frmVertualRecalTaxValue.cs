using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmVertualRecalTaxValue : frmTouchBase
    {
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnCancel;
        private LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx btnOK;
        private Panel panel1;
        private TextBox txtTotal;
        private Label label1;
        private Label lblMonths;
        private TextBox txtTaxAmt;
        private Label lblGSSNo;
        private TextBox txtDiscount;
        private Label label2;
        private TextBox txtExpFinalAmt;
        private Label label3;
        private TextBox txtNetAmount;
        public decimal dDiscAmt = 0m;
        public bool isCancel = false;


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

        public frmVertualRecalTaxValue()
        {
            InitializeComponent();
        }


        public frmVertualRecalTaxValue(decimal dVTotNetAmt, decimal dVTotGrossAmt, decimal dVTotTaxAmt, IApplication Application)
        {
            InitializeComponent();
            txtNetAmount.Text = Convert.ToString(decimal.Round(dVTotNetAmt, 2, MidpointRounding.AwayFromZero));
            txtTotal.Text = Convert.ToString(decimal.Round(dVTotGrossAmt, 2, MidpointRounding.AwayFromZero));
            txtTaxAmt.Text = Convert.ToString(decimal.Round(dVTotTaxAmt, 2, MidpointRounding.AwayFromZero));

            application = Application;
        }

        private void InitializeComponent()
        {
            this.btnCancel = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.btnOK = new LSRetailPosis.POSProcesses.WinControls.SimpleButtonEx();
            this.panel1 = new System.Windows.Forms.Panel();
            this.txtTotal = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMonths = new System.Windows.Forms.Label();
            this.txtTaxAmt = new System.Windows.Forms.TextBox();
            this.lblGSSNo = new System.Windows.Forms.Label();
            this.txtNetAmount = new System.Windows.Forms.TextBox();
            this.txtDiscount = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtExpFinalAmt = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnCancel.Appearance.Options.UseFont = true;
            this.btnCancel.Location = new System.Drawing.Point(203, 222);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Padding = new System.Windows.Forms.Padding(0);
            this.btnCancel.ShowToolTips = false;
            this.btnCancel.Size = new System.Drawing.Size(112, 45);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Tag = "btnCancel";
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Appearance.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnOK.Appearance.Options.UseFont = true;
            this.btnOK.Location = new System.Drawing.Point(87, 222);
            this.btnOK.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.btnOK.Name = "btnOK";
            this.btnOK.Padding = new System.Windows.Forms.Padding(0);
            this.btnOK.ShowToolTips = false;
            this.btnOK.Size = new System.Drawing.Size(106, 45);
            this.btnOK.TabIndex = 1;
            this.btnOK.Tag = "btnOK";
            this.btnOK.Text = "OK";
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // panel1
            // 
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel1.Controls.Add(this.txtTotal);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.lblMonths);
            this.panel1.Controls.Add(this.txtTaxAmt);
            this.panel1.Controls.Add(this.lblGSSNo);
            this.panel1.Controls.Add(this.txtNetAmount);
            this.panel1.Location = new System.Drawing.Point(7, 6);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(308, 123);
            this.panel1.TabIndex = 4;
            // 
            // txtTotal
            // 
            this.txtTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTotal.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtTotal.Enabled = false;
            this.txtTotal.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtTotal.Location = new System.Drawing.Point(129, 77);
            this.txtTotal.MaxLength = 10;
            this.txtTotal.Name = "txtTotal";
            this.txtTotal.Size = new System.Drawing.Size(159, 27);
            this.txtTotal.TabIndex = 160;
            this.txtTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Bold);
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(6, 79);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(45, 21);
            this.label1.TabIndex = 159;
            this.label1.Text = "Total";
            // 
            // lblMonths
            // 
            this.lblMonths.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMonths.AutoSize = true;
            this.lblMonths.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Bold);
            this.lblMonths.ForeColor = System.Drawing.Color.Black;
            this.lblMonths.Location = new System.Drawing.Point(6, 46);
            this.lblMonths.Name = "lblMonths";
            this.lblMonths.Size = new System.Drawing.Size(99, 21);
            this.lblMonths.TabIndex = 158;
            this.lblMonths.Text = "Tax Amount";
            // 
            // txtTaxAmt
            // 
            this.txtTaxAmt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtTaxAmt.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtTaxAmt.Enabled = false;
            this.txtTaxAmt.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtTaxAmt.Location = new System.Drawing.Point(129, 44);
            this.txtTaxAmt.MaxLength = 4;
            this.txtTaxAmt.Name = "txtTaxAmt";
            this.txtTaxAmt.Size = new System.Drawing.Size(159, 27);
            this.txtTaxAmt.TabIndex = 157;
            this.txtTaxAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // lblGSSNo
            // 
            this.lblGSSNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.lblGSSNo.AutoSize = true;
            this.lblGSSNo.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Bold);
            this.lblGSSNo.ForeColor = System.Drawing.Color.Black;
            this.lblGSSNo.Location = new System.Drawing.Point(6, 13);
            this.lblGSSNo.Name = "lblGSSNo";
            this.lblGSSNo.Size = new System.Drawing.Size(103, 21);
            this.lblGSSNo.TabIndex = 156;
            this.lblGSSNo.Text = "Net Amount";
            // 
            // txtNetAmount
            // 
            this.txtNetAmount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtNetAmount.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtNetAmount.Enabled = false;
            this.txtNetAmount.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtNetAmount.Location = new System.Drawing.Point(129, 11);
            this.txtNetAmount.MaxLength = 50;
            this.txtNetAmount.Name = "txtNetAmount";
            this.txtNetAmount.Size = new System.Drawing.Size(159, 27);
            this.txtNetAmount.TabIndex = 2;
            this.txtNetAmount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // txtDiscount
            // 
            this.txtDiscount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDiscount.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtDiscount.Location = new System.Drawing.Point(165, 135);
            this.txtDiscount.MaxLength = 10;
            this.txtDiscount.Name = "txtDiscount";
            this.txtDiscount.Size = new System.Drawing.Size(131, 27);
            this.txtDiscount.TabIndex = 0;
            this.txtDiscount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDiscount.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDiscount_KeyPress);
            this.txtDiscount.Leave += new System.EventHandler(this.txtDiscount_Leave);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Bold);
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(14, 137);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 21);
            this.label2.TabIndex = 161;
            this.label2.Text = "Discount";
            // 
            // txtExpFinalAmt
            // 
            this.txtExpFinalAmt.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtExpFinalAmt.BackColor = System.Drawing.SystemColors.InactiveBorder;
            this.txtExpFinalAmt.Enabled = false;
            this.txtExpFinalAmt.Font = new System.Drawing.Font("Segoe UI Semibold", 11.25F, System.Drawing.FontStyle.Bold);
            this.txtExpFinalAmt.Location = new System.Drawing.Point(165, 168);
            this.txtExpFinalAmt.MaxLength = 10;
            this.txtExpFinalAmt.Name = "txtExpFinalAmt";
            this.txtExpFinalAmt.Size = new System.Drawing.Size(131, 27);
            this.txtExpFinalAmt.TabIndex = 164;
            this.txtExpFinalAmt.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Segoe UI Light", 12F, System.Drawing.FontStyle.Bold);
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(14, 170);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(148, 21);
            this.label3.TabIndex = 163;
            this.label3.Text = "Exp. Final Amount";
            // 
            // frmVertualRecalTaxValue
            // 
            this.ClientSize = new System.Drawing.Size(324, 276);
            this.ControlBox = false;
            this.Controls.Add(this.txtExpFinalAmt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtDiscount);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.panel1);
            this.LookAndFeel.SkinName = "Money Twins";
            this.Name = "frmVertualRecalTaxValue";
            ((System.ComponentModel.ISupportInitialize)(this.styleController)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            decimal dMaxAllowedAmt = getMaxDiscAmt();
            if (!string.IsNullOrEmpty(txtDiscount.Text))
            {
                dDiscAmt = Convert.ToDecimal(txtDiscount.Text);
            }
            if (dDiscAmt > dMaxAllowedAmt)
            {
                MessageBox.Show("The discount amount is too high.The discount limit is " + ApplicationSettings.Terminal.StoreCurrency + "" + dMaxAllowedAmt);
                dDiscAmt = 0m;
            }
            else
            {
                this.Close();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            isCancel = true;
            this.Close();
        }

        private void txtDiscount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtDiscount.Text == string.Empty && e.KeyChar == '.')
            {
                e.Handled = true;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    e.Handled = true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    e.Handled = true;
                }

                //if (Regex.IsMatch(txtDiscount.Text, @"\.\d\d"))
                //{
                //    e.Handled = true;
                //}

                //if (e.KeyChar == (Char)Keys.Enter)
                //{
                //    e.Handled = true;
                //}
            }
        }

        private void txtDiscount_Leave(object sender, EventArgs e)
        {
            decimal dDiscA = 0m;
            decimal dTotAmt = 0m;
            if (!string.IsNullOrEmpty(txtDiscount.Text))
            {
                dDiscA = Convert.ToDecimal(txtDiscount.Text);
            }

            if (!string.IsNullOrEmpty(txtTotal.Text))
            {
                dTotAmt = Convert.ToDecimal(txtTotal.Text);
            }

            txtExpFinalAmt.Text = Convert.ToString(decimal.Round(dTotAmt - dDiscA, 2, MidpointRounding.AwayFromZero));
        }

        private decimal getMaxDiscAmt()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append(" select CAST(ISNULL(PP.MAXTOTALDISCOUNTAMOUNT,0)AS DECIMAL(28,2))  from RETAILSTAFFTABLE r");
            sbQuery.Append(" left join dbo.HCMWORKER as h on h.PERSONNELNUMBER = r.STAFFID");
            sbQuery.Append(" left join HCMPOSITIONWORKERASSIGNMENT PA on pA.WORKER=h.RECID");
            sbQuery.Append(" left join RETAILPOSITIONPOSPERMISSION PP on PP.POSITION=PA.POSITION");
            sbQuery.Append(" where STAFFID='" + ApplicationSettings.Terminal.TerminalOperator.OperatorId + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);

            int iResult = 0;
            iResult = Convert.ToInt16(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return iResult;
        }
    }
}
