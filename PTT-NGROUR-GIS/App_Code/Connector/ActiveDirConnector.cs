using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.DirectoryServices;

/// <summary>
/// Summary description for ActiveDirConnector
/// </summary>
namespace Connector
{
    public class ActiveDirConnector
    {
        private DirectoryEntry entry;
        public string Path = string.Empty;
        public string Domain = string.Empty;
        public string DefaultUserName = string.Empty;
        public string DefaultPassword = string.Empty;

        public ActiveDirConnector()
        {
            //set Path and Domain parameter from the web config
            Path = AMSCore.WebConfigReadKey("AD_PATH");
            Domain = AMSCore.WebConfigReadKey("AD_DOMAIN");
            DefaultUserName = AMSCore.WebConfigReadKey("AD_USERNAME");
            DefaultPassword = AMSCore.WebConfigReadKey("AD_PASSWORD");
        }

        public Dictionary<string, object> Authen(string username, string password)
        {
            Dictionary<string, object> userDetail = new Dictionary<string, object>();
            entry = new DirectoryEntry(Path, username, password);
            try
            {
                Object obj = entry.NativeObject;

                DirectorySearcher search = new DirectorySearcher(entry);
                string usernameField = !string.IsNullOrEmpty(AMSCore.WebConfigReadKey("AD_USERNAME_FIELD")) ? AMSCore.WebConfigReadKey("AD_USERNAME_FIELD") : "SAMAccountName";
                search.Filter = "(" + usernameField + "=" + username + ")";
                string[] requiredProps = { };


                if (!string.IsNullOrEmpty(AMSCore.WebConfigReadKey("AD_DIRECTORY_PROPERTY_OUTPUT_FIELDS")))
                {
                    //The input string is in the "xxxx,xx,xxxxx" format.
                    //It has to be transformed to an array of string.

                    requiredProps = AMSCore.WebConfigReadKey("AD_DIRECTORY_PROPERTY_OUTPUT_FIELDS").Split(',');
                    search.PropertiesToLoad.AddRange(requiredProps);
                }

                SearchResult result = search.FindOne();

                if (result != null)
                {
                    if (requiredProps.Length > 0)//if set the required property
                    {
                        ResultPropertyCollection resultPropColl = result.Properties;
                        foreach (string prop in requiredProps)
                        {
                            foreach (Object memberColl in resultPropColl[prop])
                            {
                                userDetail.Add(prop, memberColl);
                            }
                        }
                    }
                    else //if does not set the required property, it will return all properties.
                    {
                        var enumEntry = result.Properties.GetEnumerator();
                        while (enumEntry.MoveNext())
                        {
                            object value = enumEntry.Value;
                            if (value is DateTime)
                            {
                                value = Util.DateTimeToString(value as DateTime?);
                            }
                            userDetail.Add(enumEntry.Key.ToString(), value);
                        }
                    }

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                entry.Close();
            }
            return userDetail;
        }
        public Dictionary<string, object> AuthenWithDomain(string username, string password)
        {
            Dictionary<string, object> userDetail = new Dictionary<string, object>();
            string domainAndUsername = null;
            domainAndUsername = string.Format(@"{0}\{1}", this.Domain, username);
            entry = new DirectoryEntry(Path, domainAndUsername, password);
            try
            {
                Object obj = entry.NativeObject;

                DirectorySearcher search = new DirectorySearcher(entry);
                string usernameField = !string.IsNullOrEmpty(AMSCore.WebConfigReadKey("AD_USERNAME_FIELD")) ? AMSCore.WebConfigReadKey("AD_USERNAME_FIELD") : "SAMAccountName";
                search.Filter = "(" + usernameField + "=" + domainAndUsername + ")";
                string[] requiredProps = { };
                if (!string.IsNullOrEmpty(AMSCore.WebConfigReadKey("AD_DIRECTORY_PROPERTY_OUTPUT_FIELDS")))
                {
                    //The input string is in the "xxxx,xx,xxxxx" format.
                    //It has to be transformed to an array of string.
                    requiredProps = AMSCore.WebConfigReadKey("AD_DIRECTORY_PROPERTY_OUTPUT_FIELDS").Split(',');
                    search.PropertiesToLoad.AddRange(requiredProps);
                }
                SearchResult result = search.FindOne();
                if (result != null)
                {
                    if (requiredProps.Length > 0)//if the required property is not null or emptry
                    {
                        ResultPropertyCollection resultPropColl = result.Properties;
                        foreach (string prop in requiredProps)
                        {
                            foreach (Object memberColl in resultPropColl[prop])
                            {
                                userDetail.Add(prop, memberColl);
                            }
                        }
                    }
                    else //if the required property is null or emptry, it will return all properties.
                    {
                        var enumEntry = result.Properties.GetEnumerator();
                        while (enumEntry.MoveNext())
                        {
                            object value = enumEntry.Value;
                            if (value is DateTime)
                            {
                                value = Util.DateTimeToString(value as DateTime?);
                            }
                            userDetail.Add(enumEntry.Key.ToString(), value);
                        }
                    }

                }

            }
            catch
            {
                return userDetail;
            }
            finally
            {
                entry.Close();
            }
            return userDetail;
        }
        public Dictionary<string, object> GetUserDetail(string username)
        {
            var outputFields = new string[] { };
            if (!string.IsNullOrEmpty(AMSCore.WebConfigReadKey("AD_DIRECTORY_PROPERTY_OUTPUT_DETAIL_FIELDS")))
            {
                //The input string is in the "xxxx,xx,xxxxx" format.
                //It has to be transformed to an array of string.
                outputFields = AMSCore.WebConfigReadKey("AD_DIRECTORY_PROPERTY_OUTPUT_DETAIL_FIELDS").Split(',');
            }
            string usernameField = !string.IsNullOrEmpty(AMSCore.WebConfigReadKey("AD_USERNAME_FIELD")) ? AMSCore.WebConfigReadKey("AD_USERNAME_FIELD") : "SAMAccountName";
            return Search(username, usernameField, outputFields).Count() > 0 ? Search(username, usernameField, outputFields)[0] : new Dictionary<string, object>();
        }
        public List<Dictionary<string, object>> GetGroupNameMemberOf()
        {
            var groupName = new List<Dictionary<string, object>>();
            string memberField = !string.IsNullOrEmpty(AMSCore.WebConfigReadKey("AD_MEMBER_FIELD")) ? AMSCore.WebConfigReadKey("AD_MEMBER_FIELD") : "memberOf";
            //var temp = Search("", "", new string[] { "adspath","name"});
            //foreach (Dictionary<string, object> name in temp)
            //{


            //        Dictionary<string, object> groupDetail = new Dictionary<string, object>();
            //        if (name.ContainsKey("adspath") && name.ContainsKey("name"))
            //        {
            //            object adspath;
            //            name.TryGetValue("adspath", out adspath);
            //            object nameG;
            //            name.TryGetValue("name", out nameG);
            //            groupDetail.Add("adspath", adspath.ToString());
            //            groupDetail.Add("name", nameG.ToString());
            //            if (!groupName.Contains(groupDetail))
            //            {

            //                groupName.Add(groupDetail);
            //            }

            //        }

            //}
            DirectoryEntry ent = new DirectoryEntry(Path);
            DirectorySearcher search = new DirectorySearcher(ent);
            string query = "(&(objectCategory=person)(objectClass=user)(memberOf=*))";
            search.Filter = query;
            //var outputFields = new string[]{ "memberOf"};
            //search.PropertiesToLoad.AddRange(outputFields);
            SearchResultCollection mySearchResultColl = search.FindAll();
            foreach (SearchResult result in mySearchResultColl)
            {

                ResultPropertyCollection resultPropColl = result.Properties;

                //foreach (string prop in outputFields)
                //{

                foreach (Object memberColl in resultPropColl["memberOf"])
                {
                    Dictionary<string, object> groupDetail = new Dictionary<string, object>();
                    groupDetail.Add("adspath", Path + "/" + memberColl);
                    if (!groupName.Contains(groupDetail))
                    {
                        groupName.Add(groupDetail);
                    }
                }
                //}

            }
            return groupName;
        }
        public List<Dictionary<string, object>> GetGroupName()
        {

            SearchResultCollection results;
            List<Dictionary<string, object>> groupDetails = new List<Dictionary<string, object>>();
            DirectoryEntry ent = new DirectoryEntry(Path);
            DirectorySearcher search = new DirectorySearcher(ent);
            search.Filter = "(objectClass=Group)";
            search.PropertiesToLoad.Add("CN");
            search.PropertiesToLoad.Add("description");
            search.PropertiesToLoad.Add("adspath");
            search.PropertiesToLoad.Add("grouptype");
            results = search.FindAll();
            foreach (SearchResult result in results)
            {
                Dictionary<string, object> groupDetail = new Dictionary<string, object>();
                ResultPropertyCollection resultPropColl = result.Properties;
                foreach (Object cn in resultPropColl["CN"])
                {
                    groupDetail.Add("CN", cn);
                }
                foreach (Object description in resultPropColl["description"])
                {
                    groupDetail.Add("description", description);
                }
                foreach (Object adspath in resultPropColl["adspath"])
                {
                    groupDetail.Add("adspath", adspath);
                }
                foreach (Object grouptype in resultPropColl["grouptype"])
                {
                    groupDetail.Add("grouptype", grouptype);
                }


                groupDetails.Add(groupDetail);

            }
            return groupDetails;
        }
        public List<Dictionary<string, object>> GetMemberofGroup(string adspath)
        {
            List<Dictionary<string, object>> groupMemebers = new List<Dictionary<string, object>>();
            try
            {

                DirectoryEntry ent = new DirectoryEntry(adspath);
                DirectorySearcher srch = new DirectorySearcher(ent);
                SearchResultCollection coll = srch.FindAll();
                string USERNAME_FIELD = !string.IsNullOrEmpty(AMSCore.WebConfigReadKey("AD_USERNAME_FIELD")) ? AMSCore.WebConfigReadKey("AD_USERNAME_FIELD") : "SAMAccountName";
                foreach (SearchResult rs in coll)
                {
                    ResultPropertyCollection resultPropColl = rs.Properties;
                    foreach (Object memberColl in resultPropColl["member"])
                    {
                        DirectoryEntry gpMemberEntry = new DirectoryEntry("LDAP://" + memberColl);
                        System.DirectoryServices.PropertyCollection subgroupProps = gpMemberEntry.Properties;
                        groupMemebers.Add(GetGroupsMember(subgroupProps, new Dictionary<string, object>()));
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
                //return groupMemebers;
            }
            return groupMemebers;
        }
        private Dictionary<string, object> GetGroupsMember(PropertyCollection subgroupProps, Dictionary<string, object> subgroup)
        {
            if (subgroupProps.Contains("member"))
            {
                var subgroupMember = new List<Dictionary<string, object>>();
                foreach (Object subgroupProp in subgroupProps["member"])
                {
                    DirectoryEntry sub_gpMemberEntry = new DirectoryEntry("LDAP://" + subgroupProp);
                    var new_subgroupProps = sub_gpMemberEntry.Properties;
                    subgroupMember.Add(GetGroupsMember(new_subgroupProps, subgroup));
                }
                foreach (Object subgroupProp in subgroupProps["cn"])
                {
                    subgroup.Add(subgroupProp.ToString(), subgroupMember);
                }

                return subgroup;
            }
            else
            {
                var userDetail = new Dictionary<string, object>();
                var userProps = subgroupProps;
                string USERNAME_FIELD = !string.IsNullOrEmpty(AMSCore.WebConfigReadKey("AD_USERNAME_FIELD")) ? AMSCore.WebConfigReadKey("AD_USERNAME_FIELD") : "SAMAccountName";
                object obVal = userProps[USERNAME_FIELD].Value;
                if (null != obVal)
                {
                    userDetail.Add(USERNAME_FIELD, obVal.ToString());
                }
                return userDetail;
            }

        }
        public List<Dictionary<string, object>> Search(string filtertText)
        {
            var filterField = new string[] { };
            if (!string.IsNullOrEmpty(AMSCore.WebConfigReadKey("AD_DIRECTORY_PROPERTY_SEARCH_FIELDS")))
            {
                //The input string is in the "xxxx,xx,xxxxx" format.
                //It has to be transformed to an array of string.
                filterField = AMSCore.WebConfigReadKey("AD_DIRECTORY_PROPERTY_SEARCH_FIELDS").Split(',');
            }
            return Search(filtertText, filterField);
        }
        public List<Dictionary<string, object>> Search(string filtertText, string filterField)
        {
            return Search(filtertText, filterField, new string[] { });
        }
        public List<Dictionary<string, object>> Search(string filtertText, string[] filterField)
        {
            return Search(filtertText, filterField, new string[] { });
        }
        public List<Dictionary<string, object>> Search(string filtertText, string filterField, string[] outputFields)
        {
            List<Dictionary<string, object>> userDetails = new List<Dictionary<string, object>>();
            try
            {
                Object obj = entry.NativeObject;
                DirectorySearcher search = new DirectorySearcher(entry);

                // set the filter 
                if (!string.IsNullOrEmpty(filterField)) search.Filter = "(" + filterField + "=*" + filtertText + "*)";


                string[] requiredProps = { };
                if (!string.IsNullOrEmpty(AMSCore.WebConfigReadKey("AD_DIRECTORY_PROPERTY_OUTPUT_FIELDS")))
                {
                    //The input string is in the "xxxx,xx,xxxxx" format.
                    //It has to be transformed to an array of string.
                    requiredProps = AMSCore.WebConfigReadKey("AD_DIRECTORY_PROPERTY_OUTPUT_FIELDS").Split(',');
                    outputFields = outputFields.Union(requiredProps).ToArray<string>();
                }
                //search.PropertiesToLoad.AddRange(outputFields);

                // loop to add all users that match the condition to userDetails 
                foreach (SearchResult result in search.FindAll())
                {
                    if (result != null)
                    {
                        Dictionary<string, object> userDetail = new Dictionary<string, object>();
                        if (outputFields.Length > 0)
                        {
                            ResultPropertyCollection resultPropColl = result.Properties;
                            foreach (string prop in outputFields)
                            {
                                foreach (Object memberColl in resultPropColl[prop])
                                {
                                    userDetail.Add(prop, memberColl);
                                }
                            }



                        }
                        else
                        {
                            var enumEntry = result.Properties.GetEnumerator();
                            while (enumEntry.MoveNext())
                            {
                                object value = enumEntry.Value;
                                if (value is DateTime)
                                {
                                    value = Util.DateTimeToString(value as DateTime?);
                                }
                                userDetail.Add(enumEntry.Key.ToString(), value);
                            }
                        }
                        userDetails.Add(userDetail);
                    }
                }

            }
            catch// (Exception ex)
            {
                return userDetails;
            }
            finally
            {
                //_DirectoryEntry.GetMethod("Close").Invoke(entry, new object[] { });
            }
            return userDetails;
        }
        public List<Dictionary<string, object>> Search(string filtertText, string[] filterFields, string[] outputFields)
        {
            List<Dictionary<string, object>> userDetails = new List<Dictionary<string, object>>();
            try
            {
                Object obj = entry.NativeObject;

                DirectorySearcher search = new DirectorySearcher(entry);

                if (filterFields.Count() > 0) search.Filter = GetFilterText(filtertText, filterFields);
                string[] requiredProps = { };
                if (!string.IsNullOrEmpty(AMSCore.WebConfigReadKey("AD_DIRECTORY_PROPERTY_OUTPUT_FIELDS")))
                {
                    //The input string is in the "xxxx,xx,xxxxx" format.
                    //It has to be transformed to an array of string.
                    requiredProps = AMSCore.WebConfigReadKey("AD_DIRECTORY_PROPERTY_OUTPUT_FIELDS").Split(',');
                    outputFields = outputFields.Union(requiredProps).ToArray<string>();
                }
                search.PropertiesToLoad.AddRange(outputFields);
                foreach (SearchResult result in search.FindAll())
                {
                    if (result != null)
                    {
                        Dictionary<string, object> userDetail = new Dictionary<string, object>();
                        if (outputFields.Length > 0)
                        {
                            ResultPropertyCollection resultPropColl = result.Properties;
                            foreach (string prop in outputFields)
                            {
                                foreach (Object memberColl in resultPropColl[prop])
                                {
                                    userDetail.Add(prop, memberColl);
                                }
                            }

                        }
                        else
                        {
                            var enumEntry = result.Properties.GetEnumerator();
                            while (enumEntry.MoveNext())
                            {
                                object value = enumEntry.Value;
                                if (value is DateTime)
                                {
                                    value = Util.DateTimeToString(value as DateTime?);
                                }
                                userDetail.Add(enumEntry.Key.ToString(), value);
                            }
                        }
                        userDetails.Add(userDetail);
                    }
                }

            }
            catch// (Exception ex)
            {
                return userDetails;
            }
            finally
            {
                //_DirectoryEntry.GetMethod("Close").Invoke(entry, new object[] { });
            }
            return userDetails;
        }

        //This getfilterText is a recursion function for constructing a filter string.
        //(<operator><filter1><filter2>)


        private string GetFilterText(string filtertText, string[] filterFields)
        {
            return GetFilterText(filtertText, filterFields, 0);
        }

        private string GetFilterText(string filtertText, string[] filterFields, int p)
        {
            if (p < filterFields.Length - 1)
            {
                return "(|(" + filterFields[p] + "=*" + filtertText + "*)" + GetFilterText(filtertText, filterFields, p + 1) + ")";
            }
            else
            {
                return "(" + filterFields[p] + "=*" + filtertText + "*)";
            }
        }

        //public Dictionary<string, object> Authen(string username, string password)
        //{
        //    Dictionary<string, object> userDetail = null;
        //    string domainAndUsername = null;
        //    object entry = null;
        //    object deSearch = null;
        //    object result = null;
        //    object drEntry = null;
        //    object propEntry = null;
        //    System.Collections.Specialized.StringCollection propertiesToLoad = null;
        //    System.Collections.IDictionaryEnumerator enumEntry = null;

        //    try
        //    {
        //        domainAndUsername = string.Format(@"{0}\{1}", this.Domain, username);
        //        entry = Activator.CreateInstance(_DirectoryEntry, new object[] { this.Path, domainAndUsername, password });
        //        deSearch = Activator.CreateInstance(_DirectorySearcher, new object[] { entry });
        //        _DirectorySearcher.GetProperty("Filter").SetValue(deSearch, string.Format("(sAMAccountName={0})", username));
        //        propertiesToLoad = _DirectorySearcher.GetProperty("PropertiesToLoad").GetValue(deSearch) as System.Collections.Specialized.StringCollection;
        //        propertiesToLoad.Add("sAMAccountName");
        //        result = _DirectorySearcher.GetMethod("FindOne").Invoke(deSearch, new object[] { });
        //        if (result != null)
        //        {
        //            userDetail = new Dictionary<string, object>();
        //            drEntry = _SearchResult.GetMethod("GetDirectoryEntry").Invoke(result, new object[] { });
        //            propEntry = _DirectoryEntry.GetProperty("Properties").GetValue(drEntry);
        //            enumEntry = _PropertyCollection.GetMethod("GetEnumerator").Invoke(propEntry, new object[] { }) as System.Collections.IDictionaryEnumerator;
        //            enumEntry.Reset();
        //            while (enumEntry.MoveNext())
        //            {
        //                if (this.ExceptedProperties.Contains(enumEntry.Key.ToString()))
        //                {
        //                    continue;
        //                }
        //                object value = _PropertyValueCollection.GetProperty("Value").GetValue(enumEntry.Value);
        //                if (value is DateTime)
        //                {
        //                    value = Util.DateTimeToString(value as DateTime?);
        //                }
        //                userDetail.Add(enumEntry.Key.ToString(), value);
        //            }
        //        }
        //    }
        //    catch// (Exception ex)
        //    {
        //        return userDetail;
        //    }
        //    finally
        //    {
        //        _DirectoryEntry.GetMethod("Close").Invoke(entry, new object[] { });
        //    }
        //    return userDetail;
        //}


    }
}