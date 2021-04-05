using ACQ.Web.Core.Library;
using ACQ.Web.ExternalServices.Email;
using Ganss.XSS;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using static ACQ.Web.App.MvcApplication;
using ACQ.Web.App.ViewModel;

namespace ACQ.Web.App.Controllers
{
    public class ProcessingEmailController : Controller
    {
        HtmlSanitizer sanitizer = new HtmlSanitizer();
        private static string WebAPIUrl = ConfigurationManager.AppSettings["APIUrl"].ToString();

        // GET: validation Check
        [Route("validationchecker")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult validationcheck()
        {
            IEnumerable<EscalationReportData> listdata = new List<EscalationReportData>();

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebAPIUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                         parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                HttpResponseMessage response = client.GetAsync("Escalation/GetEscalationData").Result;
                if (response.IsSuccessStatusCode)
                {
                    listdata = response.Content.ReadAsAsync<IEnumerable<EscalationReportData>>().Result;

                }
            }


            return View(listdata);
        }
    }
}