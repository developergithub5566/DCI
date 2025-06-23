using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class Notification : IAuditable
    {
        public int NotificationId { get; set; }

        public string? Title { get; set; }

        public string? Description { get; set; }

        public int ModuleId { get; set; }

        public int TransactionId { get; set; }

        public string? URL { get; set; }

        public bool MarkRead { get; set; }

        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }

        public bool IsActive { get; set; }

        public int AssignId { get; set; }

    }
}
