using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
	public class DepartmentRepository : IDepartmentRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		public DepartmentRepository(DCIdbContext context)
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
		public async Task<DepartmentViewModel> GetDepartmentById(int deptId)
		{
			var context =   _dbContext.Department.AsQueryable();
			var userList = _dbContext.User.Where(x => x.IsActive).OrderBy(x => x.Lastname).ToList();

			var query = from dept in context						
						where dept.IsActive == true && dept.DepartmentId == deptId
						select new DepartmentViewModel
						{
							DepartmentId = dept.DepartmentId,
							DepartmentCode = dept.DepartmentCode,
							DepartmentName = dept.DepartmentName,
							Description = dept.Description,							
							CreatedBy = dept.CreatedBy,
							DateCreated = dept.DateCreated,						
							ApproverId = dept.ApproverId
						};
			var result = query.FirstOrDefault();
			if (result == null)
			{
				result = new DepartmentViewModel();
			}
			result.UserList = userList.Count() > 0 ? userList : null;

			return result;
		}

		public async Task<IList<DepartmentViewModel>> GetAllDepartment()
		{
			//return await _dbContext.Department.AsNoTracking().Where(x => x.IsActive == true).ToListAsync();
			try
			{
				var context = _dbContext.Department.AsQueryable().ToList();
				var userList = _dbContext.User.AsQueryable().ToList();			

				var query = from dept in context
							join user in userList on dept.CreatedBy equals user.UserId
							where dept.IsActive == true
							select new DepartmentViewModel
							{
								DepartmentId = dept.DepartmentId,
								DepartmentCode = dept.DepartmentCode,
								DepartmentName = dept.DepartmentName,
								Description = dept.Description,
								CreatedName = user.Email,
								CreatedBy = dept.CreatedBy,
								DateCreated = dept.DateCreated,							
								ApproverId = dept.ApproverId
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
		public async Task<bool> IsExistsDepartment(int DepartmentId)
		{
			return await _dbContext.Department.AnyAsync(x => x.DepartmentId == DepartmentId && x.IsActive == true);
		}

		public async Task<(int statuscode, string message)> Save(DepartmentViewModel model)
		{
			try
			{
				if (model.DepartmentId == 0)
				{
					Department entity = new Department();
					entity.DepartmentId = model.DepartmentId;
					entity.DepartmentCode = model.DepartmentCode;
					entity.DepartmentName = model.DepartmentName;
					entity.Description = model.Description;
				
					entity.ApproverId = model.ApproverId;
					entity.CreatedBy = model.CreatedBy;
					entity.DateCreated = DateTime.Now;
					entity.ModifiedBy = null;
					entity.DateModified = null;
					entity.IsActive = true;
					await _dbContext.Department.AddAsync(entity);
					await _dbContext.SaveChangesAsync();
					return (StatusCodes.Status200OK, "Successfully saved");
				}
				else
				{
					var entity = await _dbContext.Department.FirstOrDefaultAsync(x => x.DepartmentId == model.DepartmentId);
					entity.DepartmentCode = model.DepartmentCode;
					entity.DepartmentName = model.DepartmentName;
					entity.Description = model.Description;					
					entity.ApproverId = model.ApproverId;
					entity.DateCreated = entity.DateCreated;
					entity.CreatedBy = entity.CreatedBy;
					entity.DateModified = DateTime.Now;
					entity.ModifiedBy = model.ModifiedBy;
					entity.IsActive = true;

					_dbContext.Department.Entry(entity).State = EntityState.Modified;
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

		public async Task<(int statuscode, string message)> Delete(DepartmentViewModel model)
		{
			try
			{
				var entity = await _dbContext.Department.FirstOrDefaultAsync(x => x.DepartmentId == model.DepartmentId && x.IsActive == true);
				if (entity == null)
				{
					return (StatusCodes.Status406NotAcceptable, "Invalid Department Id");
				}

				entity.IsActive = false;
				entity.ModifiedBy = model.ModifiedBy;
				entity.DateModified = DateTime.Now;
				_dbContext.Department.Entry(entity).State = EntityState.Modified;
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


