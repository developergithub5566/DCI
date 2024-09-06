using DCI.WebApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Facebook;
using DCI.WebApp.Configuration;

namespace DCI.WebApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly UserSessionHelper _userSessionHelper;
		public HomeController(UserSessionHelper userSessionHelper)
		{
			this._userSessionHelper = userSessionHelper;
		}

		public IActionResult Index()
		{
			var currentUser = _userSessionHelper.GetCurrentUser();
			return View();
		}

		public IActionResult Privacy()
		{
			return View();
		}

		[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
		public IActionResult Error()
		{
			return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
		}

		[HttpPost]
		public async Task<IActionResult> Logout()
		{
			await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}
	
	}
}
