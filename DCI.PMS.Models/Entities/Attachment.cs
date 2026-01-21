using System.ComponentModel.DataAnnotations;

namespace DCI.PMS.Models.Entities
{
    public class Attachment
    {
        [Key]
        public int AttachmentId { get; set; }

        public int ProjectCreationId { get; set; }   
        public int MileStoneId { get; set; }    
        public int DeliverableId { get; set; }

        public int AttachmentType { get; set; }

        public string Filename { get; set; } = string.Empty;
        public string FileLocation { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }

        public bool IsActive { get; set; }

        // Navigation Property
        public Project? Project { get; set; }
    }

}
