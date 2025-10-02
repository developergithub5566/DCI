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
            // var biometriclogs = _dbContext.vw_AttendanceSummary.AsQueryable();
            // var wfhlogs = _dbContext.vw_AttendanceSummary.AsQueryable();


            var biometriclogs = await (from dtr in _dbContext.vw_AttendanceSummary.AsQueryable()                                     
                                       orderby dtr.DATE descending, dtr.NAME descending
                                       select new DailyTimeRecordViewModel
                                       {
                                           ID = dtr.ID,
                                           EMPLOYEE_NO = dtr.EMPLOYEE_NO,
                                           NAME = dtr.NAME,
                                           DATE = dtr.DATE,
                                           FIRST_IN = dtr.FIRST_IN,
                                           LAST_OUT = dtr.LAST_OUT,
                                           LATE = dtr.LATE,
                                           CLOCK_OUT = dtr.CLOCK_OUT,
                                           UNDER_TIME = dtr.UNDER_TIME,
                                           OVERTIME = dtr.OVERTIME,
                                           TOTAL_HOURS = dtr.TOTAL_HOURS,
                                           TOTAL_WORKING_HOURS = dtr.TOTAL_WORKING_HOURS,
                                           SOURCE =  "BIOMETRICS"
                                       }).ToListAsync();


            var wfhlogs = await (from dtr in _dbContext.vw_AttendanceSummary_WFH.AsQueryable()
                                 where dtr.STATUS == (int)EnumStatus.Approved
                                 orderby dtr.DATE descending, dtr.NAME descending
                                 select new DailyTimeRecordViewModel
                                 {
                                     ID = dtr.ID,
                                     EMPLOYEE_NO = dtr.EMPLOYEE_NO,
                                     NAME = dtr.NAME,
                                     DATE = dtr.DATE,
                                     FIRST_IN = dtr.FIRST_IN,
                                     LAST_OUT = dtr.LAST_OUT,
                                     LATE = dtr.LATE,
                                     CLOCK_OUT = dtr.CLOCK_OUT,
                                     UNDER_TIME = dtr.UNDER_TIME,
                                     OVERTIME = dtr.OVERTIME,
                                     TOTAL_HOURS = dtr.TOTAL_HOURS,
                                     TOTAL_WORKING_HOURS = dtr.TOTAL_WORKING_HOURS,
                                     SOURCE = "REMOTE"
                                 }).ToListAsync();

            var holiday = await (from hol in _dbContext.Holiday.AsQueryable()
                                 where hol.IsActive == true                           
                                 select new DailyTimeRecordViewModel
                                 {
                                     ID = 0,
                                     EMPLOYEE_NO = string.Empty,
                                     NAME = hol.HolidayName, 
                                     DATE = hol.HolidayDate.Date,
                                     FIRST_IN = string.Empty,
                                     LAST_OUT = string.Empty,
                                     LATE = string.Empty,
                                     CLOCK_OUT = string.Empty,
                                     UNDER_TIME = string.Empty,
                                     OVERTIME = string.Empty,
                                     TOTAL_HOURS = string.Empty,
                                     TOTAL_WORKING_HOURS = string.Empty,
                                     SOURCE = hol.HolidayType == 3 ? "SUSPENSION" : "HOLIDAY"
                                 }).ToListAsync();


            var attendance = biometriclogs.Concat(wfhlogs).ToList();

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
            var context = _dbContext.vw_AttendanceSummary.AsQueryable();


            var query = (from dtr in context
                         where dtr.EMPLOYEE_NO == empNo
                         select new DailyTimeRecordViewModel
                         {
                             ID = dtr.ID,
                             EMPLOYEE_NO = dtr.EMPLOYEE_NO,
                             NAME = dtr.NAME,
                             DATE = dtr.DATE,
                             FIRST_IN = dtr.FIRST_IN,
                             LAST_OUT = dtr.LAST_OUT,
                             LATE = dtr.LATE,
                             CLOCK_OUT = dtr.CLOCK_OUT,
                             UNDER_TIME = dtr.UNDER_TIME,
                             OVERTIME = dtr.OVERTIME,
                             TOTAL_HOURS = dtr.TOTAL_HOURS,
                             TOTAL_WORKING_HOURS = dtr.TOTAL_WORKING_HOURS
                         }).ToList();


            return query;
        }

        public async Task<IList<DailyTimeRecordViewModel>> GetBiometricLogsByEmployeeId(DailyTimeRecordViewModel model)
        {
            var _emp = await _dbContext.Employee.AsQueryable().Where(x => x.EmployeeId == model.EMPLOYEE_ID).FirstOrDefaultAsync();
            var empNO = _emp.EmployeeNo ?? string.Empty;

            var query = await (from logs in _dbContext.tbl_raw_logs.AsQueryable()
                               where logs.EMPLOYEE_ID == empNO && logs.DATE_TIME.Date == model.DATE.Date
                               select new DailyTimeRecordViewModel
                               {
                                   DATESTRING = logs.DATE_TIME.ToString("yyyy-MM-dd"),
                                   EMPLOYEE_NO = logs.EMPLOYEE_ID,
                                   FIRST_IN = logs.DATE_TIME.ToString("HH:mm:ss")
                               }).ToListAsync();
            return query;
        }

        public async Task<IList<DTRCorrectionViewModel>> GetAllDTRCorrection(DTRCorrectionViewModel model)
        {
            var query = (from dtr in _dbContext.DTRCorrection.AsQueryable()
                         join emp in _dbContext.Employee on dtr.EmployeeId equals emp.EmployeeId
                         join stat in _dbContext.Status on dtr.Status equals stat.StatusId
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
                             DepartmentHead = depthead.Firstname + " " + depthead.Lastname// + " " + depthead.Suffix
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
                    notifvmToApprover.URL = "/Todo/DTR";
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
                    notifvmToRequestor.URL = "/Home/Notification";
                    notifvmToRequestor.MarkRead = false;
                    notifvmToRequestor.CreatedBy = param.CreatedBy;
                    notifvmToRequestor.IsActive = true;
                    await _homeRepository.SaveNotification(notifvmToRequestor);


                    return (StatusCodes.Status200OK, string.Format("DTR request {0} has been submitted for approval.", entity.RequestNo));
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
                return (StatusCodes.Status200OK, "Successfully cancelled");
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
                int _currentMonth = DateTime.Now.Month;
                var _dtr = await _dbContext.DTRCorrection
                                                .Where(x => x.IsActive == true && x.DateFiled.Date.Year == _currentYear && x.DateFiled.Date.Month == _currentMonth)
                                                .AsQueryable()
                                                .ToListAsync();


                int totalrecords = _dtr.Count() + 1;
                string finalSetRecords = GetFormattedRecord(totalrecords);
                string yearMonth = DateTime.Now.ToString("yyyyMM");
                string req = "DTR";

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
        
 
        //public async Task<IList<WFHViewModel>> GetAllWFH(WFHViewModel model)
        //{
        //    var query = (from dtr in _dbContext.tbl_wfh_logs
        //                 select new WFHViewModel
        //                 {
        //                     ID = dtr.ID,
        //                     EMPLOYEE_NO = dtr.EMPLOYEE_ID,
        //                     FULL_NAME = dtr.FULL_NAME,
        //                     DATE_TIME = dtr.DATE_TIME

        //                 }).ToList();
        //    if ((int)EnumEmployeeScope.PerEmployee == model.ScopeTypeEmp)
        //    {
        //        //var usr = _dbContext.User.Where(x => x.UserId == model.CurrentUserId).FirstOrDefault();
        //        var emp = _dbContext.Employee.Where(x => x.EmployeeId == model.EMPLOYEE_ID).FirstOrDefault();
        //        if (emp != null)
        //            query = query.Where(x => x.EMPLOYEE_NO == emp.EmployeeNo).ToList();
        //    }
        //    return query;
        //}


    }
}
