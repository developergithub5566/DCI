using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
	public class Department : IAuditable
	{
		public int DepartmentId { get; set; } = 0;

		public string DepartmentName { get; set; } = string.Empty;

		public string? Description { get; set; } = string.Empty;

		public DateTime DateCreated { get; set; } = DateTime.Now;

		public int CreatedBy { get; set; } = 0;

		public DateTime? DateModified { get; set; } = null;

		public int? ModifiedBy { get; set; } = null;

		public bool IsActive { get; set; } = true;

		public string? DepartmentCode { get; set; } = string.Empty;
		public int? Reviewer { get; set; } = 0;
		public int? Approver { get; set; } = 0;
	}
}
