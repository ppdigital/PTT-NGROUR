using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelRiskReport
    {
        public int ORDERID { get; set; }
        public int REGION { get; set; }
        public int? LICENSE { get; set; }
        public string RC { get; set; }
        public int INTERNAL_CORROSION { get; set; }
        public int EXTERNAL_CORROSION { get; set; }
        public int THIRD_PARTY_INTERFERENCE { get; set; }
        public int LOSS_OF_GROUND_SUPPORT { get; set; }
        public int C1 { get; set; }
        public int C2 { get; set; }
        public int RISK_SCORE { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; }
    }
}