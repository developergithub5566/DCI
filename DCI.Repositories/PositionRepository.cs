using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
    public class PositionRepository : IPositionRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        public PositionRepository(DCIdbContext context)
        {
            this._dbContext = context;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
        public async Task<IList<PositionViewModel>> GetAllPosition()
        {
            try
            {
              //  var context = _dbContext.Position.AsNoTracking().ToList();
               // var userList = _dbContext.User.AsNoTracking().ToList();

                var query = from post in _dbContext.Position.AsNoTracking().ToList()
                            join user in _dbContext.User.AsNoTracking().ToList() on post.CreatedBy equals user.UserId
                            where post.IsActive == true
                            select new PositionViewModel
                            {
                                PositionId = post.PositionId,
                                PositionCode = post.PositionCode,
                                PositionName = post.PositionName,
                                Description = post.Description,
                                CreatedName =user.Fullname, // + " " + user.Lastname,
                                CreatedBy = post.CreatedBy,
                                DateCreated = post.DateCreated
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

        public async Task<PositionViewModel> GetPositionById(int postId)
        {
            var context = _dbContext.Position.AsQueryable();
            var userList = _dbContext.User.Where(x => x.IsActive).OrderBy(x => x.Fullname).ToList();

            var query = from post in context
                        where post.IsActive == true && post.PositionId == postId
                        select new PositionViewModel
                        {
                            PositionId = post.PositionId,
                            PositionCode = post.PositionCode,
                            PositionName = post.PositionName,
                            Description = post.Description,
                            CreatedBy = post.CreatedBy,
                            DateCreated = post.DateCreated,                        
                        };
            var result = query.FirstOrDefault();
            if (result == null)
            {
                result = new PositionViewModel();
            }
           

            return result;
        }

        public async Task<bool> IsExistsDepartment(int positionId)
        {
            return await _dbContext.Position.AnyAsync(x => x.PositionId == positionId && x.IsActive == true);
        }


        public async Task<bool> IsExistsPositionCode(string postCode)
        {
            return await _dbContext.Position.AnyAsync(x => x.PositionCode == postCode && x.IsActive == true);
        }

        public async Task<(int statuscode, string message)> Save(PositionViewModel model)
        {
            try
            {
                if (model.PositionId == 0)
                {
                    Position entity = new Position();
                    entity.PositionId = model.PositionId;
                    entity.PositionName = model.PositionName;
                    entity.PositionCode = model.PositionCode;
                    entity.Description = model.Description;
                    
                    entity.CreatedBy = model.CreatedBy;
                    entity.DateCreated = DateTime.Now;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    await _dbContext.Position.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();
                    return (StatusCodes.Status200OK, "Successfully saved");
                }
                else
                {
                    var entity = await _dbContext.Position.FirstOrDefaultAsync(x => x.PositionId == model.PositionId);
                    entity.PositionCode = model.PositionCode;
                    entity.PositionName = model.PositionName;
                    entity.Description = model.Description;
             
                    entity.DateCreated = entity.DateCreated;
                    entity.CreatedBy = entity.CreatedBy;
                    entity.DateModified = DateTime.Now;
                    entity.ModifiedBy = model.ModifiedBy;
                    entity.IsActive = true;

                    _dbContext.Position.Entry(entity).State = EntityState.Modified;
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

        public async Task<(int statuscode, string message)> Delete(PositionViewModel model)
        {
            try
            {
                var entity = await _dbContext.Position.FirstOrDefaultAsync(x => x.PositionId == model.PositionId && x.IsActive == true);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid Position Id");
                }

                entity.IsActive = false;
                entity.ModifiedBy = model.ModifiedBy;
                entity.DateModified = DateTime.Now;
                _dbContext.Position.Entry(entity).State = EntityState.Modified;
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
