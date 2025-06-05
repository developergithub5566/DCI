using DCI.Models.Entities;

namespace DCI.Models.ViewModel
{
    public class LeaveViewModel
    {
        public int EmployeeId { get; set; } = 0;
        public string EmpNo { get; set; } = string.Empty;
        public decimal VLBalance { get; set; } = 0;
        public decimal SLBalance { get; set; } = 0;
        public int LeaveType { get; set; } = 0;

        public List<LeaveSummaryViewModel>? vlSummaries { get; set; } = new();
        public List<LeaveSummaryViewModel>? slSummaries { get; set; } = new();
        public List<LeaveRequestHeaderViewModel>? LeaveRequestHeaderViewModel { get; set; } = new();
    }

    public class LeaveRequestHeaderViewModel
    {
        public int LeaveRequestHeaderId { get; set; }
        public int EmployeeId { get; set; }
        public string RequestNo { get; set; } = string.Empty;
        public DateTime DateFiled { get; set; }
        public int LeaveTypeId { get; set; }
        public string LeaveName { get; set; } = string.Empty;
        public int Status { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DateTime? DateModified { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public decimal NoofDays { get; set; } = 0;
        public List<LeaveRequestDetailViewModel> LeaveRequestDetailViewModel { get; set; } = new();
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

    public class LeaveForm
    {
        public int EmployeeId { get; set; }
        public int LeaveTypeId { get; set; }
        public decimal NoOfDays { get; set; }
        public string Period { get; set; }
        public string Reason { get; set; }
        public List<DateTime> SelectedDates { get; set; } = new();
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
