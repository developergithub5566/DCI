namespace DCI.Models.ViewModel
{
    public class WFHViewModel
    {
        public int ID { get; set; } = 0;
        public int EMPLOYEE_ID { get; set; } = 0;
        public string EMPLOYEE_NO { get; set; } = string.Empty;
        public string FULL_NAME { get; set; } = string.Empty;
        public DateTime DATE_TIME { get; set; } = DateTime.Now;
        public DateTime CREATED_DATE { get; set; } = DateTime.Now;
        public string CREATED_BY { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public int CurrentUserId { get; set; } = 0;
        public int ScopeTypeEmp { get; set; } = 0;
        public string TIME_IN { get; set; } = string.Empty;
        public string DATE { get; set; } = string.Empty;
    }

    public class WfhLogModel
    {
        public DateTime DATE_TIME { get; set; }
    }

    public class WfhApplicationViewModel
    {
        public WFHHeaderViewModel Header { get; set; }
        public List<WfhDetailViewModel> Details { get; set; }
    }

    public class WFHHeaderViewModel
    {
        public int WfhHeaderId { get; set; }
        public int EmployeeId { get; set; } = 0;
        public int StatusId { get; set; } = 0;
        public string Fullname { get; set; } = string.Empty;
        public string RequestNo { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public int CurrentUserId { get; set; } = 0;
        public string StatusName { get; set; } = string.Empty;
        public int CreatedBy { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int ApproverId { get; set; } = 0;
        public string DateCreatedString { get; set; } = string.Empty;
        public string? DateModifiedString { get; set; } = string.Empty;
        public string Approver { get; set; } = string.Empty;
        public string DateApprovedDisapproved { get; set; } = string.Empty;
        public string ApprovalRemarks { get; set; } = string.Empty;

        public string ApproverEmail { get; set; } = string.Empty;
        public string EmailBody { get; set; } = string.Empty;
        public string RequestorEmail { get; set; } = string.Empty;    
    }

    public class WfhDetailViewModel
    {
        public int Id { get; set; }
        public string Date { get; set; }
        public string TimeIn { get; set; }
        public string TimeOut { get; set; }

        public int WfhHeaderId { get; set; }
        public string? TotalWorkingHours { get; set; } = string.Empty;
    }    
}
