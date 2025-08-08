namespace DCI.Models.ViewModel
{
    public class HolidayViewModel 
    {
        public int HolidayId { get; set; }
        public DateTime HolidayDate { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
