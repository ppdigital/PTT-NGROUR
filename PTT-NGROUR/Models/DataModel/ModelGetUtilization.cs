using PTT_NGROUR.ExtentionAndLib;
using System.Data;
namespace PTT_NGROUR.Models.DataModel
{
    public class ModelGetUtilization
    {
        public ModelGetUtilization()
        {

        }

        public ModelGetUtilization(IDataReader pReader)
        {
            if(pReader == null)
            {
                return;
            }
            NO              = pReader.GetColumnValue("NO").GetInt();
            //ID              = pReader.GetColumnValue("ID").GetString();
            NAME            = pReader.GetColumnValue("NAME").GetString();
            VALUE           = pReader.GetColumnValue("VALUE").GetDecimal();
            //OBJ_TYPE        = pReader.GetColumnValue("OBJ_TYPE").GetString();
            FLAG            = pReader.GetColumnValue("FLAG").GetInt();            
            THRESHOLD       = pReader.GetColumnValue("THRESHOLD").GetString();
            TYPE            = pReader.GetColumnValue("TYPE").GetString();
            REGION          = pReader.GetColumnValue("REGION").GetInt();
            REGION_NAME     = pReader.GetColumnValue("REGION_NAME").GetString();
            LICENSE         = pReader.GetColumnValue("LICENSE").GetInt();
            LICENSE_NAME    = pReader.GetColumnValue("LICENSE_NAME").GetString();
            STATUS          = pReader.GetColumnValue("STATUS").GetString();
            MONTH           = pReader.GetColumnValue("MONTH").GetInt();
            YEAR            = pReader.GetColumnValue("YEAR").GetInt();
            CUST_NAME       = pReader.GetColumnValue("CUST_NAME").GetString();
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
        public string REGION_NAME { get; set; }
        //public string LICENSES { get; set; }
        public int LICENSE { get; set; }
        public string LICENSE_NAME { get; set; }
        public string STATUS { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; } 

    }

}
