using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Repositories
{
    public class RequestHistoryRepository : IRequestHistoryRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        public RequestHistoryRepository(DCIdbContext context)
        {
            _dbContext = context;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }
        public async Task<RequestorHistory> GetRequestHistoryById(int reqId)
        {
            return await _dbContext.RequestorHistory.FindAsync(reqId) ?? new RequestorHistory();
        }

        public async Task Save(RequestorHistoryViewModel model)
        {
            try
            {
                RequestorHistory entity = new RequestorHistory();
                entity.RequestorHistoryId = model.RequestorHistoryId;
                entity.DocId = model.DocId;
                entity.RequestById = model.RequestById;
                entity.CreatedBy = model.RequestById;
                entity.DateCreated = DateTime.Now;
                entity.IsActive = true;
                await _dbContext.RequestorHistory.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
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
