using DCI.Models.ViewModel;
using System.Threading.Tasks;

namespace DCI.Repositories.Interface
{
	public interface IEmailRepository : IDisposable
	{
		Task<bool> IsExistsEmail(string email);
		Task SendResetPassword(string email);
        Task SendToApproval(LeaveViewModel model);
        //Task SendSetPassword(string email);
        Task SendSetPassword(UserViewModel model);
        Task SendToRequestor(LeaveViewModel model);
		Task SendToRequestorDTR(DTRCorrectionViewModel model);
        Task SentToOvertime(OvertimeViewModel model);
        Task SentToDTRCorrection(DTRCorrectionViewModel model);
    }
}
