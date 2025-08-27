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
        public DateTime DateFrom { get; set; } = DateTime.Now;
        public DateTime DateTo { get; set; } = DateTime.Now;

        public bool IsHoliday { get; set; } = false;

        public int EMPLOYEE_ID { get; set; } = 0;
    }
    public class DTRCorrectionViewModel
    {
        public int DtrId { get; set; } = 0;
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

        public string RequestorEmail { get; set; } = string.Empty;
        public string EmailBody { get; set; } = string.Empty;

        public int ScopeTypeEmp { get; set; } = 0;
        public int CurrentUserId { get; set; } = 0;
    }

    public class EmployeeUnderTimeSummary
    {
        public string EMPLOYEE_NO { get; set; }
        public string NAME { get; set; }
        public decimal? TotalUnderTime { get; set; }
    }        
}


