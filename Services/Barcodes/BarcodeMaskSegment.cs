/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.BarcodeService
{
    public class BarcodeMaskSegment
    {

        #region Properties

        private int segmentNum;
        private int length;
        private BarcodeSegmentType type;
        private string segmentChar;
        private string maskId;
        private int decimals;

        /// <summary>
        /// Unique id for each segment that is defined in the barcode.
        /// </summary>
        public int SegmentNum
        {
            get { return segmentNum; }
            set { segmentNum = value; }
        }

        /// <summary>
        /// The length of the segment.
        /// </summary>
        public int Length
        {
            get { return length; }
            set { length = value; }
        }

        /// <summary>
        /// The type of the barcodesegment,i.e  price, quantity, or employeeid.
        /// </summary>
        public BarcodeSegmentType SegmentType
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// The same as the segment type, except now shown as a char.
        /// </summary>
        public string SegmentChar
        {
            get { return segmentChar; }
            set { segmentChar = value; }
        }

        /// <summary>
        /// A unique id to identify the mask segments for each barcode mask.
        /// </summary>
        public string MaskId
        {
            get { return maskId; }
            set { maskId = value; }
        }

        /// <summary>
        /// A number of decimals in the segment, if a price or quantity.
        /// </summary>
        public int Decimals
        {
            get { return decimals; }
            set { decimals = value; }
        }

        #endregion
    }
}
