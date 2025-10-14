
using DCI.Trigger;
using DCI.Trigger.API.DBContext;
using DCI.Trigger.API.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using DCI.Models.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
          options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerSource")));



//builder.Services.AddDbContext<DestinationDbContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServerB")));

//builder.Services.AddHangfire(x =>
//    x.UseSqlServerStorage(builder.Configuration.GetConnectionString("SqlServerSource")));

//builder.Services.AddHangfire((serviceProvider, config) =>
//{
//    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
//          .UseSimpleAssemblyNameTypeSerializer()
//          .UseRecommendedSerializerSettings()
//          .UseSqlServerStorage(
//              builder.Configuration.GetConnectionString("SqlServerA"),
//              new SqlServerStorageOptions
//              {
//                  CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
//                  SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
//                  QueuePollInterval = TimeSpan.FromSeconds(15),
//                  UseRecommendedIsolationLevel = true,
//                  DisableGlobalLocks = true
//              });
//});

builder.Services.AddScoped<ITriggerRepository, TriggerRepository>();


//builder.Services.AddTransient<OutboxProcessor>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
