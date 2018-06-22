define([
    "core/_WidgetBase",

    "dojo/text!./PokTest.html",
    "xstyle/css!./PokTest.css",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/dom-class",
	"dojo/dom-construct",

    "esri/geometry/Point",
    "esri/geometry/webMercatorUtils",

    "esrith/amos/form/Button"
], function (
    _WidgetBase,

    template,
    xstyle,

    declare,
    lang,
    array,
    on,
    domClass,
    domConstruct,

    Point,
    webMercatorUtils
    ) {
    return declare([
        _WidgetBase
    ], {
        baseClass: "PokTestComponent",
        templateString: template,

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);

            this.own(
            );

        }
    });
});