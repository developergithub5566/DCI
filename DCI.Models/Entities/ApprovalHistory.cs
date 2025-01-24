namespace DCI.Models.Entities
{
	public class ApprovalHistory
	{
		public int ApprovalHistoryId { get; set; } = 0;
		public int DocId { get; set; } = 0;
		public bool Action { get; set; }
		public int ApproverId { get; set; } = 0;
		public DateTime DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; } = 0;
		public string Remarks { get; set; } = string.Empty;
		public bool IsActive { get; set; } = true;
	}
}
