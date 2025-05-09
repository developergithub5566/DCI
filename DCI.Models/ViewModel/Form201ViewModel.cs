using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Models.ViewModel
{
    public class Form201ViewModel
    {
        public int EmployeeId { get; set; }
        public string? Firstname { get; set; } = string.Empty;
        public string? Middlename { get; set; } = string.Empty;
        public string? Lastname { get; set; } = string.Empty;
        public char? Sex { get; set; }
        public string? Prefix { get; set; } = string.Empty;
        public string? Suffix { get; set; } = string.Empty;
        public string? Nickname { get; set; } = string.Empty;
        public DateTime? DateBirth { get; set; } = null;
        public string? MobileNoPersonal { get; set; } = string.Empty;
        public string? LandlineNo { get; set; } = string.Empty;
        public string? PresentAddress { get; set; } = string.Empty;
        public string? PermanentAddress { get; set; } = string.Empty;
        public string? EmailPersonal { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; } = 0;
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;

        public int EmployeeWorkDetailsId { get; set; } = 0;
        public string? EmployeeNo { get; set; }
        public string? Email { get; set; }
        public string? SSSNo { get; set; }
        public string? Tin { get; set; }
        public string? Pagibig { get; set; }
        public string? Philhealth { get; set; }
        public string? TaxExemption { get; set; }
        public string? MobileNoOffice { get; set; }
        public int? EmploymentTypeId { get; set; }
        public int? Department { get; set; }
        public string? JobFunction { get; set; }
        public DateTime? DateHired { get; set; }
        public string? Position { get; set; }
    }
}
