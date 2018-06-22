using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using Connector;

/// <summary>
/// Summary description for AMSCSRFCore
/// </summary>
public class AMSCSRFCore
{
    public static bool IsRequestComeFromProxy(HttpRequest Request)
    {
        if (AMSCore.WebConfigReadKey("ENABLE_DATASERVICE_ACCESS_FROM_PROXY_SERVER_ONLY") == "true")
        {
            string visitorIPAddress = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

            if (string.IsNullOrEmpty(visitorIPAddress))
            {
                visitorIPAddress = HttpContext.Current.Request.UserHostAddress;
            }

            if (string.IsNullOrEmpty(visitorIPAddress))
            {
                visitorIPAddress = string.Empty;
            }

            string ipListStr = AMSCore.WebConfigReadKey("PROXY_SERVER_IP");
            List<string> ipList = ipListStr.Split(new char[] { '|' }).Select(ipStr => ipStr.Trim()).ToList();

            if (ipList.Contains(visitorIPAddress))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
            return true;
    }

    public static bool IsCSRFTokenMatch(HttpSessionState Session, QueryParameter queryParam)
    {
        if (string.Equals(WebConfigurationManager.AppSettings["ENABLE_CSRF_CHECKING"], "true"))
        {
            if (queryParam["CSRF_TOKEN"] != null && queryParam["CSRF_TOKEN"].ToString().Length != 0 && String.Equals(Session["CSRF_TOKEN"].ToString(), queryParam["CSRF_TOKEN"].ToString()))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return true;
        }
    }
}