using DCI.Models.Configuration;

namespace DCI.PMS.Models.Entities
{
    public class Client : IAuditable
    {
        public int ClientId { get; set; }

        public string ClientName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }

        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        // Navigation Property
        //public ICollection<Project>? Projects { get; set; }
    }
}
