using PTT_NGROUR.Models.DataModel;
using System.Collections.Generic;

namespace PTT_NGROUR.Models.ViewModel
{
    public class ModelOmIndex
    {
        public IEnumerable<ModelRegion> ListRegion { get; set; }
        public string Mode { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public ModelOMMaster Master { get; set; }
        public ModelOMSummary Summary { get; set; }
        public ModelOMCompletion Completion { get; set; }
        public ModelOMAccumulated Accumulated { get; set; }
    }
}