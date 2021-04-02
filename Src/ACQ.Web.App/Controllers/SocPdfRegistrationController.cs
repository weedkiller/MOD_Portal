﻿using ACQ.Web.ViewModel.AONW;
using ACQ.Web.ViewModel.EFile;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Path = System.IO.Path;
using ACQ.Web.ExternalServices.SecurityAudit;
using System.Net.Http.Headers;
using Microsoft.Graph;
using static ACQ.Web.App.MvcApplication;
using Ganss.XSS;
using GemBox.Document;
using System.Linq;

namespace ACQ.Web.App.Controllers
{
    public class SocPdfRegistrationController : Controller
    {
        // GET: SocPdfRegistration
        HtmlSanitizer sanitizer = new HtmlSanitizer();
        SAVESOCVIEWMODELBluk obj = new SAVESOCVIEWMODELBluk();
        public decimal filesize { get; set; }
        private static string UploadPath = ConfigurationManager.AppSettings["SOCImagePath"].ToString();
        private static string UploadfilePath = ConfigurationManager.AppSettings["SOCPath"].ToString();
        private static string WebAPIUrl = ConfigurationManager.AppSettings["APIUrl"].ToString();

        List<Efile.FileDetail> fileDetails = new List<Efile.FileDetail>();
        List<Efile.FileDetail> fileDetailsA = new List<Efile.FileDetail>();
        private string InitVector = @"qwertyui";
        private string baseUrl = ConfigurationManager.AppSettings["baseUrl"].ToString();

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

