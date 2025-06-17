using DCI.Data;
using DCI.Repositories;
using DCI.Models;
using Microsoft.EntityFrameworkCore;
using Serilog;
using DCI.Models.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using DCI.Repositories.Interface;
using DCI.API.Service;
using System.Configuration;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<DCIdbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("DCIConnection")));
//builder.Services.AddDbContext<DCIdbContext>(options =>
//	options.UseSqlServer(
//		builder.Configuration.GetConnectionString("DCIConnection"),
//		sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
//			maxRetryCount: 5,      
//			maxRetryDelay: TimeSpan.FromSeconds(10), 
//			errorNumbersToAdd: null) 
//	));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserAccessRepository, UserAccessRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.AddScoped<IModuleInRoleRepository, ModuleInRoleRepository>();
builder.Services.AddScoped<IModulePageRepository, ModulePageRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
builder.Services.AddScoped<IEmploymentTypeRepository, EmploymentTypeRepository>();
builder.Services.AddScoped<IUserRoleRepository, UserRoleRepository>();
//builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();
//builder.Services.AddScoped<IDocumentTypeRepository, DocumentTypeRepository>();
//builder.Services.AddScoped<IDocumentRepository, DocumentRepository>();
//builder.Services.AddScoped<ISectionRepository, SectionRepository>();
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();
builder.Services.AddScoped<ITodoRepository, TodoRepository>();
//builder.Services.AddScoped<IRequestHistoryRepository, RequestHistoryRepository>();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IDailyTimeRecordRepository, DailyTimeRecordRepository>();
builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();
builder.Services.AddScoped<IAnnouncementRepository, AnnouncementRepository>();

builder.Services.Configure<SMTPModel>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddSingleton<AuthenticationModel>();
builder.Services.Configure<AuthenticationModel>(builder.Configuration.GetSection("Authentication"));
builder.Services.Configure<FileModel>(builder.Configuration.GetSection("DocumentStorage"));

builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddHttpContextAccessor();

builder.Services.Configure<APIConfigModel>(builder.Configuration.GetSection("WebAPI"));
//builder.Services.AddSingleton<IUserContextService, UserContextService>();


Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();

var app = builder.Build();

//app.UseUserContext();
//app.UseMiddleware<UserContextMiddleware>();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseDeveloperExceptionPage();

app.Run();
