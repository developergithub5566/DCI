using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IDailyTimeRecordRepository : IDisposable
    {
        Task<IList<DailyTimeRecordViewModel>> GetAllDTR(DailyTimeRecordViewModel model);
        Task<IList<DailyTimeRecordViewModel>> GetAllDTRByEmpNo(string empNo);
        Task<IList<DTRCorrectionViewModel>> GetAllDTRCorrection(DTRCorrectionViewModel model);
        Task<DTRCorrectionViewModel> DTRCorrectionByDtrId(int dtrId);
        Task<(int statuscode, string message)> SaveDTRCorrection(DTRCorrectionViewModel param);
        Task<IList<DailyTimeRecordViewModel>> GetAllUndertime(DailyTimeRecordViewModel model);
        Task<IList<DailyTimeRecordViewModel>> GetUndertimeById(DailyTimeRecordViewModel model);
        //Task<IList<WFHViewModel>> GetAllWFHById(WFHViewModel model);
        //Task<IList<DailyTimeRecordViewModel>> GetAllWFH(DailyTimeRecordViewModel model);
       // Task<(int statuscode, string message)> SaveWFHTimeIn(WFHViewModel model);

    }
}
