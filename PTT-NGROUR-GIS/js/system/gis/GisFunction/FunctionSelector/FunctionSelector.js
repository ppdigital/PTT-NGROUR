define([
    "core/_WidgetBase",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/query",
    "dojo/dom-class",
    "dojo/dom-construct",

    "esri/InfoTemplate",
    "system/gis/GisFunction/view",
    "system/gis/GisFunction/edit"
], function (
    _WidgetBase,

    declare,
    lang,
    array,
    on,
    query,
    domClass,
    domConstruct,

    InfoTemplate,
    View,
    Edit
    ) {
    return declare([
        _WidgetBase
    ], {
        view: null,
        edit: null,
        previousParams: null,
        constructor: function (params, srcNodeRef) {
            params = params || {};
            this.previousParams = {};
        },
        postCreate: function () {
            this.inherited(arguments);
            this.view = new View();
            this.edit = new Edit();
        },
        clearScreen: function (misData) {
            //console.log("clearScreen");
            if (misData.command == 'view' || this.compareMeterParam(misData) == false) {
                
                this.edit.hidden();
            }
            if (misData.menu == 'utilization-search-byregion') {
                this.edit.hidden();
            }
            this.view.hidden();
        },
        select: function (misData) {
            console.log("select", misData);
            this.clearScreen(misData);
            switch (misData.command) {
                case "view":
                    if (misData.menu == 'utilization-customer') {
                        this.edit.start(misData);
                    } else {
                        this.view.start(misData);
                    }
                    
                    //console.log(this.reqSP(dashboardGateSP, { data: misData.data }));
                    break;
                case "edit":
                    //custommer management
                    //this.edit.start(misData);
                    break;
                case "save":
                    this.edit.save();
                    break;
                default:
                    console.log("default");
                    if (misData.menu == 'utilization-search-byregion') {
                        //this.view.start(misData);
                        this.view.zoomToRegion(misData);
                    }
            }
        },
        compareMeterParam: function (misData) {
            if (misData.METER_NUMBER == this.previousParams.METER_NUMBER &&
                misData.SHIP_TO == this.previousParams.SHIP_TO) {
                return true;
            } else {
                return false;
            }
            this.previousParams = misData.parameter;
        }
    });
});