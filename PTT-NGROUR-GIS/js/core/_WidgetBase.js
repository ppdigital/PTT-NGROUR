/**
 * @description 
 * -
 * @class _WidgetBase
 * @constructor
 * @uses _AlertMixin
 * @uses _RequestMixin
 * @uses _GeometryMixin            
 */
define([
    "dijit/_WidgetBase",
    "dijit/_TemplatedMixin",
    "dijit/_WidgetsInTemplateMixin",
    "./_ClassBase",
    "esrith/amos/map/MapExtension",

    "dojo/on",
    "dojo/dom",
    "dojo/query",
    "dojo/dom-class",
    "dojo/dom-construct",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "dojo/_base/array",

    "dojo/Deferred",
    "dojo/promise/all",

    "require"
], function (
    _WidgetBase,
    _TemplatedMixin,
    _WidgetsInTemplateMixin,
    _ClassBase,
    MapExtension,

    on,
    dom,
    query,
    domClass,
    domConstruct,
    lang,
    declare,
    array,

    Deferred,
    all,

    require
    ) {

    //var divMainLoading = dom.byId("_mainLoading_") || domConstruct.create("div", { id: "_mainLoading_" }, document.body);
    //domClass.add(divMainLoading, "amos-hide");
    //domClass.add(divMainLoading, "ui-loading");

    var wgConstant = {};

    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
        _WidgetBase,
        _TemplatedMixin,
        _WidgetsInTemplateMixin,
        _ClassBase
    ], {

        templateString: "<div></div>",

        /**
        * -
        * @property map
        * @type esri.Map
        * @public
        */
        map: null,
        _urlParam: null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
            //Add by Nattawit.kr
            params.subWidgets = params.subWidgets || this.subWidgets;
            if (params.config) {
                params.config = lang.mixin(lang.mixin({}, this.defaultConfig), params.config);
            } else {
                params.config = lang.mixin({}, this.defaultConfig);
            }
            //End Add
        },

        postCreate: function () {
            this.inherited(arguments);
        },

        startup: function () {
            this.inherited(arguments);
            if (typeof (this.showMessage) != "function") {
                this.showMessage = lang.hitch(this, function () {
                    if (!this.getParent()) return;
                    return this.getParent().showMessage.apply(this.getParent(), arguments);
                });
            }
        },

        isVisible: function (elem) {
            elem = elem ? elem.domNode || elem : this.domNode;
            return !domClass.contains(elem, "amos-hide");
        },

        toggle: function (elem) {
            elem = elem ? elem.domNode || elem : this.domNode;
            domClass.toggle(elem, "amos-hide");
        },

        show: function (elem) {
            elem = elem ? elem.domNode || elem : this.domNode;
            domClass.remove(elem, "amos-hide");
        },

        hide: function (elem) {
            elem = elem ? elem.domNode || elem : this.domNode;
            domClass.add(elem, "amos-hide");
        },

        _setMapAttr: function (map) {
            if (!map.esrith) {
                map.esrith = new MapExtension(map);
            }
            this._set("map", map);
        },

        _addEvent: function (obj, action, name, parameters) {
            this.own(on(obj, action, lang.hitch(this, function (e) {
                this.emit(name, parameters || e);
            })));
        },
        //Add By nattawit.kr 2018/06/13
        _setSubWidgetsAttr: function (value) {
            if (!value) return;
            if (!lang.isObject(value)) return;
            var newValue = null;
            var deferredObject = {};
            for (var key in value) {
                deferredObject[key] = this.loadWidget(value[key]);
            }
            newValue = all(deferredObject);
            this._set("subWidgets", newValue);
        },
        loadWidget: function (namespace) {
            var deferred = new Deferred();
            require([namespace], lang.hitch(this, function (ClassName) {
                deferred.resolve(ClassName);
            }));
            return deferred.promise;
        },
        nodeEmpty: function (dom) {
            while (dom.firstChild) {
                dom.removeChild(dom.firstChild);
            }
        },
        //End Add

        addComma: function (nStr) {
            nStr += '';
            var x = nStr.split('.');
            var x1 = x[0];
            var x2 = x.length > 1 ? '.' + x[1] : '';
            var rgx = /(\d+)(\d{3})/;
            while (rgx.test(x1)) {
                x1 = x1.replace(rgx, '$1' + ',' + '$2');
            }
            return x1 + x2;
        },

        getUrlParam: function () {
            //_urlParam
        },

        getMenu: function () {
            var s = window.location.search;
            var menu = null;
            if (s != null && s.length > 0) {
                var arrParam = s.substring(1).split("&");
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

        displayLoader: function (show) {
            if (show) {
                //query(".bg-loader").removeClass("amos-hide");
                //setTimeout(lang.hitch(this, function () {
                query(".bg-loader").addClass("show-loader");
                //}), 1);
            } else {
                query(".bg-loader").removeClass("show-loader");
                //setTimeout(lang.hitch(this, function () {
                //    query(".bg-loader").addClass("amos-hide");
                //}), 1100);
            }
        }

    });

    lang.mixin(wgDeclare, wgConstant);


    return wgDeclare;
});
