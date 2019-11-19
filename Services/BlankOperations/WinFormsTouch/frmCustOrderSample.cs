using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Dialog;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Item;
using Microsoft.Dynamics.Retail.Pos.Dimension;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using Microsoft.Dynamics.Retail.Pos.RoundingService;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Interaction;
using System.IO;

namespace BlankOperations.WinFormsTouch
{
    /// <summary>
    /// 
    /// </summary>
    public partial class frmCustOrderSample:frmTouchBase
    {
        #region Variable Declaration
        private SaleLineItem saleLineItem;
        Rounding objRounding = new Rounding();
        public IPosTransaction pos { get; set; }
        LSRetailPosis.POSProcesses.WinControls.NumPad obj;
        [Import]
        private IApplication application;

        DataTable dtItemInfo = new DataTable("dtItemInfo");
        decimal linenum = 0;
        Random randUnique = new Random();
        bool IsEdit = false;
        int EditselectedIndex = 0;
        DataSet dsSearchedOrder = new DataSet();
        string inventDimId = string.Empty;
        string PreviousPcs = string.Empty;
        public DataTable dtSample = new DataTable();
        string unitid = string.Empty;
        string Previewdimensions = string.Empty;
        decimal dStoneWtRange = 0m;


        string sBaseItemID = string.Empty;
        string sBaseConfigID = string.Empty;
        //
        string sBookedSKUItem = string.Empty;
        public DataTable dtSketch = new DataTable();
        int iOrdLineNo = 0;
        bool bView = false;
        bool bStockSample = false;


        #region enum  RateType
        enum RateType
        {
            Weight = 0,
            Pcs = 1,
            Tot = 2,
        }
        #endregion

        #region enum  MakingType
        enum MakingType
        {
            Weight = 2,
            Pieces = 0,
            Tot = 3,
            Percentage = 4,
        }
        #endregion

        #region enum  RateTypeNew
        enum RateTypeNew
        {
            Purchase = 0,
            OGP = 1,
            OGOP = 2,
            Sale = 3,
            GSS = 4,
            Exchange = 6,
            OtherExchange = 8,
        }
        #endregion

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

        }

        #endregion
        #endregion

        #region InventDimId
        public string GetInventID(string distinctProductVariantID)
        {
            string commandText = "select top(1)  INVENTDIMID from assortedinventdimcombination WHERE DISTINCTPRODUCTVARIANT='" + distinctProductVariantID + "'";

            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if(connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }


            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;
            return Convert.ToString(command.ExecuteScalar());

        }
        #endregion

