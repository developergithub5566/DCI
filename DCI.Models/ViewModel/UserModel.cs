using DCI.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DCI.Models.ViewModel
{
	public class UserModel
	{
		public int UserId { get; set; } = 0;
		public string Username { get; set; } = string.Empty;
		public string Firstname { get; set; }
		public string Middlename { get; set; } 
		public string Lastname { get; set; }
		public string ContactNo { get; set; }
		public string Email { get; set; }
		public int RoleId { get; set; }
		public IList<Role>? RoleList { get; set; }
		public List<SelectListItem>? Options { get; set; }
		public IList<User>? EmployeeList { get; set; }
	}
}
