using DCI.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Serilog;
using DCI.Trigger;
using Microsoft.AspNetCore.Identity.UI.Services;



namespace DCI.Trigger
{
    public class OutboxProcessor
    {
        private readonly AppDbContext _sourceDb;
        private readonly DestinationDbContext _destDb;
        IEmailRepository _emailRepository;

        public OutboxProcessor(AppDbContext sourceDb, DestinationDbContext destDb  ,IEmailRepository emailRepository) 
        {
            _sourceDb = sourceDb;
            _destDb = destDb;
            _emailRepository = emailRepository;
        }

        public async Task ProcessPendingMessages()
        {
            try
            {

                var pending = await _sourceDb.OutboxQueue
                .Where(x => x.Status == "Pending")
                .Take(10)
                .ToListAsync();

                foreach (var message in pending)
                {
                    try
                    {
                        var log = JsonConvert.DeserializeObject<TblRawLog>(message.Payload);
                        _destDb.TblRawLogs.Add(log);
                        await _destDb.SaveChangesAsync();

                        message.Status = "Processed";
                        message.LastError = null;

                       
                        if(log.EMPLOYEE_ID != null)
                        {
                            var emp = _destDb.Employee.FirstOrDefault(x => x.EmployeeNo == log.EMPLOYEE_ID);
                            var user = _destDb.User.FirstOrDefault(x => x.EmployeeId == emp.EmployeeId );
                            if (user != null && user.Email != null && user.EmailBiometricsNotification == true)
                            {
                                BiometricViewModel viewModel = new BiometricViewModel();
                                viewModel.Fullname = log.FULL_NAME;
                                viewModel.Email = user.Email;
                                viewModel.DateTimeInOut = log.DATE_TIME;
                                await _emailRepository.SendEmailBiometricsNotification (viewModel);
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

                    await _sourceDb.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
