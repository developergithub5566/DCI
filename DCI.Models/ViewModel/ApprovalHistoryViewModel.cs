namespace DCI.Models.ViewModel
{
	public class ApprovalHistoryViewModel
	{
		public int ApprovalHistoryId { get; set; } = 0;
		public int DocId { get; set; } = 0;
		public string DocumentName { get; set; } = string.Empty;
		public bool Action { get; set; } = true;
		public int ApproverId { get; set; } = 0;
		public DateTime DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; } = 0;
		public string? Remarks { get; set; } = string.Empty;
		public bool IsActive { get; set; } = true;
		public string CurrentStatus { get; set; } = string.Empty;
	}

	public class ApprovalViewModel
	{
		public int RequestById { get; set; } = 0;
		public bool Action { get; set; } = true;
		public string ApprovalStatus = string.Empty;
	}
}
