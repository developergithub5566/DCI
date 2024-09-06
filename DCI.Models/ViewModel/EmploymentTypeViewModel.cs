namespace DCI.Models.ViewModel
{
	public class EmploymentTypeViewModel
	{
		public int EmploymentTypeId { get; set; }

		public string EmploymentTypeName { get; set; } = string.Empty;

		public string? Description { get; set; } = string.Empty;

		public DateTime DateCreated { get; set; } = DateTime.Now;

		public int CreatedBy { get; set; } = 0;

		public DateTime? DateModified { get; set; } = DateTime.Now;

		public int? ModifiedBy { get; set; } = 0;

		public bool IsActive { get; set; } = true;
	}
}
