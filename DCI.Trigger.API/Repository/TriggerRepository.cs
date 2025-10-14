using DCI.Models.ViewModel;
using DCI.Trigger.API.DBContext;
using DCI.Trigger.API.Model;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Trigger.API.Repository
{
    public class TriggerRepository : ITriggerRepository, IDisposable
    {
        private AppDbContext _appDbContext;

        public TriggerRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;

        }
        public void Dispose()
        {
            _appDbContext.Dispose();
        }

        public async Task<IList<OutboxQueue>> GetAllAttendanceLog()
        {
            return await _appDbContext.OutboxQueue.AsNoTracking().Where(x => x.Status == "Pending").Take(10).ToListAsync();
        }


        public async Task UpdateOutBoxQueueStatus(OutboxMessageViewModel model)
        {
            try
            {
                var entities = await _appDbContext.OutboxQueue.FirstOrDefaultAsync(x => x.Id == model.Id);
                entities.Status = model.Status;
                entities.RetryCount = model.RetryCount;
                entities.LastError = model.LastError;
                _appDbContext.OutboxQueue.Entry(entities).State = EntityState.Modified;
                await _appDbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                // return model;               
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

    }
}
