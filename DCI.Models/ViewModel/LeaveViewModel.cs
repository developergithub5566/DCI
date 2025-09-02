using DCI.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DCI.Models.ViewModel
{
    public class LeaveViewModel
    {
        public int EmployeeId { get; set; } = 0;
        public string EmpNo { get; set; } = string.Empty;
        public decimal VLBalance { get; set; } = 0;
        public decimal SLBalance { get; set; } = 0;
        public decimal SPLBalance { get; set; } = 0;
        public int LeaveTypeId { get; set; } = 0;

        public List<LeaveSummaryViewModel>? vlSummaries { get; set; } = new();
        public List<LeaveSummaryViewModel>? slSummaries { get; set; } = new();

        public int LeaveRequestHeaderId { get; set; } = 0;

        public LeaveRequestHeaderViewModel? LeaveRequestHeader { get; set; } = new();
        public List<LeaveRequestHeaderViewModel>? LeaveRequestHeaderList { get; set; } = new(); 
        public List<LeaveTypeViewModel>? LeaveTypeList { get; set; } = new();

       // public string SelectedDateJson { get; set; } = string.Empty;
        public List<SelectListItem>? OptionsLeaveType { get; set; }

        public List<string> LeaveDateList { get; set; } = new();

        public List<int> YearList { get; set; } = new();
        public int SelectedYear { get; set; }

        public int CurrentUserId { get; set; } = 0;

        public string EmailBody { get; set; } = string.Empty;
        public string RequestorEmail { get; set; } = string.Empty;
        public string ApproverEmail { get; set; } = string.Empty;
        public int? ApproverId { get; set; } = 0;
        public string StatusName { get; set; } = string.Empty;
    }

    public class LeaveRequestHeaderViewModel
    {
        public int LeaveRequestHeaderId { get; set; }
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string RequestNo { get; set; } = string.Empty;
        public DateTime DateFiled { get; set; }
        public string DateFiledString { get; set; } = string.Empty;
        public int LeaveTypeId { get; set; }
        public string LeaveName { get; set; } = string.Empty;
        public int Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public decimal NoofDays { get; set; } = 0;
        public List<LeaveRequestDetailViewModel> LeaveRequestDetailList { get; set; } = new();
        public IList<LeaveType>? LeaveTypeList { get; set; }
        //public List<DateTime>? LeaveDateList { get; set; }

        public DateTime DateApprovedDisapproved { get; set; }
    }

    public class LeaveRequestDetailViewModel
    {
        public int LeaveRequestDetailId { get; set; } = 0;
        public int LeaveRequestHeaderId { get; set; } = 0;
        public DateTime LeaveDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }

    public class LeaveSummary
    {

        public int EmployeeId { get; set; }
        public string AsOf { get; set; }
        public decimal BegBal { get; set; }
        public decimal Credit { get; set; }
        public decimal Availed { get; set; }
        public decimal Monetized { get; set; }
        public decimal EndBal { get; set; }
    }

    public class LeaveSummaryViewModel
    {

        public int EmployeeId { get; set; }
        public string AsOf { get; set; }
        public decimal BegBal { get; set; }
        public decimal Credit { get; set; }
        public decimal Availed { get; set; }
        public decimal Monetized { get; set; }
        public decimal EndBal { get; set; }
    }

    public class LeaveFormViewModel
    {
        public int EmployeeId { get; set; } = 0;
        public int LeaveTypeId { get; set; } = 0;
        public decimal NoOfDays { get; set; } = 0;
        public string Period { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string RequestNo { get; set; } = string.Empty;
        public string SelectedDateJson { get; set; } = string.Empty;
        public List<DateTime> SelectedDateList { get; set; } = new();
    }

    public class LeaveTypeViewModel
    {
        public int LeaveTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
    }

    //public class LeaveSummaryVL
    //{
    //    public int LeaveRequestHeaderId { get; set; }
    //    public int EmployeeId { get; set; }
    //    public DateTime DateFiled { get; set; }
    //    public int LeaveType { get; set; }
    //    public string Reason { get; set; } = string.Empty;
    //    public DateTime? DateModified { get; set; }
    //    public string? ModifiedBy { get; set; }
    //    public bool IsActive { get; set; }
    //    public List<Employee> Employee { get; set; } = new();
    //}
}
