using DCI.Trigger;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerA")));

        builder.Services.AddDbContext<DestinationDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerB")));

        builder.Services.AddHangfire(x =>
            x.UseSqlServerStorage(builder.Configuration.GetConnectionString("SqlServerA")));

        builder.Services.AddHangfire((serviceProvider, config) =>
        {
            config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                  .UseSimpleAssemblyNameTypeSerializer()
                  .UseRecommendedSerializerSettings()
                  .UseSqlServerStorage(
                      builder.Configuration.GetConnectionString("SqlServerA"),
                      new SqlServerStorageOptions
                      {
                          CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                          SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                          QueuePollInterval = TimeSpan.FromSeconds(15),
                          UseRecommendedIsolationLevel = true,
                          DisableGlobalLocks = true
                      });
        });

        builder.Services.AddHangfireServer();

        builder.Services.AddTransient<OutboxProcessor>();

        var app = builder.Build();

        app.UseHangfireDashboard();

        //RecurringJob.AddOrUpdate<OutboxProcessor>("outbox-job",
        //    processor => processor.ProcessPendingMessages(), Cron.Minutely);

        //RecurringJob.AddOrUpdate<LeaveProcessor>("leave-credit-job",
        // processor => processor.MonthlyLeaveCredit(), "0 0 1 * *"); // Runs on the 1st day of every month

        RecurringJob.AddOrUpdate<LeaveProcessor>("leave-credit-job",
 processor => processor.MonthlyLeaveCredit(), Cron.Minutely); 

        app.MapGet("/", () => "SQL Outbox + Hangfire is running");

        app.Run();
    }
}
