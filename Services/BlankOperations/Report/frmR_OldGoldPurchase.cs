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
    public partial class frmR_OldGoldPurchase : Form
    {
        public frmR_OldGoldPurchase()
        {
            InitializeComponent();
        }

        SqlConnection connection;
        public frmR_OldGoldPurchase(SqlConnection conn)
        {
            InitializeComponent();

            connection = conn;
            if (connection.State == ConnectionState.Closed)
                connection.Open();
        }

        private void GetSalesOrderData(ref DataSet dsSalesOrder)
        {
            string sqlSalesOrder = " Select A.TRANSACTIONID,A.RECEIPTID,B.ITEMID,"+
              "  (CASE WHEN C.METALTYPE=1 THEN 'Gold' "+
                   " WHEN C.METALTYPE=0 THEN ' Other'"+
                    " WHEN C.METALTYPE=2 THEN ' Silver'"+
                   " WHEN C.METALTYPE=3 THEN ' Platinum'"+
                   " WHEN C.METALTYPE=4 THEN ' Alloy'"+
                    " WHEN C.METALTYPE=5 THEN ' Diamond '"+
                    " WHEN C.METALTYPE=6 THEN ' Pearl'"+
                    " WHEN C.METALTYPE=7 THEN ' Stone'"+
                   " WHEN C.METALTYPE=8 THEN ' Consumables'"+
                   " WHEN C.METALTYPE=11 THEN ' Watch'"+
                   " WHEN C.METALTYPE=12 THEN ' LooseDmd' "+
                    " WHEN C.METALTYPE=13 THEN ' Palladium' "+
                   " else ''"+
                    " END) METALTYPE"+
            " ,ABS(D.QUANTITY) AS QNTY,D.EXPECTEDQUANTITY,D.CRate, " +
                                   " ABS(D.AMOUNT) AS AMOUNT from RETAILTRANSACTIONTABLE A,RETAILTRANSACTIONSALESTRANS B,  " +
                                   " INVENTTABLE C,RETAIL_CUSTOMCALCULATIONS_TABLE D " +
                                   " WHERE A.TRANSACTIONID=B.TRANSACTIONID AND B.TRANSACTIONID=D.TRANSACTIONID AND B.LINENUM=D.LINENUM AND B.ITEMID=C.ITEMID and d.transactiontype=1 ";


            SqlCommand command = new SqlCommand(sqlSalesOrder, connection);

            //     command.Parameters.Add(new SqlParameter("@fromdate", Convert.ToDateTime(dateTimePicker1.Text)));
            //    command.Parameters.Add(new SqlParameter("@todate", Convert.ToDateTime(dateTimePicker2.Text)));
            //    command.Parameters.Add(new SqlParameter("@terminal", (string.IsNullOrEmpty(textBox1.Text)) ? "null" : textBox1.Text));

            SqlDataAdapter salesOrderAdapter = new SqlDataAdapter(command);

            salesOrderAdapter.Fill(dsSalesOrder, "TransactionalData");
        }

        private void frmR_OldGoldPurchase_Load(object sender, EventArgs e)
        {

            rptviewer.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = rptviewer.LocalReport;
            localReport.ReportPath = "ogpReport.rdlc";
            DataSet dataset = new DataSet();

            GetSalesOrderData(ref dataset);


            ReportDataSource dsSalesOrder = new ReportDataSource();
            dsSalesOrder.Name = "DataSet1";
            dsSalesOrder.Value = dataset.Tables[0];
            //   localReport.DataSources.Add(dsSalesOrder);
            this.rptviewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", dataset.Tables[0]));

            rptviewer.RefreshReport();
        }
    }
}
