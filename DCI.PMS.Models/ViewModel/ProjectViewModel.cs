namespace DCI.PMS.Models.ViewModel
{
    public class ProjectViewModel
    {
        public int ProjectCreationId { get; set; }

        public int ClientId { get; set; }   // FK to Client

        public string? ProjectNo { get; set; }

        public string ProjectName { get; set; } = string.Empty;

        public DateTime NOADate { get; set; }
        public DateTime NTPDate { get; set; }
        public DateTime MOADate { get; set; }

        public int ProjectDuration { get; set; }
        public int ModeOfPayment { get; set; }

        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }

        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }

        public bool IsActive { get; set; }

    }
}
