using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Models.ViewModel
{ 
    public class RoleViewModel
	{
		public int RoleId { get; set; } = 0;
        public string RoleName { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public DateTime DateCreated { get; set; } = DateTime.Now;
		public int CreatedBy { get; set; } = 0;
		public DateTime DateModified { get; set; } = DateTime.Now;
		public int ModifiedBy { get; set; } = 0;
		public bool IsActive { get; set; } = true;
	}
}
