using Microsoft.EntityFrameworkCore;

namespace DCI.Trigger
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<OutboxMessage> OutboxQueue { get; set; }
        public DbSet<TblRawLog> TblRawLogs { get; set; }
    }
}
