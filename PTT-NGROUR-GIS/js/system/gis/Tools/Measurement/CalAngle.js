/*
AMOS3.0 - Measurement CalAngle
---------------------------------------------------
Author: 
Create date: 
Description: 
---------------------------------------------------
Modify History
Detail:
Modify date:
Modify by:
*/


/**
 * @description 
 * Measurement control is a normal text field for user to enter data. Custom icons can be add for extended functionalities.
 * @class CalAngle
 * @constructor
 * @uses _WidgetBase
 * @uses _BaseClassMixin
 */
/**
 * @description 
 * Create a new Measurement widget.
 * @method
 * @constructor
 * @param {Object} [params] -
 * @param {Node|String} [srcNodeRef] -
 */
define([
    "core/_WidgetBase",
    "core/_ProjectionMixin",

	"dojo/_base/array",
	"dojo/query",
	"dojo/dom-attr",
	"dojo/dom-style",
	"dojo/dom-class",
	"dojo/dom-construct",
	"dojo/dom-geometry",
	"dojo/on",
	"dojo/_base/event",

	"dojo/_base/lang",
	"dojo/_base/declare",

    "esri/toolbars/draw",
    "esri/layers/GraphicsLayer",
    "esri/graphic",
    "esri/geometry/Point",
    "esri/SpatialReference",
    "esri/geometry/webMercatorUtils",
    "esri/geometry/ScreenPoint",
    "esri/geometry/Polyline",
    "esri/symbols/TextSymbol",
    "esri/symbols/jsonUtils",

    "require"
], function (

	_WidgetBase,
    _ProjectionMixin,

	array,
	query,
	domAttr,
	domStyle,
	domClass,
	domConstruct,
	domGeometry,
	on,
	event,

	lang,
	declare,

    Draw,
    GraphicsLayer,
    Graphic,
    Point,
    SpatialReference,
    webMercatorUtils,
    ScreenPoint,
    Polyline,
    TextSymbol,

    jsonUtils,
    require
	) {


    var wgConstant = {
        /**
		 * ERROR_REQUIRED : `required`
		 *
		 * @attribute ERROR_REQUIRED
		 * @type String
		 * @static
		 * @readonly
		 */
        ERROR_REQUIRED: "required"
    };

    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
        _WidgetBase,
        _ProjectionMixin
    ], {
        templateString: "<div></div>",
        map: null,
        _unit: null,
        _gpLayerFirstPoint: null,
        _gpLayerBaseLine: null,
        _gpLayerLineCalculate: null,
        _gpLayerTextAngle: null,

        _cntClick: 0,
        _evtMouseMove: null,
        _evtClick: null,

        baseLineLength: null,
        angelResult: null,
        firstPointGeo: null,
        lastPointGeo: null,

        defaultConfig:null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
            //if (params) {
            //    var tmp = params;
            //    params = Config;
            //    lang.mixin(params, tmp);
            //} else {
            //    params = Config;
            //}
            //lang.mixin(this, params);
            //console.log("params", params);

            //this.isGeodesic = this.isGeodesic;
            //this._defaultPointSymbol = this.defaultSymbol.point;
            //this._defaultLineSymbol = this.defaultSymbol.line;
            //this._defaultPolygonSymbol = this.defaultSymbol.polygon;
            //this._defaultTextSymbol = this.defaultSymbol.text;
        },
        activateTool: function (unit) {
            //console.log("activateTool");
            this.reset();

            //setUnit
            this._setUnitMode(unit)

            //init Draw GpLayer
            this._createGraphicLayer();
            this._status = false;

            if (this._evtClick == null) {
                this._evtClick = on(this.map, "mouse-down", lang.hitch(this, "_onMap_MouseDown"));
            }

        },

        calculateRad: function (point1, point2, unit) {
            //console.log("point1", point1);
            //console.log("point2", point2);
            //var bearing = this.bearing(point1.y.toFixed(3), point1.x.toFixed(3), point2.y.toFixed(3), point2.x.toFixed(3));// Map lat y lonx

            var bearing;
            //console.log("unit ", unit);
            switch (unit) {
                case "DEG":
                    bearing = this.calDegree(point1.x, point1.y, point2.x, point2.y);
                    break;
                case "AZH":
                    bearing = this.calAzimuth(point1.y, point1.x, point2.y, point2.x);// Map lat y lonx
                    break;
                default:
                    bearing = this.calDegree(point1.x, point1.y, point2.x, point2.y);
                    break;
            }
            return bearing.toFixed(0);
        },

        calCompleted: function () {
            var divResult = domConstruct.create("div");

            var textAngle = "ANGLE :" + this.angelResult + "°";
            var divMove = domConstruct.create("div", { className: "divRow" }, divResult);
            domConstruct.create("div", { className: "iconMove divIcon" }, divMove);
            domConstruct.create("div", { className: "divText", innerHTML: textAngle }, divMove);

            var reflexAngle = 0;
            reflexAngle = 360 - this.angelResult;
            var textreflexAngle = "REFLEX ANGLE :" + reflexAngle + "°";
            var divMove = domConstruct.create("div", { className: "divRow" }, divResult);
            domConstruct.create("div", { className: "iconMove divIcon" }, divMove);
            domConstruct.create("div", { className: "divText", innerHTML: textreflexAngle }, divMove);

            var obj = {
                angle: this.angelResult,
                reflexAngle: reflexAngle
            };

            this.emit("calculation-completed", divResult, obj);
        },

        _setUnitMode: function (unit) {
            this._unit = unit;
            //Unit Mode
            switch (this._unit) {
                case "DEG":
                    this.baseLineLength = Math.abs(this.config.angle.baseLineLength);
                    break;
                case "AZH":
                    this.baseLineLength = -Math.abs(this.config.angle.baseLineLength);
                    break;
            }
        },

        _createGraphicLayer: function () {

            this._gpLayerTextAngle = this.map.getLayer("gEdit_MeasurementAngleTextLayer");
            if (this._gpLayerTextAngle == null) {
                this._gpLayerTextAngle = new GraphicsLayer({
                    id: "gEdit_MeasurementAngleTextLayer"
                });
                this.map.addLayer(this._gpLayerTextAngle);
            }
            this._gpLayerTextAngle.clear();

            this._gpLayerBaseLine = this.map.getLayer("gEdit_MeasurementAngleBaseLineLayer");
            if (this._gpLayerBaseLine == null) {
                this._gpLayerBaseLine = new GraphicsLayer({
                    id: "gEdit_MeasurementAngleBaseLineLayer"
                });
                this.map.addLayer(this._gpLayerBaseLine);
            }
            this._gpLayerBaseLine.clear();

            this._gpLayerLineCalculate = this.map.getLayer("gEdit_MeasurementAngleLayer");
            if (this._gpLayerLineCalculate == null) {
                this._gpLayerLineCalculate = new GraphicsLayer({
                    id: "gEdit_MeasurementAngleLayer"
                });
                this.map.addLayer(this._gpLayerLineCalculate);
            }
            this._gpLayerLineCalculate.clear();

            this._gpLayerFirstPoint = this.map.getLayer("gEdit_MeasurementAngleFirstPointLayer");
            if (this._gpLayerFirstPoint == null) {
                this._gpLayerFirstPoint = new GraphicsLayer({
                    id: "gEdit_MeasurementAngleFirstPointLayer"
                });
                this.map.addLayer(this._gpLayerFirstPoint);
            }
            this._gpLayerFirstPoint.clear();
        },

        _onMap_MouseMove: function (evt) {
            //console.log("_map_mouseMove", evt);
            this._createCalculateLine(evt.mapPoint);

            switch (this._unit) {
                case "DEG":
                    this.angelResult = this.calculateRad(this.map.toScreen(this.firstPointGeo), this.map.toScreen(this.lastPointGeo), this._unit);
                    break;
                case "AZH":
                    this.angelResult = this.calculateRad(this.firstPointGeo, this.lastPointGeo, this._unit);
                    break;
                default:
                    this.angelResult = this.calculateRad(this.map.toScreen(this.firstPointGeo), this.map.toScreen(this.lastPointGeo), this._unit);
                    break;
            }

            this._createTextGraphic();
            this.calCompleted();
        },

        _onMap_MouseDown: function (evt) {
            this._cntClick++;
            //console.log("_onMap_MouseDown ", evt, this._status, this._cntClick);
            if (this._cntClick == 2) {
                this._resetEvent();
                this._status = true;

                //กรณีเป็น mobile จะเข้าอันนี้เพราะใน mobile ไม่มี event mouse-move
                if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
                    //console.log("เป็น mobileee");
                    this._onMap_MouseMove(evt);
                } 
            }
            else {
                if (this._cntClick > 2) {
                    this.activateTool(this._unit);
                }
                if (this.firstPointGeo == null && this.lastPointGeo == null) {
                    this._createFirstPoint(evt.mapPoint);
                    this._createBaseLine();

                    if (this._evtMouseMove == null) {
                        this._evtMouseMove = on(this.map, "mouse-move", lang.hitch(this, "_onMap_MouseMove")); //ใน mobile ไม่มี event mouse-move
                    }
                    this._status = false;
                }
            }
        },

        _createFirstPoint: function (geometry) {
            //console.log("_createFirstPoint", geometry);
            this.firstPointGeo = this.project.transform(geometry, 4326);

            var graphic = new Graphic(this.firstPointGeo, this.config.angle.symbols.firstPoint);
            //console.log("graphic", graphic);
            this._gpLayerFirstPoint.clear();
            this._gpLayerFirstPoint.add(graphic);
            this._cntClick = 1;
        },

        _createBaseLine: function () {
            var screenPoint = this.map.toScreen(this.firstPointGeo);
            //console.log("screenPoint", screenPoint);
            switch (this._unit) {
                case "DEG":
                    screenPoint.x = screenPoint.x + this.baseLineLength;
                    break;
                case "AZH":
                    screenPoint.y = screenPoint.y + this.baseLineLength;
                    break;
            }

            var baselastPointGeo = this.project.transform(this.map.toMap(screenPoint), 4326);
            //console.log("baselastPointGeo", baselastPointGeo);

            var baseLineGeo = new Polyline(this.firstPointGeo.spatialReference);
            baseLineGeo.addPath([this.firstPointGeo, baselastPointGeo]);
            //console.log("baseLineGeo", baseLineGeo);

            var graphicBaseLine = new Graphic(baseLineGeo, this.config.angle.symbols.baseLine);
            //console.log("graphicBaseLine", graphicBaseLine);
            this._gpLayerBaseLine.clear();
            this._gpLayerBaseLine.add(graphicBaseLine);
        },

        _createCalculateLine: function (mouseMovePointGeo) {
            this.lastPointGeo = this.project.transform(mouseMovePointGeo, 4326);

            var calculatorLineGeo = new Polyline(this.firstPointGeo.spatialReference);
            calculatorLineGeo.addPath([this.firstPointGeo, this.lastPointGeo]);

            this._gpLayerLineCalculate.clear();
            var graphicCalculatorLine = new Graphic(calculatorLineGeo, this.config.defaultSymbol.line);
            this._gpLayerLineCalculate.add(graphicCalculatorLine);

            var graphicLastPoint = new Graphic(this.lastPointGeo, this.config.defaultSymbol.point);
            //console.log("graphicLastPoint", graphicLastPoint);
            this._gpLayerLineCalculate.add(graphicLastPoint);
        },

        _createTextGraphic: function () {
            this._gpLayerTextAngle.clear();
            
            var symbolText = jsonUtils.fromJson(this.config.defaultSymbol.text.toJson());
            symbolText.text = this.angelResult + "°";
            symbolText.setOffset(0, -18);

            //var screenPoint = this.map.toScreen(this.firstPointGeo);
            //screenPoint = screenPoint.y + 100;

            var graphicTextAngle = new Graphic(this.firstPointGeo, symbolText);
            this._gpLayerTextAngle.add(graphicTextAngle);
        },

        _getTextAnglebyFormat: function () {
            return
        },

        calDegree: function (p1x, p1y, p2x, p2y) {
            var angleDeg = Math.atan2(p2y - p1y, p2x - p1x) * 180 / Math.PI;
            var deg = 0;
            if (angleDeg < 0) { deg = angleDeg * (-1); }
            else if (angleDeg > 0) { deg = (180 - angleDeg) + 180; }
            return deg;
        },

        calAzimuth: function (lat1, lng1, lat2, lng2) {
            var deg = 0;
            var dLon = lng2 - lng1;
            var y = Math.sin(dLon) * Math.cos(lat2);
            var x = Math.cos(lat1) * Math.sin(lat2) - Math.sin(lat1) * Math.cos(lat2) * Math.cos(dLon);
            var angle = Math.atan2(y, x) * 180 / Math.PI;
            if (angle < 0) { deg = 360 + angle; }
            else { deg = angle; }
            return 360 - deg;
            //return ((brng + 360) % 360);
        },

        _resetEvent: function () {
            //Event
            if (this._evtMouseMove) {
                this._evtMouseMove.remove();
                this._evtMouseMove = null;
            }
        },

        _resetGraphic: function () {
            // Graphic Layer
            if (this._gpLayerFirstPoint) {
                this._gpLayerFirstPoint.clear();
                this.map.removeLayer(this._gpLayerFirstPoint);
                this._gpLayerFirstPoint = null;
            }

            if (this._gpLayerBaseLine) {
                this._gpLayerBaseLine.clear();
                this.map.removeLayer(this._gpLayerBaseLine);
                this._gpLayerBaseLine = null;
            }

            if (this._gpLayerLineCalculate) {
                this._gpLayerLineCalculate.clear();
                this.map.removeLayer(this._gpLayerLineCalculate);
                this._gpLayerLineCalculate = null;
            }

            if (this._gpLayerTextAngle) {
                this._gpLayerTextAngle.clear();
                this.map.removeLayer(this._gpLayerTextAngle);
                this._gpLayerTextAngle = null;
            }
        },

        reset: function () {
            this.firstPointGeo = null;
            this.lastPointGeo = null;
            this.angelResult = null;
            this._cntClick = 0;

            if (this._evtClick) {
                this._evtClick.remove();
                this._evtClick = null;
            }

            this._resetEvent();
            this._resetGraphic();
        },
    });

    lang.mixin(wgDeclare, wgConstant);

    return wgDeclare;
});