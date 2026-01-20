namespace DCI.PMS.Models.ViewModel
{
    public class StatusViewModel
    {
        public int StatusId { get; set; } = 0;
        public string StatusName { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; } = 0;

        public DateTime? DateModified { get; set; } = DateTime.Now;
        public int? ModifiedBy { get; set; } = 0;

        public bool IsActive { get; set; } = true;
    }
}
