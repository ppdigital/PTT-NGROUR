using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PTT_NGROUR.ExtentionAndLib;
namespace PTT_NGROUR.Models.DataModel
{
    public class ModelGateStationImport 
    {
        public ModelGateStationImport() { }

        public ModelGateStationImport(System.Data.IDataReader pReader)
        {
            if (pReader == null)
            {
                return;
            }
            this.FLOW = pReader["FLOW"].GetDecimal();
            this.GATE_ID = pReader["GATE_ID"].GetInt();
            this.GATE_NAME = pReader["GATE_NAME"].GetString();
            this.MONTH = pReader["MONTH"].GetInt();
            this.PRESSURE = pReader["PRESSURE"].GetDecimal();
            this.REGION = pReader["REGION"].GetString();
            this.UPLOAD_BY = pReader["UPLOAD_BY"].GetString();
            this.UPLOAD_DATE = pReader["UPLOAD_DATE"].GetDate();
            this.YEAR = pReader["YEAR"].GetInt();            
        }

        public int GATE_ID { get; set; }
        public string GATE_NAME { get; set; }        
        public decimal? FLOW { get; set; }

        public decimal? PRESSURE { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; }
        public DateTime? UPLOAD_DATE { get; set; }
        public string UPLOAD_BY { get; set; }
        public string REGION { get; set; }

        public ModelGateStationImport Clone()
        {
            var result = new ModelGateStationImport()
            {
                FLOW = this.FLOW,
                GATE_ID = this.GATE_ID,
                GATE_NAME = this.GATE_NAME,
                MONTH = this.MONTH,
                PRESSURE = this.PRESSURE,
                REGION = this.REGION,
                UPLOAD_BY = this.UPLOAD_BY,
                UPLOAD_DATE = this.UPLOAD_DATE,
                YEAR =this.YEAR                
            };
            return result;
            
        }
    }
}