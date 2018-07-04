using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PTT_NGROUR.DTO;
using PTT_NGROUR.Models.ViewModel;
namespace PTT_NGROUR.Controllers
{
    public class OMController : Controller
    {
        //
        // GET: /OM/

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
        public ActionResult SearchData(string pStrYear , string pStrMonth , string[] pArrRegion)
        {
            var result = new ModelJsonResult<ModelOmIndex.ModelBarGraph>();
            try
            {
                var dto = new DtoOM();                
                var listMM = dto.GetListMeterMaintenance(pStrMonth , pStrYear) ;
                if(pArrRegion != null && pArrRegion.Any())
                {
                    
                    listMM = listMM.Where(x => pArrRegion.Contains(x.REGION));
                }
                listMM = listMM.ToList();
                var listColor = dto.GetListOmColor().ToList();
                var modelBarGraph = dto.GetModelBarGraph(listMM, listColor);
                result.SetResultValue(modelBarGraph);
                //listMM.Clear();
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
    }
}
