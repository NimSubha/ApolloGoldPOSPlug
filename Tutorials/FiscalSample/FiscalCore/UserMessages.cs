using System;
using System.Windows.Forms;

namespace Microsoft.Dynamics.Retail.Pos.FiscalCore
{
    public static class UserMessages
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.MessageBox.Show(System.String,System.String,System.Windows.Forms.MessageBoxButtons,System.Windows.Forms.MessageBoxIcon,System.Windows.Forms.MessageBoxDefaultButton)"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions")]
        public static bool ContinueWithPrinterMismatch()
        {
            DialogResult result = MessageBox.Show(
                "The connected printer does not match the value in the persisted store.  Would you like to continue?",
                "Printer Changed",
                MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);

            return (result == DialogResult.Yes);
        }

        public static string RequestCustomerTaxId(string initialCustomerTaxId)
        {
            string result = string.Empty;
            if (initialCustomerTaxId == null)
            {
                initialCustomerTaxId = string.Empty;
            }

            using (CustomerTaxIdForm theForm = new CustomerTaxIdForm())
            {
                theForm.CustomerTaxId = initialCustomerTaxId;
                theForm.ShowDialog();
                result = theForm.CustomerTaxId;
            }

            return result;
        }
    }
}
