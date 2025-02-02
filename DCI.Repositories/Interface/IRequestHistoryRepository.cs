using DCI.Models.Entities;
using DCI.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Repositories.Interface
{
    public interface IRequestHistoryRepository : IDisposable
    {
        Task<RequestorHistory> GetRequestHistoryById(int reqId);
        Task Save(RequestorHistoryViewModel model);
    }
}
