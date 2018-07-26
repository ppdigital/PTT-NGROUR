<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Page.aspx.cs" Inherits="Page" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <script>
        window.preloadMisData = null;
        window.addEventListener('message', function (event) {
            
            window.preloadMisData = event.data;
            console.log("window.preloadMisData", window.preloadMisData);
        })
        console.log("ifrmWindow", window);
    </script>
</head>
<body id="_bodyContent_" runat="server" class="claro">
</body>
</html>
