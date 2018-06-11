using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelPipelineArchive
    {
        public int PIPELINE_ID { get; set; }
        public string RC_NAME { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; }
        public DateTime? UPLOAD_DATE { get; set; }
        public string UPLOAD_BY { get; set; }
        public int REGION_ID { get; set; }
        public int FLAG_ID { get; set; }
        public decimal VELOCITY { get; set; }

    }
}