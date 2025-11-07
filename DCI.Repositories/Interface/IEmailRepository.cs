using DCI.Models.ViewModel;
using System.Threading.Tasks;

namespace DCI.Repositories.Interface
{
	public interface IEmailRepository : IDisposable
	{
		Task<bool> IsExistsEmail(string email);
		Task SendResetPassword(string email);
        Task SendSetPassword(UserViewModel model);

        Task SendToApprovalLeave(LeaveViewModel model);
        Task SendToRequestorLeave(LeaveViewModel model);

        Task SentToApprovalOvertime(OvertimeViewModel model);

        Task SentToApprovalDTRAdjustment(DTRCorrectionViewModel model);
        Task SendToRequestorDTRAdjustment(DTRCorrectionViewModel model);

        Task SentToApprovalWFH(WFHHeaderViewModel model);

        Task SendToRequestorWFH(WFHHeaderViewModel model);
        //Task SendEmailBiometricsNotification(BiometricViewModel model);

        Task SendToRequestorOT(OvertimeViewModel model);
    }
}
