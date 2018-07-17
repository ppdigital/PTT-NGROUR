using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PTT_NGROUR.Models.DataModel;
using PTT_NGROUR.ExtentionAndLib;

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
        
        public class ModelAccGraph
        {
            private string[] arrMonthName = new[] { string.Empty, "Jan", "Feb", "Mar", "April", "May", "June", "July", "Aug", "Sep", "Oct", "Nov", "Dec" };
            public string ML { get; set; }
            public int[] Actual { get; set; }
            public int[] Plan { get; set; }
            public int[] AccActual { get; set; }
            public int[] AccPlan { get; set; }
            public string[] MonthName { get; set; }

            public ModelAccGraph()
            {

            }

            public ModelAccGraph(string pStrML , IEnumerable<ModelMeterMaintenance> pListModelMeterMaintenance)
            {
                if(string.IsNullOrEmpty(pStrML) || pListModelMeterMaintenance == null || !pListModelMeterMaintenance.Any())
                {
                    return;
                }
                this.ML = pStrML;
                var listByMl = pListModelMeterMaintenance.Where(x => x.ML == pStrML).ToList();
                var listMonth = listByMl.Select(x => x.MONTH).Distinct().OrderBy(x => x).ToList();
                this.MonthName = listMonth.Select(x => arrMonthName[x]).ToArray();
                this.Actual = listByMl.GroupBy(x => x.MONTH).OrderBy(x => x.Key).Select(x => x.Sum(y => y.ACTUAL).GetInt()).ToArray();
                this.Plan = listByMl.GroupBy(x => x.MONTH).OrderBy(x => x.Key).Select(x => x.Sum(y => y.PLAN).GetInt()).ToArray();
                this.AccActual = listMonth.Select(x => listByMl.Where(y => y.MONTH <= x).Sum(z => z.ACTUAL).GetInt()).ToArray();
                this.AccPlan = listMonth.Select(x => listByMl.Where(y => y.MONTH <= x).Sum(z => z.ACTUAL + z.PLAN).GetInt()).ToArray();
            }
        }

        public IEnumerable<ModelMeterMaintenance> ListMeterMaintenance { get; set; }

        public IEnumerable<ModelOmColor> ListOmColor { get; set; }
        public ModelBarGraph BarGraph { get; set; }

        public IEnumerable<ModelRegion> ListRegion { get; set; }

        public IEnumerable<ModelAccGraph> ListAccGraph { get; set; }
    }
}