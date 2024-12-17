using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DCI.Models.Entities
{
	public class Document : IAuditable
	{
		[Key]
		public int DocId { get; set; } = 0;
		public string DocNo { get; set; } = string.Empty;
		public string DocName { get; set; } = string.Empty;
		public int DocTypeId { get; set; } = 0;
		public int Version { get; set; } = 0;
		public string Filename { get; set; } = string.Empty;
		public string FileLocation { get; set; } = string.Empty;
		public DateTime DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; } = 0;
		public DateTime? DateModified { get; set; } = null;
		public int? ModifiedBy { get; set; } = null;
		public bool IsActive { get; set; } = true;
		public int StatusId { get; set; } = 0;
		public int Reviewer { get; set; } = 0;
		public int Approver { get; set; } = 0;
		public int? RequestById { get; set; } = 0;
		public int? SectionId { get; set; } = 0;
		public int DepartmentId { get; set; } = 0;
		public int? DocCategory { get; set; } = 0;
		public string? UploadLink { get; set; } = string.Empty;
		public int? LabelId { get; set; } = 0;
	}
}
