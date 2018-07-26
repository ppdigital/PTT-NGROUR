define([
    "core/_WidgetBase",

    "dojo/text!./CustomerManagement.html",
    "xstyle/css!./CustomerManagement.css",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/dom",
    "dojo/dom-class",
	"dojo/dom-construct",

    "esri/graphic",
    "esri/geometry/Point",
    "esri/symbols/PictureMarkerSymbol",
    "esri/symbols/SimpleMarkerSymbol",
    "esri/toolbars/draw",
    "esri/graphicsUtils",

    "esrith/amos/form/Select",
    "esrith/amos/form/Button"
], function (
    _WidgetBase,

    template,
    xstyle,

    declare,
    lang,
    array,
    on,
    dom,
    domClass,
    domConstruct,

    graphic,
    Point,
    PictureMarkerSymbol,
    SimpleMarkerSymbol,
    Draw,
    graphicsUtils
    ) {
    return declare([
        _WidgetBase
    ], {
        baseClass: "CustomerManagementBar",
        templateString: template,
        toolbar: null,
        sp: null,
        params: null,

        del_sp: null,

        sp_for_reload: null,
        params_for_reload: null,
        mode: null,

        _fromSave: false,

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);
            this.own(
                 on(this.selectBox, "change", lang.hitch(this, 'filterChange'))
                 , on(this.saveButton, "click", lang.hitch(this, 'save'))
                 , on(this.deleteButton, "click", lang.hitch(this, 'deleteNode'))
                 , on(this.selectBox, "bind-completed", lang.hitch(this, 'initSelectBox'))
                 //,on(this.map, "draw-complete", lang.hitch(this, 'setGeometry'))
            );
        },
        destroy: function () {
            this.reset();
        },
        reset: function () {
            this.clearGraphic();
            this.drawDeactive();
            this.hide(this.buttonPanel);
            this.selectBox.reset();
        },
        hidden: function () {
            this.reset();
            this.hide(this.customManagementNode);
        },
        start: function (sp, params) {
            this.reset();
            this.sp_for_reload = sp;
            this.params_for_reload = params;
            this.mode = params.MODE;

            // โชว์กล่องปุ่ม save delete
            this.show(this.customManagementNode);
            //if (params.MODE == 'METERING') {
            //    this.hide(this.saveButton);
            //    this.selectBox.bind(this.reqSP(sp, params));
            //} else {
            //    // customer ให้โชว์ปุ่ม save
            //    this.show(this.saveButton);
            //    this.selectBox.bind(this.reqSP(sp, params));
            //}
            this.selectBox.bind(this.reqSP(sp, params));
        },
        initSelectBox: function (result) {
            console.log("initSelectBox", result);
            //## Nannie add(20-7-2018) : set default selected
            if (this.mode == "METERING") {
                if (result.length > 0) {
                    this.show(this.selectBox);
                    var selectedValue = result[0].ID;
                    this.selectBox.set('value', selectedValue);
                    this.drawActive();          // selected value already then active draw tools
                }
                else {
                    // Mode Add
                    this.hide(this.selectBox);
                    this.params.MODE = 'ADD';
                    this.drawActive();
                }
            }
            else {
                this.show(this.selectBox);
                //this.selectBox.set('value', null);
            }

            if (!this._fromSave) {
                this._zoomToItems(result);
            }

            this._fromSave = false;
        },
        _zoomToItems: function (result) {
            try {
                this.displayLoader(true);
                if (result.length > 0) {
                    var arrPoint = [];
                    if (result.length == 1) {
                        if (result[0].GEOMETRY) {
                            var geo = this.geometry.fromST(result[0].GEOMETRY);
                            this.addGraphic(geo);

                            this.map.resize(true);
                            this.map.reposition();

                            this.map.centerAndZoom(this.geometry.fromST(result[0].GEOMETRY), 19);
                        }
                    } else if (result.length > 1) {
                        array.forEach(result, lang.hitch(this, function (r) {
                            if (r.GEOMETRY) {
                                var geometry = this.geometry.fromST(r.GEOMETRY);
                                this.addGraphic(geometry);          // highlight geometry that first extent

                                var gp = new graphic(geometry);
                                arrPoint.push(gp);
                            }
                        }));
                        //console.log("arrPoint", arrPoint);
                        if (arrPoint.length > 0) {
                            var extent = graphicsUtils.graphicsExtent(arrPoint);
                            //console.log("extent", extent);

                            this.map.setExtent(extent.expand(2), true);
                        }
                    }
                }
            } catch (e) {
                console.error("_zoomToItems", e);
                this.displayLoader(false);
            }
        },
        // also set sp for update and delete
        filterChange: function () {
            this.params = {};
            this.sp = null;
            this.del_sp = null;
            //console.log(this.selectBox.item);
            this.show(this.buttonPanel);
            this.clearGraphic();

            if (this.selectBox.item != undefined) {
                // set mode
                if (this.selectBox.item.MODE == 'ENTRANCE') {
                    this.sp = 'CUS_U_ENTRANCE';
                    this.del_sp = 'CUS_D_ENTRANCE';
                    this.params.SHIP_TO = this.selectBox.item.ID;
                } else if (this.selectBox.item.MODE == 'OFFICE') {
                    this.sp = 'CUS_U_OFFICE';
                    this.del_sp = 'CUS_D_OFFICE';
                    this.params.SOLD_TO = this.selectBox.item.ID;
                } else if (this.selectBox.item.MODE == 'METER') {
                    this.sp = 'CUS_U_METER';
                    this.del_sp = 'CUS_D_METER';
                    var paramSpilt = this.selectBox.item.ID.split('^');
                    this.params.SHIP_TO = paramSpilt[0];
                    this.params.METER_NUMBER = paramSpilt[1];
                }

                // ถ้าไม่มี geo มา จะเป็นโหมด ADD
                if (this.selectBox.item.GEOMETRY != undefined) {
                    this.params.MODE = 'UPDATE';
                    this.zoomTo();
                } else {
                    this.params.MODE = 'ADD';
                    this.drawActive();
                }
            } else {
                this.drawDeactive();
            }
        },
        zoomTo: function () {
            var geometry = this.geometry.fromST(this.selectBox.item.GEOMETRY);
            var point = new Point(geometry);

            this.map.resize(true);
            this.map.reposition();

            this.map.centerAndZoom(point, 19).then(lang.hitch(this, function (defer) {
                this.addGraphic(point);
                this.drawActive();
            }));
        },
        addGraphic: function (shape) {
            var marker = null;
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
        drawActive: function () {
            //this.drawDeactive();
            if (this.toolbar == null) {
                this.toolbar = new Draw(this.map);
            }
            this.toolbar.activate(Draw.POINT);
            this.toolbar.on("draw-end", lang.hitch(this, this.updateGeometry));
        },
        drawDeactive: function () {
            if (this.toolbar != null) {
                this.toolbar.deactivate();
            }
        },
        updateGeometry: function (e) {
            this.params.SHAPE = this.geometry.toST(e.geometry);
            //console.log('draw-end', e, this.sp, this.params);
            this.clearGraphic();
            this.addGraphic(e.geometry);
        },
        save: function () {
            //this.toolbar.deactivate();
            this.reqSP(this.sp, this.params).query(lang.hitch(this, function (result) {
                this.alert(result.message);
                this.clearGraphic();
                console.log(result);
                window['map'] = this.map;
                // reload filter data
                this._fromSave = true;
                this.addGraphic(this.geometry.fromST(this.params.SHAPE));
                this.selectBox.bind(this.reqSP(this.sp_for_reload, this.params_for_reload));
                this.map.getLayer('PTTOUR_DATA').refresh();
            }));
        },
        deleteNode: function () {
            delete this.params.SHAPE;
            delete this.params.MODE;
            this.toolbar.deactivate();
            this.confirm("ยืนยันการลบตำแหน่ง?", lang.hitch(this, function (confirm) {
                if (confirm) {
                    this.reqSP(this.del_sp, this.params).query(lang.hitch(this, function (result) {
                        this.alert(result.message);
                        this.clearGraphic();
                        console.log(result);
                        this.reset();

                        // reload filter data
                        this.selectBox.bind(this.reqSP(this.sp_for_reload, this.params_for_reload));
                        window['map'] = this.map;
                        this.map.getLayer('PTTOUR_DATA').refresh();
                    }));
                }
            }));


        }
    });
});
