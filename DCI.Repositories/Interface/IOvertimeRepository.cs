using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IOvertimeRepository : IDisposable
    {
        Task<DailyTimeRecordViewModel> GetAllAttendanceByDate(DateTime date, string empno);
    }
}
