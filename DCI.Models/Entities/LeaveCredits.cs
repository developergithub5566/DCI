using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DCI.Models.Entities
{
    public class LeaveCredits : IAuditable
    {
        [Key]
        public int LeaveCreditId { get; set; }

        public int EmployeeId { get; set; }

        public int LeaveTypeId { get; set; }

        public decimal? BegBal { get; set; }

        public decimal? Credit { get; set; }

        public DateTime DateCreated { get; set; }

        public bool IsActive { get; set; }
    }
}
