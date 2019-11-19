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
using System.Text;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts.BusinessLogic;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Diagnostics;

namespace Microsoft.Dynamics.Retail.Pos.DiscountService
{
    /// <summary>
    /// These parameters indicate which types of AX discounts (aka trade agreement discounts, aka not Retail Periodic Discounts)
    ///  are currently activated and should be allowed on the transaction.
    /// </summary>
    public class DiscountParameters
    {
        //Line Account
        public bool SalesLineAccountItem { get; set; }
        public bool SalesLineAccountGroup { get; set; }
        public bool SalesLineAccountAll { get; set; }

        //Line Group
        public bool SalesLineGroupItem { get; set; }
        public bool SalesLineGroupGroup { get; set; }
        public bool SalesLineGroupAll { get; set; }

        //Line All
        public bool SalesLineAllItem { get; set; }
        public bool SalesLineAllGroup { get; set; }
        public bool SalesLineAllAll { get; set; }

        //MulitLine Account
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiLine")]
        public bool SalesMultiLineAccountGroup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiLine")]
        public bool SalesMultiLineAccountAll { get; set; }

        //MultiLine Group
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiLine")]
        public bool SalesMultiLineGroupGroup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiLine")]
        public bool SalesMultiLineGroupAll { get; set; }

        //MultiLine All
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiLine")]
        public bool SalesMultiLineAllGroup { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Multi"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "MultiLine")]
        public bool SalesMultiLineAllAll { get; set; }

        //Total Account
        public bool SalesEndAccountAll { get; set; }

        //Total Group
        public bool SalesEndGroupAll { get; set; }

        //Total All
        public bool SalesEndAllAll { get; set; }

        static private IUtility Utility
        {
            get { return Discount.InternalApplication.BusinessLogic.Utility; }
        }

        /// <summary>
        /// Initialize this instance with discount parameters from the database. 
        /// </summary>
        public void InitializeParameters()
        {
            this.GetDiscountParameters();
        }

        /// <summary>
        /// Get discount parameters from the database. These parameters tell what search possibilities are active.
        /// </summary>
        private void GetDiscountParameters()
        {
            string queryString = "SELECT SALESLINEACCOUNTITEM, SALESLINEACCOUNTGROUP, SALESLINEACCOUNTALL," +
                                 "SALESLINEGROUPITEM, SALESLINEGROUPGROUP, SALESLINEGROUPALL, " +
                                 "SALESLINEALLITEM, SALESLINEALLGROUP, SALESLINEALLALL, " +
                                 "SALESMULTILNACCOUNTGROUP, SALESMULTILNACCOUNTALL," +
                                 "SALESMULTILNGROUPGROUP, SALESMULTILNGROUPALL," +
                                 "SALESMULTILNALLGROUP,SALESMULTILNALLALL, " +
                                 "SALESENDACCOUNTALL, SALESENDGROUPALL, SALESENDALLALL " +
                                 "FROM PRICEPARAMETERS WHERE " +
                                 "DATAAREAID = @DATAAREAID ";

            SqlConnection connection = Discount.InternalApplication.Settings.Database.Connection;
            string dataAreaId = Discount.InternalApplication.Settings.Database.DataAreaID;

            try
            {
                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@DATAAREAID", dataAreaId);

                    if (connection.State != ConnectionState.Open) { connection.Open(); }
                    using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SingleRow))
                    {
                        reader.Read();
                        if (reader.HasRows)
                        {
                            //Line Account
                            SalesLineAccountItem = Utility.ToBool(reader["SALESLINEACCOUNTITEM"]);
                            SalesLineAccountGroup = Utility.ToBool(reader["SALESLINEACCOUNTGROUP"]);
                            SalesLineAccountAll = Utility.ToBool(reader["SALESLINEACCOUNTALL"]);

                            //Line Group
                            SalesLineGroupItem = Utility.ToBool(reader["SALESLINEGROUPITEM"]);
                            SalesLineGroupGroup = Utility.ToBool(reader["SALESLINEGROUPGROUP"]);
                            SalesLineGroupAll = Utility.ToBool(reader["SALESLINEGROUPALL"]);

                            //Line All
                            SalesLineAllItem = Utility.ToBool(reader["SALESLINEALLITEM"]);
                            SalesLineAllGroup = Utility.ToBool(reader["SALESLINEALLGROUP"]);
                            SalesLineAllAll = Utility.ToBool(reader["SALESLINEALLALL"]);

                            //MultiLine Account
                            SalesMultiLineAccountGroup = Utility.ToBool(reader["SALESMULTILNACCOUNTGROUP"]);
                            SalesMultiLineAccountAll = Utility.ToBool(reader["SALESMULTILNACCOUNTALL"]);

                            //MultiLine Group
                            SalesMultiLineGroupGroup = Utility.ToBool(reader["SALESMULTILNGROUPGROUP"]);
                            SalesMultiLineGroupAll = Utility.ToBool(reader["SALESMULTILNGROUPALL"]);

                            //MultiLine All
                            SalesMultiLineAllGroup = Utility.ToBool(reader["SALESMULTILNALLGROUP"]);
                            SalesMultiLineAllAll = Utility.ToBool(reader["SALESMULTILNALLALL"]);

                            //Total
                            SalesEndAccountAll = Utility.ToBool(reader["SALESENDACCOUNTALL"]);
                            SalesEndGroupAll = Utility.ToBool(reader["SALESENDGROUPALL"]);
                            SalesEndAllAll = Utility.ToBool(reader["SALESENDALLALL"]);
                        }
                        else
                        {
                            SalesLineAccountItem = false;
                            SalesLineAccountGroup = false;
                            SalesLineAccountAll = false;
                            SalesLineGroupItem = false;
                            SalesLineGroupGroup = false;
                            SalesLineGroupAll = false;
                            SalesLineAllItem = false;
                            SalesLineAllGroup = false;
                            SalesLineAllAll = false;
                            SalesMultiLineAccountGroup = false;
                            SalesMultiLineAccountAll = false;
                            SalesMultiLineGroupGroup = false;
                            SalesMultiLineGroupAll = false;
                            SalesMultiLineAllGroup = false;
                            SalesMultiLineAllAll = false;
                            SalesEndAccountAll = false;
                            SalesEndGroupAll = false;
                            SalesEndAllAll = false;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Technically violates CA2214 calling this.ToString().  However this instance appears to be safe.
                NetTracer.Error(ex, "DiscountParameters::GetDiscountParameters failed");
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), ex);
                throw;
            }
            finally
            {
                if (connection.State == ConnectionState.Open) { connection.Close(); }
            }
        }


