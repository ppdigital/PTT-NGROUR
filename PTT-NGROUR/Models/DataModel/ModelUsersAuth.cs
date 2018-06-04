using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.DataModel
{
    public class ModelUsersAuth
    {
        public string EMPLOYEE_ID { get; set; }

        public string FIRSTNAME { get; set; }

        public string LASTNAME { get; set; }

        public string PASSWORD { get; set; }

        public string EMAIL { get; set; }

        public bool IS_AD { get; set; }

        public int GROUP_ID { get; set; }

        public int ROLE_ID { get; set; }

        public DateTime? CREATE_DATE { get; set; }

        public string CREATE_BY { get; set; }

        public DateTime? UPDATE_DATE { get; set; }

        public string UPDATE_BY { get; set; }
    }
}