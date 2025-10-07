using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IHomeRepository : IDisposable
    {
        Task<DashboardViewModel> GetAllAnnouncement(DashboardViewModel mode);
        Task<IList<NotificationViewModel>> GetAllNotification(NotificationViewModel model);
        Task SaveNotification(NotificationViewModel model);
        Task MarkAsRead(NotificationViewModel model);
        Task SaveEmailNotificationForBiometrics(UserViewModel model);
    }
}
