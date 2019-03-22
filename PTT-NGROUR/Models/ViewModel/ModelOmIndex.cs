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
        public class ModelPipeline
        {
            public ModelPipeline(int month, IEnumerable<ModelPipelineMonitoringResults> listPipeline)
            {
                Summary = listPipeline.GroupBy(x => x.PM_ID, (pm_id, o) => new
                {
                    PM_ID = pm_id,
                    List = o.OrderByDescending(list => list.YEAR).ThenByDescending(list => list.MONTH)
                })
                .Select(x => new ModelPipelineMonitoringResultsSummary
                {
                    PM_ID = x.PM_ID,
                    PM_NAME_FULL = x.List.FirstOrDefault().PM_NAME_FULL,
                    PM_TYPE = x.List.FirstOrDefault().PM_TYPE,
                    PLAN = x.List.Where(o => o.MONTH == month).Sum(o => o.PLAN),
                    ACTUAL = x.List.Where(o => o.MONTH == month).Sum(o => o.PLAN),
                    SUM_PLAN = x.List.Sum(o => o.PLAN),
                    SUM_ACTUAL = x.List.Sum(o => o.ACTUAL),
                    SUM_MAJOR_PLAN = x.List.Where(o => o.PM_TYPE.ToLower().Equals("major")).Sum(o => o.PLAN),
                    SUM_MAJOR_ACTUAL = x.List.Where(o => o.PM_TYPE.ToLower().Equals("major")).Sum(o => o.ACTUAL),
                    SUM_MINOR_PLAN = x.List.Where(o => o.PM_TYPE.ToLower().Equals("minor")).Sum(o => o.PLAN),
                    SUM_MINOR_ACTUAL = x.List.Where(o => o.PM_TYPE.ToLower().Equals("minor")).Sum(o => o.ACTUAL),
                }).ToList();

                var xxx = listPipeline.OrderByDescending(x => x.MONTH).FirstOrDefault();
            }

            public List<ModelPipelineMonitoringResultsSummary> Summary { get; set; }
            List<ModelPipelineMonitoringResults> Results { get; set; }
        }

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
            private string[] arrMonthName = new[] { string.Empty, "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
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
                this.AccPlan = listMonth.Select(x => listByMl.Where(y => y.MONTH <= x).Sum(z => z.PLAN).GetInt()).ToArray();
                
                //one graph
                //for (int i = 0; i < Actual.Length; i++)
                //{
                //    Actual[i] -= Plan[i];
                //}
            }
        }
        public class ModelMeterMaintenanceLevel
        {
            public class ModelPmInterval
            {
                public ModelPmInterval()
                {
                    ListPlan = new List<string>();
                    ListActual = new List<string>();
                }

                public string Name { get; set; }
                public List<string> ListPlan { get; set; }
                public List<string> ListActual { get; set; }
            }
            public string Name { get; set; }
            public List<ModelPmInterval>  ListPmIntervals { get; set; }

            public ModelMeterMaintenanceLevel()
            {
                ListPmIntervals = new List<ModelPmInterval>();
            }
        }
        public ModelPipeline Pipeline { get; set; }
        public IEnumerable<ModelMeterMaintenance> ListMeterMaintenance { get; set; }
        public IEnumerable<ModelOmColor> ListOmColor { get; set; }
        public ModelBarGraph BarGraph { get; set; }
        public IEnumerable<ModelRegion> ListRegion { get; set; }
        public List<string> ListRegionForTableHeader { get; set; }
        public List<ModelMeterMaintenanceLevel> ListMeterMaintenanceLevelForTable { get; set; }
        public IEnumerable<ModelAccGraph> ListAccGraph { get; set; }
    }
}