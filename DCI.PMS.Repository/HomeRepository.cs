using DCI.Core.Common;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.PMS.Models.Entities;
using DCI.PMS.Models.ViewModel;
using DCI.PMS.Repository.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.PMS.Repository
{
    public class HomeRepository : IHomeRepository, IDisposable
    {
        private readonly DCIdbContext _dbContext;
        private readonly PMSdbContext _pmsdbContext;
        public HomeRepository(DCIdbContext context, PMSdbContext pmsContext)
        {
            this._dbContext = context;
            this._pmsdbContext = pmsContext;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
            _pmsdbContext.Dispose();
        }

        public async Task<PMSDashboardViewModel> GetDashboard()
        {
            PMSDashboardViewModel model = new PMSDashboardViewModel();

            var project = _pmsdbContext.Project.AsNoTracking().Where(x => x.IsActive == true).ToList();

 
            var _projectList =  await (from p in _pmsdbContext.Project.AsNoTracking()
                          join m in _pmsdbContext.Milestone.AsNoTracking() on p.ProjectCreationId equals m.ProjectCreationId                       
                          join c in _pmsdbContext.Client on p.ClientId equals c.ClientId
                          join s in _pmsdbContext.Status on m.Status equals s.StatusId
                          where p.IsActive == true
                          select new MilestoneViewModel
                          {
                              ProjectCreationId = p.ProjectCreationId,                            
                              ProjectName = p.ProjectName,                              
                              MilestoneName = m.MilestoneName, 
                              Percentage = m.Percentage,
                              IsActive = p.IsActive,
                            Status = m.Status,
                              StatusName = s.StatusName,
                              DateModified = m.DateModified
                          }).ToListAsync();

            model.ProjectList = _projectList;
            model.TotalProject = _projectList.Count();
            model.TotalNotStarted = _projectList.Where(x => x.Status == (int)EnumPMSStatus.NotStarted).Count();
            model.TotalInProgress = _projectList.Where(x => x.Status == (int)EnumPMSStatus.InProgress).Count();
            model.TotalCompleted = _projectList.Where(x => x.Status == (int)EnumPMSStatus.Completed).Count();

            return model;
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
                //using (var conn = new SqlConnection(_connectionString))
                //{
                //    await conn.OpenAsync();

                //    string sql = @"
                //    UPDATE Notification 
                //    SET MarkRead = 1 
                //    WHERE NotificationId = @NotificationId";

                //    using (var cmd = new SqlCommand(sql, conn))
                //    {
                //        cmd.Parameters.AddWithValue("@NotificationId", model.NotificationId);
                //        await cmd.ExecuteNonQueryAsync();
                //    }
                //}
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
