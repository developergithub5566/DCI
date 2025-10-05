using DCI.Core.Common;
using DCI.Core.Helpers;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
    public class LateRepository : ILateRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        private IEmailRepository _emailRepository;
        private readonly IHomeRepository _homeRepository;    

        public LateRepository(DCIdbContext dbContext, IEmailRepository emailRepository, IHomeRepository homeRepository)
        {
            _dbContext = dbContext;
            _emailRepository = emailRepository;
            _homeRepository = homeRepository;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<IList<DailyTimeRecordViewModel>> GetAllLate(DailyTimeRecordViewModel model)
        {
            // var context = _dbContext.vw_AttendanceSummary.AsNoTracking().Where(x => x.STATUS != (int)EnumStatus.PayrollDeducted && x.STATUS != (int)EnumStatus.VLDeducted);
            var context = _dbContext.vw_AttendanceSummary.AsNoTracking().Where(x => x.STATUS == (int)EnumStatus.Raw);
            int _currentYear = DateTime.Now.Year;

            var dateTo = model.DateTo.Date.AddDays(1).AddTicks(-1);

           
            var leaveInfo = (from li in _dbContext.LeaveInfo
                             join emp in _dbContext.Employee on li.EmployeeId equals emp.EmployeeId
                             where li.IsActive && li.DateCreated.Year == _currentYear
                             group new { li, emp } by li.EmployeeId into g
                             select new
                             {
                                 Leave = g.OrderByDescending(x => x.li.DateCreated).FirstOrDefault().li,
                                 EmpNo = g.OrderByDescending(x => x.li.DateCreated).FirstOrDefault().emp.EmployeeNo
                             }).ToList();


            var rawData = context
                        .Where(dtr => dtr.DATE >= model.DateFrom.Date && dtr.DATE <= dateTo)
                        .Select(dtr => new
                        {
                            dtr.EMPLOYEE_NO,
                            dtr.NAME,
                            dtr.LATE
                        })
                        .ToList();

            var joined = (from dtr in rawData
                          join li in leaveInfo
                              on dtr.EMPLOYEE_NO equals li.EmpNo into gj
                          from li in gj.DefaultIfEmpty()   // LEFT JOIN
                          select new
                          {
                              dtr.EMPLOYEE_NO,
                              dtr.NAME,
                              dtr.LATE,
                              LeaveBalance = li?.Leave.VLBalance ?? 0,
                          }).ToList();

            var query = joined
                .GroupBy(x => new { x.EMPLOYEE_NO, x.NAME, x.LeaveBalance })
                .Select(g =>
                {
                    var totalUnderTime = g
                        .Select(x => TimeSpan.TryParse(x.LATE, out var t) ? t : TimeSpan.Zero)
                        .Aggregate(TimeSpan.Zero, (sum, next) => sum + next);

                    var totalMinutes = g.Sum(x =>
                         TimeSpan.TryParse(x.LATE, out var t) ? t.TotalMinutes : 0);

                    return new DailyTimeRecordViewModel
                    {
                        EMPLOYEE_NO = g.Key.EMPLOYEE_NO,
                        NAME = g.Key.NAME,
                        DateFrom = model.DateFrom,
                        DateTo = model.DateTo,          
                        TOTAL_LATE = TimeHelper.ConvertMinutesToValue(totalMinutes).ToString(),          
                        TOTAL_LATEHOURS = totalUnderTime.ToString(@"hh") + " Hour/s" + " and " + totalUnderTime.ToString(@"mm") + " Minute/s",
                        VLBalance = g.Key.LeaveBalance
                    };
                })     
                .ToList();

            return query.Where(x => x.TOTAL_LATE != "0").ToList(); 
        }

        public async Task<IList<DailyTimeRecordViewModel>> GetLateById(DailyTimeRecordViewModel model)
        {
            try
            {
                var context = _dbContext.vw_AttendanceSummary.AsQueryable();

                var empdbcontext = _dbContext.Employee.Where(x => x.EmployeeNo == model.EMPLOYEE_NO).FirstOrDefault();
                var _empId = empdbcontext?.EmployeeId ?? 0;
                var dateTo = model.DateTo.Date.AddDays(1).AddTicks(-1);
               

                var dtrCorrectRaw = await (
                       from b in _dbContext.DTRCorrection
                       join s in _dbContext.Status on b.Status equals s.StatusId into sj
                       from s in sj.DefaultIfEmpty()
                       where b.IsActive
                       select new
                       {
                           b.EmployeeId,
                           Date = b.DtrDateTime.Date,
                           b.RequestNo,
                           StatusName = s.StatusName
                       }
                   ).ToListAsync();

                var dtrCorrectMap = dtrCorrectRaw
                    .GroupBy(x => new { x.EmployeeId, x.Date })
                    .ToDictionary(
                        g => g.Key,
                        g => new
                        {
                            RequestNos = string.Join(", ", g.Select(v => v.RequestNo).Where(s => s != null).Distinct()),
                            StatusNames = string.Join(", ", g.Select(v => v.StatusName).Where(s => s != null).Distinct())
                        }
                    );

                var leavesRaw = await (
                    from b in _dbContext.LeaveRequestHeader
                    join c in _dbContext.LeaveRequestDetails
                         on b.LeaveRequestHeaderId equals c.LeaveRequestHeaderId
                    join s in _dbContext.Status on b.Status equals s.StatusId into sj
                    from s in sj.DefaultIfEmpty()
                    where b.IsActive && c.LeaveDate.Date >= model.DateFrom.Date && c.LeaveDate.Date < dateTo && b.EmployeeId == _empId
                                        && (b.LeaveTypeId == (int)EnumLeaveType.HD
                                        || b.LeaveTypeId == (int)EnumLeaveType.OB
                                        || b.LeaveTypeId == (int)EnumLeaveType.SL)
                    select new
                    {
                        b.EmployeeId,
                        Date = c.LeaveDate.Date,
                        b.RequestNo,
                        StatusName = s.StatusName
                    }
                ).ToListAsync();

                var leaveMap = leavesRaw
                    .GroupBy(x => new { x.EmployeeId, x.Date })
                    .ToDictionary(
                        g => g.Key,
                        g => new
                        {
                            RequestNos = string.Join(", ", g.Select(v => v.RequestNo).Where(s => s != null).Distinct()),
                            StatusNames = string.Join(", ", g.Select(v => v.StatusName).Where(s => s != null).Distinct())
                        }
                    );


                var att = await (
                    from a in _dbContext.vw_AttendanceSummary
                    join e in _dbContext.Employee on a.EMPLOYEE_NO equals e.EmployeeNo
                    where a.EMPLOYEE_NO == model.EMPLOYEE_NO
                       && a.DATE.Date >= model.DateFrom.Date && a.DATE.Date < dateTo
                    orderby a.DATE
                    select new
                    {
                        Row = a,
                        EmployeeId = e.EmployeeId
                    }
                ).ToListAsync();     

                var result = att.Select(x =>
                {
                    var d = x.Row.DATE.Date;
                    var key = new { x.EmployeeId, Date = d };

                    // lookup leave
                    var leaveAgg = leaveMap.TryGetValue(key, out var lf) ? lf : null;

                    // lookup dtr correction
                    var corrAgg = dtrCorrectMap.TryGetValue(key, out var cf) ? cf : null;

                    // merge: if both exist, concatenate
                    var reqNos = new List<string>();
                    var statNms = new List<string>();

                    if (leaveAgg != null)
                    {
                        if (!string.IsNullOrEmpty(leaveAgg.RequestNos))
                            reqNos.Add(leaveAgg.RequestNos);
                        if (!string.IsNullOrEmpty(leaveAgg.StatusNames))
                            statNms.Add(leaveAgg.StatusNames);
                    }

                    if (corrAgg != null)
                    {
                        if (!string.IsNullOrEmpty(corrAgg.RequestNos))
                            reqNos.Add(corrAgg.RequestNos);
                        if (!string.IsNullOrEmpty(corrAgg.StatusNames))
                            statNms.Add(corrAgg.StatusNames);
                    }

                    return new DailyTimeRecordViewModel
                    {
                        ID = x.Row.ID,
                        EMPLOYEE_NO = x.Row.EMPLOYEE_NO,
                        NAME = x.Row.NAME,
                        DATE = x.Row.DATE,
                        FIRST_IN = x.Row.FIRST_IN,
                        LAST_OUT = x.Row.LAST_OUT,
                        LATE = x.Row.LATE,
                        CLOCK_OUT = x.Row.CLOCK_OUT,
                        UNDER_TIME = x.Row.UNDER_TIME,
                        OVERTIME = x.Row.OVERTIME,
                        TOTAL_HOURS = x.Row.TOTAL_HOURS,
                        TOTAL_WORKING_HOURS = x.Row.TOTAL_WORKING_HOURS,
                        DATESTRING = x.Row.DATE.ToString("MM/dd/yyyy"),
                        ModulePageId = (int)EnumModulePage.DailyTimeRecord,

                        RequestNo = string.Join(" | ", reqNos.Distinct()),
                        STATUSNAME = string.Join(" | ", statNms.Distinct())
                    };
                }).ToList();

                var query = result;

                return query;
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
            return null;
        }


        public async Task<(int statuscode, string message)> SaveLate(LateHeaderDeductionViewModel model)
        {
            try
            {
                foreach (var ut in model.LateDeductionList)
                {
                    if (ut.EmpNo != null && ut.TotalUndertime > 0)
                    {
                        var dateTo = ut.DateTo.Date.AddDays(1).AddTicks(-1);
                        var emp = _dbContext.Employee.Where(x => x.EmployeeNo == ut.EmpNo).FirstOrDefault();
                        var workdtls = _dbContext.EmployeeWorkDetails.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();
                        //var leafinfo = _dbContext.LeaveInfo.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();
                        var leafinfo = _dbContext.LeaveInfo.Where(li => li.IsActive && li.EmployeeId == emp.EmployeeId && li.DateCreated.Year == DateTime.Now.Year).OrderByDescending(li => li.DateCreated).FirstOrDefault();



                        //create audit logs for undertime execution 
                        LateHeader oth = new LateHeader();
                        oth.RequestNo = await GenereteRequestNoForLateDeduction();
                        oth.DateFrom = ut.DateFrom;
                        oth.DateTo = ut.DateTo;
                        oth.DateCreated = DateTime.Now;
                        oth.CreatedBy = model.CurrentUserId;
                        oth.IsActive = true;
                        await _dbContext.LateHeader.AddAsync(oth);
                        await _dbContext.SaveChangesAsync();

                        LateDetail otd = new LateDetail();

                        //Insert Leave application : leavetype Undertime , Status automatic approved and Insert Notification 
                        LeaveFormViewModel lvFormmodel = new LeaveFormViewModel();
                        lvFormmodel.EmployeeId = emp.EmployeeId;
                        lvFormmodel.NoOfDays = ut.TotalUndertime ?? 0;
                        await SaveLeaveForLate(lvFormmodel);


                        //var attendanceList = await _dbContext.vw_AttendanceSummary.Where(x => x.EMPLOYEE_NO == ut.EmpNo && x.DATE >= ut.DateFrom.Date && x.DATE <= dateTo.Date
                        //&& (x.STATUS != (int)EnumStatus.NotDeducted || x.STATUS != (int)EnumStatus.PayrollDeducted || x.STATUS != (int)EnumStatus.VLDeducted)).ToListAsync();

                        var attendanceList = await _dbContext.vw_AttendanceSummary.Where(x => x.EMPLOYEE_NO == ut.EmpNo && x.DATE >= ut.DateFrom.Date && x.DATE <= dateTo.Date
                                                                                                                                                      && x.STATUS == 0).ToListAsync();

                        // kung may remaining leave pa
                        if (leafinfo?.VLBalance >= ut.TotalUndertime && workdtls.EmployeeStatusId == (int)EnumEmploymentType.Regular)
                        {
                            leafinfo.VLBalance = leafinfo.VLBalance - ut.TotalUndertime ?? 0;
                            // leafinfo.DateModified = DateTime.Now;
                            // leafinfo.ModifiedBy = model.ModifiedBy;
                            leafinfo.IsActive = true;
                            _dbContext.LeaveInfo.Entry(leafinfo).State = EntityState.Modified;
                            await _dbContext.SaveChangesAsync();


                            //Update DTR attendance summary status to DEDUCTED                    
                            await _dbContext.tbl_raw_logs.Where(x => x.EMPLOYEE_ID == ut.EmpNo && x.DATE_TIME >= ut.DateFrom && x.DATE_TIME <= dateTo)
                                                         .ExecuteUpdateAsync(s => s
                                                         .SetProperty(r => r.STATUS, r => (int)EnumStatus.VLDeducted));

                            foreach (var attdnc in attendanceList)
                            {
                                otd.LateHeaderId = oth.LateHeaderId;
                                otd.AttendanceId = (int)attdnc.ID;
                                otd.DeductionType = (int)EnumDeductionType.VacationLeave;
                                otd.IsActive = true;
                                await _dbContext.LateDetail.AddAsync(otd);
                                await _dbContext.SaveChangesAsync();
                            }
                        } // kapag wala ng leave
                        else
                        {
                            //Update DTR attendance summary status to Payroll DEDUCTED                    
                            await _dbContext.tbl_raw_logs.Where(x => x.EMPLOYEE_ID == ut.EmpNo && x.DATE_TIME >= ut.DateFrom && x.DATE_TIME <= dateTo)
                                                         .ExecuteUpdateAsync(s => s
                                                         .SetProperty(r => r.STATUS, r => (int)EnumStatus.PayrollDeducted));

                            foreach (var attdnc in attendanceList)
                            {
                                otd.LateHeaderId = oth.LateHeaderId;
                                otd.AttendanceId = (int)attdnc.ID;
                                otd.DeductionType = (int)EnumDeductionType.Payroll;
                                otd.IsActive = true;
                                await _dbContext.LateDetail.AddAsync(otd);
                                await _dbContext.SaveChangesAsync();
                            }
                        }
                    }
                }

                return (StatusCodes.Status200OK, Constants.Undertime_Deduction);
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

        public async Task SaveLeaveForLate(LeaveFormViewModel param)
        {
            LeaveViewModel model = new LeaveViewModel();

            try
            {

                LeaveRequestHeader entity = new LeaveRequestHeader();
                entity.EmployeeId = param.EmployeeId;
                entity.RequestNo = await GenereteRequestNoForUndertime();
                entity.DateFiled = DateTime.Now;
                entity.LeaveTypeId = (int)EnumLeaveType.UT;
                entity.Status = (int)EnumStatus.VLDeducted;
                entity.Reason = "System-Generated Late Deduction.";
                entity.NoOfDays = param.NoOfDays;
                entity.ModifiedBy = null;
                entity.DateModified = null;
                entity.IsActive = true;
                await _dbContext.LeaveRequestHeader.AddAsync(entity);
                await _dbContext.SaveChangesAsync();


                LeaveRequestDetails entityDtl = new LeaveRequestDetails();
                entityDtl.LeaveRequestHeaderId = entity.LeaveRequestHeaderId;
                entityDtl.LeaveDate = DateTime.Now;
                entityDtl.Amount = param.NoOfDays;
                entityDtl.IsActive = true;
                await _dbContext.LeaveRequestDetails.AddAsync(entityDtl);
                await _dbContext.SaveChangesAsync();



                NotificationViewModel notifvm = new NotificationViewModel();
                notifvm.Title = "Late";
                notifvm.Description = System.String.Format("System-Generated Late Deduction.", entity.RequestNo);
                notifvm.ModuleId = (int)EnumModulePage.Undertime;
                notifvm.TransactionId = entity.LeaveRequestHeaderId;
                notifvm.AssignId = param.ApproverId;
                notifvm.URL = "/DailyTimeRecord/Late";
                notifvm.MarkRead = false;
                notifvm.CreatedBy = param.EmployeeId;
                notifvm.IsActive = true;
                await _homeRepository.SaveNotification(notifvm);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private async Task<string> GenereteRequestNoForUndertime()
        {

            try
            {
                int _currentYear = DateTime.Now.Year;
                int _currentMonth = DateTime.Now.Month;
                var _leaveContext = await _dbContext.LeaveRequestHeader
                                                .Where(x => x.IsActive == true && x.DateFiled.Date.Year == _currentYear && x.DateFiled.Date.Month == _currentMonth)
                                                .AsQueryable()
                                                .ToListAsync();


                int totalrecords = _leaveContext.Count() + 1;
                string finalSetRecords = GetFormattedRecordForLate(totalrecords);
                string yearMonth = DateTime.Now.ToString("yyyyMM");
                string req = "LT";

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

        private string GetFormattedRecordForLate(int totalRecords)
        {
            int setA = totalRecords % 1000;
            int setB = totalRecords / 1000;
            string formattedA = setA.ToString("D4");
            string formattedB = setB.ToString("D4");
            return $"{formattedA}";
        }

        private async Task<string> GenereteRequestNoForLateDeduction()
        {

            try
            {
                int _currentYear = DateTime.Now.Year;
                int _currentMonth = DateTime.Now.Month;
                var _leaveContext = await _dbContext.LateHeader
                                                .Where(x => x.IsActive == true)
                                                .AsQueryable()
                                                .ToListAsync();


                int totalrecords = _leaveContext.Count() + 1;
                string finalSetRecords = GetFormattedRecordForLate(totalrecords);
                string yearMonth = DateTime.Now.ToString("yyyyMM");
                string req = "LTD";

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


        public async Task<IList<LateHeaderViewModel>> GetLateDeduction(DailyTimeRecordViewModel model)
        {
            var rows = await (
                from ot in _dbContext.LateHeader
                join usr in _dbContext.User on ot.CreatedBy equals usr.UserId
                join emp in _dbContext.Employee on usr.EmployeeId equals emp.EmployeeId
                where ot.IsActive
                orderby ot.DateCreated descending
                select new LateHeaderViewModel
                {
                    LateHeaderId = ot.LateHeaderId,
                    RequestNo = ot.RequestNo,
                    DateFrom = ot.DateFrom,
                    DateTo = ot.DateTo,
                    DateCreated = ot.DateCreated,
                    CreatedBy = ot.CreatedBy,
                    CreatedName = emp.Lastname + " " + emp.Firstname
                })
                .AsNoTracking()
                .ToListAsync();

            return rows;
        }


        public async Task<IList<LateDetailViewModel>> GetLateDeductionByHeaderId(DailyTimeRecordViewModel model)
        {
            var rows = await (
                from ot in _dbContext.LateDetail
                join attdnce in _dbContext.vw_AttendanceSummary on ot.AttendanceId equals attdnce.ID
                //   join emp in _dbContext.Employee on usr.EmployeeId equals emp.EmployeeId
                where ot.IsActive
                //orderby ot.DateCreated descending
                select new LateDetailViewModel
                {
                    LateDetailId = ot.LateDetailId,
                    LateHeaderId = ot.LateHeaderId,
                    EMPLOYEE_NO = attdnce.EMPLOYEE_NO,
                    NAME = attdnce.NAME,
                    AttendanceId = ot.AttendanceId,
                    DATE = attdnce.DATE,
                    FIRST_IN = attdnce.FIRST_IN,
                    LAST_OUT = attdnce.LAST_OUT,
                    LATE = attdnce.LATE,
                    TOTAL_WORKING_HOURS = attdnce.TOTAL_WORKING_HOURS,
                    DeductionType = ot.DeductionType,
                    DeductionTypeName = ot.DeductionType == 1 ? "Payroll" : (ot.DeductionType == 2 ? "Vacation Leave" : "Sick Leave"),
                    // DeductionTypeName = EnumHelper.GetEnumDescriptionByTypeValue(EnumDeductionType ,(int)EnumDeductionType.VacationLeave))
                })
                .AsNoTracking()
                .ToListAsync();
            return rows;
        }

    }
}
