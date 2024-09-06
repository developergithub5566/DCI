using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
	public class Department : IAuditable
	{
		public int DepartmentId { get; set; }

		public string DepartmentName { get; set; } = string.Empty;

		public string? Description { get; set; }

		public DateTime DateCreated { get; set; } = DateTime.Now;

		public int CreatedBy { get; set; }

		public DateTime? DateModified { get; set; }

		public int? ModifiedBy { get; set; }

		public bool IsActive { get; set; } = true;
	}
}
