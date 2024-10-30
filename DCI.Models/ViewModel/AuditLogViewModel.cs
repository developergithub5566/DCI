namespace DCI.Models.ViewModel
{
	public class AuditLogViewModel
	{
		public long Id { get; set; } = 0;
		public string EntityName { get; set; } = string.Empty;
		public string ActionType { get; set; } = string.Empty;
		public string Username { get; set; } = string.Empty;
		public DateTime TimeStamp { get; set; } = DateTime.Now;
		public string EntityId { get; set; } = string.Empty;
		public Dictionary<string, object> Changes { get; set; } = new Dictionary<string, object>();
		public string ChangesSerialized { get; set; } = string.Empty;
		
	}
}
