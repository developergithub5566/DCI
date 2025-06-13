using DCI.Core.Common;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;

namespace DCI.Repositories
{
	public class TodoRepository : ITodoRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		private IEmailRepository _emailRepository;


		public TodoRepository(DCIdbContext context, IEmailRepository emailRepository)
		{
			this._dbContext = context;
			this._emailRepository = emailRepository;
			
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}


        public async Task<IList<LeaveRequestHeaderViewModel>> GetAllTodo(LeaveViewModel model)
        {
            
            var context = _dbContext.LeaveRequestHeader.AsQueryable().ToList();
            var contextDetail = _dbContext.LeaveRequestDetails.AsQueryable().ToList();
            var empList = _dbContext.Employee.AsQueryable().ToList();
            var statusList = _dbContext.Status.AsQueryable().ToList();

            //var query = from leave in context
            //            .Include(l => l.LeaveRequestDetails)
            //            join emp in empList on leave.EmployeeId equals emp.EmployeeId
            //            join stat in statusList on leave.Status equals stat.StatusId
            //            where leave.IsActive == true
            //            select new LeaveRequestHeaderViewModel
            //            {
            //                LeaveRequestHeaderId = leave.LeaveRequestHeaderId,
            //                RequestNo = leave.RequestNo,
            //                EmployeeId = leave.EmployeeId,
            //                DateFiled = leave.DateFiled,
            //                Status = leave.Status,
            //                StatusName = stat.StatusName, 
            //                Reason = leave.Reason,
            //                NoofDays = leave.NoOfDays,
            //                LeaveDateList = leave.LeaveRequestDetailList
            //                    .Select(dtl => dtl.LeaveDate.ToShortDateString())
            //                    .ToList()
            //            };

            var query = from leave in context
                            //  join dtl in _dbContext.LeaveRequestDetails on leave.LeaveRequestHeaderId equals dtl.LeaveRequestHeaderId
                        join emp in empList on leave.EmployeeId equals emp.EmployeeId
                        join stat in statusList on leave.Status equals stat.StatusId
                        where leave.IsActive == true && leave.Status == (int)EnumStatus.Pending
                        select new LeaveRequestHeaderViewModel
                        {
                            LeaveRequestHeaderId = leave.LeaveRequestHeaderId,
                            RequestNo = leave.RequestNo,
                            EmployeeId = leave.EmployeeId,
                            EmployeeName = emp.Firstname + " " +emp.Lastname,
                            DateFiled = leave.DateFiled,
                            Status = leave.Status,
                            StatusName = stat.StatusName,
                            Reason = leave.Reason,
                            NoofDays = leave.NoOfDays
                           // ,LeaveDateList = leave.LeaveRequestDetailsList.Select(dtl => dtl.LeaveDate).ToList()
                        };


            return query.ToList();
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
               // var contextDtl = _dbContext.LeaveRequestDetails.Where(x => x.LeaveRequestHeaderId == param.TransactionId);

                if (contextHdr != null) 
                {               
                    var contextLeaveInfo = _dbContext.LeaveInfo.Where(x => x.EmployeeId == contextHdr.EmployeeId).FirstOrDefault();             

                    if (contextHdr.LeaveTypeId == (int)EnumLeaveType.VL)
                    {
                        contextLeaveInfo.VLBalance = contextLeaveInfo.VLBalance - contextHdr.NoOfDays;               
                        _dbContext.LeaveInfo.Entry(contextLeaveInfo).State = EntityState.Modified;
                        _dbContext.SaveChanges();
                    }
                    else if(contextHdr.LeaveTypeId == (int)EnumLeaveType.SL)
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

                //if (NewDocumentViewModel.StatusId == (int)EnumDocumentStatus.ForApproval ||
                //    NewDocumentViewModel.StatusId == (int)EnumDocumentStatus.Approved ||
                //    NewDocumentViewModel.StatusId == (int)EnumDocumentStatus.InProgress)
                //{
                //    await _emailRepository.SendRequestor(NewDocumentViewModel, apprvm);
                //}
                //if (NewDocumentViewModel.StatusId == (int)EnumDocumentStatus.ForApproval ||
                //    NewDocumentViewModel.StatusId == (int)EnumDocumentStatus.ForReview)
                //{
                //    await _emailRepository.SendApproval(NewDocumentViewModel);
                //}
                string status = param.Status == 1 ? "approved" : "disapproved";

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
    }
}
