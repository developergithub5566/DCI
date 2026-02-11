using DCI.Models.Configuration;
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

                DashboardViewModel model = new DashboardViewModel();
              
                using (var _httpclient = new HttpClient())
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Home/GetDashboard");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<DashboardViewModel>(responseBody)!;

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
       
    }
}
