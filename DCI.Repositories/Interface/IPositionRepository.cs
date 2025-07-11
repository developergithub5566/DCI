using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IPositionRepository : IDisposable
    {
        Task<IList<PositionViewModel>> GetAllPosition();
    }
}
