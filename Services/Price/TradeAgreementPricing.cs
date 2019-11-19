/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using DE = Microsoft.Dynamics.Retail.Pos.DataEntity;
using LSRetailPosis.Settings.FunctionalityProfiles;

namespace Microsoft.Dynamics.Retail.Pos.PriceService
{
    static class TradeAgreementPricing
    {
        /// <summary>
        /// This function retrieves all possible price agreements for the given args (item, customer, currency, etc.),
        ///   item relation code (item/group/all), and account relation code (customer/price group/all).
        /// </summary>
        /// <param name="args">Arguments for price agreement search</param>
        /// <param name="itemCode">Item relation code to look for (in order: item/group/all)</param>
        /// <param name="accountCode">Account relation code to look for (in order: customer/price group/all)</param>
        /// <returns>Collection of applicable price agreements, sorted by price amount ascending</returns>
        static private IEnumerable<DE.PriceDiscTable> FindPriceAgreements(PriceAgreementArgs args, PriceDiscItemCode itemCode, PriceDiscAccountCode accountCode)
        {
            List<DE.PriceDiscTable> tradeAgreements = new List<DE.PriceDiscTable>();
            string itemRelation = args.GetItemRelation(itemCode);
            IList<string> accountRelations = args.GetAccountRelations(accountCode);
            string unitId = args.GetUnitId(itemCode);
            DateTime today = DateTime.Now.Date;

            SqlConnection connection = Price.InternalApplication.Settings.Database.Connection;
            string dataAreaId = Price.InternalApplication.Settings.Database.DataAreaID;

            bool isIndia = Functions.CountryRegion == SupportedCountryRegion.IN;

            try
            {
                // convert account relations list to data-table for use as TVP in the query.
                string queryString = @"
                        SELECT 
                            ta.PRICEUNIT,
                            ta.ALLOCATEMARKUP,
                            ta.AMOUNT,
                            ta.SEARCHAGAIN"
                            + (isIndia ? ", ta.MAXIMUMRETAILPRICE_IN" : string.Empty)
                            + @"
                        FROM 
                            PRICEDISCTABLE ta LEFT JOIN 
                                INVENTDIM invdim ON ta.INVENTDIMID = invdim.INVENTDIMID AND ta.DATAAREAID = invdim.DATAAREAID
                        WHERE
                            ta.RELATION = 4
                            AND ta.ITEMCODE = @ITEMCODE
                            AND ta.ITEMRELATION = @ITEMRELATION
                            AND ta.ACCOUNTCODE = @ACCOUNTCODE
                    
                            -- USES Tvp: CREATE TYPE FINDPRICEAGREEMENT_ACCOUNTRELATIONS_TABLETYPE AS TABLE(ACCOUNTRELATION nvarchar(20) NOT NULL);
                            AND (ta.ACCOUNTRELATION) IN (SELECT ar.ACCOUNTRELATION FROM @ACCOUNTRELATIONS ar)

                            AND ta.CURRENCY = @CURRENCYCODE
                            AND ta.UNITID = @UNITID
                            AND ta.QUANTITYAMOUNTFROM <= abs(@QUANTITY)
                            AND (ta.QUANTITYAMOUNTTO >= abs(@QUANTITY) OR ta.QUANTITYAMOUNTTO = 0)
                            AND ta.DATAAREAID = @DATAAREAID
                            AND ((ta.FROMDATE <= @TODAY OR ta.FROMDATE <= @NODATE)
                                    AND (ta.TODATE >= @TODAY OR ta.TODATE <= @NODATE))
                            AND (invdim.INVENTCOLORID in (@COLORID, ''))
                            AND (invdim.INVENTSIZEID in (@SIZEID,''))
                            AND (invdim.INVENTSTYLEID in (@STYLEID, ''))
                            AND (invdim.CONFIGID in (@CONFIGID, ''))
                            -- INCLUDE SERIAL ID
                            AND (invdim.INVENTSERIALID in (@SERIALID,''))
                            AND (invdim.INVENTLOCATIONID in (@INVENTLOCATIONID, ''))
                            AND (invdim.INVENTSITEID in ((SELECT il.INVENTSITEID FROM INVENTLOCATION il WHERE il.INVENTLOCATIONID = @INVENTLOCATIONID), ''))

                            --// ORDERBY CLAUSE MUST MATCH THAT IN AX TO ENSURE COMPATIBLE PRICING BEHAVIOR.
                            --// SEE THE CLASS PRICEDISC.FINDPRICEAGREEMENT() AND TABLE PRICEDISCTABLE.PRICEDISCIDX
                            order by ta.AMOUNT, ta.QUANTITYAMOUNTFROM, ta.QUANTITYAMOUNTTO, ta.FROMDATE";

                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    command.Parameters.AddWithValue("@DATAAREAID", dataAreaId);
                    command.Parameters.AddWithValue("@ITEMCODE", itemCode);
                    command.Parameters.AddWithValue("@ITEMRELATION", itemRelation);
                    command.Parameters.AddWithValue("@ACCOUNTCODE", accountCode);
                    command.Parameters.AddWithValue("@UNITID", unitId);
                    command.Parameters.AddWithValue("@CURRENCYCODE", args.CurrencyCode);
                    command.Parameters.AddWithValue("@QUANTITY", args.Quantity);
                    command.Parameters.AddWithValue("@COLORID", (args.Dimensions.ColorId) ?? string.Empty);
                    command.Parameters.AddWithValue("@SIZEID", (args.Dimensions.SizeId) ?? string.Empty);
                    command.Parameters.AddWithValue("@STYLEID", (args.Dimensions.StyleId) ?? string.Empty);
                    command.Parameters.AddWithValue("@CONFIGID", (args.Dimensions.ConfigId) ?? string.Empty);
                    command.Parameters.AddWithValue("@INVENTLOCATIONID", (LSRetailPosis.Settings.ApplicationSettings.Terminal.InventLocationId) ?? string.Empty);
                    command.Parameters.AddWithValue("@TODAY", today);
                    command.Parameters.AddWithValue("@NODATE", DateTime.Parse("1900-01-01"));
                    // Including Serial ID now
                    command.Parameters.AddWithValue("@SERIALID", (args.SerialId) ?? string.Empty);

                    // Fill out TVP for account relations list
                    using (DataTable accountRelationsTable = new DataTable())
                    {
                        accountRelationsTable.Columns.Add("ACCOUNTRELATION", typeof(string));
                        foreach (string relation in accountRelations)
                        {
                            accountRelationsTable.Rows.Add(relation);
                        }

                        SqlParameter param = command.Parameters.Add("@ACCOUNTRELATIONS", SqlDbType.Structured);
                        param.Direction = ParameterDirection.Input;
                        param.TypeName = "FINDPRICEAGREEMENT_ACCOUNTRELATIONS_TABLETYPE";
                        param.Value = accountRelationsTable;

                        if (connection.State != ConnectionState.Open)
                        {
                            connection.Open();
                        }

                        SqlDataReader reader = command.ExecuteReader();
                        while (reader.Read())
                        {
                            DE.PriceDiscTable pdt = new DE.PriceDiscTable()
                            {
                                PriceUnit = reader.GetDecimal(reader.GetOrdinal("PRICEUNIT")),
                                ShouldAllocateMarkup = reader.GetInt32(reader.GetOrdinal("ALLOCATEMARKUP")),
                                Amount = reader.GetDecimal(reader.GetOrdinal("AMOUNT")),
                                ShouldSearchAgain = reader.GetInt32(reader.GetOrdinal("SEARCHAGAIN")),
                                IndiaMRP = (isIndia) ? reader.GetDecimal(reader.GetOrdinal("MAXIMUMRETAILPRICE_IN")) : Decimal.Zero
                            };
                            tradeAgreements.Add(pdt);
                        }
                    }
                }
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
            }

