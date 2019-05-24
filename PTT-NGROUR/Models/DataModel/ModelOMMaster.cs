using PTT_NGROUR.ExtentionAndLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelOMMaster
    {
        public ModelOMMaster() { }
        public List<ModelOMMasterPipeline> Pipeline { get; set; }
        public List<ModelOMMasterMaintenanceLevel> MaintenanceLevel { get; set; }

        public class ModelOMMasterPipeline
        {
            public ModelOMMasterPipeline(IDataReader pReader)
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
                this.PIPELINE_HEX = pReader.GetColumnValue("PIPELINE_HEX").GetString();
            }

            public string PIPELINE_ACT_ID { get; set; }
            public string PM_NAME_ABV { get; set; }
            public string INTERVAL { get; set; }
            public string PM_ID { get; set; }
            public string PM_NAME_FULL { get; set; }
            public string PM_TYPE { get; set; }
            public string PM_SYSTEM { get; set; }
            public string PIPELINE_HEX { get; set; }
        }

        public class ModelOMMasterMaintenanceLevel
        {
            public ModelOMMasterMaintenanceLevel(System.Data.IDataReader pReader)
            {
                if (pReader == null)
                {
                    return;
                }
                this.ML_ID = pReader["ML_ID"].GetString();
                this.ML_HEX = pReader["ML_HEX"].GetString();
            }

            public string ML_ID { get; set; }
            public string ML_HEX { get; set; }
        }
    }
}