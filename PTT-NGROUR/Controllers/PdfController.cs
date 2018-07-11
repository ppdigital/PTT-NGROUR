using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Helpers;


namespace PTT_NGROUR.Controllers
{
    public class PdfController : Controller , System.Web.SessionState.IRequiresSessionState
    {
        //
        // GET: /Pdf/
        private static Models.ViewModel.ModelUtilizationReportPdfOutput _utilizationReportPdfOutput = null;

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UtilizationReportPdf(Models.ViewModel.ModelUtilizationReportPdfInput pModel)
        {
            var result = new Models.ViewModel.ModelJsonResult<string>();
            try
            {
                var session = new PTT_NGROUR.ExtentionAndLib.SessionManager();
                session.UtilizationReportPdfInput = pModel;
                result.SetResultValue("OK");
                session = null;
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult UtilizationReportPdf()
        {
            var result = new Models.ViewModel.ModelUtilizationReportPdfOutput();
            var session = new PTT_NGROUR.ExtentionAndLib.SessionManager();
            var searchCondition = session.UtilizationReportPdfInput;

            //searchCondition.SearchType

            string strCommandCurrent = "select threshold , type , month ,year from VIEW_GATE_PIPE_REPORT_Current where 1=1";
            string strCommand = "select * from VIEW_GATE_PIPE_REPORT where 1=1";
            if (searchCondition != null)
            {
                if (searchCondition.ArrID != null && searchCondition.ArrID.Any())
                {
                    string strId = string.Join(",", searchCondition.ArrID);
                    string strWhereId = string.Empty;
                    switch (searchCondition.SearchMode.Trim().ToLower())
                    {
                        case "region":
                            strWhereId = " and region in (" + strId + ")";
                            break;
                        case "license":
                            strWhereId = " and license in (" + strId + ")";
                            break;
                    }
                    strCommandCurrent += strWhereId;
                    strCommand += strWhereId;
                }
                if (!string.IsNullOrEmpty(searchCondition.Threshold)
                && !"all".Equals(searchCondition.Threshold.Trim().ToLower()))
                {
                    strCommand += " and threshold ='" + searchCondition.Threshold.Trim().Replace("'", "''") + "'";
                }
                if (!string.IsNullOrEmpty(searchCondition.SearchType)
                && !"all".Equals(searchCondition.SearchType.Trim().ToLower()))
                {
                    strCommand += " and type ='" + searchCondition.SearchType.Trim().Replace("'", "''") + "'";
                }
                if (!string.IsNullOrEmpty(searchCondition.Period))
                {
                    var arrPeriod = searchCondition.Period.Split('/');
                    if (arrPeriod != null && 2.Equals(arrPeriod.Length))
                    {
                        strCommand += " and month =" + arrPeriod[0] + " and year =" + arrPeriod[1];
                    }
                    else
                    {
                        strCommand += " and year =" + searchCondition.Period;
                    }
                }
            }
            var dal = new DAL.DAL();
            var listGatePipeCurrent = dal.ReadData(
                pStrCommand: strCommandCurrent,
                pFuncReadData: x => new Models.DataModel.ModelViewGatePipeReport(x)
            ).ToList();
            var listGatePipe = dal.ReadData(
                pStrCommand: strCommand,
                pFuncReadData: x => new Models.DataModel.ModelViewGatePipeReport(x)
            ).ToList();
            dal = null;
            result.SetListGatePipeCurrent(listGatePipeCurrent);
            result.SetListGatePipe(listGatePipe);
            //return View(result);
            _utilizationReportPdfOutput = result;
            session.UtilizationReportPdfOutput = result;
            session = null;
            return new Rotativa.ViewAsPdf(result)
            {
                CustomSwitches = "--page-offset 0 --header-right [page] --header-font-size 12"
            };
        }

        private string getMyCustomTheme()
        {
            return @"
    <Chart BackColor=""white"" AntiAliasing=""All""
        Palette=""None"" 
        PaletteCustomColors=""#16a085; #f1c40f; #e74c3c; #e67e22; #ffffff"">
            
        <ChartAreas>
            <ChartArea Name=""Default"" _Template_=""All"" 
                BackGradientStyle=""DiagonalLeft""
                BackColor=""Transparent"" 
                ShadowColor=""Transparent"">                    
                <Area3DStyle Enable3D=""True"" Inclination=""45"" Rotation=""45""/>
            </ChartArea>
        </ChartAreas>         
        <Legends>
            <Legend _Template_=""All"" Font=""Trebuchet MS, 16pt, style = Bold"" IsTextAutoFit=""False"" />
          </Legends>
    </Chart>";
        }

        public FileResult GetChartImage(string pStrChartName)
        {
            if (string.IsNullOrEmpty(pStrChartName))
            {
                return null;
            }            
            if (_utilizationReportPdfOutput == null)
            {
                return null;
            }
            Models.ViewModel.ModelUtilizationReportPdfOutput.ThresholdStatus thStatus = null;
            switch (pStrChartName.Trim().ToLower())
            {
                case "currentgate":
                    thStatus = _utilizationReportPdfOutput.CurrentGate;
                    break;
                case "currentpipeline":
                    thStatus = _utilizationReportPdfOutput.CurrentPipeline;
                    break;
                case "gate":
                    thStatus = _utilizationReportPdfOutput.Gate;
                    break;
                case "pipeline":
                    thStatus = _utilizationReportPdfOutput.Pipeline;
                    break;
            }
            
            if (thStatus == null)
            {
                return null;
            }

            var strAlert = thStatus.Alert > 0 ? "Alert" : string.Empty;
            var strFlag = thStatus.Flag > 0 ? "Flag" : string.Empty;
            var strOK = thStatus.OK > 0 ? "OK" : string.Empty;
            var strWarning = thStatus.Warning > 0 ? "Wasrning" : string.Empty;

            var tc = new Chart(width: 400, height: 400, theme: getMyCustomTheme())
                .AddTitle(pStrChartName)
                .AddSeries(
                    name: string.Empty,
                    chartType: "Doughnut",
                    xValue: new[] { strOK, strWarning, strAlert, strFlag },
                    yValues: new[] { thStatus.OK, thStatus.Warning, thStatus.Alert, thStatus.Flag }
                );
           
            var arrChartByte = tc.GetBytes();
            tc = null;
            return File(arrChartByte, "image/png");
        }
    }
}
