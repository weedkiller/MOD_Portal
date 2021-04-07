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
                    Session["Escdata"] = null;
                    Session["Escdata"] = listdata;
                }
            }

            return View(listdata);
        }

        [Route("sendEscalationEmail")]
        [HandleError]
        [ValidateAntiForgeryToken]
        public JsonResult sendEscalationEmail(List<EscalationReportData> model)
        {
            if(model!=null && model.Count()>0)
            {
                foreach(var item in model)
                {
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.BaseAddress = new Uri(WebAPIUrl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                                 parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                        HttpResponseMessage response = client.GetAsync("Escalation/GetEscalationDraft?Tasksln="+item.CaseTaskSlno).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            try
                            {
                                EscalationDraftMessage draftMsg = response.Content.ReadAsAsync<EscalationDraftMessage>().Result;
                                if(draftMsg!=null)
                                {
                                    if(item.EscalationTime<(Convert.ToInt32(item.dap_timeline)-1))
                                    {
                                        string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/EscalationEmailFormat.html"));
                                        IEnumerable<EscalationReportData> listdata = new List<EscalationReportData>();
                                        listdata = (IEnumerable<EscalationReportData>)Session["Escdata"];
                                        EscalationReportData level1 = listdata.Where(x => x.aon_id == item.aon_id).FirstOrDefault();
                                        string message = draftMsg.DraftMessage_L1.ToString();
                                        message = message.Replace("{date}", level1.Date_of_Accord_of_AoN.Value.AddDays(Convert.ToInt32(item.dap_timeline)*7).ToString("MM/dd/yyyy"));
                                        EmailHelper.sendEmailEscalation(level1.Responsible_Level1, message, mailPath);
                                    }
                                    else if(item.EscalationTime ==(Convert.ToInt32(item.dap_timeline)-1))
                                    {
                                        string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/EscalationEmailFormat.html"));
                                        IEnumerable<EscalationReportData> listdata = new List<EscalationReportData>();
                                        listdata = (IEnumerable<EscalationReportData>)Session["Escdata"];
                                        EscalationReportData level1 = listdata.Where(x => x.aon_id == item.aon_id).FirstOrDefault();
                                        //send message to L1.
                                        string L1message = draftMsg.DraftMessage_L1.ToString();
                                        L1message = L1message.Replace("{date}", level1.Date_of_Accord_of_AoN.Value.AddDays(Convert.ToInt32(item.dap_timeline)*7).ToString("MM/dd/yyyy"));
                                        EmailHelper.sendEmailEscalation(level1.Responsible_Level1, L1message, mailPath);

                                        //send message to L2.
                                        string L2message = draftMsg.DraftMessage_L2.ToString();
                                        L2message = L2message.Replace("{date}", level1.Date_of_Accord_of_AoN.Value.AddDays(Convert.ToInt32(item.dap_timeline)*7).ToString("MM/dd/yyyy"));
                                        EmailHelper.sendEmailEscalation(level1.Responsible_Level2, L2message, mailPath);

                                    }
                                    else if(item.EscalationTime > Convert.ToInt32(item.dap_timeline))
                                    {
                                        string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/EscalationEmailFormat.html"));
                                        IEnumerable<EscalationReportData> listdata = new List<EscalationReportData>();
                                        listdata = (IEnumerable<EscalationReportData>)Session["Escdata"];
                                        EscalationReportData level1 = listdata.Where(x => x.aon_id == item.aon_id).FirstOrDefault();
                                        string message = draftMsg.DraftMessage_L3.ToString();
                                        message = message.Replace("{date}", level1.Date_of_Accord_of_AoN.Value.AddDays(Convert.ToInt32(item.dap_timeline)*7).ToString("MM/dd/yyyy"));
                                        EmailHelper.sendEmailEscalation(level1.Responsible_Level3, message, mailPath);
                                    }
                                    
                                }

                                

                                
                            }
                            catch(Exception ex)
                            {

                            }
                            

                        }
                    }
                }
            }
            return Json(new { Status = true, Message="success" }, JsonRequestBehavior.AllowGet);
        }



    }
}