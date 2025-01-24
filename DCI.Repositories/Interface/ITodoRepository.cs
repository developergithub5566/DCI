using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
	public interface ITodoRepository : IDisposable
	{
		Task<IList<ApprovalHistoryViewModel>> GetTodoByApproverId(ApprovalHistoryViewModel model);
	}
}
