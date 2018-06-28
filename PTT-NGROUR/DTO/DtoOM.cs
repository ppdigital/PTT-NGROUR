using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using PTT_NGROUR.Models.DataModel;

namespace PTT_NGROUR.DTO
{
    public class DtoOM
    {
        public IEnumerable<ModelMeterMaintenance> GetListMeterMaintenance()
        {
            string strCommand = "select * from METER_MAINTENANCE";
            var dal = new DAL.DAL();
            var result = dal.ReadData<ModelMeterMaintenance>(strCommand,  x => new ModelMeterMaintenance(x));
            dal = null;
            return result;
        }
    }
}