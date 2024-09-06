using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
	public class RoleRepository : IRoleRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		public RoleRepository(DCIdbContext context)
		{
			this._dbContext = context;
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}
		public async Task<Role> GetRoleById(int roleId)
		{
			return await _dbContext.Role.FindAsync(roleId);
		}
		public async Task<bool> IsExistsRole(int roleId)
		{
			return await _dbContext.Role.AnyAsync(x => x.RoleId == roleId && x.IsActive == true);
		}

		public async Task<IList<Role>> GetAllRoles()
		{
			return await _dbContext.Role.AsNoTracking().Where(x => x.IsActive == true).ToListAsync();
		}
		public async Task<(int statuscode, string message, Role entity)> Save(RoleViewModel model)
		{
			try
			{
				if (model.RoleId == 0)
				{
					Role entity = new Role();
					entity.RoleId = model.RoleId;
					entity.RoleName = model.RoleName;
					entity.Description = model.Description;
					entity.CreatedBy = model.CreatedBy;
					entity.DateCreated = DateTime.Now;
					entity.ModifiedBy = null;
					entity.DateModified = null;
					entity.IsActive = true;
					await _dbContext.Role.AddAsync(entity);
					await _dbContext.SaveChangesAsync();
					return (StatusCodes.Status200OK, "Successfully saved", entity);
				}
				else
				{
					var entity = await _dbContext.Role.FirstOrDefaultAsync(x => x.RoleId == model.RoleId);
					entity.RoleName = model.RoleName;
					entity.Description = model.Description;
					entity.DateCreated = entity.DateCreated;
					entity.CreatedBy = entity.CreatedBy;
					entity.DateModified = DateTime.Now;
					entity.ModifiedBy = model.ModifiedBy;
					entity.IsActive = true;

					_dbContext.Role.Entry(entity).State = EntityState.Modified;
					await _dbContext.SaveChangesAsync();
					return (StatusCodes.Status200OK, "Successfully updated", entity);
				}			
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
				return (StatusCodes.Status406NotAcceptable, ex.ToString(), null );
			}
			finally
			{
				Log.CloseAndFlush();
			}
		}

		public async Task<(int statuscode, string message)> Delete(RoleViewModel model)
		{
			try
			{
				var entity = await _dbContext.Role.FirstOrDefaultAsync(x => x.RoleId == model.RoleId && x.IsActive == true);
				if (entity == null)
				{
					return (StatusCodes.Status406NotAcceptable, "Invalid role Id");
				}			

				entity.IsActive = false;
				entity.ModifiedBy = model.ModifiedBy;
				entity.DateModified = DateTime.Now;
				_dbContext.Role.Entry(entity).State = EntityState.Modified;
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
