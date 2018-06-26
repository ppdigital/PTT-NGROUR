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

    }
}