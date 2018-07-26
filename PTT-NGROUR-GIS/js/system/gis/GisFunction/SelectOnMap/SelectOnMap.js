define([
    "core/_WidgetBase",

    "dojo/text!./SelectOnMap.html",
    "xstyle/css!./SelectOnMap.css",

    "dojo/_base/declare",
    "dojo/_base/lang",
    //"dojo/_base/array",
    "dojo/on",
    //"dpjp/dom",
    //"dojo/dom-class",
	//"dojo/dom-construct"
    "esrith/amos/form/Button",
    "esri/toolbars/draw",
    "manager/ConfigManager",
    "esri/graphic",
    "esri/symbols/SimpleFillSymbol"
], function (
    _WidgetBase,

    template,
    xstyle,

    declare,
    lang,
    //array,
    on,
    //dom,
    //domClass,
    //domConstruct
    Button,
    Draw,
    ConfigManager,
    Graphic,
    SimpleFillSymbol
    ) {
    return declare([
        _WidgetBase
    ], {
        baseClass: "select-on-map",
        templateString: template,
        params: null,

        _toolbar: null,
        configures: null,
        _selected: null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
            //console.log("SelectOnMap@constructor");
        },
        postCreate: function () {
            this.inherited(arguments);
            //console.log("SelectOnMap@postCreate");
            this._toolbar = new Draw(this.map);

            this.own(
                on(this.btnSelect, "click", lang.hitch(this, "_btnSelect_Click")),
                on(this._toolbar, "draw-complete", lang.hitch(this, "_toolbar_DrawComplete"))
            );

            this.configures = ConfigManager.getInstance().getAllConfig();
            console.log("Select On Map this.map", this.map);
        },

        _btnSelect_Click: function () {
            this.map.graphics.clear();

            //## Nannie add(20-7-2018) : deactivate when already selected
            if (!this._selected) {
                this._toolbar.activate(Draw.EXTENT);
                this._selected = true;
            }
            else {
                this._toolbar.deactivate();
                this._selected = false;
            }

        },
        _toolbar_DrawComplete: function (results) {
            console.log("_toolbar_DrawComplete", results.geographicGeometry, results.geometry, results.target);
            this._toolbar.deactivate();
            var symbol = new SimpleFillSymbol(this.configures.mapConfig.defaultSymbol.polygon);
            this.map.graphics.add(new Graphic(results.geometry, symbol));

            var param = {
                GEOMETRY: this.geometry.toST(results.geometry)
            };
            console.log("UTL_Q_MAP_DATA", param);

            this.reqSP("UTL_Q_MAP_DATA", param).query(
                lang.hitch(this, "_query_Complete"),
                lang.hitch(this, "_query_Error"));

            //return to mis
        },
        _query_Complete: function (response) {
            console.log("_query_Complete", response);
            if (response.success) {
                //var dataGate = response.data;
                //var dataPipeline = response.data1;
                //var dataMeter = response.data2;

                top.initMapTable(response.data);
            }
        },
        _query_Error: function (error) {
            console.error("_query_Error", error);
            this.alert("การค้นหาข้อมูลบน Map มีปัญหา");
        }



    });
});