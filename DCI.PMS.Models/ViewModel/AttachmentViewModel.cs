namespace DCI.PMS.Models.ViewModel
{
    public class AttachmentViewModel
    {
        public int AttachmentId { get; set; } = 0;

        public int ProjectCreationId { get; set; } = 0;
        public int MileStoneId { get; set; } = 0;
        public int DeliverableId { get; set; } = 0;

        public int AttachmentType { get; set; } = 0;

        public string Filename { get; set; } = string.Empty;
        public string FileLocation { get; set; } = string.Empty;

        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; } = 0;
        public string CreatedName { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
    }
}
