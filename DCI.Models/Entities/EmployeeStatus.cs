namespace DCI.Models.Entities
{
    public class EmployeeStatus
    {
        public int EmployeeStatusId { get; set; }
        public string EmployeeStatusName { get; set; }
        public string? Description { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
