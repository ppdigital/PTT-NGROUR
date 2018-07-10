using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.ViewModel
{
    public class ModelUtilizationReportPdfInput
    {
        public string SearchMode { get; set; } //"region" : "license"
        public string[] ArrID { get; set; }
        public string Period { get; set; } // 6/2018
        public string Threshold { get; set; } // Pass , Warning , Alert 
        public string SearchType { get; set; } // Pipeline , GateStation
    }
}