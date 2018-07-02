using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PTT_NGROUR.ExtentionAndLib;
namespace PTT_NGROUR.Models.DataModel
{
    public class ModelGateStation
    {
        public ModelGateStation()
        {

        }

        public ModelGateStation(System.Data.IDataReader pReader)
        {
            if(pReader == null)
            {
                return;
            }
            this.OBJECTID       = pReader.GetColumnValue("OBJECTID").GetInt();
            this.RECNUM         = pReader.GetColumnValue("RECNUM").GetString();
            this.NAME           = pReader.GetColumnValue("NAME").GetString();
            this.ALTITUDE       = pReader.GetColumnValue("ALTITUDE").GetString();
            this.LONGNAME       = pReader.GetColumnValue("LONGNAME").GetString();
            this.FULLNAME       = pReader.GetColumnValue("FULLNAME").GetString();
            this.STATION_ID     = pReader.GetColumnValue("STATION_ID").GetString();
            this.ABBREV_NAME    = pReader.GetColumnValue("ABBREV_NAME").GetString();
            this.DESCRIPTION    = pReader.GetColumnValue("DESCRIPTION").GetString();
            this.REGION         = pReader.GetColumnValue("REGION").GetString();
            this.LICENSE        = pReader.GetColumnValue("LICENSE").GetString();
        }

        public int OBJECTID { get; set; }
        public string RECNUM { get; set; }
        public string NAME { get; set; }
        public string ALTITUDE { get; set; }
        public string LONGNAME { get; set; }
        public string FULLNAME { get; set; }
        public string STATION_ID { get; set; }
        public string ABBREV_NAME { get; set; }
        public string DESCRIPTION { get; set; }
        public string REGION { get; set; }
        public string LICENSE { get; set; }

    }
}