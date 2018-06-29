using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PTT_NGROUR.Models.DataModel;
namespace PTT_NGROUR.Models.ViewModel
{
    public class ModelOmIndex
    {
        public class ModelBarGraph
        {
            public class ModelML
            {
                public string Label { get; set; }
                public List<int> ListData { get; set; }
                public string HexColor { get; set; }
            }
            public List<string> ListLabel { get; set; }
            public List<ModelML> ListML { get; set; }            

            public ModelBarGraph() { }

            public ModelBarGraph(IEnumerable<ModelMeterMaintenance> pListModelMeterMaintenance)
            {
                if(pListModelMeterMaintenance == null || !pListModelMeterMaintenance.Any())
                {
                    return;
                }
                this.ListLabel = pListModelMeterMaintenance
                    .Select(x =>  x.REGION).Distinct().OrderBy(x => x)
                    .Select(x => "Region " + x.ToString()).Union(new string[] {"Over All" }).ToList();

                var listRegion = pListModelMeterMaintenance.Select(x => x.REGION).Distinct().OrderBy(x=> x).ToList();

                this.ListML = new List<ModelML>();

                var listML = pListModelMeterMaintenance.Select(x => x.ML).Distinct().OrderBy(x => x).ToList();
                foreach (var itemML in listML)
                {
                    var ml = new ModelML();
                    ml.Label = itemML;
                        
                    ml.ListData = new List<int>();
                    var listML1 = pListModelMeterMaintenance.Where(x => x.ML == itemML).ToList();
                    foreach (var itemRegion in listRegion)
                    {
                        var listML2 = listML1.Where(x =>  x.REGION == itemRegion).ToList();
                        var sumPlan = listML2.Select(x => x.PLAN).Sum();
                        if (decimal.Zero.Equals(sumPlan))
                        {
                            ml.ListData.Add(0);
                        }
                        else
                        {
                            var sumAc = listML2.Select(x=> x.ACTUAL).Sum();
                            var intData = Convert.ToInt32(sumAc * 100 / sumPlan);
                            ml.ListData.Add(intData);
                        }                            
                    }
                    var sumPlanAll = listML1.Select(x => x.PLAN).Sum();
                    if (decimal.Zero.Equals(sumPlanAll))
                    {
                        ml.ListData.Add(0);
                    }
                    else
                    {
                        var sumAcAll = listML1.Select(x => x.ACTUAL).Sum();
                        var intData = Convert.ToInt32(sumAcAll * 100 / sumPlanAll);
                        ml.ListData.Add(intData);
                    }
                    this.ListML.Add(ml);
                }
            }

        }
        public IEnumerable<ModelMeterMaintenance> ListMeterMaintenance { get; set; }

        public IEnumerable<ModelOmColor> ListOmColor { get; set; }
        public ModelBarGraph BarGraph { get; set; }
    }
}