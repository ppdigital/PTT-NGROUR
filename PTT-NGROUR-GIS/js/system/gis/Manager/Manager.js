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
    "system/gis/Tools/SwitchBasemap/SwitchBasemap",
    "system/gis/Tools/PrintMap/PrintMap",
    "system/gis/Tools/Measurement/Measurement",

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
    TOC,
    SwitchBasemap,
    PrintMap,
    Measurement
    ) {
    return declare([
        _WidgetBase
    ], {

        configures: null,
        viewport: null,

        //Add By Nattawit.kr 2018/06/13
        _wgMeasurement: null,
        //End Add

        _panelPrevious: null,
        _functionPrevious: null,

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
            //this._getMISData();
        },

        functionSelector: null,
        _getMISData: function () {
            window.addEventListener('message', lang.hitch(this, function (event) {
                console.log("receive mis data :", event.data);
                //this.reset();

                var misData = event.data;
                //top.map_info_json = null;

                if (this.functionSelector == null) {
                    this.functionSelector = new FunctionSelector()
                }
                this.functionSelector.select(misData);

                //console.log("event.origin", event.origin);
                //// IMPORTANT: Check the origin of the data! 
                //if (~event.origin.indexOf('http://localhost')) {
                //    // The data has been sent from your site 

                //    // The data sent with postMessage is stored in event.data 
                //    console.log("data",event.data);
                //} else {
                //    console.log("not found");
                //    // The data hasn't been sent from your site! 
                //    // Be careful! Do not use it. 
                //    return;
                //}
            }));
            //var tid = setInterval(lang.hitch(this, function () {
            //    try {
            //        if (top.map_info_json != null) {
            //            console.log(top.map_info_json);

            //            var misData = top.map_info_json;
            //            top.map_info_json = null;

            //            if (this.functionSelector == null) {
            //                this.functionSelector = new FunctionSelector()
            //            }
            //            this.functionSelector.select(misData);
            //            //this._selectComand(misData);
            //        }
            //    } catch (e) {
            //        console.error(e);
            //    }
            //}), 100);
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
            //this._getMISData();
        },

        _initMapReadyEvent: function () {
            this.own(
                //on(this.viewport, "open-systems", lang.hitch(this, "_openSystems")),
                on(this.viewport, "open-measurement", lang.hitch(this, "_openMeasurement")),
                on(this.viewport, "open-toc", lang.hitch(this, "_openTOC")),
                on(this.viewport, "open-misdata", lang.hitch(this, "_openMisData")),
                on(this.viewport, "open-function-1", lang.hitch(this, "_openFunction1")),
                on(this.viewport, "open-function-2", lang.hitch(this, "_openFunction2")),
                on(this.viewport, "open-gotoxy", lang.hitch(this, "_openGotoXY")),
                on(this.viewport, "open-basemap", lang.hitch(this, "_openBasemap")),
                on(this.viewport, "open-printmap", lang.hitch(this, "_openPrintMap"))
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

        _openTOC_RightPanel: function (e) {
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

        _toc: null,
        _panelTOC: null,
        _openTOC: function () {
            var toc;
            this.viewport.bottomPanel.toggleMinimization(true);
            this.viewport.rightPanel.toggleMinimization(true);
            if (!this._panelTOC) {
                this._panelTOC = new AwesomePanel({
                    title: "เครื่องมือแสดงรายการชั้นข้อมูล",
                    minimizeButtonVisibility: false,
                    maximizeButtonVisibility: false
                });
                domClass.add(this._panelTOC.domNode, "toc");
                this._panelTOC.toggleVisibility(false);
                this.own(on(this._panelTOC, "visibility-change", lang.hitch(this, function (e) {
                    domClass.toggle(this._panelTOC.domNode, "visible");

                    if (e.visible) {
                        if (this._functionPrevious != "toc") {
                            this._closePanelPrevious();
                        }

                        domConstruct.place(this._panelTOC.domNode, this.viewport.mapContainerNode);
                        setTimeout(lang.hitch(this, function () {
                            domClass.add(this._panelTOC.domNode, "visible");
                        }), 0);

                        this._panelPrevious = this._panelTOC;
                        this._functionPrevious = "toc";
                    }
                    else {
                        domClass.remove(this._panelTOC.domNode, "visible");
                        setTimeout(lang.hitch(this, function () {
                            this.viewport.mapContainerNode.removeChild(this._panelTOC.domNode);
                        }), ANIMATION_DURATION);

                        this._panelPrevious = null;
                    }
                })));

                this._toc = new TOC();
                this._panelTOC.place([this._toc.domNode]);
            }

            this._panelTOC.toggleVisibility(true);
            //console.log(this._panelGotoXY);
        },

        _panelMeasurement: null,
        _openMeasurement: function (e) {

            this.viewport.bottomPanel.toggleMaximization(true);
            this.viewport.rightPanel.toggleMaximization(true);
            if (!this._panelMeasurement) {
                this._panelMeasurement = new AwesomePanel({
                    title: "เครื่องมือวัด",
                    minimizeButtonVisibility: false,
                    maximizeButtonVisibility: false
                });
                domClass.add(this._panelMeasurement.domNode, "measurement");
                this._panelMeasurement.toggleVisibility(false);
                this.own(on(this._panelMeasurement, "visibility-change", lang.hitch(this, function (e) {
                    domClass.toggle(this._panelMeasurement.domNode, "visible");
                    if (e.visible) {
                        if (this._functionPrevious != "measurement") {
                            this._closePanelPrevious();
                        }

                        domConstruct.place(this._panelMeasurement.domNode, this.viewport.mapContainerNode);
                        setTimeout(lang.hitch(this, function () {
                            domClass.add(this._panelMeasurement.domNode, "visible");
                        }), 0);

                        this._panelPrevious = this._panelMeasurement;
                        this._functionPrevious = "measurement";
                    }
                    else {
                        if (this._wgMeasurement) {
                            this._wgMeasurement.reset();
                        }

                        domClass.remove(this._panelMeasurement.domNode, "visible");
                        setTimeout(lang.hitch(this, function () {
                            this.viewport.mapContainerNode.removeChild(this._panelMeasurement.domNode);
                        }), ANIMATION_DURATION);

                        this._panelPrevious = null;
                    }
                })));

                //Add By Nattawit.kr 2018/06/13
                this._wgMeasurement = new Measurement({ map: this.map });
                this._panelMeasurement.place([this._wgMeasurement.domNode]);
                //End Add
            }

            this._panelMeasurement.toggleVisibility(true);
        },

        _gotoXY: null,
        _panelGotoXY: null,
        _openGotoXY: function (e) {

            var gotoXY;
            this.viewport.bottomPanel.toggleMinimization(true);
            this.viewport.rightPanel.toggleMinimization(true);
            if (!this._panelGotoXY) {
                this._panelGotoXY = new AwesomePanel({
                    title: "เครื่องมือค้นหาตำแหน่ง",
                    minimizeButtonVisibility: false,
                    maximizeButtonVisibility: false
                });
                domClass.add(this._panelGotoXY.domNode, "gotoXY");
                this._panelGotoXY.toggleVisibility(false);
                this.own(on(this._panelGotoXY, "visibility-change", lang.hitch(this, function (e) {
                    domClass.toggle(this._panelGotoXY.domNode, "visible");
                    if (e.visible) {
                        if (this._functionPrevious != "gotoXY") {
                            this._closePanelPrevious();
                        }

                        domConstruct.place(this._panelGotoXY.domNode, this.viewport.mapContainerNode);
                        setTimeout(lang.hitch(this, function () {
                            domClass.add(this._panelGotoXY.domNode, "visible");
                        }), 0);

                        this._panelPrevious = this._panelGotoXY;
                        this._functionPrevious = "gotoXY";
                    }
                    else {
                        gotoXY.clearValueAndGraphic();
                        domClass.remove(this._panelGotoXY.domNode, "visible");
                        setTimeout(lang.hitch(this, function () {
                            this.viewport.mapContainerNode.removeChild(this._panelGotoXY.domNode);
                        }), ANIMATION_DURATION);

                        this._panelPrevious = null;
                    }
                })));

                gotoXY = new GotoXY();
                this._panelGotoXY.place([gotoXY.domNode]);
            }

            this._panelGotoXY.toggleVisibility(true);
            //console.log(this._panelGotoXY);
        },

        _panelBasemap: null,
        _switchBasemap: null,
        _openBasemap: function (e) {

            var switchBasemap;
            this.viewport.bottomPanel.toggleMinimization(true);
            this.viewport.rightPanel.toggleMinimization(true);
            if (!this._panelBasemap) {
                this._panelBasemap = new AwesomePanel({
                    title: "เครื่องมือเปลี่ยนแผนที่ฐาน",
                    minimizeButtonVisibility: false,
                    maximizeButtonVisibility: false
                });
                domClass.add(this._panelBasemap.domNode, "basemap");
                this._panelBasemap.toggleVisibility(false);
                this.own(on(this._panelBasemap, "visibility-change", lang.hitch(this, function (e) {
                    domClass.toggle(this._panelBasemap.domNode, "visible");

                    if (e.visible) {
                        if (this._functionPrevious != "basemap") {
                            this._closePanelPrevious();
                        }

                        domConstruct.place(this._panelBasemap.domNode, this.viewport.mapContainerNode);
                        setTimeout(lang.hitch(this, function () {
                            domClass.add(this._panelBasemap.domNode, "visible");
                        }), 0);

                        this._panelPrevious = this._panelBasemap;
                        this._functionPrevious = "basemap";
                    }
                    else {
                        domClass.remove(this._panelBasemap.domNode, "visible");
                        setTimeout(lang.hitch(this, function () {
                            this.viewport.mapContainerNode.removeChild(this._panelBasemap.domNode);
                        }), ANIMATION_DURATION);

                        this._panelPrevious = null;
                    }
                })));

                this._switchBasemap = new SwitchBasemap();
                this._panelBasemap.place([this._switchBasemap.domNode]);
            }

            this._panelBasemap.toggleVisibility(true);
            //console.log(this._panelGotoXY);
        },

        _printMap: null,
        _panelPrintmap: null,
        _openPrintMap: function () {
            var printMap;
            this.viewport.bottomPanel.toggleMinimization(true);
            this.viewport.rightPanel.toggleMinimization(true);
            if (!this._panelPrintmap) {
                this._panelPrintmap = new AwesomePanel({
                    title: "เครื่องมือพิมพ์ภาพแผนที่",
                    minimizeButtonVisibility: false,
                    maximizeButtonVisibility: false
                });
                domClass.add(this._panelPrintmap.domNode, "print");
                this._panelPrintmap.toggleVisibility(false);
                this.own(on(this._panelPrintmap, "visibility-change", lang.hitch(this, function (e) {
                    domClass.toggle(this._panelPrintmap.domNode, "visible");

                    if (e.visible) {
                        if (this._functionPrevious != "printmap") {
                            this._closePanelPrevious();
                        }

                        domConstruct.place(this._panelPrintmap.domNode, this.viewport.mapContainerNode);
                        setTimeout(lang.hitch(this, function () {
                            domClass.add(this._panelPrintmap.domNode, "visible");
                        }), 0);

                        this._panelPrevious = this._panelPrintmap;
                        this._functionPrevious = "printmap";
                    }
                    else {
                        domClass.remove(this._panelPrintmap.domNode, "visible");
                        setTimeout(lang.hitch(this, function () {
                            this.viewport.mapContainerNode.removeChild(this._panelPrintmap.domNode);
                        }), ANIMATION_DURATION);

                        this._panelPrevious = null;
                    }
                })));

                printMap = new PrintMap();
                this._panelPrintmap.place([printMap.domNode]);
            }

            this._panelPrintmap.toggleVisibility(true);
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
        },

        _closePanelPrevious: function () {
            if (this._panelPrevious) {
                //this.viewport.mapContainerNode.removeChild(this._panelPrevious.domNode);
                this._panelPrevious.toggleVisibility(false);
            }
        }
    });
});