using DCI.Trigger.API.Model;
using Microsoft.EntityFrameworkCore;

namespace DCI.Trigger.API.DBContext
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<OutboxQueue> OutboxQueue { get; set; }

    }
}
