/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Triggers;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    public sealed partial class RussianFiscalPrinter
    {
        #region Implementation of IDiscountTriggers - Line Discount

        /// <summary>
        /// Triggered prior to setting a line discount amount to the transaction.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="transaction"></param>
        /// <param name="lineId"></param>
        public void PreLineDiscountAmount(IPreTriggerResult preTriggerResult, IPosTransaction transaction, int lineId)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered prior to setting a line discount percent to the transaction.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="transaction"></param>
        /// <param name="lineId"></param>
        public void PreLineDiscountPercent(IPreTriggerResult preTriggerResult, IPosTransaction transaction, int lineId)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered after setting a line discount amount to the transaction.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="lineId"></param>
        public void PostLineDiscountAmount(IPosTransaction posTransaction, int lineId)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered after setting a line discount percent to the transaction.
        /// </summary>
        /// <param name="posTransaction"></param>
        /// <param name="lineId"></param>
        public void PostLineDiscountPercent(IPosTransaction posTransaction, int lineId)
        {
            //
            //Left empty on purpose
            //
        }

        #endregion

        #region Implementation of IDiscountTriggers - Total Discount

        /// <summary>
        /// Triggered prior to setting a total discount amount to the transaction.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        public void PreTotalDiscountAmount(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered prior to setting a total discount percent to the transaction.
        /// </summary>
        /// <param name="preTriggerResult"></param>
        /// <param name="posTransaction"></param>
        public void PreTotalDiscountPercent(IPreTriggerResult preTriggerResult, IPosTransaction posTransaction)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered after setting a total discount amount to the transaction.
        /// </summary>
        /// <param name="posTransaction"></param>
        public void PostTotalDiscountAmount(IPosTransaction posTransaction)
        {
            //
            //Left empty on purpose
            //
        }

        /// <summary>
        /// Triggered after setting a total discount percent to the transaction.
        /// </summary>
        /// <param name="posTransaction"></param>
        public void PostTotalDiscountPercent(IPosTransaction posTransaction)
        {
            //
            //Left empty on purpose
            //
        }

        #endregion

    }
}
