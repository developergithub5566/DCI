using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations;

namespace DCI.Models.Entities
{
    public class OvertimeDetail : IAuditable
    {

        [Key]
        public int OTDetailId { get; set; }

        public int OTHeaderId { get; set; }
        public int OTType { get; set; }

        public DateTime OTDate { get; set; }

        public DateTime OTTimeFrom { get; set; }

        public DateTime OTTimeTo { get; set; }
        public int Total { get; set; }

        public bool IsActive { get; set; }

    }
}
