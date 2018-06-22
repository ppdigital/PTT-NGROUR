define([
    "core/_WidgetBase",
    "manager/ApplicationManager",
    "manager/SessionManager",

    "dojo/text!./Login.html",
    "xstyle/css!./css/style.css",

    "esrith/amos/form/FormValidator",
    "esrith/amos/form/TextBox",
    "esrith/amos/form/Button",
    "dojo/i18n!nls/global",

    "config/app",

    "dojo/Deferred",
    "dojo/dom-construct",
    "dojo/dom-style",
    "dojo/dom-class",
    "dojo/query",
    "dojo/json",
    "dojo/on",
    "dojo/aspect",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    _WidgetBase,
    ApplicationManager,
    SessionManager,
    template,
    xstyle,

    FormValidator,
    TextBox,
    Button,
    nlsGlobal,

    appConfig,

    Deferred,
    domConstruct,
    domStyle,
    domClass,
    query,
    JSON,
    on,
    aspect,
    lang,
    declare,
    require
    ) {


    var wgConstant = {};


    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
        _WidgetBase
    ], {

        baseClass: "widget-login",
        templateString: template,

        nlsGlobal: nlsGlobal,
        dlgForgetPassword: null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
            params.isSessionTimeout = params.isSessionTimeout == null ? false : params.isSessionTimeout;
            this.applicationManager = ApplicationManager.getInstance();
        },
        postCreate: function () {
            this.inherited(arguments);
            this.own(on(this.frmValid, "enter", lang.hitch(this, "login")));
            this.own(on(this.btnLogin, "click", lang.hitch(this, "login")));
            this.own(on(this.divForgetPass, "click", lang.hitch(this, "_divForgetPassClick")));
            this._initResetPassword();
        },
        _setIsSessionTimeoutAttr: function (flag) {
            if (flag === true) {
                domClass.remove(this.domNode, "ui-body");
                this.txtUsername.set("disabled", true);
                this.hide(this.divForgetPass);
            } else {
                domClass.add(this.domNode, "ui-body");
                this.txtUsername.set("disabled", false);
                this.show(this.divForgetPass);
            }
            console.log("isSessionTimeout", flag);
            this._set("isSessionTimeout", flag);
        },

        _createForgetPassword: function (ForgetPassword, Dialog, param) {

            //  console.log("_createForgetPassword", fn);

            if (this.dlgResetPassword != null) {
                this.dlgResetPassword.destroyRecursive();
            }

            this.dlgResetPassword = new Dialog({
                closable: true,
                minable: false,
                maxable: false,
                title: nlsGlobal.message.login.changePassword
            });

            var forgetPassWord = new ForgetPassword(param);
            this.own(on(forgetPassWord, "forget-password-complete", lang.hitch(this, "_forgetPassWordComplete")));

            this.dlgResetPassword.set("content", forgetPassWord.domNode);
            this.dlgResetPassword.show();

            //  console.log("_dlgResetPasswordHide", this.dlgResetPassword);

            this.own(on(this.dlgResetPassword, "hide", lang.hitch(this, "_dlgResetPasswordHide")));

        },

        _forgetPassWordComplete: function (result) {
            this.dlgResetPassword.hide();
            this.frmValid.set("value", { PASSWORD: result.newPassword });
            this.frmValid.submit();
        },

        login: function () {
            if (this.frmValid.isValid()) {
                this.frmValid.set("disabled", true);
                this.applicationManager.login(this.frmValid.get("value")).then(lang.hitch(this, "_loginComplete"), lang.hitch(this, "_loginError"));
            } else {
                throw new Error("invalid form input.");
            }
        },
        _loginComplete: function (loginResult) {
            if (!this.isSessionTimeout) {
                this.destroyRecursive();
                this.applicationManager.start(loginResult);
            } else {
                this.sessionManager = SessionManager.getInstance(loginResult);
                this.sessionManager.start();
            }
        },
        _loginError: function (response) {
            var param = {
                username: this.frmValid.get("value").USERNAME
            };
            require(["../ForgetPassword/ForgetPassword", "esrith/amos/form/Dialog"], lang.hitch(this, function (ForgetPassword, Dialog) {
                if (response.message == "PASSWORD_CONFIG") {
                    param.mode = ForgetPassword.RESET;
                    this._createForgetPassword(ForgetPassword, Dialog, param);
                } else if (response.message == "PASSWORD_EXP") {
                    param.mode = ForgetPassword.CHANGE;
                    this._createForgetPassword(ForgetPassword, Dialog, param);
                } else {
                    var divNode = domConstruct.create("div", { innerHTML: this.nlsGlobal.message.login[response.message] || response.message });
                    this.alert(divNode, lang.hitch(this, function () {
                        this.frmValid.set("disabled", false);
                    }));
                }
            }));
        },

        startup: function () {
            this.inherited(arguments);
        },
        _initResetPassword: function () {
            //   console.log("appServer", appServer.webConfig.MAILSERVER);
            if (!this.isSessionTimeout && appServer.webConfig.EMAIL_SERVER_FLAG == "true" && appServer.webConfig.AD_CONNECT_FLAG == "false") {
                this.show(this.divForgetPass);
            } else {
                this.hide(this.divForgetPass);
            }
        },
        _divForgetPassClick: function () {

            this.frmValid.set("disabled", true);
            require(["../RequestPassword/RequestPassword", "esrith/amos/form/Dialog"], lang.hitch(this, function (RequestPassword, Dialog) {
                var requestPassword = new RequestPassword({
                    userLogin: this.frmValid.get("value").USERNAME
                });

                this.own(on(requestPassword, "reset-password-complete", lang.hitch(this, "_resetPasswordComplete")));
                this.own(on(requestPassword, "reset-password-loading", lang.hitch(this, "_resetPasswordLoading")));
                this.own(on(requestPassword, "reset-password-ready", lang.hitch(this, "_resetPasswordReady")));

                if (this.dlgForgetPassword != null) {
                    this.dlgForgetPassword.destroyRecursive();
                }

                this.dlgForgetPassword = new Dialog({
                    closable: true,
                    minable: false,
                    maxable: false,
                    title: nlsGlobal.message.login.recoverPassword
                });

                //   domStyle.set(this.dlgForgetPassword.domNode, "width", "1000px");
                domClass.add(this.dlgForgetPassword.domNode, "ui-forgetpassword-dialog");


                this.dlgForgetPassword.set("content", requestPassword.domNode);
                this.dlgForgetPassword.startup();
                this.dlgForgetPassword.show();


                this.own(on(this.dlgForgetPassword, "hide", lang.hitch(this, "_dlgResetPasswordHide")));
                //  console.log("dlgResetPassword", this.dlgForgetPassword, this.dlgForgetPassword.btnClose);
            }));

        },

        _resetPasswordLoading: function () {
            this.dlgForgetPassword.set("busy", true);
        },

        _resetPasswordReady: function () {
            this.dlgForgetPassword.set("busy", false);
        },

        _resetPasswordComplete: function () {
            // this.dlgForgetPassword.hide();
            this.frmValid.set("disabled", false);
        },

        _dlgResetPasswordHide: function () {

            console.log("_dlgResetPasswordHide");
            this.frmValid.set("disabled", false);
        }
    });

    lang.mixin(wgDeclare, wgConstant);

    return wgDeclare;
});