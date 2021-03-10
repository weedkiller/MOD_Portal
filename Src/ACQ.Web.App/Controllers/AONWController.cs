using ACQ.Web.ViewModel.AONW;
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

namespace ACQ.Web.App.Controllers
{
    //[Authorize]
    public class AONWController : Controller
    {
        private static string UploadPath = ConfigurationManager.AppSettings["SOCImagePath"].ToString();
        private static string UploadfilePath = ConfigurationManager.AppSettings["SOCPath"].ToString();
        private static string WebAPIUrl = ConfigurationManager.AppSettings["APIUrl"].ToString();
        List<Efile.FileDetail> fileDetails = new List<Efile.FileDetail>();
        List<Efile.FileDetail> fileDetailsA = new List<Efile.FileDetail>();
        List<AttachmentViewModel> fileDetailsF = new List<AttachmentViewModel>();
        // GET: AONW
        #region Index HomePage
        public ActionResult Index(SAVESOCVIEWMODEL _model)
        {
            return View(_model);
        }
        #endregion
        #region Main SOC Page All Code
        public ActionResult ViewSOCRegistration()
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
            List<SAVESOCVIEWMODEL> listData = new List<SAVESOCVIEWMODEL>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/ViewSOCRegistration").Result;
                if (response.IsSuccessStatusCode)
                {
                    listData = response.Content.ReadAsAsync<List<SAVESOCVIEWMODEL>>().Result;
                    
                }
            }

