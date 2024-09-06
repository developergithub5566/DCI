using DCI.Models.Configuration;
using DCI.Models.ViewModel;
using Newtonsoft.Json;

namespace DCI.WebApp.Configuration
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
			return null;
		}
	}
}
