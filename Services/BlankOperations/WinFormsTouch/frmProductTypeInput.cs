using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;
using System.Reflection;
using Microsoft.Dynamics.Retail.Pos.Dialog;
using Microsoft.Reporting.WinForms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmProductTypeInput : Form
    {

        public string sProductType = "";
        [Import]
        private IApplication application;


        public frmProductTypeInput()
        {
            InitializeComponent();
            LoadProductType();
        }

        public frmProductTypeInput(DataTable dtPType)
        {
            InitializeComponent();
            LoadProductType(dtPType);
        }

        private void LoadProductType(DataTable dt)
        {
            if (dt.Rows.Count > 0)
            {
                cmbProductType.DataSource = dt;
                cmbProductType.DisplayMember = "PRODUCTTYPECODE";
                cmbProductType.ValueMember = "PRODUCTTYPECODE";
            }
        }

        private void LoadProductType()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine(" select DISTINCT PRODUCTTYPECODE from INVENTTABLE where PRODUCTTYPECODE!=''");

            DataTable dtProductType = new DataTable();
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            using (SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        dtProductType.Load(reader);
                    }
                    reader.Close();
                    reader.Dispose();
                }
                cmd.Dispose();
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (dtProductType.Rows.Count > 0)
            {
                cmbProductType.DataSource = dtProductType;
                cmbProductType.DisplayMember = "PRODUCTTYPECODE";
                cmbProductType.ValueMember = "PRODUCTTYPECODE";
            }
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(cmbProductType.Text.Trim()))
            {
                sProductType = Convert.ToString(cmbProductType.Text.Trim());
                this.Close();
            }
            else
            {
                MessageBox.Show("Please select product type.");
                cmbProductType.Focus();
            }
        }
    }
}
