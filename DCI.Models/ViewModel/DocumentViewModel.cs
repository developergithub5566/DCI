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
		public IList<DocumentViewModel>? DocumentList { get; set; }

		public IList<DocumentType>? DocumentTypeList { get; set; }
		public List<SelectListItem>? OptionsDocumentType { get; set; }

		public IList<Department>? DepartmentList { get; set; }
		public List<SelectListItem>? OptionsDepartment { get; set; }

		public int? DocCategory { get; set; } = 0; //internal or internal/external 
		public int Section { get; set; } = 0; 
		public int StatusId { get; set; } = 0;
        public int Reviewer { get; set; } = 0;
        public int Approver { get; set; } = 0;
		public int DepartmentId { get; set; } = 0;
	
	}
	public class DocumentCodeViewModel
	{
	    public int DocumentCategory { get; set; } = 0; //internal or internal/external
		public int DepartmentId { get; set; } = 0;
		public int DocumentTypeId { get; set; } = 0;
		public int VersionNo { get; set; } = 0;
	}

}
