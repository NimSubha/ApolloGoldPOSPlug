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
using System.IO;
using System.Xml.Serialization;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using CS = Microsoft.Dynamics.Retail.Pos.Customer.Customer;

namespace Microsoft.Dynamics.Retail.Pos.Customer
{
    /// <summary>
    /// Customer history information
    /// </summary>
    [Serializable]
    [XmlRoot("CustomerHistory")]
    public class CustomerHistory
    {
        public CustomerHistory()
        {
            Items = new Collection<CustomerOrderHistoryItem>(new List<CustomerOrderHistoryItem>());
        }

        /// <summary>
        /// Date created (for serialization)
        /// </summary>
        [XmlElement("DateCreated")]
        public string DateCreatedString { get; set; }

        /// <summary>
        /// Last store customer visited
        /// </summary>
        [XmlElement("LastVisitStore")]
        public string LastVisitedStore { get; set; }

        /// <summary>
        /// Date of last recorded sales order (for serialization)
        /// </summary>
        [XmlElement("LastVisitDate")]
        public string LastVisitedDateString { get; set; }

        /// <summary>
        /// Total number of sales orders (for serialization)
        /// </summary>
        [XmlElement("TotalVisitsCount")]
        public string TotalVisitsCountString { get; set; }

        /// <summary>
        /// Sum of sales order line amounts (for serialization)
        /// </summary>
        [XmlElement("TotalSalesAmount")]
        public string TotalSalesAmountString { get; set; }

        /// <summary>
        /// Date created
        /// </summary>
        [XmlIgnore]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// date of last recorded sales order
        /// </summary>
        [XmlIgnore]
        public DateTime LastVisitedDate { get; set; }

        /// <summary>
        /// Total number of sales orders
        /// </summary>
        [XmlIgnore]
        public int TotalVisitsCount { get; set; }

        /// <summary>
        /// Sum of sales order line amounts
        /// </summary>
        [XmlIgnore]
        public decimal TotalSalesAmount { get; set; }

        /// <summary>
        /// Collection of order history elements
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Justification = "Non-read-only collection required for serialization.")]
        public Collection<CustomerOrderHistoryItem> Items { get; set; }

        /// <summary>
        /// Parse out strongly typed values from the serialized strings
        /// </summary>
        public void Parse()
        {
            //parse dates/numbers
            this.DateCreated = DateTime.Parse(DateCreatedString);
            this.LastVisitedDate = DateTime.Parse(LastVisitedDateString);
            this.TotalVisitsCount = int.Parse(TotalVisitsCountString);
            this.TotalSalesAmount = decimal.Parse(TotalSalesAmountString, System.Globalization.CultureInfo.InvariantCulture);

            //parse all the individual history items as well.
            foreach (CustomerOrderHistoryItem item in this.Items)
            {
                item.Parse();
            }
        }

        /// <summary>
        /// Create CustomerHistory from XML
        /// </summary>
        /// <param name="orderXml"></param>
        /// <returns></returns>
        public static CustomerHistory FromXml(string orderXml)
        {
            CustomerHistory orderInfo;
            XmlSerializer serializer = new XmlSerializer(typeof(CustomerHistory));
            using (StringReader reader = new StringReader(orderXml))
            {
                serializer = new XmlSerializer(typeof(CustomerHistory));
                orderInfo = (CustomerHistory)serializer.Deserialize(reader);
                return orderInfo;
            }
        }
    }

    /// <summary>
    /// Customer item history info
    /// </summary>
    [Serializable]
    [XmlType("Item")]
    public class CustomerOrderHistoryItem
    {
        /// <summary>
        /// Date of order (for serialization)
        /// </summary>
        [XmlAttribute("DateCreated")]
        public string OrderDateString { get; set; }

        /// <summary>
        /// Sales order id
        /// </summary>
        [XmlAttribute("SalesId")]
        public string OrderNumber { get; set; }

        /// <summary>
        /// Sales order status (for serialization)
        /// </summary>
        [XmlAttribute("Status")]
        public string OrderStatusString { get; set; }

        /// <summary>
        /// Store name
        /// </summary>
        [XmlAttribute("StoreName")]
        public string StoreName { get; set; }

        /// <summary>
        /// Item
        /// </summary>
        [XmlAttribute("ItemId")]
        public string ItemName { get; set; }

        /// <summary>
        /// Item item quantity (for serialization)
        /// </summary>
        [XmlAttribute("Quantity")]
        public string ItemQuantityString { get; set; }

        /// <summary>
        /// Item line amount (for serialization)
        /// </summary>
        [XmlAttribute("NetAmount")]
        public string ItemAmountString { get; set; }

        #region non-serialized, strongly typed properties

        /// <summary>
        /// Order status
        /// </summary>
        [XmlIgnore]
        public SalesStatus OrderStatus { get; set; }

        /// <summary>
        /// Item line quantity
        /// </summary>
        [XmlIgnore]
        public decimal ItemQuantity { get; set; }

        /// <summary>
        /// Sales order date
        /// </summary>
        [XmlIgnore]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Item line amount
        /// </summary>
        [XmlIgnore]
        public decimal ItemAmount { get; set; }

        #endregion

        /// <summary>
        /// Parse out strongly typed values from the serialized strings
        /// </summary>
        public void Parse()
        {
            this.OrderDate = DateTime.Parse(OrderDateString);
            this.OrderStatus = CS.GetSalesStatus((SalesOrderStatus)(int.Parse(OrderStatusString)));
            this.ItemQuantity = decimal.Parse(ItemQuantityString, System.Globalization.CultureInfo.InvariantCulture);
            this.ItemAmount = decimal.Parse(ItemAmountString, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
