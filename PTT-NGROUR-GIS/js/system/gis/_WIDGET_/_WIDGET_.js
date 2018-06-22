define([
    "core/_WidgetBase",

    "dojo/text!./_WIDGET_.html",
    "xstyle/css!./_WIDGET_.css",

    "dojo/_base/declare"
], function (
    _WidgetBase,

    template,
    xstyle,

    declare
    ) {
    return declare([
        _WidgetBase
    ], {
        baseClass: "_WIDGET_",
        templateString: template,

        postCreate: function () {
            this.inherited(arguments);
            this._initEvent();
        },

        _initEvent: function () {
            this.own(
                
                );
        }
    });
});