using ACQ.Web.App.ViewModel;
using ACQ.Web.ExternalServices.SecurityAudit;
using ACQ.Web.ViewModel.AONW;
using ACQ.Web.ViewModel.EFile;
using ACQ.Web.ViewModel.User;
using Ganss.XSS;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static ACQ.Web.App.MvcApplication;

namespace ACQ.Web.App.Controllers
{
    public class ContractController : Controller
    {

        HtmlSanitizer sanitizer = new HtmlSanitizer();

        public decimal filesize { get; set; }
        private static string UploadPath = ConfigurationManager.AppSettings["SOCImagePath"].ToString();
        private static string UploadfilePath = ConfigurationManager.AppSettings["SOCPath"].ToString();
        private static string WebAPIUrl = ConfigurationManager.AppSettings["APIUrl"].ToString();
        List<Efile.FileDetail> fileDetails = new List<Efile.FileDetail>();
        List<Efile.FileDetail> fileDetailsA = new List<Efile.FileDetail>();
        List<AttachmentViewModel> fileDetailsF = new List<AttachmentViewModel>();

        public ContractController()
        {
            if (BruteForceAttackss.bcontroller != "")
            {
                if (BruteForceAttackss.bcontroller == "Contract")
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
                    BruteForceAttackss.bcontroller = "Contract";
                }
            }
            else
            {
                BruteForceAttackss.bcontroller = "Contract";
            }
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


