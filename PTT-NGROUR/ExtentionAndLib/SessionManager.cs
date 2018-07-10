using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.ExtentionAndLib
{
    public class SessionManager
    {
        private enum enumSessionName
        {
            UtilizationReportPdfInput ,
            UtilizationReportPdfOutput
        }

        private object getSession(enumSessionName pSessionName)
        {
            string strSessionName = pSessionName.ToString();
            object result = HttpContext.Current.Session[strSessionName];
            return result;
        }

        private void setSession(enumSessionName pSessionName , object pObjValue)
        {
            string strSessionName = pSessionName.ToString();
            HttpContext.Current.Session[strSessionName] = pObjValue;
        }

        public Models.ViewModel.ModelUtilizationReportPdfInput UtilizationReportPdfInput
        {
            get
            {                
                var objResult = getSession(enumSessionName.UtilizationReportPdfInput);
                var result = objResult as Models.ViewModel.ModelUtilizationReportPdfInput;
                return result;
            }
            set
            {
                setSession(enumSessionName.UtilizationReportPdfInput, value);
            }
        }
    }
}