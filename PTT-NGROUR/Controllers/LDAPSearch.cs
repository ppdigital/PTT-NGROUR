using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PTT_NGROUR.Controllers
{
    public class LDAPSearch
    {
        public DataTable LDAPLoadAllUser(string domain, string username, string password, string LdapPath, string cn)
        {
            //Errmsg = "";
            DataTable result_scr = new DataTable();
            string domainAndUsername = domain + @"\" + username;
            DirectoryEntry entry = new DirectoryEntry(LdapPath, domainAndUsername, password);
            try
            {
                // Bind to the native AdsObject to force authentication.
                Object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(SAMAccountName=" + cn + ")";
                search.PropertiesToLoad.Add("cn");
                search.PropertiesToLoad.Add("SAMAccountName");
                search.PropertiesToLoad.Add("displayName");

                SearchResultCollection allUsers = search.FindAll();

                result_scr.Columns.Add("รหัสพนักงาน");
                result_scr.Columns.Add("ชื่อ");
                result_scr.Columns.Add("นามสกุล");

                foreach (SearchResult result in allUsers)
                {
                    if (result.Properties["cn"].Count > 0)
                    {
                        //string searchID = (String)result.Properties["cn"][0];
                        string ID = (String)result.Properties["SAMAccountName"][0];
                        string NAME = (String)result.Properties["displayName"][0];
                        string[] splitName = Regex.Split(NAME, " ");

                        result_scr.Rows.Add(ID, splitName[0], splitName[1]);
                    }
                }
            }
            catch (Exception ex)
            {
                //Errmsg = ex.Message;
                return result_scr = null;
                throw new Exception("Error authenticating user." + ex.Message);
            }
            return result_scr;
        }      
    }
}