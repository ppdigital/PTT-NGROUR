using PTT_NGROUR.DTO;
using PTT_NGROUR.ExtentionAndLib;
using PTT_NGROUR.Models.DataModel;
using PTT_NGROUR.Models.ViewModel;
using Rotativa;
using Rotativa.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static PTT_NGROUR.Models.DataModel.ModelOMAccumulated;
using static PTT_NGROUR.Models.DataModel.ModelOMCompletion;
using static PTT_NGROUR.Models.DataModel.ModelOMSummary;

namespace PTT_NGROUR.Controllers
{
    public class OMController : Controller
    {
        //
        // GET: /OM/
        //S[PTT_NGROUR.Controllers.AuthorizeController.CustomAuthorize]
        [AuthorizeController.CustomAuthorize]
        public ActionResult Index(string radioMY, string pStrYear, string pStrMonth, string[] pArrRegion)
        {
            if (radioMY == "year") pStrMonth = null;
            string mode = radioMY == "year" ? "yearly" : "monthly";

            ModelJsonResult<ModelOmIndex> data = GetData(pStrYear, pStrMonth, pArrRegion, mode);

            return View(data);
        }

        //[AuthorizeController.CustomAuthorize]
        public ActionResult Print(string radioMY, string pStrYear, string pStrMonth, string[] pArrRegion)
        {
            if (radioMY == "year") pStrMonth = null;
            string mode = radioMY == "year" ? "yearly" : "monthly";

            ModelJsonResult<ModelOmIndex> data = GetData(pStrYear, pStrMonth, pArrRegion, mode);
            
            return View(data);
        }

        [HttpPost]
        //[AuthorizeController.CustomAuthorize]
        public ActionResult SearchData(string pStrYear, string pStrMonth, string[] pArrRegion)
        {
            string mode = pStrMonth == null ? "yearly" : "monthly";

            return Json(GetData(pStrYear, pStrMonth, pArrRegion, mode));
        }

        public ActionResult Export(string pStrYear, string pStrMonth, string[] pArrRegion)
        {
            return new ActionAsPdf("Print", new
            {
                pStrYear,
                pStrMonth,
                pArrRegion,
            }) {
                //FileName = "Test.pdf",
                PageSize = Size.A4,
                PageOrientation = Orientation.Landscape,
                PageMargins = new Margins(5, 0, 5, 0),
                CustomSwitches = "--enable-javascript"
            };
        }


        //public ActionResult Export()
        //{
        //    // Load the Excel Template
        //    var _dir = Server.MapPath($"~/Templates/");
        //    Stream pdfStream = System.IO.File.OpenRead(Path.Combine(_dir, "OM_PM_IA_Montoring_Results.pdf"));// Load the PDF Template

        //    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //    PdfFont font = new PdfStandardFont(PdfFontFamily.Helvetica, (float)8);
        //    //PdfFont fontText = new PdfStandardFont(PdfFontFamily.Helvetica, (float)12);
        //    PdfFont fontTextTHSarabunNew = new PdfTrueTypeFont(System.IO.File.OpenRead(_hostingEnvironment.WebRootPath + $@"\assets\fonts\THSarabunNew\THSarabunNew Bold.ttf"), 15);
        //    PdfFont fontTextCalibri = new PdfTrueTypeFont(System.IO.File.OpenRead(_hostingEnvironment.WebRootPath + $@"\assets\fonts\calibri\Calibri.ttf"), 13);
        //    PdfFont fontTextCalibriBold = new PdfTrueTypeFont(System.IO.File.OpenRead(_hostingEnvironment.WebRootPath + $@"\assets\fonts\calibri\Calibri.ttf"), 13, PdfFontStyle.Bold);

        //    // Load a PDF document.
        //    PdfLoadedDocument loadedDocument = new PdfLoadedDocument(pdfStream);

        //    //Create a new PDF document.
        //    PdfDocument pdfDocument = new PdfDocument();

        //    int numPage = 1;
        //    PdfPage pdfPage;

        //    //Set the format for string.
        //    PdfStringFormat formatAlignRight = new PdfStringFormat(PdfTextAlignment.Right);
        //    PdfStringFormat formatAlignCenter = new PdfStringFormat(PdfTextAlignment.Center);

        //    edo.ForEach(e => {
        //        //
        //    });

        //    //Set properties to paginate the table.
        //    PdfGridLayoutFormat layoutFormat = new PdfGridLayoutFormat();
        //    layoutFormat.Break = PdfLayoutBreakType.FitElement;
        //    layoutFormat.Layout = PdfLayoutType.Paginate;
        //    layoutFormat.PaginateBounds = new RectangleF(20, 20, pdfDocument.Pages[0].GetClientSize().Width - 40, pdfDocument.Pages[0].GetClientSize().Height - 50);

        //    //Create a Page template that can be used as footer.
        //    RectangleF bounds = new RectangleF(0, 0, pdfDocument.Pages[0].GetClientSize().Width, 50);
        //    PdfPageTemplateElement footer = new PdfPageTemplateElement(bounds);
        //    PdfBrush brush = new PdfSolidBrush(Color.Black);

