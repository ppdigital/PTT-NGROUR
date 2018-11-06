using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.DirectoryServices;
using System.Data;
using Oracle.ManagedDataAccess.Client;

namespace PTT_NGROUR.Models
{
    public class User
    {
        DAL.DAL dal = new DAL.DAL();
        System.Data.DataSet ds; 

        public User()
        {
            //ds = dal.GetDataSet("select * from Users_Auth");
            //DataTable dt = ds.Tables[0];
            //if (dt != null)
            //{
            //    var listUsers = new List<Models.DataModel.ModelUsersAuth>();
            //    foreach (System.Data.DataRow dr in dt.Rows)
            //    {
            //        var user = new Models.DataModel.ModelUsersAuth()
            //        {
            //            //CREATE_BY = dr["CREATE_BY"].ToString(),
            //            //CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"]),
            //            FIRSTNAME = dr["FIRSTNAME"].ToString()
            //        };
            //        listUsers.Add(user);
            //    }
            //}

            try
            {
                using (var conn = new OracleConnection(dal.ConnectionString))
                {
                    conn.Open();
                    using (OracleCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "select * from Users_Auth";

                        using (OracleDataReader reader = cmd.ExecuteReader())
                        {
                            var dataTable = new DataTable();
                            dataTable.Load(reader);
                            if (dataTable != null)
                            {
                                var listUsers = new List<Models.DataModel.ModelUsersAuth>();
                                foreach (System.Data.DataRow dr in dataTable.Rows)
                                {
                                    var user = new Models.DataModel.ModelUsersAuth()
                                    {
                                        //CREATE_BY = dr["CREATE_BY"].ToString(),
                                        //CREATE_DATE = Convert.ToDateTime(dr["CREATE_DATE"]),
                                        FIRSTNAME = dr["FIRSTNAME"].ToString()
                                    };
                                    listUsers.Add(user);
                                }
                            }
                            Console.WriteLine("VisibleFieldCount: {0}", reader.VisibleFieldCount);
                            Console.WriteLine("HiddenFieldCount: {0}", reader.HiddenFieldCount);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error:{0}", ex.Message);
            }
        }//user


        [Required]
        [Display(Name = "Username")]
        public string Username
        {
            get
            {
                if (HttpContext.Current.Session["Username"] != null)
                    return (string)HttpContext.Current.Session["Username"];
                else
                    return "";
            }
            set { HttpContext.Current.Session["Username"] = value; }
        }//username
      
     
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password
        {
            get
            {
                if (HttpContext.Current.Session["Password"] != null)
                    return (string)HttpContext.Current.Session["Password"];
                else
                    return "";
            }
            set { HttpContext.Current.Session["Password"] = value; }
        }//password

        [Display(Name = "Remember on this computer")]
        public bool RememberMe { get; set; }

        public int Roleid
        {
            get
            {
                if (HttpContext.Current.Session["Roleid"] != null)
                    return (int)HttpContext.Current.Session["Roleid"];
                else
                    return 0;
            }
            set { HttpContext.Current.Session["Roleid"] = value; }
        }//role

        public string IS_AD
        {
            get
            {
                if (HttpContext.Current.Session["IS_AD"] != null)
                    return (string)HttpContext.Current.Session["IS_AD"];
                else
                    return "";
            }
            set { HttpContext.Current.Session["IS_AD"] = value; }
        }//IsAD
         public string FirstName{
           get
           {
               if (HttpContext.Current.Session["FirstName"] != null)
                   return (string)HttpContext.Current.Session["FirstName"];
               else
                   return "";
           }
           set { HttpContext.Current.Session["FirstName"] = value; }
       }
         public string LastName
         {
             get
             {
                 if (HttpContext.Current.Session["LastName"] != null)
                     return (string)HttpContext.Current.Session["LastName"];
                 else
                     return "";
             }
             set { HttpContext.Current.Session["LastName"] = value; }
         }
        public bool IsExpired(string _employeeID)
        {
            var expire = @"select 1 from VIEW_AUTH_STATUS
                          where EMPLOYEE_ID ='"+ _employeeID +"' and COUNT_PASS = 'F'";
            var result = dal.ReadData(expire, (red) => {
                return 1;
            }).Any();
            return result;
            //var ds = dal.GetDataSet(expire);
            //return ds.Tables[0].Rows.Count > 0;

           // if (expire.Count() > 0)
           // {
           //     return true;
           // }
           //else
           // {
           //     return false;
           // }
        }

        public bool Isvalid(string _username, string _password, string domain, string LdapPath)
        {
            byte[] results;
            string encryptedPassword;

            UTF8Encoding utf8 = new UTF8Encoding();
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] deskey = md5.ComputeHash(utf8.GetBytes(_password));
            TripleDESCryptoServiceProvider desalg = new TripleDESCryptoServiceProvider();
            desalg.Key = deskey;//to  pass encode key
            desalg.Mode = CipherMode.ECB;
            desalg.Padding = PaddingMode.PKCS7;
            byte[] encrypt_data = utf8.GetBytes(_password);

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

            string domainAndUsername = domain + @"\" + _username;
            DirectoryEntry entry = new DirectoryEntry(LdapPath, domainAndUsername, _password);
            try
            {
                var a = @"select * from USERS_AUTH
                        where EMPLOYEE_ID ='" + _username + "' and PASSWORD = '"+encryptedPassword+"'";
                var ds = dal.GetDataSet(a);
                if (ds.Tables[0].Rows.Count > 0)
                {
                        var c = @"select * from USERS_AUTH
                                where EMPLOYEE_ID ='" + _username + "' and PASSWORD = '"+encryptedPassword+"'";
                        var ds1 = dal.GetDataSet(c);
                        var dt = ds1.Tables[0];
                        foreach (System.Data.DataRow dr in dt.Rows) {
                            Username = dr["EMPLOYEE_ID"].ToString();
                            Password = dr["PASSWORD"].ToString();
                            FirstName = dr["FIRSTNAME"].ToString();
                            LastName = dr["LASTNAME"].ToString();
                            Roleid = Convert.ToInt32(dr["ROLE_ID"]);
                            IS_AD = dr["IS_AD"].ToString();
                        }
                    return true;
                }//end if count
                else
                {
                    Object obj = entry.NativeObject;
                    DirectorySearcher search = new DirectorySearcher(entry);
                    search.Filter = "(SAMAccountName=" + _username + ")";
                    search.PropertiesToLoad.Add("cn");
                    search.PropertiesToLoad.Add("SAMAccountName");
                    search.PropertiesToLoad.Add("displayName");
                    SearchResult result = search.FindOne();

                    LdapPath = result.Path;

                    string _filterAttribute = (String)result.Properties["cn"][0];
                    string _filterNameAttribute = (String)result.Properties["SAMAccountName"][0];
                    string _filterFullnameAttribute = (String)result.Properties["displayName"][0];

                    var u = @"select * from USERS_AUTH where EMPLOYEE_ID ='" + _filterAttribute + "'and IS_AD = '1'";
                    var ds2 = dal.GetDataSet(u);
                    if ((result != null && ds2.Tables[0].Rows.Count > 0) || (result == null && ds2.Tables[0].Rows.Count > 0))
                    {
                        var e = @"select * from USERS_AUTH where EMPLOYEE_ID ='" + _filterAttribute + "'and IS_AD = '1'";
                        var ds3 = dal.GetDataSet(e);
                        var dt = ds3.Tables[0];
                        foreach (System.Data.DataRow dr in dt.Rows)
                        {
                            Username = dr["EMPLOYEE_ID"].ToString();
                            Password = dr["PASSWORD"].ToString();
                            Roleid = Convert.ToInt32(dr["ROLE_ID"]);
                            IS_AD = dr["IS_AD"].ToString();
                        }
                        return true;
                    }
                    else
                    {
                        return false;
                    }

                }

            }//try
            catch (Exception ex)
            {
                // throw an error
            }
            return false;
        }
    }
}