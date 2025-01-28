using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
	public interface ITodoRepository : IDisposable
	{
		Task<IList<ApprovalHistoryViewModel>> GetTodoByApproverId(ApprovalHistoryViewModel model);
		Task<(int statuscode, string message)> Approval(ApprovalHistoryViewModel model);
		Task<IList<DocumentViewModel>> GetAllTodo(DocumentViewModel param);
	}
}
