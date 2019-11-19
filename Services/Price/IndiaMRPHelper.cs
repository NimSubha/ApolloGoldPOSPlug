/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System.Data;
using System.Linq;
using LSRetailPosis.DataAccess.DataUtil;
using LSRetailPosis.Settings;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.Pos.PriceService
{
    /// <summary>
    /// Helper class for retrieving India MRP (Max. retail price).
    /// </summary>
    public static class IndiaMRPHelper
    {
        /// <summary>
        /// Get MRP of the sales line item.
        /// </summary>
        /// <param name="taxableItem">sales line item</param>
        /// <returns>MRP of the sales line item</returns>
        public static decimal GetMRP(ITaxableItem taxableItem)
        {
            SaleLineItem saleItem = taxableItem as SaleLineItem;
            if (saleItem == null)
            {
                return decimal.Zero;
            }

            decimal mrp = GetMRPFromTradeAgreement(saleItem);
            if (mrp != decimal.Zero)
            {
                return mrp;
            }

            return GetMRPFromItemMaster(saleItem.ItemId);
        }

        private static decimal GetMRPFromTradeAgreement(SaleLineItem saleItem)
        {
            RetailTransaction transaction = saleItem.RetailTransaction as RetailTransaction;
            decimal quantity = transaction.SaleItems.Where(x => x.ItemId == saleItem.ItemId && !x.Voided).Sum(x => x.Quantity);
            if (quantity == decimal.Zero)
            {
                quantity = 1;
            }

            Price priceService = Price.InternalApplication.Services.Price as Price;

            if (priceService != null)
            {
                PriceResult result = priceService.GetActiveTradeAgreement(transaction, saleItem, quantity);

                return result.IndiaMRP;
            }

            return 0;
        }

        private static decimal GetMRPFromItemMaster(string itemId)
        {
            DBUtil dbUtil = new DBUtil(ApplicationSettings.Database.LocalConnection);
            LSRetailPosis.DataAccess.DataUtil.SqlSelect sqlSelect = new LSRetailPosis.DataAccess.DataUtil.SqlSelect("INVENTTABLEMODULE");
            sqlSelect.Select("MAXIMUMRETAILPRICE_IN");
            sqlSelect.Where("ITEMID", itemId, true);
            sqlSelect.Where("DATAAREAID", ApplicationSettings.Database.DATAAREAID, true);
            sqlSelect.Where("MODULETYPE", 2, true);

            DataTable dt = dbUtil.GetTable(sqlSelect);
            decimal mrp = dt.Rows.Count > 0 ? (decimal)dt.Rows[0]["MAXIMUMRETAILPRICE_IN"] : decimal.Zero;

            NetTracer.Information("GetMRP(): MRP read from table: {0} using ITEMID: {1}; DATAAREAID: {2}; MODULETYPE: {3}", mrp, itemId, ApplicationSettings.Database.DATAAREAID, 2);

            return mrp;
        }
    }
}
