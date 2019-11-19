/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter
{
    public sealed partial class RussianFiscalPrinter
    {

        #region IDiscount implementation
        /// <summary>
        /// Returns true if total discount amount is authorized.
        /// </summary>
        /// <param name="retail"></param>
        /// <param name="amountValue"></param>
        /// <param name="maxAmountValue"></param>
        /// <returns></returns>
        public bool AuthorizeTotalDiscountAmount(IRetailTransaction retail, decimal amountValue, decimal maxAmountValue)
        {
            return true;
        }

        /// <summary>
        /// Returns true if total discount percent is authorized.
        /// </summary>
        /// <param name="retail"></param>
        /// <param name="percentValue"></param>
        /// <param name="maxPercentValue"></param>
        /// <returns></returns>
        public bool AuthorizeTotalDiscountPercent(IRetailTransaction retail, decimal percentValue, decimal maxPercentValue)
        {
            return true;
        }

        /// <summary>
        /// Returns true if discount amount is authorized.
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="discountItem"></param>
        /// <param name="maximumDiscountAmt"></param>
        /// <returns></returns>
        public bool AuthorizeLineDiscountAmount(ISaleLineItem lineItem, ILineDiscountItem discountItem, decimal maximumDiscountAmt)
        {
            return true;
        }

        /// <summary>
        /// Returns true if discount percent is correct.
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="discountItem"></param>
        /// <param name="maximumDiscountPct"></param>
        /// <returns></returns>
        public bool AuthorizeLineDiscountPercent(ISaleLineItem lineItem, ILineDiscountItem discountItem, decimal maximumDiscountPct)
        {
            return true;
        }

        #endregion
    }
}
