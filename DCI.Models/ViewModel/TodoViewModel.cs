namespace DCI.Models.ViewModel
{
    public class TodoViewModel
    {

        public bool Action { get; set; } = true;
        public int ApprovalHistoryId { get; set; } = 0;
        public int ModulePageId { get; set; } = 0;
        public int TransactionId { get; set; } = 0;
        public string RequestNo { get; set; } = string.Empty;
        public int ApproverId { get; set; } = 0;
        public int Status { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; } = 0;
        public string? Remarks { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string StatusName { get; set; } = string.Empty;
        public string StatusDate { get; set; } = string.Empty;
        public int CurrentUserId { get; set; } = 0;
        public string Requestor { get; set; } = string.Empty;
        public string ModuleName { get; set; } = string.Empty;

        public int LeaveCount { get; set; } = 0;
        public int WFHCount { get; set; } = 0;
        public int OvertimeCount { get; set; } = 0;
        public int DTRCount { get; set; } = 0;

        public List<OvertimeViewModel> otList { get; set; } = new List<OvertimeViewModel>();
        public List<LeaveRequestHeaderViewModel> leaveList { get; set; } = new List<LeaveRequestHeaderViewModel>();
        public List<DTRCorrectionViewModel> dtrList { get; set; } = new List<DTRCorrectionViewModel>();
        public List<WFHHeaderViewModel> wfhList { get; set; } = new List<WFHHeaderViewModel>();

    }
}
