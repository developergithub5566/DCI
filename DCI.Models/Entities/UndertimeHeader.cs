using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class UndertimeHeader : IAuditable
    {
        public int UndertimeHeaderId { get; set; }
        public string? RequestNo { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public bool IsActive { get; set; }

        // Navigation property
        public ICollection<UndertimeDetail> Details { get; set; } = new List<UndertimeDetail>();
    }
}
