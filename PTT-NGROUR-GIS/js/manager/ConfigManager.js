define([
    "core/_ClassBase",
    "core/_WidgetBase",
    "config/map",
    "config/system",

    "dojo/store/Memory",
    "dojo/promise/all",
    "dojo/Deferred",
    "dojo/on",
    "dojo/_base/array",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    _ClassBase,
    _WidgetBase,
    mapConfig,
    systemConfig,

    Memory,
    all,
    Deferred,
    on,
    array,
    lang,
    declare,
    require
    ) {

    var instance = null;
    var wgConstant = {};

    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
        _ClassBase
    ], {

        _configures: null,

        constructor: function (jsonConfigures) {
            //jsonConfigures.data  -> General Config { .... }
            //jsonConfigures.data2  -> Map Services Config { SERV_ID, NAME, URL, ADD_MAP, ADD_TOC }
            //jsonConfigures.data3  -> Layers Config -> { LYR_ID, NAME, LYR_INDEX }
            //jsonConfigures.data4  -> System Config -> { SYS_ID, NAME }
            //jsonConfigures.data5  -> Function Config -> { SYS_ID, FN_ID, FN_NAME }
        },

        _init: function (loginResult) {
            this._configures = {};
            lang.mixin(this._configures, this._getGeneralConfig(loginResult));
            lang.mixin(this._configures, this._getMapConfig(loginResult));
            lang.mixin(this._configures, this._getLayerConfig(loginResult));
            lang.mixin(this._configures, this._getSystemConfig(loginResult));
            lang.mixin(this._configures, this._getFunctionConfig(loginResult));
        },
        _getGeneralConfig: function (response) {
            var data = response.data;
            return { generalConfig: data[0] };
        },
        _getMenu: function () {
            var s = window.location.search;
            var menu = null;
            if (s != null && s.length > 0) {
                s = s.substring(1);
                var arrParam = s.split("&");
                if (arrParam != null && arrParam.length > 0) {
                    array.forEach(arrParam, lang.hitch(this,
                        function (p) {
                            var pair = p.split("=");
                            if (pair.length == 2) {
                                if (pair[0] == "m") {
                                    menu = pair[1];
                                }
                            }
                        }))
                }
            }

            return menu;
        },
        _getMapConfig: function (response) {
            //var data = response.data2;
            try {
                //nannie add (3-7-2018) : change mapconfig to get from database
                //query map config from database
                this.reqSP("GIS_Q_CONFIG", { MENU: this._getMenu() }).query(lang.hitch(this, function (response) {

                    var mapLayers = array.map(response.data, lang.hitch(this, function (d) {
                        var lyrConfig = {};
                        var lyrProps = array.filter(response.data2, lang.hitch(this, function (lyr) { return lyr.SERVICE_ID == d["SERVICE_ID"].toString(); }));
                        array.forEach(lyrProps, lang.hitch(this, function (d) {
                            lyrConfig[d["LAYER_ID"]] = d["LAYER_INDEX"];
                        }));

                        return {
                            url: (d["SERVICE_URL"].indexOf("http") == 0 ? d["SERVICE_URL"] : (window.location.protocol + d["SERVICE_URL"])),
                            option: {
                                id: d["SERVICE_ID"].toString(),
                                name: d["SERVICE_NAME"],
                                type: d["SERVICE_TYPE"].toLowerCase(),
                                visible: d["SERVICE_VISIBLE"] === 0 ? false : true,
                                addTOC: d["ADD_TOC"] ? true : false,
                                addMap: d["ADD_MAP"] ? true : false,
                                systemId: d["SYSTEM_ID"],
                                index: d["INDEX_NO"],
                                toggle: d["TOGGLE"] ? true : false,
                                opacity: d["OPACITY"],
                                // ## Nannie add (19-7-2018) : เพิ่ม layer index ในแต่ละ map service
                                layerIndex: lyrConfig
                            }
                        };
                    }));

                    mapConfig.layers = mapLayers;
                }));

                //var mapLayers = array.map(data, lang.hitch(this, function (d) {
                //    return {
                //        url: (d["SERVICE_URL"].indexOf("http") == 0 ? d["SERVICE_URL"] : (window.location.protocol + d["SERVICE_URL"])),
                //        option: {
                //            id: d["SERVICE_ID"].toString(),
                //            name: d["SERVICE_NAME"],
                //            type: d["SERVICE_TYPE"].toLowerCase(),
                //            visible: d["SERVICE_VISIBLE"] === 0 ? false : true,
                //            addTOC: d["ADD_TOC"] ? true : false,
                //            addMap: d["ADD_MAP"] ? true : false,
                //            systemId: d["SYSTEM_ID"],
                //            index: d["INDEX_NO"]
                //        }
                //    };
                //}));

                //nannie comment (3-7-2018) : change mapconfig to get from database
                //mapConfig.layers = mapConfig.layers;
            } catch (err) {
                console.error(err);
            }
            return { mapConfig: mapConfig };
        },
        _getLayerConfig: function (response) {
            var data = response.data3;
            try {
                var lyrConfig = {};
                array.forEach(data, lang.hitch(this, function (d) {
                    if (!lyrConfig[d["SERVICE_ID"]]) {
                        lyrConfig[d["SERVICE_ID"]] = {};
                    }
                    if (!lyrConfig[d["SERVICE_ID"]][d["LAYER_ID"]]) {
                        lyrConfig[d["SERVICE_ID"]][d["LAYER_ID"]] = {};
                    }
                    lyrConfig[d["SERVICE_ID"]][d["LAYER_ID"]] = {
                        index: d["INDEX_NO"],
                        url: d["URL"]
                    }
                }));
            } catch (err) {
                console.error(err);
            }
            return { layerConfig: lyrConfig };
        },
        _getSystemConfig: function (response) {
            var data = response.data4;
            try {
                data = array.map(array.filter(data, lang.hitch(this, function (d) {
                    return d && d.SYS_ID != null;
                })), lang.hitch(this, function (d) {
                    lang.mixin(d, systemConfig.get(d.SYS_ID));
                    return d;
                }));
            } catch (err) {
                console.error(err);
            }
            return { systemConfig: new Memory({ data: data, idProperty: "SYS_ID" }) };
        },
        _getFunctionConfig: function (response) {
            var data = response.data5;
            return { fnConfig: new Memory({ data: data, idProperty: "FN_ID" }) };
        },

        getAllConfig: function () {
            return this._configures;
        },
        getSystemConfigById: function (systemId) {
            return this._configures.systemConfig.get(systemId);
        },
        getPermissionBySystemConfig: function (systemConfig) {
            var configures = lang.clone(this._configures);
            configures.mapConfig.layers = array.filter(configures.mapConfig.layers, lang.hitch(this, function (layer) {
                return layer.option.systemId.indexOf(systemConfig["id"]) > -1;
            }));
            configures.fnConfig = new Memory({
                data: this._configures.fnConfig.query({
                    SYS_ID: systemConfig["id"]
                }),
                idProperty: "FN_ID"
            });
            return configures;
        }
    });

    wgConstant.getInstance = function (loginResult) {
        if (instance === null) {
            instance = new wgDeclare();
            window._configManager = instance;
        }
        if (loginResult) {
            instance._init(loginResult);
        }
        return instance;
    }

    lang.mixin(wgDeclare, wgConstant);


    return wgDeclare;
});
