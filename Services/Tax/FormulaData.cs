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
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Diagnostics;

namespace Microsoft.Dynamics.Retail.Pos.Tax
{
	/// <summary>
	/// Handles data access to the FormulaDesigner_IN table.
	/// </summary>
	public static class FormulaData
	{
		static public SqlConnection Connection
		{
			get { return ApplicationSettings.Database.LocalConnection; }
		}

		/// <summary>
		/// Read the formula from the DB for the given group and code.
		/// </summary>
		/// <param name="taxGroup"></param>
		/// <param name="taxCode"></param>
		/// <returns></returns>
		public static Formula GetFormula(string taxGroup, string taxCode)
		{
			// Default value for the formula
			Formula formula = new Formula
			{
				TaxableBasis = TaxableBases.LineAmount,
				PriceIncludesTax = false,
				Id = 0, // Since Id's are always greater than 0 in AX, this 0 value indicates formula was not found.
				CalculationExpression = String.Empty
			};

			try
			{
				using (SqlCommand command = new SqlCommand())
				{
					command.Connection = Connection;
					command.CommandText = @"SELECT TAXABLEBASIS, PRICEINCLTAX, ID, CALCEXP1 
											FROM dbo.FORMULADESIGNER_IN	
											WHERE TAXITEMGROUP = @TaxItemGroup AND TAXCODE = @TaxCode AND DATAAREAID = @DataAreaID";

					command.Parameters.AddWithValue("@TaxItemGroup", taxGroup);
					command.Parameters.AddWithValue("@TaxCode", taxCode);
					command.Parameters.AddWithValue("@DataAreaID", ApplicationSettings.Database.DATAAREAID);

					if (Connection.State != ConnectionState.Open) { Connection.Open(); }
					using (SqlDataReader reader = command.ExecuteReader())
					{
                        if (reader.Read())
                        {
                            formula.TaxableBasis = (TaxableBases)reader["TAXABLEBASIS"];
                            formula.PriceIncludesTax = (int)reader["PRICEINCLTAX"] == 1;
                            formula.Id = (int)reader["ID"];
                            formula.CalculationExpression = (string)reader["CALCEXP1"];
                        }
                        else
                        {
                            NetTracer.Warning("GetFormula: Formula set to default. Unable to obtain formula from DB. No rows present for taxGroup: {0}; taxCode: {1}; DataAreaID: {2}", taxGroup, taxCode, ApplicationSettings.Database.DATAAREAID);
                        }
					}
				}
			}
			catch (Exception ex)
			{
                NetTracer.Warning(ex, "GetFormula: Formula set to default. Unable to obtain formula from DB due to an exception. taxGroup: {0}; taxCode: {1}", taxGroup, taxCode);
				LSRetailPosis.ApplicationExceptionHandler.HandleException("FormulaData", ex);
				// Not necessary to rethrow because formula is set to default state.
			}
			finally
			{
				if (Connection.State == ConnectionState.Open)
				{
					Connection.Close();
				}
			}
			return formula;
		}
	}
}
