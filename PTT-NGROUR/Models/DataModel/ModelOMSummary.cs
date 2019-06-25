using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelOMSummary
    {
        public ModelOMSummary() { }
        public ModelOMSummaryPipeline Pipeline { get; set; }
        public ModelOMSummaryMaintenanceLevel Gate { get; set; }
        public ModelOMSummaryMaintenanceLevel Meter { get; set; }

        public class ModelOMSummaryPipeline
        {
            public ModelOMSummaryPipeline(int month, IEnumerable<ModelMonitoringResults> listResults, string mode, IEnumerable<ModelPlanYearly> PlanYearly)
            {
                Results = listResults.ToList();

                #region Current
                if (mode.Equals("monthly"))
                {
                    Current = listResults.Where(x => x.MONTH.Equals(month))
                        .Where(x => !string.IsNullOrEmpty(x.PM_TYPE))
                        .GroupBy(x => x.PM_TYPE, (pm_type, listGroup) => new ModelMonitoringResultsType
                        {
                            PM_TYPE = pm_type,
                            Activities = listGroup.GroupBy(x => x.PM_ID, (pm_id, l) => new ModelMonitoringResultsActivity
                            {
                                PM_ID = pm_id,
                                PM_NAME = l.First().PM_NAME_FULL,
                                PM_TYPE = l.First().PM_TYPE,
                                PLAN = l.Sum(o => o.PLAN),
                                ACTUAL = l.Sum(o => o.ACTUAL),
                                PERCENTAGE = GetPercentage(l),
                            }).ToList(),
                            Percentage = GetPercentage(listGroup)
                        }).ToList();

                    CurrentOrverallPercentage = GetTypePercentage(Current);
                }
                #endregion




                #region Accumulate
                Accumulate = listResults
                    .Where(x => !string.IsNullOrEmpty(x.PM_TYPE))
                    .GroupBy(x => x.PM_TYPE, (pm_type, listGroup) => new ModelMonitoringResultsType
                    {
                        PM_TYPE = pm_type,
                        Activities = listGroup.GroupBy(x => x.PM_ID, (pm_id, l) => new ModelMonitoringResultsActivity
                        {
                            PM_ID = pm_id,
                            PM_NAME = l.First().PM_NAME_FULL,
                            PM_TYPE = l.First().PM_TYPE,
                            PLAN = PlanYearly.Any(x => x.PM_ID.Equals(pm_id)) ? PlanYearly.SingleOrDefault(x => x.PM_ID.Equals(pm_id)).PLAN : 0,
                            ACTUAL = l.Sum(o => o.ACTUAL),
                            PERCENTAGE = GetPercentage(l),
                        }).ToList(),
                        Percentage = GetPercentage(listGroup)
                    }).ToList();

                AccumulateOrverallPercentage = GetTypePercentage(Accumulate);
                #endregion

                decimal GetPercentage(IEnumerable<ModelMonitoringResults> list)
                {
                    decimal plan = list.Sum(o => o.PLAN);
                    decimal actual = list.Sum(o => o.ACTUAL);

                    if (plan.Equals(0)) return actual;

                    return Decimal.Round((actual / plan) * 100, 2);
                }

                decimal GetTypePercentage(IEnumerable<ModelMonitoringResultsType> list)
                {
                    decimal actual = list.Sum(x => x.Activities.Sum(o => o.ACTUAL));
                    decimal plan = list.Sum(x => x.Activities.Sum(o => o.PLAN)).Equals(0) ? 1 : list.Sum(x => x.Activities.Sum(o => o.PLAN));
                    return Decimal.Round((actual / plan) * 100, 2);
                }
            }

            List<ModelMonitoringResults> Results { get; set; }
            public List<ModelMonitoringResultsType> Current { get; set; }
            public decimal CurrentOrverallPercentage { get; set; }

            public List<ModelMonitoringResultsType> Accumulate { get; set; }
            public decimal AccumulateOrverallPercentage { get; set; }
        }


        public class ModelOMSummaryMaintenanceLevel
        {
            public ModelOMSummaryMaintenanceLevel(int month, int year, IEnumerable<ModelMonitoringResults> listResults, string mode, IEnumerable<ModelPlanYearly> PlanYearly)
            {
                Results = listResults.ToList();
                DateTime date = DateTime.Parse($"{month}/1/{year}", CultureInfo.InvariantCulture);

                #region Current
                if (mode.Equals("monthly"))
                {
                    Current = listResults.Where(x => x.START_DATE <= date && x.END_DATE >= date)
                        .GroupBy(x => x.PM_TYPE, (pm_type, listGroup) => new ModelMonitoringResultsType
                        {
                            Activities = listGroup.GroupBy(x => x.PM_ID, (pm_id, l) => new ModelMonitoringResultsActivity
                            {
                                PM_ID = pm_id,
                                PLAN = l.Sum(o => o.PLAN),
                                ACTUAL = l.Sum(o => o.ACTUAL),
                                PERCENTAGE = GetPercentage(l),
                            }).ToList(),
                            Percentage = GetPercentage(listGroup)
                        })
                        .ToList();

                    CurrentOrverallPercentage = GetTypePercentage(Current);
                }
                #endregion




                #region Accumulate
                Accumulate = listResults
                    .GroupBy(x => x.PM_TYPE, (pm_type, listGroup) => new ModelMonitoringResultsType
                    {
                        Activities = listGroup.GroupBy(x => x.PM_ID, (pm_id, l) => new ModelMonitoringResultsActivity
                        {
                            PM_ID = pm_id,
                            PM_NAME = l.First().PM_NAME_FULL,
                            PM_TYPE = l.First().PM_TYPE,
                            PLAN = PlanYearly.Any(x => x.PM_ID.Equals(pm_id)) ? PlanYearly.SingleOrDefault(x => x.PM_ID.Equals(pm_id)).PLAN : 0,
                            ACTUAL = l.Sum(o => o.ACTUAL),
                            PERCENTAGE = GetPercentage(l),
                        }).ToList(),
                        Percentage = GetPercentage(listGroup)
                    }).ToList();

                AccumulateOrverallPercentage = GetTypePercentage(Accumulate);
                #endregion

                decimal GetPercentage(IEnumerable<ModelMonitoringResults> list)
                {
                    decimal plan = list.Sum(o => o.PLAN);
                    decimal actual = list.Sum(o => o.ACTUAL);

                    if (plan.Equals(0)) return actual;

                    return Decimal.Round((actual / plan) * 100, 2);
                }

                decimal GetTypePercentage(IEnumerable<ModelMonitoringResultsType> list)
                {
                    decimal actual = list.Sum(x => x.Activities.Sum(o => o.ACTUAL));
                    decimal plan = list.Sum(x => x.Activities.Sum(o => o.PLAN)).Equals(0) ? 1 : list.Sum(x => x.Activities.Sum(o => o.PLAN));
                    return Decimal.Round((actual / plan) * 100, 2);
                }
            }

            List<ModelMonitoringResults> Results { get; set; }
            public List<ModelMonitoringResultsType> Current { get; set; }
            public decimal CurrentOrverallPercentage { get; set; }

            public List<ModelMonitoringResultsType> Accumulate { get; set; }
            public decimal AccumulateOrverallPercentage { get; set; }
        }
    };
}