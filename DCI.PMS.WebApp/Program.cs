using DCI.Models.Configuration;
using DCI.PMS.WebApp.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration.AddJsonFile("appsettings.json", optional: false);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<UserManager>();

builder.Services.Configure<APIConfigModel>(builder.Configuration.GetSection("ServiceUrls"));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
      .AddCookie(options =>
      {
          options.Cookie.Name = CookieAuthenticationDefaults.AuthenticationScheme;
          options.Cookie.HttpOnly = builder.Configuration.GetValue<bool>("Authentication:Cookie:HttpOnly");
          options.Cookie.SecurePolicy = builder.Configuration.GetValue<CookieSecurePolicy>("Authentication:Cookie:SecurePolicy");
          options.Cookie.SameSite = builder.Configuration.GetValue<SameSiteMode>("Authentication:Cookie:SameSite");
          options.Cookie.MaxAge = TimeSpan.FromDays(builder.Configuration.GetValue<int>("Authentication:Cookie:MaxAge"));
          options.SlidingExpiration = builder.Configuration.GetValue<bool>("Authentication:Cookie:SlidingExpiration");
          options.ExpireTimeSpan = TimeSpan.FromDays(builder.Configuration.GetValue<int>("Authentication:Cookie:ExpireTimeSpan"));
          options.LoginPath = builder.Configuration["Authentication:Cookie:LoginPath"];
          options.LogoutPath = builder.Configuration["Authentication:Cookie:LogoutPath"];
          options.AccessDeniedPath = builder.Configuration["Authentication:Cookie:AccessDeniedPath"];
      });

builder.Services.AddHttpContextAccessor();
//builder.Services.AddSession();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(builder.Configuration.GetValue<int>("Authentication:Session:IdleTimeout"));
    options.Cookie.HttpOnly = builder.Configuration.GetValue<bool>("Authentication:Session:HttpOnly");//true; 
    options.Cookie.IsEssential = builder.Configuration.GetValue<bool>("Authentication:Session:IsEssential");//true; 

});

builder.Services.AddSignalR();

builder.Services.AddScoped<UserSessionHelper>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials());
});



var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseSession();


app.UseHttpsRedirection();
app.UseStaticFiles();

//app.UseSession();
//app.UseMiddleware<EnsureUserSessionMiddleware>();  // 👈 here

app.UseAuthentication(); // jc for testing oct7
app.UseRouting();

app.UseAuthorization();

//app.MapControllerRoute(
//	name: "default",
//	pattern: "{controller=Account}/{action=Login}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Account}/{action=Login}/{id?}");
});

app.Run();
