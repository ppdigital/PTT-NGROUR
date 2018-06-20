using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTT_NGROUR.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/

        public ActionResult Index()
        {
            var dal = new DAL.DAL();
            var ds = dal.GetDataSet("SELECT LICENSE_ID ,LICENSE FROM LICENSE_MASTER");
            var dt = ds.Tables[0];
            var listLicense = new List<Models.DataModel.ModelLicenseMaster>();

            foreach (System.Data.DataRow dr in dt.Rows)
            {
                var license = new Models.DataModel.ModelLicenseMaster()
                {
                    LICENSE = dr["LICENSE"].ToString(),
                    LICENSE_ID = Convert.ToInt32(dr["LICENSE_ID"].ToString())
                };


                listLicense.Add(new Models.DataModel.ModelLicenseMaster { LICENSE = license.LICENSE, LICENSE_ID = license.LICENSE_ID });
            }
            //  var jsonResult = Json(listLicense.Distinct(), JsonRequestBehavior.AllowGet);
            //  jsonResult.MaxJsonLength = int.MaxValue;

            var dsRegion = dal.GetDataSet("SELECT REGION_ID ,REGION_NAME FROM REGION");
            var dtRegion = dsRegion.Tables[0];
            var listRegion = new List<Models.DataModel.ModelRegion>();

            foreach (System.Data.DataRow drArea in dtRegion.Rows)
            {
                var region = new Models.DataModel.ModelRegion()
                {
                    REGION_NAME = drArea["REGION_NAME"].ToString(),
                    REGION_ID = Convert.ToInt32(drArea["REGION_ID"].ToString())
                };


                listRegion.Add(new Models.DataModel.ModelRegion { REGION_NAME = region.REGION_NAME, REGION_ID = region.REGION_ID });
            }

            //  ViewBag.seLicense = listLicense;

            var model = new Models.ViewModel.ModelUtilization()
            {
                ListLicense = listLicense,
                ListRegion = listRegion

            };

            return View(model);
        }

        [HttpPost]
        public JsonResult SearchRegion(int[] region)
        {
            var dal = new DAL.DAL();
            string regionStr = string.Join("','", region);
            var searchregion = @"select * from VIEW_GATEPIPEMETER_MENU WHERE REGION IN ('" + regionStr + "')";
            var ds = dal.GetDataSet(searchregion);


            var listRegion = new List<Models.DataModel.ModelGetU>();
            if (ds.Tables[0].Rows.Count > 0)
            {


                foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                {
                    var reg = new Models.DataModel.ModelGetU()
                    {
                        NO = dr["NO"].ToString(),
                        NAME = dr["NAME"].ToString(),
                        COLOR = dr["COLOR"].ToString(),
                        VALUE = dr["VALUE"].ToString(),
                        TYPE = dr["TYPE"].ToString()
                    };
                    listRegion.Add(reg);
                }
            }

            return Json(listRegion, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SearchLicense(int[] license)
        {
            var dal = new DAL.DAL();
            string licenseStr = string.Join("','", license);
            var searchlicense = @"select * from VIEW_GATEPIPEMETER_MENU WHERE LICENSE IN ('" + licenseStr + "')";
            var ds = dal.GetDataSet(searchlicense);


            var listLicense = new List<Models.DataModel.ModelGetU>();
            if (ds.Tables[0].Rows.Count > 0)
            {


                foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                {
                    var reg = new Models.DataModel.ModelGetU()
                    {
                        NO = dr["NO"].ToString(),
                        NAME = dr["NAME"].ToString(),
                        COLOR = dr["COLOR"].ToString(),
                        VALUE = dr["VALUE"].ToString(),
                        TYPE = dr["TYPE"].ToString()
                    };
                    listLicense.Add(reg);
                }
            }

            return Json(listLicense, JsonRequestBehavior.AllowGet);
        }

    }
}
