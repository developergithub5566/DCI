using DCI.PMS.Models.Entities;

namespace DCI.PMS.Models.ViewModel
{
    public class ClientViewModel
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
       
    }
}
