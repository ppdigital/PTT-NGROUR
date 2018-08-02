using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelThresholdTable
      {
       // public int GATE_THRESHOLD_ID { get; set; }
     
        public decimal MINVAL { get; set; }
        public decimal MAXVAL { get; set; }
        public string THRESHOLD_NAME { get; set; }
    }

   
}
