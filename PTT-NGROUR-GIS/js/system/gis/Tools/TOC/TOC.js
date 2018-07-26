define([
    "core/_WidgetBase",
    "manager/ConfigManager",

    "dojo/text!./TOC.html",
    "xstyle/css!./TOC.css",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/dom-class",
	"dojo/dom-construct",

    'esri/dijit/LayerList',
    "esrith/amos/form/Select",
    "esrith/amos/form/Button"
], function (
    _WidgetBase,
    ConfigManager,
    template,
    xstyle,

    declare,
    lang,
    array,
    on,
    domClass,
    domConstruct,

    LayerList

    ) {
    return declare([
        _WidgetBase
    ], {
        baseClass: "toc",
        templateString: template,
        markerPoint: null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
            this.configures = ConfigManager.getInstance().getAllConfig();
        },
        postCreate: function () {
            this.inherited(arguments);

            this._init();
            this._initEvent()


            this.own(

            );

        },

        _init: function () {

            let layerArrLists = this._getLayerFromMap();
            const layerList = new LayerList({
                map: this.map,
                layers: layerArrLists,
                showSubLayers: true,
                showLegend: true
            });
            layerList.startup();
            domConstruct.place(layerList.domNode, this.divLayerList);
        },

        _initEvent: function () {

        },

        _getLayerFromMap: function () {
            let layerList = [];
            let _layerFromConfig = this.configures.mapConfig.layers.filter(function (l) { return l.option.type !== 'tiled' });
            this.map.layerIds.forEach(lang.hitch(this, function (id) {
                _layerFromConfig.forEach(lang.hitch(this, function (lconfig) {
                    if (lconfig.option.id == id) {
                        layerList.push({
                            id: id,
                            title: lconfig.option.name,
                            layer: this.map.getLayer(id),
                            showSubLayers: true
                        });
                    }
                }));
            }));
            return layerList;
        }

    });
});