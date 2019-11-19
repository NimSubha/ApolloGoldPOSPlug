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
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using LSRetailPosis;
using LSRetailPosis.ButtonGrid;
using LSRetailPosis.DataAccess;
using LSRetailPosis.DataAccess.DataUtil;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Notification.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.DataEntity;
using Microsoft.Practices.Prism.Interactivity.InteractionRequest;

namespace Microsoft.Dynamics.Retail.Pos.Dialog.ViewModels
{
    /// <summary>
    /// Smart search view model
    /// </summary>
    internal sealed class SearchViewModel : INotifyPropertyChanged
    {

        #region Fields

        public const int PAGE_SIZE = 50;
        private const int MINIMUM_SEARCH_TERM_LENGTH = 2;  // Minimum two characters are reqruied for search.
        public const string PROPERTY_SEARCH_TERMS = "SearchTerms";
        public const string PROPERTY_SEARCH_TYPE = "SearchType";
        public const string PROPERTY_ADD_TO_SALE = "AddToSale";

        private bool addToSale;
        private string searchTerms;
        private readonly long searchCategoryHierarchyId;
        private long searchCategoryId;
        private SearchType searchType;
        private string sortColumn = "Name";
        private bool sortAsc = true;
        private bool isLastRowLoaded;
        private bool itemPriceVisible;
        private int? selectedResult;
        private Image selectedImage;
        private List<ResultRow> result = new List<ResultRow>();
        private readonly ItemData itemData = new ItemData(ApplicationSettings.Database.LocalConnection,
                                        ApplicationSettings.Database.DATAAREAID,
                                        ApplicationSettings.Terminal.StorePrimaryId);

        #endregion

        #region Construction

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchViewModel"/> class.
        /// </summary>
        /// <param name="searchType">Startup type of the search.</param>
        /// <param name="searchTerm">The search term.</param>
        public SearchViewModel(SearchType searchType, string searchTerm)
        {
            this.MinimumSearchTermLengh = MINIMUM_SEARCH_TERM_LENGTH;
            this.SearchTerms = searchTerm ?? string.Empty;
            this.SearchType = searchType;

            // If search type is category then search term is the "Category RecId"
            if (searchType == SearchType.Category)
            {
                Tuple<string, long> categoryDetail;

                if ((long.TryParse(SearchTerms, out this.searchCategoryId))
                    && (categoryDetail = itemData.GetCategoryDetails(searchCategoryId)) != null)
                {
                    this.SearchTerms = categoryDetail.Item1;
                    this.searchCategoryHierarchyId = categoryDetail.Item2;
                }
                else
                {
                    NetTracer.Warning("SearchViewModel : Invalid category specified '{0}'", searchTerm);
                }
            }

            if (searchCategoryHierarchyId == 0)
            {
                this.searchCategoryHierarchyId = itemData.GetRetailProductHierarchy();
            }
        }

        #endregion

        #region Properties

        public bool AddToSale
        {
            get { return addToSale; }
            set
            {
                if (value != addToSale)
                {
                    addToSale = value;
                    OnPropertyChanged(PROPERTY_ADD_TO_SALE);
                }
            }
        }

