<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Report.aspx.cs" Inherits="reports_Report" %>

<%@ Register Assembly="Microsoft.ReportViewer.WebForms, Version=12.0.0.0, Culture=neutral, PublicKeyToken=89845dcd8080cc91" Namespace="Microsoft.Reporting.WebForms" TagPrefix="rsweb" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <asp:ScriptManager ID="ScriptManager1" runat="server">
            </asp:ScriptManager>
            <rsweb:ReportViewer ID="ReportViewer1" runat="server" OnPreRender="ReportViewer1_PreRender" Font-Names="Tahoma" Font-Size="10pt" WaitMessageFont-Names="Verdana" WaitMessageFont-Size="14pt" Width="100%" Height="740px" AsyncRendering="true" SizeToReportContent="true" Style="overflow: hidden;">
            </rsweb:ReportViewer>
        </div>
    </form>
</body>
</html>
