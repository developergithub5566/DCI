using DCI.Core.Common;
using DCI.Core.Helpers;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DCI.Repositories
{
    public class OvertimeRepository : IOvertimeRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        private IEmailRepository _emailRepository;
        public OvertimeRepository(DCIdbContext dbContext, IEmailRepository emailRepository)
        {
            _dbContext = dbContext;
            _emailRepository = emailRepository;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<DailyTimeRecordViewModel> GetAllAttendanceByDate(OvertimeViewModel model)
        {
            var context = _dbContext.vw_AttendanceSummary.AsNoTracking().Where(x => x.DATE == model.OTDate);
            var user = _dbContext.User.AsNoTracking().Where(x => x.UserId == model.CurrentUserId).FirstOrDefault();
            var employee = _dbContext.Employee.AsNoTracking().Where(x => x.EmployeeId == user.EmployeeId).FirstOrDefault();

            var query = (from dtr in context
                         where dtr.DATE == model.OTDate && dtr.EMPLOYEE_NO == employee.EmployeeNo
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
                         }).FirstOrDefault() ?? new DailyTimeRecordViewModel();

            query.IsHoliday = _dbContext.Holiday.AsNoTracking().Any(x => x.HolidayDate == model.OTDate);


            query.IsBiometricRecord = query.ID > 0 ? true : false;

            if (model.IsOfficialBuss)
            {
                var x = (from hdr in _dbContext.LeaveRequestHeader.AsNoTracking()
                         join dtl in _dbContext.LeaveRequestDetails.AsNoTracking() on hdr.LeaveRequestHeaderId equals dtl.LeaveRequestHeaderId
                         where hdr.IsActive && dtl.LeaveDate.Date == model.OTDate.Date && hdr.LeaveTypeId == (int)EnumLeaveType.OB && hdr.ApproverId == (int)EnumStatus.Approved
                         select new
                         {
                             Exist = hdr.LeaveRequestHeaderId
                         });

                query.IsOBFileRecord = x.Count() == 0 ? false : true;
            }

            var _wfh = _dbContext.vw_AttendanceSummary_WFH.AsNoTracking().Where(x => x.DATE == model.OTDate).FirstOrDefault();
            if(_wfh != null)
            {
                query.IsWFHFileRecord = true;
                query.FIRST_IN_WFH = _wfh.FIRST_IN;
                query.LAST_OUT_WFH = _wfh.LAST_OUT ;
            }

            return query;
        }

        public async Task<IList<OvertimeViewModel>> Overtime(OvertimeViewModel model)
        {

            var query = from ot in _dbContext.OvertimeHeader
                        join usr in _dbContext.User on ot.CreatedBy equals usr.UserId
                        join emp in _dbContext.Employee on usr.EmployeeId equals emp.EmployeeId
                        join stat in _dbContext.Status on ot.StatusId equals stat.StatusId
                        where ot.IsActive
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

            if ((int)EnumEmployeeScope.PerEmployee == model.ScopeTypeEmp)
            {
                //var usr = _dbContext.User.Where(x => x.UserId == model.CurrentUserId).FirstOrDefault();
                //var emp = _dbContext.Employee.Where(x => x.EmployeeId == usr.EmployeeId).FirstOrDefault();
                //if (emp != null)
                query = query.Where(x => x.CreatedBy == model.CurrentUserId);
            }

            return await query.ToListAsync();
        }

        //public async Task<OvertimeViewModel> AddOvertime(OvertimeViewModel model)
        //{  
        //    var query = from ot in _dbContext.OvertimeHeader
        //                join usr in _dbContext.User on ot.CreatedBy equals usr.UserId
        //                join emp in _dbContext.Employee on usr.EmployeeId equals emp.EmployeeId
        //                join stat in _dbContext.Status on ot.StatusId equals stat.StatusId
        //                where ot.IsActive && ot.OTHeaderId == model.OTHeaderId
        //                select new OvertimeViewModel
        //                {
        //                    OTHeaderId = ot.OTHeaderId,
        //                    RequestNo = ot.RequestNo,
        //                    Fullname = emp.Firstname + " " + emp.Lastname,
        //                    EmployeeId = ot.EmployeeId,
        //                    StatusId = ot.StatusId,
        //                    StatusName = stat.StatusName,
        //                    DateCreated = ot.DateCreated,
        //                    CreatedBy = ot.CreatedBy,
        //                    OvertimeDetailViewModel = _dbContext.OvertimeDetail
        //                                                               .Where(x => x.OTHeaderId == ot.OTHeaderId)
        //                                                                .Select(x => new OvertimeDetailViewModel
        //                                                                {
        //                                                                    OTHeaderId = x.OTHeaderId,
        //                                                                    OTType = x.OTType,
        //                                                                    OTDate = x.OTDate,
        //                                                                    OTTimeFrom = x.OTTimeFrom,
        //                                                                    OTTimeTo = x.OTTimeTo,
        //                                                                    TotalMinutes = x.TotalMinutes,
        //                                                                }).ToList()
        //                };

        //    return query.FirstOrDefault();
        //}
        public async Task<OvertimeViewModel?> AddOvertime(OvertimeViewModel model)
        {


            var query =
                from ot in _dbContext.OvertimeHeader.AsNoTracking()
                    //join usr in _dbContext.User.AsNoTracking() on ot.CreatedBy equals model.CurrentUserId
                join emp in _dbContext.Employee.AsNoTracking() on ot.EmployeeId equals emp.EmployeeId
                // join empWork in _dbContext.EmployeeWorkDetails.AsNoTracking() on emp.EmployeeId equals empWork.EmployeeId
                // join dept in _dbContext.Department.AsNoTracking() on empWork.DepartmentId equals dept.DepartmentId

                join usrApprover in _dbContext.User.AsNoTracking() on ot.ApproverId equals usrApprover.UserId into usrApproverGroup
                from usrApprover in usrApproverGroup.DefaultIfEmpty()

                join stat in _dbContext.Status.AsNoTracking() on ot.StatusId equals stat.StatusId
                where ot.IsActive && ot.OTHeaderId == model.OTHeaderId
                select new OvertimeViewModel
                {
                    OTHeaderId = ot.OTHeaderId,
                    RequestNo = ot.RequestNo,
                    Fullname = emp.Firstname + " " + emp.Lastname,
                    EmployeeId = ot.EmployeeId,
                    StatusId = ot.StatusId,
                    StatusName = stat.StatusName,
                    Remarks = ot.Remarks,
                    DateCreated = ot.DateCreated,
                    CreatedBy = ot.CreatedBy,
                    DateCreatedString = ot.DateCreated.ToString("yyyy-MM-dd HH:mm"),

                    // RecommendedBy = usrApprover != null ? usrApprover.Firstname + " " + usrApprover.Lastname : string.Empty,
                    // ApprovedBy = "MARCO USTARIS",
                    ApprovedBy = usrApprover != null ? usrApprover.Firstname + " " + usrApprover.Lastname : string.Empty,
                    otDetails = _dbContext.OvertimeDetail
                        .Where(x => x.OTHeaderId == ot.OTHeaderId && x.IsActive)
                        .OrderBy(x => x.OTDate).ThenBy(x => x.OTTimeFrom)
                        .Select(x => new OvertimeDetailViewModel
                        {
                            OTTypeName = x.OTType == 1 ? Constants.OverTime_Regular
                                       : x.OTType == 2 ? Constants.OverTime_NightDifferential
                                       : x.OTType == 3 ? Constants.OverTime_SpecialHoliday
                                       : x.OTType == 4 ? Constants.OverTime_After8hrs
                                       : x.OTType == 5 ? Constants.OverTime_HolidayOnRestDay
                                       : "",
                            OTHeaderId = x.OTHeaderId,
                            OTDetailId = x.OTDetailId,
                            OTType = x.OTType,
                            OTDate = x.OTDate.ToString(),
                            OTDateString = x.OTDate.ToString("yyyy-MM-dd"),
                            OTTimeFrom = x.OTTimeFrom.ToString("HH:mm:ss"),
                            OTTimeTo = x.OTTimeTo.ToString("HH:mm:ss"),
                            TotalMinutes = x.TotalMinutes,
                            TotalHours = TimeHelper.ConvertMinutesToHHMM((int)x.TotalMinutes)
                        })
                        .ToList()
                };

            return await query.FirstOrDefaultAsync() ?? new OvertimeViewModel();
        }


        public async Task<(int statuscode, string message)> SaveOvertime(OvertimeViewModel param)
        {



            try
            {
                if (param.OTHeaderId == 0)
                {
                    OvertimeHeader entity = new OvertimeHeader();

                    entity.RequestNo = await GenereteRequestNo();
                    entity.StatusId = (int)EnumStatus.ForApproval;
                    entity.EmployeeId = param.CurrentUserId;
                    entity.Remarks = param.Remarks;
                    entity.ApproverId = param.ApproverId;
                    entity.DateCreated = DateTime.Now;
                    entity.CreatedBy = param.CurrentUserId;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    await _dbContext.OvertimeHeader.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();

                    //int count = param.Entries.Count();

                    foreach (var dtl in param.otDetails)
                    {
                        // DateTime combined = date.Parse(dtl.OTDate.Date) + TimeSpan.Parse(dtl.OTTimeFrom);

                        OvertimeDetail entityDtl = new OvertimeDetail();
                        entityDtl.OTHeaderId = entity.OTHeaderId;
                        entityDtl.OTType = dtl.OTType;
                        entityDtl.OTDate = DateTime.Parse(dtl.OTDate);
                        entityDtl.OTTimeFrom = DateTime.Parse(dtl.OTDate) + TimeSpan.Parse(dtl.OTTimeFrom);
                        entityDtl.OTTimeTo = DateTime.Parse(dtl.OTDate) + TimeSpan.Parse(dtl.OTTimeTo);
                        entityDtl.IsActive = true;
                        await _dbContext.OvertimeDetail.AddAsync(entityDtl);
                        await _dbContext.SaveChangesAsync();
                    }

                    var result =
                            from b in _dbContext.User.AsNoTracking()
                            join c in _dbContext.EmployeeWorkDetails.AsNoTracking()
                                on b.EmployeeId equals c.EmployeeId into cGroup
                            from c in cGroup.DefaultIfEmpty()

                            join d in _dbContext.Department.AsNoTracking()
                                on c.DepartmentId equals d.DepartmentId into dGroup
                            from d in dGroup.DefaultIfEmpty()

                            where b.UserId == param.CurrentUserId
                            select new
                            {
                                RecommendedById = d.ApproverId
                            };

                    var hrHead = _dbContext.Department.AsNoTracking().Where(x => x.DepartmentCode == "HR").FirstOrDefault();

                    param.RecommendedById = result.FirstOrDefault().RecommendedById ?? 0;
                    param.ApproverId = hrHead.ApproverId ?? 0;
                    param.RequestNo = entity.RequestNo;
                    param.StatusId = entity.StatusId;
                    await _emailRepository.SentToApprovalOvertime(param);

                    return (StatusCodes.Status200OK, "Successfully saved");
                }
                else
                {
                    var entity = await _dbContext.OvertimeHeader.FirstOrDefaultAsync(x => x.OTHeaderId == param.OTHeaderId);

                    entity.StatusId = (int)EnumStatus.Pending;
                    entity.EmployeeId = param.CurrentUserId;
                    entity.Remarks = param.Remarks;
                    entity.DateCreated = DateTime.Now;
                    entity.CreatedBy = param.CurrentUserId;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    _dbContext.OvertimeHeader.Entry(entity).State = EntityState.Modified;
                    await _dbContext.SaveChangesAsync();

                    await SaveOTDetail(param);

                    return (StatusCodes.Status200OK, "Successfully updated");
                }
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

        private async Task SaveOTDetail(OvertimeViewModel model)
        {
            // Get all existing reviewers for the WorkflowId
            var overtime = await _dbContext.OvertimeDetail
                .Where(x => x.OTHeaderId == model.OTHeaderId)
                .ToListAsync();

            // Get the new UserIds from the model
            var newApproverUserIds = model.otDetails.Select(r => r.OTDetailId).ToHashSet();

            var apprverToAdd = new List<OvertimeDetail>();
            var apprverToUpdate = new List<OvertimeDetail>();

            foreach (var ot in model.otDetails)
            {
                var existingAppr = overtime.FirstOrDefault(x => x.OTDetailId == ot.OTDetailId);

                if (existingAppr == null)
                {

                    apprverToAdd.Add(new OvertimeDetail
                    {
                        OTHeaderId = model.OTHeaderId,
                        OTType = ot.OTType,
                        OTDate = DateTime.Parse(ot.OTDate),
                        OTTimeFrom = DateTime.Parse(ot.OTDate) + TimeSpan.Parse(ot.OTTimeFrom),
                        OTTimeTo = DateTime.Parse(ot.OTDate) + TimeSpan.Parse(ot.OTTimeTo),
                        // TotalMinutes = ot.TotalMinutes,                       
                        IsActive = true
                    });
                }

                else
                {
                    existingAppr.IsActive = true;
                    _dbContext.OvertimeDetail.Update(existingAppr);
                }
            }

            var approverToDeactivate = overtime
                .Where(x => !newApproverUserIds.Contains(x.OTDetailId) && x.IsActive)
                .ToList();

            foreach (var apprver in approverToDeactivate)
            {
                apprver.IsActive = false;
                apprverToUpdate.Add(apprver);
            }

            // Batch insert for new reviewers
            if (apprverToAdd.Any())
            {
                await _dbContext.OvertimeDetail.AddRangeAsync(apprverToAdd);
            }

            // Save changes for both new and updated records
            if (apprverToAdd.Any() || apprverToUpdate.Any())
            {
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<OvertimeEntryDto> CheckOvertimeDate(OvertimeEntryDto model)
        {
            var data = (from ot in _dbContext.vw_AttendanceSummary
                        where ot.DATE.Date == model.OTDate.Date
                        select new OvertimeEntryDto
                        {
                            OTDate = ot.DATE,
                            OTTimeFrom = ot.FIRST_IN,
                            OTTimeTo = ot.LAST_OUT
                        }).FirstOrDefault();
            return data;
        }

        private async Task<string> GenereteRequestNo()
        {

            try
            {
                int _currentYear = DateTime.Now.Year;
                int _currentMonth = DateTime.Now.Month;

                var startDate = new DateTime(_currentYear, _currentMonth, 1);
                var endDate = startDate.AddMonths(1);

                var _dtr = await _dbContext.OvertimeHeader
                    .Where(x => x.IsActive == true
                        && x.DateCreated >= startDate
                        && x.DateCreated < endDate)
                    .ToListAsync();


                int totalrecords = _dtr.Count() + 1;
                string finalSetRecords = GetFormattedRecord(totalrecords);
                string yearMonth = DateTime.Now.ToString("yyyyMM");
                string req = "OT";

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

        public async Task<OvertimePayReport> GetOvertimeSummaryAsync(OvertimePayReport param)
        {
            //dateFrom = dateFrom.AddMonths(-2);
            // 1) Base join + filters (only active rows, by employee/date range)
            OvertimePayReport model = new OvertimePayReport();

            var baseQuery =
             from h in _dbContext.OvertimeHeader.AsNoTracking()
             join d in _dbContext.OvertimeDetail.AsNoTracking()
                 on h.OTHeaderId equals d.OTHeaderId
             where h.IsActive && d.IsActive
                   && h.EmployeeId == param.EmployeeId
             // && d.OTDate >= dateFrom && d.OTDate <= dateTo
             select new
             {
                 h.EmployeeId,
                 h.RequestNo,
                 d.OTDate,
                 d.OTTimeFrom,
                 d.OTTimeTo,
                 d.OTType,
                 d.TotalMinutes
             };

            var grouped = await baseQuery
                .GroupBy(g => new
                {
                    g.EmployeeId,
                    g.RequestNo,
                    Date = g.OTDate.Date,
                    g.OTTimeFrom,
                    g.OTTimeTo
                })
                .Select(g => new
                {
                    g.Key.EmployeeId,
                    g.Key.RequestNo,
                    g.Key.Date,
                    g.Key.OTTimeFrom,
                    g.Key.OTTimeTo,

                    Regular = g.Where(x => x.OTType == (int)EnumOvertime.Regular)
                               .Sum(x => (int?)x.TotalMinutes) ?? 0,
                    NightDifferential = g.Where(x => x.OTType == (int)EnumOvertime.NightDifferential)
                                         .Sum(x => (int?)x.TotalMinutes) ?? 0,
                    SpecialHoliday = g.Where(x => x.OTType == (int)EnumOvertime.SpecialHoliday)
                                      .Sum(x => (int?)x.TotalMinutes) ?? 0,
                    After8hrs = g.Where(x => x.OTType == (int)EnumOvertime.After8hrs)
                                 .Sum(x => (int?)x.TotalMinutes) ?? 0,
                    HolidayOnRestDay = g.Where(x => x.OTType == (int)EnumOvertime.HolidayOnRestDay)
                                        .Sum(x => (int?)x.TotalMinutes) ?? 0,
                    TotalMinutes = g.Sum(x => x.TotalMinutes)
                })
                .OrderBy(x => x.Date)
                .ToListAsync();



            var employeeList = await (
                            from emp in _dbContext.Employee.AsNoTracking()
                            join dtls in _dbContext.EmployeeWorkDetails.AsNoTracking()
                                on emp.EmployeeId equals dtls.EmployeeId
                            where emp.IsActive && dtls.IsActive && !dtls.IsResigned
                            select new EmployeeViewModel
                            {
                                EmployeeId = emp.EmployeeId,
                                EmployeeNo = emp.EmployeeNo,
                                Lastname = emp.Lastname,
                                Firstname = emp.Firstname,
                                IsResigned = dtls.IsResigned
                            }
                        ).ToListAsync();


            var result = grouped.Select(x =>
            {
                var ts = TimeSpan.FromMinutes(x.TotalMinutes);
                string hhmm = $"{(int)ts.TotalHours:00}:{ts.Minutes:00}";

                return new OvertimeEmployeeDetails
                {
                    EmployeeId = x.EmployeeId,
                    RequestNo = x.RequestNo,
                    OTDateString = x.Date.ToString("yyyy-MM-dd"),
                    OTTimeFrom = x.OTTimeFrom.ToString("HH:mm"),
                    OTTimeTo = x.OTTimeTo.ToString("HH:mm"),
                    Regular = TimeHelper.ConvertMinutesToValue(x.Regular),
                    NightDifferential = TimeHelper.ConvertMinutesToValue(x.NightDifferential),
                    SpecialHoliday = TimeHelper.ConvertMinutesToValue(x.SpecialHoliday),
                    After8hrs = TimeHelper.ConvertMinutesToValue(x.After8hrs),
                    HolidayOnRestDay = TimeHelper.ConvertMinutesToValue(x.HolidayOnRestDay),
                    TotalMinutes = x.TotalMinutes,
                    TotalHours = hhmm,

                };
            }).ToList();

            model.EmployeeList = employeeList.Where(x => x.IsResigned == false).OrderBy(x => x.Lastname).ToList();
            model.OvertimeEmployeeDetails = result;
            return model;
        }
    }
}
