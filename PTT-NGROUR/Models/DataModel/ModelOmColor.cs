using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PTT_NGROUR.ExtentionAndLib;
namespace PTT_NGROUR.Models.DataModel
{
    public class ModelOmColor
    {
        public ModelOmColor()
        {

        }
        public ModelOmColor(System.Data.IDataReader pReader)
        {
            if(pReader == null)
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