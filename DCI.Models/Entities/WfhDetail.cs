using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class WfhDetail : IAuditable
    {
        public int WfhDetailId { get; set; }
        public int WfhHeaderId { get; set; }
        public int AttendanceId { get; set; }
        
        //public DateTime WFHDate { get; set; }
        public bool IsActive { get; set; }

        // Optional navigation property if using EF
        public WfhHeader? WfhHeader { get; set; }
    }
}
