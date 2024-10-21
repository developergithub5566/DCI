using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using DCI.Models.Configuration;
using Newtonsoft.Json;
using System.Text;
using DCI.Models.ViewModel;
using Serilog;
using Microsoft.AspNetCore.Http;
using static System.Net.WebRequestMethods;
using Microsoft.AspNetCore.Authorization;
using DCI.Models.Entities;
using System.Security.Claims;
using DCI.WebApp.Configuration;



namespace DCI.WebApp.Controllers
{
	public class AccountController : Controller
	{
		private readonly IOptions<APIConfigModel> _apiconfig;
		//private readonly IUserContextService _userContextService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly UserSessionHelper _userSessionHelper;
		public AccountController(IOptions<APIConfigModel> apiconfig, IHttpContextAccessor httpContextAccessor, UserSessionHelper userSessionHelper)
		{
			this._apiconfig = apiconfig;
			//this._userContextService = userContextService;
			this._httpContextAccessor = httpContextAccessor;
			this._userSessionHelper = userSessionHelper;
		}

		[HttpGet]
		public IActionResult Login()
		{
			ViewBag.Message = null;
			return View();
		}

		public async Task<IActionResult> Logoutx()
		{
			try
			{


				//await HttpContext.SignOutAsync("Cookies");
				//await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				//await HttpContext.SignOutAsync(GoogleDefaults.AuthenticationScheme);
				//HttpContext.Session.Clear();
				// Optionally, clear any other cookies

				//  _httpContextAccessor.HttpContext.Session.Clear();
				//Log.Information("Logout-");

				//Log.Information("httpcontext-" + HttpContext.User.Identity.Name.ToString());

				//Log.Information("HttpContext.Session-" + HttpContext.Session.IsAvailable);
				//if (HttpContext.Session.IsAvailable)
				//{

				//	HttpContext.Session.Clear();
				//}
				//Log.Information("Request.Cookies-" + Request.Cookies.Count().ToString());
				//if (Request.Cookies.Count > 0)
				//{

				//	foreach (var cookie in HttpContext.Request.Cookies.Keys)
				//	{
				//		Response.Cookies.Delete(cookie);
				//	}
				//}


				_httpContextAccessor.HttpContext.Session.Clear();




				if (_httpContextAccessor.HttpContext.Request.Cookies.Count > 0)
				{

					foreach (var cookie in _httpContextAccessor.HttpContext.Request.Cookies.Keys)
					{
						_httpContextAccessor.HttpContext.Response.Cookies.Delete(cookie);
					}
				}

				await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
				await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);


				return RedirectToAction("Login", "Account");
			}
			catch (Exception ex)
			{
				ViewBag.Message = ex.ToString();
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
			return RedirectToAction("Login", "Account");
		}

		public async Task<IActionResult> Login(LoginViewModel loginvm)
		{
			try
			{
				using (var clienthttp = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(loginvm), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Account/Login");

					request.Content = stringContent;
					var response = await Task.FromResult(clienthttp.Send(request));
					string responseBody = await response.Content.ReadAsStringAsync();

					if (response.IsSuccessStatusCode == true)
					{
						//var result = _userContextService.GetUserContext();
						UserManager um = JsonConvert.DeserializeObject<UserManager>(responseBody);
						_httpContextAccessor.HttpContext.Session.SetString("UserManager", JsonConvert.SerializeObject(um));

						//var _fullname = um.GetFullname(); //$"{um.Firstname} {um.Lastname}";

						var claims = new List<Claim>
						{
							new Claim(ClaimTypes.NameIdentifier, um.Identifier.ToString()),
							new Claim("UserId", um.UserId.ToString()),
							new Claim(ClaimTypes.Email, um.Email),
							new Claim("FullName", um.GetFullname()),
							new Claim(ClaimTypes.Name,um.GetFullname())
						};

						HttpContext.Session.SetString("ModuleList", JsonConvert.SerializeObject(um.ModulePageList));
						HttpContext.Session.SetString("currentFullname", um.GetFullname());

						var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
						await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

						return RedirectToAction("Index", "Home");
					}
					else
					{

						ViewBag.Message = "Invalid account! Please check your username and password.";
						return View();
					}
				}
			}
			catch (Exception ex)
			{
				ViewBag.Message = ex.ToString();
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
			return View();
		}


		// [HttpGet("ForgotPassword")]
		public IActionResult ForgotPassword()
		{
			return View();
		}
		public async Task<IActionResult> PasswordReset(string email)
		{
			PasswordResetViewModel model = new PasswordResetViewModel();
			model.Email = email;
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Account/PasswordReset");

					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);

					if (response.IsSuccessStatusCode)
					{
						return Json(new { success = true, message = "Email sent successfully." });
					}
				}
				return Json(new { success = false, message = "An error occurred. Please try again." });
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return View();
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}
		public async Task<IActionResult> VerifyToken()
		{
			return View();
		}


