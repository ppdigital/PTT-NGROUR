using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
namespace PTT_NGROUR.Controllers
{
    public class DashboardController : Controller
    {
        //
        // GET: /Dashboard/
        [AuthorizeController.CustomAuthorize]
        public ActionResult Index()
        {
            DAL.DAL dal = new DAL.DAL();
            DataSet ds = dal.GetDataSet("SELECT LICENSE_ID ,LICENSE FROM LICENSE_MASTER");
            DataTable dt = ds.Tables[0];
            List<Models.DataModel.ModelLicenseMaster> listLicense = new List<Models.DataModel.ModelLicenseMaster>();

            foreach (DataRow dr in dt.Rows)
            {
                Models.DataModel.ModelLicenseMaster license = new Models.DataModel.ModelLicenseMaster()
                {
                    LICENSE = dr["LICENSE"].ToString(),
                    LICENSE_ID = Convert.ToInt32(dr["LICENSE_ID"].ToString())
                };


                listLicense.Add(new Models.DataModel.ModelLicenseMaster { LICENSE = license.LICENSE, LICENSE_ID = license.LICENSE_ID });
            }
            //  var jsonResult = Json(listLicense.Distinct(), JsonRequestBehavior.AllowGet);
            //  jsonResult.MaxJsonLength = int.MaxValue;

            DataSet dsRegion = dal.GetDataSet("SELECT REGION_ID ,REGION_NAME FROM REGION");
            DataTable dtRegion = dsRegion.Tables[0];
            List<Models.DataModel.ModelRegion> listRegion = new List<Models.DataModel.ModelRegion>();

            foreach (DataRow drArea in dtRegion.Rows)
            {
                var region = new Models.DataModel.ModelRegion()
                {
                    REGION_NAME = drArea["REGION_NAME"].ToString(),
                    REGION_ID = Convert.ToInt32(drArea["REGION_ID"].ToString())
                };


                listRegion.Add(new Models.DataModel.ModelRegion { REGION_NAME = region.REGION_NAME, REGION_ID = region.REGION_ID });
            }

            var dsIndustry = dal.GetDataSet("SELECT DISTINCT PERMIT_NATURAL_GAS FROM GIS_NGR_PL_ME");
            var dtIndustry = dsIndustry.Tables[0];
            var listIndustry = new List<Models.DataModel.ModelIndustryMaster>();

            foreach (DataRow drArea in dtIndustry.Rows)
            {
                var region = new Models.DataModel.ModelIndustryMaster()
                {
                    PERMIT_NATURAL_GAS = drArea["PERMIT_NATURAL_GAS"].ToString()
                };

                listIndustry.Add(new Models.DataModel.ModelIndustryMaster {
                    PERMIT_NATURAL_GAS = region.PERMIT_NATURAL_GAS
                });
            }

            //  ViewBag.seLicense = listLicense;

            var model = new Models.ViewModel.ModelUtilization()
            {
                ListIndustry = listIndustry,
                ListLicense = listLicense,
                ListRegion = listRegion
            };

            return View(model);
        }

