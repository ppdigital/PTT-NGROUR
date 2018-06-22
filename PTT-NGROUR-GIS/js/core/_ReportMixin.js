/**
 * @description 
 * -
 * @class _ReportMixin
 */
define([
    //"esrith/amos/core/DataStore",

    //"config/app",
    "dojo/dom",
    "dojo/dom-construct",
    "dojo/json",
    //"dojo/aspect",
    "dojo/_base/lang",
    "dojo/_base/declare",
    "require"
], function (
    //DataStore,

    //appConfig,
    dom,
    domConstruct,
    JSON,
    //aspect,
    lang,
    declare,
    require
    ) {

    var wgConstant = {
        reqOperations: new Array()
    };

    var wgDeclare = declare(/*[DECLARED_CLASS]*/null, {

        _defaultReportPage: "reports/Report.aspx",

        /**
        * Open report
        * @method openReport
        * @public
        * @param {Object} parameters 
        * @param {Object} datasets
        * @param {Object} config
        */
        openReport: function (parameters, datasets, config) {
            try {
                var pageUrl = config.REPORT_PAGE != null && config.REPORT_PAGE != "" ? config.REPORT_PAGE : this._defaultReportPage;
                var target = config.IFRAME != null && config.IFRAME != "" ? config.IFRAME : "_blank";

                var reportForm = dom.byId("reportForm");
                if (reportForm == null) {
                    reportForm = domConstruct.create("form", { id: "reportForm", method: "post", target: target }, document.body, "last");
                } else {
                    domConstruct.empty(reportForm);
                }
                reportForm.action = pageUrl;

                this._createParams("rptParams", parameters, reportForm);
                this._createParams("rptDatasets", datasets, reportForm);
                this._createParams("rptConfig", config, reportForm);

                reportForm.submit();
            } catch (err) {
                console.error(err);
            }
        },

        _createParams: function (name, value, form) {
            domConstruct.create("input", { type: "hidden", value: JSON.stringify(value), name: name }, form, "last");
        }
    });

    return wgDeclare;
});