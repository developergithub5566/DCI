namespace DCI.Models.Entities
{
    public class LeaveDetails
    {
        public int LeaveDetailId { get; set; }

        public int LeaveId { get; set; }

        public DateTime LeaveDate { get; set; }
        public bool IsHalfDay { get; set; }
        public bool IsActive { get; set; }
        // Navigation property
        public Leave Leave { get; set; } = null!;
    }
}
