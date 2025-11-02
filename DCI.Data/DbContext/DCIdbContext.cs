using DCI.Models.Entities;
using DCI.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Http;
using DCI.Models.ViewModel;

namespace DCI.Data
{
    public class DCIdbContext : DbContext
    {
        public DCIdbContext(DbContextOptions<DCIdbContext> options) : base(options)
        {

        }
        public DbSet<User> User { get; set; }
        public DbSet<UserAccess> UserAccess { get; set; }
        public DbSet<UserExternal> UserExternal { get; set; }
        public DbSet<ModulePage> ModulePage { get; set; }
        public DbSet<ModuleInRole> ModuleInRole { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<AuditLog> AuditLog { get; set; }
        public DbSet<Department> Department { get; set; }
        public DbSet<EmploymentType> EmploymentType { get; set; }
        public DbSet<Employee> Employee { get; set; }
        public DbSet<EmployeeWorkDetails> EmployeeWorkDetails { get; set; }
        public DbSet<EmployeeStatus> EmployeeStatus { get; set; }
        public DbSet<Status> Status { get; set; }
        public DbSet<vw_AttendanceSummary> vw_AttendanceSummary { get; set; }
        public DbSet<vw_AttendanceSummary_WFH> vw_AttendanceSummary_WFH { get; set; }
        public DbSet<ApprovalHistory> ApprovalHistory { get; set; }
        public DbSet<LeaveInfo> LeaveInfo { get; set; }
        public DbSet<LeaveType> LeaveType { get; set; }
        public DbSet<LeaveRequestHeader> LeaveRequestHeader { get; set; }
        public DbSet<LeaveRequestDetails> LeaveRequestDetails { get; set; }
        public DbSet<LeaveSummary> LeaveSummary { get; set; }
        public DbSet<Announcement> Announcement { get; set; }
        public DbSet<Notification> Notification { get; set; }
        public DbSet<DTRCorrection> DTRCorrection { get; set; }
        public DbSet<Position> Position { get; set; }
        public DbSet<tbl_raw_logs> tbl_raw_logs { get; set; }
        public DbSet<tbl_wfh_logs> tbl_wfh_logs { get; set; }
        public DbSet<OvertimeHeader> OvertimeHeader { get; set; }
        public DbSet<OvertimeDetail> OvertimeDetail { get; set; }
        public DbSet<Holiday> Holiday { get; set; }
        public DbSet<WfhHeader> WfhHeader { get; set; }
        public DbSet<WfhDetail> WfhDetail { get; set; }
        public DbSet<UndertimeHeader> UndertimeHeader { get; set; }
        public DbSet<UndertimeDetail> UndertimeDetail { get; set; }
        public DbSet<LateHeader> LateHeader { get; set; }
        public DbSet<LateDetail> LateDetail { get; set; }
        public DbSet<WorkLocation> WorkLocation { get; set; }
        public DbSet<LeaveCredits> LeaveCredits { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AuditLog>().Property(ae => ae.Changes).HasConversion(
                value => JsonConvert.SerializeObject(value),
                serializedValue => JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedValue));

            modelBuilder.Entity<LeaveSummary>().HasNoKey().ToView(null);

            modelBuilder.Entity<LeaveInfo>(entity =>
            {
                entity.Property(e => e.VLBalance)
                      .HasColumnType("decimal(7,4)");
                entity.Property(e => e.SLBalance)
                      .HasColumnType("decimal(7,4)");
                entity.Property(e => e.SPLBalance)
                     .HasColumnType("decimal(7,4)");
                entity.Property(e => e.VLYear)
                     .HasColumnType("decimal(7,4)");
                entity.Property(e => e.SLYear)
                      .HasColumnType("decimal(7,4)");
                entity.Property(e => e.VLCredit)
                     .HasColumnType("decimal(7,4)");
                entity.Property(e => e.SLCredit)
               .HasColumnType("decimal(7,4)");
            });
        }
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            // Get audit entries
            var auditEntries = OnBeforeSaveChanges();

            // Save current entity
            var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

            // Save audit entries
            await OnAfterSaveChangesAsync(auditEntries);
            return result;
        }

        private List<AuditLog> OnBeforeSaveChanges()
        {
            ChangeTracker.DetectChanges();
            var entries = new List<AuditLog>();

            foreach (var entry in ChangeTracker.Entries())
            {
                // Dot not audit entities that are not tracked, not changed, or not of type IAuditable
                if (entry.State == EntityState.Detached || entry.State == EntityState.Unchanged || !(entry.Entity is IAuditable))
                    continue;

                var _createdBy = string.Empty;
                var xmodule = entry.CurrentValues.EntityType.Name;
                try
                {
                    if (xmodule != "DCI.Models.Entities.UserAccess")
                    {
                        if (entry.State == EntityState.Added)
                        {
                            var createdByProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedBy");
                            if (createdByProperty != null && createdByProperty.CurrentValue != null)
                            {
                                _createdBy = createdByProperty.CurrentValue.ToString();
                            }
                            //_createdBy = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "CreatedBy").CurrentValue.ToString();
                        }
                        else
                        {
                            var modifiedByProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "ModifiedBy");
                            if (modifiedByProperty != null && modifiedByProperty.CurrentValue != null)
                            {
                                _createdBy = modifiedByProperty.CurrentValue.ToString();
                            }
                        }
                    }
                    if (xmodule == "DCI.Models.Entities.UserAccess")
                    {
                        var modifiedByProperty = entry.Properties.FirstOrDefault(p => p.Metadata.Name == "UserId");
                        if (modifiedByProperty != null && modifiedByProperty.CurrentValue != null)
                        {
                            _createdBy = modifiedByProperty.CurrentValue.ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _createdBy = "0";
                }
                finally
                {
                    var auditLog = new AuditLog
                    {
                        ActionType = entry.State == EntityState.Added ? "INSERT" : entry.State == EntityState.Deleted ? "DELETE" : "UPDATE",
                        EntityId = entry.Properties.Single(p => p.Metadata.IsPrimaryKey()).CurrentValue.ToString(),
                        EntityName = entry.Metadata.ClrType.Name,
                        //Username = _username,
                        Username = _createdBy,
                        TimeStamp = DateTime.Now, //DateTime.UtcNow,
                        Changes = entry.Properties.Select(p => new { p.Metadata.Name, p.CurrentValue }).ToDictionary(i => i.Name, i => i.CurrentValue),

                        // TempProperties are properties that are only generated on save, e.g. ID's
                        // These properties will be set correctly after the audited entity has been saved
                        TempProperties = entry.Properties.Where(p => p.IsTemporary).ToList(),
                    };

                    entries.Add(auditLog);
                }
            }

            return entries;
        }

        private Task OnAfterSaveChangesAsync(List<AuditLog> auditEntries)
        {
            if (auditEntries == null || auditEntries.Count == 0)
                return Task.CompletedTask;

            // For each temporary property in each audit entry - update the value in the audit entry to the actual (generated) value
            foreach (var entry in auditEntries)
            {
                foreach (var prop in entry.TempProperties)
                {
                    if (prop.Metadata.IsPrimaryKey())
                    {
                        entry.EntityId = prop.CurrentValue.ToString();
                        entry.Changes[prop.Metadata.Name] = prop.CurrentValue;
                    }
                    else
                    {
                        entry.Changes[prop.Metadata.Name] = prop.CurrentValue;
                    }
                }
            }

            AuditLog.AddRange(auditEntries);
            return SaveChangesAsync();
        }
    }
}
