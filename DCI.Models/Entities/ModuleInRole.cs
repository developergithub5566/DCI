using DCI.Models.Configuration;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DCI.Models.Entities
{
    public class ModuleInRole : IAuditable
    {
   //     [Key]
        public int ModuleInRoleId { get; set; } = 0;
      //  [ForeignKey("ModulePage")]
        public int ModulePageId { get; set; } = 0;
      //  [ForeignKey("Role")]
        public int RoleId { get; set; } = 0;
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public int CreatedBy { get; set; } = 0;
        public DateTime DateModified { get; set; } = DateTime.Now;
        public int ModifiedBy { get; set; } = 0;
        public bool IsActive { get; set; } = true;
        public bool? View { get; set; } = null;
        public bool? Add { get; set; } = null;
        public bool? Update { get; set; } = null;
        public bool? Delete { get; set; } = null;
        public bool? Import { get; set; } = null;
        public bool? Export { get; set; } = null;
    }
}
