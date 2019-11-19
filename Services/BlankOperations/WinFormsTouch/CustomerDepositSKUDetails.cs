using System;
using System.Data;
using System.Windows.Forms;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.Data.SqlClient;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class CustomerDepositSKUDetails:LSRetailPosis.POSControls.Touch.frmTouchBase
    {

        #region Variable Declaration
        [Import]
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

        public DataTable dtSkuGridItems = new DataTable();
        int howmanyrows = 0;

        IPosTransaction pos;

        string OrderNo = string.Empty;

        public string lsitems = string.Empty;

        #endregion


        #region Initialization
        public CustomerDepositSKUDetails()
        {

            InitializeComponent();

        }
        public CustomerDepositSKUDetails(DataTable dt, int howManyRows, IPosTransaction posTransaction, string OrderNumber)
        {

            InitializeComponent();
            grItems.DataSource = dt;
            howmanyrows = howManyRows;
            dtSkuGridItems = dt;
            pos = posTransaction;
            OrderNo = OrderNumber;

        }

        public CustomerDepositSKUDetails(int howManyRows, IPosTransaction posTransaction, string OrderNumber)
        {

            InitializeComponent();
            howmanyrows = howManyRows;
            ShowItemSearch(0, howmanyrows);
            pos = posTransaction;
            OrderNo = OrderNumber;
        }
        #endregion


        #region Show Items on the Screen
        private void ShowItemSearch(int fromRow, int numberOfItems)
        {
            string itemid = string.Empty;
            DataTable dtSKUItems = new DataTable();

            string commandText = " SELECT ITEMID, ITEMNAME, I.UNITOFMEASURE FROM (   " +
                                 " SELECT IT.ITEMID, COALESCE(TR.NAME, IT.ITEMNAME, IT.ITEMID) AS ITEMNAME, IT.DATAAREAID,  " +
                                 "  ISNULL(IM.UNITID, '') AS UNITOFMEASURE, ROW_NUMBER()     OVER (ORDER BY IT.ITEMNAME  ASC) AS ROW  " +
                                 " FROM ASSORTEDINVENTITEMS IT     JOIN INVENTTABLEMODULE IM ON IT.ITEMID = IM.ITEMID AND IM.MODULETYPE = 2   " +
                                 " JOIN ECORESPRODUCT AS PR ON PR.RECID = IT.PRODUCT   " +
                //    "  INNER JOIN SKUTable_Posted ON IT.ITEMID = SKUTable_Posted.SkuNumber " +
                                 "  INNER JOIN SKUTableTrans ON IT.ITEMID = SKUTableTrans.SkuNumber " +  //SKU Table 
                                 " LEFT JOIN ECORESPRODUCTTRANSLATION AS TR  " +
                                 " ON PR.RECID = TR.PRODUCT AND TR.LANGUAGEID = @CULTUREID     WHERE IT.STORERECID = @STORERECID  " +
                //   " AND SKUTable_Posted.isAvailable='True' AND SKUTable_Posted.isLocked='False' " +
                                 " AND SKUTableTrans.isAvailable='True' AND SKUTableTrans.isLocked='False' " + //SKU Table 
                                 "  ) I WHERE I.DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' AND I.ROW > @FROMROW AND I.ROW <= @TOROW ";


            SqlConnection connection = new SqlConnection();

            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if(connection.State == ConnectionState.Closed)
                connection.Open();


            SqlCommand command = new SqlCommand(commandText, connection);

            SqlParameter languageIdParm = command.Parameters.Add("@CULTUREID", SqlDbType.NVarChar, 7);
            languageIdParm.Value = ApplicationSettings.Terminal.CultureName;
            SqlParameter dataAreaIdParm = command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
            dataAreaIdParm.Value = ApplicationSettings.Database.DATAAREAID;
            command.Parameters.AddWithValue("@FROMROW", fromRow);
            command.Parameters.AddWithValue("@TOROW", (fromRow + numberOfItems));
            command.Parameters.AddWithValue("@STORERECID", ApplicationSettings.Terminal.StorePrimaryId);


            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            dtSKUItems.Load(reader);

            if(connection.State == ConnectionState.Open)
                connection.Close();


            if(dtSKUItems != null && dtSKUItems.Rows.Count > 0)
            {
                DataRow drSelected = null;
                Dialog.Dialog oDialog = new Dialog.Dialog();
                oDialog.GenericSearch(dtSKUItems, ref drSelected,"Select SKU");
                if(dtSkuGridItems != null && dtSkuGridItems.Rows.Count > 0)
                {
                    if(drSelected != null)
                    {
                        itemid = Convert.ToString(drSelected["ITEMID"]);
                        dtSkuGridItems.ImportRow(drSelected);
                    }
                }
                else
                {
                    if(dtSkuGridItems != null && dtSkuGridItems.Columns.Count > 0)
                    {
                        if(drSelected != null)
                        {
                            itemid = Convert.ToString(drSelected["ITEMID"]);
                            dtSkuGridItems.ImportRow(drSelected);
                        }
                    }
                    else
                    {
                        dtSkuGridItems = dtSKUItems.Clone();
                        dtSkuGridItems.Columns.Add("SKUBOOKINGDATE", typeof(DateTime));

                        if(drSelected != null)
                        {

                            itemid = Convert.ToString(drSelected["ITEMID"]);
                            dtSkuGridItems.ImportRow(drSelected);

                            dtSkuGridItems.AcceptChanges();
                        }
                    }
                }


                UpdateSKUTable(itemid);
                foreach(DataRow dr in dtSkuGridItems.Rows)
                    dr["SKUBOOKINGDATE"] = DateTime.Now.ToShortDateString();
                grItems.DataSource = dtSkuGridItems;

            }
            else
            {
                using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Item Present.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);

                }
            }
        }
        #endregion


        #region SKU Table
        private void UpdateSKUTable(string itemnumber, bool isdeleted = false)
        {
            string commandText = string.Empty;
            if(!isdeleted)
                commandText = " UPDATE SKUTableTrans SET isLocked='True' WHERE SkuNumber = '" + itemnumber + "' "; //SKU Table 
            else
            {
                string sStoreId = ApplicationSettings.Terminal.StoreId;
                string sTerminalId = ApplicationSettings.Terminal.TerminalId;

                commandText = " IF EXISTS(SELECT TOP 1 * FROM RETAILCUSTOMERDEPOSITSKUDETAILS " +
                              " WHERE TRANSID='" + Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(pos)).TransactionId) + "' " +
                              " AND SKUNUMBER='" + itemnumber + "' ) BEGIN " + //AND STOREID = '" + sStoreId + "' AND TERMINALID = '" + sTerminalId + "'
                              " DELETE FROM RETAILCUSTOMERDEPOSITSKUDETAILS WHERE  TRANSID='" + Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(pos)).TransactionId) + "' " +
                              " AND SKUNUMBER='" + itemnumber + "' END " +  //AND STOREID = '" + sStoreId + "' AND TERMINALID = '" + sTerminalId + "'
                    //  " UPDATE SKUTable_Posted SET isLocked='False' WHERE SkuNumber = '" + itemnumber + "' ";
                              " UPDATE SKUTableTrans SET isLocked='False' WHERE SkuNumber = '" + itemnumber + "' "; //SKU Table 
            }

            SqlConnection connection = new SqlConnection();
            if(application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if(connection.State == ConnectionState.Closed)
                connection.Open();


            SqlCommand command = new SqlCommand(commandText, connection);


            command.CommandTimeout = 0;
            command.ExecuteNonQuery();

            if(connection.State == ConnectionState.Open)
                connection.Close();

        }
        #endregion


        #region Add More Items Click
        private void btnPOSItemSearch_Click(object sender, EventArgs e)
        {
            ShowItemSearch(0, howmanyrows);
        }
        #endregion


        #region Close Click
        private void btnClose_Click(object sender, EventArgs e)
        {
            SaveSKUItems();
            this.Close();
        }
        #endregion


        #region Delete Click
        private void btnDelete_Click(object sender, EventArgs e)
        {
            int OrderSelectedIndex = 0;
            if(dtSkuGridItems != null && dtSkuGridItems.Rows.Count > 0)
            {
                if(grdView.RowCount > 0)
                {
                    OrderSelectedIndex = grdView.GetSelectedRows()[0];
                    DataRow theRowToDelete = dtSkuGridItems.Rows[OrderSelectedIndex];
                    string itemid = string.Empty;
                    itemid = Convert.ToString(theRowToDelete["ITEMID"]);
                    dtSkuGridItems.Rows.Remove(theRowToDelete);
                    dtSkuGridItems.AcceptChanges();
                    grItems.DataSource = dtSkuGridItems;

                    UpdateSKUTable(Convert.ToString(itemid), true);
                }
            }
        }
        #endregion

        #region Save Items to the Table

        private void SaveSKUItems()
        {

            string commandText = " IF NOT EXISTS(SELECT TOP 1 * FROM RETAILCUSTOMERDEPOSITSKUDETAILS " +
                                   " WHERE TRANSID=@TRANSID " +
                                   " AND SKUNUMBER=@SKUNUMBER) BEGIN  " +
                                   " INSERT INTO [RETAILCUSTOMERDEPOSITSKUDETAILS] " +
                                   " ([TRANSID],[CUSTOMERID],[ORDERNUMBER],[SKUNUMBER],[SKUBOOKINGDATE],[SKURELEASEDDATE] " +
                                   " ,[SKUSALEDATE],[DELIVERED],[STOREID],[TERMINALID],[DATAAREAID],[STAFFID]) " +
                                   "  VALUES " +
                                   " (@TRANSID,@CUSTOMERID,@ORDERNUMBER,@SKUNUMBER,@SKUBOOKINGDATE,@SKURELEASEDDATE,@SKUSALEDATE " +
                                   " ,@DELIVERED,@STOREID,@TERMINALID,@DATAAREAID,@STAFFID) END ";
            SqlConnection connection = new SqlConnection();
            try
            {
                if(application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if(connection.State == ConnectionState.Closed)
                    connection.Open();



                string sItems = string.Empty;
                if(dtSkuGridItems != null && dtSkuGridItems.Rows.Count > 0)
                {
                    lsitems = string.Empty;
                    for(int ItemCount = 0; ItemCount < dtSkuGridItems.Rows.Count; ItemCount++)
                    {
                        DateTime dob = Convert.ToDateTime(Convert.ToString(dtSkuGridItems.Rows[ItemCount]["SKUBOOKINGDATE"]));

                        SqlCommand command = new SqlCommand(commandText, connection);
                        command.Parameters.Clear();
                        if(ItemCount == dtSkuGridItems.Rows.Count - 1)
                            lsitems = lsitems + "'" + (Convert.ToString(dtSkuGridItems.Rows[ItemCount]["ITEMID"])) + "'";
                        else
                            lsitems = lsitems + "'" + (Convert.ToString(dtSkuGridItems.Rows[ItemCount]["ITEMID"])) + "',";
                        command.Parameters.Add("@TRANSID", SqlDbType.NVarChar, 20).Value = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(pos)).TransactionId);
                        command.Parameters.Add("@CUSTOMERID", SqlDbType.NVarChar, 20).Value = Convert.ToString(((LSRetailPosis.Transaction.CustomerPaymentTransaction)(pos)).Customer.CustomerId);
                        command.Parameters.Add("@ORDERNUMBER", SqlDbType.NVarChar, 20).Value = OrderNo;
                        command.Parameters.Add("@SKUNUMBER", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSkuGridItems.Rows[ItemCount]["ITEMID"]);
                        command.Parameters.Add("@SKUBOOKINGDATE", SqlDbType.Date).Value = dob;
                        command.Parameters.Add("@SKURELEASEDDATE", SqlDbType.Date).Value = Convert.ToDateTime("1/1/1900");
                        command.Parameters.Add("@SKUSALEDATE", SqlDbType.Date).Value = Convert.ToDateTime("1/1/1900");
                        command.Parameters.Add("@DELIVERED", SqlDbType.Int).Value = 0;
                        command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.StoreId;
                        command.Parameters.Add("@TERMINALID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Terminal.TerminalId;
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 20).Value = ApplicationSettings.Database.DATAAREAID;

                        command.Parameters.Add("@STAFFID", SqlDbType.NVarChar, 20).Value = Convert.ToString(((LSRetailPosis.Transaction.PosTransaction)(pos)).OperatorId);


                        command.CommandTimeout = 0;
                        command.ExecuteNonQuery();
                    }
                }
            }
            catch(Exception)
            {
                if(connection.State == ConnectionState.Open)
                    connection.Close();
            }

            if(connection.State == ConnectionState.Open)
                connection.Close();
        }

        #endregion

    }
}
