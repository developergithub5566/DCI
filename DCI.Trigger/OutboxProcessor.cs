using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System.Text;



namespace DCI.Trigger
{
    public class OutboxProcessor
    {
        private readonly AppDbContext _sourceDb;
        private readonly DestinationDbContext _destDb;
        IEmailRepository _emailRepository;
        private readonly IOptions<APIConfigModel> _apiconfig;

        public OutboxProcessor(AppDbContext sourceDb, DestinationDbContext destDb, IEmailRepository emailRepository, IOptions<APIConfigModel> apiconfig)
        {
            _sourceDb = sourceDb;
            _destDb = destDb;
            _emailRepository = emailRepository;
            _apiconfig = apiconfig;
        }

        public async Task ProcessPendingMessages()
        {
            var responseBody = string.Empty;
            var response = new HttpResponseMessage();
            List<OutboxMessage> pending = new List<OutboxMessage>();

            try
            {  
                using (var _httpclient = new HttpClient())
                {                 
                    response = await _httpclient.GetAsync(_apiconfig.Value.TriggerAPIConnection + "api/Attendance/GetAllAttendanceLogs");
                    responseBody = await response.Content.ReadAsStringAsync();
                    pending = JsonConvert.DeserializeObject<List<OutboxMessage>>(responseBody);               
                }
            }
            catch (Exception ex)
            {
                Log.Information("Biometrics API call failed...");
                Log.Error(ex.ToString());
            }


            if (pending != null && pending.Count > 0)
            {
                 Log.Information("Biometrics API call succeeded...");
                 Log.Information("Total Records: " + pending?.Count().ToString());

                foreach (var message in pending)
                {
                    OutboxMessageViewModel outbox = new OutboxMessageViewModel();
                    try
                    {
                        var log = JsonConvert.DeserializeObject<TblRawLog>(message.Payload);
                        _destDb.TblRawLogs.Add(log);
                        await _destDb.SaveChangesAsync();


                        outbox.Status = "Processed";
                        outbox.LastError = null;


                        if (log.EMPLOYEE_ID != null)
                        {

                            var user = _destDb.User.FirstOrDefault(x => x.EmployeeNo == log.EMPLOYEE_ID);
                            if (user != null && user.Email != null && user.EmailBiometricsNotification == true)
                            {
                                BiometricViewModel viewModel = new BiometricViewModel();
                                viewModel.Fullname = log.FULL_NAME;
                                viewModel.Email = user.Email;
                                viewModel.DateTimeInOut = log.DATE_TIME;
                                await _emailRepository.SendEmailBiometricsNotification(viewModel);
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        message.RetryCount++;
                        message.LastError = ex.Message;

                        if (message.RetryCount >= 5)
                            message.Status = "Failed";
                    }
                    outbox.Id = message.Id;

                    try
                    {
                        using (var _httpclient = new HttpClient())
                        {                       
                            Log.Information("Update Attendance Logs Status..." + outbox.Payload.ToString());

                            var stringContentOutbox = new StringContent(JsonConvert.SerializeObject(outbox), Encoding.UTF8, "application/json");
                            var requestOutbox = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.TriggerAPIConnection + "api/Attendance/UpdateOutBoxQueueStatus");
                            requestOutbox.Content = stringContentOutbox;
                            var responseOutbox = await _httpclient.SendAsync(requestOutbox);
                            var responseBodyOutbox = await response.Content.ReadAsStringAsync();

                            Log.Information("Biometrics API Update Succeeded...");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Information("Biometrics API update failed...");
                        //Log.Information("api/Attendance/UpdateOutBoxQueueStatus...");
                        Log.Error(ex.ToString());
                    }

                }
                Log.Information("Biometrics process completed successfully.");
            }         

        }
    }
}
