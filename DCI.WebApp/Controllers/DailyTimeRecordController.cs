using AspNetCoreGeneratedDocument;
using Aspose.Words.Saving;
using DCI.Core.Common;
using DCI.Core.Helpers;
using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using DCI.WebApp.Configuration;
using DocumentFormat.OpenXml.Drawing.Charts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Net.Http;
using System.Text;
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
                    param.TypeId = id;

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
                    ViewBag.BreadCrumbLabelA = "DTR Management";
                    ViewBag.BreadCrumbLabelB = "Attendance Summary";
                    if ((int)EnumTypeData.EMP == param.TypeId)
                    {
                        ViewBag.BreadCrumbLabelA = "Daily Time Record";
                        ViewBag.BreadCrumbLabelB = "Attendance";
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
                    //  model.CurrentUserId = currentUser.UserId;
                    model.EmployeeId = currentUser.EmployeeId;

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

        public async Task<IActionResult> DTRCorrection(int id)
        {
            List<DTRCorrectionViewModel> list = new List<DTRCorrectionViewModel>();

            try
            {
                using (var _httpclient = new HttpClient())
                {
                    DTRCorrectionViewModel model = new DTRCorrectionViewModel();
                    var currentUser = _userSessionHelper.GetCurrentUser();

                    model.CreatedBy = 2;//currentUser.UserId;
                    model.TypeId = id;

                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllDTRCorrection");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        list = JsonConvert.DeserializeObject<List<DTRCorrectionViewModel>>(responseBody)!;

                    }
                    ViewBag.BreadCrumbLabel = "DTR Correction Summary";
                    if ((int)EnumTypeData.EMP == model.TypeId)
                    {
                        ViewBag.BreadCrumbLabel = "DTR Correction";
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

        public async Task<IActionResult> Undertime(DailyTimeRecordViewModel param)
        {
            List<DailyTimeRecordViewModel> model = new List<DailyTimeRecordViewModel>();
            //DailyTimeRecordViewModel param = new DailyTimeRecordViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();


                    param.CurrentUserId = 2;//currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllUndertime");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<DailyTimeRecordViewModel>>(responseBody)!;
                    }

                    if ((int)EnumTypeData.EMP == param.TypeId)
                    {

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
                        emp_name = model.FirstOrDefault().NAME ?? string.Empty;
                        emp_no = model.FirstOrDefault().EMPLOYEE_NO ?? string.Empty;

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

        public async Task<IActionResult> WFH(WFHViewModel param)
        {
            List<WFHViewModel> model = new List<WFHViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();


                    param.CurrentUserId = currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllWFHById");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<WFHViewModel>>(responseBody)!;
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

        public async Task<IActionResult> WFHTimeIn(WFHViewModel param)
        {
            //  List<DailyTimeRecordViewModel> model = new List<DailyTimeRecordViewModel>();
            try
            {


                using (var _httpclient = new HttpClient())
                {

                    var currentUser = _userSessionHelper.GetCurrentUser();

                    param.EMPLOYEE_ID = currentUser.EmployeeId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/SaveWFHTimeIn");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    string emp_name = string.Empty;
                    string emp_no = string.Empty;
                    string emp_info = string.Empty;
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

        public async Task<IActionResult> Overtime(OvertimeViewModel param)
        {
            List<OvertimeViewModel> model = new List<OvertimeViewModel>();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();


                    param.CurrentUserId = currentUser.UserId;


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/Overtime");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        model = JsonConvert.DeserializeObject<List<OvertimeViewModel>>(responseBody)!;
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
                        // model = JsonConvert.DeserializeObject<OvertimeViewModel>(responseBody)!;
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


        public async Task<IActionResult> SaveOvertime([FromBody] SubmitOvertimeViewModel param)
        {
            OvertimeViewModel model = new OvertimeViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    //var currentUser = _userSessionHelper.GetCurrentUser();


                    //param.CurrentUserId = currentUser.UserId;


                    //var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    //var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/SaveOvertime");
                    //request.Content = stringContent;
                    //var response = await _httpclient.SendAsync(request);
                    //var responseBody = await response.Content.ReadAsStringAsync();
                    //if (response.IsSuccessStatusCode == true)
                    //{
                    //    // model = JsonConvert.DeserializeObject<OvertimeViewModel>(responseBody)!;
                    //    return View(model);
                    //}

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

        public async Task<IActionResult> CategorizeOvertime([FromBody] SubmitOvertimeViewModel param)
        {
            OvertimeViewModel model = new OvertimeViewModel();
            DailyTimeRecordViewModel dtrmodel = new DailyTimeRecordViewModel();
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    // var currentUser = _userSessionHelper.GetCurrentUser(); 
                    //param.OTType = 1;
                    //param.TotalMinutes = 100;
                    param.EmployeeNo = "080343";

                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllAttendanceByDate");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        dtrmodel = JsonConvert.DeserializeObject<DailyTimeRecordViewModel>(responseBody)!;


                        if (dtrmodel != null)
                        {
                            var Outtime = Convert.ToDateTime(dtrmodel.DATE);
                            TimeSpan firstin = TimeSpan.Parse(dtrmodel.FIRST_IN);
                            TimeSpan lastout = TimeSpan.Parse(dtrmodel.LAST_OUT);
                            TimeSpan clockout = TimeSpan.Parse(dtrmodel.CLOCK_OUT);
                            var overtime = Convert.ToDateTime(dtrmodel.OVERTIME);
                            bool isHoliday = true;
                            bool isRestDay = dtrmodel.DATE.DayOfWeek == DayOfWeek.Saturday || dtrmodel.DATE.DayOfWeek == DayOfWeek.Sunday;
                            //bool isRestDay = true;

                            TimeSpan after8hrs = new TimeSpan(21, 59, 0); // 10:00 PM
                            TimeSpan nightDiffStartTime = new TimeSpan(22, 0, 0); // 10:00 PM
                            TimeSpan nightDiffEndTime = new TimeSpan(6, 0, 0);    // 6:00 AM
                            TimeSpan totalNightdiff;
                            //  TimeSpan morethan1hr = new TimeSpan(0, 60, 0);    // 6:00 AM

                            TimeSpan beforeNightDiff = new TimeSpan(22, 59, 0);    // 6:00 AM

                            List<OvertimeEntryDto> otList = new List<OvertimeEntryDto>();




                            if (isHoliday == true && isRestDay == false)
                            {
                                if (TimeSpan.TryParse(dtrmodel.TOTAL_WORKING_HOURS, out TimeSpan time))
                                {
                                    TimeSpan eightHours = TimeSpan.FromHours(8);
                                    //  var spcHolidayMinutes = firstin - lastout;
                                    if (time > eightHours)
                                    {
                                        OvertimeEntryDto otSpecialHoliday = new OvertimeEntryDto();
                                        otSpecialHoliday.OTDate = param.OTDate;
                                        otSpecialHoliday.OTTimeFrom = clockout.ToString();
                                        otSpecialHoliday.OTTimeTo = lastout.ToString();
                                        otSpecialHoliday.TotalMinutes = (int)eightHours.TotalMinutes;
                                        otSpecialHoliday.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)eightHours.TotalMinutes);
                                        otSpecialHoliday.OTType = 3;
                                        otSpecialHoliday.OTTypeName = "130% SPECIAL HOLIDAY MON - SUN / SAT-SUN (FIRST 8 HRS.)";
                                        // otList.Add(otSpecialHoliday);
                                        param.Entries.Add(otSpecialHoliday);

                                        TimeSpan spcHolidayAfter8hrs = lastout - clockout;
                                        TimeSpan morethan1hr = TimeSpan.FromHours(1);


                                        if (spcHolidayAfter8hrs > morethan1hr)
                                        {
                                            OvertimeEntryDto otAfter8hrs = new OvertimeEntryDto();
                                            otAfter8hrs.OTDate = param.OTDate;
                                            otAfter8hrs.OTTimeFrom = clockout.ToString();
                                            otAfter8hrs.OTTimeTo = lastout.ToString();
                                            otAfter8hrs.TotalMinutes = (int)spcHolidayAfter8hrs.TotalMinutes;
                                            otAfter8hrs.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)spcHolidayAfter8hrs.TotalMinutes);
                                            otAfter8hrs.OTType = 4;
                                            otAfter8hrs.OTTypeName = "169% AFTER 8 HRS OF 130%";
                                            param.Entries.Add(otAfter8hrs);
                                        }

                                        if (lastout > nightDiffStartTime)
                                        {
                                            totalNightdiff = lastout - nightDiffStartTime;

                                            OvertimeEntryDto nightdiff = new OvertimeEntryDto();
                                            nightdiff.OTDate = param.OTDate;
                                            nightdiff.OTTimeFrom = clockout.ToString();
                                            nightdiff.OTTimeTo = lastout.ToString();
                                            nightdiff.TotalMinutes = (int)totalNightdiff.TotalMinutes;
                                            nightdiff.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)totalNightdiff.TotalMinutes);
                                            nightdiff.OTType = 2;
                                            nightdiff.OTTypeName = "10% NIGHT DIFFERENTIAL (10PM - 6AM) MON - SUN / HOLIDAY";
                                            // otList.Add(nightdiff);
                                            param.Entries.Add(nightdiff);
                                        }
                                    }
                                }
                            }

                            else if (isHoliday == true && isRestDay == true)
                            {
                                if (TimeSpan.TryParse(dtrmodel.TOTAL_WORKING_HOURS, out TimeSpan holidayOnRestday))
                                {
                                    TimeSpan eightHours = TimeSpan.FromHours(8);


                                    OvertimeEntryDto otSpecialHoliday = new OvertimeEntryDto();
                                    otSpecialHoliday.OTDate = param.OTDate;
                                    otSpecialHoliday.OTTimeFrom = clockout.ToString();
                                    otSpecialHoliday.OTTimeTo = lastout.ToString();
                                    otSpecialHoliday.TotalMinutes = (int)holidayOnRestday.TotalMinutes;
                                    otSpecialHoliday.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)holidayOnRestday.TotalMinutes);
                                    otSpecialHoliday.OTType = 5;
                                    otSpecialHoliday.OTTypeName = "150% HOLIDAY ON REST DAY (SAT - SUN)";
                                    param.Entries.Add(otSpecialHoliday);

                                    //TimeSpan spcHolidayAfter8hrs = nightDiffStartTime - clockout;
                                    //TimeSpan morethan1hr = TimeSpan.FromHours(1);

                                    //if (spcHolidayAfter8hrs > morethan1hr)
                                    //{
                                    //    OvertimeEntryDto otAfter8hrs = new OvertimeEntryDto();
                                    //    otAfter8hrs.OTDate = param.OTDate;
                                    //    otAfter8hrs.OTTimeFrom = clockout.ToString();
                                    //    otAfter8hrs.OTTimeTo = lastout.ToString();
                                    //    otAfter8hrs.TotalMinutes = (int)spcHolidayAfter8hrs.TotalMinutes;
                                    //    otAfter8hrs.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)spcHolidayAfter8hrs.TotalMinutes);
                                    //    otAfter8hrs.OTType = 4;
                                    //    otAfter8hrs.OTTypeName = "169% AFTER 8 HRS OF 130%";
                                    //    param.Entries.Add(otAfter8hrs);
                                    //}

                                    if (lastout > nightDiffStartTime)
                                    {
                                        totalNightdiff = lastout - nightDiffStartTime;

                                        OvertimeEntryDto nightdiff = new OvertimeEntryDto();
                                        nightdiff.OTDate = param.OTDate;
                                        nightdiff.OTTimeFrom = clockout.ToString();
                                        nightdiff.OTTimeTo = lastout.ToString();
                                        nightdiff.TotalMinutes = (int)totalNightdiff.TotalMinutes;
                                        nightdiff.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)totalNightdiff.TotalMinutes);
                                        nightdiff.OTType = 2;
                                        nightdiff.OTTypeName = "10% NIGHT DIFFERENTIAL (10PM - 6AM) MON - SUN / HOLIDAY";
                                        param.Entries.Add(nightdiff);
                                    }

                                }
                            }
                            else
                            {


                                if (TimeSpan.TryParse(dtrmodel.TOTAL_WORKING_HOURS, out TimeSpan time))
                                {
                                    TimeSpan eightHours = TimeSpan.FromHours(8);
                                    var regOt = lastout - clockout;
                                    if (time > eightHours)
                                    {
                                        OvertimeEntryDto overtimeEntryDto = new OvertimeEntryDto();
                                        overtimeEntryDto.OTDate = param.OTDate;
                                        overtimeEntryDto.OTTimeFrom = clockout.ToString();
                                        overtimeEntryDto.OTTimeTo = lastout.ToString();
                                        overtimeEntryDto.TotalMinutes = (int)regOt.TotalMinutes;
                                        overtimeEntryDto.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)regOt.TotalMinutes);
                                        overtimeEntryDto.OTType = 1;
                                        overtimeEntryDto.OTTypeName = "125% REGULAR (AFTER OFFICE HRS. /MON - FRI / EXCEPT HOLIDAY";
                                        param.Entries.Add(overtimeEntryDto);


                                        if (lastout > nightDiffStartTime)
                                        {
                                            totalNightdiff = lastout - nightDiffStartTime;

                                            OvertimeEntryDto nightdiff = new OvertimeEntryDto();
                                            nightdiff.OTDate = param.OTDate;
                                            nightdiff.OTTimeFrom = clockout.ToString();
                                            nightdiff.OTTimeTo = lastout.ToString();
                                            nightdiff.TotalMinutes = (int)totalNightdiff.TotalMinutes;
                                            nightdiff.TotalHours = TimeHelper.ConvertMinutesToHHMM((int)totalNightdiff.TotalMinutes);
                                            nightdiff.OTType = 2;
                                            nightdiff.OTTypeName = "10% NIGHT DIFFERENTIAL (10PM - 6AM) MON - SUN / HOLIDAY";
                                            // otList.Add(nightdiff);
                                            param.Entries.Add(nightdiff);

                                        }
                                    }
                                }
                            }


                            return Json(new { success = true, data = param.Entries.ToList() });
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
    }
}