        /// <summary>
        /// Returns true or false, whether a certain relation is active for a discount search.
        /// </summary>
        /// <param name="relation">The discount relation(line,multiline,total)</param>
        /// <param name="accountCode">The account code(table,group,all)</param>
        /// <param name="itemCode">The item coude(table,group,all)</param>
        /// <returns>Returns true if the relation is active, else false.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "Grandfather")]
        public bool Activation(PriceDiscType relation, PriceDiscAccountCode accountCode, PriceDiscItemCode itemCode)
        {
            switch (accountCode)
            {
                case PriceDiscAccountCode.Table:
                    switch (itemCode)
                    {
                        case PriceDiscItemCode.Table:
                            switch (relation)
                            {
                                case PriceDiscType.LineDiscSales: return SalesLineAccountItem;
                                default: return false;
                            }
                        case PriceDiscItemCode.GroupId:
                            switch (relation)
                            {
                                case PriceDiscType.LineDiscSales: return SalesLineAccountGroup;
                                case PriceDiscType.MultiLineDiscSales: return SalesMultiLineAccountGroup;
                                default: return false;
                            }
                        case PriceDiscItemCode.All:
                            switch (relation)
                            {
                                case PriceDiscType.LineDiscSales: return SalesLineAccountAll;
                                case PriceDiscType.MultiLineDiscSales: return SalesMultiLineAccountAll;
                                case PriceDiscType.EndDiscSales: return SalesEndAccountAll;
                                default: return false;
                            }
                        default:
                            NetTracer.Warning("DiscountParameters::Activation: itemCode is out of range: {0}", itemCode);
                            throw new ArgumentOutOfRangeException("itemCode");
                    }

                case PriceDiscAccountCode.GroupId:
                    switch (itemCode)
                    {
                        case PriceDiscItemCode.Table:
                            switch (relation)
                            {
                                case PriceDiscType.LineDiscSales: return SalesLineGroupItem;
                                default: return false;
                            }
                        case PriceDiscItemCode.GroupId:
                            switch (relation)
                            {
                                case PriceDiscType.LineDiscSales: return SalesLineGroupGroup;
                                case PriceDiscType.MultiLineDiscSales: return SalesMultiLineGroupGroup;
                                default: return false;
                            }
                        case PriceDiscItemCode.All:
                            switch (relation)
                            {
                                case PriceDiscType.LineDiscSales: return SalesLineGroupAll;
                                case PriceDiscType.MultiLineDiscSales: return SalesMultiLineGroupAll;
                                case PriceDiscType.EndDiscSales: return SalesEndGroupAll;
                                default: return false;
                            }
                        default:
                            NetTracer.Warning("DiscountParameters::Activation: itemCode is out of range: {0}", itemCode);
                            throw new ArgumentOutOfRangeException("itemCode");
                    }

                case PriceDiscAccountCode.All:
                    switch (itemCode)
                    {
                        case PriceDiscItemCode.Table:
                            switch (relation)
                            {
                                case PriceDiscType.LineDiscSales: return SalesLineAllItem;
                                default: return false;
                            }
                        case PriceDiscItemCode.GroupId:
                            switch (relation)
                            {
                                case PriceDiscType.LineDiscSales: return SalesLineAllGroup;
                                case PriceDiscType.MultiLineDiscSales: return SalesMultiLineAllGroup;
                                default: return false;
                            }
                        case PriceDiscItemCode.All:
                            switch (relation)
                            {
                                case PriceDiscType.LineDiscSales: return SalesLineAllAll;
                                case PriceDiscType.MultiLineDiscSales: return SalesMultiLineAllAll;
                                case PriceDiscType.EndDiscSales: return SalesEndAllAll;
                                default: return false;
                            }
                        default:
                            NetTracer.Warning("DiscountParameters::Activation: itemCode is out of range: {0}", itemCode);
                            throw new ArgumentOutOfRangeException("itemCode");
                    }
                default:
                    NetTracer.Warning("DiscountParameters::Activation: accountCode is out of range: {0}", accountCode);
                    throw new ArgumentOutOfRangeException("accountCode");
            }
        }
    }
}
