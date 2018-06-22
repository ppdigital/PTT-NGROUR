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
    "esri/InfoTemplate",
    "esri/dijit/InfoWindow",
    "esri/symbols/PictureMarkerSymbol",
    "esri/symbols/SimpleLineSymbol",
    "esri/graphic",
    "esri/Color",

    "system/gis/test/PokTest/PokTest"
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
    InfoTemplate,
    InfoWindow,
    PictureMarkerSymbol,
    SimpleLineSymbol,
    graphic,
    Color,

    Test
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
            var data = misData.data,
                sp,
                params = {},
                Shape;

            if (misData.layer == 'GATE_STATION') {
                Shape = Point;
                this.header = 'Gate Station';
                if (misData.menu == 'dashboard') {
                    sp = 'DHB_Q_GATE_STATION';
                } else if (misData.menu == 'report') {
                    sp = 'RPT_Q_GATE_STATION';
                } else if (misData.menu == 'utilization-search') {
                    sp = 'UTL_Q_GATE_STATION';
                }
            } else if (misData.layer == 'NGR_PL') {
                Shape = Polyline;
                this.header = "Pipeline";
                if (misData.menu == 'dashboard') {
                    sp = 'DHB_Q_NGR_PL';
                } else if (misData.menu == 'report') {
                    sp = 'RPT_Q_NGR_PL';
                } else if (misData.menu == 'utilization-search') {
                    sp = 'UTL_Q_NGR_PL';
                }
            } else if (misData.layer == 'METERING') {
                Shape = Point;
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

                            for (var i = 0; i < response.data.length; i++) {
                                var obj = response.data[i];
                                var geometry = this.geometry.fromST(obj.GEOMETRY);
                                //console.log(geometry);
                                var shape = new Shape(geometry);
                                var color = response.data[i].COLOR;
                                var infoTemplate = this.getInfoTemplate(response.data[i], response.data1);
                                //console.log(shape);
                                this.zoomToShape(shape).then(lang.hitch(this, function (defer) {
                                    this.clearGraphic();
                                    this.addGraphic(shape, color, infoTemplate);

                                    var x = new Test();
                                    console.log(x);
                                    //this.map.infoWindow.setContent(x.domNode);
                                    this.map.infoWindow.setContent(infoTemplate.content);
                                    this.map.infoWindow.setTitle(infoTemplate.title);
                                    this.map.infoWindow.show(this.point);

                                    //console.log(this.map.graphics);
                                }));
                            }

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
                return this.map.setExtent(shape.getExtent());
            }
        },
        getInfoTemplate: function (data, fieldDetail) {
            var attrTable = [],
                name;
            Object.keys(data).map(lang.hitch(this, function (fieldName) {
                if (fieldName.match(/^(NAME|RC_PROJECT)$/)) {
                    name = data[fieldName];
                } else if (fieldName.match(/^(GEOMETRY|COLOR)$/)) {
                    // do nothing
                }
                else {
                    var field = this.find(fieldDetail, 'FIELDNAME', fieldName);
                    var attrContent = '<tr><td style="width: 100px; color: #337FD4;">' + field.DISPLAY + '</td><td style="color: #337FD4;">:</td><td>' + data[fieldName] + '</td></tr>';
                    attrTable += attrContent;
                }
            }));
            //console.log(attrTable);

            var infoTemplate = new InfoTemplate(
                //'<div style="box-shadow: 0 1px 0 #42546b; padding: 0.5rem 0; margin-bottom: 0.5rem;">'+ this.header+' : ' + name + '</div>',
                 this.header + ' : ' + name,
                '<table style="width: 100%;">' +
                attrTable +
                '</table>'
            );
            return infoTemplate;
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
            marker.infoTemplate = info;
            this.map.graphics.add(marker);
        },
        clearGraphic: function () {
            var graphics = this.map.graphics.graphics;
            for (var i = 0; i < graphics.length; i++) {
                if (graphics[i].infoTemplate != undefined) {
                    this.map.graphics.remove(graphics[i--]);
                }
            }
        },
        find: function (array, field, targetValue) {
            var result = null;
            array.forEach( function (object) {
                if (object[field] == targetValue) {
                    result =  object;
                }
            });
            return result;
        }
    });
});