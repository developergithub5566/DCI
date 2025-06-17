using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class Announcement : IAuditable
    {
        public int AnnouncementId { get; set; }

        public string Title { get; set; }

        public string Date { get; set; }

        public string Details { get; set; }

        public int Status { get; set; }

        public DateTime DateCreated { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? DateModified { get; set; }

        public int? ModifiedBy { get; set; } 

        public bool IsActive { get; set; }
    }
}
