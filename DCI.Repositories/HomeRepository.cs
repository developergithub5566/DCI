using DCI.Core.Common;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
    public class HomeRepository : IHomeRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        public HomeRepository(DCIdbContext context)
        {
            this._dbContext = context;
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
                                      where usr.IsActive == true && usr.DateBirth.HasValue && usr.DateBirth.Value.Month == _currentMonth
                                      select new BirthdayViewModel
                                      {
                                          EmployeeName = usr.Lastname + ", " + usr.Firstname,
                                          Birthdate = usr.DateBirth.Value.ToString("MMM dd")
                                      }).ToList();

                var dailyTime = _dbContext.vw_AttendanceSummary.Where(x => x.DATE == DateTime.Today).FirstOrDefault();
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


    }
}
