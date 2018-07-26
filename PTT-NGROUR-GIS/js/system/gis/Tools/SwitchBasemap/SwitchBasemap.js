define([
    "core/_WidgetBase",

    "dojo/text!./SwitchBasemap.html",
    "xstyle/css!./SwitchBasemap.css",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/dom-class",
	"dojo/dom-construct",

    "esri/dijit/BasemapGallery",
    "esri/dijit/Basemap",
    "esri/dijit/BasemapLayer",

    "manager/ConfigManager"

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

    BasemapGallery,
    Basemap,
    BasemapLayer,

    ConfigManager
    ) {
    return declare([
        _WidgetBase
    ], {
        baseClass: "switchbasemap",
        templateString: template,

        configures: null,
        _defaultBasemap: null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);

            this.configures = ConfigManager.getInstance().getAllConfig();

            this._init();
        },

        _init: function () {
            this._createBasemapGallery();
            this._initEvent();
        },

        _initEvent: function () {
            this.own(

            );
        },

        _createBasemapGallery: function () {

            this._basemapGallery = new BasemapGallery({
                map: this.map,
                showArcGISBasemaps: false,
                basemaps: this._genBasemapList()
                //, google: {
                //     id: 'google-map',
                //     apiOptions: {}
                // }
            }, this.basemapContainerNode);
            this._basemapGallery.startup();

            if (this._defaultBasemap) {
                setTimeout(lang.hitch(this, function () {
                    this._basemapGallery.select(this._defaultBasemap);
                }, 3000));
            }
        },

        _genBasemapList: function () {
            var basemaps = [];
            var isFirst = true;

            array.forEach(this.configures.mapConfig.layers, lang.hitch(this, function (item) {
                if (item.option.hasOwnProperty("toggle")) {
                    if (item.option.toggle) {
                        var thumbnail = "js/system/gis/resources/images/basemap/" + item.option.id + ".png";
                        basemaps.push(new Basemap({
                            id: item.option.id,
                            layers: [
                                new BasemapLayer({
                                    url: item.url
                                })
                            ],
                            title: item.option.name,
                            thumbnailUrl: thumbnail
                        }));

                        if (isFirst) {
                            this._defaultBasemap = item.option.id;
                            isFirst = false;
                        }
                    }
                }
            }));
            return basemaps;
        },

        select: function (id) {
            this._basemapGallery.select(id);
        }
    });
});