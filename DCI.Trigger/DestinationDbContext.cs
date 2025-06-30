using Microsoft.EntityFrameworkCore;

namespace DCI.Trigger
{
    public class DestinationDbContext : DbContext
    {
        public DestinationDbContext(DbContextOptions<DestinationDbContext> options) : base(options) { }

        public DbSet<TblRawLog> TblRawLogs { get; set; }
    }
}
