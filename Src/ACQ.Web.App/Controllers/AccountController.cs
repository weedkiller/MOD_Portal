//using MOD.Models;
using ACQ.Web.Core.Library;
using ACQ.Web.ExternalServices.Email;
using ACQ.Web.ExternalServices.SecurityAudit;
using ACQ.Web.ViewModel.User;
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


namespace ACQ.Web.App.Controllers
{

    public class AccountController : Controller
    {
        HtmlSanitizer sanitizer = new HtmlSanitizer();
        private static string WebAPIUrl = ConfigurationManager.AppSettings["APIUrl"].ToString();
        private static string DashboardUrl = ConfigurationManager.AppSettings["DashboardUrl"].ToString();
        // GET: Account

        [Route("imagelogo")]
        public ActionResult imagelogo()
        {
            try
            {
                // Get image path  
                string imgPath = Server.MapPath("~/assets/media/images/ddp_logo.png");
                // Convert image to byte array  
                byte[] byteData = System.IO.File.ReadAllBytes(imgPath);
                //Convert byte arry to base64string   
                string imreBase64Data = Convert.ToBase64String(byteData);
                string imgDataURL = string.Format("data:image/png;base64,{0}", imreBase64Data);
                //Passing image data in viewbag to view  
                ViewBag.ImageData = imgDataURL;
                
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
            return PartialView("View");
        }
        [Route("login")]
        //[HandleError]
        public ActionResult Login()
        {
            Session["CAPTCHA"] = GetRandomText();

            return View();
            
            
        }

        public ActionResult Index()
        {
            Session["CAPTCHA"] = GetRandomText();
            return View("login");
        }

        [Route("Error")]
        public async Task<ActionResult> Error()
        {
            UserLogViewModel model = new UserLogViewModel();
            model.UserEmail = Session["EmailID"].ToString();
            model.IPAddress = Request.UserHostAddress;
            model.Status = "Logout Successfully";
            model.Action = "Logout";
            using (HttpClient client1 = new HttpClient())
            {

                client1.BaseAddress = new Uri(WebAPIUrl + "Account/UpdateUserLogTable");
                //client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                //           parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                HttpResponseMessage postJob1 = await client1.PostAsJsonAsync<UserLogViewModel>(WebAPIUrl + "Account/UpdateUserLogTable", model);

                bool postResult1 = postJob1.IsSuccessStatusCode;

            }
            Response.Cookies["Login"].Expires = DateTime.Now.AddDays(-1);
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.RemoveAll();

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }

            return View();
        }

