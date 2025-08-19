using DCI.Data;
using DCI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace DCI.Repositories
{
    public class WfhRepository : IWfhRepository, IDisposable
    {
        private DCIdbContext _dbContext;
        public WfhRepository(DCIdbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Dispose()
        {
            _dbContext.Dispose();
        }

    }
}
