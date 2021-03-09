using ACQ.Web.ViewModel.EFile;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Linq;
using System.Data;
using static ACQ.Web.ViewModel.EFile.Efile;
using System.Text;
 

namespace ACQ.Web.App.Controllers
{
    public class EFileController : Controller
    {
        private static string WebAPIUrl = ConfigurationManager.AppSettings["APIUrl"].ToString();
        private static string UploadfilePath = ConfigurationManager.AppSettings["SOCPath"].ToString();
        public ActionResult Index(Efile.SAVEEfile _model)
        {
            try
            {
                if (!string.IsNullOrEmpty(Session["UserID"].ToString()))
                {
                    int ids = 0;

                    //if (ID != null)
                    //    ids = (Int32)ID;
                    //TempData["a_onId"] = ID;
                    //TempData.Keep("a_onId");
                    //ViewData["IsUpdate"] = true;

                    //    string strContent = "An SOC " + Convert.ToDateTime(_model.SoDate).ToString("dd/MM/yyyy") + " has been received for " + _model.item_description.ToString() + "   for service.It is proposed the SOC may be send to all DAC commitee members.";
                    //    _model.EditorContent = strContent;

                    _model.Subjects = "Approval for sending SOC to DAC/DPB committee members";
                    ids = _model.aon_id;
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(WebAPIUrl);
                        //HTTP GET
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        HttpResponseMessage response = client.GetAsync("Efile/PopulateSOCDescription").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            var data = response.Content.ReadAsAsync<List<Efile.SAVEEfile>>().Result;

                            ViewBag.SOCDesription = data.Select(x => new SelectListItem { Text = x.item_description, Value = x.aon_id.ToString() });
                        }
                    }

                    return View(_model);
                }
                else { return Redirect("/account/login"); }
            }
            catch (Exception ex) { return Redirect("/account/login"); }
        }
        [HttpPost]
        public ActionResult UploadFiles(Efile.SAVEEfile _efile)
        {
            if (!string.IsNullOrEmpty(Session["UserID"].ToString()))
            {
                string FileNames = Request["FileNames"];
                HttpFileCollectionBase files = Request.Files;
                List<Efile.FileDetail> fileDetails = new List<Efile.FileDetail>();
                int IDS = 1;

                for (int i = 0; i < files.Count; i++)
                {
                    HttpPostedFileBase file = files[i];
                    string strFileNames = Request.Files.Keys[i].ToString();
                    string fname;
                    // Checking for Internet Explorer   

                    if (Request.Browser.Browser.ToUpper() == "IE" || Request.Browser.Browser.ToUpper() == "INTERNETEXPLORER")
                    {
                        string[] testfiles = file.FileName.Split(new char[] { '\\' });
                        fname = testfiles[testfiles.Length - 1];
                    }
                    else
                    {
                        fname = file.FileName;
                        if (file != null && file.ContentLength > 0)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            Efile.FileDetail fileDetail = new Efile.FileDetail()
                            {
                                FileName = strFileNames,
                                FilePath = UploadfilePath.Replace("~","") + fileName,
                                Id = IDS
                            };
                            fileDetails.Add(fileDetail);
                            var path = Path.Combine(Server.MapPath(UploadfilePath), fileDetail.FileName);
                            file.SaveAs(path);
                            _efile._FileDetail = fileDetails;
                        }
                    }
                    TempData["FileDetails"] = _efile._FileDetail;
                    TempData.Keep("FileDetails");
                }
                return Json("Uploaded successfully", JsonRequestBehavior.AllowGet);
            }
            else { return Redirect("/account/login"); }
        }

        [HttpPost]
        public async Task<JsonResult> SubmitEfile(Efile.SAVEEfile _model)
        {

            _model._FileDetail = TempData.Peek("FileDetails") as ICollection<Efile.FileDetail>;
            _model.ContentType = "N1";
            if (TempData["FileDetails"] != null)
            {
                _model.XmlFileDetails = EfileData.CreateXML(_model._FileDetail);
                using (var client = new HttpClient())
                {
                    if (!string.IsNullOrEmpty(_model.aon_ids))
                        _model.aon_ids = _model.aon_ids.TrimStart(new Char[] { ',', '*', '.' });
                    _model.CreatedBy = Convert.ToInt32(Session["UserId"]);
                    client.BaseAddress = new Uri(WebAPIUrl + "Efile/SubmitEfileCreation");
                    HttpResponseMessage postJob = await client.PostAsJsonAsync<Efile.SAVEEfile>(WebAPIUrl + "Efile/SubmitEfileCreation", _model);
                    string url = postJob.Headers.Location.AbsoluteUri;
                    string mID = postJob.Headers.Location.Segments[4].ToString();
                    bool postResult = postJob.IsSuccessStatusCode;
                    // bool postResult = false;
                    if (postResult == true)
                    {
                        TempData["EfileID"] = mID;
                        TempData.Keep("EfileID");

                        _model.Msg = "Approved Successfully";
                        return Json("Successfully Approved", JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        _model.Msg = "UnSuccess";
                        return Json("UnSuccess", JsonRequestBehavior.AllowGet);
                    }
                }
            }
            else { return Json("Please upload file", JsonRequestBehavior.AllowGet); }

        }

        [HttpGet]
        public async Task<JsonResult> EfileDetails()
        {
            using (var client = new HttpClient())
            {
                string IDS = "";
                List<Efile.EfileDetails> listData = new List<Efile.EfileDetails>();
                //if (TempData["EfileID"] != null)
                //{
                IDS = (string)TempData.Peek("EfileID");
                int ID = Convert.ToInt32(IDS);
                client.BaseAddress = new Uri(WebAPIUrl);
                //HTTP GET
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client.GetAsync("Efile/GetAllEfileDetails?ID=" + 1 + "").Result;
                if (response.IsSuccessStatusCode)
                {
                    listData = response.Content.ReadAsAsync<List<Efile.EfileDetails>>().Result;
                }
                //}
                return Json(listData, JsonRequestBehavior.AllowGet);
            }
        }
       
        [HttpGet]
        public async Task<JsonResult> ExistsEfileDetailsByUserID()
        {
            using (var client = new HttpClient())
            {
                string IDS = "";
                List<Efile.EfileDetails> listData = new List<Efile.EfileDetails>();
                if (TempData["EfileID"] == null)
                {
                    int ID = Convert.ToInt32(Session["UserID"]);
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = client.GetAsync("Efile/ExistsEfileDetailsByUserID?ID=" + ID + "").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        listData = response.Content.ReadAsAsync<List<Efile.EfileDetails>>().Result;
                    }
                }
                return Json(listData, JsonRequestBehavior.AllowGet);
            }
        }
        public async Task<JsonResult> RoutingMember(Int32 EfileRouteID, Int32 EfileID, Int32 Efile_sequence, String Flag)
        {
            try
            {
                if (!string.IsNullOrEmpty(Session["UserID"].ToString()))
                {
                    using (var client = new HttpClient())
                    {
                        Efile.Efiles_routing _model = new Efile.Efiles_routing();
                        _model.EfileRouteID = EfileRouteID;
                        _model.EfileID = EfileID;
                        _model.Efile_sequence = Efile_sequence;
                        _model.Flag = Flag;
                        _model.Modifiedby = Convert.ToInt32(Session["UserID"]);
                        client.BaseAddress = new Uri(WebAPIUrl + "Efile/ChangeRouteSequence");
                        HttpResponseMessage postJob = await client.PostAsJsonAsync<Efile.Efiles_routing>(WebAPIUrl + "Efile/ChangeRouteSequence", _model);
                        string url = postJob.Headers.Location.AbsoluteUri;
                        //string mID = postJob.Headers.Location.Segments[4].ToString();
                        bool postResult = postJob.IsSuccessStatusCode;
                        // bool postResult = false;
                        if (postResult == true)
                        {
                            return Json("Route Successfully", JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            return Json("UnSuccess", JsonRequestBehavior.AllowGet);
                        }
                    }
                }
                else { return Json("/account/login"); }
            }
            catch (Exception) { return Json(false, JsonRequestBehavior.AllowGet); }
        }

        [HttpPost]
        public async Task<JsonResult> ExistsEfileDetailsBySquence(Efile.EfileDetails _model)
        {
            using (var client = new HttpClient())
            {
                List<Efile.EfileDetails> listData = new List<Efile.EfileDetails>();
                if (TempData["EfileID"] == null)
                {
                    _model.Efile_sequence = _model.Efile_sequence;
                    _model.userID = Convert.ToInt32(Session["UserID"]);
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    HttpResponseMessage response = await client.PostAsJsonAsync<Efile.EfileDetails>(WebAPIUrl + "Efile/ExistsEfileDetailsBySquence", _model);
                    if (response.IsSuccessStatusCode)
                    {
                        listData = response.Content.ReadAsAsync<List<Efile.EfileDetails>>().Result;
                    }
                }
                return Json(listData, JsonRequestBehavior.AllowGet);
            }
        }

        
        [HttpPost]
        public async Task<JsonResult> EfileshowAttachment(Efile.Notefile _model)
        {
            using (var client = new HttpClient())
            {
                List<Efile.Notefile> listData = new List<Efile.Notefile>();
                client.BaseAddress = new Uri(WebAPIUrl);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = await client.PostAsJsonAsync<Efile.Notefile>(WebAPIUrl + "Efile/EfileshowAttachment", _model);
                if (response.IsSuccessStatusCode)
                {
                    listData = response.Content.ReadAsAsync<List<Efile.Notefile>>().Result;
                }
                return Json(listData, JsonRequestBehavior.AllowGet);
            }
        }
    }
}