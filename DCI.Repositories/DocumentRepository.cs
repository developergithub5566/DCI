using DCI.Data;
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

namespace DCI.Repositories
{
	public class DocumentRepository : IDocumentRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		private readonly IOptions<FileModel> _filelocation;
		public DocumentRepository(DCIdbContext context, IOptions<FileModel> filelocation)
		{
			this._dbContext = context;
			this._filelocation = filelocation;
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}

		public async Task<IList<DocumentViewModel>> GetAllDocument()
		{
			//return await _dbContext.Document.AsNoTracking().Where(x => x.IsActive == true).ToListAsync();

			var context = _dbContext.Document.AsQueryable();
			//var doctypeList = _dbContext.DocumentType.AsQueryable().ToList();
			var userList = _dbContext.User.AsQueryable().ToList();

			var query = from doc in context
						join doctype in _dbContext.DocumentType on doc.DocTypeId equals doctype.DocTypeId
						join user in _dbContext.User on doc.CreatedBy equals user.UserId
						where doc.IsActive == true
						select new DocumentViewModel
						{
							DocId = doc.DocId,
							DocName = doc.DocName,
							DocNo = doc.DocNo,
							Version = doc.Version,
							Filename = doc.Filename,
							DocTypeId = doc.DocTypeId,
							DocumentTypeList = null,
							DocTypeName = doctype.Name,
							CreatedName = user.Email,
							DateCreated = doc.DateCreated,
						};


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
				var doctypeList = _dbContext.DocumentType.AsQueryable().ToList();

				var query = from doc in context
							where doc.DocId == docId
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
							};

				var result = query.FirstOrDefault();

				if (result == null)
				{
					result = new DocumentViewModel();
				}
				result.DocumentTypeList = doctypeList.Count() > 0 ? doctypeList : null;
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
					entity.DocNo = model.DocNo;
					entity.DocName = model.DocName;
					entity.DocTypeId = model.DocTypeId;
					entity.Version = model.Version;
					entity.Filename = model.DocFile != null ? model.DocFile.FileName : string.Empty;
					entity.CreatedBy = model.CreatedBy;
					entity.DateCreated = DateTime.Now;
					entity.ModifiedBy = null;
					entity.DateModified = null;
					entity.IsActive = true;
					await _dbContext.Document.AddAsync(entity);
					await _dbContext.SaveChangesAsync();
					model.DocId = entity.DocId;

					if (model.DocFile != null)
					{
						await SaveFile(model);
					}
					return (StatusCodes.Status200OK, "Successfully saved");
				}
				else
				{
					var entity = await _dbContext.Document.FirstOrDefaultAsync(x => x.DocId == model.DocId);
					entity.DocId = model.DocId;
					entity.DocNo = model.DocNo;
					entity.DocName = model.DocName;
					entity.DocTypeId = model.DocTypeId;
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
					return (StatusCodes.Status200OK, "Successfully updated");
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

				entity.IsActive = false;
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

				if (!Directory.Exists(fileloc))
					Directory.CreateDirectory(fileloc);

				string filenameLocation = Path.Combine(fileloc, model.DocFile.FileName);

				using (var stream = new FileStream(filenameLocation, FileMode.Create, FileAccess.Write))
				{
					model.DocFile.CopyTo(stream);
				}
				var entity = await _dbContext.Document.FirstOrDefaultAsync(x => x.DocId == model.DocId && x.IsActive == true);
				entity.FileLocation = fileloc;
				entity.Filename = model.DocFile.FileName;
				_dbContext.Document.Entry(entity).State = EntityState.Modified;
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
			string DOT = ".";

			var _docContext = _dbContext.Document.AsQueryable();
			var _docTypeContext = _dbContext.DocumentType.AsQueryable().Where(x => x.DocTypeId == param.DocTypeId);

			int totalrecords = _docContext.Count() + 1;

			//CD.DCC.002.000 Ver. 0.1
			//TID.PM.001.000
			//DCI.MOA.001.000
			//string strFormat = String.Format("Hello {0}", name);

			if (param.DocCategory == 0) //internal
			{
				string deptcode = string.Empty; // TID,HRD, 
				string section = string.Empty; //PM 
				string setNoFirst = totalrecords.ToString();
				string setNoSecond = string.Empty;
				string version = "0.0";

				return string.Format(deptcode, DOT, section, DOT, setNoFirst, DOT, setNoSecond, "Ver.", version);
			}
			else if (param.DocCategory == 1) //internal and external
			{
				string compcode = "DCI"; //DCI
				string doctype = string.Empty; //MOA,NOA,IA //_docTypeContext.DocTypeCode
				string setNoFirst = totalrecords.ToString();
				string setNoSecond = string.Empty;
				string version =  "0.0"; 

				return string.Format(compcode, DOT, doctype, DOT, setNoFirst, DOT, setNoSecond, "Ver.", version);
			}
			return string.Empty;
		}
	}
}
