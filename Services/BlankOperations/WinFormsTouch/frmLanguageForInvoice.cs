using System;
using System.Data.SqlClient;
using LSRetailPosis.Settings;
using System.Text;
using System.Data;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations.WinFormsTouch
{
    public partial class frmLanguageForInvoice : frmTouchBase
    {

        public bool isEnglish = false;
        public bool isArabic = false;
        public bool isBoth = false;

        public frmLanguageForInvoice()
        {
            InitializeComponent();
            int isArabicIn = 0;
            int isBothIn = 0;
            bool bOnlyEng = IsEngInv(ref isArabicIn, ref isBothIn);

            if (bOnlyEng)
                btnEnglish.Visible = true;
            else
                btnEnglish.Visible = false;

            if (isArabicIn == 1)
                btnArabic.Visible = true;
            else
                btnArabic.Visible = false;

            if (isBothIn == 1)
                btnBoth.Visible = true;
            else
                btnBoth.Visible = false;
        }

        private void btnEnglish_Click(object sender, EventArgs e)
        {
            isEnglish = true;
            this.Close();
        }

        private void btnArabic_Click(object sender, EventArgs e)
        {
            isArabic = true;
            this.Close();
        }

        private void btnBoth_Click(object sender, EventArgs e)
        {
            isBoth = true;
            this.Close();
        }

        protected bool IsEngInv(ref int isAr, ref int isBoth)
        {
            bool bResult = false;
            SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            StringBuilder commandText = new StringBuilder();
            commandText.Append("select isnull(ENGLISHVOUCHERPRINT,0),isnull(ARABICVOUCHERPRINT,0) ,ISNULL(BOTHVOUCHERPRINT,0) from RETAILPARAMETERS where  DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "'");

            if (SqlCon.State == ConnectionState.Closed)
                SqlCon.Open();

            SqlCommand command = new SqlCommand(commandText.ToString(), SqlCon);
            command.CommandTimeout = 0;
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows)
            {
                while (reader.Read())
                {
                    bResult = Convert.ToBoolean((int)reader.GetValue(0));
                    isAr = (int)reader.GetValue(1);
                    isBoth = (int)reader.GetValue(2);
                }
            }
            if (SqlCon.State == ConnectionState.Open)
                SqlCon.Close();
            return bResult;

        }
    }
}
