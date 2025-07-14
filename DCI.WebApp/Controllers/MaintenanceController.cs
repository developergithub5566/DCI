using DCI.Models.Configuration;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.WebApp.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DCI.WebApp.Controllers
{
	public class MaintenanceController : Controller
	{
		private readonly IOptions<APIConfigModel> _apiconfig;
		private readonly UserSessionHelper _userSessionHelper;
		public MaintenanceController(IOptions<APIConfigModel> apiconfig, UserSessionHelper userSessionHelper)
		{
			this._apiconfig = apiconfig;
			this._userSessionHelper = userSessionHelper;
		}
		public async Task<IActionResult> Index()
		{
			return View();
		}


		#region User
		public async Task<IActionResult> User()
		{
			List<UserModel> model = new List<UserModel>();
			try
			{
				using (var _httpclient = new HttpClient())
				{
					HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllUsers");
					string responseBody = await response.Content.ReadAsStringAsync();

					if (response.IsSuccessStatusCode == true)
					{
						model = JsonConvert.DeserializeObject<List<UserModel>>(responseBody)!;
					}
				}

				//List<UserModel> vm = new List<UserModel>();
				//vm.Options = vm.RoleList.Select(x =>
				//							   new SelectListItem
				//							   {
				//								   Value = x.RoleId.ToString(),
				//								   Text = x.RoleName
				//							   }).ToList();

			
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



		public async Task<IActionResult> EditUser(UserViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/GetUserRoleListById");
					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					var responseBody = await response.Content.ReadAsStringAsync();
					UserModel vm = JsonConvert.DeserializeObject<UserModel>(responseBody)!;

					vm.Options = vm.RoleList?.Select(x =>
												   new SelectListItem
												   {
													   Value = x.RoleId.ToString(),
													   Text = x.RoleName
												   }).ToList();

                    vm.OptionsDepartment = vm.DepartmentList?.Select(x =>
                               new SelectListItem
                               {
                                   Value = x.DepartmentId.ToString(),
                                   Text = x.DepartmentName
                               }).ToList();

                    if (response.IsSuccessStatusCode)
					{
						return Json(new { success = true, data = vm });
					}
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

		public async Task<IActionResult> SaveUser(UserViewModel model)
		{
			try
			{
				var currentUser = _userSessionHelper.GetCurrentUser();
				model.CreatedBy = currentUser.UserId;
				model.ModifiedBy = currentUser.UserId;

				using (var _httpclient = new HttpClient())
				{
					var request = new HttpRequestMessage();
					var stringContent = new StringContent("");
					if (model.UserId == 0)
					{
						stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
						request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Account/Registration");
					}
					else
					{
						stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
						request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/UpdateUser");
					}
					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					var responseBody = await response.Content.ReadAsStringAsync();
					//	UserModel vm = JsonConvert.DeserializeObject<UserModel>(responseBody)!;

					//vm.Options = vm.RoleList.Select(x =>
					//							   new SelectListItem
					//							   {
					//								   Value = x.RoleId.ToString(),
					//								   Text = x.RoleName
					//							   }).ToList();


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


		public async Task<IActionResult> DeleteUser(UserViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/DeleteUser");

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

		#endregion

		#region Role
		public async Task<IActionResult> Role()
		{
			List<RoleViewModel> model = new List<RoleViewModel>();

			using (var _httpclient = new HttpClient())
			{
				HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllRoles");
				string responseBody = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					model = JsonConvert.DeserializeObject<List<RoleViewModel>>(responseBody)!;
				}
			}
			return View(model);
		}
		public async Task<IActionResult> EditRole(RoleViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/GetRoleById");
					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					var responseBody = await response.Content.ReadAsStringAsync();
					RoleViewModel vm = JsonConvert.DeserializeObject<RoleViewModel>(responseBody)!;


					if (response.IsSuccessStatusCode)
					{
						return Json(new { success = true, data = vm });
					}

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

		public async Task<IActionResult> SaveRole(RoleViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.CreatedBy = currentUser.UserId;
					model.ModifiedBy = currentUser.UserId;

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/SaveRole");

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

		public async Task<IActionResult> DeleteRole(RoleViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.ModifiedBy = currentUser.UserId;


					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/DeleteRole");

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
		#endregion

		#region Module In Role
		public async Task<IActionResult> ModulePage()
		{
			List<ModulePage> model = new List<ModulePage>();

			using (var _httpclient = new HttpClient())
			{
				HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllModulePage");
				string responseBody = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode == true)
				{
					model = JsonConvert.DeserializeObject<List<ModulePage>>(responseBody)!;
				}
			}
			return View(model);
		}

		public async Task<IActionResult> SaveModulePage(ModulePageViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.CreatedBy = currentUser.UserId;
					model.ModifiedBy = currentUser.UserId;


					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/SaveModulePage");

					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);

					if (response.IsSuccessStatusCode)
					{
						return Json(new { success = true, message = "Module successfully created." });
					}
				}
				return Json(new { success = false, message = "An error occurred. Please try again." });
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

		public async Task<IActionResult> EditModulePage(ModulePageViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/GetModulePageById");
					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					var responseBody = await response.Content.ReadAsStringAsync();
					ModulePageViewModel vm = JsonConvert.DeserializeObject<ModulePageViewModel>(responseBody)!;

					if (response.IsSuccessStatusCode)
					{
						return Json(new { success = true, data = vm });
					}
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

		public async Task<IActionResult> DeleteModulePage(ModulePageViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.ModifiedBy = currentUser.UserId;

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/DeleteModulePage");

					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);

					if (response.IsSuccessStatusCode)
					{
						return Json(new { success = true, message = "Module page successfully deleted." });
					}
				}
				return Json(new { success = false, message = "An error occurred. Please try again." });
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
		#endregion

		#region Department
		public async Task<IActionResult> Department()
		{
			List<DepartmentViewModel> model = new List<DepartmentViewModel>();

			using (var _httpclient = new HttpClient())
			{
				HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllDepartment");
				string responseBody = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					model = JsonConvert.DeserializeObject<List<DepartmentViewModel>>(responseBody)!;
				}
			}
			return View(model);
		}
		public async Task<IActionResult> EditDepartment(DepartmentViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/GetDepartmentById");
					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					var responseBody = await response.Content.ReadAsStringAsync();
					DepartmentViewModel vm = JsonConvert.DeserializeObject<DepartmentViewModel>(responseBody)!;


					vm.OptionsReviewer = vm.UserList.Select(x =>
				   new SelectListItem
				   {
					   Value = x.UserId.ToString(),
					   Text = x.Lastname + ", " + x.Firstname
				   }).ToList();

					vm.OptionsApprover = vm.UserList.Select(x =>
				   new SelectListItem
				   {
					   Value = x.UserId.ToString(),
					   Text = x.Lastname + ", " + x.Firstname
				   }).ToList();


					if (response.IsSuccessStatusCode)
					{
						return Json(new { success = true, data = vm });
					}
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

		public async Task<IActionResult> SaveDepartment(DepartmentViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.CreatedBy = currentUser.UserId;
					model.ModifiedBy = currentUser.UserId;

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/SaveDepartment");

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

		public async Task<IActionResult> DeleteDepartment(DepartmentViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.ModifiedBy = currentUser.UserId;


					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/DeleteDepartment");

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
		#endregion

		#region Employment Type
		public async Task<IActionResult> EmploymentType()
		{
			List<EmploymentType> model = new List<EmploymentType>();

			using (var _httpclient = new HttpClient())
			{
				HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllEmploymentType");
				string responseBody = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					model = JsonConvert.DeserializeObject<List<EmploymentType>>(responseBody)!;
				}
			}
			return View(model);
		}
		public async Task<IActionResult> EditEmploymentType(EmploymentTypeViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/GetEmploymentTypeById");
					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					var responseBody = await response.Content.ReadAsStringAsync();
					EmploymentTypeViewModel vm = JsonConvert.DeserializeObject<EmploymentTypeViewModel>(responseBody)!;


					if (response.IsSuccessStatusCode)
					{
						return Json(new { success = true, data = vm });
					}
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

		public async Task<IActionResult> SaveEmploymentType(EmploymentTypeViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.CreatedBy = currentUser.UserId;
					model.ModifiedBy = currentUser.UserId;

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/SaveEmploymentType");

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

		public async Task<IActionResult> DeleteEmploymentType(EmploymentTypeViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.ModifiedBy = currentUser.UserId;

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/DeleteEmploymentType");

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
		#endregion

		#region Role Module

		public async Task<IActionResult> RoleModule(int id, string type)
		{
			SystemManagementViewModel model = new SystemManagementViewModel();
			model.RoleId = id;
			
			if (model.RoleId != 0)
			{
				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/GetModuleAccessRoleByRoleId");
					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					var responseBody = await response.Content.ReadAsStringAsync();
		
					model = JsonConvert.DeserializeObject<SystemManagementViewModel>(responseBody)!;
					model.ViewEdit = Convert.ToInt16(type);				
				}
			}
			model.ViewEdit = Convert.ToInt16(type);
			model.DashboardLabel = (id == 0 ? "Add" : (type == "1" ? "View" : "Update"));
			return View(model);
		}

		public async Task<IActionResult> SaveRoleModule([FromBody] Dictionary<string, ModuleJson> inputJson)
		{
			try
			{
				//int _roleId = inputJson.Values != null ?  Convert.ToInt32(inputJson.Values.FirstOrDefault().RoleId) : 0;
				// string _roleName = inputJson.Values != null ? inputJson.Values.FirstOrDefault().RoleName.ToString() : 0;
				// string _desc = inputJson.Values != null ? inputJson.Values.FirstOrDefault().Description.ToString() : 0;
				int _roleId = inputJson.Values?.FirstOrDefault(v => v.RoleId != null)?.RoleId ?? 0;
				string _roleName = inputJson.Values?.FirstOrDefault(v => v.RoleName != null)?.RoleName ?? string.Empty;
				string _desc = inputJson.Values?.FirstOrDefault(v => v.Description != null)?.Description ?? string.Empty;

				var currentUser = _userSessionHelper.GetCurrentUser();

				var moduleCollection = new RoleInModuleViewModel
				{
					Modules = inputJson,
					RoleVM = new RoleViewModel
					{
						RoleId = _roleId,
						RoleName = _roleName,
						Description = _desc,
						CreatedBy = currentUser.UserId,
						DateCreated = DateTime.Now,
						ModifiedBy = currentUser.UserId,
						DateModified = DateTime.Now,
						IsActive = true,
					}
				};

				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(moduleCollection), Encoding.UTF8, "application/json");

					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/SaveRoleModule");

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

		#endregion

		#region User In Role
		public async Task<IActionResult> UserRole()
		{
			List<UserInRoleViewModel> model = new List<UserInRoleViewModel>();

			using (var _httpclient = new HttpClient())
			{
				HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetUserRole");
				string responseBody = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					model = JsonConvert.DeserializeObject<List<UserInRoleViewModel>>(responseBody)!;
				}
			}
			return View(model);
		}

		public async Task<IActionResult> DeleteUserRole(int id)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					UserInRoleViewModel model = new UserInRoleViewModel();
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.ModifiedBy = currentUser.UserId;
					model.RoleId = id;

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/DeleteUserRole");

					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);

					if (response.IsSuccessStatusCode)
					{
						return Json(new { success = true, message = "User Role successfully deleted." });
					}
				}
				return Json(new { success = false, message = "An error occurred. Please try again." });
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
		#endregion

		#region Document Type
		public async Task<IActionResult> DocumentType()
		{
			List<DocumentTypeViewModel> model = new List<DocumentTypeViewModel>();

			using (var _httpclient = new HttpClient())
			{
				HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllDocumentType");
				string responseBody = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					model = JsonConvert.DeserializeObject<List<DocumentTypeViewModel>>(responseBody)!;
				}
			}
			return View(model);
		}
		public async Task<IActionResult> EditDocumentType(DocumentTypeViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/GetDocumentTypeById");
					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					var responseBody = await response.Content.ReadAsStringAsync();
					DocumentTypeViewModel vm = JsonConvert.DeserializeObject<DocumentTypeViewModel>(responseBody)!;


					if (response.IsSuccessStatusCode)
					{
						return Json(new { success = true, data = vm });
					}
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

		public async Task<IActionResult> SaveDocumentType(DocumentTypeViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.CreatedBy = currentUser.UserId;
					model.ModifiedBy = currentUser.UserId;

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/SaveDocumentType");

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

		public async Task<IActionResult> DeleteDocumentType(DocumentTypeViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.ModifiedBy = currentUser.UserId;

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/DeleteDocumentType");

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
		#endregion

		#region Audit Logs


		public async Task<IActionResult> ActivityLog(AuditLogViewModel param)
		{

			List<AuditLogViewModel> auditmodel = new List<AuditLogViewModel>();

			if (param.Id == 0) // all auditlogs from Maintenance
			{
				using (var _httpclient = new HttpClient())
				{
					HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllAuditLogs");
					string responseBody = await response.Content.ReadAsStringAsync();

					if (response.IsSuccessStatusCode)
					{
						auditmodel = JsonConvert.DeserializeObject<List<AuditLogViewModel>>(responseBody)!;
					}
					return View(auditmodel);
				}

			}

			else if (param.Id == -1) // specific user from activity user
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					param.Username = currentUser.UserId.ToString();

					var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/GetAuditLogById");
					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					var responseBody = await response.Content.ReadAsStringAsync();

					auditmodel = JsonConvert.DeserializeObject<List<AuditLogViewModel>>(responseBody)!;

					if (response.IsSuccessStatusCode)
					{
						return View(auditmodel);
					}
				}
			}
			return View(auditmodel);
		}
		#endregion

		#region Section

		public async Task<IActionResult> Section()
		{
			List<SectionViewModel> model = new List<SectionViewModel>();

			using (var _httpclient = new HttpClient())
			{
				HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllSection");
				string responseBody = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					model = JsonConvert.DeserializeObject<List<SectionViewModel>>(responseBody)!;
				}
			}
			return View(model);
		}
		public async Task<IActionResult> EditSection(SectionViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/GetSectionById");
					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					var responseBody = await response.Content.ReadAsStringAsync();
					SectionViewModel vm = JsonConvert.DeserializeObject<SectionViewModel>(responseBody)!;

					vm.OptionsDepartment = vm.DepartmentList.Select(x =>
								   new SelectListItem
								   {
									   Value = x.DepartmentId.ToString(),
									   Text = x.DepartmentName
								   }).ToList();

					if (response.IsSuccessStatusCode)
					{
						return Json(new { success = true, data = vm });
					}
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

		public async Task<IActionResult> SaveSection(SectionViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.CreatedBy = currentUser.UserId;
					model.ModifiedBy = currentUser.UserId;

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/SaveSection");

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

		public async Task<IActionResult> DeleteSection(SectionViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.ModifiedBy = currentUser.UserId;

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/DeleteSection");

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


        #endregion


        #region Announcement
        public async Task<IActionResult> Announcement()
        {
            List<AnnouncementViewModel> model = new List<AnnouncementViewModel>();

            using (var _httpclient = new HttpClient())
            {
                HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllAnnouncement");
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<AnnouncementViewModel>>(responseBody)!;
                }
            }
            return View(model);
        }
        public async Task<IActionResult> EditAnnouncement(AnnouncementViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/GetAnnouncementById");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    AnnouncementViewModel vm = JsonConvert.DeserializeObject<AnnouncementViewModel>(responseBody)!;					                   

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, data = vm });
                    }
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

        public async Task<IActionResult> SaveAnnouncement(AnnouncementViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    model.CreatedBy = currentUser.UserId;
                    model.ModifiedBy = currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/SaveAnnouncement");

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

        public async Task<IActionResult> DeleteAnnouncement(AnnouncementViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    model.ModifiedBy = currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/DeleteAnnouncement");

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
        #endregion

        #region User Employee

        public async Task<IActionResult> EmployeeList()
        {
            List<Form201ViewModel> model = new List<Form201ViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllEmployee");
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

        public async Task<IActionResult> UserEmployee()
        {
            List<UserModel> model = new List<UserModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllUsers");
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<UserModel>>(responseBody)!;
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
        public async Task<IActionResult> EditUserEmployee(UserViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/GetUserEmployeeRoleListById");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    UserModel vm = JsonConvert.DeserializeObject<UserModel>(responseBody)!;

                    vm.Options = vm.RoleList?.Select(x =>
                                                   new SelectListItem
                                                   {
                                                       Value = x.RoleId.ToString(),
                                                       Text = x.RoleName
                                                   }).ToList();

                    vm.OptionsForm201List = vm.EmployeeDropdownList?.Select(x =>
                               new SelectListItem
                               {
                                   Value = x.EmployeeId.ToString(),
                                   Text = x.Display
                               }).ToList();

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, data = vm });
                    }
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

        public async Task<IActionResult> SaveUserEmployee(UserViewModel model)
        {
            try
            {
                var currentUser = _userSessionHelper.GetCurrentUser();
                model.CreatedBy = currentUser.UserId;
                model.ModifiedBy = currentUser.UserId;
                
                using (var _httpclient = new HttpClient())
                {
                    //var request = new HttpRequestMessage();
                    //var stringContent = new StringContent("");
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Maintenance/CreateUserAccount");

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


        public async Task<IActionResult> Form201(Form201ViewModel model)
        {
            try
            {
				Form201ViewModel vm = new Form201ViewModel();
                using (var _httpclient = new HttpClient())
                {
                  //  model.EmployeeId = 1335;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Employee/GetEmployeeById");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        model.OptionsEmployeeStatus = model.EmployeeStatusList.Select(x =>
                                 new SelectListItem
                                 {
                                     Value = x.EmployeeStatusId.ToString(),
                                     Text = x.EmployeeStatusName
                                 }).ToList();


                        vm =  JsonConvert.DeserializeObject<Form201ViewModel>(responseBody)!;
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
        #endregion
    }
}
