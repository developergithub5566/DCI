using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;


namespace DCI.Repositories
{
	public class SectionRepository : ISectionRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		public SectionRepository(DCIdbContext context)
		{
			this._dbContext = context;

		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}

		public async Task<SectionViewModel> GetSectiontById(int sectionId)
		{

			SectionViewModel model = new SectionViewModel();
			try
			{
				var departmentList = _dbContext.Department.AsQueryable().ToList();

				var context = _dbContext.Section.Where(x => x.IsActive == true && x.SectionId == sectionId);

				var result = await context.Select(sec => new SectionViewModel
				{
					SectionId = sec.SectionId,
					SectionCode = sec.SectionCode,
					SectionName = sec.SectionName,
					Description = sec.Description,
					CreatedBy = sec.CreatedBy,
					DateCreated = sec.DateCreated,
					DepartmentId = sec.DepartmentId,
					DepartmentList = departmentList
				}).FirstOrDefaultAsync();

				if (result == null)
				{
					SectionViewModel sec = new SectionViewModel();
					sec.DepartmentList = departmentList.Count() > 0 ? departmentList : null;
					return sec;
				}
				return result;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
			return model;
		}
		public async Task<SectionViewModel> GetSectionByDepartmentId(SectionViewModel model)
		{
			SectionViewModel sectionmodel = new SectionViewModel();
			
			try
			{
				var sectionList =  _dbContext.Section.AsQueryable().ToList();
				var context =  sectionList.Where(x => x.IsActive == true && x.DepartmentId == model.DepartmentId);

				//var result = await context.Select(sec => new SectionViewModel
				//{
				//	SectionId = sec.SectionId,
				//	SectionCode = sec.SectionCode,
				//	SectionName = sec.SectionName,
				//	Description = sec.Description,
				//	CreatedBy = sec.CreatedBy,
				//	DateCreated = sec.DateCreated,
				//	DepartmentId = sec.DepartmentId,
				//	DepartmentList = null,
				//}).ToListAsync();

				if (context.Count() == 0)
				{
					//SectionViewModel sec = new SectionViewModel();	
					sectionmodel.SectionList = new List<Section>();//new SectionViewModel();
					return sectionmodel;
				}
				sectionmodel.SectionList = context.ToList();
				return sectionmodel;
			}
			catch (Exception ex)
			{
				Log.Error(ex.ToString());
			}
			finally
			{
				Log.CloseAndFlush();
			}
			return sectionmodel;
		}
		public async Task<IList<SectionViewModel>> GetAllSection()
		{

			var context = _dbContext.Section.AsQueryable().ToList();
			var userList = _dbContext.User.AsQueryable().ToList();
			var deptList = _dbContext.Department.AsQueryable().ToList();

			var query = from sec in context
						join user in userList on sec.CreatedBy equals user.UserId
						join dept in deptList on sec.DepartmentId equals dept.DepartmentId
						where sec.IsActive == true
						select new SectionViewModel
						{
							SectionId = sec.SectionId,
							SectionCode = sec.SectionCode,
							SectionName = sec.SectionName,
							Description = sec.Description,
							CreatedName = user.Email,
							CreatedBy = sec.CreatedBy,
							DateCreated = sec.DateCreated,
							DepartmentName = dept.DepartmentName,
						};

			return query.ToList();
		}
		public async Task<bool> IsExistsSection(int sectionId)
		{
			return await _dbContext.Section.AnyAsync(x => x.SectionId == sectionId && x.IsActive == true);
		}

		public async Task<(int statuscode, string message)> Save(SectionViewModel model)
		{
			try
			{
				if (model.SectionId == 0)
				{
					Section entity = new Section();
					entity.SectionId = model.SectionId;
					entity.SectionCode = model.SectionCode;
					entity.SectionName = model.SectionName;
					entity.Description = model.Description;
					entity.DepartmentId = model.DepartmentId;
					entity.CreatedBy = model.CreatedBy;
					entity.DateCreated = DateTime.Now;
					entity.ModifiedBy = null;
					entity.DateModified = null;
					entity.IsActive = true;
					await _dbContext.Section.AddAsync(entity);
					await _dbContext.SaveChangesAsync();
					return (StatusCodes.Status200OK, "Successfully saved");
				}
				else
				{
					var entity = await _dbContext.Section.FirstOrDefaultAsync(x => x.SectionId == model.SectionId);
					entity.SectionCode = model.SectionCode;
					entity.SectionName = model.SectionName;
					entity.Description = model.Description;
					entity.DepartmentId = model.DepartmentId;
					entity.DateCreated = entity.DateCreated;
					entity.CreatedBy = entity.CreatedBy;
					entity.DateModified = DateTime.Now;
					entity.ModifiedBy = model.ModifiedBy;
					entity.IsActive = true;

					_dbContext.Section.Entry(entity).State = EntityState.Modified;
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

		public async Task<(int statuscode, string message)> Delete(SectionViewModel model)
		{
			try
			{
				var entity = await _dbContext.Section.FirstOrDefaultAsync(x => x.SectionId == model.SectionId && x.IsActive == true);
				if (entity == null)
				{
					return (StatusCodes.Status406NotAcceptable, "Invalid Section Id");
				}

				entity.IsActive = false;
				entity.ModifiedBy = model.ModifiedBy;
				entity.DateModified = DateTime.Now;
				_dbContext.Section.Entry(entity).State = EntityState.Modified;
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
