using Microsoft.AspNetCore.Http;

namespace DCI.Models.ViewModel
{
	public class DocumentViewModel
	{
		public int DocId { get; set; }
		public string DocNo { get; set; } = string.Empty;
		public string DocName { get; set; } = string.Empty;
		public int DocTypeId { get; set; } = 0;
		public int Version { get; set; } = 0;
		public string Filename { get; set; } = string.Empty;
		public DateTime DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; } = 0;
		public DateTime? DateModified { get; set; } = null;
		public int? ModifiedBy { get; set; } = null;
		public bool IsActive { get; set; } = true;
		public IFormFile? DocFile { get; set; }
	}


}