            return tradeAgreements;
        }

        /// <summary>
        /// This function searches a list of trade agreements (assumed to be sorted with lowest prices first).
        ///   It calculates the price for each trade agreement, returning the lowest amount, and optionally stopping
        ///   early if it encounters a trade agreement with SearchAgain=False.
        /// </summary>
        /// <param name="tradeAgreements">List of price agreements, sorted by Amount ascending</param>
        /// <param name="searchAgain">Out parameter indicating whether SearchAgain=False was hit</param>
        /// <returns>Best price agreement price for the given list of trade agreements</returns>
        private static PriceResult GetBestPriceAgreement(IEnumerable<DE.PriceDiscTable> tradeAgreements, out bool searchAgain)
        {
            bool isIndia = Functions.CountryRegion == SupportedCountryRegion.IN;

            Decimal price = 0;
            Decimal indiaMRP = 0;
            searchAgain = true;
            foreach (var ta in tradeAgreements)
            {
                Decimal priceUnit = (ta.PriceUnit != 0) ? ta.PriceUnit : 1;
                Decimal markup = (ta.ShouldAllocateMarkup != 0) ? ta.Markup : 0;
                Decimal currentPrice = (ta.Amount / priceUnit) + markup;

                if ((price == 0M) || (currentPrice != 0M && price > currentPrice))
                {
                    price = currentPrice;

                    if (isIndia)
                    {
                        indiaMRP = ta.IndiaMRP;
                    }
                }

                if (ta.ShouldSearchAgain == 0)
                {
                    searchAgain = false;
                    break;
                }
            }

            PriceResult result = new PriceResult(price, PriceGroupIncludesTax.NotSpecified);

            if (isIndia)
            {
                result.IndiaMRP = indiaMRP;
            }

            return result;
        }

