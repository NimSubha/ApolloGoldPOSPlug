using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Windows.Forms;
using BlankOperations.WinFormsTouch;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.Collections.ObjectModel;
using System.IO;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using System.Text;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmExtendedPurchExch : frmTouchBase
    {
        DataTable dtExtndPurchExchng;
        string sItemId = string.Empty;
        DataTable dtBatchDetails = new DataTable();


        decimal dDiffAmt = 0m;
        string sBaseUnit1 = string.Empty;
        string sRate1 = string.Empty;
        string sConfigId1 = string.Empty;
        int iOWNDMD = 0;
        int iOWNOG = 0;
        int iOTHERDMD = 0;
        int iOTHEROG = 0;
        string sSelectedCustAcc = "";
        string sPConfigId = "";
        string sDailyBatchId = "";
        string sPItemId = "";
        string sPCustAcc = "";
        decimal dTolleranceRate = 0m;
        bool bReturn = false;
        int iDecPre = 0;
        int iRateType = 0;
        private IApplication application;
        int iMetalType = 0;
        int iPTransType = 0;
        int iCTransType = 0;
        decimal dOldStnPcs = 0;
        decimal dOldStnWt = 0m;
        decimal dOldGross = 0;
        decimal dOldNet = 0m;
        decimal dOldPcs = 0;

        decimal dPurityReading1 = 0;
        decimal dPurityReading2 = 0;
        decimal dPurityReading3 = 0;
        string sPurityPerson = string.Empty;
        string sPurityPersonName = string.Empty;

        decimal dMkAmt = 0;

        bool bIsCWItem = false;

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

        public enum MetalType
        {
            Other = 0,
            Gold = 1,
            Silver = 2,
            Platinum = 3,
            Alloy = 4,
            Diamond = 5,
            Pearl = 6,
            Stone = 7,
            Consumables = 8,
            Watch = 11,
            LooseDmd = 12,
            Palladium = 13,
            Jewellery = 14,
            Metal = 15,
            PackingMaterial = 16,
            Certificate = 17,
        }

        enum RateTypeNew
        {
            Purchase = 0,
            OGP = 1,
            OGOP = 2,
            Sale = 3,
            GSS = 4,
            Exchange = 6,
            OtherExchange = 8,
        }

        enum TransactionType
        {
            Sale = 0,
            Purchase = 1,
            Exchange = 3,
            PurchaseReturn = 2,
            ExchangeReturn = 4,
        }


        frmCustomFieldCalculations objCustomFieldCalculations;

        public frmExtendedPurchExch()
        {
            InitializeComponent();
        }

        public frmExtendedPurchExch(string sBaseItemID, string sBaseUnit, string sConfigId, string sRate,
            frmCustomFieldCalculations objCFC, DataTable dt, bool bIsReturn, string sCusAcc = "", int iRType = 0, int iCurrentTransType = 0)
        {
            InitializeComponent();
            bReturn = bIsReturn;
            sItemId = sBaseItemID;
            dtExtndPurchExchng = dt;
            iRateType = iRType;
            objCustomFieldCalculations = objCFC;

            sSelectedCustAcc = sCusAcc;
            sBaseUnit1 = sBaseUnit;
            sRate1 = sRate;
            sConfigId1 = sConfigId;

            txtGrossUnit.Text = sBaseUnit;
            txtNetUnit.Text = sBaseUnit;
            txtNetRate.Text = sRate;
            txtNetPurity.Text = sConfigId;

            txtDiamondUnit.Text = "CT";
            txtStoneUnit.Text = "Gms";
            txtPStoneUnit.Text = "CT";

            txtChangedAmt.Enabled = true;

            iMetalType = GetMetalType(sItemId);
            iCTransType = iCurrentTransType;

            GetItemType(sItemId);

            if (iMetalType == (int)MetalType.Jewellery)
            {
                decimal sRateJel = Convert.ToDecimal(GetMetalRate(sConfigId1));
                txtNetRate.Text = Convert.ToString(sRateJel);
                sRate1 = Convert.ToString(sRateJel);
                txtNetRate.Enabled = false;
                txtNetRate.BackColor = SystemColors.Control;
            }

            decimal dTollerance = getOGTolleranceRates();

            //if(dTollerance > 0 && !string.IsNullOrEmpty(sRate1))
            //    dTolleranceRate = Convert.ToDecimal(sRate1) * dTollerance / 100;

            txtToleranceRates.Text = Convert.ToString(dTollerance);

            bIsCWItem = IsCatchWtItem(sItemId);

            if (bIsCWItem)
                txtPcs.Enabled = true;
            else
                txtPcs.Enabled = false;

            if (bIsCWItem
                && (iMetalType == (int)MetalType.Gold
                    || iMetalType == (int)MetalType.Platinum
                    || iMetalType == (int)MetalType.Palladium
                    || iMetalType == (int)MetalType.Silver))
            {
                txtGrossWt.Enabled = false;
                txtStnRate.Enabled = false;
                txtStoneAmount.Enabled = false;
            }
            else
            {
                txtGrossWt.Enabled = true;
                txtStnRate.Enabled = true;
                txtStoneAmount.Enabled = true;
            }

            if (iMetalType == (int)MetalType.Stone
               || iMetalType == (int)MetalType.LooseDmd)
            {
                txtDiamondPcs.Text = txtPcs.Text;
                txtDiamondWt.Text = txtGrossWt.Text;
                txtDiamondPcs.Enabled = false;
                txtDiamondWt.Enabled = false;
                txtDiamondWt.BackColor = SystemColors.Control;
                txtDiamondPcs.BackColor = SystemColors.Control;
            }
            else
            {
                txtDiamondPcs.Enabled = true;
                txtDiamondWt.Enabled = true;
                txtDiamondWt.BackColor = SystemColors.Window;
                txtDiamondPcs.BackColor = SystemColors.Window;
            }

            if (iMetalType == (int)MetalType.Stone
                || iMetalType == (int)MetalType.LooseDmd)
            {
                txtChangedGrossWt.Enabled = false;
            }
            else
                txtChangedGrossWt.Enabled = true;

            iDecPre = GetDecimalPrecison(sBaseUnit1);



            if (dtExtndPurchExchng != null && dtExtndPurchExchng.Rows.Count > 0)
                SetValue();
            //txtRefInvoiceNo.Focus();

            if (bReturn)
            {
                grbDiamondWt.Enabled = false;
                grbGrossWt.Enabled = false;
                grbNetWt.Enabled = false;
                grbStoneWt.Enabled = false;
                txtBatchNo.Enabled = true;
                btnSearch.Enabled = true;
                txtChangedAmt.Enabled = false;
            }
            else
            {
                txtBatchNo.Enabled = false;
                btnSearch.Enabled = false;
                txtChangedAmt.Enabled = true;
            }

            if (iOWNDMD == 1 || iOTHERDMD == 1)
            {
                grbDiamondWt.Enabled = true;
                txtChangedAmt.Enabled = false;
            }
            else
            {
                grbDiamondWt.Enabled = false;
                txtChangedAmt.Enabled = true;
            }

            if (iOWNOG == 1 || iOWNDMD == 1)
                btnAvgPurity.Enabled = false;
            else
                btnAvgPurity.Enabled = true;

            if (iOWNDMD == 1)
                txtMkAmt.Enabled = true;
            else
                txtMkAmt.Enabled = false;
        }

        private void SetValue()
        {
            if (dtExtndPurchExchng != null && dtExtndPurchExchng.Rows.Count > 0)
            {
                txtRefInvoiceNo.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["REFINVOICENO"]);
                txtBatchNo.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["OGRECEIPTNO"]);
                LoadLine(txtBatchNo.Text);

                cmbLine.SelectedValue = Convert.ToString(dtExtndPurchExchng.Rows[0]["OGLINENUM"]);

                sDailyBatchId = Convert.ToString(dtExtndPurchExchng.Rows[0]["REFBATCHNO"]);
                txtPcs.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["PCS"]);
                txtGrossWt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["GROSSWT"]);
                txtChangedGrossWt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["CHANGEDGROSSWT"]);
                txtGrossUnit.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["GROSSUNIT"]);
                txtDiamondWt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDWT"]);
                txtDiamondPcs.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDPCS"]);
                txtDiamondUnit.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDUNIT"]);
                txtDmdRate.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDRATE"]);
                txtDiamondAmount.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["DMDAMOUNT"]);
                txtStoneWt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEWT"]);
                txtStonePcs.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEPCS"]);
                txtStoneUnit.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEUNIT"]);
                txtStnRate.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["STONERATE"]);
                txtStoneAmount.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["STONEAMOUNT"]);
                txtNetWt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["NETWT"]);
                txtNetRate.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["NETRATE"]);
                txtNetUnit.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["NETUNIT"]);
                txtNetPurity.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["NETPURITY"]);
                txtNetAmount.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["NETAMOUNT"]);
                txtGrossAmt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["GROSSAMT"]);
                txtActAmt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["ACTAMOUNT"]);
                txtChangedAmt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["CHANGEDAMOUNT"]);
                txtFinalAmt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["FINALAMT"]);
                chkRepair.Checked = Convert.ToBoolean(dtExtndPurchExchng.Rows[0]["ISREPAIR"]);
                txtRepairId.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["REPAIRBATCHID"]);
                //txtAvgPurity.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["REPAIRBATCHID"]);

                dPurityReading1 = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["PURITYREADING1"]);
                dPurityReading2 = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["PURITYREADING2"]);
                dPurityReading3 = Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["PURITYREADING3"]);

                sPurityPerson = Convert.ToString(dtExtndPurchExchng.Rows[0]["PURITYPERSON"]); ;
                sPurityPersonName = Convert.ToString(dtExtndPurchExchng.Rows[0]["PURITYPERSONNAME"]);

                txtPStoneWt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["OGPSTONEWT"]);
                txtPStonePcs.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["OGPSTONEPCS"]);
                txtPStoneUnit.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["OGPSTONEUNIT"]);
                txtPStoneRate.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["OGPSTONERATE"]);
                txtPStoneAmt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["OGPSTONEAMOUNT"]);
                txtMkAmt.Text = Convert.ToString(dtExtndPurchExchng.Rows[0]["OGPMAKINGAMOUNT"]);

                txtAvgPurity.Text = Convert.ToString(decimal.Round(((dPurityReading1 + dPurityReading2 + dPurityReading3) / 3), 4, MidpointRounding.AwayFromZero));
            }
        }

        private void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateTollerance())
            {
                if (!string.IsNullOrEmpty(txtNetWt.Text) && Convert.ToDecimal(txtNetWt.Text) > 0) // added on 09/07/2014 req by S.Sharma
                {
                    BuildDataTable();
                }
                else if ((iMetalType != (int)MetalType.Stone) && (iMetalType != (int)MetalType.LooseDmd))
                {
                    MessageBox.Show("Invalid Net Wt");
                }
                else
                    BuildDataTable();
            }
            //else
            //{
            //    MessageBox.Show("Invalid net rate, please check the tollerance rate");
            //}

        }

        private void BuildDataTable()
        {
            if (dtExtndPurchExchng == null || dtExtndPurchExchng.Columns.Count == 0)
            {
                dtExtndPurchExchng = new DataTable();
                dtExtndPurchExchng.Columns.Add("REFINVOICENO", typeof(string));
                dtExtndPurchExchng.Columns.Add("REFBATCHNO", typeof(string));
                dtExtndPurchExchng.Columns.Add("PCS", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("GROSSWT", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("CHANGEDGROSSWT", typeof(decimal));//
                dtExtndPurchExchng.Columns.Add("GROSSUNIT", typeof(string));
                dtExtndPurchExchng.Columns.Add("GROSSAMT", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("DMDWT", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("DMDPCS", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("DMDUNIT", typeof(string));
                dtExtndPurchExchng.Columns.Add("DMDRATE", typeof(decimal));//
                dtExtndPurchExchng.Columns.Add("DMDAMOUNT", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("STONEWT", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("STONEPCS", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("STONEUNIT", typeof(string));
                dtExtndPurchExchng.Columns.Add("STONERATE", typeof(decimal));//
                dtExtndPurchExchng.Columns.Add("STONEAMOUNT", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("NETWT", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("NETRATE", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("NETUNIT", typeof(string));
                dtExtndPurchExchng.Columns.Add("NETPURITY", typeof(string));
                dtExtndPurchExchng.Columns.Add("NETAMOUNT", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("CHANGEDAMOUNT", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("ACTAMOUNT", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("FINALAMT", typeof(decimal));//
                dtExtndPurchExchng.Columns.Add("ISREPAIR", typeof(int));
                dtExtndPurchExchng.Columns.Add("REPAIRBATCHID", typeof(string));
                dtExtndPurchExchng.Columns.Add("OGRECEIPTNO", typeof(string));
                dtExtndPurchExchng.Columns.Add("OGLINENUM", typeof(int));

                dtExtndPurchExchng.Columns.Add("PURITYREADING1", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("PURITYREADING2", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("PURITYREADING3", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("PURITYPERSON", typeof(string));
                dtExtndPurchExchng.Columns.Add("PURITYPERSONNAME", typeof(string));

                dtExtndPurchExchng.Columns.Add("OGPSTONEWT", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("OGPSTONEPCS", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("OGPSTONEUNIT", typeof(string));
                dtExtndPurchExchng.Columns.Add("OGPSTONERATE", typeof(decimal));//
                dtExtndPurchExchng.Columns.Add("OGPSTONEAMOUNT", typeof(decimal));
                dtExtndPurchExchng.Columns.Add("OGPMAKINGAMOUNT", typeof(decimal));


                //decimal dPurityReading1 = 0;
                //decimal dPurityReading2 = 0;
                //decimal dPurityReading3 = 0;
                //string sPurityPerson = string.Empty;
                //string sPurityPersonName = string.Empty;

                DataRow dr;
                dr = dtExtndPurchExchng.NewRow();

                dr["REFINVOICENO"] = txtRefInvoiceNo.Text.Trim();
                dr["REFBATCHNO"] = sDailyBatchId;// txtBatchNo.Text.Trim();

                dr["PCS"] = !string.IsNullOrEmpty(txtPcs.Text) ? decimal.Round(Convert.ToDecimal(txtPcs.Text), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["GROSSWT"] = !string.IsNullOrEmpty(txtGrossWt.Text) ? decimal.Round(Convert.ToDecimal(txtGrossWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["CHANGEDGROSSWT"] = !string.IsNullOrEmpty(txtChangedGrossWt.Text) ? decimal.Round(Convert.ToDecimal(txtChangedGrossWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["GROSSAMT"] = !string.IsNullOrEmpty(txtGrossAmt.Text) ? decimal.Round(Convert.ToDecimal(txtGrossAmt.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";

                dr["GROSSUNIT"] = txtGrossUnit.Text.Trim();
                dr["DMDWT"] = !string.IsNullOrEmpty(txtDiamondWt.Text) ? decimal.Round(Convert.ToDecimal(txtDiamondWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["DMDPCS"] = !string.IsNullOrEmpty(txtDiamondPcs.Text) ? decimal.Round(Convert.ToDecimal(txtDiamondPcs.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["DMDUNIT"] = txtDiamondUnit.Text.Trim();
                dr["DMDRATE"] = !string.IsNullOrEmpty(txtDmdRate.Text) ? decimal.Round(Convert.ToDecimal(txtDmdRate.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";

                dr["DMDAMOUNT"] = !string.IsNullOrEmpty(txtDiamondAmount.Text) ? decimal.Round(Convert.ToDecimal(txtDiamondAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["STONEWT"] = !string.IsNullOrEmpty(txtStoneWt.Text) ? decimal.Round(Convert.ToDecimal(txtStoneWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["STONEPCS"] = !string.IsNullOrEmpty(txtStonePcs.Text) ? decimal.Round(Convert.ToDecimal(txtStonePcs.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["STONEUNIT"] = txtStoneUnit.Text.Trim();
                dr["STONERATE"] = !string.IsNullOrEmpty(txtStnRate.Text) ? decimal.Round(Convert.ToDecimal(txtStnRate.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["STONEAMOUNT"] = !string.IsNullOrEmpty(txtStoneAmount.Text) ? decimal.Round(Convert.ToDecimal(txtStoneAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";

                dr["NETWT"] = !string.IsNullOrEmpty(txtNetWt.Text) ? decimal.Round(Convert.ToDecimal(txtNetWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["NETRATE"] = !string.IsNullOrEmpty(txtNetRate.Text) ? decimal.Round(Convert.ToDecimal(txtNetRate.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["NETUNIT"] = !string.IsNullOrEmpty(txtNetUnit.Text) ? txtNetUnit.Text.Trim() : "0";
                dr["NETPURITY"] = txtNetPurity.Text.Trim();
                dr["NETAMOUNT"] = !string.IsNullOrEmpty(txtNetAmount.Text) ? decimal.Round(Convert.ToDecimal(txtNetAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";

                dr["CHANGEDAMOUNT"] = !string.IsNullOrEmpty(txtChangedAmt.Text) ? decimal.Round(Convert.ToDecimal(txtChangedAmt.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["ACTAMOUNT"] = !string.IsNullOrEmpty(txtActAmt.Text) ? decimal.Round(Convert.ToDecimal(txtActAmt.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["FINALAMT"] = !string.IsNullOrEmpty(txtFinalAmt.Text) ? decimal.Round(Convert.ToDecimal(txtFinalAmt.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["ISREPAIR"] = Convert.ToInt16(chkRepair.Checked);
                dr["REPAIRBATCHID"] = txtRepairId.Text.Trim();

                dr["OGLINENUM"] = Convert.ToInt16(cmbLine.SelectedValue);
                dr["OGRECEIPTNO"] = txtBatchNo.Text.Trim();

                dr["PURITYREADING1"] = dPurityReading1;
                dr["PURITYREADING2"] = dPurityReading2;
                dr["PURITYREADING3"] = dPurityReading3;
                dr["PURITYPERSON"] = sPurityPerson;
                dr["PURITYPERSONNAME"] = sPurityPersonName;

                dr["OGPSTONEWT"] = !string.IsNullOrEmpty(txtPStoneWt.Text) ? decimal.Round(Convert.ToDecimal(txtPStoneWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["OGPSTONEPCS"] = !string.IsNullOrEmpty(txtPStonePcs.Text) ? decimal.Round(Convert.ToDecimal(txtPStonePcs.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["OGPSTONEUNIT"] = txtPStoneUnit.Text.Trim();
                dr["OGPSTONERATE"] = !string.IsNullOrEmpty(txtPStoneRate.Text) ? decimal.Round(Convert.ToDecimal(txtPStoneRate.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["OGPSTONEAMOUNT"] = !string.IsNullOrEmpty(txtPStoneAmt.Text) ? decimal.Round(Convert.ToDecimal(txtPStoneAmt.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dr["OGPMAKINGAMOUNT"] = !string.IsNullOrEmpty(txtMkAmt.Text) ? decimal.Round(Convert.ToDecimal(txtMkAmt.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";

                dtExtndPurchExchng.Rows.Add(dr);
            }
            else
            {
                dtExtndPurchExchng.Rows[0]["REFINVOICENO"] = txtRefInvoiceNo.Text.Trim();
                dtExtndPurchExchng.Rows[0]["REFBATCHNO"] = sDailyBatchId; // txtBatchNo.Text.Trim();
                dtExtndPurchExchng.Rows[0]["PCS"] = !string.IsNullOrEmpty(txtPcs.Text) ? decimal.Round(Convert.ToDecimal(txtPcs.Text), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["GROSSWT"] = !string.IsNullOrEmpty(txtGrossWt.Text) ? decimal.Round(Convert.ToDecimal(txtGrossWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["CHANGEDGROSSWT"] = !string.IsNullOrEmpty(txtChangedGrossWt.Text) ? decimal.Round(Convert.ToDecimal(txtChangedGrossWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["GROSSAMT"] = !string.IsNullOrEmpty(txtGrossAmt.Text) ? decimal.Round(Convert.ToDecimal(txtGrossAmt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["GROSSUNIT"] = txtGrossUnit.Text.Trim();
                dtExtndPurchExchng.Rows[0]["DMDWT"] = !string.IsNullOrEmpty(txtDiamondWt.Text) ? decimal.Round(Convert.ToDecimal(txtDiamondWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["DMDPCS"] = !string.IsNullOrEmpty(txtDiamondPcs.Text) ? decimal.Round(Convert.ToDecimal(txtDiamondPcs.Text), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["DMDUNIT"] = txtDiamondUnit.Text.Trim();
                dtExtndPurchExchng.Rows[0]["DMDRATE"] = !string.IsNullOrEmpty(txtDmdRate.Text) ? decimal.Round(Convert.ToDecimal(txtDmdRate.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";

                dtExtndPurchExchng.Rows[0]["DMDAMOUNT"] = !string.IsNullOrEmpty(txtDiamondAmount.Text) ? decimal.Round(Convert.ToDecimal(txtDiamondAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["STONEWT"] = !string.IsNullOrEmpty(txtStoneWt.Text) ? decimal.Round(Convert.ToDecimal(txtStoneWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["STONEPCS"] = !string.IsNullOrEmpty(txtStonePcs.Text) ? decimal.Round(Convert.ToDecimal(txtStonePcs.Text), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["STONEUNIT"] = txtStoneUnit.Text.Trim();
                dtExtndPurchExchng.Rows[0]["STONERATE"] = !string.IsNullOrEmpty(txtStnRate.Text) ? decimal.Round(Convert.ToDecimal(txtStnRate.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["STONEAMOUNT"] = !string.IsNullOrEmpty(txtStoneAmount.Text) ? decimal.Round(Convert.ToDecimal(txtStoneAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["NETWT"] = !string.IsNullOrEmpty(txtNetWt.Text) ? decimal.Round(Convert.ToDecimal(txtNetWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["NETRATE"] = !string.IsNullOrEmpty(txtNetRate.Text) ? decimal.Round(Convert.ToDecimal(txtNetRate.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["NETUNIT"] = !string.IsNullOrEmpty(txtNetUnit.Text) ? txtNetUnit.Text.Trim() : "0";
                dtExtndPurchExchng.Rows[0]["NETPURITY"] = txtNetPurity.Text.Trim();
                dtExtndPurchExchng.Rows[0]["NETAMOUNT"] = !string.IsNullOrEmpty(txtNetAmount.Text) ? decimal.Round(Convert.ToDecimal(txtNetAmount.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["CHANGEDAMOUNT"] = !string.IsNullOrEmpty(txtChangedAmt.Text) ? decimal.Round(Convert.ToDecimal(txtChangedAmt.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["ACTAMOUNT"] = !string.IsNullOrEmpty(txtActAmt.Text) ? decimal.Round(Convert.ToDecimal(txtActAmt.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["FINALAMT"] = !string.IsNullOrEmpty(txtFinalAmt.Text) ? decimal.Round(Convert.ToDecimal(txtFinalAmt.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["ISREPAIR"] = Convert.ToInt16(chkRepair.Checked);
                dtExtndPurchExchng.Rows[0]["REPAIRBATCHID"] = txtRepairId.Text.Trim();

                dtExtndPurchExchng.Rows[0]["OGLINENUM"] = Convert.ToInt16(cmbLine.SelectedValue);
                dtExtndPurchExchng.Rows[0]["OGRECEIPTNO"] = txtBatchNo.Text.Trim();

                dtExtndPurchExchng.Rows[0]["PURITYREADING1"] = dPurityReading1;
                dtExtndPurchExchng.Rows[0]["PURITYREADING2"] = dPurityReading2;
                dtExtndPurchExchng.Rows[0]["PURITYREADING3"] = dPurityReading3;
                dtExtndPurchExchng.Rows[0]["PURITYPERSON"] = sPurityPerson;
                dtExtndPurchExchng.Rows[0]["PURITYPERSONNAME"] = sPurityPersonName;

                dtExtndPurchExchng.Rows[0]["OGPSTONEWT"] = !string.IsNullOrEmpty(txtPStoneWt.Text) ? decimal.Round(Convert.ToDecimal(txtPStoneWt.Text), 3, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["OGPSTONEPCS"] = !string.IsNullOrEmpty(txtPStonePcs.Text) ? decimal.Round(Convert.ToDecimal(txtPStonePcs.Text), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["OGPSTONEUNIT"] = txtPStoneUnit.Text.Trim();
                dtExtndPurchExchng.Rows[0]["OGPSTONERATE"] = !string.IsNullOrEmpty(txtPStoneRate.Text) ? decimal.Round(Convert.ToDecimal(txtPStoneRate.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["OGPSTONEAMOUNT"] = !string.IsNullOrEmpty(txtPStoneAmt.Text) ? decimal.Round(Convert.ToDecimal(txtPStoneAmt.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";
                dtExtndPurchExchng.Rows[0]["OGPMAKINGAMOUNT"] = !string.IsNullOrEmpty(txtMkAmt.Text) ? decimal.Round(Convert.ToDecimal(txtMkAmt.Text), 2, MidpointRounding.AwayFromZero).ToString() : "0";

                dtExtndPurchExchng.AcceptChanges();
            }

            objCustomFieldCalculations.dtExtndPurchExchng = dtExtndPurchExchng;
            objCustomFieldCalculations.txtPCS.Text = txtPcs.Text;
            if (bIsCWItem)
            {
                objCustomFieldCalculations.txtTotalWeight.Text = txtChangedGrossWt.Text;// txtGrossWt.Text;
                objCustomFieldCalculations.txtQuantity.Text = txtGrossWt.Text;// txtChangedGrossWt.Text;// txtNetWt.Text;
                objCustomFieldCalculations.txtExpectedQuantity.Text = txtChangedGrossWt.Text;// txtNetWt.Text;
            }
            else
            {
                objCustomFieldCalculations.txtTotalWeight.Text = txtGrossWt.Text;// txtGrossWt.Text;
                objCustomFieldCalculations.txtQuantity.Text = txtGrossWt.Text;// txtNetWt.Text;
                objCustomFieldCalculations.txtExpectedQuantity.Text = txtGrossWt.Text;// txtNetWt.Text;
            }

            objCustomFieldCalculations.txtBatchNo.Text = sDailyBatchId;// txtBatchNo.Text;

            if (dtExtndPurchExchng != null && dtExtndPurchExchng.Rows.Count > 0)
            {
                decimal dTotalAmt = decimal.Round((Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["FINALAMT"])), 2, MidpointRounding.AwayFromZero);
                decimal dMkAmt = decimal.Round((Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["OGPMAKINGAMOUNT"])), 2, MidpointRounding.AwayFromZero);
                //+ Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["STONEAMOUNT"])
                //+ Convert.ToDecimal(dtExtndPurchExchng.Rows[0]["NETAMOUNT"])
                objCustomFieldCalculations.txtActToAmount.Text = Convert.ToString(dTotalAmt);
                objCustomFieldCalculations.txtRate.Text = Convert.ToString(dTotalAmt - dMkAmt);
                objCustomFieldCalculations.txtAmount.Text = Convert.ToString(dTotalAmt - dMkAmt);
                objCustomFieldCalculations.txtTotalAmount.Text = Convert.ToString(dTotalAmt);

                objCustomFieldCalculations.dPurityReading1 = dPurityReading1;
                objCustomFieldCalculations.dPurityReading2 = dPurityReading2;
                objCustomFieldCalculations.dPurityReading3 = dPurityReading3;
                objCustomFieldCalculations.sPurityPerson = sPurityPerson;
                objCustomFieldCalculations.sPurityPersonName = sPurityPersonName;

                if (!string.IsNullOrEmpty(txtMkAmt.Text))
                {
                    objCustomFieldCalculations.txtMakingAmount.Text = txtMkAmt.Text;
                    objCustomFieldCalculations.txtActMakingRate.Text = txtMkAmt.Text;
                    objCustomFieldCalculations.cmbMakingType.SelectedIndex = 2;
                }

            }
            else
            {
                objCustomFieldCalculations.txtActToAmount.Text = string.Empty;
                objCustomFieldCalculations.txtRate.Text = string.Empty;
                objCustomFieldCalculations.txtAmount.Text = string.Empty;
                objCustomFieldCalculations.txtTotalAmount.Text = string.Empty;

                objCustomFieldCalculations.dPurityReading1 = 0;
                objCustomFieldCalculations.dPurityReading2 = 0;
                objCustomFieldCalculations.dPurityReading3 = 0;
                objCustomFieldCalculations.sPurityPerson = sPurityPerson;
                objCustomFieldCalculations.sPurityPersonName = sPurityPersonName;

                objCustomFieldCalculations.txtMakingAmount.Text = string.Empty;
                objCustomFieldCalculations.txtActMakingRate.Text = string.Empty;
                objCustomFieldCalculations.cmbMakingType.SelectedIndex = 0;
            }

            objCustomFieldCalculations.txtLossWeight.Text = "0";

            this.Close();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            objCustomFieldCalculations.txtPCS.Text = string.Empty;
            objCustomFieldCalculations.txtTotalWeight.Text = string.Empty;
            objCustomFieldCalculations.txtQuantity.Text = string.Empty;
            objCustomFieldCalculations.txtExpectedQuantity.Text = string.Empty;
            objCustomFieldCalculations.txtRate.Text = txtNetRate.Text;
            objCustomFieldCalculations.txtAmount.Text = string.Empty;
            objCustomFieldCalculations.txtActToAmount.Text = string.Empty;
            objCustomFieldCalculations.txtTotalAmount.Text = string.Empty;
            this.Close();
        }

        #region
        private void txtGrossWt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }

            if (iDecPre == 0)
            {
                if (e.KeyChar == '.')
                {
                    e.Handled = true;
                }
            }

            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }

        }

        private void txtDiamondPcs_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void txtDiamondWt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }

        }

        private void txtDiamondAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }

        }

        private void txtStonePcs_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            //if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            //{
            //    e.Handled = true;
            //}
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }

        }

        private void txtStoneAmount_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void txtStoneWt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void txtNetWt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void txtNetRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }
        }
        #endregion

        private void txtGrossWt_Leave(object sender, EventArgs e)
        {
            if (bIsCWItem
              && (iMetalType == (int)MetalType.Gold
                  || iMetalType == (int)MetalType.Platinum
                  || iMetalType == (int)MetalType.Palladium
                  || iMetalType == (int)MetalType.Silver))
            {

                if (!string.IsNullOrEmpty(txtGrossWt.Text))
                {
                    string s = txtGrossWt.Text.ToString();
                    if (!IsNumeric(s))
                    {
                        string[] parts = s.Split('.');
                        Int64 iValue = 0;
                        if (!string.IsNullOrEmpty(parts[0]))
                        {
                            iValue = Int64.Parse(parts[0]);
                        }
                        Int64 i2 = Int64.Parse(parts[1]);

                        string sI2 = parts[1];
                        string sAfterDecimalValue = string.Empty;
                        if (sI2.ToString().Length > 3)
                        {
                            MessageBox.Show("Maximum decimal places is three.");
                            txtGrossWt.Focus();
                        }
                        else
                        {
                            CalculateNetWtAmount();
                        }
                    }
                    else
                    {
                        CalculateNetWtAmount();
                    }
                }
            }

            if (iMetalType == (int)MetalType.Stone
               || iMetalType == (int)MetalType.LooseDmd)
            {
                txtDiamondPcs.Text = txtPcs.Text;
                txtDiamondWt.Text = txtGrossWt.Text;
                txtDiamondPcs.Enabled = false;
                txtDiamondWt.Enabled = false;
                txtDiamondWt.BackColor = SystemColors.Control;
                txtDiamondPcs.BackColor = SystemColors.Control;
            }
            else
            {
                txtDiamondPcs.Enabled = true;
                txtDiamondWt.Enabled = true;
                txtDiamondWt.BackColor = SystemColors.Window;
                txtDiamondPcs.BackColor = SystemColors.Window;
            }
        }

        private void txtDiamondWt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtDiamondWt.Text))
            {
                string s = txtDiamondWt.Text.ToString();
                if (!IsNumeric(s))
                {
                    string[] parts = s.Split('.');
                    Int64 iValue = 0;
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        iValue = Int64.Parse(parts[0]);
                    }
                    Int64 i2 = Int64.Parse(parts[1]);

                    string sI2 = parts[1];
                    string sAfterDecimalValue = string.Empty;
                    if (sI2.ToString().Length > 3)
                    {
                        MessageBox.Show("Maximum decimal places is three.");
                        txtDiamondWt.Focus();
                    }
                    else
                    {
                        CalculateNetWtAmount();
                    }
                }
                else
                {
                    CalculateNetWtAmount();
                }
            }
            else     // SUBHA
            {
                txtDiamondWt.Text = "0";
                CalculateNetWtAmount();
            }
        }

        private void txtStoneWt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtStoneWt.Text))
            {
                string s = txtStoneWt.Text.ToString();
                if (!IsNumeric(s))
                {
                    string[] parts = s.Split('.');
                    Int64 iValue = 0;
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        iValue = Int64.Parse(parts[0]);
                    }
                    Int64 i2;

                    if (!string.IsNullOrEmpty((parts[1])))
                        i2 = Int64.Parse(parts[1]);

                    string sI2 = parts[1];
                    string sAfterDecimalValue = string.Empty;
                    if (sI2.ToString().Length > 3)
                    {
                        MessageBox.Show("Maximum decimal places is three.");
                        txtStoneWt.Focus();
                    }
                    else
                    {
                        CalculateNetWtAmount();
                        // CalculateFinalAmount();
                    }
                }
                else
                {
                    CalculateNetWtAmount();
                    //CalculateFinalAmount();
                }
            }
            else
            {
                CalculateNetWtAmount();
                //CalculateFinalAmount();
            }
        }

        private void frmExtendedPurchExch_Load(object sender, EventArgs e)
        {
            // txtRefInvoiceNo.Focus();
        }

        private void txtPcs_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private bool ValidateControls()
        {

            //sPTransType
            if (iCTransType == (int)TransactionType.ExchangeReturn)
            {
                if (iPTransType != (int)TransactionType.Exchange)
                {
                    MessageBox.Show("Invalid transaction", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }

            if (iCTransType == (int)TransactionType.PurchaseReturn)
            {
                if (iPTransType != (int)TransactionType.Purchase)
                {
                    MessageBox.Show("Invalid transaction", "Invalid transaction.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }

            if (sItemId.ToUpper() != sPItemId.ToUpper())// &&  && sSelectedCustAcc == sPCustAcc)
            {
                MessageBox.Show("Invalid item", "Invalid item.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (sPConfigId.ToUpper() != sConfigId1.ToUpper()) // added on 17/04/2014 for retail=1 item can be only sale trans
            {
                MessageBox.Show("Invalid item configuration.", "Invalid item configuration.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }
            if (sSelectedCustAcc.ToUpper() != sPCustAcc.ToUpper())
            {
                MessageBox.Show("Invalid customer selected.", "Invalid customer selected.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            if (!string.IsNullOrEmpty(txtBatchNo.Text))
            {
                if (IsReturendReceiptId(txtBatchNo.Text, Convert.ToInt16(cmbLine.SelectedValue)))
                {
                    MessageBox.Show("Already returned", "Already returned.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return false;
                }
            }

            return true;
        }

        private string GetMetalRate(string sConfigId1)
        {
            string sResult = "0";
            string storeId = string.Empty;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            storeId = ApplicationSettings.Database.StoreID;
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20) ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM RETAILCHANNELTABLE INNER JOIN  ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE RETAILSTORETABLE.STORENUMBER='" + storeId + "' ");//INVENTPARAMETERS

            commandText.Append(" SELECT TOP 1 CAST(RATES AS NUMERIC(28,2))AS RATES FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION ");
            commandText.Append(" AND METALTYPE in (1,2,3) AND ACTIVE=1 "); // METALTYPE -- > GOLD
            commandText.Append(" AND CONFIGIDSTANDARD='" + sConfigId1 + "'"); // RATETYPE -- > GSS->Sale 4->3 on 10/06/2016 req by S.Sharma  AND RETAIL=1

            if (iRateType == (int)RateTypeNew.Exchange) //  AND RATETYPE = " + iRateType + "
            {
                if (iOWNDMD == 1 || iOWNOG == 1)
                    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.Exchange + "' ");
                else if (iOTHERDMD == 1 || iOTHEROG == 1)
                    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OtherExchange + "' ");
            }
            else if (iRateType == (int)RateTypeNew.Purchase)
            {
                if (iOWNDMD == 1 || iOWNOG == 1)
                    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGP + "' ");
                else if (iOTHERDMD == 1 || iOTHEROG == 1)
                    commandText.Append(" AND RATETYPE='" + (int)RateTypeNew.OGOP + "' ");
            }

            commandText.Append(" ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            return Convert.ToString(sResult.Trim());
        }

        private bool ValidateTollerance()
        {
            bool bResult = true;
            decimal sRate = 0m;
            if (!string.IsNullOrEmpty(sConfigId1))
                sRate = Convert.ToDecimal(GetMetalRate(sConfigId1));
            else
                sRate = 0m;

            //if (!bReturn)
            //{
            if (string.IsNullOrEmpty(txtBatchNo.Text) && !bReturn)
            {
                if (!string.IsNullOrEmpty(txtNetRate.Text))
                {
                    decimal dNetRate = Convert.ToDecimal(txtNetRate.Text);
                    decimal dTollerance = Convert.ToDecimal(txtToleranceRates.Text);
                    decimal dSalesRate = getSaleRateFromMetalTable(sItemId, sConfigId1);
                    //===================Soutik=========================================
                    decimal dPlusTollTotRate = 0m;
                    decimal dMinusTollTotRate = 0m;
                    decimal dAveragePurityQty = 0m;
                    if (!string.IsNullOrEmpty(txtAvgPurity.Text))
                        dAveragePurityQty = Convert.ToDecimal(txtAvgPurity.Text);

                    if (dAveragePurityQty > 0)
                    {
                        decimal dAverageCalculateRate = GetAverageRate(sConfigId1, dAveragePurityQty);
                        dPlusTollTotRate = dAverageCalculateRate + dTollerance;// Soutik
                        dMinusTollTotRate = dAverageCalculateRate - dTollerance;
                    }
                    //==================================================================
                    else
                    {
                        dPlusTollTotRate = sRate + dTollerance;// open on 240419
                        dMinusTollTotRate = sRate - dTollerance;
                    }

                    if (iOTHEROG == 1 || iOTHERDMD == 1)
                    {
                        if (string.IsNullOrEmpty(txtAvgPurity.Text) || txtAvgPurity.Text == "0")
                        {
                            MessageBox.Show("Average purity is mandatory");
                            btnAvgPurity.Focus();
                            bResult = false;
                        }
                    }

                    #region Old
                    /*if ((iOWNOG == 1 || iOWNDMD == 1 || iOTHEROG == 1 || iOTHERDMD == 1)
                        && (iCTransType == (int)TransactionType.ExchangeReturn
                            || iCTransType == (int)TransactionType.Exchange))
                    {
                        if ((iMetalType == (int)MetalType.Gold
                           || iMetalType == (int)MetalType.Platinum
                           || iMetalType == (int)MetalType.Palladium
                           || iMetalType == (int)MetalType.Silver))
                        {
                            if ((Convert.ToDecimal(dNetRate) < dMinusTollTotRate || Convert.ToDecimal(dNetRate) > sRate))//sRate ,,dPlusTollTotRate
                            {
                                MessageBox.Show("Invalid net rate, please check the tollerance rate");
                                bResult = false;
                            }
                        }
                    }
                    else
                    {
                        if ((iMetalType == (int)MetalType.Gold
                            || iMetalType == (int)MetalType.Platinum
                            || iMetalType == (int)MetalType.Palladium
                            || iMetalType == (int)MetalType.Silver))
                        {
                            if ((Convert.ToDecimal(dNetRate) < dMinusTollTotRate || Convert.ToDecimal(dNetRate) > dPlusTollTotRate))//|| sRate < dNetRate
                            {
                                MessageBox.Show("Invalid net rate, please check the tollerance rate");
                                bResult = false;
                            }
                        }
                    }*/
                    #endregion

                    #region Now
                    if ((iOWNOG == 1 || iOWNDMD == 1 || iOTHEROG == 1 || iOTHERDMD == 1)
                        && (iCTransType == (int)TransactionType.ExchangeReturn
                            || iCTransType == (int)TransactionType.Exchange))
                    {
                        if ((iMetalType == (int)MetalType.Gold
                           || iMetalType == (int)MetalType.Platinum
                           || iMetalType == (int)MetalType.Palladium
                           || iMetalType == (int)MetalType.Silver))
                        {
                            if ((Convert.ToDecimal(dNetRate) < dMinusTollTotRate) || (Convert.ToDecimal(dNetRate) > dPlusTollTotRate))
                            {
                                MessageBox.Show("Invalid net rate, please check the tollerance rate");
                                bResult = false;
                            }
                            else if (dSalesRate > sRate)
                            {
                                if (dSalesRate > 0 && dSalesRate < dNetRate)
                                {
                                    MessageBox.Show("Invalid net rate, please check the board rate");
                                    bResult = false;
                                }
                                if (dSalesRate == 0)
                                {
                                    MessageBox.Show("Invalid net rate, please check the tollerance rate");
                                    bResult = false;
                                }
                            }
                            else if (dSalesRate > 0 && dSalesRate < Convert.ToDecimal(dNetRate))
                            {
                                MessageBox.Show("Invalid net rate, please check the board rate or tollerance rate");
                                bResult = false;
                            }
                        }
                    }
                    else
                    {
                        if ((iMetalType == (int)MetalType.Gold
                            || iMetalType == (int)MetalType.Platinum
                            || iMetalType == (int)MetalType.Palladium
                            || iMetalType == (int)MetalType.Silver) )
                        {
                            if ((Convert.ToDecimal(dNetRate) < dMinusTollTotRate) || Convert.ToDecimal(dNetRate) > dPlusTollTotRate)//|| sRate < dNetRate
                            {
                                MessageBox.Show("Invalid net rate, please check the tollerance rate");
                                bResult = false;
                            }
                            else if (dSalesRate > dPlusTollTotRate)
                            {
                                if (dSalesRate > 0 && dSalesRate < dNetRate)
                                {
                                    MessageBox.Show("Invalid net rate, please check the board rate");
                                    bResult = false;
                                }
                                if (dSalesRate == 0)
                                {
                                    MessageBox.Show("Invalid net rate, please check the tollerance rate");
                                    bResult = false;
                                }
                            }
                            else if (dSalesRate > 0 && dSalesRate < Convert.ToDecimal(dNetRate))
                            {
                                MessageBox.Show("Invalid net rate, please check the board rate or tollerance rate");
                                bResult = false;
                            }
                        }
                    }
                    #endregion
                }
            }
            // }
            if (bIsCWItem)
            {
                if (!string.IsNullOrEmpty(txtPcs.Text))
                {
                    if (Convert.ToDecimal(txtPcs.Text) <= 0)
                    {
                        MessageBox.Show("Catch weight enabled item should have the pcs value greater than zero");
                        bResult = false;
                    }
                }
                else
                {
                    MessageBox.Show("Catch weight enabled item should have a valid pcs value.");
                    bResult = false;
                }
            }
            if ((iMetalType != (int)MetalType.Stone) && (iMetalType != (int)MetalType.LooseDmd))
            {
                if (Convert.ToDecimal(sRate) <= 0)
                {
                    MessageBox.Show("Rate is not defined for this transaction");
                    bResult = false;
                }
            }

            //if (iOTHEROG == 1 || iOTHERDMD == 1)
            //{
            //    if (string.IsNullOrEmpty(txtAvgPurity.Text))
            //    {
            //        MessageBox.Show("Average purity is mandatory");
            //        btnAvgPurity.Focus();
            //        bResult = false;
            //    }
            //}

            if (!string.IsNullOrEmpty(txtDiamondWt.Text))
            {
                decimal dDmdWT = Convert.ToDecimal(txtDiamondWt.Text);

                if (dDmdWT > 0)
                {
                    if (Convert.ToDecimal(txtDiamondPcs.Text) <= 0)
                    {
                        MessageBox.Show("Diamond pcs is mandatory for this transaction");
                        txtDiamondPcs.Focus();
                        bResult = false;
                    }
                }
            }

            if (!string.IsNullOrEmpty(txtRepairId.Text))
            {
                string sRepBatchId = Convert.ToString(txtRepairId.Text);

                if (!IsValidBatchId(sRepBatchId, sSelectedCustAcc))
                {
                    MessageBox.Show("Please enter a valid batch id");
                    bResult = false;
                }
            }

            #region Variable
            decimal dStnWT = 0m;
            decimal dStnPcs = 0m;
            decimal dGrossWT = 0m;
            decimal dNetWt = 0m;
            decimal dPcs = 0m;

            if (!string.IsNullOrEmpty(txtPcs.Text))
                dPcs = Convert.ToDecimal(txtPcs.Text);

            if (!string.IsNullOrEmpty(txtChangedGrossWt.Text))
                dGrossWT = Convert.ToDecimal(txtChangedGrossWt.Text);

            if (!string.IsNullOrEmpty(txtNetWt.Text))
                dNetWt = Convert.ToDecimal(txtNetWt.Text);

            if (!string.IsNullOrEmpty(txtStonePcs.Text))
                dStnPcs = Convert.ToDecimal(txtStonePcs.Text);

            if (!string.IsNullOrEmpty(txtStoneWt.Text))
                dStnWT = Convert.ToDecimal(txtStoneWt.Text);
            #endregion

            #region Stone
            if (!string.IsNullOrEmpty(txtStoneWt.Text))
            {
                if (dStnWT > 0)
                {
                    if (dStnPcs <= 0)
                    {
                        MessageBox.Show("Stone pcs is mandatory for this transaction");
                        txtStonePcs.Focus();
                        bResult = false;
                    }

                    if (bReturn)
                    {
                        if (dStnPcs > dOldStnPcs)
                        {
                            MessageBox.Show("Stone pcs exceeding from actual pcs");
                            txtStonePcs.Focus();
                            bResult = false;
                        }

                        if (dStnPcs == dOldStnPcs)
                        {
                            if (dStnWT != dOldStnWt)
                            {
                                MessageBox.Show("Stone weight mismatch from actual weight");
                                txtStoneWt.Focus();
                                bResult = false;
                            }
                        }

                        if (dStnWT == dOldStnWt)
                        {
                            if (dStnPcs != dOldStnPcs)
                            {
                                MessageBox.Show("Stone pcs mismatch from actual pcs");
                                txtStonePcs.Focus();
                                bResult = false;
                            }
                        }
                    }
                }
            }
            #endregion

            #region Gross
            if ((iMetalType == (int)MetalType.Gold))
            {
                if (dPcs > 0 && dGrossWT > 0 && dNetWt > 0)
                {
                    if (bReturn)
                    {
                        if ((dStnWT + dNetWt) != dGrossWT)
                        {
                            MessageBox.Show("Gross weight mismatch from actual weight");
                            txtPcs.Focus();
                            bResult = false;
                        }

                        if (dGrossWT == dOldGross)
                        {
                            if (dPcs != dOldPcs)
                            {
                                MessageBox.Show("Pcs mismatch from actual pcs");
                                txtPcs.Focus();
                                bResult = false;
                            }
                        }

                        if (dPcs == dOldPcs)
                        {
                            if (dGrossWT != dOldGross)
                            {
                                MessageBox.Show("Gross weight mismatch from actual weight");
                                txtChangedGrossWt.Focus();
                                bResult = false;
                            }
                        }

                        if (dGrossWT == dOldGross && dPcs != dOldPcs)
                        {
                            if (dNetWt != dOldNet)
                            {
                                MessageBox.Show("Net weight mismatch from actual weight");
                                txtNetWt.Focus();
                                bResult = false;
                            }
                        }
                    }
                }
            }

            #endregion
            return bResult;
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            //if (!string.IsNullOrEmpty(txtBatchNo.Text))
            //{
            //    if (iCTransType == (int)TransactionType.PurchaseReturn)
            //        GetPurchasedItem(txtBatchNo.Text, (int)TransactionType.Purchase);
            //    else if (iCTransType == (int)TransactionType.ExchangeReturn)
            //        GetPurchasedItem(txtBatchNo.Text, (int)TransactionType.Exchange);
            //    else
            //        MessageBox.Show("Invalid transaction");
            //}

            if (!string.IsNullOrEmpty(txtBatchNo.Text) && Convert.ToInt16(cmbLine.SelectedValue) != 0)
            {
                if (iCTransType == (int)TransactionType.PurchaseReturn)
                    GetPurchasedItem(txtBatchNo.Text, Convert.ToInt16(cmbLine.SelectedValue));
                else if (iCTransType == (int)TransactionType.ExchangeReturn)
                    GetPurchasedItem(txtBatchNo.Text, Convert.ToInt16(cmbLine.SelectedValue));
                else
                    MessageBox.Show("Invalid transaction");
            }

            if (ValidateControls())
            {
                if (IsBatchItem(sItemId))
                {
                    if (!string.IsNullOrEmpty(txtBatchNo.Text))
                    {
                        if (IsValidReceiptNo(txtBatchNo.Text, Convert.ToInt16(cmbLine.SelectedValue))) //IsValidBatchNo(txtBatchNo.Text)
                        {
                            if (dtBatchDetails != null && dtBatchDetails.Rows.Count > 0)
                            {
                                foreach (DataRow dr in dtBatchDetails.Rows)
                                {
                                    txtRefInvoiceNo.Text = Convert.ToString(dr["REFINVOICENO"]);
                                    txtDiamondPcs.Text = !string.IsNullOrEmpty(Convert.ToString(dr["OGDMDPCS"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["OGDMDPCS"])), 0, MidpointRounding.AwayFromZero).ToString() : "0";

                                    txtDiamondWt.Text = Convert.ToString(dr["OGDMDWT"]);
                                    txtDiamondAmount.Text = Convert.ToString(dr["OGDMDAMT"]);
                                    txtStonePcs.Text = !string.IsNullOrEmpty(Convert.ToString(dr["OGSTONEPCS"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["OGSTONEPCS"])), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                                    txtStoneWt.Text = Convert.ToString(dr["OGSTONEWT"]);
                                    txtMkAmt.Text = Convert.ToString(dr["MAKINGAMOUNT"]);

                                    txtPStonePcs.Text = !string.IsNullOrEmpty(Convert.ToString(dr["OGPSTONEPCS"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["OGPSTONEPCS"])), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                                    txtPStoneRate.Text = !string.IsNullOrEmpty(Convert.ToString(dr["OGPSTONERATE"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["OGPSTONERATE"])), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                                    txtPStoneWt.Text = !string.IsNullOrEmpty(Convert.ToString(dr["OGPSTONEWT"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["OGPSTONEWT"])), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                                    txtPStoneAmt.Text = !string.IsNullOrEmpty(Convert.ToString(dr["OGPSTONEAMOUNT"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["OGPSTONEAMOUNT"])), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                                    txtPStoneUnit.Text = Convert.ToString(dr["OGPSTONEUNIT"]);



                                    if (!string.IsNullOrEmpty(txtStonePcs.Text))
                                        dOldStnPcs = Convert.ToDecimal(txtStonePcs.Text);

                                    if (!string.IsNullOrEmpty(txtStoneWt.Text))
                                        dOldStnWt = Convert.ToDecimal(txtStoneWt.Text);

                                    txtNetWt.Text = Convert.ToString(dr["OGNETWT"]);//4001R106IR0008021
                                    txtStoneAmount.Text = Convert.ToString(dr["OGSTONEAMT"]);
                                    txtGrossWt.Text = Convert.ToString(dr["TOTALWEIGHT"]);



                                    // PURITYREADING1,PURITYREADING2,PURITYREADING3,PURITYPERSON,PURITYPERSONNAME
                                    dPurityReading1 = Convert.ToDecimal(dr["PURITYREADING1"]);
                                    dPurityReading2 = Convert.ToDecimal(dr["PURITYREADING2"]);
                                    dPurityReading3 = Convert.ToDecimal(dr["PURITYREADING3"]);

                                    sPurityPerson = Convert.ToString(dr["PURITYPERSON"]); ;
                                    sPurityPersonName = Convert.ToString(dr["PURITYPERSONNAME"]);

                                    txtAvgPurity.Text = Convert.ToString(decimal.Round(((dPurityReading1 + dPurityReading2 + dPurityReading3) / 3), 4, MidpointRounding.AwayFromZero));
                                    decimal dAvgPurity = (!string.IsNullOrEmpty(txtAvgPurity.Text.Trim())) ? Convert.ToDecimal(txtAvgPurity.Text.Trim()) : 0m;

                                    if (dAvgPurity > 0)
                                        txtGrossWt.Text = Convert.ToString(decimal.Round(((Convert.ToDecimal(dr["QUANTITY"]))), 4, MidpointRounding.AwayFromZero));// * dAvgPurity
                                    else
                                        txtGrossWt.Text = Convert.ToString(dr["TOTALWEIGHT"]);

                                    //if (dChangedGrossWt > 0)
                                    //    dNetWt = decimal.Round((dChangedGrossWt - dDmdWt - dStoneWt), 3, MidpointRounding.AwayFromZero);

                                    //if (dChangedGrossWt > 0 && dAvgPurity > 0)
                                    //    dNetWt = decimal.Round((dChangedGrossWt - dDmdWt - dStoneWt) * dAvgPurity, 3, MidpointRounding.AwayFromZero);


                                    if (!string.IsNullOrEmpty(txtNetWt.Text))
                                        dOldNet = Convert.ToDecimal(txtNetWt.Text);

                                    if (!string.IsNullOrEmpty(txtGrossWt.Text))
                                        dOldGross = Convert.ToDecimal(txtGrossWt.Text);


                                    if (bIsCWItem
                                        && (iMetalType == (int)MetalType.Gold
                                            || iMetalType == (int)MetalType.Platinum
                                            || iMetalType == (int)MetalType.Palladium
                                            || iMetalType == (int)MetalType.Silver))
                                    {
                                        txtChangedGrossWt.Text = Convert.ToString(dr["OGCHANGEDGROSSWT"]);
                                        txtPcs.Text = !string.IsNullOrEmpty(Convert.ToString(dr["PIECES"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["PIECES"])), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                                    }
                                    else
                                    {
                                        txtChangedGrossWt.Text = Convert.ToString(dr["OGCHANGEDGROSSWT"]);
                                        int dPcs = Convert.ToInt16(!string.IsNullOrEmpty(Convert.ToString(dr["PIECES"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["PIECES"])), 0, MidpointRounding.AwayFromZero).ToString() : "0");
                                        txtPcs.Text = Convert.ToString(dPcs);
                                    }

                                    if (!string.IsNullOrEmpty(txtPcs.Text))
                                        dOldPcs = Convert.ToDecimal(txtPcs.Text);

                                    txtNetPurity.Text = Convert.ToString(dr["CONFIGID"]);
                                    txtStnRate.Text = Convert.ToString(dr["OGSTONEATE"]);// OGSTONEATE,OGGROSSAMT,OGACTAMT
                                    txtGrossAmt.Text = Convert.ToString(dr["OGGROSSAMT"]);
                                    txtActAmt.Text = Convert.ToString(dr["OGACTAMT"]);




                                    decimal dDmdAmt = Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(dr["OGDMDAMT"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["OGDMDAMT"])), 0, MidpointRounding.AwayFromZero).ToString() : "0");
                                    decimal dDmdWt = Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(dr["OGDMDWT"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["OGDMDWT"])), 0, MidpointRounding.AwayFromZero).ToString() : "0");

                                    if (dDmdWt > 0)
                                        txtDmdRate.Text = Convert.ToString(decimal.Round((dDmdAmt / dDmdWt), 2, MidpointRounding.AwayFromZero));//Convert.ToString(dCRate / dOGNETWT);


                                    //decimal dTotAmt = Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(dr["TOTALAMOUNT"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["TOTALAMOUNT"])), 0, MidpointRounding.AwayFromZero).ToString() : "0");

                                    //decimal dStnAmt = Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(dr["OGSTONEAMT"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["OGSTONEAMT"])), 0, MidpointRounding.AwayFromZero).ToString() : "0");// Convert.ToString(dr["OGSTONEATE"]);
                                    //decimal dCRate = Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(dr["CRate"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["CRate"])), 0, MidpointRounding.AwayFromZero).ToString() : "0");
                                    //decimal dOGNETWT = Convert.ToDecimal(!string.IsNullOrEmpty(Convert.ToString(dr["OGNETWT"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["OGNETWT"])), 0, MidpointRounding.AwayFromZero).ToString() : "0");


                                    //if(dCRate > 0 && dOGNETWT > 0)
                                    //    txtNetRate.Text = Convert.ToString(decimal.Round(((dCRate - dStnAmt - dDmdAmt) / dOGNETWT), 2, MidpointRounding.AwayFromZero));//Convert.ToString(dCRate / dOGNETWT);
                                    //else
                                    //    txtNetRate.Text = !string.IsNullOrEmpty(Convert.ToString(dr["CRate"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["CRate"])), 0, MidpointRounding.AwayFromZero).ToString() : "0";


                                    txtNetRate.Text = Convert.ToString(dr["OGNETRATE"]);
                                    decimal dNetRate = 0m;
                                    if (!string.IsNullOrEmpty(txtNetRate.Text))
                                        dNetRate = Convert.ToDecimal(txtNetRate.Text);

                                    sRate1 = Convert.ToString(dNetRate);
                                    // txtNetAmount.Text = Convert.ToString(decimal.Round((dNetRate * dOGNETWT), 2, MidpointRounding.AwayFromZero));

                                    txtNetAmount.Text = Convert.ToString(dr["OGNETAMT"]);
                                    txtFinalAmt.Text = Convert.ToString(dr["TOTALAMOUNT"]);
                                }

                                /// CalculateNetWtAmount();
                                txtRefInvoiceNo.Enabled = false;
                                //txtPcs.Enabled = false;
                                //txtGrossWt.Enabled = false;

                                txtDiamondWt.Enabled = false;
                                txtDiamondPcs.Enabled = false;

                                txtDiamondAmount.Enabled = false;
                                //txtStoneWt.Enabled = false; // blocked on 11/05/17 by S.Sharma
                                //txtStonePcs.Enabled = false;

                                txtStoneAmount.Enabled = false;
                                txtNetWt.Enabled = false;
                                txtNetRate.Enabled = false;

                                txtNetPurity.Enabled = false;
                                txtNetAmount.Enabled = false;
                            }

                        }
                        else
                        {
                            MessageBox.Show("Invalid Receipt no");
                            txtRefInvoiceNo.Text = "";
                            txtDiamondPcs.Text = "";
                            txtPcs.Text = "";
                            txtDiamondWt.Text = "";
                            txtDiamondAmount.Text = "";
                            txtStonePcs.Text = "";
                            txtStoneWt.Text = "";
                            txtNetWt.Text = "";
                            txtStoneAmount.Text = "";
                            txtGrossWt.Text = "";
                            txtNetPurity.Text = sConfigId1;
                            txtNetRate.Text = sRate1;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Please enter a valid receipt no");
                    }
                }
            }
        }

        private bool IsValidBatchNo(string sBatchId)
        {
            bool bReturnPcsValid = false;

            SqlConnection connection = new SqlConnection();


            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            SqlCommand SqlComm = new SqlCommand();
            SqlComm.Connection = connection;
            SqlComm.CommandType = CommandType.Text;

            SqlComm.CommandText = "select REFINVOICENO,QUANTITY,PIECES,AMOUNT,TOTALWEIGHT,EXPECTEDQUANTITY,CRate," +
                                " CONFIGID,OGDMDPCS,OGGROSSUNIT,OGDMDWT,OGDMDUNIT, " +
                                " OGDMDAMT,OGSTONEPCS,OGSTONEWT,OGSTONEUNIT,OGSTONEAMT,OGNETWT,OGNETUNIT,TOTALAMOUNT" +
                                " , OGSTONEATE,OGGROSSAMT,OGACTAMT,OGNETRATE,OGNETAMT,OGCHANGEDGROSSWT " +
                                " from RETAIL_CUSTOMCALCULATIONS_TABLE a " +
                                " left join RETAILTRANSACTIONSALESTRANS B on a.TRANSACTIONID=b.TRANSACTIONID" +
                                " and a.STOREID=b.STORE and a.TERMINALID = b.TERMINALID and a.LINENUM=b.LINENUM" +
                                " WHERE b.INVENTBATCHID = '" + sBatchId + "' and QTY>0";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            DataTable dtPurchaseBathDetails = new DataTable();

            SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
            SqlDa.Fill(dtPurchaseBathDetails);

            //decimal dPcs = Convert.ToDecimal(command.ExecuteScalar());

            dtBatchDetails = dtPurchaseBathDetails;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (dtPurchaseBathDetails != null && dtPurchaseBathDetails.Rows.Count > 0)
                bReturnPcsValid = true;


            return bReturnPcsValid;
        }

        private bool IsValidReceiptNo(string sReceiptId, int LineNo)
        {
            bool bReturnPcsValid = false;

            SqlConnection connection = new SqlConnection();


            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            SqlCommand SqlComm = new SqlCommand();
            SqlComm.Connection = connection;
            SqlComm.CommandType = CommandType.Text;

            SqlComm.CommandText = "select REFINVOICENO,QUANTITY,PIECES,AMOUNT,TOTALWEIGHT,EXPECTEDQUANTITY,CRate," +
                                " CONFIGID,OGDMDPCS,OGGROSSUNIT,OGDMDWT,OGDMDUNIT, " +
                                " OGDMDAMT,OGSTONEPCS,OGSTONEWT,OGSTONEUNIT,OGSTONEAMT,OGNETWT,OGNETUNIT,TOTALAMOUNT," +
                                " PURITYREADING1,PURITYREADING2,PURITYREADING3,PURITYPERSON,PURITYPERSONNAME," +
                                " OGSTONEATE,OGGROSSAMT,OGACTAMT,OGNETRATE,OGNETAMT,OGCHANGEDGROSSWT,MAKINGAMOUNT " +
                                ",OGPSTONEPCS,OGPSTONERATE,OGPSTONEUNIT,OGPSTONEWT,OGPSTONEAMOUNT" +
                                " from RETAIL_CUSTOMCALCULATIONS_TABLE a " +
                                " left join RETAILTRANSACTIONSALESTRANS B on a.TRANSACTIONID=b.TRANSACTIONID" +
                                " and a.STOREID=b.STORE and a.TERMINALID = b.TERMINALID and a.LINENUM=b.LINENUM" +
                                " WHERE b.ReceiptId = '" + sReceiptId + "' and QTY>0 and b.LINENUM=" + LineNo + "";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            DataTable dtPurchaseBathDetails = new DataTable();

            SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
            SqlDa.Fill(dtPurchaseBathDetails);

            //decimal dPcs = Convert.ToDecimal(command.ExecuteScalar());

            dtBatchDetails = dtPurchaseBathDetails;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (dtPurchaseBathDetails != null && dtPurchaseBathDetails.Rows.Count > 0)
                bReturnPcsValid = true;


            return bReturnPcsValid;
        }

        private bool IsReturendBatchNo(string sReceiptId, int iLine)
        {
            bool bReturnValid = false;

            SqlConnection connection = new SqlConnection();


            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            SqlCommand SqlComm = new SqlCommand();
            SqlComm.Connection = connection;
            SqlComm.CommandType = CommandType.Text;

            SqlComm.CommandText = "select REFINVOICENO,QUANTITY,PIECES,AMOUNT,TOTALWEIGHT,EXPECTEDQUANTITY,CRate," +
                                " CONFIGID,OGDMDPCS,OGGROSSUNIT,OGDMDWT,OGDMDUNIT, " +
                                " OGDMDAMT,OGSTONEPCS,OGSTONEWT,OGSTONEUNIT,OGSTONEAMT,OGNETWT,OGNETUNIT,TOTALAMOUNT" +
                                " , OGSTONEATE,OGGROSSAMT,OGACTAMT " +
                                " from RETAIL_CUSTOMCALCULATIONS_TABLE a " +
                                " left join RETAILTRANSACTIONSALESTRANS B on a.TRANSACTIONID=b.TRANSACTIONID" +
                                " and a.STOREID=b.STORE and a.TERMINALID = b.TERMINALID and a.LINENUM=b.LINENUM" +
                                " WHERE b.RECEIPTID = '" + sReceiptId + "' and QTY<0 and b.LINENUM = " + iLine + " and isOGReturn=1";

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            DataTable dtPurchaseBathDetails = new DataTable();

            SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
            SqlDa.Fill(dtPurchaseBathDetails);

            dtBatchDetails = dtPurchaseBathDetails;

            if (connection.State == ConnectionState.Open)
                connection.Close();

            if (dtPurchaseBathDetails != null && dtPurchaseBathDetails.Rows.Count > 0)
                bReturnValid = true;


            return bReturnValid;
        }


        private bool IsReturendReceiptId(string sReceiptId, int iLineId)
        {
            bool bReturnValid = false;

            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select isnull(b.RECEIPTID,'') ");
            commandText.Append(" from RETAIL_CUSTOMCALCULATIONS_TABLE a ");
            commandText.Append(" left join RETAILTRANSACTIONSALESTRANS B on a.TRANSACTIONID=b.TRANSACTIONID");
            commandText.Append(" and a.STOREID=b.STORE and a.TERMINALID = b.TERMINALID and a.LINENUM=b.LINENUM");
            commandText.Append(" WHERE b.RECEIPTID = '" + sReceiptId + "' AND b.LINENUM=" + iLineId + " and ISOGRETURN=1");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
                bReturnValid = true;
            else
                bReturnValid = false;

            return bReturnValid;
        }

        private void GetPurchasedItem(string sBatch, int iTransType)
        {
            SqlConnection conn = new SqlConnection();

            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = "select  a.CONFIGID,b.ITEMID,b.CUSTACCOUNT,A.TRANSACTIONTYPE,B.INVENTBATCHID, A.MAKINGAMOUNT" +
                            " from RETAIL_CUSTOMCALCULATIONS_TABLE a " +
                            " left join RETAILTRANSACTIONSALESTRANS B on a.TRANSACTIONID=b.TRANSACTIONID" +
                            " and a.STOREID=b.STORE and a.TERMINALID = b.TERMINALID and a.LINENUM=b.LINENUM" +
                            " WHERE b.RECEIPTID = '" + sBatch + "' and b.LINENUM = " + iTransType + "";


                SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sPConfigId = Convert.ToString(reader.GetValue(0));
                        sPItemId = Convert.ToString(reader.GetValue(1));
                        sPCustAcc = Convert.ToString(reader.GetValue(2));
                        iPTransType = Convert.ToInt16(reader.GetValue(3));
                        sDailyBatchId = Convert.ToString(reader.GetValue(4));
                        dMkAmt = Convert.ToDecimal(reader.GetValue(5));
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private void GetValidReceipt(int iTransType)
        {
            DataTable dtData = new DataTable();
            DataRow drData = null;
            SqlConnection conn = new SqlConnection();

            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = "select  b.RECEIPTID" +
                            " from RETAIL_CUSTOMCALCULATIONS_TABLE a " +
                            " left join RETAILTRANSACTIONSALESTRANS B on a.TRANSACTIONID=b.TRANSACTIONID" +
                            " and a.STOREID=b.STORE and a.TERMINALID = b.TERMINALID and a.LINENUM=b.LINENUM" +
                            " WHERE TRANSACTIONTYPE = " + iTransType + " and isnull(b.RECEIPTID,'')!= ''";


                SqlCommand command = new SqlCommand(query, conn);
                command.CommandTimeout = 0;
                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);

                adapter.Fill(dtData);

                if (conn.State == ConnectionState.Open)
                    conn.Close();

                Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch oSearch =
                    new Microsoft.Dynamics.Retail.Pos.Dialog.WinFormsTouch.frmGenericSearch(dtData, drData = null, "Receipt Search");
                oSearch.ShowDialog();
                drData = oSearch.SelectedDataRow;
                if (drData != null)
                {
                    txtBatchNo.Text = string.Empty;
                    txtBatchNo.Text = Convert.ToString(drData["RECEIPTID"]);
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }

        }

        private void LoadLine(string sReceiptId)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            string sbQuery = string.Empty;


            sbQuery = "select cast(LINENUM as int) LINENUM from RETAILTRANSACTIONSALESTRANS" +
                     " where RECEIPTID='" + sReceiptId + "' and TRANSACTIONSTATUS=0" +
                    " and ITEMID in (select itemid from INVENTTABLE where (OWNDMD=1" +
                    " or OWNOG=1 or OTHERDMD=1 or OTHEROG=1))";


            DataTable dtLine = new DataTable();
            if (!string.IsNullOrEmpty(sbQuery))
            {
                if (connection.State == ConnectionState.Closed)
                    connection.Open();
                using (SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.HasRows)
                        {
                            dtLine.Load(reader);
                        }
                        reader.Close();
                        reader.Dispose();
                    }
                    cmd.Dispose();
                }

                if (connection.State == ConnectionState.Open)
                    connection.Close();

                if (dtLine.Rows.Count > 0)
                {
                    cmbLine.DataSource = dtLine;
                    cmbLine.DisplayMember = "LINENUM";
                    cmbLine.ValueMember = "LINENUM";
                }
            }
        }

        private bool IsBatchItem(string sItemId)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            commandText = "select ISNULL(TRACKINGDIMENSIONGROUP,'') TRACKINGDIMENSIONGROUP from ECORESTRACKINGDIMENSIONGROUPFLDSETUP " +
                        " where TRACKINGDIMENSIONGROUP = (select TRACKINGDIMENSIONGROUP  from ECORESTRACKINGDIMENSIONGROUPITEM where ITEMID ='" + sItemId + "'" +
                        " and ITEMDATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "') and DIMENSIONFIELDID=2 and ISACTIVE=1";
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (!string.IsNullOrEmpty(sResult))
                return true;
            else
                return false;
        }

        private bool IsValidBatchId(string sBatchId, string sCustKarigar)
        {
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = string.Empty;
            commandText = "select RepairId from RetailRepairDetail where ISSUERECEIVE=1 and IsDelivered=0 and KARIGAR ='" + sCustKarigar + "'" +
                            " and BatchId='" + sBatchId + "'";
            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), connection);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (!string.IsNullOrEmpty(sResult))
                return true;
            else
                return false;
        }

        private bool IsCatchWtItem(string sItemId)
        {
            bool bCatchWtItem = false;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "DECLARE @ISCATCHWT INT  SET @ISCATCHWT = 0 IF EXISTS (SELECT ITEMID FROM pdscatchweightitem WHERE ITEMID = '" + sItemId + "')" +
                                 " BEGIN SET @ISCATCHWT = 1 END SELECT @ISCATCHWT";


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            bCatchWtItem = Convert.ToBoolean(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return bCatchWtItem;
        }

        private decimal getOGTolleranceRates()
        {
            decimal dRates = 0m;

            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            string commandText = "select isnull(OGMETALRATETOLERANCE,0) OGMETALRATETOLERANCE from RETAILSTORETABLE" +
                                " WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "'";


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            SqlCommand command = new SqlCommand(commandText, connection);
            command.CommandTimeout = 0;

            dRates = Convert.ToDecimal(command.ExecuteScalar());
            if (connection.State == ConnectionState.Open)
                connection.Close();

            return dRates;
        }

        private void GetItemType(string item) // for Malabar based on this new item type sales radio button or rest of the radio  button will be on/off
        {
            SqlConnection conn = new SqlConnection();

            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            try
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                string query = " SELECT TOP(1) OWNDMD,OWNOG,OTHERDMD,OTHEROG FROM INVENTTABLE WHERE ITEMID='" + item + "'";


                SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        iOWNDMD = Convert.ToInt16(reader.GetValue(0));
                        iOWNOG = Convert.ToInt16(reader.GetValue(1));
                        iOTHERDMD = Convert.ToInt16(reader.GetValue(2));
                        iOTHEROG = Convert.ToInt16(reader.GetValue(3));
                    }
                }
                reader.Close();
                reader.Dispose();
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                    conn.Close();
            }
        }

        private void txtNetRate_Leave(object sender, EventArgs e)
        {
            CalculateNetWtAmount();
        }

        private void grbNetWt_Enter(object sender, EventArgs e)
        {

        }

        private decimal GetratioPurityWise(string sConfigId)
        {
            string StoreID = ApplicationSettings.Database.StoreID;
            SqlConnection conn = new SqlConnection();

            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION NVARCHAR(20) DECLARE @TRANSCONFIGID NVARCHAR(20) DECLARE @TRANSCONFIGRATIO NUMERIC(32,16) ");
            commandText.Append("SET @TRANSCONFIGID = '" + sConfigId + "'");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  "); //INVENTPARAMETERS
           
            commandText.Append(" SELECT CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE =  1 AND CONFIGIDSTANDARD = @TRANSCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            decimal dResult = Convert.ToDecimal(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dResult;
        }

        private decimal GetAverageRate(string sConfigId,decimal dAvgPt)
        {
            string sDefaultConfigId = GetDefaultConfigId(iMetalType);
            decimal dTranPurityratio = Convert.ToDecimal(GetratioPurityWise(sConfigId1));
            decimal dbasePurityratio = Convert.ToDecimal(GetratioPurityWise(sDefaultConfigId));///inventparam
            decimal dGetTempRate = Convert.ToDecimal(sRate1) / dbasePurityratio;
            dGetTempRate = dGetTempRate * dTranPurityratio;
            dGetTempRate = decimal.Round((dGetTempRate * dAvgPt), 2, MidpointRounding.AwayFromZero);
            decimal dResult = Convert.ToDecimal(dGetTempRate);
            return dResult;
        }

        private string GetDefaultConfigId(int iMetalType = 0)
        {
            string StoreID = ApplicationSettings.Database.StoreID;
            SqlConnection conn = new SqlConnection();

            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION NVARCHAR(20)  DECLARE @FIXINGCONFIGID NVARCHAR(20)");

            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  "); //INVENTPARAMETERS

            if (iMetalType == (int)MetalType.Gold)
                commandText.Append(" SELECT DefaultConfigIdGold FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else if (iMetalType == (int)MetalType.Silver)
                commandText.Append(" SELECT  DEFAULTCONFIGIDSILVER FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else if (iMetalType == (int)MetalType.Platinum)
                commandText.Append(" SELECT  DEFAULTCONFIGIDPLATINUM FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else if (iMetalType == (int)MetalType.Palladium)
                commandText.Append(" SELECT  DEFAULTCONFIGIDPALLADIUM FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            string dResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dResult;
        }

        private void CalculateNetWtAmount(int iStoneEnter = 0)
        {
            decimal dNetWt = 0m;
            decimal dNetAmt = 0m;
            decimal dRate = 0m;
            decimal dGrossWt = (!string.IsNullOrEmpty(txtGrossWt.Text.Trim())) ? Convert.ToDecimal(txtGrossWt.Text.Trim()) : 0m;
            decimal dDmdWt = (!string.IsNullOrEmpty(txtDiamondWt.Text.Trim()) && (Convert.ToDecimal(txtDiamondWt.Text.Trim()) != 0)) ? Convert.ToDecimal(txtDiamondWt.Text.Trim()) / 5 : 0m;
            decimal dStoneWt = (!string.IsNullOrEmpty(txtStoneWt.Text.Trim()) && (Convert.ToDecimal(txtStoneWt.Text.Trim()) != 0)) ? Convert.ToDecimal(txtStoneWt.Text.Trim()) : 0m;
            decimal dPStoneWt = (!string.IsNullOrEmpty(txtPStoneWt.Text.Trim()) && (Convert.ToDecimal(txtPStoneWt.Text.Trim()) != 0)) ? Convert.ToDecimal(txtPStoneWt.Text.Trim()) / 5 : 0m;
            decimal dAvgPurity = (!string.IsNullOrEmpty(txtAvgPurity.Text.Trim())) ? Convert.ToDecimal(txtAvgPurity.Text.Trim()) : 0m;
            decimal dActAmt = (!string.IsNullOrEmpty(txtActAmt.Text.Trim())) ? Convert.ToDecimal(txtActAmt.Text.Trim()) : 0m;
            decimal dChangedAmt = (!string.IsNullOrEmpty(txtChangedAmt.Text.Trim())) ? Convert.ToDecimal(txtChangedAmt.Text.Trim()) : 0m;
            decimal dOwnDmdMkAmt = (!string.IsNullOrEmpty(txtMkAmt.Text.Trim())) ? Convert.ToDecimal(txtMkAmt.Text.Trim()) : 0m;

            if (iCTransType != (int)TransactionType.ExchangeReturn && iCTransType != (int)TransactionType.PurchaseReturn)
            {
                //=====================Get Average Rate================================================
                if (dAvgPurity > 0 )
                {
                    dRate = GetAverageRate(sConfigId1, dAvgPurity);
                    //string sDefaultConfigId = GetDefaultConfigId(iMetalType);
                    //decimal dTranPurityratio = Convert.ToDecimal(GetratioPurityWise(sConfigId1));
                    //decimal dbasePurityratio = Convert.ToDecimal(GetratioPurityWise(sDefaultConfigId));///inventparam

                    //decimal dGetTempRate = Convert.ToDecimal(sRate1) / dbasePurityratio;
                    //dGetTempRate = dGetTempRate * dTranPurityratio;
                    //dGetTempRate = decimal.Round((dGetTempRate * dAvgPurity), 2, MidpointRounding.AwayFromZero);
                    //dRate = Convert.ToDecimal(dGetTempRate);
                    txtNetRate.Text = Convert.ToString(dRate);
                }
                else
                {
                    dRate = Convert.ToDecimal(sRate1);// (!string.IsNullOrEmpty(txtNetRate.Text.Trim())) ? Convert.ToDecimal(txtNetRate.Text.Trim()) : 0m; 
                }
            
                //dRate = Convert.ToDecimal(sRate1);// (!string.IsNullOrEmpty(txtNetRate.Text.Trim())) ? Convert.ToDecimal(txtNetRate.Text.Trim()) : 0m;
            }
            else
            dRate = Convert.ToDecimal((!string.IsNullOrEmpty(txtNetRate.Text.Trim())) ? Convert.ToDecimal(txtNetRate.Text.Trim()) : 0m);
            //============================================================= // subha
            decimal ChngAmt = Convert.ToDecimal((!string.IsNullOrEmpty(txtChangedAmt.Text.Trim())) ? Convert.ToDecimal(txtChangedAmt.Text.Trim()) : 0m);
            if (ChngAmt == 0)
            {
                txtNetRate.Text = Convert.ToString(dRate);  
            }
            //============================================================= // subha

            decimal dDmdAmt = (!string.IsNullOrEmpty(txtDiamondAmount.Text.Trim())) ? Convert.ToDecimal(txtDiamondAmount.Text.Trim()) : 0m;
            decimal dStnAmt = (!string.IsNullOrEmpty(txtStoneAmount.Text.Trim())) ? Convert.ToDecimal(txtStoneAmount.Text.Trim()) : 0m;
            decimal dPStnAmt = (!string.IsNullOrEmpty(txtPStoneAmt.Text.Trim())) ? Convert.ToDecimal(txtPStoneAmt.Text.Trim()) : 0m;

            if (string.IsNullOrEmpty(txtChangedGrossWt.Text))
            {
                if (bIsCWItem
                    && (iMetalType == (int)MetalType.Gold
                   || iMetalType == (int)MetalType.Platinum
                   || iMetalType == (int)MetalType.Palladium
                   || iMetalType == (int)MetalType.Silver))
                {
                    txtChangedGrossWt.Text = Convert.ToString(dGrossWt);
                }
            }

            decimal dChangedGrossWt = (!string.IsNullOrEmpty(txtChangedGrossWt.Text.Trim())) ? Convert.ToDecimal(txtChangedGrossWt.Text.Trim()) : 0m;

            if (dChangedGrossWt > 0)
                dNetWt = decimal.Round((dChangedGrossWt), 3, MidpointRounding.AwayFromZero);


            //if (dNetWt > 0 && dAvgPurity > 0)
            //{
            //    decimal dConvertedToParamConfigQty = decimal.Round(getConvertedMetalQty(sConfigId1, dNetWt, true, iMetalType), 3, MidpointRounding.AwayFromZero);

            //    decimal dConvertedToPTransConfigQty = decimal.Round(getConvertedMetalQty(sConfigId1, (dConvertedToParamConfigQty * dAvgPurity), false, iMetalType), 3, MidpointRounding.AwayFromZero);

            //    dNetWt = dConvertedToPTransConfigQty;// decimal.Round((dChangedGrossWt - dDmdWt - dStoneWt) * dAvgPurity, 3, MidpointRounding.AwayFromZero);
            //}


            if (bIsCWItem
              && (iMetalType == (int)MetalType.Gold
                  || iMetalType == (int)MetalType.Platinum
                  || iMetalType == (int)MetalType.Palladium
                  || iMetalType == (int)MetalType.Silver))
            {
                txtGrossWt.Text = Convert.ToString(dNetWt);
            }


            if (dChangedGrossWt > 0)
                dNetWt = decimal.Round((dNetWt - dDmdWt - dStoneWt - dPStoneWt), 3, MidpointRounding.AwayFromZero);

            txtNetWt.Text = Convert.ToString(dNetWt);


            if (dNetWt > 0 && dRate > 0)
                dNetAmt = decimal.Round((dNetWt * dRate), 2, MidpointRounding.AwayFromZero);


            txtNetAmount.Text = Convert.ToString(dNetAmt);
            txtGrossAmt.Text = Convert.ToString(dNetAmt + dDmdAmt + dStnAmt + dPStnAmt + dOwnDmdMkAmt);
            txtActAmt.Text = txtGrossAmt.Text;
            txtFinalAmt.Text = txtGrossAmt.Text;
        }

        private void CalculateFinalAmount()
        {
            decimal dActAmt = 0m;
            decimal dChangedAmt = 0m;
            decimal dNetAmt = 0m;
            decimal dNetRate = 0m;
            decimal dDmdRate = 0m;
            decimal dNetWt = 0m;
            decimal dDmdWt = 0m;
            decimal dDmdAmt = 0m;
            decimal dStnAmt = 0m;
            decimal dPStnAmt = 0m;

            decimal dActRate = Convert.ToDecimal(sRate1);
            decimal dChangedGrossWt = 0;
            dChangedAmt = (!string.IsNullOrEmpty(txtChangedAmt.Text.Trim())) ? Convert.ToDecimal(txtChangedAmt.Text.Trim()) : 0m;
            dNetAmt = (!string.IsNullOrEmpty(txtNetAmount.Text.Trim())) ? Convert.ToDecimal(txtNetAmount.Text.Trim()) : 0m;
            dNetRate = (!string.IsNullOrEmpty(txtNetRate.Text.Trim())) ? Convert.ToDecimal(txtNetRate.Text.Trim()) : 0m;
            dDmdRate = (!string.IsNullOrEmpty(txtDmdRate.Text.Trim())) ? Convert.ToDecimal(txtDmdRate.Text.Trim()) : 0m;


            dDmdWt = (!string.IsNullOrEmpty(txtDiamondWt.Text.Trim())) ? Convert.ToDecimal(txtDiamondWt.Text.Trim()) : 0m;
            dDmdAmt = (!string.IsNullOrEmpty(txtDiamondAmount.Text.Trim())) ? Convert.ToDecimal(txtDiamondAmount.Text.Trim()) : 0m;
            dStnAmt = (!string.IsNullOrEmpty(txtStoneAmount.Text.Trim())) ? Convert.ToDecimal(txtStoneAmount.Text.Trim()) : 0m;

            dPStnAmt = (!string.IsNullOrEmpty(txtPStoneAmt.Text.Trim())) ? Convert.ToDecimal(txtPStoneAmt.Text.Trim()) : 0m;

            //dChangedGrossWt = (!string.IsNullOrEmpty(txtChangedGrossWt.Text.Trim())) ? Convert.ToDecimal(txtChangedGrossWt.Text.Trim()) : 0m;
            dChangedGrossWt = (!string.IsNullOrEmpty(txtGrossWt.Text.Trim())) ? Convert.ToDecimal(txtGrossWt.Text.Trim()) : 0m;
            decimal dAvgPurity = (!string.IsNullOrEmpty(txtAvgPurity.Text.Trim())) ? Convert.ToDecimal(txtAvgPurity.Text.Trim()) : 0m;
            dNetWt = (!string.IsNullOrEmpty(txtNetWt.Text.Trim())) ? Convert.ToDecimal(txtNetWt.Text.Trim()) : 0m;

            if (dAvgPurity > 0)
                txtActAmt.Text = Convert.ToString(dNetAmt + dDmdAmt + dStnAmt + dPStnAmt);
            else
                //txtActAmt.Text = Convert.ToString(decimal.Round((dActRate * dChangedGrossWt), 2, MidpointRounding.AwayFromZero));
                txtActAmt.Text = Convert.ToString(decimal.Round((dActRate * dNetWt), 2, MidpointRounding.AwayFromZero));            // subha


            dActAmt = (!string.IsNullOrEmpty(txtActAmt.Text.Trim())) ? Convert.ToDecimal(txtActAmt.Text.Trim()) : 0m;


            if (dChangedAmt > 0)
            {
                txtFinalAmt.Text = Convert.ToString(dChangedAmt);
                if (dActAmt > 0)
                    dDiffAmt = decimal.Round((dActAmt - dChangedAmt), 2, MidpointRounding.AwayFromZero);
                if (grbDiamondWt.Enabled == false)
                {
                    if (Math.Abs(dDiffAmt) > 0)
                    {
                        txtNetAmount.Text = Convert.ToString(dNetAmt - dDiffAmt);
                        txtNetRate.Text = Convert.ToString(decimal.Round(((Convert.ToDecimal(txtNetAmount.Text)) / dNetWt), 2, MidpointRounding.AwayFromZero));
                    }
                    else
                        if (dAvgPurity > 0)
                            txtNetRate.Text = Convert.ToString(GetAverageRate(sConfigId1, dAvgPurity));
                        else
                            txtNetRate.Text = sRate1;
                }
                else
                {
                    if (dDiffAmt > 0)
                    {
                        txtDiamondAmount.Text = Convert.ToString(dDmdAmt - dDiffAmt);
                        txtDmdRate.Text = Convert.ToString(decimal.Round((Convert.ToDecimal(txtDiamondAmount.Text) / dDmdWt), 2, MidpointRounding.AwayFromZero));
                    }
                    else
                        if (dAvgPurity > 0)
                            txtNetRate.Text = Convert.ToString(GetAverageRate(sConfigId1, dAvgPurity));
                        else
                            txtNetRate.Text = sRate1;
                }
            }
            else
                if (dAvgPurity > 0)
                    txtNetRate.Text = Convert.ToString(GetAverageRate(sConfigId1,dAvgPurity));
                else
                    txtNetRate.Text = sRate1;
        }

        private void txtChangedAmt_TextChanged(object sender, EventArgs e)
        {

        }

        private void txtChangedAmt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtChangedAmt.Text))
            {
                CalculateNetWtAmount();         
                CalculateFinalAmount();         
                groupBox1.Enabled = false;
                grbStoneWt.Enabled = false;
            }
            else
            {
                txtChangedAmt.Text = "0";           // subha
                CalculateNetWtAmount();             // subha
                groupBox1.Enabled = true;
                grbStoneWt.Enabled = true;
            }
        }

        private void txtDiamondAmount_Leave(object sender, EventArgs e)
        {
            txtDmdRate.Text = "";
            CalculateDmdWtAmount();
            CalculateNetWtAmount();
        }

        private void txtStoneAmount_Leave(object sender, EventArgs e)
        {
            txtStnRate.Text = "";
            CalculateStoneWtAmount();
            CalculateNetWtAmount();
        }

        private void txtDmdRate_Leave(object sender, EventArgs e)
        {
            txtDiamondAmount.Text = "";
            CalculateDmdWtAmount();
            CalculateNetWtAmount();
        }

        private void CalculateDmdWtAmount()
        {
            decimal dDmdWt = (!string.IsNullOrEmpty(txtDiamondWt.Text.Trim())) ? Convert.ToDecimal(txtDiamondWt.Text.Trim()) : 0m;
            decimal dDmdRate = (!string.IsNullOrEmpty(txtDmdRate.Text.Trim())) ? Convert.ToDecimal(txtDmdRate.Text.Trim()) : 0m;
            decimal dDmdAmt = (!string.IsNullOrEmpty(txtDiamondAmount.Text.Trim())) ? Convert.ToDecimal(txtDiamondAmount.Text.Trim()) : 0m;

            if (dDmdAmt > 0)
                txtDmdRate.Text = Convert.ToString(Convert.ToString(decimal.Round((dDmdAmt / dDmdWt), 2, MidpointRounding.AwayFromZero)));
            else
                txtDiamondAmount.Text = Convert.ToString(dDmdRate * dDmdWt);

            CalTotalAmt();
        }

        private void CalculateStoneWtAmount()
        {
            decimal dStnWt = (!string.IsNullOrEmpty(txtStoneWt.Text.Trim())) ? Convert.ToDecimal(txtStoneWt.Text.Trim()) : 0m;
            decimal dStnRate = (!string.IsNullOrEmpty(txtStnRate.Text.Trim())) ? Convert.ToDecimal(txtStnRate.Text.Trim()) : 0m;
            decimal dStnAmt = (!string.IsNullOrEmpty(txtStoneAmount.Text.Trim())) ? Convert.ToDecimal(txtStoneAmount.Text.Trim()) : 0m;

            decimal dPStnWt = (!string.IsNullOrEmpty(txtPStoneWt.Text.Trim())) ? Convert.ToDecimal(txtPStoneWt.Text.Trim()) : 0m;
            decimal dPStnRate = (!string.IsNullOrEmpty(txtPStoneRate.Text.Trim())) ? Convert.ToDecimal(txtPStoneRate.Text.Trim()) : 0m;
            decimal dPStnAmt = (!string.IsNullOrEmpty(txtPStoneAmt.Text.Trim())) ? Convert.ToDecimal(txtPStoneAmt.Text.Trim()) : 0m;

            if (dStnAmt > 0)
                txtStnRate.Text = Convert.ToString(decimal.Round((dStnAmt / dStnWt), 2, MidpointRounding.AwayFromZero));
            else
                txtStoneAmount.Text = Convert.ToString(dStnRate * dStnWt);

            if (dPStnAmt > 0)
                txtPStoneRate.Text = Convert.ToString(decimal.Round((dPStnAmt / dPStnWt), 2, MidpointRounding.AwayFromZero));
            else
                txtPStoneAmt.Text = Convert.ToString(dPStnRate * dPStnWt);

            CalTotalAmt();
        }

        private void CalTotalAmt()
        {
            decimal dDmdAmt = (!string.IsNullOrEmpty(txtDiamondAmount.Text.Trim())) ? Convert.ToDecimal(txtDiamondAmount.Text.Trim()) : 0m;
            decimal dGrossAmt = (!string.IsNullOrEmpty(txtGrossAmt.Text.Trim())) ? Convert.ToDecimal(txtGrossAmt.Text.Trim()) : 0m;
            decimal dStnAmt = (!string.IsNullOrEmpty(txtStoneAmount.Text.Trim())) ? Convert.ToDecimal(txtStoneAmount.Text.Trim()) : 0m;
            decimal dPStnAmt = (!string.IsNullOrEmpty(txtPStoneAmt.Text.Trim())) ? Convert.ToDecimal(txtPStoneAmt.Text.Trim()) : 0m;

            decimal dTotalAmt = decimal.Round((dDmdAmt + dGrossAmt + dStnAmt + dPStnAmt), 2, MidpointRounding.AwayFromZero);

            txtFinalAmt.Text = Convert.ToString(dTotalAmt);
        }

        private void txtStnRate_Leave(object sender, EventArgs e)
        {
            txtStoneAmount.Text = "";
            CalculateStoneWtAmount();
            CalculateNetWtAmount();
        }

        internal bool IsNumeric(string ValueInNumeric)
        {
            bool IsFine = true;
            foreach (char ch in ValueInNumeric)
            {
                if (!(ch >= '0' && ch <= '9'))
                {
                    IsFine = false;
                }
            }
            return IsFine;
        }

        private void txtChangedGrossWt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtChangedGrossWt.Text))
            {
                string s = txtChangedGrossWt.Text.ToString();
                if (!IsNumeric(s))
                {
                    string[] parts = s.Split('.');
                    Int64 iValue = 0;
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        iValue = Int64.Parse(parts[0]);
                    }

                    Int64 i2 = Int64.Parse(parts[1]);

                    string sI2 = parts[1];
                    string sAfterDecimalValue = string.Empty;
                    if (sI2.ToString().Length > 3)
                    {
                        MessageBox.Show("Maximum decimal places is three.");
                        txtChangedGrossWt.Focus();
                    }
                    else
                    {
                        CalculateNetWtAmount();
                    }
                }
                else
                {
                    CalculateNetWtAmount();
                }

                string sDefUnit = "";
                string sDefDmdUnit = "";

                GetDefaulParamtUnit(ref sDefUnit, ref sDefDmdUnit);

                if (!bIsCWItem && (sBaseUnit1 == sDefUnit || sBaseUnit1 == sDefDmdUnit))
                {
                    txtGrossWt.Text = txtChangedGrossWt.Text;
                    txtGrossWt.Enabled = false;
                }
                else
                    txtGrossWt.Enabled = true;
            }

        }

        private void GetDefaulParamtUnit(ref string sDefaulUnitId, ref string sDefaultDmdUnit)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;


            if (conn.State == ConnectionState.Closed)
                conn.Open();

            string commandText = " SELECT  ISNULL(DEFAULTUNITID,'')  DEFAULTUNITID ,isnull(CRWDefaultDiamondUnitId,'') CRWDefaultDiamondUnitId" +
                                " FROM INVENTPARAMETERS WHERE DATAAREAID = '" + ApplicationSettings.Database.DATAAREAID + "'";


            SqlCommand command = new SqlCommand(commandText, conn);
            command.CommandTimeout = 0;

            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        sDefaulUnitId = Convert.ToString(reader.GetValue(0));
                        sDefaultDmdUnit = Convert.ToString(reader.GetValue(1));
                    }
                }
            }

            if (conn.State == ConnectionState.Open)
                conn.Close();

        }

        private void chkRepair_CheckedChanged(object sender, EventArgs e)
        {
            if (chkRepair.Checked)
            {
                lblRepairId.Visible = true;
                txtRepairId.Visible = true;
                txtRepairId.Focus();
            }
            else
            {
                lblRepairId.Visible = false;
                txtRepairId.Visible = false;
            }
        }

        private void txtChangedAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtStnRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtDmdRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtChangedGrossWt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            //if(iDecPre == 0)
            //{
            //    if(e.KeyChar == '.')
            //    {
            //        e.Handled = true;
            //    }
            //}



            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void btnFetchOldSalesdata_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtRefInvoiceNo.Text) && !string.IsNullOrEmpty(txtOldSkuNumber.Text))
            {
                GetSBSOldSalesDataItem(txtRefInvoiceNo.Text, txtOldSkuNumber.Text);
            }
        }

        private void GetSBSOldSalesDataItem(string sOldInvNo, string sOldSkunumber)
        {
            try
            {
                if (PosApplication.Instance.TransactionServices.CheckConnection())
                {
                    ReadOnlyCollection<object> containerArray;
                    string sStoreId = ApplicationSettings.Terminal.StoreId;
                    containerArray = PosApplication.Instance.TransactionServices.InvokeExtension("GetSBSOldSalesData", sOldInvNo, sOldSkunumber);

                    DataSet dsRC = new DataSet();
                    StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                    if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                    {
                        dsRC.ReadXml(srTransDetail);
                    }
                    if (dsRC != null && dsRC.Tables[0].Rows.Count > 0)
                    {
                        foreach (DataRow dr in dsRC.Tables[0].Rows)
                        {
                            txtRefInvoiceNo.Text = Convert.ToString(dr["OldInvoice"]);
                            txtDiamondPcs.Text = !string.IsNullOrEmpty(Convert.ToString(dr["DmdPcs"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["DmdPcs"])), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                            txtPcs.Text = !string.IsNullOrEmpty(Convert.ToString(dr["CWQty"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["CWQty"])), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                            txtDmdRate.Text = Convert.ToString(dr["DMDRATE"]);
                            txtDiamondWt.Text = Convert.ToString(dr["DmdWt"]);
                            txtDiamondAmount.Text = Convert.ToString(Convert.ToDecimal(dr["DMDRATE"]) * Convert.ToDecimal(dr["DmdWt"]));
                            txtStonePcs.Text = !string.IsNullOrEmpty(Convert.ToString(dr["StonePcs"])) ? decimal.Round(Convert.ToDecimal(Convert.ToString(dr["StonePcs"])), 0, MidpointRounding.AwayFromZero).ToString() : "0";
                            txtStoneWt.Text = Convert.ToString(dr["StoneWt"]);
                            txtNetWt.Text = Convert.ToString(dr["NetWt"]);
                            txtStnRate.Text = Convert.ToString(dr["STONEATE"]);
                            txtStoneAmount.Text = Convert.ToString(Convert.ToDecimal(dr["STONEATE"]) * Convert.ToDecimal(dr["StoneWt"]));
                            txtGrossWt.Text = Convert.ToString(dr["NetWt"]);
                            txtChangedGrossWt.Text = Convert.ToString(dr["NetWt"]);
                            txtNetPurity.Text = Convert.ToString(dr["ConfigId"]);
                            txtGrossAmt.Text = Convert.ToString(dr["OldSalesValue"]);
                            txtActAmt.Text = Convert.ToString(dr["OldSalesValue"]);
                            txtNetRate.Text = Convert.ToString(dr["NetRate"]);

                            decimal dNetRate = 0m;
                            if (!string.IsNullOrEmpty(txtNetRate.Text))
                                dNetRate = Convert.ToDecimal(txtNetRate.Text);

                            decimal dNetWt = 0m;
                            if (!string.IsNullOrEmpty(txtNetWt.Text))
                                dNetWt = Convert.ToDecimal(txtNetWt.Text);

                            txtNetAmount.Text = Convert.ToString(decimal.Round((dNetRate * dNetWt), 2, MidpointRounding.AwayFromZero));
                            txtFinalAmt.Text = Convert.ToString(dr["OldSalesValue"]);
                        }
                        // CalculateNetWtAmount();
                        txtRefInvoiceNo.Enabled = false;
                        txtPcs.Enabled = false;
                        txtGrossWt.Enabled = false;

                        txtDiamondWt.Enabled = false;
                        txtDiamondPcs.Enabled = false;

                        txtDiamondAmount.Enabled = false;
                        txtStoneWt.Enabled = false;
                        txtStonePcs.Enabled = false;

                        txtStoneAmount.Enabled = false;
                        txtNetWt.Enabled = false;
                        txtNetRate.Enabled = false;

                        txtNetPurity.Enabled = false;
                        txtNetAmount.Enabled = false;

                    }
                    else
                    {
                        MessageBox.Show("Invalid old invoice no or skunumber");
                        txtOldSkuNumber.Text = "";
                        txtRefInvoiceNo.Text = "";
                        txtDiamondPcs.Text = "";
                        txtPcs.Text = "";
                        txtDiamondWt.Text = "";
                        txtDiamondAmount.Text = "";
                        txtStonePcs.Text = "";
                        txtStoneWt.Text = "";
                        txtNetWt.Text = "";
                        txtStoneAmount.Text = "";
                        txtGrossWt.Text = "";
                        txtNetPurity.Text = sConfigId1;
                        txtNetRate.Text = sRate1;
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private int GetDecimalPrecison(string sUnit)
        {
            SqlConnection connection = new SqlConnection();
            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;
            StringBuilder sbQuery = new StringBuilder();

            sbQuery.Append("SELECT top 1 DECIMALPRECISION FROM UNITOFMEASURE WHERE SYMBOL='" + sUnit + "'");

            if (connection.State == ConnectionState.Closed)
                connection.Open();
            SqlCommand cmd = new SqlCommand(sbQuery.ToString(), connection);
            return Convert.ToInt16(cmd.ExecuteScalar());
        }

        protected int GetMetalType(string sItemId)
        {
            SqlConnection conn = new SqlConnection();
            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;
            int iMetalType = 100;

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select metaltype from inventtable where itemid='" + sItemId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    iMetalType = (int)reader.GetValue(0);
                }
            }
            if (conn.State == ConnectionState.Open)
                conn.Close();
            return iMetalType;

        }

        private void btnSearchReceiptId_Click(object sender, EventArgs e)
        {

            if (iCTransType == (int)TransactionType.PurchaseReturn)
                GetValidReceipt((int)TransactionType.Purchase);
            else if (iCTransType == (int)TransactionType.ExchangeReturn)
                GetValidReceipt((int)TransactionType.Exchange);
            else
                MessageBox.Show("Invalid transaction");

        }

        private void txtBatchNo_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtBatchNo.Text.Trim()))
            {
                LoadLine(txtBatchNo.Text);
            }
        }

        private void txtBatchNo_Leave(object sender, EventArgs e)
        {
            txtBatchNo_TextChanged(sender, e);
        }

        #region  GET RATE FROM METAL TABLE
        private decimal getSaleRateFromMetalTable(string sItemId, string ConfigID)
        {
            SqlConnection conn = new SqlConnection();

            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION VARCHAR(20)");
            commandText.Append(" DECLARE @METALTYPE INT ");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM         RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "'  ");
            commandText.Append(" SELECT @METALTYPE=METALTYPE FROM [INVENTTABLE] WHERE ITEMID='" + sItemId.Trim() + "' ");

            commandText.Append(" IF(@METALTYPE IN ('" + (int)MetalType.Gold + "','" + (int)MetalType.Silver + "','" + (int)MetalType.Platinum + "','" + (int)MetalType.Palladium + "')) ");//
            commandText.Append(" BEGIN ");

            commandText.Append(" SELECT TOP 1 CAST(RATES AS decimal (16,2)) FROM METALRATES WHERE INVENTLOCATIONID=@INVENTLOCATION  ");
            commandText.Append(" AND METALTYPE=@METALTYPE ");
            commandText.Append(" AND RETAIL=1 AND RATETYPE='" + (int)RateTypeNew.Sale + "' ");
            commandText.Append(" AND ACTIVE=1 AND CONFIGIDSTANDARD='" + ConfigID.Trim() + "' ");
            commandText.Append(" ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC END ");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            if (string.IsNullOrEmpty(sResult))
                sResult = "0";

            return Convert.ToDecimal(sResult.Trim());

        }
        #endregion

        /* private decimal getConvertedToParamConfigMetalQty(string sTransConfigid, decimal dQtyToConvert,bool istranstofixing, int iMetalType = 0)
        {
            string StoreID = ApplicationSettings.Database.StoreID;
            SqlConnection conn = new SqlConnection();

            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION NVARCHAR(20)  DECLARE @FIXINGCONFIGID NVARCHAR(20)  DECLARE @TRANSCONFIGID NVARCHAR(20)  DECLARE @istranstofixing NVARCHAR(5) ");
            commandText.Append(" DECLARE @QTY NUMERIC(32,3)  DECLARE @FIXINGCONFIGRATIO NUMERIC(32,16)  DECLARE @TRANSCONFIGRATIO NUMERIC(32,16) ");
            commandText.Append("SET @TRANSCONFIGID = '" + sTransConfigid + "'");
            commandText.Append("SET @QTY = ");
            commandText.Append(dQtyToConvert);

            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  "); //INVENTPARAMETERS

            if (iMetalType == (int)MetalType.Gold)
                commandText.Append(" SELECT @FIXINGCONFIGID = DefaultConfigIdGold FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else if (iMetalType == (int)MetalType.Silver)
                commandText.Append(" SELECT @FIXINGCONFIGID = DEFAULTCONFIGIDSILVER FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else if (iMetalType == (int)MetalType.Platinum)
                commandText.Append(" SELECT @FIXINGCONFIGID = DEFAULTCONFIGIDPLATINUM FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else if (iMetalType == (int)MetalType.Palladium)
                commandText.Append(" SELECT @FIXINGCONFIGID = DEFAULTCONFIGIDPALLADIUM FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            commandText.Append(" SELECT @FIXINGCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = " + iMetalType + " AND CONFIGIDSTANDARD = @FIXINGCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" SELECT @TRANSCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE =  " + iMetalType + " AND CONFIGIDSTANDARD = @TRANSCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            //commandText.Append(" IF(@FIXINGCONFIGRATIO >= @TRANSCONFIGRATIO) BEGIN  IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) BEGIN  SELECT ISNULL((( @TRANSCONFIGRATIO * @QTY) / @FIXINGCONFIGRATIO),0) AS CONVERTEDQTY   END ");
            ////commandText.Append(" ELSE BEGIN SELECT @QTY AS CONVERTEDQTY END END");
            //commandText.Append(" ELSE BEGIN SELECT ISNULL(((@FIXINGCONFIGRATIO * @QTY) / @TRANSCONFIGRATIO),0) AS CONVERTEDQTY   END ");
            //commandText.Append(" ELSE BEGIN SELECT @QTY AS CONVERTEDQTY END END");

            commandText.Append(" IF(@FIXINGCONFIGRATIO >= @TRANSCONFIGRATIO) ");
            commandText.Append(" BEGIN  ");
            commandText.Append(" IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) ");
            commandText.Append(" BEGIN  ");
            commandText.Append(" SELECT ISNULL((( @TRANSCONFIGRATIO * @QTY) / @FIXINGCONFIGRATIO),0) AS CONVERTEDQTY ");
            commandText.Append(" END  ");
            commandText.Append(" else");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT @QTY AS CONVERTEDQTY ");
            commandText.Append(" END ");
            commandText.Append(" END");
            commandText.Append(" ELSE");
            commandText.Append(" BEGIN	");
            commandText.Append(" IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) ");
            commandText.Append(" BEGIN  ");
            commandText.Append(" SELECT ISNULL(((@FIXINGCONFIGRATIO * @QTY) / @TRANSCONFIGRATIO),0) AS CONVERTEDQTY   ");
            commandText.Append(" END  ");
            commandText.Append(" ELSE");
            commandText.Append(" BEGIN ");
            commandText.Append(" SELECT @QTY AS CONVERTEDQTY ");
            commandText.Append(" END ");
            commandText.Append(" END  ");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            decimal dResult = Convert.ToDecimal(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dResult;
        }

        private decimal getConvertedGoldQty(string sTransConfigid, decimal dQtyToConvert, bool istranstofixing)
        {
            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION NVARCHAR(20)  DECLARE @FIXINGCONFIGID NVARCHAR(20)  DECLARE @TRANSCONFIGID NVARCHAR(20)  DECLARE @istranstofixing NVARCHAR(5) ");
            commandText.Append(" DECLARE @QTY NUMERIC(32,10)  DECLARE @FIXINGCONFIGRATIO NUMERIC(32,16)  DECLARE @TRANSCONFIGRATIO NUMERIC(32,16) ");
            commandText.Append("SET @TRANSCONFIGID = '" + sTransConfigid + "'");
            commandText.Append("SET @QTY = ");
            commandText.Append(dQtyToConvert);
            if (istranstofixing)
                commandText.Append("SET @istranstofixing = 'Y'");
            else
                commandText.Append("SET @istranstofixing = 'N'");
            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  ");
            commandText.Append(" SELECT @FIXINGCONFIGID = DefaultConfigIdGold FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" SELECT @FIXINGCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = 1 AND CONFIGIDSTANDARD = @FIXINGCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" SELECT @TRANSCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = 1 AND CONFIGIDSTANDARD = @TRANSCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" IF @istranstofixing = 'Y' BEGIN  IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) BEGIN  SELECT ISNULL(((@FIXINGCONFIGRATIO * @QTY) / @TRANSCONFIGRATIO),0) AS CONVERTEDQTY   END ");
            commandText.Append(" ELSE BEGIN SELECT @QTY AS CONVERTEDQTY END END");
            commandText.Append(" ELSE BEGIN IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) BEGIN SELECT ISNULL(((@TRANSCONFIGRATIO * @QTY) / @FIXINGCONFIGRATIO),0) AS CONVERTEDQTY   END ");
            commandText.Append(" ELSE BEGIN SELECT @QTY AS CONVERTEDQTY END END");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            decimal dResult = Convert.ToDecimal(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            
            return dResult;

        }*/

        private decimal getConvertedMetalQty(string sTransConfigid, decimal dQtyToConvert, bool istranstofixing, int iMetalType = 0)
        {
            string StoreID = ApplicationSettings.Database.StoreID;
            SqlConnection conn = new SqlConnection();

            if (application != null)
                conn = application.Settings.Database.Connection;
            else
                conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" DECLARE @INVENTLOCATION NVARCHAR(20)  DECLARE @FIXINGCONFIGID NVARCHAR(20)  DECLARE @TRANSCONFIGID NVARCHAR(20)  DECLARE @istranstofixing NVARCHAR(5) ");
            commandText.Append(" DECLARE @QTY NUMERIC(32,3)  DECLARE @FIXINGCONFIGRATIO NUMERIC(32,16)  DECLARE @TRANSCONFIGRATIO NUMERIC(32,16) ");
            commandText.Append("SET @TRANSCONFIGID = '" + sTransConfigid + "'");
            commandText.Append("SET @QTY = ");
            commandText.Append(dQtyToConvert);
            if (istranstofixing)
                commandText.Append("SET @istranstofixing = 'Y'");
            else
                commandText.Append("SET @istranstofixing = 'N'");

            commandText.Append(" SELECT @INVENTLOCATION=RETAILCHANNELTABLE.INVENTLOCATION FROM  RETAILCHANNELTABLE INNER JOIN ");
            commandText.Append(" RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID ");
            commandText.Append(" WHERE STORENUMBER='" + StoreID.Trim() + "'  "); //INVENTPARAMETERS

            if (iMetalType == (int)MetalType.Gold)
                commandText.Append(" SELECT @FIXINGCONFIGID = DefaultConfigIdGold FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else if (iMetalType == (int)MetalType.Silver)
                commandText.Append(" SELECT @FIXINGCONFIGID = DEFAULTCONFIGIDSILVER FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else if (iMetalType == (int)MetalType.Platinum)
                commandText.Append(" SELECT @FIXINGCONFIGID = DEFAULTCONFIGIDPLATINUM FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            else if (iMetalType == (int)MetalType.Palladium)
                commandText.Append(" SELECT @FIXINGCONFIGID = DEFAULTCONFIGIDPALLADIUM FROM INVENTPARAMETERS WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            commandText.Append(" SELECT @FIXINGCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = " + iMetalType + " AND CONFIGIDSTANDARD = @FIXINGCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");
            commandText.Append(" SELECT @TRANSCONFIGRATIO = CONFRATIO FROM CONFIGRATIO WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE =  " + iMetalType + " AND CONFIGIDSTANDARD = @TRANSCONFIGID AND  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            //commandText.Append(" IF(@FIXINGCONFIGRATIO >= @TRANSCONFIGRATIO) ");
            //commandText.Append(" BEGIN  ");
            //    commandText.Append(" IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) ");
            //    commandText.Append(" BEGIN  ");
            //        commandText.Append(" SELECT ISNULL((( @TRANSCONFIGRATIO * @QTY) / @FIXINGCONFIGRATIO),0) AS CONVERTEDQTY ");
            //    commandText.Append(" END  ");
            //    commandText.Append(" else");
            //    commandText.Append(" BEGIN ");
            //        commandText.Append(" SELECT @QTY AS CONVERTEDQTY ");
            //    commandText.Append(" END ");
            //commandText.Append(" END");
            //commandText.Append(" ELSE");
            //    commandText.Append(" BEGIN	");
            //        commandText.Append(" IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) ");
            //        commandText.Append(" BEGIN  ");
            //        commandText.Append(" SELECT ISNULL(((@FIXINGCONFIGRATIO * @QTY) / @TRANSCONFIGRATIO),0) AS CONVERTEDQTY   ");
            //        commandText.Append(" END  ");
            //    commandText.Append(" ELSE");
            //        commandText.Append(" BEGIN ");
            //        commandText.Append(" SELECT @QTY AS CONVERTEDQTY ");
            //        commandText.Append(" END ");
            //commandText.Append(" END  ");

            commandText.Append(" IF @istranstofixing = 'Y' BEGIN  IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) BEGIN  SELECT ISNULL(((@FIXINGCONFIGRATIO * @QTY) / @TRANSCONFIGRATIO),0) AS CONVERTEDQTY   END ");
            commandText.Append(" ELSE BEGIN SELECT @QTY AS CONVERTEDQTY END END");
            commandText.Append(" ELSE BEGIN IF(@FIXINGCONFIGRATIO > 0 AND @TRANSCONFIGRATIO > 0) BEGIN SELECT ISNULL(((@TRANSCONFIGRATIO * @QTY) / @FIXINGCONFIGRATIO),0) AS CONVERTEDQTY   END ");
            commandText.Append(" ELSE BEGIN SELECT @QTY AS CONVERTEDQTY END END");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;

            decimal dResult = Convert.ToDecimal(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return dResult;
        }

        private void btnAvgPurity_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtChangedGrossWt.Text) && !string.IsNullOrEmpty(txtGrossWt.Text))
            {
                decimal dAvgQty = 0m;
                #region Avg Purity
                if (dPurityReading1 > 0)
                {
                    frmPurityReading objPR = new frmPurityReading(dPurityReading1, dPurityReading2, dPurityReading3, sPurityPerson, sPurityPersonName);
                    dPurityReading1 = 0;
                    dPurityReading2 = 0;
                    dPurityReading3 = 0;
                    sPurityPerson = "";
                    sPurityPersonName = "";

                    objPR.ShowDialog();

                    dPurityReading1 = objPR.dReading1;
                    dPurityReading2 = objPR.dReading2;
                    dPurityReading3 = objPR.dReading3;

                    dAvgQty = decimal.Round(((dPurityReading1 + dPurityReading2 + dPurityReading3) / 3), 4, MidpointRounding.AwayFromZero);

                    txtAvgPurity.Text = Convert.ToString(decimal.Round(dAvgQty, 4, MidpointRounding.AwayFromZero));
                    objPR.Close();

                }
                else
                {
                    frmPurityReading objPR = new frmPurityReading();
                    objPR.ShowDialog();

                    dPurityReading1 = objPR.dReading1;
                    dPurityReading2 = objPR.dReading2;
                    dPurityReading3 = objPR.dReading3;
                    sPurityPerson = objPR.sCode;
                    sPurityPersonName = objPR.sName;

                    txtAvgPurity.Text = Convert.ToString(decimal.Round(((objPR.dReading1 + objPR.dReading2 + objPR.dReading3) / 3), 4, MidpointRounding.AwayFromZero)); //Convert.ToString();
                    objPR.Close();
                }
                #endregion

                CalculateNetWtAmount();
            }
        }

        private void txtPStoneWt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void txtPStonePcs_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }

            //if(e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            //{
            //    e.Handled = true;
            //}
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void txtPStoneRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtPStoneRate_Leave(object sender, EventArgs e)
        {
            txtPStoneAmt.Text = "";
            CalculateStoneWtAmount();
            CalculateNetWtAmount();
        }

        private void txtPStoneWt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtPStoneWt.Text))
            {
                string s = txtPStoneWt.Text.ToString();
                if (!IsNumeric(s))
                {
                    string[] parts = s.Split('.');
                    Int64 iValue = 0;
                    if (!string.IsNullOrEmpty(parts[0]))
                    {
                        iValue = Int64.Parse(parts[0]);
                    }
                    Int64 i2;

                    if (!string.IsNullOrEmpty((parts[1])))
                        i2 = Int64.Parse(parts[1]);

                    string sI2 = parts[1];
                    string sAfterDecimalValue = string.Empty;
                    if (sI2.ToString().Length > 3)
                    {
                        MessageBox.Show("Maximum decimal places is three.");
                        txtPStoneWt.Focus();
                    }
                    else
                    {
                        CalculateNetWtAmount();
                    }
                }
                else
                {
                    CalculateNetWtAmount();
                }
            }
            else
            {
                CalculateNetWtAmount();
            }
        }

        private void txtPStoneAmt_Leave(object sender, EventArgs e)
        {
            txtPStoneAmt.Text = "";
            CalculateStoneWtAmount();
            CalculateNetWtAmount();
        }

        private void txtPStoneAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
            if (e.KeyChar == (Char)Keys.Enter)
            {
                e.Handled = true;
            }
        }

        private void txtMkAmt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }

            if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
            {
                e.Handled = true;
            }
        }

        private void txtMkAmt_Leave(object sender, EventArgs e)
        {
            CalculateNetWtAmount();
        }

    }
}
