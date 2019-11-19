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
using System.ComponentModel.Composition;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using LSRetailPosis.DataAccess;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;


namespace Microsoft.Dynamics.Retail.Pos.StoreInventoryServices
{
    #region supporting classes
    /// <summary>
    /// Single line for a StockCount or PurchaseOrderReceiving document
    /// </summary>
    class SCJournalTransaction : ISCJournalTransaction
    {
        public string RecId { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string InventDimId { get; set; }
        public decimal Counted { get; set; }
        public string InventBatchId { get; set; }
        public string WmsLocationId { get; set; }
        public string WmsPalletId { get; set; }
        public string InventSiteId { get; set; }
        public string InventLocationId { get; set; }
        public string ConfigId { get; set; }
        public string InventSizeId { get; set; }
        public string InventColorId { get; set; }
        public string InventStyleId { get; set; }
        public string InventSerialId { get; set; }
        public string Guid { get; set; }
        public bool UpdatedInAx { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// Parse a xml format of SC journal transaction into object
        /// </summary>
        /// <param name="toDocument">Xml format of a SC journal transaction</param>
        public void Parse(XElement journalTransaction)
        {
            if (journalTransaction != null)
            {
                this.RecId = PRDocumentLine.GetAttributeValue(journalTransaction, "RecId");
                this.ItemId = PRDocumentLine.GetAttributeValue(journalTransaction, "ItemId");
                this.ItemName = PRDocumentLine.GetAttributeValue(journalTransaction, StoreInventoryConstants.EcoResProductName);
                this.InventDimId = PRDocumentLine.GetAttributeValue(journalTransaction, "InventDimId");
                this.Counted = Convert.ToDecimal(PRDocumentLine.GetAttributeValue(journalTransaction, "Counted"), CultureInfo.InvariantCulture);
                this.InventBatchId = PRDocumentLine.GetAttributeValue(journalTransaction, "InventBatchId");
                this.WmsLocationId = PRDocumentLine.GetAttributeValue(journalTransaction, "WmsLocationId");
                this.WmsPalletId = PRDocumentLine.GetAttributeValue(journalTransaction, "WmsPalletId");
                this.InventSiteId = PRDocumentLine.GetAttributeValue(journalTransaction, "InventSiteId");
                this.InventLocationId = PRDocumentLine.GetAttributeValue(journalTransaction, "InventLocationId");
                this.ConfigId = PRDocumentLine.GetAttributeValue(journalTransaction, "ConfigId");
                this.InventSizeId = PRDocumentLine.GetAttributeValue(journalTransaction, "InventSizeId");
                this.InventColorId = PRDocumentLine.GetAttributeValue(journalTransaction, "InventColorId");
                this.InventStyleId = PRDocumentLine.GetAttributeValue(journalTransaction, "InventStyleId");
                this.InventSerialId = PRDocumentLine.GetAttributeValue(journalTransaction, "InventSerialId");
                this.Guid = PRDocumentLine.GetAttributeValue(journalTransaction, "Guid");
                this.UpdatedInAx = Convert.ToBoolean(PRDocumentLine.GetAttributeValue(journalTransaction, "UpdatedInAx"));
                this.Message = PRDocumentLine.GetAttributeValue(journalTransaction, "Message");
            }
        }

        /// <summary>
        /// Serialize to xml format of a SC journal transaction
        /// </summary>
        /// <returns>Xml format of a SC journal transaction</returns>
        public string ToXml()
        {
            XElement inventJournalTrans =
                new XElement("InventJournalTrans",
                    new XAttribute("RecId", this.RecId ?? string.Empty),
                    new XAttribute("ItemId", this.ItemId ?? string.Empty),
                    new XAttribute(StoreInventoryConstants.EcoResProductName, this.ItemName ?? string.Empty),
                    new XAttribute("InventDimId", this.InventDimId ?? string.Empty),
                    new XAttribute("Counted", this.Counted),
                    new XAttribute("InventBatchId", this.InventBatchId ?? string.Empty),
                    new XAttribute("WmsLocationId", this.WmsLocationId ?? string.Empty),
                    new XAttribute("WmsPalletId", this.WmsPalletId ?? string.Empty),
                    new XAttribute("InventSiteId", this.InventSiteId ?? string.Empty),
                    new XAttribute("InventLocationId", ApplicationSettings.Terminal.InventLocationId ?? string.Empty),
                    new XAttribute("ConfigId", this.ConfigId ?? string.Empty),
                    new XAttribute("InventSizeId", this.InventSizeId ?? string.Empty),
                    new XAttribute("InventColorId", this.InventColorId ?? string.Empty),
                    new XAttribute("InventStyleId", this.InventStyleId ?? string.Empty),
                    new XAttribute("InventSerialId", this.InventSerialId ?? string.Empty),
                    new XAttribute("Guid", Guid ?? string.Empty)
                    );

            return inventJournalTrans.ToString();
        }
    }

    /// <summary>
    /// Transfer order receipt document
    /// </summary>
    class TRDocument : PRDocument
    {
        public string TransferId { get; set; }
        public string InventLocationIdFrom { get; set; }
        public string InventLocationIdTo { get; set; }
        public string ShipDate { get; set; }

        /// <summary>
        /// Parse a xml format of TO document into object
        /// </summary>
        /// <param name="toDocument">Xml format of a TO document</param>
        public override void Parse(XElement toDocument)
        {
            if (toDocument != null)
            {
                // OrderType is set based on InventLocationFrom and InventLocationTo
                this.RecId = PRDocumentLine.GetAttributeValue(toDocument, "RecId");
                this.TransferId = PRDocumentLine.GetAttributeValue(toDocument, "TransferId");
                this.InventLocationIdFrom = PRDocumentLine.GetAttributeValue(toDocument, "InventLocationIdFrom");
                this.InventLocationIdTo = PRDocumentLine.GetAttributeValue(toDocument, "InventLocationIdTo");
                this.ShipDate = PRDocumentLine.GetAttributeValue(toDocument, "ShipDate");

                this.PurchId = this.TransferId;
                this.DeliveryDate = this.ShipDate;

                if (this.InventLocationIdTo == ApplicationSettings.Terminal.InventLocationId)
                {
                    this.OrderType = PRCountingType.TransferIn;
                }

                if (this.InventLocationIdFrom == ApplicationSettings.Terminal.InventLocationId)
                {
                    this.OrderType = PRCountingType.TransferOut;
                }
            }
        }

        /// <summary>
        /// Serialize to xml format of a transfer document
        /// </summary>
        /// <returns>Xml format of a transfer document</returns>
        public override string ToXml()
        {
            StringBuilder sbOutput = new StringBuilder();

            sbOutput.Append("<InventTransferTable");
            sbOutput.Append(" TransferId=\"" + this.TransferId + "\"");
            sbOutput.Append(">");

            foreach (IPRDocumentLine line in this.PRDocumentLines)
            {
                sbOutput.Append(line.ToXml());
            }

            sbOutput.Append("</InventTransferTable>");

            return sbOutput.ToString();
        }
    }


    /// <summary>
    /// PickingList document
    /// </summary>
    class PickingListDocument : PRDocument
    {
        public string PickingRouteId { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public string InventLocationId { get; set; }

        /// <summary>
        /// Parse a xml format of TO document into object
        /// </summary>
        /// <param name="toDocument">Xml format of a TO document</param>
        public override void Parse(XElement toDocument)
        {
            if (toDocument != null)
            {
                // OrderType is set based on InventLocationFrom and InventLocationTo
                this.RecId = PRDocumentLine.GetAttributeValue(toDocument, "RecId");
                this.PickingRouteId = PRDocumentLine.GetAttributeValue(toDocument, "PickingRouteId");
                this.InventLocationId = PRDocumentLine.GetAttributeValue(toDocument, "InventLocationId");
                this.DeliveryDate = PRDocumentLine.GetAttributeValue(toDocument, "DlvDate");
                this.DeliveryMethod = PRDocumentLine.GetAttributeValue(toDocument, "DlvMode");
                this.PurchId = this.PickingRouteId;
                this.OrderType = PRCountingType.PickingList;                
            }
        }

        /// <summary>
        /// Serialize to xml format of a transfer document
        /// </summary>
        /// <returns>Xml format of a transfer document</returns>
        public override string ToXml()
        {
            StringBuilder sbOutput = new StringBuilder();

            sbOutput.Append("<WMSPickingRoute");
            sbOutput.Append(" PickingRouteId=\"" + this.PickingRouteId + "\"");
            sbOutput.Append(">");

            foreach (IPRDocumentLine line in this.PRDocumentLines)
            {
                sbOutput.Append(line.ToXml());
            }

            sbOutput.Append("</WMSPickingRoute>");

            return sbOutput.ToString();
        }
    }

    class SCJournal : ISCJournal
    {
        public string RecId { get; set; }
        public string JournalId { get; set; }
        public string Description { get; set; }
        public IList<ISCJournalTransaction> SCJournalLines { get; set; }
        private string Worker { get; set; }

        public SCJournal()
        {
            this.SCJournalLines = new List<ISCJournalTransaction>();
            this.Worker = "0";
        }

        /// <summary>
        /// Parse xml data into object
        /// </summary>
        /// <param name="scJournal">Xml format of a stock count journal</param>
        public void Parse(XElement scJournal)
        {
            if (scJournal != null)
            {
                this.RecId = PRDocumentLine.GetAttributeValue(scJournal, "RecId");
                this.JournalId = PRDocumentLine.GetAttributeValue(scJournal, "JournalId");
                this.Description = PRDocumentLine.GetAttributeValue(scJournal, "Description");
            }
        }

        /// <summary>
        /// Serialize to xml format of a stock count journal
        /// </summary>
        /// <returns>Xml format of a stock count journal</returns>
        public string ToXml()
        {
            StringBuilder sbOutput = new StringBuilder();

            sbOutput.Append("<InventJournalTable");
            sbOutput.Append(" JournalId=\"" + this.JournalId + "\"");
            sbOutput.Append(" RecId=\"" + this.RecId + "\"");
            sbOutput.Append(" Worker=\"" + this.Worker + "\"");
            sbOutput.Append(">");

            foreach (ISCJournalTransaction line in this.SCJournalLines)
            {
                sbOutput.Append(line.ToXml());
            }

            sbOutput.Append("</InventJournalTable>");

            return sbOutput.ToString();
        }
    }

    /// <summary>
    /// Purchase order receipt document
    /// </summary>
    class PRDocument : IPRDocument
    {
        public PRCountingType OrderType { get; set; }
        public string RecId { get; set; }
        public string PurchId { get; set; }
        public string PurchName { get; set; }
        public string DeliveryDate { get; set; }
        public string DeliveryMethod { get; set; }

        /// <summary>
        /// Gets or sets the type of the retail status.  Maps to AX enum HHTRetailStatusTypeBase
        /// </summary>
        /// <value>The type of the retail status.</value>
        public string RetailStatusType { get; set; }

        public IList<IPRDocumentLine> PRDocumentLines { get; set; }

        /// <summary>
        /// Parse xml data into object
        /// </summary>
        /// <param name="poDocument">Xml format of a PO document</param>
        public virtual void Parse(XElement poDocument)
        {
            if (poDocument != null)
            {
                this.RecId = PRDocumentLine.GetAttributeValue(poDocument, "RecId");
                this.PurchId = PRDocumentLine.GetAttributeValue(poDocument, "PurchId"); ;
                this.DeliveryDate = PRDocumentLine.GetAttributeValue(poDocument, "DeliveryDate");
                this.DeliveryMethod = PRDocumentLine.GetAttributeValue(poDocument, "DlvMode");
                this.RetailStatusType = PRDocumentLine.GetAttributeValue(poDocument, "PurchStatus");
            }
        }

        public PRDocument()
        {
            this.PRDocumentLines = new List<IPRDocumentLine>();
            this.OrderType = PRCountingType.PurchaseOrder;
        }

        /// <summary>
        /// Serialize to xml format of a PO document
        /// </summary>
        /// <returns>Xml format of a PO document</returns>
        public virtual string ToXml()
        {
            StringBuilder sbOutput = new StringBuilder();

            sbOutput.Append("<PurchTable");
            sbOutput.Append(" PurchId=\"" + this.PurchId + "\"");
            sbOutput.Append(" RecId=\"" + this.RecId + "\"");
            sbOutput.Append(">");

            foreach (IPRDocumentLine line in this.PRDocumentLines)
            {
                sbOutput.Append(line.ToXml());
            }

            sbOutput.Append("</PurchTable>");

            return sbOutput.ToString();
        }
    }

    /// <summary>
    /// Picking list document line
    /// </summary>
    class PickingListDocumentLine : PRDocumentLine
    {
       
        /// <summary>
        /// Parse xml format of Picking List line into object
        /// </summary>
        /// <param name="pickingListDocLine">Xml format of a TO line</param>
        public override void Parse(XElement pickingListDocLine)
        {
            if (pickingListDocLine != null)
            {
                this.RecId = PRDocumentLine.GetAttributeValue(pickingListDocLine,"RecId");
                this.ItemId = PRDocumentLine.GetAttributeValue(pickingListDocLine,"ItemId");
                this.ItemName = PRDocumentLine.GetAttributeValue(pickingListDocLine,StoreInventoryConstants.EcoResProductName);
                this.InventDimId = PRDocumentLine.GetAttributeValue(pickingListDocLine,"InventDimId");
                this.InventBatchId = PRDocumentLine.GetAttributeValue(pickingListDocLine,"InventBatchId");
                this.WmsLocationId = PRDocumentLine.GetAttributeValue(pickingListDocLine,"WmsLocationId");
                this.WmsPalletId = PRDocumentLine.GetAttributeValue(pickingListDocLine,"WmsPalletId");
                this.InventSiteId = PRDocumentLine.GetAttributeValue(pickingListDocLine,"InventSiteId");
                this.InventLocationId = PRDocumentLine.GetAttributeValue(pickingListDocLine,"InventLocationId");
                this.ConfigId = PRDocumentLine.GetAttributeValue(pickingListDocLine,"ConfigId");
                this.InventSizeId = PRDocumentLine.GetAttributeValue(pickingListDocLine,"InventSizeId");
                this.InventColorId = PRDocumentLine.GetAttributeValue(pickingListDocLine,"InventColorId");
                this.InventStyleId = PRDocumentLine.GetAttributeValue(pickingListDocLine,"InventStyleId");
                this.InventSerialId = PRDocumentLine.GetAttributeValue(pickingListDocLine,"InventSerialId");
                this.QtyOrdered = Convert.ToDecimal(PRDocumentLine.GetAttributeValue(pickingListDocLine,"Qty"), CultureInfo.InvariantCulture);
                this.Guid = PRDocumentLine.GetAttributeValue(pickingListDocLine,"Guid");
                this.UpdatedInAx = Convert.ToBoolean(PRDocumentLine.GetAttributeValue(pickingListDocLine,"UpdatedInAx"));
                this.Message = PRDocumentLine.GetAttributeValue(pickingListDocLine,"Message");
                this.DeliveryMethod = PRDocumentLine.GetAttributeValue(pickingListDocLine, "DlvMode");
            }
        }

        /// <summary>
        /// Serialize TO line into xml format
        /// </summary>
        /// <returns>Xml format of a TO line</returns>
        public override string ToXml()
        {
            XElement wmsOrderTrans =
                new XElement("WMSOrderTrans",
                    new XAttribute("RecId", this.RecId ?? string.Empty),
                    new XAttribute("ItemId", this.ItemId ?? string.Empty),
                    new XAttribute(StoreInventoryConstants.EcoResProductName, this.ItemName ?? string.Empty),
                    new XAttribute("InventDimId", this.InventDimId ?? string.Empty),
                    new XAttribute("InventBatchId", this.InventBatchId ?? string.Empty),
                    new XAttribute("WmsLocationId", this.WmsLocationId ?? string.Empty),
                    new XAttribute("WmsPalletId", this.WmsPalletId ?? string.Empty),
                    new XAttribute("InventSiteId", this.InventSiteId ?? string.Empty),
                    new XAttribute("InventLocationId", ApplicationSettings.Terminal.InventLocationId ?? string.Empty),
                    new XAttribute("ConfigId", this.ConfigId ?? string.Empty),
                    new XAttribute("InventSizeId", this.InventSizeId ?? string.Empty),
                    new XAttribute("InventColorId", this.InventColorId ?? string.Empty),
                    new XAttribute("InventSerialId", this.InventSerialId ?? string.Empty),
                    new XAttribute("Qty", this.PurchReceivedNow),
                    new XAttribute("Guid", Guid ?? string.Empty),
                    new XAttribute("DlvMode", this.DeliveryMethod)
            );

            return wmsOrderTrans.ToString();
        }
    }

    /// <summary>
    /// Transfer order recepit line
    /// </summary>
    class TRDocumentLine : PRDocumentLine
    {
        public decimal QtyTransfer { get; set; }
        public decimal QtyShipped { get; set; }
        public decimal QtyReceived { get; set; }
        public decimal QtyShipNow { get; set; }
        public decimal QtyReceiveNow { get; set; }
        public decimal QtyRemainShip { get; set; }
        public decimal QtyRemainReceive { get; set; }
        private PRCountingType TRDocumentType { get; set; }

        public TRDocumentLine(PRCountingType prType)
        {
            this.TRDocumentType = prType;
        }

        /// <summary>
        /// Parse xml format of TO line into object
        /// </summary>
        /// <param name="toDocLine">Xml format of a TO line</param>
        public override void Parse(XElement toDocLine)
        {
            if (toDocLine != null)
            {
                this.RecId = PRDocumentLine.GetAttributeValue(toDocLine, "RecId");
                this.ItemId = PRDocumentLine.GetAttributeValue(toDocLine,"ItemId");
                this.ItemName = PRDocumentLine.GetAttributeValue(toDocLine,StoreInventoryConstants.EcoResProductName);
                this.InventDimId = PRDocumentLine.GetAttributeValue(toDocLine,"InventDimId");
                this.InventBatchId = PRDocumentLine.GetAttributeValue(toDocLine,"InventBatchId");
                this.WmsLocationId = PRDocumentLine.GetAttributeValue(toDocLine,"WmsLocationId");
                this.WmsPalletId = PRDocumentLine.GetAttributeValue(toDocLine,"WmsPalletId");
                this.InventSiteId = PRDocumentLine.GetAttributeValue(toDocLine,"InventSiteId");
                this.InventLocationId = PRDocumentLine.GetAttributeValue(toDocLine,"InventLocationId");
                this.ConfigId = PRDocumentLine.GetAttributeValue(toDocLine,"ConfigId");
                this.InventSizeId = PRDocumentLine.GetAttributeValue(toDocLine,"InventSizeId");
                this.InventColorId = PRDocumentLine.GetAttributeValue(toDocLine,"InventColorId");
                this.InventStyleId = PRDocumentLine.GetAttributeValue(toDocLine,"InventStyleId");
                this.InventSerialId = PRDocumentLine.GetAttributeValue(toDocLine,"InventSerialId");
                this.QtyTransfer = Convert.ToDecimal(PRDocumentLine.GetAttributeValue(toDocLine, "QtyTransfer"), CultureInfo.InvariantCulture);
                this.QtyShipped = Convert.ToDecimal(PRDocumentLine.GetAttributeValue(toDocLine, "QtyShipped"), CultureInfo.InvariantCulture);
                this.QtyReceived = Convert.ToDecimal(PRDocumentLine.GetAttributeValue(toDocLine, "QtyReceived"), CultureInfo.InvariantCulture);
                this.QtyShipNow = Convert.ToDecimal(PRDocumentLine.GetAttributeValue(toDocLine, "QtyShipNow"), CultureInfo.InvariantCulture);
                this.QtyReceiveNow = Convert.ToDecimal(PRDocumentLine.GetAttributeValue(toDocLine, "QtyReceiveNow"), CultureInfo.InvariantCulture);
                this.QtyRemainShip = Convert.ToDecimal(PRDocumentLine.GetAttributeValue(toDocLine, "QtyRemainShip"), CultureInfo.InvariantCulture);
                this.QtyRemainReceive = Convert.ToDecimal(PRDocumentLine.GetAttributeValue(toDocLine, "QtyRemainReceive"), CultureInfo.InvariantCulture);
                this.PurchUnit = PRDocumentLine.GetAttributeValue(toDocLine,"UnitId");

                this.Guid = PRDocumentLine.GetAttributeValue(toDocLine,"Guid");
                this.UpdatedInAx = Convert.ToBoolean(PRDocumentLine.GetAttributeValue(toDocLine,"UpdatedInAx"));
                this.Message = PRDocumentLine.GetAttributeValue(toDocLine,"Message");

                if (this.TRDocumentType == PRCountingType.TransferIn)
                {
                    this.PurchReceived = this.QtyReceived + this.QtyReceiveNow;
                    this.PurchReceivedNow = 0;
                }

                if (this.TRDocumentType == PRCountingType.TransferOut)
                {
                    this.PurchReceived = this.QtyShipped + this.QtyShipNow;
                    this.PurchReceivedNow = 0;
                }

                this.QtyOrdered = this.QtyTransfer;
            }
        }

        /// <summary>
        /// Serialize TO line into xml format
        /// </summary>
        /// <returns>Xml format of a TO line</returns>
        public override string ToXml()
        {
            this.QtyTransfer = this.QtyOrdered;

            if (this.TRDocumentType == PRCountingType.TransferIn)
            {
                this.QtyReceived = this.PurchReceived;
                this.QtyReceiveNow = this.PurchReceivedNow;
            }

            if (this.TRDocumentType == PRCountingType.TransferOut)
            {
                this.QtyShipped = this.PurchReceived;
                this.QtyShipNow = this.PurchReceivedNow;
            }

            this.InventLocationId = ApplicationSettings.Terminal.InventLocationId;

            XElement inventTransferLine =
                new XElement("InventTransferLine",
                    new XAttribute("RecId", this.RecId ?? string.Empty),
                    new XAttribute("ItemId", this.ItemId ?? string.Empty),
                    new XAttribute(StoreInventoryConstants.EcoResProductName, this.ItemName ?? string.Empty),
                    new XAttribute("InventDimId", this.InventDimId ?? string.Empty),
                    new XAttribute("InventBatchId", this.InventBatchId ?? string.Empty),
                    new XAttribute("WmsLocationId", this.WmsLocationId ?? string.Empty),
                    new XAttribute("WmsPalletId", this.WmsPalletId ?? string.Empty),
                    new XAttribute("InventSiteId", this.InventSiteId ?? string.Empty),
                    new XAttribute("InventLocationId", this.InventLocationId ?? string.Empty),
                    new XAttribute("ConfigId", this.ConfigId ?? string.Empty),
                    new XAttribute("InventSizeId", this.InventSizeId ?? string.Empty),
                    new XAttribute("InventColorId", this.InventColorId ?? string.Empty),
                    new XAttribute("InventSerialId", this.InventSerialId ?? string.Empty),
                    new XAttribute("QtyTransfer", this.QtyTransfer),
                    new XAttribute("QtyShipped", this.QtyShipped),
                    new XAttribute("QtyReceived", this.QtyReceived),
                    new XAttribute("QtyShipNow", this.QtyShipNow),
                    new XAttribute("QtyReceiveNow", this.QtyReceiveNow),
                    new XAttribute("QtyRemainShip", this.QtyRemainShip),
                    new XAttribute("QtyRemainReceive", this.QtyRemainReceive),
                    new XAttribute("UnitId", this.PurchUnit ?? string.Empty),
                    new XAttribute("Guid", Guid)
                    );

            return inventTransferLine.ToString();
        }
    }

    /// <summary>
    /// Purchase order receipt line
    /// </summary>
    class PRDocumentLine : IPRDocumentLine
    {
        public string RecId { get; set; }
        public string ItemId { get; set; }
        public string ItemName { get; set; }
        public string InventDimId { get; set; }
        public decimal QtyOrdered { get; set; }
        public decimal PurchQty { get; set; }
        public string BarCode { get; set; }
        public string PurchUnit { get; set; }
        public decimal PurchReceived { get; set; }
        public decimal PurchReceivedNow { get; set; }
        public string InventBatchId { get; set; }
        public string WmsLocationId { get; set; }
        public string WmsPalletId { get; set; }
        public string InventSiteId { get; set; }
        public string InventLocationId { get; set; }
        public string ConfigId { get; set; }
        public string InventSerialId { get; set; }
        public string InventSizeId { get; set; }
        public string InventColorId { get; set; }
        public string InventStyleId { get; set; }
        public bool UpdatedInAx { get; set; }
        public string Message { get; set; }
        public string Guid { get; set; }
        public string DeliveryMethod { get; set; }

        /// <summary>
        /// Parse xml format of PO line into object
        /// </summary>
        /// <param name="poDocLine">Xml format of a PO line</param>
        public virtual void Parse(XElement poDocLine)
        {
            if (poDocLine != null)
            {
                this.RecId = GetAttributeValue(poDocLine, "RecId");
                this.ItemId = GetAttributeValue(poDocLine, "ItemId");
                this.ItemName = GetAttributeValue(poDocLine, StoreInventoryConstants.EcoResProductName);
                this.InventDimId = GetAttributeValue(poDocLine, "InventDimId");
                this.QtyOrdered = Convert.ToDecimal(GetAttributeValue(poDocLine, "QtyOrdered"), CultureInfo.InvariantCulture);
                this.PurchQty = Convert.ToDecimal(GetAttributeValue(poDocLine, "PurchQty"), CultureInfo.InvariantCulture);
                this.PurchUnit = GetAttributeValue(poDocLine, "PurchUnit");
                this.PurchReceived = Convert.ToDecimal(GetAttributeValue(poDocLine, "PurchReceivedNow"), CultureInfo.InvariantCulture);
                this.PurchReceivedNow = 0;
                this.InventBatchId = GetAttributeValue(poDocLine, "InventBatchId");
                this.WmsLocationId = GetAttributeValue(poDocLine, "WmsLocationId");
                this.WmsPalletId = GetAttributeValue(poDocLine, "WmsPalletId");
                this.InventSiteId = GetAttributeValue(poDocLine, "InventSiteId");
                this.InventLocationId = GetAttributeValue(poDocLine, "InventLocationId");
                this.InventSizeId = GetAttributeValue(poDocLine, "InventSizeId");
                this.InventColorId = GetAttributeValue(poDocLine, "InventColorId");
                this.InventStyleId = GetAttributeValue(poDocLine, "InventStyleId");
                this.InventSerialId = GetAttributeValue(poDocLine, "InventSerialId");
                this.ConfigId = GetAttributeValue(poDocLine, "ConfigId");
                this.UpdatedInAx = Convert.ToBoolean(GetAttributeValue(poDocLine, "UpdatedInAx"));
                
                this.Message = GetAttributeValue(poDocLine, "Message");
                this.Guid = GetAttributeValue(poDocLine, "Guid");
                this.DeliveryMethod = GetAttributeValue(poDocLine, "DlvMode");
            }
        }

        internal static string GetAttributeValue(XElement xe, string attributeName)
        {
            string result = string.Empty;

            XAttribute attribute = xe.Attribute(attributeName);
            if (attribute != null)
            {
                result = attribute.Value;
            }

            return result;
        }

        /// <summary>
        /// Serialize PO line object into xml format
        /// </summary>
        /// <returns>Xml format of a PO line</returns>
        public virtual string ToXml()
        {
            this.InventLocationId = ApplicationSettings.Terminal.InventLocationId;

            XElement purchLine =
                new XElement("PurchLine",
                    new XAttribute("RecId", this.RecId),
                    new XAttribute("ItemId", this.ItemId),
                    new XAttribute(StoreInventoryConstants.EcoResProductName, this.ItemName ?? string.Empty),
                    new XAttribute("InventDimId", this.InventDimId ?? string.Empty),
                    new XAttribute("QtyOrdered", this.QtyOrdered),
                    new XAttribute("PurchQty", this.PurchQty),
                    new XAttribute("PurchUnit", this.PurchUnit ?? string.Empty),
                    new XAttribute("PurchReceivedNow", this.PurchReceivedNow),
                    new XAttribute("InventBatchId", this.InventBatchId ?? string.Empty),
                    new XAttribute("WmsLocationId", this.WmsLocationId ?? string.Empty),
                    new XAttribute("WmsPalletId", this.WmsPalletId ?? string.Empty),
                    new XAttribute("InventSiteId", this.InventSiteId ?? string.Empty),
                    new XAttribute("InventLocationId", this.InventLocationId ?? string.Empty),
                    new XAttribute("ConfigId", this.ConfigId ?? string.Empty),
                    new XAttribute("InventSizeId", this.InventSizeId ?? string.Empty),
                    new XAttribute("InventColorId", this.InventColorId ?? string.Empty),
                    new XAttribute("InventSerialId", this.InventSerialId ?? string.Empty),
                    new XAttribute("Guid", Guid),
                    new XAttribute("DlvMode", this.DeliveryMethod ?? string.Empty)
                    );

            return purchLine.ToString();
        }
    }
    #endregion

    /// <summary>
    /// Service implementation
    /// </summary>
    [Export(typeof(IStoreInventoryServices))]
    public sealed class StoreInventoryServices : IStoreInventoryServices
    {
        #region Webservice constants

        /// <summary>
        /// Types of valid service commands
        /// </summary>
        private static class ServiceCommandType
        {
            /// <summary>
            /// Import Stock Count
            /// </summary>
            public const string Import = "IMPORT";

            /// <summary>
            /// Get list of PR documents
            /// </summary>
            public const string PrDocList = "PRDOCLIST";

            /// <summary>
            /// Get line details for a given PR document
            /// </summary>
            public const string PrDoc = "PRDOC";

            /// <summary>
            /// Import receiving counts for a given PR document
            /// </summary>
            public const string PrDocResult = "PRDOCRESULT";

            /// <summary>
            /// Get a list of stock count mask ids
            /// </summary>
            public const string StockCountMasks = "STOCKCOUNT";
        }

        /// <summary>
        /// Types of valid etnries
        /// </summary>
        private static class ServiceEntryType
        {
            public const string PurchaseOrder = "1";

            public const string StockAreaCount = "3";
            public const string StockScheduledCount = "4";
            public const string StockCount = "5";
            public const string TransferOut = "8";
            public const string TransferIn = "11";
        }

        /// <summary>
        /// Valid document types for the PRDocList command
        /// </summary>
        private static class PrDocListType
        {
            public const string PurchaseOrder = "PO";
            public const string TransferOrder = "TO";
            public const string PickingList = "PL";
        }

        #endregion

        /// <summary>
        /// Format that the webservice expects for date fields
        /// </summary>

        [Import]
        public IApplication Application { get; set; }

        private const string DateFormat = "dd.MM.yyyy";

        /// <summary>
        /// Format that the webservice expects for time fields
        /// </summary>
        private const string TimeFormat = "hh:mm:ss";

        private static readonly Dictionary<HHTRetailStatusTypeBase, PurchaseOrderReceiptStatus> RetailStatusPurceOrderMapTable = new Dictionary<HHTRetailStatusTypeBase, PurchaseOrderReceiptStatus>()
            {
                { HHTRetailStatusTypeBase.None,             PurchaseOrderReceiptStatus.Open },
                { HHTRetailStatusTypeBase.Document,         PurchaseOrderReceiptStatus.Open },
                { HHTRetailStatusTypeBase.Sent,             PurchaseOrderReceiptStatus.Open },
                { HHTRetailStatusTypeBase.PartReceipt,      PurchaseOrderReceiptStatus.Closed },
                { HHTRetailStatusTypeBase.ClosedOk,         PurchaseOrderReceiptStatus.Closed },
                { HHTRetailStatusTypeBase.ClosedDifference, PurchaseOrderReceiptStatus.Closed },
                { HHTRetailStatusTypeBase.Canceled,         PurchaseOrderReceiptStatus.Closed }
            };

        private static string RemoveXmlDeclaration(ref string xml)
        {
            // Remove Xml declaration
            Match xmlDeclaration = Regex.Match(xml, @"<\?xml.*\?>");
            if (xmlDeclaration.Success)
            {
                xml = xml.Replace(xmlDeclaration.Value, string.Empty);
            }

            return xml;
        }
        private static PurchaseOrderReceiptStatus MapHHTRetailStatusTypeBaseToPurchaseOrderReceiptStatus(HHTRetailStatusTypeBase status)
        {
            PurchaseOrderReceiptStatus result;

            if (RetailStatusPurceOrderMapTable.ContainsKey(status))
            {   // Map the status 
                result = RetailStatusPurceOrderMapTable[status];
            }
            else
            {   // Default status to closed (not displayed in POS)
                result = PurchaseOrderReceiptStatus.Closed;
            }

            return result;
        }

        #region IStoreInventoryServices Members

        /// <summary>
        /// Commit all pending stock counts by transmitting them 
        /// up to HQ and marking the local copy as 'committed'
        /// </summary>
        public ISCJournal CommitStockCounts(string journalId, string recId)
        {
            bool succeeded = false;
            string returnMessage = string.Empty;
            string retVal = string.Empty;
            ISCJournal scJournal = new SCJournal();

            StockCountData scData = new StockCountData(
                Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID, ApplicationSettings.Terminal.StorePrimaryId);

            // Read line from database
            DataTable lines = scData.GetStockCountLines(journalId);

            scJournal.JournalId = journalId;
            scJournal.RecId = recId;

            if (lines != null)
            {
                foreach (DataRow row in lines.Rows)
                {
                    ISCJournalTransaction line = new SCJournalTransaction();

                    line.RecId = row[DataAccessConstants.RecId].ToString();
                    line.ItemId = row[DataAccessConstants.ItemNumber].ToString();
                    line.ItemName = row[DataAccessConstants.ItemName].ToString();
                    line.Counted = Convert.ToDecimal(row[DataAccessConstants.Quantity]);
                    line.ConfigId = row[DataAccessConstants.ConfigId].ToString();
                    line.Guid = row[DataAccessConstants.Guid].ToString();
                    line.InventColorId = row[DataAccessConstants.InventColorId].ToString();
                    line.InventSizeId = row[DataAccessConstants.InventSizeId].ToString();
                    line.InventStyleId = row[DataAccessConstants.InventStyleId].ToString();
                    scJournal.SCJournalLines.Add(line);
                }
            }

            // Serialize ISCJournal to xml
            string importValue = scJournal.ToXml();

            this.CallTransactionService(ref succeeded, ref returnMessage, ref retVal, "UpdateInventoryJournal", importValue);

            if (succeeded)
            {
                RemoveXmlDeclaration(ref retVal);

                // Deserialize retVal to ISCJournalTransaction
                IList<ISCJournalTransaction> scJournalTransactions = new List<ISCJournalTransaction>();
                if (!string.IsNullOrWhiteSpace(retVal))
                {
                    XDocument doc = XDocument.Parse(retVal);
                    XElement root = doc.Elements("InventJournalTable").FirstOrDefault();
                    if (root != null)
                    {
                        scJournalTransactions = root.Elements("InventJournalTrans").Select<XElement, ISCJournalTransaction>(
                            (scLine) =>
                            {
                                ISCJournalTransaction scJournalTransaction = new SCJournalTransaction();
                                scJournalTransaction.Parse(scLine);
                                return scJournalTransaction;
                            }).ToList<ISCJournalTransaction>();
                    }
                }

                scJournal.SCJournalLines = scJournalTransactions;
                return scJournal;
            }

            return null;
        }

        /// <summary>
        /// Transmit all lines of a order Receipt back to HQ, and delete local copy
        /// </summary>
        /// <param name="orderId">Order Id</param>
        /// <param name="receiptNumber">Order document receipt number</param>
        /// <param name="prType">Order receipt counting type</param>
        [SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling")]
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public IPRDocument CommitOrderReceipt(string orderId, string receiptNumber, PRCountingType prType)
        {
            bool succeeded = false;
            string returnMessage = string.Empty;
            string retVal = string.Empty;
            IPRDocument prDocument = null;

            PurchaseOrderReceiptData prData = StoreInventoryServices.GetPurchaseOrderReceiptData();

            try
            {
                // Read line from database
                DataTable lines = prData.GetPurchaseOrderReceiptLines(orderId, receiptNumber);

                if (prType == PRCountingType.PurchaseOrder)
                {
                    prDocument = new PRDocument();
                    prDocument.PurchId = orderId;
                }
                else if (prType == PRCountingType.TransferIn || prType == PRCountingType.TransferOut)
                {
                    prDocument = new TRDocument();
                    ((TRDocument)prDocument).TransferId = orderId;
                }
                else if (prType == PRCountingType.PickingList)
                {
                    prDocument = new PickingListDocument();
                    ((PickingListDocument)prDocument).PickingRouteId = orderId;
                }

                prDocument.RecId = receiptNumber;
                prDocument.OrderType = prType;

                if (lines != null)
                {
                    foreach (DataRow row in lines.Rows)
                    {
                        IPRDocumentLine line = null;
                        if (prType == PRCountingType.PurchaseOrder)
                        {
                            line = new PRDocumentLine();
                        }
                        else if (prType == PRCountingType.TransferIn || prType == PRCountingType.TransferOut)
                        {
                            line = new TRDocumentLine(prType);
                        }
                        else if (prType == PRCountingType.PickingList)
                        {
                            line = new PickingListDocumentLine();
                        }

                        line.RecId = row[DataAccessConstants.LineReceiptNumber].ToString();
                        line.ItemId = row[DataAccessConstants.ItemNumber].ToString();
                        line.ItemName = row[DataAccessConstants.ItemName].ToString();
                        line.QtyOrdered = Convert.ToDecimal(row[DataAccessConstants.QuantityOrdered]);
                        line.PurchReceived = Convert.ToDecimal(row[DataAccessConstants.QuantityReceived]);
                        line.PurchReceivedNow = Convert.ToDecimal(row[DataAccessConstants.QuantityReceivedNow]);
                        line.PurchUnit = row[DataAccessConstants.UnitOfMeasure].ToString();
                        line.Guid = row[DataAccessConstants.Guid].ToString();
                        line.InventColorId = row[DataAccessConstants.InventColorId].ToString();
                        line.InventSizeId = row[DataAccessConstants.InventSizeId].ToString();
                        line.ConfigId = row[DataAccessConstants.ConfigId].ToString();
                        line.DeliveryMethod = row[DataAccessConstants.DeliveryMethod].ToString();
                        prDocument.PRDocumentLines.Add(line);
                    }
                }

                // Serialize IPRDocument to xml
                string importValue = prDocument.ToXml();

                switch (prType)
                {
                    case PRCountingType.PurchaseOrder:
                        this.CallTransactionService(ref succeeded, ref returnMessage, ref retVal, "UpdatePurchaseOrder", importValue);
                        break;
                    case PRCountingType.TransferIn:
                    case PRCountingType.TransferOut:
                        this.CallTransactionService(ref succeeded, ref returnMessage, ref retVal, "UpdateTransferOrder", importValue);
                        break;
                    case PRCountingType.PickingList:
                        this.CallTransactionService(ref succeeded, ref returnMessage, ref retVal, "UpdatePickingList", importValue);
                        break;
                }

                if (succeeded)
                {
                    // Remove Xml declaration
                    Match xmlDeclaration = Regex.Match(retVal, @"<\?xml.*\?>");
                    if (xmlDeclaration.Success)
                    {
                        retVal = retVal.Replace(xmlDeclaration.Value, string.Empty);
                    }

                    // Deserialize retVal to IPRDocumentLine
                    IList<IPRDocumentLine> prDocLines = new List<IPRDocumentLine>();
                    if (!string.IsNullOrEmpty(retVal))
                    {
                        XDocument doc = XDocument.Parse(retVal);
                        XElement root = null;
                        if (prType == PRCountingType.PurchaseOrder)
                        {
                            root = doc.Elements("PurchTable").FirstOrDefault();
                            if (root != null)
                            {
                                prDocLines = root.Elements("PurchLine").Select<XElement, IPRDocumentLine>(
                                    (poLine) =>
                                    {
                                        IPRDocumentLine prDocumentLine = new PRDocumentLine();
                                        prDocumentLine.Parse(poLine);
                                        return prDocumentLine;
                                    }).ToList<IPRDocumentLine>();
                            }
                        }
                        else if (prType == PRCountingType.TransferIn || prType == PRCountingType.TransferOut)
                        {
                            root = doc.Elements("InventTransferTable").FirstOrDefault();
                            if (root != null)
                            {
                                prDocLines = root.Elements("InventTransferLine").Select<XElement, IPRDocumentLine>(
                                    (toLine) =>
                                    {
                                        IPRDocumentLine toDocumentLine = new TRDocumentLine(prType);
                                        toDocumentLine.Parse(toLine);
                                        return toDocumentLine;
                                    }).ToList<IPRDocumentLine>();
                            }
                        }
                        else if (prType == PRCountingType.PickingList)
                        {
                            root = doc.Elements("WMSPickingRoute").FirstOrDefault();
                            if (root != null)
                            {
                                prDocLines = root.Elements("WMSOrderTrans").Select<XElement, IPRDocumentLine>(
                                    (plLine) =>
                                    {
                                        IPRDocumentLine toDocumentLine = new PickingListDocumentLine();
                                        toDocumentLine.Parse(plLine);
                                        return toDocumentLine;
                                    }).ToList<IPRDocumentLine>();
                            }
                        }
                    }

                    prDocument.PRDocumentLines = prDocLines;
                    return prDocument;
                }

                return null;
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        private static PurchaseOrderReceiptData GetPurchaseOrderReceiptData()
        {
            return new PurchaseOrderReceiptData(
                ApplicationSettings.Database.LocalConnection,
                ApplicationSettings.Database.DATAAREAID,
                ApplicationSettings.Terminal.StorePrimaryId);
        }

        /// <summary>
        /// Create a stock count journal
        /// </summary>
        public void CreateStockCountJournal()
        {
            bool succeeded = false;
            string retVal = string.Empty;
            string returnMessage = string.Empty;
            string inventLocId = ApplicationSettings.Terminal.InventLocationId;
            this.CallTransactionService(ref succeeded, ref returnMessage, ref retVal, "CreateInventoryJournal", inventLocId, string.Format("Manual Stock Count at {0} on {1}", inventLocId, DateTime.Now.ToShortDateString()), StoreInventoryConstants.StockCountSourcePOS);

            if (succeeded)
            {
                GetStockCountJournals();
            }
        }
        /// <summary>
        /// Sync SC journals in the local DB with HQ
        /// </summary>
        public IList<ISCJournal> GetStockCountJournals()
        {
            StockCountData scData = new StockCountData(
                Application.Settings.Database.Connection, Application.Settings.Database.DataAreaID, ApplicationSettings.Terminal.StorePrimaryId);

            bool succeeded = false;
            string retVal = string.Empty;
            string returnMessage = string.Empty;
            this.CallTransactionService(ref succeeded, ref returnMessage, ref retVal, "GetInventJournals", ApplicationSettings.Terminal.InventLocationId);

            IEnumerable<ISCJournal> scJournals = new List<ISCJournal>();

            if (succeeded)
            {
                // Remove Xml declaration
                RemoveXmlDeclaration(ref retVal);

                if (!string.IsNullOrEmpty(retVal))
                {
                    XDocument doc = XDocument.Parse(retVal);
                    XElement root = doc.Elements("InventJournalTables").FirstOrDefault();

                    if (root != null)
                    {
                        scJournals = root.Elements("InventJournalTable").Select<XElement, ISCJournal>(
                            (sc) =>
                            {
                                ISCJournal scJournal = new SCJournal();
                                scJournal.Parse(sc);
                                return scJournal;
                            });
                    }
                }
            }

            // Insert for new journal, update existing journal
            foreach (ISCJournal journal in scJournals)
            {
                scData.InsertStockCountJournal(journal.RecId, journal.JournalId, journal.Description);
            }

            //Delete journals from POS if they can NOT be found in HQ
            scData.DeleteUnspecifiedJournals(scJournals.Select(journal => journal.RecId));

            return scJournals.ToList<ISCJournal>();
        }

        /// <summary>
        /// Sync SC in the local DB with HQ
        /// </summary>
        public IList<ISCJournalTransaction> GetStockCountJournalTransactions(string journalId)
        {
            try
            {

                bool succeeded = false;
                string returnMessage = string.Empty;
                string retVal = string.Empty;

                this.CallTransactionService(ref succeeded, ref returnMessage, ref retVal, "GetInventJournal", journalId, ApplicationSettings.Terminal.InventLocationId);

                if (succeeded)
                {
                    IList<ISCJournalTransaction> scJournalTransactions = new List<ISCJournalTransaction>();

                    RemoveXmlDeclaration(ref retVal);

                    if (!string.IsNullOrWhiteSpace(retVal))
                    {
                        XDocument doc = XDocument.Parse(retVal);
                        XElement root = doc.Elements("InventJournalTable").FirstOrDefault();

                        if (root != null)
                        {
                            scJournalTransactions = root.Elements("InventJournalTrans").Select<XElement, ISCJournalTransaction>(
                                    (scLine) =>
                                    {
                                        ISCJournalTransaction scJournalTransaction = new SCJournalTransaction();
                                        scJournalTransaction.Parse(scLine);
                                        return scJournalTransaction;
                                    }).ToList<ISCJournalTransaction>();

                        }
                    }

                    return scJournalTransactions;
                }

                return null;
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        /// <summary>
        /// Download list of Order Receipts from HQ and insert them into the local DB
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        public IList<IPRDocument> GetOrderReceipts()
        {
            string[] docTypes = null;

            // Get PR documents.
            // e.g. "R-000006; - PO-000023;PURCHASEUNIT;1;100.00;;No;<status>\n"
            docTypes = new string[] { PrDocListType.PurchaseOrder, PrDocListType.TransferOrder, PrDocListType.PickingList };

            bool succeeded = false;
            string retVal = string.Empty;
            string returnMessage = string.Empty;

            // Process PR documents for all document types.
            List<IPRDocument> prDocuments = new List<IPRDocument>();

            // Create DAL object
            PurchaseOrderReceiptData prData = new PurchaseOrderReceiptData(
                ApplicationSettings.Database.LocalConnection,
                ApplicationSettings.Database.DATAAREAID,
                ApplicationSettings.Terminal.StorePrimaryId);

            // Loop through each document type because TransactionService returns PO by the requested PO document type
            foreach (string docType in docTypes)
            {
                IEnumerable<IPRDocument> prDocs = new List<IPRDocument>();

                switch (docType)
                {
                    case PrDocListType.PurchaseOrder:
                        this.CallTransactionService(ref succeeded, ref returnMessage, ref retVal, "GetOpenPurchaseOrders", ApplicationSettings.Terminal.InventLocationId);
                        break;
                    case PrDocListType.TransferOrder:
                        this.CallTransactionService(ref succeeded, ref returnMessage, ref retVal, "GetOpenTransferOrders", ApplicationSettings.Terminal.InventLocationId);
                        break;
                    case PrDocListType.PickingList:
                        this.CallTransactionService(ref succeeded, ref returnMessage, ref retVal, "GetPickingLists", ApplicationSettings.Terminal.InventLocationId);
                        break;
                }

                if (succeeded)
                {
                    // Remove Xml declaration
                    RemoveXmlDeclaration(ref retVal);

                    if (!string.IsNullOrEmpty(retVal))
                    {
                        XDocument doc = XDocument.Parse(retVal);
                        XElement root = null;
                        switch (docType)
                        {
                            case PrDocListType.PurchaseOrder:
                                root = doc.Elements("PurchTables").FirstOrDefault();
                                if (root != null)
                                {
                                    prDocs = root.Elements("PurchTable").Select<XElement, IPRDocument>(
                                        (po) =>
                                        {
                                            IPRDocument prDocument = new PRDocument();
                                            prDocument.Parse(po);
                                            return prDocument;
                                        });
                                }
                                break;
                            case PrDocListType.TransferOrder:
                                root = doc.Elements("InventTransferTables").FirstOrDefault();
                                if (root != null)
                                {
                                    prDocs = root.Elements("InventTransferTable").Select<XElement, IPRDocument>(
                                        (tr) =>
                                        {
                                            IPRDocument trDocument = new TRDocument();
                                            trDocument.Parse(tr);
                                            return trDocument;
                                        });
                                }
                                break;
                            case PrDocListType.PickingList:
                                root = doc.Elements("WMSPickingRoutes").FirstOrDefault();
                                if (root != null)
                                {
                                    prDocs = root.Elements("WMSPickingRoute").Select<XElement, IPRDocument>(
                                       (pl) =>
                                       {
                                           IPRDocument plDocument = new PickingListDocument();
                                           plDocument.Parse(pl);
                                           return plDocument;
                                       });
                                }
                                break;
                        }
                    }

                    // Update PO document type with current requested document type
                    foreach (IPRDocument doc in prDocs)
                    {
                        PurchaseOrderReceiptStatus posStatus;
                        int temp;

                        if (int.TryParse(doc.RetailStatusType, out temp))
                        {
                            posStatus = MapHHTRetailStatusTypeBaseToPurchaseOrderReceiptStatus((HHTRetailStatusTypeBase)temp);
                        }
                        else
                        {
                            posStatus = PurchaseOrderReceiptStatus.Open;
                        }
                        // Insert receipt into DB if it is not already existing
                        int result = prData.InsertReceipt(doc.OrderType, doc.RecId, doc.PurchId, posStatus, doc.DeliveryMethod);
                    }

                    //Gather receipt documents per each document type
                    prDocuments.AddRange(prDocs);
                }
            }

            //Delete receipts from POS if they can NOT be found in HQ
            prData.DeleteUnspecifiedReceipts(prDocuments.Select(doc => doc.RecId));

            return prDocuments;
        }

        /// <summary>
        /// Sync order receipt lines by receipt ID in the local DB with HQ
        /// </summary>
        /// <param name="receiptID">A document with receipt ID</param>
        /// <returns>A order docuemnt with a list of receipt document lines</returns>
        public IList<IPRDocumentLine> GetOrderReceiptLines(string orderNumber, PRCountingType prType)
        {
            try
            {

                bool succeeded = false;
                string returnMessage = string.Empty;
                string retVal = string.Empty;

                if (prType == PRCountingType.PurchaseOrder)
                {
                    this.CallTransactionService(ref succeeded, ref returnMessage, ref retVal, "GetPurchaseOrder", orderNumber, ApplicationSettings.Terminal.InventLocationId);
                }
                else if (prType == PRCountingType.TransferIn || prType == PRCountingType.TransferOut)
                {
                    this.CallTransactionService(ref succeeded, ref returnMessage, ref retVal, "GetTransferOrder", orderNumber);
                }
                else if (prType == PRCountingType.PickingList)
                {
                    this.CallTransactionService(ref succeeded, ref returnMessage, ref retVal, "GetPickingList", orderNumber, ApplicationSettings.Terminal.InventLocationId);
                }

                if (succeeded)
                {
                    IList<IPRDocumentLine> prDocLines = new List<IPRDocumentLine>();

                    // Remove Xml declaration
                    RemoveXmlDeclaration(ref retVal);

                    if (!string.IsNullOrEmpty(retVal))
                    {
                        XDocument doc = XDocument.Parse(retVal);
                        XElement root = null;

                        if (prType == PRCountingType.PurchaseOrder)
                        {
                            root = doc.Elements("PurchTable").FirstOrDefault();
                            if (root != null)
                            {
                                prDocLines = root.Elements("PurchLine").Select<XElement, IPRDocumentLine>(
                                    (poLine) =>
                                    {
                                        IPRDocumentLine poDocumentLine = new PRDocumentLine();
                                        poDocumentLine.Parse(poLine);
                                        return poDocumentLine;
                                    }).ToList<IPRDocumentLine>();
                            }
                        }
                        else if (prType == PRCountingType.TransferIn || prType == PRCountingType.TransferOut)
                        {
                            root = doc.Elements("InventTransferTable").FirstOrDefault();
                            if (root != null)
                            {
                                prDocLines = root.Elements("InventTransferLine").Select<XElement, IPRDocumentLine>(
                                    (toLine) =>
                                    {
                                        IPRDocumentLine toDocumentLine = new TRDocumentLine(prType);
                                        toDocumentLine.Parse(toLine);
                                        return toDocumentLine;
                                    }).ToList<IPRDocumentLine>();
                            }
                        }
                        else if (prType == PRCountingType.PickingList)
                        {
                            root = doc.Elements("WMSPickingRoute").FirstOrDefault();
                            if (root != null)
                            {
                                prDocLines = root.Elements("WMSOrderTrans").Select<XElement, IPRDocumentLine>(
                                    (plLine) =>
                                    {
                                        IPRDocumentLine plDocumentLine = new PickingListDocumentLine();
                                        plDocumentLine.Parse(plLine);
                                        return plDocumentLine;
                                    }).ToList<IPRDocumentLine>();
                            }
                        }
                    }

                    return prDocLines;
                }

                return null;
            }
            catch (Exception ex)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
        }

        #endregion

        private void CallTransactionService(ref bool succeeded, ref string returnMessage, ref string returnResult, string methodName, params object[] parameters)
        {
            try
            {
                ReadOnlyCollection<object> result = this.Application.TransactionServices.Invoke(methodName, parameters);

                if ((result != null) && (result.Count == 4))
                {
                    bool.TryParse(result[1].ToString(), out succeeded);
                    returnMessage = result[2].ToString();
                    returnResult = result[3].ToString();
                }
            }
            catch (Exception x)
            {
                LSRetailPosis.ApplicationExceptionHandler.HandleException("StoreTransactionServices.CallTransactionService", x);
                throw;
            }
        }
    }
}
