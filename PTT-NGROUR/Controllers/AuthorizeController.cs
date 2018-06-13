using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTT_NGROUR.Controllers
{
    public class AuthorizeController : Controller
    {
        //
        // GET: /Authorize/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuBar()
        {
            return PartialView("MenuBar");
        }

    }
}
