/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;
using LSRetailPosis;
using LSRetailPosis.DataAccess;
using LSRetailPosis.DevUtilities;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;
using Microsoft.Dynamics.Retail.Pos.Customer.WinFormsTouch;
using Microsoft.Dynamics.Retail.Pos.SystemCore;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;
using DE = Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using DM = Microsoft.Dynamics.Retail.Pos.DataManager;
using System.Data.SqlClient;
using System.Text;

namespace Microsoft.Dynamics.Retail.Pos.Customer
{
    /// <summary>
    /// AX Sales Order Status
    /// </summary>
    internal enum SalesOrderStatus
    {
        None = 0,
        Backorder,
        Delivered,
        Invoiced,
        Canceled
    }

    [Export(typeof(ICustomer))]
    public class Customer : ICustomer
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        // TextID's for Customer project are reserved at 51000 - 51199
        // This class 51000 - 51019, now used 51007

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
                InternalApplication = value;
            }
        }

        internal static IApplication InternalApplication { get; private set; }

        private ICustomerSystem CustomerSystem
        {
            get { return this.Application.BusinessLogic.CustomerSystem; }
        }

        /// <summary>
        /// Enter the customer id and add the customer to the transaction
        /// </summary>
        /// <param name="retailTransaction">The retail tranaction</param>
        /// <returns>The retail tranaction</returns>
        public void EnterCustomerId(DE.IRetailTransaction retailTransaction)
        {
        }

        /// <summary>
        /// Search for the customer and add to the retailtransaction
        /// </summary>
        /// <param name="retailTransaction">The retail tranaction</param>
        public void Search(DE.IPosTransaction posTransaction)
        {
            // Show the search dialog
            DE.ICustomer customer = this.Search();

            if (posTransaction != null)
            {
                AddCustomerToTransaction(customer, posTransaction);

                #region For auto loyalty add for PNG
                if (customer != null)
                {
                    Loyalty.Loyalty objL = new Loyalty.Loyalty();
                    DE.ICardInfo cardInfo = null;
                    if (application != null)
                        objL.Application = application;
                    else
                        objL.Application = this.Application;
                    RetailTransaction retailTransaction = posTransaction as RetailTransaction;
                    objL.AddLoyaltyRequest(retailTransaction, cardInfo);
                }
                #endregion
            }
        }

        /// <summary>
        /// Add customer to transaction
        /// </summary>
        /// <param name="customer">Customer to add</param>
        /// <param name="posTransaction">Transaction</param>
        public void AddCustomerToTransaction(DE.ICustomer customer, DE.IPosTransaction posTransaction)
        {
            // !! Note - this code should follow the same steps to set the customer as PosProcesses\Customer.cs :: Execute()
            //Get information about the selected customer and add it to the transaction
            if ((customer != null) && (customer.CustomerId != null))
            {
                SalesOrderTransaction soTransaction = posTransaction as SalesOrderTransaction;


                //#region For auto loyalty add for PNG
                //if (customer != null)
                //{
                //    Loyalty.Loyalty objL = new Loyalty.Loyalty();
                //    DE.ICardInfo cardInfo = null;
                //    if (application != null)
                //        objL.Application = application;
                //    else
                //        objL.Application = this.Application;
                //    RetailTransaction retailTransaction = posTransaction as RetailTransaction;
                //    objL.AddLoyaltyRequest(retailTransaction, cardInfo);
                //}
                //#endregion

                //nimbus on 23/06/17
                string sStoreTaxState = "";
                string sStoreTaxCountry = "";

                if (Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState) != string.Empty)
                    sStoreTaxState = Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxState);

                if (Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxCountry) != string.Empty)
                    sStoreTaxCountry = Convert.ToString(ApplicationSettings.Terminal.IndiaStoreTaxCountry);

                string sCustTaxState = Convert.ToString(customer.State);
                string sCustTaxCountry = Convert.ToString(customer.Country);


                //string sTblName = "ISIGST" + ApplicationSettings.Terminal.TerminalId;

                //int iRes = isIGST(sTblName);
                string sCustGSTNo = getCustomerGSTNumber(customer.CustomerId);// according to mail of Shrikanta

                if (string.IsNullOrEmpty(sCustGSTNo))//iRes == 0
                {
                    if (sStoreTaxState != sCustTaxState) // for cal CGST and SGST only @PAN INDIA
                        customer.State = sStoreTaxState;

                    if (sStoreTaxCountry != sCustTaxCountry) // for cal CGST and SGST only @PAN INDIA
                        customer.Country = sStoreTaxCountry;
                }
                else //else part is added on 050918 
                {
                    if (string.IsNullOrEmpty(sCustTaxState))
                    {
                        using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage("State code is mandatory for this GST registered customer.", MessageBoxButtons.OK, MessageBoxIcon.Information))
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSForm(dialog);
                        }
                        return;
                    }
                }

                if (customer.CustomerTaxInformation == null)//for just created customer- GST purpose (resolved bug of MS)
                {
                    customer.CustomerTaxInformation = new LSRetailPosis.Transaction.CustomerTaxInformation();
                }
                //nim end

                if (soTransaction != null)
                {
                    // Must check for ISalesOrderTransaction before IRetailTransaction because it derives from IRetailTransaction
                    soTransaction.Customer = customer as LSRetailPosis.Transaction.Customer;
                }
                else
                {
                    RetailTransaction asRetailTransaction = posTransaction as RetailTransaction;
                    if (asRetailTransaction != null)
                    {
                        if (!asRetailTransaction.IsCustomerAllowed())
                        {
                            LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSMessageDialog(4544);
                            return;
                        }

                        DE.ICustomer invoicedCustomer = customer;
                        string invoiceAccount = customer.InvoiceAccount;

                        //If the customer has another account as invoice account
                        if (!string.IsNullOrWhiteSpace(invoiceAccount))
                        {
                            invoicedCustomer = this.CustomerSystem.GetCustomerInfo(invoiceAccount);
                        }

                        // Trigger: PreCustomerSet trigger for the operation
                        var preTriggerResult = new PreTriggerResult();

                        PosApplication.Instance.Triggers.Invoke<ICustomerTrigger>(t => t.PreCustomerSet(preTriggerResult, posTransaction, customer.CustomerId));

                        if (!TriggerHelpers.ProcessPreTriggerResults(preTriggerResult))
                        {
                            return;
                        }

                        this.CustomerSystem.SetCustomer(asRetailTransaction, customer, invoicedCustomer);

                        //If CheckCustomer returns false then the customer isn't allowed to be added to the transaction. Msg has already been displayed
                        if (!CheckCustomer(posTransaction))
                        {
                            return;
                        }

                        //If CheckInvoicedCustomer removed the customer then it isn't allowed to be added to the transaction. Msg has already been displayed
                        if (!CheckInvoicedCustomer(posTransaction))
                        {
                            return;
                        }

                        if (asRetailTransaction.Customer.UsePurchRequest)
                        {
                            asRetailTransaction.CustomerPurchRequestId = GetPurchRequestId();
                        }
                    }
                    else if (posTransaction is CustomerPaymentTransaction)
                    {
                        // Customer is not allowed to be changed  (or cleared) once a customer account deposit has been made.
                        LSRetailPosis.POSProcesses.POSFormsManager.ShowPOSMessageDialog(3084);
                    }
                }
            }
            else
            {
                NetTracer.Warning("Customer::AddCustomerToTransaction: customer parameter is null");
            }
        }

        /// <summary>
        /// Search for the address and add to the retailtransaction
        /// </summary>
        /// <param name="retailTransaction">The retail tranaction</param>
        /// <returns>The retail tranaction</returns>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Grandfathered")]
        public void SearchShippingAddress(DE.IPosTransaction posTransaction)
        {
            RetailTransaction retailTransaction = (RetailTransaction)posTransaction;

            string shippingname = string.Empty;
            string shippingaddress = string.Empty;

            DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);
            DE.IAddress address = null;
            if (customerDataManager.HasAddress(retailTransaction.Customer.PartyId))
            {
                address = SearchShippingAddress(retailTransaction.Customer);
            }
            else
            {
                // Create and add customer in AX
                address = AddNewShippingAddress(retailTransaction.Customer);
            }

            if (address != null)
            {
                Customer.InternalApplication.BusinessLogic.CustomerSystem.SetShippingAddress(retailTransaction, address);
            }
        }

        public DE.IAddress SearchShippingAddress(DE.ICustomer customer)
        {
            DE.IAddress address = null;

            using (frmShippingAddressSearch searchShippingAddressDialog = new frmShippingAddressSearch(customer))
            {
                this.Application.ApplicationFramework.POSShowForm(searchShippingAddressDialog);

                if (searchShippingAddressDialog.DialogResult == DialogResult.OK)
                {
                    address = searchShippingAddressDialog.SelectedAddress;
                }
            }

            return address;
        }

        public static DE.IAddress AddNewShippingAddress(DE.ICustomer customer)
        {
            DE.IAddress address = null;

            address = GetNewAddressDefaults(customer);
            using (frmNewShippingAddress dlg = new frmNewShippingAddress(customer, address))
            {
                InternalApplication.ApplicationFramework.POSShowForm(dlg);

                if (dlg.DialogResult == DialogResult.OK)
                {
                    address = dlg.Address;
                }
            }

            return address;
        }

        /// <summary>
        /// Invoke the 'Add Shipping Address' dialog to edit an existing address
        /// </summary>
        /// <param name="addressRecId">address rec id</param>
        /// <param name="existingCustomer">customer that this address is to be associated with</param>
        internal static DE.IAddress EditShippingAddress(long addressRecId, DE.ICustomer existingCustomer)
        {
            DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                ApplicationSettings.Database.LocalConnection,
                ApplicationSettings.Database.DATAAREAID);

            DE.IAddress address = customerDataManager.GetAddress(addressRecId);

            using (frmNewShippingAddress dlg = new frmNewShippingAddress(existingCustomer, address))
            {
                InternalApplication.ApplicationFramework.POSShowForm(dlg);

                if (dlg.DialogResult == DialogResult.OK)
                {
                    address = dlg.Address;
                }
            }
            return address;
        }

        /// <summary>
        /// Prettier concrete wrapper around call to customer system to get blank customer
        /// </summary>
        /// <returns>Newly initialized blank customer</returns>
        private static LSRetailPosis.Transaction.Customer GetBlankCustomer()
        {
            return (LSRetailPosis.Transaction.Customer)InternalApplication.BusinessLogic.CustomerSystem.GetEmptyCustomer();
        }

        private static bool CheckCustomer(DE.IPosTransaction posTransaction)
        {
            RetailTransaction retailTransaction = (RetailTransaction)posTransaction;

            if (retailTransaction.Customer.Blocked == DE.BlockedEnum.All)
            {
                //Display a message
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(51002)) //This customer has been blocked. No sales or transactions are allowed.
                {
                    Customer.InternalApplication.ApplicationFramework.POSShowForm(dialog);
                }

                //Cancel the customer account
                retailTransaction.Customer = GetBlankCustomer();

                return false;
            }

            if (retailTransaction.Customer.Blocked == DE.BlockedEnum.Invoice)
            {
                //Display message
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(51003)) //This customer has been blocked. This account can not be charged to.
                {
                    Customer.InternalApplication.ApplicationFramework.POSShowForm(dialog);
                }
            }

            return true;
        }

        private static bool CheckInvoicedCustomer(DE.IPosTransaction posTransaction)
        {
            RetailTransaction retailTransaction = (RetailTransaction)posTransaction;

            //If the Invoiced customer has All as blocked then the selected customer can not be added to the transaction
            if (retailTransaction.InvoicedCustomer.Blocked == DE.BlockedEnum.All)
            {
                //If Invoiced Customer is blocked then the original customer should be blocked too.
                retailTransaction.Customer.Blocked = DE.BlockedEnum.All;

                //Display the message
                using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(51004)) //The invoiced customer has been blocked. Charging to this account will not be allowed.
                {
                    Customer.InternalApplication.ApplicationFramework.POSShowForm(dialog);
                }

                //Cancel all customer accounts
                retailTransaction.Customer = GetBlankCustomer();
                retailTransaction.InvoicedCustomer = GetBlankCustomer();

                return false;
            }

            if (retailTransaction.InvoicedCustomer.Blocked == DE.BlockedEnum.Invoice)
            {
                //If a similar message has already been displayed for the original customer then don't display it again.
                if (retailTransaction.Customer.Blocked != DE.BlockedEnum.Invoice)
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(51005)) //This customer has been blocked. This account can not be charged to.
                    {
                        Customer.InternalApplication.ApplicationFramework.POSShowForm(dialog);
                    }
                }

                //If Invoiced Customer is blocked then the original customer should be blocked too.
                retailTransaction.Customer.Blocked = DE.BlockedEnum.Invoice;
            }

            return true;
        }

        /// <summary>
        /// Displays the customer search UI
        /// </summary>        
        /// <returns>Customer if found, null otherwise</returns>
        public DE.ICustomer Search()
        {
            string selectedCustomerId = string.Empty;

            // Show the search dialog
            using (frmCustomerSearch searchDialog = new frmCustomerSearch())
            {
                this.Application.ApplicationFramework.POSShowForm(searchDialog);

                // Quit if cancel is pressed...
                if (searchDialog.DialogResult != System.Windows.Forms.DialogResult.OK)
                {
                    return null;
                }
                selectedCustomerId = searchDialog.SelectedCustomerId;
            }

            //Get information about the selected customer and return it
            if (selectedCustomerId.Length != 0)
            {
                //Get the customer info...
                return this.CustomerSystem.GetCustomerInfo(selectedCustomerId);
            }
            else
            {
                //No customer was selected
                return null;
            }
        }

        /// <summary>
        /// Does a keyword search of the words provided and returns a list 
        /// of customer search result objects matching those words
        /// </summary>
        /// <param name="keywords">The keywords to be searched for</param>
        /// <param name="pageNumber">The page of results to seek to and return</param>
        /// <param name="pageSize">The number of results per page</param>
        /// <remarks>This method should only be called if RetailFunctionalityProfile's field 'CustomerSearchMode' is
        /// 1 or 2 (Remote or Both). That indicates that AX has the needed APIs for this call,
        /// and that it is acceptable to make this call.</remarks>
        internal static IList<DM.CustomerSearchResult> RemoteCustomerSearch(string keywords, int pageNumber, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(keywords))
            {
                throw new ArgumentException("keywords", "Keywords cannot be null or whitespace");
            }
            else if (pageNumber < 0)
            {
                throw new ArgumentException("pageNumber", "pageNumber cannot be less than 0");
            }
            else if (pageSize <= 0)
            {
                throw new ArgumentException("pageSize", "pageSize cannot be less than or equal to 0");
            }

            try
            {
                // Throw an exception containing an error message if we can't connect
                InternalApplication.TransactionServices.CheckConnection();
                // Make a call to real-time service to perform a customer keyword search via AX
                var searchResults = InternalApplication.TransactionServices.Invoke(
                    methodName: "searchCustomers",
                    parameters: new object[]
                    {
                        keywords,
                        pageNumber,
                        pageSize
                    }
                );

                if (searchResults == null || searchResults.Count < 4)
                {
                    throw new Exception("Invalid results from server-side method 'searchCustomers'");
                }

                // If the "Success" field is false, raise an exception
                if (!Convert.ToBoolean(searchResults[1]))
                {
                    throw new Exception("Call to server-side method 'searchCustomers' returned a status of failure");
                }

                XDocument xmlDoc = XDocument.Parse(searchResults[3].ToString());
                return ParseCustomerSearchXML(xmlDoc);
            }
            catch (Exception ex)
            {
                // Log the error and surface it to the application
                LSRetailPosis.ApplicationExceptionHandler.HandleException("LSRetailPosis.TransactionServices.RemoteCustomerSearch", ex);
                throw;
            }
        }

        /// <summary>
        /// A helper function that parses xml representing a list of customer search result objects
        /// </summary>
        /// <param name="xmlDoc">XML representing a list of customer search result objects</param>
        private static IList<DM.CustomerSearchResult> ParseCustomerSearchXML(XDocument xmlDoc)
        {
            List<XElement> retrievedCustomers = xmlDoc.Root.Elements("SearchResult").ToList();
            List<DM.CustomerSearchResult> parsedCustomers = new List<DM.CustomerSearchResult>(retrievedCustomers.Count);

            foreach (XElement customerXML in retrievedCustomers)
            {
                DM.CustomerSearchResult cust = new DM.CustomerSearchResult();
                cust.AccountNumber = customerXML.Element("AccountNumber").Value;
                cust.Email = customerXML.Element("Email").Value;
                cust.PrimaryStreetAddress = customerXML.Element("FullAddress").Value;
                cust.Name = customerXML.Element("FullName").Value;
                cust.PartyNumber = customerXML.Element("PartyNumber").Value;
                cust.Phone = customerXML.Element("Phone").Value;
                cust.IsRemote = true;

                parsedCustomers.Add(cust);
            }

            return parsedCustomers;
        }

        /// <summary>
        /// Retrieves the specified customer from AX and returns XML
        /// corresponding to the tables that make up that customer
        /// </summary>
        /// <param name="accountNum">The account number to be searched for</param>
        /// <remarks>This method should only be called if RetailFunctionalityProfile's field 'CustomerSearchMode' is
        /// 1 or 2 (Remote or Both). That indicates that AX has the needed APIs for this call,
        /// and that it is acceptable to make this call.</remarks>
        public static XDocument GetCustomerData(string accountNum)
        {
            if (string.IsNullOrWhiteSpace(accountNum))
            {
                throw new ArgumentException("accountNum", "The customer's account number cannot be null or whitespace.");
            }

            try
            {
                // Throw an exception containing an error message if we can't connect
                InternalApplication.TransactionServices.CheckConnection();
                var searchResults = InternalApplication.TransactionServices.Invoke(
                    methodName: "getGenericCustomerData",
                    parameters: new object[]
                    {
                        accountNum
        }
                );

                if (searchResults == null || searchResults.Count < 4)
                {
                    throw new Exception("Invalid results from server-side method 'getGenericCustomerData'");
                }

                // If the "Success" field is false, raise an exception
                if (!Convert.ToBoolean(searchResults[1]))
                {
                    throw new Exception("Call to server-side method 'getGenericCustomerData' returned a status of failure");
                }

                return XDocument.Parse(searchResults[3].ToString());
            }
            catch (Exception ex)
            {
                // Log the error and surface it to the application
                LSRetailPosis.ApplicationExceptionHandler.HandleException("LSRetailPosis.TransactionServices.GetCustomerData", ex);
                throw;
            }
        }

        /// <summary>
        /// Retrieves the specified party from AX and returns XML
        /// corresponding to the tables that make up that party
        /// (This data is a subset of customer XML)
        /// </summary>
        /// <param name="partyNum">The party number to be searched for</param>
        /// <remarks>This method should only be called if RetailFunctionalityProfile's field 'CustomerSearchMode' is
        /// 1 or 2 (Remote or Both). That indicates that AX has the needed APIs for this call,
        /// and that it is acceptable to make this call.</remarks>
        internal static XDocument GetPartyData(string partyNum)
        {
            if (string.IsNullOrWhiteSpace(partyNum))
            {
                throw new ArgumentException("partyNum", "The customer's party number cannot be null or whitespace.");
            }

            try
            {
                // Throw an exception containing an error message if we can't connect
                InternalApplication.TransactionServices.CheckConnection();
                var searchResults = InternalApplication.TransactionServices.Invoke(
                    methodName: "getGenericCustomerPartyData",
                    parameters: new object[]
                    {
                        partyNum
                    }
                );

                if (searchResults == null || searchResults.Count < 4)
                {
                    throw new Exception("Invalid results from server-side method 'getGenericCustomerPartyData'");
                }

                // If the "Success" field is false, raise an exception
                if (!Convert.ToBoolean(searchResults[1]))
                {
                    throw new Exception("Call to server-side method 'getGenericCustomerPartyData' returned a status of failure");
                }

                return XDocument.Parse(searchResults[3].ToString());
            }
            catch (Exception ex)
            {
                // Log the error and surface it to the application
                LSRetailPosis.ApplicationExceptionHandler.HandleException("LSRetailPosis.TransactionServices.GetPartyData", ex);
                throw;
            }
        }

        /// <summary>
        /// Sets the customer balance of the customer
        /// </summary>
        /// <param name="retailTransaction">The retail tranaction</param>
        public void Balance(DE.IRetailTransaction retailTransaction)
        {
        }

        /// <summary>
        /// Sets the customer status of the customer
        /// </summary>
        /// <param name="retailTransaction">The retail tranaction</param>
        public void Status(DE.IRetailTransaction retailTransaction)
        {
        }

        /// <summary>
        /// Register information about a new customer into the database
        /// </summary>
        /// <returns>Returns new customer of successful, null otherwise</returns>
        public DE.ICustomer AddNew()
        {
            DE.ICustomer customer = null;
            DE.IAddress address = null;

            CustomerData custData = new CustomerData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);

            customer = GetNewCustomerDefaults();
            address = GetNewAddressDefaults(customer);
            using (frmNewCustomer newCustDialog = new frmNewCustomer(customer, address))
            {
                this.Application.ApplicationFramework.POSShowForm(newCustDialog);

                if (newCustDialog.DialogResult == DialogResult.OK)
                {
                    customer = newCustDialog.Customer;
                    address = newCustDialog.Address;
                }
            }

            return customer;
        }

        //Start:Nim
        public DE.ICustomer AddNew(out string CustId, out string CustName, out string CustCurrency)
        {
            DE.ICustomer customer = null;
            DE.IAddress address = null;

            if (Application != null)
            {
                CustomerData custData = new CustomerData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);
            }
            else
            {
                CustomerData custData = new CustomerData(LSRetailPosis.Settings.ApplicationSettings.Database.LocalConnection, LSRetailPosis.Settings.ApplicationSettings.Database.DATAAREAID);
            }

            //CustomerData custData = new CustomerData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);

            customer = GetNewCustomerDefaults();
            address = GetNewAddressDefaults(customer);
            using (frmNewCustomer newCustDialog = new frmNewCustomer(customer, address))
            {

                if (Application != null)
                {
                    this.Application.ApplicationFramework.POSShowForm(newCustDialog);
                    // customer = newCustDialog.Customer;
                    //address = newCustDialog.Address;
                }
                else
                {
                    Customer.InternalApplication.ApplicationFramework.POSShowForm(newCustDialog);

                    customer = newCustDialog.Customer;
                    CustId = customer.CustomerId;
                    CustName = customer.Name;
                    CustCurrency = customer.Currency;
                    address = newCustDialog.Address;
                }

                // this.Application.ApplicationFramework.POSShowForm(newCustDialog);

                if (newCustDialog.DialogResult == DialogResult.OK)
                {
                    customer = newCustDialog.Customer;
                    CustId = customer.CustomerId;
                    CustName = customer.Name;
                    CustCurrency = customer.Currency;
                    address = newCustDialog.Address;
                }
            }
            CustName = customer.Name;
            CustId = customer.CustomerId;
            CustCurrency = customer.Currency;
            return customer;
        }
        //End:NIm

        private static DE.IAddress GetNewAddressDefaults(DE.ICustomer customer)
        {
            DE.IAddress address = Customer.InternalApplication.BusinessLogic.Utility.CreateAddress();

            address.AddressType = DE.AddressType.Home;
            address.BuildingComplement = (customer.AddressComplement) ?? string.Empty;
            address.City = (customer.City) ?? string.Empty;
            address.CNPJCPFNumber = (customer.CNPJCPFNumber) ?? string.Empty;
            address.Country = (customer.Country) ?? string.Empty;
            address.County = customer.County ?? string.Empty;
            address.DistrictName = (customer.DistrictName) ?? string.Empty;
            address.Email = (customer.Email) ?? string.Empty;
            address.OrgId = (customer.OrgId) ?? string.Empty;
            address.PostalCode = (customer.PostalCode) ?? string.Empty;
            address.State = (customer.State) ?? string.Empty;
            address.Telephone = (customer.Telephone) ?? string.Empty;
            address.SalesTaxGroup = (customer.SalesTaxGroup) ?? string.Empty;
            address.StreetName = (customer.StreetName) ?? string.Empty;
            address.StreetName = DM.DataManager.ReplaceNewLineChar(address.StreetName, Environment.NewLine);
            address.StreetNumber = (customer.AddressNumber) ?? string.Empty;
            address.URL = (customer.WwwAddress) ?? string.Empty;

            address.CountryISOCode = customer.PrimaryAddress.CountryISOCode ?? string.Empty;
            address.StateDescription = customer.PrimaryAddress.StateDescription ?? string.Empty;
            address.CountyDescription = customer.PrimaryAddress.CountyDescription ?? string.Empty;
            address.CityRecId = customer.PrimaryAddress.CityRecId;
            address.CityDescription = customer.PrimaryAddress.CityDescription ?? string.Empty;
            address.DistrictRecId = customer.PrimaryAddress.DistrictRecId;
            address.DistrictDescription = customer.PrimaryAddress.DistrictDescription ?? string.Empty;

            return address;
        }

        private static DE.ICustomer GetNewCustomerDefaults()
        {
            DE.ICustomer customer = Customer.InternalApplication.BusinessLogic.Utility.CreateCustomer();

            SettingsData settingsData = new SettingsData(ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);
            using (DataTable storeData = settingsData.GetStoreInformation(ApplicationSettings.Terminal.StoreId))
            {
                if (storeData.Rows.Count > 0)
                {
                    customer.Country = storeData.Rows[0]["COUNTRYREGIONID"].ToString();
                }
            }
            customer.Currency = ApplicationSettings.Terminal.StoreCurrency;
            customer.Language = ApplicationSettings.Terminal.CultureName;
            customer.RelationType = DE.RelationType.Person;

            return customer;
        }

        /// <summary>
        /// Updates the customer information in the database
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns>Returns true if operations is successful</returns>
        public bool Update(string customerId)
        {
            return false;
        }

        /// <summary>
        /// Updates the customer information in the database
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns>Returns the updated customer if succeded, null otherwise</returns>
        public DE.ICustomer UpdateCustomer(string customerId)
        {
            DE.ICustomer invoicedCustomer = null;

            DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                    ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);

            DE.ICustomer customer = customerDataManager.GetTransactionalCustomer(customerId);
            DE.IAddress address = customerDataManager.GetAddress(customer.PrimaryAddress.Id);

            // If the customer has another account as invoice account
            if (!string.IsNullOrWhiteSpace(customer.InvoiceAccount))
            {
                invoicedCustomer = customerDataManager.GetTransactionalCustomer(customer.InvoiceAccount);
            }


            using (frmNewCustomer newCustDialog = new frmNewCustomer(customer, invoicedCustomer, address))
            {
                this.Application.ApplicationFramework.POSShowForm(newCustDialog);

                if (newCustDialog.DialogResult == DialogResult.OK)
                {
                    return customer;
                }
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Param 'customer' is already validated. Oddly, this CA message only appears when parameterList has more than 18 fields."),
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "4", Justification = "Param 'entityKeys' is already validated. Oddly, this CA message only appears when parameterList has more than 18 fields.")]
        internal static void UpdateCustomer(ref bool retValue, ref string comment, ref DE.ICustomer customer, ref IList<Int64> entityKeys)
        {
            if (customer == null)
            {
                throw new ArgumentNullException("customer");
            }

            string partyName = null;
            switch (customer.RelationType)
            {
                case DE.RelationType.Person:
                    // send party name in fixed format
                    partyName = string.Format("{0} {1} {2}",
                        (customer.FirstName) ?? string.Empty,
                        (customer.MiddleName) ?? string.Empty,
                        (customer.LastName) ?? string.Empty);

                    if (string.IsNullOrWhiteSpace(partyName))
                    {
                        partyName = customer.Name;
                    }
                    break;
                default:
                    partyName = customer.Name;
                    break;
            }

            // Some of the parameters has been disabled, as they don't currently have corresponding values in the SDK, only the party name is being set.
            object[] parameterList = new object[] 
            {
                customer.RecordId,
                partyName,
                customer.CustGroup,
                customer.Currency,
                customer.Language,
                customer.Telephone,
                customer.TelephoneId,
                customer.MobilePhone,
                customer.Email,
                customer.EmailId,
                customer.WwwAddress,
                customer.UrlId,
                customer.MultiLineDiscountGroup,
                customer.TotalDiscountGroup,
                customer.LineDiscountGroup,
                customer.PriceGroup,
                customer.SalesTaxGroup,
                customer.CreditLimit,
                (int)customer.Blocked,
                (customer.OrgId) ?? string.Empty,
                customer.UsePurchRequest,
                customer.VatNum,
                customer.InvoiceAccount == null ? string.Empty : customer.InvoiceAccount,
                customer.MandatoryCreditLimit,
                customer.ContactPerson,
                customer.UseOrderNumberReference,
                (int)customer.ReceiptSettings,
                customer.ReceiptEmailAddress,
                (customer.IdentificationNumber) ?? string.Empty
            };

            ReadOnlyCollection<object> containerArray = InternalApplication.TransactionServices.Invoke("UpdateCustomer", parameterList);

            try
            {
                retValue = (bool)containerArray[1];
                comment = (string)containerArray[2];

                if (retValue && containerArray.Count > 3)   //container array puts data starting at index 3 - check for data being present
                {
                    customer.CustomerId = Utility.ToStr(containerArray[3]);
                    customer.SalesTaxGroup = Utility.ToStr(containerArray[4]);
                    customer.PartyId = Utility.ToInt64(containerArray[5]);
                    customer.RecordId = Utility.ToStr(containerArray[6]);

                    if (entityKeys == null)
                    {
                        entityKeys = new List<Int64>(containerArray.Count - 5);
                    }

                    entityKeys.Clear();

                    // save into entity keys collection, the data needed for saving the customer starting @ PartyId
                    for (int i = 5; i < containerArray.Count; i++)
                    {
                        entityKeys.Add(Utility.ToInt64(containerArray[i]));
                    }
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException("LSRetailPosis.TransactionServices.UpdateCustomer", ex);
            }
        }

        /// <summary>
        /// Delete the customer from the database if the customer holds no customer transactions.
        /// </summary>
        /// <param name="customerId">The customer id</param>
        /// <returns>Returns true if operations is successful</returns>
        public bool Delete(string customerId)
        {
            return false;
        }

        /// <summary>
        /// Show customertransactions for customer
        /// </summary>
        /// <param name="customerId">The id of the customer</param>
        public void Transactions(string customerId)
        {
            string[] custTransactionsLanguageTexts = new string[3];
            custTransactionsLanguageTexts[0] = ApplicationLocalizer.Language.Translate(51151);
            custTransactionsLanguageTexts[1] = ApplicationLocalizer.Language.Translate(51152);
            custTransactionsLanguageTexts[2] = ApplicationLocalizer.Language.Translate(51153);

            CustomerData customerData = new CustomerData(Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID);
            using (DataTable dt = customerData.GetCustomerTransactions(string.Empty, customerId, 1, custTransactionsLanguageTexts))
            {
                if (dt.Rows.Count > 0)
                {
                    using (frmCustomerTransactions customerTransactions = new frmCustomerTransactions(customerId, Application))
                    {
                        this.Application.ApplicationFramework.POSShowForm(customerTransactions);
                    }
                }
                else
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(51000))//No transactions are found for the customer.
                    {
                        this.Application.ApplicationFramework.POSShowForm(dialog);
                    }
                }
            }
        }

        /// <summary>
        /// Print customertransactions for customer
        /// </summary>
        /// <param name="customerId">The id of the customer</param>
        public void TransactionsReport(string customerId)
        {
            using (frmPrintSelection printSelection = new frmPrintSelection(customerId))
            {
                this.Application.ApplicationFramework.POSShowForm(printSelection);
                if (printSelection.DialogResult != DialogResult.OK)
                {
                    return;
                }
                else
                {
                    ///Do the report.
                }
            }
        }

        /// <summary>
        /// Prints a balance report for all customer with balance not equal to zero.
        /// </summary>
        public void BalanceReport()
        {
            BalanceReport balanceReport = new BalanceReport();
            balanceReport.Print();
        }

        /// <summary>
        /// validate customer details from database with the transaction made by the customer.
        /// </summary>
        /// <param name="valid"></param>
        /// <param name="comment"></param>
        /// <param name="manualAuthenticationCode"></param>
        /// <param name="customerId"></param>
        /// <param name="amount"></param>
        /// <param name="retailTransaction"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1")]
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "5", Justification = "Grandfather")]
        public void AuthorizeCustomerAccountPayment(ref bool valid, ref string comment, ref string manualAuthenticationCode, string customerId, decimal amount, DE.IRetailTransaction retailTransaction)
        {
            try
            {
                LSRetailPosis.ApplicationLog.Log(this.ToString(), "Customer.AuthorizeCustomerAccountPayment()", LSRetailPosis.LogTraceLevel.Trace);
                //Get the customer information for the customer
                DM.CustomerDataManager customerDataManager = new DM.CustomerDataManager(
                    ApplicationSettings.Database.LocalConnection, ApplicationSettings.Database.DATAAREAID);


                #region CRW
                if (string.IsNullOrEmpty(customerId) && retailTransaction !=null)
                {
                    customerId = ((LSRetailPosis.Transaction.RetailTransaction)(retailTransaction)).Customer.CustomerId;
                }
                #endregion

                DE.ICustomer tempCust = customerDataManager.GetTransactionalCustomer(customerId);

                if (!string.IsNullOrEmpty(tempCust.InvoiceAccount))
                {
                    DE.ICustomer tempInvCust = customerDataManager.GetTransactionalCustomer(tempCust.InvoiceAccount);
                    if (tempInvCust.Blocked == DE.BlockedEnum.All)
                    {
                        tempCust.Blocked = tempInvCust.Blocked;
                    }
                    else if (tempInvCust.Blocked == DE.BlockedEnum.Invoice && tempCust.Blocked != DE.BlockedEnum.All)
                    {
                        tempCust.Blocked = DE.BlockedEnum.Invoice;
                    }
                }

                // if we do a return we shouldn't check for credit limit.
                if (amount <= 0)
                {
                    valid = true;
                    return;
                }

                // we need the TS call regardless because we need the replication counter to identify the pending transactions.
                CustomerBalances customerBalances = Customer.GetCustomerBalances(tempCust.CustomerId, tempCust.InvoiceAccount);

                //check against the balance data (posted and local pending transactions)
                valid = IsBalanceOverTheLimit(tempCust, customerBalances, ref comment);

                // Use the InvoiceAccount if it is present.
                if (!string.IsNullOrEmpty(tempCust.InvoiceAccount))
                {
                    // we add the local balances to the amount of the order. 
                    decimal amountToCheck = amount + customerBalances.LocalInvoicePendingBalance;
                    this.Application.TransactionServices.ValidateCustomerStatus(ref valid, ref comment, tempCust.InvoiceAccount, amountToCheck, retailTransaction.StoreCurrencyCode);
                }
                else
                {
                    decimal amountToCheck = amount + customerBalances.LocalPendingBalance;
                    this.Application.TransactionServices.ValidateCustomerStatus(ref valid, ref comment, tempCust.CustomerId, amountToCheck, retailTransaction.StoreCurrencyCode);
                }

                if (!valid)
                {
                    comment = LSRetailPosis.ApplicationLocalizer.Language.Translate(51007) + " Customer Credit Limit Balance :"
                        + Convert.ToString(decimal.Round(customerBalances.CreditLimit - customerBalances.Balance - customerBalances.LocalPendingBalance, 2, MidpointRounding.AwayFromZero));// The amount charged is higher than existing creditlimit
                    return;
                }
            }
            catch (LSRetailPosis.PosisException px)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), px);
                throw;
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        private static bool IsBalanceOverTheLimit(DE.ICustomer tempCust, CustomerBalances customerBalances, ref string comment)
        {
            bool valid = true;

            if (!string.IsNullOrEmpty(tempCust.InvoiceAccount))
            {
                // we do the check against the invoice account  
                if ((customerBalances.InvoiceAccountCreditLimit > 0) &&
                    ((customerBalances.InvoiceAccountBalance + customerBalances.LocalInvoicePendingBalance) > customerBalances.InvoiceAccountCreditLimit))
                {
                    valid = false;
                    comment = LSRetailPosis.ApplicationLocalizer.Language.Translate(51007);  // The amount charged is higher than existing creditlimit
                }
            }
            else
            {
                // we do the check against the customer account
                if ((customerBalances.CreditLimit > 0) &&
                    ((customerBalances.Balance + customerBalances.LocalPendingBalance) > customerBalances.CreditLimit))
                {
                    valid = false;
                    comment = LSRetailPosis.ApplicationLocalizer.Language.Translate(51007);  // The amount charged is higher than existing creditlimit
                }
            }

            return valid;
        }

        private static string GetPurchRequestId()
        {
            string retVal = string.Empty;

            try
            {

                InputConfirmation inputConfirmation = new InputConfirmation()
                {
                    PromptText = LSRetailPosis.ApplicationLocalizer.Language.Translate(51001), // Enter purchase request id
                };

                InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(inputConfirmation, () =>
                {
                    retVal = inputConfirmation.EnteredText;
                    if (retVal.Length > 20)
                    {
                        retVal = retVal.Substring(0, 20);
                    }
                }
                );

                InternalApplication.Services.Interaction.InteractionRequest(request);

            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "Customer::GetPurchRequestId failed");
            }

            return retVal;
        }

        /// <summary>
        /// Displays UI to enter a customer ID and returns the customer based on the customer ID entered.
        /// </summary>
        /// <returns>Customer if found, otherwise null</returns>
        public DE.ICustomer GetCustomer()
        {
            DE.ICustomer customer = null;

            InputConfirmation inputConfirmation = new InputConfirmation()
            {
                MaxLength = 30,
                PromptText = LSRetailPosis.ApplicationLocalizer.Language.Translate(3060), // "Enter a customer id.";
            };

            InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(inputConfirmation, () =>
            {
                if (inputConfirmation.Confirmed)
                {
                    customer = this.CustomerSystem.GetCustomerInfo(inputConfirmation.EnteredText);
                }
            }
            );

            InternalApplication.Services.Interaction.InteractionRequest(request);

            return customer;
        }

        public void AddShippingAddress(DE.IPosTransaction posTransaction)
        {
            RetailTransaction retailTransaction = (RetailTransaction)posTransaction;
            DE.IAddress address = AddNewShippingAddress(retailTransaction.Customer);
            if (address != null)
            {
                Customer.InternalApplication.BusinessLogic.CustomerSystem.SetShippingAddress(retailTransaction, address);
            }
        }

        private string getCustomerGSTNumber(string sCustId)
        {
            string sResult = "";

            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;

            StringBuilder commandText = new StringBuilder();
            commandText.Append(" select isnull(tri.REGISTRATIONNUMBER,'') from ");
            commandText.Append(" custtable c1 left join DIRPARTYTABLE dpt on dpt.recid=c1.PARTY");
            commandText.Append(" left join dirpartylocation dpl on dpl.PARTY=c1.PARTY");
            commandText.Append(" left join taxinformation_in tii on tii.REGISTRATIONLOCATION=dpl.LOCATION");
            commandText.Append(" left join TaxRegistrationNumbers_IN tri on tri.RECID=tii.GSTIN where");
            commandText.Append(" dpl.ISPRIMARY=1 and tii.ISPRIMARY=1 and c1.accountnum = '" + sCustId + "'");

            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return sResult;

        }

        /// <summary>
        /// Get sales order history for the given customer
        /// </summary>
        /// <param name="customerId">id of the customer (must be non-null/empty)</param>
        /// <returns>CustomerHistory object if successfull, NULL if not.</returns>
        internal static CustomerHistory GetCustomerHistory(string customerId)
        {
            if (string.IsNullOrWhiteSpace(customerId)) throw new ArgumentNullException("customerId");

            const int successIndex = 1; // index of the success/fail result
            const int commentIndex = 2; // index of the error message or comment
            const int payloadIndex = 3; // index of the content/payload.

            try
            {
                CustomerHistory history = null;
                ReadOnlyCollection<object> containerArray;
                bool retValue;
                string comment;

                // Begin by checking if there is a connection to the Transaction Service
                if (Customer.InternalApplication.TransactionServices.CheckConnection())
                {
                    // Send request to AX
                    containerArray = InternalApplication.TransactionServices.Invoke("getCustomerHistory",
                        customerId,
                        (int)Functions.DaysCustomerHistory);

                    retValue = (bool)containerArray[successIndex];
                    comment = containerArray[commentIndex].ToString();

                    if (retValue)
                    {
                        // Only set the Id if we successfully created the order/quote
                        string xmlString = containerArray[payloadIndex].ToString();
                        history = CustomerHistory.FromXml(xmlString);
                    }
                    else
                    {
                        ApplicationLog.Log(
                            typeof(Customer).ToString(),
                            string.Format("{0}\n{1}", ApplicationLocalizer.Language.Translate(99412), comment), //"an error occured in the operation"
                            LogTraceLevel.Error);
                        Customer.InternalApplication.Services.Dialog.ShowMessage(99412, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }

                if (history != null)
                {
                    history.Parse();
                }

                return history;
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(typeof(Customer).ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Convert SalesOrderStatus to SalesStatus
        /// </summary>
        /// <param name="salesOrderStatus">SalesOrderStatus</param>
        /// <returns>SalesStatus</returns>
        internal static DE.SalesStatus GetSalesStatus(SalesOrderStatus salesOrderStatus)
        {
            switch (salesOrderStatus)
            {
                case SalesOrderStatus.Backorder: return DE.SalesStatus.Created;
                case SalesOrderStatus.Delivered: return DE.SalesStatus.Delivered;
                case SalesOrderStatus.Invoiced: return DE.SalesStatus.Invoiced;
                case SalesOrderStatus.Canceled: return DE.SalesStatus.Canceled;
                default: return DE.SalesStatus.Unknown;
            }
        }

        /// <summary>
        /// Gets customer balances from Ax and adds the local transaction not yet pushed to HQ.  
        /// </summary>
        /// <param name="customerId">Customer account number for which we make the call.</param>
        /// <param name="invoiceCustomerId">Customer linked invoice account number (if any)</param>
        /// <returns>An instance of CustomerBalamces class populated with proper data.</returns>
        internal static CustomerBalances GetCustomerBalances(string customerId, string invoiceCustomerId)
        {
            bool validResult = false;
            ReadOnlyCollection<object> returnValue = null;
            CustomerBalances customerBalances = new CustomerBalances();

            if (string.IsNullOrWhiteSpace(customerId))
            {
                return customerBalances;
            }

            try
            {
                returnValue = InternalApplication.TransactionServices.Invoke(
                 "GetCustomerBalance",
                 new object[] { customerId, LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreCurrency, LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId }
                 );

                validResult = Convert.ToBoolean(returnValue[1]);

                if (validResult)
                {
                    customerBalances.Balance = Convert.ToDecimal(returnValue[3]);
                    customerBalances.CreditLimit = Convert.ToDecimal(returnValue[4]);
                    customerBalances.InvoiceAccountBalance = Convert.ToDecimal(returnValue[5]);
                    customerBalances.InvoiceAccountCreditLimit = Convert.ToDecimal(returnValue[6]);
                    customerBalances.LastReplicatedTransactionCounter = Convert.ToInt64(returnValue[7]);
                }
                else
                {
                    ApplicationLog.Log(
                        typeof(Customer).ToString(),
                        string.Format("{0}\n{1}", ApplicationLocalizer.Language.Translate(99412), returnValue[2]), //"an error occured in the operation"
                        LogTraceLevel.Error);
                    Customer.InternalApplication.Services.Dialog.ShowMessage(99412, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(typeof(Customer).ToString(), ex);
                return customerBalances;
            }

            TransactionData transData = new TransactionData(PosApplication.Instance.Settings.Database.Connection,
                                                    PosApplication.Instance.Settings.Database.DataAreaID, PosApplication.Instance);

            customerBalances.LocalPendingBalance = transData.GetCustomerLocalPendingBalance(customerId, LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId, customerBalances.LastReplicatedTransactionCounter);

            if (!string.IsNullOrWhiteSpace(invoiceCustomerId))
            {
                customerBalances.LocalInvoicePendingBalance = transData.GetCustomerLocalPendingBalance(invoiceCustomerId, LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId, customerBalances.LastReplicatedTransactionCounter);
            }

            return customerBalances;
        }
    }

    /// <summary>
    /// String formatting class for Customer Order SalesStatus enum
    /// </summary>
    internal class SalesOrderStatusFormatter : IFormatProvider, ICustomFormatter
    {
        private readonly string Unknown;
        private readonly string Confirmed;
        private readonly string Created;
        private readonly string Processing;
        private readonly string Lost;
        private readonly string Canceled;
        private readonly string Sent;
        private readonly string Delivered;
        private readonly string Invoiced;

        /// <summary>
        /// Formats status settings.
        /// </summary>
        public SalesOrderStatusFormatter()
        {
            Unknown = ApplicationLocalizer.Language.Translate(56375); // "None";
            Confirmed = ApplicationLocalizer.Language.Translate(56404); // "Confirmed";
            Created = ApplicationLocalizer.Language.Translate(56376); // "Created";
            Processing = ApplicationLocalizer.Language.Translate(56402); // "Processing";
            Lost = ApplicationLocalizer.Language.Translate(56403); // "Lost";
            Canceled = ApplicationLocalizer.Language.Translate(56379); // "Canceled";
            Sent = ApplicationLocalizer.Language.Translate(56405); // "Sent";
            Delivered = ApplicationLocalizer.Language.Translate(56377); // "Delivered";
            Invoiced = ApplicationLocalizer.Language.Translate(56378); // "Invoiced";
        }

        /// <summary>
        /// The GetFormat method of the IFormatProvider interface.
        /// This must return an object that provides formatting services for the specified type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public object GetFormat(System.Type type)
        {
            return this;
        }

        /// <summary>
        /// The Format method of the ICustomFormatter interface.
        /// This must format the specified value according to the specified format settings.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="arg"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg is int || arg is DE.SalesStatus)
            {
                switch ((int)arg)
                {
                    case (int)DE.SalesStatus.Unknown: return this.Unknown;
                    case (int)DE.SalesStatus.Confirmed: return this.Confirmed;
                    case (int)DE.SalesStatus.Created: return this.Created;
                    case (int)DE.SalesStatus.Processing: return this.Processing;
                    case (int)DE.SalesStatus.Lost: return this.Lost;
                    case (int)DE.SalesStatus.Canceled: return this.Canceled;
                    case (int)DE.SalesStatus.Sent: return this.Sent;
                    case (int)DE.SalesStatus.Delivered: return this.Delivered;
                    case (int)DE.SalesStatus.Invoiced: return this.Invoiced;
                    default: return string.Empty;
                }
            }
            return (arg == null) ? string.Empty : arg.ToString();
        }
    }
}
