/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using LSRetailPosis.DevUtilities;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Customer.ViewModels;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using System.Data.SqlClient;
using System.Data;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using CS = Microsoft.Dynamics.Retail.Pos.Customer.Customer;
using System.Text.RegularExpressions;

namespace Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch
{
    partial class frmNewCustomer : frmTouchBase
    {
        private CustomerViewModel viewModel;
        private AddressViewModel addressViewModel;

        #region Nimbus
        int intRes = 0;
        int intKarigar = 0;

        string sISDCode = string.Empty;

        DataTable dtSalutation;
        DataTable dtGender;
        Int64 iSalutation = 5; //other
        int iGender = 0;
        #region Enum IdProof
        /// <summary>
        /// added on 16/09/2013 by RHossain
        /// </summary>
        private enum IdentityProof
        {
            Aadhar = 0,
            PAN = 1,
            Driving_License = 2,
            Passport_No = 3,
            Voter_Id = 4,
            Emp_Id = 5,
        }

        enum MonthsOfYear // added on 03/12/2014 RH
        {
            None = 0,
            January = 1,
            February = 2,
            March = 3,
            April = 4,
            May = 5,
            June = 6,
            July = 7,
            August = 8,
            September = 9,
            October = 10,
            November = 11,
            December = 12,
        }
        private DataTable GetDataTable(string sSQL)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                SqlComm.CommandText = sSQL;// "SELECT REPAIRID AS [Repair No],OrderDate as [Order Date],CUSTACCOUNT AS [Customer Account], CustName as [Customer Name] from RetailRepairReturnHdr where CUSTACCOUNT = '" + sCustAccount + "' AND IsDelivered = 0";

                DataTable dt = new DataTable();
                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dt);

                return dt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        #endregion

        string sISDC = string.Empty;
        string sCustAgeBracket = string.Empty;
        string sNationality = string.Empty;
        string sCustClassGrp = string.Empty;

        #endregion

        private const string FilterFormat =
            @"[OrderDate] LIKE '%{0}%' 
            OR [OrderNumber] LIKE '%{0}%' 
            OR [StoreName] LIKE '%{0}%' 
            OR [OrderStatus] LIKE '%{0}%' 
            OR [ItemName] LIKE '%{0}%' 
            OR [ItemQuantity] LIKE '%{0}%' 
            OR [ItemAmount] LIKE '%{0}%'";

        public frmNewCustomer(ICustomer newCustomer, IAddress newAddress)
            : this(newCustomer, null, newAddress)
        {
        }

        public frmNewCustomer(ICustomer newCustomer, ICustomer invoiceCustomer, IAddress newAddress)
        {
            InitializeComponent();

            //Start:Nim
            #region Salutation
            //dtSalutation = new DataTable();

            //dtSalutation.Columns.Add("Name", typeof(String));
            //dtSalutation.Columns.Add("Value", typeof(int));

            //dtSalutation.Rows.Add("HE", 0);
            //dtSalutation.Rows.Add("HH", 1);
            //dtSalutation.Rows.Add("M/S", 2);
            //dtSalutation.Rows.Add("Mr.", 3);
            //dtSalutation.Rows.Add("Mrs.", 4);
            //dtSalutation.Rows.Add("Ms.", 5);
            //dtSalutation.Rows.Add("Sayyid", 6);
            //dtSalutation.Rows.Add("Sayyida", 7);
            //dtSalutation.Rows.Add("Sheikh", 8);
            //dtSalutation.Rows.Add("Sheikha", 9);
            //dtSalutation.AcceptChanges();
            #endregion

            #region Gender

            dtGender = new DataTable();
            dtGender.Columns.Add("Name", typeof(String));
            dtGender.Columns.Add("Value", typeof(int));

            dtGender.Rows.Add("Unknown", 0);
            dtGender.Rows.Add("Male", 1);
            dtGender.Rows.Add("Female", 2);
            dtGender.AcceptChanges();

            #endregion
            //End:Nim

            // set and bind the VM for the customer fields
            this.viewModel = new CustomerViewModel(newCustomer, invoiceCustomer);
            this.bindingSource.Add(this.viewModel);

            // set the VM for the address control
            addressViewModel = new AddressViewModel(newAddress, newCustomer);
            this.viewAddressUserControl1.SetProperties(newCustomer, addressViewModel);
            this.viewAddressUserControl1.SetEditable(true);

            //Start:Nim
            GetCompanyISDCode(this.addressViewModel.Country);

            txtCustClassificationGroup.Text = GetValueById("select CustClassificationId from RETAILSTORETABLE where STORENUMBER='" + ApplicationSettings.Terminal.StoreId + "'");

            EditCustomerFillData(newCustomer);// Nimbus


            labelName.Visible = this.viewModel.IsNameVisible;// Nimbus recall
            textBoxName.Visible = this.viewModel.IsNameVisible;// Nimbus recall

            this.viewAddressUserControl1.SetEditable(true);
            //End:Nim


            this.gridOrders.DataBindings.Add("DataSource", this.bindingSource, "Items");

            if (Functions.CountryRegion != SupportedCountryRegion.BR)
            {
                //labelCpfCnpjNumber.Visible = false;
                //textBoxCpfCnpjNumber.Visible = false;
            }

            if (Functions.CountryRegion == SupportedCountryRegion.IN)
            {
                txtNimCountry.Text = "IND";
                this.addressViewModel.Country = "IND";
            }

            // Set formatter to convert sales status enum to text
            this.columnOrderStatus.DisplayFormat.Format = new SalesOrderStatusFormatter();
        }

        public ICustomer Customer
        {
            get { return this.viewModel.Customer; }
        }
        public IAddress Address
        {
            get { return addressViewModel.Address; }
        }

        protected override void OnLoad(EventArgs e)
        {
            if (!this.DesignMode)
            {
                TranslateLabels();
                SetTextBoxFocus();
                SetRadioButtonsEnabledState();

                this.timer1.InvokeOnTick(
                    ApplicationSettings.Terminal.AutoLogOffTimeOutInMin * 30, /*seconds per half-minute (half-time for popups)*/
                    this.Close);
            }

            base.OnLoad(e);
        }

        private void SetRadioButtonsEnabledState()
        {
            // enable the customer type radio buttons only if a new customer 
            this.radioOrg.Enabled = this.viewModel.Customer.IsEmptyCustomer();
            this.radioPerson.Enabled = this.viewModel.Customer.IsEmptyCustomer();
        }

