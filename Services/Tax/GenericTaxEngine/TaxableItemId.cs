/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
namespace Microsoft.Dynamics.Retail.Pos.Tax.GenericTaxEngine
{
    /// <summary>
    /// Uniquely identifies the taxable item by item table ID and record ID.
    /// </summary>
    public class TaxableItemId
    {
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="itemTableId"></param>
        /// <param name="itemRecordId"></param>
        public TaxableItemId(int itemTableId, long itemRecordId)
        {
            TableId = itemTableId;
            RecordId = itemRecordId;
        }

        /// <summary>
        /// The table ID.
        /// </summary>
        public int TableId { get; private set; }

        /// <summary>
        /// The record ID.
        /// </summary>
        public long RecordId { get; private set; }
    }
}
