using DCI.Models.Configuration;

namespace DCI.Models.Entities 
{
    public class ModulePage : IAuditable
{
        public int ModulePageId { get; set; }

        public string ModuleName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

		public int CreatedBy { get; set; }

        public DateTime? DateModified { get; set; } 

		public int? ModifiedBy { get; set; }

		public bool IsActive { get; set; } = true;
	}
}
