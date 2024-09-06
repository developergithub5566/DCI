using DCI.Models.Entities;
using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
	public interface IDocumentTypeRepository : IDisposable
	{
		Task<DocumentType> GetDocumentTypeById(int docTypeId);
		Task<IList<DocumentType>> GetAllDocumentType();
		Task<bool> IsExistsDocumentType(int docTypeId);
		Task<(int statuscode, string message)> Save(DocumentTypeViewModel model);
		Task<(int statuscode, string message)> Delete(DocumentTypeViewModel model);
	}
}
