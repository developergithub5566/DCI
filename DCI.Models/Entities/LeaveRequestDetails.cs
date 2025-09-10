using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DCI.Models.Entities
{
    public class LeaveRequestDetails : IAuditable
    {
        [Key]
        public int LeaveRequestDetailId { get; set; }
        public int LeaveRequestHeaderId { get; set; }        
        public DateTime LeaveDate { get; set; }
        [Column(TypeName = "decimal(7,4)")]
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }     
    }

}
