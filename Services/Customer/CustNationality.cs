using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using LSRetailPosis.Settings;

namespace Microsoft.Dynamics.Retail.Pos.Customer
{
    public class CustNationality
    {
        public string NationalityId { get; set; }
        public string Description { get; set; }
        public static IList<CustNationality> GetNatinalityList()
        {
            IList<CustNationality> list = new List<CustNationality>();
            DataTable dtNationality = GetData();
            foreach (DataRow dr in dtNationality.Rows)
            {
                CustNationality obj = new CustNationality();
                obj.Description = Convert.ToString(dr["Nationality"]);
                obj.NationalityId = Convert.ToString(dr["Country"]);//Country
                list.Add(obj);
            }
            return list;
        }
        public static DataTable GetData()
        {
            DataTable dtNationality = new DataTable();
            string sSqlstr = "select r.COUNTRYREGIONID AS [Code],t.SHORTNAME as [Name]  from LOGISTICSADDRESSCOUNTRYREGION  R " +
                             " left join  LOGISTICSADDRESSCOUNTRYREGIONTRANSLATION T on r.COUNTRYREGIONID =t.COUNTRYREGIONID " +
                             " where t.LANGUAGEID ='" + ApplicationSettings.Terminal.CultureName + "' order by t.SHORTNAME";
            return GetDataTable(sSqlstr);
        }
        private static DataTable GetDataTable(string sSQL)
        {
            try
            {
                SqlConnection SqlCon = new SqlConnection(ApplicationSettings.Database.LocalConnectionString);
                SqlCon.Open();

                SqlCommand SqlComm = new SqlCommand();
                SqlComm.Connection = SqlCon;
                SqlComm.CommandType = CommandType.Text;

                SqlComm.CommandText = sSQL;// "SELECT REPAIRID AS [Repair No],OrderDate as [Order Date],CUSTACCOUNT AS [Customer Account], CustName as [Customer Name] from RetailRepairReturnHdr where CUSTACCOUNT = '" + sCustAccount + "' AND IsDelivered = 0";

                DataTable dt = new DataTable();
                SqlDataAdapter SqlDa = new SqlDataAdapter(SqlComm);
                SqlDa.Fill(dt);

                return dt;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }
}
