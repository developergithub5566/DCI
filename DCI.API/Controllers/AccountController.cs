using DCI.Core.Helpers;
using DCI.Core.Common;
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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


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
		//IDocumentTypeRepository _documentTypeRepository;

		private readonly UserManager<User> _userManager;
		private readonly IConfiguration _configuration;

		public AccountController(IUserRepository userRepository, IEmailRepository emailRepository, IUserAccessRepository useraccessRepository, AuthenticationModel authenConfig,
			IConfiguration configuration, IRoleRepository roleRepository, IUserContextService userContextService)
		{
			this._userRepository = userRepository;
			this._useraccessRepository = useraccessRepository;
			this._emailRepository = emailRepository;
			this._authenConfig = authenConfig;
			this._configuration = configuration;
			this._roleRepository = roleRepository;
			this._userContextService = userContextService;
			//this._documentTypeRepository = documentTypeRepository;
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

			await _emailRepository.SendSetPassword(model); // Send email for password reset 

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
					var token = GenerateJwtToken(loginvm.Email);
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

		private string GenerateJwtToken(string username)
		{
			var claims = new[]
			{
			new Claim(JwtRegisteredClaimNames.Sub, username),
			new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
		};
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("JWTAuthenticationHIGHsecuredPasswordVVVp1OH7Xzyr"));
			//var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("your_super_secret_key"));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: "yourdomain.com",
				audience: "yourdomain.com",
				claims: claims,
				expires: DateTime.Now.AddMinutes(30),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}

		[HttpPost]
		[Route("ChangePassword")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordViewModel model)
		{
			try
			{
				var inValidPass = await PasswordValidation(model);

				if (inValidPass.isValidPass)
				{
					return StatusCode(inValidPass.statuscode, inValidPass.message);
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


		private async Task<(bool isValidPass, int statuscode, string message)> PasswordValidation(ChangePasswordViewModel pass)
		{
			
			if (!await _userRepository.IsExistsUsername(pass.Email))
			{			
				return (true, StatusCodes.Status403Forbidden, "Username not found.");
			}

			if (pass.NewPassword != pass.ConfirmPassword)
			{
				//return StatusCode(StatusCodes.Status401Unauthorized, "Passwords do not match.");
				return (true, StatusCodes.Status401Unauthorized, "Passwords do not match.");
			}

			if (pass.NewPassword.Contains(" "))
			{
				//return StatusCode(StatusCodes.Status403Forbidden, "Password cannot contain space");
				return (true, StatusCodes.Status403Forbidden, "Password cannot contain space.");
			}

			if (Utilities.IsMinCharacter(8, pass.NewPassword.ToString()))
			{
				return (true, StatusCodes.Status403Forbidden, "Password cannot be less then 8 characters.");
			}

			if (Utilities.IsMaxCharacter(32, pass.NewPassword.ToString()))
			{
				return (true, StatusCodes.Status403Forbidden, "Password cannot be more then 32 characters.");
			}

			if (!Utilities.IsContainsNumber(pass.NewPassword.ToString()))
			{
				return (true, StatusCodes.Status403Forbidden, "Password must contain number.");
			}

			if (!Utilities.IsContainsLowerCase(pass.NewPassword.ToString()))
			{
				return (true, StatusCodes.Status403Forbidden, "Password must contain lowercase character.");
			}

			if (!Utilities.IsContainsUpperCase(pass.NewPassword.ToString()))
			{
				return (true, StatusCodes.Status403Forbidden, "Password must contain uppercase character.");
			}			

			string specialCharacterString = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
			HashSet<char> specialCharacters = specialCharacterString.ToCharArray().ToHashSet();

		
			char[] charArray = pass.NewPassword.ToCharArray();

			foreach (char charpass in charArray)
			{
				if (specialCharacters.Contains(charpass))
				{
					return (false, StatusCodes.Status100Continue, string.Empty);
				}
				
			}	
			return (true, StatusCodes.Status403Forbidden, "Password must contain special character." );
		}
	}
}
