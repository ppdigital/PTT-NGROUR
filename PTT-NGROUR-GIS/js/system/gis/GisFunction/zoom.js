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
    "esri/graphic",
    "esri/Color",

    "system/gis/GisFunction/EditStatusInfoTemplate/EditStatusInfoTemplate"
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
    graphic,
    Color,

    EditStatusInfoTemplate
    ) {
    return declare([
        _WidgetBase
    ], {
        point: null,
        header: null,
        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);

            //this.zoom();
        },
        start: function (misData) {
            var data = misData.parameter,
                sp,
                updateSP,
                params = {},
                Shape;

            if (misData.type == 'GATE_STATION') {
                Shape = Point;
                updateSP = 'ULT_U_STATUS_GATE_STATION';
                this.header = 'Gate Station';
                if (misData.menu == 'dashboard') {
                    sp = 'DHB_Q_GATE_STATION';
                } else if (misData.menu == 'report') {
                    sp = 'RPT_Q_GATE_STATION';
                } else if (misData.menu == 'utilization-search') {
                    sp = 'UTL_Q_GATE_STATION';
                }
            } else if (misData.type == 'PIPELINE') {
                Shape = Polyline;
                updateSP = 'ULT_U_STATUS_PIPELINE';
                this.header = "Pipeline";
                if (misData.menu == 'dashboard') {
                    sp = 'DHB_Q_NGR_PL';
                } else if (misData.menu == 'report') {
                    sp = 'RPT_Q_NGR_PL';
                } else if (misData.menu == 'utilization-search') {
                    sp = 'UTL_Q_NGR_PL';
                }
            } else if (misData.type == 'METERING') {
                Shape = Point;
                updateSP = 'ULT_U_STATUS_METERING';
                this.header = 'Metering';
                sp = 'UTL_Q_METERING';
            }
            // map data to params
            Object.keys(data).forEach(lang.hitch(this, function (object) {
                params[object] = data[object];
            }));

            //console.log(params);
            this.reqSP(sp, params).query(
                lang.hitch(this, function (response) {
                    //this.hideLoading();
                    //console.log("GIS_Q_RESPON_AREA", response);
                    console.log(sp, response);
                    if (response.success == true) {
                        if (response.data.length > 0) {
                            var geometry = this.geometry.fromST(response.data[0].GEOMETRY);
                            var shape = new Shape(geometry);
                            var color = response.data[0].COLOR;
                            var customTemplate = new EditStatusInfoTemplate(updateSP, params);
                            customTemplate.setInfoTemplate(response.data[0], response.data1, this.header);
                            if (misData.flag == true && !misData.type.match(/^(METERING)$/)) {
                                customTemplate.showFlagButton();
                            }
                            var infoTemplate = customTemplate.getInfoTemplate();
                            
                            if (response.data.length > 1) {
                                for (var i = 1; i < response.data.length; i++) {
                                    var obj = response.data[i];
                                    var line = this.geometry.fromST(obj.GEOMETRY);
                                    //console.log(geometry);
                                    for (var j = 0; j < line.paths.length; j++) {
                                        shape.addPath(line.paths[j]);
                                    }
                                    //console.log(shape);
                                }
                            }

                            //var infoTemplate = this.getInfoTemplate(response.data[0], response.data1);

                            //console.log();
                            this.zoomToShape(shape).then(lang.hitch(this, function (defer) {
                                this.clearGraphic();
                                this.addGraphic(shape, color, infoTemplate);

                                //var x = new EditStatusInfoTemplate(y, response.data1);
                                //console.log(x);
                                //this.map.infoWindow.setContent(x.domNode);
                                this.map.infoWindow.setContent(infoTemplate);
                                this.map.infoWindow.setTitle(infoTemplate.title);
                                this.map.infoWindow.show(this.point);

                                //console.log(this.map.graphics);
                            }));

                        }
                    }
                }),
                lang.hitch(this, function (err) {
                    console.log(sp, "fail", err);
                    //this.hideLoading();
                })
            );
        },
        zoomToShape: function (shape) {
            if (shape.type === 'point') {
                return this.map.centerAndZoom(shape, 19);
            } else {
                return this.map.setExtent(shape.getExtent(), true);
            }
        },

        addGraphic: function (shape, color, info) {
            var marker = null;
            if (shape.type == 'point') {
                var pictureMarkerSymbol = new PictureMarkerSymbol('./js/system/gis/resources/images/pin.png', 29, 46);
                pictureMarkerSymbol = pictureMarkerSymbol.setOffset(0, 23);
                marker = new graphic(shape, pictureMarkerSymbol);
                this.point = shape;
            } else {
                var lineSymbol = new SimpleLineSymbol().setColor(Color.fromHex(color)).setWidth(5);
                marker = new graphic(shape, lineSymbol);
                //console.log(shape.paths);
                var medianPath = Math.floor(shape.paths.length / 2),
                    medianPoint = Math.floor(shape.paths[medianPath].length / 2);
                //console.log(medianPath, medianPoint);
                this.point = shape.getPoint(medianPath, medianPoint);
            }
            //marker.setInfoTemplate(info);
            //Window['map'] = this.map;
            this.map.graphics.add(marker);
            console.log(this.map);
        },
        clearGraphic: function () {
            var graphics = this.map.graphics.graphics;
            for (var i = 0; i < graphics.length; i++) {
                //if (graphics[i].infoTemplate != undefined) {
                    this.map.graphics.remove(graphics[i--]);
                //}
            }
        }
    });
});