                if (file.ContentType != "application/vnd.ms-excel" && file.ContentType != "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                {
                    return false;
                }
                else
                {

                    if (FileExtension == ".xls" || FileExtension == ".xlsx")
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
        [Route("Contract")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult Contract()
        {
            if (TempData["File"] != null)
            {
                ViewBag.File = TempData["File"].ToString();
            }
            if (TempData["FileStage"] != null)
            {
                ViewBag.FileStage = TempData["FileStage"].ToString();
            }
            if (TempData["FieldName"] != null)
            {
                ViewBag.FieldName = TempData["FieldName"].ToString();
            }
            if (TempData["ExcelColoumn"] != null)
            {
                ViewBag.ExcelColoumn = TempData["ExcelColoumn"].ToString();
            }
            if (TempData["Uploadsuccess"] != null)
            {
                bool upload = (bool)TempData["Uploadsuccess"];
                if (upload)
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
                if (cnt.Contrct_Detail.Descriptions != null)
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
                    
                    stages.Add(stageDetail);
                }

                _contracts.Stage_Detail = stages;

                List<ContractStagePayment> stagesPayList = new List<ContractStagePayment>();
                foreach (var pay in cnt.Stage_Payment_Detail.ToList())
                {
                    ContractStagePayment stagepay = new ContractStagePayment();
                    stagepay.Id = Convert.ToInt32(sanitizer.Sanitize(pay.Id.ToString()));
                    if (pay.StageNumber != null)
                    {
                        stagepay.StageNumber = Convert.ToInt32(sanitizer.Sanitize(pay.StageNumber.ToString()));
                    }
                    if (pay.RevisedDateOfpayment != null)
                    {
                        stagepay.RevisedDateOfpayment = Convert.ToDateTime(sanitizer.Sanitize(pay.RevisedDateOfpayment.ToString()));
                    }
                    if (pay.ReasonsForSlippage != null)
                    {
                        stagepay.ReasonsForSlippage = sanitizer.Sanitize(pay.ReasonsForSlippage);
                    }
                    if (pay.ActualDateOfPayment != null)
                    {
                        stagepay.ActualDateOfPayment = Convert.ToDateTime(sanitizer.Sanitize(pay.ActualDateOfPayment.ToString()));
                    }

                    if (pay.TotalPaymentMade > 0)
                    {
                        stagepay.TotalPaymentMade = Convert.ToDecimal(sanitizer.Sanitize(pay.TotalPaymentMade.ToString()));
                    }
                    if (pay.FullorPartPaymentMade != null)
                    {
                        stagepay.FullorPartPaymentMade = sanitizer.Sanitize(pay.FullorPartPaymentMade);
                    }
                    if (pay.ExpendMadeTill31March != null)
                    {
                        stagepay.ExpendMadeTill31March = Convert.ToDecimal(sanitizer.Sanitize(pay.ExpendMadeTill31March.ToString()));
                    }
                    stagesPayList.Add(stagepay);
                }
                _contracts.Stage_Payment_Detail = stagesPayList;


                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    HttpResponseMessage postJob = await client.PostAsJsonAsync<Contracts>(WebAPIUrl + "Contract/SaveContract", _contracts);
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



        [Route("SaveContractMasterExcel")]
        [HttpPost]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveContractMasterExcel(ImportExcel excel)
        {
            string Massege = "";
            try
            {
                List<ContractDetails> Contracts = new List<ContractDetails>();
                string filePath = string.Empty;
                //if (ModelState.IsValid)
                //{
                if (excel.file.ContentLength > 0)
                {
                    string path = Server.MapPath("/ExcelFile/");
                    filePath = path + Path.GetFileName(excel.file.FileName);
                    string extension = Path.GetExtension(excel.file.FileName);
                    if (!FileCheckformat(excel.file, extension))
                    {
                        TempData["File"] = "Please upload only .xls or .xlsx file and File size Should Be UpTo 1 MB";
                        ModelState.AddModelError("file", "Please upload Only Excel File");
                        return RedirectToAction("Contract", "Contract");
                    }
                    //if (excel.file.ContentType == "application/vnd.ms-excel" || excel.file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    //{
                    //if (extension == ".xls" || extension == ".xlsx")
                    //{
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    excel.file.SaveAs(filePath);
                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".xls": //Excel 97-03.
                            conString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = { 0 }; Extended Properties = 'Excel 8.0;HDR=YES'";
                            break;
                        case ".xlsx": //Excel 07 and above.
                            conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                            break;
                    }

                    conString = string.Format(conString, filePath);

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                DataTable dt = new DataTable();
                                cmdExcel.Connection = connExcel;

                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dt);
                                connExcel.Close();

                                foreach (DataRow row in dt.Rows)
                                {
                                    ContractDetails stage = new ContractDetails();
                                    if (row["ContractId"] != DBNull.Value)
                                    {
                                        stage.ContractId = sanitizer.Sanitize(row["ContractId"].ToString());
                                    }

                                    if (row["Contract_Number"] != DBNull.Value)
                                    {
                                        stage.Contract_Number = sanitizer.Sanitize(row["Contract_Number"].ToString());
                                    }

                                    if (row["DateOfContractSigning"] != DBNull.Value)
                                    {
                                        stage.DateOfContractSigning = Convert.ToDateTime(sanitizer.Sanitize(row["DateOfContractSigning"].ToString()));
                                    }
                                    if (row["Descriptions"] != DBNull.Value)
                                    {
                                        stage.Descriptions = sanitizer.Sanitize(row["Descriptions"].ToString());
                                    }
                                    if (row["Category"] != DBNull.Value)
                                    {
                                        stage.Category = sanitizer.Sanitize(row["Category"].ToString());
                                    }
                                    if (row["EffectiveDate"] != DBNull.Value)
                                    {
                                        stage.EffectiveDate = Convert.ToDateTime(sanitizer.Sanitize(row["EffectiveDate"].ToString()));
                                    }
                                    if (row["ABGDate"] != DBNull.Value)
                                    {
                                        stage.ABGDate = Convert.ToDateTime(sanitizer.Sanitize(row["ABGDate"].ToString()));
                                    }
                                    if (row["PWBGPercentage"] != DBNull.Value)
                                    {
                                        stage.PWBGPercentage = Convert.ToInt32(sanitizer.Sanitize(row["PWBGPercentage"].ToString()));
                                    }
                                    if (row["PWBGDate"] != DBNull.Value)
                                    {
                                        stage.PWBGDate = Convert.ToDateTime(sanitizer.Sanitize(row["PWBGDate"].ToString()));
                                    }
                                    if (row["Incoterms"] != DBNull.Value)
                                    {
                                        stage.Incoterms = sanitizer.Sanitize(row["Incoterms"].ToString());
                                    }
                                    if (row["Warranty"] != DBNull.Value)
                                    {
                                        stage.Warranty = sanitizer.Sanitize(row["Warranty"].ToString());
                                    }
                                    if (row["ContractValue"] != DBNull.Value)
                                    {
                                        stage.ContractValue = Convert.ToDecimal(sanitizer.Sanitize(row["ContractValue"].ToString()));
                                    }
                                    if (row["FEContent"] != DBNull.Value)
                                    {
                                        stage.FEContent = sanitizer.Sanitize(row["FEContent"].ToString());
                                    }
                                    if (row["TaxesAndDuties"] != DBNull.Value)
                                    {
                                        stage.TaxesAndDuties = sanitizer.Sanitize(row["TaxesAndDuties"].ToString());
                                    }
                                    if (row["FinalDeliveryDate"] != DBNull.Value)
                                    {
                                        stage.FinalDeliveryDate = Convert.ToDateTime(sanitizer.Sanitize(row["FinalDeliveryDate"].ToString()));
                                    }
                                    if (row["GracePeriod"] != DBNull.Value)
                                    {
                                        stage.GracePeriod = sanitizer.Sanitize(row["GracePeriod"].ToString());
                                    }
                                    if (row["Services"] != DBNull.Value)
                                    {
                                        stage.Services = sanitizer.Sanitize(row["Services"].ToString());
                                    }
                                    Contracts.Add(stage);


                                }
                            }
                        }
                    }

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(WebAPIUrl);
                        HttpResponseMessage postJob = await client.PostAsJsonAsync<List<ContractDetails>>(WebAPIUrl + "Contract/SaveContractExcel", Contracts);
                        bool postResult = postJob.IsSuccessStatusCode;
                        if (postResult == true)
                        {

                            Massege = "Success";
                            TempData["Uploadsuccess"] = true;
                        }
                        else
                        {
                            TempData["Uploadsuccess"] = false;
                            Massege = "Failed";
                        }
                    }
                    //}
                    //}

                }
                //}
                //else
                //{
                //    TempData["File"] = "Upload Only Excel.";
                //    ModelState.AddModelError("", "Please upload Only Excel File");
                //    return RedirectToAction("Contract", "AONW");

