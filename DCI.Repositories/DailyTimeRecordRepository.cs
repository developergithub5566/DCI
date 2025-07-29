using DCI.Core.Common;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Reflection.PortableExecutable;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        public async Task<IList<DailyTimeRecordViewModel>> GetAllDTR(DailyTimeRecordViewModel model)
        {
            var context = _dbContext.vw_AttendanceSummary.AsQueryable();


            var query = await (from dtr in context

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
                             TOTAL_WORKING_HOURS = dtr.TOTAL_WORKING_HOURS
                         }).ToListAsync();

            if((int)EnumTypeData.EMP == model.TypeId)
            {
                //var emp = _dbContext.Employee.Where(x => x.EmployeeId == model.CurrentUserId).FirstOrDefault();
                //if(emp != null) 
                //query = query.Where(x => x.EMPLOYEE_NO == emp.EmployeeNo).ToList();
                var usr = _dbContext.User.Where(x => x.UserId == model.CurrentUserId).FirstOrDefault();
                var emp = _dbContext.Employee.Where(x => x.EmployeeId == usr.EmployeeId).FirstOrDefault();
                if(emp != null) 
                query = query.Where(x => x.EMPLOYEE_NO == emp.EmployeeNo).ToList();
            }

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
                             LATE = dtr.LATE,
                             CLOCK_OUT = dtr.CLOCK_OUT,
                             UNDER_TIME = dtr.UNDER_TIME,
                             OVERTIME = dtr.OVERTIME,
                             TOTAL_HOURS = dtr.TOTAL_HOURS,
                             TOTAL_WORKING_HOURS = dtr.TOTAL_WORKING_HOURS
                         }).ToList();


            return query;
        }
        public async Task<IList<DTRCorrectionViewModel>> GetAllDTRCorrection(DTRCorrectionViewModel model)
        {    
            var query = (from dtr in _dbContext.DTRCorrection.AsQueryable()
                         join stat in _dbContext.Status on dtr.Status equals stat.StatusId
                         where dtr.CreatedBy == model.CreatedBy
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
                             Filename = dtr.Filename,
                             FileLocation = dtr.FileLocation,
                             CreatedBy = dtr.CreatedBy,
                             IsActive = dtr.IsActive
                         }).ToList();


            if ((int)EnumTypeData.EMP == model.TypeId)
            {           
                    query = query.Where(x => x.CreatedBy == model.CreatedBy).ToList();
            }
            return query;
        }

        public async Task<DTRCorrectionViewModel> DTRCorrectionByDtrId(int dtrId)
        {
            var query = (from dtr in _dbContext.DTRCorrection.AsQueryable()
                         join stat in _dbContext.Status on dtr.Status equals stat.StatusId
                         join emp in _dbContext.EmployeeWorkDetails on dtr.CreatedBy equals emp.EmployeeId
                         join dept in _dbContext.Department on emp.DepartmentId equals dept.DepartmentId
                         join depthead in _dbContext.Employee on dept.ApproverId equals depthead.EmployeeId
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
                             Filename = dtr.Filename,
                             FileLocation = dtr.FileLocation,
                             CreatedBy = dtr.CreatedBy,
                             IsActive = dtr.IsActive,
                             DepartmentHead = depthead.Firstname + " " + depthead.Lastname + " " + depthead.Suffix
                         }).FirstOrDefault();


            return query;
        }


        public async Task<(int statuscode, string message)> SaveDTRCorrection(DTRCorrectionViewModel param)
        {
            DTRCorrectionViewModel model = new DTRCorrectionViewModel();

            try
            {
                if (model.DtrId == 0)
                {
                    DTRCorrection entity = new DTRCorrection();
                    entity.DateFiled = DateTime.Now;
                    entity.RequestNo = await GenereteRequestNo();
                    entity.DtrType = param.DtrType; 
                    entity.DateFiled = DateTime.Now;
                    entity.DtrDateTime = param.DtrDateTime;
                    entity.Status = (int)EnumStatus.ForApproval;
                    entity.Reason = param.Reason;
                    entity.Filename = param.Filename;
                    entity.FileLocation = param.FileLocation;
                    entity.CreatedBy = param.CreatedBy;
                    entity.IsActive = true;
                    await _dbContext.DTRCorrection.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();
               
                    //var workdtls = _dbContext.EmployeeWorkDetails.Where(x => x.EmployeeId == param.EmployeeId).FirstOrDefault();
                    //var dept = _dbContext.Department.Where(x => x.DepartmentId == workdtls.DepartmentId).FirstOrDefault();
                    //model.ApproverId = dept.ApproverId;
                    //model.LeaveRequestHeader.Status = entity.Status;
                    //model.LeaveRequestHeader.RequestNo = entity.RequestNo;
                    //await _emailRepository.SendToApproval(model);

                    //NotificationViewModel notifvm = new NotificationViewModel();
                    //notifvm.Title = "Leave";
                    //notifvm.Description = String.Format("You have been assigned leave request {0} for review", entity.RequestNo);
                    //notifvm.ModuleId = (int)EnumModulePage.Leave;
                    //notifvm.TransactionId = entity.LeaveRequestHeaderId;
                    //notifvm.AssignId = dept.ApproverId ?? 0;
                    //notifvm.URL = "/Todo/Index/?leaveId=" + model.LeaveRequestHeaderId;
                    //notifvm.MarkRead = false;
                    //notifvm.CreatedBy = param.EmployeeId;
                    //notifvm.IsActive = true;
                    //await _homeRepository.SaveNotification(notifvm);


                    return (StatusCodes.Status200OK, "Successfully saved");
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


        public async Task<IList<DailyTimeRecordViewModel>> GetAllUndertime(DailyTimeRecordViewModel model)
        {
            var context = _dbContext.vw_AttendanceSummary.AsQueryable();


            var dateTo = model.DateTo.Date.AddDays(1).AddTicks(-1);


            //var query = (from dtr in context
            //             where dtr.DATE >= dateFrom && dtr.DATE <= dateTo
            //             group dtr by new { dtr.EMPLOYEE_NO, dtr.NAME } into g
            //             select new DailyTimeRecordViewModel
            //             {
            //                 EMPLOYEE_NO = g.Key.EMPLOYEE_NO,
            //                 NAME = g.Key.NAME,
            //                 TOTAL_UNDERTIME = g.Sum(x =>
            //                     TimeSpan.TryParse(x.UNDER_TIME, out var t) ? (decimal?)t.TotalMinutes : 0m)
            //             }).ToList();

            //var rawData = context
            //                .Where(dtr => dtr.DATE >= dateFrom && dtr.DATE <= dateTo)
            //                .Select(dtr => new 
            //                {
            //                    dtr.EMPLOYEE_NO,
            //                    dtr.NAME,
            //                    dtr.UNDER_TIME
            //                })
            //                .ToList(); // ← materialize data to memory

            //var query = rawData
            //    .GroupBy(x => new { x.EMPLOYEE_NO, x.NAME })
            //    .Select(g =>  
            //    {
            //        var totalMinutes = g.Sum(x =>
            //            TimeSpan.TryParse(x.UNDER_TIME, out var t) ? t.TotalMinutes : 0);

            //        return new DailyTimeRecordViewModel
            //        {
            //            EMPLOYEE_NO = g.Key.EMPLOYEE_NO,
            //            NAME = g.Key.NAME,
            //            TOTAL_UNDERTIME = (decimal)totalMinutes
            //        };
            //    })
            //    .ToList();

            var rawData = context
                        .Where(dtr => dtr.DATE >= model.DateFrom.Date && dtr.DATE <= dateTo)
                        .Select(dtr => new
                        {
                            dtr.EMPLOYEE_NO,                        
                            dtr.NAME,
                            dtr.UNDER_TIME
                        })
                        .ToList(); 

            var query = rawData
                .GroupBy(x => new { x.EMPLOYEE_NO, x.NAME })
                .Select(g =>
                {
                    var totalUnderTime = g
                        .Select(x => TimeSpan.TryParse(x.UNDER_TIME, out var t) ? t : TimeSpan.Zero)
                        .Aggregate(TimeSpan.Zero, (sum, next) => sum + next);

                    var totalMinutes = g.Sum(x =>
                         TimeSpan.TryParse(x.UNDER_TIME, out var t) ? t.TotalMinutes : 0);

                    return new DailyTimeRecordViewModel
                    {
                        EMPLOYEE_NO = g.Key.EMPLOYEE_NO,
                        NAME = g.Key.NAME,          
                        DateFrom = model.DateFrom,
                        DateTo = model.DateTo,
                        TOTAL_UNDERTIME =  string.Format("{0:0.0000}",totalMinutes) + " or " + totalUnderTime.ToString(@"hh\:mm\:ss") 
                    };
                })
                .ToList();

            return query;
        }

        public async Task<IList<DailyTimeRecordViewModel>> GetUndertimeById(DailyTimeRecordViewModel model)
        {
            var context = _dbContext.vw_AttendanceSummary.AsQueryable();


            DateTime dateFrom = new DateTime(2024, 6, 1);
           

            var dateTo = model.DateTo.Date.AddDays(1).AddTicks(-1);

            var query = (from dtr in context
                         //join emp in _dbContext.Employee on dtr.EMPLOYEE_NO equals emp.EmployeeNo
                         where dtr.DATE >= dateFrom && dtr.DATE <= dateTo && dtr.EMPLOYEE_NO == model.EMPLOYEE_NO // && emp.EmployeeId == model.ID
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
                         }).ToList();


            return query;
        }

        public async Task<IList<WFHViewModel>> GetAllWFHById(WFHViewModel model)
        {
           // var context = _dbContext.tbl_wfh_logs.AsQueryable();

            var query = (from dtr in _dbContext.tbl_wfh_logs

                         select new WFHViewModel
                         {
                             ID = dtr.ID,
                             EMPLOYEE_ID = dtr.EMPLOYEE_ID,
                             FULL_NAME = dtr.FULL_NAME,
                             DATE_TIME = dtr.DATE_TIME                        
                           
                         }).ToList();


            return query;
        }

        public async Task<(int statuscode, string message)> SaveWFHTimeIn(WFHViewModel model)
        {

            model.EMPLOYEE_ID = "080343";
            model.FULL_NAME = "John A Cabugao";
            model.CREATED_BY = "SYSAD";

            tbl_wfh_logs entity = new tbl_wfh_logs();
            entity.EMPLOYEE_ID = model.EMPLOYEE_ID;
            entity.FULL_NAME = model.FULL_NAME;
            entity.DATE_TIME = DateTime.Now;
            entity.CREATED_DATE = DateTime.Now;
            entity.CREATED_BY = model.CREATED_BY;            
            await _dbContext.tbl_wfh_logs.AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return (StatusCodes.Status200OK, "Successfully saved");
        }
    }
}
