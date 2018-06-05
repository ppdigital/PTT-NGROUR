using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.ExtentionAndLib
{
    public static class Ext
    {
        public static string GetString( this object pObject){
            if (pObject == null || Convert.IsDBNull(pObject))
            {
                return string.Empty;
            }
            return pObject.ToString();
        }

        public static int GetInt(this object pObject)
        {
            if (pObject == null || Convert.IsDBNull(pObject))
            {
                return 0;
            }
            return Convert.ToInt32(pObject);
        }

        public static DateTime? GetDate(this object pObject)
        {
            if (pObject == null || Convert.IsDBNull(pObject))
            {
                return null;
            }
            return Convert.ToDateTime(pObject);
        }

        public static decimal GetDecimal(this object pObject)
        {
            if (pObject == null || Convert.IsDBNull(pObject))
            {
                return decimal.Zero;
            }
            return Convert.ToDecimal(pObject);
        }

        public static T GetEnum<T>(this object pObject, T pDefaultValue) where T : struct
        {           
            string strValue = pObject.GetString();
            if (string.IsNullOrEmpty(strValue))
            {
                return pDefaultValue;
            }            
            T result;
            return Enum.TryParse<T>(strValue, true, out result) ? result : pDefaultValue;
        }
    }
}