        [HttpPost]
        public JsonResult AllGate()
        {
            DAL.DAL dal = new DAL.DAL();
            DataSet ds = dal.GetDataSet("SELECT * FROM VIEW_GATEPIPEMETER_MENU WHERE TYPE = 'GATESTATION' AND REGION IS NOT NULL");
            decimal green = 0;
            decimal red = 0;
            decimal yellow = 0;
            decimal total = 0;
            foreach (DataRow dr in ds.Tables[0].Rows)
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
            dr1["Name"] = "OK";
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
            dr1["Name"] = "OK";
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

            string strCommand;

            strCommand = @"SELECT * FROM VIEW_GATEPIPEMETER_MENU WHERE YEAR > 2018 AND TYPE NOT LIKE 'METERING' AND REGION IN ('" + regionStr + "')"; 
            var listUtilization = dal.ReadData(strCommand, x => new Models.DataModel.ModelGetUtilization(x)).ToList();

            strCommand = @"SELECT * FROM VIEW_OM_CURRENT WHERE REGION IN ('" + regionStr + "')";
            var listOM = dal.ReadData(strCommand, x => new Models.DataModel.ModelGetOM(x)).ToList();

            strCommand = @"SELECT * FROM VIEW_RISK_CURRENT WHERE REGION IN ('" + regionStr + "')";
            var listRisk = dal.ReadData(strCommand, x => new Models.DataModel.ModelGetRisk(x)).ToList();

            dal = null;
            return Json(new
            {
                utilization = listUtilization,
                om = listOM,
                risk = listRisk
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SearchIndustry(string[] industry)
        {
            var dal = new DAL.DAL();
            string industryStr = string.Join("','", industry);

            string strCommand;

            strCommand = @"SELECT * FROM VIEW_GATEPIPEMETER_MENU WHERE YEAR > 2018 AND TYPE NOT LIKE 'METERING' AND NAME IN (SELECT DISTINCT RC_PROJECT FROM GIS_NGR_PL_ME WHERE PERMIT_NATURAL_GAS IN ('" + industryStr + "'))";
            var listUtilization = dal.ReadData(strCommand, x => new Models.DataModel.ModelGetUtilization(x)).ToList();

            strCommand = @"SELECT * FROM VIEW_OM_CURRENT WHERE RC_NAME IN (SELECT DISTINCT RC_PROJECT FROM GIS_NGR_PL_ME WHERE PERMIT_NATURAL_GAS IN ('" + industryStr + "'))";
            var listOM = dal.ReadData(strCommand, x => new Models.DataModel.ModelGetOM(x)).ToList();

            strCommand = @"SELECT * FROM VIEW_RISK_CURRENT WHERE RC_NAME IN (SELECT DISTINCT RC_PROJECT FROM GIS_NGR_PL_ME WHERE PERMIT_NATURAL_GAS IN ('" + industryStr + "'))";
            var listRisk = dal.ReadData(strCommand, x => new Models.DataModel.ModelGetRisk(x)).ToList();

            dal = null;
            return Json(new
            {
                utilization = listUtilization,
                om = listOM,
                risk = listRisk
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SearchRegionAll()
        {
            var dal = new DAL.DAL();
            var searchregion = @"select * from VIEW_GATEPIPEMETER_MENU WHERE TYPE NOT LIKE 'METERING' AND REGION IS NOT NULL";
            var ds = dal.GetDataSet(searchregion);

            var listUtilization = new List<Models.DataModel.ModelGetUtilization>();
            if (ds.Tables[0].Rows.Count > 0)
            {


                foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                {
                    var reg = new Models.DataModel.ModelGetUtilization()
                    {
                        //CUST_NAME = dr["CUST_NAME"].ToString(),
                        NO = Convert.ToInt32(dr["NO"].ToString()),
                        NAME = dr["NAME"].ToString(),
                        VALUE = Convert.ToDecimal(dr["VALUE"].ToString()),
                        TYPE = dr["TYPE"].ToString(),
                        FLAG = Convert.ToInt32(dr["FLAG"].ToString()),
                        REGION = Convert.ToInt32(dr["REGION"].ToString()),
                        LICENSE = Convert.ToInt32(dr["LICENSE"].ToString()),
                        MONTH = Convert.ToInt32(dr["MONTH"].ToString()),
                        YEAR = Convert.ToInt32(dr["YEAR"].ToString()),
                        THRESHOLD = dr["THRESHOLD"].ToString(),
                    };
                    listUtilization.Add(reg);
                }
            }

            return Json(new
            {
                utilization = listUtilization
            }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SearchLicense(int[] license)
        {
            var dal = new DAL.DAL();
            string licenseStr = string.Join("','", license);

            string strCommand;

            strCommand = @"SELECT * from VIEW_GATEPIPEMETER_MENU WHERE LICENSE IN ('" + licenseStr + "') AND TYPE NOT LIKE 'METERING'";
            var listUtilization = dal.ReadData(strCommand, x => new Models.DataModel.ModelGetUtilization(x)).ToList();

            strCommand = @"SELECT * FROM VIEW_OM_CURRENT WHERE LICENSE_NO IN ('" + licenseStr + "')";
            var listOM = dal.ReadData(strCommand, x => new Models.DataModel.ModelGetRisk(x)).ToList();

            strCommand = @"SELECT * FROM VIEW_RISK_CURRENT WHERE LICENSE_NO IN ('" + licenseStr + "')";
            var listRisk = dal.ReadData(strCommand, x => new Models.DataModel.ModelGetRisk(x)).ToList();

            dal = null;
            return Json(new
            {
                utilization = listUtilization,
                om = listOM,
                risk = listRisk
            }, JsonRequestBehavior.AllowGet);
        }

    }
}
