using PTT_NGROUR.ExtentionAndLib;
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
            this.RC = pReader.GetColumnValue("CP_RC").GetString();
            this.PM_ID = pReader.GetColumnValue("CP_PM").GetString();
            this.PM_NAME_FULL = pReader.GetColumnValue("CP_PM_NAME_FULL").GetString();
            this.PM_TYPE = pReader.GetColumnValue("CP_PM_TYPE").GetString();
            this.REGION = pReader.GetColumnValue("CP_REGION_API").GetString();
            this.PLAN = pReader.GetColumnValue("CP_PLAN").GetDecimal();
            this.ACTUAL = pReader.GetColumnValue("CP_ACTUAL").GetDecimal();
            this.MONTH = pReader.GetColumnValue("CP_MONTH").GetInt();
            this.YEAR = pReader.GetColumnValue("CP_YEAR").GetInt();
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

    public class ModelPipelineMonitoringResultsSummary
    {
        public string PM_ID { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; }
        public string PM_NAME_FULL { get; set; }
        public string PM_TYPE { get; set; }
        public decimal PLAN { get; set; }
        public decimal ACTUAL { get; set; }
        public decimal SUM_PLAN { get; set; }
        public decimal SUM_ACTUAL { get; set; }
        public decimal SUM_MAJOR_PLAN { get; set; }
        public decimal SUM_MAJOR_ACTUAL { get; set; }
        public decimal SUM_MINOR_PLAN { get; set; }
        public decimal SUM_MINOR_ACTUAL { get; set; }
    }
}