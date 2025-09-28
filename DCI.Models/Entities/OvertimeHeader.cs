using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DCI.Models.Entities
{
    public class OvertimeHeader : IAuditable
    {
        [Key]
        public int OTHeaderId { get; set; }
        public string RequestNo { get; set; }

        public int EmployeeId { get; set; }

      //  public int Total { get; set; }

        public int StatusId { get; set; }
         public string Remarks { get; set; }

        public DateTime DateCreated { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? DateModified { get; set; }

        public int? ModifiedBy { get; set; }

        public bool IsActive { get; set; }

        public int? ApproverId { get; set; }
    }
}
