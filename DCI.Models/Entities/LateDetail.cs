using System.ComponentModel.DataAnnotations;

namespace DCI.Models.Entities
{
    public class LateDetail
    {
        [Key]
        public int LateDetailId { get; set; }
        public int LateHeaderId { get; set; }
        public int AttendanceId { get; set; }
        public int DeductionType { get; set; }
        public bool IsActive { get; set; }

        // Navigation property
        public LateHeader LateHeader { get; set; }
    }
}
