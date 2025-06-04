using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DCI.Models.Entities
{
    public class LeaveRequestDetails : IAuditable
    {
        [Key]
        public int LeaveRequestDetailId { get; set; }
        public int LeaveRequestHeaderId { get; set; }        
        public DateTime LeaveDate { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }     
    }

}
