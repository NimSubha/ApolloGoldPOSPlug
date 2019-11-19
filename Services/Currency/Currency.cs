/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using LSRetailPosis.Settings;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.DataManager;

namespace Microsoft.Dynamics.Retail.Pos.Currency
{
	[Export(typeof(ICurrency))]
	public class Currency : ICurrency
	{
		[Import]
		public IApplication Application { get; set; }

		#region ICurrency Members

		/// <summary>
		/// Returns the latest exchange rate for a given currency code.
		/// If the currency code is DKK, the default currency EUR and the exchange rate is 744.50 then 
		/// for 100 EUR you will get 744.50 DKK.
		/// Axapta stores exchange rate for 100 unit of each currency.
		/// </summary>
		/// <param name="currencyCode">The currency name,i.e. USD, DKK, EUR</param>
		/// <returns>The currency rate</returns>
		public decimal ExchangeRate(string currencyCode)
		{
			decimal exchangeRate = 1.00M; //Default exchange rate value if the company currency and the store currency are the same.
			if (ApplicationSettings.Terminal.CompanyCurrency != currencyCode)
			{
				exchangeRate = GetExchangeRate(ApplicationSettings.Terminal.CompanyCurrency, currencyCode);
			}

			return exchangeRate;
		}

		/// <summary>
		/// Returns the valid exchange rate for a given pair of currencies.
		/// </summary>
		/// <param name="fromCurrency">From currency.</param>
		/// <param name="toCurrency">To currency.</param>
		/// <returns>The exchange rate</returns>
		/// <remarks></remarks>
		public decimal GetExchangeRate(string fromCurrency, string toCurrency)
		{
			decimal exchangeRate = 0.00M;

			// Check wheter currency code in parameters are whitespaces or null.
			if (string.IsNullOrWhiteSpace(fromCurrency))
			{
				throw new ArgumentNullException("fromCurrency");
			}

			if (string.IsNullOrWhiteSpace(toCurrency))
			{
				throw new ArgumentNullException("toCurrency");
			}

			// If from and to currency is the same one return 1.0M
			if (string.Equals(fromCurrency.Trim(), toCurrency.Trim(), StringComparison.OrdinalIgnoreCase))
			{
				return 1.0M;
			}

			//Exchange Rate Data manager
			ExchangeRateDataManager exchangeRateData = new ExchangeRateDataManager(
				ApplicationSettings.Database.LocalConnection,
				ApplicationSettings.Database.DATAAREAID);

			//Gets valid Exchange Rate Type for POS
			Int64 exchangeRateType = ApplicationSettings.Terminal.ExchangeRateType;
			exchangeRate = exchangeRateData.GetExchangeRate(fromCurrency, toCurrency, exchangeRateType);
			return exchangeRate / 100;
		}

