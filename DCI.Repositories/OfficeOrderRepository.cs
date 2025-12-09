using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
    public class OfficeOrderRepository : IOfficeOrderRepository , IDisposable
    {
        private DCIdbContext _dbContext;
        public OfficeOrderRepository(DCIdbContext context)
        {
            this._dbContext = context;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
        public async Task<OfficeOrderViewModel> GetOfficeOrderById(int ooId)
        {        
            var query = from oo in _dbContext.OfficeOrder.AsNoTracking()
                        join user in _dbContext.User.AsNoTracking() on oo.CreatedBy equals user.UserId
                        where oo.IsActive == true && oo.OfficeOrderId == ooId
                        select new OfficeOrderViewModel
                        {
                            OfficeOrderId = oo.OfficeOrderId,
                            OrderName = oo.OrderName,
                            OrderDate = oo.OrderDate,
                            Description = oo.Description,
                            Filename = oo.Filename,
                            FilePath = oo.FilePath,
                            CreatedBy = oo.CreatedBy,
                            CreatedName = user.Email, 
                            DateCreated = oo.DateCreated,
                            DateModified = oo.DateModified,
                            ModifiedBy = oo.ModifiedBy,
                            IsActive = oo.IsActive
                        };
            var result = query.FirstOrDefault();
            if (result == null)
            {
                result = new OfficeOrderViewModel();
            }

            return result;
        }

        public async Task<IList<OfficeOrderViewModel>> GetAllOfficeOrder()
        {

            try
            {    
                var query = from oo in _dbContext.OfficeOrder.AsNoTracking()
                            join user in _dbContext.User.AsNoTracking() on oo.CreatedBy equals user.UserId
                            where oo.IsActive == true
                            select new OfficeOrderViewModel
                            {
                                OfficeOrderId = oo.OfficeOrderId,
                                OrderName = oo.OrderName,
                                OrderDate = oo.OrderDate,
                                Description = oo.Description,
                                Filename = oo.Filename,
                                FilePath = oo.FilePath,
                                CreatedBy = oo.CreatedBy,
                                CreatedName = user.Email,
                                DateCreated = oo.DateCreated,
                                DateModified = oo.DateModified,
                                ModifiedBy = oo.ModifiedBy,
                                IsActive = oo.IsActive
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

        public async Task<bool> IsExistsOfficeOrder(int ooId)
        {
            return await _dbContext.OfficeOrder.AnyAsync(x => x.OfficeOrderId == ooId && x.IsActive == true);
        }

        public async Task<(int statuscode, string message)> Save(OfficeOrderViewModel model)
        {
            try
            {
                if (model.OfficeOrderId == 0)
                {
                    OfficeOrder entity = new OfficeOrder();
                    entity.OfficeOrderId = model.OfficeOrderId;
                    entity.OrderName = model.OrderName;
                    entity.OrderDate = model.OrderDate;
                    entity.Description = model.Description;
                    entity.Filename = model.Filename;
                    entity.FilePath = model.FilePath;
                    entity.CreatedBy = model.CreatedBy;
                    entity.DateCreated = DateTime.Now;
                    entity.ModifiedBy = null;
                    entity.DateModified = null;
                    entity.IsActive = true;
                    await _dbContext.OfficeOrder.AddAsync(entity);
                    await _dbContext.SaveChangesAsync();
                    return (StatusCodes.Status200OK, "Successfully saved");
                }
                else
                {
                    var entity = await _dbContext.OfficeOrder.FirstOrDefaultAsync(x => x.OfficeOrderId == model.OfficeOrderId);                 
                    entity.OrderName = model.OrderName;
                    entity.OrderDate = model.OrderDate;
                    entity.Description = model.Description;
                    entity.Filename = model.Filename;
                    entity.FilePath = model.FilePath;
                    entity.DateCreated = entity.DateCreated;
                    entity.CreatedBy = entity.CreatedBy;
                    entity.DateModified = DateTime.Now;
                    entity.ModifiedBy = model.ModifiedBy;
                    entity.IsActive = true;

                    _dbContext.OfficeOrder.Entry(entity).State = EntityState.Modified;
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

        public async Task<(int statuscode, string message)> Delete(OfficeOrderViewModel model)
        {
            try
            {
                var entity = await _dbContext.OfficeOrder.FirstOrDefaultAsync(x => x.OfficeOrderId == model.OfficeOrderId && x.IsActive == true);
                if (entity == null)
                {
                    return (StatusCodes.Status406NotAcceptable, "Invalid Office Order Id");
                }

                entity.IsActive = false;
                entity.ModifiedBy = model.ModifiedBy;
                entity.DateModified = DateTime.Now;
                _dbContext.OfficeOrder.Entry(entity).State = EntityState.Modified;
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
