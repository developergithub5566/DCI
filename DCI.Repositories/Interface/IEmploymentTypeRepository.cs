using DCI.Models.Entities;
using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
	public interface IEmploymentTypeRepository : IDisposable
	{
		Task<EmploymentType> GetEmploymentTypeById(int empTypeId);
		Task<IList<EmploymentType>> GetAllEmploymentType();
		Task<bool> IsExistsEmploymentType(int empTypeId);
		Task<(int statuscode, string message)> Save(EmploymentTypeViewModel model);
		Task<(int statuscode, string message)> Delete(EmploymentTypeViewModel model);
	}
}
