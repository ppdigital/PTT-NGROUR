using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.ViewModel
{
    public class ModelUtilization
    {
        public List<DataModel.ModelIndustryMaster> ListIndustry { get; set; }
        public List<DataModel.ModelLicenseMaster> ListLicense { get; set; }
        public List<DataModel.ModelRegion> ListRegion { get; set; }
        public List<DataModel.ModelThresholdTable> ListThresholdGate { get; set; }
        public List<DataModel.ModelThresholdTable> ListThresholdPipe { get; set; }
    }


}