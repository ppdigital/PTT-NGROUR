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
              NO          = pReader.GetColumnValue("NO").GetInt();
           // ID          = pReader.GetColumnValue("ID").GetString();
            NAME        = pReader.GetColumnValue("NAME").GetString();
            VALUE       = pReader.GetColumnValue("VALUE").GetDecimal();
          //  OBJ_TYPE    = pReader.GetColumnValue("OBJ_TYPE").GetString();
            FLAG        = pReader.GetColumnValue("FLAG").GetInt();            
            THRESHOLD   = pReader.GetColumnValue("THRESHOLD").GetString();
            TYPE        = pReader.GetColumnValue("TYPE").GetString();
            REGION      = pReader.GetColumnValue("REGION").GetInt();
            LICENSE     = pReader.GetColumnValue("LICENSE").GetInt();
            STATUS      = pReader.GetColumnValue("STATUS").GetString();
            MONTH       = pReader.GetColumnValue("MONTH").GetInt();
            YEAR        = pReader.GetColumnValue("YEAR").GetInt();
            CUST_NAME   = pReader.GetColumnValue("CUST_NAME").GetString();
        }

        public int NO { get; set; }
       // public string ID { get; set; }
        public string NAME { get; set; }
        public decimal VALUE { get; set; }
        public string CUST_NAME { get; set; }
       // public string OBJ_TYPE { get; set; }
        public int FLAG { get; set; } 
        //public string COLOR { get; set; }
        public string THRESHOLD { get; set; }
        public string TYPE { get; set; }
        public int REGION { get; set; }
        public int LICENSE { get; set; }
        public string STATUS { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; } 

    }

}
