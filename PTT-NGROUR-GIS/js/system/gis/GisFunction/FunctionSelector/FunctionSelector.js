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
    "system/gis/GisFunction/zoom"
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
    zoom
    ) {
    return declare([
        _WidgetBase
    ], {

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);


        },

        select: function (misData) {
            console.log("select", misData);
            switch (misData.command) {
                case "view":
                    var z = new zoom();
                    z.start(misData);
                    //console.log(this.reqSP(dashboardGateSP, { data: misData.data }));
                    break;
                case "edit":
                    //custommer management
                    break;
            }
        }
    });
});