using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using Hangfire.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
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
                   // param.DATE = DateTime.Parse("2025-10-02");
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
                // TimeSpan eightHours = TimeSpan.FromHours(8); //8HRs
                if (attendance is not null && attendance.Count() > 5) // temporary validation for holiday 
                {
                    Log.Information("Attendance records retrieved successfully from Biometrics API.");


                    var userlist = _destDb.User.Where(x => x.IsActive && x.EmailAttendanceConfirmation == true).ToList();

                    foreach (var emp in userlist)
                    {
                        string emailSubject = string.Empty;
                        string remarks = string.Empty;

                        var biometrics = attendance.Where(x => x.EMPLOYEE_NO == emp.EmployeeNo).FirstOrDefault();

                        if (biometrics is not null && biometrics.ID > 0)
                        {
                            TimeSpan firstin = new TimeSpan();
                            TimeSpan lastout = new TimeSpan();
                            TimeSpan clockout = new TimeSpan();

                            firstin = TimeSpan.Parse(biometrics.FIRST_IN);
                            lastout = TimeSpan.Parse(biometrics.LAST_OUT);
                            clockout = TimeSpan.Parse(biometrics.CLOCK_OUT);

                            if (firstin.TotalMinutes > 0 && lastout == TimeSpan.Zero)
                            {
                                emailSubject = "No Time Out";
                                remarks = "Please file a Half-Day Leave, Official Business, or DTR Adjustment request.";
                            }
                            else if (firstin > clockout)
                            {
                                emailSubject = "No Time In";
                                remarks = "Please file a Half-Day Leave, Official Business, or DTR Adjustment request.";
                            }
                            else if (lastout < clockout)
                            {
                                emailSubject = "Half day or OB";
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
    }
}
