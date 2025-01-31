using DCI.Models.Entities;
using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
	public interface IDepartmentRepository : IDisposable
	{
		Task<(int statuscode, string message)> Save(DepartmentViewModel model);
		Task<(int statuscode, string message)> Delete(DepartmentViewModel model);
		//Task<Department> GetDepartmentById(int DepartmentId);
		Task<DepartmentViewModel> GetDepartmentById(int DepartmentId);
		Task<IList<DepartmentViewModel>> GetAllDepartment();
		Task<bool> IsExistsDepartment(int DepartmentId);
	}
}
