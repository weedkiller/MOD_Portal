using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ACQ.Web.App
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterHttpFilters(GlobalConfiguration.Configuration.Filters);
        }

        protected void Application_Error()
        {
            string ipaddress = Request.UserHostAddress;
            Exception ex = Server.GetLastError();
            Server.ClearError();
            //Response.Redirect(String.Format("~/Error"));
        }

        public class SessionExpireAttribute : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                HttpContext ctx = HttpContext.Current;
                // check  sessions here
                if (HttpContext.Current.Session["Comp_ReferenceNo"] == null)
                {
                    filterContext.Result = new RedirectResult(HttpContext.Current.Session["UrlPath"].ToString());
                    return;
                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
