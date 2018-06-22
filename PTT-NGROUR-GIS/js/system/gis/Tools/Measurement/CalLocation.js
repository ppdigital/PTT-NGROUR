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

	"require",
    "esri/toolbars/draw",
    "esri/layers/GraphicsLayer",
    "esri/symbols/PictureMarkerSymbol",
    "esri/graphic",
    "esri/geometry/Point",
    "esri/SpatialReference",

    "esri/symbols/jsonUtils"

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

	require,
    Draw,
    GraphicsLayer,
    PictureMarkerSymbol,
    Graphic,
    Point,
    SpatialReference,

    jsonUtils

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
        //ERROR_REQUIRED: "required"
    };

    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
        _WidgetBase,
        _ProjectionMixin
    ], {
        templateString: "<div></div>",
        className: "amos-Measurement",
        map: null,
        drawObj: null,
        mouseClickGeo: null,
        mouseMoveGeo: null,
        _evtMouseMove: null,
        _unit: null,
        _drawTools: null,
        _graphicsLayer: null,
        _mobile:false,

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

            if (/Android|webOS|iPhone|iPad|iPod|BlackBerry|IEMobile|Opera Mini/i.test(navigator.userAgent)) {
                _mobile = true;
            } else {
                _mobile = false;
            }
        },
        _labelX: null,
        _labelY: null,
        activateTool: function (item) {
            //console.log(item);
            this._unit = item["VALUE"];
            this._labelX = item["LABEL_X"];
            this._labelY = item["LABEL_Y"];
            this._drawTools = this.map.esrith.tools.draw.activate({
                type: "point",
                callback: lang.hitch(this, "_drawTools_drawComplete"),
            });
            //console.log("drawtools: ", this._drawTools);

            if (this._evtMouseMove == null) {
                this._evtMouseMove = on(this.map, "mouse-move", lang.hitch(this, "_map_mouseMove"));
            }
            //call for default result
            this.calCompleted();


            this._graphicsLayer = this.map.getLayer("gEdit_MeasurementLocationLayer");
            if (this._graphicsLayer == null) {
                this._graphicsLayer = new GraphicsLayer({
                    id: "gEdit_MeasurementLocationLayer"
                });
                this.map.addLayer(this._graphicsLayer);
            }
            //this._graphicsLayer.clear();
        },

        calCompleted: function () {
            var tempMouseMove = null;
            var tempMouseClick = null;
            var divResult = domConstruct.create("div", {class: "divResultContainer"});

            if (this._labelX && this._labelY) {
                var divTitle = domConstruct.create("div", { className: "" }, divResult);
                domConstruct.create("div", { class: "divIcon" }, divTitle);
                domConstruct.create("div", { class: "divTitle", innerHTML: this._labelX }, divTitle);
                domConstruct.create("div", { class: "divTitle", innerHTML: this._labelY }, divTitle);
            }
            switch (this._unit) {
                case "UTM": {
                    if (_mobile==false) {
                        if (this.mouseMoveGeo) {
                            tempMouseMove = this.project.transform(this.mouseMoveGeo, 32647);
                            var divMove = domConstruct.create("div", { className: "" }, divResult); //ซ่อนอันนี้
                            domConstruct.create("div", { className: "iconMove divIcon" }, divMove);
                            domConstruct.create("div", { className: "divText col-x-y", innerHTML: tempMouseMove.x.toFixed(6) }, divMove);
                            domConstruct.create("div", { className: "divText col-x-y", innerHTML: tempMouseMove.y.toFixed(6) }, divMove);
                        } else {
                            var divMove = domConstruct.create("div", { className: "" }, divResult);
                            domConstruct.create("div", { className: "iconMove divIcon" }, divMove);
                            domConstruct.create("div", { className: "divText col-x-y", innerHTML: " - " }, divMove);
                            domConstruct.create("div", { className: "divText col-x-y", innerHTML: " - " }, divMove);
                        }
                    }

                    if (this.mouseClickGeo) {
                        tempMouseClick = this.project.transform(this.mouseClickGeo, 32647);
                        var divClick = domConstruct.create("div", { className: "" }, divResult);
                        domConstruct.create("div", { className: "iconClick divIcon" }, divClick);
                        domConstruct.create("div", { className: "divText col-x-y", innerHTML: tempMouseClick.x.toFixed(6) }, divClick);
                        domConstruct.create("div", { className: "divText col-x-y", innerHTML: tempMouseClick.y.toFixed(6) }, divClick);
                    } else {
                        var divClick = domConstruct.create("div", { className: "" }, divResult);
                        domConstruct.create("div", { className: "iconClick divIcon" }, divClick);
                        domConstruct.create("div", { className: "divText col-x-y", innerHTML: " - " }, divClick);
                        domConstruct.create("div", { className: "divText col-x-y", innerHTML: " - " }, divClick);
                    }
                    break;
                }
                case "WebMercator": {
                    if (_mobile == false) {
                        if (this.mouseMoveGeo) {
                            tempMouseMove = this.project.transform(this.mouseMoveGeo, 3857);
                            var divMove = domConstruct.create("div", { className: "" }, divResult);
                            domConstruct.create("div", { className: "iconMove divIcon" }, divMove);
                            domConstruct.create("div", { className: "divText col-x-y", innerHTML: tempMouseMove.x.toFixed(6) }, divMove);
                            domConstruct.create("div", { className: "divText col-x-y", innerHTML: tempMouseMove.y.toFixed(6) }, divMove);
                        } else {
                            var divMove = domConstruct.create("div", { className: "" }, divResult);
                            domConstruct.create("div", { className: "iconMove divIcon" }, divMove);
                            domConstruct.create("div", { className: "divText col-x-y", innerHTML: " - " }, divMove);
                            domConstruct.create("div", { className: "divText col-x-y", innerHTML: " - " }, divMove);
                        }
                    }
                    if (this.mouseClickGeo) {
                        tempMouseClick = this.project.transform(this.mouseClickGeo, 3857);
                        var divClick = domConstruct.create("div", { className: "" }, divResult);
                        domConstruct.create("div", { className: "iconClick divIcon" }, divClick);
                        domConstruct.create("div", { className: "divText col-x-y", innerHTML: tempMouseClick.x.toFixed(6) }, divClick);
                        domConstruct.create("div", { className: "divText col-x-y", innerHTML: tempMouseClick.y.toFixed(6) }, divClick);
                    } else {
                        var divClick = domConstruct.create("div", { className: "" }, divResult);
                        domConstruct.create("div", { className: "iconClick divIcon" }, divClick);
                        domConstruct.create("div", { className: "divText col-x-y", innerHTML: " - " }, divClick);
                        domConstruct.create("div", { className: "divText col-x-y", innerHTML: " - " }, divClick);
                    }
                    break;
                }
                case "LATLONG": {
                    if (_mobile == false) {
                        if (this.mouseMoveGeo) {
                            tempMouseMove = this.project.transform(this.mouseMoveGeo, 4326);
                            var divMove = domConstruct.create("div", { className: "" }, divResult);
                            domConstruct.create("div", { className: "iconMove divIcon" }, divMove);
                            domConstruct.create("div", { className: "divText col-x-y", innerHTML: tempMouseMove.x.toFixed(6) }, divMove);
                            domConstruct.create("div", { className: "divText col-x-y", innerHTML: tempMouseMove.y.toFixed(6) }, divMove);
                        } else {
                            var divMove = domConstruct.create("div", { className: "" }, divResult);
                            domConstruct.create("div", { className: "iconMove divIcon" }, divMove);
                            domConstruct.create("div", { className: "divText col-x-y", innerHTML: " - " }, divMove);
                            domConstruct.create("div", { className: "divText col-x-y", innerHTML: " - " }, divMove);
                        }
                    }
                    if (this.mouseClickGeo) {
                        tempMouseClick = this.project.transform(this.mouseClickGeo, 4326);
                        var divClick = domConstruct.create("div", { className: "" }, divResult);
                        domConstruct.create("div", { className: "iconClick divIcon" }, divClick);
                        domConstruct.create("div", { className: "divText col-x-y", innerHTML: tempMouseClick.x.toFixed(6) }, divClick);
                        domConstruct.create("div", { className: "divText col-x-y", innerHTML: tempMouseClick.y.toFixed(6) }, divClick);
                    } else {
                        var divClick = domConstruct.create("div", { className: "" }, divResult);
                        domConstruct.create("div", { className: "iconClick divIcon" }, divClick);
                        domConstruct.create("div", { className: "divText col-x-y", innerHTML: " - " }, divClick);
                        domConstruct.create("div", { className: "divText col-x-y", innerHTML: " - " }, divClick);
                    }
                    break;
                }
                case "MGRS": {
                    if (_mobile == false) {
                        if (this.mouseMoveGeo) {
                            tempMouseMove = this.project.toMGRS(this.mouseMoveGeo);
                            var divMove = domConstruct.create("div", { className: "" }, divResult);
                            domConstruct.create("div", { className: "iconMove divIcon" }, divMove);
                            domConstruct.create("div", { className: "divText", innerHTML: tempMouseMove }, divMove);
                        } else {
                            var divMove = domConstruct.create("div", { className: "" }, divResult);
                            domConstruct.create("div", { className: "iconMove divIcon" }, divMove);
                            domConstruct.create("div", { className: "divText", innerHTML: " - " }, divMove);
                        }
                    }
                    if (this.mouseClickGeo) {
                        tempMouseClick = this.project.toMGRS(this.mouseClickGeo);
                        var divClick = domConstruct.create("div", { className: "" }, divResult);
                        domConstruct.create("div", { className: "iconClick divIcon" }, divClick);
                        domConstruct.create("div", { className: "divText", innerHTML: tempMouseClick }, divClick);
                    } else {
                        var divClick = domConstruct.create("div", { className: "" }, divResult);
                        domConstruct.create("div", { className: "iconClick divIcon" }, divClick);
                        domConstruct.create("div", { className: "divText", innerHTML: " - " }, divClick);
                    }
                    break;
                }

                case "DMS": {
                    if (_mobile == false) {
                        if (this.mouseMoveGeo) {
                            tempMouseMove = this.project.transform(this.mouseMoveGeo, 4326);
                            tempMouseMove = this._DDtoDMS(tempMouseMove);
                            var divMove = domConstruct.create("div", { className: "" }, divResult);
                            domConstruct.create("div", { className: "iconMove divIcon" }, divMove);
                            domConstruct.create("div", { className: "divText", innerHTML: tempMouseMove.x + " , " + tempMouseMove.y }, divMove);
                        } else {
                            var divMove = domConstruct.create("div", { className: "" }, divResult);
                            domConstruct.create("div", { className: "iconMove divIcon" }, divMove);
                            domConstruct.create("div", { className: "divText", innerHTML: " - " }, divMove);
                        }
                    }

                    if (this.mouseClickGeo) {
                        tempMouseClick = this.project.transform(this.mouseClickGeo, 4326);
                        tempMouseClick = this._DDtoDMS(tempMouseClick);
                        var divClick = domConstruct.create("div", { className: "" }, divResult);
                        domConstruct.create("div", { className: "iconClick divIcon" }, divClick);
                        domConstruct.create("div", { className: "divText", innerHTML: tempMouseClick.x + " , " + tempMouseClick.y }, divClick);
                    } else {
                        var divClick = domConstruct.create("div", { className: "" }, divResult);
                        domConstruct.create("div", { className: "iconClick divIcon" }, divClick);
                        domConstruct.create("div", { className: "divText", innerHTML: " - " }, divClick);
                    }

                    //console.log("this.mouseClickGeo", this.mouseClickGeo);
                    //console.log("this.tempMouseClick", tempMouseClick);


                    break;
                }
            }

            var obj = {
                mouseMove: tempMouseMove,
                mouseClick: tempMouseMove
            };

            this.emit("calculation-completed", divResult, obj);
        },

        reset: function () {
            this.map.esrith.tools.deactivate();

            this.mouseMoveGeo = null;
            this.mouseClickGeo = null;

            if (this._evtMouseMove) {
                this._evtMouseMove.remove();
                this._evtMouseMove = null;
            }

            if (this._graphicsLayer) {
                this._graphicsLayer.clear();
                this.map.removeLayer(this._graphicsLayer);
                this._graphicsLayer = null;
            }
        },

        _drawTools_drawComplete: function (obj) {
            this.mouseClickGeo = obj.geometry;
            this._graphicsLayer.clear();
            var graphic = new Graphic(obj.geometry, this.config.location.symbols.point);
            this._graphicsLayer.add(graphic);
            this.calCompleted();
        },

        _map_mouseMove: function (evt) {
            this.mouseMoveGeo = evt.mapPoint;
            this.calCompleted();
        },

        _DDtoDMS: function (value) {
            var d = parseInt(Math.floor(value.x.toString()));
            var dDouble = parseFloat(Math.floor(value.x).toString())
            var m = parseInt(Math.floor(((value.x - dDouble) * 60)).toString());
            var mDouble = parseFloat(((value.x - dDouble) * 60).toString());
            var s = parseInt((((parseFloat(((value.x - dDouble) * 60).toString())) - m) * 60).toString());
            var sString = "";

            if (s.toString().length == 1) {
                sString += "0" + s;
            }
            else {
                sString = s.toString();
            }
            var result = d + "° " + m + "' " + sString + "\" E";
            value.x = result;
            /////////////////////////////////////
            var d = parseInt(Math.floor(value.y.toString()));
            var dDouble = parseFloat(Math.floor(value.y).toString())
            var m = parseInt(Math.floor(((value.y - dDouble) * 60)).toString());
            var mDouble = parseFloat(((value.y - dDouble) * 60).toString());
            var s = parseInt((((parseFloat(((value.y - dDouble) * 60).toString())) - m) * 60).toString());
            var sString = "";

            if (s.toString().length == 1) {
                sString += "0" + s;
            }
            else {
                sString = s.toString();
            }
            var result = d + "° " + m + "' " + sString + "\" N";
            value.y = result;

            return value;

        }

    });


    lang.mixin(wgDeclare, wgConstant);


    return wgDeclare;
});