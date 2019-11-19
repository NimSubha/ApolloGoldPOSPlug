/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Settings.HardwareProfiles;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.TenderItem;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using System.Data.SqlClient;
using LSRetailPosis.Transaction.Line.SaleItem;
using System.Data;
using Microsoft.Dynamics.Retail.Pos.BlankOperations.Report;

namespace Microsoft.Dynamics.Retail.Pos.Printing
{
    [Export(typeof(IPrinting))]
    public class Printing : IPrinting
    {
        /// <summary>
        /// IApplication instance.
        /// </summary>
        private IApplication application;

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
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
                InternalApplication = value;
            }
        }


        /// <summary>
        /// Gets or sets the static IApplication instance.
        /// </summary>
        internal static IApplication InternalApplication { get; private set; }

        //Start:Nim
        public static int iPrintFromShowJournal = 0;
        //End;Nim

        /// <summary>
        /// Returns true if print preview ids shown.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <returns></returns>
        public bool ShowPrintPreview(FormType formType, IPosTransaction posTransaction)
        {
            if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
            {
                return Printing.InternalApplication.Services.Peripherals.FiscalPrinter.ShowPrintPreview(formType, posTransaction);
            }

            FormModulation formMod = new FormModulation(Application.Settings.Database.Connection);
            RetailTransaction retailTransaction = (RetailTransaction)posTransaction;

            FormInfo formInfo = formMod.GetInfoForForm(formType, false, LSRetailPosis.Settings.HardwareProfiles.Printer.ReceiptProfileId);
            formMod.GetTransformedTransaction(formInfo, retailTransaction);
            string textForPreview = formInfo.Header;
            textForPreview += formInfo.Details;
            textForPreview += formInfo.Footer;
            textForPreview = textForPreview.Replace("|1C", string.Empty);
            textForPreview = textForPreview.Replace("|2C", string.Empty);

            ICollection<Point> signaturePoints = null;
            if (retailTransaction.TenderLines != null
                && retailTransaction.TenderLines.Count > 0
                && retailTransaction.TenderLines.First.Value != null)
            {
                signaturePoints = retailTransaction.TenderLines.First.Value.SignatureData;
            }

            using (frmReportList preview = new frmReportList(textForPreview, signaturePoints))
            {
                this.Application.ApplicationFramework.POSShowForm(preview);
                if (preview.DialogResult == DialogResult.OK)
                {
                    if (LSRetailPosis.Settings.HardwareProfiles.Printer.DeviceType == LSRetailPosis.Settings.HardwareProfiles.DeviceTypes.None)
                    {
                        this.Application.Services.Dialog.ShowMessage(ApplicationLocalizer.Language.Translate(10060), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                        return false;
                    }
                    iPrintFromShowJournal = 1;//Added nim
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Print the standard slip, returns false if printing should be aborted altogether.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <param name="copyReceipt"></param>
        /// <returns></returns>
        public bool PrintReceipt(FormType formType, IPosTransaction posTransaction, bool copyReceipt)
        {
            //Base
            #region Base Commented
            /* if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
             {
                 bool proceedPrinting = false;
                 if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.SupportPrintingReceiptInNonFiscalMode(copyReceipt))
                 {
                     proceedPrinting = Printing.InternalApplication.Services.Peripherals.FiscalPrinter.PrintReceipt(formType, posTransaction, copyReceipt);
                 }

                 if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.ProhibitPrintingReceiptOnNonFiscalPrinters(copyReceipt))
                 {
                     return proceedPrinting;
                 }
             }

             FormModulation formMod = new FormModulation(Application.Settings.Database.Connection);
             IList<PrinterAssociation> printerMapping = PrintingActions.GetActivePrinters(formMod, formType, copyReceipt);

             bool result = false;
             foreach (PrinterAssociation printerMap in printerMapping)
             {
                 bool printResult = PrintingActions.PrintFormTransaction(printerMap, formMod, formType, posTransaction, copyReceipt);

                 result = result || printResult;
             }*/
            #endregion

            //Start:Nim
            #region Nimbus
            bool result = false;
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (formType == FormType.Receipt)
            {
                //SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

                //Start :02/07/2014
                if (posTransaction is IRetailTransaction)
                {
                    RetailTransaction retailTransaction = (RetailTransaction)posTransaction;
                    SaleLineItem item = retailTransaction.SaleItems.Last.Value;

                    //if (item.Description == "Issue gift card")
                    //{
                    //    frmPrintGV objGv = new frmPrintGV(item.Comment, retailTransaction.BeginDateTime.ToShortDateString(), Convert.ToString(item.NetAmount));
                    //    objGv.ShowDialog();
                    //}
                    //else
                    //{
                    #region Receipt Print
                    if (item.Description == "Add to gift card")
                    {
                        //====
                        if (application != null)
                            connection = application.Settings.Database.Connection;
                        else
                            connection = ApplicationSettings.Database.LocalConnection;
                        string sTransactionId = retailTransaction.TransactionId;
                        string sTerminalId = retailTransaction.TerminalId;
                        string sCardNo = string.Empty;
                        decimal sAmt = 0;

                        DataTable dt = GetGiftCardAmountInfo(connection, sTransactionId, sTerminalId);

                        if (dt != null && dt.Rows.Count > 0)
                        {
                            for (int i = 0; i <= dt.Rows.Count - 1; i++)
                            {
                                if (string.IsNullOrEmpty(sCardNo))
                                {
                                    sAmt = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]);

                                    sCardNo = Convert.ToString(dt.Rows[i]["COMMENT"]);
                                    sCardNo = new String('x', Convert.ToInt16(sCardNo.Length) - 4) + sCardNo.Substring(Convert.ToInt16(sCardNo.Length) - 4);

                                }
                                else
                                {
                                    sCardNo = sCardNo + "  / " + new String('x', Convert.ToInt16(Convert.ToString(dt.Rows[i]["COMMENT"]).Length) - 4) + Convert.ToString(dt.Rows[i]["COMMENT"]).Substring(Convert.ToInt16(Convert.ToString(dt.Rows[i]["COMMENT"]).Length) - 4);
                                    sAmt = sAmt + Convert.ToDecimal(dt.Rows[i]["AMOUNT"]);
                                }
                            }
                        }
                        frmR_ProductAdvanceReceipt objProdAdv = new frmR_ProductAdvanceReceipt(posTransaction, connection, sTransactionId, Convert.ToString(sAmt), sTerminalId, item.Description, sCardNo);
                        objProdAdv.ShowDialog();
                    }
                    else
                    {
                        if (retailTransaction.RefundReceiptId == "1")
                        {
                            string sTransactionId = retailTransaction.TransactionId;
                            string sTerminalId = retailTransaction.TerminalId;
                            string sCardNo = string.Empty;
                            decimal sAmt = 0;

                            DataTable dt = GetGiftCardAmountInfo(connection, sTransactionId, sTerminalId);

                            if (dt != null && dt.Rows.Count > 0)
                            {
                                for (int i = 0; i <= dt.Rows.Count - 1; i++)
                                {
                                    if (string.IsNullOrEmpty(sCardNo))
                                    {
                                        sAmt = Convert.ToDecimal(dt.Rows[i]["AMOUNT"]);

                                        sCardNo = Convert.ToString(dt.Rows[i]["COMMENT"]);
                                        sCardNo = new String('x', Convert.ToInt16(sCardNo.Length) - 4) + sCardNo.Substring(Convert.ToInt16(sCardNo.Length) - 4);

                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(Convert.ToString(dt.Rows[i]["COMMENT"])))
                                        {
                                            sCardNo = sCardNo + "  / " + new String('x', Convert.ToInt16(Convert.ToString(dt.Rows[i]["COMMENT"]).Length) - 4) + Convert.ToString(dt.Rows[i]["COMMENT"]).Substring(Convert.ToInt16(Convert.ToString(dt.Rows[i]["COMMENT"]).Length) - 4);
                                        }
                                        sAmt = sAmt + Convert.ToDecimal(dt.Rows[i]["AMOUNT"]);
                                    }
                                }
                            }

                            frmR_ProductAdvanceReceipt objProdAdv = new frmR_ProductAdvanceReceipt(posTransaction, connection, sTransactionId, Convert.ToString(sAmt), sTerminalId, item.Description, sCardNo, 1);
                            objProdAdv.ShowDialog();
                        }
                        else
                        {
                            if (retailTransaction.IncomeExpenseAmounts == 0) // voucher stop for income expense
                            {
                                if (iPrintFromShowJournal == 1)
                                {
                                    ChooseInvoiceLanguage(posTransaction, copyReceipt, connection, iPrintFromShowJournal);

                                    iPrintFromShowJournal = 0;
                                }
                                else
                                {
                                    ChooseInvoiceLanguage(posTransaction, copyReceipt, connection, iPrintFromShowJournal);
                                }

                                Nim_PrintAdvanceReceipt(posTransaction, 1);
                                //}
                            }
                            else
                            {
                                frmIncomeExpVoucher objIncExp = new frmIncomeExpVoucher(posTransaction, connection);
                                objIncExp.ShowDialog();
                            }
                        }
                    }
                    #endregion
                    //}
                    //retailTransaction
                }

                //End : 02/07/2014
            }
            else if (formType == FormType.CustomerAccountDeposit)
            {
                Nim_PrintAdvanceReceipt(posTransaction, 0);
            }
            else
            {
                //Base
                if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
                {
                    bool proceedPrinting = false;
                    if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.SupportPrintingReceiptInNonFiscalMode(copyReceipt))
                    {
                        proceedPrinting = Printing.InternalApplication.Services.Peripherals.FiscalPrinter.PrintReceipt(formType, posTransaction, copyReceipt);
                    }

                    if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.ProhibitPrintingReceiptOnNonFiscalPrinters(copyReceipt))
                    {
                        return proceedPrinting;
                    }
                }

                FormModulation formMod = new FormModulation(Application.Settings.Database.Connection);
                IList<PrinterAssociation> printerMapping = PrintingActions.GetActivePrinters(formMod, formType, copyReceipt);

                //bool result = false;
                foreach (PrinterAssociation printerMap in printerMapping)
                {
                    bool printResult = PrintingActions.PrintFormTransaction(printerMap, formMod, formType, posTransaction, copyReceipt);

                    result = result || printResult;
                }
                //Base
            }
            #endregion
            //End:Nim

            return result;
        }


        //Start:Nim
        #region Nimbus
        private void Nim_PrintAdvanceReceipt(IPosTransaction posTransaction, int iAdvAdjust)
        {
            RetailTransaction retailTransaction = posTransaction as RetailTransaction;
            string sGSSNo = string.Empty;
            if (retailTransaction != null)
            {
                SqlConnection connection = new SqlConnection();
                if (application != null)
                    connection = application.Settings.Database.Connection;
                else
                    connection = ApplicationSettings.Database.LocalConnection;
                string sTransactionId = retailTransaction.TransactionId;
                string sTerminalId = retailTransaction.TerminalId;

                DataTable dtAdv = GetAdvanceInfo(connection, sTransactionId, sTerminalId);
                bool bIsConToAd = iSConvertToAdv(connection, sTransactionId, sTerminalId);

                if (dtAdv != null && dtAdv.Rows.Count > 0)
                {
                    sGSSNo = Convert.ToString(dtAdv.Rows[0]["GSSNUMBER"]);

                    if (sGSSNo != string.Empty)
                    {
                        //frmR_GSSInstalmentReceipt objRGSS = new frmR_GSSInstalmentReceipt(posTransaction, connection, sTransactionId, Convert.ToString(dtAdv.Rows[0]["AMOUNT"]), sGSSNo, sTerminalId);
                        //objRGSS.ShowDialog();

                        //if (iPrintFromShowJournal == 1)
                        //{
                        ChooseInvoiceLanguageForGSS(posTransaction, connection, sTransactionId, Convert.ToString(dtAdv.Rows[0]["AMOUNT"]), sGSSNo, sTerminalId);
                        iPrintFromShowJournal = 0;
                        //}

                    }
                    else
                    {
                        //frmR_ProductAdvanceReceipt objProdAdv = new frmR_ProductAdvanceReceipt(posTransaction, connection, sTransactionId, Convert.ToString(dtAdv.Rows[0]["AMOUNT"]), sTerminalId, "", "", 0, bIsConToAd);
                        //objProdAdv.ShowDialog();
                        ChooseInvoiceLanguageForAdv(posTransaction, connection, sTransactionId, Convert.ToString(dtAdv.Rows[0]["AMOUNT"]), sTerminalId, bIsConToAd);
                    }

                }
            }
        }

        private static void ChooseInvoiceLanguage(IPosTransaction posTransaction, bool copyReceipt, SqlConnection connection, int isItFromShowJournal)
        {
            //Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch.frmLanguageForInvoice
            //    objLI = new BlankOperations.WinFormsTouch.frmLanguageForInvoice();

            //objLI.ShowDialog();

            int iNone = 0;
            int iLanguage = 0;
            //if (objLI.isEnglish == true)
            //    iLanguage = 1;
            //else if (objLI.isArabic == true)
            //    iLanguage = 2;
            //else if (objLI.isBoth == true)
            //    iLanguage = 3;
            //else
            //    iNone = 1;

            //if (iNone == 0)
            //{
            //    frmSaleInv reportfrm = new frmSaleInv(posTransaction, connection, copyReceipt, 0, iLanguage, isItFromShowJournal);
            //    reportfrm.ShowDialog();
            //}

            //if(iPrintFromShowJournal==0)

            frmGSTSalesInv reportfrm = new frmGSTSalesInv(posTransaction, connection, false, 0, "", iPrintFromShowJournal);
            reportfrm.ShowDialog();
        }

        private static void ChooseInvoiceLanguageForAdv(IPosTransaction posTransaction,
            SqlConnection connection, string sTransactionId, string sAmt, string sTerminalId, bool bIsConToAd)
        {
            //Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch.frmLanguageForInvoice
            //    objLI = new BlankOperations.WinFormsTouch.frmLanguageForInvoice();

            //objLI.ShowDialog();

            int iNone = 0;
            int iLanguage = 0;
            //if (objLI.isEnglish == true)
            //    iLanguage = 1;
            //else if (objLI.isArabic == true)
            //    iLanguage = 2;
            //else if (objLI.isBoth == true)
            //    iLanguage = 3;
            //else
            //    iNone = 1;

            if (iNone == 0)
            {
                frmR_ProductAdvanceReceipt objProdAdv = new frmR_ProductAdvanceReceipt(posTransaction, connection, sTransactionId, sAmt, sTerminalId, "", "", 0, bIsConToAd, iLanguage);
                objProdAdv.ShowDialog();
            }
        }

        private static void ChooseInvoiceLanguageForGSS(IPosTransaction posTransaction,
            SqlConnection connection, string sTransactionId, string sAmt, string sGSSNo, string sTerminalId)
        {
            //Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch.frmLanguageForInvoice
            //    objLI = new BlankOperations.WinFormsTouch.frmLanguageForInvoice();

            //objLI.ShowDialog();

            int iNone = 0;
            int iLanguage = 0;
            //if (objLI.isEnglish == true)
            //    iLanguage = 1;
            //else if (objLI.isArabic == true)
            //    iLanguage = 2;
            //else if (objLI.isBoth == true)
            //    iLanguage = 3;
            //else
            //    iNone = 1;

            if (iNone == 0)
            {
                frmR_GSSInstalmentReceipt objRGSS = new frmR_GSSInstalmentReceipt(posTransaction, connection, sTransactionId, sAmt, sGSSNo, sTerminalId, iLanguage);
                objRGSS.ShowDialog();
            }
        }

        public DataTable GetAdvanceInfo(SqlConnection conn, string sTransactionId, string sTerminalId)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            DataTable dt = new DataTable();
            string commandText = "SELECT ISNULL(GSSNUMBER,0) AS GSSNUMBER, CAST(ISNULL(AMOUNT,0) AS DECIMAL(28,2)) AS AMOUNT FROM  RETAILDEPOSITTABLE WHERE TRANSACTIONID ='" + sTransactionId + "' AND TERMINALID ='" + sTerminalId + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dt;

        }

        private bool iSConvertToAdv(SqlConnection conn, string sTransactionId, string sTerminalId)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("select ISCONVERTTOTADV,* from RETAILADJUSTMENTTABLE WHERE TRANSACTIONID ='" + sTransactionId + "' AND RETAILTERMINALID ='" + sTerminalId + "'");


            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), conn);
            return Convert.ToBoolean(cmd.ExecuteScalar());
        }

        private string getAdvanceAdjustedId(SqlConnection conn, string sTransactionId)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();
            StringBuilder sbQuery = new StringBuilder();
            //            select ADVANCEADJUSTMENTID from RETAIL_CUSTOMCALCULATIONS_TABLE 
            //where TRANSACTIONID='0000008679' and  isnull(ADVANCEADJUSTMENTID,'')!=''

            sbQuery.Append("select ADVANCEADJUSTMENTID from RETAIL_CUSTOMCALCULATIONS_TABLE WHERE TRANSACTIONID ='" + sTransactionId + "'");
            sbQuery.Append(" and  isnull(ADVANCEADJUSTMENTID,'')!=''");// AND RETAILTERMINALID ='" + sTerminalId + "'


            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), conn);
            return Convert.ToString(cmd.ExecuteScalar());
        }

        public DataTable GetGiftCardAmountInfo(SqlConnection conn, string sTransactionId, string sTerminalId)
        {
            if (conn.State == ConnectionState.Closed)
                conn.Open();

            DataTable dt = new DataTable();
            string commandText = "SELECT COMMENT, CAST(ISNULL(PRICE,0) AS DECIMAL(28,2)) AS AMOUNT FROM  RETAILTRANSACTIONSALESTRANS WHERE TRANSACTIONID ='" + sTransactionId + "' AND TERMINALID ='" + sTerminalId + "'";

            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            SqlDataAdapter da = new SqlDataAdapter(command);
            da.Fill(dt);
            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dt;

        }
        #endregion
        //End:Nim

        /// <summary>
        /// Print card slips.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <param name="tenderLineItem"></param>
        /// <param name="copyReceipt"></param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "only single cast of each type per condition")]
        public void PrintCardReceipt(FormType formType, IPosTransaction posTransaction, ITenderLineItem tenderLineItem, bool copyReceipt)
        {
            PrintingActions.Print(formType, copyReceipt, true,
                delegate(FormModulation formMod, FormInfo formInfo)
                {
                    return formMod.GetTransformedCardTender(formInfo, ((ICardTenderLineItem)tenderLineItem).EFTInfo, (RetailTransaction)posTransaction);
                });
        }

        /// <summary>
        /// Print card slips.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <param name="eftInfo"></param>
        /// <param name="copyReceipt"></param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "only single cast of each type per condition")]
        public void PrintCardReceipt(FormType formType, IPosTransaction posTransaction, IEFTInfo eftInfo, bool copyReceipt)
        {
            PrintingActions.Print(formType, copyReceipt, true,
                delegate(FormModulation formMod, FormInfo formInfo)
                {
                    return formMod.GetTransformedCardTender(formInfo, eftInfo, (RetailTransaction)posTransaction);
                });
        }

        /// <summary>
        /// Print customer account slips.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <param name="tenderLineItem"></param>
        /// <param name="copyReceipt"></param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "only single cast of each type per condition")]
        public void PrintCustomerReceipt(FormType formType, IPosTransaction posTransaction, ITenderLineItem tenderLineItem, bool copyReceipt)
        {
            PrintingActions.Print(formType, copyReceipt, true,
                delegate(FormModulation formMod, FormInfo formInfo)
                {
                    return formMod.GetTransformedTender(formInfo, (TenderLineItem)tenderLineItem, (RetailTransaction)posTransaction);
                });
        }

        /// <summary>
        /// Print declare starting amount receipt
        /// </summary>
        /// <param name="posTransaction">FloatEntryTransaction</param>
        public void PrintStartngAmountReceipt(IPosTransaction posTransaction)
        {
            if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled() &&
                Functions.CountryRegion == SupportedCountryRegion.RU)
            {
                Printing.InternalApplication.Services.Peripherals.FiscalPrinter.PrintStartingAmount(posTransaction);
                return;
            }

            bool copyReceipt = false;

            PrintingActions.Print(FormType.FloatEntry, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();
                StartingAmountTransaction startingAmountTransaction = (StartingAmountTransaction)posTransaction;

                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10077, false);
                reportLayout.AppendLine(PrintingActions.SingleLine);

                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.FormatTenderLine(ApplicationLocalizer.Language.Translate(10078),
                    Printing.InternalApplication.Services.Rounding.Round(startingAmountTransaction.Amount, true)));
                reportLayout.AppendLine(startingAmountTransaction.Description.ToString());
                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                PrintingActions.PrepareReceiptFooter(reportLayout);

                return reportLayout.ToString();
            });

            //Start:Nim
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            frmIncomeExpVoucher objIncExp = new frmIncomeExpVoucher(posTransaction, connection, 3);
            objIncExp.ShowDialog();
            //End:Nim
        }

        /// <summary>
        /// Print Float Entry Receipt
        /// </summary>
        /// <param name="posTransaction">FloatEntryTransaction</param>
        public void PrintFloatEntryReceipt(IPosTransaction posTransaction)
        {
            if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
            {
                Printing.InternalApplication.Services.Peripherals.FiscalPrinter.PrintFloatEntry(posTransaction);
                return;
            }

            bool copyReceipt = false;

            PrintingActions.Print(FormType.FloatEntry, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();
                FloatEntryTransaction asFloatEntryTransaction = (FloatEntryTransaction)posTransaction;

                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10061, copyReceipt);
                reportLayout.AppendLine(PrintingActions.SingleLine);

                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.FormatTenderLine(ApplicationLocalizer.Language.Translate(10062),
                    Printing.InternalApplication.Services.Rounding.Round(asFloatEntryTransaction.Amount, true)));
                reportLayout.AppendLine(asFloatEntryTransaction.Description.ToString());
                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                PrintingActions.PrepareReceiptFooter(reportLayout);

                return reportLayout.ToString();
            });
        }

        /// <summary>
        /// Print Tender Removal Receipt
        /// </summary>
        /// <param name="posTransaction">RemoveTenderTransaction</param>
        public void PrintRemoveTenderReceipt(IPosTransaction posTransaction)
        {
            if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
            {
                Printing.InternalApplication.Services.Peripherals.FiscalPrinter.PrintRemoveTender(posTransaction);
                return;
            }

            bool copyReceipt = false;

            PrintingActions.Print(FormType.RemoveTender, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();
                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10063, copyReceipt);
                reportLayout.AppendLine(PrintingActions.SingleLine);

                reportLayout.AppendLine();
                RemoveTenderTransaction asRemoveTenderTransaction = (RemoveTenderTransaction)posTransaction;
                reportLayout.AppendLine(PrintingActions.FormatTenderLine(ApplicationLocalizer.Language.Translate(10064),
                    Printing.InternalApplication.Services.Rounding.Round(asRemoveTenderTransaction.Amount, true)));
                reportLayout.AppendLine(asRemoveTenderTransaction.Description.ToString());
                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                PrintingActions.PrepareReceiptFooter(reportLayout);

                formMod = new FormModulation(Application.Settings.Database.Connection);
                formInfo = formMod.GetInfoForForm(FormType.FloatEntry, copyReceipt, LSRetailPosis.Settings.HardwareProfiles.Printer.ReceiptProfileId);

                return reportLayout.ToString();
            });
        }

        /// <summary>
        /// Print credit card memo.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="posTransaction"></param>
        /// <param name="tenderLineItem"></param>
        /// <param name="copyReceipt"></param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "only single cast of each type per condition")]
        public void PrintCreditMemo(FormType formType, IPosTransaction posTransaction, ITenderLineItem tenderLineItem, bool copyReceipt)
        {
            PrintingActions.Print(formType, copyReceipt, true,
                delegate(FormModulation formMod, FormInfo formInfo)
                {
                    return formMod.GetTransformedTender(formInfo, (TenderLineItem)tenderLineItem, (RetailTransaction)posTransaction);
                });
        }

        /// <summary>
        /// Print balance of credit card memo.
        /// </summary>
        /// <param name="formType"></param>
        /// <param name="balance"></param>
        /// <param name="copyReceipt"></param>
        public void PrintCreditMemoBalance(FormType formType, Decimal balance, bool copyReceipt)
        {
            PrintingActions.Print(formType, copyReceipt, true,
                delegate(FormModulation formMod, FormInfo formInfo)
                {
                    IRetailTransaction tr = Printing.InternalApplication.BusinessLogic.Utility.CreateRetailTransaction(
                        ApplicationSettings.Terminal.StoreId,
                        ApplicationSettings.Terminal.StoreCurrency,
                        ApplicationSettings.Terminal.TaxIncludedInPrice,
                        Printing.InternalApplication.Services.Rounding);
                    tr.AmountToAccount = balance;
                    formMod.GetTransformedTransaction(formInfo, (RetailTransaction)tr);

                    return formInfo.Header;
                });
        }

        /// <summary>
        /// Print invoice receipt.
        /// </summary>
        /// <param name="posTransaction">The pos transaction.</param>
        /// <param name="copyInvoice">if set to <c>true</c> [copy invoice].</param>
        /// <param name="printPreview">Not supported.</param>
        /// <returns></returns>
        public bool PrintInvoice(IPosTransaction posTransaction, bool copyInvoice, bool printPreview)
        {
            if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
            {
                Printing.InternalApplication.Services.Peripherals.FiscalPrinter.PrintInvoice(posTransaction, copyInvoice, printPreview);
                return true;
            }

            FormModulation formMod = new FormModulation(Application.Settings.Database.Connection);

            bool noPrinterDefined = true; // Initilize as error

            IList<PrinterAssociation> printerMapping = PrintingActions.GetActivePrinters(formMod, FormType.Invoice, copyInvoice);

            bool result = true;
            foreach (PrinterAssociation printerMap in printerMapping)
            {
                noPrinterDefined = noPrinterDefined && (printerMap.Type == DeviceTypes.None);

                if ((printerMap.Type == DeviceTypes.OPOS) || (printerMap.Type == DeviceTypes.Windows))
                {
                    bool printResult = PrintingActions.PrintFormTransaction(printerMap, formMod, FormType.Invoice, posTransaction, copyInvoice);

                    result = result && printResult;
                }
            }

            if (noPrinterDefined)
            {
                // 10060 - No printer type has been defined.
                this.Application.Services.Dialog.ShowMessage(ApplicationLocalizer.Language.Translate(10060), MessageBoxButtons.OK, MessageBoxIcon.Stop);
                result = false;
            }

            return result;

        }

        /// <summary>
        /// Print Tender Decaraton Receipt
        /// </summary>
        /// <param name="posTransaction">TenderDeclarationTransaction</param>
        public void PrintTenderDeclaration(IPosTransaction posTransaction)
        {
            bool copyReceipt = false;

            PrintingActions.Print(FormType.TenderDeclaration, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();
                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10065, copyReceipt);
                reportLayout.AppendLine(PrintingActions.SingleLine);

                PrintingActions.PrepareReceiptTenders(reportLayout, posTransaction);
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                PrintingActions.PrepareReceiptFooter(reportLayout);

                return reportLayout.ToString();
            });
        }

        /// <summary>
        /// Print Bank drop Receipt
        /// </summary>
        /// <param name="posTransaction">BankDropTransaction</param>
        public void PrintBankDrop(IPosTransaction posTransaction)
        {
            if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
            {
                Printing.InternalApplication.Services.Peripherals.FiscalPrinter.PrintBankDrop(posTransaction);
                return;
            }

            bool copyReceipt = false;

            PrintingActions.Print(FormType.BankDrop, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();
                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10066, copyReceipt);
                reportLayout.AppendLine(PrintingActions.FormatHeaderLine(10069, ((BankDropTransaction)posTransaction).BankBagNo.ToString(), true));
                reportLayout.AppendLine(PrintingActions.SingleLine);

                PrintingActions.PrepareReceiptTenders(reportLayout, posTransaction);
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                PrintingActions.PrepareReceiptFooter(reportLayout);

                return reportLayout.ToString();
            });

            //Start:Nim
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            frmIncomeExpVoucher objIncExp = new frmIncomeExpVoucher(posTransaction, connection, 2);
            objIncExp.ShowDialog();
            //End:Nim
        }

        /// <summary>
        /// Print safe drop Receipt
        /// </summary>
        /// <param name="posTransaction">SafeDropTransaction</param>
        public void PrintSafeDrop(IPosTransaction posTransaction)
        {
            if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
            {
                Printing.InternalApplication.Services.Peripherals.FiscalPrinter.PrintSafeDrop(posTransaction);
                return;
            }

            bool copyReceipt = false;

            PrintingActions.Print(FormType.SafeDrop, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();
                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10067, copyReceipt);
                reportLayout.AppendLine(PrintingActions.SingleLine);

                PrintingActions.PrepareReceiptTenders(reportLayout, posTransaction);
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                PrintingActions.PrepareReceiptFooter(reportLayout);

                return reportLayout.ToString();
            });

            //Start:Nim
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            frmIncomeExpVoucher objIncExp = new frmIncomeExpVoucher(posTransaction, connection, 1);
            objIncExp.ShowDialog();
            //ENd:Nim
        }

        /// <summary>
        /// Pring Gift Certificate
        /// </summary>
        /// <param name="formType">Currently unused</param>
        /// <param name="posTransaction"></param>
        /// <param name="giftCardItem"></param>
        /// <param name="copyReceipt">True if it is duplicate</param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Grandfather")]
        public void PrintGiftCertificate(FormType formType, IPosTransaction posTransaction, IGiftCardLineItem giftCardLineItem, bool copyReceipt)
        {
            PrintingActions.Print(formType, copyReceipt, false, delegate(FormModulation formMod, FormInfo formInfo)
            {
                StringBuilder reportLayout = new StringBuilder();

                PrintingActions.PrepareReceiptHeader(reportLayout, posTransaction, 10068, copyReceipt);
                reportLayout.AppendLine(PrintingActions.SingleLine);

                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.FormatTenderLine(ApplicationLocalizer.Language.Translate(10070), giftCardLineItem.SerialNumber));
                reportLayout.AppendLine(PrintingActions.FormatTenderLine(ApplicationLocalizer.Language.Translate(10071),
                    Printing.InternalApplication.Services.Rounding.RoundForDisplay(giftCardLineItem.Balance,
                    true, false)));
                reportLayout.AppendLine();
                reportLayout.AppendLine(PrintingActions.DoubleLine);

                PrintingActions.PrepareReceiptFooter(reportLayout);

                return reportLayout.ToString();
            });
        }

        /// <summary>
        /// Print pack slip.
        /// </summary>
        /// <param name="posTransaction">Transaction instance.</param>
        public void PrintPackSlip(IPosTransaction posTransaction)
        {
            bool copyReceipt = false;
            FormType formType = FormType.PackingSlip;

            PrintingActions.Print(formType, copyReceipt, true, delegate(FormModulation formMod, FormInfo formInfo)
            {
                formMod.GetTransformedTransaction(formInfo, (RetailTransaction)posTransaction);

                return formInfo.Header + formInfo.Details + formInfo.Footer;
            });
        }

        /// <summary>
        /// Print directly using the printText provided
        /// </summary>
        /// <param name="allowFallback">if set to <c>true</c> [allow fallback].</param>
        /// <param name="printText">The print text.</param>
        public void PrintDefault(bool allowFallback, string printText)
        {
            if (Printing.InternalApplication.Services.Peripherals.Printer.IsActive)
            {   // Print to the default printer (#1)
                Printing.InternalApplication.Services.Peripherals.Printer.PrintReceipt(printText);
            }
            else if (allowFallback && Printing.InternalApplication.Services.Peripherals.Printer2.IsActive)
            {   // Use the fallback printer
                Printing.InternalApplication.Services.Peripherals.Printer2.PrintReceipt(printText);
            }
            else
            {
                NetTracer.Information("Printing.PrintDefault() - printing is skipped as no printer is available.");
            }
        }

        /// <summary>
        /// Prints external receipt, i.e., any document received from an external device or a service.
        /// </summary>
        /// <param name="text">The text to be printed.</param>
        /// <remarks>
        /// Prints external receipt on the fiscal printer if it is enabled; otherwise prints on every available printer (printer 1 and/or printer 2).
        /// </remarks>
        public void PrintExternalReceipt(string text)
        {
            if (Printing.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
            {
                Printing.InternalApplication.Services.Peripherals.FiscalPrinter.PrintReceipt(text);
            }
            else
            {
                if (Printing.InternalApplication.Services.Peripherals.Printer.IsActive)
                {
                    Printing.InternalApplication.Services.Peripherals.Printer.PrintReceipt(text);
                }
                if (Printing.InternalApplication.Services.Peripherals.Printer2.IsActive)
                {
                    Printing.InternalApplication.Services.Peripherals.Printer2.PrintReceipt(text);
                }
            }
        }
    }
}
