define([
    "core/_WidgetBase",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/query",
    "dojo/dom-class",
    "dojo/dom-construct",

    "esri/geometry/Point",
    "esri/geometry/Polyline",
    "esri/symbols/PictureMarkerSymbol",
    "esri/symbols/SimpleLineSymbol",
    "esri/symbols/SimpleFillSymbol",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/graphic",
    "esri/Color",

    "esri/tasks/QueryTask",
    "esri/tasks/query",
    "esri/geometry/geometryEngine",

    "manager/ConfigManager",
    "./EditStatusInfoTemplate/EditStatusInfoTemplate",
    "./SelectOnMap/SelectOnMap",
], function (
    _WidgetBase,

    declare,
    lang,
    array,
    on,
    query,
    domClass,
    domConstruct,

    Point,
    Polyline,
    PictureMarkerSymbol,
    SimpleLineSymbol,
    SimpleFillSymbol,
    SimpleMarkerSymbol,
    graphic,
    Color,

    QueryTask,
    Query,
    geometryEngine,

    ConfigManager,
    EditStatusInfoTemplate,
    SelectOnMap
    ) {
    return declare([
        _WidgetBase
    ], {
        point: null,
        header: null,
        configures: null,

        _misData: null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);
            //this.zoom();
            this.configures = ConfigManager.getInstance().getAllConfig();
        },
        start: function (misData) {
            this._misData = misData;

            var data = misData.parameter,
                sp,
                updateSP,
                params = {},
                Shape;

            // map data to params
            Object.keys(data).forEach(lang.hitch(this, function (object) {
                params[object] = data[object];
            }));

            if (misData.type == 'GATE_STATION' || misData.type == 'GATESTATION') {
                Shape = Point;
                updateSP = 'UTL_U_STATUS_GATE_STATION';
                this.header = 'Gate Station';
                if (misData.menu == 'dashboard') {
                    sp = 'DHB_Q_GATE_STATION';
                } else if (misData.menu == 'utilization-report') {
                    sp = 'RPT_Q_GATE_STATION';
                } else if (misData.menu == 'utilization-search') {
                    sp = 'UTL_Q_GATE_STATION';
                }
            } else if (misData.type == 'PIPELINE') {
                Shape = Polyline;
                updateSP = 'UTL_U_STATUS_PIPELINE';
                this.header = "Pipeline";
                if (misData.menu == 'dashboard') {
                    sp = 'DHB_Q_NGR_PL';
                } else if (misData.menu == 'utilization-report') {
                    sp = 'RPT_Q_NGR_PL';
                } else if (misData.menu == 'utilization-search') {
                    sp = 'UTL_Q_NGR_PL';
                }
            } else if (misData.type == 'METERING') {
                Shape = Point;
                this.header = 'Metering';
                if (misData.menu == 'utilization-search') {
                    sp = 'UTL_Q_METERING';
                }
            } else if (misData.type == 'OFFICE') {
                Shape = Point;
                sp = 'CUS_Q_OFFICE';
                this.header = 'Office';
            } else if (misData.type == 'CUSTOMER') {
                Shape = Point;
                sp = 'CUS_Q_CUSTOMER';
                this.header = 'Customer';
            } //else if (misData.menu == "utilization-search-byregion") {

            //}

            //console.log(params);
            this.reqSP(sp, params).query(
                lang.hitch(this, function (response) {
                    console.log(sp, response);
                    if (response.success == true) {
                        if (response.data.length > 0) {
                            var geometry = this.geometry.fromST(response.data[0].GEOMETRY);
                            var shape = new Shape(geometry);

                            var color = response.data[0].COLOR;
                            var customTemplate = new EditStatusInfoTemplate();

                            //## Nann add(23-7-2018) : Update highlight color
                            this.own(on(customTemplate, "update-status-completed", lang.hitch(this, function () {
                                this.reqSP(sp, params).query(lang.hitch(this, function (updResponse) {
                                    if (updResponse.data.length > 0) {
                                        customTemplate.setUpdatedDate(updResponse.data[0].UPDATED_DATE);

                                        if (misData.type == 'PIPELINE') {
                                            var updColor = updResponse.data[0].COLOR;

                                            this.clearGraphic();

                                            customTemplate.setHilightColor(updColor);
                                            customTemplate.addHilight();

                                            this.addGraphic(shape, updColor, infoTemplate);
                                        }
                                    }
                                }));
                            })));

                            //###

                            customTemplate.setUpdateSP(updateSP, response.data[0], misData);
                            customTemplate.setShape(shape);
                            customTemplate.setMultiInfoTemplate(response.data, response.data1, this.header);

                            if (misData.flag == true && !misData.type.match(/^(METERING)$/)) {
                                customTemplate.showFlagButton();
                            }
                            var infoTemplate = customTemplate.getInfoTemplate();
                            customTemplate.setHilightColor(color);          // Nannie move(16-7-2018): ย้ายจาก if ข้างล่าง เพราะว่า ใน if คือ pipeline ที่มากกว่า 1 เส้น เส้นเดียวก็ต้อง set highlight

                            if (response.data.length > 1) {
                                for (var i = 1; i < response.data.length; i++) {
                                    var obj = response.data[i];
                                    var geometry = this.geometry.fromST(obj.GEOMETRY);

                                    // ## Nannie (6-7-2018) : มีกรณีที่เป็น Point ด้วย ต้องเช็คว่าเป้น line ถึงจะทำงาน
                                    if (geometry.hasOwnProperty("paths")) {
                                        for (var j = 0; j < geometry.paths.length; j++) {
                                            shape.addPath(geometry.paths[j]);
                                        }
                                    }
                                    // ##
                                    //console.log(shape);
                                }
                                //customTemplate.setHilightColor(color);
                            }

                            //var infoTemplate = this.getInfoTemplate(response.data[0], response.data1);

                            //## Nann comment(17-7-2018) : มี zoom อยู๋ที่ widget EditStatusInfoTemplate อยู่แล้ว
                            //this.zoomToShape(shape).then(lang.hitch(this, function (defer) {

                            this.clearGraphic();
                            this.addGraphic(shape, color, infoTemplate);

                            //## Nann edit(24-7-2018) : move to EditStatusInfoTemplate widget
                            //this.map.infoWindow.clearFeatures();
                            //this.map.infoWindow.setContent(infoTemplate);
                            //this.map.infoWindow.setTitle(infoTemplate.headerInfo);
                            //this.map.infoWindow.show(this.point);

                            //should fix soon
                            customTemplate.showFirstPage();
                            //##

                            // }));
                            //##

                        }
                    }
                }),
                lang.hitch(this, function (err) {
                    console.log(sp, "fail", err);
                    //this.hideLoading();
                })
            );

            //console.log("getMenu", this.getMenu());
            //if (this.getMenu() == "utilization") {
            //   new SelectOnMap();
            //}
        },

        zoomToShape: function (shape) {
            if (shape.type === 'point') {
                return this.map.centerAndZoom(shape, 19);
            } else if (shape.type === 'polygon') {
                return this.map.setExtent(shape.getExtent().expand(1), true);
            } else {
                if (this.map.getLevel() != 18) {
                    this.map.setLevel(18);
                }
                return this.map.setExtent(shape.getExtent().expand(1), true);       //## Nannie (6-7-2018) : เพิ่มให้ Expand จากเส้นออกมานิดหน่อย
            }
        },

        addGraphic: function (shape, color, info) {
            var marker = null;
            if (shape.type == 'point') {
                var imageUrl;
                if (this._misData.type == 'GATE_STATION' || this._misData.type == 'GATESTATION') {
                    imageUrl = "./js/system/gis/resources/images/gatestation/" + color.replace("#", "") + ".png";
                    console.log("imageUrl", imageUrl, color.replace("#", ""));

                    var pictureMarkerSymbol = new PictureMarkerSymbol(imageUrl, 28, 40);
                    pictureMarkerSymbol = pictureMarkerSymbol.setOffset(0, 7);
                    marker = new graphic(shape, pictureMarkerSymbol);

                    this.map.graphics.add(marker);
                }

                //var pictureMarkerSymbol = new PictureMarkerSymbol('./js/system/gis/resources/images/pin.png', 29, 46);
                //pictureMarkerSymbol = pictureMarkerSymbol.setOffset(0, 23);
                //marker = new graphic(shape, pictureMarkerSymbol);

                var markerSymbol = new SimpleMarkerSymbol({
                    "color": [255, 0, 0, 255],
                    "size": 8,
                    "angle": -30,
                    "xoffset": 0,
                    "yoffset": 0,
                    "type": "esriSMS",
                    "style": "esriSMSCircle"
                    , "outline": {
                        "color": [0, 0, 0, 255],
                        "width": 2,
                        "type": "esriSLS",
                        "style": "esriSLSSolid"
                    }
                });
                marker = new graphic(shape, markerSymbol);
                this.point = shape;
            }
            else if (shape.type === 'polygon') {
                var polygonSymbol = new SimpleFillSymbol(SimpleFillSymbol.STYLE_SOLID,
                       new SimpleLineSymbol(SimpleLineSymbol.STYLE_DASHDOT,

                       new Color([255, 0, 0]), 2), new Color([255, 255, 0, 0.25])
                     );
                marker = new graphic(shape, polygonSymbol);
                this.point = shape;
            } else {
                var lineSymbol = new SimpleLineSymbol().setColor(Color.fromHex(color)).setWidth(5);
                marker = new graphic(shape, lineSymbol);
                //console.log(shape.paths);
                //var medianPath = Math.floor(shape.paths.length / 2),
                //    medianPoint = Math.floor(shape.paths[medianPath].length / 2);
                ////console.log(medianPath, medianPoint);
                //this.point = shape.getPoint(medianPath, medianPoint);
                this.point = shape.getPoint(0, 0);
            }
            //marker.setInfoTemplate(info);
            //Window['map'] = this.map;
            this.map.graphics.add(marker);

        },
        clearGraphic: function () {
            var graphics = this.map.graphics.graphics;
            for (var i = 0; i < graphics.length; i++) {
                if (graphics[i].symbol != undefined) {
                    this.map.graphics.remove(graphics[i--]);
                }
            }
        },
        hidden: function () {
            this.map.infoWindow.clearFeatures();
            this.map.infoWindow.hide();
            this.clearGraphic();
        },

        zoomToRegion: function (misData) {
            try {
                this.displayLoader(true);
                if (misData.parameter != null && lang.isArray(misData.parameter.REGION) && misData.parameter.REGION.length > 0) {

                    var queryTask = new QueryTask(this.configures.mapConfig.services.REGION_QUERY);
                    var query = new Query();
                    query.where = "PTTOUR.REGION.REGION_ID in (" + misData.parameter.REGION.join(",") + ")";
                    //query.outSpatialReference = { wkid: 102100 };
                    query.returnGeometry = true;
                    query.outFields = ["PTTOUR.REGION.REGION_NAME_TH"];
                    query.maxAllowableOffset = 1000;

                    queryTask.execute(query,
                        lang.hitch(this, function (result) {
                            //console.log("result", result);
                            var arrGeo = [];
                            array.forEach(result.features,
                                lang.hitch(this, function (feature) {
                                    if (feature.geometry) {
                                        arrGeo.push(feature.geometry);
                                    }
                                }));
                            if (arrGeo.length > 0) {
                                var union = geometryEngine.union(arrGeo);
                                //console.log("union", union);
                                this.addGraphic(union);         // ## Nannie add(20-7-2018) : add graphic
                                this.zoomToShape(union);
                            }
                            this.displayLoader(false);
                        }),
                        lang.hitch(this, function (error) {
                            console.error(error);
                            this.displayLoader(false);
                        }));
                }
            } catch (e) {
                this.displayLoader(false);
                console.error("zoomToRegion", e);
            }

        }
    });
});