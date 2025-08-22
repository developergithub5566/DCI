using DCI.Core.Common;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DCI.Repositories
{
    public class WfhRepository : IWfhRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        public WfhRepository(DCIdbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<IList<DailyTimeRecordViewModel>> GetAllWFH(DailyTimeRecordViewModel model)
        {
            var context = _dbContext.vw_AttendanceSummary_WFH.AsQueryable();


            var query = await (from dtr in context
                               join stat in _dbContext.Status on dtr.STATUS equals stat.StatusId
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
                                   STATUS = dtr.STATUS,
                                   STATUSNAME = stat.StatusName
                               }).ToListAsync();

            if ((int)EnumEmployeeScope.PerEmployee == model.ScopeTypeEmp)
            {
                //var usr = _dbContext.User.Where(x => x.UserId == model.CurrentUserId).FirstOrDefault();
                var emp = _dbContext.Employee.Where(x => x.EmployeeId == model.EMPLOYEE_ID).FirstOrDefault();
                if (emp != null)
                    query = query.Where(x => x.EMPLOYEE_NO == emp.EmployeeNo).ToList();
            }

            return query;
        }

        public async Task<(int statuscode, string message)> SaveWFHTimeIn(WFHViewModel model)
        {

            var empdtl = _dbContext.Employee.Where(x => x.EmployeeId == model.EMPLOYEE_ID).FirstOrDefault();

            model.EMPLOYEE_NO = empdtl.EmployeeNo ?? string.Empty;
            model.FULL_NAME = empdtl.Firstname + " " + empdtl.Lastname;
            model.CREATED_BY = "SYSAD";

            tbl_wfh_logs entity = new tbl_wfh_logs();
            entity.EMPLOYEE_ID = model.EMPLOYEE_NO;
            entity.FULL_NAME = model.FULL_NAME;
            entity.DATE_TIME = DateTime.Now;
            entity.CREATED_DATE = DateTime.Now;
            entity.CREATED_BY = model.CREATED_BY;
            await _dbContext.tbl_wfh_logs.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return (StatusCodes.Status200OK, "Successfully saved");
        }

        public async Task<(int statuscode, string message)> SaveWFHApplication(WfhApplicationViewModel model)
        {
            WfhHeader entity = new WfhHeader();
            entity.WfhHeaderId = model.Header.WfhHeaderId;
            entity.RequestNo = model.Header.RequestNo;
            entity.EmployeeId = model.Header.EmployeeId;
            entity.Status = (int)EnumStatus.ForApproval;
            entity.ApproverId = 1;
            entity.Remarks = model.Header.Remarks;
            entity.DateCreated = DateTime.Now;
            entity.CreatedBy = model.Header.CurrentUserId;
            entity.DateModified = null;
            entity.ModifiedBy = null;
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

            return (StatusCodes.Status200OK, "Successfully saved");
        }


    }
}
