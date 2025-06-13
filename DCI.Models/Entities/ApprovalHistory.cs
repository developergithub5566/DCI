namespace DCI.Models.Entities
{
    public class ApprovalHistory
    {
        public int ApprovalHistoryId { get; set; }

        public int ModulePageId { get; set; }

        public int TransactionId { get; set; }

        public int ApproverId { get; set; }

        public int Status { get; set; }

        public string? Remarks { get; set; }

        public DateTime DateCreated { get; set; }

        public int CreatedBy { get; set; }

        public bool IsActive { get; set; }
    }
}
