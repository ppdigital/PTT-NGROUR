define([
    "core/_WidgetBase",

    "dojo/text!./Viewport.html",
    "xstyle/css!./Viewport.css",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/query",
    "dojo/dom-class",
    "dojo/dom-style",
    "dojo/dom-construct",
    "dojo/dom-geometry",

    "manager/MapManager",

    "system/gis/Manager/Manager",

    "widgets/AwesomePanel/AwesomePanel",
    "widgets/AwesomeButton/AwesomeButton",

    "system/gis/Tools/CoordinateBar/CoordinateBar"
], function (
    _WidgetBase,

    template,
    xstyle,

    declare,
    lang,
    array,
    on,
    query,
    domClass,
    domStyle,
    domConstruct,
    domGeom,

    MapManager,

    Manager,

    AwesomePanel,
    AwesomeButton,

    CoordinateBar
    ) {
    return declare([
        _WidgetBase
    ], {
        baseClass: "viewport-gis",
        templateString: template,

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);

            this._initGlobal();

            this._initEvent();
            this._windowResize();

            this._createBottomPanel();
            this._createRightPanel();
            this._createMeasurementPanel();

            new Manager({
                viewport: this
            });

            this.TEST();
        },

        _initGlobal: function () {
            getScreenSize = function () {
                return {
                    w: window.innerWidth || document.body.clientWidth,
                    h: window.innerHeight || document.body.clientHeight
                }
            }
            ANIMATION_DURATION = 200;
            SIZE_SMALL = 768;
            //SIZE_MEDIUM = 1280;
            SIZE_MEDIUM = 1100;
        },

        TEST: function () {
            this.own(
                );
        },

        _windowResize: function () {
            if (getScreenSize().w > SIZE_SMALL) {
                domConstruct.place(this.rightPanelNode, this.mainContainerWrapperNode, "first");
            } else {
                domConstruct.place(this.rightPanelNode, this.mainContainerWrapperNode, "last");
            }
            this._reCalculatePanelSize();
        },

        _initEvent: function () {
            this.own(
                on(window, "resize", lang.hitch(this, "_windowResize")),
                on(this.btnToggleMenuPanel, "click", lang.hitch(this, "toggleVisibilityMenu")),
                on(this.dimNode, "click", lang.hitch(this, function (e) {
                    e.stopPropagation();
                    this.toggleVisibilityMenu(false);
                })),
                on(this.mapNode, "load", lang.hitch(this, function (e) {
                    console.log(e);
                }))
            );
        },

        createMenuList: function (menuConfig) {
            array.forEach(menuConfig, lang.hitch(this, function (item) {
                this._appendMenu(item);
            }));
        },

        _appendMenu: function (menu) {
            if (menu.isHeader) {
                domConstruct.create("div", {
                    className: "header " + (menu.className || ""),
                    innerHTML: menu.label || ""
                }, this.menuContainer);
            } else if (menu.isDivider) {
                domConstruct.create("div", {
                    className: "divider " + (menu.className || "")
                }, this.menuContainer);
            } else {
                var menuNode = domConstruct.create("div", {
                    className: "item " + (menu.className || "")
                }, this.menuContainer);
                domConstruct.create("div", {
                    className: "icon " + (menu.iconClassName || ""),
                    style: {
                        backgroundImage: menu.iconUrl ? "url(" + menu.iconUrl + ")" : ""
                    }
                }, menuNode);
                var labelNode = domConstruct.create("div", {
                    className: "label",
                    innerHTML: menu.label || ""
                }, menuNode);
                array.forEach(menu.events, lang.hitch(this, function (event) {
                    this._addEvent(menuNode, event.action, event.name, menu);
                }));
                if (menu.subMenus && menu.subMenus.length > 0) {
                    var subMenuContainerNode = domConstruct.create("div", {
                        className: "sub-menu-container"
                    }, labelNode);
                    array.forEach(menu.subMenus, lang.hitch(this, function (item) {
                        this._appendSubMenu(item, subMenuContainerNode);
                    }));
                    this.own(on(menuNode, "click", lang.hitch(this, function (e) {
                        domClass.toggle(subMenuContainerNode, "visible");
                    })));
                } else {
                    this.own(on(menuNode, "click", lang.hitch(this, "_menuSelect", menu, menuNode)));
                }
            }
        },

        _appendSubMenu: function (subMenu, subMenuContainerNode) {
            var subMenuNode = domConstruct.create("div", {
                className: "item " + (subMenu.className || "")
            }, subMenuContainerNode);
            domConstruct.create("div", {
                className: "icon " + (subMenu.iconClassName || ""),
                style: {
                    backgroundImage: subMenu.iconUrl ? "url(" + subMenu.iconUrl + ")" : ""
                }
            }, subMenuNode);
            domConstruct.create("div", {
                className: "label",
                innerHTML: subMenu.label || ""
            }, subMenuNode);
            array.forEach(subMenu.events, lang.hitch(this, function (event) {
                this._addEvent(subMenuNode, event.action, event.name, subMenu);
            }));
            this.own(on(subMenuNode, "click", lang.hitch(this, "_menuSelect", subMenu, subMenuNode)));
        },

        _menuSelect: function (menu, menuNode, e) {
            this.emit("menu-select", {
                target: this,
                menu: menu,
                menuNode: menuNode
            });
            e.stopPropagation();
        },

        createOnMapItem: function (itemConfig) {
            console.log(itemConfig);
            array.forEach(itemConfig, lang.hitch(this, function (item) {
                this._appendOnMapItem(item);
            }));
        },

        _appendOnMapItem: function (onMapItem) {
            var wg = new AwesomeButton(onMapItem);
            domStyle.set(wg.domNode, onMapItem.style);
            this.own(on(wg, "sub-item-click", lang.hitch(this, function () {
                wg.toggleVisibilitySubItemContainer(false);
            })));
            domConstruct.place(wg.domNode, this.mapContainerNode);
            array.forEach(onMapItem.events, lang.hitch(this, function (event) {
                this._addEvent(wg.domNode, event.action, event.name, onMapItem);
            }));
            array.forEach(onMapItem.subItems, lang.hitch(this, function (subItem) {
                array.forEach(subItem.events, lang.hitch(this, function (event) {
                    this.own(on(wg, event.name, lang.hitch(this, "_onMapItemSubItemEventForward", event.name)));
                }));
            }));
        },

        _onMapItemSubItemEventForward: function (name, e) {
            this.emit(name, e);
        },

        _coordinateBarPanel: null,
        createMap: function (mapConfig) {
            var map = new MapManager(mapConfig, this.mapNode);
            this.own(on(map, "load", lang.hitch(this, function (obj) {
                //console.log("obj: ", obj);

                _coordinateBarPanel = new CoordinateBar();
                domConstruct.place(_coordinateBarPanel.domNode, this.coordinateBarNode);

                this.emit("map-ready", {
                    target: this
                });


            })));
        },

        bottomPanel: null,
        _createBottomPanel: function () {
            domConstruct.empty(this.bottomPanelContainerNode);
            this.bottomPanel = new AwesomePanel();
            this.bottomPanel.toggleVisibility(false);
            this.own(
                on(this.bottomPanel, "minimization-change", lang.hitch(this, function (e) {
                    //console.log("minimization-change: ", e);
                    if (e.minimize) {
                        domClass.add(this.bottomPanelNode, "minimize");
                        domStyle.set(this.bottomPanelCellNode, "height", ((100 / domGeom.position(this.contentPanelNode).h) * 40) + "%");
                    } else {
                        domClass.remove(this.bottomPanelNode, "minimize");
                        domStyle.set(this.bottomPanelCellNode, "height", "");
                    }
                    this._reCalculatePanelSize();
                })),
                on(this.bottomPanel, "maximization-change", lang.hitch(this, function (e) {
                    //console.log("maximization-change: ", e);
                    if (e.maximize) {
                        domClass.add(this.bottomPanelNode, "maximize");
                    } else {
                        domClass.remove(this.bottomPanelNode, "maximize");
                    }
                })),
                on(this.bottomPanel, "visibility-change", lang.hitch(this, function (e) {
                    //console.log("visibility-change: ", e);
                    domClass.remove(this.bottomPanelNode, "hide");
                    setTimeout(lang.hitch(this, function () {
                        if (e.visible) {
                            domClass.add(this.contentPanelNode, "show-bottom-panel");
                            domClass.add(this.bottomPanelNode, "visible");
                        } else {
                            domClass.remove(this.bottomPanelNode, "visible");
                            domStyle.set(this.bottomPanelCellNode, "height", "");
                            domClass.remove(this.bottomPanelNode, "minimize");
                            domClass.remove(this.bottomPanelNode, "maximize");
                            if (getScreenSize().w > SIZE_MEDIUM) {
                                domClass.remove(this.contentPanelNode, "show-bottom-panel");
                                domClass.add(this.bottomPanelNode, "hide");
                            } else {
                                setTimeout(lang.hitch(this, function () {
                                    domClass.remove(this.contentPanelNode, "show-bottom-panel");
                                    domClass.add(this.bottomPanelNode, "hide");
                                }), ANIMATION_DURATION);
                            }
                        }
                        this._reCalculatePanelSize();
                    }), 50);
                })),
                on(this.bottomPanel, "reset", lang.hitch(this, function (e) {
                    domStyle.set(this.bottomPanelCellNode, "height", "");
                    domClass.remove(this.bottomPanelNode, "minimize");
                    domClass.remove(this.bottomPanelNode, "maximize");
                }))
                );
            domConstruct.place(this.bottomPanel.domNode, this.bottomPanelContainerNode);
        },

        rightPanel: null,
        _createRightPanel: function () {
            domConstruct.empty(this.rightPanelContainerNode);
            this.rightPanel = new AwesomePanel();
            this.rightPanel.toggleVisibility(false);
            this.own(
                on(this.rightPanel, "minimization-change", lang.hitch(this, function (e) {
                    //console.log("minimization-change: ", e);
                    if (e.minimize) {
                        domClass.add(this.rightPanelNode, "minimize");
                        domStyle.set(this.rightPanelCellNode, "height", ((100 / domGeom.position(this.mainContainerWrapperNode).h) * 40) + "%");
                    } else {
                        domClass.remove(this.rightPanelNode, "minimize");
                        domStyle.set(this.rightPanelCellNode, "height", "");
                    }
                    this._reCalculatePanelSize();

                })),
                on(this.rightPanel, "maximization-change", lang.hitch(this, function (e) {
                    //console.log("maximization-change: ", e);
                    if (e.maximize) {
                        domClass.add(this.rightPanelNode, "maximize");
                    } else {
                        domClass.remove(this.rightPanelNode, "maximize");
                    }
                    this._reCalculatePanelSize();
                })),
                on(this.rightPanel, "visibility-change", lang.hitch(this, function (e) {
                    //console.log("visibility-change: ", e);
                    domClass.remove(this.rightPanelNode, "hide");
                    setTimeout(lang.hitch(this, function () {
                        if (e.visible) {
                            domClass.add(this.rightPanelNode, "visible");
                        } else {
                            domClass.remove(this.rightPanelNode, "visible");
                            domStyle.set(this.rightPanelCellNode, "height", "");
                            domClass.remove(this.rightPanelNode, "minimize");
                            domClass.remove(this.rightPanelNode, "maximize");
                            setTimeout(lang.hitch(this, function () {
                                domClass.add(this.rightPanelNode, "hide");
                            }), ANIMATION_DURATION);
                        }
                        this._reCalculatePanelSize();
                    }), 50);

                })),
                on(this.rightPanel, "reset", lang.hitch(this, function (e) {
                    domStyle.set(this.rightPanelCellNode, "height", "");
                    domClass.remove(this.rightPanelNode, "minimize");
                    domClass.remove(this.rightPanelNode, "maximize");
                }))
                );
            domConstruct.place(this.rightPanel.domNode, this.rightPanelContainerNode);
        },

        _looper: null,
        _reCalculatePanelSize: function () {
            this._looper = null;
            var width = getScreenSize().w;
            setTimeout(lang.hitch(this, function () {
                var h = domGeom.position(this.contentPanelNode).h;
                if (width > 1101) {
                    domStyle.set(this.mapNode, "height", h + "px");
                } else {
                    if (domClass.contains(this.bottomPanelNode, "minimize")) {
                        domStyle.set(this.bottomPanelCellNode, "height", ((100 / h) * 40) + "%");
                        domStyle.set(this.mapNode, "height", (h - 40) + "px");
                    } else if (domClass.contains(this.bottomPanelNode, "visible")) {
                        domStyle.set(this.mapNode, "height", (h / 2) + "px");
                    } else {
                        domStyle.set(this.mapNode, "height", h + "px");
                    }
                }
            }), ANIMATION_DURATION);
            var timer = 0;
            this._looper = lang.hitch(this, function () {
                if (this._looper && timer <= ANIMATION_DURATION) {
                    if (width > SIZE_SMALL) {
                        domStyle.set(this.contentPanelWrapperNode, "height", "");
                    } else {
                        var h = domGeom.position(this.mainContainerNode).h;
                        if (domClass.contains(this.rightPanelNode, "minimize")) {
                            domStyle.set(this.contentPanelWrapperNode, "height", (h - 40) + "px");
                        } else if (domClass.contains(this.rightPanelNode, "visible")) {
                            domStyle.set(this.contentPanelWrapperNode, "height", (h / 2) + "px");
                        } else {
                            domStyle.set(this.contentPanelWrapperNode, "height", h + "px");
                        }
                    }
                    timer += 10;
                    setTimeout(lang.hitch(this, this._looper), 10);
                } else {
                    this._looper = null;
                }
            });
            this._looper();
        },

        measurementPanel: null,
        _createMeasurementPanel: function () {
            domConstruct.empty(this.measurementPanelContainerNode);
            this.measurementPanel = new AwesomePanel();
            domClass.add(this.measurementPanel.domNode, 'measurement');
            this.measurementPanel.toggleVisibility(false);
            this.own(
                on(this.measurementPanel, "minimization-change", lang.hitch(this, function (e) {
                    ////console.log("minimization-change: ", e);
                    if (e.minimize) {
                        domClass.add(this.measurementPanelNode, "minimize");
                        domStyle.set(this.measurementPanelCellNode, "height", ((100 / domGeom.position(this.contentPanelNode).h) * 48) + "%");
                    } else {
                        domClass.remove(this.measurementPanelNode, "minimize");
                        domStyle.set(this.measurementPanelCellNode, "height", "");
                    }
                    this._reCalculatePanelSize();
                })),
                on(this.measurementPanel, "maximization-change", lang.hitch(this, function (e) {
                    ////console.log("maximization-change: ", e);
                    if (e.maximize) {
                        domClass.add(this.measurementPanelNode, "maximize");
                        this.bottomPanel.toggleMinimization(true);
                        this.printPanel.toggleMinimization(true);
                    } else {
                        domClass.remove(this.measurementPanelNode, "maximize");
                    }
                })),
                on(this.measurementPanel, "visibility-change", lang.hitch(this, function (e) {
                    ////console.log("visibility-change: ", e);
                    domClass.remove(this.measurementPanelNode, "hide");
                    setTimeout(lang.hitch(this, function () {
                        if (e.visible) {
                            //domClass.add(this.contentPanelNode, "show-bottom-panel");
                            domClass.add(this.measurementPanelNode, "visible");
                            query(".awesome-button > .wrapper .button.measurement").addClass("active");
                            query(".awesome-button > .wrapper > .sub-item-container .button.measurement").addClass("active");
                        } else {
                            domClass.remove(this.measurementPanelNode, "visible");
                            domStyle.set(this.measurementPanelCellNode, "height", "");
                            domClass.remove(this.measurementPanelNode, "minimize");
                            domClass.remove(this.measurementPanelNode, "maximize");
                            if (getScreenSize().w > SIZE_MEDIUM) {
                                //domClass.remove(this.contentPanelNode, "show-bottom-panel");
                                domClass.add(this.measurementPanelNode, "hide");
                            } else {
                                setTimeout(lang.hitch(this, function () {
                                    //domClass.remove(this.contentPanelNode, "show-bottom-panel");
                                    domClass.add(this.measurementPanelNode, "hide");
                                }), ANIMATION_DURATION);
                            }
                            query(".awesome-button > .wrapper .button.measurement.active").removeClass("active");
                            query(".awesome-button > .wrapper > .sub-item-container .button.measurement.active").removeClass("active");
                        }
                        this._reCalculatePanelSize();
                    }), 50);
                })),
                on(this.measurementPanel, "reset", lang.hitch(this, function (e) {
                    domStyle.set(this.measurementPanelCellNode, "height", "");
                    domClass.remove(this.measurementPanelNode, "minimize");
                    domClass.remove(this.measurementPanelNode, "maximize");
                }))
                );
            domConstruct.place(this.measurementPanel.domNode, this.measurementPanelContainerNode);
        },

        toggleVisibilityMenu: function (visibility) {
            if (typeof (visibility) !== "boolean") {
                visibility = null;
            }
            if (visibility === false || (visibility === null && domClass.contains(this.domNode, "show-menu-panel"))) {
                domClass.remove(this.domNode, "show-menu-panel");
            } else {
                domClass.add(this.domNode, "show-menu-panel");
            }
        }
    });
});