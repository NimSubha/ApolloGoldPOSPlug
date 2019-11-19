using System;
using System.Data;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmGeneralSearch: frmTouchBase
    {
        DataTable dtSCList = new DataTable();
        private String selectedChannel;

        public String SelectedChannel 
        {
            get { return selectedChannel; } 
        }

        public frmGeneralSearch()
        {
            InitializeComponent();
        }
        public frmGeneralSearch(DataTable  dtList)
        {
            InitializeComponent();
            dtSCList = dtList;
            grItems.DataSource = dtSCList;
        }

        private void SelectCustomer()
        {
            if(grdView.RowCount > 0)
            {
                int selectedIndex = grdView.GetSelectedRows()[0];

                if(dtSCList.Rows.Count > 0)
                {
                    DataRow theRowToSelect = dtSCList.Rows[selectedIndex];
                    selectedChannel = Convert.ToString(theRowToSelect["CHANNELTYPE"]);
                    
                    this.Close();
                }
            }
            else
            {
                this.Close();
            }
        }

        private void grdView_DoubleClick(object sender, EventArgs e)
        {
            SelectCustomer();
        }

        private void btnUp_Click(object sender, EventArgs e)
        {
            grdView.MovePrev();
        }

        private void btnPgUp_Click(object sender, EventArgs e)
        {
            grdView.MovePrevPage();
        }

        private void btnSelect_Click(object sender, EventArgs e)
        {
            SelectCustomer();
        }

        private void btnPgDown_Click(object sender, EventArgs e)
        {
            grdView.MoveNextPage();
        }

        private void btnDown_Click(object sender, EventArgs e)
        {
            grdView.MoveNext();
        }
    }
}
