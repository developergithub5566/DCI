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
		public string? Fullname { get; set; } = string.Empty;
		public string? EmployeeNo { get; set; } = string.Empty;		
		public int RoleId { get; set; }
        public bool EmailBiometricsNotification { get; set; }
        public DateTime? DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; }
		public DateTime? DateModified { get; set; }
		public int? ModifiedBy { get; set; }
		public bool IsActive { get; set; }	
		public virtual UserAccess UserAccess { get; set; }
        public bool EmailAttendanceConfirmation { get; set; }
    }
}
