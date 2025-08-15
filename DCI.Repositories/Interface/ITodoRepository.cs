using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
	public interface ITodoRepository : IDisposable
	{
        //Task<IList<ApprovalHistoryViewModel>> GetTodoByApproverId(ApprovalHistoryViewModel model);
        //Task<(int statuscode, string message)> Approval(ApprovalHistoryViewModel model);
        Task<IList<LeaveRequestHeaderViewModel>> GetAllTodoLeave(LeaveViewModel model);
        Task<IList<DTRCorrectionViewModel>> GetAllTodoDtr(DTRCorrectionViewModel model);
        Task<(int statuscode, string message)> Approval(ApprovalHistoryViewModel model);
        Task<IList<ApprovalHistoryViewModel>> GetApprovalHistory(ApprovalHistoryViewModel model);
        Task<(int statuscode, string message)> ApprovalDtr(ApprovalHistoryViewModel param);
        Task<IList<OvertimeViewModel>> GetAllTodoOvertime(OvertimeViewModel model);
    }
}
