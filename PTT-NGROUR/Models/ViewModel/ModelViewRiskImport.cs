﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PTT_NGROUR.Models.ViewModel
{
    public class ModelViewRiskImport
    {
        public string RC_NAME { get; set; }
        public int MONTH { get; set; }
        public int YEAR { get; set; }
        public List<HttpPostedFileBase> FILE { get; set; }
    }
}