using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data.SqlClient;
using System.Data;
//using GenCode128;
using LSRetailPosis.Settings;
using Microsoft.Reporting.WinForms;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;
using System.Drawing;
namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.Report
{
    class clApprovalSales
    {
        string sRemarks = string.Empty;
        string sCurrencySymbol = "";
        public static string Currency;
        public string sCPinCode { get; set; }
        public DataTable dtItemDetails_Main { get; set; }
        public RetailTransaction retailTrans { get; set; }
        private DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                   LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection,
                   LSRetailPosis.Settings.ApplicationSettings.Database.DATAAREAID);
        private IApplication application;
        BlankOperations oBlank = new BlankOperations();

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
        public clApprovalSales(IPosTransaction posTransaction)
        {
            retailTrans = posTransaction as RetailTransaction;
            if (retailTrans != null)
            {
                if (Convert.ToString(retailTrans.Customer.PostalCode) != string.Empty)
                    sCPinCode = Convert.ToString(retailTrans.Customer.PostalCode);
                Currency = retailTrans.Customer.Currency;
            }

        }
        public string Amtinwds(double amt)
        {
            object[] words = new object[28];
            string Awds = null;
            string x = null;
            string y = null;
            string a = null;
            string t = null;
            string cror = null;
            string lakh = null;
            string lak2 = null;
            string thou = null;
            string tho2 = null;
            string hund = null;
            string rupe = null;
            string rup2 = null;
            string pais = null;
            string pai2 = null;

            words[1] = "One ";
            words[2] = "Two ";
            words[3] = "Three ";
            words[4] = "Four ";
            words[5] = "Five ";
            words[6] = "Six ";
            words[7] = "Seven ";
            words[8] = "Eight ";
            words[9] = "Nine ";
            words[10] = "Ten ";
            words[11] = "Eleven ";
            words[12] = "Twelve ";
            words[13] = "Thirteen ";
            words[14] = "Fourteen ";
            words[15] = "Fifteen ";
            words[16] = "Sixteen ";
            words[17] = "Seventeen ";
            words[18] = "Eighteen ";
            words[19] = "Ninteen ";
            words[20] = "Twenty ";
            words[21] = "Thirty ";
            words[22] = "Forty ";
            words[23] = "Fifty ";
            words[24] = "Sixty ";
            words[25] = "Seventy ";
            words[26] = "Eighty ";
            words[27] = "Ninety ";

            if (amt >= 1)
            {
                Awds = "";
            }
            else
            {
                Awds = "";
            }
            x = (amt.ToString("0.00")).PadLeft(12, Convert.ToChar("0"));
            cror = x.Substring(1, 1);
            lakh = x.Substring(2, 2);
            lak2 = x.Substring(3, 1);
            thou = x.Substring(4, 2);
            tho2 = x.Substring(5, 1);
            hund = x.Substring(6, 1);
            rupe = x.Substring(7, 2);
            rup2 = x.Substring(8, 1);
            pais = x.Substring(10, 2);
            pai2 = x.Substring(11, 1);
            y = "";
            if (Convert.ToInt32(cror) > 0)
            {
                y = words[Convert.ToInt32(cror)].ToString() + "crores ";
            }
            t = Convert.ToString(lakh);
            if (Convert.ToInt32(t) > 0)
            {
                if (Convert.ToInt32(t) > 20)
                {
                    a = lakh.Substring(0, 1);
                    y = y + words[18 + Convert.ToInt32(a)];
                    if (Convert.ToInt32(lak2) != 0)
                        y = y + words[Convert.ToInt32(lak2)];
                    else
                        y = y + "";
                }
                else
                {
                    y = y + words[Convert.ToInt32(t)];
                }
                y = y + "lakhs ";
            }
            t = Convert.ToString(thou);
            if (Convert.ToInt32(t) > 0)
            {
                if (Convert.ToInt32(t) > 20)
                {
                    a = thou.Substring(0, 1);
                    y = y + words[18 + Convert.ToInt32(a)];
                    if (Convert.ToInt32(tho2) != 0)
                        y = y + words[Convert.ToInt32(tho2)];
                    else
                        y = y + "";
                }
                else
                {
                    y = y + words[Convert.ToInt32(t)];
                }
                y = y + "thousand ";
            }
            if (Convert.ToInt32(hund) > 0)
            {
                y = y + words[Convert.ToInt32(hund)] + "hundred ";
            }
            t = Convert.ToString(rupe);
            if (Convert.ToInt32(t) > 0)
            {
                if (Convert.ToInt32(t) > 20)
                {
                    a = rupe.Substring(0, 1);
                    y = y + words[18 + Convert.ToInt32(a)];
                    if (Convert.ToInt32(rup2) != 0)
                        y = y + words[Convert.ToInt32(rup2)];
                    else
                        y = y + "";
                }
                else
                {
                    y = y + words[Convert.ToInt32(t)];
                }
            }
            t = Convert.ToString(pais);
            if (Convert.ToInt32(t) > 0)
            {
                //y = y + "paise ";
                y = y + "and ";
                if (Convert.ToInt32(t) > 20)
                {
                    a = pais.Substring(0, 1);
                    y = y + words[18 + Convert.ToInt32(a)];
                    if (Convert.ToInt32(pai2) != 0)
                        y = y + words[Convert.ToInt32(pai2)] + "paise ";
                    else
                        //y = y + "";
                        y += "paise ";
                }
                else
                {
                    y = y + words[Convert.ToInt32(t)] + "paise ";
                }
            }
            string amtwrd = "";
            if (y.Length > 0)
            {
                amtwrd = Awds + y + "only ";
            }
            return amtwrd;
        }
        private string GetCompanyName()
        {
            string sCName = string.Empty;

            SqlConnection conn = new SqlConnection();

            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            string sQry = "SELECT ISNULL(A.NAME,'') FROM DIRPARTYTABLE A INNER JOIN COMPANYINFO B ON A.RECID = B.RECID WHERE B.DATAAREA = '" + ApplicationSettings.Database.DATAAREAID + "'";

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
       
        
        #region Print Voucher
        public void PrintApprovalVoucher(string sOrderNo, DataTable dtItemdetails, IPosTransaction posTransaction,string Title,DataTable dtIngr=null )
        {
            //PageSettings ps = new PageSettings { Landscape = false, PaperSize = new PaperSize { RawKind = (int)PaperKind.A4 }, Margins = new Margins { Top = 0, Right = 0, Bottom = 0, Left = 0 } };
            sCurrencySymbol = oBlank.GetCurrencySymbol();
            //datasources
            List<ReportDataSource> rds = new List<ReportDataSource>();
            rds.Add(new ReportDataSource("HEADERINFO", (DataTable)GetHeaderInfo(sOrderNo)));
            //dtItemDetails = dtItemdetails;
            //rds.Add(new ReportDataSource("DETAILSINFO", (DataTable)GetDetailInfo(sOrderNo, dtItemdetails)));

            rds.Add(new ReportDataSource("DETAILSINFO", (DataTable)GetDetailInfo(sOrderNo, dtItemdetails, dtIngr)));
            rds.Add(new ReportDataSource("DETAILSFOOTERINFO", (DataTable)GetDetailFooterInfo(dtItemDetails_Main)));
            
            //parameters
            List<ReportParameter> rps = new List<ReportParameter>();
            decimal dTotalAmt = 0;
            decimal dRoweTotal = 0;
           

            foreach (DataRow dr in dtItemDetails_Main.Rows)
            {
                decimal dVatAmt = 0;
                if(!string.IsNullOrEmpty(Convert.ToString(dr["VATAMOUNT"])))
                    dVatAmt = Convert.ToDecimal(dr["VATAMOUNT"]);

                if (!string.IsNullOrEmpty(Convert.ToString(dr["ROWTOTALAMOUNT"])))
                    dTotalAmt += Convert.ToDecimal(dr["ROWTOTALAMOUNT"]) + dVatAmt;
                //dTotalAmt += Convert.ToDecimal(dr["ROWTOTALAMOUNT"]);
            }
            

            Currency = GetCurrencyText(GetHeaderInfo(sOrderNo).Rows[0]["Currency"].ToString());
            string sAmtInWords = Currency + " " + Amtinwds(Math.Abs(Convert.ToDouble(dTotalAmt)));
            RetailTransaction retailTrans = posTransaction as RetailTransaction;

            if (retailTrans != null)
                sCPinCode = Convert.ToString(retailTrans.Customer.PostalCode);
            else
            {
                var SelectedCust = customerDataManager.GetTransactionalCustomer(GetHeaderInfo(sOrderNo).Rows[0]["CUSTACCOUNT"].ToString());
                sCPinCode = SelectedCust.PostalCode;
            }
            string sCompanyName = GetCompanyName();
            rps.Add(new ReportParameter("prmTransactionId", string.IsNullOrEmpty(posTransaction.TransactionId) ? " " : posTransaction.TransactionId, true));
            rps.Add(new ReportParameter("prmTotalInWrd", string.IsNullOrEmpty(sAmtInWords) ? " " : sAmtInWords, true));
            string sDate = string.Empty;
            
            
            rps.Add(new ReportParameter("prmPinCode", string.IsNullOrEmpty(sCPinCode) ? " " : sCPinCode, true));
            rps.Add(new ReportParameter("prmCompany", string.IsNullOrEmpty(sCompanyName) ? " " : sCompanyName, true));
            rps.Add(new ReportParameter("prmTitle", Title, true));
            rps.Add(new ReportParameter("Remarks", sRemarks, true));
            
            if (Title == "INWARD")
            {
                rps.Add(new ReportParameter("prmSubTiltle", "", true));
                sDate = System.DateTime.Now.ToShortDateString();
                rps.Add(new ReportParameter("prmApprDt", string.IsNullOrEmpty(sDate) ? " " : sDate, true));
               // rds.Add(new ReportDataSource("BARCODEIMGTABLE", (DataTable)GetBarcodeInfo(Title, "")));
                rps.Add(new ReportParameter("prmBarcode", string.IsNullOrEmpty("") ? " " : "", true));
                rps.Add(new ReportParameter("pIsVisibleTentative", "0",true));
            }
            else
            {
                sDate = Convert.ToString(GetHeaderInfo(sOrderNo).Rows[0]["ApprovalDate"]);
                rps.Add(new ReportParameter("prmApprDt", string.IsNullOrEmpty(sDate) ? " " : sDate, true));
                rps.Add(new ReportParameter("prmSubTiltle", "SUSPENSE ITEM ON APPROVAL", true));
                //rds.Add(new ReportDataSource("BARCODEIMGTABLE", (DataTable)GetBarcodeInfo(Title, sOrderNo)));
                rps.Add(new ReportParameter("prmBarcode", string.IsNullOrEmpty(sOrderNo) ? " " : sOrderNo, true));
                rps.Add(new ReportParameter("pIsVisibleTentative","1",true));
                
            }
            rps.Add(new ReportParameter("cs", string.IsNullOrEmpty(sCurrencySymbol) ? " " : sCurrencySymbol, true));

            string reportName = @"rptApprvDeliveryNote";
            //string reportName = @"Copy of rptCustOrdVoucher";
            string reportPath = @"Microsoft.Dynamics.Retail.Pos.BlankOperations.Report." + reportName + ".rdlc";
            //RdlcViewer rptView = new RdlcViewer("Approval Sale Voucher", reportPath, rds, rps, null,2);
            //rptView.ShowDialog();
            //rptView.Close();
        }


        private DataTable GetHeaderInfo(string sOrderNo)
        {
            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;//CONVERT(VARCHAR(15),APPROVALDATE,103) AS APPROVALDATE,CONVERT(VARCHAR(15),RETURNDATE,103) AS RETURNDATE
                SqlComm.CommandText = @"SELECT C.CURRENCY,AH.APPROVALNUM,CONVERT(VARCHAR(15),AH.APPROVALDATE,103) AS APPROVALDATE,AH.ALLRETURN,AH.CREATEDON,AH.CUSTACCOUNT,
                                        AH.CUSTADDRESS,AH.CUSTNAME,AH.CUSTPHONE,CONVERT(VARCHAR(15),AH.RETURNDATE,103) AS RETURNDATE,AH.SALEITEMSDETAILS,AH.TERMINALID,
                                        AH.TOTALAMAKING,AH.TOTALAMOUNT,AH.DATAAREAID,AH.DESCRIPTION,AH.SALEITEMSSUBDETAILS,AH.STAFFID,AH.STOREID
                                        FROM CUSTTABLE C INNER JOIN NIM_APPROVALSALE_HEADER AH 
                                        ON C.ACCOUNTNUM=AH.CUSTACCOUNT WHERE AH.APPROVALNUM='" + sOrderNo + "'";

                DataTable dtApprSaleHeader = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dtApprSaleHeader);
                dtApprSaleHeader.Columns.Add("CUSTPINCODE", typeof(string));

                SqlCon.Close();

                /// <summary>
                /// for cancellation of add customer , SelectCust=null or Not null @palas Jana 8-10-2014
                /// </summary>
                /// <param name="operationInfo"></param>
                /// <param name="posTransaction"></param>        
                /// 
                dtApprSaleHeader.Rows[0]["CUSTPINCODE"] = sCPinCode;
                #region Change the address format for custaddress column value by Palas Jana @ 08-12-2014
                
                var SelectedCust = customerDataManager.GetTransactionalCustomer(dtApprSaleHeader.Rows[0]["CUSTACCOUNT"].ToString());
                dtApprSaleHeader.Rows[0]["CUSTADDRESS"] = BlankOperations.AddressLines(SelectedCust);
                dtApprSaleHeader.Rows[0]["CUSTNAME"] = BlankOperations.GetCustomerNameWithSalutation(SelectedCust);
                sRemarks =Convert.ToString(dtApprSaleHeader.Rows[0]["DESCRIPTION"]);

                #endregion
                return dtApprSaleHeader;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
           
        }
        private string GetCurrencyText(string Currency)
        {
            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "select TXT from CURRENCY  where CURRENCYCODE ='" + Currency + "'";
                string sCurrencyCode= Convert.ToString(SqlComm.ExecuteScalar());
                string[] CurrencySplit = sCurrencyCode.Split(' ');
                if (CurrencySplit.Length > 1)
                    return CurrencySplit[1];
                else
                    return CurrencySplit[0];
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private DataTable GetDetailInfo(string sOrderNo,DataTable dt)
        //{
        //    #region commented
        //    //try
        //    //{
        //    //    SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
        //    //    SqlCon.Open();

        //    //    SqlCommand SqlComm = new SqlCommand();
        //    //    SqlComm.Connection = SqlCon;
        //    //    SqlComm.CommandType = CommandType.Text;
        //    //    SqlComm.CommandText = "select ITEMID,PCS,QTY,CRate as RATE,AMOUNT,MAKINGRATE,MAKINGAMOUNT," +
        //    //                          " LineTotalAmt as TOTALAMOUNT,REMARKSDTL as REMARKS FROM CUSTORDER_DETAILS" +
        //    //                          " WHERE ORDERNUM='" + sOrderNo + "'";

        //    //    DataTable CustBalDt = new DataTable();

        //    //    SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
        //    //    SqlDa.Fill(CustBalDt);

        //    //    return CustBalDt;

        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    throw new Exception(ex.Message);
        //    //}
        //    #endregion
        //    DataTable dtReturn = new DataTable();
        //    if (dt.Rows.Count > 0)
        //    {
        //        //if (dt.Rows[0]["APPROVALNUM"].ToString() == sOrderNo)
        //        //    dtReturn = dt;
        //        dtReturn = dt;
        //        string sDataAreaID=ApplicationSettings.Database.DATAAREAID;
        //        string sVATper = getVATDefault(sDataAreaID).Rows[0][0].ToString();
        //        decimal sVAT =Convert.ToDecimal( getVATDefault(sDataAreaID).Rows[0][1].ToString());
        //        #region Adding New Columns
        //        dtReturn.Columns.Add("VAT", typeof(string));
        //        dtReturn.Columns.Add("VATAMOUNT", typeof(decimal));
        //        foreach (DataRow dr in dtReturn.Rows)
        //        {
        //            decimal dLineTotal=Convert.ToDecimal(dr["ROWTOTALAMOUNT"].ToString());
        //            decimal dVatAmt = dLineTotal * sVAT / 100;
        //            dr["VATAMOUNT"] = dVatAmt;
        //            dr["VAT"] = sVATper;
        //        }
        //        dtItemDetails_Main = dtReturn;
        //        #endregion

        //        # region //ADDED ON 28/10/14 RH For image show in sales voucher
        //        string sSaleItem = string.Empty;
        //        string sItemParentId = string.Empty;
        //        string sArchivePath = string.Empty;

        //        DataTable dtDetail = new DataTable();
        //        dtDetail = dtItemDetails_Main;

        //        dtDetail.Columns.Add("ORDERLINEIMAGE", typeof(string));
        //        int i = 1;
        //        foreach(DataRow d in dtDetail.Rows)
        //        {
        //            sArchivePath = GetArchivePathFromImage();
        //            string path = sArchivePath + "" + sOrderNo + "_" + i + ".jpeg"; //

        //            if(File.Exists(path))
        //            {
        //                Image img = Image.FromFile(path);
        //                byte[] arr;
        //                using(MemoryStream ms1 = new MemoryStream())
        //                {
        //                    img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
        //                    arr = ms1.ToArray();
        //                }

        //                d["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
        //            }
        //            else
        //            {
        //                sSaleItem = Convert.ToString(d["ITEMID"]);
        //                sItemParentId = GetItemParentId(Convert.ToString(d["ITEMID"]));


        //                path = sArchivePath + "" + sItemParentId + "" + ".jpeg"; //

        //                if(File.Exists(path))
        //                {
        //                    Image img = Image.FromFile(path);
        //                    byte[] arr;
        //                    using(MemoryStream ms1 = new MemoryStream())
        //                    {
        //                        img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
        //                        arr = ms1.ToArray();
        //                    }

        //                    d["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
        //                }
        //                else
        //                    d["ORDERLINEIMAGE"] = "";

        //            }
        //            i++;
        //        }
        //        dtDetail.AcceptChanges();
        //        #endregion//end
        //    }
        //    return dtReturn=dt;
        //}

        private DataTable GetDetailInfo(string sOrderNo, DataTable dt, DataTable dtIngrd)
        {
            #region commented
            //try
            //{
            //    SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            //    SqlCon.Open();

            //    SqlCommand SqlComm = new SqlCommand();
            //    SqlComm.Connection = SqlCon;
            //    SqlComm.CommandType = CommandType.Text;
            //    SqlComm.CommandText = "select ITEMID,PCS,QTY,CRate as RATE,AMOUNT,MAKINGRATE,MAKINGAMOUNT," +
            //                          " LineTotalAmt as TOTALAMOUNT,REMARKSDTL as REMARKS FROM CUSTORDER_DETAILS" +
            //                          " WHERE ORDERNUM='" + sOrderNo + "'";

            //    DataTable CustBalDt = new DataTable();

            //    SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
            //    SqlDa.Fill(CustBalDt);

            //    return CustBalDt;

            //}
            //catch (Exception ex)
            //{
            //    throw new Exception(ex.Message);
            //}
            #endregion
            DataTable dtReturn = new DataTable();
            if(dt.Rows.Count > 0)
            {
                //if (dt.Rows[0]["APPROVALNUM"].ToString() == sOrderNo)
                //    dtReturn = dt;
                dtReturn = dt;
                string sVATper = string.Empty;
                decimal sVAT = 0;

                string sDataAreaID = ApplicationSettings.Database.DATAAREAID;
                if (getVATDefault(sDataAreaID) != null && getVATDefault(sDataAreaID).Rows.Count > 0)
                {
                    sVATper = getVATDefault(sDataAreaID).Rows[0][0].ToString();
                    sVAT = Convert.ToDecimal(getVATDefault(sDataAreaID).Rows[0][1].ToString());
                }
                else
                {
                    sVATper = "";
                    sVAT = 0;
                }

                 
                #region Adding New Columns
                dtReturn.Columns.Add("VAT", typeof(string));
                dtReturn.Columns.Add("VATAMOUNT", typeof(decimal));
                dtReturn.Columns.Add("Line", typeof(Int16));
                Int32 lineNum=1;
                foreach(DataRow dr in dtReturn.Rows)
                {
                    decimal dLineTotal = Convert.ToDecimal(dr["ROWTOTALAMOUNT"].ToString());
                    decimal dVatAmt = dLineTotal * sVAT / 100;
                    dr["VATAMOUNT"] = dVatAmt;
                    dr["VAT"] = sVATper;
                    dr["Line"] = lineNum;
                    lineNum++;
                }
                dtItemDetails_Main = dtReturn;
                #endregion

                # region //ADDED ON 28/10/14 RH For image show in sales voucher
                string sSaleItem = string.Empty;
                string sItemParentId = string.Empty;
                string sArchivePath = string.Empty;

                DataTable dtDetail = new DataTable();
                dtDetail = dtItemDetails_Main;

                dtDetail.Columns.Add("ORDERLINEIMAGE", typeof(string));
                int i = 1;
                foreach(DataRow d in dtDetail.Rows)
                {
                    sArchivePath = GetArchivePathFromImage();
                    string path = sArchivePath + "" + sOrderNo + "_" + i + ".jpeg"; //

                    if(File.Exists(path))
                    {
                        Image img = Image.FromFile(path);
                        byte[] arr;
                        using(MemoryStream ms1 = new MemoryStream())
                        {
                            img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                            arr = ms1.ToArray();
                        }

                        d["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
                    }
                    else
                    {
                        sSaleItem = Convert.ToString(d["ITEMID"]);
                        sItemParentId = GetItemParentId(Convert.ToString(d["ITEMID"]));


                        path = sArchivePath + "" + sItemParentId + "" + ".jpg"; //

                        if(File.Exists(path))
                        {
                            Image img = Image.FromFile(path);
                            byte[] arr;
                            using(MemoryStream ms1 = new MemoryStream())
                            {
                                img.Save(ms1, System.Drawing.Imaging.ImageFormat.Jpeg);
                                arr = ms1.ToArray();
                            }

                            d["ORDERLINEIMAGE"] = Convert.ToBase64String(arr);
                        }
                        else
                            d["ORDERLINEIMAGE"] = "";

                    }
                    i++;
                }
                dtDetail.AcceptChanges();
                #region Changes Start from 11-11-2014 @ Palas Jana
                dtDetail.Columns.Add("NetWt", typeof(string));
                dtDetail.Columns.Add("IngradientID", typeof(string));
                dtDetail.Columns.Add("IngrdDesc", typeof(string));
                DataTable Newdtbl = dtDetail.Clone();
                foreach(DataRow dr in dtDetail.Rows)
                {
                    //Add first row of dtDetail to Newdtbl Datatable
                    DataRow newrow = Newdtbl.NewRow();
                    //To calculate Net Wt Start
                    decimal dNetWt = 0;
                    foreach(DataRow drIngrd in dtIngrd.Rows)
                    {
                        if(dr[0].ToString() == drIngrd[0].ToString())
                        {
                            if(Convert.ToInt16(drIngrd["METALTYPE"]) == 1 || Convert.ToInt16(drIngrd["METALTYPE"]) == 2 || Convert.ToInt16(drIngrd["METALTYPE"]) == 3
                                || Convert.ToInt16(drIngrd["METALTYPE"]) == 13)
                                dNetWt += Convert.ToDecimal(drIngrd[8]);
                        }
                    }
                    foreach(DataColumn dc in dtDetail.Columns)
                    {
                        newrow[dc.ColumnName] = dr[dc.ColumnName];
                    }
                    newrow["NetWt"] = dNetWt.ToString();
                    Newdtbl.Rows.Add(newrow);
                    //end first row
                    //if there is non zero rows in dtIngrd against the Main Item ID i.e UniqueID , Then create new row to Newdtbl DataTable
                    foreach(DataRow drIngrd in dtIngrd.Rows)
                    {
                        if(dr[0].ToString() == drIngrd[0].ToString())
                        {
                            DataRow childRow = Newdtbl.NewRow();
                            childRow[0] = dr[0];//UniqueID
                            childRow["IngradientID"] = drIngrd[1];//IngradientID
                            childRow["IngrdDesc"] = drIngrd[1] + "-" + drIngrd[2];//IngrdDesc
                            childRow[3] = drIngrd[3];//congiuration
                            childRow[4] = drIngrd[4];//Color
                            childRow[5] = drIngrd[5];//Size
                            childRow[6] = drIngrd[6];//Style
                            childRow[7] = drIngrd[7];//Pcs
                            //childRow["QUANTITY"] = drIngrd[7];
                            childRow["NetWt"] = drIngrd[8];//NetWt
                            childRow["AMOUNT"] = drIngrd[11];//AMOUNT
                            childRow["ROWTOTALAMOUNT"] = 0;//ROWTOTALAMOUNT
                            childRow["RATE"] = drIngrd["RATE"];//RATE
                            childRow["Line"] = dr["Line"];
                            Newdtbl.Rows.Add(childRow);
                        }
                    }
                    //end adding child row.


                }
                #endregion end palas
                #endregion//end
                //Reset dtReturn and dtItemDetails_Main.
                dtReturn = new DataTable();
                dtItemDetails_Main = new DataTable();

                dtReturn = Newdtbl;
                dtItemDetails_Main = dtReturn;
            }
            return dtReturn;
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

        private DataTable  getVATDefault(string sDataAreaID)
        {

            try
            {

                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = @"select t2.TAXCODE,T1.SALESTAX_IN from TAXPARAMETERS t1 inner join TAXONITEM t2 on 
                                        t1.DATAAREAID=t2.DATAAREAID
                                        where t1.DATAAREAID='" + sDataAreaID+"'";
                DataTable dt = new DataTable();
                SqlDataAdapter sda = new SqlDataAdapter(SqlComm);
                sda.Fill(dt);
                SqlCon.Close();
                return dt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //private DataTable GetBarcodeInfo(string Title, string barcodeText = "")
        //{
        //    MemoryStream ms = new MemoryStream();
        //    Code128Rendering.MakeBarcodeImage(barcodeText, 20, true).Save(ms, System.Drawing.Imaging.ImageFormat.Png);

        //    Byte[] bitmap01 = null;
        //    //bitmap01 = ms.GetBuffer();
        //    bitmap01 = Code128Rendering.GetQrBarcode(barcodeText);
        //    //ms.Dispose();

        //    DataTable dtBarcode = new DataTable();
        //    dtBarcode.Columns.Add("ID", typeof(int));
        //    dtBarcode.Columns.Add("BARCODEIMG", typeof(byte[]));
        //    DataRow dr = dtBarcode.NewRow();
        //    dr["ID"] = 1;

        //    if(Title != "INWARD")
        //        dr["BARCODEIMG"] = bitmap01;
        //    else
        //        dr["BARCODEIMG"] = null;
        //    dtBarcode.Rows.Add(dr);

        //    return dtBarcode;
        //}

        private DataTable GetDetailFooterInfo(DataTable dtItemdetails)
        {
            DataTable dtReturn = new DataTable();

            dtReturn.Columns.Add("PCS", typeof(Int32));
            dtReturn.Columns.Add("GROSSWT", typeof(decimal));
            dtReturn.Columns.Add("NETWT", typeof(decimal));
            dtReturn.Columns.Add("AMOUNT", typeof(decimal));
            dtReturn.Columns.Add("MKGAMT", typeof(decimal));
            dtReturn.Columns.Add("TOTALAMT", typeof(decimal));
            int Ipcs = 0;
            decimal dGrswt = 0, dNetWt = 0, dAmt = 0, dMkgAmt = 0, dTAmt = 0;

            if(dtItemdetails != null)
            {
                foreach(DataRow dr in dtItemdetails.Rows)
                {
                    string sItemID = dr[1].ToString();
                    if(!string.IsNullOrEmpty(sItemID))
                    {
                        Ipcs += Convert.ToInt32(dr[7]);

                        dGrswt += Convert.ToDecimal(dr[8]);

                        dNetWt += Convert.ToDecimal(dr["NetWt"]);
                        dAmt += Convert.ToDecimal(dr["Amount"]);
                        dMkgAmt += Convert.ToDecimal(dr["MAKINGAMOUNT"]);
                        //dTAmt += Convert.ToDecimal(dr["ROWTOTALAMOUNT"]) + Convert.ToDecimal(dr["VATAMOUNT"]);
                        dTAmt += Convert.ToDecimal(dr["ROWTOTALAMOUNT"]) + Convert.ToDecimal(dr["VATAMOUNT"]);
                    }
                }
                DataRow newRow = dtReturn.NewRow();
                newRow[0] = Ipcs;
                newRow[1] = dGrswt;
                newRow[2] = dNetWt;
                newRow[3] = dAmt;
                newRow[4] = dMkgAmt;
                newRow[5] = dTAmt;
                dtReturn.Rows.Add(newRow);
            }
            return dtReturn;
        }
        #endregion
    }
}
