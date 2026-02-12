using DCI.PMS.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.PMS.Repository.Interface
{
    public interface IPMSEmailRepository : IDisposable
    {
        Task SendProjectCreation(ProjectViewModel model);
        Task SendProjectCompleted(ProjectViewModel model);
    }
}
