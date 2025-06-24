using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IDailyTimeRecordRepository : IDisposable
    {
        Task<IList<DailyTimeRecordViewModel>> GetAllDTR();
        Task<IList<DailyTimeRecordViewModel>> GetAllDTRByEmpNo(string empNo);
        Task<IList<DTRCorrectionViewModel>> GetAllDTRCorrectionByEmpId(int empId);
    }
}
