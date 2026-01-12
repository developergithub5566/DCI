using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.PMS.Models.ViewModel
{
    public class DeliverableViewModel
    {
        public int DeliverableId { get; set; } = 0;

        public int MileStoneId { get; set; } = 0;
        public string DeliverableName { get; set; } = string.Empty;
        public int Status { get; set; } = 0;
        public DateTime DateCreated { get; set; } = new DateTime();
        public int CreatedBy { get; set; } = 0;

        public DateTime? DateModified { get; set; } = new DateTime();
        public int? ModifiedBy { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public string? CreatedName { get; set; } = string.Empty;
    }
}
