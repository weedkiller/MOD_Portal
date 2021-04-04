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
            //BundleConfig.RegisterBundles(BundleTable.Bundles);
            FilterConfig.RegisterHttpFilters(GlobalConfiguration.Configuration.Filters);
            AntiForgeryConfig.SuppressIdentityHeuristicChecks = true;
            MvcHandler.DisableMvcResponseHeader = true;
        }

        protected void Application_Error()
        {
            string ipaddress = Request.UserHostAddress;
            Exception ex = Server.GetLastError();
            Server.ClearError();
            Response.Redirect(String.Format("~/Error"));

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {
            string[] headers = { "Server", "X-AspNet-Version" };

            if (!Response.HeadersWritten)
            {
                Response.AddOnSendingHeaders((c) =>
                {
                    if (c != null && c.Response != null && c.Response.Headers != null)
                    {
                        foreach (string header in headers)
                        {
                            if (c.Response.Headers[header] != null)
                            {
                                c.Response.Headers.Remove(header);
                            }
                        }
                    }
                });
            }

        }

        protected void Application_EndRequest()
        {
            // removing excessive headers. They don't need to see this.
            Response.Headers.Remove("header_name");
        }
        public class SessionExpire : ActionFilterAttribute
        {
            public override void OnActionExecuting(ActionExecutingContext filterContext)
            {
                HttpContext ctx = HttpContext.Current;
                // check  sessions here
                if (HttpContext.Current.Session["UserID"] == null)
                {
                    filterContext.Result = new RedirectResult("~/login");
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
                    filterContext.Result = new RedirectResult("~/login");
                    return;
                }
                base.OnActionExecuting(filterContext);
            }
        }

       
    }
}