		public async Task<IActionResult> ValidateToken(ValidateTokenViewModel model)
		{
			try
			{
				using (var _httpclient = new HttpClient())
				{
					var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
					var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Account/ValidateToken");

					request.Content = stringContent;
					var response = await _httpclient.SendAsync(request);
					string responseBody = await response.Content.ReadAsStringAsync();
					if (response.IsSuccessStatusCode)
					{
						ChangePasswordViewModel changemodel = new ChangePasswordViewModel();
						changemodel.Email = responseBody;
						return RedirectToAction("ChangePassword", changemodel);
					}
					return RedirectToAction("VerifyToken");
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return View();
			}
			finally
			{
				Log.CloseAndFlush();
			}

		}

		public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
		{
			try
			{
				var currentUser = _userSessionHelper.GetCurrentUser();
				model.Email = currentUser?.Email != null ? currentUser.Email : model.Email;

				if (model.NewPassword != "" && model.ConfirmPassword != "")
				{
					using (var _httpclient = new HttpClient())
					{
						var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
						var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Account/ChangePassword");

						request.Content = stringContent;
						var response = await _httpclient.SendAsync(request);
						string responseBody = await response.Content.ReadAsStringAsync();
						if (response.IsSuccessStatusCode)
						{
							return Json(new { success = true, message = responseBody });
						}
						return Json(new { success = false, message = responseBody });
					}
				}
				return View();
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return Json(new { success = false, message = ex.Message });
			}
			finally
			{
				Log.CloseAndFlush();
			}

		}


		[HttpGet]
		public IActionResult GoogleLogin()
		{
			var redirectUrl = Url.Action(nameof(GoogleResponse));
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, GoogleDefaults.AuthenticationScheme);
		}

		[HttpGet]
		public async Task<IActionResult> GoogleResponse()
		{
			var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			if (!result.Succeeded) return BadRequest();

			var externalClaims = result.Principal.Identities
								.FirstOrDefault()?.Claims
								.Select(claim => new
								{
									claim.Type,
									claim.Value
								});

			var externalUser = new ExternalUserModel
			{
				Identifier = externalClaims?.FirstOrDefault(c => c.Type.Contains("nameidentifier"))?.Value ?? string.Empty,
				Firstname = externalClaims?.FirstOrDefault(c => c.Type.Contains("givenname"))?.Value ?? string.Empty,
				Lastname = externalClaims?.FirstOrDefault(c => c.Type.Contains("surname"))?.Value ?? string.Empty,
				Email = externalClaims?.FirstOrDefault(c => c.Type.Contains("emailaddress"))?.Value ?? string.Empty,
				Provider = "Google"
			};

			using (var clienthttp = new HttpClient())
			{
				var stringContent = new StringContent(JsonConvert.SerializeObject(externalUser), Encoding.UTF8, "application/json");
				var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Account/GetSaveExternalUser");

				request.Content = stringContent;
				var response = await Task.FromResult(clienthttp.Send(request));
				string responseBody = await response.Content.ReadAsStringAsync();

				if (response.IsSuccessStatusCode == true)
				{
					//var result = _userContextService.GetUserContext();
					UserManager um = JsonConvert.DeserializeObject<UserManager>(responseBody);
					_httpContextAccessor.HttpContext.Session.SetString("UserManager", JsonConvert.SerializeObject(um));

					var claims = new List<Claim>
						{
							new Claim(ClaimTypes.NameIdentifier, um.Identifier.ToString()),
							new Claim("UserId", um.UserId.ToString()),
							new Claim(ClaimTypes.Email, um.Email),
							new Claim("FullName", $"{um.Firstname} {um.Lastname}")
						};

					// Create a ClaimsIdentity and ClaimsPrincipal
					var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
					var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

					// Sign in the user
					await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

					return RedirectToAction("Index", "Home");
				}
				else
				{
					return View();
				}

				//return Ok(claims);
			}
		}

		[HttpGet("FacebookLogin")]
		public IActionResult FacebookLogin()
		{
			var redirectUrl = Url.Action(nameof(FacebookResponse));
			var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
			return Challenge(properties, FacebookDefaults.AuthenticationScheme);
		}

		[HttpGet("FacebookResponse")]
		public async Task<IActionResult> FacebookResponse()
		{
			var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
			if (!result.Succeeded) return BadRequest();

			var claims = result.Principal.Identities
								.FirstOrDefault()?.Claims
								.Select(claim => new
								{
									claim.Type,
									claim.Value
								});


			return Ok(claims);
		}


		//public async Task<IActionResult> Registration(RegistrationViewModel model)
		//{
		//    try
		//    {
		//        using (var _httpclient = new HttpClient())
		//        {
		//            var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
		//            var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Account/Registration");

		//            request.Content = stringContent;
		//            var response = await _httpclient.SendAsync(request);

		//            if (response.IsSuccessStatusCode)
		//            {
		//                return Json(new { success = true, message = "User successfully created." });
		//            }
		//        }
		//        return Json(new { success = false, message = "An error occurred. Please try again." });
		//    }
		//    catch (Exception ex)
		//    {
		//        Log.Error(ex.ToString());
		//        return Json(new { success = false, message = ex.Message });
		//    }
		//    finally
		//    {
		//        Log.CloseAndFlush();
		//    }
		//}


	}
}
