using DCI.Models.ViewModel;
using DCI.PMS.Models.ViewModel;

namespace DCI.PMS.Repository.Interface
{
    public interface IHomeRepository : IDisposable
    {
        Task<PMSDashboardViewModel> GetDashboard();
        Task<IList<NotificationViewModel>> GetAllNotification(NotificationViewModel model);
        Task SaveNotification(NotificationViewModel model);
        Task MarkAsRead(NotificationViewModel model);
    }
}
