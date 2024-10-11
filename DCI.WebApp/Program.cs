using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using DCI.Models.Configuration;
using Serilog;
using DCI.WebApp.Configuration;
using DCI.WebApp.Services;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

//using DCI.Repositories.Interface;
//using DCI.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json", optional: false);

builder.Services.AddControllersWithViews();

//builder.Services.AddScoped<IUserRepository, UserRepository>();

//builder.Services.AddSingleton<IUserContextService, UserContextService>();
//builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<UserManager>();

builder.Services.Configure<APIConfigModel>(builder.Configuration.GetSection("WebAPI"));

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	//options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
})
	  .AddCookie(options =>
	  {
		  options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
		  options.Cookie.HttpOnly = builder.Configuration.GetValue<bool>("Authentication:Cookie:HttpOnly");
		  options.Cookie.SecurePolicy = builder.Configuration.GetValue<CookieSecurePolicy>("Authentication:Cookie:SecurePolicy");
		  options.Cookie.SameSite = SameSiteMode.None; // // builder.Configuration.GetValue<SameSiteMode>("Authentication:Cookie:SameSite");
		  options.Cookie.MaxAge = TimeSpan.FromDays(builder.Configuration.GetValue<int>("Authentication:Cookie:MaxAge"));
		  options.SlidingExpiration = builder.Configuration.GetValue<bool>("Authentication:Cookie:SlidingExpiration");
		  options.ExpireTimeSpan = TimeSpan.FromDays(builder.Configuration.GetValue<int>("Authentication:Cookie:ExpireTimeSpan"));
		  options.LoginPath = builder.Configuration["Authentication:Cookie:LoginPath"];
		  options.LogoutPath = builder.Configuration["Authentication:Cookie:LogoutPath"];
		  options.AccessDeniedPath = builder.Configuration["Authentication:Cookie:AccessDeniedPath"];
	  });
//.AddGoogle(options =>
//{
// options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
// options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
// options.CallbackPath = new PathString("/signin-google");
// //options.CallbackPath = new PathString("/Account/Login");
//})
//.AddFacebook(options =>
//{
// options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
// options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
// options.CallbackPath = new PathString("/signin-facebook");
//});

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowAllOrigins",
		builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});


Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger();


builder.Services.AddHttpContextAccessor();
//builder.Services.AddSession();
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("Authentication:Session:IdleTimeout"));
	options.Cookie.HttpOnly = builder.Configuration.GetValue<bool>("Authentication:Session:HttpOnly");//true; 
	options.Cookie.IsEssential = builder.Configuration.GetValue<bool>("Authentication:Session:IsEssential");//true; 

});

builder.Services.AddScoped<UserSessionHelper>();
builder.Services.AddScoped<DocumentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//	app.UseExceptionHandler("/Home/Error");
//	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//	app.UseHsts();
//}

app.UseSession(); // Ensure session is used


app.UseHttpsRedirection();
app.UseStaticFiles();

//app.UseSession();

app.UseAuthentication(); // jc for testing oct7
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();
