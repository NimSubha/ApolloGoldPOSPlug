/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Reflection;
using System.Security;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;
using Microsoft.Dynamics.Retail.Pos.Tax.Properties;


namespace Microsoft.Dynamics.Retail.Pos.Tax
{
    /// <summary>
    /// Class to manage loading Tax Providers
    /// </summary>
    internal static class ProviderLoader
    {
        private const string DefaultProviderMarker = "*";

        /// <summary>
        /// Load all registered TaxCodeProviders
        /// </summary>
        public static List<ITaxProvider> Load(ITaxProvider defaultProvider)
        {
            List<ITaxProvider> list = new List<ITaxProvider>();

            Assembly asm;
            Type providerType;
            ITaxProvider provider;            

            foreach (ProviderDefinition p in GetProviderDefinitions())
            {
                try
                {
                    if (string.Equals(p.AssemblyPath, DefaultProviderMarker) && string.Equals(p.ClassName, DefaultProviderMarker))
                    {
                        provider = defaultProvider;
                    }
                    else
                    {
                        // Load the provider from the given assembly+class
                        NetTracer.Information("Loading provider from given assembly+class AssemblyPath: {0}; ClassName: {1}", p.AssemblyPath, p.ClassName);
                        asm = Assembly.LoadFrom(p.AssemblyPath);
                        providerType = asm.GetType(p.ClassName);  //Namespace.Classname
                        provider = (ITaxProvider)Activator.CreateInstance(providerType);
                    }
                    list.Add(provider);
                }
                catch (FileNotFoundException ex)
                {
                    NetTracer.Warning(ex, "Tax.ProviderLoader.Load() handled FileNotFoundException. FileName: {0}", ex.FileName);
                    LSRetailPosis.ApplicationExceptionHandler.HandleException("Tax.ProviderLoader.Load()", ex);
                }
                catch (ArgumentNullException ex)
                {
                    NetTracer.Warning(ex, "Tax.ProviderLoader.Load() handled ArgumentNullException. ParamName: {0}", ex.ParamName);
                    LSRetailPosis.ApplicationExceptionHandler.HandleException("Tax.ProviderLoader.Load()", ex);
                }
                catch (FileLoadException ex)
                {
                    NetTracer.Warning(ex, "Tax.ProviderLoader.Load() handled FileLoadException. FileName: {0}", ex.FileName);
                    LSRetailPosis.ApplicationExceptionHandler.HandleException("Tax.ProviderLoader.Load()", ex);
                }
                catch (BadImageFormatException ex)
                {
                    NetTracer.Warning(ex, "Tax.ProviderLoader.Load() handled BadImageFormatException. FileName: {0}", ex.FileName);
                    LSRetailPosis.ApplicationExceptionHandler.HandleException("Tax.ProviderLoader.Load()", ex);
                }
                catch (SecurityException ex)
                {
                    NetTracer.Warning(ex, "Tax.ProviderLoader.Load() handled SecurityException.");
                    LSRetailPosis.ApplicationExceptionHandler.HandleException("Tax.ProviderLoader.Load()", ex);
                }
                catch (ArgumentException ex)
                {
                    NetTracer.Warning(ex, "Tax.ProviderLoader.Load() handled ArgumentException. ParamName: {0}", ex.ParamName);
                    LSRetailPosis.ApplicationExceptionHandler.HandleException("Tax.ProviderLoader.Load()", ex);
                }
                catch (PathTooLongException ex)
                {
                    NetTracer.Warning(ex, "Tax.ProviderLoader.Load() handled PathTooLongException.");
                    LSRetailPosis.ApplicationExceptionHandler.HandleException("Tax.ProviderLoader.Load()", ex);
                }
                catch (MemberAccessException ex)
                {
                    NetTracer.Warning(ex, "Tax.ProviderLoader.Load() handled MemberAccessException.");
                    LSRetailPosis.ApplicationExceptionHandler.HandleException("Tax.ProviderLoader.Load()", ex);
                }
                catch(InvalidCastException ex)
                {
                    NetTracer.Warning(ex, "Tax.ProviderLoader.Load() handled InvalidCastException.");
                    LSRetailPosis.ApplicationExceptionHandler.HandleException("Tax.ProviderLoader.Load()", ex);
                }

            }
            return list;
        }

        /// <summary>
        /// Read definitions of TaxProviders from the local settings
        /// </summary>
        /// <returns></returns>
        private static IEnumerable<ProviderDefinition> GetProviderDefinitions()
        {
            // read list of paths to provider .dlls and class names from config file
            List<ProviderDefinition> list = new List<ProviderDefinition>();

            StringCollection assemblies = Settings.Default.ProviderAssemblies;
            StringCollection classes = Settings.Default.ProviderClassNames;

            int max = Math.Min(assemblies.Count, classes.Count);
            for (int i = 0; i < max; i++)
            {
                list.Add(new ProviderDefinition(assemblies[i], classes[i]));
            }

            return list;
        }

        /// <summary>
        /// Definition of a single provider
        /// </summary>
        private class ProviderDefinition
        {
            public string AssemblyPath { get; private set; }
            public string ClassName { get; private set; }

            public ProviderDefinition(string path, string className)
            {
                this.AssemblyPath = path;
                this.ClassName = className;
            }
        }
    }
}