		/// <summary>
		/// Gets detailed currency information as per passed currency code.
		/// </summary>
		/// <param name="currencyCode"></param>
		/// <returns></returns>
		public ICurrencyInfo DetailedCurrencyInfo(string currencyCode)
		{
			ICurrencyInfo currInfo = this.Application.BusinessLogic.Utility.CreateCurrencyInfo();
			currInfo.PosCurrencyRate = GetExchangeRate(ApplicationSettings.Terminal.StoreCurrency, currencyCode);
			string queryString = "SELECT AMOUNT, TYPE, CURRENCY FROM RETAILSTORECASHDECLARATIONTABLE WHERE "
				+ "DATAAREAID = @DATAAREAID AND CURRENCY = @CURRENCYCODE AND STOREID = @STOREID ORDER BY AMOUNT";

			List<ICurrencyItemInfo> currencyItemInfoList = new List<ICurrencyItemInfo>();

			SqlConnection connection = Application.Settings.Database.Connection;

			try
			{
				using (SqlCommand command = new SqlCommand(queryString, connection))
				{
					SqlParameter dataAreaIdParm = command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
					dataAreaIdParm.Value = Application.Settings.Database.DataAreaID;
					SqlParameter currencyCodeParm = command.Parameters.Add("@CURRENCYCODE", SqlDbType.NVarChar, 3);
					currencyCodeParm.Value = currencyCode;
					SqlParameter storeIDParm = command.Parameters.Add("@STOREID", SqlDbType.NVarChar, 10);
					storeIDParm.Value = ApplicationSettings.Terminal.StoreId;

					if (connection.State != ConnectionState.Open)
					{
						connection.Open();
					}

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
					{
						while (reader.Read())
						{
							ICurrencyItemInfo currItem = this.Application.BusinessLogic.Utility.CreateCurrencyItemInfo();
							currItem.CurrValue = reader.GetDecimal(reader.GetOrdinal("AMOUNT"));
							currItem.CurrType = (CurrType)reader.GetInt32(reader.GetOrdinal("TYPE"));
							currencyItemInfoList.Add(currItem);
						}

						currInfo.CurrencyItems = currencyItemInfoList.ToArray();
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

			return currInfo;
		}

		/// <summary>
		/// Gets all the currecy codes as per passed parameter.
		/// </summary>
		/// <param name="localCurrencyCode"></param>
		/// <param name="usedCurrencyCode"></param>
		/// <param name="currTypes"></param>
		[SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Grandfather")]
		public void GetAllCurrTypes(string localCurrencyCode, string usedCurrencyCode, ref ArrayList currTypes)
		{
			string queryString = "SELECT CURRENCYCODE FROM CURRENCY ORDER BY CURRENCYCODE";
			SqlConnection connection = Application.Settings.Database.Connection;

			try
			{
				using (SqlCommand command = new SqlCommand(queryString, connection))
				{
					if (connection.State != ConnectionState.Open)
					{
						connection.Open();
					}

					using (SqlDataReader reader = command.ExecuteReader(CommandBehavior.SequentialAccess))
					{
						while (reader.Read())
						{
							string currCode = reader.GetString(reader.GetOrdinal("CURRENCYCODE"));
							if (currCode != localCurrencyCode)
							{
								currTypes.Add(currCode);
							}
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
		}

		/// <summary>
		/// Converts a value from one currency to another.
		/// </summary>
		/// <param name="fromCurrencyCode">The currency which the orgValue is in.</param>
		/// <param name="toCurrencyCode">The currency to which to convert the orgValue to.</param>
		/// <param name="orgValue">The value to be converted.</param>
		/// <returns>The value as it is after conversion in the toCurrencyCode rounded according to the toCurrencyCode rounding setup.</returns>
		public decimal CurrencyToCurrency(string fromCurrencyCode, string toCurrencyCode, decimal orgValue)
		{
			if (fromCurrencyCode == null)
			{
				throw new ArgumentNullException("fromCurrencyCode");
			}

			if (toCurrencyCode == null)
			{
				throw new ArgumentNullException("toCurrencyCode");
			}

			try
			{
				// If from and to currency is the same one return the original value
				if (fromCurrencyCode.Trim() == toCurrencyCode.Trim())
				{
					return this.Application.Services.Rounding.Round(orgValue, fromCurrencyCode);
				}

				// If the value to be converted is 0 then just return 0
				if (orgValue == 0M)
				{
					return orgValue;
				}

				decimal toExchRate = GetExchangeRate(fromCurrencyCode.Trim(), toCurrencyCode.Trim());

				// try a cross exchange rate
				if (toExchRate == 0M)
				{
					// From -> Store
					decimal currencyExchangeRate = GetExchangeRate(fromCurrencyCode.Trim(), ApplicationSettings.Terminal.StoreCurrency);

					// Store -> To
					decimal storeExchangeRate = GetExchangeRate(ApplicationSettings.Terminal.StoreCurrency, toCurrencyCode.Trim());

					// From -> Store -> To
					toExchRate = (currencyExchangeRate * storeExchangeRate);
				}

				if (toExchRate > 0M)
				{
					return this.Application.Services.Rounding.Round(orgValue * toExchRate, toCurrencyCode);
				}

				return 0M;
			}
			catch (Exception)
			{
				throw;
			}
		}

		#endregion
	}
}
