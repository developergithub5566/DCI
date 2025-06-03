using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DCI.Models.Entities
{
    public class LeaveInfo : IAuditable
    {
        [Key]
        public int LeaveId { get; set; }

        public int EmployeeId { get; set; }

        public decimal VLBalance { get; set; }

        public decimal SLBalance { get; set; }

        public DateTime? DateModified { get; set; }

        public int? ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public Employee? Employee { get; set; }
    }
}
