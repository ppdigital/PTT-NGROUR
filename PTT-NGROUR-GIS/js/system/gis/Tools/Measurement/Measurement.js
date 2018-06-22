/*
AMOS3.0 - Measurement
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
 * Measurement control is evt normal text field for user to enter data. Custom icons can be add for extended functionalities.
 * @class Measurement
 * @constructor
 * @uses _WidgetBase
 * @uses _BaseClassMixin
 */
/**
 * @description 
 * Create evt new Measurement widget.
 * @method
 * @constructor
 * @param {Object} [params] -
 * @param {Node|String} [srcNodeRef] -
 */
define([
    "core/_WidgetBase",

	"dojo/text!./Measurement.html",
	"xstyle/css!./Measurement.css",

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
	"dojo/_base/lang",
	"dojo/_base/declare",

	"esri/graphic",
	"esri/layers/GraphicsLayer",
	"esri/geometry/Polyline",
	"esri/geometry/Polygon",
	"esri/geometry/geodesicUtils",
	"esri/geometry/webMercatorUtils",

	"esrith/amos/form/Button",
	"esrith/amos/form/ImageButton",
	"esrith/amos/form/Select",
	"esrith/amos/core/DataStore",

	"./config",

	"require"

], function (

	_WidgetBase,

	template,
	measurementCss,

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
	lang,
	declare,

	Graphic,
	GraphicsLayer,
	Polyline,
	Polygon,
	geodesicUtils,
	webMercatorUtils,

	Button,
	ImageButton,
	FilteringSelect,
	DataStore,

	defaultConfig,
	require

	) {


    var wgConstant = {
        "AREA": {
            "acres": "Acres"
        },

    };

    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
		_WidgetBase,
    ], {
        baseClass: "amos-Measurement",
        templateString: template,

        defaultConfig: defaultConfig,
        map: null,

        _activateTool: null,
        _area: null,
        _distance: null,
        _location: null,
        _angle: null,
        _freehandAngle: null,

        subWidgets: {
            'CalArea': "system/gis/Tools/Measurement/CalArea",
            'CalDistance': "system/gis/Tools/Measurement/CalDistance",
            'CalLocation': "system/gis/Tools/Measurement/CalLocation",
            //'CalAngle': "widgets/Tools/Measurement/CalAngle",
            //'CalFreehandAngle': 'widgets/Tools/Measurement/CalFreehandAngle'
        },

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },

        postCreate: function () {
            this.inherited(arguments);

            this.own(
            on(this.btnArea, "click", lang.hitch(this, "_onMeasurementTool_Click", "area")),
            on(this.btnDistance, "click", lang.hitch(this, "_onMeasurementTool_Click", "distance")),
            on(this.btnLocation, "click", lang.hitch(this, "_onMeasurementTool_Click", "location")),
            on(this.btnAngle, "click", lang.hitch(this, "_onMeasurementTool_Click", "angle")),
            on(this.btnFreehandAngle, "click", lang.hitch(this, "_onMeasurementTool_Click", "freehandAngle")),
            on(this.fltUnit, "change", lang.hitch(this, "_onFltUnit_Change")),
            this.listen('call-stop-measurement', lang.hitch(this, function () {
                console.log("call-stop-measurement");
                this.reset();
            }))
            );
        },

        startup: function () {
            this.inherited(arguments);
        },

        _onMeasurementTool_Click: function (tool) {
            domClass.remove(this.map.container, "move");
            this.call('call-stop-buffer-click');
            this.call('call-stop-identify');
            this.subWidgets.then(lang.hitch(this, function (widgetClass) {
                //console.log("_onMeasurementTool_Click: ", tool, this._activateTool);

                if (this._activateTool != tool) {
                    //activate
                    this.reset();
                    this._activateTool = tool;
                }
                else {
                    //deactivate
                    this._activateTool = null;
                    this.reset();
                    this.call('call-restart-identify');
                    return;
                }

                switch (tool) {
                    case "area":
                        this.btnArea.activate();
                        if (this._area == null) {
                            this._area = new widgetClass.CalArea({ 'config': this.config, 'map': this.map });
                            on(this._area, "calculation-completed", lang.hitch(this, "showResult", "area"));
                        }
                        this.fltUnit.bind(new DataStore(this.config.area.units));
                        this.fltUnit.set("value", this.config.area.units[0].VALUE);
                        break;

                    case "distance":
                        this.btnDistance.activate();
                        if (this._distance == null) {
                            this._distance = new widgetClass.CalDistance({ 'config': this.config, 'map': this.map });
                            on(this._distance, "calculation-completed", lang.hitch(this, "showResult", "distance"));
                        }
                        this.fltUnit.bind(new DataStore(this.config.distance.units));
                        this.fltUnit.set("value", this.config.distance.units[0].VALUE);
                        break;

                    case "location":
                        this.btnLocation.activate();
                        if (this._location == null) {
                            this._location = new widgetClass.CalLocation({ 'config': this.config, 'map': this.map });
                            on(this._location, "calculation-completed", lang.hitch(this, "showResult", "location"));
                        }
                        this.fltUnit.bind(new DataStore(this.config.location.units));
                        this.fltUnit.set("value", this.config.location.units[0].VALUE);
                        break;

                    case "angle":
                        this.btnAngle.activate();
                        if (this._angle == null) {
                            this._angle = new widgetClass.CalAngle({ 'config': this.config, 'map': this.map });
                            on(this._angle, "calculation-completed", lang.hitch(this, "showResult", "angle"));
                        }
                        this.fltUnit.bind(new DataStore(this.config.angle.units));
                        this.fltUnit.set("value", this.config.angle.units[0].VALUE);
                        break;

                    case "freehandAngle":
                        this.btnFreehandAngle.activate();
                        if (this._freehandAngle == null) {
                            this._freehandAngle = new widgetClass.CalFreehandAngle({ 'config': this.config, 'map': this.map });
                            on(this._freehandAngle, "calculation-completed", lang.hitch(this, "showResult", "freehandAngle"));
                        }
                        this.fltUnit.bind(new DataStore(this.config.freehandAngle.units));
                        this.fltUnit.set("value", this.config.freehandAngle.units[0].VALUE);
                        break;
                }
            }));
        },

        showResult: function (tool, result) {
            domConstruct.empty(this.divResult);
            this.divResult.appendChild(result);
        },

        reset: function () {
            domConstruct.empty(this.divResult);
            this.fltUnit.unbind();

            if (this._activateTool) {
                this._activateTool = null;
            }

            if (this._area) {
                this._area.reset();
            }
            if (this._distance) {
                this._distance.reset();
            }
            if (this._location) {
                this._location.reset();
            }
            if (this._angle) {
                this._angle.reset();
            }
            if (this._freehandAngle) {
                this._freehandAngle.reset();
            }

            this.btnArea.deactivate();
            this.btnDistance.deactivate();
            this.btnLocation.deactivate();
            this.btnAngle.deactivate();
            this.btnFreehandAngle.deactivate();
        },

        _onFltUnit_Change: function () {
            //console.log("this.fltUnit", this.fltUnit.get("value"), this.fltUnit.get("displayedValue"), this._activateTool);
            switch (this._activateTool) {
                case "area":
                    this._area.activateTool(this.fltUnit.get("item"));
                    break;
                case "distance":
                    this._distance.activateTool(this.fltUnit.get("item"));
                    break;
                case "location":
                    this._location.activateTool(this.fltUnit.get("item"));
                    break;
                case "angle":
                    this._angle.activateTool(this.fltUnit.get("value"), this.fltUnit.get("displayedValue"));
                    break;
                case "freehandAngle":
                    this._freehandAngle.activateTool(this.fltUnit.get("value"), this.fltUnit.get("displayedValue"));
                    break;
            }
        },

        destroy: function () {
            this.reset();
        },

    });
    return wgDeclare;
});