using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class LeaveRequestHeader : IAuditable
    {
        public int LeaveRequestHeaderId { get; set; }
        public int EmployeeId { get; set; }
        public string RequestNo { get; set; }
        public DateTime DateFiled { get; set; }
        public int LeaveTypeId { get; set; }
        public int Status { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime? DateModified { get; set; }
        public string? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public Employee? Employee { get; set; }   
    }
}
