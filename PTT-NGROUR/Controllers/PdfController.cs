using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;



namespace PTT_NGROUR.Controllers
{
    public class PdfController : Controller
    {
        //
        // GET: /Pdf/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost] public ActionResult UtilizationReportPdf(Models.ViewModel.ModelUtilizationReportPdfInput pModel)
        {
            var result = new Models.ViewModel.ModelJsonResult<string>();
            try
            {
                var session = new PTT_NGROUR.ExtentionAndLib.SessionManager();
                session.UtilizationReportPdfInput = pModel;
                result.SetResultValue("OK");
                session = null;
            }catch(Exception ex)
            {
                result.SetException(ex);
            }
            return Json(result , JsonRequestBehavior.AllowGet);
        }

         public ActionResult UtilizationReportPdf()
        {
            var result = new Models.ViewModel.ModelUtilizationReportPdfOutput();
            var session = new PTT_NGROUR.ExtentionAndLib.SessionManager();
            var searchCondition = session.UtilizationReportPdfInput;
            session = null;
            //searchCondition.SearchType

            string strCommandCurrent = "select threshold , type , month ,year from VIEW_GATE_PIPE_REPORT_Current where 1=1";
            string strCommand = "select * from VIEW_GATE_PIPE_REPORT where 1=1";
            if (searchCondition != null)
            {
                if(searchCondition.ArrID !=null && searchCondition.ArrID.Any())
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
                if(!string.IsNullOrEmpty(searchCondition.Threshold) 
                && !"all".Equals(searchCondition.Threshold.Trim().ToLower()))
                {
                    strCommand += " and threshold ='" + searchCondition.Threshold.Trim().Replace("'","''") + "'";
                }
                if (!string.IsNullOrEmpty(searchCondition.SearchType) 
                && !"all".Equals(searchCondition.SearchType.Trim().ToLower()))
                {
                    strCommand += " and type ='" + searchCondition.SearchType.Trim().Replace("'", "''") + "'";                    
                }
                if (!string.IsNullOrEmpty(searchCondition.Period))
                {
                    var arrPeriod = searchCondition.Period.Split('/');
                    if(arrPeriod != null && 2.Equals(arrPeriod.Length))
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
            return new Rotativa.ViewAsPdf(result)
            {
                CustomSwitches = "--page-offset 0 --header-right [page] --header-font-size 12"
            };
        }
    }
}
