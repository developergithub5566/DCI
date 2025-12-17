using DCI.PMS.Models.ViewModel;

namespace DCI.PMS.Repository.Interface
{
    public interface IProjectRepository : IDisposable
    {
        Task<IList<ProjectViewModel>> GetAllProject();
        Task<(int statuscode, string message)> Save(ProjectViewModel model);
    }
}
