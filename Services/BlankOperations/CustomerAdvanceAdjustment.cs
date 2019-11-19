using System;
using System.Data.SqlClient;
using System.Data;
using LSRetailPosis.Settings;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations
{
    public class CustomerAdvanceAdjustment
    {
        IPosTransaction pos;
        public CustomerAdvanceAdjustment(IPosTransaction posTransaction)
        {
            pos = posTransaction;
        }

        #region - Changed By Nimbus - FOR AMOUNT ADJUSTMENT
        public DataRow AmountToBeAdjusted(string custAccount, bool isTranIdExists = false, string custaccount = null, string ordernum = null)
        {
            string TransID = string.Empty;

            #region Multiple Adjustment
            if (custaccount != null && ordernum != null)
            {
                BlankOperations oBlank = new BlankOperations();
                DataTable dt = new DataTable();//oBlank.CustomerAdvanceData(custAccount);
                RetailTransaction retailTrans = pos as RetailTransaction;
                //string order = Convert.ToString(retailTrans.PartnerData.AdjustmentOrderNum);
                //string cust = Convert.ToString(retailTrans.PartnerData.AdjustmentCustAccount);

                string sTableName = "ORDERINFO" + "" + ApplicationSettings.Database.TerminalID;
                string order = "";
                string cust = "";
                getMultiAdjOrderNo(sTableName, ref order, ref cust);
                //dropMultiAdjOrderNo(sTableName);

                DataRow drReturn = null;

                try
                {
                    if(PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray;
                        string sStoreId = ApplicationSettings.Terminal.StoreId;
                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("getUnadjustedAdvance", custAccount);

                        DataSet dsWH = new DataSet();
                        StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                        if(Convert.ToString(containerArray[3]).Trim().Length > 38)
                        {
                            dsWH.ReadXml(srTransDetail);
                        }
                        if(dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                        {
                            dt = dsWH.Tables[0];

                            foreach (DataRow drNew in dt.Select("ORDERNUM='" + order + "' AND  CustAccount='" + cust + "'"))// AND ISADJUSTED=0
                            {
                                if(string.IsNullOrEmpty(TransID))
                                    TransID = "'" + Convert.ToString(drNew["TransactionID"]) + "'";
                                else
                                    TransID += ",'" + Convert.ToString(drNew["TransactionID"]) + "'";
                               // drNew["ISADJUSTED"] = 1;
                                drReturn = drNew;
                                break;
                            }
                        }
                        else
                        {
                            using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Active Deposit found for the selected customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                    }
                }
                catch(Exception ex)
                {
                }

                return drReturn;
              
            }
            #endregion

            #region Single Adjustment
            else
            {
                System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem> saleline = new System.Collections.Generic.LinkedList<LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem>(((LSRetailPosis.Transaction.RetailTransaction)(pos)).SaleItems);
                if (isTranIdExists)
                {
                    foreach (var sale in saleline)
                    {
                        if (sale.ItemType == LSRetailPosis.Transaction.Line.SaleItem.SaleLineItem.ItemTypes.Service && !sale.Voided)
                        {
                            if (string.IsNullOrEmpty(TransID))
                                TransID = "'" + sale.PartnerData.ServiceItemCashAdjustmentTransactionID + "'";
                            else
                                TransID += ",'" + sale.PartnerData.ServiceItemCashAdjustmentTransactionID + "'";
                        }

                    }
                }
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                // Create a Command  
                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                DataRow drSelected = null;
              
                try
                {
                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray;
                        string sStoreId = ApplicationSettings.Terminal.StoreId;
                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("getUnadjustedAdvance", custAccount);

                        DataSet dsWH = new DataSet();
                        StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));
                        
                        if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                        {
                            dsWH.ReadXml(srTransDetail);
                        }
                        if (dsWH != null && dsWH.Tables[0].Rows.Count > 0)
                        {
                            DataTable dtWithOutOrder = new DataTable();
                            dtWithOutOrder = dsWH.Tables[0].Clone();

                            foreach (DataRow drNew in dsWH.Tables[0].Select("ORDERNUM=''"))
                            {
                                dtWithOutOrder.ImportRow(drNew);
                            }

                            Dialog.WinFormsTouch.frmGenericSearch OSearch = new Dialog.WinFormsTouch.frmGenericSearch(dtWithOutOrder, drSelected, "Advance Adjustment");
                            OSearch.ShowDialog();
                            drSelected = OSearch.SelectedDataRow;
                        }
                        else
                        {
                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("No Active Deposit found for the selected customer.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                }
                return drSelected;

            }
            #endregion
        }

        private void getMultiAdjOrderNo(string sTableName, ref string sOrder, ref string sCust)
        {
            SqlConnection connection = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            sbQuery.Append(" BEGIN SELECT ISNULL(ORDERNO,'') AS ORDERNO,ISNULL(CUSTACC,'') AS CUSTACC FROM " + sTableName + " END");
            //sbQuery.Append("SELECT ISNULL(ORDERNO,'') AS ORDERNO,ISNULL(CUSTACC,'') AS CUSTACC FROM " + sTableName + "");
            DataTable dtGSS = new DataTable();
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            SqlDataReader reader = cmd.ExecuteReader();

            if(reader.HasRows)
            {
                while(reader.Read())
                {
                    sOrder = Convert.ToString(reader.GetValue(0));
                    sCust = Convert.ToString(reader.GetValue(1));
                }
            }
            reader.Close();
            reader.Dispose();
            if(connection.State == ConnectionState.Open)
                connection.Close();
        }

        private void dropMultiAdjOrderNo(string sTableName)
        {
            SqlConnection connection = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            
            StringBuilder sbQuery = new StringBuilder();
            sbQuery.Append(" IF EXISTS (SELECT A.NAME  FROM SYSOBJECTS A WHERE A.TYPE = 'U' AND A.NAME ='" + sTableName + "')");
            sbQuery.Append(" drop table " + sTableName + "");
            DataTable dtGSS = new DataTable();
            if(connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand command = new SqlCommand(sbQuery.ToString(), connection);
            command.CommandTimeout = 0;
            command.ExecuteScalar();

            if(connection.State == ConnectionState.Open)
                connection.Close();
        }
        #endregion
    }
}
