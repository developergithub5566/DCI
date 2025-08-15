using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class Holiday : IAuditable
    {
        public int HolidayId { get; set; }
        public DateTime HolidayDate { get; set; }
        public string HolidayName { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
