using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Data.SqlClient;


namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmCounterWiseStkReport : Form
    {
        SqlConnection connection;
        bool bCategoryWise = false;
        string sCoun = string.Empty;
        DateTime dtTransDate = Convert.ToDateTime("01/01/1900");

        public frmCounterWiseStkReport()
        {
            InitializeComponent();
        }

        public frmCounterWiseStkReport(SqlConnection Conn)//, string sTransactionDate
        {
            InitializeComponent();
            connection = Conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            //if (!string.IsNullOrEmpty(sTransactionDate))
            //    dtTransDate = Convert.ToDateTime(sTransactionDate);

        }

        public frmCounterWiseStkReport(SqlConnection Conn, int iCatWise, string sCounter = null)//, string sTransactionDate
        {
            InitializeComponent();
            connection = Conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
            if (iCatWise == 1)
                bCategoryWise = true;

            sCoun = sCounter;
            //if (!string.IsNullOrEmpty(sTransactionDate))
            //    dtTransDate = Convert.ToDateTime(sTransactionDate);

        }
        private void frmCounterWiseStkReport_Load(object sender, EventArgs e)
        {
            rptViewerStk.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = rptViewerStk.LocalReport;

            localReport.ReportPath = "rptCounterWiseStock.rdlc";

            
            DataTable dtTransaction = new DataTable();
            GetStockData(ref dtTransaction);

            ReportDataSource rdsStockCount = new ReportDataSource();
            rdsStockCount.Name = "dsCWStk";
            rdsStockCount.Value = dtTransaction;
            rptViewerStk.LocalReport.DataSources.Clear();
            rptViewerStk.LocalReport.DataSources.Add(rdsStockCount);

            ReportParameter[] param = new ReportParameter[1];

            param[0] = new ReportParameter("ParDate", dtTransDate.ToShortDateString());
            this.rptViewerStk.LocalReport.SetParameters(param);

            this.rptViewerStk.RefreshReport();

        }

        private void GetStockData(ref DataTable dtTrans)
        {
            #region
            //string sQuery = string.Empty;

            //if(!bCategoryWise)
            //{
            //    sQuery = " SELECT  CAST(SUM(ISNULL(A.PDSCWQTY,0)) AS NUMERIC (28,0)) AS PCS," +
            //               " CAST(SUM(ISNULL(A.QTY,0)) AS NUMERIC (28,3)) AS Quantity," +
            //               " A.TOCOUNTER AS [Counter]" +
            //               " ,B.ARTICLE_CODE AS CODE" +
            //               " ,ISNULL(C.[DESCRIPTION],'')AS [DESCRIPTION], SUM(ISNULL(B.SETOF,0)) AS SETOF" +
            //         " FROM SKUTable_Posted A INNER JOIN INVENTTABLE B ON A.SKUNUMBER = B.ITEMID" +
            //               " FROM SKUTableTrans A INNER JOIN INVENTTABLE B ON A.SKUNUMBER = B.ITEMID" +
            //               " LEFT OUTER JOIN Article_Master C ON B.ARTICLE_CODE = C.ARTICLE_CODE" +
            //               " WHERE ISNULL(TOCOUNTER,'') <> ''" +
            //               " AND ISNULL(ISAVAILABLE,0) = 1" +
            //        " AND CONVERT(VARCHAR(10),a.CREATEDON,103)=@Transdate" + //CONVERT(VARCHAR(10),a.CREATEDON,103) dtTransDate.ToShortDateString()
            //               " GROUP BY B.ARTICLE_CODE,C.[DESCRIPTION],A.TOCOUNTER" +
            //               " ORDER BY  A.TOCOUNTER,B.ARTICLE_CODE";
            //}
            //else
            //{

            //sQuery = " SELECT  CAST(SUM(ISNULL(A.PDSCWQTY,0)) AS NUMERIC (28,0)) AS PCS," +
            //        " CAST(SUM(ISNULL(A.QTY,0)) AS NUMERIC (28,3)) AS Quantity, " +
            //        " A.TOCOUNTER AS [Counter] ,ISNULL(EC.CODE,'') CODE ,ISNULL(EC.NAME,'') AS [DESCRIPTION], " +
            //        " SUM(ISNULL(B.SETOF,0)) AS SETOF FROM SKUTableTrans A INNER JOIN INVENTTABLE B" +
            //        " ON A.SKUNUMBER = B.ITEMID left join ECORESPRODUCTCATEGORY ECP  ON B.PRODUCT = ECP.PRODUCT " +
            //        " left join ECORESCATEGORY EC   ON EC.RECID =ECP.CATEGORY " +
            //        " WHERE ISNULL(TOCOUNTER,'') <> '' AND ISNULL(ISAVAILABLE,0) = 1 ";
            //if(!string.IsNullOrEmpty(sCoun))
            //    sQuery = sQuery + " AND A.TOCOUNTER ='" + sCoun + "'";
            //sQuery = sQuery + " GROUP BY EC.CODE,EC.NAME,A.TOCOUNTER ORDER BY  A.TOCOUNTER,EC.CODE";
            //}

            //SqlCommand command = new SqlCommand(sQuery, connection);

            //command.Parameters.Add(new SqlParameter("@Transdate", dtTransDate.ToString("dd/MM/yyyy")));

            //command.CommandTimeout = 0;
            //SqlDataAdapter daStock = new SqlDataAdapter(command);
            //daStock.Fill(dsStock, "Transaction");
            #endregion

            if(!bCategoryWise)
            {
                string sQuery = "GETSTOCKREPORT";
                SqlCommand command = new SqlCommand(sQuery, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add("@TOCOUNTER", SqlDbType.NVarChar).Value = sCoun;
                command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;               
                SqlDataAdapter daTrans = new SqlDataAdapter(command);
                daTrans.Fill(dtTrans);
            }
            else
            {

                string sQuery = "GETSTOCKREPORTCATEGORYWISE";
                SqlCommand command = new SqlCommand(sQuery, connection);
                command.CommandType = CommandType.StoredProcedure;
                command.CommandTimeout = 0;
                command.Parameters.Clear();
                command.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;
                command.Parameters.Add("@TOCOUNTER", SqlDbType.NVarChar).Value = sCoun;
                SqlDataAdapter daTrans = new SqlDataAdapter(command);
                daTrans.Fill(dtTrans);
            }
            
        }
    }
}
