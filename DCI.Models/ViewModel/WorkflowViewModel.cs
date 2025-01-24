namespace DCI.Models.ViewModel
{
	public class WorkflowViewModel
	{
		public int DocId { get; set; } = 0;	
		public string? DocNo { get; set; } = string.Empty;
		public string RequestBy { get; set; } = string.Empty;
		public string RequestByDatetime { get; set; } = string.Empty;
		public string ReviewedBy { get; set; } = string.Empty;
		public string ReviewedByDatetime { get; set; } = string.Empty;
		public string ApprovedBy { get; set; } = string.Empty;
		public string ApprovedByDatetime { get; set; } = string.Empty;
		public string CurrentStatus { get; set; } = string.Empty;
		public int StatusId { get; set; } = 0;
		public string ReviewedStatus { get; set; } = string.Empty;
		public string ApprovedStatus { get; set; } = string.Empty;
	}
}
