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
    public class DTRCorrectionViewModel
    {
        public int DtrId { get; set; } = 0;
        public DateTime DateFiled { get; set; }
        public int DtrType { get; set; } = 0;
        public DateTime DateTime { get; set; }
        public int Status { get; set; } = 0;
        public string Reason { get; set; } = string.Empty;
        public string? Filename { get; set; } = string.Empty;
        public string? FileLocation { get; set; } = string.Empty;
        public int CreatedBy { get; set; } = 0;
        public bool IsActive { get; set; } = true;
    }

}


