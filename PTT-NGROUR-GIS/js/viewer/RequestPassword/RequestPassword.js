define([
    "core/_WidgetBase",
    "core/_SnackBarMixin",
    "dojo/text!./RequestPassword.html",
    "xstyle/css!./css/style.css",

    "esrith/amos/form/Dialog",
    "esrith/amos/form/FormValidator",
    "esrith/amos/form/TextBox",
    "esrith/amos/form/Button",
    "esrith/amos/form/EmailTextBox",
    "../ForgetPassword/ForgetPassword",

    "dojo/i18n!nls/global",

    "config/app",

    "dojo/dom-construct",
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
    EmailTextBox,
    ForgetPassword,

    nlsGlobal,

    appConfig,

    domConstruct,
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
        _WidgetBase,
        _SnackBarMixin
    ], {

        baseClass: "widget-request-password",
        templateString: template,
        nlsGlobal: nlsGlobal,
        sp: "UM_Q_VERIFY_USER",

        userLogin: null,
        userId: null,
        checkUser: false,
        token: null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);


            // console.log("txtEmail", this.txtEmail);
            this.own(on(this.btnRequestPass, "click", lang.hitch(this, "_btnRequestPassClick")));
            this.own(on(this.txtEmail, "blur", lang.hitch(this, "_txtEmailBlur")));
            this.own(on(this.txtEmail, "click", lang.hitch(this, "_txtEmailClick")));
            this.own(on(this.txtEmail, "change", lang.hitch(this, "_txtEmailChange")));

            this._checkUser();

        },

        startup: function () {
            this.inherited(arguments);

        },

        _txtEmailBlur: function (response) {
            //  console.log("_txtEmailBlur", this.notificationContainer);

            if (!this.txtEmail.get("valid")) {
                return;
            }
            var userName = "";
            //if (this.checkUser == true) {
            //    userName = this.userLogin;
            //}

            //var req = this.reqSP(this.sp, { "USERNAME": userName, "EMAIL": this.txtEmail.get("value") }, true);
            var req = this.reqUrl("./?p="+this.sp, { "USERNAME": userName, "EMAIL": this.txtEmail.get("value") }, false);
            req.query(lang.hitch(this, "_checkEmailComplete"), lang.hitch(this, "_checkEmailError"));

        },
        _txtEmailClick: function () {
            this.clearMessages();
        },

        _txtEmailChange: function (value) {
            //console.log("_txtEmailChange", value);
            if (value == "" || !this.txtEmail.get("valid")) {
                this.btnRequestPass.set("disabled", true);
            } else {
                // this.btnRequestPass.set("disabled", false);
            }
        },

        _checkUser: function () {
        	//var req = this.reqSP(this.sp, { "USERNAME": this.userLogin, "EMAIL": "" }, true);
        	var req = this.reqUrl("./?p=" + this.sp, { "USERNAME": this.userLogin, "EMAIL": "" }, false);
            req.query(lang.hitch(this, "_checkUserComplete"), lang.hitch(this, "checkUserError"));
        },

        _checkUserComplete: function (response) {
            if (response.success) {
                if (response.message != "") {
                    this.btnRequestPass.set("disabled", true);
                    this.showMessage(this.nlsGlobal.message.login[response.message], "warning", 5);
                }
                else {

                    if (response.data.length > 0) {
                        this.btnRequestPass.set("disabled", false);
                        this.txtEmail.set("value", response.data[0].EMAIL);
                        this.userId = response.data[0].USER_ID;
                        this.checkUser = true;
                    }
                }
            }
        },
        checkUserError: function (error) {
            console.log("checkUserError", error);
        },


        _checkEmailComplete: function (response) {
            if (response.success) {
                if (response.message != "") {
                    this.btnRequestPass.set("disabled", true);
                    this.showMessage(this.nlsGlobal.message.login[response.message], "warning", 5);
                }
                else {
                    if (response.data.length > 0) {
                        this.btnRequestPass.set("disabled", false);
                        //  this.txtEmail.set("value", response.data[0].EMAIL);
                        this.userId = response.data[0].USER_ID;
                    } else {
                        this.showMessage(this.nlsGlobal.message.login.emailNotFound, "warning", 5);
                    }
                }
            }

        },
        _checkEmailError: function (error) {
            console.log("checkEmailError", error);
            this.showMessage(nlsGlobal.errorSystem, "warning", 5);
        },

        _btnRequestPassClick: function () {

            if (!this.txtEmail.get("valid")) {
                return;
            }

            if (this.notificationContainer.children.length > 0) {
                this.showMessage(this.nlsGlobal.message.login.emailNotFound, "warning", 5);
                return;
            }

            this.showMessage("", "loading");
            this.emit("reset-password-loading", {});

            this.disable(true);

            //var req = this.reqSP("UM_Q_MAIL_TOKEN", { "USERID": this.userId }, false);
            var req = this.reqUrl("./?p=" + "UM_Q_MAIL_TOKEN", { "USERID": this.userId }, false);
            req.query(lang.hitch(this, "_getContentTokenComplete"), lang.hitch(this, "_getContentTokenError"));
        },

        _getContentTokenComplete: function (response) {
            console.log("_getContentTokenComplete", response);
            if (response.success) {
                if (response.data.length > 0) {
                    this.token = response.data[0].TOKEN;
                    this.sendForgetPassEmail(response.data[0]);
                } else {
                    this.showMessage(nlsGlobal.message.login.NO_DATA, "warning", 5);
                }
            }
        },

        _getContentTokenError: function (error) {
            console.log("_getContentTokenError", error);
            this.disable(false);
            this.clearMessages();
            this.emit("reset-password-ready", {});
            this.showMessage(nlsGlobal.message.login.sendEmailError, "warning", 5);
        },

        sendForgetPassEmail: function (result) {
            console.log("onLOGIN_ForgotPassword_Complete", result);


            var param = {
                MAIL_FROM: "",
                MAIL_TO: result.EMAIL,
                MAIL_CC: result.CC_EMAIL == null ? "" : result.CC_EMAIL,
                MAIL_BCC: "",
                MAIL_SUBJECT: this.nlsGlobal.message.login.emailSubject,
                MAIL_BODY: lang.replace(result.CONTENT, [appConfig.host + "/"])
            };

            // console.log("JSON", JSON.stringify(param));

            //var postData = JSON.stringify(param);

            this.reqDS("SEND_MAIL", param).query(lang.hitch(this, "sendEmailComplete"), lang.hitch(this, "sendEmailError"));

        },

        sendEmailComplete: function (response) {

            this.emit("reset-password-ready", {});
            this.clearMessages();
            this.disable(false);
            if (response.success) {

            	//var req = this.reqSP("UM_U_RESET_PWD", { "USERID": this.userId, "MODE": ForgetPassword.FORGET, "TOKEN": this.token }, false);
            	var req = this.reqUrl("./?p=" + "UM_U_RESET_PWD", { "USERID": this.userId, "MODE": ForgetPassword.FORGET, "TOKEN": this.token }, false);
                req.query(lang.hitch(this, "_resetPasswordComplete"), lang.hitch(this, "_resetPasswordError"));
            }

        },

        sendEmailError: function (error) {
            console.log("sendEmailError", error);

            this.disable(false);
            this.clearMessages();
            this.emit("reset-password-ready", {});
            this.showMessage(nlsGlobal.message.login.sendEmailError, "warning", 5);
        },


        _resetPasswordComplete: function (response) {
            // console.log("_resetPasswordComplete", response);
            if (response.success) {

                this.showMessage(nlsGlobal.message.login.sendEmailComplete, "success", 5);
                this.emit("reset-password-complete", {});

            } else {

            }
        },

        _resetPasswordError: function (error) {
            // console.log("UM_U_RESET_PWD_error", error);

            this.disable(false);
            this.clearMessages();
            this.emit("reset-password-ready", {});
            this.showMessage(nlsGlobal.errorSystem, "warning", 5);
        },

        disable: function (flag) {
            this.txtEmail.set("disabled", flag);
            this.btnRequestPass.set("disabled", flag);
        }

    });

    lang.mixin(wgDeclare, wgConstant);

    return wgDeclare;
});