define([
    "core/_WidgetBase",

    "dojo/text!./EditStatusInfoTemplate.html",
    "xstyle/css!./EditStatusInfoTemplate.css",

    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/dom-class",
	"dojo/dom-construct",

    "esri/geometry/Point",
    "esri/geometry/webMercatorUtils",
    "esri/InfoTemplate",
    "esri/dijit/InfoWindow",

    "esrith/amos/form/Button",
    "esrith/amos/form/RadioButton",
    "esrith/amos/form/TextArea"
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
    webMercatorUtils,
    InfoTemplate,
    InfoWindow
    ) {
    return declare([
        _WidgetBase
    ], {
        baseClass: "EditStatusInfoTemplate",
        templateString: template,
        infoTemplate: null,
        updateSP: null,
        params: null,

        constructor: function (updateSp, params) {
            //params = params || {};
            //console.log(updateSp, params);
            this.updateSP = updateSp;
            this.params = params;
        },
        postCreate: function () {
            this.inherited(arguments);

            this.own(
                on(this.flagButton, "click", lang.hitch(this,  this.goToEditPage)),
                on(this.backButton, "click", lang.hitch(this, this.goToInfoPage)),
                on(this.saveButton, "click", lang.hitch(this, this.updateStatus))
                
            );

        },

        setInfoTemplate: function (data, fieldDetail, header) {
            var attrTable = [],
                name;
            Object.keys(data).map(lang.hitch(this, function (fieldName) {
                if (fieldName.match(/^(NAME|RC_PROJECT|METER_NAME)$/)) {
                    name = data[fieldName];
                } else if (fieldName.match(/^(GEOMETRY|COLOR)$/)) {
                    // do nothing
                }
                else {
                    var field = this.find(fieldDetail, 'FIELDNAME', fieldName);
                    var attrContent = '<tr><td style="width: 100px; color: #337FD4;">' + field.DISPLAY + '</td><td style="color: #337FD4;">:</td><td>' + data[fieldName] + '</td></tr>';
                    attrTable += attrContent;
                }
            }));
            //console.log(attrTable);

            this.infoTemplate = new InfoTemplate(
                //'<div style="box-shadow: 0 1px 0 #42546b; padding: 0.5rem 0; margin-bottom: 0.5rem;">'+ this.header+' : ' + name + '</div>',
                 header + ' : ' + name,
                '<table style="width: 100%;">' +
                attrTable +
                '</table>'
            );
            this.infoWidget.title = header + ' : ' + name;
            domConstruct.place(this.infoTemplate.content, this.infoContainer);
        },
        getInfoTemplate: function () {
            //return this.infoTemplate;
            return this.infoWidget;
        },
        find: function (array, field, targetValue) {
            var result = null;
            array.forEach(function (object) {
                if (object[field] == targetValue) {
                    result = object;
                }
            });
            return result;
        },
        showFlagButton: function () {
            this.show(this.flagButton);
        },
        goToEditPage: function () {
            this.hide(this.infoPage);
            this.show(this.editPage);
        },
        goToInfoPage: function () {
            this.show(this.infoPage);
            this.hide(this.editPage);
        },
        updateStatus: function () {
            //this.params.STATUS = 
            console.log(this.x.value);
        }
    });
});