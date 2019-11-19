using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using LSRetailPosis.Settings;
using System.Data.SqlClient;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;
using Microsoft.Reporting.WinForms;
using System.IO;
using Microsoft.Dynamics.Retail.Pos.Dialog;

namespace BlankOperations.WinFormsTouch
{
    public partial class frmCustOrderSampleReturn : frmTouchBase
    {
        #region POS Application Vars
        public IPosTransaction pos { get; set; }
       
        [Import]
        private IApplication application;
        #endregion

        #region Member Vars
        public DataTable dtSample = new DataTable();
        public DataTable dtStone = new DataTable();//added on 16/09/2014 by Palas
        DataTable dtNotReturn = new DataTable();
        frmOrderDetails frmOrderDtls = null;

        Int16 iIsStoneRet = 0;//added on 16/09/2014 by Palas
        string sTableName = string.Empty;
        string sStnFlg = string.Empty;//added on 16/09/2014 by Palas
        string sfrmTitle = string.Empty;//added on 16/09/2014 by Palas
        #region Included by Palas @ 19-11-2014
        string sOrderNum = string.Empty;
        public RetailTransaction retailTrans { get; set; }
        frmCustomerOrder objCustomerOrder;
        clApprovalSales _clApprSale;
        private string sCPinCode;
        #endregion
        #endregion

        enum RateType
        {
            Weight = 0,
            Pcs = 1,
            Tot = 2,
        }
              
        enum MakingType
        {
            Weight = 2,
            Pieces = 0,
            Tot = 3,
            Percentage = 4,
        }

        enum WastageType // Added for wastage 
        {
            //  None    = 0,
            Weight = 0,
            Percentage = 1,
        }
       

        // Int16 iISStnRet = 0 added on 16/09/2014 by Palas
        public frmCustOrderSampleReturn(IPosTransaction posTransaction, IApplication Application, DataTable dt, frmOrderDetails frmOrdDtl, Int16 iISStnRet = 0)
        {
            InitializeComponent();

            pos = posTransaction;
            application = Application;
            frmOrderDtls = frmOrdDtl;
            
           
            
            //modified on 16/09/2014 by Palas
            iIsStoneRet = iISStnRet;
            if (iIsStoneRet == 1)
            {
                dtStone = dt;
                dtNotReturn = null;
                dtNotReturn = dtStone.Clone();
                dtNotReturn.Columns["ISRETURNED"].DataType = typeof(bool);// change the datatype of the column ISRETURNED(int) to bool on 24/09/2014
                
                dtStone.Select("ISRETURNED=false").CopyToDataTable(dtNotReturn, LoadOption.PreserveChanges);
                sfrmTitle = "Customer Order Stone Return";
                colGrWt.Visible = false;
                colNtWt.Visible = false;
                colTotalAmt.Visible = false;
                colSize.Visible = true;
                colColor.Visible = true;
                colItemId.VisibleIndex = 1;
                colColor.VisibleIndex = 2;
                colColor.Caption = "Code";
                colSize.VisibleIndex = 3;
                colPCS.VisibleIndex = 4;
                colDiaWt.VisibleIndex = 5;
                colDiaAmt.VisibleIndex = 6;
                colStnWt.VisibleIndex = 7;
                colStnAmt.VisibleIndex = 8;
                colRemarks.VisibleIndex = 9;
                grItems.DataSource = dtNotReturn.DefaultView;
            }
            else
            {
                dtSample = dt;
                dtNotReturn = null;
                dtNotReturn = dtSample.Clone();
                dtNotReturn.Columns["ISRETURNED"].DataType = typeof(bool);// change the datatype of the column ISRETURNED(int) to bool on 24/09/2014
                dtSample.Select("ISRETURNED=False").CopyToDataTable(dtNotReturn, LoadOption.PreserveChanges);
                sfrmTitle = "Customer Order Sample Return";
                
                grItems.DataSource = dtNotReturn.DefaultView;
            }
            label1.Text = sfrmTitle;
            this.Text = sfrmTitle;

            //grItems.DataSource = dtNotReturn.DefaultView;
        }

