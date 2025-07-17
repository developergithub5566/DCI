using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using DCI.WebApp.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http;
using System.Text;

namespace DCI.WebApp.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly IOptions<APIConfigModel> _apiconfig;
        private readonly UserSessionHelper _userSessionHelper;
        public EmployeeController(IOptions<APIConfigModel> apiconfig, UserSessionHelper userSessionHelper)
        {
            this._apiconfig = apiconfig;
            this._userSessionHelper = userSessionHelper;
        }

        public async Task<IActionResult> Index()
        {
            List<Form201ViewModel> model = new List<Form201ViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Employee/GetAllEmployee");
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<Form201ViewModel>>(responseBody)!;
                    }
                }         

                return View(model);

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return View(model);
        }



        public async Task<IActionResult> Form201(Form201ViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    model.EmployeeId = currentUser.EmployeeId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Employee/GetEmployeeById");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Form201ViewModel vm = JsonConvert.DeserializeObject<Form201ViewModel>(responseBody)!;
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

        public async Task<IActionResult> SaveEmployee(Form201ViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {   
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Employee/SaveEmployee");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                
                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = responseBody });
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
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        public async Task<IActionResult> Update201Form(Form201ViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Employee/Update201Form");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                   
                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = responseBody });
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
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        public async Task<IActionResult> DeleteEmployee(Form201ViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    model.ModifiedBy = currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Employee/DeleteEmployee");

                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    string responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = responseBody });
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

    }
}
