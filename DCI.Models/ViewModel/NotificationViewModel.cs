
namespace DCI.Models.ViewModel
{
    public class NotificationViewModel
    {
        public int NotificationId { get; set; } = 0;

        public string? Title { get; set; } = string.Empty;

        public string? Description { get; set; } = string.Empty;

        public int ModuleId { get; set; } = 0;

        public int TransactionId { get; set; } = 0;

        public string? URL { get; set; } = string.Empty;

        public bool MarkRead { get; set; } = false;

        public bool IsActive { get; set; } = true;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; } = 0;
        public int AssignId { get; set; } = 0;
    }
}
