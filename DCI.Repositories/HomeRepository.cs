using DCI.Core.Common;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace DCI.Repositories
{
    public class HomeRepository : IHomeRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        private readonly string _connectionString;

        public HomeRepository(DCIdbContext context, IConfiguration configuration)
        {
            this._dbContext = context;
            _connectionString = configuration.GetConnectionString("DCIConnection");
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<DashboardViewModel> GetAllAnnouncement(DashboardViewModel model)
        {

            try
            {
               // DashboardViewModel model = new DashboardViewModel();                             

                model.AnnouncementList = (from announce in _dbContext.Announcement
                                          join usr in _dbContext.User on announce.CreatedBy equals usr.UserId 
                                          where announce.IsActive == true && announce.Status == (int)EnumStatus.Active
                                          select new AnnouncementViewModel
                                          {
                                              AnnouncementId = announce.AnnouncementId,
                                              Title = announce.Title,
                                              Date = announce.Date,
                                              Details = announce.Details,
                                              Status = announce.Status,
                                              CreatedBy = announce.CreatedBy,
                                              DateCreated = announce.DateCreated,
                                              DateModified = announce.DateModified,
                                              ModifiedBy = announce.ModifiedBy,
                                              IsActive = announce.IsActive,
                                              CreatedName = usr.Firstname + " " + usr.Lastname
                                          }).ToList();

                int _currentMonth = DateTime.Now.Month;

                model.BirthdayList = (from usr in _dbContext.Employee
                                      join wrkdtls in _dbContext.EmployeeWorkDetails on usr.EmployeeId equals wrkdtls.EmployeeId
                                      where usr.IsActive == true && usr.DateBirth.HasValue && usr.DateBirth.Value.Month == _currentMonth && wrkdtls.IsResigned == false
                                      select new BirthdayViewModel
                                      {
                                          EmployeeName = usr.Lastname + ", " + usr.Firstname,
                                          Birthdate = usr.DateBirth.Value.ToString("MMM dd")
                                      }).ToList();

                var currentEmail = _dbContext.User.Where(x => x.UserId == model.CurrentUserId).FirstOrDefault();

                // model.CurrentUserId
                var currentEmp = _dbContext.Employee.Where(x => x.Email == currentEmail.Email).FirstOrDefault();
                string _empNo = currentEmp != null ? currentEmp.EmployeeNo : string.Empty;

                var dailyTime = _dbContext.vw_AttendanceSummary.Where(x => x.DATE == DateTime.Today && x.EMPLOYEE_NO == _empNo).FirstOrDefault();
                model.FIRST_IN = dailyTime != null ? dailyTime.FIRST_IN : "--:--:--";
                model.LAST_OUT = dailyTime != null ? dailyTime.LAST_OUT : "--:--:--";

                return model;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return null;
        }

        public async Task<IList<NotificationViewModel>> GetAllNotification(NotificationViewModel model)
        {
            var query = await _dbContext.Notification
                .Where(notif => notif.IsActive == true && notif.AssignId == model.AssignId)
                .Select(notif => new NotificationViewModel
                {
                    NotificationId = notif.NotificationId,
                    Title = notif.Title,
                    Description = notif.Description,
                    ModuleId = notif.ModuleId,
                    TransactionId = notif.TransactionId,
                    AssignId = notif.AssignId,
                    URL = notif.URL,
                    MarkRead = notif.MarkRead,
                    CreatedBy = notif.CreatedBy,
                    DateCreated = notif.DateCreated,
                    DateNotification = notif.DateCreated.ToString("MMMM dd, yyyy h:mm tt"),
                    IsActive = notif.IsActive,
                })
                .ToListAsync();

            return query;
        }


        public async Task SaveNotification(NotificationViewModel model)
        {
            try
            {
                Notification entity = new Notification();
                entity.Title = model.Title;
                entity.Description = model.Description;
                entity.ModuleId = model.ModuleId;
                entity.TransactionId = model.TransactionId;
                entity.AssignId = model.AssignId;
                entity.URL = model.URL;
                entity.MarkRead = model.MarkRead;
                entity.DateCreated = DateTime.Now;
                entity.CreatedBy = model.CreatedBy;
                entity.IsActive = true;
                await _dbContext.Notification.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public async Task MarkAsRead(NotificationViewModel model)
        {
            try
            {
                using (var conn = new SqlConnection(_connectionString))
                {
                    await conn.OpenAsync();

                    string sql = @"
                    UPDATE Notification 
                    SET MarkRead = 1 
                    WHERE NotificationId = @NotificationId";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@NotificationId", model.NotificationId);
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

    }
}
