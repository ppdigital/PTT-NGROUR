using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.ViewModel
{
    public enum EnumJsonResultStatus
    {
        OK, Error, SessionTimeOut
    }

    public class ModelJsonResult<T>
    {
        public ModelJsonResult()
        {

        }

        public void SetException(Exception ex)
        {
            this.ErrorMessage = ex.Message;
            this.ErrorStackTrace = ex.StackTrace;
            this.Status = EnumJsonResultStatus.Error;
        }

        public void SetError(string pStrError)
        {
            this.ErrorMessage = pStrError;
            this.Status = EnumJsonResultStatus.Error;
        }

        public void SetSessionTimeOut()
        {
            this.Status = EnumJsonResultStatus.SessionTimeOut;
            this.ErrorMessage = "Session Time Out Please Login Again";
        }

        public void SetResultValue(T pValue)
        {
            this.Result = pValue;
            this.Status = EnumJsonResultStatus.OK;
        }

        public T Result { get; set; }

        public EnumJsonResultStatus Status { get; set; }

        public string ErrorMessage { get; set; }

        public string ErrorStackTrace { get; set; }

    }
}