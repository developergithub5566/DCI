using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IOfficeOrderRepository : IDisposable
    {
        Task<OfficeOrderViewModel> GetOfficeOrderById(int ooId);
        Task<IList<OfficeOrderViewModel>> GetAllOfficeOrder();
        Task<bool> IsExistsOfficeOrder(int ooId);
        Task<(int statuscode, string message)> Save(OfficeOrderViewModel model);
        Task<(int statuscode, string message)> Delete(OfficeOrderViewModel model);
    }
}
