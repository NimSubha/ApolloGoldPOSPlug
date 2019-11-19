using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace Enums
{
    public class EnumClass
    {
        List<string> returnLineNum = new List<string>();

        #region TransactionType
        public enum TransactionType
        {
            Sale = 0,
            Purchase = 1,
            Exchange = 3,
            PurchaseReturn = 2,
            ExchangeReturn = 4,
            Adjustment=5,
        }
        #endregion

        #region enum  CalcType
        public enum CalcType
        {
            Weight = 0,
            Pieces = 1,
            Tot = 2,
        }
        #endregion

        #region enum  MakingType
        public enum MakingType
        {
            Weight = 2,
            Pieces = 0,
            Tot = 3,
            Percentage = 4,
        }
        #endregion


        #region enum DepositType
        public enum DepositType
        {
            GSS = 0,
            Normal = 1,
        }
        #endregion

        #region enum  RateType
        public enum RateType
        {
            Purchase = 0,
            OGP = 1,
            OGOP = 2,
            Sale = 3,
            GSS = 4,
            Exchange = 6,
            OtherExchange = 8,
        }
        #endregion

        #region enum MetalType
        public enum MetalType
        {
            Other = 0,
            Gold = 1,
            Silver = 2,
            Platinum = 3,
            Alloy = 4,
            Diamond = 5,
            Pearl = 6,
            Stone = 7,
            Consumables = 8,
            Watch = 11,
            LooseDmd = 12,
            Palladium = 13,
            Jewellery = 14,
            Metal=15,
            PackingMaterial=16,
            CertificateDmd=17,
            GiftVoucher = 18,
        }
        #endregion

        #region enum LType
        public enum LType
        {
            Main = 0,
            Sub = 1,

        }
        #endregion

        #region enum InventoryTransactionType
        public enum InventoryTransactionType
        {
            Void = 0,
            Sale = 1,
            Return = 2

        }
        #endregion

        #region enum CRWRetailDiscPermission
        public enum CRWRetailDiscPermission // added on 29/08/2014
        {
            Cashier = 0,
            Salesperson = 1,
            Manager = 2,
            Other = 3,
        }
        #endregion
        
        public List<string> storeLineNum(string lineNum)
        {
            if (returnLineNum != null)
            {
                returnLineNum.Add(lineNum);
                return returnLineNum;
            }
            else
            {
                return null;
            }
        }

      //  public string ValidateMinDeposit(SqlConnection connection, out string MaxAmount)
        public string ValidateMinDeposit(SqlConnection connection, out string MaxAmount, string sTerminalId, decimal dOrdAmt)// RETAILTEMPTABLE
        {
            string commandText = " DECLARE @MINDEPOSITPCT NUMERIC(28,16) " +
                            " SELECT @MINDEPOSITPCT=ISNULL(MINIMUMDEPOSITFORCUSTORDER,0) from RETAILPARAMETERS " +// changes on 28/12/2015
                            // " SELECT ISNULL(((MINIMUMDEPOSITFORCUSTORDER/100)*@MINDEPOSITPCT),0),MINIMUMDEPOSITFORCUSTORDER FROM RETAILTEMPTABLE WHERE ID=1 ";
                            " SELECT ISNULL(((MINIMUMDEPOSITFORCUSTORDER/100)*@MINDEPOSITPCT),0),MINIMUMDEPOSITFORCUSTORDER FROM RETAILTEMPTABLE WHERE ID=1 AND TERMINALID = '" + sTerminalId + "' "; // RETAILTEMPTABLE


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            SqlCommand command = new SqlCommand(commandText, connection);
            SqlDataReader reader = null;
            reader = command.ExecuteReader();
            string sMinAmount = string.Empty;
            string sMaxAmount = string.Empty;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sMinAmount = Convert.ToString(reader.GetValue(0));
                    sMaxAmount = Convert.ToString(reader.GetValue(1));
                    MaxAmount = sMaxAmount;
                }
                reader.Close();

            }
            else
            {
                reader.Close();
                sMinAmount = "0";
                sMaxAmount = "0";
            }
            connection.Close();
            MaxAmount = sMaxAmount;
            return sMinAmount;
        }

        #region Customer Advance Book Qty & Days ---Soutik
        public string CustomerAdvValidateMinDeposit(SqlConnection connection, out string MaxAmount, string sTerminalId, decimal dOrdAmt)// RETAILTEMPTABLE
        {
            string commandText = " DECLARE @MINADVBOOKQTYPER NUMERIC(28,16) " +
                            " SELECT @MINADVBOOKQTYPER=ISNULL(ADVBOOKQTYPER,0) from RETAILPARAMETERS " +// changes on 28/12/2015
                            " SELECT ISNULL(((MINIMUMDEPOSITFORCUSTORDER/100)*@MINADVBOOKQTYPER),0),MINIMUMDEPOSITFORCUSTORDER FROM RETAILTEMPTABLE WHERE ID=1 AND TERMINALID = '" + sTerminalId + "' "; // RETAILTEMPTABLE


            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }

            SqlCommand command = new SqlCommand(commandText, connection);
            SqlDataReader reader = null;
            reader = command.ExecuteReader();
            string sMinAmount = string.Empty;
            string sMaxAmount = string.Empty;
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    sMinAmount = Convert.ToString(reader.GetValue(0));
                    sMaxAmount = Convert.ToString(reader.GetValue(1));
                    MaxAmount = sMaxAmount;
                }
                reader.Close();

            }
            else
            {
                reader.Close();
                sMinAmount = "0";
                sMaxAmount = "0";
            }
            connection.Close();
            MaxAmount = sMaxAmount;
            return sMinAmount;
        }
        #endregion
    }
}
