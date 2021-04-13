﻿using ACQ.Web.ViewModel.AONW;
using ACQ.Web.ViewModel.EFile;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using ACQ.Web.ExternalServices.SecurityAudit;
using ACQ.Web.ExternalServices.Email;
using Newtonsoft.Json;
using Ganss.XSS;
using static ACQ.Web.App.MvcApplication;
using ACQ.Web.ViewModel.User;
using ACQ.Web.App.ViewModel;
using static ACQ.Web.App.Controllers.SocPdfRegistrationController;
using System.Text;
using System.Security.Cryptography;

namespace ACQ.Web.App.Controllers
{
    //[Authorize]
    public class AONWController : Controller
    {
        HtmlSanitizer sanitizer = new HtmlSanitizer();

        private static string UploadPath = ConfigurationManager.AppSettings["SOCImagePath"].ToString();
        private static string UploadfilePath = ConfigurationManager.AppSettings["SOCPath"].ToString();
        private static string WebAPIUrl = ConfigurationManager.AppSettings["APIUrl"].ToString();
        private string baseUrl = ConfigurationManager.AppSettings["baseUrl"].ToString();
        private string InitVector = @"qwertyui";
        List<Efile.FileDetail> fileDetails = new List<Efile.FileDetail>();
        List<Efile.FileDetail> fileDetailsA = new List<Efile.FileDetail>();
        List<AttachmentViewModel> fileDetailsF = new List<AttachmentViewModel>();

        public AONWController()
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

        // GET: AONW
        [Route("Index")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        #region Index HomePage
        public ActionResult Index(SAVESOCVIEWMODEL _model)
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("Index");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model.roleList.Count != 0)
                            {
                                return View(_model);
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }
            return View();
        }
        #endregion
        #region Main SOC Page All Code
        [Route("ViewSOC")]
        [HandleError]
        [HandleError(ExceptionType = typeof(NullReferenceException), Master = "Account", View = "Error")]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ViewSOCRegistration()
        {
<<<<<<< HEAD
            if (Session["UserID"] != null)
=======
            string mSercive = "";
            if (Session["Department"].ToString() == "IDS")
            {
                mSercive = "Joint Staff";
            }
            else
            {
                mSercive = Session["Department"].ToString();
            }
            SAVESOCVIEWMODEL Socmodel = new SAVESOCVIEWMODEL();
            List<SAVESOCVIEWMODEL> listData = new List<SAVESOCVIEWMODEL>();
            using (var client = new HttpClient())
>>>>>>> 021776b656087bef15790c272a94c3531d5fdfb6
            {
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData = new List<AddFormMenuViewModel>();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                List<roleViewModel> listData2 = new List<roleViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("ViewSOCRegistration");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model.roleList.Count != 0)
                            {
                                string mSercive = "";
                                if (Session["Department"].ToString() == "IDS")
                                {
                                    mSercive = "Joint Staff";
                                }
                                else
                                {
                                    mSercive = Session["Department"].ToString();
                                }
                                SAVESOCVIEWMODEL Socmodel = new SAVESOCVIEWMODEL();
                                List<SAVESOCVIEWMODEL> listData5 = new List<SAVESOCVIEWMODEL>();
                                using (var client = new HttpClient())
                                {
                                    client.DefaultRequestHeaders.Clear();
                                    client.BaseAddress = new Uri(WebAPIUrl);
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                                             parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                                    HttpResponseMessage response1 = client.GetAsync("AONW/ViewSOCRegistration").Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        listData5 = response1.Content.ReadAsAsync<List<SAVESOCVIEWMODEL>>().Result;

                                    }
                                }

                                if (mSercive != "Acquisition Wing")
                                {
                                    Socmodel.SOCVIEW = listData5.Where(x => x.Service_Lead_Service == mSercive).ToList();
                                }
                                else
                                {
                                    Socmodel.SOCVIEW = listData5;
                                }
                                return View(Socmodel);
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View(model);
            }

