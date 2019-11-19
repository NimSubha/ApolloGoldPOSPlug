/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Linq;
using System.Text;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.EOD
{
    /// <summary>
    /// Class warps Batch printing functions.
    /// </summary>
    static class BatchPrinting
    {

        #region Fields

        private const int paperWidth = 55;
        private static readonly string singleLine = string.Empty.PadLeft(paperWidth, '-');
        private static readonly string lineFormat = ApplicationLocalizer.Language.Translate(7060);
        private static readonly string currencyFormat = ApplicationLocalizer.Language.Translate(7061);
        private static readonly string typeFormat = ApplicationLocalizer.Language.Translate(7062);

        #endregion

        #region Methods

        /// <summary>
        /// Print a batch to the printer as X or Z report.
        /// </summary>
        /// <param name="batch">Calculated batch object</param>
        /// <param name="reportType">Report type</param>
        public static void Print(this Batch batch, ReportType reportType)
        {
            // TextID's for the Z/X Report are reserved at 7000 - 7099

            StringBuilder reportLayout = new StringBuilder(2500);

            // Header
            reportLayout.PrepareHeader(batch, reportType);

            // Total Amounts
            reportLayout.AppendReportLine(7015);
            reportLayout.AppendReportLine(7016, RoundDecimal(batch.SalesTotal));
            reportLayout.AppendReportLine(7017, RoundDecimal(batch.ReturnsTotal));
            reportLayout.AppendReportLine(7018, RoundDecimal(batch.TaxTotal));
            reportLayout.AppendReportLine(7019, RoundDecimal(batch.DiscountTotal));
            reportLayout.AppendReportLine(7020, RoundDecimal(batch.RoundedAmountTotal));
            reportLayout.AppendReportLine(7021, RoundDecimal(batch.PaidToAccountTotal));
            reportLayout.AppendReportLine(7022, RoundDecimal(batch.IncomeAccountTotal));
            reportLayout.AppendReportLine(7023, RoundDecimal(batch.ExpenseAccountTotal));
            reportLayout.AppendLine();

            // Statistics
            reportLayout.AppendReportLine(7035);
            reportLayout.AppendReportLine(7036, batch.SalesCount);
            reportLayout.AppendReportLine(7038, batch.CustomersCount);
            reportLayout.AppendReportLine(7039, batch.VoidsCount);
            reportLayout.AppendReportLine(7040, batch.LogOnCount);
            reportLayout.AppendReportLine(7041, batch.NoSaleCount);

            if (reportType == ReportType.XReport)
            {
                reportLayout.AppendReportLine(7042, batch.SuspendedTransactionsCount);
            }

            reportLayout.AppendLine();

            // Tender totals
            reportLayout.AppendReportLine(7045);
            reportLayout.AppendReportLine(7047, RoundDecimal(batch.TenderedTotal));
            reportLayout.AppendReportLine(7048, RoundDecimal(batch.ChangeTotal));
            reportLayout.AppendReportLine(7069, RoundDecimal(batch.StartingAmountTotal));
            reportLayout.AppendReportLine(7049, RoundDecimal(batch.FloatEntryAmountTotal));
            reportLayout.AppendReportLine(7050, RoundDecimal(batch.RemoveTenderAmountTotal));
            reportLayout.AppendReportLine(7051, RoundDecimal(batch.BankDropTotal));
            reportLayout.AppendReportLine(7052, RoundDecimal(batch.SafeDropTotal));
            reportLayout.AppendReportLine(7053, RoundDecimal(batch.DeclareTenderAmountTotal));

            bool amountShort = batch.OverShortTotal < 0;

            reportLayout.AppendReportLine(amountShort ? 7055 : 7054, RoundDecimal(amountShort ? decimal.Negate(batch.OverShortTotal) : batch.OverShortTotal));
            reportLayout.AppendLine();

            // Income/Expense
            if (batch.AccountLines.Count > 0)
            {
                reportLayout.AppendReportLine(7030);
                foreach (BatchAccountLine accountLine in batch.AccountLines.OrderBy(a => a.AccountType))
                {
                    int typeResourceId = 0;

                    switch (accountLine.AccountType)
                    {
                        case IncomeExpenseAccountType.Income:
                            typeResourceId = 7031;
                            break;

                        case IncomeExpenseAccountType.Expense:
                            typeResourceId = 7032;
                            break;

                        default:
                            String message = string.Format("Unsupported account Type '{0}'.", accountLine.AccountType);
                            NetTracer.Error(message);
                            throw new NotSupportedException(message);
                    }

                    reportLayout.AppendReportLine(string.Format(typeFormat, accountLine.AccountNumber, ApplicationLocalizer.Language.Translate(typeResourceId)),
                            RoundDecimal(accountLine.Amount));
                }

                reportLayout.AppendLine();
            }

            // Tenders
            if (reportType == ReportType.ZReport && batch.TenderLines.Count > 0)
            {
                reportLayout.AppendReportLine(7046);
                foreach (BatchTenderLine tenderLine in batch.TenderLines.OrderBy(t => t.TenderName))
                {
                    string formatedTenderName = tenderLine.TenderName;

                    if (ApplicationSettings.Terminal.StoreCurrency != tenderLine.Currency)
                    {
                        formatedTenderName = string.Format(currencyFormat, tenderLine.TenderName, tenderLine.Currency);
                    }

                    reportLayout.AppendReportLine(string.Format(typeFormat, formatedTenderName, ApplicationLocalizer.Language.Translate(7049)),
                            RoundDecimal(tenderLine.AddToTenderAmountCur, tenderLine.Currency));
                    reportLayout.AppendReportLine(string.Format(typeFormat, formatedTenderName, ApplicationLocalizer.Language.Translate(7056)),
                            RoundDecimal(tenderLine.ShiftAmountCur, tenderLine.Currency));
                    reportLayout.AppendReportLine(string.Format(typeFormat, formatedTenderName, ApplicationLocalizer.Language.Translate(7050)),
                            RoundDecimal(tenderLine.RemoveFromTenderAmountCur, tenderLine.Currency));

                    if (tenderLine.CountingRequired)
                    {
                        reportLayout.AppendReportLine(string.Format(typeFormat, formatedTenderName, ApplicationLocalizer.Language.Translate(7053)),
                                RoundDecimal(tenderLine.DeclareTenderAmountCur, tenderLine.Currency));

                        amountShort = tenderLine.OverShortAmountCur < 0;

                        reportLayout.AppendReportLine(string.Format(typeFormat, formatedTenderName, ApplicationLocalizer.Language.Translate(amountShort ? 7055 : 7054)),
                            RoundDecimal(amountShort ? decimal.Negate(tenderLine.OverShortAmountCur) : tenderLine.OverShortAmountCur, tenderLine.Currency));
                    }

                    reportLayout.AppendReportLine(string.Format(typeFormat,
                        formatedTenderName, ApplicationLocalizer.Language.Translate(7057)), tenderLine.Count);

                    reportLayout.AppendLine();
                }
            }

            if (((object) EOD.InternalApplication.Services.Printing) is IPrintingV2)
            {   // Print to the default printer
                EOD.InternalApplication.Services.Printing.PrintDefault(true, reportLayout.ToString());
            }
            else
            {   // Legacy support - direct print to printer #1
                NetTracer.Warning("BatchPrinting.Print - Printing service does not support default printer.  Using printer #1");
                EOD.InternalApplication.Services.Peripherals.Printer.PrintReceipt(reportLayout.ToString());
            }
        }

        /// <summary>
        /// Print a shift staging report.
        /// </summary>
        /// <param name="batchStaging"></param>
        public static void Print(this IPosBatchStaging batchStaging)
        {
            StringBuilder reportLayout = new StringBuilder(1000);
            int headerStringId = 0;
            int statusStringId = 0;

            switch (batchStaging.Status)
            {
                case PosBatchStatus.Suspended:
                    headerStringId = 7063;
                    statusStringId = 7067;
                    break;

                case PosBatchStatus.BlindClosed:
                    headerStringId = 7064;
                    statusStringId = 7068;
                    break;

                default:
                    NetTracer.Error("Unsupported batchStaging status {0}", batchStaging.Status);
                    throw new NotSupportedException();
            }

            // Header
            reportLayout.AppendLine(ApplicationLocalizer.Language.Translate(headerStringId));
            reportLayout.AppendLine();

            // Current information
            reportLayout.AppendReportLine(7006, DateTime.Now.ToShortDateString());
            reportLayout.AppendReportLine(7007, DateTime.Now.ToShortTimeString());
            reportLayout.AppendReportLine(7003, ApplicationSettings.Terminal.TerminalId);
            reportLayout.AppendLine();

            // Content
            reportLayout.AppendReportLine(7065, batchStaging.TerminalId);
            reportLayout.AppendReportLine(7005, batchStaging.BatchId);
            reportLayout.AppendReportLine(7066, ApplicationLocalizer.Language.Translate(statusStringId));
            reportLayout.AppendReportLine(7008, batchStaging.StartDateTime.ToShortDateString());
            reportLayout.AppendReportLine(7009, batchStaging.StartDateTime.ToShortTimeString());
            reportLayout.AppendReportLine(7004, batchStaging.StaffId);

            EOD.InternalApplication.Services.Peripherals.Printer.PrintReceipt(reportLayout.ToString());
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Prepare report header.
        /// </summary>
        /// <param name="reportLayout"></param>
        /// <param name="reportType"></param>
        private static void PrepareHeader(this StringBuilder reportLayout, Batch batch, ReportType reportType)
        {
            reportLayout.AppendLine(singleLine);
            switch (reportType)
            {
                case ReportType.XReport:
                    reportLayout.AppendReportLine(7000);
                    break;

                case ReportType.ZReport:
                    reportLayout.AppendReportLine(7001);
                    break;

                default:
                    String message = string.Format("Unsupported Report Type '{0}'.", reportType);
                    NetTracer.Error(message);
                    throw new NotSupportedException(message);
            }

            reportLayout.AppendReportHeaderLine(7002, ApplicationSettings.Terminal.StoreId, true);
            reportLayout.AppendReportHeaderLine(7006, DateTime.Now.ToShortDateString(), false);
            reportLayout.AppendReportHeaderLine(7003, ApplicationSettings.Terminal.TerminalId, true);
            reportLayout.AppendReportHeaderLine(7007, DateTime.Now.ToShortTimeString(), false);
            reportLayout.AppendReportHeaderLine(7004, ApplicationSettings.Terminal.TerminalOperator.OperatorId, true);
            reportLayout.AppendLine();
            reportLayout.AppendLine();
            reportLayout.AppendReportHeaderLine(7005, ApplicationLocalizer.Language.Translate(206, batch.TerminalId, batch.BatchId), true);
            reportLayout.AppendLine();
            reportLayout.AppendReportHeaderLine(7008, batch.StartDateTime.ToShortDateString(), true);

            if (reportType == ReportType.ZReport)
            {
                reportLayout.AppendReportHeaderLine(7010, batch.CloseDateTime.ToShortDateString(), false);
            }
            else
            {
                reportLayout.AppendLine();
            }

            reportLayout.AppendReportHeaderLine(7009, batch.StartDateTime.ToShortTimeString(), true);
            if (reportType == ReportType.ZReport)
            {
                reportLayout.AppendReportHeaderLine(7011, batch.CloseDateTime.ToShortTimeString(), false);
            }
            else
            {
                reportLayout.AppendLine();
            }

            reportLayout.AppendLine();
        }

        /// <summary>
        /// Append Report header line.
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="titleResourceId">Resouce Id of Title part of the line</param>
        /// <param name="value">Value part of the line</param>
        /// <param name="firstPart">True for first part of header, false for second.</param>
        private static void AppendReportHeaderLine(this StringBuilder stringBuilder, int titleResourceId, string value, bool firstPart)
        {
            int partWidth = (paperWidth / 2);
            int titleWidth = (int)(partWidth * 0.5);
            int valueWidth = (int)(partWidth * 0.4);
            string title = ApplicationLocalizer.Language.Translate(titleResourceId);
            string line = string.Format(lineFormat, title.PadRight(titleWidth), value.PadLeft(valueWidth));

            if (firstPart)
            {
                stringBuilder.Append(line.PadRight(partWidth));
            }
            else
            {
                stringBuilder.AppendLine(line.PadLeft(partWidth));
            }
        }

        /// <summary>
        /// Append report line.
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="titleStringId">Resource id of title string</param>
        /// <param name="value">Value of tender item</param>
        private static void AppendReportLine(this StringBuilder stringBuilder, int titleStringId, object value)
        {
            string title = ApplicationLocalizer.Language.Translate(titleStringId);

            stringBuilder.AppendReportLine(title, value);
        }

        /// <summary>
        /// Append report line.
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="title">Title string</param>
        /// <param name="value">Value of tender item</param>
        private static void AppendReportLine(this StringBuilder stringBuilder, string title, object value)
        {
            stringBuilder.AppendLine(string.Format(lineFormat, title, value.ToString().PadLeft(paperWidth - title.Length - 2)));
        }

        /// <summary>
        /// Append a report title.
        /// </summary>
        /// <param name="stringBuilder"></param>
        /// <param name="titleResourceId">Resource Id</param>
        private static void AppendReportLine(this StringBuilder stringBuilder, int titleResourceId)
        {
            stringBuilder.AppendLine(ApplicationLocalizer.Language.Translate(titleResourceId));
            stringBuilder.AppendLine(singleLine);
        }

        /// <summary>
        /// Round amount to decimal using store currency code.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private static string RoundDecimal(decimal value)
        {
            return RoundDecimal(value, ApplicationSettings.Terminal.StoreCurrency);
        }

        /// <summary>
        /// Rount amount to decimal using given curreny code.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        private static string RoundDecimal(decimal value, string currency)
        {
            return EOD.InternalApplication.Services.Rounding.RoundForDisplay(value, currency, true, false);
        }

        #endregion

    }
}
