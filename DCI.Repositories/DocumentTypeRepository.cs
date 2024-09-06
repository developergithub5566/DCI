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
	public class DocumentTypeRepository : IDocumentTypeRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		public DocumentTypeRepository(DCIdbContext context)
		{
			this._dbContext = context;
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}

		public async Task<IList<DocumentType>> GetAllDocumentType()
		{
			return await _dbContext.DocumentType.AsNoTracking().Where(x => x.IsActive == true).ToListAsync();
		}

		public async Task<DocumentType> GetDocumentTypeById(int docTypeId)
		{
			return await _dbContext.DocumentType.FindAsync(docTypeId);
		}

		public async Task<bool> IsExistsDocumentType(int docTypeId)
		{
			return await _dbContext.DocumentType.AnyAsync(x => x.DocTypeId == docTypeId && x.IsActive == true);
		}

		public async Task<(int statuscode, string message)> Save(DocumentTypeViewModel model)
		{
			try
			{
				if (model.DocTypeId == 0)
				{
					DocumentType entity = new DocumentType();
					entity.DocTypeId = model.DocTypeId;
					entity.Name = model.Name;
					entity.CreatedBy = model.CreatedBy;
					entity.DateCreated = DateTime.Now;
					entity.ModifiedBy = null;
					entity.DateModified = null;
					entity.IsActive = true;
					await _dbContext.DocumentType.AddAsync(entity);
					await _dbContext.SaveChangesAsync();
					return (StatusCodes.Status200OK, "Successfully saved");
				}
				else
				{
					var entity = await _dbContext.DocumentType.FirstOrDefaultAsync(x => x.DocTypeId == model.DocTypeId);
					entity.DocTypeId = model.DocTypeId;
					entity.Name = model.Name;
					entity.DateCreated = entity.DateCreated;
					entity.CreatedBy = entity.CreatedBy;
					entity.DateModified = DateTime.Now;
					entity.ModifiedBy = model.ModifiedBy;
					entity.IsActive = true;

					_dbContext.DocumentType.Entry(entity).State = EntityState.Modified;
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

		public async Task<(int statuscode, string message)> Delete(DocumentTypeViewModel model)
		{
			try
			{
				var entity = await _dbContext.DocumentType.FirstOrDefaultAsync(x => x.DocTypeId == model.DocTypeId && x.IsActive == true);
				if (entity == null)
				{
					return (StatusCodes.Status406NotAcceptable, "Invalid document type Id");
				}

				entity.IsActive = false;
				entity.ModifiedBy = model.ModifiedBy;
				entity.DateModified = DateTime.Now;
				_dbContext.DocumentType.Entry(entity).State = EntityState.Modified;
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
