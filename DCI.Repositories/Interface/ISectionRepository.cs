using DCI.Models.Entities;
using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
	public interface ISectionRepository : IDisposable
	{
		Task<(int statuscode, string message)> Save(SectionViewModel model);
		Task<(int statuscode, string message)> Delete(SectionViewModel model);
		Task<SectionViewModel> GetSectiontById(int sectionId);
		Task<IList<SectionViewModel>> GetAllSection();
		Task<bool> IsExistsSection(int sectionId);
		Task<SectionViewModel> GetSectionByDepartmentId(SectionViewModel model);
	}
}
