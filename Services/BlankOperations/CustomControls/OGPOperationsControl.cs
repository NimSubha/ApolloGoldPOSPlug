using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    sealed partial class OGPOperationsControl : OrderDetailsPage
    {
        public string sPieces = string.Empty;
        public string sGoldWeight = string.Empty;
        public string sRate = string.Empty;
        public string sCalculationType = string.Empty;
        public string sTotalWeight = string.Empty;
        public string sLossPercentage = string.Empty;
        public string sLossWeight = string.Empty;
        public string sExpectedQuantity = string.Empty;
        public string sTotalAmount = string.Empty;
        public string txtAmount = string.Empty;
        Helper oHelper = new Helper();
        public string sNumPadEntryType = string.Empty;
        #region enum  RateType
        enum RateType
        {
            Weight = 0,
            Pieces = 1,
            Tot = 2,
        }
        #endregion
        public OGPOperationsControl()
        {
            InitializeComponent();
            sTotalAmount = lblTAmount.Text = string.Empty;
            sExpectedQuantity = lblExpectedQty.Text = string.Empty;
            sCalculationType = txtRateType.Text = Convert.ToString(RateType.Weight);
        }

        protected override void OnViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Transaction":
                    bindingSource.ResetBindings(false);
                    break;
                default:
                    break;
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {


            sCalculationType = txtRateType.Text = oHelper.ConvertEnumToDataTable("RateType", "RATETYPE", txtRateType.Text);
            txtRateType_TextChanged(sender, e);
        }

        #region Key press and text changed events
        private void txtPCS_TextChanged(object sender, EventArgs e)
        {
            txtRateType_TextChanged(sender, e);
            sPieces = txtPCS.Text;
        }

        private void txtQuantity_TextChanged(object sender, EventArgs e)
        {
            txtRateType_TextChanged(sender, e);
            sGoldWeight = txtQuantity.Text;
        }

        private void txtRate_TextChanged(object sender, EventArgs e)
        {
            txtRateType_TextChanged(sender, e);
            sRate = txtRate.Text;
        }

        private void txtRateType_TextChanged(object sender, EventArgs e)
        {
            if (txtRateType.Text.ToUpper().Trim() == Convert.ToString(RateType.Weight).ToUpper().Trim() && !string.IsNullOrEmpty(txtRate.Text.Trim()) && !string.IsNullOrEmpty(txtLossWt.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * Convert.ToDecimal(lblExpectedQty.Text.Trim());
                sTotalAmount = lblTAmount.Text = txtAmount = Convert.ToString(decimalAmount);
            }
            if (txtRateType.Text.ToUpper().Trim() == Convert.ToString(RateType.Weight).ToUpper().Trim() && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtLossWt.Text.Trim())))
            {
                sTotalAmount = lblTAmount.Text = txtAmount = string.Empty;
            }
            if (txtRateType.Text.ToUpper().Trim() == Convert.ToString(RateType.Pieces).ToUpper().Trim() && !string.IsNullOrEmpty(txtRate.Text.Trim()) && !string.IsNullOrEmpty(txtPCS.Text.Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = Convert.ToDecimal(txtRate.Text.Trim()) * Convert.ToDecimal(txtPCS.Text.Trim());
                sTotalAmount = lblTAmount.Text = txtAmount = Convert.ToString(decimalAmount);
            }
            if (txtRateType.Text.ToUpper().Trim() == Convert.ToString(RateType.Pieces).ToUpper().Trim() && (string.IsNullOrEmpty(txtRate.Text.Trim()) || string.IsNullOrEmpty(txtPCS.Text.Trim())))
            {
                sTotalAmount = lblTAmount.Text = txtAmount = string.Empty;
            }
            if (txtRateType.Text.ToUpper().Trim() == Convert.ToString(RateType.Tot).ToUpper().Trim() && !string.IsNullOrEmpty(txtRate.Text.Trim().Trim()))
            {
                Decimal decimalAmount = 0m;
                decimalAmount = Convert.ToDecimal(txtRate.Text.Trim());
                sTotalAmount = lblTAmount.Text = txtAmount = Convert.ToString(decimal.Round(decimalAmount, 2, MidpointRounding.AwayFromZero));
            }
            if (string.IsNullOrEmpty(txtRate.Text.Trim()))
            {
                sTotalAmount = lblTAmount.Text = txtAmount = string.Empty;
            }
        }

        private void txtTotalWeight_TextChanged(object sender, EventArgs e)
        {

            txtQuantity.Text = string.IsNullOrEmpty(txtTotalWeight.Text.Trim()) ? string.Empty : txtTotalWeight.Text.Trim();
            txtLossPct_TextChanged(sender, e);
            sTotalWeight = txtTotalWeight.Text;

        }

        private void txtLossPct_TextChanged(object sender, EventArgs e)
        {

            if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()) && !string.IsNullOrEmpty(txtLossPct.Text.Trim()))
            {
                decimal CalPerLossWt = Convert.ToDecimal(txtQuantity.Text.Trim()) * Convert.ToDecimal(txtLossPct.Text.Trim()) / 100;
                decimal CalLossWt = Convert.ToDecimal(txtQuantity.Text.Trim()) - Convert.ToDecimal(CalPerLossWt);
                sExpectedQuantity = lblExpectedQty.Text = Convert.ToString(CalLossWt);
                sLossWeight = txtLossWt.Text = Convert.ToString(CalPerLossWt);

            }
            if (string.IsNullOrEmpty(txtQuantity.Text.Trim()) || string.IsNullOrEmpty(txtLossPct.Text.Trim()))
            {
                sLossWeight = txtLossWt.Text = string.Empty;
                sExpectedQuantity = lblExpectedQty.Text = string.Empty;

            }
            txtLossWt_Leave(sender, e);

            txtRateType_TextChanged(sender, e);
            sLossPercentage = txtLossPct.Text;
        }

        private void txtLossWt_TextChanged(object sender, EventArgs e)
        {
            txtRateType_TextChanged(sender, e);
            sLossWeight = txtLossWt.Text;
        }

        private void lblExpectedQty_Click(object sender, EventArgs e)
        {
            txtRateType_TextChanged(sender, e);
            sExpectedQuantity = lblExpectedQty.Text;
        }

        private void txtLossWt_Leave(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtQuantity.Text.Trim()) && !string.IsNullOrEmpty(txtLossWt.Text.Trim()))
            {
                decimal dLossWt = 0m;
                dLossWt = Convert.ToDecimal(txtQuantity.Text.Trim()) - Convert.ToDecimal(lblExpectedQty.Text.Trim());
                txtLossWt.Text = Convert.ToString(dLossWt);
                txtRateType_TextChanged(sender, e);
            }
            sLossWeight = txtLossWt.Text;
        }

        private void txtPCS_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = oHelper.ValidateKeyPress(sender, e, 0);

        }

        private void txtQuantity_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = oHelper.ValidateKeyPress(sender, e, 0);
        }

        private void txtRate_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = oHelper.ValidateKeyPress(sender, e, 1);
        }

        private void txtTotalWeight_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = oHelper.ValidateKeyPress(sender, e, 1);
        }

        private void txtLossPct_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = oHelper.ValidateKeyPress(sender, e, 1);
        }

        private void txtLossWt_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = oHelper.ValidateKeyPress(sender, e, 1);
        }
        #endregion

        private void txtPCS_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "P";
            numpadEntry.PromptText = "Enter Pieces:";
        }

        private void txtQuantity_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "Q";
            numpadEntry.PromptText = "Enter Gold Weight:";
        }

        private void txtRate_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "R";
            numpadEntry.PromptText = "Enter Rate:";
        }

        private void txtTotalWeight_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "TW";
            numpadEntry.PromptText = "Enter Total Weight:";
        }

        private void txtLossPct_Enter(object sender, EventArgs e)
        {
            sNumPadEntryType = "LP";
            numpadEntry.PromptText = "Enter Loss Percentage:";
        }

        private void numpadEntry_EnterButtonPressed()
        {
            if (string.IsNullOrEmpty(txtPCS.Text) || sNumPadEntryType.ToUpper().Trim() == "P")
            {
                txtPCS.Text = numpadEntry.EnteredValue;
                numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;

                txtRate.Focus();
                numpadEntry.PromptText = "Enter Rate:";


                numpadEntry.Refresh();
                numpadEntry.Focus();

                return;
            }

            if (string.IsNullOrEmpty(txtTotalWeight.Text) || sNumPadEntryType.ToUpper().Trim() == "TW")
            {
                txtTotalWeight.Text = numpadEntry.EnteredValue;
                numpadEntry.PromptText = "Enter Rate:";
                numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                txtRate.Focus();
                numpadEntry.Refresh();
                numpadEntry.Focus();
                numpadEntry.EnteredValue = string.Empty;
                return;
            }

            if (string.IsNullOrEmpty(txtRate.Text) || sNumPadEntryType.ToUpper().Trim() == "R")
            {
                txtRate.Text = numpadEntry.EnteredValue;

                numpadEntry.PromptText = "Enter Total Weight:";

                numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price;
                txtTotalWeight.Focus();
            }

            else if (string.IsNullOrEmpty(txtTotalWeight.Text) || sNumPadEntryType.ToUpper().Trim() == "TW")
            {

                txtTotalWeight.Text = numpadEntry.EnteredValue;
                numpadEntry.PromptText = "Enter Loss Percentage:";
                numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;
                txtLossPct.Focus();

            }

            else if (string.IsNullOrEmpty(txtLossPct.Text) || sNumPadEntryType.ToUpper().Trim() == "LP")
            {
                txtLossPct.Text = numpadEntry.EnteredValue;
                numpadEntry.PromptText = "Enter Pieces:";
                numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price;
                sNumPadEntryType = "P";
            }

            if (numpadEntry.EntryType == Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Price)
                numpadEntry.EntryType = Microsoft.Dynamics.Retail.Pos.Contracts.UI.NumpadEntryTypes.Numeric;

            numpadEntry.Refresh();
            numpadEntry.Focus();
            numpadEntry.EnteredValue = string.Empty;
            return;
        }

        private void lblExpectedQty_TextChanged(object sender, EventArgs e)
        {
            txtRateType_TextChanged(sender, e);
            sExpectedQuantity = lblExpectedQty.Text;
        }

        private void chkOwn_CheckedChanged(object sender, EventArgs e)
        {

        }


    }
}
