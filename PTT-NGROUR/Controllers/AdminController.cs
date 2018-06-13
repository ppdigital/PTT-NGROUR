using PTT_NGROUR.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PTT_NGROUR.ExtentionAndLib;
using System.Text;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.DirectoryServices;
using PTT_NGROUR.Controllers;
using System.Data;

namespace PTT_NGROUR.Controllers
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult UserManagement()
        {
            var dal = new DAL.DAL();

            var ds = dal.GetDataSet("SELECT A.EMPLOYEE_ID, A.FIRSTNAME, A.LASTNAME, A.EMAIL, R.ROLE_NAME, G.GROUP_NAME, A.CREATE_DATE, A.CREATE_BY, A.ROLE_ID FROM USERS_AUTH A LEFT JOIN USERS_GROUP G ON A.GROUP_ID = G.GROUP_ID LEFT JOIN USERS_ROLE R ON A.ROLE_ID = R.ROLE_ID ORDER BY CREATE_DATE DESC");
            var dt = ds.Tables[0];
            var listUsers = new List<Models.DataModel.ModelUsersAuth>();
            
            foreach (System.Data.DataRow dr in dt.Rows)
            {
                var user = new Models.DataModel.ModelUsersAuth()
                {
                    EMPLOYEE_ID = dr["EMPLOYEE_ID"].ToString(),
                    FIRSTNAME = dr["FIRSTNAME"].ToString(),
                    LASTNAME = dr["LASTNAME"].ToString(),
                    EMAIL = dr["EMAIL"].ToString(),
                    ROLE_ID = Convert.ToInt32(dr["ROLE_ID"].ToString()),
                    ROLE_NAME = dr["ROLE_NAME"].ToString(),
                    CREATE_DATE = dr["CREATE_DATE"].GetDate(),// Convert.ToDateTime(dr["CREATE_DATE"].ToString()),
                    CREATE_BY = dr["CREATE_BY"].ToString(),
                    GROUP_NAME = dr["GROUP_NAME"].ToString(),

                    //GROUP_ID = Convert.ToInt32(dr["GROUP_ID"].ToString())

                };
                listUsers.Add(user);
            }

            var d_role = dal.GetDataSet("select ROLE_ID ,ROLE_NAME from USERS_ROLE");
            var dt_role = d_role.Tables[0];
            var listRole = new List<Models.DataModel.ModelUsersRole>();
            foreach (System.Data.DataRow drole in dt_role.Rows)
            {
                var role = new Models.DataModel.ModelUsersRole()
                {
                    ROLE_ID = Convert.ToInt32(drole["ROLE_ID"].ToString()),
                    ROLE_NAME = drole["ROLE_NAME"].ToString()
                };

                listRole.Add(new Models.DataModel.ModelUsersRole { ROLE_NAME = role.ROLE_NAME, ROLE_ID = role.ROLE_ID });    
            }
            ViewBag.seRoleEdit = listRole;
            ViewBag.seCreateRole = listRole;
            var model = new Models.UserManagement()
            {
                ModelUsersAuth = listUsers,
                ModelUsersRole = listRole
            };

            return View(model);

        }

        public ActionResult SearchbyAD(string txtSeacrh)
        {
            List<userManagement> bufferUser = new List<userManagement>();
            userManagement userSearch = new userManagement();

            if (txtSeacrh != null)
            {

                try
                {
                    string cn = txtSeacrh;
                    //string login_name = "520154";
                    //string pass = "100Piper$2";
                    string login_name = "ictsupport";
                    string pass = "1234";
                    string[] txtTemp = Regex.Split(cn, " ");
                    //cn = "";
                    SQLServerQuery ii = new SQLServerQuery();
                    DataTable Final_result = new DataTable();
                    if (txtTemp.Length >= 2 && txtTemp[1] != null)
                    {
                        Final_result = ii.SearchInPIS(txtTemp[0], txtTemp[1]);

                        if (Final_result.Rows.Count > 0)
                        {

                        }
                        else
                        {
                            LDAPSearch searchInLdap = new LDAPSearch();
                            DataTable dtTwo = searchInLdap.LDAPLoadAllUser("PTTICT", login_name, pass, @"LDAP://PTTICT.CORP", cn);
                            if (dtTwo.Rows.Count > 0)
                            {

                                for (int i = 0; i < dtTwo.Rows.Count; i++)
                                {
                                    userSearch.EMPLOYEE_ID = dtTwo.Rows[i][0].ToString();
                                    userSearch.FIRSTNAME = dtTwo.Rows[i][1].ToString();
                                    userSearch.LASTNAME = dtTwo.Rows[i][2].ToString();
                                    userSearch.EMAIL = Final_result.Rows[i][3].ToString();
                                    bufferUser.Add(userSearch);

                                }
                            }
                            else
                            {

                            }
                        }
                    }
                    else
                    {
                        Final_result = ii.SearchInPIS(txtTemp[0], null);
                        if (Final_result.Rows.Count > 0)
                        {
                            for (int i = 0; i < Final_result.Rows.Count; i++)
                            {
                                userSearch = new userManagement();
                                userSearch.EMPLOYEE_ID = Final_result.Rows[i][0].ToString();
                                userSearch.FIRSTNAME = Final_result.Rows[i][1].ToString();
                                userSearch.LASTNAME = Final_result.Rows[i][2].ToString();
                                userSearch.EMAIL = Final_result.Rows[i][3].ToString();
                                bufferUser.Add(userSearch);
                            }
                        }
                        else
                        {
                            LDAPSearch searchInLdap = new LDAPSearch();
                            DataTable dtTwo = searchInLdap.LDAPLoadAllUser("PTTICT", login_name, pass, @"LDAP://PTTICT.CORP", cn);
                            if (dtTwo.Rows.Count > 0)
                            {

                                for (int i = 0; i < dtTwo.Rows.Count; i++)
                                {
                                    userSearch.EMPLOYEE_ID = dtTwo.Rows[i][0].ToString();
                                    userSearch.FIRSTNAME = dtTwo.Rows[i][1].ToString();
                                    userSearch.LASTNAME = dtTwo.Rows[i][2].ToString();
                                    userSearch.EMAIL = Final_result.Rows[i][3].ToString();
                                    bufferUser.Add(userSearch);

                                }
                            }
                            else
                            {

                            }
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return PartialView("SearchbyAD", bufferUser);

        }

        public ActionResult CreateUser(string txtEMPID, string txtFNC, string txtLNC, string PWC, string IsADCreate, string txtMailCreate, int selectRoleCreate)
        {var dal = new DAL.DAL();
                string LabelText = "เพิ่มข้อมูลเรียบร้อย";
              string txtIsADCreate = IsADCreate;
              int group = 0;
              string password = "";
              string username = User.Identity.Name;
            if (txtEMPID != "" && txtFNC != "" && txtLNC != "")
            {
                
                
                var expire = @"select * from VIEW_AUTH_STATUS
                          where EMPLOYEE_ID ='" + txtEMPID + "'";
                var ds = dal.GetDataSet(expire);

                if (ds.Tables[0].Rows.Count > 0)
                { LabelText = "มีชื่อผู้ใช้ในระบบแล้ว"; }

                else
                {
                    if (txtIsADCreate == "0")
                    {
                        

                        if (PWC != "")
                        {
                            group = 2;
                            byte[] results;
                            string encryptedPassword;
                            // string decryptedPassword;
                            UTF8Encoding utf8 = new UTF8Encoding();
                            //to create the object for UTF8Encoding  class
                            //TO create the object for MD5CryptoServiceProvider 
                            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
                            byte[] deskey = md5.ComputeHash(utf8.GetBytes(PWC));
                            //to convert to binary passkey
                            //TO create the object for  TripleDESCryptoServiceProvider 
                            TripleDESCryptoServiceProvider desalg = new TripleDESCryptoServiceProvider();
                            desalg.Key = deskey;//to  pass encode key
                            desalg.Mode = CipherMode.ECB;
                            desalg.Padding = PaddingMode.PKCS7;
                            byte[] encrypt_data = utf8.GetBytes(PWC);
                            //to convert the string to utf encoding binary 

                            try
                            {
                                //To transform the utf binary code to md5 encrypt    
                                ICryptoTransform encryptor = desalg.CreateEncryptor();
                                results = encryptor.TransformFinalBlock(encrypt_data, 0, encrypt_data.Length);
                                encryptedPassword = Convert.ToBase64String(results);
                                password = encryptedPassword;
                            }
                            finally
                            {
                                //to clear the allocated memory
                                desalg.Clear();
                                md5.Clear();
                            }

                        }
                        else
                        {
                            LabelText = "กรุณากรอก Password";
                        }

                    }
                    else { password = "";
                    group = 1;
                    }
                    string insertLog = "'" + txtEMPID.Trim() + "','" + txtFNC + "','" + txtLNC + "','" + IsADCreate + "','" + txtMailCreate + "','" + password + "','" + selectRoleCreate + "',Sysdate,'" + group + "','" + username + "'";
                    string strCommand = "INSERT into USERS_AUTH (EMPLOYEE_ID,FIRSTNAME,LASTNAME,IS_AD,EMAIL,PASSWORD,ROLE_ID,CREATE_DATE,GROUP_ID,CREATE_BY) VALUES (" + insertLog + ")";
                    var con = dal.GetConnection();
                    con.Open();
                    dal.GetCommand(strCommand, con).ExecuteNonQuery();
                    con.Close();
                    con.Dispose();
                    //  return Content(LabelText, Redirect("UserManagement"));

                    // return RedirectToAction("UserManagement");
                }
            }
            else
            {
                LabelText = "กรุณากรอกข้อมูลให้ครบถ้วน";
            }
             return Content(LabelText);
    }
        [HttpPost]
        public ActionResult EditUser(string txtEmployeeIDEdit, string seRoleEdit, string txtMailEdit)
        {   //string textEdit = "แก้ไข้อมูลเรียบร้อย";
        int roleInt = Convert.ToInt32(seRoleEdit);
            if (txtMailEdit != "")
            {
            var dal = new DAL.DAL();
            string username = User.Identity.Name;
            string strCommand = "UPDATE USERS_AUTH SET ROLE_ID ='" + roleInt + "',EMAIL='" + txtMailEdit + "',UPDATE_DATE=Sysdate,UPDATE_BY='Por' WHERE EMPLOYEE_ID='" + txtEmployeeIDEdit + "'";
            var con = dal.GetConnection();
            con.Open();
            dal.GetCommand(strCommand, con).ExecuteNonQuery();
            con.Close();
            con.Dispose();
          }
            //else { textEdit = "โปรดกรอกข้อมูลให้ครบถ้วน"; }
            //ViewBag.textAlert = textEdit;
            //TempData["message"] = textEdit;
            return Redirect("UserManagement"); 
        }
        public ActionResult DeleteUser(string Id)
        {
            if (Id != "")
            {
                var dal = new DAL.DAL();
                string username = User.Identity.Name;
                string strCommand = "DELETE FROM USERS_AUTH WHERE EMPLOYEE_ID ='" + Id.Trim() + "'";
                var con = dal.GetConnection();
                con.Open();
                dal.GetCommand(strCommand, con).ExecuteNonQuery();
                con.Close();
                con.Dispose();
            }
            return RedirectToAction("UserManagement");
        }    

}
    public class userManagement
    {
        public string EMPLOYEE_ID { get; set; }
        public string FIRSTNAME { get; set; }
        public string LASTNAME { get; set; }
        //public string USERNAME { get; set; }
        public string PASSWORD { get; set; }
        public string IS_AD { get; set; }
        public Nullable<DateTime> CREATE_DATE { get; set; }
        public string CREATE_BY { get; set; }
        public Nullable<DateTime> UPDATE_DATE { get; set; }
        public string UPDATE_BY { get; set; }
        public decimal GROUP_ID { get; set; }
        public string GROUP_NAME { get; set; }
        public decimal FLEET_ID { get; set; }
        public string FLEET_NAME { get; set; }
        public string CARRIER_CODE { get; set; }
        public string CARRIER_NAME { get; set; }
        public string CARRIER_SHORTNAME { get; set; }
        public int ROLE_ID { get; set; }
        public string ROLE_NAME { get; set; }
        public string ROLE_ALARM { get; set; }
        public string EMAIL { get; set; }
        public string STATUS { get; set; }
    }
}
