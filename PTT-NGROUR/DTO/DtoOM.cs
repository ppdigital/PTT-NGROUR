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
    public class DtoOM
    {
        public IEnumerable<ModelMeterMaintenance> GetListMeterMaintenance()
        {
            string strCommand = "select * from METER_MAINTENANCE";
            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand,  x => new ModelMeterMaintenance(x));
            dal = null;
            return result;
        }

        public IEnumerable<ModelMeterMaintenance> GetListMeterMaintenance(string pStrMonth , string pStrYear , string[] pArrRegion)
        {
            string strCommand = "select * from METER_MAINTENANCE where 1=1 ";
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
            var result = dal.ReadData(strCommand, x => new ModelMeterMaintenance(x));
            dal = null;
            return result;
        }

        public IEnumerable<ModelOmColor> GetListOmColor()
        {
            string strCommand = "select * from OM_GATE_COLOR";
            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelOmColor(x));
            dal = null;
            return result;
        }

        public IEnumerable<ModelRegion> GetListRegion()
        {
            string strCommand = "select * from REGION";
            var dal = new DAL.DAL();
            var result = dal.ReadData(strCommand, x => new ModelRegion(x));
            dal = null;
            return result;
        }

        public ModelOmIndex.ModelBarGraph GetModelBarGraph(
        IEnumerable<ModelMeterMaintenance> pListModelMeterMaintenance , 
        IEnumerable<ModelOmColor> pListModelOmColor)
        {
            if(pListModelMeterMaintenance == null || !pListModelMeterMaintenance.Any() || pListModelOmColor == null || !pListModelOmColor.Any())
            {
                return null;
            }
            var result = new ModelOmIndex.ModelBarGraph();
            var listRegion = pListModelMeterMaintenance.Select(x => x.REGION)
                .Distinct()
                .OrderBy(x => x.Length)
                .ThenBy(x => x)
                .ToList();
            result.ListLabel = listRegion
                    .Select(x => "Region " + x.ToString())
                    .Union(new string[] { "Over All" })
                    .ToList();
            result.ListML = new List<ModelOmIndex.ModelBarGraph.ModelML>();
            var listML = pListModelMeterMaintenance.Select(x => x.ML).Distinct().OrderBy(x => x).ToList();
            foreach (var itemML in listML)
            {
                var ml = new ModelOmIndex.ModelBarGraph.ModelML();
                ml.Label = itemML;
                var omColor = pListModelOmColor.Where(x => x.ML_ID == itemML).FirstOrDefault();
                if(omColor == null)
                {
                    ml.HexColor = "#000000";
                }
                else
                {
                    ml.HexColor = omColor.ML_HEX;
                }
                omColor = null;
                
                ml.ListData = new List<int>();
                var listML1 = pListModelMeterMaintenance.Where(x => x.ML == itemML).ToList();
                foreach (var itemRegion in listRegion)
                {
                    var listML2 = listML1.Where(x => x.REGION == itemRegion).ToList();
                    var sumPlan = listML2.Select(x => x.PLAN).Sum();
                    if (decimal.Zero.Equals(sumPlan))
                    {                        
                        ml.ListData.Add(0);
                    }
                    else
                    {
                        var sumAc = listML2.Select(x => x.ACTUAL).Sum();
                        var intData = Convert.ToInt32(sumAc * 100 / sumPlan);
                        ml.ListData.Add(intData);
                    }

                    listML2.Clear();
                    listML2 = null;
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
                result.ListML.Add(ml);

                listML1.Clear();
                listML1 = null;

            }
            listRegion.Clear();
            listRegion = null;
            listML.Clear();
            listML = null;
            GC.Collect();
            return result;
        }

        public ModelOmIndex.ModelAccGraph[] GetModelAccGraph(IEnumerable<ModelMeterMaintenance> pListModelMeterMaintenance)
        {
            if(pListModelMeterMaintenance == null || !pListModelMeterMaintenance.Any())
            {
                return null;
            }

            var result = pListModelMeterMaintenance.GroupBy(x => x.ML).Select(x => new ModelOmIndex.ModelAccGraph(x.Key, pListModelMeterMaintenance)).ToArray(); 

            return result;
        }

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

        public List< ModelOmIndex.ModelMeterMaintenanceLevel> 
        GetModelModelMeterMaintenanceLevel(
        IEnumerable<ModelMeterMaintenance> pListModelMeterMaintenance , 
        IEnumerable<string> pListRegion)
        {
            if (pListModelMeterMaintenance == null || !pListModelMeterMaintenance.Any())
            {
                return null;
            }
            var result = new List<ModelOmIndex.ModelMeterMaintenanceLevel>();
            //var xs = pListModelMeterMaintenance
            //    .OrderBy(x => x.REGION)
            //    .GroupBy(x => new { x.ML, x.PM_INTERVAL })
            //    .GroupBy(x => x.Key.ML);
            //    //.Select(x=> x.Key.fi)
            //    //xs.Select(x=> new { x.Key , x.Select(x2=> x2.Select() )
            //foreach( var x2 in xs)
            //{
            //    foreach(var x3 in x2)
            //    {
            //        foreach(var x4 in x3)
            //        {

            //        }
            //    }
            //}
            var listMlName = pListModelMeterMaintenance.Select(x => x.ML).Distinct().ToList();
            //var listRegion = pListModelMeterMaintenance.Select(x => x.REGION).Distinct().OrderBy(x=>x).ToList();
            foreach( var strMl in listMlName)
            {
                var ml = new ModelOmIndex.ModelMeterMaintenanceLevel();
                result.Add(ml);
                ml.Name = strMl;
                var listFilter = pListModelMeterMaintenance.Where(x => x.ML == strMl).ToList();
                var listPmName = listFilter.Select(x => x.PM_INTERVAL).Distinct();
                foreach (var strPm in listPmName)
                {
                    var pmi = new ModelOmIndex.ModelMeterMaintenanceLevel.ModelPmInterval();
                    ml.ListPmIntervals.Add(pmi);
                    pmi.Name = strPm;
                    var listFilterPm = listFilter.Where(x => x.PM_INTERVAL == strPm).ToList();
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