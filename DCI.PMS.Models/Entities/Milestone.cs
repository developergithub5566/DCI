namespace DCI.PMS.Models.Entities
{
    public class Milestone
    {
        public int MileStoneId { get; set; }

        public int? ProjectCreationId { get; set; }   // FK (nullable based on your schema)

        public string MilestoneName { get; set; } = string.Empty;

        public string Percentage { get; set; } = string.Empty;

        public DateTime? TargetCompletedDate { get; set; }
        public DateTime? ActualCompletionDate { get; set; }

        public int PaymentStatus { get; set; }
        public int Status { get; set; }

        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }

        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        // Navigation Properties
        public Project? Project { get; set; }
        public ICollection<Deliverable>? Deliverables { get; set; }
    }

}
