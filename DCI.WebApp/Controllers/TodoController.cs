using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using DCI.WebApp.Configuration;
using Microsoft.AspNetCore.Mvc;
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

		public async Task<IActionResult> Todo(ApprovalHistoryViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					//var currentUser = _userSessionHelper.GetCurrentUser();
					//model.ModifiedBy = currentUser.UserId;

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Todo/GetTodoByApproverId");

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


		public async Task<IActionResult> Index()
		{
			List<DocumentViewModel> model = new List<DocumentViewModel>();
		
			using (var _httpclient = new HttpClient())
			{
				DocumentViewModel _filterRoleModel = new DocumentViewModel();

				var currentUser = _userSessionHelper.GetCurrentUser();
				_filterRoleModel.RoleId = currentUser.RoleId;

				var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
				var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/GetAllDocument");

				request.Content = stringContent;
				var response = await _httpclient.SendAsync(request);
				var responseBody = await response.Content.ReadAsStringAsync();
				if (response.IsSuccessStatusCode)
				{
					model = JsonConvert.DeserializeObject<List<DocumentViewModel>>(responseBody)!;
				}

			}
			return View(model);
		}
	}
}
