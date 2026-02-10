using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.PMS.Models.ViewModel
{
    public class CoordinatorViewModel
    {
        public int Id { get; set; } = 0;

        public int ProjectCreationId { get; set; } = 0;
        public int MileStoneId { get; set; } = 0;

        public int UserId { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
