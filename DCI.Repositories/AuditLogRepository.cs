using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using DCI.Core.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Channels;
using Serilog;

namespace DCI.Repositories
{
	public class AuditLogRepository : IAuditLogRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		public AuditLogRepository(DCIdbContext context)
		{
			this._dbContext = context;
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}
		public async Task<IList<AuditLogViewModel>> GetAuditLogById(AuditLogViewModel model)
		{
			try
			{
				var audit = await _dbContext.AuditLog.AsNoTracking().ToListAsync();
				var user = await _dbContext.User.AsNoTracking().ToListAsync();

				var query = from log in audit
							join u in user
							on Convert.ToInt32(string.IsNullOrEmpty(log.Username) ? (int?)null : Convert.ToInt32(log.Username)) equals u.UserId
							where log.Username == model.Username
							select new AuditLogViewModel
							{
								Id = log.Id,
								EntityName = log.EntityName,
								ActionType = log.ActionType,
								Username = DCI.Core.Common.Utilities.GetUsernameByEmail(u.Email),
								TimeStamp = log.TimeStamp,
								EntityId = log.EntityId,
								Changes = log.Changes,
								ChangesSerialized = GetChangesAsFormattedString(log.Changes),
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
		public async Task<IList<AuditLogViewModel>> GetAllAuditLogs()
		{
			try
			{
				var audit = await _dbContext.AuditLog.AsNoTracking().ToListAsync();
				var user = await _dbContext.User.AsNoTracking().ToListAsync();

				var query = from log in audit
							join u in user
								on Convert.ToInt32(string.IsNullOrEmpty(log.Username) ? (int?)null : Convert.ToInt32(log.Username)) equals u.UserId
							select new AuditLogViewModel
							{
								Id = log.Id,
								EntityName = log.EntityName,
								ActionType = log.ActionType,
								Username = DCI.Core.Common.Utilities.GetUsernameByEmail(u.Email),
								TimeStamp = log.TimeStamp,
								EntityId = log.EntityId,
								Changes = log.Changes,
								ChangesSerialized = GetChangesAsFormattedString(log.Changes),
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
		public string GetChangesAsFormattedString(Dictionary<string, object> changeData)
		{
			if (changeData == null || !changeData.Any())
				return string.Empty;

			var stringBuilder = new StringBuilder();
			foreach (var change in changeData)
			{
				string key = change.Key;
				if(key != "Password")
				{
					string value = change.Value != null ? change.Value.ToString() : "null";
					stringBuilder.AppendLine($"{key}: {value}");
				}
				
			}

			return stringBuilder.ToString();
		}
	}
}

