﻿require([
    "config/app",
    "esri/config",
    "esri/urlUtils"
], function (
    appConfig,
    esriConfig,
    urlUtils
    ) {

    esriConfig.defaults.io.alwaysUseProxy = true;
    urlUtils.addProxyRule({
        proxyUrl: appConfig.proxy,
        urlPrefix: appConfig.host + "/" + "services/DataService.svc"
    });
    esriConfig.defaults.io.proxyUrl = appConfig.proxy;
    esriConfig.defaults.io.timeout = 60000;
    esriConfig.defaults.io.useCors = true;

    esriConfig.defaults.io.errorHandler = function (error) { };
    esriConfig.defaults.io.corsDetection = true;
    esriConfig.defaults.io.corsEnabledServers = ["www.arcgis.com", "tiles.arcgis.com", "services.arcgis.com", "services1.arcgis.com", "services2.arcgis.com", "services3.arcgis.com", "static.arcgis.com", "utility.arcgisonline.com", "geocode.arcgis.com", "geoenrich.arcgis.com", "qaext.arcgis.com", "tilesqa.arcgis.com", "servicesqa.arcgis.com", "servicesqa1.arcgis.com", "servicesqa2.arcgis.com", "servicesqa3.arcgis.com", "geocodeqa.arcgis.com", "geoenrichqa.arcgis.com", "dev.arcgis.com", "devext.arcgis.com", "tilesdevext.arcgis.com", "servicesdev.arcgis.com", "servicesdev1.arcgis.com", "servicesdev2.arcgis.com", "servicesdev3.arcgis.com", "geocodedev.arcgis.com", "geoenrichdev.arcgis.com"];
    esriConfig.defaults.io.postLength = 2000;

    esriConfig.defaults.map.panDuration = 350;
    esriConfig.defaults.map.panRate = 50;
    esriConfig.defaults.map.zoomDuration = 500;
    esriConfig.defaults.map.zoomRate = 50;

    esriConfig.defaults.map.slider = {
        "left": "10px",
        "top": "10px",
        "width": null,
        "height": "200px"
    };
    esriConfig.defaults.map.sliderLabel = {
        "tick": 5,
        "labels": null,
        "style": "width:2em; font-family:Verdana; font-size:75%;"
    };

    esriConfig.defaults.map.zoomSymbol = {
        "color": [0, 0, 0, 64],
        "outline": {
            "color": [255, 0, 0, 255],
            "width": 1.5,
            "style": "esriSLSSolid"
        },
        "style": "esriSFSSolid"
    };
});