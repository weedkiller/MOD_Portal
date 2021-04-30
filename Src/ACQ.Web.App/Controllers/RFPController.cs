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

namespace ACQ.Web.App.Controllers
{
    public class RFPController : Controller
    {
        // GET: RFP

        HtmlSanitizer sanitizer = new HtmlSanitizer();
        private static string WebAPIUrl = ConfigurationManager.AppSettings["APIUrl"].ToString();
        public decimal filesize { get; set; }

        public RFPController()
        {
            if (BruteForceAttackss.bcontroller != "")
            {
                if (BruteForceAttackss.bcontroller == "RFP")
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
                else
                {
                    BruteForceAttackss.refreshcount = 0;
                    BruteForceAttackss.date = null;
                    BruteForceAttackss.bcontroller = "RFP";
                }
            }
            else
            {
                BruteForceAttackss.bcontroller = "RFP";
            }
        }



        [Route("ViewRFP")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        [HttpGet]
        public ActionResult ViewRFP()
        {
            IEnumerable<Service> listdata = new List<Service>();
            IEnumerable<ListRfpServices> SOCData = new List<ListRfpServices>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebAPIUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                         parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                HttpResponseMessage response = client.GetAsync("RFP/GetServices").Result;
                if (response.IsSuccessStatusCode)
                {
                    listdata = response.Content.ReadAsAsync<IEnumerable<Service>>().Result;
                }
            }

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebAPIUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                         parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                HttpResponseMessage response1 = client.GetAsync("RFP/GetSOCData").Result;
                if (response1.IsSuccessStatusCode)
                {
                    SOCData = response1.Content.ReadAsAsync<IEnumerable<ListRfpServices>>().Result;
                }
            }

            ViewBag.services = listdata;
            if(SOCData!=null && SOCData.Count()>0 && Session["SectionID"]!=null)
            {
                if(Convert.ToInt32(Session["SectionID"])==14)
                {
                    var data = SOCData.Where(x => x.Service_Lead_Service.ToLower() == "airforce").ToList();
                    if(data!=null && data.Count()>0)
                    {
                        ViewBag.SOC = data;
                    }
                    
                }
                else if (Convert.ToInt32(Session["SectionID"]) == 11)
                {
                    var data = SOCData.Where(x => x.Service_Lead_Service.ToLower() == "army").ToList();
                    if (data != null && data.Count() > 0)
                    {
                        ViewBag.SOC = data;
                    }

                }
                else if (Convert.ToInt32(Session["SectionID"]) == 15)
                {
                    var data = SOCData.Where(x => x.Service_Lead_Service.ToLower() == "navy").ToList();
                    if (data != null && data.Count() > 0)
                    {
                        ViewBag.SOC = data;
                    }

                }
                else if (Convert.ToInt32(Session["SectionID"]) == 16)
                {
                    var data = SOCData.Where(x => x.Service_Lead_Service.ToLower() == "icg").ToList();
                    if (data != null && data.Count() > 0)
                    {
                        ViewBag.SOC = data;
                    }

                }
                else if (Convert.ToInt32(Session["SectionID"]) == 17)
                {
                    var data = SOCData.Where(x => x.Service_Lead_Service.ToLower() == "ids").ToList();
                    if (data != null && data.Count() > 0)
                    {
                        ViewBag.SOC = data;
                    }

                }
            }
            
            return View();
        }