        //    //Create page number field.
        //    PdfPageNumberField pageNumber = new PdfPageNumberField(font, brush);

        //    //Create page count field.
        //    PdfPageCountField count = new PdfPageCountField(font, brush);

        //    //Add the fields in composite fields.
        //    PdfCompositeField compositeField = new PdfCompositeField(font, brush, "Page {0} of {1}", pageNumber, count);

        //    string printDate = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss", _cultureENInfo);
        //    PdfCompositeField compositePrintDate = new PdfCompositeField(font, brush, string.Format("Printed from PDC/CloseShop      Printed Date/Time : {0}", printDate));

        //    compositeField.Bounds = footer.Bounds;

        //    //Draw the composite field in footer.
        //    compositeField.Draw(footer.Graphics, new PointF(pdfDocument.Pages[0].GetClientSize().Width - (float)63.5, 30));
        //    compositePrintDate.Draw(footer.Graphics, new PointF((float)24, 30));

        //    //Add the footer template at the bottom.
        //    pdfDocument.Template.Bottom = footer;

        //    MemoryStream ms = new MemoryStream();
        //    pdfDocument.Save(ms);
        //    ms.Position = 0;

        //    //Close the document
        //    pdfDocument.Close(true);

        //    // Close file
        //    pdfStream.Dispose();

        //    //Save the document.
        //    return File(ms, "Application/pdf", $"OM_PM_IA_Montoring_Results_{DateTime.Now.ToString("yyy_MM_dd_HHmmss")}.pdf");
        //}

        //public JsonResult Test()
        //{
        //    var modelOm = new ModelOmIndex();
        //    var dto = new DtoOM();
        //    var listMM = dto.GetListMeterMaintenance().ToList();
        //    var listRegionHeader = dto.GetListRegionForTableHeader(listMM);
        //    var result =  dto.GetModelModelMeterMaintenanceLevel(listMM , listRegionHeader);
        //    return Json(result ,  JsonRequestBehavior.AllowGet);
        //}

        private ModelJsonResult<ModelOmIndex> GetData(string pStrYear, string pStrMonth, string[] pArrRegion, string mode)
        {
            ModelJsonResult<ModelOmIndex> result = new ModelJsonResult<ModelOmIndex>();
            try
            {
                // Inint
                ModelOmIndex modelOm = new ModelOmIndex();
                DtoOM dto = new DtoOM();
                int _intMonth = pStrMonth == null ? DateTime.Now.Month : pStrMonth.GetInt();
                int intYear = pStrYear == null ? DateTime.Now.Year : pStrYear.GetInt();
                int intMonth = mode.Equals("yearly") ? 12 : _intMonth;
                var maintenanceLevel = dto.GetListMaintenanceLevelColor();

                // Master Data
                modelOm.ListRegion = dto.GetListRegion()
                    .OrderBy(x => x.REGION_NAME.Length)
                    .ThenBy(x => x.REGION_NAME)
                    .ToList();
                modelOm.Mode = mode;
                modelOm.Year = intYear;
                modelOm.Month = intMonth;
                modelOm.Master = new ModelOMMaster
                {
                    Pipeline = dto.GetPipelineActivity(),
                    MaintenanceLevel = maintenanceLevel
                };


                // Summary
                IEnumerable<ModelMonitoringResults> listPipeline = dto.GetListOMPipelineHistory(intMonth, intYear, pArrRegion, true);
                IEnumerable<ModelMonitoringResults> listGate = dto.GetListOMGateHistory(intMonth, intYear, pArrRegion, true);
                IEnumerable<ModelMonitoringResults> listMeter = dto.GetListOMMeterHistory(intMonth, intYear, pArrRegion, true);

                modelOm.Summary = new ModelOMSummary
                {
                    Pipeline = new ModelOMSummaryPipeline(intMonth, listPipeline),
                    Gate = new ModelOMSummaryMaintenanceLevel(intMonth, intYear, listGate, mode),
                    Meter = new ModelOMSummaryMaintenanceLevel(intMonth, intYear, listMeter, mode),
                };

                modelOm.Completion = new ModelOMCompletion
                {
                    Pipeline = new ModelOMCompletionPipeline(intMonth, listPipeline, mode),
                    Gate = new ModelOMCompletionMaintenanceLevel(intMonth, intYear, listGate, mode),
                    Meter = new ModelOMCompletionMaintenanceLevel(intMonth, intYear, listMeter, mode),
                };

                ModelOMAccumulated accumulated = new ModelOMAccumulated();

                accumulated.getPipeline(intMonth, intYear, listPipeline, mode);
                accumulated.getGate(intMonth, intYear, listGate, mode);
                accumulated.getMeter(intMonth, intYear, listMeter, mode);
                modelOm.Accumulated = accumulated;

                result.SetResultValue(modelOm);
                dto = null;
                GC.Collect();
            }
            catch (Exception ex)
            {
                result.SetException(ex);
            }

            return result;
        }
    }
}
