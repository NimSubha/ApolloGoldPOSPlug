using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using Microsoft.Reporting.WinForms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmEstimationReport : Form
    {
        BlankOperations oBlank = new BlankOperations();
        SqlConnection connection;
        DataTable dtEstDetail;
        DataTable dtEstIngrd;
        string sCustName = string.Empty;
        string sCustId = string.Empty;

        string sInvoiceNo = string.Empty;
        string sEstDate = string.Empty;
        string sProdDesc = "-";
        string sSKUNo = "-";
        string sDesignNo = "-";
        string sComplexity = "-";
        string sGrossWt = "0";
        string sStoreCode = string.Empty;
        string sStoreName = string.Empty;
        string sGoldRate = string.Empty;
        string sSilverRate = string.Empty;
        string sTerminalCode = string.Empty;
        int iNew = 0;
        string sContactNo = "";
        string sTime = "";

        public frmEstimationReport()
        {
            InitializeComponent();
        }

        public frmEstimationReport(IPosTransaction posTransaction, DataTable dtDetail, DataTable dtIngredient, SqlConnection conn = null)
        {
            InitializeComponent();

            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                    sInvoiceNo = Convert.ToString(retailTrans.TransactionId);
              
                sEstDate = retailTrans.BeginDateTime.ToShortDateString();
                sTime = retailTrans.BeginDateTime.ToShortTimeString();

                dtEstDetail = dtDetail;
                dtEstIngrd = dtIngredient;

                if (Convert.ToString(retailTrans.Customer.Name) != string.Empty)
                    sCustName = Convert.ToString(retailTrans.Customer.Name);

                if (Convert.ToString(retailTrans.Customer.CustomerId) != string.Empty)
                    sCustId = Convert.ToString(retailTrans.Customer.CustomerId);

                if (!string.IsNullOrEmpty(retailTrans.Customer.Telephone))
                    sContactNo = Convert.ToString(retailTrans.Customer.Telephone);


                if (sContactNo == "")
                {
                    sContactNo = GetCustomerMobilePrimary(retailTrans.Customer.CustomerId);// "select retailmobileprimary from custtable";
                }


                //Start: added on 24-03-2014 R.Hossain
                if (Convert.ToString(ApplicationSettings.Terminal.StoreId) != string.Empty)
                    sStoreCode = Convert.ToString(ApplicationSettings.Terminal.StoreId);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.TerminalId) != string.Empty)
                    sTerminalCode = Convert.ToString(ApplicationSettings.Terminal.TerminalId);

                sGoldRate = getConfigId(1) + " ; " + getMetalRate(1);
                sSilverRate = getConfigId(2) + " ; " + getMetalRate(2);
                //sPltRate = getConfigId(3) + " ; " + getMetalRate(3);

                if (dtEstDetail != null && dtEstDetail.Rows.Count > 0)
                {
                    sSKUNo = Convert.ToString(dtEstDetail.Rows[0]["ItemId"]); ;
                }

                sGrossWt = getValue("SELECT CAST(QTY AS decimal (6,3)) AS QTY  FROM SKUTABLE_POSTED WHERE SKUNumber ='" + sSKUNo + "'");
                sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                sDesignNo = getValue("SELECT ITEMIDPARENT FROM INVENTTABLE WHERE ITEMID='" + sSKUNo + "'");
                sComplexity = getValue("select COMPLEXITY_CODE from INVENTTABLE  WHERE ITEMID='" + sSKUNo + "'");
                //End: added on 24-03-2014 R.Hossain

            }
            else
            {
                sInvoiceNo = "Customer Estimation";
                sEstDate = DateTime.Now.ToShortDateString();
                dtEstDetail = dtDetail;
                dtEstIngrd = dtIngredient;

                //Start: added on 24-03-2014 R.Hossain
                if (Convert.ToString(ApplicationSettings.Terminal.StoreId) != string.Empty)
                    sStoreCode = Convert.ToString(ApplicationSettings.Terminal.StoreId);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.TerminalId) != string.Empty)
                    sTerminalCode = Convert.ToString(ApplicationSettings.Terminal.TerminalId);

                sGoldRate = getConfigId(1) + " ; " + getMetalRate(1);
                sSilverRate = getConfigId(2) + " ; " + getMetalRate(2);
                //sPltRate = getConfigId(3) + " ; " + getMetalRate(3);

                if (dtEstDetail != null && dtEstDetail.Rows.Count > 0)
                {
                    sSKUNo = Convert.ToString(dtEstDetail.Rows[0]["ITEMID"]); ;
                }

                sGrossWt = getValue("SELECT CAST(QTY AS decimal (10,3)) AS QTY  FROM SKUTABLE_POSTED WHERE SKUNumber ='" + sSKUNo + "'");
                sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                sDesignNo = getValue("SELECT ITEMIDPARENT FROM INVENTTABLE WHERE ITEMID='" + sSKUNo + "'");
                sComplexity = getValue("select COMPLEXITY_CODE from INVENTTABLE  WHERE ITEMID='" + sSKUNo + "'");
            }
        }

        public frmEstimationReport(IPosTransaction posTransaction, DataTable dtDetail, SqlConnection conn = null)
        {
            InitializeComponent();
            iNew = 1;
            RetailTransaction retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                if (Convert.ToString(retailTrans.TransactionId) != string.Empty)
                    sInvoiceNo = Convert.ToString(retailTrans.TransactionId);
                sEstDate = DateTime.Now.ToShortDateString();
                dtEstDetail = dtDetail;

                if (Convert.ToString(retailTrans.Customer.Name) != string.Empty)
                    sCustName = Convert.ToString(retailTrans.Customer.Name);

                if (Convert.ToString(retailTrans.Customer.CustomerId) != string.Empty)
                    sCustId = Convert.ToString(retailTrans.Customer.CustomerId);

                if (!string.IsNullOrEmpty(retailTrans.Customer.Telephone))
                    sContactNo = Convert.ToString(retailTrans.Customer.Telephone);


                if (sContactNo == "")
                {
                    sContactNo = GetCustomerMobilePrimary(retailTrans.Customer.CustomerId);// "select retailmobileprimary from custtable";
                }


                //Start: added on 24-03-2014 R.Hossain
                if (Convert.ToString(ApplicationSettings.Terminal.StoreId) != string.Empty)
                    sStoreCode = Convert.ToString(ApplicationSettings.Terminal.StoreId);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.TerminalId) != string.Empty)
                    sTerminalCode = Convert.ToString(ApplicationSettings.Terminal.TerminalId);

                // sGoldRate = getConfigId(1) + " ; " + getMetalRate(1);
                sSilverRate = getConfigId(2) + " ; " + getMetalRate(2);
                //sPltRate = getConfigId(3) + " ; " + getMetalRate(3);

                if (dtEstDetail != null && dtEstDetail.Rows.Count > 0)
                {
                    sSKUNo = Convert.ToString(dtEstDetail.Rows[0]["SKU"]); ;
                }

                sGrossWt = getValue("SELECT CAST(QTY AS decimal (6,3)) AS QTY  FROM SKUTABLE_POSTED WHERE SKUNumber ='" + sSKUNo + "'");
                sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                sDesignNo = getValue("SELECT ITEMIDPARENT FROM INVENTTABLE WHERE ITEMID='" + sSKUNo + "'");
                sComplexity = getValue("select COMPLEXITY_CODE from INVENTTABLE  WHERE ITEMID='" + sSKUNo + "'");
                //End: added on 24-03-2014 R.Hossain

            }
            else
            {
                sInvoiceNo = "Customer Estimation";
                sEstDate = DateTime.Now.ToShortDateString();
                dtEstDetail = dtDetail;

                if (Convert.ToString(retailTrans.Customer.Name) != string.Empty)
                    sCustName = Convert.ToString(retailTrans.Customer.Name);
                //Start: added on 24-03-2014 R.Hossain
                if (Convert.ToString(ApplicationSettings.Terminal.StoreId) != string.Empty)
                    sStoreCode = Convert.ToString(ApplicationSettings.Terminal.StoreId);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.TerminalId) != string.Empty)
                    sTerminalCode = Convert.ToString(ApplicationSettings.Terminal.TerminalId);

                if (Convert.ToString(retailTrans.Customer.CustomerId) != string.Empty)
                    sCustId = Convert.ToString(retailTrans.Customer.CustomerId);

                if (!string.IsNullOrEmpty(retailTrans.Customer.Telephone))
                    sContactNo = Convert.ToString(retailTrans.Customer.Telephone);


                if (sContactNo == "-")
                {
                    sContactNo = GetCustomerMobilePrimary(retailTrans.Customer.CustomerId);// "select retailmobileprimary from custtable";
                }

                //sGoldRate = getConfigId(1) + " ; " + getMetalRate(1);
                sSilverRate = getConfigId(2) + " ; " + getMetalRate(2);
                //sPltRate = getConfigId(3) + " ; " + getMetalRate(3);

                if (dtEstDetail != null && dtEstDetail.Rows.Count > 0)
                {
                    sSKUNo = Convert.ToString(dtEstDetail.Rows[0]["SKU"]); ;
                }

                sGrossWt = getValue("SELECT CAST(QTY AS decimal (10,3)) AS QTY  FROM SKUTABLE_POSTED WHERE SKUNumber ='" + sSKUNo + "'");
                sProdDesc = getValue("SELECT R.NAME FROM ECORESPRODUCTTRANSLATION AS R, INVENTTABLE AS E WHERE E.PRODUCT  = R.PRODUCT and E.ITEMID='" + sSKUNo + "'");
                sDesignNo = getValue("SELECT ITEMIDPARENT FROM INVENTTABLE WHERE ITEMID='" + sSKUNo + "'");
                sComplexity = getValue("select COMPLEXITY_CODE from INVENTTABLE  WHERE ITEMID='" + sSKUNo + "'");
            }

            DataTable dtMatalRate = new DataTable();
            GetMetalRates(ref dtMatalRate);
        }
        
        private void frmEstimationReport_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;

            localReport.ReportPath = "rptEstimation.rdlc";

            DataSet dsDetail = new DataSet();
            dsDetail.Tables.Add(dtEstDetail);

            DataSet dsIngredient = new DataSet();
            dsIngredient.Tables.Add(dtEstIngrd);

            //DataSet dsBarcode = new DataSet();
            //GetBarcodeInfo(ref dsBarcode, bitmap01);

            this.reportViewer1.LocalReport.DataSources.Clear();
            ReportDataSource rdsDetail = new ReportDataSource();
            rdsDetail.Name = "Detail";
            //  rdsDetail.Value = dtEstDetail;
            rdsDetail.Value = dsDetail.Tables[0];
            // this.reportViewer1.LocalReport.DataSources.Clear();

            //this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Detail", dtEstDetail));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Detail", dsDetail.Tables[0]));

            // this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Detail", rdsDetail.Value));

            ReportDataSource rdsIngredient = new ReportDataSource();
            rdsIngredient.Name = "Ingredient";
            //rdsIngredient.Value = dtEstIngrd;
            rdsIngredient.Value = dsIngredient.Tables[0];

            // this.reportViewer1.LocalReport.DataSources.Clear();
            //  this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Ingredient", dtEstIngrd));
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("Ingredient", dsIngredient.Tables[0]));

            ReportParameter[] param = new ReportParameter[17];
            param[0] = new ReportParameter("InvoiceNo", sInvoiceNo);
            param[1] = new ReportParameter("EstDate", sEstDate);

            //Start: added on 24-03-2014 R.Hossain
            param[2] = new ReportParameter("ProdDesc", sProdDesc);
            param[3] = new ReportParameter("SKUNo", sSKUNo);
            param[4] = new ReportParameter("DesignNo", sDesignNo);
            param[5] = new ReportParameter("Complexity", sComplexity);
            param[6] = new ReportParameter("GrossWt", sGrossWt);
            param[7] = new ReportParameter("StoreCode", sStoreCode);
            param[8] = new ReportParameter("StoreName", sStoreName);
            param[9] = new ReportParameter("GoldRate", sGoldRate);
            param[10] = new ReportParameter("SilverRate", sSilverRate);
            param[11] = new ReportParameter("TerminalCode", sTerminalCode);
            param[12] = new ReportParameter("Time", sTime);
            param[13] = new ReportParameter("PltRate", "");
            param[14] = new ReportParameter("CustAcc", sCustId);
            param[15] = new ReportParameter("CustName", sCustName);
            param[16] = new ReportParameter("CustMobile", sContactNo);

            //End  :  added on 24-03-2014 R.Hossain

            this.reportViewer1.LocalReport.SetParameters(param);

            this.reportViewer1.RefreshReport();

            //oBlank.ExportForEstimation(reportViewer1.LocalReport);
            //oBlank.Print_Estimation();

            //oBlank.ExportForEstimation(reportViewer1.LocalReport);
            //oBlank.Print_Estimation();

            //oBlank.ExportForEstimation(reportViewer1.LocalReport);
            //oBlank.Print_Estimation();
        }

        private decimal getMetalRate(int iMetalType)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @CONFIGIDSTANDARD VARCHAR(20) ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + sStoreCode.Trim() + "'  ");
            if (iMetalType == 1)
                commandText.Append(" SELECT @CONFIGIDSTANDARD = DEFAULTCONFIGIDGOLD FROM RETAILPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else if (iMetalType == 2)
                commandText.Append(" SELECT @CONFIGIDSTANDARD = DEFAULTCONFIGIDSILVER FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else
                commandText.Append(" SELECT @CONFIGIDSTANDARD = DEFAULTCONFIGIDPLATINUM FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT TOP 1 CAST(ISNULL(RATES,0) AS decimal (6,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            commandText.Append(" AND METALTYPE=" + iMetalType + " ");
            commandText.Append(" AND RETAIL=1 AND RATETYPE=3"); // SAle            
            commandText.Append(" AND ACTIVE=1 AND ");
            commandText.Append(" CONFIGIDSTANDARD =@CONFIGIDSTANDARD");
            commandText.Append(" ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");
            commandText.Append(" END ");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return Convert.ToDecimal(sResult.Trim());
            }
            else
            {
                return 0;
            }
        }

        private string getConfigId(int iMetalType) // from inventparameter
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            if (iMetalType == 1)
                commandText.Append(" SELECT DEFAULTCONFIGIDGOLD FROM RETAILPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else if (iMetalType == 2)
                commandText.Append(" SELECT DEFAULTCONFIGIDSILVER FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else
                commandText.Append(" SELECT DEFAULTCONFIGIDPLATINUM FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        private string getValue(string sSqlString) // passing sql query string return  one string value
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(sSqlString);

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        private void GetMetalRates(ref DataTable dTMetalRates) // added on 31/03/2014 RHossain for  show the tax detail in line
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string sQuery = "GETMETALRATES";
            SqlCommand command = new SqlCommand(sQuery, conn);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 0;
            command.Parameters.Clear();
            command.Parameters.Add("@STOREID", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
            command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
            SqlDataAdapter daCol = new SqlDataAdapter(command);
            daCol.Fill(dTMetalRates);

            if (conn.State == ConnectionState.Open)
                conn.Close();
            //if(dTMetalRates != null && dTMetalRates.Rows.Count > 0)
            //{
            //    for(int i = 1; i <= dTMetalRates.Rows.Count; i++)
            //    {
            //        if(i == 1)
            //            sMetalRates = Convert.ToString(dTMetalRates.Rows[i - 1]["CONFIGIDSTANDARD"]) + " Rates : " + Convert.ToString(dTMetalRates.Rows[i - 1]["RATES"]);
            //        else
            //            sMetalRates = sMetalRates + Environment.NewLine + Convert.ToString(dTMetalRates.Rows[i - 1]["CONFIGIDSTANDARD"]) + " Rates : " + Convert.ToString(dTMetalRates.Rows[i - 1]["RATES"]);
            //    }
            //}

            DataTable dt = GetConfigListForRateShow();

            if (dTMetalRates != null && dTMetalRates.Rows.Count > 0)
            {
                for (int i = 1; i <= dTMetalRates.Rows.Count; i++)
                {

                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (Convert.ToString(dTMetalRates.Rows[i - 1]["CONFIGIDSTANDARD"]) == Convert.ToString(row["CONFIG"]))
                            {
                                if (Convert.ToInt16(row["SEQUENCE"]) == 1)
                                {
                                    sGoldRate = sGoldRate + Convert.ToString(row["CONFIG"]) + ":" + " Rates : " + Convert.ToString(dTMetalRates.Rows[i - 1]["RATES"]) + "; ";
                                }

                                if (Convert.ToInt16(row["SEQUENCE"]) == 2)
                                {
                                    sGoldRate = sGoldRate + Convert.ToString(row["CONFIG"]) + ":" + " Rates : " + Convert.ToString(dTMetalRates.Rows[i - 1]["RATES"]) + "; ";
                                }

                                if (Convert.ToInt16(row["SEQUENCE"]) == 3)
                                {
                                    sGoldRate = sGoldRate + Convert.ToString(row["CONFIG"]) + ":" + " Rates : " + Convert.ToString(dTMetalRates.Rows[i - 1]["RATES"]);
                                }
                            }
                        }
                    }
                }
            }
        }

        private DataTable GetConfigListForRateShow()
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "SELECT top 3 isnull(SEQUENCE,0) SEQUENCE ,isnull(CONFIG,'') CONFIG FROM RATESHOWCONFIGURATION order by SEQUENCE asc";

                DataTable CustBalDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustBalDt);

                if (SqlCon.State == ConnectionState.Closed)
                    SqlCon.Open();

                return CustBalDt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GetCustomerMobilePrimary(string sCustAcc)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("SELECT RETAILMOBILEPRIMARY FROM CUSTTABLE WHERE ACCOUNTNUM='" + sCustAcc + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }
    }
}
