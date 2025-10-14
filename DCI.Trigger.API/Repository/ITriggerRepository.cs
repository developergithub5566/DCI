using DCI.Models.ViewModel;
using DCI.Trigger.API.Model;

namespace DCI.Trigger.API.Repository
{
    public interface ITriggerRepository : IDisposable
    {
        Task<IList<OutboxQueue>> GetAllAttendanceLog();
        Task UpdateOutBoxQueueStatus(OutboxMessageViewModel model);
    }
}
