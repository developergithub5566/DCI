using DCI.PMS.Models.ViewModel;

namespace DCI.PMS.Repository.Interface
{
    public interface ITrackerRepository : IDisposable
    {
        Task<IList<TrackerViewModel>> GetAllTracker();
    }
}
