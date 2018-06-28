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
            result.ListMeterMaintenance = dto.GetListMeterMaintenance();
            result.BarGraph = new ModelOmIndex.ModelBarGraph(result.ListMeterMaintenance);
            return View(result);
        }

    }
}
