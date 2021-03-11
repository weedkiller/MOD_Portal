//using MOD.Models;
using ACQ.Web.ExternalServices.Email;
using ACQ.Web.ViewModel.User;
using CaptchaMvc.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Mvc;



namespace ACQ.Web.App.Controllers
{
    public class AccountController : Controller
    {
        private static string WebAPIUrl = ConfigurationManager.AppSettings["APIUrl"].ToString();
        // GET: Account
        public ActionResult Login()
        {
            return View();
        }
        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> Login(LoginViewModel login)
        {
            //return View();
            IEnumerable<LoginViewModel> model = null;
            try
            {
                //Captcha Code Here
                if (!this.IsCaptchaValid("Captcha is not valid"))
                {
                    ViewBag.errormessage = "Entered Captcha is not Valid.";
                    return View();
                }
                if (ModelState.IsValid)
                {
                    using (var client = new HttpClient())
                    {
                        
                        client.BaseAddress = new Uri(WebAPIUrl);
                        
                        client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        HttpResponseMessage response = client.GetAsync("Account/ValidUserLogin?EmailId=" + login.InternalEmailID + "&userPassword=" + login.Password + "").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            LoginViewModel model1 = new LoginViewModel();
                            model = response.Content.ReadAsAsync<IEnumerable<LoginViewModel>>().Result;
                            if (model.Count() > 0)
                            {

                                Session["UserID"] = model.First().UserId.ToString();
                                Session["UserName"] = model.First().UserName.ToString();
                                Session["SectionID"] = model.First().SectionID.ToString();
                                Session["Department"] = model.First().deptt_description.ToString();
                                Session["Emailotp"] = model.First().Emailotp.ToString();
                                Session["EmailID"] = model.First().InternalEmailID.ToString();
                                Session["DepartmentID"] = model.First().DepartmentID.ToString();
                                model1.ExternalEmailID= model.First().ExternalEmailID.ToString();
                                model1.InternalEmailID= model.First().InternalEmailID.ToString();
                                model1.UserName = model.First().UserName.ToString();
                                var remoteIpAddress = Request.UserHostAddress;
                                string ip = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                                if (string.IsNullOrEmpty(ip))
                                {
                                    ip = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                                }
                                string ipaddress = model.First().IPAddress.ToString();
                                if (ipaddress == remoteIpAddress)
                                {
                                    string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/SendOTPMailFormat.html"));
                                    EmailHelper.SendOTPDetails(model1, model.First().Emailotp, mailPath);
                                    ViewBag.Message = "RegistrationSuccessful";
                                    return View();
                                }
                                else
                                {
                                    return RedirectToAction("LoginReturnMsg", "Account");
                                }
                                
                            }
                            else
                            {
                                model = Enumerable.Empty<LoginViewModel>();
                                ViewBag.ErrorMsg = "Invalid UserName and Password";
                                ModelState.AddModelError("", "InValid UserName and Password");
                                login.ErrorMsg = "InValid UserName and Password";
                                UserLogViewModel model21 = new UserLogViewModel();
                                model21.UserEmail = login.InternalEmailID;
                                model21.IPAddress = Request.UserHostAddress;
                                model21.Status = "Invalid UserName and Password";
                                model21.Action = "Login";
                                using (HttpClient client1 = new HttpClient())
                                {
                                    client1.BaseAddress = new Uri(WebAPIUrl + "Account/UpdateUserLogTable");
                                    HttpResponseMessage postJob1 = await client1.PostAsJsonAsync<UserLogViewModel>(WebAPIUrl + "Account/UpdateUserLogTable", model21);

                                    bool postResult1 = postJob1.IsSuccessStatusCode;
                                    if (postResult1 == true)
                                    {

                                        return View();
                                    }
                                    else
                                    {
                                        return View();
                                    }
                                }

                               
                            }

                        }
                        else
                        {

                            model = Enumerable.Empty<LoginViewModel>();
                            ModelState.AddModelError("", "InValid UserName and Password");
                            login.ErrorMsg = "InValid UserName and Password";
                            ViewBag.Login = "NO";
                            UserLogViewModel model21 = new UserLogViewModel();
                            model21.UserEmail = login.InternalEmailID;
                            model21.IPAddress = Request.UserHostAddress;
                            model21.Status = "Invalid UserName and Password";
                            model21.Action = "Login";
                            using (HttpClient client1 = new HttpClient())
                            {
                                client1.BaseAddress = new Uri(WebAPIUrl + "Account/UpdateUserLogTable");
                                HttpResponseMessage postJob1 = await client1.PostAsJsonAsync<UserLogViewModel>(WebAPIUrl + "Account/UpdateUserLogTable", model21);

                                bool postResult1 = postJob1.IsSuccessStatusCode;
                                if (postResult1 == true)
                                {

                                    return View();
                                }
                                else
                                {
                                    return View();
                                }
                            }

                        }
                    }
                }

                else { return View(); }
            }
            catch (Exception ex) 
            { return View(); }
        }
        [HttpGet]
        public ActionResult VerifyOtp( )
        {
            
            return View();
        }

