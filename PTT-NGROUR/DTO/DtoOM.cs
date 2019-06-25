using PTT_NGROUR.ExtentionAndLib;
using PTT_NGROUR.Models.DataModel;
using PTT_NGROUR.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using static PTT_NGROUR.Models.DataModel.ModelOMMaster;

namespace PTT_NGROUR.DTO
{
    public class DtoOM
    {
        public List<ModelOMMasterPipeline> GetPipelineActivity()
        {
            string strCommand = "SELECT * FROM VIEW_PIPELINE_ACTIVITY";
            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelOMMasterPipeline(x));
            dal = null;
            return result.ToList();
        }

        public List<ModelOMMasterMaintenanceLevel> GetListMaintenanceLevelColor()
        {
            string strCommand = "SELECT * FROM OM_GATE_COLOR";
            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelOMMasterMaintenanceLevel(x));
            dal = null;
            return result.ToList();
        }

        public IEnumerable<ModelMonitoringResults> GetListOMPipelineHistory(int month, int year, string[] pArrRegion, bool accumulate)
        {
            if (month.Equals(0) && !accumulate)
            {
                return null;
            }
            string strCommand = "SELECT * FROM VIEW_OM_PIPELINE_HISTORY WHERE 1=1 ";
            if (!month.Equals(0))
            {
                strCommand += $" AND MONTH {(accumulate ? "<" :  "")}= " + month;
            }

            if (!month.Equals(0))
            {
                strCommand += " AND YEAR = " + year;
            }

            if (pArrRegion != null && pArrRegion.Any())
            {
                string strAllRegion = string.Join(",", pArrRegion);
                strCommand += " AND REGION in (" + strAllRegion + ")";
            }

            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelMonitoringResults(x));
            dal = null;
            return result;
        }

        public IEnumerable<ModelPlanYearly> GetListOMPipelinePlanYearly(int year, string[] pArrRegion)
        {
            string strCommand = "SELECT PM, SUM(PLAN) AS PLAN FROM VIEW_OM_PIPELINE_HISTORY WHERE YEAR = " + year;

            if (pArrRegion != null && pArrRegion.Any()) strCommand += " AND REGION in (" + string.Join(",", pArrRegion) + ")";

            strCommand += " GROUP BY PM"; 

            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelPlanYearly(x));
            dal = null;
            return result;
        }

        public IEnumerable<ModelMonitoringResults> GetListOMGateHistory(int month, int year, string[] pArrRegion, bool accumulate)
        {
            if (month.Equals(0) && !accumulate)
            {
                return null;
            }
            string strCommand = "SELECT * FROM VIEW_OM_ML_GATE_HISTORY WHERE 1=1 ";
            if (!month.Equals(0))
            {
                if (accumulate)
                {
                    strCommand += $" AND EXTRACT(month FROM END_DATE) <= {month} AND EXTRACT(year FROM END_DATE) = " + year;
                }
                else
                {
                    strCommand += $" AND TO_DATE ('{year}/{month}/01', 'yyyy/mm/dd') BETWEEN START_DATE AND END_DATE";
                }
            }

            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelMonitoringResults(x));
            dal = null;
            return result;
        }

        public IEnumerable<ModelPlanYearly> GetListOMGatePlanYearly(int year, string[] pArrRegion)
        {
            string strCommand = "SELECT PM, SUM(PLAN) AS PLAN FROM VIEW_OM_ML_GATE_HISTORY WHERE EXTRACT(year FROM END_DATE) = " + year;

            if (pArrRegion != null && pArrRegion.Any()) strCommand += " AND REGION in (" + string.Join(",", pArrRegion) + ")";

            strCommand += " GROUP BY PM";

            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelPlanYearly(x));
            dal = null;
            return result;
        }

        public IEnumerable<ModelMonitoringResults> GetListOMMeterHistory(int month, int year, string[] pArrRegion, bool accumulate)
        {
            if (month.Equals(0) && !accumulate)
            {
                return null;
            }
            string strCommand = "SELECT * FROM VIEW_OM_ML_METER_HISTORY WHERE 1=1 ";
            if (!month.Equals(0))
            {
                if(accumulate)
                {
                    strCommand += $" AND EXTRACT(month FROM END_DATE) <= {month} AND EXTRACT(year FROM END_DATE) = " + year;
                }
                else
                {
                    strCommand += $" AND TO_DATE ('{year}/{month}/01', 'yyyy/mm/dd') BETWEEN START_DATE AND END_DATE";
                }
            }

            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelMonitoringResults(x));
            dal = null;
            return result;
        }

        public IEnumerable<ModelPlanYearly> GetListOMMeterPlanYearly(int year, string[] pArrRegion)
        {
            string strCommand = "SELECT PM, SUM(PLAN) AS PLAN FROM VIEW_OM_ML_METER_HISTORY WHERE EXTRACT(year FROM END_DATE) = " + year;

            if (pArrRegion != null && pArrRegion.Any()) strCommand += " AND REGION in (" + string.Join(",", pArrRegion) + ")";

            strCommand += " GROUP BY PM";

            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelPlanYearly(x));
            dal = null;
            return result;
        }

        public IEnumerable<ModelMeterMaintenance> GetListMeterMaintenance()
        {
            string strCommand = "SELECT * from METER_MAINTENANCE";
            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelMeterMaintenance(x));
            dal = null;
            return result;
        }

        public IEnumerable<ModelMeterMaintenance> GetListMeterMaintenance(string pStrMonth , string pStrYear , string[] pArrRegion)
        {
            string strCommand = "SELECT * from METER_MAINTENANCE where 1=1 ";
            if (!string.IsNullOrEmpty(pStrMonth))
            {
                strCommand += " AND MONTH =" + pStrMonth;
            }
            if (!string.IsNullOrEmpty(pStrYear))
            {
                strCommand += " AND YEAR =" + pStrYear;
            }
            if(pArrRegion != null && pArrRegion.Any())
            {
                string strAllRegion = string.Join(",", pArrRegion);
                strCommand += " AND REGION in (" + strAllRegion + ")";
            }
            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelMeterMaintenance(x));
            dal = null;
            return result;
        }

        public IEnumerable<ModelRegion> GetListRegion()
        {
            string strCommand = "SELECT * FROM REGION";
            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelRegion(x));
            dal = null;
            return result;
        }

        public IEnumerable<ModelPmInterval> GetListPmInterval()
        {
            string strCommand = "select * from PM_INTERVAL";
            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelPmInterval(x));
            dal = null;
            return result;
        }

        //public ModelOmIndex.ModelBarGraph GetModelBarGraph(
        //IEnumerable<ModelMeterMaintenance> pListModelMeterMaintenance , 
        //IEnumerable<ModelOmColor> pListModelOmColor)
        //{
        //    if(pListModelMeterMaintenance == null || !pListModelMeterMaintenance.Any() || pListModelOmColor == null || !pListModelOmColor.Any())
        //    {
        //        return null;
        //    }
        //    var result = new ModelOmIndex.ModelBarGraph();
        //    var listRegion = pListModelMeterMaintenance.Select(x => x.REGION)
        //        .Distinct().Where(x => !string.IsNullOrEmpty(x))
        //        .OrderBy(x => x.Length)
        //        .ThenBy(x => x)
        //        .ToList();
        //    result.ListLabel = listRegion
        //            .Select(x => "Region " + x.ToString())
        //            .Union(new string[] { "Over All" })
        //            .ToList();
        //    result.ListML = new List<ModelOmIndex.ModelBarGraph.ModelML>();
        //    var listML = pListModelMeterMaintenance.Select(x => x.ML).Distinct().OrderBy(x => x).ToList();
        //    foreach (var itemML in listML)
        //    {
        //        var ml = new ModelOmIndex.ModelBarGraph.ModelML();
        //        ml.Label = itemML;
        //        var omColor = pListModelOmColor.Where(x => x.ML_ID == itemML).FirstOrDefault();
        //        if(omColor == null)
        //        {
        //            ml.HexColor = "#000000";
        //        }
        //        else
        //        {
        //            ml.HexColor = omColor.ML_HEX;
        //        }
        //        omColor = null;
                
        //        ml.ListData = new List<int>();
        //        var listML1 = pListModelMeterMaintenance.Where(x => x.ML == itemML).ToList();
        //        foreach (var itemRegion in listRegion)
        //        {
        //            var listML2 = listML1.Where(x => x.REGION == itemRegion).ToList();
        //            var sumPlan = listML2.Select(x => x.PLAN).Sum();
        //            var sumAc = listML2.Select(x => x.ACTUAL).Sum();
        //            if (sumPlan==sumAc)
        //            {                        
        //                ml.ListData.Add(100);
        //            }
        //            else if (decimal.Zero.Equals(sumPlan))
        //            {
        //                ml.ListData.Add((sumAc * 100).GetInt());
        //            }
        //            else
        //            {                        
        //                var intData = Convert.ToInt32(sumAc * 100 / sumPlan);
        //                ml.ListData.Add(intData);
        //            }
        //            //ml.ListData.Add(50);
        //            listML2.Clear();
        //            listML2 = null;
        //        }
        //        var sumPlanAll = listML1.Select(x => x.PLAN).Sum();
        //        if (decimal.Zero.Equals(sumPlanAll))
        //        {
        //            ml.ListData.Add(0);
        //        }
        //        else
        //        {
        //            var sumAcAll = listML1.Select(x => x.ACTUAL).Sum();
        //            var intData = Convert.ToInt32(sumAcAll * 100 / sumPlanAll);
        //            ml.ListData.Add(intData);
        //        }
        //        result.ListML.Add(ml);

        //        listML1.Clear();
        //        listML1 = null;

        //    }
        //    listRegion.Clear();
        //    listRegion = null;
        //    listML.Clear();
        //    listML = null;
        //    GC.Collect();
        //    return result;
        //}

        //public ModelOmIndex.ModelAccGraph[] GetModelAccGraph(IEnumerable<ModelMeterMaintenance> pListModelMeterMaintenance)
        //{
        //    if(pListModelMeterMaintenance == null || !pListModelMeterMaintenance.Any())
        //    {
        //        return null;
        //    }

        //    var result = pListModelMeterMaintenance.GroupBy(x => x.ML).Select(x => new ModelOmIndex.ModelAccGraph(x.Key, pListModelMeterMaintenance)).ToArray(); 

        //    return result;
        //}

        public List<string> GetListRegionForTableHeader(IEnumerable<ModelMeterMaintenance> pListModelMeterMaintenance)
        {
            if (pListModelMeterMaintenance == null || !pListModelMeterMaintenance.Any())
            {
                return null;
            }
            var result = pListModelMeterMaintenance
                .Select(x => x.REGION)
                .Distinct()
                .OrderBy(x => x.Length)
                .ThenBy(x=>x)
                .ToList();
            return result;
        }

        //public List< ModelOmIndex.ModelMeterMaintenanceLevel> 
        //GetModelModelMeterMaintenanceLevel(
        //IEnumerable<ModelMeterMaintenance> pListModelMeterMaintenance , 
        //IEnumerable<string> pListRegion)
        //{
        //    if (pListModelMeterMaintenance == null || !pListModelMeterMaintenance.Any())
        //    {
        //        return null;
        //    }
        //    var result = new List<ModelOmIndex.ModelMeterMaintenanceLevel>();            
        //    var listMlName = pListModelMeterMaintenance.Select(x => x.ML).Distinct().ToList();
        //    var listPmInterval = GetListPmInterval().OrderBy(x => x.PM_ID).ToList();            
        //    foreach( var strMl in listMlName)
        //    {
        //        var ml = new ModelOmIndex.ModelMeterMaintenanceLevel();
        //        result.Add(ml);
        //        ml.Name = strMl;
        //        var listFilter = pListModelMeterMaintenance.Where(x => x.ML == strMl).ToList();
        //        var listPmId = listFilter
        //            .Select(x => x.PM_ID)
        //            .Distinct()
        //            .OrderBy(x=>x)                    
        //            .ToList();
        //        foreach (var intPmId in listPmId)
        //        {
        //            var pmi = new ModelOmIndex.ModelMeterMaintenanceLevel.ModelPmInterval();
        //            ml.ListPmIntervals.Add(pmi);
        //            string strPm = string.Empty;
        //            var pmInterval = listPmInterval.Where(x => x.PM_ID == intPmId).FirstOrDefault();
        //            if(pmInterval != null)
        //            {
        //                strPm = pmInterval.INTERVAL;
        //            }
        //            pmi.Name = strPm;
                    
        //            var listFilterPm = listFilter.Where(x => x.PM_ID == intPmId).ToList();
        //            foreach(var strRg in pListRegion)
        //            {
        //                var strPlan = string.Empty;
        //                var strActual = string.Empty;
        //                var listFilterRegion = listFilterPm.Where(x => x.REGION == strRg).ToList();
        //                if (listFilterRegion.Any())
        //                {
        //                    strPlan = listFilterRegion.Sum(x => x.PLAN).GetString();
        //                    strActual = listFilterRegion.Sum(x => x.ACTUAL).GetString();
        //                }                        
        //                pmi.ListPlan.Add(strPlan);
        //                pmi.ListActual.Add(strActual);                        
        //            }
        //        }
        //    }
        //    return result;
        //}

    }
}