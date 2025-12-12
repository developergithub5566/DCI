namespace DCI.PMS.Models.Entities
{
    public class Attachment
    {
        public int AttachmentId { get; set; }

        public int ProjectCreationId { get; set; }   // FK to ProjectCreation

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
