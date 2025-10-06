using DCI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DCI.Trigger
{
    public class DestinationDbContext : DbContext
    {
        public DestinationDbContext(DbContextOptions<DestinationDbContext> options) : base(options) { }

        public DbSet<TblRawLog> TblRawLogs { get; set; }

        public DbSet<LeaveCredits> LeaveCredits { get; set; }
        public DbSet<LeaveInfo> LeaveInfo { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeeWorkDetails> EmployeeWorkDetails { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<User> User { get; set; }
    }
}
