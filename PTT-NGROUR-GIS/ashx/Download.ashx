<%@ WebHandler Language="C#" Class="Download" %>

using System;
using System.Linq;
using System.Web;

public class Download : IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        try
        {
            if (!string.IsNullOrEmpty(context.Request["p"]))
            {
                object expClass = Activator.CreateInstance(typeof(AMSCore).Assembly.GetType(context.Request["p"]), new object[]{
                new Connector.QueryParameter(context)
            });

                string fullname = (string)expClass.GetType().GetProperty("FullName").GetValue(expClass);
                string filename = (string)expClass.GetType().GetProperty("FileName").GetValue(expClass);
                string fileContentType = (string)expClass.GetType().GetProperty("FileContentType").GetValue(expClass);
                byte[] fileContent = (byte[])expClass.GetType().GetProperty("FileContent").GetValue(expClass);
                bool isAddHeader = !string.IsNullOrEmpty(filename);
                if (!string.IsNullOrEmpty(fullname))
                {
                    if (Connector.NetworkConnector.Access(fullname))
                    {
                        System.IO.FileInfo fileInfo = new System.IO.FileInfo(fullname);
                        if (fileInfo.Exists)
                        {
                            if (isAddHeader)
                            {
                                if (context.Request.UserAgent.IndexOf("Mozilla") != -1 && context.Request.UserAgent.IndexOf("Edge") == -1)
                                {
                                    context.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
                                }
                                else
                                {
                                    context.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8));
                                }
                                context.Response.AddHeader("Content-Length", fileInfo.Length.ToString());
                            }
                            context.Response.ContentType = string.IsNullOrEmpty(fileContentType) ? System.Web.MimeMapping.GetMimeMapping(filename) : fileContentType;
                            context.Response.ClearContent();
                            context.Response.WriteFile(fullname);
                        }
                        else
                        {
                            if (isAddHeader)
                            {
                                context.Response.AddHeader("Content-Disposition", "attachment; filename=" + "File_not_found.txt");
                            }
                            context.Response.ContentType = "text/plain";
                            context.Response.ClearContent();
                            context.Response.Write(new Connector.QueryResult()
                            {
                                Success = false,
                                Message = "File not found."
                            }.ToJson());
                        }
                    }
                }
                else if (fileContent != null)
                {
                    if (isAddHeader)
                    {
                        if (context.Request.UserAgent.IndexOf("Mozilla") != -1 && context.Request.UserAgent.IndexOf("Edge") == -1)
                        {
                            context.Response.AddHeader("Content-Disposition", "attachment; filename=" + filename);
                        }
                        else
                        {
                            context.Response.AddHeader("Content-Disposition", "attachment; filename=" + HttpUtility.UrlEncode(filename, System.Text.Encoding.UTF8));
                        }
                        context.Response.AddHeader("Content-Length", fileContent.Length.ToString());
                    }
                    context.Response.ContentType = string.IsNullOrEmpty(fileContentType) ? System.Web.MimeMapping.GetMimeMapping(filename) : fileContentType;
                    context.Response.ClearContent();
                    context.Response.BinaryWrite(fileContent);
                }
            }
            else
            {
                context.Response.ContentType = "text/html";
                context.Response.ClearContent();
                context.Response.Write("<div></div>");
            }
        }
        catch (Exception ex)
        {
            context.Response.AddHeader("Content-Disposition", "attachment; filename=" + "File_error.txt");
            context.Response.ContentType = "text/plain";
            context.Response.ClearContent();
            context.Response.Write(new Connector.QueryResult(ex).ToJson());
        }

    }

    public bool IsReusable
    {
        get
        {
            return false;
        }
    }

}