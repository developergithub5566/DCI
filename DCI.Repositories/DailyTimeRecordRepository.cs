using DCI.Core.Common;
using DCI.Core.Helpers;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Serilog;
using System.ComponentModel;
using System.Reflection.PortableExecutable;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static QRCoder.PayloadGenerator;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DCI.Repositories
{
    public class DailyTimeRecordRepository : IDailyTimeRecordRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        private readonly IHomeRepository _homeRepository;
        private IEmailRepository _emailRepository;

        public DailyTimeRecordRepository(DCIdbContext dbContext, IHomeRepository homeRepository, IEmailRepository emailRepository)
        {
            _dbContext = dbContext;
            _homeRepository = homeRepository;
            _emailRepository = emailRepository;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<IList<DailyTimeRecordViewModel>> GetAllDTR(DailyTimeRecordViewModel model)
        {
            var biometriclogs = await (from dtr in _dbContext.vw_AttendanceSummary.AsNoTracking()
                                       join emp in _dbContext.Employee.AsNoTracking() on dtr.EMPLOYEE_NO equals emp.EmployeeNo
                                       orderby dtr.DATE descending
                                       select new DailyTimeRecordViewModel
                                       {
                                           ID = dtr.ID,
                                           EMPLOYEE_NO = dtr.EMPLOYEE_NO,
                                           NAME = emp.Firstname + " " + emp.Lastname,
                                           DATE = dtr.DATE,
                                           FIRST_IN = dtr.FIRST_IN,
                                           LAST_OUT = dtr.LAST_OUT,
                                           LATE = dtr.LATE,
                                           CLOCK_OUT = dtr.CLOCK_OUT,
                                           UNDER_TIME = dtr.UNDER_TIME,
                                           OVERTIME = dtr.OVERTIME,
                                           TOTAL_HOURS = dtr.TOTAL_HOURS,
                                           TOTAL_WORKING_HOURS = dtr.TOTAL_WORKING_HOURS,
                                           SOURCE = Constants.Source_Biometrics
                                       }).ToListAsync();


            var wfhlogs = await (from dtr in _dbContext.vw_AttendanceSummary_WFH.AsNoTracking()
                                 join emp in _dbContext.Employee.AsNoTracking() on dtr.EMPLOYEE_NO equals emp.EmployeeNo
                                 where dtr.STATUS == (int)EnumStatus.Approved
                                 orderby dtr.DATE descending
                                 select new DailyTimeRecordViewModel
                                 {
                                     ID = dtr.ID,
                                     EMPLOYEE_NO = dtr.EMPLOYEE_NO,
                                     NAME = emp.Firstname + " " + emp.Lastname,
                                     DATE = dtr.DATE,
                                     FIRST_IN = dtr.FIRST_IN,
                                     LAST_OUT = dtr.LAST_OUT,
                                     LATE = dtr.LATE,
                                     CLOCK_OUT = dtr.CLOCK_OUT,
                                     UNDER_TIME = dtr.UNDER_TIME,
                                     OVERTIME = dtr.OVERTIME,
                                     TOTAL_HOURS = dtr.TOTAL_HOURS,
                                     TOTAL_WORKING_HOURS = dtr.TOTAL_WORKING_HOURS,
                                     SOURCE = Constants.Source_Remote
                                 }).ToListAsync();

            var holiday = await (from hol in _dbContext.Holiday.AsNoTracking()
                                 where hol.IsActive == true
                                 select new DailyTimeRecordViewModel
                                 {
                                     ID = 0,
                                     EMPLOYEE_NO = string.Empty,
                                     NAME = hol.HolidayName,
                                     DATE = hol.HolidayDate.Date,
                                     FIRST_IN = "00:00:00",
                                     LAST_OUT = "00:00:00",
                                     LATE = "00:00:00",
                                     CLOCK_OUT = "00:00:00",
                                     UNDER_TIME = "00:00:00",
                                     OVERTIME = "00:00:00",
                                     TOTAL_HOURS = "00:00:00",
                                     TOTAL_WORKING_HOURS = "00:00:00",
                                     SOURCE = hol.HolidayType == (int)EnumHoliday.Suspension ? Constants.Source_Suspension : Constants.Source_Holiday
                                 }).ToListAsync();

            var filterDate = new DateTime(2025, 10, 1); //migration start Oct28

            //var officialBusiness = await (from dtl in _dbContext.LeaveRequestDetails.AsNoTracking()
            //                              join ob in _dbContext.LeaveRequestHeader.AsNoTracking() on dtl.LeaveRequestHeaderId equals ob.LeaveRequestHeaderId
            //                              join emp in _dbContext.Employee.AsNoTracking() on ob.EmployeeId equals emp.EmployeeId
            //                               where 
            //                               ob.IsActive == true 
            //                               && (ob.LeaveTypeId == (int)EnumLeaveType.OB || ob.LeaveTypeId == (int)EnumLeaveType.HDOB) 
            //                               && ob.Status == (int)EnumStatus.Approved 
            //                               && ob.Status == (int)EnumStatus.Approved && dtl.LeaveDate >= filterDate
            //                              select new DailyTimeRecordViewModel
            //                     {
            //                         ID = 0,
            //                         EMPLOYEE_NO = emp.EmployeeNo,
            //                         NAME = emp.Firstname + " " + emp.Lastname,
            //                         DATE = dtl.LeaveDate.Date,
            //                         FIRST_IN = "00:00:00",
            //                         LAST_OUT = "00:00:00",
            //                         LATE = "00:00:00",
            //                         CLOCK_OUT = "00:00:00",
            //                         UNDER_TIME = "00:00:00",
            //                         OVERTIME = "00:00:00",
            //                         TOTAL_HOURS = "00:00:00",
            //                         TOTAL_WORKING_HOURS = "00:00:00",
            //                         SOURCE = Constants.Source_OfficialBusiness
            //                     }).ToListAsync();
            //string statusClass = item.STATUS switch
            //{
            //    (int)EnumStatus.Approved => "btn-primary",
            //    (int)EnumStatus.ForApproval => "btn-success",
            //    (int)EnumStatus.Rejected => "btn-danger",
            //    (int)EnumStatus.Cancelled => "btn-secondary",
            //    (int)EnumStatus.VLDeducted
            //    or (int)EnumStatus.PayrollDeducted => "btn-warning",
            //    _ => "btn-dark"
            //};

            var officialBusiness = await (from dtl in _dbContext.LeaveRequestDetails.AsNoTracking()
                                          join ob in _dbContext.LeaveRequestHeader.AsNoTracking() on dtl.LeaveRequestHeaderId equals ob.LeaveRequestHeaderId
                                          join emp in _dbContext.Employee.AsNoTracking() on ob.EmployeeId equals emp.EmployeeId
                                          join stat in _dbContext.Status.AsNoTracking() on ob.Status equals stat.StatusId
                                          where
                                          ob.IsActive == true
                                          && (ob.LeaveTypeId != (int)EnumLeaveType.SLMon && ob.LeaveTypeId != (int)EnumLeaveType.VLMon)
                                         // && ob.Status == (int)EnumStatus.Approved
                                          && dtl.LeaveDate >= filterDate
                                          select new DailyTimeRecordViewModel
                                          {
                                              ID = 0,
                                              EMPLOYEE_NO = emp.EmployeeNo,
                                              NAME = emp.Firstname + " " + emp.Lastname,
                                              DATE = dtl.LeaveDate.Date,
                                              FIRST_IN = "00:00:00",
                                              LAST_OUT = "00:00:00",
                                              LATE = "00:00:00",
                                              CLOCK_OUT = "00:00:00",
                                              UNDER_TIME = "00:00:00",
                                              OVERTIME = "00:00:00",
                                              TOTAL_HOURS = "00:00:00",
                                              TOTAL_WORKING_HOURS = "00:00:00",
                                              // SOURCE = Constants.Source_OfficialBusiness
                                              STATUSNAME = stat.StatusName,
                                              RequestNo = ob.RequestNo,
                                              SOURCE =
                                                    (ob.LeaveTypeId == (int)EnumLeaveType.OB || ob.LeaveTypeId == (int)EnumLeaveType.HDOB) ? Constants.Source_OfficialBusiness
                                                        : (ob.LeaveTypeId == (int)EnumLeaveType.VL || ob.LeaveTypeId == (int)EnumLeaveType.HDVL) ? Constants.Source_VacationLeave
                                                        : (ob.LeaveTypeId == (int)EnumLeaveType.SL || ob.LeaveTypeId == (int)EnumLeaveType.HDSL) ? Constants.Source_SickLeave
                                                        : ob.LeaveTypeId == (int)EnumLeaveType.SPL ? Constants.Source_SpecialLeave
                                                        : ob.LeaveTypeId == (int)EnumLeaveType.ML ? Constants.Source_MaternityLeave
                                                        : ob.LeaveTypeId == (int)EnumLeaveType.PL ? Constants.Source_PaternityLeave
                                                        : "Leave"
                                          }).ToListAsync();


            var attendance = biometriclogs.Concat(wfhlogs).Concat(officialBusiness).ToList();

            if ((int)EnumEmployeeScope.PerEmployee == model.ScopeTypeEmp)
            {
                var usr = _dbContext.User.Where(x => x.UserId == model.CurrentUserId).FirstOrDefault();
                var emp = _dbContext.Employee.Where(x => x.EmployeeId == usr.EmployeeId).FirstOrDefault();
                if (emp != null)
                    attendance = attendance.Where(x => x.EMPLOYEE_NO == emp.EmployeeNo).ToList();
            }
    
            return attendance.Concat(holiday).ToList();
        }

        public async Task<IList<DailyTimeRecordViewModel>> GetAllDTRByEmpNo(string empNo)
        {
            //var context = _dbContext.vw_AttendanceSummary.AsQueryable();


            var query = await (from dtr in _dbContext.vw_AttendanceSummary.AsNoTracking()
                               join emp in _dbContext.Employee.AsNoTracking() on dtr.EMPLOYEE_NO equals emp.EmployeeNo
                               where dtr.EMPLOYEE_NO == empNo
                               select new DailyTimeRecordViewModel
                               {
                                   ID = dtr.ID,
                                   EMPLOYEE_NO = dtr.EMPLOYEE_NO,
                                   NAME = emp.Firstname + " " + emp.Lastname,
                                   DATE = dtr.DATE,
                                   FIRST_IN = dtr.FIRST_IN,
                                   LAST_OUT = dtr.LAST_OUT,
                                   LATE = dtr.LATE,
                                   CLOCK_OUT = dtr.CLOCK_OUT,
                                   UNDER_TIME = dtr.UNDER_TIME,
                                   OVERTIME = dtr.OVERTIME,
                                   TOTAL_HOURS = dtr.TOTAL_HOURS,
                                   TOTAL_WORKING_HOURS = dtr.TOTAL_WORKING_HOURS
                               }).ToListAsync();


            return query;
        }

        public async Task<IList<DailyTimeRecordViewModel>> GetBiometricLogsByEmployeeId(DailyTimeRecordViewModel model)
        {
            var _emp = await _dbContext.Employee.AsNoTracking().Where(x => x.EmployeeId == model.EMPLOYEE_ID).FirstOrDefaultAsync();
            var empNO = _emp.EmployeeNo ?? string.Empty;

            var query = await (from logs in _dbContext.tbl_raw_logs.AsQueryable()
                               where logs.EMPLOYEE_ID == empNO && logs.DATE_TIME.Date == model.DATE.Date
                               select new DailyTimeRecordViewModel
                               {
                                   DATESTRING = logs.DATE_TIME.ToString("yyyy-MM-dd"),
                                   EMPLOYEE_NO = logs.EMPLOYEE_ID,
                                   FIRST_IN = logs.DATE_TIME.ToString("HH:mm:ss"),
                                   //  SOURCE = logs.STATUS == 11 ? "DTR Adjustment" : "BIOMETRICS"
                               }).ToListAsync();
            return query;
        }

        public async Task<IList<DTRCorrectionViewModel>> GetAllDTRCorrection(DTRCorrectionViewModel model)
        {
            var query = (from dtr in _dbContext.DTRCorrection.AsNoTracking()
                         join emp in _dbContext.Employee.AsNoTracking() on dtr.EmployeeId equals emp.EmployeeId
                         join stat in _dbContext.Status.AsNoTracking() on dtr.Status equals stat.StatusId
                         where dtr.CreatedBy == model.CreatedBy
                         select new DTRCorrectionViewModel
                         {
                             DtrId = dtr.DtrId,
                             RequestNo = dtr.RequestNo,
                             EmployeeName = emp.Firstname + " " + emp.Lastname,
                             DateFiled = dtr.DateFiled,
                             DtrType = dtr.DtrType,
                             DtrDateTime = dtr.DtrDateTime,
                             Status = dtr.Status,
                             StatusName = stat.StatusName,
                             Reason = dtr.Reason,
                             CreatedBy = dtr.CreatedBy,
                             IsActive = dtr.IsActive
                         }).ToList();


            if ((int)EnumEmployeeScope.PerEmployee == model.ScopeTypeEmp)
            {
                query = query.Where(x => x.CreatedBy == model.CreatedBy).ToList();
            }
            else
            {
                query = query.Where(x => x.Status == (int)EnumStatus.Approved || x.Status == (int)EnumStatus.Rejected).ToList();
            }
            return query;
        }

        public async Task<DTRCorrectionViewModel> DTRCorrectionByDtrId(int dtrId)
        {
            var query = await (from dtr in _dbContext.DTRCorrection.AsNoTracking()
                               join stat in _dbContext.Status.AsNoTracking() on dtr.Status equals stat.StatusId
                               join emp in _dbContext.EmployeeWorkDetails.AsNoTracking() on dtr.EmployeeId equals emp.EmployeeId
                               //join dept in _dbContext.Department on emp.DepartmentId equals dept.DepartmentId
                               join depthead in _dbContext.User.AsNoTracking() on dtr.ApproverId equals depthead.UserId
                               join apprvl in _dbContext.ApprovalHistory.AsNoTracking().Where(x => x.ModulePageId == (int)EnumModulePage.DTRCorrection)
                               on dtr.DtrId equals apprvl.TransactionId into ah
                               from apprvl in ah.DefaultIfEmpty()
                               where dtr.DtrId == dtrId
                               select new DTRCorrectionViewModel
                               {
                                   DtrId = dtr.DtrId,
                                   RequestNo = dtr.RequestNo,
                                   DateFiled = dtr.DateFiled,
                                   DtrType = dtr.DtrType,
                                   DtrDateTime = dtr.DtrDateTime,
                                   Status = dtr.Status,
                                   StatusName = stat.StatusName,
                                   Reason = dtr.Reason,
                                   CreatedBy = dtr.CreatedBy,
                                   ModifiedBy = dtr.ModifiedBy,
                                   DateModified = dtr.DateModified,
                                   IsActive = dtr.IsActive,
                                   DateApprovedDisapproved = apprvl != null ? apprvl.DateCreated.ToString("yyyy-MM-dd HH:mm") : string.Empty,
                                   ApprovalRemarks = apprvl != null ? apprvl.Remarks : string.Empty,
                                   DepartmentHead = depthead.Fullname ,
                               }).FirstOrDefaultAsync();

            return query;
        }


        public async Task<(int statuscode, string message)> SaveDTRCorrection(DTRCorrectionViewModel param)
        {
            try
            {
                if (param.DtrId == 0)
                {
                    DTRCorrection entity = new DTRCorrection();
                    entity.DateFiled = DateTime.Now;
                    entity.EmployeeId = param.EmployeeId;
                    entity.RequestNo = await GenereteRequestNo();
                    entity.DtrType = param.DtrType;
                    entity.DateFiled = DateTime.Now;
                    entity.DtrDateTime = param.DtrDateTime;
                    entity.Status = (int)EnumStatus.ForApproval;
                    entity.ApproverId = param.ApproverId;
                    entity.RawLogsId = 0;
                    entity.Reason = param.Reason;
                    //entity.Filename = param.Filename;
                    //entity.FileLocation = param.FileLocation;
                    entity.CreatedBy = param.CreatedBy;
                    entity.IsActive = true;
                    await _dbContext.DTRCorrection.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();

                    //Send Email Notification to Approver
                    DTRCorrectionViewModel model = new DTRCorrectionViewModel();
                    model.ApproverId = param.ApproverId;
                    model.Status = entity.Status;
                    model.RequestNo = entity.RequestNo;
                    await _emailRepository.SentToApprovalDTRAdjustment(model);

                    //Send Application Notification to Approver
                    NotificationViewModel notifvmToApprover = new NotificationViewModel();
                    notifvmToApprover.Title = "DTR Adjustment";
                    notifvmToApprover.Description = System.String.Format("You have been assigned DTR adjustment request {0} for approval.", entity.RequestNo);
                    notifvmToApprover.ModuleId = (int)EnumModulePage.DTRCorrection;
                    notifvmToApprover.TransactionId = entity.DtrId;
                    notifvmToApprover.AssignId = param.ApproverId;
                    // notifvmToApprover.URL = "/Todo/Index/?leaveId=" + model.DtrId;
                    notifvmToApprover.URL = "/Todo/Index/";
                    notifvmToApprover.MarkRead = false;
                    notifvmToApprover.CreatedBy = param.CreatedBy;
                    notifvmToApprover.IsActive = true;
                    await _homeRepository.SaveNotification(notifvmToApprover);

                    //Send Application Notification to Requestor
                    NotificationViewModel notifvmToRequestor = new NotificationViewModel();
                    notifvmToRequestor.Title = "DTR Adjustment";
                    notifvmToRequestor.Description = System.String.Format("Your DTR adjustment request {0} has been submitted for approval.", entity.RequestNo);
                    notifvmToRequestor.ModuleId = (int)EnumModulePage.DTRCorrection;
                    notifvmToRequestor.TransactionId = entity.DtrId;
                    notifvmToRequestor.AssignId = param.CreatedBy;
                    //  notifvmToSelf.URL = "/Todo/Index/?leaveId=" + model.DtrId;
                    notifvmToRequestor.URL = "/DailyTimeRecord/DTRCorrection?DtrId=1";
                    notifvmToRequestor.MarkRead = false;
                    notifvmToRequestor.CreatedBy = param.CreatedBy;
                    notifvmToRequestor.IsActive = true;
                    await _homeRepository.SaveNotification(notifvmToRequestor);

                    return (StatusCodes.Status200OK,  $"DTR Adjustment request {entity.RequestNo} has been submitted for approval.");
                    // return (StatusCodes.Status200OK, string.Format("DTR request {0} has been submitted for approval.", entity.RequestNo));
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

        public async Task<(int statuscode, string message)> CancelDTRCorrection(DTRCorrectionViewModel model)
        {
            try
            {
                var entity = await _dbContext.DTRCorrection.FirstOrDefaultAsync(x => x.DtrId == model.DtrId && x.IsActive == true);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid DTR Adjustment Id");
                }
                entity.Status = (int)EnumStatus.Cancelled;
                entity.ModifiedBy = model.ModifiedBy;
                entity.DateModified = DateTime.Now;
                entity.IsActive = true;
                _dbContext.DTRCorrection.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                //Send Application Notification to Approver
                NotificationViewModel notifvmToApprover = new NotificationViewModel();
                notifvmToApprover.Title = "DTR Adjustment";
                notifvmToApprover.Description = System.String.Format("DTR adjustment request {0} has been cancelled by the requestor.", entity.RequestNo);
                notifvmToApprover.ModuleId = (int)EnumModulePage.DTRCorrection;
                notifvmToApprover.TransactionId = entity.DtrId;
                notifvmToApprover.AssignId = entity.ApproverId;
                notifvmToApprover.URL = "/Todo/Index/";
                notifvmToApprover.MarkRead = false;
                notifvmToApprover.CreatedBy = entity.CreatedBy;
                notifvmToApprover.IsActive = true;
                await _homeRepository.SaveNotification(notifvmToApprover);

                return (StatusCodes.Status200OK, System.String.Format("DTR adjustment request {0} has been cancelled.", entity.RequestNo));
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

        private async Task<string> GenereteRequestNo()
        {

            try
            {
                int _currentYear = DateTime.Now.Year;
                // int _currentMonth = DateTime.Now.Month;
                var _dtr = await _dbContext.DTRCorrection
                                                .Where(x => x.IsActive == true && x.DateFiled.Date.Year == _currentYear)
                                                .AsNoTracking()
                                                .ToListAsync();


                int totalrecords = _dtr.Count() + 1;
                string finalSetRecords = FormatHelper.GetFormattedRequestNo(totalrecords);
                string yearMonth = DateTime.Now.ToString("yyyyMM");
                string req = Constants.ModuleCode_DTR;

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

        //Job Trigger. Everyday Email Attendance Confirmation
        //Source Code from DCI.Trigger.API/AttendanceProcessor.cs
        public async Task<IList<DailyTimeRecordViewModel>> GetAllDTRByDate(DailyTimeRecordViewModel model) 
        {  
            var biometriclogs = await (from dtr in _dbContext.vw_AttendanceSummary.AsNoTracking()
                                       join emp in _dbContext.Employee.AsNoTracking() on dtr.EMPLOYEE_NO equals emp.EmployeeNo                                       
                                       orderby dtr.DATE descending
                                       select new DailyTimeRecordViewModel
                                       {
                                           ID = dtr.ID,
                                           EMPLOYEE_NO = dtr.EMPLOYEE_NO,
                                           NAME = emp.Firstname + " " + emp.Lastname,
                                           DATE = dtr.DATE,
                                           FIRST_IN = dtr.FIRST_IN,
                                           LAST_OUT = dtr.LAST_OUT,
                                           LATE = dtr.LATE,
                                           CLOCK_OUT = dtr.CLOCK_OUT,
                                           UNDER_TIME = dtr.UNDER_TIME,
                                           OVERTIME = dtr.OVERTIME,
                                           TOTAL_HOURS = dtr.TOTAL_HOURS,
                                           TOTAL_WORKING_HOURS = dtr.TOTAL_WORKING_HOURS,
                                           SOURCE = Constants.Source_Biometrics
                                       }).ToListAsync();

            if ((int)EnumScopeTypeJobRecurring.DAILY == model.ScopeTypeJobRecurring) //DAILY
            {             
                biometriclogs = biometriclogs.Where(x => x.DATE.Date == model.DATE.Date).ToList();
            }
            else  // MONTHLY
            {
                biometriclogs = biometriclogs.Where(x => x.DATE.Month == model.DATE.Month && x.DATE.Year == model.DATE.Year).ToList();
            }

                var wfhlogs = await (from dtr in _dbContext.vw_AttendanceSummary_WFH.AsNoTracking()
                                     join emp in _dbContext.Employee.AsNoTracking() on dtr.EMPLOYEE_NO equals emp.EmployeeNo
                                     where dtr.STATUS == (int)EnumStatus.Approved // && dtr.DATE.Date == model.DATE.Date
                                     orderby dtr.DATE descending
                                     select new DailyTimeRecordViewModel
                                     {
                                         ID = dtr.ID,
                                         EMPLOYEE_NO = dtr.EMPLOYEE_NO,
                                         NAME = emp.Firstname + " " + emp.Lastname,
                                         DATE = dtr.DATE,
                                         FIRST_IN = dtr.FIRST_IN,
                                         LAST_OUT = dtr.LAST_OUT,
                                         LATE = dtr.LATE,
                                         CLOCK_OUT = dtr.CLOCK_OUT,
                                         UNDER_TIME = dtr.UNDER_TIME,
                                         OVERTIME = dtr.OVERTIME,
                                         TOTAL_HOURS = dtr.TOTAL_HOURS,
                                         TOTAL_WORKING_HOURS = dtr.TOTAL_WORKING_HOURS,
                                         SOURCE = Constants.Source_Remote
                                     }).ToListAsync();


            if ((int)EnumScopeTypeJobRecurring.DAILY == model.ScopeTypeJobRecurring) //DAILY
            {
                wfhlogs = wfhlogs.Where(x => x.DATE.Date == model.DATE.Date).ToList();
            }
            else  // MONTHLY
            {
                wfhlogs = wfhlogs.Where(x => x.DATE.Month == model.DATE.Month && x.DATE.Year == model.DATE.Year).ToList();
            }
                      

            var officialBusiness = await (from dtl in _dbContext.LeaveRequestDetails.AsNoTracking()
                                          join ob in _dbContext.LeaveRequestHeader.AsNoTracking() on dtl.LeaveRequestHeaderId equals ob.LeaveRequestHeaderId
                                          join emp in _dbContext.Employee.AsNoTracking() on ob.EmployeeId equals emp.EmployeeId
                                          join lvtype in _dbContext.LeaveType.AsNoTracking() on ob.LeaveTypeId equals lvtype.LeaveTypeId
                                          where ob.IsActive == true
                                          //&& (ob.LeaveTypeId == (int)EnumLeaveType.OB || ob.LeaveTypeId == (int)EnumLeaveType.HDOB) 
                                          && (ob.LeaveTypeId != (int)EnumLeaveType.SLMon || ob.LeaveTypeId != (int)EnumLeaveType.VLMon) 
                                          && ob.Status == (int)EnumStatus.Approved
                                          //&& dtl.LeaveDate.Date == model.DATE.Date
                                          select new DailyTimeRecordViewModel
                                          {
                                              ID = 0,
                                              EMPLOYEE_NO = emp.EmployeeNo,
                                              NAME = emp.Firstname + " " + emp.Lastname,
                                              DATE = dtl.LeaveDate.Date,
                                              FIRST_IN = "00:00:00",
                                              LAST_OUT = "00:00:00",
                                              LATE = "00:00:00",
                                              CLOCK_OUT = "00:00:00",
                                              UNDER_TIME = "00:00:00",
                                              OVERTIME = "00:00:00",
                                              TOTAL_HOURS = "00:00:00",
                                              TOTAL_WORKING_HOURS = "00:00:00",
                                              //SOURCE = Constants.Source_OfficialBusiness
                                              SOURCE = lvtype.Description
                                          }).ToListAsync();

            if ((int)EnumScopeTypeJobRecurring.DAILY == model.ScopeTypeJobRecurring) //DAILY
            {
                officialBusiness = officialBusiness.Where(x => x.DATE.Date == model.DATE.Date).ToList();
            }
            else  // MONTHLY
            {
                officialBusiness = officialBusiness.Where(x => x.DATE.Month == model.DATE.Month && x.DATE.Year == model.DATE.Year).ToList();
            }


            var attendance = biometriclogs.Concat(wfhlogs).Concat(officialBusiness).ToList();


            if ((int)EnumScopeTypeJobRecurring.MONTHLY == model.ScopeTypeJobRecurring) //MONTHLY ONLY
            {
                //MONTHLY ONLY
                var holiday = await (from hol in _dbContext.Holiday.AsNoTracking()
                                     where hol.IsActive == true
                                     && hol.HolidayDate.Month == model.DATE.Month
                                     && hol.HolidayDate.Year == model.DATE.Year
                                     select new DailyTimeRecordViewModel
                                     {
                                         ID = 0,
                                         EMPLOYEE_NO = string.Empty,
                                         NAME = hol.HolidayName,
                                         DATE = hol.HolidayDate.Date,
                                         FIRST_IN = "00:00:00",
                                         LAST_OUT = "00:00:00",
                                         LATE = "00:00:00",
                                         CLOCK_OUT = "00:00:00",
                                         UNDER_TIME = "00:00:00",
                                         OVERTIME = "00:00:00",
                                         TOTAL_HOURS = "00:00:00",
                                         TOTAL_WORKING_HOURS = "00:00:00",
                                         SOURCE = hol.HolidayType == (int)EnumHoliday.Suspension ? Constants.Source_Suspension : Constants.Source_Holiday
                                     }).ToListAsync();

                attendance = attendance.Concat(holiday).ToList();
            }

            return attendance.ToList();
        }

    }
}
