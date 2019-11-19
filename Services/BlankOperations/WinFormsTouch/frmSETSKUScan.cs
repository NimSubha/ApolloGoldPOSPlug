using System;
using System.Data;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmSETSKUScan:Form
    {
        SqlConnection conn = new SqlConnection();
        public IPosTransaction pos { get; set; }

        [Import]
        private IApplication application;
       
        public DataTable dtSETSku = new DataTable(); 

        public frmSETSKUScan()
        {
            InitializeComponent();
        }
        public frmSETSKUScan(IApplication Application)
        {
            InitializeComponent();

            application = Application;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnClearProduct_Click(object sender, EventArgs e)
        {
            txtItemId.Text = "";
            txtItemId.Focus();
        }

        private void btnPOSItemSearch_Click(object sender, EventArgs e)
        {
            DataTable dtProduct = new DataTable();
            DataRow drProduct = null;
            SqlConnection conn = new SqlConnection();
            conn = application.Settings.Database.Connection;
            if(conn.State == ConnectionState.Closed)
                conn.Open();
            string commandText = string.Empty;

            commandText = "SELECT isnull(SETSKUNUMBER,'') SETSKUNUMBER FROM SETSKUTABLEPOSTED WHERE DEACTIVATE=0";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter adapter = new SqlDataAdapter(commandText, conn);
            adapter.Fill(dtProduct);

            if(conn.State == ConnectionState.Open)
                conn.Close();

            Dialog.WinFormsTouch.frmGenericSearch oSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtProduct, drProduct = null, "Product Search");
            oSearch.ShowDialog();
            drProduct = oSearch.SelectedDataRow;

            if(drProduct != null)
            {
                txtItemId.Text = string.Empty;
                txtItemId.Text = Convert.ToString(drProduct["SETSKUNUMBER"]);
            }
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            if(IsSETSKU(txtItemId.Text))
            {
                try
                {
                    SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                    SqlCon.Open();

                    SqlCommand SqlComm = new SqlCommand();
                    SqlComm.Connection = SqlCon;
                    SqlComm.CommandType = CommandType.Text;
                    SqlComm.CommandText = "select isnull(skunumber,'') skunumber from SETSKULINEPOSTED " +
                                          " WHERE SETSKUNUMBER='" + txtItemId.Text.Trim() + "'";

                    SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                    SqlDa.Fill(dtSETSku);

                    this.Close();
                }
                catch(Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Invalid SET SKU");
            }

        }

        private bool IsSETSKU(string sItemId)
        {
            bool bSetItem = false;

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "SELECT SETSKUNUMBER FROM SETSKUTABLEPOSTED WHERE SETSKUNUMBER = '" + sItemId + "' and DEACTIVATE=0";


            if(connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            string sSetItem = Convert.ToString(command.ExecuteScalar());
            if(connection.State == ConnectionState.Open)
                connection.Close();

            if(!string.IsNullOrEmpty(sSetItem))
                bSetItem = true;


            return bSetItem;
        }
    }
}
