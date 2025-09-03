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
        public decimal SPLBalance { get; set; }
        
        public decimal Credit { get; set; }

        public DateTime DateCreated { get; set; }

        public int CreatedBy { get; set; }

        public bool IsActive { get; set; }

        public Employee? Employee { get; set; }
    }
}
