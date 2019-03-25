using PTT_NGROUR.ExtentionAndLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelPipelineActivity
    {
        public ModelPipelineActivity() { }

        public ModelPipelineActivity(IDataReader pReader)
        {
            if (pReader == null)
            {
                return;
            }
            this.PIPELINE_ACT_ID = pReader.GetColumnValue("PIPELINE_ACT_ID").GetString();
            this.PM_NAME_ABV = pReader.GetColumnValue("PM_NAME_ABV").GetString();
            this.INTERVAL = pReader.GetColumnValue("INTERVAL").GetString();
            this.PM_ID = pReader.GetColumnValue("PM_ID").GetString();
            this.PM_NAME_FULL = pReader.GetColumnValue("PM_NAME_FULL").GetString();
            this.PM_TYPE = pReader.GetColumnValue("PM_TYPE").GetString();
            this.PM_NAME_FULL = pReader.GetColumnValue("PM_NAME_FULL").GetString();
            this.PM_SYSTEM = pReader.GetColumnValue("PM_SYSTEM").GetString();
        }
        public string PIPELINE_ACT_ID { get; set; }
        public string PM_NAME_ABV { get; set; }
        public string INTERVAL { get; set; }
        public string PM_ID { get; set; }
        public string PM_NAME_FULL { get; set; }
        public string PM_TYPE { get; set; }
        public string PM_SYSTEM { get; set; }
    }
}