/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Forms;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;

namespace RussiaItemTriggers
{
    [Export(typeof(IItemTrigger))]
    public class RussiaItemTriggers: IItemTrigger, IGlobalization
    {
        #region Globalization
        private readonly ReadOnlyCollection<string> supportedCountryRegions = new ReadOnlyCollection<string>(new string[] { SupportedCountryRegion.RU.ToString() });

        public System.Collections.ObjectModel.ReadOnlyCollection<string> SupportedCountryRegions
        {
            get { return supportedCountryRegions; }
        }
        #endregion

        #region Item Triggers
        public void PreSale(IPreTriggerResult preTriggerResult, Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.ISaleLineItem saleLineItem, Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.IPosTransaction posTransaction)
        {
            // Left empty on purpose.
        }

        public void PostSale(Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.IPosTransaction posTransaction)
        {
            // Left empty on purpose.
        }

        public void PreReturnItem(IPreTriggerResult preTriggerResult, Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.IPosTransaction posTransaction)
        {
            // Left empty on purpose.
        }

        public void PostReturnItem(Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.IPosTransaction posTransaction)
        {
            // Left empty on purpose.
        }

        public void PreVoidItem(IPreTriggerResult preTriggerResult, Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.IPosTransaction posTransaction, int lineId)
        {
            // Left empty on purpose.
        }

        public void PostVoidItem(Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.IPosTransaction posTransaction, int lineId)
        {
            // Left empty on purpose.
        }

        public void PreSetQty(IPreTriggerResult preTriggerResult, Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.ISaleLineItem saleLineItem, Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.IPosTransaction posTransaction, int lineId)
        {
            // Left empty on purpose.
        }

        public void PostSetQty(Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.IPosTransaction posTransaction, Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.ISaleLineItem saleLineItem)
        {
            RetailTransaction retailTransaction = posTransaction as RetailTransaction;

            if (retailTransaction == null)
            {
                throw new InvalidOperationException("posTransaction as RetailTransaction is null");
            }

            if (retailTransaction.SaleItems == null)
            {
                throw new InvalidOperationException("(posTransaction as RetailTransaction).SaleItems is null");
            }

            if (saleLineItem == null)
            {
                throw new ArgumentNullException("saleLineItem");
            }

            if (LSRetailPosis.Settings.ApplicationSettings.Terminal.ProcessGiftCardsAsPrepayments &&
                saleLineItem.Quantity < 0)
            {
                if (!retailTransaction.OperationCancelled &&
                    retailTransaction.SaleItems.OfType<IGiftCardLineItem>().Any(l => (!l.Voided && l.Amount != decimal.Zero && !l.AddTo)))
                {
                    TriggerHelpers.ShowDialog(MessageBoxButtons.OK, MessageBoxIcon.Error, 107005);
                    retailTransaction.OperationCancelled = true;
                }

                if (!retailTransaction.OperationCancelled &&
                    retailTransaction.SaleItems.OfType<IGiftCardLineItem>().Any(l => (!l.Voided && l.AddTo)))
                {
                    TriggerHelpers.ShowDialog(MessageBoxButtons.OK, MessageBoxIcon.Error, 107006);
                    retailTransaction.OperationCancelled = true;
                }
            }

            if (!retailTransaction.OperationCancelled &&
                !LSRetailPosis.Settings.ApplicationSettings.Terminal.ProcessReturnsAsInOriginalSaleShift_RU &&
                saleLineItem.Quantity < 0 &&
                !saleLineItem.ReceiptReturnItem &&
                retailTransaction.SaleItems.Any(l => (!l.Voided && l.ReceiptReturnItem)))
            {
                TriggerHelpers.ShowDialog(MessageBoxButtons.OK, MessageBoxIcon.Error, 106041);
                retailTransaction.OperationCancelled = true;
            }
        }

        public void PrePriceOverride(IPreTriggerResult preTriggerResult, Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.ISaleLineItem saleLineItem, Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.IPosTransaction posTransaction, int lineId)
        {
            // Left empty on purpose.
        }

        public void PostPriceOverride(Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.IPosTransaction posTransaction, Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.ISaleLineItem saleLineItem)
        {
            // Left empty on purpose.
        }

        public void PreClearQty(IPreTriggerResult preTriggerResult, Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.ISaleLineItem saleLineItem, Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.IPosTransaction posTransaction, int lineId)
        {
            // Left empty on purpose.
        }

        public void PostClearQty(Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.IPosTransaction posTransaction, Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity.ISaleLineItem saleLineItem)
        {
            // Left empty on purpose.
        }

        #endregion
    }
}
