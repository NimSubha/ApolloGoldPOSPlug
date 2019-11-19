/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Data;
using LSRetailPosis.DataAccess;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using LSRetailPosis.POSProcesses;
using System.Windows.Forms;

namespace Microsoft.Dynamics.Retail.Pos.Customer
{
    // Get all text through the Translation function in the ApplicationLocalizer
    // TextID's for Customer BalanceReport are reserved at 51040 - 51059
    // Used textid's 51040 - 51049

    class TransactionReport
    {
        private System.Data.DataTable customerTransactions;
        private CustomerData customerData;

        /// <summary>
        /// Validating the payment transaction from database.
        /// Print the transaction report in a specific format.
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="numberOfMonths"></param>
        public void Print(string customerId, int numberOfMonths)
        {
            const string returnSign = "\r\n";

            try
            {
                //Find the start date
                DateTime fromDate = DateTime.Now;
                if (numberOfMonths == -1)
                {
                    fromDate = fromDate.AddYears(-100);
                }
                else
                {
                    fromDate = fromDate.AddMonths(-1 * numberOfMonths + 1);
                }

                fromDate = fromDate.AddDays(-1 * fromDate.Day + 1);
                fromDate = fromDate.AddHours(-1 * fromDate.Hour);
                fromDate = fromDate.AddMinutes(-1 * fromDate.Minute);
                fromDate = fromDate.AddSeconds(-1 * fromDate.Second);

                //Get the transactions
                customerData = new CustomerData(LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection, LSRetailPosis.Settings.ApplicationSettings.Database.DATAAREAID);
                customerTransactions = customerData.GetCustomerTransactions(customerId, fromDate);

                if (customerTransactions.Rows.Count > 0)
                {
                    //Get customer information
                    DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                        ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
                    ICustomer customerInfo = customerDataManager.GetTransactionalCustomer(customerId);

                    string line = "--------------------------------------------" + returnSign;
                    #region Create the report
                    //Print header information

                    string report = "\n" + LSRetailPosis.ApplicationLocalizer.Language.Translate(51040) + " " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + returnSign + returnSign;  //Report printed 
                    report += LSRetailPosis.ApplicationLocalizer.Language.Translate(51041).PadRight(15, '.').Substring(0, 15) + ": " + customerId + returnSign;     //Customer Id
                    report += LSRetailPosis.ApplicationLocalizer.Language.Translate(51042).PadRight(15, '.').Substring(0, 15) + ": " + customerInfo.Name + returnSign + returnSign; //Name

                    //ReceiptId         //Date          //Type          //Amount        
                    report += LSRetailPosis.ApplicationLocalizer.Language.Translate(51043).PadRight(10).Substring(0, 10) + " " + LSRetailPosis.ApplicationLocalizer.Language.Translate(51044).PadRight(10).Substring(0, 10) + " " + LSRetailPosis.ApplicationLocalizer.Language.Translate(51045).PadRight(8) + " " + LSRetailPosis.ApplicationLocalizer.Language.Translate(51046).PadLeft(13) + returnSign;
                    report += line;

                    Decimal subTotal = 0;
                    Decimal total = 0;
                    Decimal amount = 0;
                    int prevMonthNumber = 0;
                    string prevMonthName = string.Empty;
                    DateTime transDate;
                    string receiptId = string.Empty;
                    string transactionType = string.Empty;
                    string subTotalText = string.Empty;

                    //Loop throug the customer transactions
                    foreach (DataRow row in customerTransactions.Select())
                    {
                        transDate = (DateTime)row["TRANSDATE"];
                        amount = (Decimal)row["AMOUNT"];
                        transactionType = (String)row["TRANSACTIONTYPE"];

                        if (row["RECEIPTID"] == DBNull.Value)
                        {
                            receiptId = string.Empty;
                        }
                        else
                        {
                            receiptId = (String)row["RECEIPTID"];
                        }

                        if (prevMonthNumber == 0)
                        {
                            prevMonthNumber = transDate.Month;
                        }

                        if (transDate.Month != prevMonthNumber)
                        {
                            //Print subtotals
                            report += line;
                            subTotalText = LSRetailPosis.ApplicationLocalizer.Language.Translate(51047) + " " + prevMonthName.ToUpper() + ": ";
                            report += subTotalText.PadLeft(32) + subTotal.ToString("n2").PadLeft(12) + returnSign;
                            report += line;
                            subTotal = 0;
                        }

                        subTotal += amount;
                        total += amount;

                        //Print tranactions
                        report += receiptId.PadRight(10) + " " + transDate.ToShortDateString().PadRight(10) + " " + transactionType.PadRight(8) + amount.ToString("n2").PadLeft(14) + returnSign;

                        prevMonthNumber = transDate.Month;
                        prevMonthName = transDate.ToString("MMMM");
                    }

                    report += line;

                    //Print subtotals for the last month if more than one.
                    if (numberOfMonths > 1)
                    {
                        subTotalText = LSRetailPosis.ApplicationLocalizer.Language.Translate(51047) + " " + prevMonthName.ToUpper() + ": ";
                        report += subTotalText.PadLeft(32) + subTotal.ToString("n2").PadLeft(12) + returnSign;
                        report += "============================================" + returnSign; ;
                    }

                    //Print totals
                    report += LSRetailPosis.ApplicationLocalizer.Language.Translate(51048).PadLeft(27) + ": " + total.ToString("n2").PadLeft(15) + returnSign + returnSign;
                    decimal balanceNow = customerDataManager.GetBalance(customerId);
                    report += LSRetailPosis.ApplicationLocalizer.Language.Translate(51049).PadRight(15, '.') + ": " + balanceNow.ToString("n2") + returnSign + returnSign + returnSign;
                    //Send the report to the printer
                    #endregion

                    //System.Windows.Forms.DialogResult result = System.Windows.Forms.DialogResult.No;

                    using (frmReportList reportPreview = new frmReportList(report))
                    {
						POSFormsManager.ShowPOSForm(reportPreview);
						if (reportPreview.DialogResult == DialogResult.OK)
                        {
                            if (Customer.InternalApplication.Services.Printing is Microsoft.Dynamics.Retail.Pos.Contracts.Services.IPrintingV2)
                            {   // Print to the default printer
                                Customer.InternalApplication.Services.Printing.PrintDefault(true, report);
                            }
                            else
                            {   // Legacy support - direct print to printer #1
                                NetTracer.Warning("TransactionReport.Print - Printing service does not support default printer.  Using printer #1");
                                Customer.InternalApplication.Services.Peripherals.Printer.PrintReceipt(report);
                            }


                            Customer.InternalApplication.Services.Peripherals.Printer.PrintReceipt(report);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "TransactionReport::Print failed for customerId {0} numberOfMonths {1}", customerId, numberOfMonths);
                throw;
            }
        }
    }
}
