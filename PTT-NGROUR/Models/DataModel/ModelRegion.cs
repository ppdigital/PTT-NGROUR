using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PTT_NGROUR.ExtentionAndLib;
using System.Data;

namespace PTT_NGROUR.Models.DataModel
{

    public class ModelRegion
    {
        public ModelRegion()
        {

        }

        public ModelRegion(IDataReader pReader)
        {
            if (pReader == null) return;
            REGION_ID = pReader.GetColumnValue("REGION_ID").GetInt();
            REGION_NAME = pReader.GetColumnValue("REGION_NAME").GetString();
            REGION_NAME_EN = pReader.GetColumnValue("REGION_NAME_EN").GetString();
            REGION_NAME_TH = pReader.GetColumnValue("REGION_NAME_TH").GetString();
        }

        public int REGION_ID { get; set; }
        public string REGION_NAME { get; set; }
        public string REGION_NAME_EN { get; set; }
        public string REGION_NAME_TH { get; set; }
    }
}