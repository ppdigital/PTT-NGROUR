define([
    "core/_WidgetBase",

    "dojo/text!./InfoTitle.html",
    "xstyle/css!./InfoTitle.css",

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
        baseClass: "info-title",
        templateString: template,

        _type: null,
        _name: null,
        _totalFeature: 0,

        constructor: function (params, srcNodeRef) {
            params = params || {};

            this._type = params.Header;
            this._name = params.Name;
            this._totalFeature = params.TotalFeature;
        },
        postCreate: function () {
            this.inherited(arguments);

            this._init();
            this._initEvent();
        },

        _init: function () {
            this.typeLabel.innerHTML = this._type + " : ";
            this.valueLabel.innerHTML = this._name;

            if (this._totalFeature == 1) {
                this.hide(this.pageContainer);
            }
            else {
                this.show(this.pageContainer);
                this.endPage.innerHTML = this._totalFeature;
            }
        },

        _initEvent: function () {
            this.own(
              on(this.divPrevious, "click", lang.hitch(this, "_previousClick")),
              on(this.divNext, "click", lang.hitch(this, "_nextClick"))
            );
        },

        _previousClick: function () {
            this.emit("previous-feature-click");
        },

        _nextClick: function (e) {
            this.emit("next-feature-click");
        },

        setCurrentPage: function (current) {
            this.currentPage.innerHTML = current;
        }

    });
});