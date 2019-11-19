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
using System.Threading;
using LSRetailPosis.DataAccess.DataUtil;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using DE = Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.Dimension
{
    [Export(typeof(IDimension))]
    public class Dimension : IDimension
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        // TextID's for Dimension are reserved at 50100 - 50149

        [Import]
        public IApplication Application { get; set; }

        #region IDimension Members

        const string querySelectClause = @" 
            SELECT TR.NAME AS DESCRIPTION, IDC.RETAILVARIANTID AS VARIANTID, IDC.DISTINCTPRODUCTVARIANT,  
            I.INVENTSIZEID AS SIZEID, I.INVENTCOLORID AS COLORID, I.INVENTSTYLEID AS STYLEID, I.CONFIGID AS CONFIGID,
            ISNULL(DVC.DESCRIPTION, '') AS COLOR, ISNULL(DVSZ.DESCRIPTION, '') AS SIZE, ISNULL(DVST.DESCRIPTION, '') AS STYLE, 
            ISNULL(DVCFG.DESCRIPTION, '') AS CONFIG, B.ITEMBARCODE, ISNULL(B.UNITID, '') AS UNITID,
            DVC.RETAILDISPLAYORDER AS COLORDISPLAYORDER, DVSZ.RETAILDISPLAYORDER AS SIZEDISPLAYORDER, DVST.RETAILDISPLAYORDER AS STYLEDISPLAYORDER
            FROM ASSORTEDINVENTDIMCOMBINATION IDC
            INNER JOIN ASSORTEDINVENTITEMS AIT ON AIT.ITEMID = IDC.ITEMID
            INNER JOIN INVENTDIM I ON I.INVENTDIMID = IDC.INVENTDIMID AND I.DATAAREAID = IDC.DATAAREAID

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

            LEFT OUTER JOIN INVENTITEMBARCODE B ON IDC.RETAILVARIANTID = B.RBOVARIANTID AND IDC.DATAAREAID = B.DATAAREAID

            LEFT JOIN ECORESPRODUCTTRANSLATION AS TR ON IDC.DISTINCTPRODUCTVARIANT = TR.PRODUCT AND TR.LANGUAGEID = @CULTUREID";

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Caller is responsible for disposing returned object</remarks>
        /// <param name="itemId"></param>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "Caller is responsible for disposing returned object")]
        public System.Data.DataTable GetDimensions(string itemId)
        {
            LSRetailPosis.ApplicationLog.Log("Dimension.GetDimensions", "Get dimensions", LSRetailPosis.LogTraceLevel.Trace);

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
                string sql = querySelectClause + @" 
                    WHERE IDC.ITEMID = @ITEMID
                    AND IDC.DATAAREAID=@DATAAREAID
                    AND IDC.STORERECID=@STORERECID
                    AND AIT.STORERECID=@STORERECID";

                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    command.Parameters.AddWithValue("@ITEMID", itemId);
                    //command.Parameters.AddWithValue("@DATAAREAID", Application.Settings.Database.DataAreaID);
                    if (Application != null)
                    {
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Application.Settings.Database.DataAreaID;
                    }
                    else
                    {
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;
                    }

                    command.Parameters.AddWithValue("@STORERECID", LSRetailPosis.Settings.ApplicationSettings.Terminal.StorePrimaryId);
                    command.Parameters.AddWithValue("@CULTUREID", Thread.CurrentThread.CurrentUICulture.Name);

                    if (connection.State != ConnectionState.Open) { connection.Open(); }

                    using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                    {
                        System.Data.DataTable table = new System.Data.DataTable();
                        adapter.Fill(table);

                        return table;
                    }
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open) { connection.Close(); }
            }
        }

        public void GetDimensionForVariant(DE.IDimension dimension)
        {
            if (dimension == null)
            {
                throw new ArgumentNullException("dimension");
            }

            LSRetailPosis.ApplicationLog.Log("Dimension.GetDimensionForVariant", "Get dimension for variant", LSRetailPosis.LogTraceLevel.Trace);

            try
            {
                string sql = querySelectClause + @" 
                    WHERE IDC.RETAILVARIANTID = @RETAILVARIANTID
                    AND IDC.DATAAREAID = @DATAAREAID
                    AND IDC.STORERECID = @STORERECID
                    AND AIT.STORERECID = @STORERECID";


                if (Application != null)
                {
                    using (DataTable layoutTable = new DBUtil(Application.Settings.Database.Connection).GetTable(sql,
                     new SqlParameter("@RETAILVARIANTID", dimension.VariantId),
                     new SqlParameter("@DATAAREAID", Application.Settings.Database.DataAreaID),
                     new SqlParameter("@STORERECID", LSRetailPosis.Settings.ApplicationSettings.Terminal.StorePrimaryId),
                     new SqlParameter("@CULTUREID", Thread.CurrentThread.CurrentUICulture.Name)))
                    {
                        if (layoutTable.Rows.Count > 0)
                        {
                            dimension.SizeId = layoutTable.Rows[0]["SIZEID"].ToString();
                            dimension.ColorId = layoutTable.Rows[0]["COLORID"].ToString();
                            dimension.StyleId = layoutTable.Rows[0]["STYLEID"].ToString();
                            dimension.ConfigId = layoutTable.Rows[0]["CONFIGID"].ToString();
                            dimension.SizeName = layoutTable.Rows[0]["SIZE"].ToString();
                            dimension.ColorName = layoutTable.Rows[0]["COLOR"].ToString();
                            dimension.StyleName = layoutTable.Rows[0]["STYLE"].ToString();
                            dimension.ConfigName = layoutTable.Rows[0]["CONFIG"].ToString();
                            dimension.DistinctProductVariantId = (Int64)layoutTable.Rows[0]["DISTINCTPRODUCTVARIANT"];
                        }
                    }
                }
                else
                {
                    using (DataTable layoutTable = new DBUtil(ApplicationSettings.Database.LocalConnection).GetTable(sql,
                     new SqlParameter("@RETAILVARIANTID", dimension.VariantId),
                     new SqlParameter("@DATAAREAID", ApplicationSettings.Database.DATAAREAID),
                     new SqlParameter("@STORERECID", LSRetailPosis.Settings.ApplicationSettings.Terminal.StorePrimaryId),
                     new SqlParameter("@CULTUREID", Thread.CurrentThread.CurrentUICulture.Name)))
                    {
                        if (layoutTable.Rows.Count > 0)
                        {
                            dimension.SizeId = layoutTable.Rows[0]["SIZEID"].ToString();
                            dimension.ColorId = layoutTable.Rows[0]["COLORID"].ToString();
                            dimension.StyleId = layoutTable.Rows[0]["STYLEID"].ToString();
                            dimension.ConfigId = layoutTable.Rows[0]["CONFIGID"].ToString();
                            dimension.SizeName = layoutTable.Rows[0]["SIZE"].ToString();
                            dimension.ColorName = layoutTable.Rows[0]["COLOR"].ToString();
                            dimension.StyleName = layoutTable.Rows[0]["STYLE"].ToString();
                            dimension.ConfigName = layoutTable.Rows[0]["CONFIG"].ToString();
                            dimension.DistinctProductVariantId = (Int64)layoutTable.Rows[0]["DISTINCTPRODUCTVARIANT"];
                        }
                    }
                }

                
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        #endregion
    }
}
