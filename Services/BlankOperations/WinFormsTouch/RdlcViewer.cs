using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;
using System.Drawing.Printing;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    /// <summary>
    /// Designed By Md Ibrahim Adham Molla aka MIAM
    /// Properties :
    /// ReportPath: holds rdlc report file
    /// ReportDataSources: holds rdlc report data sources
    /// ReportParameters: holds rdlc report parameters
    /// </summary>
    public partial class RdlcViewer : Form
    {
        #region Private Properties
        /// <summary>
        /// holds rdlc report file
        /// </summary>
        private string ReportPath { get; set; }
        /// <summary>
        /// holds rdlc report data sources
        /// </summary>
        private List<ReportDataSource> ReportDataSources { get; set; }
        /// <summary>
        /// holds rdlc report parameters
        /// </summary>
        private List<ReportParameter> ReportParameters { get; set; }
        /// <summary>
        /// holds rdlc report viewer page settings
        /// </summary>
        private PageSettings ReportViewerPageSettings { get; set; }
        #endregion

        #region Initialization
        /// <summary>
        /// Constructor with Datasources
        /// </summary>
        /// <param name="title">Title of Report Viewer</param>
        /// <param name="reportPath">Path of rdlc report file</param>
        /// <param name="rds">Datasources</param>
        /// <param name="ps" default="null">Report Viewer PageSettings</param>
        public RdlcViewer(string title, string reportPath, List<ReportDataSource> rds, PageSettings ps = null)
        {
            InitializeComponent();
            this.Text = title;
            this.ReportPath = reportPath;
            this.ReportDataSources = rds;
            this.ReportViewerPageSettings = ps;
        }
        /// <summary>
        /// Constructor with Datasources and parameters
        /// </summary>
        /// <param name="title">Title of Report Viewer</param>
        /// <param name="reportPath">Path of rdlc report file</param>
        /// <param name="rds">Datasources</param>
        /// <param name="rps">Parameters</param>
        /// <param name="ps" default="null">Report Viewer PageSettings</param>
        public RdlcViewer(string title, string reportPath, List<ReportDataSource> rds, List<ReportParameter> rps, PageSettings ps = null)
        {
            InitializeComponent();
            this.Text = title;
            this.ReportPath = reportPath;
            this.ReportDataSources = rds;
            this.ReportParameters = rps;
            this.ReportViewerPageSettings = ps;
        }
        #endregion

        #region Form Load : View Report
        private void RdlcViewer_Load(object sender, EventArgs e)
        {
            reportViewer1.Reset();
            if (ReportViewerPageSettings != null)
                reportViewer1.SetPageSettings(ReportViewerPageSettings);
            reportViewer1.LocalReport.ReportEmbeddedResource = ReportPath;

            #region Check DataSource Matched
            bool dsMisMatched = false;
            IList<string> dsName = reportViewer1.LocalReport.GetDataSourceNames();
            if (ReportDataSources == null && dsName.Count > 0)
            {
                dsMisMatched = true;
            }
            else
            {
                foreach (ReportDataSource rds in ReportDataSources)
                {
                    if (!dsName.Contains(rds.Name))
                    {
                        dsMisMatched = true;
                        break;
                    }
                }
            }
            if (dsMisMatched)
            {
                Font newFont = new System.Drawing.Font(FontFamily.GenericMonospace, 20, FontStyle.Bold);
                Label msgLabel = new Label { AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = newFont };
                msgLabel.Text = "DataSources mismatched error occurred.";
                reportViewer1.Visible = false;
                this.Controls.Add(msgLabel);
                return;
            }
            #endregion

            #region Check Parameters Matched
            bool paramMisMatched = false;
            ReportParameterInfoCollection paramList = reportViewer1.LocalReport.GetParameters();
            List<string> lstParamList = new List<string>();
            foreach (ReportParameterInfo item in paramList)
            {
                lstParamList.Add(item.Name);
            }

            if (ReportParameters == null && paramList.Count > 0)
            {
                paramMisMatched = true;
            }
            else
            {
                foreach (ReportParameter rps in ReportParameters)
                {
                    if (!lstParamList.Contains(rps.Name))
                    {
                        paramMisMatched = true;
                        break;
                    }
                }
            }
            if (paramMisMatched)
            {
                Font newFont = new System.Drawing.Font(FontFamily.GenericMonospace, 20, FontStyle.Bold);
                Label msgLabel = new Label { AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = newFont };
                msgLabel.Text = "Parameters mismatched error occurred.";
                reportViewer1.Visible = false;
                this.Controls.Add(msgLabel);
                return;
            }
            #endregion

            this.reportViewer1.LocalReport.DataSources.Clear();
            if (ReportDataSources != null)
            {
                foreach (ReportDataSource rds in ReportDataSources)
                {
                    this.reportViewer1.LocalReport.DataSources.Add(rds);
                }
            }

            if (ReportParameters != null)
            {
                foreach (ReportParameter rps in ReportParameters)
                {
                    this.reportViewer1.LocalReport.SetParameters(rps);
                }
            }

            this.reportViewer1.RefreshReport();
        }
        #endregion
    }
}
