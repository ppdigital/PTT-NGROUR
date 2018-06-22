/**
 * @description 
 * -
 * @class Map
 * @constructor
 */
/**
 * @method
 * @constructor
 * @param {Node|String} divId map's element
 * @param {config/map} param
*/
define([
    "core/_ClassBase",
    "core/_WidgetBase",

    "dojo/_base/declare",
    "require",
    "esrith/amos/map/Map",
    "dojo/Evented",

    "dojo/i18n!esri/nls/jsapi",
    "esri/geometry/jsonUtils",
    "esri/symbols/jsonUtils",
    "esri/graphic",
    "esri/toolbars/edit",
    "esri/toolbars/draw",
    "esri/toolbars/navigation",
    "esri/request",
    "esri/graphicsUtils",
    "esri/layers/FeatureLayer",

    "dojo/store/Memory",
    "dojo/Deferred",
    "dojo/promise/all",
    "dojo/json",
    "dojo/topic",
    "dojo/on",
    "dojo/_base/lang",
    "dojo/_base/array"
], function (
    _ClassBase,
    _WidgetBase,

    declare,
    require,
    Map,
    Evented,

    bundle,
    gJsonUtils,
    sJsonUtils,
    Graphic,
    Edit,
    Draw,
    Navigation,
    esriRequest,
    graphicsUtils,
    FeatureLayer,

    Memory,
    Deferred,
    all,
    JSON,
    topic,
    on,
    lang,
    array
    ) {


    var instance = null;
    var wgConstant = {
        creates: new Memory({ data: [], idProperty: "mapId" }),
        getInstance: function (systemId) {
            if (arguments.length == 0) {
                return wgDeclare.creates.data[0];
            }
            return wgDeclare.creates.get(systemId);
        }
    };

    var wgDeclare = declare([
        Evented
    ], {

        map: null,


        _map: null,
        _layers: null,
        _layersAddResultHandler: null,
        _loadHandler: null,

        _mapConfig: null,

        mapId: "",


        _typeMapper: {
            "tiled": "esri/layers/ArcGISTiledMapServiceLayer",
            "dynamic": "esri/layers/ArcGISDynamicMapServiceLayer",
            "feature": "esri/layers/FeatureLayer",
            "bing": "esri/virtualearth/VETiledLayer"
        },

        constructor: function (param, divId) {
            param = param || {};
            //param.configures = param.configures || {};
            //console.log("map param=", param);
            this._mapConfig = param;
            console.log("this._mapConfig", this._mapConfig);
            this.mapId = param.mapId;

            if (this._mapConfig.option.extent != null) {
                this._mapConfig.option.extent = gJsonUtils.fromJson(this._mapConfig.option.extent);
            }
            var deferredList = [];
            var deferred = null;
            this._map = new Map(divId, this._mapConfig.option);
            this._layers = new Array();

            array.forEach(this._mapConfig.layers, lang.hitch(this, function (layerConfig) {
                if (layerConfig.option.addMap) {
                    deferredList.push(this._createMapServiceLayer(layerConfig));
                }
            }));
            array.forEach(this._mapConfig.features, lang.hitch(this, function (layerConfig) {
                layerConfig.option.type = "feature";
                deferredList.push(this._createMapServiceLayer(layerConfig));
            }));

            all(deferredList).then(lang.hitch(this, function (responses) {
                this._layers = responses;
                this._map.addLayers(this._layers);
            }), lang.hitch(this, function (err) {
                console.error(err);
            }));
            this._loadHandler = on(this._map, "load", lang.hitch(this, function (evt) {
                try {
                    _map = evt.map;
                    this._loadHandler.remove();
                    this._loadHandler = null;
                    this._initMap(evt.map);
                    this.emit("load", evt);

                } catch (err) { console.error(err); }
            }));
            wgConstant.creates = new Memory({ data: [], idProperty: "mapId" });
            wgConstant.creates.add(lang.mixin(this._map, {
                mapId: this.mapId
            }));
        },
        _createMapServiceLayer: function (mapServiceConfig) {
            var deferred = new Deferred();
            require([
                this._typeMapper[mapServiceConfig.option.type]
            ], lang.hitch(this, function (
                MapServiceLayer
            ) {
                try {
                    var requireProperty = mapServiceConfig.url || mapServiceConfig.featureCollectionObject;
                    if (!requireProperty) {
                        throw new Error("_createMapServiceLayer Error : ", JSON.stringify(mapServiceConfig));
                    }
                    var mapServiceLayer = new MapServiceLayer(mapServiceConfig.url || mapServiceConfig.featureCollectionObject, mapServiceConfig.option);
                    deferred.resolve(mapServiceLayer);
                } catch (err) {
                    deferred.reject(err);
                }
            }));
            return deferred.promise;
        },
        _initMap: function (map) {
            _ClassBase.prototype.map = map;
            _WidgetBase.prototype.map = map;
            this._disableFunction(map);
            this._layersAddResultHandler = on(map, "layers-add-result", lang.hitch(this, function (_map, evt) {
                this._layersAddResultHandler.remove();
                this._layersAddResultHandler = null;
                array.forEach(evt.layers, lang.hitch(this, function (layer) {
                    if (layer.success != true) {
                        console.error("layers-add-result", layer.error);
                    }
                }));
                this.emit("load-completed", _map);
            }, map));
        },
        _disableFunction: function (map) {
            map.disableKeyboardNavigation();
        }
    });

    lang.mixin(wgDeclare, wgConstant);

    if (appServer.isDebug == "1") {
        window._mapManager = wgDeclare;
    }

    return wgDeclare;
});