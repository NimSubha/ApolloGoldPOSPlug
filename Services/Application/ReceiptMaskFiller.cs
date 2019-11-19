/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.Dynamics.Retail.Pos.ApplicationService
{
    sealed public class ReceiptMaskFiller
    {
        private ReceiptMaskFiller() { }

        struct FormatBlock
        {
            public char blockChar;
            public int startIdx;
            public int length;

            public FormatBlock(char blockChar, int startIdx, int length)
            {
                this.blockChar = blockChar;
                this.startIdx = startIdx;
                this.length = length;
            }
        }

        /// <summary>
        /// Given a mask and lots of possible parameters fill in the template mask
        /// </summary>
        /// <param name="receiptMask">Template mask for receipt Id</param>
        /// <param name="storeId">Store Id</param>
        /// <param name="terminalId">Terminal Id</param>
        /// <param name="staffId">Staff Id</param>
        /// <param name="seedValue">Receipt number, the sequential part of the receipt Id</param>
        /// <returns>Populated receipt ID from mask</returns>
        static public string FillMask(string receiptMask, string seedValue, string storeId, string terminalId, string staffId)
        {
            return FillMask(receiptMask, seedValue, storeId, terminalId, staffId, DateTime.Now);
        }

        /// <summary>
        /// Given a mask and lots of possible parameters fill in the template mask
        /// </summary>
        /// <param name="receiptMask">Template mask for receipt Id</param>
        /// <param name="storeId">Store Id</param>
        /// <param name="terminalId">Terminal Id</param>
        /// <param name="staffId">Staff Id</param>
        /// <param name="seedValue">Receipt number, the sequential part of the receipt Id</param>
        /// <param name="currentDate">Date for receipt Id</param>
        /// <returns>Populated receipt ID from mask</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Grandfather")]
        static public string FillMask(string receiptMask, string seedValue, string storeId, string terminalId, string staffId, DateTime currentDate)
        {
            string receiptId = receiptMask;

            // Get list of blocks in mask
            ReadOnlyCollection<FormatBlock> blockList = GetFormatBlocks(receiptId, new char[] { 'S', 'T', 'C', '#', 'd', 'M', 'D', 'Y' });

            // For each block, fill with contents
            foreach (FormatBlock formatBlock in blockList)
            {
                switch (formatBlock.blockChar)
                {
                    case 'S':
                        receiptId = FillBlock(receiptId, formatBlock, storeId);
                        break;

                    case 'T':
                        receiptId = FillBlock(receiptId, formatBlock, terminalId);
                        break;

                    case 'C':
                        receiptId = FillBlock(receiptId, formatBlock, staffId);
                        break;

                    case '#':
                        receiptId = FillBlock(receiptId, formatBlock, seedValue);
                        break;

                    case 'd':
                        if (formatBlock.length >= 3) // ddd is a valid block but dd isn't
                        {
                            receiptId = FillBlock(receiptId, formatBlock, currentDate.DayOfYear.ToString());
                        }
                        break;

                    case 'D':
                        if (formatBlock.length >= 2) // DD is a valid block but D isn't
                        {
                            receiptId = FillBlock(receiptId, formatBlock, currentDate.Day.ToString());
                        }
                        break;

                    case 'M':
                        if (formatBlock.length >= 2)
                        {
                            receiptId = FillBlock(receiptId, formatBlock, currentDate.Month.ToString());
                        }
                        break;

                    case 'Y':
                        if (formatBlock.length >= 2)
                        {
                            receiptId = FillBlock(receiptId, formatBlock, currentDate.Year.ToString());
                        }
                        break;

                    default:
                        break;
                }
            }

            // if empty mask, default to seed number
            if (string.IsNullOrEmpty(receiptId))
            {
                receiptId = seedValue.ToString();
            }

            return receiptId;
        }

        /// <summary>
        /// Given template mask string, and characters which indicated a formatted block, get all formatted blocks
        /// </summary>
        /// <param name="mask">Template mask string to search for format blocks</param>
        /// <param name="formatChars">Array of characters valid for each format block type</param>
        /// <returns>List of FormatBlocks containing character, start, and length of block in template</returns>
        static private ReadOnlyCollection<FormatBlock> GetFormatBlocks(string mask, char[] formatChars)
        {
            List<FormatBlock> blockList = new List<FormatBlock>();
            int currentIdx;
            int startBlockIdx;
            int endBlockIdx;
            int blockLength;

            currentIdx = mask.IndexOfAny(formatChars);
            while (currentIdx < mask.Length &&
                currentIdx >= 0)
            {
                //Find the next template block
                startBlockIdx = currentIdx;
                endBlockIdx = FindEndOfBlock(mask, startBlockIdx);
                blockLength = endBlockIdx - startBlockIdx + 1;

                //Add block to list
                blockList.Add(new FormatBlock(mask[startBlockIdx], startBlockIdx, blockLength));

                //Increment to next block start
                currentIdx = mask.IndexOfAny(formatChars, endBlockIdx + 1);
            }

            return new ReadOnlyCollection<FormatBlock>(blockList);
        }

        /// <summary>
        /// Given a string, contents, and format block in the string, fill format block with contents
        /// </summary>
        /// <param name="template">String with block to be filled</param>
        /// <param name="formatBlock">Struct decribing the block to be filled in the template string</param>
        /// <param name="contents">String to put into the templated format block</param>
        /// <returns></returns>
        static private string FillBlock(string template, FormatBlock formatBlock, string contents)
        {
            string filledTemplate = template;

            string trimmed = contents.PadLeft(formatBlock.length, '0');
            trimmed = trimmed.Substring(trimmed.Length - formatBlock.length);

            filledTemplate = filledTemplate.Remove(formatBlock.startIdx, formatBlock.length);
            filledTemplate = filledTemplate.Insert(formatBlock.startIdx, trimmed);

            return filledTemplate;
        }
        
        /// <summary>
        /// Finds end of block of same character. Returns index of last occurence of character in block.
        /// </summary>
        /// <param name="template">String to search</param>
        /// <param name="startOfBlock">Index of beginning of block</param>
        /// <returns>Returns index of last occurence of character in block.</returns>
        static private int FindEndOfBlock(string template, int startOfBlock)
        {
            int currentIdx = startOfBlock;
            char blockChar = template[startOfBlock];

            while (currentIdx < template.Length && 
                template[currentIdx] == blockChar)
            {
                currentIdx += 1;
            }

            return currentIdx-1;
        }
    }
}
