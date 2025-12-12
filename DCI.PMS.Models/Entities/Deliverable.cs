namespace DCI.PMS.Models.Entities
{
    public class Deliverable
    {
        public int DeliverableId { get; set; }

        public int MileStoneId { get; set; }   // FK to MileStone

        public string DeliverableName { get; set; } = string.Empty;

        public int Status { get; set; }

        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }

        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        // Navigation Property
        public Milestone? Milestone { get; set; }
    }

}
