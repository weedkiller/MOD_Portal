using ACQ.Web.ViewModel.FormMenu;
using Microsoft.Office.Interop.Word;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace ACQ.Web.App.Controllers
{
    public class FormMenuController : Controller
    {
        private static string WebAPIUrl = ConfigurationManager.AppSettings["APIUrl"].ToString();
        private object sanitizer;

        // GET: FormMenu
        [HttpGet]
        public async Task<ActionResult> AddFormMenu()
        {
            AddFormMenuViewModel model = new AddFormMenuViewModel();
            //List<AddFormMenuViewModel> listData = new List<AddFormMenuViewModel>();
            using (HttpClient client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri(WebAPIUrl);

                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client1.GetAsync("MasterFormMenu/GetFormMenuList").Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        model = response.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else
                {
                    
                }
            }
            return View(model);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[SessionExpire]
        public async Task<ActionResult> AddFormMenu(AddFormMenuViewModel input)
        {



            if (ModelState.IsValid)
            {
                //if (Session["emailid"] != null)
                //{
                    //input.UserName = Session["emailid"].ToString();
                    //input.TokenId = Session["tokenid"].ToString();
                   // input.UserName = sanitizer.Sanitize(input.UserName);
                   // input.Password = sanitizer.Sanitize(input.TokenId);
                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri(WebAPIUrl + "MasterMenu/ChangePassword");
                        HttpResponseMessage postJob = await client.PostAsJsonAsync<AddFormMenuViewModel>(WebAPIUrl + "MasterFormMenu/AddFormMenu", input);
                        //string url = postJob.Headers.Location.AbsoluteUri;
                        //string mID = postJob.Headers.Location.Segments[4].ToString();
                        // string mEmail = postJob.Headers.Location.Segments[5].ToString();
                        bool postResult = postJob.IsSuccessStatusCode;
                        if (postResult == true)
                        {
                        }
                        else
                        {
                            
                            return View();
                        }
                    }
                //}
                //else { Redirect("/Account/Login"); }

            }
            return RedirectToAction("GetFormMenu", "FormMenu");
           // return View();
        }

        [HttpGet]
        public async Task<ActionResult> GetFormMenu()
        {
            AddFormMenuViewModel model = new AddFormMenuViewModel();
            //List<AddFormMenuViewModel> listData = new List<AddFormMenuViewModel>();
            using (HttpClient client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri(WebAPIUrl);

                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client1.GetAsync("MasterFormMenu/GetFormMenuList").Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        model = response.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else
                {

                }
            }
            return View(model);
        }

        [HttpGet]
        public ActionResult AddRole()
        {
            AddFormMenuViewModel model = new AddFormMenuViewModel();
            //UserViewModel model1 = new UserViewModel();
            List<UserViewModel> listData = new List<UserViewModel>();
            using (HttpClient client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri(WebAPIUrl);

                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client1.GetAsync("MasterFormMenu/GetFormMenuList").Result;
                HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetUserList").Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        model = response.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                        listData = response1.Content.ReadAsAsync<List<UserViewModel>>().Result;
                        model.UserList = listData;
                       // List list = (List)model1.UserList;
                       // model.userList = (List<userViewModel>)(List)model1.UserList;
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else
                {

                }
            }
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> AddRole(AddFormMenuViewModel viewModel)
        {
            AddFormMenuViewModel model = new AddFormMenuViewModel();
            if (ModelState.IsValid)
            {
                //if (Session["emailid"] != null)
                //{
                //input.UserName = Session["emailid"].ToString();
                //input.TokenId = Session["tokenid"].ToString();
                // input.UserName = sanitizer.Sanitize(input.UserName);
                // input.Password = sanitizer.Sanitize(input.TokenId);
                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(WebAPIUrl + "MasterMenu/AddRole");
                    HttpResponseMessage postJob = await client.PostAsJsonAsync<AddFormMenuViewModel>(WebAPIUrl + "MasterFormMenu/AddRole", viewModel);
                    //string url = postJob.Headers.Location.AbsoluteUri;
                    //string mID = postJob.Headers.Location.Segments[4].ToString();
                    // string mEmail = postJob.Headers.Location.Segments[5].ToString();
                    bool postResult = postJob.IsSuccessStatusCode;
                    if (postResult == true)
                    {
                        ViewBag.Msg = "Save";
                    }
                    else
                    {

                        return View();
                    }
                }
                //}
                //else { Redirect("/Account/Login"); }

            }
            return View();
        }
    }
}