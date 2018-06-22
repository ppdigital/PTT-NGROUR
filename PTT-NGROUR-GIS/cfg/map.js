define([
    "esri/tasks/GeometryService",
    "esri/config",
    "dojo/has",
    "dojo/_base/lang",
    "require"
],
function (
    GeometryService,
    esriConfig,
    has,
    lang,
    require
    ) {

    var mapConfig = {};


    lang.mixin(mapConfig, {
        "option": {
            "logo": false,
            "slider": false,
            "sliderPosition": "top-left",
            "sliderStyle": "small",
            "showAttribution": false,
            "fadeOnZoom": true,
            "scale": 244648.868618,
            "center": [100.626, 13.608]
        },
        "layers":   [
            {
                "url": "https://pttarcgisserver.pttplc.com/arcgis/rest/services/NOSTRA_CACHE/MapServer",
                "option": {
                    "id": "NOSTRA_CACHE",
                    "name": "NOSTRA_CACHE",
                    "type": "tiled",
                    "visible": true,
                    "addTOC": true,
                    "addMap": true,
                    "systemId": "GIS",
                    "index": 0
                }
            },
            {
                "url": "https://nonpttarcgisserver.pttplc.com/arcgis/rest/services/PTT_BASEMAP/MapServer",
                "option": {
                    "id": "PTT_BASEMAP",
                    "name": "PTT_BASEMAP",
                    "type": "dynamic",
                    "visible": true,
                    "addTOC": true,
                    "addMap": true,
                    "systemId": "GIS",
                    "index": 1
                }
            },
            {
                "url": "https://nonpttarcgisserver.pttplc.com/arcgis/rest/services/ORTHO_CACHE/MapServer",
                "option": {
                    "id": "ORTHO_CACHE",
                    "name": "ORTHO_CACHE",
                    "type": "tiled",
                    "visible": false,
                    "addTOC": true,
                    "addMap": true,
                    "systemId": "GIS",
                    "index": 2
                }
            },
            {
                "url": "https://nonpttarcgisserver.pttplc.com/arcgis/rest/services/PTT_OUR/PTTOUR_DATA/MapServer",
                "option": {
                    "id": "PTTOUR_DATA",
                    "name": "PTTOUR_DATA",
                    "type": "dynamic",
                    "visible": true,
                    "addTOC": true,
                    "addMap": true,
                    "systemId": "GIS",
                    "index": 3
                }
            },
            {
                "url": "https://nonpttarcgisserver.pttplc.com/arcgis/rest/services/PTT_OUR/PTTOUR_PL/MapServer",
                "option": {
                    "id": "PTTOUR_PL",
                    "name": "PTTOUR_PL",
                    "type": "dynamic",
                    "visible": true,
                    "addTOC": true,
                    "addMap": true,
                    "systemId": "GIS",
                    "index": 4
                }
            },
            {
                "url": "https://nonpttarcgisserver.pttplc.com/arcgis/rest/services/PTT_OUR/PTTOUR_POLYGON/MapServer",
                "option": {
                    "id": "PTTOUR_POLYGON",
                    "name": "PTTOUR_POLYGON",
                    "type": "dynamic",
                    "visible": false,
                    "addTOC": true,
                    "addMap": true,
                    "systemId": "GIS",
                    "index": 5
                }
            }
            
        ],
        "features": [{
            "featureCollectionObject": {
                "layerDefinition": {
                    "geometryType": "esriGeometryPolygon",
                    "fields": [{
                        "name": "OBJECTID",
                        "type": "esriFieldTypeOID",
                        "alias": "ObjectId"
                    }],
                    "drawingInfo": {
                        "renderer": {
                            "type": "simple",
                            "symbol": {
                                "type": "esriSFS",
                                "style": "esriSFSSolid",
                                "color": [0, 0, 0, 100],
                                "outline": {
                                    "type": "esriSLS",
                                    "style": "esriSLSSolid",
                                    "color": [110, 110, 110, 255],
                                    "width": 5
                                }
                            }
                        }
                    }
                },
                "featureSet": null
            },
            "option": {
                "id": "gLayerPolygon",
                "className": "g-layer-polygon",
                "visible": true,
                "opacity": 1
                //"styling": has("ie") && has("ie") <= 8 ? true : false
            }
        }, {
            "featureCollectionObject": {
                "layerDefinition": {
                    "geometryType": "esriGeometryPolyline",
                    "fields": [{
                        "name": "OBJECTID",
                        "type": "esriFieldTypeOID",
                        "alias": "ObjectId"
                    }],
                    "drawingInfo": {
                        "renderer": {
                            "type": "simple",
                            "symbol": {
                                "type": "esriSLS",
                                "style": "esriSLSSolid",
                                "color": [0, 200, 255, 200],
                                "width": 3
                            }
                        }
                    }
                },
                "featureSet": null
            },
            "option": {
                "id": "gLayerPolyline",
                "className": "g-layer-polyline",
                "visible": true,
                "opacity": 1,
                "styling": has("ie") && has("ie") <= 8 ? true : false
            }
        }, {
            "featureCollectionObject": {
                "layerDefinition": {
                    "geometryType": "esriGeometryPoint",
                    "fields": [{
                        "name": "OBJECTID",
                        "type": "esriFieldTypeOID",
                        "alias": "ObjectId"
                    }],
                    "drawingInfo": {
                        "renderer": {
                            "type": "simple",
                            "symbol": {
                                "type": "esriSMS",
                                "style": "esriSMSCircle",
                                "color": [255, 0, 0, 128],
                                "size": 12,
                                "angle": 0,
                                "xoffset": 0,
                                "yoffset": 0,
                                "outline": {
                                    "type": "esriSLS",
                                    "style": "esriSLSSolid",
                                    "color": [0, 0, 0, 255],
                                    "width": 1
                                }
                            }
                        }
                    }
                },
                "featureSet": null
            },
            "option": {
                "id": "gLayerPoint",
                "className": "g-layer-point",
                "visible": true,
                "opacity": 1
                //"styling": has("ie") && has("ie") <= 8 ? true : false
            }
        }],

        "services": {},


        "defaultSymbol": {
            "point": {
                "type": "esriSMS",
                "style": "esriSMSCircle",
                "color": [255, 0, 0, 128],
                "size": 12,
                "angle": 0,
                "xoffset": 0,
                "yoffset": 0,
                "outline": {
                    "type": "esriSLS",
                    "style": "esriSLSSolid",
                    "color": [0, 0, 0, 255],
                    "width": 1
                }
            },
            "polyline": {
                "type": "esriSLS",
                "style": "esriSLSSolid",
                "color": [0, 0, 0, 255],
                "width": 3
            },
            "polygon": {
                "type": "esriSFS",
                "style": "esriSFSSolid",
                "color": [0, 0, 0, 50],
                "outline": {
                    "type": "esriSLS",
                    "style": "esriSLSSolid",
                    "color": [255, 0, 0, 255],
                    "width": 2
                }
            }
        }
    });

    esriConfig.defaults.geometryService = new GeometryService(mapConfig.services.geometry);

    return mapConfig;
});
