namespace DCI.Models.ViewModel
{
    public class AnnouncementViewModel
    {
        public int AnnouncementId { get; set; } = 0;

        public string Title { get; set; } = string.Empty;

        public string Date { get; set; } = string.Empty;

        public string Details { get; set; } = string.Empty;

        public int Status { get; set; } = 0;

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public int CreatedBy { get; set; } = 0;

        public DateTime? DateModified { get; set; }

        public int? ModifiedBy { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public string CreatedName { get; set; } = string.Empty;
    }
}
