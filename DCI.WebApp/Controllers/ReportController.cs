using DCI.Core.Common;
using DCI.Core.Helpers;
using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using DCI.WebApp.Configuration;
using DCI.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
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
        //public  async Task<IActionResult> Index(DocumentViewModel param)
        //{
        //    List<DocumentViewModel> model = new List<DocumentViewModel>();

        //    using (var _httpclient = new HttpClient())
        //    {
        //        DocumentViewModel _filterRoleModel = new DocumentViewModel();

        //        var currentUser = _userSessionHelper.GetCurrentUser();
        //        _filterRoleModel.CurrentRoleId = currentUser.RoleId;
        //        _filterRoleModel.StatusId = param.StatusId;

        //        var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
        //        var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/ReportsListofDocumentByStatus");

        //        request.Content = stringContent;
        //        var response = await _httpclient.SendAsync(request);
        //        var responseBody = await response.Content.ReadAsStringAsync();
        //        if (response.IsSuccessStatusCode)
        //        {
        //            model = JsonConvert.DeserializeObject<List<DocumentViewModel>>(responseBody)!;
        //            return View(model);
        //        }
        //    }
        //    return View(model);
        //}

        public async Task<IActionResult> Index(DocumentViewModel param)
        {
            //var documentData = new List<ReportGraphViewModel>
            //{
            //new ReportGraphViewModel { Status = "Pending", Count = 10 },
            //new ReportGraphViewModel { Status = "Approved", Count = 15 },
            //new ReportGraphViewModel { Status = "Rejected", Count = 5 },

            //};
            ReportGraphsDataViewModel model = new ReportGraphsDataViewModel();

            using (var _httpclient = new HttpClient())
            {
                HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Employee/ReportGraphByStatus");
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<ReportGraphsDataViewModel>(responseBody)!;
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
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/ReportsListofDocument");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<DocumentViewModel>>(responseBody)!;

                    if (param.StatusId > 0)
                    {
                        model = model.Where(x => x.StatusId == param.StatusId).ToList();
                    }
                    return Json(new { success = true, message = "test", data = model });
                }
            }
            return Json(new { success = false, message = "Error" });
        }
        public async Task<IActionResult> ReportStatus(DocumentViewModel param)
        {
            List<DocumentViewModel> model = new List<DocumentViewModel>();

            using (var _httpclient = new HttpClient())
            {
                DocumentViewModel _filterRoleModel = new DocumentViewModel();

                var currentUser = _userSessionHelper.GetCurrentUser();
                _filterRoleModel.CurrentRoleId = currentUser.RoleId;
                _filterRoleModel.StatusId = param.StatusId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/ReportsListofDocument");

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

        public async Task<IActionResult> ReportStatusLoadData(DocumentViewModel param)
        {
            List<DocumentViewModel> model = new List<DocumentViewModel>();

            using (var _httpclient = new HttpClient())
            {
                DocumentViewModel _filterRoleModel = new DocumentViewModel();

                var currentUser = _userSessionHelper.GetCurrentUser();
                _filterRoleModel.CurrentRoleId = currentUser.RoleId;
                _filterRoleModel.StatusId = param.StatusId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/ReportsListofDocument");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<DocumentViewModel>>(responseBody)!;

                    if (param.StatusId > 0)
                    {
                        model = model.Where(x => x.StatusId == param.StatusId).ToList();
                    }
                    return Json(new { success = true, message = "", data = model });
                }
            }
            return Json(new { success = false, message = "Error" });
        }

        public async Task<IActionResult> ReportDocType(DocumentViewModel param)
        {
            List<DocumentViewModel> model = new List<DocumentViewModel>();

            using (var _httpclient = new HttpClient())
            {
                DocumentViewModel _filterRoleModel = new DocumentViewModel();

                var currentUser = _userSessionHelper.GetCurrentUser();
                _filterRoleModel.CurrentRoleId = currentUser.RoleId;
                _filterRoleModel.StatusId = param.StatusId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/ReportsListofDocument");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<DocumentViewModel>>(responseBody)!;
                    //return View(model);
                }
            }

            List<DocumentTypeViewModel> doctypemodel = new List<DocumentTypeViewModel>();

            using (var _httpclient = new HttpClient())
            {
                HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllDocumentType");
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    doctypemodel = JsonConvert.DeserializeObject<List<DocumentTypeViewModel>>(responseBody)!;
                }
            }

            var doctype = doctypemodel.Select(r => new SelectListItem
            {
                Value = r.DocTypeId.ToString(),
                Text = r.Name
            }).ToList();

            ViewBag.DocTypeList = doctype;

            return View(model);
        }

        public async Task<IActionResult> ReportDocTypeLoadData(DocumentViewModel param)
        {
            List<DocumentViewModel> model = new List<DocumentViewModel>();

            using (var _httpclient = new HttpClient())
            {
                DocumentViewModel _filterRoleModel = new DocumentViewModel();

                var currentUser = _userSessionHelper.GetCurrentUser();
                _filterRoleModel.CurrentRoleId = currentUser.RoleId;
                _filterRoleModel.StatusId = param.StatusId;

                var stringContent = new StringContent(JsonConvert.SerializeObject(_filterRoleModel), Encoding.UTF8, "application/json");
                var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Document/ReportsListofDocument");

                request.Content = stringContent;
                var response = await _httpclient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                if (response.IsSuccessStatusCode)
                {
                    model = JsonConvert.DeserializeObject<List<DocumentViewModel>>(responseBody)!;

                    if (param.DocTypeId > 0)
                    {
                        model = model.Where(x => x.DocTypeId == param.DocTypeId).ToList();
                    }
                    //   return Json(new { success = true, message = "", data = model });
                }
            }
            //List<DocumentTypeViewModel> doctypemodel = new List<DocumentTypeViewModel>();

            //using (var _httpclient = new HttpClient())
            //{
            //    HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllDocumentType");
            //    string responseBody = await response.Content.ReadAsStringAsync();

            //    if (response.IsSuccessStatusCode)
            //    {
            //        doctypemodel = JsonConvert.DeserializeObject<List<DocumentTypeViewModel>>(responseBody)!;
            //    }
            //}

            //var doctype = doctypemodel.Select(r => new SelectListItem
            //{
            //    Value = r.DocTypeId.ToString(),
            //    Text = r.Name
            //}).ToList();

            //ViewBag.DocTypeList = doctype;

            return Json(new { success = true, message = "", data = model });

        }




        public async Task<IActionResult> Undertime(DailyTimeRecordViewModel param)
        {
            List<UndertimeHeaderViewModel> model = new List<UndertimeHeaderViewModel>();

            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    param.CurrentUserId = 1;//currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetUndertimeDeduction");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<UndertimeHeaderViewModel>>(responseBody)!;
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

        public async Task<IActionResult> UndertimeByEmpNo(UndertimeHeaderViewModel param) //UndertimeDeductionByHeaderId
        {
            List<UndertimeDetailViewModel> model = new List<UndertimeDetailViewModel>();

            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    // param.CurrentUserId = 1;//currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetUndertimeDeductionByHeaderId");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<UndertimeDetailViewModel>>(responseBody)!;
                    }
                    // return Json(new { success = true, message = "", data = model });
                    ViewBag.RequestNo = param.RequestNo ?? string.Empty;

                    return View(model);
                }

                return Json(new { success = false, message = "", data = model });

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
        public async Task<IActionResult> WFH(DailyTimeRecordViewModel param)
        {
            List<DailyTimeRecordViewModel> model = new List<DailyTimeRecordViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();

                    param.ScopeTypeEmp = (int)EnumEmployeeScope.ALL;
                    param.EMPLOYEE_ID = currentUser.EmployeeId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllWFH");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<DailyTimeRecordViewModel>>(responseBody)!;
                    }
                    ViewBag.BreadCrumbLabel = "WFH Summary";
                    ViewBag.ApproverHead = currentUser?.ApproverHead;
                    if ((int)EnumEmployeeScope.PerEmployee == param.ScopeTypeEmp)
                    {
                        ViewBag.BreadCrumbLabel = "WFH";
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
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        public async Task<IActionResult> Attendance(int id)
        {
            List<DailyTimeRecordViewModel> model = new List<DailyTimeRecordViewModel>();
            DailyTimeRecordViewModel param = new DailyTimeRecordViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    param.ScopeTypeEmp = id;

                    param.CurrentUserId = currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllDTR");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<DailyTimeRecordViewModel>>(responseBody)!;
                    }
                    //ViewBag.BreadCrumbLabelA = "DTR Management";
                    //ViewBag.BreadCrumbLabelB = "Attendance Summary";
                    //if ((int)EnumEmployeeScope.PerEmployee == param.ScopeTypeEmp)
                    //{
                    //    ViewBag.BreadCrumbLabelA = "Daily Time Record";
                    //    ViewBag.BreadCrumbLabelB = "Attendance";
                    //}
                    ViewBag.Fullname = currentUser?.Fullname;
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

        public async Task<IActionResult> MasterList()
        {
            List<Form201ViewModel> model = new List<Form201ViewModel>();
            try
            {
                var currentUser = _userSessionHelper.GetCurrentUser();
                using (var _httpclient = new HttpClient())
                {

                    HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Maintenance/GetAllEmployee");
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<Form201ViewModel>>(responseBody)!;
                    }
                }
                ViewBag.Fullname = currentUser?.Fullname;
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
                        vm = JsonConvert.DeserializeObject<Form201ViewModel>(responseBody)!;
                        vm.OptionsEmployeeStatus = vm.EmployeeStatusList.Select(x =>
                                 new SelectListItem
                                 {
                                     Value = x.EmployeeStatusId.ToString(),
                                     Text = x.EmployeeStatusName
                                 }).ToList();

                        vm.OptionsPosition = vm.PositionList.Select(x =>
                               new SelectListItem
                               {
                                   Value = x.PositionId.ToString(),
                                   Text = x.PositionName
                               }).ToList();

                        vm.OptionsDepartment = vm.DepartmentList.Select(x =>
                               new SelectListItem
                               {
                                   Value = x.DepartmentId.ToString(),
                                   Text = x.DepartmentName
                               }).ToList();

                        vm.OptionsWorkLocation = vm.WorkLocationList.Select(x =>
                           new SelectListItem
                           {
                               Value = x.WorkLocationId.ToString(),
                               Text = x.Location
                           }).ToList();

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

        public async Task<IActionResult> DTRAdjustment(int DtrId)
        {
            List<DTRCorrectionViewModel> list = new List<DTRCorrectionViewModel>();

            try
            {
                using (var _httpclient = new HttpClient())
                {
                    DTRCorrectionViewModel model = new DTRCorrectionViewModel();
                    var currentUser = _userSessionHelper.GetCurrentUser();

                    model.CreatedBy = currentUser.UserId;
                    model.ScopeTypeEmp = DtrId;

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

        public async Task<IActionResult> Overtime(OvertimeViewModel param)
        {
            List<OvertimeViewModel> model = new List<OvertimeViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();

                    param.ScopeTypeEmp = (int)EnumEmployeeScope.ALL;
                    param.CurrentUserId = currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/Overtime");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
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

        public async Task<IActionResult> AddOvertime(OvertimeViewModel param)
        {
            OvertimeViewModel model = new OvertimeViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();


                    param.CurrentUserId = currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/AddOvertime");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<OvertimeViewModel>(responseBody)!;

                        model.ApprovedBy = model.OTHeaderId == 0 ? currentUser.ApproverHead : model.ApprovedBy;
                        model.StatusName = model.OTHeaderId == 0 ? "Draft" : model.StatusName;

                        return View(model);
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
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        public async Task<IActionResult> OvertimePay(OvertimePayReport param)
        {
            // List<OvertimePayReport> model = new List<OvertimePayReport>();
            OvertimePayReport model = new OvertimePayReport();
            try
            {
                var currentUser = _userSessionHelper.GetCurrentUser();

                using (var _httpclient = new HttpClient())
                {
                   

                    //  param.ScopeTypeEmp = (int)EnumEmployeeScope.ALL;
                    // param.CurrentUserId = currentUser.UserId;
                    param.EmployeeId = currentUser.EmployeeId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetOvertimeSummaryAsync");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<OvertimePayReport>(responseBody)!;

                        model.OptionsEmployee = model.EmployeeList?.Select(x =>
                           new SelectListItem
                           {
                               Value = x.EmployeeId.ToString(),
                               Text = x.Lastname  + " " + x.Firstname
                           }).ToList();

                    }

                }
                ViewBag.Fullname = currentUser?.Fullname;
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
            return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        public async Task<IActionResult> LeaveList(LeaveViewModel model)
        {
            try
            {
                List<LeaveReportViewModel> leaveReportViewModel = new List<LeaveReportViewModel>();

                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    model.CurrentUserId = currentUser.UserId;
                    model.EmployeeId = currentUser.EmployeeId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllLeaveReport");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        leaveReportViewModel = JsonConvert.DeserializeObject<List<LeaveReportViewModel>>(responseBody)!;
                    }
                }

                return View(leaveReportViewModel);

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

        public async Task<IActionResult> Leave(LeaveViewModel model)
        {

            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    model.CurrentUserId = currentUser.UserId;
                    model.EmployeeId = model.EmployeeId;

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

    }
}
