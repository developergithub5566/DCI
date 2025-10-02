using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class LateHeader : IAuditable
    {
        public int LateHeaderId { get; set; }
        public string? RequestNo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }

        // Navigation property
        public ICollection<LateDetail> Details { get; set; } = new List<LateDetail>();
    }
}
