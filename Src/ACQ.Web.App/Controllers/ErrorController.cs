using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ACQ.Web.App.Controllers
{
    public class ErrorController : Controller
    {
        // GET: ErrorController
       

        public ActionResult NotFound()
        {
            return View();
        }
    }
}