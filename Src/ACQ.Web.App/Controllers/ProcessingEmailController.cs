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
using ACQ.Web.ExternalServices.SecurityAudit;
using ACQ.Web.ViewModel.User;
using System.Globalization;

namespace ACQ.Web.App.Controllers
{
    public class ProcessingEmailController : Controller
    {
        public ProcessingEmailController()
        {
            if (BruteForceAttackss.refreshcount == 0 && BruteForceAttackss.date == null)
            {
                BruteForceAttackss.date = System.DateTime.Now;
                BruteForceAttackss.refreshcount = 1;
            }
            else
            {
                TimeSpan tt = System.DateTime.Now - BruteForceAttackss.date.Value;
                if (tt.TotalSeconds <= 30)
                {
                    if (BruteForceAttackss.refreshcount > 20)
                    {
                        if (System.Web.HttpContext.Current.Session["EmailID"] != null)
                        {
                            IEnumerable<LoginViewModel> model = null;
                            using (var client2 = new HttpClient())
                            {
                                client2.DefaultRequestHeaders.Clear();
                                client2.BaseAddress = new Uri(WebAPIUrl);
                                client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                                    parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                                HttpResponseMessage response = client2.GetAsync(requestUri: "Account/GetUserLoginBlock?EmailId=" + System.Web.HttpContext.Current.Session["EmailID"].ToString()).Result;
                                if (response.IsSuccessStatusCode)
                                {
                                    LoginViewModel model1 = new LoginViewModel();
                                    model = response.Content.ReadAsAsync<IEnumerable<LoginViewModel>>().Result;
                                    if (model.First().Message == "Blocked")
                                    {
                                        System.Web.HttpContext.Current.Response.Redirect("/Account/Logout");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        BruteForceAttackss.refreshcount = BruteForceAttackss.refreshcount + 1;
                    }
                }
                else
                {
                    if (BruteForceAttackss.refreshcount > 20)
                    {
                        if (System.Web.HttpContext.Current.Session["EmailID"] != null)
                        {
                            IEnumerable<LoginViewModel> model = null;
                            using (var client2 = new HttpClient())
                            {
                                client2.DefaultRequestHeaders.Clear();
                                client2.BaseAddress = new Uri(WebAPIUrl);
                                client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                                    parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                                HttpResponseMessage response = client2.GetAsync(requestUri: "Account/GetUserLoginBlock?EmailId=" + System.Web.HttpContext.Current.Session["EmailID"].ToString()).Result;
                                if (response.IsSuccessStatusCode)
                                {
                                    LoginViewModel model1 = new LoginViewModel();
                                    model = response.Content.ReadAsAsync<IEnumerable<LoginViewModel>>().Result;
                                    if (model.First().Message == "Blocked")
                                    {
                                        System.Web.HttpContext.Current.Response.Redirect("/Account/Logout");

                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        BruteForceAttackss.refreshcount = BruteForceAttackss.refreshcount + 1;
                    }
                }

            }
        }
        HtmlSanitizer sanitizer = new HtmlSanitizer();
        private static string WebAPIUrl = ConfigurationManager.AppSettings["APIUrl"].ToString();
        

        // GET: validation Check
        [Route("validationchecker")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult validationcheck()
        {
            IEnumerable<ViewModel.EscalationReportData> listdata = new List<ViewModel.EscalationReportData>();

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
                    listdata = response.Content.ReadAsAsync<IEnumerable<ViewModel.EscalationReportData>>().Result;
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
                foreach (var item in model)
                {
                    string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/EscalationEmailFormat.html"));
                    IEnumerable<EscalationReportData> listdata = new List<EscalationReportData>();
                    listdata = (IEnumerable<ViewModel.EscalationReportData>)Session["Escdata"];
                    EscalationReportData lrepot = listdata.Where(x => x.aon_id == item.aon_id && x.MSG_TYPE==item.MSG_TYPE).FirstOrDefault();
                    //string message = draftMsg.DraftMessage_L1;
                    //message = message.Replace("{date}", lrepot.Date_of_Accord_of_AoN.Value.AddDays(Convert.ToInt32(item.dap_timeline) * 7).ToString("MM/dd/yyyy"));
                    if(!string.IsNullOrEmpty(lrepot.L1_Officer_Email) && lrepot.L1_Officer_Email!="N/A")
                    {
                        EmailHelper.sendEmailEscalation(lrepot.L1_Officer_Email, lrepot.msg, mailPath);
                    }

                    if (!string.IsNullOrEmpty(lrepot.L2_Officer_Email) && lrepot.L2_Officer_Email != "N/A")
                    {
                        EmailHelper.sendEmailEscalation(lrepot.L2_Officer_Email, lrepot.msg, mailPath);
                    }
                    if (!string.IsNullOrEmpty(lrepot.L3_Officer_Email) && lrepot.L3_Officer_Email != "N/A")
                    {
                        EmailHelper.sendEmailEscalation(lrepot.L3_Officer_Email, lrepot.msg, mailPath);
                    }
                    if (!string.IsNullOrEmpty(lrepot.L4_Officer_Email) && lrepot.L4_Officer_Email != "N/A")
                    {
                        EmailHelper.sendEmailEscalation(lrepot.L4_Officer_Email, lrepot.msg, mailPath);
                    }

                    if (!string.IsNullOrEmpty(lrepot.L5_ADGAcq_Email) && lrepot.L5_ADGAcq_Email != "N/A")
                    {
                        EmailHelper.sendEmailEscalation(lrepot.L5_ADGAcq_Email, lrepot.msg, mailPath);
                    }

                    if (!string.IsNullOrEmpty(lrepot.L6_JS_AM_Email) && lrepot.L6_JS_AM_Email != "N/A")
                    {
                        EmailHelper.sendEmailEscalation(lrepot.L6_JS_AM_Email, lrepot.msg, mailPath);
                    }

                    if (!string.IsNullOrEmpty(lrepot.L7_FM_Email) && lrepot.L7_FM_Email != "N/A")
                    {
                        EmailHelper.sendEmailEscalation(lrepot.L7_FM_Email, lrepot.msg, mailPath);
                    }

                    if (!string.IsNullOrEmpty(lrepot.L8_DG_Acq_Email) && lrepot.L8_DG_Acq_Email != "N/A")
                    {
                        EmailHelper.sendEmailEscalation(lrepot.L8_DG_Acq_Email, lrepot.msg, mailPath);
                    }

                    if (!string.IsNullOrEmpty(lrepot.L9_AS_FA_Email) && lrepot.L9_AS_FA_Email != "N/A")
                    {
                        EmailHelper.sendEmailEscalation(lrepot.L9_AS_FA_Email, lrepot.msg, mailPath);
                    }
                }
            }
            return Json(new { Status = true, Message="success" }, JsonRequestBehavior.AllowGet);
        }

        [Route("searchescalationdata")]
        [HandleError]
        [ValidateAntiForgeryToken]
        public JsonResult searchescalationdata(string startdate,string enddate)
        {
            datesearch result = new datesearch();
            if(Session["Escdata"]!=null)
            {
                try
                {
                    IEnumerable<EscalationReportData> listdata = new List<EscalationReportData>();
                    listdata = (IEnumerable<ViewModel.EscalationReportData>)Session["Escdata"];
                    var sdate = DateTime.ParseExact(startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture); 
                    var edate = DateTime.ParseExact(enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var data = listdata.Where(x => x.date_of_alert >= sdate && x.date_of_alert <= edate).ToList();
                    if (data != null && data.Count() > 0)
                    {
                        result.startdate = Convert.ToDateTime(startdate);
                        result.enddate = Convert.ToDateTime(enddate);
                        result.data = data;
                        result.Status = true;
                    }
                    else
                    {
                        result.startdate = Convert.ToDateTime(startdate);
                        result.enddate = Convert.ToDateTime(enddate);
                        result.Status = false;
                    }
                }
                catch(Exception ex)
                {
                    result.Status = false;
                    result.startdate = Convert.ToDateTime(startdate);
                    result.enddate = Convert.ToDateTime(enddate);
                }
            }
            else result.Status = false;

            return Json(new { result= result }, JsonRequestBehavior.AllowGet);
        }

    }
}