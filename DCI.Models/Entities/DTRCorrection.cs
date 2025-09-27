using DCI.Models.Configuration;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.ComponentModel.DataAnnotations;

namespace DCI.Models.Entities
{
    public class DTRCorrection : IAuditable
    {
        [Key]
        public int DtrId { get; set; }
        public int EmployeeId { get; set; }
        public string RequestNo { get; set; }

        public DateTime DateFiled { get; set; }

        public int DtrType { get; set; }

        public DateTime DtrDateTime { get; set; }
         public int Status { get; set; }

        public string Reason { get; set; } = string.Empty;

         public int ApproverId { get; set; }

        //public string? Filename { get; set; }

        //public string? FileLocation { get; set; }

        public int CreatedBy { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? DateModified { get; set; }

        public bool IsActive { get; set; }

         public int RawLogsId { get; set; }
    }
}
