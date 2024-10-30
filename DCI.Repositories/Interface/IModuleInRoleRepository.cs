using DCI.Models.Entities;
using DCI.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Repositories.Interface
{
    public interface IModuleInRoleRepository : IDisposable
    {
		Task<(int statuscode, string message)> Save(ModuleInRoleViewModel model);
		Task<(int statuscode, string message)> Delete(int id);
        Task<IList<ModuleInRole>> GetAllModuleInRole();
        Task<ModuleInRole> GetModuleInRoleById(int moduleinroleid);
		Task<bool> IsExistsModuleInRole(int moduleinroleid);
        Task DeletebyRoleId(int roleId);
		Task<IList<int>> GetModuleInRoleByRoleId(int roleId);
		Task<IList<ModuleInRoleViewModel>> GetModuleAccessByRoleId(int roleId);
	}
}
