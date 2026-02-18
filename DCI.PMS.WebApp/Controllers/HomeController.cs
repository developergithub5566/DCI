using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using DCI.PMS.Models.ViewModel;
using DCI.PMS.WebApp.Configuration;
using DCI.PMS.WebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Diagnostics;
using System.Text;

namespace DCI.PMS.WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly IOptions<APIConfigModel> _apiconfig;
        private readonly UserSessionHelper _userSessionHelper;

     
        public HomeController(IOptions<APIConfigModel> apiconfig, UserSessionHelper userSessionHelper)
        {
            this._apiconfig = apiconfig;
            this._userSessionHelper = userSessionHelper;
        }

        public async Task<IActionResult> Index()
        {
            try
            {

                PMSDashboardViewModel model = new PMSDashboardViewModel();
              
                using (var _httpclient = new HttpClient())
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Home/GetDashboard");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<PMSDashboardViewModel>(responseBody)!;

                    if (response.IsSuccessStatusCode)
                    {
                        return View(model);
                    }
                   
                }
                return View(model);
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

        public async Task<IActionResult> Profile()
        {
            UserViewModel model = new UserViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    model.UserId = currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiESS + "api/Maintenance/GetUserRoleListById");
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

        public async Task<IActionResult> Notification()
        {
            try
            {
                List<NotificationViewModel> model = new List<NotificationViewModel>();

                using (var _httpclient = new HttpClient())
                {
                    NotificationViewModel _filterRoleModel = new NotificationViewModel();
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    _filterRoleModel.AssignId = currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Home/GetAllNotification");

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
    }
}
