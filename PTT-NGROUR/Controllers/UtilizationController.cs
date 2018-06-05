using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
//using OUR_App.Models;
//using OfficeOpenXml;
using System.IO;
using OfficeOpenXml;

namespace OUR_App.Controllers
{
    public class UtilizationController : Controller
    {
        //
        // GET: /Utilization/

        #region - Col PipeLine -

        private const int _intColPipelineName = 0;
        private const int  _intColPipelineFlow = 1;
        private const int  _intColPipelineDiameter = 2;
        private const int  _intColPipelineLength = 3;
        private const int  _intColPipelineEfficiency = 4;
        private const int  _intColPipelineRoughness = 5;
        private const int  _intColPipelineLoad = 6;
        private const int  _intColPipelineResultDownstreamVelocity = 7;
        private const int  _intColPipelineOutsideDiameter = 8;
        private const int  _intColPipelineWallThickness = 9;
        private const int _intColPipelineServiceState = 10;

        #endregion
        
        #region - Col GateStation - 

        private const int _intColGateName = 0;
        private const int _intColGatePressure = 1;
        private const int _intColGateFlow = 2;
        private const int _intColGateDescription = 3;

        #endregion
        public ActionResult Index()
        {
            return View();
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
        public ActionResult InsertExceldata()
        {
            string inYear = Request["year"];
            string inMonth = Request["month"];
            string inRegion = Request["region"];
            string inType = Request["type"];



            var CountRowInDBBeforInsert = 0;
            string fPath = "";
            string fname = "";

            

            HttpFileCollectionBase files = Request.Files;

            
            if (files != null && files.Count > 0)
            {
                foreach (HttpPostedFileBase fb in files)
                {
                    if (fb == null || fb.InputStream == null)
                    {
                        continue;
                    }
                    string exttension = System.IO.Path.GetExtension(fb.FileName);
                    if (!(new string[] { ".xls", ".xlsx" }).Contains(exttension))
                    {
                        continue;
                    }
                    
                    var package = new ExcelPackage(fb.InputStream);
                    if (package.Workbook.Worksheets.Any())
                    {
                        var exWorkSheet = package.Workbook.Worksheets[1];

                        int intColCount = exWorkSheet.Dimension.End.Column;
                        int intRowCount = exWorkSheet.Dimension.End.Row;

                        int intStartRow = 4;
                        int intEndRow = intRowCount - 6;
                    
                        switch (inType) {
                            case "gate":
                                for (int intRow = intStartRow; intRow < intEndRow; ++intRow)
                                {
                                    string strGateName = exWorkSheet.Cells[intRow, _intColGateName].Text;
                                    string strG = exWorkSheet.Cells[intRow , _intColGatePressure].
                                }   
                                break;
                            case "pipeline":
                                for (int intRow = intStartRow; intRow < intEndRow; ++intRow)
                                {

                                }
                                break;
                        }

                    }


                    package.Dispose();
                    package = null;
                }
                //------------------------------[ rdxrydjyt ]

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    
                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = file.FileName;
                    }

                    fPath = Path.Combine(Server.MapPath("~/Content/ExcelFile/"), fname);
                    file.SaveAs(fPath);
                }

                var fileInfo = new FileInfo(@fPath);
                string query = string.Format("Select * from [{0}]", "Sheet1$");
                string CheckTypeFlieUpload = Path.GetExtension(fname);
                if (CheckTypeFlieUpload == ".xls" || CheckTypeFlieUpload == ".xlsx")
                {
                    using (ExcelPackage package = new ExcelPackage(fileInfo))
                    {
                        var filenameExcel = fname;
                        ExcelWorksheet wors = package.Workbook.Worksheets[1];
                        string[] columnName = new string[wors.Dimension.End.Column]; //หัวคอลัมภ์
                        string[,] DataTable = new string[wors.Dimension.End.Row - 1, wors.Dimension.End.Column]; // data in excel
                        string table_name = wors.Name; // ชือ Sheet


                        int k = 4; //ตัดหัวออก เริ่มบรรทัดที่ 4
                        int num = 0;

                        for (int j = 0; j < wors.Dimension.End.Row - 6; j++)
                        {
                            for (int i = 0; i < wors.Dimension.End.Column; i++)
                            {

                                if (wors.Cells[k, i + 1].Text != "")
                                {
                                    if (wors.Cells[k, i + 1].Text != "")
                                    {
                                        DataTable[num, i] = wors.Cells[k, i + 1].Text;

                                    }

                                }


                            }
                            k = k + 1;
                            num = num + 1;
                        }

                        Console.WriteLine(DataTable);
                        int countRow = wors.Dimension.End.Row - 6;   // count row in excel
                        var cr = new EntitiesImportExcel();
                        //int num_gate = 20;

                        //------------Count Row In DB Befor Insert-------------
                        if (inType == "gate")
                        {
                            using (var ccs = new EntitiesImportExcel())
                            {
                                CountRowInDBBeforInsert = (from o in ccs.GATESTATION_IMPORT
                                                           select o).Count();
                            }
                            //------------End Count Row In DB Befor Insert--------------

                            if (countRow > 0)
                            {
                                try
                                {
                                    string inGATE_NAME = "";
                                    string inPRESSURE = "";
                                    string inFLOW = "";

                                    for (int x = 0; x < wors.Dimension.End.Row - 9; x++)
                                    {

                                        int y = 0;
                                        inGATE_NAME = DataTable[x, y].ToString();
                                        inPRESSURE = DataTable[x, y + 1].ToString();
                                        inFLOW = DataTable[x, y + 2].ToString();

                                        try
                                        {

                                            using (var ccs = new EntitiesImportExcel())
                                            {
                                                GATESTATION_IMPORT cs = new GATESTATION_IMPORT();

                                                if (inGATE_NAME == "NULL")
                                                {
                                                    return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet);
                                                }
                                                else { cs.GATE_NAME = Convert.ToString(inGATE_NAME); }


                                                if (inPRESSURE == "NULL")
                                                {
                                                    cs.PRESSURE = null;
                                                }
                                                else
                                                {
                                                    cs.PRESSURE = Convert.ToString(inPRESSURE);
                                                }


                                                if (inFLOW == "NULL")
                                                {
                                                    cs.FLOW = null;
                                                }
                                                else
                                                {
                                                    cs.FLOW = Convert.ToDecimal(inFLOW);
                                                }
                                                // cs.MONTH = 5;
                                                cs.YEAR = Convert.ToDecimal(inYear);
                                                cs.MONTH = Convert.ToDecimal(inMonth);
                                                //cs.YEAR = Convert.ToDecimal(Request["Year"]);

                                                //  cs.YEAR = Convert.ToDecimal(Request["seYear"].ToString());

                                                cs.UPLOAD_DATE = DateTime.Now;
                                                cs.UPLOAD_BY = "User1";
                                                cs.REGION_ID = Convert.ToDecimal(inRegion);

                                                ccs.GATESTATION_IMPORT.Add(cs);
                                                ccs.SaveChanges();

                                            } // num_gate = num_gate + 1; 
                                        }
                                        catch
                                        {
                                            // ข้อมูลซำใน Database return Content("เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่");
                                            return Json(new { success = false, responseText = "เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่" });
                                        }


                                    } //

                                }//
                                catch
                                { return Json(new { success = false, responseText = "เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่" }); }//1

                            }
                        }
                        else if (inType == "pipeline")
                        {

                            using (var ccs = new EntitiesImportExcel())
                            {
                                CountRowInDBBeforInsert = (from o in ccs.PIPELINE_IMPORT
                                                           select o).Count();
                            }
                            //------------End Count Row In DB Befor Insert--------------

                            if (countRow > 0)
                            {
                                try
                                {

                                    string inRC_NAME = "";
                                    string inFLOW = "";
                                    string inDIAMETER = "";
                                    string inLENGTH = "";
                                    string inEFFICIENCY = "";
                                    string inROUGHNESS = "";
                                    string inLOAD = "";
                                    string inVELOCITY = "";
                                    string inOUTSIDE_DIAMETER = "";
                                    string inWALL_THICKNESS = "";
                                    string inSERVICE_STATE = "";

                                    for (int x = 0; x < wors.Dimension.End.Row - 9; x++)
                                    {
                                        int y = 0;

                                        inRC_NAME = DataTable[x, y].ToString();
                                        inFLOW = DataTable[x, y + 1].ToString();
                                        inDIAMETER = DataTable[x, y + 2].ToString();
                                        inLENGTH = DataTable[x, y + 3].ToString();
                                        inEFFICIENCY = DataTable[x, y + 4].ToString();
                                        inROUGHNESS = DataTable[x, y + 5].ToString();
                                        inLOAD = DataTable[x, y + 6].ToString();
                                        inVELOCITY = DataTable[x, y + 7].ToString();
                                        inOUTSIDE_DIAMETER = DataTable[x, y + 8].ToString();
                                        inWALL_THICKNESS = DataTable[x, y + 9].ToString();
                                        inSERVICE_STATE = DataTable[x, y + 10].ToString();

                                        try
                                        {


                                            using (var ccs = new EntitiesImportExcel())
                                            {
                                                PIPELINE_IMPORT cs = new PIPELINE_IMPORT();
                                                //  cs.PIPELINE_ID = y + 1;
                                                cs.MONTH = Convert.ToDecimal(inMonth);
                                                cs.YEAR = Convert.ToDecimal(inYear);
                                                cs.UPLOAD_DATE = DateTime.Now;
                                                if (inRC_NAME == "NULL")
                                                { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
                                                else { cs.RC_NAME = Convert.ToString(inRC_NAME); }

                                                if (inFLOW == "NULL")
                                                { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
                                                else { cs.FLOW = Convert.ToDecimal(inFLOW); }

                                                if (inDIAMETER == "NULL")
                                                { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
                                                else { cs.DIAMETER = Convert.ToDecimal(inDIAMETER); }

                                                if (inLENGTH == "NULL")
                                                { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
                                                else
                                                { cs.LENGTH = Convert.ToDecimal(inLENGTH); }

                                                if (inEFFICIENCY == "NULL")
                                                { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
                                                else
                                                { cs.EFFICIENCY = Convert.ToDecimal(inEFFICIENCY); }

                                                if (inROUGHNESS == "NULL")
                                                { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
                                                else
                                                { cs.ROUGHNESS = Convert.ToDecimal(inROUGHNESS); }

                                                if (inLOAD == "NULL")
                                                { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
                                                else
                                                { cs.LOAD = Convert.ToDecimal(inLOAD); }

                                                if (inVELOCITY == "NULL")
                                                { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
                                                else
                                                { cs.VELOCITY = Convert.ToDecimal(inVELOCITY); }

                                                if (inOUTSIDE_DIAMETER == "NULL")
                                                { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
                                                else
                                                { cs.OUTSIDE_DIAMETER = Convert.ToDecimal(inOUTSIDE_DIAMETER); }

                                                if (inWALL_THICKNESS == "NULL")
                                                { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
                                                else
                                                { cs.WALL_THICKNESS = Convert.ToDecimal(inWALL_THICKNESS); }

                                                if (inSERVICE_STATE == "NULL")
                                                { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
                                                else
                                                { cs.SERVICE_STATE = Convert.ToString(inSERVICE_STATE); }



                                                //  cs.YEAR = Convert.ToDecimal(Request["seYear"].ToString());


                                                cs.UPLOAD_BY = "User1";
                                                cs.REGION_ID = Convert.ToDecimal(inRegion);

                                                ccs.PIPELINE_IMPORT.Add(cs);
                                                ccs.SaveChanges();

                                            }
                                        }
                                        catch
                                        {
                                            // ข้อมูลซำใน Database return Content("เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่");
                                            return Json(new { success = false, responseText = "เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่" });
                                        }

                                    } //

                                }//
                                catch
                                { return Json(new { success = false, responseText = "เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่" }); }//1



                            }
                        }
                        return Json(new { success = true, responseText = "เพิ่มข้อมูลสำเร็จ" });

                        //------------Count Row In DB After Insert-------------

                        /* var CountRowInDBAfterInsert = 0;
                         using (var ccs1 = new EntitiesImportExcel())
                         {
                             CountRowInDBAfterInsert = (from o in ccs1.GATESTATION_IMPORT
                                                        select o).Count();
                         }
                         //------------End Count Row In DB After Insert--------------

                         int countRowbeforALL = CountRowInDBBeforInsert + countRow-3;

                         if (countRowbeforALL == CountRowInDBAfterInsert)
                         {
                             //Process.Start("C:\\Quest Software\\Toad for Oracle 11.6\\Toad.exe");
                             //Process.Start("C:\\Users\\Administrator\\Desktop\\Autogen_ห้ามลบ\\RouteGenerator.exe");
                             //Process.Start("D:\\Users\\zparinya.th\\Desktop\\stands\\The_world_c252.png");
                             return Json(new { responseText = "เพิ่มข้อมูลสำเร็จ" });
                         }
                         else
                         {
                             return Json(new { success = false, responseText = "เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่" });
                         } */



                    }

                }

                else
                {
                    //return Content("file ไม่ถูกต้อง กรุณาเลือก file ใหม่");
                    return Json(new { success = false, responseText = "file ไม่ถูกต้อง กรุณาเลือก file ใหม่" });
                }

            }
            else
            {
                //return Content("กรุณาเลือกไฟล์ที่ต้องการ Upload");
                return Json(new { success = false, responseText = "กรุณาเลือกไฟล์ที่ต้องการ Upload" });
            }

        }//3

        //[HttpPost]
        //public ActionResult InsertExceldata(string year, string month, string region, string type)
        //{
        //    string inYear = Request["year"];
        //    string inMonth = Request["month"];
        //    string inRegion = Request["region"];
        //    string inType = Request["type"];



        //    var CountRowInDBBeforInsert = 0;
        //    string fPath = "";
        //    string fname = "";

        //    // string inYear = Year;

        //    // string inYear = Convert.ToDecimal(Request["seYear"].ToString());
        //    //HttpFileCollectionBase files = Request.Files;
        //    // string year = Request.Year;

        //    HttpFileCollectionBase files = Request.Files;



        //    if (files.Count > 0)
        //    {

        //        for (int i = 0; i < files.Count; i++)
        //        {
        //            HttpPostedFileBase file = files[i];

        //            if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
        //            {
        //                string[] testfiles = file.FileName.Split(new char[] { '\\' });
        //                fname = testfiles[testfiles.Length - 1];
        //            }
        //            else
        //            {
        //                fname = file.FileName;
        //            }

        //            fPath = Path.Combine(Server.MapPath("~/Content/ExcelFile/"), fname);
        //            file.SaveAs(fPath);
        //        }

        //        var fileInfo = new FileInfo(@fPath);
        //        string query = string.Format("Select * from [{0}]", "Sheet1$");
        //        string CheckTypeFlieUpload = Path.GetExtension(fname);
        //        if (CheckTypeFlieUpload == ".xls" || CheckTypeFlieUpload == ".xlsx")
        //        {
        //            using (ExcelPackage package = new ExcelPackage(fileInfo))
        //            {
        //                var filenameExcel = fname;
        //                ExcelWorksheet wors = package.Workbook.Worksheets[1];
        //                string[] columnName = new string[wors.Dimension.End.Column]; //หัวคอลัมภ์
        //                string[,] DataTable = new string[wors.Dimension.End.Row - 1, wors.Dimension.End.Column]; // data in excel
        //                string table_name = wors.Name; // ชือ Sheet


        //                int k = 4; //ตัดหัวออก เริ่มบรรทัดที่ 4
        //                int num = 0;

        //                for (int j = 0; j < wors.Dimension.End.Row - 6; j++)
        //                {
        //                    for (int i = 0; i < wors.Dimension.End.Column; i++)
        //                    {

        //                        if (wors.Cells[k, i + 1].Text != "")
        //                        {
        //                            if (wors.Cells[k, i + 1].Text != "")
        //                            {
        //                                DataTable[num, i] = wors.Cells[k, i + 1].Text;

        //                            }

        //                        }


        //                    }
        //                    k = k + 1;
        //                    num = num + 1;
        //                }

        //                Console.WriteLine(DataTable);
        //                int countRow = wors.Dimension.End.Row - 6;   // count row in excel
        //                var cr = new EntitiesImportExcel();
        //                //int num_gate = 20;

        //                //------------Count Row In DB Befor Insert-------------
        //                if (inType == "gate")
        //                {
        //                    using (var ccs = new EntitiesImportExcel())
        //                    {
        //                        CountRowInDBBeforInsert = (from o in ccs.GATESTATION_IMPORT
        //                                                   select o).Count();
        //                    }
        //                    //------------End Count Row In DB Befor Insert--------------

        //                    if (countRow > 0)
        //                    {
        //                        try
        //                        {
        //                            string inGATE_NAME = "";
        //                            string inPRESSURE = "";
        //                            string inFLOW = "";

        //                            for (int x = 0; x < wors.Dimension.End.Row - 9; x++)
        //                            {

        //                                int y = 0;
        //                                inGATE_NAME = DataTable[x, y].ToString();
        //                                inPRESSURE = DataTable[x, y + 1].ToString();
        //                                inFLOW = DataTable[x, y + 2].ToString();

        //                                try
        //                                {

        //                                    using (var ccs = new EntitiesImportExcel())
        //                                    {
        //                                        GATESTATION_IMPORT cs = new GATESTATION_IMPORT();

        //                                        if (inGATE_NAME == "NULL")
        //                                        {
        //                                            return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet);
        //                                        }
        //                                        else { cs.GATE_NAME = Convert.ToString(inGATE_NAME); }


        //                                        if (inPRESSURE == "NULL")
        //                                        {
        //                                            cs.PRESSURE = null;
        //                                        }
        //                                        else
        //                                        {
        //                                            cs.PRESSURE = Convert.ToString(inPRESSURE);
        //                                        }


        //                                        if (inFLOW == "NULL")
        //                                        {
        //                                            cs.FLOW = null;
        //                                        }
        //                                        else
        //                                        {
        //                                            cs.FLOW = Convert.ToDecimal(inFLOW);
        //                                        }
        //                                        // cs.MONTH = 5;
        //                                        cs.YEAR = Convert.ToDecimal(inYear);
        //                                        cs.MONTH = Convert.ToDecimal(inMonth);
        //                                        //cs.YEAR = Convert.ToDecimal(Request["Year"]);

        //                                        //  cs.YEAR = Convert.ToDecimal(Request["seYear"].ToString());

        //                                        cs.UPLOAD_DATE = DateTime.Now;
        //                                        cs.UPLOAD_BY = "User1";
        //                                        cs.REGION_ID = Convert.ToDecimal(inRegion);

        //                                        ccs.GATESTATION_IMPORT.Add(cs);
        //                                        ccs.SaveChanges();

        //                                    } // num_gate = num_gate + 1; 
        //                                }
        //                                catch
        //                                {
        //                                    // ข้อมูลซำใน Database return Content("เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่");
        //                                    return Json(new { success = false, responseText = "เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่" });
        //                                }


        //                            } //

        //                        }//
        //                        catch
        //                        { return Json(new { success = false, responseText = "เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่" }); }//1

        //                    }
        //                }
        //                else if (inType == "pipeline")
        //                {

        //                    using (var ccs = new EntitiesImportExcel())
        //                    {
        //                        CountRowInDBBeforInsert = (from o in ccs.PIPELINE_IMPORT
        //                                                   select o).Count();
        //                    }
        //                    //------------End Count Row In DB Befor Insert--------------

        //                    if (countRow > 0)
        //                    {
        //                        try
        //                        {

        //                            string inRC_NAME = "";
        //                            string inFLOW = "";
        //                            string inDIAMETER = "";
        //                            string inLENGTH = "";
        //                            string inEFFICIENCY = "";
        //                            string inROUGHNESS = "";
        //                            string inLOAD = "";
        //                            string inVELOCITY = "";
        //                            string inOUTSIDE_DIAMETER = "";
        //                            string inWALL_THICKNESS = "";
        //                            string inSERVICE_STATE = "";

        //                            for (int x = 0; x < wors.Dimension.End.Row - 9; x++)
        //                            {
        //                                int y = 0;

        //                                inRC_NAME = DataTable[x, y].ToString();
        //                                inFLOW = DataTable[x, y + 1].ToString();
        //                                inDIAMETER = DataTable[x, y + 2].ToString();
        //                                inLENGTH = DataTable[x, y + 3].ToString();
        //                                inEFFICIENCY = DataTable[x, y + 4].ToString();
        //                                inROUGHNESS = DataTable[x, y + 5].ToString();
        //                                inLOAD = DataTable[x, y + 6].ToString();
        //                                inVELOCITY = DataTable[x, y + 7].ToString();
        //                                inOUTSIDE_DIAMETER = DataTable[x, y + 8].ToString();
        //                                inWALL_THICKNESS = DataTable[x, y + 9].ToString();
        //                                inSERVICE_STATE = DataTable[x, y + 10].ToString();

        //                                try
        //                                {


        //                                    using (var ccs = new EntitiesImportExcel())
        //                                    {
        //                                        PIPELINE_IMPORT cs = new PIPELINE_IMPORT();
        //                                        //  cs.PIPELINE_ID = y + 1;
        //                                        cs.MONTH = Convert.ToDecimal(inMonth);
        //                                        cs.YEAR = Convert.ToDecimal(inYear);
        //                                        cs.UPLOAD_DATE = DateTime.Now;
        //                                        if (inRC_NAME == "NULL")
        //                                        { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
        //                                        else { cs.RC_NAME = Convert.ToString(inRC_NAME); }

        //                                        if (inFLOW == "NULL")
        //                                        { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
        //                                        else { cs.FLOW = Convert.ToDecimal(inFLOW); }

        //                                        if (inDIAMETER == "NULL")
        //                                        { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
        //                                        else { cs.DIAMETER = Convert.ToDecimal(inDIAMETER); }

        //                                        if (inLENGTH == "NULL")
        //                                        { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
        //                                        else
        //                                        { cs.LENGTH = Convert.ToDecimal(inLENGTH); }

        //                                        if (inEFFICIENCY == "NULL")
        //                                        { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
        //                                        else
        //                                        { cs.EFFICIENCY = Convert.ToDecimal(inEFFICIENCY); }

        //                                        if (inROUGHNESS == "NULL")
        //                                        { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
        //                                        else
        //                                        { cs.ROUGHNESS = Convert.ToDecimal(inROUGHNESS); }

        //                                        if (inLOAD == "NULL")
        //                                        { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
        //                                        else
        //                                        { cs.LOAD = Convert.ToDecimal(inLOAD); }

        //                                        if (inVELOCITY == "NULL")
        //                                        { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
        //                                        else
        //                                        { cs.VELOCITY = Convert.ToDecimal(inVELOCITY); }

        //                                        if (inOUTSIDE_DIAMETER == "NULL")
        //                                        { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
        //                                        else
        //                                        { cs.OUTSIDE_DIAMETER = Convert.ToDecimal(inOUTSIDE_DIAMETER); }

        //                                        if (inWALL_THICKNESS == "NULL")
        //                                        { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
        //                                        else
        //                                        { cs.WALL_THICKNESS = Convert.ToDecimal(inWALL_THICKNESS); }

        //                                        if (inSERVICE_STATE == "NULL")
        //                                        { return Json(new { success = false, responseText = "มีข้อมูลที่เป็นช่องว่าง" }, JsonRequestBehavior.AllowGet); }
        //                                        else
        //                                        { cs.SERVICE_STATE = Convert.ToString(inSERVICE_STATE); }



        //                                        //  cs.YEAR = Convert.ToDecimal(Request["seYear"].ToString());


        //                                        cs.UPLOAD_BY = "User1";
        //                                        cs.REGION_ID = Convert.ToDecimal(inRegion);

        //                                        ccs.PIPELINE_IMPORT.Add(cs);
        //                                        ccs.SaveChanges();

        //                                    }
        //                                }
        //                                catch
        //                                {
        //                                    // ข้อมูลซำใน Database return Content("เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่");
        //                                    return Json(new { success = false, responseText = "เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่" });
        //                                }

        //                            } //

        //                        }//
        //                        catch
        //                        { return Json(new { success = false, responseText = "เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่" }); }//1



        //                    }
        //                }
        //                return Json(new { success = true, responseText = "เพิ่มข้อมูลสำเร็จ" });

        //                //------------Count Row In DB After Insert-------------

        //                /* var CountRowInDBAfterInsert = 0;
        //                 using (var ccs1 = new EntitiesImportExcel())
        //                 {
        //                     CountRowInDBAfterInsert = (from o in ccs1.GATESTATION_IMPORT
        //                                                select o).Count();
        //                 }
        //                 //------------End Count Row In DB After Insert--------------

        //                 int countRowbeforALL = CountRowInDBBeforInsert + countRow-3;

        //                 if (countRowbeforALL == CountRowInDBAfterInsert)
        //                 {
        //                     //Process.Start("C:\\Quest Software\\Toad for Oracle 11.6\\Toad.exe");
        //                     //Process.Start("C:\\Users\\Administrator\\Desktop\\Autogen_ห้ามลบ\\RouteGenerator.exe");
        //                     //Process.Start("D:\\Users\\zparinya.th\\Desktop\\stands\\The_world_c252.png");
        //                     return Json(new { responseText = "เพิ่มข้อมูลสำเร็จ" });
        //                 }
        //                 else
        //                 {
        //                     return Json(new { success = false, responseText = "เพิ่มข้อมูลไม่สำเร็จกรุณาเพิ่มข้อมูลใหม่" });
        //                 } */



        //            }

        //        }

        //        else
        //        {
        //            //return Content("file ไม่ถูกต้อง กรุณาเลือก file ใหม่");
        //            return Json(new { success = false, responseText = "file ไม่ถูกต้อง กรุณาเลือก file ใหม่" });
        //        }

        //    }
        //    else
        //    {
        //        //return Content("กรุณาเลือกไฟล์ที่ต้องการ Upload");
        //        return Json(new { success = false, responseText = "กรุณาเลือกไฟล์ที่ต้องการ Upload" });
        //    }

        //}//3

    }
}
