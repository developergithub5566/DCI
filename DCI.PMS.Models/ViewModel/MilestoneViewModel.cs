namespace DCI.PMS.Models.ViewModel
{
    public class MilestoneViewModel
    {
        public int MileStoneId { get; set; }

        public int ProjectCreationId { get; set; }

        public string MilestoneName { get; set; } = string.Empty;

        public double Percentage { get; set; } = 0;

        public DateTime? TargetCompletedDate { get; set; }
        public DateTime? ActualCompletionDate { get; set; }

        public int PaymentStatus { get; set; }
        public int Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string PaymentStatusName { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }

        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public List<DeliverableViewModel>? DeliverableList { get; set; } = new();
        public List<StatusViewModel>? StatusList { get; set; } = new();
        public DeliverableViewModel Deliverable { get; set; } = new();

        public string TargetCompletedDateString { get; set; } = string.Empty;
        public string ActualCompletionDateString { get; set; } = string.Empty;

    }
}
