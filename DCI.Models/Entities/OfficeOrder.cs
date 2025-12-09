using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class OfficeOrder : IAuditable
    {
        public int OfficeOrderId { get; set; }
        public string? OrderName { get; set; }
        public DateTime OrderDate { get; set; }
        public string? Description { get; set; }
        public string? Filename { get; set; }
        public string? FilePath { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
