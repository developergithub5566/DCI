using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;


namespace DCI.Repositories
{
    public class ModulePageRepository : IModulePageRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        public ModulePageRepository(DCIdbContext context)
        {
            this._dbContext = context;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<ModulePage> GetModulePageById(int modulepageid)
        {
             return await _dbContext.ModulePage.FindAsync(modulepageid);
        }

		public async Task<bool> IsExistsModulePage(int moduleId)
		{
			return await _dbContext.ModulePage.AnyAsync(x => x.ModulePageId == moduleId && x.IsActive == true);
		}

		public async Task<IList<ModulePage>> GetAllModulePage()
        {
            return await _dbContext.ModulePage.AsNoTracking().Where(x => x.IsActive == true).ToListAsync();
        }

        public async Task<(int statuscode, string message)> Save(ModulePageViewModel model)
        {
            try
            {
                if (model.ModulePageId == 0)
                {
                    ModulePage entity = new ModulePage();
                    entity.ModuleName = model.ModuleName;
                    entity.Description = model.Description;
                    entity.CreatedBy = model.CreatedBy;
                    entity.DateCreated = DateTime.Now;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    await _dbContext.ModulePage.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();
					return (StatusCodes.Status200OK, "Successfully saved");
				}
                else
                {
                    var entity = await _dbContext.ModulePage.FirstOrDefaultAsync(x => x.ModulePageId == model.ModulePageId);
                    entity.ModuleName = model.ModuleName;
                    entity.Description = model.Description;
                    entity.DateCreated = entity.DateCreated;
                    entity.CreatedBy = entity.CreatedBy;
                    entity.DateModified = DateTime.Now;
                    entity.ModifiedBy = model.ModifiedBy;
                    entity.IsActive = true;

                    _dbContext.ModulePage.Entry(entity).State = EntityState.Modified;
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

        public async Task<(int statuscode, string message)> Delete(ModulePageViewModel model)
        {
            try
            {
				var entity = await _dbContext.ModulePage.FirstOrDefaultAsync(x => x.ModulePageId == model.ModulePageId);
				if (entity == null)
				{
					return (StatusCodes.Status406NotAcceptable, "Invalid module page Id");
				}

				entity.IsActive = false;
                entity.ModifiedBy = model.ModifiedBy;
				entity.DateModified = DateTime.Now;
				_dbContext.ModulePage.Entry(entity).State = EntityState.Modified;
				await _dbContext.SaveChangesAsync();
				return (StatusCodes.Status200OK, "Successfully deleted");
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
