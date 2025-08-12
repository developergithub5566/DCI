using DCI.Models.Configuration;
using Microsoft.VisualBasic;

namespace DCI.Models.ViewModel
{
    public class OvertimeViewModel
    {
        public int OTHeaderId { get; set; } = 0;
        
        public string RequestNo { get; set; } = string.Empty;

        public int EmployeeId { get; set; } = 0;

        public int Total { get; set; } = 0;
        public string TotalString { get; set; } = string.Empty;

        public int StatusId { get; set; } = 0;
        public string StatusName { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; } = DateTime.Now;

        public int CreatedBy { get; set; } = 0;

        public DateTime? DateModified { get; set; }

        public int? ModifiedBy { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public int CurrentUserId { get; set; } = 0;

        public List<OvertimeDetailViewModel> OvertimeDetailViewModel { get; set; } = new();

        public string Fullname { get; set; } = string.Empty;
        public string RecommendedBy { get; set; } = string.Empty;
        public string ApprovedBy { get; set; } = string.Empty;
    }

    public class OvertimeDetailViewModel
    {
        public int OTDetailId { get; set; } = 0;

        public int OTHeaderId { get; set; } = 0;

        public int OTType { get; set; } = 0;

        public DateTime OTDate { get; set; } = DateTime.Now;

        public DateTime OTTimeFrom { get; set; } = DateTime.Now;

        public DateTime OTTimeTo { get; set; } = DateTime.Now;
        public int TotalMinutes { get; set; } = 0;

        public bool IsActive { get; set; } = true;

    }

    public class SubmitOvertimeViewModel
    {
        public List<OvertimeEntryDto> Entries { get; set; } = new List<OvertimeEntryDto>();
       // public int GrandTotalMinutes { get; set; } = 0;
        public DateTime OTDate { get; set; } = DateTime.Now;
        public string OTTimeFrom { get; set; } = string.Empty;
        public string OTTimeTo { get; set; } = string.Empty;
        //public int CurrentUserId { get; set; }
        public string EmployeeNo { get; set; } = string.Empty;
        public bool IsOfficialBuss { get; set; } = false;
        public int CurrentUserId { get; set; } = 0;
        public int EmployeeId { get; set; } = 0;
        public string RequestNo { get; set; } = string.Empty;
    }

    public class OvertimeEntryDto
    {
        public int OTType { get; set; } = 0;
        public string OTTypeName { get; set; } = string.Empty;
        public DateTime OTDate { get; set; } = DateTime.Now;
        public string OTDateString { get; set; } = string.Empty;
        public string OTTimeFrom { get; set; } = string.Empty;
        public string OTTimeTo { get; set; } = string.Empty;
        public int TotalMinutes { get; set; } = 0;
        public string TotalHours { get; set; } = string.Empty;
        public string EmployeeNo { get; set; } = string.Empty;
    }




}
