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
                                    Contracts.Add(new ContractDetails
                                    {
                                        ContractId = row["ContractId"].ToString(),
                                        Contract_Number = row["Contract_Number"].ToString(),
                                        DateOfContractSigning = Convert.ToDateTime(row["DateOfContractSigning"]),
                                        Descriptions = row["Descriptions"].ToString(),
                                        Category = row["Category"].ToString(),
                                        EffectiveDate = Convert.ToDateTime(row["EffectiveDate"]),
                                        ABGDate = Convert.ToDateTime(row["ABGDate"]),
                                        PWBGPercentage = Convert.ToInt32(row["PWBGPercentage"]),
                                        PWBGDate = Convert.ToDateTime(row["PWBGDate"]),
                                        Incoterms = row["Incoterms"].ToString(),
                                        Warranty = row["Warranty"].ToString(),
                                        ContractValue = row["ContractValue"].ToString(),
                                        FEContent = row["FEContent"].ToString(),
                                        TaxesAndDuties = row["TaxesAndDuties"].ToString(),
                                        FinalDeliveryDate = Convert.ToDateTime(row["FinalDeliveryDate"]),
                                        GracePeriod = row["GracePeriod"].ToString(),
                                        Services = row["Services"].ToString()
                                    });
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
                    }
                    else
                    {

                        Massege = "Failed";
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            //return Json(Massege, JsonRequestBehavior.AllowGet);
            return RedirectToAction("Contract","AONW");
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
                                    Contracts.Add(new StageDetail
                                    {
                                        ContractmasterId = Convert.ToInt32(row["ContractmasterId"]),
                                        StageNumber = Convert.ToInt32(row["StageNumber"]),
                                        stageDescription = row["stageDescription"].ToString(),
                                        StageStartdate = Convert.ToDateTime(row["StageStartdate"]),
                                        StageCompletionDate = Convert.ToDateTime(row["StageCompletionDate"]),
                                        PercentOfContractValue = Convert.ToInt32(row["PercentOfContractValue"]),
                                        Amount = Convert.ToDecimal(row["Amount"]),
                                        DueDateOfPayment = Convert.ToDateTime(row["DueDateOfPayment"]),
                                        Conditions = row["Conditions"].ToString(),
                                        RevisedDateOfpayment = Convert.ToDateTime(row["RevisedDateOfpayment"]),
                                        ReasonsForSlippage = row["ReasonsForSlippage"].ToString(),
                                        ActualDateOfPayment = Convert.ToDateTime(row["ActualDateOfPayment"]),
                                        TotalPaymentMade = row["TotalPaymentMade"].ToString(),
                                        FullorPartPaymentMade = row["FullorPartPaymentMade"].ToString()

                                    });
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
                    }
                    else
                    {

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
        public ActionResult ContractAndStageList(string ConMasterId)
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
                    HttpResponseMessage response = client.GetAsync("Contract/ContractAndStageList?ConMasterId=" + ConMasterId + "").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        //ViewBag.ContractStageList = response.Content.ReadAsAsync<Contracts>().Result;
                        contract = response.Content.ReadAsAsync<Contracts>().Result;
                    }
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }


            return View(contract);
        }

    }
}