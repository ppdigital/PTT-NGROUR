using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PTT_NGROUR.DAL;
using PTT_NGROUR.DTO;
using System.Data;
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
        public JsonResult AllGate()
        {
            var dal = new DAL.DAL();
            var ds = dal.GetDataSet("SELECT * FROM VIEW_GATEPIPEMETER_MENU WHERE TYPE = 'GATESTATION' AND REGION IS NOT NULL");
            decimal green = 0;
            decimal red = 0;
            decimal yellow = 0;
            decimal total = 0;
            foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["COLOR"].Equals("Green"))
                {
                    green++;
                }
                else if (dr["COLOR"].Equals("Red"))
                {
                    red++;
                }
                else if (dr["COLOR"].Equals("Yellow"))
                {
                    yellow++;
                }

            }
            total = ds.Tables[0].Rows.Count;
            //green = (green / total) * 100;
            //red = (red / total) * 100;
            //yellow = (yellow / total) * 100;

            List<object> iData = new List<object>();
            //Creating sample data  
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", System.Type.GetType("System.String"));
            dt.Columns.Add("Color", System.Type.GetType("System.String"));

            DataRow dr1 = dt.NewRow();
            dr1 = dt.NewRow();
            dr1["Name"] = "Alert";
            dr1["Color"] = red;
            dt.Rows.Add(dr1);

            dr1 = dt.NewRow();
            dr1["Name"] = "Warning";
            dr1["Color"] = yellow;
            dt.Rows.Add(dr1);

            dr1 = dt.NewRow();
            dr1["Name"] = "Pass";
            dr1["Color"] = green;
            dt.Rows.Add(dr1);




            //Looping and extracting each DataColumn to List<Object>  
            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(x);
            }
            //Source data returned as JSON  
            return Json(iData, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult AllPipeline()
        {
            var dal = new DAL.DAL();
            var ds = dal.GetDataSet("SELECT * FROM VIEW_GATEPIPEMETER_MENU WHERE TYPE = 'PIPELINE' AND REGION IS NOT NULL");
            decimal green = 0;
            decimal red = 0;
            decimal yellow = 0;
            decimal total = 0;
            foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
            {
                if (dr["COLOR"].Equals("Green"))
                {
                    green++;
                }
                else if (dr["COLOR"].Equals("Red"))
                {
                    red++;
                }
                else if (dr["COLOR"].Equals("Yellow"))
                {
                    yellow++;
                }

            }
            total = ds.Tables[0].Rows.Count;
            //green = (green / total) * 100;
            //red = (red / total) * 100;
            //yellow = (yellow / total) * 100;

            List<object> iData = new List<object>();
            //Creating sample data  
            DataTable dt = new DataTable();
            dt.Columns.Add("Name", System.Type.GetType("System.String"));
            dt.Columns.Add("Color", System.Type.GetType("System.String"));

            DataRow dr1 = dt.NewRow();
            dr1 = dt.NewRow();
            dr1["Name"] = "Alert";
            dr1["Color"] = red;
            dt.Rows.Add(dr1);

            dr1 = dt.NewRow();
            dr1["Name"] = "Warning";
            dr1["Color"] = yellow;
            dt.Rows.Add(dr1);

            dr1 = dt.NewRow();
            dr1["Name"] = "Pass";
            dr1["Color"] = green;
            dt.Rows.Add(dr1);




            //Looping and extracting each DataColumn to List<Object>  
            foreach (DataColumn dc in dt.Columns)
            {
                List<object> x = new List<object>();
                x = (from DataRow drr in dt.Rows select drr[dc.ColumnName]).ToList();
                iData.Add(x);
            }
            //Source data returned as JSON  
            return Json(iData, JsonRequestBehavior.AllowGet);
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
