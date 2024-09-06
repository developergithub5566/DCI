using DCI.Models.Entities;
using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
	public interface IDocumentRepository : IDisposable
	{
		Task<Document> GetDocumentById(int docId);
		Task<IList<Document>> GetAllDocument();
		Task<bool> IsExistsDocument(int docId);
		Task<(int statuscode, string message)> Save(DocumentViewModel model);
		Task<(int statuscode, string message)> Delete(DocumentViewModel model);

	}
}
