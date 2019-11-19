/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Dynamics.Retail.FiscalPrinter.RussianFiscalPrinter;

namespace Microsoft.Dynamics.Retail.FiscalPrinter.ShtrihPrinter
{
    /// <summary>
    /// Field manager class contains logic for working with Field class.
    /// </summary>   
    internal sealed class FieldManager 
    {
        /// <summary>
        /// Retrives discount type from the field type.
        /// </summary>
        /// <param name="fieldType">Field type</param>
        /// <returns>Discount type</returns>
        public static DiscountType FieldType2DiscountType(FieldType fieldType)
        {
            DiscountType discountType;

            switch (fieldType)
            {
                case FieldType.SummaryDiscountAmount:
                case FieldType.SummaryDiscountPercent:
                    discountType = DiscountType.SummaryDiscount;
                    break;
                case FieldType.LineDiscountAmount:
                case FieldType.LineDiscountPercent:
                    discountType = DiscountType.LineDiscount;
                    break;
                case FieldType.PeriodicDiscountAmount:
                case FieldType.PeriodicDiscountPercent:
                    discountType = DiscountType.PeriodicDiscount;
                    break;
                case FieldType.ReceiptDiscountAmount:
                case FieldType.ReceiptDiscountPercent:
                    discountType = DiscountType.ReceiptDiscount;
                    break;
                case FieldType.RoundingDiscountAmount:
                case FieldType.RoundingDiscountPercent:
                    discountType = DiscountType.RoundingDiscount;
                    break;
                case FieldType.LoyaltyAmount:
                case FieldType.LoyaltyPercent:
                    discountType = DiscountType.LoyaltyDiscount;
                    break;
                default:
                    throw new ArgumentException();
            }

            return discountType;
        }

        private static readonly List<FieldType> discountAmountFields = new List<FieldType>() 
        { 
            FieldType.SummaryDiscountAmount, 
            FieldType.LineDiscountAmount, 
            FieldType.PeriodicDiscountAmount, 
            FieldType.ReceiptDiscountAmount,
            FieldType.RoundingDiscountAmount,
            FieldType.LoyaltyAmount
        };

        /// <summary>
        /// Gets list of discount amount fields
        /// </summary>
        /// <returns>List of discount amount fields</returns>
        public static List<FieldType> DiscountAmountFields
        {
            get { return discountAmountFields; }           
        }

        private static readonly List<FieldType> discountPercentFields = new List<FieldType>() 
        { 
            FieldType.SummaryDiscountPercent, 
            FieldType.LineDiscountPercent, 
            FieldType.PeriodicDiscountPercent, 
            FieldType.ReceiptDiscountPercent,
            FieldType.RoundingDiscountPercent,
            FieldType.LoyaltyPercent
        };

        /// <summary>
        /// Gets list of discount percent fields
        /// </summary>
        /// <returns>List of discount percent fields</returns>
        public static List<FieldType> DiscountPercentFields
        {
            get { return discountPercentFields; }           
        }

        /// <summary>
        /// Gets the formated text value of the field depending on the field parameters (type, legth, alignment).
        /// </summary>        
        /// <param name="field">The <see cref="Field"/> object encapsulating the configuration file element.</param>
        /// <param name="parametersDictionary">A dictionary containing the values for the field pararameters.</param>        
        /// <returns>Field value represented as a formatted string.</returms>
        public static string GetFormatedText(Field field, IDictionary<FieldType, string> parametersDictionary = null)
        {
            string result = string.Empty;
            result = GetFieldValue(field, parametersDictionary);

            if ((field.Length > 0))
            {
                result = string.Format("{0," + (field.Length * (int)field.Alignment).ToString() + "}", result);
                result = Truncate(result, field.Length);                
            }

            return result;
        }

        /// <summary>
        /// Gets a substring of the first N characters.                     
        /// </summary>        
        /// <param name="source">The source string to truncate.</param>
        /// <param name="length">Max length of the substring.</param>      
        /// <returns>String after truncation.</returns>
        internal static string Truncate(string source, int length)
        {
	        if (source.Length > length)
	        {
	            source = source.Substring(0, length);
	        }

	        return source;
        }


        /// <summary>
        /// Gets the formated text value of the field depending on the field type and parameters.                
        /// </summary>
        /// <param name="field">The <see cref="Field"/> object encapsulating the configuration file element.</param>
        /// <param name="parametersDictionary">A dictioanary containing the values for the field parameters.</param>
        /// <returns>Field value string representation.</returns>
        private static string GetFieldValue(Field field,  IDictionary<FieldType, string> parametersDictionary = null)
        {
            switch (GetFieldCategory(field.Type))
            {
                case (FieldCategory.Text): 
                    return field.Value;
                case (FieldCategory.Parameter):
                    return parametersDictionary[field.Type];
                default:
                    return null;                      
            }            
        }

        /// <summary>
        /// Gets field category.
        /// </summary>        
        /// <param name="fieldType">Type of the field.</param>        
        /// <returns>Field category</returns>    
        private static FieldCategory GetFieldCategory(FieldType fieldType)
        {
            FieldCategoryAttribute fieldCategoryAttribute = (FieldCategoryAttribute)Attribute.GetCustomAttribute(GetTypeMemberInfo(fieldType), typeof(FieldCategoryAttribute));
            return fieldCategoryAttribute.Category;
        }

        /// <summary>
        /// Gets the <see cref="MemberInfo"/> object.
        /// </summary>        
        /// <param name="fieldType">The type of the field.</param>        
        /// <returns>MemberInfo object containing the metadata information.</returns>
        private static MemberInfo GetTypeMemberInfo(FieldType fieldType)
        {
            return typeof(FieldType).GetField(Enum.GetName(typeof(FieldType), fieldType));
        }   

    }
}
