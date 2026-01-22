using DCI.PMS.Models.ViewModel;

namespace DCI.PMS.Repository.Interface
{
    public interface IProjectRepository : IDisposable
    {
        Task<ProjectViewModel> GetProjectId(int projectId);
        Task<IList<ProjectViewModel>> GetAllProject();
        // Task<(int statuscode, string message)> SaveProject(ProjectViewModel model);
        Task SaveProject(ProjectViewModel model);
        Task<ProjectViewModel> GetProjectById(ProjectViewModel model);
        Task<ProjectViewModel> GetMilestoneByProjectId(ProjectViewModel model);
        Task<(int statuscode, string message)> DeleteProject(ProjectViewModel model);
        Task<(int statuscode, string message)> DeleteMilestone(MilestoneViewModel model);
        Task<(int statuscode, string message)> DeleteDeliverable(DeliverableViewModel model);
        Task SaveMilestone(MilestoneViewModel model);
        Task<MilestoneViewModel> GetDeliverablesByMilestoneId(MilestoneViewModel model);
        Task SaveDeliverable(DeliverableViewModel model);
    }
}