        [Route("SoCRegistration")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult SoCPdfRegistration()
        {

            return View();
        }


        public class ReturnData
        {
            public bool success { get; set; }
            public string url { get; set; }
            public string name { get; set; }
            public string error { get; set; }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("SoCRegistration")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public async Task<ActionResult> SoCPdfRegistration(HttpPostedFileBase file, string SoC_Type)
        {
            
            dynamic expando = new ExpandoObject();
            var marksModel = expando as IDictionary<string, object>;
            string filepath = "";
            string fullpath = "";
            string mSercive = "";
            if (Request.Files.Count > 0)
            {

                System.Web.HttpPostedFileBase fileSoC = Request.Files[0];


                try
                {
                    if (fileSoC.ContentLength > 0)
                    {
                        AttachmentViewModel objattach = new AttachmentViewModel();
                        ViewBag.SoC = fileSoC.FileName;
                        string filename = DateTime.Now.ToString("ddmmhhss") + fileSoC.FileName;

                        if (!FileCheckformat(fileSoC, ".doc"))
                        {
                            ViewBag.UploadStatusmsg = "Please upload only .doc or .docx file and File size Should Be UpTo 1 MB";
                            ViewBag.UploadStatus = "errormsg";
                            return View();
                        }

                        //filepath = UploadfilePath + filename;
                        var FileDataContent = Request.Files[0];
                        var stream = FileDataContent.InputStream;
                        var fileName = Path.GetFileName(FileDataContent.FileName);
                        var UploadPath = Server.MapPath("~/excelfolder/");
                        Directory.CreateDirectory(UploadPath);
                        string path = Path.Combine(UploadPath, fileName);
                        try
                        {
                            if (System.IO.File.Exists(path))
                                System.IO.File.Delete(path);
                            using (var fileStream = System.IO.File.Create(path))
                            {
                                stream.CopyTo(fileStream);
                            }

                        }
                        catch (IOException ex)
                        {
                            // handle  
                        }

                        using (var client = new HttpClient())
                        {

                            Application word1 = new Application();
                            object readOnly = false;
                            object isVisible = true;
                            object missing = System.Reflection.Missing.Value;
                            Document doc = word1.Documents.Open(path, ref missing, ref readOnly, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref missing, ref isVisible);


                            List<string> contentControlText = new List<string>();

                            foreach (ContentControl CC in doc.ContentControls)
                            {
                                contentControlText.Add(CC.Range.Text);

                            }

                            if (Session["Department"].ToString() == "IDS")
                            {
                                mSercive = "Joint Staff";
                            }
                            else
                            {
                                mSercive = Session["Department"].ToString();
                            }


                            if (contentControlText.Count > 0)
                            {
                                if (contentControlText[0].ToString() == "Click here to enter text.")
                                {
                                    ViewBag.UploadStatus = "NAME OF PROPOSAL";
                                    return View();
                                }

                                if (contentControlText[1].ToString() == "Choose an item.")
                                {
                                    ViewBag.UploadStatus = "SERVICE";
                                    return View();
                                }

                                if (contentControlText[65].ToString() == "Choose an item.")
                                {
                                    ViewBag.UploadStatus = "CATEGORISATION STATUS";
                                    return View();
                                }

                                if (contentControlText[4].ToString() == "Click here to enter text.")
                                {
                                    ViewBag.UploadStatus = "Unique Reference no";
                                    return View();
                                }


                                if (contentControlText[70].ToString() == "Click here to enter text.")
                                {
                                    ViewBag.UploadStatus = "Quantity";
                                    return View();
                                }

                                if (contentControlText[73].ToString() == "Click here to enter text.")
                                {
                                    ViewBag.UploadStatus = "Estimated cost";
                                    return View();
                                }

                                if (Session["SectionID"].ToString() == "1" || Session["SectionID"].ToString() == "12" || Session["SectionID"].ToString() == "13")
                                {

                                }
                                else
                                {
                                    if (contentControlText[1].ToString() == mSercive)
                                    {

                                    }
                                    else
                                    {
                                        ViewBag.UploadStatus = "Type";
                                        return View();
                                    }
                                }

                                obj.aon_id = "0";
                                obj.item_description = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[0].ToString().Trim()));
                                obj.Service_Lead_Service = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[1].ToString().Trim()));
                                obj.DPP_DAP = null;
                                obj.SystemCase = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[3].ToString()));
                                obj.SoCCase = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[4].ToString()));
                                obj.Categorisation = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[67].ToString()));
                                obj.IC_percentage = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[69].ToString()));
                                obj.Quantity = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[70].ToString()));
                                obj.Cost = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[73].ToString()));
                                obj.Essential_parameters = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[75].ToString()));
                                obj.EPP = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[77].ToString()));
                                obj.Trials_Required = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[81].ToString()));
                                obj.Offset_applicable = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[83].ToString()));
                                obj.Option_clause_applicable = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[85].ToString()));
                                obj.Warrenty_applicable = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[87].ToString()));
                                obj.Warrenty_Remarks = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[88].ToString()));
                                obj.Any_other_aspect = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[90].ToString()));
                                obj.SocAName = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[91].ToString()));
                                obj.SocADesignation = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[92].ToString()));
                                obj.SocAApprovalRef = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[93].ToString()));
                                obj.SocAApprovalDate = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[94].ToString()));
                                obj.SocSDName = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[95].ToString()));
                                obj.SocSDDesignation = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[96].ToString()));
                                obj.SocSDPhone = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[97].ToString()));
                                obj.SocSDDate = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[98].ToString()));
                                obj.SoDate = DateTime.Now;
                                obj.Tax_Duties = "Inclusive Tax & Duties";
                                obj.Type_of_Acquisition = "";
                                obj.AMC_applicable = "";
                                obj.AMCRemarks = "";
                                obj.Remarks = "";
                                obj.SoCType = "";
                                obj.AoN_Accorded_By = Encryption.Encrypt(sanitizer.Sanitize(contentControlText[2].ToString()));
                                obj.AoN_validity = 6;
                                obj.AoN_validity_unit = "Month";
                                obj.CreatedBy = Convert.ToInt16(Session["UserID"].ToString());
                                obj.CreatedOn = DateTime.Now;
                                obj.DeletedBy = 1;
                                obj.DeletedOn = null;
                                obj.IsDeleted = false;
                                obj.SoC_Type = SoC_Type;

                                ViewBag.UploadStatus = "";
                                fileSoC.SaveAs(Path.Combine(Server.MapPath(UploadfilePath), filename));
                                Uploadencryption(fileSoC);
                                fullpath = Server.MapPath(UploadfilePath) + filename;

                                objattach.AttachmentFileName = "SOC";
                                objattach.Path = filename;
                                objattach.RefId = 2;
                            }
                            else
                            {


                                ViewBag.UploadStatus = "File format not correct";
                                return View();
                            }

                                    //closing document object   
                                    ((_Document)doc).Close();

                            //Quit application object to end process  
                            ((_Application)word1).Quit();

                        }



                        SAVESOCVIEWMODEL model = new SAVESOCVIEWMODEL();

                        using (HttpClient client1 = new HttpClient())
                        {
                            client1.BaseAddress = new Uri(WebAPIUrl + "AONW/AONWCreateBulk");
                            HttpResponseMessage postJob = await client1.PostAsJsonAsync<SAVESOCVIEWMODELBluk>(WebAPIUrl + "AONW/AONWCreateBulk", obj);
                            string url = postJob.Headers.Location.AbsoluteUri;
                            string mID = postJob.Headers.Location.Segments[4].ToString();
                            bool postResult = postJob.IsSuccessStatusCode;
                            if (postResult == true)
                            {

                                objattach.aon_id = Convert.ToInt32(mID);

                                if (postResult == true)
                                {
                                    System.Web.HttpPostedFileBase fileSoCPPT = Request.Files[1];

                                    if (fileSoCPPT.ContentLength > 0)
                                    {
                                        ViewBag.SoC = fileSoCPPT.FileName;
                                        filename = objattach.aon_id.ToString() + "_" + "SOCPPT" + "_" + fileSoCPPT.FileName.ToString();
                                        if (!FileCheckformat(fileSoCPPT, ".ppt"))
                                        {
                                            ViewBag.UploadStatusmsg = "Please upload only .ppt or .pptx file and File size Should Be UpTo 1 MB";
                                            ViewBag.UploadStatus = "errormsg";
                                            return View();
                                        }
                                        else
                                        {
                                            ViewBag.UploadStatus = "";
                                            filepath = UploadfilePath + filename;
                                            Uploadencryption(fileSoCPPT);
                                            fullpath = Server.MapPath(UploadfilePath) + filename;
                                        }
                                    }



                                    System.Web.HttpPostedFileBase fileChairman = Request.Files[2];
                                    if (fileChairman.ContentLength > 0)
                                    {
                                        if (!FileCheckformat(fileChairman, ".ppt"))
                                        {
                                            ViewBag.UploadStatusmsg = "Please upload only .pdf file and File size Should Be UpTo 1 MB";
                                            ViewBag.UploadStatus = "errormsg";
                                            return View();
                                        }
                                        else
                                        {
                                            ViewBag.UploadStatus = "";
                                            filepath = UploadfilePath + filename;
                                            Uploadencryption(fileChairman);
                                            fullpath = Server.MapPath(UploadfilePath) + filename;
                                        }

                                    }

                                    System.Web.HttpPostedFileBase fileDraft = Request.Files[3];
                                    if (fileDraft.ContentLength > 0)
                                    {
                                        ViewBag.Draft = fileDraft.FileName;
                                        filename = objattach.aon_id.ToString() + "_" + "SOCDraft" + "_" + fileDraft.FileName;
                                        if (!FileCheckformat(fileDraft, ".pdf"))
                                        {
                                            ViewBag.UploadStatusmsg = "Please upload only .pdf file and File size Should Be UpTo 1 MB";
                                            ViewBag.UploadStatus = "errormsg";
                                            return View();
                                        }
                                        else
                                        {
                                            ViewBag.UploadStatus = "";
                                            filepath = UploadfilePath + filename;
                                            Uploadencryption(fileDraft);
                                            fullpath = Server.MapPath(UploadfilePath) + filename;
                                        }

                                    }

                                    System.Web.HttpPostedFileBase fileOtherDoucment = Request.Files[4];
                                    if (fileOtherDoucment.ContentLength > 0)
                                    {
                                        ViewBag.SoC = fileOtherDoucment.FileName;
                                        filename = objattach.aon_id.ToString() + "_" + "SOCOther" + "_" + fileOtherDoucment.FileName;
                                        if (!FileCheckformat(fileOtherDoucment, ".pdf"))
                                        {
                                            ViewBag.UploadStatusmsg = "Please upload only .pdf file and File size Should Be UpTo 1 MB";
                                            ViewBag.UploadStatus = "errormsg";
                                            return View();
                                        }
                                        else
                                        {
                                            ViewBag.UploadStatus = "";
                                            filepath = UploadfilePath + filename;
                                            Uploadencryption(fileOtherDoucment);
                                            fullpath = Server.MapPath(UploadfilePath) + filename;
                                        }


                                    }

                                    if (TempData["FileAA"] != null)
                                    {
                                        fileDetailsA = TempData["FileAA"] as List<Efile.FileDetail>;
                                    }
                                    string path1 = Server.MapPath(UploadfilePath);
                                    HttpFileCollectionBase filesA = Request.Files;
                                    string mFilename = "";
                                    for (int i = 0; i < filesA.Count; i++)
                                    {

                                        HttpPostedFileBase fileA = filesA[i];

                                        if (i == 0)
                                        {
                                            mFilename = "SOC";
                                        }
                                        else if (i == 1)
                                        {
                                            mFilename = "SoC PPT";
                                        }
                                        else if (i == 2)
                                        {
                                            mFilename = "Chairman's brief";
                                        }
                                        else if (i == 3)
                                        {
                                            mFilename = "Draft RFP";
                                        }
                                        else if (i == 5)
                                        {
                                            mFilename = "Any Other Document";
                                        }

                                        if (i != 4)
                                        {
                                            string str = EncryptFile(fileA, "ugs@4321");
                                            if (str != "")
                                            {
                                                Efile.FileDetail fileDetailA = new Efile.FileDetail()
                                                {
                                                    FileName = mFilename,
                                                    FilePath = str,
                                                    Id = 1,
                                                };

                                                fileDetailsA.Add(fileDetailA);
                                            }
                                        }
                                    }


                                    TempData["FileAA"] = fileDetailsA;



                                }

                                return RedirectToActionPermanent("ViewSocMaster", "AONW", new { ID = Encryption.Encrypt(objattach.aon_id.ToString()) });
                            }
                            else
                            {
                                ViewBag.NotSave = "SOC registration not saved";
                            }
                        }
                    }
                    else
                    {
                        ViewBag.UploadStatus = "Only .doc or .docx file";
                        return View();
                    }

                }


                catch (Exception ex)
                {

                    ViewBag.UploadStatusmsg = "Access is denied to read soc file and format";
                    ViewBag.UploadStatus = "errormsg";
                    //ViewBag.msg = ex.Message;
                    return View();
                }
            }
            else
            {
                ViewBag.UploadStatus = "Can not be empty";

            }

            return View();

        }



        [HttpPost]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult UploadFiles(string id)
        {
            if (TempData["FileA"] != null)
            {
                fileDetails = TempData["FileA"] as List<Efile.FileDetail>;
            }
            string path = Server.MapPath(UploadfilePath);
            HttpFileCollectionBase files = Request.Files;


            for (int i = 0; i < files.Count; i++)
            {

                HttpPostedFileBase file = files[i];
                if (!FileCheckformat(file, ".pdf"))
                {
                    ViewBag.UploadStatusmsg = "Please upload only .pdf file and File size Should Be UpTo 1 MB";
                    ViewBag.UploadStatus = "errormsg";
                    return View();
                }

                string str = EncryptFile(file, "ugs@4321");
                if (str != "")
                {
                    file.SaveAs(path + file.FileName);
                    Efile.FileDetail fileDetail = new Efile.FileDetail()
                    {
                        FileName = sanitizer.Sanitize(id),
                        FilePath = str,
                        Id = 1,
                    };
                    fileDetails.Add(fileDetail);
                }
            }


            TempData["FileA"] = fileDetails;
            return Json(files.Count + " Files Uploaded!");
        }

        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public string EncryptFile(HttpPostedFileBase file, string password)
        {

            ReturnData result = new ReturnData();

            result.success = false;
            try
            {
                if (Request.Files.Count > 0)
                {

                    string filename = Path.GetFileName(file.FileName);
                    string outputFile = Path.Combine(baseUrl, filename);


                    UnicodeEncoding UE = new UnicodeEncoding();
                    byte[] key = UE.GetBytes(password);
                    byte[] IV = UE.GetBytes(InitVector);
                    //byte[] IV2 = ASCIIEncoding.UTF8.GetBytes(initVector);
                    FileStream fsCrypt = new FileStream(outputFile, FileMode.Create);
                    RijndaelManaged RMCrypto = new RijndaelManaged();
                    ICryptoTransform encryptor = RMCrypto.CreateEncryptor(key, IV);
                    CryptoStream cs = new CryptoStream(fsCrypt, encryptor, CryptoStreamMode.Write);

                    int data;
                    while ((data = file.InputStream.ReadByte()) != -1)
                    {
                        cs.WriteByte((byte)data);

                    }

                    file.InputStream.Close();
                    cs.Close();
                    fsCrypt.Close();

                    result.success = true;
                    result.url = filename;
                    result.name = filename;
                }

            }
            catch (Exception e)
            {

                result.success = false;
                result.url = string.Empty;
                result.name = string.Empty;
                result.error = e.Message;
            }

            return result.url;
        }

        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public string DecryptFile(string filename, string filePath, string password)
        {
            ReturnData result = new ReturnData();
            try
            {
                string outputFile = Path.Combine(baseUrl, "decry_" + filename);
                UnicodeEncoding UE = new UnicodeEncoding();
                byte[] key = UE.GetBytes(password);
                byte[] IV = UE.GetBytes(InitVector);
                FileStream fs = new FileStream(filePath, FileMode.Open);
                RijndaelManaged RMCrypto = new RijndaelManaged();
                ICryptoTransform decryptor = RMCrypto.CreateDecryptor(key, IV);
                CryptoStream cs = new CryptoStream(fs, decryptor, CryptoStreamMode.Read);
                FileStream fsOut = new FileStream(outputFile, FileMode.Create);

                int data;
                while ((data = cs.ReadByte()) != -1)
                {
                    fsOut.WriteByte((byte)data);
                }

                fs.Close();
                fsOut.Close();
                cs.Close();

                result.name = "decry_" + filename;
                result.url = "decry_" + filename;
                result.success = true;

            }

            catch (Exception e)
            {

            }

            return result.url;
        }
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public void Uploadencryption(HttpPostedFileBase files)
        {
            // Add code to upload file with encryption

            byte[] file = new byte[files.ContentLength];
            files.InputStream.Read(file, 0, files.ContentLength);

            string fileName = files.FileName;

            // key for encryption
            byte[] Key = Encoding.UTF8.GetBytes("asdf!@#$1234ASDF");
            try
            {
                string outputFile = Path.Combine(Server.MapPath(UploadfilePath), fileName);
                if (System.IO.File.Exists(outputFile))
                {
                    // Show Already exist Message 
                }
                else
                {
                    FileStream fs = new FileStream(outputFile, FileMode.Create);
                    RijndaelManaged rmCryp = new RijndaelManaged();
                    CryptoStream cs = new CryptoStream(fs, rmCryp.CreateEncryptor(Key, Key), CryptoStreamMode.Write);
                    foreach (var data in file)
                    {
                        cs.WriteByte((byte)data);
                    }
                    cs.Close();
                    fs.Close();
                }


            }
            catch
            {
                Response.Write("Encryption Failed! Please try again.");
            }
        }
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ViewfileandDoc()
        {

            string mFilename = Request.QueryString["path"].ToString();
            string filePath = baseUrl + mFilename;

            string iframPath = DecryptFile(mFilename, filePath, "ugs@4321");
            ViewBag.path = @"\excelfolder\" + iframPath;
            return View();
        }
    }
}