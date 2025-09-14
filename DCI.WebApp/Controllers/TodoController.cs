using DCI.Core.Common;
using DCI.Core.Helpers;
using DCI.Models.Configuration;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.WebApp.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Text;

namespace DCI.WebApp.Controllers
{
    public class TodoController : Controller
    {
        private readonly IOptions<APIConfigModel> _apiconfig;
        private readonly UserSessionHelper _userSessionHelper;


        public TodoController(IOptions<APIConfigModel> apiconfig, UserSessionHelper userSessionHelper)
        {
            _apiconfig = apiconfig;
            _userSessionHelper = userSessionHelper;
        }

        //public async Task<IActionResult> Todo(ApprovalHistoryViewModel model)
        //{
        //	try
        //	{
        //		using (var _httpclient = new HttpClient())
        //		{
        //			//var currentUser = _userSessionHelper.GetCurrentUser();
        //			//model.ModifiedBy = currentUser.UserId;

        //			var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
        //			var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/GetTodoByApproverId");

        //			request.Content = stringContent;
        //			var response = await _httpclient.SendAsync(request);
        //			var responseBody = await response.Content.ReadAsStringAsync();
        //			if (response.IsSuccessStatusCode)
        //			{
        //				return Json(new { success = true, message = responseBody });
        //			}
        //			return Json(new { success = false, message = responseBody });
        //		}
        //	}
        //	catch (Exception ex)
        //	{
        //		Log.Error(ex.ToString());
        //		return Json(new { success = false, message = ex.Message });
        //	}
        //	finally
        //	{
        //		Log.CloseAndFlush();
        //	}
        //}



        public async Task<IActionResult> Leave()
        {
            List<LeaveRequestHeaderViewModel> model = new List<LeaveRequestHeaderViewModel>();

            using (var _httpclient = new HttpClient())
            {
                LeaveViewModel _filterRoleModel = new LeaveViewModel();

                var currentUser = _userSessionHelper.GetCurrentUser();
                //_filterRoleModel = currentUser.RoleId;
                //_filterRoleModel.CurrentUserId = currentUser.UserId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/GetAllTodoLeave");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<LeaveRequestHeaderViewModel>>(responseBody)!;
                }

            }
            return View(model);
        }

        public async Task<IActionResult> DTR()
        {
            List<DTRCorrectionViewModel> model = new List<DTRCorrectionViewModel>();

            using (var _httpclient = new HttpClient())
            {
                DTRCorrectionViewModel _filterRoleModel = new DTRCorrectionViewModel();

                var currentUser = _userSessionHelper.GetCurrentUser();
                _filterRoleModel.CurrentUserId = currentUser.UserId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/GetAllTodoDtr");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<DTRCorrectionViewModel>>(responseBody)!;
                }

            }
            return View(model);
        }
        public async Task<IActionResult> ApprovalLeave(ApprovalHistoryViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    model.CreatedBy = currentUser.UserId;
                    model.ApproverId = currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/ApprovalLeave");

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
        }

        public async Task<IActionResult> ApprovalDtr(ApprovalHistoryViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    model.CreatedBy = currentUser.UserId;
                    model.ApproverId = currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/ApprovalDtr");

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
        }

        public async Task<IActionResult> ApprovalLog()
        {
            List<ApprovalHistoryViewModel> model = new List<ApprovalHistoryViewModel>();

            using (var _httpclient = new HttpClient())
            {
                LeaveViewModel _filterModel = new LeaveViewModel();

                var currentUser = _userSessionHelper.GetCurrentUser();
                //_filterRoleModel = currentUser.RoleId;
                _filterModel.CurrentUserId = currentUser.UserId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/GetApprovalHistory");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<ApprovalHistoryViewModel>>(responseBody)!;
                }

            }
            return View(model);
        }

        public async Task<IActionResult> ApprovalWFH(ApprovalHistoryViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    model.CreatedBy = currentUser.UserId;
                    model.ApproverId = currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/ApprovalWFH");

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
        }

        public async Task<IActionResult> ApprovalOvertime(ApprovalHistoryViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    model.CreatedBy = currentUser.UserId;
                    model.ApproverId = currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/ApprovalOvertime");

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
        }


        public async Task<IActionResult> Overtime()
        {
            List<OvertimeViewModel> model = new List<OvertimeViewModel>();

            using (var _httpclient = new HttpClient())
            {
                OvertimeViewModel _filterRoleModel = new OvertimeViewModel();

                var currentUser = _userSessionHelper.GetCurrentUser();
                //_filterRoleModel.CurrentUserId = currentUser.RoleId;
                _filterRoleModel.CurrentUserId = currentUser.UserId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/GetAllTodoOvertime");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<OvertimeViewModel>>(responseBody)!;

                    foreach (var x in model)
                    {
                        x.TotalString = TimeHelper.ConvertMinutesToHHMM((int)x.Total);
                    }
                }

            }
            return View(model);
        }

        public async Task<IActionResult> WFH()
        {
            List<WFHHeaderViewModel> model = new List<WFHHeaderViewModel>();

            using (var _httpclient = new HttpClient())
            {
                WFHHeaderViewModel _filterRoleModel = new WFHHeaderViewModel();

                var currentUser = _userSessionHelper.GetCurrentUser();
                _filterRoleModel.EmployeeId = currentUser.EmployeeId;
                _filterRoleModel.CurrentUserId = currentUser.UserId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/GetAllWFH");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<WFHHeaderViewModel>>(responseBody)!;
                }

            }
            return View(model);
        }
    }
}
