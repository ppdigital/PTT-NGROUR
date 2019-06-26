using PTT_NGROUR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PTT_NGROUR.Controllers
{
    public class AuthorizeController : Controller
    {
        //
        // GET: /Authorize/
        private User UserA = new User();

        [PTT_NGROUR.Controllers.AuthorizeController.CustomAuthorize]
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MenuBar()
        {
            if (UserA.Roleid == 2) {
                ViewData["UImport_page"] = "none";
                ViewData["Threshold_page"] = "none";
                ViewData["Admin_page"] = "none";
                ViewData["RiskFile_page"] = "none";
 
            }
            return PartialView("MenuBar");
        }//end MenuBar

        public class CustomAuthorize : AuthorizeAttribute
        {
            private User userA = new User();
            public override void OnAuthorization(AuthorizationContext filterContext)
            {

                string controlName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName;
                string ActName = filterContext.ActionDescriptor.ActionName;
                bool cnN = false;
                var url = filterContext.HttpContext.Request.UrlReferrer == null ? "" : filterContext.HttpContext.Request.UrlReferrer.AbsolutePath;

                List<string> user1 = new List<string>() {
                    "Home/Index",
                    "User/ResetPassword",
                    "Dashboard/Index",
                    "Utilization/Index", "Utilization/Customer", "Utilization/Report",
                    "OM/Index",
                    "Risk/Index", "Risk/Report", "Risk/RiskManagementGraph",
                };


                if (userA.Roleid == 1)
                {
                    cnN = true;
                }
                else if (userA.Roleid == 2)
                {
                    cnN = user1.Contains(controlName + "/" + ActName);
                }
                if (filterContext.HttpContext.User.Identity.IsAuthenticated && cnN)
                {
                    base.OnAuthorization(filterContext);
                }
                else
                {
                    if (url == "" || userA.Roleid == 0)
                    {
                        filterContext.Result = new RedirectToRouteResult(new
                        RouteValueDictionary(new { controller = "User", action = "Login" }));
                    }
                    else
                    {
                        filterContext.Result = new RedirectResult("~" + url);
                    }
                }

            }
        }//end CustomAuthorzie

    }
}
