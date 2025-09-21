using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IOvertimeRepository : IDisposable
    {
        // Task<DailyTimeRecordViewModel> GetAllAttendanceByDate(DateTime date, string empno);
        Task<DailyTimeRecordViewModel> GetAllAttendanceByDate(OvertimeViewModel model);
        Task<IList<OvertimeViewModel>> Overtime(OvertimeViewModel model);
        Task<OvertimeViewModel> AddOvertime(OvertimeViewModel model);
        Task<(int statuscode, string message)> SaveOvertime(OvertimeViewModel param);
        Task<OvertimeEntryDto> CheckOvertimeDate(OvertimeEntryDto model);
    }
}
