<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Log.aspx.cs" Inherits="Log" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Log</title>

    <style type="text/css">
        @import 'css/ui-default.css';

        html, body {
            height: 100%;
            width: 100%;
            margin: 0;
            overflow: hidden;
        }

        #borderContainerThree {
            width: 100%;
            height: 100%;
            overflow: hidden;
            border: none;
        }

        .dijitBorderContainer .dijitContentPane {
            left: 0 !important;
            right: 0 !important;
        }

            .dijitBorderContainer .dijitContentPane.ui-log-menu {
                top: 0 !important;
            }


        .claro .dijitSplitContainer-child, .claro .dijitBorderContainer-child {
            border: 0;
        }

        .ui-log-query .amos-textbox-label {
            width: auto;
        }

        .ui-log-login .amos-textbox-label {
            width: 7em;
        }

        .amos-dialog-content .ui-log-form .amos-textbox-label {
            width: 11em;
        }

        .ui-viewer-form .amos-textbox-label {
        }

        .amos-textbox {
            padding-bottom: 5px;
            padding-top: 5px;
        }

        .amos-content-pane.ui-log-sub-path {
            padding: 0;
        }

        .amos-textbox-info-message {
            display: none;
        }

        .div-header {
            text-align: center;
        }

        .ui-log-table {
            /*width: 100%;
            position: absolute;
            bottom: 0px;
            left: 0px;
            right: 0px;
            overflow: auto;
            top: 80px;*/
        }

        .ui-log-query > * {
            vertical-align: middle;
        }

        .amos-textbox-container {
            text-align: left;
        }

        /*@media only screen and (min-width:0px) and (max-width: 667px) {

            .ui-log-table {
                top: 160px;
            }
        }


        @media only screen and (min-width:0px) and (max-width: 654px) {

            .ui-log-table {
                top: 170px;
            }
        }

        @media only screen and (min-width:0px) and (max-width: 580px) {

            .ui-log-table {
                top: 210px;
            }
        }

        @media only screen and (min-width:0px) and (max-width: 360px) {

            .ui-log-table {
                top: 285px;
            }
        }*/
    </style>


    <script type="text/javascript">

        var test11 = 33;
        var tblresult;
        var login;
        require([


            "esrith/amos/form/FormValidator",
            "esrith/amos/form/DateTextBox",
            "esrith/amos/form/NumberTextBox",
         //   "esrith/amos/form/PasswordTextBox",
            "esrith/amos/form/TextBox",
            "esrith/amos/form/Button",
            "esrith/amos/form/FilteringSelect",
            "esrith/amos/form/Select",
            "esrith/amos/form/CheckBox",
            "esrith/amos/form/RadioButton",
            "esrith/amos/form/Dialog",
            "esrith/amos/form/Table",

            "esrith/amos/container/ContentPane",


            "esrith/amos/core/DataStore",
            "esrith/amos/core/ColumnDef",
            "esrith/amos/core/Date",



            "dojo/keys",
            "dojox/fx/scroll",
            "dojox/fx",
            "dojo/_base/array",
            "dijit/registry",

            "dojo/request",
            "dojo/_base/lang",

            "dojo/dom-geometry",
            "dojo/dom-style",
            "dojo/dom-class",
            "dojo/dom-construct",
            "dojo/dom",
            "dojo/on",
            "dijit/layout/TabContainer",
            "dijit/layout/BorderContainer",
            "dijit/layout/ContentPane",
            "dojo/ready"

        ], function (
            FormValidator,
            DateTextBox,
            NumberTextBox,
          //  PasswordTextBox,
            TextBox,
            Button,
            FilteringSelect,
            Select,
            CheckBox,
            RadioButton,
            Dialog,
            Table,

            ContentPane,

            DataStore,
            ColumnDef,
            amosDate,


            keys,
            fxScroll,
            fx,
            array,
            registry,

            request,
            lang,

            domGeom,
            domStyle,
            domClass,
            domConStruct,
            dom,
            on,
            TabContainer,
            BorderContainer,
            ContentPane,
            ready

            ) {
            ready(function () {


                console.log("login", login);
                if (login == true) {
                    hideLogin();
                    loadSetting();
                }

                var logFilterData = [
                   { CODE: "ERROR", DESCR: "ERROR" },
                    { CODE: "WARN", DESCR: "WARN" },
                    { CODE: "INFO", DESCR: "INFO" },
                    { CODE: "DEBUG", DESCR: "DEBUG" }
                ];

                var ageData = [
                  { CODE: "today", DESCR: "Today" },
                  { CODE: "yesterday", DESCR: "Yesterday" },
                  { CODE: "lastthreedays", DESCR: "Last Three Days" },
                  { CODE: "lastweek", DESCR: "Last 7 Days" },
                  { CODE: "thismonth", DESCR: "This Month" },
                  { CODE: "lastmonth", DESCR: "Last Month" },
                  { CODE: "all", DESCR: "All" },
                  { CODE: "day", DESCR: "Select Date" },
                  { CODE: "month", DESCR: "Select Month" }
                ];

                var sourceData = [
                  { CODE: "all", DESCR: "All" },
                  { CODE: "store", DESCR: "Store" },
                  { CODE: "request", DESCR: "Request" },
                  { CODE: "detail", DESCR: "Detail" }
                ];

                var machinesData = [
                  { CODE: "all", DESCR: "All" },
                  { CODE: "store", DESCR: "Store" }
                ];

                var timeData = [
                    { CODE: "00", DESCR: "00:00", DESCR2: "00:59" },
                    { CODE: "01", DESCR: "01:00", DESCR2: "01:59" },
                    { CODE: "02", DESCR: "02:00", DESCR2: "02:59" },
                    { CODE: "03", DESCR: "03:00", DESCR2: "03:59" },
                    { CODE: "04", DESCR: "04:00", DESCR2: "04:59" },
                    { CODE: "05", DESCR: "05:00", DESCR2: "05:59" },
                    { CODE: "06", DESCR: "06:00", DESCR2: "06:59" },
                    { CODE: "07", DESCR: "07:00", DESCR2: "07:59" },
                    { CODE: "08", DESCR: "08:00", DESCR2: "08:59" },
                    { CODE: "09", DESCR: "09:00", DESCR2: "09:59" },
                    { CODE: "10", DESCR: "10:00", DESCR2: "10:59" },
                    { CODE: "11", DESCR: "11:00", DESCR2: "11:59" },
                    { CODE: "12", DESCR: "12:00", DESCR2: "12:59" },
                    { CODE: "13", DESCR: "13:00", DESCR2: "13:59" },
                    { CODE: "14", DESCR: "14:00", DESCR2: "14:59" },
                    { CODE: "15", DESCR: "15:00", DESCR2: "15:59" },
                    { CODE: "16", DESCR: "16:00", DESCR2: "16:59" },
                    { CODE: "17", DESCR: "17:00", DESCR2: "17:59" },
                    { CODE: "18", DESCR: "18:00", DESCR2: "18:59" },
                    { CODE: "19", DESCR: "19:00", DESCR2: "19:59" },
                    { CODE: "20", DESCR: "20:00", DESCR2: "20:59" },
                    { CODE: "21", DESCR: "21:00", DESCR2: "21:59" },
                    { CODE: "22", DESCR: "22:00", DESCR2: "22:59" },
                    { CODE: "23", DESCR: "23:00", DESCR2: "23:59" }
                ];

                var pageSizeData = [
                    { CODE: 10, DESCR: "10" },
                    { CODE: 20, DESCR: "20" },
                    { CODE: 30, DESCR: "30" },
                    { CODE: 50, DESCR: "50" },
                    { CODE: 100, DESCR: "100" },
                    { CODE: 200, DESCR: "200" }
                ];

                var columnDefs = [
                    {
                        header: 'Level',
                        width: '100px',
                        columnType: ColumnDef.TYPE_TEXT,
                        fieldName: "LEVEL",
                        align: "center"
                    },
                    {
                        header: 'Date',
                        fieldName: 'DATE',
                        width: '150px',
                        columnType: ColumnDef.TYPE_TEXT,
                        align: "center"
                    }, {
                        header: 'Message',
                        //   align: "right",
                        fieldName: 'MESSAGE_DOM',
                        columnType: ColumnDef.TYPE_DOMNODE
                    },
                    {
                        header: 'Function',
                        //   align: "right",
                        width: '110px',
                        fieldName: 'FUNCTION_DOM',
                        columnType: ColumnDef.TYPE_DOMNODE
                    },
                    {
                        header: 'Session',
                        //   align: "right",
                        width: '110px',
                        fieldName: 'SESSION_DOM',
                        columnType: ColumnDef.TYPE_DOMNODE
                    },
                    {
                        header: 'Source',
                        align: "right",
                        width: '100px',
                        fieldName: 'SOURCE',
                        columnType: ColumnDef.TYPE_TEXT
                    }

                ];


                //   setLogFilter(logFilterData);
                setAge(ageData);
                setSource(sourceData);
                setTime(timeData);
                setPageSize(pageSizeData);

                on(registry.byId("btnQuery"), "click", lang.hitch(this, _btnQuery_click, "22"));
                on(registry.byId("selAge"), "change", lang.hitch(this, _selectAge_change));
                on(registry.byId("startTime"), "change", lang.hitch(this, _startTime_change));
                on(registry.byId("endTime"), "change", lang.hitch(this, _endTime_change));

                on(registry.byId("btnExport"), "click", lang.hitch(this, _btnExport_click));
                on(registry.byId("btnDeleteDialog"), "click", lang.hitch(this, _btnDeleteDialog_click));
                on(registry.byId("btnSetting"), "click", lang.hitch(this, _btnSetting_click));

                on(registry.byId("btnSaveSetting"), "click", lang.hitch(this, _btnSaveSetting_click));
                on(registry.byId("btnClearSetting"), "click", lang.hitch(this, _btnClearSetting_click));

                on(registry.byId("btnAdd"), "click", lang.hitch(this, _btnAdd_click));

                on(registry.byId("btnDelLog"), "click", lang.hitch(this, _btnDelLog_click));
                on(registry.byId("rdoDelNum"), "change", lang.hitch(this, _rdoDelNum_change));


                on(registry.byId("tblResult"), "page_Changed", lang.hitch(this, _tblResult_page_Chaged));
                registry.byId("tblResult").set("columns", columnDefs);

                on(registry.byId("selPageSize"), "change", lang.hitch(this, _selPageSize_Change));

                on(registry.byId("btnClearLogin"), "click", lang.hitch(this, _btnClearLogin_click));
                on(registry.byId("btnLogin"), "click", lang.hitch(this, _btnLogin_click));
                on(registry.byId("txtUser"), "keypress", lang.hitch(this, _txtUser_KeyPress));
                on(registry.byId("txtPassword"), "keypress", lang.hitch(this, _txtPassword_KeyPress));

                //    console.log("amosDate", amosDate(), amosDate);
                var date = amosDate();

                registry.byId("dateAge").set("value", date.toString("dd/MM/yyyy"));
                registry.byId("monthAge").set("value", date.toString("MM/yyyy"));

                //on(window, "resize", function (event) {
                //    //console.log('resized dom', dom.byId("formMain"));
                //    //console.log('resized registry', registry.byId("formMain"));

                //    domStyle.set(dom.byId("tblResult"), "top",( domStyle.get(dom.byId("formMain"), "height") + 50 ) + "px")

                //    console.log("resize resized", domStyle.get(dom.byId("formMain"), "width"), domStyle.get(dom.byId("formMain"), "height"));

                //});


                //on(dom.byId("formMain"), "resize", function (event) {
                //    console.log("formMain resize", event);
                //});
            });


            function loadSetting() {
                console.log("loadSetting");
                var arrLogpath = strLogpath.split("|");

                registry.byId("ntxtKeepLog").set("value", keeplog);

                console.log(" before setfltMachine");

                array.forEach(arrLogpath, lang.hitch(this, function (item, i) {
                    if (i == 0) {
                        registry.byId("txtLogPath").set("value", item);
                    } else {

                        var divMain = domConStruct.create("div", { style: { display: "block" } });

                        var textBox = new TextBox({
                            name: 'LOGPATH',
                            label: ' ',
                            style: 'display:inline-block;padding-right:4px;width:350px;',
                            value: item
                        });

                        if (item.substr(0, 1) == "\\" && item.substr(1, 1) != "\\") {
                            textBox.set("value", "\\" + item);
                        }


                        domConStruct.place(textBox.domNode, divMain, "last");

                        var btnDelText = new Button({
                            label: 'Delete '
                        });

                        domConStruct.place(btnDelText.domNode, divMain, "last");
                        on(btnDelText, "click", lang.hitch(this, _btnDelText_click, textBox, divMain));

                        //   domConStruct.place(divMain, registry.byId("divBtnSetting").domNode, "before"); 
                        domConStruct.place(divMain, registry.byId("divSubLogPath").domNode, "last");

                    }
                }));

                setfltMachine();
                loadComplete();

            }

            function loadComplete() {
                _btnQuery_click();
            }

            function _rdoDelNum_change(value) {
                // console.log("_rdoDelNum_change", value);
            }

            function _btnQuery_click(t) {


                var param = registry.byId("formMain").get("value");
                param.REQUESTTYPE = "query";
                param.MACHINE = registry.byId("fltMachine").get("item").PATH;
                param.STARTTIME = registry.byId("startTime").get("item").CODE;
                param.ENDTIME = registry.byId("endTime").get("item").CODE;

                request.post("Log.aspx", {
                    handleAs: "json",
                    data: param
                }).then(lang.hitch(this, _request_complete),
                lang.hitch(this, function (error) {
                    console.log("error", error);

                }));

            }

            function _request_complete(response) {
                //  console.log("_request_complete", response);

                if (response.success == true) {
                    _manageFormat(response.data, response.search);

                } else {
                    alert("เกิดข้อผิดพลาด : " + response.message);
                }

            }

            function _manageFormat(obj, txtSearch) {

                var arrResult = new Array();
                var objResult = new Object();
                var i = 1;

                var divMessage = null;
                var divLogFunc = null;
                var divLogSess = null;
                var str = "";
                var dataStr = "";
                var extractFunc = [];
                var extractSess = [];

                for (var index in obj) {
                    itemDir = lang.clone(obj[index]);
                    for (var index in itemDir) {

                        itemFile = lang.clone(itemDir[index]);
                        array.forEach(itemFile, lang.hitch(this, function (index, itemTxt) {
                            divMessage = domConStruct.create("div")
                            divLogFunc = domConStruct.create("div")
                            divLogSess = domConStruct.create("div")

                            str = lang.clone(itemTxt.split(" - "));
                            dataStr = lang.trim(str[1]);
                            objResult = new Object();

                            extractFunc = _extractTaggedInfo(dataStr, "LOGFUNCTION");
                            dataStr = extractFunc[0];
                            divLogFunc.innerHTML = extractFunc[1];
                            extractSess = _extractTaggedInfo(dataStr, "LOGSESSION");
                            dataStr = extractSess[0];
                            divLogSess.innerHTML = extractSess[1];


                            divMessage.innerHTML = dataStr;
                            objResult.LEVEL = str[0].split(" ")[0];
                            objResult.DATE = str[0].split(" ")[str[0].split(" ").length - 2] + " " + str[0].split(" ")[str[0].split(" ").length - 1].split(",")[0];
                            objResult.MESSAGE = lang.trim(str[1]);
                            objResult.MESSAGE_DOM = divMessage;
                            objResult.FUNCTION_DOM = divLogFunc;
                            objResult.SESSION_DOM = divLogSess;
                            objResult.ROW = lang.clone(i);
                            objResult.SOURCE = lang.clone(index.split('.')[0].split('-')[0]);

                            arrResult.push(objResult);
                            i++;
                        }, index));
                    }
                }
                //    console.log("arrResult", arrResult);

                var ds = new DataStore({ data: arrResult });


                // createTable(ds, txtSearch);
                bindTable(ds, txtSearch);

            }

            function bindTable(dataStore, txtSearch) {

                var tblresult = registry.byId("tblResult");
                tblresult.bind(dataStore);

                if (txtSearch != "") {
                    var exp = {};
                    exp.fieldName = "MESSAGE";
                    exp.text = txtSearch;
                    exp.filterType = Table.FILTER_CONTAIN;
                    tblresult.filter(exp);
                }
            }


            function createTable(dataStore, txtSearch) {
                var columnDefs = [

                    {
                        header: 'Level',
                        width: '100px',
                        columnType: ColumnDef.TYPE_TEXT,
                        fieldName: "LEVEL",
                        align: "center",
                        sortable: true

                    },
                    {
                        header: 'Date',
                        fieldName: 'DATE',
                        width: '150px',
                        columnType: ColumnDef.TYPE_TEXT,
                        align: "center",
                        sortable: true
                    }, {
                        header: 'Message',
                        //   align: "right",
                        fieldName: 'MESSAGE',
                        columnType: ColumnDef.TYPE_DOMNODE
                    },
                    {
                        header: 'Source',
                        align: "right",
                        width: '100px',
                        fieldName: 'SOURCE',
                        columnType: ColumnDef.TYPE_TEXT,
                        sortable: true
                    }

                ];

                tblresult = new Table({
                    enabledTotal: false,
                    alwaysShowPaging: true,
                    primaryKeyField: "ROW",
                    alternateRowColors: false
                    //    , pageSize: 10
                });

                tblresult.set("columns", columnDefs);
                //tblresult.set("rowSelect", true);

                tblresult.bind(dataStore);

                domConStruct.place(tblresult.domNode, dom.byId("divTableResult"), "only");

                //right: 0px; left: 0px; overflow: scroll; bottom: 0px; top: 120px;

            }


            function Func(tt) {
                alert(tt);
            }

            function setLogFilter(data) {
                var ds = new DataStore({ data: data });
                registry.byId("selFilter").set("valueField", "CODE");
                registry.byId("selFilter").set("displayField", "DESCR");
                registry.byId("selFilter").bind(ds);

                registry.byId("selFilter").set("value", "ERROR");

            }

            function setAge(data) {
                var ds = new DataStore({ data: data });
                registry.byId("selAge").set("valueField", "CODE");
                registry.byId("selAge").set("displayField", "DESCR");
                registry.byId("selAge").bind(ds);

                registry.byId("selAge").set("value", "today");
            }

            function setSource(data) {
                var ds = new DataStore({ data: data });
                registry.byId("selSource").set("valueField", "CODE");
                registry.byId("selSource").set("displayField", "DESCR");
                registry.byId("selSource").bind(ds);

                registry.byId("selSource").set("value", "all");
            }

            function setTime(data) {
                var hr = new Date().getHours();
                //console.log("hour number", hr);
                if (hr < 10) {
                    hr = "0" + hr;
                }

                var dsStart = new DataStore({ data: data });
                registry.byId("startTime").set("valueField", "CODE");
                registry.byId("startTime").set("displayField", "DESCR");
                registry.byId("startTime").bind(dsStart);

                //registry.byId("startTime").set("value", hr);
                registry.byId("startTime").set("value", "00");

                var dsEnd = new DataStore({ data: data });
                registry.byId("endTime").set("valueField", "CODE");
                registry.byId("endTime").set("displayField", "DESCR2");
                registry.byId("endTime").bind(dsEnd);

                //registry.byId("endTime").set("value", hr);
                registry.byId("endTime").set("value", "23");
            }

            function setfltMachine(data) {

                registry.byId("fltMachine").unbind();

                var arrLogpath = strLogpath.split("|");

                var arrData = [];

                array.forEach(arrLogpath, lang.hitch(this, function (item, i) {
                    var obj = {};

                    obj.CODE = i + 1;
                    obj.PATH = item;
                    obj.DESCR = item;

                    if (item.substr(0, 1) == "\\" && item.substr(1, 1) != "\\") {
                        obj.DESCR = "\\" + item;
                        obj.PATH = "\\" + item;
                    }


                    var str = item.split("/")[0];
                    //   obj.DESCR = str;
                    console.log("fltMachine str", str, item);




                    arrData.push(obj);
                }));

                var ds = new DataStore({ data: arrData });
                registry.byId("fltMachine").set("valueField", "CODE");
                registry.byId("fltMachine").set("displayField", "DESCR");
                registry.byId("fltMachine").bind(ds);

                registry.byId("fltMachine").set("value", "1");
            }

            function setPageSize(data) {
                var ds = new DataStore({ data: data });
                registry.byId("selPageSize").set("valueField", "CODE");
                registry.byId("selPageSize").set("displayField", "DESCR");
                registry.byId("selPageSize").bind(ds);

                registry.byId("selPageSize").set("value", 50);
            }

            function _selectAge_change(value) {

                if (value == "day") {
                    domClass.remove(registry.byId("dateAge").domNode, "amos-hide");
                    domClass.add(registry.byId("monthAge").domNode, "amos-hide");

                }
                else if (value == "month") {
                    domClass.remove(registry.byId("monthAge").domNode, "amos-hide");
                    domClass.add(registry.byId("dateAge").domNode, "amos-hide");
                }
                else {
                    domClass.add(registry.byId("dateAge").domNode, "amos-hide");
                    domClass.add(registry.byId("monthAge").domNode, "amos-hide");
                }
            }

            function _startTime_change(value) {
                var start = parseInt(value);
                var end = parseInt(registry.byId("endTime").value);
                console.log("start and end hr", start, end);

                if (start > end) {
                    if (end < 10) {
                        end = "0" + end;
                    }
                    registry.byId("startTime").set("value", end);
                }
            }

            function _endTime_change(value) {
                var start = parseInt(registry.byId("startTime").value);
                var end = parseInt(value);
                console.log("start and end hr", start, end);

                if (start > end) {
                    if (start < 10) {
                        start = "0" + start;
                    }
                    registry.byId("endTime").set("value", start);
                }
            }

            function _selPageSize_Change(value) {
                registry.byId("tblResult").set("pageSize", value);
            }

            function _btnExport_click() {
                var tblresult = registry.byId("tblResult");
                console.log("tblresult", tblresult);

                JSONToCSVConvertor(tblresult.dataStore.data, "", true);
            }

            function _btnDeleteDialog_click() {

                registry.byId("diaDeleteLog").show();
            }

            function _btnSetting_click() {
                clearSetting();
                registry.byId("diaSetting").show();
            }


            function _btnDelLog_click() {

                var param = registry.byId("formDelete").get("value");

                param.REQUESTTYPE = "delete";

                request.post("Log.aspx", {
                    handleAs: "json",
                    data: param
                }).then(lang.hitch(this, _btnDelLog_complete),
                lang.hitch(this, function (error) {
                    console.log("error", error);
                })
                );

            }

            function _btnDelLog_complete(response) {

                console.log("_btnDelLog_complete");
            }


            function _btnSaveSetting_click() {
                var param = registry.byId("formSetting").get("value");

                param.REQUESTTYPE = "setting";

                request.post("Log.aspx", {
                    handleAs: "json",
                    data: param
                }).then(lang.hitch(this, _btnSaveSetting_complete),
                lang.hitch(this, function (error) {
                    console.log("error", error);
                })
                );

            }

            function _btnClearSetting_click() {

                clearSetting();

            }

            function clearSetting() {
                domConStruct.empty(registry.byId("divSubLogPath").domNode);

                loadSetting();
            }

            function _btnAdd_click() {
                var divMain = domConStruct.create("div", { style: { display: "block" } });
                var textBox = new TextBox({
                    name: 'LOGPATH',
                    label: ' ',
                    style: 'display:inline-block;padding-right:4px;width:350px;'
                    //    , value:'\\'
                });
                domConStruct.place(textBox.domNode, divMain, "last");

                var btnDelText = new Button({
                    label: 'Delete '
                });

                domConStruct.place(btnDelText.domNode, divMain, "last");
                on(btnDelText, "click", lang.hitch(this, _btnDelText_click, textBox, divMain));
                //domConStruct.place(divMain, registry.byId("divBtnSetting").domNode, "before");
                domConStruct.place(divMain, registry.byId("divSubLogPath").domNode, "last");
            }

            function _btnDelText_click(textBox, divMain) {
                textBox.destroy()
                domConStruct.destroy(divMain);

            }

            function _btnSaveSetting_complete(response) {

                if (response.success == true) {
                    // response.data 
                    console.log("_btnSaveSetting_complete", response.data);
                    keeplog = response.data.keeplog;
                    strLogpath = response.data.strLogpath;

                    setfltMachine();
                }

            }

            function _btnLogin_click() {
                if ((registry.byId("txtUser").value == "admin" || registry.byId("txtUser").value == "Admin") && registry.byId("txtPassword").value == "P@ssw0rd") {


                    param = {
                        REQUESTTYPE: "login"
                    };

                    request.post("Log.aspx", {
                        handleAs: "json",
                        data: param
                    }).then(lang.hitch(this, login_complete),
                    lang.hitch(this, function (error) {
                        console.log("error", error);
                    })
                    );

                } else {
                    alert("username หรือ password ไม่ถูกต้อง");
                }

            }

            function login_complete() {

                //    console.log(keeplog, "keeplog");

                hideLogin();
                //   loadSetting();

                param = {
                    REQUESTTYPE: "readXml"
                };

                request.post("Log.aspx", {
                    handleAs: "json",
                    data: param
                }).then(lang.hitch(this, readXml),
                 lang.hitch(this, function (error) {
                     console.log("error", error);
                 })
                 );
            }

            function readXml(response) {
                console.log("readXml", response);
                //  data
                if (response.success == true) {
                    keeplog = response.data.keeplog;
                    strLogpath = response.data.strLogpath;

                    loadSetting();
                }


            }

            function hideLogin() {



                domStyle.set(dom.byId("divMain"), "visibility", "visible");
                domStyle.set(dom.byId("divDetail"), "visibility", "visible");


                //console.log("divMain", dom.byId("divMain"));

                domStyle.set(dom.byId("divLogin"), "display", "none");



                //  console.log("hideLogin", dom.byId("divLogin"));
            }

            function _btnClearLogin_click() {
                registry.byId("txtUser").reset();
                registry.byId("txtPassword").reset();
            }

            function _txtUser_KeyPress(key) {
                if (key.keyCode == keys.ENTER) {
                    _btnLogin_click();
                }
            }

            function _txtPassword_KeyPress(key) {
                if (key.keyCode == keys.ENTER) {
                    _btnLogin_click();
                }
            }

            function _tblResult_page_Chaged() {

                //  console.log("_tblResult_page_Chaged", dom.byId("divFocus"), dom.byId("divTable"), dom.byId("divMain"), fx);

                fx.smoothScroll({
                    //  node: dom.byId("divFocus"), //-> Node ที่จะแสดง
                    // win: dom.byId("divMain"),
                    duration: 400,
                }).play();


            }

            function JSONToCSVConvertor(JSONData, ReportTitle, ShowLabel) {
                //If JSONData is not an object then JSON.parse will parse the JSON string in an Object
                var arrData = typeof JSONData != 'object' ? JSON.parse(JSONData) : JSONData;

                var CSV = '';
                //Set Report title in first row or line

                // CSV += ReportTitle + '\r\n\n';

                //This condition will generate the Label/Header
                if (ShowLabel) {
                    var row = "";

                    //This loop will extract the label from 1st index of on array
                    for (var index in arrData[0]) {

                        //Now convert each value to string and comma-seprated
                        row += index + ',';
                    }

                    row = row.slice(0, -1);

                    //append Label row with line break
                    CSV += row + '\r\n';
                }

                //1st loop is to extract each row
                for (var i = 0; i < arrData.length; i++) {
                    var row = "";

                    //2nd loop will extract each column and convert it in string comma-seprated
                    for (var index in arrData[i]) {
                        row += '"' + arrData[i][index] + '",';
                    }

                    row.slice(0, row.length - 1);

                    //add a line break after each row
                    CSV += row + '\r\n';
                }

                if (CSV == '') {
                    alert("Invalid data");
                    return;
                }

                //Generate a file name
                var fileName = "fileLogs";
                //this will remove the blank-spaces from the title and replace it with an underscore
                fileName += ReportTitle.replace(/ /g, "_");

                //Initialize file format you want csv or xls
                var uri = 'data:text/csv;charset=utf-8,' + escape(CSV);

                // Now the little tricky part.
                // you can use either>> window.open(uri);
                // but this will not work in some browsers
                // or you will not get the correct file extension    

                //this trick will generate a temp <a /> tag
                var link = document.createElement("a");
                link.href = uri;

                //set the visibility hidden so it will not effect on your web-layout
                link.style = "visibility:hidden";
                link.download = fileName + ".csv";

                //this part will append the anchor tag and remove it after automatic click
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
            }

            function _extractTaggedInfo(str, tag) {
                var arrStr = [];
                var strS1 = "";
                var strS2 = "";
                var taggedData = "";
                var tagLen = tag.length;

                var startTagIndex = str.indexOf("<" + tag + "><br/>");
                var endTagIndex = str.indexOf("</" + tag + "><br/>");

                if (startTagIndex != -1 && endTagIndex != -1) {
                    strS1 = str.substring(0, startTagIndex);
                    taggedData = str.substring((startTagIndex + tagLen + 7), endTagIndex);
                    strS2 = str.substring((endTagIndex + tagLen + 8));

                    arrStr = [(strS1 + strS2), taggedData];
                }
                else {
                    arrStr = [str, ""];
                }
                //console.log(arrStr[0]);
                //console.log(arrStr[1]);
                return arrStr;
            }

        });
    </script>
