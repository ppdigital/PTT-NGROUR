using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTT_NGROUR.Controllers
{
    public class HomeController : Controller
    {

        //
        // GET: /Home/

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
        { return View(); }

    }
}
