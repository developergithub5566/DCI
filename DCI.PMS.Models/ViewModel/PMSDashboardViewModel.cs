namespace DCI.PMS.Models.ViewModel
{
    public class PMSDashboardViewModel
    {
        public List<MilestoneViewModel> ProjectList { get; set; } = new();
        public int TotalProject { get; set; } = 0;
        public int TotalNotStarted { get; set; } = 0;
        public int TotalInProgress { get; set; } = 0;
        public int TotalCompleted { get; set; } = 0;
    }
}
