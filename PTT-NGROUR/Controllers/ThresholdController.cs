using PTT_NGROUR.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PTT_NGROUR.ExtentionAndLib;

namespace PTT_NGROUR.Controllers
{
    public class ThresholdController : Controller
    {
        //
        // GET: /Threshold/
        [PTT_NGROUR.Controllers.AuthorizeController.CustomAuthorize]
        public ActionResult Index()
        {

            string strQuery = @"select 
                PIPELINE_THRESHOLD_ID THRESHOLD_ID , 
                'PipeLine' ThresholdType ,
                COLOR , 
                MINVAL , 
                MAXVAL 
            from PIPELINE_THRESHOLD
            union all
            select 
                GATE_THRESHOLD_ID THRESHOLD_ID , 
                'GateStation' ThresholdType ,
                COLOR , 
                MINVAL , 
                MAXVAL 
            from GATESTATION_THRESHOLD";
            var dal = new DAL.DAL();
            var listThreshold = dal.ReadData<ModelThresholdItem>(strQuery, reader =>
            {
                var th = new ModelThresholdItem();
                th.Color = reader["COLOR"].GetString();
                th.MaxValue = reader["MAXVAL"].GetDecimal();
                th.MinValue = reader["MINVAL"].GetDecimal();
                th.ThresholdId = reader["THRESHOLD_ID"].GetInt();
                th.ThresholdType = reader["ThresholdType"].GetEnum<EnumThresholdType>(EnumThresholdType.None);
                return th;
            }).OrderBy(x=> x.ThresholdId);
            ModelThreshold model = new ModelThreshold() { 
                ThresholdItems = listThreshold.ToArray() 
            };
            return View(model);
        }

    }
}
