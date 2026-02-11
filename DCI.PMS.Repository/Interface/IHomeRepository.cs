using DCI.PMS.Models.ViewModel;

namespace DCI.PMS.Repository.Interface
{
    public interface IHomeRepository : IDisposable
    {
        Task<DashboardViewModel> GetDashboard();
    }
}
