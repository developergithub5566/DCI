using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
    public class ModuleInRoleRepository : IModuleInRoleRepository, IDisposable
    {
        private readonly DCIdbContext _dbContext;

        public ModuleInRoleRepository(DCIdbContext context)
        {
            this._dbContext = context;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
        public async Task<ModuleInRole> GetModuleInRoleById(int moduleinroleid)
        {
            return await _dbContext.ModuleInRole.FindAsync(moduleinroleid);
        }	
		public async Task<bool> IsExistsModuleInRole(int moduleinroleid)
        {
            return await _dbContext.ModuleInRole.AnyAsync(x => x.ModuleInRoleId == moduleinroleid && x.IsActive == true);
        }
        public async Task<IList<ModuleInRole>> GetAllModuleInRole()
        {
            return await _dbContext.ModuleInRole.AsNoTracking().Where(x => x.IsActive == true).ToListAsync();
        }
		public async Task<IList<int>> GetModuleInRoleByRoleId(int roleId)
		{
			var result =   _dbContext.ModuleInRole.AsNoTracking().Where(x => x.IsActive == true && x.RoleId == roleId);
            return result.Select(x => x.ModulePageId).ToList();
		}
		public async Task<(int statuscode, string message)> Save(ModuleInRoleViewModel model)
        {
            try
            {
                if (model.ModuleInRoleId == 0)
                {
                    ModuleInRole entity = new ModuleInRole();
                    entity.ModulePageId = model.ModulePageId;
                    entity.RoleId = model.RoleId;
                    entity.View = model.View;
                    entity.Add = model.Add;
                    entity.Update = model.Update;
                    entity.Delete = model.Delete;
                    entity.Import = model.Import;
                    entity.Export = model.Export;
                    entity.CreatedBy = model.CreatedBy;
                    entity.DateCreated = DateTime.Now;
                    entity.ModifiedBy = model.ModifiedBy;
                    entity.DateModified = DateTime.Now;
                    entity.IsActive = true;
                    await _dbContext.ModuleInRole.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();
                    return (StatusCodes.Status200OK, "Successfully saved");
                }
                else
                {
                    var entity = await _dbContext.ModuleInRole.FirstOrDefaultAsync(x => x.ModuleInRoleId == model.ModuleInRoleId);
                    entity.ModulePageId = model.ModulePageId;
                    entity.RoleId = model.RoleId;
                    entity.Add = model.Add;
                    entity.Update = model.Update;
                    entity.Delete = model.Delete;
                    entity.Import = model.Import;
                    entity.Export = model.Export;
                    entity.DateCreated = entity.DateCreated;
                    entity.CreatedBy = entity.CreatedBy;
                    entity.DateModified = DateTime.Now;
                    entity.ModifiedBy = model.ModifiedBy;
                    entity.IsActive = true;
                    _dbContext.ModuleInRole.Entry(entity).State = EntityState.Modified;
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
        public async Task<(int statuscode, string message)> Delete(int id)
        {
            try
            {
                var entity = await _dbContext.ModuleInRole.FirstOrDefaultAsync(x => x.ModuleInRoleId == id);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid Module In role Id");
                }
                entity.IsActive = false;
                entity.ModifiedBy = 0;
                entity.DateModified = DateTime.Now;
                _dbContext.ModuleInRole.Entry(entity).State = EntityState.Modified;
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
        public async Task DeletebyRoleId(int roleId)
        {
            try
            {
                var entitiesToDelete = await _dbContext.ModuleInRole.Where(x => x.RoleId == roleId).ToListAsync();

                if (entitiesToDelete.Any())
                {
                    _dbContext.ModuleInRole.RemoveRange(entitiesToDelete);
                    await _dbContext.SaveChangesAsync();
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
        }
    }
}
