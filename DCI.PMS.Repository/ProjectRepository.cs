using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.PMS.Models.Entities;
using DCI.PMS.Models.ViewModel;
using DCI.PMS.Repository.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.PMS.Repository
{
    public class ProjectRepository : IProjectRepository, IDisposable
    {
        private readonly DCIdbContext _dbContext;
        private readonly PMSdbContext _pmsdbContext;
        public ProjectRepository(DCIdbContext context, PMSdbContext pmsContext)
        {  
            this._dbContext = context;
            this._pmsdbContext = pmsContext;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }     

        public async Task<ProjectViewModel> GetProjectId(int projectId)
        {
            var users = _dbContext.User.AsNoTracking().ToList();
            var projects = _pmsdbContext.Project.AsNoTracking().ToList();

            var query = projects
                 .Join(
                     users,
                     p => p.CreatedBy,
                     u => u.UserId,
                     (p, u) => new ProjectViewModel
                     {
                         ProjectNo = p.ProjectNo,
                         IsActive = p.IsActive
                     }
                 );
            var result = query.FirstOrDefault();
            if (result == null)
            {
                result = new ProjectViewModel();
            }

            return result;
        }

        public async Task<IList<ProjectViewModel>> GetAllProject()
        {

            try
            {               
                var query = from proj in _pmsdbContext.Project.AsNoTracking()
                         //   join user in _dbContext.User.AsNoTracking() on hol.CreatedBy equals user.UserId
                            where proj.IsActive == true
                            select new ProjectViewModel
                            {
                                ProjectCreationId = proj.ProjectCreationId,
                                ProjectName = proj.ProjectName,
               
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

        public async Task<bool> IsExistsProject(int projectId)
        {
            return await _pmsdbContext.Project.AnyAsync(x => x.ProjectCreationId == projectId && x.IsActive == true);
        }

        //public async Task<(int statuscode, string message)> Save(ProjectViewModel model)
        //{
        //    try
        //    {
        //        if (model.ProjectCreationId == 0)
        //        {
        //            Project entity = new Project();
        //            entity.ProjectCreationId = model.ProjectCreationId;
        //            entity.ClientId = model.ClientId;
        //            entity.ProjectNo = model.ProjectNo;
        //            entity.ProjectName = model.ProjectName;
        //            entity.NOADate = model.NOADate;
        //            entity.NTPDate = model.NTPDate;
        //            entity.MOADate = model.MOADate;
        //            entity.ProjectDuration = model.ProjectDuration;
        //            entity.ModeOfPayment = model.ModeOfPayment;
        //            entity.CreatedBy = model.CreatedBy;
        //            entity.DateCreated = DateTime.Now;
        //            entity.ModifiedBy = null;
        //            entity.DateModified = null;
        //            entity.IsActive = true;
        //            await _pmsdbContext.Project.AddAsync(entity);
        //            await _pmsdbContext.SaveChangesAsync();
        //            return (StatusCodes.Status200OK, "Successfully saved");
        //        }
        //        else
        //        {
        //            var entity = await _pmsdbContext.Holiday.FirstOrDefaultAsync(x => x.HolidayId == model.HolidayId);
        //            entity.HolidayId = model.HolidayId;
        //            entity.HolidayDate = model.HolidayDate;
        //            entity.HolidayName = model.HolidayName;
        //            entity.Description = model.Description;
        //            entity.HolidayType = model.HolidayType;
        //            entity.DateCreated = entity.DateCreated;
        //            entity.CreatedBy = entity.CreatedBy;
        //            entity.DateModified = DateTime.Now;
        //            entity.ModifiedBy = model.ModifiedBy;
        //            entity.IsActive = true;

        //            _dbContext.Holiday.Entry(entity).State = EntityState.Modified;
        //            await _pmsdbContext.SaveChangesAsync();
        //            return (StatusCodes.Status200OK, "Successfully updated");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.ToString());
        //        return (StatusCodes.Status406NotAcceptable, ex.ToString());
        //    }
        //    finally
        //    {
        //        Log.CloseAndFlush();
        //    }
        //}

        //public async Task<(int statuscode, string message)> Delete(HolidayViewModel model)
        //{
        //    try
        //    {
        //        var entity = await _pmsdbContext.Holiday.FirstOrDefaultAsync(x => x.HolidayId == model.HolidayId && x.IsActive == true);
        //        if (entity == null)
        //        {
        //            return (StatusCodes.Status406NotAcceptable, "Invalid Holiday Id");
        //        }

        //        entity.IsActive = false;
        //        entity.ModifiedBy = model.ModifiedBy;
        //        entity.DateModified = DateTime.Now;
        //        _pmsdbContext.Holiday.Entry(entity).State = EntityState.Modified;
        //        await _pmsdbContext.SaveChangesAsync();
        //        return (StatusCodes.Status200OK, "Successfully deleted");
        //    }
        //    catch (Exception ex)
        //    {
        //        Log.Error(ex.ToString());
        //        return (StatusCodes.Status400BadRequest, ex.ToString());
        //    }
        //    finally
        //    {
        //        Log.CloseAndFlush();
        //    }
        //}




    }
}
