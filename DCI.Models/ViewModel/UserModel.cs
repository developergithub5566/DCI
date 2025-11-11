using DCI.Models.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DCI.Models.ViewModel
{
    public class UserModel
    {
        public int UserId { get; set; } = 0;
        public int EmployeeId { get; set; } = 0;
        public string Username { get; set; } = string.Empty;
        public string Fullname { get; set; }
        public string EmployeeNo { get; set; }
        //public string Lastname { get; set; }
        //public string ContactNo { get; set; }
        public string Email { get; set; }
        public int RoleId { get; set; }
        public string RoleName { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public IList<Role>? RoleList { get; set; }
        public List<SelectListItem>? Options { get; set; }
        public IList<User>? EmployeeList { get; set; }
        public IList<Employee>? Form201List { get; set; }         
        public IList<EmployeeDropdownModel>? EmployeeDropdownList { get; set; }
        public List<SelectListItem>? OptionsForm201List { get; set; }
        public IList<UserViewModel>? UserViewList { get; set; }
        public IList<Department>? DepartmentList { get; set; }
        public List<SelectListItem>? OptionsDepartment { get; set; }
        public bool EmailBiometricsNotification { get; set; }
        public bool EmailAttendanceConfirmation { get; set; }
        
    }

    public class EmployeeDropdownModel
    {
        public int EmployeeId { get; set; } = 0;
        public string Display { get; set; } = string.Empty;
    }
}
