using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Models.ViewModel
{
	public class DocumentTypeViewModel
	{
		public int DocTypeId { get; set; } = 0;
		public string Name { get; set; } = string.Empty;
		public DateTime DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; }
		public DateTime? DateModified { get; set; } = null;
		public int? ModifiedBy { get; set; } = null;
		public bool IsActive { get; set; } = true;
		public string CreatedName { get; set; } = string.Empty;
		public string? Description { get; set; } = string.Empty;
	}
}
