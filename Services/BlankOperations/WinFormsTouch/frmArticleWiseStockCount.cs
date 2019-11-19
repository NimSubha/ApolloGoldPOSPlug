using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.ApplicationService;
using System.Reflection;
using Microsoft.Dynamics.Retail.Pos.Dialog;
using Microsoft.Reporting.WinForms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmArticleWiseStockCount:frmTouchBase
    {
        public IPosTransaction pos { get; set; }

        [Import]
        private IApplication application;
        DataTable dtFetchedData = new DataTable();
        bool IsEdit = false;
        int EditselectedIndex = 0;
        int isCW = 0;
        bool bSearchMode = false;

        enum CRWStockType
        {
            SKU = 0,
            Bulk = 1,
        }
        #region enum MetalType
        public enum MetalType
        {
            Other = 0,
            Gold = 1,
            Silver = 2,
            Platinum = 3,
            Alloy = 4,
            Diamond = 5,
            Pearl = 6,
            Stone = 7,
            Consumables = 8,
            Watch = 11,
            LooseDmd = 12,
            Palladium = 13,
            Jewellery = 14,
            Metal = 15,
            PackingMaterial = 16,
            Certificate = 17,
            GiftVoucher = 18,
        }
        #endregion

        enum CRWStockTakingType
        {
            Daily = 0,
            Monthly = 1,
        }
        enum ReceiptTransactionType
        {
            ArticleWiseStockVoucher = 15
        }


        public frmArticleWiseStockCount()
        {
            InitializeComponent();
        }

        public frmArticleWiseStockCount(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();

            BindCombo();
            pos = posTransaction;
            application = Application;
            string sVoucher = GetStockVoucher();
            txtVoucherNo.Text = sVoucher;
            lblEditLine.Visible = false;
            btnAdd.Enabled = false;
            LoadProductType();
            LoadArticle();
            LoadSubArticle();
            btnRePrint.Enabled = false;
            if(IsEdit == false)
            {
                txtPCS.Enabled = false;
                txtQuantity.Enabled = false;
                txtGrossWt.Enabled = false;
                txtRemarks.Enabled = false;
                btnSearchCountedBy.Enabled = false;
            }
        }

        private string GetStockVoucher()
        {
            string TransId = string.Empty;
            TransId = GetNextStockVoucher();
            return TransId;
        }

        public string GetNextStockVoucher()
        {
            try
            {
                ReceiptTransactionType transType = ReceiptTransactionType.ArticleWiseStockVoucher;
                string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId;
                string terminalId = LSRetailPosis.Settings.ApplicationSettings.Terminal.TerminalId;
                string staffId = pos.OperatorId;
                string mask;

                string funcProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
                transactionNumber((int)transType, funcProfileId, out mask);
                if(string.IsNullOrEmpty(mask))
                    return string.Empty;
                else
                {
                    string seedValue = GetSeedVal().ToString();
                    return ReceiptMaskFiller.FillMask(mask, seedValue, storeId, terminalId, staffId);
                }

            }
            catch(Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        private string GetSeedVal()
        {
            string sFuncProfileId = LSRetailPosis.Settings.FunctionalityProfiles.Functions.ProfileId;
            int iTransType = (int)ReceiptTransactionType.ArticleWiseStockVoucher;

            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = "DECLARE @VAL AS INT  SELECT @VAL = CHARINDEX('#',mask) FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID ='" + sFuncProfileId + "'  AND RECEIPTTRANSTYPE = " + iTransType + " " +
                                     " SELECT  MAX(CAST(ISNULL(SUBSTRING(StockVoucher,@VAL,LEN(StockVoucher)),0) AS INTEGER)) + 1 from CRWArticalStockTakingTable";

                using(SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if(conn.State != ConnectionState.Open)
                        conn.Open();
                    Val = Convert.ToString(command.ExecuteScalar());
                    if(string.IsNullOrEmpty(Val))
                        Val = "1";


                    return Val;

                }
            }
            finally
            {
                if(conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private void transactionNumber(int transType, string funcProfile, out string mask)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string Val = string.Empty;
            try
            {
                string queryString = " SELECT MASK FROM RETAILRECEIPTMASKS WHERE FUNCPROFILEID='" + funcProfile.Trim() + "' " +
                                     " AND RECEIPTTRANSTYPE=" + transType;
                using(SqlCommand command = new SqlCommand(queryString, conn))
                {
                    if(conn.State != ConnectionState.Open)
                    {
                        conn.Open();
                    }
                    Val = Convert.ToString(command.ExecuteScalar());
                    mask = Val;
                }
            }
            finally
            {
                if(conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }

        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        void BindCombo()
        {
            cmbStockTakingType.DataSource = Enum.GetValues(typeof(CRWStockTakingType));
            cmbStockType.DataSource = Enum.GetValues(typeof(CRWStockType));
        }

        private void LoadProductType()
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine(" select DISTINCT PRODUCTTYPECODE from INVENTTABLE where PRODUCTTYPECODE!=''");

            DataTable dtProductType = new DataTable();
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            using(SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection))
            {
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        dtProductType.Load(reader);
                    }
                    reader.Close();
                    reader.Dispose();
                }
                cmd.Dispose();
            }

            if(connection.State == ConnectionState.Open)
                connection.Close();

            if(dtProductType.Rows.Count > 0)
            {
                cmbProductType.DataSource = dtProductType;
                cmbProductType.DisplayMember = "PRODUCTTYPECODE";
                cmbProductType.ValueMember = "PRODUCTTYPECODE";
            }
        }

        private void LoadArticle()
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine("select DISTINCT ARTICLE_CODE   from INVENTTABLE where ARTICLE_CODE !=''");// 

            DataTable dtArticle = new DataTable();
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            using(SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection))
            {
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        dtArticle.Load(reader);
                    }
                    reader.Close();
                    reader.Dispose();
                }
                cmd.Dispose();
            }

            if(connection.State == ConnectionState.Open)
                connection.Close();

            if(dtArticle.Rows.Count > 0)
            {
                cCmbArticle.Properties.DataSource = dtArticle;
                cCmbArticle.Properties.DisplayMember = "ARTICLE_CODE";
                cCmbArticle.Properties.ValueMember = "ARTICLE_CODE";
            }
        }

        private void LoadSubArticle()
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.AppendLine(" select DISTINCT BASEDESIGNCODE from INVENTTABLE ");

            DataTable dtSubArticle = new DataTable();
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            using(SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection))
            {
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    if(reader.HasRows)
                    {
                        dtSubArticle.Load(reader);
                    }
                    reader.Close();
                    reader.Dispose();
                }
                cmd.Dispose();
            }

            if(connection.State == ConnectionState.Open)
                connection.Close();

            if(dtSubArticle.Rows.Count > 0)
            {
                cCmbSubArticle.Properties.DataSource = dtSubArticle;
                cCmbSubArticle.Properties.DisplayMember = "BASEDESIGNCODE";
                cCmbSubArticle.Properties.ValueMember = "BASEDESIGNCODE";
            }
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            if(ValidateControls())
            {
                if(IsEdit == false)
                {
                    string displayArticleValues = cCmbArticle.Properties.GetDisplayText(cCmbArticle.EditValue);
                    string displaySubArticleValues = cCmbSubArticle.Properties.GetDisplayText(cCmbSubArticle.EditValue);
                    string displayProductValues = cmbProductType.Text;

                    List<string> lArticlevals = displayArticleValues.Split(cCmbArticle.Properties.SeparatorChar).ToList();
                    List<string> lSubArticlevals = displaySubArticleValues.Split(cCmbSubArticle.Properties.SeparatorChar).ToList();
                   // List<string> lProductvals = displayProductValues.Split(cmbProductType.Properties.SeparatorChar).ToList();

                    string sArticl = string.Format("'{0}'", string.Join("','", lArticlevals.Select(i => i.Replace(" ", "")).ToArray()));
                    string sSubArticle = string.Format("'{0}'", string.Join("','", lSubArticlevals.Select(i => i.Replace(" ", "")).ToArray()));
                    //string sProduct = string.Format("'{0}'", string.Join("','", lProductvals.Select(i => i.Replace(" ", "")).ToArray()));

                    if(cmbStockType.SelectedIndex == (int)CRWStockType.SKU)
                    {
                        string sSql = "";
                        if (cmbStockTakingType.SelectedIndex == (int)CRWStockTakingType.Monthly)
                        {
                             sSql = "SELECT ARTICLE as [Article Code],b.DESCRIPTION as [Article Name]," +
                                       " SUBARTICLE,bd.DESCRIPTION as [Sub Article Name],METALTYPE, " +
                                       " SUM(PDSCWQTY) [On Hand CW], SUM(QTY) [On Hand Qty] ," +
                                // " SUM(GrossWeight) [On Hand Gross Wt]," +
                                       " '' [Physical CW], '' [Physical Qty] ," +
                                       " SUM(BarcodeLabelWeight) [Barcode label weight]," +
                                       " SUM(PDSCWQTY) [Difference CW], (SUM(QTY) + SUM(BarcodeLabelWeight)) [Difference Qty] ," + //'' [Difference Gross Wt] ,
                                       " '' [Counted By], '' [Name] ,'' [Remarks],ISCW " +
                                       "  FROM [VIEWARTICLEWISESTOCKFORSKU] a" +
                                       " left join ARTICLE_MASTER b on a.ARTICLE =b.ARTICLE_CODE " +
                                       " left join BASE_DESIGN_MASTER BD on a.SUBARTICLE = bd.BASEDESIGNCODE" +
                                       " where PCODE in ('" + displayProductValues + "') AND ARTICLE in (" + sArticl + ") AND SUBARTICLE in (" + sSubArticle + ")" +
                                       "  GROUP BY ISCW, ARTICLE ,b.DESCRIPTION,SUBARTICLE,bd.DESCRIPTION ,METALTYPE";
                        }
                        else
                        {
                             sSql = "SELECT ARTICLE as [Article Code],b.DESCRIPTION as [Article Name]," +
                                        " SUBARTICLE,bd.DESCRIPTION as [Sub Article Name],METALTYPE, " +
                                        " SUM(PDSCWQTY) [On Hand CW], 0 as [On Hand Qty] ," +
                                // " SUM(GrossWeight) [On Hand Gross Wt]," +
                                        " '' [Physical CW], 0 [Physical Qty] ," +
                                        " 0 [Barcode label weight]," +
                                        " SUM(PDSCWQTY) [Difference CW], 0 [Difference Qty] ," + //'' [Difference Gross Wt] ,
                                        " '' [Counted By], '' [Name] ,'' [Remarks],ISCW " +
                                        "  FROM [VIEWARTICLEWISESTOCKFORSKU] a" +
                                        " left join ARTICLE_MASTER b on a.ARTICLE =b.ARTICLE_CODE " +
                                        " left join BASE_DESIGN_MASTER BD on a.SUBARTICLE = bd.BASEDESIGNCODE" +
                                        " where PCODE in ('" + displayProductValues + "') AND ARTICLE in (" + sArticl + ") AND SUBARTICLE in (" + sSubArticle + ")" +
                                        "  GROUP BY ISCW, ARTICLE ,b.DESCRIPTION,SUBARTICLE,bd.DESCRIPTION ,METALTYPE";
                        }

                        dtFetchedData = GetData(sSql);
                    }
                    else
                    {
                        string sSql = "SELECT ARTICLE as [Article Code],b.DESCRIPTION as [Article Name]," +
                                    " SUBARTICLE,bd.DESCRIPTION as [Sub Article Name],METALTYPE, " +
                                    " SUM(PDSCWQTY) [On Hand CW], SUM(QTY) [On Hand Qty] ," +
                                   // " SUM(GrossWeight) [On Hand Gross Wt]," +
                                    " '' [Physical CW], '' [Physical Qty] ," +
                                   // " '' [Physical Gross Wt]," +
                                    " SUM(BarcodeLabelWeight) [Barcode label weight]," +
                                    " SUM(PDSCWQTY) [Difference CW], SUM(QTY) [Difference Qty] ," +//'' [Difference Gross Wt] ,
                                    " '' [Counted By], '' [Name] ,'' [Remarks] ,ISCW" +
                                    "  FROM [VIEWARTICLEWISESTOCKFORBULK] a" +
                                    " left join ARTICLE_MASTER b on a.ARTICLE =b.ARTICLE_CODE " +
                                    " left join BASE_DESIGN_MASTER BD on a.SUBARTICLE = bd.BASEDESIGNCODE" +
                                    " where PCODE in ('" + displayProductValues + "') AND ARTICLE in (" + sArticl + ") AND SUBARTICLE in (" + sSubArticle + ")" +
                                    "  GROUP BY ISCW, ARTICLE ,b.DESCRIPTION,SUBARTICLE,bd.DESCRIPTION ,METALTYPE";

                        dtFetchedData = GetData(sSql);
                    }

                    if(dtFetchedData != null && dtFetchedData.Rows.Count > 0)
                    {
                        gridStock.DataSource = dtFetchedData;

                        GetTotalOfItemList();

                        cCmbArticle.Enabled = false;
                        cmbProductType.Enabled = false;
                        cCmbSubArticle.Enabled = false;
                        cmbStockTakingType.Enabled = false;
                        cmbStockType.Enabled = false;

                        btnAdd.Enabled = true;
                    }
                    else
                    {
                        cCmbArticle.Enabled = true;
                        cmbProductType.Enabled = true;
                        cCmbSubArticle.Enabled = true;
                        cmbStockTakingType.Enabled = true;
                        cmbStockType.Enabled = true;

                        btnAdd.Enabled = false;
                    }
                }

            }

        }

        private void GetTotalOfItemList()
        {
            //Start : added on 26/05/2014
            int iOHC = 0;
            decimal dOHQ = 0m;
            decimal dOHG = 0m;
            int iPC = 0;
            decimal dPQ = 0m;
            decimal dPGW = 0m;
            int iDC = 0;
            decimal dDPQ = 0m;
            decimal dDPGQ = 0m;
            decimal dBLW = 0m;   //" SUM(BarcodeLabelWeight) [Barcode label weight]," +

            foreach(DataRow dr in dtFetchedData.Rows)
            {
                iOHC = iOHC + Convert.ToInt32(dr["On Hand CW"]);
                dOHQ = dOHQ + Convert.ToDecimal(dr["On Hand Qty"]);
                //dOHG = dOHG + Convert.ToDecimal(dr[4]);

                if(!string.IsNullOrEmpty(Convert.ToString(dr["Physical CW"])))
                    iPC = iPC + Convert.ToInt32(dr["Physical CW"]);
                if(!string.IsNullOrEmpty(Convert.ToString(dr["Physical Qty"])))
                    dPQ = dPQ + Convert.ToDecimal(dr["Physical Qty"]);
                //dPGW = dPGW + Convert.ToDecimal(dr[7]);
                if(!string.IsNullOrEmpty(Convert.ToString(dr["Difference CW"])))
                    iDC = iDC + Convert.ToInt32(dr["Difference CW"]);

                if(!string.IsNullOrEmpty(Convert.ToString(dr["Difference Qty"])))
                    dDPQ = dDPQ + Convert.ToDecimal(dr["Difference Qty"]);

                if(!string.IsNullOrEmpty(Convert.ToString(dr["Barcode label weight"])))
                    dBLW = dBLW + Convert.ToDecimal(dr["Barcode label weight"]);
                //dDPGQ = dDPGQ + Convert.ToDecimal(dr[10]);
            }

            lblOHC.Text = Convert.ToString(iOHC);
            lblOHQ.Text = Convert.ToString(dOHQ);
           // lblOHGQ.Text = Convert.ToString(dOHG);

            lblPC.Text = Convert.ToString(iPC);
            lblPQ.Text = Convert.ToString(dPQ);
            //lblPGW.Text = Convert.ToString(dPGW);

            lblDC.Text = Convert.ToString(iDC);
            lblDQ.Text = Convert.ToString(dDPQ);
            //lblDGW.Text = Convert.ToString(dDPGQ);

            lblBLW.Text = Convert.ToString(dBLW);
            //End : added on 26/05/2014
        }

        private DataTable GetData(string sSql)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            DataTable dt = new DataTable();
            ;
            if(conn.State == ConnectionState.Closed)
                conn.Open();

            using(SqlCommand command = new SqlCommand(sSql, conn))
            {
                SqlDataAdapter da = new SqlDataAdapter(command);
                da.Fill(dt);
            }
            if(conn.State == ConnectionState.Open)
                conn.Close();

            return dt;
        }

        bool ValidateControls()
        {
            if(string.IsNullOrEmpty(cCmbArticle.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Article code can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    cCmbArticle.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if(string.IsNullOrEmpty(cmbProductType.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Product type can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    cmbProductType.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if(string.IsNullOrEmpty(cCmbSubArticle.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Sub article can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    cCmbSubArticle.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        bool ValidateControlsAtEdit()
        {
            //if(cmbStockTakingType.SelectedIndex == (int)CRWStockTakingType.Daily)
            //{
                if(txtPCS.Enabled)
                {
                    if(string.IsNullOrEmpty(txtPCS.Text.Trim()))
                    {
                        using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Physical CW quantity can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            txtPCS.Focus();
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            return false;
                        }
                    }
                }
                if(txtQuantity.Enabled)
                {
                    if(string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                    {
                        using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Physical quantity can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            txtQuantity.Focus();
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            return false;
                        }
                    }
                }
                //if(isCW == 1)
                //{
                //    if(string.IsNullOrEmpty(txtPCS.Text.Trim()))
                //    {
                //        using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Physical CW quantity can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                //        {
                //            txtPCS.Focus();
                //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                //            return false;
                //        }
                //    }
                //}
                //else
                //{
                //    if(string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                //    {
                //        using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Physical quantity can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                //        {
                //            txtQuantity.Focus();
                //            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                //            return false;
                //        }
                //    }
                //}
           // }
            //else
            //{
            //    if(isCW == 1)
            //    {
            //        if(string.IsNullOrEmpty(txtPCS.Text.Trim()))
            //        {
            //            using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Physical CW quantity can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //            {
            //                txtPCS.Focus();
            //                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //                return false;
            //            }
            //        }
            //        if(string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            //        {
            //            using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Physical quantity can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //            {
            //                txtQuantity.Focus();
            //                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //                return false;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        if(string.IsNullOrEmpty(txtPCS.Text.Trim()))
            //        {
            //            using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Physical quantity can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //            {
            //                txtPCS.Focus();
            //                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //                return false;
            //            }
            //        }
            //        if(string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            //        {
            //            using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Physical quantity can not be blank or empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //            {
            //                txtQuantity.Focus();
            //                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //                return false;
            //            }
            //        }
            //    }
            //}
            if(string.IsNullOrEmpty(txtCountedBy.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Counted by can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnSearchCountedBy.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            else
            {
                return true;
            }
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            bSearchMode = false;
            ClearControll();
            dtFetchedData.Clear();

            cCmbArticle.Enabled = true;
            cmbProductType.Enabled = true;
            cCmbSubArticle.Enabled = true;
            cmbStockTakingType.Enabled = true;
            cmbStockType.Enabled = true;
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsEdit = false;
            if(dtFetchedData != null && dtFetchedData.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    IsEdit = true;

                    EditselectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToSelect = dtFetchedData.Rows[EditselectedIndex];
                    txtPCS.Text = Convert.ToString(theRowToSelect["Physical CW"]);
                    txtQuantity.Text = Convert.ToString(theRowToSelect["Physical Qty"]);
                   // txtGrossWt.Text = Convert.ToString(theRowToSelect["Physical Gross Wt"]);
                    txtCountedBy.Text = Convert.ToString(theRowToSelect["Counted By"]);
                    txtCountedBy.Tag = Convert.ToString(theRowToSelect["Name"]);
                    txtRemarks.Text = Convert.ToString(theRowToSelect["Remarks"]);
                    isCW = Convert.ToInt16(theRowToSelect["ISCW"]);

                    if(IsEdit == true)
                    {
                        lblEditLine.Visible = true;
                        txtPCS.Enabled = true;
                        txtQuantity.Enabled = true;
                        if (cmbStockType.SelectedIndex == (int)CRWStockType.SKU)
                        {
                            if (cmbStockTakingType.SelectedIndex == (int)CRWStockTakingType.Daily)
                            {
                                txtPCS.Enabled = true;
                                txtQuantity.Enabled = false;
                            }
                            else
                            {
                                txtPCS.Enabled = true;
                                txtQuantity.Enabled = true;
                            }
                        }
                        /*if(cmbStockTakingType.SelectedIndex == (int)CRWStockTakingType.Daily) // commented on 19/09/17 req by S.Sharma
                        {
                            txtPCS.Enabled = true;
                            txtQuantity.Enabled = false;
                            //if(isCW == 1)
                            //{
                            //    txtPCS.Enabled = true;
                            //    txtQuantity.Enabled = false;
                            //}
                            //else
                            //{
                            //    txtQuantity.Enabled = true;
                            //    txtPCS.Enabled = false;
                            //}
                            //txtGrossWt.Enabled = false;
                        }
                        else
                        {
                            txtPCS.Enabled = true;
                            txtQuantity.Enabled = true;
                            //if(isCW == 1)
                            //{
                            //    txtPCS.Enabled = true;
                            //    txtQuantity.Enabled = true;
                            //    txtGrossWt.Enabled = false;
                            //}
                            //else
                            //{
                            //    txtPCS.Enabled = false;
                            //    txtQuantity.Enabled = true;
                            //    txtGrossWt.Enabled = true;
                            //}
                        }*/
                        btnAdd.Enabled = true;
                        txtRemarks.Enabled = true;
                        btnSearchCountedBy.Enabled = true;
                        btnFetch.Enabled = false;
                    }

                    lblEditLine.Text = "Selected line for edit is  : " + "" + (EditselectedIndex + 1);
                }
            }
        }

        private void simpleButtonEx1_Click(object sender, EventArgs e)
        {
            LSRetailPosis.POSProcesses.frmSalesPerson dialog = new LSRetailPosis.POSProcesses.frmSalesPerson();
            dialog.ShowDialog();
            txtCountedBy.Text = dialog.SelectedEmployeeId;
            txtCountedBy.Tag = dialog.SelectEmployeeName;
        }

        private void ClearControll()
        {
            txtPCS.Enabled = false;
            txtQuantity.Enabled = false;
            txtGrossWt.Enabled = false;
            txtRemarks.Enabled = false;
            btnRePrint.Enabled = false;

            btnSearchCountedBy.Enabled = false;
            txtPCS.Text = "";
            txtQuantity.Text = "";
            txtGrossWt.Text = "";
            txtRemarks.Text = "";
            txtCountedBy.Text = "";
            lblEditLine.Visible = false;

            lblOHC.Text = "";
            lblOHQ.Text = "";
            //lblOHGQ.Text = "";

            lblPC.Text = "";
            lblPQ.Text = "";
            //lblPGW.Text = "";

            lblDC.Text = "";
            lblDQ.Text = "";
            //lblDGW.Text = "";
            btnAdd.Enabled = false;

            if(bSearchMode == true)
            {
                panel1.Enabled = false;
                btnFetch.Enabled = false;
                btnCommit.Enabled = false;
                btnEdit.Enabled = false;
            }
            else
            {
                panel1.Enabled = true;
                btnFetch.Enabled = true;
                btnCommit.Enabled = true;
                btnEdit.Enabled = true;
            }
        }

        private void btnCommit_Click(object sender, EventArgs e)
        {
            SaveData();
            string sVoucher = GetStockVoucher();
            txtVoucherNo.Text = sVoucher;
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if(ValidateControlsAtEdit())
            {
                if(IsEdit == true && dtFetchedData != null && dtFetchedData.Rows.Count > 0)
                {
                    DataRow EditRow = dtFetchedData.Rows[EditselectedIndex];
                    EditRow["Physical CW"] = txtPCS.Text.Trim();
                    EditRow["Physical Qty"] = txtQuantity.Text.Trim();
                   // EditRow["Physical Gross Wt"] = txtGrossWt.Text.Trim();
                    EditRow["Counted By"] = txtCountedBy.Text.Trim();
                    EditRow["Name"] = txtCountedBy.Tag;
                    EditRow["Remarks"] = txtRemarks.Text.Trim();

                    decimal dOnHandCW = 0;
                    decimal dOnHandQty = 0;
                   // decimal dOnHandGrossWt = 0;

                    decimal dPhyCW = 0;
                    decimal dPhyQty = 0;
                   // decimal dPhyGrossWt = 0;
                    decimal dBLW = 0m;

                    if(!string.IsNullOrEmpty(Convert.ToString(EditRow["On Hand CW"])))
                        dOnHandCW = Convert.ToDecimal(EditRow["On Hand CW"]);

                    if(!string.IsNullOrEmpty(Convert.ToString(EditRow["On Hand Qty"])))
                        dOnHandQty = Convert.ToDecimal(EditRow["On Hand Qty"]);

                    //if(!string.IsNullOrEmpty(Convert.ToString(EditRow["On Hand Gross Wt"])))
                    //    dOnHandGrossWt = Convert.ToDecimal(EditRow["On Hand Gross Wt"]);

                    if(!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                        dPhyCW = Convert.ToDecimal(txtPCS.Text.Trim());

                    if(!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        dPhyQty = Convert.ToDecimal(txtQuantity.Text.Trim());

                    //if(!string.IsNullOrEmpty(txtGrossWt.Text.Trim()))
                    //    dPhyGrossWt = Convert.ToDecimal(txtGrossWt.Text.Trim());

                    if(!string.IsNullOrEmpty(Convert.ToString(EditRow["Barcode label weight"])))
                        dBLW = Convert.ToDecimal(EditRow["Barcode label weight"]);

                    if(cmbStockTakingType.SelectedIndex == (int)CRWStockTakingType.Monthly)
                    {
                        EditRow["Difference Qty"] = Convert.ToString(decimal.Round(Convert.ToDecimal(dPhyQty-(dOnHandQty + dBLW)), 3, MidpointRounding.AwayFromZero));
                       // EditRow["Difference Gross Wt"] = Convert.ToString(decimal.Round(Convert.ToDecimal(dOnHandGrossWt - dPhyGrossWt), 3, MidpointRounding.AwayFromZero));
                    }

                    EditRow["Difference CW"] = Convert.ToString(decimal.Round(Convert.ToDecimal(dPhyCW-dOnHandCW), 0, MidpointRounding.AwayFromZero));

                    txtRemarks.Enabled = true;
                    btnSearchCountedBy.Enabled = true;
                    btnFetch.Enabled = false;

                    dtFetchedData.AcceptChanges();
                    gridStock.DataSource = dtFetchedData.DefaultView;

                    GetTotalOfItemList();

                    ClearControll();
                }
            }
        }

        private void SaveData()
        {
            int iHeader = 0;
            int iDetails = 0;
            string sInventSiteId = GetInventSiteId();

            SqlTransaction transaction = null;

            #region  HEADER
            string commandText = " INSERT INTO [CRWArticalStockTakingTable]([DATAAREAID],[STOCKVOUCHER]," +
                                 " [TRANSDATE],[INVENTLOCATIONID],[STOCKTAKINGTYPE]," +
                                 " [STOCKTYPE],[PRODUCTTYPE],[ARTICALCODE],[SUBARTICALCODE] " +
                                 " ,[CONFIRMED],[INVENTSITEID],[CONFIRMDATE],[RETAIL])" +
                                 " VALUES(@DATAAREAID,@STOCKVOUCHER,@TRANSDATE,@INVENTLOCATIONID,@STOCKTAKINGTYPE,@STOCKTYPE,@PRODUCTTYPE," +
                                 " @ARTICALCODE,@SUBARTICALCODE,@CONFIRMED,@INVENTSITEID,@CONFIRMDATE,@RETAIL)";
            SqlConnection connection = new SqlConnection();
            try
            {
                if(application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if(connection.State == ConnectionState.Closed)
                    connection.Open();

                transaction = connection.BeginTransaction();

                SqlCommand command = new SqlCommand(commandText, connection, transaction);
                command.Parameters.Clear();
                if(application != null)
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                else
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                command.Parameters.Add("@STOCKVOUCHER", SqlDbType.NVarChar).Value = txtVoucherNo.Text.Trim();
                command.Parameters.Add("@TRANSDATE", SqlDbType.DateTime).Value = dtpTransDate.Value;
                command.Parameters.Add("@INVENTLOCATIONID", SqlDbType.NVarChar, 10).Value = ApplicationSettings.Terminal.StoreId;
                command.Parameters.Add("@STOCKTAKINGTYPE", SqlDbType.Int).Value = cmbStockTakingType.SelectedIndex;
                command.Parameters.Add("@STOCKTYPE", SqlDbType.Int).Value = cmbStockType.SelectedIndex;
                command.Parameters.Add("@PRODUCTTYPE", SqlDbType.NVarChar, 1000).Value = cmbProductType.Text;
                command.Parameters.Add("@ARTICALCODE", SqlDbType.NVarChar, 1000).Value = cCmbArticle.Text;
                command.Parameters.Add("@SUBARTICALCODE", SqlDbType.NVarChar, 1000).Value = cCmbSubArticle.Text;
                command.Parameters.Add("@CONFIRMED", SqlDbType.Int).Value = 1;
                command.Parameters.Add("@INVENTSITEID", SqlDbType.NVarChar, 10).Value = sInventSiteId;
                command.Parameters.Add("@CONFIRMDATE", SqlDbType.DateTime).Value = dtpTransDate.Value;
                command.Parameters.Add("@RETAIL", SqlDbType.Int).Value = 1;

            #endregion

                command.CommandTimeout = 0;
                iHeader = command.ExecuteNonQuery();

                if(iHeader == 1)
                {
                    if(dtFetchedData != null && dtFetchedData.Rows.Count > 0)
                    {
                        #region  DETAILS
                        string commandDetail = " INSERT INTO [CRWArticalStockTakingLine]([STOCKVOUCHER],[ARTICLE_CODE],[ARTICLEDESCRIPTION]" +
                                                 " ,[ONHANDCW],[ONHANDQTY],[ONHANDGROSSQTY] " +
                                                 " ,[PHYSICALCW],[PHYSICALQTY],[PHYSICALGROSSQTY],"+
                                                 " [DIFFERENCECW],[DIFFERENCEQTY],[DIFFERENCEGROSSQTY]" +
                                                 " ,[COUNTEDBY],[REMARKS],[LINENUM],[STOCKTAKINGTYPE],"+
                                                 " [CATCHWEIGHT],[STOCKTYPE],[DATAAREAID],BarcodeLabelWeight,SUBARTICLE,METALTYPE )" +
                                                 " VALUES(@STOCKVOUCHER,@ARTICLE_CODE,@ARTICLEDESCRIPTION,@ONHANDCW,@ONHANDQTY,@ONHANDGROSSQTY" +
                                                 " ,@PHYSICALCW ,@PHYSICALQTY,@PHYSICALGROSSQTY ,@DIFFERENCECW ,@DIFFERENCEQTY,@DIFFERENCEGROSSQTY" +
                                                 " ,@COUNTEDBY  ,@REMARKS , @LINENUM, @STOCKTAKINGTYPE " +
                                                 " ,@CATCHWEIGHT,@STOCKTYPE,@DATAAREAID,@BarcodeLabelWeight,@SUBARTICLE,@METALTYPE )";

                        for(int ItemCount = 0; ItemCount < dtFetchedData.Rows.Count; ItemCount++)
                        {
                            SqlCommand cmdDetail = new SqlCommand(commandDetail, connection, transaction);
                            cmdDetail.Parameters.Add("@STOCKVOUCHER", SqlDbType.NVarChar, 20).Value = txtVoucherNo.Text;
                            cmdDetail.Parameters.Add("@ARTICLE_CODE", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtFetchedData.Rows[ItemCount]["Article Code"]);

                            cmdDetail.Parameters.Add("@ARTICLEDESCRIPTION", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtFetchedData.Rows[ItemCount]["Article Name"]);
                            cmdDetail.Parameters.Add("@ONHANDCW", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtFetchedData.Rows[ItemCount]["On Hand CW"]);
                            cmdDetail.Parameters.Add("@ONHANDQTY", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtFetchedData.Rows[ItemCount]["On Hand Qty"]);
                            cmdDetail.Parameters.Add("@ONHANDGROSSQTY", SqlDbType.NVarChar, 20).Value = 0;//Convert.ToString(dtFetchedData.Rows[ItemCount]["On Hand Gross Wt"])
                            if(string.IsNullOrEmpty(Convert.ToString(dtFetchedData.Rows[ItemCount]["Physical CW"])))
                                cmdDetail.Parameters.Add("@PHYSICALCW", SqlDbType.Decimal).Value = 0;
                            else
                                cmdDetail.Parameters.Add("@PHYSICALCW", SqlDbType.Decimal).Value = Convert.ToDecimal(dtFetchedData.Rows[ItemCount]["Physical CW"]);

                            if(string.IsNullOrEmpty(Convert.ToString(dtFetchedData.Rows[ItemCount]["Physical Qty"])))
                                cmdDetail.Parameters.Add("@PHYSICALQTY", SqlDbType.Decimal).Value = 0;
                            else
                                cmdDetail.Parameters.Add("@PHYSICALQTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtFetchedData.Rows[ItemCount]["Physical Qty"]);

                           // if(string.IsNullOrEmpty(Convert.ToString(dtFetchedData.Rows[ItemCount]["Physical Gross Wt"])))
                                cmdDetail.Parameters.Add("@PHYSICALGROSSQTY", SqlDbType.Decimal).Value = 0;
                            //else
                            //    cmdDetail.Parameters.Add("@PHYSICALGROSSQTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtFetchedData.Rows[ItemCount]["Physical Gross Wt"]);

                            if(string.IsNullOrEmpty(Convert.ToString(dtFetchedData.Rows[ItemCount]["Difference CW"])))
                                cmdDetail.Parameters.Add("@DIFFERENCECW", SqlDbType.Decimal).Value = 0;
                            else
                                cmdDetail.Parameters.Add("@DIFFERENCECW", SqlDbType.Decimal).Value = Convert.ToDecimal(dtFetchedData.Rows[ItemCount]["Difference CW"]);
                            if(string.IsNullOrEmpty(Convert.ToString(dtFetchedData.Rows[ItemCount]["Difference Qty"])))
                                cmdDetail.Parameters.Add("@DIFFERENCEQTY", SqlDbType.Decimal).Value = 0;
                            else
                                cmdDetail.Parameters.Add("@DIFFERENCEQTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtFetchedData.Rows[ItemCount]["Difference Qty"]);

                          //  if(string.IsNullOrEmpty(Convert.ToString(dtFetchedData.Rows[ItemCount]["Difference Gross Wt"])))
                                cmdDetail.Parameters.Add("@DIFFERENCEGROSSQTY", SqlDbType.Decimal).Value = 0;
                            //else
                            //    cmdDetail.Parameters.Add("@DIFFERENCEGROSSQTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtFetchedData.Rows[ItemCount]["Difference Gross Wt"]);

                            cmdDetail.Parameters.Add("@COUNTEDBY", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtFetchedData.Rows[ItemCount]["Counted By"]);

                            if(string.IsNullOrEmpty(Convert.ToString(dtFetchedData.Rows[ItemCount]["Remarks"])))
                                cmdDetail.Parameters.Add("@REMARKS", SqlDbType.NVarChar, 255).Value = "";
                            else
                                cmdDetail.Parameters.Add("@REMARKS", SqlDbType.NVarChar, 255).Value = Convert.ToString(dtFetchedData.Rows[ItemCount]["Remarks"]);

                            cmdDetail.Parameters.Add("@LINENUM", SqlDbType.Int).Value = ItemCount + 1;

                            cmdDetail.Parameters.Add("@STOCKTAKINGTYPE", SqlDbType.Int).Value = cmbStockTakingType.SelectedIndex;

                            if(string.IsNullOrEmpty(Convert.ToString(dtFetchedData.Rows[ItemCount]["On Hand CW"])))
                                cmdDetail.Parameters.Add("@CATCHWEIGHT", SqlDbType.Decimal).Value = 0;
                            else
                                cmdDetail.Parameters.Add("@CATCHWEIGHT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtFetchedData.Rows[ItemCount]["On Hand CW"]);

                            cmdDetail.Parameters.Add("@STOCKTYPE", SqlDbType.Int).Value = cmbStockType.SelectedIndex;

                            if(application != null)
                                cmdDetail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = application.Settings.Database.DataAreaID;
                            else
                                cmdDetail.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;


                            if(string.IsNullOrEmpty(Convert.ToString(dtFetchedData.Rows[ItemCount]["Barcode label weight"])))
                                cmdDetail.Parameters.Add("@BarcodeLabelWeight", SqlDbType.Decimal).Value = 0;
                            else
                                cmdDetail.Parameters.Add("@BarcodeLabelWeight", SqlDbType.Decimal).Value = Convert.ToDecimal(dtFetchedData.Rows[ItemCount]["Barcode label weight"]);

                            cmdDetail.Parameters.Add("@SUBARTICLE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtFetchedData.Rows[ItemCount]["SUBARTICLE"]);

                            cmdDetail.Parameters.Add("@METALTYPE", SqlDbType.Int).Value = (int)((MetalType)Enum.Parse(typeof(MetalType), Convert.ToString(dtFetchedData.Rows[ItemCount]["METALTYPE"]))); //Convert.ToInt16(dtFetchedData.Rows[ItemCount]["METALTYPE"]);

                            cmdDetail.CommandTimeout = 0;
                            iDetails = cmdDetail.ExecuteNonQuery();
                            cmdDetail.Dispose();
                        }
                        #endregion
                    }
                }
                transaction.Commit();
                command.Dispose();
                transaction.Dispose();

                if(iHeader == 1 || iDetails != 0)
                {
                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Save successfully done.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        try
                        {
                            PrintVoucher();
                        }
                        catch { }

                        ClearControll();
                        dtFetchedData.Clear();

                        cCmbArticle.Enabled = true;
                        cmbProductType.Enabled = true;
                        cCmbSubArticle.Enabled = true;
                        cmbStockTakingType.Enabled = true;
                        cmbStockType.Enabled = true;
                    }
                }
                else
                {
                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("DataBase error occured.Please try again later.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                }
            }
            catch(Exception ex)
            {
                transaction.Rollback();
                transaction.Dispose();

                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(ex.Message.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                }

            }
            finally
            {
                if(connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

        private string GetInventSiteId()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select isnull(INVENTSITEID,'') INVENTSITEID from INVENTLOCATION   where INVENTLOCATIONID='" + ApplicationSettings.Database.StoreID + "'");

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();
            if(!string.IsNullOrEmpty(sResult))
                return sResult.Trim();
            else
                return "-";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            string sSearchNumber = "";
            bSearchMode = true;

            if(string.IsNullOrEmpty(sSearchNumber))
            {
                #region voucher search
                DataTable dtGridItems = new DataTable();

                string commandText = " select STOCKVOUCHER,CONVERT(VARCHAR(11),TRANSDATE,103) as TRANSDATE," +
                                     " (CASE WHEN STOCKTAKINGTYPE = 0 THEN 'Daily'" +
                                              " ELSE 'Monthly' END) AS STOCKTAKINGTYPE," +
                                    " (CASE WHEN STOCKTYPE = 0 THEN 'SKU'" +
                                             " ELSE 'Bulk' END) STOCKTYPE " +
                                     " from CRWArticalStockTakingTable order by STOCKVOUCHER";

                SqlConnection connection = new SqlConnection();

                if(application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if(connection.State == ConnectionState.Closed)
                    connection.Open();

                SqlCommand command = new SqlCommand(commandText, connection);


                command.CommandTimeout = 0;
                SqlDataReader reader = command.ExecuteReader();
                dtGridItems = new DataTable();
                dtGridItems.Load(reader);
                if(dtGridItems != null && dtGridItems.Rows.Count > 0)
                {
                    DataRow selRow = null;
                    Dialog.Dialog objCustOrderSearch = new Dialog.Dialog();

                    objCustOrderSearch.GenericSearch(dtGridItems, ref selRow, "Article wise stock count list");
                    if(selRow != null)
                    {
                        sSearchNumber = Convert.ToString(selRow["STOCKVOUCHER"]);
                        txtVoucherNo.Text = sSearchNumber;
                    }
                    else
                        return;
                }
                else
                {
                    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Order Exists.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    return;
                }

                #endregion

                #region line search
                if(!string.IsNullOrEmpty(sSearchNumber))
                {
                    GetVoucherHeaderData(sSearchNumber);

                    string commandText1 = "select ARTICLE_CODE [Article Code]," +
                                        " ARTICLEDESCRIPTION [Article Name],ONHANDCW [On Hand CW]," +
                                        " ONHANDQTY [On Hand Qty],ONHANDGROSSQTY [On Hand Gross Wt]," +
                                        " PHYSICALCW [Physical CW],PHYSICALQTY [Physical Qty],PHYSICALGROSSQTY [Physical Gross Wt]," +
                                        " DIFFERENCECW as [Difference CW],DIFFERENCEQTY as [Difference Qty]," +
                                        " (PHYSICALGROSSQTY - ONHANDGROSSQTY) [Difference Gross Wt]," +
                                        " Barcodelabelweight as [Barcode label weight],SUBARTICLE," +
                                        " (CASE WHEN METALTYPE=1 THEN 'Gold'" + 
				                        " WHEN METALTYPE=0 THEN 'Other'" +
				                        " WHEN METALTYPE=2 THEN 'Silver'" +
				                        " WHEN METALTYPE=3 THEN 'Platinum'" +
				                        " WHEN METALTYPE=4 THEN 'Alloy'" +
				                        " WHEN METALTYPE=5 THEN 'Diamond '" +
				                        " WHEN METALTYPE=6 THEN 'Pearl'" +
				                        " WHEN METALTYPE=7 THEN 'Stone'" +
				                        " WHEN METALTYPE=8 THEN 'Consumables'" +
				                        " WHEN METALTYPE=11 THEN 'Watch'" +
				                        " WHEN METALTYPE=12 THEN 'LooseDmd' " +
				                        " WHEN METALTYPE=13 THEN 'Palladium' " +
				                        " WHEN METALTYPE=14 THEN 'Jewellery'" +
				                        " WHEN METALTYPE=15 THEN 'Metal'" +
				                        " WHEN METALTYPE=16 THEN 'PackingMaterial'" + 
				                        " WHEN METALTYPE=17 THEN 'Certificate' " +
				                        " WHEN METALTYPE=18 THEN 'GiftVoucher' " +
                                        " else ''" +
				                        "    END) METALTYPE,"+
                                        " COUNTEDBY [Counted By],isnull(b.Name,'') Name,isnull(REMARKS,'') Remarks" +
                                        " ,bd.DESCRIPTION as [Sub Article Name]" +
                                        " from CRWArticalStockTakingLine A" +
                                        " left join (  select R.STAFFID as Code,d.NAME as Name from RETAILSTAFFTABLE r" +
                                        " left join dbo.HCMWORKER as h on h.PERSONNELNUMBER = r.STAFFID" +
                                        " left join dbo.DIRPARTYTABLE as d on d.RECID = h.PERSON ) B on A.COUNTEDBY=B.Code" +
                                        " left join BASE_DESIGN_MASTER BD on a.SUBARTICLE = bd.BASEDESIGNCODE" +
                                        " Where A.STOCKVOUCHER='" + sSearchNumber + "' order by A.LINENUM ";


                    SqlConnection connection1 = new SqlConnection();
                    if(application != null)
                        connection = application.Settings.Database.Connection;
                    else
                        connection = ApplicationSettings.Database.LocalConnection;
                    if(connection.State == ConnectionState.Closed)
                        connection.Open();

                    SqlCommand command1 = new SqlCommand(commandText1, connection);
                    command1.CommandTimeout = 0;

                    SqlDataAdapter adapter = new SqlDataAdapter(commandText1, connection);
                    dtFetchedData = new DataTable();
                    adapter.Fill(dtFetchedData);

                    if(dtFetchedData != null && dtFetchedData.Rows.Count > 0)
                    {
                        GetTotalOfItemList();
                        gridStock.DataSource = dtFetchedData;
                        btnRePrint.Enabled = true;
                    }
                }
                #endregion
            }

            #region Controll enabled/disabled
            if(bSearchMode == true)
            {
                panel1.Enabled = false;
                btnAdd.Enabled = false;
                btnFetch.Enabled = false;
                btnCommit.Enabled = false;
                btnEdit.Enabled = false;
            }
            else
            {
                panel1.Enabled = true;
                btnAdd.Enabled = true;
                btnFetch.Enabled = true;
                btnCommit.Enabled = true;
                btnEdit.Enabled = true;
            }
            #endregion
        }

        private void GetVoucherHeaderData(string sVou)
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select CONVERT(VARCHAR(11),TRANSDATE,103) as TRANSDATE,STOCKTAKINGTYPE,STOCKTYPE , PRODUCTTYPE,");
            sbQuery.Append("ARTICALCODE,SUBARTICALCODE from CRWArticalStockTakingTable");
            sbQuery.Append(" WHERE STOCKVOUCHER = '" + sVou + "' ");
            DataTable dtGSS = new DataTable();
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = cmd.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    dtpTransDate.Text = Convert.ToString(reader.GetValue(0));
                    cmbStockTakingType.SelectedIndex = Convert.ToInt16(reader.GetValue(1));
                    cmbStockType.SelectedIndex = Convert.ToInt16(reader.GetValue(2));
                    cmbProductType.Text = Convert.ToString(reader.GetValue(3));
                    cCmbArticle.Text = Convert.ToString(reader.GetValue(4));
                    cCmbSubArticle.Text = Convert.ToString(reader.GetValue(5));
                }
            }
            reader.Close();
            reader.Dispose();

            if(connection.State == ConnectionState.Open)
                connection.Close();
        }

        private void PrintVoucher()
        {
            string sCompanyName = GetCompanyName();//aded on 14/04/2014 R.Hossain

            //datasources
            List<ReportDataSource> rds = new List<ReportDataSource>();
            rds.Add(new ReportDataSource("HEADERINFO", (DataTable)GetHeaderInfo()));
            rds.Add(new ReportDataSource("DETAILINFO", (DataTable)GetDetailInfo()));
            //parameters
            List<ReportParameter> rps = new List<ReportParameter>();
            rps.Add(new ReportParameter("StoreName", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreName) ? " " : ApplicationSettings.Terminal.StoreName, true));
            rps.Add(new ReportParameter("StoreAddress", string.IsNullOrEmpty(ApplicationSettings.Terminal.StoreAddress) ? " " : ApplicationSettings.Terminal.StoreAddress, true));
            rps.Add(new ReportParameter("StorePhone", string.IsNullOrEmpty(ApplicationSettings.Terminal.StorePhone) ? " " : ApplicationSettings.Terminal.StorePhone, true));
            rps.Add(new ReportParameter("Title", "Article Wise Stock Taking", true));
            rps.Add(new ReportParameter("CompName", sCompanyName, true));
            rps.Add(new ReportParameter("PrintedBy", ApplicationSettings.Terminal.TerminalOperator.OperatorId, true));

            string reportName = @"rptArticleWiseStockTaking";
            string reportPath = @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";
            RdlcViewer rptView = new RdlcViewer("Article Wise Stock Taking Report", reportPath, rds, rps, null);
            rptView.ShowDialog();
        }

        private string GetCompanyName()
        {
            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            string sCName = string.Empty;

            string sQry = "SELECT ISNULL(A.NAME,'') FROM DIRPARTYTABLE A INNER JOIN COMPANYINFO B" +
                " ON A.RECID = B.RECID WHERE B.DATAAREA = '" + ApplicationSettings.Database.DATAAREAID + "'";

            SqlCommand cmd = new SqlCommand(sQry, connection);
            cmd.CommandTimeout = 0;
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            sCName = Convert.ToString(cmd.ExecuteScalar());

            if(connection.State == ConnectionState.Open)
                connection.Close();
          
            return sCName;

        }
        private DataTable GetHeaderInfo()
        {
            DataTable dtHeader = new DataTable();
            dtHeader.Columns.Add("VOUCHERNO", typeof(string));
            dtHeader.Columns.Add("TRANSDATE", typeof(DateTime));
            dtHeader.Columns.Add("WAREHOUSE", typeof(string));
            dtHeader.Columns.Add("STOCKTAKINGTYPE", typeof(string));
            dtHeader.Columns.Add("STOCKTYPE", typeof(string));
            dtHeader.Columns.Add("PRODUCTTYPE", typeof(string));
            dtHeader.Columns.Add("ARTICLE", typeof(string));
            dtHeader.Columns.Add("SUBARTICLE", typeof(string));

            DataRow dr = dtHeader.NewRow();
            dr["VOUCHERNO"] = txtVoucherNo.Text;
            dr["TRANSDATE"] = dtpTransDate.Value;
            dr["WAREHOUSE"] = ApplicationSettings.Database.StoreID;
            dr["STOCKTAKINGTYPE"] = cmbStockTakingType.Text;
            dr["STOCKTYPE"] = cmbStockType.Text;
            dr["PRODUCTTYPE"] = cmbProductType.Text;
            dr["ARTICLE"] = cCmbArticle.Text;
            dr["SUBARTICLE"] = cCmbSubArticle.Text;
            
            dtHeader.Rows.Add(dr);

            return dtHeader;
        }
        private DataTable GetDetailInfo()
        {
            DataTable dtDetails = new DataTable();
            dtDetails.Columns.Add("ArticleCode", typeof(string));
            dtDetails.Columns.Add("ArticleName", typeof(string));
            dtDetails.Columns.Add("SubArticle", typeof(string));
            dtDetails.Columns.Add("SubArticleName", typeof(string));
            dtDetails.Columns.Add("MetalType", typeof(string));
            dtDetails.Columns.Add("OnHandCW", typeof(decimal));
            dtDetails.Columns.Add("OnHandQty", typeof(decimal));
           // dtDetails.Columns.Add("OnHandGrossWt", typeof(decimal));
            dtDetails.Columns.Add("PhysicalCW", typeof(decimal));
            dtDetails.Columns.Add("PhysicalQty", typeof(decimal));
           // dtDetails.Columns.Add("PhysicalGrossWt", typeof(decimal));
            dtDetails.Columns.Add("DifferenceCW", typeof(decimal));
            dtDetails.Columns.Add("DifferenceQty", typeof(decimal));
           // dtDetails.Columns.Add("DifferenceGrossWt", typeof(decimal));
            dtDetails.Columns.Add("BarCodeLabelQty", typeof(decimal));
            dtDetails.Columns.Add("CountedBy", typeof(string));
            dtDetails.Columns.Add("Name", typeof(string));
            dtDetails.Columns.Add("Remarks", typeof(string));

            foreach(DataRow item in dtFetchedData.Rows)
            {
                DataRow dr = dtDetails.NewRow();
                dr["ArticleCode"] = item["Article Code"];
                dr["ArticleName"] = Convert.ToString(item["Article Name"]);
                dr["SubArticle"] = item["SubArticle"];
                dr["SubArticleName"] = item["Sub Article Name"];
                dr["MetalType"] = Convert.ToString(item["MetalType"]);
                dr["OnHandCW"] = Convert.ToDecimal(item["On Hand CW"]);
                dr["OnHandQty"] = Convert.ToDecimal(item["On Hand Qty"]);
               // dr["OnHandGrossWt"] = Convert.ToDecimal(item["On Hand Gross Wt"]);
                dr["PhysicalCW"] = Convert.ToDecimal(item["Physical CW"]);
                dr["PhysicalQty"] = Convert.ToDecimal(item["Physical Qty"]);
                //dr["PhysicalGrossWt"] = Convert.ToDecimal(item["Physical Gross Wt"]);
                dr["DifferenceCW"] = Convert.ToDecimal(item["Difference CW"]);
                dr["DifferenceQty"] = Convert.ToDecimal(item["Difference Qty"]);
               // dr["DifferenceGrossWt"] = Convert.ToDecimal(item["Difference Gross Wt"]);
                dr["BarCodeLabelQty"] = Convert.ToDecimal(item["Barcode label weight"]);
                dr["CountedBy"] = item["Counted By"];
                dr["Name"] = item["Name"];
                dr["Remarks"] = item["Remarks"];
                dtDetails.Rows.Add(dr);
            }

            return dtDetails;
        }

        private void btnRePrint_Click(object sender, EventArgs e)
        {
            PrintVoucher();
        }

        private void txtPCS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }
        }
    }
}
