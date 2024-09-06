using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Repositories
{
	public class DocumentRepository : IDocumentRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		public DocumentRepository(DCIdbContext context)
		{
			this._dbContext = context;
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}

		public async Task<IList<Document>> GetAllDocument()
		{
			return await _dbContext.Document.AsNoTracking().Where(x => x.IsActive == true).ToListAsync();
		}

		public async Task<Document> GetDocumentById(int docId)
		{
			return await _dbContext.Document.FindAsync(docId);
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
					entity.Filename = model.Filename;
					entity.CreatedBy = model.CreatedBy;
					entity.DateCreated = DateTime.Now;

					entity.ModifiedBy = null;
					entity.DateModified = null;					
					entity.IsActive = true;
					await _dbContext.Document.AddAsync(entity);
					await _dbContext.SaveChangesAsync();
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
					entity.Filename = model.Filename;
					entity.DateCreated = entity.DateCreated;
					entity.CreatedBy = entity.CreatedBy;
					entity.DateModified = DateTime.Now;
					entity.ModifiedBy = model.ModifiedBy;
					entity.IsActive = true;

					_dbContext.Document.Entry(entity).State = EntityState.Modified;
					await _dbContext.SaveChangesAsync();
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
				var entity = await _dbContext.Document.FirstOrDefaultAsync(x => x.DocTypeId == model.DocTypeId && x.IsActive == true);
				if (entity == null)
				{
					return (StatusCodes.Status406NotAcceptable, "Invalid document type Id");
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
	}
}