            return View();

        }

        [Route(" ViewSOCComment")]
        [HandleError]
        [HandleError(ExceptionType = typeof(NullReferenceException), Master = "Account", View = "Error")]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ViewSOCComment()
        {

            if (Session["UserID"] != null)
            {
<<<<<<< HEAD
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
=======
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/GetCommentt?ID=" + id + "").Result;
                if (response.IsSuccessStatusCode)
>>>>>>> 021776b656087bef15790c272a94c3531d5fdfb6
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("ViewSOCComment");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model.roleList.Count != 0)
                            {
                                SocCommentViewModel Socmodel = new SocCommentViewModel();
                                List<SocCommentViewModel> listData = new List<SocCommentViewModel>();
                                var id = Session["UserID"].ToString();
                                using (var client = new HttpClient())
                                {
                                    client.BaseAddress = new Uri(WebAPIUrl);
                                    //HTTP GET
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                    HttpResponseMessage response1 = client.GetAsync("AONW/GetCommentt?ID=" + id + "").Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        listData = response1.Content.ReadAsAsync<List<SocCommentViewModel>>().Result;

                                    }
                                }

                                Socmodel.SOCVIEWComment = listData;
                                return View(Socmodel);
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View(model);
            }

            return View();
        }

        [Route("ViewSocSendMail")]
        [HandleError]
        [HandleError(ExceptionType = typeof(NullReferenceException), Master = "Account", View = "Error")]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ViewSocSendMail(string ID)
        {
            if (Session["UserID"] != null)
            {
<<<<<<< HEAD
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
=======
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/GetSendMail?ID=" + ID + "").Result;
                if (response.IsSuccessStatusCode)
>>>>>>> 021776b656087bef15790c272a94c3531d5fdfb6
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("ViewSocSendMail");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model.roleList.Count != 0)
                            {
                                acqmstmemberSendMailViewModel Socmodel = new acqmstmemberSendMailViewModel();
                                List<acqmstmemberSendMailViewModel> listData = new List<acqmstmemberSendMailViewModel>();
                                ID = Encryption.Decrypt(ID);
                                using (var client = new HttpClient())
                                {
                                    client.BaseAddress = new Uri(WebAPIUrl);
                                    //HTTP GET
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                    HttpResponseMessage response1 = client.GetAsync("AONW/GetSendMail?ID=" + ID + "").Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        Session["id"] = ID;
                                        listData = response1.Content.ReadAsAsync<List<acqmstmemberSendMailViewModel>>().Result;

                                    }
                                }

                                Socmodel.SOCMailVIEW = listData;
                                return View(Socmodel);
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }

            return View();
        }
        [Route("SendMailToAll")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult SendMailToAll()
        {
         
            acqmstmemberSendMailViewModel Socmodel = new acqmstmemberSendMailViewModel();
            List<acqmstmemberSendMailViewModel> listData = new List<acqmstmemberSendMailViewModel>();
            var id = Session["id"].ToString();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/GetSendMail?ID=" + id + "").Result;
                if (response.IsSuccessStatusCode)
                {
                    listData = response.Content.ReadAsAsync<List<acqmstmemberSendMailViewModel>>().Result;
                    foreach (var dummyList in listData)
                    {
                        string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/SendOTPMailFormat.html"));
                        EmailHelper.SendAllDetails(dummyList.Email, mailPath);
                        ViewBag.Message = "RegistrationSuccessful";
                    }


                }
            }

            Socmodel.SOCMailVIEW = listData;
            //return RedirectToAction("ViewSOCRegistration");
            return Json("Success", JsonRequestBehavior.AllowGet);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("WAonCreate")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public async Task<ActionResult> WAonCreate(SAVESOCVIEWMODEL model, HttpPostedFileBase file)
        {
            
            try
            {
                if (Session["UserID"] != null)
                {
                    model.CreatedBy = Convert.ToInt32(Session["UserID"]);
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(WebAPIUrl + "AONW/AONWCreate");
                        HttpResponseMessage postJob = await client.PostAsJsonAsync<SAVESOCVIEWMODEL>(WebAPIUrl + "AONW/AONWCreate", model);
                        string url = postJob.Headers.Location.AbsoluteUri;
                        string mID = postJob.Headers.Location.Segments[4].ToString();
                        bool postResult = postJob.IsSuccessStatusCode;
                        if (postResult == true)
                        {
                            if (Request.Files.Count > 0)
                            {
                                System.Web.HttpPostedFileBase fileSoC = Request.Files[0];
                                System.Web.HttpPostedFileBase fileSoCAnnexure = Request.Files[1];
                                System.Web.HttpPostedFileBase fileSoCPPT = Request.Files[2];
                                System.Web.HttpPostedFileBase fileSoCOther = Request.Files[3];

                                if (fileSoC.ContentLength > 0)
                                {
                                    string filename = mID + "_" + "SOC" + "_" + fileSoC.FileName;
                                    ViewBag.SOC = fileSoC.FileName;

                                    string FileExtension = Path.GetExtension(fileSoC.FileName);
                                    string fullpath = Server.MapPath(UploadfilePath) + filename;

                                    using (HttpClient client1 = new HttpClient())
                                    {
                                        AttachmentViewModel objattach = new AttachmentViewModel();
                                        objattach.aon_id = Convert.ToInt32(mID);
                                        objattach.AttachmentFileName = filename;
                                        objattach.Path = fullpath;
                                        objattach.RecDate = DateTime.Now;
                                        objattach.RefId = 2;
                                        client1.BaseAddress = new Uri(WebAPIUrl + "AONW/SaveAttachFile");
                                        HttpResponseMessage postJob1 = await client.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/SaveAttachFile", objattach);
                                        postResult = postJob1.IsSuccessStatusCode;
                                    }

                                }
                                if (fileSoCAnnexure.ContentLength > 0)
                                {
                                    string filename = mID + "_" + "SOCAnnexure" + "_" + fileSoCAnnexure.FileName;
                                    ViewBag.SOC = fileSoCAnnexure.FileName;

                                    string FileExtension = Path.GetExtension(fileSoCAnnexure.FileName);
                                    string fullpath = Server.MapPath(UploadfilePath) + filename;

                                    using (HttpClient client1 = new HttpClient())
                                    {
                                        AttachmentViewModel objattach = new AttachmentViewModel();
                                        objattach.aon_id = Convert.ToInt32(mID);
                                        objattach.AttachmentFileName = filename;
                                        objattach.Path = fullpath;
                                        objattach.RecDate = DateTime.Now;
                                        objattach.RefId = 3;
                                        client1.BaseAddress = new Uri(WebAPIUrl + "AONW/SaveAttachFile");
                                        HttpResponseMessage postJob1 = await client.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/SaveAttachFile", objattach);
                                        postResult = postJob1.IsSuccessStatusCode;
                                    }

                                }
                                if (fileSoCPPT.ContentLength > 0)
                                {
                                    string filename = mID + "_" + "SOCPPT" + "_" + fileSoCPPT.FileName;
                                    ViewBag.SOCPPT = fileSoCPPT.FileName;

                                    string FileExtension = Path.GetExtension(fileSoCPPT.FileName);
                                    string fullpath = Server.MapPath(UploadfilePath) + filename;

                                    using (HttpClient client1 = new HttpClient())
                                    {
                                        AttachmentViewModel objattach = new AttachmentViewModel();
                                        objattach.aon_id = Convert.ToInt32(mID);
                                        objattach.AttachmentFileName = filename;
                                        objattach.Path = fullpath;
                                        objattach.RecDate = DateTime.Now;
                                        objattach.RefId = 4;
                                        client1.BaseAddress = new Uri(WebAPIUrl + "AONW/SaveAttachFile");
                                        HttpResponseMessage postJob1 = await client.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/SaveAttachFile", objattach);
                                        postResult = postJob1.IsSuccessStatusCode;
                                    }
                                }
                                if (fileSoCOther.ContentLength > 0)
                                {
                                    string filename = mID + "_" + "SOCOther" + "_" + fileSoCOther.FileName;
                                    ViewBag.SoCOther = fileSoCOther.FileName;

                                    string FileExtension = Path.GetExtension(fileSoCOther.FileName);
                                    string fullpath = Server.MapPath(UploadfilePath) + filename;
                                    using (HttpClient client1 = new HttpClient())
                                    {
                                        AttachmentViewModel objattach = new AttachmentViewModel();
                                        objattach.aon_id = Convert.ToInt32(mID);
                                        objattach.AttachmentFileName = filename;
                                        objattach.Path = fullpath;
                                        objattach.RecDate = DateTime.Now;
                                        objattach.RefId = 5;
                                        client1.BaseAddress = new Uri(WebAPIUrl + "AONW/SaveAttachFile");
                                        HttpResponseMessage postJob1 = await client.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/SaveAttachFile", objattach);
                                        postResult = postJob1.IsSuccessStatusCode;
                                    }
                                }
                            }
                            else
                            {
                                model.ErrorMsg = "Can not be empty SoC meta data";
                                ViewBag.UploadStatus = "Can not be empty SoC meta data";
                                return View("Index", model);
                            }
                            ViewBag.Save = "SOC Registration Successfully";
                            model.ErrorMsg = "SOC Registration Successfully";
                            return View("Index", model);
                        }
                        else
                        {
                            ViewBag.Save = "SOC registration not saved";
                            model.ErrorMsg = "SOC registration not saved";
                            return View("Index", model);
                        }
                    }
                }
                else { return Redirect("/account/login"); }
            }
            catch (Exception)
            {
                ViewBag.NotSave = "Can not empty SOC fields";
                model.ErrorMsg = "Can not empty SOC fields";
                return View("Index", model);
            }
        }
        [Route("ViewSocMaster")]
        [HandleError]
        [HandleError(ExceptionType = typeof(NullReferenceException), Master = "Account", View = "Error")]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ViewSocMaster(string ID, string mtype)
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model1 = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("ViewSocMaster");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model1 = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model1.roleList.Count != 0)
                            {
                                try
                                {
                                    SAVESOCVIEWMODEL model = new SAVESOCVIEWMODEL();
                                    using (var client = new HttpClient())
                                    {
                                        client.BaseAddress = new Uri(WebAPIUrl);
                                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                                              parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                        HttpResponseMessage response = client.GetAsync("AONW/EditSocMaster?ID=" + Encryption.Decrypt(ID) + "").Result;
                                        if (response.IsSuccessStatusCode)
                                        {
                                            model = response.Content.ReadAsAsync<SAVESOCVIEWMODEL>().Result;
                                            model.item_description = Encryption.Decrypt(model.item_description);
                                            ViewBag.item = Encryption.Encrypt(model.item_description);
                                            model.Quantity = Encryption.Decrypt(model.Quantity);
                                            model.Cost = Encryption.Decrypt(model.Cost);
                                            model.Categorisation = Encryption.Decrypt(model.Categorisation);
                                            model.Service_Lead_Service = Encryption.Decrypt(model.Service_Lead_Service);
                                            model.AoN_Accorded_By = Encryption.Decrypt(model.AoN_Accorded_By);
                                            model.SystemCase = Encryption.Decrypt(model.SystemCase);
                                            model.SoCCase = Encryption.Decrypt(model.SoCCase);
                                            model.IC_percentage = Encryption.Decrypt(model.IC_percentage);
                                            model.Essential_parameters = Encryption.Decrypt(model.Essential_parameters);
                                            model.EPP = Encryption.Decrypt(model.EPP);
                                            model.Trials_Required = Encryption.Decrypt(model.Trials_Required);
                                            model.Offset_applicable = Encryption.Decrypt(model.Offset_applicable);
                                            model.Option_clause_applicable = Encryption.Decrypt(model.Option_clause_applicable);
                                            model.Warrenty_applicable = Encryption.Decrypt(model.Warrenty_applicable);
                                            model.Warrenty_Remarks = Encryption.Decrypt(model.Warrenty_Remarks);
                                            model.Any_other_aspect = Encryption.Decrypt(model.Any_other_aspect);
                                            model.SocAName = Encryption.Decrypt(model.SocAName);
                                            model.SocADesignation = Encryption.Decrypt(model.SocADesignation);
                                            model.SocAApprovalRef = Encryption.Decrypt(model.SocAApprovalRef);
                                            model.SocAApprovalDate = Encryption.Decrypt(model.SocAApprovalDate);
                                            model.SocSDName = Encryption.Decrypt(model.SocSDName);
                                            model.SocSDDesignation = Encryption.Decrypt(model.SocSDDesignation);
                                            model.SocSDPhone = Encryption.Decrypt(model.SocSDPhone);
                                            model.SocSDDate = Encryption.Decrypt(model.SocSDDate);
                                        }

                                    }


                                    List<AttachmentViewModel> listData = new List<AttachmentViewModel>();
                                    if (mtype != null)
                                    {
                                        using (var client = new HttpClient())
                                        {
                                            client1.BaseAddress = new Uri(WebAPIUrl);
                                            //HTTP GET
                                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                            HttpResponseMessage response = client.GetAsync("AONW/GetAttachFile?ID=" + Encryption.Decrypt(ID) + "").Result;
                                            if (response.IsSuccessStatusCode)
                                            {
                                                listData = response.Content.ReadAsAsync<List<AttachmentViewModel>>().Result;
                                            }
                                        }

                                        ViewBag.UploadStatus = "Comment";
                                    }
                                    else
                                    {
                                        TempData["FileF"] = null;
                                        if (TempData["FileA"] != null)
                                        {
                                            fileDetails = TempData["FileA"] as List<Efile.FileDetail>;
                                        }

                                        foreach (var m in fileDetails)
                                        {
                                            AttachmentViewModel fileDetail = new AttachmentViewModel()
                                            {
                                                AttachmentFileName = m.FileName,
                                                Path = m.FilePath,
                                                AttachmentID = Convert.ToInt16(m.Id),
                                            };
                                            listData.Add(fileDetail);
                                        }

                                        if (TempData["FileAA"] != null)
                                        {
                                            fileDetailsA = TempData["FileAA"] as List<Efile.FileDetail>;
                                        }

                                        foreach (var m in fileDetailsA)
                                        {
                                            AttachmentViewModel fileDetailA = new AttachmentViewModel()
                                            {
                                                AttachmentFileName = m.FileName,
                                                Path = m.FilePath,
                                                AttachmentID = Convert.ToInt16(m.Id),
                                            };
                                            listData.Add(fileDetailA);
                                        }

                                        TempData["FileF"] = listData;
                                        ViewBag.UploadStatus = "Review";
                                    }
                                    model.FileDetail = listData;

                                    return View(model);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }

            return View();
        }

        public bool FileCheckformat(HttpPostedFileBase file, string mFileExtension)
        {
            int filesize = 1024;
            string FileExtension = Path.GetExtension(file.FileName);
            int count = file.FileName.Count(f => f == '.');
            if (count > 1)
            {
                return false;
            }

            if (file.ContentLength > (filesize * 1024))
            {
                return false;
            }

            if (mFileExtension == ".doc")
            {
                if (file.ContentType != "application/msword" && file.ContentType != "application/vnd.openxmlformats-officedocument.wordprocessingml.document")
                {
                    return false;
                }

                if (FileExtension == ".doc" || FileExtension == ".docx")
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            else if (mFileExtension == ".ppt")
            {
                if (file.ContentType != "application/vnd.ms-powerpoint" || file.ContentType != "application/vnd.openxmlformats-officedocument.presentationml.presentation")
                {
                    return false;
                }
                if (FileExtension == ".ppt" || FileExtension == ".pptx")
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (file.ContentType != "application/pdf")
                {
                    return false;
                }
                if (FileExtension == ".pdf")
                {
                    return true;

                }
                else
                {
                    return false;
                }
            }

        }
        private void EncryptFile(string inputFile, string outputFile)
        {

            try
            {
                string password = @"myKey123"; // Your Key Here
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                string cryptFile = outputFile;
                FileStream fsCrypt = new FileStream(cryptFile, FileMode.Create);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateEncryptor(key, key),
                    CryptoStreamMode.Write);

                FileStream fsIn = new FileStream(inputFile, FileMode.Open);

                int data;
                while ((data = fsIn.ReadByte()) != -1)
                    cs.WriteByte((byte)data);

                fsIn.Close();
                cs.Close();
                fsCrypt.Close();
            }
            catch(Exception e)
            {
                
            }
        }

        private void  DecryptFile(string inputFile, string outputFile)
        {

            {
                string password = @"myKey123"; // Your Key Here

                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);

                FileStream fsCrypt = new FileStream(inputFile, FileMode.Open);

                RijndaelManaged RMCrypto = new RijndaelManaged();

                CryptoStream cs = new CryptoStream(fsCrypt,
                    RMCrypto.CreateDecryptor(key, key),
                    CryptoStreamMode.Read);

                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                    fsOut.WriteByte((byte)data);

                fsOut.Close();
                cs.Close();
                fsCrypt.Close();
            }
        }

        [Route("ViewApprovalDocs")]
        [HttpGet]
        public ActionResult ViewApprovalDocs(string ID)
        {
            int meeting_id = Convert.ToInt32(Encryption.Decrypt(ID));
            string path = "";
            string outputfile = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/ViewMOMApproval?meeting_id=" + meeting_id).Result;
                if (response.IsSuccessStatusCode)
                {

                    string result= response.Content.ReadAsStringAsync().Result;
                    result = result.Substring(1);
                    result = result.Replace('"', ' ');
                    string filename = result.Substring(result.LastIndexOf(@"\"));
                    filename = filename.Replace("\\", "");
                    outputfile = baseUrl + "decry_" + filename;
                    DecryptFile(result,outputfile);
                }

            }
            return Redirect(outputfile);
        }

        [Route("UploadDocs")]
        [HttpPost]
        public ActionResult UploadDocs(FormCollection collection)
        {
            Random rand = new Random();
            int userId = GetUserID();
            string meeting_id = collection["meeting_id"];
            HttpFileCollectionBase files = Request.Files;
            string path = UploadfilePath;
            string outputpath = "";
            for (int i = 0; i < files.Count; i++)
            {

                HttpPostedFileBase file = files[i];
                if (!FileCheckformat(file, ".pdf"))
                {
                    ViewBag.UploadStatusmsg = "Please upload only .pdf file and File size Should Be UpTo 1 MB";
                    ViewBag.UploadStatus = "errormsg";
                    return View();
                }
                outputpath = baseUrl+ "encry_"+file.FileName;
                string inPath = baseUrl+file.FileName;
                path += file.FileName;
                file.SaveAs(Server.MapPath(path));
                EncryptFile(inPath, outputpath);
            }
            
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/UpdateMOMApproval?ID=" + meeting_id + "&filepath="+outputpath+"&updatedby="+userId).Result;
                if (response.IsSuccessStatusCode)
                {
                    // model = response.Content.ReadAsAsync<SAVESOCVIEWMODEL>().Result;
                    TempData["Msg"] = "MOM Uploaded Successfully..!!";
                }

            }
            return RedirectToAction("UploadApprovalDocs");
        }
        [Route("EditSocMaster")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult EditSocMaster(int ID)
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("EditSocMaster");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model.roleList.Count != 0)
                            {
                                try
                                {
                                    SAVESOCVIEWMODEL model1 = new SAVESOCVIEWMODEL();
                                    using (var client = new HttpClient())
                                    {
                                        client.BaseAddress = new Uri(WebAPIUrl);
                                        //HTTP GET
                                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                        HttpResponseMessage response = client.GetAsync("AONW/EditSocMaster?ID=" + ID + "").Result;
                                        if (response.IsSuccessStatusCode)
                                        {
                                            model1 = response.Content.ReadAsAsync<SAVESOCVIEWMODEL>().Result;

                                        }

                                    }
                                    return View(model1);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }
            return View();

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        public async Task<ActionResult> EditSocMaster(SAVESOCVIEWMODEL _model)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(WebAPIUrl + "AONW/UpdateSocMaster1");
                        HttpResponseMessage postJob = await client.PostAsJsonAsync<SAVESOCVIEWMODEL>(WebAPIUrl + "AONW/UpdateSocMaster1", _model);
                        bool postResult = postJob.IsSuccessStatusCode;
                        if (postResult == true)
                        {
                            if (Request.Files.Count > 0)
                            {
                                System.Web.HttpPostedFileBase fileSoC = Request.Files[0];
                                System.Web.HttpPostedFileBase fileSoCAnnexure = Request.Files[1];
                                System.Web.HttpPostedFileBase fileSoCPPT = Request.Files[2];
                                System.Web.HttpPostedFileBase fileSoCOther = Request.Files[3];

                                if (fileSoC.ContentLength > 0)
                                {
                                    string filename = _model.aon_id.ToString() + "_" + "SOC" + "_" + fileSoC.FileName;
                                    ViewBag.SOC = fileSoC.FileName;

                                    string FileExtension = Path.GetExtension(fileSoC.FileName);
                                    string fullpath = Server.MapPath(UploadfilePath) + filename;

                                    using (HttpClient client1 = new HttpClient())
                                    {
                                        AttachmentViewModel objattach = new AttachmentViewModel();
                                        objattach.aon_id = 32;
                                        objattach.AttachmentFileName = filename;
                                        objattach.Path = fullpath;
                                        objattach.RecDate = DateTime.Now;
                                        objattach.RefId = 2;
                                        client1.BaseAddress = new Uri(WebAPIUrl + "AONW/UpdateAttachmentSOC");
                                        HttpResponseMessage postJob1 = await client.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/UpdateAttachmentSOC", objattach);
                                        postResult = postJob1.IsSuccessStatusCode;
                                    }

                                }
                                if (fileSoCAnnexure.ContentLength > 0)
                                {
                                    string filename = _model.aon_id.ToString() + "_" + "SOCAnnexure" + "_" + fileSoCAnnexure.FileName;
                                    ViewBag.SOC = fileSoCAnnexure.FileName;

                                    string FileExtension = Path.GetExtension(fileSoCAnnexure.FileName);
                                    string fullpath = Server.MapPath(UploadfilePath) + filename;

                                    using (HttpClient client1 = new HttpClient())
                                    {
                                        AttachmentViewModel objattach = new AttachmentViewModel();
                                        objattach.aon_id = Convert.ToInt32(_model.aon_id);
                                        objattach.AttachmentFileName = filename;
                                        objattach.Path = fullpath;
                                        objattach.RecDate = DateTime.Now;
                                        objattach.RefId = 3;
                                        client1.BaseAddress = new Uri(WebAPIUrl + "AONW/UpdateAttachmentSOC");
                                        HttpResponseMessage postJob1 = await client.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/UpdateAttachmentSOC", objattach);
                                        postResult = postJob1.IsSuccessStatusCode;
                                    }

                                }
                                if (fileSoCPPT.ContentLength > 0)
                                {
                                    string filename = _model.aon_id.ToString() + "_" + "SOCPPT" + "_" + fileSoCPPT.FileName;
                                    ViewBag.SOCPPT = fileSoCPPT.FileName;

                                    string FileExtension = Path.GetExtension(fileSoCPPT.FileName);
                                    string fullpath = Server.MapPath(UploadfilePath) + filename;

                                    using (HttpClient client1 = new HttpClient())
                                    {
                                        AttachmentViewModel objattach = new AttachmentViewModel();
                                        objattach.aon_id = Convert.ToInt32(_model.aon_id);
                                        objattach.AttachmentFileName = filename;
                                        objattach.Path = fullpath;
                                        objattach.RecDate = DateTime.Now;
                                        objattach.RefId = 4;
                                        client1.BaseAddress = new Uri(WebAPIUrl + "AONW/UpdateAttachmentSOC");
                                        HttpResponseMessage postJob1 = await client.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/UpdateAttachmentSOC", objattach);
                                        postResult = postJob1.IsSuccessStatusCode;
                                    }
                                }
                                if (fileSoCOther.ContentLength > 0)
                                {
                                    string filename = _model.aon_id.ToString() + "_" + "SOCOther" + "_" + fileSoCOther.FileName;
                                    ViewBag.SoCOther = fileSoCOther.FileName;

                                    string FileExtension = Path.GetExtension(fileSoCOther.FileName);
                                    string fullpath = Server.MapPath(UploadfilePath) + filename;
                                    using (HttpClient client1 = new HttpClient())
                                    {
                                        AttachmentViewModel objattach = new AttachmentViewModel();
                                        objattach.aon_id = Convert.ToInt32(_model.aon_id);
                                        objattach.AttachmentFileName = filename;
                                        objattach.Path = fullpath;
                                        objattach.RecDate = DateTime.Now;
                                        objattach.RefId = 5;
                                        client1.BaseAddress = new Uri(WebAPIUrl + "AONW/UpdateAttachmentSOC");
                                        HttpResponseMessage postJob1 = await client.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/UpdateAttachmentSOC", objattach);
                                        postResult = postJob1.IsSuccessStatusCode;
                                    }
                                }
                            }

                            ViewBag.Msgg = "Record Update Successfully";
                        }
                        else
                        {
                            ViewBag.Msgg = "Record Not Update Successfully";
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            // return RedirectToAction("AddMeetingAgenda", "AONW", new { id, ViewBag.mtype, ViewBag.dated });

            return View(_model);


        }
        [Route("SOCApproval")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult SOCApproval()
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("SOCApproval");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model.roleList.Count != 0)
                            {

                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }
            return View();
        }

        [Route("DeleteSoc")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult DeleteSocMaster(int ID)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/DeleteSocMasterByID?id=" + ID).Result;
                    if (response.IsSuccessStatusCode)
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("ViewSOCRegistration");
        }
        [Route("UpdateSocMaster")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public async Task<ActionResult> UpdateSocMaster(string ID)
        {
            try
            {
                Int16 mmID = Convert.ToInt16(Encryption.Decrypt(ID));
                SAVESOCVIEWMODELBluk model = new SAVESOCVIEWMODELBluk();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                     parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                    HttpResponseMessage response = client.GetAsync("AONW/EditSocMaster?ID=" + mmID + "").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        model = response.Content.ReadAsAsync<SAVESOCVIEWMODELBluk>().Result;
                        using (HttpClient client1 = new HttpClient())
                        {
                            client1.BaseAddress = new Uri(WebAPIUrl + "AONW/AONWCreateBulk");
                            HttpResponseMessage postJob = await client1.PostAsJsonAsync<SAVESOCVIEWMODELBluk>(WebAPIUrl + "AONW/AONWCreateBulk", model);
                            string url = postJob.Headers.Location.AbsoluteUri;
                            string mID = postJob.Headers.Location.Segments[4].ToString();
                            bool postResult = postJob.IsSuccessStatusCode;
                            if (postResult == true)
                            {
                                AttachmentViewModel objattach = new AttachmentViewModel();
                                if (TempData["FileF"] != null)
                                {
                                    fileDetailsF = TempData["FileF"] as List<AttachmentViewModel>;
                                }
                                for (int i = 0; fileDetailsF.Count > i; i++)
                                {
                                    objattach.aon_id = mmID;
                                    objattach.AttachmentFileName = fileDetailsF[i].AttachmentFileName;
                                    objattach.Path = fileDetailsF[i].Path;
                                    objattach.RecDate = DateTime.Now;
                                    objattach.RefId = fileDetailsF[i].RefId;
                                    using (var client2 = new HttpClient())
                                    {
                                        client2.BaseAddress = new Uri(WebAPIUrl + "AONW/SaveAttachFile");
                                        HttpResponseMessage postJob2 = await client2.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/SaveAttachFile", objattach);
                                        postResult = postJob2.IsSuccessStatusCode;
                                    }
                                }
                                return RedirectToAction("ViewSOCRegistration");
                            }
                        }
                    }

                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        #region Save Meeting Code All

        [Route("ViewMeeting")]
        [HandleError]
        [HandleError(ExceptionType = typeof(NullReferenceException), Master = "Account", View = "Error")]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ViewMeeting()
        {
<<<<<<< HEAD
            if(Session["UserID"]!=null)
            {
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData = new List<AddFormMenuViewModel>();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                List<roleViewModel> listData2 = new List<roleViewModel>();
                using (HttpClient client1 = new HttpClient())
=======
            try
            {
                SechduleMeetingAgedaViewModel Socmodel = new SechduleMeetingAgedaViewModel();
                List<SechduleMeetingAgedaViewModel> listData = new List<SechduleMeetingAgedaViewModel>();
                int UserID = GetUserID();

                using (var client = new HttpClient())
>>>>>>> 021776b656087bef15790c272a94c3531d5fdfb6
                {
                   
                    var  loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("ViewMeeting");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName +  "").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if(model.roleList.Count!=0)
                            {
                                try
                                {

                                    SechduleMeetingAgedaViewModel Socmodel = new SechduleMeetingAgedaViewModel();
                                    List<SechduleMeetingAgedaViewModel> listData3 = new List<SechduleMeetingAgedaViewModel>();
                                    int UserID = GetUserID();

                                    using (var client = new HttpClient())
                                    {
                                        client.BaseAddress = new Uri(WebAPIUrl);
                                        //HTTP GET
                                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                        HttpResponseMessage response1 = client.GetAsync("AONW/CreateMeetings?UserID=" + UserID).Result;
                                        if (response.IsSuccessStatusCode)
                                        {
                                            listData3 = response1.Content.ReadAsAsync<List<SechduleMeetingAgedaViewModel>>().Result;
                                        }
                                    }

                                    ViewBag.UserID = UserID;
                                    Socmodel.ListofMeeting = listData3;
                                    return View(Socmodel);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }

                         
                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View(model);
            }
            return View();

        }
        [Route("PrepareFinalMeeting")]
        [HandleError]
        [HandleError(ExceptionType = typeof(NullReferenceException), Master = "Account", View = "Error")]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult PrepareFinalMeeting(string id, string mtype, string dated)
        {
            using (var client = new HttpClient())
            {
                Int16 mmID = Convert.ToInt16(Encryption.Decrypt(id));
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/PrepareFinalMeeting?ID=" + mmID + "").Result;
                if (response.IsSuccessStatusCode)
                {

                }
            }
            return RedirectToAction("AddMeetingAgenda", new { id, mtype, dated });
        }
        [Route("createMeeting")]
        [HandleError]
        [HandleError(ExceptionType = typeof(NullReferenceException), Master = "Account", View = "Error")]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult createMeeting()
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("createMeeting");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model.roleList.Count != 0)
                            {
                                SetParticipantSession();
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }
           
            return View();
        }

        [Route("AddSocCommit")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult AddSocCommit(string id)
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("AddSocCommit");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model.roleList.Count != 0)
                            {
                                SocCommentViewModel model1 = new SocCommentViewModel();
                                try
                                {
                                    if (id != null)
                                    {
                                        Int16 mID = Convert.ToInt16(Encryption.Decrypt(id));
                                        model1.SoCId = Convert.ToInt32(sanitizer.Sanitize(mID.ToString()));
                                        // Session["item"] = Request.QueryString["item"].ToString();
                                        //if(Request != null && string.IsNullOrEmpty(Request.QueryString["item"]))
                                        //{
                                        ViewBag.item = Request.QueryString["item"].ToString();
                                        //  ViewBag.aonId = Encryption.Decrypt(Session["item"].ToString());
                                        ViewBag.aonId = Encryption.Decrypt(Request.QueryString["item"].ToString());
                                        ViewBag.id = id;
                                        // }
                                    }
                                }
                                catch (Exception EX)
                                {

                                }
                                return View(model1);
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }

            return View();
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        [SessionExpire]
        public async Task<ActionResult> AddSocCommit(SocCommentViewModel model)
        {
            string Res = "";
            if (ModelState.IsValid)
            {
                try
                {
                    model.Comments = sanitizer.Sanitize(model.Comments);
                    model.IsActive = "Y";
                    model.SocCommentID = 0;
                    model.UserID = Convert.ToInt16(Session["UserID"].ToString());
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(WebAPIUrl + "AONW/AddSocCommit");
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                          parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                        HttpResponseMessage postJob = await client.PostAsJsonAsync<SocCommentViewModel>(WebAPIUrl + "AONW/AddSocCommit", model);
                        bool postResult = postJob.IsSuccessStatusCode;
                        if (postResult == true)
                        {
                            Res = "Success";
                            ViewBag.Msg = "Record Saved Successfully";
                        }
                        else
                        {
                            Res = "Failed";
                            ViewBag.Msg = "Record Not Saved Successfully";
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            //  ViewBag.aonId = Session["item"].ToString();
            // return View(model);
            return Json(Res, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("SaveMeetings")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveMeetings(SechduleMeetingAgedaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    List<MeetingParticipants> listData = new List<MeetingParticipants>();
                    listData = Session["participants"] as List<MeetingParticipants>;

                    listData = listData.Where(x => model.officers_participated.Contains(x.UserID.ToString()) && x.meeting_type == model.dac_dpb).ToList();

                    model.Participants = new List<MeetingParticipants>();
                    model.Participants = listData;

                    model.Remarks = sanitizer.Sanitize(model.Remarks);
                    model.Meeting_Number = sanitizer.Sanitize(model.Meeting_Number);
                    model.officers_participated = sanitizer.Sanitize(model.officers_participated);
                    model.Distribution_List = sanitizer.Sanitize(model.Distribution_List);

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(WebAPIUrl + "AONW/SaveMeetings");
                        HttpResponseMessage postJob = await client.PostAsJsonAsync<SechduleMeetingAgedaViewModel>(WebAPIUrl + "AONW/SaveMeetings", model);
                        bool postResult = postJob.IsSuccessStatusCode;
                        if (postResult == true)
                        {
                            ViewBag.Msg = "Record Saved Successfully";
                        }
                        else
                        {
                            ViewBag.Msg = "Record Not Saved Successfully";
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return View("createMeeting");
        }
        [Route("GenerateReport")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult GenerateReport(string ID, string Version = null)
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model1 = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("GenerateReport");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model1 = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model1.roleList.Count != 0)
                            {
                                Int16 mID = Convert.ToInt16(Encryption.Decrypt(ID));

                                SechduleMeetingAgedaViewModel model = new SechduleMeetingAgedaViewModel();
                                using (var client = new HttpClient())
                                {
                                    client.BaseAddress = new Uri(WebAPIUrl);
                                    //HTTP GET
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                    HttpResponseMessage response = client.GetAsync("AONW/EditMeeting?ID=" + mID + "").Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        model = response.Content.ReadAsAsync<SechduleMeetingAgedaViewModel>().Result;
                                        model.meeting_id = mID.ToString();

                                    }
                                }
                                MeetingAgenda Socmodel = new MeetingAgenda();
                                List<MeetingAgenda> listData = new List<MeetingAgenda>();
                                using (var client = new HttpClient())
                                {
                                    client.BaseAddress = new Uri(WebAPIUrl);
                                    //HTTP GET
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                    HttpResponseMessage response = client.GetAsync("AONW/BindMeetingAgenda?ID=" + mID + "&Version=" + Version).Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        listData = response.Content.ReadAsAsync<List<MeetingAgenda>>().Result;

                                        listData.ForEach(f =>
                                        {
                                            f.MeetingAgendaDateString = f.MeetingAgendaDate.HasValue ? f.MeetingAgendaDate.Value.ToString("dd/MM/yyyy") : "";
                                        });
                                        model.TrnListMeeting = new List<MeetingAgenda>();
                                        model.TrnListMeeting = listData;
                                    }
                                }
                                return View(model);
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }

            return View();
        }
        [Route("EmailToMeetingParticipants")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult EmailToMeetingParticipants(string ID)
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("EmailToMeetingParticipants");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model.roleList.Count != 0)
                            {
                                List<MeetingParticipants> listData = new List<MeetingParticipants>();
                                Int16 mID = Convert.ToInt16(Encryption.Decrypt(ID));
                                using (var client = new HttpClient())
                                {
                                    client.BaseAddress = new Uri(WebAPIUrl);
                                    //HTTP GET
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                    HttpResponseMessage response = client.GetAsync("AONW/GetParticipantsDataForMail?ID=" + mID).Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        listData = response.Content.ReadAsAsync<List<MeetingParticipants>>().Result;

                                    }
                                }
                                Session["participants"] = listData;
                                return View(listData);
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }
            return View();

        }
        [Route("SendMailToParticiants")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult SendMailToParticiants()
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("SendMailToParticiants");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model.roleList.Count != 0)
                            {
                                List<MeetingParticipants> listData = new List<MeetingParticipants>();
                                listData = Session["participants"] as List<MeetingParticipants>;
                                string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/SendMeetingMailToParticipants.html"));
                                listData.ForEach(f =>
                                {
                                    EmailHelper.SendToParticipants(f.Email, mailPath);
                                });
                                ViewBag.Msgg = "Email Sent sucessfully";
                                return View("EmailToMeetingParticipants", listData);
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }
            return View();

        }
        [Route("SetParticipantSession")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public void SetParticipantSession()
        {
            List<MeetingParticipants> listData = new List<MeetingParticipants>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/GetParticipantsForMeeting").Result;
                if (response.IsSuccessStatusCode)
                {
                    listData = response.Content.ReadAsAsync<List<MeetingParticipants>>().Result;

                }
            }
            Session["participants"] = listData;
        }
        [Route("EditMeeting")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult EditMeeting(string ID)
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model1 = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("SoCPdfRegistration");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model1 = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model1.roleList.Count != 0)
                            {
                                try
                                {
                                    Int16 mID = Convert.ToInt16(Encryption.Decrypt(ID));
                                    SechduleMeetingAgedaViewModel model = new SechduleMeetingAgedaViewModel();
                                    using (var client = new HttpClient())
                                    {
                                        client.BaseAddress = new Uri(WebAPIUrl);
                                        //HTTP GET
                                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                        HttpResponseMessage response = client.GetAsync("AONW/EditMeeting?ID=" + mID + "").Result;
                                        if (response.IsSuccessStatusCode)
                                        {
                                            model = response.Content.ReadAsAsync<SechduleMeetingAgedaViewModel>().Result;
                                            model.meeting_id = mID.ToString();
                                        }
                                    }
                                    SetParticipantSession();
                                    var List = Session["participants"] as List<MeetingParticipants>;
                                    List = List.Where(x => x.meeting_type == model.dac_dpb).ToList();
                                    List.ForEach(f =>
                                    {
                                        if (model.Participants.Where(x => x.UserID == f.UserID).FirstOrDefault() != null)
                                            f.IsSelected = true;
                                    });
                                    ViewBag.ParticipantsList = List;

                                    return View(model);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }

                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("EditMeeting")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public async Task<ActionResult> EditMeeting(SechduleMeetingAgedaViewModel _model)
        {
            List<MeetingParticipants> listData = new List<MeetingParticipants>();
            listData = Session["participants"] as List<MeetingParticipants>;
            if (ModelState.IsValid)
            {
                try
                {
                    var newlistData = listData.Where(x => _model.officers_participated_list.Contains(x.UserID.ToString()) && x.meeting_type == _model.dac_dpb).ToList();

                    _model.Participants = new List<MeetingParticipants>();
                    _model.Participants = newlistData;
                    using (var client = new HttpClient())
                    {
                        // client.BaseAddress = new Uri(WebAPIUrl + "AONW/UpdateMeetingMaster");
                        client.BaseAddress = new Uri(WebAPIUrl);
                        HttpResponseMessage postJob = await client.PostAsJsonAsync<SechduleMeetingAgedaViewModel>(WebAPIUrl + "AONW/UpdateMeetingMaster", _model);
                        bool postResult = postJob.IsSuccessStatusCode;
                        if (postResult == true)
                        {
                            ViewBag.Msgg = "Record Update Successfully";
                        }
                        else
                        {
                            ViewBag.Msgg = "Record Not Update Successfully";
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            ViewBag.ParticipantsList = listData.Where(x => x.meeting_type == _model.dac_dpb).ToList();
            return View(_model);
        }
        [Route("DeleteMeeting")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult DeleteMeeting(string ID)
        {
            try
            {
                Int16 mID = Convert.ToInt16(Encryption.Decrypt(ID));
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/DeleteMeetingByID?ID=" + mID).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ViewMeeting");
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("createMeeting");
        }
        #endregion
        #region Meeting Agenda Page Code
        [Route("BindDDl")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult BindDDl()
        {
            try
            {
                AONDescription Socmodel = new AONDescription();
                List<AONDescription> listData = new List<AONDescription>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/BindDDl").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        listData = response.Content.ReadAsAsync<List<AONDescription>>().Result;
                    }
                }

                return Json(listData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [Route("BindDDlSoC")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult BindDDlSoC(string id)
        {
            try
            {
                SAVESOCVIEWMODEL Socmodel = new SAVESOCVIEWMODEL();
                List<SAVESOCVIEWMODEL> listData = new List<SAVESOCVIEWMODEL>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/BindDDlSoC?Type=SoCType&value=" + id).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        listData = response.Content.ReadAsAsync<List<SAVESOCVIEWMODEL>>().Result;
                    }
                }
                return Json(listData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        //[Route("BindMeetingAgenda")]
        //[HandleError]
        //[SessionExpire]
        //[SessionExpireRefNo]
        public async Task<JsonResult> BindMeetingAgenda(int id)
        {
            MeetingAgenda Socmodel = new MeetingAgenda();
            List<MeetingAgenda> listData = new List<MeetingAgenda>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/BindMeetingAgenda?ID=" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    listData = response.Content.ReadAsAsync<List<MeetingAgenda>>().Result;
                    ViewBag.previousMeeting = listData;
                    listData.ForEach(f =>
                    {
                        f.MeetingAgendaDateString = f.MeetingAgendaDate.HasValue ? f.MeetingAgendaDate.Value.ToString("dd/MM/yyyy") : "";
                    });

                }
            }
            return await Task.FromResult(Json(listData, JsonRequestBehavior.AllowGet));
        }
        [Route("GetParticipantsForMeeting")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult GetParticipantsForMeeting(string mtype)
        {
            List<MeetingParticipants> listData = new List<MeetingParticipants>();
            listData = Session["participants"] as List<MeetingParticipants>;

            listData = listData.Where(x => x.meeting_type == mtype).ToList();

            return PartialView("_MeetingParticipants", listData);
        }
        [Route("LockMeetingComments")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult LockMeetingComments(int ID)
        {
            int userID = GetUserID();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/LockMeetingComments?ID=" + ID + "&UserID=" + userID).Result;
                if (response.IsSuccessStatusCode)
                {

                }
            }

            return RedirectToAction("ViewMeeting", "AONW");
        }
        public ActionResult ViewMeetingComments(string Id)
        {
            int userID = GetUserID();

            if (Convert.ToInt32(Session["SectionId"])==1 && Convert.ToInt32(Session["SectionId"])==12)
            {
                userID = 0;
            }
            dynamic listData = null;
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/GetComments?ID="+Id+"&userID="+userID).Result;
                if (response.IsSuccessStatusCode)
                {
                    listData = response.Content.ReadAsAsync<List<tbl_trn_MeetingAgendaComments>>().Result;
                }
            }
            ViewBag.CommentList = listData;
            ViewBag.Meeting_id = Id;
            return View();
        }

        public ActionResult SubmitMeetingComments(string meeting_id)
        {
            int userID = GetUserID();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/SubmitMeetingComments?ID="+meeting_id+"&userId="+userID).Result;
                if (response.IsSuccessStatusCode)
                {
                    //string results = response.Content.ReadAsAsync<string>().Result;
                    
                }
            }
            return View();
        }
        int id = 0;
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("CreateAgenda")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public async Task<ActionResult> CreateAgenda(MeetingAgenda _model)
        {
            ViewBag.AgendaItem1 = _model.AgendaItem1;

            int mid = Convert.ToInt32(_model.meeting_id);
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl + "AONW/CreateAgenda");
                    HttpResponseMessage postJob = await client.PostAsJsonAsync<MeetingAgenda>(WebAPIUrl + "AONW/CreateAgenda", _model);
                    bool postResult = postJob.IsSuccessStatusCode;
                    if (postResult == true)
                    {
                        TempData["Msg"] = "Sucessfully Saved Record";

                    }
                    else
                    {
                        TempData["Msg"] = "Some error occured record not saved";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            ViewBag.dated = Session["mdate"].ToString();
            ViewBag.mtype = Session["mtype"].ToString();
            string id = Encryption.Encrypt(mid.ToString());

            return RedirectToAction("AddMeetingAgenda", "AONW", new { id, ViewBag.mtype, ViewBag.dated });
        }
        [Route("AddMeetingAgenda")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult AddMeetingAgenda(string id, string mtype, string dated)
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("AddMeetingAgenda");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model.roleList.Count != 0)
                            {
                                ViewBag.mtype = mtype;
                                ViewBag.dated = dated;
                                MeetingAgenda meetingAgenda = new MeetingAgenda();
                                if (id == "0")
                                {
                                    meetingAgenda.meeting_id = Convert.ToInt16(id);
                                }
                                else
                                {
                                    meetingAgenda.meeting_id = Convert.ToInt16(Encryption.Decrypt(id));
                                }

                                meetingAgenda.MeetingAgendaComment = new MeetingAgendaComment();
                                meetingAgenda.MeetingAgendaCommentList = new List<MeetingAgendaComment>();
                                meetingAgenda.MeetingAgendaComment.UserID = GetUserID();
                                #region Get Meeting Dropdown
                                List<SAVESOCVIEWMODEL> dropdownTypeofAgenda = new List<SAVESOCVIEWMODEL>();
                                using (var client = new HttpClient())
                                {
                                    client.BaseAddress = new Uri(WebAPIUrl);
                                    //HTTP GET
                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                    HttpResponseMessage response = client.GetAsync("AONW/GetTypeOfAgenda").Result;
                                    if (response.IsSuccessStatusCode)
                                    {
                                        dropdownTypeofAgenda = response.Content.ReadAsAsync<List<SAVESOCVIEWMODEL>>().Result;
                                    }
                                }
                                dropdownTypeofAgenda.ForEach(f =>
                                {
                                    f.item_description = string.Concat(f.Service_Lead_Service, "-", f.SoCCase, "-", f.UniqueID, "-", f.item_description);
                                });
                                Session["dropdownTypeofAgenda"] = dropdownTypeofAgenda;
                                Session["mdate"] = dated;
                                Session["mtype"] = mtype;
                                ViewBag.dropdownTypeofAgenda = dropdownTypeofAgenda;
                                #endregion

                                return View(meetingAgenda);
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }

            return View();
        }

        [HttpGet]
        [Route("CheckTypeofAgendaExists")]
        public ActionResult CheckTypeofAgendaExists(int id, int typeofAgenda)
        {
            //string results = "";
            //using (var client = new HttpClient())
            //{
            //    client.BaseAddress = new Uri(WebAPIUrl);
            //    //HTTP GET
            //    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
            //    HttpResponseMessage response = client.GetAsync("AONW/CheckTypeofAgendaExists?id="+id+ "&typeofAgenda="+typeofAgenda).Result;
            //    if (response.IsSuccessStatusCode)
            //    {
            //         results = response.Content.ReadAsAsync<List<SAVESOCVIEWMODEL>>().Result.ToString();
            //    }
            //}
            //return results;
            return View();
        }
        [Route("GetUserID")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        private int GetUserID()
        {
            int UserID = Convert.ToInt32(Session["UserID"]);
            int sectionID = Convert.ToInt32(Session["SectionID"]);
            if (sectionID == 13)
                UserID = 0;
            return UserID;
        }
        //[Route("EditMeetingAgenda")]
        //[HandleError]
        //[SessionExpire]
        //[SessionExpireRefNo]
        public ActionResult EditMeetingAgenda(int ID)
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model1 = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("EditMeetingAgenda");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model1 = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model1.roleList.Count != 0)
                            {
                                try
                                {
                                    MeetingAgenda model = new MeetingAgenda();
                                    int UserID = GetUserID();

                                    using (var client = new HttpClient())
                                    {
                                        client.BaseAddress = new Uri(WebAPIUrl);
                                        //HTTP GET
                                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                        HttpResponseMessage response = client.GetAsync("AONW/EditMeetingAgenda?ID=" + ID + "&UserID=" + UserID).Result;
                                        if (response.IsSuccessStatusCode)
                                        {
                                            model = response.Content.ReadAsAsync<MeetingAgenda>().Result;
                                            if (UserID > 0)
                                                model.Comments = JsonConvert.SerializeObject(model.MeetingAgendaComment);
                                            else
                                                model.Comments = JsonConvert.SerializeObject(model.MeetingAgendaCommentList);
                                        }
                                    }
                                    List<SAVESOCVIEWMODEL> dropdownTypeofAgenda = (List<SAVESOCVIEWMODEL>)Session["dropdownTypeofAgenda"];
                                    ViewBag.dropdownTypeofAgenda = dropdownTypeofAgenda;
                                    ViewBag.dated = Session["mdate"].ToString();
                                    ViewBag.mtype = Session["mtype"].ToString();
                                    ViewBag.PageMode = "Edit";
                                    return View("AddMeetingAgenda", model);
                                }
                                catch (Exception ex)
                                {
                                    throw ex;
                                }
                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }

            return View();
        }

        [HttpGet]
        [Route("UploadApprovalDocs")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult UploadApprovalDocs(string ID,string mtype,string dated)
        {
            ViewBag.Meeting_id = ID;
            ViewBag.mtype = mtype;
            ViewBag.dated = dated;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("UpdateMeetingAgenda")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public async Task<ActionResult> UpdateMeetingAgenda(MeetingAgenda _model)
        {
            //if (ModelState.IsValid)
            //{
            try
            {
                if (GetUserID() > 0)
                {
                    MeetingAgendaComment meetingCommentAgenda = new MeetingAgendaComment();
                    meetingCommentAgenda = JsonConvert.DeserializeObject<MeetingAgendaComment>(sanitizer.Sanitize(_model.Comments));
                    if (!string.IsNullOrEmpty(meetingCommentAgenda.ProposalComment) || !string.IsNullOrEmpty(meetingCommentAgenda.BackgroundComment) || !string.IsNullOrEmpty(meetingCommentAgenda.ApprovalSoughtComment)
                        || !string.IsNullOrEmpty(meetingCommentAgenda.DecisionComment) || !string.IsNullOrEmpty(meetingCommentAgenda.DiscussionComment))
                    {
                        _model.MeetingAgendaComment = new MeetingAgendaComment();
                        _model.MeetingAgendaComment = meetingCommentAgenda;
                    }
                    else
                    {
                        _model.MeetingAgendaComment = new MeetingAgendaComment();
                        meetingCommentAgenda.IsDelete = true;
                        _model.MeetingAgendaComment = meetingCommentAgenda;
                    }
                }
                else
                    _model.MeetingAgendaComment = new MeetingAgendaComment();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    HttpResponseMessage postJob = await client.PostAsJsonAsync<MeetingAgenda>(WebAPIUrl + "AONW/UpdateMeetingAgenda", _model);
                    bool postResult = postJob.IsSuccessStatusCode;
                    if (postResult == true)
                    {
                        TempData["Msg"] = "Record Update Successfully";
                    }
                    else
                    {
                        TempData["Msg"] = "Record Not Update Successfully";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            // }

            //  id = _model.meeting_id.Value;
            ViewBag.dated = Session["mdate"].ToString();
            ViewBag.mtype = Session["mtype"].ToString();
            //int Id  = (Encryption.Encrypt(_model.meeting_id.Value));
            string id = Encryption.Encrypt(_model.meeting_id.Value.ToString());
            return RedirectToAction("AddMeetingAgenda", "AONW", new { id, ViewBag.mtype, ViewBag.dated });
        }
        //[Route("DeleteMeetingAgenda")]
        //[HandleError]
        //[SessionExpire]
        //[SessionExpireRefNo]
        public ActionResult DeleteMeetingAgenda(int id, int meeting_id)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/DeleteMeetingAgendaByID?id=" + id).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        TempData["Msg"] = "Meeting agenda delete successfully";
                    }
                    else
                    {
                        TempData["Msg"] = "Some error occured record not deleted";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            ViewBag.dated = Session["mdate"].ToString();
            ViewBag.mtype = Session["mtype"].ToString();
            return RedirectToAction("AddMeetingAgenda", "AONW", new { id = meeting_id, ViewBag.mtype, ViewBag.dated });
        }

        [Route("SocTimeline")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult SocTimeline()
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("SocTimeline");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model.roleList.Count != 0)
                            {

                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }
            return View();
        }



        #endregion
        #region TimeLineCode
        [Route("BindDDlSoCMByID")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult BindDDlSoCMByID(string id)
        {
            try
            {
                SAVESOCVIEWMODEL Socmodel = new SAVESOCVIEWMODEL();
                List<SAVESOCVIEWMODEL> listData = new List<SAVESOCVIEWMODEL>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/BindDDlSoCMByID?id=" + id).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        listData = response.Content.ReadAsAsync<List<SAVESOCVIEWMODEL>>().Result;
                    }
                }
                ViewBag.mData = listData.First().SoDate;
                return Json(ViewBag.mData, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        #region Contract
        [Route("Contract")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult Contract()
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("Contract");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        try
                        {
                            model = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model.roleList.Count != 0)
                            {

                            }
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    }
                    else
                    {
                        return RedirectToAction("Login", "Account");
                    }
                }
                // return View();
            }
            return View();
        }


        [Route("SaveContract")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> SaveContract(Contracts cnt)
        {
            int i = 0;
            try
            {
                Contracts _contracts = new Contracts();
                ContractDetails contractDetails = new ContractDetails
                {
                    ContractId = sanitizer.Sanitize(cnt.Contrct_Detail.ContractId),
                    Contract_Number = sanitizer.Sanitize(cnt.Contrct_Detail.Contract_Number),
                    DateOfContractSigning = Convert.ToDateTime(sanitizer.Sanitize(cnt.Contrct_Detail.DateOfContractSigning.ToString())),
                    Descriptions = sanitizer.Sanitize(cnt.Contrct_Detail.Descriptions),
                    Category = sanitizer.Sanitize(cnt.Contrct_Detail.Category),
                    EffectiveDate = Convert.ToDateTime(sanitizer.Sanitize(cnt.Contrct_Detail.EffectiveDate.ToString())),
                    ABGDate = Convert.ToDateTime(sanitizer.Sanitize(cnt.Contrct_Detail.ABGDate.ToString())),
                    PWBGPercentage = Convert.ToInt32(sanitizer.Sanitize(cnt.Contrct_Detail.PWBGPercentage.ToString())),
                    PWBGDate = Convert.ToDateTime(sanitizer.Sanitize(cnt.Contrct_Detail.PWBGDate.ToString())),
                    Incoterms = sanitizer.Sanitize(cnt.Contrct_Detail.Incoterms),
                    Warranty = sanitizer.Sanitize(cnt.Contrct_Detail.Warranty),
                    ContractValue = sanitizer.Sanitize(cnt.Contrct_Detail.ContractValue),
                    FEContent = sanitizer.Sanitize(cnt.Contrct_Detail.FEContent),
                    TaxesAndDuties = sanitizer.Sanitize(cnt.Contrct_Detail.TaxesAndDuties),
                    FinalDeliveryDate = Convert.ToDateTime(sanitizer.Sanitize(cnt.Contrct_Detail.FinalDeliveryDate.ToString())),
                    GracePeriod = sanitizer.Sanitize(cnt.Contrct_Detail.GracePeriod),
                };

                _contracts.Contrct_Detail = contractDetails;

                List<StageDetail> stages = new List<StageDetail>();
                foreach(var item in cnt.Stage_Detail.ToList())
                {
                    StageDetail stageDetail = new StageDetail
                    {
                        StageNumber = Convert.ToInt32(sanitizer.Sanitize(item.StageNumber.ToString())),
                        stageDescription = sanitizer.Sanitize(item.stageDescription),
                        StageStartdate = Convert.ToDateTime(sanitizer.Sanitize(item.StageStartdate.ToString())),
                        StageCompletionDate = Convert.ToDateTime(sanitizer.Sanitize(item.StageCompletionDate.ToString())),
                        PercentOfContractValue = Convert.ToInt32(sanitizer.Sanitize(item.PercentOfContractValue.ToString())),
                        Amount = Convert.ToDecimal(sanitizer.Sanitize(item.Amount.ToString())),
                        DueDateOfPayment = Convert.ToDateTime(sanitizer.Sanitize(item.DueDateOfPayment.ToString())),
                        Conditions = sanitizer.Sanitize(item.Conditions),
                        RevisedDateOfpayment = Convert.ToDateTime(sanitizer.Sanitize(item.RevisedDateOfpayment.ToString())),
                        ReasonsForSlippage = sanitizer.Sanitize(item.ReasonsForSlippage),
                        ActualDateOfPayment = Convert.ToDateTime(sanitizer.Sanitize(item.ActualDateOfPayment.ToString())),
                        TotalPaymentMade = sanitizer.Sanitize(item.TotalPaymentMade),
                        FullorPartPaymentMade = sanitizer.Sanitize(item.FullorPartPaymentMade),
                    };
                    stages.Add(stageDetail);
                }

                _contracts.Stage_Detail = stages;


                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    HttpResponseMessage postJob = await client.PostAsJsonAsync<Contracts>(WebAPIUrl + "AONW/SaveContract", _contracts);
                    bool postResult = postJob.IsSuccessStatusCode;
                    if (postResult == true)
                    {
                        //TempData["Msg"] = "Record Saved Successfully";
                        i = 1;
                    }
                    else
                    {
                        //TempData["Msg"] = "Record Not Saved Successfully";
                        i = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(i, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}