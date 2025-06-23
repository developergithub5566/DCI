using DCI.WebApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Facebook;
using DCI.WebApp.Configuration;
using DCI.Models.ViewModel;
using Newtonsoft.Json;
using DCI.Models.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.Rendering;
using Serilog;
using System.Text;
using Microsoft.AspNetCore.Authorization;


namespace DCI.WebApp.Controllers
{

    public class HomeController : Controller
    {
        private readonly IOptions<APIConfigModel> _apiconfig;
        private readonly UserSessionHelper _userSessionHelper;

        public HomeController(IOptions<APIConfigModel> apiconfig, UserSessionHelper userSessionHelper)
        {
            this._apiconfig = apiconfig;
            _userSessionHelper = userSessionHelper;
        }

        public async Task<IActionResult> Index(DashboardViewModel model)
        {
            //DashboardViewModel model = new DashboardViewModel();

            using (var _httpclient = new HttpClient())
            {
                var currentUser = _userSessionHelper.GetCurrentUser();
                model.CurrentUserId = currentUser.UserId;
          

                var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Home/GetAllAnnouncement");
                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode == true)
                {
                    model = JsonConvert.DeserializeObject<DashboardViewModel>(responseBody)!;
                    return View(model);
                }           
            }
            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Profile()
        {
            UserViewModel model = new UserViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                    {
                        return RedirectToAction("Logout", "Account");
                    }
                    model.UserId = currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/GetUserRoleListById");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    UserViewModel vm = JsonConvert.DeserializeObject<UserViewModel>(responseBody)!;

                    if (response.IsSuccessStatusCode)
                    {
                        return View(vm);
                    }
                    return Json(new { success = false, message = responseBody });
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                Log.CloseAndFlush();
            }

        }

        public async Task<IActionResult> ChangePass()
        {
            return View();
        }

        public async Task<IActionResult> Notification()
        {
            List<NotificationViewModel> model = new List<NotificationViewModel>();

            using (var _httpclient = new HttpClient())
            {
                NotificationViewModel _filterRoleModel = new NotificationViewModel();
                var currentUser = _userSessionHelper.GetCurrentUser();
                if (currentUser == null)
                {
                    return RedirectToAction("Logout", "Account");
                }
                _filterRoleModel.AssignId = currentUser.UserId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Home/GetAllNotification");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<NotificationViewModel>>(responseBody)!;
                }
            }
            return View(model);
        }



    }
}
