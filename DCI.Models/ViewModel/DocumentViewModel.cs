using DCI.Models.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DCI.Models.ViewModel
{
	public class DocumentViewModel
	{
		public int DocId { get; set; }
		public string DocNo { get; set; } = string.Empty;
		public string DocName { get; set; } = string.Empty;
		public int DocTypeId { get; set; } = 0;
		public string DocTypeName { get; set; } = string.Empty;
		public int Version { get; set; } = 0;
		public string Filename { get; set; } = string.Empty;
		public string FileLocation { get; set; } = string.Empty;
		public DateTime DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; } = 0;
		public string CreatedName { get; set; } = string.Empty;
		public DateTime? DateModified { get; set; } = null;
		public int? ModifiedBy { get; set; } = null;
		public bool IsActive { get; set; } = true;
		public IFormFile? DocFile { get; set; }
		public List<SelectListItem>? Options { get; set; }
		public IList<DocumentType>? DocumentTypeList { get; set; }
		public IList<DocumentViewModel>? DocumentList { get; set; }
	}
	public class DocumentCodeViewModel
	{
	    public int DocumentCategory { get; set; } = 0; //internal or internal/external
		public int DepartmentId { get; set; } = 0;
		public int DocumentTypeId { get; set; } = 0;
		public int VersionNo { get; set; } = 0;
	}

}
