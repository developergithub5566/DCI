using AspNetCoreGeneratedDocument;
using Aspose.Words.Saving;
using DCI.Core.Common;
using DCI.Core.Helpers;
using DCI.Models.Configuration;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.WebApp.Configuration;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using DocumentFormat.OpenXml.Office2016.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http;
using System.Text;
using System.Threading;
//using static System.Runtime.InteropServices.JavaScript.JSType;


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


        public async Task<IActionResult> Attendance(int id)
        {
            List<DailyTimeRecordViewModel> model = new List<DailyTimeRecordViewModel>();
            DailyTimeRecordViewModel param = new DailyTimeRecordViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

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

        public async Task<IActionResult> AttendanceLogs([FromBody] DailyTimeRecordViewModel param)
        {
            List<DailyTimeRecordViewModel> model = new List<DailyTimeRecordViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");


                    param.EMPLOYEE_ID = currentUser.EmployeeId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetBiometricLogsByEmployeeId");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<DailyTimeRecordViewModel>>(responseBody)!;
                    }

                }
                return Json(new { success = true, data = model });
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

        public async Task<IActionResult> DTR(DailyTimeRecordViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    //model.EMPLOYEE_NO = "080280";

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
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    model.CurrentUserId = currentUser.UserId;
                    model.EmployeeId = currentUser.EmployeeId;


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

        public async Task<IActionResult> SaveLeave(LeaveFormViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    //  model.CurrentUserId = currentUser.UserId;
                    model.EmployeeId = currentUser.EmployeeId;
                    model.ApproverId = currentUser.ApproverId;
                    model.CurrentUserId = currentUser.UserId;

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
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");


                    model.CurrentUserId = currentUser.UserId;
                    model.EmployeeId = currentUser.EmployeeId;
                    model.ApproverId = currentUser.ApproverId;

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
                        model.ApproverHead = model.LeaveRequestHeader.LeaveRequestHeaderId > 0 ? model.LeaveRequestHeader.ApproverHead : currentUser?.ApproverHead;

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
        }

        public async Task<IActionResult> CancelLeave(LeaveRequestHeaderViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    model.ModifiedBy = currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/CancelLeave");

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

        public async Task<IActionResult> LeaveDropdown(int filteryear)
        {
            LeaveViewModel model = new LeaveViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    model.CurrentUserId = currentUser.UserId;
                    model.EmployeeId = currentUser.EmployeeId;
                    model.FilterYear = filteryear;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllLeave");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<LeaveViewModel>(responseBody)!;
                        return Json(new { success = true, data = model });
                    }
                }
                return Json(new { success = false, data = model });

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

        public async Task<IActionResult> DTRCorrection(int DtrId)
        {
            List<DTRCorrectionViewModel> list = new List<DTRCorrectionViewModel>();

            try
            {
                using (var _httpclient = new HttpClient())
                {
                    DTRCorrectionViewModel model = new DTRCorrectionViewModel();
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

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
                    ViewBag.BreadCrumbLabel = "DTR Adjustment Summary";
                    ViewBag.ApproverHead = currentUser?.ApproverHead;
                    if ((int)EnumEmployeeScope.PerEmployee == model.ScopeTypeEmp)
                    {
                        ViewBag.BreadCrumbLabel = "DTR Adjustment";
                    }
                    ViewBag.Fullname = currentUser?.Fullname;
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

        public async Task<IActionResult> CancelDTRCorrection(DTRCorrectionViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    model.ModifiedBy = currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/CancelDTRCorrection");

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

        public async Task<IActionResult> SaveDTRCorrection(DTRCorrectionViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    model.CreatedBy = currentUser.UserId;
                    model.EmployeeId = currentUser.EmployeeId;
                    model.ApproverId = currentUser.ApproverId;

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

        public async Task<IActionResult> Undertime(DailyTimeRecordViewModel param)
        {
            List<DailyTimeRecordViewModel> model = new List<DailyTimeRecordViewModel>();
            //DailyTimeRecordViewModel param = new DailyTimeRecordViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");



                    param.CurrentUserId = currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllUndertime");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
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

        public async Task<IActionResult> UndertimeById(DailyTimeRecordViewModel param)
        {
            List<DailyTimeRecordViewModel> model = new List<DailyTimeRecordViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    // var currentUser = _userSessionHelper.GetCurrentUser();
                    //  model.CreatedBy = 2;//currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetUndertimeById");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    string emp_name = string.Empty;
                    string emp_no = string.Empty;
                    string emp_info = string.Empty;
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<DailyTimeRecordViewModel>>(responseBody)!;
                        emp_name = model.Count > 0 ? model.FirstOrDefault().NAME : string.Empty;
                        emp_no = model.Count > 0 ? model.FirstOrDefault().EMPLOYEE_NO : string.Empty;

                    }
                    return Json(new { success = true, data = model, emp = String.Format("{0} - {1}", emp_no, emp_name) });
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

        public async Task<IActionResult> SaveUndertime([FromBody] List<UndertimeDeductionViewModel> model)
        {
            try
            {
                UndertimeHeaderDeductionViewModel headermodel = new UndertimeHeaderDeductionViewModel();
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    headermodel.CurrentUserId = currentUser.UserId;
                    headermodel.UndertimeDeductionList = model;
                    headermodel.DateFrom = model.Count() > 0 ? model.FirstOrDefault().DateFrom : DateTime.Now;
                    headermodel.DateTo = model.Count() > 0 ? model.FirstOrDefault().DateTo : DateTime.Now;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(headermodel), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/SaveUndertime");
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

        public async Task<IActionResult> WFH(DailyTimeRecordViewModel param)
        {
            List<DailyTimeRecordViewModel> model = new List<DailyTimeRecordViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    param.ScopeTypeEmp = (int)EnumEmployeeScope.PerEmployee;
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

                    ViewBag.ApproverHead = currentUser?.ApproverHead;
                    ViewBag.Fullname = currentUser?.Fullname;
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

        public async Task<IActionResult> WFHLogs([FromBody] WFHViewModel param)
        {
            List<WFHViewModel> model = new List<WFHViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    // param.ScopeTypeEmp = (int)EnumEmployeeScope.PerEmployee;
                    param.EMPLOYEE_ID = currentUser.EmployeeId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetWFHLogsByEmployeeId");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<WFHViewModel>>(responseBody)!;
                    }

                }
                return Json(new { success = true, data = model });
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

        public async Task<IActionResult> WFHTimeIn(WFHViewModel param)
        {
            //  List<DailyTimeRecordViewModel> model = new List<DailyTimeRecordViewModel>();
            try
            {
                //using (var client = new HttpClient())
                //{
                //    LoginViewModel loginvm = new LoginViewModel();
                //    loginvm.Email = currentUser.Email;
                //    loginvm.Password = param.Password;
                //    var stringContentPassword = new StringContent(JsonConvert.SerializeObject(loginvm), Encoding.UTF8, "application/json");
                //    var requestPassword = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Account/Login");
                //    requestPassword.Content = stringContentPassword;
                //    var responsePassword = client.Send(requestPassword); //await Task.FromResult(_httpclient.Send(requestPassword));
                //    //  string responseBodyPassword = await responsePassword.Content.ReadAsStringAsync();
                //    if (!responsePassword.IsSuccessStatusCode)
                //    {
                //        return Json(new { success = false, message = "Invalid password!" });
                //    }
                //}

                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    param.EMPLOYEE_ID = currentUser.EmployeeId;
                    param.Password = string.Empty;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/SaveWFHTimeIn");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    //string emp_name = string.Empty;
                    //string emp_no = string.Empty;
                    //string emp_info = string.Empty;
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

        public async Task<IActionResult> SaveWFHApplication([FromBody] WfhApplicationViewModel param)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    param.Header.CurrentUserId = currentUser.UserId;
                    param.Header.EmployeeId = currentUser.EmployeeId;
                    param.Header.ApproverId = currentUser.ApproverId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/SaveWFHApplication");
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

        //public async Task<IActionResult> WFHApplication(WFHHeaderViewModel param)
        //{
        //    List<WFHHeaderViewModel> model = new List<WFHHeaderViewModel>();
        //    try
        //    {
        //        using (var _httpclient = new HttpClient())
        //        {
        //            var currentUser = _userSessionHelper.GetCurrentUser();
        //            if (currentUser == null)
        //                return RedirectToAction("Logout", "Account");



        //            var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
        //            var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllWFHApplication");
        //            request.Content = stringContent;
        //            var response = await _httpclient.SendAsync(request);
        //            var responseBody = await response.Content.ReadAsStringAsync();
        //            if (response.IsSuccessStatusCode == true)
        //            {
        //                model = JsonConvert.DeserializeObject<List<WFHHeaderViewModel>>(responseBody)!;
        //            }
        //            ViewBag.Fullname = currentUser?.Fullname;
        //        }
        //        return View(model);
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.ToString());
        //        return Json(new { success = false, message = ex.Message });
        //    }
        //    finally
        //    {
        //        Log.CloseAndFlush();
        //    }
        //    return Json(new { success = false, message = "An error occurred. Please try again." });
        //}

        public async Task<IActionResult> WFHApplication(WFHHeaderViewModel param)
        {
            List<WFHHeaderViewModel> model = new List<WFHHeaderViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    param.EmployeeId = currentUser.EmployeeId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllWFHApplication");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<WFHHeaderViewModel>>(responseBody)!;
                        return Json(new { success = true, data = model });
                    }
                    ViewBag.Fullname = currentUser?.Fullname;
                }
                return Json(new { success = true, data = string.Empty });
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

        public async Task<IActionResult> WFHApplicationDetails(WFHHeaderViewModel param)
        {
            //List<WfhDetailViewModel> model = new List<WfhDetailViewModel>();
            WfhApplicationViewModel model = new WfhApplicationViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetWFHApplicationDetailByWfhHeaderId");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<WfhApplicationViewModel>(responseBody)!;
                    }
                }
                return Json(new { success = true, data = model });
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

        public async Task<IActionResult> CancelWFHApplication(WFHHeaderViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");
                    model.CurrentUserId = currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/CancelWFHApplication");

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

        public async Task<IActionResult> Overtime(OvertimeViewModel param)
        {
            List<OvertimeViewModel> model = new List<OvertimeViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    param.ScopeTypeEmp = (int)EnumEmployeeScope.PerEmployee;
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
                    ViewBag.Fullname = currentUser?.Fullname;
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

        public async Task<IActionResult> OvertimeReport(OvertimePayReport param)
        {
            OvertimePayReport model = new OvertimePayReport();
            try
            {
                var currentUser = _userSessionHelper.GetCurrentUser();
                if (currentUser == null)
                    return RedirectToAction("Logout", "Account");

                using (var _httpclient = new HttpClient())
                {
                    param.EmployeeId = currentUser.EmployeeId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetOvertimeSummaryAsync");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<OvertimePayReport>(responseBody)!;
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

        public async Task<IActionResult> AddOvertime(OvertimeViewModel param)
        {
            OvertimeViewModel model = new OvertimeViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

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
                        model.Total = model.otDetails?.Sum(x => x.TotalMinutes) ?? 0;
                        model.TotalString = TimeHelper.ConvertMinutesToHHMM(model.Total);
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

        public async Task<IActionResult> AddOvertimeJson(OvertimeViewModel param)
        {
            OvertimeViewModel model = new OvertimeViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    param.CurrentUserId = currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/AddOvertime");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<OvertimeViewModel>(responseBody)!;
                        model.Total = model.otDetails?.Sum(x => x.TotalMinutes) ?? 0;
                        model.TotalString = TimeHelper.ConvertMinutesToHHMM(model.Total);
                        return Json(new { success = true, data = model });
                    }

                }
                return Json(new { success = false, data = model });
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

        public async Task<IActionResult> SaveOvertime([FromBody] OvertimeViewModel param)
        {
            OvertimeViewModel model = new OvertimeViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");
                    param.ApproverId = currentUser.ApproverId;
                    param.CurrentUserId = currentUser.UserId;
                    param.EmployeeId = currentUser.EmployeeId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/SaveOvertime");
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
            //  return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        public async Task<IActionResult> CategorizeOvertime([FromBody] OvertimeViewModel param)
        {
            //OvertimeViewModel param = new OvertimeViewModel();
            OvertimeViewModel model = new OvertimeViewModel();
            DailyTimeRecordViewModel dtrmodel = new DailyTimeRecordViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");
                    param.CurrentUserId = currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllAttendanceByDate");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        dtrmodel = JsonConvert.DeserializeObject<DailyTimeRecordViewModel>(responseBody)!;

                        TimeSpan firstin = new TimeSpan();
                        TimeSpan lastout = new TimeSpan();
                        TimeSpan clockout = new TimeSpan();
                        TimeSpan _firstinFinalLog = new TimeSpan();
                        TimeSpan _lastoutFinalLog = new TimeSpan();
                        TimeSpan totalWorkingHours = TimeSpan.Zero;

                        bool isHoliday = dtrmodel.IsHoliday;
                        bool isRestDay = param.OTDate.DayOfWeek == DayOfWeek.Saturday || param.OTDate.DayOfWeek == DayOfWeek.Sunday;
                        TimeSpan after8hrs = new TimeSpan(21, 59, 0);
                        TimeSpan nightDiffStartTime = new TimeSpan(22, 0, 0); //AFTER 10PM
                        TimeSpan nightDiffEndTime = new TimeSpan(6, 0, 0);
                        TimeSpan beforeNightDiff = new TimeSpan(22, 59, 0); //BEFORE 10PM
                        TimeSpan morethan1hr = TimeSpan.FromHours(1); //MORE THAN 1HR
                        TimeSpan eightHours = TimeSpan.FromHours(8); //8HRs
                        TimeSpan lunchBreak = TimeSpan.FromHours(1); //LUNCH BREAK
                        TimeSpan totalNightdiff;
                        TimeSpan totalWorkingHoursFromLogs = TimeSpan.Zero;

                        List<OvertimeDetailViewModel> otList = new List<OvertimeDetailViewModel>();


                        firstin = TimeSpan.Parse(param.OTTimeFrom);
                        lastout = TimeSpan.Parse(param.OTTimeTo);


                        //Official Business Approved or not
                        if (param.IsOfficialBuss)
                        {
                            firstin = TimeSpan.Parse(param.OTTimeFrom);
                            lastout = TimeSpan.Parse(param.OTTimeTo);
                            clockout = TimeSpan.Parse("16:30");
                        }
                        //Biometrics only
                        else if (dtrmodel.IsBiometricRecord && !dtrmodel.IsWFHFileRecord && !dtrmodel.IsOBFileRecord)
                        {
                            _firstinFinalLog = TimeSpan.Parse(dtrmodel.FIRST_IN);
                            _lastoutFinalLog = TimeSpan.Parse(dtrmodel.LAST_OUT);
                            clockout = TimeSpan.Parse(dtrmodel.CLOCK_OUT);
                            totalWorkingHoursFromLogs = TimeSpan.Parse(dtrmodel.TOTAL_WORKING_HOURS);
                        }
                        //WFH only
                        else if (!dtrmodel.IsBiometricRecord && dtrmodel.IsWFHFileRecord && !dtrmodel.IsOBFileRecord)
                        {
                            _firstinFinalLog = TimeSpan.Parse(dtrmodel.FIRST_IN_WFH);
                            _lastoutFinalLog = TimeSpan.Parse(dtrmodel.LAST_OUT_WFH);
                            clockout = TimeSpan.Parse(dtrmodel.CLOCK_OUT_WFH);
                            totalWorkingHoursFromLogs = TimeSpan.Parse(dtrmodel.TOTAL_WORKING_HOURS_WFH);

                        }
                        //WFH + biometrics (Get earlier time and late time between WFH and bio)
                        else if (dtrmodel.IsBiometricRecord && dtrmodel.IsWFHFileRecord && !dtrmodel.IsOBFileRecord)
                        {
                            _firstinFinalLog = TimeSpan.Parse(dtrmodel.FIRST_IN) < TimeSpan.Parse(dtrmodel.FIRST_IN_WFH) ? TimeSpan.Parse(dtrmodel.FIRST_IN) : TimeSpan.Parse(dtrmodel.FIRST_IN_WFH);
                            _lastoutFinalLog = TimeSpan.Parse(dtrmodel.LAST_OUT) > TimeSpan.Parse(dtrmodel.LAST_OUT_WFH) ? TimeSpan.Parse(dtrmodel.LAST_OUT) : TimeSpan.Parse(dtrmodel.LAST_OUT_WFH);
                            clockout = TimeSpan.Parse(dtrmodel.FIRST_IN) < TimeSpan.Parse(dtrmodel.FIRST_IN_WFH) ? TimeSpan.Parse(dtrmodel.CLOCK_OUT) : TimeSpan.Parse(dtrmodel.CLOCK_OUT_WFH);
                            totalWorkingHoursFromLogs = _lastoutFinalLog - _firstinFinalLog;
                            totalWorkingHoursFromLogs = totalWorkingHours >= eightHours ? totalWorkingHours - lunchBreak : totalWorkingHours;
                        }


                        //Working Hours

                        totalWorkingHours = lastout - firstin;
                        totalWorkingHours = totalWorkingHours >= eightHours ? totalWorkingHours - lunchBreak : totalWorkingHours;


                        //if (param.IsOfficialBuss && !dtrmodel.IsOBFileRecord)
                        //{
                        //    return Json(new { success = false, message = Constants.Msg_NoOfficialBusinessRecord });
                        //}

                        if (!param.IsOfficialBuss && !isHoliday && !isRestDay && !dtrmodel.IsWFHFileRecord && !dtrmodel.IsBiometricRecord)
                        {
                            return Json(new { success = false, message = Constants.Msg_NoBiometricsFound });
                        }

                        if (!param.IsOfficialBuss && !isHoliday && !isRestDay && totalWorkingHours < morethan1hr)
                        {
                            return Json(new { success = false, message = Constants.OverTime_Requires1Hr });
                        }

                        if (!param.IsOfficialBuss && !isHoliday && !isRestDay && lastout > _lastoutFinalLog)
                        {
                            return Json(new { success = false, message = Constants.OverTime_ExceedsActualTimeOut });
                        }

                        //REGULAR or SPECIAL HOLIDAY and REST DAY
                        if (isHoliday == true && isRestDay == true)
                        {
                            //if (totalWorkingHours >= eightHours)
                            //{
                            //    totalWorkingHours = totalWorkingHours - lunchBreak;
                            //}
                            //totalWorkingHours = totalWorkingHours < eightHours ? totalWorkingHours : eightHours;

                            OvertimeDetailViewModel otSpecialHoliday = new OvertimeDetailViewModel();
                            otSpecialHoliday.OTDate = param.OTDate.ToString();
                            otSpecialHoliday.OTDateString = param.OTDate.ToString("yyyy-MM-dd");
                            otSpecialHoliday.OTTimeFrom = firstin.ToString();
                            otSpecialHoliday.OTTimeTo = lastout.ToString();
                            otSpecialHoliday.TotalMinutes = (int)totalWorkingHours.TotalMinutes; //(int)holidayOnRestday.TotalMinutes;
                            otSpecialHoliday.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)totalWorkingHours.TotalMinutes);  //TimeHelper.ConvertMinutesToHHMM((int)holidayOnRestday.TotalMinutes);
                            otSpecialHoliday.OTType = (int)EnumOvertime.HolidayOnRestDay;
                            otSpecialHoliday.OTTypeName = Constants.OverTime_HolidayOnRestDay;
                            param.otDetails.Add(otSpecialHoliday);

                            if (lastout >= nightDiffStartTime)
                            {
                                totalNightdiff = lastout - nightDiffStartTime;

                                OvertimeDetailViewModel nightdiff = new OvertimeDetailViewModel();
                                nightdiff.OTDate = param.OTDate.ToString();
                                nightdiff.OTDateString = param.OTDate.ToString("yyyy-MM-dd");
                                nightdiff.OTTimeFrom = firstin.ToString();
                                nightdiff.OTTimeTo = lastout.ToString();
                                nightdiff.TotalMinutes = (int)totalNightdiff.TotalMinutes;
                                nightdiff.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)totalNightdiff.TotalMinutes);
                                nightdiff.OTType = (int)EnumOvertime.NightDifferential;
                                nightdiff.OTTypeName = Constants.OverTime_NightDifferential;
                                param.otDetails.Add(nightdiff);
                            }
                        }

                        //REGULAR or SPECIAL HOLIDAY or REST DAY
                        else if (isHoliday == true || isRestDay == true)
                        {
                            //if (totalWorkingHours >= eightHours)
                            //{
                            //    totalWorkingHours = totalWorkingHours - lunchBreak;                          
                            //}
                            //totalWorkingHours = totalWorkingHours < eightHours ? totalWorkingHours : eightHours;
                            var IsEightHour = totalWorkingHours >= eightHours ? eightHours : totalWorkingHours;

                            OvertimeDetailViewModel otSpecialHoliday = new OvertimeDetailViewModel();
                            otSpecialHoliday.OTDate = param.OTDate.ToString();
                            otSpecialHoliday.OTDateString = param.OTDate.ToString("yyyy-MM-dd");
                            otSpecialHoliday.OTTimeFrom = firstin.ToString();
                            otSpecialHoliday.OTTimeTo = lastout.ToString();
                            otSpecialHoliday.TotalMinutes = (int)IsEightHour.TotalMinutes;
                            otSpecialHoliday.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)IsEightHour.TotalMinutes);
                            otSpecialHoliday.OTType = (int)EnumOvertime.SpecialHoliday;
                            otSpecialHoliday.OTTypeName = Constants.OverTime_SpecialHoliday;
                            param.otDetails.Add(otSpecialHoliday);

                            if (totalWorkingHours >= eightHours)
                            {
                                TimeSpan spcHolidayStart8hrs;
                                TimeSpan spcHolidayAfter8hrs;

                                spcHolidayStart8hrs = firstin + eightHours + lunchBreak;

                                if (lastout < nightDiffStartTime)
                                {
                                    spcHolidayAfter8hrs = lastout - spcHolidayStart8hrs;
                                }
                                else
                                {
                                    spcHolidayAfter8hrs = nightDiffStartTime - spcHolidayStart8hrs;
                                }

                                if ((int)spcHolidayAfter8hrs.TotalMinutes > (int)TimeSpan.Zero.TotalMinutes)
                                {
                                    OvertimeDetailViewModel otAfter8hrs = new OvertimeDetailViewModel();
                                    otAfter8hrs.OTDate = param.OTDate.ToString();
                                    otAfter8hrs.OTDateString = param.OTDate.ToString("yyyy-MM-dd");
                                    otAfter8hrs.OTTimeFrom = firstin.ToString();
                                    otAfter8hrs.OTTimeTo = lastout.ToString();
                                    otAfter8hrs.TotalMinutes = (int)spcHolidayAfter8hrs.TotalMinutes;
                                    otAfter8hrs.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)spcHolidayAfter8hrs.TotalMinutes);
                                    otAfter8hrs.OTType = (int)EnumOvertime.After8hrs;
                                    otAfter8hrs.OTTypeName = Constants.OverTime_After8hrs;
                                    param.otDetails.Add(otAfter8hrs);
                                }
                            }

                            if (lastout > nightDiffStartTime)
                            {
                                totalNightdiff = lastout - nightDiffStartTime;

                                OvertimeDetailViewModel nightdiff = new OvertimeDetailViewModel();
                                nightdiff.OTDate = param.OTDate.ToString();
                                nightdiff.OTDateString = param.OTDate.ToString("yyyy-MM-dd");
                                nightdiff.OTTimeFrom = firstin.ToString();
                                nightdiff.OTTimeTo = lastout.ToString();
                                nightdiff.TotalMinutes = (int)totalNightdiff.TotalMinutes;
                                nightdiff.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)totalNightdiff.TotalMinutes);
                                nightdiff.OTType = (int)EnumOvertime.NightDifferential;
                                nightdiff.OTTypeName = Constants.OverTime_NightDifferential;
                                param.otDetails.Add(nightdiff);
                            }
                        }

                        else if (param.IsOfficialBuss == true && isHoliday == false && isRestDay == false)
                        {
                            var regOt = lastout - clockout;
                            if (regOt >= morethan1hr)
                            {
                                OvertimeDetailViewModel overtimeEntryDto = new OvertimeDetailViewModel();
                                overtimeEntryDto.OTDate = param.OTDate.ToString();
                                overtimeEntryDto.OTDateString = param.OTDate.ToString("yyyy-MM-dd");
                                overtimeEntryDto.OTTimeFrom = clockout.ToString();
                                overtimeEntryDto.OTTimeTo = lastout.ToString();
                                overtimeEntryDto.TotalMinutes = (int)regOt.TotalMinutes;
                                overtimeEntryDto.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)regOt.TotalMinutes);
                                overtimeEntryDto.OTType = (int)EnumOvertime.Regular;
                                overtimeEntryDto.OTTypeName = Constants.OverTime_Regular;
                                param.otDetails.Add(overtimeEntryDto);

                                if (lastout > nightDiffStartTime)
                                {
                                    totalNightdiff = lastout - nightDiffStartTime;

                                    OvertimeDetailViewModel nightdiff = new OvertimeDetailViewModel();
                                    nightdiff.OTDate = param.OTDate.ToString();
                                    nightdiff.OTDateString = param.OTDate.ToString("yyyy-MM-dd");
                                    nightdiff.OTTimeFrom = clockout.ToString();
                                    nightdiff.OTTimeTo = lastout.ToString();
                                    nightdiff.TotalMinutes = (int)totalNightdiff.TotalMinutes;
                                    nightdiff.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)totalNightdiff.TotalMinutes);
                                    nightdiff.OTType = (int)EnumOvertime.NightDifferential;
                                    nightdiff.OTTypeName = Constants.OverTime_NightDifferential;
                                    param.otDetails.Add(nightdiff);
                                }
                            }
                            else
                            {
                                return Json(new { success = false, message = Constants.OverTime_Requires1Hr });
                            }
                        }
                        else
                        {
                            var regOt = lastout - clockout;
                            if (regOt >= morethan1hr)
                            {
                                if (!param.IsOfficialBuss && totalWorkingHoursFromLogs > eightHours)
                                {
                                    OvertimeDetailViewModel overtimeEntryDto = new OvertimeDetailViewModel();
                                    overtimeEntryDto.OTDate = param.OTDate.ToString();
                                    overtimeEntryDto.OTDateString = param.OTDate.ToString("yyyy-MM-dd");
                                    overtimeEntryDto.OTTimeFrom = clockout.ToString();
                                    overtimeEntryDto.OTTimeTo = lastout.ToString();
                                    overtimeEntryDto.TotalMinutes = (int)regOt.TotalMinutes;
                                    overtimeEntryDto.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)regOt.TotalMinutes);
                                    overtimeEntryDto.OTType = (int)EnumOvertime.Regular;
                                    overtimeEntryDto.OTTypeName = Constants.OverTime_Regular;
                                    param.otDetails.Add(overtimeEntryDto);

                                    if (lastout > nightDiffStartTime)
                                    {
                                        totalNightdiff = lastout - nightDiffStartTime;

                                        OvertimeDetailViewModel nightdiff = new OvertimeDetailViewModel();
                                        nightdiff.OTDate = param.OTDate.ToString();
                                        nightdiff.OTDateString = param.OTDate.ToString("yyyy-MM-dd");
                                        nightdiff.OTTimeFrom = clockout.ToString();
                                        nightdiff.OTTimeTo = lastout.ToString();
                                        nightdiff.TotalMinutes = (int)totalNightdiff.TotalMinutes;
                                        nightdiff.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)totalNightdiff.TotalMinutes);
                                        nightdiff.OTType = (int)EnumOvertime.NightDifferential;
                                        nightdiff.OTTypeName = Constants.OverTime_NightDifferential;
                                        param.otDetails.Add(nightdiff);

                                    }
                                }
                                else
                                {
                                    return Json(new { success = false, message = Constants.OverTime_Requires8HrNotBeenMet });
                                }
                            }
                            else
                            {
                                return Json(new { success = false, message = Constants.OverTime_Requires1Hr });
                            }

                        }
                        return Json(new { success = true, data = param.otDetails.ToList() });
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

        public async Task<IActionResult> CheckOvertimeDate([FromBody] OvertimeEntryDto param)
        {
            OvertimeEntryDto model = new OvertimeEntryDto();
            // OvertimeEntryDto param = new OvertimeEntryDto();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    //var currentUser = _userSessionHelper.GetCurrentUser();
                    //   param.CurrentUserId = currentUser.UserId;

                    // param.OTDate = OTDate ?? DateTime.Now;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/CheckOvertimeDate");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode)
                    {
                        model = JsonConvert.DeserializeObject<OvertimeEntryDto>(responseBody)!;
                        return Json(new { success = true, data = model });
                    }
                    return Json(new { success = false, data = model });

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
            //  return Json(new { success = false, message = "An error occurred. Please try again." });
        }

        public async Task<IActionResult> Late(DailyTimeRecordViewModel param)
        {
            List<DailyTimeRecordViewModel> model = new List<DailyTimeRecordViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");
                    param.CurrentUserId = currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllLate");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
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

        public async Task<IActionResult> LateById(DailyTimeRecordViewModel param)
        {
            List<DailyTimeRecordViewModel> model = new List<DailyTimeRecordViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    // var currentUser = _userSessionHelper.GetCurrentUser();
                    //  model.CreatedBy = 2;//currentUser.UserId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetLateById");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    string emp_name = string.Empty;
                    string emp_no = string.Empty;
                    string emp_info = string.Empty;
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<DailyTimeRecordViewModel>>(responseBody)!;
                        emp_name = model.Count > 0 ? model.FirstOrDefault().NAME : string.Empty;
                        emp_no = model.Count > 0 ? model.FirstOrDefault().EMPLOYEE_NO : string.Empty;

                    }
                    return Json(new { success = true, data = model, emp = String.Format("{0} - {1}", emp_no, emp_name) });
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

        public async Task<IActionResult> SaveLate([FromBody] List<LateDeductionViewModel> model)
        {
            try
            {
                LateHeaderDeductionViewModel headermodel = new LateHeaderDeductionViewModel();
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");
                    headermodel.CurrentUserId = currentUser.UserId;
                    headermodel.LateDeductionList = model;
                    headermodel.DateFrom = model.Count() > 0 ? model.FirstOrDefault().DateFrom : DateTime.Now;
                    headermodel.DateTo = model.Count() > 0 ? model.FirstOrDefault().DateTo : DateTime.Now;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(headermodel), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/SaveLate");
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


        public async Task<IActionResult> LeaveManagement(LeaveViewModel model)
        {
            //   LeaveViewModel model = new LeaveViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    model.CurrentUserId = currentUser.UserId;
                    model.EmployeeId = currentUser.EmployeeId;
                    
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllLeaveMangement");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<LeaveViewModel>(responseBody)!;
                    }
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
        public async Task<IActionResult> RequestLeaveMangement(LeaveViewModel model)
        {

            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");


                    model.CurrentUserId = currentUser.UserId;
                    model.EmployeeId = currentUser.EmployeeId;
                    model.ApproverId = currentUser.ApproverId;
                    model.RequestFiledBy = (int)EnumRequestFiledBy.HR;
                 

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


                        model.OptionsForm201List = model.EmployeeDropdownList?.Select(x =>
                                   new SelectListItem
                                   {
                                       Value = x.EmployeeId.ToString(),
                                       Text = x.Display
                                   }).ToList();

                        model.ApproverHead = currentUser.Fullname;

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
        }

        public async Task<IActionResult> SaveLeaveManagement(LeaveFormViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    _httpclient.Timeout = TimeSpan.FromSeconds(30);

                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                   
                    model.EmployeeId = model.EmployeeId; //selected Employee
                    model.ApproverId = currentUser.UserId;  //PreparedBy
                    model.CurrentUserId = currentUser.UserId;//PreparedBy

                    model.SelectedDateList = JsonConvert.DeserializeObject<List<DateTime>>(model.SelectedDateJson);


                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/SaveLeaveManagement");
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

        public async Task<IActionResult> GetAllLeaveMangementByEmpId(LeaveViewModel param)
        {          
            try
            {
                LeaveViewModel model = new LeaveViewModel();

                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    if (currentUser == null)
                        return RedirectToAction("Logout", "Account");

                    param.CurrentUserId = currentUser.UserId;
                    param.EmployeeId = param.EmployeeId;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllLeaveMangementByEmpId");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<LeaveViewModel>(responseBody)!;
                    }
                    ViewBag.Fullname = currentUser?.Fullname;

                    return Json(new { success = true, data = model });
                }             

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return Json(new { success = false, message = ex.ToString() });
            }
            finally
            {
                Log.CloseAndFlush();
            }           
        }

    }
}
