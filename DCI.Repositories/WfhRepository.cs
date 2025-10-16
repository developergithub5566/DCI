using DCI.Core.Common;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
    public class WfhRepository : IWfhRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        private readonly IHomeRepository _homeRepository;
        private IEmailRepository _emailRepository;

        public WfhRepository(DCIdbContext dbContext, IHomeRepository homeRepository, IEmailRepository emailRepository)
        {
            _dbContext = dbContext;
            _homeRepository = homeRepository;
            _emailRepository = emailRepository;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<IList<DailyTimeRecordViewModel>> GetAllWFH(DailyTimeRecordViewModel model)
        {
            //var context = _dbContext.vw_AttendanceSummary_WFH.AsQueryable();
            var query = await (from dtr in _dbContext.vw_AttendanceSummary_WFH.AsNoTracking()
                               join emp in _dbContext.Employee on dtr.EMPLOYEE_NO equals emp.EmployeeNo
                               join stat in _dbContext.Status on dtr.STATUS equals stat.StatusId
                               orderby dtr.DATE descending//, dtr.NAME descending
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
                                   STATUS = dtr.STATUS,
                                   STATUSNAME = stat.StatusName
                               }).ToListAsync();

            if ((int)EnumEmployeeScope.PerEmployee == model.ScopeTypeEmp)
            {
                //var usr = _dbContext.User.Where(x => x.UserId == model.CurrentUserId).FirstOrDefault();
                var emp = _dbContext.Employee.AsNoTracking().Where(x => x.EmployeeId == model.EMPLOYEE_ID).FirstOrDefault();
                if (emp != null)
                    query = query.Where(x => x.EMPLOYEE_NO == emp.EmployeeNo).ToList();
            }

            return query;
        }

        public async Task<(int statuscode, string message)> SaveWFHTimeIn(WFHViewModel model)
        {

            try
            {
                var empdtl = await _dbContext.Employee.Where(x => x.EmployeeId == model.EMPLOYEE_ID).FirstOrDefaultAsync();
                model.EMPLOYEE_NO = empdtl.EmployeeNo ?? string.Empty;
                model.FULL_NAME = empdtl.Firstname + " " + empdtl.Lastname;
                model.CREATED_BY = Constants.SYSAD;

                tbl_wfh_logs entity = new tbl_wfh_logs();
                entity.EMPLOYEE_ID = model.EMPLOYEE_NO;
                entity.FULL_NAME = model.FULL_NAME;
                entity.DATE_TIME = DateTime.Now;
                entity.CREATED_DATE = DateTime.Now;
                entity.CREATED_BY = model.CREATED_BY;
                entity.STATUS = (int)EnumStatus.Draft;
                await _dbContext.tbl_wfh_logs.AddAsync(entity);
                await _dbContext.SaveChangesAsync();

                return (StatusCodes.Status200OK, "WFH time-in recorded successfully.");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                return (StatusCodes.Status404NotFound, ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }        
        }


        public async Task<IList<WFHHeaderViewModel>> GetAllWFHApplication(WFHHeaderViewModel model)
        {
           // var context = _dbContext.WfhHeader.AsQueryable();


            var query = await (from hdr in _dbContext.WfhHeader.AsQueryable()
                               join emp in _dbContext.Employee.AsNoTracking() on hdr.EmployeeId equals emp.EmployeeId
                               join stat in _dbContext.Status on hdr.Status equals stat.StatusId
                               //orderby hdr.DateCreated descending, hdr.NAME descending
                               select new WFHHeaderViewModel
                               {                                  
                                       WfhHeaderId = hdr.WfhHeaderId,
                                       RequestNo = hdr.RequestNo,
                                       Fullname = emp.Lastname + " " + emp.Firstname,
                                       StatusId = hdr.Status,
                                       Remarks = hdr.Remarks,   
                                       DateCreated = hdr.DateCreated,
                                       DateCreatedString = hdr.DateCreated.ToString("yyyy-MM-dd HH:mm"),
                                   StatusName = stat.StatusName,                   
                       
                               }).ToListAsync();
            return query;
        }

    
        public async Task<WfhApplicationViewModel> GetWFHApplicationDetailByWfhHeaderId(WFHHeaderViewModel model)
        {
            WfhApplicationViewModel wfh = new WfhApplicationViewModel();
            var dtlsquery = await (from hdr in _dbContext.WfhDetail.AsNoTracking()
                               join att in _dbContext.vw_AttendanceSummary_WFH.AsNoTracking() on hdr.AttendanceId equals att.ID
                               where hdr.WfhHeaderId == model.WfhHeaderId
                               select new WfhDetailViewModel
                               {
                                   WfhHeaderId = hdr.WfhHeaderId,
                                   Date = att.DATE.ToString(),
                                   TimeIn = att.FIRST_IN,
                                   TimeOut = att.LAST_OUT,
                                   TotalWorkingHours = att.TOTAL_WORKING_HOURS
                               }).ToListAsync();

            var hdrquery = await (from hdr in _dbContext.WfhHeader.AsNoTracking()
                                  join emp in _dbContext.Employee.AsNoTracking() on hdr.EmployeeId equals emp.EmployeeId
                                  join stat in _dbContext.Status.AsNoTracking() on hdr.Status equals stat.StatusId
                                  join apprvl in _dbContext.ApprovalHistory.AsNoTracking().Where(x => x.ModulePageId == (int)EnumModulePage.WFH) 
                                  on hdr.WfhHeaderId equals apprvl.TransactionId into ah
                                  from apprvl in ah.DefaultIfEmpty()
                                  where hdr.WfhHeaderId == model.WfhHeaderId
                                   select new WFHHeaderViewModel
                                   {
                                       WfhHeaderId = hdr.WfhHeaderId,
                                       RequestNo = hdr.RequestNo,
                                       Remarks = hdr.Remarks,
                                       StatusId = hdr.Status,
                                       StatusName = stat.StatusName ,
                                       DateCreatedString = hdr.DateCreated.ToString("yyyy-MM-dd HH:mm"),
                                       DateModifiedString = hdr.DateModified.HasValue ? hdr.DateModified.Value.ToString("yyyy-MM-dd HH:mm") : string.Empty,
                                       Approver = emp.Firstname + " " + emp.Lastname,
                                       DateApprovedDisapproved = apprvl.DateCreated.ToString("yyyy-MM-dd HH:mm"),
                                       ApprovalRemarks = apprvl != null ? apprvl.Remarks : string.Empty,
                                   }).FirstOrDefaultAsync();


            wfh.Details = dtlsquery;
            wfh.Header = hdrquery;

            return wfh;
        }

        public async Task<IList<WFHViewModel>> GetWFHLogsByEmployeeId(WFHViewModel model)
        {
            var _emp = await _dbContext.Employee.AsQueryable().Where(x => x.EmployeeId == model.EMPLOYEE_ID).FirstOrDefaultAsync();
            var empNO = _emp.EmployeeNo ?? string.Empty;

            var query = await (from logs in _dbContext.tbl_wfh_logs.AsQueryable()                         
                               where logs.EMPLOYEE_ID == empNO && logs.DATE_TIME.Date == model.DATE_TIME.Date
                               select new WFHViewModel
                               {
                                   DATE = logs.DATE_TIME.ToString("MM/dd/yyyy"),
                                   EMPLOYEE_NO = logs.EMPLOYEE_ID ,
                                   TIME_IN = logs.DATE_TIME.ToString("HH:mm:ss")
                               }).ToListAsync();
            return query;
        }


        public async Task<(int statuscode, string message)> SaveWFHApplication(WfhApplicationViewModel model)
        {
            WfhHeader entity = new WfhHeader();
            entity.WfhHeaderId = model.Header.WfhHeaderId;
            entity.RequestNo = await GenereteRequestNo();
            entity.EmployeeId = model.Header.EmployeeId;
            entity.Status = (int)EnumStatus.ForApproval;
            entity.ApproverId = model.Header.ApproverId;
            entity.Remarks = model.Header.Remarks;
            entity.DateCreated = DateTime.Now;
            entity.CreatedBy = model.Header.CurrentUserId;
            entity.DateModified = null;
            entity.ModifiedBy = null;
            entity.IsActive = true;
            await _dbContext.WfhHeader.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            foreach(var x in model.Details)
            {
                WfhDetail dtl = new WfhDetail();
                dtl.WfhHeaderId = entity.WfhHeaderId;
                dtl.AttendanceId = x.Id;
                dtl.IsActive = true;
                await _dbContext.WfhDetail.AddAsync(dtl);
                await _dbContext.SaveChangesAsync();
            }

            //Send Email Notification to Approver
            WFHHeaderViewModel email = new WFHHeaderViewModel();
            email.ApproverId = model.Header.ApproverId;
            email.StatusId = entity.Status;
            email.RequestNo = entity.RequestNo;
            await _emailRepository.SentToApprovalWFH(email);

            //Send Application Notification to Approver
            NotificationViewModel notifvmToApprover = new NotificationViewModel();
            notifvmToApprover.Title = "WFH";
            notifvmToApprover.Description = System.String.Format("You have been assigned WFH request {0} for approval.", entity.RequestNo);
            notifvmToApprover.ModuleId = (int)EnumModulePage.WFH;
            notifvmToApprover.TransactionId = entity.WfhHeaderId;
            notifvmToApprover.AssignId = model.Header.ApproverId;
            notifvmToApprover.URL = "/Todo/Index/";
            notifvmToApprover.MarkRead = false;
            notifvmToApprover.CreatedBy = entity.CreatedBy;
            notifvmToApprover.IsActive = true;
            await _homeRepository.SaveNotification(notifvmToApprover);

            //Send Application Notification to Requestor
            NotificationViewModel notifvmToRequestor = new NotificationViewModel();
            notifvmToRequestor.Title = "WFH";
            notifvmToRequestor.Description = System.String.Format("Your WFH request {0} has been submitted for approval.", entity.RequestNo);
            notifvmToRequestor.ModuleId = (int)EnumModulePage.WFH;
            notifvmToRequestor.TransactionId = entity.WfhHeaderId;
            notifvmToRequestor.AssignId = entity.CreatedBy;          
            notifvmToRequestor.URL = "/DailyTimeRecord/WFH";
            notifvmToRequestor.MarkRead = false;
            notifvmToRequestor.CreatedBy = entity.CreatedBy;
            notifvmToRequestor.IsActive = true;
            await _homeRepository.SaveNotification(notifvmToRequestor);

            return (StatusCodes.Status200OK, string.Format("WFH application request {0} has been submitted for approval.", entity.RequestNo));          
        }

        public async Task<(int statuscode, string message)> CancelWFHApplication(WFHHeaderViewModel model)
        {
            try
            {
                var entity = await _dbContext.WfhHeader.FirstOrDefaultAsync(x => x.WfhHeaderId == model.WfhHeaderId && x.IsActive == true);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid WFH Id");
                }
                entity.Status = (int)EnumStatus.Cancelled;
                entity.ModifiedBy = model.CurrentUserId;
                entity.DateModified = DateTime.Now;
                entity.IsActive = true;
                _dbContext.WfhHeader.Entry(entity).State = EntityState.Modified;
                await _dbContext.SaveChangesAsync();

                //Send Application Notification to Approver
                NotificationViewModel notifvmToApprover = new NotificationViewModel();
                notifvmToApprover.Title = "WFH";
                notifvmToApprover.Description = System.String.Format("WFH request {0} has been cancelled by the requestor.", entity.RequestNo);
                notifvmToApprover.ModuleId = (int)EnumModulePage.WFH;
                notifvmToApprover.TransactionId = entity.WfhHeaderId;
                notifvmToApprover.AssignId = entity.ApproverId;
                notifvmToApprover.URL = "/Todo/Index/";
                notifvmToApprover.MarkRead = false;
                notifvmToApprover.CreatedBy = entity.CreatedBy;
                notifvmToApprover.IsActive = true;
                await _homeRepository.SaveNotification(notifvmToApprover);

                return (StatusCodes.Status200OK, System.String.Format("WFH request {0} has been cancelled.", entity.RequestNo));
                
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

                //var startDate = new DateTime(_currentYear, _currentMonth, 1);
                //var endDate = startDate.AddMonths(1);                             

                var _dtr = await _dbContext.WfhHeader.AsNoTracking()
                    .Where(x => x.IsActive == true && x.DateCreated.Date.Year == _currentYear)
                    .AsNoTracking()
                    .ToListAsync();


                int totalrecords = _dtr.Count() + 1;
                string finalSetRecords = GetFormattedRecord(totalrecords);
                string yearMonth = DateTime.Now.ToString("yyyyMM");
                string req = Constants.ModuleCode_WFH;

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
