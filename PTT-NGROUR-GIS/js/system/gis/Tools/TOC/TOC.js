define([
    "core/_WidgetBase",

    "dojo/text!./TOC.html",
    "xstyle/css!./TOC.css",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/dom-class",
	"dojo/dom-construct",

    "esrith/amos/form/Select",
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
    domConstruct

    ) {
    return declare([
        _WidgetBase
    ], {
        baseClass: "TOCComponent",
        templateString: template,
        markerPoint: null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);
            //console.log('it\'s GotoXY', this.map);

            this.own(
                 //on(this.map, "mouse-move", lang.hitch(this, 'cursorLocation'))
            );

        }
    });
});