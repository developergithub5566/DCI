using DCI.Data;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;

namespace DCI.Repositories
{
    public class DailyTimeRecordRepository : IDailyTimeRecordRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        public DailyTimeRecordRepository(DCIdbContext dbContext) 
        {
            _dbContext = dbContext;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<IList<DailyTimeRecordViewModel>> GetAllDTR()
        {
            var context = _dbContext.vw_AttendanceSummary.AsQueryable();


            var query = (from dtr in context
                             // where dept.IsActive == true && dept.DepartmentId == deptId
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
                         LATE= dtr.LATE,
                         CLOCK_OUT = dtr.CLOCK_OUT,
                         UNDER_TIME = dtr.UNDER_TIME,
                         OVERTIME = dtr.OVERTIME,
                         TOTAL_HOURS = dtr.TOTAL_HOURS,
                         TOTAL_WORKING_HOURS = dtr.TOTAL_WORKING_HOURS
                        }).ToList();
            

            return query;
        }
    }
}
