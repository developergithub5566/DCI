using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class WorkLocation : IAuditable
    {
        public int WorkLocationId { get; set; }
        public string? Code { get; set; }
        public string Location { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
