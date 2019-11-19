using System;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmOptionSelectionExchangeBuyback : frmTouchBase //Form
    {
        public bool isExchange = false;
        public bool isFullReturn = false;
        public bool isCancel = false;
        public bool isCashBack = false;
        public bool isConvertToAdv = false;
        bool bTaxApplicable = false;

        private IApplication application;

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
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

        enum CRWRetailDiscPermission // added on 060917
        {
            Cashier = 0,
            Salesperson = 1,
            Manager = 2,
            Other = 3,
            Manager2 = 4,
        }

        public frmOptionSelectionExchangeBuyback()
        {
            InitializeComponent();
            int iSPId = 0;
            iSPId = getUserDiscountPermissionId();
            bTaxApplicable = IsTaxApplicable();

            if (Convert.ToInt16(iSPId) != (int)CRWRetailDiscPermission.Manager2)
                btnExchangeFull.Visible = false;
            else
                btnExchangeFull.Visible = true;

            //if (bTaxApplicable)
            //    btnExchange.Visible = false;
            //else
            //    btnExchange.Visible = true;
        }

        private int getUserDiscountPermissionId()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select RETAILDISCPERMISSION from RETAILSTAFFTABLE where STAFFID='" + ApplicationSettings.Terminal.TerminalOperator.OperatorId + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);

            int iResult = 0;
            iResult = Convert.ToInt16(cmd.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return iResult;
        }

        private void btnExchange_Click(object sender, EventArgs e)
        {
            string sTblName = "NEGCASHPAY" + ApplicationSettings.Terminal.TerminalId;
            TempTableCreate(sTblName);
            UpdateNegCashIfo(sTblName, ApplicationSettings.Terminal.TerminalId, 0, 1);
            isExchange = true;
            this.Close();
        }

        private void btnExchangeFull_Click(object sender, EventArgs e)
        {
            isFullReturn = true;
            isCashBack = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            isCancel = true;
            this.Close();
        }

        private void btnCovertToAdv_Click(object sender, EventArgs e)
        {
            string sTblName = "NEGCASHPAY" + ApplicationSettings.Terminal.TerminalId;
            TempTableCreate(sTblName);
            UpdateNegCashIfo(sTblName, ApplicationSettings.Terminal.TerminalId, 0, 1);
            isFullReturn = true;
            isConvertToAdv = true;
            this.Close();
        }

        private void TempTableCreate(string sTableName)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();


            commandText.Append("SET TRANSACTION ISOLATION LEVEL SNAPSHOT ;IF NOT EXISTS (SELECT A.NAME FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN CREATE TABLE " + sTableName + "(");
            commandText.Append(" TERMINALID NVARCHAR (20),NEGAMT NUMERIC (20,2),SRCASH int,IsTransAmt INT NOT NULL DEFAULT 0) END");

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();

            if(conn.State == ConnectionState.Open)
                conn.Close();

        }

        private void UpdateNegCashIfo(string sTableName, string sTerminalId, decimal dEmpAmt,int iSRCashPay)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();


            commandText.Append("IF EXISTS (SELECT A.NAME FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            commandText.Append(" BEGIN  INSERT INTO  " + sTableName + "  VALUES('" + sTerminalId + "',");
            commandText.Append(" '" + dEmpAmt + "'," + iSRCashPay + ",1) END");


            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            command.ExecuteNonQuery();
            if(conn.State == ConnectionState.Open)
                conn.Close();

        }

        private bool IsTaxApplicable()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" select TAXAPPLICABLE from RETAILPARAMETERS where  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            bool sResult = Convert.ToBoolean(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sResult;

        }
    }
}
