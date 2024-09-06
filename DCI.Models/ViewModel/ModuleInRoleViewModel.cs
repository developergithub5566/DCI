using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Models.ViewModel
{
    public class ModuleInRoleViewModel
    {
        public int ModuleInRoleId { get; set; } = 0;
		public int ModulePageId { get; set; } = 0;

		public int RoleId { get; set; } = 0;
		public DateTime DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; } = 0;
		public DateTime DateModified { get; set; }
		public int ModifiedBy { get; set; } = 0;
		public bool IsActive { get; set; } = true;
		public bool? View { get; set; } = false;
		public bool? Add { get; set; } = false;
		public bool? Update { get; set; } = false;
		public bool? Delete { get; set; } = false;
		public bool? Import { get; set; } = false;
		public bool? Export { get; set; } = false;
		public string RoleName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
	}
    //public class SaveModuleInRoleViewModel
    //{
    //    public int ModuleInRoleId { get; set; }
    //    public int ModulePageId { get; set; }
    //    public int RoleId { get; set; }
    //}
}