        [Route("RFPComments")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        [HttpGet]
        public ActionResult RFPComments()
        {
            IEnumerable<sharedRFP> SOCData = new List<sharedRFP>();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebAPIUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                         parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                HttpResponseMessage response = client.GetAsync("RFP/GetCommentedrfp").Result;
                if (response.IsSuccessStatusCode)
                {
                    SOCData = response.Content.ReadAsAsync<IEnumerable<sharedRFP>>().Result;
                }
            }

            
            if (SOCData != null && SOCData.Count() > 0 && Session["SectionID"] != null)
            {
                if (Convert.ToInt32(Session["SectionID"]) == 14)
                {
                    var data = SOCData.Where(x => x.Service.ToLower() == "airforce").ToList();
                    if (data != null && data.Count() > 0)
                    {
                        ViewBag.SOC = data;
                    }

                }
                else if (Convert.ToInt32(Session["SectionID"]) == 11)
                {
                    var data = SOCData.Where(x => x.Service.ToLower() == "army").ToList();
                    if (data != null && data.Count() > 0)
                    {
                        ViewBag.SOC = data;
                    }

                }
                else if (Convert.ToInt32(Session["SectionID"]) == 15)
                {
                    var data = SOCData.Where(x => x.Service.ToLower() == "navy").ToList();
                    if (data != null && data.Count() > 0)
                    {
                        ViewBag.SOC = data;
                    }

                }
                else if (Convert.ToInt32(Session["SectionID"]) == 16)
                {
                    var data = SOCData.Where(x => x.Service.ToLower() == "icg").ToList();
                    if (data != null && data.Count() > 0)
                    {
                        ViewBag.SOC = data;
                    }

                }
                else if (Convert.ToInt32(Session["SectionID"]) == 17)
                {
                    var data = SOCData.Where(x => x.Service.ToLower() == "ids").ToList();
                    if (data != null && data.Count() > 0)
                    {
                        ViewBag.SOC = data;
                    }

                }
            }
            return View();
        }
        [Route("CollegiateMeetings")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        [HttpGet]
        public ActionResult CollegiateMeetings()
        {
            return View();
        }
        [Route("RODApproval")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        [HttpGet]
        public ActionResult RODApproval()
        {
            return View();
        }
        [Route("FinalRFPUpload")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        [HttpGet]
        public ActionResult FinalRFPUpload()
        {
            return View();
        }
        [Route("IssueOfRFP")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        [HttpGet]
        public ActionResult IssueOfRFP()
        {
            return View();
        }


        [Route("ViewUploadedRFP")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        [HttpGet]
        public ActionResult ViewUploadedRFP()
        {
            IEnumerable<sharedRFP> sharedRFP = new List<sharedRFP>();
            var id = Session["UserID"].ToString();
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Clear();
                client.BaseAddress = new Uri(WebAPIUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                         parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                HttpResponseMessage response = client.GetAsync("RFP/Getsharedrfp?UserId="+ id).Result;
                if (response.IsSuccessStatusCode)
                {
                    sharedRFP = response.Content.ReadAsAsync<IEnumerable<sharedRFP>>().Result;
                }
            }
            return View(sharedRFP);
        }


        [Route("UploadComments")]
        [HandleError]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UploadComments(UploadComment model)
        {
            bool taskstatus = false;
            if (ModelState.IsValid)
            {
                if (model.MyFile != null && FileCheckformat(model.MyFile, Path.GetExtension(model.MyFile.FileName)))
                {
                    
                    var content = model.MyFile.ContentType;
                    string path = Path.Combine(Server.MapPath("~/UploadSOC"), Path.GetFileName(model.MyFile.FileName));
                    model.MyFile.SaveAs(path);
                    sharedRFP shared = new sharedRFP();
                    shared.Id= Convert.ToInt32(sanitizer.Sanitize(model.CommentId.ToString()));
                    shared.UploadedComment = "~/UploadSOC/" + model.MyFile.FileName;
                    using (var client = new HttpClient())
                    { 
                        client.DefaultRequestHeaders.Clear();
                    client.BaseAddress = new Uri(WebAPIUrl);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                             parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                    HttpResponseMessage response = await client.PostAsJsonAsync<sharedRFP>(WebAPIUrl + "RFP/UploadComments", shared);
                    bool postResult = response.IsSuccessStatusCode;
                        if(postResult)
                        {
                            taskstatus = true;
                        }
                    }

                }
            }
                return Json(taskstatus, JsonRequestBehavior.AllowGet);
        }

        public bool FileCheckformat(HttpPostedFileBase file, string mFileExtension)
        {

            filesize = 1024;
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
            string contenttype = String.Empty;
            Stream checkStream = file.InputStream;
            BinaryReader chkBinary = new BinaryReader(checkStream);
            Byte[] chkbytes = chkBinary.ReadBytes(0x10);

            string data_as_hex = BitConverter.ToString(chkbytes);
            string magicCheck = data_as_hex.Substring(0, 11);

            //Set the contenttype based on File Extension
            switch (magicCheck)
            {
                case "FF-D8-FF-E1":
                    contenttype = "image/jpg";
                    break;
                case "FF-D8-FF-E0":
                    contenttype = "image/jpeg";
                    break;
                case "25-50-44-46":
                    contenttype = "text/pdf";
                    break;
                case "D0-CF-11-E0":
                    contenttype = "text/xls";
                    break;
                case "50-4B-03-04":
                    contenttype = "text/xlsx";
                    break;

            }
            if (contenttype != String.Empty)
            {
                Byte[] bytes = chkBinary.ReadBytes((Int32)checkStream.Length);


                if (file.ContentType != "application/pdf")
                {
                    return false;
                }
                else
                {

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
            else
            {
                return false;
            }




        }

        [Route("GetRFPdata")]
        [HandleError]
        [ValidateAntiForgeryToken]
        public JsonResult GetRFPdata(int service=0)
        {
            ApiResponseRfp responseAPI = new ApiResponseRfp();
            if(service>0)
            {
                try
                {
                    var aon = Convert.ToInt32(sanitizer.Sanitize(service.ToString()));
                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.BaseAddress = new Uri(WebAPIUrl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                                 parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                        HttpResponseMessage response = client.GetAsync("RFP/GetSOCFilterData?aonId=" + aon).Result;
                        if (response.IsSuccessStatusCode)
                        {
                            responseAPI = response.Content.ReadAsAsync<ApiResponseRfp>().Result;
                        }
                    }

                }
                catch (Exception e)
                {
                    responseAPI.Status = false;
                    responseAPI.Message = "Service are temporarily unavailable.";
                }
            }
            else
            {
                responseAPI.Status = false;
                responseAPI.Message = "Incorrect input provided...";
            }
            return Json(responseAPI, JsonRequestBehavior.AllowGet);
        }

        [Route("Updatevendor")]
        [HandleError]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdatevendorType(int VendorType = 0, int Id = 0)
        {
            if (VendorType != 0 && Id != 0)
            { bool Updatestatus = false;
                try
                {
                    AttachmentData model = new AttachmentData();
                    model.Id= Convert.ToInt32(sanitizer.Sanitize(Id.ToString()));
                    model.VendorType = Convert.ToInt32(sanitizer.Sanitize(VendorType.ToString()));

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.BaseAddress = new Uri(WebAPIUrl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                                 parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                        HttpResponseMessage response =await client.PostAsJsonAsync<AttachmentData>(WebAPIUrl + "RFP/UpdateVendorType", model);
                        bool postResult = response.IsSuccessStatusCode;
                        if(postResult)
                        {
                            Updatestatus = true;
                        }
                    }

                }
                catch (Exception ex)
                {
                    Updatestatus = false;
                }
                return Json(Updatestatus, JsonRequestBehavior.AllowGet);
            }
            else return Json(false, JsonRequestBehavior.AllowGet);

        }


        [Route("sharedraftrfp")]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> sharedraftrfp(int Id = 0)
        {
            IEnumerable<UserViewModel> Users = new List<UserViewModel>();
            string roles = "";
            bool sent = false;
            if (Id != 0)
            {
                try
                {
                    AttachmentData model = new AttachmentData();
                    model.Id = Convert.ToInt32(sanitizer.Sanitize(Id.ToString()));

                    using (var client = new HttpClient())
                    {
                        client.DefaultRequestHeaders.Clear();
                        client.BaseAddress = new Uri(WebAPIUrl);
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                                 parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                        HttpResponseMessage response = await client.PostAsJsonAsync<AttachmentData>(WebAPIUrl + "RFP/SendDraftRFP", model);
                      
                        if (response.IsSuccessStatusCode)
                        {
                            Users= response.Content.ReadAsAsync<IEnumerable<UserViewModel>>().Result;
                        }
                    }

                    

                    if(Users!=null && Users.Count()>0)
                    {
                        foreach(var item in Users)
                        {
                            string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/EscalationEmailFormat.html"));
                            string Messge = "Draft RFP uploaded. Please login to your account to download and provide comments.";
                            EmailHelper.sendEmailEscalation(item.UserEmail, Messge, mailPath);
                            if(string.IsNullOrEmpty(roles))
                            {
                                roles = roles + item.Designation;
                            }
                            else
                            {
                                roles = roles +","+ item.Designation;
                            }
                            
                        }
                    }
                    sent = true;
                }
                catch (Exception ex)
                {
                    sent = false;
                }
                
            }
            else
            {
                sent = false;
            }
            return Json(new { Status = sent,Sendto= roles }, JsonRequestBehavior.AllowGet);
        }





        [Route("viewfile")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public FileResult viewfile(string filename)
        {
            string ReportURL = Server.MapPath("~/UploadSOC/"+filename);
            byte[] FileBytes = System.IO.File.ReadAllBytes(ReportURL);
            return File(FileBytes, "application/pdf");
        }


        
    }
}