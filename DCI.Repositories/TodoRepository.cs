using DCI.Core.Common;
using DCI.Data;
using DCI.Models.Entities;
using DCI.Models.ViewModel;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace DCI.Repositories
{
	public class TodoRepository : ITodoRepository, IDisposable
	{
		private DCIdbContext _dbContext;
		private IEmailRepository _emailRepository;


		public TodoRepository(DCIdbContext context, IEmailRepository emailRepository)
		{
			this._dbContext = context;
			this._emailRepository = emailRepository;
			
		}
		public void Dispose()
		{
			_dbContext.Dispose();
		}






		
	}
}
