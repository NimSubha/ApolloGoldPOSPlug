/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System.Collections.Generic;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics365.Tax.Core;

namespace Microsoft.Dynamics.Retail.Pos.Tax.GenericTaxEngine
{
    /// <summary>
    /// Class holds the mapping data between source retail transaction and corresponding taxable document
    /// </summary>
    public class TaxableDocumentMapping
    {
        private Dictionary<ITaxableItem, TaxableItemId> taxableItemsMap;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="retailTransaction"></param>
        public TaxableDocumentMapping(RetailTransaction retailTransaction, TaxableDocumentObject taxableDocument)
        {
            RetailTransaction = retailTransaction;
            TaxableDocument = taxableDocument;
            taxableItemsMap = new Dictionary<ITaxableItem, TaxableItemId>();
        }

        /// <summary>
        /// The retail transaction.
        /// </summary>
        public RetailTransaction RetailTransaction { get; private set; }

        /// <summary>
        /// The taxable document.
        /// </summary>
        public TaxableDocumentObject TaxableDocument { get; private set; }

        /// <summary>
        /// Adds mapping between taxable item and its ID.
        /// </summary>
        /// <param name="taxableItem">The taxable item.</param>
        /// <param name="itemId">The taxable item ID.</param>
        public void AddTaxableItemMap(ITaxableItem taxableItem, TaxableItemId itemId)
        {
            taxableItemsMap.Add(taxableItem, itemId);
        }

        /// <summary>
        /// Looks up the ID for the taxable item.
        /// </summary>
        /// <param name="taxableItem">The taxable item.</param>
        /// <returns>The taxable item ID if found; otherwise null.</returns>
        public TaxableItemId lookupTaxableItemId(ITaxableItem taxableItem)
        {
            TaxableItemId value;
            taxableItemsMap.TryGetValue(taxableItem, out value);

            return value;
        }
    }
}
