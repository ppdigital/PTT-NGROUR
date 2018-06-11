using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace PTT_NGROUR.Controllers
{
    public class SQLServerQuery 
    {
        public string conn = ConfigurationManager.ConnectionStrings["SQLDatabase"].ConnectionString;
        public SqlConnection SqlConn = new SqlConnection();

        public DataTable LoadPIS(string schtext, string pisincs)
        {
            string condi = "";
            if (!string.IsNullOrEmpty(pisincs))
                condi = " and a.[code] not in (" + pisincs + ")";
            SqlConn.ConnectionString = conn;
            DataSet dataset = new DataSet();
            SqlDataAdapter adapter = new SqlDataAdapter();
            adapter.SelectCommand = new SqlCommand(@"select * from( SELECT  b.[code] as P_ID
                  ,[FULLNAMETH]
                  ,[mobile]
                     ,[email]
                  ,b.POSNAME
                  ,c.unitname,[unitabbr],FULLNAMETH +'('+[unitabbr]+')' as name_fk
                  ,FULLNAMETH +' ' + [unitabbr]+' '+unitname+' '+POSNAME+' '+b.CODE as keyword
                FROM [PIS].[dbo].[personel_info] b
                left join [PIS].[dbo].[unit] c on b.UNITCODE=c.unitcode
                left join [PIS].[dbo].[directory_info] a on a.code=b.CODE
                where FULLNAMETH is not null    " +
                condi
                + ")a where a.keyword like '%" + schtext + "%' "
                + " order by [FULLNAMETH]", SqlConn);
            SqlConn.Open();
            adapter.Fill(dataset);
            SqlConn.Close();
            return dataset.Tables[0];
        }


        public DataTable LoadINPIS(string schtext, string pisincs)
        {
            {
                string condi = "";
                DataSet dataset = new DataSet();
                if (!string.IsNullOrEmpty(pisincs))
                {
                    condi = " and b.[code]  in (" + pisincs + ")";
                    SqlConn.ConnectionString = conn;

                    SqlDataAdapter adapter = new SqlDataAdapter();
                    adapter.SelectCommand = new SqlCommand(@"select * from( SELECT  b.[code] as P_ID
                  ,[FULLNAMETH]
                  ,[mobile]
                   ,[email]
                  ,b.POSNAME
                  ,c.unitname,[unitabbr],FULLNAMETH +'('+[unitabbr]+')' as name_fk
                  ,FULLNAMETH +' ' + [unitabbr]+' '+unitname+' '+POSNAME+' '+b.CODE as keyword
                  FROM [PIS].[dbo].[personel_info] b
                  left join [PIS].[dbo].[unit] c on b.UNITCODE=c.unitcode
                  left join [PIS].[dbo].[directory_info] a on a.code=b.CODE
                  where FULLNAMETH is not null    " + condi
                     + ")a where a.keyword like '%" + schtext + "%' "
                    + " order by [FULLNAMETH]", SqlConn);
                    SqlConn.Open();
                    adapter.Fill(dataset);
                    SqlConn.Close();
                    return dataset.Tables[0];
                }
                else
                {
                    return null;
                }
            }
        }

        public DataTable SearchInPIS(string temp1, string temp2)
        {
            if (!string.IsNullOrEmpty(temp1) && !string.IsNullOrEmpty(temp2))
            {
                SqlConn.ConnectionString = conn;
                DataTable searchPIS = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(@"                               
                select CODE AS รหัสพนักงาน,FNAME AS ชื่อ,LNAME AS นามสกุล,EmailAddr AS อีเมล์ from [PIS].[dbo].[personel_info] where [FNAME] like '%" + temp1 + "%' and [LNAME]= '%" + temp2 + "%'" +
                " or [FNAME_ENG] like '%" + temp1 + "%' and [LNAME_ENG] like '%" + temp2 + "%'"
                , SqlConn);
                SqlConn.Open();
                adapter.Fill(searchPIS);
                SqlConn.Close();
                return searchPIS;
            }
            else if (!string.IsNullOrEmpty(temp1))
            {
                SqlConn.ConnectionString = conn;
                DataTable searchPIS = new DataTable();
                SqlDataAdapter adapter = new SqlDataAdapter();
                adapter.SelectCommand = new SqlCommand(@"
                  select CODE AS รหัสพนักงาน,FNAME AS ชื่อ,LNAME AS นามสกุล,EmailAddr AS อีเมล์  from [PIS].[dbo].[personel_info] where CODE like '%" + temp1 + "%'" +
                  " or FNAME like '%" + temp1 + "%' or LNAME like '%" + temp1 + "%' or LNAME_ENG like '%" + temp1 + "%' or FNAME_ENG like '%" + temp1 + "%'" +
                  " order by CODE", SqlConn);
                SqlConn.Open();
                adapter.Fill(searchPIS);
                SqlConn.Close();
                return searchPIS;
            }
            else
            {
                return null;
            }
        }

    }
}
