using System;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmOptionSelectionGSSorCustomerOrder : frmTouchBase
    {
        Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations objBlank = new Microsoft.Dynamics.Retail.Pos.BlankOperations.BlankOperations();
        enum AdvAgainst
        {
            None = 0,
            OGPurchase = 1,
            OGExchange = 2,
            SaleReturn = 3,
        }
        enum TransactionType
        {
            Sale = 0,
            Purchase = 1,
            Exchange = 3,
            PurchaseReturn = 2,
            ExchangeReturn = 4,
        }
        string sCustId = string.Empty;

        public bool isGSS = false;
        public int iAdvAgainst = 0;
        public string sAdvAgainstVou = string.Empty;

        public frmOptionSelectionGSSorCustomerOrder()
        {
            InitializeComponent();
            cmbAdvAgainst.DataSource = Enum.GetValues(typeof(AdvAgainst));
        }

        public frmOptionSelectionGSSorCustomerOrder(IPosTransaction posTransaction)
        {
            InitializeComponent();
            cmbAdvAgainst.DataSource = Enum.GetValues(typeof(AdvAgainst));

             LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;
             if(custTrans != null && custTrans.Amount != 0)
             {
                 custTrans.PartnerData.EFTCardNo = string.Empty;

                 if(!string.IsNullOrEmpty(Convert.ToString(custTrans.Customer.CustomerId)))
                 {
                     sCustId = Convert.ToString(custTrans.Customer.CustomerId);
                 }
             }
        }

        public frmOptionSelectionGSSorCustomerOrder(IPosTransaction posTransaction,int iFromCustOrderOrRep)
        {
            InitializeComponent();
            if(iFromCustOrderOrRep == 1)
            {
                btnGSS.Visible = false;
            }

            if(iFromCustOrderOrRep == 2)
            {
                btnGSS.Visible = false;
                cmbAdvAgainst.Visible = false;
                label4.Visible = false;
                label5.Visible = false;
                cmbVoucher.Visible = false;
            }
            //else
            //{
            //    cmbAdvAgainst.Visible = true;
            //    label4.Visible = true;
            //    label5.Visible = true;
            //    cmbVoucher.Visible = true;
            //}

            cmbAdvAgainst.DataSource = Enum.GetValues(typeof(AdvAgainst));

            LSRetailPosis.Transaction.CustomerPaymentTransaction custTrans = posTransaction as LSRetailPosis.Transaction.CustomerPaymentTransaction;
            if(custTrans != null && custTrans.Amount != 0)
            {
                custTrans.PartnerData.EFTCardNo = string.Empty;

                if(!string.IsNullOrEmpty(Convert.ToString(custTrans.Customer.CustomerId)))
                {
                    sCustId = Convert.ToString(custTrans.Customer.CustomerId);
                }
            }
        }

        private void btnNormalCustomerDeposit_Click(object sender, EventArgs e)
        {
            isGSS = false;
            iAdvAgainst=cmbAdvAgainst.SelectedIndex;
            sAdvAgainstVou = cmbVoucher.Text.Trim() == null ? string.Empty : cmbVoucher.Text.Trim();

            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            isGSS = true;
            this.Close();
        }

        private void cmbAdvAgainst_SelectedIndexChanged(object sender, EventArgs e)
        {
            int iTransType = 0;
            cmbVoucher.DataSource = null;

            switch(Convert.ToString(cmbAdvAgainst.SelectedIndex))
            {
                case "1":
                    iTransType = (int)TransactionType.Purchase;
                    break;
                case "2":
                    iTransType = (int)TransactionType.Exchange;
                    break;
                case "3":
                    iTransType = 100;//sales return
                    break;
                default:
                    iTransType = 0;
                    break;
            }

            if(iTransType > 0)
            {
                string sSQl = "select rct.TRANSACTIONTYPE as TRANSACTIONTYPE,rtst.RECEIPTID as RECEIPTID from RETAIL_CUSTOMCALCULATIONS_TABLE rct";
                sSQl = sSQl + " left join RETAILTRANSACTIONSALESTRANS rtst on rct.TRANSACTIONID=rtst.TRANSACTIONID";
                sSQl = sSQl + " and rct.TERMINALID=rtst.TERMINALID and rct.STOREID=rtst.STORE";
                sSQl = sSQl + " and rct.LINENUM=rtst.LINENUM where rtst.CUSTACCOUNT='" + sCustId + "'";
                sSQl = sSQl + " and isnull(rtst.RECEIPTID,'')!=''";
                sSQl = sSQl + " and CONVERT(VARCHAR(11),TRANSDATE,103)='" + DateTime.Now.Date.ToShortDateString() + "'";
                sSQl = sSQl + " and isnull(rtst.RECEIPTID,'') not in (select VOUCHERAGAINST from CUSTORDER_HEADER where CUSTACCOUNT='" + sCustId + "')";
                sSQl = sSQl + " and isnull(rtst.RECEIPTID,'') not in (select RECEIPTID from RETAILTRANSACTIONTABLE where CUSTACCOUNT='" + sCustId + "' AND ADVANCEDONE=1)";
                //select * from RETAILTRANSACTIONTABLE where RECEIPTID='4001R102IR001063' AND ADVANCEDONE=0

                if(iTransType == 100)
                    sSQl = sSQl + " and rtst.RETURNNOSALE=1 ";
                else
                    sSQl = sSQl + " and rct.TRANSACTIONTYPE =" + iTransType + "";

                sSQl = sSQl + " order by rct.TRANSACTIONID desc";


                cmbVoucher.Items.Add(" ");
                cmbVoucher.DataSource = objBlank.NIM_LoadCombo("", "", "", sSQl);
                cmbVoucher.DisplayMember = "RECEIPTID";
                cmbVoucher.ValueMember = "TRANSACTIONTYPE";
            }
        }
    }
}
