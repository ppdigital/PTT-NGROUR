using PTT_NGROUR.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
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
            var UsersLog = new List<Models.DataModel.ModelUsersLog>();
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
                    string browser = Request.Browser.Id;
                    #region Insert Log
                    string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                    if (string.IsNullOrEmpty(ip))
                    {
                        ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                    }
                    //add log to USERS_AUTH_LOG
                    var dal = new DAL.DAL();
                    string empty = "''";
                    string insertLog = "'" + user.Username + "',Sysdate,'" + this.Session.SessionID + "','" + ip + "','" + Request.Browser.Id + "'," + empty;
                    string strCommand = "INSERT into USERS_AUTH_LOG (EMPLOYEE_ID,DATE_LOGIN,SESSION_ID,IPADDRESS,BROWSER,LOG_STATUS) VALUES ("+insertLog+")";
                    var con = dal.GetConnection();
                    con.Open();
                    dal.GetCommand(strCommand, con).ExecuteNonQuery();
                    con.Close();
                    con.Dispose();

                    
                    #endregion
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
