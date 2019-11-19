/*
SAMPLE CODE NOTICE

THIS SAMPLE CODE IS MADE AVAILABLE AS IS.  MICROSOFT MAKES NO WARRANTIES, WHETHER EXPRESS OR IMPLIED, 
OF FITNESS FOR A PARTICULAR PURPOSE, OF ACCURACY OR COMPLETENESS OF RESPONSES, OF RESULTS, OR CONDITIONS OF MERCHANTABILITY.  
THE ENTIRE RISK OF THE USE OR THE RESULTS FROM THE USE OF THIS SAMPLE CODE REMAINS WITH THE USER.  
NO TECHNICAL SUPPORT IS PROVIDED.  YOU MAY NOT DISTRIBUTE THIS CODE UNLESS YOU HAVE A LICENSE AGREEMENT WITH MICROSOFT THAT ALLOWS YOU TO DO SO.
*/

using System;
using System.ComponentModel.Composition;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;
using LSRetailPosis.POSProcesses;
using LSRetailPosis.Transaction;
using Microsoft.Dynamics.Retail.Diagnostics;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using Microsoft.Dynamics.Retail.Pos.Contracts.DataEntity;
using Microsoft.Dynamics.Retail.Pos.Contracts.Services;

namespace Microsoft.Dynamics.Retail.Pos.TenderRestriction
{
    [Export(typeof(ITenderRestriction))]
    public class TenderRestriction : ITenderRestriction
    {
        // Get all text through the Translation function in the ApplicationLocalizer
        //
        // TextID's for TenderRestriction are reserved at 50250 - 50299
        // In use now are ID's: 50252

        #region Properties

        [Import]
        public IApplication Application { get; set; }

        #endregion

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Grandfather")]
        public enum ItemStatus
        {
            EXCLUDE = 0,
            INCLUDE = 1
        }

        [SuppressMessage("Microsoft.Design", "CA1034:NestedTypesShouldNotBeVisible", Justification = "Grandfather")]
        public enum RestrictionType
        {
            Item = 0,
            Department = 1
        }

        public decimal FindTenderRestriction(IRetailTransaction retailTransaction, ICardInfo cardInfo)
        {
            NetTracer.Information("TenderRestriction::FindTenderRestriction - Start");
            RetailTransaction transaction = retailTransaction as RetailTransaction;
            if (transaction == null)
            {
                throw new ArgumentNullException("retailTransaction");
            }

            if (cardInfo == null)
            {
                throw new ArgumentNullException("cardInfo");
            }

            decimal payableAmount = 0; //the return value of total amount payd with tender id
            ItemStatus itemStatus;     //is the item included / excluded by the tender id

            // Check if there are items in the transaction
            if (transaction.SaleItems.Count == 0)
            {
                POSFormsManager.ShowPOSMessageDialog(50251); //"There are no sales items to check."
                return payableAmount;
            }

            // Calclulating how much amount of the original amount can be paid.
            foreach (ISaleLineItem lineItem in transaction.SaleItems)
            {
                if (string.IsNullOrEmpty(lineItem.TenderRestrictionId) && (!lineItem.Voided))
                {
                    itemStatus = CheckTenderRestriction(cardInfo.RestrictionCode, cardInfo.TenderTypeId, lineItem.ItemId, lineItem.ItemGroupId);

                    if (itemStatus == ItemStatus.INCLUDE)
                    {
                        lineItem.TenderRestrictionId = cardInfo.RestrictionCode;
                        lineItem.FleetCardNumber = cardInfo.CardNumber;
                        lineItem.PaymentIndex = transaction.TenderLines.Count;
                        payableAmount += lineItem.NetAmountWithTax;
                    }
                }
            }

            if (payableAmount != retailTransaction.NetAmountWithTax)
            {
                // If nothing can be paid, then it can be concluded that this card is prohibited
                if (payableAmount == 0)
                {
                    using (LSRetailPosis.POSProcesses.frmMessage dialog = new LSRetailPosis.POSProcesses.frmMessage(50253))
                    {
                        this.Application.ApplicationFramework.POSShowForm(dialog);
                    }
                }
                else
                {
                    string message = string.Format(
                        LSRetailPosis.ApplicationLocalizer.Language.Translate(50151),
                        this.Application.Services.Rounding.Round(payableAmount, false));

                    using (frmTenderRestriction frmExcluded = new frmTenderRestriction(transaction))
                    {
                        frmExcluded.DisplayMsg = message;
                        this.Application.ApplicationFramework.POSShowForm(frmExcluded);

                        if (frmExcluded.DialogResult == DialogResult.No)
                        {
                            ClearTenderRestriction(retailTransaction);
                            return 0;
                        }
                    }
                }
            }

            NetTracer.Information("TenderRestriction::FindTenderRestriction - End");
            return payableAmount;
        }

