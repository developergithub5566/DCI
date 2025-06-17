using DCI.Core.Common;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
    public class AnnouncementRepository : IAnnouncementRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        public AnnouncementRepository(DCIdbContext context)
        {
            this._dbContext = context;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<AnnouncementViewModel> GetAnnouncementById(int announceId)
        {
            var context = _dbContext.Announcement.AsQueryable();
            //var userList = _dbContext.User.Where(x => x.IsActive).OrderBy(x => x.Lastname).ToList();

            var query = from announce in context
                        where announce.IsActive == true //&& announce.Status == (int)EnumStatus.Active
                        select new AnnouncementViewModel
                        {
                            AnnouncementId = announce.AnnouncementId,
                            Title = announce.Title,
                            Date = announce.Date,
                            Details = announce.Details,
                            Status = announce.Status,
                            CreatedBy = announce.CreatedBy,
                            DateCreated = announce.DateCreated,
                            DateModified = announce.DateModified,
                            ModifiedBy = announce.ModifiedBy,
                            IsActive = announce.IsActive
                        };
            var result = query.FirstOrDefault();
            if (result == null)
            {
                result = new AnnouncementViewModel();
            }          

            return result;
        }

        public async Task<IList<AnnouncementViewModel>> GetAllAnnouncement()
        {
         
            try
            {
                var context = _dbContext.Announcement.AsQueryable().ToList();
               // var userList = _dbContext.User.AsQueryable().ToList();

                var query = from announce in context
                           // join user in userList on dept.CreatedBy equals user.UserId
                            where announce.IsActive == true
                            select new AnnouncementViewModel
                            {
                                AnnouncementId = announce.AnnouncementId,
                                Title = announce.Title,
                                Date = announce.Date,
                                Details = announce.Details,
                                Status = announce.Status,
                                CreatedBy = announce.CreatedBy,
                                DateCreated = announce.DateCreated,
                                DateModified = announce.DateModified,
                                ModifiedBy = announce.ModifiedBy,
                                IsActive = announce.IsActive
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

        public async Task<(int statuscode, string message)> Save(AnnouncementViewModel model)
        {
            try
            {
                if (model.AnnouncementId == 0)
                {
                    Announcement entity = new Announcement();
                    entity.AnnouncementId = model.AnnouncementId;
                    entity.Title = model.Title;
                    entity.Date = model.Date;
                    entity.Details = model.Details;
                    entity.Status = model.Status;                  
                    entity.CreatedBy = model.CreatedBy;
                    entity.DateCreated = DateTime.Now;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    await _dbContext.Announcement.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();
                    return (StatusCodes.Status200OK, "Successfully saved");
                }
                else
                {
                    var entity = await _dbContext.Announcement.FirstOrDefaultAsync(x => x.AnnouncementId == model.AnnouncementId);
                    entity.AnnouncementId = model.AnnouncementId;
                    entity.Title = model.Title;
                    entity.Date = model.Date;
                    entity.Details = model.Details;
                    entity.Status = model.Status;
                    entity.DateCreated = entity.DateCreated;
                    entity.CreatedBy = entity.CreatedBy;
                    entity.DateModified = DateTime.Now;
                    entity.ModifiedBy = model.ModifiedBy;
                    entity.IsActive = true;
                    _dbContext.Announcement.Entry(entity).State = EntityState.Modified;
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

        public async Task<(int statuscode, string message)> Delete(AnnouncementViewModel model)
        {
            try
            {
                var entity = await _dbContext.Announcement.FirstOrDefaultAsync(x => x.AnnouncementId == model.AnnouncementId && x.IsActive == true);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid Announcement Id");
                }

                entity.IsActive = false;
                entity.ModifiedBy = model.ModifiedBy;
                entity.DateModified = DateTime.Now;
                _dbContext.Announcement.Entry(entity).State = EntityState.Modified;
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
