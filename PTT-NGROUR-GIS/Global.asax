<%@ Application Language="C#" %>

<script RunAt="server">

    protected void Application_Start(object sender, EventArgs e)
    {
        // Code that runs on application startup
    }

    protected void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }


    protected void Application_Error(object sender, EventArgs e)
    {
        // Code that runs when an unhandled error occurs

    }

    protected void Session_Start(object sender, EventArgs e)
    {
        // Code that runs when a new session is started

        //to fix
        //"Session state has created a session id, but cannot save it because the response was already flushed by the application."
        string sessionId = Session.SessionID;

		Session["CSRF_TOKEN"] = AMSDuplicateAuthenCore.GenerateToken();
    }

    protected void Session_End(object sender, EventArgs e)
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.

		AMSBasePage.ApplicationLogout(Session);
    }

    protected void Application_BeginRequest(object sender, EventArgs e)
    {

    }

    protected void Application_AuthenticateRequest(object sender, EventArgs e)
    {

    }

    protected void Application_PreSendRequestHeaders()
    {
        try
        {
            Response.Headers.Remove("Server");
            Response.Headers.Remove("X-AspNet-Version");
            Response.Headers.Remove("X-AspNetMvc-Version");
        }
        catch { }
    }


</script>
