using DCI.Data;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
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
                var context = _dbContext.Position.AsQueryable().ToList();
                var userList = _dbContext.User.AsQueryable().ToList();

                var query = from post in context
                            join user in userList on post.CreatedBy equals user.UserId
                            where post.IsActive == true
                            select new PositionViewModel
                            {
                                PositionId = post.PositionId,
                                PositionCode = post.PositionCode,
                                PositionName = post.PositionName,
                                Description = post.Description,
                                CreatedName = user.Lastname,
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
    }
}
