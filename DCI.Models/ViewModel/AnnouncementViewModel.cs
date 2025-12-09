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

    public class BirthdayViewModel
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string Birthdate { get; set; } = string.Empty;
        public DateTime? Birthday { get; set; } = null;
    }

    public class DashboardViewModel
    {
        public int CurrentUserId { get; set; } = 0;
        public string FIRST_IN { get; set; } = string.Empty;
        public string LAST_OUT { get; set; } = string.Empty;
        public List<AnnouncementViewModel> AnnouncementList { get; set; } = new();
        public List<BirthdayViewModel> BirthdayList { get; set; } = new();
        public List<OfficeOrderViewModel> OfficeOrderList { get; set; } = new();
    }
}
