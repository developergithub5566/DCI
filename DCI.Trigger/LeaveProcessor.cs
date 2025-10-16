using DCI.Core.Common;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Serilog;


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
            Log.Information("Monthly Leave Credit started...");

            try
            {
                var emplist =
                      (from emp in _dbContext.EmployeeWorkDetails.AsNoTracking()
                       where emp.IsActive && !emp.IsResigned
                             && emp.EmployeeStatusId != 3 && emp.EmployeeStatusId != 4
                       join usr in _dbContext.User.AsNoTracking()
                           on emp.EmployeeId equals usr.EmployeeId

                       from leaveinfo in _dbContext.LeaveInfo.AsNoTracking()
                           .Where(li => li.IsActive && li.EmployeeId == emp.EmployeeId)
                           .OrderByDescending(li => li.DateCreated)

                           .Take(1)
                           .DefaultIfEmpty()
                       select new LeaveCreditViewModel
                       {
                           EmployeeId = emp.EmployeeId,
                           UserId = usr.UserId,
                           VLBegBal = leaveinfo != null ? leaveinfo.VLBalance : 0,
                           VLCredit = leaveinfo != null ? leaveinfo.VLCredit : 0,
                           SLBegBal = leaveinfo != null ? leaveinfo.SLBalance : 0,
                           SLCredit = leaveinfo != null ? leaveinfo.SLCredit : 0
                       })
                      .ToList();

                Log.Information("Total Employee Credited: " + emplist.Count().ToString());

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
                                LeaveTypeId = (int)EnumLeaveType.SL,
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
                var notifList = new List<Notification>();

                Log.Information("Processing leave credit allocation...");

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

                        //Send Application Notification to Approver
                        var notif = new Notification
                        {
                            Title = "Leave Credit",
                            Description = "Your leave credits have been updated today.",
                            ModuleId = (int)EnumModulePage.TriggerLeaveCreditProcess,
                            TransactionId = 1,
                            AssignId = emp.UserId,
                            URL = "/DailyTimeRecord/Leave",
                            MarkRead = false,
                            CreatedBy = 0,
                            IsActive = true,
                            DateCreated = DateTime.Now
                        };
                        notifList.Add(notif);

                        Log.Information("Employee Id:" + existingLeave.EmployeeId.ToString() + ","
                            + "VLBalance:" + emp.VLBegBal.ToString() + "," + "VL:" + emp.VLCredit.ToString() + ","
                            + "SLBalance:" + emp.SLBegBal.ToString() + "," + "SL:" + emp.SLCredit.ToString());
                    }
                }

                _dbContext.LeaveInfo.UpdateRange(leaveInfosToUpdate);
                await _dbContext.Notification.AddRangeAsync(notifList);
                await _dbContext.SaveChangesAsync();
                Log.Information("Monthly Leave Credit successfully completed.");
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
            }
        }
    }
}
