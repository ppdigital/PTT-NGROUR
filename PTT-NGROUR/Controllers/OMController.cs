using PTT_NGROUR.DTO;
using PTT_NGROUR.ExtentionAndLib;
using PTT_NGROUR.Models.DataModel;
using PTT_NGROUR.Models.ViewModel;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static PTT_NGROUR.Models.DataModel.ModelOMAccumulated;
using static PTT_NGROUR.Models.DataModel.ModelOMCompletion;
using static PTT_NGROUR.Models.DataModel.ModelOMSummary;

namespace PTT_NGROUR.Controllers
{
    public class OMController : Controller
    {
        //
        // GET: /OM/
        [AuthorizeController.CustomAuthorize]
        public ActionResult Index(string radioMY, string pStrYear, string pStrMonth, string[] pArrRegion)
        {
            if (radioMY == "year") pStrMonth = null;
            string mode = radioMY == "year" ? "yearly" : "monthly";

            ModelJsonResult<ModelOmIndex> data = GetData(pStrYear, pStrMonth, pArrRegion, mode);

            return View(data);
        }

        [HttpGet]
        public ActionResult Print(string radioMY, string pStrYear, string pStrMonth, string pStrRegion)
        {
            if (radioMY == "year") pStrMonth = null;
            string mode = radioMY == "year" ? "yearly" : "monthly";

            string[] pArrRegion = pStrRegion == null ? null : pStrRegion.Split(',');

            ModelJsonResult<ModelOmIndex> data = GetData(pStrYear, pStrMonth, pArrRegion, mode);
            
            return View(data);
        }

        [HttpPost]
        [AuthorizeController.CustomAuthorize]
        public ActionResult SearchData(string pStrYear, string pStrMonth, string[] pArrRegion)
        {
            string mode = pStrMonth == null ? "yearly" : "monthly";

            return Json(GetData(pStrYear, pStrMonth, pArrRegion, mode));
        }

        [HttpGet]
        [AuthorizeController.CustomAuthorize]
        public ActionResult Export(string radioMY, string pStrYear, string pStrMonth, string[] pArrRegion)
        {
            return new ActionAsPdf("Print", new
            {
                radioMY,
                pStrYear,
                pStrMonth,
                pStrRegion = string.Join(",", pArrRegion),
            }) {
                //FileName = "Test.pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Landscape,
                PageMargins = new Margins(5, 0, 5, 0),
                CustomSwitches = "--enable-javascript"
            };
        }

        private ModelJsonResult<ModelOmIndex> GetData(string pStrYear, string pStrMonth, string[] pArrRegion, string mode)
        {
            ModelJsonResult<ModelOmIndex> result = new ModelJsonResult<ModelOmIndex>();
            try
            {
                // Inint
                ModelOmIndex modelOm = new ModelOmIndex();
                DtoOM dto = new DtoOM();
                int _intMonth = pStrMonth == null ? DateTime.Now.Month : pStrMonth.GetInt();
                int intYear = pStrYear == null ? DateTime.Now.Year : pStrYear.GetInt();
                int intMonth = mode.Equals("yearly") ? 12 : _intMonth;
                var maintenanceLevel = dto.GetListMaintenanceLevelColor();

                // Master Data
                modelOm.ListRegion = dto.GetListRegion()
                    .OrderBy(x => x.REGION_NAME.Length)
                    .ThenBy(x => x.REGION_NAME)
                    .ToList();
                modelOm.Mode = mode;
                modelOm.Year = intYear;
                modelOm.Month = intMonth;
                modelOm.Master = new ModelOMMaster
                {
                    Pipeline = dto.GetPipelineActivity(),
                    MaintenanceLevel = maintenanceLevel
                };

                // Summary
                List<ModelMonitoringResults> listPipeline = dto.GetListOMPipelineHistory(intMonth, intYear, pArrRegion, true);
                List<ModelPlanYearly> listPipelinePlan = dto.GetListOMPipelinePlanYearly(intYear, pArrRegion);
                List<ModelMonitoringResults> listGate = dto.GetListOMGateHistory(intMonth, intYear, pArrRegion, true);
                List<ModelPlanYearly> listGatePlan = dto.GetListOMGatePlanYearly(intYear, pArrRegion);
                List<ModelMonitoringResults> listMeter = dto.GetListOMMeterHistory(intMonth, intYear, pArrRegion, true);
                List<ModelPlanYearly> listMeterPlan = dto.GetListOMMeterPlanYearly(intYear, pArrRegion);

                modelOm.Summary = new ModelOMSummary
                {
                    Pipeline = new ModelOMSummaryPipeline(intMonth, listPipeline, mode, listPipelinePlan),
                    Gate = new ModelOMSummaryMaintenanceLevel(intMonth, intYear, listGate, mode, listGatePlan),
                    Meter = new ModelOMSummaryMaintenanceLevel(intMonth, intYear, listMeter, mode, listMeterPlan),
                };

                modelOm.Completion = new ModelOMCompletion
                {
                    Pipeline = new ModelOMCompletionPipeline(intMonth, listPipeline, mode),
                    Gate = new ModelOMCompletionMaintenanceLevel(intMonth, intYear, listGate, mode),
                    Meter = new ModelOMCompletionMaintenanceLevel(intMonth, intYear, listMeter, mode),
                };

                ModelOMAccumulated accumulated = new ModelOMAccumulated();

                accumulated.getPipeline(intMonth, intYear, listPipeline, mode);
                accumulated.getGate(intMonth, intYear, listGate, mode);
                accumulated.getMeter(intMonth, intYear, listMeter, mode);
                modelOm.Accumulated = accumulated;

                result.SetResultValue(modelOm);
                dto = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                throw ex;
                //result.SetException(ex);
            }

            return result;
        }
    }
}
