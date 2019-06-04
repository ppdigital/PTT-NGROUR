using PTT_NGROUR.ExtentionAndLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelOMCompletion
    {
        public ModelOMCompletion() { }
        public ModelOMCompletionPipeline Pipeline { get; set; }
        public ModelOMCompletionMaintenanceLevel Gate { get; set; }
        public ModelOMCompletionMaintenanceLevel Meter { get; set; }

        public class ModelOMCompletionPipeline
        {
            public ModelOMCompletionPipeline(int month, IEnumerable<ModelMonitoringResults> listResults, string mode)
            {
                Results = listResults.ToList();
                
                if (!month.Equals(0))
                {
                    listResults = listResults.Where(x => x.MONTH.Equals(month) || mode.Equals("yearly"))
                        .Where(x => !string.IsNullOrEmpty(x.PM_TYPE));

                    #region Activity
                    Activity = listResults
                        .GroupBy(type => type.PM_TYPE, (pm_type, types) => new ModelTypeResults
                        {
                            PM_TYPE = pm_type,
                            Activities = types.GroupBy(activity => activity.PM_ID, (pm_id, activities) => new ModelActivityResults
                            {
                                PM_ID = pm_id,
                                Regions = activities.GroupBy(x => x.REGION, (region_id, x) => new
                                {
                                    REGION = region_id,
                                    List = x.ToList()
                                })
                                .Select(x => new ModelResults
                                {
                                    REGION_ID = x.REGION.Replace("Region", "").GetInt(),
                                    REGION = x.REGION,
                                    PLAN = x.List.Sum(o => o.PLAN),
                                    ACTUAL = x.List.Sum(o => o.ACTUAL),
                                })
                                .OrderBy(x => x.REGION_ID)
                                .ToList()
                            }).ToList()
                        }).ToList();
                    #endregion

                    #region Region
                    Region = listResults.GroupBy(x => x.REGION, (region_id, x) => new {
                        REGION = region_id,
                        List = x.ToList()
                    })
                    .Select(x => new ModelResults
                    {
                        REGION_ID = x.REGION.Replace("Region", "").GetInt(),
                        REGION = x.REGION,
                        Activities = x.List.GroupBy(o => o.PM_ID, (pm_id, o) => new
                        {
                            PM_ID = pm_id,
                            List = o.OrderByDescending(list => list.YEAR).ThenByDescending(list => list.MONTH)
                        })
                        .Select(o => new ModelMonitoringResultsActivity
                        {
                            PM_ID = o.PM_ID,
                            PM_NAME = o.List.First().PM_NAME_FULL,
                            PM_TYPE = o.List.First().PM_TYPE,
                            PLAN = o.List.Sum(p => p.PLAN),
                            ACTUAL = o.List.Sum(p => p.ACTUAL),
                            PERCENTAGE = GetPercentage(o.List),
                        })
                        .OrderBy(o => o.PM_ID)
                        .ToList()
                    })
                    .OrderBy(x => x.REGION_ID)
                    .ToList();
                    #endregion
                }
            }

            List<ModelMonitoringResults> Results { get; set; }
            public List<ModelTypeResults> Activity { get; set; }
            public List<ModelResults> Region { get; set; }

            decimal GetPercentage(IOrderedEnumerable<ModelMonitoringResults> list)
            {
                decimal plan = list.Sum(o => o.PLAN);
                decimal actual = list.Sum(o => o.ACTUAL);

                if (plan.Equals(0)) return actual;

                return Decimal.Round((actual / plan) * 100, 2);
            }
        }

        public class ModelOMCompletionMaintenanceLevel
        {
            public ModelOMCompletionMaintenanceLevel(int month, int year, IEnumerable<ModelMonitoringResults> listResults, string mode)
            {
                Results = listResults.ToList();
                DateTime date = DateTime.Parse($"{month}/1/{year}", System.Globalization.CultureInfo.InvariantCulture);

                if (!month.Equals(0))
                {
                    listResults = listResults.Where(x => (x.START_DATE <= date && x.END_DATE >= date) || mode.Equals("yearly"));

                    #region Activity
                    Activity = listResults
                        .GroupBy(type => type.PM_TYPE, (pm_type, types) => new ModelIntervalTypeResults
                        {
                            PM_TYPE = pm_type,
                            Activities = types.GroupBy(activity => activity.PM_ID, (pm_id, interval) => new ModelIntervalResults
                            {
                                PM_ID = pm_id,
                                Activities = interval.GroupBy(x => x.INTERVAL, (interval_id, regions) => new ModelIntervalActivityResults
                                {
                                    INTERVAL = interval_id,
                                    Regions = regions.GroupBy(x => x.REGION, (region_id, x) => new {
                                        REGION = region_id,
                                        List = x.ToList()
                                    })
                                    .Select(x => new ModelResults
                                    {
                                        REGION_ID = x.REGION.Replace("Region", "").GetInt(),
                                        REGION = x.REGION,
                                        PLAN = x.List.Sum(o => o.PLAN),
                                        ACTUAL = x.List.Sum(o => o.ACTUAL),
                                        //PERCENTAGE = GetPercentage(o.List),
                                    })
                                    .OrderBy(x => x.REGION_ID)
                                    .ToList()
                                }).ToList()
                            }).ToList()
                        }).ToList();
                    #endregion

                    #region Region
                    Region = listResults
                        .GroupBy(x => x.REGION, (region_id, x) => new {
                            REGION = region_id,
                            List = x.ToList()
                        })
                    .Select(x => new ModelResults
                    {
                        REGION_ID = x.REGION.Replace("Region", "").GetInt(),
                        REGION = x.REGION,
                        Activities = x.List.GroupBy(o => o.PM_ID, (pm_id, o) => new
                        {
                            PM_ID = pm_id,
                            List = o.OrderByDescending(list => list.YEAR).ThenByDescending(list => list.MONTH)
                        })
                        .Select(o => new ModelMonitoringResultsActivity
                        {
                            PM_ID = o.PM_ID,
                            PM_NAME = o.List.First().PM_NAME_FULL,
                            PM_TYPE = o.List.First().PM_TYPE,
                            PLAN = o.List.Sum(p => p.PLAN),
                            ACTUAL = o.List.Sum(p => p.ACTUAL),
                            PERCENTAGE = GetPercentage(o.List),
                        })
                        .OrderBy(o => o.PM_ID)
                        .ToList()
                    })
                    .OrderBy(x => x.REGION_ID)
                    .ToList();
                    #endregion
                }
            }

            List<ModelMonitoringResults> Results { get; set; }
            public List<ModelIntervalTypeResults> Activity { get; set; }
            public List<ModelResults> Region { get; set; }

            decimal GetPercentage(IOrderedEnumerable<ModelMonitoringResults> list)
            {
                decimal plan = list.Sum(o => o.PLAN);
                decimal actual = list.Sum(o => o.ACTUAL);

                if(plan.Equals(0)) return actual;

                return Decimal.Round((actual / plan) * 100, 2);
            }
        }
    }
}