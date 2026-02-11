using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.PMS.Models.ViewModel
{
    public class DashboardViewModel
    {
        public List<MilestoneViewModel> ProjectList { get; set; } = new();
        public int TotalProject { get; set; } = 0;
        public int TotalNotStarted { get; set; } = 0;
        public int TotalInProgress { get; set; } = 0;
        public int TotalCompleted { get; set; } = 0;
    }
}
