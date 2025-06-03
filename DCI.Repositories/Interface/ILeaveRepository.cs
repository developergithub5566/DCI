using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface ILeaveRepository : IDisposable
    {
        Task<LeaveViewModel> GetAllLeave(LeaveViewModel param);
    }
}
