using PTT_NGROUR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTT_NGROUR.Controllers
{  
    public class HomeController : Controller
    {
        private User UserH = new User();

        //
        // GET: /Home/
        [PTT_NGROUR.Controllers.AuthorizeController.CustomAuthorize]
        public ActionResult Index()
        {
            //var dal = new DAL.DAL();

            //var ds = dal.GetDataSet("select * from Users_Auth");

            //var dt = ds.Tables[0];

            //var listUsers = new List<Models.DataModel.ModelUsersAuth>();

            //foreach (System.Data.DataRow dr in dt.Rows)
            //{
            //    var user = new Models.DataModel.ModelUsersAuth() {
            //        //CREATE_BY = dr["CREATE_BY"].ToString(),
            //        //CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"]),
            //        FIRSTNAME = dr["FIRSTNAME"].ToString()
            //    };
            //    listUsers.Add(user);
            //}

            //var model = new Models.ViewModel.ModelHome()
            //{
            //    ListUsersAuth = listUsers
            //};

            //return View(model);
            return RedirectToAction("Login", "User");
        }
        public ActionResult MenuMobile()
        { 
            if (UserH.Roleid == 2)
            {
                ViewData["UImport_page"] = "none";
                ViewData["Threshold_page"] = "none";
                ViewData["Admin_page"] = "none";

            }
            return PartialView("MenuMobile");
        }

        public ActionResult MenuWeb()
        {
            if (UserH.Roleid == 2)
            {
                ViewData["UserManage_page"] = "none";
                

            }
            return PartialView("MenuWeb");
        }

        public ActionResult UserProfile(string UserNo)
        {
            var dal = new DAL.DAL();
            var user = @"select * from Users_Auth where EMPLOYEE_ID ='" + UserNo + "'";
            var ds_user = dal.GetDataSet(user);
            var listUser = new List<Models.DataModel.ModelUsersAuth>();
            if (ds_user.Tables[0].Rows.Count > 0)
            {
                foreach (System.Data.DataRow druser in ds_user.Tables[0].Rows)
                {
                    var status_Adduser = new Models.DataModel.ModelUsersAuth()
                    {
                        FIRSTNAME = druser["FIRSTNAME"].ToString(),
                        LASTNAME = druser["LASTNAME"].ToString()
                    };
                    listUser.Add(status_Adduser);
                }
            }

            var model = new Models.UserManagement()
            {
                ListUser = listUser

            };

            return View(model);
        }
    }
}
