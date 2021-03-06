using ACQ.Web.ExternalServices.Email;
using ACQ.Web.ExternalServices.SecurityAudit;
using ACQ.Web.ViewModel.FormMenu;
using ACQ.Web.ViewModel.User;
using Ganss.XSS;
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
using ACQ.Web.ViewModel.MasterRole;

namespace ACQ.Web.App.Controllers
{
    public class FormMenuController : Controller
    {
        private static string WebAPIUrl = ConfigurationManager.AppSettings["APIUrl"].ToString();
        HtmlSanitizer sanitizer = new HtmlSanitizer();


        public FormMenuController()
        {
            if (BruteForceAttackss.bcontroller != "")
            {
                if (BruteForceAttackss.bcontroller == "FormMenu")
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
                    BruteForceAttackss.bcontroller = "FormMenu";
                }
            }
            else
            {
                BruteForceAttackss.bcontroller = "FormMenu";
            }
        }

        // GET: FormMenu
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult> AddFormMenu()
        {
            AddFormMenuViewModel model = new AddFormMenuViewModel();
            if (Session["UserID"] != null)
            {
              
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                //using (HttpClient client1 = new HttpClient())
                //{

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("AddFormMenu");
                    //client1.BaseAddress = new Uri(WebAPIUrl);

                    //client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    //HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    //if (response1.IsSuccessStatusCode)
                   // {
                        try
                        {
                          //  model = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            //if (model.roleList.Count != 0)
                            //{
                                AddFormMenuViewModel model1 = new AddFormMenuViewModel();
                                using (HttpClient client = new HttpClient())
                                {
                                    client.BaseAddress = new Uri(WebAPIUrl);

                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                    HttpResponseMessage response = client.GetAsync("MasterFormMenu/GetFormMenuList").Result;

                                    if (response.IsSuccessStatusCode)
                                    {
                                        try
                                        {
                                            model1 = response.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                                        }
                                        catch (Exception ex)
                                        {

                                        }

                                    }
                                    else
                                    {

                                    }
                                }
                                
                                return View(model1);
                            //}
                            //else
                            //{
                            //    return RedirectToAction("Login", "Account");
                            //}


                        }
                        catch (Exception ex)
                        {

                        }

                    //}
                    //else
                    //{
                    //    return RedirectToAction("Login", "Account");
                    //}
                //}
                // return View();
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
        [Route("AddRole")]
        public ActionResult AddRole()
        {
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model1 = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
                using (HttpClient client1 = new HttpClient())
                {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("AddRole");
                    client1.BaseAddress = new Uri(WebAPIUrl);

                    //client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    //HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    //if (response1.IsSuccessStatusCode)
                    // {
                    try
                    {
                        // model1 = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                        // if (model1.roleList.Count != 0)
                        // {
                        AddFormMenuViewModel model = new AddFormMenuViewModel();
                        List<UserViewModel> listData = new List<UserViewModel>();
                        using (HttpClient client = new HttpClient())
                        {
                            client.BaseAddress = new Uri(WebAPIUrl);

                            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                            HttpResponseMessage response = client.GetAsync("MasterFormMenu/GetFormMenuList").Result;
                            HttpResponseMessage response2 = client.GetAsync("MasterFormMenu/GetUserList").Result;

                            if (response.IsSuccessStatusCode)
                            {
                                try
                                {
                                    model = response.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                                    listData = response2.Content.ReadAsAsync<List<UserViewModel>>().Result;
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
                        //}
                        //else
                        //{
                        //    return RedirectToAction("Login", "Account");
                        //}
                    }
                    catch (Exception ex)
                    {

                    }
                //}
                    
                   
                }
                // return View();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [HandleError]
        [Route("AddRole")]
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
                    viewModel.CreatedBy = Convert.ToInt32(Session["UserID"]);
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
            if (Session["UserID"] != null)
            {
                AddFormMenuViewModel model1 = new AddFormMenuViewModel();
                List<AddFormMenuViewModel> listData1 = new List<AddFormMenuViewModel>();
               // using (HttpClient client1 = new HttpClient())
               // {

                    var loginid = sanitizer.Sanitize(Session["UserID"].ToString());
                    var formName = sanitizer.Sanitize("SoCPdfRegistration");
                    //client1.BaseAddress = new Uri(WebAPIUrl);

                    //client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                    //HttpResponseMessage response1 = client1.GetAsync("MasterFormMenu/GetRoleByIdUrl?UserID=" + loginid + "&FormName=" + formName + "").Result;
                    //if (response1.IsSuccessStatusCode)
                   // {
                        try
                        {
                            //model1 = response1.Content.ReadAsAsync<AddFormMenuViewModel>().Result;
                            if (model1.roleList.Count != 0)
                            {
                                AddFormMenuViewModel model = new AddFormMenuViewModel();
                                using (HttpClient client = new HttpClient())
                                {
                                    client.BaseAddress = new Uri(WebAPIUrl);

                                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                                    HttpResponseMessage response = client.GetAsync("MasterFormMenu/GetFormMenuList").Result;

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
                            else
                            {
                                return RedirectToAction("Login", "Account");
                            }


                        }
                        catch (Exception ex)
                        {

                        }

                    //}
                    //else
                    //{
                    //    return RedirectToAction("Login", "Account");
                    //}
                }
                // return View();
           // }

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
        [Route("ViewUserRights")]
        public ActionResult ViewUserRights()
        {
            using (HttpClient client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri(WebAPIUrl);

                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client1.GetAsync("MasterFormMenu/GetMenuList").Result;

                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        ViewBag.MenuList = response.Content.ReadAsAsync<List<AddFormMenuViewModel>>().Result;
                        
                    }
                    catch (Exception ex)
                    {

                    }

                }
            }
                return View();
        }

        [HttpPost]
        [Route("ViewUserRights")]
        public ActionResult ViewUserRights(string MenuItem)
        {
            ViewBag.SelectedMenuItem = MenuItem;
            using (HttpClient client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri(WebAPIUrl);
                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client1.GetAsync("MasterFormMenu/GetAssignedMenuUsers?menuId="+MenuItem).Result;
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        ViewBag.UserList = response.Content.ReadAsAsync<List<UserViewModel>>().Result;

                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
            using (HttpClient client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri(WebAPIUrl);
                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client1.GetAsync("MasterFormMenu/GetMenuList").Result;
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        ViewBag.MenuList = response.Content.ReadAsAsync<List<AddFormMenuViewModel>>().Result;

                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return View();
        }
        [HttpGet]
        [Route("RolesHistory")]
        public ActionResult RolesHistory(string UserId)
        {
            string uId = Encryption.Decrypt(UserId);
            using (HttpClient client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri(WebAPIUrl);
                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client1.GetAsync("MasterFormMenu/GetUserRolesHistory?userId="+uId).Result;
                if (response.IsSuccessStatusCode)
                {
                    try
                    {
                        ViewBag.UserRoleList = response.Content.ReadAsAsync<List<tbl_Master_Role>>().Result;
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            return View();
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
        [HttpGet]
        public ActionResult GetRoleList(int roleId)
        {
            AddFormMenuViewModel model = new AddFormMenuViewModel();
            List<roleViewModel> listData = new List<roleViewModel>();
            using (HttpClient client1 = new HttpClient())
            {
                client1.BaseAddress = new Uri(WebAPIUrl);

                client1.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType: "application/json"));
                HttpResponseMessage response = client1.GetAsync("MasterFormMenu/GetRoleById?UserID=" + roleId + "").Result;

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
    }
}