        #region Overload the Constructor frmCustOrderSampleReturn Start from 19-11-2014 @Palas Jana
        public frmCustOrderSampleReturn(IPosTransaction posTransaction, IApplication Application, string sCustOrderNo, bool btxtBoxVisible, Int16 iOption = 0)
        {
            InitializeComponent();

            sOrderNum = sCustOrderNo;
            pos = posTransaction;
            application = Application;
            retailTrans = pos as RetailTransaction;
            if (retailTrans != null)
            {
                if (!string.IsNullOrEmpty(retailTrans.Customer.PostalCode))

                    sCPinCode = Convert.ToString(retailTrans.Customer.PostalCode);

            }
            else
            {
                sCPinCode = Convert.ToString(application.BusinessLogic.CustomerSystem.GetCustomerInfo(null).PostalCode);
            }
            pnlEntry.Visible = btxtBoxVisible;
            iIsStoneRet = iOption;
            if (iIsStoneRet == 0)
                label1.Text = "Client's Order Sample Return";
            else
                label1.Text = "Client's Order Stone Return";

        }

        #endregion
        #region Customer Order Sample Return,//Nimbus by MIAM @ 10Jun14 : update in CUSTORDSAMPLE and CUSTORDER_HEADER
        private void btnReturn_Click(object sender, EventArgs e)
        {
            if (dtNotReturn.Rows.Count > 0)
            {
                string orderno = string.Empty;
                StringBuilder commandText1 = new StringBuilder();
                   //added or modified on 16/09/2014 by Palas
                if (iIsStoneRet == 1)
                {
                    sTableName = "CUSTORDSTONE";
                    sStnFlg = "STONEFLAG";
                }
                else
                {
                    sTableName = "CUSTORDSAMPLE";
                    sStnFlg = "SAMPLEFLAG";
                }
                  //added or modified on 16/09/2014 by Palas
                foreach (DataRow dr in dtNotReturn.Rows)
                {
                    orderno = dr["ORDERNUM"].ToString();
                    if (Convert.ToBoolean(dr["ISRETURNED"]))
                    {
                        commandText1.Append("UPDATE " + sTableName + " SET ISRETURNED=1 ");
                        commandText1.AppendFormat(" WHERE ORDERNUM='{0}' ", dr["ORDERNUM"]);
                        commandText1.AppendFormat(" AND LINENUM='{0}' ", dr["LINENUM"]);
                        commandText1.AppendFormat(" AND STOREID='{0}' ", ApplicationSettings.Terminal.StoreId);
                        commandText1.AppendFormat(" AND TERMINALID='{0}' ", ApplicationSettings.Terminal.TerminalId);
                        commandText1.AppendFormat(" AND DATAAREAID='{0}'; ", ApplicationSettings.Database.DATAAREAID);
                    }
                }
                commandText1.AppendLine();
                commandText1.AppendLine("DECLARE @SAMRETCOUNT INT;");
                commandText1.AppendLine(" SELECT @SAMRETCOUNT=COUNT(*) FROM "+sTableName+" ");
                commandText1.AppendFormat(" WHERE ORDERNUM='{0}' ", orderno);
                commandText1.AppendLine(" AND ISRETURNED=0");
                commandText1.AppendFormat(" AND STOREID='{0}' ", ApplicationSettings.Terminal.StoreId);
                commandText1.AppendFormat(" AND TERMINALID='{0}' ", ApplicationSettings.Terminal.TerminalId);
                commandText1.AppendFormat(" AND DATAAREAID='{0}'; ", ApplicationSettings.Database.DATAAREAID);
                commandText1.AppendLine("IF @SAMRETCOUNT <=0 BEGIN UPDATE CUSTORDER_HEADER SET " + sStnFlg + " =0 ");
                commandText1.AppendFormat(" WHERE ORDERNUM='{0}' ", orderno);
                commandText1.AppendFormat(" AND STOREID='{0}' ", ApplicationSettings.Terminal.StoreId);
                commandText1.AppendFormat(" AND TERMINALID='{0}' ", ApplicationSettings.Terminal.TerminalId);
                commandText1.AppendFormat(" AND DATAAREAID='{0}' END; ", ApplicationSettings.Database.DATAAREAID);

                SqlConnection connection = new SqlConnection();
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;

                try
                {
                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    using (SqlCommand command = new SqlCommand(commandText1.ToString(), connection))
                    {
                        string Msg = string.Empty;
                        command.CommandTimeout = 0;
                        if (command.ExecuteNonQuery() > 0)
                        {
                            //update local sample atble
                            commandText1 = new StringBuilder();
                            if (iIsStoneRet == 0)
                            {
                                commandText1.AppendLine(" SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID] ");
                                commandText1.AppendLine(" ,[CONFIGID] [CONFIGURATION],[CODE] [COLOR],[SIZEID] [SIZE],[STYLE],[INVENTDIMID],[PCS],[QTY] [QUANTITY],[NETTWT],[DIAWT],[DIAAMT],[STNWT] ");
                                commandText1.AppendLine(" ,[STNAMT],[TOTALAMT],[DATAAREAID],[STAFFID],[REPLICATIONCOUNTER],[UNITID],[REMARKSDTL] [RemarksDtl],[ISRETURNED] ");
                                commandText1.AppendLine(" FROM [CUSTORDSAMPLE] WHERE ORDERNUM='" + orderno + "' ");
                                Msg="Customer Order Sample has been returned successfully.";
                            }
                            else
                            {
                                commandText1.AppendLine(" SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID],[CODE] [COLOR],[SIZEID] [SIZE] ");
                                commandText1.AppendLine(" ,[PCS],[DIAWT],[DIAAMT],[STNWT]");
                                commandText1.AppendLine(" ,[STNAMT],[DATAAREAID],[STAFFID],[REPLICATIONCOUNTER],[REMARKSDTL] [RemarksDtl],[ISRETURNED] ");
                                commandText1.AppendLine(" FROM [CUSTORDSTONE] WHERE ORDERNUM='" + orderno + "' ");
                                Msg = "Stones have been returned successfully.";
                            }


                            SqlCommand command1 = new SqlCommand(commandText1.ToString(), connection);
                            command1.CommandTimeout = 0;
                            SqlDataAdapter da = new SqlDataAdapter(command1);
                            DataTable dtNewSample = new DataTable();
                            da.Fill(dtNewSample);
                            if (dtNewSample.Rows.Count > 0 && frmOrderDtls != null)
                            {
                                frmOrderDtls.UpdatelocalSampleTable(dtNewSample);
                            }

                            using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(Msg, MessageBoxButtons.OK, MessageBoxIcon.Information))
                            {
                                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                                this.Close();
                            }
                        }
                        command.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(ex.Message.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                    {
                        connection.Close();
                    }
                    orderno = Convert.ToString(dtNotReturn.Rows[0]["ORDERNUM"]);
                   // PrintVoucher(orderno, iIsStoneRet,dtNotReturn);
                }
            }
            else
            {
                this.Close();
            }
        }
        #endregion
        #region Print Voucher Method created by Palas Jana @ 19-11-2014
        private void PrintVoucher(string sOrderNo, int OptionValue, DataTable dtNotReturn)
        {
            Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations oBlank = new Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations();
            List<ReportDataSource> rds = new List<ReportDataSource>();
            rds.Add(new ReportDataSource("HEADERINFO", oBlank.GetHeaderInfo(sOrderNo)));
            sCPinCode = oBlank.sPincode;//Get Pincode from Blank Application
            //rds.Add(new ReportDataSource("BARCODEIMGTABLE", (DataTable)oBlank.GetBarcodeInfo(sOrderNo)));
            string sArchivePath = oBlank.GetArchivePathFromImage();
            List<ReportParameter> rps = new List<ReportParameter>();
            //Pass Common Parameters
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            rps.Add(new ReportParameter("prmCompany", oBlank.GetCompanyName(connection)));
            rps.Add(new ReportParameter("prmBarcode", sOrderNo));
            rps.Add(new ReportParameter("prmPinCode", string.IsNullOrEmpty(sCPinCode) ? " " : sCPinCode, true));

            string reportPath = string.Empty;
            string reportName = string.Empty;
            RdlcViewer rptView = null;
            if (OptionValue == 0)
            {
                dtNotReturn.Columns.Add("LINEIMAGE", typeof(string));
                int i = 1;
                foreach (DataRow dr in dtNotReturn.Rows)
                {
                    //string path = sArchivePath + "_" + dr["LINENUM"] + "_S" + ".jpeg"; //
                    string path = sArchivePath + "" + sOrderNo + "_" + i + "_S" + ".jpeg"; //

                    if (File.Exists(path))
                    {
                        Image img = Image.FromFile(path);
                        byte[] arr;
                        using (MemoryStream ms1 = new MemoryStream())
                        {
                            img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                            arr = ms1.ToArray();
                        }

                        dr["LINEIMAGE"] = Convert.ToBase64String(arr);
                    }
                    i++;
                }
                rds.Add(new ReportDataSource("CUSTORDSAMPLE", dtNotReturn));
                decimal dTotalAmt = 0;
                foreach (DataRow dr in dtNotReturn.Rows)
                {
                    dTotalAmt += Convert.ToDecimal(dr["TOTALAMT"]);
                }
                DataTable dtDetailsInfo = oBlank.GetDetailInfo(sOrderNo);
                decimal dGrswt = 0;
                foreach (DataRow dr in dtDetailsInfo.Rows)
                {
                    dGrswt += Convert.ToDecimal(dr["QTY"]);
                }
                string sGrossWt = Convert.ToString(dGrswt);
                reportName = @"rptCustOrderSampleReturn";
                _clApprSale = new clApprovalSales(pos);

                rps.Add(new ReportParameter("prmGrossWt", sGrossWt));
                rps.Add(new ReportParameter("prmTotalInWrd", _clApprSale.Amtinwds(Convert.ToDouble(dTotalAmt))));
                reportPath = reportPath + @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";
                rptView = new RdlcViewer("Client's Sample Return Voucher", reportPath, rds, rps, null);
            }
            else
            {
                DataTable dtreturned = new DataTable();
                dtreturned = dtNotReturn.Clone();
                dtNotReturn.Select("ISRETURNED=True").CopyToDataTable(dtreturned, LoadOption.PreserveChanges);
                
                rds.Add(new ReportDataSource("CUSTORDSTONE", dtreturned));
                reportName = @"rptCustOrderStoneReturn";
                reportPath = reportPath + @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";
                rptView = new RdlcViewer("Client's Stone Return Voucher", reportPath, rds, rps, null);
            }
            rptView.ShowDialog();
            rptView.Close();
        }
        #endregion
     

        #region Search OrderNo and Against the Order Number Bind the Grid.
        private void btnSearchOrder_Click(object sender, EventArgs e)
        {
            string sCustOrderSearchNumber = string.Empty;

            DataTable dtGridItems = new DataTable();
            string commandText =string.Empty;

            commandText = " SELECT ORDERNUM,CONVERT(VARCHAR(15),ORDERDATE,103) AS ORDERDATE ,CONVERT(VARCHAR(15),DELIVERYDATE,103) AS DELIVERYDATE , " +
                                 " CUSTNAME ,CONVERT(VARCHAR(15),TOTALAMOUNT) AS TOTALAMOUNT  FROM CUSTORDER_HEADER ORDER BY ORDERNUM ";
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
                Dialog objCustOrderSearch = new Dialog();

                objCustOrderSearch.GenericSearch(dtGridItems, ref selRow, "Customer Order");
                if(selRow != null)
                    sCustOrderSearchNumber = Convert.ToString(selRow["ORDERNUM"]);
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
            if(!string.IsNullOrEmpty(sCustOrderSearchNumber))
            {
                DataSet dsOrderSearched = new DataSet();

                commandText = " SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID] " +
                                " ,[CONFIGID] [CONFIGURATION],[CODE] [COLOR],[SIZEID] [SIZE],[STYLE],[INVENTDIMID],[PCS],[QTY] [QUANTITY],[NETTWT],[DIAWT],[DIAAMT],[STNWT] " +
                                " ,[STNAMT],[TOTALAMT],[DATAAREAID],[STAFFID],[REPLICATIONCOUNTER],[UNITID],[REMARKSDTL],[ISRETURNED]  FROM [CUSTORDSAMPLE] WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "' " +
                 
                                " SELECT [ORDERNUM],[LINENUM],[STOREID],[TERMINALID],[ITEMID] " +
                                " ,[CODE] [COLOR],[SIZEID] [SIZE],[STYLE],[PCS],[DIAWT],[DIAAMT],[STNWT] " +
                                " ,[STNAMT],[DATAAREAID],[STAFFID],[REMARKSDTL],[ISRETURNED]" +
                                " FROM [CUSTORDSTONE] WHERE ORDERNUM='" + sCustOrderSearchNumber.Trim() + "'";


                if(application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;


                if(connection.State == ConnectionState.Closed)
                    connection.Open();
               

                SqlCommand command1 = new SqlCommand(commandText, connection);

                command1.CommandTimeout = 0;

                SqlDataAdapter adapter = new SqlDataAdapter(commandText, connection);
                dsOrderSearched = new DataSet();
                adapter.Fill(dsOrderSearched);

                if(dsOrderSearched != null && dsOrderSearched.Tables.Count > 0)
                {
                    dtSample = dsOrderSearched.Tables[0];
                    dtStone = dsOrderSearched.Tables[1];
                }
            }
            dtNotReturn = null;
            if (iIsStoneRet == 1)
                dtNotReturn = dtStone.Clone();
            else
                dtNotReturn = dtSample.Clone();

            dtNotReturn.Columns["ISRETURNED"].DataType = typeof(bool);// change the datatype of the column ISRETURNED(int) to bool on 24/09/2014
            if (iIsStoneRet == 0)
            {
                dtSample.Select("ISRETURNED=False").CopyToDataTable(dtNotReturn, LoadOption.PreserveChanges);
                sfrmTitle = "Client's Order Sample Return";
            }
            else
            {
                dtStone.Select("ISRETURNED=False").CopyToDataTable(dtNotReturn, LoadOption.PreserveChanges);
                sfrmTitle = "Client's Order Stone Return";
            }


            grItems.DataSource = dtNotReturn.DefaultView;
            //frmCustOrderSampleReturn objSampleRtn = new frmCustOrderSampleReturn(pos, application, dtSample, objOrderdetails, 0);
            if (retailTrans != null)
            {
                if (!string.IsNullOrEmpty(retailTrans.Customer.PostalCode))
                    sCPinCode = Convert.ToString(retailTrans.Customer.PostalCode);
            }
            //else
            //{
            //    sCPinCode = Convert.ToString(application.BusinessLogic.CustomerSystem.GetCustomerInfo(objCustomerOrder.txtCustomerAccount.Text).PostalCode);
            //}
            txtOrderNo.Text = sCustOrderSearchNumber;
            // grItems.DataSource = dtNotReturn.DefaultView;
        }
        #endregion
        #region List of Returned Item
        //added or modified on 16/09/2014 by Palas
        private void simpleButtonEx1_Click(object sender, EventArgs e)
        {
            DataTable dtRetItem = new DataTable();
            if (iIsStoneRet == 1)
            {
                dtRetItem = dtStone.Clone();
                if(dtRetItem != null && dtRetItem.Columns.Count > 0)
                {
                    dtRetItem.Columns["ISRETURNED"].DataType = typeof(bool);// change the datatype of the column ISRETURNED(int) to bool on 24/09/2014
                    dtStone.Select("ISRETURNED=true").CopyToDataTable(dtRetItem, LoadOption.PreserveChanges);
                    gridColumn3.Visible = true;
                    gridColumn3.Width = 80;
                    gridColumn4.Visible = true;
                    gridColumn4.Caption = "Code";
                    gridColumn14.Visible = false;
                    gridColumn15.Visible = true;
                    gridColumn5.Visible = false;
                    gridColumn6.Visible = false;
                    gridColumn8.Visible = false;
                    gridColumn9.Visible = false;
                    gridColumn15.Width = 200;
                }
            }
            else
            {
                dtRetItem = dtSample.Clone();
                if(dtRetItem != null && dtRetItem.Columns.Count > 0)
                {
                    dtRetItem.Columns["ISRETURNED"].DataType = typeof(bool);// change the datatype of the column ISRETURNED(int) to bool on 24/09/2014
                    dtSample.Select("ISRETURNED=True").CopyToDataTable(dtRetItem, LoadOption.PreserveChanges);
                }
            }
            grReturmItem.DataSource = null;
            grReturmItem.DataSource = dtRetItem.DefaultView;
            grReturmItem.Location = new System.Drawing.Point(0, 80);
            grItems.Visible = false;
            panel1.Location = new Point(8, 100);
            panel1.Size = new Size(883, 203);
            panel1.Refresh();
            panel1.Visible = true;

        }
        #endregion

        #region Hide panel
        private void grReturmItem_DoubleClick(object sender, EventArgs e)
        {
            //panel1.Visible = false;
        }
        #endregion

        #region Close Form
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        #endregion

        private void btnReturnedItemPrint_Click(object sender, EventArgs e)
        {
            DataTable dtRetItem = new DataTable();
            if (iIsStoneRet == 1)
            {
                dtRetItem = dtStone.Clone();
                if(dtRetItem != null && dtRetItem.Columns.Count > 0)
                {
                    dtRetItem.Columns["ISRETURNED"].DataType = typeof(bool);// change the datatype of the column ISRETURNED(int) to bool on 24/09/2014
                    dtStone.Select("ISRETURNED=true").CopyToDataTable(dtRetItem, LoadOption.PreserveChanges);
                    gridColumn3.Visible = true;
                    gridColumn3.Width = 80;
                    gridColumn4.Visible = true;
                    gridColumn4.Caption = "Code";
                    gridColumn14.Visible = false;
                    gridColumn15.Visible = true;
                    gridColumn5.Visible = false;
                    gridColumn6.Visible = false;
                    gridColumn8.Visible = false;
                    gridColumn9.Visible = false;
                    gridColumn15.Width = 200;
                }
            }
            else
            {
                dtRetItem = dtSample.Clone();
                if (dtRetItem != null && dtRetItem.Columns.Count > 0)
                dtRetItem.Columns["ISRETURNED"].DataType = typeof(bool);// change the datatype of the column ISRETURNED(int) to bool on 24/09/2014
                dtSample.Select("ISRETURNED=True").CopyToDataTable(dtRetItem, LoadOption.PreserveChanges);
            }
            if (dtRetItem != null && dtRetItem.Rows.Count > 0)
            {
                sOrderNum = Convert.ToString(dtRetItem.Rows[0]["ORDERNUM"]);
                //PrintVoucher(sOrderNum, iIsStoneRet, dtRetItem);
            }
            grItems.Visible = false;
            grReturmItem.Location = new System.Drawing.Point(0, 80);
            panel1.Location = new Point(8, 100);
            panel1.Size = new Size(883, 203);
            panel1.Refresh();
            panel1.Visible = true;
           
        }

       
    }
}
