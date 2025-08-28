using DCI.Core.Common;
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
                var context = _dbContext.DTRCorrection.AsQueryable().ToList();
                var empList = _dbContext.Employee.AsQueryable().ToList();
                var statusList = _dbContext.Status.AsQueryable().ToList();


                var query = (from dtr in context
                             join emp in empList on dtr.CreatedBy equals emp.EmployeeId
                             join stat in statusList on dtr.Status equals stat.StatusId
                             where dtr.IsActive == true && dtr.Status == (int)EnumStatus.ForApproval
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


                var contextHdr = _dbContext.LeaveRequestHeader.Where(x => x.LeaveRequestHeaderId == param.TransactionId).FirstOrDefault();

                if (contextHdr != null)
                {
                    var contextLeaveInfo = _dbContext.LeaveInfo.Where(x => x.EmployeeId == contextHdr.EmployeeId).FirstOrDefault();

                    if (contextHdr.LeaveTypeId == (int)EnumLeaveType.VL)
                    {
                        contextLeaveInfo.VLBalance = contextLeaveInfo.VLBalance - contextHdr.NoOfDays;
                        _dbContext.LeaveInfo.Entry(contextLeaveInfo).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    else if (contextHdr.LeaveTypeId == (int)EnumLeaveType.SL)
                    {
                        contextLeaveInfo.SLBalance = contextLeaveInfo.SLBalance - contextHdr.NoOfDays;
                        _dbContext.LeaveInfo.Entry(contextLeaveInfo).State = EntityState.Modified;
                        _dbContext.SaveChanges();
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
                await _emailRepository.SendToRequestor(entitiesToViewModel);

                string status = param.Status == (int)EnumStatus.Approved ? "approved" : "disapproved";

                NotificationViewModel notifvm = new NotificationViewModel();
                notifvm.Title = "Leave";
                notifvm.Description = String.Format("Leave Request No {0} has been {1}", contextHdr.RequestNo, status);
                notifvm.ModuleId = (int)EnumModulePage.Leave;
                notifvm.TransactionId = param.TransactionId;
                notifvm.AssignId = contextHdr.EmployeeId;
                notifvm.URL = "/DailyTimeRecord/Leave/?leaveId=" + contextHdr.LeaveRequestHeaderId;
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
                entity.ModulePageId = (int)EnumModulePage.DailyTimeRecord;
                entity.TransactionId = param.TransactionId;
                entity.ApproverId = param.ApproverId;
                entity.Status = param.Status;
                entity.Remarks = param.Remarks;
                entity.CreatedBy = param.CreatedBy;
                entity.DateCreated = DateTime.Now;
                entity.IsActive = true;
                await _dbContext.ApprovalHistory.AddAsync(entity);
                await _dbContext.SaveChangesAsync();


                var contextHdr = _dbContext.DTRCorrection.Where(x => x.DtrId == param.TransactionId).FirstOrDefault();
                contextHdr.Status = param.Status;
                _dbContext.DTRCorrection.Entry(contextHdr).State = EntityState.Modified;
                _dbContext.SaveChanges();

                //DTRCorrectionViewModel dtr = new DTRCorrectionViewModel();
                //dtr.DtrId = contextHdr.DtrId;

                var entitiesToViewModel = await _dtrRepository.DTRCorrectionByDtrId(contextHdr.DtrId);

                // Send Email Notif
                await _emailRepository.SendToRequestorDTR(entitiesToViewModel);

                string status = param.Status == (int)EnumStatus.Approved ? "approved" : "disapproved";

                NotificationViewModel notifvm = new NotificationViewModel();
                notifvm.Title = "DTR";
                notifvm.Description = String.Format("DTR Correction Request No {0} has been {1}", contextHdr.RequestNo, status);
                notifvm.ModuleId = (int)EnumModulePage.DailyTimeRecord;
                notifvm.TransactionId = param.TransactionId;
                notifvm.AssignId = contextHdr.CreatedBy;
                notifvm.URL = "/DailyTimeRecord/DTRCorrectionById/?dtrId=" + contextHdr.DtrId;
                notifvm.MarkRead = false;
                notifvm.CreatedBy = param.CreatedBy;
                notifvm.IsActive = true;
                await _homeRepository.SaveNotification(notifvm);

                return (StatusCodes.Status200OK, String.Format("DTR Request No {0} has been {1}.", contextHdr.RequestNo, status));
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
                var leaveHdr = _dbContext.LeaveRequestHeader.AsQueryable().ToList();
                var empList = _dbContext.Employee.AsQueryable().ToList();
                var statusList = _dbContext.Status.AsQueryable().ToList();
                var approvalLog = _dbContext.ApprovalHistory.AsQueryable().ToList();
                var dtrcontext = _dbContext.DTRCorrection.AsQueryable().ToList();

                var queryLeave = from apprv in approvalLog
                                 join hdr in leaveHdr on apprv.TransactionId equals hdr.LeaveRequestHeaderId
                                 join emp in empList on hdr.EmployeeId equals emp.EmployeeId
                                 join stat in statusList on hdr.Status equals stat.StatusId
                                 where apprv.IsActive == true && apprv.ApproverId == model.CurrentUserId && apprv.ModulePageId == (int)EnumModulePage.Leave
                                 select new ApprovalHistoryViewModel
                                 {
                                     ApprovalHistoryId = apprv.ApprovalHistoryId,
                                     RequestNo = hdr.RequestNo,
                                     Requestor = emp.Firstname + " " + emp.Lastname,
                                     Status = hdr.Status,
                                     StatusName = stat.StatusName,
                                     StatusDate = apprv.DateCreated.ToString(),
                                     ModuleName = "Leave",
                                     DateCreated = hdr.DateFiled
                                 };

                var queryDTR = from apprv in approvalLog
                               join dtr in dtrcontext on apprv.TransactionId equals dtr.DtrId
                               join emp in empList on dtr.CreatedBy equals emp.EmployeeId
                               join stat in statusList on dtr.Status equals stat.StatusId
                               where apprv.IsActive == true && apprv.ApproverId == model.CurrentUserId && apprv.ModulePageId == (int)EnumModulePage.DailyTimeRecord
                               select new ApprovalHistoryViewModel
                               {
                                   ApprovalHistoryId = apprv.ApprovalHistoryId,
                                   RequestNo = dtr.RequestNo,
                                   Requestor = emp.Firstname + " " + emp.Lastname,
                                   Status = dtr.Status,
                                   StatusName = stat.StatusName,
                                   StatusDate = apprv.DateCreated.ToString(),
                                   ModuleName = "DTR",
                                   DateCreated = dtr.DateFiled
                               };

                return queryLeave.Concat(queryDTR).ToList();


                //  return query.ToList();
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


    }
}
