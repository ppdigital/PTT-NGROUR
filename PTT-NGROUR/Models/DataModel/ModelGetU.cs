using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using PTT_NGROUR.ExtentionAndLib;
namespace PTT_NGROUR.Models.DataModel
{
    public class ModelGetU
    {
        public ModelGetU()
        {

        }

        public ModelGetU(IDataReader pReader)
        {
            if(pReader == null)
            {
                return;
            }
            NO          = pReader.GetColumnValue("NO").GetString();
            ID          = pReader.GetColumnValue("ID").GetString();
            NAME        = pReader.GetColumnValue("NAME").GetString();
            VALUE       = pReader.GetColumnValue("VALUE").GetString();
            OBJ_TYPE    = pReader.GetColumnValue("OBJ_TYPE").GetString();
            FLAG        = pReader.GetColumnValue("FLAG").GetString();            
            THRESHOLD   = pReader.GetColumnValue("THRESHOLD").GetString();
            TYPE        = pReader.GetColumnValue("TYPE").GetString();
            REGION      = pReader.GetColumnValue("REGION").GetString();
            LICENSE     = pReader.GetColumnValue("LICENSE").GetString();
            STATUS      = pReader.GetColumnValue("STATUS").GetString();
            MONTH       = pReader.GetColumnValue("MONTH").GetString();
            YEAR        = pReader.GetColumnValue("YEAR").GetString();
        }

        public string NO { get; set; }
        public string ID { get; set; }
        public string NAME { get; set; }
        public string VALUE { get; set; }
        public string OBJ_TYPE { get; set; }
        public string FLAG { get; set; } 
        //public string COLOR { get; set; }
        public string THRESHOLD { get; set; }
        public string TYPE { get; set; }
        public string REGION { get; set; }
        public string LICENSE { get; set; }
        public string STATUS { get; set; }
        public string MONTH { get; set; }
        public string YEAR { get; set; } 

    }

}