        public void ClearTenderRestriction(IRetailTransaction retailTransaction)
        {
            NetTracer.Information("TenderRestriction::ClearTenderRestriction - Start");
            RetailTransaction transaction = retailTransaction as RetailTransaction;
            if (transaction == null)
                throw new ArgumentNullException("retailTransaction");

            for (int i = 0; i < transaction.SaleItems.Count; i++)
            {
                ISaleLineItem lineItem = transaction.GetItem(i + 1);

                lineItem.TenderRestrictionId = string.Empty;
                lineItem.FleetCardNumber = string.Empty;
                lineItem.PaymentIndex = -1;
            }
            NetTracer.Information("TenderRestriction::ClearTenderRestriction - End");
        }

        private ItemStatus CheckTenderRestriction(string restrictionId, string tenderId, string itemId, string groupId)
        {
            if (string.IsNullOrEmpty(restrictionId))
            {
                return ItemStatus.INCLUDE;
            }

            // Check if the item has a tender restriction.
            string itemRestr = " SELECT TOP 1 INCLUDE " +
                                 " FROM POSISTENDERRESTRICTIONS " +
                                 " WHERE (RESTRICTIONID = @RESTRICTIONID) " +
                                 " AND (TENDERID = @TENDERID) " +
                                 " AND ( " +
                                 "            ITEMORGROUPID = 'ALL' " +
                                 "        OR (ITEMORGROUPID = @ITEMID AND TYPE = @TYPE) " +
                                 "      ) " +
                                 " AND DATAAREAID = @DATAAREAID ";

            // Check if the group has a tender restriction.
            string groupRestr = " SELECT TOP 1 INCLUDE " +
                                 " FROM POSISTENDERRESTRICTIONS " +
                                 " WHERE (RESTRICTIONID = @RESTRICTIONID) " +
                                 " AND (TENDERID = @TENDERID) " +
                                 " AND ( " +
                                 "            ITEMORGROUPID = 'ALL' " +
                                 "        OR (ITEMORGROUPID = @GROUPID AND TYPE = @GROUPTYPE) " +
                                 "      ) " +
                                 " AND DATAAREAID = @DATAAREAID ";

            SqlConnection connection = Application.Settings.Database.Connection;
            try
            {
                using (SqlCommand itemCommand = new SqlCommand(itemRestr, connection))
                {
                    SqlParameter restrictionIdParam = itemCommand.Parameters.Add("@RESTRICTIONID", SqlDbType.NVarChar);
                    restrictionIdParam.Value = restrictionId;
                    SqlParameter tenderIdParam = itemCommand.Parameters.Add("@TENDERID", SqlDbType.NVarChar);
                    tenderIdParam.Value = tenderId;
                    SqlParameter itemIdParam = itemCommand.Parameters.Add("@ITEMID", SqlDbType.NVarChar, 20);
                    itemIdParam.Value = itemId;
                    SqlParameter typeParam = itemCommand.Parameters.Add("@TYPE", SqlDbType.Int);
                    typeParam.Value = (int)RestrictionType.Item;
                    SqlParameter dataAreaIdParm = itemCommand.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
                    dataAreaIdParm.Value = Application.Settings.Database.DataAreaID;

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    SqlDataReader reader = itemCommand.ExecuteReader(CommandBehavior.SingleRow);
                    reader.Read();
                    if (reader.HasRows)
                    {
                        int retrn = reader.GetInt32(reader.GetOrdinal("INCLUDE"));
                        return (ItemStatus)retrn;
                    }
                }

                using (SqlCommand groupCommand = new SqlCommand(groupRestr, connection))
                {
                    SqlParameter restrictionIdParamGr = groupCommand.Parameters.Add("@RESTRICTIONID", SqlDbType.NVarChar);
                    restrictionIdParamGr.Value = restrictionId;
                    SqlParameter tenderIdParamGr = groupCommand.Parameters.Add("@TENDERID", SqlDbType.NVarChar);
                    tenderIdParamGr.Value = tenderId;
                    SqlParameter groupIdParamGr = groupCommand.Parameters.Add("@GROUPID", SqlDbType.NVarChar, 20);
                    groupIdParamGr.Value = groupId;
                    SqlParameter groupTypeParam = groupCommand.Parameters.Add("@GROUPTYPE", SqlDbType.Int);
                    groupTypeParam.Value = (int)RestrictionType.Department;
                    SqlParameter dataAreaIdParmGr = groupCommand.Parameters.Add("@DATAAREAID", SqlDbType.NVarChar, 4);
                    dataAreaIdParmGr.Value = Application.Settings.Database.DataAreaID;

                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    SqlDataReader readerGroup = groupCommand.ExecuteReader(CommandBehavior.SingleRow);
                    readerGroup.Read();
                    if (readerGroup.HasRows)
                    {
                        int retrn = readerGroup.GetInt32(readerGroup.GetOrdinal("INCLUDE"));
                        return (ItemStatus)retrn;
                    }

                    return ItemStatus.EXCLUDE;
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
    }
}
