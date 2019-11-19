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
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class TransReportFrm : frmTouchBase
    {
        public TransReportFrm()
        {
            InitializeComponent();
        }
        SqlConnection connection;
        public TransReportFrm(SqlConnection conn)
        {
            InitializeComponent();

            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        private void TransReportFrm_Load(object sender, EventArgs e)
        {


        }

        private void GetSalesOrderData(ref DataSet dsSalesOrder)
        {
            string sqlSalesOrder = " SELECT     RETAILTRANSACTIONSALESTRANS.ITEMID,  " +
                                   " (CASE WHEN RETAIL_CUSTOMCALCULATIONS_TABLE.TRANSACTIONTYPE = 0 THEN 'Sale' WHEN RETAIL_CUSTOMCALCULATIONS_TABLE.TRANSACTIONTYPE = 1 THEN  " +
                                   " 'Purchase' WHEN RETAIL_CUSTOMCALCULATIONS_TABLE.TRANSACTIONTYPE = 2 THEN 'Purchase Return' WHEN RETAIL_CUSTOMCALCULATIONS_TABLE.TRANSACTIONTYPE " +
                                   " = 3 THEN 'Exchange' WHEN RETAIL_CUSTOMCALCULATIONS_TABLE.TRANSACTIONTYPE = 4 THEN 'Exchange Return' WHEN RETAIL_CUSTOMCALCULATIONS_TABLE.TRANSACTIONTYPE " +
                                   " = 5 THEN 'Adjustment' END) AS TYPE, RETAIL_CUSTOMCALCULATIONS_TABLE.PIECES, RETAIL_CUSTOMCALCULATIONS_TABLE.QUANTITY,  " +
                                   " RETAIL_CUSTOMCALCULATIONS_TABLE.TOTALAMOUNT, RETAILTRANSACTIONSALESTRANS.TRANSACTIONID " +
                                   " FROM         RETAILTRANSACTIONSALESTRANS INNER JOIN " +
                                   "  RETAIL_CUSTOMCALCULATIONS_TABLE ON RETAILTRANSACTIONSALESTRANS.TRANSACTIONID = RETAIL_CUSTOMCALCULATIONS_TABLE.TRANSACTIONID AND  " +
                                   " RETAILTRANSACTIONSALESTRANS.LINENUM = RETAIL_CUSTOMCALCULATIONS_TABLE.LINENUM " +
                               //    " WHERE  RETAILTRANSACTIONSALESTRANS.TRANSDATE BETWEEN  @fromdate AND  @todate  AND RETAILTRANSACTIONSALESTRANS.TERMINALID=ISNULL(@terminal, RETAILTRANSACTIONSALESTRANS.TERMINALID) " +
                                   " ORDER BY RETAIL_CUSTOMCALCULATIONS_TABLE.TRANSACTIONID ";
            //" RETAILTRANSACTIONSALESTRANS ON RETAILTRANSACTIONTABLE.TRANSACTIONID = RETAILTRANSACTIONSALESTRANS.TRANSACTIONID INNER JOIN " +
            //  " RETAIL_CUSTOMCALCULATIONS_TABLE ON RETAILTRANSACTIONSALESTRANS.TRANSACTIONID = RETAIL_CUSTOMCALCULATIONS_TABLE.TRANSACTIONID ";


            SqlCommand command = new SqlCommand(sqlSalesOrder, connection);

       //     command.Parameters.Add(new SqlParameter("@fromdate", Convert.ToDateTime(dateTimePicker1.Text)));
        //    command.Parameters.Add(new SqlParameter("@todate", Convert.ToDateTime(dateTimePicker2.Text)));
        //    command.Parameters.Add(new SqlParameter("@terminal", (string.IsNullOrEmpty(textBox1.Text)) ? "null" : textBox1.Text));

            SqlDataAdapter salesOrderAdapter = new SqlDataAdapter(command);

            salesOrderAdapter.Fill(dsSalesOrder, "TransactionalData");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            rptViewer.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = rptViewer.LocalReport;
            localReport.ReportPath = "TransactionReport.rdlc";
            DataSet dataset = new DataSet();

            GetSalesOrderData(ref dataset);


            ReportDataSource dsSalesOrder = new ReportDataSource();
            dsSalesOrder.Name = "DataSet1";
            dsSalesOrder.Value = dataset.Tables[0];
            //   localReport.DataSources.Add(dsSalesOrder);
            this.rptViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataset.Tables[0]));

            rptViewer.RefreshReport();
        }


    }
}
