<%@ WebHandler Language="C#" Class="Proxy" %>

/*
 * DotNet proxy client.
 *
 * Version 1.0
 * See https://github.com/Esri/resource-proxy for more information.
 *
 */

#define TRACE
using System;
using System.Linq;
using System.IO;
using System.Web;
using System.Xml.Serialization;
using System.Web.Caching;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text;

public class Proxy : IHttpHandler,
    System.Web.SessionState.IReadOnlySessionState //*** for passing
{

    class RateMeter
    {
        double _rate; //internal rate is stored in requests per second
        int _countCap;
        double _count = 0;
        DateTime _lastUpdate = DateTime.Now;

        public RateMeter(int rate_limit, int rate_limit_period)
        {
            _rate = (double)rate_limit / rate_limit_period / 60;
            _countCap = rate_limit;
        }

        //called when rate-limited endpoint is invoked
        public bool click()
        {
            TimeSpan ts = DateTime.Now - _lastUpdate;
            _lastUpdate = DateTime.Now;
            //assuming uniform distribution of requests over time,
            //reducing the counter according to # of seconds passed
            //since last invocation
            _count = Math.Max(0, _count - ts.TotalSeconds * _rate);
            if (_count <= _countCap)
            {
                //good to proceed
                _count++;
                return true;
            }
            return false;
        }

        public bool canBeCleaned()
        {
            TimeSpan ts = DateTime.Now - _lastUpdate;
            return _count - ts.TotalSeconds * _rate <= 0;
        }
    }

    private static string PROXY_REFERER = "http://localhost/ashx/Proxy.ashx";
    private static string DEFAULT_OAUTH = "https://www.arcgis.com/sharing/oauth2/";
    private static int CLEAN_RATEMAP_AFTER = 10000; //clean the rateMap every xxxx requests

    private static Object _rateMapLock = new Object();

    private static System.Net.IWebProxy _networkCredential = null;

    private HttpContext _context = null;
    private StringBuilder _logString = null;

    public void ProcessRequest(HttpContext context)
    {
        _context = context;
        _logString = new StringBuilder();

        string errorMessage = "";

        HttpResponse response = context.Response;
        try
        {
            response.Headers.Clear();
        }
        catch { }
        if (context.Request.Url.Query.Length < 1)
        {
            string errorMsg = "No URL specified";
            _logString.AppendLine("ErrorMsg: " + errorMsg);
            AMSCore.WriteRequestLogError(_logString.ToString());
            log(TraceLevel.Error, errorMsg);
            sendErrorResponse(context.Response, null, errorMsg, System.Net.HttpStatusCode.BadRequest);
            return;
        }

        //get the forward URL.
        string uri = context.Request.Url.Query.Substring(1);
        if (uri.StartsWith("ping"))
        {
            //uri = Encryptor.Decode(uri);
            context.Response.Write("{ message: 'pong'}");
            return;
        }

        _logString.AppendLine(uri);

        //if url is encoded, decode it.
        if (uri.StartsWith("http%3a%2f%2f") || uri.StartsWith("https%3a%2f%2f"))
            uri = HttpUtility.UrlDecode(uri);

        log(TraceLevel.Info, uri);
        ServerUrl serverUrl = null;
        ProxyConfig proxyConfig = null;

        //passThrough = true means the forward URL is not listed in <serverUrls> tag in proxy.config file.
        // so that request must be check for security because it is not recongized in proxy.config
        bool passThrough = false;
        try
        {
            _logString.AppendLine("-> getConfig");
            proxyConfig = getConfig();
            _logString.AppendLine("-> proxyConfig.GetConfigServerUrl");

            //find that the forward URL is match at least one item in the list of <serverUrls> in proxy.config
            serverUrl = proxyConfig.GetConfigServerUrl(uri);
            _logString.AppendLine("-> proxyConfig.getNetworkCredential");
            _networkCredential = proxyConfig.getNetworkCredential();
            passThrough = serverUrl == null;
        }
        //if XML couldn't be parsed
        catch (InvalidOperationException ex)
        {
            string errorMsg = ex.GetBaseException().Message + " " + uri;
            _logString.AppendLine("ErrorMsg -> InvalidOperationException: " + ex.GetBaseException().Message);
            AMSCore.WriteRequestLogError(_logString.ToString());
            log(TraceLevel.Error, errorMsg);
            sendErrorResponse(context.Response, null, errorMsg, System.Net.HttpStatusCode.InternalServerError);
            return;
        }
        //if mustMatch was set to true and URL wasn't in the list
        catch (ArgumentException ex)
        {
            string errorMsg = ex.Message + " " + uri;
            _logString.AppendLine("ErrorMsg -> ArgumentException: " + ex.Message);
            AMSCore.WriteRequestLogError(_logString.ToString());
            log(TraceLevel.Error, errorMsg);
            sendErrorResponse(context.Response, null, errorMsg, System.Net.HttpStatusCode.Forbidden);
            return;
        }

        //any passthrough request must be check for authentication by default.
        if ((serverUrl == null || !serverUrl.BypassAuthen) &&
            !AMSDuplicateAuthenCore.IsValidAuthen(_context.Request, _context.Session, out errorMessage))
        {
            AMSBasePage.ApplicationLogout(_context.Session, _context.Response);
            _context.Response.Clear();

            string userID = "";
            if (_context.Session["DVS_USER_ID"] != null)
                userID = _context.Session["DVS_USER_ID"].ToString();

            if (errorMessage == "DUPLICATE_LOGIN")
            {
                string errorDetail = string.Format("userID : {0} Duplicate login detected.", userID);
                _logString.AppendLine("ErrorMsg: " + errorDetail);
                AMSCore.WriteRequestLogError(_logString.ToString());
                log(TraceLevel.Error, errorDetail);

                string errorDetailPageUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.VirtualPathUtility.ToAbsolute("~/error/duplicateLogin.html");
                sendErrorResponse(context.Response, "redirect to url @( " + errorDetailPageUrl + " )@", "DUPLICATE_LOGIN", System.Net.HttpStatusCode.Forbidden);
            }
            else
            {
                string logoutPageUrl = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + System.Web.VirtualPathUtility.ToAbsolute("~/?p=logout");

                sendErrorResponse(context.Response, "redirect to url @( " + logoutPageUrl + " )@", "NOT_AUTHORIZED", System.Net.HttpStatusCode.Forbidden);
            }

            return;
        }

        //use actual request header instead of a placeholder, if present
        if (context.Request.Headers["referer"] != null)
            PROXY_REFERER = context.Request.Headers["referer"];

        //referer
        //check against the list of referers if they have been specified in the proxy.config
        _logString.AppendLine("-> check against the list of referers");
        String[] allowedReferersArray = ProxyConfig.GetAllowedReferersArray();
        if (allowedReferersArray != null && allowedReferersArray.Length > 0)
        {
            bool allowed = false;
            string requestReferer = context.Request.Headers["referer"];

            foreach (var referer in allowedReferersArray)
            {
                if ((allowedReferersArray.Length == 1) && referer == String.Empty)
                    break;

                if (requestReferer != null && requestReferer != String.Empty && (ProxyConfig.isUrlPrefixMatch(referer, requestReferer)) || referer == "*")
                {
                    allowed = true;
                    break;
                }
            }

            if (!allowed)
            {
                string errorMsg = "Proxy is being used from an unsupported referer: " + context.Request.Headers["referer"];
                _logString.AppendLine("ErrorMsg -> Referer: " + errorMsg);
                AMSCore.WriteRequestLogError(_logString.ToString());
                log(TraceLevel.Error, errorMsg);
                sendErrorResponse(context.Response, null, errorMsg, System.Net.HttpStatusCode.Forbidden);
                return;
            }
        }

        //Throttling: checking the rate limit coming from particular client IP
        if (!passThrough && serverUrl.RateLimit > -1)
        {
            lock (_rateMapLock)
            {
                _logString.AppendLine("-> checking the rate limit coming from particular client IP");
                ConcurrentDictionary<string, RateMeter> ratemap = (ConcurrentDictionary<string, RateMeter>)context.Application["rateMap"];
                if (ratemap == null)
                {
                    ratemap = new ConcurrentDictionary<string, RateMeter>();
                    context.Application["rateMap"] = ratemap;
                    context.Application["rateMap_cleanup_counter"] = 0;
                }
                string key = "[" + serverUrl.Url + "]x[" + context.Request.UserHostAddress + "]";
                RateMeter rate;
                if (!ratemap.TryGetValue(key, out rate))
                {
                    rate = new RateMeter(serverUrl.RateLimit, serverUrl.RateLimitPeriod);
                    ratemap.TryAdd(key, rate);
                }
                if (!rate.click())
                {
                    _logString.AppendLine("ErrorMsg -> RateLimited: " + " Pair " + key + " is throttled to " + serverUrl.RateLimit + " requests per " + serverUrl.RateLimitPeriod + " minute(s). Come back later.");
                    AMSCore.WriteRequestLogError(_logString.ToString());
                    log(TraceLevel.Warning, " Pair " + key + " is throttled to " + serverUrl.RateLimit + " requests per " + serverUrl.RateLimitPeriod + " minute(s). Come back later.");
                    sendErrorResponse(context.Response, "This is a metered resource, number of requests have exceeded the rate limit interval.", "Unable to proxy request for requested resource", System.Net.HttpStatusCode.PaymentRequired);
                    return;
                }

                //making sure the rateMap gets periodically cleaned up so it does not grow uncontrollably
                int cnt = (int)context.Application["rateMap_cleanup_counter"];
                cnt++;
                if (cnt >= CLEAN_RATEMAP_AFTER)
                {
                    cnt = 0;
                    cleanUpRatemap(ratemap);
                }
                context.Application["rateMap_cleanup_counter"] = cnt;
            }
        }

        //readying body (if any) of POST request
        _logString.AppendLine("-> readying body of POST request");
        byte[] postBody = null;
        try
        {
            Connector.QueryParameter queryParam = null;

            if (!passThrough && serverUrl.UseSession)
            {
                postBody = getSession(context, out queryParam);
                if (queryParam.IsUseSession == true && context.Session.IsNewSession)
                {
                    throw new Exception("__SESSION_TIMEOUT__");
                }
            }
            else
            {
                queryParam = new Connector.QueryParameter();

                foreach (string key in context.Request.Params.AllKeys)
                    queryParam.Add(key, context.Request.Params[key]);

                postBody = readRequestPostBody(context);
            }
            _logString.AppendLine("postBody: " + Encoding.UTF8.GetString(postBody));

            //any passthrough request must be check for CSRF by default.
            if ((passThrough || serverUrl == null || serverUrl.CheckCSRFToken) && !AMSCSRFCore.IsCSRFTokenMatch(_context.Session, queryParam))
            {
                //invalid CSRF token should not clear the session because hacker can revoke the user session by sending an invalid CSRF key.

                sendErrorResponse(context.Response, "", "CSRF_TOKEN_MISMATCH", System.Net.HttpStatusCode.Forbidden);
                return;
            }
        }
        catch (TimeoutException ex)
        {
            //response.StatusCode = (int)System.Net.HttpStatusCode.;
            //response.StatusCode = 500;
            _logString.AppendLine("ErrorMsg -> TimeoutException: " + ex.GetBaseException().Message);
            AMSCore.WriteRequestLogError(_logString.ToString());
            response.Write(new Connector.QueryResult(ex).ToJson());
            //string errorMsg = ex.Message;
            //log(TraceLevel.Error, errorMsg);
            //sendErrorResponse(context.Response, null, errorMsg, System.Net.HttpStatusCode.ExpectationFailed);
            return;
        }
        catch (Exception ex)
        {
            _logString.AppendLine("ErrorMsg -> Exception: " + ex.GetBaseException().Message);
            AMSCore.WriteRequestLogError(_logString.ToString());
            response.Write(new Connector.QueryResult(ex).ToJson());
            return;
        }

        string post = System.Text.Encoding.UTF8.GetString(postBody);

        //if token comes with client request, it takes precedence over token or credentials stored in configuration
        bool hasClientToken = uri.Contains("?token=") || uri.Contains("&token=") || post.Contains("?token=") || post.Contains("&token=");
        string token = string.Empty;
        if (!passThrough && !hasClientToken)
        {
            _logString.AppendLine("-> readying token");
            // Get new token and append to the request.
            // But first, look up in the application scope, maybe it's already there:
            token = (String)context.Application["token_for_" + serverUrl.Url];
            bool tokenIsInApplicationScope = !String.IsNullOrEmpty(token);

            //if still no token, let's see if there is an access token or if are credentials stored in configuration which we can use to obtain new token
            if (!tokenIsInApplicationScope)
            {
                token = serverUrl.AccessToken;
                if (String.IsNullOrEmpty(token))
                    token = getNewTokenIfCredentialsAreSpecified(serverUrl, uri);
            }

            if (!String.IsNullOrEmpty(token) && !tokenIsInApplicationScope)
            {
                //storing the token in Application scope, to do not waste time on requesting new one untill it expires or the app is restarted.
                context.Application.Lock();
                context.Application["token_for_" + serverUrl.Url] = token;
                context.Application.UnLock();
            }
        }

        //name by which token parameter is passed (if url actually came from the list)
        string tokenParamName = serverUrl != null ? serverUrl.TokenParamName : null;


        if (String.IsNullOrEmpty(tokenParamName))
            tokenParamName = "token";

        //forwarding original request
        System.Net.WebResponse serverResponse = null;
        try
        {
            _logString.AppendLine("-> forwarding request");
            serverResponse = forwardToServer(context, addTokenToUri(uri, token, tokenParamName), postBody);
        }
        catch (System.Net.WebException webExc)
        {

            string errorMsg = webExc.Message + " " + uri;
            _logString.AppendLine("ErrorMsg -> System.Net.WebException: " + webExc.Message);
            AMSCore.WriteRequestLogError(_logString.ToString());

            log(TraceLevel.Error, errorMsg);

            if (webExc.Response != null)
            {
                string contentEncoding = (webExc.Response as System.Net.HttpWebResponse).ContentEncoding;
                context.Response.AddHeader("Content-Encoding", contentEncoding);

                using (Stream responseStream = webExc.Response.GetResponseStream())
                {
                    byte[] bytes = new byte[32768];
                    int bytesRead = 0;

                    while ((bytesRead = responseStream.Read(bytes, 0, bytes.Length)) > 0)
                    {
                        responseStream.Write(bytes, 0, bytesRead);
                    }

                    context.Response.StatusCode = (int)(webExc.Response as System.Net.HttpWebResponse).StatusCode;
                    context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                }
            }
            else
            {
                System.Net.HttpStatusCode statusCode = System.Net.HttpStatusCode.InternalServerError;
                sendErrorResponse(context.Response, null, errorMsg, statusCode);
            }
            return;
        }

        _logString.AppendLine("-> fetchAndPassBackToClient");
        if (passThrough || string.IsNullOrEmpty(token) || hasClientToken)
        {
            //if token is not required or provided by the client, just fetch the response as is:
            fetchAndPassBackToClient(serverResponse, response, true);
        }
        else
        {
            //credentials for secured service have come from configuration file:
            //it means that the proxy is responsible for making sure they were properly applied:

            //first attempt to send the request:
            bool tokenRequired = fetchAndPassBackToClient(serverResponse, response, false);


            //checking if previously used token has expired and needs to be renewed
            if (tokenRequired)
            {
                _logString.AppendLine("-> Renewing token and trying again.");
                log(TraceLevel.Info, "Renewing token and trying again.");
                //server returned error - potential cause: token has expired.
                //we'll do second attempt to call the server with renewed token:
                token = getNewTokenIfCredentialsAreSpecified(serverUrl, uri);
                serverResponse = forwardToServer(context, addTokenToUri(uri, token, tokenParamName), postBody);

                //storing the token in Application scope, to do not waste time on requesting new one untill it expires or the app is restarted.
                context.Application.Lock();
                context.Application["token_for_" + serverUrl.Url] = token;
                context.Application.UnLock();

                _logString.AppendLine("-> fetchAndPassBackToClient");
                fetchAndPassBackToClient(serverResponse, response, true);
            }
        }
        AMSCore.WriteRequestLogInfo(_logString.ToString());
        response.End();
    }

    public bool IsReusable
    {
        get { return true; }
    }

    /**
    * Private
    */

    private byte[] getSession(HttpContext context, out Connector.QueryParameter queryParam)
    {
        byte[] postBody = new byte[0];
        queryParam = null;
        try
        {
            queryParam = new Connector.QueryParameter(context);
            if (queryParam == null) return postBody;
            postBody = queryParam.ToBytes();
            //foreach (string s in context.Session.Keys)
            //{
            //    if (!s.StartsWith(queryParam.prefixSession)) continue;
            //    queryParam.Add(s, context.Session[s]);
            //}
        }
        catch { }
        return postBody;
    }

    private byte[] readRequestPostBody(HttpContext context)
    {
        if (context.Request.InputStream.Length > 0)
        {
            byte[] bytes = new byte[context.Request.InputStream.Length];
            context.Request.InputStream.Read(bytes, 0, (int)context.Request.InputStream.Length);
            return bytes;
        }
        return new byte[0];
    }

    private System.Net.WebResponse forwardToServer(HttpContext context, string uri, byte[] postBody)
    {
        return
            postBody.Length > 0 ?
            doHTTPRequest(uri, postBody, "POST", context.Request.Headers["referer"], context.Request.ContentType) :
            doHTTPRequest(uri, context.Request.HttpMethod);
    }

    private bool fetchAndPassBackToClient(System.Net.WebResponse serverResponse, HttpResponse clientResponse, bool ignoreAuthenticationErrors)
    {
        if (serverResponse != null)
        {
            clientResponse.ContentType = serverResponse.ContentType;
            using (Stream byteStream = serverResponse.GetResponseStream())
            {
                if (!string.IsNullOrEmpty(_context.Request.ContentType) && _context.Request.ContentType.StartsWith("multipart/form-data"))
                {
                    clientResponse.ContentType = "text/html";
                    clientResponse.ContentEncoding = System.Text.Encoding.UTF8;
                    using (StreamReader sr = new StreamReader(byteStream))
                    {
                        string strResponse = sr.ReadToEnd();
                        _logString.AppendLine("Response: " + strResponse.Trim());
                        if (
                            !ignoreAuthenticationErrors
                            && strResponse.IndexOf("{\"error\":{") > -1
                            && (strResponse.IndexOf("\"code\":498") > -1 || strResponse.IndexOf("\"code\":499") > -1)
                        )
                        {
                            AMSCore.WriteRequestLogError(_logString.ToString());
                            return true;
                        }
                        clientResponse.Write(strResponse);
                    }
                }
                // Text response
                else if (serverResponse.ContentType.Contains("text") ||
                    serverResponse.ContentType.Contains("json") ||
                    serverResponse.ContentType.Contains("xml"))
                {
                    using (StreamReader sr = new StreamReader(byteStream))
                    {
                        string strResponse = sr.ReadToEnd();
                        _logString.AppendLine("Response: " + strResponse.Trim());
                        if (
                            !ignoreAuthenticationErrors
                            && strResponse.IndexOf("{\"error\":{") > -1
                            && (strResponse.IndexOf("\"code\":498") > -1 || strResponse.IndexOf("\"code\":499") > -1)
                        )
                        {
                            AMSCore.WriteRequestLogError(_logString.ToString());
                            return true;
                        }
                        clientResponse.Write(strResponse);
                    }
                }
                else
                {
                    // Binary response (image, lyr file, other binary file)

                    // Tell client not to cache the image since it's dynamic
                    clientResponse.CacheControl = "no-cache";
                    byte[] buffer = new byte[32768];
                    int read;
                    while ((read = byteStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        clientResponse.OutputStream.Write(buffer, 0, read);
                    }
                    clientResponse.OutputStream.Close();
                    if (clientResponse.ContentType == "application/octet-stream")
                    {
                        try
                        {
                            Connector.QueryResult queryResult = new Connector.QueryResult(Encoding.UTF8.GetString(buffer).Trim('\0'));
                            _logString.AppendLine("Response: " + queryResult.ToJson());
                            if (!queryResult.Success)
                            {
                                AMSCore.WriteRequestLogError(_logString.ToString());
                            }
                        }
                        catch { }
                    }
                }
                serverResponse.Close();
            }
        }
        return false;
    }

    private System.Net.WebResponse doHTTPRequest(string uri, string method, bool fixPOSTUri = false)
    {
        byte[] bytes = new byte[0];
        String contentType = null;

        if (method.Equals("POST"))
        {
            String[] uriArray = uri.Split(new[] { '?' }, 2);

            if (uriArray.Length > 1)
            {
                contentType = "application/x-www-form-urlencoded";
                String queryString = uriArray[1];
                if (fixPOSTUri)
                {
                    uri = uriArray[0];
                }
                bytes = System.Text.Encoding.UTF8.GetBytes(queryString);
            }
        }

        return doHTTPRequest(uri, bytes, method, PROXY_REFERER, contentType);
    }

    private System.Net.WebResponse doHTTPRequest(string uri, byte[] bytes, string method, string referer, string contentType)
    {
        System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
        System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(uri);
        req.ServicePoint.Expect100Continue = false;
        req.Referer = referer;
        req.Method = method;
        req.Proxy = _networkCredential;
        if (bytes != null && bytes.Length > 0 || method == "POST")
        {
            req.Method = "POST";
            req.ContentType = string.IsNullOrEmpty(contentType) ? "application/x-www-form-urlencoded" : contentType;
            req.ContentLength = bytes != null ? bytes.Length : 0;
            using (Stream outputStream = req.GetRequestStream())
            {
                outputStream.Write(bytes, 0, bytes.Length);
            }
        }
        return req.GetResponse();
    }

    private string webResponseToString(System.Net.WebResponse serverResponse)
    {
        using (Stream byteStream = serverResponse.GetResponseStream())
        {
            using (StreamReader sr = new StreamReader(byteStream))
            {
                string strResponse = sr.ReadToEnd();
                return strResponse;
            }
        }
    }

    private string getNewTokenIfCredentialsAreSpecified(ServerUrl su, string reqUrl)
    {
        string token = "";
        string infoUrl = "";

        bool isUserLogin = !String.IsNullOrEmpty(su.Username) && !String.IsNullOrEmpty(su.Password);
        bool isAppLogin = !String.IsNullOrEmpty(su.ClientId) && !String.IsNullOrEmpty(su.ClientSecret);
        if (isUserLogin || isAppLogin)
        {
            log(TraceLevel.Info, "Matching credentials found in configuration file. OAuth 2.0 mode: " + isAppLogin);
            if (isAppLogin)
            {
                //OAuth 2.0 mode authentication
                //"App Login" - authenticating using client_id and client_secret stored in config
                su.OAuth2Endpoint = string.IsNullOrEmpty(su.OAuth2Endpoint) ? DEFAULT_OAUTH : su.OAuth2Endpoint;
                if (su.OAuth2Endpoint[su.OAuth2Endpoint.Length - 1] != '/')
                    su.OAuth2Endpoint += "/";
                log(TraceLevel.Info, "Service is secured by " + su.OAuth2Endpoint + ": getting new token...");
                string uri = su.OAuth2Endpoint + "token?client_id=" + su.ClientId + "&client_secret=" + su.ClientSecret + "&grant_type=client_credentials&f=json";
                string tokenResponse = webResponseToString(doHTTPRequest(uri, "POST", true));
                token = extractToken(tokenResponse, "token");
                if (!string.IsNullOrEmpty(token))
                    token = exchangePortalTokenForServerToken(token, su);
            }
            else
            {
                //standalone ArcGIS Server/ArcGIS Online token-based authentication

                //if a request is already being made to generate a token, just let it go
                if (reqUrl.ToLower().Contains("/generatetoken"))
                {
                    string tokenResponse = webResponseToString(doHTTPRequest(reqUrl, "POST", true));
                    token = extractToken(tokenResponse, "token");
                    return token;
                }

                //lets look for '/rest/' in the requested URL (could be 'rest/services', 'rest/community'...)
                if (reqUrl.ToLower().Contains("/rest/"))
                    infoUrl = reqUrl.Substring(0, reqUrl.IndexOf("/rest/", StringComparison.OrdinalIgnoreCase));

                //if we don't find 'rest', lets look for the portal specific 'sharing' instead
                else if (reqUrl.ToLower().Contains("/sharing/"))
                {
                    infoUrl = reqUrl.Substring(0, reqUrl.IndexOf("/sharing/", StringComparison.OrdinalIgnoreCase));
                    infoUrl = infoUrl + "/sharing";
                }
                else
                    throw new ApplicationException("Unable to determine the correct URL to request a token to access private resources");

                if (infoUrl != "")
                {
                    log(TraceLevel.Info, " Querying security endpoint...");
                    infoUrl += "/rest/info?f=json";
                    //lets send a request to try and determine the URL of a token generator
                    string infoResponse = webResponseToString(doHTTPRequest(infoUrl, "GET"));
                    String tokenServiceUri = getJsonValue(infoResponse, "tokenServicesUrl");
                    if (string.IsNullOrEmpty(tokenServiceUri))
                        tokenServiceUri = getJsonValue(infoResponse, "tokenServiceUrl");
                    if (tokenServiceUri != "")
                    {
                        log(TraceLevel.Info, " Service is secured by " + tokenServiceUri + ": getting new token...");
                        string uri = tokenServiceUri + "?f=json&request=getToken&referer=" + PROXY_REFERER + "&expiration=60&username=" + su.Username + "&password=" + su.Password;
                        string tokenResponse = webResponseToString(doHTTPRequest(uri, "POST", true));
                        token = extractToken(tokenResponse, "token");
                    }
                }


            }
        }
        return token;
    }

    private string exchangePortalTokenForServerToken(string portalToken, ServerUrl su)
    {
        //ideally, we should POST the token request
        log(TraceLevel.Info, " Exchanging Portal token for Server-specific token for " + su.Url + "...");
        string uri = su.OAuth2Endpoint.Substring(0, su.OAuth2Endpoint.IndexOf("/oauth2/", StringComparison.OrdinalIgnoreCase)) +
             "/generateToken?token=" + portalToken + "&serverURL=" + su.Url + "&f=json";
        string tokenResponse = webResponseToString(doHTTPRequest(uri, "GET"));
        return extractToken(tokenResponse, "token");
    }

    private static void sendErrorResponse(HttpResponse response, String errorDetails, String errorMessage, System.Net.HttpStatusCode errorCode)
    {
        Connector.QueryResult queryResult = new Connector.QueryResult();
        queryResult.Success = false;
        queryResult.Message = errorMessage;
        queryResult.AddOutputParam("code", errorCode);
        if (!string.IsNullOrEmpty(errorDetails))
            queryResult.AddOutputParam("details", string.Format("[message:\"{0}\"]", errorDetails));
        response.StatusCode = (int)errorCode;
        //this displays our customized error messages instead of IIS's custom errors
        response.TrySkipIisCustomErrors = true;
        response.Write(queryResult.ToJson());
        response.Flush();
    }

    private static string getClientIp(HttpRequest request)
    {
        if (request == null)
            return null;
        string remoteAddr = request.ServerVariables["HTTP_X_FORWARDED_FOR"];
        if (string.IsNullOrWhiteSpace(remoteAddr))
        {
            remoteAddr = request.ServerVariables["REMOTE_ADDR"];
        }
        else
        {
            // the HTTP_X_FORWARDED_FOR may contain an array of IP, this can happen if you connect through a proxy.
            string[] ipRange = remoteAddr.Split(',');
            remoteAddr = ipRange[ipRange.Length - 1];
        }
        return remoteAddr;
    }

    private string addTokenToUri(string uri, string token, string tokenParamName)
    {
        if (!String.IsNullOrEmpty(token))
            uri += uri.Contains("?") ? "&" + tokenParamName + "=" + token : "?" + tokenParamName + "=" + token;
        return uri;
    }

    private string extractToken(string tokenResponse, string key)
    {
        string token = getJsonValue(tokenResponse, key);
        if (string.IsNullOrEmpty(token))
            log(TraceLevel.Error, " Token cannot be obtained: " + tokenResponse);
        else
            log(TraceLevel.Info, " Token obtained: " + token);
        return token;
    }

    private string getJsonValue(string text, string key)
    {
        int i = text.IndexOf(key);
        String value = "";
        if (i > -1)
        {
            value = text.Substring(text.IndexOf(':', i) + 1).Trim();
            value = value.Length > 0 && value[0] == '"' ?
                value.Substring(1, value.IndexOf('"', 1) - 1) :
                value = value.Substring(0, Math.Max(0, Math.Min(Math.Min(value.IndexOf(","), value.IndexOf("]")), value.IndexOf("}"))));
        }
        return value;
    }

    private void cleanUpRatemap(ConcurrentDictionary<string, RateMeter> ratemap)
    {
        foreach (string key in ratemap.Keys)
        {
            RateMeter rate = ratemap[key];
            if (rate.canBeCleaned())
                ratemap.TryRemove(key, out rate);
        }
    }

    /**
    * Static
    */
    private static ProxyConfig getConfig()
    {
        ProxyConfig config = ProxyConfig.GetCurrentConfig();
        if (config != null)
            return config;
        else
            throw new ApplicationException("The proxy configuration file cannot be found, or is not readable.");
    }

    //writing Log file
    private static void log(TraceLevel logLevel, string msg)
    {
        string logMessage = string.Format("{0} {1}", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), msg);
        if (TraceLevel.Error == logLevel)
        {
            Trace.TraceError(logMessage);
        }
        else if (TraceLevel.Warning == logLevel)
        {
            Trace.TraceWarning(logMessage);
        }
        else
        {
            Trace.TraceInformation(logMessage);
        }
    }

    private static int Bytes2Int(byte b1, byte b2, byte b3)
    {
        int r = 0;
        byte b0 = 0xff;

        if ((b1 & 0x80) != 0) r |= b0 << 24;
        r |= b1 << 16;
        r |= b2 << 8;
        r |= b3;
        return r;
    }
}


