using PTT_NGROUR.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PTT_NGROUR.Controllers
{
    public class UserController : Controller
    {
        private User userConTR = new User();
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
                    return RedirectToAction("MenuWeb", "Home");
                    //return RedirectToAction("ResetPassword", "User");
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
        public ActionResult ResetPassword(string id, bool isExpired)
        {
            if (id == null)
            {
                return RedirectToAction("Login", "User");
            }
            else
            {
                ViewBag.isExpired = isExpired;
                return View();
            }
        }//ResetPassword
        public ActionResult ChangePassword(string NewPassword, string OldPassword, string ConfirmNewPassword, Models.User user)
        {
            string ChangePassText = "Password changed successfully";
            byte[] results;
            if (NewPassword == "" || OldPassword == "" || ConfirmNewPassword == "")
            {
                ChangePassText = "Please complete the data";
            }else{
                string encryptedPassword;

                UTF8Encoding utf8 = new UTF8Encoding();
                MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                byte[] deskey = md5.ComputeHash(utf8.GetBytes(OldPassword));
                TripleDESCryptoServiceProvider desalg = new TripleDESCryptoServiceProvider();
                desalg.Key = deskey;//to  pass encode key
                desalg.Mode = CipherMode.ECB;
                desalg.Padding = PaddingMode.PKCS7;
                byte[] encrypt_data = utf8.GetBytes(OldPassword);         
                try
                {
                    ICryptoTransform encryptor = desalg.CreateEncryptor();
                    results = encryptor.TransformFinalBlock(encrypt_data, 0, encrypt_data.Length);
                    encryptedPassword = Convert.ToBase64String(results);
                }
                finally
                {
                    desalg.Clear();
                    md5.Clear();
                }
                if (encryptedPassword == userConTR.Password)
                {
                    if (NewPassword == ConfirmNewPassword)
                    {
                        string username = User.Identity.Name;
                        byte[] resultsChange;
                        string encryptedChangePassword;
                        UTF8Encoding utf8_ = new UTF8Encoding();
                        MD5CryptoServiceProvider md5_ = new MD5CryptoServiceProvider();
                        byte[] deskey_ = md5_.ComputeHash(utf8_.GetBytes(ConfirmNewPassword));

                        TripleDESCryptoServiceProvider desalg_ = new TripleDESCryptoServiceProvider();
                        desalg_.Key = deskey_;//to  pass encode key
                        desalg_.Mode = CipherMode.ECB;
                        desalg_.Padding = PaddingMode.PKCS7;
                        byte[] encrypt_data_ = utf8_.GetBytes(ConfirmNewPassword);

                        try
                        {
                            ICryptoTransform encryptor_ = desalg_.CreateEncryptor();
                            resultsChange = encryptor_.TransformFinalBlock(encrypt_data_, 0, encrypt_data_.Length);
                            encryptedChangePassword = Convert.ToBase64String(resultsChange);
                        }
                        finally
                        {
                            desalg_.Clear();
                            md5_.Clear();
                        }
                        var dal = new DAL.DAL();
                        var dal2 = new DAL.DAL();
                        string CommandAuth = "UPDATE USERS_AUTH SET PASSWORD = '"+encryptedChangePassword+"', UPDATE_DATE = Sysdate, UPDATE_BY = '"+username+"' WHERE EMPLOYEE_ID ='"+userConTR.Username+"'";
                        string CommandStatus = "UPDATE USERS_AUTH_STATUS SET PASSDATE = Sysdate WHERE EMPLOYEE_ID ='" + userConTR.Username + "'";
                        var con = dal.GetConnection();
                        var con2 = dal2.GetConnection();
                        con.Open();
                        con2.Open();
                        dal.GetCommand(CommandAuth, con).ExecuteNonQuery();
                        dal.GetCommand(CommandStatus, con2).ExecuteNonQuery();
                        con.Close();
                        con2.Close();
                        con.Dispose();
                        con2.Dispose();
                    }
                    else
                    {
                        ChangePassText = "New Password dose not match Confirm Password";
                    }
                }
                else {
                    ChangePassText = "Old Password is not valid";
                }
            }//end check empty

            return Content(ChangePassText);
        }//ChangePassword
    }
}
