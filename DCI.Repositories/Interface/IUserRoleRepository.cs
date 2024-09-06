using DCI.Models.Entities;
using DCI.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Repositories.Interface
{
	public interface IUserRoleRepository : IDisposable
	{
		Task<(int statuscode, string message)> Save(RoleInModuleViewModel model);
        //Task<RoleInModuleViewModel> GetModuleAccessRoleByRoleId(int roleId);
        Task<SystemManagementViewModel> GetModuleAccessRoleByRoleId(int roleId);
        Task<IList<UserInRoleViewModel>> GetUserRole();
        Task<(int statuscode, string message)> Delete(UserInRoleViewModel model);
    }
}
