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
using LSRetailPosis.DataAccess;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using Services = Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.Item
{
    [Export(typeof(Services.IItem))]
    public class Item : Services.IItem
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        //
        // TextID's for the Item service are reserved at 61000 - 61999
        // TextID's for the following modules are as follows:
        //
        // Item.cs:                                      x - x.  The last in use: x
        // WinFormsTouch/frmItemSearch.cs:               61500 - 61599
        // winformsKeyboard/frmItemSearch.cs:            61600 - 61699
        // WinformsTouch/frmSerialIdSearch.cs:           61700 - 61799
        // WinformsKeyboard/frmSerialIdSearch.cs         61800 - 61899

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
        [Import]
        public IApplication Application { get; set; }

        private IUtility Utility
        {
            get { return this.Application.BusinessLogic.Utility; }
        }

        #region IItem Members

        //Start:Nim
        [Import]
        public IApplication application { get; set; }
        public void MYProcessItem(ISaleLineItem saleLineItem, IApplication Application)
        {
            application = Application;
            ProcessItem(saleLineItem, false);
        }
        //End:Nim

        /// <summary>
        /// Add all information about the item.
        /// </summary>
        /// <param name="saleLineItem"></param>
        public void ProcessItem(ISaleLineItem saleLineItem)
        {
            ProcessItem(saleLineItem, false);
        }

        /// <summary>
        /// Add all information about the item..
        /// </summary>
        /// <param name="saleLineItem">The sale line item.</param>
        /// <param name="bypassSerialNumberEntry">if set to <c>true</c> [bypass serial number entry].</param>
        /// <returns></returns>
        public void ProcessItem(ISaleLineItem saleLineItem, bool bypassSerialNumberEntry)
        {
            SaleLineItem lineItem = (SaleLineItem)saleLineItem;
            if (lineItem == null)
            {
                NetTracer.Warning("saleLineItem parameter is null");
                throw new ArgumentNullException("saleLineItem");
            }

            try
            {
                if (!string.IsNullOrEmpty(lineItem.ItemId))
                {
                    GetInventTableInfo(ref lineItem);
                    if (lineItem.Found)
                    {
                        GetInventTableModuleInfo(ref lineItem);
                        GetRBOInventTableInfo(ref lineItem);
                        GetInventDimInfo(ref lineItem);
                        if (!bypassSerialNumberEntry)
                        {
                            GetDimensionGroupInfo(ref lineItem);
                        }
                        GetIndiaTaxInformation(ref lineItem);
                    }
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Displays the Item Search dialog.
        /// Returns false if the user pressed cancel.
        /// Returns true if the user did choose to sell a selected item.  
        /// In this case the selecedItemId contains the item id of the item being sold.
        /// </summary>
        /// <param name="selectedItemId"></param>
        /// <param name="numberOfDisplayedRows"></param>
        /// <returns></returns>
        public bool ItemSearch(ref string selectedItemId, int numberOfDisplayedRows)
        {
            try
            {
                DialogResult dialogResult = this.Application.Services.Dialog.ItemSearch(numberOfDisplayedRows, ref selectedItemId);

                // Quit if cancel is pressed...
                if (dialogResult == System.Windows.Forms.DialogResult.Cancel)
                {
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        #endregion

        # region Private methods

        /// <summary>
        /// Gets indian tax information for sales line item.
        /// </summary>
        /// <param name="saleLineItem">Sales line item.</param>
        private void GetIndiaTaxInformation(ref SaleLineItem saleLineItem)
        {
            if (Functions.CountryRegion != SupportedCountryRegion.IN)
            {
                return;
            }

            //SqlConnection connection = Application.Settings.Database.Connection;
            //Start:Nim
            SqlConnection connection = new SqlConnection();
            if (Application != null)
            {
                connection = Application.Settings.Database.Connection;
            }
            else
            {
                connection = LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection;
            }
            //ENd:Nim

            try
            {
                const string queryString = @"SELECT TOP 1 IT.ITEMID, ISNULL(HSN.CODE,'') AS HSNCODE, ISNULL(SAC.SAC, '') AS SACCODE, IT.EXEMPT_IN AS EXEMPT
                                            FROM INVENTTABLE IT 
                                            LEFT OUTER JOIN HSNCODETABLE_IN HSN ON IT.HSNCODETABLE_IN = HSN.RECID
                                            LEFT OUTER JOIN SERVICEACCOUNTINGCODETABLE_IN SAC ON IT.SERVICEACCOUNTINGCODETABLE_IN = SAC.RECID
                                            WHERE IT.ITEMID = @ITEMID AND IT.DATAAREAID = @DATAAREAID ";

                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = saleLineItem.ItemId;
                    //command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Application.Settings.Database.DataAreaID;
                    if (Application != null)
                    {
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Application.Settings.Database.DataAreaID;
                    }
                    else
                    {
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                    }
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        reader.Read();

                        if (reader.HasRows)
                        {
                            saleLineItem.HSNCode = reader.GetString(reader.GetOrdinal("HSNCODE"));
                            saleLineItem.ServiceAccountingCode = reader.GetString(reader.GetOrdinal("SACCODE"));
                            saleLineItem.Exempt = Convert.ToBoolean(reader.GetInt32(reader.GetOrdinal("EXEMPT")));//Utility.ToBool
                        }
                        else
                        {
                            saleLineItem.HSNCode = String.Empty;
                            saleLineItem.ServiceAccountingCode = String.Empty;
                            saleLineItem.Exempt = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }


        }

        private void GetInventTableInfo(ref SaleLineItem saleLineItem)
        {
            //SqlConnection connection = Application.Settings.Database.Connection;
            //Start:Nim
            SqlConnection connection = new SqlConnection();
            if (Application != null)
            {
                connection = Application.Settings.Database.Connection;
            }
            else
            {
                connection = LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection;
            }
            //ENd:Nim

            try
            {
                string queryString = "SELECT I.[ITEMTYPE], ISNULL(TR.NAME, I.ITEMNAME) AS ITEMNAME, I.[NAMEALIAS], I.[ITEMGROUPID], I.[DIMGROUPID], I.[PRODUCT] "
                    + "FROM ASSORTEDINVENTITEMS I "
                    + "JOIN ECORESPRODUCT AS PR ON PR.RECID = I.PRODUCT "
                    + "LEFT JOIN ECORESPRODUCTTRANSLATION AS TR ON PR.RECID = TR.PRODUCT AND LANGUAGEID = @CULTUREID "
                    + "WHERE I.ITEMID = @ITEMID AND I.DATAAREAID=@DATAAREAID AND I.STORERECID = @STORERECID ";

                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = saleLineItem.ItemId;
                    // command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Application.Settings.Database.DataAreaID;
                    if (Application != null)
                    {
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Application.Settings.Database.DataAreaID;
                    }
                    else
                    {
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                    }

                    command.Parameters.Add("@CULTUREID", SqlDbType.NVarChar, 7).Value = ApplicationSettings.Terminal.CultureName;
                    command.Parameters.Add("@STORERECID", SqlDbType.BigInt).Value = LSRetailPosis.Settings.ApplicationSettings.Terminal.StorePrimaryId;

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        reader.Read();

                        if (reader.HasRows)
                        {
                            saleLineItem.ItemType = (BaseSaleItem.ItemTypes)reader.GetInt32(reader.GetOrdinal("ITEMTYPE"));
                            //saleLineItem.ItemGroupId = reader.GetString(reader.GetOrdinal("ITEMGROUPID"));//base
                            saleLineItem.ItemGroupId = Convert.ToString(reader.GetOrdinal("ITEMGROUPID"));//Added by Nim
                            saleLineItem.DimensionGroupId = reader.GetString(reader.GetOrdinal("DIMGROUPID"));
                            saleLineItem.DescriptionAlias = reader.GetString(reader.GetOrdinal("NAMEALIAS"));
                            saleLineItem.ProductId = reader.GetInt64(reader.GetOrdinal("PRODUCT"));

                            if (string.IsNullOrEmpty(saleLineItem.Description))
                            {
                                saleLineItem.Description = reader.GetString(reader.GetOrdinal("ITEMNAME"));
                            }

                            saleLineItem.Found = true;
                        }
                        else
                        {
                            saleLineItem.Found = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Grandfather as this causes early loop termination")]
        private void GetInventTableModuleInfo(ref SaleLineItem saleLineItem)
        {
            //SqlConnection connection = Application.Settings.Database.Connection;
            //Start:Nim
            SqlConnection connection = new SqlConnection();
            if (Application != null)
            {
                connection = Application.Settings.Database.Connection;
            }
            else
            {
                connection = LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection;
            }
            //End:Nim

            try
            {
                string queryString = "SELECT M.[LINEDISC],M.[MULTILINEDISC],M.[ENDDISC],ISNULL(M.[UNITID], '') AS UNITID, M.MODULETYPE, M.TAXITEMGROUPID, M.MARKUP, M.ALLOCATEMARKUP, M.PRICEQTY, M.PRICE "
                    + "FROM INVENTTABLEMODULE M "
                    + "WHERE M.ITEMID=@ITEMID AND M.DATAAREAID=@DATAAREAID ORDER BY M.MODULETYPE ASC";

                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = saleLineItem.ItemId;
                    //command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Application.Settings.Database.DataAreaID;
                    //Start:Nim
                    if (Application != null)
                    {
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Application.Settings.Database.DataAreaID;
                    }
                    else
                    {
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                    }
                    //End:Nim

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        saleLineItem.LineDiscountGroup = string.Empty;
                        saleLineItem.MultiLineDiscountGroup = string.Empty;
                        saleLineItem.IncludedInTotalDiscount = false;

                        while (reader.Read())
                        {
                            if ((Int32)reader["MODULETYPE"] == 2)
                            {
                                saleLineItem.BackofficeSalesOrderUnitOfMeasure = reader.GetString(reader.GetOrdinal("UNITID"));
                                saleLineItem.LineDiscountGroup = Convert.ToString(reader["LINEDISC"]);//Utility.ToString
                                saleLineItem.MultiLineDiscountGroup = Convert.ToString(reader["MULTILINEDISC"]);//Utility.ToString
                                saleLineItem.IncludedInTotalDiscount = Convert.ToBoolean(reader["ENDDISC"]);//Utility.ToBool

                                saleLineItem.TaxGroupId = Convert.ToString(reader["TAXITEMGROUPID"]);
                                saleLineItem.TaxGroupIdOriginal = saleLineItem.TaxGroupId;
                                saleLineItem.Markup = reader.GetDecimal(reader.GetOrdinal("MARKUP"));
                                saleLineItem.PriceQty = reader.GetDecimal(reader.GetOrdinal("PRICEQTY"));
                                // if value is 1, it is per unit Price charge  - for some reason, for a boolean flag, AX uses an entire int.
                                saleLineItem.AllocateMarkup = (reader.GetInt32(reader.GetOrdinal("ALLOCATEMARKUP")) == 1);

                                reader.Close();
                                //Base
                                //UnitOfMeasureData uomData = new UnitOfMeasureData(
                                //    connection,
                                //    this.Application.Settings.Database.DataAreaID,
                                //    LSRetailPosis.Settings.ApplicationSettings.Terminal.StorePrimaryId,
                                //    this.Application);
                                //saleLineItem.BackofficeSalesOrderUnitOfMeasureName = uomData.GetUnitName(saleLineItem.BackofficeSalesOrderUnitOfMeasure);

                                //Start:Nim
                                UnitOfMeasureData uomData;
                                if (Application != null)
                                {
                                    uomData = new UnitOfMeasureData(
                                        connection,
                                        this.Application.Settings.Database.DataAreaID,
                                        LSRetailPosis.Settings.ApplicationSettings.Terminal.StorePrimaryId,
                                        this.Application);
                                    saleLineItem.BackofficeSalesOrderUnitOfMeasureName = uomData.GetUnitName(saleLineItem.BackofficeSalesOrderUnitOfMeasure);
                                }
                                else
                                {
                                    uomData = new UnitOfMeasureData(
                                              connection,
                                              ApplicationSettings.Database.DATAAREAID,
                                              LSRetailPosis.Settings.ApplicationSettings.Terminal.StorePrimaryId,
                                              application);
                                    saleLineItem.BackofficeSalesOrderUnitOfMeasureName = uomData.GetUnitName(saleLineItem.BackofficeSalesOrderUnitOfMeasure);
                                }
                                //End:Nim

                                if (!string.IsNullOrEmpty(saleLineItem.BarcodeId) || !string.IsNullOrEmpty(saleLineItem.SalesOrderUnitOfMeasure))
                                {   // The unit of measure on the barcode always takes presedense over the unit of measure on the item itself.

                                    saleLineItem.SalesOrderUnitOfMeasureName = uomData.GetUnitName(saleLineItem.SalesOrderUnitOfMeasure);
                                    saleLineItem.UnitQtyConversion = uomData.GetUOMFactor(saleLineItem.SalesOrderUnitOfMeasure, saleLineItem.BackofficeSalesOrderUnitOfMeasure, saleLineItem);
                                }
                                else
                                {   // initialize the active unit of measure for the item to this as the default
                                    saleLineItem.SalesOrderUnitOfMeasure = saleLineItem.BackofficeSalesOrderUnitOfMeasure;
                                    // Default is SalesOrderUnitOfMeasureName == BackofficeSalesOrderUnitOfMeasureName
                                    saleLineItem.SalesOrderUnitOfMeasureName = saleLineItem.BackofficeSalesOrderUnitOfMeasureName;

                                    //base
                                    //saleLineItem.UnitQtyConversion = (UnitQtyConversion)this.Application.BusinessLogic.Utility.CreateUnitQtyConversion();

                                    //Start:Nim
                                    if (Application != null)
                                    {
                                        saleLineItem.UnitQtyConversion = (UnitQtyConversion)this.Application.BusinessLogic.Utility.CreateUnitQtyConversion();
                                    }
                                    else
                                    {
                                        saleLineItem.UnitQtyConversion = (UnitQtyConversion)application.BusinessLogic.Utility.CreateUnitQtyConversion();
                                    }
                                    //End:Nim

                                }
                                // Must break out because at this point the reader has been closed
                                break;
                            }
                            else if ((Int32)reader["MODULETYPE"] == 0)
                            {
                                saleLineItem.InventOrderUnitOfMeasure = (string)reader["UNITID"] ?? string.Empty;
                                saleLineItem.CostPrice = (decimal)reader["PRICE"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfather")]
        private void GetRBOInventTableInfo(ref SaleLineItem saleLineItem)
        {
            //SqlConnection connection = Application.Settings.Database.Connection;

            //Start:Nim
            SqlConnection connection = new SqlConnection();
            if (Application != null)
            {
                connection = Application.Settings.Database.Connection;
            }
            else
            {
                connection = LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection;
            }
            //End:Nim

            try
            {
                string queryString = @" SELECT QTYBECOMESNEGATIVE,    ZEROPRICEVALID,    NODISCOUNTALLOWED, SCALEITEM, BLOCKEDONPOS, DATETOBEBLOCKED, 
                                    ISNULL(DATETOACTIVATEITEM, 0) AS DATETOACTIVATEITEM, 
                                    KEYINGINQTY, KEYINGINPRICE,    MUSTKEYINCOMMENT" +
                                    ", PROHIBITRETURN_RU " +
                                    @"FROM RETAILINVENTTABLE 
                                    WHERE ITEMID = @ITEMID AND DATAAREAID=@DATAAREAID ";

                using (SqlCommand command = new SqlCommand(queryString.ToString(), connection))
                {
                    command.Parameters.AddWithValue("@ITEMID", saleLineItem.ItemId);
                    //command.Parameters.AddWithValue("@DATAAREAID", Application.Settings.Database.DataAreaID);
                    //Start:Nim
                    if (Application != null)
                    {
                        command.Parameters.AddWithValue("@DATAAREAID", Application.Settings.Database.DataAreaID);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@DATAAREAID", ApplicationSettings.Database.DATAAREAID);
                    }
                    //End:Nim

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        reader.Read();

                        if (reader.HasRows)
                        {
                            saleLineItem.QtyBecomesNegative = ((int)reader["QTYBECOMESNEGATIVE"] != 0);
                            saleLineItem.ZeroPriceValid = ((int)reader["ZEROPRICEVALID"] != 0);
                            saleLineItem.NoDiscountAllowed = ((int)reader["NODISCOUNTALLOWED"] != 0);
                            saleLineItem.ScaleItem = ((int)reader["SCALEITEM"] != 0);
                            saleLineItem.Blocked |= ((int)reader["BLOCKEDONPOS"] == 1);
                            saleLineItem.DateToBeBlocked = (DateTime)reader["DATETOBEBLOCKED"];
                            saleLineItem.DateToActivateItem = (DateTime)reader["DATETOACTIVATEITEM"];
                            int keyingInQty = (int)reader["KEYINGINQTY"];
                            int keyingInPrice = (int)reader["KEYINGINPRICE"];

                            switch (keyingInQty)
                            {
                                case 0: saleLineItem.KeyInQuantity = KeyInQuantities.NotMandatory; break;
                                case 1: saleLineItem.KeyInQuantity = KeyInQuantities.MustKeyInQuantity; break;
                                case 2: saleLineItem.KeyInQuantity = KeyInQuantities.EnteringQuantityNotAllowed; break;
                            }

                            switch (keyingInPrice)
                            {
                                case 0: saleLineItem.KeyInPrice = KeyInPrices.NotMandatory; break;
                                case 1: saleLineItem.KeyInPrice = KeyInPrices.MustKeyInNewPrice; break;
                                case 2: saleLineItem.KeyInPrice = KeyInPrices.MustKeyInEqualHigherPrice; break;
                                case 3: saleLineItem.KeyInPrice = KeyInPrices.MustKeyInEqualLowerPrice; break;
                                case 4: saleLineItem.KeyInPrice = KeyInPrices.EnteringPriceNotAllowed; break;
                            }

                            if (reader["MUSTKEYINCOMMENT"] != System.DBNull.Value)
                            {
                                saleLineItem.MustKeyInComment = ((int)reader["MUSTKEYINCOMMENT"] == 1);
                            }
                            else
                            {
                                saleLineItem.MustKeyInComment = false;
                            }

                            saleLineItem.ProhibitReturn = ((int)reader["PROHIBITRETURN_RU"] != 0);
                        }
                        else
                        {
                            saleLineItem.QtyBecomesNegative = false;
                            saleLineItem.ZeroPriceValid = false;
                            saleLineItem.NoDiscountAllowed = false;
                            saleLineItem.ScaleItem = false;
                            saleLineItem.MustKeyInComment = false;
                            saleLineItem.ProhibitReturn = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        private void GetInventDimInfo(ref SaleLineItem saleLineItem)
        {
            //SqlConnection connection = Application.Settings.Database.Connection;
            //Start:Nim
            SqlConnection connection = new SqlConnection();
            if (Application != null)
            {
                connection = Application.Settings.Database.Connection;
            }
            else
            {
                connection = LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection;
            }
            //End:Nim

            try
            {
                if (string.IsNullOrEmpty(saleLineItem.Dimension.VariantId))
                {
                    //If variant is not set for the item but dimensions are found the enter them manually
                    string queryString = "SELECT ITEMID FROM ASSORTEDINVENTDIMCOMBINATION I WHERE I.ITEMID = @ITEMID AND I.DATAAREAID=@DATAAREAID AND I.STORERECID = @STORERECID ";
                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        command.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = saleLineItem.ItemId;
                        //command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Application.Settings.Database.DataAreaID;
                        //Start:Nim
                        if (Application != null)
                        {
                            command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Application.Settings.Database.DataAreaID;
                        }
                        else
                        {
                            command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                        }
                        //End:Nim

                        command.Parameters.Add("@STORERECID", SqlDbType.BigInt).Value = LSRetailPosis.Settings.ApplicationSettings.Terminal.StorePrimaryId;

                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            reader.Read();
                            saleLineItem.Dimension.EnterDimensions = reader.HasRows;
                        }
                    }
                }
                else
                {
                    string queryString = @"
                            SELECT I.INVENTSIZEID AS SIZEID, I.INVENTCOLORID AS COLORID, I.INVENTSTYLEID AS STYLEID, I.CONFIGID AS CONFIGID, 
                    ISNULL(DVC.DESCRIPTION, '') AS COLOR, ISNULL(DVSZ.DESCRIPTION, '') AS SIZE, ISNULL(DVST.DESCRIPTION, '') AS STYLE, 
                    ISNULL(DVCFG.DESCRIPTION, '') AS CONFIG, 
                    IDC.DISTINCTPRODUCTVARIANT
                    FROM ASSORTEDINVENTDIMCOMBINATION AS IDC
                    INNER JOIN INVENTDIM I ON I.INVENTDIMID = IDC.INVENTDIMID AND I.DATAAREAID = IDC.DATAAREAID
                    INNER JOIN ASSORTEDINVENTITEMS AIT ON AIT.ITEMID = IDC.ITEMID AND AIT.STORERECID = @STORERECID

                                        LEFT OUTER JOIN ECORESCOLOR ON ECORESCOLOR.NAME = I.INVENTCOLORID
                                        LEFT OUTER JOIN ECORESPRODUCTMASTERCOLOR ON (ECORESPRODUCTMASTERCOLOR.COLOR = ECORESCOLOR.RECID)
                                        AND (ECORESPRODUCTMASTERCOLOR.COLORPRODUCTMASTER = AIT.PRODUCT)
                                        LEFT OUTER JOIN ECORESPRODUCTMASTERDIMENSIONVALUE DVC ON DVC.RECID = ECORESPRODUCTMASTERCOLOR.RECID

                                        LEFT OUTER JOIN ECORESSIZE ON ECORESSIZE.NAME = I.INVENTSIZEID
                                        LEFT OUTER JOIN ECORESPRODUCTMASTERSIZE ON (ECORESPRODUCTMASTERSIZE.SIZE_ = ECORESSIZE.RECID)
                                        AND (ECORESPRODUCTMASTERSIZE.SIZEPRODUCTMASTER = AIT.PRODUCT)
                                        LEFT OUTER JOIN ECORESPRODUCTMASTERDIMENSIONVALUE DVSZ ON DVSZ.RECID = ECORESPRODUCTMASTERSIZE.RECID

                                        LEFT OUTER JOIN ECORESSTYLE ON ECORESSTYLE.NAME = I.INVENTSTYLEID
                    LEFT OUTER JOIN ECORESPRODUCTMASTERSTYLE ON (ECORESPRODUCTMASTERSTYLE.STYLE = ECORESSTYLE.RECID)
                    AND (ECORESPRODUCTMASTERSTYLE.STYLEPRODUCTMASTER = AIT.PRODUCT)
                    LEFT OUTER JOIN ECORESPRODUCTMASTERDIMENSIONVALUE DVST ON DVST.RECID = ECORESPRODUCTMASTERSTYLE.RECID

                                        LEFT OUTER JOIN ECORESCONFIGURATION ON ECORESCONFIGURATION.NAME = I.CONFIGID
                                        LEFT OUTER JOIN ECORESPRODUCTMASTERCONFIGURATION ON (ECORESPRODUCTMASTERCONFIGURATION.CONFIGURATION = ECORESCONFIGURATION.RECID)
                                        AND (ECORESPRODUCTMASTERCONFIGURATION.CONFIGPRODUCTMASTER = AIT.PRODUCT)
                                        LEFT OUTER JOIN ECORESPRODUCTMASTERDIMENSIONVALUE DVCFG ON DVCFG.RECID = ECORESPRODUCTMASTERCONFIGURATION.RECID

                                        WHERE (IDC.RETAILVARIANTID = @RetailVariantId) AND (IDC.DATAAREAID = @DATAAREAID) AND (IDC.STORERECID = @STORERECID) ";

                    using (SqlCommand command = new SqlCommand(queryString, connection))
                    {
                        command.Parameters.Add("@STORERECID", SqlDbType.BigInt).Value = LSRetailPosis.Settings.ApplicationSettings.Terminal.StorePrimaryId;
                        command.Parameters.Add("@RETAILVARIANTID", SqlDbType.NVarChar, 20).Value = saleLineItem.Dimension.VariantId;
                        // command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Application.Settings.Database.DataAreaID;
                        //Start:Nim
                        if (Application != null)
                        {
                            command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Application.Settings.Database.DataAreaID;
                        }
                        else
                        {
                            command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                        }
                        //End:Nim

                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                        {
                            reader.Read();

                            if (reader.HasRows)
                            {
                                saleLineItem.Dimension.ColorId = Utility.ToString(reader["COLORID"]);
                                saleLineItem.Dimension.SizeId = Utility.ToString(reader["SIZEID"]);
                                saleLineItem.Dimension.StyleId = Utility.ToString(reader["STYLEID"]);
                                saleLineItem.Dimension.ConfigId = Utility.ToString(reader["CONFIGID"]);

                                saleLineItem.Dimension.ColorName = Utility.ToString(reader["COLOR"]);
                                saleLineItem.Dimension.SizeName = Utility.ToString(reader["SIZE"]);
                                saleLineItem.Dimension.StyleName = Utility.ToString(reader["STYLE"]);
                                saleLineItem.Dimension.ConfigName = Utility.ToString(reader["CONFIG"]);
                                saleLineItem.Dimension.DistinctProductVariantId = (Int64)reader["DISTINCTPRODUCTVARIANT"];

                                saleLineItem.Dimension.EnterDimensions = false;
                            }
                            else
                            {
                                saleLineItem.Dimension.EnterDimensions = true;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfather")]
        private void GetDimensionGroupInfo(ref SaleLineItem saleLineItem)
        {
            //SqlConnection connection = Application.Settings.Database.Connection;
            //Start:Nim
            SqlConnection connection = new SqlConnection();
            if (Application != null)
            {
                connection = Application.Settings.Database.Connection;
            }
            else
            {
                connection = LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection;
            }
            //End:Nim


            try
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                bool hasTrackingDimensionWithSerialAlive = false;
                bool allowBlank = true;

                //Find out whether the item's tracking dimension group has active serial number setup. Serial number is one of field in tracking dimension group
                //and the value is 5. The rest are invent dimensions are listed below (including product dimension/storage dimension/tracking dimension. Ideally 
                //we should define ENUM for this, but we don't use most of these.

                //1, Dimension No.
                //2, Batch number
                //3, Location
                //4, Pallet ID
                //5, Serial number
                //6, Warehouse
                //7, Configuration
                //8, Size
                //9, Color

                // Table ECORESTRACKINGDIMENSIONGROUPITEM will always have an entry if Tracking dimension group is assigned either on the product/ release product. 
                string queryString = @"
                                select I.ITEMID, DGP.TRACKINGDIMENSIONGROUP, DGF.DIMENSIONFIELDID, DGF.ISACTIVE, DGF.ISALLOWBLANKISSUEENABLED
                                from ASSORTEDINVENTITEMS I 
                                inner join ECORESTRACKINGDIMENSIONGROUPITEM DGP on I.ITEMID = DGP.ITEMID AND I.DATAAREAID = DGP.ITEMDATAAREAID                               
                                inner join ECORESTRACKINGDIMENSIONGROUPFLDSETUP DGF on DGP.TRACKINGDIMENSIONGROUP = DGF.TRACKINGDIMENSIONGROUP
                                where DGF.DIMENSIONFIELDID = 5 and DGF.ISACTIVE = 1
                                and I.ITEMID = @ITEMID and I.STORERECID = @STORERECID ";

                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = saleLineItem.ItemId;
                    command.Parameters.Add("@STORERECID", SqlDbType.BigInt).Value = LSRetailPosis.Settings.ApplicationSettings.Terminal.StorePrimaryId;

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleResult))
                    {
                        reader.Read();

                        if (reader.HasRows)
                        {
                            hasTrackingDimensionWithSerialAlive = true;
                            allowBlank = Utility.ToBool(reader.GetInt32(reader.GetOrdinal("ISALLOWBLANKISSUEENABLED")));
                        }
                    }
                }

                if (hasTrackingDimensionWithSerialAlive)
                {
                    string querySerial = " SELECT ISNULL(INVENTSERIALID, '') AS INVENTSERIALID, ISNULL(RFIDTAGID, '') AS RFIDTAGID FROM INVENTSERIAL WHERE DATAAREAID = @DATAAREAID2 ";

                    if (!string.IsNullOrEmpty(saleLineItem.RFIDTagId))
                    {
                        querySerial += " AND RFIDTAGID = @ITEMID ";
                    }
                    else
                    {
                        querySerial += " AND ITEMID = @ITEMID ";
                    }

                    using (SqlCommand serialCommand = new SqlCommand(querySerial, connection))
                    {

                        if (!string.IsNullOrEmpty(saleLineItem.RFIDTagId))
                        {
                            serialCommand.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 24).Value = saleLineItem.RFIDTagId;
                        }
                        else
                        {
                            serialCommand.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = saleLineItem.ItemId;
                        }

                        //serialCommand.Parameters.Add("@DATAAREAID2", SqlDbType.NVarChar, 4).Value = Application.Settings.Database.DataAreaID;

                        //Start:Nim
                        if (Application != null)
                        {
                            serialCommand.Parameters.Add("@DATAAREAID2", SqlDbType.NVarChar, 4).Value = Application.Settings.Database.DataAreaID;
                        }
                        else
                        {
                            serialCommand.Parameters.Add("@DATAAREAID2", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                        }
                        //End:Nim


                        using (SqlDataReader serialReader = serialCommand.ExecuteReader())
                        {
                            using (DataTable serialNo = new DataTable())
                            {
                                serialNo.Load(serialReader);

                                if (serialNo.Rows.Count == 0)
                                {
                                    InputSerialNumber(saleLineItem, allowBlank, this.Application);
                                }
                                else if (serialNo.Rows.Count == 1)
                                {
                                    saleLineItem.SerialId = (string)serialNo.Rows[0]["INVENTSERIALID"];
                                    saleLineItem.RFIDTagId = (string)serialNo.Rows[0]["RFIDTAGID"];
                                }
                                else
                                {
                                    SelectSerialNumber(saleLineItem, allowBlank, this.Application);
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (connection.State != ConnectionState.Closed)
                {
                    connection.Close();
                }
            }
        }

        private static void SelectSerialNumber(ISaleLineItem saleLineItem, bool allowBlank, IApplication application)
        {
            using (WinFormsTouch.frmSerialIdSearch searchDialog = new WinFormsTouch.frmSerialIdSearch(100))
            {
                searchDialog.ItemId = saleLineItem.ItemId;

                // Show the search dialog

                bool inputValid;
                do
                {
                    inputValid = true;

                    application.ApplicationFramework.POSShowForm(searchDialog);

                    // Quit if cancel is pressed...
                    if (searchDialog.DialogResult == System.Windows.Forms.DialogResult.Cancel && allowBlank)
                    {
                        return;
                    }
                    else if (searchDialog.DialogResult == DialogResult.Cancel && !allowBlank)
                    {
                        inputValid = false;
                    }
                } while (!inputValid);

                saleLineItem.SerialId = searchDialog.SelectedSerialId;
                saleLineItem.RFIDTagId = searchDialog.SelectedRFIDTagId;
            }
        }

        private static void InputSerialNumber(ISaleLineItem saleLineItem, bool allowBlank, IApplication application)
        {
            bool inputValid = true;
            do
            {
                InputConfirmation inputConfirmation = new InputConfirmation()
                {
                    MaxLength = 20,
                    PromptText = LSRetailPosis.ApplicationLocalizer.Language.Translate(61000), //Enter serial no.
                };

                InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(inputConfirmation, () =>
                {
                    if (inputConfirmation.Confirmed)
                    {
                        saleLineItem.SerialId = inputConfirmation.EnteredText;
                        inputValid = true;
                    }
                    else
                    {
                        inputValid = allowBlank;
                    }
                }
                );

                application.Services.Interaction.InteractionRequest(request);
            } while (!inputValid);
        }

        # endregion
    }
}
