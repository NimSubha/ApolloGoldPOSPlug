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
    public partial class frmCounterWiseStkDtlReport : Form
    {
        SqlConnection connection;
        DateTime dtTransDate = Convert.ToDateTime("01/01/1900");
        string sCounter = "";

        public frmCounterWiseStkDtlReport()
        {
            InitializeComponent();
        }

        public frmCounterWiseStkDtlReport(SqlConnection Conn)//, string sTransactionDate
        {
            InitializeComponent();
            connection = Conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            //if (!string.IsNullOrEmpty(sTransactionDate))
            //    dtTransDate = Convert.ToDateTime(sTransactionDate);
        }

        public frmCounterWiseStkDtlReport(SqlConnection Conn, string sC)//, string sTransactionDate
        {
            InitializeComponent();
            connection = Conn;
            if(connection.State == ConnectionState.Closed)
                connection.Open();

            sCounter = sC;
            //if (!string.IsNullOrEmpty(sTransactionDate))
            //    dtTransDate = Convert.ToDateTime(sTransactionDate);
        }

        private void frmCounterWiseStkDtlReport_Load(object sender, EventArgs e)
        {
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            localReport.ReportPath = "rptCounterWiseStockDtl.rdlc";
            DataSet dataset = new DataSet();
            GetStockData(ref dataset);
            ReportDataSource rdsStockCount = new ReportDataSource("dsCWStkDtl", dataset.Tables[0]);
            reportViewer1.LocalReport.DataSources.Clear();
            reportViewer1.LocalReport.DataSources.Add(rdsStockCount);

            ReportParameter[] param = new ReportParameter[1];

            param[0] = new ReportParameter("ParDate", dtTransDate.ToShortDateString());
            this.reportViewer1.LocalReport.SetParameters(param);

            this.reportViewer1.RefreshReport();
        }

        private void GetStockData(ref DataSet dsStock)
        {
            string sQuery = " SELECT A.TOCOUNTER AS [Counter],A.SkuNumber," +
                            " CAST(ISNULL(A.QTY,0) AS NUMERIC (28,3)) AS Quantity, F.NAME AS ITEMDESC,ISNULL(B.SETOF,0) AS SETOF," +
                            " CAST(ISNULL(SKP.NETQTY,0) AS NUMERIC (28,3)) NETQTY,"+
                            " CAST(ISNULL(SKP.DMDQTY,0) AS NUMERIC (28,3)) DMDQTY, "+
                           // " CAST(ISNULL(SKP.STNQTY,0) AS NUMERIC (28,3)) STNQTY,
                            "  Isnull((select Sum(qty)*0.2 from SKULine_Posted x,INVENTTABLE Y where X.ItemID = Y.ITEMID AND X.SkuNumber = SKP.SKUNUMBER AND Y.METALTYPE = 7 And X.UnitID ='CT'),0)+ "+
                            " Isnull((select Sum(qty) from SKULine_Posted x,INVENTTABLE Y where X.ItemID = Y.ITEMID AND X.SkuNumber = SKP.SKUNUMBER AND Y.METALTYPE = 7 And X.UnitID ='Gms'),0) STNQTY, "+
                            " isnull(b.ARTICLE_CODE,'') as ArticleCode  " +
                            " FROM SKUTableTrans A INNER JOIN INVENTTABLE B ON A.SKUNUMBER = B.ITEMID " +
                            " LEFT OUTER JOIN ECORESPRODUCT E ON B.PRODUCT = E.RECID" +
                            " LEFT OUTER JOIN ECORESPRODUCTTRANSLATION F ON E.RECID = F.PRODUCT and f.LANGUAGEID='en-us'" +
                            " LEFT JOIN SKUTable_Posted SKP ON A.SkuNumber=SKP.SkuNumber "+
                            " WHERE ISNULL(A.TOCOUNTER,'') <> '' AND ISNULL(A.ISAVAILABLE,0) = 1" +
                            " AND A.TOCOUNTER = COALESCE(NULLIF('" + sCounter + "', ''), A.TOCOUNTER) " + // added on 22/11/2016
                            //" AND CONVERT(VARCHAR(10),a.CREATEDON,103)=@Transdate" + //CONVERT(VARCHAR(10),a.CREATEDON,103) dtTransDate.ToShortDateString()
                            " ORDER BY A.TOCOUNTER, SkuNumber";

            SqlCommand command = new SqlCommand(sQuery, connection);
            //command.Parameters.Add(new SqlParameter("@Transdate", dtTransDate.ToString("dd/MM/yyyy")));

            command.CommandTimeout = 0;
            SqlDataAdapter daStock = new SqlDataAdapter(command);

            daStock.Fill(dsStock, "Transaction");

        }
    }
}
