define([
    "core/_WidgetBase",
    "manager/ApplicationManager",
    "dojo/text!./SystemSelector.html",
    "xstyle/css!./css/style.css",

    "dojo/_base/array",
    "dojo/dom-style",
    "dojo/dom-construct",
    "dojo/on",
    "dojo/aspect",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    _WidgetBase,
    ApplicationManager,
    template,
    xstyle,

    array,
    domStyle,
    domConstruct,
    on,
    aspect,
    lang,
    declare,
    require
    ) {


    var wgConstant = {};


    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
        _WidgetBase
    ], {

        baseClass: "widget-system-selector",
        templateString: template,

        constructor: function (params, srcNodeRef) {
            params = params || {};
            this.applicationManager = ApplicationManager.getInstance();
        },
        postCreate: function () {
            this.inherited(arguments);
            array.forEach(this.systemConfig.data, lang.hitch(this, function (sysDetail, i, total) {
                this._createElement(sysDetail, i, total.length);
            }));
            this._createElement({
                SYS_ID: -1,
                SYS_NAME: "ออกจากระบบ",
                ICON_URL: "login_menu4.png"
            }, this.systemConfig.data.length, this.systemConfig.data.length);
        },
        startup: function () {
            this.inherited(arguments);
        },
        _createElement: function (sysDetail, index, total) {
            var table = domConstruct.create("div", {
                style: {
                    display: "table",
                    width: "100%",
                    cursor: "pointer"
                    //background: this.colorPatterns[(index == total ? this.colorPatterns.length - 1 : index)]
                }
            });
            var row = domConstruct.create("div", {
                style: {
                    display: "table-row"
                }
            }, table);
            //var cell1 = domConstruct.create("div", {
            //    style: {
            //        display: "table-cell",
            //        padding: "0px 15px",
            //        verticalAlign: "middle"
            //    }
            //}, row);
            var cell2 = domConstruct.create("div", {
                style: {
                    display: "table-cell",
                    width: "100%",
                    height: "60px",
                    padding: "0px 10px",
                    verticalAlign: "middle"
                }
            }, row);
            //var cell3 = domConstruct.create("div", {
            //    style: {
            //        display: "table-cell",
            //        padding: "0px 20px",
            //        verticalAlign: "middle"
            //    }
            //}, row);

            //cell1.innerHTML = lang.replace("<img src='images/system/{0}' style='float:left;' />", [(sysDetail["ICON_URL"] ? sysDetail["ICON_URL"] : "x.png")]);
            cell2.innerHTML = sysDetail["SYS_NAME"];
            //cell3.innerHTML = "<img src='images/system/arrow1.png' style='float:left;' />";
            this.own(on(table, "click", lang.hitch(this, "_system_Click", sysDetail)));
            domConstruct.place(table, this.domNode);
        },
        _system_Click: function (sysDetail) {
            if (sysDetail["SYS_ID"] == -1) {
                this.applicationManager.logout();
                this.applicationManager.close();
            } else {
                this.applicationManager.openSystem(sysDetail, this.domNode.parentNode);
            }
            this.destroyRecursive();
        }
    });

    lang.mixin(wgDeclare, wgConstant);

    return wgDeclare;
});