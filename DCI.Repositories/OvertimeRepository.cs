using DCI.Data;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DCI.Repositories
{
    public class OvertimeRepository : IOvertimeRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        public OvertimeRepository(DCIdbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<DailyTimeRecordViewModel> GetAllAttendanceByDate(DateTime date, string empno)
        {
                 var context = _dbContext.vw_AttendanceSummary.Where(x => x.DATE == date);

                var query = (from dtr in context
                             where dtr.DATE == date && dtr.EMPLOYEE_NO == empno
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
                                 DATESTRING = dtr.DATE.ToString("MM/dd/yyyy")
                             }).FirstOrDefault();
            return query;                  
        }
    }
}
