using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web;
using System.DirectoryServices;

namespace PTT_NGROUR.Models
{
    public class UserManagement
    {
        DAL.DAL dal_user = new DAL.DAL();
        System.Data.DataSet dataset;

        public UserManagement()
        {
            dataset = dal_user.GetDataSet("select * from Users_Auth");
            var dt = dataset.Tables[0];
            var listUsers = new List<Models.DataModel.ModelUsersAuth>();
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                var userManage = new Models.DataModel.ModelUsersAuth()
                {
                    
                    EMPLOYEE_ID = dr["EMPLOYEE_ID"].ToString(),
                    FIRSTNAME = dr["FIRSTNAME"].ToString(),
                    LASTNAME = dr["LASTNAME"].ToString(),
                    EMAIL = dr["EMAIL"].ToString(),
                    ROLE_ID = Convert.ToInt32(dr["ROLE_ID"]),
                    CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"]),
                    CREATE_BY = dr["CREATE_BY"].ToString(),
                    IS_AD = Convert.ToBoolean(dr["IS_AD"])


                };

                listUsers.Add(userManage);
            }
        }//user
        

      
    }
}