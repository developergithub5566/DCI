using DCI.Data;
using DCI.PMS.Models.ViewModel;
using DCI.PMS.Repository.Interface;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Serilog;
using System.Data;

namespace DCI.PMS.Repository
{
    public class TrackerRepository : ITrackerRepository, IDisposable
    {
        private readonly IOptions<DCI.Models.ViewModel.FileModel> _fileconfig;
        private readonly DCIdbContext _dbContext;
        private readonly PMSdbContext _pmsdbContext;
        private readonly IDbConnection _connection;
        public TrackerRepository(DCIdbContext context, PMSdbContext pmsContext, IOptions<DCI.Models.ViewModel.FileModel> fileconfig, IConfiguration configuration)
        {
            this._dbContext = context;
            this._pmsdbContext = pmsContext;
            this._fileconfig = fileconfig;
            // this._connection = connection;

            var connStr = configuration.GetConnectionString("DCIConnectionPMS") ?? throw new InvalidOperationException("Connection string not found.");
            _connection = new SqlConnection(connStr);


        }
        public void Dispose()
        {
            _dbContext.Dispose();
            _pmsdbContext.Dispose();
        }

        public async Task<IList<TrackerViewModel>> GetAllTracker()
        {
            try
            {
                var users = await _dbContext.User
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .Select(u => new
                                        {
                                            u.UserId,
                                            u.Fullname
                                        })
                                        .ToListAsync();

                var projects = await _pmsdbContext.Project
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .Select(p => new
                                        {
                                            p.ProjectCreationId,
                                            p.ProjectNo,
                                            p.ProjectName,
                                            p.ClientId,
                                            p.NOADate,
                                            p.NTPDate,
                                            p.MOADate,
                                            p.ProjectDuration,
                                            p.ProjectCost,
                                            p.ModeOfPayment,
                                            p.Status,
                                            p.CreatedBy,
                                            p.IsActive
                                        })
                                        .ToListAsync();

                var status = await _pmsdbContext.Status
                                       .AsNoTracking()
                                       .Where(p => p.IsActive)
                                       .Select(s => new
                                       {
                                           s.StatusId,
                                           s.StatusName
                                       })
                                       .ToListAsync();

                var _clientList = await _pmsdbContext.Client
                      .AsNoTracking()
                      .Where(c => c.IsActive)
                      .Select(c => new ClientViewModel
                      {
                          ClientId = c.ClientId,
                          ClientName = c.ClientName,
                          Description = c.Description,
                          DateCreated = c.DateCreated,
                          CreatedBy = c.CreatedBy,
                          IsActive = c.IsActive
                      })
                      .ToListAsync();


                var _milestoneList = await _pmsdbContext.Milestone
                 .AsNoTracking()
                 .Where(c => c.IsActive)
                 .Select(c => new MilestoneViewModel
                 {
                     MileStoneId = c.MileStoneId,
                     ProjectCreationId = c.ProjectCreationId,
                     MilestoneName = c.MilestoneName,
                     Percentage = c.Percentage,
                     TargetCompletedDate = c.TargetCompletedDate,
                     TargetCompletedDateString = c.TargetCompletedDate.HasValue ? c.TargetCompletedDate.Value.ToString("MM/dd/yyyy") : null,
                     ActualCompletionDate = c.ActualCompletionDate,
                     ActualCompletionDateString = c.ActualCompletionDate.HasValue ? c.ActualCompletionDate.Value.ToString("MM/dd/yyyy") : null,
                     PaymentStatus = c.PaymentStatus,
                     PaymentStatusName = c.PaymentStatus == 1 ? "Paid" : "Unpaid",
                     Status = c.Status,
                     DateCreated = c.DateCreated,
                     CreatedBy = c.CreatedBy,
                     DateModified = c.DateModified,
                     ModifiedBy = c.ModifiedBy,
                     IsActive = c.IsActive,
                     Remarks = c.Remarks
                 })
                 .ToListAsync();


                var result = from m in _milestoneList
                             join p in projects on m.ProjectCreationId equals p.ProjectCreationId
                             join u in users on p.CreatedBy equals u.UserId
                             join s in status on p.Status equals s.StatusId
                             join c in _clientList on p.ClientId equals c.ClientId
                             select new TrackerViewModel
                             {
                                 MileStoneId = m.MileStoneId,
                                 ProjectCreationId = p.ProjectCreationId,
                                 MilestoneName = m.MilestoneName,
                                 Percentage = m.Percentage,
                                 TargetCompletedDate = m.TargetCompletedDate,
                                 ActualCompletionDate = m.ActualCompletionDate,
                                 PaymentStatus = m.PaymentStatus,
                                 PaymentStatusName = m.PaymentStatusName,
                                 Status = m.Status,
                                 DateCreated = m.DateCreated,
                                 MilestoneOwnerId = m.CreatedBy,
                                 MilestoneOwnerName = u.Fullname,
                                 DateModified = m.DateModified,
                                 ModifiedBy = m.ModifiedBy,
                                 IsActive = p.IsActive,                               
                                 StatusName = s.StatusName,
                                 ProjectOwnerId = p.CreatedBy,
                                 ProjectOwnerName = u.Fullname,
                             };

                return result.ToList();
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
