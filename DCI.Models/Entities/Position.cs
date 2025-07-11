using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class Position : IAuditable
    {
        public int PositionId { get; set; }
        public string? PositionCode { get; set; }
        public string PositionName { get; set; }
        public string? Description { get; set; }
        public int? Level { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
