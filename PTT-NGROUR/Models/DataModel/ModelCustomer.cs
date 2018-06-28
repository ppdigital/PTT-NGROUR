using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelVIEW_SHIPTO_SOLDTO
    {
        public int count { get; set; }
        public string SHIP_TO { get; set; }
        public string SHIP_TO_NAME { get; set; }
        public string SHIP_TO_SNAME { get; set; }
        public string SHIP_TO_ADDRESS { get; set; }
        public int SHIP_TO_FLAG_SHAPE { get; set; }
        public string SOLD_TO { get; set; }
        public string SOLD_TO_NAME { get; set; }
        public string SOLD_TO_ADDRESS { get; set; }
        public int SOLD_TO_FLAG_SHAPE { get; set; }
        public string IE_NAME { get; set; }
        public string CONTRACT_TYPE { get; set; }
        public string REGION { get; set; }
        public string STATUS { get; set; } 

    }

    public class ModelNGR_CUST_ALL
    {
        public string CUST_NAME { get; set; }
        public string CUST_SNAME { get; set; }
        public string SHIP_TO { get; set; }
        public string CUST_NAME_EN { get; set; }
        public string SHIP_TO_ADDRESS { get; set; }
        public string PLANT_NAME { get; set; }
        public string SALES_DISTRICT { get; set; }
        public string CONTRACT_TYPE { get; set; }
        public string IE_NAME { get; set; }
        public string REGION { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }
        public string OBJECTID { get; set; }
        public string SOLD_TO { get; set; }
        public int FLAG_SHAPE { get; set; }
        public string SOLD_TO_NAME { get; set; }
        public string SOLD_TO_ADDRESS { get; set; }
        public string STATUS { get; set; }
  
    }
    public class ModelNGR_CUSTOMER
    {
        public string CUST_NAME { get; set; }
        public string CUST_SNAME { get; set; }
        public string SHIP_TO { get; set; }
        public string CUST_NAME_EN { get; set; }
        public string SHIP_TO_ADDRESS { get; set; }
        public string SOLD_TO { get; set; }
        public string PLANT_NAME { get; set; }
        public string SALES_DISTRICT { get; set; }
        public string CONTRACT_TYPE { get; set; }
        public string IE_NAME { get; set; }
        public string REGION { get; set; }
        public string STATUS { get; set; }
        public int FLAG_SHAPE { get; set; }
        public string UPDATED_BY { get; set; }
        public DateTime? UPDATED_DATE { get; set; }


    }

    public class ModelNGR_CUSTOMER_OFFICE
    {
        public string OBJECTID { get; set; }
        public string SOLD_TO { get; set; }
        public int FLAG_SHAPE { get; set; }
        public string SOLD_TO_NAME { get; set; }
        public string SOLD_TO_ADDRESS { get; set; }
        public string STATUS { get; set; }
      

    }

    public class ModelVIEW_METER
    {
        public int countMeter { get; set; }
        public string CUST_NAME { get; set; }
        public string LICENSE_CODE { get; set; }
        public string METER_NAME { get; set; }
        public string METER_NUMBER { get; set; }
        public string METER_TYPE { get; set; }
        public string REGION { get; set; }
        public string SHIP_TO { get; set; }
        public string SOLD_TO { get; set; }
        public string SOLD_TO_NAME { get; set; }
        public string STATUS { get; set; }

    }

    public class ModelMETER
    {
        public int OBJECT_ID_T { get; set; }
        public int countMeter_T { get; set; }
        public string CUST_NAME_T { get; set; }
        public string LICENSE_CODE_T { get; set; }
        public string METER_NAME_T { get; set; }
        public string METER_NUMBER_T { get; set; }
        public string METER_TYPE_T { get; set; }
        public string REGION_T { get; set; }
        public string SHIP_TO_T { get; set; }
        public string SOLD_TO_T { get; set; }
        public string SOLD_TO_NAME_T { get; set; }
        public string STATUS_T { get; set; }
        public DateTime? COMMDATE_T { get; set; }

    }

}
