/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Interop.OposConstants;
using Interop.OposPOSPrinter;
using LSRetailPosis;
using LSRetailPosis.POSControls;
using LSRetailPosis.POSControls.Touch;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.HardwareProfiles;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.Services
{
	/// <summary>
	/// Class implements IPrinter interface.
	/// </summary>
    [Export(typeof(IPrinter))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public sealed class Printer : IPrinter
	{

		#region Fields

		private OPOSPOSPrinterClass oposPrinter;

		private readonly int characterSet = Convert.ToInt32(LSRetailPosis.Settings.HardwareProfiles.Printer.Characaterset);

		private const string barCodeRegEx = "<B: (.+?)>";
		private const int totalNumberOfLines = 40;
        private const string NORMAL_TEXT_MARKER = "|1C";
        private const string BOLD_TEXT_MARKER = "|2C";
        private const string ESC_CHARACTER = "\x1B";

        private Barcode barCode = new BarcodeCode39();
        private string[] printText;
        private int printTextLine;
		private frmMessage dialog;
		private int linesLeftOnCurrentPage;
		private string[] headerLines;

        private DeviceTypes deviceType;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="Printer"/> class.
        /// </summary>
        public Printer()
        {
        }

        /// Setup printer object with device settings
        /// </summary>
        /// <param name="deviceTypeName">Type of the device.</param>
        /// <param name="deviceName">Name of the device.</param>
        /// <param name="deviceDescription">The device description.</param>
        public void SetUpPrinter(string deviceTypeName, string deviceName, string deviceDescription)
        {
            DeviceTypes device;
            if (Enum.TryParse<DeviceTypes>(deviceTypeName, out device))
            {
                this.deviceType = device;
                this.DeviceName = deviceName;
                this.DeviceDescription = deviceDescription;
            }
            else
            {
                throw new ArgumentException("deviceType");
            }
        }

        #endregion


		#region Public Methods

		/// <summary>
		/// Load the device.
		/// </summary>
		/// <exception cref="IOException"></exception>
		public void Load()
		{
			if (this.deviceType == DeviceTypes.None)
				return;

			if (this.deviceType == DeviceTypes.OPOS)
			{
                NetTracer.Information("Peripheral [CashDrawer] - OPOS device loading: {0}", this.DeviceName ?? "<Undefined>");

                oposPrinter = new OPOSPOSPrinterClass();

				// Open
				oposPrinter.Open(this.DeviceName);
				Peripherals.CheckResultCode(this, oposPrinter.ResultCode);

				// Claim
				oposPrinter.ClaimDevice(Peripherals.ClaimTimeOut);
				Peripherals.CheckResultCode(this, oposPrinter.ResultCode);

				// Enable/Configure
				oposPrinter.DeviceEnabled = true;
				oposPrinter.AsyncMode = false;
				oposPrinter.CharacterSet = characterSet;
				oposPrinter.RecLineChars = 56;
				oposPrinter.SlpLineChars = 60;

				// Loading a bitmap for the printer
				string logoFile = Path.Combine(ApplicationSettings.GetAppPath(), "RetailPOSLogo.bmp");

				if (File.Exists(logoFile))
				{
                    NetTracer.Information("Peripheral [Printer] - OPOS printer bitmap load");
                    oposPrinter.SetBitmap(1, (int)OPOSPOSPrinterConstants.PTR_S_RECEIPT, logoFile, 500, (int)OPOSPOSPrinterConstants.PTR_BM_CENTER);
				}
            }

            IsActive = true;
		}

		/// <summary>
		/// Unload the device.
		/// </summary>
		public void Unload()
		{
			if (IsActive && oposPrinter != null)
			{
                NetTracer.Information("Peripheral [Printer] - Device Released");

				oposPrinter.ReleaseDevice();
				oposPrinter.Close();
				IsActive = true;
			}
		}

		/// <summary>
		/// Print text on the OPOS printer.
		/// </summary>
		/// <param name="text"></param>
		public void PrintReceipt(string text)
		{
			if (Peripherals.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
			{
        	    if (Peripherals.InternalApplication.Services.Peripherals.FiscalPrinter.SupportPrintingReceiptInNonFiscalMode(false))
		        {
                    Peripherals.InternalApplication.Services.Peripherals.FiscalPrinter.PrintReceipt(text);
		        }
	
        	    if (Peripherals.InternalApplication.Services.Peripherals.FiscalPrinter.ProhibitPrintingReceiptOnNonFiscalPrinters(false))
		        {
                    return;
		        }
			}

            // Always print to text file if test hook is enabled
			PrinterTestHook(text, "Receipt");

			if (!IsActive)
				return;

            try
            {
                NetTracer.Information("Peripheral [Printer] - Print Receipt");

                switch (this.deviceType)
                {
                    case DeviceTypes.OPOS:
                        OPOSPrinting(text);
                        break;

                    case DeviceTypes.Windows:
                        WindowsPrinting(text, this.DeviceName);
                        break;
                }
            }
            catch (Exception ex)
            {
                NetTracer.Warning("Peripheral [Printer] - Print Receipt Error: {0}", ex.ToString());

                ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                Peripherals.InternalApplication.Services.Dialog.ShowMessage(6212);
            }
		}

		/// <summary>
		/// Print text on the OPOS printer as slip.
		/// </summary>
		/// <param name="text"></param>
		public void PrintSlip(string text)
		{
			PrintSlip(text, string.Empty, string.Empty);
		}

		/// <summary>
		/// Prints a slip containing the text in the textToPrint parameter
		/// </summary>
		/// <param name="header"></param>
		/// <param name="details"></param>
		/// <param name="footer"></param>
		public void PrintSlip(string header, string details, string footer)
		{
			if (Peripherals.InternalApplication.Services.Peripherals.FiscalPrinter.FiscalPrinterEnabled())
			{
				Peripherals.InternalApplication.Services.Peripherals.FiscalPrinter.PrintSlip(header, details, footer);
				return;
			}

		    if (!IsActive)
				return;

            NetTracer.Information("Peripheral [Printer] - Print Slip");

            if (this.deviceType == DeviceTypes.OPOS)
            {   // Slip printing is only supported on OPOS printer
                headerLines = GetStringArray(header);
                string[] itemLines = GetStringArray(details);
                string[] footerLines = GetStringArray(footer);

                linesLeftOnCurrentPage = 0;

                if (LoadNextSlipPaper(true))
                {
                    try
                    {
                        PrintArray(itemLines);

                        // if there is not space for the footer on the current page then must be prompted for a new page
                        if ((linesLeftOnCurrentPage < footerLines.Length) && (!LoadNextSlipPaper(false)))
                            return;

                        PrintArray(footerLines);
                        RemoveSlipPaper();
                    }
                    finally
                    {
                        CloseExistingMessageWindow();
                    }
                }
            }
            else
            {   // Slip printing is not supported for this device type
                PrintReceipt(header + details + footer);
            }

		}

		/// <summary>
		/// Print a receipt on the windows printer.
		/// </summary>
		/// <param name="textToPrint"></param>
		/// <param name="printerName"></param>
		public void WindowsPrinting(string textToPrint, string printerName)
		{
            if (!string.IsNullOrWhiteSpace(textToPrint))
            {
                using (PrintDocument printDoc = new PrintDocument())
                {
                    printDoc.PrinterSettings.PrinterName = printerName;
                    string subString = textToPrint.Replace(ESC_CHARACTER, string.Empty);
                    printText = subString.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                    printTextLine = 0;

                    printDoc.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(printDoc_PrintPage);
                    printDoc.Print();
                }
            }
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Print a receipt containing the text to the OPOS Printer.
		/// </summary>
		/// <param name="textToPrint"> The text to print on the receipt</param>
		private void OPOSPrinting(string textToPrint)
		{
			Match barCodeMarkerMatch = Regex.Match(textToPrint, barCodeRegEx, RegexOptions.Compiled | RegexOptions.IgnoreCase);
			bool printBarcode = false;
			string receiptId = string.Empty;

			if (barCodeMarkerMatch.Success)
			{
				printBarcode = true;

				// Get the receiptId
				receiptId = barCodeMarkerMatch.Groups[1].ToString();

				// Delete the barcode marker from the printed string
				textToPrint = textToPrint.Remove(barCodeMarkerMatch.Index, barCodeMarkerMatch.Length);
			}

			// replace ESC with Char(27) and add a CRLF to the end
			textToPrint = textToPrint.Replace("ESC", ((char)27).ToString());
			textToPrint = textToPrint.Replace("<L>", "\x1B|1B\x1B|bC");

			if (LSRetailPosis.Settings.HardwareProfiles.Printer.BinaryConversion == true)
			{
				oposPrinter.BinaryConversion = 2;  // OposBcDecimal
				textToPrint = Peripherals.ConvertToBCD(textToPrint + "\r\n\r\n\r\n", characterSet);
			}

			oposPrinter.PrintNormal((int)OPOSPOSPrinterConstants.PTR_S_RECEIPT, textToPrint);
			oposPrinter.BinaryConversion = 0;  // OposBcNone

			// Check if we should print the receipt id as a barcode on the receipt
			if (printBarcode == true)
			{
				oposPrinter.PrintBarCode((int)OPOSPOSPrinterConstants.PTR_S_RECEIPT, receiptId, (int)OPOSPOSPrinterConstants.PTR_BCS_Code128,
						80, 80, (int)OPOSPOSPrinterConstants.PTR_BC_CENTER, (int)OPOSPOSPrinterConstants.PTR_BC_TEXT_BELOW);
				oposPrinter.PrintNormal((int)OPOSPOSPrinterConstants.PTR_S_RECEIPT, "\r\n\r\n\r\n\r\n");
			}

			oposPrinter.CutPaper(100);
		}

		private void NewStatusWindow(int stringId)
		{
			CloseExistingMessageWindow();
			dialog = new frmMessage(stringId, LSPosMessageTypeButton.NoButtons, MessageBoxIcon.Information);
			POSFormsManager.ShowPOSFormModeless(dialog);
		}

		private void CloseExistingMessageWindow()
		{
			if (dialog != null)
			{
				dialog.Dispose();
			}
		}

		private static string[] GetStringArray(string text)
		{
			string[] sep = new string[] { Environment.NewLine };
			if (text.EndsWith(Environment.NewLine))
				text = text.Substring(0, text.Length - 2);
			return text.Split(sep, StringSplitOptions.None);
		}

		private void PrintArray(string[] array)
		{
			NewStatusWindow(99);

			string textToPrint = string.Empty;

			foreach (string text in array)
			{
				if ((linesLeftOnCurrentPage == 0) && (!LoadNextSlipPaper(false)))
					return;

				textToPrint = text;

				if (LSRetailPosis.Settings.HardwareProfiles.Printer.BinaryConversion == true)
				{
					oposPrinter.BinaryConversion = 2;  // OposBcDecimal
					textToPrint = Peripherals.ConvertToBCD(text + Environment.NewLine, this.characterSet);
				}

				oposPrinter.PrintNormal((int)OPOSPOSPrinterConstants.PTR_S_SLIP, textToPrint);
				oposPrinter.BinaryConversion = 0;  // OposBcNone
				linesLeftOnCurrentPage--;
			}
		}

		private bool LoadNextSlipPaper(bool firstSlip)
		{
			bool result = true;
			bool tryAgain;

            NetTracer.Information("Peripheral [Printer] - Load next slip paper");

			if (!firstSlip)
				RemoveSlipPaper();

			do
			{
                
                tryAgain = false;
				NewStatusWindow(98);
				oposPrinter.BeginInsertion(LSRetailPosis.Settings.HardwareProfiles.Printer.DocInsertRemovalTimeout * 1000);

				if (oposPrinter.ResultCode == (int)OPOS_Constants.OPOS_SUCCESS)
				{
					NewStatusWindow(99);
					oposPrinter.EndInsertion();
					linesLeftOnCurrentPage = totalNumberOfLines;
					PrintArray(headerLines);
				}
				else
				{
					CloseExistingMessageWindow();
					using (frmMessage errDialog = new frmMessage(13051, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
					{
						POSFormsManager.ShowPOSForm(errDialog);
						if (errDialog.DialogResult == DialogResult.Yes)
							tryAgain = true;
						else
							result = false;
					}
				}
			} while (tryAgain);

			return result;
		}

		private void RemoveSlipPaper()
		{
			bool tryAgain;

            NetTracer.Information("Peripheral [Printer] - Remove slip paper");

			do
			{
				tryAgain = false;
				NewStatusWindow(100);
				oposPrinter.BeginRemoval(LSRetailPosis.Settings.HardwareProfiles.Printer.DocInsertRemovalTimeout * 1000);

				if (oposPrinter.ResultCode == (int)OPOS_Constants.OPOS_SUCCESS)
				{
					oposPrinter.EndRemoval();
				}
				else if (oposPrinter.ResultCode == (int)OPOS_Constants.OPOS_E_TIMEOUT)
				{
					CloseExistingMessageWindow();
					using (frmMessage errDialog = new frmMessage(13052, MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
					{
						POSFormsManager.ShowPOSForm(errDialog);
						if (errDialog.DialogResult == DialogResult.OK)
							tryAgain = true;
					}
				}
			} while (tryAgain);
		}

		/// <summary>
		/// Prints given text to sequential file in %TEMP% if printer hook is enabled
		/// </summary>
		/// <param name="text">Text to print</param>
		static private void PrinterTestHook(string text, string filePrefix)
		{
			if (ApplicationSettings.PrintToDisk)
			{
				string directory = Environment.GetEnvironmentVariable("TEMP", EnvironmentVariableTarget.Machine);
				string fileName = NextFileNameForPrinterHook(directory, filePrefix);
				fileName = Path.Combine(directory, fileName);
				using (TextWriter printedFile = new StreamWriter(fileName))
				{
					text = text.Replace(NORMAL_TEXT_MARKER, string.Empty).Replace(BOLD_TEXT_MARKER, string.Empty).Replace(ESC_CHARACTER, string.Empty);
					printedFile.Write(text);
				}
			}
		}

		/// <summary>
		/// Creates sequential files of format prefix_######.txt in the given directory.
		/// </summary>
		/// <param name="directory">Directory where files are stored/searched</param>
		/// <param name="prefix">What the beginning of the file should be named</param>
		/// <returns>Next sequential file name</returns>
		static private string NextFileNameForPrinterHook(string directory, string prefix)
		{
			DirectoryInfo di = new DirectoryInfo(directory);
			FileInfo[] files = di.GetFiles(prefix + "_*.txt");
			int max = 0;

			foreach (FileInfo file in files)
			{
				string fileName = file.Name;
				string[] pieces = fileName.Split(new char[] { '_', '.' });
				int fileNumber = Int32.Parse(pieces[1]);
				if (fileNumber > max)
				{
					max = fileNumber;
				}
			}

			string nextNumber = (max + 1).ToString().PadLeft(6, '0');
			string nextName = string.Format("{0}_{1}.txt", prefix, nextNumber);

			return nextName;
		}

		#endregion

		#region Events

        /// <summary>
        /// Prints the selected page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            const int LineHeight = 10;

            try
            {
                const string textFontName = "Courier New";
                const int textFontSize = 7;

                e.HasMorePages = false;
                using (Font textFont = new Font(textFontName, textFontSize, FontStyle.Regular))
                using (Font textFontBold = new Font(textFontName, textFontSize, FontStyle.Bold))
                {
                    string temp = string.Empty;
                    float x = 0, y = 0;
                    float dpiXRatio = e.Graphics.DpiX / 96f; // 96dpi = 100%
                    float dpiYRatio = e.Graphics.DpiY / 96f; // 96dpi = 100%
                    float contentWidth = printText.Max(str => str.Replace(NORMAL_TEXT_MARKER, string.Empty).Replace(BOLD_TEXT_MARKER, string.Empty).Length) * dpiXRatio; // Line with max length = content width

                    while (this.printTextLine < printText.Length)
                    {
                        string subString;
                        int index, index2;

                        if (y + LineHeight >= e.MarginBounds.Height)
                        {   // No more room - advance to next page
                            e.HasMorePages = true;
                            return;
                        }

                        index = printText[this.printTextLine].IndexOf(BOLD_TEXT_MARKER);

                        if (index >= 0)
                        {
                            // Text line printing with bold Text in it.

                            x = 0;

                            subString = printText[this.printTextLine];

                            while (subString.Length > 0)
                            {
                                index2 = subString.IndexOf(BOLD_TEXT_MARKER);

                                if (index2 >= 0)
                                {
                                    temp = subString.Substring(0, index2).Replace(NORMAL_TEXT_MARKER, string.Empty).Replace(BOLD_TEXT_MARKER, string.Empty);
                                    e.Graphics.DrawString(temp, textFont, Brushes.Black, x + e.MarginBounds.Left, y + e.MarginBounds.Top);
                                    x = x + (temp.Length * 6);

                                    index2 = index2 + 3;
                                    subString = subString.Substring(index2, subString.Length - index2);
                                    index2 = subString.IndexOf(NORMAL_TEXT_MARKER);

                                    temp = subString.Substring(0, index2).Replace(NORMAL_TEXT_MARKER, string.Empty).Replace(BOLD_TEXT_MARKER, string.Empty);
                                    e.Graphics.DrawString(temp, textFontBold, Brushes.Black, x + e.MarginBounds.Left, y + e.MarginBounds.Top);
                                    x = x + (temp.Length * 6);

                                    subString = subString.Substring(index2, subString.Length - index2);
                                }
                                else
                                {
                                    subString = subString.Replace(NORMAL_TEXT_MARKER, string.Empty).Replace(BOLD_TEXT_MARKER, string.Empty);
                                    e.Graphics.DrawString(subString, textFont, Brushes.Black, x + e.MarginBounds.Left, y + e.MarginBounds.Top);
                                    subString = string.Empty;
                                }
                            }
                        }
                        else
                        {
                            // Text line printing with no bold Text in it.

                            subString = printText[this.printTextLine].Replace(NORMAL_TEXT_MARKER, string.Empty);

                            Match barCodeMarkerMatch = Regex.Match(subString, barCodeRegEx, RegexOptions.Compiled | RegexOptions.IgnoreCase);

                            if (barCodeMarkerMatch.Success)
                            {
                                // Get the receiptId
                                subString = barCodeMarkerMatch.Groups[1].ToString();

                                using (Image barcodeImage = barCode.Create(subString, e.Graphics.DpiX, e.Graphics.DpiY))
                                {
                                    if (barcodeImage != null)
                                    {
                                        float barcodeHeight = (barcodeImage.Height / dpiYRatio);

                                        if (y + barcodeHeight >= e.MarginBounds.Height)
                                        {   // No more room - advance to next page
                                            e.HasMorePages = true;
                                            return;
                                        }

                                        // Render barcode in the center of receipt.
                                        e.Graphics.DrawImage(barcodeImage, ((contentWidth - (barcodeImage.Width / dpiXRatio)) / 2) + e.MarginBounds.Left, y + e.MarginBounds.Top);
                                        y += barcodeHeight;
                                    }
                                }
                            }
                            else
                            {
                                e.Graphics.DrawString(subString, textFont, Brushes.Black, e.MarginBounds.Left, y + e.MarginBounds.Top);
                            }
                        }
                        y = y + LineHeight;

                        printTextLine += 1;

                    } // of while()
                } // of using()
            } // of try
            catch (Exception ex)
            {
                NetTracer.Warning("Peripheral [Printer] - Exception in print page");

                ApplicationExceptionHandler.HandleException(this.ToString(), ex);
            }
        }

		#endregion

        /// <summary>
        /// Gets a value indicating whether this instance is active.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is active; otherwise, <c>false</c>.
        /// </value>
        public bool IsActive
        {
            get; private set;
        }

        /// <summary>
        /// Device Name (may be null or empty)
        /// </summary>
        public string DeviceName 
        { 
            get; private set;  
        }

        /// <summary>
        /// Device Description (may be null or empty)
        /// </summary>
        public string DeviceDescription 
        { 
            get; private set; 
        }
    }
}
