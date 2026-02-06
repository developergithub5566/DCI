using Microsoft.AspNetCore.Http;

namespace DCI.PMS.Models.ViewModel
{
    public class TrackerViewModel
    {
        public int MileStoneId { get; set; } = 0;

        public int ProjectCreationId { get; set; } = 0;

        public string MilestoneName { get; set; } = string.Empty;

        public double Percentage { get; set; } = 0;

        public DateTime? TargetCompletedDate { get; set; } = DateTime.Now;
        public DateTime? ActualCompletionDate { get; set; } = DateTime.Now;

        public int PaymentStatus { get; set; } = 0;
        public int Status { get; set; } = 0;
        public string? StatusName { get; set; } = string.Empty;
        public string? PaymentStatusName { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int ProjectOwnerId { get; set; } = 0;

        public string ProjectOwnerName { get; set; } = string.Empty;

        public DateTime? DateModified { get; set; } = DateTime.Now;
        public int? ModifiedBy { get; set; } = 0;

        public bool IsActive { get; set; } = true;
        public string Remarks { get; set; } = string.Empty;

        public List<DeliverableViewModel>? DeliverableList { get; set; } = new();
        public List<StatusViewModel>? StatusList { get; set; } = new();
        public DeliverableViewModel Deliverable { get; set; } = new();

        public string? TargetCompletedDateString { get; set; } = string.Empty;
        public string? ActualCompletionDateString { get; set; } = string.Empty;
        public List<IFormFile> OtherAttachmentMilestone { get; set; } = new();
        public List<AttachmentViewModel>? AttachmentList { get; set; } = new();
        public int MilestoneOwnerId { get; set; } = 0;

        public string MilestoneOwnerName { get; set; } = string.Empty;
    }
}
