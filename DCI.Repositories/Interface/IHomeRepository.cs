using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IHomeRepository : IDisposable
    {
        Task<DashboardViewModel> GetAllAnnouncement(DashboardViewModel mode);
        Task<IList<NotificationViewModel>> GetAllNotification(NotificationViewModel model);
    }
}
