using DCI.Core.Common;
using DCI.Core.Helpers;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.PortableExecutable;


namespace DCI.Repositories
{
    public class TodoRepository : ITodoRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        private IEmailRepository _emailRepository;
        private ILeaveRepository _leaveRepository;
        private IDailyTimeRecordRepository _dtrRepository;
        private readonly IHomeRepository _homeRepository;

        public TodoRepository(DCIdbContext context, IEmailRepository emailRepository, ILeaveRepository leaveRepository, IHomeRepository homeRepository, IDailyTimeRecordRepository dtrRepository)
        {
            this._dbContext = context;
            this._emailRepository = emailRepository;
            this._leaveRepository = leaveRepository;
            this._homeRepository = homeRepository;
            _dtrRepository = dtrRepository;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<TodoViewModel> GetAllTodo(TodoViewModel model)
        {
            try
            {
                var leavequery = await (
                              from leave in _dbContext.LeaveRequestHeader.AsNoTracking()
                              join emp in _dbContext.Employee.AsNoTracking() on leave.EmployeeId equals emp.EmployeeId
                              join stat in _dbContext.Status.AsNoTracking() on leave.Status equals stat.StatusId
                              where leave.IsActive && leave.Status == (int)EnumStatus.ForApproval
                              orderby leave.DateFiled descending
                              select new LeaveRequestHeaderViewModel
                              {
                                  LeaveRequestHeaderId = leave.LeaveRequestHeaderId,
                                  RequestNo = leave.RequestNo,
                                  EmployeeId = leave.EmployeeId,
                                  EmployeeName = emp.Firstname + " " + emp.Lastname,
                                  DateFiled = leave.DateFiled,
                                  DateFiledString = leave.DateFiled.ToShortDateString(),
                                  Status = leave.Status,
                                  StatusName = stat.StatusName,
                                  Reason = leave.Reason,
                                  NoofDays = leave.NoOfDays,
                                  LeaveRequestDetailList = (from dtl in _dbContext.LeaveRequestDetails
                                                            where dtl.LeaveRequestHeaderId == leave.LeaveRequestHeaderId
                                                            select new LeaveRequestDetailViewModel
                                                            {
                                                                LeaveRequestHeaderId = dtl.LeaveRequestHeaderId,
                                                                LeaveRequestDetailId = dtl.LeaveRequestDetailId,
                                                                LeaveDate = dtl.LeaveDate,
                                                                Amount = dtl.Amount
                                                            }).ToList()
                              }).ToListAsync();

                model.leaveList = leavequery;
                model.LeaveCount = leavequery.Count();

                var dtrquery = await (
                                    from dtr in _dbContext.DTRCorrection.AsNoTracking()
                                    //join usr in _dbContext.User.AsNoTracking() on dtr.CreatedBy equals usr.UserId
                                    join emp in _dbContext.Employee.AsNoTracking() on dtr.EmployeeId equals emp.EmployeeId
                                    join stat in _dbContext.Status.AsNoTracking() on dtr.Status equals stat.StatusId
                                    where dtr.IsActive
                                       && dtr.Status == (int)EnumStatus.ForApproval
                                       && dtr.ApproverId == model.CurrentUserId
                                    orderby dtr.DateFiled descending
                                    select new DTRCorrectionViewModel
                                    {
                                        DtrId = dtr.DtrId,
                                        RequestNo = dtr.RequestNo,
                                        DateFiled = dtr.DateFiled,
                                        DtrDateTime = dtr.DtrDateTime,
                                        DtrType = dtr.DtrType,
                                        EmployeeName = emp.Firstname + " " + emp.Lastname,
                                        Status = dtr.Status,
                                        StatusName = stat.StatusName,
                                        Reason = dtr.Reason
                                    }
                                ).ToListAsync();

                model.dtrList = dtrquery.ToList();
                model.DTRCount = dtrquery.Count();

                var overtimequery = await (
                                from ot in _dbContext.OvertimeHeader.AsNoTracking()
                                join usr in _dbContext.User.AsNoTracking() on ot.CreatedBy equals usr.UserId
                                join emp in _dbContext.Employee.AsNoTracking() on usr.EmployeeId equals emp.EmployeeId
                                join stat in _dbContext.Status.AsNoTracking() on ot.StatusId equals stat.StatusId
                                where ot.IsActive && ot.CreatedBy == model.CurrentUserId
                                orderby ot.DateCreated descending
                                select new OvertimeViewModel
                                {
                                    OTHeaderId = ot.OTHeaderId,
                                    RequestNo = ot.RequestNo,
                                    Fullname = emp.Firstname + " " + emp.Lastname,
                                    EmployeeId = ot.EmployeeId,
                                    StatusId = ot.StatusId,
                                    StatusName = stat.StatusName,
                                    DateCreated = ot.DateCreated,
                                    CreatedBy = ot.CreatedBy,

                                    // safe Sum pattern for SQL translation
                                    Total = _dbContext.OvertimeDetail
                                                .Where(x => x.OTHeaderId == ot.OTHeaderId)
                                                .Select(x => (int?)x.TotalMinutes)
                                                .Sum() ?? 0
                                }
                                    ).ToListAsync();

                model.otList = overtimequery;
                model.OvertimeCount = overtimequery.Count();

                var wfhquery = await (
                                       from hdr in _dbContext.WfhHeader.AsNoTracking()
                                       join emp in _dbContext.Employee.AsNoTracking() on hdr.EmployeeId equals emp.EmployeeId
                                       join stat in _dbContext.Status.AsNoTracking() on hdr.Status equals stat.StatusId
                                       orderby hdr.DateCreated descending
                                       select new WFHHeaderViewModel
                                       {
                                           WfhHeaderId = hdr.WfhHeaderId,
                                           RequestNo = hdr.RequestNo,
                                           Fullname = emp.Lastname + " " + emp.Firstname,
                                           StatusId = hdr.Status,
                                           Remarks = hdr.Remarks,
                                           DateCreated = hdr.DateCreated,
                                           StatusName = stat.StatusName
                                       }
                                   ).ToListAsync();

                model.wfhList = wfhquery;
                model.WFHCount = wfhquery.Count();

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


        public async Task<IList<LeaveRequestHeaderViewModel>> GetAllTodoLeave(LeaveViewModel model)
        {
            try
            {
                var context = _dbContext.LeaveRequestHeader.AsQueryable().ToList();
                var contextDetail = _dbContext.LeaveRequestDetails.AsQueryable().ToList();
                var empList = _dbContext.Employee.AsQueryable().ToList();
                var statusList = _dbContext.Status.AsQueryable().ToList();


                var query = (from leave in context
                                 //  join dtl in _dbContext.LeaveRequestDetails on leave.LeaveRequestHeaderId equals dtl.LeaveRequestHeaderId
                             join emp in empList on leave.EmployeeId equals emp.EmployeeId
                             join stat in statusList on leave.Status equals stat.StatusId
                             where leave.IsActive == true && leave.Status == (int)EnumStatus.ForApproval
                             select new LeaveRequestHeaderViewModel
                             {
                                 LeaveRequestHeaderId = leave.LeaveRequestHeaderId,
                                 RequestNo = leave.RequestNo,
                                 EmployeeId = leave.EmployeeId,
                                 EmployeeName = emp.Firstname + " " + emp.Lastname,
                                 DateFiled = leave.DateFiled,
                                 DateFiledString = leave.DateFiled.ToShortDateString(),
                                 Status = leave.Status,
                                 StatusName = stat.StatusName,
                                 Reason = leave.Reason,
                                 NoofDays = leave.NoOfDays,
                                 LeaveRequestDetailList = (from dtl in _dbContext.LeaveRequestDetails
                                                           where dtl.LeaveRequestHeaderId == leave.LeaveRequestHeaderId
                                                           select new LeaveRequestDetailViewModel
                                                           {
                                                               LeaveRequestHeaderId = dtl.LeaveRequestHeaderId,
                                                               LeaveRequestDetailId = dtl.LeaveRequestDetailId,
                                                               LeaveDate = dtl.LeaveDate,
                                                               Amount = dtl.Amount
                                                           }).ToList()
                             }).ToList();


                return query.ToList();
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

        public async Task<IList<DTRCorrectionViewModel>> GetAllTodoDtr(DTRCorrectionViewModel model)
        {
            try
            {
                var query = await (
                              from dtr in _dbContext.DTRCorrection
                              join usr in _dbContext.User on dtr.CreatedBy equals usr.UserId
                              join emp in _dbContext.Employee on usr.EmployeeId equals emp.EmployeeId
                              join stat in _dbContext.Status on dtr.Status equals stat.StatusId
                              where dtr.IsActive
                                    && dtr.Status == (int)EnumStatus.ForApproval
                                    && dtr.ApproverId == model.CurrentUserId
                              select new DTRCorrectionViewModel
                              {
                                  DtrId = dtr.DtrId,
                                  RequestNo = dtr.RequestNo,
                                  DateFiled = dtr.DateFiled,
                                  DtrDateTime = dtr.DtrDateTime,
                                  DtrType = dtr.DtrType,
                                  EmployeeName = emp.Firstname + " " + emp.Lastname,
                                  Status = dtr.Status,
                                  StatusName = stat.StatusName,
                                  Reason = dtr.Reason,
                              })
                              .AsNoTracking()
                              .ToListAsync();

                return query;

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


        public async Task<(int statuscode, string message)> ApprovalLeave(ApprovalHistoryViewModel param)
        {
            try
            {
                ApprovalHistory entity = new ApprovalHistory();
                entity.ModulePageId = (int)EnumModulePage.Leave;
                entity.TransactionId = param.TransactionId;
                entity.ApproverId = param.ApproverId;
                entity.Status = param.Status;
                entity.Remarks = param.Remarks;
                entity.CreatedBy = param.CreatedBy;
                entity.DateCreated = DateTime.Now;
                entity.IsActive = true;
                await _dbContext.ApprovalHistory.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                int _currentYear = DateTime.Now.Year;

                var contextHdr = _dbContext.LeaveRequestHeader.Where(x => x.LeaveRequestHeaderId == param.TransactionId).FirstOrDefault();
                var contextDtl = _dbContext.LeaveRequestDetails.Where(x => x.IsActive && x.LeaveRequestHeaderId == contextHdr.LeaveRequestHeaderId).ToList();
                var emp = _dbContext.Employee.Where(x => x.EmployeeId == contextHdr.EmployeeId).FirstOrDefault();
                var empDetails = _dbContext.EmployeeWorkDetails.Where(x => x.EmployeeId == contextHdr.EmployeeId).FirstOrDefault();


                if (contextHdr != null && param.Status == (int)EnumStatus.Approved)
                {
                    if (empDetails?.EmployeeStatusId == (int)EnumEmploymentType.Regular)
                    {
                        var contextLeaveInfo = _dbContext.LeaveInfo.Where(x => x.EmployeeId == contextHdr.EmployeeId && x.DateCreated.Date.Year == _currentYear).OrderByDescending(x => x.DateCreated).FirstOrDefault();

                        if (contextHdr.LeaveTypeId == (int)EnumLeaveType.VL)
                        {
                            contextLeaveInfo.VLBalance = contextLeaveInfo.VLBalance - contextHdr.NoOfDays;
                            contextHdr.DeductionType = (int)EnumDeductionType.Payroll;
                        }
                        else if (contextHdr.LeaveTypeId == (int)EnumLeaveType.SL)
                        {
                            contextLeaveInfo.SLBalance = contextLeaveInfo.SLBalance - contextHdr.NoOfDays;
                            contextHdr.DeductionType = (int)EnumDeductionType.SickLeave;
                        }
                        else if (contextHdr.LeaveTypeId == (int)EnumLeaveType.SPL)
                        {
                            contextLeaveInfo.SPLBalance = contextLeaveInfo.SPLBalance - contextHdr.NoOfDays;
                            contextHdr.DeductionType = (int)EnumDeductionType.SpecialLeave;
                        }
                        _dbContext.LeaveInfo.Entry(contextLeaveInfo).State = EntityState.Modified;
                        _dbContext.SaveChanges();



                        foreach (var raw in contextDtl)
                        {
                            //Update DTR attendance summary status to VL DEDUCTED                    
                            await _dbContext.tbl_raw_logs.Where(x => x.EMPLOYEE_ID == emp.EmployeeNo && x.DATE_TIME.Date == raw.LeaveDate.Date)
                                                         .ExecuteUpdateAsync(s => s
                                                         .SetProperty(r => r.STATUS, r => (int)EnumStatus.VLDeducted));
                        }
                    }
                    else // probitionary and contractual/projectbased
                    {
                        foreach (var raw in contextDtl)
                        {
                            //Update DTR attendance summary status to Payroll DEDUCTED                    
                            await _dbContext.tbl_raw_logs.Where(x => x.EMPLOYEE_ID == emp.EmployeeNo && x.DATE_TIME.Date == raw.LeaveDate.Date)
                                                         .ExecuteUpdateAsync(s => s
                                                         .SetProperty(r => r.STATUS, r => (int)EnumStatus.PayrollDeducted));
                        }
                        contextHdr.DeductionType = (int)EnumDeductionType.Payroll;
                    }
                }

                contextHdr.Status = param.Status;
                contextHdr.ModifiedBy = param.ApproverId;
                contextHdr.DateModified = DateTime.Now;
                _dbContext.LeaveRequestHeader.Entry(contextHdr).State = EntityState.Modified;
                _dbContext.SaveChanges();

                LeaveViewModel lv = new LeaveViewModel();
                lv.LeaveRequestHeaderId = contextHdr.LeaveRequestHeaderId;

                var entitiesToViewModel = await _leaveRepository.RequestLeave(lv);

                // Send Email Notif
                await _emailRepository.SendToRequestorLeave(entitiesToViewModel);

                string status = param.Status == (int)EnumStatus.Approved ? "approved" : "disapproved";


                var user = _dbContext.User.Where(x => x.EmployeeId == contextHdr.EmployeeId).FirstOrDefault();

                NotificationViewModel notifvm = new NotificationViewModel();
                notifvm.Title = "Leave";
                notifvm.Description = String.Format("Leave request {0} has been {1}", contextHdr.RequestNo, status);
                notifvm.ModuleId = (int)EnumModulePage.Leave;
                notifvm.TransactionId = param.TransactionId;
                notifvm.AssignId = user.UserId;
                notifvm.URL = "/DailyTimeRecord/Leave";
                notifvm.MarkRead = false;
                notifvm.CreatedBy = param.CreatedBy;
                notifvm.IsActive = true;
                await _homeRepository.SaveNotification(notifvm);

                return (StatusCodes.Status200OK, String.Format("Leave Request {0} has been {1}.", contextHdr.RequestNo, status));
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

        public async Task<(int statuscode, string message)> ApprovalDtr(ApprovalHistoryViewModel param)
        {
            try
            {
                ApprovalHistory entity = new ApprovalHistory();
                entity.ModulePageId = (int)EnumModulePage.DTRCorrection;
                entity.TransactionId = param.TransactionId;
                entity.ApproverId = param.ApproverId;
                entity.Status = param.Status;
                entity.Remarks = param.Remarks;
                entity.CreatedBy = param.CreatedBy;
                entity.DateCreated = DateTime.Now;
                entity.IsActive = true;
                await _dbContext.ApprovalHistory.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                var _user = _dbContext.User.AsNoTracking().Where(x => x.UserId == param.CreatedBy).FirstOrDefault();
                string _middleInitial = string.IsNullOrWhiteSpace(_user.Middlename) ? string.Empty : _user.Middlename.Substring(0, 1).ToUpper();

                var contextHdr = _dbContext.DTRCorrection.AsNoTracking().Where(x => x.DtrId == param.TransactionId).FirstOrDefault();

                tbl_raw_logs raw_logs = new tbl_raw_logs();
                raw_logs.EMPLOYEE_ID = contextHdr.EmployeeId.ToString();
                raw_logs.FULL_NAME = _user.Firstname + " " + _middleInitial + " " + _user.Lastname;
                raw_logs.DATE_TIME = contextHdr.DtrDateTime;
                raw_logs.CREATED_DATE = DateTime.Now;
                raw_logs.CREATED_BY = Constants.SYSAD;
                raw_logs.STATUS = 0;
                await _dbContext.tbl_raw_logs.AddAsync(raw_logs);
                await _dbContext.SaveChangesAsync();

                
                contextHdr.Status = param.Status;
                contextHdr.RawLogsId = raw_logs.ID;
                _dbContext.DTRCorrection.Entry(contextHdr).State = EntityState.Modified;
                _dbContext.SaveChanges();                         

                var entitiesToViewModel = await _dtrRepository.DTRCorrectionByDtrId(contextHdr.DtrId);  

                //Send Email Notification to Requestor
                await _emailRepository.SendToRequestorDTRAdjustment(entitiesToViewModel);
                string status = param.Status == (int)EnumStatus.Approved ? "approved" : "disapproved";

                //Send Application Notification to Requestor
                NotificationViewModel notifvm = new NotificationViewModel();
                notifvm.Title = "DTR Adjustment";
                notifvm.Description = String.Format("DTR adjustment request {0} has been {1}", contextHdr.RequestNo, status);
                notifvm.ModuleId = (int)EnumModulePage.DTRCorrection;
                notifvm.TransactionId = param.TransactionId;
                notifvm.AssignId = contextHdr.CreatedBy;
                // notifvm.URL = "/DailyTimeRecord/DTRCorrectionById/?dtrId=" + contextHdr.DtrId;
                notifvm.URL = "/DailyTimeRecord/DTRCorrection";
                notifvm.MarkRead = false;
                notifvm.CreatedBy = param.CreatedBy;
                notifvm.IsActive = true;
                await _homeRepository.SaveNotification(notifvm);


                return (StatusCodes.Status200OK, String.Format("DTR adjustment request {0} has been {1}.", contextHdr.RequestNo, status));
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

        public async Task<(int statuscode, string message)> ApprovalWFH(ApprovalHistoryViewModel param)
        {
            try
            {
                ApprovalHistory entity = new ApprovalHistory();
                entity.ModulePageId = (int)EnumModulePage.WFH;
                entity.TransactionId = param.TransactionId;
                entity.ApproverId = param.ApproverId;
                entity.Status = param.Status;
                entity.Remarks = param.Remarks;
                entity.CreatedBy = param.CreatedBy;
                entity.DateCreated = DateTime.Now;
                entity.IsActive = true;
                await _dbContext.ApprovalHistory.AddAsync(entity);
                await _dbContext.SaveChangesAsync();


                //var contextHdr = _dbContext.WfhHeader.Where(x => x.WfhHeaderId == param.TransactionId).FirstOrDefault();
                //contextHdr.Status = param.Status;
                //_dbContext.WfhHeader.Entry(contextHdr).State = EntityState.Modified;
                //_dbContext.SaveChanges();

                //var contextDtl = _dbContext.WfhDetail.Where(x => x.WfhHeaderId == param.TransactionId).ToList();

                //foreach (var dtl in contextDtl)
                //{
                //    var _attendance = _dbContext.vw_AttendanceSummary_WFH.Where(x => x.ID == dtl.AttendanceId).FirstOrDefault();
                //    var wfhDate = _attendance?.DATE;
                //    var empNo = _attendance?.EMPLOYEE_NO;

                //    var _wfhlogs = _dbContext.tbl_wfh_logs.Where(x => x.EMPLOYEE_ID == empNo && x.DATE_TIME == wfhDate).FirstOrDefault();
                //    _wfhlogs.STATUS = param.Status;
                //    _dbContext.tbl_wfh_logs.Entry(_wfhlogs).State = EntityState.Modified;
                //    await _dbContext.SaveChangesAsync();
                //}

                //OPTIMIZED CODE CHATGPT START
                var contextHdr = await _dbContext.WfhHeader
                                        .FindAsync(param.TransactionId);

                if (contextHdr != null)
                {
                    contextHdr.Status = param.Status;
                }

                var contextDtl = await _dbContext.WfhDetail
                    .Where(x => x.WfhHeaderId == param.TransactionId)
                    .ToListAsync();

                var attendanceIds = contextDtl.Select(d => d.AttendanceId).ToList();

                var attendanceList = await _dbContext.vw_AttendanceSummary_WFH
                    .Where(x => attendanceIds.Contains((int)x.ID))
                    .ToListAsync();

                var empNoDatePairs = attendanceList
                    .Select(a => new { a.EMPLOYEE_NO, a.DATE })
                    .ToList();

                //var wfhLogs = await _dbContext.tbl_wfh_logs
                //    .Where(x => empNoDatePairs.Any(p => p.EMPLOYEE_NO == x.EMPLOYEE_ID && p.DATE == x.DATE_TIME))
                //    .ToListAsync();
                //var empNoDateKeys = empNoDatePairs
                //    .Select(p => new { p.EMPLOYEE_NO, p.DATE })
                //    .ToList();

                var empNos = empNoDatePairs.Select(p => p.EMPLOYEE_NO).Distinct().ToList();
                var dates = empNoDatePairs.Select(p => p.DATE).Distinct().ToList();

                var wfhLogs = await _dbContext.tbl_wfh_logs
                    .Where(x => empNos.Contains(x.EMPLOYEE_ID) && dates.Contains(x.DATE_TIME.Date))
                    .ToListAsync();

                foreach (var log in wfhLogs)
                {
                    log.STATUS = param.Status;
                }


                await _dbContext.SaveChangesAsync();
                //OPTIMIZED CODE CHATGPT END




                // var entitiesToViewModel = await _dtrRepository.DTRCorrectionByDtrId(contextHdr.DtrId);

                // Send Email Notif
                //   await _emailRepository.SendToRequestorDTR(entitiesToViewModel);

                string status = param.Status == (int)EnumStatus.Approved ? "approved" : "disapproved";

                NotificationViewModel notifvm = new NotificationViewModel();
                notifvm.Title = "WFH";
                notifvm.Description = String.Format("WFH Request No {0} has been {1}", contextHdr.RequestNo, status);
                notifvm.ModuleId = (int)EnumModulePage.WFH;
                notifvm.TransactionId = param.TransactionId;
                notifvm.AssignId = contextHdr.CreatedBy;
                notifvm.URL = "/DailyTimeRecord/WFH/?wfhHeader=" + contextHdr.WfhHeaderId;
                notifvm.MarkRead = false;
                notifvm.CreatedBy = param.CreatedBy;
                notifvm.IsActive = true;
                await _homeRepository.SaveNotification(notifvm);

                return (StatusCodes.Status200OK, String.Format("WFH Request No {0} has been {1}.", contextHdr.RequestNo, status));
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

        //public async Task<IList<LeaveRequestHeaderViewModel>> GetApprovalLog(LeaveViewModel model)
        //{
        //    try
        //    {
        //        var leaveHdr = _dbContext.LeaveRequestHeader.AsQueryable().ToList();
        //        var contextDetail = _dbContext.LeaveRequestDetails.AsQueryable().ToList();
        //        var empList = _dbContext.Employee.AsQueryable().ToList();
        //        var statusList = _dbContext.Status.AsQueryable().ToList();
        //        var approvalLog = _dbContext.ApprovalHistory.AsQueryable().ToList();


        //        var query = from apprv in approvalLog
        //                    join hdr in leaveHdr on apprv.TransactionId equals hdr.LeaveRequestHeaderId
        //                    join emp in empList on hdr.EmployeeId equals emp.EmployeeId
        //                    join stat in statusList on hdr.Status equals stat.StatusId
        //                    where apprv.IsActive == true && apprv.ApproverId == model.CurrentUserId && apprv.ModulePageId == (int)EnumModulePage.Leave
        //                    select new LeaveRequestHeaderViewModel
        //                    {
        //                        LeaveRequestHeaderId = hdr.LeaveRequestHeaderId,
        //                        RequestNo = hdr.RequestNo,
        //                        EmployeeId = hdr.EmployeeId,
        //                        EmployeeName = emp.Firstname + " " + emp.Lastname,
        //                        DateFiled = hdr.DateFiled,
        //                        Status = hdr.Status,
        //                        StatusName = stat.StatusName,
        //                        Reason = hdr.Reason,
        //                        NoofDays = hdr.NoOfDays,
        //                        DateApprovedDisapproved = apprv.DateCreated,
        //                        LeaveRequestDetailList = (from dtl in _dbContext.LeaveRequestDetails
        //                                                  where dtl.LeaveRequestHeaderId == hdr.LeaveRequestHeaderId
        //                                                  select new LeaveRequestDetailViewModel
        //                                                  {
        //                                                      LeaveRequestHeaderId = dtl.LeaveRequestHeaderId,
        //                                                      LeaveRequestDetailId = dtl.LeaveRequestDetailId,
        //                                                      LeaveDate = dtl.LeaveDate,
        //                                                      Amount = dtl.Amount
        //                                                  }).ToList()
        //                    };


        //        return query.ToList();
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.ToString());
        //        return null;
        //    }
        //    finally
        //    {
        //        Log.CloseAndFlush();
        //    }
        //}

        public async Task<IList<ApprovalHistoryViewModel>> GetApprovalHistory(ApprovalHistoryViewModel model)
        {
            try
            {

                var approvals = _dbContext.ApprovalHistory
              .AsNoTracking()
              .Where(a => a.IsActive && a.ApproverId == model.CurrentUserId);

                var leaveQ =
                    from apprv in approvals.Where(a => a.ModulePageId == (int)EnumModulePage.Leave)
                    join hdr in _dbContext.LeaveRequestHeader.AsNoTracking() on apprv.TransactionId equals hdr.LeaveRequestHeaderId
                    join emp in _dbContext.Employee.AsNoTracking() on hdr.EmployeeId equals emp.EmployeeId
                    join stat in _dbContext.Status.AsNoTracking() on hdr.Status equals stat.StatusId
                    select new ApprovalHistoryViewModel
                    {
                        ApprovalHistoryId = apprv.ApprovalHistoryId,
                        RequestNo = hdr.RequestNo,
                        Requestor = emp.Firstname + " " + emp.Lastname,
                        Status = hdr.Status,
                        StatusName = stat.StatusName,
                        StatusDate = apprv.DateCreated,
                        ModuleName = EnumHelper.GetEnumDescriptionByTypeValue(typeof(EnumModulePage), (int)EnumModulePage.Leave),
                        DateCreated = hdr.DateFiled
                    };

                var dtrQ =
                    from apprv in approvals.Where(a => a.ModulePageId == (int)EnumModulePage.DTRCorrection)
                    join dtr in _dbContext.DTRCorrection.AsNoTracking() on apprv.TransactionId equals dtr.DtrId
                    join emp in _dbContext.Employee.AsNoTracking() on dtr.EmployeeId equals emp.EmployeeId
                    join stat in _dbContext.Status.AsNoTracking() on dtr.Status equals stat.StatusId
                    select new ApprovalHistoryViewModel
                    {
                        ApprovalHistoryId = apprv.ApprovalHistoryId,
                        RequestNo = dtr.RequestNo,
                        Requestor = emp.Firstname + " " + emp.Lastname,
                        Status = dtr.Status,
                        StatusName = stat.StatusName,
                        StatusDate = apprv.DateCreated,
                        ModuleName = EnumHelper.GetEnumDescriptionByTypeValue(typeof(EnumModulePage), (int)EnumModulePage.DTRCorrection),
                        DateCreated = dtr.DateFiled
                    };

                var wfhQ =
                    from apprv in approvals.Where(a => a.ModulePageId == (int)EnumModulePage.WFH)
                    join wfh in _dbContext.WfhHeader.AsNoTracking() on apprv.TransactionId equals wfh.WfhHeaderId
                    join emp in _dbContext.Employee.AsNoTracking() on wfh.EmployeeId equals emp.EmployeeId
                    join stat in _dbContext.Status.AsNoTracking() on wfh.Status equals stat.StatusId
                    select new ApprovalHistoryViewModel
                    {
                        ApprovalHistoryId = apprv.ApprovalHistoryId,
                        RequestNo = wfh.RequestNo,
                        Requestor = emp.Firstname + " " + emp.Lastname,
                        Status = wfh.Status,
                        StatusName = stat.StatusName,
                        StatusDate = apprv.DateCreated,
                        ModuleName = EnumHelper.GetEnumDescriptionByTypeValue(typeof(EnumModulePage), (int)EnumModulePage.WFH),
                        DateCreated = wfh.DateCreated
                    };

                // Single translation (UNION ALL) and single round trip
                var result = await leaveQ
                    .Concat(dtrQ)
                    .Concat(wfhQ)
                    //.OrderByDescending(x => x.StatusDate)
                    .ToListAsync();

                return result;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return new List<ApprovalHistoryViewModel>();
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public async Task<IList<OvertimeViewModel>> GetAllTodoOvertime(OvertimeViewModel model)
        {

            var query = from ot in _dbContext.OvertimeHeader
                        join usr in _dbContext.User on ot.CreatedBy equals usr.UserId
                        join emp in _dbContext.Employee on usr.EmployeeId equals emp.EmployeeId
                        join stat in _dbContext.Status on ot.StatusId equals stat.StatusId
                        where ot.IsActive && ot.CreatedBy == model.CurrentUserId
                        select new OvertimeViewModel
                        {
                            OTHeaderId = ot.OTHeaderId,
                            RequestNo = ot.RequestNo,
                            Fullname = emp.Firstname + " " + emp.Lastname,
                            EmployeeId = ot.EmployeeId,
                            StatusId = ot.StatusId,
                            StatusName = stat.StatusName,
                            DateCreated = ot.DateCreated,
                            CreatedBy = ot.CreatedBy,
                            Total = _dbContext.OvertimeDetail.Where(x => x.OTHeaderId == ot.OTHeaderId).Sum(x => x.TotalMinutes)
                        };

            return await query.ToListAsync();
        }

        public async Task<IList<WFHHeaderViewModel>> GetAllWFH(WFHHeaderViewModel model)
        {
            var query = await (from hdr in _dbContext.WfhHeader.AsQueryable()
                               join emp in _dbContext.Employee.AsNoTracking() on hdr.EmployeeId equals emp.EmployeeId
                               join stat in _dbContext.Status on hdr.Status equals stat.StatusId
                               select new WFHHeaderViewModel
                               {
                                   WfhHeaderId = hdr.WfhHeaderId,
                                   RequestNo = hdr.RequestNo,
                                   Fullname = emp.Lastname + " " + emp.Firstname,
                                   StatusId = hdr.Status,
                                   Remarks = hdr.Remarks,
                                   DateCreated = hdr.DateCreated,
                                   StatusName = stat.StatusName,

                               }).ToListAsync();
            return query;
        }

        public async Task<(int statuscode, string message)> ApprovalOvertime(ApprovalHistoryViewModel param)
        {
            try
            {
                ApprovalHistory entity = new ApprovalHistory();
                entity.ModulePageId = (int)EnumModulePage.WFH;
                entity.TransactionId = param.TransactionId;
                entity.ApproverId = param.ApproverId;
                entity.Status = param.Status;
                entity.Remarks = param.Remarks;
                entity.CreatedBy = param.CreatedBy;
                entity.DateCreated = DateTime.Now;
                entity.IsActive = true;
                await _dbContext.ApprovalHistory.AddAsync(entity);
                await _dbContext.SaveChangesAsync();


                //OPTIMIZED CODE CHATGPT END


                // var entitiesToViewModel = await _dtrRepository.DTRCorrectionByDtrId(contextHdr.DtrId);

                // Send Email Notif
                //   await _emailRepository.SendToRequestorDTR(entitiesToViewModel);

                string status = param.Status == (int)EnumStatus.Approved ? "approved" : "disapproved";

                //NotificationViewModel notifvm = new NotificationViewModel();
                //notifvm.Title = "WFH";
                //notifvm.Description = String.Format("WFH Request No {0} has been {1}", contextHdr.RequestNo, status);
                //notifvm.ModuleId = (int)EnumModulePage.WFH;
                //notifvm.TransactionId = param.TransactionId;
                //notifvm.AssignId = contextHdr.CreatedBy;
                //notifvm.URL = "/DailyTimeRecord/WFH/?wfhHeader=" + contextHdr.WfhHeaderId;
                //notifvm.MarkRead = false;
                //notifvm.CreatedBy = param.CreatedBy;
                //notifvm.IsActive = true;
                //await _homeRepository.SaveNotification(notifvm);

                return (StatusCodes.Status200OK, String.Format("WFH Request No {0} has been {1}.", "RequestNo", status));
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
    }
}
