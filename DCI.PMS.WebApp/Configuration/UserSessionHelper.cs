using DCI.Models.Configuration;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace DCI.PMS.WebApp.Configuration
{
    public class UserSessionHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserSessionHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public UserManager GetCurrentUser()
        {
            var userJson = _httpContextAccessor.HttpContext.Session.GetString("UserManager");

            if (userJson != null)
            {
                return JsonConvert.DeserializeObject<UserManager>(userJson);
            }

            _httpContextAccessor.HttpContext.Session.Clear();
            _httpContextAccessor.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return null;
        }
    }
}