</head>
<body class="claro">


    <div data-dojo-type="dijit/layout/BorderContainer" data-dojo-props="design:'sidebar', gutters:true, liveSplitters:true" id="borderContainerThree">

        <div data-dojo-type="dijit/layout/ContentPane" data-dojo-props="region:'top', splitter:false " style="top: 0 !important; right: 0 !important;" class="ui-log-menu">
            <%--     <div style="display: none;" id="divMain">
               </div>--%>
            <div style="visibility: hidden;" id="divMain">
                <div id="menu" style="text-align: right; padding-bottom: 5px;">
                    <div data-dojo-type="esrith/amos/form/Button" id="btnExport">Export </div>
                    <div data-dojo-type="esrith/amos/form/Button" id="btnDeleteDialog" style="display: none;">Delete Log </div>
                    <div data-dojo-type="esrith/amos/form/Button" id="btnSetting">Setting </div>
                </div>

                <div id="formMain" data-dojo-type="esrith/amos/form/FormValidator" data-dojo-props="errorMessageVisible:false" class="ui-log-query"
                    style="">


                    <%--<div data-dojo-type="esrith/amos/form/Select" id="selFilter" data-dojo-props=" name:'LOGFILTER', label:'Log Filter:'"></div>--%>
                    <div style="display: inline-block; border: 1px solid; padding: 4px 4px 2px; margin-left: 3px;">
                        <div style="display: inline-block; padding-top: 2px; white-space: nowrap;">Level :&nbsp; </div>
                        <div data-dojo-type="esrith/amos/form/CheckBox" id="chkFilter1" data-dojo-props=" name:'FILTER',value:'ERROR',label:'ERROR', checked:true"></div>
                        <div data-dojo-type="esrith/amos/form/CheckBox" id="chkFilter2" data-dojo-props=" name:'FILTER',value:'WARN',label:'WARN'"></div>
                        <div data-dojo-type="esrith/amos/form/CheckBox" id="chkFilter3" data-dojo-props=" name:'FILTER',value:'INFO',label:'INFO'"></div>
                        <div data-dojo-type="esrith/amos/form/CheckBox" id="chkFilter4" data-dojo-props=" name:'FILTER',value:'DEBUG',label:'DEBUG'"></div>
                    </div>
                    &nbsp;
                       <div data-dojo-type="esrith/amos/form/Select" id="selAge" data-dojo-props=" name:'AGE', label:'Date: &nbsp; '" style="display: inline-block; width: 100px;"></div>

                    <div data-dojo-type="esrith/amos/form/DateTextBox" id="dateAge" data-dojo-props=" name:'DATEAGE'" class="amos-hide" style="display: inline-block; width: 110px;"></div>
                    <div data-dojo-type="esrith/amos/form/DateTextBox" id="monthAge" data-dojo-props=" name:'MONTHAGE', format:'MM/yyyy'" class="amos-hide" style="display: inline-block; width: 110px;"></div>
                    &nbsp; 
                    <div style="display: inline-block;">
                        <div data-dojo-type="esrith/amos/form/FilteringSelect" id="startTime" data-dojo-props=" name:'STARTTIME', label:'Time: &nbsp; '" style="display: inline-block; width: 83px;"></div>
                        &nbsp;
                        <div data-dojo-type="esrith/amos/form/FilteringSelect" id="endTime" data-dojo-props=" name:'ENDTIME', label:' - &nbsp; '" style="display: inline-block; width: 83px;"></div>
                    </div>
                    &nbsp;
                        <div data-dojo-type="esrith/amos/form/Select" id="selSource" data-dojo-props=" name:'SOURCE', label:&nbsp;  'Source:&nbsp;'" style="display: inline-block; width: 90px;"></div>
                    &nbsp; 
                        <div data-dojo-type="esrith/amos/form/FilteringSelect" id="fltMachine" data-dojo-props=" name:'MACHINE', label:'Machine: &nbsp; '" style="display: inline-block; width: 180px;"></div>
                    &nbsp;
                    <div style="display: inline-block;">
                        <div data-dojo-type="esrith/amos/form/TextBox" id="txtSearch" data-dojo-props=" name:'SEARCH', label:'Search:&nbsp;  '" style="display: inline-block; width: 150px;"></div>
                        &nbsp;
                        <div data-dojo-type="esrith/amos/form/Select" id="selPageSize" data-dojo-props=" name:'PAGESIZE', label:&nbsp;  'Page Size:&nbsp;'" style="display: inline-block; width: 55px;"></div>
                        &nbsp;
                        <div data-dojo-type="esrith/amos/form/Button" id="btnQuery">Query </div>
                        
                    </div>
                </div>

            </div>
        </div>

        <div data-dojo-type="dijit/layout/ContentPane" data-dojo-props=" region:'center', splitter:false " style="z-index: 0;">

            <div style="visibility: hidden;" id="divDetail">
                <div id="divTable" style="padding-top: 5px;">
                    <div id="divFocus">
                    </div>

                    <div id="tblResult" data-dojo-type="esrith/amos/form/Table"
                        data-dojo-props="enabledTotal: false,alwaysShowPaging: true,primaryKeyField: 'ROW',  alternateRowColors: true, rowSelect:false, pageSize:50"
                        style=""
                        class="ui-log-table">
                    </div>
                </div>
            </div>

            <div id="diaSetting" data-dojo-type="esrith/amos/form/Dialog" data-dojo-props="resizable:true">

                <div id="formSetting" data-dojo-type="esrith/amos/form/FormValidator" data-dojo-props="errorMessageVisible:false" class="ui-log-form">

                    <div data-dojo-type="esrith/amos/form/NumberTextBox" id="ntxtKeepLog" data-dojo-props=" name:'KEEPLOG', label:'Keep logs for at least:'" style="width: 350px;"></div>

                    <div data-dojo-type="esrith/amos/form/TextBox" id="txtLogPath" data-dojo-props=" name:'LOGPATH', label:'Log file path:'" style="display: inline-block; width: 350px;"></div>

                    <div data-dojo-type="esrith/amos/form/Button" id="btnAdd" style="width: 60px;">Add </div>

                    <div id="divSubLogPath" data-dojo-type="esrith/amos/container/ContentPane" class="ui-log-sub-path">
                    </div>

                    <div id="divBtnSetting">
                        <div data-dojo-type="esrith/amos/form/Button" id="btnSaveSetting">Save </div>
                        <div data-dojo-type="esrith/amos/form/Button" id="btnClearSetting">Clear </div>

                    </div>
                </div>

            </div>

            <div id="diaDeleteLog" data-dojo-type="esrith/amos/form/Dialog" data-dojo-props="resizable:true">
                <div id="formDelete" data-dojo-type="esrith/amos/form/FormValidator" data-dojo-props="errorMessageVisible:false" class="ui-viewer-form">
                    <div>
                        <div id="rdoDelToday" data-dojo-type="esrith/amos/form/RadioButton" data-dojo-props="label:'Today',name:'TYPE',value:'today',checked:true">
                        </div>
                    </div>
                    <div>
                        <div id="rdoDelAll" data-dojo-type="esrith/amos/form/RadioButton" data-dojo-props="label:'All',name:'TYPE',value:'all'">
                        </div>
                    </div>
                    <div>
                        <div id="rdoDelNum" data-dojo-type="esrith/amos/form/RadioButton" data-dojo-props="label:'กำหนดเอง',name:'TYPE',value:'number'">
                        </div>
                        <div data-dojo-type="esrith/amos/form/NumberTextBox" id="ntxtNumber" data-dojo-props=" name:'RETROSPECT', label:'Search:'"></div>
                    </div>
                    <div data-dojo-type="esrith/amos/form/Button" id="btnDelLog">Delete </div>
                </div>
            </div>


            <div id="divLogin">
                <div class="ui-log-login" style="border: 1px solid #a09d9e; text-align: center; width: 400px; left: 50%; position: fixed; top: 40%; margin-left: -180px; margin-top: -30px; border-radius: 5px; padding-bottom: 10px;">
                    <div>
                        <div style="font-size: 25px; background-color: rgb(204, 192, 166); padding: 10px; margin-bottom: 10px;">
                            Log Manage

                        </div>
                        <div data-dojo-type="esrith/amos/form/TextBox" id="txtUser" data-dojo-props=" name:'USER', label:'ชื่อผู้ใช้งาน : '"></div>
                        <div data-dojo-type="esrith/amos/form/TextBox" id="txtPassword" data-dojo-props=" name:'PASSWORD', label:'รหัสผ่าน : ',type:'password'"></div>



                        <div data-dojo-type="esrith/amos/form/Button" id="btnLogin">Login </div>
                        <div data-dojo-type="esrith/amos/form/Button" id="btnClearLogin">Clear </div>


                    </div>
                </div>


            </div>



        </div>



    </div>
    <%-- </div>--%>
</body>
</html>

