/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Windows.Forms;
using LSRetailPosis.Settings.FunctionalityProfiles;
using LSRetailPosis.Transaction;
using LSRetailPosis.Transaction.Line.SaleItem;
using LSRetailPosis.Transaction.Line.Tax;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.Tax;
using Microsoft.Dynamics.Retail.Pos.Tax.GenericTaxEngine;

namespace Microsoft.Dynamics.Retail.Pos.TaxService
{
    enum ModuleType
    {
        Sales = 2
    };

    [Export(typeof(ITax))]
    public class Tax : ITax, IInitializeable
    {

        #region Properties

        /// <summary>
        /// IApplication instance.
        /// </summary>
        private IApplication application;

        private TaxEngineServiceProxy taxEngineServiceProxy;

        /// <summary>
        /// Gets or sets the IApplication instance.
        /// </summary>
        [Import]
        public IApplication Application
        {
            get
            {
                return this.application;
            }
            set
            {
                this.application = value;
                InternalApplication = value;
            }
        }

        /// <summary>
        /// Gets or sets the static IApplication instance.
        /// </summary>
        internal static IApplication InternalApplication { get; private set; }

        private List<ITaxProvider> Providers { get; set; }

        #endregion

        #region IInitializeable

        /// <summary>
        /// Initialize Tax service.
        /// </summary>
        public void Initialize()
        {
            this.LoadProviders();

            taxEngineServiceProxy = new TaxEngineServiceProxy(Application);
        }

        public void Initialize(IApplication nimApplication)
        {
            this.LoadProviders();

            taxEngineServiceProxy = new TaxEngineServiceProxy(nimApplication);
        }

        /// <summary>
        /// Uninitialize Tax service.
        /// </summary>
        public void Uninitialize()
        {
        }

        #endregion

        #region ITax

        /// <summary>
        /// Calculates the tax for the last item.
        /// </summary>
        /// <param name="retailTransaction">The transaction to be calculated</param>
        public void CalculateTax(IRetailTransaction retailTransaction)
        {
            RetailTransaction transaction = retailTransaction as RetailTransaction;
            if (transaction == null)
            {
                NetTracer.Error("Argument retailTransaction is null");
                throw new ArgumentNullException("retailTransaction");
            }
            foreach (ISaleLineItem saleItem in transaction.SaleItems)
            {
                saleItem.TaxRatePct = 0;
                saleItem.TaxLines.Clear();
                saleItem.TaxMeasures.Clear();
                saleItem.CalculateLine();
            }

            ClearMiscChargeTaxLines(transaction);

            if (TaxEngineServiceProxy.IsGTECalculationRequired(transaction))
            {
                taxEngineServiceProxy.CalculateTax(transaction);
                transaction.TaxCalculationType = TaxCalculationType.GTE;
            }
            else
            {
                foreach (ITaxProvider provider in Providers)
                {
                    provider.CalculateTax(transaction);
                }
                transaction.TaxCalculationType = TaxCalculationType.Regular;
            }
        }
        
        /// <summary>
        /// Calculates the taxes for a single item
        /// </summary>
        /// <param name="lineItem"></param>
        /// <param name="transaction"></param>
        [SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public void CalculateTax(ISaleLineItem lineItem, IRetailTransaction transaction)
        {
            try
            {
                if (TaxEngineServiceProxy.IsGTECalculationRequired(transaction) || transaction.TaxCalculationType == TaxCalculationType.GTE)
                {
                    CalculateTax(transaction);
                }
                else
                {
                    lineItem.TaxRatePct = 0;
                    lineItem.TaxLines.Clear();
                    lineItem.CalculateLine();

                    foreach (ITaxProvider provider in Providers)
                    {
                        provider.CalculateTax(lineItem, transaction);
                    }
                }
            }
            catch (Exception x)
            {
                NetTracer.Error(x, "CalculateTax threw an exception: {0}", x.Message);
                LSRetailPosis.ApplicationExceptionHandler.HandleException(this.ToString(), x);
                throw;
            }
        }

        public string GetTaxRegime(IAddress address)
        {
            throw new NotSupportedException("this is not supported operation in POS");
        }

        /// <summary>
        /// Load all registered TaxCodeProviders
        /// </summary>
        protected void LoadProviders()
        {
            ITaxProvider defaultProvider;

            NetTracer.Information("Tax.LoadProviders(): Functions.CountryRegion: {0}", Functions.CountryRegion);

            //Add default provider
            switch (Functions.CountryRegion)
            {
                case SupportedCountryRegion.IN:
                    defaultProvider = new TaxCodeProviderIndia(Application);
                    break;
                case SupportedCountryRegion.BR:
                    defaultProvider = new TaxCodeProviderBrazil(Application);
                    break;
                default:
                    defaultProvider = new TaxCodeProvider(Application);
                    break;
            }

            //Load providers from config file
            Providers = ProviderLoader.Load(defaultProvider);
        }

        public DialogResult TaxOverrideList(TaxOverrideBy overrideBy, out string selectedTaxOverride)
        {
            string selection = string.Empty;

            var result = this.Application.Services.Dialog.TaxOverrideSearch(0, (int)overrideBy, ref selection);

            NetTracer.Information("Tax.TaxOverrideList(): Dialog.SelectedTaxOverrideCode: {0}", result);

            selectedTaxOverride = selection;

            return result;
        }

        /// <summary>
        /// Clears the tax lines in misc. charge.
        /// </summary>
        /// <param name="retailTransaction">Transaction.</param>
        private static void ClearMiscChargeTaxLines(RetailTransaction retailTransaction)
        {
            foreach (MiscellaneousCharge charge in retailTransaction.MiscellaneousCharges)
            {
                charge.TaxLines.Clear();
                charge.TaxMeasures.Clear();
            }

            foreach (SaleLineItem lineItem in retailTransaction.SaleItems)
            {
                foreach (MiscellaneousCharge charge in lineItem.MiscellaneousCharges)
                {
                    charge.TaxLines.Clear();
                    charge.TaxMeasures.Clear();
                }
            }
            
        }

        #endregion
    }
}
