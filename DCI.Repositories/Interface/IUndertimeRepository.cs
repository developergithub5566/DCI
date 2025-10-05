using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IUndertimeRepository : IDisposable
    {
        Task<IList<DailyTimeRecordViewModel>> GetAllUndertime(DailyTimeRecordViewModel model);
        Task<IList<DailyTimeRecordViewModel>> GetUndertimeById(DailyTimeRecordViewModel model);
        //  Task<(int statuscode, string message)> SaveUndertime(List<UndertimeDeductionViewModel> model);
        Task<(int statuscode, string message)> SaveUndertime(UndertimeHeaderDeductionViewModel model);        
          Task<IList<UndertimeHeaderViewModel>> GetUndertimeDeduction(DailyTimeRecordViewModel model);
        Task<IList<UndertimeDetailViewModel>> GetUndertimeDeductionByHeaderId(DailyTimeRecordViewModel model);
    }
}
