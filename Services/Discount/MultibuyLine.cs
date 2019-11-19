/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Dynamics.Retail.Diagnostics;

namespace Microsoft.Dynamics.Retail.Pos.DiscountService
{
    /// <summary>
    /// MultibuyLine associates a discount value with a quantity of items.
    ///  It is used by quantity discounts, and by using many together, the various discounts
    ///  thresholds are defined for increasing discount item quantities.
    ///  This class can also initialize itself to the applicable quantity and discount value
    ///  for a given multibuy offer and item quantity.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multibuy", Justification="Multibuy is a type of retail discount, not abbreviated or misspelled")]
    public class MultibuyLine
    {
        /// <summary>
        /// Minimum quantity of items to activate this Multibuy Line Offer
        /// </summary>
        public decimal MinQuantity { get; private set; }

        /// <summary>
        /// Discount value of the line, could be unit price or percent. Which is decided by the Offer
        ///   this line belongs to.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Pct", Justification = "Cannot change public API.")]
        public decimal UnitPriceOrDiscPct { get; private set; }

        private MultibuyLine() { }

        /// <summary>
        /// Construct instance of MultibuyLine value type with given values
        /// </summary>
        /// <param name="qualifyingQuantity">Number of items to activate this discount</param>
        /// <param name="discountValue">Value of the discount (unit price of percent, based on offer header method)</param>
        public MultibuyLine(decimal qualifyingQuantity, decimal discountValue)
        {
            this.MinQuantity = qualifyingQuantity;
            this.UnitPriceOrDiscPct = discountValue;
        }

        /// <summary>
        /// Construct instance of MultibuyLine for the given Offer and item quantity. This will find the first
        ///   multibuy line for the offer whose quantity meets but does not exceed the given quantity and return it.
        /// </summary>
        /// <param name="offerId">Id of the Offer being attempted</param>
        /// <param name="quantity">Number of items for the offer</param>
        /// <returns></returns>
        public static MultibuyLine FetchForOfferAndQuantity(string offerId, decimal quantity)
        {
            return GetMultiBuyDiscountLine(offerId, quantity);
        }

        /// <summary>
        /// Construct default empty MultibuyLine object
        /// </summary>
        /// <returns></returns>
        public static MultibuyLine Empty()
        {
            return new MultibuyLine(0, 0);
        }

        static private MultibuyLine GetMultiBuyDiscountLine(string offerId, decimal quantity)
        {
            SqlConnection connection = Discount.InternalApplication.Settings.Database.Connection;
            string dataAreaId = Discount.InternalApplication.Settings.Database.DataAreaID;

            try
            {
                offerId = offerId ?? String.Empty;

                string queryString = @"
                                    SELECT * FROM POSMULTIBUYDISCOUNTLINE 
                                    WHERE MINQUANTITY = (SELECT MAX(MINQUANTITY) 
                                    FROM POSMULTIBUYDISCOUNTLINE 
                                    WHERE MINQUANTITY <= @MINQUANTITY AND OFFERID = @OFFERID AND DATAAREAID = @DATAAREAID) 
                                    AND DATAAREAID = @DATAAREAID AND OFFERID = @OFFERID";

                using (SqlCommand command = new SqlCommand(queryString.ToString(), connection))
                {
                    command.Parameters.AddWithValue("@MINQUANTITY", quantity);
                    command.Parameters.AddWithValue("@DATAAREAID", dataAreaId);
                    command.Parameters.AddWithValue("@OFFERID", offerId);

                    MultibuyLine multiBuyLine = null;

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        if (reader.Read())
                        {
                            multiBuyLine = new MultibuyLine((decimal)reader["MINQUANTITY"], (decimal)reader["UNITPRICEORDISCPCT"]);
                        }
                    }

                    return multiBuyLine ?? MultibuyLine.Empty();
                }
            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "MultibuyLine::GetMultiBuyDiscountLine failed for offerId {0} quantity {1}", offerId, quantity);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(typeof(MultibuyLine).ToString(), ex);
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }
        }

    }
}
