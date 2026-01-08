using DCI.PMS.Models.ViewModel;

namespace DCI.PMS.Repository.Interface
{
    public interface IProjectRepository : IDisposable
    {
        Task<ProjectViewModel> GetProjectId(int projectId);
        Task<IList<ProjectViewModel>> GetAllProject();
        Task<(int statuscode, string message)> SaveProject(ProjectViewModel model);
        Task<ProjectViewModel> GetProjectById(ProjectViewModel model);
        Task<ProjectViewModel> GetMilestoneByProjectId(ProjectViewModel model);
        Task<(int statuscode, string message)> Delete(ProjectViewModel model);
    }
}
