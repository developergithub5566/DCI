namespace DCI.Trigger.API.Model
{
    public class OutboxQueue
    {
        public int Id { get; set; }
        public string Payload { get; set; } = string.Empty;
        public string Status { get; set; } = "Pending";
        public int RetryCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? LastError { get; set; }
    }
}
