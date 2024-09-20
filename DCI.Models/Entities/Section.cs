using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
	public class Section : IAuditable
	{
		public int SectionId { get; set; } = 0;
		public string SectionCode { get; set; } = string.Empty;

		public string SectionName { get; set; } = string.Empty;
		public int DepartmentId { get; set; } = 0;

		public string? Description { get; set; } = string.Empty;

		public DateTime DateCreated { get; set; } = DateTime.Now;

		public int CreatedBy { get; set; } = 0;

		public DateTime? DateModified { get; set; } = null;

		public int? ModifiedBy { get; set; } = null;

		public bool IsActive { get; set; } = true;


	}
}
