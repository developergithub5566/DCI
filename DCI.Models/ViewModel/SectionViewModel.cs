using DCI.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DCI.Models.ViewModel
{
	public class SectionViewModel
	{
		public int SectionId { get; set; } 
		public string SectionCode { get; set; } = string.Empty;

		public string SectionName { get; set; } = string.Empty;
		public int DepartmentId { get; set; } = 0;
		public string DepartmentName { get; set; } = string.Empty;

		public string? Description { get; set; } = string.Empty;

		public DateTime DateCreated { get; set; } = DateTime.Now;

		public int CreatedBy { get; set; } = 0;

		public DateTime? DateModified { get; set; } = null;

		public int? ModifiedBy { get; set; } = null;

		public bool IsActive { get; set; } = true;
		public string CreatedName { get; set; } = string.Empty;
		public IList<Section>? SectionList { get; set; } 
		public List<SelectListItem>? OptionsSection { get; set; }
		public IList<Department>? DepartmentList { get; set; }
		public List<SelectListItem>? OptionsDepartment { get; set; }


	}
}
