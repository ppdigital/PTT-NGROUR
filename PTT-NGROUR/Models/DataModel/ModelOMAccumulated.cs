using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelOMAccumulated
    {
        public void getPipeline(int month, int year, IEnumerable<ModelMonitoringResults> list, string mode)
        {
            Pipeline = list.Where(x => !string.IsNullOrEmpty(x.PM_TYPE) && (x.MONTH <= month || mode.Equals("yearly")))
                .Where(x => x.PLAN > 0 || x.ACTUAL > 0)
                .GroupBy(pm => new { pm.PM_ID, pm.MONTH })
                .Select(g => new ModelAccumulatedResults
                {
                    PM_ID = g.Key.PM_ID,
                    MONTH = g.Key.MONTH,
                    PLAN = g.Sum(x => x.PLAN),
                    ACTUAL = g.Sum(x => x.ACTUAL)

                })
                .ToList();
        }

        public void getGate(int month, int year, IEnumerable<ModelMonitoringResults> list, string mode)
        {
            Gate = getMaintenanceLevel(month, year, list, mode);
        }

        public void getMeter(int month, int year, IEnumerable<ModelMonitoringResults> list, string mode)
        {
            Meter = getMaintenanceLevel(month, year, list, mode);
        }

        public List<ModelAccumulatedResults> getMaintenanceLevel(int month, int year, IEnumerable<ModelMonitoringResults> list, string mode)
        {
            if(mode.Equals("yearly"))
            {
                month = 12;
            }

            DateTime endDate = DateTime.Parse($"{month}/1/{year}", System.Globalization.CultureInfo.InvariantCulture);

            List<ModelAccumulatedResults> accumulatedResults = new List<ModelAccumulatedResults>();
            List<ModelMonitoringResults> monitoringList = list.Where(x => x.END_DATE <= endDate)
                    .Where(x => x.PLAN > 0 || x.ACTUAL > 0)
                    .ToList();

            for (int i = 1; i <= month; i++)
            {
                var xsdsd = monitoringList
                    .Where(x => (x.START_DATE.HasValue && i >= x.START_DATE.Value.Month) && (x.END_DATE.HasValue && i <= x.END_DATE.Value.Month))
                    //.Where(x => x.PLAN > 0 || x.ACTUAL > 0)
                    .GroupBy(pm => pm.PM_ID)
                    .Select(g => new ModelAccumulatedResults
                    {
                        PM_ID = g.Key,
                        MONTH = i,
                        PLAN = g.Sum(x => x.PLAN),
                        ACTUAL = g.Sum(x => x.ACTUAL)
                    })
                    .ToList();

                xsdsd.ForEach(x => {
                        accumulatedResults.Add(new ModelAccumulatedResults
                        {
                            PM_ID = x.PM_ID,
                            MONTH = i,
                            PLAN = x.PLAN,
                            ACTUAL = x.ACTUAL
                        });
                    });
            }

            return accumulatedResults;
        }

        public List<ModelAccumulatedResults> Pipeline { get; set; }
        public List<ModelAccumulatedResults> Gate { get; set; }
        public List<ModelAccumulatedResults> Meter { get; set; }
    }
}