                //}
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                string val = ex.Message.ToString();
                Match match = Regex.Match(val, @"'([^']*)");
                if (match.Success)
                {
                    string col = match.Groups[1].Value;
                    TempData["FieldName"] = "Please check excel coloumn name. It should be" + " " + col + ".";
                }
                ModelState.AddModelError("file", "Please upload Only Excel File");
                return RedirectToAction("Contract", "Contract");
            }
            return RedirectToAction("Contract", "Contract");
        }

        [Route("SaveStageExcel")]
        [HttpPost]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveStageExcel(ImportExcel excel)
        {
            string Massege = "";
            try
            {
                List<StageDetail> Contracts = new List<StageDetail>();
                string filePath = string.Empty;

                //if (ModelState.IsValid)
                //{
                if (excel.file != null)
                {
                    string path = Server.MapPath("/ExcelFile/");
                    filePath = path + Path.GetFileName(excel.file.FileName);
                    string extension = Path.GetExtension(excel.file.FileName);
                    if (!FileCheckformat(excel.file, extension))
                    {
                        TempData["FileStage"] = "Please upload only .xls or .xlsx file and File size Should Be UpTo 1 MB";
                        ModelState.AddModelError("file", "Please upload Only Excel File");
                        return RedirectToAction("Contract", "Contract");
                    }
                    //if (excel.file.ContentType == "application/vnd.ms-excel" || excel.file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    //{
                    //if (extension == ".xls" || extension == ".xlsx")
                    //{
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    excel.file.SaveAs(filePath);

                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".xls": //Excel 97-03.
                            conString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = { 0 }; Extended Properties = 'Excel 8.0;HDR=YES'";
                            break;
                        case ".xlsx": //Excel 07 and above.
                                      //conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                            break;
                    }

                    conString = string.Format(conString, filePath);

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                DataTable dt = new DataTable();
                                cmdExcel.Connection = connExcel;

                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dt);
                                connExcel.Close();

                                foreach (DataRow row in dt.Rows)
                                {
                                    StageDetail stage = new StageDetail();
                                    //if (row["ContractmasterId"] != DBNull.Value)
                                    //{
                                    //    stage.ContractmasterId = Convert.ToInt32(sanitizer.Sanitize(row["ContractmasterId"].ToString()));
                                    //}
                                    if (row["ContractId"] != DBNull.Value)
                                    {
                                        stage.ContractId = sanitizer.Sanitize(row["ContractId"].ToString());
                                    }
                                    if (row["StageNumber"] != DBNull.Value)
                                    {
                                        stage.StageNumber = Convert.ToInt32(sanitizer.Sanitize(row["StageNumber"].ToString()));
                                    }
                                    if (row["stageDescription"] != DBNull.Value)
                                    {
                                        stage.stageDescription = sanitizer.Sanitize(row["stageDescription"].ToString());
                                    }
                                    if (row["StageStartdate"] != DBNull.Value)
                                    {
                                        stage.StageStartdate = Convert.ToDateTime(sanitizer.Sanitize(row["StageStartdate"].ToString()));
                                    }
                                    if (row["StageCompletionDate"] != DBNull.Value)
                                    {
                                        stage.StageCompletionDate = Convert.ToDateTime(sanitizer.Sanitize(row["StageCompletionDate"].ToString()));
                                    }
                                    if (row["PercentOfContractValue"] != DBNull.Value)
                                    {
                                        stage.PercentOfContractValue = Convert.ToInt32(sanitizer.Sanitize(row["PercentOfContractValue"].ToString()));
                                    }
                                    if (row["Amount"] != DBNull.Value)
                                    {
                                        stage.Amount = Convert.ToDecimal(sanitizer.Sanitize(row["Amount"].ToString()));
                                    }
                                    if (row["DueDateOfPayment"] != DBNull.Value)
                                    {
                                        stage.DueDateOfPayment = Convert.ToDateTime(sanitizer.Sanitize(row["DueDateOfPayment"].ToString()));
                                    }
                                    if (row["Conditions"] != DBNull.Value)
                                    {
                                        stage.Conditions = sanitizer.Sanitize(row["Conditions"].ToString());
                                    }
                                    //if (row["RevisedDateOfpayment"] != DBNull.Value)
                                    //{
                                    //    stage.RevisedDateOfpayment = Convert.ToDateTime(sanitizer.Sanitize(row["RevisedDateOfpayment"].ToString()));
                                    //}
                                    //if (row["ReasonsForSlippage"] != DBNull.Value)
                                    //{
                                    //    stage.ReasonsForSlippage = sanitizer.Sanitize(row["ReasonsForSlippage"].ToString());
                                    //}
                                    //if (row["ActualDateOfPayment"] != DBNull.Value)
                                    //{
                                    //    stage.ActualDateOfPayment = Convert.ToDateTime(sanitizer.Sanitize(row["ActualDateOfPayment"].ToString()));
                                    //}
                                    //if (row["TotalPaymentMade"] != DBNull.Value)
                                    //{
                                    //    stage.TotalPaymentMade = Convert.ToDecimal(sanitizer.Sanitize(row["TotalPaymentMade"].ToString()));
                                    //}
                                    //if (row["FullorPartPaymentMade"] != DBNull.Value)
                                    //{
                                    //    stage.FullorPartPaymentMade = sanitizer.Sanitize(row["FullorPartPaymentMade"].ToString());
                                    //}
                                    //if (row["ExpendMadeTill31March"] != DBNull.Value)
                                    //{
                                    //    stage.ExpendMadeTill31March = Convert.ToDecimal(sanitizer.Sanitize(row["ExpendMadeTill31March"].ToString()));
                                    //}

                                    Contracts.Add(stage);



                                }
                            }
                        }
                    }

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(WebAPIUrl);
                        HttpResponseMessage postJob = await client.PostAsJsonAsync<List<StageDetail>>(WebAPIUrl + "Contract/SaveStageExcel", Contracts);
                        bool postResult = postJob.IsSuccessStatusCode;
                        if (postResult == true)
                        {

                            Massege = "Success";
                            TempData["Uploadsuccess"] = true;
                        }
                        else
                        {
                            TempData["Uploadsuccess"] = false;
                            Massege = "Failed";
                        }
                    }
                    //}
                    //}


                }
                //}
                //else
                //{
                //    TempData["FileStage"] = "Upload Only Excel.";
                //    ModelState.AddModelError("", "Please upload Only Excel File");
                //    return RedirectToAction("Contract", "AONW");
                //}
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                string val = ex.Message.ToString();
                Match match = Regex.Match(val, @"'([^']*)");
                if (match.Success)
                {
                    string col = match.Groups[1].Value;
                    TempData["ExcelColoumn"] = "Please check excel coloumn name. It should be" + " " + col + ".";
                }
                ModelState.AddModelError("", "Please upload Only Excel File");
                return RedirectToAction("Contract", "Contract");
            }
            return RedirectToAction("Contract", "Contract");
        }

        [Route("SaveStagePaymentExcel")]
        [HttpPost]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveStagePaymentExcel(ImportExcel excel)
        {
            string Massege = "";
            try
            {
                List<ContractStagePayment> Contracts = new List<ContractStagePayment>();
                string filePath = string.Empty;

                //if (ModelState.IsValid)
                //{
                if (excel.file != null)
                {
                    string path = Server.MapPath("/ExcelFile/");
                    filePath = path + Path.GetFileName(excel.file.FileName);
                    string extension = Path.GetExtension(excel.file.FileName);
                    if (!FileCheckformat(excel.file, extension))
                    {
                        TempData["FileStagePayment"] = "Please upload only .xls or .xlsx file and File size Should Be UpTo 1 MB";
                        ModelState.AddModelError("file", "Please upload Only Excel File");
                        return RedirectToAction("Contract", "Contract");
                    }
                    //if (excel.file.ContentType == "application/vnd.ms-excel" || excel.file.ContentType == "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet")
                    //{
                    //if (extension == ".xls" || extension == ".xlsx")
                    //{
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    excel.file.SaveAs(filePath);

                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".xls": //Excel 97-03.
                            conString = "Provider = Microsoft.Jet.OLEDB.4.0; Data Source = { 0 }; Extended Properties = 'Excel 8.0;HDR=YES'";
                            break;
                        case ".xlsx": //Excel 07 and above.
                                      //conString = ConfigurationManager.ConnectionStrings["Excel07ConString"].ConnectionString;
                            conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES'";
                            break;
                    }

                    conString = string.Format(conString, filePath);

                    using (OleDbConnection connExcel = new OleDbConnection(conString))
                    {
                        using (OleDbCommand cmdExcel = new OleDbCommand())
                        {
                            using (OleDbDataAdapter odaExcel = new OleDbDataAdapter())
                            {
                                DataTable dt = new DataTable();
                                cmdExcel.Connection = connExcel;

                                //Get the name of First Sheet.
                                connExcel.Open();
                                DataTable dtExcelSchema;
                                dtExcelSchema = connExcel.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                                string sheetName = dtExcelSchema.Rows[0]["TABLE_NAME"].ToString();
                                connExcel.Close();

                                //Read Data from First Sheet.
                                connExcel.Open();
                                cmdExcel.CommandText = "SELECT * From [" + sheetName + "]";
                                odaExcel.SelectCommand = cmdExcel;
                                odaExcel.Fill(dt);
                                connExcel.Close();

                                foreach (DataRow row in dt.Rows)
                                {
                                    ContractStagePayment stage = new ContractStagePayment();
                                    
                                    if (row["ContractId"] != DBNull.Value)
                                    {
                                        stage.ContractId = sanitizer.Sanitize(row["ContractId"].ToString());
                                    }
                                    if (row["StageNumber"] != DBNull.Value)
                                    {
                                        stage.StageNumber = Convert.ToInt32(sanitizer.Sanitize(row["StageNumber"].ToString()));
                                    }
                                    if (row["RevisedDateOfpayment"] != DBNull.Value)
                                    {
                                        stage.RevisedDateOfpayment = Convert.ToDateTime(sanitizer.Sanitize(row["RevisedDateOfpayment"].ToString()));
                                    }
                                    if (row["ReasonsForSlippage"] != DBNull.Value)
                                    {
                                        stage.ReasonsForSlippage = sanitizer.Sanitize(row["ReasonsForSlippage"].ToString());
                                    }
                                    if (row["ActualDateOfPayment"] != DBNull.Value)
                                    {
                                        stage.ActualDateOfPayment = Convert.ToDateTime(sanitizer.Sanitize(row["ActualDateOfPayment"].ToString()));
                                    }
                                    if (row["TotalPaymentMade"] != DBNull.Value)
                                    {
                                        stage.TotalPaymentMade = Convert.ToDecimal(sanitizer.Sanitize(row["TotalPaymentMade"].ToString()));
                                    }
                                    if (row["FullorPartPaymentMade"] != DBNull.Value)
                                    {
                                        stage.FullorPartPaymentMade = sanitizer.Sanitize(row["FullorPartPaymentMade"].ToString());
                                    }
                                    if (row["ExpendMadeTill31March"] != DBNull.Value)
                                    {
                                        stage.ExpendMadeTill31March = Convert.ToDecimal(sanitizer.Sanitize(row["ExpendMadeTill31March"].ToString()));
                                    }

                                    Contracts.Add(stage);



                                }
                            }
                        }
                    }

                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(WebAPIUrl);
                        HttpResponseMessage postJob = await client.PostAsJsonAsync<List<ContractStagePayment>>(WebAPIUrl + "Contract/SaveStagePaymentExcel", Contracts);
                        bool postResult = postJob.IsSuccessStatusCode;
                        if (postResult == true)
                        {

                            Massege = "Success";
                            TempData["Uploadsuccess"] = true;
                        }
                        else
                        {
                            TempData["Uploadsuccess"] = false;
                            Massege = "Failed";
                        }
                    }
                    //}
                    //}


                }
                //}
                //else
                //{
                //    TempData["FileStage"] = "Upload Only Excel.";
                //    ModelState.AddModelError("", "Please upload Only Excel File");
                //    return RedirectToAction("Contract", "AONW");
                //}
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                string val = ex.Message.ToString();
                Match match = Regex.Match(val, @"'([^']*)");
                if (match.Success)
                {
                    string col = match.Groups[1].Value;
                    TempData["StagePaymentColoumn"] = "Please check excel coloumn name. It should be" + " " + col + ".";
                }
                ModelState.AddModelError("", "Please upload Only Excel File");
                return RedirectToAction("Contract", "Contract");
            }
            return RedirectToAction("Contract", "Contract");
        }
        [Route("ContractList")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ContractList()
        {
            ContractDetails Socmodel = new ContractDetails();
            List<ContractDetails> listData = new List<ContractDetails>();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                         parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                    HttpResponseMessage response = client.GetAsync("Contract/ContractList").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        ViewBag.ContractList = response.Content.ReadAsAsync<List<ContractDetails>>().Result;
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                return View();
            }

            return View();
        }

        [Route("ContractStageList")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ContractAndStageList(string ContractId, bool cont = false)
        {
            ContractsAndStageList contract = new ContractsAndStageList();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                         parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                    HttpResponseMessage response = client.GetAsync("Contract/ContractAndStageList?ContractId=" + ContractId + "").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        contract = response.Content.ReadAsAsync<ContractsAndStageList>().Result;
                    }
                }
                ViewBag.cont = cont;
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                return View();
            }


            return View(contract);
        }

        [Route("UpdateContract")]
        [HttpPost]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateContract(Contracts cnt)
        {
            string Msg = "";
            try
            {
                Contracts _contracts = new Contracts();
                ContractDetails contractDetails = new ContractDetails();
                contractDetails.Id = Convert.ToInt32(sanitizer.Sanitize(cnt.Contrct_Detail.Id.ToString()));
                contractDetails.ContractId = sanitizer.Sanitize(cnt.Contrct_Detail.ContractId);
                contractDetails.Contract_Number = sanitizer.Sanitize(cnt.Contrct_Detail.Contract_Number);
                if (cnt.Contrct_Detail.DateOfContractSigning != null)
                {
                    contractDetails.DateOfContractSigning = Convert.ToDateTime(sanitizer.Sanitize(cnt.Contrct_Detail.DateOfContractSigning.ToString()));
                }
                contractDetails.Descriptions = sanitizer.Sanitize(cnt.Contrct_Detail.Descriptions);
                contractDetails.Category = sanitizer.Sanitize(cnt.Contrct_Detail.Category);

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
                    stageDetail.Id = Convert.ToInt32(sanitizer.Sanitize(item.Id.ToString()));
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
                   

                    stages.Add(stageDetail);
                }

                _contracts.Stage_Detail = stages;

                List<ContractStagePayment> stagesPayList = new List<ContractStagePayment>();
                foreach (var pay in cnt.Stage_Payment_Detail.ToList())
                {
                    ContractStagePayment stagepay = new ContractStagePayment();
                    stagepay.Id = Convert.ToInt32(sanitizer.Sanitize(pay.Id.ToString()));
                    if (pay.StageNumber != null)
                    {
                        stagepay.StageNumber = Convert.ToInt32(sanitizer.Sanitize(pay.StageNumber.ToString()));
                    }
                    if (pay.RevisedDateOfpayment != null)
                    {
                        stagepay.RevisedDateOfpayment = Convert.ToDateTime(sanitizer.Sanitize(pay.RevisedDateOfpayment.ToString()));
                    }
                    if (pay.ReasonsForSlippage != null)
                    {
                        stagepay.ReasonsForSlippage = sanitizer.Sanitize(pay.ReasonsForSlippage);
                    }
                    if (pay.ActualDateOfPayment != null)
                    {
                        stagepay.ActualDateOfPayment = Convert.ToDateTime(sanitizer.Sanitize(pay.ActualDateOfPayment.ToString()));
                    }

                    if (pay.TotalPaymentMade > 0)
                    {
                        stagepay.TotalPaymentMade = Convert.ToDecimal(sanitizer.Sanitize(pay.TotalPaymentMade.ToString()));
                    }
                    if (pay.FullorPartPaymentMade != null)
                    {
                        stagepay.FullorPartPaymentMade = sanitizer.Sanitize(pay.FullorPartPaymentMade);
                    }
                    if (pay.ExpendMadeTill31March != null)
                    {
                        stagepay.ExpendMadeTill31March = Convert.ToDecimal(sanitizer.Sanitize(pay.ExpendMadeTill31March.ToString()));
                    }
                    stagesPayList.Add(stagepay);
                }
                _contracts.Stage_Payment_Detail = stagesPayList;


                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    HttpResponseMessage postJob = await client.PostAsJsonAsync<Contracts>(WebAPIUrl + "Contract/UpdateContract", _contracts);
                    bool postResult = postJob.IsSuccessStatusCode;
                    if (postResult == true)
                    {
                        Msg = "Updated";
                    }
                    else
                    {
                        Msg = "Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                Msg = "Failed";
                ViewBag.Message = ex.Message.ToString();
                return Json(Msg, JsonRequestBehavior.AllowGet);
            }
            return Json(Msg, JsonRequestBehavior.AllowGet);
        }
        [Route("ContractReport")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ContractReport()
        {

            return View();
        }
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult GetContractBaseOnServices(string Service = "")
        {
            string msg = "";
            ContractPaymentSum model = new ContractPaymentSum();

            ViewBag.Service = Service;
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                         parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                    HttpResponseMessage response = client.GetAsync("Contract/GetContractBaseOnServices?Service=" + Service + "").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        model = response.Content.ReadAsAsync<ContractPaymentSum>().Result;
                    }
                }

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                return RedirectToAction("ContractReport", "Contract");
            }
            return PartialView("_ContractMasterPartial", model);
        }

        [Route("GetContractBaseOnFinancialYear")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult GetContractBaseOnFinancialYear(string Service = "", string FinancialYear = "")
        {
            ContractPaymentSum model = new ContractPaymentSum();


            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                         parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                    HttpResponseMessage response = client.GetAsync("Contract/GetContractBaseOnFinancialYear?Service=" + Service + "&FinancialYear=" + FinancialYear + "").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        model = response.Content.ReadAsAsync<ContractPaymentSum>().Result;
                        ViewBag.Service = model.FinancialYear.Select(s => s.Service).FirstOrDefault();
                        ViewBag.FinancialYear = model.FinancialYear.Select(f => f.FinancialYear).FirstOrDefault();
                    }
                }

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                return RedirectToAction("GetContractBaseOnFinancialYear", "Contract", new { Service = Service, FinancialYear = FinancialYear });

            }
            return View(model);
        }

        [Route("GetContractBaseOnContractID")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult GetContractBaseOnContractID(string Service = "", string ContractId = "")
        {
            ContractPaymentSum model = new ContractPaymentSum();


            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                         parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                    HttpResponseMessage response = client.GetAsync("Contract/GetContractBaseOnContractID?Service=" + Service + "&ContractId=" + ContractId + "").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        model = response.Content.ReadAsAsync<ContractPaymentSum>().Result;
                        ViewBag.Service = model.FinancialYear.Select(s => s.Service).FirstOrDefault();
                        ViewBag.ContractId = model.FinancialYear.Select(s => s.ContractId).FirstOrDefault();
                    }
                }

            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message.ToString();
                return RedirectToAction("GetContractBaseOnContractID", "Contract", new { Service = Service, ContractId = ContractId });

            }
            return View(model);
        }

        [Route("UpdateContractStagePayment")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<JsonResult> UpdateContractStagePayment(ContractStagePayment cnt)
        {
            string msg = "";
            try
            {
                ContractStagePayment _contracts = new ContractStagePayment();
                ContractStagePayment contractDetails = new ContractStagePayment();
                if (cnt.ContractId != null)
                {
                    contractDetails.ContractId = sanitizer.Sanitize(cnt.ContractId);
                }
                if (cnt.StageNumber > 0)
                {
                    contractDetails.StageNumber = Convert.ToInt32(sanitizer.Sanitize(cnt.StageNumber.ToString()));
                }
                if (cnt.RevisedDateOfpayment != null)
                {
                    contractDetails.RevisedDateOfpayment = Convert.ToDateTime(sanitizer.Sanitize(cnt.RevisedDateOfpayment.ToString()));
                }
                if (cnt.ReasonsForSlippage != null)
                {
                    contractDetails.ReasonsForSlippage = sanitizer.Sanitize(cnt.ReasonsForSlippage.ToString());
                }
                if (cnt.ActualDateOfPayment != null)
                {
                    contractDetails.ActualDateOfPayment = Convert.ToDateTime(sanitizer.Sanitize(cnt.ActualDateOfPayment.ToString()));
                }

                if (cnt.FullorPartPaymentMade != null)
                {
                    contractDetails.FullorPartPaymentMade = sanitizer.Sanitize(cnt.FullorPartPaymentMade.ToString());
                }

                if (cnt.TotalPaymentMade > 0)
                {
                    contractDetails.TotalPaymentMade = Convert.ToDecimal(sanitizer.Sanitize(cnt.TotalPaymentMade.ToString()));
                }
                if (cnt.ExpendMadeTill31March > 0)
                {
                    contractDetails.ExpendMadeTill31March = Convert.ToDecimal(sanitizer.Sanitize(cnt.ExpendMadeTill31March.ToString()));
                }

                _contracts = contractDetails;

                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    HttpResponseMessage postJob = await client.PostAsJsonAsync<ContractStagePayment>(WebAPIUrl + "Contract/UpdateContractStagePayment", _contracts);
                    bool postResult = postJob.IsSuccessStatusCode;
                    if (postResult == true)
                    {

                        msg = "Success";
                    }
                    else
                    {

                        msg = "Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                msg = "Failed";
                ViewBag.Message = ex.Message.ToString();
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            return Json(msg, JsonRequestBehavior.AllowGet);
        }

    }
}