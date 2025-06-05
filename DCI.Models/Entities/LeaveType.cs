using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class LeaveType : IAuditable
    {
        public int LeaveTypeId { get; set; }
        public string? Description { get; set; }
        public decimal? Amount { get; set; }
        public DateTime DateCreated { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public DateTime? DateModified { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
