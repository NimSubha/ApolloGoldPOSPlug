/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.ShtrihPrinter
{
    /// <summary>
    /// Class for working with Shtrih-M printer tables.
    /// </summary>
    internal sealed class PrinterTableController
    {
        /// <summary>
        /// Class encapsulating field position.
        /// </summary>
        private class FieldPosition
        {
            public FieldPosition(int tableId, int rowId, int fieldId)
            {
                TableId = tableId;
                RowId = rowId;
                FieldId = fieldId;
            }

            public int TableId { get; set; }
            public int RowId { get; set; }
            public int FieldId { get; set; }

            public override int GetHashCode()
            {
                return TableId << 16 | RowId << 8 | FieldId;
            }
        }

        private LayoutType? currentLayoutType = null;
        private PrinterSettingsCollection defaultParameters;
        private bool printerParametersInitialized = false;
        private IShtrihMServiceFunctions printerService;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="printerService">Interface to have access to printer service functions (working with printer tables).</param>
        /// <param name="defaultParameters">A collection of default parameters for the printer.</param>
        public PrinterTableController(IShtrihMServiceFunctions printerService, PrinterSettingsCollection defaultParameters)
        {
            this.printerService = printerService;
            this.defaultParameters = defaultParameters;
        }

        private void SetStringValueToPrinter(FieldPosition position, string value)
        {
            printerService.WriteTableFieldValueOfString(position.TableId, position.RowId, position.FieldId, value);
        }

        private void SetIntValueToPrinter(FieldPosition position, int value)
        {
            printerService.WriteTableFieldValueOfInteger(position.TableId, position.RowId, position.FieldId, value);
        }

        /// <summary>
        /// Sets a value to a printer parameter.
        /// </summary>
        /// <param name="parameter">Printer parameter</param>
        /// <param name="fieldInfo">Field info object</param>
        public void SetValueToPrinter(PrinterParameter parameter, ShtrihMFieldInfo fieldInfo)
        {
            var position = new FieldPosition(parameter.TableId, parameter.RowId, parameter.FieldId);

            switch (fieldInfo.FieldType)
            {
                case PrinterParameterType.Integer:
                    SetIntValueToPrinter(position, int.Parse(parameter.Value));
                    break;
                case PrinterParameterType.String:
                    SetStringValueToPrinter(position, parameter.Value);
                    break;
                default:
                    break;
            }
        }

        private static bool IsDefaultLayout(LayoutType? layout)
        {
            return layout == null;
        }

        private static void LogError(int resourceID, params object[] args)
        {
            string message = Resources.Translate(resourceID, args);
            LogError(message);
        }

        private static void LogError(string message)
        {
            LogHelper.LogError("PrinterTableController.ValidateParameter", message);
        }

        private static bool ValidateParameter(PrinterParameter parameter, ShtrihMFieldInfo fieldInfo)
        {
            if (fieldInfo.FieldType == PrinterParameterType.Integer)
            {
                int tmpValue;

                if (!int.TryParse(parameter.Value, out tmpValue))
                {
                    LogError(Resources.ConfigFileValidationError, parameter.LineNumber, Resources.Translate(Resources.InvalidIntegerNumberValue, parameter.Value, parameter.TableId, parameter.FieldId, parameter.RowId));
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Sets printer parameters for the specified layout.
        /// </summary>
        /// <param name="layoutType">Layout type to load parameters from.</param>
        /// <remarks>
        /// If layout is not found, general printer parameners will be applied.
        /// If layout is found, parameters for specified layout will be applied.
        /// </remarks>
        public void SetPrinterParameters(LayoutType? layoutType, PrinterSettingsCollection parameters)
        {
            bool hasErrors = false;
            bool applyParameters = !printerParametersInitialized;

            if (layoutType != currentLayoutType && printerParametersInitialized)
            {
                if (layoutType != null)
                {
                    // Rewrite default parameters before applying layout-specific ones.
                    SetPrinterDefaultParameters();
                }

                applyParameters = true;
            }

            if (applyParameters && parameters != null)
            {
                var prohibitedTablesShowErrors = new Dictionary<int, bool>();

                foreach (PrinterParameter parameter in parameters)
                {
                    var fieldInfo = printerService.GetTableFieldInfo(parameter.TableId, parameter.FieldId);
                    string reasonMessage = string.Empty;
                    bool showError = false;
                    bool canSetParameter = true;

                    if (prohibitedTablesShowErrors.TryGetValue(parameter.TableId, out showError))
                    {
                        hasErrors = hasErrors || showError;
                        canSetParameter = false;
                    }
                    else if (!printerService.CanWriteToTable(IsDefaultLayout(layoutType), parameter.TableId, out reasonMessage, out showError))
                    {
                        LogError(Resources.ConfigFileValidationError, parameter.LineNumber, reasonMessage);
                        hasErrors = hasErrors || showError;
                        canSetParameter = false;
                        prohibitedTablesShowErrors.Add(parameter.TableId, showError);
                    }

                    if (!ValidateParameter(parameter, fieldInfo))
                    {
                        hasErrors = true;
                        canSetParameter = false;
                    }

                    if (canSetParameter)
                    {
                        SetValueToPrinter(parameter, fieldInfo);
                    }
                }

                if (hasErrors)
                {
                    UserMessages.ShowException(Resources.Translate(Resources.SomePrinterParametersCannotBeSet));
                }

                printerParametersInitialized = true;
                currentLayoutType = layoutType;
            }
        }

        /// <summary>
        /// Sets the printer layout parameters.
        /// </summary>
        /// <param name="layout">Layout type</param>
        /// <param name="parameters">Configuration parameters</param>
        public void SetPrinterLayoutParameters(LayoutType layout, PrinterSettingsCollection parameters)
        {
            SetPrinterParameters(layout, parameters);
        }

        /// <summary>
        /// Sets printer default parameters.
        /// </summary>
        public void SetPrinterDefaultParameters()
        {
            SetPrinterParameters(null, defaultParameters);
        }
    }
}
