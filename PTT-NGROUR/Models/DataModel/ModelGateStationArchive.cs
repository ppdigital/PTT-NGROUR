using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelGateStationArchive
    {
        public ModelGateStationArchive()
        {

        }

        public ModelGateStationArchive(ModelGateStationImport pModelGateImport)
        {
            this.FLOW = pModelGateImport.FLOW;
            this.GATE_NAME = pModelGateImport.GATE_NAME;
            this.MONTH = pModelGateImport.MONTH;
            this.REGION = pModelGateImport.REGION;
            this.UPLOAD_BY = pModelGateImport.UPLOAD_BY;
            this.YEAR = pModelGateImport.YEAR;
            this.GATE_ID = pModelGateImport.GATE_ID;
            this.UPLOAD_DATE = pModelGateImport.UPLOAD_DATE;            
        }

        public int GATE_ID { get; set; }
        public string GATE_NAME { get; set; }
        
        public decimal FLOW { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; }
        public DateTime? UPLOAD_DATE { get; set; }
        public string UPLOAD_BY { get; set; }
        public string REGION { get; set; }
        public int FLAG_ID { get; set; }
    }
}