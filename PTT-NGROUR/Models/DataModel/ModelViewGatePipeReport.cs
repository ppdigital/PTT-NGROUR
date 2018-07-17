using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using PTT_NGROUR.ExtentionAndLib;
namespace PTT_NGROUR.Models.DataModel
{
    public class ModelViewGatePipeReport
    {
        public enum GatePipeReportType
        {
            NONE , PIPELINE, GATESTATION  
        }

        public enum GatePipeReportThreshold
        {
            NONE , OK, Warning , Alert 
        }

        public ModelViewGatePipeReport() { }

        public ModelViewGatePipeReport(IDataReader pReader)
        {
            if (pReader == null) return;
            NO = pReader.GetColumnValue("NO").GetInt();
            NAME = pReader.GetColumnValue("NAME").GetString();
            REGION = pReader.GetColumnValue("REGION").GetInt();
            LICENSE = pReader.GetColumnValue("LICENSE").GetInt();
            VALUE = pReader.GetColumnValue("VALUE").GetDecimal();
            COLOR = pReader.GetColumnValue("COLOR").GetString();
            THRESHOLD = pReader.GetColumnValue("THRESHOLD").GetString();
            MONTH = pReader.GetColumnValue("MONTH").GetInt();
            YEAR = pReader.GetColumnValue("YEAR").GetInt();
            TYPE = pReader.GetColumnValue("TYPE").GetString();
        }

        public int NO { get; set; }
        public string NAME { get; set; }
        public int REGION { get; set; }
        public int LICENSE { get; set; }
        public decimal VALUE { get; set; }
        public string COLOR { get; set; }
        public string THRESHOLD { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; }
        public string TYPE { get; set; }


    }
}