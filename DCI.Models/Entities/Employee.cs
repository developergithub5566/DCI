using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class Employee : IAuditable
    {
        public int EmployeeId { get; set; }
        public string? EmployeeNo { get; set; }
        public string? Email { get; set; }
        public string? Firstname { get; set; }
        public string? Middlename { get; set; }
        public string? Lastname { get; set; }
        public string? Sex { get; set; }
        public string? Prefix { get; set; }
        public string? Suffix { get; set; }
        public string? Nickname { get; set; }
        public DateTime? DateBirth { get; set; }
        public string? MobileNoPersonal { get; set; }
        public string? LandlineNo { get; set; }
        public string? PresentAddress { get; set; }
        public string? PermanentAddress { get; set; }
        public string? EmailPersonal { get; set; }
        public DateTime DateCreated { get; set; }
        public int CreatedBy { get; set; } 
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; }
    }
}
