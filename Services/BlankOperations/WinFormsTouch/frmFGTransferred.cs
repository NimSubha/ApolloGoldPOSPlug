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
    public partial class frmFGTransferred : frmTouchBase
    {
        string sInventLocationId = string.Empty;

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
        public frmFGTransferred()
        {
            InitializeComponent();
            GetWarehouseInfo();
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

        private void btnGTId_Click(object sender, EventArgs e)
        {
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
           
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetGTIdFGTransfer", txtVendorAc.Text, sInventLocationId);

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

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            try
            {
                ReadOnlyCollection<object> containerArray;
                string sMsg = string.Empty;

                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("UpdateFGTransfer", txtGTId.Text.Trim());
                    sMsg = Convert.ToString(containerArray[2]);
                    MessageBox.Show(sMsg);
                    ClearControls();
                }
            }
            catch (Exception ex)
            {


            }

        }

        private void ClearControls()
        {
            txtVendorAc.Text = string.Empty;
            txtVendorName.Text = string.Empty;
            txtGTId.Text = string.Empty;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
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
