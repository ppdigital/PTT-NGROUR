define([
    "core/_ClassBase",
    "./LayoutManager",
    "./ApplicationManager",
    "./ConfigManager",
    "./SessionManager",

    "dojo/aspect",
    "dojo/store/Memory",
    "dojo/promise/all",
    "dojo/Deferred",
    "dojo/dom-style",
    "dojo/dom-construct",
    "dojo/on",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    _ClassBase,
    LayoutManager,
    ApplicationManager,
    ConfigManager,
    SessionManager,

    aspect,
    Memory,
    all,
    Deferred,
    domStyle,
    domConstruct,
    on,
    lang,
    declare,
    require
    ) {

    var instance = null;
    var wgConstant = {
        creates: new Memory({ data: [], idProperty: "systemId" }),
        getInstance: function (systemId) {
            return wgDeclare.creates.get(systemId);
        }
    };

    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
        _ClassBase
    ], {

        loadedWidget: null,

        constructor: function (params) {
            params = params || {};
            lang.mixin(this, params);
            wgDeclare.creates.add(this);
            this.loadedWidget = new Memory({ data: [], idProperty: "name" });
        },
        getWidget: function (config) {
            return (this.loadedWidget.get(config.name) || {}).widget;
        },
        getWidgetByName: function (widgetName) {
            return (this.loadedWidget.get(widgetName) || {}).widget;
        },
        loadWidget: function (config) {
            var deferred = new Deferred();
            require([
                //config.content + "/" + config.content.substring(config.content.lastIndexOf("/") + 1)
                config.content
            ], lang.hitch(this, function (
                Widget
                ) {
                deferred.resolve(Widget);
            }));
            return deferred.promise;
        },
        createWidget: function (config) {
            var openedWidget = this.getWidget(config);


            if (openedWidget) {
                var deferred = new Deferred();

                //Bank 3June2016
                if (config.hasOwnProperty('panelName')) {
                    deferred.resolve({
                        panel: openedWidget.getPanel(),
                        widget: openedWidget
                    });
                }
                else {
                    deferred.resolve({
                        panel: null,
                        widget: openedWidget
                    });
                    openedWidget.startup();
                }
                /////////////////

                return deferred.promise;
            } else {
                var deferredObject = {};
                deferredObject.panel = this.panelManager.createPanelByName(config.panelName);
                deferredObject.widget = this.loadWidget(config);
                var domTarget = {};
                if (config.position) {
                    domTarget = this.layoutManager.createTargetByConfig(config);
                }
                return all(deferredObject).then(lang.hitch(this, function (result) {
                    var params = this._constructMixin(result);
                    lang.mixin(params, config.options || {});
                    if (domTarget) {
                        domStyle.set(domTarget, config.style);
                    }
                    var widget = new result.widget(params, domTarget);
                    aspect.after(widget, "destroy", lang.hitch(this, function (argReturn) {
                        this.loadedWidget.remove(config.name);
                        return argReturn;
                    }));
                    if (result.panel) {
                        result.panel.set({
                            title: config.label || config.name,
                            content: widget.domNode
                        });
                        widget.getPanel = lang.hitch(this, function () {
                            return result.panel;
                        });
                        result.panel.on("show", lang.hitch(this, function () {
                            this.layoutManager.resize();
                            result.panel.resize();
                            widget.onOpen();
                        }));
                        result.panel.on("hide", lang.hitch(this, function () {
                            this.layoutManager.resize();
                            widget.onClose();
                            if (config.destroyOnClose === true) {
                                widget.destroyRecursive();
                            }
                        }));
                        widget.startup();
                    } else {
                        if (widget.templateString && widget.domNode) {
                            //---widget---//
                            widget.startup();
                        } else {
                            //---class---//
                            //do nothing
                        }
                    }
                    this.loadedWidget.add({
                        name: config.name,
                        widget: widget
                    });
                    return {
                        panel: result.panel,
                        widget: widget
                    };
                }));
            }
        },
        createWidgetByName: function (widgetName) {
            return this.createWidget(this.widgetConfig.get(widgetName));
        },
        openWidget: function (config) {
            if (lang.isString(config)) {
                return this.createWidgetByName(config).then(lang.hitch(this, function (result) {
                    if (result.panel) {
                        result.panel.show();
                    }
                    return result;
                }));
            } else {
                return this.createWidget(config).then(lang.hitch(this, function (result) {
                    if (result.panel) {
                        result.panel.show();
                    }
                    return result;
                }));
            }
        },
        _constructMixin: function (loadResult) {
            var params = {
                systemId: this.systemId,
                layoutManager: this.layoutManager,
                panelManager: this.panelManager,
                widgetManager: this.widgetManager,
                configures: this.configures,
                panelConfig: this.panelConfig,
                widgetConfig: this.widgetConfig,
                applicationManager: ApplicationManager.getInstance(),
                configManager: ConfigManager.getInstance(),
                sessionManager: SessionManager.getInstance(),
                session: this.session,
                ready: lang.hitch(this, function () { })
            };
            if (loadResult.panel) {
                params.getPanel = lang.hitch(this, function () { });
                params.onOpen = lang.hitch(this, function () { });
                params.onClose = lang.hitch(this, function () { });
            }
            return params;
        }
    });

    if (appServer.isDebug == "1") {
        window._widgetManager = wgDeclare;
    }
    lang.mixin(wgDeclare, wgConstant);
    return wgDeclare;
});