[XmlRoot("ProxyConfig")]
public class ProxyConfig
{
    private static object _lockobject = new object();
    public static ProxyConfig LoadProxyConfig(string fileName)
    {
        ProxyConfig config = null;
        lock (_lockobject)
        {
            if (System.IO.File.Exists(fileName))
            {
                XmlSerializer reader = new XmlSerializer(typeof(ProxyConfig));
                using (System.IO.StreamReader file = new System.IO.StreamReader(fileName))
                {
                    try
                    {
                        config = (ProxyConfig)reader.Deserialize(file);
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
        }
        return config;
    }

    public static ProxyConfig GetCurrentConfig()
    {
        ProxyConfig config = HttpRuntime.Cache["proxyConfig"] as ProxyConfig;
        if (config == null)
        {
            string fileName = HttpContext.Current.Server.MapPath("~/proxy.config");
            config = LoadProxyConfig(fileName);
            if (config != null)
            {
                CacheDependency dep = new CacheDependency(fileName);
                HttpRuntime.Cache.Insert("proxyConfig", config, dep);
            }
        }
        return config;
    }

    //referer
    //create an array with valid referers using the allowedReferers String that is defined in the proxy.config
    public static String[] GetAllowedReferersArray()
    {
        if (allowedReferers == null)
            return null;

        return allowedReferers.Split(',');
    }

    //referer
    //check if URL starts with prefix...
    public static bool isUrlPrefixMatch(String prefix, String uri)
    {

        return uri.ToLower().StartsWith(prefix.ToLower()) ||
                    uri.ToLower().Replace("https://", "http://").StartsWith(prefix.ToLower()) ||
                    uri.ToLower().Substring(uri.IndexOf("//")).StartsWith(prefix.ToLower());
    }

    public System.Net.IWebProxy getNetworkCredential()
    {
        System.Net.IWebProxy reqProxy = null;
        System.Net.ICredentials reqCredential = null;
        if (string.IsNullOrEmpty(ProxyCredential.Address)) return null;
        reqProxy = new System.Net.WebProxy(ProxyCredential.Address, ProxyCredential.BypassOnLocal, ProxyCredential.BypassUrl.Split(','));
        if (!string.IsNullOrEmpty(ProxyCredential.Username) && !string.IsNullOrEmpty(ProxyCredential.Password))
        {
            if (string.IsNullOrEmpty(ProxyCredential.Domain))
            {
                reqCredential = new System.Net.NetworkCredential(ProxyCredential.Username, ProxyCredential.Password);
            }
            else
            {
                reqCredential = new System.Net.NetworkCredential(ProxyCredential.Username, ProxyCredential.Password, ProxyCredential.Domain);
            }
            reqProxy.Credentials = reqCredential;
        }
        return reqProxy;
    }

    ServerUrl[] serverUrls;
    bool mustMatch;
    //referer
    static String allowedReferers;

    [XmlArray("serverUrls")]
    [XmlArrayItem("serverUrl")]
    public ServerUrl[] ServerUrls
    {
        get { return this.serverUrls; }
        set
        {
            this.serverUrls = value;
        }
    }
    [XmlAttribute("mustMatch")]
    public bool MustMatch
    {
        get { return mustMatch; }
        set
        { mustMatch = value; }
    }

    //referer
    [XmlAttribute("allowedReferers")]
    public string AllowedReferers
    {
        get { return allowedReferers; }
        set
        {
            allowedReferers = value;
        }
    }

    [XmlElement("proxyCredential")]
    public ProxyCredentialClass ProxyCredential
    {
        get;
        set;
    }

    public ServerUrl GetConfigServerUrl(string uri)
    {
        //split both request and proxy.config urls and compare them
        string[] uriParts = uri.Split(new char[] { '/', '?' }, StringSplitOptions.RemoveEmptyEntries);
        string[] configUriParts = new string[] { };
        string uriCompare = string.Empty;
        foreach (ServerUrl su in serverUrls)
        {
            //if a relative path is specified in the proxy.config, append what's in the request itself
            if (!su.Url.StartsWith("http"))
                uriCompare = string.Format("{0}://{1}{2}/{3}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, HttpContext.Current.Request.ApplicationPath, su.Url);
            else
                uriCompare = su.Url;
            //su.Url = AMSCore.ApplicationUrl + su.Url;
            //su.Url = su.Url.Insert(0, uriParts[0]);
            //else if (!su.Url.StartsWith("/"))


            configUriParts = uriCompare.Split(new char[] { '/', '?' }, StringSplitOptions.RemoveEmptyEntries);

            //if the request has less parts than the config, don't allow
            if (configUriParts.Length > uriParts.Length) continue;

            int i = 0;
            for (i = 0; i < configUriParts.Length; i++)
            {

                if (!configUriParts[i].ToLower().Equals(HttpUtility.UrlDecode(uriParts[i].ToLower()))) break;
            }
            if (i == configUriParts.Length)
            {
                //if the urls don't match exactly, and the individual matchAll tag is 'false', don't allow
                if (configUriParts.Length == uriParts.Length || su.MatchAll)
                    return su;
            }
        }

        if (mustMatch)
            throw new ArgumentException("Proxy is being used for an unsupported service:");

        return null;
    }


}

public class ServerUrl
{
    string url;
    bool matchAll;
    bool useSession;
    bool bypassAuthen;
    bool checkCSRFToken;
    string oauth2Endpoint;
    string username;
    string password;
    string clientId;
    string clientSecret;
    string accessToken;
    string tokenParamName;
    string rateLimit;
    string rateLimitPeriod;

    [XmlAttribute("url")]
    public string Url
    {
        get { return url; }
        set { url = value; }
    }
    [XmlAttribute("matchAll")]
    public bool MatchAll
    {
        get { return matchAll; }
        set { matchAll = value; }
    }
    [XmlAttribute("bypassAuthen")]
    public bool BypassAuthen
    {
        get { return bypassAuthen; }
        set { bypassAuthen = value; }
    }
    [XmlAttribute("checkCSRFToken")]
    public bool CheckCSRFToken
    {
        get { return checkCSRFToken; }
        set { checkCSRFToken = value; }
    }
    [XmlAttribute("useSession")]
    public bool UseSession
    {
        get { return useSession; }
        set { useSession = value; }
    }
    [XmlAttribute("oauth2Endpoint")]
    public string OAuth2Endpoint
    {
        get { return oauth2Endpoint; }
        set { oauth2Endpoint = value; }
    }
    [XmlAttribute("username")]
    public string Username
    {
        get { return username; }
        set { username = value; }
    }
    [XmlAttribute("password")]
    public string Password
    {
        get { return password; }
        set { password = value; }
    }
    [XmlAttribute("clientId")]
    public string ClientId
    {
        get { return clientId; }
        set { clientId = value; }
    }
    [XmlAttribute("clientSecret")]
    public string ClientSecret
    {
        get { return clientSecret; }
        set { clientSecret = value; }
    }
    [XmlAttribute("accessToken")]
    public string AccessToken
    {
        get { return accessToken; }
        set { accessToken = value; }
    }
    [XmlAttribute("tokenParamName")]
    public string TokenParamName
    {
        get { return tokenParamName; }
        set { tokenParamName = value; }
    }
    [XmlAttribute("rateLimit")]
    public int RateLimit
    {
        get { return string.IsNullOrEmpty(rateLimit) ? -1 : int.Parse(rateLimit); }
        set { rateLimit = value.ToString(); }
    }
    [XmlAttribute("rateLimitPeriod")]
    public int RateLimitPeriod
    {
        get { return string.IsNullOrEmpty(rateLimitPeriod) ? 60 : int.Parse(rateLimitPeriod); }
        set { rateLimitPeriod = value.ToString(); }
    }
}

public class ProxyCredentialClass
{
    [XmlAttribute("address")]
    public string Address
    {
        get;
        set;
    }

    [XmlAttribute("domain")]
    public string Domain
    {
        get;
        set;
    }

    [XmlAttribute("username")]
    public string Username
    {
        get;
        set;
    }

    [XmlAttribute("password")]
    public string Password
    {
        get;
        set;
    }

    [XmlAttribute("bypassOnLocal")]
    public bool BypassOnLocal
    {
        get;
        set;
    }

    [XmlAttribute("bypassUrl")]
    public string BypassUrl
    {
        get;
        set;
    }



}