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
            this.ML = pReader["ML"].GetString();
            this.PM_INTERVAL = pReader["PM_INTERVAL"].GetString();
            this.REGION = pReader["REGION"].GetInt();
            this.PLAN = pReader["PLAN"].GetDecimal();
            this.ACTUAL = pReader["ACTUAL"].GetDecimal();
            this.MONTH = pReader["MONTH"].GetInt();
            this.YEAR = pReader["YEAR"].GetInt();
        }

        public string ML { get; set; }

        public string PM_INTERVAL { get; set; }

        public int REGION { get; set; }

        public decimal PLAN { get; set; }

        public decimal ACTUAL { get; set; }

        public int MONTH { get; set; }

        public int YEAR { get; set; }
    }
}