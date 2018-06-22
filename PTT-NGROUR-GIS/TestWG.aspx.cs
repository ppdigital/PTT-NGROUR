using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

public partial class TestWG : AMSBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack && !IsCallback)
        {
            InitSession();
            InitialPage();
            SetSession();
        }
    }
    protected void InitialPage()
    {
        string page = string.Empty;
        page = !string.IsNullOrEmpty(Request.QueryString["page"]) ? Request.QueryString["page"] : string.Empty;
        if (!string.IsNullOrEmpty(page))
        {
            if (System.IO.File.Exists(Server.MapPath(string.Format("~/test/{0}", page))))
            {
                string textHtml = System.IO.File.ReadAllText(Server.MapPath(string.Format("~/test/{0}", page)));
                Title = page;
                divTesterContainer.InnerHtml = textHtml;
            }
        }
        else
        {
            foreach (System.IO.FileInfo file in new System.IO.DirectoryInfo(Server.MapPath("~/test")).GetFiles("*.html", System.IO.SearchOption.AllDirectories))
            {
                divTesterContainer.InnerHtml += string.Format("<a href='{0}.aspx?page={1}' target='_blank'>{1}</a>", this.GetType().BaseType.Name, file.Name) + "<br/>";
            }
        }
    }
    protected void InitSession()
    {
        foreach (string name in sessionName)
        {
            string sessionKey = name.Split('|')[0];
            string sessionValue = name.Split('|')[1];
            controlSession.Controls.Add(new Label()
            {
                CssClass = "ui-session-label",
                Text = sessionKey
            });
            controlSession.Controls.Add(new TextBox()
            {
                CssClass = "ui-session-textbox",
                ID = sessionKey,
                Text = Session[name] != null ? Session[name].ToString() : sessionValue
            });
        }
    }
    protected void SetSession()
    {
        foreach (Control c in controlSession.Controls)
        {
            if (c is TextBox)
            {
                string sessionValue = (c as TextBox).Text;
                Session[c.ID] = string.IsNullOrEmpty(sessionValue) ? null : sessionValue;
            }
        }
    }
    protected void btnGetSession_Click(object sender, EventArgs e)
    {
        SetSession();
    }
    protected void btnClearSession_Click(object sender, EventArgs e)
    {
        Session.Clear();
    }
}
