using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmR_GSSAccStaement : Form

    {
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
            }
        }
        SqlConnection connection;
        DateTime dtTransDate = Convert.ToDateTime("01/01/1900");
        BlankOperations oBlank = new BlankOperations();
        DataSet dsGSSAccStatement;

        string sCustName = "-";
        string sCustId = "-";
        string sCustAddress = "-";
        string sCPinCode = "-";
        string sContactNo = "-";      

        string sSchemePeriod = "";
        string sStoreName = "-";
        string sStoreAddress = "-";
        string sStorePhNo = "...";

        string sInsAmt = "";       
        string sSchemeCode = "";       
        string sTerminal = string.Empty;
        string sOpDate = string.Empty;
        string sCompanyName = string.Empty; //aded on 14/04/2014 R.Hossain
        string sCINNo = string.Empty;//aded on 14/04/2014 R.Hossain
        string sGSSAccNumber = "";
        decimal   dTotAmt = 0;
        string sDueDateOfMonthPay = string.Empty;
        string sAmountInWords = string.Empty;
        string sGSSDueDays = "0";
        string sRPerc = "0";
        string sPurDiscOnMat = "0";
        string sGovtIdInfo = "";
        bool bGSSTaxApplicable = false;

        private enum IdentityProof
        {
            Govt_Id = 0,
            Driving_License = 1,
            Passport_No = 2
        }

        DataTable dtCustom = new DataTable();
       
        public frmR_GSSAccStaement(SqlConnection conn, DataSet dsGSSAccSt, string sGSSAcc,string sCName,string sCAdd,string sCId, string sCContactNo)
        {
            InitializeComponent();


            if (!string.IsNullOrEmpty(Convert.ToString(sCName)))
                sCustName = Convert.ToString(sCName);
            if (!string.IsNullOrEmpty(Convert.ToString(sCId)))
                sCustId = Convert.ToString(sCId);
            if (!string.IsNullOrEmpty(sCAdd)) 
                sCustAddress = Convert.ToString(sCAdd);
            if (!string.IsNullOrEmpty(sCContactNo))
                sContactNo = Convert.ToString(sCContactNo);
            
            if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
            if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
            if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

            sGSSAccNumber = sGSSAcc;
            connection = conn;

            bGSSTaxApplicable = IsGSSEmiInclOfTax(connection);

            if (connection.State == ConnectionState.Closed)
                connection.Open();

           

            GetGSSAccInfo(sGSSAcc, connection);
            sGovtIdInfo = GetCustomerGovtIdInfo(sCustId, connection);

            dsGSSAccStatement = new DataSet();
            dsGSSAccStatement = dsGSSAccSt;

           
            for (int i = 0; i <= dsGSSAccStatement.Tables[0].Rows.Count - 1; i++)
            {
                dTotAmt = dTotAmt + Convert.ToDecimal(dsGSSAccStatement.Tables[0].Rows[i]["Amount"]);
            }

            sAmountInWords =  oBlank.Amtinwds(Convert.ToDouble(Math.Abs(dTotAmt)));

            sCompanyName = oBlank.GetCompanyName(conn);//aded on 14/04/2014 R.Hossain
            sGSSDueDays=Convert.ToString(GSSDueDays());

            GetCustomData(dsGSSAccStatement.Tables[0]);
        }

        private bool IsGSSEmiInclOfTax(SqlConnection conn)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" select isnull(GSSEmiInclOfTax,0) from RETAILPARAMETERS where  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            bool sResult = Convert.ToBoolean(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sResult;

        }

        private string GetCustomerGovtIdInfo(string sCustAcc, SqlConnection conn)
        {
            
            string sResult = "";
            int iGovtId = 0;
            string sGovtIdNo = "";
            string sql = "select GOVTIDENTITY,isnull(GOVTIDNO,'') GOVTIDNO from CUSTTABLE " +
                         " where ACCOUNTNUM='" + sCustAcc + "'";
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            DataTable dtCustInfo = new DataTable();
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandTimeout = 0;
            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtCustInfo);

            if (dtCustInfo != null && dtCustInfo.Rows.Count > 0)
            {
                iGovtId = Convert.ToInt16(dtCustInfo.Rows[0]["GOVTIDENTITY"]);

                if (Convert.ToString(dtCustInfo.Rows[0]["GOVTIDNO"]) == string.Empty)
                    sGovtIdNo = "";
                else
                    sGovtIdNo = Convert.ToString(dtCustInfo.Rows[0]["GOVTIDNO"]);

                if (!string.IsNullOrEmpty(sGovtIdNo))
                {
                    if (iGovtId == (int)IdentityProof.Driving_License)
                        sResult = "Driving License  : " + sGovtIdNo;
                    else if (iGovtId == (int)IdentityProof.Govt_Id)
                        sResult = "Govt. Id  : " + sGovtIdNo;
                    else if (iGovtId == (int)IdentityProof.Passport_No)
                        sResult = "Passport No  : " + sGovtIdNo;
                }
            }

            return sResult;
        }

        private void GetCustomData(DataTable dtFromAx)
        {
            dtCustom = new DataTable("GSS_Statement");
            dtCustom = dtFromAx.Copy();

            dtCustom.Columns.Add("NoOfDaysOfLatePay", typeof(string));
            dtCustom.Columns.Add("Days", typeof(int));
            dtCustom.Columns.Add("AmtExclTax", typeof(decimal)); 
            dtCustom.AcceptChanges();

            for (int i = 0; i <= dtCustom.Rows.Count - 1; i++)
            {
                if (i == 0)
                    dtCustom.Rows[i]["Duedate"] = dtCustom.Rows[i]["Duedate"];
                else
                    dtCustom.Rows[i]["Duedate"] = dtCustom.Rows[i]["Duedate"];// Convert.ToDateTime(dtCustom.Rows[i]["Duedate"]).AddDays(Convert.ToInt32(sGSSDueDays) * (i)).ToString("dd-MM-yyyy");

                //if (string.IsNullOrEmpty(Convert.ToString(dtCustom.Rows[i]["Transdate"])) || (Convert.ToDateTime(dtCustom.Rows[i]["Transdate"]).Subtract(Convert.ToDateTime(dtCustom.Rows[i]["Duedate"])).TotalDays) < 0)
                //    dtCustom.Rows[i]["NoOfDaysOfLatePay"] = "";
                //else
                //    dtCustom.Rows[i]["NoOfDaysOfLatePay"] = Convert.ToDateTime(dtCustom.Rows[i]["Transdate"]).Subtract(Convert.ToDateTime(dtCustom.Rows[i]["Duedate"])).TotalDays;

                if (string.IsNullOrEmpty(Convert.ToString(dtCustom.Rows[i]["Number_Days_Skipped"])))
                    dtCustom.Rows[i]["NoOfDaysOfLatePay"] = "";
                else
                    dtCustom.Rows[i]["NoOfDaysOfLatePay"] = Convert.ToString(dtCustom.Rows[i]["Number_Days_Skipped"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtCustom.Rows[i]["MonthDate"])))
                    dtCustom.Rows[i]["Days"] = "0";
                else
                    dtCustom.Rows[i]["Days"] = System.DateTime.DaysInMonth(Convert.ToDateTime(dtCustom.Rows[i]["MonthDate"]).Year, Convert.ToInt16(Convert.ToDateTime(dtCustom.Rows[i]["MonthDate"]).Month));// Convert.ToString(dtCustom.Rows[i]["Number_Days_Skipped"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtCustom.Rows[i]["TaxAmount"])))
                    dtCustom.Rows[i]["AmtExclTax"] = "0";
                else
                    dtCustom.Rows[i]["AmtExclTax"] =Convert.ToString(Convert.ToDecimal(dtCustom.Rows[i]["Amount"]) - Convert.ToDecimal(dtCustom.Rows[i]["TaxAmount"]));

                dtCustom.AcceptChanges();
            }
           
        }

        private void frmR_GSSAccStaement_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            //if (bGSSTaxApplicable)
            //{
            //    if (ApplicationSettings.Terminal.StoreCountry == "SG" || ApplicationSettings.Terminal.StoreCountry == "MY")
            //        localReport.ReportPath = "rptGSSAccStatementSGMY.rdlc";
            //    else
            //        localReport.ReportPath = "taxRptGSSAccStatement.rdlc";
            //}
            //else
            //localReport.ReportPath = "rptGSSAccStatementSGMY.rdlc";

            localReport.ReportPath = "taxRptGSSAccStatement.rdlc";

            ReportDataSource rdsGSSAccStatementReport = new ReportDataSource("dsGSSStatement", dtCustom);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rdsGSSAccStatementReport);

            ReportParameter[] param = new ReportParameter[20];
            param[0] = new ReportParameter("CName", sCustName);
            param[1] = new ReportParameter("CAddress", sCustAddress);
            param[2] = new ReportParameter("CPinCode", sCPinCode);
            param[3] = new ReportParameter("CContactNo", sContactNo);
            param[4] = new ReportParameter("CId", sCustId);
            param[5] = new ReportParameter("EnrolDate", sOpDate);
            param[6] = new ReportParameter("PurDiscOnMat", sPurDiscOnMat);
            param[7] = new ReportParameter("AmtPerM", sInsAmt);
            param[8] = new ReportParameter("StoreName", sStoreName);
            param[9] = new ReportParameter("DueDateOfMonthPay", sDueDateOfMonthPay);
            param[10] = new ReportParameter("AmtInWords", sAmountInWords);
            param[11] = new ReportParameter("ScemePeriod", sSchemePeriod );
            param[12] = new ReportParameter("GSSAccNumber", sGSSAccNumber);
            param[13] = new ReportParameter("SchemeCode", sSchemeCode);
            param[14] = new ReportParameter("StorePhone", sStorePhNo);
            param[15] = new ReportParameter("CompName", sCompanyName);
            param[16] = new ReportParameter("StoreAddress", sStoreAddress);
            param[17] = new ReportParameter("GSSDueDays", sGSSDueDays);
            param[18] = new ReportParameter("TotAmt", Convert.ToString(decimal.Round(Convert.ToDecimal(dTotAmt), 2)));
            param[19] = new ReportParameter("GovtId", sGovtIdInfo);
                

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }

        private void GetGSSAccInfo(string sGSSNo, SqlConnection conn)
        {

            string sql = "select CONVERT(VARCHAR(11),OPENINGDATE ,103) as OPENINGDATE,A.SCHEMECODE,"+
                         " INSTALLMENTAMOUNT,B.NOOFMONTH,B.RoyalityPercentage   from GSSACCOUNTOPENINGPOSTED A " +
                         " LEFT JOIN GSSSCHEMEMASTER_POSTED B ON A.SCHEMECODE =B.SCHEMECODE" +
                         " WHERE GSSACCOUNTNO ='" + sGSSNo + "' AND ISNULL(A.SCHEMECODE,'')<>''";

            DataTable dtGSSAccInfo = new DataTable();
            SqlCommand command = new SqlCommand(sql, conn);
            command.CommandTimeout = 0;

            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dtGSSAccInfo);

            if (dtGSSAccInfo != null && dtGSSAccInfo.Rows.Count > 0)
            {
                if (Convert.ToString(dtGSSAccInfo.Rows[0]["OPENINGDATE"]) == string.Empty)
                {
                    sOpDate = "-";
                    sDueDateOfMonthPay = "-";
                }
                else
                {
                    sOpDate = Convert.ToString(Convert.ToDateTime(dtGSSAccInfo.Rows[0]["OPENINGDATE"]).ToString("dd-MM-yyyy"));
                    sDueDateOfMonthPay = AddOrdinal(Convert.ToDateTime(dtGSSAccInfo.Rows[0]["OPENINGDATE"]).Day) + "  Of Every Month";
                }
                if (Convert.ToString(dtGSSAccInfo.Rows[0]["SCHEMECODE"]) == string.Empty)
                    sSchemeCode = "-";
                else
                    sSchemeCode = Convert.ToString(dtGSSAccInfo.Rows[0]["SCHEMECODE"]);

                if (Convert.ToString(dtGSSAccInfo.Rows[0]["INSTALLMENTAMOUNT"]) == string.Empty)
                    sInsAmt = "0";
                else
                    sInsAmt = Convert.ToString(decimal.Round(Convert.ToDecimal(dtGSSAccInfo.Rows[0]["INSTALLMENTAMOUNT"]),2));//Convert.ToString(decimal.Round(100, true, 3, MidpointRounding.AwayFromZero));
                
                if (Convert.ToString(dtGSSAccInfo.Rows[0]["NOOFMONTH"]) == string.Empty)
                    sSchemePeriod = "-";
                else
                    sSchemePeriod = Convert.ToString(dtGSSAccInfo.Rows[0]["NOOFMONTH"]) + " Months";

                if (Convert.ToString(dtGSSAccInfo.Rows[0]["RoyalityPercentage"]) == string.Empty)
                     sRPerc = "0";
                else
                     sRPerc = Convert.ToString(decimal.Round(Convert.ToDecimal(dtGSSAccInfo.Rows[0]["RoyalityPercentage"]), 2));//Convert.ToString(decimal.Round(100, true, 3, MidpointRounding.AwayFromZero));


                sPurDiscOnMat = Convert.ToString(decimal.Round((Convert.ToDecimal(sRPerc) * Convert.ToDecimal(sInsAmt) / 100), 2));
                
            }
        }

        private int  GSSDueDays()
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT TOP(1) GSSDueDays FROM [RETAILPARAMETERS] ");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToInt16(cmd.ExecuteScalar());
        }


        public static string AddOrdinal(int num)
        {
            if (num <= 0) return num.ToString();

            switch (num % 100)
            {
                case 11:
                case 12:
                case 13:
                    return num + "th";
            }

            switch (num % 10)
            {
                case 1:
                    return num + "st";
                case 2:
                    return num + "nd";
                case 3:
                    return num + "rd";
                default:
                    return num + "th";
            }

        }
    }
}
