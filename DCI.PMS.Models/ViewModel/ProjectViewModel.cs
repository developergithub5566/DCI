using Microsoft.AspNetCore.Http;

namespace DCI.PMS.Models.ViewModel
{
    public class ProjectViewModel
    {
        public int ProjectCreationId { get; set; } = 0;

        public int ClientId { get; set; }   = 0;

        public string? ProjectNo { get; set; } = string.Empty;

        public string ProjectName { get; set; } = string.Empty;

        public DateTime NOADate { get; set; } = new DateTime();
        public DateTime NTPDate { get; set; } = new DateTime();
        public DateTime MOADate { get; set; } = new DateTime();

        public int ProjectDuration { get; set; } = 0;
        public decimal ProjectCost { get; set; } = 0;
        public int ModeOfPayment { get; set; } = 0;

        public DateTime DateCreated { get; set; } = new DateTime();
        public int CreatedBy { get; set; } = 0;

        public DateTime? DateModified { get; set; } = new DateTime();
        public int? ModifiedBy { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public string? CreatedName { get; set; } = string.Empty;

        public IFormFile NOADateUploadFile { get; set; } 
        public IFormFile NTPDateUploadFile { get; set; } 
        public IFormFile MOADateUploadFile { get; set; } 

    }
}
