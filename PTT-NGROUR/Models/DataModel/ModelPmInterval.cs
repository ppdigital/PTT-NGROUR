using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PTT_NGROUR.ExtentionAndLib;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelPmInterval
    {
        public ModelPmInterval()
        {

        }

        public ModelPmInterval(System.Data.IDataReader pReader)
        {
            if(pReader == null)
            {
                return;
            }
            PM_ID = pReader.GetColumnValue("PM_ID").GetInt();
            INTERVAL = pReader.GetColumnValue("INTERVAL").GetString();
        }

        public int PM_ID { get; set; }
        public string INTERVAL { get; set; }
    }
}