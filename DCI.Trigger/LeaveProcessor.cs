using DCI.Core.Common;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace DCI.Trigger
{
    public class LeaveProcessor
    {
        private readonly DestinationDbContext _dbContext;

        public LeaveProcessor(DestinationDbContext context)
        {
            _dbContext = context;
        }

        public async Task MonthlyLeaveCredit()
        {
            try
            {

                //var emplist = await (
                //                 from emp in _dbContext.EmployeeWorkDetails.AsNoTracking()
                //                 where emp.IsActive
                //                       && !emp.IsResigned
                //                       && emp.EmployeeStatusId != 3
                //                       && emp.EmployeeStatusId != 4
                //                 join leaveinfo in (
                //                     from li in _dbContext.LeaveInfo.AsNoTracking()
                //                     where li.IsActive
                //                     group li by li.EmployeeId into g
                //                     select g.OrderByDescending(x => x.DateCreated).FirstOrDefault()                                    
                //                 ) on emp.EmployeeId equals leaveinfo.EmployeeId
                //                 select new LeaveCreditViewModel
                //                     {
                //                         EmployeeId = emp.EmployeeId,
                //                         VLBegBal = leaveinfo.VLBalance,
                //                         VLCredit = leaveinfo.VLCredit,
                //                         SLBegBal = leaveinfo.SLBalance,
                //                         SLCredit = leaveinfo.SLCredit
                //                     }).ToListAsync();
                var emplist = (from emp in _dbContext.EmployeeWorkDetails.AsNoTracking().ToList()
                               where emp.IsActive && !emp.IsResigned && emp.EmployeeStatusId != 3 && emp.EmployeeStatusId != 4
                               join leaveinfo in _dbContext.LeaveInfo.AsNoTracking()
                                   .Where(x => x.IsActive)
                                   .GroupBy(x => x.EmployeeId)
                                   .Select(g => g.OrderByDescending(x => x.DateCreated).First())
                                   .ToList()
                               on emp.EmployeeId equals leaveinfo.EmployeeId
                               select new LeaveCreditViewModel
                               {
                                   EmployeeId = emp.EmployeeId,
                                   VLBegBal = leaveinfo.VLBalance,
                                   VLCredit = leaveinfo.VLCredit,
                                   SLBegBal = leaveinfo.SLBalance,
                                   SLCredit = leaveinfo.SLCredit
                               }).ToList();


                var now = DateTime.Now;

                // Build all new entities in memory first
                var leaveCredits = emplist.SelectMany(emp => new[]
                {
                            new LeaveCredits
                            {
                                EmployeeId = emp.EmployeeId,
                                LeaveTypeId = (int)EnumLeaveType.VL,
                                BegBal = emp.VLBegBal,
                                Credit = emp.VLCredit,
                                DateCreated = now,
                                IsActive = true
                            },
                            new LeaveCredits
                            {
                                EmployeeId = emp.EmployeeId,
                                LeaveTypeId = (int)EnumLeaveType.SL, // fixed!
                                BegBal = emp.SLBegBal,
                                Credit = emp.SLCredit,
                                DateCreated = now,
                                IsActive = true
                            }
                }).ToList();
                // Bulk insert once
                await _dbContext.LeaveCredits.AddRangeAsync(leaveCredits);
                await _dbContext.SaveChangesAsync();


                var leaveInfosToUpdate = new List<LeaveInfo>();
                foreach (var emp in emplist)
                {
                    // Get the existing LeaveInfo record for this employee
                    var existingLeave = await _dbContext.LeaveInfo
                        .Where(x => x.EmployeeId == emp.EmployeeId)
                        .OrderByDescending(x => x.DateCreated)
                        .FirstOrDefaultAsync();

                    if (existingLeave != null)
                    {
                        // Update balances
                        existingLeave.VLBalance = (emp.VLBegBal ?? 0) + (emp.VLCredit ?? 0);
                        existingLeave.SLBalance = (emp.SLBegBal ?? 0) + (emp.SLCredit ?? 0);
                        existingLeave.DateCreated = DateTime.Now;
                        existingLeave.IsActive = true;

                        leaveInfosToUpdate.Add(existingLeave);
                    }
                }
                // Perform bulk update in one go
                _dbContext.LeaveInfo.UpdateRange(leaveInfosToUpdate);
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }
        }
    }
}
