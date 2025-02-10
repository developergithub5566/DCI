using DCI.Core.Common;
using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using DCI.WebApp.Configuration;
using DCI.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

namespace DCI.WebApp.Controllers
{
    public class ReportController : Controller
    {
        private readonly IOptions<APIConfigModel> _apiconfig;
        private readonly UserSessionHelper _userSessionHelper;
        private readonly DocumentService _documentService;

        public ReportController(IOptions<APIConfigModel> apiconfig, UserSessionHelper userSessionHelper, DocumentService documentService)
        {
            this._apiconfig = apiconfig;
            this._userSessionHelper = userSessionHelper;
            this._documentService = documentService;
        }
        public  async Task<IActionResult> Index(DocumentViewModel param)
        {
            List<DocumentViewModel> model = new List<DocumentViewModel>();

            using (var _httpclient = new HttpClient())
            {
                DocumentViewModel _filterRoleModel = new DocumentViewModel();

                var currentUser = _userSessionHelper.GetCurrentUser();
                _filterRoleModel.CurrentRoleId = currentUser.RoleId;
                _filterRoleModel.StatusId = param.StatusId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/ReportsListofDocumentByStatus");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<DocumentViewModel>>(responseBody)!;
                    return View(model);
                }
            }
            return View(model);
        }

        public async Task<IActionResult> LoadData(DocumentViewModel param)
        {
            List<DocumentViewModel> model = new List<DocumentViewModel>();

            using (var _httpclient = new HttpClient())
            {
                DocumentViewModel _filterRoleModel = new DocumentViewModel();

                var currentUser = _userSessionHelper.GetCurrentUser();
                _filterRoleModel.CurrentRoleId = currentUser.RoleId;
                _filterRoleModel.StatusId = param.StatusId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/ReportsListofDocumentByStatus");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<DocumentViewModel>>(responseBody)!;

                    if(param.StatusId > 0 )
                    {
                        model = model.Where(x => x.StatusId == param.StatusId).ToList();
                    }                  
                    return Json(new { success = true, message = "test", data = model });
                }
            }
            return Json(new { success = false, message = "Error" });
        }

    }
}
