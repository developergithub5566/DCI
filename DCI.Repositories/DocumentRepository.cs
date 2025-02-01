using DCI.Data;
using DCI.Core.Common;
using DCI.Models.Configuration;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using DCI.Core.Helpers;
using System.Text.RegularExpressions;
using System.Diagnostics.CodeAnalysis;

namespace DCI.Repositories
{
	public class DocumentRepository : IDocumentRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		private readonly IOptions<FileModel> _filelocation;
		private IEmailRepository _emailRepository;
		private IDepartmentRepository _departmentRepository;

		public DocumentRepository(DCIdbContext context, IOptions<FileModel> filelocation, IEmailRepository emailRepository, IDepartmentRepository departmentRepository)
		{
			this._dbContext = context;
			this._filelocation = filelocation;
			this._emailRepository = emailRepository;
			this._departmentRepository = departmentRepository;
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}

		public async Task<IList<DocumentViewModel>> GetAllDocument(DocumentViewModel param)
		{
			//return await _dbContext.Document.AsNoTracking().Where(x => x.IsActive == true).ToListAsync();

			var context = _dbContext.Document.AsQueryable();
			//var doctypeList = _dbContext.DocumentType.AsQueryable().ToList();
			var userList = _dbContext.User.AsQueryable().ToList();

			var query = from doc in context
						join doctype in _dbContext.DocumentType on doc.DocTypeId equals doctype.DocTypeId
						join user in _dbContext.User on doc.CreatedBy equals user.UserId
						join stat in _dbContext.Status on doc.StatusId equals stat.StatusId
						where doc.IsActive == true
						select new DocumentViewModel
						{
							DocId = doc.DocId,
							DocName = doc.DocName,
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

			if (param.CurrentRoleId == (int)EnumRole.User)
			{
				query = query.Where(x => x.StatusId != (int)EnumDocumentStatus.Deleted);
			}

			//result.DocumentTypeList = doctypeList.Count() > 0 ? doctypeList : null;
			//result.DocumentList = query.ToList();
			return query.ToList();
		}

		public async Task<DocumentViewModel> GetDocumentById(int docId)
		{
			try
			{
				//DocumentViewModel usermodel = new DocumentViewModel();

				var context = _dbContext.Document.AsQueryable();
				var doctypeList = _dbContext.DocumentType.Where(x => x.IsActive == true).AsQueryable().ToList();
				var departmentList = _dbContext.Department.Where(x => x.IsActive == true).AsQueryable().ToList();
				var sectionList = _dbContext.Section.Where(x => x.IsActive == true).AsQueryable().ToList();
				//var userList = _dbContext.User.Where(x => x.IsActive == true).AsQueryable().ToList();
				var userList = _dbContext.User.Where(x => x.IsActive).OrderBy(x => x.Lastname).ToList();

				var query = from doc in context
							where doc.DocId == docId &&
							doc.IsActive == true
							select new DocumentViewModel
							{
								DocId = doc.DocId,
								DocName = doc.DocName,
								DocNo = doc.DocNo,
								Version = doc.Version,
								Filename = doc.Filename,
								FileLocation = doc.FileLocation,
								DocTypeId = doc.DocTypeId,
								DocumentTypeList = null,
								DocTypeName = string.Empty,
								StatusId = doc.StatusId,
								Reviewer = doc.Reviewer,
								Approver = doc.Approver,
								DepartmentId = doc.DepartmentId,
								DocCategory = doc.DocCategory,
								RequestById = doc.RequestById,
								SectionId = doc.SectionId,
								FormsProcess = doc.FormsProcess,
							};

				var result = query.FirstOrDefault();

				if (result == null)
				{
					result = new DocumentViewModel();
				}
				result.DocumentTypeList = doctypeList.Count() > 0 ? doctypeList : null;
				result.DepartmentList = departmentList.Count() > 0 ? departmentList : null;
				result.SectionList = sectionList.Count() > 0 ? sectionList : null;
				result.UserList = userList.Count() > 0 ? userList : null;
				result.DocumentList = null;
				return result;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return null;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public async Task<bool> IsExistsDocument(int docId)
		{
			return await _dbContext.Document.AnyAsync(x => x.DocId == docId && x.IsActive == true);
		}

		public async Task<(int statuscode, string message)> Save(DocumentViewModel model)
		{
			try
			{
				if (model.DocId == 0)
				{
					Document entity = new Document();
					entity.DocId = model.DocId;
					entity.DocNo = await GenerateDocCode(model);
					entity.DocName = model.DocName;
					entity.DocTypeId = model.DocTypeId;
					entity.DocCategory = model.DocCategory;
					entity.DepartmentId = model.DepartmentId;

					DepartmentViewModel deptViewModel = await _departmentRepository.GetDepartmentById(model.DepartmentId);
					entity.Reviewer = deptViewModel.Reviewer ?? 0;
					entity.Approver = deptViewModel.Approver ?? 0;

					entity.StatusId = model.StatusId ?? (int)EnumDocumentStatus.InProgress;
					//entity.SectionId = model.SectionId;
					entity.FormsProcess = model.FormsProcess;
					entity.Version = model.Version;
					entity.Filename = model.DocFile != null ? model.DocFile.FileName : string.Empty;
					entity.CreatedBy = model.CreatedBy;
					entity.DateCreated = DateTime.Now;
					entity.UploadLink = TokenGeneratorHelper.GetToken();
					entity.ModifiedBy = null;
					entity.DateModified = null;
					entity.IsActive = true;
					entity.RequestById = model.RequestById;
					await _dbContext.Document.AddAsync(entity);
					await _dbContext.SaveChangesAsync();
					model.DocId = entity.DocId;
					model.DocNo = entity.DocNo;
					model.UploadLink = entity.UploadLink;
					model.StatusId = entity.StatusId;

					if (model.DocFile != null)
					{
						await SaveFile(model);
					}

					if (model.StatusId == (int)EnumDocumentStatus.InProgress)
					{
						await _emailRepository.SendUploadFile(model);
					}

					if (model.StatusId == (int)EnumDocumentStatus.ForReview)
					{
						await _emailRepository.SendApproval(model);
					}


					return (StatusCodes.Status200OK, String.Format("Document {0} has been created successfully.", entity.DocNo));
				}
				else
				{
					var entity = await _dbContext.Document.FirstOrDefaultAsync(x => x.DocId == model.DocId);
					entity.DocId = model.DocId;
					entity.DocNo = entity.DocNo;
					entity.DocName = model.DocName;
					entity.DocTypeId = model.DocTypeId;
					entity.DocCategory = model.DocCategory;
					entity.DepartmentId = model.DepartmentId;
					//entity.SectionId = model.SectionId;
					entity.FormsProcess = model.FormsProcess;
					entity.RequestById = model.RequestById;
					entity.Version = model.Version;
					entity.Filename = model.DocFile != null ? model.DocFile.FileName : entity.Filename;
					entity.DateCreated = entity.DateCreated;
					entity.CreatedBy = entity.CreatedBy;
					entity.DateModified = DateTime.Now;
					entity.ModifiedBy = model.ModifiedBy;
					entity.IsActive = true;

					_dbContext.Document.Entry(entity).State = EntityState.Modified;
					await _dbContext.SaveChangesAsync();

					model.DocId = entity.DocId;

					if (model.DocFile != null)
					{
						await SaveFile(model);
					}

					if (model.StatusId == (int)EnumDocumentStatus.InProgress)
					{
						await _emailRepository.SendUploadFile(model);
					}

					if (model.StatusId == (int)EnumDocumentStatus.ForReview)
					{
						await _emailRepository.SendApproval(model);
					}

					return (StatusCodes.Status200OK, String.Format("Document {0} has been updated successfully.", entity.DocNo));
				}
				
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

		public async Task<(int statuscode, string message)> Delete(DocumentViewModel model)
		{
			try
			{
				var entity = await _dbContext.Document.FirstOrDefaultAsync(x => x.DocId == model.DocId && x.IsActive == true);
				if (entity == null)
				{
					return (StatusCodes.Status406NotAcceptable, "Invalid document Id");
				}

				entity.UploadLink = string.Empty;
				entity.StatusId = (int)EnumDocumentStatus.Deleted;
				entity.IsActive = true;
				entity.ModifiedBy = model.ModifiedBy;
				entity.DateModified = DateTime.Now;
				_dbContext.Document.Entry(entity).State = EntityState.Modified;
				await _dbContext.SaveChangesAsync();
				return (StatusCodes.Status200OK, "Successfully deleted");
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return (StatusCodes.Status400BadRequest, ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		private async Task SaveFile(DocumentViewModel model)
		{
			try
			{
				string fileloc = _filelocation.Value.FileLocation + model.DocId.ToString() + @"\";
				string filename = model.DocFile.FileName.Replace(",", "");

				if (!Directory.Exists(fileloc))
					Directory.CreateDirectory(fileloc);

				string filenameLocation = Path.Combine(fileloc, filename);

				using (var stream = new FileStream(filenameLocation, FileMode.Create, FileAccess.Write))
				{
					model.DocFile.CopyTo(stream);
				}

				var entity = await _dbContext.Document.FirstOrDefaultAsync(x => x.DocId == model.DocId && x.IsActive == true);
				entity.FileLocation = fileloc;


				entity.ModifiedBy = model.ModifiedBy ?? null;
				entity.DateModified = model.DateModified ?? null;
				entity.StatusId = model.StatusId ?? 0;
				if (entity.Filename != null && entity.Filename != "")
				{
					entity.Version = Convert.ToInt16(entity.Version) + 1;
					entity.DocNo = IncrementVersion(entity.DocNo);
				}
				entity.Filename = filename;
				_dbContext.Document.Entry(entity).State = EntityState.Modified;

				if (model.StatusId == (int)EnumDocumentStatus.ForReview)
				{
					model.Reviewer = entity.Reviewer;
					model.Approver = entity.Approver;
					await _emailRepository.SendApproval(model);
				}
				await _dbContext.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		private async Task<string> GenerateDocCode(DocumentViewModel param)
		{

			try
			{
				string docCode = await GetDeletedDocNoToReused(param);

				if (docCode != string.Empty)
				{
					return docCode;
				}

				var _docContext = await _dbContext.Document
												.Where(x => x.DepartmentId == param.DepartmentId && x.IsActive == true)
												.AsQueryable()
												.ToListAsync();

				var _deptContext = await _dbContext.Department
												.AsQueryable()
												.FirstOrDefaultAsync(x => x.DepartmentId == param.DepartmentId && x.IsActive == true);
				/*
				var _docTypeContext = await _dbContext.DocumentType
												.AsQueryable()
												.FirstOrDefaultAsync(x => x.DocTypeId == param.DocTypeId && x.IsActive == true);

				var _section = await _dbContext.Section
												.AsQueryable()
												.FirstOrDefaultAsync(x => x.DepartmentId == param.DepartmentId && x.IsActive == true);
				*/

				int totalrecords = _docContext.Count() + 1;
				string version = "0.0";
				string finalSetRecords = GetFormattedRecord(totalrecords);
				string yearLastTwoDigit = DateTime.Now.Year.ToString().Substring(2, 2);
				string deptcode = _deptContext?.DepartmentCode?.Trim() ?? string.Empty;
				string formsProcess = param.FormsProcess == 1 ? EnumFormsProcess.F.ToString() : EnumFormsProcess.P.ToString();

				if (param.DocCategory == (int)EnumDocumentCategory.Internal)
				{
					//string section = _section?.SectionCode?.Trim() ?? string.Empty;
					return $"{Constants.DocControlNo}-{yearLastTwoDigit}-{deptcode}.{finalSetRecords} Ver.{version}";
				}
				else if (param.DocCategory == (int)EnumDocumentCategory.BothInExternal)
				{
					//string compcode = Constants.CompanyCode;
					//string doctype = _docTypeContext.Name ?? string.Empty;
					return $"{Constants.CompanyCode}-{yearLastTwoDigit}-{deptcode}-{formsProcess}{finalSetRecords} Ver.{version}";
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
			return string.Empty;
		}

		private string GetFormattedRecord(int totalRecords)
		{
			//format 000.000
			//if 1000 then 000.001
			//if 1001 then 001.001
			//if 1002 then 002.001
			//if 2000 then 000.002
			//if 2001 then 001.002

			int setA = totalRecords % 1000;
			int setB = totalRecords / 1000;
			string formattedA = setA.ToString("D3");
			string formattedB = setB.ToString("D3");
			return $"{formattedA}";
		}

		private async Task<string> GetDeletedDocNoToReused(DocumentViewModel param)
		{
			var entity = await _dbContext.Document
												.Where(x => x.DepartmentId == param.DepartmentId && x.DocCategory == param.DocCategory && x.FormsProcess == param.FormsProcess
												&& x.IsActive == true
												&& x.ReUsed == false
												&& x.StatusId == (int)EnumDocumentStatus.Deleted)
												.FirstOrDefaultAsync();
			if (entity != null)
			{
				entity.ReUsed = true;
				entity.ModifiedBy = param.ModifiedBy;
				entity.DateModified = DateTime.Now;
				_dbContext.Document.Entry(entity).State = EntityState.Modified;
				await _dbContext.SaveChangesAsync();
			}


			return entity?.DocNo ?? string.Empty;
		}


		public static string IncrementVersion(string versionString)
		{
			// Define a regex pattern to extract the current version number
			string pattern = @"Ver\.\s*(\d+)";
			var match = Regex.Match(versionString, pattern);

			if (match.Success)
			{
				int currentVersion = int.Parse(match.Groups[1].Value);

				int newVersion = currentVersion + 1;

				string updatedVersionString = Regex.Replace(versionString, pattern, $"Ver.{newVersion}");

				return updatedVersionString;
			}
			return versionString;
		}

		public async Task<DocumentViewModel> ValidateToken(ValidateTokenViewModel param)
		{
			DocumentViewModel vm = new DocumentViewModel();
			try
			{
				var result = _dbContext.Document.Where(x => x.UploadLink == param.Token && x.IsActive == true);

				if (result.Count() > 0)
				{
					return await result.Select(doc => new DocumentViewModel
					{
						DocId = doc.DocId,
						DocName = doc.DocName,
						DocNo = doc.DocNo,
						RequestById = doc.RequestById,
						Filename = doc.Filename,
						//Version = doc.Version,
						//FileLocation = doc.FileLocation,
						//DocTypeId = doc.DocTypeId,
						//DocumentTypeList = null,
						//DocTypeName = string.Empty,
						//StatusId = doc.StatusId,
						//Reviewer = doc.Reviewer,
						//Approver = doc.Approver,
						//DepartmentId = doc.DepartmentId,
						//DocCategory = doc.DocCategory
					}).FirstAsync();
				}
				else
				{
					return vm;
				}
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return vm;
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public async Task<(int statuscode, string message)> UploadFile(DocumentViewModel model)
		{
			await SaveFile(model);
			return (StatusCodes.Status200OK, "Your file has been successfully uploaded.");
		}
		public async Task<IList<HomePageViewModel>> HomePage()
		{
			var doctypeList = _dbContext.DocumentType.Where(x => x.IsActive == true);
			var docContext = _dbContext.Document.Where(x => x.IsActive == true);

			var result = await doctypeList
			   .GroupJoin(
				   docContext, // join with documents
				   doctype => doctype.DocTypeId, // key from DocumentType
				   doc => doc.DocTypeId, // key from Document
				   (doctype, docs) => new HomePageViewModel
				   {
					   DocumentType = doctype.Name,
					   TotalCount = docs.Count() // count the associated documents or 0 if no documents exist
				   }
			   )
			   .ToListAsync();

			return result;
		}

		public async Task<WorkflowViewModel> Workflow(DocumentViewModel param)
		{
			//var query = await (from doc in _dbContext.Document
			//				   join stat in _dbContext.Status on doc.StatusId equals stat.StatusId
			//				   join request in _dbContext.User on doc.RequestById equals request.UserId
			//				   join reviewer in _dbContext.User on doc.Reviewer equals reviewer.UserId into reviewerGroup
			//				   from reviewer in reviewerGroup.DefaultIfEmpty()

			//				   join approver in _dbContext.User on doc.Approver equals approver.UserId into approverGroup
			//				   from approver in approverGroup.DefaultIfEmpty()

			//				   where doc.IsActive && doc.DocId == param.DocId
					var query = await (
					from doc in _dbContext.Document
					join stat in _dbContext.Status on doc.StatusId equals stat.StatusId into statGroup
					from stat in statGroup.DefaultIfEmpty()

					join request in _dbContext.User on doc.RequestById equals request.UserId into requestGroup
					from request in requestGroup.DefaultIfEmpty()

					join reviewer in _dbContext.User on doc.Reviewer equals reviewer.UserId into reviewerGroup
					from reviewer in reviewerGroup.DefaultIfEmpty()

					join apprUser in _dbContext.User on doc.Approver equals apprUser.UserId into apprUserGroup
					from apprUser in apprUserGroup.DefaultIfEmpty()

					join reviewApprv in _dbContext.ApprovalHistory on new { doc.DocId, CreatedBy = doc.Reviewer } equals new { reviewApprv.DocId, reviewApprv.CreatedBy } into reviewApprvGroup
					from reviewApprv in reviewApprvGroup.DefaultIfEmpty()

					join apprHist in _dbContext.ApprovalHistory on new { doc.DocId, CreatedBy = doc.Approver } equals new { apprHist.DocId, apprHist.CreatedBy } into apprHistGroup
					from apprHist in apprHistGroup.DefaultIfEmpty()

					where doc.IsActive && doc.DocId == param.DocId
					select new WorkflowViewModel
					{
						DocId = doc.DocId,
						DocNo = doc.DocNo,
						StatusId = doc.StatusId,
						RequestBy = $"{request.Firstname} {request.Lastname}",
						RequestByDatetime = doc.DateCreated.ToString(),
						ReviewedBy = reviewer != null ? $"{reviewer.Firstname} {reviewer.Lastname}" : string.Empty,
						ReviewedByDatetime = reviewer != null ? reviewApprv.DateCreated.ToString() : string.Empty,
						ApprovedBy = apprUser != null ? $"{apprUser.Firstname} {apprUser.Lastname}" : string.Empty,
						ApprovedByDatetime = apprUser != null ? apprHist.DateCreated.ToString() : string.Empty,
						CurrentStatus = stat.StatusName,
						ReviewedRemarks = reviewApprv.Remarks,
						ApprovedRemarks = apprHist.Remarks,
					}).FirstOrDefaultAsync();

			if (query.StatusId == (int)EnumDocumentStatus.ForReview)
			{
				query.ReviewedStatus = EnumDocumentStatus.InProgress.ToString();
				query.ApprovedStatus = EnumDocumentStatus.Pending.ToString();
				query.CurrentStatus = "Review";
				query.Percentage = 33;	

			}
			else if (query.StatusId == (int)EnumDocumentStatus.ForApproval)
			{
				query.ReviewedStatus = EnumDocumentStatus.Reviewed.ToString();
				query.ApprovedStatus = EnumDocumentStatus.InProgress.ToString();
				query.CurrentStatus = "Approval";
				query.Percentage = 66;
			}
			else if (query.StatusId == (int)EnumDocumentStatus.Approved)
			{
				query.CurrentStatus = "Completed";
				query.Percentage = 100;
			}
			return query;
		}

		public async Task<DocumentViewModel> UpdateApprovalStatusByDocId(DocumentViewModel model, ApprovalViewModel apprvm)
		{
			var entity = await _dbContext.Document.FirstOrDefaultAsync(x => x.DocId == model.DocId);

			if (entity.StatusId == (int)EnumDocumentStatus.ForReview && apprvm.Action)
			{
				entity.StatusId = (int)EnumDocumentStatus.ForApproval;
			}
			else if (entity.StatusId == (int)EnumDocumentStatus.ForApproval && apprvm.Action)
			{
				entity.StatusId = (int)EnumDocumentStatus.Approved;
			}
			else if (entity.StatusId == (int)EnumDocumentStatus.ForReview && !apprvm.Action)
			{
				entity.StatusId = (int)EnumDocumentStatus.InProgress;
			}
			else if (entity.StatusId == (int)EnumDocumentStatus.ForApproval && !apprvm.Action)
			{
				entity.StatusId = (int)EnumDocumentStatus.InProgress;
			}

			entity.DateModified = DateTime.Now;
			entity.ModifiedBy = model.ModifiedBy;
			entity.IsActive = true;

			_dbContext.Document.Entry(entity).State = EntityState.Modified;
			await _dbContext.SaveChangesAsync();

			model.StatusId = entity.StatusId;
			model.RequestById = entity.RequestById;
			model.UploadLink = entity.UploadLink;
			return model;
		}	}
}
