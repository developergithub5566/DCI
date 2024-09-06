using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class Role : IAuditable
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;

		public int CreatedBy { get; set; }

        public DateTime? DateModified { get; set; }

        public int? ModifiedBy { get; set; }

		public bool IsActive { get; set; } = true;
	}
}
