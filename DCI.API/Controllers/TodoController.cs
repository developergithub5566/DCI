using DCI.Data;
using DCI.Models.ViewModel;
using DCI.Repositories;
using DCI.Repositories.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DCI.API.Controllers
{
	[ApiController]
	[Route("api/[controller]")]

	public class TodoController : Controller
	{
		private readonly DCIdbContext _dcIdbContext;
		ITodoRepository _todoRepository;


		public TodoController(ITodoRepository todoRepository)
		{
			_todoRepository = todoRepository;
		}

		[HttpPost]
		[Route("GetTodoByApproverId")]
		public async Task<IActionResult> GetTodoByApproverId([FromBody] ApprovalHistoryViewModel model)
		{		
			return Ok(await _todoRepository.GetTodoByApproverId(model));
		}
	}
}
