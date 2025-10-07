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
        public int? EmployeeId { get; set; }
		public string? Firstname { get; set; } = string.Empty;
		public string? Middlename { get; set; } = string.Empty;
		public string? Lastname { get; set; } = string.Empty;
		public string? ContactNo { get; set; } = string.Empty;
		public int RoleId { get; set; }
		//public virtual Role RoleList { get; set; }
		public DateTime? DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; }
		public DateTime? DateModified { get; set; }
		public int? ModifiedBy { get; set; }
		public bool IsActive { get; set; }	
		public virtual UserAccess UserAccess { get; set; }
		public bool EmailBiometricsNotification { get; set; }
        //  public int DepartmentId { get; set; }
        //public ICollection<Role> UserRolelist { get; set; }
    }
}
