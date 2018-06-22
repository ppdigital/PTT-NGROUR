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
    "esri/symbols/jsonUtils",

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
        ERROR_REQUIRED: "required",


    };

    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
        _WidgetBase
    ], {
        templateString: "<div></div>",
        className: "amos-measurement-area",
        unit: null,
        unitLabel: null,
        segmentUnit:null,
        map: null,
        _graphicsLayer: null,

        geodesicWkid: [3857, 102100, 4326],

        pattern: "#,###.##",

        defaultConfig:null,
        tempGeometry: null,
        _drawTools: null,
        // attr overide
        _startLineSegmentGraphic:null,
        _tempGraphic: null,
        _geometryService: null,
        _segmentTextSymbolOnMouseMove: null,
        _areaTextSymbolOnMouseMove:null,
        _segmentTextSymbol:null,
        _measurePointGraphics:[],
        _polylineGraphics: [],
        _inputPoints: [],
        _currentStartPt: null,
        activeTool: "area",

        _mouseClickMapHandler: null,
        _doubleClickMapHandler: null,
        _mouseMoveMapHandler: null,

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

            //this._geometryService = config.defaults.geometryService;
            this.map = params.map;
        },

        _checkCS: function (a) {
            if (a.wkid) return 3857 === a.wkid || 102100 === a.wkid || 102113 === a.wkid ? "Web Mercator" : O.isDefined(P[a.wkid]) ? "PCS" : "GCS"; if (a.wkt) return -1 !== a.wkt.indexOf("WGS_1984_Web_Mercator") ? "Web Mercator" : 0 === a.wkt.indexOf("PROJCS") ? "PCS" : "GCS";
        },

        activateTool: function (item)
        {
            this.map.cs = this._checkCS(this.map.spatialReference);

            this._graphicsLayer = this.map.getLayer("gEdit_MeasurementAreaLayer");
            if (this._graphicsLayer == null) {
                this._graphicsLayer = new GraphicsLayer({
                    id: "gEdit_MeasurementAreaLayer"
                });
                this.map.addLayer(this._graphicsLayer);                
                on(this._graphicsLayer, "graphic-add", lang.hitch(this, this._graphicAdded));
            }

            this.unit = item["VALUE"];
            this.unitLabel = item["LABEL"];
            this.unitShort = item["SHORTLABEL"];
            this.segmentUnit = item["SEGMENT_UNIT"];

            if (this._polygonGraphic == null)
            {
                this._setupDrawTool();
            } else {
                this.calculateArea(this._polygonGraphic.geometry, this.unit);
                this._updateTextSymbol(this._polygonGraphic.geometry);
            }
        },

        _updateTextSymbol: function (geometry)
        {
            var i;
            if (this._segmentTextSymbolOnMouseMove != null)
            {
                for (i = 0; i < this._segmentTextSymbolOnMouseMove.length; i++)
                    this._graphicsLayer.remove(this._segmentTextSymbolOnMouseMove[i]);
            }
            this._segmentTextSymbolOnMouseMove = [];

            var pointList = geometry._measurementWidget_inputPoints;//geometry.rings[0];

            if (this._segmentTextSymbol != null)
                for (i = 0; i < this._segmentTextSymbol.length; i++)
                {
                    this._graphicsLayer.remove(this._segmentTextSymbol[i]);
                }

            this._segmentTextSymbol = [];

            if (this.config.area.showSegmentLabel)
            {
                for (i = 0; i < pointList.length; i++)
                {
                    var point1 = pointList[i];
                    var point2 = pointList[(i + 1) % pointList.length];
                    //point1 = new Point(point1[0], point1[1], this.map.spatialReference);
                    //point2 = new Point(point2[0], point2[1], this.map.spatialReference);

                    var segment = new Polyline(this.map.spatialReference);
                    segment.addPath([point1, point2]);
                    if (this.config.isGeodesic == true)
                        segment = this._densifyGeometry(segment);

                    var segmentLength = parseFloat(this.calculateDistance(segment, this.segmentUnit.VALUE));

                    var param = {
                        geometry: segment,
                        unit: this.segmentUnit.VALUE,
                        shortUnit: this.segmentUnit.SHORTLABEL,
                        value: segmentLength,
                        text: number.format(segmentLength, { pattern: this.config.area.pattern }) + " " + this.segmentUnit.SHORTLABEL
                    };

                    var segmentTextSymbol = this.createTextSymbol(param);
                    this._segmentTextSymbol.push(segmentTextSymbol);
                    this._graphicsLayer.add(segmentTextSymbol);
                }
            }
        },

        _graphicAdded: function(graphic)
        {
            var i, j;
            var graphicList = graphic.target.graphics;

            for (i = 0; i < graphicList.length; i++)
            {
                var shapeList = graphicList[i].getShapes();
                var graphicType = graphicList[i].geometry.type;

                for (j = 0; j < shapeList.length; j++)
                    if (graphicType == "polygon")
                        shapeList[j].moveToBack();
                    else
                        shapeList[j].moveToFront();
            }
        },

        _setupDrawTool: function ()
        {
            this.map.disableDoubleClickZoom();
            this._inputPoints = [];
            this._measurePointGraphics = [];
            this._segmentTextSymbol = [];

            if (this._mouseClickMapHandler == null) {
                this._mouseClickMapHandler = on(this.map, "click", lang.hitch(this, "_measureAreaMouseClickHandler"));
            }
            if (this._doubleClickMapHandler == null) {
                this._doubleClickMapHandler = on(this.map, "dbl-click", lang.hitch(this, "_measureAreaDblClickHandler"));
            }

            this._drawTools = this.map.esrith.tools.draw.activate({
                type: "polygon",
            });
            
            array.forEach(this._drawTools, lang.hitch(this, function (drawTool) {
                //drawTool.setFillSymbol(this.config.defaultSymbol.polygon);
                //drawTool.setLineSymbol(this.config.defaultSymbol.line);
                //drawTool.setMarkerSymbol(this.config.defaultSymbol.point);
                drawTool.setFillSymbol(new SimpleFillSymbol(SimpleFillSymbol.STYLE_NULL));
                drawTool.setLineSymbol(new SimpleLineSymbol(SimpleLineSymbol.STYLE_NULL));
                drawTool.setMarkerSymbol(new SimpleMarkerSymbol(SimpleMarkerSymbol.STYLE_NULL));

            }));
        },

        _measureAreaMouseClickHandler: function (a)
        {
            console.log("_measureAreaMouseClickHandler", a);
            var c;
            this.map.snappingManager && (c = this.map.snappingManager._snappingPoint);
            c = c || a.mapPoint;

            this._inputPoints.push(c);
            this._currentStartPt = c;

            if (this._mouseMoveMapHandler == null) {
                this._mouseMoveMapHandler = on(this.map, "mouse-move", lang.hitch(this, "_measureAreaMouseMoveHandler"));
            }

            if (this._inputPoints.length < 2)
            {
                this._graphicsLayer.clear();
                this._segmentTextSymbol = [];

                this._tempGraphic = new Graphic();
                this._tempGraphic.setSymbol(this.config.defaultSymbol.line);
                this._tempGraphic.setGeometry(new Polyline(this.map.spatialReference));
                this._graphicsLayer.add(this._tempGraphic);

                this._manageResult(0);
            }
            else if (this._inputPoints.length == 2)
            {
                var segment = new Polyline(this.map.spatialReference);
                segment.addPath([this._inputPoints[0], this._inputPoints[1]]);
                if (this.config.isGeodesic == true)
                    segment = this._densifyGeometry(segment);

                var segmentGraphic = new Graphic();
                segmentGraphic.setSymbol(this.config.defaultSymbol.line);
                segmentGraphic.setGeometry(segment);
                this._graphicsLayer.add(segmentGraphic);

                this._startLineSegmentGraphic = segmentGraphic;
            }
            else 
            {
                if (this._startLineSegmentGraphic != null)
                    this._graphicsLayer.remove(this._startLineSegmentGraphic);
                this._startLineSegmentGraphic = null;

                if (this._polygonGraphic != null)
                {
                    this._graphicsLayer.remove(this._polygonGraphic);
                    //this._polylineGraphics.push(this._tempGraphic);
                    this._polygonGraphic = this._createPolygonGraphic(this._inputPoints);
                    this._graphicsLayer.add(this._polygonGraphic).getShape().moveToBack();
                    //this._measureGraphic = this._polygonGraphic;
                    //this._polylineGraphics.pop();
                }
                else
                {
                    this._polygonGraphic = this._createPolygonGraphic(this._inputPoints);
                    this._graphicsLayer.add(this._polygonGraphic).getShape().moveToBack();
                }

                this.calculateArea(this._polygonGraphic.geometry, this.unit);
            }

            if (this._inputPoints.length >= 2)
            {
                var segment = new Polyline(this.map.spatialReference);
                segment.addPath([this._inputPoints[this._inputPoints.length - 2], this._inputPoints[this._inputPoints.length - 1]]);
                if (this.config.isGeodesic == true)
                    segment = this._densifyGeometry(segment);

                if (this.config.area.showSegmentLabel)
                {
                    var segmentLength = parseFloat(this.calculateDistance(segment, this.segmentUnit.VALUE));

                    var param = {
                        geometry: segment,
                        unit: this.segmentUnit.VALUE,
                        shortUnit: this.segmentUnit.SHORTLABEL,
                        value: segmentLength,
                        text: number.format(segmentLength, { pattern: this.config.area.pattern }) + " " + this.segmentUnit.SHORTLABEL
                    };

                    var segmentTextSymbol = this.createTextSymbol(param);
                    this._segmentTextSymbol.push(segmentTextSymbol);
                    this._graphicsLayer.add(segmentTextSymbol);
                }
            }
        },

        _measureAreaMouseMoveHandler: function (a)
        {
            //console.log("_measureAreaMouseMoveHandler", a)
            var i;
            if (this._segmentTextSymbolOnMouseMove != null)
            {
                for (i = 0; i < this._segmentTextSymbolOnMouseMove.length; i++)
                    this._graphicsLayer.remove(this._segmentTextSymbolOnMouseMove[i]);
            }
            this._segmentTextSymbolOnMouseMove = [];
            
            var b;
            if (this._inputPoints.length > 0)
            {
                var segment = new Polyline(this.map.spatialReference), e;
                this.map.snappingManager && (e = this.map.snappingManager._snappingPoint);
                b = e || a.mapPoint;

                var path = [this._currentStartPt, b];
                segment.addPath(path);
                if (this.config.isGeodesic == true)
                    segment = this._densifyGeometry(segment);

                if (this.config.area.showSegmentLabel)
                {
                    var segmentLength = parseFloat(this.calculateDistance(segment, this.segmentUnit.VALUE));

                    var param = {
                        geometry: segment,
                        unit: this.segmentUnit.VALUE,
                        shortUnit: this.segmentUnit.SHORTLABEL,
                        value: segmentLength,
                        text: number.format(segmentLength, { pattern: this.config.area.pattern }) + " " + this.segmentUnit.SHORTLABEL
                    };

                    var segmentTextSymbol = this.createTextSymbol(param);
                    this._segmentTextSymbolOnMouseMove.push(segmentTextSymbol);
                    this._graphicsLayer.add(segmentTextSymbol);
                }

                if (this._inputPoints.length >= 2)
                {
                    var inputPoints = this._inputPoints.slice();

                    inputPoints.push(b);
                    var polygonGraphic = this._createPolygonGraphic(inputPoints);

                    this.calculateArea(polygonGraphic.geometry, this.unit)
                }

                this._tempGraphic.setGeometry(segment)
            }

            if (this._inputPoints.length > 1) {
                var segment = new Polyline(this.map.spatialReference);
                segment.addPath([b, this._inputPoints[0]]);
                if (this.config.isGeodesic == true)
                    segment = this._densifyGeometry(segment);
                
                if (this.config.area.showSegmentLabel)
                {
                    var segmentLength = parseFloat(this.calculateDistance(segment, this.segmentUnit.VALUE));

                    var param = {
                        geometry: segment,
                        unit: this.segmentUnit.VALUE,
                        shortUnit: this.segmentUnit.SHORTLABEL,
                        value: segmentLength,
                        text: number.format(segmentLength, { pattern: this.config.area.pattern }) + " " + this.segmentUnit.SHORTLABEL
                    };

                    var segmentTextSymbol = this.createTextSymbol(param);
                    this._segmentTextSymbolOnMouseMove.push(segmentTextSymbol);
                    this._graphicsLayer.add(segmentTextSymbol);
                }

                this._tempGraphic.setGeometry(this._tempGraphic.geometry.addPath(segment.paths[0]));
            }
        },

        _polygonGraphic: null,
        _measureAreaDblClickHandler: function (a)
        {
            console.log("_measureAreaDblClickHandler",a)
            if (this._inputPoints.length >= 2)
            {
                var i;
                if (this._segmentTextSymbolOnMouseMove != null)
                {
                    for (i = 0; i < this._segmentTextSymbolOnMouseMove.length; i++)
                        this._graphicsLayer.remove(this._segmentTextSymbolOnMouseMove[i]);
                }
                this._segmentTextSymbolOnMouseMove = [];

                if (this._mouseMoveMapHandler)
                {
                    this._mouseMoveMapHandler.remove();
                    this._mouseMoveMapHandler = null;
                }

                var c;
                this.map.snappingManager && (c = this.map.snappingManager._snappingPoint);
                c = c || a.mapPoint;

                if ("touch" === this.map.navigationManager.eventModel && sniff("ios")) {
                    //this._measureAreaMouseClickHandler(a);
                    if (this._mouseClickMapHandler != null) {
                        this._mouseClickMapHandler.remove();
                        setTimeout(lang.hitch(this, function () {
                            this._mouseClickMapHandler = on(this.map, "click", lang.hitch(this, "_measureAreaMouseClickHandler"));
                        }), 100);
                    }
                }

                var polygonGraphic = this._createPolygonGraphic(this._inputPoints);
                if (this._polygonGraphic == null)
                {
                    this._inputPoints = [];
                    this._setupDrawTool();
                    return;
                }
                else
                    this._graphicsLayer.remove(this._polygonGraphic);

                //this._polylineGraphics.push(this._tempGraphic);
                this._polygonGraphic = polygonGraphic;//this._generatePolygonFromPaths();
                this._graphicsLayer.add(this._polygonGraphic).getShape().moveToBack();

                if (this.config.area.showSegmentLabel)
                {
                    var segment = new Polyline(this.map.spatialReference);
                    segment.addPath([this._inputPoints[this._inputPoints.length - 1], this._inputPoints[0]]);
                    if (this.config.isGeodesic == true)
                        segment = this._densifyGeometry(segment);

                    var segmentLength = parseFloat(this.calculateDistance(segment, this.segmentUnit.VALUE));

                    var param = {
                        geometry: segment,
                        unit: this.segmentUnit.VALUE,
                        shortUnit: this.segmentUnit.SHORTLABEL,
                        value: segmentLength,
                        text: number.format(segmentLength, { pattern: this.config.area.pattern }) + " " + this.segmentUnit.SHORTLABEL
                    };

                    var segmentTextSymbol = this.createTextSymbol(param);
                    this._segmentTextSymbol.push(segmentTextSymbol);
                    this._graphicsLayer.add(segmentTextSymbol);
                }

                this._inputPoints = [];
                //this._polylineGraphics = [];
                this.calculateArea(polygonGraphic.geometry, this.unit);
            }
        },

        _createPolygonGraphic: function(pointList)
        {
            var polygon = new Polygon(this.map.spatialReference);
            var ring = [];
            for (var i = 0; i < pointList.length; i++)
            {
                ring.push([pointList[i].x, pointList[i].y])
            };

            ring.push([pointList[0].x, pointList[0].y]);
            polygon.addRing(ring);
            if (this.config.isGeodesic == true)
                polygon = this._densifyGeometry(polygon);

            polygon._measurementWidget_inputPoints = pointList;

            var polygonGraphic = new Graphic();
            polygonGraphic.setGeometry(polygon);
            polygonGraphic.setSymbol(this.config.defaultSymbol.polygon);

            return polygonGraphic;
        },

        //_generatePolygonFromPaths: function () {
        //    var d = [];
        //    array.forEach(this._polylineGraphics, lang.hitch(this, function (a) {
        //        array.forEach(a.geometry.paths, lang.hitch(this, function (a) {
        //            array.forEach(a, lang.hitch(this, function (a) {
        //                d.push(a)
        //            }));
        //        }))
        //    }));
        //    d.push(d[0]);
        //    var a = new Polygon(this.map.spatialReference);
        //    a.addRing(d);
        //    var a = this._densifyGeometry(a), b = new Graphic;
        //    b.setGeometry(a);
        //    b.setSymbol(this.config.defaultSymbol.polygon);
        //    this._measureGraphic = b;
        //    return b;
        //},

        calculateArea: function (geometry, unit) {
            //console.log("calculateArea", geometry, unit);
            var result = "";

            if (this._areaTextSymbolOnMouseMove != null)
                this._graphicsLayer.remove(this._areaTextSymbolOnMouseMove);
            this._areaTextSymbolOnMouseMove = null;

            try {
                if (this.config.isGeodesic == true) {
                    if (this.geodesicWkid.indexOf(geometry.spatialReference.wkid) >= 0) {
                        if (this.unit == "area-rai") {
                            result = geometryEngine.geodesicArea(geometryEngine.simplify(geometry), "square-meters");
                            result = this.calculateThaiArea(result);
                        } else {
                            result = geometryEngine.geodesicArea(geometryEngine.simplify(geometry), unit);
                        }
                    }
                    else {

                        if (this.unit == "area-rai") {
                            result = geometryEngine.planarArea(geometryEngine.simplify(geometry), "square-meters");
                            result = this.calculateThaiArea(result);
                        } else {
                            result = geometryEngine.planarArea(geometryEngine.simplify(geometry), unit);
                        }
                    }
                }
                else {
                    if (this.unit == "area-rai") {
                        result = geometryEngine.planarArea(geometryEngine.simplify(geometry), "square-meters");
                        result = this.calculateThaiArea(result);
                    } else {
                        result = geometryEngine.planarArea(geometryEngine.simplify(geometry), unit);
                    }
                }
            }
            catch (err) {
                console.log(err);
            }
            //this.createTextSymbol(geometryEngine.simplify(geometry), number.format(result, { pattern: this.area.pattern }) + " " + this.unitLabel);
            this._manageResult(result);

            param = {
                geometry: geometry,
                unit: this.unit,
                shortUnit: this.unitShort,
                value: result,
                text: this.unit == "area-rai" ? result : number.format(result, { pattern: this.config.area.pattern }) + " " + this.unitShort
            };

            this._areaTextSymbolOnMouseMove = this.createTextSymbol(param);
            this._graphicsLayer.add(this._areaTextSymbolOnMouseMove);

            return result;
        },

        calculateThaiArea: function (m2) {
            var result = "";
            var arrUnit = this.unitLabel.split(" ");
            var areaRai = new AreaRai();
            areaRai.set(m2, "m2");

            areaRai.label = {
                rai: arrUnit[0],
                ngan: arrUnit[1],
                wa: arrUnit[2]
            };

            areaRai.rai = number.format(areaRai.rai, { pattern: this.config.area.pattern });
            areaRai.ngan = number.format(areaRai.ngan, { pattern: this.config.area.pattern });
            areaRai.wa = number.format(areaRai.wa, { pattern: this.config.area.pattern });

            //rai: number.format(arrUnit[0], { pattern: this.pattern }),
            //ngan: number.format(arrUnit[1], { pattern: this.pattern }),
            //wa: number.format(arrUnit[2], { pattern: this.pattern })

            var result = lang.replace("{rai} {label.rai} {ngaan} {label.ngan} {wa} {label.wa}", areaRai);

            return result;
        },

        //_textsymbol: null,
        //createTextSymbol: function (geometry, text) {
        //	//console.log("createTextSymbol", geometry);
        //	if (this._textsymbol != null) {
        //		this._graphicsLayer.remove(this._textsymbol);
        //	}
        //	var point = geometry.getExtent().getCenter();
        //	var font = new Font("20px", Font.STYLE_NORMAL, Font.VARIANT_NORMAL, Font.WEIGHT_BOLDER);
        //	var symbolText = jsonUtils.fromJson(this.config.defaultSymbol.text.toJson());
        //	symbolText.setText(text);
        //	var labelPointGraphic = new Graphic(point, symbolText);
        //	//console.log("labelPointGraphic", labelPointGraphic);
        //	//console.log("labelPointGraphic   symbolText", symbolText);
        //	//console.log("labelPointGraphic   textSymbol", textSymbol);
        //	this._textsymbol = labelPointGraphic; 
        //	this._graphicsLayer.add(labelPointGraphic);
        //	// symbolText.setOffset();
        //},

        calculateDistance: function (geometry, unit)
        {
            var result = "";
            var multiplier = 1;

            if (unit == "wa")
            {
                unit = "meters";
                multiplier = 0.5;
            }
            try{
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

        _manageResult: function (result) {
            var divResult;
            if (this.unit == "area-rai") {
                divResult = domConstruct.create("div", { innerHTML: result });
            }
            else {
                //  console.log("_manageResult", result, number);
                divResult = domConstruct.create("div", {
                    innerHTML: number.format(result, { pattern: this.config.area.pattern }) + " " + this.unitLabel
                });
            }
            this.emit("calculation-completed", divResult);
        },

        getUnit: function () {
            if ("unit" !== this.unit) {
                return this.unit;
            }
        },

        _setUnitAttr: function (unit) {
            // this._set("unit", unit);
            this.unit = unit;
        },

        _densifyGeometry: function (d) {
            "Web Mercator" === this.map.cs && (d = webMercatorUtils.webMercatorToGeographic(d));
            d = "PCS" === this.map.cs ? d : geodesicUtils.geodesicDensify(d, 5E5); "Web Mercator" === this.map.cs && (d = webMercatorUtils.geographicToWebMercator(d));
            return d;
        },

        reset: function ()
        {
            this._polygonGraphic = null;
            this._polylineGraphics = [];
            this._inputPoints = [];
            this._segmentTextSymbol = [];
            this.map.esrith.tools.deactivate();

            this.map.enableDoubleClickZoom();

            if (this._mouseClickMapHandler != null) {
                this._mouseClickMapHandler.remove();
                this._mouseClickMapHandler = null
            }
            if (this._doubleClickMapHandler != null) {
                this._doubleClickMapHandler.remove();
                this._doubleClickMapHandler = null;
            }

            if (this._mouseMoveMapHandler != null) {
                this._mouseMoveMapHandler.remove();
                this._mouseMoveMapHandler = null;
            }
              
            if (this._graphicsLayer != null) {
                this._graphicsLayer.clear();
                this.map.removeLayer(this._graphicsLayer);
            }
            this._graphicslayer = null;
        }
    });


    lang.mixin(wgDeclare, wgConstant);


    return wgDeclare;
});