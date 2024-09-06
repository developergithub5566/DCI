using DCI.Models.Entities;
using DCI.Models.Configuration;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DCI.Data
{
	public class DCIdbContext : DbContext
	{
		private readonly string _username;
		public DCIdbContext(DbContextOptions<DCIdbContext> options) : base(options)
		{
			_username = "Admin";
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
		public DbSet<JobApplicant> JobApplicant { get; set; }
		public DbSet<Document> Document { get; set; }
		public DbSet<DocumentType> DocumentType { get; set; }
		

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<AuditLog>().Property(ae => ae.Changes).HasConversion(
				value => JsonConvert.SerializeObject(value),
				serializedValue => JsonConvert.DeserializeObject<Dictionary<string, object>>(serializedValue));	
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

				var auditLog = new AuditLog
				{
					ActionType = entry.State == EntityState.Added ? "INSERT" : entry.State == EntityState.Deleted ? "DELETE" : "UPDATE",
					EntityId = entry.Properties.Single(p => p.Metadata.IsPrimaryKey()).CurrentValue.ToString(),
					EntityName = entry.Metadata.ClrType.Name,
					Username = _username,
					TimeStamp = DateTime.UtcNow,
					Changes = entry.Properties.Select(p => new { p.Metadata.Name, p.CurrentValue }).ToDictionary(i => i.Name, i => i.CurrentValue),

					// TempProperties are properties that are only generated on save, e.g. ID's
					// These properties will be set correctly after the audited entity has been saved
					TempProperties = entry.Properties.Where(p => p.IsTemporary).ToList(),
				};

				entries.Add(auditLog);
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
