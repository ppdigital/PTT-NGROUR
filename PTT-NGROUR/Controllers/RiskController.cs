using OfficeOpenXml;
using PTT_NGROUR.DTO;
using PTT_NGROUR.ExtentionAndLib;
using PTT_NGROUR.Models;
using PTT_NGROUR.Models.DataModel;
using PTT_NGROUR.Models.ViewModel;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PTT_NGROUR.Controllers
{
    public class RiskController : Controller
    {
        private User UserA = new User();

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

            ViewData["UserRoleId"] = UserA.Roleid;
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

        // GET: /Risk/Report
        [AuthorizeController.CustomAuthorize]
        public ActionResult RiskManagementGraph()
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

        //[AuthorizeController.CustomAuthorize]
        public ActionResult RiskManagementGraphPrint(ModelViewRiskReport model)
        {
            if(string.IsNullOrEmpty(model.Year))
            {
                model.Year = DateTime.Now.Year.ToString();
            }

            if (string.IsNullOrEmpty(model.Type))
            {
                model.Type = "risk";
            }

            DAL.DAL dal = new DAL.DAL();

            ViewData["AcceptanceCriteria"] = dal.ReadData(
               "SELECT RISK_CRITERIA, UPDATE_DATE, UPDATE_BY FROM RISK_THRESHOLD",
               x => new ModelAcceptanceCriteria(x)).Select(x => x.RISK_CRITERIA).FirstOrDefault();

            List<ModelGetRisk> riskReport = this.GetData(model);

            return View(riskReport);
        }

        [HttpPost]
        public ActionResult RiskManagementGraphExport(ModelViewRiskReport model)
        {
            return new ActionAsPdf("RiskManagementGraphPrint", model)
            {
                //FileName = "Test.pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Landscape,
                PageMargins = new Margins(5, 0, 5, 0),
                CustomSwitches = "--enable-javascript"
            };
        }

        // POST: /Risk/Json
        [HttpPost]
        //[AuthorizeController.CustomAuthorize]
        public JsonResult Json(ModelViewRiskReport model)
        {
            List<ModelGetRisk> riskReport = this.GetData(model);

            return Json(riskReport, JsonRequestBehavior.AllowGet);
        }

        private List<ModelGetRisk> GetData(ModelViewRiskReport model)
        {
            string strCommand = $"SELECT * FROM VIEW_RISK_HISTORY WHERE YEAR = {model.Year}";

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
            var riskReport = dal.ReadData(strCommand, x => new ModelGetRisk(x)).ToList();

            riskReport.ForEach(x =>
            {
                ModelViewRiskImport _model = new ModelViewRiskImport
                {
                    RC_NAME = x.RC_NAME,
                    YEAR = x.YEAR
                };

                string _path = GetPathUploadPath(_model); ;
                string[] FileList = new string[] { };

                if (Directory.Exists(_path))
                {
                    DirectoryInfo d = new DirectoryInfo(_path);
                    FileInfo[] Files = d.GetFiles(); //Getting files
                    FileList = Files.Select(s => s.Name).ToArray();
                }

                x.FILES = FileList;
                x.HAS_FILE = FileList.Count() > 0;
            });

            return riskReport;
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
            var dto = new DtoRisk();

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
                            var workSheet = currentSheet.Single(x => x.Name.Equals(dto.WorksheetName));
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
        int pIntYear,
        ModelInsertExcelData pModelResult)
        {
            var dto = new DtoRisk();

            var listExcelRiskManagement = dto.ReadExcelRiskManagementImport(
                pStreamExcel: pFileStream,
                pStrUploadBy: pStrUploadBy,
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
                    .FirstOrDefault(x => x.RC == risks.RC && x.YEAR == risks.YEAR);
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

        // POST: /Risk/Upload
        [HttpPost]
        //[AuthorizeController.CustomAuthorize]
        public JsonResult Upload(ModelViewRiskImport model)
        {
            string _dir = GetPathUploadPath(model);

            // Check Directory Exist
            if (!Directory.Exists(_dir))
            {
                Directory.CreateDirectory(_dir);
            }

            foreach (HttpPostedFileBase FILE in model.FILES)
            {
                string _fileName = DateTime.Now.ToString("yyyyMMddHHmmssfff") + "_" + FILE.FileName;
                string _path = Path.Combine(_dir, _fileName);
                FILE.SaveAs(_path);

                var dto = new DtoRisk();
                string username = User.Identity.Name;
                dto.InsertRiskFile(username, model.RC_NAME, model.YEAR, _fileName);
                dto = null;
            }

            return Json(new { });
        }

        // POST: /Risk/FileJson
        [HttpPost]
        [AuthorizeController.CustomAuthorize]
        public JsonResult FileJson(string mode)
        {
            StringBuilder strCommand = new StringBuilder();
            //strCommand.AppendFormat("SELECT * FROM RISK_FILE WHERE YEAR = {0}", model.YEAR);
            strCommand.Append("SELECT * FROM RISK_FILE");

            if (string.IsNullOrEmpty(mode) || mode.Equals("year"))
            {
                strCommand.AppendFormat(" WHERE RC_NAME IS NULL");
            }
            else
            {
                strCommand.AppendFormat(" WHERE RC_NAME IS NOT NULL");
            }

            DAL.DAL dal = new DAL.DAL();
            List<ModelGetRiskFile> riskFiles = dal.ReadData(strCommand.ToString(), x => new ModelGetRiskFile(x)).ToList();

            //Filter file in system
            riskFiles = riskFiles.Where(x => this.FilterFileInSystem(x)).ToList();

            // List by Yearly
            if (string.IsNullOrEmpty(mode) || mode.Equals("year"))
            {
                return Json(riskFiles.GroupBy(x => x.YEAR).Select(x => new
                {
                    YEAR = x.Key,
                    LAST_UPDATED_AT = x.OrderByDescending(o => o.UPLOADED_AT).Select(o => o.UPLOADED_AT).First(),
                    LAST_UPADTED_BY = x.OrderByDescending(o => o.UPLOADED_AT).Select(o => o.UPLOADED_BY).First(),
                    FILES = x.Select(o => new
                    {
                        o.FILE_NAME,
                        o.YEAR,
                        o.RC_NAME
                    })
                    .ToList()
                }), JsonRequestBehavior.AllowGet);
            }

            // List by RC
            return Json(riskFiles.GroupBy(x => x.RC_NAME).Select(x => new
            {
                RC_NAME = x.Key,
                LAST_UPDATED_AT = x.OrderByDescending(o => o.UPLOADED_AT).Select(o => o.UPLOADED_AT).First(),
                LAST_UPADTED_BY = x.OrderByDescending(o => o.UPLOADED_AT).Select(o => o.UPLOADED_BY).First(),
                FILES = x.Select(o => new
                {
                    o.FILE_NAME,
                    o.YEAR,
                    o.RC_NAME
                })
                .ToList()
            }), JsonRequestBehavior.AllowGet);
        }

        // GET: /Risk/ListFile
        [HttpGet]
        //[AuthorizeController.CustomAuthorize]
        public JsonResult ListFile(ModelViewRiskImport model)
        {
            StringBuilder strCommand = new StringBuilder();
            strCommand.AppendFormat("SELECT * FROM RISK_FILE WHERE YEAR = {0} AND RC_NAME = '{1}'", model.YEAR, model.RC_NAME);

            DAL.DAL dal = new DAL.DAL();
            List<ModelGetRiskFile> riskFiles = dal.ReadData(strCommand.ToString(), x => new ModelGetRiskFile(x)).ToList();

            //Filter file in system
            string[] FileList = riskFiles.Where(x => this.FilterFileInSystem(x)).Select(x => x.FILE_NAME).ToArray();

            return Json(FileList, JsonRequestBehavior.AllowGet);
        }

        private bool FilterFileInSystem(ModelGetRiskFile riskFiles)
        {
            ModelViewRiskImport _model = new ModelViewRiskImport
            {
                RC_NAME = riskFiles.RC_NAME,
                YEAR = riskFiles.YEAR
            };

            string _path = Path.Combine(GetPathUploadPath(_model), riskFiles.FILE_NAME);
            return System.IO.File.Exists(_path);
        }

        // GET: Risk/Download/2019/RC0653110116/3/25620429092912544_1.txt
        [HttpGet]
        [AuthorizeController.CustomAuthorize]
        public ActionResult Download(ModelViewRiskDownload model)
        {
            ModelViewRiskImport _model = new ModelViewRiskImport
            {
                RC_NAME = model.RC_NAME,
                YEAR = model.YEAR
            };

            string _path = Path.Combine(GetPathUploadPath(_model), model.FILE_NAME);
            if (System.IO.File.Exists(_path))
            {
                FileStream file = System.IO.File.OpenRead(_path);
                return File(file, "Application/octet-stream", model.FILE_NAME);
            }

            return HttpNotFound();
        }

        private string GetPathUploadPath(ModelViewRiskImport model)
        {
            string dir = Server.MapPath("~/UploadedFiles");

            if (string.IsNullOrEmpty(model.RC_NAME))
            {
                dir = Path.Combine(dir, "year", model.YEAR.ToString());
            }
            else
            {
                dir = Path.Combine(dir, "rc", model.RC_NAME, model.YEAR.ToString());
            }

            return dir;
        }
        #endregion

        #region AcceptanceCriteria
        [HttpGet]
        [AuthorizeController.CustomAuthorize]
        public ActionResult AcceptanceCriteria()
        {
            DAL.DAL dal = new DAL.DAL();
            string strCommand = "SELECT RISK_CRITERIA, UPDATE_DATE, UPDATE_BY FROM RISK_THRESHOLD";
            ModelAcceptanceCriteria result = dal.ReadData(strCommand, x => new ModelAcceptanceCriteria(x)).FirstOrDefault();

            result.PreviousUrl = System.Web.HttpContext.Current.Request.UrlReferrer.ToString() ?? Url.Action("Index");

            return View(result);
        }

        [HttpPost]
        [AuthorizeController.CustomAuthorize]
        public ActionResult AcceptanceCriteria(ModelAcceptanceCriteria model)
        {
            if (ModelState.IsValid)
            {
                model.UPDATE_BY = User.Identity.Name;

                DtoRisk dto = new DtoRisk();
                dto.UpdateAcceptanceCriteria(model);
                dto = null;

                TempData["status"] = "successfully";
                TempData["PreviousUrl"] = model.PreviousUrl;
                return RedirectToAction("AcceptanceCriteria");
            }

            return View();
        }
        #endregion
    }
}
