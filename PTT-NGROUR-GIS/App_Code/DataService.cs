using Connector;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.ServiceModel.Web;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Linq;
using System.Web;

[ServiceContract(Namespace = "")]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class DataService
{
    private static Stream GenerateErrorResponse(string errorDetails, string errorMessage, System.Net.HttpStatusCode errorCode)
    {
        Connector.QueryResult queryResult = new Connector.QueryResult();
        queryResult.Success = false;
        queryResult.Message = errorMessage;
        queryResult.AddOutputParam("code", errorCode);
        if (!string.IsNullOrEmpty(errorDetails))
            queryResult.AddOutputParam("details", string.Format("[message:\"{0}\"]", errorDetails));

        return queryResult.ToStream();
    }

    [OperationContract]
    [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
    public Stream DS_TRANSIT(Stream requestStream)
    {
        if (!AMSCSRFCore.IsRequestComeFromProxy(HttpContext.Current.Request))
            return GenerateErrorResponse(null, "Requester IP is not authorized", System.Net.HttpStatusCode.Forbidden);

        IDatabaseConnector dbConnector = new DatabaseConnectorClass();
        return dbConnector.ExecuteStoredProcedure(requestStream).ToStream();
    }

    [OperationContract]
    [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
    public Stream SEND_MAIL(Stream postParam)
    {
        if (!AMSCSRFCore.IsRequestComeFromProxy(HttpContext.Current.Request))
            return GenerateErrorResponse(null, "Requester IP is not authorized", System.Net.HttpStatusCode.Forbidden);

        QueryParameter queryParameter = null;
        QueryResult queryResult = null;
        try
        {
            queryParameter = new QueryParameter(postParam);
            queryResult = new QueryResult();
            AMSCore.SendEmail(queryParameter);
            return queryResult.ToStream();
        }
        catch (Exception ex)
        {
            queryResult = new QueryResult(ex);
        }
        return queryResult.ToStream();
    }

    #region Sample-Service
    [OperationContract]
    [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
    public Stream COOR_IMAGE(Stream postParam)
    {
        if (!AMSCSRFCore.IsRequestComeFromProxy(HttpContext.Current.Request))
            return GenerateErrorResponse(null, "Requester IP is not authorized", System.Net.HttpStatusCode.Forbidden);

        QueryParameter queryParameter = null;
        QueryResult queryResult = null;

        try
        {
            queryParameter = new QueryParameter(postParam);
            queryResult = new QueryResult();
            Dictionary<string, object> coor = null;
            if (queryParameter.Files != null)
            {
                FileParameter fileParameter = queryParameter.Files[0];
                string imagePath = Path.Combine(fileParameter.File.DirectoryName, fileParameter.File.Name);
                Image imageFile = Image.FromFile(imagePath);
                coor = AMSCore.CoordinateImage(imageFile);
            }
            else
            {
                coor = AMSCore.CoordinateImage(queryParameter["IMAGE_URL"].ToString());
            }
        }
        catch (Exception ex)
        {
            queryResult = new QueryResult(ex);
        }

        return queryResult.ToStream();
    }

    [OperationContract]
    [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
    public Stream DS_SAMPLE_FILE_UPLOAD(Stream requestStream)
    {
        if (!AMSCSRFCore.IsRequestComeFromProxy(HttpContext.Current.Request))
            return GenerateErrorResponse(null, "Requester IP is not authorized", System.Net.HttpStatusCode.Forbidden);

        IDatabaseConnector dbConnector = new DatabaseConnectorClass();
        QueryParameter queryParam = new QueryParameter(requestStream);
        QueryResult queryResult = dbConnector.ExecuteStoredProcedure(queryParam);
        string targetPath = AMSCore.WebConfigReadKey("TEMPORARY_PATH");
        if (NetworkConnector.Access(targetPath))
        {
            foreach (FileParameter fileParameter in queryParam.Files)
            {
                fileParameter.Save(targetPath);
            }
        }
        return queryResult.ToStream(true);
    }

    [OperationContract]
    [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
    public Stream CRATE_THUMBNAIL(Stream postParam)
    {
        if (!AMSCSRFCore.IsRequestComeFromProxy(HttpContext.Current.Request))
            return GenerateErrorResponse(null, "Requester IP is not authorized", System.Net.HttpStatusCode.Forbidden);

        QueryParameter queryParameter = null;
        QueryResult queryResult = null;

        try
        {
            queryParameter = new QueryParameter(postParam);
            queryResult = new QueryResult();
            if (queryParameter.Files != null)
            {
                FileParameter fileParameter = queryParameter.Files[0];
                string imagePath = Path.Combine(fileParameter.File.DirectoryName, fileParameter.File.Name);
                Image imageFile = Image.FromFile(imagePath);
                Image img = AMSCore.CreateThumbnail(imageFile, queryParameter);



                string[] fileName = queryParameter.Files[0].Name.Split('.');
                string fileSave = fileParameter.File.DirectoryName + "\\" + fileName[0] + "_Thumbnail" + "." + fileName[fileName.Length - 1].ToString();

                //  Core.FixedSize(imageFile, 256, 256);


                //using (var canvas = Graphics.FromImage(img))
                //{ 


                //}




                //  img.Save(fileSave, imageFile.RawFormat);








            }
            //else
            //{
            //    string imageUrl = queryParameter["IMAGE_URL"].ToString();
            //    Core.ResizeImage(imageUrl, queryParameter);
            //}




        }
        catch (Exception ex)
        {
            queryResult = new QueryResult(ex);
        }

        return queryResult.ToStream();
    }

    #endregion

    #region UM-Service
    [OperationContract]
    [WebInvoke(BodyStyle = WebMessageBodyStyle.WrappedRequest, Method = "POST", ResponseFormat = WebMessageFormat.Json)]
    public Stream DS_I_USER(Stream requestStream)
    {
        if (!AMSCSRFCore.IsRequestComeFromProxy(HttpContext.Current.Request))
            return GenerateErrorResponse(null, "Requester IP is not authorized", System.Net.HttpStatusCode.Forbidden);

        QueryParameter queryParam = null;
        QueryResult queryResult = null;
        try
        {
            queryParam = new QueryParameter(requestStream);
            var SavePath = AMSCore.WebConfigReadKey("PATH_UPLOAD_UM");
            if (queryParam.Files != null && queryParam.Files.Count > 0)
            {
                if (NetworkConnector.Access(SavePath))
                {
                    if (!string.IsNullOrEmpty(queryParam.Parameter["IMG"].ToString()))
                    {
                        string path = Path.Combine(AMSCore.WebConfigReadKey("PATH_UPLOAD_UM"), queryParam.Parameter["IMG"].ToString());
                        if (File.Exists(path))
                        {
                            System.IO.File.Delete(path);
                        }
                    }

                    var FileName = DateTime.Now.Ticks.ToString() + Path.GetFileName(queryParam.Files[0].Name);
                    queryParam.Files[0].Save(SavePath, FileName);
                    queryParam.Parameter["IMG"] = FileName;
                }
            }
            else
            {
                // queryParam.Parameter["IMG" FileName);
            }
            queryResult = new QueryResult();
            queryResult.AddOutputParam("success", true);
            queryResult.AddOutputParam("params", queryParam.Parameter);
        }
        catch (Exception ex)
        {
            queryResult = new QueryResult(ex);
        }
        return queryResult.ToStream(true);
    }
    #endregion
}

