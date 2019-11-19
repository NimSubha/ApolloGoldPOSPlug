using System.Windows.Forms;

namespace Microsoft.Dynamics.Retail.Pos.FiscalCore
{
    public partial class CustomerTaxIdForm : Form
    {
        public CustomerTaxIdForm()
        {
            InitializeComponent();
        }

        public string CustomerTaxId { get; set; }

        private void CustomerTaxIdForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.DialogResult == DialogResult.OK)
            {
                string newTaxId = txtCustomerTaxId.Text.Trim();
                this.CustomerTaxId = newTaxId;
            }
        }
    }
}
