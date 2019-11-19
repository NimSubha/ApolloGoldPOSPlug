using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Windows.Forms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    class Helper
    {
        #region enum  RateType
        enum RateType
        {
            Weight = 0,
            Pieces = 1,
            Tot = 2,
        }
        #endregion

        #region enum  MakingType
        enum MakingType
        {
            Weight = 2,
            Pieces = 0,
            Tot = 3,
            Percentage = 4,
        }
        #endregion

        public string ConvertEnumToDataTable(string colName, string EnumType, string Value, DataTable dtOrder = null)
        {
            DataTable dt = new DataTable();
            //  dt.Columns.Add(colName, typeof(string));

            if (EnumType.ToUpper().Trim() == "RATETYPE")
            {
                dt.Columns.Add(colName, typeof(string));
                dt.Rows.Add(RateType.Weight);
                dt.Rows.Add(RateType.Pieces);
                dt.Rows.Add(RateType.Tot);
            }
            else if (EnumType.ToUpper().Trim() == "MAKINGTYPE")
            {
                dt.Columns.Add(colName, typeof(string));
                dt.Rows.Add(MakingType.Weight);
                dt.Rows.Add(MakingType.Pieces);
                dt.Rows.Add(MakingType.Tot);
                dt.Rows.Add(MakingType.Percentage);
            }
            else
            {
                dt = dtOrder;
            }

            Dialog.WinFormsTouch.frmGenericLookup oLookup = new Dialog.WinFormsTouch.frmGenericLookup(dt, 0, colName);
            oLookup.ShowDialog();
            DataRow dr = oLookup.SelectedDataRow;
            if (dr != null)
                return Convert.ToString(dr[colName]);
            else
                return Value;

        }

        public bool ValidateKeyPress(object sender, KeyPressEventArgs e, int type)
        {
            if (type == 0)
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    return true;
                }
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    return true;

                }
                return false;
            }
            else
            {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
                {
                    return true;
                }

                if (e.KeyChar == '.' && (sender as TextBox).Text.IndexOf('.') > -1)
                {
                    return true;
                }
                if (e.KeyChar == (Char)Keys.Enter)
                {
                    return true;

                }
                return false;
            }
        }
    }
}
