using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static PTT_NGROUR.DTO.DtoRisk;

namespace PTT_NGROUR.Models.ViewModel
{
    public class ModelRisk
    {
        public List<DataModel.ModelRiskType> ListRiskType { get; set; }
        public List<DataModel.ModelLicenseMaster> ListLicense { get; set; }
        public List<DataModel.ModelRegion> ListRegion { get; set; }
    }
}