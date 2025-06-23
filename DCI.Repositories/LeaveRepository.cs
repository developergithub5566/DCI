using DCI.Core.Common;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection.PortableExecutable;

namespace DCI.Repositories
{
    public class LeaveRepository : ILeaveRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        private readonly IEmailRepository _emailRepository;

        public LeaveRepository(DCIdbContext dbContext, IEmailRepository emailRepository)
        {
            _dbContext = dbContext;
            _emailRepository = emailRepository;
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

            model.LeaveRequestHeaderList = (from lheader in _dbContext.LeaveRequestHeader
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
                                                     LeaveName = lvtype.Description,
                                                     Status = lheader.Status,
                                                     StatusName = stat.StatusName,
                                                     Reason = lheader.Reason,
                                                     DateModified = lheader.DateModified,
                                                     ModifiedBy = lheader.ModifiedBy,
                                                     IsActive = lheader.IsActive,
                                                     NoofDays = _dbContext.LeaveRequestDetails
                                                                                     .Where(ld => ld.LeaveRequestHeaderId == lheader.LeaveRequestHeaderId)
                                                                                     .Sum(ld => (decimal?)ld.Amount) ?? 0,

                                                     LeaveRequestDetailList = _dbContext.LeaveRequestDetails
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

        public async Task<LeaveViewModel> RequestLeave(LeaveViewModel param)
        {
            LeaveViewModel model = new LeaveViewModel();
          
         

            var query = from lheader in _dbContext.LeaveRequestHeader
                        join lvtype in _dbContext.LeaveType on lheader.LeaveTypeId equals lvtype.LeaveTypeId
                        where lheader.LeaveRequestHeaderId == param.LeaveRequestHeaderId
                        select new LeaveRequestHeaderViewModel
                        {
                            LeaveRequestHeaderId = lheader.LeaveRequestHeaderId,
                            EmployeeId = lheader.EmployeeId,
                            RequestNo = lheader.RequestNo,
                            DateFiled = lheader.DateFiled,
                            LeaveTypeId = lheader.LeaveTypeId,
                            LeaveName = lvtype.Description,
                            Reason = lheader.Reason,
                            NoofDays = lheader.NoOfDays,
                            Status = lheader.Status
                        };

            model.LeaveRequestHeader = query.FirstOrDefault();

            if (param.LeaveRequestHeaderId == 0)
            {
                model = new LeaveViewModel();
            }
            else
            {
                //var leaveDtl = (from dtl in _dbContext.LeaveRequestDetails
                //            where dtl.LeaveRequestHeaderId == param.LeaveRequestHeaderId
                //            select new LeaveRequestDetailViewModel
                //            {
                //                LeaveRequestHeaderId = dtl.LeaveRequestHeaderId,
                //                LeaveDate = dtl.LeaveDate,
                //                Amount = dtl.Amount                              
                //            }).ToList();

                var leaveDates = _dbContext.LeaveRequestDetails
                                 .Where(dtl => dtl.LeaveRequestHeaderId == param.LeaveRequestHeaderId)
                                 .Select(dtl => dtl.LeaveDate)
                                 .ToList();
                model.LeaveDateList = leaveDates;
               // model.LeaveRequestHeader.LeaveRequestDetailList = leaveDtl;
               // model.LeaveRequestHeader.LeaveRequestDetailList = _dbContext.LeaveRequestDetails.Where(x => x.LeaveRequestHeaderId == param.LeaveRequestHeaderId).ToList();
            }

            var leavetypeList = _dbContext.LeaveType.Where(x => x.IsActive == true).AsQueryable().ToList();
            model.LeaveRequestHeader.LeaveTypeList = leavetypeList.Count() > 0 ? leavetypeList : null;

            return model;
        }

        //public async Task<LeaveViewModel> RequestLeave(LeaveViewModel param)
        //{
        //    LeaveViewModel model = new LeaveViewModel();
        //    param.LeaveRequestHeaderId = 4;

        //    var leaveList = (from lheader in _dbContext.LeaveRequestHeader
        //                 join lvtype in _dbContext.LeaveType on lheader.LeaveTypeId equals lvtype.LeaveTypeId
        //                  where lheader.LeaveRequestHeaderId == param.LeaveRequestHeaderId
        //                  select new LeaveRequestHeaderViewModel
        //                  {
        //                      LeaveRequestHeaderId = lheader.LeaveRequestHeaderId,
        //                      RequestNo = lheader.RequestNo,
        //                      DateFiled = lheader.DateFiled,
        //                      LeaveTypeId = lheader.LeaveTypeId,
        //                      LeaveName = lvtype.Description                           
        //                  }).ToList() ?? new List<LeaveRequestHeaderViewModel>();
        //    model.LeaveRequestHeaderList = leaveList;

        //    var leavetypeList = _dbContext.LeaveType.Where(x => x.IsActive == true).AsQueryable().ToList();
        //    model.LeaveTypeList = leavetypeList.Count() > 0 ? leavetypeList : null;

        //    return model;
        //}

        public async Task<(int statuscode, string message)> SaveLeave(LeaveFormViewModel param)
        {
            LeaveViewModel model = new LeaveViewModel();

            try
            {
                if (model.LeaveTypeId == 0)
                {
                    LeaveRequestHeader entity = new LeaveRequestHeader();
                    entity.EmployeeId = param.EmployeeId;
                    entity.RequestNo = await GenereteRequestNo();
                    entity.DateFiled = DateTime.Now;
                    entity.LeaveTypeId = param.LeaveTypeId;
                    entity.Status = (int)EnumStatus.ForApproval;
                    entity.Reason = param.Reason;
                    entity.NoOfDays = param.NoOfDays;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    await _dbContext.LeaveRequestHeader.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();                

                    int count = param.SelectedDateList.Count();

                    foreach (var date in param.SelectedDateList)
                    {   
                        LeaveRequestDetails entityDtl = new LeaveRequestDetails();
                        entityDtl.LeaveRequestHeaderId = entity.LeaveRequestHeaderId;
                        entityDtl.LeaveDate = date;
                        entityDtl.Amount = param.NoOfDays / count;
                        entityDtl.IsActive = true;
                        await _dbContext.LeaveRequestDetails.AddAsync(entityDtl);
                        await _dbContext.SaveChangesAsync();
                    }

                    var workdtls = _dbContext.EmployeeWorkDetails.Where(x => x.EmployeeId == param.EmployeeId).FirstOrDefault();
                    var dept = _dbContext.Department.Where(x => x.DepartmentId == workdtls.DepartmentId).FirstOrDefault();
                    model.ApproverId = dept.ApproverId;
                    model.LeaveRequestHeader.Status = entity.Status;
                    model.LeaveRequestHeader.RequestNo = entity.RequestNo;
                   await _emailRepository.SendToApproval(model);

                    return (StatusCodes.Status200OK, "Successfully saved");
                }              
                return (StatusCodes.Status200OK, "Successfully updated");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return (StatusCodes.Status406NotAcceptable, ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }           
        }

        private async Task<string> GenereteRequestNo()
        {

            try
            {     
                var _leaveContext = await _dbContext.LeaveRequestHeader
                                                .Where(x => x.IsActive == true)
                                                .AsQueryable()
                                                .ToListAsync();
    

                int totalrecords = _leaveContext.Count() + 1;
                string version = "0";
                string finalSetRecords = GetFormattedRecord(totalrecords);
                // DateTime now = DateTime.Now;
                string yearMonth = DateTime.Now.ToString("yyyyMM");
                string req = "REQ";
         
                return $"{req}-{yearMonth}-{finalSetRecords}";
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return string.Empty;
        }

        private string GetFormattedRecord(int totalRecords)
        {        
            int setA = totalRecords % 1000;
            int setB = totalRecords / 1000;
            string formattedA = setA.ToString("D4");
            string formattedB = setB.ToString("D4");
            return $"{formattedA}";
        }
    }
}
