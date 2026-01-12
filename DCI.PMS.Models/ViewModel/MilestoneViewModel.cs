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

        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }

        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public List<DeliverableViewModel>? DeliverableList { get; set; } = new();

    }
}
