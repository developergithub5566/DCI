using Aspose.Words.Drawing;
using Aspose.Words;
using DCI.Models.Configuration;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.WebApp.Configuration;
using DCI.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Metadata;
using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2010.Excel;
using GroupDocs.Viewer.Options;
using Microsoft.AspNetCore.StaticFiles;


namespace DCI.WebApp.Controllers
{
	public class DocumentController : Controller
	{
		private readonly IOptions<APIConfigModel> _apiconfig;
		private readonly UserSessionHelper _userSessionHelper;
		private readonly DocumentService _documentService;
		public DocumentController(IOptions<APIConfigModel> apiconfig, UserSessionHelper userSessionHelper, DocumentService documentService)
		{
			this._apiconfig = apiconfig;
			this._userSessionHelper = userSessionHelper;
			this._documentService = documentService;
		}
		public async Task<IActionResult> Index()
		{
			List<DocumentViewModel> model = new List<DocumentViewModel>();

			//using (var _httpclient = new HttpClient())
			//{
			//	HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Document/GetAllDocument");
			//	string responseBody = await response.Content.ReadAsStringAsync();

			//	if (response.IsSuccessStatusCode)
			//	{
			//		model = JsonConvert.DeserializeObject<List<DocumentViewModel>>(responseBody)!;					
			//	}
			//}
			//return View(model);
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

		public async Task<IActionResult> EditDocument(DocumentViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/GetDocumentById");
					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					var responseBody = await response.Content.ReadAsStringAsync();
					DocumentViewModel vm = JsonConvert.DeserializeObject<DocumentViewModel>(responseBody)!;

					vm.OptionsDocumentType = vm.DocumentTypeList.Select(x =>
									   new SelectListItem
									   {
										   Value = x.DocTypeId.ToString(),
										   Text = x.Name
									   }).ToList();

					vm.OptionsDepartment = vm.DepartmentList.Select(x =>
									   new SelectListItem
									   {
										   Value = x.DepartmentId.ToString(),
										   Text = x.DepartmentName
									   }).ToList();

					//vm.OptionsSection = vm.SectionList.Select(x =>
					//				   new SelectListItem
					//				   {
					//					   Value = x.SectionId.ToString(),
					//					   Text = x.SectionName
					//				   }).ToList();

					vm.OptionsRequestBy = vm.UserList.Select(x =>
					   new SelectListItem
					   {
						   Value = x.UserId.ToString(),
						   Text = x.Lastname + ", " + x.Firstname
					   }).ToList();

					if (response.IsSuccessStatusCode)
					{
						return Json(new { success = true, data = vm });
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

		public async Task<IActionResult> SaveDocument(DocumentViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.CreatedBy = currentUser.UserId;
					model.ModifiedBy = currentUser.UserId;


					//_httpclient.BaseAddress = new Uri(_apiconfig.Value.apiConnection + "api/Document/SaveDocument");

					var data = new MultipartFormDataContent();
					data.Add(new StringContent(model.DocId.ToString() ?? ""), "DocId");
					data.Add(new StringContent(model.DocNo ?? ""), "DocNo");
					data.Add(new StringContent(model.DocName ?? ""), "DocName");
					data.Add(new StringContent(model.DocTypeId.ToString() ?? ""), "DocTypeId");
					data.Add(new StringContent(model.Version.ToString() ?? ""), "Version");

					data.Add(new StringContent(model.DepartmentId.ToString() ?? ""), "DepartmentId");
					data.Add(new StringContent(model.DocCategory.ToString() ?? ""), "DocCategory");
					//data.Add(new StringContent(model.Section.ToString() ?? ""), "Section");
					data.Add(new StringContent(model.FormsProcess.ToString() ?? ""), "LabelId");
					data.Add(new StringContent(model.StatusId.ToString() ?? ""), "StatusId");
					data.Add(new StringContent(model.Reviewer.ToString() ?? ""), "Reviewer");
					data.Add(new StringContent(model.Approver.ToString() ?? ""), "Approver");
					data.Add(new StringContent(model.RequestById.ToString() ?? ""), "RequestById");
					data.Add(new StringContent(model.SectionId.ToString() ?? ""), "SectionId");

					data.Add(new StringContent(model.CreatedName.ToString() ?? ""), "CreatedName");
					data.Add(new StringContent(model.DateCreated.ToString() ?? ""), "DateCreated");
					data.Add(new StringContent(model.CreatedBy.ToString() ?? ""), "CreatedBy");
					data.Add(new StringContent(model.ModifiedBy.ToString() ?? ""), "ModifiedBy");
					data.Add(new StringContent(model.DateModified.ToString() ?? ""), "DateModified");
					data.Add(new StringContent(model.IsActive.ToString() ?? ""), "IsActive");

					if (model.DocFile != null)
					{
						var fileContent = new StreamContent(model.DocFile!.OpenReadStream());
						data.Add(fileContent, "DocFile", model.DocFile.FileName);
					}

					var response = await _httpclient.PostAsync(_apiconfig.Value.apiConnection + "api/Document/SaveDocument", data);
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
				Log.Error(ex.Message.ToString());
				return Json(new { success = false, message = ex.Message });
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public async Task<IActionResult> DeleteDocument(DocumentViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.ModifiedBy = currentUser.UserId;

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/DeleteDocument");

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


		public async Task<IActionResult> ViewDocument(DocumentViewModel model)
		{
			//string filePath = @"C:\\DCI File\\Output\dci.txt"; 		
			//var pdfFilePath = _documentService.ConvertWordToPdf(filePath);

			//var pdfBytes = await System.IO.File.ReadAllBytesAsync(pdfFilePath);

			//Response.Headers.Add("Content-Disposition", "inline; filename=dci.txt");

			//return File(pdfBytes, "application/pdf");
			//return View();

			try
			{
				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/GetDocumentById");
					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					var responseBody = await response.Content.ReadAsStringAsync();
					DocumentViewModel vm = JsonConvert.DeserializeObject<DocumentViewModel>(responseBody)!;

					string filePath = vm.FileLocation + vm.Filename;

					//var pdfFilePath = _documentService.ConvertWordToPdf(filePath);

					var pdfBytes = await System.IO.File.ReadAllBytesAsync(filePath);

					// View PDF next tab
					Response.Headers.Add("Content-Disposition", "inline; filename=" + vm.Filename);
					return File(pdfBytes, "application/pdf");




					// automatic download file
					//return File(pdfBytes, "application/pdf", vm.Filename, true);
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


		public async Task<IActionResult> Upload(ValidateTokenViewModel model)
		{
			try
			{
				DocumentViewModel vm = new DocumentViewModel();
				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/ValidateToken");

					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					string responseBody = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode)
					{
						vm = JsonConvert.DeserializeObject<DocumentViewModel>(responseBody)!;
						return View(vm);
					}
					return RedirectToAction("VerifyToken");
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return View();
			}
			finally
			{
				Log.CloseAndFlush();
			}

		}
		public async Task<IActionResult> UploadFile(DocumentViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{


					var data = new MultipartFormDataContent();
					data.Add(new StringContent(model.DocId.ToString() ?? ""), "DocId");
					data.Add(new StringContent(model.DocNo.ToString() ?? ""), "DocNo");
					data.Add(new StringContent(model.RequestById.ToString() ?? ""), "ModifiedBy");
					data.Add(new StringContent(DateTime.Now.ToString() ?? ""), "DateModified");

					if (model.DocFile != null)
					{
						var fileContent = new StreamContent(model.DocFile!.OpenReadStream());
						data.Add(fileContent, "DocFile", model.DocFile.FileName);
					}

					var response = await _httpclient.PostAsync(_apiconfig.Value.apiConnection + "api/Document/UploadFile", data);
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
				Log.Error(ex.Message.ToString());
				return Json(new { success = false, message = ex.Message });
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public async Task<IActionResult> VerifyToken()
		{
			return View();
		}

		public async Task<IActionResult> Workflow(DocumentViewModel model)
		{
			try
			{


				//vm.OptionsRequestBy = vm.UserList.Select(x =>
				//   new SelectListItem
				//   {
				//	   Value = x.UserId.ToString(),
				//	   Text = x.Lastname + ", " + x.Firstname
				//   }).ToList();


				using (var _httpclient = new HttpClient())
				{
					var currentUser = _userSessionHelper.GetCurrentUser();
					model.ModifiedBy = currentUser.UserId;

					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/WorkflowByDocId");

					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					var responseBody = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode)
					{
						WorkflowViewModel vm = JsonConvert.DeserializeObject<WorkflowViewModel>(responseBody)!;
						return View(vm);
					}
					//return Json(new { success = false, message = responseBody });
					return View();
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

			return View();
		}
	}
}
