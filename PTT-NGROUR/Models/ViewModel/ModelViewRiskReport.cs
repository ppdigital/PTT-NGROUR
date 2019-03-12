using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.ViewModel
{
    public class ModelViewRiskReport
    {
        public string Type { get; set; }
        public string Month { get; set; }
        public string Year { get; set; }
        public List<string> Lists { get; set; }
    }
}