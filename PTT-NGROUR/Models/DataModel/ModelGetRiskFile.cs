using PTT_NGROUR.ExtentionAndLib;
using System;
using System.Data;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelGetRiskFile
    {
        public ModelGetRiskFile()
        {

        }

        public ModelGetRiskFile(IDataReader pReader)
        {
            if (pReader == null)
            {
                return;
            }
            ID = pReader.GetColumnValue("ID").GetInt();
            RC_NAME = pReader.GetColumnValue("RC_NAME").GetString();
            YEAR = pReader.GetColumnValue("YEAR").GetInt();
            FILE_NAME = pReader.GetColumnValue("FILE_NAME").GetString();
            UPLOADED_AT = pReader.GetColumnValue("UPLOADED_AT").GetDate();
            UPLOADED_BY = pReader.GetColumnValue("UPLOADED_BY").GetString();
        }

        public int ID { get; set; }
        public string RC_NAME { get; set; }
        public int YEAR { get; set; }
        public string FILE_NAME { get; set; }
        public DateTime? UPLOADED_AT { get; set; }
        public string UPLOADED_BY { get; set; }
    }
}