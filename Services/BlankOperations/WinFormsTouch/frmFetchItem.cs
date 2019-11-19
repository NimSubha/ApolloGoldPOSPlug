using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Collections.ObjectModel;
using LSRetailPosis.Settings;
using System.IO;
using System.Data.SqlClient;
using System.ComponentModel.Composition;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmFetchItem : frmTouchBase
    {
        [Import]
        private IApplication application;
        public IPosTransaction pos { get; set; }

        DataTable dtInventTable = new DataTable();
        DataTable dtInventTableModule = new DataTable();
        DataTable dtEcoResProduct = new DataTable();
        DataTable dtEcoResProductCategory = new DataTable();
        DataTable dtEcoResProductTranslation = new DataTable();
        DataTable dtEcoResTrackingDimensionGroupItem = new DataTable();
        DataTable dtEcoResTrackingDimensionGroupProduct = new DataTable();
        DataTable dtInventDim = new DataTable();
        DataTable dtInventDimCombination = new DataTable();
        DataTable dtInventItemBarcode = new DataTable();
        DataTable dtInventItemGroupItem = new DataTable();
        DataTable dtRetailAssortmentExploded = new DataTable();
        DataTable dtRetailInventTable = new DataTable();
        DataTable dtUnitOfMeasureConversion = new DataTable();
        DataTable dtEcoResProductMasterDimensionValue_Config = new DataTable();
        DataTable dtEcoResProductMasterDimensionValue_Color = new DataTable();
        DataTable dtEcoResProductMasterDimensionValue_size = new DataTable();
        DataTable dtEcoResProductMasterDimensionValue_style = new DataTable();
        DataTable dtSKUTable = new DataTable();
        DataTable dtSKULine = new DataTable();
        DataTable dtPDSCWItem = new DataTable();
        DataTable dtPriceDiscTable = new DataTable();
        DataTable dtConfigMaster = new DataTable();
        DataTable dtColorMaster = new DataTable();
        DataTable dtSizeMaster = new DataTable();
        DataTable dtStyleMaster = new DataTable();

        DataTable dtEcoConfig = new DataTable();
        DataTable dtEcoColor = new DataTable();
        DataTable dtEcoSize = new DataTable();
        DataTable dtEcoStyle = new DataTable();
        DataTable dtHSN = new DataTable();
        DataTable dtSAC = new DataTable();

        int iAnyTableEffected = 0;

        public frmFetchItem()
        {
            InitializeComponent();
        }

        public frmFetchItem(IPosTransaction posTransaction, IApplication Application)
        {
            InitializeComponent();
        }

        private void btnFetch_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtItem.Text))
            {
                //if (!IsExistInLocal(txtItem.Text))
                //{
                try
                {
                    if (PosApplication.Instance.TransactionServices.CheckConnection())
                    {

                        string sStoreId = ApplicationSettings.Database.StoreID;
                        ReadOnlyCollection<object> containerArray;
                        containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetItemDetails", txtItem.Text, sStoreId);

                        if (Convert.ToBoolean(containerArray[1]) == true)
                        {
                            #region DataSet
                            DataSet dsIT = new DataSet();
                            DataSet dsITM = new DataSet();
                            DataSet dsERP = new DataSet();
                            DataSet dsERPC = new DataSet();
                            DataSet dsERPT = new DataSet();
                            DataSet dsERTDG = new DataSet();
                            DataSet dsERTDGP = new DataSet();
                            DataSet dsID = new DataSet();
                            DataSet dsIDC = new DataSet();
                            DataSet dsIIB = new DataSet();
                            DataSet dsIIGI = new DataSet();
                            DataSet dsRAE = new DataSet();
                            DataSet dsRIT = new DataSet();
                            DataSet dsUMC = new DataSet();
                            DataSet dsConf = new DataSet();
                            DataSet dsSize = new DataSet();
                            DataSet dsColor = new DataSet();
                            DataSet dsStyle = new DataSet();
                            DataSet dsSKUTable = new DataSet();
                            DataSet dsSKULine = new DataSet();
                            DataSet dsPDSCWItem = new DataSet();
                            DataSet dsPriceDiscTable = new DataSet();
                            DataSet dsConfMaster = new DataSet();
                            DataSet dsSizeMaster = new DataSet();
                            DataSet dsColorMaster = new DataSet();
                            DataSet dsStyleMaster = new DataSet();

                            DataSet dsEcoConf = new DataSet();
                            DataSet dsEcoSize = new DataSet();
                            DataSet dsEcoColor = new DataSet();
                            DataSet dsEcoStyle = new DataSet();
                            DataSet dsHSN = new DataSet();
                            DataSet dsSAC = new DataSet();
                            #endregion

                            #region StringReader
                            StringReader srIT = new StringReader(Convert.ToString(containerArray[3]));
                            StringReader srITM = new StringReader(Convert.ToString(containerArray[4]));
                            StringReader srERP = new StringReader(Convert.ToString(containerArray[5]));
                            StringReader srERPC = new StringReader(Convert.ToString(containerArray[6]));
                            StringReader srERPT = new StringReader(Convert.ToString(containerArray[7]));
                            StringReader srERTDG = new StringReader(Convert.ToString(containerArray[8]));
                            StringReader srERTDGP = new StringReader(Convert.ToString(containerArray[9]));
                            StringReader srID = new StringReader(Convert.ToString(containerArray[10]));
                            StringReader srIDC = new StringReader(Convert.ToString(containerArray[11]));
                            StringReader srIIB = new StringReader(Convert.ToString(containerArray[12]));
                            StringReader srIIGI = new StringReader(Convert.ToString(containerArray[13]));
                            StringReader srRAE = new StringReader(Convert.ToString(containerArray[14]));
                            StringReader srRIT = new StringReader(Convert.ToString(containerArray[15]));
                            StringReader srUMC = new StringReader(Convert.ToString(containerArray[16]));
                            StringReader srConf = new StringReader(Convert.ToString(containerArray[17]));
                            StringReader srSize = new StringReader(Convert.ToString(containerArray[18]));
                            StringReader srColor = new StringReader(Convert.ToString(containerArray[19]));
                            StringReader srStyle = new StringReader(Convert.ToString(containerArray[20]));
                            StringReader srSKUTable = new StringReader(Convert.ToString(containerArray[21]));
                            StringReader srSKULine = new StringReader(Convert.ToString(containerArray[22]));
                            StringReader srPDSCWItem = new StringReader(Convert.ToString(containerArray[23]));
                            StringReader srPDT = new StringReader(Convert.ToString(containerArray[24]));
                            StringReader srConfMstr = new StringReader(Convert.ToString(containerArray[25]));
                            StringReader srColorMstr = new StringReader(Convert.ToString(containerArray[26]));
                            StringReader srSizeMstr = new StringReader(Convert.ToString(containerArray[27]));
                            StringReader srStyleMstr = new StringReader(Convert.ToString(containerArray[28]));

                            StringReader srEcoConf = new StringReader(Convert.ToString(containerArray[29]));
                            StringReader srEcoColor = new StringReader(Convert.ToString(containerArray[30]));
                            StringReader srEcoSize = new StringReader(Convert.ToString(containerArray[31]));
                            StringReader srEcoStyle = new StringReader(Convert.ToString(containerArray[32]));
                            StringReader srHSN = new StringReader(Convert.ToString(containerArray[33]));
                            StringReader srSAC = new StringReader(Convert.ToString(containerArray[34]));
                            #endregion

                            #region Get Table data
                            if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                            {
                                dsIT.ReadXml(srIT);
                            }
                            if (dsIT.Tables.Count > 0)
                            {
                                if (dsIT != null && dsIT.Tables[0].Rows.Count > 0)
                                {
                                    dtInventTable = dsIT.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[4]).Trim().Length > 38)
                            {
                                dsITM.ReadXml(srITM);
                            }
                            if (dsITM.Tables.Count > 0)
                            {
                                if (dsITM != null && dsITM.Tables[0].Rows.Count > 0)
                                {
                                    dtInventTableModule = dsITM.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[5]).Trim().Length > 38)
                            {
                                dsERP.ReadXml(srERP);
                            }
                            if (dsERP.Tables.Count > 0)
                            {
                                if (dsERP != null && dsERP.Tables[0].Rows.Count > 0)
                                {
                                    dtEcoResProduct = dsERP.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[6]).Trim().Length > 38)
                            {
                                dsERPC.ReadXml(srERPC);
                            }
                            if (dsERPC.Tables.Count > 0)
                            {
                                if (dsERPC != null && dsERPC.Tables[0].Rows.Count > 0)
                                {
                                    dtEcoResProductCategory = dsERPC.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[7]).Trim().Length > 38)
                            {
                                dsERPT.ReadXml(srERPT);
                            }
                            if (dsERPT.Tables.Count > 0)
                            {
                                if (dsERPT != null && dsERPT.Tables[0].Rows.Count > 0)
                                {
                                    dtEcoResProductTranslation = dsERPT.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[8]).Trim().Length > 38)
                            {
                                dsERTDG.ReadXml(srERTDG);
                            }
                            if (dsERTDG.Tables.Count > 0)
                            {
                                if (dsERTDG != null && dsERTDG.Tables[0].Rows.Count > 0)
                                {
                                    dtEcoResTrackingDimensionGroupItem = dsERTDG.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[9]).Trim().Length > 38)
                            {
                                dsERTDGP.ReadXml(srERTDGP);
                            }
                            if (dsERTDGP.Tables.Count > 0)
                            {
                                if (dsERTDGP != null && dsERTDGP.Tables[0].Rows.Count > 0)
                                {
                                    dtEcoResTrackingDimensionGroupProduct = dsERTDGP.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[10]).Trim().Length > 38)
                            {
                                dsID.ReadXml(srID);
                            }
                            if (dsID.Tables.Count > 0)
                            {
                                if (dsID != null && dsID.Tables[0].Rows.Count > 0)
                                {
                                    dtInventDim = dsID.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[11]).Trim().Length > 38)
                            {
                                dsIDC.ReadXml(srIDC);
                            }
                            if (dsIDC.Tables.Count > 0)
                            {
                                if (dsIDC != null && dsIDC.Tables[0].Rows.Count > 0)
                                {
                                    dtInventDimCombination = dsIDC.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[12]).Trim().Length > 38)
                            {
                                dsIIB.ReadXml(srIIB);
                            }
                            if (dsIIB.Tables.Count > 0)
                            {
                                if (dsIIB != null && dsIIB.Tables[0].Rows.Count > 0)
                                {
                                    dtInventItemBarcode = dsIIB.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[13]).Trim().Length > 38)
                            {
                                dsIIGI.ReadXml(srIIGI);
                            }
                            if (dsIIGI.Tables.Count > 0)
                            {
                                if (dsIIGI != null && dsIIGI.Tables[0].Rows.Count > 0)
                                {
                                    dtInventItemGroupItem = dsIIGI.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[14]).Trim().Length > 38)
                            {
                                dsRAE.ReadXml(srRAE);
                            }
                            if (dsRAE.Tables.Count > 0)
                            {
                                if (dsRAE != null && dsRAE.Tables[0].Rows.Count > 0)
                                {
                                    dtRetailAssortmentExploded = dsRAE.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[15]).Trim().Length > 38)
                            {
                                dsRIT.ReadXml(srRIT);
                            }
                            if (dsRIT.Tables.Count > 0)
                            {
                                if (dsRIT != null && dsRIT.Tables[0].Rows.Count > 0)
                                {
                                    dtRetailInventTable = dsRIT.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[16]).Trim().Length > 38)
                            {
                                dsUMC.ReadXml(srUMC);
                            }
                            if (dsUMC.Tables.Count > 0)
                            {
                                if (dsUMC != null && dsUMC.Tables[0].Rows.Count > 0)
                                {
                                    dtUnitOfMeasureConversion = dsUMC.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[17]).Trim().Length > 38)
                            {
                                dsConf.ReadXml(srConf);
                            }
                            if (dsConf.Tables.Count > 0)
                            {
                                if (dsConf != null && dsConf.Tables[0].Rows.Count > 0)
                                {
                                    dtEcoResProductMasterDimensionValue_Config = dsConf.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[18]).Trim().Length > 38)
                            {
                                dsSize.ReadXml(srSize);
                            }
                            if (dsSize.Tables.Count > 0)
                            {
                                if (dsSize != null && dsSize.Tables[0].Rows.Count > 0)
                                {
                                    dtEcoResProductMasterDimensionValue_size = dsSize.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[19]).Trim().Length > 38)
                            {
                                dsColor.ReadXml(srColor);
                            }
                            if (dsColor.Tables.Count > 0)
                            {
                                if (dsColor != null && dsColor.Tables[0].Rows.Count > 0)
                                {
                                    dtEcoResProductMasterDimensionValue_Color = dsColor.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[20]).Trim().Length > 38)
                            {
                                dsStyle.ReadXml(srStyle);
                            }
                            if (dsStyle.Tables.Count > 0)
                            {
                                if (dsStyle != null && dsStyle.Tables[0].Rows.Count > 0)
                                {
                                    dtEcoResProductMasterDimensionValue_style = dsStyle.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[21]).Trim().Length > 38)
                            {
                                dsSKUTable.ReadXml(srSKUTable);
                            }
                            if (dsSKUTable.Tables.Count > 0)
                            {
                                if (dsSKUTable != null && dsSKUTable.Tables[0].Rows.Count > 0)
                                {
                                    dtSKUTable = dsSKUTable.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[22]).Trim().Length > 38)
                            {
                                dsSKULine.ReadXml(srSKULine);
                            }
                            if (dsSKULine.Tables.Count > 0)
                            {
                                if (dsSKULine != null && dsSKULine.Tables[0].Rows.Count > 0)
                                {
                                    dtSKULine = dsSKULine.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[23]).Trim().Length > 38)
                            {
                                dsPDSCWItem.ReadXml(srPDSCWItem);
                            }
                            if (dsPDSCWItem.Tables.Count > 0)
                            {
                                if (dsPDSCWItem != null && dsPDSCWItem.Tables[0].Rows.Count > 0)
                                {
                                    dtPDSCWItem = dsPDSCWItem.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[24]).Trim().Length > 38)
                            {
                                dsPriceDiscTable.ReadXml(srPDT);
                            }
                            if (dsPriceDiscTable.Tables.Count > 0)
                            {
                                if (dsPriceDiscTable != null && dsPriceDiscTable.Tables[0].Rows.Count > 0)
                                {
                                    dtPriceDiscTable = dsPriceDiscTable.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[25]).Trim().Length > 38)
                            {
                                dsConfMaster.ReadXml(srConfMstr);
                            }
                            if (dsConfMaster.Tables.Count > 0)
                            {
                                if (dsConfMaster != null && dsConfMaster.Tables[0].Rows.Count > 0)
                                {
                                    dtConfigMaster = dsConfMaster.Tables[0];
                                }
                            }


                            if (Convert.ToString(containerArray[26]).Trim().Length > 38)
                            {
                                dsColorMaster.ReadXml(srColorMstr);
                            }
                            if (dsColorMaster.Tables.Count > 0)
                            {
                                if (dsColorMaster != null && dsColorMaster.Tables[0].Rows.Count > 0)
                                {
                                    dtColorMaster = dsColorMaster.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[27]).Trim().Length > 38)
                            {
                                dsSizeMaster.ReadXml(srSizeMstr);
                            }
                            if (dsSizeMaster.Tables.Count > 0)
                            {
                                if (dsSizeMaster != null && dsSizeMaster.Tables[0].Rows.Count > 0)
                                {
                                    dtSizeMaster = dsSizeMaster.Tables[0];
                                }
                            }


                            if (Convert.ToString(containerArray[28]).Trim().Length > 38)
                            {
                                dsStyleMaster.ReadXml(srStyleMstr);
                            }
                            if (dsStyleMaster.Tables.Count > 0)
                            {
                                if (dsStyleMaster != null && dsStyleMaster.Tables[0].Rows.Count > 0)
                                {
                                    dtStyleMaster = dsStyleMaster.Tables[0];
                                }
                            }


                            if (Convert.ToString(containerArray[29]).Trim().Length > 38)
                            {
                                dsEcoConf.ReadXml(srEcoConf);
                            }
                            if (dsEcoConf.Tables.Count > 0)
                            {
                                if (dsEcoConf != null && dsEcoConf.Tables[0].Rows.Count > 0)
                                {
                                    dtEcoConfig = dsEcoConf.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[30]).Trim().Length > 38)
                            {
                                dsEcoColor.ReadXml(srEcoColor);
                            }
                            if (dsEcoColor.Tables.Count > 0)
                            {
                                if (dsEcoColor != null && dsEcoColor.Tables[0].Rows.Count > 0)
                                {
                                    dtEcoColor = dsEcoColor.Tables[0];
                                }
                            }


                            if (Convert.ToString(containerArray[31]).Trim().Length > 38)
                            {
                                dsEcoSize.ReadXml(srEcoSize);
                            }
                            if (dsEcoSize.Tables.Count > 0)
                            {
                                if (dsEcoSize != null && dsEcoSize.Tables[0].Rows.Count > 0)
                                {
                                    dtEcoSize = dsEcoSize.Tables[0];
                                }
                            }


                            if (Convert.ToString(containerArray[32]).Trim().Length > 38)
                            {
                                dsEcoStyle.ReadXml(srEcoStyle);
                            }
                            if (dsEcoStyle.Tables.Count > 0)
                            {
                                if (dsEcoStyle != null && dsEcoStyle.Tables[0].Rows.Count > 0)
                                {
                                    dtEcoStyle = dsEcoStyle.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[33]).Trim().Length > 38)
                            {
                                dsHSN.ReadXml(srHSN);
                            }
                            if (dsHSN.Tables.Count > 0)
                            {
                                if (dsHSN != null && dsHSN.Tables[0].Rows.Count > 0)
                                {
                                    dtHSN = dsHSN.Tables[0];
                                }
                            }

                            if (Convert.ToString(containerArray[34]).Trim().Length > 38)
                            {
                                dsSAC.ReadXml(srSAC);
                            }
                            if (dsSAC.Tables.Count > 0)
                            {
                                if (dsSAC != null && dsSAC.Tables[0].Rows.Count > 0)
                                {
                                    dtSAC = dsSAC.Tables[0];
                                } 
                            }

                            #endregion

                            #region SaveItem
                            SaveItem(dtInventTable
                                    , dtInventTableModule
                                    , dtEcoResProduct
                                    , dtEcoResProductCategory
                                    , dtEcoResProductTranslation
                                    , dtEcoResTrackingDimensionGroupItem
                                    , dtEcoResTrackingDimensionGroupProduct
                                    , dtInventDim
                                    , dtInventDimCombination
                                    , dtInventItemBarcode
                                    , dtInventItemGroupItem
                                    , dtRetailAssortmentExploded
                                    , dtRetailInventTable
                                    , dtUnitOfMeasureConversion
                                    , dtEcoResProductMasterDimensionValue_Config
                                    , dtEcoResProductMasterDimensionValue_Color
                                    , dtEcoResProductMasterDimensionValue_size
                                    , dtEcoResProductMasterDimensionValue_style
                                    , dtSKUTable
                                    , dtSKULine
                                    , dtPDSCWItem
                                    , dtPriceDiscTable
                                    , dtConfigMaster
                                    , dtColorMaster
                                    , dtSizeMaster
                                    , dtStyleMaster
                                    , dtEcoConfig
                                    , dtEcoColor
                                    , dtEcoSize
                                    , dtEcoStyle
                                    , dtHSN
                                    , dtSAC);
                            #endregion
                        }
                        else
                        {
                            MessageBox.Show("No record found in HO");
                            txtItem.Focus();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Please contact to your admin for check real time service.");
                    txtItem.Focus();
                }
            }
            else
            {
                MessageBox.Show("Please enter a item.");
                txtItem.Focus();
            }
        }

        private void SaveItem(DataTable dtIT,
                              DataTable dtITM,
                              DataTable dtERP,
                              DataTable dtERPC,
                              DataTable dtERPT,
                              DataTable dtETDGI,
                              DataTable dtETGP,
                              DataTable dtID,
                              DataTable dtIDC,
                              DataTable dtIIB,
                              DataTable dtIIGI,
                              DataTable dtRAE,
                              DataTable dtRIT,
                              DataTable dtUMC,
                              DataTable dtConfig,
                              DataTable dtColor,
                              DataTable dtSize,
                              DataTable dtStyle,
                              DataTable dtSKUT,
                              DataTable dtSKUL,
                              DataTable dtPdsCwItem,
                              DataTable dtPDT,
                              DataTable dtConfigMstr,
                              DataTable dtColorMstr,
                              DataTable dtSizeMstr,
                              DataTable dtStyleMstr,
                              DataTable dtEcoConfig,
                              DataTable dtEcoColor,
                              DataTable dtEcoSize,
                              DataTable dtEcoStyle,
                              DataTable dtHSN,
                              DataTable dtSAC)
        {
            int iInventTable = 0;

            SqlTransaction transaction = null;

            if (dtIT != null && dtIT.Rows.Count > 0)
            {
                #region SQl qry for insert inventtable
                string commandText = " DELETE FROM INVENTTABLE " +
                                     " WHERE ItemId=@ItemId ; BEGIN" +
                                            " INSERT INTO [INVENTTABLE](ABCContributionMargin," +
                                            "ABCRevenue,ABCTieUp,ABCValue,AltItemId,Article_code" +
                                            ",BatchNumGroupId,BOMManualReceipt" +
                                            ",CostModel,Density,Depth,ExceptionCode_BR" +
                                            ",grossDepth,grossHeight,grossWidth,Height" +
                                            ",InventProductType_BR" +
                                            ",ItemBuyerGroupId,ItemDimCostPrice,ItemId" +
                                            ",ItemIdParent,ItemType,MetalType,MFG_Code" +
                                            ",MRP,NameAlias,NetWeight" +
                                            ",PBAItemConfigurable," +
                                            "PrimaryVendorId,ProdGroupId,ProdPoolId," +
                                            "Product,projCategoryId,PropertyId" +
                                            ",PurchModel,RecId,ReqGroupId,SalesContributionRatio" +
                                            ",SalesModel,SalesPercentMarkup,SalesPriceModelBasic" +
                                            ",SerialNumGroupId,SetOf,sortCode" +
                                            ",StdQty ,TaraWeight,TaxationOrigin_BR" +
                                            ",TaxFiscalClassification_BR,TaxServiceCode_BR,UnitVolume" +
                                            ",UseAltItemId,Width,WMSPickingQtyTime,DATAAREAID," +
                                            " RETAIL," +
                                            " COMPLEXITY_CODE," +
                                            " GIFT," +
                                            " CustomerStn," +
                                            " [Sample]," +
                                            " PRODUCTTYPECODE," +
                                            " COLLECTIONCODE," +
                                            " OWNOG," +
                                            " OTHEROG," +
                                            " OWNDMD," +
                                            " OTHERDMD," +
                                            " CRWMetalRateCalcType," +
                                            " CRWSETPRODUCTS," +
                                            " BaseDesignCode," +
                                            " NOPRICEREQUIRED," +
                                            " SALESRETURNOG," +
                                            " NOEXTRAMKADDITION,NOCOSTVALIDATERETAIL,"+
                                            " HSNCODETABLE_IN,ServiceAccountingCodeTable_IN,RepairItem)" +
                                            " VALUES(@ABCContributionMargin,@ABCRevenue,@ABCTieUp" +
                                            ",@ABCValue,@AltItemId,@Article_code" +
                                            ",@BatchNumGroupId,@BOMManualReceipt" +
                                            ",@CostModel,@Density,@Depth" +
                                            ",@ExceptionCode_BR,@grossDepth,@grossHeight" +
                                            ",@grossWidth,@Height" +
                                            ",@InventProductType_BR,@ItemBuyerGroupId" +
                                            ",@ItemDimCostPrice,@ItemId,@ItemIdParent" +
                                            ",@ItemType,@MetalType,@MFG_Code" +
                                            ",@MRP,@NameAlias,@NetWeight" +
                                            ",@PBAItemConfigurable,@PrimaryVendorId,@ProdGroupId" +
                                            ",@ProdPoolId,@Product,@projCategoryId" +
                                            ",@PropertyId,@PurchModel,@RecId" +
                                            ",@ReqGroupId,@SalesContributionRatio,@SalesModel" +
                                            ",@SalesPercentMarkup,@SalesPriceModelBasic,@SerialNumGroupId" +
                                            ",@SetOf,@sortCode,@StdQty" +
                                            ",@TaraWeight,@TaxationOrigin_BR,@TaxFiscalClassification_BR" +
                                            ",@TaxServiceCode_BR,@UnitVolume,@UseAltItemId" +
                                            ",@Width,@WMSPickingQtyTime,@DATAAREAID," +
                                            " @RETAIL," +
                                            " @COMPLEXITY_CODE," +
                                            " @GIFT," +
                                            " @CustomerStn," +
                                            " @Sample," +
                                            " @PRODUCTTYPECODE," +
                                            " @COLLECTIONCODE," +
                                            " @OWNOG," +
                                            " @OTHEROG," +
                                            " @OWNDMD," +
                                            " @OTHERDMD," +
                                            " @CRWMetalRateCalcType," +
                                            " @CRWSETPRODUCTS," +
                                            " @BaseDesignCode," +
                                            " @NOPRICEREQUIRED," +
                                            " @SALESRETURNOG," +
                                            " @NOEXTRAMKADDITION,@NOCOSTVALIDATERETAIL,"+
                                            " @HSNCODETABLE_IN,@ServiceAccountingCodeTable_IN,@RepairItem) END";

                #endregion


                SqlConnection connection = new SqlConnection();
                try
                {
                    if (application != null)
                        connection = application.Settings.Database.Connection;
                    else
                        connection = ApplicationSettings.Database.LocalConnection;


                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    transaction = connection.BeginTransaction();

                    SqlCommand command = new SqlCommand(commandText, connection, transaction);
                    command.Parameters.Clear();

                    #region data for inventtable
                    string sItemId = Convert.ToString(dtIT.Rows[0]["ItemId"]);
                    command.Parameters.Add("@ABCContributionMargin", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["ABCContributionMargin"]);
                    command.Parameters.Add("@ABCRevenue", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["ABCRevenue"]);
                    command.Parameters.Add("@ABCTieUp", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["ABCTieUp"]);
                    command.Parameters.Add("@ABCValue", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["ABCValue"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["ALTITEMID"])))
                        command.Parameters.Add("@ALTITEMID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@ALTITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIT.Rows[0]["ALTITEMID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["Article_code"])))
                        command.Parameters.Add("@Article_code", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@Article_code", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIT.Rows[0]["Article_code"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["BatchNumGroupId"])))
                        command.Parameters.Add("@BatchNumGroupId", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@BatchNumGroupId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIT.Rows[0]["BatchNumGroupId"]);

                    command.Parameters.Add("@BOMManualReceipt", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["BOMManualReceipt"]);


                    command.Parameters.Add("@CostModel", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["CostModel"]);
                    command.Parameters.Add("@Density", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIT.Rows[0]["Density"]);
                    command.Parameters.Add("@Depth", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIT.Rows[0]["Depth"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["ExceptionCode_BR"])))
                        command.Parameters.Add("@ExceptionCode_BR", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@ExceptionCode_BR", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIT.Rows[0]["ExceptionCode_BR"]);

                    command.Parameters.Add("@grossDepth", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIT.Rows[0]["grossDepth"]);
                    command.Parameters.Add("@grossHeight", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIT.Rows[0]["grossHeight"]);
                    command.Parameters.Add("@grossWidth", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIT.Rows[0]["grossWidth"]);
                    command.Parameters.Add("@Height", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIT.Rows[0]["Height"]);
                    //command.Parameters.Add("@Ingredient", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["Ingredient"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["InventProductType_BR"])))
                        command.Parameters.Add("@InventProductType_BR", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@InventProductType_BR", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIT.Rows[0]["InventProductType_BR"]);
                    // command.Parameters.Add("@IsBooklet", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["IsBooklet"]);//

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["ItemBuyerGroupId"])))
                        command.Parameters.Add("@ItemBuyerGroupId", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@ItemBuyerGroupId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIT.Rows[0]["ItemBuyerGroupId"]);

                    command.Parameters.Add("@ItemDimCostPrice", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["ItemDimCostPrice"]);//

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["ITEMID"])))
                        command.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIT.Rows[0]["ITEMID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["ItemIdParent"])))
                        command.Parameters.Add("@ItemIdParent", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@ItemIdParent", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIT.Rows[0]["ItemIdParent"]);

                    command.Parameters.Add("@ItemType", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["ItemType"]);
                    command.Parameters.Add("@MetalType", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["MetalType"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["MFG_Code"])))
                        command.Parameters.Add("@MFG_Code", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@MFG_Code", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIT.Rows[0]["MFG_Code"]);

                    command.Parameters.Add("@MRP", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["MRP"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["NameAlias"])))
                        command.Parameters.Add("@NameAlias", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@NameAlias", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIT.Rows[0]["NameAlias"]);

                    command.Parameters.Add("@NetWeight", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIT.Rows[0]["NetWeight"]);

                    //command.Parameters.Add("@OWN", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["OWN"]);
                    //if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["PBAInventItemGroupId"])))
                    //    command.Parameters.Add("@PBAInventItemGroupId", SqlDbType.NVarChar, 10).Value = "";
                    //else
                    //    command.Parameters.Add("@PBAInventItemGroupId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIT.Rows[0]["PBAInventItemGroupId"]);

                    command.Parameters.Add("@PBAItemConfigurable", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["PBAItemConfigurable"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["PrimaryVendorId"])))
                        command.Parameters.Add("@PrimaryVendorId", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@PrimaryVendorId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIT.Rows[0]["PrimaryVendorId"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["ProdGroupId"])))
                        command.Parameters.Add("@ProdGroupId", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@ProdGroupId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIT.Rows[0]["ProdGroupId"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["ProdPoolId"])))
                        command.Parameters.Add("@ProdPoolId", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@ProdPoolId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIT.Rows[0]["ProdPoolId"]);

                    command.Parameters.Add("@Product", SqlDbType.BigInt).Value = Convert.ToInt64(dtIT.Rows[0]["Product"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["projCategoryId"])))
                        command.Parameters.Add("@projCategoryId", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@projCategoryId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIT.Rows[0]["projCategoryId"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["PropertyId"])))
                        command.Parameters.Add("@PropertyId", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@PropertyId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIT.Rows[0]["PropertyId"]);

                    command.Parameters.Add("@PurchModel", SqlDbType.BigInt).Value = Convert.ToInt64(dtIT.Rows[0]["PurchModel"]);

                    command.Parameters.Add("@RecId", SqlDbType.BigInt).Value = Convert.ToInt64(dtIT.Rows[0]["RecId"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["ReqGroupId"])))
                        command.Parameters.Add("@ReqGroupId", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@ReqGroupId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIT.Rows[0]["ReqGroupId"]);

                    command.Parameters.Add("@SalesContributionRatio", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIT.Rows[0]["SalesContributionRatio"]);
                    command.Parameters.Add("@SalesModel", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["SalesModel"]);

                    command.Parameters.Add("@SalesPercentMarkup", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIT.Rows[0]["SalesPercentMarkup"]);
                    command.Parameters.Add("@SalesPriceModelBasic", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["SalesPriceModelBasic"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["SerialNumGroupId"])))
                        command.Parameters.Add("@SerialNumGroupId", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@SerialNumGroupId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIT.Rows[0]["SerialNumGroupId"]);

                    command.Parameters.Add("@SetOf", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["SetOf"]);
                    command.Parameters.Add("@sortCode", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["sortCode"]);
                    command.Parameters.Add("@StdQty", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIT.Rows[0]["StdQty"]);
                    command.Parameters.Add("@TaraWeight", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIT.Rows[0]["TaraWeight"]);

                    command.Parameters.Add("@TaxationOrigin_BR", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["TaxationOrigin_BR"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["TaxFiscalClassification_BR"])))
                        command.Parameters.Add("@TaxFiscalClassification_BR", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@TaxFiscalClassification_BR", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIT.Rows[0]["TaxFiscalClassification_BR"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["TaxServiceCode_BR"])))
                        command.Parameters.Add("@TaxServiceCode_BR", SqlDbType.NVarChar, 10).Value = "";
                    else
                        command.Parameters.Add("@TaxServiceCode_BR", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIT.Rows[0]["TaxServiceCode_BR"]);

                    command.Parameters.Add("@UnitVolume", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIT.Rows[0]["UnitVolume"]);
                    command.Parameters.Add("@UseAltItemId", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["UseAltItemId"]);
                    command.Parameters.Add("@Width", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIT.Rows[0]["Width"]);
                    command.Parameters.Add("@WMSPickingQtyTime", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["WMSPickingQtyTime"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["DATAAREAID"])))
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = "";
                    else
                        command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtIT.Rows[0]["DATAAREAID"]);


                    command.Parameters.Add("@RETAIL", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["RETAIL"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["COMPLEXITY_CODE"])))
                        command.Parameters.Add("@COMPLEXITY_CODE", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@COMPLEXITY_CODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIT.Rows[0]["COMPLEXITY_CODE"]);
                    command.Parameters.Add("@GIFT", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["GIFT"]);
                    command.Parameters.Add("@CustomerStn", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["CustomerStn"]);
                    command.Parameters.Add("@Sample", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["Sample"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["PRODUCTTYPECODE"])))
                        command.Parameters.Add("@PRODUCTTYPECODE", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@PRODUCTTYPECODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIT.Rows[0]["PRODUCTTYPECODE"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["COLLECTIONCODE"])))
                        command.Parameters.Add("@COLLECTIONCODE", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@COLLECTIONCODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIT.Rows[0]["COLLECTIONCODE"]);

                    command.Parameters.Add("@OWNOG", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["OWNOG"]);
                    command.Parameters.Add("@OTHEROG", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["OTHEROG"]);
                    command.Parameters.Add("@OWNDMD", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["OWNDMD"]);
                    command.Parameters.Add("@OTHERDMD", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["OTHERDMD"]);
                    command.Parameters.Add("@CRWMetalRateCalcType", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["CRWMetalRateCalcType"]);
                    command.Parameters.Add("@CRWSETPRODUCTS", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["CRWSETPRODUCTS"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIT.Rows[0]["BaseDesignCode"])))
                        command.Parameters.Add("@BaseDesignCode", SqlDbType.NVarChar, 20).Value = "";
                    else
                        command.Parameters.Add("@BaseDesignCode", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIT.Rows[0]["BaseDesignCode"]);

                    command.Parameters.Add("@NOPRICEREQUIRED", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["NOPRICEREQUIRED"]);
                    command.Parameters.Add("@SALESRETURNOG", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["SALESRETURNOG"]);

                    command.Parameters.Add("@NOEXTRAMKADDITION", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["CRWNoExtraMakingAdition"]);
                    command.Parameters.Add("@NOCOSTVALIDATERETAIL", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["CRWNoCostPriceRetail"]);
                   
                    command.Parameters.Add("@HSNCODETABLE_IN", SqlDbType.BigInt).Value = Convert.ToInt64(dtIT.Rows[0]["HSNCODETABLE_IN"]);//added on 011018
                    command.Parameters.Add("@ServiceAccountingCodeTable_IN", SqlDbType.BigInt).Value = Convert.ToInt64(dtIT.Rows[0]["ServiceAccountingCodeTable_IN"]);//added on 011018
                    command.Parameters.Add("@RepairItem", SqlDbType.Int).Value = Convert.ToInt16(dtIT.Rows[0]["CRWRepairItem"]);
                    //CRWRepairItem
                    #endregion

                    command.CommandTimeout = 0;
                    iInventTable = command.ExecuteNonQuery();
                    iAnyTableEffected = iInventTable;

                    SaveInventTableModule(dtITM, transaction, connection);
                    SaveEcoResProduct(dtERP, transaction, connection);
                    SaveEcoResProductCategory(dtERPC, transaction, connection);
                    SaveEcoResProductTranslation(dtERPT, transaction, connection);
                    SaveEcoResTrackingDimensionGroupItem(dtETDGI, transaction, connection);
                    SaveEcoResTrackingDimensionGroupProduct(dtETGP, transaction, connection);
                    SaveInventDim(dtID, transaction, connection);
                    SaveInventDimCombination(dtIDC, transaction, connection);
                    SaveInventItemBarcode(dtIIB, transaction, connection);
                    SaveInventItemGroupItem(dtIIGI, transaction, connection);
                    SaveRetailAssortmentExploded(dtRAE, transaction, connection);
                    SaveRetailInventTable(dtRIT, transaction, connection);
                    SaveUnitOfMeasureConversion(dtUMC, transaction, connection);
                    SaveECORESPRODUCTMASTERDIMENSIONVALUE(dtConfig, transaction, connection);
                    SaveECORESPRODUCTMASTERDIMENSIONVALUE(dtSize, transaction, connection);
                    SaveECORESPRODUCTMASTERDIMENSIONVALUE(dtStyle, transaction, connection);
                    SaveECORESPRODUCTMASTERDIMENSIONVALUE(dtColor, transaction, connection);
                    SaveSKUTable(dtSKUT, transaction, connection);
                    SaveSKULine(dtSKUL, transaction, connection);
                    SavePDSCWITEM(dtPdsCwItem, transaction, connection);
                    SavePriceDiscTable(dtPDT, transaction, connection);

                    SaveECORESPRODUCTMASTERCONFIG(dtConfigMstr, transaction, connection);
                    SaveECORESPRODUCTMASTERCOLOR(dtColorMstr, transaction, connection);
                    SaveECORESPRODUCTMASTERSIZE(dtSizeMstr, transaction, connection);
                    SaveECORESPRODUCTMASTERSTYLE(dtStyleMstr, transaction, connection);

                    SaveECORESCONFIG(dtEcoConfig, transaction, connection);
                    SaveECORESCOLOR(dtEcoColor, transaction, connection);
                    SaveECORESSIZE(dtEcoSize, transaction, connection);
                    SaveECORESSTYLE(dtEcoStyle, transaction, connection);
                    SaveHSNCODETABLE_IN(dtHSN, transaction, connection);//added on 011018
                    SaveSACCODETABLE_IN(dtSAC, transaction, connection);//added on 011018

                    transaction.Commit();
                    command.Dispose();
                    transaction.Dispose();

                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Item fetched successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        this.Close();
                    }
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    transaction.Dispose();

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

                }
            }
        }

        private static void SaveInventTableModule(DataTable dtITM, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtITM != null && dtITM.Rows.Count > 0)
            {
                string commandITM = " IF NOT EXISTS(SELECT TOP 1 ItemId FROM INVENTTABLEMODULE " +
                                            " WHERE ItemId=@ItemId ) BEGIN" +
                                            " INSERT INTO [INVENTTABLEMODULE](AllocateMarkup,EndDisc,ItemId" +
                                            ",LineDisc,Markup,MarkupGroupId" +
                                            ",MaximumRetailPrice_IN,ModuleType,MultiLineDisc" +
                                            ",Price,PriceQty,PriceUnit" +
                                            ",RecId,TaxItemGroupId,UnitId,DataAreaId)" +
                                            " VALUES(@AllocateMarkup,@EndDisc,@ItemId" +
                                            ",@LineDisc,@Markup,@MarkupGroupId" +
                                            ",@MaximumRetailPrice_IN,@ModuleType,@MultiLineDisc" +
                                            ",@Price,@PriceQty,@PriceUnit" +
                                            ",@RecId,@TaxItemGroupId,@UnitId,@DataAreaId) END";
                for (int ItemCount = 0; ItemCount < dtITM.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdITM = new SqlCommand(commandITM, connection, transaction);

                    cmdITM.Parameters.Add("@AllocateMarkup", SqlDbType.Int).Value = Convert.ToInt16(dtITM.Rows[ItemCount]["AllocateMarkup"]);
                    cmdITM.Parameters.Add("@EndDisc", SqlDbType.Int).Value = Convert.ToInt16(dtITM.Rows[ItemCount]["EndDisc"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtITM.Rows[ItemCount]["ItemId"])))
                        cmdITM.Parameters.Add("@ItemId", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdITM.Parameters.Add("@ItemId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtITM.Rows[ItemCount]["ItemId"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtITM.Rows[ItemCount]["LineDisc"])))
                        cmdITM.Parameters.Add("@LineDisc", SqlDbType.Decimal).Value = 0;
                    else
                        cmdITM.Parameters.Add("@LineDisc", SqlDbType.Decimal).Value = Convert.ToDecimal(dtITM.Rows[ItemCount]["LineDisc"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtITM.Rows[ItemCount]["Markup"])))
                        cmdITM.Parameters.Add("@Markup", SqlDbType.Decimal).Value = 0;
                    else
                        cmdITM.Parameters.Add("@Markup", SqlDbType.Decimal).Value = Convert.ToDecimal(dtITM.Rows[ItemCount]["Markup"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtITM.Rows[ItemCount]["MarkupGroupId"])))
                        cmdITM.Parameters.Add("@MarkupGroupId", SqlDbType.NVarChar, 10).Value = "";
                    else
                        cmdITM.Parameters.Add("@MarkupGroupId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtITM.Rows[ItemCount]["MarkupGroupId"]);

                    cmdITM.Parameters.Add("@MaximumRetailPrice_IN", SqlDbType.Decimal).Value = Convert.ToDecimal(dtITM.Rows[ItemCount]["MaximumRetailPrice_IN"]);

                    cmdITM.Parameters.Add("@ModuleType", SqlDbType.Int).Value = Convert.ToInt16(dtITM.Rows[ItemCount]["ModuleType"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtITM.Rows[ItemCount]["MultiLineDisc"])))
                        cmdITM.Parameters.Add("@MultiLineDisc", SqlDbType.NVarChar, 10).Value = "";
                    else
                        cmdITM.Parameters.Add("@MultiLineDisc", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtITM.Rows[ItemCount]["MultiLineDisc"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtITM.Rows[ItemCount]["Price"])))
                        cmdITM.Parameters.Add("@Price", SqlDbType.Decimal).Value = 0;
                    else
                        cmdITM.Parameters.Add("@Price", SqlDbType.Decimal).Value = Convert.ToDecimal(dtITM.Rows[ItemCount]["Price"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtITM.Rows[ItemCount]["PriceQty"])))
                        cmdITM.Parameters.Add("@PriceQty", SqlDbType.Decimal).Value = 0;
                    else
                        cmdITM.Parameters.Add("@PriceQty", SqlDbType.Decimal).Value = Convert.ToDecimal(dtITM.Rows[ItemCount]["PriceQty"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtITM.Rows[ItemCount]["PriceUnit"])))
                        cmdITM.Parameters.Add("@PriceUnit", SqlDbType.Decimal).Value = 0;
                    else
                        cmdITM.Parameters.Add("@PriceUnit", SqlDbType.Decimal).Value = Convert.ToDecimal(dtITM.Rows[ItemCount]["PriceUnit"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtITM.Rows[ItemCount]["RECID"])))
                        cmdITM.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdITM.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtITM.Rows[ItemCount]["RECID"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtITM.Rows[ItemCount]["TaxItemGroupId"])))
                        cmdITM.Parameters.Add("@TaxItemGroupId", SqlDbType.NVarChar, 10).Value = "";
                    else
                        cmdITM.Parameters.Add("@TaxItemGroupId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtITM.Rows[ItemCount]["TaxItemGroupId"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtITM.Rows[ItemCount]["UnitId"])))
                        cmdITM.Parameters.Add("@UnitId", SqlDbType.NVarChar, 10).Value = "";
                    else
                        cmdITM.Parameters.Add("@UnitId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtITM.Rows[ItemCount]["UnitId"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtITM.Rows[ItemCount]["DataAreaId"])))
                        cmdITM.Parameters.Add("@DataAreaId", SqlDbType.NVarChar, 4).Value = "";
                    else
                        cmdITM.Parameters.Add("@DataAreaId", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtITM.Rows[ItemCount]["DataAreaId"]);

                    cmdITM.CommandTimeout = 0;
                    cmdITM.ExecuteNonQuery();
                    cmdITM.Dispose();
                }
            }
        }

        private static void SaveEcoResProduct(DataTable dtERP, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtERP != null && dtERP.Rows.Count > 0)
            {
                string commandERP = " IF NOT EXISTS(SELECT TOP 1 RECID FROM EcoResProduct " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [EcoResProduct](DISPLAYPRODUCTNUMBER," +
                                            " INSTANCERELATIONTYPE," +
                                            " PRODUCTTYPE," +
                                            " RECID," +
                                            " RECVERSION," +
                                            " RELATIONTYPE," +
                                            " SEARCHNAME)" +
                                            " VALUES(@DISPLAYPRODUCTNUMBER," +
                                            " @INSTANCERELATIONTYPE," +
                                            " @PRODUCTTYPE," +
                                            " @RECID," +
                                            " @RECVERSION," +
                                            " @RELATIONTYPE," +
                                            " @SEARCHNAME) END";
                for (int ItemCount = 0; ItemCount < dtERP.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdERP = new SqlCommand(commandERP, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtERP.Rows[ItemCount]["DISPLAYPRODUCTNUMBER"])))
                        cmdERP.Parameters.Add("@DISPLAYPRODUCTNUMBER", SqlDbType.NVarChar, 70).Value = "";
                    else
                        cmdERP.Parameters.Add("@DISPLAYPRODUCTNUMBER", SqlDbType.NVarChar, 70).Value = Convert.ToString(dtERP.Rows[ItemCount]["DISPLAYPRODUCTNUMBER"]);

                    cmdERP.Parameters.Add("@INSTANCERELATIONTYPE", SqlDbType.BigInt).Value = Convert.ToInt64(dtERP.Rows[ItemCount]["INSTANCERELATIONTYPE"]);

                    cmdERP.Parameters.Add("@PRODUCTTYPE", SqlDbType.Int).Value = Convert.ToInt16(dtERP.Rows[ItemCount]["PRODUCTTYPE"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtERP.Rows[ItemCount]["RECID"])))
                        cmdERP.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdERP.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtERP.Rows[ItemCount]["RECID"]);

                    cmdERP.Parameters.Add("@RECVERSION", SqlDbType.BigInt).Value = Convert.ToInt64(dtERP.Rows[ItemCount]["RECVERSION"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtERP.Rows[ItemCount]["RELATIONTYPE"])))
                        cmdERP.Parameters.Add("@RELATIONTYPE", SqlDbType.BigInt).Value = 0;
                    else
                        cmdERP.Parameters.Add("@RELATIONTYPE", SqlDbType.BigInt).Value = Convert.ToInt64(dtERP.Rows[ItemCount]["RELATIONTYPE"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtERP.Rows[ItemCount]["SEARCHNAME"])))
                        cmdERP.Parameters.Add("@SEARCHNAME", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdERP.Parameters.Add("@SEARCHNAME", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtERP.Rows[ItemCount]["SEARCHNAME"]);


                    cmdERP.CommandTimeout = 0;
                    cmdERP.ExecuteNonQuery();
                    cmdERP.Dispose();
                }
            }
        }

        private static void SaveEcoResProductCategory(DataTable dtERPC, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtERPC != null && dtERPC.Rows.Count > 0)
            {
                string commandERPC = " IF NOT EXISTS(SELECT TOP 1 RECID FROM EcoResProductCategory " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [EcoResProductCategory](CATEGORY," +
                                            " CATEGORYHIERARCHY," +
                                            " RECID," +
                                            " RECVERSION," +
                                            " PRODUCT)" +
                                            " VALUES(@CATEGORY," +
                                            " @CATEGORYHIERARCHY," +
                                            " @RECID," +
                                            " @RECVERSION," +
                                            " @PRODUCT) END";
                for (int ItemCount = 0; ItemCount < dtERPC.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdERPC = new SqlCommand(commandERPC, connection, transaction);

                    cmdERPC.Parameters.Add("@CATEGORY", SqlDbType.BigInt).Value = Convert.ToInt64(dtERPC.Rows[ItemCount]["CATEGORY"]);
                    cmdERPC.Parameters.Add("@CATEGORYHIERARCHY", SqlDbType.BigInt).Value = Convert.ToInt64(dtERPC.Rows[ItemCount]["CATEGORYHIERARCHY"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtERPC.Rows[ItemCount]["RECID"])))
                        cmdERPC.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdERPC.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtERPC.Rows[ItemCount]["RECID"]);
                    cmdERPC.Parameters.Add("@RECVERSION", SqlDbType.BigInt).Value = Convert.ToInt64(dtERPC.Rows[ItemCount]["RECVERSION"]);
                    cmdERPC.Parameters.Add("@PRODUCT", SqlDbType.BigInt).Value = Convert.ToInt64(dtERPC.Rows[ItemCount]["PRODUCT"]);

                    cmdERPC.CommandTimeout = 0;
                    cmdERPC.ExecuteNonQuery();
                    cmdERPC.Dispose();
                }
            }
        }

        private static void SaveEcoResProductTranslation(DataTable dtERPT, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtERPT != null && dtERPT.Rows.Count > 0)
            {
                string commandERPT = " IF NOT EXISTS(SELECT TOP 1 RECID FROM EcoResProductTranslation " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [EcoResProductTranslation](DESCRIPTION," +
                                            " LANGUAGEID," +
                                            " NAME," +
                                            " RECID," +
                                            " RECVERSION," +
                                            " PRODUCT)" +
                                            " VALUES(@DESCRIPTION," +
                                            " @LANGUAGEID," +
                                            " @NAME," +
                                            " @RECID," +
                                            " @RECVERSION," +
                                            " @PRODUCT) END";

                for (int ItemCount = 0; ItemCount < dtERPT.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdERPT = new SqlCommand(commandERPT, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtERPT.Rows[ItemCount]["DESCRIPTION"])))
                        cmdERPT.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar, 1000).Value = "";
                    else
                        cmdERPT.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar, 1000).Value = Convert.ToString(dtERPT.Rows[ItemCount]["DESCRIPTION"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtERPT.Rows[ItemCount]["LANGUAGEID"])))
                        cmdERPT.Parameters.Add("@LANGUAGEID", SqlDbType.NVarChar, 7).Value = "";
                    else
                        cmdERPT.Parameters.Add("@LANGUAGEID", SqlDbType.NVarChar, 7).Value = Convert.ToString(dtERPT.Rows[ItemCount]["LANGUAGEID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtERPT.Rows[ItemCount]["NAME"])))
                        cmdERPT.Parameters.Add("@NAME", SqlDbType.NVarChar, 60).Value = "";
                    else
                        cmdERPT.Parameters.Add("@NAME", SqlDbType.NVarChar, 60).Value = Convert.ToString(dtERPT.Rows[ItemCount]["NAME"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtERPT.Rows[ItemCount]["RECID"])))
                        cmdERPT.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdERPT.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtERPT.Rows[ItemCount]["RECID"]);

                    cmdERPT.Parameters.Add("@RECVERSION", SqlDbType.BigInt).Value = Convert.ToInt64(dtERPT.Rows[ItemCount]["RECVERSION"]);
                    cmdERPT.Parameters.Add("@PRODUCT", SqlDbType.BigInt).Value = Convert.ToInt64(dtERPT.Rows[ItemCount]["PRODUCT"]);

                    cmdERPT.CommandTimeout = 0;
                    cmdERPT.ExecuteNonQuery();
                    cmdERPT.Dispose();
                }
            }
        }

        private static void SaveEcoResTrackingDimensionGroupItem(DataTable dtERTDGI, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtERTDGI != null && dtERTDGI.Rows.Count > 0)
            {
                string commandERTDGI = " IF NOT EXISTS(SELECT TOP 1 RECID FROM EcoResTrackingDimensionGroupItem " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [EcoResTrackingDimensionGroupItem](TRACKINGDIMENSIONGROUP,ITEMID,ITEMDATAAREAID,RECID)" +
                                            " VALUES(@TRACKINGDIMENSIONGROUP,@ITEMID,@ITEMDATAAREAID,@RECID) END";
                for (int ItemCount = 0; ItemCount < dtERTDGI.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdERTDGI = new SqlCommand(commandERTDGI, connection, transaction);
                    if (string.IsNullOrEmpty(Convert.ToString(dtERTDGI.Rows[ItemCount]["TRACKINGDIMENSIONGROUP"])))
                        cmdERTDGI.Parameters.Add("@TRACKINGDIMENSIONGROUP", SqlDbType.BigInt).Value = 0;
                    else
                        cmdERTDGI.Parameters.Add("@TRACKINGDIMENSIONGROUP", SqlDbType.BigInt).Value = Convert.ToInt64(dtERTDGI.Rows[ItemCount]["TRACKINGDIMENSIONGROUP"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtERTDGI.Rows[ItemCount]["ItemId"])))
                        cmdERTDGI.Parameters.Add("@ItemId", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdERTDGI.Parameters.Add("@ItemId", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtERTDGI.Rows[ItemCount]["ItemId"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtERTDGI.Rows[ItemCount]["ItemDataAreaId"])))
                        cmdERTDGI.Parameters.Add("@ItemDataAreaId", SqlDbType.NVarChar, 4).Value = "";
                    else
                        cmdERTDGI.Parameters.Add("@ItemDataAreaId", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtERTDGI.Rows[ItemCount]["ItemDataAreaId"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtERTDGI.Rows[ItemCount]["RECID"])))
                        cmdERTDGI.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdERTDGI.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtERTDGI.Rows[ItemCount]["RECID"]);

                    cmdERTDGI.CommandTimeout = 0;
                    cmdERTDGI.ExecuteNonQuery();
                    cmdERTDGI.Dispose();
                }
            }
        }

        private static void SaveEcoResTrackingDimensionGroupProduct(DataTable dtERTDGP, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtERTDGP != null && dtERTDGP.Rows.Count > 0)
            {
                string commandERTDGP = " IF NOT EXISTS(SELECT TOP 1 RECID FROM EcoResTrackingDimensionGroupProduct " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [EcoResTrackingDimensionGroupProduct](PRODUCT,TRACKINGDIMENSIONGROUP,RECID )" +
                                            " VALUES(@PRODUCT,@TRACKINGDIMENSIONGROUP,@RECID) END";
                for (int ItemCount = 0; ItemCount < dtERTDGP.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdERTDGP = new SqlCommand(commandERTDGP, connection, transaction);


                    if (string.IsNullOrEmpty(Convert.ToString(dtERTDGP.Rows[ItemCount]["PRODUCT"])))
                        cmdERTDGP.Parameters.Add("@PRODUCT", SqlDbType.BigInt).Value = 0;
                    else
                        cmdERTDGP.Parameters.Add("@PRODUCT", SqlDbType.BigInt).Value = Convert.ToInt64(dtERTDGP.Rows[ItemCount]["PRODUCT"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtERTDGP.Rows[ItemCount]["TRACKINGDIMENSIONGROUP"])))
                        cmdERTDGP.Parameters.Add("@TRACKINGDIMENSIONGROUP", SqlDbType.BigInt).Value = 0;
                    else
                        cmdERTDGP.Parameters.Add("@TRACKINGDIMENSIONGROUP", SqlDbType.BigInt).Value = Convert.ToInt64(dtERTDGP.Rows[ItemCount]["TRACKINGDIMENSIONGROUP"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtERTDGP.Rows[ItemCount]["RECID"])))
                        cmdERTDGP.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdERTDGP.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtERTDGP.Rows[ItemCount]["RECID"]);

                    cmdERTDGP.CommandTimeout = 0;
                    cmdERTDGP.ExecuteNonQuery();
                    cmdERTDGP.Dispose();
                }
            }
        }

        private static void SaveInventDim(DataTable dtID, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtID != null && dtID.Rows.Count > 0)
            {
                string commandID = " IF NOT EXISTS(SELECT TOP 1 INVENTDIMID FROM InventDim " +
                                            " WHERE INVENTDIMID=@INVENTDIMID and DATAAREAID=@DATAAREAID ) BEGIN" +
                                            " INSERT INTO [InventDim](INVENTDIMID" +
                                                                    ",INVENTBATCHID" +
                                                                    ",WMSLOCATIONID" +
                    //  ",WMSPALLETID"+
                                                                    ",INVENTSERIALID" +
                                                                    ",INVENTLOCATIONID" +
                                                                    ",CONFIGID" +
                                                                    ",INVENTSIZEID" +
                                                                    ",INVENTCOLORID" +
                                                                    ",INVENTSTYLEID" +
                    // ",INVENTSITEID"+
                                                                    ",DATAAREAID" +
                    // ",RECVERSION"+
                                                                    ",RECID)" +
                                            " VALUES(@INVENTDIMID" +
                                                ",@INVENTBATCHID" +
                                                ",@WMSLOCATIONID" +
                    // ",@WMSPALLETID" +
                                                ",@INVENTSERIALID" +
                                                ",@INVENTLOCATIONID" +
                                                ",@CONFIGID" +
                                                ",@INVENTSIZEID" +
                                                ",@INVENTCOLORID" +
                                                ",@INVENTSTYLEID" +
                    //",@INVENTSITEID" +
                                                ",@DATAAREAID" +
                    // ",@RECVERSION" +
                                                ",@RECID) END";

                for (int ItemCount = 0; ItemCount < dtID.Rows.Count; ItemCount++)
                {

                    SqlCommand cmdID = new SqlCommand(commandID, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtID.Rows[ItemCount]["INVENTDIMID"])))
                        cmdID.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdID.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtID.Rows[ItemCount]["INVENTDIMID"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtID.Rows[ItemCount]["INVENTBATCHID"])))
                        cmdID.Parameters.Add("@INVENTBATCHID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdID.Parameters.Add("@INVENTBATCHID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtID.Rows[ItemCount]["INVENTBATCHID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtID.Rows[ItemCount]["WMSLOCATIONID"])))
                        cmdID.Parameters.Add("@WMSLOCATIONID", SqlDbType.NVarChar, 10).Value = "";
                    else
                        cmdID.Parameters.Add("@WMSLOCATIONID", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtID.Rows[ItemCount]["WMSLOCATIONID"]);

                    //if (string.IsNullOrEmpty(Convert.ToString(dtID.Rows[ItemCount]["WMSPALLETID"])))
                    //    cmdID.Parameters.Add("@WMSPALLETID", SqlDbType.NVarChar, 18).Value = "";
                    //else
                    //    cmdID.Parameters.Add("@WMSPALLETID", SqlDbType.NVarChar, 18).Value = Convert.ToString(dtID.Rows[ItemCount]["WMSPALLETID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtID.Rows[ItemCount]["INVENTSERIALID"])))
                        cmdID.Parameters.Add("@INVENTSERIALID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdID.Parameters.Add("@INVENTSERIALID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtID.Rows[ItemCount]["INVENTSERIALID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtID.Rows[ItemCount]["INVENTLOCATIONID"])))
                        cmdID.Parameters.Add("@INVENTLOCATIONID", SqlDbType.NVarChar, 10).Value = "";
                    else
                        cmdID.Parameters.Add("@INVENTLOCATIONID", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtID.Rows[ItemCount]["INVENTLOCATIONID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtID.Rows[ItemCount]["CONFIGID"])))
                        cmdID.Parameters.Add("@CONFIGID", SqlDbType.NVarChar, 10).Value = "";
                    else
                        cmdID.Parameters.Add("@CONFIGID", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtID.Rows[ItemCount]["CONFIGID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtID.Rows[ItemCount]["INVENTSIZEID"])))
                        cmdID.Parameters.Add("@INVENTSIZEID", SqlDbType.NVarChar, 10).Value = "";
                    else
                        cmdID.Parameters.Add("@INVENTSIZEID", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtID.Rows[ItemCount]["INVENTSIZEID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtID.Rows[ItemCount]["INVENTCOLORID"])))
                        cmdID.Parameters.Add("@INVENTCOLORID", SqlDbType.NVarChar, 10).Value = "";
                    else
                        cmdID.Parameters.Add("@INVENTCOLORID", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtID.Rows[ItemCount]["INVENTCOLORID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtID.Rows[ItemCount]["INVENTSTYLEID"])))
                        cmdID.Parameters.Add("@INVENTSTYLEID", SqlDbType.NVarChar, 10).Value = "";
                    else
                        cmdID.Parameters.Add("@INVENTSTYLEID", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtID.Rows[ItemCount]["INVENTSTYLEID"]);

                    //if (string.IsNullOrEmpty(Convert.ToString(dtID.Rows[ItemCount]["INVENTSITEID"])))
                    //    cmdID.Parameters.Add("@INVENTSITEID", SqlDbType.NVarChar, 10).Value = "";
                    //else
                    //    cmdID.Parameters.Add("@INVENTSITEID", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtID.Rows[ItemCount]["INVENTSITEID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtID.Rows[ItemCount]["DATAAREAID"])))
                        cmdID.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = "";
                    else
                        cmdID.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtID.Rows[ItemCount]["DATAAREAID"]);

                    // cmdID.Parameters.Add("@RECVERSION", SqlDbType.Int).Value = Convert.ToInt16(dtID.Rows[ItemCount]["RECVERSION"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtID.Rows[ItemCount]["RECID"])))
                        cmdID.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdID.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtID.Rows[ItemCount]["RECID"]);

                    cmdID.CommandTimeout = 0;
                    cmdID.ExecuteNonQuery();
                    cmdID.Dispose();
                }
            }
        }

        private static void SaveInventDimCombination(DataTable dtIDC, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtIDC != null && dtIDC.Rows.Count > 0)
            {
                string commandIDC = " IF NOT EXISTS(SELECT TOP 1 ITEMID FROM InventDimCombination " +
                                            " WHERE ITEMID=@ITEMID and INVENTDIMID=@INVENTDIMID and DATAAREAID=@DATAAREAID ) BEGIN" +
                                            " INSERT INTO [InventDimCombination](ITEMID" +
                                                    ",INVENTDIMID" +
                                                    ",DISTINCTPRODUCTVARIANT" +
                                                    ",DATAAREAID" +
                    //",RECVERSION"+
                                                    ",RECID" +
                                                    ",RETAILVARIANTID)" +
                                            " VALUES(@ITEMID" +
                                                    ",@INVENTDIMID" +
                                                    ",@DISTINCTPRODUCTVARIANT" +
                                                    ",@DATAAREAID" +
                    //",@RECVERSION" +
                                                    ",@RECID" +
                                                    ",@RETAILVARIANTID) END";
                for (int ItemCount = 0; ItemCount < dtIDC.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdIDC = new SqlCommand(commandIDC, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIDC.Rows[ItemCount]["ITEMID"])))
                        cmdIDC.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdIDC.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIDC.Rows[ItemCount]["ITEMID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIDC.Rows[ItemCount]["INVENTDIMID"])))
                        cmdIDC.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdIDC.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIDC.Rows[ItemCount]["INVENTDIMID"]);


                    if (string.IsNullOrEmpty(Convert.ToString(dtIDC.Rows[ItemCount]["DISTINCTPRODUCTVARIANT"])))
                        cmdIDC.Parameters.Add("@DISTINCTPRODUCTVARIANT", SqlDbType.BigInt).Value = 0;
                    else
                        cmdIDC.Parameters.Add("@DISTINCTPRODUCTVARIANT", SqlDbType.BigInt).Value = Convert.ToInt64(dtIDC.Rows[ItemCount]["DISTINCTPRODUCTVARIANT"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIDC.Rows[ItemCount]["DATAAREAID"])))
                        cmdIDC.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = "";
                    else
                        cmdIDC.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtIDC.Rows[ItemCount]["DATAAREAID"]);

                    // cmdIDC.Parameters.Add("@RECVERSION", SqlDbType.Int).Value = Convert.ToInt16(dtIDC.Rows[ItemCount]["RECVERSION"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIDC.Rows[ItemCount]["RECID"])))
                        cmdIDC.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdIDC.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtIDC.Rows[ItemCount]["RECID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIDC.Rows[ItemCount]["RETAILVARIANTID"])))
                        cmdIDC.Parameters.Add("@RETAILVARIANTID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdIDC.Parameters.Add("@RETAILVARIANTID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIDC.Rows[ItemCount]["RETAILVARIANTID"]);

                    cmdIDC.CommandTimeout = 0;
                    cmdIDC.ExecuteNonQuery();
                    cmdIDC.Dispose();
                }
            }
        }

        private static void SaveInventItemBarcode(DataTable dtIIBC, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtIIBC != null && dtIIBC.Rows.Count > 0)
            {
                string commandIIBC = " IF NOT EXISTS(SELECT TOP 1 RECID FROM InventItemBarcode " +
                                            " WHERE RECID=@RECID) BEGIN" +
                                            " INSERT INTO [InventItemBarcode](ITEMBARCODE" +
                                                            ",ITEMID" +
                                                            ",INVENTDIMID" +
                    //",BARCODESETUPID"+
                    //",USEFORPRINTING"+
                    //",USEFORINPUT"+
                                                            ",DESCRIPTION" +
                                                            ",QTY" +
                                                            ",UNITID" +
                                                            ",RBOVARIANTID" +
                    //",RETAILSHOWFORITEM"+
                    //",BLOCKED"+
                    //",MODIFIEDDATE"+
                    //",DEL_MODIFIEDTIME"+
                    //",MODIFIEDBY"+
                                                            ",DATAAREAID" +
                    // ",RECVERSION"+
                    // ",PARTITION"+
                                                            ",RECID)" +
                                                    " VALUES(@ITEMBARCODE" +
                                                            ",@ITEMID" +
                                                            ",@INVENTDIMID" +
                    //",@BARCODESETUPID" +
                    //",@USEFORPRINTING" +
                    //",@USEFORINPUT" +
                                                            ",@DESCRIPTION" +
                                                            ",@QTY" +
                                                            ",@UNITID" +
                                                            ",@RBOVARIANTID" +
                    //",@RETAILSHOWFORITEM" +
                    //",@BLOCKED" +
                    //",@MODIFIEDDATE" +
                    //",@DEL_MODIFIEDTIME" +
                    //",@MODIFIEDBY" +
                                                            ",@DATAAREAID" +
                    // ",@RECVERSION" +
                    // ",@PARTITION" +
                                                            ",@RECID) END";

                for (int ItemCount = 0; ItemCount < dtIIBC.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdIIBC = new SqlCommand(commandIIBC, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIIBC.Rows[ItemCount]["ITEMBARCODE"])))
                        cmdIIBC.Parameters.Add("@ITEMBARCODE", SqlDbType.NVarChar, 80).Value = "";
                    else
                        cmdIIBC.Parameters.Add("@ITEMBARCODE", SqlDbType.NVarChar, 80).Value = Convert.ToString(dtIIBC.Rows[ItemCount]["ITEMBARCODE"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIIBC.Rows[ItemCount]["ITEMID"])))
                        cmdIIBC.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdIIBC.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIIBC.Rows[ItemCount]["ITEMID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIIBC.Rows[ItemCount]["INVENTDIMID"])))
                        cmdIIBC.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdIIBC.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIIBC.Rows[ItemCount]["INVENTDIMID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIIBC.Rows[ItemCount]["DESCRIPTION"])))
                        cmdIIBC.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar, 60).Value = "";
                    else
                        cmdIIBC.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar, 60).Value = Convert.ToString(dtIIBC.Rows[ItemCount]["DESCRIPTION"]);

                    cmdIIBC.Parameters.Add("@QTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtIIBC.Rows[ItemCount]["QTY"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIIBC.Rows[ItemCount]["UNITID"])))
                        cmdIIBC.Parameters.Add("@UNITID", SqlDbType.NVarChar, 10).Value = "";
                    else
                        cmdIIBC.Parameters.Add("@UNITID", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIIBC.Rows[ItemCount]["UNITID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIIBC.Rows[ItemCount]["RetailVariantId"])))
                        cmdIIBC.Parameters.Add("@RBOVARIANTID", SqlDbType.NVarChar, 10).Value = "";
                    else
                        cmdIIBC.Parameters.Add("@RBOVARIANTID", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIIBC.Rows[ItemCount]["RetailVariantId"]);



                    if (string.IsNullOrEmpty(Convert.ToString(dtIIBC.Rows[ItemCount]["DATAAREAID"])))
                        cmdIIBC.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = "";
                    else
                        cmdIIBC.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtIIBC.Rows[ItemCount]["DATAAREAID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIIBC.Rows[ItemCount]["RECID"])))
                        cmdIIBC.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdIIBC.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtIIBC.Rows[ItemCount]["RECID"]);

                    cmdIIBC.CommandTimeout = 0;
                    cmdIIBC.ExecuteNonQuery();
                    cmdIIBC.Dispose();
                }
            }
        }

        private static void SaveInventItemGroupItem(DataTable dtIIGI, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtIIGI != null && dtIIGI.Rows.Count > 0)
            {
                string commandIIGI = " IF NOT EXISTS(SELECT TOP 1 RECID FROM InventItemGroupItem " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [InventItemGroupItem](ITEMID" +
                                                        ",ITEMDATAAREAID" +
                                                        ",ITEMGROUPID" +
                                                        ",ITEMGROUPDATAAREAID" +
                                                        ",RECID)" +
                                            " VALUES(@ITEMID" +
                                                    ",@ITEMDATAAREAID" +
                                                    ",@ITEMGROUPID" +
                                                    ",@ITEMGROUPDATAAREAID" +
                                                    ",@RECID) END";

                for (int ItemCount = 0; ItemCount < dtIIGI.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdIIGI = new SqlCommand(commandIIGI, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIIGI.Rows[ItemCount]["ITEMID"])))
                        cmdIIGI.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdIIGI.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtIIGI.Rows[ItemCount]["ITEMID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIIGI.Rows[ItemCount]["ITEMDATAAREAID"])))
                        cmdIIGI.Parameters.Add("@ITEMDATAAREAID", SqlDbType.NVarChar, 4).Value = "";
                    else
                        cmdIIGI.Parameters.Add("@ITEMDATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtIIGI.Rows[ItemCount]["ITEMDATAAREAID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIIGI.Rows[ItemCount]["ITEMGROUPID"])))
                        cmdIIGI.Parameters.Add("@ITEMGROUPID", SqlDbType.NVarChar, 10).Value = "";
                    else
                        cmdIIGI.Parameters.Add("@ITEMGROUPID", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtIIGI.Rows[ItemCount]["ITEMGROUPID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIIGI.Rows[ItemCount]["ITEMGROUPDATAAREAID"])))
                        cmdIIGI.Parameters.Add("@ITEMGROUPDATAAREAID", SqlDbType.NVarChar, 4).Value = "";
                    else
                        cmdIIGI.Parameters.Add("@ITEMGROUPDATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtIIGI.Rows[ItemCount]["ITEMGROUPDATAAREAID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtIIGI.Rows[ItemCount]["RECID"])))
                        cmdIIGI.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdIIGI.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtIIGI.Rows[ItemCount]["RECID"]);

                    cmdIIGI.CommandTimeout = 0;
                    cmdIIGI.ExecuteNonQuery();
                    cmdIIGI.Dispose();
                }
            }
        }

        private static void SaveRetailAssortmentExploded(DataTable dtRAE, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtRAE != null && dtRAE.Rows.Count > 0)
            {
                string commandRAE = " IF NOT EXISTS(SELECT TOP 1 RECID FROM RetailAssortmentExploded " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [RetailAssortmentExploded](ASSORTMENTRECID" +
                                                            ",CHANNELDATAAREAID" +
                                                            ",INVENTDIMID" +
                                                            ",ITEMID" +
                                                            ",OMOPERATINGUNITID" +
                                                            ",VALIDFROM" +
                                                            ",VALIDTO" +
                    // ",RECVERSION"+
                                                            ",RECID)" +
                                            " VALUES(@ASSORTMENTRECID" +
                                                    ",@CHANNELDATAAREAID" +
                                                    ",@INVENTDIMID" +
                                                    ",@ITEMID" +
                                                    ",@OMOPERATINGUNITID" +
                                                    ",@VALIDFROM" +
                                                    ",@VALIDTO" +
                    // ",@RECVERSION" +
                                                    ",@RECID) END";

                SqlCommand cmdRAE = new SqlCommand(commandRAE, connection, transaction);
                if (string.IsNullOrEmpty(Convert.ToString(dtRAE.Rows[0]["ASSORTMENTRECID"])))
                    cmdRAE.Parameters.Add("@ASSORTMENTRECID", SqlDbType.BigInt).Value = 0;
                else
                    cmdRAE.Parameters.Add("@ASSORTMENTRECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtRAE.Rows[0]["ASSORTMENTRECID"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtRAE.Rows[0]["CHANNELDATAAREAID"])))
                    cmdRAE.Parameters.Add("@CHANNELDATAAREAID", SqlDbType.NVarChar, 4).Value = "";
                else
                    cmdRAE.Parameters.Add("@CHANNELDATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtRAE.Rows[0]["CHANNELDATAAREAID"]);
                if (string.IsNullOrEmpty(Convert.ToString(dtRAE.Rows[0]["INVENTDIMID"])))
                    cmdRAE.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdRAE.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtRAE.Rows[0]["INVENTDIMID"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtRAE.Rows[0]["ITEMID"])))
                    cmdRAE.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdRAE.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtRAE.Rows[0]["ITEMID"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtRAE.Rows[0]["OMOPERATINGUNITID"])))
                    cmdRAE.Parameters.Add("@OMOPERATINGUNITID", SqlDbType.BigInt).Value = 0;
                else
                    cmdRAE.Parameters.Add("@OMOPERATINGUNITID", SqlDbType.BigInt).Value = Convert.ToInt64(dtRAE.Rows[0]["OMOPERATINGUNITID"]);


                if (string.IsNullOrEmpty(Convert.ToString(dtRAE.Rows[0]["VALIDFROM"])))
                    cmdRAE.Parameters.Add("@VALIDFROM", SqlDbType.DateTime).Value = Convert.ToDateTime("01-Jan-1900");
                else
                    cmdRAE.Parameters.Add("@VALIDFROM", SqlDbType.DateTime).Value = Convert.ToDateTime(dtRAE.Rows[0]["VALIDFROM"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtRAE.Rows[0]["VALIDTO"])))
                    cmdRAE.Parameters.Add("@VALIDTO", SqlDbType.DateTime).Value = Convert.ToDateTime("01-Jan-1900");
                else
                    cmdRAE.Parameters.Add("@VALIDTO", SqlDbType.DateTime).Value = Convert.ToDateTime(dtRAE.Rows[0]["VALIDTO"]);

                // cmdRAE.Parameters.Add("@RECVERSION", SqlDbType.Int).Value = Convert.ToInt16(dtRAE.Rows[0]["RECVERSION"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtRAE.Rows[0]["RECID"])))
                    cmdRAE.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                else
                    cmdRAE.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtRAE.Rows[0]["RECID"]);

                cmdRAE.CommandTimeout = 0;
                cmdRAE.ExecuteNonQuery();
                cmdRAE.Dispose();
            }
        }

        private static void SaveRetailInventTable(DataTable dtRIT, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtRIT != null && dtRIT.Rows.Count > 0)
            {
                string commandRIT = " IF NOT EXISTS(SELECT TOP 1 ITEMID FROM RetailInventTable " +
                                            " WHERE ITEMID=@ITEMID AND DATAAREAID=@DATAAREAID ) BEGIN" +
                                            " INSERT INTO [RetailInventTable](ITEMID" +
                                                        ",ZEROPRICEVALID" +
                                                        ",QTYBECOMESNEGATIVE" +
                                                        ",NODISCOUNTALLOWED" +
                                                        ",KEYINGINPRICE" +
                                                        ",SCALEITEM" +
                                                        ",KEYINGINQTY" +
                                                        ",DATEBLOCKED" +
                                                        ",DATETOBEBLOCKED" +
                                                        ",BLOCKEDONPOS" +
                                                        ",DATAAREAID" +
                                                        ",RECID" +
                                                        ",MUSTKEYINCOMMENT" +
                                                        ",DATETOACTIVATEITEM)" +
                                            " VALUES(@ITEMID" +
                                                        ",@ZEROPRICEVALID" +
                                                        ",@QTYBECOMESNEGATIVE" +
                                                        ",@NODISCOUNTALLOWED" +
                                                        ",@KEYINGINPRICE" +
                                                        ",@SCALEITEM" +
                                                        ",@KEYINGINQTY" +
                                                        ",@DATEBLOCKED" +
                                                        ",@DATETOBEBLOCKED" +
                                                        ",@BLOCKEDONPOS" +
                                                        ",@DATAAREAID" +
                                                        ",@RECID" +
                                                        ",@MUSTKEYINCOMMENT" +
                                                        ",@DATETOACTIVATEITEM) END";

                for (int ItemCount = 0; ItemCount < dtRIT.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdRIT = new SqlCommand(commandRIT, connection, transaction);


                    if (string.IsNullOrEmpty(Convert.ToString(dtRIT.Rows[ItemCount]["ITEMID"])))
                        cmdRIT.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdRIT.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtRIT.Rows[ItemCount]["ITEMID"]);

                    cmdRIT.Parameters.Add("@ZEROPRICEVALID", SqlDbType.Int).Value = Convert.ToInt16(dtRIT.Rows[ItemCount]["ZEROPRICEVALID"]);
                    cmdRIT.Parameters.Add("@QTYBECOMESNEGATIVE", SqlDbType.Int).Value = Convert.ToInt16(dtRIT.Rows[ItemCount]["QTYBECOMESNEGATIVE"]);
                    cmdRIT.Parameters.Add("@NODISCOUNTALLOWED", SqlDbType.Int).Value = Convert.ToInt16(dtRIT.Rows[ItemCount]["NODISCOUNTALLOWED"]);
                    cmdRIT.Parameters.Add("@KEYINGINPRICE", SqlDbType.Int).Value = Convert.ToInt16(dtRIT.Rows[ItemCount]["KEYINGINPRICE"]);
                    cmdRIT.Parameters.Add("@SCALEITEM", SqlDbType.Int).Value = Convert.ToInt16(dtRIT.Rows[ItemCount]["SCALEITEM"]);
                    cmdRIT.Parameters.Add("@KEYINGINQTY", SqlDbType.Int).Value = Convert.ToInt16(dtRIT.Rows[ItemCount]["KEYINGINQTY"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtRIT.Rows[ItemCount]["DATEBLOCKED"])))
                        cmdRIT.Parameters.Add("@DATEBLOCKED", SqlDbType.DateTime).Value = Convert.ToDateTime("01-Jan-1900");
                    else
                        cmdRIT.Parameters.Add("@DATEBLOCKED", SqlDbType.DateTime).Value = Convert.ToDateTime(dtRIT.Rows[ItemCount]["DATEBLOCKED"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtRIT.Rows[ItemCount]["DATETOBEBLOCKED"])))
                        cmdRIT.Parameters.Add("@DATETOBEBLOCKED", SqlDbType.DateTime).Value = Convert.ToDateTime("01-Jan-1900");
                    else
                        cmdRIT.Parameters.Add("@DATETOBEBLOCKED", SqlDbType.DateTime).Value = Convert.ToDateTime(dtRIT.Rows[ItemCount]["DATETOBEBLOCKED"]);

                    cmdRIT.Parameters.Add("@BLOCKEDONPOS", SqlDbType.Int).Value = Convert.ToInt16(dtRIT.Rows[ItemCount]["BLOCKEDONPOS"]);


                    if (string.IsNullOrEmpty(Convert.ToString(dtRIT.Rows[ItemCount]["DATAAREAID"])))
                        cmdRIT.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = "";
                    else
                        cmdRIT.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtRIT.Rows[ItemCount]["DATAAREAID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtRIT.Rows[ItemCount]["RECID"])))
                        cmdRIT.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdRIT.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtRIT.Rows[ItemCount]["RECID"]);

                    cmdRIT.Parameters.Add("@MUSTKEYINCOMMENT", SqlDbType.Int).Value = Convert.ToInt16(dtRIT.Rows[ItemCount]["MUSTKEYINCOMMENT"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtRIT.Rows[ItemCount]["DATETOACTIVATEITEM"])))
                        cmdRIT.Parameters.Add("@DATETOACTIVATEITEM", SqlDbType.DateTime).Value = Convert.ToDateTime("01-Jan-1900");
                    else
                        cmdRIT.Parameters.Add("@DATETOACTIVATEITEM", SqlDbType.DateTime).Value = Convert.ToDateTime(dtRIT.Rows[ItemCount]["DATETOACTIVATEITEM"]);

                    cmdRIT.CommandTimeout = 0;
                    cmdRIT.ExecuteNonQuery();
                    cmdRIT.Dispose();
                }
            }
        }

        private static void SaveUnitOfMeasureConversion(DataTable dtUMC, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtUMC != null && dtUMC.Rows.Count > 0)
            {
                string commandUMC = " IF NOT EXISTS(SELECT TOP 1 RECID FROM UnitOfMeasureConversion " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [UnitOfMeasureConversion](FROMUNITOFMEASURE" +
                                                        ",TOUNITOFMEASURE" +
                                                        ",PRODUCT" +
                                                        ",FACTOR" +
                                                        ",NUMERATOR" +
                                                        ",DENOMINATOR" +
                                                        ",INNEROFFSET" +
                                                        ",OUTEROFFSET" +
                                                        ",ROUNDING" +
                    //",RECVERSION"+
                                                        ",RECID)" +
                                            " VALUES(@FROMUNITOFMEASURE" +
                                                        ",@TOUNITOFMEASURE" +
                                                        ",@PRODUCT" +
                                                        ",@FACTOR" +
                                                        ",@NUMERATOR" +
                                                        ",@DENOMINATOR" +
                                                        ",@INNEROFFSET" +
                                                        ",@OUTEROFFSET" +
                                                        ",@ROUNDING" +
                    // ",@RECVERSION" +
                                                        ",@RECID) END";

                for (int ItemCount = 0; ItemCount < dtUMC.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdUMC = new SqlCommand(commandUMC, connection, transaction);
                    if (string.IsNullOrEmpty(Convert.ToString(dtUMC.Rows[ItemCount]["FROMUNITOFMEASURE"])))
                        cmdUMC.Parameters.Add("@FROMUNITOFMEASURE", SqlDbType.BigInt).Value = 0;
                    else
                        cmdUMC.Parameters.Add("@FROMUNITOFMEASURE", SqlDbType.BigInt).Value = Convert.ToInt64(dtUMC.Rows[ItemCount]["FROMUNITOFMEASURE"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtUMC.Rows[ItemCount]["TOUNITOFMEASURE"])))
                        cmdUMC.Parameters.Add("@TOUNITOFMEASURE", SqlDbType.BigInt).Value = 0;
                    else
                        cmdUMC.Parameters.Add("@TOUNITOFMEASURE", SqlDbType.BigInt).Value = Convert.ToInt64(dtUMC.Rows[ItemCount]["TOUNITOFMEASURE"]);
                    if (string.IsNullOrEmpty(Convert.ToString(dtUMC.Rows[ItemCount]["PRODUCT"])))
                        cmdUMC.Parameters.Add("@PRODUCT", SqlDbType.BigInt).Value = 0;
                    else
                        cmdUMC.Parameters.Add("@PRODUCT", SqlDbType.BigInt).Value = Convert.ToInt64(dtUMC.Rows[ItemCount]["PRODUCT"]);


                    if (string.IsNullOrEmpty(Convert.ToString(dtUMC.Rows[ItemCount]["FACTOR"])))
                        cmdUMC.Parameters.Add("@FACTOR", SqlDbType.Decimal).Value = 0;
                    else
                        cmdUMC.Parameters.Add("@FACTOR", SqlDbType.Decimal).Value = Convert.ToDecimal(dtUMC.Rows[ItemCount]["FACTOR"]);
                    cmdUMC.Parameters.Add("@NUMERATOR", SqlDbType.Int).Value = Convert.ToInt16(dtUMC.Rows[ItemCount]["NUMERATOR"]);
                    cmdUMC.Parameters.Add("@DENOMINATOR", SqlDbType.Int).Value = Convert.ToInt16(dtUMC.Rows[ItemCount]["DENOMINATOR"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtUMC.Rows[ItemCount]["INNEROFFSET"])))
                        cmdUMC.Parameters.Add("@INNEROFFSET", SqlDbType.Decimal).Value = 0;
                    else
                        cmdUMC.Parameters.Add("@INNEROFFSET", SqlDbType.Decimal).Value = Convert.ToDecimal(dtUMC.Rows[ItemCount]["INNEROFFSET"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtUMC.Rows[ItemCount]["OUTEROFFSET"])))
                        cmdUMC.Parameters.Add("@OUTEROFFSET", SqlDbType.Decimal).Value = 0;
                    else
                        cmdUMC.Parameters.Add("@OUTEROFFSET", SqlDbType.Decimal).Value = Convert.ToDecimal(dtUMC.Rows[ItemCount]["OUTEROFFSET"]);

                    cmdUMC.Parameters.Add("@ROUNDING", SqlDbType.Int).Value = Convert.ToInt16(dtUMC.Rows[ItemCount]["ROUNDING"]);

                    //cmdUMC.Parameters.Add("@RECVERSION", SqlDbType.Int).Value = Convert.ToInt16(dtUMC.Rows[ItemCount]["RECVERSION"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtUMC.Rows[ItemCount]["RECID"])))
                        cmdUMC.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdUMC.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtUMC.Rows[ItemCount]["RECID"]);

                    cmdUMC.CommandTimeout = 0;
                    cmdUMC.ExecuteNonQuery();
                    cmdUMC.Dispose();
                }
            }
        }

        private static void SaveECORESPRODUCTMASTERDIMENSIONVALUE(DataTable dtConf, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtConf != null && dtConf.Rows.Count > 0)
            {
                string commandERPD = " IF NOT EXISTS(SELECT TOP 1 RECID FROM ECORESPRODUCTMASTERDIMENSIONVALUE " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [ECORESPRODUCTMASTERDIMENSIONVALUE](INSTANCERELATIONTYPE" +
                                                    ",ADDITIONALDESCRIPTION" +
                                                    ",DESCRIPTION" +
                    //",RECVERSION"+
                    // ",RELATIONTYPE"+
                                                    ",RECID" +
                                                    ",RETAILWEIGHT" +
                                                    ",RETAILDISPLAYORDER)" +
                                            " VALUES(@INSTANCERELATIONTYPE" +
                                                    ",@ADDITIONALDESCRIPTION" +
                                                    ",@DESCRIPTION" +
                    //",@RECVERSION" +
                    //",@RELATIONTYPE" +
                                                    ",@RECID" +
                                                    ",@RETAILWEIGHT" +
                                                    ",@RETAILDISPLAYORDER) END";

                for (int ItemCount = 0; ItemCount < dtConf.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdECPD = new SqlCommand(commandERPD, connection, transaction);
                    if (string.IsNullOrEmpty(Convert.ToString(dtConf.Rows[ItemCount]["INSTANCERELATIONTYPE"])))
                        cmdECPD.Parameters.Add("@INSTANCERELATIONTYPE", SqlDbType.BigInt).Value = 0;
                    else
                        cmdECPD.Parameters.Add("@INSTANCERELATIONTYPE", SqlDbType.BigInt).Value = Convert.ToInt64(dtConf.Rows[ItemCount]["INSTANCERELATIONTYPE"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtConf.Rows[ItemCount]["ADDITIONALDESCRIPTION"])))
                        cmdECPD.Parameters.Add("@ADDITIONALDESCRIPTION", SqlDbType.NVarChar, 1000).Value = "";
                    else
                        cmdECPD.Parameters.Add("@ADDITIONALDESCRIPTION", SqlDbType.NVarChar, 1000).Value = Convert.ToInt64(dtConf.Rows[ItemCount]["ADDITIONALDESCRIPTION"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtConf.Rows[ItemCount]["DESCRIPTION"])))
                        cmdECPD.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar, 60).Value = "";
                    else
                        cmdECPD.Parameters.Add("@DESCRIPTION", SqlDbType.NVarChar, 60).Value = Convert.ToString(dtConf.Rows[ItemCount]["DESCRIPTION"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtConf.Rows[ItemCount]["RECID"])))
                        cmdECPD.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdECPD.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtConf.Rows[ItemCount]["RECID"]);

                    cmdECPD.Parameters.Add("@RETAILWEIGHT", SqlDbType.Int).Value = Convert.ToInt16(dtConf.Rows[ItemCount]["RETAILWEIGHT"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtConf.Rows[ItemCount]["RETAILDISPLAYORDER"])))
                        cmdECPD.Parameters.Add("@RETAILDISPLAYORDER", SqlDbType.Decimal).Value = 0;
                    else
                        cmdECPD.Parameters.Add("@RETAILDISPLAYORDER", SqlDbType.Decimal).Value = Convert.ToDecimal(dtConf.Rows[ItemCount]["RETAILDISPLAYORDER"]);

                    cmdECPD.CommandTimeout = 0;
                    cmdECPD.ExecuteNonQuery();
                    cmdECPD.Dispose();
                }
            }
        }

        private static void SaveSKUTable(DataTable dtSKUT, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtSKUT != null && dtSKUT.Rows.Count > 0)
            {
                string commandSKUT = " DELETE FROM SKUTable_Posted " +
                                            " WHERE SkuNumber=@SkuNumber; BEGIN" +
                                            " INSERT INTO [SKUTable_Posted](SkuDate,SkuNumber,DATAAREAID," +
                                                    " PDSCWQTY,qty,INGREDIENT," +
                                                    " ECORESCONFIGURATIONNAME,INVENTCOLORID,INVENTSIZEID," +
                                                    " NETQTY,DMDQTY,STNQTY,VEND_ACCOUNT," +
                                                    " SKUCertificate,UpdatedCostPrice,GroupCostPrice," +
                                                    " SellingPrice,GrossWeight,NetWeight," +
                                                    " TagPrice,TagCurrency,TAGPRICEMST," +
                                                    " EXTERNALITEMID,SERIALNUMBER,UNITID," +
                                                    " StoreGroup,MinPurAmt," +
                                                    " FreeGift,CRWGiftType,GiftVoucherCode," +
                                                    " ValidityDay,ACCOUNTTYPE,CostPrice," +
                                                    " TRANSFERCOSTPRICE,BarcodeLabelWeight," +
                                                    " TotalCostPrice,CertificationNo,CRWSTONETRANSFERCOST,"+
                                                    " CUSTORDERNUM,CUSTORDERLINENUM)" +
                                            " VALUES(@SkuDate" +
                                                    ",@SkuNumber,@DATAAREAID," +
                                                    " @PDSCWQTY,@qty,@INGREDIENT," +
                                                    " @ECORESCONFIGURATIONNAME,@INVENTCOLORID,@INVENTSIZEID," +
                                                    " @NETQTY,@DMDQTY,@STNQTY,@VEND_ACCOUNT," +
                                                    " @SKUCertificate,@UpdatedCostPrice,@GroupCostPrice," +
                                                    " @SellingPrice,@GrossWeight,@NetWeight," +
                                                    " @TagPrice,@TagCurrency,@TAGPRICEMST," +
                                                    " @EXTERNALITEMID,@SERIALNUMBER,@UNITID," +
                                                    " @StoreGroup,@MinPurAmt," +
                                                    " @FreeGift,@CRWGiftType,@GiftVoucherCode," +
                                                    " @ValidityDay,@ACCOUNTTYPE,@CostPrice," +
                                                    " @TRANSFERCOSTPRICE,@BarcodeLabelWeight," +
                                                    " @TotalCostPrice,@CertificationNo,@CRWSTONETRANSFERCOST,"+
                                                    " @CUSTORDERNUM,@CUSTORDERLINENUM) END";

                SqlCommand cmdSKUT = new SqlCommand(commandSKUT, connection, transaction);

                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["SKUCreationDate"])))
                    cmdSKUT.Parameters.Add("@SkuDate", SqlDbType.DateTime).Value = Convert.ToDateTime("01-Jan-1900");
                else
                    cmdSKUT.Parameters.Add("@SkuDate", SqlDbType.DateTime).Value = Convert.ToDateTime(dtSKUT.Rows[0]["SKUCreationDate"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["SkuNumber"])))
                    cmdSKUT.Parameters.Add("@SkuNumber", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdSKUT.Parameters.Add("@SkuNumber", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUT.Rows[0]["SkuNumber"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["DataAreaId"])))
                    cmdSKUT.Parameters.Add("@DataAreaId", SqlDbType.NVarChar, 4).Value = "";
                else
                    cmdSKUT.Parameters.Add("@DataAreaId", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtSKUT.Rows[0]["DataAreaId"]);

                cmdSKUT.Parameters.Add("@PDSCWQTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["PDSCWQTY"]);
                cmdSKUT.Parameters.Add("@qty", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["qty"]);
                cmdSKUT.Parameters.Add("@INGREDIENT", SqlDbType.Int).Value = Convert.ToInt16(dtSKUT.Rows[0]["INGREDIENT"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["ECORESCONFIGURATIONNAME"])))
                    cmdSKUT.Parameters.Add("@ECORESCONFIGURATIONNAME", SqlDbType.NVarChar, 10).Value = "";
                else
                    cmdSKUT.Parameters.Add("@ECORESCONFIGURATIONNAME", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtSKUT.Rows[0]["ECORESCONFIGURATIONNAME"]);
                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["INVENTCOLORID"])))
                    cmdSKUT.Parameters.Add("@INVENTCOLORID", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdSKUT.Parameters.Add("@INVENTCOLORID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUT.Rows[0]["INVENTCOLORID"]);
                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["INVENTSIZEID"])))
                    cmdSKUT.Parameters.Add("@INVENTSIZEID", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdSKUT.Parameters.Add("@INVENTSIZEID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUT.Rows[0]["INVENTSIZEID"]);

                cmdSKUT.Parameters.Add("@NETQTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["NETQTY"]);
                cmdSKUT.Parameters.Add("@DMDQTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["DMDQTY"]);
                cmdSKUT.Parameters.Add("@STNQTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["STNQTY"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["VENDACCOUNT"])))
                    cmdSKUT.Parameters.Add("@VEND_ACCOUNT", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdSKUT.Parameters.Add("@VEND_ACCOUNT", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUT.Rows[0]["VENDACCOUNT"]);

                cmdSKUT.Parameters.Add("@SKUCertificate", SqlDbType.Int).Value = Convert.ToInt16(dtSKUT.Rows[0]["Certificate"]);
                cmdSKUT.Parameters.Add("@UpdatedCostPrice", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["UpdatedCostPrice"]);
                cmdSKUT.Parameters.Add("@GroupCostPrice", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["GroupCostPrice"]);
                cmdSKUT.Parameters.Add("@SellingPrice", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["SellingPrice"]);
                cmdSKUT.Parameters.Add("@GrossWeight", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["GrossWeight"]);
                cmdSKUT.Parameters.Add("@NetWeight", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["NetWeight"]);
                cmdSKUT.Parameters.Add("@TagPrice", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["TagPrice"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["TagCurrency"])))
                    cmdSKUT.Parameters.Add("@TagCurrency", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdSKUT.Parameters.Add("@TagCurrency", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUT.Rows[0]["TagCurrency"]);

                cmdSKUT.Parameters.Add("@TAGPRICEMST", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["TAGPRICEMST"]);
                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["EXTERNALITEMID"])))
                    cmdSKUT.Parameters.Add("@EXTERNALITEMID", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdSKUT.Parameters.Add("@EXTERNALITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUT.Rows[0]["EXTERNALITEMID"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["SERIALNUMBER"])))
                    cmdSKUT.Parameters.Add("@SERIALNUMBER", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdSKUT.Parameters.Add("@SERIALNUMBER", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUT.Rows[0]["SERIALNUMBER"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["UnitId"])))
                    cmdSKUT.Parameters.Add("@UnitId", SqlDbType.NVarChar, 10).Value = "";
                else
                    cmdSKUT.Parameters.Add("@UnitId", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtSKUT.Rows[0]["UnitId"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["StoreGroup"])))
                    cmdSKUT.Parameters.Add("@StoreGroup", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdSKUT.Parameters.Add("@StoreGroup", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUT.Rows[0]["StoreGroup"]);

                cmdSKUT.Parameters.Add("@MinPurAmt", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["MinPurAmt"]);

                cmdSKUT.Parameters.Add("@FreeGift", SqlDbType.Int).Value = Convert.ToInt16(dtSKUT.Rows[0]["FreeGift"]);

                cmdSKUT.Parameters.Add("@CRWGiftType", SqlDbType.Int).Value = Convert.ToInt16(dtSKUT.Rows[0]["CRWGiftType"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["GiftVoucherCode"])))
                    cmdSKUT.Parameters.Add("@GiftVoucherCode", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdSKUT.Parameters.Add("@GiftVoucherCode", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUT.Rows[0]["GiftVoucherCode"]);

                cmdSKUT.Parameters.Add("@ValidityDay", SqlDbType.Int).Value = Convert.ToInt16(dtSKUT.Rows[0]["Day"]);
                cmdSKUT.Parameters.Add("@AccountType", SqlDbType.Int).Value = Convert.ToInt16(dtSKUT.Rows[0]["AccountType"]);

                cmdSKUT.Parameters.Add("@CostPrice", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["CostPrice"]);
                cmdSKUT.Parameters.Add("@TRANSFERCOSTPRICE", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["TRANSFERCOSTPRICE"]);
                cmdSKUT.Parameters.Add("@BarcodeLabelWeight", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["BarcodeLabelWeight"]);
                cmdSKUT.Parameters.Add("@TotalCostPrice", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["TotalCost"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["CertificationNo"])))
                    cmdSKUT.Parameters.Add("@CertificationNo", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdSKUT.Parameters.Add("@CertificationNo", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUT.Rows[0]["CertificationNo"]);
                cmdSKUT.Parameters.Add("@CRWSTONETRANSFERCOST", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUT.Rows[0]["CRWSTONETRANSFERCOST"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtSKUT.Rows[0]["CUSTORDERNO"])))
                    cmdSKUT.Parameters.Add("@CUSTORDERNUM", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdSKUT.Parameters.Add("@CUSTORDERNUM", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUT.Rows[0]["CUSTORDERNO"]);

                int OrderLine = 0;
                if (Convert.ToDecimal(dtSKUT.Rows[0]["CUSTLINENUM"]) > 0)
                {
                    OrderLine = Convert.ToInt16(dtSKUT.Rows[0]["CUSTLINENUM"]);
                }

                cmdSKUT.Parameters.Add("@CUSTORDERLINENUM", SqlDbType.Int).Value = OrderLine;// Convert.ToInt16(dtSKUT.Rows[0]["CUSTLINENUM"]);

                cmdSKUT.CommandTimeout = 0;
                cmdSKUT.ExecuteNonQuery();
                cmdSKUT.Dispose();
            }
        }

        private static void SaveSKULine(DataTable dtSKUL, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtSKUL != null && dtSKUL.Rows.Count > 0)
            {
                string commandSKUL = " DELETE FROM SKULine_Posted " +
                                            " WHERE SkuNumber=@SkuNumber and LineNum=@LineNum; BEGIN" +
                                            " INSERT INTO [SKULine_Posted](SkuNumber,LineNum," +
                                                       " ItemID,InventDimID," +
                                                       " qty,CValue,UnitID," +
                                                       " DATAAREAID,PDSCWQTY,CRate )" +
                                            " VALUES(@SkuNumber,@LineNum," +
                                                    " @ItemID,@InventDimID,@qty," +
                                                    " @CValue,@UnitID,@DATAAREAID," +
                                                    " @PDSCWQTY, @CRate) END";

                for (int ItemCount = 0; ItemCount < dtSKUL.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdSKUL = new SqlCommand(commandSKUL, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtSKUL.Rows[ItemCount]["SkuNumber"])))
                        cmdSKUL.Parameters.Add("@SkuNumber", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdSKUL.Parameters.Add("@SkuNumber", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUL.Rows[ItemCount]["SkuNumber"]);

                    cmdSKUL.Parameters.Add("@LineNum", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUL.Rows[ItemCount]["LineNum"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtSKUL.Rows[ItemCount]["ItemID"])))
                        cmdSKUL.Parameters.Add("@ItemID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdSKUL.Parameters.Add("@ItemID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUL.Rows[ItemCount]["ItemID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtSKUL.Rows[ItemCount]["InventDimID"])))
                        cmdSKUL.Parameters.Add("@InventDimID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdSKUL.Parameters.Add("@InventDimID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUL.Rows[ItemCount]["InventDimID"]);

                    cmdSKUL.Parameters.Add("@qty", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUL.Rows[ItemCount]["qty"]);
                    cmdSKUL.Parameters.Add("@CValue", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUL.Rows[ItemCount]["CValue"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtSKUL.Rows[ItemCount]["UnitID"])))
                        cmdSKUL.Parameters.Add("@UnitID", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdSKUL.Parameters.Add("@UnitID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSKUL.Rows[ItemCount]["UnitID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtSKUL.Rows[ItemCount]["DataAreaId"])))
                        cmdSKUL.Parameters.Add("@DataAreaId", SqlDbType.NVarChar, 4).Value = "";
                    else
                        cmdSKUL.Parameters.Add("@DataAreaId", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtSKUL.Rows[ItemCount]["DataAreaId"]);

                    cmdSKUL.Parameters.Add("@PDSCWQTY", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUL.Rows[ItemCount]["PDSCWQTY"]);
                    cmdSKUL.Parameters.Add("@CRate", SqlDbType.Decimal).Value = Convert.ToDecimal(dtSKUL.Rows[ItemCount]["CRate"]);


                    cmdSKUL.CommandTimeout = 0;
                    cmdSKUL.ExecuteNonQuery();
                    cmdSKUL.Dispose();
                }
            }
        }

        private static void SavePDSCWITEM(DataTable dtPdsCWItem, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtPdsCWItem != null && dtPdsCWItem.Rows.Count > 0)
            {
                string commandPdsCWItem = " IF NOT EXISTS(SELECT TOP 1 ITEMID FROM PDSCATCHWEIGHTITEM " +
                                            " WHERE ITEMID=@ITEMID and DATAAREAID=@DATAAREAID ) BEGIN" +
                                            " INSERT INTO [PDSCATCHWEIGHTITEM](ITEMID" +
                                                    ",PDSCWMAX" +
                                                    ",PDSCWMIN" +
                                                    ",PDSCWUNITID" +
                                                    ",DATAAREAID)" +
                                            " VALUES(@ITEMID" +
                                                    ",@PDSCWMAX" +
                                                    ",@PDSCWMIN" +
                                                    ",@PDSCWUNITID" +
                                                    ",@DATAAREAID) END";

                SqlCommand cmdPdsCWitem = new SqlCommand(commandPdsCWItem, connection, transaction);

                if (string.IsNullOrEmpty(Convert.ToString(dtPdsCWItem.Rows[0]["ITEMID"])))
                    cmdPdsCWitem.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = "";
                else
                    cmdPdsCWitem.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtPdsCWItem.Rows[0]["ITEMID"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtPdsCWItem.Rows[0]["PDSCWMAX"])))
                    cmdPdsCWitem.Parameters.Add("@PDSCWMAX", SqlDbType.Decimal).Value = 0;
                else
                    cmdPdsCWitem.Parameters.Add("@PDSCWMAX", SqlDbType.Decimal).Value = Convert.ToDecimal(dtPdsCWItem.Rows[0]["PDSCWMAX"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtPdsCWItem.Rows[0]["PDSCWMIN"])))
                    cmdPdsCWitem.Parameters.Add("@PDSCWMIN", SqlDbType.Decimal).Value = 0;
                else
                    cmdPdsCWitem.Parameters.Add("@PDSCWMIN", SqlDbType.Decimal).Value = Convert.ToDecimal(dtPdsCWItem.Rows[0]["PDSCWMIN"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtPdsCWItem.Rows[0]["PDSCWUNITID"])))
                    cmdPdsCWitem.Parameters.Add("@PDSCWUNITID", SqlDbType.NVarChar, 10).Value = "";
                else
                    cmdPdsCWitem.Parameters.Add("@PDSCWUNITID", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtPdsCWItem.Rows[0]["PDSCWUNITID"]);

                if (string.IsNullOrEmpty(Convert.ToString(dtPdsCWItem.Rows[0]["DATAAREAID"])))
                    cmdPdsCWitem.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = "";
                else
                    cmdPdsCWitem.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtPdsCWItem.Rows[0]["DATAAREAID"]);

                cmdPdsCWitem.CommandTimeout = 0;
                cmdPdsCWitem.ExecuteNonQuery();
                cmdPdsCWitem.Dispose();
            }
        }

        private static void SavePriceDiscTable(DataTable dtPDT, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtPDT != null && dtPDT.Rows.Count > 0)
            {
                if (Convert.ToInt64(dtPDT.Rows[0]["RECID"]) > 0)
                {
                    string commandPDT = " DELETE FROM PRICEDISCTABLE " +
                                                " WHERE ITEMRELATION=@ITEMRELATION; BEGIN" +
                                                " INSERT INTO [PRICEDISCTABLE](ITEMCODE,ACCOUNTCODE,ITEMRELATION" +
                                                        ",ACCOUNTRELATION,QUANTITYAMOUNTFROM,FROMDATE,TODATE" +
                                                        ",AMOUNT,CURRENCY,PERCENT1,PERCENT2,DELIVERYTIME" +
                                                        ",SEARCHAGAIN,PRICEUNIT,RELATION,QUANTITYAMOUNTTO" +
                                                        ",UNITID,MARKUP,ALLOCATEMARKUP,MODULE,INVENTDIMID,DATAAREAID" +
                                                        ",RECID,MAXIMUMRETAILPRICE_IN)" +
                                                " VALUES(@ITEMCODE,@ACCOUNTCODE,@ITEMRELATION" +
                                                        ",@ACCOUNTRELATION,@QUANTITYAMOUNTFROM,@FROMDATE,@TODATE" +
                                                        ",@AMOUNT,@CURRENCY,@PERCENT1,@PERCENT2,@DELIVERYTIME" +
                                                        ",@SEARCHAGAIN,@PRICEUNIT,@RELATION,@QUANTITYAMOUNTTO" +
                                                        ",@UNITID,@MARKUP,@ALLOCATEMARKUP,@MODULE,@INVENTDIMID,@DATAAREAID" +
                                                        ",@RECID,@MAXIMUMRETAILPRICE_IN) END";


                    for (int ItemCount = 0; ItemCount < dtPDT.Rows.Count; ItemCount++)
                    {
                        SqlCommand cmdPDT = new SqlCommand(commandPDT, connection, transaction);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["ITEMCODE"])))
                            cmdPDT.Parameters.Add("@ITEMCODE", SqlDbType.Int).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@ITEMCODE", SqlDbType.Int).Value = Convert.ToInt16(dtPDT.Rows[ItemCount]["ITEMCODE"]);
                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["ACCOUNTCODE"])))
                            cmdPDT.Parameters.Add("@ACCOUNTCODE", SqlDbType.Int).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@ACCOUNTCODE", SqlDbType.Int).Value = Convert.ToInt16(dtPDT.Rows[ItemCount]["ACCOUNTCODE"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["ITEMRELATION"])))
                            cmdPDT.Parameters.Add("@ITEMRELATION", SqlDbType.NVarChar, 20).Value = "";
                        else
                            cmdPDT.Parameters.Add("@ITEMRELATION", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtPDT.Rows[ItemCount]["ITEMRELATION"]);
                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["ACCOUNTRELATION"])))
                            cmdPDT.Parameters.Add("@ACCOUNTRELATION", SqlDbType.NVarChar, 20).Value = "";
                        else
                            cmdPDT.Parameters.Add("@ACCOUNTRELATION", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtPDT.Rows[ItemCount]["ACCOUNTRELATION"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["QUANTITYAMOUNTFROM"])))
                            cmdPDT.Parameters.Add("@QUANTITYAMOUNTFROM", SqlDbType.Decimal).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@QUANTITYAMOUNTFROM", SqlDbType.Decimal).Value = Convert.ToDecimal(dtPDT.Rows[ItemCount]["QUANTITYAMOUNTFROM"]);


                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["FROMDATE"])))
                            cmdPDT.Parameters.Add("@FROMDATE", SqlDbType.DateTime).Value = Convert.ToDateTime("01-Jan-1900");
                        else
                            cmdPDT.Parameters.Add("@FROMDATE", SqlDbType.DateTime).Value = Convert.ToDateTime(dtPDT.Rows[ItemCount]["FROMDATE"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["TODATE"])))
                            cmdPDT.Parameters.Add("@TODATE", SqlDbType.DateTime).Value = Convert.ToDateTime("01-Jan-1900");
                        else
                            cmdPDT.Parameters.Add("@TODATE", SqlDbType.DateTime).Value = Convert.ToDateTime(dtPDT.Rows[ItemCount]["TODATE"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["AMOUNT"])))
                            cmdPDT.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@AMOUNT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtPDT.Rows[ItemCount]["AMOUNT"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["CURRENCY"])))
                            cmdPDT.Parameters.Add("@CURRENCY", SqlDbType.NVarChar, 10).Value = "";
                        else
                            cmdPDT.Parameters.Add("@CURRENCY", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtPDT.Rows[ItemCount]["CURRENCY"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["PERCENT1"])))
                            cmdPDT.Parameters.Add("@PERCENT1", SqlDbType.Decimal).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@PERCENT1", SqlDbType.Decimal).Value = Convert.ToDecimal(dtPDT.Rows[ItemCount]["PERCENT1"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["PERCENT2"])))
                            cmdPDT.Parameters.Add("@PERCENT2", SqlDbType.Decimal).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@PERCENT2", SqlDbType.Decimal).Value = Convert.ToDecimal(dtPDT.Rows[ItemCount]["PERCENT2"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["DELIVERYTIME"])))
                            cmdPDT.Parameters.Add("@DELIVERYTIME", SqlDbType.Int).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@DELIVERYTIME", SqlDbType.Int).Value = Convert.ToInt16(dtPDT.Rows[ItemCount]["DELIVERYTIME"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["SEARCHAGAIN"])))
                            cmdPDT.Parameters.Add("@SEARCHAGAIN", SqlDbType.Int).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@SEARCHAGAIN", SqlDbType.Int).Value = Convert.ToInt16(dtPDT.Rows[ItemCount]["SEARCHAGAIN"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["PRICEUNIT"])))
                            cmdPDT.Parameters.Add("@PRICEUNIT", SqlDbType.Decimal).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@PRICEUNIT", SqlDbType.Decimal).Value = Convert.ToDecimal(dtPDT.Rows[ItemCount]["PRICEUNIT"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["RELATION"])))
                            cmdPDT.Parameters.Add("@RELATION", SqlDbType.Int).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@RELATION", SqlDbType.Int).Value = Convert.ToInt16(dtPDT.Rows[ItemCount]["RELATION"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["QUANTITYAMOUNTTO"])))
                            cmdPDT.Parameters.Add("@QUANTITYAMOUNTTO", SqlDbType.Decimal).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@QUANTITYAMOUNTTO", SqlDbType.Decimal).Value = Convert.ToDecimal(dtPDT.Rows[ItemCount]["QUANTITYAMOUNTTO"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["UNITID"])))
                            cmdPDT.Parameters.Add("@UNITID", SqlDbType.NVarChar, 10).Value = "";
                        else
                            cmdPDT.Parameters.Add("@UNITID", SqlDbType.NVarChar, 10).Value = Convert.ToString(dtPDT.Rows[ItemCount]["UNITID"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["MARKUP"])))
                            cmdPDT.Parameters.Add("@MARKUP", SqlDbType.Decimal).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@MARKUP", SqlDbType.Decimal).Value = Convert.ToDecimal(dtPDT.Rows[ItemCount]["MARKUP"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["ALLOCATEMARKUP"])))
                            cmdPDT.Parameters.Add("@ALLOCATEMARKUP", SqlDbType.Int).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@ALLOCATEMARKUP", SqlDbType.Int).Value = Convert.ToInt16(dtPDT.Rows[ItemCount]["ALLOCATEMARKUP"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["MODULE"])))
                            cmdPDT.Parameters.Add("@MODULE", SqlDbType.Int).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@MODULE", SqlDbType.Int).Value = Convert.ToInt16(dtPDT.Rows[ItemCount]["MODULE"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["INVENTDIMID"])))
                            cmdPDT.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = "";
                        else
                            cmdPDT.Parameters.Add("@INVENTDIMID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtPDT.Rows[ItemCount]["INVENTDIMID"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["DATAAREAID"])))
                            cmdPDT.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = "";
                        else
                            cmdPDT.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtPDT.Rows[ItemCount]["DATAAREAID"]);


                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["RECID"])))
                            cmdPDT.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtPDT.Rows[ItemCount]["RECID"]);

                        if (string.IsNullOrEmpty(Convert.ToString(dtPDT.Rows[ItemCount]["MAXIMUMRETAILPRICE_IN"])))
                            cmdPDT.Parameters.Add("@MAXIMUMRETAILPRICE_IN", SqlDbType.Decimal).Value = 0;
                        else
                            cmdPDT.Parameters.Add("@MAXIMUMRETAILPRICE_IN", SqlDbType.Decimal).Value = Convert.ToDecimal(dtPDT.Rows[ItemCount]["MAXIMUMRETAILPRICE_IN"]);

                        cmdPDT.CommandTimeout = 0;
                        cmdPDT.ExecuteNonQuery();
                        cmdPDT.Dispose();
                    }
                }
            }
        }

        private bool IsExistInLocal(string sItemId)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            string commandText = "SELECT top 1 isnull(ITEMID,'') ITEMID" +
                                 " from INVENTTABLE where  ITEMID ='" + sItemId + "'";


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (!string.IsNullOrEmpty(sResult))
                return true;
            else
                return false;
        }

        private static void SaveECORESPRODUCTMASTERCONFIG(DataTable dtEPMConfig, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtEPMConfig != null && dtEPMConfig.Rows.Count > 0)
            {
                string commandEPMConfig = " IF NOT EXISTS(SELECT TOP 1 RECID FROM ECORESPRODUCTMASTERCONFIGURATION " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [ECORESPRODUCTMASTERCONFIGURATION](RECID" +
                                                    ",CONFIGURATION" +
                                                    ",CONFIGPRODUCTDIMENSIONATTRIBUTE" +
                                                    ",CONFIGPRODUCTMASTER)" +
                                            " VALUES(@RECID" +
                                                    ",@CONFIGURATION" +
                                                    ",@CONFIGPRODUCTDIMENSIONATTRIBUTE" +
                                                    ",@CONFIGPRODUCTMASTER) END";

                for (int ItemCount = 0; ItemCount < dtEPMConfig.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdEPMConfig = new SqlCommand(commandEPMConfig, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMConfig.Rows[ItemCount]["RECID"])))
                        cmdEPMConfig.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEPMConfig.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMConfig.Rows[ItemCount]["RECID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMConfig.Rows[ItemCount]["CONFIGURATION"])))
                        cmdEPMConfig.Parameters.Add("@CONFIGURATION", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEPMConfig.Parameters.Add("@CONFIGURATION", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMConfig.Rows[ItemCount]["CONFIGURATION"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMConfig.Rows[ItemCount]["CONFIGPRODUCTDIMENSIONATTRIBUTE"])))
                        cmdEPMConfig.Parameters.Add("@CONFIGPRODUCTDIMENSIONATTRIBUTE", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEPMConfig.Parameters.Add("@CONFIGPRODUCTDIMENSIONATTRIBUTE", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMConfig.Rows[ItemCount]["CONFIGPRODUCTDIMENSIONATTRIBUTE"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMConfig.Rows[ItemCount]["CONFIGPRODUCTMASTER"])))
                        cmdEPMConfig.Parameters.Add("@CONFIGPRODUCTMASTER", SqlDbType.BigInt).Value = "";
                    else
                        cmdEPMConfig.Parameters.Add("@CONFIGPRODUCTMASTER", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMConfig.Rows[ItemCount]["CONFIGPRODUCTMASTER"]);

                    cmdEPMConfig.CommandTimeout = 0;
                    cmdEPMConfig.ExecuteNonQuery();
                    cmdEPMConfig.Dispose();
                }
            }
        }
        private static void SaveECORESPRODUCTMASTERCOLOR(DataTable dtEPMColor, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtEPMColor != null && dtEPMColor.Rows.Count > 0)
            {
                string commandEPMColor = " IF NOT EXISTS(SELECT TOP 1 RECID FROM ECORESPRODUCTMASTERCOLOR " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [ECORESPRODUCTMASTERCOLOR](RECID" +
                                                    ",COLOR" +
                                                    ",COLORPRODUCTDIMENSIONATTRIBUTE" +
                                                    ",COLORPRODUCTMASTER)" +
                                            " VALUES(@RECID" +
                                                    ",@COLOR" +
                                                    ",@COLORPRODUCTDIMENSIONATTRIBUTE" +
                                                    ",@COLORPRODUCTMASTER) END";

                for (int ItemCount = 0; ItemCount < dtEPMColor.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdEPMColor = new SqlCommand(commandEPMColor, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMColor.Rows[ItemCount]["RECID"])))
                        cmdEPMColor.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEPMColor.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMColor.Rows[ItemCount]["RECID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMColor.Rows[ItemCount]["COLOR"])))
                        cmdEPMColor.Parameters.Add("@COLOR", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEPMColor.Parameters.Add("@COLOR", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMColor.Rows[ItemCount]["COLOR"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMColor.Rows[ItemCount]["COLORPRODUCTDIMENSIONATTRIBUTE"])))
                        cmdEPMColor.Parameters.Add("@COLORPRODUCTDIMENSIONATTRIBUTE", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEPMColor.Parameters.Add("@COLORPRODUCTDIMENSIONATTRIBUTE", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMColor.Rows[ItemCount]["COLORPRODUCTDIMENSIONATTRIBUTE"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMColor.Rows[ItemCount]["COLORPRODUCTMASTER"])))
                        cmdEPMColor.Parameters.Add("@COLORPRODUCTMASTER", SqlDbType.BigInt).Value = "";
                    else
                        cmdEPMColor.Parameters.Add("@COLORPRODUCTMASTER", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMColor.Rows[ItemCount]["COLORPRODUCTMASTER"]);

                    cmdEPMColor.CommandTimeout = 0;
                    cmdEPMColor.ExecuteNonQuery();
                    cmdEPMColor.Dispose();
                }
            }
        }
        private static void SaveECORESPRODUCTMASTERSIZE(DataTable dtEPMSize, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtEPMSize != null && dtEPMSize.Rows.Count > 0)
            {
                string commandEPMSize = " IF NOT EXISTS(SELECT TOP 1 RECID FROM ECORESPRODUCTMASTERSIZE " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [ECORESPRODUCTMASTERSIZE](RECID" +
                                                    ",SIZE_" +
                                                    ",SIZEPRODUCTDIMENSIONATTRIBUTE" +
                                                    ",SIZEPRODUCTMASTER)" +
                                            " VALUES(@RECID" +
                                                    ",@SIZE" +
                                                    ",@SIZEPRODUCTDIMENSIONATTRIBUTE" +
                                                    ",@SIZEPRODUCTMASTER) END";

                for (int ItemCount = 0; ItemCount < dtEPMSize.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdEPMSize = new SqlCommand(commandEPMSize, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMSize.Rows[ItemCount]["RECID"])))
                        cmdEPMSize.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEPMSize.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMSize.Rows[ItemCount]["RECID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMSize.Rows[ItemCount]["SIZE"])))
                        cmdEPMSize.Parameters.Add("@SIZE", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEPMSize.Parameters.Add("@SIZE", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMSize.Rows[ItemCount]["SIZE"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMSize.Rows[ItemCount]["SIZEPRODUCTDIMENSIONATTRIBUTE"])))
                        cmdEPMSize.Parameters.Add("@SIZEPRODUCTDIMENSIONATTRIBUTE", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEPMSize.Parameters.Add("@SIZEPRODUCTDIMENSIONATTRIBUTE", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMSize.Rows[ItemCount]["SIZEPRODUCTDIMENSIONATTRIBUTE"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMSize.Rows[ItemCount]["SIZEPRODUCTMASTER"])))
                        cmdEPMSize.Parameters.Add("@SIZEPRODUCTMASTER", SqlDbType.BigInt).Value = "";
                    else
                        cmdEPMSize.Parameters.Add("@SIZEPRODUCTMASTER", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMSize.Rows[ItemCount]["SIZEPRODUCTMASTER"]);

                    cmdEPMSize.CommandTimeout = 0;
                    cmdEPMSize.ExecuteNonQuery();
                    cmdEPMSize.Dispose();
                }
            }
        }
        private static void SaveECORESPRODUCTMASTERSTYLE(DataTable dtEPMStyle, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtEPMStyle != null && dtEPMStyle.Rows.Count > 0)
            {
                string commandEPMStyle = " IF NOT EXISTS(SELECT TOP 1 RECID FROM ECORESPRODUCTMASTERSTYLE " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [ECORESPRODUCTMASTERSTYLE](RECID" +
                                                    ",STYLE" +
                                                    ",STYLEPRODUCTDIMENSIONATTRIBUTE" +
                                                    ",STYLEPRODUCTMASTER)" +
                                            " VALUES(@RECID" +
                                                    ",@STYLE" +
                                                    ",@STYLEPRODUCTDIMENSIONATTRIBUTE" +
                                                    ",@STYLEPRODUCTMASTER) END";

                for (int ItemCount = 0; ItemCount < dtEPMStyle.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdEPMStyle = new SqlCommand(commandEPMStyle, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMStyle.Rows[ItemCount]["RECID"])))
                        cmdEPMStyle.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEPMStyle.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMStyle.Rows[ItemCount]["RECID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMStyle.Rows[ItemCount]["STYLE"])))
                        cmdEPMStyle.Parameters.Add("@STYLE", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEPMStyle.Parameters.Add("@STYLE", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMStyle.Rows[ItemCount]["STYLE"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMStyle.Rows[ItemCount]["STYLEPRODUCTDIMENSIONATTRIBUTE"])))
                        cmdEPMStyle.Parameters.Add("@STYLEPRODUCTDIMENSIONATTRIBUTE", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEPMStyle.Parameters.Add("@STYLEPRODUCTDIMENSIONATTRIBUTE", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMStyle.Rows[ItemCount]["STYLEPRODUCTDIMENSIONATTRIBUTE"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEPMStyle.Rows[ItemCount]["STYLEPRODUCTMASTER"])))
                        cmdEPMStyle.Parameters.Add("@STYLEPRODUCTMASTER", SqlDbType.BigInt).Value = "";
                    else
                        cmdEPMStyle.Parameters.Add("@STYLEPRODUCTMASTER", SqlDbType.BigInt).Value = Convert.ToInt64(dtEPMStyle.Rows[ItemCount]["STYLEPRODUCTMASTER"]);

                    cmdEPMStyle.CommandTimeout = 0;
                    cmdEPMStyle.ExecuteNonQuery();
                    cmdEPMStyle.Dispose();
                }
            }
        }

        private static void SaveECORESCONFIG(DataTable dtEConfig, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtEConfig != null && dtEConfig.Rows.Count > 0)
            {
                string commandEConfig = " IF NOT EXISTS(SELECT TOP 1 RECID FROM ECORESCONFIGURATION " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [ECORESCONFIGURATION](RECID" +
                                                    ",NAME)" +
                                            " VALUES(@RECID" +
                                                    ",@NAME) END";

                for (int ItemCount = 0; ItemCount < dtEConfig.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdEConfig = new SqlCommand(commandEConfig, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEConfig.Rows[ItemCount]["RECID"])))
                        cmdEConfig.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEConfig.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtEConfig.Rows[ItemCount]["RECID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEConfig.Rows[ItemCount]["NAME"])))
                        cmdEConfig.Parameters.Add("@NAME", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdEConfig.Parameters.Add("@NAME", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtEConfig.Rows[ItemCount]["NAME"]);

                    cmdEConfig.CommandTimeout = 0;
                    cmdEConfig.ExecuteNonQuery();
                    cmdEConfig.Dispose();
                }
            }
        }
        private static void SaveECORESCOLOR(DataTable dtEColor, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtEColor != null && dtEColor.Rows.Count > 0)
            {
                string commandEColor = " IF NOT EXISTS(SELECT TOP 1 RECID FROM ECORESCOLOR " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [ECORESCOLOR](RECID" +
                                                    ",NAME)" +
                                            " VALUES(@RECID" +
                                                    ",@NAME) END";

                for (int ItemCount = 0; ItemCount < dtEColor.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdEColor = new SqlCommand(commandEColor, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEColor.Rows[ItemCount]["RECID"])))
                        cmdEColor.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEColor.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtEColor.Rows[ItemCount]["RECID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEColor.Rows[ItemCount]["NAME"])))
                        cmdEColor.Parameters.Add("@NAME", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdEColor.Parameters.Add("@NAME", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtEColor.Rows[ItemCount]["NAME"]);

                    cmdEColor.CommandTimeout = 0;
                    cmdEColor.ExecuteNonQuery();
                    cmdEColor.Dispose();
                }
            }
        }
        private static void SaveECORESSIZE(DataTable dtESize, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtESize != null && dtESize.Rows.Count > 0)
            {
                string commandESize = " IF NOT EXISTS(SELECT TOP 1 RECID FROM ECORESSIZE " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [ECORESSIZE](RECID" +
                                                    ",NAME)" +
                                            " VALUES(@RECID" +
                                                    ",@NAME) END";

                for (int ItemCount = 0; ItemCount < dtESize.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdESize = new SqlCommand(commandESize, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtESize.Rows[ItemCount]["RECID"])))
                        cmdESize.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdESize.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtESize.Rows[ItemCount]["RECID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtESize.Rows[ItemCount]["NAME"])))
                        cmdESize.Parameters.Add("@NAME", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdESize.Parameters.Add("@NAME", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtESize.Rows[ItemCount]["NAME"]);

                    cmdESize.CommandTimeout = 0;
                    cmdESize.ExecuteNonQuery();
                    cmdESize.Dispose();
                }
            }
        }
        private static void SaveECORESSTYLE(DataTable dtEStyle, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtEStyle != null && dtEStyle.Rows.Count > 0)
            {
                string commandEStyle = " IF NOT EXISTS(SELECT TOP 1 RECID FROM ECORESSTYLE " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [ECORESSTYLE](RECID" +
                                                    ",NAME)" +
                                            " VALUES(@RECID" +
                                                    ",@NAME) END";

                for (int ItemCount = 0; ItemCount < dtEStyle.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdEStyle = new SqlCommand(commandEStyle, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEStyle.Rows[ItemCount]["RECID"])))
                        cmdEStyle.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdEStyle.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtEStyle.Rows[ItemCount]["RECID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtEStyle.Rows[ItemCount]["NAME"])))
                        cmdEStyle.Parameters.Add("@NAME", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdEStyle.Parameters.Add("@NAME", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtEStyle.Rows[ItemCount]["NAME"]);

                    cmdEStyle.CommandTimeout = 0;
                    cmdEStyle.ExecuteNonQuery();
                    cmdEStyle.Dispose();
                }
            }
        }

        private static void SaveHSNCODETABLE_IN(DataTable dtHSN, SqlTransaction transaction, SqlConnection connection)
        {
            if (dtHSN != null && dtHSN.Rows.Count > 0) 
            {
                string commandHSN = " IF NOT EXISTS(SELECT TOP 1 RECID FROM HSNCODETABLE_IN " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [HSNCODETABLE_IN](RECID," +
                                                    "CODE,DATAAREAID)" +
                                            " VALUES(@RECID" +
                                                    ",@CODE,@DATAAREAID) END";

                for (int ItemCount = 0; ItemCount < dtHSN.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdHSN = new SqlCommand(commandHSN, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtHSN.Rows[ItemCount]["RECID"])))
                        cmdHSN.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdHSN.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtHSN.Rows[ItemCount]["RECID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtHSN.Rows[ItemCount]["CODE"])))
                        cmdHSN.Parameters.Add("@CODE", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdHSN.Parameters.Add("@CODE", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtHSN.Rows[ItemCount]["CODE"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtHSN.Rows[ItemCount]["DATAAREAID"])))
                        cmdHSN.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = "";
                    else
                        cmdHSN.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtHSN.Rows[ItemCount]["DATAAREAID"]);

                    cmdHSN.CommandTimeout = 0;
                    cmdHSN.ExecuteNonQuery();
                    cmdHSN.Dispose();
                }
            }
        }

        private static void SaveSACCODETABLE_IN(DataTable dtSAC, SqlTransaction transaction, SqlConnection connection) 
        {
            if (dtSAC != null && dtSAC.Rows.Count > 0)
            {
                string commandHSN = " IF NOT EXISTS(SELECT TOP 1 RECID FROM ServiceAccountingCodeTable_IN " +
                                            " WHERE RECID=@RECID ) BEGIN" +
                                            " INSERT INTO [ServiceAccountingCodeTable_IN](RECID," +
                                                    "SAC,DATAAREAID)" +
                                            " VALUES(@RECID" +
                                                    ",@SAC,@DATAAREAID) END";

                for (int ItemCount = 0; ItemCount < dtSAC.Rows.Count; ItemCount++)
                {
                    SqlCommand cmdSAC = new SqlCommand(commandHSN, connection, transaction);

                    if (string.IsNullOrEmpty(Convert.ToString(dtSAC.Rows[ItemCount]["RECID"])))
                        cmdSAC.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                    else
                        cmdSAC.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtSAC.Rows[ItemCount]["RECID"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtSAC.Rows[ItemCount]["SAC"])))
                        cmdSAC.Parameters.Add("@SAC", SqlDbType.NVarChar, 20).Value = "";
                    else
                        cmdSAC.Parameters.Add("@SAC", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtSAC.Rows[ItemCount]["SAC"]);

                    if (string.IsNullOrEmpty(Convert.ToString(dtSAC.Rows[ItemCount]["DATAAREAID"])))
                        cmdSAC.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = "";
                    else
                        cmdSAC.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = Convert.ToString(dtSAC.Rows[ItemCount]["DATAAREAID"]);

                    cmdSAC.CommandTimeout = 0;
                    cmdSAC.ExecuteNonQuery();
                    cmdSAC.Dispose();
                }
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