        /// <summary>
        /// Gets the results.
        /// </summary>
        public ReadOnlyCollection<ResultRow> Results
        {
            get
            {
                return this.result.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets or sets the search terms.
        /// </summary>
        public string SearchTerms
        {
            get { return searchTerms; }
            set
            {
                if (!string.Equals(searchTerms, value, StringComparison.OrdinalIgnoreCase))
                {
                    searchTerms = value;
                    OnPropertyChanged(PROPERTY_SEARCH_TERMS);
                }
            }
        }

        /// <summary>
        /// Gets or sets the selected image.
        /// </summary>
        public Image SelectedImage
        {
            get { return selectedImage; }
            set
            {
                if (selectedImage != value)
                {
                    if (selectedImage != null)
                    {
                        selectedImage.Dispose();
                    }

                    selectedImage = value;
                    OnPropertyChanged("SelectedImage");
                }
            }
        }

        /// <summary>
        /// Gets the selected result.
        /// </summary>
        /// <remarks>Selected result if available, null otherwise.</remarks>
        public ResultRow SelectedResult
        {
            get
            {
                return (selectedResult.HasValue && selectedResult.Value >= 0)
                    ? this.result[selectedResult.Value]
                    : null;
            }
        }

        /// <summary>
        /// Gets the type of the search.
        /// </summary>
        public SearchType SearchType
        {
            get { return searchType; }
            private set
            {
                if (searchType != value)
                {
                    searchType = value;

                    OnPropertyChanged(PROPERTY_SEARCH_TYPE);
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether item price visible.
        /// </summary>
        /// <value> 
        ///   <c>true</c> if true then price is visible; otherwise not.</c>.
        /// </value>
        public bool ItemPriceVisible
        {
            get { return itemPriceVisible; }
            set
            {
                if (itemPriceVisible != value)
                {
                    if (value)
                    {
                        CalculatePrice();
                    }

                    itemPriceVisible = value;
                    OnPropertyChanged("ItemPriceVisible");
                }
            }
        }

        /// <summary>
        /// Gets the minimum search term lengh.
        /// </summary>
        public int MinimumSearchTermLengh { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Updates the result list.
        /// </summary>
        /// <param name="fromRow">From row.</param>
        private void UpdateResultList(int fromRow)
        {
            NetTracer.Information("SearchViewModel : UpdateResultList : Start");

            if (fromRow == 0)
            {
                this.result.Clear();
                ExecuteSelect(null);
            }

            if (isLastRowLoaded
                || this.SearchTerms.Length < MinimumSearchTermLengh)
            {
                return;
            }

            SqlConnection connection = ApplicationSettings.Database.LocalConnection;

            try
            {
                using (SqlCommand command = new SqlCommand())
                {
                    if (SearchType == SearchType.Category)
                    {
                        GetCategoryQuery(command);

                        // Cagegory mode automatically switchs to items when executed.
                        this.SearchType = SearchType.Item;
                    }
                    else
                    {
                        GetDefaultQuery(command);
                    }

                    command.Connection = connection;
                    command.Parameters.AddWithValue("@FROMROW", fromRow);
                    command.Parameters.AddWithValue("@TOROW", (fromRow + PAGE_SIZE));

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            this.result.Add(new ResultRow(reader));
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }


            // If we didn't get back a full page of results then we loaded everything
            if (this.result.Count % PAGE_SIZE > 0)
                isLastRowLoaded = true;

            // Get the price of the items if enabled.
            if (ItemPriceVisible)
            {
                CalculatePrice();
            }

            OnPropertyChanged("Results");

            NetTracer.Information("SearchViewModel : UpdateResultList : End");
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "No user input")]
        private void GetDefaultQuery(SqlCommand command)
        {
            bool includeItems = (this.SearchType == SearchType.All || this.SearchType == SearchType.Item);
            bool includeCustomers = (this.SearchType == SearchType.All || this.SearchType == SearchType.Customer);
            bool includeCategories = (this.SearchType == SearchType.All);
            const string excludeCondition = "0=1 AND";

            string query = string.Format(
                "SELECT * FROM " +
                    "(SELECT *, ROW_NUMBER() OVER (ORDER BY {0} {1}) AS ROW " +
                        "FROM " +
                        "(SELECT IT.ITEMID AS NUMBER, COALESCE(TR.NAME, IT.ITEMNAME, IT.ITEMID) AS NAME, @ITEMTYPE AS TYPE, ISNULL(IM.UNITID, '') AS TAG " +
                            "FROM ASSORTEDINVENTITEMS IT " +
                            "JOIN INVENTTABLEMODULE IM ON IT.ITEMID = IM.ITEMID AND IM.MODULETYPE = 2 " +
                            "JOIN ECORESPRODUCT AS PR ON PR.RECID = IT.PRODUCT " +
                            "LEFT JOIN ECORESPRODUCTTRANSLATION AS TR ON PR.RECID = TR.PRODUCT AND TR.LANGUAGEID = @CULTUREID  " +
                            "WHERE {2} IT.STORERECID = @STORERECID AND (ISNULL(TR.NAME, IT.ITEMNAME) LIKE @SEARCHTERM OR PR.SEARCHNAME LIKE @SEARCHTERM OR IT.ITEMID LIKE @SEARCHTERM) " +

                       /* "UNION ALL " + RH

                        "SELECT C.ACCOUNTNUM AS NUMBER, P.NAME AS NAME, @CUSTOMERTYPE AS TYPE, '' AS TAG " +
                            "FROM CUSTTABLE C " +
                            "JOIN DIRPARTYTABLE P ON C.PARTY = P.RECID " +
                            "LEFT JOIN " +
                                "(SELECT  PARTY, MAX([ADDRESS]) AS [ADDRESS] " +
                                    "FROM DIRPARTYLOCATION L " +
                                    "LEFT JOIN LOGISTICSPOSTALADDRESS PA ON PA.LOCATION = L.LOCATION " +
                                    "WHERE L.VALIDFROM <= SYSUTCDATETIME() AND L.VALIDTO >= SYSUTCDATETIME() AND L.ISPOSTALADDRESS = 1 AND ([ADDRESS] LIKE @SEARCHTERM) " +
                                    "GROUP BY PARTY) AD " +
                                "ON AD.PARTY = C.PARTY " +
                            "LEFT JOIN " +
                                "(SELECT PARTY, MAX(LOCATOR) AS LOCATOR " +
                                    "FROM DIRPARTYLOCATION L " +
                                    "LEFT JOIN LOGISTICSELECTRONICADDRESS EA ON EA.LOCATION = L.LOCATION " +
                                    "WHERE L.VALIDFROM <= SYSUTCDATETIME() AND L.VALIDTO >= SYSUTCDATETIME() AND (EA.TYPE = @EA_PHONE OR EA.TYPE = @EA_EMAIL) AND LOCATOR LIKE @SEARCHTERM " +
                                    "GROUP BY PARTY) ED " +
                                "ON ED.PARTY = C.PARTY " +
                            "WHERE {3} (C.ACCOUNTNUM LIKE @SEARCHTERM OR P.NAME LIKE @SEARCHTERM OR AD.[ADDRESS] IS NOT NULL OR ED.LOCATOR IS NOT NULL) " +*/

                            //"UNION ALL " +

                            //"SELECT CAST(C.RECID AS nvarchar) AS NUMBER, ISNULL(NAME, FRIENDLYNAME) AS NAME, @CATEGORYTYPE AS TYPE, '' AS TAG " +
                //    "FROM ECORESCATEGORY C " +
                //    "LEFT JOIN ECORESCATEGORYTRANSLATION T ON T.CATEGORY = C.RECID AND T.LANGUAGEID = @CULTUREID " +
                //    "WHERE {4} ISACTIVE = 1 AND CATEGORYHIERARCHY = @CATEGORYHIERARCHY AND NAME LIKE @SEARCHTERM " +
                        ") UN " +
                    ") RN " +
                    "WHERE RN.ROW >= @FROMROW AND RN.ROW <= @TOROW",
                    sortColumn,
                    sortAsc ? "ASC" : "DESC",
                    includeItems ? string.Empty : excludeCondition,
                    includeCustomers ? string.Empty : excludeCondition,
                    includeCategories ? string.Empty : excludeCondition);

            command.CommandText = query;
            command.Parameters.AddWithValue("@CULTUREID", ApplicationSettings.Terminal.CultureName);
            command.Parameters.AddWithValue("@ITEMTYPE", SearchType.Item);
            command.Parameters.AddWithValue("@CUSTOMERTYPE", SearchType.Customer);
            command.Parameters.AddWithValue("@CATEGORYTYPE", SearchType.Category);
            command.Parameters.AddWithValue("@CATEGORYHIERARCHY", searchCategoryHierarchyId);
            command.Parameters.AddWithValue("@EA_PHONE", ElectronicAddressMethodType.Phone);
            command.Parameters.AddWithValue("@EA_EMAIL", ElectronicAddressMethodType.Email);
            command.Parameters.AddWithValue("@STORERECID", ApplicationSettings.Terminal.StorePrimaryId);
            command.Parameters.AddWithValue("@SEARCHTERM", String.Format("%{0}%", this.SearchTerms));
        }

        [SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "No user input")]
        private void GetCategoryQuery(SqlCommand command)
        {
            string query = string.Format(
                "SELECT * FROM " +
                    "(SELECT *, ROW_NUMBER() OVER (ORDER BY {0} {1}) AS ROW " +
                        "FROM " +
                        "(SELECT IT.ITEMID AS NUMBER, COALESCE(TR.NAME, IT.ITEMNAME, IT.ITEMID) AS NAME, @ITEMTYPE AS TYPE, ISNULL(IM.UNITID, '') AS TAG " +
                            "FROM ASSORTEDINVENTITEMS IT " +
                            "JOIN INVENTTABLEMODULE IM ON IT.ITEMID = IM.ITEMID AND IM.MODULETYPE = 2 " +
                            "JOIN ECORESPRODUCTCATEGORY PC ON PC.PRODUCT = IT.PRODUCT " +
                            "JOIN ECORESPRODUCT AS PR ON PR.RECID = IT.PRODUCT " +
                            "JOIN RETAILCATEGORYCONTAINMENTLOOKUP CL ON CL.CONTAINEDCATEGORY = PC.CATEGORY AND CL.CATEGORY = @CATEGORY " +
                            "LEFT JOIN ECORESPRODUCTTRANSLATION AS TR ON PR.RECID = TR.PRODUCT AND TR.LANGUAGEID = @CULTUREID " +
                            "WHERE  IT.STORERECID = @STORERECID " +
                        ") UN " +
                    ") RN " +
                    "WHERE RN.ROW >= @FROMROW AND RN.ROW <= @TOROW",
                    sortColumn,
                    sortAsc ? "ASC" : "DESC");

            command.CommandText = query;
            command.Parameters.AddWithValue("@CULTUREID", ApplicationSettings.Terminal.CultureName);
            command.Parameters.AddWithValue("@ITEMTYPE", SearchType.Item);
            command.Parameters.AddWithValue("@STORERECID", ApplicationSettings.Terminal.StorePrimaryId);
            command.Parameters.AddWithValue("@CATEGORY", this.searchCategoryId);
        }

        /// <summary>
        /// Gets the item image.
        /// </summary>
        /// <param name="itemNumber">The item number.</param>
        /// <returns>The image if available, else null.</returns>
        private static Image GetItemImage(string itemNumber)
        {
            Image result = null;
            SqlConnection connection = ApplicationSettings.Database.LocalConnection;
            const string query = "SELECT TOP 1 MEDIUMSIZE FROM ECORESPRODUCTIMAGE IM " +
                                    "INNER JOIN INVENTTABLE IT ON IM.REFRECORD = IT.RECID " +
                                    "WHERE ITEMID = @ITEMNUMBER " +
                                    "ORDER BY DEFAULTIMAGE DESC ";

            try
            {
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ITEMNUMBER", itemNumber);

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    result = GUIHelper.GetBitmap(command.ExecuteScalar() as byte[]);
                }
            }
            catch (Exception ex)
            {
                // Image loading failure should not block operation.
                NetTracer.Warning(ex, "SearchViewModel : GetItemImage : Failed to load image for item {0}", itemNumber);
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return result;
        }

        /// <summary>
        /// Calculates the price.
        /// </summary>
        private void CalculatePrice()
        {
            foreach (var row in this.Results)
            {
                row.CalculateItemPrice();
            }
        }

        #endregion

        #region Commands

        /// <summary>
        /// Executes the search.
        /// </summary>
        public void ExecuteSearch()
        {
            // Reset last row loaded flag
            isLastRowLoaded = false;

            UpdateResultList(0);
        }

        /// <summary>
        /// Executes the next page.
        /// </summary>
        public void ExecuteNextPage()
        {
            UpdateResultList(result.Count);
        }

        /// <summary>
        /// Executes the clear.
        /// </summary>
        public void ExecuteClear()
        {
            this.SearchTerms = null;

            this.result.Clear();

            ExecuteSelect(null);
        }

        /// <summary>
        /// Executes the select.
        /// </summary>
        /// <param name="selectedIndex">Index of the selected.</param>
        public void ExecuteSelect(int? selectedIndex)
        {
            if (this.selectedResult != selectedIndex)
            {
                this.selectedResult = selectedIndex;

                // Clear the image if there is any.
                this.SelectedImage = null;

                OnPropertyChanged("SelectedResult");
            }
        }

        /// <summary>
        /// Executes the customer transactions.
        /// </summary>
        public void ExecuteCustomerTransactions()
        {
            Dialog.InternalApplication.Services.Customer.Transactions(this.SelectedResult.Number);
        }

        /// <summary>
        /// Shows the product details form.
        /// </summary>
        public void ExecuteProductDetails()
        {
            string itemNumber = (this.SelectedResult != null) ? this.SelectedResult.Number : string.Empty;

            InteractionRequestedEventArgs request = new InteractionRequestedEventArgs(
                    new ProductDetailsConfirmation() { ItemNumber = itemNumber }, () => { });
            Dialog.InternalApplication.Services.Interaction.InteractionRequest(request);

            ProductDetailsConfirmation confirmation = request.Context as ProductDetailsConfirmation;
            this.AddToSale = confirmation != null && confirmation.Confirmed && confirmation.AddToSale;
        }

        /// <summary>
        /// Executes the filter all.
        /// </summary>
        public void ExecuteFilterAll()
        {
            if (this.SearchType != SearchType.All)
            {
                this.SearchType = SearchType.All;
                ExecuteSearch();
            }
        }

        /// <summary>
        /// Executes the filter items only.
        /// </summary>
        public void ExecuteFilterItemsOnly()
        {
            if (this.SearchType != SearchType.Item)
            {
                this.SearchType = SearchType.Item;
                ExecuteSearch();
            }
        }

        /// <summary>
        /// Executes the filter customers only.
        /// </summary>
        public void ExecuteFilterCustomersOnly()
        {
            if (this.SearchType != SearchType.Customer)
            {
                this.SearchType = SearchType.Customer;
                ExecuteSearch();
            }
        }

        /// <summary>
        /// Executes the category search.
        /// </summary>
        /// <param name="categoryId">The category id.</param>
        public void ExecuteFilterCategory()
        {
            if (this.SearchType != SearchType.Category)
            {
                // For category search, the term is not user input rather it is category number.
                this.SearchType = SearchType.Category;
                this.searchCategoryId = long.Parse(this.SelectedResult.Number);
                this.SearchTerms = this.SelectedResult.Name;

                ExecuteSearch();
            }
        }

        /// <summary>
        /// Executes the load row detail.
        /// </summary>
        public void ExecuteLoadRowDetail()
        {
            ResultRow selectedRow = this.SelectedResult;

            if (selectedRow != null)
            {
                if ((selectedRow.SearchType == SearchType.Customer) && (selectedRow.cachedCustomerResult == null))
                {
                    selectedRow.cachedCustomerResult = Dialog.InternalApplication.BusinessLogic.CustomerSystem.GetCustomerInfo(selectedRow.Number);
                }
                else if (selectedRow.SearchType == SearchType.Item)
                {
                    this.SelectedImage = GetItemImage(selectedRow.Number);
                }
            }

            OnPropertyChanged("SelectedResult");
        }

        #endregion

        #region INotifyPropertyChanged Members

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        private void OnPropertyChanged(string propertyName)
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

                throw new ArgumentException(msg);
            }
        }

        #endregion

    }

    #region Types

    /// <summary>
    /// Search row data object.
    /// </summary>
    internal class ResultRow
    {
        internal ICustomer cachedCustomerResult = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResultRow"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public ResultRow(IDataReader reader)
        {
            this.Number = DBUtil.ToStr(reader["NUMBER"]);
            this.Name = DBUtil.ToStr(reader["NAME"]);
            this.SearchType = (SearchType)DBUtil.ToInt32(reader["TYPE"]);
            this.Tag = reader["TAG"];
        }

        /// <summary>
        /// Gets the number.
        /// </summary>
        public string Number
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the name.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of the search.
        /// </summary>
        public SearchType SearchType
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets row type specific tag
        /// </summary>
        public object Tag
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the item price.
        /// </summary>
        public string ItemPrice
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the type of the formatted result.
        /// </summary>
        /// <value>
        /// The type of the formatted result.
        /// </value>
        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Used for data binding.")]
        public string FormattedResultType
        {
            get
            {
                switch (this.SearchType)
                {
                    case SearchType.Item:
                        return ApplicationLocalizer.Language.Translate(1741);
                    case SearchType.Customer:
                        return ApplicationLocalizer.Language.Translate(1742);
                    case SearchType.Category:
                        return ApplicationLocalizer.Language.Translate(1743);
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// Gets the customer.
        /// </summary>
        public ICustomer GetCustomer()
        {
            return cachedCustomerResult;
        }

        /// <summary>
        /// Calculates the item price.
        /// </summary>
        public void CalculateItemPrice()
        {
            if (this.SearchType == SearchType.Item && this.ItemPrice == null)
            {
                try
                {
                    decimal price = Dialog.InternalApplication.Services.Price.GetItemPrice(this.Number, (string)this.Tag);
                    this.ItemPrice = Dialog.InternalApplication.Services.Rounding.RoundForDisplay(price, true, false);
                }
                catch (Exception ex)
                {
                    // Price calculation failure should not block operation.
                    NetTracer.Warning(ex, "SearchResult : CalculateItemPrice : Failed to calculate price for item {0}", this.Number);
                }
            }
        }
    }

    /// <summary>
    /// Serach result data object.
    /// </summary>
    internal class SearchResult : ISearchResult
    {
        /// <summary>
        /// Gets the type of the search result.
        /// </summary>
        public SearchResultType SearchType { get; set; }

        /// <summary>
        /// Gets the number.
        /// </summary>
        public string Number { get; set; }

    }


    #endregion
}
