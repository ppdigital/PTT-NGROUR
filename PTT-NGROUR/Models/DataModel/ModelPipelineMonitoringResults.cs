using PTT_NGROUR.ExtentionAndLib;
using System.Collections.Generic;
using System.Data;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelPipelineMonitoringResults
    {
        public ModelPipelineMonitoringResults() { }

        public ModelPipelineMonitoringResults(IDataReader pReader)
        {
            if(pReader == null)
            {
                return;
            }
            this.YEAR = pReader.GetColumnValue("YEAR").GetInt();
            this.MONTH = pReader.GetColumnValue("MONTH").GetInt();
            this.REGION = pReader.GetColumnValue("REGION").GetString();
            this.RC = pReader.GetColumnValue("RC").GetString();
            this.PM_TYPE = pReader.GetColumnValue("PM_TYPE").GetString();
            this.PM_ID = pReader.GetColumnValue("PM").GetString();
            this.PM_NAME_FULL = pReader.GetColumnValue("PM_NAME_FULL").GetString();
            this.PLAN = pReader.GetColumnValue("PLAN").GetDecimal();
            this.ACTUAL = pReader.GetColumnValue("ACTUAL").GetDecimal();
        }

        public string RC { get; set; }
        public string PM_NAME_FULL { get; set; }

        public string PM_TYPE { get; set; }

        public string PM_ID { get; set; }

        public string REGION { get; set; }

        public decimal PLAN { get; set; }

        public decimal ACTUAL { get; set; }

        public int MONTH { get; set; }

        public int YEAR { get; set; }
    }

    public class ModelPipelineMonitoringResultsType
    {
        public string PM_TYPE { get; set; }
        public List<ModelPipelineMonitoringResultsActivity> Activities { get; set; }
        public decimal Percentage { get; set; }
    }

    public class ModelPipelineMonitoringResultsRegion
    {
        public int REGION_ID { get; set; }
        public string REGION { get; set; }
        public List<ModelPipelineMonitoringResultsActivity> Activities { get; set; }
    }

    public class ModelPipelineMonitoringResultsActivity
    {
        public string REGION { get; set; }
        public string RC { get; set; }
        public string PM_ID { get; set; }
        public string PM_NAME { get; set; }
        public string PM_TYPE { get; set; }
        public decimal PLAN { get; set; }
        public decimal ACTUAL { get; set; }
        public decimal PERCENTAGE { get; set; }
    }
}