using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class Page : AMSBasePage
{

    protected void Page_Load(object sender, EventArgs e)
    {
        string request = string.IsNullOrEmpty(Request["p"]) ? "portal" : Request["p"];
        string wgNamespace = string.Empty;
        Dictionary<string, object> wgParameters = null;

        if (!string.IsNullOrEmpty(request))
        {
            bool writeHTMLContent = PageSelector(request, out wgNamespace, out wgParameters);

            if (writeHTMLContent)
            {
                if (!string.IsNullOrEmpty(wgNamespace))
                {
                    PageInitialize(wgNamespace, wgParameters);
                }
                else
                {
                    Response.End();
                    return;
                }
            }
            else
            {
                Response.End();
                return;
            }
        }
        else
        {
            Response.End();
            return;
        }
    }
    private void PageInitialize(string wgNamespace, Dictionary<string, object> wgParameters)
    {
        //InsertStyleTag(string.Format("@import 'css/ui-default.css{0}';", appCacheBust));
        //Add By nattawit.kr 2018/06/15
        InsertStyleTag(string.Format("@import 'css/ui-default.css{0}';@import 'css/ui-override.css{0}';", appCacheBust));
        //End Add
        System.Web.Script.Serialization.JavaScriptSerializer serializer = new System.Web.Script.Serialization.JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
        string wgParamString = serializer.Serialize(wgParameters).Trim();
        LiteralControl ltr = new LiteralControl();
        StringBuilder text = new StringBuilder();
        text.AppendLine(string.Format("<div data-dojo-type='{0}' data-dojo-props='{1}'></div>", wgNamespace, wgParamString.Substring(1).Substring(0, wgParamString.Length - 2)));
        ltr.Text = text.ToString();
        _bodyContent_.Controls.Add(ltr);
    }
    

    
}