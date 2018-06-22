define([
	"core/_WidgetBase",

    "dojo/_base/declare",

	"dojo/text!./AwesomeButton.html",
    "xstyle/css!./AwesomeButton.css",

	"dojo/on",
    "dojo/_base/lang",
	"dojo/_base/array",
	"dojo/query",
	"dojo/dom-construct",
	"dojo/dom-class",
    "dojo/dom-style",
    "require"
], function (
    _WidgetBase,

    declare,

	template,
    xstyle,

    on,
    lang,
	array,
    query,
	domConstruct,
	domClass,
    domStyle,
    require
    ) {
    return declare([
		_WidgetBase
    ], {
        baseClass: "awesome-button",
        templateString: template,

        iconClassName: null,
        buttonClassName: null,
        iconUrl: null,
        title: null,
        subItems: null,
        subItemPosition: null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },

        postCreate: function () {
            this.inherited(arguments);
            this._initEvent();
        },

        _initEvent: function () {
            this.own(
                on(this.btnNode, "click", lang.hitch(this, function (e) {
                    if (this._subItems) {
                        this.toggleVisibilitySubItemContainer();
                    } else {
                        this.emit("click", {
                            target: this,
                            event: e
                        });
                    }
                }))
                );
        },

        _setIconClassNameAttr: function (value) {
            this._set("iconClassName", value);
            domClass.add(this.btnNode, this.iconClassName);
        },

        _setButtonClassNameAttr: function (value) {
            this._set("buttonClassName", value);
            domClass.add(this.domNode, this.buttonClassName);
        },

        _setIconUrlAttr: function (value) {
            this._set("iconUrl", value);
            domStyle.set(this.btnNode, "background-image", "url(" + this.iconUrl + ")");
        },

        _setTitleAttr: function (value) {
            this._set("title", value);
            this.domNode.title = this.title;
        },

        _setSubItemsAttr: function (value) {
            this._set("subItems", value);
            this.toggleVisibilitySubItemContainer(false);
            if (this.subItems) {
                this._subItems = [];
                array.forEach(this.subItems, lang.hitch(this, function (subItem) {
                    var subItemDomNode = domConstruct.create("div", {
                        className: (subItem.className ? "button " + subItem.className : "button") + " " + (subItem.iconClassName || ""),
                        title: subItem.title || "",
                        style: {
                            backgroundImage: subItem.iconUrl ? "url(" + subItem.iconUrl + ")" : ""
                        }
                    }, this.subItemContainer);
                    this.own(
                        on(subItemDomNode, "click", lang.hitch(this, function (e) {
                            this.emit("sub-item-click", {
                                target: this,
                                event: e,
                                subItem: subItem
                            });
                        }))
                        );
                    array.forEach(subItem.events, lang.hitch(this, function (event) {
                        this._addEvent(subItemDomNode, event.action, event.name, subItem);
                    }));
                    this._subItems.push({
                        item: subItem,
                        domNode: subItemDomNode
                    });
                }));
            } else {
                this._subItems = null;
            }
        },

        _subItemPositionList: ["top", "right", "bottom", "left"],
        _setSubItemPositionAttr: function (value) {
            domClass.remove(this.subItemContainer, this._subItemPositionList);
            if (this._subItemPositionList.indexOf(value) > -1) {
                this._set("subItemPosition", value);
                domClass.add(this.subItemContainer, this.subItemPosition);
            } else {
                this._set("subItemPosition", null);
            }
        },

        toggleVisibilitySubItemContainer: function (visibility) {
            if (!this._subItems || !this.subItemPosition) {
                visibility = false;
            }
            if (typeof (visibility) !== "boolean") {
                visibility = null;
            }
            if (visibility === false || (visibility === null && domClass.contains(this.subItemContainer, "visible"))) {
                array.forEach(this._subItems, lang.hitch(this, function (item, index) {
                    domClass.remove(item.domNode, "visible");
                }));
                setTimeout(lang.hitch(this, function () {
                    domClass.remove(this.subItemContainer, "visible");
                }), 200);
            } else {
                switch (this.subItemPosition) {
                    case "top":
                    case "bottom":
                        domStyle.set(this.subItemContainer, "width", "");
                        domStyle.set(this.subItemContainer, "height", (this._subItems.length * 48) + "px");
                        break;

                    case "right":
                    case "left":
                        domStyle.set(this.subItemContainer, "width", (this._subItems.length * 48) + "px");
                        domStyle.set(this.subItemContainer, "height", "");
                        break;
                }
                domClass.add(this.subItemContainer, "visible");
                var length = this.subItemPosition === "top" || this.subItemPosition === "left" ? this._subItems.length - 1 : null;
                array.forEach(this._subItems, lang.hitch(this, function (item, index) {
                    this.subItemPosition === "top" || this.subItemPosition === "left" ? length : null;
                    setTimeout(lang.hitch(this, function () {
                        domClass.add(item.domNode, "visible");
                    }), ((length ? length - index : index) * 25));
                }));
            }
        }
    });
});