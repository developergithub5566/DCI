using DCI.Models.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCI.Trigger
{
    public interface IEmailRepository : IDisposable
    {
        Task SendEmailBiometricsNotification(BiometricViewModel model);
    }
}
