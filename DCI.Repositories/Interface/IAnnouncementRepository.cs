using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IAnnouncementRepository : IDisposable
    {
        Task<AnnouncementViewModel> GetAnnouncementById(int announceId);
        Task<IList<AnnouncementViewModel>> GetAllAnnouncement();
        Task<(int statuscode, string message)> Delete(AnnouncementViewModel model);
        Task<(int statuscode, string message)> Save(AnnouncementViewModel model);
    }
}
