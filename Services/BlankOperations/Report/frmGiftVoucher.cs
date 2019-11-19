using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.IO;
using BarcodeLib.Barcode;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    public partial class frmGiftVoucher : Form
    {
        string sBC = "";
        string sDate = "";
        string sAmt = "";

        public frmGiftVoucher()
        {
            InitializeComponent();
        }

        public frmGiftVoucher(string sGVNo, string _GVDate, string _sAmt)
        {
            InitializeComponent();

            sBC = sGVNo;
            sDate = _GVDate;
            sAmt = _sAmt;
        }

        private void frmGiftVoucher_Load(object sender, EventArgs e)
        {
            #region BarcodeDatatable
            BarcodeLib.Barcode.Linear codabar = new BarcodeLib.Barcode.Linear();
            codabar.Type = BarcodeType.CODE39;
            codabar.Data = sBC;

            codabar.UOM = UnitOfMeasure.PIXEL;
            codabar.BarColor = System.Drawing.Color.Black;
            codabar.BarWidth = 2;
            codabar.CodabarStartChar = CodabarStartStopChar.C;
            codabar.ImageFormat = System.Drawing.Imaging.ImageFormat.Gif;

            MemoryStream ms = new MemoryStream();
            codabar.drawBarcode(ms);
            Byte[] bitmap01 = null;
            bitmap01 = ms.GetBuffer();

            DataTable dtBarcode = new DataTable();
            dtBarcode.Columns.Add("ID", typeof(int));
            dtBarcode.Columns.Add("BARCODEIMG", typeof(byte[]));
            DataRow dr = dtBarcode.NewRow();
            dr["ID"] = 1;
            dr["BARCODEIMG"] = bitmap01;
            dtBarcode.Rows.Add(dr);
            dr = null;
            #endregion

            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;

            localReport.ReportPath = "rptGVPrint.rdlc";

            ReportDataSource dsBCI = new ReportDataSource();
            dsBCI.Name = "dsBarCode";
            dsBCI.Value = dtBarcode;
            this.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("dsBarCode", dtBarcode));

            ReportParameter[] param;
            param = new ReportParameter[3];
            param[0] = new ReportParameter("GVNumber", sBC);
            param[1] = new ReportParameter("GVAmount", sAmt);
            param[2] = new ReportParameter("GVDate", sDate);

            this.reportViewer1.LocalReport.SetParameters(param);
            this.reportViewer1.RefreshReport();
        }
    }
}
