using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models
{
    public class UserManagement
    {
        public List<DataModel.ModelUsersAuth> ModelUsersAuth { get; set; }
        public List<DataModel.ModelUsersRole> ModelUsersRole { get; set; }
    }
}