define([
    "dojo/_base/lang",
    "require"
],
function (
    lang,
    require
    ) {


    var appConfig = {};

    if (window.console && window.appServer && window.appServer.isDebug == 0) {
        window.console.log = function () { };
        window.console.error = function () { };
        window.console.info = function () { };
        window.console.dir = function () { };
    }
    var host = window.location.href;
    if (host.indexOf("?") != -1) {
        host = host.substring(0, host.indexOf("?"));
    }
    if (host.indexOf("#") != -1) {
        host = host.substring(0, host.indexOf("#"));
    }
    host = host.substring(0, host.lastIndexOf("/"));
    lang.mixin(appConfig, {
        "host": host,
        "proxy": lang.replace("{0}/{1}", [host, "ashx/Proxy.ashx"]),
        "dataService": function (arg1, arg2) {
            var svcName = arg2 ? arg1 : "DataService.svc";
            var spName = arg2 || arg1 || "DS_TRANSIT";
            var url = lang.replace("{0}/{1}/{2}/{3}", [host, "services", svcName, spName]);
            return url;
        }
    });

    return appConfig;
});
