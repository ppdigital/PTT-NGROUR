define([
    "core/_WidgetBase",

    "dojo/text!./GotoXY.html",
    "xstyle/css!./GotoXY.css",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/dom-class",
	"dojo/dom-construct",

    "esri/geometry/Point",
    "esri/geometry/webMercatorUtils",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/graphic",

    "esrith/amos/form/Button",
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

    Point,
    webMercatorUtils,
    SimpleMarkerSymbol,
    graphic
    ) {
    return declare([
        _WidgetBase
    ], {
        baseClass: "gotoXYComponent",
        templateString: template,
        markerPoint: null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);
            //console.log('it\'s GotoXY', this.map);

            this.own(
                 //on(this.map, "mouse-move", lang.hitch(this, 'cursorLocation')),
                 on(this.projectionType, "change", lang.hitch(this, 'switchGeometryType')),
                 on(this.submit, "click", lang.hitch(this, 'gotoPosition')),
                 on(this.clear, "click", lang.hitch(this, 'clearValueAndGraphic'))
            );

        },
        switchGeometryType: function(){
            //console.log(this.projectionType.value);
            this.clearValueAndGraphic();
            if (this.projectionType.value == "Lat/Lon") {
                this.axle1Label.innerHTML = 'Lat';
                this.axle2Label.innerHTML = 'Lon';
            } else {
                this.axle1Label.innerHTML = 'X';
                this.axle2Label.innerHTML = 'Y';
            }
        },
        //cursorLocation: function(e){
        //    console.log(e.mapPoint);
        //},
        gotoPosition: function(){
            
            if (this.validatePosition(this.axle1.value, this.axle2.value)) {
                var axle1 = parseFloat(this.axle1.value),
                    axle2 = parseFloat(this.axle2.value);
                if(this.projectionType.value == "Lat/Lon"){
                    var positionTransformed = webMercatorUtils.lngLatToXY(axle2, axle1);
                    axle1 = positionTransformed[0];
                    axle2 = positionTransformed[1];
                }
                //console.log('validate');
                var point = new Point(axle1, axle2, this.map.spatialReference);
                //console.log(point);
                if(this.map.getZoom() < 10){
                    this.map.centerAndZoom(point, 10);
                }else{
                    this.map.centerAt(point);
                }
                var symbol = new SimpleMarkerSymbol({
                    "type": "esriSMS",
                    "style": "esriSMSCircle",
                    "color": [0, 0, 255, 128],
                    "size": 12,
                    "angle": 0,
                    "xoffset": 0,
                    "yoffset": 0,
                    "outline": {
                        "type": "esriSLS",
                        "style": "esriSLSSolid",
                        "color": [0, 0, 0, 255],
                        "width": 1
                    }
                });
                var marker = new graphic(point, symbol);
                //console.log(marker);
                this.clearGraphic();
                this.map.graphics.add(marker);
                this.markerPoint = marker;
            }
            //console.log(this.verticleValue.value, this.horizontalValue.value);
        },
        validatePosition:function(x, y){
            return this.isNumber(x) && this.isNumber(y);
        },
        isNumber: function (n) {
          return !isNaN(parseFloat(n)) && isFinite(n);
        },
        clearValueAndGraphic: function () {
            this.axle1.reset();
            this.axle2.reset();
            this.clearGraphic();
        },
        clearGraphic: function () {
            if (this.markerPoint != null) {
                this.map.graphics.remove(this.markerPoint);
                this.markerPoint = null;
            }
        }
    });
});