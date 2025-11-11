using DCI.Models.Configuration;
using DCI.Trigger;
using Hangfire;
using Hangfire.Console;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

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
                      })
                  .UseConsole(); 
        });


        builder.Services.AddHangfireServer();
        builder.Services.Configure<APIConfigModel>(builder.Configuration.GetSection("WebAPI"));
        builder.Services.AddTransient<OutboxProcessor>();
        builder.Services.AddTransient<AttendanceProcessor>();
        builder.Services.Configure<SMTPModel>(builder.Configuration.GetSection("SmtpSettings"));
        builder.Services.AddScoped<IEmailRepository, EmailRepository>();
        Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

        var app = builder.Build();

        app.UseHangfireDashboard();

        var jobId = "attendance-confirmation-job";
        RecurringJob.RemoveIfExists(jobId);
        RecurringJob.AddOrUpdate<AttendanceProcessor>(
            jobId,
            p => p.AttendanceConfirmationProcessor(),
            "0 22 * * 1-5",
            TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")
        );


        RecurringJob.AddOrUpdate<OutboxProcessor>("outbox-job",
         processor => processor.ProcessPendingMessages(), Cron.Minutely);


        RecurringJob.AddOrUpdate<LeaveProcessor>("leave-credit-job",
         processor => processor.MonthlyLeaveCredit(), "0 5 1 * *", TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")); // Runs on the 1st day of every month at 5:00am


    

        //RecurringJob.RemoveIfExists("attendance-confirmation-job");
        //RecurringJob.AddOrUpdate<AttendanceProcessor>("attendance-confirmation-job",
        // processor => processor.AttendanceConfirmationProcessor(), "0 22 * * 1-5", TimeZoneInfo.FindSystemTimeZoneById("Asia/Manila")); // 10:00 PM, Monday–Friday


        app.MapGet("/", () => "SQL Outbox + Hangfire is running");

        app.Run();
    }
}
