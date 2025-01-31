using DCI.Core.Common;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
	public class TodoRepository : ITodoRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		private IEmailRepository _emailRepository;
		private IDocumentRepository _documentRepository;

		public TodoRepository(DCIdbContext context, IEmailRepository emailRepository, IDocumentRepository documentRepository)
		{
			this._dbContext = context;
			this._emailRepository = emailRepository;
			this._documentRepository = documentRepository;
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}

		//public async Task<Department> GetDepartmentById(int DepartmentId)
		//{
		//	return await _dbContext.Department.FindAsync(DepartmentId);
		//}



		public async Task<IList<DocumentViewModel>> GetAllTodo(DocumentViewModel param)
		{
			try
			{
				var reviewerDocuments = _dbContext.Document.AsQueryable()
				.Where(x => x.Reviewer == param.CurrentUserId && x.StatusId == (int)EnumDocumentStatus.ForReview && x.IsActive);

				var approverDocuments = _dbContext.Document.AsQueryable()
					.Where(x => x.Approver == param.CurrentUserId && x.StatusId == (int)EnumDocumentStatus.ForApproval && x.IsActive);

				var combinedDocuments = reviewerDocuments.Union(approverDocuments);

				var query = from doc in combinedDocuments
							join doctype in _dbContext.DocumentType on doc.DocTypeId equals doctype.DocTypeId
							join user in _dbContext.User on doc.CreatedBy equals user.UserId
							join stat in _dbContext.Status on doc.StatusId equals stat.StatusId
							select new DocumentViewModel
							{
								DocName = doc.DocName,
								DocId = doc.DocId,
								DocNo = doc.DocNo,
								Version = doc.Version,
								Filename = doc.Filename,
								DocTypeId = doc.DocTypeId,
								StatusId = doc.StatusId,
								StatusName = stat.StatusName,
								DocumentTypeList = null,
								DocTypeName = doctype.Name,
								CreatedName = user.Email,
								DateCreated = doc.DateCreated,
							};

				// Execute the query and return the results
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
		public async Task<(int statuscode, string message)> Approval(ApprovalHistoryViewModel model)
		{
			try
			{
				ApprovalHistory entity = new ApprovalHistory();
				entity.ApprovalHistoryId = model.ApprovalHistoryId;
				entity.DocId = model.DocId;
				entity.Action = model.Action;
				entity.ApproverId = model.ApproverId;
				entity.CreatedBy = model.CreatedBy;
				entity.DateCreated = DateTime.Now;
				entity.Remarks = model.Remarks;
				entity.IsActive = true;
				await _dbContext.ApprovalHistory.AddAsync(entity);
				await _dbContext.SaveChangesAsync();

				var docVm = await _documentRepository.GetDocumentById(entity.DocId);

				ApprovalViewModel apprvm = new ApprovalViewModel();
				apprvm.Action = model.Action;		

				var NewDocumentViewModel = await _documentRepository.UpdateApprovalStatusByDocId(docVm, apprvm);

				if (NewDocumentViewModel.StatusId == (int)EnumDocumentStatus.ForApproval || NewDocumentViewModel.StatusId == (int)EnumDocumentStatus.Approved)
				{
					await _emailRepository.SendRequestor(NewDocumentViewModel, apprvm);
				}
				if (NewDocumentViewModel.StatusId == (int)EnumDocumentStatus.ForApproval || NewDocumentViewModel.StatusId == (int)EnumDocumentStatus.ForReview)
				{
					await _emailRepository.SendApproval(NewDocumentViewModel);
				}
			
				return (StatusCodes.Status200OK, String.Format("Document {0} has been updated.", NewDocumentViewModel.DocNo));
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return (StatusCodes.Status406NotAcceptable, ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}
	}
}
