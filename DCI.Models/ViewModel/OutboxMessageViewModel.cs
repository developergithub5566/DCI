namespace DCI.Models.ViewModel
{
    public class OutboxMessageViewModel
    {
        public int Id { get; set; } = 0;
        public string Payload { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public int RetryCount { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string? LastError { get; set; } = string.Empty;
    }
}
