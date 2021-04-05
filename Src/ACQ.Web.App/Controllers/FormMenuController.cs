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
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        [HandleError]
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
                        client.BaseAddress = new Uri(WebAPIUrl + "MasterMenu/AddFormMenu");
                        HttpResponseMessage postJob = await client.PostAsJsonAsync<AddFormMenuViewModel>(WebAPIUrl + "MasterFormMenu/AddFormMenu", input);
                       
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
   
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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
        [ValidateAntiForgeryToken]
        [HandleError]
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
        public ActionResult GetRoleList( int roleId)
        {
            AddFormMenuViewModel model = new AddFormMenuViewModel();
            List<roleViewModel> listData = new List<roleViewModel>();
            using (HttpClient client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri(WebAPIUrl);

                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client1.GetAsync("MasterFormMenu/GetRoleById?UserID=" + roleId +"").Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        model = response.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                        listData = model.roleList;
                        //IEnumerable<SelectListItem> subcategoriesData1 = listData.Select(m => new SelectListItem()
                        //{
                        //    Text = m.FormName.ToString(),
                        //    Value = m.FormMenuID.ToString(),
                        //});
                        return Json(listData, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else
                {

                }
            }
            return View();
        }

        [HttpGet]
        public ActionResult GetSideMenuBar()
        {
            AddFormMenuViewModel model = new AddFormMenuViewModel();
            List<AddFormMenuViewModel> listData = new List<AddFormMenuViewModel>();
            List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
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
                        foreach(var par in model.menuList)
                       {
                            
                            if (par.MenuId == 0)
                            {
                                AddFormMenuViewModel mod = new AddFormMenuViewModel();
                                mod.ID = par.ID;
                                mod.From_menu = par.From_menu;
                                mod.ActionName = par.ActionName;
                                mod.Controller = par.Controller;
                                mod.MenuId = par.MenuId;
                                mod.IsActive = par.IsActive;

                                listData.Add(mod);
                            
                            }
                            else if (par.MenuId != 0)
                            {
                                AddFormMenuViewModel mod1 = new AddFormMenuViewModel();
                                mod1.ID = par.ID;
                                mod1.From_menu = par.From_menu;
                                mod1.ActionName = par.ActionName;
                                mod1.Controller = par.Controller;
                                mod1.MenuId = par.MenuId;
                                mod1.IsActive = par.IsActive;

                                listData1.Add(mod1);
                                
                            }
                        }
                        model.parentList = listData;
                        model.chidList = listData1;
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else
                {

                }
            }
           // return View(model);
            return PartialView(model);
        }

        [HttpGet]
        public ActionResult GetMenuListJSON()
        {
            AddFormMenuViewModel model = new AddFormMenuViewModel();
            List<AddFormMenuViewModel> listData = new List<AddFormMenuViewModel>();
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
                        listData = model.menuList;
                        return Json(listData, JsonRequestBehavior.AllowGet);
                    }
                    catch (Exception ex)
                    {

                    }

                }
                else
                {

                }
            }
            return View();
        }
    }
}