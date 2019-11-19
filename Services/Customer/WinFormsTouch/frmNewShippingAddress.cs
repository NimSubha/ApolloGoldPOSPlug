/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.ComponentModel;
using System.Windows.Forms;
using LSRetailPosis.POSProcesses;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Customer.ViewModels;

namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    partial class frmNewShippingAddress : frmTouchBase
    {
        private AddressViewModel viewModel;

        public frmNewShippingAddress(ICustomer existingCustomer, IAddress newAddress)
        {
            InitializeComponent();

            this.Customer = existingCustomer;

            // set and bind the VM for the customer fields
            this.viewModel = new AddressViewModel(newAddress, existingCustomer);
            this.addressViewModelBindingSource.Add(this.viewModel);

            // set the VM for the address control
            addressUserControl1.SetViewModel(this.viewModel);
        }

        public IAddress Address
        {
            get { return viewModel.Address; }
        }

        public ICustomer Customer { get; private set; }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                TranslateLabels();

                this.viewModel.PropertyChanged += new PropertyChangedEventHandler(OnViewModel_PropertyChanged);
            }

            base.OnLoad(e);
        }

        void OnViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        private void TranslateLabels()
        {
            labelName.Text          = LSRetailPosis.ApplicationLocalizer.Language.Translate(51129); // Name:
            labelAddressType.Text   = LSRetailPosis.ApplicationLocalizer.Language.Translate(904); // Address Type:
            labelSalesTaxGroup.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51123); // Sales Tax Group:
            labelPhone.Text         = LSRetailPosis.ApplicationLocalizer.Language.Translate(51134); // Phone number:
            labelEmail.Text         = LSRetailPosis.ApplicationLocalizer.Language.Translate(51138); // E-mail:
            labelWebSite.Text       = LSRetailPosis.ApplicationLocalizer.Language.Translate(51127); // Web site:
            labelContactInfo.Text   = LSRetailPosis.ApplicationLocalizer.Language.Translate(56308); // Contact information

            if (0 == this.viewModel.Address.Id)
            {
                labelHeading.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51162); // Add address
            }
            else
            {
                labelHeading.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51163); // Edit address
            }

            btnCancel.Text          = LSRetailPosis.ApplicationLocalizer.Language.Translate(56319); // Cancel
            btnSave.Text            = LSRetailPosis.ApplicationLocalizer.Language.Translate(56318); // Save
            btnClear.Text           = LSRetailPosis.ApplicationLocalizer.Language.Translate(51149); // Clear

            this.Text               = LSRetailPosis.ApplicationLocalizer.Language.Translate(51183); // Clear
        }

        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if (hevent == null)
                throw new ArgumentNullException("hevent");

            LSRetailPosis.POSControls.POSFormsManager.ShowHelp(this);

            hevent.Handled = true;
            base.OnHelpRequested(hevent);
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            // Clear the customer
            //Start:Nim
            // Added on 12.08.2013 // nimbus -- this block was blank

            viewModel.Street = string.Empty;
            viewModel.City = string.Empty;
            viewModel.DistrictName = string.Empty;
            viewModel.BuildingComplement = string.Empty;
            viewModel.Postbox = string.Empty;
            //  viewModel.County = string.Empty;
            viewModel.State = string.Empty;
            viewModel.Zip = string.Empty;
            viewModel.Country = string.Empty;
            //
            textBoxName.Text = string.Empty;
            textBoxAddressType.Text = string.Empty;
            textBoxSalesTaxGroup.Text = string.Empty;
            textBoxPhone.Text = string.Empty;
            textBoxEmail.Text = string.Empty;
            textBoxWebSite.Text = string.Empty;
            //End:Nim
            //------
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (e == null)
            {
                throw new ArgumentNullException("e");
            }

            // only tries to save the address if the customer also exists on AX
            if (this.DialogResult == DialogResult.OK
                && !string.IsNullOrWhiteSpace(this.Customer.RecordId)
                && !this.viewModel.SaveShippingAddress(this.Customer))
            {
                e.Cancel = true;
            }

            base.OnClosing(e);
        }

        private void OnAddressType_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteSelectAddressType();
        }

        private void OnSalesTaxGroup_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteSelectSalesTaxGroup();
        }
    }
}
