define([
    "core/_WidgetBase",
    "core/_ReportMixin",
    "dojo/text!./PrintMap.html",
    "xstyle/css!./PrintMap.css",

    "esri/tasks/PrintTask",
    "esri/tasks/PrintTemplate",
    "esri/tasks/PrintParameters",

    "esrith/amos/form/RadioButtonList",
    "esrith/amos/form/TextBox",
    "esrith/amos/form/Button",


    "dojo/_base/declare",
    "dojo/_base/lang",
    "dojo/_base/array",
    "dojo/on",
    "dojo/dom-class",
	"dojo/dom-construct",

    "manager/ConfigManager"

], function (
    _WidgetBase,
    _ReportMixin,
    template,
    xstyle,

    PrintTask,
    PrintTemplate,
    PrintParameters,

    CheckBox,
    TextBox,
    Button,

    declare,
    lang,
    array,
    on,
    domClass,
    domConstruct,

    ConfigManager
    ) {
    return declare([
        _WidgetBase, _ReportMixin
    ], {
        baseClass: "print-map",
        templateString: template,

        configures: null,
        _defaultBasemap: null,

        _paperType: {},
        _paperSize: {},
        _exportType: {},
        _SIZEA4: {},
        _SIZEA3: {},

        constructor: function (params, srcNodeRef) {
            params = params || {};
            this._paperSize = [
                {
                    CODE: 1,
                    DESCR: "A3"
                },
                {
                    CODE: 2,
                    DESCR: "A4"
                }
            ];
            this._paperType = [
                {
                    CODE: 1,
                    DESCR: "Portrait"
                },
                {
                    CODE: 2,
                    DESCR: "Landscape"
                }
            ];
            this._exportType = [
                {
                    CODE: 1,
                    DESCR: "Word"
                },
                {
                    CODE: 2,
                    DESCR: "PDF"
                },
                {
                    CODE: 3,
                    DESCR: "Image"
                }
            ];
            this._SIZEA3 = {
                width: '',
                height: ''
            };

            this._SIZE = {
                1: { // Portrait
                    width: 825,
                    height: 1190
                },
                2: { //Landscape
                    width: 1290,
                    height: 825
                }
            }
        },

        postCreate: function () {
            this.inherited(arguments);
            this.configures = ConfigManager.getInstance().getAllConfig();
            this._init();
        },

        _init: function () {
            this._initEvent();
            this.radioListSize.bind(this._paperSize);
            this.radioListType.bind(this._paperType);
            this.radioListExport.bind(this._exportType);

            this.radioListSize.set("value", 2);     //ขนาดกระดาษ
            this.radioListType.set("value", 1);     //แนวกระดาษ
            this.radioListExport.set("value", 3);   //รูปแบบผลลัพธ์
        },

        _initEvent: function () {
            this.own(
                on(this.btnPrint, "click", lang.hitch(this, "_btnPrint_Click"))
            );
        },

        _btnPrint_Click: function () {

            let _valid = this._getFormValid();
            if (_valid) {
                this.confirm("ยืนยันการพิมพ์ข้อมูล", lang.hitch(this, function (confirm) {
                    if (confirm) {
                        this.displayLoader(true);

                        let dpi = 96;
                        let width = this._SIZE[this.radioListType.get("value")[0]].width;
                        let height = this._SIZE[this.radioListType.get("value")[0]].height;
                        let template = new PrintTemplate();
                        template.exportOptions = {
                            width: width,
                            height: height,
                            dpi: dpi
                        };
                        template.format = "png32";
                        template.layout = "MAP_ONLY";
                        template.preserveScale = true;
                        let _printServiceUrl = _configManager._configures.mapConfig.layers.filter(function (item) { return item.option.id === "PTT_PRINT" })[0].url;
                        let printTask = new PrintTask(_printServiceUrl);
                        var params = new PrintParameters();
                        params.map = this.map;
                        params.template = template;
                        printTask.execute(params, lang.hitch(this, "_printComplete"), lang.hitch(this, "_printError"));
                    }
                }));

            } else {
                this.alert('กรุณาระบุค่าให้ถูกต้อง');
            }
        },

        _getFormValid: function () {
            //return !!this.divTitle.get("value") && !!(this.radioListSize.get("value")[0]) && !!(this.radioListType.get("value")[0]) && !!(this.radioListExport.get("value")[0]);
            return !!(this.radioListSize.get("value")[0]) && !!(this.radioListType.get("value")[0]) && !!(this.radioListExport.get("value")[0]);
        },

        _getFileExtension: function (val) {
            if (val == 1) return ".doc"
            else if (val == 2) return ".pdf"
            else if (val == 3) return ".png"
        },

        _printComplete: function (res) {
            this.displayLoader(false);

            var params = this.divForm.get("value");
            let rpt = new _ReportMixin();
            var config = {
                "RDLC_PATH": "print/",
                "REPORT_PAGE": "reports/Report.aspx",
                "REPORT_NAME": this.divTitle.get("value"),
                "DISABLE_EXPORT": "excel",
                "IFRAME": null,
                "EXTENSION": this._getFileExtension(params.FILE_EXTENSION),
                "FILE_TYPE": params.FILE_TYPE,
                "PAPER_SIZE": params.PAPER_SIZE
            };

            let parameters = {
                IMAGE_URL: res.url,
                TITLE: this.divTitle.get("value")
            };

            rpt.openReport(parameters, {}, config);

        },

        _printError: function (res) {
            console.log("print error.");
        }

    });
});