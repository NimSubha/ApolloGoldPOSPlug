/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Linq;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.ShtrihPrinter
{
    /// <summary>
    /// Loyalty manager class contains the logic for calculating the loyalty balance and amounts.
    /// </summary>          
    public sealed class LoyaltyManager
    {
        /// <summary>
        /// Loyalty points added
        /// </summary>
        public decimal LoyaltyPointsAdded { get; private set; }

        /// <summary>
        /// Loyalty points used
        /// </summary>
        public decimal LoyaltyPointsUsed { get; private set; }

        /// <summary>
        /// Loyalty points balance
        /// </summary>
        public decimal LoyaltyBalance { get; private set; }

        /// <summary>
        /// Loyalty paid amount
        /// </summary>
        public decimal LoyaltyAmount { get; private set; }

        /// <summary>
        /// Loyalty paid percent
        /// </summary>
        public decimal LoyaltyPercent { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="LoyaltyManager"/> class.
        /// </summary>
        /// <param name="transaction">Retail transaction</param>
        public LoyaltyManager(RetailTransaction transaction)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException("transaction");
            }

            if (transaction.LoyaltyItem.UsageType == LSRetailPosis.Transaction.Line.LoyaltyItem.LoyaltyItemUsageType.UsedForLoyaltyDiscount)
            {
                LoyaltyPointsAdded = decimal.Zero;
                LoyaltyPointsUsed = transaction.LoyaltyItem.CalculatedLoyaltyPoints;
                LoyaltyAmount = transaction.LoyaltyDiscount;
                LoyaltyPercent = (transaction.NetAmountWithTaxAndCharges != decimal.Zero) ? (LoyaltyAmount / (transaction.NetAmountWithTaxAndCharges + LoyaltyAmount) * 100m) : decimal.Zero;
            }
            else
            {
                var tenderLines = transaction.TenderLines.Where(t => t.Voided == false).OfType<ILoyaltyTenderLineItem>();
                LoyaltyPointsAdded = transaction.LoyaltyItem.CalculatedLoyaltyPoints;
                LoyaltyPointsUsed = tenderLines.Sum(l => l.LoyaltyPoints);
                LoyaltyAmount = tenderLines.Sum(l => l.Amount);
                LoyaltyPercent = (transaction.NetAmountWithTaxAndCharges != decimal.Zero) ? (LoyaltyAmount / transaction.NetAmountWithTaxAndCharges * 100m) : decimal.Zero;
            }

            LoyaltyBalance = transaction.LoyaltyItem.AccumulatedLoyaltyPoints + LoyaltyPointsAdded + LoyaltyPointsUsed;
        }
    }
}
