using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.ComponentModel.Composition;

using Microsoft.Dynamics.Retail.Pos.Contracts.UI;
using LSRetailPosis.Transaction;
using System.Data.SqlClient;
using Microsoft.Dynamics.Retail.Pos.Contracts;
using LSRetailPosis.Settings;
using System.Globalization;

namespace Microsoft.Dynamics.Retail.Pos.BlankOperations
{
    [Export(typeof(IPosCustomControl))]
    [PartCreationPolicy(CreationPolicy.NonShared)]


    public partial class NIM_HomeControl : UserControl, IPosCustomControl
    {
        [Import]
        private IApplication application;

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
            Metal = 15,
            PackingMaterial = 16,
            Certificate = 17,
            GiftVoucher = 18,
        }
        #endregion

        public NIM_HomeControl()
        {
            InitializeComponent();
        }

        public void LoadLayout(string layoutId)
        {
            timer1.Interval = 1000;
            timer1.Enabled = true;
            timer1.Start();
        }

        public void TransactionChanged(Contracts.DataEntity.IPosTransaction transaction)
        {
            RetailTransaction retailTransaction = transaction as RetailTransaction;
            if (retailTransaction != null)
                lblCustTransDateTime.Text = retailTransaction.Customer.CustomerId;
            else
                lblCustTransDateTime.Text = "";

        }

        private void ShowCurrentRate()
        {
            #region OLd
            //DataTable dtRate = new DataTable();
            //SqlCommand SqlComm = new SqlCommand();

            //SqlComm.CommandType = CommandType.Text;

            //SqlComm.CommandText = "SELECT top (1) RATES,CONFIGIDSTANDARD FROM METALRATES WHERE CONFIGIDSTANDARD" +
            //                    " =(select top 1 DEFAULTCONFIGIDGOLD from INVENTPARAMETERS)" +
            //                    " and MetalType=" + (int)MetalType.Gold + "" +
            //                    " and RateType= " + (int)Enums.EnumClass.RateType.Sale + "" +
            //                    " and INVENTLOCATIONID='" + ApplicationSettings.Database.StoreID +"' " +
            //                    " and Active = 1 order by TRANSDATE desc, [Time] Desc ";

            //SqlConnection connection = new SqlConnection();

            //if (application != null)
            //    connection = application.Settings.Database.Connection;
            //else
            //    connection = ApplicationSettings.Database.LocalConnection;

            //if (connection.State == ConnectionState.Closed)
            //    connection.Open();

            //SqlComm.Connection = connection;

            //SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
            //SqlDa.Fill(dtRate);

            //if (dtRate != null && dtRate.Rows.Count > 0)
            //{
            //    lblLatestRate.Text = " Today's Gold Rate" + " (" + Convert.ToString(dtRate.Rows[0]["CONFIGIDSTANDARD"]) + ")"
            //        + System.Environment.NewLine + "           " +
            //         Convert.ToString(decimal.Round((Convert.ToDecimal(Convert.ToString(dtRate.Rows[0]["RATES"]))), 2, MidpointRounding.AwayFromZero));

            //}
            #endregion

            GetMetalRates();
        }

