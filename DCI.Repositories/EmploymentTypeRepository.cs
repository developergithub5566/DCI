using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
	public class EmploymentTypeRepository : IEmploymentTypeRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		public EmploymentTypeRepository(DCIdbContext context)
		{
			this._dbContext = context;
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}

		public async Task<EmploymentType> GetEmploymentTypeById(int empTypeId)
		{
			return await _dbContext.EmploymentType.FindAsync(empTypeId);
		}
		public async Task<IList<EmploymentType>> GetAllEmploymentType()
		{
			return await _dbContext.EmploymentType.AsNoTracking().Where(x => x.IsActive == true).ToListAsync();
		}
		public async Task<bool> IsExistsEmploymentType(int empTypeId)
		{
			return await _dbContext.EmploymentType.AnyAsync(x => x.EmploymentTypeId == empTypeId && x.IsActive == true);
		}

		public async Task<(int statuscode, string message)> Save(EmploymentTypeViewModel model)
		{
			try
			{
				if (model.EmploymentTypeId == 0)
				{
					EmploymentType entity = new EmploymentType();
					entity.EmploymentTypeId = model.EmploymentTypeId;
					entity.EmploymentTypeName = model.EmploymentTypeName;
					entity.Description = model.Description;
					entity.CreatedBy = model.CreatedBy;
					entity.DateCreated = DateTime.Now;
					entity.ModifiedBy = null;
					entity.DateModified = null;
					entity.IsActive = true;
					await _dbContext.EmploymentType.AddAsync(entity);
					await _dbContext.SaveChangesAsync();
					return (StatusCodes.Status200OK, "Successfully saved");
				}
				else
				{
					var entity = await _dbContext.EmploymentType.FirstOrDefaultAsync(x => x.EmploymentTypeId == model.EmploymentTypeId);
					entity.EmploymentTypeName = model.EmploymentTypeName;
					entity.Description = model.Description;
					entity.DateCreated = entity.DateCreated;
					entity.CreatedBy = entity.CreatedBy;
					entity.DateModified = DateTime.Now;
					entity.ModifiedBy = model.ModifiedBy;
					entity.IsActive = true;

					_dbContext.EmploymentType.Entry(entity).State = EntityState.Modified;
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

		public async Task<(int statuscode, string message)> Delete(EmploymentTypeViewModel model)
		{
			try
			{
				var entity = await _dbContext.EmploymentType.FirstOrDefaultAsync(x => x.EmploymentTypeId == model.EmploymentTypeId && x.IsActive == true);
				if (entity == null)
				{
					return (StatusCodes.Status406NotAcceptable, "Invalid employment type Id");
				}

				entity.IsActive = false;
				entity.ModifiedBy = model.ModifiedBy;
				entity.DateModified = DateTime.Now;
				_dbContext.EmploymentType.Entry(entity).State = EntityState.Modified;
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
