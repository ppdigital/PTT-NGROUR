define([
    "core/_WidgetBase",
    "manager/ApplicationManager",
    "manager/SessionManager",

    "dojo/i18n!nls/global",
    "dojo/on",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    _WidgetBase,
    ApplicationManager,
    SessionManager,

    nlsGlobal,
    on,
    lang,
    declare,
    require
    ) {


    var wgConstant = {};

    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
        _WidgetBase
    ], {
        baseClass: "ui-body viewer-portal",
        configures: null,
        constructor: function (params, srcNodeRef) {
            params = params || {};
            this.applicationManager = ApplicationManager.getInstance();
            this.sessionManager = SessionManager.getInstance();
            on(this.applicationManager, "application-start", lang.hitch(this, "_workBeforeOpen"));
            on(this.applicationManager, "application-close", lang.hitch(this, "promptLogin"));
            on(this.sessionManager, "session-end", lang.hitch(this, "promptLoginSessionEnd"));
            on(this.sessionManager, "session-start", lang.hitch(this, "closeLoginSessionEnd"));
            on(this.applicationManager, "system-close", lang.hitch(this, "_systemClose"));
        },
        postCreate: function () {
            this.inherited(arguments);
            if (!this.permission) {
                this.promptLogin();
            } else if (this.permission && this.permission["data"]) {
                this.applicationManager.start(this.permission);
            }
        },
        _workBeforeOpen: function (evt) {
            if (evt.session && evt.session["SYSTEM_ID"] && evt.session["SYSTEM_ID"] != -1) {
                this.applicationManager.openSystem(evt.session["SYSTEM_ID"], this.domNode);
            } else {
                if (evt.configures.systemConfig.data.length == 1) {
                    this.applicationManager.openSystem(evt.configures.systemConfig.data[0], this.domNode);
                } else if (evt.configures.systemConfig.data.length > 1) {
                    this.promptSelector(evt.configures.systemConfig);
                }
            }
        },
        _systemClose: function (evt) {
            if (evt.configures.systemConfig.data.length > 1) {
                this.promptSelector(evt.configures.systemConfig);
            } else {
                this.applicationManager.close();
            }
        },
        promptLogin: function () {
            require([
                "../Login/Login"
            ], lang.hitch(this, function (
                Login
                ) {
                var login = new Login().placeAt(this.domNode);
                login.startup();
            }));
        },
        promptLoginSessionEnd: function (evt) {
            this.applicationManager.logout();
            require([
                "./Login/Login",
                "esrith/amos/form/Dialog",
                "dojo/dom-construct",
            ], lang.hitch(this, function (
                Login,
                Dialog,
                domConstruct
                ) {
                var divLogin = domConstruct.create("div");
                var divDialog = domConstruct.create("div", {}, this.domNode);
                this._loginFormSessionEnd = new Login({
                    isSessionTimeout: true,
                    session: evt.data
                }, divLogin);
                this._dialogSessionEnd = new Dialog({
                    closable: false,
                    content: this._loginFormSessionEnd.domNode
                }, divDialog);
                this._dialogSessionEnd.startup();
                this._dialogSessionEnd.show();
            }));
        },
        closeLoginSessionEnd: function () {
            if (this._dialogSessionEnd) {
                this._dialogSessionEnd.hide().then(lang.hitch(this, function () {
                    this._loginFormSessionEnd.destroyRecursive();
                    this._dialogSessionEnd.destroyRecursive();
                    this._loginFormSessionEnd = null;
                    this._dialogSessionEnd = null;
                }));
            }
        },
        promptSelector: function (systemConfig) {
            require([
                "../SystemSelector/SystemSelector"
            ], lang.hitch(this, function (
                SystemSelector
                ) {
                var systemSelector = new SystemSelector({
                    systemConfig: systemConfig
                }).placeAt(this.domNode);
                systemSelector.startup();
            }));
        }
    });

    lang.mixin(wgDeclare, wgConstant);

    return wgDeclare;
});
