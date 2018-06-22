define([
    "core/_ClassBase",
    "./SessionManager",
    "./ConfigManager",

    "dojo/store/Memory",
    "dojo/i18n!nls/global",
    "dojo/Deferred",
    "dojo/on",
    "dojo/promise/all",
    "dojo/dom-construct",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    _ClassBase,
    SessionManager,
    ConfigManager,

    Memory,
    nlsGlobal,
    Deferred,
    on,
    all,
    domConstruct,
    lang,
    declare,
    require
    ) {

    var instance = null;
    var wgConstant = {};

    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
        _ClassBase
    ], {

        systemMemory: null,

        start: function (loginResult) {
            this.configManager = ConfigManager.getInstance(loginResult);
            this.configures = this.configManager.getAllConfig();
            this.sessionManager = SessionManager.getInstance(loginResult);
            this.session = this.sessionManager.getSession();
            this.sessionManager.start();
            this.systemMemory = new Memory({ data: [], idProperty: "systemId" });

            this.emit("application-start", {
                configures: this.configures,
                session: this.session,
                target: this
            });
        },
        close: function () {
            this._closeAllSystem();
            this.sessionManager.reset();
            this.logout().then(lang.hitch(this, function () {
                this.emit("application-close", {
                    target: this
                });
            }));
        },
        login: function (loginParam) {
            var deferred = new Deferred();
            var dataStore = this.reqUrl("?p=APP_Q_LOGIN", loginParam);
            dataStore.useProxy = false;
            dataStore.preventCache = false;
            dataStore.query(lang.hitch(this, function (resposne) {
                deferred.resolve(resposne);
            }), lang.hitch(this, function (err) {
                deferred.reject(err);
            }));
            return deferred.promise;
        },
        logout: function () {
            var deferred = new Deferred();
            var dataStore = this.reqUrl("?p=logout");
            dataStore.sync = true;
            dataStore.useProxy = false;
            dataStore.preventCache = false;
            dataStore.query(lang.hitch(this, function (resposne) {
                deferred.resolve(resposne);
            }), lang.hitch(this, function (err) {
                deferred.resolve(err);
            }));
            return deferred.promise;
        },

        getSystemById: function (systemId) {
            return this.systemMemory.get(systemId);
        },
        getSystemByConfig: function (systemConfig) {
            systemConfig = systemConfig || {};
            return this.systemMemory.get(systemConfig.id);
        },
        openSystem: function (systemConfig, targetNode) {
            var deferred = new Deferred();
            try {
                if (systemConfig && lang.isObject(systemConfig)) {
                    var divLayout = domConstruct.create("div", {}, targetNode || document.body);
                    require(((systemConfig.widget || "./LayoutManager") +
                        (systemConfig.panelConfig ? "," + systemConfig.panelConfig : "") +
                        (systemConfig.widgetConfig ? "," + systemConfig.widgetConfig : ""))
                        .split(","), lang.hitch(this, function (
                            WidgetSystem,
                            panelConfig,
                            widgetConfig
                        ) {
                            if (this.session["SYSTEM_ID"] != systemConfig["id"]) {
                                this.session["SYSTEM_ID"] = systemConfig["id"];
                                var reqSystem = this.reqUrl("?p=system", {
                                    SYS_ID: systemConfig["id"]
                                });
                                reqSystem.useProxy = false;
                                reqSystem.preventCache = false;
                                reqSystem.query();
                            }
                            var params = {
                                systemId: systemConfig["id"],
                                panelConfig: panelConfig,
                                widgetConfig: widgetConfig,
                                session: this.session,
                                configures: this.configManager.getPermissionBySystemConfig(systemConfig),
                            };
                            var widgetSystem = new WidgetSystem(params, divLayout);
                            widgetSystem.startup();
                            this.systemMemory.add(widgetSystem);
                            deferred.resolve(widgetSystem);
                        }));
                } else if (systemConfig && lang.isString(systemConfig)) {
                    return this.openSystem(this.configManager.getSystemConfigById(systemConfig), targetNode);
                } else {
                    throw new Error("invalid system");
                }
            } catch (err) {
                throw new Error("openSystem: " + err.message);
            }
            return deferred.promise;
        },
        closeSystem: function (systemConfig) {
            var deferred = new Deferred();
            var systemId = "";
            if (systemConfig && lang.isObject(systemConfig)) {
                systemId = systemConfig.id;
            } else if (systemConfig && lang.isString(systemConfig)) {
                systemId = systemConfig;
            } else {
                throw new Error("invalid system");
            }
            if (this.configures.systemConfig.data.length == 1) {
                this.close();
                deferred.resolve(null);
                return deferred.promise;
            }
            this.session["SYSTEM_ID"] = "-1";
            var reqSystem = this.reqUrl("?p=system", {
                SYS_ID: "-1"
            });
            reqSystem.useProxy = false;
            reqSystem.preventCache = false;
            reqSystem.query(lang.hitch(this, function () {
                var widgetSystem = this.systemMemory.get(systemId);
                widgetSystem.destroyRecursive();
                widgetSystem = null;
                this.systemMemory.remove(systemId);
                deferred.resolve(widgetSystem);
                this.emit("system-close", {
                    systemConfig: systemConfig,
                    configures: this.configures,
                    target: widgetSystem
                });
            }));
            return deferred.promise;
        },
        _closeAllSystem: function () {
            var deferred = new Deferred();
            var systemId = "";
            this.session["SYSTEM_ID"] = -1;
            var reqSystem = this.reqUrl("?p=system", {
                SYS_ID: -1
            });
            reqSystem.useProxy = false;
            reqSystem.preventCache = false;
            reqSystem.query(lang.hitch(this, function () {
                var widgetSystem = null;
                while (this.systemMemory.data.length > 0) {
                    widgetSystem = this.systemMemory.data[0];
                    widgetSystem.destroyRecursive();
                    this.systemMemory.remove(widgetSystem[this.systemMemory.idProperty]);
                    widgetSystem = null;
                }
                deferred.resolve(widgetSystem);
            }));
            return deferred.promise;
        }
    });

    wgConstant.getInstance = function () {
        if (instance === null) {
            instance = new wgDeclare();
            window._applicationManager = instance;
        }
        return instance;
    }

    lang.mixin(wgDeclare, wgConstant);


    return wgDeclare;
});
