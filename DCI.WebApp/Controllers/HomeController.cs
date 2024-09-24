using DCI.WebApp.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.Facebook;
using DCI.WebApp.Configuration;
using DCI.Models.ViewModel;
using Newtonsoft.Json;
using DCI.Models.Configuration;
using Microsoft.Extensions.Options;


namespace DCI.WebApp.Controllers
{
	public class HomeController : Controller
	{
		private readonly IOptions<APIConfigModel> _apiconfig;
		private readonly UserSessionHelper _userSessionHelper;

		public HomeController(IOptions<APIConfigModel> apiconfig, UserSessionHelper userSessionHelper)
		{
			this._apiconfig = apiconfig;
			_userSessionHelper = userSessionHelper;
		}

		public async Task<IActionResult> Index()
		{
			//var currentUser = _userSessionHelper.GetCurrentUser();

			List<HomePageViewModel> model = new List<HomePageViewModel>();

			using (var _httpclient = new HttpClient())
			{
				HttpResponseMessage response = await _httpclient.GetAsync(_apiconfig.Value.apiConnection + "api/Document/HomePage");
				string responseBody = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode)
				{
					model = JsonConvert.DeserializeObject<List<HomePageViewModel>>(responseBody)!;
				}
			}
			return View(model);
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

		public async Task<IActionResult> Profile()
		{
			var model = _userSessionHelper.GetCurrentUser();
			return View(model);
		}

	}
}
