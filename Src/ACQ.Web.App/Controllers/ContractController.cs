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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static ACQ.Web.App.MvcApplication;

namespace ACQ.Web.App.Controllers
{
    public class ContractController : Controller
    {

        HtmlSanitizer sanitizer = new HtmlSanitizer();

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






        [Route("SaveContractMasterExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveContractMasterExcel(ImportExcel excel)
        {
            string Massege = "";
            try
            {
                List<ContractDetails> Contracts = new List<ContractDetails>();
                string filePath = string.Empty;
                if (excel.file != null)
                {
                    string path = Server.MapPath("/ExcelFile/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(excel.file.FileName);
                    string extension = Path.GetExtension(excel.file.FileName);
                    excel.file.SaveAs(filePath);

                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".xls": //Excel 97-03.
                            //conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
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
                                    ContractDetails stage = new ContractDetails();
                                    if (row["ContractId"] != DBNull.Value)
                                    {
                                        stage.ContractId = row["ContractId"].ToString();
                                    }
                                    if (row["Contract_Number"] != DBNull.Value)
                                    {
                                        stage.Contract_Number = row["Contract_Number"].ToString();
                                    }
                                    if (row["DateOfContractSigning"] != DBNull.Value)
                                    {
                                        stage.DateOfContractSigning = Convert.ToDateTime(row["DateOfContractSigning"]);
                                    }
                                    if (row["Descriptions"] != DBNull.Value)
                                    {
                                        stage.Descriptions = row["Descriptions"].ToString();
                                    }
                                    if (row["Category"] != DBNull.Value)
                                    {
                                        stage.Category = row["Category"].ToString();
                                    }
                                    if (row["EffectiveDate"] != DBNull.Value)
                                    {
                                        stage.EffectiveDate = Convert.ToDateTime(row["EffectiveDate"]);
                                    }
                                    if (row["ABGDate"] != DBNull.Value)
                                    {
                                        stage.ABGDate = Convert.ToDateTime(row["ABGDate"]);
                                    }
                                    if (row["PWBGPercentage"] != DBNull.Value)
                                    {
                                        stage.PWBGPercentage = Convert.ToInt32(row["PWBGPercentage"]);
                                    }
                                    if (row["PWBGDate"] != DBNull.Value)
                                    {
                                        stage.PWBGDate = Convert.ToDateTime(row["PWBGDate"]);
                                    }
                                    if (row["Incoterms"] != DBNull.Value)
                                    {
                                        stage.Incoterms = row["Incoterms"].ToString();
                                    }
                                    if (row["Warranty"] != DBNull.Value)
                                    {
                                        stage.Warranty = row["Warranty"].ToString();
                                    }
                                    if (row["ContractValue"] != DBNull.Value)
                                    {
                                        stage.ContractValue = Convert.ToDecimal(row["ContractValue"]);
                                    }
                                    if (row["FEContent"] != DBNull.Value)
                                    {
                                        stage.FEContent = row["FEContent"].ToString();
                                    }
                                    if (row["TaxesAndDuties"] != DBNull.Value)
                                    {
                                        stage.TaxesAndDuties = row["TaxesAndDuties"].ToString();
                                    }
                                    if (row["FinalDeliveryDate"] != DBNull.Value)
                                    {
                                        stage.FinalDeliveryDate = Convert.ToDateTime(row["FinalDeliveryDate"]);
                                    }
                                    if (row["GracePeriod"] != DBNull.Value)
                                    {
                                        stage.GracePeriod = row["GracePeriod"].ToString();
                                    }
                                    if (row["Services"] != DBNull.Value)
                                    {
                                        stage.Services = row["Services"].ToString();
                                    }
                                    Contracts.Add(stage);


                                }
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return Json(Massege, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Contract", "AONW");
        }

        [Route("SaveStageExcel")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SaveStageExcel(ImportExcel excel)
        {
            string Massege = "";
            try
            {
                List<StageDetail> Contracts = new List<StageDetail>();
                string filePath = string.Empty;
                if (excel.file != null)
                {
                    string path = Server.MapPath("/ExcelFile/");
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }

                    filePath = path + Path.GetFileName(excel.file.FileName);
                    string extension = Path.GetExtension(excel.file.FileName);
                    excel.file.SaveAs(filePath);

                    string conString = string.Empty;
                    switch (extension)
                    {
                        case ".xls": //Excel 97-03.
                            //conString = ConfigurationManager.ConnectionStrings["Excel03ConString"].ConnectionString;
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
                                    if (row["ContractmasterId"] != DBNull.Value)
                                    {
                                        stage.ContractmasterId = Convert.ToInt32(row["ContractmasterId"]);
                                    }
                                    if (row["ContractId"] != DBNull.Value)
                                    {
                                        stage.ContractId = row["ContractId"].ToString();
                                    }
                                    if (row["StageNumber"] != DBNull.Value)
                                    {
                                        stage.StageNumber = Convert.ToInt32(row["StageNumber"]);
                                    }
                                    if (row["stageDescription"] != DBNull.Value)
                                    {
                                        stage.stageDescription = row["stageDescription"].ToString();
                                    }
                                    if (row["StageStartdate"] != DBNull.Value)
                                    {
                                        stage.StageStartdate = Convert.ToDateTime(row["StageStartdate"]);
                                    }
                                    if (row["StageCompletionDate"] != DBNull.Value)
                                    {
                                        stage.StageCompletionDate = Convert.ToDateTime(row["StageCompletionDate"]);
                                    }
                                    if (row["PercentOfContractValue"] != DBNull.Value)
                                    {
                                        stage.PercentOfContractValue = Convert.ToInt32(row["PercentOfContractValue"]);
                                    }
                                    if (row["Amount"] != DBNull.Value)
                                    {
                                        stage.Amount = Convert.ToDecimal(row["Amount"]);
                                    }
                                    if (row["DueDateOfPayment"] != DBNull.Value)
                                    {
                                        stage.DueDateOfPayment = Convert.ToDateTime(row["DueDateOfPayment"]);
                                    }
                                    if (row["Conditions"] != DBNull.Value)
                                    {
                                        stage.Conditions = row["Conditions"].ToString();
                                    }
                                    if (row["RevisedDateOfpayment"] != DBNull.Value)
                                    {
                                        stage.RevisedDateOfpayment = Convert.ToDateTime(row["RevisedDateOfpayment"]);
                                    }
                                    if (row["ReasonsForSlippage"] != DBNull.Value)
                                    {
                                        stage.ReasonsForSlippage = row["ReasonsForSlippage"].ToString();
                                    }
                                    if (row["ActualDateOfPayment"] != DBNull.Value)
                                    {
                                        stage.ActualDateOfPayment = Convert.ToDateTime(row["ActualDateOfPayment"]);
                                    }
                                    if (row["TotalPaymentMade"] != DBNull.Value)
                                    {
                                        stage.TotalPaymentMade = Convert.ToDecimal(row["TotalPaymentMade"]);
                                    }
                                    if (row["FullorPartPaymentMade"] != DBNull.Value)
                                    {
                                        stage.FullorPartPaymentMade = row["FullorPartPaymentMade"].ToString();
                                    }
                                    if (row["ExpendMadeTill31March"] != DBNull.Value)
                                    {
                                        stage.ExpendMadeTill31March = Convert.ToDecimal(row["ExpendMadeTill31March"]);
                                    }

                                    Contracts.Add(stage);



                                }
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
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return Json(Massege, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Contract", "AONW");
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

                throw ex;
            }


            return View();
        }

        [Route("ContractStageList")]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ContractAndStageList(string ContractId, bool cont=false)
        {
            Contracts contract = new Contracts();
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
                        //ViewBag.ContractStageList = response.Content.ReadAsAsync<Contracts>().Result;
                        contract = response.Content.ReadAsAsync<Contracts>().Result;
                        var des = contract.Contrct_Detail.Descriptions;
                    }
                }
                ViewBag.cont = cont;
            }
            catch (Exception ex)
            {

                
            }


            return View(contract);
        }

        [Route("UpdateContract")]
        [HttpPost]
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
                    if (item.TotalPaymentMade > 0)
                    {
                        stageDetail.TotalPaymentMade = Convert.ToDecimal(sanitizer.Sanitize(item.TotalPaymentMade.ToString()));
                    }
                    stageDetail.FullorPartPaymentMade = sanitizer.Sanitize(item.FullorPartPaymentMade);
                    if (item.ExpendMadeTill31March > 0)
                    {
                        stageDetail.ExpendMadeTill31March = Convert.ToDecimal(sanitizer.Sanitize(item.ExpendMadeTill31March.ToString()));
                    }

                    stages.Add(stageDetail);
                }

                _contracts.Stage_Detail = stages;


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
                throw ex;
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
      
        public ActionResult GetContractBaseOnServices(string Service="")
        {
            string msg = "";
            StageDetail contract = new StageDetail();
          
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

                        //ViewBag.ContractList = response.Content.ReadAsAsync<List<ContractDetails>>().Result;
                        contract = response.Content.ReadAsAsync<StageDetail>().Result;

                    }
                }
               
            }
            catch (Exception ex)
            {


            }
            return PartialView("_ContractMasterPartial", contract);
        }

        public async Task<PartialViewResult> GetStageSumPayment(string ContractId = "")
        {
            string msg = "";
            Contracts contract = new Contracts();
            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl);
                    //HTTP GET
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                         parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                    HttpResponseMessage response = client.GetAsync("Contract/GetSumOfStagePayment?ContractId=" + ContractId + "").Result;

                    if (response.IsSuccessStatusCode)
                    {

                        ViewBag.SumOfStage = response.Content.ReadAsAsync<List<StageDetail>>().Result;

                    }
                }

            }
            catch (Exception ex)
            {


            }
            return PartialView("_ContractStagePartial", ViewBag.SumOfStage);
        }
    }
}