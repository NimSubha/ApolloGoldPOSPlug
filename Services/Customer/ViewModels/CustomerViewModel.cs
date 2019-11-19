/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;
using LSRetailPosis;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using CS = Microsoft.Dynamics.Retail.Pos.Customer.Customer;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using System.ComponentModel.Composition;

namespace Microsoft.Dynamics.Retail.Pos.Customer.ViewModels
{
    class CustomerViewModel : INotifyPropertyChanged
    {
        private readonly string CaptionCurrency = LSRetailPosis.ApplicationLocalizer.Language.Translate(51125); // Currency:
        private readonly string CaptionCustomerGroup = LSRetailPosis.ApplicationLocalizer.Language.Translate(51124); // Customer group:
        private readonly string CaptionLanguage = LSRetailPosis.ApplicationLocalizer.Language.Translate(51137); // Language:

        private Lazy<DM.CustomerDataManager> custDataManager = new Lazy<DM.CustomerDataManager>(
            () => new DM.CustomerDataManager(
                        CS.InternalApplication.Settings.Database.Connection,
                        CS.InternalApplication.Settings.Database.DataAreaID));

        private IList<DataEntity.Language> languages;
        private CustomerHistory history;
        private bool historyLoadedFromAx;   //Only TRUE if the history has been populated from AX, otherwise history is blank/empty.
        private bool isBalanceLoadedFromAx; // Only TRUE if the balances and credit limit has been loaded from from AX, otherwise balance related fields re blank/empty.
        private bool hasInvoiceAccount;   // Only TRUE if the customer has a linked invoice account.

        //Start:Nim
        private string _nationality;
        private string _nationalityId;
        private string _religion;
        //End:Nim

