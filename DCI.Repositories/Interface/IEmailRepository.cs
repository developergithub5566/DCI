using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
	public interface IEmailRepository : IDisposable
	{
		Task<bool> IsExistsEmail(string email);
		Task SendResetPassword(string email);
		Task SendUploadFile(DocumentViewModel model);
		Task SendSetPassword(string email);
		Task SendApproval(DocumentViewModel model);
		Task SendRequestor(DocumentViewModel model, ApprovalViewModel apprvm);
	}
}
