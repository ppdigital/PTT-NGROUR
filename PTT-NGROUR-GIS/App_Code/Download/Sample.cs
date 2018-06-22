using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Sample
/// </summary>
public class Sample
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

    public Sample(Connector.QueryParameter queryParam)
    {
        //queryParam -> content from client
        FullName = System.IO.Path.Combine(
            AMSCore.WebConfigReadKey("TEMPORARY_PATH"), //system path from web.config
            queryParam["SAMPLE_PARAM"] + ".zip" //filename from client
            );
        FileName = "ทดสอบ_Download_Files.zip";
        FileContentType = null;
        FileContent = null;
    }
}