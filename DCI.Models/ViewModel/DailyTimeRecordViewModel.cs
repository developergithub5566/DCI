using DCI.Models.Entities;

namespace DCI.Models.ViewModel
{
    public class DailyTimeRecordViewModel
    {
        public long ID { get; set; } = 0;
        public string EMPLOYEE_NO { get; set; } = string.Empty;
        public string NAME { get; set; } = string.Empty;
        public DateTime DATE { get; set; } = DateTime.Now;
        public string FIRST_IN { get; set; } = string.Empty;
        public string LAST_OUT { get; set; } = string.Empty;
        public string LATE { get; set; } = string.Empty;
        public string CLOCK_OUT { get; set; } = string.Empty;
        public string UNDER_TIME { get; set; } = string.Empty;
        public string OVERTIME { get; set; } = string.Empty;
        public string TOTAL_HOURS { get; set; } = string.Empty;
        public string TOTAL_WORKING_HOURS { get; set; } = string.Empty;
        public int STATUS { get; set; } = 0;
        public string STATUSNAME { get; set; } = string.Empty;

        public int ScopeTypeEmp { get; set; } = 0;
        public int CurrentUserId { get; set; } = 0;

        public string DATESTRING { get; set; } = string.Empty;
        public string DATE_COVERED { get; set; } = string.Empty;
        public string TOTAL_UNDERTIME { get; set; } = string.Empty;
        public string TOTAL_UNDERTIMEHOURS { get; set; } = string.Empty;

        public string TOTAL_LATE { get; set; } = string.Empty;
        public string TOTAL_LATEHOURS { get; set; } = string.Empty;

        public string SOURCE { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; } = DateTime.Now;
        public DateTime DateTo { get; set; } = DateTime.Now;

        public bool IsHoliday { get; set; } = false;
        //public int IsHolidayRegularSpecial { get; set; } = 0;

        public int EMPLOYEE_ID { get; set; } = 0;

        public int ModulePageId { get; set; } = 0;
        public string RequestNo { get; set; } = string.Empty;
        public decimal VLBalance { get; set; } = 0;

        public bool IsBiometricRecord { get; set; } = false;
        public bool IsOBFileRecord { get; set; } = false;
        public bool IsWFHFileRecord { get; set; } = false;

        public string FIRST_IN_WFH { get; set; } = string.Empty;
        public string LAST_OUT_WFH { get; set; } = string.Empty;
        public string CLOCK_OUT_WFH { get; set; } = string.Empty;
        public string TOTAL_WORKING_HOURS_WFH { get; set; } = string.Empty;

        public int ScopeTypeJobRecurring { get; set; } = 0;
        //   public string DayType { get; set; } = string.Empty;
      //  public DateTime DateFilter { get; set; } = DateTime.Now;

    }
    public class DTRCorrectionViewModel
    {
        public int DtrId { get; set; } = 0;
        public int EmployeeId { get; set; } = 0;
        public string RequestNo { get; set; } = string.Empty;
        public DateTime DateFiled { get; set; }
        public int DtrType { get; set; } = 0;
        public DateTime DtrDateTime { get; set; }
        public int Status { get; set; } = 0;
        public string StatusName { get; set; } = string.Empty;
        public string EmployeeName { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public string? Filename { get; set; } = string.Empty;
        public string? FileLocation { get; set; } = string.Empty;
        public string DeptHead { get; set; } = string.Empty;
        public int CreatedBy { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public string TimeIn { get; set; } = string.Empty;
        public string TimeOut { get; set; } = string.Empty;
        public string DepartmentHead { get; set; } = string.Empty;
        public int ApproverId { get; set; } = 0;
        public string RequestorEmail { get; set; } = string.Empty;
        public string ApproverEmail { get; set; } = string.Empty;
        public string EmailBody { get; set; } = string.Empty;

        public int? ModifiedBy { get; set; } = 0;
        public DateTime? DateModified { get; set; } = DateTime.Now;

        public string DateApprovedDisapproved { get; set; } = string.Empty;
        public string ApprovalRemarks { get; set; } = string.Empty;

        public int ScopeTypeEmp { get; set; } = 0;
        public int CurrentUserId { get; set; } = 0;
    }

    public class EmployeeUnderTimeSummary
    {
        public string EMPLOYEE_NO { get; set; }
        public string NAME { get; set; }
        public decimal? TotalUnderTime { get; set; }
    }

    public class UndertimeDeductionViewModel
    {
        public string EmpNo { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; } 
        public DateTime DateTo { get; set; }
        public decimal? TotalUndertime { get; set; } = 0;
    }

    public class UndertimeHeaderDeductionViewModel
    {
        public int CurrentUserId { get; set; } = 0;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<UndertimeDeductionViewModel> UndertimeDeductionList { get; set; } = new List<UndertimeDeductionViewModel>();
    }

    public class UndertimeHeaderViewModel
    {
        public int UndertimeHeaderId { get; set; } = 0;
        public string RequestNo { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int CreatedBy { get; set; } = 0;
        public DateTime DateCreated { get; set; }
        public string CreatedName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;     
    }
    public class UndertimeDetailViewModel
    {
        public int UndertimeDetailId { get; set; } = 0;
        public int UndertimeHeaderId { get; set; } = 0;
        public int AttendanceId { get; set; } = 0;
        public int DeductionType { get; set; } = 0;
        public string DeductionTypeName { get; set; } = string.Empty;
        public int CreatedBy { get; set; } = 0;
        public bool IsActive { get; set; } = true;        

        public string EMPLOYEE_NO { get; set; } = string.Empty;
        public string NAME { get; set; } = string.Empty;
        public DateTime DATE { get; set; } = DateTime.Now;
        public string FIRST_IN { get; set; } = string.Empty;
        public string LAST_OUT { get; set; } = string.Empty;
        public string TOTAL_WORKING_HOURS { get; set; } = string.Empty;
        public string LATE { get; set; } = string.Empty;
        public string CLOCK_OUT { get; set; } = string.Empty;
        public string UNDER_TIME { get; set; } = string.Empty;   
    }

    public class LateDeductionViewModel
    {
        public string EmpNo { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public decimal? TotalUndertime { get; set; } = 0;
    }

    public class LateHeaderDeductionViewModel
    {
        public int CurrentUserId { get; set; } = 0;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public List<LateDeductionViewModel> LateDeductionList { get; set; } = new List<LateDeductionViewModel>();
    }

    public class LateHeaderViewModel
    {
        public int LateHeaderId { get; set; } = 0;
        public string RequestNo { get; set; } = string.Empty;
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public int CreatedBy { get; set; } = 0;
        public DateTime DateCreated { get; set; }
        public string CreatedName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
    public class LateDetailViewModel
    {
        public int LateDetailId { get; set; } = 0;
        public int LateHeaderId { get; set; } = 0;
        public int AttendanceId { get; set; } = 0;
        public int DeductionType { get; set; } = 0;
        public string DeductionTypeName { get; set; } = string.Empty;
        public int CreatedBy { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public string EMPLOYEE_NO { get; set; } = string.Empty;
        public string NAME { get; set; } = string.Empty;
        public DateTime DATE { get; set; } = DateTime.Now;
        public string FIRST_IN { get; set; } = string.Empty;
        public string LAST_OUT { get; set; } = string.Empty;
        public string TOTAL_WORKING_HOURS { get; set; } = string.Empty;
        public string LATE { get; set; } = string.Empty;
        public string CLOCK_OUT { get; set; } = string.Empty;
        public string LATE_TIME { get; set; } = string.Empty;
    }
}


