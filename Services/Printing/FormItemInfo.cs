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
using System.Text;
using Microsoft.Dynamics.Retail.Diagnostics;

namespace Microsoft.Dynamics.Retail.Pos.Printing
{
    public class FormItemInfo
    {
        private string variable;
        private bool isVariable;
        private int lineIndex;
        private int charIndex;
        private string valueString;
        private valign vertAlign;
        private char fill;
        private int length;
        private string prefix;
        private System.Drawing.FontStyle fontStyle;
        private int sizeFactor = 1;

        /// <summary>
        /// Get property for SizeFactor.
        /// </summary>
        public int SizeFactor
        {
            get
            {
                return sizeFactor;
            }
        }

        /// <summary>
        /// Get/set property for Variable.
        /// </summary>
        public string Variable
        {
            get
            {
                return variable;
            }
            set
            {
                if (variable == value)
                    return;
                variable = value;
            }
        }

        /// <summary>
        /// Get/set property for IsVariable.
        /// </summary>
        public bool IsVariable
        {
            get
            {
                return isVariable;
            }
            set
            {
                if (isVariable == value)
                    return;
                isVariable = value;
            }
        }

        /// <summary>
        /// Get/set property for LineIndex.
        /// </summary>
        public int LineIndex
        {
            get
            {
                return lineIndex;
            }
            set
            {
                if (lineIndex == value)
                    return;
                lineIndex = value;
            }
        }

        /// <summary>
        /// Get/set property for CharIndex.
        /// </summary>
        public int CharIndex
        {
            get
            {
                return charIndex;
            }
            set
            {
                if (charIndex == value)
                    return;
                charIndex = value;
            }
        }

        /// <summary>
        /// Get/set property for ValueString.
        /// </summary>
        public string ValueString
        {
            get
            {
                return valueString;
            }
            set
            {
                if (valueString == value)
                    return;
                valueString = value;
            }
        }

        /// <summary>
        /// Get/set property for VertAlign.
        /// </summary>
        public valign VertAlign
        {
            get
            {
                return vertAlign;
            }
            set
            {
                if (vertAlign == value)
                    return;
                vertAlign = value;
            }
        }

        /// <summary>
        /// Get/set property for Fill.
        /// </summary>
        public char Fill
        {
            get
            {
                return fill;
            }
            set
            {
                if (fill == value)
                    return;
                fill = value;
            }
        }

        /// <summary>
        /// Get/set property for Length.
        /// </summary>
        public int Length
        {
            get
            {
                // if font is bold then letters occupy more space and therefor have more length
                return length / sizeFactor;
            }
            set
            {
                if (length == value)
                    return;
                length = value;
            }
        }

        /// <summary>
        /// Get/set property for Prefix.
        /// </summary>
        public string Prefix
        {
            get
            {
                return prefix;
            }
            set
            {
                if (prefix == value)
                    return;
                prefix = value;
            }
        }

        /// <summary>
        /// Get/set property for FontStyle.
        /// </summary>
        public System.Drawing.FontStyle FontStyle
        {
            get
            {
                return fontStyle;
            }
            set
            {
                if (fontStyle == value)
                    return;
                fontStyle = value;
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="formItem"></param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Grandfather")]
        public FormItemInfo(DataRow formItem)
        {
            try
            {
                this.charIndex = Convert.ToInt16(formItem["nr"]);
                this.valueString = formItem["value"].ToString();
                if (formItem["valign"].ToString() == "right")
                    this.vertAlign = valign.right;
                else if (formItem["valign"].ToString() == "left")
                    this.vertAlign = valign.left;
                else if (formItem["valign"].ToString() == "center")
                    this.vertAlign = valign.center;
                this.fill = Convert.ToChar(string.Concat(formItem["fill"]));
                this.Variable = formItem["variable"].ToString();
                this.IsVariable = formItem["variable"].ToString().Length != 0;
                this.Prefix = formItem["prefix"].ToString();
                
                this.fontStyle = (System.Drawing.FontStyle)Convert.ToInt32(formItem["FontStyle"].ToString());
                if (this.fontStyle == System.Drawing.FontStyle.Bold)
                    this.sizeFactor = 2;
                else
                    this.sizeFactor = 1;

                this.length = Convert.ToInt16(formItem["length"]) + (this.prefix.Length * this.sizeFactor);
            }
            catch (Exception ex)
            {
                NetTracer.Error(ex, "FormItemInfo::FormItemInfo failed");
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public FormItemInfo()
        {
        }
    }
}
