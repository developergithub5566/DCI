using Microsoft.EntityFrameworkCore;

namespace DCI.Trigger
{
    public class PMSDbContext : DbContext
    {
        public PMSDbContext(DbContextOptions<PMSDbContext> options) : base(options) { }
        public DbSet<ProjectDto> ProjectDto { get; set; }

    }
}
