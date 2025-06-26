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
    public class DailyTimeRecordController : Controller
    {
        private readonly IOptions<APIConfigModel> _apiconfig;
        private readonly UserSessionHelper _userSessionHelper;

        public DailyTimeRecordController(IOptions<APIConfigModel> apiconfig, UserSessionHelper userSessionHelper)
        {
            this._apiconfig = apiconfig;
            this._userSessionHelper = userSessionHelper;
        }   

        public async Task<IActionResult> List()
        {
            List<DailyTimeRecordViewModel> model = new List<DailyTimeRecordViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllDTR");
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<DailyTimeRecordViewModel>>(responseBody)!;
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

        public async Task<IActionResult> DTR(DailyTimeRecordViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    model.EMPLOYEE_NO = "080280";

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllDTRByEmpNo");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    DailyTimeRecordViewModel vm = JsonConvert.DeserializeObject<DailyTimeRecordViewModel>(responseBody)!;
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

        public async Task<IActionResult> Leave(LeaveViewModel model)
        {
         //   LeaveViewModel model = new LeaveViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();

                    model.EmployeeId = 2;//currentUser.UserId;

                    //model.SLBalance = 0;
                    //model.VLBalance = 0;
                    //  model.LeaveType = 1;
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllLeave");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<LeaveViewModel>(responseBody)!;
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

        public async Task<IActionResult> SaveLeave(LeaveFormViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    model.EmployeeId = 2;
        
                    model.SelectedDateList = JsonConvert.DeserializeObject<List<DateTime>>(model.SelectedDateJson);


                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/SaveLeave");
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

        public async Task<IActionResult> RequestLeave(LeaveViewModel model)
        {
           
            try
            { 
                using (var _httpclient = new HttpClient())
                {
                    model.EmployeeId = 2;
                  //  model.LeaveRequestHeaderId = 4;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/RequestLeave");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<LeaveViewModel>(responseBody)!;

                        model.OptionsLeaveType = model.LeaveRequestHeader.LeaveTypeList.Select(x =>
                                    new SelectListItem
                                    {
                                        Value = x.LeaveTypeId.ToString(),
                                        Text = x.Description
                                    }).ToList();

                        //model.SelectedDateJson = JsonConvert.SerializeObject(model.LeaveDateList);
                       // var dateList = new List<string> { "2025-06-09","2025-06-10" };
                      // // var jsonDates = JsonConvert.SerializeObject(dateList);
                      //  model.SelectedDateJson = jsonDates;
                        //string json = JsonConvert.SerializeObject(model.LeaveDateList);
                    }
                    return Json(new { success = true, data = model });
                }
                return Json(new { success = false, message = "" });
                // return View(model);

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
          //  return View(model);
        }

        public async Task<IActionResult> DTRCorrection()
        {  
            List<DTRCorrectionViewModel> list = new List<DTRCorrectionViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    DTRCorrectionViewModel model = new DTRCorrectionViewModel();
                    var currentUser = _userSessionHelper.GetCurrentUser();

                    model.CreatedBy = 2;//currentUser.UserId;

                   
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllDTRCorrection");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        list = JsonConvert.DeserializeObject<List<DTRCorrectionViewModel>>(responseBody)!;
            
                    }
                }

                return View(list);

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return View(list);
        }

        public async Task<IActionResult> DTRCorrectionById(DTRCorrectionViewModel model)
        {
           // DTRCorrectionViewModel model = new DTRCorrectionViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                  
                   // var currentUser = _userSessionHelper.GetCurrentUser();

                  //  model.CreatedBy = 2;//currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/DTRCorrectionById");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<DTRCorrectionViewModel>(responseBody)!;

                    }
                    return Json(new { success = true, data = model });
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

        public async Task<IActionResult> SaveDTRCorrection(DTRCorrectionViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    model.CreatedBy = 2;                    

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/SaveDTRCorrection");
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

        

    }
}
