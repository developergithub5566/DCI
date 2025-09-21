using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IPositionRepository : IDisposable
    {
        Task<IList<PositionViewModel>> GetAllPosition();
        Task<PositionViewModel> GetPositionById(int postId);
        Task<(int statuscode, string message)> Delete(PositionViewModel model);
        Task<(int statuscode, string message)> Save(PositionViewModel model);
    }
}
