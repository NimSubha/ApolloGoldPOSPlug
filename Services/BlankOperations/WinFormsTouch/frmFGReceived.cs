using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using System.Collections.ObjectModel;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.IO;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmFGReceived : frmTouchBase
    {
        string sInventLocationId = string.Empty;
        DataSet dsGTSKU;
        //Start : added on 26/05/2014
        int iNoSku = 0;
        int iNoOfSetOf = 0;
        decimal dTotQty = 0;
        //End : added on 26/05/2014

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

        public frmFGReceived()
        {
            InitializeComponent();
            GetWarehouseInfo();
            
        }

        private void btnWHSearch_Click(object sender, EventArgs e)
        {
           try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetWarehouse", sStoreId);

                    DataSet dsWH = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsWH.ReadXml(srTransDetail);
                    }
                    if (dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                    {
                        Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dsWH.Tables[0], null, "Warehouse");
                        Osearch.ShowDialog();

                        DataRow dr = Osearch.SelectedDataRow;

                        if (dr != null)
                        {
                            sInventLocationId = Convert.ToString(dr["InventLocationId"]);
                            txtWarehouse.Text = Convert.ToString(dr["Name"]);
                        }
                    }
                }
            }
             catch (Exception ex)
             {


             }
        }

        private void btnVendorSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetVendAccount");

                    DataSet dsVendor = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)  
                    {
                        dsVendor.ReadXml(srTransDetail);
                    }
                    if (dsVendor != null && dsVendor.Tables[0].Rows.Count > 0)
                    {
                        Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dsVendor.Tables[0], null, "Vendor");
                        Osearch.ShowDialog();

                        DataRow dr = Osearch.SelectedDataRow;

                        if (dr != null)
                        {
                            txtVendorAc.Text = Convert.ToString(dr["AccountNum"]);
                            txtVendorName.Text = Convert.ToString(dr["Name"]);
                        }
                    
                    }
                }
            }
            catch (Exception ex)
            {


            }
        }

        private void btnGTId_Click(object sender, EventArgs e)
        {
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetGTId", txtVendorAc.Text, sInventLocationId);

                    DataSet dsGTId = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsGTId.ReadXml(srTransDetail);
                    }
                    if (dsGTId != null && dsGTId.Tables[0].Rows.Count > 0)
                    {
                        Dialog.WinFormsTouch.frmGenericSearch Osearch = new Dialog.WinFormsTouch.frmGenericSearch(dsGTId.Tables[0], null, "Goods Transferred Id");
                        Osearch.ShowDialog();

                        DataRow dr = Osearch.SelectedDataRow;

                        if (dr != null)
                        {
                            txtGTId.Text = Convert.ToString(dr["GTID"]);
                        }

                    }
                }
            }
            catch (Exception ex)
            {


            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            

            if (dsGTSKU != null && dsGTSKU.Tables[0].Rows.Count > 0)
            {
                return;
            }
            if (string.IsNullOrEmpty(txtVendorAc.Text))
            {
                MessageBox.Show("Select vendor");
                return;
            }
            if (string.IsNullOrEmpty(sInventLocationId))
            {
                MessageBox.Show("Select warehouse");
                return;
            }
            if (string.IsNullOrEmpty(txtGTId.Text))
            {
                MessageBox.Show("Select Goods Transferred Id");
                return;
            }
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetGTIdSKUNumber", txtGTId.Text);
                    dsGTSKU = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));
                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsGTSKU.ReadXml(srTransDetail);
                    }
                    if (dsGTSKU != null && dsGTSKU.Tables[0].Rows.Count > 0)
                    {
                        // Pcs -- Qty -- Acknowledged
                        dsGTSKU.Tables[0].Columns.Add("WH", typeof(string));
                        dsGTSKU.Tables[0].Columns.Add("GTID", typeof(string));
                        dsGTSKU.Tables[0].Columns.Add("Acknowledged", typeof(bool));
                        dsGTSKU.Tables[0].AcceptChanges();

                        foreach (DataRow dr in dsGTSKU.Tables[0].Rows)
                        {
                            dr["WH"] = sInventLocationId.Trim();
                            dr["GTID"] = txtGTId.Text.Trim();
                            dr["Acknowledged"] = 0;
                            dsGTSKU.Tables[0].AcceptChanges();
                        }
                        grItems.DataSource = dsGTSKU.Tables[0].DefaultView;

                       
                    }
                }
            }
            catch (Exception ex)
            {


            }

        }

        private void btnAcknowledge_Click(object sender, EventArgs e)
        {

            if ((!string.IsNullOrEmpty(txtAckSKUNo.Text)) && dsGTSKU != null && dsGTSKU.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow dr in dsGTSKU.Tables[0].Rows)
                {
                    if (txtAckSKUNo.Text.Trim().ToUpper() == Convert.ToString(dr["SKUNumber"]).Trim().ToUpper())
                    {
                        dr["Acknowledged"] = 1;
                        dsGTSKU.Tables[0].AcceptChanges();
                        txtAckSKUNo.Text = string.Empty;


                        //Start : added on 26/05/2014                       
                        iNoSku = iNoSku + 1;
                        iNoOfSetOf = iNoOfSetOf + Convert.ToInt32(Convert.ToDecimal(dr[6]));
                        dTotQty = dTotQty + Convert.ToDecimal(dr[7]);
                        
                        lblTotNoOfSKU.Text = Convert.ToString(iNoSku);
                        lblTotSetOf.Text = Convert.ToString(iNoOfSetOf);
                        lblTotQty.Text = Convert.ToString(dTotQty);
                        //End : added on 26/05/2014
                    }
                }
            }
        }

        private void btnPost_Click(object sender, EventArgs e)
        {
            if (dsGTSKU != null && dsGTSKU.Tables[0].Rows.Count > 0)
            {
                DataRow drXML;
                DataTable dt = new DataTable("FGDETAILS");

                dt.Columns.Add("SKUNumber", typeof(string));
                dt.Columns.Add("WH", typeof(string));
                dt.Columns.Add("GTID", typeof(string));
                dt.Columns.Add("ACKNOWLEDGED", typeof(string));

                dt.AcceptChanges();

                foreach (DataRow dr in dsGTSKU.Tables[0].Rows)
                {
                    drXML = dt.NewRow();

                    drXML["SKUNumber"] = Convert.ToString(dr["SKUNumber"]);
                    drXML["WH"] = Convert.ToString(dr["WH"]);
                    drXML["GTID"] = Convert.ToString(dr["GTID"]);
                    drXML["ACKNOWLEDGED"] = Convert.ToBoolean(dr["Acknowledged"]) ? "1":"0";

                    dt.Rows.Add(drXML);
                    dt.AcceptChanges();
                }

                try
                {
                    ReadOnlyCollection<object> containerArray;
                    string sMsg = string.Empty;
                    MemoryStream mstr = new MemoryStream();
                    dt.WriteXml(mstr, true);
                    mstr.Seek(0, SeekOrigin.Begin);
                    StreamReader sr = new StreamReader(mstr);
                    string sSKU = string.Empty;
                    sSKU = sr.ReadToEnd();
                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                       containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("FGReceiveCreate", sSKU);
                       sMsg = Convert.ToString(containerArray[2]);
                       MessageBox.Show(sMsg);
                       ClearControls();
                    }
                }
                
                catch (Exception ex)
                {


                }
            }
        }

        private void ClearGrid()
        {
            txtGTId.Text = string.Empty;
            dsGTSKU = null;
            grItems.DataSource = null;
        }
        private void ClearControls()
        {
            txtVendorAc.Text = string.Empty;
            txtVendorName.Text = string.Empty;
            txtGTId.Text = string.Empty;
            dsGTSKU = null;
            grItems.DataSource = null;
            lblTotNoOfSKU.Text = "";
            lblTotQty.Text = "";
            lblTotSetOf.Text = "";
            dTotQty = 0;
            iNoSku = 0;
            iNoOfSetOf = 0;
        }
        
        private void txtVendorAc_TextChanged(object sender, EventArgs e)
        {
            ClearGrid();
        }

        private void txtWarehouse_TextChanged(object sender, EventArgs e)
        {
            ClearGrid();
        }

        private void txtGTId_TextChanged(object sender, EventArgs e)
        {
            dsGTSKU = null;
            grItems.DataSource = null;
        }

        private void txtAckSKUNo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyValue == 13) // this line added on 27/05/2014
            {
                if ((!string.IsNullOrEmpty(txtAckSKUNo.Text)) && dsGTSKU != null && dsGTSKU.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow dr in dsGTSKU.Tables[0].Rows)
                    {
                        if (txtAckSKUNo.Text.Trim().ToUpper() == Convert.ToString(dr["SKUNumber"]).Trim().ToUpper())
                        {
                            dr["Acknowledged"] = 1;
                            dsGTSKU.Tables[0].AcceptChanges();
                            txtAckSKUNo.Text = string.Empty;

                            //Start : added on 26/05/2014                       
                            iNoSku = iNoSku + 1;
                            iNoOfSetOf = iNoOfSetOf + Convert.ToInt32(Convert.ToDecimal(dr[6]));
                            dTotQty = dTotQty + Convert.ToDecimal(dr[7]);

                            lblTotNoOfSKU.Text = Convert.ToString(iNoSku);
                            lblTotSetOf.Text = Convert.ToString(iNoOfSetOf);
                            lblTotQty.Text = Convert.ToString(dTotQty);
                            //End : added on 26/05/2014
                        }
                    }
                }
            }

        }

        private void GetWarehouseInfo()
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string storeId = ApplicationSettings.Database.StoreID;
            string sDataAreaId = ApplicationSettings.Database.DATAAREAID;

            StringBuilder commandText = new StringBuilder();

            commandText.Append(" SELECT C.INVENTLOCATIONID,C.NAME FROM RETAILCHANNELTABLE A INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE B ON A.RECID = B.RECID INNER JOIN INVENTLOCATION C ");
            commandText.Append(" ON A.INVENTLOCATION = C.INVENTLOCATIONID ");
            commandText.Append(" WHERE B.STORENUMBER='" + storeId + "' AND C.DATAAREAID = '" + sDataAreaId + "'  ");

            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }


            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;

            SqlDataReader reader = command.ExecuteReader();

            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sInventLocationId = Convert.ToString(reader.GetValue(0));
                    txtWarehouse.Text = Convert.ToString(reader.GetValue(1));
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();


        }

    }
}
