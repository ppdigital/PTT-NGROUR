define([
    "core/_WidgetBase",

    "dojo/text!./SystemSelection.html",
    "xstyle/css!./SystemSelection.css",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/dom-class",
	"dojo/dom-construct"
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
        baseClass: "system-selection",
        templateString: template,

        systemItems: null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },
        postCreate: function () {
            this.inherited(arguments);
            this._initEvent();
            this.hide(this.domNode);
        },

        _initEvent: function () {
            this.own(
                on(this.domNode, "click", lang.hitch(this, function (e) {
                    if (e.target === this.domNode || e.target === this.containerNode) {
                        this.emit("close");
                    }
                }))
                );
        },

        _setSystemItemsAttr: function (value) {
            this._set("systemItems", value);
            domConstruct.empty(this.containerNode);
            array.forEach(this.systemItems, lang.hitch(this, function (systemItem) {
                var systemItemNode = domConstruct.create("div", {
                    className: systemItem.className ? "item " + systemItem.className : "item",
                    title: systemItem.title || ""
                }, this.containerNode);
                domConstruct.create("div", {
                    className: "animate"
                }, systemItemNode);
                domConstruct.create("div", {
                    className: systemItem.iconClassName ? "icon " + systemItem.iconClassName : "icon"
                }, systemItemNode);
                domConstruct.create("div", {
                    className: "label",
                    innerHTML: systemItem.label
                }, systemItemNode);
                array.forEach(systemItem.events, lang.hitch(this, function (event) {
                    this._addEvent(systemItemNode, event.action, event.name, systemItem);
                }));
            }));
        },

        toggleVisibility: function (visibility) {
            if (typeof (visibility) !== "boolean") {
                visibility = null;
            }
            if (visibility === false || (visibility === null && domClass.contains(this.domNode, "visible"))) {
                domClass.remove(this.domNode, "visible");
                setTimeout(lang.hitch(this, function () {
                    this.hide(this.domNode);
                }), ANIMATION_DURATION);
            } else {
                this.show(this.domNode);
                setTimeout(lang.hitch(this, function () {
                    domClass.add(this.domNode, "visible");
                }), 0);
            }
        },
    });
});