using DCI.Data;
using DCI.Models.Configuration;
using DCI.PMS.Repository;
using DCI.PMS.Repository.Interface;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PMSdbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("DCIConnectionPMS")));
builder.Services.AddDbContext<DCIdbContext>(Options => Options.UseSqlServer(builder.Configuration.GetConnectionString("DCIConnectionESS")));

builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

//builder.Services.Configure<SMTPModel>(builder.Configuration.GetSection("SmtpSettings"));
//builder.Services.AddSingleton<AuthenticationModel>();
//builder.Services.Configure<AuthenticationModel>(builder.Configuration.GetSection("Authentication"));
//builder.Services.Configure<FileModel>(builder.Configuration.GetSection("DocumentStorage"));
builder.Services.Configure<APIConfigModel>(builder.Configuration.GetSection("ServiceUrls"));
builder.Services.Configure<FileModel>(builder.Configuration.GetSection("FileStorage"));

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
app.UseDeveloperExceptionPage();

app.Run();
