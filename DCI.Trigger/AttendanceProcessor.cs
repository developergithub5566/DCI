using DCI.Core.Common;
using DCI.Models.Configuration;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using Hangfire.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Trigger
{
    public class AttendanceProcessor
    {
        private readonly AppDbContext _sourceDb;
        private readonly DestinationDbContext _destDb;
        IEmailRepository _emailRepository;
        private readonly IOptions<APIConfigModel> _apiconfig;

        public AttendanceProcessor(AppDbContext sourceDb, DestinationDbContext destDb, IEmailRepository emailRepository, IOptions<APIConfigModel> apiconfig)
        {
            _sourceDb = sourceDb;
            _destDb = destDb;
            _emailRepository = emailRepository;
            _apiconfig = apiconfig;
        }

        public async Task AttendanceConfirmationProcessor()
        {

            List<DailyTimeRecordViewModel> attendance = new List<DailyTimeRecordViewModel>();

            try
            {
                using (var _httpclient = new HttpClient())
                {
                    DailyTimeRecordViewModel param = new DailyTimeRecordViewModel();
                    param.DATE = DateTime.Now;
                     //param.DateFilter = DateTime.Parse("2025-11-13");
                    param.ScopeTypeJobRecurring = (int)EnumScopeTypeJobRecurring.DAILY;
                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllDTRByDate");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();
                    if (response.IsSuccessStatusCode == true)
                    {
                        attendance = JsonConvert.DeserializeObject<List<DailyTimeRecordViewModel>>(responseBody)!;
                       // attendance = attendance.Where(x => x.EMPLOYEE_NO == "080349").ToList();
                    }
                }
             
                if (attendance is not null && attendance.Count() > 5) // temporary validation for holiday 
                {
                    Log.Information("Attendance records retrieved successfully from Biometrics API.");
                    var userlist = _destDb.User.Where(x => x.IsActive).ToList();
                   // userlist = userlist.Where(x => x.EmployeeNo == "080349").ToList();

                    foreach (var emp in userlist)
                    {
                        string emailSubject = string.Empty;
                        string remarks = string.Empty;

                        var biometrics = attendance.Where(x => x.EMPLOYEE_NO == emp.EmployeeNo).FirstOrDefault();

                        if (biometrics is not null && biometrics.ID > 0)
                        {   
                            bool hasFirstIn = TimeSpan.TryParse(biometrics.FIRST_IN, out TimeSpan firstin);
                            bool hasLastOut = TimeSpan.TryParse(biometrics.LAST_OUT, out TimeSpan lastout);
                            bool hasClockOut = TimeSpan.TryParse(biometrics.CLOCK_OUT, out TimeSpan clockout);

                            if (hasFirstIn && hasLastOut && firstin == TimeSpan.Zero && lastout == TimeSpan.Zero)
                            {
                                emailSubject = "No Attendance";
                                remarks = "Please file a Leave or Official Business request.";
                            }
                            else if (hasFirstIn && firstin.TotalMinutes > 0 &&  lastout == TimeSpan.Zero)
                            {
                                // No Time Out
                                emailSubject = "No Time Out";
                                remarks = "Please file a Half-Day Leave, Official Business, or DTR Adjustment request.";
                            }
                            else if (!hasFirstIn || firstin == TimeSpan.Zero)
                            {
                                // No Time In
                                emailSubject = "No Time In";
                                remarks = "Please file a Half-Day Leave, Official Business, or DTR Adjustment request.";
                            }
                            else if (hasLastOut && hasClockOut && lastout < clockout)
                            {
                                // Logged out early
                                emailSubject = "Half Day or OB";
                                remarks = "Please file a Half-Day Leave, Official Business, or DTR Adjustment request.";
                            }                         
                        }
                        else
                        {
                            emailSubject = "No Attendance";
                            remarks = "Please file a Leave or Official Business request.";
                        }

                        if (emailSubject != string.Empty)
                        {
                            BiometricViewModel viewModel = new BiometricViewModel();
                            viewModel.UserId = emp.UserId;
                            viewModel.EmployeeId = emp.EmployeeId ?? 0;
                            viewModel.EmployeeNo = emp.EmployeeNo;
                            viewModel.Fullname = emp.Fullname;
                            viewModel.Email = emp.Email;
                            viewModel.STATUS_STRING = emailSubject;
                            viewModel.REMARKS = remarks;
                            viewModel.DATE = biometrics.DATE.ToString("MMMM dd, yyyy");
                            viewModel.FIRST_IN = string.IsNullOrEmpty(biometrics.FIRST_IN) ? "N/A" : biometrics.FIRST_IN;
                            viewModel.LAST_OUT = string.IsNullOrEmpty(biometrics.LAST_OUT) ? "N/A" : biometrics.LAST_OUT;

                            await _emailRepository.SendEmailAttendanceConfirmationNotification(viewModel);
                        }
                    }
                }

            }
            catch (HttpRequestException httpEx)
            {
                Log.Information("AttendanceConfirmationProcessor...");
                Log.Information("Biometrics API call failed...");
                Log.Error(httpEx.ToString());
            }
            catch (JsonException jsonEx)
            {
                // JSON SERIALIZATION / DESERIALIZATION ERRORS
                Log.Error(jsonEx, "Error parsing Biometrics API response.");
            }
            catch (SmtpException smtpEx)
            {
                // EMAIL SENDING ERRORS
                Log.Error(smtpEx, "Failed to send Biometrics email notification.");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.Information("Total Records: " + attendance?.Count().ToString());
                Log.Information("Attendance confirmation process completed successfully.");
            }




        }


        public async Task AttendanceConfirmationProcessorMonthly()
        {

            List<DailyTimeRecordViewModel> attendance = new List<DailyTimeRecordViewModel>();

            try
            {
                //DateTime today = DateTime.Today;

                //// Check if tomorrow is next month
                //bool isLastDay = today.AddDays(1).Month != today.Month;

                //if (!isLastDay)
                //    return;


                DailyTimeRecordViewModel param = new DailyTimeRecordViewModel();

                using (var _httpclient = new HttpClient())
                {

                    //ram.DATE = DateTime.Now;
                    param.DATE = DateTime.Parse("2025-10-02");
                    param.ScopeTypeJobRecurring = (int)EnumScopeTypeJobRecurring.MONTHLY;


                    //if(DateTime.Now.Day < 5) //incase of manual execution during beginning of the month, get last month attendance
                    //{
                    //    param.DATE = DateTime.Now.AddMonths(-1);
                    //}


                    var stringContent = new StringContent(JsonConvert.SerializeObject(param), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/DailyTimeRecord/GetAllDTRByDate");
                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    var responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                        attendance = JsonConvert.DeserializeObject<List<DailyTimeRecordViewModel>>(responseBody)!;
                    }
                }
                Log.Information("Attendance records retrieved successfully from Biometrics API.");

                var userlist = _destDb.User.Where(x => x.IsActive && x.EmailAttendanceConfirmation == true).ToList();

                foreach (var emp in userlist)
                {
                    var result = GenerateAttendanceTable(attendance, emp.EmployeeNo, param.DATE.Year, param.DATE.Month);

                    string attendanceTable = BuildAttendanceHtmlTable(result, emp.EmployeeNo, emp.Fullname, param.DATE.Year, param.DATE.Month);


                    BiometricViewModel viewModel = new BiometricViewModel();
                    viewModel.UserId = emp.UserId;
                    viewModel.EmployeeId = emp.EmployeeId ?? 0;
                    viewModel.EmployeeNo = emp.EmployeeNo;
                    viewModel.Fullname = emp.Fullname;
                    viewModel.Email = emp.Email;     
                    viewModel.BODYTABLE = attendanceTable;

                    await _emailRepository.SendEmailAttendanceConfirmationNotificationMonthly(viewModel);
                }


            }
            catch (HttpRequestException httpEx)
            {
                Log.Information("AttendanceConfirmationProcessor...");
                Log.Information("Biometrics API call failed...");
                Log.Error(httpEx.ToString());
            }
            catch (JsonException jsonEx)
            {
                // JSON SERIALIZATION / DESERIALIZATION ERRORS
                Log.Error(jsonEx, "Error parsing Biometrics API response.");
            }
            catch (SmtpException smtpEx)
            {
                // EMAIL SENDING ERRORS
                Log.Error(smtpEx, "Failed to send Biometrics email notification.");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.Information("Total Records: " + attendance?.Count().ToString());
                Log.Information("Attendance confirmation process completed successfully.");
            }
        }

        public class AttendanceDisplay
        {
            public DateTime Date { get; set; }
            public string DayType { get; set; }
            public string FIRST_IN { get; set; }
            public string LAST_OUT { get; set; }
        }

        public static List<AttendanceDisplay> GenerateAttendanceTable(List<DailyTimeRecordViewModel> biometrics, string employeeNo, int year, int month)
        {
            var daysInMonth = DateTime.DaysInMonth(year, month);
            var result = new List<AttendanceDisplay>();

            for (int day = 1; day <= daysInMonth; day++)
            {
                var currentDate = new DateTime(year, month, day);
                var record = biometrics.FirstOrDefault(a => a.DATE.Date == currentDate.Date && a.EMPLOYEE_NO == employeeNo);

                if (currentDate.DayOfWeek == DayOfWeek.Saturday || currentDate.DayOfWeek == DayOfWeek.Sunday)
                {
                    result.Add(new AttendanceDisplay
                    {
                        Date = currentDate,
                        DayType = "Weekend",
                        FIRST_IN = "-",
                        LAST_OUT = "-"
                    });
                }
                else if (record == null)
                {
                    result.Add(new AttendanceDisplay
                    {
                        Date = currentDate,
                        DayType = "No attendance",
                        FIRST_IN = "-",
                        LAST_OUT = "-"
                    });
                }
                else
                {
                    result.Add(new AttendanceDisplay
                    {
                        Date = currentDate,
                        DayType = record.SOURCE,
                        FIRST_IN = record.FIRST_IN?.ToString(),
                        LAST_OUT = record.LAST_OUT?.ToString()
                    });
                }
            }

            return result;
        }

        public static string BuildAttendanceHtmlTable(List<AttendanceDisplay> result, string employeeNo, string Name, int year, int month)
        {
            var sb = new StringBuilder();

            // Basic styles — move to CSS file if using in web app
            //sb.AppendLine("<style>");
            //sb.AppendLine(".att-table { border-collapse: collapse; width: 60%;  font-size:8px; font-family: Arial, sans-serif; }");
            //sb.AppendLine(".att-table, .att-table th, .att-table td { border: 1px solid #000; }"); // added visible border
            //sb.AppendLine(".att-table th, .att-table td { padding: 8px; text-align: left; }");
            //sb.AppendLine(".att-table th { background: #f8f9fa; font-weight: 600; }");
            //sb.AppendLine(".att-weekend { background: #f2f2f2; }");     // weekend grey
            //sb.AppendLine(".att-norecord { background: #fff3f3; }");    // missing record light red
            //sb.AppendLine(".att-present { background: #ffffff; }");
            //sb.AppendLine("</style>");

            // sb.AppendLine($"<div><strong>Attendance for {CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month)} {year}</strong></div>");
            sb.AppendLine("<table style='width:40%; border-collapse:collapse; font-family:Arial, sans-serif; font-size:8px; border:1px solid #000;'>"); 
            //  sb.AppendLine("<table class='att-table'>");
            sb.AppendLine("<thead>");
            sb.AppendLine("<tr style='border:1px solid #000; background-color:#f8f9fa;'>");  
            sb.AppendLine("<th style='border:1px solid #000; padding:6px;'>Employee No</th>");
            sb.AppendLine("<th style='border:1px solid #000; padding:6px;'>Name</th>");
            sb.AppendLine("<th style='border:1px solid #000; padding:6px;'>Date</th>");
            sb.AppendLine("<th style='border:1px solid #000; padding:6px;'>Time In</th>");
            sb.AppendLine("<th style='border:1px solid #000; padding:6px;'>Time Out</th>");
            sb.AppendLine("<th style='border:1px solid #000; padding:6px;'>Source</th>");
            sb.AppendLine("</tr>");
            sb.AppendLine("</thead>");
            sb.AppendLine("<tbody>");

            foreach (var item in result)
            {
                // choose CSS class
                string cssClass = item.DayType == "Weekend" ? "att-weekend"
                               : item.DayType == "No Record" ? "att-norecord"
                               : "att-present";

                // safe encoding
                string dateText = WebUtility.HtmlEncode(item.Date.ToString("yyyy-MM-dd"));
                // string dayText = WebUtility.HtmlEncode(item.Date.ToString("dddd"));
                string statusText = WebUtility.HtmlEncode(item.DayType ?? "-");
                string timeIn = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(item.FIRST_IN) ? "-" : item.FIRST_IN);
                string timeOut = WebUtility.HtmlEncode(string.IsNullOrWhiteSpace(item.LAST_OUT) ? "-" : item.LAST_OUT);

                sb.AppendLine($"<tr class='{cssClass}'>");
                sb.AppendLine($"  <td style='border:1px solid #000; padding:6px;'>{employeeNo}</td>");
                sb.AppendLine($" <td style='border:1px solid #000; padding:6px;'>{Name}</td>");
                sb.AppendLine($" <td style='border:1px solid #000; padding:6px;'>{dateText}</td>");
                //sb.AppendLine($"  <td>{dayText}</td>");

                sb.AppendLine($"  <td style='border:1px solid #000; padding:6px;'>{timeIn}</td>");
                sb.AppendLine($"  <td style='border:1px solid #000; padding:6px;'>{timeOut}</td>");
                sb.AppendLine($"  <td style='border:1px solid #000; padding:6px;'>{statusText}</td>");
                sb.AppendLine("</tr>");
            }

            sb.AppendLine("</tbody>");
            sb.AppendLine("</table>");

            return sb.ToString();
        }

    }
}
