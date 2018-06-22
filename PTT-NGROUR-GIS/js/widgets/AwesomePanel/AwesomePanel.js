define([
	"core/_WidgetBase",

    "dojo/_base/declare",

	"dojo/text!./AwesomePanel.html",
    "xstyle/css!./AwesomePanel.css",

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
        baseClass: "awesome-panel visible",
        templateString: template,

        backButtonVisibility: false,
        minimizeButtonVisibility: true,
        maximizeButtonVisibility: true,
        closeButtonVisibility: true,
        title: null,

        constructor: function (params, srcNodeRef) {
            params = params || {};
        },

        postCreate: function () {
            this.inherited(arguments);
            this._initEvent();
        },

        _setBackButtonVisibilityAttr: function (value) {
            this._set("backButtonVisibility", value);
            if (this.backButtonVisibility) {
                domClass.add(this.btnBack, "visible");
            } else {
                domClass.remove(this.btnBack, "visible");
            }
        },

        _setMinimizeButtonVisibilityAttr: function (value) {
            this._set("minimizeButtonVisibility", value);
            if (this.minimizeButtonVisibility) {
                domClass.add(this.btnMinimize, "visible");
            } else {
                domClass.remove(this.btnMinimize, "visible");
            }
        },

        _setMaximizeButtonVisibilityAttr: function (value) {
            this._set("maximizeButtonVisibility", value);
            if (this.maximizeButtonVisibility) {
                domClass.add(this.btnMaximize, "visible");
            } else {
                domClass.remove(this.btnMaximize, "visible");
            }
        },

        _setCloseButtonVisibilityAttr: function (value) {
            this._set("closeButtonVisibility", value);
            if (this.closeButtonVisibility) {
                domClass.add(this.btnClose, "visible");
            } else {
                domClass.remove(this.btnClose, "visible");
            }
        },

        _setTitleAttr: function (value) {
            this._set("title", value);
            this.titleNode.innerHTML = this.title;
        },

        _initEvent: function () {
            this.own(
                on(this.btnBack, "click", lang.hitch(this, lang.hitch(this, function () {
                    this.emit("back", {
                        target: this
                    });
                }))),
                on(this.btnMinimize, "click", lang.hitch(this, "toggleMinimization")),
                on(this.btnMaximize, "click", lang.hitch(this, "toggleMaximization")),
                on(this.btnClose, "click", lang.hitch(this, "toggleVisibility", false)),
                on(this.btnPrevious, "click", lang.hitch(this, "_navigate", -1)),
                on(this.btnNext, "click", lang.hitch(this, "_navigate", 1))
                );
        },

        toggleMinimization: function (minimization) {
            if (!domClass.contains(this.domNode, "visible")) {
                return;
            }
            if (typeof (minimization) !== "boolean") {
                minimization = null;
            }
            if (minimization === false || (minimization === null && domClass.contains(this.domNode, "minimize"))) {
                domClass.remove(this.domNode, "minimize");
            } else {
                domClass.add(this.domNode, "minimize");
            }
            this.emit("minimization-change", {
                target: this,
                minimize: domClass.contains(this.domNode, "minimize")
            });
        },

        toggleMaximization: function (maximization) {
            if (!domClass.contains(this.domNode, "visible")) {
                return;
            }
            if (typeof (maximization) !== "boolean") {
                maximization = null;
            }
            if (maximization === false || (maximization === null && domClass.contains(this.domNode, "maximize"))) {
                domClass.remove(this.domNode, "maximize");
            } else {
                domClass.add(this.domNode, "maximize");
            }
            this.emit("maximization-change", {
                target: this,
                maximize: domClass.contains(this.domNode, "maximize")
            });
        },

        toggleVisibility: function (visibility) {
            if (typeof (visibility) !== "boolean") {
                visibility = null;
            }
            if (visibility === false || (visibility === null && domClass.contains(this.domNode, "visible"))) {
                domClass.remove(this.domNode, "visible");
            } else {
                domClass.add(this.domNode, "visible");
            }
            this.emit("visibility-change", {
                target: this,
                visible: domClass.contains(this.domNode, "visible")
            });
        },

        _navigate: function (amount) {
            if ((amount === -1 && domClass.contains(this.btnPrevious, "disable")) || (amount === 1 && domClass.contains(this.btnNext, "disable"))) {
                return;
            }
            if (amount) {
                this._currentIndex += amount;
            }
            if (this._currentIndex == 0) {
                if (!domClass.contains(this.btnPrevious, "disable")) {
                    domClass.add(this.btnPrevious, "disable");
                }
            } else if (domClass.contains(this.btnPrevious, "disable")) {
                domClass.remove(this.btnPrevious, "disable");
            }
            if (this._currentIndex >= (this.widgets.length - 1)) {
                if (!domClass.contains(this.btnNext, "disable")) {
                    domClass.add(this.btnNext, "disable");
                }
            } else if (domClass.contains(this.btnNext, "disable")) {
                domClass.remove(this.btnNext, "disable");
            }
            if (this.containerNode.firstChild) {
                this.containerNode.removeChild(this.containerNode.firstChild);
            }
            domConstruct.place(this.widgets[this._currentIndex].domNode || this.widgets[this._currentIndex], this.containerNode);
            this.amountNode.innerHTML = (this._currentIndex + 1) + "/" + this.widgets.length;
            this.emit("index-change", {
                target: this,
                currentIndex: this._currentIndex,
                currentWidget: this.widgets[this._currentIndex]
            });
        },

        reset: function (keepNode) {
            this.set("backButtonVisibility", false);
            this.set("title", "");
            domClass.remove(this.domNode, "expand");
            domClass.remove(this.domNode, "minimize");
            domClass.remove(this.domNode, "maximize");
            this.clear(keepNode);
            this.emit("reset", {
                target: this
            });
        },

        clear: function (keepNode) {
            if (keepNode) {
                array.forEach(this.containerNode.children, lang.hitch(this, function (node) {
                    this.containerNode.removeChild(node);
                }));
            } else {
                array.forEach(this.widgets, lang.hitch(this, function (wg) {
                    try {
                        wg.destroyRecursive();
                    } catch (error) {
                        domConstruct.destroy(wg);
                    }
                }));
            }
            domConstruct.empty(this.containerNode);
            this.widgets = null;
            this._currentIndex = 0;
            domClass.remove(this.domNode, "with-navigator");
        },

        place: function (widgets) {
            this.clear();
            this.widgets = widgets;
            this._currentIndex = 0;
            domClass.remove(this.domNode, "with-navigator");
            if (this.widgets && this.widgets.length > 0) {
                if (this.widgets.length > 1) {
                    domClass.add(this.domNode, "with-navigator");
                }
                this._navigate();
            }
        }
    });
});