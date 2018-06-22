/*
AMOS3.0 - Measurement CalArea
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
 * @class Measurement
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
    "dojo/Evented",
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
    Evented,
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
        className: "amos-Measurement",

        map: null,
        _unit: null,
        _gpLayerPoint: null,
        _gpLayerPoint: null,
        _gpLayerLineCalculate: null,
        _gpLayerTextAngle: null,

        _drawObj: null,
        _drawObjHanlder: null,

        _evtMouseMove: null,

        _defaultPointSymbol: null,
        _defaultLineSymbol: null,
        _defaultPolygonSymbol: null,
        _defaultTextSymbol: null,

        baseLineLength: null,
        angelResult: null,
        firstPointGeo: null,
        lastPointGeo: null,

        constructor: function (params, srcNodeRef) {
            //if (params) {
            //    var tmp = params;
            //    params = Config;
            //    lang.mixin(params, tmp);
            //} else {
            //    params = Config;
            //}
            //lang.mixin(this, params);
            //console.log("params", params);

            //this.config.isGeodesic = this.config.isGeodesic;
            //this.config.defaultSymbol.point = this.defaultSymbol.point;
            //this.config.defaultSymbol.line = this.defaultSymbol.line;
            //this.config.defaultSymbol.polygon = this.defaultSymbol.polygon;
            //this.config.defaultSymbol.text = this.defaultSymbol.text;
        },

        activateTool: function (unit) {
            this.reset();

            //setUnit
            this._setUnitMode(unit)

            this._createGraphicLayer();
            this._status = false;

            if (this._evtClick == null) {
                this._evtClick = on(this.map, "mouse-down", lang.hitch(this, "_onMap_MouseDown"));
            }

        },

        reset: function () {
            this.firstPointGeo = null;
            this.secondPointGeo = null;
            this.lastPointGeo = null;
            this.angelResult = null;

            if (this._evtClick) {
                this._evtClick.remove();
                this._evtClick = null;
            }

            this._resetEvent();
            this._resetGraphic();
        },

        calculateRad: function (point1, point2, point3, unit) {
            var bearing;
            var deg1, deg2;
            switch (unit) {
                case "DEG":
                    deg1 = this.calDegree(point2.x, point2.y, point1.x, point1.y); // Calculate Deg(RotationPoint, firstPoint);
                    deg2 = this.calDegree(point2.x, point2.y, point3.x, point3.y); // Calculate Deg(RotationPoint, LastPoint);
                    bearing = Math.abs(deg1 - deg2);
                    break;
                default:
                    deg1 = this.calDegree(point2.x, point2.y, point1.x, point1.y);
                    deg2 = this.calDegree(point2.x, point2.y, point3.x, point3.y);
                    bearing = Math.abs(deg1 - deg2);
                    break;
            }
            if (bearing > 180) {
                bearing = 360 - bearing;
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

        _resetEvent: function () {
            //Event
            if (this._evtMouseMove) {
                this._evtMouseMove.remove();
                this._evtMouseMove = null;
            }
        },

        _resetGraphic: function () {
            // Graphic Layer
            if (this._gpLayerPoint) {
                this._gpLayerPoint.clear();
                this.map.removeLayer(this._gpLayerPoint);
            }

            if (this._gpLayerLineCalculate) {
                this._gpLayerLineCalculate.clear();
                this.map.removeLayer(this._gpLayerLineCalculate);
            }

            if (this._gpLayerSecondLine) {
                this._gpLayerSecondLine.clear();
                this.map.removeLayer(this._gpLayerSecondLine);
            }

            if (this._gpLayerTextAngle) {
                this._gpLayerTextAngle.clear();
                this.map.removeLayer(this._gpLayerTextAngle);
            }
        },

        _setUnitMode: function (unit) {
            this._unit = unit;
            //Unit Mode
            switch (this._unit) {
                case "DEG":
                    this.baseLineLength = Math.abs(this.baseLineLength);
                    break;
            }
        },

        _createGraphicLayer: function () {
            this._gpLayerTextAngle = this.map.getLayer("gEdit_MeasurementFreehandAngleText");
            if (this._gpLayerTextAngle == null) {
                this._gpLayerTextAngle = new GraphicsLayer({
                    id: "gEdit_MeasurementFreehandAngleText"
                });
                this.map.addLayer(this._gpLayerTextAngle, 0);
            }
            this._gpLayerTextAngle.clear();


            this._gpLayerSecondLine = this.map.getLayer("gEdit_MeasurementFreehandAngleSecondLine");
            if (this._gpLayerSecondLine == null) {
                this._gpLayerSecondLine = new GraphicsLayer({
                    id: "gEdit_MeasurementFreehandAngleSecondLine"
                });
                this.map.addLayer(this._gpLayerSecondLine);
            }
            this._gpLayerSecondLine.clear();

            this._gpLayerLineCalculate = this.map.getLayer("gEdit_MeasurementFreehandAngleLineCal");
            if (this._gpLayerLineCalculate == null) {
                this._gpLayerLineCalculate = new GraphicsLayer({
                    id: "gEdit_MeasurementFreehandAngleLineCal"
                });
                this.map.addLayer(this._gpLayerLineCalculate);
            }
            this._gpLayerLineCalculate.clear();

            this._gpLayerPoint = this.map.getLayer("gEdit_MeasurementFreehandAnglePoint");
            if (this._gpLayerPoint == null) {
                this._gpLayerPoint = new GraphicsLayer({
                    id: "gEdit_MeasurementFreehandAnglePoint"
                });
                this.map.addLayer(this._gpLayerPoint);
            }
            this._gpLayerPoint.clear();
        },

        _map_mouseMove: function (evt) {
            //console.log("_map_mouseMove", evt);
            if (this.secondPointGeo == null) {
                this._createBaseLine(evt.mapPoint);
            }
            else {
                this._createCalculateLine(evt.mapPoint);

                switch (this._unit) {
                    case "DEG":
                        this.angelResult = this.calculateRad(this.map.toScreen(this.firstPointGeo), this.map.toScreen(this.secondPointGeo), this.map.toScreen(this.lastPointGeo), this._unit);
                        break;
                    default:
                        this.angelResult = this.calculateRad(this.map.toScreen(this.firstPointGeo), this.map.toScreen(this.secondPointGeo), this.map.toScreen(this.lastPointGeo), this._unit);
                        break;
                }

                this._createTextGraphic();
                this.calCompleted();
            }

        },

        _cntClick: 0,
        _onMap_MouseDown: function (evt) {
            this._cntClick++;
            //console.log("_onMap_MouseDown ", evt, this._status, this._cntClick);
            if (this._cntClick == 3) {
                this._resetEvent();
                this._status = true;
            }
            else {
                if (this._cntClick > 3) {
                    this.activateTool(this._unit);
                }
                if (this.firstPointGeo == null) {
                    this._createFirstPoint(evt.mapPoint);
                    this._status = false;
                    if (this._evtMouseMove == null) {
                        this._evtMouseMove = on(this.map, "mouse-move", lang.hitch(this, "_map_mouseMove"));
                    }
                }
                else if (this.secondPointGeo == null) {
                    this._createSecondPoint(evt.mapPoint);
                    this._status = false;
                }
            }
        },

        _createFirstPoint: function (geometry) {
            //console.log("_createFirstPoint", geometry);
            this.firstPointGeo = this.project.transform(geometry, 4326);

            var graphic = new Graphic(this.firstPointGeo, this.config.defaultSymbol.point);
            //console.log("graphic", graphic);
            this._gpLayerPoint.clear();
            this._gpLayerPoint.add(graphic);
            this._cntClick = 1;
        },
        _createSecondPoint: function (geometry) {
            //console.log("_createSecondPoint", geometry);
            this.secondPointGeo = this.project.transform(geometry, 4326);

            var calculatorLineGeo = new Polyline(this.firstPointGeo.spatialReference);
            calculatorLineGeo.addPath([this.firstPointGeo, this.secondPointGeo]);
            this._gpLayerLineCalculate.clear();
            var graphicCalculatorLine = new Graphic(calculatorLineGeo, this.config.defaultSymbol.line);
            this._gpLayerLineCalculate.add(graphicCalculatorLine);

            var graphic = new Graphic(this.secondPointGeo, this.config.defaultSymbol.point);
            //console.log("graphic", graphic);
            //this._gpLayerPoint.clear();
            //this._gpLayerPoint.add(graphic);
        },
        _createBaseLine: function (mouseMovePointGeo) {
            var tempSecond = this.project.transform(mouseMovePointGeo, 4326);

            var calculatorLineGeo = new Polyline(this.firstPointGeo.spatialReference);
            calculatorLineGeo.addPath([this.firstPointGeo, tempSecond]);

            this._gpLayerSecondLine.clear();
            var graphicCalculatorLine = new Graphic(calculatorLineGeo, this.config.defaultSymbol.line);
            this._gpLayerSecondLine.add(graphicCalculatorLine);

            var graphicSecondPoint = new Graphic(tempSecond, this.config.defaultSymbol.point);
            //console.log("graphicLastPoint", graphicLastPoint);
            //this._gpLayerPoint.clear();
            //this._gpLayerPoint.add(graphicSecondPoint);
        },

        _createCalculateLine: function (mouseMovePointGeo) {
            this.lastPointGeo = this.project.transform(mouseMovePointGeo, 4326);

            var calculatorLineGeo = new Polyline(this.firstPointGeo.spatialReference);
            calculatorLineGeo.addPath([this.firstPointGeo, this.secondPointGeo, this.lastPointGeo]);

            this._gpLayerSecondLine.clear();
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

            //var screenPoint = this.map.toScreen(this.firstPointGeo);
            //screenPoint = screenPoint.y + 100;

            var graphicTextAngle = new Graphic(this.secondPointGeo, symbolText);
            this._gpLayerTextAngle.add(graphicTextAngle);
        },

        calDegree: function (p1x, p1y, p2x, p2y) {
            var angleDeg = Math.atan2(p2y - p1y, p2x - p1x) * 180 / Math.PI;
            var deg = 0;
            if (angleDeg < 0) { deg = angleDeg * (-1); }
            else if (angleDeg > 0) { deg = (180 - angleDeg) + 180; }
            return deg;
        },

        //calAzimuth: function (lat1, lng1, lat2, lng2) {
        //    var deg = 0;
        //    var dLon = lng2 - lng1;
        //    var y = Math.sin(dLon) * Math.cos(lat2);
        //    var x = Math.cos(lat1) * Math.sin(lat2) - Math.sin(lat1) * Math.cos(lat2) * Math.cos(dLon);
        //    var angle = Math.atan2(y, x) * 180 / Math.PI;
        //    if (angle < 0) { deg = 360 + angle; }
        //    else { deg = angle; }
        //    return 360 - deg;
        //    //return ((brng + 360) % 360);
        //}

    });


    lang.mixin(wgDeclare, wgConstant);


    return wgDeclare;
});