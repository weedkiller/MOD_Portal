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
using System.Web.UI.WebControls;
using System.Web.UI;
using OfficeOpenXml;

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
        public ActionResult validationcheck(bool isAlertsent = false)
        {
            IEnumerable<EscalationReportData> listdata = new List<EscalationReportData>();
            if (isAlertsent)
            {

                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(WebAPIUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                             parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                    HttpResponseMessage response = client.GetAsync("Escalation/GetProactiveAletReport").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        listdata = response.Content.ReadAsAsync<IEnumerable<EscalationReportData>>().Result;
                        Session["previousAlert"] = null;
                        Session["previousAlert"] = listdata;
                    }
                }


            }
            else
            {

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

            }
            ViewBag.isalertsent = isAlertsent;

            return View(listdata);
        }

        [Route("sendEscalationEmail")]
        [HandleError]
        [ValidateAntiForgeryToken]
        public JsonResult sendEscalationEmail(List<EscalationReportData> model)
        {
            if (model != null && model.Count() > 0)
            {
                foreach (var item in model)
                {
                    string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/EscalationEmailFormat.html"));
                    IEnumerable<EscalationReportData> listdata = new List<EscalationReportData>();
                    listdata = (IEnumerable<ViewModel.EscalationReportData>)Session["Escdata"];
                    EscalationReportData lrepot = listdata.Where(x => x.aon_id == item.aon_id && x.MSG_TYPE == item.MSG_TYPE).FirstOrDefault();
                    //string message = draftMsg.DraftMessage_L1;
                    //message = message.Replace("{date}", lrepot.Date_of_Accord_of_AoN.Value.AddDays(Convert.ToInt32(item.dap_timeline) * 7).ToString("MM/dd/yyyy"));
                    if (!string.IsNullOrEmpty(lrepot.L1_Officer_Email) && lrepot.L1_Officer_Email != "N/A")
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
            return Json(new { Status = true, Message = "success" }, JsonRequestBehavior.AllowGet);
        }

        [Route("searchescalationdata")]
        [HandleError]
        [ValidateAntiForgeryToken]
        public JsonResult searchescalationdata(string startdate, string enddate)
        {
            datesearch result = new datesearch();
            if (Session["Escdata"] != null)
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
                        result.startdate = Convert.ToDateTime(sdate);
                        result.enddate = Convert.ToDateTime(edate);
                        result.data = data;
                        result.Status = true;
                    }
                    else
                    {
                        result.startdate = Convert.ToDateTime(sdate);
                        result.enddate = Convert.ToDateTime(edate);
                        result.Status = false;
                    }
                }
                catch (Exception ex)
                {
                    var sdate = DateTime.ParseExact(startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var edate = DateTime.ParseExact(enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    result.Status = false;
                    result.startdate = Convert.ToDateTime(sdate);
                    result.enddate = Convert.ToDateTime(edate);
                }
            }
            else result.Status = false;

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }


        [Route("searchsentalert")]
        [HandleError]
        [ValidateAntiForgeryToken]
        public JsonResult searchsentalert(string startdate, string enddate)
        {
            datesearch result = new datesearch();
            if (Session["previousAlert"] != null)
            {
                try
                {
                    IEnumerable<EscalationReportData> listdata = new List<EscalationReportData>();
                    listdata = (IEnumerable<ViewModel.EscalationReportData>)Session["previousAlert"];
                    var sdate = DateTime.ParseExact(startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var edate = DateTime.ParseExact(enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var data = listdata.Where(x => x.date_of_alert >= sdate && x.date_of_alert <= edate).ToList();
                    if (data != null && data.Count() > 0)
                    {
                        result.startdate = Convert.ToDateTime(sdate);
                        result.enddate = Convert.ToDateTime(edate);
                        result.data = data;
                        result.Status = true;
                    }
                    else
                    {
                        result.startdate = Convert.ToDateTime(sdate);
                        result.enddate = Convert.ToDateTime(edate);
                        result.Status = false;
                    }
                }
                catch (Exception ex)
                {
                    var sdate = DateTime.ParseExact(startdate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    var edate = DateTime.ParseExact(enddate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    result.Status = false;
                    result.startdate = Convert.ToDateTime(sdate);
                    result.enddate = Convert.ToDateTime(edate);
                }
            }
            else result.Status = false;

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }

        [Route("getdataforEdit")]
        [HandleError]
        [ValidateAntiForgeryToken]
        public JsonResult getdataforEdit(EscalationReportData model)
        {
            EscalationReportData result = new EscalationReportData();
            if (Session["Escdata"] != null)
            {
                IEnumerable<EscalationReportData> listdata = new List<EscalationReportData>();
                listdata = (IEnumerable<ViewModel.EscalationReportData>)Session["Escdata"];
                var id = Convert.ToInt32(sanitizer.Sanitize(model.Id.ToString()));

                var data = listdata.Where(x => x.Id == id).FirstOrDefault();
                result = data;
            }
            if (result != null)
            {
                return Json(new { Status = true, result = result }, JsonRequestBehavior.AllowGet);
            }
            else return Json(new { Status = false, result = result }, JsonRequestBehavior.AllowGet);
        }

        [Route("UpdateEscalationMessage")]
        [HandleError]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateEscalationMessage(EscalationReportData model)
        {
            bool result = false;
            try
            {
                if (Session["UserID"] != null)
                {
                    model.aon_id = Convert.ToInt32(sanitizer.Sanitize(model.aon_id.ToString()));
                    model.Id = Convert.ToInt32(sanitizer.Sanitize(model.Id.ToString()));
                    model.MSG_TYPE = sanitizer.Sanitize(model.MSG_TYPE);
                    model.msg = sanitizer.Sanitize(model.msg);
                    var id = Session["UserID"].ToString();
                    model.Modified_by = id;
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.BaseAddress = new Uri(WebAPIUrl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                                 parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                        HttpResponseMessage response = await client.PostAsJsonAsync<EscalationReportData>(WebAPIUrl + "Escalation/UpdateEscalationDraft", model);

                        if (response.IsSuccessStatusCode)
                        {

                            result = true;
                        }
                        else
                        {

                            result = false;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                result = false;
            }

            return Json(new { Status = result, Message = result ? "Updated successfully" : "Update failed! server connection failed." }, JsonRequestBehavior.AllowGet);

        }

        [Route("ExportToExcel")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ExportToExcel()
        {
            if (Session["Escdata"] != null)
            {
                IEnumerable<EscalationReportData> listdata = new List<EscalationReportData>();
                listdata = (IEnumerable<EscalationReportData>)Session["Escdata"];

                List<ExportExcel> exceldata = new List<ExportExcel>();
                foreach (var item in listdata.ToList())
                {
                    if (item.L1_Officer_Phone != null && item.L1_Officer_Phone != "N/A")
                    {
                        ExportExcel l1 = new ExportExcel();
                        l1.Message = item.msg;
                        l1.Phone = item.L1_Officer_Phone;
                        l1.TemplateId = string.IsNullOrEmpty(item.TemplateId) == true ? "" : item.TemplateId;
                        exceldata.Add(l1);
                    }
                    if (item.L2_Officer_Phone != null && item.L2_Officer_Phone != "N/A")
                    {
                        ExportExcel l2 = new ExportExcel();
                        l2.Message = item.msg;
                        l2.Phone = item.L2_Officer_Phone;
                        l2.TemplateId = string.IsNullOrEmpty(item.TemplateId) == true ? "" : item.TemplateId;
                        exceldata.Add(l2);
                    }

                    if (item.L3_Officer_Phone != null && item.L3_Officer_Phone != "N/A")
                    {
                        ExportExcel l3 = new ExportExcel();
                        l3.Message = item.msg;
                        l3.Phone = item.L3_Officer_Phone;
                        l3.TemplateId = string.IsNullOrEmpty(item.TemplateId) == true ? "" : item.TemplateId;
                        exceldata.Add(l3);
                    }

                    if (item.L4_Officer_Phone != null && item.L4_Officer_Phone != "N/A")
                    {
                        ExportExcel l4 = new ExportExcel();
                        l4.Message = item.msg;
                        l4.Phone = item.L4_Officer_Phone;
                        l4.TemplateId = string.IsNullOrEmpty(item.TemplateId) == true ? "" : item.TemplateId;
                        exceldata.Add(l4);
                    }

                }

                if (exceldata.Count() > 0)
                {
                    ExcelPackage Ep = new ExcelPackage();
                    ExcelWorksheet Sheet = Ep.Workbook.Worksheets.Add("ProactiveAlertsReport");

                    for (var i = 0; i < exceldata.Count(); i++)
                    {
                        Sheet.Cells[string.Format("A{0}", i + 1)].Value = exceldata[i].Phone;
                        Sheet.Cells[string.Format("B{0}", i + 1)].Value = exceldata[i].TemplateId;
                        Sheet.Cells[string.Format("C{0}", i + 1)].Value = exceldata[i].Message;
                    }


                    Sheet.Cells["A:AZ"].AutoFitColumns();
                    Response.Clear();
                    Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    Response.AddHeader("content-disposition", "attachment: filename=" + "ProactiveAlertsReport.xlsx");
                    Response.BinaryWrite(Ep.GetAsByteArray());
                    Response.End();

                }


            }

            return View();

        }

    }
}