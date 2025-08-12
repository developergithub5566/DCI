using DCI.Core.Common;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
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

        public async Task<IList<OvertimeViewModel>> Overtime(OvertimeViewModel model)
        {

            var query = from ot in _dbContext.OvertimeHeader
                        join usr in _dbContext.User on ot.CreatedBy equals usr.UserId
                        join emp in _dbContext.Employee on usr.EmployeeId equals emp.EmployeeId
                        join stat in _dbContext.Status on ot.StatusId equals stat.StatusId
                        where ot.IsActive && ot.CreatedBy == model.CurrentUserId
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
                join usr in _dbContext.User.AsNoTracking() on ot.CreatedBy equals usr.UserId
                join emp in _dbContext.Employee.AsNoTracking() on usr.EmployeeId equals emp.EmployeeId
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
                    DateCreated = ot.DateCreated,
                    CreatedBy = ot.CreatedBy,
                    OvertimeDetailViewModel = _dbContext.OvertimeDetail
                        .Where(x => x.OTHeaderId == ot.OTHeaderId /* && x.IsActive */)
                        .OrderBy(x => x.OTDate).ThenBy(x => x.OTTimeFrom)
                        .Select(x => new OvertimeDetailViewModel
                        {
                            OTHeaderId = x.OTHeaderId,
                            OTType = x.OTType,
                            OTDate = x.OTDate,
                            OTTimeFrom = x.OTTimeFrom,
                            OTTimeTo = x.OTTimeTo,
                            TotalMinutes = x.TotalMinutes
                        })
                        .ToList()
                };

            return await query.FirstOrDefaultAsync();
        }


        public async Task<(int statuscode, string message)> SaveOvertime(SubmitOvertimeViewModel param)
        {

      

            try
            {
                if (param.RequestNo ==  string.Empty)
                {
                    OvertimeHeader entity = new OvertimeHeader();
                   
                    entity.RequestNo = await GenereteRequestNo();
                    entity.StatusId = (int)EnumStatus.Pending;
                    entity.EmployeeId = param.CurrentUserId;
                    entity.DateCreated = DateTime.Now;
                    entity.CreatedBy = param.CurrentUserId;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    await _dbContext.OvertimeHeader.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();

                    //int count = param.Entries.Count();

                    foreach (var dtl in param.Entries)
                    {
                        DateTime combined = dtl.OTDate.Date + TimeSpan.Parse(dtl.OTTimeFrom);

                        OvertimeDetail entityDtl = new OvertimeDetail();
                        entityDtl.OTHeaderId = entity.OTHeaderId;
                        entityDtl.OTType = dtl.OTType;
                        entityDtl.OTDate = dtl.OTDate;
                        entityDtl.OTTimeFrom = dtl.OTDate.Date + TimeSpan.Parse(dtl.OTTimeFrom);
                        entityDtl.OTTimeTo = dtl.OTDate.Date + TimeSpan.Parse(dtl.OTTimeTo);
                        entityDtl.IsActive = true;
                        await _dbContext.OvertimeDetail.AddAsync(entityDtl);
                        await _dbContext.SaveChangesAsync();
                    }
                    return (StatusCodes.Status200OK, "Successfully saved");
                }
                else
                {
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
    }
}
