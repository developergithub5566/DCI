using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface ILeaveRepository : IDisposable
    {
        Task<LeaveViewModel> GetAllLeave(LeaveViewModel param);
        //Task<LeaveViewModel> RequestLeave(LeaveViewModel param);
        Task<LeaveViewModel> RequestLeave(LeaveViewModel param);
        Task<(int statuscode, string message)> SaveLeave(LeaveFormViewModel param);
        Task<IList<LeaveReportViewModel>> GetAllLeaveReport();
        Task<(int statuscode, string message)> CancelLeave(LeaveRequestHeaderViewModel model);
    }
}
