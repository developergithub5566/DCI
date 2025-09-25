using DCI.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public string? EmployeeNo { get; set; }
        public string? Firstname { get; set; } = string.Empty;
        public string? Middlename { get; set; } = string.Empty;
        public string? Lastname { get; set; } = string.Empty;
        public string? Sex { get; set; }
        public string? Prefix { get; set; } = string.Empty;
        public string? Suffix { get; set; } = string.Empty;
        public string? Nickname { get; set; } = string.Empty;
        public DateTime? DateBirth { get; set; } = null;
        public int? CivilStatus { get; set; }
        public string? MobileNoPersonal { get; set; } = string.Empty;
        public string? LandlineNo { get; set; } = string.Empty;
        public string? PresentAddress { get; set; } = string.Empty;
        public string? PermanentAddress { get; set; } = string.Empty;
        public string? EmailPersonal { get; set; } = string.Empty;
        public string? ContactPerson { get; set; } = string.Empty;
        public string? ContactPersonNo { get; set; } = string.Empty;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; } = 0;
        public DateTime? DateModified { get; set; }
        public int? ModifiedBy { get; set; }
        public bool IsActive { get; set; } = true;

        public int EmployeeWorkDetailsId { get; set; } = 0;
        public string? Email { get; set; }
        public string? SSSNo { get; set; }
        public string? Tin { get; set; }
        public string? Pagibig { get; set; }
        public string? Philhealth { get; set; }
        public string? NationalId { get; set; }
        public string? MobileNoOffice { get; set; }
        public int? EmployeeStatusId { get; set; }
        public string? EmployeeStatusName { get; set; }
        public int? DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string? JobFunction { get; set; }
        public DateTime? DateHired { get; set; }
        public DateTime? DateRegularized { get; set; }
        public int? PositionId { get; set; }
        public string? PositionName { get; set; } = string.Empty;
        public bool IsResigned { get; set; } = false;
        public DateTime? ResignedDate { get; set; }
        public int? BandLevel { get; set; }
        public int? WorkLocation { get; set; } = 0;
        public string? WorkLocationName { get; set; } = string.Empty;
        public int? PayrollType { get; set; } = 0;
        public IList<Position>? PositionList { get; set; }
        public List<SelectListItem>? OptionsPosition { get; set; }

        public IList<Department>? DepartmentList { get; set; }
        public List<SelectListItem>? OptionsDepartment { get; set; }

        public IList<EmployeeStatus>? EmployeeStatusList { get; set; }
        public List<SelectListItem>? OptionsEmployeeStatus { get; set; }

        public IList<WorkLocation>? WorkLocationList { get; set; }
        public List<SelectListItem>? OptionsWorkLocation { get; set; }

        public int CurrentUserId { get; set; } = 0;
        public decimal VLCredit { get; set; }
        public decimal SLCredit { get; set; }
    }
}