        private void TranslateLabels()
        {
            lblHeader.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(56329); // customer information:
            labelName.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51129); // Name:
            labelFirstName.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51166); // First Name:
            labelMiddleName.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51167); // Middle Name:
            labelLastName.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51168); // Last Name:
            labelGroup.Text = "Govt. Id";// LSRetailPosis.ApplicationLocalizer.Language.Translate(51124); // Customer group:
            //labelCurrency.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51125); // Currency:
            labelLanguage.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51137); // Language:

            labelPhone.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51134); // Phone number:
            labelEmail.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51138); // E-mail:
            //labelReceiptEmail.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51128); // Receipt e-mail:
            //labelWebSite.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51127); // Web site:
            //labelCpfCnpjNumber.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51207); // CPF / CNPJ:

            labelDateCreated.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51214); // Date created:
            labelTotalVisits.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51213); // Total visits:
            labelBalance.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(1566); // Balance:

            labelInvAccountName.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51225); // Invoice account name:
            labelInvAccountId.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51226); // Invoice account ID:
            labelInvBalance.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51227); // Invoice acount balance:

            labelSearch.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51209); // Sales orders:
            labelStoreLastVisited.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51211); // Store last visited:
            labelTotalSales.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51210); // Total sales:
            labelDateLastVisit.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51212); // Date of last visit:

            tabPageContact.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51216); // Customer details
            tabPageHistory.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51215); // Transaction history

            columnAmount.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51217); // Amount
            columnDate.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51223); // Date
            columnItem.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51220); // Item
            columnOrderNumber.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51222); // Order number
            columnOrderStatus.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51224); // Order status
            columnQuantity.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51219); // Quantity
            columnStore.Caption = LSRetailPosis.ApplicationLocalizer.Language.Translate(51221); // Store

            labelType.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51169); // Type:
            radioOrg.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51165); // Organization
            radioPerson.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51164); // Person

            btnCancel.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(56319); // Cancel
            btnSave.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(56318); // Save
            btnClear.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51149); // Clear

            this.Text = LSRetailPosis.ApplicationLocalizer.Language.Translate(51041); // Customer
        }

        #region events
        protected override void OnHelpRequested(HelpEventArgs hevent)
        {
            if (hevent == null)
                throw new ArgumentNullException("hevent");

            LSRetailPosis.POSControls.POSFormsManager.ShowHelp(this);

            hevent.Handled = true;
            base.OnHelpRequested(hevent);
        }

        private void OnCustomerGroup_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteSelectGroup();
        }

        private void OnCurrency_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteSelectCurrency();
        }

        private void OnLanguage_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteSelectLanguage();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteClear();
            this.addressViewModel.ExecuteClear();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected override void OnClosing(CancelEventArgs e)
        {
            if (this.DialogResult == System.Windows.Forms.DialogResult.OK)
            {
                if (!this.SaveCustomer())
                {
                    e.Cancel = true;
                }
            }
            base.OnClosing(e);
        }

        #endregion

        private void SetSearchFilter(string p)
        {
            string filter = string.Empty;

            if (!string.IsNullOrWhiteSpace(p))
            {
                filter = string.Format(FilterFormat, p);
            }
            this.gridView.ActiveFilterString = filter;
        }

        private void SetTextBoxFocus()
        {
            //Base
            /*if (this.viewModel.IsPerson)
            {
                //Person
                textBoxFirstName.Select();
            }
            else
            {
                //Organization
                textBoxName.Select();
            }*/

            #region Nim
            if (this.viewModel.IsPerson)
            {
                btnSalutation.Visible = true;
                lblSalutation.Visible = true;
                txtSalutation.Visible = true;
                //Person
                btnSalutation.Select();
                //textBoxFirstName.Select();
            }
            else
            {
                //Organization
                btnSalutation.Visible = false;
                lblSalutation.Visible = false;
                txtSalutation.Visible = false;
                textBoxName.Select();
            }
            #endregion
        }

        private bool SaveCustomer()
        {
            try
            {
                bool createdLocal = false;
                bool createdAx = false;
                string comment = null;

                //Start:Nim
                //string sReligion = null;
                string sCustClassificationFGrp = null;
                string sStaffId = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
                //sReligion = txtReligion.Text.Trim();
                sCustClassificationFGrp = txtCustClassificationGroup.Text;
                //End:Nim

                DialogResult prompt = Pos.Customer.Customer.InternalApplication.Services.Dialog.ShowMessage(51148, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (prompt == System.Windows.Forms.DialogResult.Yes)
                {
                    IList<Int64> entityKeys = new List<Int64>();
                    this.addressViewModel.Address.FormattedAddress = this.addressViewModel.FormattedAddress;
                    ICustomer tempCustomer = MergeAddress(this.viewModel.Customer, this.addressViewModel.Address);
                    bool isEmptyCustomer = this.viewModel.Customer.IsEmptyCustomer();

                    //Start:Nim
                    if (chkResidence.Checked)// added on 07/12/2015 req by K.Saha
                        intRes = 1;
                    else
                        intRes = 0;

                    if (chkKarigar.Checked)// added on 29/12/2016 req by S.Sharma
                        intKarigar = 1;
                    else
                        intKarigar = 0;
                    //End:Nim

                    // this.isEmptyCustomer is initialized at form load and uses the incoming customer object
                    if (isEmptyCustomer)
                    {
                        if (ValidateControls()) // nimbus
                        {
                            //Base
                            // Attempt to save in AX
                            //Pos.Customer.Customer.InternalApplication.TransactionServices.NewCustomer(ref createdAx, ref comment, ref tempCustomer, ApplicationSettings.Terminal.StorePrimaryId, ref entityKeys);

                            //Start:Nim
                            // Attempt to save in AX
                            //Pos.Customer.Customer.InternalApplication.TransactionServices.NewCustomer(ref createdAx, ref comment, ref tempCustomer, ApplicationSettings.Terminal.StorePrimaryId, ref entityKeys);
                            ReadOnlyCollection<object> containerArray;

                            containerArray = Pos.Customer.Customer.InternalApplication.TransactionServices.InvokeExtension("CustAccountDuplicateCheck",
                                                                                                     tempCustomer.FirstName + " " + tempCustomer.MiddleName + " " + tempCustomer.LastName,
                                                                                                     txtMobilePrimary.Text.Trim());

                            bool bIsDuplicate = Convert.ToBoolean(containerArray[1]);

                            if (bIsDuplicate == false)
                                Pos.Customer.Customer.InternalApplication.TransactionServices.NewCustomer(ref createdAx, ref comment, ref tempCustomer, ApplicationSettings.Terminal.StorePrimaryId, ref entityKeys);
                            else
                                MessageBox.Show("Same name with same mobile is already exist.");
                            //End:Nim

                        }
                        #region Nimbus UpdateRetailCustomerInfo RTS
                        if (createdAx)
                        {
                            DataTable dtCustTable = new DataTable();
                            DataTable dtCardTable = new DataTable();
                            ReadOnlyCollection<object> containerArray;
                            containerArray = Pos.Customer.Customer.InternalApplication.TransactionServices.InvokeExtension("UpdateRetailCustomerInfo",
                                                                                                      tempCustomer.CustomerId, iSalutation, iGender, //dtDOB.EditValue,
                                                                                                      txtReligion.Text.Trim(), txtOccupation.Text.Trim(),// dtMarriage.EditValue,
                                                                                                      txtSTD.Text.Trim(), txtMobilePrimary.Text.Trim(), txtMobileSecondary.Text.Trim(),
                                                                                                      textCustAgeBracket.Text.Trim(), cmbBMonth.SelectedIndex,
                                                                                                      txtBDay.Text.Trim(), txtBYear.Text.Trim(), cmbAnnMonth.SelectedIndex, txtAnnDay.Text.Trim(),
                                                                                                      txtAnnYear.Text.Trim(), ApplicationSettings.Database.StoreID, intRes,
                                                                                                      txtCustClassificationGroup.Text.Trim(), cmbIdType.SelectedIndex, txtIdNo.Text.Trim(),
                                                                                                      intKarigar, textBoxEmail.Text.Trim(), txtBankAcc.Text.Trim(),
                                                                                                      txtIFSCCode.Text.Trim(), txtBankName.Text.Trim(), sStaffId,
                                                                                                      cmbIdType2.SelectedIndex, txtIdNo2.Text.Trim(), txtPIN.Text.Trim());

                            /*if (Convert.ToBoolean(containerArray[1]) == true)
                            {
                                DataSet dsRETAILLOYALTYCUSTTABLE = new DataSet();
                                DataSet dsRETAILLOYALTYMSRCARDTABLE = new DataSet();
                                StringReader srRETAILLOYALTYCUSTTABLE = new StringReader(Convert.ToString(containerArray[3]));
                                StringReader srRETAILLOYALTYMSRCARDTABLE = new StringReader(Convert.ToString(containerArray[4]));

                                if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                                {
                                    dsRETAILLOYALTYCUSTTABLE.ReadXml(srRETAILLOYALTYCUSTTABLE);
                                }
                                if (dsRETAILLOYALTYCUSTTABLE != null && dsRETAILLOYALTYCUSTTABLE.Tables[0].Rows.Count > 0)
                                {
                                    dtCustTable = dsRETAILLOYALTYCUSTTABLE.Tables[0];
                                }

                                if (Convert.ToString(containerArray[4]).Trim().Length > 38)
                                {
                                    dsRETAILLOYALTYMSRCARDTABLE.ReadXml(srRETAILLOYALTYMSRCARDTABLE);
                                }
                                if (dsRETAILLOYALTYMSRCARDTABLE != null && dsRETAILLOYALTYMSRCARDTABLE.Tables[0].Rows.Count > 0)
                                {
                                    dtCardTable = dsRETAILLOYALTYMSRCARDTABLE.Tables[0];
                                }

                                GetCustomer(dtCustTable, dtCardTable);

                            }*/
                        }
                        #endregion
                    }
                    else
                    {
                        Pos.Customer.Customer.UpdateCustomer(ref createdAx, ref comment, ref tempCustomer, ref entityKeys);
                        //Start:Nim
                        if (createdAx)
                        {


                            Pos.Customer.Customer.InternalApplication.TransactionServices.InvokeExtension("UpdateRetailCustomerInfo",
                                                                                                     tempCustomer.CustomerId, iSalutation, iGender, //dtDOB.EditValue,
                                                                                                     txtReligion.Text.Trim(), txtOccupation.Text.Trim(),// dtMarriage.EditValue,
                                                                                                     txtSTD.Text.Trim(), txtMobilePrimary.Text.Trim(), txtMobileSecondary.Text.Trim(),
                                                                                                     textCustAgeBracket.Text.Trim(), cmbBMonth.SelectedIndex,
                                                                                                     txtBDay.Text.Trim(), txtBYear.Text.Trim(), cmbAnnMonth.SelectedIndex, txtAnnDay.Text.Trim(),
                                                                                                     txtAnnYear.Text.Trim(), ApplicationSettings.Database.StoreID, intRes,
                                                                                                     txtCustClassificationGroup.Text.Trim(), cmbIdType.SelectedIndex, txtIdNo.Text.Trim(),
                                                                                                     intKarigar, textBoxEmail.Text.Trim(), txtBankAcc.Text.Trim(),
                                                                                                     txtIFSCCode.Text.Trim(), txtBankName.Text.Trim(), sStaffId,
                                                                                                     cmbIdType2.SelectedIndex, txtIdNo2.Text.Trim(), txtPIN.Text.Trim());




                        }
                        //End:Nim
                    }

                    // Was the customer created in AX
                    if (createdAx)
                    {
                        // Was the customer created locally
                        DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                            ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);

                        LSRetailPosis.Transaction.Customer transactionalCustomer = tempCustomer as LSRetailPosis.Transaction.Customer;

                        if (isEmptyCustomer)
                        {
                            createdLocal = customerDataManager.SaveTransactionalCustomer(transactionalCustomer, entityKeys);
                        }
                        else
                        {
                            createdLocal = customerDataManager.UpdateTransactionalCustomer(transactionalCustomer, entityKeys);
                        }

                        //Update the VM
                        this.viewModel = new CustomerViewModel(tempCustomer);

                        #region Nimbus

                        UpdateCustomerInfo(tempCustomer.CustomerId);

                        #endregion
                    }

                    if (!createdAx)
                    {
                        Pos.Customer.Customer.InternalApplication.Services.Dialog.ShowMessage(51159, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else if (!createdLocal)
                    {
                        Pos.Customer.Customer.InternalApplication.Services.Dialog.ShowMessage(51156, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                return createdAx && createdLocal;
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                Pos.Customer.Customer.InternalApplication.Services.Dialog.ShowMessage(51158, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }

        private static ICustomer MergeAddress(ICustomer iCustomer, IAddress iAddress)
        {
            if (iCustomer != null && iAddress != null)
            {
                iCustomer.AddressComplement = iAddress.BuildingComplement;
                iCustomer.City = iAddress.City;

                iCustomer.Country = iAddress.Country;
                iCustomer.County = iAddress.County;
                iCustomer.DistrictName = iAddress.DistrictName;

                iCustomer.Extension = iAddress.Extension;
                iCustomer.OrgId = iAddress.OrgId;
                iCustomer.PostalCode = iAddress.PostalCode;
                iCustomer.State = iAddress.State;
                iCustomer.StreetName = iAddress.StreetName;
                iCustomer.AddressNumber = iAddress.StreetNumber;
                iCustomer.Address = iAddress.FormattedAddress;
                iCustomer.PrimaryAddress.AddressType = iAddress.AddressType;
                iCustomer.PrimaryAddress.Name = iAddress.Name;
                iCustomer.PrimaryAddress.Email = iAddress.Email;
                iCustomer.PrimaryAddress.Telephone = iAddress.Telephone;
                iCustomer.PrimaryAddress.URL = iAddress.URL;
                iCustomer.PrimaryAddress.SalesTaxGroup = iAddress.SalesTaxGroup;

                iCustomer.PrimaryAddress.CountryISOCode = iAddress.CountryISOCode;
                iCustomer.PrimaryAddress.StateDescription = iAddress.StateDescription;
                iCustomer.PrimaryAddress.CountyDescription = iAddress.CountyDescription;
                iCustomer.PrimaryAddress.CityRecId = iAddress.CityRecId;
                iCustomer.PrimaryAddress.CityDescription = iAddress.CityDescription;
                iCustomer.PrimaryAddress.DistrictRecId = iAddress.DistrictRecId;
                iCustomer.PrimaryAddress.DistrictDescription = iAddress.DistrictDescription;

                switch (iCustomer.RelationType)
                {
                    case RelationType.Person:
                        // send party name in fixed format
                        string name = Utility.JoinStrings(" ", iCustomer.FirstName, iCustomer.MiddleName, iCustomer.LastName);

                        if (!string.IsNullOrWhiteSpace(name))
                        {
                            iCustomer.Name = name;
                        }
                        break;
                    default:
                        // No change
                        break;
                }
            }
            return iCustomer;
        }

        private void buttonSearch_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            SetSearchFilter(this.textSearch.Text.Trim());
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            SetSearchFilter(string.Empty);
            this.textSearch.Text = string.Empty;
        }

        private void tabControlParent_SelectedPageChanged(object sender, DevExpress.XtraTab.TabPageChangedEventArgs e)
        {
            timer1.Restart(); //reset the timer.            
            if (e.Page == this.tabPageHistory)
            {
                this.BeginInvoke(new Action(this.viewModel.ExecuteLoadHistory));
                this.BeginInvoke(new Action(this.viewModel.ExecuteLoadBalances));
            }
        }

        private void btnPgUp_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            gridView.MovePrevPage();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            gridView.MovePrev();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            gridView.MoveNext();
        }

        private void btnPgDown_Click(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            gridView.MoveNextPage();
        }

        private void textSearch_Enter(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            this.AcceptButton = buttonSearch;
        }

        private void textSearch_Leave(object sender, EventArgs e)
        {
            timer1.Restart(); //reset the timer.
            this.AcceptButton = null;
        }

        //Start: Nim
        #region Nim
        private void btnSalutation_Click(object sender, EventArgs e)
        {
            //Dialog.WinFormsTouch.frmGenericLookup oLook = new Dialog.WinFormsTouch.frmGenericLookup(CustBalDt, 3, "");

            BlankOperations.BlankOperations objBl = new BlankOperations.BlankOperations();
            dtSalutation = objBl.NIM_LoadCombo("DirNameAffix", "Affix as Name,RECID as Id", " where AffixType=1");

            DataRow drSal = null;
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(dtSalutation, 0, ref drSal, "Salutation");
            if (dialogResult == DialogResult.OK && drSal != null)
            {
                iSalutation = Convert.ToInt64(drSal["Id"]);
                txtSalutation.Text = Convert.ToString(drSal["Name"]);
            }
        }

        private void EditCustomerFillData(ICustomer newCustomer)
        {
            this.viewModel.ExecuteSelectGroup();
            //GetCompanyISDCode(this.addressViewModel.Country);
            if (!string.IsNullOrEmpty(txtNimCountry.Text))
                txtSTD.Text = GetValueById("select ISDCODE from LOGISTICSADDRESSCOUNTRYREGION where COUNTRYREGIONID='" + txtNimCountry.Text + "'");

            // txtCustClassificationGroup.Text = GetValueById("select CustClassificationId from RETAILSTORETABLE where STORENUMBER='" + ApplicationSettings.Terminal.StoreId + "'");

            cmbBMonth.DataSource = Enum.GetValues(typeof(MonthsOfYear));
            cmbAnnMonth.DataSource = Enum.GetValues(typeof(MonthsOfYear));
            cmbIdType.DataSource = Enum.GetValues(typeof(IdentityProof));
            cmbIdType2.DataSource = Enum.GetValues(typeof(IdentityProof));

            if (newCustomer.CustomerId != null)
            {
                try
                {
                    if (Pos.Customer.Customer.InternalApplication.TransactionServices.CheckConnection())
                    {
                        ReadOnlyCollection<object> containerArray;
                        string sStoreId = ApplicationSettings.Terminal.StoreId;

                        containerArray = Pos.Customer.Customer.InternalApplication.TransactionServices.InvokeExtension("GetRetailCustomerInfo", newCustomer.CustomerId);

                        DataSet dsRC = new DataSet();
                        StringReader srTransDetail = new StringReader(Convert.ToString(containerArray[3]));

                        if (Convert.ToString(containerArray[3]).Trim().Length > 38)
                        {
                            dsRC.ReadXml(srTransDetail);
                        }
                        if (dsRC != null && dsRC.Tables[0].Rows.Count > 0)
                        {
                            iSalutation = Convert.ToInt64((dsRC.Tables[0].Rows[0]["PersonalTitle"]));

                            txtSalutation.Text = GetValueById("select AFFIX  from DIRNAMEAFFIX where RECID='" + Convert.ToString((dsRC.Tables[0].Rows[0]["PersonalTitle"]) + "'"));
                            //txtReligion.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["Religion"]));
                            txtOccupation.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["Occupation"]));
                            txtSTD.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["RetailSTDCode"]));
                            txtMobilePrimary.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["RetailMobilePrimary"]));
                            txtMobilePrimary.Enabled = false;
                            txtMobileSecondary.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["RetailMobileSecondary"]));
                            txtMobileSecondary.Enabled = false;
                            //textNationality.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["CitizenshipCountryRegion"]));
                            textCustAgeBracket.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["CustAgeBracket"]));
                            cmbBMonth.SelectedIndex = Convert.ToInt16((dsRC.Tables[0].Rows[0]["BirthMonth"]));
                            txtBDay.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["BirthDay"]));
                            txtBYear.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["BirthYear"]));
                            cmbAnnMonth.SelectedIndex = Convert.ToInt16((dsRC.Tables[0].Rows[0]["AnniversaryMonth"]));
                            txtAnnDay.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["AnniversaryDay"]));
                            txtAnnYear.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["AnniversaryYear"]));

                            txtCustClassificationGroup.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["CustClassificationId"]));

                            // this.Customer.CustGroup = txtCustClassificationGroup.Text;

                            cmbIdType.SelectedIndex = Convert.ToInt16((dsRC.Tables[0].Rows[0]["GovtIdentity"]));
                            txtIdNo.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["GovtIdNo"]));
                            txtGender.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["Gender"]));

                            if (!string.IsNullOrEmpty(Convert.ToString((dsRC.Tables[0].Rows[0]["Resident"]))))
                            {
                                if (Convert.ToInt16((dsRC.Tables[0].Rows[0]["Resident"])) == 1)
                                    chkResidence.Checked = true;
                                else
                                    chkResidence.Checked = false;
                            }

                            if (!string.IsNullOrEmpty(Convert.ToString((dsRC.Tables[0].Rows[0]["ISKARIGAR"]))))
                            {
                                if (Convert.ToInt16((dsRC.Tables[0].Rows[0]["ISKARIGAR"])) == 1)
                                    chkKarigar.Checked = true;
                                else
                                    chkKarigar.Checked = false;
                            }
                            this.viewModel.NationalityId = Convert.ToString((dsRC.Tables[0].Rows[0]["CitizenshipCountryRegion"]));
                            txtReligion.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["Religion"]));

                            if (dtGender != null && dtGender.Rows.Count > 0)
                            {
                                foreach (DataRow drNew in dtGender.Select("Name='" + Convert.ToString((dsRC.Tables[0].Rows[0]["Gender"])) + "'"))
                                {
                                    iGender = Convert.ToInt16(drNew["Value"]);
                                }
                            }

                            txtBankAcc.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["BankAccount"]));
                            txtIFSCCode.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["IFSCCode"]));
                            txtBankName.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["BankName"]));

                            if (!string.IsNullOrEmpty(Convert.ToString((dsRC.Tables[0].Rows[0]["ISAPPROVED"]))))
                            {
                                if (Convert.ToInt16((dsRC.Tables[0].Rows[0]["ISAPPROVED"])) == 1)
                                    chkApproved.Checked = true;
                                else
                                    chkApproved.Checked = false;
                            }

                            if (chkApproved.Checked)
                            {
                                txtBankAcc.Enabled = false;
                                txtIFSCCode.Enabled = false;
                                txtBankName.Enabled = false;
                                btnOS.Enabled = false;
                            }
                            else
                            {
                                txtBankAcc.Enabled = true;
                                txtIFSCCode.Enabled = true;
                                txtBankName.Enabled = true;
                                btnOS.Enabled = true;
                            }
                            cmbIdType2.SelectedIndex = Convert.ToInt16((dsRC.Tables[0].Rows[0]["GovtIdentity2"]));
                            txtIdNo2.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["GovtIdNo2"]));
                            txtPIN.Text = Convert.ToString((dsRC.Tables[0].Rows[0]["CRWZipCode"]));

                            //iGender = Convert.ToInt16((dsRC.Tables[0].Rows[0]["Gender"]));

                            txtNimState.Text = newCustomer.State;
                            txtNimCountry.Text = newCustomer.Country;
                            txtSTD.Enabled = false;
                            btnNationality.Enabled = false;
                        }
                    }
                }
                catch (Exception ex)
                {

                }

            }
        }

        public string GetValueById(string sSql)
        {
            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(sSql.ToString());

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();
            if (!string.IsNullOrEmpty(sResult))
            {
                return sResult.Trim();
            }
            else
            {
                return "-";
            }
        }

        private void txtBDay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtAnnDay_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtBYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void txtAnnYear_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void btnNationality_Click(object sender, EventArgs e)
        {
            this.viewModel.ExecuteNationality();

            txtSTD.Text = GetValueById("select ISDCODE from LOGISTICSADDRESSCOUNTRYREGION where COUNTRYREGIONID='" + txtBankAcc.Text + "'");
        }

        private void btnAgeBracket_Click(object sender, EventArgs e)
        {
            DataTable dtCustAgeBracket = new DataTable();
            string sSqlstr = "SELECT BracketCode,[Description]  FROM [DBO].[CRWCUSTAGEBRACKET]";
            dtCustAgeBracket = GetDataTable(sSqlstr);

            DataRow drSal = null;
            //var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(dtCustAgeBracket, 2, ref drSal, "Cust Age Bracket");
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericSearch(dtCustAgeBracket, ref drSal, "Cust Age Bracket");
            if (dialogResult == DialogResult.OK && drSal != null)
            {
                sCustAgeBracket = Convert.ToString(drSal["BracketCode"]);
                textCustAgeBracket.Text = sCustAgeBracket;
            }
        }

        private void btnReligion_Click(object sender, EventArgs e)
        {
            this.addressViewModel.NimExecuteSelectReligion();
            txtReligion.Text = this.addressViewModel.sReligion;
        }

        private void btnGender_Click(object sender, EventArgs e)
        {
            DataRow drGen = null;
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(dtGender, 0, ref drGen, "Gender");
            if (dialogResult == DialogResult.OK && drGen != null)
            {
                iGender = Convert.ToInt16(drGen["Value"]);
                txtGender.Text = Convert.ToString(drGen["Name"]);
            }
        }

        private void btnNimCountry_Click(object sender, EventArgs e)
        {
            this.addressViewModel.ExecuteSelectCountry();

            txtNimCountry.Text = this.addressViewModel.Country;
            txtNimState.Text = this.addressViewModel.State;
        }

        private void btnNimState_Click(object sender, EventArgs e)
        {
            this.addressViewModel.ExecuteSelectState();
            txtNimState.Text = this.addressViewModel.State;
        }

        private void btnOC_Click(object sender, EventArgs e)
        {
            this.addressViewModel.NimExecuteSelectOriginCountry();

            //txtIFSCCode.Text = this.addressViewModel.sOriginCountry;
            //txtBankName.Text = this.addressViewModel.sOriginState;
        }

        private void btnOS_Click(object sender, EventArgs e)
        {
            DataTable dtCustAgeBracket = new DataTable();
            string sSqlstr = "SELECT Code,[Description]  FROM [DBO].[BANKMASTER]";
            dtCustAgeBracket = GetDataTable(sSqlstr);

            DataRow drSal = null;
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericSearch(dtCustAgeBracket, ref drSal, "Bank List");
            if (dialogResult == DialogResult.OK && drSal != null)
            {
                string sBank = Convert.ToString(drSal["Code"]);
                txtBankName.Text = sBank;
            }
            //this.addressViewModel.NimExecuteSelectOriginState();
            //txtBankName.Text = this.addressViewModel.sOriginState;
        }
        private bool ValidateControls()
        {
            bool bReturn = true;

            //if(string.IsNullOrEmpty(textBoxEmail.Text.Trim())) // added on 03/12/2014 req by S.Sharma
            //{
            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
            //                                            frmMessage("Please enter a valid email.", MessageBoxButtons.OK, MessageBoxIcon.Error))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        textBoxEmail.Focus();
            //    }

            //    bReturn = false;

            //}

            if (string.IsNullOrEmpty(txtSalutation.Text.Trim())) // added on 03/12/2014 req by S.Sharma
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                        frmMessage("Please select a salutation.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }

                bReturn = false;
            }

            if (radioPerson.Checked)
            {
                if (string.IsNullOrEmpty(this.textBoxFirstName.Text.Trim()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                            frmMessage("Please enter first name.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }

                    bReturn = false;
                }

                if (string.IsNullOrEmpty(this.textBoxLastName.Text.Trim()))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                            frmMessage("Please enter last name.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }

                    bReturn = false;
                }
            }

            //if(string.IsNullOrEmpty(this.addressViewModel.City.Trim())) 
            //{
            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
            //                                            frmMessage("Please enter city.", MessageBoxButtons.OK, MessageBoxIcon.Error))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //    }

            //    bReturn = false;
            //}
            if (string.IsNullOrEmpty(this.addressViewModel.Country.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                        frmMessage("Please enter nationality.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                }

                bReturn = false;
            }
            else
            {
                string sIsStateMandatory = GetValueById("select STATEMANDATORY from LOGISTICSADDRESSCOUNTRYREGION where COUNTRYREGIONID='" + this.addressViewModel.Country.Trim() + "'");

                if (sIsStateMandatory == "1")
                {
                    if (string.IsNullOrEmpty(this.addressViewModel.State))
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                                frmMessage("Please enter state.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }

                        bReturn = false;
                    }
                }
            }

            if (!string.IsNullOrEmpty(txtIFSCCode.Text.Trim()))
            {
                string sIsStateMandatory = GetValueById("select STATEMANDATORY from LOGISTICSADDRESSCOUNTRYREGION where COUNTRYREGIONID='" + txtIFSCCode.Text.Trim() + "'");

                if (sIsStateMandatory == "1")
                {
                    if (string.IsNullOrEmpty(txtBankName.Text))
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                                frmMessage("Please enter state.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }

                        bReturn = false;
                    }
                }
            }

            if (string.IsNullOrEmpty(txtMobilePrimary.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                        frmMessage("Please enter a valid mobile no.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtMobilePrimary.Focus();
                }

                bReturn = false;
            }

            int iMinMobileDigit = 0;
            int iMaxMobileDigit = 0;

            iMinMobileDigit = Convert.ToInt16(GetValueById(" SELECT ISNULL(MINMOBILEDIGIT,0) AS MINMOBILEDIGIT FROM RETAILSTORETABLE  WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "'"));
            iMaxMobileDigit = Convert.ToInt16(GetValueById(" SELECT ISNULL(MAXMOBILEDIGIT,0) AS MAXMOBILEDIGIT FROM RETAILSTORETABLE  WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "'"));

            if (!string.IsNullOrEmpty(txtSTD.Text) && !string.IsNullOrEmpty(txtMobilePrimary.Text))
            {
                if (txtSTD.Text == sISDCode)
                {
                    if (txtMobilePrimary.Text.Length > iMaxMobileDigit || txtMobilePrimary.Text.Length < iMinMobileDigit)
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Mobile number should be within " + iMaxMobileDigit + "  digits", MessageBoxButtons.OK, MessageBoxIcon.Error))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                            txtMobilePrimary.Focus();
                            bReturn = false;
                        }
                    }
                }
            }
            //if(txtMobilePrimary.Text.Trim().Length > 13 || txtMobilePrimary.Text.Trim().Length < 7) // added on 27/06/2014 req by S.Sharma
            //{
            //    using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
            //                                            frmMessage("Please enter a valid mobile no.", MessageBoxButtons.OK, MessageBoxIcon.Error))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        txtMobilePrimary.Focus();
            //    }

            //    bReturn = false;
            //}

            //if (string.IsNullOrEmpty(txtBankAcc.Text.Trim()))
            //{
            //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
            //                                            frmMessage("Please select bank account.", MessageBoxButtons.OK, MessageBoxIcon.Error))
            //    {
            //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //        txtBankAcc.Focus();
            //    }

            //    bReturn = false;
            //}
            if (string.IsNullOrEmpty(txtNimCountry.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                        frmMessage("Please select nationality.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtNimCountry.Focus();
                }

                bReturn = false;
            }

            if (string.IsNullOrEmpty(txtCustClassificationGroup.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                        frmMessage("Please select customer classification group.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtCustClassificationGroup.Focus();
                }

                bReturn = false;
            }

            if (string.IsNullOrEmpty(txtPIN.Text.Trim()))
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.
                                                        frmMessage("Please select ZIP postal code.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    txtPIN.Focus();
                }

                bReturn = false;
            }

            return bReturn;
        }
        private void textBoxPhone_Leave(object sender, EventArgs e)
        {
            string sPhone = "";

            sPhone = Convert.ToString(txtSTD.Text).Trim() + Convert.ToString(textBoxPhone.Text).Trim();

            if (sPhone.Length > 13)
            {
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Phone number should be within 10 digits", MessageBoxButtons.OK, MessageBoxIcon.Error))
                {
                    LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    textBoxPhone.Focus();
                }
            }
        }
        private void txtSTD_Leave(object sender, EventArgs e)
        {
            string sPhone = "";
            int iMinMobileDigit = 0;
            int iMaxMobileDigit = 0;


            iMinMobileDigit = Convert.ToInt16(GetValueById(" SELECT ISNULL(MINMOBILEDIGIT,0) AS MINMOBILEDIGIT FROM RETAILSTORETABLE  WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "'"));
            iMaxMobileDigit = Convert.ToInt16(GetValueById(" SELECT ISNULL(MAXMOBILEDIGIT,0) AS MAXMOBILEDIGIT FROM RETAILSTORETABLE  WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "'"));

            sPhone = Convert.ToString(txtSTD.Text).Trim() + Convert.ToString(textBoxPhone.Text).Trim();

            //if(!string.IsNullOrEmpty(txtSTD.Text))
            //{
            //    if(txtSTD.Text == sISDCode)
            //    {
            //        if(sPhone.Length > iMaxMobileDigit || sPhone.Length < iMinMobileDigit)
            //        {
            //            using(LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Phone number should be within " + iMaxMobileDigit + "  digits", MessageBoxButtons.OK, MessageBoxIcon.Error))
            //            {
            //                LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
            //            }
            //        }
            //    }
            //}
        }
        private void textBoxPhone_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[0-9+]$")))
            {
                e.Handled = true;
            }
        }
        private void txtSTD_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[0-9+]$")))
            {
                e.Handled = true;
            }
        }
        private void txtMobilePrimary_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[0-9+]$")))
            {
                e.Handled = true;
            }
            if (txtMobilePrimary.Text.Length == 0)
            {
                if (e.KeyChar == '0')
                {
                    e.Handled = true;
                }
            }
        }
        private void UpdateCustomerInfo(string sCustomerId)
        {
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            string sStaffId = ApplicationSettings.Terminal.TerminalOperator.OperatorId;
            DateTime dtMarrage = Convert.ToDateTime("01-01-1900");
            DateTime dtDOB = Convert.ToDateTime("01-01-1900");


            if (cmbBMonth.SelectedIndex > 0 && !string.IsNullOrEmpty(txtBDay.Text) && !string.IsNullOrEmpty(txtBYear.Text))
            {
                dtDOB = Convert.ToDateTime(Convert.ToInt16(txtBDay.Text) + "-" + cmbBMonth.SelectedIndex + "-" + Convert.ToInt16(txtBYear.Text));
            }

            if (cmbAnnMonth.SelectedIndex > 0 && !string.IsNullOrEmpty(txtAnnDay.Text) && !string.IsNullOrEmpty(txtAnnYear.Text))
            {
                dtMarrage = Convert.ToDateTime(Convert.ToInt16(txtAnnDay.Text) + "-" + cmbAnnMonth.SelectedIndex + "-" + Convert.ToInt16(txtAnnYear.Text));
            }

            string commandText = " UPDATE CUSTTABLE SET RETAILSALUTATION=@RETAILSALUTATION,RETAILGENDER=@RETAILGENDER," +
                                 " RETAILDOB=@RETAILDOB,RELIGION=@RELIGION,OCCUPATION = @OCCUPATION,RETAILMARRIAGEDATE = @RETAILMARRIAGEDATE, " +//
                                 " RETAILSTDCODE =@RETAILSTDCODE, RETAILMOBILEPRIMARY = @RETAILMOBILEPRIMARY," +
                                 " RETAILMOBILESECONDARY=@RETAILMOBILESECONDARY,CustClassificationId=@CustClassificationId, " +
                                 " GOVTIDENTITY=@GOVTIDENTITY,GOVTIDNO=@GOVTIDNO,ISKARIGAR=@ISKARIGAR,CRWRECEIPTEMAIL=@CRWRECEIPTEMAIL," +
                //" ORIGINCOUNTRY=@ORIGINCOUNTRY,ORIGINSTATE=@ORIGINSTATE,"+
                                 " BankAccount=@BankAccount,IFSCCode=@IFSCCode,BankName=@BankName,RetailStaffId=@RetailStaffId," +
                                 " GOVTIDENTITY2=@GOVTIDENTITY2,GOVTIDNO2=@GOVTIDNO2,CRWZipCode=@CRWZipCode WHERE ACCOUNTNUM = '" + sCustomerId + "' ";

            SqlCommand command = new SqlCommand(commandText, SqlCon);

            SqlCon.Open();

            command.Parameters.Clear();
            command.Parameters.Add("@RETAILSALUTATION", SqlDbType.BigInt).Value = iSalutation;
            command.Parameters.Add("@RETAILGENDER", SqlDbType.Int).Value = iGender;
            command.Parameters.Add("@RETAILDOB", SqlDbType.DateTime).Value = dtDOB;
            command.Parameters.Add("@RELIGION", SqlDbType.NVarChar, 20).Value = txtReligion.Text.Trim();
            command.Parameters.Add("@OCCUPATION", SqlDbType.NVarChar, 60).Value = txtOccupation.Text.Trim();
            command.Parameters.Add("@RETAILMARRIAGEDATE", SqlDbType.DateTime).Value = dtMarrage;

            command.Parameters.Add("@RETAILSTDCODE", SqlDbType.NVarChar, 10).Value = txtSTD.Text.Trim();
            command.Parameters.Add("@RETAILMOBILEPRIMARY", SqlDbType.NVarChar, 20).Value = txtMobilePrimary.Text.Trim();
            command.Parameters.Add("@RETAILMOBILESECONDARY", SqlDbType.NVarChar, 20).Value = txtMobileSecondary.Text.Trim();
            command.Parameters.Add("@CustClassificationId", SqlDbType.NVarChar, 20).Value = txtCustClassificationGroup.Text.Trim();
            command.Parameters.Add("@GOVTIDENTITY", SqlDbType.Int).Value = cmbIdType.SelectedIndex;
            command.Parameters.Add("@GOVTIDNO", SqlDbType.NVarChar, 20).Value = txtIdNo.Text.Trim();
            command.Parameters.Add("@ISKARIGAR", SqlDbType.Int).Value = intKarigar;
            command.Parameters.Add("@CRWRECEIPTEMAIL", SqlDbType.NVarChar, 80).Value = textBoxEmail.Text.Trim();
            command.Parameters.Add("@BankAccount", SqlDbType.NVarChar, 20).Value = txtBankAcc.Text.Trim();
            command.Parameters.Add("@IFSCCode", SqlDbType.NVarChar, 20).Value = txtIFSCCode.Text.Trim();
            command.Parameters.Add("@BankName", SqlDbType.NVarChar, 20).Value = txtBankName.Text.Trim();
            command.Parameters.Add("@RetailStaffId", SqlDbType.NVarChar, 20).Value = sStaffId;
            command.Parameters.Add("@GOVTIDENTITY2", SqlDbType.Int).Value = cmbIdType2.SelectedIndex;
            command.Parameters.Add("@GOVTIDNO2", SqlDbType.NVarChar, 20).Value = txtIdNo2.Text.Trim();
            command.Parameters.Add("@CRWZipCode", SqlDbType.NVarChar, 6).Value = txtPIN.Text.Trim();


            command.ExecuteNonQuery();
            SqlCon.Close();
        }
        private void GetCompanyISDCode(string sCountryCode = "")
        {
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
            string sQry = string.Empty;

            //changes for malabar
            sQry = " SELECT ISNULL(ISDCODE,'') AS ISDCODE FROM RETAILSTORETABLE" +
                    "  WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "' ";

            SqlCommand cmd = new SqlCommand(sQry, SqlCon);
            cmd.CommandTimeout = 0;
            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();
            sISDCode = Convert.ToString(cmd.ExecuteScalar());

            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();

            txtSTD.Text = string.Empty;
            txtSTD.Text = sISDCode;

            if (txtMobilePrimary.Text.Trim().Length < 5) // added on 27/06/2014 req by S.Sharma
            {
                txtMobilePrimary.Text = string.Empty;
                txtMobileSecondary.Text = string.Empty;
            }
        }

        private void GetCustomer(DataTable dtCustTable, DataTable dtCardTable)
        {
            int iCustTable = 0;
            string sCustAcc = string.Empty;

            SqlTransaction transaction = null;

            #region RETAILLOYALTYCUSTTABLE
            if (dtCustTable != null && dtCustTable.Rows.Count > 0)
            {
                string commandText = " INSERT INTO [RETAILLOYALTYCUSTTABLE]([ACCOUNTNUM],[CUSTNAME],[LOYALTYCUSTID],[RECID],DATAAREAID)" +
                                     " VALUES(@ACCOUNTNUM,@CUSTNAME,@LOYALTYCUSTID,@RECID,@DATAAREAID)";

                SqlConnection connection = new SqlConnection();
                try
                {

                    connection = ApplicationSettings.Database.LocalConnection;


                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    transaction = connection.BeginTransaction();

                    SqlCommand command = new SqlCommand(commandText, connection, transaction);
                    command.Parameters.Clear();

                    sCustAcc = Convert.ToString(dtCustTable.Rows[0]["ACCOUNTNUM"]);
                    command.Parameters.Add("@ACCOUNTNUM", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCustTable.Rows[0]["ACCOUNTNUM"]);
                    command.Parameters.Add("@CUSTNAME", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCustTable.Rows[0]["CUSTNAME"]);
                    command.Parameters.Add("@LOYALTYCUSTID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCustTable.Rows[0]["LOYALTYCUSTID"]);
                    command.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtCustTable.Rows[0]["RECID"]);
                    command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

            #endregion

                    command.CommandTimeout = 0;
                    iCustTable = command.ExecuteNonQuery();

                    if (iCustTable == 1)
                    {
                        if (dtCardTable != null && dtCardTable.Rows.Count > 0)
                        {
                            #region RETAILLOYALTYMSRCARDTABLE
                            string commandDIRPARTYTABLE = " IF NOT EXISTS(SELECT TOP 1 RECID FROM RETAILLOYALTYMSRCARDTABLE " +
                                                        " WHERE RECID=@RECID ) BEGIN" +
                                                        " INSERT INTO [RETAILLOYALTYMSRCARDTABLE](LINKID,LINKTYPE,LOYALTYCUSTID," +
                                                        " LOYALTYTENDER,LOYALTYSCHEMEID,CARDNUMBER,RECID,DATAAREAID)" +
                                                        " VALUES(@LINKID,@LINKTYPE,@LOYALTYCUSTID,@LOYALTYTENDER,@LOYALTYSCHEMEID," +
                                                        " @CARDNUMBER,@RECID,@DATAAREAID) END";

                            SqlCommand cmdDirParty = new SqlCommand(commandDIRPARTYTABLE, connection, transaction);
                            if (string.IsNullOrEmpty(Convert.ToString(dtCardTable.Rows[0]["LINKID"])))
                                cmdDirParty.Parameters.Add("@LINKID", SqlDbType.NVarChar, 20).Value = "";
                            else
                                cmdDirParty.Parameters.Add("@LINKID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCardTable.Rows[0]["LINKID"]);

                            cmdDirParty.Parameters.Add("@LINKTYPE", SqlDbType.Int).Value = Convert.ToInt64(dtCardTable.Rows[0]["LINKTYPE"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtCardTable.Rows[0]["LOYALTYCUSTID"])))
                                cmdDirParty.Parameters.Add("@LOYALTYCUSTID", SqlDbType.NVarChar, 20).Value = "";
                            else
                                cmdDirParty.Parameters.Add("@LOYALTYCUSTID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCardTable.Rows[0]["LOYALTYCUSTID"]);

                            cmdDirParty.Parameters.Add("@LOYALTYTENDER", SqlDbType.Int).Value = Convert.ToInt64(dtCardTable.Rows[0]["LOYALTYTENDER"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtCardTable.Rows[0]["LOYALTYSCHEMEID"])))
                                cmdDirParty.Parameters.Add("@LOYALTYSCHEMEID", SqlDbType.NVarChar, 20).Value = "";
                            else
                                cmdDirParty.Parameters.Add("@LOYALTYSCHEMEID", SqlDbType.NVarChar, 20).Value = Convert.ToString(dtCardTable.Rows[0]["LOYALTYSCHEMEID"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtCardTable.Rows[0]["CARDNUMBER"])))
                                cmdDirParty.Parameters.Add("@CARDNUMBER", SqlDbType.NVarChar, 60).Value = "";
                            else
                                cmdDirParty.Parameters.Add("@CARDNUMBER", SqlDbType.NVarChar, 60).Value = Convert.ToInt64(dtCardTable.Rows[0]["CARDNUMBER"]);

                            if (string.IsNullOrEmpty(Convert.ToString(dtCardTable.Rows[0]["RECID"])))
                                cmdDirParty.Parameters.Add("@RECID", SqlDbType.BigInt).Value = 0;
                            else
                                cmdDirParty.Parameters.Add("@RECID", SqlDbType.BigInt).Value = Convert.ToInt64(dtCardTable.Rows[0]["RECID"]);
                            cmdDirParty.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4).Value = ApplicationSettings.Database.DATAAREAID;

                            cmdDirParty.CommandTimeout = 0;
                            cmdDirParty.ExecuteNonQuery();
                            cmdDirParty.Dispose();

                            #endregion
                        }
                    }

                    transaction.Commit();
                    command.Dispose();
                    transaction.Dispose();

                    //if (iCustTable == 1)
                    //{
                    //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Customer  " + sCustAcc + "   fetched successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    //    {
                    //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    //        this.Close();
                    //    }
                    //}
                    //else
                    //{
                    //    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("DataBase error occured.Please try again later.", MessageBoxButtons.OK, MessageBoxIcon.Error))
                    //    {
                    //        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    //    }
                    //}
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

        #endregion

        private void cmbIdType_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtIdNo.Text))
            {
                if (cmbIdType.SelectedIndex == (int)IdentityProof.PAN)
                {
                    if (!bISValidPAN(txtIdNo.Text))
                    {
                        MessageBox.Show("Invalid PAN no");
                        txtIdNo.Focus();
                    }
                }
                else if (cmbIdType.SelectedIndex == (int)IdentityProof.Aadhar)
                {
                    if (!bISValidAadhar(txtIdNo.Text))
                    {
                        MessageBox.Show("Invalid Aadhar no");
                        txtIdNo.Focus();
                    }
                }
            }
        }

        private bool bISValidPAN(string sPANNo)
        {
            string regex = @"^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$"; //@"[A-Z]{5}[0-9]{4}[A-Z]{1}";^([a-zA-Z]){5}([0-9]){4}([a-zA-Z]){1}?$

            return Regex.IsMatch(sPANNo, regex);
        }
        private bool bISValidAadhar(string sAadharNo)
        {
            string regex = @"^([0-9]){12}?$";

            return Regex.IsMatch(sAadharNo, regex);
        }

        private void txtIdNo_Validating(object sender, CancelEventArgs e)
        {
            cmbIdType_SelectedIndexChanged(sender, e);
        }

        private void textBoxFirstName_Leave(object sender, EventArgs e)
        {
            textBoxFirstName.Text = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Convert.ToString(textBoxFirstName.Text).ToLower());
        }

        private void textBoxMiddleName_Leave(object sender, EventArgs e)
        {
            textBoxMiddleName.Text = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Convert.ToString(textBoxMiddleName.Text).ToLower());
        }

        private void textBoxLastName_Leave(object sender, EventArgs e)
        {
            textBoxLastName.Text = System.Threading.Thread.CurrentThread.CurrentCulture.TextInfo.ToTitleCase(Convert.ToString(textBoxLastName.Text).ToLower());
        }

        private void textBoxLastName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[a-zA-Z]$")))
            {
                e.Handled = true;
            }
        }

        private void textBoxMiddleName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[a-zA-Z]$")))
            {
                e.Handled = true;
            }
        }

        private void textBoxFirstName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((!char.IsControl(e.KeyChar)) && (!Regex.IsMatch(Convert.ToString(e.KeyChar), @"[a-zA-Z]$")))
            {
                e.Handled = true;
            }
        }

        private void txtIFSCCode_Leave(object sender, EventArgs e)
        {
            txtIFSCCode.Text = Convert.ToString(txtIFSCCode.Text).ToUpper();
        }

        private void textBoxEmail_Validating(object sender, CancelEventArgs e)
        {
            string pattern = null;
            pattern = "^([0-9a-zA-Z]([_\\-\\.\\w]*[0-9a-zA-Z])*@([0-9a-zA-Z][-\\w]*[0-9a-zA-Z]\\.)+[a-zA-Z]{2,9})$";

            if (textBoxEmail.Text.Trim() == "")
            {
                return;
            }
            else
            {
                if (!(Regex.IsMatch(textBoxEmail.Text.Trim(), pattern)))
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("Not a valid Email address ", MessageBoxButtons.OK, MessageBoxIcon.Information))
                    {
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                    }
                    textBoxEmail.Select();
                    return;
                }
            }
        }

        private void cmbIdType2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txtIdNo2.Text))
            {
                if (cmbIdType2.SelectedIndex == (int)IdentityProof.PAN)
                {
                    if (!bISValidPAN(txtIdNo2.Text))
                    {
                        MessageBox.Show("Invalid PAN no");
                        txtIdNo2.Focus();
                    }
                }
                else if (cmbIdType2.SelectedIndex == (int)IdentityProof.Aadhar)
                {
                    if (!bISValidAadhar(txtIdNo2.Text))
                    {
                        MessageBox.Show("Invalid Aadhar no");
                        txtIdNo2.Focus();
                    }
                }
            }
        }

        private void txtIdNo2_Validating(object sender, CancelEventArgs e)
        {
            cmbIdType2_SelectedIndexChanged(sender, e);
        }

        private void radioOrg_Click(object sender, EventArgs e)
        {
            if (this.viewModel.Customer.IsEmptyCustomer())
            {
                labelName.Visible = this.viewModel.IsNameVisible;// Nimbus recall
                textBoxName.Visible = this.viewModel.IsNameVisible;// Nimbus recall
                lblFNS.Visible = false;
                lblLNS.Visible = false;

                this.Customer.CustGroup = GetValueById("select CORPORATECLASSIFICATIONID from RETAILSTORETABLE where STORENUMBER='" + ApplicationSettings.Terminal.StoreId + "'");
            }
        }

        private void radioPerson_CheckedChanged(object sender, EventArgs e)
        {
            if (this.viewModel.Customer.IsEmptyCustomer())
            {
                lblFNS.Visible = true;
                lblLNS.Visible = true;

                txtCustClassificationGroup.Text = GetValueById("select CustClassificationId from RETAILSTORETABLE where STORENUMBER='" + ApplicationSettings.Terminal.StoreId + "'");
            }
        }

        private void txtMobilePrimary_Leave(object sender, EventArgs e)
        {
            textBoxPhone.Text = txtMobilePrimary.Text;
            this.viewModel.Phone = txtMobilePrimary.Text;
        }

        private void btnCRWZip_Click(object sender, EventArgs e)
        {
            string sSqlstr = "select CITY,ZIPCODE from LOGISTICSADDRESSZIPCODE";

            DataTable dt = GetDataTable(sSqlstr);
            DataRow drSal = null;
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericSearch(dt, ref drSal, "ZIP CODE SEARCH");

            if (dialogResult == DialogResult.OK && drSal != null)
            {
                txtPIN.Text = Convert.ToString(drSal["ZIPCODE"]);
            }
        }


        //end:Nim
    }
}