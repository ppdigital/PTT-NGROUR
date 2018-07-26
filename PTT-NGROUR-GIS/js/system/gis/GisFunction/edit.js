define([
    "core/_WidgetBase",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/query",
    "dojo/dom",
    "dojo/dom-class",
    "dojo/dom-construct",

    "esri/geometry/Point",
    "esri/geometry/Polyline",
    "esri/symbols/PictureMarkerSymbol",
    "esri/symbols/SimpleLineSymbol",
    "esri/graphic",
    "esri/Color",

    "system/gis/GisFunction/EditStatusInfoTemplate/EditStatusInfoTemplate",
    "system/gis/GisFunction/CustomerManagement/CustomerManagement"
], function (
    _WidgetBase,

    declare,
    lang,
    array,
    on,
    query,
    dom,
    domClass,
    domConstruct,

    Point,
    Polyline,
    PictureMarkerSymbol,
    SimpleLineSymbol,
    graphic,
    Color,

    EditStatusInfoTemplate,
    CustomerManagement
    ) {
    return declare([
        _WidgetBase
    ], {
        point: null,
        header: null,
        customerManagement: null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);

            this.customerManagement = new CustomerManagement();
            customerManageBarNode = dom.byId('customerManageBarNode');
            domConstruct.empty(customerManageBarNode);
            domConstruct.place(this.customerManagement.domNode, customerManageBarNode);
        },
        destroy: function () {

        },
        start: function (misData) {
            var data = misData.parameter,
                sp,
                updateSP,
                params = {},
                Shape;

            sp = 'LUT_Q_CUSTOMERS';
            params.MODE = misData.type;
            if (misData.type == 'METERING') {
                Shape = Point;
                //sp = 'CUS_Q_METER';
                this.header = 'Metering';
                //params.MODE = "METERING";
            } else if (misData.type == 'CUSTOMER') {
                Shape = Point;
                //sp = 'LUT_Q_CUSTOMERS';
                this.header = 'CUSTOMER';
            }
            // map data to params
            Object.keys(data).forEach(lang.hitch(this, function (object) {
                params[object] = data[object];
            }));

            this.customerManagement.start(sp, params);

            //console.log(params);




        },
        save: function () {
            this.customerManagement.save();
        },
        hidden: function () {
            this.customerManagement.hidden();
        }

    });
});