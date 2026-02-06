using DCI.Models.Configuration;
using DCI.PMS.Models.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Text;

namespace DCI.PMS.WebApp.Controllers
{
    public class TrackerController : Controller
    {
        private readonly IOptions<APIConfigModel> _apiconfig;
     //   private readonly UserSessionHelper _userSessionHelper;

        public TrackerController(IOptions<APIConfigModel> apiconfig)
        {
            this._apiconfig = apiconfig;
            
        }

        public  async Task<IActionResult> Index()
        {
            try
            {

                TrackerViewModel model = new TrackerViewModel();
            

                using (var _httpclient = new HttpClient())
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Tracker/GetAllTracker");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                   

                    if (response.IsSuccessStatusCode)
                    {

                        List<TrackerViewModel> list = JsonConvert.DeserializeObject<List<TrackerViewModel>>(responseBody)!;
                        return View(list);
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

        public IActionResult Calendar()
        {
            return View();
        }
    }
}
