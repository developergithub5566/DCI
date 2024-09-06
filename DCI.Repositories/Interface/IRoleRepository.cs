using DCI.Models.Entities;
using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
	public interface IRoleRepository : IDisposable
	{
		Task<(int statuscode, string message, Role entity)> Save(RoleViewModel model);
		Task<(int statuscode, string message)> Delete(RoleViewModel model);
		Task<Role> GetRoleById(int roleId);
		Task<IList<Role>> GetAllRoles();
		Task<bool> IsExistsRole(int roleId);

	}
}
