using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IWfhRepository : IDisposable
    {
        Task<IList<DailyTimeRecordViewModel>> GetAllWFH(DailyTimeRecordViewModel model);
        Task<(int statuscode, string message)> SaveWFHTimeIn(WFHViewModel model);
        Task<(int statuscode, string message)> SaveWFHApplication(WfhApplicationViewModel model);
        Task<IList<WFHHeaderViewModel>> GetAllWFHApplication(WFHHeaderViewModel model);
        Task<IList<WfhDetailViewModel>> GetWFHApplicationDetailByWfhHeaderId(WFHHeaderViewModel model);
        Task<IList<WFHViewModel>> GetWFHLogsByEmployeeId(WFHViewModel model);
    }
}
