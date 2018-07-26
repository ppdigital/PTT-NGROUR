define([
    "core/_WidgetBase",

    "dojo/text!./CoordinateBar.html",
    "xstyle/css!./CoordinateBar.css",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/dom-class",
	"dojo/dom-construct",

    "esri/geometry/webMercatorUtils",
    "esrith/amos/form/NumberTextBox",
    "esrith/amos/form/Select"
], function (
    _WidgetBase,

    template,
    xstyle,

    declare,
    lang,
    array,
    on,
    domClass,
    domConstruct,

    webMercatorUtils
    ) {
    return declare([
        _WidgetBase
    ], {
        baseClass: "coordinateBar",
        templateString: template,
        isLatLon: Boolean = true,

        //currentPositionType: 

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);
            //console.log("it\"s CoordinateBar", this.map);

            this.own(
                 on(this.map, "mouse-move", lang.hitch(this, "cursorLocation")),
                 on(this.switchGeometryTypeButton, "change", lang.hitch(this, "switchGeometryType"))
            );

        },
        cursorLocation: function (e) {
            var position = {};
            if (this.isLatLon) {
                var latlon = webMercatorUtils.xyToLngLat(e.mapPoint.x, e.mapPoint.y);
                position.x = latlon[1];
                position.y = latlon[0];
            } else {
                position.x = e.mapPoint.x;
                position.y = e.mapPoint.y;
            }

            //this.axle1.set("value", position.x.toFixed(8));
            //this.axle2.set("value", position.y.toFixed(8));

            this.axle1.innerHTML = this.addComma(position.x.toFixed(8));
            this.axle2.innerHTML = this.addComma(position.y.toFixed(8));
        },
        switchGeometryType: function (e) {
            this.isLatLon = !this.isLatLon;
            this.switchGeometryTypeButton.innerHTML = (this.isLatLon) ? "Lat/Long" : "UTM";
            //this.axle1.reset();
            //this.axle2.reset();

            this.axle1.innerHTML = "";
            this.axle2.innerHTML = "";

            if (this.isLatLon) {
                this.axle1Label.innerHTML = "Lat : ";
                this.axle2Label.innerHTML = "Long : ";
            } else {
                this.axle1Label.innerHTML = "X : ";
                this.axle2Label.innerHTML = "Y : ";
            }
        }

    });
});