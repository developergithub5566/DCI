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
        public int Percentage { get; set; } = 0;
        public string ReviewedRemarks { get; set; } = string.Empty;
        public string ApprovedRemarks { get; set; } = string.Empty;
    }
    public class ApprovalHistoryDetailViewmodel
    {
        public int DocId { get; set; } = 0;
        public int? ApproverId { get; set; } = 0;
        public bool? Action { get; set; }
        public string Approver { get; set; }
        public DateTime DateCreated { get; set; }
        public string Remarks { get; set; } = string.Empty;

       
    }
    public class ApprovalHistoryHeaderViewmodel
    {
        public string RequestBy { get; set; } = string.Empty;
        public DateTime RequestByDatetime { get; set; }
        public int Percentage { get; set; } = 0;
        public string CurrentStatus { get; set; } = string.Empty;
        public int StatusId { get; set; } = 0;
        public int Reviewer { get; set; } = 0;
        public int Approver { get; set; } = 0;
        public List<ApprovalHistoryDetailViewmodel> ApprovalDetails { get; set; }
    }
}
