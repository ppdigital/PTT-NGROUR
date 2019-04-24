using PTT_NGROUR.ExtentionAndLib;
using System.Data;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelGetOM
    {
        public ModelGetOM()
        {

        }

        public ModelGetOM(IDataReader pReader)
        {
            if (pReader == null)
            {
                return;
            }
            REGION = pReader.GetColumnValue("REGION").GetInt();
            REGION_NAME = pReader.GetColumnValue("REGION_NAME").GetString();
            NAME = pReader.GetColumnValue("NAME").GetString();
            LICENSE_NO = pReader.GetColumnValue("LICENSE_NO").GetInt();
            PM_TYPE = pReader.GetColumnValue("PM_TYPE").GetString();
            PM = pReader.GetColumnValue("PM").GetString();
            PM_NAME_FULL = pReader.GetColumnValue("PM_NAME_FULL").GetString();
            PLAN = pReader.GetColumnValue("PLAN").GetDecimal();
            ACTUAL = pReader.GetColumnValue("ACTUAL").GetDecimal();
            TYPE = pReader.GetColumnValue("TYPE").GetString();
            MONTH = pReader.GetColumnValue("MONTH").GetInt();
            YEAR = pReader.GetColumnValue("YEAR").GetInt();
        }

        public int REGION { get; set; }
        public string REGION_NAME { get; set; }
        public string NAME { get; set; }
        public int LICENSE_NO { get; set; }
        public string PM_TYPE { get; set; }
        public string PM { get; set; }
        public string PM_NAME_FULL { get; set; }
        public decimal PLAN { get; set; }
        public decimal ACTUAL { get; set; }
        public string TYPE { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; }
    }
}