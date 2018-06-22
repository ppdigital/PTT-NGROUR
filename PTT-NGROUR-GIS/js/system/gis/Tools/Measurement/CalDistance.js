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
      "esrith/amos/core/areaRai",

    "esri/graphic",
    "esri/symbols/SimpleLineSymbol",
    "esri/symbols/SimpleFillSymbol",
    "esri/symbols/SimpleMarkerSymbol",

    "esri/symbols/TextSymbol",
    "esri/symbols/Font",
    "esri/geometry/Point",
    "esri/geometry/Polyline",
    "esri/geometry/Polygon",

    "esri/geometry/geodesicUtils",
    "esri/geometry/webMercatorUtils",
    "esri/tasks/AreasAndLengthsParameters",
    "esri/tasks/GeometryService",
    "esri/layers/GraphicsLayer",
    "esri/units",
    "esri/symbols/jsonUtils",
    "esri/geometry/geometryEngine",
    "esri/toolbars/draw",
    "esri/config",

    "dojo/store/Memory",
    "dojo/sniff",
    "dojo/number",
    "dojo/_base/Color",
    "dojo/_base/array",
    "dojo/query",
    "dojo/dom-attr",
    "dojo/dom-style",
    "dojo/dom-class",
    "dojo/dom-construct",
    "dojo/dom-geometry",
    "dojo/on",
    "dojo/_base/connect",
    "dojo/_base/event",
    "dojo/Evented",
    "dojo/_base/lang",
    "dojo/_base/declare",

    "require"

], function (
    _WidgetBase,
    AreaRai,

    Graphic,
    SimpleLineSymbol,
    SimpleFillSymbol,
    SimpleMarkerSymbol,
    TextSymbol,
    Font,
    Point,
    Polyline,
    Polygon,

    geodesicUtils,
    webMercatorUtils,
    AreasAndLengthsParameters,
    GeometryService,
    GraphicsLayer,
    units,
    jsonUtils,
    geometryEngine,
    Draw,
    config,

    Memory,
    sniff,
    number,
    Color,
    array,
    query,
    domAttr,
    domStyle,
    domClass,
    domConstruct,
    domGeometry,
    on,
    connect,
    event,
    Evented,
    lang,
    declare,

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
        _WidgetBase
    ], {
        templateString: "<div></div>",
        className: "amos-measurement-distance",

        map: null,
        _graphicsLayer: null,
        _graphicsPointLayer: null,
        tempLastLine: null,
        pattern: "#,###.##",

        unit: "",
        unitLabel: "",
        unitShort: "",
        geodesicWkid: [3857, 102100, 4326],
        tempGeometry: null,
        showLableDistances: false,
        _sumDistance: null,

        _drawTools: null,

        // attr overide 
        _geometryService: null,
        _polylineGraphics: [],
        _inputPoints: [],
        _borderlessFillSymbol: null,
        _currentStartPt: null,
        activeTool: "area",
        _textsymbol: null,
        _segmentTextSymbol: null,
        _segmentTextSymbolOnMouseMove: null,
        _segmentSymbolOnMouseMove:null,
        _mouseClickMapHandler: null,
        _mouseMoveMapHandler: null,
        _tempPoint: null,


        constructor: function (params, srcNodeRef) {
            params = params || {};
        },

        _checkCS: function (a)
        {
            if (a.wkid) return 3857 === a.wkid || 102100 === a.wkid || 102113 === a.wkid ? "Web Mercator" : O.isDefined(P[a.wkid]) ? "PCS" : "GCS"; if (a.wkt) return -1 !== a.wkt.indexOf("WGS_1984_Web_Mercator") ? "Web Mercator" : 0 === a.wkt.indexOf("PROJCS") ? "PCS" : "GCS";
        },

        activateTool: function (item, value, label)
        {
            this.map.cs = this._checkCS(this.map.spatialReference);

            this._graphicsLayer = this.map.getLayer("gEdit_MeasurementDistanceLayer");
            if (this._graphicsLayer == null) {
                this._graphicsLayer = new GraphicsLayer({
                    id: "gEdit_MeasurementDistanceLayer"
                });
                this.map.addLayer(this._graphicsLayer); //เส้น
            }


            this._graphicsPointLayer = this.map.getLayer("gEdit_MeasurementDistancePointLayer");
            if (this._graphicsPointLayer == null) {
                this._graphicsPointLayer = new GraphicsLayer({
                    id: "gEdit_MeasurementDistancePointLayer"
                });
                this.map.addLayer(this._graphicsPointLayer); //point
            }

            this._graphicsTextLayer = this.map.getLayer("gEdit_MeasurementDistanceTextLayer");
            if (this._graphicsTextLayer == null) {
                this._graphicsTextLayer = new GraphicsLayer({
                    id: "gEdit_MeasurementDistanceTextLayer"
                });
                this.map.addLayer(this._graphicsTextLayer); //ระยะทาง
            }

            if (item) {
                this.unit = item["VALUE"];
                this.unitLabel = item["LABEL"];
                this.unitShort = item["SHORTLABEL"];

                if (this.tempGeometry == null)
                    this._setupDrawTool();
                else
                {
                    var result = this.calculateDistance(this.tempGeometry, item["VALUE"]);
                    this._manageResult(result);
                    this._updateTextSymbol(result);
                }
            }

        },

        _setupDrawTool: function ()
        {
            this.map.disableDoubleClickZoom();
            this._inputPoints = [];

            if (this._mouseClickMapHandler == null) {
                this._mouseClickMapHandler = on(this.map, "click", lang.hitch(this, "_measureDistanceMouseClickHandler"));
            }
            if (this._doubleClickMapHandler == null) {
                this._doubleClickMapHandler = on(this.map, "dbl-click", lang.hitch(this, "_measureDistanceDblClickHandler"));
            }

            this._drawTools = this.map.esrith.tools.draw.activate({
                type: "polyline",
            });

            array.forEach(this._drawTools, lang.hitch(this, function (drawTool)
            {
                //drawTool.setFillSymbol(this.config.defaultSymbol.polygon);
                //drawTool.setLineSymbol(this.config.defaultSymbol.line);
                //drawTool.setMarkerSymbol(this.config.defaultSymbol.point);
                drawTool.setFillSymbol(new SimpleFillSymbol(SimpleFillSymbol.STYLE_NULL));
                drawTool.setLineSymbol(new SimpleLineSymbol(SimpleLineSymbol.STYLE_NULL));
                drawTool.setMarkerSymbol(new SimpleMarkerSymbol(SimpleMarkerSymbol.STYLE_NULL));
            }));
        },

        _measureDistanceMouseClickHandler: function (a)
        {    
            var c;
            this.map.snappingManager && (c = this.map.snappingManager._snappingPoint);
            c = c || a.mapPoint;

            this._inputPoints.push(c);
            this._tempPoint = c;

            if (this._segmentSymbolOnMouseMove != null)
                this._graphicsLayer.remove(this._segmentSymbolOnMouseMove);
            this._segmentSymbolOnMouseMove = null;

            if (this._inputPoints.length <= 1) 
            {
                this._currentStartPt = c;
                this._sumDistance = 0;
                this._graphicsLayer.clear();
                this._graphicsPointLayer.clear();
                this._graphicsTextLayer.clear();

                this.tempGeometry = null;
                this._segmentTextSymbol = [];
                this._segmentTextSymbolOnMouseMove = null;
                this.result = 0;
                this._manageResult(this.result);

                var pointGraphic = new Graphic(c, this.config.defaultSymbol.point);
                this._graphicsPointLayer.add(pointGraphic);

                if (this._mouseMoveMapHandler == null)
                    this._mouseMoveMapHandler = connect.connect(this.map, "onMouseMove", this, "_measureAreaMouseMoveHandler");
            }
            else
            {
                var line = new Polyline(this.map.spatialReference);
                line.addPath([this._inputPoints[this._inputPoints.length - 2], c]);
                if (this.config.isGeodesic == true)
                    line = this._densifyGeometry(line);

                this.tempLastLine = new Graphic(line, this._areaLineSymbol);

                if (this.config.distance.showSegmentLabel)
                {
                    var path = [this._inputPoints[this._inputPoints.length - 2], c];
                    var segment = new Polyline(this.map.spatialReference);
                    segment.addPath(path);
                    if (this.config.isGeodesic == true)
                        segment = this._densifyGeometry(segment);

                    var segmentLength = parseFloat(this.calculateDistance(segment, this.unit));

                    var param = {
                        geometry: segment,
                        unit: this.unit,
                        shortUnit: this.unitShort,
                        value: segmentLength,
                        text: number.format(segmentLength, { pattern: this.config.distance.pattern }) + " " + this.unitShort
                    };

                    var segmentSymbol = this.createTextSymbol(param);
                    this._segmentTextSymbol.push(segmentSymbol);
                    this._graphicsTextLayer.add(segmentSymbol);
                }

                var multiPoint = this._inputPoints;
                var polyLine = new Polyline(this.map.spatialReference);
                for (i = 0; i < multiPoint.length - 1; i++) {
                    polyLine.addPath([multiPoint[i], multiPoint[i + 1]]);
                }
                if (this.config.isGeodesic == true)
                    polyLine = this._densifyGeometry(polyLine);

                this.tempGeometry = polyLine;
                this.tempGeometry._measurementWidget_inputPoints = this._inputPoints;

                var result = parseFloat(parseFloat(this.calculateDistance(polyLine, this.unit)).toFixed(2));
                this._sumDistance = result;

                var pointGraphic = new Graphic(c, this.config.defaultSymbol.point);
                this._graphicsPointLayer.add(pointGraphic);

                var segmentGraphic  = new Graphic(polyLine, this.config.defaultSymbol.line);
                this._graphicsLayer.add(segmentGraphic);
                this._manageResult(result);
            } 
        },

        _measureDistanceDblClickHandler: function (evt)
        {
            if ("touch" === this.map.navigationManager.eventModel && sniff("ios")) {
                //this._measureAreaMouseClickHandler(a);
                if (this._mouseClickMapHandler != null) {
                    this._mouseClickMapHandler.remove();
                    setTimeout(lang.hitch(this, function () {
                        this._mouseClickMapHandler = on(this.map, "click", lang.hitch(this, "_measureDistanceMouseClickHandler"));
                    }), 100);
                }
            }

            if (this._textsymbol != null)
                this._graphicsTextLayer.remove(this._textsymbol);

            if (this._inputPoints.length >= 2)
            {
                if (this._segmentTextSymbolOnMouseMove != null)
                    this._graphicsTextLayer.remove(this._segmentTextSymbolOnMouseMove);
                this._segmentTextSymbolOnMouseMove = null;

                var segmentLength = parseFloat(this.calculateDistance(this.tempGeometry, this.unit));

                var param = {
                    geometry: evt.mapPoint,
                    unit: this.unit,
                    shortUnit: this.unitShort,
                    value: segmentLength,
                    text: number.format(segmentLength, { pattern: this.config.distance.pattern }) + " " + this.unitShort
                };

                this._textsymbol = this.createTextSymbol(param);
                this._graphicsTextLayer.add(this._textsymbol);   

                this._inputPoints = [];
            }
        },

        _measureAreaMouseMoveHandler: function (evt)
        {
            if (this._segmentTextSymbolOnMouseMove != null)
                this._graphicsTextLayer.remove(this._segmentTextSymbolOnMouseMove);

            if (this._segmentSymbolOnMouseMove != null)
                this._graphicsLayer.remove(this._segmentSymbolOnMouseMove);

            if (this._inputPoints.length >= 1)
            {
                if (this._textsymbol != null)
                    this._graphicsTextLayer.remove(this._textsymbol);

                if (this._inputPoints.length === 1)
                {
                    //   this.tempGeometry = new Polyline(this.map.spatialReference);
                    this._sumDistance = 0;
                }
                //console.log(this.tempGeometry, "this.tempGeometry;");
                //var polyLine = null;
                //polyLine = new Polyline(this.tempGeometry.toJson());

                var path = [this._tempPoint, evt.mapPoint];
                var segment = new Polyline(this.map.spatialReference);
                segment.addPath(path);
                if (this.config.isGeodesic == true)
                    segment = this._densifyGeometry(segment);

                this._segmentSymbolOnMouseMove = new Graphic(segment, this.config.defaultSymbol.line);
                this._graphicsLayer.add(this._segmentSymbolOnMouseMove);

                var segmentLength = parseFloat(this.calculateDistance(segment, this.unit));
                var totalDistance = segmentLength + this._sumDistance;

                if (this.config.distance.showSegmentLabel)
                {
                    var param = {
                        geometry: segment,
                        unit: this.unit,
                        shortUnit: this.unitShort,
                        value: segmentLength,
                        text: number.format(segmentLength, { pattern: this.config.distance.pattern }) + " " + this.unitShort
                    };

                    this._segmentTextSymbolOnMouseMove = this.createTextSymbol(param);
                    this._graphicsTextLayer.add(this._segmentTextSymbolOnMouseMove);
                }

                var param = {
                    geometry: evt.mapPoint,
                    unit: this.unit,
                    shortUnit: this.unitShort,
                    value: totalDistance,
                    text: number.format(totalDistance, { pattern: this.config.distance.pattern }) + " " + this.unitShort
                };

                this._textsymbol = this.createTextSymbol(param);
                this._graphicsTextLayer.add(this._textsymbol);
                this._manageResult(totalDistance);
            }
        },

        createTextSymbol: function (param)
        {
            var point = null;
            var angle = null;
            var offsetX = null;
            var offsetY = null;

            if (param.geometry.type == "polyline")
            {
                var deltaY = param.geometry.paths[0][param.geometry.paths[0].length - 1][1] - param.geometry.paths[0][0][1];
                var deltaX = param.geometry.paths[0][param.geometry.paths[0].length - 1][0] - param.geometry.paths[0][0][0];
                var angleDeg = Math.atan2(deltaY, deltaX) * 180 / Math.PI;

                if (angleDeg < 0)
                    angleDeg = 360 + angleDeg;

                if (angleDeg > 90 && angleDeg < 270)
                    angleDeg += 180;

                angleDeg = angleDeg % 360;

                angle = 360 - angleDeg;
                offsetX = Math.sin(angle * Math.PI / 180) * 10;
                offsetY = Math.cos(angle * Math.PI / 180) * 10;

                if (param.geometry.paths[0].length > 2)
                {
                    var middleIndex = (param.geometry.paths[0].length - 1) / 2;
                    point = new Point((param.geometry.paths[0][Math.floor(middleIndex)][0] + param.geometry.paths[0][Math.ceil(middleIndex)][0]) / 2,
                              (param.geometry.paths[0][Math.floor(middleIndex)][1] + param.geometry.paths[0][Math.ceil(middleIndex)][1]) / 2,
                              this.map.spatialReference);
                }
                else if (param.geometry.paths[0].length == 2)
                    point = new Point((param.geometry.paths[0][1][0] + param.geometry.paths[0][0][0]) / 2,
                              (param.geometry.paths[0][1][1] + param.geometry.paths[0][0][1]) / 2,
                              this.map.spatialReference);
                else if (param.geometry.paths[0].length == 1)
                {
                    point = new Point(param.geometry.paths[0][0][0], param.geometry.paths[0][0][1], this.map.spatialReference);
                }
                else
                    point = new Point();
            }
            else if (param.geometry.type == "point")
                point = param.geometry;
            else
            {
                point = param.geometry.getCentroid();
                offsetX = 0;
                offsetY = 0;
            }

            var symbolText = jsonUtils.fromJson(this.config.defaultSymbol.text.toJson());
            symbolText.setText(param.text);

            if (angle != null)
                symbolText.setAngle(angle);

            if (offsetX != null && offsetY != null)
                symbolText.setOffset(offsetX, offsetY);

            var labelPointGraphic = new Graphic(point, symbolText, param);
            return labelPointGraphic;
        },

        _updateTextSymbol: function (result)
        {
            var i;

            if (this._textsymbol != null) 
                this._graphicsTextLayer.remove(this._textsymbol);
            
            if (this._segmentTextSymbolOnMouseMove != null)
                this._graphicsTextLayer.remove(this._segmentTextSymbolOnMouseMove);

            if (this._segmentTextSymbol != null)
            {
                for (i = 0; i < this._segmentTextSymbol.length; i++)
                    this._graphicsTextLayer.remove(this._segmentTextSymbol[i]);
            }
            this._segmentTextSymbol = [];

            var pointList = this.tempGeometry._measurementWidget_inputPoints;

            if (pointList.length >= 2)
            {
                if (this.config.distance.showSegmentLabel)
                {
                    for (i = 0; i < pointList.length - 1; i++)
                    {
                        //var path = [new Point(this.tempGeometry.paths[i][0][0], this.tempGeometry.paths[i][0][1], this.map.spatialReference),
                        //            new Point(this.tempGeometry.paths[i][1][0], this.tempGeometry.paths[i][1][1], this.map.spatialReference)];
                        var path = [pointList[i], pointList[i + 1]];

                        var segment = new Polyline(this.map.spatialReference);
                        segment.addPath(path);
                        if (this.config.isGeodesic == true)
                            segment = this._densifyGeometry(segment);
                        var segmentLength = parseFloat(this.calculateDistance(segment, this.unit));

                        var param = {
                            geometry: segment,
                            unit: this.unit,
                            shortUnit: this.unitShort,
                            value: segmentLength,
                            text: number.format(segmentLength, { pattern: this.config.distance.pattern }) + " " + this.unitShort
                        };

                        var segmentSymbol = this.createTextSymbol(param);
                        this._segmentTextSymbol.push(segmentSymbol);
                        this._graphicsTextLayer.add(segmentSymbol);
                    }
                }

                //var point = new Point(this.tempGeometry.paths[this.tempGeometry.paths.length - 1][1][0], this.tempGeometry.paths[this.tempGeometry.paths.length - 1][1][1], this.map.spatialReference);
                //console.log(point);
                var point = pointList[pointList.length - 1];

                var param = {
                    geometry: point,
                    unit: this.unit,
                    shortUnit: this.unitShort,
                    value: result,
                    text: number.format(result, { pattern: this.config.distance.pattern }) + " " + this.unitShort
                };

                this._textsymbol = this.createTextSymbol(param);
                //console.log(this._textsymbol);
                this._graphicsTextLayer.add(this._textsymbol);
            }
        },

        calculateDistance: function (geometry, unit)
        {
            var result = "";
            var multiplier = 1;

            if (unit == "wa")
            {
                unit = "meters";
                multiplier = 0.5;
            }

            try {
                if (this.config.isGeodesic == true)
                {
                    if (this.geodesicWkid.indexOf(geometry.spatialReference.wkid) >= 0)
                    {
                        result = geometryEngine.geodesicLength(geometryEngine.simplify(geometry), unit);
                    }
                    else
                    {
                        result = geometryEngine.planarLength(geometryEngine.simplify(geometry), unit);
                    }
                }
                else
                {
                    result = geometryEngine.planarLength(geometryEngine.simplify(geometry), unit);
                }
            }
            catch (err) {
                console.log(err);
            }

            return result * multiplier;
        },

        _manageResult: function (result) {
            var divResult;
            divResult = domConstruct.create("div", {
                innerHTML: number.format(result, { pattern: this.config.distance.pattern }) + " " + this.unitLabel
            });
            this.emit("calculation-completed", divResult);
        },

        getUnit: function () {
            if ("unit" !== this.unit) {
                return this.unit;
            }
        },
        _setUnitAttr: function (unit) {
            this.unit = unit;
        },

        _densifyGeometry: function (d)
        {
            "Web Mercator" === this.map.cs && (d = webMercatorUtils.webMercatorToGeographic(d));
            d = "PCS" === this.map.cs ? d : geodesicUtils.geodesicDensify(d, 5E5); "Web Mercator" === this.map.cs && (d = webMercatorUtils.geographicToWebMercator(d));
            return d;
        },

        reset: function () {
            this.tempGeometry = null;
            this._tempPoint = null;
            this._polylineGraphics = [];
            this._inputPoints = [];
            this.map.esrith.tools.deactivate();
            this.map.enableDoubleClickZoom();

            if (this._mouseClickMapHandler != null) {
                this._mouseClickMapHandler.remove();
                this._mouseClickMapHandler = null;
            }

            if (this._mouseMoveMapHandler != null) {
                this._mouseMoveMapHandler.remove();
                this._mouseMoveMapHandler = null;
            }

            if (this._doubleClickMapHandler != null) {
                this._doubleClickMapHandler.remove();
                this._doubleClickMapHandler = null;
            }

            if (this._graphicsLayer) {
                this._graphicsLayer.clear();
                this.map.removeLayer(this._graphicsLayer);
                this._graphicsLayer = null;
            }

            if (this._graphicsPointLayer) {
                this._graphicsPointLayer.clear();
                this.map.removeLayer(this._graphicsPointLayer);
                this._graphicsPointLayer = null;
            }

            if (this._graphicsTextLayer) {
                this._graphicsTextLayer.clear();
                this.map.removeLayer(this._graphicsTextLayer);
                this._graphicsTextLayer = null;

            }

        }
    });

    lang.mixin(wgDeclare, wgConstant);


    return wgDeclare;
});