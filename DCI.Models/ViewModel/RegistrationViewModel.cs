using DCI.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DCI.Models.ViewModel
{
	//public class RegistrationViewModel
	//{
	//	public int UserId { get; set; } = 0;
	//	public string Email { get; set; } = string.Empty;
	//	public string Firstname { get; set; } = string.Empty;
	//	public string Lastname { get; set; } = string.Empty;
	//	public string ContactNo { get; set; } = string.Empty;
	//	public int RoleId { get; set; } = 0;
	//	public string Password { get; set; } = string.Empty;
	//}

	public class LoginViewModel
	{
		public int UserId { get; set; } = 0;
		public int UserAccessId { get; set; } = 0;
		public string Email { get; set; } = string.Empty;
		public string Password { get; set; } = string.Empty;
	}
	public class PasswordResetViewModel
	{
		public string Email { get; set; }
	}
		
	public class ValidatePasswordTokenViewModel
	{
		public string Token { get; set; }
	}

	public class UserViewModel
	{
		public int UserId { get; set; } = 0;
		public string Firstname { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;
		public string ContactNo { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public int RoleId { get; set; } = 0;
		public string Password { get; set; } = string.Empty;
		public DateTime DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; } = 0;
		public string CreatedName { get; set; } = string.Empty;
		public DateTime? DateModified { get; set; } = null;
		public int? ModifiedBy { get; set; } = null;
		public bool IsActive { get; set; } = true;
	}
	//public class UpdateUserViewModel
	//{
	//	public int UserId { get; set; } = 0;
	//	public string Username { get; set; } = string.Empty;
	//	public string Firstname { get; set; }
	//	public string Lastname { get; set; }
	//	public string ContactNo { get; set; }
	//	public string Email { get; set; }
	//	public int RoleId { get; set; }
	//}
	public class UserModel
	{
		public int UserId { get; set; } = 0;
		public string Username { get; set; } = string.Empty;
		public string Firstname { get; set; }
		public string Lastname { get; set; }
		public string ContactNo { get; set; }
		public string Email { get; set; }
		public int RoleId { get; set; }
		public IList<Role>? RoleList { get; set; }
		public List<SelectListItem>? Options { get; set; }
		public IList<User>? EmployeeList { get; set; }
	}
}