        private void GetMetalRates() // added on 31/03/2014 RHossain for  show the tax detail in line
        {
            DataTable dTMetalRates = new DataTable();

            SqlConnection connection = new SqlConnection();
            // string sMetalRates = "";

            if (application != null)
                connection = application.Settings.Database.Connection;
            else
                connection = ApplicationSettings.Database.LocalConnection;

            if (connection.State == ConnectionState.Closed)
                connection.Open();

            //SqlTransaction sqlTrans =  connection.BeginTransaction();

            string sQuery = "GETMETALRATES";
            SqlCommand command1 = new SqlCommand(sQuery, connection);
            command1.CommandType = CommandType.StoredProcedure;
            command1.CommandTimeout = 0;
            command1.Parameters.Clear();
            command1.Parameters.Add("@STOREID", SqlDbType.NVarChar).Value = ApplicationSettings.Terminal.StoreId;
            command1.Parameters.Add("@P_EXECSTATUS", SqlDbType.Int).Value = 0;

            SqlDataAdapter daCol = new SqlDataAdapter(command1);
            daCol.Fill(dTMetalRates);


            if (connection.State == ConnectionState.Closed)
                connection.Close();


            DataTable dt = GetConfigListForRateShow();

            if (dTMetalRates != null && dTMetalRates.Rows.Count > 0)
            {
                for (int i = 1; i <= dTMetalRates.Rows.Count; i++)
                {
                    //    if(i == 1)
                    //        sMetalRates = Convert.ToString(dTMetalRates.Rows[i - 1]["CONFIGIDSTANDARD"]) + " Rates : " + Convert.ToString(dTMetalRates.Rows[i - 1]["RATES"]);
                    //    else
                    //        sMetalRates = sMetalRates + Environment.NewLine + Convert.ToString(dTMetalRates.Rows[i - 1]["CONFIGIDSTANDARD"]) + " Rates : " + Convert.ToString(dTMetalRates.Rows[i - 1]["RATES"]);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        foreach (DataRow row in dt.Rows)
                        {
                            if (Convert.ToString(dTMetalRates.Rows[i - 1]["CONFIGIDSTANDARD"]) == Convert.ToString(row["CONFIG"]))
                            {
                                //if(i == 1)
                                //if (lblDate.Text == "l1")
                                string sDC = GetDefaultConfigId();

                                if (sDC == Convert.ToString(row["CONFIG"]))
                                    lblDate.Text = Convert.ToString(dTMetalRates.Rows[i - 1]["TIMEA"]);

                                if (Convert.ToInt16(row["SEQUENCE"]) == 1)
                                {
                                    lblC1.Text = Convert.ToString(row["CONFIG"]) + ":";
                                    lblCR1.Text = Convert.ToString(dTMetalRates.Rows[i - 1]["RATES"]);
                                }

                                if (Convert.ToInt16(row["SEQUENCE"]) == 2)
                                {
                                    lblC2.Text = Convert.ToString(row["CONFIG"]) + ":";
                                    lblCR2.Text = Convert.ToString(dTMetalRates.Rows[i - 1]["RATES"]);
                                }

                                if (Convert.ToInt16(row["SEQUENCE"]) == 3)
                                {
                                    lblC3.Text = Convert.ToString(row["CONFIG"]) + ":";
                                    lblCR3.Text = Convert.ToString(dTMetalRates.Rows[i - 1]["RATES"]);
                                }

                                if (Convert.ToInt16(row["SEQUENCE"]) == 4)
                                {
                                    lblC4.Text = Convert.ToString(row["CONFIG"]) + ":";
                                    lblCR4.Text = Convert.ToString(dTMetalRates.Rows[i - 1]["RATES"]);
                                }

                            }
                        }
                    }

                    //DateTime localDate = DateTime.Now;

                    //var culture = new CultureInfo(ApplicationSettings.Terminal.CultureName);
                    //lblDate.Text = localDate.ToString(culture);
                }
            }

            //lblC1.Text = sMetalRates;
        }

        
        private void timer1_Tick(object sender, EventArgs e)
        {
            ShowCurrentRate();
        }


        private DataTable GetConfigListForRateShow()
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                if (SqlCon.State == ConnectionState.Closed)
                    SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;
                SqlComm.CommandText = "SELECT isnull(SEQUENCE,0) SEQUENCE ,isnull(CONFIG,'') CONFIG FROM RATESHOWCONFIGURATION ";

                DataTable CustBalDt = new DataTable();

                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(CustBalDt);

                if (SqlCon.State == ConnectionState.Open)
                    SqlCon.Close();

                return CustBalDt;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private string GetDefaultConfigId()
        {
            string storeId = string.Empty;
            SqlConnection conn = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);

            storeId = ApplicationSettings.Database.StoreID;
            StringBuilder commandText = new StringBuilder();

            commandText.Append(" SELECT isnull(DEFAULTCONFIGIDGOLD,'') FROM [RETAILPARAMETERS] WHERE DATAAREAID='" + ApplicationSettings.Database.DATAAREAID + "' ");


            if (conn.State == ConnectionState.Closed)
                conn.Open();
            SqlCommand command = new SqlCommand(commandText.ToString(), conn);
            command.CommandTimeout = 0;
            string sResult = Convert.ToString(command.ExecuteScalar());

            if (conn.State == ConnectionState.Open)
                conn.Close();

            return Convert.ToString(sResult.Trim());
        }
    }
}
