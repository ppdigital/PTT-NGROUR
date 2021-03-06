﻿/**
 * @description 
 * -
 * @class _RequestMixin
 */
define([
    "esrith/amos/core/DataStore",

    "config/app",

    "dojo/on",
    "dojo/cookie",
    "dojo/json",
    "dojo/aspect",
    "dojo/query",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    DataStore,

    appConfig,

    on,
    cookie,
    JSON,
    aspect,
    query,
    lang,
    declare,
    require
    ) {

    var wgConstant = {
        reqOperations: new Array()
    };

    var wgDeclare = declare(/*[DECLARED_CLASS]*/null, {

        _defaultDSName: "DataService.svc",
        _defaultMethodName: "DS_TRANSIT",

        /**
        * Send HTTP request to URL.
        * @method reqUrl
        * @public
        * @param {String} url 
        * @param {Object} [param]
        */
        reqUrl: function (url, param, useProxy) {
            try {
                //this.displayLoader(true);
                param = param || {};
                param.CSRF_TOKEN = cookie("CSRF_TOKEN");

                if (useProxy == null)
                    useProxy = url.toLowerCase().indexOf("proxy.ashx") == -1;

                if (url.indexOf("http") != 0) {
                    url = lang.replace("{0}/{1}", [appConfig.host, url]);
                }
                var dataStore = new DataStore(url, param);
                //url.toLowerCase().indexOf(lang.replace("{protocol}//{host}", window.location).toLowerCase()) == 0;
                dataStore.useProxy = useProxy;
                dataStore.preventCache = dataStore.useProxy;

                var showLoaderOnQuery = aspect.before(dataStore, "query", lang.hitch(this, function () {
                    this.displayLoader(true);
                    showLoaderOnQuery.remove();
                }));

                var hideLoaderAfterComplete = on(dataStore, "query-completed", lang.hitch(this, function () {
                    this.displayLoader(false);
                    hideLoaderAfterComplete.remove();
                    hideLoaderAfterError.remove();
                }));
                
                var hideLoaderAfterError = on(dataStore, "query-error", lang.hitch(this, function () {
                    this.displayLoader(false);
                    hideLoaderAfterComplete.remove();
                    hideLoaderAfterError.remove();
                }));

                return dataStore;
            } catch (err) {
                this.displayLoader(false);
                console.error(err);
            }
        },


        /**
        * Send HTTP request to DataService.svc
        * @method reqDS
        * @public
        * @param {String} methodName
        * @param {Object} [param]
        * @return {DataStore}
        */
        /**
        * Send HTTP request to DataService.
        * @method reqDS
        * @public
        * @param {String} svcName 
        * @param {String} methodName 
        * @param {Object} [param]
        * @return {DataStore}
        */
        reqDS: function (svcName, methodName, param, useProxy) {
            try {
                if (typeof (arguments[1]) != "string") {
                    param = lang.clone(arguments[1] || {});
                    methodName = lang.clone(arguments[0]);
                    svcName = this._defaultDSName;
                } else {
                    param = param || {};
                }
                var url = appConfig.dataService(svcName, methodName);

                console.log("reqDS url", url);

                return this.reqUrl(url, param, useProxy);
            } catch (err) {
                console.error(err);
            }
        },

        /**
        * Send HTTP request to Stored Procedure.
        * @method reqDS
        * @public
        * @param {String} spName 
        * @param {Object} [param]
        * @return {DataStore}
        */
        reqSP: function (spName, param, useProxy) {
            try {
                param = param || {};
                param = lang.clone(param);
                lang.mixin(param, {
                    SP: spName
                });
                return this.reqDS(this._defaultDSName, this._defaultMethodName, param, useProxy);
            } catch (err) {
                console.error(err);
            }
        },

        /**
        * Send Notification.
        * @method reqNT
        * @public
        * @param {String} ntName 
        * @param {Object} [ntParam]
        * @param {String} [spName]
        * @param {Object} [spParam]
        * @return {DataStore}
        */
        reqNT: function (ntName, ntParam, spName, spParam) {
            try {
                if (ntName == null || ntName == "") {
                    throw new Error("ntName ไม่มีการระบุค่า");
                }

                var param = {};

                ntParam = ntParam || {};
                ntParam = lang.clone(ntParam);

                if (spName == null || spName == "") {
                    spName = null;
                    spParam = {};
                } else {
                    spParam = spParam || {};
                    spParam = lang.clone(spParam);
                }

                lang.mixin(param, ntParam, spParam, {
                    NT: ntName,
                    SP: spName
                });

                return this.reqDS(this._defaultDSName, this._defaultMethodName, param);
            } catch (err) {
                console.error(err);
            }
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

    return wgDeclare;
});