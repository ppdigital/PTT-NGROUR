using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTT_NGROUR.App_Start
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult UserManagement()
        {  var dal = new DAL.DAL();

            var ds = dal.GetDataSet("select * from Users_Auth");

           var dt = ds.Tables[0];

            var listUsers = new List<Models.DataModel.ModelUsersAuth>();

           foreach (System.Data.DataRow dr in dt.Rows)
           {
               var user = new Models.DataModel.ModelUsersAuth() {
                   EMPLOYEE_ID = dr["EMPLOYEE_ID"].ToString(),
                   FIRSTNAME = dr["FIRSTNAME"].ToString(),
                   LASTNAME = dr["LASTNAME"].ToString(),
                   EMAIL = dr["EMAIL"].ToString(),
                   ROLE_ID = Convert.ToInt32(dr["ROLE_ID"]),
                   CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"]),
                   CREATE_BY = dr["CREATE_BY"].ToString(),
                   IS_AD = Convert.ToBoolean(dr["IS_AD"])

               };
               listUsers.Add(user);
           }

           var model = new Models.ViewModel.ModelHome()
            {
             ListUsersAuth = listUsers
           };

            return View(model);
         
        }

    }
}
