using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DCI.Models.Entities
{
    public class LeaveRequestHeader : IAuditable
    {
        [Key]
        public int LeaveRequestHeaderId { get; set; }
        public int EmployeeId { get; set; }
        public string RequestNo { get; set; }
        public DateTime DateFiled { get; set; }
        public int LeaveTypeId { get; set; }
        public int Status { get; set; }
        public string Reason { get; set; } = string.Empty;
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public Employee? Employee { get; set; }
         public decimal NoOfDays { get; set; }
        public virtual ICollection<LeaveRequestDetails> LeaveRequestDetailsList { get; set; }
    }
}
