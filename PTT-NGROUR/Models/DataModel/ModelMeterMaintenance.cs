using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PTT_NGROUR.ExtentionAndLib;
namespace PTT_NGROUR.Models.DataModel
{
    public class ModelMeterMaintenance
    {

        public ModelMeterMaintenance()
        {

        }

        public ModelMeterMaintenance(System.Data.IDataReader pReader)
        {
            if(pReader == null)
            {
                return;
            }
            this.ML = pReader.GetColumnValue("ML").GetString();
            this.PM_ID = pReader.GetColumnValue("PM_ID").GetInt();
            this.REGION = pReader.GetColumnValue("REGION").GetString();
            this.PLAN = pReader.GetColumnValue("PLAN").GetDecimal();
            this.ACTUAL = pReader.GetColumnValue("ACTUAL").GetDecimal();
            this.MONTH = pReader.GetColumnValue("MONTH").GetInt();
            this.YEAR = pReader.GetColumnValue("YEAR").GetInt();
        }

        public string ML { get; set; }

        public int PM_ID { get; set; }

        public string REGION { get; set; }

        public decimal PLAN { get; set; }

        public decimal ACTUAL { get; set; }

        public int MONTH { get; set; }

        public int YEAR { get; set; }
    }
}