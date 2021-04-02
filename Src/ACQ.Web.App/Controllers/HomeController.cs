
using System.Web.Mvc;
using static ACQ.Web.App.MvcApplication;

namespace ACQ.Web.App.Controllers
{
    public class HomeController : Controller
    {

        [Route("home")]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult index()
        {
            return View();
        }
        


    }


}