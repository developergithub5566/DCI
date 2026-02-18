using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using DCI.PMS.WebApp.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Security.Claims;
using System.Text;
using Serilog;

namespace DCI.PMS.WebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly IOptions<APIConfigModel> _apiconfig;        
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserSessionHelper _userSessionHelper;
        public AccountController(IOptions<APIConfigModel> apiconfig, IHttpContextAccessor httpContextAccessor, UserSessionHelper userSessionHelper)
        {
            this._apiconfig = apiconfig;        
            this._httpContextAccessor = httpContextAccessor;
            this._userSessionHelper = userSessionHelper;
        }

        [HttpGet]
        public IActionResult Login()
        {
            ViewBag.Message = null;
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            try
            {
                _httpContextAccessor.HttpContext!.Session.Clear();

                if (_httpContextAccessor.HttpContext.Request.Cookies.Count > 0)
                {
                    foreach (var cookie in _httpContextAccessor.HttpContext.Request.Cookies.Keys)
                    {
                        _httpContextAccessor.HttpContext.Response.Cookies.Delete(cookie);
                    }
                }

                await _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.ToString();
                //Log.Error(ex.ToString());
            }
            finally
            {
               // Log.CloseAndFlush();
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
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiESS + "api/Account/Login");

                    request.Content = stringContent;
                    var response = await Task.FromResult(clienthttp.Send(request));
                    string responseBody = await response.Content.ReadAsStringAsync();

                    if (response.IsSuccessStatusCode == true)
                    {
                    
                        UserManager um = JsonConvert.DeserializeObject<UserManager>(responseBody);
                        _httpContextAccessor.HttpContext!.Session.SetString("UserManager", JsonConvert.SerializeObject(um));

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.NameIdentifier, um.Identifier.ToString()),
                            new Claim("UserId", um.UserId.ToString()),
                            new Claim(ClaimTypes.Email, um.Email),
                            new Claim("FullName", um.GetFullname()),
                            new Claim(ClaimTypes.Name,um.GetFullname())
                        };

                        HttpContext.Session.SetString("currentFullname", um.GetFullname());
                       // HttpContext.Session.SetString("ModulePageAccess", JsonConvert.SerializeObject(um.ModulePageAccess));

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
              //  Log.Error(ex.ToString());
            }
            finally
            {
               // Log.CloseAndFlush();
            }
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
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiESS + "api/Account/PasswordReset");

                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);

                    if (response.IsSuccessStatusCode)
                    {
                        return Json(new { success = true, message = "Email sent successfully." });
                    }
                    else
                    {
                        return Json(new { success = false, message = "Email address does not exist." });
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
                        changemodel.Token = model.Token;
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
        public async Task<IActionResult> ChangePassword(ValidateTokenViewModel model)
        {
            try
            {
                using (var _httpclient = new HttpClient())
                {
                    var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                    var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiConnection + "api/Account/ValidateToken");

                    request.Content = stringContent;
                    var response = await _httpclient.SendAsync(request);
                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();
                        ViewBag.Email = responseBody;
                        return View();
                    }
                    ViewBag.Email = "";
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


        public async Task<IActionResult> HomeChangePassword(ChangePasswordViewModel model)
        {
            try
            {                                                                                                                                                                                            
                if (!model.IsResetPassword)
                {
                    var currentUser = _userSessionHelper.GetCurrentUser();
                    model.Email = currentUser?.Email != null ? currentUser.Email : model.Email;
                }

                if (model.NewPassword != "" && model.ConfirmPassword != "")
                {
                    using (var _httpclient = new HttpClient())
                    {
                        var stringContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
                        var request = new HttpRequestMessage(HttpMethod.Post, _apiconfig.Value.apiESS + "api/Account/ChangePassword");

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
                return Json(new { success = false, message = "An error occurred. Please try again." });
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
        public IActionResult ForgotPassword()
        {
            return View();
        }
    }
}
