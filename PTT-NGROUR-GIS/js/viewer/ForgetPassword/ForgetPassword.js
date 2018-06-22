define([
    "core/_WidgetBase",
    "core/_SnackBarMixin",

    "dojo/text!./ForgetPassword.html",
    "xstyle/css!./css/style.css",

    "esrith/amos/form/Dialog",
    "esrith/amos/form/FormValidator",
    "esrith/amos/form/TextBox",
    "esrith/amos/form/Button",
    "esrith/amos/form/PasswordTextBox",
    "dojo/i18n!nls/global",

    "config/app",

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
    _SnackBarMixin,
    template,
    xstyle,

    Dialog,
    FormValidator,
    TextBox,
    Button,
    PasswordTextBox,
    nlsGlobal,

    appConfig,

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


    var wgConstant = {
        FORGET: "FORGET",
        RESET: "RESET",
        CHANGE: "CHANGE"
    };


    var wgDeclare = declare(/*[DECLARED_CLASS]*/[
        _WidgetBase,
        _SnackBarMixin
    ], {

        baseClass: "widget-forget-password",
        templateString: template,

        nlsGlobal: nlsGlobal,
        mode: "FORGET",
        username: null,

        token: null,
        userid: null,

        forgetByEmail: false,

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);

            // this.mode = wgConstant.CHANGE;

            if (this.mode == wgConstant.CHANGE) {
                this.ptxtOldPass.set("required", true);
                domStyle.set(this.ptxtOldPass.domNode, "display", "block");
            }

            this.own(on(this.btnSubmit, "click", lang.hitch(this, "_btnSubmitClick")));
            this.own(on(this.btnClear, "click", lang.hitch(this, "_btnClearClick")));

            this.own(on(this.ptxtNewPass, "blur", lang.hitch(this, "_ptxtNewPassOnBlur")));
            this.own(on(this.ptxtNewPassAgain, "blur", lang.hitch(this, "_ptxtNewPassAgainOnBlur")));
            this.own(on(this.ptxtOldPass, "click", lang.hitch(this, function () { this.clearMessages(); })));
            this.own(on(this.ptxtNewPass, "click", lang.hitch(this, function () { this.clearMessages(); })));
            this.own(on(this.ptxtNewPassAgain, "click", lang.hitch(this, function () { this.clearMessages(); })));


            //console.log("username", this.username);
            //console.log("mode", this.mode);


            //console.log("token", this.token);
            //console.log("userid", this.userid);

            if (this.forgetByEmail)
            {				
            	//var req = this.reqSP("UM_Q_VERIFY_USER", { TOKEN: this.token, USERID: this.userid }, true);
            	var req = this.reqUrl("./?p=UM_Q_VERIFY_USER", { TOKEN: this.token, USERID: this.userid }, false);

                req.query(lang.hitch(this, "_verifyUserComplete"), lang.hitch(this, "_verifyUserError"));
            }

            if (this.username != null) {
                this.show();
            }
        },

        startup: function () {
            this.inherited(arguments);
        },

        _ptxtNewPassOnBlur: function () {

            if (this.ptxtNewPassAgain.get("error") == true) {
                if (this.ptxtNewPass.get("value") === this.ptxtNewPassAgain.get("value") && this.ptxtNewPassAgain.get("value") !== "") {
                    this.ptxtNewPassAgain.set("error", false)
                }
            }
        },

        _ptxtNewPassAgainOnBlur: function () {
            if (this.ptxtNewPass.get("value") !== this.ptxtNewPassAgain.get("value")) {
                this.ptxtNewPassAgain.showError(nlsGlobal.message.login.forgetpasswordEqual);
            }
        },

        _btnSubmitClick: function () {
            this.clearMessages();
            if (!this.frmValid.get("valid")) {
                return;
            }

            if (this.ptxtNewPassAgain.get("error") == true || this.ptxtNewPass.get("value") !== this.ptxtNewPassAgain.get("value")) {
                this.ptxtNewPassAgain.showError(nlsGlobal.message.login.forgetpasswordEqual);
                return;
            }

            if (this.forgetByEmail == true) {
            	//var req = this.reqSP("UM_Q_VERIFY_USER", { TOKEN: this.token, USERID: this.userid }, true);
            	var req = this.reqUrl("./?p=UM_Q_VERIFY_USER", { TOKEN: this.token, USERID: this.userid }, false);

                req.query(lang.hitch(this, "checkTokenBeforeSaveComplete"), lang.hitch(this, "checkTokenBeforeSaveError"));
            } else {
                this.saveNewPassword();
            }


        },

        checkTokenBeforeSaveComplete: function (response) {
            if (response.success == true) {
                if (response.data.length > 0) {
                    this.saveNewPassword();
                } else {
                    var divNode = domConstruct.create("div", { innerHTML: nlsGlobal.message.login.tokenEmailError });
                    this.alert(divNode);
                }
            }
        },

        checkTokenBeforeSaveError: function (error) {
            console.log("checkTokenBeforeSaveError", error);
            var divNode = domConstruct.create("div", { innerHTML: nlsGlobal.message.login.tokenEmailError });
            this.alert(divNode);

        },

        saveNewPassword: function () {

            var param = {
                USERNAME: this.username,
                MODE: this.mode,
                PASSWORD_NEW: this.ptxtNewPass.get("value")
            };

            if (this.mode == wgConstant.CHANGE) {
                param.PASSWORD_OLD = this.ptxtOldPass.get("value")
            }
            //var req = this.reqSP("UM_U_PWD", param, true);
            var req = this.reqUrl("./?p=UM_U_PWD", param, false);
            req.query(lang.hitch(this, "_updatePasswordComplete"), lang.hitch(this, "_updatePasswordError"));
        },

        _updatePasswordComplete: function (response) {
            if (response.success == true) {
                if (response.message == "" || response.message == "CORRECT") {
                    if (this.token != "" && this.forgetByEmail == true) {
                        var divNode = domConstruct.create("div", { innerHTML: nlsGlobal.message.login.changePasswordComplete });
                        this.alert(divNode, lang.hitch(this, function () {
                            window.location = "Page.aspx";
                        }));
                    }
                    this.emit("forget-password-complete", { newPassword: this.frmValid.get("value").NEWPASSWORD });
                    this.reset();
                    this.hide(this.wgForgetPassword);

                } else {
                    this.showMessage(nlsGlobal.message.login[response.message]);
                }

            } else {

            }
        },
        _updatePasswordError: function (error) {
            console.log("_updatePasswordError", error);
        },

        _verifyUserComplete: function (response) {
            if (response.success == true) {
                if (response.data.length > 0) {
                    var data = response.data[0];
                    this.username = data.USERNAME;
                    domConstruct.create("div", { innerHTML: nlsGlobal.message.login.forgetpasswordHeader + data.NAME }, this.divHeader, "only");
                    this.show(this.wgForgetPassword);
                } else {
                    var divNode = domConstruct.create("div", { innerHTML: nlsGlobal.errorSystem });
                    this.alert(divNode);
                }
            }
        },

        _verifyUserError: function (error) {
            console.log("_verifyUserError", error);
        },

        _btnClearClick: function () {
            this.clearMessages();
            this.frmValid.reset();
        },

        reset: function () {
            this.frmValid.reset();
        }
    });

    lang.mixin(wgDeclare, wgConstant);

    return wgDeclare;
});