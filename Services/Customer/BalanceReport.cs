/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using DE = Microsoft.Dynamics.Retail.Pos.DataEntity;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;


namespace Microsoft.Dynamics.Retail.Pos.Customer
{
    // Get all text through the Translation function in the ApplicationLocalizer
    // TextID's for Customer BalanceReport are reserved at 51020 - 51039
    // Used textid's 51020 - 51025

    class BalanceReport
    {
        /// <summary>
        /// Fetch the balance details from database.
        /// Prints the balance report as per specific format.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
        public void Print()
        {
            DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);

            //Get the balances
            IList<DE.CustomerBalanceInfo> customerBalances = customerDataManager.GetCustomersBalances();

            if ((customerBalances != null) && (customerBalances.Count > 0))
            {
                //Print header information
                int charsInLine = 44;
                string line = "--------------------------------------------" + "\n";
                string reportName = LSRetailPosis.ApplicationLocalizer.Language.Translate(51020); //"BALANCE REPORT"
                int leftPad = (charsInLine - reportName.Length) / 2;
                string report = "\n" + reportName.PadLeft(leftPad + reportName.Length) + "\n\n\n";
                report += LSRetailPosis.ApplicationLocalizer.Language.Translate(51021) + " " + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToShortTimeString() + "\n\n"; //"Report printed"
                //Customer         //Name               //Balance
                report += LSRetailPosis.ApplicationLocalizer.Language.Translate(51022).PadRight(10).Substring(0, 10) + " " + LSRetailPosis.ApplicationLocalizer.Language.Translate(51023).PadRight(20).Substring(0, 20) + " " + LSRetailPosis.ApplicationLocalizer.Language.Translate(51024).PadLeft(12).Substring(0, 12) + "\n";
                report += line;

                string customerId = string.Empty;
                string name = string.Empty;
                decimal balance = 0;
                decimal total = 0;

                //Loop throug the customer balances
                foreach (var customerBalance in customerBalances)
                {
                    customerId = customerBalance.AccountNumber;
                    name = customerBalance.Name;
                    balance = customerBalance.Balance;
                    total += balance;

                    report += customerId.PadRight(10) + " " + name.PadRight(20).Substring(0, 20) + " " + (balance.ToString("n2")).PadLeft(12) + "\n";
                }

                //Printer footer
                report += line;
                string totalText = LSRetailPosis.ApplicationLocalizer.Language.Translate(51025) + ": "; //TOTAL
                report += totalText.PadLeft(32) + (total.ToString("n2")).PadLeft(12) + "\n\n\n";
                
                //Send text to printer
                if (((object) Customer.InternalApplication.Services.Printing) is IPrintingV2)
                {   // Print to the default printer
                    Customer.InternalApplication.Services.Printing.PrintDefault(true, report);
                }
                else
                {   // Legacy support - direct print to printer #1
                    NetTracer.Warning("BalanceReport.Print - Printing service does not support default printer.  Using printer #1");
                    Customer.InternalApplication.Services.Peripherals.Printer.PrintReceipt(report);
                }

            }
        }
    }
}
