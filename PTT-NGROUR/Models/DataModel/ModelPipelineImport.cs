using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelPipelineImport
    {
        public int PIPELINE_ID { get; set; }

        public string RC_NAME { get; set; }

        public decimal FLOW { get; set; }

        public decimal DIAMETER { get; set; }

        public decimal LENGTH { get; set; }

        public decimal EFFICIENCY { get; set; }

        public decimal ROUGHNESS { get; set; }

        public decimal LOAD { get; set; }

        public decimal VELOCITY { get; set; }

        public decimal OUTSIDE_DIAMETER { get; set; }

        public decimal WALL_THICKNESS { get; set; }

        public string SERVICE_STATE { get; set; }

        public int MONTH { get; set; }

        public int YEAR { get; set; }

        public DateTime? UPLOAD_DATE { get; set; }

        public string UPLOAD_BY { get; set; }

        public int REGION_ID { get; set; }

        


    }
}