using DCI.Models.Configuration;

namespace DCI.PMS.Models.Entities
{
    public class Coordinator : IAuditable
    {
        public int Id { get; set; }

        public int ProjectCreationId { get; set; }
        public int MileStoneId { get; set; }

        public int UserId { get; set; }

        public bool IsActive { get; set; }

        public DateTime? DateCreated { get; set; }
    }
}
