define([
    "core/_ClassBase",
    "./LayoutManager",

    "dojo/Deferred",
    "dojo/store/Memory",
    "dojo/on",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    _ClassBase,
    LayoutManager,

    Deferred,
    Memory,
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

        createdPanel: null,

        constructor: function (params) {
            params = params || {};
            lang.mixin(this, params);
            wgDeclare.creates.add(this);
            this.createdPanel = new Memory({ data: [], idProperty: "id" });
        },
        createPanelByName: function (panelName) {
            return this.createPanelByConfig(this.panelConfig.get(panelName));
        },
        createPanelByConfig: function (panelConfig) {
            var deferred = new Deferred();
            if (!panelConfig) {
                deferred.resolve();
                return;
            }
            var domTarget = this.layoutManager.createTargetByConfig(panelConfig);
            require([
                "esrith/amos/container/FloatingPanel"
            ], lang.hitch(this, function (
                _domTarget,
                FloatingPanel
                ) {
                var panel = new FloatingPanel(panelConfig, _domTarget);
                panel.startup();
                this.createdPanel.add(panel);
                deferred.resolve(panel);
            }, domTarget));
            return deferred.promise;
        },
        getPanelByName: function (panelName) {
            return this.createdPanel.query({ name: panelName });
        },
        getPanelByPosition: function (panelPosition) {
            return this.createdPanel.query({ position: panelPosition });
        },
        getPanelById: function (panelId) {
            return this.createdPanel.get(panelId);
        }
    });

    if (appServer.isDebug == "1") {
        window._panelManager = wgDeclare;
    }

    lang.mixin(wgDeclare, wgConstant);
    return wgDeclare;
});
