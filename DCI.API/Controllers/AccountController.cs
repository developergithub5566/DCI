using DCI.Core.Helpers;
using DCI.Data;
using DCI.Models.Configuration;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Serilog;
using DCI.API.Service;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace DCI.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AccountController : Controller
	{
		private readonly DCIdbContext _dcIdbContext;
		private readonly AuthenticationModel _authenConfig;
		private readonly IUserContextService _userContextService;
		IUserRepository _userRepository;
		IUserAccessRepository _useraccessRepository;
		IEmailRepository _emailRepository;
		IRoleRepository _roleRepository;
		IDocumentTypeRepository _documentTypeRepository;

		private readonly UserManager<User> _userManager;
		private readonly IConfiguration _configuration;

		public AccountController(IUserRepository userRepository, IEmailRepository emailRepository, IUserAccessRepository useraccessRepository, AuthenticationModel authenConfig,
			IConfiguration configuration, IRoleRepository roleRepository, IUserContextService userContextService, IDocumentTypeRepository documentTypeRepository)
		{
			this._userRepository = userRepository;
			this._useraccessRepository = useraccessRepository;
			this._emailRepository = emailRepository;
			this._authenConfig = authenConfig;
			this._configuration = configuration;
			this._roleRepository = roleRepository;
			this._userContextService = userContextService;
			this._documentTypeRepository = documentTypeRepository;
		}

		[HttpPost]
		[Route("Registration")]
		public async Task<IActionResult> Registration([FromBody] UserViewModel model)
		{
			if (model.UserId == 0 && await _userRepository.IsExistsUsername(model.Email))
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Username is already exist");
			}
			if (model.UserId == 0 && await _emailRepository.IsExistsEmail(model.Email))
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Email address is already exist");
			}
			if (!await _roleRepository.IsExistsRole(model.RoleId))
			{
				return StatusCode(StatusCodes.Status403Forbidden, "Role Id does not exist");
			}

			var result = await _userRepository.Registration(model);

			await _emailRepository.SendSetPassword(model.Email); // Send email for password reset 

			return StatusCode(result.statuscode, result.message);
		}

		[HttpPost]
		[Route("PasswordReset")]
		public async Task<IActionResult> PasswordReset([FromBody] PasswordResetViewModel vm)
		{
			try
			{
				bool IsExists = await _emailRepository.IsExistsEmail(vm.Email);

				if (await _emailRepository.IsExistsEmail(vm.Email))
				{
					await _emailRepository.SendResetPassword(vm.Email);
					return Ok("Email sent successfully.");
				}
				else
				{
					return Ok("Email address does not exist");
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
			return BadRequest();
		}

		[HttpPost]
		[Route("ValidateToken")]
		public async Task<IActionResult> ValidateToken([FromBody] ValidateTokenViewModel model)
		{
			try
			{
				var result = await _useraccessRepository.ValidateToken(model.Token);
				return StatusCode(result.statuscode, result.message);
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
			return BadRequest();
		}		

		[HttpPost]
		[Route("Login")]
		public async Task<IActionResult> Login([FromBody] LoginViewModel loginvm)
		{
			try
			{
				var userEntity = await _userRepository.Login(loginvm.Email);

				if (userEntity == null)
				{
					return NotFound("Username not found");
				}

				if (PasswordHashingHelper.VerifyPassword(loginvm.Password, userEntity.Password))
				{
					var userContext = _userContextService.GetUserContext(loginvm.Email);
					return Ok(userContext.Result);
					//return Ok("Login Successful");
				}
				else
				{
					return NotFound("Incorrect password");
				}
				return NotFound("Invalid account");
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
			}
			Log.CloseAndFlush();
			return BadRequest("Invalid account");
		}



		[HttpPost]
		[Route("ChangePassword")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
		{
			try
			{
				if (!await _userRepository.IsExistsUsername(model.Email))
				{
					return StatusCode(StatusCodes.Status403Forbidden, "Username not found");
				}
				var result = await _useraccessRepository.ChangePassword(model);
				return StatusCode(result.statuscode, result.message);
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
			return BadRequest();
		}



		[HttpPost]
		[Route("SaveExternalUser")]
		public async Task<IActionResult> SaveExternalUser([FromBody] ExternalUserModel model)
		{
			var result = await _userRepository.SaveExternalUser(model);
			return StatusCode(result.statuscode, result.message);
		}
		[HttpPost]
		[Route("GetSaveExternalUser")]
		public async Task<IActionResult> GetSaveExternalUser([FromBody] ExternalUserModel model)
		{
			var result = await _userRepository.GetExternalUser(model);
			if (result.isExists)
			{
				var userContext = await _userContextService.GetUserContext(result.email);
				return Ok(userContext);
			}
			else
			{
				await _userRepository.SaveExternalUser(model);
				var userContext = await _userContextService.GetUserContext(model.Email);
				return Ok(userContext);
			}	
		}

	}
}
