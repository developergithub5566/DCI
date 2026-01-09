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
            _pmsdbContext.Dispose();
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
                var users = await _dbContext.User
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .Select(u => new
                                        {
                                            u.UserId,
                                            u.Fullname
                                        })
                                        .ToListAsync();

                var projects = await _pmsdbContext.Project
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .Select(p => new
                                        {
                                            p.ProjectCreationId,
                                            p.ProjectNo,
                                            p.ProjectName,
                                            p.NOADate,
                                            p.NTPDate,
                                            p.MOADate,
                                            p.ProjectDuration,
                                            p.ProjectCost,
                                            p.ModeOfPayment,
                                            p.CreatedBy,
                                            p.IsActive
                                        })
                                        .ToListAsync();

                var result = from p in projects
                             join u in users on p.CreatedBy equals u.UserId
                             select new ProjectViewModel
                             {
                                 ProjectCreationId = p.ProjectCreationId,
                                 ProjectNo = p.ProjectNo,
                                 ProjectName = p.ProjectName,
                                 NOADate = p.NOADate,
                                 NTPDate = p.NTPDate,
                                 MOADate = p.MOADate,
                                 ProjectDuration = p.ProjectDuration,
                                 ProjectCost = p.ProjectCost,
                                 ModeOfPayment = p.ModeOfPayment,
                                 IsActive = p.IsActive,
                                 CreatedName = u.Fullname
                             };

                return result.ToList();
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

        public async Task<ProjectViewModel> GetProjectById(ProjectViewModel model)
        {

            try
            {
                var users = await _dbContext.User
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .Select(u => new
                                        {
                                            u.UserId,
                                            u.Fullname
                                        })
                                        .ToListAsync();

                var projects = await _pmsdbContext.Project
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .Select(p => new
                                        {
                                            p.ProjectCreationId,
                                            p.ProjectNo,
                                            p.ProjectName,
                                            p.ClientId,
                                            p.NOADate,
                                            p.NTPDate,
                                            p.MOADate,
                                            p.ProjectDuration,
                                            p.ProjectCost,
                                            p.ModeOfPayment,
                                            p.CreatedBy,
                                            p.IsActive
                                        })
                                        .ToListAsync();

                var result = (from p in projects
                              join u in users on p.CreatedBy equals u.UserId
                              join c in _pmsdbContext.Client on p.ClientId equals c.ClientId
                              where p.ProjectCreationId == model.ProjectCreationId
                              select new ProjectViewModel
                              {
                                  ProjectCreationId = p.ProjectCreationId,
                                  ProjectNo = p.ProjectNo,
                                  ProjectName = p.ProjectName,
                                  ClientId = p.ClientId,
                                  ClientName = c.ClientName,
                                  NOADate = p.NOADate,
                                  NTPDate = p.NTPDate,
                                  MOADate = p.MOADate,
                                  ProjectDuration = p.ProjectDuration,
                                  ProjectCost = p.ProjectCost,
                                  ModeOfPayment = p.ModeOfPayment,
                                  IsActive = p.IsActive,
                                  CreatedName = u.Fullname
                              }).FirstOrDefault();

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
            return null;
        }


        public async Task<bool> IsExistsProject(int projectId)
        {
            return await _pmsdbContext.Project.AnyAsync(x => x.ProjectCreationId == projectId && x.IsActive == true);
        }

        public async Task<(int statuscode, string message)> SaveProject(ProjectViewModel model)
        {
            try
            {
                if (model.ProjectCreationId == 0)
                {
                    Project entity = new Project();
                    entity.ProjectCreationId = model.ProjectCreationId;
                    entity.ClientId = model.ClientId;
                    entity.ProjectNo = model.ProjectNo;
                    entity.ProjectName = model.ProjectName;
                    entity.NOADate = model.NOADate;
                    entity.NTPDate = model.NTPDate;
                    entity.MOADate = model.MOADate;
                    entity.ProjectDuration = model.ProjectDuration;
                    entity.ProjectCost = model.ProjectCost;
                    entity.ModeOfPayment = model.ModeOfPayment;
                    entity.CreatedBy = model.CreatedBy;
                    entity.DateCreated = DateTime.Now;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    await _pmsdbContext.Project.AddAsync(entity);
                    await _pmsdbContext.SaveChangesAsync();
                    return (StatusCodes.Status200OK, "Successfully saved");
                }
                else
                {
                    var entity = await _pmsdbContext.Project.FirstOrDefaultAsync(x => x.ProjectCreationId == model.ProjectCreationId);
                    entity.ProjectCreationId = model.ProjectCreationId;
                    entity.ClientId = model.ClientId;
                    entity.ProjectNo = model.ProjectNo;
                    entity.ProjectName = model.ProjectName;
                    entity.NOADate = model.NOADate;
                    entity.MOADate = model.MOADate;
                    entity.ProjectDuration = model.ProjectDuration;
                    entity.ProjectCost = model.ProjectCost;
                    entity.ModeOfPayment = model.ModeOfPayment;
                    //entity.DateCreated = entity.DateCreated;
                    //entity.CreatedBy = entity.CreatedBy;
                    entity.DateModified = DateTime.Now;
                    entity.ModifiedBy = model.ModifiedBy;
                    entity.IsActive = true;

                    _pmsdbContext.Project.Entry(entity).State = EntityState.Modified;
                    await _pmsdbContext.SaveChangesAsync();
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

        public async Task<ProjectViewModel> GetMilestoneByProjectId(ProjectViewModel model)
        {

            try
            {
                var users = await _dbContext.User
                                        .AsNoTracking()
                                        .Where(p => p.IsActive)
                                        .Select(u => new
                                        {
                                            u.UserId,
                                            u.Fullname
                                        })
                                        .ToListAsync();

                var milestone = _pmsdbContext.Milestone
                                        .AsNoTracking()
                                        .Where(p => p.IsActive).ToList();


                var result = (from m in milestone
                              join u in users on m.CreatedBy equals u.UserId
                              where m.ProjectCreationId == model.ProjectCreationId
                              select new MilestoneViewModel
                              {
                                  MileStoneId = m.MileStoneId,
                                  ProjectCreationId = m.ProjectCreationId,
                                  MilestoneName = m.MilestoneName,
                                  Percentage = m.Percentage,
                                  TargetCompletedDate = m.TargetCompletedDate,
                                  ActualCompletionDate = m.ActualCompletionDate,
                                  PaymentStatus = m.PaymentStatus,
                                  Status = m.Status,
                                  DateCreated = m.DateCreated,
                                  CreatedBy = m.CreatedBy,
                                  DateModified = m.DateModified,
                                  ModifiedBy = m.ModifiedBy,
                                  IsActive = m.IsActive,
                              }).ToList();

                model.MilestoneList = result;

                return model;

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

        public async Task<(int statuscode, string message)> SaveMilestone(MilestoneViewModel model)
        {
            try
            {
                if (model.ProjectCreationId == 0)
                {
                    Milestone entity = new Milestone();
                    entity.MilestoneName = model.MilestoneName;
                    entity.Percentage = model.Percentage;
                    entity.TargetCompletedDate = model.TargetCompletedDate;
                    entity.ActualCompletionDate = model.ActualCompletionDate;
                    entity.PaymentStatus = model.PaymentStatus;
                    entity.Status = model.Status;                             
                    entity.CreatedBy = model.CreatedBy;
                    entity.DateCreated = DateTime.Now;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    await _pmsdbContext.Milestone.AddAsync(entity);
                    await _pmsdbContext.SaveChangesAsync();
                    return (StatusCodes.Status200OK, "Successfully saved");
                }
                else
                {
                    var entity = await _pmsdbContext.Milestone.FirstOrDefaultAsync(x => x.MileStoneId == model.MileStoneId);
                    entity.MilestoneName = model.MilestoneName;
                    entity.Percentage = model.Percentage;
                    entity.TargetCompletedDate = model.TargetCompletedDate;
                    entity.ActualCompletionDate = model.ActualCompletionDate;
                    entity.PaymentStatus = model.PaymentStatus;
                    entity.Status = model.Status;    
                    entity.DateModified = DateTime.Now;
                    entity.ModifiedBy = model.ModifiedBy;
                    entity.IsActive = true;

                    _pmsdbContext.Milestone.Entry(entity).State = EntityState.Modified;
                    await _pmsdbContext.SaveChangesAsync();
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


        public async Task<(int statuscode, string message)> Delete(ProjectViewModel model)
        {
            try
            {
                var entity = await _pmsdbContext.Project.FirstOrDefaultAsync(x => x.ProjectCreationId == model.ProjectCreationId && x.IsActive == true);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid Project Id");
                }

                entity.IsActive = false;
                entity.ModifiedBy = model.ModifiedBy;
                entity.DateModified = DateTime.Now;
                _pmsdbContext.Project.Entry(entity).State = EntityState.Modified;
                await _pmsdbContext.SaveChangesAsync();
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
