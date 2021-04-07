﻿using ACQ.Web.Core.Library;
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
        public JsonResult sendEscalationEmail(List<ViewModel.EscalationReportData> model)
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
                            string draftMsg = response.Content.ReadAsAsync<string>().Result;
                            string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/EscalationEmailFormat.html"));
                            IEnumerable<ViewModel.EscalationReportData> listdata = new List<ViewModel.EscalationReportData>();
                            listdata=(IEnumerable<ViewModel.EscalationReportData>)Session["Escdata"];
                            string email = listdata.Where(x => x.aon_id == item.aon_id).FirstOrDefault().Responsible_Level1;

                            EmailHelper.sendEmailEscalation(email, draftMsg, mailPath);

                        }
                    }
                }
            }
            return Json(new { Status = true, Message="success" }, JsonRequestBehavior.AllowGet);
        }



    }
}