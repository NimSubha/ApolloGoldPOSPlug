/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.Dynamics.Retail.FiscalPrinter.ShtrihPrinter.Constants;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.ShtrihPrinter
{
    /// <summary>
    /// Class for validation config file elements
    /// </summary>
    internal static class PrinterConfigurationValidator
    {
        /// <summary>
        /// Validates that all properties in configuration element can be accessed.
        /// </summary>
        /// <param name="element">Configuration element</param>
        private static void ValidateAllProperties(IConfigurationElementInfo element)
        {
            try
            {
                var props = element.GetType().GetProperties();

                foreach (var pi in props)
                {
                    foreach (var attr in pi.GetCustomAttributes(false))
                    {
                        var configProp = attr as ConfigurationPropertyAttribute;

                        if (configProp != null)
                        {
                            //Throws exception if value is of invalid type
                            pi.GetValue(element, null);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ThrowException(element, ex);
            }
        }

        /// <summary>
        /// Validates configuration file.
        /// </summary>
        /// <param name="config">Printer configuration</param>
        public static void ValidateConfig(PrinterConfiguration config)
        {
            ValidateAllProperties(config);
            ValidateLayouts(config);
            ValidateImages(config);
            ValidateTaxMapping(config);
            ValidateTenderTypesMapping(config);
            ValidateRibbonSettings(config);
        }

        private static void ValidateRibbonSettings(PrinterConfiguration config)
        {
            ValidateAllProperties(config.RibbonSettings);
        }

        private static void ValidateImages(PrinterConfiguration config)
        {
            foreach (Image image in config.ImageList)
            {
                ValidateImage(image);
            }

            if (!string.IsNullOrWhiteSpace(config.ImageList.DefaultImageId))
            {
                var defaultImage = config.ImageList.FindImage(config.ImageList.DefaultImageId);

                if (defaultImage == null)
                {
                    ThrowException(config.ImageList, Resources.InvalidImageId, config.ImageList.DefaultImageId);
                }
            }

        }

        private static void ValidateTaxMapping(PrinterConfiguration config)
        {
            if (config.TaxMapping == null || config.TaxMapping.Count == 0)
            {
                ThrowException(config, Resources.TaxMappingIsNotSet);
            }

            foreach (Tax tax in config.TaxMapping)
            {
                ValidateTax(tax);
            }
        }

        private static void ValidateTax(Tax tax)
        {
            if (!ValueInRange(tax.PrinterTaxId, 1, ShtrihConstants.MaximumNumberOfTaxCodesSupported))
            {
                throw new Exception(Resources.Translate(Resources.TaxMappingWithWrongTaxFound, tax.TaxCode, tax.PrinterTaxId));
            }

            ValidateAllProperties(tax);
        }

        private static void ValidateTenderTypesMapping(PrinterConfiguration config)
        {
            TenderTypesMappingCollection tenderTypesMapping = config.TenderTypesMapping;

            if (tenderTypesMapping == null || tenderTypesMapping.Count == 0)
            {
                ThrowException(tenderTypesMapping, Resources.TenderTypesMappingIsEmpty);
            }

            ISet<int> tenderTypes = new HashSet<int>();

            foreach (TenderType tenderType in tenderTypesMapping)
            {
                if (tenderTypes.Contains(tenderType.TenderTypeId))
                {
                    ThrowException(tenderType, Resources.DuplicateTenderType, tenderType.TenderTypeId);
                }

                tenderTypes.Add(tenderType.TenderTypeId);

                if (!ValueInRange(tenderType.PrinterPaymentType, 1, ShtrihConstants.MaximumNumberOfPaymentTypesSupported))
                {
                    ThrowException(tenderType, Resources.PrinterPaymentTypeShouldBeInRange, 1, ShtrihConstants.MaximumNumberOfPaymentTypesSupported);
                }

                ValidateAllProperties(tenderType);
            }
        }

        private static bool ValueInRange(int value, int minValue, int maxValue)
        {
            return value >= minValue && value <= maxValue;
        }

        private static void ValidateImage(Image image)
        {
            const int LineMin = 1, LineMax = 1200;
            Func<int, bool> inRange = v => ValueInRange(v, LineMin, LineMax);

            if (!inRange(image.StartLine) || !inRange(image.EndLine))
            {
                ThrowException(image, Resources.ImageLineInvalidRange);
            }

            if (image.StartLine > image.EndLine)
            {
                ThrowException(image, Resources.ImageStartLineGreaterThanEndLine);
            }

            ValidateAllProperties(image);
        }

        private static void ValidateLayouts(PrinterConfiguration config)
        {
            foreach (Layout layout in config.Layouts)
            {
                ValidateLayout(config, layout);
            }
        }

        private static void ValidateLayout(PrinterConfiguration config, Layout layout)
        {
            ValidateAllProperties(layout);

            // Validating imageId attribute value in case it is not empty
            if (!string.IsNullOrWhiteSpace(layout.ImageId))
            {
                var image = config.ImageList.FindImage(layout.ImageId);

                if (image == null)
                {
                    ThrowException(layout, Resources.InvalidImageId, layout.ImageId);
                }
            }

            int[] linesOfTypeCount = new int[Enum.GetNames(typeof(LineType)).Length];

            // Validating layout document sections
            foreach (DocumentSection docSect in layout.DocumentSectionCollection)
            {
                ValidateDocumentSection(config, layout, docSect, linesOfTypeCount);
            }

            // Validating no duplicate discount line types in layout
            if (linesOfTypeCount[(int)LineType.SummaryDiscount] > 1 ||
                linesOfTypeCount[(int)LineType.LineDiscount] > 1 ||
                linesOfTypeCount[(int)LineType.PeriodicDiscount] > 1 ||
                linesOfTypeCount[(int)LineType.ReceiptDiscount] > 1 ||
                linesOfTypeCount[(int)LineType.LoyaltyDiscount] > 1)
            {
                ThrowException(layout, Resources.LayoutCannotContainDuplicateDiscountLines);
            }

            // Validating no other discount lines in layout containing summary discount line
            if (linesOfTypeCount[(int)LineType.SummaryDiscount] != 0 &&
                (linesOfTypeCount[(int)LineType.LineDiscount] != 0 ||
                 linesOfTypeCount[(int)LineType.PeriodicDiscount] != 0 ||
                 linesOfTypeCount[(int)LineType.ReceiptDiscount] != 0))
            {
                ThrowException(layout, Resources.OtherDiscountsNotAllowedWithSummaryDiscount);
            }
        }


        private static bool BothSalesAndReturnLayoutsExist(PrinterConfiguration config)
        {
            bool saleExists = false, returnExist = false;

            foreach (var layout in config.Layouts.Cast<Layout>())
            {
                saleExists = saleExists || layout.Type == LayoutType.Sale;
                returnExist = returnExist || layout.Type == LayoutType.Return;
                if (saleExists && returnExist)
                    break;
            }

            return saleExists && returnExist;
        }

        private static bool IsSalesOrReturnLayout(PrinterConfiguration config, Layout layout)
        {
            return (layout.Type == LayoutType.Sale || layout.Type == LayoutType.Return ||
                (!BothSalesAndReturnLayoutsExist(config) && layout.Type == LayoutType.Default));
        }

        /// <summary>
        /// Validates that the provided <paramref name="documentSectionType"/> is valid for the specified <paramref name="layoutType"/>.
        /// </summary>
        /// <param name="layoutType">The <see cref="LayoutType"/>.</param>
        /// <param name="documentSectionType">The <see cref="DocumentSectionType"/>.</param>
        /// <param name="elementInfo">An instance of the <see cref="IConfigurationElementInfo"/> providing the context for the validated element in the configuration file.</param>
        private static void ValidateLayoutTypeDocumentSectionType(LayoutType layoutType, DocumentSectionType documentSectionType, IConfigurationElementInfo elementInfo)
        {
            DocumentSectionType[] layoutTypeAllowedDocumentSectionTypes = GetAllowedDocumentSectionTypes(layoutType);
            if (layoutTypeAllowedDocumentSectionTypes != null && !layoutTypeAllowedDocumentSectionTypes.Contains(documentSectionType))
            {
                ThrowException(elementInfo, Resources.LayoutOfTypeCanOnlyContainSectionOfType, layoutType, string.Join(", ", layoutTypeAllowedDocumentSectionTypes));
            }

            DocumentSectionType[] layoutTypeProhibitedDocumentSectionTypes = GetProhibitedDocumentSectionTypes(layoutType);
            if (layoutTypeProhibitedDocumentSectionTypes != null && layoutTypeProhibitedDocumentSectionTypes.Contains(documentSectionType))
            {
                ThrowException(elementInfo, Resources.LayoutOfTypeCannotContainSectionOfType, layoutType, documentSectionType);
            }

            LayoutType[] documentSectionTypeAllowedLayoutTypes = GetAllowedLayoutTypes(documentSectionType);
            if (documentSectionTypeAllowedLayoutTypes != null && !documentSectionTypeAllowedLayoutTypes.Contains(layoutType))
            {
                ThrowException(elementInfo, Resources.DocumentSectionOfTypeIsAllowedOnlyInLayoutsOfType, documentSectionType, string.Join(", ", documentSectionTypeAllowedLayoutTypes));
            }

            LayoutType[] documentSectionTypeProhibitedLayoutTypes = GetProhibitedLayoutTypes(documentSectionType);
            if (documentSectionTypeProhibitedLayoutTypes != null && documentSectionTypeProhibitedLayoutTypes.Contains(layoutType))
            {
                ThrowException(elementInfo, Resources.LayoutOfTypeCannotContainSectionOfType, layoutType, documentSectionType);
            }
        }

        /// <summary>
        /// Validates that the provided <paramref name="lineType"/> is valid for the specified <paramref name="layoutType"/> and <paramref name="documentSectionType"/>.
        /// </summary>
        /// <param name="layoutType">The <see cref="LayoutType"/>.</param>
        /// <param name="documentSectionType">The <see cref="DocumentSectionType"/>.</param>
        /// <param name="lineType">The <see cref="LineType"/>.</param>
        /// <param name="elementInfo">An instance of the <see cref="IConfigurationElementInfo"/> providing the context for the validated element in the configuration file.</param>
        private static void ValidateLayoutTypeDocumentSectionTypeLineType(LayoutType layoutType, DocumentSectionType documentSectionType, LineType lineType, IConfigurationElementInfo elementInfo)
        {
            LineType[] layoutTypeAllowedLineTypes = GetAllowedLineTypes(layoutType);
            if (layoutTypeAllowedLineTypes != null && !layoutTypeAllowedLineTypes.Contains(lineType))
            {
                ThrowException(elementInfo, Resources.LayoutOfTypeCanContainOnlyLinesOfType, layoutType, string.Join(", ", layoutTypeAllowedLineTypes));
            }

            LineType[] layoutTypeProhibitedLineTypes = GetProhibitedLineTypes(layoutType);
            if (layoutTypeProhibitedLineTypes != null && layoutTypeProhibitedLineTypes.Contains(lineType))
            {
                ThrowException(elementInfo, Resources.LayoutOfTypeCannotContainLineOfType, layoutType, lineType);
            }

            LineType[] documentSectionTypeAllowedLineTypes = GetAllowedLineTypes(documentSectionType);
            if (documentSectionTypeAllowedLineTypes != null && !documentSectionTypeAllowedLineTypes.Contains(lineType))
            {
                ThrowException(elementInfo, Resources.DocumentSectionOfTypeCanContainOnlyLinesOfType, documentSectionType, string.Join(", ", documentSectionTypeAllowedLineTypes));
            }

            LineType[] documentSectionTypeProhibitedLineTypes = GetProhibitedLineTypes(documentSectionType);
            if (documentSectionTypeProhibitedLineTypes != null && documentSectionTypeProhibitedLineTypes.Contains(lineType))
            {
                ThrowException(elementInfo, Resources.DocumentSectionOfTypeCannotContainLineOfType, documentSectionType, lineType);
            }

            LayoutType[] lineTypeAllowedLayoutTypes = GetAllowedLayoutTypes(lineType);
            if (lineTypeAllowedLayoutTypes != null && !lineTypeAllowedLayoutTypes.Contains(layoutType))
            {
                ThrowException(elementInfo, Resources.LineOfTypeIsAllowedOnlyInLayoutsOfType, lineType, string.Join(", ", lineTypeAllowedLayoutTypes));
            }

            LayoutType[] lineTypeProhibitedLayoutTypes = GetProhibitedLayoutTypes(lineType);
            if (lineTypeProhibitedLayoutTypes != null && lineTypeProhibitedLayoutTypes.Contains(layoutType))
            {
                ThrowException(elementInfo, Resources.LayoutOfTypeCannotContainLineOfType, layoutType, lineType);
            }

            DocumentSectionType[] lineTypeAllowedDocumentSectionTypes = GetAllowedDocumentSectionTypes(lineType);
            if (lineTypeAllowedDocumentSectionTypes != null && !lineTypeAllowedDocumentSectionTypes.Contains(documentSectionType))
            {
                ThrowException(elementInfo, Resources.LineOfTypeIsAllowedOnlyInDocumentSectionsOfType, lineType, string.Join(", ", lineTypeAllowedDocumentSectionTypes));
            }

            DocumentSectionType[] lineTypeProhibitedDocumentSectionTypes = GetProhibitedDocumentSectionTypes(lineType);
            if (lineTypeProhibitedDocumentSectionTypes != null && lineTypeProhibitedDocumentSectionTypes.Contains(documentSectionType))
            {
                ThrowException(elementInfo, Resources.DocumentSectionOfTypeCannotContainLineOfType, documentSectionType, lineType);
            }
        }

        /// <summary>
        /// Validates that the provided <paramref name="fieldType"/> is valid for the specified <paramref name="layoutType"/>, <paramref name="documentSectionType"/> and <paramref name="lineType"/>.
        /// </summary>
        /// <param name="layoutType">The <see cref="LayoutType"/>.</param>
        /// <param name="documentSectionType">The <see cref="DocumentSectionType"/>.</param>
        /// <param name="fieldType">The <see cref="FieldType"/>.</param>
        /// <param name="lineType">The <see cref="LineType"/>.</param>
        /// <param name="elementInfo">An instance of the <see cref="IConfigurationElementInfo"/> providing the context for the validated element in the configuration file.</param>
        private static void ValidateLayoutTypeDocumentSectionTypeLineTypeFieldType(LayoutType layoutType, DocumentSectionType documentSectionType, LineType lineType, FieldType fieldType, IConfigurationElementInfo elementInfo)
        {
            FieldType[] layoutAllowedFieldTypes = GetAllowedFieldTypes(layoutType);
            if (layoutAllowedFieldTypes != null && !layoutAllowedFieldTypes.Contains(fieldType))
            {
                ThrowException(elementInfo, Resources.LayoutOfTypeCanContainOnlyFieldsOfType, layoutType, string.Join(", ", layoutAllowedFieldTypes));
            }

            FieldType[] layoutProhibitedFieldTypes = GetProhibitedFieldTypes(layoutType);
            if (layoutProhibitedFieldTypes != null && layoutProhibitedFieldTypes.Contains(fieldType))
            {
                ThrowException(elementInfo, Resources.LayoutOfTypeCannotContainFieldOfType, layoutType, fieldType);
            }

            FieldType[] documentSectionAllowedFieldTypes = GetAllowedFieldTypes(documentSectionType);
            if (documentSectionAllowedFieldTypes != null && !documentSectionAllowedFieldTypes.Contains(fieldType))
            {
                ThrowException(elementInfo, Resources.DocumentSectionOfTypeCanContainOnlyFieldsOfType, documentSectionType, string.Join(", ", documentSectionAllowedFieldTypes));
            }

            FieldType[] documentSectionProhibitedFieldTypes = GetProhibitedFieldTypes(documentSectionType);
            if (documentSectionProhibitedFieldTypes != null && documentSectionProhibitedFieldTypes.Contains(fieldType))
            {
                ThrowException(elementInfo, Resources.DocumentSectionOfTypeCannotContainFieldOfType, documentSectionType, fieldType);
            }

            FieldType[] lineTypeAllowedFieldTypes = GetAllowedFieldTypes(lineType);
            if (lineTypeAllowedFieldTypes != null && !lineTypeAllowedFieldTypes.Contains(fieldType))
            {
                ThrowException(elementInfo, Resources.LineOfTypeCanContainOnlyFieldsOfType, lineType, string.Join(", ", lineTypeAllowedFieldTypes));
            }

            FieldType[] lineTypeProhibitedFieldTypes = GetProhibitedFieldTypes(lineType);
            if (lineTypeProhibitedFieldTypes != null && lineTypeProhibitedFieldTypes.Contains(fieldType))
            {
                ThrowException(elementInfo, Resources.LineOfTypeCannotContainFieldOfType, lineType, fieldType);
            }

            LayoutType[] fieldTypeAllowedLayoutTypes = GetAllowedLayoutTypes(fieldType);
            if (fieldTypeAllowedLayoutTypes != null && !fieldTypeAllowedLayoutTypes.Contains(layoutType))
            {
                ThrowException(elementInfo, Resources.FieldOfTypeIsAllowedOnlyInLayoutsOfType, fieldType, string.Join(", ", fieldTypeAllowedLayoutTypes));
            }

            LayoutType[] fieldTypeProhibitedLayoutTypes = GetProhibitedLayoutTypes(fieldType);
            if (fieldTypeProhibitedLayoutTypes != null && fieldTypeProhibitedLayoutTypes.Contains(layoutType))
            {
                ThrowException(elementInfo, Resources.LayoutOfTypeCannotContainFieldOfType, layoutType, fieldType);
            }

            DocumentSectionType[] fieldTypeAllowedDocumentSectionTypes = GetAllowedDocumentSectionTypes(fieldType);
            if (fieldTypeAllowedDocumentSectionTypes != null && !fieldTypeAllowedDocumentSectionTypes.Contains(documentSectionType))
            {
                ThrowException(elementInfo, Resources.FieldOfTypeIsAllowedOnlyInDocumentSectionsOfType, fieldType, string.Join(", ", fieldTypeAllowedDocumentSectionTypes));
            }

            DocumentSectionType[] fieldTypeProhibitedDocumentSectionTypes = GetProhibitedDocumentSectionTypes(fieldType);
            if (fieldTypeProhibitedDocumentSectionTypes != null && fieldTypeProhibitedDocumentSectionTypes.Contains(documentSectionType))
            {
                ThrowException(elementInfo, Resources.DocumentSectionOfTypeCannotContainFieldOfType, documentSectionType, fieldType);
            }

            LineType[] fieldTypeAllowedLineTypes = GetAllowedLineTypes(fieldType);
            if (fieldTypeAllowedLineTypes != null && !fieldTypeAllowedLineTypes.Contains(lineType))
            {
                ThrowException(elementInfo, Resources.FieldOfTypeIsAllowedOnlyInLinesOfType, fieldType, string.Join(", ", fieldTypeAllowedLineTypes));
            }

            LineType[] fieldTypeProhibitedLineTypes = GetProhibitedLineTypes(fieldType);
            if (fieldTypeProhibitedLineTypes != null && fieldTypeProhibitedLineTypes.Contains(lineType))
            {
                ThrowException(elementInfo, Resources.LineOfTypeCannotContainFieldOfType, lineType, fieldType);
            }
        }

        /// <summary>
        /// Gets the array of restriction values.
        /// </summary>
        /// <typeparam name="A">The type of attributes to search for.</typeparam>
        /// <typeparam name="R">The type of the restriction values.</typeparam>
        /// <param name="enumeration">The enumeration value to get the attributes from.</param>
        /// <returns>An array of restriction values of type <typeparamref name="R"/>.</returns>
        private static R[] GetRestrictions<A, R>(Enum enumeration) where A : Attribute
        {
            Attribute[] typeAttributes = GetCustomAttributes<A>(enumeration);
            if (typeAttributes == null)
            {
                return null;
            }
            return typeAttributes.OfType<IRestrictions<R>>().Select(_ => _.Restrictions).SingleOrDefault();
        }

        private static FieldType[] GetAllowedFieldTypes(LayoutType layoutType)
        {
            return GetRestrictions<FieldTypeAllowedAttribute, FieldType>(layoutType);
        }

        private static FieldType[] GetProhibitedFieldTypes(LayoutType layoutType)
        {
            return GetRestrictions<FieldTypeProhibitedAttribute, FieldType>(layoutType);
        }

        private static FieldType[] GetAllowedFieldTypes(DocumentSectionType documentSectionType)
        {
            return GetRestrictions<FieldTypeAllowedAttribute, FieldType>(documentSectionType);
        }

        private static FieldType[] GetProhibitedFieldTypes(DocumentSectionType documentSectionType)
        {
            return GetRestrictions<FieldTypeProhibitedAttribute, FieldType>(documentSectionType);
        }

        private static FieldType[] GetAllowedFieldTypes(LineType lineType)
        {
            return GetRestrictions<FieldTypeAllowedAttribute, FieldType>(lineType);
        }

        private static FieldType[] GetProhibitedFieldTypes(LineType lineType)
        {
            return GetRestrictions<FieldTypeProhibitedAttribute, FieldType>(lineType);
        }

        private static LineType[] GetAllowedLineTypes(LayoutType layoutType)
        {
            return GetRestrictions<LineTypeAllowedAttribute, LineType>(layoutType);
        }

        private static LineType[] GetProhibitedLineTypes(LayoutType layoutType)
        {
            return GetRestrictions<LineTypeProhibitedAttribute, LineType>(layoutType);
        }

        private static LineType[] GetAllowedLineTypes(DocumentSectionType documentSectionType)
        {
            return GetRestrictions<LineTypeAllowedAttribute, LineType>(documentSectionType);
        }

        private static LineType[] GetProhibitedLineTypes(DocumentSectionType documentSectionType)
        {
            return GetRestrictions<LineTypeProhibitedAttribute, LineType>(documentSectionType);
        }

        private static LineType[] GetAllowedLineTypes(FieldType fieldType)
        {
            return GetRestrictions<LineTypeAllowedAttribute, LineType>(fieldType);
        }

        private static LineType[] GetProhibitedLineTypes(FieldType fieldType)
        {
            return GetRestrictions<LineTypeProhibitedAttribute, LineType>(fieldType);
        }

        private static DocumentSectionType[] GetAllowedDocumentSectionTypes(LayoutType layoutType)
        {
            return GetRestrictions<DocumentSectionTypeAllowedAttribute, DocumentSectionType>(layoutType);
        }

        private static DocumentSectionType[] GetProhibitedDocumentSectionTypes(LayoutType layoutType)
        {
            return GetRestrictions<DocumentSectionTypeProhibitedAttribute, DocumentSectionType>(layoutType);
        }

        private static DocumentSectionType[] GetAllowedDocumentSectionTypes(LineType lineType)
        {
            return GetRestrictions<DocumentSectionTypeAllowedAttribute, DocumentSectionType>(lineType);
        }

        private static DocumentSectionType[] GetProhibitedDocumentSectionTypes(LineType lineType)
        {
            return GetRestrictions<DocumentSectionTypeProhibitedAttribute, DocumentSectionType>(lineType);
        }

        private static DocumentSectionType[] GetAllowedDocumentSectionTypes(FieldType fieldType)
        {
            return GetRestrictions<DocumentSectionTypeAllowedAttribute, DocumentSectionType>(fieldType);
        }

        private static DocumentSectionType[] GetProhibitedDocumentSectionTypes(FieldType fieldType)
        {
            return GetRestrictions<DocumentSectionTypeProhibitedAttribute, DocumentSectionType>(fieldType);
        }

        private static LayoutType[] GetAllowedLayoutTypes(DocumentSectionType documentSectionType)
        {
            return GetRestrictions<LayoutTypeAllowedAttribute, LayoutType>(documentSectionType);
        }

        private static LayoutType[] GetProhibitedLayoutTypes(DocumentSectionType documentSectionType)
        {
            return GetRestrictions<LayoutTypeProhibitedAttribute, LayoutType>(documentSectionType);
        }

        private static LayoutType[] GetAllowedLayoutTypes(LineType lineType)
        {
            return GetRestrictions<LayoutTypeAllowedAttribute, LayoutType>(lineType);
        }

        private static LayoutType[] GetProhibitedLayoutTypes(LineType lineType)
        {
            return GetRestrictions<LayoutTypeProhibitedAttribute, LayoutType>(lineType);
        }

        private static LayoutType[] GetAllowedLayoutTypes(FieldType fieldType)
        {
            return GetRestrictions<LayoutTypeAllowedAttribute, LayoutType>(fieldType);
        }

        private static LayoutType[] GetProhibitedLayoutTypes(FieldType fieldType)
        {
            return GetRestrictions<LayoutTypeProhibitedAttribute, LayoutType>(fieldType);
        }

        private static Attribute[] GetCustomAttributes<A>(Enum enumeration) where A : Attribute
        {
            Type enumerationType = enumeration.GetType();
            return Attribute.GetCustomAttributes(enumerationType.GetField(Enum.GetName(enumerationType, enumeration)), typeof(A));
        }

        private static void ValidateDocumentSection(PrinterConfiguration config, Layout layout, DocumentSection documentSection, int[] linesOfTypeLayoutCount)
        {
            //SalesLine document section type is valid only for Sale and Return layouts
            if (documentSection.Type == DocumentSectionType.SalesLine && !IsSalesOrReturnLayout(config, layout))
            {
                ThrowException(documentSection, Resources.SalesLineSectionValidOnlyForSalesLayout);
            }

            //TotalsHeader document section type is valid only for Sale and Return layouts
            if (documentSection.Type == DocumentSectionType.TotalsHeader && !IsSalesOrReturnLayout(config, layout))
            {
                ThrowException(documentSection, Resources.TotalsHeaderSectionValidOnlyForSalesLayout);
            }

            ValidateLayoutTypeDocumentSectionType(layout.Type, documentSection.Type, documentSection);

            int[] linesOfTypeSectionCount = new int[Enum.GetNames(typeof(LineType)).Length];

            foreach (Line line in documentSection.LineCollection)
            {
                linesOfTypeSectionCount[(int)line.Type]++;
                ValidateLine(layout, documentSection, line);

                // Validating sales line document section does not contain any discount line before fiscal line in config file
                if (documentSection.Type == DocumentSectionType.SalesLine &&
                    linesOfTypeSectionCount[(int)LineType.SalesFiscal] == 0 &&
                    LineManager.LineTypeIsOfDiscountType(line.Type))
                {
                    ThrowException(documentSection, Resources.DiscountLineNotAllowedBeforeFiscalLine);
                }
            }

            // Validating document section does not contain more than one fiscal line
            if (linesOfTypeSectionCount[(int)LineType.SalesFiscal] > 1)
            {
                ThrowException(documentSection, Resources.OnlyOneSalesFiscalLineAllowedPerSection);
            }

            // Validating document section does not contain duplicate discount line types 
            if (linesOfTypeSectionCount[(int)LineType.SummaryDiscount] > 1 ||
                linesOfTypeSectionCount[(int)LineType.LineDiscount] > 1 ||
                linesOfTypeSectionCount[(int)LineType.PeriodicDiscount] > 1 ||
                linesOfTypeSectionCount[(int)LineType.ReceiptDiscount] > 1 ||
                linesOfTypeSectionCount[(int)LineType.RoundingDiscount] > 1 ||
                linesOfTypeSectionCount[(int)LineType.LoyaltyDiscount] > 1)
            {
                ThrowException(documentSection, Resources.DocumentSectionCannotContainDuplicateDiscountLines);
            }

            // Validating document section does not contain more than one gift card payment line
            if (linesOfTypeSectionCount[(int)LineType.GiftCardDiscount] > 1)
            {
                ThrowException(documentSection, Resources.DocumentSectionCannotContainMoreThanOneGiftCardPaymentLine);
            }

            // add section line type counters to layout counters
            for (int i = 0; i < linesOfTypeSectionCount.Length; i++)
            {
                linesOfTypeLayoutCount[i] += linesOfTypeSectionCount[i];
            }

            ValidateAllProperties(documentSection);

            ValidateRoundingSettings(documentSection);
        }

        private static void ValidateRoundingSettings(DocumentSection documentSection)
        {
            bool isAddRoundingToDiscount = false, hasRoundingDiscountLine = false;
            foreach (Line line in documentSection.LineCollection)
            {
                if (line.AddRoundingToDiscount.HasValue && line.AddRoundingToDiscount.Value)
                {
                    isAddRoundingToDiscount = true;
                }
                if (line.Type == LineType.RoundingDiscount)
                    hasRoundingDiscountLine = true;

                if (isAddRoundingToDiscount && hasRoundingDiscountLine)
                    ThrowException(documentSection, Resources.RoundingDiscountLineWithAddRoundingToDiscountPropertyNotAllowed);
            }
        }

        private static void ValidateLine(Layout layout, DocumentSection documentSection, Line line)
        {
            ValidateLayoutTypeDocumentSectionTypeLineType(layout.Type, documentSection.Type, line.Type, line);

            foreach (Field field in line.FieldCollection)
            {
                ValidateField(layout, documentSection, line, field);
            }

            ValidateAllProperties(line);
            ValidateHideIfEmptyField(layout, documentSection, line);
        }

        private static void ValidateHideIfEmptyFieldType(FieldType hideIfEmptyFieldType, IConfigurationElementInfo elementInfo)
        {
            FieldTypeProhibitedAttribute hideIfEmptyFieldAttribute = (FieldTypeProhibitedAttribute)Attribute.GetCustomAttribute(typeof(Line).GetProperty("HideIfEmptyField"), typeof(FieldTypeAllowedAttribute), false);

            if (hideIfEmptyFieldAttribute != null && 
                hideIfEmptyFieldAttribute.FieldTypes != null && 
                hideIfEmptyFieldAttribute.FieldTypes.Contains(hideIfEmptyFieldType))
            {
                ThrowException(elementInfo, Resources.HideIfEmptyFieldNotAllowedForFieldOfType, hideIfEmptyFieldType);
            }
        }

        private static void ValidateHideIfEmptyField(Layout layout, DocumentSection documentSection, Line line)
        {
            if (line.HideIfEmptyField != null)
            {
                if (line.Type == LineType.SalesFiscal)
                {
                    ThrowException(line, Resources.HideIfEmptyFieldNotAllowedInSalesFiscalLine);
                }

                if (line.HideIfEmptyField.HasValue)
                {
                    ValidateHideIfEmptyFieldType(line.HideIfEmptyField.Value, line);
                }

                ValidateLayoutTypeDocumentSectionTypeLineTypeFieldType(layout.Type, documentSection.Type, line.Type, line.HideIfEmptyField.Value, line);
            }
        }

        private static void ValidateField(Layout layout, DocumentSection docSect, Line line, Field field)
        {
            ValidateLayoutTypeDocumentSectionTypeLineTypeFieldType(layout.Type, docSect.Type, line.Type, field.Type, field);

            if (field.Length < 0 || field.Length > Int16.MaxValue)
            {
                ThrowException(layout, Resources.InvalidAttributeValue, "length", field.Length);
            }

            ValidateAllProperties(field);
        }

        private static void ThrowException(IConfigurationElementInfo elementinfo, int resourceId, params object[] args)
        {
            string message = Resources.Translate(resourceId, args);
            ThrowException(elementinfo, new ApplicationException(message));
        }

        private static void ThrowException(IConfigurationElementInfo elementinfo, Exception ex)
        {
            if (ex != null)
            {
                ex = ex.InnerException ?? ex;
                string message = Resources.Translate(Resources.ConfigFileValidationError, elementinfo.LineNumber, ex.Message);
                throw new ApplicationException(message);
            }
        }
    }
}
