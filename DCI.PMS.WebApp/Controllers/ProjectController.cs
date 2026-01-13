using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using DCI.PMS.Models.ViewModel;
using DCI.PMS.WebApp.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
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

        //public IActionResult CreateEdit()
        //{
        //    return View();
        //}


        public async Task<IActionResult> CreateEdit(int id)
        {
            try
            {
                ProjectViewModel model = new ProjectViewModel();
               
                    using (var _httpclient = new HttpClient())
                    {

                        model.ProjectCreationId = id;
                        var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                        var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/GetProjectById");
                        request.Content = stringContent;
                        var response = await _httpclient.SendAsync(request);
                        var responseBody = await response.Content.ReadAsStringAsync();
                        ProjectViewModel vm = JsonConvert.DeserializeObject<ProjectViewModel>(responseBody)!;

                        if (response.IsSuccessStatusCode)
                        {
                            //return Json(new { success = true, data = vm });
                            return View(vm);
                        }
                    return View(vm);
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
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        public async Task<IActionResult> SaveProjectMilestone(ProjectViewModel model)
        {
            try
            {       
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");
                    // model.CurrentUserId = currentUser.UserId;
                  //  model.ProjectCreationId = ProjectCreationId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/SaveProject");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
           
                }



                using (var _httpclient = new HttpClient())
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/GetMilestoneByProjectId");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    model = JsonConvert.DeserializeObject<ProjectViewModel>(responseBody)!;

                    if (response.IsSuccessStatusCode)
                    {
                        return View(model);
                    }
                    return View(model);
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
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        public async Task<IActionResult> AddMilestone(MilestoneViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");
                  

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/SaveMilestone");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                
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
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        public async Task<IActionResult> Deliverables(MilestoneViewModel model)
        {
         
            using (var _httpclient = new HttpClient())
            {
                var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/GetDeliverablesByMilestoneId");
                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<MilestoneViewModel>(responseBody)!;

                if (response.IsSuccessStatusCode)
                {
                    return View(model);
                }
                return View(model);
            }
        }

        public async Task<IActionResult> AddDeliverable(DeliverableViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");


                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiPMS + "api/Project/SaveDeliverable");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
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
            return Json(new { success = false, message = "An error occurred. Please try again." });
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
                Log.Error(ex.ToString());
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
