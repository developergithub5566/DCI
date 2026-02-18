using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public string? ModeOfPaymentName { get; set; } = string.Empty;
        public int Status { get; set; } = 0;
        public int PaymentStatus { get; set; } = 0;

        public string? StatusName { get; set; } = string.Empty;
      //  public string? PaymentStatusName { get; set; } = string.Empty;
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

        //public List<DeliverableViewModel>? DeliveryList { get; set; } = new();
        public List<StatusViewModel>? StatusList { get; set; } = new();
        public bool? IsNOAFile { get; set; } = false;
        public int NOAFileId { get; set; } = 0;
        public bool? IsNTPFile { get; set; } = false;
        public int NTPFileId { get; set; } = 0;
        public bool? IsMOAFile { get; set; } = false;
        public int MOAFileId { get; set; } = 0;

        public List<IFormFile> OtherAttachment { get; set; } = new();

        public int MilestoneId { get; set; } = 0;

      //  public UserSelectViewModel SelectedCoordinator = new();
        public List<int>? SelectedCoordinator { get; set; } = new();
        public List<SelectListItem>? UserList { get; set; } = new();
        public string? EmailBody { get; set; } = string.Empty;
        public List<UserEmailList>? UserEmailList { get; set; } = new();
         public string? Fullname { get; set; } = string.Empty;

        public int ProjectStatusType { get; set; } = 0;

        public List<CoordinatorViewModel> CoordinatorsList { get; set; } = new();

         public int CurrentUserId { get; set; } = 0;
    }

    public class UserEmailList
    {
        public string Email { get; set; } = string.Empty;
        public string Fullname { get; set; } = string.Empty;
    }
}
