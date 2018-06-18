using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.ViewModel
{
    public class ModelInsertExcelData
    {
        public bool success { get; set; }

        public string responseText { get; set; }

        public DataModel.ModelPipelineImport[] ListUnSuccessPipeLine { get; set; }

        public DataModel.ModelPipelineImport[] ListSuccessPipeLine { get; set; }

        //public DataModel.ModelPipelineImport[] ListDuplicatePipeLine { get; set; }

        public DataModel.ModelGateStationImport[] ListSuccessGateStation { get; set; }

        public DataModel.ModelGateStationImport[] ListUnSuccessGateStation { get; set; }

        //public DataModel.ModelGateStationImport[] ListDuplicateGateStation { get; set; }

    }
}