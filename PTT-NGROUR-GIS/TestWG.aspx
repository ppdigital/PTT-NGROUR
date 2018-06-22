<%@ Page Language="C#" AutoEventWireup="true" CodeFile="TestWG.aspx.cs" Inherits="TestWG" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Widget Tester</title>
    <style type="text/css">
        #divTesterContainer {
            overflow: auto;
        }

        .ui-session-form {
            position: absolute;
            width: 0;
            top: 0;
            bottom: 0;
            right: 0;
            margin: 0;
            padding: 0;
            overflow: hidden;
            white-space: nowrap;
            -moz-transition: width ease-out 0.3s;
            -o-transition: width ease-out 0.3s;
            -webkit-transition: width ease-out 0.3s;
            transition: width ease-out 0.3s;
            background: rgba(250, 250, 250, 0.8);
        }

        .ui-session-label {
            display: block;
        }

        .ui-session-textbox {
            display: block;
        }
    </style>
    <script type="text/javascript">
        var _domStyle = null;
        var _dom = null;
        var _registry = null;
        var _on = null;
        var _lang = null;

        require([


            "dojo/dom-style",
            "dojo/dom",
            "dojo/on",
            "dijit/registry",
            "dojo/_base/lang",
            "dojo/ready"

        ], function (


            domStyle,
            dom,
            on,
            registry,
            lang,
            ready

           ) {

            _domStyle = domStyle;
            _dom = dom;
            _on = on;
            _registry = registry;
            _lang = lang;

            ready(function () {
                var frmSession = _dom.byId("frmSession");
                _on(document.body, "mousemove", _lang.hitch(this, function (evt) {
                    if (document.body.clientWidth - evt.clientX < 5) {
                        _domStyle.set(frmSession, "width", "300px");
                    }
                }));
                _on(frmSession, "mouseout", _lang.hitch(this, function (evt) {
                    _domStyle.set(frmSession, "width", "0");
                }));
                _on(frmSession, "mouseover", _lang.hitch(this, function (evt) {
                    _domStyle.set(frmSession, "width", "300px");
                }));

            });
        });

    </script>
</head>
<body class="claro">
    <div id="divTesterContainer" runat="server" class="ui-body"></div>
    <form id="frmSession" class="ui-session-form" post="post" name="frmSession" runat="server">
        <asp:ScriptManager ID="ScriptManager1" runat="server"></asp:ScriptManager>
        <div id="controlSession" runat="server"></div>
        <asp:UpdatePanel ID="UpdatePanel1" runat="server">
            <ContentTemplate>
                <asp:Button ID="btnGetSession" runat="server" Text="Get Session" OnClick="btnGetSession_Click" />
                <asp:Button ID="btnClearSession" runat="server" Text="Clear Session" OnClick="btnClearSession_Click" />
            </ContentTemplate>
        </asp:UpdatePanel>
    </form>
</body>
</html>
