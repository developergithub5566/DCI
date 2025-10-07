using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
    public interface IEmployeeRepository : IDisposable
    {
        Task<Form201ViewModel> GetEmployeeById(int empId);
        Task<IList<Form201ViewModel>> GetAllEmployee(CancellationToken ct = default);
        Task<(int statuscode, string message)> Save(Form201ViewModel model);
        Task<(int statuscode, string message)> Delete(Form201ViewModel model);
        Task<(int statuscode, string message)> Update201Form(Form201ViewModel model);
        Task<ReportGraphsDataViewModel> ReportGraphByStatus();

    }
}
