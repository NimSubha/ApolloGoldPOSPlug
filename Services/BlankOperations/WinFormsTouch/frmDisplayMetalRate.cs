using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmDisplayMetalRate : frmTouchBase
    {
        enum DisplayMetalType
        { 
            None        = 0,
            Gold        = 1,
            Silver      = 2,
            Platinum    = 3,
            Palladium   = 4,
        }


        public frmDisplayMetalRate()
        {
            InitializeComponent();

            cmbMetalType.DataSource = Enum.GetValues(typeof(DisplayMetalType));
            cmbMetalType.Focus();
            dtpDate.Value = DateTime.Now;
        }

        private void cmbMetalType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch(Convert.ToInt32(cmbMetalType.SelectedValue))
            {
                case 0:
                    grdMetalRate.DataSource = null;
                    break;
                case 1:
                    grdMetalRate.DataSource = null;
                    DisplayMetalRate(1);
                    break;
                case 2:
                    grdMetalRate.DataSource = null;
                    DisplayMetalRate(2);
                    break;
                case 3:
                    grdMetalRate.DataSource = null;
                    DisplayMetalRate(3);
                    break;
                case 4:
                    grdMetalRate.DataSource = null;
                    DisplayMetalRate(13);
                    break;
            }

        }
    
        private void DisplayMetalRate(int iMetalType)
        {

            SqlConnection conn = new SqlConnection();
            conn = ApplicationSettings.Database.LocalConnection;
            string storeId = LSRetailPosis.Settings.ApplicationSettings.Terminal.StoreId; 
            DataTable dt = new DataTable();

            string sQuery = " DECLARE @INVENTLOCATION VARCHAR(20) SELECT @INVENTLOCATION = RETAILCHANNELTABLE.INVENTLOCATION" +
                            " FROM   RETAILCHANNELTABLE  INNER JOIN RETAILSTORETABLE ON RETAILCHANNELTABLE.RECID = RETAILSTORETABLE.RECID" +
                            " WHERE STORENUMBER = '" + storeId + "'" +
                            " SELECT GS.RATES , 'Sale' AS RATETYPE,GS.CONFIGIDSTANDARD,TRANSDATE FROM (" +
                            " SELECT CAST(RATES AS NUMERIC (28,2)) AS RATES , RATETYPE,CONFIGIDSTANDARD " +
                            " ,ROW_NUMBER()OVER(PARTITION BY CONFIGIDSTANDARD ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC)as num,DATEADD(second, [TIME], [TRANSDATE]) TRANSDATE" +
                            " FROM METALRATES WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = " + iMetalType + "" +
                            " AND RATETYPE = 3 AND RETAIL = 1 AND ACTIVE=1  AND [TRANSDATE]='" + Convert.ToDateTime(dtpDate.Text).ToString("dd-MMM-yyyy") + "' )GS WHERE GS.NUM = 1" +
                            " union SELECT GS.RATES , 'Own OG Purchase' AS RATETYPE,GS.CONFIGIDSTANDARD,TRANSDATE FROM (" +
                            " SELECT CAST(RATES AS NUMERIC (28,2)) AS RATES ,RATETYPE,CONFIGIDSTANDARD " +
                            " ,ROW_NUMBER()OVER(PARTITION BY CONFIGIDSTANDARD ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC)as num,DATEADD(second, [TIME], [TRANSDATE]) TRANSDATE" +
                            " FROM METALRATES WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = " + iMetalType + "" +
                            " AND RATETYPE = 1 AND ACTIVE=1  AND [TRANSDATE]='" + Convert.ToDateTime(dtpDate.Text).ToString("dd-MMM-yyyy") + "')GS WHERE GS.NUM = 1" +
                            " union SELECT GS.RATES , 'Other OG Purchase' AS RATETYPE,GS.CONFIGIDSTANDARD,TRANSDATE FROM (" +
                            " SELECT CAST(RATES AS NUMERIC (28,2)) AS RATES ,RATETYPE,CONFIGIDSTANDARD " +
                            " ,ROW_NUMBER()OVER(PARTITION BY CONFIGIDSTANDARD ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC)as num,DATEADD(second, [TIME], [TRANSDATE]) TRANSDATE" +
                            " FROM METALRATES WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = " + iMetalType + "" +
                            " AND RATETYPE = 2 AND ACTIVE=1 AND [TRANSDATE]='" + Convert.ToDateTime(dtpDate.Text).ToString("dd-MMM-yyyy") + "')GS WHERE GS.NUM = 1" +
                            " union SELECT GS.RATES , 'GSS' AS RATETYPE, GS.CONFIGIDSTANDARD,TRANSDATE FROM (" +
                            " SELECT CAST(RATES AS NUMERIC (28,2)) AS RATES ,RATETYPE,CONFIGIDSTANDARD " +
                            " ,ROW_NUMBER()OVER(PARTITION BY CONFIGIDSTANDARD ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC)as num,DATEADD(second, [TIME], [TRANSDATE]) TRANSDATE" +
                            " FROM METALRATES WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = " + iMetalType + "" +
                            " AND RATETYPE = 4 AND ACTIVE=1 AND [TRANSDATE]='" + Convert.ToDateTime(dtpDate.Text).ToString("dd-MMM-yyyy") + "')GS WHERE GS.NUM = 1" +
                            " union SELECT GS.RATES ,'Own Exchange' AS RATETYPE, GS.CONFIGIDSTANDARD,TRANSDATE FROM (" +
                            " SELECT CAST(RATES AS NUMERIC (28,2)) AS RATES , METALTYPE,RATETYPE,CONFIGIDSTANDARD" +
                            " ,ROW_NUMBER()OVER(PARTITION BY CONFIGIDSTANDARD ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC)as num,DATEADD(second, [TIME], [TRANSDATE]) TRANSDATE" +
                            " FROM METALRATES WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = " + iMetalType + "" +
                            " AND RATETYPE = 6 AND ACTIVE=1 AND [TRANSDATE]='" + Convert.ToDateTime(dtpDate.Text).ToString("dd-MMM-yyyy") + "')GS WHERE GS.NUM = 1" +
                            " union SELECT GS.RATES ,'Other Exchange' AS RATETYPE, GS.CONFIGIDSTANDARD,TRANSDATE FROM (" +
                            " SELECT CAST(RATES AS NUMERIC (28,2)) AS RATES , METALTYPE,RATETYPE,CONFIGIDSTANDARD" +
                            " ,ROW_NUMBER()OVER(PARTITION BY CONFIGIDSTANDARD ORDER BY DATEADD(second, [TIME], [TRANSDATE]) DESC)as num,DATEADD(second, [TIME], [TRANSDATE]) TRANSDATE" +
                            " FROM METALRATES WHERE INVENTLOCATIONID = @INVENTLOCATION AND METALTYPE = " + iMetalType + "" +
                            " AND RATETYPE = 8 AND ACTIVE=1 AND [TRANSDATE]='" + Convert.ToDateTime(dtpDate.Text).ToString("dd-MMM-yyyy") + "')GS WHERE GS.NUM = 1 order by CONFIGIDSTANDARD";

            using (SqlCommand command = new SqlCommand(sQuery, conn))
            {
                    SqlDataAdapter da = new SqlDataAdapter(command);
                    da.Fill(dt);
            }

            if (dt != null && dt.Rows.Count > 0)
            {
                grdMetalRate.DataSource = dt;
            }

        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void dtpDate_ValueChanged(object sender, EventArgs e)
        {
            cmbMetalType_SelectedIndexChanged(sender, e);
        }
    }
}
