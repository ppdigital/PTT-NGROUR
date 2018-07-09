using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.ViewModel
{
    public class ModelReportPDF
    {
        public string SearchMode { get; set; }
        public string[] ArrID { get; set; }
        public string Period { get; set; }
        public string Threshold { get; set; }
        public string SearchType { get; set; }
    }
}