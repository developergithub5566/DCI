using DCI.Models.Configuration;

namespace DCI.PMS.Models.Entities
{
    public class Status : IAuditable
    {
        public int StatusId { get; set; } = 0;
        public string StatusName { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; } = 0;

        public DateTime? DateModified { get; set; } = null;

        public int? ModifiedBy { get; set; } = null;

        public bool IsActive { get; set; } = true;
    }
}