        #region frmCustOrderSample
        public frmCustOrderSample(IPosTransaction posTransaction, IApplication Application, decimal line)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            linenum = line;
            btnPOSItemSearch.Focus();
        }

        public frmCustOrderSample(IPosTransaction posTransaction, IApplication Application, DataTable dt, decimal line, DataTable dtSampleSketch = null, bool isView = false)
        {
            InitializeComponent();
            pos = posTransaction;
            application = Application;
            this.dtSample = dt;
            iOrdLineNo = Convert.ToInt16(line);

            if(dtItemInfo == null || dtItemInfo.Columns.Count == 0)
            {
                dtItemInfo = dtSample.Clone();
                if(dtSample != null && dtSample.Rows.Count > 0)
                {
                    //DataTable dt = new DataTable();
                    dtSample.Select("LINENUM=" + line).CopyToDataTable(dtItemInfo, LoadOption.PreserveChanges);
                    dtSketch = dtSampleSketch;
                }
            }
            //Start: 

            if(dtSample != null && dtSample.Rows.Count > 0)
            {
                //DataTable dt = new DataTable();

                string path = string.Empty;
                string sArchivePath = GetArchivePathFromImage();

                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    DataRow drRowToUpdate = dtItemInfo.Rows[selectedRow];
                    DataTable dtSketchDownload = new DataTable();
                    if(dtSketch != null && dtSketch.Rows.Count > 0)
                    {
                        foreach(DataRow dtDown in dtSketch.Rows)
                        {
                            if(Convert.ToString(drRowToUpdate["UNIQUEID"]) == Convert.ToString(dtDown["UNIQUEID"]))
                            {
                                dtSketch = dtSampleSketch;
                                break;
                            }
                            else
                            {
                                DataRow dr = dtSketch.NewRow();
                                dr["UNIQUEID"] = Convert.ToString(drRowToUpdate["UNIQUEID"]);
                                dr["LINENUM"] = Convert.ToInt16(line);

                                if(!string.IsNullOrEmpty(path))
                                {
                                    if(File.Exists(path))
                                    {
                                        System.Drawing.Image image = System.Drawing.Image.FromFile(Convert.ToString(path));
                                        int width = image.Width;
                                        int height = image.Height;
                                        //decimal aspectRatio = width > height ? decimal.divide(width, height) : decimal.divide(height, width);
                                        int fileSize = (int)new System.IO.FileInfo(Convert.ToString(path)).Length;
                                    }
                                }

                                dr["SKETCH"] = Convert.ToString(path); //GetByteArray(

                                dtSketch.Rows.Add(dr);
                            }
                        }
                    }

                    if(bView)
                    {
                        if(dtSketch != null && dtSketch.Rows.Count > 0)
                        {
                            foreach(DataRow dtDown in dtSketch.Rows)
                            {
                                dtSketchDownload = dtSketch.Clone();
                                dtSketchDownload.ImportRow(dtDown);
                                dtSketchDownload.AcceptChanges();
                                break;
                            }
                        }
                    }
                }
            }

            //End

            #region Commented
            //if(dtSample.Rows.Count > 0)
            //{
            //    dtSample.Select("LINENUM=" + line).CopyToDataTable(dtItemInfo, LoadOption.PreserveChanges);
            //    dtSketch = dtSampleSketch;


            //    //// image show from folder
            //    string path = string.Empty;
            //    string sArchivePath = GetArchivePathFromImage();


            //    if(dtSketch.Rows.Count > 0)
            //        path = Convert.ToString(dtSketch.Rows[0]["SKETCH"]);
            //    else
            //    {
            //        if(dtSample.Rows.Count > 0 && isView)
            //            path = sArchivePath + "" + Convert.ToString(dtSample.Rows[0]["ORDERNUM"]) + "_" + line + "_S" + ".jpeg"; //
            //    }

            //    dtSketch = new DataTable();
            //    dtSketch.Columns.Add("UNIQUEID", typeof(string));
            //    dtSketch.Columns.Add("LINENUM", typeof(decimal));
            //    dtSketch.Columns.Add("SKETCH", typeof(string));

            //    DataRow dr = dtSketch.NewRow();
            //    dr["UNIQUEID"] = "1";
            //    dr["LINENUM"] = Convert.ToInt16(line);

            //    if(!string.IsNullOrEmpty(path))
            //    {
            //        if(File.Exists(path))
            //        {
            //            System.Drawing.Image image = System.Drawing.Image.FromFile(Convert.ToString(path));
            //            int width = image.Width;
            //            int height = image.Height;
            //            //decimal aspectRatio = width > height ? decimal.divide(width, height) : decimal.divide(height, width);
            //            int fileSize = (int)new System.IO.FileInfo(Convert.ToString(path)).Length;
            //        }
            //    }

            //    dr["SKETCH"] = Convert.ToString(path); //GetByteArray(

            //    dtSketch.Rows.Add(dr);
            //}
            #endregion

            if(dtItemInfo.Rows.Count == 0)
            {
                btnAddItem.Enabled = true;
            }
            else if(dtItemInfo.Rows.Count == 1)
            {
                btnAddItem.Enabled = false;
            }
            grItems.DataSource = dtItemInfo.DefaultView;
            bView = isView;
            if(bView)
            {
                btnSubmit.Enabled = false;
                btnEdit.Enabled = false;
                btnDelete.Enabled = false;
            }
            linenum = line;
            btnPOSItemSearch.Focus();
        }
        #endregion

        public string GetArchivePathFromImage()
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select TOP(1) ARCHIVEPATH  from RETAILSTORETABLE where STORENUMBER='" + ApplicationSettings.Database.StoreID + "'");

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();
            if(!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        private string GetItemParentId(string sSalesItem)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select ITEMIDPARENT  from INVENTTABLE  where ITEMID='" + sSalesItem + "'");

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();
            if(!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        #region Dimenssion info
        private string ColorSizeStyleConfig()
        {
            string dash = " - ";
            StringBuilder colorSizeStyleConfig;
            if(!string.IsNullOrEmpty(cmbCode.Text))
                colorSizeStyleConfig = new StringBuilder("Color : " + cmbCode.Text);
            else
                colorSizeStyleConfig = new StringBuilder(cmbCode.Text);

            if(!string.IsNullOrEmpty(cmbSize.Text))
            {
                if(colorSizeStyleConfig.Length > 0)
                {
                    colorSizeStyleConfig.Append(dash);
                }
                colorSizeStyleConfig.Append(" Size : " + cmbSize.Text);
            }

            if(!string.IsNullOrEmpty(cmbStyle.Text))
            {
                if(colorSizeStyleConfig.Length > 0)
                {
                    colorSizeStyleConfig.Append(dash);
                }
                colorSizeStyleConfig.Append(" Style : " + cmbStyle.Text);
            }

            if(!string.IsNullOrEmpty(cmbConfig.Text))
            {
                if(colorSizeStyleConfig.Length > 0) { colorSizeStyleConfig.Append(dash); }
                colorSizeStyleConfig.Append(" Configuration : " + cmbConfig.Text);
            }

            return colorSizeStyleConfig.ToString();
        }
        #endregion

        #region POS-ITEM-SEARCH-CLICK
        private void btnPOSItemSearch_Click(object sender, EventArgs e)
        {
            if(IsEdit)
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("You are in editing mode", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return;
                }
            }
            Dialog objdialog = new Dialog();
            string str = string.Empty;
            DataSet dsItem = new DataSet();
            //  objdialog.MyItemSearch(500, ref str, out  dsItem, " AND  I.ITEMID NOT IN (SELECT ITEMID FROM INVENTTABLE WHERE RETAIL=1) "); // blocked on 12.09.2013 // SKU allow

            // objdialog.MyItemSearch(500, ref str, out  dsItem, "");
            objdialog.MyItemSearch(500, ref str, out  dsItem, " AND  I.ITEMID IN (SELECT ITEMID FROM INVENTTABLE WHERE Sample=1) ");

            saleLineItem = new SaleLineItem();

            if(dsItem != null && dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
            {
                if(!string.IsNullOrEmpty(Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"])))
                {
                    saleLineItem.ItemId = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                    bStockSample = false;

                    Item objItem = new Item();
                    objItem.MYProcessItem(saleLineItem, application);

                    Dimension objDim = new Dimension();
                    DataTable dtDimension = objDim.GetDimensions(saleLineItem.ItemId);


                    if(dtDimension != null && dtDimension.Rows.Count > 0)
                    {
                        DimensionConfirmation dimConfirmation = new DimensionConfirmation();
                        dimConfirmation.InventDimCombination = dtDimension;
                        dimConfirmation.DimensionData = saleLineItem.Dimension;

                        frmDimensions objfrmDim = new frmDimensions(dimConfirmation);
                        objfrmDim.ShowDialog();

                        if(objfrmDim.SelectDimCombination != null)
                        {
                            inventDimId = GetInventID(Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[2]));
                            DataTable dtcmbCode = new DataTable();
                            dtcmbCode.Columns.Add("CodeID", typeof(string));
                            dtcmbCode.Columns.Add("CodeValue", typeof(string));
                            DataRow drCode;
                            drCode = dtcmbCode.NewRow();
                            drCode["CodeID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[4]);
                            drCode["CodeValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[4]);
                            dtcmbCode.Rows.Add(drCode);
                            cmbCode.DataSource = dtcmbCode;
                            cmbCode.DisplayMember = "CodeValue";
                            cmbCode.ValueMember = "CodeID";

                            DataTable dtSize = new DataTable();
                            dtSize.Columns.Add("SizeID", typeof(string));
                            dtSize.Columns.Add("SizeValue", typeof(string));
                            DataRow drSize;
                            drSize = dtSize.NewRow();
                            drSize["SizeID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[3]);
                            drSize["SizeValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[3]);
                            dtSize.Rows.Add(drSize);
                            cmbSize.DataSource = dtSize;
                            cmbSize.DisplayMember = "SizeID";
                            cmbSize.ValueMember = "SizeValue";

                            DataTable dtConfig = new DataTable();
                            dtConfig.Columns.Add("ConfigID", typeof(string));
                            dtConfig.Columns.Add("ConfigValue", typeof(string));
                            DataRow drConfig;
                            drConfig = dtConfig.NewRow();
                            drConfig["ConfigID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[6]);
                            drConfig["ConfigValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[6]);
                            dtConfig.Rows.Add(drConfig);
                            cmbConfig.DataSource = dtConfig;
                            cmbConfig.DisplayMember = "ConfigID";
                            cmbConfig.ValueMember = "ConfigValue";

                            DataTable dtStyle = new DataTable();
                            dtStyle.Columns.Add("StyleID", typeof(string));
                            dtStyle.Columns.Add("StyleValue", typeof(string));
                            DataRow drStyle;
                            drStyle = dtStyle.NewRow();
                            drStyle["StyleID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[5]);
                            drStyle["StyleValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[5]);
                            dtStyle.Rows.Add(drStyle);
                            cmbStyle.DataSource = dtStyle;
                            cmbStyle.DisplayMember = "StyleID";
                            cmbStyle.ValueMember = "StyleValue";
                            cmbConfig.Enabled = false;
                            cmbCode.Enabled = false;
                            cmbSize.Enabled = false;
                            cmbStyle.Enabled = false;

                            Previewdimensions = ColorSizeStyleConfig();
                        }


                    }

                    else
                    {
                        cmbStyle.Text = string.Empty;
                        cmbConfig.Text = string.Empty;
                        cmbCode.Text = string.Empty;
                        cmbSize.Text = string.Empty;
                        cmbConfig.Enabled = false;
                        cmbCode.Enabled = false;
                        cmbSize.Enabled = false;
                        cmbStyle.Enabled = false;
                    }
                    SqlConnection conn = new SqlConnection();
                    if(application != null)
                        conn = application.Settings.Database.Connection;
                    else
                        conn = ApplicationSettings.Database.LocalConnection;
                    txtPCS.Focus();
                    txtPCS.Text = "1";
                    string sQty = string.Empty;
                    sQty = GetStandardQuantityFromDB(conn, Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]));
                    if(!string.IsNullOrEmpty(sQty))
                    {
                        sQty = Convert.ToString(decimal.Round(Convert.ToDecimal(sQty), 3, MidpointRounding.AwayFromZero));
                        txtQuantity.Text = Convert.ToString(Convert.ToDecimal(sQty) == 0 ? string.Empty : Convert.ToString(sQty));
                    }
                    txtItemId.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                    txtItemName.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMNAME"]);
                    //txtRate.Text = getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);
                    //if (!string.IsNullOrEmpty(txtRate.Text))
                    //    cmbRateType.SelectedIndex = cmbRateType.FindStringExact("Tot");

                    unitid = saleLineItem.BackofficeSalesOrderUnitOfMeasure;
                    //  txtRate.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMPRICE"]).Remove(0, 1).Trim();

                    if(Convert.ToString(txtItemId.Text.Trim()) == string.Empty
                        || Convert.ToString(cmbConfig.Text.Trim()) == string.Empty)
                    {
                        sBaseItemID = string.Empty;
                        sBaseConfigID = string.Empty;
                    }
                    else
                    {
                        sBaseItemID = txtItemId.Text.Trim();
                        sBaseConfigID = cmbConfig.Text;
                    }
                }
            }


        }
        #endregion

        #region  GET RATE FROM METAL TABLE
        public string getRateFromMetalTable(string itemid, string configuration, string batchid, string colorid, string sizeid, string weight, string pcs = null)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + itemid.Trim() + "' ");

            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");
            commandText.Append(" BEGIN ");
            //   }
            commandText.Append(" SELECT TOP 1 CAST(RATES AS numeric(26,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + configuration.Trim() + "' ");
            commandText.Append("   ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC ");

            if(string.IsNullOrEmpty(weight))
            {
                weight = "0";
            }
            string ldweight = string.Empty;
            ldweight = "0";
            if(pcs != null && Convert.ToDecimal(pcs) != Convert.ToDecimal(0))   // Convert.ToDecimal(pcs) != Convert.ToDecimal(0) added as per BOM.PDSCWQTY added in BOM on request of urvi and arunava 
            {
                ldweight = Convert.ToString(decimal.Round((Convert.ToDecimal(weight) / Convert.ToDecimal(pcs)), 3, MidpointRounding.AwayFromZero)); //Changes on 11/04/2014 R.Hossain
                //dStoneWtRange = decimal.Round(Convert.ToDecimal(dr["QTY"]) / Convert.ToDecimal(dr["PCS"]), 3, MidpointRounding.AwayFromZero);
            }
            else
            {
                ldweight = Convert.ToString(Convert.ToDecimal(weight));
            }

            //commandText.Append(" END ");
            //commandText.Append(" ELSE ");
            //commandText.Append(" BEGIN ");
            //commandText.Append(" SELECT  CAST(UNIT_RATE AS decimal (6,2)) FROM RETAILCUSTOMERSTONEAGGREEMENTDETAIL WHERE  ");
            //commandText.Append(" WAREHOUSE=@INVENTLOCATION AND ('" + weight.Trim() + "' BETWEEN FROM_WEIGHT AND TO_WEIGHT)  ");
            //commandText.Append(" AND (DATEADD(dd, DATEDIFF(dd,0,GETDATE()), 0) BETWEEN FROMDATE AND TODATE) ");
            //commandText.Append(" AND ITEMID='" + itemid.Trim() + "' AND INVENTBATCHID='" + batchid.Trim() + "' ");
            //commandText.Append(" AND INVENTSIZEID='" + sizeid.Trim() + "' AND INVENTCOLORID='" + colorid.Trim() + "' ");
            //commandText.Append(" END ");

            commandText.Append(" END ");
            commandText.Append(" ELSE IF(@METALTYPE IN ('" + (int)MetalType.LooseDmd + "','" + (int)MetalType.Stone + "')) ");
            commandText.Append(" BEGIN ");

            commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(26, 2))   ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + ldweight.Trim() + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + itemid.Trim() + "') AND   "); //(INVENTDIM.INVENTBATCHID = '" + batchid.Trim() + "') AND
            //commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid.Trim() + "') ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid + "') "); //22.01.2014 -- off trim
            // commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "')");
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013

            commandText.Append(" END ");
            commandText.Append(" ELSE ");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(26, 2))   ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + weight.Trim() + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + itemid.Trim() + "') AND (INVENTDIM.INVENTBATCHID = '" + batchid.Trim() + "') AND  ");
            //commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid.Trim() + "') ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + sizeid + "') AND (INVENTDIM.INVENTCOLORID = '" + colorid + "') "); //22.01.2014 -- off trim
            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1"); // modified on 02.09.2013

            commandText.Append(" END ");
            //   }

            //    SELECT CONVERT(DATETIME,SUBSTRING(CONVERT(VARCHAR(10),DATEADD(dd,DATEDIFF(dd,0,CAST(TRANSDATE AS VARCHAR)),0),120) + ' ' +
            //    CAST(CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME) AS VARCHAR),0,24),121)  FROM METALRATES

            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;


            if(conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }

            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();
            if(!string.IsNullOrEmpty(sResult))
                return Convert.ToString(objRounding.Round(Convert.ToDecimal(sResult.Trim())));
            else
                return string.Empty;

        }
        #endregion

        #region  Get Ingredient Calculation Type

        private string GetIngredientCalcType(string ItemID, string configuration, string batchid, string ColorID, string SizeID, string GrWeight, string pcs = null)  // added
        {
            StringBuilder commandText = new StringBuilder();

            string grweigh = string.Empty;
            if(string.IsNullOrEmpty(GrWeight))
                grweigh = "0";
            else
                grweigh = GrWeight;

            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + ItemID.Trim() + "' ");

            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.LooseDmd + "','" + (int)MetalType.Stone + "')) ");
            commandText.Append(" BEGIN ");

            // commandText.Append(" SELECT     CAST(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.UNIT_RATE AS numeric(28, 2))   ");
            commandText.Append(" SELECT     ISNULL(RETAILCUSTOMERSTONEAGGREEMENTDETAIL.CALCTYPE,0) AS CALCTYPE ");
            commandText.Append(" FROM         RETAILCUSTOMERSTONEAGGREEMENTDETAIL INNER JOIN ");
            commandText.Append(" INVENTDIM ON RETAILCUSTOMERSTONEAGGREEMENTDETAIL.INVENTDIMID = INVENTDIM.INVENTDIMID ");
            // commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND ('" + grweigh + "' BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");
            commandText.Append(" WHERE     (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.WAREHOUSE = @INVENTLOCATION) AND (  ");
            commandText.Append(dStoneWtRange); //COs  Modification
            commandText.Append(" BETWEEN RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FROM_WEIGHT AND  ");

            commandText.Append("  RETAILCUSTOMERSTONEAGGREEMENTDETAIL.TO_WEIGHT) AND (DATEADD(dd, DATEDIFF(dd, 0, GETDATE()), 0) BETWEEN  ");
            commandText.Append(" RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ToDate) AND  ");
            //  commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "') AND (INVENTDIM.INVENTBATCHID = '" + BatchID.Trim() + "') AND  ");
            commandText.Append("  (RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID = '" + ItemID.Trim() + "')  AND  ");
            commandText.Append("  (INVENTDIM.INVENTSIZEID = '" + SizeID.Trim() + "') AND (INVENTDIM.INVENTCOLORID = '" + ColorID.Trim() + "') ");

            commandText.Append(" AND (INVENTDIM.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "') AND RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ACTIVATE = 1");

            commandText.Append(" ORDER BY RETAILCUSTOMERSTONEAGGREEMENTDETAIL.ITEMID DESC, RETAILCUSTOMERSTONEAGGREEMENTDETAIL.FromDate DESC");  // Changed order sequence on 03.06.2013 as per u.das
            commandText.Append(" END ");
            // }

            //   commandText.Append("AND CAST(cast(([TIME] / 3600) as varchar(10)) + ':' + cast(([TIME] % 60) as varchar(10)) AS TIME)<=CAST(CONVERT(VARCHAR(8),GETDATE(),108) AS TIME)  ");

            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return Convert.ToString(sResult.Trim());

        }
        #endregion

        #region ADD CLICK
        private void btnAddItem_Click(object sender, EventArgs e)
        {
            string sUniqueNo = string.Empty;
            if(ValidateControls())
            {
                DataRow dr;
                if(IsEdit == false && dtItemInfo != null && dtItemInfo.Rows.Count == 0 && dtItemInfo.Columns.Count == 0)
                {
                    IsEdit = false;
                    //dtItemInfo.Columns.Add("OrdLineNo", typeof(string));
                    dtItemInfo.Columns.Add("UNIQUEID", typeof(string));
                    dtItemInfo.Columns.Add("LINENUM", typeof(decimal));
                    dtItemInfo.Columns.Add("ITEMID", typeof(string));
                    dtItemInfo.Columns.Add("ITEMNAME", typeof(string));
                    dtItemInfo.Columns.Add("CONFIGURATION", typeof(string));
                    dtItemInfo.Columns.Add("COLOR", typeof(string));
                    dtItemInfo.Columns.Add("SIZE", typeof(string));
                    dtItemInfo.Columns.Add("STYLE", typeof(string));
                    dtItemInfo.Columns.Add("PCS", typeof(decimal));
                    dtItemInfo.Columns.Add("QUANTITY", typeof(decimal));

                    dtItemInfo.Columns.Add("NETTWT", typeof(decimal));
                    dtItemInfo.Columns.Add("DIAWT", typeof(decimal));
                    dtItemInfo.Columns.Add("DIAAMT", typeof(decimal));
                    dtItemInfo.Columns.Add("STNWT", typeof(decimal));
                    dtItemInfo.Columns.Add("STNAMT", typeof(decimal));
                    dtItemInfo.Columns.Add("TOTALAMT", typeof(decimal));

                    dtItemInfo.Columns.Add("INVENTDIMID", typeof(string));
                    dtItemInfo.Columns.Add("UNITID", typeof(string));
                    dtItemInfo.Columns.Add("DIMENSIONS", typeof(string));

                    dtItemInfo.Columns.Add("REMARKSDTL", typeof(string));
                    dtItemInfo.Columns.Add("ISRETURNED", typeof(string));
                    dtItemInfo.Columns.Add("STOCKSAMPLE", typeof(string));
                    

                }
                if(IsEdit == false)
                {
                    dr = dtItemInfo.NewRow();
                    // dr["OrdLineNo"] = iOrdLineNo;
                    dr["UNIQUEID"] = sUniqueNo = Convert.ToString(randUnique.Next());
                    dr["LINENUM"] = linenum;
                    dr["ITEMID"] = Convert.ToString(txtItemId.Text.Trim());
                    dr["ITEMNAME"] = Convert.ToString(txtItemName.Text.Trim());
                    dr["CONFIGURATION"] = Convert.ToString(cmbConfig.Text.Trim());
                    dr["COLOR"] = Convert.ToString(cmbCode.Text.Trim());
                    dr["STYLE"] = Convert.ToString(cmbStyle.Text.Trim());
                    dr["SIZE"] = Convert.ToString(cmbSize.Text.Trim());
                    if(!string.IsNullOrEmpty(txtPCS.Text.Trim()))
                        dr["PCS"] = objRounding.Round(Convert.ToDecimal(txtPCS.Text.Trim()), 0);
                    else
                        dr["PCS"] = objRounding.Round(decimal.Zero, 0);
                    if(!string.IsNullOrEmpty(txtQuantity.Text.Trim()))
                        dr["QUANTITY"] = objRounding.Round(Convert.ToDecimal(txtQuantity.Text.Trim()), 3);
                    else
                        dr["QUANTITY"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtNettWt.Text.Trim()))
                        dr["NETTWT"] = objRounding.Round(Convert.ToDecimal(txtNettWt.Text.Trim()), 3);
                    else
                        dr["NETTWT"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtDiaWt.Text.Trim()))
                        dr["DIAWT"] = objRounding.Round(Convert.ToDecimal(txtDiaWt.Text.Trim()), 3);
                    else
                        dr["DIAWT"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtDiaAmt.Text.Trim()))
                        dr["DIAAMT"] = objRounding.Round(Convert.ToDecimal(txtDiaAmt.Text.Trim()), 2);
                    else
                        dr["DIAAMT"] = objRounding.Round(decimal.Zero, 2);

                    if(!string.IsNullOrEmpty(txtStnWt.Text.Trim()))
                        dr["STNWT"] = objRounding.Round(Convert.ToDecimal(txtStnWt.Text.Trim()), 3);
                    else
                        dr["STNWT"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtStnAmt.Text.Trim()))
                        dr["STNAMT"] = objRounding.Round(Convert.ToDecimal(txtStnAmt.Text.Trim()), 2);
                    else
                        dr["STNAMT"] = objRounding.Round(decimal.Zero, 2);

                    if(!string.IsNullOrEmpty(txtTotalAmt.Text.Trim()))
                        dr["TOTALAMT"] = objRounding.Round(Convert.ToDecimal(txtTotalAmt.Text.Trim()), 2);
                    else
                        dr["TOTALAMT"] = objRounding.Round(decimal.Zero, 2);

                    dr["INVENTDIMID"] = string.IsNullOrEmpty(inventDimId) ? string.Empty : inventDimId;
                    dr["UNITID"] = string.IsNullOrEmpty(unitid) ? string.Empty : unitid;
                    dr["DIMENSIONS"] = Previewdimensions;

                    dr["REMARKSDTL"] = txtRemarks.Text.Trim();
                    dr["ISRETURNED"] = "False";
                    dr["STOCKSAMPLE"] = bStockSample;

                    dtItemInfo.Rows.Add(dr);

                    grItems.DataSource = dtItemInfo.DefaultView;
                }
                if(IsEdit == true)
                {
                    DataRow EditRow = dtItemInfo.Rows[EditselectedIndex];
                    EditRow["PCS"] = txtPCS.Text.Trim();
                    sUniqueNo = Convert.ToString(EditRow["UNIQUEID"]);
                    EditRow["QUANTITY"] = txtQuantity.Text.Trim();
                    if(!string.IsNullOrEmpty(txtNettWt.Text.Trim()))
                        EditRow["NETTWT"] = decimal.Round(Convert.ToDecimal(txtNettWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        EditRow["NETTWT"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtDiaWt.Text.Trim()))
                        EditRow["DIAWT"] = decimal.Round(Convert.ToDecimal(txtDiaWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        EditRow["DIAWT"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtDiaAmt.Text.Trim()))
                        EditRow["DIAAMT"] = decimal.Round(Convert.ToDecimal(txtDiaAmt.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        EditRow["DIAAMT"] = objRounding.Round(decimal.Zero, 2);

                    if(!string.IsNullOrEmpty(txtStnWt.Text.Trim()))
                        EditRow["STNWT"] = decimal.Round(Convert.ToDecimal(txtStnWt.Text.Trim()), 3, MidpointRounding.AwayFromZero);
                    else
                        EditRow["STNWT"] = objRounding.Round(decimal.Zero, 3);

                    if(!string.IsNullOrEmpty(txtStnAmt.Text.Trim()))
                        EditRow["STNAMT"] = decimal.Round(Convert.ToDecimal(txtStnAmt.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        EditRow["STNAMT"] = objRounding.Round(decimal.Zero, 2);

                    if(!string.IsNullOrEmpty(txtTotalAmt.Text.Trim()))
                        EditRow["TOTALAMT"] = decimal.Round(Convert.ToDecimal(txtTotalAmt.Text.Trim()), 2, MidpointRounding.AwayFromZero);
                    else
                        EditRow["TOTALAMT"] = objRounding.Round(decimal.Zero, 2);

                    EditRow["REMARKSDTL"] = txtRemarks.Text.Trim();
                    EditRow["ISRETURNED"] = "False";
                    EditRow["STOCKSAMPLE"] = bStockSample;

                    dtItemInfo.AcceptChanges();

                    grItems.DataSource = dtItemInfo.DefaultView;
                }

                ClearControls();
            }
            if(dtItemInfo.Rows.Count == 0)
            {
                btnAddItem.Enabled = true;
            }
            else if(dtItemInfo.Rows.Count == 1)
            {
                btnAddItem.Enabled = false;
            }
        }
        #endregion

        #region Submmit Click
        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if(dtSample.Columns.Count == 0)
            {
                dtSample = dtItemInfo.Clone();
            }

            foreach(DataRow dr in dtItemInfo.Rows)
            {
                if(dtSample.Rows.Count > 0)
                {
                    DataRow[] drSam = dtSample.Select("LINENUM=" + dr["LINENUM"]);
                    if(drSam.Count() > 0)
                    {
                        drSam[0]["PCS"] = dr["PCS"];
                        drSam[0]["QUANTITY"] = dr["QUANTITY"];
                        drSam[0]["NETTWT"] = dr["NETTWT"];
                        drSam[0]["DIAWT"] = dr["DIAWT"];
                        drSam[0]["DIAAMT"] = dr["DIAAMT"];
                        drSam[0]["STNWT"] = dr["STNWT"];
                        drSam[0]["STNAMT"] = dr["STNAMT"];
                        drSam[0]["TOTALAMT"] = dr["TOTALAMT"];
                        drSam[0]["REMARKSDTL"] = dr["RemarksDtl"];
                        drSam[0]["STOCKSAMPLE"] = dr["STOCKSAMPLE"];
                        
                    }
                    else
                    {
                        dtSample.ImportRow(dr);
                    }
                }
                else
                {
                    dtSample.ImportRow(dr);
                }

            }
            this.Close();

        }
        #endregion

        #region EDIT CLICK
        private void btnEdit_Click(object sender, EventArgs e)
        {
            IsEdit = false;
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    IsEdit = true;
                    EditselectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToSelect = dtItemInfo.Rows[EditselectedIndex];
                    cmbStyle.Text = Convert.ToString(theRowToSelect["STYLE"]);
                    cmbCode.Text = Convert.ToString(theRowToSelect["COLOR"]);
                    cmbSize.Text = Convert.ToString(theRowToSelect["SIZE"]);
                    cmbConfig.Text = Convert.ToString(theRowToSelect["CONFIGURATION"]);
                    txtItemId.Text = Convert.ToString(theRowToSelect["ITEMID"]);
                    txtItemName.Text = Convert.ToString(theRowToSelect["ITEMNAME"]);
                    txtPCS.Text = Convert.ToString(theRowToSelect["PCS"]);
                    txtQuantity.Text = Convert.ToString(theRowToSelect["QUANTITY"]);

                    txtNettWt.Text = Convert.ToString(theRowToSelect["NETTWT"]);
                    txtDiaWt.Text = Convert.ToString(theRowToSelect["DIAWT"]);
                    txtDiaAmt.Text = Convert.ToString(theRowToSelect["DIAAMT"]);
                    txtStnWt.Text = Convert.ToString(theRowToSelect["STNWT"]);
                    txtStnAmt.Text = Convert.ToString(theRowToSelect["STNAMT"]);
                    txtTotalAmt.Text = Convert.ToString(theRowToSelect["TOTALAMT"]);


                    txtRemarks.Text = Convert.ToString(theRowToSelect["REMARKSDTL"]);

                    btnAddItem.Enabled = true;
                }
            }
        }
        #endregion

        #region DELETE CLICK
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int DeleteSelectedIndex = 0;
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    DeleteSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtItemInfo.Rows[DeleteSelectedIndex];


                    dtItemInfo.Rows.Remove(theRowToDelete);
                    grItems.DataSource = dtItemInfo.DefaultView;
                }

            }
            if(DeleteSelectedIndex == 0 && dtItemInfo != null && dtItemInfo.Rows.Count == 0)
            {
                grItems.DataSource = null;
                dtItemInfo.Clear();
            }
            IsEdit = false;

            if(dtItemInfo.Rows.Count == 0)
            {
                btnAddItem.Enabled = true;
            }
            else if(dtItemInfo.Rows.Count == 1)
            {
                btnAddItem.Enabled = false;
            }
        }
        #endregion

        #region CLear Click
        private void btnClear_Click(object sender, EventArgs e)
        {
            saleLineItem = new SaleLineItem();
            objRounding = new Rounding();
            dtItemInfo = new DataTable();
            randUnique = new Random();
            IsEdit = false;
            EditselectedIndex = 0;
            dsSearchedOrder = new DataSet();
            grItems.DataSource = null;
            ClearControls();
        }
        #endregion

        #region Cancel Click
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region ValidateControls()
        bool ValidateControls()
        {

            if(txtItemId.Text.ToUpper().Trim() == "ITEM ID" || string.IsNullOrEmpty(txtItemId.Text))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item Id can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnPOSItemSearch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if(txtItemName.Text.ToUpper().Trim() == "ITEM NAME" || string.IsNullOrEmpty(txtItemName.Text))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item name can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    btnPOSItemSearch.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }
            if(string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtPCS.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if((!string.IsNullOrEmpty(txtPCS.Text.Trim())) && (Convert.ToDecimal(txtPCS.Text) > 1))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("PCS can not be greater than 1", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtPCS.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            if(string.IsNullOrEmpty(txtQuantity.Text.Trim()))
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Gross Wt can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    txtQuantity.Focus();
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return false;
                }
            }

            //if(string.IsNullOrEmpty(txtNettWt.Text.Trim()))
            //{
            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Nett Wt can not be blank and empty", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //    {
            //        txtNettWt.Focus();
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        return false;
            //    }
            //}

            //if(Convert.ToDecimal(txtNettWt.Text.Trim()) > Convert.ToDecimal(txtQuantity.Text.Trim()))
            //{
            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Nett Wt can not be greater than Gross Wt", MessageBoxButtons.OK, MessageBoxIcon.Information))
            //    {
            //        txtNettWt.Focus();
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        return false;
            //    }
            //}
            else
            {
                return true;
            }
        }
        #endregion

        #region ClearControls
        private void ClearControls()
        {
            txtItemId.Text = string.Empty;
            txtItemName.Text = string.Empty;
            cmbCode.Text = string.Empty;
            cmbStyle.Text = string.Empty;
            cmbSize.Text = string.Empty;
            cmbConfig.Text = string.Empty;
            txtPCS.Text = string.Empty;
            txtQuantity.Text = string.Empty;
            txtNettWt.Text = string.Empty;
            txtDiaWt.Text = string.Empty;
            txtDiaAmt.Text = string.Empty;
            txtStnWt.Text = string.Empty;
            txtStnAmt.Text = string.Empty;
            txtTotalAmt.Text = string.Empty;
            txtRemarks.Text = string.Empty;

            if(dtItemInfo.Rows.Count == 0)
            {
                btnAddItem.Enabled = true;
            }
            else if(dtItemInfo.Rows.Count == 1)
            {
                btnAddItem.Enabled = false;
            }
        }
        #endregion

        #region Key Press Events
        private void txtPCS_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

        }

        //private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}

        //private void txtNettWt_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}

        //private void txtDiaWt_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}
        //private void txtDiaAmt_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}

        //private void txtStnWt_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}
        //private void txtStnAmt_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}

        //private void txtTotalAmt_KeyPress(object sender, KeyPressEventArgs e)
        //{
        //    NumericKeyPress(sender, e);
        //}

        private void NumericKeyPress(object sender, KeyPressEventArgs e)
        {
            if(!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }
        #endregion

        #region Get Quantity
        public string GetStandardQuantityFromDB(SqlConnection conn, string itemid)
        {
            if(conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = " SELECT STDQTY FROM INVENTTABLE WHERE ITEMID='" + itemid + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            string sQty = Convert.ToString(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return sQty;

        }
        #endregion

        #region SKU item Checking
        private bool IsSKUItem(string sItemId)
        {
            SqlConnection conn = new SqlConnection();
            if(application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @IsSKU AS INT; SET @IsSKU = 0; IF EXISTS(SELECT TOP(1) [SkuNumber] FROM [SKUTable_Posted] WHERE  [SkuNumber] = '" + sItemId + "') ");
            commandText.Append(" BEGIN SET @IsSKU = 1 END SELECT @IsSKU");


            if(conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            bool bVal = Convert.ToBoolean(command.ExecuteScalar());

            if(conn.State == ConnectionState.Open)
                conn.Close();

            return bVal;

        }
        #endregion

        private void btnAddImage_Click(object sender, EventArgs e)
        {
            if(dtItemInfo != null && dtItemInfo.Rows.Count > 0)
            {
                DataTable dt = new DataTable();

                if(grdView.RowCount > 0)
                {
                    int selectedRow = grdView.GetSelectedRows()[0];
                    DataRow drRowToUpdate = dtItemInfo.Rows[selectedRow];
                    DataTable dtSketchDownload = new DataTable();
                    if(dtSketch != null && dtSketch.Rows.Count > 0)
                    {
                        foreach(DataRow dtDown in dtSketch.Rows)
                        {
                            if(Convert.ToString(drRowToUpdate["UNIQUEID"]) == Convert.ToString(dtDown["UNIQUEID"]))
                            {
                                dtSketchDownload = dtSketch.Clone();
                                dtSketchDownload.ImportRow(dtDown);
                                dtSketchDownload.AcceptChanges();
                                break;
                            }
                        }
                    }

                    if(bView)
                    {
                        if(dtSketch != null && dtSketch.Rows.Count > 0)
                        {
                            foreach(DataRow dtDown in dtSketch.Rows)
                            {
                                dtSketchDownload = dtSketch.Clone();
                                dtSketchDownload.ImportRow(dtDown);
                                dtSketchDownload.AcceptChanges();
                                break;
                            }
                        }
                        else
                        {
                            if(grdView.RowCount > 0)
                            {
                                int selectedRow1 = grdView.GetSelectedRows()[0];
                                DataRow drRowToUpdate1 = dtItemInfo.Rows[selectedRow1];
                                string path = string.Empty;
                                string sArchivePath = GetArchivePathFromImage();
                                
                                path = sArchivePath + "" + Convert.ToString(drRowToUpdate1["ORDERNUM"]) + "_" + Convert.ToInt16(drRowToUpdate1["LINENUM"]) + "_S" + ".jpeg"; //
                                frmOrderSketch objSketch = new frmOrderSketch(path, 1);
                            }
                        }
                    }
                    else
                    {

                        frmOrderSketch oSketch = new frmOrderSketch(dt, Convert.ToString(drRowToUpdate["UNIQUEID"]), dtSketchDownload, linenum);//selectedRow + 1

                        //oSketch.lblBreadCrumbs.Text = lblBreadCrumbs.Text + ">" + " line no:" + " " + Convert.ToInt16(selectedRow + 1);
                        oSketch.ShowDialog();
                        if(oSketch.dtUploadSketch != null && oSketch.dtUploadSketch.Rows.Count > 0)
                        {
                            if(dtSketch != null && dtSketch.Rows.Count > 0)
                            {
                                foreach(DataRow dr in oSketch.dtUploadSketch.Rows)
                                {
                                    foreach(DataRow drSk in dtSketch.Rows)
                                    {
                                        if(Convert.ToString(dr["UNIQUEID"]) == Convert.ToString(drSk["UNIQUEID"]))
                                        {
                                            drSk.Delete();
                                            dtSketch.AcceptChanges();
                                            break;
                                        }
                                    }
                                    dtSketch.ImportRow(dr);
                                }
                                dtSketch.AcceptChanges();
                                oSketch.dtUploadSketch = new DataTable();
                            }
                            else
                            {
                                dtSketch = oSketch.dtUploadSketch;
                            }
                        }
                        else
                        {
                            foreach(DataRow drSk in dtSketch.Rows)
                            {
                                if(Convert.ToString(drRowToUpdate["UNIQUEID"]) == Convert.ToString(drSk["UNIQUEID"]))
                                {
                                    drSk.Delete();
                                    dtSketch.AcceptChanges();
                                    break;
                                }
                            }
                        }
                    }

                }
            }
            //else if(dsSearchedOrder != null && dsSearchedOrder.Tables.Count > 0 && dsSearchedOrder.Tables[1].Rows.Count > 0)
            //{
            //    if(grdView.RowCount > 0)
            //    {
            //        int selectedRow = grdView.GetSelectedRows()[0];
            //        DataRow drRowToUpdate = dsSearchedOrder.Tables[1].Rows[selectedRow];

            //        frmOrderSketch objSketch = new frmOrderSketch(dsSearchedOrder.Tables[1], Convert.ToDecimal(drRowToUpdate["LINENUM"]), "");
            //    }
            //}
            else
            {
                MessageBox.Show("Please enter at least one row to upload the sketch details or " +
                "there are no sketch present for the selected item");
            }
        }

        private void btnStockSample_Click(object sender, EventArgs e)
        {
            if (IsEdit)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("You are in editing mode", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    return;
                }
            }
            Dialog objdialog = new Dialog();
            string str = string.Empty;
            DataSet dsItem = new DataSet();
            
            objdialog.MyItemSearch(500, ref str, out  dsItem, " AND  I.ITEMID IN (SELECT ITEMID FROM INVENTTABLE WHERE RETAIL=1) ");

            saleLineItem = new SaleLineItem();

            if (dsItem != null && dsItem.Tables.Count > 0 && dsItem.Tables[0].Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"])))
                {
                    saleLineItem.ItemId = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);

                    bStockSample = true;

                    Item objItem = new Item();
                    objItem.MYProcessItem(saleLineItem, application);

                    Dimension objDim = new Dimension();
                    DataTable dtDimension = objDim.GetDimensions(saleLineItem.ItemId);


                    if (dtDimension != null && dtDimension.Rows.Count > 0)
                    {
                        DimensionConfirmation dimConfirmation = new DimensionConfirmation();
                        dimConfirmation.InventDimCombination = dtDimension;
                        dimConfirmation.DimensionData = saleLineItem.Dimension;

                        frmDimensions objfrmDim = new frmDimensions(dimConfirmation);
                        objfrmDim.ShowDialog();

                        if (objfrmDim.SelectDimCombination != null)
                        {
                            inventDimId = GetInventID(Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[2]));
                            DataTable dtcmbCode = new DataTable();
                            dtcmbCode.Columns.Add("CodeID", typeof(string));
                            dtcmbCode.Columns.Add("CodeValue", typeof(string));
                            DataRow drCode;
                            drCode = dtcmbCode.NewRow();
                            drCode["CodeID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[4]);
                            drCode["CodeValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[4]);
                            dtcmbCode.Rows.Add(drCode);
                            cmbCode.DataSource = dtcmbCode;
                            cmbCode.DisplayMember = "CodeValue";
                            cmbCode.ValueMember = "CodeID";

                            DataTable dtSize = new DataTable();
                            dtSize.Columns.Add("SizeID", typeof(string));
                            dtSize.Columns.Add("SizeValue", typeof(string));
                            DataRow drSize;
                            drSize = dtSize.NewRow();
                            drSize["SizeID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[3]);
                            drSize["SizeValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[3]);
                            dtSize.Rows.Add(drSize);
                            cmbSize.DataSource = dtSize;
                            cmbSize.DisplayMember = "SizeID";
                            cmbSize.ValueMember = "SizeValue";

                            DataTable dtConfig = new DataTable();
                            dtConfig.Columns.Add("ConfigID", typeof(string));
                            dtConfig.Columns.Add("ConfigValue", typeof(string));
                            DataRow drConfig;
                            drConfig = dtConfig.NewRow();
                            drConfig["ConfigID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[6]);
                            drConfig["ConfigValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[6]);
                            dtConfig.Rows.Add(drConfig);
                            cmbConfig.DataSource = dtConfig;
                            cmbConfig.DisplayMember = "ConfigID";
                            cmbConfig.ValueMember = "ConfigValue";

                            DataTable dtStyle = new DataTable();
                            dtStyle.Columns.Add("StyleID", typeof(string));
                            dtStyle.Columns.Add("StyleValue", typeof(string));
                            DataRow drStyle;
                            drStyle = dtStyle.NewRow();
                            drStyle["StyleID"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[5]);
                            drStyle["StyleValue"] = Convert.ToString(objfrmDim.SelectDimCombination.ItemArray[5]);
                            dtStyle.Rows.Add(drStyle);
                            cmbStyle.DataSource = dtStyle;
                            cmbStyle.DisplayMember = "StyleID";
                            cmbStyle.ValueMember = "StyleValue";
                            cmbConfig.Enabled = false;
                            cmbCode.Enabled = false;
                            cmbSize.Enabled = false;
                            cmbStyle.Enabled = false;

                            Previewdimensions = ColorSizeStyleConfig();
                        }
                    }
                    else
                    {
                        cmbStyle.Text = string.Empty;
                        cmbConfig.Text = string.Empty;
                        cmbCode.Text = string.Empty;
                        cmbSize.Text = string.Empty;
                        cmbConfig.Enabled = false;
                        cmbCode.Enabled = false;
                        cmbSize.Enabled = false;
                        cmbStyle.Enabled = false;
                    }
                    SqlConnection conn = new SqlConnection();
                    if (application != null)
                        conn = application.Settings.Database.Connection;
                    else
                        conn = ApplicationSettings.Database.LocalConnection;
                    txtPCS.Focus();
                    txtPCS.Text = "1";
                    string sQty = string.Empty;
                    sQty = GetStandardQuantityFromDB(conn, Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]));
                    if (!string.IsNullOrEmpty(sQty))
                    {
                        sQty = Convert.ToString(decimal.Round(Convert.ToDecimal(sQty), 3, MidpointRounding.AwayFromZero));
                        txtQuantity.Text = Convert.ToString(Convert.ToDecimal(sQty) == 0 ? string.Empty : Convert.ToString(sQty));
                    }
                    txtItemId.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMID"]);
                    txtItemName.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMNAME"]);
                    //txtRate.Text = getRateFromMetalTable(txtItemId.Text, cmbConfig.Text, cmbStyle.Text, cmbCode.Text, cmbSize.Text, txtQuantity.Text);
                    //if (!string.IsNullOrEmpty(txtRate.Text))
                    //    cmbRateType.SelectedIndex = cmbRateType.FindStringExact("Tot");

                    unitid = saleLineItem.BackofficeSalesOrderUnitOfMeasure;
                    //  txtRate.Text = Convert.ToString(dsItem.Tables[0].Rows[0]["ITEMPRICE"]).Remove(0, 1).Trim();

                    if (Convert.ToString(txtItemId.Text.Trim()) == string.Empty
                        || Convert.ToString(cmbConfig.Text.Trim()) == string.Empty)
                    {
                        sBaseItemID = string.Empty;
                        sBaseConfigID = string.Empty;
                    }
                    else
                    {
                        sBaseItemID = txtItemId.Text.Trim();
                        sBaseConfigID = cmbConfig.Text;
                    }
                }
            }
        }
    }

}