        /// <summary>
        /// This function takes arguments (customer, item, currency, etc.) related to price (trade) agreement
        ///  as well as the set of currently enabled trade agreement types. It returns the best trade agreement
        ///  price for the given constraints.
        ///  
        /// As in AX, the method searches for a price on the given item which has been given to a
        ///  customer, price group, or anyone (in given precedence order). If a price is found and marked as
        ///  SearchAgain=False, the search will terminate. Otherwise, search for lowest price will continue.
        ///  
        /// To recap, the logic is that three searches are done for customer, price group, and all, each bracket
        ///   will return the lowest price it has for the constraints. If it has SearchAgain=True, then the search
        ///   for lowest price continues to the next bracket.
        /// </summary>
        /// <param name="args">Arguments for price agreement search</param>
        /// <param name="priceParameters">Set of enabled price agreement types</param>
        /// <returns>Most applicable price for the given price agreement constraints.</returns>        
        public static PriceResult priceAgr(PriceAgreementArgs args, PriceParameters priceParameters)
        {
            PriceResult priceResult = new PriceResult(0M, PriceGroupIncludesTax.NotSpecified);

            for (int idx = 0; idx < 9; idx++)
            {
                // Enum values for ItemCode/AccountCode:  0=Table, 1=Group, 2=All
                PriceDiscItemCode itemCode = (PriceDiscItemCode)(idx % 3);    //Mod divsion
                PriceDiscAccountCode accountCode = (PriceDiscAccountCode)(idx / 3); //three possible item-/account-Codes, as described in the ENUMs.

                if (priceParameters.IsRelationActive(accountCode, itemCode))
                {
                    IList<string> accountRelations = args.GetAccountRelations(accountCode);
                    string itemRelation = args.GetItemRelation(itemCode);

                    if (accountRelations.All(a => ValidRelation(accountCode, a)) &&
                        (ValidRelation(itemCode, itemRelation)))
                    {
                        bool searchAgain;
                        PriceResult currentPriceResult = GetBestPriceAgreement(FindPriceAgreements(args, itemCode, accountCode), out searchAgain);

                        if (priceResult.Price == 0M ||
                            (currentPriceResult.Price > 0M && currentPriceResult.Price < priceResult.Price))
                        {
                            priceResult = currentPriceResult;
                        }

                        if (!searchAgain)
                        {
                            break;
                        }
                    }
                }
            }

            return priceResult;
        }

        /// <summary>
        /// Is there a valid relation between the itemcode and relation?
        /// </summary>
        /// <param name="itemCode"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        private static bool ValidRelation(PriceDiscItemCode itemCode, string relation)
        {
            bool ok = true;

            if (!string.IsNullOrEmpty(relation) && (itemCode == PriceDiscItemCode.All))
            {
                ok = false;
            }

            if (string.IsNullOrEmpty(relation) && (itemCode != PriceDiscItemCode.All))
            {
                ok = false;
            }

            return ok;
        }

        /// <summary>
        /// Is there a valid relation between the accountcode and relation?
        /// </summary>
        /// <param name="accountCode"></param>
        /// <param name="relation"></param>
        /// <returns></returns>
        private static bool ValidRelation(PriceDiscAccountCode accountCode, string relation)
        {
            bool ok = true;

            if (!string.IsNullOrEmpty(relation) && (accountCode == PriceDiscAccountCode.All))
            {
                ok = false;
            }

            if (string.IsNullOrEmpty(relation) && (accountCode != PriceDiscAccountCode.All))
            {
                ok = false;
            }

            return ok;
        }
    }
}
