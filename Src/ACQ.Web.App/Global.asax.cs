using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
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
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
        }

        protected void Application_Error()
        {
            string ipaddress = Request.UserHostAddress;
            Exception ex = Server.GetLastError();
            Server.ClearError();
            Response.Redirect(String.Format("~/Account/Error"));

            //Exception exception = Server.GetLastError();
            //HttpException httpException = exception as HttpException;


        }


        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            var app = sender as HttpApplication;
            if (app != null && app.Context != null)
            {
                app.Context.Response.Headers.Remove("Server");
                app.Response.Headers.Remove("Server");           //Remove Server Header   
                app.Response.Headers.Remove("X-AspNet-Version"); //Remove X-AspNet-Version Header
            }
        }
        public class SessionExpire : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                HttpContext ctx = HttpContext.Current;
                // check  sessions here
                if (HttpContext.Current.Session["UserName"] == null)
                {
                    filterContext.Result = new RedirectResult("~/Account/Login");
                    return;
                }
                base.OnActionExecuting(filterContext);
            }
        }
        public class SessionExpireRefNo : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                HttpContext ctx = HttpContext.Current;
                // check  sessions here
                if (HttpContext.Current.Session["UserName"] == null)
                {
                    filterContext.Result = new RedirectResult("~/Account/Login");
                    return;
                }
                base.OnActionExecuting(filterContext);
            }
        }
    }
}
