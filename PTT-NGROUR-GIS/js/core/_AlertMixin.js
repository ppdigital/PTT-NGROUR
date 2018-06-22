/**
 * @description 
 * -
 * @class _AlertMixin
 */
define([
    "dojo/i18n!nls/global",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    nlsGlobal,
    lang,
    declare,
    require
    ) {


    var wgDeclare = declare(/*[DECLARED_CLASS]*/null, {


        _dlgDefaultTitle: nlsGlobal.dialog.defaultTitle,


        /**
        * Display a alert dialog box with your message.
        * @method alert
        * @public
        * @param {String} message 
        */
        /**
        * @method alert
        * @public
        * @param {String} message 
        * @param {String} title  
        */
        /**
        * @method alert
        * @public
        * @param {String} message 
        * @param {Function} callback function(confirm){ }
        */
        /**
        * @method alert
        * @public
        * @param {String} message 
        * @param {String} title 
        * @param {Function} callback function(confirm){ }
        */
        /**
        * @method alert
        * @public
        * @param {String} message 
        * @param {Function} callback function(confirm){ }
        * @param {String} title 
        */
        alert: function (message) {
            var args = arguments;
            require(["esrith/amos/core/AlertIndicator"], lang.hitch(this, function (alertIndicator) {
                var title = null;
                var callback = null;
                if (args.length > 1) {
                    for (var i = 1; i < args.length; i++) {
                        if (typeof (args[i]) == "string") {
                            title = args[i];
                        } else if (typeof (args[i]) == "function") {
                            callback = args[i];
                        }
                    }
                }


                title = title || this._dlgDefaultTitle;
                callback = callback || function () { };
                alertIndicator.alert(message, callback, {
                    title: title,
                    closable: true
                });
            }));
        },


        /**
        * Display a confirm dialog box with your message.
        * @method confirm
        * @public
        * @param {String} message 
        */
        /**
        * @method confirm
        * @public
        * @param {String} message 
        * @param {String} title  
        */
        /**
        * @method confirm
        * @public
        * @param {String} message 
        * @param {Function} callback function(confirm){ }
        */
        /**
        * @method confirm
        * @public
        * @param {String} message 
        * @param {String} title 
        * @param {Function} callback function(confirm){ }
        */
        confirm: function (message) {
            var args = arguments;
            require(["esrith/amos/core/AlertIndicator"], lang.hitch(this, function (alertIndicator) {
                var title = null;
                var callback = null;
                if (args.length > 1) {
                    for (var i = 1; i < args.length; i++) {
                        if (typeof (args[i]) == "string") {
                            title = args[i];
                        } else if (typeof (args[i]) == "function") {
                            callback = args[i];
                        }
                    }
                }
                title = title || this._dlgDefaultTitle;
                callback = callback || function () { };
                alertIndicator.confirm(message, callback, {
                    title: title,
                    closable: true
                });
            }));
        },


        /**
        * Display a warning dialog box with your message.
        * @method warning
        * @public
        * @param {String|Object} message 
        */
        /**
        * @method warning
        * @public
        * @param {String|Object} message 
        * @param {String} title  
        */
        /**
        * @method warning
        * @public
        * @param {String|Object} message 
        * @param {Function} callback function(confirm){ }
        */
        /**
        * @method warning
        * @public
        * @param {String|Object} message 
        * @param {String} title 
        * @param {Function} callback function(confirm){ }
        */
        warning: function (message) {
            var args = arguments;
            require(["esrith/amos/core/AlertIndicator"], lang.hitch(this, function (alertIndicator) {
                if (typeof (message) == "object") {
                    message = message.message;
                }
                var title = null;
                var callback = null;
                if (args.length > 1) {
                    for (var i = 1; i < args.length; i++) {
                        if (typeof (args[i]) == "string") {
                            title = args[i];
                        } else if (typeof (args[i]) == "function") {
                            callback = args[i];
                        }
                    }
                }

                title = title || this._dlgDefaultTitle;
                callback = callback || function () { };
                alertIndicator.alert(message, callback, {
                    title: title,
                    closable: true
                });
            }));
        },


        /**
        * Display a error dialog box with your message.
        * @method error
        * @public
        * @param {String|Object} message 
        */
        /**
        * @method error
        * @public
        * @param {String|Object} message 
        * @param {String} title  
        */
        /**
        * @method error
        * @public
        * @param {String|Object} message 
        * @param {Function} callback function(confirm){ }
        */
        /**
        * @method error
        * @public
        * @param {String|Object} message 
        * @param {String} title 
        * @param {Function} callback function(confirm){ }
        */
        error: function (message) {
            var args = arguments;
            require(["esrith/amos/core/AlertIndicator"], lang.hitch(this, function (alertIndicator) {
                if (typeof (message) == "object") {
                    message = message.message;
                }
                var title = null;
                var callback = null;
                if (args.length > 1) {
                    for (var i = 1; i < args.length; i++) {
                        if (typeof (args[i]) == "string") {
                            title = args[i];
                        } else if (typeof (args[i]) == "function") {
                            callback = args[i];
                        }
                    }
                }

                title = title || this._dlgDefaultTitle;
                callback = callback || function () { };
                alertIndicator.alert(message, callback, {
                    title: title,
                    closable: true
                });
            }));
        }

    });

    return wgDeclare;
});