        //Start:Nim
        private IApplication application;

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
                //  InternalApplication = value;
            }
        }
        //End:Nim

        public CustomerViewModel(ICustomer customer)
            : this(customer, null)
        {
        }

        public CustomerViewModel(ICustomer customer, ICustomer invoiceCustomer)
        {
            this.Customer = customer;
            this.history = new CustomerHistory();

            if (invoiceCustomer != null)
            {
                this.hasInvoiceAccount = true;
                this.InvoiceCustomer = invoiceCustomer;
            }
        }

        public ICustomer Customer
        {
            get;
            private set;
        }

        public ICustomer InvoiceCustomer
        {
            get;
            private set;
        }

        public string Name
        {
            get { return this.Customer.Name; }
            set
            {
                if (this.Customer.Name != value)
                {
                    this.Customer.Name = value;
                    OnPropertyChanged("Name");
                    OnPropertyChanged("Validated");
                }
            }
        }

        public string InvoiceName
        {
            get
            {
                if (this.hasInvoiceAccount)
                {
                    return this.InvoiceCustomer.Name;
                }
                else
                {
                    return null;
                }
            }
        }

        public string InvoiceAccountId
        {
            get
            {
                if (this.hasInvoiceAccount)
                {
                    return this.InvoiceCustomer.CustomerId;
                }
                else
                {
                    return null;
                }
            }
        }

        public bool IsNameVisible
        {
            get
            {
                if (Customer.IsEmptyCustomer())
                {
                    return ((IsOrganization || Customer.IsEmptyCustomer()) && !IsPerson);
                }
                else
                    return true;
                
            }
        }

        //start:nim
        public string Religion
        {
            get { return this._religion; }
            set
            {
                if (this._religion != value)
                {
                    this._religion = value;
                    OnPropertyChanged("Religion");
                    //OnPropertyChanged("Validated");
                }
            }
        }
        public string Nationality
        {
            get { return this._nationality; }
            set
            {
                if (this._nationality != value)
                {
                    this._nationality = value;
                    OnPropertyChanged("Nationality");
                    //OnPropertyChanged("Validated");
                }
            }
        }
        public string NationalityId
        {
            get { return this._nationalityId; }
            set
            {

                this._nationalityId = value;
                OnPropertyChanged("NationalityId");
            }
        }
        //end:nim


        public string FirstName
        {
            get { return this.Customer.FirstName; }
            set
            {
                if (this.Customer.FirstName != value)
                {
                    this.Customer.FirstName = value;
                    OnPropertyChanged("FirstName");
                    OnPropertyChanged("Validated");
                }
            }
        }

        public string MiddleName
        {
            get { return this.Customer.MiddleName; }
            set
            {
                if (this.Customer.MiddleName != value)
                {
                    this.Customer.MiddleName = value;
                    OnPropertyChanged("MiddleName");
                    OnPropertyChanged("Validated");
                }
            }
        }

        public string LastName
        {
            get { return this.Customer.LastName; }
            set
            {
                if (this.Customer.LastName != value)
                {
                    this.Customer.LastName = value;
                    OnPropertyChanged("LastName");
                    OnPropertyChanged("Validated");
                }
            }
        }

        public bool AreSplitNameFieldsVisible
        {
            get
            {
                return (IsPerson && Customer.IsEmptyCustomer());
            }
        }

        public string CustomerGroup
        {
            get { return this.Customer.CustGroup; }
            set
            {
                this.Customer.CustGroup = value;
                OnPropertyChanged("CustomerGroup");
                OnPropertyChanged("Validated");
            }
        }

        public string Currency
        {
            get { return this.Customer.Currency; }
            set
            {
                this.Customer.Currency = value;
                OnPropertyChanged("Currency");
                OnPropertyChanged("Validated");
            }
        }

        public CultureInfo Language
        {
            get
            {
                // when updating customer details, a Customer object coming in can have 
                // a null language. That leads to a null dereference exception in this 
                // property since a new CultureInfo object cannot be created. Hence the 
                // initialization to the terminal language.
                if (null == this.Customer.Language)
                {
                    this.Customer.Language = ApplicationSettings.Terminal.CultureName;
                }

                return new CultureInfo(this.Customer.Language);
            }
            set
            {
                this.Customer.Language = value.Name;
                OnPropertyChanged("Language");
                OnPropertyChanged("Validated");
            }
        }

        public string Phone
        {
            get { return this.Customer.Telephone; }
            set
            {
                this.Customer.Telephone = value;
                OnPropertyChanged("Phone");
            }
        }

        public string Email
        {
            get { return this.Customer.Email; }
            set
            {
                this.Customer.Email = value;
                OnPropertyChanged("Email");
            }
        }

        public string ReceiptEmail
        {
            get { return this.Customer.ReceiptEmailAddress; }
            set
            {
                this.Customer.ReceiptEmailAddress = value;
                OnPropertyChanged("ReceiptEmail");
            }
        }

        public string WebSite
        {
            get { return this.Customer.WwwAddress; }
            set
            {
                this.Customer.WwwAddress = value;
                OnPropertyChanged("WebSite");
            }
        }

        /// <summary>
        /// The tax identification (CNPJ or CPF) of the customer.
        /// </summary>
        public string CNPJCPFNumber
        {
            get { return this.Customer.CNPJCPFNumber; }
            set
            {
                this.Customer.CNPJCPFNumber = value;
                OnPropertyChanged("CNPJCPFNumber");
            }
        }

        public bool IsPerson
        {
            get { return this.Customer.RelationType == RelationType.Person; }
            set
            {
                this.Customer.RelationType = (value) ? RelationType.Person : RelationType.Organization;
                OnPropertyChanged("IsPerson");
                OnPropertyChanged("IsOrganization");
            }
        }

        public bool IsOrganization
        {
            get { return this.Customer.RelationType == RelationType.Organization; }
            set
            {
                this.Customer.RelationType = (value) ? RelationType.Organization : RelationType.Person;
                OnPropertyChanged("IsPerson");
                OnPropertyChanged("IsOrganization");
            }
        }

        public string Balance
        {
            get { return PosApplication.Instance.Services.Rounding.Round(this.Customer.Balance, true); }
        }

        public string InvoiceBalance
        {
            get {
                if (this.hasInvoiceAccount)
                {
                    return PosApplication.Instance.Services.Rounding.Round(this.InvoiceCustomer.Balance, true);
                }
                else
                {
                    return string.Empty;
                }
            }
        }

        public bool HasInvoiceAccount
        {
            get { return this.hasInvoiceAccount; }
        }

        public DateTime DateCreated
        {
            get { return this.history.DateCreated; }
            set
            {
                this.history.DateCreated = value;
                OnPropertyChanged("DateCreated");
            }
        }

        public string LastVisitedStore
        {
            get { return this.history.LastVisitedStore; }
            set
            {
                this.history.LastVisitedStore = value;
                OnPropertyChanged("LastVisitedStore");
            }
        }

        public DateTime LastVisitedDate
        {
            get { return this.history.LastVisitedDate; }
            set
            {
                this.history.LastVisitedDate = value;
                OnPropertyChanged("LastVisitedDate");
            }
        }

        public int TotalVisitsCount
        {
            get { return this.history.TotalVisitsCount; }
            set
            {
                this.history.TotalVisitsCount = value;
                OnPropertyChanged("TotalVisitsCount");
            }
        }

        public string TotalSalesAmount
        {
            get { return PosApplication.Instance.Services.Rounding.Round(this.history.TotalSalesAmount, true); }
        }

        public Collection<CustomerOrderHistoryItem> Items
        {
            get { return this.history.Items; }
        }

        public bool Validated
        {
            get
            {
                bool requiredFieldsEmpty =
                    string.IsNullOrWhiteSpace(this.Customer.CustGroup) ||
                    string.IsNullOrWhiteSpace(this.Customer.Currency) ||
                    string.IsNullOrWhiteSpace(this.Customer.Language) ||

                    // name empty when creating a new organization
                    (string.IsNullOrWhiteSpace(this.Customer.Name)
                    && (this.Customer.RelationType == RelationType.Organization)
                    && this.Customer.IsEmptyCustomer()) ||

                    // name empty when editing an organization or person
                    (string.IsNullOrWhiteSpace(this.Customer.Name)
                    && !this.Customer.IsEmptyCustomer()) ||

                    // both first and last name empty when creating a new person 
                    (string.IsNullOrWhiteSpace(this.Customer.FirstName)
                    && string.IsNullOrWhiteSpace(this.Customer.LastName)
                    && (this.Customer.RelationType == RelationType.Person)
                    && this.Customer.IsEmptyCustomer());

                return !requiredFieldsEmpty;
            }
        }

        public void ExecuteSelectCurrency()
        {
            DM.StoreDataManager dm = new DM.StoreDataManager(
                        CS.InternalApplication.Settings.Database.Connection,
                        CS.InternalApplication.Settings.Database.DataAreaID);

            var list = dm.GetCurrencies();
            if (list.Count > 0)
            {
                string code;
                var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(
                    (System.Collections.IList)list, "Code", CaptionCurrency, "Code", out code, this.Currency);
                if (dialogResult == DialogResult.OK && code != null)
                {
                    this.Currency = code;
                }
            }
        }

        public void ExecuteSelectGroup()
        {
            var groups = custDataManager.Value.GetCustomerGroups();
            if (groups.Count > 0)
            {
                //string groupId;
                //var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(
                //    (System.Collections.IList)groups, "Description", CaptionCustomerGroup, "Name", out groupId, this.CustomerGroup);
                //if (dialogResult == DialogResult.OK && groupId != null)
                //{
                //    this.CustomerGroup = groupId;
                //}

                //Start:Nim
                string groupId = "";
                groupId = GetDefaultCustGroup();
                this.CustomerGroup = groupId;
                //End:Nim
            }
        }

        public void ExecuteSelectLanguage()
        {
            if (languages == null)
                languages = custDataManager.Value.GetLanguages();

            if (languages.Count > 0)
            {
                string langCode;
                var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(
                    (System.Collections.IList)languages, "DisplayName", CaptionLanguage, "LanguageCode", out langCode, this.Language.Name);
                if (dialogResult == DialogResult.OK && langCode != null)
                {
                    this.Language = new System.Globalization.CultureInfo(langCode);
                }
            }
        }

        internal void ExecuteClear()
        {
            this.Currency = ApplicationSettings.Terminal.StoreCurrency;
            this.CustomerGroup = string.Empty;
            this.Email = string.Empty;
            this.FirstName = string.Empty;
            this.Language = new CultureInfo(ApplicationSettings.Terminal.CultureName);
            this.LastName = string.Empty;
            this.MiddleName = string.Empty;
            this.Name = string.Empty;
            this.Phone = string.Empty;
            this.ReceiptEmail = string.Empty;
            this.WebSite = string.Empty;
            this.CNPJCPFNumber = string.Empty;
        }

        internal void ExecuteLoadHistory()
        {
            if (!(this.Customer.IsEmptyCustomer() || this.historyLoadedFromAx))
            {
                try
                {
                    this.history = CS.GetCustomerHistory(this.Customer.CustomerId);

                }
                catch (Exception ex)
                {
                    ApplicationLog.Log(
                            typeof(Customer).ToString(),
                            string.Format("{0}\n{1}", ApplicationLocalizer.Language.Translate(99412), ex.Message), //"an error occured in the operation"
                            LogTraceLevel.Error);
                    CS.InternalApplication.Services.Dialog.ShowMessage(99412, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                this.historyLoadedFromAx = true;
                OnPropertyChanged("DateCreated");
                OnPropertyChanged("LastVisitedStore");
                OnPropertyChanged("LastVisitedDate");
                OnPropertyChanged("TotalVisitsCount");
                OnPropertyChanged("TotalSalesAmount");
            }
        }

        internal void ExecuteLoadBalances()
        {
            CustomerBalances customerBalances = new CustomerBalances();
            // if there is a customer and the balance was not already loaded from Ax.
            if (!this.Customer.IsEmptyCustomer() && !this.isBalanceLoadedFromAx)
            {
                string invoiceCustomerId = this.hasInvoiceAccount ? this.InvoiceCustomer.CustomerId : null;
                customerBalances = CS.GetCustomerBalances(this.Customer.CustomerId, invoiceCustomerId);

                this.Customer.Balance = customerBalances.Balance + customerBalances.LocalPendingBalance;
                this.Customer.CreditLimit = customerBalances.CreditLimit;

                if (this.hasInvoiceAccount)
                {
                    this.InvoiceCustomer.Balance = customerBalances.InvoiceAccountBalance + customerBalances.LocalInvoicePendingBalance;
                    this.InvoiceCustomer.CreditLimit = customerBalances.InvoiceAccountCreditLimit;
                }

                this.isBalanceLoadedFromAx = true;
                
                // we update the properties because they are updated indirectly by this call
                OnPropertyChanged("Balance");
                OnPropertyChanged("InvoiceBalance");
                OnPropertyChanged("HasInvoiceAccount");
            }
        }

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        [System.Diagnostics.Conditional("DEBUG")]
        [System.Diagnostics.DebuggerStepThrough]
        private void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,  
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;

                throw new Exception(msg);
            }
        }

        #endregion

        //Start:Nim
        internal void ExecuteNationality()
        {
            //System.Collections.IList nList = CustNationality.GetNatinalityList() as System.Collections.IList;
            //string nationalityId = string.Empty;
            //var dialogResult = CS.InternalApplication.Services.Dialog.GenericLookup(
            //    nList, "NationalityId", "Nationality", "NationalityId", out nationalityId, this.NationalityId);
            //if (dialogResult == DialogResult.OK && nationalityId != null)
            //{
            //    this.NationalityId = nationalityId;
            //}

            DataTable dt = CustNationality.GetData();
            DataRow drSal = null;
            var dialogResult = CS.InternalApplication.Services.Dialog.GenericSearch(dt, ref drSal, "Residence Country");
            if (dialogResult == DialogResult.OK && drSal != null)
            {
                this.NationalityId = Convert.ToString(drSal["Code"]);
            }
        }

        private string GetDefaultCustGroup() // For Malabar Gold Dubai
        {
            string sCustGrp = "";
            //get rate from db
            SqlConnection connection = new SqlConnection();

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;


            if (connection.State == ConnectionState.Closed)
                connection.Open();

            StringBuilder commandText = new StringBuilder();

            commandText.AppendLine("SELECT ISNULL(CUSTGROUP,'') AS CUSTGROUP FROM RETAILSTORETABLE WHERE STORENUMBER='" + ApplicationSettings.Database.StoreID + "' ");

            using (SqlCommand cmd = new SqlCommand(commandText.ToString(), connection))
            {
                using (SqlDataReader reader = cmd.ExecuteReader())
                {

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            sCustGrp = Convert.ToString(reader.GetValue(reader.GetOrdinal("CUSTGROUP")));
                        }
                    }
                    reader.Close();
                    reader.Dispose();
                }
            }

            if (connection.State == ConnectionState.Open)
                connection.Close();

            return sCustGrp;
        }
        //End:Nim

    }
}