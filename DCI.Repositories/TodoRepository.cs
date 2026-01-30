using DCI.Core.Common;
using DCI.Core.Helpers;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Globalization;


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
                              join leavetype in _dbContext.LeaveType.AsNoTracking() on leave.LeaveTypeId equals leavetype.LeaveTypeId
                              join stat in _dbContext.Status.AsNoTracking() on leave.Status equals stat.StatusId
                              where leave.IsActive == true && leave.Status == (int)EnumStatus.ForApproval && leave.ApproverId == model.CurrentUserId
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
                                  LeaveName = leavetype.Description,
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
                                         join usr in _dbContext.User.AsNoTracking() on dtr.CreatedBy equals usr.UserId
                                         join emp in _dbContext.Employee.AsNoTracking() on usr.EmployeeId equals emp.EmployeeId
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
                                where ot.IsActive && ot.StatusId == (int)EnumStatus.ForApproval && ot.ApproverId == model.CurrentUserId
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
                                       where hdr.IsActive && hdr.Status == (int)EnumStatus.ForApproval && hdr.ApproverId == model.CurrentUserId
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
                //var context = _dbContext.LeaveRequestHeader.AsNoTracking().ToList();
                //var contextDetail = _dbContext.LeaveRequestDetails.AsNoTracking().ToList();
                //var empList = _dbContext.Employee.AsNoTracking().ToList();
                //var statusList = _dbContext.Status.AsNoTracking().ToList();


                var query = (from leave in _dbContext.LeaveRequestHeader.AsNoTracking().ToList()
                             join emp in _dbContext.Employee.AsNoTracking().ToList() on leave.EmployeeId equals emp.EmployeeId
                             join stat in _dbContext.Status.AsNoTracking().ToList() on leave.Status equals stat.StatusId
                             where leave.IsActive == true && leave.Status == (int)EnumStatus.ForApproval && leave.ApproverId == model.CurrentUserId
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
                                 LeaveRequestDetailList = (from dtl in _dbContext.LeaveRequestDetails.AsNoTracking()
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
                              from dtr in _dbContext.DTRCorrection.AsNoTracking()
                              join usr in _dbContext.User.AsNoTracking() on dtr.CreatedBy equals usr.UserId
                              join emp in _dbContext.Employee.AsNoTracking() on usr.EmployeeId equals emp.EmployeeId
                              join stat in _dbContext.Status.AsNoTracking() on dtr.Status equals stat.StatusId
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
                //diff model but same process/computation etc. (LeaveRepository\LeaveDeduction.cs)

                int _currentYear = DateTime.Now.Year;
                var contextHdr = _dbContext.LeaveRequestHeader.Where(x => x.LeaveRequestHeaderId == param.TransactionId).FirstOrDefault();
                var contextDtl = _dbContext.LeaveRequestDetails.Where(x => x.IsActive && x.LeaveRequestHeaderId == contextHdr.LeaveRequestHeaderId).ToList();
                var emp = _dbContext.Employee.Where(x => x.EmployeeId == contextHdr.EmployeeId).FirstOrDefault();
                var empDetails = _dbContext.EmployeeWorkDetails.Where(x => x.EmployeeId == contextHdr.EmployeeId).FirstOrDefault();

                LeaveFormViewModel lvmodel = new LeaveFormViewModel();
                lvmodel.EmployeeId = contextHdr.EmployeeId;


                if (contextHdr != null && param.Status == (int)EnumStatus.Approved)
                {
                    if (empDetails?.EmployeeStatusId == (int)EnumEmploymentType.Regular)
                    {
                        var contextLeaveInfo = _dbContext.LeaveInfo.Where(x => x.EmployeeId == contextHdr.EmployeeId && x.DateCreated.Date.Year == _currentYear).OrderByDescending(x => x.DateCreated).FirstOrDefault();


                        if (contextHdr.LeaveTypeId == (int)EnumLeaveType.HDVL)
                        {
                            if (contextLeaveInfo.VLBalance >= contextHdr.NoOfDays)
                            {
                                contextLeaveInfo.VLBalance = contextLeaveInfo.VLBalance - contextHdr.NoOfDays;
                                contextHdr.DeductionType = (int)EnumDeductionType.VacationLeave;
                            }
                            else
                            {
                                decimal excessDays = contextHdr.NoOfDays - contextLeaveInfo.VLBalance;
                                contextLeaveInfo.VLBalance = 0;
                                contextHdr.DeductionType = (int)EnumDeductionType.VacationLeave;
                                contextHdr.NoOfDays = contextLeaveInfo.VLBalance;


                                lvmodel.NoOfDays = excessDays;
                                lvmodel.LeaveTypeId = contextHdr.LeaveTypeId;
                                lvmodel.Status = (int)EnumStatus.PayrollDeducted;
                                await _leaveRepository.ExcessLeaveDeduction(lvmodel);
                            }
                        }
                        if (contextHdr.LeaveTypeId == (int)EnumLeaveType.HDSL)
                        {
                            if (contextLeaveInfo.SLBalance >= contextHdr.NoOfDays)
                            {
                                contextLeaveInfo.SLBalance = contextLeaveInfo.SLBalance - contextHdr.NoOfDays;
                                contextHdr.DeductionType = (int)EnumDeductionType.SickLeave;
                            }
                            else
                            {
                                decimal excessDays = contextHdr.NoOfDays - contextLeaveInfo.SLBalance;
                                contextLeaveInfo.SLBalance = 0;
                                contextHdr.DeductionType = (int)EnumDeductionType.SickLeave;
                                contextHdr.NoOfDays = contextLeaveInfo.SLBalance;


                                lvmodel.NoOfDays = excessDays;
                                lvmodel.LeaveTypeId = contextHdr.LeaveTypeId;
                                lvmodel.Status = (int)EnumStatus.PayrollDeducted;
                                await _leaveRepository.ExcessLeaveDeduction(lvmodel);
                            }
                        }
                        else if (contextHdr.LeaveTypeId == (int)EnumLeaveType.VL || contextHdr.LeaveTypeId == (int)EnumLeaveType.VLMon)
                        {
                            if (contextLeaveInfo.VLBalance >= contextHdr.NoOfDays)
                            {
                                contextLeaveInfo.VLBalance = contextLeaveInfo.VLBalance - contextHdr.NoOfDays;
                                contextHdr.DeductionType = (int)EnumDeductionType.VacationLeave;
                            }
                            else
                            {
                                decimal excessDays = contextHdr.NoOfDays - contextLeaveInfo.VLBalance;
                                contextLeaveInfo.VLBalance = 0;
                                contextHdr.DeductionType = (int)EnumDeductionType.VacationLeave;
                                contextHdr.NoOfDays = contextLeaveInfo.VLBalance;


                                lvmodel.NoOfDays = excessDays;
                                lvmodel.LeaveTypeId = contextHdr.LeaveTypeId;
                                lvmodel.Status = (int)EnumStatus.PayrollDeducted;
                                await _leaveRepository.ExcessLeaveDeduction(lvmodel);
                            }
                        }
                        else if (contextHdr.LeaveTypeId == (int)EnumLeaveType.SL || contextHdr.LeaveTypeId == (int)EnumLeaveType.SLMon)
                        {
                            if (contextLeaveInfo.SLBalance >= contextHdr.NoOfDays)
                            {
                                contextLeaveInfo.SLBalance = contextLeaveInfo.SLBalance - contextHdr.NoOfDays;
                                contextHdr.DeductionType = (int)EnumDeductionType.SickLeave;
                            }
                            else
                            {
                                decimal excessDays = contextHdr.NoOfDays - contextLeaveInfo.SLBalance;
                                contextLeaveInfo.SLBalance = 0;
                                contextHdr.DeductionType = (int)EnumDeductionType.SickLeave;
                                contextHdr.NoOfDays = contextLeaveInfo.SLBalance;

                                lvmodel.NoOfDays = excessDays;
                                lvmodel.LeaveTypeId = contextHdr.LeaveTypeId;
                                lvmodel.Status = (int)EnumStatus.PayrollDeducted;
                                await _leaveRepository.ExcessLeaveDeduction(lvmodel);
                            }
                        }
                        else if (contextHdr.LeaveTypeId == (int)EnumLeaveType.SPL)
                        {
                            contextLeaveInfo.SPLBalance = contextLeaveInfo.SPLBalance - contextHdr.NoOfDays;
                            contextHdr.DeductionType = (int)EnumDeductionType.SpecialLeave;
                        }

                        _dbContext.LeaveInfo.Entry(contextLeaveInfo).State = EntityState.Modified;
                        _dbContext.SaveChanges();

                        if (contextHdr.LeaveTypeId != (int)EnumLeaveType.SLMon && contextHdr.LeaveTypeId != (int)EnumLeaveType.VLMon)
                        {
                            if (contextHdr.LeaveTypeId == (int)EnumLeaveType.SL
                               && contextHdr.LeaveTypeId == (int)EnumLeaveType.VL
                               && contextHdr.LeaveTypeId == (int)EnumLeaveType.SLMon
                               && contextHdr.LeaveTypeId == (int)EnumLeaveType.VLMon
                             )
                            {
                                foreach (var raw in contextDtl)
                                {
                                    //Update DTR attendance summary status to VL DEDUCTED                    
                                    await _dbContext.tbl_raw_logs.Where(x => x.EMPLOYEE_ID == emp.EmployeeNo && x.DATE_TIME.Date == raw.LeaveDate.Date)
                                                                 .ExecuteUpdateAsync(s => s
                                                                 .SetProperty(r => r.STATUS, r => (int)EnumStatus.VLDeducted));
                                }
                            }
                        }

                    }
                    else // probitionary and contractual/projectbased
                    {
                        if (contextHdr.LeaveTypeId == (int)EnumLeaveType.SL
                           && contextHdr.LeaveTypeId == (int)EnumLeaveType.VL
                           && contextHdr.LeaveTypeId == (int)EnumLeaveType.SLMon
                           && contextHdr.LeaveTypeId == (int)EnumLeaveType.VLMon
                         )
                        {
                            foreach (var raw in contextDtl)
                            {
                                //Update DTR attendance summary status to Payroll DEDUCTED                    
                                await _dbContext.tbl_raw_logs.Where(x => x.EMPLOYEE_ID == emp.EmployeeNo && x.DATE_TIME.Date == raw.LeaveDate.Date)
                                                             .ExecuteUpdateAsync(s => s
                                                             .SetProperty(r => r.STATUS, r => (int)EnumStatus.PayrollDeducted));
                            }
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
                entitiesToViewModel.LeaveTypeId = contextHdr.LeaveTypeId;
                // Send Email Notif
                await _emailRepository.SendToRequestorLeave(entitiesToViewModel);

                string status = param.Status == (int)EnumStatus.Approved ? "approved" : "disapproved";


                var user = _dbContext.User.Where(x => x.EmployeeId == contextHdr.EmployeeId).FirstOrDefault();

                string _leavetype = FormatHelper.GetLeaveTypeName(contextHdr.LeaveTypeId);
                TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;

                NotificationViewModel notifvm = new NotificationViewModel();
                notifvm.Title = $"{textInfo.ToTitleCase(_leavetype.ToLower())} Request";
                // notifvm.Description = String.Format("Leave request {0} has been {1}.", contextHdr.RequestNo, status);
                notifvm.Description = $"{textInfo.ToTitleCase(_leavetype.ToLower())} request {contextHdr.RequestNo} has been {status}.";
                notifvm.ModuleId = (int)EnumModulePage.Leave;
                notifvm.TransactionId = param.TransactionId;
                notifvm.AssignId = user.UserId;
                notifvm.URL = "/DailyTimeRecord/Leave";
                notifvm.MarkRead = false;
                notifvm.CreatedBy = param.CreatedBy;
                notifvm.IsActive = true;
                await _homeRepository.SaveNotification(notifvm);

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

                return (StatusCodes.Status200OK, $"{textInfo.ToTitleCase(_leavetype.ToLower())} request {contextHdr.RequestNo} has been {status}.");
                // return (StatusCodes.Status200OK, String.Format("Leave request {0} has been {1}.", contextHdr.RequestNo, status));
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
                var contextHdr = _dbContext.DTRCorrection.Where(x => x.DtrId == param.TransactionId).FirstOrDefault();

                var empwrk = _dbContext.EmployeeWorkDetails.Where(x => x.EmployeeId == contextHdr.EmployeeId).FirstOrDefault();

                var emp = _dbContext.Employee.Where(x => x.EmployeeId == contextHdr.EmployeeId).FirstOrDefault();

                if (empwrk != null && empwrk.WorkLocation == (int)EnumWorkLocation.MO)
                {
                    tbl_raw_logs raw_logs = new tbl_raw_logs();

                    var totalCount = await _dbContext.tbl_raw_logs
                                                        .AsNoTracking()
                                                        .CountAsync();

                    raw_logs.ID = totalCount + 1;
                    raw_logs.EMPLOYEE_ID = emp.EmployeeNo;
                    raw_logs.FULL_NAME = emp.Firstname + " " + emp.Lastname;
                    raw_logs.DATE_TIME = contextHdr.DtrDateTime;
                    raw_logs.CREATED_DATE = DateTime.Now;
                    raw_logs.CREATED_BY = Constants.SYSAD;
                    raw_logs.STATUS = (int)EnumStatus.Raw;
                    await _dbContext.tbl_raw_logs.AddAsync(raw_logs);
                    await _dbContext.SaveChangesAsync();

                    contextHdr.Status = param.Status;
                    contextHdr.RawLogsId = raw_logs.ID;
                    _dbContext.DTRCorrection.Entry(contextHdr).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                }
                else
                {   
                    tbl_wfh_logs wfh_logs = new tbl_wfh_logs();
                    var totalCount = await _dbContext.tbl_wfh_logs.AsNoTracking().CountAsync();

                  //  wfh_logs.ID = totalCount + 1;
                    wfh_logs.EMPLOYEE_ID = emp.EmployeeNo;
                    wfh_logs.FULL_NAME = emp.Firstname + " " + emp.Lastname;
                    wfh_logs.DATE_TIME = contextHdr.DtrDateTime;
                    wfh_logs.CREATED_DATE = DateTime.Now;
                    wfh_logs.CREATED_BY = Constants.SYSAD;
                    wfh_logs.STATUS = (int)EnumStatus.Draft;
                    await _dbContext.tbl_wfh_logs.AddAsync(wfh_logs);
                    await _dbContext.SaveChangesAsync();

                    contextHdr.Status = param.Status;
                    contextHdr.RawLogsId = wfh_logs.ID;
                    _dbContext.DTRCorrection.Entry(contextHdr).State = EntityState.Modified;
                    _dbContext.SaveChanges();
                }



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
                notifvm.URL = "/DailyTimeRecord/DTRCorrection";
                notifvm.MarkRead = false;
                notifvm.CreatedBy = param.CreatedBy;
                notifvm.IsActive = true;
                await _homeRepository.SaveNotification(notifvm);


                return (StatusCodes.Status200OK, String.Format("DTR Adjustment request {0} has been {1}.", contextHdr.RequestNo, status));
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

                /**
                 var attendanceList = await _dbContext.vw_AttendanceSummary_WFH 
                    .Where(x => attendanceIds.Contains((int)x.ID))
                    .ToListAsync();

                var empNoDatePairs = attendanceList
                    .Select(a => new { a.EMPLOYEE_NO, a.DATE })
                    .ToList(); 2026.01.30**/

                var wfhapplicationList =
                  from wfh in _dbContext.WfhApplication.AsNoTracking()
                  join usr in _dbContext.User.AsNoTracking() on wfh.EmployeeId equals usr.EmployeeId
                  select new 
                  {
                      ID = wfh.Id,
                      EMPLOYEE_NO = usr.EmployeeNo,
                      DATE = wfh.Date,
                  };

                var attendanceList = await wfhapplicationList
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

                if (param.Status == (int)EnumStatus.Rejected) //2025.12.15
                {
                    param.Status = (int)EnumStatus.Draft;
                }

                foreach (var log in wfhLogs)
                {
                    log.STATUS = param.Status;
                }


                await _dbContext.SaveChangesAsync();
                //OPTIMIZED CODE CHATGPT END

                WFHHeaderViewModel emailvm = new WFHHeaderViewModel();
                emailvm.RequestNo = contextHdr.RequestNo;
                emailvm.StatusId = contextHdr.Status;
                emailvm.ApproverId = contextHdr.ApproverId;
                emailvm.CreatedBy = contextHdr.CreatedBy;
                // Send Email Notif
                await _emailRepository.SendToRequestorWFH(emailvm);

                string status = param.Status == (int)EnumStatus.Approved ? "approved" : "disapproved";

                NotificationViewModel notifvm = new NotificationViewModel();
                notifvm.Title = "WFH";
                notifvm.Description = String.Format("WFH Request {0} has been {1}", contextHdr.RequestNo, status);
                notifvm.ModuleId = (int)EnumModulePage.WFH;
                notifvm.TransactionId = param.TransactionId;
                notifvm.AssignId = contextHdr.CreatedBy;
                notifvm.URL = "/DailyTimeRecord/WFHApplication";
                notifvm.MarkRead = false;
                notifvm.CreatedBy = param.CurrentUserId;
                notifvm.IsActive = true;
                await _homeRepository.SaveNotification(notifvm);

                return (StatusCodes.Status200OK, String.Format("WFH Request {0} has been {1}.", contextHdr.RequestNo, status));
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

            var query = from ot in _dbContext.OvertimeHeader.AsNoTracking()
                        join usr in _dbContext.User.AsNoTracking() on ot.CreatedBy equals usr.UserId
                        join emp in _dbContext.Employee.AsNoTracking() on usr.EmployeeId equals emp.EmployeeId
                        join stat in _dbContext.Status.AsNoTracking() on ot.StatusId equals stat.StatusId
                        // where ot.IsActive && ot.CreatedBy == model.CurrentUserId
                        where ot.IsActive && ot.StatusId == (int)EnumStatus.ForApproval && ot.ApproverId == model.CurrentUserId
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
            var query = await (from hdr in _dbContext.WfhHeader.AsNoTracking()
                               join emp in _dbContext.Employee.AsNoTracking() on hdr.EmployeeId equals emp.EmployeeId
                               join stat in _dbContext.Status.AsNoTracking() on hdr.Status equals stat.StatusId
                               where hdr.IsActive && hdr.Status == (int)EnumStatus.ForApproval && hdr.ApproverId == model.CurrentUserId
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
                entity.ModulePageId = (int)EnumModulePage.Overtime;
                entity.TransactionId = param.TransactionId;
                entity.ApproverId = param.ApproverId;
                entity.Status = param.Status;
                entity.Remarks = param.Remarks;
                entity.CreatedBy = param.CreatedBy;
                entity.DateCreated = DateTime.Now;
                entity.IsActive = true;
                await _dbContext.ApprovalHistory.AddAsync(entity);
                await _dbContext.SaveChangesAsync();


                var contextOT = await _dbContext.OvertimeHeader.Where(x => x.OTHeaderId == param.TransactionId).FirstOrDefaultAsync();
                contextOT.StatusId = param.Status;
                contextOT.ModifiedBy = param.CurrentUserId;
                contextOT.DateModified = DateTime.Now;
                _dbContext.OvertimeHeader.Entry(contextOT).State = EntityState.Modified;
                _dbContext.SaveChanges();

                // Send Email Notif
                OvertimeViewModel otvm = new OvertimeViewModel();
                otvm.RequestNo = contextOT.RequestNo;
                otvm.StatusId = contextOT.StatusId;
                otvm.CreatedBy = contextOT.CreatedBy;
                await _emailRepository.SendToRequestorOT(otvm);

                string status = param.Status == (int)EnumStatus.Approved ? "approved" : "disapproved";

                NotificationViewModel notifvm = new NotificationViewModel();
                notifvm.Title = "Overtime";
                notifvm.Description = $"Overtime request {contextOT.RequestNo} has been {status}.";
                notifvm.ModuleId = (int)EnumModulePage.Overtime;
                notifvm.TransactionId = param.TransactionId;
                notifvm.AssignId = contextOT.CreatedBy;
                notifvm.URL = "/DailyTimeRecord/Overtime";
                notifvm.MarkRead = false;
                notifvm.CreatedBy = param.CurrentUserId;
                notifvm.IsActive = true;
                await _homeRepository.SaveNotification(notifvm);

                return (StatusCodes.Status200OK, String.Format("Overtime request {0} has been {1}.", contextOT.RequestNo, status));
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
