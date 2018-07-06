using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using OUR_App.Models;
//using OfficeOpenXml;
using System.IO;
using OfficeOpenXml;
using PTT_NGROUR.ExtentionAndLib;
using PTT_NGROUR.DAL;

using PTT_NGROUR.DTO;

using PTT_NGROUR.Models.DataModel;
using PTT_NGROUR.Models.ViewModel;
using PTT_NGROUR.Models;
using System.Data;
using Rotativa;

namespace PTT_NGROUR.Controllers
{
    public class UtilizationController : Controller
    {
        //
        // GET: /Utilization/

        #region - Col PipeLine -

        private const int _intColPipelineName = 1;
        private const int _intColPipelineFlow = 2;
        private const int _intColPipelineDiameter = 3;
        private const int _intColPipelineLength = 4;
        private const int _intColPipelineEfficiency = 5;
        private const int _intColPipelineRoughness = 6;
        private const int _intColPipelineLoad = 7;
        private const int _intColPipelineResultDownstreamVelocity = 8;
        private const int _intColPipelineOutsideDiameter = 9;
        private const int _intColPipelineWallThickness = 10;
        private const int _intColPipelineServiceState = 11;

        #endregion

        #region - Col GateStation -

        private const int _intColGateName = 1;
        private const int _intColGatePressure = 2;
        private const int _intColGateFlow = 3;
        private const int _intColGateDescription = 4;

        #endregion

        public ActionResult ThresholdSetting()
        {
            var dto = new DTO.DtoUtilization();
            var listThreshold = dto.GetListThreshold()
                .OrderByDescending(x => x.ThresholdId)
                .ThenByDescending(x=> x.ThresholdType).ToArray();
            var modelResult = new Models.ViewModel.ModelThreshold()
            {
                ThresholdItems = listThreshold
            };
            return View(modelResult);
        }

        [HttpPost]
        public ActionResult SaveThresholdSetting(ModelThresholdItem[] pListThreshold)
        {
            var result = new ModelJsonResult<string>();
            try
            {
                if (pListThreshold == null || !pListThreshold.Any())
                {
                    result.SetError("Input Is Null Or Empty");
                    return Json(result);
                }
                var dto = new DTO.DtoUtilization();
                foreach (var th in pListThreshold.Where(x => x != null))
                {
                    th.UPDATED_BY = User.Identity.Name;
                    dto.UpdateThreshold(th);
                }
                dto = null;
                result.SetResultValue("Update Complete");

            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }
            return Json(result);
        }

        [PTT_NGROUR.Controllers.AuthorizeController.CustomAuthorize]
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
        public ActionResult Report()
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



        public ActionResult ReportPdf()
        {

            
            var dal = new DAL.DAL();
            var searchregion = @"select * from VIEW_GATEPIPEMETER_MENU WHERE TYPE NOT LIKE 'METERING' AND REGION IS NOT NULL";
            var ds = dal.GetDataSet(searchregion);


            var listRegion = dal.ReadData(searchregion, x => new ModelGetU(x)).ToList(); //new List<Models.DataModel.ModelGetU>();
            //if (ds.Tables[0].Rows.Count > 0)
            //{


            //    foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
            //    {
            //        var reg = new Models.DataModel.ModelGetU()
            //        {
            //            NO = dr["NO"].ToString(),
            //            NAME = dr["NAME"].ToString(),
            //            //COLOR = dr["COLOR"].ToString(),
            //            VALUE = dr["VALUE"].ToString(),
            //            TYPE = dr["TYPE"].ToString(),
            //            FLAG = dr["FLAG"].ToString(),
            //            REGION = dr["REGION"].ToString(),
            //            LICENSE = dr["LICENSE"].ToString(),
            //            STATUS = dr["STATUS"].ToString(),
            //            MONTH = dr["MONTH"].ToString(),
            //            YEAR = dr["YEAR"].ToString(),
            //        };
            //        listRegion.Add(reg);
            //    }
            //}

            //ds.Tables[0].Clear();
            //ds.Tables[0].Dispose();
            //ds.Clear();
            //ds.Dispose();
            //ds = null;
            dal = null;
            return new ViewAsPdf(listRegion);
            //return View(listRegion);
        }

        public ActionResult PrintViewToPdf()
        {
            var report = new ActionAsPdf("Report");
            return report;
        }

