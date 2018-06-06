using PTT_NGROUR.Models.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PTT_NGROUR.ExtentionAndLib;
using System.Text;
using System.Security.Cryptography;

namespace PTT_NGROUR.App_Start
{
    public class AdminController : Controller
    {
        //
        // GET: /Admin/

        public ActionResult UserManagement()
        {
            var dal = new DAL.DAL();

            var ds = dal.GetDataSet("select EMPLOYEE_ID ,FIRSTNAME ,LASTNAME ,EMAIL ,ROLE_ID ,GROUP_ID ,CREATE_DATE ,CREATE_BY from Users_Auth");

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
                    CREATE_DATE = dr["CREATE_DATE"].GetDate(),// Convert.ToDateTime(dr["CREATE_DATE"].ToString()),
                    CREATE_BY = dr["CREATE_BY"].ToString(),
                    GROUP_ID = Convert.ToInt32(dr["GROUP_ID"].ToString())

                };
                listUsers.Add(user);
            }

            var model = new Models.UserManagement()
            {
                ModelUsersAuth = listUsers
            };

            return View(model);

        }

        public ActionResult CreateUser(string txtEMPID, string txtFNC, string txtLNC, string PWC, string IsADCreate, string txtMailCreate, int selectRoleCreate)
        {var dal = new DAL.DAL();
                string LabelText = "เพิ่มข้อมูลเรียบร้อย";
              string txtIsADCreate = IsADCreate;
              int group = 0;
              string password = "";

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
                    else { password = "''";
                    group = 1;
                    }
                    string insertLog = "'" + txtEMPID.Trim() + "','" + txtFNC + "','" + txtLNC + "','" + IsADCreate + "','" + txtMailCreate + "','" + password + "','" + selectRoleCreate + "',Sysdate,'" + group + "','Por'";
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
