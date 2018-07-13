using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.ViewModel
{
    public class Customer
    {
        public List<DataModel.ModelVIEW_SHIPTO_SOLDTO> ListViewShipToSoldTo { get; set; }
        public List<DataModel.ModelNGR_CUST_ALL> ListCustAll { get; set; }
        public List<DataModel.ModelVIEW_METER> ListViewMeter { get; set; }
        public List<DataModel.ModelMETER> ListMeter { get; set; }
        public List<DataModel.ModeVIEW_CUSTOMER> ListViewCustomer { get; set; }
        public List<DataModel.ModeLICENSE_MASTER> ListLicenseMaster { get; set; }
        public List<DataModel.ModelMETER_TYPE> ListMeterType { get; set; }
        public List<DataModel.ModelSTATUS> ListStatus { get; set; }
    }
}