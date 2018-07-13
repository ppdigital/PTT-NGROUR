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
            return GetInt(pObject , 0);
        }

        public static int GetInt(this object pObject , int pIntDefaultValue)
        {
            string strValue = pObject.GetString();
            int result;
            return int.TryParse(strValue , out result)?result : pIntDefaultValue;
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
            return GetDecimal(pObject , decimal.Zero);
        }

        public static decimal GetDecimal(this object pObject , decimal pDecDefaultValue)
        {  
            string strValue = pObject.GetString();
            decimal result;
            return decimal.TryParse(strValue, out result)?result : pDecDefaultValue;   
        }
        public static Oracle.ManagedDataAccess.Types.OracleTimeStamp? GetTimeStamp(this object pObject)
        {
            try
            {
                if (pObject == null || Convert.IsDBNull(pObject))
                {
                    return null;
                }
                var result = (Oracle.ManagedDataAccess.Types.OracleTimeStamp)pObject;
                return result;
            }
            catch 
            {
                return null;                
            }        
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

        public static object GetColumnValue(this System.Data.IDataReader pReader , string pStrColName)
        {
            if(pReader == null || string.IsNullOrEmpty(pStrColName))
            {
                return null;
            }
            for (int i = 0; i < pReader.FieldCount; i++)
            {
                if (pReader.GetName(i).Equals(pStrColName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return pReader[pStrColName];
                }
            }
            return null;
        }
    }
}