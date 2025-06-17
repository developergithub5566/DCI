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
		
		ITodoRepository _todoRepository;


		public TodoController(ITodoRepository todoRepository)
		{
			_todoRepository = todoRepository;
		}

        //[HttpPost]
        //[Route("GetTodoByApproverId")]
        //public async Task<IActionResult> GetTodoByApproverId([FromBody] ApprovalHistoryViewModel model)
        //{		
        //	return Ok(await _todoRepository.GetTodoByApproverId(model));
        //}

        [HttpPost]
        [Route("Approval")]
        public async Task<IActionResult> Approval([FromBody] ApprovalHistoryViewModel model)
        {
             var result = await _todoRepository.Approval(model);
            return StatusCode(result.statuscode, result.message);
        }
        //[HttpPost]
        //[Route("GetAllTodo")]
        //public async Task<IActionResult> GetAllTodo([FromBody] DocumentViewModel model)
        //{	
        //	var result = await _todoRepository.GetAllTodo(model);
        //	return Ok(result);
        //}

        [HttpPost]
        [Route("GetAllTodo")]
        public async Task<IActionResult> GetAllTodo([FromBody] LeaveViewModel model)
        {
            var result = await _todoRepository.GetAllTodo(model);
            return Ok(result);
        }

        [HttpPost]
        [Route("GetApprovalLog")]
        public async Task<IActionResult> GetApprovalLog([FromBody] LeaveViewModel model)
        {
            var result = await _todoRepository.GetApprovalLog(model);
            return Ok(result);
        }

    }
}