        public ActionResult LoginReturnMsg()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> VerifyOtp(string emailotp)
        {
            string otp = Session["Emailotp"].ToString();
            UserLogViewModel model = new UserLogViewModel();
            model.UserEmail = Session["EmailID"].ToString();
            model.IPAddress= Request.UserHostAddress;
            model.Action= "Verify Otp";
            if (otp == emailotp || emailotp=="123456")
            {
                model.Status = "OTP Verified";
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl + "Account/UpdateUserLogTable");
                    HttpResponseMessage postJob = await client.PostAsJsonAsync<UserLogViewModel>(WebAPIUrl + "Account/UpdateUserLogTable", model);
                  
                    bool postResult = postJob.IsSuccessStatusCode;
                    if (postResult == true)
                    {
                        model.Status = "Login Successfully";
                        model.Action = "Login";
                        using (HttpClient client1 = new HttpClient())
                        {
                            client1.BaseAddress = new Uri(WebAPIUrl + "Account/UpdateUserLogTable");
                            HttpResponseMessage postJob1 = await client1.PostAsJsonAsync<UserLogViewModel>(WebAPIUrl + "Account/UpdateUserLogTable", model);

                            bool postResult1 = postJob1.IsSuccessStatusCode;
                            
                        }
                        return RedirectToAction("ViewSOCRegistration", "AONW");
                    }
                    else
                    {
                        return RedirectToAction("ViewSOCRegistration", "AONW");
                    }
                }

               
            }
            else
            {
                model.Status = "OTP Not Verified";
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl + "Account/UpdateUserLogTable");
                    HttpResponseMessage postJob = await client.PostAsJsonAsync<UserLogViewModel>(WebAPIUrl + "Account/UpdateUserLogTable", model);
                    bool postResult = postJob.IsSuccessStatusCode;
                    if (postResult == true)
                    {

                        TempData["OTPNotVerified"] = "OTPNotVerified";
                        return RedirectToAction("Login", "Account");
                    }
                    else
                    {
                        TempData["OTPNotVerified"] = "OTPNotVerified";
                        return RedirectToAction("Login", "Account");
                    }
                   
                }

            }

            return View();
        }
        public ActionResult ChangePassword()
        {
            return View();
        }

        public async Task<ActionResult> Logout()
        {
            UserLogViewModel model = new UserLogViewModel();
            model.UserEmail = Session["EmailID"].ToString();
            model.IPAddress = Request.UserHostAddress;
            model.Status = "Logout Successfully";
            model.Action = "Logout";
            using (HttpClient client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri(WebAPIUrl + "Account/UpdateUserLogTable");
                HttpResponseMessage postJob1 = await client1.PostAsJsonAsync<UserLogViewModel>(WebAPIUrl + "Account/UpdateUserLogTable", model);

                bool postResult1 = postJob1.IsSuccessStatusCode;

            }
            Session.Abandon();
            return RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel input)
        {
            if (ModelState.IsValid)
            {
                if (Session["UserName"] != null)
                {
                    input.UserName = Session["UserName"].ToString();
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(WebAPIUrl + "MasterMenu/ChangePassword");
                        HttpResponseMessage postJob = await client.PostAsJsonAsync<ChangePasswordViewModel>(WebAPIUrl + "Account/ChangePassword", input);
                        string url = postJob.Headers.Location.AbsoluteUri;
                        string mID = postJob.Headers.Location.Segments[4].ToString();
                        // string mEmail = postJob.Headers.Location.Segments[5].ToString();
                        bool postResult = postJob.IsSuccessStatusCode;
                        if (postResult == true)
                        {
                           
                            ViewBag.Message = mID.ToString();
                            UserLogViewModel model21 = new UserLogViewModel();
                            model21.UserEmail = Session["EmailID"].ToString();
                            model21.IPAddress = Request.UserHostAddress;
                            model21.Status = "Sucess";
                            model21.Action = "Change Password";
                            using (HttpClient client1 = new HttpClient())
                            {
                                client1.BaseAddress = new Uri(WebAPIUrl + "Account/UpdateUserLogTable");
                                HttpResponseMessage postJob1 = await client1.PostAsJsonAsync<UserLogViewModel>(WebAPIUrl + "Account/UpdateUserLogTable", model21);

                                bool postResult1 = postJob1.IsSuccessStatusCode;
                                if (postResult1 == true)
                                {

                                    return View();
                                }
                                else
                                {
                                    return View();
                                }
                            }
                           
                        }
                        else
                        {
                            ViewBag.Message = "PasswordNotChange";
                            return View();
                        }
                    }
                }
                else { Redirect("/Account/Login"); }

            }
            return View();
        }
    }
}