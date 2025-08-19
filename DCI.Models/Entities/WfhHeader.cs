using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class WfhHeader : IAuditable
    {
        public int WfhHeaderId { get; set; }
        public string? RequestNo { get; set; }
        public int EmployeeId { get; set; }
        public int Status { get; set; }
        public int ApproverId { get; set; }
        public string? Remarks { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }

}
