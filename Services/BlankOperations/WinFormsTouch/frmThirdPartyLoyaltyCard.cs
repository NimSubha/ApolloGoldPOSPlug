using System;
using System.Data;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmThirdPartyLoyaltyCard:Form
    {
        public string sTypeCode = string.Empty;
        public string sProvider = string.Empty;
        public string sCardNo = string.Empty;
        int iNoOfChar = 0;

        private IApplication application;
        DataTable dtLTCode = new DataTable();


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

        public frmThirdPartyLoyaltyCard(DataTable dtLT)
        {
            InitializeComponent();

            dtLTCode = dtLT;
            cmbTypeCode.DataSource = dtLTCode;
            cmbTypeCode.DisplayMember = "loyaltyTypeCode";
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if(Validate())
            {
                sTypeCode = cmbTypeCode.Text.Trim();
                sProvider = txtLoyaltyProvider.Text;
                sCardNo = txtLoyaltyNo.Text;
                this.Close();
            }
        }

        private bool Validate()
        {
            if(string.IsNullOrEmpty(cmbTypeCode.Text))
            {
                cmbTypeCode.Focus();
                MessageBox.Show("Please sleect type code.", "Please sleect type code.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }            
            if(string.IsNullOrEmpty(txtLoyaltyProvider.Text))
            {
                txtLoyaltyProvider.Focus();
                MessageBox.Show("Please enter provider.", "Please enter provider.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if(string.IsNullOrEmpty(txtLoyaltyNo.Text))
            {
                txtLoyaltyNo.Focus();
                MessageBox.Show("Please enter loyalty no.", "Please enter loyalty no.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if(txtLoyaltyNo.TextLength != iNoOfChar)
            {
                MessageBox.Show("Invalid loyalty card.", "Invalid loyalty card.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            else
                return true;

        }

        private void GetLengthOfLoyaltyNo(string sTypeCode)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            commandText = "select isnull(NoOfChar,0) NoOfChar, isnull(LoyaltyProvider,'') LoyaltyProvider" +
                        " from CRWThirdPartyLoyaltymaster where LOYALTYTYPECODE ='" + sTypeCode + "'";

            if(conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand cmd = new SqlCommand(commandText.ToString(), conn);
            SqlDataReader reader = cmd.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    iNoOfChar = Convert.ToInt16(reader.GetValue(0));
                    sProvider = Convert.ToString(reader.GetValue(1));
                }
            }
            reader.Close();
            reader.Dispose();
        }

        private void cmbTypeCode_SelectedIndexChanged(object sender, EventArgs e)
        {
            GetLengthOfLoyaltyNo(cmbTypeCode.Text);
            txtLoyaltyProvider.Text = sProvider;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }


    }
}
