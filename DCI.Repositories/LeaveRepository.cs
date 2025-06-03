using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
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
            model.SLBalance  = leaveinfo.SLBalance;

            var leaveReqHeaderDbContext = _dbContext.LeaveRequestHeader.AsQueryable();

            //model.LeaveRequestHeaderViewModel = (from lheader in leaveReqHeaderDbContext
            //                                     select new LeaveRequestHeaderViewModel
            //                                     {
            //                                         LeaveRequestHeaderId = lheader.LeaveRequestHeaderId,
            //                                         EmployeeId = lheader.EmployeeId,
            //                                         RequestNo = lheader.RequestNo,
            //                                         DateFiled = lheader.DateFiled,
            //                                         LeaveTypeId = lheader.LeaveTypeId                                                    
            //                                     }).ToList();

            //model.LeaveRequestHeaderViewModel = _dbContext.LeaveRequestHeader
            //                                    .Where(h => h.IsActive)
            //                                    .Select(h => new LeaveRequestHeaderViewModel
            //                                    {
            //                                        LeaveRequestHeaderId = h.LeaveRequestHeaderId,
            //                                        EmployeeId = h.EmployeeId,
            //                                        RequestNo = h.RequestNo,
            //                                        DateFiled = h.DateFiled,
            //                                        LeaveTypeId = h.LeaveTypeId,
            //                                        Reason = h.Reason,
            //                                        DateModified = h.DateModified,
            //                                        ModifiedBy = h.ModifiedBy,
            //                                        IsActive = h.IsActive,

            //                                        LeaveRequestDetailViewModel = h.LeaveRequestDetails
            //                                            .Where(d => d.IsActive)
            //                                            .Select(d => new LeaveRequestDetailViewModel
            //                                            {
            //                                                LeaveRequestDetailId = d.LeaveRequestDetailId,
            //                                                LeaveRequestHeaderId = d.LeaveRequestHeaderId,
            //                                                LeaveDate = d.LeaveDate,
            //                                                Amount = d.Amount,
            //                                                IsActive = d.IsActive
            //                                            }).ToList()
            //                                    }).ToList();




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
