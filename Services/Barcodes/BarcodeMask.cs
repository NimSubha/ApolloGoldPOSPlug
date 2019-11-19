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
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.BarcodeService
{
    public class BarcodeMask
    {
        #region Member variables

        private string description;
        private string mask;
        private string prefix;
        private BarcodeType symbology;
        private BarcodeInternalType internalType;
        private string maskId;
        private DateTime timeStarted;
        private DateTime timeFinished;
        private TimeSpan timeElapsed;
        private bool found;

        private SqlConnection connection;
        private string DATAAREAID;

        #endregion

        #region Properties

        /// <summary>
        /// The mask description
        /// </summary>
        public string Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// A mask string
        /// </summary>
        public string Mask
        {
            get { return mask; }
            set { mask = value; }
        }

        /// <summary>
        /// The barcode prefix
        /// </summary>
        public string Prefix
        {
            get { return prefix; }
            set { prefix = value; }
        }

        /// <summary>
        /// The type of a barcode, i.e EAN 13,UPC-E.
        /// </summary>
        public BarcodeType Symbology
        {
            get { return symbology; }
            set { symbology = value; }
        }

        /// <summary>
        /// Is the barcode used for a customer,coupon etc.
        /// </summary>
        public BarcodeInternalType InternalType
        {
            get { return internalType; }
            set { internalType = value; }
        }

        /// <summary>
        /// A unique id for the mask.
        /// </summary>
        public string MaskId
        {
            get { return maskId; }
            set { maskId = value; }
        }

        /// <summary>
        /// The time when a BarcodeMask function was called.
        /// </summary>
        public DateTime TimeStarted
        {
            get { return timeStarted; }
            set { timeStarted = value; }
        }

        /// <summary>
        /// The time when a BarcodeMask function finished execution.
        /// </summary>
        public DateTime TimeFinished
        {
            get { return timeFinished; }
            set { timeFinished = value; }
        }

        /// <summary>
        /// The time that elapsed when executing a BarcodeMask function.
        /// </summary>
        public TimeSpan TimeElapsed
        {
            get { return timeElapsed; }
            set { timeElapsed = value; }
        }

        /// <summary>
        /// Set true if a BarcodeMask was found, else false.
        /// </summary>
        public bool Found
        {
            get { return found; }
            set { found = value; }
        }

        #endregion

        #region Ctor
        /// <summary>
        /// Construcutor.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="DATAAREAID"></param>
        public BarcodeMask(SqlConnection connection, string DATAAREAID)
        {
            this.connection = connection;
            this.DATAAREAID = DATAAREAID;
        }

        #endregion

        #region Methods

        /// <summary>
        /// A method to locate a mask for a barcode. Takes the barcode line entered. Variable "found" is true if found, else false.
        /// </summary>
        /// <param name="barcodeId">The barcode as it was entered or scanned.</param>
        internal void Find(string barcodeId)
        {
            timeStarted = DateTime.Now;
            found = false;
            string barcodePrefix = barcodeId.Substring(0, 1) + "%";
            string queryString = "SELECT LEN(PREFIX) AS LENGTH,* FROM RETAILBARCODEMASKTABLE WHERE PREFIX LIKE @PREFIX AND DATAAREAID = @DATAAREAID ORDER BY LENGTH DESC,MASK";

            try
            {

                using (SqlCommand command = new SqlCommand(queryString, connection))
                {
                    SqlParameter prefixParm = command.Parameters.Add("@PREFIX", SqlDbType.NVarChar, 22);
                    prefixParm.Value = barcodePrefix;
                    SqlParameter dataAreaIdParm = command.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
                    dataAreaIdParm.Value = DATAAREAID;

                    if (connection.State == ConnectionState.Closed)
                    {
                        connection.Open();
                    }

                    SqlDataReader reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        string readPrefix = reader.GetString(reader.GetOrdinal("PREFIX"));
                        if (barcodeId.Length >= readPrefix.Length)
                        {
                            barcodePrefix = barcodeId.Substring(0, readPrefix.Length);
                            if (readPrefix == barcodePrefix)
                            {
                                description = reader.GetString(reader.GetOrdinal("DESCRIPTION"));
                                mask = reader.GetString(reader.GetOrdinal("MASK"));
                                prefix = reader.GetString(reader.GetOrdinal("PREFIX"));
                                maskId = reader.GetString(reader.GetOrdinal("MASKID"));
                                symbology = (BarcodeType)reader.GetInt32(reader.GetOrdinal("SYMBOLOGY"));
                                internalType = (BarcodeInternalType)reader.GetInt32(reader.GetOrdinal("TYPE"));
                                timeFinished = DateTime.Now;
                                timeElapsed = timeFinished - timeStarted;

                                if (barcodeId.Length == mask.Length)
                                {
                                    found = true;
                                    break;
                                }
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

        #endregion
    }
}
