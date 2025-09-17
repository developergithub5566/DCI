namespace DCI.Models.ViewModel
{
    public class HolidayViewModel 
    {
        public int HolidayId { get; set; } = 0;
        public DateTime HolidayDate { get; set; } = DateTime.Now;
        public string HolidayName { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public int HolidayType { get; set; } = 0;
        public string HolidayTypeName { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; } = 0;
        public DateTime? DateModified { get; set; } = DateTime.Now;
        public int? ModifiedBy { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public string CreatedName { get; set; } = string.Empty;
    }
}
