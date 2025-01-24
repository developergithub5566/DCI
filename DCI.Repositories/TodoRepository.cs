using DCI.Data;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
	public class TodoRepository : ITodoRepository, IDisposable
	{
		private DCIdbContext _dbContext;

		public TodoRepository(DCIdbContext context)
		{
			this._dbContext = context;
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}

		//public async Task<Department> GetDepartmentById(int DepartmentId)
		//{
		//	return await _dbContext.Department.FindAsync(DepartmentId);
		//}

		public async Task<IList<ApprovalHistoryViewModel>> GetTodoByApproverId(ApprovalHistoryViewModel model)
		{

			try
			{
				var context = _dbContext.ApprovalHistory.AsQueryable().Where(x => x.IsActive == true).ToList();
				var userList = _dbContext.User.AsQueryable().Where(x => x.IsActive == true).ToList();
				var docList = _dbContext.Document.AsQueryable().Where(x => x.IsActive == true).ToList();
				var statusList = _dbContext.Status.AsQueryable().Where(x => x.IsActive == true).ToList();

				var query = from apprv in context
							join user in userList on apprv.ApproverId equals user.UserId
							join doc in docList on apprv.DocId equals doc.DocId
							join stat in statusList on doc.StatusId equals stat.StatusId
							where apprv.IsActive == true
							select new ApprovalHistoryViewModel
							{
								ApprovalHistoryId = apprv.ApprovalHistoryId,
								DocId = apprv.DocId,
								DocumentName = doc.DocName,
								Action = apprv.Action,
								ApproverId = apprv.ApproverId,
								Remarks = apprv.Remarks,
								CreatedBy = apprv.CreatedBy,
								DateCreated = apprv.DateCreated,
								CurrentStatus = stat.StatusName
							};

				return query.ToList();
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
			return null;
		}
		//public async Task<bool> IsExistsDepartment(int DepartmentId)
		//{
		//	return await _dbContext.Department.AnyAsync(x => x.DepartmentId == DepartmentId && x.IsActive == true);
		//}
	}
}
