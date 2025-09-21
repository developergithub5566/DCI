using Microsoft.VisualBasic;

namespace DCI.Models.ViewModel
{
    public class PositionViewModel
    {
        public int PositionId { get; set; }
        public string? PositionCode { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public string CreatedName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int? Level { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; } = 0;
        public DateTime? DateModified { get; set; } = DateTime.Now;
        public int? ModifiedBy { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }
}
