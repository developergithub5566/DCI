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
    public class UndertimeRepository : IUndertimeRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        private IEmailRepository _emailRepository;
        private readonly IHomeRepository _homeRepository;

        public UndertimeRepository(DCIdbContext dbContext, IEmailRepository emailRepository , IHomeRepository homeRepository)
        {
            _dbContext = dbContext;
            _emailRepository = emailRepository;
            _homeRepository = homeRepository;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<IList<DailyTimeRecordViewModel>> GetAllUndertime(DailyTimeRecordViewModel model)
        {
            //var context = _dbContext.vw_AttendanceSummary.AsQueryable().Where(x => x.STATUS != (int)EnumStatus.PayrollDeducted);
            //var context = _dbContext.vw_AttendanceSummary.AsQueryable().Where(x => x.STATUS == (int)EnumStatus.Raw);
            var context = (from dtr in _dbContext.vw_AttendanceSummary.AsNoTracking()
                           join emp in _dbContext.Employee.AsNoTracking() on dtr.EMPLOYEE_NO equals emp.EmployeeNo
                           where dtr.STATUS == (int)EnumStatus.Raw
                           select new
                           {
                               EMPLOYEE_NO = dtr.EMPLOYEE_NO,
                               DATE = dtr.DATE,
                               NAME = emp.Firstname + " " + emp.Lastname,
                               UNDER_TIME = dtr.UNDER_TIME,
                           }).ToList();


            int _currentYear = DateTime.Now.Year;

            var dateTo = model.DateTo.Date.AddDays(1).AddTicks(-1);

            //var leaveInfo = (from li in _dbContext.LeaveInfo
            //                 join emp in _dbContext.Employee on li.EmployeeId equals emp.EmployeeId
            //                 where li.IsActive && li.DateCreated.Year == 2025
            //                 group li by li.EmployeeId into g
            //                 select g.OrderByDescending(x => x.DateCreated).FirstOrDefault()
            //               ).ToList();

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
                            dtr.UNDER_TIME
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
                              dtr.UNDER_TIME,
                              LeaveBalance = li?.Leave.VLBalance ?? 0,
                          }).ToList();

            var query = joined
                .GroupBy(x => new { x.EMPLOYEE_NO, x.NAME, x.LeaveBalance })
                .Select(g =>
                {
                    var totalUnderTime = g
                        .Select(x => TimeSpan.TryParse(x.UNDER_TIME, out var t) ? t : TimeSpan.Zero)
                        .Aggregate(TimeSpan.Zero, (sum, next) => sum + next);

                    //var totalMinutes = g.Sum(x =>
                    //     TimeSpan.TryParse(x.UNDER_TIME, out var t) ? t.TotalMinutes : 0);
                    var totalMinutes = g.Sum(x =>
                    {
                        if (string.IsNullOrWhiteSpace(x.UNDER_TIME))
                            return 0.0;

                        var parts = x.UNDER_TIME.Split(':');
                        var cleanLate = parts.Length >= 2 ? $"{parts[0]}:{parts[1]}" : x.UNDER_TIME;

                        return TimeSpan.TryParse(cleanLate, out var t)
                            ? t.TotalMinutes
                            : 0.0;
                    });


                    return new DailyTimeRecordViewModel
                    {
                        EMPLOYEE_NO = g.Key.EMPLOYEE_NO,
                        NAME = g.Key.NAME,
                        DateFrom = model.DateFrom,
                        DateTo = model.DateTo,
                        //TOTAL_UNDERTIME = string.Format("{0:0.0000}", totalMinutes)  ,
                        TOTAL_UNDERTIME = TimeHelper.ConvertMinutesToValue(totalMinutes).ToString(),
                        //TOTAL_UNDERTIMEHOURS = "Hour:" + totalUnderTime.ToString(@"hh") + " Minute:" + totalUnderTime.ToString(@"mm"), // + " Second:" + totalUnderTime.ToString(@"ss")
                        TOTAL_UNDERTIMEHOURS = totalUnderTime.ToString(@"hh") + " Hour/s" + " and " + totalUnderTime.ToString(@"mm") + " Minute/s",
                        VLBalance = g.Key.LeaveBalance
                    };
                })                 
                .ToList();

            return query.Where(x => x.TOTAL_UNDERTIME != "0").ToList();
        }

        public async Task<IList<DailyTimeRecordViewModel>> GetUndertimeById(DailyTimeRecordViewModel model)
        {
            try
            {
                var context = _dbContext.vw_AttendanceSummary.AsQueryable();

                var empdbcontext = _dbContext.Employee.Where(x => x.EmployeeNo == model.EMPLOYEE_NO).FirstOrDefault();
                var _empId = empdbcontext?.EmployeeId ?? 0;

                var dateTo = model.DateTo.Date.AddDays(1).AddTicks(-1);


                //var leaveQuery =
                //              from lv in _dbContext.LeaveRequestDetails
                //              join hdr in _dbContext.LeaveRequestHeader on lv.LeaveRequestHeaderId equals hdr.LeaveRequestHeaderId
                //              join stat in _dbContext.Status on hdr.Status equals stat.StatusId
                //              where lv.LeaveDate >= model.DateFrom && lv.LeaveDate <= dateTo
                //                    && hdr.EmployeeId == _empId
                //                    && (hdr.LeaveTypeId == (int)EnumLeaveType.HD
                //                        || hdr.LeaveTypeId == (int)EnumLeaveType.OB
                //                        || hdr.LeaveTypeId == (int)EnumLeaveType.SL)
                //              select new
                //              {
                //                  LeaveDate = lv.LeaveDate,
                //                  RequestNo = hdr.RequestNo,
                //                  StatusName = stat.StatusName
                //              };


                //var query = (from dtr in context
                //             where dtr.DATE >= model.DateFrom && dtr.DATE <= dateTo && dtr.EMPLOYEE_NO == model.EMPLOYEE_NO
                //             join lv in leaveQuery on dtr.DATE.Date equals lv.LeaveDate.Date into gj
                //             from lv in gj.DefaultIfEmpty()
                //             select new DailyTimeRecordViewModel
                //             {
                //                 ID = dtr.ID,
                //                 EMPLOYEE_NO = dtr.EMPLOYEE_NO,
                //                 NAME = dtr.NAME,
                //                 DATE = dtr.DATE,
                //                 FIRST_IN = dtr.FIRST_IN,
                //                 LAST_OUT = dtr.LAST_OUT,
                //                 LATE = dtr.LATE,
                //                 CLOCK_OUT = dtr.CLOCK_OUT,
                //                 UNDER_TIME = dtr.UNDER_TIME,
                //                 OVERTIME = dtr.OVERTIME,
                //                 TOTAL_HOURS = dtr.TOTAL_HOURS,
                //                 TOTAL_WORKING_HOURS = dtr.TOTAL_WORKING_HOURS,
                //                 DATESTRING = dtr.DATE.ToString("MM/dd/yyyy"),
                //                 ModulePageId = (int)EnumModulePage.DailyTimeRecord,
                //                 RequestNo = (lv == null ? "" : lv.RequestNo),
                //                 STATUSNAME = (lv == null ? "" : lv.StatusName)
                //             }).ToList();


                // inputs
                //var empNo = "080343";
                //var dateFrom = new DateTime(2025, 9, 1);
                //var dateToEx = new DateTime(2025, 9, 9).AddDays(1); // half-open, inclusive end

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
                                        && (b.LeaveTypeId == (int)EnumLeaveType.HDVL
                                        || b.LeaveTypeId == (int)EnumLeaveType.HDSL
                                        || b.LeaveTypeId == (int)EnumLeaveType.OB
                                        || b.LeaveTypeId == (int)EnumLeaveType.HDOB
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
                        EmployeeId = e.EmployeeId,
                        NAME = e.Firstname + " " + e.Lastname,
                    }
                ).ToListAsync();


                //var result = att.Select(x =>
                //{
                //    var d = x.Row.DATE.Date; 
                //    var key = new { x.EmployeeId, Date = d };

                //    var agg = leaveMap.TryGetValue(key, out var found) ? found : null;

                //    return new DailyTimeRecordViewModel
                //    {
                //        ID = x.Row.ID,
                //        EMPLOYEE_NO = x.Row.EMPLOYEE_NO,
                //        NAME = x.Row.NAME,
                //        DATE = x.Row.DATE,
                //        FIRST_IN = x.Row.FIRST_IN,
                //        LAST_OUT = x.Row.LAST_OUT,
                //        LATE = x.Row.LATE,
                //        CLOCK_OUT = x.Row.CLOCK_OUT,
                //        UNDER_TIME = x.Row.UNDER_TIME,
                //        OVERTIME = x.Row.OVERTIME,
                //        TOTAL_HOURS = x.Row.TOTAL_HOURS,
                //        TOTAL_WORKING_HOURS = x.Row.TOTAL_WORKING_HOURS,
                //        DATESTRING = x.Row.DATE.ToString("MM/dd/yyyy"),
                //        ModulePageId = (int)EnumModulePage.DailyTimeRecord,

                //        RequestNo = agg?.RequestNos ?? string.Empty,
                //        STATUSNAME = agg?.StatusNames ?? string.Empty
                //    };
                //}).ToList();

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
                        NAME = x.NAME,
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


        public async Task<(int statuscode, string message)> SaveUndertime(UndertimeHeaderDeductionViewModel model)
        {
            //   DTRCorrectionViewModel model = new DTRCorrectionViewModel();

            try
            {
                if (model.UndertimeDeductionList.Count() > 0)
                {
                    //create audit logs for undertime execution 
                    UndertimeHeader oth = new UndertimeHeader();
                    oth.RequestNo = await GenereteRequestNoForUndertimeDeduction();
                    oth.DateFrom = model.DateFrom;
                    oth.DateTo = model.DateTo;
                    oth.DateCreated = DateTime.Now;
                    oth.CreatedBy = model.CurrentUserId;
                    oth.IsActive = true;
                    await _dbContext.UndertimeHeader.AddAsync(oth);
                    await _dbContext.SaveChangesAsync();

                    foreach (var ut in model.UndertimeDeductionList)
                    {
                        if (ut.EmpNo != null && ut.TotalUndertime > 0)
                        {
                            var dateTo = ut.DateTo.Date.AddDays(1).AddTicks(-1);
                            var emp = _dbContext.Employee.Where(x => x.EmployeeNo == ut.EmpNo).FirstOrDefault();
                            var workdtls = _dbContext.EmployeeWorkDetails.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();
                            //var leafinfo = _dbContext.LeaveInfo.Where(x => x.EmployeeId == emp.EmployeeId).FirstOrDefault();
                            var leafinfo = _dbContext.LeaveInfo.Where(li => li.IsActive && li.EmployeeId == emp.EmployeeId && li.DateCreated.Year == DateTime.Now.Year).OrderByDescending(li => li.DateCreated).FirstOrDefault();

                                                     

                           

                            //Insert Leave application : leavetype Undertime , Status automatic approved and Insert Notification 
                            LeaveFormViewModel lvFormmodel = new LeaveFormViewModel();
                            lvFormmodel.EmployeeId = emp.EmployeeId;
                            lvFormmodel.NoOfDays = ut.TotalUndertime ?? 0;
                            lvFormmodel.DateFromTo = model.DateFrom.ToShortDateString() + " to " + model.DateTo.ToShortDateString();
                            await SaveLeaveForUndertime(lvFormmodel);


                            var attendanceList = await _dbContext.vw_AttendanceSummary.Where(x => x.EMPLOYEE_NO == ut.EmpNo && x.DATE >= ut.DateFrom.Date && x.DATE <= dateTo.Date
                                                                                                                                                          && x.STATUS == (int)EnumStatus.Raw).ToListAsync();

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
                                    UndertimeDetail otd = new UndertimeDetail();
                                    otd.UndertimeHeaderId = oth.UndertimeHeaderId;
                                    otd.AttendanceId = (int)attdnc.ID;
                                    otd.DeductionType = (int)EnumDeductionType.VacationLeave;
                                    otd.IsActive = true;
                                    await _dbContext.UndertimeDetail.AddAsync(otd);
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
                                    UndertimeDetail otd = new UndertimeDetail();
                                    otd.UndertimeHeaderId = oth.UndertimeHeaderId;
                                    otd.AttendanceId = (int)attdnc.ID;
                                    otd.DeductionType = (int)EnumDeductionType.Payroll;
                                    otd.IsActive = true;
                                    await _dbContext.UndertimeDetail.AddAsync(otd);
                                    await _dbContext.SaveChangesAsync();
                                }
                            }                          
                        }
                    }

                    //Send Notification to Executor/HR 
                    NotificationViewModel notifvm = new NotificationViewModel();
                    notifvm.Title = "Undertime Deduction";
                    notifvm.Description = System.String.Format("Undertime deduction process {0} has been executed successfully.", oth.RequestNo);
                    notifvm.ModuleId = (int)EnumModulePage.Undertime;
                    notifvm.TransactionId = oth.UndertimeHeaderId;
                    notifvm.AssignId = model.CurrentUserId;
                    notifvm.URL = "/Report/Undertime";
                    notifvm.MarkRead = false;
                    notifvm.CreatedBy = model.CurrentUserId;
                    notifvm.IsActive = true;
                    await _homeRepository.SaveNotification(notifvm);

                    return (StatusCodes.Status200OK, Constants.Msg_Deduction);
                }

                return (StatusCodes.Status200OK, Constants.Msg_NoRecordFound);
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

        private async Task SaveLeaveForUndertime(LeaveFormViewModel param)
        {
            LeaveViewModel model = new LeaveViewModel();

            try
            {

                LeaveRequestHeader entity = new LeaveRequestHeader();
                entity.EmployeeId = param.EmployeeId;
                entity.RequestNo = await GenereteRequestNoForLeave();
                entity.DateFiled = DateTime.Now;
                entity.LeaveTypeId = (int)EnumLeaveType.UT;
                entity.Status = (int)EnumStatus.VLDeducted;
                entity.Reason = System.String.Format("System-Generated Undertime Deduction for the period {0}." , param.DateFromTo);
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


                var usr = _dbContext.User.AsNoTracking().Where(x => x.EmployeeId == param.EmployeeId).FirstOrDefault();

                NotificationViewModel notifvm = new NotificationViewModel();
                notifvm.Title = "Undertime";
                notifvm.Description = System.String.Format("System-Generated Undertime Deduction has been processed.", entity.RequestNo);
                notifvm.ModuleId = (int)EnumModulePage.Undertime;
                notifvm.TransactionId = entity.LeaveRequestHeaderId;
                notifvm.AssignId = usr != null ? usr.UserId : 0;
                notifvm.URL = "/DailyTimeRecord/Leave";
                notifvm.MarkRead = false;
                notifvm.CreatedBy = param.CurrentUserId;
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

        private async Task<string> GenereteRequestNoForLeave()
        {

            try
            {
                int _currentYear = DateTime.Now.Year;
                int _currentMonth = DateTime.Now.Month;
                var _leaveContext = await _dbContext.LeaveRequestHeader
                                                .Where(x => x.IsActive == true && x.DateFiled.Date.Year == _currentYear)
                                                .AsQueryable()
                                                .ToListAsync();


                int totalrecords = _leaveContext.Count() + 1;
                string finalSetRecords = FormatHelper.GetFormattedRequestNo(totalrecords);
                string yearMonth = DateTime.Now.ToString("yyyyMM");
                string req = Constants.ModuleCode_Leave;

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

        //private string GetFormattedRecordForUndertime(int totalRecords)
        //{
        //    int setA = totalRecords % 1000;
        //    int setB = totalRecords / 1000;
        //    string formattedA = setA.ToString("D4");
        //    string formattedB = setB.ToString("D4");
        //    return $"{formattedA}";
        //}

        private async Task<string> GenereteRequestNoForUndertimeDeduction()
        {

            try
            {
                int _currentYear = DateTime.Now.Year;
               // int _currentMonth = DateTime.Now.Month;
                var _leaveContext = await _dbContext.UndertimeHeader
                                                .Where(x => x.IsActive == true && x.DateCreated.Date.Year == _currentYear)
                                                .AsNoTracking()
                                                .ToListAsync();


                int totalrecords = _leaveContext.Count() + 1;
                string finalSetRecords = FormatHelper.GetFormattedRequestNo(totalrecords);
                string yearMonth = DateTime.Now.ToString("yyyyMM");
                string req = Constants.ModuleCode_Undertime_Deduction;

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


        public async Task<IList<UndertimeHeaderViewModel>> GetUndertimeDeduction(DailyTimeRecordViewModel model)
        {
            var rows = await (
                from ot in _dbContext.UndertimeHeader.AsNoTracking()
                join usr in _dbContext.User.AsNoTracking() on ot.CreatedBy equals usr.UserId
                join emp in _dbContext.Employee.AsNoTracking() on usr.EmployeeId equals emp.EmployeeId
                where ot.IsActive 
                orderby ot.DateCreated descending
                select new UndertimeHeaderViewModel
                {
                    UndertimeHeaderId = ot.UndertimeHeaderId,
                    RequestNo = ot.RequestNo,
                    DateFrom = ot.DateFrom,
                    DateTo = ot.DateTo,
                    DateCreated = ot.DateCreated,
                    CreatedBy = ot.CreatedBy,
                    CreatedName = emp.Lastname + " " + emp.Firstname // <-- your original logic
                })
                .AsNoTracking()
                .ToListAsync();

            return rows;
        }

        
        public async Task<IList<UndertimeDetailViewModel>> GetUndertimeDeductionByHeaderId(UndertimeHeaderViewModel model)
        {
            var rows = await (
                from ot in _dbContext.UndertimeDetail.AsNoTracking()
                join attdnce in _dbContext.vw_AttendanceSummary.AsNoTracking() on ot.AttendanceId equals attdnce.ID
                join emp in _dbContext.Employee.AsNoTracking() on attdnce.EMPLOYEE_NO equals emp.EmployeeNo
                where ot.IsActive && ot.UndertimeHeaderId == model.UndertimeHeaderId
                //orderby ot.DateCreated descending
                select new UndertimeDetailViewModel
                {
                    UndertimeDetailId = ot.UndertimeDetailId,
                    UndertimeHeaderId = ot.UndertimeHeaderId,
                    EMPLOYEE_NO = attdnce.EMPLOYEE_NO,
                    NAME = emp.Firstname + " " + emp.Lastname,
                    AttendanceId = ot.AttendanceId,
                    DATE = attdnce.DATE,
                    FIRST_IN = attdnce.FIRST_IN,
                    LAST_OUT = attdnce.LAST_OUT,
                    UNDER_TIME = attdnce.UNDER_TIME,
                    TOTAL_WORKING_HOURS = attdnce.TOTAL_WORKING_HOURS,
                    DeductionType = ot.DeductionType,
                    DeductionTypeName = ot.DeductionType == (int)EnumDeductionType.Payroll ? "Payroll" : (ot.DeductionType == (int)EnumDeductionType.VacationLeave ? "Vacation Leave" : "Sick Leave"),
                   // DeductionTypeName = EnumHelper.GetEnumDescriptionByTypeValue(EnumDeductionType ,(int)EnumDeductionType.VacationLeave))
                })               
                .ToListAsync();



           // var dsad = EnumHelper.GetEnumDescriptionByTypeValue(DCI.Core.Common.EnumDeductionType, 1);
            return rows;
        }

    }
}
