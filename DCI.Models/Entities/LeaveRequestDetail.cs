using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class LeaveRequestDetail : IAuditable
    {
        public int LeaveRequestDetailId { get; set; }
        public int LeaveRequestHeaderId { get; set; }        
        public DateTime LeaveDate { get; set; }
        public decimal Amount { get; set; }
        public bool IsActive { get; set; }
        public LeaveRequestHeader? LeaveRequestHeader { get; set; }
    }

}
