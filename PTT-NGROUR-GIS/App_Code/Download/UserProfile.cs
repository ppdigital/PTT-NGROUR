using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for UserProfile
/// </summary>
public class UserProfile
{
    //---choose one---//
    public byte[] FileContent { get; set; }  //call Response.BinaryWrite Method
    public string FullName { get; set; } //call Response.WriteFile Method
    //--------------//

    //---optional---//
    public string FileName { get; set; } //display in download dialog.
    //--------------//

    //---optional---//
    public string FileContentType { get; set; }
    //--------------//

    public UserProfile(Connector.QueryParameter queryParam)
    {
        //queryParam -> content from client
        FullName = System.IO.Path.Combine(
            AMSCore.WebConfigReadKey("PATH_UPLOAD_UM"), //system path from web.config
            queryParam["IMG"].ToString() //filename from client
            );
        FileName = string.Empty;
        FileContentType = null;
        FileContent = null;
    }
}