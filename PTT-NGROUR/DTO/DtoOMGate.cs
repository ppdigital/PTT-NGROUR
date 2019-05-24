using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PTT_NGROUR.Models.DataModel;
using PTT_NGROUR.Models.ViewModel;
using PTT_NGROUR.ExtentionAndLib;
using System.Collections;

namespace PTT_NGROUR.DTO
{
    public class DtoOMGate
    {
        public IEnumerable<ModelGateMaintenance> GetListGateMaintenance()
        {
            string strCommand = "select * from GATE_MAINTENANCE";
            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelGateMaintenance(x));
            dal = null;
            return result;
        }

        public IEnumerable<ModelGateMaintenance> GetListGateMaintenance(string pStrMonth, string pStrYear, string[] pArrRegion)
        {
            string strCommand = "select * from Gate_MAINTENANCE where 1=1 ";
            if (!string.IsNullOrEmpty(pStrMonth))
            {
                strCommand += " and MONTH =" + pStrMonth;
            }
            if (!string.IsNullOrEmpty(pStrYear))
            {
                strCommand += " and YEAR =" + pStrYear;
            }
            if(pArrRegion != null && pArrRegion.Any())
            {
                string strAllRegion = string.Join(",", pArrRegion);
                strCommand += " and region in (" + strAllRegion + ")";
            }
            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelGateMaintenance(x));
            dal = null;
            return result;
        }

        //public IEnumerable<ModelOmColor> GetListOmColor()
        //{
        //    string strCommand = "select * from OM_GATE_COLOR";
        //    var dal = new DAL.DAL();
        //    var result = dal.ReadData(strCommand, x => new ModelOmColor(x));
        //    dal = null;
        //    return result;
        //}

        public IEnumerable<ModelRegion> GetListRegion()
        {
            string strCommand = "select * from REGION";
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

        //public ModelOmIndexGate.ModelBarGraph GetModelBarGraph(
        //IEnumerable<ModelGateMaintenance> pListModelGateMaintenance, 
        //IEnumerable<ModelOmColor> pListModelOmColor)
        //{
        //    if(pListModelGateMaintenance == null || !pListModelGateMaintenance.Any() || pListModelOmColor == null || !pListModelOmColor.Any())
        //    {
        //        return null;
        //    }
        //    var result = new ModelOmIndexGate.ModelBarGraph();
        //    var listRegion = pListModelGateMaintenance.Select(x => x.REGION)
        //        .Distinct().Where(x => !string.IsNullOrEmpty(x))
        //        .OrderBy(x => x.Length)
        //        .ThenBy(x => x)
        //        .ToList();
        //    result.ListLabel = listRegion
        //            .Select(x => "Region " + x.ToString())
        //            .Union(new string[] { "Over All" })
        //            .ToList();
        //    result.ListML = new List<ModelOmIndexGate.ModelBarGraph.ModelML>();
        //    var listML = pListModelGateMaintenance.Select(x => x.ML).Distinct().OrderBy(x => x).ToList();
        //    foreach (var itemML in listML)
        //    {
        //        var ml = new ModelOmIndexGate.ModelBarGraph.ModelML();
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
        //        var listML1 = pListModelGateMaintenance.Where(x => x.ML == itemML).ToList();
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

        public ModelOmIndexGate.ModelAccGraph[] GetModelAccGraph(IEnumerable<ModelGateMaintenance> pListModelGateMaintenance)
        {
            if(pListModelGateMaintenance == null || !pListModelGateMaintenance.Any())
            {
                return null;
            }

            var result = pListModelGateMaintenance.GroupBy(x => x.ML).Select(x => new ModelOmIndexGate.ModelAccGraph(x.Key, pListModelGateMaintenance)).ToArray(); 

            return result;
        }

        public List<string> GetListRegionForTableHeader(IEnumerable<ModelGateMaintenance> pListModelGateMaintenance)
        {
            if (pListModelGateMaintenance == null || !pListModelGateMaintenance.Any())
            {
                return null;
            }
            var result = pListModelGateMaintenance
                .Select(x => x.REGION)
                .Distinct()
                .OrderBy(x => x.Length)
                .ThenBy(x=>x)
                .ToList();
            return result;
        }

        public List< ModelOmIndexGate.ModelGateMaintenanceLevel> 
        GetModelModelGateMaintenanceLevel(
        IEnumerable<ModelGateMaintenance> pListModelGateMaintenance , 
        IEnumerable<string> pListRegion)
        {
            if (pListModelGateMaintenance == null || !pListModelGateMaintenance.Any())
            {
                return null;
            }
            var result = new List<ModelOmIndexGate.ModelGateMaintenanceLevel>();            
            var listMlName = pListModelGateMaintenance.Select(x => x.ML).Distinct().ToList();
            var listPmInterval = GetListPmInterval().OrderBy(x => x.PM_ID).ToList();            
            foreach( var strMl in listMlName)
            {
                var ml = new ModelOmIndexGate.ModelGateMaintenanceLevel();
                result.Add(ml);
                ml.Name = strMl;
                var listFilter = pListModelGateMaintenance.Where(x => x.ML == strMl).ToList();
                var listPmId = listFilter
                    .Select(x => x.PM_ID)
                    .Distinct()
                    .OrderBy(x=>x)                    
                    .ToList();
                foreach (var intPmId in listPmId)
                {
                    var pmi = new ModelOmIndexGate.ModelGateMaintenanceLevel.ModelPmInterval();
                    ml.ListPmIntervals.Add(pmi);
                    string strPm = string.Empty;
                    var pmInterval = listPmInterval.Where(x => x.PM_ID == intPmId).FirstOrDefault();
                    if(pmInterval != null)
                    {
                        strPm = pmInterval.INTERVAL;
                    }
                    pmi.Name = strPm;
                    
                    var listFilterPm = listFilter.Where(x => x.PM_ID == intPmId).ToList();
                    foreach(var strRg in pListRegion)
                    {
                        var strPlan = string.Empty;
                        var strActual = string.Empty;
                        var listFilterRegion = listFilterPm.Where(x => x.REGION == strRg).ToList();
                        if (listFilterRegion.Any())
                        {
                            strPlan = listFilterRegion.Sum(x => x.PLAN).GetString();
                            strActual = listFilterRegion.Sum(x => x.ACTUAL).GetString();
                        }                        
                        pmi.ListPlan.Add(strPlan);
                        pmi.ListActual.Add(strActual);                        
                    }
                }
            }
            return result;
        }

    }
}