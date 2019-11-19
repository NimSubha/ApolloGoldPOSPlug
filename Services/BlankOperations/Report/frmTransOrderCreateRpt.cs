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

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmTransOrderCreateRpt : Form
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

        DataSet dsTOHdr, dsTODtl;
        string sAmtword = string.Empty;
        int iPrintOption = 0; // 0 -- summary and 1-- Detail
        string sCompanyName = string.Empty;
        string sStoreAddress = "-";
        string sStorePhNo = "...";
        string sStoreName = "";
        DataTable dtCustom = new DataTable();
        string sTitle = "";

        public frmTransOrderCreateRpt()
        {
            InitializeComponent();
        }

        public frmTransOrderCreateRpt(DataSet dsHdr, DataSet dsDtl, int iOption, string sVouTtile)
        {
            InitializeComponent();
            if (dsHdr != null && dsDtl != null)
            {
                iPrintOption = iOption;
                dsTOHdr = new DataSet();
                dsTOHdr = dsHdr;
                dsTODtl = new DataSet();
                dsTODtl = dsDtl;
                //sAmtword = Convert.ToString(dsTOHdr.Tables[0].Rows[0]["CRWTransferAmount"]);
                sCompanyName = GetCompanyName();

                if (Convert.ToString(ApplicationSettings.Terminal.StoreName) != string.Empty)
                    sStoreName = Convert.ToString(ApplicationSettings.Terminal.StoreName);
                if (Convert.ToString(ApplicationSettings.Terminal.StoreAddress) != string.Empty)
                    sStoreAddress = Convert.ToString(ApplicationSettings.Terminal.StoreAddress);
                if (!string.IsNullOrEmpty(Convert.ToString(ApplicationSettings.Terminal.StorePhone)))
                    sStorePhNo = Convert.ToString(ApplicationSettings.Terminal.StorePhone);

                GetCustomData(dsDtl.Tables[0]);
                sTitle = sVouTtile;
            }
        }

        private void GetCustomData(DataTable dtFromAx)
        {
            dtCustom = new DataTable("dtCustom");

            dtCustom.Columns.Add("ItemId", typeof(string));
            dtCustom.Columns.Add("ITEMNAME", typeof(string));
            dtCustom.Columns.Add("PdsCWQtyTransfer", typeof(decimal));
            dtCustom.Columns.Add("GrossWt", typeof(decimal));
            dtCustom.Columns.Add("StoneWt", typeof(decimal));
            dtCustom.Columns.Add("NettQty", typeof(decimal));
            dtCustom.Columns.Add("CONFIGID", typeof(string));
            dtCustom.Columns.Add("CRWTransferAmount", typeof(decimal));
            dtCustom.Columns.Add("HSNCode", typeof(string));
            dtCustom.Columns.Add("TotalBaseAmt", typeof(decimal));
            dtCustom.Columns.Add("DmdQty", typeof(decimal));
            dtCustom.Columns.Add("InventBatchId", typeof(string));
            dtCustom.Columns.Add("UnitPrice", typeof(decimal));
            dtCustom.Columns.Add("TaxValue", typeof(decimal));
            dtCustom.Columns.Add("TaxAmount", typeof(decimal));
            dtCustom.Columns.Add("Setof", typeof(int));

            dtCustom.AcceptChanges();

            if (dtFromAx != null && dtFromAx.Rows.Count > 0)
            {
                for (int i = 0; i <= dtFromAx.Rows.Count - 1; i++)
                {
                    var row = dtCustom.NewRow();
                    row["ItemId"] = Convert.ToString(dtFromAx.Rows[i]["ItemId"]);
                    row["ITEMNAME"] = Convert.ToString(dtFromAx.Rows[i]["ITEMNAME"]);
                    row["PdsCWQtyTransfer"] = Convert.ToDecimal(dtFromAx.Rows[i]["PdsCWQtyTransfer"]);
                    row["GrossWt"] = Convert.ToDecimal(dtFromAx.Rows[i]["GrossWt"]);
                    row["StoneWt"] = Convert.ToDecimal(dtFromAx.Rows[i]["StoneWt"]);
                    row["NettQty"] = Convert.ToDecimal(dtFromAx.Rows[i]["NettQty"]);
                    row["CONFIGID"] = Convert.ToString(dtFromAx.Rows[i]["CONFIGID"]);
                    row["CRWTransferAmount"] = Convert.ToDecimal(dtFromAx.Rows[i]["CRWTransferAmount"]);
                    row["HSNCode"] = Convert.ToString(dtFromAx.Rows[i]["HSNCode"]);
                    row["TotalBaseAmt"] = Convert.ToDecimal(dtFromAx.Rows[i]["TotalBaseAmt"]);
                    row["DmdQty"] = Convert.ToDecimal(dtFromAx.Rows[i]["DmdQty"]);
                    row["InventBatchId"] = Convert.ToString(dtFromAx.Rows[i]["InventBatchId"]);
                    row["UnitPrice"] = Convert.ToDecimal(dtFromAx.Rows[i]["UnitPrice"]);
                    row["TaxValue"] = Convert.ToDecimal(dtFromAx.Rows[i]["TaxValue"]);
                    row["TaxAmount"] = Convert.ToDecimal(dtFromAx.Rows[i]["TaxAmount"]);
                    row["Setof"] = Convert.ToInt16(dtFromAx.Rows[i]["Setof"]);

                    dtCustom.Rows.Add(row);
                }
            }

        }


       

        private void frmTransOrderCreateRpt_Load(object sender, EventArgs e)
        {

            DataTable dtStoreLogo = new DataTable();

            dtStoreLogo = GetStoreLogo();

            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            if (iPrintOption == 0)
                localReport.ReportPath = "rptTransOrderCreate.rdlc";
            //else

            //    localReport.ReportPath = "rptTransOrderCreateDtl.rdlc";

            ReportDataSource rdsHdr = new ReportDataSource("dsTransOrder", dsTOHdr.Tables[0]);
            this.reportViewer1.LocalReport.DataSources.Clear();
            this.reportViewer1.LocalReport.DataSources.Add(rdsHdr);
            ReportDataSource rdsDtl = new ReportDataSource("dsTransOrderDtl", dtCustom);
            this.reportViewer1.LocalReport.DataSources.Add(rdsDtl);

            this.reportViewer1.LocalReport.EnableExternalImages = true;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("STORELOGO", dtStoreLogo));


            ReportParameter[] param = new ReportParameter[4];
            param[0] = new ReportParameter("CompanyName", sCompanyName);
            param[1] = new ReportParameter("StoreAddress", sStoreAddress);
            param[2] = new ReportParameter("StorePhone", sStorePhNo);
            param[3] = new ReportParameter("Title", sTitle);

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }

        private DataTable GetStoreLogo()  // SKU allow
        {
            DataTable dt = new DataTable();
            SqlConnection conn = new SqlConnection();

            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "SELECT 1 AS ID, STOREIMAGE AS LOGOIMG from CRWRETAILSTOREIMG where storenumber='" + ApplicationSettings.Terminal.StoreId + "'";
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            using (SqlCommand command = new SqlCommand(commandText, conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dt;
        }

        private string GetCompanyName()
        {
            string sCName = string.Empty;

            SqlConnection conn = new SqlConnection();

            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string sQry = "SELECT ISNULL(A.NAME,'') FROM DIRPARTYTABLE A INNER JOIN COMPANYINFO B" +
                " ON A.RECID = B.RECID WHERE B.DATAAREA = '" + ApplicationSettings.Database.DATAAREAID + "'";

            //using (SqlCommand cmd = new SqlCommand(sQry, conn))
            //{
            SqlCommand cmd = new SqlCommand(sQry, conn);
            cmd.CommandTimeout = 0;
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            sCName = Convert.ToString(cmd.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            //}

            return sCName;

        }

    }
}
