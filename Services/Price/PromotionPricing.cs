/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using LSRetailPosis.Transaction.Line.SaleItem;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Diagnostics;

namespace Microsoft.Dynamics.Retail.Pos.PriceService
{
    public static class PromotionPricing
    {
        /// <summary>
        /// Given a set of promotion lines, tentative item price, and item, calculate the price after promotions are applied
        /// </summary>
        /// <param name="promotionLines">List of promotion configurations active for this item</param>
        /// <param name="price">Price of the item before the promotion, derived from trade agreement or base item price</param>
        /// <param name="saleItem">The sale item whose price is being determined</param>
        /// <returns>Unrounded price after applying all promotions</returns>
        public static decimal CalculatePromotionPrice(IEnumerable<PromotionInfo> promotionLines, decimal price, BaseSaleItem saleItem)
        {
            if (saleItem == null)
            {
                NetTracer.Warning("saleItem parameter is null");
                throw new ArgumentNullException("saleItem");
            }

            if (promotionLines == null || promotionLines.Count() == 0)
            {
                return price;
            }

            decimal promoPrice = price;
            IList<PromoPrice> promoPrices = new List<PromoPrice>();

            foreach (PromotionInfo promo in promotionLines)
            {
                PromoPrice promoLine = new PromoPrice();
                promoLine.PromoId = promo.PromoId;
                promoLine.Concurrency = promo.Concurrency;
                promoLine.IsCompoundable = false;

                switch (promo.DiscountMethod)
                {
                    case DiscountMethod.DiscountPercent:
                        promoLine.PercentOff = promo.MaxDiscPct;
                        promoLine.IsCompoundable = true;
                        promoPrices.Add(promoLine);
                        break;

                    case DiscountMethod.OfferPrice:
                        if (!saleItem.Transaction.TaxIncludedInPrice)
                        {
                            promoLine.AmountOff = price - promo.Price;
                            promoPrices.Add(promoLine);
                        }
                        break;

                    case DiscountMethod.OfferPriceInclTax:
                        if (saleItem.Transaction.TaxIncludedInPrice)
                        {
                            promoLine.AmountOff = price - promo.PriceInclTax;
                            promoPrices.Add(promoLine);
                        }
                        break;

                    case DiscountMethod.DiscountAmount:
                        promoLine.AmountOff = promo.MaxDiscAmount;
                        promoLine.IsCompoundable = true;
                        promoPrices.Add(promoLine);
                        break;
                }
            }

            promoPrice = price - FindConcurrentPromoAmount(promoPrices, price);

            return promoPrice;
        }

        /// <summary>
        /// Struct to hold related fields for defining a promotion price. These are 
        ///  processed by the promotion concurrency code
        /// </summary>
        private struct PromoPrice
        {
            /// <summary>
            /// Offer Id of the promotion, used to sort compounded promos
            /// </summary>
            public String PromoId;

            /// <summary>
            /// Amount off given by the promotion
            /// </summary>
            public Decimal AmountOff;

            /// <summary>
            /// Percent off given by the promotion
            /// </summary>
            public Decimal PercentOff;

            /// <summary>
            /// Concurrency setting of the promotion
            /// </summary>
            public ConcurrencyMode Concurrency;

            /// <summary>
            /// Whether the current promotion can be compounded. If this was derived from an offer price, 
            ///   it can not be compounded, but the best can be chosen before compounding more amounts and percents
            /// </summary>
            public bool IsCompoundable;
        }

        /// <summary>
        /// Find the total promotion discount amount taken off the item price
        /// </summary>
        /// <param name="promoPrices">List of promotion prices to consider</param>
        /// <param name="price">Tentative price of the item before promotions are applied</param>
        /// <returns></returns>
        private static Decimal FindConcurrentPromoAmount(IEnumerable<PromoPrice> promoPrices, Decimal price)
        {
            Func<PromoPrice, Decimal> overallPromotionAmount = 
                (p => Math.Max(p.AmountOff, p.PercentOff * price / 100));

            IList<PromoPrice> activePromotions = new List<PromoPrice>();
            Decimal totalPromoAmount = 0;

            if (promoPrices.Count() != 0)
            {
                // if there are exclusive promotions, take only the best exclusive promotion
                var exclusivePromos = promoPrices.Where(p => p.Concurrency == ConcurrencyMode.Exclusive).ToList();
                if (exclusivePromos.Count > 0)
                {
                    activePromotions = exclusivePromos.OrderByDescending(overallPromotionAmount).Take(1).ToList();
                }
                // if there are best price promotions, find only the best price from all discounts (i.e. from best price and compounded)
                else if (promoPrices.Any(p => p.Concurrency == ConcurrencyMode.BestPrice))
                {
                    activePromotions = promoPrices.OrderByDescending(overallPromotionAmount).Take(1).ToList();
                }
                // otherwise, apply compounded promotions, taking the best non-compoundable promotion first
                else
                {
                    activePromotions = promoPrices.Where(p => !p.IsCompoundable)
                        .OrderByDescending(overallPromotionAmount).Take(1)
                        .Concat(promoPrices.Where(p => p.IsCompoundable)).ToList();
                }

                // combine all active promotions
                totalPromoAmount = CombinePromotionPrices(activePromotions, price);
            }
            else
            {
                NetTracer.Warning("PromotionPricing::FindConcurrentPromoAmount: promoPrices count is zero");
            }

            return totalPromoAmount;
        }

        /// <summary>
        /// Given a list of promotion prices, crunch them down into a single discounted amount
        /// </summary>
        /// <param name="promoPrices">Prices to crunch</param>
        /// <param name="price">Tentative item price before promotions</param>
        /// <returns></returns>
        private static Decimal CombinePromotionPrices(IEnumerable<PromoPrice> promoPrices, Decimal price)
        {
            Decimal totalAmount = 0;

            // only first non-compoundable promotion will be applied
            if (promoPrices.Any(p => !p.IsCompoundable))
            {
                PromoPrice nonCompoundable = promoPrices.Where(p => !p.IsCompoundable).First();
                totalAmount = Math.Max(nonCompoundable.AmountOff, nonCompoundable.PercentOff * price / 100);
            }

            // apply compoundable promotions in order of offer Id
            foreach(var promo in promoPrices.Where(p => p.IsCompoundable).OrderBy(p => p.PromoId))
            {
                // otherwise add the amount or percent to the total amount discounted
                if (promo.AmountOff != 0)
                {
                    totalAmount += promo.AmountOff;
                }
                else if (promo.PercentOff != 0)
                {
                    totalAmount += (price - totalAmount) * promo.PercentOff / 100;
                }
            }

            // don't discount more than the price
            if (totalAmount > price)
            {
                totalAmount = price;
            }

            return totalAmount;
        }
    }
}
