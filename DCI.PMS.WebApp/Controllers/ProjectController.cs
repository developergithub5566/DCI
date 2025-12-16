using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using DCI.PMS.Models.ViewModel;
using DCI.PMS.WebApp.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace DCI.PMS.WebApp.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IOptions<APIConfigModel> _apiconfig;
        private readonly UserSessionHelper _userSessionHelper;

        public ProjectController(IOptions<APIConfigModel> apiconfig, UserSessionHelper userSessionHelper)
        {
            this._apiconfig = apiconfig;
            this._userSessionHelper = userSessionHelper;
        }
      
        public IActionResult CreateEdit()
        {
            return View();
        }

      
        public IActionResult Milestone(ProjectViewModel model)
        {
            return View();
        }

        public IActionResult Deliverables()
        {
            return View();
        }

        public async Task<IActionResult> Index(ProjectViewModel model)
        {
            try
            {
                List<ProjectViewModel> list = new List<ProjectViewModel>();
                using (var _httpclient = new HttpClient())
                {
                    
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/GetAllProject");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        list = JsonConvert.DeserializeObject<List<ProjectViewModel>>(responseBody)!;
                    }
                    return View(list);
                }
            }
            catch (Exception ex)
            {
               // Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.Message });
            }
            finally
            {
                //Log.CloseAndFlush();
            }
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }
    }
}
