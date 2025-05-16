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
    }

    public class LeaveViewModel
    {
        public int LeaveId { get; set; } = 0;
        public DateTime DateFiled { get; set; } = DateTime.Now;
        public int LeaveType { get; set; } = 0;
        public string Reason { get; set; } = string.Empty;
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        public List<LeaveDetailViewModel> LeaveDetails { get; set; } = new();
    }
    public class LeaveDetailViewModel
    {
        public int LeaveDetailId { get; set; } = 0;
        public DateTime LeaveDate { get; set; } = DateTime.Now;
        public bool IsHalfDay { get; set; } = false;
        public bool IsActive { get; set; } = true;
    }
}
