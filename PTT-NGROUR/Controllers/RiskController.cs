using OfficeOpenXml;
using PTT_NGROUR.DTO;
using PTT_NGROUR.ExtentionAndLib;
using PTT_NGROUR.Models.DataModel;
using PTT_NGROUR.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PTT_NGROUR.Controllers
{
    public class RiskController : Controller
    {
        // GET: /Risk/
        [AuthorizeController.CustomAuthorize]
        public ActionResult Index()
        {
            DAL.DAL dal = new DAL.DAL();

            #region License
            var dsLicense = dal.GetDataSet("SELECT LICENSE_ID ,LICENSE FROM LICENSE_MASTER");
            var dtLicense = dsLicense.Tables[0];
            var listLicense = new List<ModelLicenseMaster>();

            foreach (System.Data.DataRow dr in dtLicense.Rows)
            {
                var license = new ModelLicenseMaster()
                {
                    LICENSE = dr["LICENSE"].ToString(),
                    LICENSE_ID = Convert.ToInt32(dr["LICENSE_ID"].ToString())
                };

                listLicense.Add(new ModelLicenseMaster { LICENSE = license.LICENSE, LICENSE_ID = license.LICENSE_ID });
            }
            dsLicense.Dispose();
            dtLicense.Dispose();
            #endregion

            #region Region
            var dsRegion = dal.GetDataSet("SELECT REGION_ID ,REGION_NAME FROM REGION");
            var dtRegion = dsRegion.Tables[0];
            var listRegion = new List<ModelRegion>();

            foreach (System.Data.DataRow drArea in dtRegion.Rows)
            {
                var region = new ModelRegion()
                {
                    REGION_NAME = drArea["REGION_NAME"].ToString(),
                    REGION_ID = Convert.ToInt32(drArea["REGION_ID"].ToString())
                };

                listRegion.Add(new ModelRegion { REGION_NAME = region.REGION_NAME, REGION_ID = region.REGION_ID });
            }
            dtRegion.Dispose();
            dsRegion.Dispose();
            #endregion

            ViewData["AcceptanceCriteria"] = dal.ReadData(
                "SELECT RISK_CRITERIA, UPDATE_DATE, UPDATE_BY FROM RISK_THRESHOLD",
                x => new ModelAcceptanceCriteria(x)).Select(x => x.RISK_CRITERIA).FirstOrDefault();

            var model = new ModelRisk()
            {
                ListLicense = listLicense,
                ListRegion = listRegion
            };

            return View(model);
        }

        // GET: /Risk/Report
        [AuthorizeController.CustomAuthorize]
        public ActionResult Report()
        {
            DAL.DAL dal = new DAL.DAL();

            #region Risk Type
            var dsRiskType = dal.GetDataSet("SELECT ID ,RISK_TYPE FROM RISK_TYPE");
            var dtRiskType = dsRiskType.Tables[0];
            var listRiskType = new List<ModelRiskType>();

            foreach (System.Data.DataRow dr in dtRiskType.Rows)
            {
                var riskType = new ModelRiskType()
                {
                    ID = Convert.ToInt32(dr["ID"].ToString()),
                    RISK_TYPE = dr["RISK_TYPE"].ToString()
                };


                listRiskType.Add(new ModelRiskType { ID = riskType.ID, RISK_TYPE = riskType.RISK_TYPE });
            }
            dsRiskType.Dispose();
            dtRiskType.Dispose();
            #endregion

            #region License
            var dsLicense = dal.GetDataSet("SELECT LICENSE_ID ,LICENSE FROM LICENSE_MASTER");
            var dtLicense = dsLicense.Tables[0];
            var listLicense = new List<ModelLicenseMaster>();

            foreach (System.Data.DataRow dr in dtLicense.Rows)
            {
                var license = new ModelLicenseMaster()
                {
                    LICENSE = dr["LICENSE"].ToString(),
                    LICENSE_ID = Convert.ToInt32(dr["LICENSE_ID"].ToString())
                };

                listLicense.Add(new ModelLicenseMaster { LICENSE = license.LICENSE, LICENSE_ID = license.LICENSE_ID });
            }
            dsLicense.Dispose();
            dtLicense.Dispose();
            #endregion

            #region Region
            var dsRegion = dal.GetDataSet("SELECT REGION_ID ,REGION_NAME FROM REGION");
            var dtRegion = dsRegion.Tables[0];
            var listRegion = new List<ModelRegion>();

            foreach (System.Data.DataRow drArea in dtRegion.Rows)
            {
                var region = new ModelRegion()
                {
                    REGION_NAME = drArea["REGION_NAME"].ToString(),
                    REGION_ID = Convert.ToInt32(drArea["REGION_ID"].ToString())
                };

                listRegion.Add(new ModelRegion { REGION_NAME = region.REGION_NAME, REGION_ID = region.REGION_ID });
            }
            dtRegion.Dispose();
            dsRegion.Dispose();
            #endregion

            ViewData["AcceptanceCriteria"] = dal.ReadData(
                "SELECT RISK_CRITERIA, UPDATE_DATE, UPDATE_BY FROM RISK_THRESHOLD",
                x => new ModelAcceptanceCriteria(x)).Select(x => x.RISK_CRITERIA).FirstOrDefault();

            var model = new ModelRisk()
            {
                ListRiskType = listRiskType,
                ListLicense = listLicense,
                ListRegion = listRegion
            };

            return View(model);
        }

        // POST: /Risk/Json
        [HttpPost]
        [AuthorizeController.CustomAuthorize]
        public JsonResult Json(ModelViewRiskReport model)
        {
            string strCommand = $"SELECT * FROM VIEW_RISK_HISTORY WHERE MONTH = {model.Month} AND YEAR = {model.Year}";

            if (model.Type.Equals("risk"))
            {
                strCommand = $"{strCommand} AND (RISK_SCORE_RISK = 'RISK' OR INTERNAL_CORROSION_RISK = 'RISK' OR EXTERNAL_CORROSION_RISK = 'RISK' OR THIRD_PARTY_INTERFERENCE_RISK = 'RISK' OR LOSS_OF_GROUND_SUPPORT_RISK = 'RISK')";
            }
            else if (model.Lists != null)
            {
                if (model.Type.Equals("region"))
                {
                    strCommand = $"{strCommand} AND REGION IN ('{ string.Join("','", model.Lists) }')";
                }
                else if (model.Type.Equals("license"))
                {
                    strCommand = $"{strCommand} AND LICENSE_NO IN ('{ string.Join("','", model.Lists) }')";
                }
            }
            var dal = new DAL.DAL();
            var riskReport = dal.ReadData(strCommand, x => new Models.DataModel.ModelGetRisk(x)).ToList(); ;

            return Json(riskReport, JsonRequestBehavior.AllowGet);
        }

        #region Import Excel
        // POST: /Risk/ImportExcel
        [AuthorizeController.CustomAuthorize]
        public ActionResult ImportExcel()
        {
            return View();
        }

        // POST: /Risk/UploadFile
        [AuthorizeController.CustomAuthorize]
        public JsonResult UploadFile()
        {
            var dto = new DTO.DtoRisk();

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
                        byte[] fileBytes = new byte[Request.ContentLength];
                        var data = Request.InputStream.Read(fileBytes, 0, Convert.ToInt32(Request.ContentLength));
                        using (var package = new ExcelPackage(Request.InputStream))
                        {
                            var currentSheet = package.Workbook.Worksheets;
                            var workSheet = currentSheet.Single(x => x.Index.Equals(dto.Worksheet));
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

        // POST: /Risk/IsDuplicateExcelData
        [HttpPost]
        [AuthorizeController.CustomAuthorize]
        public JsonResult IsDuplicateExcelData()
        {
            var result = new ModelJsonResult<bool>();
            string inYear = Request["year"];
            string inMonth = Request["month"];

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

            var dto = new DTO.DtoRisk();
            bool isDuplicate = false;
            var listRiskManagement = dto.ReadExcelRiskManagementImport(
                pStreamExcel: fb.InputStream,
                pIntMonth: inMonth.GetInt(),
                pIntYear: inYear.GetInt(),
                pStrUploadBy: User.Identity.Name
            );

            isDuplicate = dto.GetListRiskManagementImportDuplicate(listRiskManagement).Any();
            result.SetResultValue(isDuplicate);

            return Json(result);
        }

        // POST: /Risk/InsertExceldata
        [HttpPost]
        [AuthorizeController.CustomAuthorize]
        public JsonResult InsertExceldata()
        {
            var result = new ModelJsonResult<ModelInsertExcelData>();
            var modelInsertExcel = new ModelInsertExcelData();
            try
            {
                string inYear = Request["year"];
                string inMonth = Request["month"];

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

                insertExcelRiskManagementData(
                   pFileStream: fb.InputStream,
                   pStrUploadBy: User.Identity.Name,
                   pIntMonth: inMonth.GetInt(),
                   pIntYear: inYear.GetInt(),
                   pModelResult: modelInsertExcel
                );
                result.SetResultValue(modelInsertExcel);

                return Json(result);

            }
            catch (Exception ex)
            {
                result.SetException(ex);
                return Json(result);
            }
        }

        private void insertExcelRiskManagementData(
        Stream pFileStream,
        string pStrUploadBy,
        int pIntMonth,
        int pIntYear,
        ModelInsertExcelData pModelResult)
        {
            var dto = new DtoRisk();

            var listExcelRiskManagement = dto.ReadExcelRiskManagementImport(
                pStreamExcel: pFileStream,
                pStrUploadBy: pStrUploadBy,
                pIntMonth: pIntMonth,
                pIntYear: pIntYear
                )
                .Where(x => x != null)
                .GroupBy(x => x.RC)
                .Select(g => g.Last())
                .ToList();

            var duplicateInListAfter = listExcelRiskManagement.GroupBy(x => x.RC)
              .Where(g => g.Count() > 1)
              .Select(y => new { Element = y.Key, Counter = y.Count() })
              .ToList();

            var listRiskManagementDuplicate = dto.GetListRiskManagementImportDuplicate(listExcelRiskManagement)
            .Where(x => x != null).ToList();

            for (int i = listExcelRiskManagement.Count - 1; i >= 0; --i)
            {
                var risks = listExcelRiskManagement[i];
                var risksDuplicate = listRiskManagementDuplicate
                    .FirstOrDefault(x => x.RC == risks.RC && x.MONTH == risks.MONTH && x.YEAR == risks.YEAR);
                // Insert RiskManagement
                if (risksDuplicate == null)
                {
                    dto.InsertRiskManagementImport(risks);
                }
                // If Exists Update Pipeline
                else
                {
                    dto.UpdateRiskManagementImport(risks);
                }
            }

            dto = null;
            listExcelRiskManagement.Clear();
            listExcelRiskManagement = null;
            listRiskManagementDuplicate.Clear();
            listRiskManagementDuplicate = null;
            GC.Collect();
        }
        #endregion

        #region File
        public ActionResult File()
        {
            return View();
        }
        #endregion

        [HttpGet]
        [AuthorizeController.CustomAuthorize]
        #region AcceptanceCriteria
        public ActionResult AcceptanceCriteria()
        {
            var dal = new DAL.DAL();
            string strCommand = "SELECT RISK_CRITERIA, UPDATE_DATE, UPDATE_BY FROM RISK_THRESHOLD";
            var result = dal.ReadData(strCommand, x => new ModelAcceptanceCriteria(x)).FirstOrDefault();
            return View(result);
        }

        [HttpPost]
        [AuthorizeController.CustomAuthorize]
        public ActionResult AcceptanceCriteria(ModelAcceptanceCriteria model)
        {
            if (ModelState.IsValid)
            {
                model.UPDATE_BY = User.Identity.Name;

                var dto = new DtoRisk();
                dto.UpdateAcceptanceCriteria(model);
                dto = null;

                TempData["status"] = "successfully";
                return RedirectToAction("AcceptanceCriteria");
            }

            return View();
        }
        #endregion
    }
}