        public ActionResult Customer()
        {
            var dal = new DAL.DAL();
            var i = 0;
            var ds = dal.GetDataSet("SELECT * FROM VIEW_SHIPTO_SOLDTO");
            var dt = ds.Tables[0];
            var listCus = new List<Models.DataModel.ModelVIEW_SHIPTO_SOLDTO>();
            foreach (System.Data.DataRow dr in dt.Rows)
            {    i = i + 1;
                var cus = new Models.DataModel.ModelVIEW_SHIPTO_SOLDTO()
                {   count = i,
                    SHIP_TO_NAME = dr["SHIP_TO_NAME"].ToString(),
                    SHIP_TO_SNAME = dr["SHIP_TO_SNAME"].ToString(),
                    SHIP_TO = dr["SHIP_TO"].ToString(),
                    SOLD_TO = dr["SOLD_TO"].ToString(),
                    SOLD_TO_FLAG_SHAPE = Convert.ToInt32(dr["SOLD_TO_FLAG_SHAPE"].ToString()),
                    SHIP_TO_FLAG_SHAPE = Convert.ToInt32(dr["SHIP_TO_FLAG_SHAPE"].ToString())
                };
                
                listCus.Add(cus);
            }

            var dscus = dal.GetDataSet("SELECT * FROM NGR_CUSTOMER C LEFT JOIN NGR_CUSTOMER_OFFICE CO ON C.SOLD_TO = CO.SOLD_TO");
            var dtcus = dscus.Tables[0];
            var listCusAll = new List<Models.DataModel.ModelNGR_CUST_ALL>();
            foreach (System.Data.DataRow drcus in dtcus.Rows)
            {
                var cusAll = new Models.DataModel.ModelNGR_CUST_ALL()
                {
                    CUST_NAME = drcus["CUST_NAME"].ToString(),
                    CUST_SNAME = drcus["CUST_SNAME"].ToString(),
                    SHIP_TO = drcus["SHIP_TO"].ToString(),
                    CUST_NAME_EN = drcus["CUST_NAME_EN"].ToString(),
                    SHIP_TO_ADDRESS = drcus["SHIP_TO_ADDRESS"].ToString(),
                    PLANT_NAME = drcus["PLANT_NAME"].ToString(),
                    SALES_DISTRICT = drcus["SALES_DISTRICT"].ToString(),
                    CONTRACT_TYPE = drcus["CONTRACT_TYPE"].ToString(),
                    IE_NAME = drcus["IE_NAME"].ToString(),
                    REGION = drcus["REGION"].ToString(),
                    UPDATED_BY = drcus["UPDATED_BY"].ToString(),
                    UPDATED_DATE = drcus["UPDATED_DATE"].GetDate(),

                    OBJECTID = drcus["OBJECTID"].ToString(),
                    SOLD_TO = drcus["SOLD_TO"].ToString(),
                    FLAG_SHAPE = Convert.ToInt32(drcus["FLAG_SHAPE"].ToString()),
                    SOLD_TO_NAME = drcus["SOLD_TO_NAME"].ToString(),
                    SOLD_TO_ADDRESS = drcus["SOLD_TO_ADDRESS"].ToString(),
                    STATUS = drcus["STATUS"].ToString()
                };
                listCusAll.Add(cusAll);
            }
            var j = 0;
            var dsmeter = dal.GetDataSet("SELECT * FROM VIEW_METER");
            var dtmeter = dsmeter.Tables[0];
            //public List<DataModel.ModelVIEW_METER> ListViewMeter { get; set; }
            var listMeterV = new List<Models.DataModel.ModelVIEW_METER>();
            foreach (System.Data.DataRow drmeter in dtmeter.Rows)
            {
                j = j + 1;
                var meter = new Models.DataModel.ModelVIEW_METER()
                {
                    countMeter = j,
                    CUST_NAME = drmeter["CUST_NAME"].ToString(),
                    LICENSE_CODE = drmeter["LICENSE_CODE"].GetInt(),
                
                    METER_NAME = drmeter["METER_NAME"].ToString(),
                    METER_NUMBER = drmeter["METER_NUMBER"].ToString(),
                    METER_TYPE = drmeter["METER_TYPE"].ToString(),
                    REGION = drmeter["REGION"].ToString(),
                    SHIP_TO = drmeter["SHIP_TO"].ToString(),
                    SOLD_TO = drmeter["SOLD_TO"].ToString(),
                    SOLD_TO_NAME = drmeter["SOLD_TO_NAME"].ToString(),
                    STATUS = drmeter["STATUS"].ToString()
                };

                listMeterV.Add(meter);
            }

            var dsmeterT = dal.GetDataSet("SELECT A.METER_NUMBER,A.METER_NAME,A.METER_TYPE,STATUS_DETAIL,A.LICENSE_CODE,B.SHIP_TO,B.CUST_NAME,C.SOLD_TO,C.SOLD_TO_NAME,A.STATUS,A.COMMDATE,A.OBJECT_ID, CASE WHEN (SELECT REGION_ID FROM REGION WHERE REGION_NAME = B.REGION) IS NOT NULL THEN (SELECT REGION_ID FROM REGION WHERE REGION_NAME = B.REGION) WHEN (SELECT REGION_ID FROM REGION WHERE REGION_NAME_TH = B.REGION) IS NOT NULL THEN (SELECT REGION_ID FROM REGION WHERE REGION_NAME_TH = B.REGION) WHEN (SELECT REGION_ID FROM REGION WHERE REGION_NAME_EN = B.REGION) IS NOT NULL THEN (SELECT REGION_ID FROM REGION WHERE REGION_NAME_EN = B.REGION) ELSE B.REGION END REGION FROM NGR_CUSTOMER_METER A, NGR_CUSTOMER B, NGR_CUSTOMER_OFFICE C, STATUS D WHERE A.SHIP_TO = B.SHIP_TO AND B.SOLD_TO = C.SOLD_TO AND A.STATUS = D.STATUS_ID(+)");
            var dtmeterT = dsmeterT.Tables[0];
            //public List<DataModel.ModelVIEW_METER> ListViewMeter { get; set; }
            var listMeterT = new List<Models.DataModel.ModelMETER>();
            foreach (System.Data.DataRow drmeterT in dtmeterT.Rows)
            {
                
                var meterT = new Models.DataModel.ModelMETER()
                {
                    OBJECT_ID_T = Convert.ToInt32(drmeterT["OBJECT_ID"].ToString()),
                    CUST_NAME_T = drmeterT["CUST_NAME"].ToString(),
                    LICENSE_CODE_T = drmeterT["LICENSE_CODE"].ToString(),
                    METER_NAME_T = drmeterT["METER_NAME"].ToString(),
                    METER_NUMBER_T = drmeterT["METER_NUMBER"].ToString(),
                    METER_TYPE_T = drmeterT["METER_TYPE"].ToString(),
                    REGION_T = Convert.ToInt32(drmeterT["REGION"].ToString()),
                    SHIP_TO_T = drmeterT["SHIP_TO"].ToString(),
                    SOLD_TO_T = drmeterT["SOLD_TO"].ToString(),
                    SOLD_TO_NAME_T = drmeterT["SOLD_TO_NAME"].ToString(),
                    STATUS_T = drmeterT["STATUS"].ToString(),
                    COMMDATE_T = drmeterT["COMMDATE"].GetDate(),
                };

                listMeterT.Add(meterT);
            }
  
            var model = new Models.ViewModel.Customer()
            {
                ListViewShipToSoldTo = listCus,
                ListCustAll = listCusAll,
                ListViewMeter = listMeterV,
                ListMeter = listMeterT
               
            };

            return View(model);
        }


