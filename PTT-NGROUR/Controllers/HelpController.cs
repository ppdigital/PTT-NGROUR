using PTT_NGROUR.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PTT_NGROUR.Controllers
{   
    public class HelpController : Controller
    {
        public ActionResult Index()
        {
            return View();
          
        }

        public FileResult Download(string filetype)
        {
         
            string filename = "";
            string fileexename = "";
            string fileextendname = "";

            if (filetype == "1")
            {
                filename = "คู่มือการใช้งานระบบ NGROUR.pdf";
                fileexename = "คู่มือการใช้งานระบบ NGROUR.pdf";
                fileextendname = "application/pdf";
            }
            else if (filetype == "2")
            {
                filename = "คู่มือการใช้งานระบบ NGROUR เพิ่มเติม.pdf";
                fileexename = "คู่มือการใช้งานระบบ NGROUR เพิ่มเติม.pdf";
                fileextendname = "application/pdf";
            }
            

            return File("~/App_Data/" + filename, fileextendname, fileexename);
        }

    }
}
