using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using LSRetailPosis.Settings;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Net;

namespace Microsoft.Dynamics.Retail.Pos.Loyalty
{
    public partial class frmLoyaltyOTP : Form
    {
        private static string CustomerId = string.Empty;
        string sCustomerName = "";
        private IApplication application;
        public bool IsValidOTP = false;
        DateTime dtRequestTime;
        int OrigTime = 120;

        string sTransactionId = "";
        Timer tmr = new Timer();
        Timer tmr1 = new Timer();
        internal static IApplication InternalApplication { get; private set; }
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
                InternalApplication = value;
            }
        }
        private enum OTPTransactionType
        {
            None = 0,
            Loaylty = 1,
        }

        public frmLoyaltyOTP(string strCustomerId, string sCustName, string sTransId, DateTime dtReq)
        {
            InitializeComponent();
            btnGetOTP.Enabled = false;
            CustomerId = strCustomerId;
            sCustomerName = sCustName;
            dtRequestTime = dtReq;
            sTransactionId = sTransId;

            OTPGenerationTimer();
            lblTimerShow.Text = "";
            displayTimerStart();

        }

        private void OTPGenerationTimer()
        {
            tmr.Interval = 1000 * 60 * 2;
            tmr.Enabled = true;
            tmr.Tick += new System.EventHandler(OnTimerEvent);
            tmr.Start();
        }

        private void displayTimerStart()
        {
            tmr1.Interval = 1000;
            tmr1.Enabled = true;
            tmr1.Tick += new System.EventHandler(tmr1_Tick);
            tmr1.Start();
        }


        void tmr1_Tick(object sender, EventArgs e)
        {
            OrigTime--;
            lblTimerShow.Text = "Re-send button will be active after " + OrigTime / 60 + ":" + ((OrigTime % 60) >= 10 ? (OrigTime % 60).ToString() : "0" + OrigTime % 60);

            if (btnGetOTP.Enabled)
            {
                tmr1.Stop();
                tmr.Stop();
            }
        }

        private void OnTimerEvent(object sender, EventArgs e)
        {
            btnGetOTP.Enabled = true;
            OTPCancel(CustomerId, sTransactionId);
           
            tmr.Stop();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            string strOTP = txtOTP.Text;
            OTPCancel(CustomerId, sTransactionId);
            MessageBox.Show("OTP Time Expired!");
            IsValidOTP = false;
            Dispose();
            Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            string strOTP = txtOTP.Text;

            if (string.IsNullOrEmpty(strOTP))
            {
                MessageBox.Show("OTP Cannot be left Blank!");
                IsValidOTP = false;
            }
            else
            {
                if (ValidateOTP(CustomerId, sTransactionId, Convert.ToInt16(strOTP)))
                {
                    IsValidOTP = true;
                    var a = dtRequestTime;
                    var b = DateTime.Now;
                    if (Double.Parse((b - a).TotalSeconds.ToString()) > 120.00)//2 min
                    {
                        IsValidOTP = false;
                        OTPCancel(CustomerId, sTransactionId);
                        MessageBox.Show("OTP Time Expired!");
                    }
                    Dispose();
                    Close();
                }
                else
                {
                    IsValidOTP = false;
                    MessageBox.Show("Invalid OTP!");
                }

                OTPUsed(CustomerId, sTransactionId, Convert.ToInt16(strOTP));
                this.Close();
            }

        }

        private void OTPCancel(string CustomerId, string sTransId)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " UPDATE CRWOTPTransTable SET OTPCanceled=1 WHERE CUSTACCOUNT = '" + CustomerId + "'" +
                                 " and TransactionId='" + sTransId + "'" +
                                 " and RETAILTERMINALID='" + ApplicationSettings.Database.TerminalID + "'" +
                                 " and Transtype=" + (int)OTPTransactionType.Loaylty + "";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.ExecuteNonQuery();

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        private void OTPUsed(string CustomerId, string sTransId, int iLoyaltyOTP)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            string commandText = " UPDATE CRWOTPTransTable SET OTPUsed=1 WHERE CUSTACCOUNT = '" + CustomerId + "'" +
                " and TRANSACTIONOTP=" + iLoyaltyOTP + " and TransactionId='" + sTransId + "'" +
                " and RETAILTERMINALID='" + ApplicationSettings.Database.TerminalID + "'" +
                " and Transtype=" + (int)OTPTransactionType.Loaylty + "";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.ExecuteNonQuery();

            if (connection.State == ConnectionState.Open)
                connection.Close();
        }

        private bool ValidateOTP(string CustomerId, string sTransId, int iLoyaltyOTP)
        {
            bool bReturn = false;
            int iValue1 = 0;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "SELECT TRANSACTIONOTP " +
                " FROM CRWOTPTransTable where CUSTACCOUNT= '" + CustomerId + "' and TransactionId='" + sTransId + "'" +
                " and RETAILTERMINALID='" + ApplicationSettings.Database.TerminalID + "'" +
                " and Transtype=" + (int)OTPTransactionType.Loaylty + " and OTPUsed=0 and OTPCanceled=0";

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            iValue1 = Convert.ToInt16(command.ExecuteScalar());

            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (iValue1 == iLoyaltyOTP)
                bReturn = true;
            else
                bReturn = false;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bReturn;
        }

        private void btnGetOTP_Click(object sender, EventArgs e)
        {
            string sMNo = getCustomerMobilePrimary(CustomerId);
            string sTextSMS = "";
            int iOTP = GenerateRandomNo();
            dtRequestTime=DateTime.Now;

            sTextSMS = "Dear " + sCustomerName + " Your OTP to authorize loyalty point redemption at PNG Jewellers Store is " + iOTP + ". this OTP will be valid for 10 minutes";

            SendSMS(sTextSMS, sMNo);
            SaveLoyaltyOTPInfo(sTransactionId, Convert.ToString(iOTP), CustomerId);
            MessageBox.Show("The resend OTP is : " + iOTP + "");
            btnGetOTP.Enabled = false;
            OrigTime = 120;
            displayTimerStart();
            OTPGenerationTimer();
        }


        private string getCustomerMobilePrimary(string sCustAcc)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select RETAILMOBILEPRIMARY from CUSTTABLE A ");
            commandText.Append(" where ACCOUNTNUM ='" + sCustAcc + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "";
        }

        public int GenerateRandomNo()
        {
            int _min = 1000;
            int _max = 9999;
            Random _rdm = new Random();
            return _rdm.Next(_min, _max);
        }

        public void SendSMS(string sTextSMS, string sMNo)
        {

            string _webURL = "";// "http://bulksmspune.mobi/sendurlcomma.asp?user=";
            string _userId = "";//"20064043";
            string _password = "";// "smst2018";

            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT ISNULL(PASSWORD,''),isnull(USERID,''),isnull(WEBURL,'') FROM CRWSMSSETUP ");
            commandText.Append(" where ISACTIVE=1 AND DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command1 = new SqlCommand(commandText.ToString(), conn);
            SqlDataReader reader1 = command1.ExecuteReader();

            if (reader1.HasRows)
            {
                while (reader1.Read())
                {
                    _password = Convert.ToString(reader1.GetValue(0));
                    _userId = Convert.ToString(reader1.GetValue(1));
                    _webURL = Convert.ToString(reader1.GetValue(2));
                }
            }
            command1.CommandTimeout = 0;
            reader1.Dispose();

            // http://bulksmspune.mobi/sendurlcomma.asp?user=20064043&pwd=smst2018&smstype=3&mobileno=9850991010&msgtext=Hello

            string sSMSAPI = _webURL + _userId + "&pwd=" + _password + "&smstype=3&mobileno=" + sMNo + "&msgtext=" + sTextSMS + "";

            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(sSMSAPI);
            HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
            System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
            string responseString = respStreamReader.ReadToEnd();

            respStreamReader.Close();
            myResp.Close();

        }

        private void SaveLoyaltyOTPInfo(string sTransId, string sOTP, string sCustId)
        {
            string commandText = " INSERT INTO [CRWOTPTransTable] " +
                                   " ([TransactionId],[TransType],[TransactionOTP]," +
                                   " [CustAccount],[TransDate],[RetailStaffId],[RetailStoreId] " +
                                   " ,RetailTerminalId,[DATAAREAID],[OTPUsed],[OTPCanceled]) " +
                                   "  VALUES " +
                                   " (@TransactionId,@TransType,@TransactionOTP,@CustAccount," +
                                   "  @TransDate,@RetailStaffId,@RetailStoreId,@RetailTerminalId,@DATAAREAID " +
                                   " ,@OTPUsed,@OTPCanceled) ";

            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(commandText, conn))
            {
                command.Parameters.Add("@TransactionId", SqlDbType.NVarChar, 20).Value = sTransId;
                command.Parameters.Add("@TransType", SqlDbType.Int).Value = (int)OTPTransactionType.Loaylty;
                command.Parameters.Add("@TransactionOTP", SqlDbType.NVarChar, 20).Value = sOTP;
                command.Parameters.Add("@CustAccount", SqlDbType.NVarChar, 20).Value = sCustId;
                command.Parameters.Add("@TransDate", SqlDbType.Date).Value = Convert.ToDateTime(DateTime.Now).Date;
                command.Parameters.Add("@RetailStaffId", SqlDbType.NVarChar, 20).Value = Convert.ToString(ApplicationSettings.Terminal.TerminalOperator.OperatorId);
                command.Parameters.Add("@RetailStoreId", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@RetailTerminalId", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.TerminalId;
                command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                command.Parameters.Add("@OTPUsed", SqlDbType.Int).Value = 0;
                command.Parameters.Add("@OTPCanceled", SqlDbType.Int).Value = 0;

                command.ExecuteNonQuery();
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

        }
    }
}
