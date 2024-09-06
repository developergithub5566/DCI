using DCI.Models.Entities;
using DCI.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Repositories.Interface
{
	public interface IModulePageRepository : IDisposable
	{
		Task<(int statuscode, string message)> Save(ModulePageViewModel model);

        Task<(int statuscode, string message)> Delete(ModulePageViewModel model);
		Task<ModulePage> GetModulePageById(int modulepageid);
		Task<IList<ModulePage>> GetAllModulePage();
		Task<bool> IsExistsModulePage(int moduleId);

	}
}
