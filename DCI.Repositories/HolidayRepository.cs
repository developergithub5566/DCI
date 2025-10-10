using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
    public class HolidayRepository : IHolidayRepository, IDisposable
    {
        private DCIdbContext _dbContext;

        public HolidayRepository(DCIdbContext context)
        {
            this._dbContext = context;
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public async Task<HolidayViewModel> GetHolidayById(int holidayId)
        {
        //    var context = _dbContext.Holiday.AsQueryable();
            var userList = _dbContext.User.Where(x => x.IsActive).OrderBy(x => x.Fullname).ToList();

            var query = from hol in _dbContext.Holiday.AsQueryable()
                        join user in _dbContext.User.AsQueryable() on hol.CreatedBy equals user.UserId
                        where hol.IsActive == true && hol.HolidayId == holidayId
                        select new HolidayViewModel
                        {
                            HolidayId = hol.HolidayId,
                            HolidayDate = hol.HolidayDate,
                            HolidayName = hol.HolidayName,
                            Description = hol.Description,
                            HolidayType = hol.HolidayType,
                            CreatedBy = hol.CreatedBy,
                            CreatedName = user.Fullname, // + " " + user.Lastname,
                            DateCreated = hol.DateCreated,
                            DateModified = hol.DateModified,
                            ModifiedBy = hol.ModifiedBy,
                            IsActive = hol.IsActive
                        };
            var result = query.FirstOrDefault();
            if (result == null)
            {
                result = new HolidayViewModel();
            }
            
            return result;
        }

        public async Task<IList<HolidayViewModel>> GetAllHoliday()
        {
           
            try
            {
                var context = _dbContext.Holiday.AsQueryable().ToList();
                

                var query = from hol in _dbContext.Holiday.AsQueryable()
                            join user in _dbContext.User.AsQueryable() on hol.CreatedBy equals user.UserId
                            where hol.IsActive == true
                            select new HolidayViewModel
                            {
                                HolidayId = hol.HolidayId,
                                HolidayDate = hol.HolidayDate,
                                HolidayName = hol.HolidayName,
                                HolidayTypeName = hol.HolidayType == 1 ? "Regular" : "Special",
                                Description = hol.Description,
                                HolidayType = hol.HolidayType,
                                CreatedBy = hol.CreatedBy,
                                CreatedName = user.Fullname, // + " " + user.Lastname,
                                DateCreated = hol.DateCreated,
                                DateModified = hol.DateModified,
                                ModifiedBy = hol.ModifiedBy,
                                IsActive = hol.IsActive
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

        public async Task<bool> IsExistsHoliday(int holidayId)
        {
            return await _dbContext.Holiday.AnyAsync(x => x.HolidayId == holidayId && x.IsActive == true);
        }

        public async Task<(int statuscode, string message)> Save(HolidayViewModel model)
        {
            try
            {
                if (model.HolidayId == 0)
                {
                    Holiday entity = new Holiday();
                    entity.HolidayId = model.HolidayId;
                    entity.HolidayDate = model.HolidayDate;
                    entity.HolidayName = model.HolidayName;
                    entity.Description = model.Description;
                    entity.HolidayType = model.HolidayType;
                    entity.CreatedBy = model.CreatedBy;
                    entity.DateCreated = DateTime.Now;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    await _dbContext.Holiday.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();
                    return (StatusCodes.Status200OK, "Successfully saved");
                }
                else
                {
                    var entity = await _dbContext.Holiday.FirstOrDefaultAsync(x => x.HolidayId == model.HolidayId);
                    entity.HolidayId = model.HolidayId;
                    entity.HolidayDate = model.HolidayDate;
                    entity.HolidayName = model.HolidayName;
                    entity.Description = model.Description;
                    entity.HolidayType = model.HolidayType;
                    entity.DateCreated = entity.DateCreated;
                    entity.CreatedBy = entity.CreatedBy;
                    entity.DateModified = DateTime.Now;
                    entity.ModifiedBy = model.ModifiedBy;
                    entity.IsActive = true;

                    _dbContext.Holiday.Entry(entity).State = EntityState.Modified;
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

        public async Task<(int statuscode, string message)> Delete(HolidayViewModel model)
        {
            try
            {
                var entity = await _dbContext.Holiday.FirstOrDefaultAsync(x => x.HolidayId == model.HolidayId && x.IsActive == true);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid Holiday Id");
                }

                entity.IsActive = false;
                entity.ModifiedBy = model.ModifiedBy;
                entity.DateModified = DateTime.Now;
                _dbContext.Holiday.Entry(entity).State = EntityState.Modified;
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
