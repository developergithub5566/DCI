using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DCI.Models.Entities
{

	public class User : IAuditable
	{
		[Key]
		public int UserId { get; set; }
		[Required(ErrorMessage = "Username is required")]
		[EmailAddress(ErrorMessage = "Invalid email address")]
		public string Email { get; set; } = string.Empty;
		public string Firstname { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;
		public string ContactNo { get; set; } = string.Empty;
		public int RoleId { get; set; }
		//public virtual Role RoleList { get; set; }
		public DateTime? DateCreated { get; set; } = DateTime.Now;
		public string CreatedBy { get; set; }
		public DateTime DateModified { get; set; }
		public string ModifiedBy { get; set; }
		public bool IsActive { get; set; }
		public virtual UserAccess UserAccess { get; set; }
		//public ICollection<Role> UserRolelist { get; set; }
	}
}
