using DCI.Models.Configuration;
using DCI.Models.Entities;
using DCI.Repositories.Interface;

namespace DCI.API.Service
{
	public class UserContextMiddleware
	{
		private readonly RequestDelegate _next;
		private readonly IUserContextService _userContextService;
		public UserContextMiddleware(RequestDelegate next, IUserContextService userContextService)
		{
			_next = next;
			_userContextService = userContextService;
		}

		public async Task InvokeAsync(HttpContext context)
		{
			var userContext = _userContextService.GetUserContext(string.Empty);
			context.Items["UserContext"] = userContext;
			await _next(context);
		}
	}
	public static class UserContextMiddlewareExtensions
	{
		public static IApplicationBuilder UseUserContext(this IApplicationBuilder builder)
		{
			return builder.UseMiddleware<UserContextMiddleware>();
		}
	}

	public interface IUserContextService
	{
		Task<UserManager> GetUserContext(string email);
	}

	public class UserContextService : IUserContextService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IUserRepository _userRepository;
		private readonly IModuleInRoleRepository _moduleInRoleRepository;

		public UserContextService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository, IModuleInRoleRepository moduleInRoleRepository)
		{
			_httpContextAccessor = httpContextAccessor;
			_userRepository = userRepository;
			_moduleInRoleRepository = moduleInRoleRepository;
		}

		public async Task<UserManager> GetUserContext(string email)
		{
			var usermodel = new User();
			var usermanager = new UserManager();

            var user = _httpContextAccessor.HttpContext.User;
			if (user == null || !user.Identity.IsAuthenticated)
			{
              //  usermodel = await _userRepository.GetUserByEmail(email);
                usermanager = await _userRepository.GetUserManagerByEmail(email);

            }

		//	var moduleList = await _moduleInRoleRepository.GetModuleInRoleByRoleId(usermodel.RoleId); // need  to remove
			var modulepageAccess = await _moduleInRoleRepository.GetModuleAccessByRoleId(usermanager.RoleId);
			usermanager.ModulePageAccess = modulepageAccess;

			return usermanager;
            //return new UserManager
            //{
            //	UserId = usermodel.UserId,
            //	Email = usermodel.Email,
            //	Lastname = usermodel.Lastname,
            //	Middlename = usermodel.Middlename,
            //	Firstname = usermodel.Firstname,
            //	RoleId = usermodel.RoleId,
            //             EmployeeId = usermodel.EmployeeId ?? 0,
            //             ModulePageList = null,  //remove ModulePageList
            //	ModulePageAccess = modulepageAccess,
            //};	
        }
	}
}
