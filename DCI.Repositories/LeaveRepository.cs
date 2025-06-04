using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections;
using System.Collections.Generic;

namespace DCI.Repositories
{
    public class LeaveRepository : ILeaveRepository, IDisposable
    {
        private DCIdbContext _dbContext;

        public LeaveRepository(DCIdbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<LeaveViewModel> GetAllLeave(LeaveViewModel param)
        {
            LeaveViewModel model = new LeaveViewModel();
            model.EmployeeId = param.EmployeeId;

            var leaveinfo = _dbContext.LeaveInfo.Where(x => x.EmployeeId == param.EmployeeId).FirstOrDefault();
            model.VLBalance = leaveinfo.VLBalance;
            model.SLBalance = leaveinfo.SLBalance;

            var leaveReqHeaderDbContext = _dbContext.LeaveRequestHeader.AsQueryable();

            model.LeaveRequestHeaderViewModel = (from lheader in _dbContext.LeaveRequestHeader
                                                 join lvtype in _dbContext.LeaveType
                                                 on lheader.LeaveTypeId equals lvtype.LeaveTypeId
                                                 join stat in _dbContext.Status
                                               on lheader.Status equals stat.StatusId
                                                 select new LeaveRequestHeaderViewModel
                                                 {
                                                     LeaveRequestHeaderId = lheader.LeaveRequestHeaderId,
                                                     EmployeeId = lheader.EmployeeId,
                                                     RequestNo = lheader.RequestNo,
                                                     DateFiled = lheader.DateFiled,
                                                     LeaveTypeId = lheader.LeaveTypeId,
                                                     LeaveName = lvtype.Name,
                                                     Status = lheader.Status,
                                                     StatusName = stat.StatusName,
                                                     Reason = lheader.Reason,
                                                     DateModified = lheader.DateModified,
                                                     ModifiedBy = lheader.ModifiedBy,
                                                     IsActive = lheader.IsActive,
                                                     NoofDays = _dbContext.LeaveRequestDetails
                                                                                     .Where(ld => ld.LeaveRequestHeaderId == lheader.LeaveRequestHeaderId)
                                                                                     .Sum(ld => (decimal?)ld.Amount) ?? 0,

                                                     LeaveRequestDetailViewModel = _dbContext.LeaveRequestDetails
                                                         .Where(ld => ld.LeaveRequestHeaderId == lheader.LeaveRequestHeaderId)
                                                         .Select(ld => new LeaveRequestDetailViewModel
                                                         {
                                                             LeaveRequestDetailId = ld.LeaveRequestDetailId,
                                                             LeaveRequestHeaderId = ld.LeaveRequestHeaderId,
                                                             LeaveDate = ld.LeaveDate,
                                                             Amount = ld.Amount,
                                                             IsActive = ld.IsActive
                                                         }).ToList()
                                                 }).ToList();



            var vlSummary = _dbContext.LeaveSummary
                    .FromSqlInterpolated($"EXEC sp_GetVacationLeaveBalance @EmployeeId = {param.EmployeeId},@Year = {2025}, @LeaveType =  'VL'  ")
                    .ToList();

            model.vlSummaries = vlSummary
               .Select(dtr => new LeaveSummaryViewModel
               {
                   EmployeeId = dtr.EmployeeId,
                   AsOf = dtr.AsOf,
                   BegBal = dtr.BegBal,
                   Credit = dtr.Credit,
                   Availed = dtr.Availed,
                   Monetized = dtr.Monetized,
                   EndBal = dtr.EndBal
               }).ToList();


            var slSummary = _dbContext.LeaveSummary
                   .FromSqlInterpolated($"EXEC sp_GetVacationLeaveBalance @EmployeeId = {param.EmployeeId},@Year = {2025}, @LeaveType =  'SL'  ")
                   .ToList();

            model.slSummaries = slSummary
               .Select(dtr => new LeaveSummaryViewModel
               {
                   EmployeeId = dtr.EmployeeId,
                   AsOf = dtr.AsOf,
                   BegBal = dtr.BegBal,
                   Credit = dtr.Credit,
                   Availed = dtr.Availed,
                   Monetized = dtr.Monetized,
                   EndBal = dtr.EndBal
               }).ToList();


            return model;
        }

    }
}
