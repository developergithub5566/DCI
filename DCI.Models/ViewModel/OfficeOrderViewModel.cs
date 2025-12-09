namespace DCI.Models.ViewModel
{
    public class OfficeOrderViewModel
    {
          public int OfficeOrderId { get; set; }
        public string? OrderName { get; set; } = string.Empty;
        public DateTime OrderDate { get; set; }
        public string? Description { get; set; } = string.Empty;
        public string? Filename { get; set; } = string.Empty;
        public string? FilePath { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
        public string CreatedName { get; set; } = string.Empty;
    }
}
