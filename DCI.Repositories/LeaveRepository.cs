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
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DCI.Repositories
{
    public class LeaveRepository : ILeaveRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        private readonly IEmailRepository _emailRepository;
        private readonly IHomeRepository _homeRepository;

        public LeaveRepository(DCIdbContext dbContext, IEmailRepository emailRepository, IHomeRepository homeRepository)
        {
            _dbContext = dbContext;
            _emailRepository = emailRepository;
            _homeRepository = homeRepository;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<LeaveViewModel> GetAllLeave(LeaveViewModel param)
        {
            LeaveViewModel model = new LeaveViewModel();
            model.EmployeeId = param.EmployeeId;

            var work = _dbContext.Employee.Where(x => x.EmployeeId == param.EmployeeId).FirstOrDefault();
            model.EmpNo = work?.EmployeeNo ?? string.Empty;
            model.EmployeeName = work != null ? work.Firstname + " " + work.Lastname : string.Empty;
            int _currentYear = DateTime.Now.Year;

            var leaveinfo = _dbContext.LeaveInfo.Where(x => x.EmployeeId == param.EmployeeId && x.DateCreated.Date.Year == _currentYear && x.IsActive).OrderByDescending(x => x.DateCreated).FirstOrDefault();
            model.VLBalance = leaveinfo?.VLBalance ?? 0;
            model.SLBalance = leaveinfo?.SLBalance ?? 0;
            model.SPLBalance = leaveinfo?.SPLBalance ?? 0;

            var leaveReqHeaderDbContext = _dbContext.LeaveRequestHeader.AsQueryable();

            model.LeaveRequestHeaderList = (from lheader in _dbContext.LeaveRequestHeader
                                            join lvtype in _dbContext.LeaveType
                                            on lheader.LeaveTypeId equals lvtype.LeaveTypeId
                                            join stat in _dbContext.Status
                                            on lheader.Status equals stat.StatusId
                                            where lheader.EmployeeId == param.EmployeeId && lheader.IsActive
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
                                            }).OrderByDescending(x => x.LeaveRequestHeaderId).ToList();


            await GetYearList(model, param.EmployeeId);

            var workdtls = _dbContext.EmployeeWorkDetails.Where(x => x.EmployeeId == param.EmployeeId).FirstOrDefault();

            int selectedYear = param.FilterYear > 0 ? param.FilterYear : _currentYear;


            if (workdtls != null && (workdtls.EmployeeStatusId == (int)EnumEmploymentType.Regular || workdtls.EmployeeStatusId == (int)EnumEmploymentType.Probationary))
            {
                model.vlSummaries = GetLeaveSummary(param.EmployeeId, selectedYear, "VL", false);
                model.slSummaries = GetLeaveSummary(param.EmployeeId, selectedYear, "SL", false);

            }
            else
            {
                model.vlSummaries = GetLeaveSummary(param.EmployeeId, selectedYear, "VL", true);
                model.slSummaries = GetLeaveSummary(param.EmployeeId, selectedYear, "SL", true);
            }

            return model;
        }

        private List<LeaveSummaryViewModel> GetLeaveSummary(int employeeId, int year, string leaveType, bool isContractual)
        {
            IQueryable<LeaveSummary> query;

            if (isContractual)
            {
                query = _dbContext.LeaveSummary
                         .FromSqlInterpolated($"EXEC get_LeaveBalanceContractual @Year = {year}");
            }
            else
            {
                query = _dbContext.LeaveSummary
                         .FromSqlInterpolated($"EXEC get_LeaveBalance @EmployeeId = {employeeId}, @Year = {year}, @LeaveType = {leaveType}");
            }

            return query.ToList()
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
        }


        private async Task<LeaveViewModel> GetYearList(LeaveViewModel model, int empId)
        {
            var workdtls = _dbContext.EmployeeWorkDetails.Where(x => x.EmployeeId == empId).FirstOrDefault();

            int startYear = workdtls?.DateHired?.Year ?? DateTime.Now.Year;
            int currentYear = DateTime.Now.Year;

            var _yearList = new LeaveViewModel
            {
                YearList = Enumerable.Range(startYear, currentYear - startYear + 1)
                            .OrderByDescending(y => y) // 🔽 Order from latest to earliest
                            .ToList(),
                SelectedYear = currentYear
            };

            model.YearList = _yearList.YearList;

            return model;
        }

        public async Task<LeaveViewModel> RequestLeave(LeaveViewModel param)
        {
            LeaveViewModel model = new LeaveViewModel();
            try
            {
           
                var leaveinfo = _dbContext.LeaveInfo.Where(x => x.EmployeeId == param.EmployeeId && x.IsActive).OrderByDescending(x => x.DateCreated).FirstOrDefault();

                var query = from lheader in _dbContext.LeaveRequestHeader.AsNoTracking()
                            join lvtype in _dbContext.LeaveType.AsNoTracking() on lheader.LeaveTypeId equals lvtype.LeaveTypeId                            
                            join stat in _dbContext.Status.AsNoTracking() on lheader.Status equals stat.StatusId
                            join emp in _dbContext.Employee.AsNoTracking() on lheader.EmployeeId equals emp.EmployeeId
                            join apprvl in _dbContext.ApprovalHistory.AsNoTracking().Where(x => x.ModulePageId == (int)EnumModulePage.Leave)
                            on lheader.LeaveRequestHeaderId equals apprvl.TransactionId into ah
                            from apprvl in ah.DefaultIfEmpty()  

                            where lheader.LeaveRequestHeaderId == param.LeaveRequestHeaderId //&& apprvl.IsActive 
                            select new LeaveRequestHeaderViewModel
                            {
                                LeaveRequestHeaderId = lheader.LeaveRequestHeaderId,
                                EmployeeId = lheader.EmployeeId,
                                RequestNo = lheader.RequestNo,
                                DateFiled = lheader.DateFiled,
                                DateFiledString = lheader.DateFiled.ToString("MM/dd/yyyy hh:mm tt"),
                                LeaveTypeId = lheader.LeaveTypeId,
                                LeaveName = lvtype.Description,
                                Reason = lheader.Reason,
                                NoofDays = lheader.NoOfDays,
                                Status = lheader.Status,
                                StatusName = stat.StatusName,
                                EmployeeName = emp.Firstname + " " + emp.Lastname,
                                DateApprovedDisapproved = apprvl != null ? apprvl.DateCreated.ToString("yyyy-MM-dd HH:mm") : string.Empty,
                                ApprovalRemarks = apprvl != null ? apprvl.Remarks : string.Empty,
                                DateModified = lheader.DateModified,
                                ModifiedBy = lheader.ModifiedBy,                           
                            };
           
                model.LeaveRequestHeader = await query.AsNoTracking().FirstOrDefaultAsync();
           

                if (param.LeaveRequestHeaderId == 0)
                {
                    model = new LeaveViewModel();
                    model.LeaveRequestHeader.SPLBalance = leaveinfo.SPLBalance;
                    model.LeaveRequestHeader.SLBalance = leaveinfo.SLBalance;
                    model.LeaveRequestHeader.VLBalance = leaveinfo.VLBalance;
                }
                else
                {
                    model.LeaveDateList = _dbContext.LeaveRequestDetails
                                     .Where(dtl => dtl.LeaveRequestHeaderId == param.LeaveRequestHeaderId)
                                     .Select(dtl => dtl.LeaveDate.Date.ToShortDateString())
                                     .ToList();
                  
                }

                var leavetypeList = _dbContext.LeaveType.Where(x => x.IsActive == true).AsNoTracking().ToList();
                model.LeaveRequestHeader.LeaveTypeList = leavetypeList.Count() > 0 ? leavetypeList : null;

                return model;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return null;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public async Task<(int statuscode, string message)> CancelLeave(LeaveRequestHeaderViewModel model)
        {
            try
            {
                var entity = await _dbContext.LeaveRequestHeader.FirstOrDefaultAsync(x => x.LeaveRequestHeaderId == model.LeaveRequestHeaderId && x.IsActive == true);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid Leave Id");
                }
                entity.Status = (int)EnumStatus.Cancelled;
                entity.ModifiedBy = model.ModifiedBy;
                entity.DateModified = DateTime.Now;
                entity.IsActive = true;
                _dbContext.LeaveRequestHeader.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                var usr = _dbContext.User.AsNoTracking().Where(x => x.EmployeeId == entity.EmployeeId).FirstOrDefault();

                //Send Application Notification to Approver
                NotificationViewModel notifvmToApprover = new NotificationViewModel();
                notifvmToApprover.Title = "Leave";
                notifvmToApprover.Description = System.String.Format("Leave request {0} has been cancelled by the requestor.", entity.RequestNo);
                notifvmToApprover.ModuleId = (int)EnumModulePage.Leave;
                notifvmToApprover.TransactionId = entity.LeaveRequestHeaderId;
                notifvmToApprover.AssignId = entity.ApproverId;
                notifvmToApprover.URL = "/Todo/Index/";
                notifvmToApprover.MarkRead = false;
                notifvmToApprover.CreatedBy = usr != null ? usr.UserId : 0;
                notifvmToApprover.IsActive = true;
                await _homeRepository.SaveNotification(notifvmToApprover);

                return (StatusCodes.Status200OK, System.String.Format("Leave request {0} has been cancelled.", entity.RequestNo));

               // return (StatusCodes.Status200OK, "Successfully cancelled");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return (StatusCodes.Status400BadRequest, ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
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


            try
            {
                if (param.LeaveRequestHeader == 0)
                {
                    LeaveRequestHeader entity = new LeaveRequestHeader();
                    entity.EmployeeId = param.EmployeeId;
                    entity.RequestNo = await GenereteRequestNo();
                    entity.DateFiled = DateTime.Now;
                    entity.LeaveTypeId = param.LeaveTypeId;
                    entity.Status = (int)EnumStatus.ForApproval;
                    entity.ApproverId = param.ApproverId;
                    entity.Reason = param.Reason;
                    entity.NoOfDays = param.NoOfDays;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    await _dbContext.LeaveRequestHeader.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();

                    int count = param.SelectedDateList.Count();

                    if(param.LeaveTypeId == (int)EnumLeaveType.VLMon || param.LeaveTypeId == (int)EnumLeaveType.SLMon)
                    {
                        LeaveRequestDetails entityDtl = new LeaveRequestDetails();
                        entityDtl.LeaveRequestHeaderId = entity.LeaveRequestHeaderId;
                        entityDtl.LeaveDate = DateTime.Now;
                        entityDtl.Amount = param.NoOfDays;
                        entityDtl.IsActive = true;
                        await _dbContext.LeaveRequestDetails.AddAsync(entityDtl);
                        await _dbContext.SaveChangesAsync();
                    }

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

                    LeaveViewModel model = new LeaveViewModel();
                    //var workdtls = _dbContext.EmployeeWorkDetails.Where(x => x.EmployeeId == param.EmployeeId).FirstOrDefault();
                    //var dept = _dbContext.Department.Where(x => x.DepartmentId == workdtls.DepartmentId).FirstOrDefault();
                    model.ApproverId = param.ApproverId;//.ApproverId;
                    model.LeaveRequestHeader.Status = entity.Status;
                    model.LeaveRequestHeader.RequestNo = entity.RequestNo;
                    await _emailRepository.SendToApprovalLeave(model);


                    //Send Application Notification to Approver
                    NotificationViewModel notifvm = new NotificationViewModel();
                    notifvm.Title = "Leave Request";
                    notifvm.Description = System.String.Format("You have been assigned leave request {0} for approval", entity.RequestNo);
                    notifvm.ModuleId = (int)EnumModulePage.Leave;
                    notifvm.TransactionId = entity.LeaveRequestHeaderId;
                    notifvm.AssignId = param.ApproverId;                   
                    notifvm.URL = "/Todo/Index";
                    notifvm.MarkRead = false;
                    notifvm.CreatedBy = param.CurrentUserId;
                    notifvm.IsActive = true;
                    await _homeRepository.SaveNotification(notifvm);


                    //Send Application Notification to Requestor
                    NotificationViewModel notifvmRequestor = new NotificationViewModel();
                    notifvmRequestor.Title = "Leave Request";
                    notifvmRequestor.Description = System.String.Format("Your Leave request {0} has been submitted for approval.", entity.RequestNo);
                    notifvmRequestor.ModuleId = (int)EnumModulePage.Leave;
                    notifvmRequestor.TransactionId = entity.LeaveRequestHeaderId;
                    notifvmRequestor.AssignId = param.CurrentUserId;        
                    notifvmRequestor.URL = "/Home/Notification";
                    notifvmRequestor.MarkRead = false;
                    notifvmRequestor.CreatedBy = param.CurrentUserId;
                    notifvmRequestor.IsActive = true;
                    await _homeRepository.SaveNotification(notifvmRequestor);


                    return (StatusCodes.Status200OK, string.Format("Leave request {0} has been submitted for approval.", entity.RequestNo));

                }
                return (StatusCodes.Status400BadRequest, "An error occurred. Please try again.");
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
                int _currentYear = DateTime.Now.Year;
                int _currentMonth = DateTime.Now.Month;
                var _leaveContext = await _dbContext.LeaveRequestHeader
                                                .Where(x => x.IsActive == true && x.DateFiled.Date.Year == _currentYear && x.DateFiled.Date.Month == _currentMonth)
                                                .AsQueryable()
                                                .ToListAsync();


                int totalrecords = _leaveContext.Count() + 1;
                string finalSetRecords = GetFormattedRecord(totalrecords);
                string yearMonth = DateTime.Now.ToString("yyyyMM");
                string req = "RQT";

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

        public async Task<IList<LeaveReportViewModel>> GetAllLeaveReport()
        {
            var data = _dbContext.Employee
                      .AsNoTracking()
                      .Where(emp => emp.IsActive)
                      .Join(
                          _dbContext.EmployeeWorkDetails
                              .AsNoTracking()
                              .Where(wrk => wrk.ResignedDate == null),
                          emp => emp.EmployeeId,
                          wrk => wrk.EmployeeId,
                          (emp, wrk) => emp
                      )
                      .GroupJoin(
                          _dbContext.LeaveInfo.AsNoTracking(),
                          emp => emp.EmployeeId,
                          lvinfo => lvinfo.EmployeeId,
                          (emp, lvinfos) => new { emp, lvinfo = lvinfos.OrderByDescending(l => l.DateCreated).FirstOrDefault() } 
                      )
                      .Where(x => x.lvinfo != null) // avoid employees with no LeaveInfo
                      .Select(x => new LeaveReportViewModel
                      {
                          EmployeeId = x.emp.EmployeeId,
                          EmpNo = x.emp.EmployeeNo,
                          EmployeeName = string.Concat(x.emp.Firstname, " ", x.emp.Lastname),
                          VLBalance = x.lvinfo.VLBalance,
                          SLBalance = x.lvinfo.SLBalance,
                          SPLBalance = x.lvinfo.SPLBalance,
                          PendingApplication = 0,
                          VLFiled = 0,
                          SLFiled = 0
                      })
                      .OrderByDescending(x => x.EmployeeName)
                      .ToList();

                            return data;

        }
    }
}
