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

namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    partial class AddressUserControl : UserControl
    {
        private AddressViewModel viewModel;

        public AddressUserControl()
        {
            InitializeComponent();

            //this.viewModel = new AddressViewModel();
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                TranslateLabels();
                this.viewModel.SetElementSelection();
            }
            base.OnLoad(e);
        }

        public void SetViewModel(AddressViewModel model)
        {
            this.viewModel = model;
            this.bindingSource.Add(this.viewModel);
            this.viewModel.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(OnViewModel_PropertyChanged);
        }

        void OnViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void TranslateLabels()
        {
            labelAddress.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51199); // ADDRESS
            labelStreet.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51131); // Street:
            labelCity.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51130); // City:
            labelCounty.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51206); // County:
            labelCountry.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51117); // Country/Region:
            labelState.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51119); // State:
            labelZip.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51118); // ZIP/Postal code:
            labelDistrict.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51143); // District name:
            labelOrganization.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51133); // Organization number:
            labelStateDescription.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51119); // State:
            labelCountyDescription.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51206); // County:
            labelCityDescription.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51130); // City:
            labelDistrictDescription.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51143); // District name:

            labelStreetNumber.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51200); // Street number:
            labelBuildingComplement.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51201); // Building complement:
            labelPostbox.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51202); // Postbox:
        }

        private void OnStateButton_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteSelectState();
        }

        private void OnZipButton_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteSelectZipPostalCode();
        }

        private void OnCountryButton_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteSelectCountry();
        }

        private void OnCountyButton_Click(object sender, System.EventArgs e)
        {
            this.viewModel.ExecuteSelectCounty();
        }

        private void OnCityButton_Click(object sender, System.EventArgs e)
        {
            this.viewModel.ExecuteSelectCity();
        }

        private void OnDistrictButton_Click(object sender, System.EventArgs e)
        {
            this.viewModel.ExecuteSelectDistrict();
        }
    }
}