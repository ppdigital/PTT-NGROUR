using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelRiskReport
    {
        public int REGION { get; set; }
        public string LICENSE { get; set; }
        public string RC { get; set; }
        public double INTERNAL_CORROSION { get; set; }
        public double EXTERNAL_CORROSION { get; set; }
        public double THIRD_PARTY_INTERFERENCE { get; set; }
        public double LOSS_OF_GROUND_SUPPORT { get; set; }
        public double RISK_SCORE { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; }
    }
}