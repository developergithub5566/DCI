using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface ILateRepository : IDisposable
    {
        Task<IList<DailyTimeRecordViewModel>> GetAllLate(DailyTimeRecordViewModel model);
        Task<IList<DailyTimeRecordViewModel>> GetLateById(DailyTimeRecordViewModel model);
        Task<(int statuscode, string message)> SaveLate(List<LateDeductionViewModel> model);
        Task<IList<LateHeaderViewModel>> GetLateDeduction(DailyTimeRecordViewModel model);
        Task<IList<LateDetailViewModel>> GetLateDeductionByHeaderId(DailyTimeRecordViewModel model);
    }
}
