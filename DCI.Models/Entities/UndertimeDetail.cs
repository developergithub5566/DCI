using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class UndertimeDetail : IAuditable
    {
        public int UndertimeDetailId { get; set; }
        public int UndertimeHeaderId { get; set; }
        public int AttendanceId { get; set; }
        public int DeductionType { get; set; }
        public bool IsActive { get; set; }

        // Navigation property
        public UndertimeHeader UndertimeHeader { get; set; }
    }
}