            if (mSercive != "Acquisition Wing")
            {
                Socmodel.SOCVIEW = listData.Where(x => x.Service_Lead_Service == mSercive).ToList();
            }
            else
            {
                Socmodel.SOCVIEW = listData;
            }
            return View(Socmodel);
        }
        public ActionResult ViewSOCComment()
        {
            
            SocCommentViewModel Socmodel = new SocCommentViewModel();
            List<SocCommentViewModel> listData = new List<SocCommentViewModel>();
            var id = Session["UserID"].ToString();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/GetCommenttDetails?ID=" + id + "").Result;
                if (response.IsSuccessStatusCode)
                {
                    listData = response.Content.ReadAsAsync<List<SocCommentViewModel>>().Result;

                }
            }

            Socmodel.SOCVIEWComment = listData;
            return View(Socmodel);
        }
        public ActionResult ViewSocSendMail(int ID)
        {
            acqmstmemberSendMailViewModel Socmodel = new acqmstmemberSendMailViewModel();
            List<acqmstmemberSendMailViewModel> listData = new List<acqmstmemberSendMailViewModel>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/GetSendMail?ID=" + ID + "").Result;
                if (response.IsSuccessStatusCode)
                {
                    Session["id"] = ID;
                    listData = response.Content.ReadAsAsync<List<acqmstmemberSendMailViewModel>>().Result;

                }
            }

            Socmodel.SOCMailVIEW = listData;
            return View(Socmodel);
        }
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
                    foreach(var dummyList in listData)
                    {
                        string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/SendOTPMailFormat.html"));
                        EmailHelper.SendAllDetails( dummyList.Email, mailPath);
                        ViewBag.Message = "RegistrationSuccessful";
                    }


                }
            }

            Socmodel.SOCMailVIEW = listData;
            return RedirectToAction("ViewSOCRegistration");
        }
        [HttpPost]
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
                                        objattach.AttachmentFileName =filename;
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
                                    string filename = mID+ "_" + "SOCPPT" + "_" + fileSoCPPT.FileName;
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

        public ActionResult ViewSocMaster(int ID, string mtype)
        {
            try
            {
                SAVESOCVIEWMODEL model = new SAVESOCVIEWMODEL();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/EditSocMaster?ID=" + ID + "").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        model = response.Content.ReadAsAsync<SAVESOCVIEWMODEL>().Result;
                        model.item_description = Encryption.Decrypt(model.item_description);
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
                    using (var client1 = new HttpClient())
                    {
                        client1.BaseAddress = new Uri(WebAPIUrl);
                        //HTTP GET
                        client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        HttpResponseMessage response = client1.GetAsync("AONW/GetAttachFile?ID=" + ID + "").Result;
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
        public ActionResult EditSocMaster(int ID)
        {
            try
            {
                SAVESOCVIEWMODEL model = new SAVESOCVIEWMODEL();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/EditSocMaster?ID=" + ID + "").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        model = response.Content.ReadAsAsync<SAVESOCVIEWMODEL>().Result;
                       
                    }

                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
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
        public ActionResult SOCApproval()
        {
            return View();
        }
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

        public async Task<ActionResult> UpdateSocMaster(int ID)
        {
            try
            {
                SAVESOCVIEWMODELBluk model = new SAVESOCVIEWMODELBluk();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/EditSocMaster?ID=" + ID + "").Result;
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
                                    objattach.aon_id = ID;
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
        public ActionResult ViewMeeting()
       {
            try
            {
                SechduleMeetingAgedaViewModel Socmodel = new SechduleMeetingAgedaViewModel();
                List<SechduleMeetingAgedaViewModel> listData = new List<SechduleMeetingAgedaViewModel>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/CreateMeetings").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        listData = response.Content.ReadAsAsync<List<SechduleMeetingAgedaViewModel>>().Result;
                    }
                }

                Socmodel.ListofMeeting = listData;
                return View(Socmodel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public ActionResult createMeeting()
        {
            return View();
        }
        public ActionResult AddSocCommit(int id)
        {
            SocCommentViewModel model = new SocCommentViewModel();
            model.SoCId = id;
            Session["item"]= Request.QueryString["item"].ToString();
            ViewBag.aonId = Session["item"].ToString();
            return View(model);

        }
        [HttpPost]
        public async Task<ActionResult> AddSocCommit(SocCommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    model.IsActive = "Y";
                    model.SocCommentID = 0;
                    model.UserID = Convert.ToInt16(Session["UserID"].ToString());

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(WebAPIUrl + "AONW/AddSocCommit");
                        HttpResponseMessage postJob = await client.PostAsJsonAsync<SocCommentViewModel>(WebAPIUrl + "AONW/AddSocCommit", model);
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
            ViewBag.aonId = Session["item"].ToString();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveMeetings(SechduleMeetingAgedaViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
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
        public ActionResult GenerateReport(int ID)
        {

            SechduleMeetingAgedaViewModel model = new SechduleMeetingAgedaViewModel();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/EditMeeting?ID=" + ID + "").Result;
                if (response.IsSuccessStatusCode)
                {
                    model = response.Content.ReadAsAsync<SechduleMeetingAgedaViewModel>().Result;
                    model.meeting_id = ID;

                }
            }
            MeetingAgenda Socmodel = new MeetingAgenda();
            List<MeetingAgenda> listData = new List<MeetingAgenda>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/BindMeetingAgenda?ID=" + ID).Result;
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
        public ActionResult EditMeeting(int ID)
        {
            try
            {
                SechduleMeetingAgedaViewModel model = new SechduleMeetingAgedaViewModel();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/EditMeeting?ID=" + ID + "").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        model = response.Content.ReadAsAsync<SechduleMeetingAgedaViewModel>().Result;
                        model.meeting_id = ID;
                    }
                }
                return View(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ActionResult> EditMeeting(SechduleMeetingAgedaViewModel _model)
        {

            if (ModelState.IsValid)
            {
                try
                {
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
            return View(_model);
        }
        public ActionResult DeleteMeeting(int ID)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/DeleteMeetingByID?ID=" + ID).Result;
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
        int id = 0;
        [HttpPost]
        public async Task<ActionResult> CreateAgenda(MeetingAgenda _model)
        {
            ViewBag.AgendaItem1 = _model.AgendaItem1;
            if (ModelState.IsValid)
            {
                id = Convert.ToInt32(_model.meeting_id);
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
            }
            ViewBag.dated = Session["mdate"].ToString();
            ViewBag.mtype = Session["mtype"].ToString();
            return RedirectToAction("AddMeetingAgenda", "AONW", new { id, ViewBag.mtype, ViewBag.dated });
        }
        public ActionResult AddMeetingAgenda(int id, string mtype, string dated)
        {
            ViewBag.mtype = mtype;
            ViewBag.dated = dated;
            MeetingAgenda meetingAgenda = new MeetingAgenda();
            meetingAgenda.meeting_id = id;
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
        public ActionResult EditMeetingAgenda(int ID)
        {
            try
            {
                MeetingAgenda model = new MeetingAgenda();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/EditMeetingAgenda?ID=" + ID + "").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        model = response.Content.ReadAsAsync<MeetingAgenda>().Result;

                    }
                }
                List<SAVESOCVIEWMODEL> dropdownTypeofAgenda = (List<SAVESOCVIEWMODEL>)Session["dropdownTypeofAgenda"];
                ViewBag.dropdownTypeofAgenda = dropdownTypeofAgenda;
                ViewBag.dated = Session["mdate"].ToString();
                ViewBag.mtype = Session["mtype"].ToString();
                return View("AddMeetingAgenda", model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [HttpPost]
        public async Task<ActionResult> UpdateMeetingAgenda(MeetingAgenda _model)
        {
            if (ModelState.IsValid)
            {
                try
                {
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
            }

            id = _model.meeting_id.Value;
            ViewBag.dated = Session["mdate"].ToString();
            ViewBag.mtype = Session["mtype"].ToString();
            return RedirectToAction("AddMeetingAgenda", "AONW", new { id, ViewBag.mtype, ViewBag.dated });
        }
        public ActionResult DeleteMeetingAgenda(int id,int meeting_id)
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

        OleDbConnection Econ;
        SAVESOCVIEWMODELBluk obj = new SAVESOCVIEWMODELBluk();
        public ActionResult BulkUpload()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> BulkUpload(HttpPostedFileBase file)
        {
            if (Request.Files.Count > 0)
            {

                System.Web.HttpPostedFileBase fileSoCMetaData = Request.Files[0];

                try
                {
                    if (fileSoCMetaData.ContentLength > 0)
                    {

                        string filename = Guid.NewGuid() + Path.GetExtension(fileSoCMetaData.FileName);
                        ViewBag.SoCMetaData = fileSoCMetaData.FileName;
                        string FileExtension = Path.GetExtension(fileSoCMetaData.FileName);
                        if (FileExtension == ".xls" || FileExtension == ".xlsx")
                        {
                            ViewBag.UploadStatus = "";
                            string filepath = UploadfilePath + filename;

                            fileSoCMetaData.SaveAs(Path.Combine(Server.MapPath(UploadfilePath), filename));

                            string fullpath = Server.MapPath(UploadfilePath) + filename;

                            ExcelConn(fullpath, FileExtension);
                            string query = string.Format("Select * from [{0}]", "Sheet1$");

                            if (Econ != null)
                                Econ.Open();

                            DataSet ds = new DataSet();
                            OleDbDataAdapter oda = new OleDbDataAdapter(query, Econ);

                            if (Econ.State == ConnectionState.Open)
                            {
                                Econ.Close();
                            }
                            oda.Fill(ds);

                            DataTable dt = ds.Tables[0];
                            obj.aon_id = 0;
                            obj.DPP_DAP = dt.Rows[0]["F2"].ToString();
                            obj.Categorisation = dt.Rows[1]["F2"].ToString();
                            obj.Service_Lead_Service = dt.Rows[2]["F2"].ToString();
                            obj.item_description = dt.Rows[3]["F2"].ToString();
                            obj.SoDate = Convert.ToDateTime(dt.Rows[4]["F2"].ToString());
                            obj.Quantity = dt.Rows[5]["F2"].ToString();
                            obj.Cost = dt.Rows[6]["F2"].ToString();
                            obj.Tax_Duties = dt.Rows[7]["F2"].ToString();
                            obj.Type_of_Acquisition = dt.Rows[8]["F2"].ToString();
                            obj.Trials_Required = dt.Rows[9]["F2"].ToString();
                            obj.Essential_parameters = dt.Rows[10]["F2"].ToString();
                            obj.Any_other_aspect = dt.Rows[11]["F2"].ToString();
                            obj.IC_percentage = dt.Rows[12]["F2"].ToString();
                            obj.Option_clause_applicable = dt.Rows[13]["F2"].ToString();
                            obj.Offset_applicable = dt.Rows[14]["F2"].ToString();
                            obj.AMC_applicable = dt.Rows[15]["F2"].ToString();
                            obj.AMCRemarks = dt.Rows[16]["F2"].ToString();
                            obj.Warrenty_applicable = dt.Rows[17]["F2"].ToString();
                            obj.Warrenty_Remarks = dt.Rows[18]["F2"].ToString();
                            obj.SoCCase = dt.Rows[19]["F2"].ToString();
                            obj.Remarks = dt.Rows[20]["F2"].ToString();
                            obj.SoCType = dt.Rows[21]["F2"].ToString();
                            obj.AoN_Accorded_By = "DPB";
                            obj.AoN_validity = 1;
                            obj.AoN_validity_unit = "Month";
                            obj.CreatedBy = 1;
                            obj.CreatedOn = System.DateTime.Now;
                            obj.DeletedBy = 1;
                            obj.DeletedOn = System.DateTime.Now;
                            obj.IsDeleted = false;

                            SAVESOCVIEWMODEL model = new SAVESOCVIEWMODEL();
                            IEnumerable<SAVESOCVIEWMODEL> Badge = null;
                            using (var client1 = new HttpClient())
                            {
                                client1.BaseAddress = new Uri(WebAPIUrl);
                                //HTTP GET
                                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                HttpResponseMessage response1 = client1.GetAsync("AONW/GetDetailSOCRegistration?Type=SoCCase&value=" + obj.SoCCase).Result;
                                if (response1.IsSuccessStatusCode)
                                {
                                    Badge = response1.Content.ReadAsAsync<IEnumerable<SAVESOCVIEWMODEL>>().Result;

                                }


                            }
                            if (Badge.Count() != 0)
                            {
                                if (Badge.First().SoCCase == obj.SoCCase && Badge.First().Service_Lead_Service == obj.Service_Lead_Service)
                                {
                                    ViewBag.Save = "Unique Id already exist";
                                    return View();
                                }

                            }
                            using (HttpClient client1 = new HttpClient())
                            {
                                client1.BaseAddress = new Uri(WebAPIUrl + "AONW/AONWCreateBulk");
                                HttpResponseMessage postJob = await client1.PostAsJsonAsync<SAVESOCVIEWMODELBluk>(WebAPIUrl + "AONW/AONWCreateBulk", obj);
                                string url = postJob.Headers.Location.AbsoluteUri;
                                string mID = postJob.Headers.Location.Segments[4].ToString();
                                bool postResult = postJob.IsSuccessStatusCode;
                                if (postResult == true)
                                {
                                    AttachmentViewModel objattach = new AttachmentViewModel();
                                    objattach.aon_id = Convert.ToInt32(mID);
                                    objattach.AttachmentFileName = mID.ToString() + "_" + "SoCMetaData" + "_" + filename;
                                    objattach.Path = fullpath;
                                    objattach.RecDate = DateTime.Now;
                                    objattach.RefId = 1;

                                    using (var client = new HttpClient())
                                    {
                                        client.BaseAddress = new Uri(WebAPIUrl + "AONW/SaveAttachFile");
                                        HttpResponseMessage postJob1 = await client.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/SaveAttachFile", objattach);
                                        postResult = postJob1.IsSuccessStatusCode;

                                    }
                                    if (postResult == true)
                                    {
                                        System.Web.HttpPostedFileBase fileSoC = Request.Files[1];

                                        if (fileSoC.ContentLength > 0)
                                        {
                                            ViewBag.SoC = fileSoC.FileName;
                                            filename = objattach.aon_id.ToString() + "_" + "SOC" + "_" + fileSoC.FileName.ToString();

                                            FileExtension = Path.GetExtension(fileSoC.FileName);
                                            if (FileExtension == ".pdf" || FileExtension == ".PDF")
                                            {
                                                ViewBag.UploadStatus = "";
                                                filepath = UploadfilePath + filename;
                                                fileSoC.SaveAs(Path.Combine(Server.MapPath(UploadfilePath), filename));
                                                fullpath = Server.MapPath(UploadfilePath) + filename;
                                            }
                                            objattach.AttachmentFileName = filename;
                                            objattach.Path = fullpath;
                                            objattach.RefId = 2;
                                            using (var client = new HttpClient())
                                            {
                                                client.BaseAddress = new Uri(WebAPIUrl + "AONW/SaveAttachFile");
                                                HttpResponseMessage postJob1 = await client.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/SaveAttachFile", objattach);
                                                postResult = postJob1.IsSuccessStatusCode;
                                            }
                                        }
                                        System.Web.HttpPostedFileBase fileSoCAnnexure = Request.Files[2];

                                        if (fileSoCAnnexure.ContentLength > 0)
                                        {
                                            ViewBag.SoC = fileSoCAnnexure.FileName;
                                            filename = objattach.aon_id.ToString() + "_" + "SOCAnnexure" + "_" + fileSoCAnnexure.FileName.ToString();
                                            FileExtension = Path.GetExtension(fileSoCAnnexure.FileName);
                                            if (FileExtension == ".pdf" || FileExtension == ".PDF")
                                            {
                                                ViewBag.UploadStatus = "";
                                                filepath = UploadfilePath + filename;
                                                fileSoCAnnexure.SaveAs(Path.Combine(Server.MapPath(UploadfilePath), filename));
                                                fullpath = Server.MapPath(UploadfilePath) + filename;
                                            }
                                            objattach.AttachmentFileName = filename;
                                            objattach.Path = fullpath;
                                            objattach.RefId = 3;
                                            using (var client = new HttpClient())
                                            {
                                                client.BaseAddress = new Uri(WebAPIUrl + "AONW/SaveAttachFile");
                                                HttpResponseMessage postJob2 = await client.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/SaveAttachFile", objattach);
                                                postResult = postJob2.IsSuccessStatusCode;
                                            }

                                        }
                                        System.Web.HttpPostedFileBase fileSoCPPT = Request.Files[3];

                                        if (fileSoCPPT.ContentLength > 0)
                                        {
                                            ViewBag.SoC = fileSoCPPT.FileName;
                                            filename = objattach.aon_id.ToString() + "_" + "SOCPPT" + "_" + fileSoCPPT.FileName.ToString();
                                            FileExtension = Path.GetExtension(fileSoCPPT.FileName);
                                            if (FileExtension == ".ppt")
                                            {
                                                ViewBag.UploadStatus = "";
                                                filepath = UploadfilePath + filename;
                                                fileSoCPPT.SaveAs(Path.Combine(Server.MapPath(UploadfilePath), filename));
                                                fullpath = Server.MapPath(UploadfilePath) + filename;

                                            }
                                            objattach.AttachmentFileName = filename;
                                            objattach.Path = fullpath;
                                            objattach.RefId = 4;
                                            using (var client = new HttpClient())
                                            {
                                                client.BaseAddress = new Uri(WebAPIUrl + "AONW/SaveAttachFile");
                                                HttpResponseMessage postJob2 = await client.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/SaveAttachFile", objattach);
                                                postResult = postJob2.IsSuccessStatusCode;
                                            }
                                        }
                                        System.Web.HttpPostedFileBase fileOtherDoucment = Request.Files[4];
                                        if (fileOtherDoucment.ContentLength > 0)
                                        {
                                            ViewBag.SoC = fileOtherDoucment.FileName;
                                            filename = objattach.aon_id.ToString() + "_" + "SOCOther" + "_" + fileOtherDoucment.FileName;
                                            FileExtension = Path.GetExtension(fileOtherDoucment.FileName);
                                            if (FileExtension == ".pdf")
                                            {
                                                ViewBag.UploadStatus = "";
                                                filepath = UploadfilePath + filename;
                                                fileOtherDoucment.SaveAs(Path.Combine(Server.MapPath(UploadfilePath), filename));
                                                fullpath = Server.MapPath(UploadfilePath) + filename;
                                            }
                                            objattach.AttachmentFileName = filename;
                                            objattach.Path = fullpath;
                                            objattach.RefId = 5;
                                            using (var client = new HttpClient())
                                            {
                                                client.BaseAddress = new Uri(WebAPIUrl + "AONW/SaveAttachFile");
                                                HttpResponseMessage postJob2 = await client.PostAsJsonAsync<AttachmentViewModel>(WebAPIUrl + "AONW/SaveAttachFile", objattach);
                                                postResult = postJob2.IsSuccessStatusCode;
                                            }
                                        }

                                    }

                                    ViewBag.Save = "SOC Registration Successfully";
                                }
                                else
                                {
                                    ViewBag.NotSave = "SOC registration not saved";
                                }
                            }
                        }
                        else
                        {
                            ViewBag.UploadStatus = "Only .xls or .xlsx file";
                            return View();
                        }

                    }
                    else
                    {
                        ViewBag.UploadStatus = "Can not be empty SoC meta data";
                        return View();
                    }
                }
                catch (Exception ex) { }
            }
            else
            {
                ViewBag.UploadStatus = "Can not be empty";

            }

            return View();

        }
        private void ExcelConn(string filepath, string extnsion)
        {
            string constr = string.Empty;
            if (extnsion == ".xlsx")
            {
                constr = string.Format(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=""Excel 12.0 Xml;HDR=YES;""", filepath);
            }
            else { constr = string.Format(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=Yes'""", filepath); }
            Econ = new OleDbConnection(constr);
        }
        public ActionResult SocTimeline()
        {
            return View();
        }

       

        #endregion
        #region TimeLineCode
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
    }
}