using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.SessionState;
using Connector;

/// <summary>
/// Summary description for AMSDuplicateAuthenCore
/// </summary>
public class AMSDuplicateAuthenCore
{
    public static string GetStringSha256Hash(string text)
    {
        if (String.IsNullOrEmpty(text))
            return String.Empty;

        using (var sha = new System.Security.Cryptography.SHA256Managed())
        {
            byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
            byte[] hash = sha.ComputeHash(textData);
            return BitConverter.ToString(hash).Replace("-", String.Empty);
        }
    }

    public static string GenerateToken()
    {
        string token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        return token;
    }

    public static bool IsTokenMatchInDatabase(string userID, string token)
    {
        if (string.IsNullOrEmpty(token))
            return false;

        IDatabaseConnector dbConnector = new DatabaseConnectorClass();
        QueryParameter queryParam = new QueryParameter();
        queryParam.Add("USER_ID", userID);
        queryParam.Add("TOKEN", GetStringSha256Hash(token));

        QueryResult queryResult = dbConnector.ExecuteStoredProcedure("APP_Q_MATCH_TOKEN", queryParam);

        string matchStr = "";

        DataTable resultData = queryResult.DataTable;
        if (resultData != null && resultData.Rows.Count > 0 && resultData.Columns.Count > 0)
            matchStr = queryResult.DataTable.Rows[0][0].ToString();
        
        return matchStr == "1";
    }

    public static void StoreToken(string userID, string token)
    {
        if (string.IsNullOrEmpty(token))
            throw new Exception("Authentication token cannot be empty");

        IDatabaseConnector dbConnector = new DatabaseConnectorClass();
        QueryParameter queryParam = new QueryParameter();
        queryParam.Add("USER_ID", userID);
        queryParam.Add("TOKEN", GetStringSha256Hash(token));

        dbConnector.ExecuteStoredProcedure("APP_I_TOKEN", queryParam);
    }

    public static void ClearToken(string userID, string token)
    {
        if (string.IsNullOrEmpty(token))
            return;

        IDatabaseConnector dbConnector = new DatabaseConnectorClass();
        QueryParameter queryParam = new QueryParameter();
        queryParam.Add("USER_ID", userID);
        queryParam.Add("TOKEN", GetStringSha256Hash(token));

        dbConnector.ExecuteStoredProcedure("APP_D_TOKEN", queryParam);
    }

    public static bool IsValidAuthen(HttpRequest Request, HttpSessionState Session, out string errorMessage)
    {
        string userID = "";
        string cookieToken = ""; 
        string sessionToken = "";

        if(Session["DVS_USER_ID"] != null)
            userID = Session["DVS_USER_ID"].ToString();

        if(Request.Cookies["AUTHEN_TOKEN"] != null)
            cookieToken = Request.Cookies["AUTHEN_TOKEN"].Value;

        if(Session["AUTHEN_TOKEN"] != null)
            sessionToken = Session["AUTHEN_TOKEN"].ToString();

        errorMessage = "";

        if (!string.IsNullOrEmpty(sessionToken) && sessionToken == cookieToken)
        {
            if (AMSCore.WebConfigReadKey("ENABLE_DUPLICATE_AUTHEN_CHECKING") == "true")
            {
                if (IsTokenMatchInDatabase(userID, sessionToken))
                    return true;
                else
                {
                    //Duplicate login detected.
                    errorMessage = "DUPLICATE_LOGIN";
                    return false;
                }
            }
            else
                return true;
        }
        else if (Session["DVS_IS_GUEST"] != null && (bool)Session["DVS_IS_GUEST"] == true)
        {
            return true;
        }
        else 
        {
            errorMessage = "NOT_AUTHORIZED";
            return false;
        }
    }
}