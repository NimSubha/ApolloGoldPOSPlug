using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class CustomerOrderDetails : Form
    {
        public int lineId = 0;
        public string index = string.Empty;
        public DataTable dtCustOrderDetails = new DataTable();
        string Idx = string.Empty;
        DataTable dtSubOrderDetails = new DataTable();
        public CustomerOrderDetails(DataSet dsCustOrder, string selIndex)
        {
            InitializeComponent();
            Idx = selIndex;
            dgvCustomerOrder.AutoGenerateColumns = false;
            dgvCustomerOrder.DataSource = dsCustOrder.Tables[0];
            if (dsCustOrder.Tables.Count > 1)
            {
                dtSubOrderDetails = dsCustOrder.Tables[1];
            }
        }



        #region Cell Double Click
        private void dgvCustomerOrder_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            dtCustOrderDetails = new DataTable();
            dtCustOrderDetails.Columns.Add("LINENUM", typeof(string));
            dtCustOrderDetails.Columns.Add("ITEMID", typeof(string));
            dtCustOrderDetails.Columns.Add("RATE", typeof(string));
            dtCustOrderDetails.Columns.Add("MAKINGRATE", typeof(string));
            dtCustOrderDetails.Columns.Add("INGREDIENTSAMOUNT", typeof(string));
            index = Convert.ToString(e.RowIndex);
            //  lineId = e.RowIndex + 1;
            lineId = Convert.ToInt16(dgvCustomerOrder.Rows[e.RowIndex].Cells[0].Value);
            dtCustOrderDetails.Rows.Add((dgvCustomerOrder.Rows[e.RowIndex].Cells[0].Value), (dgvCustomerOrder.Rows[e.RowIndex].Cells[1].Value), (dgvCustomerOrder.Rows[e.RowIndex].Cells[8].Value),
                                        (dgvCustomerOrder.Rows[e.RowIndex].Cells[9].Value), (dgvCustomerOrder.Rows[e.RowIndex].Cells[10].Value));
            this.Close();
        }
        #endregion

        #region Form Load
        private void CustomerOrderDetails_Load(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dgvCustomerOrder.Rows)
            {
                row.Cells["colExtendedDetails"].Value = "Details";
            }
            if (!string.IsNullOrEmpty(Idx))
                dgvCustomerOrder.Rows[Convert.ToInt16(Idx)].Selected = true;
        }
        #endregion

        #region Grid Cell Click
        private void dgvCustomerOrder_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvCustomerOrder.Columns["colExtendedDetails"].Index && e.RowIndex >= 0)
            {
                //   MessageBox.Show(Convert.ToString(dgvSchedularDetails.Rows[e.RowIndex].Cells["ScheduleOccurenceID"].Value));

                DataRow[] dr = dtSubOrderDetails.Select("ORDERDETAILNUM='" + Convert.ToString(dgvCustomerOrder.Rows[e.RowIndex].Cells[0].Value) + "'");
                if (dr.Length > 0)
                {
                    DataTable dt = new DataTable();
                    dt = dtSubOrderDetails.Clone();
                    foreach (DataRow row in dr)
                    {
                        dt.ImportRow(row);
                    }
                    CustomerSubOrderDetails oSubDetails = new CustomerSubOrderDetails(dt);
                    oSubDetails.ShowDialog();

                }
                else
                {
                    MessageBox.Show("NO SUB DETAILS PRESENT.", "WARNING ! ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

            }
        }
        #endregion

        #region Grid Enter Click
        private void dgvCustomerOrder_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            index = Convert.ToString(e.RowIndex);
            lineId = Convert.ToInt16(dgvCustomerOrder.Rows[e.RowIndex].Cells[0].Value);
            this.Close();
        }
        #endregion

        private void dgvCustomerOrder_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            index = Convert.ToString(e.RowIndex);
            lineId = Convert.ToInt16(dgvCustomerOrder.Rows[e.RowIndex].Cells[0].Value);
            this.Close();
        }


    }
}
