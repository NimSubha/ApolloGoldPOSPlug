/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Diagnostics;

namespace Microsoft.Dynamics.Retail.Pos.BarcodeService
{
    /// <summary>
    /// Class implementing the interface IBarcodes
    /// </summary>
    [Export(typeof(IBarcode))]
    public class Barcode : IBarcode
    {

        #region Member variables

        [Import]
        public IApplication Application { get; set; }

        private IUtility Utility {
            get { return this.Application.BusinessLogic.Utility; }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Returns barcode information as BarcodeInfo type.
        /// </summary>
        /// <param name="barcodeEntrytype"></param>
        /// <param name="barcode"></param>
        /// <returns></returns>
        public IBarcodeInfo ProcessBarcode(BarcodeEntryType barcodeEntrytype, string barcode)
        {
            IScanInfo scanInfo = this.Utility.CreateScanInfo();
            scanInfo.ScanDataLabel = barcode;
            scanInfo.EntryType = barcodeEntrytype;
            return ProcessBarcode(scanInfo);
        }

        /// <summary>
        /// Process barcode information and initialize a SaleLineItem object.
        /// </summary>
        /// <param name="iSaleLineItem">SaleLineitem object to initialize.</param>
        /// <param name="scanInfo">ScanInfo object. Can be null if barCodeInfo or barcode parameter is provided.</param>
        /// <param name="barcodeInfo">Barcode information object. Can be null if scanInfo or barcode is provided.</param>
        /// <param name="barcode">Barcode string as entered into POS. Can be null if scanInfo or barcodeInfo is provided.</param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "0#")]
        public IBarcodeInfo ProcessBarcode(ISaleLineItem iSaleLineItem, IScanInfo scanInfo, IBarcodeInfo barcodeInfo, string barcode)
        {
            if (iSaleLineItem == null)
            {
                throw new ArgumentNullException("iSaleLineItem");
            }

            // Caller can provide either barCode, scanIno or barCodeInfo object to process.
            if (barcodeInfo == null)
            {
                if (scanInfo == null)
                {
                    if (string.IsNullOrWhiteSpace(barcode))
                    {
                        throw new ArgumentNullException("barcode");
                    }

                    scanInfo = this.Utility.CreateScanInfo();
                    scanInfo.ScanDataLabel = barcode;
                }

                barcodeInfo = ProcessBarcode(scanInfo);
            }

            SaleLineItem saleLineItem = (SaleLineItem)iSaleLineItem;

            // The barcodeInfo has already been populated by the ProcessInput operation, which triggered the ItemSale operation
            if ((barcodeInfo.InternalType == BarcodeInternalType.Item) && (barcodeInfo.ItemId != null))
            {
                // The entry was a barcode which was found and now we have the item id...
                saleLineItem.ItemId = barcodeInfo.ItemId;
                saleLineItem.BarcodeId = barcodeInfo.BarcodeId;
                saleLineItem.SalesOrderUnitOfMeasure = barcodeInfo.UnitId;
                // Note that the following values will be set later (by Item):
                //      saleLineItem.SalesOrderUnitOfMeasureName
                //      saleLineItem.UnitQuantityFactor
                     
                
                saleLineItem.EntryType = barcodeInfo.EntryType;
                saleLineItem.Dimension.ColorId = barcodeInfo.InventColorId;
                saleLineItem.Dimension.SizeId = barcodeInfo.InventSizeId;
                saleLineItem.Dimension.StyleId = barcodeInfo.InventStyleId;
                saleLineItem.Dimension.ConfigId = barcodeInfo.ConfigId;
            }
            else
            {
                // It could be an ItemId
                saleLineItem.ItemId = barcodeInfo.BarcodeId;
                saleLineItem.EntryType = barcodeInfo.EntryType;
            }

            saleLineItem.Dimension.VariantId = barcodeInfo.VariantId;
            if (barcodeInfo.BarcodeQuantity > 0)
            {
                saleLineItem.Quantity = barcodeInfo.BarcodeQuantity;
            }
            else if (barcodeInfo.QtySold > 0)
            {
                saleLineItem.Quantity = barcodeInfo.QtySold;
            }
            if (saleLineItem.Quantity > 0)
            {
                saleLineItem.UnitQuantity = saleLineItem.UnitQtyConversion.Convert(saleLineItem.Quantity);
            }
            if (barcodeInfo.BarcodePrice > 0)
            {
                saleLineItem.BarcodeCalculatedQuantity = 1;
                saleLineItem.BarcodeEmbeddedPrice = barcodeInfo.BarcodePrice;
                saleLineItem.PriceInBarcode = true;
            }

            return barcodeInfo;
        }

        /// <summary>
        /// Returns barcode information as BarcodeInfo datatype.
        /// </summary>
        /// <param name="scanInfo"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public IBarcodeInfo ProcessBarcode(IScanInfo scanInfo)
        {
            IBarcodeInfo barcodeInfo = this.Utility.CreateBarcodeInfo();

            try
            {
                // The barcodeInfo is null and we need to populate it. The operation was triggered by i.e. a button or ItemSearch

                string barcodeid = scanInfo.ScanDataLabel;

                barcodeInfo.BarcodeId = barcodeid;
                barcodeInfo.ItemBarcode = barcodeid;
                barcodeInfo.EntryType = scanInfo.EntryType;
                barcodeInfo.TimeStarted = DateTime.Now;

                //If entered barcode contains "," or "."  then it should be processed as an empty barcode
                if (barcodeid.Contains(",") || barcodeid.Contains("."))
                {
                    barcodeid = string.Empty;
                }

                if (string.IsNullOrEmpty(barcodeid))
                {
                    barcodeInfo.Found = false;
                }
                else
                {
                    try
                    {
                        // If the input string that we are trying to identify contains any other caracters than numeric characters....
                        if (VerifyCheckDigit(barcodeInfo) == true) { barcodeInfo.CheckDigitValidated = true; } else { barcodeInfo.CheckDigitValidated = false; }
                    }
                    /// <exception cref="System.Exception">Thrown when the input string is not a barcode.</exception>
                    catch (Exception)
                    {
                    }

                    //Check if barcode is found as it was entered into the system.
                    Find(barcodeInfo, true);

                    //If not found, check if it is a maskable barcode.
                    if (barcodeInfo.Found == false)
                    {
                        BarcodeMask barcodeMask = new BarcodeMask(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);
                        barcodeMask.Find(barcodeid);
                        if (barcodeMask.Found == true)
                        {
                            barcodeInfo.MaskId = barcodeMask.MaskId;
                            barcodeInfo.Prefix = barcodeMask.Prefix;
                            barcodeInfo.InternalType = barcodeMask.InternalType;
                            barcodeInfo.Symbology = barcodeMask.Symbology;
                            barcodeInfo.BarcodePrice = 0;
                            barcodeInfo.BarcodeQuantity = 0;
                            barcodeInfo.DataEntry = string.Empty;
                            barcodeInfo.EmployeeId = string.Empty;
                            barcodeInfo.CouponId = string.Empty;
                            barcodeInfo.EANLicenseId = string.Empty;
                            barcodeInfo.CustomerId = string.Empty;
                            barcodeInfo.DiscountCode = string.Empty;

                            switch (barcodeMask.InternalType)
                            {
                                case BarcodeInternalType.Item:
                                case BarcodeInternalType.Customer:
                                case BarcodeInternalType.DataEntry:
                                case BarcodeInternalType.Employee:
                                case BarcodeInternalType.Salesperson:
                                case BarcodeInternalType.Pharmacy:
                                case BarcodeInternalType.DiscountCode:
                                case BarcodeInternalType.GiftCard:
                                case BarcodeInternalType.LoyaltyCard:
                                    ProcessMaskSegments(barcodeInfo);
                                    break;

                                case BarcodeInternalType.Coupon:
                                    break;

                                default:
                                    NetTracer.Warning("ProcessBarcode: Unsupported bardcode type {0}", barcodeMask.InternalType);
                                    break;
                            }
                        }
                    }
                    else
                    {
                        barcodeInfo.InternalType = BarcodeInternalType.Item;
                    }
                }

                return barcodeInfo;
            }
            finally
            {
                barcodeInfo.TimeFinished = DateTime.Now;
                barcodeInfo.TimeElapsed = barcodeInfo.TimeFinished - barcodeInfo.TimeStarted;
            }
        }

        /// <summary>
        /// Calculates the barcode checkdigit to verify if it is correct.
        /// The checkdigit is taken as the last digit in the barcode
        /// </summary>
        /// <returns>bool</returns>
        private static bool VerifyCheckDigit(IBarcodeInfo barcodeInfo)
        {
            bool match = false;

            //Calculate the checkdigit
            int checkDigit = CalcCheckDigit(barcodeInfo.BarcodeId.Substring(0, barcodeInfo.BarcodeId.Length - 1));
            int lastDigit;

            if (int.TryParse(barcodeInfo.BarcodeId.Substring(barcodeInfo.BarcodeId.Length - 1, 1), out lastDigit))
            {
                match = (lastDigit == checkDigit);
            }

            return match;
        }

        /// <summary>
        /// Processes the different barcode segments
        /// </summary>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfather")]
        private void ProcessMaskSegments(IBarcodeInfo barcodeInfo)
        {
            SqlConnection connection = Application.Settings.Database.Connection;

            string queryString = "SELECT * FROM RETAILBARCODEMASKSEGMENT WHERE MASKID = @MASKID AND DATAAREAID = @DATAAREAID ORDER BY SEGMENTNUM";
            try
            {
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    SqlParameter maskIdParm = command.Parameters.Add("@MASKID", SqlDbType.NVarChar, 10);
                    maskIdParm.Value = barcodeInfo.MaskId;
                    SqlParameter dataAreaIdParm = command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
                    dataAreaIdParm.Value = Application.Settings.Database.DataAreaID;

                    BarcodeMaskSegment Segment = new BarcodeMaskSegment();
                    int position = barcodeInfo.Prefix.Length;
                    bool barcodeInfoV2 = (((object)barcodeInfo) is IBarcodeInfoV2);

                    if (connection.State != ConnectionState.Open) { connection.Open(); }
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        using (DataTable barcodeSegmentTable = new DataTable())
                        {
                            barcodeSegmentTable.Load(reader);

                            int? barcodeMaskLength = null;

                            foreach (DataRow row in barcodeSegmentTable.Rows)
                            {
                                Segment.SegmentType = (BarcodeSegmentType)row["TYPE"];
                                Segment.Length = Convert.ToInt32(row["LENGTH"]);
                                Segment.Decimals = (int)row["DECIMALS"];

                                if (position + Segment.Length > barcodeInfo.BarcodeId.Length)
                                {
                                    return;
                                }

                                switch (Segment.SegmentType)
                                {
                                    case BarcodeSegmentType.Item:
                                        {
                                            // Calculate the length of the barcode
                                            if (barcodeMaskLength == null)
                                            {
                                                barcodeMaskLength = barcodeInfo.Prefix.Length;
                                                foreach (DataRow segment in barcodeSegmentTable.Rows)
                                                {
                                                    barcodeMaskLength += Convert.ToInt32(segment["LENGTH"]);
                                                }
                                            }

                                            string barcodeSubString = barcodeInfo.BarcodeId.Substring(0, position + Segment.Length);
                                            barcodeInfo.ItemBarcode = barcodeSubString.PadRight((int)barcodeMaskLength, '_');

                                            Find(barcodeInfo);
                                            if (barcodeInfo.Found == false)
                                            {
                                                // Try searching with a calculated check digit instead of wild cards.
                                                barcodeInfo.ItemBarcode = barcodeSubString + Convert.ToString(CalcCheckDigit(barcodeSubString));
                                                Find(barcodeInfo);
                                            }
                                        }
                                        break;
                                    case BarcodeSegmentType.AnyNumber:
                                        break;
                                    case BarcodeSegmentType.CheckDigit:
                                        break;
                                    case BarcodeSegmentType.SizeDigit:
                                        {
                                            barcodeInfo.InventSizeId = barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                            break;
                                        }
                                    case BarcodeSegmentType.ColorDigit:
                                        {
                                            barcodeInfo.InventColorId = barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                            break;
                                        }
                                    case BarcodeSegmentType.StyleDigit:
                                        {
                                            barcodeInfo.InventStyleId = barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                            break;
                                        }
                                    case BarcodeSegmentType.EANLicenseCode:
                                        {
                                            barcodeInfo.EANLicenseId = barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                        }
                                        break;
                                    case BarcodeSegmentType.Price:
                                        {
                                            string temp = barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                            barcodeInfo.BarcodePrice = Convert.ToDecimal(temp);
                                            barcodeInfo.BarcodePrice = barcodeInfo.BarcodePrice / (decimal)Math.Pow(10, Segment.Decimals);
                                            barcodeInfo.Decimals = Segment.Decimals;
                                        }
                                        break;
                                    case BarcodeSegmentType.Quantity:
                                        {
                                            string temp = barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                            barcodeInfo.BarcodeQuantity = Convert.ToDecimal(temp);
                                            barcodeInfo.BarcodeQuantity = barcodeInfo.BarcodeQuantity / (decimal)Math.Pow(10, Segment.Decimals);
                                            barcodeInfo.Decimals = Segment.Decimals;
                                        }
                                        break;
                                    case BarcodeSegmentType.Employee:
                                        {
                                            barcodeInfo.EmployeeId = barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                        }
                                        break;
                                    case BarcodeSegmentType.Salesperson:
                                        {
                                            barcodeInfo.SalespersonId = barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                        }
                                        break;
                                    case BarcodeSegmentType.Customer:
                                        {
                                            barcodeInfo.CustomerId = barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                        }
                                        break;
                                    case BarcodeSegmentType.DataEntry:
                                        {
                                            barcodeInfo.DataEntry = barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                        }
                                        break;
                                    case BarcodeSegmentType.Pharmacy:
                                        {
                                            barcodeInfo.PharmacyPrescriptionId = barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                        }
                                        break;
                                    case BarcodeSegmentType.ConfigDigit:
                                        {
                                            barcodeInfo.ConfigId = barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                            break;
                                        }
                                    case BarcodeSegmentType.DiscountCode:
                                        {
                                            barcodeInfo.DiscountCode = barcodeInfo.BarcodeId.Substring(position, Segment.Length).TrimStart('0');
                                            break;
                                        }
                                    case BarcodeSegmentType.GiftCard:
                                        if (barcodeInfoV2)
                                        {
                                            barcodeInfo.GiftCardNumber = barcodeInfo.Prefix + barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                        }
                                        break;
                                    case BarcodeSegmentType.LoyaltyCard:
                                        if (barcodeInfoV2)
                                        {
                                            barcodeInfo.LoyaltyCardNumber = barcodeInfo.Prefix + barcodeInfo.BarcodeId.Substring(position, Segment.Length);
                                        }
                                        break;
          
                                    default:
                                        break;
                                }
                                position = position + Segment.Length;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (connection.State != ConnectionState.Closed) { connection.Close(); }
            }
        }

        /// <summary>
        /// Gets the barcode information if it is found in the system.
        /// </summary>
        /// <param name="barcodeInfo">The barcode info.</param>
        /// <param name="promptIfMultiple">if set to <c>true</c> [prompt if multiple].</param>
        private void Find(IBarcodeInfo barcodeInfo, bool promptIfMultiple = false)
        {
            SqlConnection connection = Application.Settings.Database.Connection;

            try
            {
		        string queryString = "SELECT B.[ITEMID],B.[DESCRIPTION],B.[ITEMBARCODE],B.[INVENTDIMID],B.[RBOVARIANTID],B.[BLOCKED],B.[UNITID],B.[QTY], " +
                                     "ISNULL(D.CONFIGID, '') AS CONFIGID, " +
                                     "ISNULL(D.INVENTSTYLEID, '') AS INVENTSTYLEID, ISNULL(D.INVENTCOLORID, '') AS INVENTCOLORID, ISNULL(D.INVENTSIZEID, '') AS INVENTSIZEID " +
                                     "FROM INVENTITEMBARCODE B " +
                                     "LEFT JOIN INVENTDIM D ON B.INVENTDIMID = D.INVENTDIMID AND B.DATAAREAID = D.DATAAREAID " +
                                     "WHERE B.ITEMBARCODE LIKE @ITEMBARCODE AND B.DATAAREAID=@DATAAREAID ";

                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    SqlParameter barcodeParm = command.Parameters.Add("@ITEMBARCODE", SqlDbType.NVarChar, 80);
                    barcodeParm.Value = barcodeInfo.ItemBarcode;
                    SqlParameter dataAreaIdParm = command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
                    dataAreaIdParm.Value = Application.Settings.Database.DataAreaID;

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.Default))
                    {
                        using (DataTable table = new DataTable())
                        {
                            table.Load(reader);
                            DataRow row = null;
                            bool needRestoreBarcodeId = false;

                            if (table.Rows.Count == 1 || (!promptIfMultiple && (table.Rows.Count > 0)))
                            {   // Select the only row (or first row if prompting is prevented)
                                row = table.Rows[0];
                            }
                            else if (table.Rows.Count > 1)
                            {   // Let the user select if more than one entry was found.
                                DialogResult result = this.Application.Services.Dialog.GenericLookup(table, 1, ref row,
                                    LSRetailPosis.ApplicationLocalizer.Language.Translate(3599)); // An item must be selected.

                                if (result != DialogResult.OK)
                                {
                                    row = null; // Nothing selected
                                }
                                else
                                {
                                    needRestoreBarcodeId = true;
                                }
                            }

                            if (row != null)
                            {
                                barcodeInfo.ItemId = (string) row["ITEMID"];
                                barcodeInfo.Description = (string) row["DESCRIPTION"];

                                if (needRestoreBarcodeId)
                                {
                                    barcodeInfo.BarcodeId = (string)row["ITEMBARCODE"];
                                    barcodeInfo.ItemBarcode = (string)row["ITEMBARCODE"];
                                }

                                //barcodeInfo.InventDimId = reader.GetString(reader.GetOrdinal("InventDimId")) ?? "";
                                barcodeInfo.InventColorId = (string) row["INVENTCOLORID"];
                                barcodeInfo.InventSizeId = (string) row["INVENTSIZEID"];
                                barcodeInfo.InventStyleId = (string) row["INVENTSTYLEID"];
                                barcodeInfo.ConfigId = (string) row["CONFIGID"];
                                barcodeInfo.VariantId = (string) row["RBOVARIANTID"] ?? string.Empty;
                                int intTemp = (int) row["BLOCKED"];
                                if (intTemp == 0) { barcodeInfo.Blocked = false; } else { barcodeInfo.Blocked = true; }
                                barcodeInfo.UnitId = this.Utility.ToString(row["UNITID"]);
                                barcodeInfo.QtySold = this.Utility.ToDecimal(row["QTY"]);
                                barcodeInfo.BarcodeQuantity = this.Utility.ToDecimal(row["QTY"]);
                                barcodeInfo.Message = string.Empty;
                                barcodeInfo.Found = true;
                            }
                            else
                            {
                                barcodeInfo.Message = "Barcode not found";
                                barcodeInfo.Found = false;
                            }

                            if (barcodeInfo.VariantId == null)
                            {
                                barcodeInfo.VariantId = string.Empty;
                            }
                        }
                    }
                }
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Calculates the checkdigit for a barcode, without a checkdigit using the Universal Product Code (UPC) algorithm
        /// </summary>
        /// <remarks>If the barcode contains non-digits then -1 is returned</remarks>
        /// <param name="barcode">barcode without a checkdigit</param>
        /// <returns>The calculated checkdigit</returns>
        private static int CalcCheckDigit(string barcode)
        {
            int even = 0;
            int odd = 0;
            int total = 0;
            int checkDigit = 0;

            for (int i = 0; i < barcode.Length; i++)
            {
                int temp;

                if (int.TryParse(barcode.Substring((barcode.Length - 1 - i), 1), out temp))
                {
                    if (((i + 1) % 2) == 0)
                    {
                        even += temp;
                    }
                    else
                    {
                        odd += temp;
                    }
                }
                else
                {   // Not valid as it contians non-numeric data
                    return -1;
                }
            }

            total = (odd * 3) + even;
            checkDigit = 10 - (total % 10);

            return checkDigit;
        }

        #endregion
    }
}
