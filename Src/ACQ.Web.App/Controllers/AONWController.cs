using ACQ.Web.App.ViewModel;
using ACQ.Web.ExternalServices.Email;
using ACQ.Web.ExternalServices.SecurityAudit;
using ACQ.Web.ViewModel.AONW;
using ACQ.Web.ViewModel.EFile;
using ACQ.Web.ViewModel.User;
using Ganss.XSS;
using Newtonsoft.Json;
using SignLib;
using SignLib.Certificates;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Linq;
using static ACQ.Web.App.MvcApplication;



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
            if(BruteForceAttackss.bcontroller!= "")
            {
                if(BruteForceAttackss.bcontroller == "AONW")
                {
                    if (BruteForceAttackss.refreshcount == 0 && BruteForceAttackss.date == null)
                    {
                        BruteForceAttackss.date = System.DateTime.Now;
                        BruteForceAttackss.refreshcount = 1;
                    }
                    else
                    {
                        TimeSpan tt = System.DateTime.Now - BruteForceAttackss.date.Value;
                        if (tt.TotalSeconds <= 30 && BruteForceAttackss.refreshcount > 20)
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
                                            BruteForceAttackss.refreshcount = 0;
                                            BruteForceAttackss.date = null;
                                            BruteForceAttackss.bcontroller = "";
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
            else
            {
                BruteForceAttackss.bcontroller = "AONW";
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
            return View(_model);
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
            string mSercive = "";
            if (Session["Department"].ToString() == "IDS")
            {
                mSercive = "Joint Staff";
            }
            else
            {
                mSercive = sanitizer.Sanitize(Session["Department"].ToString()); 
            }
            SAVESOCVIEWMODEL Socmodel = new SAVESOCVIEWMODEL();
            List<SAVESOCVIEWMODEL> listData = new List<SAVESOCVIEWMODEL>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebAPIUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                         parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
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

        [Route("ViewSocComments")]
        [HandleError]
        [HttpGet]
        public ActionResult ViewSocComments(string ID)
        {
            List<SocCommentViewModel> listData = new List<SocCommentViewModel>();
            int userId = Convert.ToInt32(Session["UserID"]);
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/GetCommentt?socId="+ID+"&ID=" + userId + "").Result;
                if (response.IsSuccessStatusCode)
                {
                    listData = response.Content.ReadAsAsync<List<SocCommentViewModel>>().Result;
                    ViewBag.ListData = listData;
                }
            }
            return View();
        }

        [Route("ViewSOCComment")]
        [HandleError]
        [HandleError(ExceptionType = typeof(NullReferenceException), Master = "Account", View = "Error")]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ViewSOCComment()
        {

            SocCommentViewModel Socmodel = new SocCommentViewModel();
            List<SocCommentViewModel> listData = new List<SocCommentViewModel>();
            var id = sanitizer.Sanitize(Session["UserID"].ToString());
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/GetCommentt?ID=" + id + "").Result;
                if (response.IsSuccessStatusCode)
                {
                    listData = response.Content.ReadAsAsync<List<SocCommentViewModel>>().Result;

                }
            }

            Socmodel.SOCVIEWComment = listData;
            return View(Socmodel);
        }

        [Route("ViewSocSendMail")]
        [HandleError]
        [HandleError(ExceptionType = typeof(NullReferenceException), Master = "Account", View = "Error")]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ViewSocSendMail(string ID)
        {
            acqmstmemberSendMailViewModel Socmodel = new acqmstmemberSendMailViewModel();
            List<acqmstmemberSendMailViewModel> listData = new List<acqmstmemberSendMailViewModel>();
            ID = Encryption.Decrypt(sanitizer.Sanitize(ID));
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


        [Route("SendMailToAll")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult SendMailToAll()
        {
         
            acqmstmemberSendMailViewModel Socmodel = new acqmstmemberSendMailViewModel();
            List<acqmstmemberSendMailViewModel> listData = new List<acqmstmemberSendMailViewModel>();
            var id = sanitizer.Sanitize(Session["id"].ToString()); 
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
                    using (var client1 = new HttpClient())
                    {
                        client1.BaseAddress = new Uri(WebAPIUrl);
                        //HTTP GET
                        client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        HttpResponseMessage response = client1.GetAsync("AONW/GetAttachFile?ID=" + Encryption.Decrypt(ID) + "").Result;
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
            HtmlSanitizer sanitizer = new HtmlSanitizer();
            int meeting_id = Convert.ToInt32(Encryption.Decrypt(sanitizer .Sanitize(ID)));
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
                    ViewBag.Url = "/excelfolder/decry_"+filename;
                    //Response.Write("<script>alert('Hello');document.location='" + outputfile + "'</script>");
                }

            }

            return View();
        }

        [Route("UploadDocs")]
        [HttpPost]
        [SessionExpire]
        [SessionExpireRefNo]
        [ValidateAntiForgeryToken]
        public ActionResult UploadDocs(FormCollection collection)
        {
            try
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
                        TempData["Msg"] = "Please upload only.pdf file and File size Should Be UpTo 1 MB";
                        return RedirectToAction("ViewMeeting");
                    }
                    outputpath = baseUrl + "encry_" + file.FileName;
                    string inPath = baseUrl + file.FileName;
                    path += file.FileName;
                    file.SaveAs(Server.MapPath(path));
                    EncryptFile(inPath, outputpath);
                    FileInfo fi = new FileInfo(Server.MapPath(inPath));
                    fi.Delete();
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/UpdateMOMApproval?ID=" + meeting_id + "&filepath=" + outputpath + "&updatedby=" + userId).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        // model = response.Content.ReadAsAsync<SAVESOCVIEWMODEL>().Result;
                        TempData["Msg"] = "MOM Uploaded Successfully..!!";
                    }

                }
                return RedirectToAction("ViewMeeting");
            }
            catch(Exception e)
            {
                throw e;
            }
          
        }
        [Route("SearchFilter")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult SearchFilter(string type)
        {
            try
            {
                HtmlSanitizer sanitizer = new HtmlSanitizer();
                SechduleMeetingAgedaViewModel Socmodel = new SechduleMeetingAgedaViewModel();
                List<SechduleMeetingAgedaViewModel> listData = new List<SechduleMeetingAgedaViewModel>();
                int UserID = GetUserID();
                type = sanitizer.Sanitize(type);
                using (var client = new HttpClient())
                {
                    
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/CreateMeetings?type="+type+"&UserID=" + UserID).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        listData = response.Content.ReadAsAsync<List<SechduleMeetingAgedaViewModel>>().Result;
                    }
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/GetPrintStatus").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var PrintStatusList = response.Content.ReadAsAsync<List<tbl_print_history>>().Result;
                        ViewBag.PrintStatusList = PrintStatusList;
                    }
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/GetMailParticipants").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var MeetingPartcipantsList = response.Content.ReadAsAsync<List<tbl_trn_MeetingParticipants>>().Result;
                        ViewBag.MeetingPartcipantsList = MeetingPartcipantsList;
                    }
                }
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/GetMOMApproval").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var MOMApproval = response.Content.ReadAsAsync<List<tbl_mom_approval>>().Result;
                        ViewBag.MOMApproval = MOMApproval;
                    }
                }

                ViewBag.UserID = UserID;
                Socmodel.ListofMeeting = listData;
                return View("ViewMeeting", Socmodel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
          
        }

        [Route("EditSocMaster")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
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
            try
            {
                SechduleMeetingAgedaViewModel Socmodel = new SechduleMeetingAgedaViewModel();
                List<SechduleMeetingAgedaViewModel> listData = new List<SechduleMeetingAgedaViewModel>();
                int UserID = GetUserID();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/CreateMeetings?UserID=" + UserID).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        listData = response.Content.ReadAsAsync<List<SechduleMeetingAgedaViewModel>>().Result;
                    }
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/GetPrintStatus").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var PrintStatusList = response.Content.ReadAsAsync<List<tbl_print_history>>().Result;
                        ViewBag.PrintStatusList = PrintStatusList;
                    }
                }

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/GetMailParticipants").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var MeetingPartcipantsList = response.Content.ReadAsAsync<List<tbl_trn_MeetingParticipants>>().Result;
                        ViewBag.MeetingPartcipantsList = MeetingPartcipantsList;
                    }
                }
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/GetMOMApproval").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var MOMApproval = response.Content.ReadAsAsync<List<tbl_mom_approval>>().Result;
                        ViewBag.MOMApproval = MOMApproval;
                    }
                }

                ViewBag.UserID = UserID;
                Socmodel.ListofMeeting = listData;
                return View(Socmodel);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        [Route("PrepareFinalMeeting")]
        [HandleError]
        [HandleError(ExceptionType = typeof(NullReferenceException), Master = "Account", View = "Error")]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult PrepareFinalMeeting(string id, string mtype, string dated)
        {
            HtmlSanitizer sanitizer = new HtmlSanitizer();
            using (var client = new HttpClient())
            {
                Int16 mmID = Convert.ToInt16(Encryption.Decrypt(sanitizer.Sanitize(id)));
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
            SetParticipantSession();
            return View();
        }

        [Route("AddSocCommit")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult AddSocCommit(string id)
        {
            SocCommentViewModel model = new SocCommentViewModel();
            try
            {
                if (id != null)
                {
                    Int16 mID = Convert.ToInt16(Encryption.Decrypt(id));
                    model.SoCId = Convert.ToInt32(sanitizer.Sanitize(mID.ToString()));

                    if (Request.QueryString["item"] != null)
                    {
                        ViewBag.item = Request.QueryString["item"].ToString();
                        ViewBag.aonId = Encryption.Decrypt(Request.QueryString["item"].ToString());
                        ViewBag.id = id;
                        Session["item"] = Request.QueryString["item"].ToString();
                    }
                    else
                    {
                        ViewBag.item = Session["item"].ToString();
                        ViewBag.aonId = Encryption.Decrypt(Session["item"].ToString());
                        ViewBag.id = id;
                    }

                    List<SocCommentViewModel> listData = new List<SocCommentViewModel>();
                    int userId = Convert.ToInt32(Session["UserID"]);
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(WebAPIUrl);
                        //HTTP GET
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        HttpResponseMessage response = client.GetAsync("AONW/GetCommentt?socId=" + mID + "&ID=" + userId).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            listData = response.Content.ReadAsAsync<List<SocCommentViewModel>>().Result;
                            ViewBag.ListData = listData;
                        }
                    }
                   

                }
            }
            catch (Exception EX)
            {

            }
            return View(model);
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

                    string[] mData = dscsign(model.Comments);
                    model.DataValue = mData[0];
                    model.SignValue = mData[1];
                    model.IssuedTo = mData[2];
                    model.Path = mData[3];

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

            //ViewBag.aonId = Session["item"].ToString();
            //return View(model);
            return Json(Res, JsonRequestBehavior.AllowGet);
        }


        public string[] dscsign(string mData)
        {
            String[] dscData = new String[4];
            string requestUristring = string.Format("https://127.0.0.1:55103/dsc/getCertList");
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(AcceptAllCertifications);
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(requestUristring);
            HttpWebResponse myResp = (HttpWebResponse)myReq.GetResponse();
            System.IO.StreamReader respStreamReader = new System.IO.StreamReader(myResp.GetResponseStream());
            string responseString = respStreamReader.ReadToEnd();
            string str = responseString.ToString();
            string mxStr = str;


            XElement xmlResponse;
            xmlResponse = XElement.Parse(str); //Complete response file in product

            string signature = xmlResponse.LastNode.PreviousNode.ToString(); //Signature

            XmlDocument doc = new XmlDocument();
            doc.LoadXml(signature);
            string jsonText = JsonConvert.SerializeXmlNode(doc);


            Newtonsoft.Json.Linq.JObject oListjs1 = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(jsonText);
            string responses = Convert.ToString(Convert.ToString(oListjs1));
            Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(responses);

            string serialno = myDeserializedClass.CertificateDetail.SerialNumber;
            string mSubject = myDeserializedClass.CertificateDetail.IssuedTo;
            XmlSignature cs = new XmlSignature(serialno);
            cs.DigitalSignatureCertificate =

            //Load the signature certificate from Microsoft Certificate Store
            cs.DigitalSignatureCertificate = DigitalCertificate.LoadCertificate(false, "",
            "Select the certificate", "");
            cs.IncludeKeyInfo = true;
            cs.IncludeSignatureCertificate = true;
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version='1.0' encoding='UTF-8'?>");


            sb.AppendLine("<value>" + mData + "</value>");
            string revocationxml = sb.ToString();
            XmlDocument doc1 = new XmlDocument();
            doc1.LoadXml(revocationxml);
            doc1.Save(baseUrl + serialno + "test.xml");



            //apply the digital signature
            cs.ApplyDigitalSignature(baseUrl + serialno + "test.xml", baseUrl + serialno + "test[signed].xml");

            XmlSignature cv = new XmlSignature(serialno);
            //for SHA-256 signatures, use XmlSignatureSha256 class
            Console.WriteLine("Signatures: " + cv.GetNumberOfSignatures(baseUrl + serialno + "test[signed].xml"));
            ///verify the first signature
            Console.WriteLine("Status: " + cv.VerifyDigitalSignature(baseUrl + serialno + "test[signed].xml"));
            
            XmlDocument doc2 = new XmlDocument();
            doc2.Load(baseUrl + serialno + "test[signed].xml");
            string xml = doc2.OuterXml;
            string jsonTextnew = JsonConvert.SerializeXmlNode(doc2);
            Newtonsoft.Json.Linq.JObject oListjs2 = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(jsonTextnew);
            string responsesNEW = Convert.ToString(Convert.ToString(oListjs2));

            RootNEW mySignature = JsonConvert.DeserializeObject<RootNEW>(responsesNEW);
            string DigestValue = mySignature.value.Signature.SignedInfo.Reference.DigestValue;
            string SignatureValue = mySignature.value.Signature.SignatureValue;
            string X509Certificate = mySignature.value.Signature.KeyInfo.X509Data.X509Certificate;
            string crtfile = serialno + DateTime.Now.ToString("HHmmss");
            FileStream fs1 = new FileStream(baseUrl + crtfile + ".txt", FileMode.OpenOrCreate, FileAccess.Write);
            StreamWriter writer = new StreamWriter(fs1);
            writer.Write(X509Certificate);
            writer.Close();

            string myfile = baseUrl + crtfile + ".txt";
            FileInfo f = new FileInfo(myfile);
            f.MoveTo(Path.ChangeExtension(myfile, ".crt"));
            
            dscData[0] = X509Certificate;
            dscData[1] = DigestValue;
            dscData[2] = SignatureValue;
            dscData[3] = crtfile + ".crt";
            return dscData;
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class Xml
        {
            [JsonProperty("@version")]
            public string Version { get; set; }

            [JsonProperty("@encoding")]
            public string Encoding { get; set; }
        }

        public class CanonicalizationMethod
        {
            [JsonProperty("@Algorithm")]
            public string Algorithm { get; set; }
        }

        public class SignatureMethod
        {
            [JsonProperty("@Algorithm")]
            public string Algorithm { get; set; }
        }

        public class Transform
        {
            [JsonProperty("@Algorithm")]
            public string Algorithm { get; set; }
        }

        public class Transforms
        {
            public List<Transform> Transform { get; set; }
        }

        public class DigestMethod
        {
            [JsonProperty("@Algorithm")]
            public string Algorithm { get; set; }
        }

        public class Reference
        {
            [JsonProperty("@URI")]
            public string URI { get; set; }
            public Transforms Transforms { get; set; }
            public DigestMethod DigestMethod { get; set; }
            public string DigestValue { get; set; }
        }

        public class SignedInfo
        {
            public CanonicalizationMethod CanonicalizationMethod { get; set; }
            public SignatureMethod SignatureMethod { get; set; }
            public Reference Reference { get; set; }
        }

        public class RSAKeyValue
        {
            public string Modulus { get; set; }
            public string Exponent { get; set; }
        }

        public class KeyValue
        {
            public RSAKeyValue RSAKeyValue { get; set; }
        }

        public class X509Data
        {
            public string X509Certificate { get; set; }
        }

        public class KeyInfo
        {
            public KeyValue KeyValue { get; set; }
            public X509Data X509Data { get; set; }
        }

        public class Signature
        {
            [JsonProperty("@xmlns")]
            public string Xmlns { get; set; }
            public SignedInfo SignedInfo { get; set; }
            public string SignatureValue { get; set; }
            public KeyInfo KeyInfo { get; set; }
        }

        public class Value
        {
            [JsonProperty("#text")]
            public string Text { get; set; }
            public Signature Signature { get; set; }
        }

        public class RootNEW
        {
            [JsonProperty("?xml")]
            public Xml Xml { get; set; }
            public Value value { get; set; }
        }


        public Boolean AcceptAllCertifications(Object sender, System.Security.Cryptography.X509Certificates.X509Certificate certification, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse); 
        public class RevocationRequest
        {
            public string SerialNumber { get; set; }
            public string CdpPoint { get; set; }
            public string CertLevel { get; set; }
        }

        public class Revocation
        {
            public List<RevocationRequest> RevocationRequest { get; set; }
        }

        public class CertificateDetail
        {
            [JsonProperty("@cdpPoint")]
            public string CdpPoint { get; set; }

            [JsonProperty("@certType")]
            public string CertType { get; set; }

            [JsonProperty("@hasChain")]
            public string HasChain { get; set; }

            [JsonProperty("@issuedBy")]
            public string IssuedBy { get; set; }

            [JsonProperty("@issuedTo")]
            public string IssuedTo { get; set; }

            [JsonProperty("@notAfter")]
            public string NotAfter { get; set; }

            [JsonProperty("@serialNumber")]
            public string SerialNumber { get; set; }
            public Revocation Revocation { get; set; }
        }

        public class Root
        {
            public CertificateDetail CertificateDetail { get; set; }
        }

        [HttpPost]
        [Route("SaveMeetings")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveMeetings(SechduleMeetingAgedaViewModel model)
        {
                try
                {
                    List<MeetingParticipants> listData = new List<MeetingParticipants>();
                if (Session["participants"] != null)
                {
                    listData = Session["participants"] as List<MeetingParticipants>;
                    if (model.officers_participated !=null)
                    {
                        listData = listData.Where(x => model.officers_participated.Contains(x.UserID.ToString()) && x.meeting_type == model.dac_dpb).ToList();
                    }
                }
                    model.Participants = new List<MeetingParticipants>();
                    model.Participants = listData;

                    model.Remarks = sanitizer.Sanitize(model.Remarks);
                    model.Meeting_Number = sanitizer.Sanitize(model.Meeting_Number)+sanitizer.Sanitize(model.Meeting_Year);
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
            
            return View("createMeeting");
        }
        [Route("UpdatePrintTrials")]
        [HandleError]
        [SessionExpire]
        public string UpdatePrintTrials(string ID)
        {
            int mID = Convert.ToInt32(Encryption.Decrypt(ID));

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("AONW/UpdatePrintTrials?meetingId=" + mID + "&userId="+Session["UserID"]).Result;
                if (response.IsSuccessStatusCode)
                {

                }
            }
            return "";
        }

        [Route("GenerateReport")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult GenerateReport(string ID, string Version = null)
        {
            HtmlSanitizer sanitizer = new HtmlSanitizer();
            Int16 mID = Convert.ToInt16(Encryption.Decrypt(sanitizer.Sanitize(ID)));

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
        [Route("EmailToMeetingParticipants")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult EmailToMeetingParticipants(string ID)
        {
            HtmlSanitizer sanitizer = new HtmlSanitizer();
            List<MeetingParticipants> listData = new List<MeetingParticipants>();
            Int16 mID = Convert.ToInt16(Encryption.Decrypt(sanitizer.Sanitize(ID)));
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
        [Route("SendMailToParticiants")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult SendMailToParticiants()
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
        [HttpPost]
        [Route("GenerateAONNumber")]
        public ActionResult GenerateAONNumber(string meeting_id,string agenda_description,string TypeofAgenda,string Pid)
        {

            int meetingId = Convert.ToInt32(Encryption.Decrypt(meeting_id));
            try
            {
                SechduleMeetingAgedaViewModel model = new SechduleMeetingAgedaViewModel();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/EditMeeting?ID=" + meetingId + "").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        model = response.Content.ReadAsAsync<SechduleMeetingAgedaViewModel>().Result;
                        model.meeting_id = meetingId.ToString();
                    }

                    string socCase = "";
                    using (var client1 = new HttpClient())
                    {
                        client1.BaseAddress = new Uri(WebAPIUrl);
                        //HTTP GET
                        client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        response = client.GetAsync("AONW/getSocCaseNo?ID=" + TypeofAgenda + "").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var obj = response.Content.ReadAsAsync<acq_soc_master>().Result;
                            socCase = obj.SoCCase;
                            socCase = Encryption.Decrypt(socCase);
                           
                        }
                    }
                    
                    string type = model.dac_dpb;
                    string meeting_number = model.Meeting_Number;
                    string service = "";
                    if(agenda_description.ToLower().Contains("army"))
                    {
                        service = "IA";
                    }
                    else if (agenda_description.ToLower().Contains("icg"))
                    {
                        service = "CG";
                    }
                    else if (agenda_description.ToLower().Contains("navy"))
                    {
                        service = "IN";
                    }
                    else if (agenda_description.ToLower().Contains("joint"))
                    {
                        service = "js";
                    }
                    else if (agenda_description.ToLower().Contains("airforce"))
                    {
                        service = "AF";
                    }
                    string AonNumber = type + meeting_number + service+socCase;

                    using (var client2 = new HttpClient())
                    {
                        client2.BaseAddress = new Uri(WebAPIUrl);
                        //HTTP GET
                        client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        response = client.GetAsync("AONW/updateAONNumber?aon=" + AonNumber + "&pid="+Pid).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var obj = response.Content.ReadAsStringAsync().Result;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("ViewMeeting");
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
        [SessionExpire]
        [SessionExpireRefNo]
        [HandleError]
        public ActionResult ViewMeetingComments(string Id)
        {
            try
            {
                HtmlSanitizer sanitizer = new HtmlSanitizer();
                int userID = GetUserID();

                if (Convert.ToInt32(Session["SectionId"]) == 1 && Convert.ToInt32(Session["SectionId"]) == 12)
                {
                    userID = 0;
                }
                dynamic listData = null;
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/GetComments?ID=" + sanitizer.Sanitize(Id) + "&userID=" + userID).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        listData = response.Content.ReadAsAsync<List<tbl_trn_MeetingAgendaComments>>().Result;
                    }
                }
                ViewBag.CommentList = listData;
                ViewBag.Meeting_id = Id;
                return View();
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        [SessionExpire]
        [HandleError]
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
                       var res=postJob.Content.ReadAsAsync<MeetingAgenda>().Result;
                        TempData["Msg"] =res.Msg;

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
            HtmlSanitizer sanitizer = new HtmlSanitizer();
            ViewBag.mtype = sanitizer.Sanitize(mtype);
            ViewBag.dated = sanitizer.Sanitize(dated);
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
            try
            {
                HtmlSanitizer sanitizer = new HtmlSanitizer();
                MeetingAgenda model = new MeetingAgenda();
                int UserID = GetUserID();

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/EditMeetingAgenda?ID=" + sanitizer.Sanitize(ID.ToString()) + "&UserID=" + UserID).Result;
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

        [HttpGet]
        [Route("UploadApprovalDocs")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult UploadApprovalDocs(string ID,string mtype,string dated)
        {
            try
            {
                HtmlSanitizer sanitizer = new HtmlSanitizer();
                string Version = null;
                ViewBag.Meeting_id = sanitizer.Sanitize(ID);
                ViewBag.mtype = Encryption.Decrypt(sanitizer.Sanitize(mtype));
                ViewBag.dated = Encryption.Decrypt(sanitizer.Sanitize(dated));
                int mID = Convert.ToInt32(Encryption.Decrypt(ID));
                SechduleMeetingAgedaViewModel model = new SechduleMeetingAgedaViewModel();
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
                        ViewBag.MeetingAgendaList = listData;


                    }
                }
                return View();
            }
            catch(Exception e)
            {
                throw e;
            }
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
            string mtg_Id = Encryption.Encrypt(meeting_id.ToString());
            return RedirectToAction("AddMeetingAgenda", "AONW", new { id = mtg_Id, ViewBag.mtype, ViewBag.dated });
        }

        [Route("SocTimeline")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult SocTimeline()
        {
            return View();
        }




        #endregion

        #region UpdateSOC
        [Route("UpdateSOC")]
        [HttpGet]
        [SessionExpire]
        [HandleError]
        public ActionResult UpdateSOC(string ID)
        {
            HtmlSanitizer sanitizer = new HtmlSanitizer();
            try
            {
                ID = Encryption.Decrypt(sanitizer.Sanitize(ID));
                SAVESOCVIEWMODEL model = new SAVESOCVIEWMODEL();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                          parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("AONW/EditSocMaster?ID=" + ID + "").Result;
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
                    return View(model);

                }
            }
            catch(Exception e)
            {
                throw e;
            }
        }
        [HttpPost]
        [Route("UpdateSOC")]
        [SessionExpire]
        [SessionExpireRefNo]
        [HandleError]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateSOC(SAVESOCVIEWMODEL model)
        {
            try
            {
                HtmlSanitizer sentizer = new HtmlSanitizer();
                model.item_description = Encryption.Encrypt(sentizer.Sanitize(model.item_description));
                model.Service_Lead_Service = Encryption.Encrypt(sentizer.Sanitize(model.Service_Lead_Service));
                model.SoCCase = Encryption.Encrypt(sentizer.Sanitize(model.SoCCase));
                model.Quantity = Encryption.Encrypt(sentizer.Sanitize(model.Quantity));
                model.Cost = Encryption.Encrypt(sentizer.Sanitize(model.Cost));
                model.Tax_Duties = Encryption.Encrypt(sentizer.Sanitize(model.Tax_Duties));
                model.IC_percentage = Encryption.Encrypt(sentizer.Sanitize(model.IC_percentage));
                model.Essential_parameters = Encryption.Encrypt(sentizer.Sanitize(model.Essential_parameters));
                model.Any_other_aspect = Encryption.Encrypt(sentizer.Sanitize(model.Any_other_aspect));
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(WebAPIUrl);
                HttpResponseMessage postJob = await client.PostAsJsonAsync<SAVESOCVIEWMODEL>(WebAPIUrl + "AONW/UpdateSOC", model);
                bool postResult = postJob.IsSuccessStatusCode;
                if (postResult == true)
                {
                    //TempData["Msg"] = "Record Saved Successfully";
                }
                else
                {
                    //TempData["Msg"] = "Record Not Saved Successfully";

                }
                ViewBag.Msg = "Updated Successfully";
            }
            catch(Exception e)
            {
                throw e;
            }

            return RedirectToAction("UpdateSoc", new { ID = model.aon_id });
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
            if(TempData["File"]!=null)
            {
                ViewBag.File = TempData["File"].ToString();
            }
            if (TempData["FileStage"]!=null)
            {
                ViewBag.FileStage = TempData["FileStage"].ToString();
            }
            if (TempData["FieldName"] != null)
            {
                ViewBag.FieldName = TempData["FieldName"].ToString();
            }
            if (TempData["ExcelColoumn"]!=null)
            {
                ViewBag.ExcelColoumn = TempData["ExcelColoumn"].ToString();
            }
            if(TempData["Uploadsuccess"]!=null)
            {
                bool upload =(bool)TempData["Uploadsuccess"];
                if(upload)
                {
                    ViewBag.Uploadsuccess = "True";
                }
                else
                {
                    ViewBag.Uploadsuccess = "False";

                }

                TempData["Uploadsuccess"] = null;


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
                ContractDetails contractDetails = new ContractDetails();
                contractDetails.ContractId = sanitizer.Sanitize(cnt.Contrct_Detail.ContractId);
                contractDetails.Contract_Number = sanitizer.Sanitize(cnt.Contrct_Detail.Contract_Number);
                if (cnt.Contrct_Detail.Services != null)
                {
                    contractDetails.Services = sanitizer.Sanitize(cnt.Contrct_Detail.Services.ToString());
                }
                if (cnt.Contrct_Detail.DateOfContractSigning != null)
                {
                    contractDetails.DateOfContractSigning = Convert.ToDateTime(sanitizer.Sanitize(cnt.Contrct_Detail.DateOfContractSigning.ToString()));
                }
                if(cnt.Contrct_Detail.Descriptions != null)
                {
                    contractDetails.Descriptions = sanitizer.Sanitize(cnt.Contrct_Detail.Descriptions);
                }

                if (cnt.Contrct_Detail.Category != null)
                {
                    contractDetails.Category = sanitizer.Sanitize(cnt.Contrct_Detail.Category);
                }

                

                if (cnt.Contrct_Detail.EffectiveDate != null)
                {
                    contractDetails.EffectiveDate = Convert.ToDateTime(sanitizer.Sanitize(cnt.Contrct_Detail.EffectiveDate.ToString()));
                }
                if (cnt.Contrct_Detail.ABGDate != null)
                {
                    contractDetails.ABGDate = Convert.ToDateTime(sanitizer.Sanitize(cnt.Contrct_Detail.ABGDate.ToString()));
                }
                if (cnt.Contrct_Detail.PWBGPercentage != null)
                {
                    contractDetails.PWBGPercentage = Convert.ToInt32(sanitizer.Sanitize(cnt.Contrct_Detail.PWBGPercentage.ToString()));
                }
                if (cnt.Contrct_Detail.PWBGDate != null)
                {
                    contractDetails.PWBGDate = Convert.ToDateTime(sanitizer.Sanitize(cnt.Contrct_Detail.PWBGDate.ToString()));
                }
                if (cnt.Contrct_Detail.Incoterms != null)
                {
                    contractDetails.Incoterms = sanitizer.Sanitize(cnt.Contrct_Detail.Incoterms);
                }
                if (cnt.Contrct_Detail.Warranty != null)
                {
                    contractDetails.Warranty = sanitizer.Sanitize(cnt.Contrct_Detail.Warranty);
                }
                if (cnt.Contrct_Detail.ContractValue != null)
                {
                    contractDetails.ContractValue = Convert.ToDecimal(sanitizer.Sanitize(cnt.Contrct_Detail.ContractValue.ToString()));
                }
                if (cnt.Contrct_Detail.FEContent != null)
                {
                    contractDetails.FEContent = sanitizer.Sanitize(cnt.Contrct_Detail.FEContent);
                }
                if (cnt.Contrct_Detail.TaxesAndDuties != null)
                {
                    contractDetails.TaxesAndDuties = sanitizer.Sanitize(cnt.Contrct_Detail.TaxesAndDuties);
                }
                if (cnt.Contrct_Detail.FinalDeliveryDate != null)
                {
                    contractDetails.FinalDeliveryDate = Convert.ToDateTime(sanitizer.Sanitize(cnt.Contrct_Detail.FinalDeliveryDate.ToString()));
                }
                if (cnt.Contrct_Detail.GracePeriod != null)
                {
                    contractDetails.GracePeriod = sanitizer.Sanitize(cnt.Contrct_Detail.GracePeriod);
                }


                _contracts.Contrct_Detail = contractDetails;

                List<StageDetail> stages = new List<StageDetail>();
                foreach (var item in cnt.Stage_Detail.ToList())
                {
                    StageDetail stageDetail = new StageDetail();

                    stageDetail.ContractId = sanitizer.Sanitize(cnt.Contrct_Detail.ContractId);
                    stageDetail.StageNumber = Convert.ToInt32(sanitizer.Sanitize(item.StageNumber.ToString()));
                    stageDetail.stageDescription = sanitizer.Sanitize(item.stageDescription);
                    if (item.StageCompletionDate != null)
                    {
                        stageDetail.StageStartdate = Convert.ToDateTime(sanitizer.Sanitize(item.StageStartdate.ToString()));
                    }
                    if (item.StageCompletionDate != null)
                    {
                        stageDetail.StageCompletionDate = Convert.ToDateTime(sanitizer.Sanitize(item.StageCompletionDate.ToString()));
                    }
                    if (item.PercentOfContractValue != null)
                    {
                        stageDetail.PercentOfContractValue = Convert.ToInt32(sanitizer.Sanitize(item.PercentOfContractValue.ToString()));
                    }
                    if (item.Amount > 0)
                    {
                        stageDetail.Amount = Convert.ToDecimal(sanitizer.Sanitize(item.Amount.ToString()));
                    }
                    if (item.DueDateOfPayment != null)
                    {
                        stageDetail.DueDateOfPayment = Convert.ToDateTime(sanitizer.Sanitize(item.DueDateOfPayment.ToString()));
                    }
                    if (item.Conditions != null)
                    {
                        stageDetail.Conditions = sanitizer.Sanitize(item.Conditions);
                    }
                    if (item.RevisedDateOfpayment != null)
                    {
                        stageDetail.RevisedDateOfpayment = Convert.ToDateTime(sanitizer.Sanitize(item.RevisedDateOfpayment.ToString()));
                    }
                    if (item.ReasonsForSlippage != null)
                    {
                        stageDetail.ReasonsForSlippage = sanitizer.Sanitize(item.ReasonsForSlippage);
                    }
                    if (item.ActualDateOfPayment != null)
                    {
                        stageDetail.ActualDateOfPayment = Convert.ToDateTime(sanitizer.Sanitize(item.ActualDateOfPayment.ToString()));
                    }

                    stageDetail.TotalPaymentMade = Convert.ToDecimal(sanitizer.Sanitize(item.TotalPaymentMade.ToString()));
                    stageDetail.FullorPartPaymentMade = sanitizer.Sanitize(item.FullorPartPaymentMade);
                    if (item.ExpendMadeTill31March != null)
                    {
                        stageDetail.ExpendMadeTill31March = Convert.ToDecimal(sanitizer.Sanitize(item.ExpendMadeTill31March.ToString()));
                    }

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
                i = 0;
                ViewBag.Message = ex.Message.ToString();
                return Json(i, JsonRequestBehavior.AllowGet);
            }
            return Json(i, JsonRequestBehavior.AllowGet);
        }
        #endregion
    }
}