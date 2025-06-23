using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
	public interface IEmailRepository : IDisposable
	{
		Task<bool> IsExistsEmail(string email);
		Task SendResetPassword(string email);
        Task SendToApproval(LeaveViewModel model);
		Task SendSetPassword(string email);
		Task SendToRequestor(LeaveViewModel model);
    }
}
