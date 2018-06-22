define([
    "core/_WidgetBase",

    "manager/ConfigManager",

    "system/gis/configs/GISConfig",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/query",
    "dojo/dom-class",
    "dojo/dom-construct",

    "system/gis/SystemSelection/SystemSelection",

    "widgets/AwesomeButton/AwesomeButton",
    "widgets/AwesomePanel/AwesomePanel",

    "system/gis/GisFunction/FunctionSelector/FunctionSelector",

    "system/gis/Tools/GotoXY/GotoXY",
    "system/gis/Tools/TOC/TOC",

    "xstyle/css!./../css/GIS.css"
], function (
    _WidgetBase,

    ConfigManager,

    GISConfig,

    declare,
    lang,
    array,
    on,
    query,
    domClass,
    domConstruct,

    SystemSelection,

    AwesomeButton,
    AwesomePanel,

    FunctionSelector,

    GotoXY,
    TOC
    ) {
    return declare([
        _WidgetBase
    ], {

        configures: null,
        viewport: null,

        //Add By Nattawit.kr 2018/06/13
        _wgMeasurement: null,
        //End Add

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);

            this.configures = ConfigManager.getInstance().getAllConfig();
            //console.log("this.configures: ", this.configures);

            this._initEvent();

            this._createMenu();
            //this.configures.mapConfig.option.basemap = "streets";
            this.viewport.createMap(this.configures.mapConfig);

            this._getMISData();
        },

        functionSelector: null,
        _getMISData: function () {
            var tid = setInterval(lang.hitch(this, function () {
                try {
                    if (top.misJSON != null) {
                        //console.log(top.misJSON);
                        //window.misData = top.misJSON;
                        var misData = top.misJSON;
                        top.misJSON = null;

                        if (this.functionSelector == null) {
                            this.functionSelector = new FunctionSelector()
                        }
                        this.functionSelector.select(misData);
                        //this._selectComand(misData);
                    }
                } catch (e) {
                    console.error(e);
                }
            }), 100);
        },
        //_selectComand: function (misData) {
        //    switch (misData.comand) {
        //        case "zoom": 
        //            break;
        //        case "search":
        //            break;
        //    }
        //},

        _initEvent: function () {
            this.own(
                on(this.viewport, "map-ready", lang.hitch(this, "_mapReady")),

                on(this.viewport, "menu-select", lang.hitch(this, "_menuSelect"))
                );
        },


        _mapReady: function () {
            this._initMapReadyEvent();
            this._createOnMapItem();
        },

        _initMapReadyEvent: function () {
            this.own(
                on(this.viewport, "open-routing", lang.hitch(this, "_openRouting")),
                on(this.viewport, "open-search", lang.hitch(this, "_openSearch")),
                on(this.viewport, "open-buffer", lang.hitch(this, "_openBuffer")),

                //on(this.viewport, "open-systems", lang.hitch(this, "_openSystems")),
                on(this.viewport, "open-measurement", lang.hitch(this, "_openMeasurement")),
                on(this.viewport, "open-toc", lang.hitch(this, "_openTOC")),
                on(this.viewport, "open-misdata", lang.hitch(this, "_openMisData")),
                on(this.viewport, "open-function-1", lang.hitch(this, "_openFunction1")),
                on(this.viewport, "open-function-2", lang.hitch(this, "_openFunction2")),
                on(this.viewport, "open-gotoxy", lang.hitch(this, "_openGotoXY"))
                );
        },

        _createMenu: function () {
            this.viewport.createMenuList(GISConfig.menus);
        },

        _createOnMapItem: function () {
            this.viewport.createOnMapItem(GISConfig.onMapItems);
        },

        _menuSelect: function (e) {
            query(".viewport-gis > .menu-panel > .container .item.selected").removeClass("selected");
            domClass.add(e.menuNode, "selected");
        },

        _openRouting: function (e) {
            this.viewport.toggleVisibilityMenu(false);
            this.viewport.bottomPanel.reset();
            this.viewport.bottomPanel.set("title", e.label);
            this.viewport.bottomPanel.place([domConstruct.create("div", {
                innerHTML: "Routing contents",
                style: {
                    width: "100%",
                    height: "100%",
                    backgroundColor: "#30b88b"
                }
            })]);
            this.viewport.bottomPanel.toggleVisibility(true);
        },

        _openSearch: function (e) {
            this.viewport.toggleVisibilityMenu(false);
            this.viewport.bottomPanel.reset();
            this.viewport.bottomPanel.set("title", e.label);
            var doms = [domConstruct.create("div", {
                innerHTML: "Search #1",
                style: {
                    width: "100%",
                    height: "100%",
                    backgroundColor: "#30b88b"
                }
            })];
            doms.push(domConstruct.create("div", {
                innerHTML: "Search #2",
                style: {
                    width: "100%",
                    height: "100%",
                    backgroundColor: "#30b88b"
                }
            }));
            doms.push(domConstruct.create("div", {
                innerHTML: "Search #3",
                style: {
                    width: "100%",
                    height: "100%",
                    backgroundColor: "#30b88b"
                }
            }));
            this.viewport.bottomPanel.place(doms);
            this.viewport.bottomPanel.toggleVisibility(true);
        },

        _openBuffer: function (e) {
            this.viewport.toggleVisibilityMenu(false);
            this.viewport.bottomPanel.reset();
            this.viewport.bottomPanel.set("backButtonVisibility", true);
            this.viewport.bottomPanel.set("title", e.label);
            require(["system/gis/_WIDGET_/_WIDGET_"], lang.hitch(this, function (_WIDGET_) {
                var wg = new _WIDGET_();
                this.viewport.bottomPanel.place([wg.domNode]);
            }));
            this.viewport.bottomPanel.toggleVisibility(true);
        },

        _systemSelection: null,
        //_openSystems: function (e) {
        //    if (this._systemSelection) {
        //        this._systemSelection.toggleVisibility(true);
        //    } else {
        //        this._systemSelection = new SystemSelection({
        //            systemItems: GISConfig.systemSelectionItems
        //        });
        //        this.own(
        //            on(this._systemSelection, "close", lang.hitch(this, function () {
        //                this._systemSelection.toggleVisibility(false);
        //            })),
        //            on(this._systemSelection, "system-logout", lang.hitch(this, function (e) {
        //                console.log("system-logout");
        //            })),
        //            on(this._systemSelection, "system-gis", lang.hitch(this, function (e) {
        //                this._systemSelection.toggleVisibility(false);
        //            })),
        //            on(this._systemSelection, "system-um", lang.hitch(this, function (e) {
        //                this._systemSelection.toggleVisibility(false);
        //            }))
        //        );
        //        domConstruct.place(this._systemSelection.domNode, document.body);
        //        this._systemSelection.toggleVisibility(true);
        //    }
        //},

        _panelMeasurement: null,
        _wgMeasurement: null,
        _openMeasurement: function (e) {
            this.viewport.bottomPanel.toggleMaximization(true);
            this.viewport.rightPanel.toggleMaximization(true);
            if (!this._panelMeasurement) {
                this._panelMeasurement = new AwesomePanel({
                    title: "Measurement",
                    minimizeButtonVisibility: false,
                    maximizeButtonVisibility: false
                });
                domClass.add(this._panelMeasurement.domNode, "measurement");
                this._panelMeasurement.toggleVisibility(false);
                this.own(on(this._panelMeasurement, "visibility-change", lang.hitch(this, function (e) {
                    domClass.toggle(this._panelMeasurement.domNode, "visible");
                    if (e.visible) {
                        domConstruct.place(this._panelMeasurement.domNode, this.viewport.mapContainerNode);
                        setTimeout(lang.hitch(this, function () {
                            domClass.add(this._panelMeasurement.domNode, "visible");
                        }), 0);
                    } else {
                        domClass.remove(this._panelMeasurement.domNode, "visible");
                        setTimeout(lang.hitch(this, function () {
                            this.viewport.mapContainerNode.removeChild(this._panelMeasurement.domNode);
                        }), ANIMATION_DURATION);
                    }
                })));

                //Add By Nattawit.kr 2018/06/13
                require(["system/gis/Tools/Measurement/Measurement"], lang.hitch(this, function (Measurement) {
                    this._wgMeasurement = new Measurement({ map: this.map });
                    this._panelMeasurement.place([this._wgMeasurement.domNode]);
                    this.own(on(this._panelMeasurement, "visibility-change", lang.hitch(this, function (e) {
                        if (e.visible == false) {
                            this._wgMeasurement.reset();
                        }
                    })));
                }));

                //this._panelMeasurement.place([domConstruct.create("div", {
                //    innerHTML: "Measurement contents",
                //    style: {
                //        width: "100%",
                //        height: "100%",
                //        backgroundColor: "#30b88b"
                //    }
                //})]);
                //End Add
            }
            this._panelMeasurement.toggleVisibility(true);
        },

        _openTOC: function (e) {
            this.viewport.rightPanel.reset();
            this.viewport.rightPanel.set("title", "TOC");
            var TOCComponent = new TOC();
            this.viewport.rightPanel.place([TOCComponent.domNode]);
            //this.viewport.rightPanel.place([domConstruct.create("div", {
            //    innerHTML: "TOC contents",
            //    style: {
            //        width: "100%",
            //        height: "100%",
            //        backgroundColor: "#30b88b"
            //    }
            //})]);
            this.viewport.rightPanel.toggleVisibility(true);
        },

        _openMisData: function (e) {
            console.log("window.misData", window.misData);
            console.log("fromST", this.geometry.fromST("LINESTRING  ( 11192934.95010000 1529791.92690000, 11192977.15520000 1529837.83040000, 11192978.54630000 1529836.72300000, 11192979.21220000 1529837.39950000)"));
            //console.log("fromST", this.geometry.fromST);
            this.viewport.rightPanel.reset();
            this.viewport.rightPanel.set("title", "MIS Data");
            this.viewport.rightPanel.place([domConstruct.create("div", {
                innerHTML: JSON.stringify(window.misData),
                style: {
                    width: "100%",
                    height: "100%",
                    backgroundColor: "#30b88b"
                }
            })]);
            this.viewport.rightPanel.toggleVisibility(true);
        },

        _panelGotoXY: null,
        _openGotoXY: function (e) {
            var gotoXY;
            this.viewport.bottomPanel.toggleMinimization(true);
            this.viewport.rightPanel.toggleMinimization(true);
            if (!this._panelGotoXY) {
                this._panelGotoXY = new AwesomePanel({
                    title: "Goto XY",
                    minimizeButtonVisibility: false,
                    maximizeButtonVisibility: false
                });
                domClass.add(this._panelGotoXY.domNode, "gotoXY");
                this._panelGotoXY.toggleVisibility(false);
                this.own(on(this._panelGotoXY, "visibility-change", lang.hitch(this, function (e) {
                    domClass.toggle(this._panelGotoXY.domNode, "visible");
                    if (e.visible) {
                        domConstruct.place(this._panelGotoXY.domNode, this.viewport.mapContainerNode);
                        setTimeout(lang.hitch(this, function () {
                            domClass.add(this._panelGotoXY.domNode, "visible");
                        }), 0);
                    } else {
                        gotoXY.clearValueAndGraphic();
                        domClass.remove(this._panelGotoXY.domNode, "visible");
                        setTimeout(lang.hitch(this, function () {
                            this.viewport.mapContainerNode.removeChild(this._panelGotoXY.domNode);
                        }), ANIMATION_DURATION);
                    }
                })));

                gotoXY = new GotoXY();
                this._panelGotoXY.place([gotoXY.domNode]);
            }
            this._panelGotoXY.toggleVisibility(true);
            //console.log(this._panelGotoXY);
        },

        _openFunction1: function (e) {
            this.viewport.bottomPanel.reset();
            this.viewport.bottomPanel.set("title", "Function #1");
            this.viewport.bottomPanel.place([domConstruct.create("div", {
                innerHTML: "Function #1 contents 1",
                style: {
                    width: "100%",
                    height: "100%",
                    backgroundColor: "#30b88b"
                }
            }), domConstruct.create("div", {
                innerHTML: "Function #1 contents 2",
                style: {
                    width: "100%",
                    height: "100%",
                    backgroundColor: "#30b88b"
                }
            })]);
            this.viewport.bottomPanel.toggleVisibility(true);
        },

        _openFunction2: function (e) {
            this.viewport.bottomPanel.reset();
            this.viewport.bottomPanel.set("title", "Function #2");
            this.viewport.bottomPanel.place([domConstruct.create("div", {
                innerHTML: "Function #2 contents",
                style: {
                    width: "100%",
                    height: "100%",
                    backgroundColor: "#30b88b"
                }
            })]);
            this.viewport.bottomPanel.toggleVisibility(true);
        }
    });
});