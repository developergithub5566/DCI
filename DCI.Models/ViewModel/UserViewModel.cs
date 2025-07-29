namespace DCI.Models.ViewModel
{
	public class UserViewModel
	{
		public int UserId { get; set; } = 0;
        public int EmployeeId { get; set; } = 0;
        public string Firstname { get; set; } = string.Empty;
		public string? Middlename { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;
		public string? ContactNo { get; set; } = string.Empty;
		public string Email { get; set; } = string.Empty;
		public int RoleId { get; set; } = 0;
        public string RoleName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
		public DateTime DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; } = 0;
		public string CreatedName { get; set; } = string.Empty;
		public DateTime? DateModified { get; set; } = null;
		public int? ModifiedBy { get; set; } = null;
		public bool IsActive { get; set; } = true;	
		public int? IsAddEdit { get; set; } = 0;
    }
}
