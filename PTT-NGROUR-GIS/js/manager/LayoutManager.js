define([
    "core/_WidgetBase",

    "dojo/text!./templates/LayoutManager.html",
    "xstyle/css!./css/LayoutManager.css",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/dom-construct",

    "system/gis/Viewport/Viewport"
], function (
    _WidgetBase,

    template,
    xstyle,

    declare,
    lang,
    domConstruct,

    GISViewport
    ) {
    return declare([
        _WidgetBase
    ], {
        baseClass: "layout-manager",
        templateString: template,

        postCreate: function () {
            this.inherited(arguments);
            domConstruct.place(new GISViewport().domNode, this.domNode);
        }
    });
});