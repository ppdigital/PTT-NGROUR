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
    "esri/geometry/Polyline",
    "esri/symbols/SimpleLineSymbol",
    "esri/graphic",
    "esri/Color",

    "esrith/amos/form/FormValidator",

    "system/gis/GisFunction/EditStatusInfoTemplate/InfoTitle",

    "esrith/amos/form/Button",
    "esrith/amos/form/RadioButton",
    "esrith/amos/form/TextArea",
    "esrith/amos/form/TextBox"

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
    InfoWindow,
    Polyline,
    SimpleLineSymbol,
    graphic,
    Color,

    FormValidator,

    InfoTitle


    ) {
    return declare([
        _WidgetBase
    ], {
        baseClass: "EditStatusInfoTemplate",
        templateString: template,
        infoTemplate: null,
        updateSP: null,
        params: null,
        currentInfoPage: null,
        numberInfoPage: null,
        shape: null,
        hilightColor: null,
        hilightGraphic: null,

        _previousEvt: null,
        _nextEvt: null,
        _infoHeader: null,

        _clickPage: false,

        constructor: function (params, srcNodeRef) {
            params = params || {};
            this.currentInfoPage = 0;
        },
        postCreate: function () {
            this.inherited(arguments);

            this.own(
                on(this.flagButton, "click", lang.hitch(this, 'goToEditPage')),
                on(this.backButton, "click", lang.hitch(this, 'goToInfoPage')),
                on(this.saveButton, "click", lang.hitch(this, 'updateStatus'))//,
                //on(this.previousLine, "click", lang.hitch(this, 'previousPage')),
                //on(this.nextLine, "click", lang.hitch(this, 'nextPage'))
            );

        },
        setUpdateSP: function (updateSp, params, misData) {
            //params = params || {};
            //console.log(updateSp, params);
            this.updateSP = updateSp;
            this.params = {};
            this.params.FLAGBY = misData.flagby;
            this.params.MONTH = misData.parameter.MONTH;
            this.params.YEAR = misData.parameter.YEAR;
            Object.keys(params).map(lang.hitch(this, function (fieldName) {
                if (!fieldName.match(/^(GEOMETRY|COLOR)$/)) {
                    this.params[fieldName] = params[fieldName];
                }
            }));
        },
        setShape: function (shape) {
            this.shape = shape;
        },
        setHilightColor: function (hilightColor) {
            this.hilightColor = hilightColor;
        },
        setMultiInfoTemplate: function (data, fieldDetail, header) {
            var attrTable = [],
                name = data[0].TITLE_INFO;
            this.numberInfoPage = data.length;

            //## Nannie comment(17-7-2018) : change to widget InfoTitle
            //if (this.numberInfoPage == 1) {
            //    this.hide(this.infoButton);
            //} else {
            //    this.show(this.infoButton);
            //}
            console.log("data", data);
            data.map(lang.hitch(this, function (lineData, i) {
                //console.log("lineData", lineData);
                //this.comment.set('value', lineData.FLAG_COMMENT);
                this.frmValidator.set('value', { status: lineData.FLAG_STATUS, comment: lineData.FLAG_COMMENT, updatedDate: lineData.UPDATED_DATE });
                attrTable += '<table style="width: 100%;" data-dojo-attach-point="info' + i + '" class="amos-hide">';
                Object.keys(lineData).map(lang.hitch(this, function (fieldName) {
                    if (!fieldName.match(/^(GEOMETRY|COLOR|FLAG_COMMENT|FLAG_STATUS|TITLE_INFO|CONTENT_INFO)$/)) {
                        var field = this.find(fieldDetail, 'FIELDNAME', fieldName);
                        if (field != null) {
                            var attrContent = '<tr><td style="width: 150px; color: #337FD4;">' + field.DISPLAY + '</td><td style="color: #337FD4;">:</td><td>' + lineData[fieldName] + '</td></tr>';
                            attrTable += attrContent;
                        }
                    }
                    else {
                        if (fieldName == "CONTENT_INFO") {
                            attrContent = '<tr colspan="2"><td style="width: 150px; color: #e57d21; font-weight:bold; padding-bottom:5px">' + lineData[fieldName] + '</td></tr>';
                            attrTable += attrContent;
                        }
                    }
                }));
                attrTable += '</table>';
            }));

            //## Nannie add(17-7-2018) : set header title with page
            var argu = { Header: header, Name: name, TotalFeature: this.numberInfoPage };
            this._infoHeader = new InfoTitle(argu);

            if (this._previousEvt != null) {
                this._previousEvt.remove();
                this._previousEvt = null;
            }
            this._previousEvt = on(this._infoHeader, "previous-feature-click", lang.hitch(this, "previousPage"));

            if (this._nextEvt != null) {
                this._nextEvt.remove();
                this._nextEvt = null;
            }
            this._nextEvt = on(this._infoHeader, "next-feature-click", lang.hitch(this, "nextPage"));

            this.infoTemplate = new InfoTemplate(
                this._infoHeader.domNode.innerHTML,
                attrTable
            );

            this.infoWidget.title = header + ' : ' + name;
            this.infoWidget.headerInfo = this._infoHeader.domNode;
            //##

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
        showFirstPage: function () {
            this._clickPage = false;
            domClass.remove(this.infoContainer.children[0], 'amos-hide');
            this.slideTo(0);
        },
        previousPage: function () {
            this._clickPage = true;
            this.slideTo(-1);
        },
        nextPage: function () {
            this._clickPage = true;
            this.slideTo(1);
        },
        slideTo: function (i) {
            var currentPage = this.currentInfoPage,
                nextPage = ((this.currentInfoPage + i) % this.numberInfoPage + this.numberInfoPage) % this.numberInfoPage;
            this.currentInfoPage = nextPage;
            //console.log(this.currentInfoPage);
            domClass.add(this.infoContainer.children[currentPage], 'amos-hide');
            domClass.remove(this.infoContainer.children[nextPage], 'amos-hide');
            this.map.infoWindow.hide();

            var geometry;
            if (this.shape.type === 'point') {
                geometry = this.shape;
            } else {
                var medianPoint = Math.floor(this.shape.paths[this.currentInfoPage].length / 2);
                geometry = this.shape.getPoint(this.currentInfoPage, medianPoint);

                this.addHilight();
            }
            if (!this._clickPage) {
                this.map.infoWindow.clearFeatures();
                this.map.infoWindow.setContent(this.infoWidget);
                this.map.infoWindow.setTitle(this.infoWidget.headerInfo);
            }
            this.map.infoWindow.show(geometry);

            this.map.resize(true);
            this.map.reposition();
            this.map.centerAndZoom(geometry, 17);

            // Nannie add (17-7-2018) : แสดง label ของ page
            this._infoHeader.setCurrentPage(this.currentInfoPage + 1);

        },
        addHilight: function () {
            var newColor = Color.fromHex(this.hilightColor);
            newColor.a = 0.65;
            //generate opposite color
            //newColor = { r: 255 - newColor.r, g: 255 - newColor.g, b: 255 - newColor.b, a: 0.75 };
            //newColor = { r: 96, g: 19, b: 124, a: 0.75 };

            var lineSymbol = new SimpleLineSymbol(SimpleLineSymbol.STYLE_SOLID, new Color(newColor), 15);
            var polyline = new Polyline({
                paths: [this.shape.paths[this.currentInfoPage]],
                spatialReference: this.shape.spatialReference,
                cache: this.shape.cache
            });

            this.removeHilight();
            this.hilightGraphic = new graphic(polyline, lineSymbol);
            this.map.graphics.add(this.hilightGraphic);
        },
        removeHilight: function () {
            if (this.hilightGraphic != null) {
                this.map.graphics.remove(this.hilightGraphic);
            }
        },
        showFlagButton: function () {
            this.show(this.flagButton);
        },
        goToEditPage: function () {
            this.hide(this.infoPage);
            this.show(this.editPage);
            this.infoWidget.scrollTop = 0;
        },
        goToInfoPage: function () {
            this.show(this.infoPage);
            this.hide(this.editPage);
            this.infoWidget.scrollTop = 0;
        },
        updateStatus: function () {
            this.params.FLAG_STATUS = this.frmValidator.get("value").status;
            this.params.FLAG_COMMENT = this.comment.get("value");
            this.reqSP(this.updateSP, this.params).query(lang.hitch(this, function (response) {
                console.log(response);
                if (response.success == true) {

                    this.alert(response.message);
                    this.goToInfoPage();
                    window['map'] = this.map;

                    var layerData = this.map.getLayer('PTTOUR_DATA_SYMBOL');
                    if (layerData) {
                        layerData.refresh();
                    }

                    //## Nannie add(23-7-2018) : emit to view.js for refresh highlight color and other.
                    this.emit("update-status-completed");

                } else {
                    this.alert('เกิดข้อผิดพลาด:', response.message);
                }
            }));
        },

        setUpdatedDate: function (value) {
            this.updatedDate.set("value", value);
        }
    });
});