        public ActionResult MenuUtilization()
        { return View(); }

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
                        ID = dr["ID"].ToString(),
                        NAME = dr["NAME"].ToString(),
                        //COLOR = dr["COLOR"].ToString(),
                        THRESHOLD = dr["THRESHOLD"].ToString(),
                        OBJ_TYPE = dr["OBJ_TYPE"].ToString(),
                        VALUE = dr["VALUE"].ToString(),
                        TYPE = dr["TYPE"].ToString(),
                        FLAG = dr["FLAG"].ToString(),
                        REGION = dr["REGION"].ToString(),
                        LICENSE = Convert.ToInt32(dr["LICENSE"].ToString()),
                        MONTH = dr["MONTH"].ToString(),
                        YEAR = dr["YEAR"].ToString(),
                        STATUS = dr["STATUS"].ToString(),
                        
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
                        ID = dr["ID"].ToString(),
                        NAME = dr["NAME"].ToString(),
                        //COLOR = dr["COLOR"].ToString(),
                        THRESHOLD = dr["THRESHOLD"].ToString(),
                        OBJ_TYPE = dr["OBJ_TYPE"].ToString(),
                        VALUE = dr["VALUE"].ToString(),
                        TYPE = dr["TYPE"].ToString(),
                        FLAG = dr["FLAG"].ToString(),
                        REGION = dr["REGION"].ToString(),
                        LICENSE = Convert.ToInt32(dr["LICENSE"].ToString()),
                        MONTH = dr["MONTH"].ToString(),
                        YEAR = dr["YEAR"].ToString(),
                        STATUS = dr["STATUS"].ToString(),
                    };
                    listLicense.Add(reg);
                }
            }

            return Json(listLicense, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ImportExcel()
        {
            return View();
        }
        public JsonResult UploadFile()
        {
            if (Request.Files.Count > 0)
            {
                try
                {
                    object[,] obj = null;
                    int noOfCol = 0;
                    int noOfRow = 0;
                    HttpFileCollectionBase file = Request.Files;
                    if ((file != null) && (file.Count > 0))
                    {
                        //string fileName = file.FileName;
                        //string fileContentType = file.ContentType;
                        byte[] fileBytes = new byte[Request.ContentLength];
                        var data = Request.InputStream.Read(fileBytes, 0, Convert.ToInt32(Request.ContentLength));
                        // var usersList = new List<Users>();
                        //using (var package = new ExcelPackage())
                        using (var package = new ExcelPackage(Request.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.First();
                            noOfCol = workSheet.Dimension.End.Column;
                            noOfRow = workSheet.Dimension.End.Row;
                            obj = new object[noOfRow, noOfCol];
                            obj = (object[,])workSheet.Cells.Value;
                        }
                    }
                    return Json(new { data = obj, row = noOfRow, col = noOfCol }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception ex)
                {

                }

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult IsDuplicateExcelData()
        {
            var result = new ModelJsonResult<bool>();
            string inYear = Request["year"];
            string inMonth = Request["month"];
            string inRegion = Request["region"];
            string inType = Request["type"];

            HttpFileCollectionBase files = Request.Files;


            if (files == null || 0.Equals(files.Count))
            {
                result.SetError("No File Data To Load");
                return Json(result);
            }

            var fb = files[0];

            if (fb == null || fb.InputStream == null)
            {
                result.SetError("No File Data To Load");
                return Json(result);
            }
            string exttension = System.IO.Path.GetExtension(fb.FileName);
            if (!(new string[] { ".xls", ".xlsx" }).Contains(exttension))
            {
                result.SetError("File Type Not In xls or xlsx");
                return Json(result);
            }
            var dto = new DTO.DtoUtilization();
            bool isDuplicate = false;
            switch (inType)
            {
                case "pipeline":
                    var listPipeLine = dto.ReadExcelPipelineImport(
                        pStreamExcel: fb.InputStream,
                        pIntMonth: inMonth.GetInt(),
                        pStrRegionId: inRegion,
                        pStrUploadBy: User.Identity.Name,
                        pIntYear: inYear.GetInt()
                    );
                    isDuplicate = dto.GetListPipelineImportDuplicate(listPipeLine).Any();
                    
                    break;
                case "gate":
                    var listGate = dto.ReadExcelGateStationImport( 
                        pStreamExcel: fb.InputStream,  
                        pIntMonth: inMonth.GetInt(), 
                        pStrRegionId : inRegion, 
                        pStrUploadBy: User.Identity.Name, 
                        pIntYear: inYear.GetInt()
                    );
                    isDuplicate = dto.GetListGateImportDuplicate(listGate).Any();
                    
                    break;
            }
            result.SetResultValue(isDuplicate);

            return Json(result);
        }

        [HttpPost]
        public JsonResult InsertExceldata()
        {
            var result = new ModelJsonResult<ModelInsertExcelData>();
            var modelInsertExcel = new ModelInsertExcelData();
            try
            {

                string inYear = Request["year"];
                string inMonth = Request["month"];
                string inRegion = Request["region"];
                string inType = Request["type"];

                HttpFileCollectionBase files = Request.Files;


                if (files == null || 0.Equals(files.Count))
                {
                    result.SetError("No File Data To Load");
                    return Json(result);
                }

                var fb = files[0];

                if (fb == null || fb.InputStream == null)
                {
                    result.SetError("No File Data To Load");
                    return Json(result);
                }
                string exttension = System.IO.Path.GetExtension(fb.FileName);
                if (!(new string[] { ".xls", ".xlsx" }).Contains(exttension))
                {
                    result.SetError("File Type Not In xls or xlsx");
                    return Json(result);
                }
                switch (inType)
                {
                    case "pipeline":
                        insertExcelPipelineData(
                           pFileStream: fb.InputStream,
                           pIntMonth: inMonth.GetInt(),
                           pStrRegionId: inRegion,
                           pStrUploadBy: User.Identity.Name,
                           pIntYear: inYear.GetInt(),
                           pModelResult: modelInsertExcel
                        );
                        result.SetResultValue(modelInsertExcel);
                        break;                        
                    case "gate":
                        insertExcelGateData(
                            pFileStream: fb.InputStream,
                            pIntMonth: inMonth.GetInt(),
                            pStrRegionId: inRegion,
                            pStrUploadBy: User.Identity.Name,
                            pIntYear: inYear.GetInt(),
                            pModelResult: modelInsertExcel
                        );
                        result.SetResultValue(modelInsertExcel);
                        break;
                    default:
                        result.SetError("Incomplete Unknow Type : " + inType);
                        break;
                }
                return Json(result);
                
            }
            catch (Exception ex)
            {
                result.SetException(ex);
                return Json(result);
            }
        }

        private void insertExcelPipelineData(
        Stream pFileStream,
        int pIntMonth,
        string pStrRegionId,
        string pStrUploadBy,
        int pIntYear,
        ModelInsertExcelData pModelResult)
        {
            var dto = new DtoUtilization();
            var listExcelPipeline = dto.ReadExcelPipelineImport(
                 pStreamExcel: pFileStream,
                 pIntMonth: pIntMonth,
                 pIntYear: pIntYear,
                 pStrUploadBy: pStrUploadBy,
                 pStrRegionId: pStrRegionId).Where(x => x != null).ToList();

            //select distinct RC_Project from GIS_NGR_PL
            var listRc = dto.GetListRcProject().Where(x => !string.IsNullOrEmpty(x)).ToList();

           
            var listPipelineDuplicate = dto.GetListPipelineImportDuplicate(listExcelPipeline)
            .Where(x => x != null).ToList();

            var dicPipeline = new Dictionary<string, List<ModelPipelineImport>>();
            var listError = new List<ModelPipelineImport>();
            for (int i = listExcelPipeline.Count - 1; i >= 0; --i)
            {
                var pipeLine = listExcelPipeline[i];
                var pipeLineDuplicate = listPipelineDuplicate.FirstOrDefault(x => x.RC_NAME == pipeLine.RC_NAME);
                // Insert Pipeline
                if (pipeLineDuplicate == null)
                {
                    dto.InsertPipelineImport(pipeLine);
                }
                else
                {
                    // If Exists Update Pipeline
                    pipeLine.PIPELINE_ID = pipeLineDuplicate.PIPELINE_ID;
                    dto.UpdatePipelineImport(pipeLine);
                }

                // Group List Pipeline To Dictionary By RcName
                string strRcName = pipeLine.RC_NAME.Split('-')[0];
                if (listRc.Contains(strRcName))
                {
                    if (dicPipeline.ContainsKey(strRcName))
                    {
                        dicPipeline[strRcName].Add(pipeLine.Clone());
                    }
                    else
                    {
                        var listPl = new List<ModelPipelineImport>() { pipeLine.Clone() };
                        dicPipeline.Add(strRcName, listPl);
                    }
                }
                else
                {
                    listExcelPipeline.RemoveAt(i);
                    listError.Add(pipeLine);
                }
            }
            pModelResult.ListSuccessPipeLine = listExcelPipeline.ToArray();
            pModelResult.ListUnSuccessPipeLine = listError.ToArray();

            //convert Dictionary To List PipelineArchive
            var listPLA = new List<ModelPipelineArchive>();
           
            foreach(var dic in dicPipeline)
            {
                var decMax = dic.Value.Max(x => x.VELOCITY);
                var pl = dic.Value.Where(x => x.VELOCITY == decMax).First();
                pl.RC_NAME = dic.Key;
                listPLA.Add(new ModelPipelineArchive(pl));
                dic.Value.Clear();                
            }

            var listPlAD = dto.GetListPipelineArchiveDuplicate(listPLA).Where(x=> x!= null).ToList();

            foreach(var pla in listPLA)
            {
                var plad = listPlAD.Where(x => x.RC_NAME == pla.RC_NAME).FirstOrDefault();
                if(plad == null)
                {
                    // Insert PipelineArchive
                    dto.InsertPipelineArchive(pla);
                }
                else
                {
                    // If Exists Update PipelineArchive
                    pla.PIPELINE_ID = plad.PIPELINE_ID;
                    dto.UpdatePipelineArchive(pla);
                }
            }

            dto = null;
            listError.Clear();
            listError = null;
            listExcelPipeline.Clear();
            listExcelPipeline = null;
            listPipelineDuplicate.Clear();
            listPipelineDuplicate = null;
            listPLA.Clear();
            listPLA = null;
            listPlAD.Clear();
            listPlAD = null;
            listRc.Clear();
            listRc = null;
            dicPipeline.Clear();
            dicPipeline = null;
            GC.Collect();                        
        }

        private void insertExcelPipelineData2(
        Stream pFileStream,
        int pIntMonth,
        string pStrRegionId,
        string pStrUploadBy,
        int pIntYear,
        ModelInsertExcelData pModelResult)
        {
            var dto = new DtoUtilization();
            var listExcelPipeline = dto.ReadExcelPipelineImport(
                 pStreamExcel: pFileStream,
                 pIntMonth: pIntMonth,
                 pIntYear: pIntYear,
                 pStrUploadBy: pStrUploadBy,
                 pStrRegionId: pStrRegionId).Where(x => x != null).ToList();
            var listRc = dto.GetListRcProject().Where(x => !string.IsNullOrEmpty(x)).ToList();

            var listPipelineAll = dto.GetListPipeline().ToList();

            var listPipelineDuplicate = dto.GetListPipelineImportDuplicate(listExcelPipeline)
            .Where(x => x != null).ToList();

            for (int i = listExcelPipeline.Count - 1; i >= 0; --i)
            {
                var pipeLine = listExcelPipeline[i];
                var pipeLineDuplicate = listPipelineDuplicate.FirstOrDefault(x => x.RC_NAME == pipeLine.RC_NAME);
                if (pipeLineDuplicate == null)
                {
                    dto.InsertPipelineImport(pipeLine);
                }
                else
                {
                    pipeLine.PIPELINE_ID = pipeLineDuplicate.PIPELINE_ID;
                    dto.UpdatePipelineImport(pipeLine);
                }
            }


            //var listPipelineForInsert = listExcelPipeline
            //    .Where(x => listPipelineDuplicate == null || listPipelineDuplicate.Any(y => y.RC_NAME == x.RC_NAME))
            //    .ToList();
            //listPipelineForInsert.ForEach(z => dto.InsertPipelineImport(z));

            //var listPipelineForUpdate = listExcelPipeline
            //    .Where(x => listPipelineDuplicate != null && listPipelineDuplicate.Any(y => y.RC_NAME == x.RC_NAME))
            //    .ToList();



            var listPipelineArchive =
                (from pi in listExcelPipeline
                 group pi by pi.RC_NAME.Split('-')[0] into lpi
                 let maxV = lpi.Max(x => x.VELOCITY)
                 where listRc.Contains(lpi.Key)
                 select new
                 {
                     rcName = lpi.Key,
                     pipeLineImport = lpi.Where(x => x.VELOCITY == maxV).FirstOrDefault()
                 }).Select(x => new ModelPipelineArchive()
                 {
                     MONTH = x.pipeLineImport.MONTH,
                     RC_NAME = x.rcName,
                     REGION = x.pipeLineImport.REGION,
                     UPLOAD_BY = x.pipeLineImport.UPLOAD_BY,
                     VELOCITY = x.pipeLineImport.VELOCITY,
                     YEAR = x.pipeLineImport.YEAR
                 }).ToList();
            var listPipelineArchiveDuplicate = dto.GetListPipelineArchiveDuplicate(listPipelineArchive)
                .Where(x => x != null).ToList();
            for (int i = listPipelineArchive.Count - 1; i >= 0; --i)
            {
                var pla = listPipelineArchive[i];
                var plaDupplicate = listPipelineArchiveDuplicate.FirstOrDefault(x => x.RC_NAME == pla.RC_NAME);
                if (plaDupplicate == null)
                {
                    dto.InsertPipelineArchive(pla);
                }
                else
                {
                    pla.PIPELINE_ID = plaDupplicate.PIPELINE_ID;
                    dto.UpdatePipelineArchive(pla);
                }
            }

            pModelResult.ListSuccessPipeLine = listExcelPipeline.Where(x => listPipelineArchive.Any(y => x.RC_NAME.StartsWith(y.RC_NAME))).ToArray();
            //pModelResult.ListUnSuccessPipeLine = listExcelPipeline.Where(x => !listPipelineArchive.Any(y => x.RC_NAME.StartsWith(y.RC_NAME))).ToArray();
            pModelResult.ListUnSuccessPipeLine = listExcelPipeline.Where(x => pModelResult.ListSuccessPipeLine.Any(y => y.RC_NAME == x.RC_NAME)).ToArray();
        }
        private ModelPipelineImport[] insertExcelPipelineData2(
        Stream pFileStream,
        int pIntMonth,
        int pIntRegionId,
        string pStrUploadBy,
        int pIntYear)
        {
            var dto = new DtoUtilization();
            var listPipeLineImport = dto.ReadExcelPipelineImport(pFileStream, pIntMonth, pIntRegionId.ToString(), pStrUploadBy, pIntYear).ToList();
            var listRcProject = dto.GetListRcProject();
            foreach (ModelPipelineImport modelPI in listPipeLineImport)
            {
                if (modelPI == null)
                {
                    continue;
                }
                dto.InsertPipelineImport(modelPI);
            }
            var listPipelineArchive = (from pi in listPipeLineImport
                                       group pi by pi.RC_NAME.Split('-')[0]
                into lpi
                                       let maxV = lpi.Max(x => x.VELOCITY)
                                       where listRcProject.Contains(lpi.Key)
                                       select new
                                       {
                                           rcName = lpi.Key,
                                           pipeLineImport = lpi.Where(x => x.VELOCITY == maxV).FirstOrDefault()
                                       }).Select(x => new ModelPipelineArchive()
                                       {
                                           MONTH = x.pipeLineImport.MONTH,
                                           RC_NAME = x.rcName,
                                           REGION = x.pipeLineImport.REGION,
                                           UPLOAD_BY = x.pipeLineImport.UPLOAD_BY,
                                           VELOCITY = x.pipeLineImport.VELOCITY,
                                           YEAR = x.pipeLineImport.YEAR
                                       });//.ForEach(x=> dto.InsertPipelineArchive(x));
            foreach (ModelPipelineArchive pa in listPipelineArchive)
            {
                dto.InsertPipelineArchive(pa);
            }
            var result = listPipeLineImport.Where(x => !listRcProject.Contains(x.RC_NAME.Split('-')[0])).ToArray();
            dto = null;
            return result;
        }

        private void insertExcelGateData(
        Stream pFileStream,
        int pIntMonth,
        string pStrRegionId,
        string pStrUploadBy,
        int pIntYear,
        ModelInsertExcelData pModelResult)
        {
            var dto = new DtoUtilization();

            var listGate = dto.ReadExcelGateStationImport(pFileStream, pIntMonth, pStrRegionId, pStrUploadBy, pIntYear).ToList();

            //var listGisGateDataName = dto.GetListGisGateStationName();
            var listGateStation = dto.GetListModelGateStation();
            var listAbbr = listGateStation
                .Select(x => x.ABBREV_NAME)
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct().ToList();
            //var listStationId = dto.GetListStationID().ToList();
            var listGateHaveMasterData = listGate.Where(x => listAbbr.Contains(x.GATE_NAME)).ToList();

            var listGateImportDuplicate = dto.GetListGateImportDuplicate(listGate).ToList();

            var listGateDupplicate = new List<ModelGateStationImport>();

            var listGateUnsuccess = new List<ModelGateStationImport>();

            var listArchiveDuplicate = dto.GetListGateArchiveDuplicate(listGate);

            for (int i = listGate.Count - 1; i >= 0; --i)
            {
                var gateItem = listGate[i];

                var gateDuplicate = listGateImportDuplicate.FirstOrDefault(x => x.GATE_NAME == gateItem.GATE_NAME);
                if (gateDuplicate == null)
                {
                    dto.InsertGateImport(gateItem);


                }
                else if (gateItem.FLOW != gateDuplicate.FLOW && gateItem.PRESSURE != gateDuplicate.PRESSURE)
                {
                    gateItem.GATE_ID = gateDuplicate.GATE_ID;
                    dto.UpdateGateImport(gateItem);
                }

                if (!listAbbr.Contains(gateItem.GATE_NAME))
                {
                    listGate.RemoveAt(i);
                    listGateUnsuccess.Add(gateItem);
                }
                else
                {
                    var gateArchiveDuplicate = listArchiveDuplicate.FirstOrDefault(x => x.GATE_NAME == gateItem.GATE_NAME);

                    var ga2 = new ModelGateStationArchive(gateItem);

                    if (gateArchiveDuplicate == null)
                    {
                        dto.InsertGateArchive(ga2);
                    }
                    else
                    {
                        ga2.GATE_ID = gateArchiveDuplicate.GATE_ID;
                        dto.UpdateGateArchive(ga2);
                    }
                }
            }
            //pModelResult.ListDuplicateGateStation = listGateDupplicate.ToArray();
            pModelResult.ListUnSuccessGateStation = listGateUnsuccess.ToArray();
            pModelResult.ListSuccessGateStation = listGate.ToArray();
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
        public ActionResult EditCustomer(string txtCustNameENEdit, string txtShipToEdit)
        {   //string textEdit = "แก้ไข้อมูลเรียบร้อย";

            var dalEditCus = new DAL.DAL();
            string username = User.Identity.Name;
            string strCommandEditCus = "UPDATE NGR_CUSTOMER SET CUST_NAME_EN ='" + txtCustNameENEdit + "',UPDATED_DATE=sysdate,UPDATED_BY='" + username + "' WHERE SHIP_TO='" + txtShipToEdit + "'";
            var con = dalEditCus.GetConnection();
            con.Open();
            dalEditCus.GetCommand(strCommandEditCus, con).ExecuteNonQuery();
            con.Close();
            con.Dispose();

            //else { textEdit = "โปรดกรอกข้อมูลให้ครบถ้วน"; }
            //ViewBag.textAlert = textEdit;
            //TempData["message"] = textEdit;
            return Redirect("Customer");
        }
             [HttpPost]
             public ActionResult EditMeter(string datepicCommdateMEdit, string txtMeterNameMEdit, string txtMeterNumMEdit, string txtMeterTypeMEdit, int seStatusMEdit, int txtObjectIDMEdit)
             {   //string textEdit = "แก้ไข้อมูลเรียบร้อย";
                 //String[] date1 = date.split("/");
                 //var mainDate = DateTime.ParseExact(datepicCommdateMEdit, "dd/MM/yyyy HH:mm:ss", null);
                 var dalEditMeter = new DAL.DAL();
                 string username = User.Identity.Name;
                 string strCommandEditMeter = "UPDATE NGR_CUSTOMER_METER SET METER_NUMBER = '" + txtMeterNumMEdit + "',METER_NAME ='" + txtMeterNameMEdit + "',STATUS='" + seStatusMEdit + "',COMMDATE=to_timestamp( '" + datepicCommdateMEdit + "', 'dd/mm/yyyy' ),UPDATED_DATE=Sysdate,UPDATED_BY='" + username + "' WHERE OBJECT_ID='" + txtObjectIDMEdit + "'";
                 var conMeter = dalEditMeter.GetConnection();
                 conMeter.Open();
                 dalEditMeter.GetCommand(strCommandEditMeter, conMeter).ExecuteNonQuery();
                 conMeter.Close();
                 conMeter.Dispose();

                 //else { textEdit = "โปรดกรอกข้อมูลให้ครบถ้วน"; }
                 //ViewBag.textAlert = textEdit;
                 //TempData["message"] = textEdit;
                 return Redirect("Customer");
             }

             [HttpPost]
             public JsonResult RegionAllReport()
             {
                 var dal = new DAL.DAL();
                // var searchregion = @"select * from VIEW_GATE_PIPE_REPORT_CURRENT V LEFT JOIN REGION R ON R.REGION_ID = V.REGION LEFT JOIN LICENSE_MASTER L ON to_number(V.LICENSE_NO) = L.LICENSE_ID WHERE REGION IS NOT NULL";
                 var searchregion = @"select * from VIEW_GATE_PIPE_REPORT_CURRENT WHERE REGION IS NOT NULL";
                 var ds = dal.GetDataSet(searchregion);


                 var listRegion = new List<Models.DataModel.ModelGetU>();
                 if (ds.Tables[0].Rows.Count > 0)
                 {
                     foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                     {
                         var reg = new Models.DataModel.ModelGetU()
                         {
                             NAME = dr["NAME"].ToString(),
                             REGION = dr["REGION"].ToString(),
                             LICENSE = dr["LICENSE_NO"].GetInt(),
                             VALUE = dr["VALUE"].ToString(),
                             //COLOR = dr["COLOR"].ToString(),
                             MONTH = dr["MONTH"].ToString(),
                             YEAR = dr["YEAR"].ToString(),
                             TYPE = dr["TYPE"].ToString(),
                             THRESHOLD = dr["THRESHOLD"].ToString()
                         };
                         listRegion.Add(reg);
                     }
                 }

                 return Json(listRegion, JsonRequestBehavior.AllowGet);
             }

             [HttpPost]
             public JsonResult SearchRegionReport(string threshold,string type,string month,string year,int[] Multidata)
             {
                 var searchregion = "";
                 var dal = new DAL.DAL();
                 string regionStr = string.Join("','", Multidata);
                 if (month != "null" && year != "null" && threshold == "All" && type == "All")
                 { searchregion = @"select * from VIEW_GATE_PIPE_REPORT WHERE REGION IN ('" + regionStr + "') AND MONTH IN ('" + month + "') AND YEAR IN ('" + year + "')"; }
                 else if (month != "null" && year != "null" && threshold != "All" && type == "All")
                 { searchregion = @"select * from VIEW_GATE_PIPE_REPORT WHERE REGION IN ('" + regionStr + "') AND MONTH IN ('" + month + "') AND YEAR IN ('" + year + "') AND THRESHOLD IN ('" + threshold + "')"; }
                 else if (month != "null" && year != "null" && threshold == "All" && type != "All")
                 { searchregion = @"select * from VIEW_GATE_PIPE_REPORT WHERE REGION IN ('" + regionStr + "') AND MONTH IN ('" + month + "') AND YEAR IN ('" + year + "') AND TYPE IN ('" + type + "') "; }
                 else if (month != "null" && year != "null" && threshold != "All" && type != "All")
                 { searchregion = @"select * from VIEW_GATE_PIPE_REPORT WHERE REGION IN ('" + regionStr + "') AND MONTH IN ('" + month + "') AND YEAR IN ('" + year + "') AND TYPE IN ('" + type + "') AND THRESHOLD IN ('" + threshold + "')"; }

                 else if (month == "null" && year != "null" && threshold == "All" && type == "All")
                 { searchregion = @"select * from VIEW_GATE_PIPE_REPORT WHERE REGION IN ('" + regionStr + "') AND YEAR IN ('" + year + "') "; }
                 else if (month == "null" && year != "null" && threshold != "All" && type == "All")
                 { searchregion = @"select * from VIEW_GATE_PIPE_REPORT WHERE REGION IN ('" + regionStr + "') AND YEAR IN ('" + year + "') AND THRESHOLD IN ('" + threshold + "')"; }
                 else if (month == "null" && year != "null" && threshold == "All" && type != "All")
                 { searchregion = @"select * from VIEW_GATE_PIPE_REPORT WHERE REGION IN ('" + regionStr + "') AND YEAR IN ('" + year + "') AND TYPE IN ('" + type + "') "; }
                 else if (month == "null" && year != "null" && threshold != "All" && type != "All")
                 { searchregion = @"select * from VIEW_GATE_PIPE_REPORT WHERE REGION IN ('" + regionStr + "') AND YEAR IN ('" + year + "') AND TYPE IN ('" + type + "') AND THRESHOLD IN ('" + threshold + "')"; }

                 else if (month == "null" && year == "null" && threshold == "All" && type == "All")
                 { searchregion = @"select * from VIEW_GATE_PIPE_REPORT WHERE REGION IN ('" + regionStr + "')"; }
                 else if (month == "null" && year == "null" && threshold != "All" && type == "All")
                 { searchregion = @"select * from VIEW_GATE_PIPE_REPORT WHERE REGION IN ('" + regionStr + "') AND THRESHOLD IN ('" + threshold + "')"; }
                 else if (month == "null" && year == "null" && threshold == "All" && type != "All")
                 { searchregion = @"select * from VIEW_GATE_PIPE_REPORT WHERE REGION IN ('" + regionStr + "')  AND TYPE IN ('" + type + "')"; }
                 else if (month == "null" && year == "null" && threshold == "All" && type != "All")
                 { searchregion = @"select * from VIEW_GATE_PIPE_REPORT WHERE REGION IN ('" + regionStr + "')  AND TYPE IN ('" + type + "') AND THRESHOLD IN ('" + threshold + "')"; }
                 var ds = dal.GetDataSet(searchregion);


                 var listRegion = new List<Models.DataModel.ModelGetU>();
                 if (ds.Tables[0].Rows.Count > 0)
                 {


                     foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                     {
                         var reg = new Models.DataModel.ModelGetU()
                         {

                             NAME = dr["NAME"].ToString(),
                             REGION = dr["REGION"].ToString(),
                             LICENSE = dr["LICENSE_NO"].GetInt(),
                             VALUE = dr["VALUE"].ToString(),
                             //COLOR = dr["COLOR"].ToString(),
                             MONTH = dr["MONTH"].ToString(),
                             YEAR = dr["YEAR"].ToString(),
                             TYPE = dr["TYPE"].ToString(),
                             THRESHOLD = dr["THRESHOLD"].ToString()
                         };
                         listRegion.Add(reg);
                     }
                 }

                 return Json(listRegion, JsonRequestBehavior.AllowGet);
             }

             [HttpPost]
             public JsonResult SearchLicenseReport(string threshold,string type,string month, string year, int[] Multidata)
             {
                 var searchlicense="";
                 var dal = new DAL.DAL();
                 string licenseStr = string.Join("','", Multidata);
                 if (month != "null" && year != "null" && threshold == "All" && type == "All")
                 { searchlicense = @"select * from VIEW_GATE_PIPE_REPORT WHERE LICENSE_NO IN ('" + licenseStr + "') AND MONTH IN ('" + month + "') AND YEAR IN ('" + year + "')"; }
                 else if (month != "null" && year != "null" && threshold != "All" && type == "All")
                 { searchlicense = @"select * from VIEW_GATE_PIPE_REPORT WHERE LICENSE_NO IN ('" + licenseStr + "') AND MONTH IN ('" + month + "') AND YEAR IN ('" + year + "') AND THRESHOLD IN ('" + threshold + "')"; }
                 else if (month != "null" && year != "null" && threshold == "All" && type != "All")
                 { searchlicense = @"select * from VIEW_GATE_PIPE_REPORT WHERE LICENSE_NO IN ('" + licenseStr + "') AND MONTH IN ('" + month + "') AND YEAR IN ('" + year + "') AND TYPE IN ('" + type + "')"; }
                 else if (month != "null" && year != "null" && threshold != "All" && type != "All")
                 { searchlicense = @"select * from VIEW_GATE_PIPE_REPORT WHERE LICENSE_NO IN ('" + licenseStr + "') AND MONTH IN ('" + month + "') AND YEAR IN ('" + year + "') AND TYPE IN ('" + type + "') AND THRESHOLD IN ('" + threshold + "')"; }

                 else if (month == "null" && year != "null" && threshold == "All" && type == "All")
                 { searchlicense = @"select * from VIEW_GATE_PIPE_REPORT WHERE LICENSE_NO IN ('" + licenseStr + "') AND YEAR IN ('" + year + "') "; }
                 else if (month == "null" && year != "null" && threshold != "All" && type == "All")
                 { searchlicense = @"select * from VIEW_GATE_PIPE_REPORT WHERE LICENSE_NO IN ('" + licenseStr + "') AND YEAR IN ('" + year + "') AND THRESHOLD IN ('" + threshold + "')"; }
                 else if (month == "null" && year != "null" && threshold == "All" && type != "All")
                 { searchlicense = @"select * from VIEW_GATE_PIPE_REPORT WHERE LICENSE_NO IN ('" + licenseStr + "') AND YEAR IN ('" + year + "') AND TYPE IN ('" + type + "')"; }
                 else if (month == "null" && year != "null" && threshold != "All" && type != "All")
                 { searchlicense = @"select * from VIEW_GATE_PIPE_REPORT WHERE LICENSE_NO IN ('" + licenseStr + "') AND YEAR IN ('" + year + "') AND TYPE IN ('" + type + "') AND THRESHOLD IN ('" + threshold + "')"; }

                 else if (month == "null" && year == "null" && threshold == "All" && type == "All") { searchlicense = @"select * from VIEW_GATE_PIPE_REPORT WHERE LICENSE_NO IN ('" + licenseStr + "')"; }
                 else if (month == "null" && year == "null" && threshold != "All" && type == "All") { searchlicense = @"select * from VIEW_GATE_PIPE_REPORT WHERE LICENSE_NO IN ('" + licenseStr + "') AND THRESHOLD IN ('" + threshold + "')"; }
                 else if (month == "null" && year == "null" && threshold == "All" && type != "All") { searchlicense = @"select * from VIEW_GATE_PIPE_REPORT WHERE LICENSE_NO IN ('" + licenseStr + "') AND TYPE IN ('" + type + "')"; }
                 else if (month == "null" && year == "null" && threshold != "All" && type != "All") { searchlicense = @"select * from VIEW_GATE_PIPE_REPORT WHERE LICENSE_NO IN ('" + licenseStr + "') AND TYPE IN ('" + type + "') AND THRESHOLD IN ('" + threshold + "')"; }
                 var ds = dal.GetDataSet(searchlicense);


                 var listLicense = new List<Models.DataModel.ModelGetU>();
                 if (ds.Tables[0].Rows.Count > 0)
                 {


                     foreach (System.Data.DataRow dr in ds.Tables[0].Rows)
                     {
                         var reg = new Models.DataModel.ModelGetU()
                         {
                             NAME = dr["NAME"].ToString(),
                             REGION = dr["REGION"].ToString(),
                             LICENSE = dr["LICENSE_NO"].GetInt(),
                             VALUE = dr["VALUE"].ToString(),
                             //COLOR = dr["COLOR"].ToString(),
                             MONTH = dr["MONTH"].ToString(),
                             YEAR = dr["YEAR"].ToString(),
                             TYPE = dr["TYPE"].ToString(),
                             THRESHOLD = dr["THRESHOLD"].ToString()
                         };
                         listLicense.Add(reg);
                     }
                 }

                 return Json(listLicense, JsonRequestBehavior.AllowGet);
             }

             public JsonResult Report_CurrentPipeline()
             {
                 var dal = new DAL.DAL();
                 var ds = dal.GetDataSet("SELECT * FROM VIEW_GATE_PIPE_REPORT_CURRENT WHERE TYPE = 'PIPELINE' AND REGION IS NOT NULL");
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

        [HttpPost] public JsonResult VIEW_GATE_PIPE_REPORT_CURRENT(string pType , int[] pArrId)
        {
            var result = new ModelJsonResult<List<ModelViewGatePipeReport>>();
            try
            {
                var dal = new DAL.DAL();
                string strCommand = "SELECT * FROM VIEW_GATE_PIPE_REPORT_CURRENT";
                var listData = dal.ReadData(strCommand, x => new ModelViewGatePipeReport(x));
                if (pArrId != null && pArrId.Any())
                {
                    switch (pType.ToLower())
                    {
                        case "region":
                            listData = listData.Where(x => pArrId.Contains(x.REGION));
                            break;
                        case "license":
                            listData = listData.Where(x => pArrId.Contains(x.LICENSE_NO));
                            break;
                    }
                }
                result.SetResultValue( listData.ToList());

                dal = null;
            }
            catch(Exception ex)
            {
                result.SetException(ex);
            }
            
            return Json(result);
        }

             [HttpPost]
             public JsonResult Report_CurrentGate()
             {
                 var dal = new DAL.DAL();
                 var ds = dal.GetDataSet("SELECT * FROM VIEW_GATE_PIPE_REPORT_CURRENT WHERE TYPE = 'GATESTATION' AND REGION IS NOT NULL");
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
     
           
        

       
    }
    public class utilization
    {
        public string REGION_NAME { get; set; }
        public int REGION_ID { get; set; }
    }

}
