using DCI.Core.Common;
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



		public async Task<IActionResult> Index()
		{
			List<LeaveRequestHeaderViewModel> model = new List<LeaveRequestHeaderViewModel>();

			using (var _httpclient = new HttpClient())
			{
				LeaveViewModel _filterRoleModel = new LeaveViewModel();

				var currentUser = _userSessionHelper.GetCurrentUser();
				//_filterRoleModel = currentUser.RoleId;
				//_filterRoleModel.CurrentUserId = currentUser.UserId;

				var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
				var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/GetAllTodo");

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
		public async Task<IActionResult> Approval(ApprovalHistoryViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.CreatedBy = currentUser.UserId;
					model.ApproverId = currentUser.UserId;
			
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/Approval");

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
            List<LeaveRequestHeaderViewModel> model = new List<LeaveRequestHeaderViewModel>();

            using (var _httpclient = new HttpClient())
            {
                LeaveViewModel _filterModel = new LeaveViewModel();

                var currentUser = _userSessionHelper.GetCurrentUser();
                //_filterRoleModel = currentUser.RoleId;
                _filterModel.CurrentUserId = currentUser.UserId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/GetApprovalLog");

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
    }
}
