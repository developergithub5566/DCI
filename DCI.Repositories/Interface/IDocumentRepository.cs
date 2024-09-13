using DCI.Models.Entities;
using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
	public interface IDocumentRepository : IDisposable
	{
		Task<DocumentViewModel> GetDocumentById(int docId);
		Task<IList<DocumentViewModel>> GetAllDocument();
		Task<bool> IsExistsDocument(int docId);
		Task<(int statuscode, string message)> Save(DocumentViewModel model);
		Task<(int statuscode, string message)> Delete(DocumentViewModel model);

	}
}
