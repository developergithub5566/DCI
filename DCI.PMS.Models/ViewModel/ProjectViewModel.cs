using Microsoft.AspNetCore.Http;
using System.Diagnostics.Contracts;

namespace DCI.PMS.Models.ViewModel
{
    public class ProjectViewModel
    {
        public int ProjectCreationId { get; set; } = 0;

        public int ClientId { get; set; }   = 0;

        public string ClientName { get; set; } = string.Empty;

        public string? ProjectNo { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public DateTime NOADate { get; set; } = new DateTime();
        public DateTime NTPDate { get; set; } = new DateTime();
        public DateTime MOADate { get; set; } = new DateTime();

        public int ProjectDuration { get; set; } = 0;
        public decimal ProjectCost { get; set; } = 0;
        public int ModeOfPayment { get; set; } = 0;
        public int Status { get; set; } = 0;
        public int PaymentStatus { get; set; } = 0;
        public DateTime DateCreated { get; set; } = new DateTime();
        public int CreatedBy { get; set; } = 0;

        public DateTime? DateModified { get; set; } = new DateTime();
        public int? ModifiedBy { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public string? CreatedName { get; set; } = string.Empty;

        public IFormFile? NOAFile { get; set; }
        public IFormFile? NTPFile { get; set; }
        public IFormFile? MOAFile { get; set; }

        public MilestoneViewModel Milestone { get; set; } = new();
        public List<MilestoneViewModel>? MilestoneList { get; set; } = new();
        public List<ClientViewModel>? ClientList { get; set; } = new();
        public List<AttachmentViewModel>? AttachmentList { get; set; } = new();

        public List<StatusViewModel>? StatusList { get; set; } = new();
        public bool? IsNOAFile { get; set; } = false;
        public bool? IsNTPFile { get; set; } = false;
        public bool? IsMOAFile { get; set; } = false;
    }
}
