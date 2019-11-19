/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Windows.Forms;
using Microsoft.Dynamics.Retail.Pos.Customer.ViewModels;
using DE = Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;


namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    /// <summary>
    /// Control shows a given address formatted according to the country in a view only box.
    /// Button to open frmNewShippingAddress to edit address can be set (default is set)
    /// Label to display the caption Address can be set/unset or rewritten with a string resource id.
    /// Requires address and customer to be set through SetProperties method.
    /// </summary>
    public partial class ViewAddressUserControl : UserControl
    {
        private AddressViewModel viewModel;
        private DE.ICustomer customer;
       // private int textLabelAddress = 1669;
        private bool editable = true;

        //private int textLabelAddress = 1669;//Base
        //start:Nim
        private int textLabelAddress = 999996; // changed on 12.07.2013
        //End:Nim

        public ViewAddressUserControl()
        {
            InitializeComponent();
        }

        public void SetEditable(bool value)
        {
            editable = value;
            btnEdit.Visible = value;
        }

        public void SetCaption(int value)
        {
            this.textLabelAddress = value;
            TranslateLabels();
        }

        public void SetCaptionVisibility(bool value)
        {
            labelAddress.Visible = value;
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                TranslateLabels();
            }
            base.OnLoad(e);
        }

        private void TranslateLabels()
        {
            labelAddress.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(textLabelAddress); // Address:
            btnEdit.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51163); // Edit Address
        }

        private void btnEdit_Click(object sender, EventArgs e)
        {
            if (!editable)
            {
                return;
            }
            using (frmNewShippingAddress newShippingAddressDialog = new frmNewShippingAddress(customer, viewModel.Address))
            {
                Customer.InternalApplication.ApplicationFramework.POSShowForm(newShippingAddressDialog);

                if (newShippingAddressDialog.DialogResult == DialogResult.OK)
                {
                    this.lblFormattedAddress.Text = viewModel.FormattedAddress;
                }
            }
        }

        public void SetProperties(DE.ICustomer parameterCustomer, AddressViewModel parameterViewModel)
        {
            this.viewModel = parameterViewModel;
            this.customer = parameterCustomer;
            this.viewAddressBindingSource.Clear();
            this.viewAddressBindingSource.Add(this.viewModel);
        }

        public void SetProperties(DE.ICustomer parameterCustomer, DE.IAddress address)
        {
            this.viewModel = new AddressViewModel(address, parameterCustomer);
            this.customer = parameterCustomer;
            this.viewAddressBindingSource.Clear();
            this.viewAddressBindingSource.Add(this.viewModel);
        }
    }
}
