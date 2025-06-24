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
using System.Reflection.PortableExecutable;

namespace DCI.Repositories
{
    public class TodoRepository : ITodoRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        private IEmailRepository _emailRepository;
        private ILeaveRepository _leaveRepository;
        private readonly IHomeRepository _homeRepository;

        public TodoRepository(DCIdbContext context, IEmailRepository emailRepository, ILeaveRepository leaveRepository, IHomeRepository homeRepository)
        {
            this._dbContext = context;
            this._emailRepository = emailRepository;
            this._leaveRepository = leaveRepository;
            this._homeRepository = homeRepository;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }


        public async Task<IList<LeaveRequestHeaderViewModel>> GetAllTodo(LeaveViewModel model)
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


        public async Task<(int statuscode, string message)> Approval(ApprovalHistoryViewModel param)
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
                notifvm.Description =  String.Format("Leave Request No {0} has been {1}", contextHdr.RequestNo , status);
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

        public async Task<IList<LeaveRequestHeaderViewModel>> GetApprovalLog(LeaveViewModel model)
        {
            try
            {
                var leaveHdr = _dbContext.LeaveRequestHeader.AsQueryable().ToList();
                var contextDetail = _dbContext.LeaveRequestDetails.AsQueryable().ToList();
                var empList = _dbContext.Employee.AsQueryable().ToList();
                var statusList = _dbContext.Status.AsQueryable().ToList();
                var approvalLog = _dbContext.ApprovalHistory.AsQueryable().ToList();


                var query = from apprv in approvalLog
                            join hdr in leaveHdr on apprv.TransactionId equals hdr.LeaveRequestHeaderId
                            join emp in empList on hdr.EmployeeId equals emp.EmployeeId
                            join stat in statusList on hdr.Status equals stat.StatusId
                            where apprv.IsActive == true && apprv.ApproverId == model.CurrentUserId && apprv.ModulePageId == (int)EnumModulePage.Leave
                            select new LeaveRequestHeaderViewModel
                            {
                                LeaveRequestHeaderId = hdr.LeaveRequestHeaderId,
                                RequestNo = hdr.RequestNo,
                                EmployeeId = hdr.EmployeeId,
                                EmployeeName = emp.Firstname + " " + emp.Lastname,
                                DateFiled = hdr.DateFiled,
                                Status = hdr.Status,
                                StatusName = stat.StatusName,
                                Reason = hdr.Reason,
                                NoofDays = hdr.NoOfDays,
                                DateApprovedDisapproved = apprv.DateCreated,
                                LeaveRequestDetailList = (from dtl in _dbContext.LeaveRequestDetails
                                                          where dtl.LeaveRequestHeaderId == hdr.LeaveRequestHeaderId
                                                          select new LeaveRequestDetailViewModel
                                                          {
                                                              LeaveRequestHeaderId = dtl.LeaveRequestHeaderId,
                                                              LeaveRequestDetailId = dtl.LeaveRequestDetailId,
                                                              LeaveDate = dtl.LeaveDate,
                                                              Amount = dtl.Amount
                                                          }).ToList()
                            };


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

    }
}
