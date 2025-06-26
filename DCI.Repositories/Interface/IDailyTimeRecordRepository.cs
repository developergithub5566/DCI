using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IDailyTimeRecordRepository : IDisposable
    {
        Task<IList<DailyTimeRecordViewModel>> GetAllDTR();
        Task<IList<DailyTimeRecordViewModel>> GetAllDTRByEmpNo(string empNo);
        Task<IList<DTRCorrectionViewModel>> GetAllDTRCorrection(int empId);
        Task<DTRCorrectionViewModel> DTRCorrectionByDtrId(int dtrId);
        Task<(int statuscode, string message)> SaveDTRCorrection(DTRCorrectionViewModel param);
     
    }
}
