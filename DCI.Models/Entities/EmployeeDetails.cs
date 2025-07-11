using DCI.Models.Configuration;

namespace DCI.Models.Entities
{
    public class EmployeeWorkDetails : IAuditable
    {
        public int EmployeeWorkDetailsId { get; set; }
        public int EmployeeId { get; set; }   
        public string? SSSNo { get; set; }
        public string? Tin { get; set; }
        public string? Pagibig { get; set; }
        public string? Philhealth { get; set; }
        public string? TaxExemption { get; set; }
        public string? MobileNoOffice { get; set; }
        public int? EmployeeStatusId { get; set; }
        public int? DepartmentId { get; set; }
        public string? JobFunction { get; set; }
        public DateTime? DateHired { get; set; }
        public int? Position { get; set; }
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsResigned { get; set; }
        public bool IsActive { get; set; }

        public Employee? Employee { get; set; }  // Navigation property
    }
}
