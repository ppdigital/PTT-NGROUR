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
        {
            return View();
        }

    }
}
