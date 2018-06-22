document.title = "PTT-OUR-GIS";
var dojoConfig = (function () {
    var host = window.location.href;
    if (host.indexOf("?") != -1) {
        host = host.substring(0, host.indexOf("?"));
    }
    if (host.indexOf("#") != -1) {
        host = host.substring(0, host.indexOf("#"));
    }
    host = host.substring(0, host.lastIndexOf("/"));
    return {
        appUrl: host,
        parseOnLoad: true,
        tlmSiblingOfDojo: false,
        isDebug: false,
        packages: [{
            name: "nls",
            location: host + "/nls"
        }, {
            name: "config",
            location: host + "/cfg"
        }, {
            name: "core",
            location: host + "/js/core"
        }, {
            name: "viewer",
            location: host + "/js/viewer"
        }, {
            name: "manager",
            location: host + "/js/manager"
        }, {
            name: "system",
            location: host + "/js/system"
        }, {
            name: "widgets",
            location: host + "/js/widgets"
        }, {
            name: "um",
            location: host + "/js/um"
        }],
        usePlainJson: true,
        locale: "th",
        extraLocale: ["en"],
        async: true,
        cacheBust: "v=" + window.appVersion
    };
})();