using DCI.Models.Entities;
using DCI.Models.ViewModel;

namespace DCI.Repositories.Interface
{
	public interface IAuditLogRepository : IDisposable
	{
		Task<IList<AuditLogViewModel>> GetAuditLogById(AuditLogViewModel model);
		Task<IList<AuditLogViewModel>> GetAllAuditLogs();
	}
}
