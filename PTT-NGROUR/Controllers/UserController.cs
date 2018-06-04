using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PTT_NGROUR.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(Models.User user)
        {
            string domainName = string.Empty;
            string adPath = string.Empty;
            string strError = string.Empty;

            domainName = ConfigurationManager.AppSettings["DirectoryDomain"];
            adPath = ConfigurationManager.AppSettings["DirectoryPath"];

            if (!String.IsNullOrEmpty(domainName) && !String.IsNullOrEmpty(adPath))
            {
                if (user.Isvalid(user.Username, user.Password, domainName, adPath))
                {
                    if (user.IsExpired(user.Username))
                    {
                        return RedirectToAction("ResetPassword", new { id = user.Username, isExpired = true });
                    }
                    FormsAuthentication.SetAuthCookie(user.Username, user.RememberMe);
                    //return RedirectToAction("Index", "Dashboard");
                    return RedirectToAction("UserManagement", "Admin");
                }
                else
                {
                    ModelState.AddModelError("", "Login data is incorrect!");
                }
                return View(user);

            }

            return RedirectToAction("Login", "User");
        }//Login
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }//Logout
    }
}
