using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DCI.Models.Entities
{
    public class DTRCorrection : IAuditable
    {
        [Key]
        public int DtrId { get; set; }

        public DateTime DateFiled { get; set; }

        public int DtrType { get; set; }

        public DateTime DateTime { get; set; }
         public int Status { get; set; }

        public string Reason { get; set; } = string.Empty;

        public string? Filename { get; set; }

        public string? FileLocation { get; set; }

        public int CreatedBy { get; set; }

        public bool IsActive { get; set; }
    }
}