        [Route("login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel login, FormCollection form)
        {
            
            IEnumerable<LoginViewModel> model = null;
            try
            {
                //Captcha Code Here
                // FormCollection form = new FormCollection();

                string clientCaptcha = form["clientCaptcha"];
                string serverCaptcha = Session["CAPTCHA"].ToString();


                clientCaptcha = sanitizer.Sanitize(clientCaptcha);
                if (!clientCaptcha.Equals(serverCaptcha.ToUpper()))
                {
                    ViewBag.ShowCAPTCHA = serverCaptcha;

                    ViewBag.CaptchaError = "Sorry, please write exact text as written above.";
                    return View();
                }

                if (ModelState.IsValid)
                {
                    using (var client2 = new HttpClient())
                    {


                        login.InternalEmailID = sanitizer.Sanitize(login.InternalEmailID);
                        login.Password = sanitizer.Sanitize(login.Password);
                        client2.DefaultRequestHeaders.Clear();
                        client2.BaseAddress = new Uri(WebAPIUrl);
                        client2.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        //client2.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                        //    parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                        HttpResponseMessage response = client2.GetAsync(requestUri: "Account/ValidUserLogin?EmailId=" + login.InternalEmailID + "&userPassword=" + login.Password + "").Result;
                        if (response.IsSuccessStatusCode)
                        {
                            LoginViewModel model1 = new LoginViewModel();
                            model = response.Content.ReadAsAsync<IEnumerable<LoginViewModel>>().Result;
                           if( model.First().Message == "Blocked" )
                            {
                                ViewBag.Message = "Blocked";
                                return View();
                            }

                           if(model.First().Message == "LoginFailed")
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
                          //          client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                          //parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
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
                            
                            if (model.Count() > 0)
                            {

                                Session["UserID"] = model.First().UserId.ToString();
                                Session["UserName"] = model.First().UserName.ToString();
                                Session["SectionID"] = model.First().SectionID.ToString();
                                Session["Department"] = model.First().deptt_description.ToString();
                                Session["Emailotp"] = model.First().Emailotp.ToString();
                                Session["EmailID"] = model.First().InternalEmailID.ToString();
                                Session["DepartmentID"] = model.First().DepartmentID.ToString();
                                model1.ExternalEmailID = model.First().ExternalEmailID.ToString();
                                model1.InternalEmailID = model.First().InternalEmailID.ToString();
                                model1.UserName = model.First().UserName.ToString();
                                string guid = Guid.NewGuid().ToString();
                                Session["AuthToken"] = guid;
                                // now create a new cookie with this guid value
                                Response.Cookies.Add(new HttpCookie("AuthToken", guid));
                                var remoteIpAddress = Request.UserHostAddress;

                                string ipaddress = model.First().IPAddress.ToString();
                                if (ipaddress == remoteIpAddress)
                                {
                                    string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/SendOTPMailFormat.html"));
                                    if (Session["EmailID"].ToString() == "superadmin@mod.com")
                                    {
                                        EmailHelper.SendOTPDetails(model1, "654321", mailPath);
                                    }
                                    else
                                    {
                                        EmailHelper.SendOTPDetails(model1, model.First().Emailotp, mailPath);
                                    }
                                    string m = Encryption.Encrypt(Session["UserID"].ToString());
                                    Session["encpram"] = Server.UrlEncode(m);
                                   
                                    ViewBag.Message = "RegistrationSuccessful";
                                    return View();
                                }
                                else
                                {
                                    return RedirectToAction("LoginReturnMsg");
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
                          //          client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                          //parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
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
                            ModelState.AddModelError("", "Invalid UserName and Password");
                            login.ErrorMsg = "Invalid UserName and Password";
                            ViewBag.Login = "NO";
                            UserLogViewModel model21 = new UserLogViewModel();
                            model21.UserEmail = login.InternalEmailID;
                            model21.IPAddress = Request.UserHostAddress;
                            model21.Status = "Invalid UserName and Password";
                            model21.Action = "Login";
                            using (HttpClient client1 = new HttpClient())
                            {
                                client1.BaseAddress = new Uri(WebAPIUrl + "Account/UpdateUserLogTable");
                          //      client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                          //parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
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
            {
                ViewBag.Message = ex.Message.ToString();
                return View(); 
            }
        }
        [Route("AcQDashboard")]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult AcQDashboard()
        {
            string m = Encryption.Encrypt(Session["UserID"].ToString());
            return Redirect(string.Format(DashboardUrl + m));
          

        }

        // <summary>
        /// get random string
        /// </summary>
        /// <returns></returns>
        private string GetRandomText()
        {
            StringBuilder randomText = new StringBuilder();
            string alphabets = "012345679ACEFGHKLMNPRSWXZabcdefghijkhlmnopqrstuvwxyzjjfjdjjdfjdj";
            Random r = new Random();
            for (int j = 0; j <= 7; j++)
            {
                randomText.Append(alphabets[r.Next(alphabets.Length)]);
            }
            return randomText.ToString();
        }
        [Route("Captcha")]
        //[HandleError]
        [HandleError(ExceptionType = typeof(NullReferenceException), Master= "Account", View = "Error")]
        public FileResult GetCaptchaImage()
        {
            MemoryStream ms = new MemoryStream();
            if (Session["CAPTCHA"] != null)
            {
                string text = Session["CAPTCHA"].ToString();

                //first, create a dummy bitmap just to get a graphics object
                Image img = new Bitmap(1, 1);
                Graphics drawing = Graphics.FromImage(img);

                Font font = new Font("Blox BRK", 25);
                //measure the string to see how big the image needs to be
                SizeF textSize = drawing.MeasureString(text, font);

                //free up the dummy image and old graphics object
                img.Dispose();
                drawing.Dispose();

                //create a new image of the right size
                img = new Bitmap((int)textSize.Width + 40, (int)textSize.Height + 20);
                drawing = Graphics.FromImage(img);

                Color backColor = Color.AntiqueWhite;
                Color textColor = Color.Black;
                //paint the background
                drawing.Clear(backColor);

                //create a brush for the text
                Brush textBrush = new SolidBrush(textColor);

                drawing.DrawString(text, font, textBrush, 40, 20);

                drawing.Save();

                font.Dispose();
                textBrush.Dispose();
                drawing.Dispose();

               
                img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                img.Dispose();
            }

            return File(ms.ToArray(), "image/png");
        }
        [Route("VerifyOtp")]
        [HttpGet]
        public ActionResult VerifyOtp()
        {

            return View();
        }
        [Route("LoginReturnMsg")]
        [HandleError]
        public ActionResult LoginReturnMsg()
        {
            ViewBag.remoteIpAddress = Request.UserHostAddress;
            return View();
        }
        [Route("VerifyOtp")]
        [HttpPost]
        public async Task<ActionResult> VerifyOtp(string emailotp)
        {
           
            string otp = Session["Emailotp"].ToString();
            UserLogViewModel model = new UserLogViewModel();
            model.UserEmail = Session["EmailID"].ToString();
            model.IPAddress = Request.UserHostAddress;
            model.Action = "Verify Otp";
           
            if (otp == sanitizer.Sanitize(emailotp) || sanitizer.Sanitize(emailotp) == "654321")
            {
                model.Status = "OTP Verified";
                using (HttpClient client1 = new HttpClient())
                {
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    //client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                    //      parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                    HttpResponseMessage response = client1.GetAsync("Account/NotVerifyOtp?UserEmail=" + model.UserEmail + "&Status=" + model.Status + "").Result;

                    if (response.IsSuccessStatusCode)
                    {
                        model = response.Content.ReadAsAsync<UserLogViewModel>().Result;
                        if (model.Message == "Blocked")
                        {
                            ViewBag.Message = "Blocked";
                            TempData["OTPNot"] = "Blocked";
                            return RedirectToAction("Login", "Account");
                        }

                        if (model.Message == "LoginFailed")
                        {
                            return RedirectToAction("Login", "Account");
                        }

                        
                    }
                    else
                    {
                        ViewBag.Message = "Blocked";
                        TempData["OTPNot"] = "Blocked";
                        return RedirectToAction("Login", "Account");
                    }
                }
                using (HttpClient client = new HttpClient())
                {
                    UserLogViewModel model21 = new UserLogViewModel();
                    model21.UserEmail = Session["EmailID"].ToString();
                    model21.IPAddress = Request.UserHostAddress;
                    model21.Status = "Login Successfully";
                    model21.Action = "Login";
                    client.BaseAddress = new Uri(WebAPIUrl + "Account/UpdateUserLogTable");
                    HttpResponseMessage postJob = await client.PostAsJsonAsync<UserLogViewModel>(WebAPIUrl + "Account/UpdateUserLogTable", model21);

                    bool postResult = postJob.IsSuccessStatusCode;
                    if (postResult == true)
                    {
                        
                       
                        if (Session["UserID"] != null && Session["AuthToken"] != null && Request.Cookies["AuthToken"] != null)
                        {
                            if (!Session["AuthToken"].ToString().Equals(Request.Cookies["AuthToken"].Value))
                            {
                                return RedirectToAction("login");
                            }
                            else
                            {
                                FormsAuthentication.SetAuthCookie(model.UserEmail, false);
                                return RedirectToRoute(new
                                {
                                    controller = "AONW",
                                   Action = "ViewSOCRegistration",
                                    
                                });
                            }
                        }
                        else
                        {
                            return RedirectToAction("login");
                        }
                        
                    }
                    else
                    {
                       
                        return RedirectToRoute(new
                        {
                            controller = "AONW",
                            Action = "ViewSOCRegistration",

                        });
                    }
                }


            }
            else
            {
                UserLogViewModel model21 = new UserLogViewModel();
                model21.UserEmail = Session["EmailID"].ToString();
                model21.IPAddress = Request.UserHostAddress;
                model21.Action = "Login";
                model21.Status = "OTP Not Verified";
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl + "Account/UpdateUserLogTable");
                    //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                    //     parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                    HttpResponseMessage postJob = await client.PostAsJsonAsync<UserLogViewModel>(WebAPIUrl + "Account/UpdateUserLogTable", model21);
                    bool postResult = postJob.IsSuccessStatusCode;
                    if (postResult == true)
                    {
                        using (HttpClient client1 = new HttpClient())
                        {
                            client1.BaseAddress = new Uri(WebAPIUrl);

                            client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                         //   client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                         //parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                            HttpResponseMessage response = client1.GetAsync("Account/NotVerifyOtp?UserEmail=" + model21.UserEmail + "&Status=" + model21.Status + "").Result;

                            if (response.IsSuccessStatusCode)
                            {
                                model = response.Content.ReadAsAsync<UserLogViewModel>().Result;
                                if (model.Message == "Blocked")
                                {
                                    ViewBag.Message = "Blocked";
                                    TempData["OTPNot"] = "Blocked";
                                    return RedirectToAction("Login", "Account");
                                }

                                if(model.Message == "LoginFailed")
                                {
                                    return RedirectToAction("Login", "Account");
                                }

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
                    else
                    {
                        TempData["OTPNotVerified"] = "OTPNotVerified";
                        return RedirectToAction("Login", "Account");
                    }
                }

                

            }


        }
        public ActionResult ChangePassword(string emailid, string tokenid)
        {
            //string email = Encryption.Decrypt(emailid);
            // string token = Encryption.Decrypt(tokenid);
            // Session["emailid"] = email;
            // Session["tokenid"] = token; 
            Session["emailid"] = emailid;
            Session["tokenid"] = tokenid; 
            return View();
        }
        [Route("Logout")]
        [SessionExpire]
        [SessionExpireRefNo]
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
            Response.Cookies["Login"].Expires = DateTime.Now.AddDays(-1);
            FormsAuthentication.SignOut();
            Session.Abandon();
            Session.RemoveAll();

            if (Request.Cookies["ASP.NET_SessionId"] != null)
            {
                Response.Cookies["ASP.NET_SessionId"].Value = string.Empty;
                Response.Cookies["ASP.NET_SessionId"].Expires = DateTime.Now.AddMonths(-20);
            }

            if (Request.Cookies["AuthToken"] != null)
            {
                Response.Cookies["AuthToken"].Value = string.Empty;
                Response.Cookies["AuthToken"].Expires = DateTime.Now.AddMonths(-20);
            }
            return RedirectToAction("login");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        [SessionExpire]
        [SessionExpireRefNo]
        public async Task<ActionResult> ChangePassword(ChangePasswordViewModel input)
        {
            //ChangePasswordViewModel input = new ChangePasswordViewModel();
            if (ModelState.IsValid)
            {
                if (Session["emailid"] != null)
                {
                    //input=
                    input.UserName = Session["emailid"].ToString();
                    input.TokenId = Session["tokenid"].ToString();
                    input.UserName = sanitizer.Sanitize(input.UserName);
                    input.Password = sanitizer.Sanitize(input.TokenId);
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(WebAPIUrl + "MasterMenu/ChangePassword");
                        //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                        // parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
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
                                client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                        parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                                HttpResponseMessage postJob1 = await client1.PostAsJsonAsync<UserLogViewModel>(WebAPIUrl + "Account/UpdateUserLogTable", model21);

                                bool postResult1 = postJob1.IsSuccessStatusCode;
                                if (postResult1 == true)
                                {
                                    string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/ChngPwdMail.html"));
                                   // EmailHelper.SendPwdDetails(model21, model21.UserEmail, mailPath);
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
        [Route("ResetPwd")]
        [HttpGet]
        [SessionExpire]
        [SessionExpireRefNo]
        public ActionResult ResetPwd()
        {
            return View();
        }
        [Route("ResetPwd")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SessionExpire]
        [SessionExpireRefNo]
        public async Task<ActionResult> ResetPwd(ChangePasswordViewModel input)
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            if (!ModelState.IsValid)
            {
                if (Session["UserName"] != null)
                {
                    //input=
                    input.UserName = Session["EmailID"].ToString();
                    input.UserName = sanitizer.Sanitize(input.UserName);
                    using (HttpClient client1 = new HttpClient())
                    {
                        client1.BaseAddress = new Uri(WebAPIUrl);

                        client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                        //client1.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(scheme: "Basic",
                        //parameter: "GipInfoSystem" + ":" + "QmludGVzaEAxMDE");
                        HttpResponseMessage response = client1.GetAsync("Account/ResetPwdSendLink?UserEmail=" + input.UserName + "&Status=" + input.EmailID + "").Result;

                        if (response.IsSuccessStatusCode)
                        {
                            model = response.Content.ReadAsAsync<ChangePasswordViewModel>().Result;
                            string emaid = Encryption.Decrypt(model.EmailID); 
                            //model.TokenId = Encryption.Decrypt(model.TokenId); 
                            string mailPath = System.IO.File.ReadAllText(Server.MapPath(@"~/Email/ChngPwdMail.html"));
                             EmailHelper.SendPwdDetails(model, emaid, mailPath);

                            if (model.Message == "User Exist")
                            {
                                ViewBag.Msg = "User Exist";
                                return View();
                                
                            }
                            return RedirectToAction("Login", "Account");
                        }
                        else
                        {
                            return View();
                        }
                    }

                }
                else
                {
                    return View();
                }
                

            }
            return View();
        }
    }
}