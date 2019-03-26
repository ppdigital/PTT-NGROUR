using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PTT_NGROUR.DTO;
using PTT_NGROUR.Models.ViewModel;
using PTT_NGROUR.ExtentionAndLib;
using static PTT_NGROUR.Models.ViewModel.ModelOmIndex;
using PTT_NGROUR.Models.DataModel;

namespace PTT_NGROUR.Controllers
{
    public class OMController : Controller
    {
        //
        // GET: /OM/
        //S[PTT_NGROUR.Controllers.AuthorizeController.CustomAuthorize]
        public ActionResult Index()
        {
            var dto = new DtoOM();
            var result = new ModelOmIndex();
            result.ListMeterMaintenance = dto.GetListMeterMaintenance().ToList();
            result.ListOmColor = dto.GetListOmColor().ToList();
            result.BarGraph = dto.GetModelBarGraph(
                pListModelMeterMaintenance: result.ListMeterMaintenance, 
                pListModelOmColor: result.ListOmColor);
            result.ListRegion = dto.GetListRegion()
                .OrderBy(x=> x.REGION_NAME.Length)
                .ThenBy(x=> x.REGION_NAME)
                .ToList();
            dto = null;
            GC.Collect();
            return View(result);
        }

        [HttpPost]
        public ActionResult SearchData(string pStrYear, string pStrMonth, string[] pArrRegion)
        {
            var result = new ModelJsonResult<ModelOmIndex>();
            try
            {
                ModelOmIndex modelOm = new ModelOmIndex();
                var dto = new DtoOM();
                var listAllMM = dto.GetListMeterMaintenance(string.Empty, pStrYear, pArrRegion).ToList();
                List<ModelMeterMaintenance> listMM = null;
                int intYear = pStrYear.GetInt();
                int intMonth = pStrMonth.GetInt();
                if (intMonth > 0)
                {
                    listMM = listAllMM.Where(x => intMonth.Equals(x.MONTH)).ToList();
                }
                else
                {
                    listMM = listAllMM;
                }
                //var listMM = dto.GetListMeterMaintenance(pStrMonth, pStrYear, pArrRegion).ToList();
                var listColor = dto.GetListOmColor().ToList();

                IEnumerable<ModelPipelineMonitoringResults> listPipeline = dto.GetListPipelineMonitoringResults(pStrMonth, pStrYear, pArrRegion, true);

                modelOm.Year = intYear;
                modelOm.Month = intMonth;
                modelOm.PipelineActivity = dto.GetPipelineActivity();
                modelOm.Pipeline = new ModelPipeline
                {
                    Summary = new ModelPipelineSummary(intMonth, listPipeline),
                    Results = new ModelPipelineResults(intMonth, listPipeline),
                };
                modelOm.BarGraph = dto.GetModelBarGraph(listMM, listColor);
                modelOm.ListRegionForTableHeader = dto.GetListRegionForTableHeader(listMM);
                modelOm.ListMeterMaintenanceLevelForTable = dto.GetModelModelMeterMaintenanceLevel(listMM, modelOm.ListRegionForTableHeader);
                modelOm.ListAccGraph = dto.GetModelAccGraph(listAllMM);
                result.SetResultValue(modelOm);
                listMM.Clear();
                listMM = null;
                listColor.Clear();
                listColor = null;
                dto = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }
            return Json(result);
        }

        [HttpPost]
        public ActionResult SearchData2(string pStrYear , string pStrMonth , string[] pArrRegion)
        {
            var result = new ModelJsonResult<ModelOmIndex.ModelBarGraph>();
            try
            {
                var dto = new DtoOM();                
                var listMM = dto.GetListMeterMaintenance(pStrMonth , pStrYear , pArrRegion).ToList() ;                
                var listColor = dto.GetListOmColor().ToList();
                var modelBarGraph = dto.GetModelBarGraph(listMM, listColor);
                result.SetResultValue(modelBarGraph);
                listMM.Clear();
                listMM = null;
                listColor.Clear();
                listColor = null;
                dto = null;
                GC.Collect();
            }
            catch(Exception ex)
            {
                result.SetException(ex);
            }
            return Json(result);
        }

        public JsonResult Test()
        {
            var modelOm = new ModelOmIndex();
            var dto = new DtoOM();
            var listMM = dto.GetListMeterMaintenance().ToList();
            var listRegionHeader = dto.GetListRegionForTableHeader(listMM);
            var result =  dto.GetModelModelMeterMaintenanceLevel(listMM , listRegionHeader);
            return Json(result ,  JsonRequestBehavior.AllowGet);
        }

    }
}
